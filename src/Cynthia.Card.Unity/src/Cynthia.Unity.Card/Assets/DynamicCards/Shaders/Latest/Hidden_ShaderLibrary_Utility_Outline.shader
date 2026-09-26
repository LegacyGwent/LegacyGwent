Shader "DynamicCards/Latest/Hidden_ShaderLibrary_Utility_Outline" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_OutlineColor("_OutlineColor",Color)=(0.0,0.0,0.0,1.0)
_OutlineSize("_OutlineSize",Float)=2.0
}
SubShader {
Tags {}
Pass {
Cull Back
ZWrite On
ZTest LEqual
Blend One Zero
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_MainTex;
Texture2D<float4> _MainTex;
float4 _MainTex_TexelSize;
float4 _OutlineColor;
int _OutlineSize;












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
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[5];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_TexelSize.x,_MainTex_TexelSize.y,_MainTex_TexelSize.z,_MainTex_TexelSize.w);
dc_f_cb0[3]=float4(_OutlineColor.x,_OutlineColor.y,_OutlineColor.z,_OutlineColor.w);
dc_f_cb0[4]=float4(_OutlineSize,0,0,0);



  float4 r0,r1,r2,r3,r4,r5,r6,r7,r8,r9,r10;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = _MainTex.Sample(sampler_MainTex, v1.xy).xyzw;
  r1.x = saturate(r0.w);
  r0.xyz = -dc_f_cb0[3].xyz + r0.xyz;
  r1.xyz = r1.xxx * r0.xyz + dc_f_cb0[3].xyz;
  r0.x = asint(dc_f_cb0[4].x) + 1;
  r2.z = 0;
  r1.w = r0.w;
  r0.y = 1;
  while (true) {
    r0.z = cmp((int)r0.y >= (int)r0.x);
    if (r0.z != 0) break;
    r0.z = (int)r0.y;
    r2.xy = dc_f_cb0[2].yx * r0.zz;
    r3.xyzw = v1.xyxy + r2.zxyz;
    r4.xyzw = _MainTex.Sample(sampler_MainTex, r3.xy).xyzw;
    r5.xyzw = v1.xyxy + -r2.zxyz;
    r6.xyzw = _MainTex.Sample(sampler_MainTex, r5.xy).xyzw;
    r3.xyzw = _MainTex.Sample(sampler_MainTex, r3.zw).xyzw;
    r5.xyzw = _MainTex.Sample(sampler_MainTex, r5.zw).xyzw;
    r0.z = 0.707000017 * r0.z;
    r3.z = dc_f_cb0[2].y * r0.z;
    r2.xy = r0.zz * dc_f_cb0[2].xy + v1.xy;
    r7.xyzw = _MainTex.Sample(sampler_MainTex, r2.xy).xyzw;
    r2.xy = -r0.zz * dc_f_cb0[2].xy + v1.xy;
    r8.xyzw = _MainTex.Sample(sampler_MainTex, r2.xy).xyzw;
    r0.z = (int)-r0.y;
    r0.z = dc_f_cb0[2].x * r0.z;
    r3.x = 0.707000017 * r0.z;
    r2.xy = v1.xy + r3.xz;
    r9.xyzw = _MainTex.Sample(sampler_MainTex, r2.xy).xyzw;
    r2.xy = v1.xy + -r3.xz;
    r10.xyzw = _MainTex.Sample(sampler_MainTex, r2.xy).xyzw;
    r0.z = max(r4.w, r1.w);
    r0.z = max(r0.z, r6.w);
    r0.z = max(r0.z, r3.w);
    r0.z = max(r0.z, r5.w);
    r0.z = max(r0.z, r7.w);
    r0.z = max(r0.z, r8.w);
    r0.z = max(r0.z, r9.w);
    r1.w = max(r0.z, r10.w);
    r0.y = (int)r0.y + 1;
  }
  o0.xyzw = saturate(r1.xyzw);
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
