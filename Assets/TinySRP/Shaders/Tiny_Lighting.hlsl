#ifndef TINYSRP_LIGHTING
#define TINYSRP_LIGHTING

//计算入射光
float3 IncomingLight(SurfaceData surface, Light light) {
	return saturate(dot(surface.normal, light.direction)) * light.color;
}

//计算高光强度
float SpecularStrength (SurfaceData surface, BRDFData brdf, Light light) {
	float3 h = SafeNormalize(light.direction + surface.viewDirection);
	float nh2 = Square(saturate(dot(surface.normal, h)));
	float lh2 = Square(saturate(dot(light.direction, h)));
	float r2 = Square(brdf.roughness);
	float d2 = Square(nh2 * (r2 - 1.0) + 1.00001);
	float normalization = brdf.roughness * 4.0 + 2.0;
	return r2 / (d2 * max(0.1, lh2) * normalization);
}

//PBR直接光照
float3 DirectBRDF (SurfaceData surface, BRDFData brdf, Light light) {
	return SpecularStrength(surface, brdf, light) * brdf.specular + brdf.diffuse;
}

float3 GetLighting(SurfaceData surface, BRDFData brdf, Light light) {
	return IncomingLight(surface, light) * DirectBRDF(surface, brdf, light);
}

//计算光照
float3 GetLighting (SurfaceData surface, BRDFData brdf) {
	float3 color = 0.0;
	for (int i = 0; i < GetDirectionalLightCount(); i++) {
		color += GetLighting(surface, brdf, GetDirectionalLight(i));
	}
	return color;
}

#endif