Shader "DynamicCards/Legacy/VFX_Effects_ImageLayerShaderFakeLightTop" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Light("_Light",2D)="white" {}
_LightIntensity("_LightIntensity",Float)=1.0
__Turbulence("__Turbulence",2D)="white" {}
_Speed("_Speed",Float)=1.0
_node_357("_node_357",Color)=(1.0,1.0,1.0,1.0)
_Vertex_Color("_Vertex_Color",Float)=0.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Back
ZWrite Off
ZTest Always
Blend One One
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_Light;
SamplerState sampler_MainTex;
SamplerState sampler__Turbulence;
Texture2D<float4> _Light;
Texture2D<float4> _MainTex;
Texture2D<float4> __Turbulence;
float _LightIntensity;
float _Speed;
float _Vertex_Color;
float4 _Light_ST;
float4 _MainTex_ST;
float4 _TimeEditor;
float4 __Turbulence_ST;
float4 _node_357;












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
float4 dc_f_cb0[10];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[4]=float4(_Light_ST.x,_Light_ST.y,_Light_ST.z,_Light_ST.w);
dc_f_cb0[5]=float4(_LightIntensity,0,0,0);
dc_f_cb0[6]=float4(__Turbulence_ST.x,__Turbulence_ST.y,__Turbulence_ST.z,__Turbulence_ST.w);
dc_f_cb0[7]=float4(_Speed,0,0,0);
dc_f_cb0[8]=float4(_node_357.x,_node_357.y,_node_357.z,_node_357.w);
dc_f_cb0[9]=float4(_Vertex_Color,0,0,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r0.y = 0.5 + dc_f_cb0[7].x;
  r0.x = r0.y * r0.x;
  r0.xy = r0.xx * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r0.xyzw = __Turbulence.Sample(sampler__Turbulence, r0.xy).xyzw;
  r0.yz = v1.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r1.xyzw = _Light.Sample(sampler_Light, r0.yz).xyzw;
  r0.yzw = dc_f_cb0[5].xxx * r1.xyz;
  r1.xy = v1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r1.xy).xyzw;
  r0.xyz = r0.yzw * r0.xxx + r1.xyz;
  o0.w = r1.w;
  r1.xyz = dc_f_cb0[8].xyz * v2.xyz;
  r2.xyz = -v2.xyz * dc_f_cb0[8].xyz + dc_f_cb0[8].xyz;
  r1.xyz = dc_f_cb0[9].xxx * r2.xyz + r1.xyz;
  o0.xyz = r1.xyz * r0.xyz;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
