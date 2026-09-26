Shader "DynamicCards/Legacy/VFX_Special_Unicorn_teleport_orange" {
Properties {
_node_4295("_node_4295",2D)="white" {}
_node_7181("_node_7181",Color)=(0.8235294222831726,0.4707314074039459,0.24221450090408325,1.0)
_dot("_dot",2D)="white" {}
_texture_speed("_texture_speed",Float)=0.20000000298023224
_node_9364("_node_9364",Float)=3.0
_brightness("_brightness",Float)=6.0
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
SamplerState sampler_node_4295;
Texture2D<float4> _dot;
Texture2D<float4> _node_4295;
float _brightness;
float _node_9364;
float _texture_speed;
float4 _TimeEditor;
float4 _dot_ST;
float4 _node_4295_ST;
float4 _node_7181;












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
dc_f_cb0[3]=float4(_node_4295_ST.x,_node_4295_ST.y,_node_4295_ST.z,_node_4295_ST.w);
dc_f_cb0[4]=float4(_node_7181.x,_node_7181.y,_node_7181.z,_node_7181.w);
dc_f_cb0[5]=float4(_dot_ST.x,_dot_ST.y,_dot_ST.z,_dot_ST.w);
dc_f_cb0[6]=float4(_texture_speed,_node_9364,_brightness,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r0.y = r0.x * dc_f_cb0[6].x + v1.y;
  r0.x = v1.x;
  r0.zw = r0.xy * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r0.xy = r0.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.xyzw = _node_4295.Sample(sampler_node_4295, r0.xy).xyzw;
  r0.x = dot(r1.xyz, float3(0.300000012,0.589999974,0.109999999));
  r1.xyzw = _dot.Sample(sampler_dot, r0.zw).xyzw;
  r0.yzw = r1.xyz * r1.xyz;
  r0.yzw = r1.xyz * r0.yzw;
  r0.yzw = r0.yzw + r0.yzw;
  r0.xyz = r0.xxx * r0.xxx + r0.yzw;
  r0.xyz = abs(v1.yyy) * r0.xyz;
  r0.w = 1 + -v1.y;
  r0.xyz = r0.xyz * abs(r0.www);
  r0.xyz = dc_f_cb0[6].zzz * r0.xyz;
  r0.xyz = dc_f_cb0[4].xyz * r0.xyz;
  r0.w = dc_f_cb0[6].y + abs(v1.y);
  r0.w = -2 + r0.w;
  o0.xyz = r0.xyz * r0.www;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
