Shader "DynamicCards/Latest/UI_GeneralSoftMaskNoise" {
Properties {
_MainTex("_MainTex",2D)="bump" {}
_Color("_Color",Color)=(1.0,1.0,1.0,1.0)
_Noise("_Noise",2D)="white" {}
_NoiseContrast("_NoiseContrast",Float)=0.0
_NoiseIntensity("_NoiseIntensity",Float)=0.0
_NoisePower("_NoisePower",Float)=0.05000000074505806
_NoiseSpeed("_NoiseSpeed",Float)=0.0
_NoiseHorizontalSpeed("_NoiseHorizontalSpeed",Float)=0.0
_NoiseVerticalSpeed("_NoiseVerticalSpeed",Float)=0.0
_Cutoff("_Cutoff",Float)=0.5
PixelSnap("PixelSnap",Float)=0.0
}
SubShader {
Tags {"CanUseSpriteAtlas"="true" "IGNOREPROJECTOR"="true" "PreviewType"="Plane" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Off
ZWrite Off
ZTest LEqual
Blend One OneMinusSrcAlpha
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_MainTex;
SamplerState sampler_Noise;
Texture2D<float4> _MainTex;
Texture2D<float4> _Noise;
float _NoiseContrast;
float _NoiseHorizontalSpeed;
float _NoiseIntensity;
float _NoisePower;
float _NoiseSpeed;
float _NoiseVerticalSpeed;
float4 _Color;
float4 _MainTex_ST;
float4 _Noise_ST;












void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
  out float4 o2 : COLOR0)
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
dc_v_cb1[17]=float4(0,0,0,0);
dc_v_cb1[18]=float4(0,0,0,0);
dc_v_cb1[19]=float4(0,0,0,0);
dc_v_cb1[20]=float4(0,0,0,0);
float4 dc_v_cb0[4];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);



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
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[8];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_Color.x,_Color.y,_Color.z,_Color.w);
dc_f_cb0[4]=float4(_NoiseSpeed,0,0,0);
dc_f_cb0[5]=float4(_Noise_ST.x,_Noise_ST.y,_Noise_ST.z,_Noise_ST.w);
dc_f_cb0[6]=float4(_NoiseContrast,_NoisePower,_NoiseIntensity,_NoiseHorizontalSpeed);
dc_f_cb0[7]=float4(_NoiseVerticalSpeed,0,0,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb1[0].x * dc_f_cb0[4].x;
  r1.xyzw = r0.xxxx * float4(1,0.100000001,-1,-0.129999995) + v1.xyxy;
  r0.xyzw = r0.xxxx * float4(-0.200000003,-0.0900000036,-0.0500000007,0.529999971) + v1.xyxy;
  r2.xyzw = float4(0,1,0,1) * dc_f_cb1[0].yyyy;
  r1.xyzw = r2.wzwz * dc_f_cb0[6].wwww + r1.xyzw;
  r1.xyzw = r2.xyzw * dc_f_cb0[7].xxxx + r1.xyzw;
  r1.xyzw = r1.xyzw * dc_f_cb0[5].xyxy + dc_f_cb0[5].zwzw;
  r3.xyzw = _Noise.Sample(sampler_Noise, r1.xy).xyzw;
  r1.xyzw = _Noise.Sample(sampler_Noise, r1.zw).xyzw;
  r1.x = r3.x + r1.y;
  r0.xyzw = r2.wzwz * dc_f_cb0[6].wwww + r0.xyzw;
  r0.xyzw = r2.xyzw * dc_f_cb0[7].xxxx + r0.xyzw;
  r0.xyzw = r0.xyzw * dc_f_cb0[5].xyxy + dc_f_cb0[5].zwzw;
  r2.xyzw = _Noise.Sample(sampler_Noise, r0.xy).xyzw;
  r0.xyzw = _Noise.Sample(sampler_Noise, r0.zw).xyzw;
  r0.x = r2.z + r1.x;
  r0.x = r0.x + r0.w;
  r0.x = dc_f_cb0[6].x / r0.x;
  r0.xyz = dc_f_cb0[3].xyz * r0.xxx;
  r0.xyz = log2(r0.xyz);
  r0.xyz = dc_f_cb0[6].yyy * r0.xyz;
  r0.xyz = exp2(r0.xyz);
  r0.xyz = min(float3(1,1,1), r0.xyz);
  r0.xyz = dc_f_cb0[6].zzz * r0.xyz;
  r1.xy = v1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r1.xy).xyzw;
  r1.xyzw = dc_f_cb0[3].wxyz * r1.wxyz;
  r1.xyzw = v2.wxyz * r1.xyzw;
  r1.yzw = r1.yzw * r1.xxx;
  o0.w = r1.x;
  o0.xyz = r0.xyz * r1.yzw + r1.yzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
