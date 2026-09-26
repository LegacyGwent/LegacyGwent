Shader "DynamicCards/Native/Custom_ApearShader" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Strenght("_Strenght",Float)=1.0
_Border("_Border",Float)=1.0
_NoiseStr("_NoiseStr",Float)=1.0
_TimeMulti("_TimeMulti",Float)=1.0
_Noise("_Noise",2D)="white" {}
_Reflection("_Reflection",Float)=0.0
_TintCol("_TintCol",Color)=(0.0,0.0,0.0,0.0)
_Strenght_Burn("_Strenght_Burn",Float)=1.0
_BurnValue("_BurnValue",Float)=1.0
_OpacityBlur("_OpacityBlur",Float)=1.0
_FrenselValue("_FrenselValue",Float)=1.0
_opacityColor("_opacityColor",Color)=(1.0,1.0,1.0,1.0)
_NoiseSmall("_NoiseSmall",2D)="white" {}
_Mask("_Mask",2D)="white" {}
_MaskVertical("_MaskVertical",Float)=0.0
_MaskBlur("_MaskBlur",Float)=1.0
_somfReflect("_somfReflect",Float)=1.0
_Opacity2("_Opacity2",Float)=0.0
_FresnelColor("_FresnelColor",Color)=(1.0,1.0,1.0,1.0)
_MaskStrenght("_MaskStrenght",Float)=0.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
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
SamplerState sampler_Mask;
SamplerState sampler_Noise;
SamplerState sampler_NoiseSmall;
Texture2D<float4> _GrabTexture;
Texture2D<float4> _MainTex;
Texture2D<float4> _Mask;
Texture2D<float4> _Noise;
Texture2D<float4> _NoiseSmall;
float _Border;
float _BurnValue;
float _FrenselValue;
float _MaskBlur;
float _MaskStrenght;
float _MaskVertical;
float _NoiseStr;
float _Opacity2;
float _OpacityBlur;
float _Reflection;
float _Strenght;
float _Strenght_Burn;
float _TimeMulti;
float _somfReflect;
float4 _FresnelColor;
float4 _MainTex_ST;
float4 _Mask_ST;
float4 _NoiseSmall_ST;
float4 _Noise_ST;
float4 _TintCol;
float4 _opacityColor;














