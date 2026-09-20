Shader "DynamicCards/Latest/VFX_Common_AlphaBlended_moveInDirection_VertexColor_UVNoise_Light" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_AlphaMult("_AlphaMult",Float)=1.0
_AlphaPower("_AlphaPower",Float)=1.0
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_MainU("_MainU",Float)=0.0
_MainV("_MainV",Float)=0.0
_NoiseTex("_NoiseTex",2D)="white" {}
_NoiseU("_NoiseU",Float)=0.0
_NoiseV("_NoiseV",Float)=0.0
_NoiseMult("_NoiseMult",Float)=0.0
_ColorGain("_ColorGain",Float)=1.0
_NoiseMultOverLifetime("_NoiseMultOverLifetime",Float)=0.0
_Normal("_Normal",2D)="bump" {}
_NormalMult("_NormalMult",Float)=1.0
_SelfIllum("_SelfIllum",Float)=0.0
_Specular("_Specular",2D)="white" {}
_SpecularColor("_SpecularColor",Color)=(0.5,0.5,0.5,1.0)
_Glossiness("_Glossiness",Float)=25.0
_SpecMult("_SpecMult",Float)=1.0
_Cubemap("_Cubemap",Cube)="_Skybox" {}
_OpacityClip("_OpacityClip",Float)=1.0
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
SamplerState sampler_Cubemap;
SamplerState sampler_MainTex;
SamplerState sampler_NoiseTex;
SamplerState sampler_Normal;
SamplerState sampler_Specular;
Texture2D<float4> _MainTex;
Texture2D<float4> _NoiseTex;
Texture2D<float4> _Normal;
Texture2D<float4> _Specular;
TextureCube<float4> _Cubemap;
float _AlphaMult;
float _AlphaPower;
float _ColorGain;
float _Glossiness;
float _MainU;
float _MainV;
float _NoiseMult;
float _NoiseMultOverLifetime;
float _NoiseU;
float _NoiseV;
float _NormalMult;
float _OpacityClip;
float _SelfIllum;
float _SpecMult;
float4 _MainTex_ST;
float4 _NoiseTex_ST;
float4 _Normal_ST;
float4 _SpecularColor;
float4 _Specular_ST;
float4 _TintColor;












