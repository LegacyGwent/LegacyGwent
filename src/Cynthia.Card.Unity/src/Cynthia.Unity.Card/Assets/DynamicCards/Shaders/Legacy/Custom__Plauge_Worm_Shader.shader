Shader "DynamicCards/Legacy/Custom__Plauge_Worm_Shader" {
Properties {
_Texture_Diffuse("_Texture_Diffuse",2D)="white" {}
_UVangleRot_Diffuse("_UVangleRot_Diffuse",Float)=0.0
_NoiseScale_Diffuse("_NoiseScale_Diffuse",Float)=1.0
_Time_UV_offsetMultiplier_Diffuse("_Time_UV_offsetMultiplier_Diffuse",Float)=1.0
_Texture_Vertex_Displace("_Texture_Vertex_Displace",2D)="white" {}
_UVangleRot("_UVangleRot",Float)=0.0
_Tex_vertex_Distortion("_Tex_vertex_Distortion",2D)="white" {}
_NoiseScale("_NoiseScale",Float)=1.0
_noise_str("_noise_str",Float)=0.0
_Minimum_Size_Tweak("_Minimum_Size_Tweak",Float)=0.0
_Size_Multiplier("_Size_Multiplier",Float)=0.0
_test_color("_test_color",Color)=(0.5,0.5,0.5,1.0)
_FakeLightDirection("_FakeLightDirection",Vector)=(0.0,0.0,0.0,0.0)
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
SamplerState sampler_Tex_vertex_Distortion;
SamplerState sampler_Texture_Diffuse;
SamplerState sampler_Texture_Vertex_Displace;
Texture2D<float4> _Tex_vertex_Distortion;
Texture2D<float4> _Texture_Diffuse;
Texture2D<float4> _Texture_Vertex_Displace;
float _Minimum_Size_Tweak;
float _NoiseScale;
float _NoiseScale_Diffuse;
float _Size_Multiplier;
float _Time_UV_offsetMultiplier_Diffuse;
float _UVangleRot;
float _UVangleRot_Diffuse;
float _noise_str;
float4 _FakeLightDirection;
float4 _Tex_vertex_Distortion_ST;
float4 _Texture_Diffuse_ST;
float4 _Texture_Vertex_Displace_ST;
float4 _TimeEditor;






















void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float2 v2 : TEXCOORD0,
  float4 v3 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float3 o2 : TEXCOORD1,
  out float4 o3 : COLOR0)
{
float4 dc_v_cb2[21];
dc_v_cb2[0]=float4(0,0,0,0);
dc_v_cb2[1]=float4(0,0,0,0);
dc_v_cb2[2]=float4(0,0,0,0);
dc_v_cb2[3]=float4(0,0,0,0);
dc_v_cb2[4]=float4(0,0,0,0);
dc_v_cb2[5]=float4(0,0,0,0);
dc_v_cb2[6]=float4(0,0,0,0);
dc_v_cb2[7]=float4(0,0,0,0);
dc_v_cb2[8]=float4(0,0,0,0);
dc_v_cb2[9]=float4(0,0,0,0);
dc_v_cb2[10]=float4(0,0,0,0);
dc_v_cb2[11]=float4(0,0,0,0);
dc_v_cb2[12]=float4(0,0,0,0);
dc_v_cb2[13]=float4(0,0,0,0);
dc_v_cb2[14]=float4(0,0,0,0);
dc_v_cb2[15]=float4(0,0,0,0);
dc_v_cb2[16]=float4(0,0,0,0);
dc_v_cb2[17]=float4(unity_MatrixVP[0][0],unity_MatrixVP[1][0],unity_MatrixVP[2][0],unity_MatrixVP[3][0]);
dc_v_cb2[18]=float4(unity_MatrixVP[0][1],unity_MatrixVP[1][1],unity_MatrixVP[2][1],unity_MatrixVP[3][1]);
dc_v_cb2[19]=float4(unity_MatrixVP[0][2],unity_MatrixVP[1][2],unity_MatrixVP[2][2],unity_MatrixVP[3][2]);
dc_v_cb2[20]=float4(unity_MatrixVP[0][3],unity_MatrixVP[1][3],unity_MatrixVP[2][3],unity_MatrixVP[3][3]);
float4 dc_v_cb1[7];
dc_v_cb1[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb1[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb1[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb1[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
dc_v_cb1[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_v_cb1[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_v_cb1[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);
float4 dc_v_cb0[7];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(_Texture_Vertex_Displace_ST.x,_Texture_Vertex_Displace_ST.y,_Texture_Vertex_Displace_ST.z,_Texture_Vertex_Displace_ST.w);
dc_v_cb0[4]=float4(_UVangleRot,0,0,0);
dc_v_cb0[5]=float4(_Tex_vertex_Distortion_ST.x,_Tex_vertex_Distortion_ST.y,_Tex_vertex_Distortion_ST.z,_Tex_vertex_Distortion_ST.w);
dc_v_cb0[6]=float4(_NoiseScale,_noise_str,_Minimum_Size_Tweak,_Size_Multiplier);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = 0.0174532924 * dc_v_cb0[4].x;
  sincos(r0.x, r0.x, r1.x);
  r2.z = r0.x;
  r0.yz = float2(-0.5,-0.5) + v2.xy;
  r2.y = r1.x;
  r2.x = -r0.x;
  r1.y = dot(r0.yz, r2.xy);
  r1.x = dot(r0.yz, r2.yz);
  r0.xy = float2(0.5,0.5) + r1.xy;
  r0.zw = dc_v_cb0[6].xx * v2.xy;
  r0.zw = r0.zw * dc_v_cb0[5].xy + dc_v_cb0[5].zw;
  r1.xyzw = _Tex_vertex_Distortion.SampleLevel(sampler_Tex_vertex_Distortion, r0.zw, 0).xyzw;
  r0.xy = dc_v_cb0[6].yy * r1.xx + r0.xy;
  r0.xy = r0.xy * dc_v_cb0[3].xy + dc_v_cb0[3].zw;
  r0.xyzw = _Texture_Vertex_Displace.SampleLevel(sampler_Texture_Vertex_Displace, r0.xy, 0).xyzw;
  r0.x = dc_v_cb0[6].w * r0.x;
  r0.y = 1 + -dc_v_cb0[6].z;
  r0.x = r0.x * r0.y + dc_v_cb0[6].z;
  r0.xyz = r0.xxx * v1.xyz + v0.xyz;
  r1.xyzw = dc_v_cb1[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb1[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb1[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb1[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb2[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb2[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb2[20].xyzw * r0.wwww + r1.xyzw;
  o1.xy = v2.xy;
  r0.x = dot(v1.xyz, dc_v_cb1[4].xyz);
  r0.y = dot(v1.xyz, dc_v_cb1[5].xyz);
  r0.z = dot(v1.xyz, dc_v_cb1[6].xyz);
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  o2.xyz = r0.xyz * r0.www;
  o3.xyzw = v3.xyzw;
  return;
}
























void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float3 v2 : TEXCOORD1,
  float4 v3 : COLOR0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[10];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_Texture_Vertex_Displace_ST.x,_Texture_Vertex_Displace_ST.y,_Texture_Vertex_Displace_ST.z,_Texture_Vertex_Displace_ST.w);
dc_f_cb0[4]=float4(_UVangleRot,0,0,0);
dc_f_cb0[5]=float4(_Tex_vertex_Distortion_ST.x,_Tex_vertex_Distortion_ST.y,_Tex_vertex_Distortion_ST.z,_Tex_vertex_Distortion_ST.w);
dc_f_cb0[6]=float4(_NoiseScale,_noise_str,0,0);
dc_f_cb0[7]=float4(_Texture_Diffuse_ST.x,_Texture_Diffuse_ST.y,_Texture_Diffuse_ST.z,_Texture_Diffuse_ST.w);
dc_f_cb0[8]=float4(_UVangleRot_Diffuse,_NoiseScale_Diffuse,_Time_UV_offsetMultiplier_Diffuse,0);
dc_f_cb0[9]=float4(_FakeLightDirection.x,_FakeLightDirection.y,_FakeLightDirection.z,_FakeLightDirection.w);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb1[0].x + dc_f_cb0[2].x;
  r0.x = r0.x * dc_f_cb0[8].z + v1.y;
  r0.y = dc_f_cb0[8].y * r0.x;
  r0.x = dc_f_cb0[8].y * v1.x;
  r0.xy = float2(-0.5,-0.5) + r0.xy;
  r0.z = 0.0174532924 * dc_f_cb0[8].x;
  sincos(r0.z, r1.x, r2.x);
  r3.z = r1.x;
  r3.y = r2.x;
  r3.x = -r1.x;
  r1.y = dot(r0.xy, r3.xy);
  r1.x = dot(r0.xy, r3.yz);
  r0.xy = float2(0.5,0.5) + r1.xy;
  r0.xy = r0.xy * dc_f_cb0[7].xy + dc_f_cb0[7].zw;
  r0.xyzw = _Texture_Diffuse.Sample(sampler_Texture_Diffuse, r0.xy).yzwx;
  r1.x = cmp(r0.x >= r0.y);
  r1.x = r1.x ? 1.000000 : 0;
  r2.xy = r0.yx;
  r3.xy = -r2.xy + r0.xy;
  r2.zw = float2(-1,0.666666687);
  r3.zw = float2(1,-1);
  r1.xyzw = r1.xxxx * r3.xyzw + r2.xyzw;
  r2.x = cmp(r0.w >= r1.x);
  r2.x = r2.x ? 1.000000 : 0;
  r0.xyz = r1.xyw;
  r1.xyw = r0.wyx;
  r1.xyzw = r1.xyzw + -r0.xyzw;
  r0.xyzw = r2.xxxx * r1.xyzw + r0.xyzw;
  r1.x = min(r0.w, r0.y);
  r1.x = -r1.x + r0.x;
  r1.y = r1.x * 6 + 1.00000001e-10;
  r0.y = r0.w + -r0.y;
  r0.y = r0.y / r1.y;
  r0.y = r0.z + r0.y;
  r0.yzw = float3(0,-0.333333343,0.333333343) + abs(r0.yyy);
  r0.yzw = frac(r0.yzw);
  r0.yzw = -r0.yzw * float3(2,2,2) + float3(1,1,1);
  r0.yzw = saturate(abs(r0.yzw) * float3(3,3,3) + float3(-1,-1,-1));
  r0.yzw = float3(-1,-1,-1) + r0.yzw;
  r1.y = 1.00000001e-10 + r0.x;
  r0.x = r0.x + r0.x;
  r0.x = r0.x * r0.x;
  r1.x = r1.x / r1.y;
  r0.yzw = r1.xxx * r0.yzw + float3(1,1,1);
  r0.xyz = r0.yzw * r0.xxx;
  r0.w = dot(v2.xyz, dc_f_cb0[9].xyz);
  r0.w = r0.w * 0.0500000007 + 0.0500000007;
  r0.w = r0.w + r0.w;
  o0.xyz = v3.xyz * r0.xyz + r0.www;
  r0.x = 0.0174532924 * dc_f_cb0[4].x;
  sincos(r0.x, r0.x, r1.x);
  r2.z = r0.x;
  r0.yz = float2(-0.5,-0.5) + v1.xy;
  r2.y = r1.x;
  r2.x = -r0.x;
  r1.y = dot(r0.yz, r2.xy);
  r1.x = dot(r0.yz, r2.yz);
  r0.xy = float2(0.5,0.5) + r1.xy;
  r0.zw = dc_f_cb0[6].xx * v1.xy;
  r0.zw = r0.zw * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r1.xyzw = _Tex_vertex_Distortion.Sample(sampler_Tex_vertex_Distortion, r0.zw).xyzw;
  r0.xy = dc_f_cb0[6].yy * r1.xx + r0.xy;
  r0.xy = r0.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r0.xyzw = _Texture_Vertex_Displace.Sample(sampler_Texture_Vertex_Displace, r0.xy).xyzw;
  r0.w = cmp(r0.y >= r0.z);
  r0.w = r0.w ? 1.000000 : 0;
  r0.y = r0.y + -r0.z;
  r0.y = r0.w * r0.y + r0.z;
  r0.z = cmp(r0.x >= r0.y);
  r0.x = r0.x + -r0.y;
  r0.z = r0.z ? 1.000000 : 0;
  r0.x = r0.z * r0.x + r0.y;
  o0.w = v3.w * r0.x;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
