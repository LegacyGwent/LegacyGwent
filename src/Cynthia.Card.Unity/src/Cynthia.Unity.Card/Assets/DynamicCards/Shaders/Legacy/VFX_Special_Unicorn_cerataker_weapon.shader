Shader "DynamicCards/Legacy/VFX_Special_Unicorn_cerataker_weapon" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_texture_brightness("_texture_brightness",Float)=6.0
_node_192("_node_192",2D)="bump" {}
_distortion("_distortion",Float)=0.20000000298023224
_distortion_speed("_distortion_speed",Float)=0.20000000298023224
_distortion_scale("_distortion_scale",Float)=2.0
_main_colour("_main_colour",Color)=(0.1757137030363083,0.4779411852359772,0.3778933882713318,1.0)
_node_2200("_node_2200",2D)="white" {}
_mask("_mask",2D)="white" {}
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
SamplerState sampler_MainTex;
SamplerState sampler_mask;
SamplerState sampler_node_192;
SamplerState sampler_node_2200;
Texture2D<float4> _MainTex;
Texture2D<float4> _mask;
Texture2D<float4> _node_192;
Texture2D<float4> _node_2200;
float _distortion;
float _distortion_scale;
float _distortion_speed;
float _texture_brightness;
float4 _MainTex_ST;
float4 _TimeEditor;
float4 _main_colour;
float4 _mask_ST;
float4 _node_192_ST;
float4 _node_2200_ST;












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
dc_f_cb0[4]=float4(_texture_brightness,0,0,0);
dc_f_cb0[5]=float4(_node_192_ST.x,_node_192_ST.y,_node_192_ST.z,_node_192_ST.w);
dc_f_cb0[6]=float4(_distortion,_distortion_speed,_distortion_scale,0);
dc_f_cb0[7]=float4(_main_colour.x,_main_colour.y,_main_colour.z,_main_colour.w);
dc_f_cb0[8]=float4(_node_2200_ST.x,_node_2200_ST.y,_node_2200_ST.z,_node_2200_ST.w);
dc_f_cb0[9]=float4(_mask_ST.x,_mask_ST.y,_mask_ST.z,_mask_ST.w);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb0[6].z * v1.x;
  r0.z = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r0.w = r0.z * dc_f_cb0[6].y + v1.y;
  r1.x = r0.z * -0.0500000007 + v1.x;
  r0.y = dc_f_cb0[6].z * r0.w;
  r0.xy = r0.xy * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r0.xyzw = _node_192.Sample(sampler_node_192, r0.xy).xyzw;
  r0.xy = r0.wy * float2(2,2) + float2(-1,-1);
  r0.xy = dc_f_cb0[6].xx * r0.xy;
  r1.y = v1.y;
  r0.zw = r1.xy * float2(3,3) + r0.xy;
  r0.xy = v1.xy * float2(6,6) + r0.xy;
  r0.xy = r0.xy * dc_f_cb0[8].xy + dc_f_cb0[8].zw;
  r1.xyzw = _node_2200.Sample(sampler_node_2200, r0.xy).xyzw;
  r0.xy = r0.zw * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r2.xyz = r1.xyz * r1.xyz;
  r2.xyz = r2.xyz * r1.xyz;
  r0.xyz = r2.xyz * r0.xyz;
  r0.x = dot(r0.xyz, float3(0.300000012,0.589999974,0.109999999));
  r0.x = 0.5625 * r0.x;
  r0.xyz = r1.xyz * float3(0.100000001,0.100000001,0.100000001) + r0.xxx;
  r0.xyz = v2.xyz * r0.xyz;
  r0.xyz = v2.www * r0.xyz;
  r1.xyz = dc_f_cb0[7].xyz * dc_f_cb0[4].xxx;
  r0.xyz = r1.xyz * r0.xyz;
  r1.xy = v1.xy * dc_f_cb0[9].xy + dc_f_cb0[9].zw;
  r1.xyzw = _mask.Sample(sampler_mask, r1.xy).xyzw;
  o0.xyz = r1.xyz * r0.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
