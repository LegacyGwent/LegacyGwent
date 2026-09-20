Shader "DynamicCards/Legacy/VFX_Special_Unicorn_alchemist_fire" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_TintColor("_TintColor",Color)=(0.5,0.5,0.5,1.0)
_Turbulence("_Turbulence",2D)="white" {}
_Strength("_Strength",Float)=1.0
_text_speed("_text_speed",Float)=-1.0
_texture_brightness("_texture_brightness",Float)=12.0
_texture_title_v("_texture_title_v",Float)=3.0
_turbulence_speed("_turbulence_speed",Float)=-1.0
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Off
ZWrite Off
ZTest LEqual
Blend One One
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_MainTex;
SamplerState sampler_Turbulence;
Texture2D<float4> _MainTex;
Texture2D<float4> _Turbulence;
float _Strength;
float _text_speed;
float _texture_brightness;
float _texture_title_v;
float _turbulence_speed;
float4 _MainTex_ST;
float4 _TimeEditor;
float4 _TintColor;
float4 _Turbulence_ST;




















void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : TEXCOORD0,
  float2 v2 : TEXCOORD1,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
  out float2 p1 : TEXCOORD1)
{
float4 dc_v_cb3[21];
dc_v_cb3[0]=float4(0,0,0,0);
dc_v_cb3[1]=float4(0,0,0,0);
dc_v_cb3[2]=float4(0,0,0,0);
dc_v_cb3[3]=float4(0,0,0,0);
dc_v_cb3[4]=float4(0,0,0,0);
dc_v_cb3[5]=float4(0,0,0,0);
dc_v_cb3[6]=float4(0,0,0,0);
dc_v_cb3[7]=float4(0,0,0,0);
dc_v_cb3[8]=float4(0,0,0,0);
dc_v_cb3[9]=float4(0,0,0,0);
dc_v_cb3[10]=float4(0,0,0,0);
dc_v_cb3[11]=float4(0,0,0,0);
dc_v_cb3[12]=float4(0,0,0,0);
dc_v_cb3[13]=float4(0,0,0,0);
dc_v_cb3[14]=float4(0,0,0,0);
dc_v_cb3[15]=float4(0,0,0,0);
dc_v_cb3[16]=float4(0,0,0,0);
dc_v_cb3[17]=float4(unity_MatrixVP[0][0],unity_MatrixVP[1][0],unity_MatrixVP[2][0],unity_MatrixVP[3][0]);
dc_v_cb3[18]=float4(unity_MatrixVP[0][1],unity_MatrixVP[1][1],unity_MatrixVP[2][1],unity_MatrixVP[3][1]);
dc_v_cb3[19]=float4(unity_MatrixVP[0][2],unity_MatrixVP[1][2],unity_MatrixVP[2][2],unity_MatrixVP[3][2]);
dc_v_cb3[20]=float4(unity_MatrixVP[0][3],unity_MatrixVP[1][3],unity_MatrixVP[2][3],unity_MatrixVP[3][3]);
float4 dc_v_cb2[4];
dc_v_cb2[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb2[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb2[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb2[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
float4 dc_v_cb1[1];
dc_v_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_v_cb0[8];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(_Turbulence_ST.x,_Turbulence_ST.y,_Turbulence_ST.z,_Turbulence_ST.w);
dc_v_cb0[6]=float4(_Strength,0,0,0);
dc_v_cb0[7]=float4(_turbulence_speed,0,0,0);

float4 dc_pack_OSGN_1=float4(0,0,0,0);

  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_v_cb1[0].y + dc_v_cb0[2].y;
  r0.y = r0.x * dc_v_cb0[7].x + v2.y;
  r0.x = v2.x;
  r0.xy = r0.xy * dc_v_cb0[5].xy + dc_v_cb0[5].zw;
  r0.xyzw = _Turbulence.SampleLevel(sampler_Turbulence, r0.xy, 0).xyzw;
  r0.xyz = r0.xyz * float3(2,2,2) + float3(-1,-1,-1);
  r0.xyz = abs(v1.yyy) * r0.xyz;
  r0.xyz = dc_v_cb0[6].xxx * r0.xyz + v0.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
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
  uint v2 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[4]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[5]=float4(0,0,0,0);
dc_f_cb0[6]=float4(0,_text_speed,_texture_brightness,_texture_title_v);

float4 dc_pack_ISGN_1=float4(v1.x,v1.y,w1.x,w1.y);

  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r0.x = r0.x * dc_f_cb0[6].y + dc_pack_ISGN_1.y;
  r0.y = dc_f_cb0[6].w * r0.x;
  r0.x = dc_pack_ISGN_1.x;
  r0.xy = r0.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.xyz = r0.xyz * r0.xyz;
  r1.xyz = dc_f_cb0[6].zzz * dc_f_cb0[4].xyz;
  r0.xyz = r1.xyz * r0.xyz;
  r0.w = 1 + -dc_pack_ISGN_1.y;
  o0.xyz = r0.xyz * abs(r0.www);
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
