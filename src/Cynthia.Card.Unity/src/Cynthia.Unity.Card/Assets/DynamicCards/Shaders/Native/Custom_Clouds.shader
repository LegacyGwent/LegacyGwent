Shader "DynamicCards/Native/Custom_Clouds" {
Properties {
_MainTex("_MainTex",2D)="bump" {}
_FlowMap("_FlowMap",2D)="bump" {}
_Addcolormask("_Addcolormask",2D)="white" {}
_Offsetamount("_Offsetamount",Float)=-0.6499999761581421
_Timemultiplier("_Timemultiplier",Float)=0.25
_Geometrywavelength("_Geometrywavelength",Float)=0.0
_Geometrytimescale("_Geometrytimescale",Float)=0.0
_Mappingoffset("_Mappingoffset",Float)=0.0
_GeometryuseUV("_GeometryuseUV",Float)=0.0
_Geometryoffset("_Geometryoffset",Float)=0.0
_UVset0or1("_UVset0or1",Float)=0.0
_Addcolorpower("_Addcolorpower",Float)=0.0
_Addcolormultiplier("_Addcolormultiplier",Float)=0.0
_Addcolorspeed("_Addcolorspeed",Float)=0.0
_Addcolor("_Addcolor",Color)=(0.5,0.5,0.5,1.0)
_Alphamult("_Alphamult",Float)=1.0
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
SamplerState sampler_Addcolormask;
SamplerState sampler_FlowMap;
SamplerState sampler_MainTex;
Texture2D<float4> _Addcolormask;
Texture2D<float4> _FlowMap;
Texture2D<float4> _MainTex;
float _Addcolormultiplier;
float _Addcolorpower;
float _Addcolorspeed;
float _Alphamult;
float _Geometryoffset;
float _Geometrytimescale;
float _GeometryuseUV;
float _Geometrywavelength;
float _Mappingoffset;
float _Offsetamount;
float _Timemultiplier;
float _UVset0or1;
float4 _Addcolor;
float4 _Addcolormask_ST;
float4 _FlowMap_ST;
float4 _MainTex_ST;
float4 _TimeEditor;




















void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : TEXCOORD0,
  float2 v2 : TEXCOORD1,
  float4 v3 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0,
  out float2 p1 : TEXCOORD1,
  out float4 o2 : COLOR0)
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
float4 dc_v_cb1[1];
dc_v_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_v_cb0[7];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(_FlowMap_ST.x,_FlowMap_ST.y,_FlowMap_ST.z,_FlowMap_ST.w);
dc_v_cb0[5]=float4(0,0,_Geometrywavelength,_Geometrytimescale);
dc_v_cb0[6]=float4(_GeometryuseUV,_Geometryoffset,_UVset0or1,_Mappingoffset);

float4 dc_pack_OSGN_1=float4(0,0,0,0);

  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = v2.y + -v2.x;
  r0.x = dc_v_cb0[6].x * r0.x + v2.x;
  r0.x = dc_v_cb0[6].w + r0.x;
  r0.y = dc_v_cb1[0].y + dc_v_cb0[2].y;
  r0.y = dc_v_cb0[5].w * r0.y;
  r0.x = r0.x * dc_v_cb0[5].z + r0.y;
  r0.x = sin(r0.x);
  r0.x = dc_v_cb0[6].y * r0.x;
  r0.yz = v2.xy + -v1.xy;
  r0.yz = dc_v_cb0[6].zz * r0.yz + v1.xy;
  r0.yz = r0.yz * dc_v_cb0[4].xy + dc_v_cb0[4].zw;
  r1.xyzw = _FlowMap.SampleLevel(sampler_FlowMap, r0.yz, 0).xyzw;
  r0.xyz = r1.zzz * r0.xxx + v0.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
  dc_pack_OSGN_1.xy = v1.xy;
  dc_pack_OSGN_1.zw = v2.xy;
  o2.xyzw = v3.xyzw;
  o1=dc_pack_OSGN_1.xy;
p1=dc_pack_OSGN_1.zw;
return;
}
























void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  float2 w1 : TEXCOORD1,
  float4 v2 : COLOR0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[11];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[4]=float4(_FlowMap_ST.x,_FlowMap_ST.y,_FlowMap_ST.z,_FlowMap_ST.w);
dc_f_cb0[5]=float4(_Offsetamount,_Timemultiplier,0,0);
dc_f_cb0[6]=float4(0,0,_UVset0or1,0);
dc_f_cb0[7]=float4(_Addcolorpower,_Addcolormultiplier,_Addcolorspeed,0);
dc_f_cb0[8]=float4(_Addcolor.x,_Addcolor.y,_Addcolor.z,_Addcolor.w);
dc_f_cb0[9]=float4(_Addcolormask_ST.x,_Addcolormask_ST.y,_Addcolormask_ST.z,_Addcolormask_ST.w);
dc_f_cb0[10]=float4(_Alphamult,0,0,0);

float4 dc_pack_ISGN_1=float4(v1.x,v1.y,w1.x,w1.y);

  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = dc_pack_ISGN_1.zw + -dc_pack_ISGN_1.xy;
  r0.xy = dc_f_cb0[6].zz * r0.xy + dc_pack_ISGN_1.xy;
  r0.zw = r0.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r1.xyzw = _FlowMap.Sample(sampler_FlowMap, r0.zw).xyzw;
  r0.zw = r1.xy * float2(2,2) + float2(-1,-1);
  r0.zw = dc_f_cb0[5].xx * r0.zw;
  r1.x = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r1.y = r1.x * dc_f_cb0[5].y + 0.5;
  r1.y = frac(r1.y);
  r1.yz = r0.zw * r1.yy + r0.xy;
  r1.yz = r1.yz * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r2.xyzw = _MainTex.Sample(sampler_MainTex, r1.yz).xyzw;
  r1.y = dc_f_cb0[5].y * r1.x;
  r3.x = dc_f_cb0[7].z * r1.x;
  r1.x = frac(r1.y);
  r0.xy = r0.zw * r1.xx + r0.xy;
  r0.z = -0.5 + r1.x;
  r0.z = r0.z + r0.z;
  r0.xy = r0.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r2.xyzw = r2.xyzw + -r1.xyzw;
  r0.xyzw = abs(r0.zzzz) * r2.xyzw + r1.xyzw;
  r1.x = dot(r0.xyz, float3(0.300000012,0.589999974,0.109999999));
  r3.y = 1;
  r1.yz = dc_pack_ISGN_1.zw + r3.xy;
  r1.yz = r1.yz * dc_f_cb0[9].xy + dc_f_cb0[9].zw;
  r2.xyzw = _Addcolormask.Sample(sampler_Addcolormask, r1.yz).xyzw;
  r1.x = r2.x * r1.x;
  r1.x = log2(r1.x);
  r1.x = dc_f_cb0[7].x * r1.x;
  r1.x = exp2(r1.x);
  r1.x = dc_f_cb0[7].y * r1.x;
  o0.xyz = r1.xxx * dc_f_cb0[8].xyz + r0.xyz;
  r0.x = dc_f_cb0[10].x * r0.w;
  o0.w = v2.w * r0.x;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
