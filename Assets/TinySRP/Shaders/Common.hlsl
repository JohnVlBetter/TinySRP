#ifndef TINY_COMMON_INCLUDED
#define TINY_COMMON_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "UnityInput.hlsl"

#define UNITY_MATRIX_M        unity_ObjectToWorld
#define UNITY_MATRIX_I_M      unity_WorldToObject
#define UNITY_MATRIX_V        unity_MatrixV
#define UNITY_MATRIX_VP       unity_MatrixVP
#define UNITY_MATRIX_P        glstate_matrix_projection
#define UNITY_PREV_MATRIX_M   unity_Prev_MatrixM
#define UNITY_PREV_MATRIX_I_M unity_Prev_MatrixIM

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

float Square (float v) {
	return v * v;
}
	
#endif