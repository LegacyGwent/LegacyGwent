Shader "DynamicCards/Latest/Leader_Shup" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Intensity("_Intensity",Float)=1.0
_MR_Tex("_MR_Tex",2D)="white" {}
_RoughVal("_RoughVal",Float)=0.0
_MetalVal("_MetalVal",Float)=0.0
_EmissVal("_EmissVal",Float)=0.0
_Glow("_Glow",Float)=0.0
_NormTex("_NormTex",2D)="bump" {}
_NormVal("_NormVal",Float)=0.0
_Outline("_Outline",Float)=1.0
_GradientLength("_GradientLength",Float)=1.0
_GradienOffset("_GradienOffset",Float)=0.0
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_Darken("_Darken",Float)=0.4000000059604645
_Cut("_Cut",Float)=0.5
}
SubShader {
Tags {"RenderType"="Opaque"}
Pass {
Cull Front
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
float _Outline;














void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  out float4 o0 : SV_POSITION0)
{
float4 dc_v_cb2[21];
dc_v_cb2[0]=float4(0,0,0,0);
dc_v_cb2[1]=float4(0,0,0,0);
dc_v_cb2[2]=float4(0,0,0,0);
dc_v_cb2[3]=float4(0,0,0,0);
dc_v_cb2[4]=float4(0,0,0,0);
dc_v_cb2[5]=float4(0,0,0,0);
dc_v_cb2[6]=float4(0,0,0,0);
dc_v_cb2[7]=float4(0,0,0,0);
dc_v_cb2[8]=float4(0,0,0,0);
dc_v_cb2[9]=float4(0,0,0,0);
dc_v_cb2[10]=float4(0,0,0,0);
dc_v_cb2[11]=float4(0,0,0,0);
dc_v_cb2[12]=float4(0,0,0,0);
dc_v_cb2[13]=float4(0,0,0,0);
dc_v_cb2[14]=float4(0,0,0,0);
dc_v_cb2[15]=float4(0,0,0,0);
dc_v_cb2[16]=float4(0,0,0,0);
dc_v_cb2[17]=float4(unity_MatrixVP[0][0],unity_MatrixVP[1][0],unity_MatrixVP[2][0],unity_MatrixVP[3][0]);
dc_v_cb2[18]=float4(unity_MatrixVP[0][1],unity_MatrixVP[1][1],unity_MatrixVP[2][1],unity_MatrixVP[3][1]);
dc_v_cb2[19]=float4(unity_MatrixVP[0][2],unity_MatrixVP[1][2],unity_MatrixVP[2][2],unity_MatrixVP[3][2]);
dc_v_cb2[20]=float4(unity_MatrixVP[0][3],unity_MatrixVP[1][3],unity_MatrixVP[2][3],unity_MatrixVP[3][3]);
float4 dc_v_cb1[4];
dc_v_cb1[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb1[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb1[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb1[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
float4 dc_v_cb0[3];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_Outline,0,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyz = v1.xyz * dc_v_cb0[2].xxx + v0.xyz;
  r1.xyzw = dc_v_cb1[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb1[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb1[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb1[3].xyzw * v0.wwww + r0.xyzw;
  r1.xyzw = dc_v_cb2[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb2[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb2[20].xyzw * r0.wwww + r1.xyzw;
  return;
}








void dc_f(
  float4 v0 : SV_POSITION0,
  out float4 o0 : SV_Target0)
{




  o0.xyzw = float4(0,0,0,1);
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
