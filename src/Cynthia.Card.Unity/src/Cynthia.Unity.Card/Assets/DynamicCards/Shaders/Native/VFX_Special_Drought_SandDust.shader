Shader "DynamicCards/Native/VFX_Special_Drought_SandDust" {
Properties {
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_UV_swap("_UV_swap",Float)=0.0
_U_speed("_U_speed",Float)=0.0
_V_speed("_V_speed",Float)=0.0
_MainTex("_MainTex",2D)="white" {}
_MaskTex("_MaskTex",2D)="white" {}
_Use_vertex_color("_Use_vertex_color",Float)=1.0
_use_vertex_alpha("_use_vertex_alpha",Float)=1.0
_Alpha_Exponent("_Alpha_Exponent",Float)=1.0
_Alpha_Multiplier("_Alpha_Multiplier",Float)=1.0
_Mask_Color_Blend("_Mask_Color_Blend",Float)=0.0
_Cutoff("_Cutoff",Float)=0.5
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
Texture2D<float4> _MainTex;
Texture2D<float4> _MaskTex;
float _Alpha_Exponent;
float _Alpha_Multiplier;
float _Mask_Color_Blend;
float _UV_swap;
float _U_speed;
float _Use_vertex_color;
float _V_speed;
float _use_vertex_alpha;
float4 _MainTex_ST;
float4 _MaskTex_ST;
float4 _TimeEditor;
float4 _TintColor;












void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
  out float4 o2 : COLOR0)
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
  o2.xyzw = v2.xyzw;
  return;
}




















void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[9];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[4]=float4(_UV_swap,_U_speed,_V_speed,0);
dc_f_cb0[5]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[6]=float4(_MaskTex_ST.x,_MaskTex_ST.y,_MaskTex_ST.z,_MaskTex_ST.w);
dc_f_cb0[7]=float4(_Use_vertex_color,_use_vertex_alpha,_Alpha_Exponent,_Alpha_Multiplier);
dc_f_cb0[8]=float4(_Mask_Color_Blend,0,0,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.yx + -v1.xy;
  r0.xy = dc_f_cb0[4].xx * r0.xy + v1.xy;
  r0.z = dc_f_cb1[0].x + dc_f_cb0[2].x;
  r0.xy = dc_f_cb0[4].yz * r0.zz + r0.xy;
  r0.xy = r0.xy * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r1.xy = v1.xy * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r1.xyzw = _MaskTex.Sample(sampler_MaskTex, r1.xy).xyzw;
  r2.xyz = float3(1,1,1) + -r1.xyz;
  r2.xyz = dc_f_cb0[8].xxx * r2.xyz + r1.xyz;
  r1.x = r1.x * r1.w;
  r0.w = r1.x * r0.w;
  r0.xyz = r2.xyz * r0.xyz;
  r0.xyzw = dc_f_cb0[3].xyzw * r0.xyzw;
  r1.xyzw = float4(-1,-1,-1,-1) + v2.xyzw;
  r1.xyzw = dc_f_cb0[7].xxxy * r1.xyzw + float4(1,1,1,1);
  o0.xyz = r1.xyz * r0.xyz;
  r0.x = r1.w * r0.w;
  r0.x = log2(r0.x);
  r0.x = dc_f_cb0[7].z * r0.x;
  r0.x = exp2(r0.x);
  o0.w = saturate(dc_f_cb0[7].w * r0.x);
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
