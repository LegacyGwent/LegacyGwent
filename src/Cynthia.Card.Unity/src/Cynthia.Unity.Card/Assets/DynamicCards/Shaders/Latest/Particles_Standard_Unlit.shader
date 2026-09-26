Shader "DynamicCards/Latest/Particles_Standard_Unlit" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Color("_Color",Color)=(1.0,1.0,1.0,1.0)
_Cutoff("_Cutoff",Float)=0.5
_BumpScale("_BumpScale",Float)=1.0
_BumpMap("_BumpMap",2D)="bump" {}
_EmissionColor("_EmissionColor",Color)=(0.0,0.0,0.0,1.0)
_EmissionMap("_EmissionMap",2D)="white" {}
_DistortionStrength("_DistortionStrength",Float)=1.0
_DistortionBlend("_DistortionBlend",Float)=0.5
_SoftParticlesNearFadeDistance("_SoftParticlesNearFadeDistance",Float)=0.0
_SoftParticlesFarFadeDistance("_SoftParticlesFarFadeDistance",Float)=1.0
_CameraNearFadeDistance("_CameraNearFadeDistance",Float)=1.0
_CameraFarFadeDistance("_CameraFarFadeDistance",Float)=2.0
_Mode("_Mode",Float)=0.0
_ColorMode("_ColorMode",Float)=0.0
_FlipbookMode("_FlipbookMode",Float)=0.0
_LightingEnabled("_LightingEnabled",Float)=0.0
_DistortionEnabled("_DistortionEnabled",Float)=0.0
_EmissionEnabled("_EmissionEnabled",Float)=0.0
_BlendOp("_BlendOp",Float)=0.0
_SrcBlend("_SrcBlend",Float)=1.0
_DstBlend("_DstBlend",Float)=0.0
_ZWrite("_ZWrite",Float)=1.0
_Cull("_Cull",Float)=2.0
_SoftParticlesEnabled("_SoftParticlesEnabled",Float)=0.0
_CameraFadingEnabled("_CameraFadingEnabled",Float)=0.0
_SoftParticleFadeParams("_SoftParticleFadeParams",Vector)=(0.0,0.0,0.0,0.0)
_CameraFadeParams("_CameraFadeParams",Vector)=(0.0,0.0,0.0,0.0)
_ColorAddSubDiff("_ColorAddSubDiff",Vector)=(0.0,0.0,0.0,0.0)
_DistortionStrengthScaled("_DistortionStrengthScaled",Float)=0.0
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "PerformanceChecks"="False" "PreviewType"="Plane" "RenderType"="Opaque"}
GrabPass { "_GrabTexture" }
Pass {
Cull [_Cull]
ZWrite [_ZWrite]
ZTest LEqual
Blend [_SrcBlend] [_DstBlend]
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_MainTex;
Texture2D<float4> _MainTex;














void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float4 v2 : COLOR0,
  float2 v3 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : COLOR0,
  out float2 o2 : TEXCOORD1)
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
dc_v_cb2[17]=float4(0,0,0,0);
dc_v_cb2[18]=float4(0,0,0,0);
dc_v_cb2[19]=float4(0,0,0,0);
dc_v_cb2[20]=float4(0,0,0,0);
float4 dc_v_cb1[4];
dc_v_cb1[0]=float4(0,0,0,0);
dc_v_cb1[1]=float4(0,0,0,0);
dc_v_cb1[2]=float4(0,0,0,0);
dc_v_cb1[3]=float4(0,0,0,0);
float4 dc_v_cb0[5];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);



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
  o0.xyzw = dc_v_cb2[20].xyzw * r0.wwww + r1.xyzw;
  o1.xyzw = v2.xyzw;
  o2.xy = v3.xy * dc_v_cb0[4].xy + dc_v_cb0[4].zw;
  return;
}














void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : COLOR0,
  float2 v2 : TEXCOORD1,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[6];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(0,0,0,0);
dc_f_cb0[5]=float4(0,0,0,0);



  float4 r0;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = _MainTex.Sample(sampler_MainTex, v2.xy).xyzw;
  r0.xyz = dc_f_cb0[5].xyz * r0.xyz;
  o0.xyz = v1.xyz * r0.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
