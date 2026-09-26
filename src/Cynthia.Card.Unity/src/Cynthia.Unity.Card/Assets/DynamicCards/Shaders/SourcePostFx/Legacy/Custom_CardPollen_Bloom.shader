Shader "DynamicCards/SourcePostFx/Legacy/Custom_CardPollen_Bloom" {
Properties {
_MainTex("_MainTex",2D)="black" {}
_Spread("_Spread",Float)=0.004999999888241291
_Intensity("_Intensity",Float)=1.0
_Contrast("_Contrast",Float)=2.0
}
SubShader {
Tags {"CanUseSpriteAtlas"="true" "IGNOREPROJECTOR"="true" "QUEUE"="Geometry+1" "RenderType"="Opaque"}
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
SamplerState sampler_MainTex;
Texture2D<float4> _MainTex;
float _Contrast;
float _Intensity;
float _Spread;












void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0)
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
  return;
}














void dc_f(
  float4 v0 : SV_POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[3];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_Spread,_Intensity,_Contrast,0);



  float4 r0,r1,r2,r3,r4,r5;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyzw = _MainTex.Sample(sampler_MainTex, v1.xy).xyzw;
  r1.x = cmp(0.180000007 < v1.x);
  if (r1.x != 0) {
    r1.x = dot(r0.xyz, float3(0.298999995,0.587000012,0.114));
    r1.x = 1 + -r1.x;
    r2.x = dc_f_cb0[2].x * r1.x;
    r2.y = -dc_f_cb0[2].x * r1.x;
    r1.xyz = float3(0.104163997,0.104163997,0.104163997) * r0.xyz;
    r3.xyzw = r2.xxxx * float4(0.75,0.75,1.60000002,1.60000002) + v1.xyxy;
    r4.xyzw = _MainTex.Sample(sampler_MainTex, r3.xy).xyzw;
    r1.xyz = r4.xyz * float3(0.154005006,0.154005006,0.154005006) + r1.xyz;
    r4.xyzw = -r2.xxxx * float4(0.75,0.75,1.60000002,1.60000002) + v1.xyxy;
    r5.xyzw = _MainTex.Sample(sampler_MainTex, r4.xy).xyzw;
    r1.xyz = r5.xyz * float3(0.154005006,0.154005006,0.154005006) + r1.xyz;
    r3.xyzw = _MainTex.Sample(sampler_MainTex, r3.zw).xyzw;
    r1.xyz = r3.xyz * float3(0.0453912988,0.0453912988,0.0453912988) + r1.xyz;
    r3.xyzw = _MainTex.Sample(sampler_MainTex, r4.zw).xyzw;
    r1.xyz = r3.xyz * float3(0.0453912988,0.0453912988,0.0453912988) + r1.xyz;
    r1.xyz = r0.xyz * float3(0.104163997,0.104163997,0.104163997) + r1.xyz;
    r3.xyzw = r2.xyxy * float4(0.75,0.75,1.60000002,1.60000002) + v1.xyxy;
    r4.xyzw = _MainTex.Sample(sampler_MainTex, r3.xy).xyzw;
    r1.xyz = r4.xyz * float3(0.154005006,0.154005006,0.154005006) + r1.xyz;
    r2.xyzw = -r2.xyxy * float4(0.75,0.75,1.60000002,1.60000002) + v1.xyxy;
    r4.xyzw = _MainTex.Sample(sampler_MainTex, r2.xy).xyzw;
    r1.xyz = r4.xyz * float3(0.154005006,0.154005006,0.154005006) + r1.xyz;
    r3.xyzw = _MainTex.Sample(sampler_MainTex, r3.zw).xyzw;
    r1.xyz = r3.xyz * float3(0.0453912988,0.0453912988,0.0453912988) + r1.xyz;
    r2.xyzw = _MainTex.Sample(sampler_MainTex, r2.zw).xyzw;
    r1.xyz = r2.xyz * float3(0.0453912988,0.0453912988,0.0453912988) + r1.xyz;
    r1.xyz = float3(-0.5,-0.5,-0.5) + r1.xyz;
    r1.xyz = r1.xyz * dc_f_cb0[2].zzz + float3(0.5,0.5,0.5);
    r1.xyz = max(float3(0,0,0), r1.xyz);
    r0.xyz = r1.xyz * dc_f_cb0[2].yyy + r0.xyz;
  }
  o0.xyzw = r0.xyzw;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
