Shader "DynamicCards/Legacy/Custom_Cards_CardCore_ImageLayerShaderMaskedGlow" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Noise("_Noise",2D)="white" {}
_Mask("_Mask",2D)="white" {}
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_Intensity("_Intensity",Float)=1.0
_Speed("_Speed",Float)=1.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Back
ZWrite On
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
SamplerState sampler_Mask;
SamplerState sampler_Noise;
Texture2D<float4> _MainTex;
Texture2D<float4> _Mask;
Texture2D<float4> _Noise;
float _Cutoff;
float _Intensity;
float _Speed;
float4 _MainTex_ST;
float4 _Mask_ST;
float4 _Noise_ST;
float4 _TintColor;












void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : TEXCOORD0,
  float2 v2 : TEXCOORD1,
  float4 v3 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
  out float2 p1 : TEXCOORD1,
  out float4 o2 : TEXCOORD2)
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
  o2.xyzw = v3.xyzw;
  o1=dc_pack_OSGN_1.xy;
p1=dc_pack_OSGN_1.zw;
return;
}
























void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  float2 w1 : TEXCOORD1,
  float4 v2 : TEXCOORD2,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_Noise_ST.x,_Noise_ST.y,_Noise_ST.z,_Noise_ST.w);
dc_f_cb0[4]=float4(_Mask_ST.x,_Mask_ST.y,_Mask_ST.z,_Mask_ST.w);
dc_f_cb0[5]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[6]=float4(_Intensity,_Speed,_Cutoff,0);

float4 dc_pack_ISGN_1=float4(v1.x,v1.y,w1.x,w1.y);

  float4 r0,r1,r2,r3,r4,r5;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = dc_pack_ISGN_1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r1.x = cmp(dc_f_cb0[6].z >= r0.w);
  if (r1.x != 0) discard;
  r1.xy = dc_pack_ISGN_1.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r1.xyzw = _Mask.Sample(sampler_Mask, r1.xy).xyzw;
  r2.x = dc_f_cb1[0].x * dc_f_cb0[6].y;
  r2.y = 0;
  r2.xy = dc_pack_ISGN_1.zw + r2.xy;
  r2.xy = r2.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r2.xyzw = _Noise.Sample(sampler_Noise, r2.xy).xyzw;
  r3.x = -dc_f_cb1[0].x * dc_f_cb0[6].y;
  r3.y = 0;
  r2.yz = dc_pack_ISGN_1.zw + r3.xy;
  r2.yz = r2.yz * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r4.xyzw = _Noise.Sample(sampler_Noise, r2.yz).xyzw;
  r3.zw = float2(0.899999976,-1.52999997) * r3.xx;
  r3.xyzw = dc_pack_ISGN_1.zwzw + -r3.yzyw;
  r3.xyzw = r3.xyzw * dc_f_cb0[3].xyxy + dc_f_cb0[3].zwzw;
  r5.xyzw = _Noise.Sample(sampler_Noise, r3.xy).xyzw;
  r3.xyzw = _Noise.Sample(sampler_Noise, r3.zw).xyzw;
  r2.x = r4.y + r2.x;
  r2.x = r2.x + r5.z;
  r2.x = r2.x + r3.w;
  r2.x = 0.00100000005 + r2.x;
  r2.x = dc_f_cb0[6].x / r2.x;
  r2.x = r2.x * r1.x;
  r3.xyzw = dc_f_cb0[5].xyzw * r0.xyzw;
  r1.xyzw = r3.xyzw * r1.xyzw;
  r0.xyzw = r1.xyzw * r2.xxxx + r0.xyzw;
  o0.xyzw = v2.xyzw * r0.xyzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
