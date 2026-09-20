Shader "DynamicCards/Legacy/VFX_Effects_MovingTextureOverlay" {
Properties {
_MainTex("_MainTex",2D)="white" {}
_MovingTex("_MovingTex",2D)="white" {}
_Color("_Color",Color)=(0.5,0.5,0.5,1.0)
_Uspeed("_Uspeed",Float)=1.0
_Vspeed("_Vspeed",Float)=1.0
_Uspeed2("_Uspeed2",Float)=0.10000000149011612
_Vspeed2("_Vspeed2",Float)=0.10000000149011612
_alpha("_alpha",Float)=0.75
_mask_multiply("_mask_multiply",Float)=2.0
_opacity_power("_opacity_power",Float)=1.0
_Brightness("_Brightness",Float)=1.0
_Contrast("_Contrast",Float)=1.0
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
SamplerState sampler_MainTex;
SamplerState sampler_MovingTex;
Texture2D<float4> _MainTex;
Texture2D<float4> _MovingTex;
float _Brightness;
float _Contrast;
float _Uspeed2;
float _Uspeed;
float _Vspeed2;
float _Vspeed;
float _alpha;
float _mask_multiply;
float _opacity_power;
float4 _Color;
float4 _MainTex_ST;
float4 _MovingTex_ST;
float4 _TimeEditor;












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
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[9];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_Color.x,_Color.y,_Color.z,_Color.w);
dc_f_cb0[4]=float4(_Vspeed,_Uspeed,_alpha,_mask_multiply);
dc_f_cb0[5]=float4(_opacity_power,_Uspeed2,_Vspeed2,0);
dc_f_cb0[6]=float4(_MovingTex_ST.x,_MovingTex_ST.y,_MovingTex_ST.z,_MovingTex_ST.w);
dc_f_cb0[7]=float4(_MainTex_ST.x,_MainTex_ST.y,_MainTex_ST.z,_MainTex_ST.w);
dc_f_cb0[8]=float4(_Brightness,_Contrast,0,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_f_cb1[0].x + dc_f_cb0[2].x;
  r0.yz = r0.xx * dc_f_cb0[5].yz + v1.xy;
  r0.xw = r0.xx * dc_f_cb0[4].yx + v1.xy;
  r0.xw = r0.xw * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r1.xyzw = _MovingTex.Sample(sampler_MovingTex, r0.xw).xyzw;
  r0.xy = r0.yz * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r0.xyzw = _MovingTex.Sample(sampler_MovingTex, r0.xy).xyzw;
  r0.xyz = r1.xyz * r0.xyz;
  r0.xyz = log2(r0.xyz);
  r0.xyz = dc_f_cb0[8].yyy * r0.xyz;
  r0.xyz = exp2(r0.xyz);
  r0.w = 1 + -v1.y;
  r0.w = min(1, abs(r0.w));
  r0.w = dc_f_cb0[4].w * r0.w;
  r0.xyz = r0.xyz * r0.www;
  r0.xyz = dc_f_cb0[3].xyz * r0.xyz;
  r1.xy = v1.xy * dc_f_cb0[7].xy + dc_f_cb0[7].zw;
  r2.xyzw = _MainTex.Sample(sampler_MainTex, r1.xy).xyzw;
  r1.xyz = dc_f_cb0[8].xxx * r2.xyz;
  o0.xyz = r1.xyz * r0.xyz;
  r0.x = 3 * abs(v1.y);
  r0.x = min(1, r0.x);
  r0.x = r1.w * r0.x;
  r0.x = dc_f_cb0[4].z * r0.x;
  r0.x = log2(r0.x);
  r0.x = dc_f_cb0[5].x * r0.x;
  o0.w = exp2(r0.x);
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
