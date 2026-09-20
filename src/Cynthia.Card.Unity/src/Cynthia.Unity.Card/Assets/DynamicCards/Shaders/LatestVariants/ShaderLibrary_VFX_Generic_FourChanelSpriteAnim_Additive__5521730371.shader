Shader "DynamicCards/LatestVariant/ShaderLibrary_VFX_Generic_FourChanelSpriteAnim_Additive__5521730371" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_ColorOne("_ColorOne",Color)=(1.0,1.0,1.0,1.0)
_ColorTwo("_ColorTwo",Color)=(1.0,1.0,1.0,1.0)
_ColorsBlendPower("_ColorsBlendPower",Float)=1.0
_ColorsBlendOffset("_ColorsBlendOffset",Float)=0.0
_ColorSaturationPower("_ColorSaturationPower",Float)=0.0
_OpacityExponent("_OpacityExponent",Float)=1.0
_OpacityMultiplier("_OpacityMultiplier",Float)=1.0
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "PreviewType"="Plane" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Off
ZWrite Off
ZTest LEqual
Blend SrcAlpha One
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_MainTex;
Texture2D<float4> _MainTex;
float _ColorSaturationPower;
float _ColorsBlendOffset;
float _ColorsBlendPower;
float _OpacityExponent;
float _OpacityMultiplier;
float4 _ColorOne;
float4 _ColorTwo;
float4 _MainTex_ST;














void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : COLOR0,
  float3 v2 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : COLOR0,
  out float4 o2 : TEXCOORD0,
  out int2 o3 : TEXCOORD1)
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

  r0.xyzw = dc_v_cb1[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb1[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[2].xyzw * v0.zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb1[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb2[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb2[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb2[20].xyzw * r0.wwww + r1.xyzw;
  o1.xyzw = v1.xyzw;
  r0.x = -0.00999999978 + dc_v_cb0[2].x;
  r0.x = max(0, r0.x);
  r0.x = min(dc_v_cb0[2].x, r0.x);
  r0.y = 4 * v2.z;
  r0.y = frac(r0.y);
  r0.z = dc_v_cb0[2].x * dc_v_cb0[2].y;
  r0.y = r0.y * r0.z;
  r0.y = floor(r0.y);
  r0.y = dc_v_cb0[2].x + r0.y;
  r0.x = r0.y / r0.x;
  r0.x = floor(r0.x);
  r0.y = -dc_v_cb0[2].x * r0.x + r0.y;
  r0.x = v2.y + -r0.x;
  o2.y = r0.x / dc_v_cb0[2].y;
  r0.x = v2.x + r0.y;
  o2.x = r0.x / dc_v_cb0[2].x;
  o2.z = v2.z;
  r0.x = 4 * r0.z;
  r0.y = frac(v2.z);
  r0.x = r0.y * r0.x;
  r0.x = floor(r0.x);
  r0.x = r0.x / r0.z;
  o3.x = (int)r0.x;
  o3.y = 0;
  return;
}














void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : COLOR0,
  float4 v2 : TEXCOORD0,
  nointerpolation int2 v3 : TEXCOORD1,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(_ColorOne.x,_ColorOne.y,_ColorOne.z,_ColorOne.w);
dc_f_cb0[4]=float4(_ColorTwo.x,_ColorTwo.y,_ColorTwo.z,_ColorTwo.w);
dc_f_cb0[5]=float4(_ColorsBlendPower,_ColorsBlendOffset,_ColorSaturationPower,_OpacityExponent);
dc_f_cb0[6]=float4(_OpacityMultiplier,0,0,0);



  const float4 icb[] = { { 1.000000, 0, 0, 0},
                              { 0, 1.000000, 0, 0},
                              { 0, 0, 1.000000, 0},
                              { 0, 0, 0, 1.000000} };
  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = _MainTex.Sample(sampler_MainTex, v2.xy).xyzw;
  r1.x = v3.x;
  r0.x = dot(r0.xyzw, icb[r1.x+0].xyzw);
  r0.x = log2(r0.x);
  r0.y = dc_f_cb0[5].z * dc_f_cb0[5].x;
  r0.y = dot(r0.yy, r0.xx);
  r0.xz = dc_f_cb0[5].xw * r0.xx;
  r0.xz = exp2(r0.xz);
  r0.y = exp2(r0.y);
  r0.x = dc_f_cb0[5].y + r0.x;
  r0.z = saturate(dc_f_cb0[6].x * r0.z);
  r1.xyzw = dc_f_cb0[4].xyzw + -dc_f_cb0[3].xyzw;
  r1.xyzw = r0.xxxx * r1.xyzw + dc_f_cb0[3].xyzw;
  r2.xyzw = dc_f_cb0[5].zzzz + r1.xyzw;
  r2.xyzw = r2.xyzw * r0.yyyy;
  r1.xyzw = saturate(r2.xyzw * dc_f_cb0[5].zzzz + r1.xyzw);
  r0.x = v1.w * r1.w;
  o0.xyz = r1.xyz;
  o0.w = r0.z * r0.x;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
