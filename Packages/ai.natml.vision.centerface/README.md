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
    "ai.natml.vision.centerface": "1.0.0"
  }
}
```

## Detecting Faces in an Image
First, create the CenterFace predictor:
```csharp
// Fetch the model data from NatML
var modelData = await MLModelData.FromHub("@natsuite/centerface");
// Deserialize the model
var model = modelData.Deserialize();
// Create the CenterFace predictor
var predictor = new CenterFacePredictor(model);
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
- Unity 2020.3+
- [NatML 1.0.11+](https://github.com/natmlx/NatML)

## Quick Tips
- Discover more ML models on [NatML Hub](https://hub.natml.ai).
- See the [NatML documentation](https://docs.natml.ai/unity).
- Join the [NatML community on Discord](https://discord.gg/y5vwgXkz2f).
- Discuss [NatML on Unity Forums](https://forum.unity.com/threads/open-beta-natml-machine-learning-runtime.1109339/).
- Contact us at [hi@natml.ai](mailto:hi@natml.ai).

Thank you very much!