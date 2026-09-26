Shader "DynamicCards/Latest/ShaderLibrary_Utility_ShadowReciever" {
Properties {
_ShadowWS("_ShadowWS",2D)="white" {}
_ColorTint("_ColorTint",Color)=(1.0,1.0,1.0,1.0)
_ShadowDisplacement("_ShadowDisplacement",2D)="grey" {}
_ShadowDisplacementAmmount("_ShadowDisplacementAmmount",Float)=0.5
_ShadowAnimationSpeed("_ShadowAnimationSpeed",Float)=1.0
_ShadowAnimationWorldPositionOffset("_ShadowAnimationWorldPositionOffset",Float)=10.0
_IsShadowProxy("_IsShadowProxy",Float)=1.0
}
SubShader {
Tags {"CustomTag"="SHADOWCASTER" "QUEUE"="Background+1"}
Pass {
Cull Back
ZWrite Off
ZTest Off
Blend DstColor Zero
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_ShadowDisplacement;
SamplerState sampler_ShadowTex;
SamplerState sampler_ShadowWS;
Texture2D<float4> _ShadowDisplacement;
Texture2D<float4> _ShadowTex;
Texture2D<float4> _ShadowWS;
float _ShadowAnimationSpeed;
float _ShadowAnimationWorldPositionOffset;
float _ShadowDisplacementAmmount;
float _ShadowOpacity;
float4 _ColorTint;
float4 _ShadowTex_ST;
float4 _ShadowWS_ST;
float4x4 _ShadowProjectorMatrix;














void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : COLOR0,
  out float2 o0 : TEXCOORD0,
  out float2 p0 : TEXCOORD1,
  out float4 o1 : TEXCOORD2,
  out float4 o2 : SV_POSITION0)
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
dc_v_cb2[17]=float4(unity_MatrixVP[0][0],unity_MatrixVP[1][0],unity_MatrixVP[2][0],unity_MatrixVP[3][0]);
dc_v_cb2[18]=float4(unity_MatrixVP[0][1],unity_MatrixVP[1][1],unity_MatrixVP[2][1],unity_MatrixVP[3][1]);
dc_v_cb2[19]=float4(unity_MatrixVP[0][2],unity_MatrixVP[1][2],unity_MatrixVP[2][2],unity_MatrixVP[3][2]);
dc_v_cb2[20]=float4(unity_MatrixVP[0][3],unity_MatrixVP[1][3],unity_MatrixVP[2][3],unity_MatrixVP[3][3]);
float4 dc_v_cb1[4];
dc_v_cb1[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb1[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb1[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb1[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
float4 dc_v_cb0[8];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_ShadowTex_ST.x,_ShadowTex_ST.y,_ShadowTex_ST.z,_ShadowTex_ST.w);
dc_v_cb0[3]=float4(_ShadowWS_ST.x,_ShadowWS_ST.y,_ShadowWS_ST.z,_ShadowWS_ST.w);
dc_v_cb0[4]=float4(_ShadowProjectorMatrix[0][0],_ShadowProjectorMatrix[1][0],_ShadowProjectorMatrix[2][0],_ShadowProjectorMatrix[3][0]);
dc_v_cb0[5]=float4(_ShadowProjectorMatrix[0][1],_ShadowProjectorMatrix[1][1],_ShadowProjectorMatrix[2][1],_ShadowProjectorMatrix[3][1]);
dc_v_cb0[6]=float4(_ShadowProjectorMatrix[0][2],_ShadowProjectorMatrix[1][2],_ShadowProjectorMatrix[2][2],_ShadowProjectorMatrix[3][2]);
dc_v_cb0[7]=float4(_ShadowProjectorMatrix[0][3],_ShadowProjectorMatrix[1][3],_ShadowProjectorMatrix[2][3],_ShadowProjectorMatrix[3][3]);

float4 dc_pack_OSGN_0=float4(0,0,0,0);

  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb1[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb1[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[2].xyzw * v0.zzzz + r0.xyzw;
  r1.xyzw = dc_v_cb1[3].xyzw * v0.wwww + r0.xyzw;
  r0.xyzw = dc_v_cb1[3].xyzw + r0.xyzw;
  r2.xy = dc_v_cb0[5].xz * r1.yy;
  r2.xy = dc_v_cb0[4].xz * r1.xx + r2.xy;
  r2.xy = dc_v_cb0[6].xz * r1.zz + r2.xy;
  r1.yw = dc_v_cb0[7].xz * r1.ww + r2.xy;
  r1.xz = -dc_v_cb1[3].xz + r1.xz;
  r1.xz = -dc_v_cb0[3].zw + r1.xz;
  dc_pack_OSGN_0.xy = dc_v_cb0[3].xy * r1.xz;
  dc_pack_OSGN_0.xy = r1.yw * dc_v_cb0[2].xy + dc_v_cb0[2].zw;
  o1.xyzw = v1.xyzw;
  r1.xyzw = dc_v_cb2[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb2[19].xyzw * r0.zzzz + r1.xyzw;
  o2.xyzw = dc_v_cb2[20].xyzw * r0.wwww + r1.xyzw;
  o0=dc_pack_OSGN_0.xy;
p0=dc_pack_OSGN_0.zw;
return;
}
























void dc_f(
  float2 v0 : TEXCOORD0,
  float2 w0 : TEXCOORD1,
  float4 v1 : TEXCOORD2,
  float4 v2 : SV_POSITION0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[10];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(0,0,0,0);
dc_f_cb0[5]=float4(0,0,0,0);
dc_f_cb0[6]=float4(0,0,0,0);
dc_f_cb0[7]=float4(0,0,0,0);
dc_f_cb0[8]=float4(_ColorTint.x,_ColorTint.y,_ColorTint.z,_ColorTint.w);
dc_f_cb0[9]=float4(_ShadowOpacity,_ShadowDisplacementAmmount,_ShadowAnimationSpeed,_ShadowAnimationWorldPositionOffset);

float4 dc_pack_ISGN_0=float4(v0.x,v0.y,w0.x,w0.y);

  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_pack_ISGN_0.x + dc_pack_ISGN_0.y;
  r0.x = dc_f_cb0[9].w * r0.x;
  r0.x = dc_f_cb1[0].z * dc_f_cb0[9].z + r0.x;
  r0.x = sin(r0.x);
  r0.y = 0.00999999978 * dc_f_cb0[9].y;
  r0.x = r0.y * r0.x;
  r1.xyzw = _ShadowDisplacement.Sample(sampler_ShadowDisplacement, dc_pack_ISGN_0.xy).xyzw;
  r0.yz = r1.xy * float2(2,2) + float2(-1,-1);
  r0.xy = r0.yz * r0.xx + dc_pack_ISGN_0.xy;
  r0.xyzw = _ShadowWS.Sample(sampler_ShadowWS, r0.xy).xyzw;
  r1.xyzw = _ShadowTex.Sample(sampler_ShadowTex, dc_pack_ISGN_0.xy).xyzw;
  r0.y = -r1.x * dc_f_cb0[9].x + 1;
  r0.x = min(r0.y, r0.x);
  r0.y = 1 + -dc_f_cb0[8].w;
  r0.x = r0.x + r0.y;
  r0.x = -1 + r0.x;
  r0.xyz = v1.xxx * r0.xxx + float3(1,1,1);
  r1.xyzw = saturate(dc_f_cb0[8].xyzw + r0.zzzz);
  r0.w = 1;
  o0.xyzw = r1.xyzw * r0.xyzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
