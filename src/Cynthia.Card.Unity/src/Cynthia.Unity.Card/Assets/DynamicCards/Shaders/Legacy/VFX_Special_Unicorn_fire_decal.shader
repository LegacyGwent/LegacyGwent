Shader "DynamicCards/Legacy/VFX_Special_Unicorn_fire_decal" {
Properties {
_node_3140("_node_3140",2D)="white" {}
_node_3690("_node_3690",Color)=(0.5,0.5,0.5,1.0)
_speed01("_speed01",Float)=0.03999999910593033
_speed02("_speed02",Float)=-0.009999999776482582
_power01("_power01",Float)=4.0
_speed_v("_speed_v",Float)=0.0
_dark_sphere("_dark_sphere",Float)=0.4000000059604645
_mipmaps("_mipmaps",Float)=1.0
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
SamplerState sampler_node_3140;
Texture2D<float4> _node_3140;
float _dark_sphere;
float _mipmaps;
float _power01;
float _speed01;
float _speed02;
float _speed_v;
float4 _TimeEditor;
float4 _node_3140_ST;
float4 _node_3690;












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
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_node_3140_ST.x,_node_3140_ST.y,_node_3140_ST.z,_node_3140_ST.w);
dc_f_cb0[4]=float4(_node_3690.x,_node_3690.y,_node_3690.z,_node_3690.w);
dc_f_cb0[5]=float4(_speed01,_speed02,_power01,_speed_v);
dc_f_cb0[6]=float4(_dark_sphere,_mipmaps,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.w = v1.y;
  r1.x = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r0.xyz = r1.xxx * dc_f_cb0[5].xwy + v1.xyx;
  r0.zw = r0.zw * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r0.xy = dc_f_cb0[3].xy * r0.xy;
  r0.xy = r0.xy * float2(3,3) + dc_f_cb0[3].zw;
  r1.xyzw = _node_3140.SampleLevel(sampler_node_3140, r0.xy, dc_f_cb0[6].y).xyzw;
  r1.xyz = r1.xyz * r1.xyz;
  r0.xyzw = _node_3140.SampleLevel(sampler_node_3140, r0.zw, dc_f_cb0[6].y).xyzw;
  r0.xyz = log2(r0.xyz);
  r0.xyz = dc_f_cb0[5].zzz * r0.xyz;
  r0.xyz = exp2(r0.xyz);
  r0.xyz = r1.xyz * r0.xyz;
  r0.x = dot(r0.xyz, float3(0.300000012,0.589999974,0.109999999));
  r0.xyz = dc_f_cb0[4].xyz * r0.xxx;
  r1.xy = v1.xy * float2(2,2) + float2(-1,-1);
  r0.w = dot(r1.xy, r1.xy);
  r0.w = sqrt(r0.w);
  r0.w = r0.w * 2 + -1;
  r0.w = saturate(1 + -r0.w);
  r0.xyz = r0.www * r0.xyz;
  r0.w = r0.w * r0.w;
  r0.xyz = float3(22,22,22) * r0.xyz;
  r1.x = dot(r0.xyz, r0.www);
  o0.xyz = r0.xyz;
  o0.w = r0.w * dc_f_cb0[6].x + r1.x;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
