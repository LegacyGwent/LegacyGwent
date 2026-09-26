Shader "DynamicCards/Legacy/Skybox_Procedural" {
Properties {
_SunDisk("_SunDisk",Float)=2.0
_SunSize("_SunSize",Float)=0.03999999910593033
_AtmosphereThickness("_AtmosphereThickness",Float)=1.0
_SkyTint("_SkyTint",Color)=(0.5,0.5,0.5,1.0)
_GroundColor("_GroundColor",Color)=(0.36899998784065247,0.3490000069141388,0.3409999907016754,1.0)
_Exposure("_Exposure",Float)=1.2999999523162842
}
SubShader {
Tags {"PreviewType"="Skybox" "QUEUE"="Background" "RenderType"="Background"}
Pass {
Cull Off
ZWrite Off
ZTest LEqual
Blend One Zero
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
float _AtmosphereThickness;
float _Exposure;
float3 _GroundColor;
float3 _SkyTint;
















void dc_v(
  float4 v0 : POSITION0,
  out float4 o0 : SV_POSITION0,
  out float o1 : TEXCOORD0,
  out float3 p1 : TEXCOORD1,
  out float3 o2 : TEXCOORD2)
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
dc_v_cb1[0]=float4(_WorldSpaceLightPos0.x,_WorldSpaceLightPos0.y,_WorldSpaceLightPos0.z,_WorldSpaceLightPos0.w);
float4 dc_v_cb0[7];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(_Exposure,_GroundColor.x,_GroundColor.y,_GroundColor.z);
dc_v_cb0[5]=float4(0,_SkyTint.x,_SkyTint.y,_SkyTint.z);
dc_v_cb0[6]=float4(_AtmosphereThickness,0,0,0);

float4 dc_pack_OSGN_1=float4(0,0,0,0);

  float4 r0,r1,r2,r3,r4,r5,r6;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb2[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb2[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * v0.zzzz + r0.xyzw;
  r1.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r2.xyzw = dc_v_cb3[18].xyzw * r1.yyyy;
  r2.xyzw = dc_v_cb3[17].xyzw * r1.xxxx + r2.xyzw;
  r2.xyzw = dc_v_cb3[19].xyzw * r1.zzzz + r2.xyzw;
  o0.xyzw = dc_v_cb3[20].xyzw * r1.wwww + r2.xyzw;
  r1.xyz = float3(1,1,1) + -dc_v_cb0[5].yzw;
  r1.xyz = r1.xyz * float3(0.300000012,0.300000042,0.300000012) + float3(0.5,0.419999987,0.324999988);
  r1.xyz = r1.xyz * r1.xyz;
  r1.xyz = r1.xyz * r1.xyz;
  r1.xyz = float3(1,1,1) / r1.xyz;
  r0.w = log2(dc_v_cb0[6].x);
  r0.w = 2.5 * r0.w;
  r0.w = exp2(r0.w);
  r2.xy = float2(0.049999997,0.0314159282) * r0.ww;
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  r3.xyz = r0.xyz * r0.www;
  r0.x = cmp(r3.y >= 0);
  if (r0.x != 0) {
    r0.x = r3.y * r3.y + 0.0506249666;
    r0.x = sqrt(r0.x);
    r0.x = -r0.y * r0.w + r0.x;
    r0.y = -r3.y * 1 + 1;
    r0.z = r0.y * 5.25 + -6.80000019;
    r0.z = r0.y * r0.z + 3.82999992;
    r0.z = r0.y * r0.z + 0.458999991;
    r0.y = r0.y * r0.z + -0.00286999997;
    r0.y = 1.44269502 * r0.y;
    r0.y = exp2(r0.y);
    r0.xyz = float3(0.5,0.246031836,20) * r0.xyx;
    r4.xyz = r3.xyz * r0.xxx;
    r4.xyz = r4.xyz * float3(0.5,0.5,0.5) + float3(0,1.00010002,0);
    r0.w = dot(r4.xyz, r4.xyz);
    r0.w = sqrt(r0.w);
    r1.w = 1 + -r0.w;
    r1.w = 230.831207 * r1.w;
    r1.w = exp2(r1.w);
    r2.z = dot(dc_v_cb1[0].xyz, r4.xyz);
    r2.z = r2.z / r0.w;
    r2.w = dot(r3.xyz, r4.xyz);
    r0.w = r2.w / r0.w;
    r2.z = 1 + -r2.z;
    r2.w = r2.z * 5.25 + -6.80000019;
    r2.w = r2.z * r2.w + 3.82999992;
    r2.w = r2.z * r2.w + 0.458999991;
    r2.z = r2.z * r2.w + -0.00286999997;
    r2.z = 1.44269502 * r2.z;
    r2.z = exp2(r2.z);
    r0.w = 1 + -r0.w;
    r2.w = r0.w * 5.25 + -6.80000019;
    r2.w = r0.w * r2.w + 3.82999992;
    r2.w = r0.w * r2.w + 0.458999991;
    r0.w = r0.w * r2.w + -0.00286999997;
    r0.w = 1.44269502 * r0.w;
    r0.w = exp2(r0.w);
    r0.w = 0.25 * r0.w;
    r0.w = r2.z * 0.25 + -r0.w;
    r0.w = r1.w * r0.w + r0.y;
    r0.w = max(0, r0.w);
    r0.w = min(50, r0.w);
    r5.xyz = r1.xyz * r2.yyy + float3(0.0125663709,0.0125663709,0.0125663709);
    r6.xyz = r5.xyz * -r0.www;
    r6.xyz = float3(1.44269502,1.44269502,1.44269502) * r6.xyz;
    r6.xyz = exp2(r6.xyz);
    r0.w = r1.w * r0.z;
    r4.xyz = r3.xyz * r0.xxx + r4.xyz;
    r0.x = dot(r4.xyz, r4.xyz);
    r0.x = sqrt(r0.x);
    r1.w = 1 + -r0.x;
    r1.w = 230.831207 * r1.w;
    r1.w = exp2(r1.w);
    r2.z = dot(dc_v_cb1[0].xyz, r4.xyz);
    r2.z = r2.z / r0.x;
    r2.w = dot(r3.xyz, r4.xyz);
    r0.x = r2.w / r0.x;
    r2.z = 1 + -r2.z;
    r2.w = r2.z * 5.25 + -6.80000019;
    r2.w = r2.z * r2.w + 3.82999992;
    r2.w = r2.z * r2.w + 0.458999991;
    r2.z = r2.z * r2.w + -0.00286999997;
    r2.z = 1.44269502 * r2.z;
    r2.z = exp2(r2.z);
    r0.x = 1 + -r0.x;
    r2.w = r0.x * 5.25 + -6.80000019;
    r2.w = r0.x * r2.w + 3.82999992;
    r2.w = r0.x * r2.w + 0.458999991;
    r0.x = r0.x * r2.w + -0.00286999997;
    r0.x = 1.44269502 * r0.x;
    r0.x = exp2(r0.x);
    r0.x = 0.25 * r0.x;
    r0.x = r2.z * 0.25 + -r0.x;
    r0.x = r1.w * r0.x + r0.y;
    r0.x = max(0, r0.x);
    r0.x = min(50, r0.x);
    r4.xyz = -r0.xxx * r5.xyz;
    r4.xyz = float3(1.44269502,1.44269502,1.44269502) * r4.xyz;
    r4.xyz = exp2(r4.xyz);
    r0.x = r1.w * r0.z;
    r0.xyz = r4.xyz * r0.xxx;
    r0.xyz = r6.xyz * r0.www + r0.xyz;
    r4.xyz = r2.xxx * r1.xyz;
    r4.xyz = r4.xyz * r0.xyz;
    r0.xyz = float3(0.0199999996,0.0199999996,0.0199999996) * r0.xyz;
  } else {
    r0.w = min(-0.00100000005, r3.y);
    r0.w = -9.99999975e-05 / r0.w;
    r5.xyz = r0.www * r3.xyz + float3(0,1.00010002,0);
    r1.w = dot(-r3.xyz, r5.xyz);
    r2.z = dot(dc_v_cb1[0].xyz, r5.xyz);
    r1.w = 1 + -r1.w;
    r2.w = r1.w * 5.25 + -6.80000019;
    r2.w = r1.w * r2.w + 3.82999992;
    r2.w = r1.w * r2.w + 0.458999991;
    r1.w = r1.w * r2.w + -0.00286999997;
    r1.w = 1.44269502 * r1.w;
    r1.w = exp2(r1.w);
    r2.z = 1 + -r2.z;
    r2.w = r2.z * 5.25 + -6.80000019;
    r2.w = r2.z * r2.w + 3.82999992;
    r2.w = r2.z * r2.w + 0.458999991;
    r2.z = r2.z * r2.w + -0.00286999997;
    r2.z = 1.44269502 * r2.z;
    r2.z = exp2(r2.z);
    r5.xy = float2(0.25,0.249900013) * r1.ww;
    r1.w = r2.z * 0.25 + r5.x;
    r2.zw = float2(0.5,20) * r0.ww;
    r5.xzw = r3.xyz * r2.zzz;
    r5.xzw = r5.xzw * float3(0.5,0.5,0.5) + float3(0,1.00010002,0);
    r0.w = dot(r5.xzw, r5.xzw);
    r0.w = sqrt(r0.w);
    r0.w = 1 + -r0.w;
    r0.w = 230.831207 * r0.w;
    r0.w = exp2(r0.w);
    r1.w = r0.w * r1.w + -r5.y;
    r1.w = max(0, r1.w);
    r1.w = min(50, r1.w);
    r5.xyz = r1.xyz * r2.yyy + float3(0.0125663709,0.0125663709,0.0125663709);
    r5.xyz = r5.xyz * -r1.www;
    r5.xyz = float3(1.44269502,1.44269502,1.44269502) * r5.xyz;
    r0.xyz = exp2(r5.xyz);
    r0.w = r0.w * r2.w;
    r2.yzw = r0.xyz * r0.www;
    r1.xyz = r1.xyz * r2.xxx + float3(0.0199999996,0.0199999996,0.0199999996);
    r4.xyz = r2.yzw * r1.xyz;
  }
  dc_pack_OSGN_1.x = -50 * r3.y;
  r1.xyz = dc_v_cb0[4].yzw * dc_v_cb0[4].yzw;
  r0.xyz = r1.xyz * r0.xyz + r4.xyz;
  dc_pack_OSGN_1.yzw = dc_v_cb0[4].xxx * r0.xyz;
  r0.x = dot(dc_v_cb1[0].xyz, -r3.xyz);
  r0.x = r0.x * r0.x;
  r0.x = r0.x * 0.75 + 0.75;
  r0.xyz = r4.xyz * r0.xxx;
  o2.xyz = dc_v_cb0[4].xxx * r0.xyz;
  o1=dc_pack_OSGN_1.x;
p1=dc_pack_OSGN_1.yzw;
return;
}








void dc_f(
  float4 v0 : SV_POSITION0,
  float v1 : TEXCOORD0,
  float3 w1 : TEXCOORD1,
  float3 v2 : TEXCOORD2,
  out float4 o0 : SV_Target0)
{


float4 dc_pack_ISGN_1=float4(v1,w1.x,w1.y,w1.z);

  float4 r0;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = saturate(dc_pack_ISGN_1.x);
  r0.yzw = -v2.xyz + dc_pack_ISGN_1.yzw;
  r0.xyz = r0.xxx * r0.yzw + v2.xyz;
  o0.xyz = sqrt(r0.xyz);
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
