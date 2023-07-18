#ifndef TINYSRP_INPUT_DATA
#define TINYSRP_INPUT_DATA

//表面数据
struct SurfaceData {
	float3 normal;
	float3 viewDirection;
	float3 color;
	float alpha;
	float metallic;
	float smoothness;
};

//BRDF
struct BRDFData {
	float3 diffuse;
	float3 specular;
	float roughness;
};

#define MIN_REFLECTIVITY 0.04

float OneMinusReflectivity (float metallic) {
	float range = 1.0 - MIN_REFLECTIVITY;
	return range - metallic * range;
}

//获取BRDF数据
//todo 后续改成自己那套pbr 也可能做成延迟渲染的
BRDFData GetBRDFData (SurfaceData surface, bool applyAlphaToDiffuse = false) {
	BRDFData brdf;

	float oneMinusReflectivity = OneMinusReflectivity(surface.metallic);
	brdf.diffuse = surface.color * oneMinusReflectivity;
	if (applyAlphaToDiffuse) {
		brdf.diffuse *= surface.alpha;
	}

	brdf.specular = lerp(MIN_REFLECTIVITY, surface.color, surface.metallic);
	
	float perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(surface.smoothness);
	brdf.roughness = PerceptualRoughnessToRoughness(perceptualRoughness);

	return brdf;
}

//光源相关数据

//平行光最多4盏 
#define MAX_DIRECTIONAL_LIGHT_COUNT 4

CBUFFER_START(TINYLight)
	int _DirectionalLightCount;
	float4 _DirectionalLightColors[MAX_DIRECTIONAL_LIGHT_COUNT];
	float4 _DirectionalLightDirections[MAX_DIRECTIONAL_LIGHT_COUNT];
CBUFFER_END

int GetDirectionalLightCount () {
	return _DirectionalLightCount;
}

struct Light {
	float3 color;
	float3 direction;
};

//获取指定索引的平行光数据
Light GetDirectionalLight (int index) {
	Light light;
	light.color = _DirectionalLightColors[index].rgb;
	light.direction = _DirectionalLightDirections[index].xyz;
	return light;
}

#endif