void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float2 v2 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float4 o3 : TEXCOORD2,
  out float4 o4 : TEXCOORD3)
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
dc_v_cb2[9]=float4(unity_MatrixV[0][0],unity_MatrixV[1][0],unity_MatrixV[2][0],unity_MatrixV[3][0]);
dc_v_cb2[10]=float4(unity_MatrixV[0][1],unity_MatrixV[1][1],unity_MatrixV[2][1],unity_MatrixV[3][1]);
dc_v_cb2[11]=float4(unity_MatrixV[0][2],unity_MatrixV[1][2],unity_MatrixV[2][2],unity_MatrixV[3][2]);
dc_v_cb2[12]=float4(unity_MatrixV[0][3],unity_MatrixV[1][3],unity_MatrixV[2][3],unity_MatrixV[3][3]);
dc_v_cb2[13]=float4(0,0,0,0);
dc_v_cb2[14]=float4(0,0,0,0);
dc_v_cb2[15]=float4(0,0,0,0);
dc_v_cb2[16]=float4(0,0,0,0);
dc_v_cb2[17]=float4(unity_MatrixVP[0][0],unity_MatrixVP[1][0],unity_MatrixVP[2][0],unity_MatrixVP[3][0]);
dc_v_cb2[18]=float4(unity_MatrixVP[0][1],unity_MatrixVP[1][1],unity_MatrixVP[2][1],unity_MatrixVP[3][1]);
dc_v_cb2[19]=float4(unity_MatrixVP[0][2],unity_MatrixVP[1][2],unity_MatrixVP[2][2],unity_MatrixVP[3][2]);
dc_v_cb2[20]=float4(unity_MatrixVP[0][3],unity_MatrixVP[1][3],unity_MatrixVP[2][3],unity_MatrixVP[3][3]);
float4 dc_v_cb1[7];
dc_v_cb1[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb1[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb1[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb1[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
dc_v_cb1[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_v_cb1[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_v_cb1[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);
float4 dc_v_cb0[6];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(_ProjectionParams.x,_ProjectionParams.y,_ProjectionParams.z,_ProjectionParams.w);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb1[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb1[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[2].xyzw * v0.zzzz + r0.xyzw;
  r1.xyzw = dc_v_cb1[3].xyzw + r0.xyzw;
  o2.xyzw = dc_v_cb1[3].xyzw * v0.wwww + r0.xyzw;
  r0.xyzw = dc_v_cb2[18].xyzw * r1.yyyy;
  r0.xyzw = dc_v_cb2[17].xyzw * r1.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb2[19].xyzw * r1.zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb2[20].xyzw * r1.wwww + r0.xyzw;
  o0.xyzw = r0.xyzw;
  o1.xy = v2.xy;
  r2.x = dot(v1.xyz, dc_v_cb1[4].xyz);
  r2.y = dot(v1.xyz, dc_v_cb1[5].xyz);
  r2.z = dot(v1.xyz, dc_v_cb1[6].xyz);
  r0.z = dot(r2.xyz, r2.xyz);
  r0.z = rsqrt(r0.z);
  o3.xyz = r2.xyz * r0.zzz;
  r0.z = dc_v_cb2[10].z * r1.y;
  r0.z = dc_v_cb2[9].z * r1.x + r0.z;
  r0.z = dc_v_cb2[11].z * r1.z + r0.z;
  r0.z = dc_v_cb2[12].z * r1.w + r0.z;
  o4.z = -r0.z;
  r0.y = dc_v_cb0[5].x * r0.y;
  r1.xzw = float3(0.5,0.5,0.5) * r0.xwy;
  o4.w = r0.w;
  o4.xy = r1.xw + r1.zz;
  return;
}
































void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float4 v3 : TEXCOORD2,
  float4 v4 : TEXCOORD3,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[5];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(_WorldSpaceCameraPos.x,_WorldSpaceCameraPos.y,_WorldSpaceCameraPos.z,0);
float4 dc_f_cb0[14];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_Strenght,_Border,_NoiseStr,_TimeMulti);
dc_f_cb0[4]=float4(_Noise_ST.x,_Noise_ST.y,_Noise_ST.z,_Noise_ST.w);
dc_f_cb0[5]=float4(_Reflection,0,0,0);
dc_f_cb0[6]=float4(_TintCol.x,_TintCol.y,_TintCol.z,_TintCol.w);
dc_f_cb0[7]=float4(_Strenght_Burn,_BurnValue,_OpacityBlur,_FrenselValue);
dc_f_cb0[8]=float4(_opacityColor.x,_opacityColor.y,_opacityColor.z,_opacityColor.w);
dc_f_cb0[9]=float4(_NoiseSmall_ST.x,_NoiseSmall_ST.y,_NoiseSmall_ST.z,_NoiseSmall_ST.w);
dc_f_cb0[10]=float4(_Mask_ST.x,_Mask_ST.y,_Mask_ST.z,_Mask_ST.w);
dc_f_cb0[11]=float4(_MaskVertical,_MaskBlur,_somfReflect,_Opacity2);
dc_f_cb0[12]=float4(_FresnelColor.x,_FresnelColor.y,_FresnelColor.z,_FresnelColor.w);
dc_f_cb0[13]=float4(_MaskStrenght,0,0,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyz = dc_f_cb1[4].xyz + -v2.xyz;
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  r0.xyz = r0.xyz * r0.www;
  r0.w = dot(v3.xyz, v3.xyz);
  r0.w = rsqrt(r0.w);
  r1.xyz = v3.xyz * r0.www;
  r0.x = dot(r1.xyz, r0.xyz);
  r0.x = max(0, r0.x);
  r0.x = 1 + -r0.x;
  r0.x = log2(r0.x);
  r0.x = dc_f_cb0[7].w * r0.x;
  r0.x = exp2(r0.x);
  r0.x = r0.x + r0.x;
  r0.y = r0.x * r0.x;
  r0.zw = v1.xy * dc_f_cb0[9].xy + dc_f_cb0[9].zw;
  r1.xyzw = _NoiseSmall.Sample(sampler_NoiseSmall, r0.zw).xyzw;
  r0.zw = v1.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r2.xyzw = _Noise.Sample(sampler_Noise, r0.zw).xyzw;
  r0.z = -r2.y + r1.y;
  r0.w = dc_f_cb0[11].x + v1.y;
  r0.w = saturate(dc_f_cb0[11].y * r0.w);
  r0.z = r0.w * r0.z + r2.y;
  r1.xy = dc_f_cb1[0].yy * dc_f_cb0[3].ww + v1.xy;
  r1.zw = r1.xy * dc_f_cb0[9].xy + dc_f_cb0[9].zw;
  r2.xyzw = _NoiseSmall.Sample(sampler_NoiseSmall, r1.zw).xyzw;
  r1.zw = r1.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r1.xy = dc_f_cb0[11].zz * r1.xy;
  r1.xy = r1.xy * dc_f_cb0[9].xy + dc_f_cb0[9].zw;
  r3.xyzw = _NoiseSmall.Sample(sampler_NoiseSmall, r1.xy).xyzw;
  r1.xy = dc_f_cb0[5].xx * r3.xy;
  r3.xyzw = _Noise.Sample(sampler_Noise, r1.zw).xyzw;
  r1.zw = -r3.xy + r2.xy;
  r1.zw = r0.ww * r1.zw + r3.xy;
  r0.w = saturate(dc_f_cb0[11].w + r0.w);
  r0.z = r1.z * r0.z;
  r1.zw = r1.zw * dc_f_cb0[3].zz + v1.xy;
  r1.zw = r1.zw * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r2.xyzw = _MainTex.Sample(sampler_MainTex, r1.zw).xyzw;
  r1.zw = v1.xy * dc_f_cb0[10].xy + dc_f_cb0[10].zw;
  r3.xyzw = _Mask.Sample(sampler_Mask, r1.zw).xyzw;
  r1.z = saturate(dc_f_cb0[13].x + r3.x);
  r1.w = dc_f_cb0[3].x * r1.z;
  r1.z = dc_f_cb0[7].x * r1.z;
  r1.z = r1.z * r0.z;
  r0.z = r1.w * r0.z;
  r0.z = log2(r0.z);
  r0.z = dc_f_cb0[7].z * r0.z;
  r0.z = exp2(r0.z);
  r0.z = min(1, r0.z);
  r0.z = 1 + -r0.z;
  r1.z = log2(r1.z);
  r1.z = dc_f_cb0[3].y * r1.z;
  r1.z = exp2(r1.z);
  r1.z = min(1, r1.z);
  r3.xyz = -r1.zzz * dc_f_cb0[6].xyz + float3(1,1,1);
  r3.xyz = dc_f_cb0[7].yyy * r3.xyz;
  r2.xyz = r2.xyz / r3.xyz;
  r0.x = r0.y * r0.x + r0.z;
  r3.xyz = dc_f_cb0[12].xyz + -dc_f_cb0[8].xyz;
  r3.xyz = r0.xxx * r3.xyz + dc_f_cb0[8].xyz;
  r0.x = saturate(dc_f_cb0[8].w * r0.w + r0.x);
  r0.x = r2.w * r0.x;
  r2.xyz = -r3.xyz + r2.xyz;
  r0.yzw = r0.zzz * r2.xyz + r3.xyz;
  r1.zw = v4.xy / v4.ww;
  r1.xy = r1.xy * r2.ww + r1.zw;
  r1.xyzw = _GrabTexture.Sample(sampler_GrabTexture, r1.xy).xyzw;
  r0.yzw = -r1.xyz + r0.yzw;
  o0.xyz = r0.xxx * r0.yzw + r1.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
