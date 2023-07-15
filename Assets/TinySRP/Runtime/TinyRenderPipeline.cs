using UnityEngine;
using UnityEngine.Rendering;

public class TinyRenderPipeline : RenderPipeline
{
    static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (var camera in cameras)
        {
            RenderSingleCamera(context, camera);
        }
    }

    void RenderSingleCamera(ScriptableRenderContext context, Camera camera)
    {
        ScriptableCullingParameters cullingParameters;
        if (!camera.TryGetCullingParameters(out cullingParameters))
        {
            //ûʲô����Ⱦ�ľ�return
            return;
        }
        CullingResults cullingResults = context.Cull(ref cullingParameters);

        context.SetupCameraProperties(camera);

        var cmdBuffer = new CommandBuffer { name = camera.name };

        var clearFlags = camera.clearFlags;
        //clear depth color
        cmdBuffer.ClearRenderTarget(
            (clearFlags & CameraClearFlags.Depth) != 0, 
            (clearFlags & CameraClearFlags.Color) != 0, 
            camera.backgroundColor
        );
        context.ExecuteCommandBuffer( cmdBuffer );

        cmdBuffer.Release();

        //����Opaque����
        var sortingSettings = new SortingSettings(camera)
        {
            criteria = SortingCriteria.CommonOpaque
        };
        var drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

        //������պ�
        context.DrawSkybox(camera);

        //����Transparent����
        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

        context.Submit();
    }
}