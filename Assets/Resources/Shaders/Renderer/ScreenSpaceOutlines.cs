using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class ScreenSpaceOutlines : ScriptableRendererFeature
{
    [System.Serializable]
    private class ScreenSpaceOutlineSettings
    {
        [Header("General Outline Settings")]
        public Color outlineColor = Color.black;
        [Range(0.0f, 20.0f)]
        public float outlineScale = 1.0f;

        [Header("Depth Settings")]
        [Range(0.0f, 100.0f)]
        public float depthThreshold = 1.5f;
        [Range(0.0f, 500.0f)]
        public float robertsCrossMultiplier = 100.0f;

        [Header("Normal Settings")]
        [Range(0.0f, 1.0f)]
        public float normalThreshold = 0.4f;

        [Header("Depth Normal Relation Settings")]
        [Range(0.0f, 2.0f)]
        public float steepAngleThreshold = 0.2f;
        [Range(0.0f, 500.0f)]
        public float steepAngleMultiplier = 25.0f;

        [Header("General Scene View Space Normal Texture Settings")]
        public RenderTextureFormat colorFormat;
        public int depthBufferBits;
        public FilterMode filterMode;
        public Color backgroundColor = Color.clear;

        [Header("View Space Normal Texture Object Draw Settings")]
        public PerObjectData perObjectData;
        public bool enableDynamicBatching;
        public bool enableInstancing;
    }

    private class ScreenSpaceOutlinePass : ScriptableRenderPass
    {
        private readonly Material screenSpaceOutlineMaterial;
        private readonly ScreenSpaceOutlineSettings settings;
        private readonly FilteringSettings filteringSettings;
        private readonly List<ShaderTagId> shaderTagIdList;
        private readonly Material normalsMaterial;

        private static readonly int SceneViewSpaceNormalsId = Shader.PropertyToID("_SceneViewSpaceNormals");

        private class NormalsPassData
        {
            public RendererListHandle RendererList;
            public Color BackgroundColor;
        }

        public ScreenSpaceOutlinePass(RenderPassEvent renderPassEvent, LayerMask layerMask, uint renderMask, ScreenSpaceOutlineSettings settings)
        {
            this.settings = settings;
            this.renderPassEvent = renderPassEvent;
            requiresIntermediateTexture = true;

            var outlineShader = Shader.Find("Hidden/Outlines");
            if (outlineShader == null)
            {
                Debug.LogWarning("ScreenSpaceOutlines: shader 'Hidden/Outlines' was not found. The renderer feature will be disabled.");
                return;
            }

            var viewSpaceNormalsShader = Shader.Find("Hidden/ViewSpaceNormals");
            if (viewSpaceNormalsShader == null)
            {
                Debug.LogWarning("ScreenSpaceOutlines: shader 'Hidden/ViewSpaceNormals' was not found. The renderer feature will be disabled.");
                return;
            }

            screenSpaceOutlineMaterial = new Material(outlineShader);
            screenSpaceOutlineMaterial.SetColor("_OutlineColor", settings.outlineColor);
            screenSpaceOutlineMaterial.SetFloat("_OutlineScale", settings.outlineScale);
            screenSpaceOutlineMaterial.SetFloat("_DepthThreshold", settings.depthThreshold);
            screenSpaceOutlineMaterial.SetFloat("_RobertsCrossMultiplier", settings.robertsCrossMultiplier);
            screenSpaceOutlineMaterial.SetFloat("_NormalThreshold", settings.normalThreshold);
            screenSpaceOutlineMaterial.SetFloat("_SteepAngleThreshold", settings.steepAngleThreshold);
            screenSpaceOutlineMaterial.SetFloat("_SteepAngleMultiplier", settings.steepAngleMultiplier);

            filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask, renderMask);
            shaderTagIdList = new List<ShaderTagId>
            {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("LightweightForward"),
                new ShaderTagId("SRPDefaultUnlit")
            };

            normalsMaterial = new Material(viewSpaceNormalsShader);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            var renderingData = frameData.Get<UniversalRenderingData>();
            var cameraData = frameData.Get<UniversalCameraData>();
            var lightData = frameData.Get<UniversalLightData>();

            if (resourceData.isActiveTargetBackBuffer)
                return;

            if (screenSpaceOutlineMaterial == null || normalsMaterial == null)
                return;

            var normalsDesc = cameraData.cameraTargetDescriptor;
            normalsDesc.colorFormat = settings.colorFormat;
            normalsDesc.depthBufferBits = settings.depthBufferBits;
            var normalsTexture = UniversalRenderer.CreateRenderGraphTexture(
                renderGraph,
                normalsDesc,
                "_ScreenSpaceOutlineNormals",
                true,
                settings.filterMode);

            using (var builder = renderGraph.AddRasterRenderPass<NormalsPassData>("ScreenSpaceOutlinesNormals", out var passData))
            {
                var drawSettings = RenderingUtils.CreateDrawingSettings(
                    shaderTagIdList,
                    renderingData,
                    cameraData,
                    lightData,
                    cameraData.defaultOpaqueSortFlags);

                drawSettings.perObjectData = settings.perObjectData;
                drawSettings.enableDynamicBatching = settings.enableDynamicBatching;
                drawSettings.enableInstancing = settings.enableInstancing;
                drawSettings.overrideMaterial = normalsMaterial;

                var rendererListParams = new RendererListParams(renderingData.cullResults, drawSettings, filteringSettings);
                passData.RendererList = renderGraph.CreateRendererList(rendererListParams);
                passData.BackgroundColor = settings.backgroundColor;

                if (!passData.RendererList.IsValid())
                    return;

                builder.UseRendererList(passData.RendererList);
                builder.SetRenderAttachment(normalsTexture, 0, AccessFlags.Write);
                builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.Read);
                builder.SetGlobalTextureAfterPass(normalsTexture, SceneViewSpaceNormalsId);

                builder.SetRenderFunc(static (NormalsPassData data, RasterGraphContext context) =>
                {
                    context.cmd.ClearRenderTarget(RTClearFlags.Color, data.BackgroundColor, 1, 0);
                    context.cmd.DrawRendererList(data.RendererList);
                });
            }

            var tempDesc = renderGraph.GetTextureDesc(resourceData.activeColorTexture);
            tempDesc.name = "_ScreenSpaceOutlineTemp";
            tempDesc.clearBuffer = false;
            var tempTexture = renderGraph.CreateTexture(tempDesc);

            var outlineBlit = new RenderGraphUtils.BlitMaterialParameters(
                resourceData.activeColorTexture,
                tempTexture,
                screenSpaceOutlineMaterial,
                0);
            renderGraph.AddBlitPass(outlineBlit, "ScreenSpaceOutlinesBlit");
            renderGraph.AddCopyPass(tempTexture, resourceData.activeColorTexture, "ScreenSpaceOutlinesCopyBack");
        }

        public void Release()
        {
            CoreUtils.Destroy(screenSpaceOutlineMaterial);
            CoreUtils.Destroy(normalsMaterial);
        }
    }

    [SerializeField] private RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingSkybox;
    [SerializeField] private LayerMask outlinesLayerMask;
    [SerializeField] private uint renderLayerMask;
    [SerializeField] private ScreenSpaceOutlineSettings outlineSettings = new ScreenSpaceOutlineSettings();

    private ScreenSpaceOutlinePass screenSpaceOutlinePass;
    private bool isSupported;

    public override void Create()
    {
        if (renderPassEvent < RenderPassEvent.BeforeRenderingPrePasses)
            renderPassEvent = RenderPassEvent.BeforeRenderingPrePasses;

        screenSpaceOutlinePass = new ScreenSpaceOutlinePass(renderPassEvent, outlinesLayerMask, renderLayerMask, outlineSettings);
        isSupported = screenSpaceOutlinePass != null;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (screenSpaceOutlinePass == null)
            return;

        renderer.EnqueuePass(screenSpaceOutlinePass);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            screenSpaceOutlinePass?.Release();
    }
}
