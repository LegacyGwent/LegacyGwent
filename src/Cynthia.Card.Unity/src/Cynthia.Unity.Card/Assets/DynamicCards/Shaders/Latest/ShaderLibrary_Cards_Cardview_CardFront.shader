Shader "DynamicCards/Latest/ShaderLibrary_Cards_Cardview_CardFront" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_PremTex("_PremTex",2D)="black" {}
_FrameTex("_FrameTex",2D)="black" {}
_Saturation("_Saturation",Float)=1.0
_FrameSaturation("_FrameSaturation",Float)=1.0
_LightingInfluence("_LightingInfluence",Float)=1.0
_IsPremium("_IsPremium",Float)=0.0
_FlowMask("_FlowMask",2D)="black" {}
_FlowParameters("_FlowParameters",Vector)=(0.5,0.5,0.4000000059604645,0.10000000149011612)
_PremiumShineSpeed("_PremiumShineSpeed",Float)=-0.05000000074505806
_PremiumShineMultiplier("_PremiumShineMultiplier",Float)=20.0
_SmallSparksColor("_SmallSparksColor",Color)=(0.44999998807907104,0.10000000149011612,0.0,1.0)
_BigSparksColor("_BigSparksColor",Color)=(1.0,0.6499999761581421,0.0,1.0)
_GhostingInfluence("_GhostingInfluence",Float)=0.0
_DarkenInfluence("_DarkenInfluence",Float)=0.0
_DarkenColor("_DarkenColor",Color)=(0.0,1.0,0.699999988079071,1.0)
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_GhostingTintColor("_GhostingTintColor",Color)=(0.0,0.699999988079071,1.0,1.0)
_IsShadowProxy("_IsShadowProxy",Float)=0.0
}
SubShader {
Tags {"CustomTag"="SHADOWCASTER" "QUEUE"="Geometry"}
Pass {
Cull Back
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
SamplerState sampler_FrameTex;
SamplerState sampler_MainTex;
SamplerState sampler_PremTex;
Texture2D<float4> _FrameTex;
Texture2D<float4> _MainTex;
Texture2D<float4> _PremTex;
float _DarkenInfluence;
float _FrameSaturation;
float _GhostingInfluence;
float _IsPremium;
float _LightingInfluence;
float _Saturation;
float4 _DarkenColor;
float4 _GhostingTintColor;
float4 _MainTex_ST;
float4 _PremTex_ST;
float4 _TintColor;
















void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float2 o0 : TEXCOORD0,
  out float2 p0 : TEXCOORD1,
  out float4 o1 : TEXCOORD2,
  out float4 o2 : TEXCOORD3,
  out float2 o3 : TEXCOORD4,
  out float4 o4 : SV_POSITION0,
  out float4 o5 : COLOR0)
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
float4 dc_v_cb1[6];
dc_v_cb1[0]=float4(0,0,0,0);
dc_v_cb1[1]=float4(0,0,0,0);
dc_v_cb1[2]=float4(0,0,0,0);
dc_v_cb1[3]=float4(0,0,0,0);
dc_v_cb1[4]=float4(0,0,0,0);
dc_v_cb1[5]=float4(_ProjectionParams.x,_ProjectionParams.y,_ProjectionParams.z,_ProjectionParams.w);
float4 dc_v_cb0[6];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_v_cb0[5]=float4(_PremTex_ST.x,_PremTex_ST.y,_PremTex_ST.z,_PremTex_ST.w);

float4 dc_pack_OSGN_0=float4(0,0,0,0);

  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  dc_pack_OSGN_0.xy = v1.xy * dc_v_cb0[4].xy + dc_v_cb0[4].zw;
  dc_pack_OSGN_0.xy = v1.xy;
  o1.xyz = float3(0,0,0);
  r0.xyzw = dc_v_cb2[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb2[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * v0.zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
  r1.x = dc_v_cb1[5].x * r0.y;
  r1.w = 0.5 * r1.x;
  r1.xz = float2(0.5,0.5) * r0.xw;
  o2.xy = r1.xw + r1.zz;
  o2.zw = r0.zw;
  o4.xyzw = r0.xyzw;
  o3.xy = v1.xy * dc_v_cb0[5].xy + dc_v_cb0[5].zw;
  o5.xyzw = v2.xyzw;
  o0=dc_pack_OSGN_0.xy;
p0=dc_pack_OSGN_0.zw;
return;
}
























void dc_f(
  float2 v0 : TEXCOORD0,
  float2 w0 : TEXCOORD1,
  float4 v1 : TEXCOORD2,
  float4 v2 : TEXCOORD3,
  float2 v3 : TEXCOORD4,
  float4 v4 : SV_POSITION0,
  float4 v5 : COLOR0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[7];
dc_f_cb1[0]=float4(0,0,0,0);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(0,0,0,0);
dc_f_cb1[5]=float4(0,0,0,0);
dc_f_cb1[6]=float4(_ScreenParams.x,_ScreenParams.y,_ScreenParams.z,_ScreenParams.w);
float4 dc_f_cb0[17];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_LightColor0.x,_LightColor0.y,_LightColor0.z,_LightColor0.w);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(0,0,0,0);
dc_f_cb0[5]=float4(0,0,0,0);
dc_f_cb0[6]=float4(0,0,0,0);
dc_f_cb0[7]=float4(0,0,0,0);
dc_f_cb0[8]=float4(0,0,0,0);
dc_f_cb0[9]=float4(0,0,0,0);
dc_f_cb0[10]=float4(0,0,0,0);
dc_f_cb0[11]=float4(0,0,_Saturation,_FrameSaturation);
dc_f_cb0[12]=float4(_LightingInfluence,_IsPremium,_DarkenInfluence,0);
dc_f_cb0[13]=float4(_DarkenColor.x,_DarkenColor.y,_DarkenColor.z,_DarkenColor.w);
dc_f_cb0[14]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[15]=float4(_GhostingTintColor.x,_GhostingTintColor.y,_GhostingTintColor.z,_GhostingTintColor.w);
dc_f_cb0[16]=float4(_GhostingInfluence,0,0,0);

float4 dc_pack_ISGN_0=float4(v0.x,v0.y,w0.x,w0.y);

  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  float4 x0[16];
  x0[0].x = 0.0588235296;
  x0[1].x = 0.529411793;
  x0[2].x = 0.176470593;
  x0[3].x = 0.647058845;
  x0[4].x = 0.764705896;
  x0[5].x = 0.294117659;
  x0[6].x = 0.882352948;
  x0[7].x = 0.411764711;
  x0[8].x = 0.235294119;
  x0[9].x = 0.70588237;
  x0[10].x = 0.117647059;
  x0[11].x = 0.588235319;
  x0[12].x = 0.941176474;
  x0[13].x = 0.470588237;
  x0[14].x = 0.823529422;
  x0[15].x = 0.352941185;
  r0.xy = v2.xy / v2.ww;
  r0.xy = dc_f_cb1[6].xy * r0.xy;
  r0.xy = (uint2)r0.xy;
  r0.x = (uint)r0.x << 2;
  r0.xy = (int2)r0.xy & int2(12,3);
  r0.x = (int)r0.y + (int)r0.x;
  r0.x = x0[r0.x+0].x;
  r0.y = -dc_f_cb0[16].x * 0.5 + 1;
  r0.x = r0.y + -r0.x;
  r0.x = cmp(r0.x < 0);
  if (r0.x != 0) discard;
  r0.xyzw = _PremTex.Sample(sampler_PremTex, v3.xy).xyzw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, dc_pack_ISGN_0.xy).xyzw;
  r0.xyz = -r1.xyz + r0.xyz;
  r0.xyz = dc_f_cb0[12].yyy * r0.xyz + r1.xyz;
  r0.w = dot(dc_f_cb0[2].xyz, dc_f_cb0[2].xyz);
  r0.w = sqrt(r0.w);
  r0.w = r0.w * 0.699999988 + -1;
  r0.w = dc_f_cb0[12].x * r0.w + 1;
  r0.xyz = r0.xyz * r0.www;
  r0.w = dot(r0.xyz, float3(0.219999999,0.707000017,0.0710000023));
  r1.xyz = r0.www * dc_f_cb0[13].xyz + -r0.xyz;
  r0.xyz = dc_f_cb0[12].zzz * r1.xyz + r0.xyz;
  r0.xyz = r0.xyz + -r0.www;
  r1.x = saturate(dc_f_cb0[11].z);
  r0.xyz = r1.xxx * r0.xyz + r0.www;
  r1.xyz = dc_f_cb0[16].xxx * dc_f_cb0[15].xyz;
  r0.xyz = r1.xyz * r0.www + r0.xyz;
  r1.xyzw = _FrameTex.Sample(sampler_FrameTex, dc_pack_ISGN_0.xy).xyzw;
  r0.w = dot(r1.xyz, float3(0.219999999,0.707000017,0.0710000023));
  r1.xyz = r1.xyz + -r0.www;
  r1.xyz = dc_f_cb0[11].www * r1.xyz + r0.www;
  r1.xyz = r1.xyz + -r0.xyz;
  r0.xyz = r1.www * r1.xyz + r0.xyz;
  r0.w = 1;
  r0.xyzw = v5.xyzw * r0.xyzw;
  o0.xyzw = dc_f_cb0[14].xyzw * r0.xyzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
