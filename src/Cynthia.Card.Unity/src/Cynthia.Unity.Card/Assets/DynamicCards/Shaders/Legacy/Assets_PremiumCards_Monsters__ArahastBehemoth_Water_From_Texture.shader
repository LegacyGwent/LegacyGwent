Shader "DynamicCards/Legacy/Assets_PremiumCards_Monsters__ArahastBehemoth_Water_From_Texture" {
Properties {
_TextureofWater("_TextureofWater",2D)="white" {}
_Alpha_Slider("_Alpha_Slider",Float)=2.863248109817505
_FakeSpecularHighAlphaSeperation("_FakeSpecularHighAlphaSeperation",Float)=1.0
_FakeSpecularStrEmission("_FakeSpecularStrEmission",Float)=0.5
_DiffuseTint("_DiffuseTint",Color)=(0.5,0.5,0.5,1.0)
_Refraction_STR("_Refraction_STR",Float)=0.5
_Displace_UV_Scale("_Displace_UV_Scale",Float)=1.0
_NormalDisplace("_NormalDisplace",2D)="bump" {}
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
GrabPass {  }
Pass {
Cull Off
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
SamplerState sampler_GrabTexture;
SamplerState sampler_NormalDisplace;
SamplerState sampler_TextureofWater;
Texture2D<float4> _GrabTexture;
Texture2D<float4> _NormalDisplace;
Texture2D<float4> _TextureofWater;
float _Alpha_Slider;
float _Displace_UV_Scale;
float _FakeSpecularHighAlphaSeperation;
float _FakeSpecularStrEmission;
float _Refraction_STR;
float4 _DiffuseTint;
float4 _NormalDisplace_ST;
float4 _TextureofWater_ST;












void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float4 v2 : TANGENT0,
  float2 v3 : TEXCOORD0,
  float4 v4 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float4 o3 : TEXCOORD2,
  out float4 o4 : TEXCOORD3,
  out float4 o5 : TEXCOORD4,
  out float4 o6 : TEXCOORD5,
  out float4 o7 : COLOR0)
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
  r0.xyzw = dc_v_cb1[20].xyzw * r1.wwww + r0.xyzw;
  o0.xyzw = r0.xyzw;
  o6.xyzw = r0.xyzw;
  o1.xy = v3.xy;
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
  o7.xyzw = v4.xyzw;
  return;
}




























void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float4 v3 : TEXCOORD2,
  float4 v4 : TEXCOORD3,
  float4 v5 : TEXCOORD4,
  float4 v6 : TEXCOORD5,
  float4 v7 : COLOR0,
  uint v8 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb3[1];
dc_f_cb3[0]=float4(glstate_lightmodel_ambient.x,glstate_lightmodel_ambient.y,glstate_lightmodel_ambient.z,glstate_lightmodel_ambient.w);
float4 dc_f_cb2[1];
dc_f_cb2[0]=float4(_WorldSpaceLightPos0.x,_WorldSpaceLightPos0.y,_WorldSpaceLightPos0.z,_WorldSpaceLightPos0.w);
float4 dc_f_cb1[6];
dc_f_cb1[0]=float4(0,0,0,0);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(0,0,0,0);
dc_f_cb1[5]=float4(_ProjectionParams.x,_ProjectionParams.y,_ProjectionParams.z,_ProjectionParams.w);
float4 dc_f_cb0[8];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_LightColor0.x,_LightColor0.y,_LightColor0.z,_LightColor0.w);
dc_f_cb0[3]=float4(_TextureofWater_ST.x,_TextureofWater_ST.y,_TextureofWater_ST.z,_TextureofWater_ST.w);
dc_f_cb0[4]=float4(_Alpha_Slider,_FakeSpecularHighAlphaSeperation,_FakeSpecularStrEmission,0);
dc_f_cb0[5]=float4(_DiffuseTint.x,_DiffuseTint.y,_DiffuseTint.z,_DiffuseTint.w);
dc_f_cb0[6]=float4(_NormalDisplace_ST.x,_NormalDisplace_ST.y,_NormalDisplace_ST.z,_NormalDisplace_ST.w);
dc_f_cb0[7]=float4(_Refraction_STR,_Displace_UV_Scale,0,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r0.xyzw = _TextureofWater.Sample(sampler_TextureofWater, r0.xy).xyzw;
  r1.x = r0.w * 100 + -0.5;
  r1.x = cmp(r1.x < 0);
  if (r1.x != 0) discard;
  r1.x = dot(v3.xyz, v3.xyz);
  r1.x = rsqrt(r1.x);
  r1.xyz = v3.xyz * r1.xxx;
  r1.w = v8.x ? 1 : -1;
  r1.xyz = r1.xyz * r1.www;
  r2.xy = dc_f_cb0[7].yy * v1.xy;
  r2.xy = r2.xy * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r2.xyzw = _NormalDisplace.Sample(sampler_NormalDisplace, r2.xy).xyzw;
  r2.xy = r2.wy * float2(2,2) + float2(-1,-1);
  r3.xyz = v5.xyz * r2.yyy;
  r3.xyz = r2.xxx * v4.xyz + r3.xyz;
  r1.w = dot(r2.xy, r2.xy);
  r2.xy = r2.xy * r0.ww;
  r2.xy = dc_f_cb0[7].xx * r2.xy;
  r1.w = min(1, r1.w);
  r1.w = 1 + -r1.w;
  r1.w = sqrt(r1.w);
  r1.xyz = r1.www * r1.xyz + r3.xyz;
  r1.w = dot(r1.xyz, r1.xyz);
  r1.w = rsqrt(r1.w);
  r1.xyz = r1.xyz * r1.www;
  r1.w = dot(dc_f_cb2[0].xyz, dc_f_cb2[0].xyz);
  r1.w = rsqrt(r1.w);
  r3.xyz = dc_f_cb2[0].xyz * r1.www;
  r1.x = dot(r1.xyz, r3.xyz);
  r1.x = max(0, r1.x);
  r1.yzw = dc_f_cb3[0].xyz + dc_f_cb3[0].xyz;
  r1.xyz = r1.xxx * dc_f_cb0[2].xyz + r1.yzw;
  r1.w = log2(r0.w);
  r1.w = dc_f_cb0[4].y * r1.w;
  r1.w = exp2(r1.w);
  r1.w = dc_f_cb0[4].z * r1.w;
  r0.xyz = dc_f_cb0[5].xyz * r0.xyz;
  r0.w = dc_f_cb0[4].x * r0.w;
  r0.xyzw = v7.xyzw * r0.xyzw;
  r0.xyz = r1.xyz * r0.xyz + r1.www;
  r1.x = -dc_f_cb1[5].x * dc_f_cb1[5].x;
  r3.xy = v6.xy / v6.ww;
  r3.z = r3.y * r1.x;
  r1.xy = r3.xz * float2(0.5,0.5) + float2(0.5,0.5);
  r1.xy = r2.xy * v7.ww + r1.xy;
  r1.xyzw = _GrabTexture.Sample(sampler_GrabTexture, r1.xy).xyzw;
  r0.xyz = -r1.xyz + r0.xyz;
  o0.xyz = r0.www * r0.xyz + r1.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
