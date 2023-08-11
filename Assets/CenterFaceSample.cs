/* 
*   CenterFace
*   Copyright (c) 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using System.Threading.Tasks;
    using UnityEngine;
    using NatML.Vision;
    using VideoKit;
    using Visualizers;

    public sealed class CenterFaceSample : MonoBehaviour {

        [Header(@"Camera Manager")]
        public VideoKitCameraManager cameraManager;

        [Header(@"UI")]
        public CenterFaceVisualizer visualizer;
    
        private CenterFacePredictor predictor;

        async void Start () {
            // Create the predictor
            predictor = await CenterFacePredictor.Create();
            // Listen for camera frames
            cameraManager.OnCameraFrame.AddListener(OnCameraFrame);
        }

        void OnCameraFrame (CameraFrame frame) {
            // Predict faces
            var faces = predictor.Predict(frame);
            // Visualize
            visualizer.Render(faces);
        }

        void OnDisable () {
            // Stop listening for frames
            cameraManager.OnCameraFrame.RemoveListener(OnCameraFrame);
            // Dispose the model
            predictor?.Dispose();
        }
    }
}