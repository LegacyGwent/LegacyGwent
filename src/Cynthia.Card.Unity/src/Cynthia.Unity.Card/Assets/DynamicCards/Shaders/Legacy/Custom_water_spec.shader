Shader "DynamicCards/Legacy/Custom_water_spec" {
Properties {
_MainTex("_MainTex",2D)="gray" {}
_colour("_colour",Color)=(0.2867647111415863,0.016868509352207184,0.016868509352207184,1.0)
_alpcha_multiply("_alpcha_multiply",Float)=2.0
_cubemap("_cubemap",Cube)="_Skybox" {}
_normal_map("_normal_map",2D)="black" {}
_colour_spec("_colour_spec",Color)=(0.6397058963775635,0.23518599569797516,0.23518599569797516,1.0)
_cube_power("_cube_power",Float)=6.0
_cube_brightness("_cube_brightness",Float)=8.0
_mipmaps("_mipmaps",Float)=1.0
_Alpha("_Alpha",Float)=1.0
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
SamplerState sampler_MainTex;
SamplerState sampler_cubemap;
SamplerState sampler_normal_map;
Texture2D<float4> _MainTex;
Texture2D<float4> _normal_map;
TextureCube<float4> _cubemap;
float _Alpha;
float _alpcha_multiply;
float _cube_brightness;
float _cube_power;
float _mipmaps;
float4 _MainTex_ST;
float4 _colour;
float4 _colour_spec;
float4 _normal_map_ST;












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
float4 dc_f_cb0[8];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_colour.x,_colour.y,_colour.z,_colour.w);
dc_f_cb0[4]=float4(_alpcha_multiply,0,0,0);
dc_f_cb0[5]=float4(_normal_map_ST.x,_normal_map_ST.y,_normal_map_ST.z,_normal_map_ST.w);
dc_f_cb0[6]=float4(_colour_spec.x,_colour_spec.y,_colour_spec.z,_colour_spec.w);
dc_f_cb0[7]=float4(_cube_power,_cube_brightness,_mipmaps,_Alpha);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r0.xyzw = _normal_map.SampleLevel(sampler_normal_map, r0.xy, dc_f_cb0[7].z).xyzw;
  r0.xyzw = _cubemap.Sample(sampler_cubemap, r0.xyz).xyzw;
  r0.xyz = log2(r0.xyz);
  r0.xyz = dc_f_cb0[7].xxx * r0.xyz;
  r0.xyz = exp2(r0.xyz);
  r0.xyz = dc_f_cb0[7].yyy * r0.xyz;
  r0.xyz = dc_f_cb0[6].xyz * r0.xyz;
  r1.xy = v1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r1.xyzw = _MainTex.SampleLevel(sampler_MainTex, r1.xy, dc_f_cb0[7].z).xyzw;
  r0.w = dot(r1.xyz, float3(0.300000012,0.589999974,0.109999999));
  o0.xyz = dc_f_cb0[3].xyz * r0.www + r0.xyz;
  r0.x = v2.w * dc_f_cb0[7].w + -1;
  r0.x = r0.x + r1.w;
  o0.w = dc_f_cb0[4].x * r0.x;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
