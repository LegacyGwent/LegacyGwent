Shader "DynamicCards/Native/VFX_Special_Unicorn_meshroom_det_m" {
Properties {
_node_4315("_node_4315",2D)="white" {}
_node_2873("_node_2873",Color)=(0.5,0.5,0.5,1.0)
_colour_multiply("_colour_multiply",Float)=0.5
_smoke_speed("_smoke_speed",Float)=-0.10000000149011612
_dot_text("_dot_text",2D)="white" {}
_dot_multiply("_dot_multiply",Float)=2.0
_dot_speed("_dot_speed",Float)=-0.10000000149011612
_turbulence("_turbulence",2D)="white" {}
_turbulence_strech("_turbulence_strech",Float)=12.0
_turbulence_speed("_turbulence_speed",Float)=-0.20000000298023224
_smoke_colour_multiply("_smoke_colour_multiply",Float)=0.6000000238418579
_texture_offset("_texture_offset",Float)=1.0
_turbulenceofset("_turbulenceofset",Float)=1.0
_mipmap("_mipmap",Float)=1.0
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
SamplerState sampler_dot_text;
SamplerState sampler_node_4315;
SamplerState sampler_turbulence;
Texture2D<float4> _dot_text;
Texture2D<float4> _node_4315;
Texture2D<float4> _turbulence;
float _colour_multiply;
float _dot_multiply;
float _dot_speed;
float _mipmap;
float _smoke_colour_multiply;
float _smoke_speed;
float _texture_offset;
float _turbulence_speed;
float _turbulence_strech;
float _turbulenceofset;
float4 _TimeEditor;
float4 _dot_text_ST;
float4 _node_2873;
float4 _node_4315_ST;
float4 _turbulence_ST;




















void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0)
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
float4 dc_v_cb0[11];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(0,0,0,0);
dc_v_cb0[6]=float4(0,0,0,0);
dc_v_cb0[7]=float4(0,0,0,0);
dc_v_cb0[8]=float4(_turbulence_ST.x,_turbulence_ST.y,_turbulence_ST.z,_turbulence_ST.w);
dc_v_cb0[9]=float4(_turbulence_strech,_turbulence_speed,0,0);
dc_v_cb0[10]=float4(_turbulenceofset,0,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_v_cb1[0].y + dc_v_cb0[2].y;
  r0.x = r0.x * dc_v_cb0[9].y + v1.y;
  r0.y = r0.x * 3 + dc_v_cb0[10].x;
  r0.x = v1.x;
  r0.xy = r0.xy * dc_v_cb0[8].xy + dc_v_cb0[8].zw;
  r0.xyzw = _turbulence.SampleLevel(sampler_turbulence, r0.xy, 0).xyzw;
  r0.x = r0.z * 2 + -1;
  r0.x = abs(v1.y) * r0.x;
  r0.xyz = dc_v_cb0[9].xxx * r0.xxx + v0.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
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
float4 dc_f_cb0[11];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_node_4315_ST.x,_node_4315_ST.y,_node_4315_ST.z,_node_4315_ST.w);
dc_f_cb0[4]=float4(_node_2873.x,_node_2873.y,_node_2873.z,_node_2873.w);
dc_f_cb0[5]=float4(_colour_multiply,_smoke_speed,0,0);
dc_f_cb0[6]=float4(_dot_text_ST.x,_dot_text_ST.y,_dot_text_ST.z,_dot_text_ST.w);
dc_f_cb0[7]=float4(_dot_multiply,_dot_speed,0,0);
dc_f_cb0[8]=float4(0,0,0,0);
dc_f_cb0[9]=float4(0,0,_smoke_colour_multiply,_texture_offset);
dc_f_cb0[10]=float4(0,_mipmap,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb0[6].x * v1.x;
  r0.z = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r0.w = r0.z * dc_f_cb0[7].y + v1.y;
  r0.z = r0.z * dc_f_cb0[5].y + v1.y;
  r1.y = dc_f_cb0[9].w + r0.z;
  r0.y = dc_f_cb0[6].y * r0.w;
  r0.xy = r0.xy * float2(6,12) + dc_f_cb0[6].zw;
  r0.xyzw = _dot_text.SampleLevel(sampler_dot_text, r0.xy, dc_f_cb0[10].y).xyzw;
  r0.xyz = log2(r0.xyz);
  r0.xyz = float3(1.29999995,1.29999995,1.29999995) * r0.xyz;
  r0.xyz = exp2(r0.xyz);
  r0.xyz = dc_f_cb0[7].xxx * r0.xyz;
  r1.x = v1.x;
  r1.xy = r1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.xyzw = _node_4315.Sample(sampler_node_4315, r1.xy).xyzw;
  r0.xyz = r1.xyz * r0.xyz;
  r0.xyz = r1.xyz * dc_f_cb0[9].zzz + r0.xyz;
  r1.xyz = dc_f_cb0[5].xxx * dc_f_cb0[4].xyz;
  r0.xyz = r1.xyz * r0.xyz;
  r0.w = 1 + -v1.y;
  r0.xyz = r0.xyz * abs(r0.www);
  r0.w = 4 * abs(v1.y);
  r0.w = min(1, r0.w);
  o0.xyz = r0.xyz * r0.www;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
