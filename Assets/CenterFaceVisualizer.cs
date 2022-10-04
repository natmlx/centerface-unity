/* 
*   CenterFace
*   Copyright (c) 2022 NatML Inc. All Right Reserved.
*/

namespace NatML.Examples.Visualizers {

    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// </summary>
    [RequireComponent(typeof(RawImage), typeof(AspectRatioFitter))]
    public sealed class CenterFaceVisualizer : MonoBehaviour {

        #region --Inspector--
        public Image faceRect;
        #endregion


        #region --Client API--
        /// <summary>
        /// Detection source image.
        /// </summary>
        public Texture2D image {
            get => rawImage.texture as Texture2D;
            set {
                rawImage.texture = value;
                aspectFitter.aspectRatio = (float)value.width / value.height;
            }
        }

        /// <summary>
        /// Render a set of detected faces.
        /// </summary>
        /// <param name="faces">Faces to render.</param>
        public void Render (params Rect[] faces) {
            // Delete current
            foreach (var rect in currentRects)
                GameObject.Destroy(rect.gameObject);
            currentRects.Clear();
            // Render rects
            var imageRect = new Rect(0, 0, image.width, image.height);
            foreach (var face in faces) {
                var faceUI = Instantiate(faceRect, transform);
                faceUI.gameObject.SetActive(true);
                Render(faceUI, face);
                currentRects.Add(faceUI);
            }
        }
        #endregion


        #region --Operations--
        private RawImage rawImage;
        private AspectRatioFitter aspectFitter;
        private readonly List<Image> currentRects = new List<Image>();

        void Awake () {
            rawImage = GetComponent<RawImage>();
            aspectFitter = GetComponent<AspectRatioFitter>();
        }

        void Render (Image prefab, Rect faceRect) {
            var rectTransform = prefab.transform as RectTransform;
            var imageTransform = rawImage.transform as RectTransform;
            rectTransform.anchorMin = 0.5f * Vector2.one;
            rectTransform.anchorMax = 0.5f * Vector2.one;
            rectTransform.pivot = Vector2.zero;
            rectTransform.sizeDelta = Vector2.Scale(imageTransform.rect.size, faceRect.size);
            rectTransform.anchoredPosition = Rect.NormalizedToPoint(imageTransform.rect, faceRect.position);
        }
        #endregion
    }
}