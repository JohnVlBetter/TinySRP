using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Lighting : MonoBehaviour
{
    const string bufferName = "Lighting";

    CommandBuffer buffer = new CommandBuffer()
    {
        name = bufferName,
    };

    //平行光最多4盏
    const int MAX_DIRECTIONAL_LIGHT_COUNT = 4;

    static int dirLightCountId = Shader.PropertyToID("_DirectionalLightCount"),
               dirLightColorsId = Shader.PropertyToID("_DirectionalLightColors"),
               dirLightDirectionsId = Shader.PropertyToID("_DirectionalLightDirections");

    static Vector4[]　dirLightColors = new Vector4[MAX_DIRECTIONAL_LIGHT_COUNT],
                      dirLightDirections = new Vector4[MAX_DIRECTIONAL_LIGHT_COUNT];

    CullingResults cullingResults;

    Shadows shadows;

    public void Setup(ScriptableRenderContext context, CullingResults cullingResults, Shadows shadows)
    {
        this.cullingResults = cullingResults;
        this.shadows = shadows;

        buffer.BeginSample(bufferName);

        SetupLights();

        buffer.EndSample(bufferName);

        context.ExecuteCommandBuffer(buffer);

        buffer.Clear();
    }

    //设置光源数据
    void SetupLights() {
        NativeArray<VisibleLight> visibleLights= cullingResults.visibleLights;

        int dirLightCount = 0;

        for (int i = 0; i < visibleLights.Length; i++)
        {
            VisibleLight visibleLight = visibleLights[i];
            if (visibleLight.lightType == LightType.Directional)
            {
                SetupDirectionalLight(dirLightCount++, ref visibleLight);
                if (dirLightCount >= MAX_DIRECTIONAL_LIGHT_COUNT)
                {
                    break;
                }
            }
        }

        buffer.SetGlobalInt(dirLightCountId, visibleLights.Length);
        buffer.SetGlobalVectorArray(dirLightColorsId, dirLightColors);
        buffer.SetGlobalVectorArray(dirLightDirectionsId, dirLightDirections);
    }

    void SetupDirectionalLight(int index, ref VisibleLight visibleLight) {
        dirLightColors[index] = visibleLight.finalColor;
        dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2);
        shadows.ReserveDirectionalShadows(visibleLight.light, index);
    }
}
