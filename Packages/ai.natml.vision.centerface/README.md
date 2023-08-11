# CenterFace
[CenterFace](https://github.com/Star-Clouds/CenterFace) face detection.

## Installing CenterFace
Add the following items to your Unity project's `Packages/manifest.json`:
```json
{
  "scopedRegistries": [
    {
      "name": "NatML",
      "url": "https://registry.npmjs.com",
      "scopes": ["ai.natml"]
    }
  ],
  "dependencies": {
    "ai.natml.vision.centerface": "1.0.1"
  }
}
```

## Detecting Faces in an Image
First, create the CenterFace predictor:
```csharp
// Create the CenterFace predictor
var predictor = await CenterFacePredictor.Create();
```

Then detect faces in the image:
```csharp
// Create image feature
Texture2D image = ...;
var imageFeature = new MLImageFeature(image); // This also accepts a `Color32[]` or `byte[]`
(imageFeature.mean, imageFeature.std) = modelData.normalization;
imageFeature.aspectMode = modelData.aspectMode;
// Detect faces
Rect[] faces = predictor.Predict(imageFeature);
```

___

## Requirements
- Unity 2022.3+

## Quick Tips
- Discover more ML models on [NatML Hub](https://hub.natml.ai).
- See the [NatML documentation](https://docs.natml.ai/unity).
- Join the [NatML community on Discord](https://natml.ai/community).
- Contact us at [hi@natml.ai](mailto:hi@natml.ai).

Thank you very much!