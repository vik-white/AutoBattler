using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_6000_0_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
#endif
using UnityEngine.Rendering.Universal;
using UnityEngine.Scripting.APIUpdating;
using ShaderIdCommon = LeTai.Asset.TranslucentImage.ShaderId;

namespace LeTai.Asset.TranslucentImage.UniversalRP
{
enum RendererType
{
    Universal,
    Renderer2D
}

[MovedFrom("LeTai.Asset.TranslucentImage.LWRP")]
struct TISPassData
{
    public RendererType           rendererType;
    public RenderTargetIdentifier cameraColorTarget;
    public TranslucentImageSource blurSource;
    public IBlurAlgorithm         blurAlgorithm;
    public RenderOrder            renderOrder;
    public BlitMode               blitMode;
    public bool                   isPreviewing;
}

[MovedFrom("LeTai.Asset.TranslucentImage.LWRP")]
public class TranslucentImageBlurRenderPass : ScriptableRenderPass
{
    private const string PROFILER_TAG = "Translucent Image Source";

#if UNITY_6000_0_OR_NEWER
    class RenderGraphPassData
    {
        public TextureHandle source;
        public Material      previewMaterial;
        public TISPassData   passData;
    }
#endif

    readonly UniversalRendererInternal universalRendererInternal;
    readonly RenderTargetIdentifier    afterPostprocessTexture;

    TISPassData currentPassData;
    Material    previewMaterial;

    public Material PreviewMaterial
    {
        get
        {
            if (!previewMaterial)
                previewMaterial = CoreUtils.CreateEngineMaterial("Hidden/FillCrop_UniversalRP");

            return previewMaterial;
        }
    }

    internal TranslucentImageBlurRenderPass(UniversalRendererInternal universalRendererInternal)
    {
        this.universalRendererInternal = universalRendererInternal;
        afterPostprocessTexture = new RenderTargetIdentifier(Shader.PropertyToID("_AfterPostProcessTexture"),
                                                             0, CubemapFace.Unknown, -1);
#if UNITY_6000_0_OR_NEWER
        requiresIntermediateTexture = true;
#endif
    }

    ~TranslucentImageBlurRenderPass()
    {
        CoreUtils.Destroy(previewMaterial);
    }

    internal void Setup(TISPassData passData)
    {
        currentPassData = passData;
    }

#if UNITY_6000_0_OR_NEWER
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        var resourceData = frameData.Get<UniversalResourceData>();

        if (resourceData.isActiveTargetBackBuffer)
        {
            Debug.LogError("Translucent Image needs an intermediate color texture in URP RenderGraph.");
            return;
        }

        var source = resourceData.activeColorTexture;
        if (!source.IsValid() || currentPassData.blurSource == null || currentPassData.blurSource.BlurredScreen == null)
            return;

        using (var builder = renderGraph.AddUnsafePass<RenderGraphPassData>(PROFILER_TAG, out var passData))
        {
            passData.source          = source;
            passData.previewMaterial = currentPassData.isPreviewing ? PreviewMaterial : null;
            passData.passData        = currentPassData;

            builder.UseTexture(passData.source, currentPassData.isPreviewing ? AccessFlags.ReadWrite : AccessFlags.Read);
            builder.AllowPassCulling(false);
            builder.SetRenderFunc(static (RenderGraphPassData data, UnsafeGraphContext context) => Execute(data, context));
        }
    }

    static void Execute(RenderGraphPassData data, UnsafeGraphContext context)
    {
        var cmd        = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
        var source     = (RenderTargetIdentifier)data.source;
        var sourceData = data.passData.blurSource;

        data.passData.blurAlgorithm.Blur(cmd,
                                         source,
                                         sourceData.BlurRegion,
                                         sourceData.BlurredScreen);

        if (data.passData.isPreviewing)
        {
            data.previewMaterial.SetVector(ShaderIdCommon.CROP_REGION,
                                           sourceData.BlurRegion.ToMinMaxVector());
            cmd.BlitCustom(sourceData.BlurredScreen,
                           source,
                           data.previewMaterial,
                           0,
                           data.passData.blitMode);
        }
    }
#else
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var                    cmd = CommandBufferPool.Get(PROFILER_TAG);
        RenderTargetIdentifier source;
#if URP12_OR_NEWER
        if (currentPassData.rendererType == RendererType.Universal)
        {
            source = universalRendererInternal.GetBackBuffer();
        }
        else
        {
#endif
        bool useAfterPostTex = renderingData.cameraData.postProcessEnabled;
#if URP12_OR_NEWER
            useAfterPostTex &= currentPassData.renderOrder == RenderOrder.AfterPostProcessing;
#endif
        source = useAfterPostTex
                     ? afterPostprocessTexture
                     : currentPassData.cameraColorTarget;
#if URP12_OR_NEWER
        }
#endif

        currentPassData.blurAlgorithm.Blur(cmd,
                                           source,
                                           currentPassData.blurSource.BlurRegion,
                                           currentPassData.blurSource.BlurredScreen);

        if (currentPassData.isPreviewing)
        {
            PreviewMaterial.SetVector(ShaderIdCommon.CROP_REGION,
                                      currentPassData.blurSource.BlurRegion.ToMinMaxVector());
            cmd.BlitCustom(currentPassData.blurSource.BlurredScreen,
                           source,
                           PreviewMaterial,
                           0,
                           BlitMode.Triangle);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
#endif
}
}
