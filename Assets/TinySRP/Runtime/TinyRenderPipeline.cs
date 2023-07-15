using UnityEditor;
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
#if UNITY_EDITOR
        //Scene显示ui
        PrepareForSceneWindow(camera);
#endif

        ScriptableCullingParameters cullingParameters;
        if (!camera.TryGetCullingParameters(out cullingParameters))
        {
            //没什么可渲染的就return
            return;
        }
        CullingResults cullingResults = context.Cull(ref cullingParameters);

        context.SetupCameraProperties(camera);

        var cmdBuffer = CommandBufferPool.Get(camera.name);

        var clearFlags = camera.clearFlags;
        //clear depth color
        cmdBuffer.ClearRenderTarget(
            clearFlags <= CameraClearFlags.Depth, 
            clearFlags == CameraClearFlags.Color,
            clearFlags == CameraClearFlags.Color ?
                camera.backgroundColor.linear : Color.clear
        );
        context.ExecuteCommandBuffer(cmdBuffer);

        CommandBufferPool.Release(cmdBuffer);

        //绘制Opaque物体
        var sortingSettings = new SortingSettings(camera)
        {
            criteria = SortingCriteria.CommonOpaque
        };
        var drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

        //绘制天空盒
        context.DrawSkybox(camera);

        //绘制Transparent物体
        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

#if UNITY_EDITOR
        //绘制不支持的材质
        DrawUnsupportedShaders(context, camera, cullingResults);
        //绘制Gizmos
        DrawGizmos(context, camera);
#endif

        context.Submit();
    }

#region Editor Only
#if UNITY_EDITOR
    static ShaderTagId[] legacyShaderTagIds = {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM"),
        new ShaderTagId("UniversalForward"),
    };
    static Material errorMaterial;
    //绘制unity自带的shader，显示为洋红
    void DrawUnsupportedShaders(ScriptableRenderContext context, Camera camera, CullingResults cullingResults)
    {
        if (errorMaterial == null)
        {
            errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
        }
        var drawingSettings = new DrawingSettings(
            legacyShaderTagIds[0], new SortingSettings(camera)
        )
        {
            overrideMaterial = errorMaterial
        }; ;
        for (int i = 1; i < legacyShaderTagIds.Length; i++)
        {
            drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
        }
        var filteringSettings = FilteringSettings.defaultValue;
        context.DrawRenderers(
            cullingResults, ref drawingSettings, ref filteringSettings
        );
    }

    //绘制Gizmos
    void DrawGizmos(ScriptableRenderContext context, Camera camera)
    {
        if (Handles.ShouldRenderGizmos())
        {
            context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
            context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
        }
    }

    //Scene显示ui
    void PrepareForSceneWindow(Camera camera)
    {
        if (camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
    }
#endif
    #endregion
}