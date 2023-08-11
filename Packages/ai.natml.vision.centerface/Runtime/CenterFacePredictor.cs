/* 
*   CenterFace
*   Copyright (c) 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityEngine;
    using NatML.Features;
    using NatML.Internal;
    using NatML.Types;

    /// <summary>
    /// CenterFace face predictor.
    /// This predictor accepts an image feature and produces a list of face rectangles.
    /// Face rectangles are always specified in normalized coordinates.
    /// </summary>
    public sealed class CenterFacePredictor : IMLPredictor<Rect[]> {

        #region --Client API--
        /// <summary>
        /// CenterFace predictor tag on NatML Hub.
        /// </summary>
        public const string Tag = "@natsuite/centerface";

        /// <summary>
        /// Detect faces in an image.
        /// </summary>
        /// <param name="inputs">Input image.</param>
        /// <returns>Detected faces.</returns>
        public Rect[] Predict (params MLFeature[] inputs) {
            // Check
            if (inputs.Length != 1)
                throw new ArgumentException(@"CenterFace predictor expects a single feature", nameof(inputs));
            // Check type
            var input = inputs[0];
            var imageType = MLImageType.FromType(input.type);
            var imageFeature = input as MLImageFeature;
            if (!imageType)
                throw new ArgumentException(@"CenterFace predictor expects an an array or image feature", nameof(inputs));
            // Apply normalization
            if (imageFeature != null) {
                (imageFeature.mean, imageFeature.std) = model.normalization;
                imageFeature.aspectMode = model.aspectMode;
            }
            // Predict
            var inputType = model.inputs[0] as MLImageType;
            using var inputFeature = (input as IMLEdgeFeature).Create(inputType);
            using var outputFeatures = model.Predict(inputFeature);
            // Marshal
            var heatmap = new MLArrayFeature<float>(outputFeatures[0]); // (1,1,H,W)
            var scale = new MLArrayFeature<float>(outputFeatures[1]);   // (1,2,H,W)
            var offset = new MLArrayFeature<float>(outputFeatures[2]);  // (1,2,H,W)
            var (widthInv, heightInv) = (1f / inputType.width, 1f / inputType.height);
            var candidateBoxes = new List<Rect>();
            var candidateScores = new List<float>();
            for (int j = 0, jlen = heatmap.shape[2]; j < jlen; ++j)
                for (int i = 0, ilen = heatmap.shape[3]; i < ilen; ++i) {
                    // Check
                    var score = heatmap[0, 0, j, i];
                    if (score < minScore)
                        continue;
                    // Extract
                    var sy = Mathf.Exp(scale[0,0,j,i]) * 4;
                    var sx = Mathf.Exp(scale[0,1,j,i]) * 4;
                    var oy = offset[0,0,j,i];
                    var ox = offset[0,1,j,i];
                    var x1 = Mathf.Clamp((i + ox + 0.5f) * 4f - sx / 2f, 0, inputType.width);
                    var y1 = Mathf.Clamp((j + oy + 0.5f) * 4f - sy / 2f, 0, inputType.height);
                    var x2 = Mathf.Min(x1 + sx, inputType.width);
                    var y2 = Mathf.Min(y1 + sy, inputType.height);
                    var rawBox = Rect.MinMaxRect(x1 * widthInv, 1f - y2 * heightInv, x2 * widthInv, 1f - y1 * heightInv);
                    var box = imageFeature?.TransformRect(rawBox, inputType) ?? rawBox;
                    // Save
                    candidateBoxes.Add(box);
                    candidateScores.Add(score);
                }
            var keepIdx = MLImageFeature.NonMaxSuppression(candidateBoxes, candidateScores, maxIoU);
            var result = keepIdx.Select(i => candidateBoxes[i]).ToArray();
            // Return
            return result;
        }

        /// <summary>
        /// Dispose the model and release resources.
        /// </summary>
        public void Dispose () => model.Dispose();

        /// <summary>
        /// Create the CenterFace predictor.
        /// </summary>
        /// <param name="minScore">Minimum candidate score.</param>
        /// <param name="maxIoU">Maximum intersection-over-union score for overlap removal.</param>
        /// <param name="configuration"></param>
        /// <param name="accessKey"></param>
        /// <returns>CenterFace predictor.</returns>
        public static async Task<CenterFacePredictor> Create (
            float minScore = 0.5f,
            float maxIoU = 0.5f,
            MLEdgeModel.Configuration configuration = null,
            string accessKey = null
        ) {
            var model = await MLEdgeModel.Create(Tag, configuration, accessKey);
            var predictor = new CenterFacePredictor(model, minScore, maxIoU);
            return predictor;
        }
        #endregion


        #region --Operations--
        private readonly MLEdgeModel model;
        private readonly float minScore;
        private readonly float maxIoU;

        private CenterFacePredictor (MLEdgeModel model, float minScore = 0.5f, float maxIoU = 0.5f) {
            this.model = model as MLEdgeModel;
            this.minScore = minScore;
            this.maxIoU = maxIoU;
        }
        #endregion
    }
}