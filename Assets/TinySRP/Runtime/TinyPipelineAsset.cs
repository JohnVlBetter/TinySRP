using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/Tiny Pipeline Asset")]
public class TinyPipelineAsset : RenderPipelineAsset
{
    //合批相关设置
    [SerializeField]
    bool useDynamicBatching = true, useGPUInstancing = true, useSRPBatcher = true;

    //阴影相关设置
    [SerializeField]
    ShadowSettings shadowSettings = default;

    protected override RenderPipeline CreatePipeline()
    {
        return new TinyRenderPipeline(useDynamicBatching, useGPUInstancing, useSRPBatcher, shadowSettings);
    }
}


[System.Serializable]
public class ShadowSettings
{
    //阴影最大距离
    [Min(0f)]
    public float maxDistance = 100f;

    //阴影图大小
    public enum ShadowTextureSize
    {
        _256 = 256, _512 = 512, _1024 = 1024,
        _2048 = 2048, _4096 = 4096, _8192 = 8192
    }

    //平行光阴影设置
    [System.Serializable]
    public struct DirectionalShadow
    {
        public ShadowTextureSize size;
    }

    public DirectionalShadow directional = new DirectionalShadow
    {
        size = ShadowTextureSize._1024
    };
}