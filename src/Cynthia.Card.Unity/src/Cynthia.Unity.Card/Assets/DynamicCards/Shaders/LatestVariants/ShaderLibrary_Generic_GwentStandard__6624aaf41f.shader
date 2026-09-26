Shader "DynamicCards/LatestVariant/ShaderLibrary_Generic_GwentStandard__6624aaf41f" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_SecondTex("_SecondTex",2D)="black" {}
_ThirdTex("_ThirdTex",2D)="white" {}
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_SecondTintColor("_SecondTintColor",Color)=(1.0,1.0,1.0,1.0)
_ThirdTintColor("_ThirdTintColor",Color)=(1.0,1.0,1.0,1.0)
_Cutoff("_Cutoff",Float)=0.5
_Brightness("_Brightness",Float)=1.0
_AplhaRemap("_AplhaRemap",Vector)=(0.0,1.0,1.0,1.0)
_Multiplier("_Multiplier",Float)=1.0
_MainTexUVAnim("_MainTexUVAnim",Vector)=(0.0,0.0,0.0,0.0)
_SecondTexUVAnim("_SecondTexUVAnim",Vector)=(0.0,0.0,0.0,0.0)
_ThirdTexUVAnim("_ThirdTexUVAnim",Vector)=(0.0,0.0,0.0,0.0)
_MaskUVAnim("_MaskUVAnim",Vector)=(0.0,0.0,0.0,0.0)
_MainTex_SSUV("_MainTex_SSUV",Float)=0.0
_SecondTex_SSUV("_SecondTex_SSUV",Float)=0.0
_ThirdTex_SSUV("_ThirdTex_SSUV",Float)=0.0
_Mask_SSUV("_Mask_SSUV",Float)=0.0
_MaskRemap("_MaskRemap",Vector)=(0.0,1.0,0.0,0.0)
_FlowTex("_FlowTex",2D)="white" {}
_FlowAnimSpeed("_FlowAnimSpeed",Float)=0.0
_FlowAnimDist("_FlowAnimDist",Float)=0.0
_FramesNum("_FramesNum",Float)=0.0
_FramesX("_FramesX",Float)=0.0
_FramesY("_FramesY",Float)=0.0
_TimeMultiplier("_TimeMultiplier",Float)=1.0
_AlphaPremultiply("_AlphaPremultiply",Float)=1.0
_FresnelRemap("_FresnelRemap",Vector)=(0.0,1.0,1.0,1.0)
_Mask("_Mask",2D)="white" {}
_Mode("_Mode",Float)=0.0
_SrcBlend("_SrcBlend",Float)=1.0
_DstBlend("_DstBlend",Float)=0.0
_ZWrite("_ZWrite",Float)=1.0
_Cull("_Cull",Float)=2.0
_DoubleSided("_DoubleSided",Float)=2.0
_AlphaToMask("_AlphaToMask",Float)=0.0
_ZTest("_ZTest",Float)=4.0
_Offset("_Offset",Float)=0.0
_ColorMask("_ColorMask",Float)=15.0
_SrcAlphaBlend("_SrcAlphaBlend",Float)=1.0
_DstAlphaBlend("_DstAlphaBlend",Float)=10.0
_BlendOp("_BlendOp",Float)=0.0
_Overrides("_Overrides",Float)=0.0
_Anim("_Anim",Float)=0.0
_Special("_Special",Float)=0.0
_Modifier("_Modifier",Float)=0.0
}
SubShader {
Tags {"RenderType"="Opaque"}
Pass {
Cull [_Cull]
ZWrite [_ZWrite]
ZTest [_ZTest]
Blend [_SrcBlend] [_DstBlend]
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_MainTex;
SamplerState sampler_SecondTex;
SamplerState sampler_ThirdTex;
Texture2D<float4> _MainTex;
Texture2D<float4> _SecondTex;
Texture2D<float4> _ThirdTex;
float _AlphaPremultiply;
float _Brightness;
float _MainTex_SSUV;
float _Multiplier;
float _Offset;
float _SecondTex_SSUV;
float _ThirdTex_SSUV;
float _TimeMultiplier;
float _TimeOffset;
float2 _MainTexUVAnim;
float2 _SecondTexUVAnim;
float2 _ThirdTexUVAnim;
float4 _AplhaRemap;
float4 _MainTex_ST;
float4 _SecondTex_ST;
float4 _SecondTintColor;
float4 _ThirdTex_ST;
float4 _ThirdTintColor;
float4 _TintColor;


















void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float4 v2 : TANGENT0,
  float4 v3 : COLOR0,
  float4 v4 : TEXCOORD0,
  float2 v5 : TEXCOORD1,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
  out float2 p1 : TEXCOORD1,
  out float4 o2 : TEXCOORD2,
  out float4 o3 : TEXCOORD6,
  out float3 o4 : TEXCOORD7,
  out float4 o5 : COLOR0,
  out float3 o6 : NORMAL0)
{
float4 dc_v_cb4[1];
dc_v_cb4[0]=float4(_TimeOffset,0,0,0);
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
float4 dc_v_cb1[8];
dc_v_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
dc_v_cb1[1]=float4(0,0,0,0);
dc_v_cb1[2]=float4(0,0,0,0);
dc_v_cb1[3]=float4(0,0,0,0);
dc_v_cb1[4]=float4(0,0,0,0);
dc_v_cb1[5]=float4(_ProjectionParams.x,_ProjectionParams.y,_ProjectionParams.z,_ProjectionParams.w);
dc_v_cb1[6]=float4(_ScreenParams.x,_ScreenParams.y,_ScreenParams.z,_ScreenParams.w);
dc_v_cb1[7]=float4(_ZBufferParams.x,_ZBufferParams.y,_ZBufferParams.z,_ZBufferParams.w);
float4 dc_v_cb0[29];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_v_cb0[6]=float4(_MainTexUVAnim.x,_MainTexUVAnim.y,_MainTex_SSUV,0);
dc_v_cb0[7]=float4(0,0,0,0);
dc_v_cb0[8]=float4(_SecondTex_ST.x,_SecondTex_ST.y,_SecondTex_ST.z,_SecondTex_ST.w);
dc_v_cb0[9]=float4(_SecondTexUVAnim.x,_SecondTexUVAnim.y,_SecondTex_SSUV,0);
dc_v_cb0[10]=float4(0,0,0,0);
dc_v_cb0[11]=float4(_ThirdTex_ST.x,_ThirdTex_ST.y,_ThirdTex_ST.z,_ThirdTex_ST.w);
dc_v_cb0[12]=float4(_ThirdTexUVAnim.x,_ThirdTexUVAnim.y,_ThirdTex_SSUV,0);
dc_v_cb0[13]=float4(0,0,0,0);
dc_v_cb0[14]=float4(0,0,0,0);
dc_v_cb0[15]=float4(0,0,0,0);
dc_v_cb0[16]=float4(0,0,0,0);
dc_v_cb0[17]=float4(0,0,0,0);
dc_v_cb0[18]=float4(0,0,0,0);
dc_v_cb0[19]=float4(0,0,0,0);
dc_v_cb0[20]=float4(0,0,0,0);
dc_v_cb0[21]=float4(0,0,0,0);
dc_v_cb0[22]=float4(0,0,_TimeMultiplier,0);
dc_v_cb0[23]=float4(0,0,0,0);
dc_v_cb0[24]=float4(0,0,0,0);
dc_v_cb0[25]=float4(0,0,0,0);
dc_v_cb0[26]=float4(0,0,0,0);
dc_v_cb0[27]=float4(0,0,0,0);
dc_v_cb0[28]=float4(_Offset,0,0,0);

float4 dc_pack_OSGN_1=float4(0,0,0,0);

  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_v_cb0[28].x / dc_v_cb1[7].z;
  r1.xyzw = dc_v_cb2[1].xyzw * v0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * v0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb2[2].xyzw * v0.zzzz + r1.xyzw;
  r1.xyzw = dc_v_cb2[3].xyzw + r1.xyzw;
  r2.xyzw = dc_v_cb3[18].xyzw * r1.yyyy;
  r2.xyzw = dc_v_cb3[17].xyzw * r1.xxxx + r2.xyzw;
  r2.xyzw = dc_v_cb3[19].xyzw * r1.zzzz + r2.xyzw;
  r2.xyzw = dc_v_cb3[20].xyzw * r1.wwww + r2.xyzw;
  o0.z = r2.z + r0.x;
  o0.xyw = r2.xyw;
  r0.xyz = dc_v_cb3[18].xyw * r1.yyy;
  r0.xyz = dc_v_cb3[17].xyw * r1.xxx + r0.xyz;
  r0.xyz = dc_v_cb3[19].xyw * r1.zzz + r0.xyz;
  r0.xyz = dc_v_cb3[20].xyw * r1.www + r0.xyz;
  r0.w = dc_v_cb1[5].x * r0.y;
  r0.xy = r0.xw / r0.zz;
  r0.w = dc_v_cb1[6].y / dc_v_cb1[6].x;
  r0.z = r0.y * -r0.w;
  r0.yw = dc_v_cb0[6].zz * r0.xz;
  r1.x = 1 + -dc_v_cb0[6].z;
  r0.yw = v4.xy * r1.xx + r0.yw;
  r0.yw = r0.yw * dc_v_cb0[5].xy + dc_v_cb0[5].zw;
  r1.x = dc_v_cb1[0].x * dc_v_cb0[22].z + dc_v_cb4[0].x;
  r1.x = v4.z + r1.x;
  r1.x = v4.w + r1.x;
  dc_pack_OSGN_1.xy = dc_v_cb0[6].xy * r1.xx + r0.yw;
  r0.yw = dc_v_cb0[9].zz * r0.xz;
  r0.xz = dc_v_cb0[12].zz * r0.xz;
  r1.y = 1 + -dc_v_cb0[9].z;
  r0.yw = v4.xy * r1.yy + r0.yw;
  r0.yw = r0.yw * dc_v_cb0[8].xy + dc_v_cb0[8].zw;
  dc_pack_OSGN_1.zw = dc_v_cb0[9].xy * r1.xx + r0.yw;
  r0.y = 1 + -dc_v_cb0[12].z;
  r0.xy = v4.xy * r0.yy + r0.xz;
  r0.xy = r0.xy * dc_v_cb0[11].xy + dc_v_cb0[11].zw;
  o2.xy = dc_v_cb0[12].xy * r1.xx + r0.xy;
  o3.xyz = float3(0,0,0);
  o4.xyz = float3(0,0,0);
  o5.xyzw = v3.xyzw;
  o6.xyz = float3(0,0,0);
  o1=dc_pack_OSGN_1.xy;
p1=dc_pack_OSGN_1.zw;
return;
}






















void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  float2 w1 : TEXCOORD1,
  float4 v2 : TEXCOORD2,
  float4 v3 : TEXCOORD6,
  float3 v4 : TEXCOORD7,
  float4 v5 : COLOR0,
  float3 v6 : NORMAL0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[27];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,_Brightness,0,0);
