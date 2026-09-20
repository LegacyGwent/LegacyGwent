Shader "DynamicCards/Legacy/Custom_Particle_AlphaBlendedRefraction" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_RefractionMulti("_RefractionMulti",Float)=0.0
_AlphaMulti("_AlphaMulti",Float)=1.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"QUEUE"="Transparent" "RenderType"="Transparent"}
GrabPass {  }
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
SamplerState sampler_GrabTexture;
SamplerState sampler_MainTex;
Texture2D<float4> _GrabTexture;
Texture2D<float4> _MainTex;
float _RefractionMulti;
float4 _MainTex_ST;












void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float4 o3 : COLOR0)
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
  r0.xyzw = dc_v_cb1[20].xyzw * r0.wwww + r1.xyzw;
  o0.xyzw = r0.xyzw;
  o2.xyzw = r0.xyzw;
  o1.xy = v1.xy;
  o3.xyzw = v2.xyzw;
  return;
}




















void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float4 v3 : COLOR0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[6];
dc_f_cb1[0]=float4(0,0,0,0);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(0,0,0,0);
dc_f_cb1[5]=float4(_ProjectionParams.x,_ProjectionParams.y,_ProjectionParams.z,_ProjectionParams.w);
float4 dc_f_cb0[4];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_RefractionMulti,0,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = -dc_f_cb1[5].x * dc_f_cb1[5].x;
  r1.xy = v2.xy / v2.ww;
  r1.z = r1.y * r0.x;
  r0.xy = r1.xz * float2(0.5,0.5) + float2(0.5,0.5);
  r0.zw = v1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r0.zw).xyzw;
  r0.z = dc_f_cb0[3].x * r1.w;
  r1.xw = float2(1,0);
  r1.yz = v3.ww;
  r0.zw = r1.xy * r0.zz;
  r0.xy = r0.zw * r1.zw + r0.xy;
  r0.xyzw = _GrabTexture.Sample(sampler_GrabTexture, r0.xy).xyzw;
  o0.xyz = r0.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
