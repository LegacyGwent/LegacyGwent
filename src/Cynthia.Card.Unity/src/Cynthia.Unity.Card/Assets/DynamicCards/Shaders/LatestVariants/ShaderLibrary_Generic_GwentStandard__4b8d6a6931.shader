Shader "DynamicCards/LatestVariant/ShaderLibrary_Generic_GwentStandard__4b8d6a6931" {
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
SamplerState sampler_FlowTex;
SamplerState sampler_MainTex;
Texture2D<float4> _FlowTex;
Texture2D<float4> _MainTex;
float _AlphaPremultiply;
float _Brightness;
float _FlowAnimDist;
float _FlowAnimSpeed;
float _MainTex_SSUV;
float _Offset;
float _SecondTex_SSUV;
float _ThirdTex_SSUV;
float _TimeMultiplier;
float _TimeOffset;
float4 _AplhaRemap;
float4 _FlowTex_ST;
float4 _FresnelRemap;
float4 _MainTex_ST;
float4 _SecondTex_ST;
float4 _ThirdTex_ST;
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
  out float2 o2 : TEXCOORD2,
  out float p2 : TEXCOORD5,
  out float4 o3 : TEXCOORD3,
  out float4 o4 : TEXCOORD6,
  out float3 o5 : TEXCOORD7,
  out float4 o6 : COLOR0,
  out float3 o7 : NORMAL0)
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
float4 dc_v_cb2[7];
dc_v_cb2[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb2[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb2[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb2[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
dc_v_cb2[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_v_cb2[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_v_cb2[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);
float4 dc_v_cb1[8];
dc_v_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
dc_v_cb1[1]=float4(0,0,0,0);
dc_v_cb1[2]=float4(0,0,0,0);
dc_v_cb1[3]=float4(0,0,0,0);
dc_v_cb1[4]=float4(_WorldSpaceCameraPos.x,_WorldSpaceCameraPos.y,_WorldSpaceCameraPos.z,0);
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
dc_v_cb0[6]=float4(0,0,_MainTex_SSUV,0);
dc_v_cb0[7]=float4(0,0,0,0);
dc_v_cb0[8]=float4(_SecondTex_ST.x,_SecondTex_ST.y,_SecondTex_ST.z,_SecondTex_ST.w);
dc_v_cb0[9]=float4(0,0,_SecondTex_SSUV,0);
dc_v_cb0[10]=float4(0,0,0,0);
dc_v_cb0[11]=float4(_ThirdTex_ST.x,_ThirdTex_ST.y,_ThirdTex_ST.z,_ThirdTex_ST.w);
dc_v_cb0[12]=float4(0,0,_ThirdTex_SSUV,0);
dc_v_cb0[13]=float4(0,0,0,0);
dc_v_cb0[14]=float4(0,0,0,0);
dc_v_cb0[15]=float4(0,0,0,0);
dc_v_cb0[16]=float4(0,0,0,0);
dc_v_cb0[17]=float4(0,0,0,0);
dc_v_cb0[18]=float4(_FlowAnimSpeed,0,0,0);
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
float4 dc_pack_OSGN_2=float4(0,0,0,0);

  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_v_cb0[28].x / dc_v_cb1[7].z;
  r1.xyzw = dc_v_cb2[1].xyzw * v0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * v0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb2[2].xyzw * v0.zzzz + r1.xyzw;
  r2.xyzw = dc_v_cb2[3].xyzw + r1.xyzw;
  r0.yzw = dc_v_cb2[3].xyz * v0.www + r1.xyz;
  r0.yzw = dc_v_cb1[4].xyz + -r0.yzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r2.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r2.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r2.zzzz + r1.xyzw;
  r1.xyzw = dc_v_cb3[20].xyzw * r2.wwww + r1.xyzw;
  o0.z = r1.z + r0.x;
  o0.xyw = r1.xyw;
  r1.xyz = dc_v_cb3[18].xyw * r2.yyy;
  r1.xyz = dc_v_cb3[17].xyw * r2.xxx + r1.xyz;
  r1.xyz = dc_v_cb3[19].xyw * r2.zzz + r1.xyz;
  r1.xyz = dc_v_cb3[20].xyw * r2.www + r1.xyz;
  r1.w = dc_v_cb1[5].x * r1.y;
  r1.xy = r1.xw / r1.zz;
  r0.x = dc_v_cb1[6].y / dc_v_cb1[6].x;
  r1.z = r1.y * -r0.x;
  r1.yw = dc_v_cb0[6].zz * r1.xz;
  r0.x = 1 + -dc_v_cb0[6].z;
  r1.yw = v4.xy * r0.xx + r1.yw;
  dc_pack_OSGN_1.xy = r1.yw * dc_v_cb0[5].xy + dc_v_cb0[5].zw;
  r1.yw = dc_v_cb0[9].zz * r1.xz;
  r1.xz = dc_v_cb0[12].zz * r1.xz;
  r0.x = 1 + -dc_v_cb0[9].z;
  r1.yw = v4.xy * r0.xx + r1.yw;
  dc_pack_OSGN_1.zw = r1.yw * dc_v_cb0[8].xy + dc_v_cb0[8].zw;
  r0.x = dot(r0.yzw, r0.yzw);
  r0.x = rsqrt(r0.x);
  r0.xyz = r0.yzw * r0.xxx;
  r2.x = dot(v1.xyz, dc_v_cb2[4].xyz);
  r2.y = dot(v1.xyz, dc_v_cb2[5].xyz);
  r2.z = dot(v1.xyz, dc_v_cb2[6].xyz);
  r0.w = dot(r2.xyz, r2.xyz);
  r0.w = rsqrt(r0.w);
  r2.xyz = r2.xyz * r0.www;
  dc_pack_OSGN_2.z = dot(r2.xyz, r0.xyz);
  r0.x = 1 + -dc_v_cb0[12].z;
  r0.xy = v4.xy * r0.xx + r1.xz;
  dc_pack_OSGN_2.xy = r0.xy * dc_v_cb0[11].xy + dc_v_cb0[11].zw;
  r0.x = dc_v_cb1[0].x * dc_v_cb0[22].z + dc_v_cb4[0].x;
  r0.x = v4.z + r0.x;
  r0.x = v4.w + r0.x;
  r0.y = r0.x * dc_v_cb0[18].x + 0.5;
  r0.x = dc_v_cb0[18].x * r0.x;
  r0.x = frac(r0.x);
  o3.y = frac(r0.y);
  r0.y = 0.5 + -r0.x;
  o3.x = r0.x;
  r0.x = r0.y + r0.y;
  o3.z = abs(r0.x);
  o4.xyz = float3(0,0,0);
  o5.xyz = float3(0,0,0);
  o6.xyzw = v3.xyzw;
  o7.xyz = float3(0,0,0);
  o1=dc_pack_OSGN_1.xy;
p1=dc_pack_OSGN_1.zw;
o2=dc_pack_OSGN_2.xy;
p2=dc_pack_OSGN_2.z;
return;
}


















void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  float2 w1 : TEXCOORD1,
  float2 v2 : TEXCOORD2,
  float w2 : TEXCOORD5,
  float4 v3 : TEXCOORD3,
  float4 v4 : TEXCOORD6,
  float3 v5 : TEXCOORD7,
  float4 v6 : COLOR0,
  float3 v7 : NORMAL0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[27];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,_Brightness,0,0);
dc_f_cb0[3]=float4(_AplhaRemap.x,_AplhaRemap.y,_AplhaRemap.z,_AplhaRemap.w);
dc_f_cb0[4]=float4(0,0,0,0);
dc_f_cb0[5]=float4(0,0,0,0);
dc_f_cb0[6]=float4(0,0,0,0);
dc_f_cb0[7]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[8]=float4(0,0,0,0);
dc_f_cb0[9]=float4(0,0,0,0);
dc_f_cb0[10]=float4(0,0,0,0);
dc_f_cb0[11]=float4(0,0,0,0);
dc_f_cb0[12]=float4(0,0,0,0);
dc_f_cb0[13]=float4(0,0,0,0);
dc_f_cb0[14]=float4(0,0,0,0);
dc_f_cb0[15]=float4(0,0,0,0);
dc_f_cb0[16]=float4(_FlowTex_ST.x,_FlowTex_ST.y,_FlowTex_ST.z,_FlowTex_ST.w);
dc_f_cb0[17]=float4(0,0,0,0);
dc_f_cb0[18]=float4(0,_FlowAnimDist,0,0);
dc_f_cb0[19]=float4(0,0,0,0);
dc_f_cb0[20]=float4(0,0,0,0);
dc_f_cb0[21]=float4(0,0,0,0);
dc_f_cb0[22]=float4(0,0,0,0);
dc_f_cb0[23]=float4(_FresnelRemap.x,_FresnelRemap.y,_FresnelRemap.z,_FresnelRemap.w);
dc_f_cb0[24]=float4(0,0,0,0);
dc_f_cb0[25]=float4(0,0,0,0);
dc_f_cb0[26]=float4(0,0,0,_AlphaPremultiply);

float4 dc_pack_ISGN_1=float4(v1.x,v1.y,w1.x,w1.y);
float4 dc_pack_ISGN_2=float4(v2.x,v2.y,w2,0);

  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = dc_pack_ISGN_1.xy * dc_f_cb0[16].xy + dc_f_cb0[16].zw;
  r0.xyzw = _FlowTex.Sample(sampler_FlowTex, r0.xy).xyzw;
  r0.xyzw = float4(-0.00380000006,-0.00380000006,-0.00380000006,-0.00380000006) + r0.xyxy;
  r0.xyzw = r0.xyzw * float4(2,2,2,2) + float4(-1,-1,-1,-1);
  r0.xyzw = dc_f_cb0[18].yyyy * r0.xyzw;
  r0.xyzw = r0.xyzw * v3.xxyy + dc_pack_ISGN_1.xyxy;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r0.zw).xyzw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r1.xyzw = r1.xyzw + -r0.xyzw;
  r0.xyzw = v3.zzzz * r1.xyzw + r0.xyzw;
  r0.xyzw = dc_f_cb0[7].xyzw * r0.xyzw;
  r0.xyzw = v6.xyzw * r0.xyzw;
  r1.x = 1 + -dc_f_cb0[3].z;
  r1.xy = dc_f_cb0[3].xy + r1.xx;
  r1.y = r1.y + -r1.x;
  r1.x = r1.x / r1.y;
  r1.y = 1 / r1.y;
  r0.w = saturate(r0.w * r1.y + -r1.x);
  r0.xyz = dc_f_cb0[2].yyy * r0.xyz;
  r1.x = dc_f_cb0[23].y + -dc_f_cb0[23].x;
  r1.y = 1 / r1.x;
  r1.x = dc_f_cb0[23].x / r1.x;
  r1.x = saturate(abs(dc_pack_ISGN_2.z) * r1.y + -r1.x);
  o0.w = r1.x * r0.w;
  r0.w = saturate(dc_f_cb0[26].w + v6.w);
  o0.xyz = r0.xyz * r0.www;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
