#if UNITY_PIPELINE_URP
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Abiogenesis3d
{
    [Serializable]
    public class PixelArtEdgeHighlightsSettings
    {
        [Header("These values are being set by PixelArtEdgeHighlights script!")]
        [Range(0, 1)] public float convexHighlight = 0.5f;
        [Range(0, 1)] public float outlineShadow = 0.5f;
        [Range(0, 1)] public float concaveShadow = 1;
        [Range(0.001f, 0.03f)] public float depthSensitivity = 0.002f;
        [Range(0, 10)] public int debugEffect;
        [HideInInspector] public Vector4 test1;
        public Shader shader; // public needed for builds or Shader.Find cannot find the file
    }

    public class PixelArtEdgeHighlightsFeature : ScriptableRendererFeature
    {
        public PixelArtEdgeHighlightsSettings settings;
        [HideInInspector] public Material material;
        PixelArtEdgeHighlightsPass pixelArtEdgeHighlightsPass;

        public override void Create()
        {
            if (settings == null) return;

            if (settings.shader == null) settings.shader = Shader.Find("Abiogenesis3d/PixelArtEdgeHighlights");
            if (settings.shader == null)
            {
                // throw new Exception("Shader not found");
                return;
            }

            material = new Material(settings.shader);

            pixelArtEdgeHighlightsPass = new PixelArtEdgeHighlightsPass(material, settings);
            UpdateMaterialProperties();

            // TODO: this is not sufficient for build, ShaderPreprocessor script is also needed
            var unityPipelineURP = GlobalKeyword.Create("UNITY_PIPELINE_URP");
            Shader.EnableKeyword(unityPipelineURP);
        }

        public void UpdateMaterialProperties()
        {
            // NOTE: this happens when this is an inactive quality level renderer
            if (pixelArtEdgeHighlightsPass == null) return;
            pixelArtEdgeHighlightsPass.UpdateMaterialProperties();
        }

        public void SetIsMatDirty(bool value)
        {
            pixelArtEdgeHighlightsPass.isMatDirty = value;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var passInput = ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal;
            pixelArtEdgeHighlightsPass.ConfigureInput(passInput);

            pixelArtEdgeHighlightsPass.SetRenderer(renderer);
            renderer.EnqueuePass(pixelArtEdgeHighlightsPass);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(material);
        }
    }

    class PixelArtEdgeHighlightsPass : ScriptableRenderPass
    {
        string profilerTag = "PixelArtEdgeHighlights_Profiler";

        ScriptableRenderer renderer;
        PixelArtEdgeHighlightsSettings settings;

        Material material;
        public bool isMatDirty;

        public void UpdateMaterialProperties()
        {
            if (!material) return;

            material.SetFloat("_ConvexHighlight", settings.convexHighlight);
            material.SetFloat("_OutlineShadow", settings.outlineShadow);
            material.SetFloat("_ConcaveShadow", settings.concaveShadow);
            material.SetFloat("_DepthSensitivity", settings.depthSensitivity);
            material.SetInt("_DebugEffect", settings.debugEffect);
            // material.SetVector("_Test1", settings.test1);
        }

        public PixelArtEdgeHighlightsPass(Material _material, PixelArtEdgeHighlightsSettings _settings)
        {
            material = _material;
            settings = _settings;
            renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
        }

        public void SetRenderer(ScriptableRenderer r)
        {
            renderer = r;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd, ref renderingData);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!material) return;

            if (renderingData.cameraData.isSceneViewCamera) return;
            if (renderingData.cameraData.isPreviewCamera) return;

            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, new ProfilingSampler(profilerTag)))
            {
                if (isMatDirty)
                {
                    isMatDirty = false;
                    UpdateMaterialProperties();
                }

                // TODO: figure out how to prevent loss of effect on scene save without this
                UpdateMaterialProperties();

                #if UNITY_2022_1_OR_NEWER
                cmd.SetRenderTarget(renderer.cameraColorTargetHandle);
                #else
                cmd.SetRenderTarget(renderer.cameraColorTarget);
                #endif
                cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, material, submeshIndex: 0, shaderPass: 0);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }
}
#endif
