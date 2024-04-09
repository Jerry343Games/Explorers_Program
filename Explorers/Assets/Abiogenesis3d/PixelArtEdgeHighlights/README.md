# UPixelator - Pixel Art Edge Highlights

Thank you for purchasing Pixel Art Edge Highlights! ❤️  
If you like the asset I would appreciate your review!  

**AssetStore Reviews bug:**  
If you only see this message "Please download this asset to leave a review":  
Click on one of the N star blue rows and the "Write a Review" button will show up.  

## Contact
If you have any questions or feedback, please contact me at reslav.hollos@gmail.com.  
You can also join the [Discord server](https://discord.gg/uFEDDpS8ad)!  

## How to update!
### v1 -> v1.3.0
- Please first delete `Assets/Abiogenesis3d` and `Assets/Editor/Abiogenesis3d` folders
- [UPixelator] install minimum v2.1.0 version

## Quick start
- Drag and drop `Prefabs/PixelArtEdgeHighlights` into scene.  
- Or open the scene under the `Example/Scenes` folder.
- Otherwise go to the [Setup](#setup) section of this readme.  

## Description
Pixel Art Edge Highlights is a post process effect that draws thin edges based on the depth and normal textures screen output.

It is intended to be used with the [Unity Pixelator](https://assetstore.unity.com/packages/slug/243562) asset that
pixelates whole 3d scenes in realtime and handles camera movement smoothing and pixel creep reduction.

(You can also use it as a standalone effect to add detail on the edges that would otherwise be
indistinguishable like flat colors where normals are equal, see screenshots with the effect off).

## Edge Types
- `Convex Highlights` lightens outward edges
- `Outline Shadow` darkens objects outline
- `Concave Shadow` darkens inward edges

## Supported Pipelines
- Built-in ✓
- URP ✓

## Tested builds
- Unity 2021.3 (Builtin, URP 12): Windows, WebGL  
- Unity 2022.3 (Builtin, URP 14): Windows, WebGL  

## Please note
- This asset automatically sets some camera and render pipeline settings, if you're worried please backup your work.
- Shader values are adjusted for Camera farClipPlane being 100, with higher values depthmap precision is reduced.
- There should be a single active instance of the script in the project and additional instances will deactivate themselves.

# Setup
- Scene:
  - Drag the `PixelArtEdgeHighlights` prefab into scene and should immediatelly see the effect
  - Drag the `PixelArtEdgeHighlights - Canvas` prefab into scene to get the runtime UI controls
- Camera:
  - If you're using scene pixelation set `Projection: Orthographic`
  - Have the camera distanced about 10 meters from the target (internal shader values for depth map are adjusted for it)
- Game window
  - Change `Free Aspect` to a fixed resolution eg. `1920x1080`
  - Aspect ratio of width to height should not be close to or over `2:1`
  - Set `Scale: 1` to have pixels render 1 on 1

## Demo scene
- Demo scene is located under the `Example` folder

## Recommended usage
- Lower values from `0.2` to `0.5` are suggested to get nice colored edges just enough to make the highlights differentiate geometry.

## Excluding objects from the effect
1. [URP] Render objects you want to exclude on a specific layer with a `Render Objects` feature:
  - Add a new layer called `IgnoreEffect` and put your objects on it
  - Add a new `Render Objects` renderer feature to the URP renderer asset and set:
    - `Event: BeforeRenderingTransparents`
    - `Queue: Opaque`
    - `LayerMask: IgnoreEffect`
    - `Override Mode: None`

2. The effect executes before transparents so materials with render queue 3000+ are excluded.
- On a material you want to exclude check for `Custom Render Queue` and if it's hidden try with inspector Debug mode
  - To open the inspector Debug mode click the tripple dots icon next to the Lock icon in the upper right corner of any inspector window
- [URP] If setting it to 3000 clamps it back to 2000 it is a bug in Unity that was fixed several times but is again back.
  - However you can still set it to 3000 by finding `Saved Properties > Floats > _QueueOffset` and setting that to 1000.
- [Multi cameras] If an opaque material becomes transparent it could mean that it is not outputing alpha values correctly.
  - To change that you can hardcode the alpha value to 1 (eg. `return float4(some_output.rgb, 1);`).
- [Shader Graph] `Advanced Options > Queue Control: UserOverride` then `Render Queue` will appear
- [Builtin: Standard] Set Rendering Mode to `Fade` (otherwise you wont be able to set the queue higher)

## Known Issues
- [Unity 2022; URP] There is a `fullscreenMesh` obsolete warning, I will address it when I find replacement code that works
- [Builtin] Particle shader does not output normals to _CameraDepthNormalsTexture so it does not hide the effect behind it [Forum](https://forum.unity.com/threads/1540778)

## In progress/research
- [URP] Exclude cameras with ignore flag from renderer feature effect
