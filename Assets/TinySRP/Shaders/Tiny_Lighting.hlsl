#ifndef TINYSRP_LIGHTING
#define TINYSRP_LIGHTING

//计算入射光
float3 IncomingLight(SurfaceData surface, Light light) {
	return saturate(dot(surface.normal, light.direction)) * light.color;
}

float3 GetLighting(SurfaceData surface, Light light) {
	return IncomingLight(surface, light) * surface.color;
}


float3 GetLighting (SurfaceData surface) {
	float3 color = 0.0;
	for (int i = 0; i < GetDirectionalLightCount(); i++) {
		color += GetLighting(surface, GetDirectionalLight(i));
	}
	return color;
}


#endif