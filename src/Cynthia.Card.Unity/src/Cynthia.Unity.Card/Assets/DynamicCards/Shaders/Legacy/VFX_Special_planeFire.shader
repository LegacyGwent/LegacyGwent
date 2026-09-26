Shader "DynamicCards/Legacy/VFX_Special_planeFire" {
Properties {
_Distortmap("_Distortmap",2D)="white" {}
_MainSecnoisedir("_MainSecnoisedir",Vector)=(0.0,0.0,0.0,0.0)
_Distortdirmulti("_Distortdirmulti",Vector)=(0.0,0.0,0.0,0.0)
_Distortionamount("_Distortionamount",Float)=0.0
_Noisemult("_Noisemult",Float)=1.0
_Contrast("_Contrast",Float)=1.0
_Minimumcontrast("_Minimumcontrast",Float)=1.0
_Contrastexponent("_Contrastexponent",Float)=1.0
_Maincolor("_Maincolor",Color)=(0.5,0.5,0.5,1.0)
_Blurcolor("_Blurcolor",Color)=(0.5,0.5,0.5,1.0)
_Distortcontrast("_Distortcontrast",Float)=1.0
_Mainnoise("_Mainnoise",2D)="white" {}
_Secondarynoise("_Secondarynoise",2D)="white" {}
_Noisespeed("_Noisespeed",Float)=1.0
_Geometryspeed("_Geometryspeed",Float)=1.0
_Geometryscale("_Geometryscale",Float)=1.0
_Offsetamount("_Offsetamount",Float)=1.0
_Vdraw("_Vdraw",Float)=-1.0
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
SamplerState sampler_Distortmap;
SamplerState sampler_Mainnoise;
SamplerState sampler_Secondarynoise;
Texture2D<float4> _Distortmap;
Texture2D<float4> _Mainnoise;
Texture2D<float4> _Secondarynoise;
float _Contrast;
float _Contrastexponent;
float _Distortcontrast;
float _Distortionamount;
float _Geometryscale;
float _Geometryspeed;
float _Minimumcontrast;
float _Noisemult;
float _Noisespeed;
float _Offsetamount;
float _Vdraw;
float4 _Blurcolor;
float4 _Distortdirmulti;
float4 _Distortmap_ST;
float4 _MainSecnoisedir;
float4 _Maincolor;
float4 _Mainnoise_ST;
float4 _Secondarynoise_ST;
float4 _TimeEditor;
















void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0)
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
float4 dc_v_cb0[14];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(0,0,0,0);
dc_v_cb0[6]=float4(0,0,0,0);
dc_v_cb0[7]=float4(0,0,0,0);
dc_v_cb0[8]=float4(0,0,0,0);
dc_v_cb0[9]=float4(0,0,0,0);
dc_v_cb0[10]=float4(0,0,0,0);
dc_v_cb0[11]=float4(0,0,0,0);
dc_v_cb0[12]=float4(0,0,_Geometryspeed,_Geometryscale);
dc_v_cb0[13]=float4(_Offsetamount,0,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_v_cb1[0].y + dc_v_cb0[2].y;
  r0.x = dc_v_cb0[12].z * r0.x;
  r1.x = v1.x;
  r1.y = 0;
  r0.xy = r1.xy * dc_v_cb0[12].ww + r0.xx;
  r0.xy = sin(r0.xy);
  r0.xy = dc_v_cb0[13].xx * r0.xy;
  r0.z = 0;
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
  return;
}
























void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[14];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_Mainnoise_ST.x,_Mainnoise_ST.y,_Mainnoise_ST.z,_Mainnoise_ST.w);
dc_f_cb0[4]=float4(_Distortmap_ST.x,_Distortmap_ST.y,_Distortmap_ST.z,_Distortmap_ST.w);
dc_f_cb0[5]=float4(_Distortionamount,0,0,0);
dc_f_cb0[6]=float4(_Secondarynoise_ST.x,_Secondarynoise_ST.y,_Secondarynoise_ST.z,_Secondarynoise_ST.w);
dc_f_cb0[7]=float4(_MainSecnoisedir.x,_MainSecnoisedir.y,_MainSecnoisedir.z,_MainSecnoisedir.w);
dc_f_cb0[8]=float4(_Distortdirmulti.x,_Distortdirmulti.y,_Distortdirmulti.z,_Distortdirmulti.w);
dc_f_cb0[9]=float4(_Noisemult,_Contrast,_Contrastexponent,_Minimumcontrast);
dc_f_cb0[10]=float4(_Blurcolor.x,_Blurcolor.y,_Blurcolor.z,_Blurcolor.w);
dc_f_cb0[11]=float4(_Maincolor.x,_Maincolor.y,_Maincolor.z,_Maincolor.w);
dc_f_cb0[12]=float4(_Distortcontrast,_Noisespeed,0,0);
dc_f_cb0[13]=float4(0,_Vdraw,0,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r0.yz = -dc_f_cb0[8].xy * r0.xx + v1.xy;
  r0.yz = r0.yz * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r1.xyzw = _Distortmap.Sample(sampler_Distortmap, r0.yz).xyzw;
  r1.xyzw = float4(-0.5,-0.5,-0.5,-0.5) + r1.xyxy;
  r1.xyzw = dc_f_cb0[12].xxxx * r1.xyzw;
  r1.xyzw = dc_f_cb0[5].xxxx * r1.xyzw;
  r0.y = 1 + -v1.y;
  r0.z = v1.y * r0.y;
  r0.y = saturate(-dc_f_cb0[13].y + r0.y);
  r2.xy = -r0.zz * float2(4,4) + float2(1,2);
  r0.z = 4 * r0.z;
  r1.xyzw = r2.yyyy * r1.xyzw;
  r0.w = log2(r2.x);
  r0.w = dc_f_cb0[9].z * r0.w;
  r0.w = exp2(r0.w);
  r0.w = r0.w * dc_f_cb0[9].y + dc_f_cb0[9].w;
  r2.xyzw = dc_f_cb0[12].yyyy * dc_f_cb0[7].xyzw;
  r2.xyzw = -r0.xxxx * r2.xyzw + v1.xyxy;
  r1.xyzw = r1.xyzw * dc_f_cb0[8].zwzw + r2.xyzw;
  r1.xy = r1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.zw = r1.zw * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r2.xyzw = _Secondarynoise.Sample(sampler_Secondarynoise, r1.zw).xyzw;
  r1.xyzw = _Mainnoise.Sample(sampler_Mainnoise, r1.xy).xyzw;
  r0.x = r1.x * r2.x + -0.5;
  r1.x = r1.y * r2.y;
  r1.xyz = dc_f_cb0[10].xyz * r1.xxx;
  r0.x = r0.x * r0.w + 0.5;
  r1.xyz = dc_f_cb0[11].xyz * r0.xxx + r1.xyz;
  r1.xyz = dc_f_cb0[9].xxx * r1.xyz;
  r0.xzw = r1.xyz * r0.zzz;
  o0.xyz = r0.yyy * r0.xzw;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
