using UnityEngine;
using UnityEngine.Rendering;

public class TinyRenderPipeline : RenderPipeline
{
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (var camera in cameras)
        {
            RenderSingleCamera(context, camera);
        }
    }

    void RenderSingleCamera(ScriptableRenderContext context, Camera camera)
    {
        context.SetupCameraProperties(camera);

        context.DrawSkybox(camera);

        context.Submit();
    }
}