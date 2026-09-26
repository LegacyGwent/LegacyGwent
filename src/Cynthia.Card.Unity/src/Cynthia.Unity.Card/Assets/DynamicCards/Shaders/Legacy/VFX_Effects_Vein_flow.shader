Shader "DynamicCards/Legacy/VFX_Effects_Vein_flow" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_VeinsNoise("_VeinsNoise",2D)="white" {}
_VeinsTechnical("_VeinsTechnical",2D)="white" {}
_Addcolor("_Addcolor",Color)=(0.5,0.5,0.5,1.0)
_Veinglowmultiply("_Veinglowmultiply",Float)=1.0
_Timemultiplier("_Timemultiplier",Float)=1.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"QUEUE"="AlphaTest" "RenderType"="TransparentCutout"}
Pass {
Cull Back
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
SamplerState sampler_MainTex;
SamplerState sampler_VeinsNoise;
SamplerState sampler_VeinsTechnical;
Texture2D<float4> _MainTex;
Texture2D<float4> _VeinsNoise;
Texture2D<float4> _VeinsTechnical;
float _Timemultiplier;
float _Veinglowmultiply;
float4 _Addcolor;
float4 _MainTex_ST;
float4 _TimeEditor;
float4 _VeinsNoise_ST;
float4 _VeinsTechnical_ST;












void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : TEXCOORD0,
  float2 v2 : TEXCOORD1,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
  out float2 p1 : TEXCOORD1)
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

float4 dc_pack_OSGN_1=float4(0,0,0,0);

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
  dc_pack_OSGN_1.xy = v1.xy;
  dc_pack_OSGN_1.zw = v2.xy;
  o1=dc_pack_OSGN_1.xy;
p1=dc_pack_OSGN_1.zw;
return;
}
























void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  float2 w1 : TEXCOORD1,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[9];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[4]=float4(_VeinsTechnical_ST.x,_VeinsTechnical_ST.y,_VeinsTechnical_ST.z,_VeinsTechnical_ST.w);
dc_f_cb0[5]=float4(_VeinsNoise_ST.x,_VeinsNoise_ST.y,_VeinsNoise_ST.z,_VeinsNoise_ST.w);
dc_f_cb0[6]=float4(_Timemultiplier,0,0,0);
dc_f_cb0[7]=float4(_Addcolor.x,_Addcolor.y,_Addcolor.z,_Addcolor.w);
dc_f_cb0[8]=float4(_Veinglowmultiply,0,0,0);

float4 dc_pack_ISGN_1=float4(v1.x,v1.y,w1.x,w1.y);

  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = dc_pack_ISGN_1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.w = -0.5 + r0.w;
  r0.w = cmp(r0.w < 0);
  if (r0.w != 0) discard;
  r0.w = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r1.y = dc_f_cb0[6].x * r0.w;
  r1.x = 0;
  r1.xy = dc_pack_ISGN_1.zw + r1.xy;
  r1.xy = r1.xy * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r1.xyzw = _VeinsNoise.Sample(sampler_VeinsNoise, r1.xy).xyzw;
  r1.yz = dc_pack_ISGN_1.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r2.xyzw = _VeinsTechnical.Sample(sampler_VeinsTechnical, r1.yz).xyzw;
  r0.w = r2.x * r1.x;
  r0.w = dc_f_cb0[8].x * r0.w;
  r1.xyz = dc_f_cb0[7].xyz + -r0.xyz;
  o0.xyz = r0.www * r1.xyz + r0.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
