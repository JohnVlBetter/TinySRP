#ifndef TINYSRP_LIGHTING
#define TINYSRP_LIGHTING

float3 IncomingLight(SurfaceData surface, Light light) {
	return saturate(dot(surface.normal, light.direction)) * light.color;
}

float3 GetLighting(SurfaceData surface, Light light) {
	return IncomingLight(surface, light) * surface.color;
}

#endif