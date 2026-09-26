Shader "DynamicCards/Legacy/Custom_Backtrack_add_shader" {
Properties {
_Tint("_Tint",Color)=(0.0,0.8308823704719543,0.6245942115783691,1.0)
_texture("_texture",2D)="white" {}
_Texture_Contrast_Power("_Texture_Contrast_Power",Float)=4.0
_Tex_MASK_Alpha("_Tex_MASK_Alpha",2D)="white" {}
_Tex_MASK_contrast_power("_Tex_MASK_contrast_power",Float)=1.0
_Tex_MASK_Time("_Tex_MASK_Time",Float)=0.0
_intensity("_intensity",Float)=0.0
_noise("_noise",2D)="bump" {}
_noise_contrast_power("_noise_contrast_power",Float)=3.0
_Noise_STR("_Noise_STR",Float)=2.0
_NormalDirectionInfluence("_NormalDirectionInfluence",Float)=0.0
_Displace_Direction_Vecto("_Displace_Direction_Vecto",Vector)=(1.0,1.0,1.0,0.0)
_UV_Time_Multiplier("_UV_Time_Multiplier",Float)=99.0
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
SamplerState sampler_Tex_MASK_Alpha;
SamplerState sampler_noise;
SamplerState sampler_texture;
Texture2D<float4> _Tex_MASK_Alpha;
Texture2D<float4> _noise;
Texture2D<float4> _texture;
float _Noise_STR;
float _NormalDirectionInfluence;
float _Tex_MASK_Time;
float _Tex_MASK_contrast_power;
float _Texture_Contrast_Power;
float _UV_Time_Multiplier;
float _intensity;
float _noise_contrast_power;
float4 _Displace_Direction_Vecto;
float4 _Tex_MASK_Alpha_ST;
float4 _TimeEditor;
float4 _Tint;
float4 _noise_ST;
float4 _texture_ST;




















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
float4 dc_v_cb0[10];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(0,0,0,0);
dc_v_cb0[6]=float4(_noise_ST.x,_noise_ST.y,_noise_ST.z,_noise_ST.w);
dc_v_cb0[7]=float4(_UV_Time_Multiplier,_noise_contrast_power,_Noise_STR,0);
dc_v_cb0[8]=float4(_NormalDirectionInfluence,0,0,0);
dc_v_cb0[9]=float4(_Displace_Direction_Vecto.x,_Displace_Direction_Vecto.y,_Displace_Direction_Vecto.z,_Displace_Direction_Vecto.w);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_v_cb1[0].x + dc_v_cb0[2].x;
  r0.y = r0.x * dc_v_cb0[7].x + v2.y;
  r0.x = v2.x + r0.y;
  r0.xy = r0.xy * dc_v_cb0[6].xy + dc_v_cb0[6].zw;
  r0.xyzw = _noise.SampleLevel(sampler_noise, r0.xy, 0).xyzw;
  r0.xyz = log2(r0.xyz);
  r0.xyz = dc_v_cb0[7].yyy * r0.xyz;
  r0.xyz = exp2(r0.xyz);
  r0.xyz = dc_v_cb0[7].zzz * r0.xyz;
  r1.xyz = dc_v_cb0[9].xyz * r0.xyz;
  r0.xyz = r0.xyz * v1.xyz + -r1.xyz;
  r0.xyz = dc_v_cb0[8].xxx * r0.xyz + r1.xyz;
  r0.xyz = v0.xyz + r0.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
  o1.xy = v2.xy;
  r0.x = dot(v1.xyz, dc_v_cb2[4].xyz);
  r0.y = dot(v1.xyz, dc_v_cb2[5].xyz);
  r0.z = dot(v1.xyz, dc_v_cb2[6].xyz);
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
float4 dc_f_cb0[12];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_Tint.x,_Tint.y,_Tint.z,_Tint.w);
dc_f_cb0[4]=float4(_texture_ST.x,_texture_ST.y,_texture_ST.z,_texture_ST.w);
dc_f_cb0[5]=float4(_intensity,0,0,0);
dc_f_cb0[6]=float4(0,0,0,0);
dc_f_cb0[7]=float4(0,0,0,_Texture_Contrast_Power);
dc_f_cb0[8]=float4(0,0,0,0);
dc_f_cb0[9]=float4(0,0,0,0);
dc_f_cb0[10]=float4(_Tex_MASK_Alpha_ST.x,_Tex_MASK_Alpha_ST.y,_Tex_MASK_Alpha_ST.z,_Tex_MASK_Alpha_ST.w);
dc_f_cb0[11]=float4(_Tex_MASK_contrast_power,_Tex_MASK_Time,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r0.xyzw = _texture.Sample(sampler_texture, r0.xy).xyzw;
  r0.x = dot(r0.xyz, float3(0.300000012,0.589999974,0.109999999));
  r0.y = log2(r0.x);
  r0.y = dc_f_cb0[7].w * r0.y;
  r0.y = exp2(r0.y);
  r0.y = r0.y + r0.y;
  r0.xyz = dc_f_cb0[3].xyz * r0.xxx + r0.yyy;
  r0.xyz = dc_f_cb0[5].xxx * r0.xyz;
  r0.xyz = r0.xyz * r0.www;
  r0.w = dc_f_cb1[0].x + dc_f_cb0[2].x;
  r1.y = r0.w * dc_f_cb0[11].y + v1.y;
  r1.x = v1.x + r1.y;
  r1.xy = r1.xy * dc_f_cb0[10].xy + dc_f_cb0[10].zw;
  r1.xyzw = _Tex_MASK_Alpha.Sample(sampler_Tex_MASK_Alpha, r1.xy).xyzw;
  r0.w = log2(r1.w);
  r0.w = dc_f_cb0[11].x * r0.w;
  r0.w = exp2(r0.w);
  r0.xyz = r0.xyz * r0.www;
  o0.xyz = v3.xyz * r0.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
