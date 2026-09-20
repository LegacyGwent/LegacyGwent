Shader "DynamicCards/Legacy/Custom_Particle_AdditiveCC_AnimtedTexture_Distort" {
Properties {
_OpacityDefault0fordiffusetest1("_OpacityDefault0fordiffusetest1",Float)=0.0
_MainTex("_MainTex",2D)="white" {}
_maxParticleBrightness("_maxParticleBrightness",Float)=1.0
_TintColor("_TintColor",Color)=(0.5,0.5,0.5,1.0)
_AnimatedTextureTurnOn("_AnimatedTextureTurnOn",Float)=0.0
_Column("_Column",Float)=1.0
_Row("_Row",Float)=1.0
_speed("_speed",Float)=1.0
_numberofframes("_numberofframes",Float)=1.0
_TurnOnRandomFrameOffset("_TurnOnRandomFrameOffset",Float)=0.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
GrabPass { "Refraction" }
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
SamplerState samplerRefraction;
SamplerState sampler_MainTex;
Texture2D<float4> Refraction;
Texture2D<float4> _MainTex;
float _AnimatedTextureTurnOn;
float _Column;
float _OpacityDefault0fordiffusetest1;
float _Row;
float _TurnOnRandomFrameOffset;
float _maxParticleBrightness;
float _numberofframes;
float _speed;
float4 _MainTex_ST;
float4 _TimeEditor;
float4 _TintColor;














void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float2 v2 : TEXCOORD0,
  float4 v3 : COLOR0,
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
float4 dc_v_cb1[7];
dc_v_cb1[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb1[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb1[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb1[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
dc_v_cb1[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_v_cb1[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_v_cb1[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);
float4 dc_v_cb0[6];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(_ProjectionParams.x,_ProjectionParams.y,_ProjectionParams.z,_ProjectionParams.w);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb1[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb1[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[2].xyzw * v0.zzzz + r0.xyzw;
  r1.xyzw = dc_v_cb1[3].xyzw + r0.xyzw;
  o2.xyzw = dc_v_cb1[3].xyzw * v0.wwww + r0.xyzw;
  r0.xyzw = dc_v_cb2[18].xyzw * r1.yyyy;
  r0.xyzw = dc_v_cb2[17].xyzw * r1.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb2[19].xyzw * r1.zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb2[20].xyzw * r1.wwww + r0.xyzw;
  o0.xyzw = r0.xyzw;
  r0.xy = r0.xy / r0.ww;
  o1.xy = v2.xy;
  r1.x = dot(v1.xyz, dc_v_cb1[4].xyz);
  r1.y = dot(v1.xyz, dc_v_cb1[5].xyz);
  r1.z = dot(v1.xyz, dc_v_cb1[6].xyz);
  r1.w = dot(r1.xyz, r1.xyz);
  r1.w = rsqrt(r1.w);
  o3.xyz = r1.xyz * r1.www;
  r0.z = dc_v_cb0[5].x * r0.y;
  r0.w = 0;
  o4.xyzw = r0.xzww;
  o5.xyzw = v3.xyzw;
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
float4 dc_f_cb3[1];
dc_f_cb3[0]=float4(glstate_lightmodel_ambient.x,glstate_lightmodel_ambient.y,glstate_lightmodel_ambient.z,glstate_lightmodel_ambient.w);
float4 dc_f_cb2[1];
dc_f_cb2[0]=float4(_WorldSpaceLightPos0.x,_WorldSpaceLightPos0.y,_WorldSpaceLightPos0.z,_WorldSpaceLightPos0.w);
float4 dc_f_cb1[6];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(0,0,0,0);
dc_f_cb1[5]=float4(_ProjectionParams.x,_ProjectionParams.y,_ProjectionParams.z,_ProjectionParams.w);
float4 dc_f_cb0[8];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_LightColor0.x,_LightColor0.y,_LightColor0.z,_LightColor0.w);
dc_f_cb0[3]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[4]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[5]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[6]=float4(_maxParticleBrightness,_Column,_Row,_speed);
dc_f_cb0[7]=float4(_numberofframes,_TurnOnRandomFrameOffset,_AnimatedTextureTurnOn,_OpacityDefault0fordiffusetest1);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = -1 + dc_f_cb0[6].z;
  r0.x = v5.x * r0.x;
  r0.x = round(r0.x);
  r0.x = dc_f_cb0[7].y * r0.x;
  r0.yz = v1.xy * float2(-1,1) + float2(1,0);
  r0.xy = r0.xx * float2(0,1) + r0.yz;
  r0.z = dc_f_cb1[0].y + dc_f_cb0[3].y;
  r0.z = dc_f_cb0[6].w * r0.z;
  r0.z = r0.z / dc_f_cb0[7].x;
  r0.w = cmp(r0.z >= -r0.z);
  r0.z = frac(abs(r0.z));
  r0.z = r0.w ? r0.z : -r0.z;
  r0.z = dc_f_cb0[7].x * r0.z;
  r0.z = round(r0.z);
  r1.x = 1 / dc_f_cb0[6].y;
  r0.w = r1.x * r0.z;
  r2.y = floor(r0.w);
  r2.x = -dc_f_cb0[6].y * r2.y + r0.z;
  r0.xy = r2.xy + r0.xy;
  r1.y = 1 / -dc_f_cb0[6].z;
  r0.xy = r0.xy * r1.xy + -v1.xy;
  r0.xy = dc_f_cb0[7].zz * r0.xy + v1.xy;
  r0.xy = r0.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r1.x = -0.5 + r0.w;
  r1.x = cmp(r1.x < 0);
  if (r1.x != 0) discard;
  r1.x = r0.w + r0.w;
  r1.xyz = dc_f_cb0[5].xyz * r1.xxx;
  r0.xyz = r1.xyz * r0.xyz;
  r0.xyz = dc_f_cb0[6].xxx * r0.xyz;
  r1.xy = float2(0.0500000007,0.0500000007) * r0.xy;
  r1.xy = r1.xy * r0.ww;
  r2.y = -dc_f_cb1[5].x * v4.y;
  r2.x = v4.x;
  r1.zw = r2.xy * float2(0.5,0.5) + float2(0.5,0.5);
  r1.xy = r1.xy * v5.ww + r1.zw;
  r1.xyzw = Refraction.Sample(samplerRefraction, r1.xy).xyzw;
  r0.w = dot(dc_f_cb2[0].xyz, dc_f_cb2[0].xyz);
  r0.w = rsqrt(r0.w);
  r2.xyz = dc_f_cb2[0].xyz * r0.www;
  r0.w = dot(v3.xyz, r2.xyz);
  r0.w = max(0, r0.w);
  r2.xyz = dc_f_cb3[0].xyz + dc_f_cb3[0].xyz;
  r2.xyz = r0.www * dc_f_cb0[2].xyz + r2.xyz;
  r0.xyz = r2.xyz * r0.xyz + -r1.xyz;
  o0.xyz = dc_f_cb0[7].www * r0.xyz + r1.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
