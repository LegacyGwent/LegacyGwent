Shader "DynamicCards/Native/VFX_Common_AB_noiseMask" {
Properties {
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_TintColor2("_TintColor2",Color)=(1.0,1.0,1.0,1.0)
_Sharpness("_Sharpness",Float)=1.0
_Shade("_Shade",Float)=0.0
_OpacityContrast("_OpacityContrast",Float)=2.799999952316284
_OpacitySize("_OpacitySize",Float)=0.0
_TiemSpeed("_TiemSpeed",Float)=1.0
_Min("_Min",Float)=-20.0
_Max("_Max",Float)=20.0
_Noise3("_Noise3",2D)="white" {}
_NoiseSpeed2("_NoiseSpeed2",Float)=0.0
_SubOpaCont("_SubOpaCont",Float)=2.799999952316284
_SubOpaSize("_SubOpaSize",Float)=0.1899999976158142
_Noise01("_Noise01",2D)="white" {}
_Sub_size("_Sub_size",Float)=1.0
_DropSize("_DropSize",Float)=1.0
_DropNoiseSize("_DropNoiseSize",Float)=0.0
_MaskMovea("_MaskMovea",Float)=0.0
_UVNoise("_UVNoise",2D)="white" {}
_SpecColour("_SpecColour",Color)=(1.0,1.0,1.0,1.0)
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
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
SamplerState sampler_Noise01;
SamplerState sampler_Noise3;
SamplerState sampler_UVNoise;
Texture2D<float4> _Noise01;
Texture2D<float4> _Noise3;
Texture2D<float4> _UVNoise;
float _DropNoiseSize;
float _DropSize;
float _MaskMovea;
float _Max;
float _Min;
float _NoiseSpeed2;
float _OpacityContrast;
float _OpacitySize;
float _Shade;
float _Sharpness;
float _SubOpaCont;
float _SubOpaSize;
float _Sub_size;
float _TiemSpeed;
float4 _Noise01_ST;
float4 _Noise3_ST;
float4 _SpecColour;
float4 _TintColor2;
float4 _TintColor;
float4 _UVNoise_ST;












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
  uint v2 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[13];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[3]=float4(_OpacityContrast,_TiemSpeed,_Min,_Max);
dc_f_cb0[4]=float4(_OpacitySize,0,0,0);
dc_f_cb0[5]=float4(_Noise3_ST.x,_Noise3_ST.y,_Noise3_ST.z,_Noise3_ST.w);
dc_f_cb0[6]=float4(_NoiseSpeed2,_SubOpaCont,_SubOpaSize,0);
dc_f_cb0[7]=float4(_Noise01_ST.x,_Noise01_ST.y,_Noise01_ST.z,_Noise01_ST.w);
dc_f_cb0[8]=float4(_Sub_size,_DropSize,_DropNoiseSize,0);
dc_f_cb0[9]=float4(_TintColor2.x,_TintColor2.y,_TintColor2.z,_TintColor2.w);
dc_f_cb0[10]=float4(_Sharpness,_Shade,_MaskMovea,0);
dc_f_cb0[11]=float4(_UVNoise_ST.x,_UVNoise_ST.y,_UVNoise_ST.z,_UVNoise_ST.w);
dc_f_cb0[12]=float4(_SpecColour.x,_SpecColour.y,_SpecColour.z,_SpecColour.w);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb1[0].x * dc_f_cb0[6].x;
  r0.x = r0.x * 0.5 + v1.x;
  r0.y = v1.y;
  r0.xy = r0.xy * dc_f_cb0[11].xy + dc_f_cb0[11].zw;
  r0.xyzw = _UVNoise.Sample(sampler_UVNoise, r0.xy).xyzw;
  r1.z = dc_f_cb1[0].x * dc_f_cb0[6].x + v1.x;
  r1.yw = v1.yy;
  r0.yz = r1.zw * dc_f_cb0[11].xy + dc_f_cb0[11].zw;
  r2.xyzw = _UVNoise.Sample(sampler_UVNoise, r0.yz).xyzw;
  r0.x = r2.x + r0.x;
  r0.x = -1 + r0.x;
  r0.y = 1 + -v1.y;
  r0.y = saturate(v1.y * r0.y);
  r0.z = saturate(dc_f_cb0[6].y * r0.y + dc_f_cb0[6].z);
  r0.y = saturate(r0.y * dc_f_cb0[3].x + dc_f_cb0[4].x);
  r0.y = r0.y * 2 + -1;
  r0.z = 1 + -r0.z;
  r0.z = r0.z * 2 + -1;
  r0.w = 1.5 * dc_f_cb0[8].y;
  r1.x = dc_f_cb1[0].x * dc_f_cb0[3].y + v1.x;
  r1.zw = dc_f_cb0[8].xx * r1.xy;
  r1.zw = r1.zw * dc_f_cb0[7].xy + dc_f_cb0[7].zw;
  r2.xyzw = _Noise01.Sample(sampler_Noise01, r1.zw).xyzw;
  r0.w = r2.x / r0.w;
  r1.z = dc_f_cb0[10].y * r2.x;
  r0.z = r0.z + -r0.w;
  r0.z = r0.x * dc_f_cb0[8].z + r0.z;
  r0.z = 1 + r0.z;
  r0.w = dc_f_cb0[10].z + v1.x;
  r0.w = log2(r0.w);
  r0.w = 20 * r0.w;
  r0.w = exp2(r0.w);
  r0.w = min(1, r0.w);
  r2.xy = r1.xy * dc_f_cb0[7].xy + dc_f_cb0[7].zw;
  r1.xy = r1.xy * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r3.xyzw = _Noise3.Sample(sampler_Noise3, r1.xy).xyzw;
  r1.x = log2(r3.x);
  r1.x = dc_f_cb0[10].x * r1.x;
  r1.x = exp2(r1.x);
  r1.x = min(1, r1.x);
  r2.xyzw = _Noise01.Sample(sampler_Noise01, r2.xy).xyzw;
  r0.y = -r2.x * 0.666666687 + r0.y;
  r0.x = r0.x * dc_f_cb0[8].z + r0.y;
  r0.x = 1 + r0.x;
  r0.xz = saturate(r0.wz * r0.xw);
  r0.x = r0.x + -r0.z;
  r0.y = r0.x * 2 + -1;
  r0.y = saturate(r0.y * r1.z);
  r1.yzw = -dc_f_cb0[9].xyz + dc_f_cb0[2].xyz;
  r1.xyz = r1.xxx * r1.yzw + dc_f_cb0[9].xyz;
  o0.xyz = r0.yyy * dc_f_cb0[12].xyz + r1.xyz;
  r0.y = dc_f_cb0[3].w + -dc_f_cb0[3].z;
  o0.w = r0.x * r0.y + dc_f_cb0[3].z;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
