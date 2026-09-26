Shader "DynamicCards/Native/VFX_Effects_ImageLayerShaderSpecular" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_Light("_Light",2D)="white" {}
_LightIntensity("_LightIntensity",Float)=1.0
__Turbulence("__Turbulence",2D)="white" {}
_Speed("_Speed",Float)=-0.5
_TimeOffset("_TimeOffset",Float)=0.5
_LightTint("_LightTint",Color)=(1.0,1.0,1.0,1.0)
_SpecSharpness("_SpecSharpness",Float)=1.0
_SpecSize("_SpecSize",Float)=0.0
_off_RG_on("_off_RG_on",Float)=0.0
_on_B("_on_B",Float)=0.0
_Move("_Move",Float)=0.0
_Displaceamount("_Displaceamount",Float)=0.0
_SpecRange("_SpecRange",Float)=1.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"QUEUE"="AlphaTest" "RenderType"="TransparentCutout"}
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
SamplerState sampler_Light;
SamplerState sampler_MainTex;
SamplerState sampler__Turbulence;
Texture2D<float4> _Light;
Texture2D<float4> _MainTex;
Texture2D<float4> __Turbulence;
float _Displaceamount;
float _LightIntensity;
float _Move;
float _SpecRange;
float _SpecSharpness;
float _SpecSize;
float _Speed;
float _TimeOffset;
float _off_RG_on;
float _on_B;
float4 _LightTint;
float4 _Light_ST;
float4 _MainTex_ST;
float4 __Turbulence_ST;












void dc_v(
  float4 v0 : POSITION0,
  float3 v1 : NORMAL0,
  float2 v2 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float3 o3 : TEXCOORD2)
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
float4 dc_v_cb0[7];
dc_v_cb0[0]=float4(unity_ObjectToWorld[0][0],unity_ObjectToWorld[1][0],unity_ObjectToWorld[2][0],unity_ObjectToWorld[3][0]);
dc_v_cb0[1]=float4(unity_ObjectToWorld[0][1],unity_ObjectToWorld[1][1],unity_ObjectToWorld[2][1],unity_ObjectToWorld[3][1]);
dc_v_cb0[2]=float4(unity_ObjectToWorld[0][2],unity_ObjectToWorld[1][2],unity_ObjectToWorld[2][2],unity_ObjectToWorld[3][2]);
dc_v_cb0[3]=float4(unity_ObjectToWorld[0][3],unity_ObjectToWorld[1][3],unity_ObjectToWorld[2][3],unity_ObjectToWorld[3][3]);
dc_v_cb0[4]=float4(unity_WorldToObject[0][0],unity_WorldToObject[1][0],unity_WorldToObject[2][0],unity_WorldToObject[3][0]);
dc_v_cb0[5]=float4(unity_WorldToObject[0][1],unity_WorldToObject[1][1],unity_WorldToObject[2][1],unity_WorldToObject[3][1]);
dc_v_cb0[6]=float4(unity_WorldToObject[0][2],unity_WorldToObject[1][2],unity_WorldToObject[2][2],unity_WorldToObject[3][2]);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = dc_v_cb0[1].xyzw * v0.yyyy;
  r0.xyzw = dc_v_cb0[0].xyzw * v0.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb0[2].xyzw * v0.zzzz + r0.xyzw;
  r1.xyzw = dc_v_cb0[3].xyzw + r0.xyzw;
  o2.xyzw = dc_v_cb0[3].xyzw * v0.wwww + r0.xyzw;
  r0.xyzw = dc_v_cb1[18].xyzw * r1.yyyy;
  r0.xyzw = dc_v_cb1[17].xyzw * r1.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[19].xyzw * r1.zzzz + r0.xyzw;
  o0.xyzw = dc_v_cb1[20].xyzw * r1.wwww + r0.xyzw;
  o1.xy = v2.xy;
  r0.x = dot(v1.xyz, dc_v_cb0[4].xyz);
  r0.y = dot(v1.xyz, dc_v_cb0[5].xyz);
  r0.z = dot(v1.xyz, dc_v_cb0[6].xyz);
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  o3.xyz = r0.xyz * r0.www;
  return;
}
























void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float3 v3 : TEXCOORD2,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[5];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(_WorldSpaceCameraPos.x,_WorldSpaceCameraPos.y,_WorldSpaceCameraPos.z,0);
float4 dc_f_cb0[10];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[3]=float4(_Light_ST.x,_Light_ST.y,_Light_ST.z,_Light_ST.w);
dc_f_cb0[4]=float4(_LightIntensity,0,0,0);
dc_f_cb0[5]=float4(__Turbulence_ST.x,__Turbulence_ST.y,__Turbulence_ST.z,__Turbulence_ST.w);
dc_f_cb0[6]=float4(_Speed,_TimeOffset,0,0);
dc_f_cb0[7]=float4(_LightTint.x,_LightTint.y,_LightTint.z,_LightTint.w);
dc_f_cb0[8]=float4(_SpecSharpness,_SpecSize,_off_RG_on,_on_B);
dc_f_cb0[9]=float4(_Move,_Displaceamount,_SpecRange,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.w = -0.5 + r0.w;
  r0.w = cmp(r0.w < 0);
  if (r0.w != 0) discard;
  r1.xyz = dc_f_cb1[4].xyz + -v2.xyz;
  r0.w = dot(r1.xyz, r1.xyz);
  r0.w = rsqrt(r0.w);
  r2.xyz = r1.xyz * r0.www;
  r1.w = dot(-r2.xyz, v3.xyz);
  r1.w = r1.w + r1.w;
  r2.xyz = v3.xyz * -r1.www + -r2.xyz;
  r1.xyz = r1.xyz * r0.www + -r2.xyz;
  r0.w = r1.y + -r1.x;
  r0.w = dc_f_cb0[8].z * r0.w + r1.x;
  r1.x = r1.z + -r0.w;
  r0.w = dc_f_cb0[8].w * r1.x + r0.w;
  r0.w = dc_f_cb0[9].x + r0.w;
  r0.w = dc_f_cb0[9].z * dc_f_cb0[9].y + r0.w;
  r1.x = -dc_f_cb0[8].x * r0.w + dc_f_cb0[8].y;
  r0.w = dc_f_cb0[8].x * r0.w + dc_f_cb0[8].y;
  r1.x = 1 + r1.x;
  r0.w = saturate(r1.x * r0.w);
  r1.x = 0.5 + dc_f_cb0[6].x;
  r1.y = dc_f_cb1[0].x + dc_f_cb0[6].y;
  r1.x = r1.x * r1.y;
  r1.xy = r1.xx * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r1.xyzw = __Turbulence.Sample(sampler__Turbulence, r1.xy).xyzw;
  r1.yz = v1.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r2.xyzw = _Light.Sample(sampler_Light, r1.yz).xyzw;
  r1.yzw = dc_f_cb0[7].xyz * r2.xyz;
  r1.yzw = dc_f_cb0[4].xxx * r1.yzw;
  r1.xyz = r1.yzw * r1.xxx;
  o0.xyz = r1.xyz * r0.www + r0.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
