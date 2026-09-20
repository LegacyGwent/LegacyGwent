Shader "DynamicCards/Legacy/VFX_Special_Unicorn_unicorn_detail_mesh" {
Properties {
_node_11("_node_11",2D)="white" {}
_colour("_colour",Color)=(0.32179930806159973,0.7058823108673096,0.4674859941005707,1.0)
_node_5196("_node_5196",2D)="white" {}
_brightness("_brightness",Float)=1.0
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
SamplerState sampler_node_11;
SamplerState sampler_node_5196;
Texture2D<float4> _node_11;
Texture2D<float4> _node_5196;
float _brightness;
float4 _TimeEditor;
float4 _colour;
float4 _node_11_ST;
float4 _node_5196_ST;












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
  uint v2 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_node_11_ST.x,_node_11_ST.y,_node_11_ST.z,_node_11_ST.w);
dc_f_cb0[4]=float4(_colour.x,_colour.y,_colour.z,_colour.w);
dc_f_cb0[5]=float4(_node_5196_ST.x,_node_5196_ST.y,_node_5196_ST.z,_node_5196_ST.w);
dc_f_cb0[6]=float4(_brightness,0,0,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r0.yz = r0.xx * float2(0.100000001,0.219999999) + v1.yy;
  r0.xw = v1.xx;
  r1.xy = dc_f_cb0[5].yx * r0.yx;
  r0.xy = r0.wz * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r0.xyzw = _node_11.Sample(sampler_node_11, r0.xy).xyzw;
  r1.z = 2 * r1.x;
  r1.xy = dc_f_cb0[5].zw + r1.yz;
  r1.xyzw = _node_5196.Sample(sampler_node_5196, r1.xy).xyzw;
  r2.xyz = r0.xyz * r0.xyz;
  r0.xyz = r0.xyz * r2.xyz + r1.xyz;
  r0.xyz = dc_f_cb0[4].xyz * r0.xyz;
  r0.xyz = float3(8,8,8) * r0.xyz;
  r1.xy = float2(1,1) + -v1.xy;
  r0.w = abs(v1.x) * abs(r1.x);
  r1.x = dot(abs(v1.yy), abs(r1.yy));
  r1.x = min(1, r1.x);
  r0.xyz = r0.xyz * r0.www;
  r0.xyz = r0.xyz * r1.xxx;
  o0.xyz = dc_f_cb0[6].xxx * r0.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
