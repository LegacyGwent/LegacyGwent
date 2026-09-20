Shader "DynamicCards/Latest/Hidden_ShaderLibrary_Utility_ColorRangeRemap" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Color("_Color",Color)=(1.0,1.0,1.0,1.0)
_Hue("_Hue",Float)=0.0
_Transform("_Transform",Float)=1.0
_Mask("_Mask",Float)=0.0
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
float _Hue;
float _Transform;
int _Mask;












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
float4 dc_f_cb0[4];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(_Hue,_Mask,_Transform,0);



  float4 r0,r1,r2,r3,r4;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.zw = float2(-1,0.666666687);
  r1.zw = float2(1,-1);
  r2.xyzw = _MainTex.Sample(sampler_MainTex, v1.xy).xyzw;
  r3.x = saturate(r2.x);
  r3.x = 1 + -r3.x;
  r3.xyz = r3.xxx * r2.xyz;
  r3.xyz = r3.xyz * float3(-1,-1,0) + r2.xyz;
  r3.xyz = r3.xyz * float3(2,2,2) + -r2.xyz;
  r3.xyw = dc_f_cb0[3].zzz * r3.yzx + r2.yzx;
  r4.x = cmp(r3.x >= r3.y);
  r4.x = r4.x ? 1.000000 : 0;
  r0.xy = r3.yx;
  r1.xy = r3.xy + -r0.xy;
  r0.xyzw = r4.xxxx * r1.xyzw + r0.xyzw;
  r3.xyz = r0.xyw;
  r1.x = cmp(r3.w >= r3.x);
  r1.x = r1.x ? 1.000000 : 0;
  r0.xyw = r3.wyx;
  r0.xyzw = -r3.xyzw + r0.xyzw;
  r0.xyzw = r1.xxxx * r0.xyzw + r3.xyzw;
  r1.x = min(r0.w, r0.y);
  r1.x = -r1.x + r0.x;
  r1.y = r1.x * 6 + 1.00000001e-10;
  r0.y = r0.w + -r0.y;
  r0.y = r0.y / r1.y;
  r0.y = r0.z + r0.y;
  r0.y = dc_f_cb0[3].x + abs(r0.y);
  r0.yzw = float3(1,0.666666687,0.333333343) + r0.yyy;
  r0.yzw = frac(r0.yzw);
  r0.yzw = r0.yzw * float3(6,6,6) + float3(-3,-3,-3);
  r0.yzw = saturate(float3(-1,-1,-1) + abs(r0.yzw));
  r0.yzw = float3(-1,-1,-1) + r0.yzw;
  r1.y = 1.00000001e-10 + r0.x;
  r1.x = r1.x / r1.y;
  r0.yzw = r1.xxx * r0.yzw + float3(1,1,1);
  r0.xyz = r0.xxx * r0.yzw + -r2.xyz;
  r0.w = r2.x + -r2.y;
  r0.w = r0.w + -r2.z;
  r0.w = saturate(3 * r0.w);
  r0.w = log2(r0.w);
  r0.w = 0.75 * r0.w;
  r0.w = exp2(r0.w);
  r0.w = 4 * r0.w;
  r0.w = min(1, r0.w);
  r0.xyz = r0.www * r0.xyz + r2.xyz;
  o0.w = r2.w;
  r1.x = cmp(asint(dc_f_cb0[3].y) == 1);
  o0.xyz = r1.xxx ? r0.www : r0.xyz;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
