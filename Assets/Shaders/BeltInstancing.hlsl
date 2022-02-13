#ifndef BELT_SHADER_INCLUDED
#define BELT_SHADER_INCLUDED

struct InstanceData {
	float4x4 worldMatrix;
	float4x4 worldMatrixInverse;
};

StructuredBuffer<InstanceData> _PerInstanceData;

// https://github.com/Unity-Technologies/Graphics/blob/master/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/ParticlesInstancing.hlsl
void instancingSetup() {
	#ifndef SHADERGRAPH_PREVIEW
		unity_ObjectToWorld = mul(unity_ObjectToWorld, _PerInstanceData[unity_InstanceID].worldMatrix);
		unity_WorldToObject = mul(unity_WorldToObject, _PerInstanceData[unity_InstanceID].worldMatrixInverse);
	#endif
}

void GetInstanceID_float(out float Out){
	Out = 0;
	#ifndef SHADERGRAPH_PREVIEW
	#if UNITY_ANY_INSTANCING_ENABLED
	Out = unity_InstanceID;
	#endif
	#endif
}

void Instancing_float(float3 Position, out float3 Out) {
	Out = Position;
}

#endif