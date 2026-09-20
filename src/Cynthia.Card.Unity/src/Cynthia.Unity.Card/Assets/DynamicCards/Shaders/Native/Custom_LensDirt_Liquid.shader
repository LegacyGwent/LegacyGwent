Shader "DynamicCards/Native/Custom_LensDirt_Liquid" {
Properties {
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_Distortionpower("_Distortionpower",Float)=0.0
_MainText("_MainText",2D)="white" {}
_DistortStrength("_DistortStrength",Float)=0.0
_DropSize("_DropSize",Float)=1.0
_Mask("_Mask",2D)="white" {}
_OpacityClip("_OpacityClip",Float)=1.0
_Brightnes("_Brightnes",Float)=0.0
_Contrast("_Contrast",Float)=1.0
_VTEturetiling("_VTEturetiling",Float)=1.0
_UTextureTiling("_UTextureTiling",Float)=1.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"QUEUE"="Transparent" "RenderType"="Overlay"}
GrabPass { "Refraction" }
Pass {
Cull Off
ZWrite Off
ZTest LEqual
Blend One Zero
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState samplerRefraction;
SamplerState sampler_MainText;
SamplerState sampler_Mask;
Texture2D<float4> Refraction;
Texture2D<float4> _MainText;
Texture2D<float4> _Mask;
float _Brightnes;
float _Contrast;
float _DistortStrength;
float _Distortionpower;
float _DropSize;
float _OpacityClip;
float _UTextureTiling;
float _VTEturetiling;
float4 _MainText_ST;
float4 _Mask_ST;
float4 _TintColor;














void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
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
float4 dc_v_cb1[4];
dc_v_cb1[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb1[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb1[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb1[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
float4 dc_v_cb0[6];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(_ProjectionParams.x,_ProjectionParams.y,_ProjectionParams.z,_ProjectionParams.w);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb1[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb1[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[2].xyzw * v0.zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb1[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb2[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb2[19].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb2[20].xyzw * r0.wwww + r1.xyzw;
  o0.xyzw = r0.xyzw;
  r0.xy = r0.xy / r0.ww;
  o1.xy = v1.xy;
  r0.z = dc_v_cb0[5].x * r0.y;
  r0.w = 0;
  o2.xyzw = r0.xzww;
  o3.xyzw = v2.xyzw;
  return;
}
























void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float4 v3 : COLOR0,
  uint v4 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[6];
dc_f_cb1[0]=float4(0,0,0,0);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(0,0,0,0);
dc_f_cb1[5]=float4(_ProjectionParams.x,_ProjectionParams.y,_ProjectionParams.z,_ProjectionParams.w);
float4 dc_f_cb0[9];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[3]=float4(_Distortionpower,0,0,0);
dc_f_cb0[4]=float4(_MainText_ST.x,_MainText_ST.y,_MainText_ST.z,_MainText_ST.w);
dc_f_cb0[5]=float4(_DistortStrength,_DropSize,0,0);
dc_f_cb0[6]=float4(_Mask_ST.x,_Mask_ST.y,_Mask_ST.z,_Mask_ST.w);
dc_f_cb0[7]=float4(_OpacityClip,_Brightnes,_Contrast,_VTEturetiling);
dc_f_cb0[8]=float4(_UTextureTiling,0,0,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v2.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r0.xyzw = _MainText.Sample(sampler_MainText, r0.xy).xyzw;
  r0.x = dc_f_cb0[5].x + r0.x;
  r0.x = saturate(dc_f_cb0[5].y * r0.x);
  r0.yz = v1.xy * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r1.xyzw = _Mask.Sample(sampler_Mask, r0.yz).xyzw;
  r0.xyzw = r1.xyzw * r0.xxxx;
  r1.x = dc_f_cb0[7].x * r0.x;
  r1.x = r1.x * r0.w;
  r1.x = r1.x * v3.w + -0.5;
  r1.x = cmp(r1.x < 0);
  if (r1.x != 0) discard;
  r1.xy = float2(1,1) + -v1.xy;
  r1.x = dc_f_cb0[8].x * r1.x;
  r1.y = dc_f_cb0[7].w * r1.y;
  r2.xy = frac(r1.xy);
  r1.xy = r2.xy * float2(2,2) + float2(-1,-1);
  r1.z = dot(r1.xy, r1.xy);
  r1.xy = dc_f_cb0[3].xx * r1.xy;
  r1.z = sqrt(r1.z);
  r1.z = 1 + -r1.z;
  r1.xy = r1.xy * r1.zz;
  r1.xy = max(float2(-1,-1), r1.xy);
  r1.xy = min(float2(1,1), r1.xy);
  r1.xy = r1.xy * r0.xx;
  r1.xy = r1.xy * r0.ww;
  r2.y = -dc_f_cb1[5].x * v2.y;
  r2.x = v2.x;
  r1.zw = r2.xy * float2(0.5,0.5) + float2(0.5,0.5);
  r1.xy = r1.xy * v3.xx + r1.zw;
  r1.xyzw = Refraction.Sample(samplerRefraction, r1.xy).xyzw;
  r2.xyz = r0.xyz * r0.www;
  r2.xyz = dc_f_cb0[2].xyz * r2.xyz;
  r2.xyz = dc_f_cb0[2].www * r2.xyz;
  r2.xyz = r2.xyz * dc_f_cb0[7].zzz + dc_f_cb0[7].yyy;
  r2.xyz = r2.xyz + -r1.xyz;
  r0.y = dc_f_cb0[2].w * r0.w;
  r0.x = r0.y * r0.x;
  r0.x = v3.y * r0.x;
  o0.xyz = r0.xxx * r2.xyz + r1.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
