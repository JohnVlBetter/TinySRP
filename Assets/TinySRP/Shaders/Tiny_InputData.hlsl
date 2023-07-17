#ifndef TINYSRP_INPUT_DATA
#define TINYSRP_INPUT_DATA

struct SurfaceData {
	float3 normal;
	float3 color;
	float alpha;
};

CBUFFER_START(TINYLight)
	float3 _DirectionalLightColor;
	float3 _DirectionalLightDirection;
CBUFFER_END

struct Light {
	float3 color;
	float3 direction;
};

Light GetDirectionalLight () {
	Light light;
	light.color = _DirectionalLightColor;
	light.direction = _DirectionalLightDirection;
	return light;
}

#endif