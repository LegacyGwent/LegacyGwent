Shader "DynamicCards/Legacy/VFX_Special_Unicorn_the_last_wish_glow" {
Properties {
_colour_fresnel("_colour_fresnel",Color)=(0.5,0.5,0.5,1.0)
_colour_multiply("_colour_multiply",Float)=12.0
_colour_main("_colour_main",Color)=(0.5,0.5,0.5,1.0)
_fresnel_shelf("_fresnel_shelf",Float)=1.0
_cubemap("_cubemap",Cube)="_Skybox" {}
_cube_multiply("_cube_multiply",Float)=2.0
_cube_power("_cube_power",Float)=2.0
_colour_cube_map("_colour_cube_map",Color)=(0.5,0.5,0.5,1.0)
_noise_vertex("_noise_vertex",2D)="white" {}
_texture1("_texture1",2D)="white" {}
_texture2("_texture2",2D)="white" {}
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
SamplerState sampler_cubemap;
SamplerState sampler_noise_vertex;
SamplerState sampler_texture1;
SamplerState sampler_texture2;
Texture2D<float4> _noise_vertex;
Texture2D<float4> _texture1;
Texture2D<float4> _texture2;
TextureCube<float4> _cubemap;
float _colour_multiply;
float _cube_multiply;
float _cube_power;
float _fresnel_shelf;
float4 _TimeEditor;
float4 _colour_cube_map;
float4 _colour_fresnel;
float4 _colour_main;
float4 _noise_vertex_ST;
float4 _texture1_ST;
float4 _texture2_ST;




















void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float2 v2 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float3 o3 : TEXCOORD2)
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
float4 dc_v_cb2[7];
dc_v_cb2[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb2[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb2[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb2[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
dc_v_cb2[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_v_cb2[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_v_cb2[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);
float4 dc_v_cb1[1];
dc_v_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_v_cb0[9];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(0,0,0,0);
dc_v_cb0[6]=float4(0,0,0,0);
dc_v_cb0[7]=float4(0,0,0,0);
dc_v_cb0[8]=float4(_noise_vertex_ST.x,_noise_vertex_ST.y,_noise_vertex_ST.z,_noise_vertex_ST.w);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_v_cb1[0].y + dc_v_cb0[2].y;
  r0.x = r0.x * 8 + v2.x;
  r0.y = v2.y;
  r0.xy = r0.xy * dc_v_cb0[8].xy + dc_v_cb0[8].zw;
  r0.xyzw = _noise_vertex.SampleLevel(sampler_noise_vertex, r0.xy, 0).xyzw;
  r0.xyz = r0.xyz * float3(0.600000024,0.600000024,0.600000024) + v0.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r1.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  o2.xyzw = dc_v_cb2[3].xyzw * v0.wwww + r0.xyzw;
  r0.xyzw = dc_v_cb3[18].xyzw * r1.yyyy;
  r0.xyzw = dc_v_cb3[17].xyzw * r1.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb3[19].xyzw * r1.zzzz + r0.xyzw;
  o0.xyzw = dc_v_cb3[20].xyzw * r1.wwww + r0.xyzw;
  o1.xy = v2.xy;
  r0.x = dot(v1.xyz, dc_v_cb2[4].xyz);
  r0.y = dot(v1.xyz, dc_v_cb2[5].xyz);
  r0.z = dot(v1.xyz, dc_v_cb2[6].xyz);
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  o3.xyz = r0.xyz * r0.www;
  return;
}
























void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float3 v3 : TEXCOORD2,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[5];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(_WorldSpaceCameraPos.x,_WorldSpaceCameraPos.y,_WorldSpaceCameraPos.z,0);
float4 dc_f_cb0[11];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_colour_fresnel.x,_colour_fresnel.y,_colour_fresnel.z,_colour_fresnel.w);
dc_f_cb0[4]=float4(_colour_multiply,0,0,0);
dc_f_cb0[5]=float4(_colour_main.x,_colour_main.y,_colour_main.z,_colour_main.w);
dc_f_cb0[6]=float4(_fresnel_shelf,_cube_multiply,_cube_power,0);
dc_f_cb0[7]=float4(_colour_cube_map.x,_colour_cube_map.y,_colour_cube_map.z,_colour_cube_map.w);
dc_f_cb0[8]=float4(0,0,0,0);
dc_f_cb0[9]=float4(_texture1_ST.x,_texture1_ST.y,_texture1_ST.z,_texture1_ST.w);
dc_f_cb0[10]=float4(_texture2_ST.x,_texture2_ST.y,_texture2_ST.z,_texture2_ST.w);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r0.xyzw = r0.xxxx * float4(0.0500000007,0.0500000007,-0.0399999991,-0.0399999991) + v1.xyxy;
  r0.xy = dc_f_cb0[9].xy * r0.xy;
  r0.zw = dc_f_cb0[10].xy * r0.zw;
  r0.zw = r0.zw * float2(2,2) + dc_f_cb0[10].zw;
  r1.xyzw = _texture2.Sample(sampler_texture2, r0.zw).xyzw;
  r0.xy = r0.xy * float2(1.5,1.5) + dc_f_cb0[9].zw;
  r0.xyzw = _texture1.Sample(sampler_texture1, r0.xy).xyzw;
  r0.xyz = r0.xyz * r1.xyz;
  r0.xyz = float3(4,4,4) * r0.xyz;
  r0.xyz = log2(r0.xyz);
  r0.xyz = float3(0.800000012,0.800000012,0.800000012) * r0.xyz;
  r0.xyz = exp2(r0.xyz);
  r1.xyz = dc_f_cb1[4].xyz + -v2.xyz;
  r0.w = dot(r1.xyz, r1.xyz);
  r0.w = rsqrt(r0.w);
  r1.xyz = r1.xyz * r0.www;
  r0.w = dot(v3.xyz, v3.xyz);
  r0.w = rsqrt(r0.w);
  r2.xyz = v3.xyz * r0.www;
  r0.w = dot(-r1.xyz, r2.xyz);
  r0.w = r0.w + r0.w;
  r3.xyz = r2.xyz * -r0.www + -r1.xyz;
  r0.w = dot(r2.xyz, r1.xyz);
  r0.w = max(0, r0.w);
  r0.w = 1 + -r0.w;
  r1.xyzw = _cubemap.Sample(sampler_cubemap, r3.xyz).xyzw;
  r2.xyz = dc_f_cb0[7].xyz * dc_f_cb0[6].yyy;
  r1.xyz = r2.xyz * r1.xyz;
  r1.xyz = log2(r1.xyz);
  r1.xyz = dc_f_cb0[6].zzz * r1.xyz;
  r1.xyz = exp2(r1.xyz);
  r0.xyz = r1.xyz + r0.xyz;
  r1.x = log2(r0.w);
  r0.w = rsqrt(r0.w);
  o0.w = 1 / r0.w;
  r0.w = dc_f_cb0[6].x * r1.x;
  r0.w = exp2(r0.w);
  r1.xyz = dc_f_cb0[4].xxx * dc_f_cb0[3].xyz;
  r1.xyz = r1.xyz * r0.www;
  o0.xyz = r0.xyz * dc_f_cb0[5].xyz + r1.xyz;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