dc_f_cb0[3]=float4(_AplhaRemap.x,_AplhaRemap.y,_AplhaRemap.z,_AplhaRemap.w);
dc_f_cb0[4]=float4(_Multiplier,0,0,0);
dc_f_cb0[5]=float4(0,0,0,0);
dc_f_cb0[6]=float4(0,0,0,0);
dc_f_cb0[7]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[8]=float4(0,0,0,0);
dc_f_cb0[9]=float4(0,0,0,0);
dc_f_cb0[10]=float4(_SecondTintColor.x,_SecondTintColor.y,_SecondTintColor.z,_SecondTintColor.w);
dc_f_cb0[11]=float4(0,0,0,0);
dc_f_cb0[12]=float4(0,0,0,0);
dc_f_cb0[13]=float4(_ThirdTintColor.x,_ThirdTintColor.y,_ThirdTintColor.z,_ThirdTintColor.w);
dc_f_cb0[14]=float4(0,0,0,0);
dc_f_cb0[15]=float4(0,0,0,0);
dc_f_cb0[16]=float4(0,0,0,0);
dc_f_cb0[17]=float4(0,0,0,0);
dc_f_cb0[18]=float4(0,0,0,0);
dc_f_cb0[19]=float4(0,0,0,0);
dc_f_cb0[20]=float4(0,0,0,0);
dc_f_cb0[21]=float4(0,0,0,0);
dc_f_cb0[22]=float4(0,0,0,0);
dc_f_cb0[23]=float4(0,0,0,0);
dc_f_cb0[24]=float4(0,0,0,0);
dc_f_cb0[25]=float4(0,0,0,0);
dc_f_cb0[26]=float4(0,0,0,_AlphaPremultiply);

