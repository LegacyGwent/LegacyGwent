Shader "DynamicCards/Legacy/VFX_Special_Unicorn_lacerate_trail" {
Properties {
_node_7946("_node_7946",Color)=(0.970588207244873,0.7953705191612244,0.3354237973690033,1.0)
_node_7207("_node_7207",2D)="white" {}
_node_8470("_node_8470",2D)="white" {}
_colour_multiply("_colour_multiply",Float)=1.5
_dot_multiply("_dot_multiply",Float)=5.0
_s("_s",Float)=3.0
_speed("_speed",Float)=0.10000000149011612
_speed2("_speed2",Float)=0.33000001311302185
_distortion("_distortion",2D)="white" {}
_distortion_value("_distortion_value",Float)=0.11999999731779099
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
SamplerState sampler_distortion;
SamplerState sampler_node_7207;
SamplerState sampler_node_8470;
Texture2D<float4> _distortion;
Texture2D<float4> _node_7207;
Texture2D<float4> _node_8470;
float _colour_multiply;
float _distortion_value;
float _dot_multiply;
float _s;
float _speed2;
float _speed;
float4 _TimeEditor;
float4 _distortion_ST;
float4 _node_7207_ST;
float4 _node_7946;
float4 _node_8470_ST;












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
float4 dc_f_cb0[10];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_node_7946.x,_node_7946.y,_node_7946.z,_node_7946.w);
dc_f_cb0[4]=float4(_node_7207_ST.x,_node_7207_ST.y,_node_7207_ST.z,_node_7207_ST.w);
dc_f_cb0[5]=float4(_node_8470_ST.x,_node_8470_ST.y,_node_8470_ST.z,_node_8470_ST.w);
dc_f_cb0[6]=float4(_colour_multiply,_dot_multiply,_s,_speed);
dc_f_cb0[7]=float4(_speed2,0,0,0);
dc_f_cb0[8]=float4(_distortion_ST.x,_distortion_ST.y,_distortion_ST.z,_distortion_ST.w);
dc_f_cb0[9]=float4(_distortion_value,0,0,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = 1.5 * abs(v1.x);
  r0.yz = float2(1,1) + -v1.yx;
  r0.y = saturate(r0.y + r0.y);
  r0.z = 5 * abs(r0.z);
  r0.xz = min(float2(1,1), r0.xz);
  r0.y = abs(v1.y) * r0.y;
  r0.x = r0.y * r0.x;
  r0.x = r0.x * r0.z;
  r0.y = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r0.zw = r0.yy * float2(0.0500000007,0.0500000007) + v1.xy;
  r0.zw = r0.zw * dc_f_cb0[8].xy + dc_f_cb0[8].zw;
  r1.xyzw = _distortion.Sample(sampler_distortion, r0.zw).xyzw;
  r0.zw = r1.xx * dc_f_cb0[9].xx + v1.xy;
  r1.x = r0.y * dc_f_cb0[6].w + r0.z;
  r0.y = r0.y * dc_f_cb0[7].x + r0.z;
  r1.x = dc_f_cb0[4].x * r1.x;
  r1.y = dc_f_cb0[4].y * r0.w;
  r2.xy = dc_f_cb0[5].xy * r0.yw;
  r0.yz = r2.xy * float2(1.5,1.5) + dc_f_cb0[5].zw;
  r2.xyzw = _node_8470.Sample(sampler_node_8470, r0.yz).xyzw;
  r0.yzw = log2(r2.xyz);
  r0.yzw = dc_f_cb0[6].zzz * r0.yzw;
  r0.yzw = exp2(r0.yzw);
  r0.yzw = dc_f_cb0[6].yyy * r0.yzw;
  r1.xy = r1.xy * float2(2,0.5) + dc_f_cb0[4].zw;
  r1.xyzw = _node_7207.Sample(sampler_node_7207, r1.xy).xyzw;
  r1.xyz = r1.xyz * r0.xxx + r0.xxx;
  r2.xyz = r1.xyz * r1.xyz;
  r1.xyz = r2.xyz * r1.xyz;
  r0.xyz = r0.yzw * r1.xyz + r1.xyz;
  r1.xyz = dc_f_cb0[6].xxx * dc_f_cb0[3].xyz;
  r0.xyz = r1.xyz * r0.xyz;
  r0.xyz = log2(r0.xyz);
  r0.xyz = float3(1.20000005,1.20000005,1.20000005) * r0.xyz;
  o0.xyz = exp2(r0.xyz);
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
