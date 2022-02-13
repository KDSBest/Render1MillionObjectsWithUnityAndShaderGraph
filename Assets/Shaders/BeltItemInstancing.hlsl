#ifndef BELT_ITEM_SHADER_INCLUDED
#define BELT_ITEM_SHADER_INCLUDED

struct InstanceItemData {
	float4x4 worldMatrix;
	float4x4 worldMatrixInverse;
	float4 color;
};

StructuredBuffer<InstanceItemData> _PerInstanceItemData;

// https://github.com/Unity-Technologies/Graphics/blob/master/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/ParticlesInstancing.hlsl
void instancingItemSetup() {
	#ifndef SHADERGRAPH_PREVIEW
		unity_ObjectToWorld = mul(unity_ObjectToWorld, _PerInstanceItemData[unity_InstanceID].worldMatrix);
		unity_WorldToObject = mul(unity_WorldToObject, _PerInstanceItemData[unity_InstanceID].worldMatrixInverse);
	#endif
}

void GetInstanceItemID_float(out float Out){
	Out = 0;
	#ifndef SHADERGRAPH_PREVIEW
	#if UNITY_ANY_INSTANCING_ENABLED
	Out = unity_InstanceID;
	#endif
	#endif
}

void InstancingItem_float(float3 Position, out float3 Out) {
	Out = Position;
}

void GetColorItem_float(out float4 Out) {
	Out = float4(1, 1, 1, 1);
	#ifndef SHADERGRAPH_PREVIEW
	#if UNITY_ANY_INSTANCING_ENABLED
	Out = _PerInstanceItemData[unity_InstanceID].color;
	#endif
	#endif
}

#endif