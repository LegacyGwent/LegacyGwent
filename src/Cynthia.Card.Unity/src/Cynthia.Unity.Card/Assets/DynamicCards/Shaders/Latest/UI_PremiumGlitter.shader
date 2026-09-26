Shader "DynamicCards/Latest/UI_PremiumGlitter" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_MainGain("_MainGain",Float)=1.0
_SecondTex("_SecondTex",2D)="white" {}
_SecondGain("_SecondGain",Float)=1.0
_AlphaGain("_AlphaGain",Float)=1.0
_AlphaPower("_AlphaPower",Float)=1.0
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_ScrollSpeed("_ScrollSpeed",Vector)=(0.0,0.0,0.0,0.0)
_MaskTex("_MaskTex",2D)="white" {}
_ColorMask("_ColorMask",Color)=(1.0,1.0,1.0,1.0)
_Cutoff("_Cutoff",Float)=0.5
_Stencil("_Stencil",Float)=0.0
_StencilReadMask("_StencilReadMask",Float)=255.0
_StencilWriteMask("_StencilWriteMask",Float)=255.0
_StencilComp("_StencilComp",Float)=8.0
_StencilOp("_StencilOp",Float)=0.0
_StencilOpFail("_StencilOpFail",Float)=0.0
_StencilOpZFail("_StencilOpZFail",Float)=0.0
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Back
ZWrite Off
ZTest LEqual
Blend SrcAlpha OneMinusSrcAlpha
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_MainTex;
SamplerState sampler_MaskTex;
SamplerState sampler_SecondTex;
Texture2D<float4> _MainTex;
Texture2D<float4> _MaskTex;
Texture2D<float4> _SecondTex;
float _AlphaGain;
float _AlphaPower;
float _MainGain;
float _SecondGain;
float4 _ColorMask;
float4 _MainTex_ST;
float4 _MaskTex_ST;
float4 _ScrollSpeed;
float4 _SecondTex_ST;
float4 _TintColor;












void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0)
{
float4 dc_v_cb1[21];
dc_v_cb1[0]=float4(0,0,0,0);
dc_v_cb1[1]=float4(0,0,0,0);
dc_v_cb1[2]=float4(0,0,0,0);
dc_v_cb1[3]=float4(0,0,0,0);
dc_v_cb1[4]=float4(0,0,0,0);
dc_v_cb1[5]=float4(0,0,0,0);
dc_v_cb1[6]=float4(0,0,0,0);
dc_v_cb1[7]=float4(0,0,0,0);
dc_v_cb1[8]=float4(0,0,0,0);
dc_v_cb1[9]=float4(0,0,0,0);
dc_v_cb1[10]=float4(0,0,0,0);
dc_v_cb1[11]=float4(0,0,0,0);
dc_v_cb1[12]=float4(0,0,0,0);
dc_v_cb1[13]=float4(0,0,0,0);
dc_v_cb1[14]=float4(0,0,0,0);
dc_v_cb1[15]=float4(0,0,0,0);
dc_v_cb1[16]=float4(0,0,0,0);
dc_v_cb1[17]=float4(unity_MatrixVP[0][0],unity_MatrixVP[1][0],unity_MatrixVP[2][0],unity_MatrixVP[3][0]);
dc_v_cb1[18]=float4(unity_MatrixVP[0][1],unity_MatrixVP[1][1],unity_MatrixVP[2][1],unity_MatrixVP[3][1]);
dc_v_cb1[19]=float4(unity_MatrixVP[0][2],unity_MatrixVP[1][2],unity_MatrixVP[2][2],unity_MatrixVP[3][2]);
dc_v_cb1[20]=float4(unity_MatrixVP[0][3],unity_MatrixVP[1][3],unity_MatrixVP[2][3],unity_MatrixVP[3][3]);
float4 dc_v_cb0[4];
dc_v_cb0[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb0[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb0[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb0[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb0[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb0[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb0[2].xyzw * v0.zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb0[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb1[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb1[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb1[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb1[20].xyzw * r0.wwww + r1.xyzw;
  o1.xy = v1.xy;
  return;
}
























void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[9];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_SecondTex_ST.x,_SecondTex_ST.y,_SecondTex_ST.z,_SecondTex_ST.w);
dc_f_cb0[4]=float4(_MaskTex_ST.x,_MaskTex_ST.y,_MaskTex_ST.z,_MaskTex_ST.w);
dc_f_cb0[5]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[6]=float4(_ScrollSpeed.x,_ScrollSpeed.y,_ScrollSpeed.z,_ScrollSpeed.w);
dc_f_cb0[7]=float4(_SecondGain,_MainGain,_AlphaGain,_AlphaPower);
dc_f_cb0[8]=float4(_ColorMask.x,_ColorMask.y,_ColorMask.z,_ColorMask.w);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_f_cb0[6].xyzw * dc_f_cb1[0].yyyy + v1.xyxy;
  r0.zw = r0.zw * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r0.xy = r0.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.xyzw = _SecondTex.Sample(sampler_SecondTex, r0.zw).xyzw;
  r0.x = r0.x * r0.w + -1;
  r0.x = dc_f_cb0[7].x * r0.x + 1;
  r0.y = r1.w * r0.x;
  r1.xyz = dc_f_cb0[7].yyy * r1.xyz;
  r0.xzw = r1.xyz * r0.xxx;
  r0.xzw = dc_f_cb0[5].xyz * r0.xzw;
  o0.xyz = dc_f_cb0[8].xyz * r0.xzw;
  r0.x = log2(r0.y);
  r0.x = dc_f_cb0[7].w * r0.x;
  r0.x = exp2(r0.x);
  r0.x = saturate(dc_f_cb0[7].z * r0.x);
  r0.yz = v1.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r1.xyzw = _MaskTex.Sample(sampler_MaskTex, r0.yz).xyzw;
  r0.y = r1.x * r1.w;
  r0.x = r0.x * r0.y;
  r0.x = dc_f_cb0[5].w * r0.x;
  o0.w = dc_f_cb0[8].w * r0.x;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
