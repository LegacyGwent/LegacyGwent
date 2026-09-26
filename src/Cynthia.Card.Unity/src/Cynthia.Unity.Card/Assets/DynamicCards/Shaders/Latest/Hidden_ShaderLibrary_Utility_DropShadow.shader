Shader "DynamicCards/Latest/Hidden_ShaderLibrary_Utility_DropShadow" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Radius("_Radius",Float)=0.0
_Opacity("_Opacity",Float)=1.0
_Offset("_Offset",Vector)=(0.0,0.0,0.0,0.0)
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
float _Opacity;
float4 _MainTex_TexelSize;
float4 _Offset;
int _Radius;












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
dc_f_cb0[3]=float4(_Radius,_Opacity,0,0);
dc_f_cb0[4]=float4(_Offset.x,_Offset.y,_Offset.z,_Offset.w);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = dc_f_cb0[4].xy * dc_f_cb0[2].xy + v1.xy;
  r0.z = asint(dc_f_cb0[3].x);
  r0.w = r0.z * r0.z;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).wxyz;
  r1.x = saturate(r1.x);
  r0.w = 0.5 * r0.w;
  r2.x = dc_f_cb0[2].x;
  r2.y = 0;
  r1.y = r1.x;
  r1.zw = float2(1,1);
  while (true) {
    r2.z = cmp(r0.z < r1.w);
    if (r2.z != 0) break;
    r2.z = r1.w * r1.w;
    r2.z = -r2.z / r0.w;
    r2.z = 1.44269502 * r2.z;
    r2.z = exp2(r2.z);
    r3.xy = -r1.ww * r2.xy + r0.xy;
    r3.xyzw = _MainTex.Sample(sampler_MainTex, r3.xy).wxyz;
    r3.x = saturate(r3.x);
    r2.w = r3.x * r2.z + r1.y;
    r3.xy = r1.ww * r2.xy + r0.xy;
    r3.xyzw = _MainTex.Sample(sampler_MainTex, r3.xy).wxyz;
    r3.x = saturate(r3.x);
    r1.y = r3.x * r2.z + r2.w;
    r1.z = r2.z * 2 + r1.z;
    r1.w = 1 + r1.w;
  }
  r0.x = saturate(r1.y / r1.z);
  o0.w = dc_f_cb0[3].y * r0.x;
  o0.xyz = float3(0,0,0);
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
