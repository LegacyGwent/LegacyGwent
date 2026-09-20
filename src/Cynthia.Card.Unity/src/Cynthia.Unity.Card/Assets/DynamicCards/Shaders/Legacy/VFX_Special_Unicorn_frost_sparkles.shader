Shader "DynamicCards/Legacy/VFX_Special_Unicorn_frost_sparkles" {
Properties {
_dot("_dot",2D)="white" {}
_sparkles("_sparkles",2D)="white" {}
_brightness("_brightness",Float)=2.0
_main_brightness("_main_brightness",Float)=122.0
_power1("_power1",Float)=4.0
_power_screen("_power_screen",Float)=1.0
_speed("_speed",Float)=0.004999999888241291
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Back
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
SamplerState sampler_dot;
SamplerState sampler_sparkles;
Texture2D<float4> _dot;
Texture2D<float4> _sparkles;
float _brightness;
float _main_brightness;
float _power1;
float _power_screen;
float _speed;
float4 _TimeEditor;
float4 _TintColor;
float4 _dot_ST;
float4 _sparkles_ST;












void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float2 v2 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float3 o3 : TEXCOORD2)
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
float4 dc_v_cb0[7];
dc_v_cb0[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb0[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb0[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb0[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
dc_v_cb0[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_v_cb0[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_v_cb0[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb0[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb0[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb0[2].xyzw * v0.zzzz + r0.xyzw;
  r1.xyzw = dc_v_cb0[3].xyzw + r0.xyzw;
  o2.xyzw = dc_v_cb0[3].xyzw * v0.wwww + r0.xyzw;
  r0.xyzw = dc_v_cb1[18].xyzw * r1.yyyy;
  r0.xyzw = dc_v_cb1[17].xyzw * r1.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[19].xyzw * r1.zzzz + r0.xyzw;
  o0.xyzw = dc_v_cb1[20].xyzw * r1.wwww + r0.xyzw;
  o1.xy = v2.xy;
  r0.x = dot(v1.xyz, dc_v_cb0[4].xyz);
  r0.y = dot(v1.xyz, dc_v_cb0[5].xyz);
  r0.z = dot(v1.xyz, dc_v_cb0[6].xyz);
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  o3.xyz = r0.xyz * r0.www;
  return;
}




















void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float3 v3 : TEXCOORD2,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[8];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_dot_ST.x,_dot_ST.y,_dot_ST.z,_dot_ST.w);
dc_f_cb0[4]=float4(_sparkles_ST.x,_sparkles_ST.y,_sparkles_ST.z,_sparkles_ST.w);
dc_f_cb0[5]=float4(_brightness,_main_brightness,_power1,_power_screen);
dc_f_cb0[6]=float4(_speed,0,0,0);
dc_f_cb0[7]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = v2.yzzx * dc_f_cb0[4].xyxy + dc_f_cb0[4].zwzw;
  r1.xyzw = _sparkles.Sample(sampler_sparkles, r0.zw).xyzw;
  r0.xyzw = _sparkles.SampleLevel(sampler_sparkles, r0.xy, 1).xyzw;
  r0.w = dot(v3.xyz, v3.xyz);
  r0.w = rsqrt(r0.w);
  r2.xyz = v3.xyz * r0.www;
  r1.xyz = abs(r2.yyy) * r1.xyz;
  r0.xyz = abs(r2.xxx) * r0.xyz + r1.xyz;
  r1.xy = v2.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r1.xyzw = _sparkles.Sample(sampler_sparkles, r1.xy).xyzw;
  r0.xyz = abs(r2.zzz) * r1.xyz + r0.xyz;
  r0.xyz = log2(r0.xyz);
  r0.xyz = dc_f_cb0[5].zzz * r0.xyz;
  r0.xyz = exp2(r0.xyz);
  r0.xyz = dc_f_cb0[5].xxx * r0.xyz;
  r0.w = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r1.xy = dc_f_cb0[6].xx * r0.ww + v1.xy;
  r1.xy = r1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.xyzw = _dot.SampleLevel(sampler_dot, r1.xy, 1).xyzw;
  r1.xyz = log2(r1.xyz);
  r1.xyz = dc_f_cb0[5].www * r1.xyz;
  r1.xyz = exp2(r1.xyz);
  r0.xyz = r1.xyz * r0.xyz;
  r0.xyz = dc_f_cb0[5].yyy * r0.xyz;
  r0.xyz = dc_f_cb0[7].xyz * r0.xyz;
  o0.xyz = dc_f_cb0[7].www * r0.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
