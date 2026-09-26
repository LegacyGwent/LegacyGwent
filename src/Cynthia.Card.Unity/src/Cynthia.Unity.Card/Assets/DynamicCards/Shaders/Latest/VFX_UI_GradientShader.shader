Shader "DynamicCards/Latest/VFX_UI_GradientShader" {
Properties {
_Progress("_Progress",Float)=0.0
_StartProgress("_StartProgress",Float)=0.0
_GlowValue("_GlowValue",Float)=0.0
_MainTex("_MainTex",2D)="white" {}
_TintColor("_TintColor",Color)=(0.5,0.5,0.5,1.0)
_SecondTintColor("_SecondTintColor",Color)=(0.5,0.5,0.5,1.0)
_Squash("_Squash",Float)=0.5
_StencilComp("_StencilComp",Float)=8.0
_Stencil("_Stencil",Float)=0.0
_StencilOp("_StencilOp",Float)=0.0
_StencilWriteMask("_StencilWriteMask",Float)=255.0
_StencilReadMask("_StencilReadMask",Float)=255.0
_ColorMask("_ColorMask",Float)=15.0
_UseUIAlphaClip("_UseUIAlphaClip",Float)=0.0
}
SubShader {
Tags {"CanUseSpriteAtlas"="true" "IGNOREPROJECTOR"="true" "PreviewType"="Plane" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Off
ZWrite Off
ZTest [unity_GUIZTestMode]
Blend SrcAlpha OneMinusSrcAlpha
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_MainTex;
Texture2D<float4> _MainTex;
float _GlowValue;
float _Progress;
float _Squash;
float _StartProgress;
float3 _SecondTintColor;
float3 _TintColor;
float4 _MainTex_ST;
float4 _TextureSampleAdd;














void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float2 o0 : TEXCOORD0,
  out float4 o1 : SV_POSITION0,
  out float4 o2 : COLOR0,
  out float4 o3 : TEXCOORD1)
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
dc_v_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  o0.xy = v1.xy * dc_v_cb0[2].xy + dc_v_cb0[2].zw;
  r0.xyzw = dc_v_cb1[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb1[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[2].xyzw * v0.zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb1[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb2[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb2[19].xyzw * r0.zzzz + r1.xyzw;
  o1.xyzw = dc_v_cb2[20].xyzw * r0.wwww + r1.xyzw;
  o2.xyzw = v2.xyzw;
  o3.xyzw = v0.xyzw;
  return;
}














void dc_f(
  float2 v0 : TEXCOORD0,
  float4 v1 : SV_POSITION0,
  float4 v2 : COLOR0,
  float4 v3 : TEXCOORD1,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(_TintColor.x,_TintColor.y,_TintColor.z,0);
dc_f_cb0[4]=float4(_SecondTintColor.x,_SecondTintColor.y,_SecondTintColor.z,_GlowValue);
dc_f_cb0[5]=float4(_Progress,_StartProgress,_Squash,0);
dc_f_cb0[6]=float4(_TextureSampleAdd.x,_TextureSampleAdd.y,_TextureSampleAdd.z,_TextureSampleAdd.w);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb0[5].x * 0.200000003 + -0.200000003;
  r0.y = -1 + dc_f_cb0[5].z;
  r0.y = r0.y * -30 + 10;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, v0.xy).xyzw;
  r0.z = -1 + r1.y;
  r0.zw = dc_f_cb0[5].xy + r0.zz;
  r0.x = saturate(r0.y * r0.w + r0.x);
  r0.y = saturate(r0.y * r0.z + dc_f_cb0[5].x);
  r0.x = r0.y + -r0.x;
  r0.y = r0.y * r1.w;
  r2.w = v2.w * r0.y;
  r0.x = dc_f_cb0[4].w * r0.x;
  r0.yzw = -dc_f_cb0[4].xyz + dc_f_cb0[3].xyz;
  r0.yzw = r1.xxx * r0.yzw + dc_f_cb0[4].xyz;
  r1.xyz = r0.xxx * r0.yzw;
  r2.xyz = r1.xyz * v2.xyz + r0.yzw;
  o0.xyzw = dc_f_cb0[6].xyzw + r2.xyzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