void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float4 v2 : TANGENT0,
  float4 v3 : TEXCOORD0,
  float4 v4 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float4 o3 : TEXCOORD2,
  out float4 o4 : TEXCOORD3,
  out float3 o5 : TEXCOORD4,
  out float4 o6 : COLOR0)
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
float4 dc_v_cb0[7];
dc_v_cb0[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb0[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb0[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb0[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
dc_v_cb0[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_v_cb0[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_v_cb0[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb0[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb0[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb0[2].xyzw * v0.zzzz + r0.xyzw;
  r1.xyzw = dc_v_cb0[3].xyzw + r0.xyzw;
  o2.xyzw = dc_v_cb0[3].xyzw * v0.wwww + r0.xyzw;
  r0.xyzw = dc_v_cb1[18].xyzw * r1.yyyy;
  r0.xyzw = dc_v_cb1[17].xyzw * r1.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[19].xyzw * r1.zzzz + r0.xyzw;
  o0.xyzw = dc_v_cb1[20].xyzw * r1.wwww + r0.xyzw;
  o1.xyzw = v3.xyzw;
  r0.x = dot(v1.xyz, dc_v_cb0[4].xyz);
  r0.y = dot(v1.xyz, dc_v_cb0[5].xyz);
  r0.z = dot(v1.xyz, dc_v_cb0[6].xyz);
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  r0.xyz = r0.xyz * r0.www;
  o3.xyz = r0.xyz;
  r1.xyz = dc_v_cb0[1].xyz * v2.yyy;
  r1.xyz = dc_v_cb0[0].xyz * v2.xxx + r1.xyz;
  r1.xyz = dc_v_cb0[2].xyz * v2.zzz + r1.xyz;
  r0.w = dot(r1.xyz, r1.xyz);
  r0.w = rsqrt(r0.w);
  r1.xyz = r1.xyz * r0.www;
  o4.xyz = r1.xyz;
  r2.xyz = r1.yzx * r0.zxy;
  r0.xyz = r0.yzx * r1.zxy + -r2.xyz;
  r0.xyz = v2.www * r0.xyz;
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  o5.xyz = r0.xyz * r0.www;
  o6.xyzw = v4.xyzw;
  return;
}




































void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float4 v3 : TEXCOORD2,
  float4 v4 : TEXCOORD3,
  float3 v5 : TEXCOORD4,
  float4 v6 : COLOR0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb3[1];
dc_f_cb3[0]=float4(glstate_lightmodel_ambient.x,glstate_lightmodel_ambient.y,glstate_lightmodel_ambient.z,glstate_lightmodel_ambient.w);
float4 dc_f_cb2[1];
dc_f_cb2[0]=float4(_WorldSpaceLightPos0.x,_WorldSpaceLightPos0.y,_WorldSpaceLightPos0.z,_WorldSpaceLightPos0.w);
float4 dc_f_cb1[5];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(_WorldSpaceCameraPos.x,_WorldSpaceCameraPos.y,_WorldSpaceCameraPos.z,0);
float4 dc_f_cb0[14];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_LightColor0.x,_LightColor0.y,_LightColor0.z,_LightColor0.w);
dc_f_cb0[3]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[4]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[5]=float4(_MainU,_MainV,0,0);
dc_f_cb0[6]=float4(_NoiseTex_ST.x,_NoiseTex_ST.y,_NoiseTex_ST.z,_NoiseTex_ST.w);
dc_f_cb0[7]=float4(_NoiseU,_NoiseV,_NoiseMult,_AlphaMult);
dc_f_cb0[8]=float4(_AlphaPower,_ColorGain,_NoiseMultOverLifetime,0);
dc_f_cb0[9]=float4(_Normal_ST.x,_Normal_ST.y,_Normal_ST.z,_Normal_ST.w);
dc_f_cb0[10]=float4(_NormalMult,_SelfIllum,0,0);
dc_f_cb0[11]=float4(_Specular_ST.x,_Specular_ST.y,_Specular_ST.z,_Specular_ST.w);
dc_f_cb0[12]=float4(_SpecularColor.x,_SpecularColor.y,_SpecularColor.z,_SpecularColor.w);
dc_f_cb0[13]=float4(_Glossiness,_SpecMult,_OpacityClip,0);



  float4 r0,r1,r2,r3,r4;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = dc_f_cb1[0].yy * dc_f_cb0[7].xy + v1.xy;
  r0.xy = v1.ww + r0.xy;
  r0.xy = r0.xy * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r0.xyzw = _NoiseTex.Sample(sampler_NoiseTex, r0.xy).xyzw;
  r0.xy = float2(-0.5,-0.5) + r0.xy;
  r0.z = dc_f_cb0[7].z * v1.z + -dc_f_cb0[7].z;
  r0.z = dc_f_cb0[8].z * r0.z + dc_f_cb0[7].z;
  r1.xy = dc_f_cb1[0].yy * dc_f_cb0[5].xy + v1.xy;
  r1.xy = v1.ww + r1.xy;
  r0.xy = r0.xy * r0.zz + r1.xy;
  r0.zw = r0.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r0.zw).xyzw;
  r0.z = log2(r1.w);
  r1.xyz = dc_f_cb0[4].xyz * r1.xyz;
  r1.xyz = v6.xyz * r1.xyz;
  r1.xyz = dc_f_cb0[8].yyy * r1.xyz;
  r0.z = dc_f_cb0[8].x * r0.z;
  r0.z = exp2(r0.z);
  r0.z = dc_f_cb0[4].w * r0.z;
  r0.z = v6.w * r0.z;
  r0.z = saturate(dc_f_cb0[7].w * r0.z);
  r0.w = r0.z * dc_f_cb0[13].z + -0.5;
  o0.w = r0.z;
  r0.z = cmp(r0.w < 0);
  if (r0.z != 0) discard;
  r0.zw = r0.xy * dc_f_cb0[11].xy + dc_f_cb0[11].zw;
  r0.xy = r0.xy * dc_f_cb0[9].xy + dc_f_cb0[9].zw;
  r2.xyzw = _Normal.Sample(sampler_Normal, r0.xy).xyzw;
  r0.xyzw = _Specular.Sample(sampler_Specular, r0.zw).xyzw;
  r0.xyz = dc_f_cb0[12].xyz * r0.xyz;
  r0.xyz = dc_f_cb0[13].yyy * r0.xyz;
  r2.x = r2.x * r2.w;
  r2.zw = r2.xy * float2(2,2) + float2(-1,-1);
  r3.xy = r2.xy + r2.xy;
  r0.w = dot(r2.zw, r2.zw);
  r0.w = min(1, r0.w);
  r0.w = 1 + -r0.w;
  r3.z = sqrt(r0.w);
  r2.xyz = float3(-1.5,-1.5,-1) + r3.xyz;
  r2.xyz = dc_f_cb0[10].xxx * r2.xyz + float3(0.5,0.5,1);
  r3.xyz = v5.xyz * r2.yyy;
  r2.xyw = r2.xxx * v4.xyz + r3.xyz;
  r0.w = dot(v3.xyz, v3.xyz);
  r0.w = rsqrt(r0.w);
  r3.xyz = v3.xyz * r0.www;
  r2.xyz = r2.zzz * r3.xyz + r2.xyw;
  r0.w = dot(r2.xyz, r2.xyz);
  r0.w = rsqrt(r0.w);
  r2.xyz = r2.xyz * r0.www;
  r3.xyz = dc_f_cb1[4].xyz + -v2.xyz;
  r0.w = dot(r3.xyz, r3.xyz);
  r0.w = rsqrt(r0.w);
  r4.xyz = r3.xyz * r0.www;
  r1.w = dot(-r4.xyz, r2.xyz);
  r1.w = r1.w + r1.w;
  r4.xyz = r2.xyz * -r1.www + -r4.xyz;
  r4.xyzw = _Cubemap.Sample(sampler_Cubemap, r4.xyz).xyzw;
  r0.xyz = r4.xyz * r0.xyz;
  r1.w = dot(dc_f_cb2[0].xyz, dc_f_cb2[0].xyz);
  r1.w = rsqrt(r1.w);
  r4.xyz = dc_f_cb2[0].xyz * r1.www;
  r3.xyz = r3.xyz * r0.www + r4.xyz;
  r0.w = dot(r2.xyz, r4.xyz);
  r0.w = max(0, r0.w);
  r1.w = dot(r3.xyz, r3.xyz);
  r1.w = rsqrt(r1.w);
  r3.xyz = r3.xyz * r1.www;
  r1.w = dot(r3.xyz, r2.xyz);
  r1.w = max(0, r1.w);
  r1.w = log2(r1.w);
  r1.w = dc_f_cb0[13].x * r1.w;
  r1.w = exp2(r1.w);
  r2.xyz = dc_f_cb0[2].xyz * r1.www;
  r0.xyz = r2.xyz * r0.xyz;
  r2.xyz = dc_f_cb3[0].xyz + dc_f_cb3[0].xyz;
  r2.xyz = r0.www * dc_f_cb0[2].xyz + r2.xyz;
  r0.xyz = r2.xyz * r1.xyz + r0.xyz;
  o0.xyz = r1.xyz * dc_f_cb0[10].yyy + r0.xyz;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
