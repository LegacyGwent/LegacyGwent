Shader "DynamicCards/Legacy/Custom_Cards_CardCore_ImageLayerShaderCurvedWorld" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Mask("_Mask",2D)="white" {}
_Noise("_Noise",2D)="white" {}
_Speed("_Speed",Float)=0.0
_Curvature("_Curvature",Float)=0.0010000000474974513
_Curvature2("_Curvature2",Float)=0.0010000000474974513
_CutOffThreshold("_CutOffThreshold",Float)=0.5
_CamPosition("_CamPosition",Vector)=(0.0,0.0,0.0,0.0)
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Back
ZWrite On
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
SamplerState sampler_Mask;
SamplerState sampler_Noise;
Texture2D<float4> _MainTex;
Texture2D<float4> _Mask;
Texture2D<float4> _Noise;
float _Curvature2;
float _Curvature;
float _CutOffThreshold;
float _Speed;
float4 _CamPosition;
float4 _MainTex_ST;
float4 _Mask_ST;
float4 _Noise_ST;














void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0)
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
float4 dc_v_cb1[4];
dc_v_cb1[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb1[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb1[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb1[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
float4 dc_v_cb0[7];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(0,_Curvature,_Curvature2,0);
dc_v_cb0[6]=float4(_CamPosition.x,_CamPosition.y,_CamPosition.z,_CamPosition.w);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = dc_v_cb1[1].zx * v0.yy;
  r0.xy = dc_v_cb1[0].zx * v0.xx + r0.xy;
  r0.xy = dc_v_cb1[2].zx * v0.zz + r0.xy;
  r0.xy = dc_v_cb1[3].zx * v0.ww + r0.xy;
  r0.xy = -dc_v_cb0[6].zx + r0.xy;
  r0.xy = r0.xy * r0.xy;
  r0.y = r0.x + r0.y;
  r1.xy = -dc_v_cb0[5].zy * r0.xy;
  r1.z = 0;
  r0.xyz = v0.xyz + r1.xyz;
  r1.xyzw = dc_v_cb1[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb1[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb1[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb1[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb2[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb2[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb2[20].xyzw * r0.wwww + r1.xyzw;
  o1.xy = v1.xy;
  return;
}
























void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[8];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_Mask_ST.x,_Mask_ST.y,_Mask_ST.z,_Mask_ST.w);
dc_f_cb0[4]=float4(_Noise_ST.x,_Noise_ST.y,_Noise_ST.z,_Noise_ST.w);
dc_f_cb0[5]=float4(_Speed,0,0,0);
dc_f_cb0[6]=float4(0,0,0,0);
dc_f_cb0[7]=float4(_CutOffThreshold,0,0,0);



  float4 r0,r1,r2,r3,r4,r5;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r1.x = cmp(dc_f_cb0[7].x >= r0.w);
  if (r1.x != 0) discard;
  r1.x = -dc_f_cb1[0].x * dc_f_cb0[5].x + -0.131999999;
  r1.yw = float2(0.532000005,0.312000006);
  r1.xy = r1.xy * dc_f_cb0[4].xy + v1.xy;
  r1.xy = dc_f_cb0[4].zw + r1.xy;
  r2.xyzw = _Noise.Sample(sampler_Noise, r1.xy).xyzw;
  r3.xy = dc_f_cb1[0].xx * -dc_f_cb0[5].xx + float2(0.490999997,0.512000024);
  r3.zw = float2(-0.950999975,0.112000003);
  r3.xyzw = r3.zxyw * dc_f_cb0[4].xyxy + v1.xyxy;
  r3.xyzw = dc_f_cb0[4].zwzw + r3.xyzw;
  r4.xyzw = _Noise.Sample(sampler_Noise, r3.xy).xyzw;
  r1.z = dc_f_cb1[0].x * dc_f_cb0[5].x + 0.811999977;
  r1.xy = r1.zw * dc_f_cb0[4].xy + v1.xy;
  r1.xy = dc_f_cb0[4].zw + r1.xy;
  r1.xyzw = _Noise.Sample(sampler_Noise, r1.xy).xyzw;
  r3.xyzw = _Noise.Sample(sampler_Noise, r3.zw).xyzw;
  r1.xy = v1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r5.xyzw = _Mask.Sample(sampler_Mask, r1.xy).xyzw;
  r1.x = r4.y + r2.x;
  r1.x = r1.x + r1.z;
  r1.x = r1.x + r3.w;
  r1.x = r1.x * r1.x;
  o0.xyz = r1.xxx * r5.xxx + r0.xyz;
  o0.w = r0.w;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
