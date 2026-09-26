Shader "DynamicCards/Native/VFX_Special_Unicorn_snow" {
Properties {
_node_331("_node_331",2D)="white" {}
_multiply("_multiply",Float)=0.20000000298023224
_power("_power",Float)=1.0
_node_2453("_node_2453",Color)=(1.0,1.0,1.0,1.0)
_node_5030("_node_5030",2D)="white" {}
_speed_snow("_speed_snow",Float)=0.33000001311302185
_snow_multiply_colour("_snow_multiply_colour",Float)=0.10000000149011612
_snow_colour("_snow_colour",Color)=(1.0,1.0,1.0,1.0)
_node_2747("_node_2747",Color)=(1.0,1.0,1.0,1.0)
_mask_top("_mask_top",Float)=1.0
_cloud_speed("_cloud_speed",Float)=6.0
_snow_power("_snow_power",Float)=1.0
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
SamplerState sampler_node_331;
SamplerState sampler_node_5030;
Texture2D<float4> _node_331;
Texture2D<float4> _node_5030;
float _cloud_speed;
float _mask_top;
float _multiply;
float _power;
float _snow_multiply_colour;
float _snow_power;
float _speed_snow;
float4 _TimeEditor;
float4 _node_2453;
float4 _node_2747;
float4 _node_331_ST;
float4 _node_5030_ST;
float4 _snow_colour;












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
float4 dc_f_cb0[11];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_node_331_ST.x,_node_331_ST.y,_node_331_ST.z,_node_331_ST.w);
dc_f_cb0[4]=float4(_multiply,_power,0,0);
dc_f_cb0[5]=float4(_node_2453.x,_node_2453.y,_node_2453.z,_node_2453.w);
dc_f_cb0[6]=float4(_node_5030_ST.x,_node_5030_ST.y,_node_5030_ST.z,_node_5030_ST.w);
dc_f_cb0[7]=float4(_speed_snow,_snow_multiply_colour,0,0);
dc_f_cb0[8]=float4(_snow_colour.x,_snow_colour.y,_snow_colour.z,_snow_colour.w);
dc_f_cb0[9]=float4(_node_2747.x,_node_2747.y,_node_2747.z,_node_2747.w);
dc_f_cb0[10]=float4(_mask_top,_cloud_speed,_snow_power,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = dc_f_cb1[0].xy + dc_f_cb0[2].xy;
  r0.y = r0.y * dc_f_cb0[7].x + v1.x;
  r1.y = r0.x * dc_f_cb0[10].y + v1.x;
  r0.x = dc_f_cb0[6].x * r0.y;
  r0.y = dc_f_cb0[6].y * v1.y;
  r0.xy = r0.xy * float2(5,2) + dc_f_cb0[6].zw;
  r0.xyzw = _node_5030.Sample(sampler_node_5030, r0.xy).xyzw;
  r0.xyz = log2(r0.xyz);
  r0.xyz = dc_f_cb0[10].zzz * r0.xyz;
  r0.xyz = exp2(r0.xyz);
  r0.xyz = dc_f_cb0[7].yyy * r0.xyz;
  r0.xyz = dc_f_cb0[8].xyz * r0.xyz;
  r1.x = v1.y;
  r1.xy = r1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.xyzw = _node_331.Sample(sampler_node_331, r1.xy).xyzw;
  r1.xyz = log2(r1.xyz);
  r1.xyz = dc_f_cb0[4].yyy * r1.xyz;
  r1.xyz = exp2(r1.xyz);
  r1.xyz = dc_f_cb0[4].xxx * r1.xyz;
  r0.xyz = r1.xyz * dc_f_cb0[5].xyz + r0.xyz;
  r0.w = 1 + -v1.y;
  r0.w = abs(r0.w) * 6.66666698 + -1;
  r1.x = abs(v1.y) * 6.66666698 + -1;
  r0.w = r1.x * r0.w;
  r0.xyz = r0.xyz * r0.www;
  r0.w = 1.17647099 * abs(v1.x);
  r1.xyz = -abs(v1.xxx) * float3(1.17647099,1.17647099,1.17647099) + dc_f_cb0[9].xyz;
  r1.xyz = dc_f_cb0[10].xxx * r1.xyz + r0.www;
  r0.xyz = r1.xyz * r0.xyz;
  r0.xyz = v2.www * r0.xyz;
  o0.xyz = float3(0.5,0.5,0.5) * r0.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
