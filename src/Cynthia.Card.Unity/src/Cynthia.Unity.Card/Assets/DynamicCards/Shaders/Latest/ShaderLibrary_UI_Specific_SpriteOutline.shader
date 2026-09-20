Shader "DynamicCards/Latest/ShaderLibrary_UI_Specific_SpriteOutline" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_SepiaColor("_SepiaColor",Color)=(0.6000000238418579,0.5,0.4000000059604645,0.6000000238418579)
_GhostingInfluence("_GhostingInfluence",Float)=0.0
_Saturation("_Saturation",Float)=0.0
_StencilComp("_StencilComp",Float)=8.0
_Stencil("_Stencil",Float)=0.0
_StencilOp("_StencilOp",Float)=0.0
_StencilWriteMask("_StencilWriteMask",Float)=255.0
_StencilReadMask("_StencilReadMask",Float)=255.0
_ColorMask("_ColorMask",Float)=15.0
_test("_test",Vector)=(0.0,1.0,0.0,0.0)
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
float _GhostingInfluence;
float _Saturation;
float4 _MainTex_TexelSize;
float4 _SepiaColor;
float4 _TintColor;














void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float4 o3 : TEXCOORD2,
  out float4 o4 : TEXCOORD3,
  out float4 o5 : COLOR0)
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
float4 dc_v_cb0[4];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_MainTex_TexelSize.x,_MainTex_TexelSize.y,_MainTex_TexelSize.z,_MainTex_TexelSize.w);
dc_v_cb0[3]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb1[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb1[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[2].xyzw * v0.zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb1[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb2[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb2[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb2[20].xyzw * r0.wwww + r1.xyzw;
  o1.xy = v1.xy;
  o2.w = v1.y;
  r0.xzw = float3(-1.5,1.5,-1.5) * dc_v_cb0[2].yxx;
  r0.y = 0;
  o2.xyz = v1.xyx + r0.zyx;
  o3.xy = v1.xy + r0.yz;
  r0.z = 0;
  o3.zw = v1.xy + r0.zw;
  o4.xyzw = v0.xyzw;
  o5.xyzw = dc_v_cb0[3].xyzw * v2.xyzw;
  return;
}














void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float4 v3 : TEXCOORD2,
  float4 v4 : TEXCOORD3,
  float4 v5 : COLOR0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(_Saturation,0,0,0);
dc_f_cb0[5]=float4(_SepiaColor.x,_SepiaColor.y,_SepiaColor.z,_SepiaColor.w);
dc_f_cb0[6]=float4(_GhostingInfluence,0,0,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = _MainTex.Sample(sampler_MainTex, v2.xy).xyzw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, v2.zw).xyzw;
  r0.x = r1.w + r0.w;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, v3.xy).xyzw;
  r0.x = r1.w + r0.x;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, v3.zw).xyzw;
  r0.x = r1.w + r0.x;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, v1.xy).xyzw;
  r0.w = saturate(r1.w + r0.x);
  r2.x = dc_f_cb0[6].x * dc_f_cb0[5].w;
  r2.yzw = dc_f_cb0[5].xyz + -r1.xyz;
  r1.xyz = r2.xxx * r2.yzw + r1.xyz;
  r2.xyz = -v5.xyz + r1.xyz;
  r0.xyz = r1.www * r2.xyz + v5.xyz;
  r0.xyzw = -r1.xyzw + r0.xyzw;
  r2.x = saturate(dc_f_cb0[4].x);
  r0.xyzw = r2.xxxx * r0.xyzw + r1.xyzw;
  o0.xyzw = v5.wwww * r0.xyzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
