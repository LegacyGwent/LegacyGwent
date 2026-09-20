Shader "DynamicCards/Legacy/VFX_Special_Unicorn_unicorn_fx" {
Properties {
_texture("_texture",2D)="white" {}
_node_9182("_node_9182",Color)=(0.2973616123199463,0.7352941036224365,0.5540807247161865,1.0)
_node_340("_node_340",2D)="white" {}
_node_214("_node_214",Color)=(0.654411792755127,0.48344171047210693,0.11548440158367157,1.0)
_colour("_colour",Float)=4.0
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Front
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
SamplerState sampler_node_340;
SamplerState sampler_texture;
Texture2D<float4> _node_340;
Texture2D<float4> _texture;
float _colour;
float4 _TimeEditor;
float4 _node_214;
float4 _node_340_ST;
float4 _node_9182;
float4 _texture_ST;












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
float4 dc_f_cb0[8];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_texture_ST.x,_texture_ST.y,_texture_ST.z,_texture_ST.w);
dc_f_cb0[4]=float4(_node_9182.x,_node_9182.y,_node_9182.z,_node_9182.w);
dc_f_cb0[5]=float4(_node_340_ST.x,_node_340_ST.y,_node_340_ST.z,_node_340_ST.w);
dc_f_cb0[6]=float4(_node_214.x,_node_214.y,_node_214.z,_node_214.w);
dc_f_cb0[7]=float4(_colour,0,0,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r0.xyzw = r0.xxxx * float4(-0.0599999987,-0.0900000036,0.00999999978,0.0199999996) + v1.xxyy;
  r0.w = dc_f_cb0[3].y * r0.w;
  r1.y = 1.5 * r0.w;
  r0.z = dc_f_cb0[5].y * r0.z;
  r2.y = 4 * r0.z;
  r1.x = dc_f_cb0[3].x * r0.y;
  r2.x = dc_f_cb0[5].x * r0.x;
  r0.xy = dc_f_cb0[5].zw + r2.xy;
  r0.xyzw = _node_340.Sample(sampler_node_340, r0.xy).xyzw;
  r0.xyz = log2(r0.xyz);
  r0.xyz = float3(1.5,1.5,1.5) * r0.xyz;
  r0.xyz = exp2(r0.xyz);
  r0.xyz = dc_f_cb0[6].xyz * r0.xyz;
  r1.xy = dc_f_cb0[3].zw + r1.xy;
  r1.xyzw = _texture.Sample(sampler_texture, r1.xy).xyzw;
  r1.xyz = log2(r1.xyz);
  r1.xyz = float3(2.5,2.5,2.5) * r1.xyz;
  r1.xyz = exp2(r1.xyz);
  r2.xyz = dc_f_cb0[7].xxx * dc_f_cb0[4].xyz;
  r1.xyz = r2.xyz * r1.xyz;
  r0.xyz = r0.xyz * float3(4,4,4) + r1.xyz;
  r0.xyz = abs(v1.yyy) * r0.xyz;
  r0.w = 1 + -v1.y;
  r0.w = saturate(abs(r0.w) * 6 + -1);
  o0.xyz = r0.xyz * r0.www;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
