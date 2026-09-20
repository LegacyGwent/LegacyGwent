Shader "DynamicCards/Native/VFX_Common_AlphaBlended_wavey" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Usetextureoffset("_Usetextureoffset",Float)=1.0
_Offsettexture("_Offsettexture",2D)="white" {}
_UseUorV("_UseUorV",Float)=0.0
_Timemultiplier("_Timemultiplier",Float)=1.0
_Timeoffset("_Timeoffset",Float)=0.0
_Offsetamount("_Offsetamount",Float)=0.10000000149011612
_Wavelength("_Wavelength",Float)=1.0
_UVmidpoint("_UVmidpoint",Float)=0.0
_Elementwidth("_Elementwidth",Float)=1.0
_StartEndexponent("_StartEndexponent",Float)=1.0
_cutoff("_cutoff",Float)=1.0
_VerticalHorizontal("_VerticalHorizontal",Float)=0.0
_VColourOn("_VColourOn",Float)=0.0
_Texturetimeoffset("_Texturetimeoffset",Float)=0.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"QUEUE"="AlphaTest" "RenderType"="TransparentCutout"}
Pass {
Cull Off
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
SamplerState sampler_Offsettexture;
Texture2D<float4> _MainTex;
Texture2D<float4> _Offsettexture;
float _Elementwidth;
float _Offsetamount;
float _StartEndexponent;
float _Texturetimeoffset;
float _Timemultiplier;
float _Timeoffset;
float _UVmidpoint;
float _UseUorV;
float _Usetextureoffset;
float _VColourOn;
float _VerticalHorizontal;
float _Wavelength;
float _cutoff;
float4 _MainTex_ST;
float4 _Offsettexture_ST;




















void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
  out float4 o2 : COLOR0)
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
float4 dc_v_cb2[4];
dc_v_cb2[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb2[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb2[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb2[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
float4 dc_v_cb1[1];
dc_v_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_v_cb0[8];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(_UseUorV,_Timemultiplier,_Offsetamount,_Wavelength);
dc_v_cb0[4]=float4(_UVmidpoint,_Elementwidth,_StartEndexponent,_Timeoffset);
dc_v_cb0[5]=float4(0,_Usetextureoffset,0,0);
dc_v_cb0[6]=float4(_Offsettexture_ST.x,_Offsettexture_ST.y,_Offsettexture_ST.z,_Offsettexture_ST.w);
dc_v_cb0[7]=float4(_VerticalHorizontal,_VColourOn,_Texturetimeoffset,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = v1.y + -v1.x;
  r0.x = dc_v_cb0[3].x * r0.x + v1.x;
  r0.x = -dc_v_cb0[4].x + r0.x;
  r0.y = dc_v_cb1[0].y * dc_v_cb0[3].y;
  r0.y = r0.x * dc_v_cb0[3].w + r0.y;
  r0.y = dc_v_cb0[4].w + r0.y;
  r0.y = v2.w * dc_v_cb0[7].z + r0.y;
  r0.y = sin(r0.y);
  r1.y = dc_v_cb0[3].z * r0.y;
  r1.xz = float2(0,0);
  r0.yzw = r1.yzz + -r1.zyz;
  r0.yzw = dc_v_cb0[7].xxx * r0.yzw + r1.xyz;
  r1.x = 1 / dc_v_cb0[4].y;
  r0.x = r1.x * r0.x;
  r0.x = min(1, abs(r0.x));
  r0.x = log2(r0.x);
  r0.x = dc_v_cb0[4].z * r0.x;
  r0.x = exp2(r0.x);
  r0.xyz = r0.xxx * r0.yzw;
  r1.xy = v1.xy * dc_v_cb0[6].xy + dc_v_cb0[6].zw;
  r1.xyzw = _Offsettexture.SampleLevel(sampler_Offsettexture, r1.xy, 0).xyzw;
  r0.w = -1 + r1.z;
  r0.w = dc_v_cb0[5].y * r0.w + 1;
  r1.xyz = r0.xyz * r0.www;
  r0.xyz = r0.xyz * v2.zzz + -r1.xyz;
  r0.xyz = dc_v_cb0[7].yyy * r0.xyz + r1.xyz;
  r0.xyz = v0.xyz + r0.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
  o1.xy = v1.xy;
  o2.xyzw = v2.xyzw;
  return;
}














void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  uint v3 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[6];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(0,0,0,0);
dc_f_cb0[5]=float4(_cutoff,0,0,0);



  float4 r0;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.w = r0.w * dc_f_cb0[5].x + -0.5;
  o0.xyz = r0.xyz;
  r0.x = cmp(r0.w < 0);
  if (r0.x != 0) discard;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