float4 dc_pack_ISGN_1=float4(v1.x,v1.y,w1.x,w1.y);

  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = _SecondTex.Sample(sampler_SecondTex, dc_pack_ISGN_1.zw).xyzw;
  r0.xy = r0.xy * float2(2,2) + float2(-1,-1);
  r0.xy = dc_f_cb0[10].xy * r0.xy;
  r1.xyzw = _ThirdTex.Sample(sampler_ThirdTex, v2.xy).xyzw;
  r0.zw = dc_f_cb0[13].xy * r1.xy;
  r0.xy = r0.xy * r0.zw;
  r0.xy = r0.xy * dc_f_cb0[4].xx + dc_pack_ISGN_1.xy;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.xyzw = dc_f_cb0[7].xyzw * r0.xyzw;
  r0.xyzw = v5.xyzw * r0.xyzw;
  r1.x = 1 + -dc_f_cb0[3].z;
  r1.xy = dc_f_cb0[3].xy + r1.xx;
  r1.y = r1.y + -r1.x;
  r1.x = r1.x / r1.y;
  r1.y = 1 / r1.y;
  o0.w = saturate(r0.w * r1.y + -r1.x);
  r0.xyz = dc_f_cb0[2].yyy * r0.xyz;
  r0.w = saturate(dc_f_cb0[26].w + v5.w);
  o0.xyz = r0.xyz * r0.www;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
