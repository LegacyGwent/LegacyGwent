Shader "DynamicCards/Legacy/VFX_Effects_QuenShield" {
Properties {
_Noise("_Noise",2D)="white" {}
_RimCol("_RimCol",Color)=(0.0,0.0,0.0,1.0)
_Falloff("_Falloff",Float)=5.0
_Transparency("_Transparency",Float)=15.0
_Strength("_Strength",Float)=5.0
_ImpactPoint("_ImpactPoint",Vector)=(0.0,0.0,0.0,0.0)
_Speed("_Speed",Float)=1.0
_Intensity("_Intensity",Float)=1.0
_Appear("_Appear",Float)=5.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "LIGHTMODE"="FORWARDBASE" "QUEUE"="Transparent" "RenderType"="Transparent"}
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
SamplerState sampler_Noise;
Texture2D<float4> _Noise;
float _Appear;
float _Falloff;
float _Intensity;
float _Speed;
float _Strength;
float _Transparency;
float4 _ImpactPoint;
float4 _Noise_ST;
float4 _RimCol;


















void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float4 v2 : COLOR0,
  float4 v3 : TEXCOORD0,
  float2 v4 : TEXCOORD1,
  out float4 o0 : SV_POSITION0,
  out float3 o1 : TEXCOORD0,
  out float p1 : TEXCOORD4,
  out float4 o2 : TEXCOORD1,
  out float2 o3 : TEXCOORD2,
  out float2 p3 : TEXCOORD3,
  out float4 o4 : TEXCOORD5)
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
float4 dc_v_cb1[8];
dc_v_cb1[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb1[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb1[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb1[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
dc_v_cb1[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_v_cb1[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_v_cb1[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);
dc_v_cb1[7]=float4(unity_WorldToObject[0][3],unity_WorldToObject[1][3],unity_WorldToObject[2][3],unity_WorldToObject[3][3]);
float4 dc_v_cb0[7];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_Noise_ST.x,_Noise_ST.y,_Noise_ST.z,_Noise_ST.w);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,_Strength,0);
dc_v_cb0[5]=float4(_ImpactPoint.x,_ImpactPoint.y,_ImpactPoint.z,_ImpactPoint.w);
dc_v_cb0[6]=float4(0,0,_Appear,0);

float4 dc_pack_OSGN_1=float4(0,0,0,0);
float4 dc_pack_OSGN_3=float4(0,0,0,0);

  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb1[5].xyzw * dc_v_cb0[5].yyyy;
  r0.xyzw = dc_v_cb1[4].xyzw * dc_v_cb0[5].xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[6].xyzw * dc_v_cb0[5].zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb1[7].xyzw * dc_v_cb0[5].wwww + r0.xyzw;
  r0.xyzw = -v0.xyzw + r0.xyzw;
  r0.x = dot(r0.xyzw, r0.xyzw);
  r0.x = sqrt(r0.x);
  r0.x = 0.920000017 * r0.x;
  r0.y = 100 * dc_v_cb0[4].z;
  r0.x = r0.y / r0.x;
  r0.yz = v4.xy * dc_v_cb0[2].xy + dc_v_cb0[2].zw;
  r1.xyzw = _Noise.SampleLevel(sampler_Noise, r0.yz, 0).xyzw;
  r0.y = r1.y * r0.x;
  dc_pack_OSGN_1.w = r0.x;
  r0.x = r0.y * dc_v_cb0[6].z + v0.y;
  r0.xyzw = dc_v_cb1[1].xyzw * r0.xxxx;
  r0.xyzw = dc_v_cb1[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[2].xyzw * v0.zzzz + r0.xyzw;
  r0.xyzw = dc_v_cb1[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb2[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb2[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb2[20].xyzw * r0.wwww + r1.xyzw;
  r0.xyz = dc_v_cb1[1].xyz * v1.yyy;
  r0.xyz = dc_v_cb1[0].xyz * v1.xxx + r0.xyz;
  dc_pack_OSGN_1.xyz = dc_v_cb1[2].xyz * v1.zzz + r0.xyz;
  r0.xyz = dc_v_cb1[1].xyz * v0.yyy;
  r0.xyz = dc_v_cb1[0].xyz * v0.xxx + r0.xyz;
  r0.xyz = dc_v_cb1[2].xyz * v0.zzz + r0.xyz;
  o2.xyz = dc_v_cb1[3].xyz * v0.www + r0.xyz;
  dc_pack_OSGN_3.xy = v3.xy;
  dc_pack_OSGN_3.zw = v4.xy;
  o4.xyzw = v2.xyzw;
  o1=dc_pack_OSGN_1.xyz;
p1=dc_pack_OSGN_1.w;
o3=dc_pack_OSGN_3.xy;
p3=dc_pack_OSGN_3.zw;
return;
}
















void dc_f(
  float4 v0 : SV_POSITION0,
  float3 v1 : TEXCOORD0,
  float w1 : TEXCOORD4,
  float4 v2 : TEXCOORD1,
  float2 v3 : TEXCOORD2,
  float2 w3 : TEXCOORD3,
  float4 v4 : TEXCOORD5,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[5];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(_WorldSpaceCameraPos.x,_WorldSpaceCameraPos.y,_WorldSpaceCameraPos.z,0);
float4 dc_f_cb0[7];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_Noise_ST.x,_Noise_ST.y,_Noise_ST.z,_Noise_ST.w);
dc_f_cb0[3]=float4(_RimCol.x,_RimCol.y,_RimCol.z,_RimCol.w);
dc_f_cb0[4]=float4(_Falloff,_Transparency,0,0);
dc_f_cb0[5]=float4(0,0,0,0);
dc_f_cb0[6]=float4(_Speed,_Intensity,_Appear,0);

float4 dc_pack_ISGN_1=float4(v1.x,v1.y,v1.z,w1);
float4 dc_pack_ISGN_3=float4(v3.x,v3.y,w3.x,w3.y);

  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb1[0].x * dc_f_cb0[6].x;
  r0.y = 0;
  r0.xy = dc_pack_ISGN_3.xy + r0.xy;
  r0.xy = r0.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r0.xyzw = _Noise.Sample(sampler_Noise, r0.xy).xyzw;
  r1.x = -dc_f_cb1[0].x * dc_f_cb0[6].x;
  r1.y = 0;
  r0.yz = dc_pack_ISGN_3.xy + r1.xy;
  r1.zw = float2(0.899999976,-1.52999997) * r1.xx;
  r1.xyzw = dc_pack_ISGN_3.xyxy + -r1.yzyw;
  r1.xyzw = r1.xyzw * dc_f_cb0[2].xyxy + dc_f_cb0[2].zwzw;
  r0.yz = r0.yz * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r2.xyzw = _Noise.Sample(sampler_Noise, r0.yz).xyzw;
  r0.x = r2.y + r0.x;
  r2.xyzw = _Noise.Sample(sampler_Noise, r1.xy).xyzw;
  r1.xyzw = _Noise.Sample(sampler_Noise, r1.zw).xyzw;
  r0.x = r2.z + r0.x;
  r0.x = r0.x + r1.w;
  r0.x = 0.00100000005 + r0.x;
  r0.x = dc_f_cb0[6].y / r0.x;
  r0.y = dc_pack_ISGN_1.w * 0.0500000007 + 0.5;
  r0.z = abs(r0.y) * -0.0187292993 + 0.0742610022;
  r0.z = r0.z * abs(r0.y) + -0.212114394;
  r0.z = r0.z * abs(r0.y) + 1.57072878;
  r0.w = 1 + -abs(r0.y);
  r0.y = cmp(r0.y < -r0.y);
  r0.w = sqrt(r0.w);
  r1.x = r0.z * r0.w;
  r1.x = r1.x * -2 + 3.14159274;
  r0.y = r0.y ? r1.x : 0;
  r0.y = r0.z * r0.w + r0.y;
  r0.y = 1.57079637 + -r0.y;
  r1.xyz = -dc_f_cb1[4].xyz + v2.xyz;
  r0.z = dot(r1.xyz, r1.xyz);
  r0.z = rsqrt(r0.z);
  r1.xyz = r1.xyz * r0.zzz;
  r0.z = dot(dc_pack_ISGN_1.xyz, dc_pack_ISGN_1.xyz);
  r0.z = rsqrt(r0.z);
  r2.xyz = dc_pack_ISGN_1.xyz * r0.zzz;
  r0.z = dot(-r1.xyz, r2.xyz);
  r0.z = 1 + -r0.z;
  r0.w = log2(r0.z);
  r0.w = dc_f_cb0[4].x * r0.w;
  r0.w = exp2(r0.w);
  r0.w = dc_f_cb0[4].y * r0.w;
  r0.y = r0.w * r0.y;
  r1.xyz = log2(v4.xyz);
  r1.xyz = dc_f_cb0[6].zzz * r1.xyz;
  r1.xyz = exp2(r1.xyz);
  r2.xyz = r1.xyz * r0.yyy;
  o0.w = r0.y;
  r2.xyz = dc_f_cb0[3].xyz * r2.xyz;
  r0.xyw = r2.xyz * r0.xxx;
  r1.w = -0.117647059 * dc_pack_ISGN_1.w;
  sincos(r1.w, r2.x, r3.x);
  r1.w = r2.x / r3.x;
  r1.w = r1.w * r1.w;
  r1.w = min(2, r1.w);
  r1.xyz = r1.www * r1.xyz;
  r0.xyw = r1.xyz * dc_f_cb0[3].xyz + r0.xyw;
  o0.xyz = r0.zzz * r0.xyw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
