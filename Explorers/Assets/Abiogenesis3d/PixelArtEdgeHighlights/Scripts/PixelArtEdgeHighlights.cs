using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Abiogenesis3d
{
    public enum PixelArtEdgeHighlightsDebugEffect
    {
        None, // 0
        Highlights,
        DepthMap,
        NormalsMap,
        Sobel,
        DepthDiff,
        NormalsDiff,
        ConcaveNormalsDiff,
    }

    [Serializable]
    public class PixelArtEdgeHighlightsCameraInfo
    {
        public Camera cam;
        [HideInInspector] public Camera lastCam;
        [HideInInspector] public MirrorOnRenderImageOpaque mirrorOnRenderImageOpaque;
    }

    [ExecuteInEditMode]
    public class PixelArtEdgeHighlights : MonoBehaviour
    {
        [Range(0, 1)] public float convexHighlight = 0.5f;
        [Range(0, 1)] public float outlineShadow = 0.5f;
        [Range(0, 1)] public float concaveShadow = 1;
        public PixelArtEdgeHighlightsDebugEffect debugEffect = PixelArtEdgeHighlightsDebugEffect.None;
        // public Vector4 test1;
        [Header("Experimental")]
        [Range(0.001f, 0.03f)] public float depthSensitivity = 0.002f;
        public bool clampCameraFarClipPlane = true;

        [Header("To ignore a camera add PixelArtEdgeHighlightsIgnore component to it.")]
        public bool autoDetectCameras = true;
        public List<PixelArtEdgeHighlightsCameraInfo> cameraInfos = new List<PixelArtEdgeHighlightsCameraInfo>();

    #if UNITY_PIPELINE_URP
    #else
        Material material;
        public Shader shader;
    #endif

        // in URP effect is a renderer feature and not a blit
    #if UNITY_PIPELINE_URP
        public List<PixelArtEdgeHighlightsFeature> rendererFeatures;
    #else
        void UpdateMaterialProperties()
        {
            var _ConvexHighlight = convexHighlight;
            var _OutlineShadow = outlineShadow;

            if (QualitySettings.activeColorSpace == ColorSpace.Gamma)
            {
                // TODO: this is an approximation, what is precise way to convert from gamma?
                _ConvexHighlight *= 0.25f;
                _OutlineShadow *= 0.75f;
            }

            material.SetFloat("_ConvexHighlight", _ConvexHighlight);
            material.SetFloat("_OutlineShadow", _OutlineShadow);
            material.SetFloat("_ConcaveShadow", concaveShadow);
            material.SetFloat("_DepthSensitivity", depthSensitivity);
            material.SetInt("_DebugEffect", (int)debugEffect);
            // material.SetVector("_Test1", test1);
        }
    #endif

        float GetCamDepthOr0(Camera cam)
        {
            if (cam != null) return cam.depth;
            return 0;
        }

        Type GetIgnoredType()
        {
            return typeof(PixelArtEdgeHighlightsIgnore);
        }

        // TODO: export this into helper file
        void AutoDetectCameras()
        {
            var allCameras = FindObjectsOfType<Camera>();

            foreach(var cam in allCameras)
            {
                var ignoreTag = cam.GetComponent(GetIgnoredType());
                var camInfo = cameraInfos.FirstOrDefault(c => c.cam == cam);

                if (camInfo == null)
                {
                    if (ignoreTag == null)
                    {
                        camInfo = new PixelArtEdgeHighlightsCameraInfo {cam = cam};
                        cameraInfos = cameraInfos.Concat(new[] {camInfo}).ToList();
                    }
                }
                else
                {
                    if (ignoreTag != null)
                        cameraInfos = cameraInfos.Where(c => c.cam != cam).ToList();
                }
            }
            cameraInfos = cameraInfos.OrderBy(c => GetCamDepthOr0(c.cam)).ToList();
        }

        void CheckForInstances()
        {
            var existingInstances = FindObjectsOfType<PixelArtEdgeHighlights>();
            if (existingInstances.Length > 1)
            {
                Debug.Log($"PixelArtEdgeHighlights: There should only be one active instance in the scene. Deactivating: {name}");
                enabled = false;
                return;
            }
        }
        void OnEnable()
        {
            CheckForInstances();
        }

        void Update()
        {
            if (autoDetectCameras) AutoDetectCameras();

            cameraInfos.ForEach(camInfo => {
                if (!camInfo.cam)
                {
                    // no camera, cleanup mirror
                    if (camInfo.mirrorOnRenderImageOpaque) DestroyImmediate(camInfo.mirrorOnRenderImageOpaque);
                    return;
                }

                // NOTE: this handles camera change from inspector
                if (camInfo.cam != camInfo.lastCam)
                {
                    if (camInfo.mirrorOnRenderImageOpaque) DestroyImmediate(camInfo.mirrorOnRenderImageOpaque);
                }

                HandleCam(camInfo);

            #if UNITY_PIPELINE_URP
                if (camInfo.mirrorOnRenderImageOpaque) DestroyImmediate(camInfo.mirrorOnRenderImageOpaque);
            #else
                EnsureMirrorOnRenderImage(camInfo);
                camInfo.mirrorOnRenderImageOpaque.renderImageCallback -= RenderImage;
                camInfo.mirrorOnRenderImageOpaque.renderImageCallback += RenderImage;
            #endif

                camInfo.lastCam = camInfo.cam;
            });

        #if UNITY_PIPELINE_URP
        #if UNITY_EDITOR
            // TODO: call less frequently
            var urpAssets = PipelineAssets.GetUrpAssets();
            foreach(var urpAsset in urpAssets)
                SetupRenderFeatures.SetDownsamplingToNone(urpAsset);
            rendererFeatures = SetupRenderFeatures.AddAndGetRendererFeatures<PixelArtEdgeHighlightsFeature>(urpAssets);
        #endif
            if (rendererFeatures.Count == 0)
            {
                Debug.Log("Renderer Features could not be added.");
                return;
            }

            foreach (var feature in rendererFeatures)
            {
                if (!feature) continue;

                // NOTE: this fixes issue of missing shader when switching scenes
                if (!feature.material) feature.Create();

                var isDirty = false;
                if (feature.settings.convexHighlight != convexHighlight) isDirty = true;
                if (feature.settings.outlineShadow != outlineShadow) isDirty = true;
                if (feature.settings.concaveShadow != concaveShadow) isDirty = true;
                if (feature.settings.depthSensitivity != depthSensitivity) isDirty = true;
                if (feature.settings.debugEffect != (int)debugEffect) isDirty = true;

                feature.settings.convexHighlight = convexHighlight;
                feature.settings.outlineShadow = outlineShadow;
                feature.settings.concaveShadow = concaveShadow;
                feature.settings.depthSensitivity = depthSensitivity;
                feature.settings.debugEffect = (int)debugEffect;

                if (isDirty) feature.UpdateMaterialProperties();

                feature.SetActive(true);
            }
        #else
        #endif
        }

        void HandleCam(PixelArtEdgeHighlightsCameraInfo camInfo)
        {
            camInfo.cam.depthTextureMode = DepthTextureMode.Depth | DepthTextureMode.DepthNormals;
            camInfo.cam.allowMSAA = false;

            if (clampCameraFarClipPlane)
            {
                var maxFarClipPlane = 100;
                if (camInfo.cam.farClipPlane > maxFarClipPlane)
                {
                    camInfo.cam.farClipPlane = maxFarClipPlane;
                    Debug.Log($"PixelArtEdgeHighlights: Clamping camera.farClipPlane to {maxFarClipPlane}. Shader values are adjusted for that depthmap precision. To go beyond please uncheck the clampCameraFarClipPlane option and adjust depthSensitivity. With larger farClipPlane the depthmap becomes less precise.");
                }
            }
        }

    #if UNITY_PIPELINE_URP
    #else
        void EnsureMirrorOnRenderImage(PixelArtEdgeHighlightsCameraInfo camInfo)
        {
            if (camInfo.mirrorOnRenderImageOpaque) return;

            var mirrorOnRenderImageOpaques = camInfo.cam.GetComponents<MirrorOnRenderImageOpaque>();
            foreach (var mirror in mirrorOnRenderImageOpaques)
            {
                // NOTE: cleanup previous leftover components
                if (mirror.target == "PixelArtEdgeHighlights") DestroyImmediate(mirror);
            }

            camInfo.mirrorOnRenderImageOpaque = camInfo.cam.gameObject.AddComponent<MirrorOnRenderImageOpaque>();
            camInfo.mirrorOnRenderImageOpaque.target = "PixelArtEdgeHighlights";
            camInfo.mirrorOnRenderImageOpaque.targetGO = gameObject;
        }

        void RenderImage(RenderTexture source, RenderTexture destination)
        {
            if (!shader) shader = Shader.Find("Abiogenesis3d/PixelArtEdgeHighlights");
            if (!material) material = new Material(shader);

            UpdateMaterialProperties();

            material.SetTexture("_MainTex", source);
            Graphics.Blit(source, destination, material);
        }
    #endif

        void OnDisable()
        {
        #if UNITY_PIPELINE_URP
            foreach (var feature in rendererFeatures)
            {
                if (!feature) continue;
                feature.SetActive(false);
            }
        #else
            cameraInfos.ForEach(camInfo => {
                if (!camInfo.cam) return;

                // TODO: solve this better
                // this cleans up components if the user changes cameraInfos manually
                var mirrorOnRenderImageOpaques = camInfo.cam.GetComponents<MirrorOnRenderImageOpaque>();
                foreach (var mirror in mirrorOnRenderImageOpaques)
                {
                    if (mirror.target == "PixelArtEdgeHighlights")
                        DestroyImmediate(mirror);
                }
                if (camInfo.mirrorOnRenderImageOpaque) DestroyImmediate(camInfo.mirrorOnRenderImageOpaque);
            });
        #endif
        }
    }
}
