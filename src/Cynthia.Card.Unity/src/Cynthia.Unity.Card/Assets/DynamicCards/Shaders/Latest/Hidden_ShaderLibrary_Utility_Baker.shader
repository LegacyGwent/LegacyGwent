Shader "DynamicCards/Latest/Hidden_ShaderLibrary_Utility_Baker" {
Properties {

}
SubShader {
Tags {"RenderType"="Opaque"}
Pass {
Cull Off
ZWrite On
ZTest LEqual
Blend One Zero
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -









void dc_v(
  float4 v0 : POSITION0,
  uint v1 : SV_VertexID0,
  float4 v2 : TEXCOORD0,
  out float4 o0 : SV_Position0,
  out uint4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1)
{






  o0.xyzw = v0.xyzw;
  o1.x = v1.x;
  o2.xyzw = v2.xyzw;
  return;
}








void dc_f(
  float4 v0 : SV_POSITION0,
  float3 v1 : TEXCOORD0,
  out float4 o0 : SV_Target0)
{




  o0.xyz = v1.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
