#ifndef TINYSRP_INPUT_DATA
#define TINYSRP_INPUT_DATA

//表面数据
struct SurfaceData {
	float3 normal;
	float3 color;
	float alpha;
};

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