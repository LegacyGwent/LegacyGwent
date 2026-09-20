Shader "DynamicCards/Latest/UI_Blur_GrabPassDisplay" {
Properties {

}
SubShader {
Tags {"PreviewType"="Plane" "QUEUE"="Transparent" "RenderType"="Opaque"}
GrabPass { "_UIGrab" }
Pass {
Cull Off
ZWrite Off
ZTest [unity_GUIZTestMode]
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












void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
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
  r0.xyzw = dc_v_cb1[20].xyzw * r0.wwww + r1.xyzw;
  o0.xyzw = r0.xyzw;
  r0.xy = r0.xy * float2(1,-1) + r0.ww;
  o1.zw = r0.zw;
  o1.xy = float2(0.5,0.5) * r0.xy;
  o2.xyzw = v2.xyzw;
  return;
}












void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_Target0)
{




  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy / v1.ww;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r1.x = r0.w * v2.w + -0.00100000005;
  r0.xyzw = v2.xyzw * r0.xyzw;
  o0.xyzw = r0.xyzw;
  r0.x = cmp(r1.x < 0);
  if (r0.x != 0) discard;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
