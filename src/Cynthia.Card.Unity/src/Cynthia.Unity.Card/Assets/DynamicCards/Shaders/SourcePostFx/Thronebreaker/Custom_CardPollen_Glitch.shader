Shader "DynamicCards/SourcePostFx/Thronebreaker/Custom_CardPollen_Glitch" {
Properties {
_MainTex("_MainTex",2D)="black" {}
_Intensity("_Intensity",Float)=2.0
_Speed("_Speed",Float)=2.0
_Glitch("_Glitch",2D)="black" {}
_GlitchVisibility("_GlitchVisibility",Float)=1.0
_GlitchAnimationLength("_GlitchAnimationLength",Float)=1.0
}
SubShader {
Tags {"CanUseSpriteAtlas"="true" "IGNOREPROJECTOR"="true" "QUEUE"="Geometry+1" "RenderType"="Opaque"}
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
SamplerState sampler_Glitch;
SamplerState sampler_MainTex;
Texture2D<float4> _Glitch;
Texture2D<float4> _MainTex;
float _GlitchAnimationLength;
float _GlitchVisibility;
float _Intensity;
float _Speed;












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
float4 dc_f_cb0[3];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_Speed,_Intensity,_GlitchVisibility,_GlitchAnimationLength);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = cmp(v1.x < 0.180000007);
  if (r0.x != 0) {
    o0.xyzw = _MainTex.Sample(sampler_MainTex, v1.xy).xyzw;
    return;
  }
  r0.xy = dc_f_cb1[0].yx * dc_f_cb0[2].wx;
  r0.x = cmp(r0.x >= -r0.x);
  r0.x = r0.x ? dc_f_cb0[2].w : -dc_f_cb0[2].w;
  r0.z = 1 / r0.x;
  r0.z = dc_f_cb1[0].y * r0.z;
  r0.z = frac(r0.z);
  r0.x = r0.x * r0.z;
  r0.x = 36 * r0.x;
  r0.x = floor(r0.x);
  r0.z = 6 * r0.x;
  r0.z = cmp(r0.z >= -r0.z);
  r0.zw = r0.zz ? float2(6,0.166666672) : float2(-6,-0.166666672);
  r0.w = r0.x * r0.w;
  r0.w = frac(r0.w);
  r0.z = r0.z * r0.w;
  r1.xy = float2(0.165999994,0.166600004) * v1.xy;
  r2.x = r0.z * 0.166666672 + r1.x;
  r2.y = r0.x * 0.0277777798 + r1.y;
  r1.xyzw = _Glitch.Sample(sampler_Glitch, r2.xy).xyzw;
  r0.x = 9 * r0.y;
  r0.yz = v1.yy * float2(8,0.400000006) + r0.xx;
  r0.y = sin(r0.y);
  r0.x = v1.y * 2 + r0.x;
  r0.x = cos(r0.x);
  r0.xy = float2(0.75,0.5) + r0.xy;
  r0.x = r0.y * r0.x;
  r0.y = cos(r0.z);
  r0.y = 0.300000012 + -r0.y;
  r0.x = r0.x * r0.y;
  r0.y = cmp(r0.x < 0);
  r0.z = floor(r0.x);
  r0.x = ceil(r0.x);
  r0.x = r0.y ? r0.z : r0.x;
  r0.x = r0.x * dc_f_cb0[2].y + v1.x;
  r0.y = v1.y;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.w = 1;
  o0.xyzw = r1.xyzw * dc_f_cb0[2].zzzz + r0.xyzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
