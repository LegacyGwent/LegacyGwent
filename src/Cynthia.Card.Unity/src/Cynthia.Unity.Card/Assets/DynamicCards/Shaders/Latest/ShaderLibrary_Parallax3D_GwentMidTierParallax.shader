Shader "DynamicCards/Latest/ShaderLibrary_Parallax3D_GwentMidTierParallax" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_SecondTex("_SecondTex",2D)="Grey" {}
_ThirdTex("_ThirdTex",2D)="Grey" {}
_FlowTex("_FlowTex",2D)="Grey" {}
_MatcapTex("_MatcapTex",2D)="Grey" {}
_MatcapColor("_MatcapColor",Color)=(1.0,1.0,1.0,1.0)
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_SecondTintColor("_SecondTintColor",Color)=(1.0,1.0,1.0,1.0)
_ThirdTintColor("_ThirdTintColor",Color)=(1.0,1.0,1.0,1.0)
_ParallaxIntensity("_ParallaxIntensity",Float)=0.029999999329447746
_ParallaxOffset("_ParallaxOffset",Float)=1.0
_FlowAnimSpeed("_FlowAnimSpeed",Float)=1.0
_FlowAnimDist("_FlowAnimDist",Float)=0.0
_MainTexUVAnim("_MainTexUVAnim",Vector)=(0.0,0.0,0.0,0.0)
_SecondTexUVAnim("_SecondTexUVAnim",Vector)=(0.0,0.0,0.0,0.0)
_ThirdTexUVAnim("_ThirdTexUVAnim",Vector)=(0.0,0.0,0.0,0.0)
_FlowTexUVAnim("_FlowTexUVAnim",Vector)=(0.0,0.0,0.0,0.0)
_MainTex_SSUV("_MainTex_SSUV",Float)=0.0
_SecondTex_SSUV("_SecondTex_SSUV",Float)=0.0
_ThirdTex_SSUV("_ThirdTex_SSUV",Float)=0.0
_FlowTex_SSUV("_FlowTex_SSUV",Float)=0.0
_MatcapTex_SSUV("_MatcapTex_SSUV",Float)=0.0
_AlphaClip("_AlphaClip",Float)=0.5
_ParallaxRectClipExtendX("_ParallaxRectClipExtendX",Float)=0.5099999904632568
_ParallaxRectClipExtendY("_ParallaxRectClipExtendY",Float)=0.7699999809265137
_Mode("_Mode",Float)=0.0
_SrcBlend("_SrcBlend",Float)=1.0
_DstBlend("_DstBlend",Float)=0.0
_ZWrite("_ZWrite",Float)=1.0
_Cull("_Cull",Float)=2.0
_DoubleSided("_DoubleSided",Float)=2.0
_Anim("_Anim",Float)=0.0
_Special("_Special",Float)=0.0
_Modifier("_Modifier",Float)=0.0
_TimeMultiplier("_TimeMultiplier",Float)=0.0
_ZTest("_ZTest",Float)=4.0
_Offset("_Offset",Float)=0.0
_ColorMask("_ColorMask",Float)=15.0
_SrcAlphaBlend("_SrcAlphaBlend",Float)=1.0
_DstAlphaBlend("_DstAlphaBlend",Float)=10.0
_BlendOp("_BlendOp",Float)=0.0
_Overrides("_Overrides",Float)=0.0
_TimeOffset("_TimeOffset",Float)=0.0
}
SubShader {
Tags {"RenderType"="Opaque"}
Pass {
Cull [_Cull]
ZWrite [_ZWrite]
ZTest [_ZTest]
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
float _AlphaClip;
float _Offset;
float _ParallaxOffset;
float _ParallaxRectClipExtendX;
float _ParallaxRectClipExtendY;
float4 _MainTex_ST;
float4 _TintColor;
















void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : NORMAL0,
  float4 v3 : TANGENT0,
  out float2 o0 : TEXCOORD0,
  out float4 o1 : SV_POSITION0,
  out float4 o2 : TEXCOORD2,
  out float4 o3 : TEXCOORD7,
  out float3 o4 : TEXCOORD6)
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
dc_v_cb3[17]=float4(0,0,0,0);
dc_v_cb3[18]=float4(0,0,0,0);
dc_v_cb3[19]=float4(0,0,0,0);
dc_v_cb3[20]=float4(0,0,0,0);
float4 dc_v_cb2[4];
dc_v_cb2[0]=float4(0,0,0,0);
dc_v_cb2[1]=float4(0,0,0,0);
dc_v_cb2[2]=float4(0,0,0,0);
dc_v_cb2[3]=float4(0,0,0,0);
float4 dc_v_cb1[8];
dc_v_cb1[0]=float4(0,0,0,0);
dc_v_cb1[1]=float4(0,0,0,0);
dc_v_cb1[2]=float4(0,0,0,0);
dc_v_cb1[3]=float4(0,0,0,0);
dc_v_cb1[4]=float4(_WorldSpaceCameraPos.x,_WorldSpaceCameraPos.y,_WorldSpaceCameraPos.z,0);
dc_v_cb1[5]=float4(0,0,0,0);
dc_v_cb1[6]=float4(0,0,0,0);
dc_v_cb1[7]=float4(_ZBufferParams.x,_ZBufferParams.y,_ZBufferParams.z,_ZBufferParams.w);
float4 dc_v_cb0[13];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_v_cb0[5]=float4(0,0,0,0);
dc_v_cb0[6]=float4(0,0,0,0);
dc_v_cb0[7]=float4(0,0,0,0);
dc_v_cb0[8]=float4(0,0,0,0);
dc_v_cb0[9]=float4(0,0,0,0);
dc_v_cb0[10]=float4(0,0,0,0);
dc_v_cb0[11]=float4(0,0,0,0);
dc_v_cb0[12]=float4(0,_Offset,_ParallaxOffset,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  o0.xy = v1.xy * dc_v_cb0[4].xy + dc_v_cb0[4].zw;
  r0.x = dot(dc_v_cb2[2].xyz, dc_v_cb2[2].xyz);
  r0.x = rsqrt(r0.x);
  r0.xyz = dc_v_cb2[2].xyz * r0.xxx;
  r0.w = dot(dc_v_cb2[0].xyz, dc_v_cb2[0].xyz);
  r0.w = rsqrt(r0.w);
  r1.xyz = dc_v_cb2[0].xyz * r0.www;
  r2.xyz = r1.yzx * r0.zxy;
  r2.xyz = r0.yzx * r1.zxy + -r2.xyz;
  r3.xyz = dc_v_cb2[1].xyz * v0.yyy;
  r3.xyz = dc_v_cb2[0].xyz * v0.xxx + r3.xyz;
  r3.xyz = dc_v_cb2[2].xyz * v0.zzz + r3.xyz;
  r3.xyz = dc_v_cb2[3].xyz * v0.www + r3.xyz;
  r3.xyz = -dc_v_cb1[4].xyz + r3.xyz;
  r0.w = dot(r3.xyz, r3.xyz);
  r0.w = rsqrt(r0.w);
  r3.xyz = r3.xyz * r0.www;
  r2.y = dot(r2.xyz, r3.xyz);
  r2.x = dot(r1.xyz, r3.xyz);
  r0.x = dot(r3.xyz, r0.xyz);
  r0.yz = r2.xy / r0.xx;
  r0.yz = v0.zz * r0.yz;
  r0.w = saturate(-r0.x);
  r0.w = 3.33333325 * r0.w;
  r0.w = min(1, r0.w);
  r1.x = r0.w * -2 + 3;
  r0.w = r0.w * r0.w;
  r0.w = r1.x * r0.w;
  r0.yz = -r0.yz * r0.ww;
  r1.xy = r0.yz * dc_v_cb0[12].zz + v0.xy;
  r0.yz = dc_v_cb0[12].zz * r0.yz;
  o3.xy = r0.yz;
  r2.xyzw = dc_v_cb2[1].xyzw * r1.yyyy;
  r2.xyzw = dc_v_cb2[0].xyzw * r1.xxxx + r2.xyzw;
  o4.xy = r1.xy;
  r0.y = 0.0299999993 * v0.z;
  r1.xyzw = dc_v_cb2[2].xyzw * r0.yyyy + r2.xyzw;
  r2.xyzw = dc_v_cb2[3].xyzw + r1.xyzw;
  o2.xyz = dc_v_cb2[3].xyz * v0.www + r1.xyz;
  r1.xyzw = dc_v_cb3[18].xyzw * r2.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r2.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r2.zzzz + r1.xyzw;
  r1.xyzw = dc_v_cb3[20].xyzw * r2.wwww + r1.xyzw;
  r0.y = dc_v_cb0[12].y / dc_v_cb1[7].z;
  o1.z = r1.z + r0.y;
  o1.xyw = r1.xyw;
  r0.y = cmp(0 < r0.x);
  r0.x = cmp(r0.x < 0);
  r0.x = (int)r0.y + (int)-r0.x;
  o3.z = (int)r0.x;
  o4.z = 0.0299999993 * v0.z;
  return;
}














void dc_f(
  float2 v0 : TEXCOORD0,
  float4 v1 : SV_POSITION0,
  float4 v2 : TEXCOORD2,
  float4 v3 : TEXCOORD7,
  float3 v4 : TEXCOORD6,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[15];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(0,0,0,0);
dc_f_cb0[5]=float4(0,0,0,0);
dc_f_cb0[6]=float4(0,0,0,0);
dc_f_cb0[7]=float4(0,0,0,0);
dc_f_cb0[8]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[9]=float4(0,0,0,0);
dc_f_cb0[10]=float4(0,0,0,0);
dc_f_cb0[11]=float4(0,0,0,0);
dc_f_cb0[12]=float4(0,0,0,0);
dc_f_cb0[13]=float4(0,0,_AlphaClip,_ParallaxRectClipExtendX);
dc_f_cb0[14]=float4(_ParallaxRectClipExtendY,0,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = cmp(v3.z < 0);
  if (r0.x != 0) discard;
  r0.x = cmp(0 < v4.z);
  r0.y = cmp(v4.z < 0);
  r0.x = (int)-r0.x + (int)r0.y;
  r0.x = (int)r0.x;
  r0.y = abs(v4.y) * r0.x + dc_f_cb0[14].x;
  r0.x = abs(v4.x) * r0.x + dc_f_cb0[13].w;
  r0.x = -abs(v3.x) * 0.0500000007 + r0.x;
  r0.y = -abs(v3.y) * 0.0500000007 + r0.y;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, v0.xy).xyzw;
  r0.z = r1.w * dc_f_cb0[8].w + -dc_f_cb0[13].z;
  r1.xyzw = dc_f_cb0[8].xyzw * r1.xyzw;
  o0.xyzw = r1.xyzw;
  r0.x = min(r0.z, r0.x);
  r0.x = min(r0.x, r0.y);
  r0.x = cmp(r0.x < 0);
  if (r0.x != 0) discard;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
