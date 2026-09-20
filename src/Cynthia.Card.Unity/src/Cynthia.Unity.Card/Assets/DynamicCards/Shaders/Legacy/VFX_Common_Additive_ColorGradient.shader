Shader "DynamicCards/Legacy/VFX_Common_Additive_ColorGradient" {
Properties {
_node_332("_node_332",2D)="white" {}
_ReplaceBlackwith("_ReplaceBlackwith",Color)=(0.7160000205039978,0.28904658555984497,0.0,1.0)
_ReplaceWhitewith("_ReplaceWhitewith",Color)=(0.5839999914169312,0.36000001430511475,0.0,1.0)
_SaturationDivision("_SaturationDivision",Float)=1.0
_ValueExponent("_ValueExponent",Float)=1.0
_ValueMultiplier("_ValueMultiplier",Float)=1.0
_MixExponent("_MixExponent",Float)=1.0
_Invertforsaturation("_Invertforsaturation",Float)=0.0
_UTile("_UTile",Float)=3.0
_VTile("_VTile",Float)=8.0
_Flattenamount("_Flattenamount",Float)=1.0
_Useflattening("_Useflattening",Float)=0.0
_FlipU("_FlipU",Float)=0.0
_MipLevel("_MipLevel",Float)=0.0
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Back
ZWrite Off
ZTest LEqual
Blend One One
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
SamplerState sampler_node_332;
Texture2D<float4> _node_332;
float _Flattenamount;
float _FlipU;
float _Invertforsaturation;
float _MipLevel;
float _MixExponent;
float _SaturationDivision;
float _UTile;
float _Useflattening;
float _VTile;
float _ValueExponent;
float _ValueMultiplier;
float4 _ReplaceBlackwith;
float4 _ReplaceWhitewith;
float4 _node_332_ST;














void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float4 o3 : COLOR0)
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
float4 dc_v_cb1[7];
dc_v_cb1[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb1[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb1[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb1[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
dc_v_cb1[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_v_cb1[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_v_cb1[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);
float4 dc_v_cb0[8];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(0,0,0,0);
dc_v_cb0[6]=float4(0,0,0,_Flattenamount);
dc_v_cb0[7]=float4(_Useflattening,0,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_v_cb1[1].z * v0.y;
  r0.x = dc_v_cb1[0].z * v0.x + r0.x;
  r0.x = dc_v_cb1[2].z * v0.z + r0.x;
  r0.x = dc_v_cb1[3].z * v0.w + r0.x;
  r0.y = cmp(r0.x >= 0);
  r0.z = cmp(0 >= r0.x);
  r0.yz = r0.yz ? float2(1,1) : 0;
  r0.xy = r0.zz * r0.xy;
  r0.x = r0.y * -r0.x + r0.x;
  r0.xyz = dc_v_cb1[6].xyz * r0.xxx;
  r0.xyz = dc_v_cb0[6].www * r0.xyz;
  r0.xyz = dc_v_cb0[7].xxx * -r0.xyz + v0.xyz;
  r1.xyzw = dc_v_cb1[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb1[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb1[2].xyzw * r0.zzzz + r1.xyzw;
  r1.xyzw = dc_v_cb1[3].xyzw + r0.xyzw;
  o2.xyzw = dc_v_cb1[3].xyzw * v0.wwww + r0.xyzw;
  r0.xyzw = dc_v_cb2[18].xyzw * r1.yyyy;
  r0.xyzw = dc_v_cb2[17].xyzw * r1.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb2[19].xyzw * r1.zzzz + r0.xyzw;
  o0.xyzw = dc_v_cb2[20].xyzw * r1.wwww + r0.xyzw;
  o1.xy = v1.xy;
  o3.xyzw = v2.xyzw;
  return;
}














void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float4 v3 : COLOR0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[8];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_node_332_ST.x,_node_332_ST.y,_node_332_ST.z,_node_332_ST.w);
dc_f_cb0[3]=float4(_ReplaceBlackwith.x,_ReplaceBlackwith.y,_ReplaceBlackwith.z,_ReplaceBlackwith.w);
dc_f_cb0[4]=float4(_ReplaceWhitewith.x,_ReplaceWhitewith.y,_ReplaceWhitewith.z,_ReplaceWhitewith.w);
dc_f_cb0[5]=float4(_SaturationDivision,_ValueExponent,_ValueMultiplier,_MixExponent);
dc_f_cb0[6]=float4(_Invertforsaturation,_UTile,_VTile,0);
dc_f_cb0[7]=float4(0,_FlipU,_MipLevel,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb0[6].y * dc_f_cb0[6].z;
  r0.y = r0.x * 4 + -1;
  r0.y = v3.x * r0.y;
  r0.y = r0.y / r0.x;
  r0.z = cmp(r0.y >= -r0.y);
  r0.w = frac(abs(r0.y));
  r0.z = r0.z ? r0.w : -r0.w;
  r0.x = r0.z * r0.x;
  r0.xy = floor(r0.xy);
  r0.zw = float2(1,1) / dc_f_cb0[6].yz;
  r1.x = r0.x * r0.z;
  r1.y = floor(r1.x);
  r1.x = -dc_f_cb0[6].y * r1.y + r0.x;
  r2.xz = float2(1,1) + -v1.xy;
  r0.x = -v1.x + r2.x;
  r2.y = dc_f_cb0[7].y * r0.x + v1.x;
  r1.xy = r2.yz + r1.xy;
  r0.x = r1.x * r0.z;
  r0.z = -r1.y * r0.w + 1;
  r0.xz = r0.xz * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r1.xyzw = _node_332.SampleLevel(sampler_node_332, r0.xz, dc_f_cb0[7].z).xyzw;
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
  r0.xy = dc_f_cb0[5].wy * r0.xx;
  r0.xy = exp2(r0.xy);
  r0.z = 1 + -r0.y;
  r0.z = r0.z + -r0.y;
  r0.z = dc_f_cb0[6].x * r0.z + r0.y;
  r0.z = r0.z / dc_f_cb0[5].x;
  r1.xyz = dc_f_cb0[4].xyz + -dc_f_cb0[3].xyz;
  r1.xyw = r0.xxx * r1.yzx + dc_f_cb0[3].yzx;
  r0.x = dc_f_cb0[5].z * r0.y;
  r0.y = cmp(r1.x >= r1.y);
  r0.y = r0.y ? 1.000000 : 0;
  r2.xy = r1.yx;
  r3.xy = -r2.xy + r1.xy;
  r2.zw = float2(-1,0.666666687);
  r3.zw = float2(1,-1);
  r2.xyzw = r0.yyyy * r3.xyzw + r2.xyzw;
  r0.y = cmp(r1.w >= r2.x);
  r0.y = r0.y ? 1.000000 : 0;
  r1.xyz = r2.xyw;
  r2.xyw = r1.wyx;
  r2.xyzw = r2.xyzw + -r1.xyzw;
  r1.xyzw = r0.yyyy * r2.xyzw + r1.xyzw;
  r0.y = min(r1.w, r1.y);
  r0.y = r1.x + -r0.y;
  r0.y = r0.y * 6 + 1.00000001e-10;
  r0.w = r1.w + -r1.y;
  r0.y = r0.w / r0.y;
  r0.y = r1.z + r0.y;
  r1.xyz = float3(0,-0.333333343,0.333333343) + abs(r0.yyy);
  r1.xyz = frac(r1.xyz);
  r1.xyz = -r1.xyz * float3(2,2,2) + float3(1,1,1);
  r1.xyz = saturate(abs(r1.xyz) * float3(3,3,3) + float3(-1,-1,-1));
  r1.xyz = float3(-1,-1,-1) + r1.xyz;
  r0.yzw = r0.zzz * r1.xyz + float3(1,1,1);
  r0.xyz = r0.yzw * r0.xxx;
  o0.xyz = v3.www * r0.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
