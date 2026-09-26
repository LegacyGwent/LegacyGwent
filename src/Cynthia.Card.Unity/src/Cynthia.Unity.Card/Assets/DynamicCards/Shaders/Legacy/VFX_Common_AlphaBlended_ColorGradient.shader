Shader "DynamicCards/Legacy/VFX_Common_AlphaBlended_ColorGradient" {
Properties {
_node_332("_node_332",2D)="white" {}
_ReplaceBlackwith("_ReplaceBlackwith",Color)=(0.7160000205039978,0.28904658555984497,0.0,1.0)
_ReplaceWhitewith("_ReplaceWhitewith",Color)=(0.5839999914169312,0.36000001430511475,0.0,1.0)
_MixExponent("_MixExponent",Float)=1.0
_UTile("_UTile",Float)=3.0
_VTile("_VTile",Float)=8.0
_OpacityExponent("_OpacityExponent",Float)=1.0
_OpacityMultiplier("_OpacityMultiplier",Float)=1.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
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
SamplerState sampler_node_332;
Texture2D<float4> _node_332;
float _MixExponent;
float _OpacityExponent;
float _OpacityMultiplier;
float _UTile;
float _VTile;
float4 _ReplaceBlackwith;
float4 _ReplaceWhitewith;
float4 _node_332_ST;












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
  o2.xyzw = v2.xyzw;
  return;
}














void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_node_332_ST.x,_node_332_ST.y,_node_332_ST.z,_node_332_ST.w);
dc_f_cb0[3]=float4(_ReplaceBlackwith.x,_ReplaceBlackwith.y,_ReplaceBlackwith.z,_ReplaceBlackwith.w);
dc_f_cb0[4]=float4(_ReplaceWhitewith.x,_ReplaceWhitewith.y,_ReplaceWhitewith.z,_ReplaceWhitewith.w);
dc_f_cb0[5]=float4(_MixExponent,_UTile,_VTile,_OpacityExponent);
dc_f_cb0[6]=float4(_OpacityMultiplier,0,0,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb0[5].y * dc_f_cb0[5].z;
  r0.y = r0.x * 16 + -1;
  r0.y = v2.x * r0.y;
  r0.y = r0.y / r0.x;
  r0.z = cmp(r0.y >= -r0.y);
  r0.w = frac(abs(r0.y));
  r0.z = r0.z ? r0.w : -r0.w;
  r0.x = r0.z * r0.x;
  r0.xy = floor(r0.xy);
  r0.zw = float2(1,1) / dc_f_cb0[5].yz;
  r1.x = r0.x * r0.z;
  r1.x = floor(r1.x);
  r2.x = -dc_f_cb0[5].y * r1.x + r0.x;
  r1.y = -v1.y + r1.x;
  r1.x = v1.x;
  r2.y = 1;
  r1.xy = r2.xy + r1.xy;
  r0.x = r1.x * r0.z;
  r0.z = -r1.y * r0.w + 1;
  r0.xz = r0.xz * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r1.xyzw = _node_332.Sample(sampler_node_332, r0.xz).xyzw;
  r0.xz = r1.yw + -r1.xz;
  r0.w = 0.5 * r0.y;
  r0.y = saturate(-1 + r0.y);
  r1.y = cmp(r0.w >= -r0.w);
  r0.w = frac(abs(r0.w));
  r0.w = r1.y ? r0.w : -r0.w;
  r0.w = r0.w + r0.w;
  r0.xz = r0.ww * r0.xz + r1.xz;
  r0.z = r0.z + -r0.x;
  r0.x = r0.y * r0.z + r0.x;
  r0.x = log2(r0.x);
  r0.xy = dc_f_cb0[5].xw * r0.xx;
  r0.xy = exp2(r0.xy);
  r1.xyz = dc_f_cb0[4].xyz + -dc_f_cb0[3].xyz;
  o0.xyz = r0.xxx * r1.xyz + dc_f_cb0[3].xyz;
  r0.x = dc_f_cb0[6].x * r0.y;
  o0.w = saturate(v2.w * r0.x);
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
