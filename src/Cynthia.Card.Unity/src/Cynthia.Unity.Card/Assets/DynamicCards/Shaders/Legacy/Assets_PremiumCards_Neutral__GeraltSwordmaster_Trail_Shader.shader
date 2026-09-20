Shader "DynamicCards/Legacy/Assets_PremiumCards_Neutral__GeraltSwordmaster_Trail_Shader" {
Properties {
_Texture("_Texture",2D)="white" {}
_TEX_Tint("_TEX_Tint",Color)=(0.3897058963775635,0.0,0.0,1.0)
_TEX_2("_TEX_2",2D)="white" {}
_TEX_2_Color("_TEX_2_Color",Color)=(0.5,0.5,0.5,1.0)
_TEX_2_Power("_TEX_2_Power",Float)=1.0
_opacity_mult("_opacity_mult",Float)=0.5
_Time_mult("_Time_mult",Float)=1.0
_Mask("_Mask",2D)="white" {}
_Mask__UVRot("_Mask__UVRot",Float)=0.0
_Mask_contrastpower("_Mask_contrastpower",Float)=1.0
_Mask_2("_Mask_2",2D)="black" {}
_Mask_2_contrastpower("_Mask_2_contrastpower",Float)=1.0
_Mask_3("_Mask_3",2D)="white" {}
_Mask_3_UVRot("_Mask_3_UVRot",Float)=0.0
_TEX_Displace("_TEX_Displace",2D)="white" {}
_Displace_STR("_Displace_STR",Float)=0.0
_Mask_Reveal("_Mask_Reveal",2D)="white" {}
_Mask_contrastpower_copy("_Mask_contrastpower_copy",Float)=1.0
_Mask__UVRot_copy("_Mask__UVRot_copy",Float)=0.0
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
GrabPass {  }
Pass {
Cull Off
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
SamplerState sampler_GrabTexture;
SamplerState sampler_Mask;
SamplerState sampler_Mask_2;
SamplerState sampler_Mask_3;
SamplerState sampler_Mask_Reveal;
SamplerState sampler_TEX_2;
SamplerState sampler_TEX_Displace;
SamplerState sampler_Texture;
Texture2D<float4> _GrabTexture;
Texture2D<float4> _Mask;
Texture2D<float4> _Mask_2;
Texture2D<float4> _Mask_3;
Texture2D<float4> _Mask_Reveal;
Texture2D<float4> _TEX_2;
Texture2D<float4> _TEX_Displace;
Texture2D<float4> _Texture;
float _Displace_STR;
float _Mask_2_contrastpower;
float _Mask_3_UVRot;
float _Mask__UVRot;
float _Mask__UVRot_copy;
float _Mask_contrastpower;
float _Mask_contrastpower_copy;
float _TEX_2_Power;
float _Time_mult;
float _opacity_mult;
float4 _Mask_2_ST;
float4 _Mask_3_ST;
float4 _Mask_Reveal_ST;
float4 _Mask_ST;
float4 _TEX_2_Color;
float4 _TEX_2_ST;
float4 _TEX_Displace_ST;
float4 _TEX_Tint;
float4 _Texture_ST;
float4 _TimeEditor;












void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1)
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
  o2.xyzw = r0.xyzw;
  o1.xy = v1.xy;
  return;
}












































void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  uint v3 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[6];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
dc_f_cb1[1]=float4(0,0,0,0);
dc_f_cb1[2]=float4(0,0,0,0);
dc_f_cb1[3]=float4(0,0,0,0);
dc_f_cb1[4]=float4(0,0,0,0);
dc_f_cb1[5]=float4(_ProjectionParams.x,_ProjectionParams.y,_ProjectionParams.z,_ProjectionParams.w);
float4 dc_f_cb0[19];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_Texture_ST.x,_Texture_ST.y,_Texture_ST.z,_Texture_ST.w);
dc_f_cb0[4]=float4(_opacity_mult,_Time_mult,0,0);
dc_f_cb0[5]=float4(_Mask_ST.x,_Mask_ST.y,_Mask_ST.z,_Mask_ST.w);
dc_f_cb0[6]=float4(_Mask_contrastpower,0,0,0);
dc_f_cb0[7]=float4(_Mask_2_ST.x,_Mask_2_ST.y,_Mask_2_ST.z,_Mask_2_ST.w);
dc_f_cb0[8]=float4(_Mask_2_contrastpower,0,0,0);
dc_f_cb0[9]=float4(_TEX_Tint.x,_TEX_Tint.y,_TEX_Tint.z,_TEX_Tint.w);
dc_f_cb0[10]=float4(_Mask_3_ST.x,_Mask_3_ST.y,_Mask_3_ST.z,_Mask_3_ST.w);
dc_f_cb0[11]=float4(_Mask_3_UVRot,0,0,0);
dc_f_cb0[12]=float4(_TEX_Displace_ST.x,_TEX_Displace_ST.y,_TEX_Displace_ST.z,_TEX_Displace_ST.w);
dc_f_cb0[13]=float4(_Displace_STR,0,0,0);
dc_f_cb0[14]=float4(_TEX_2_ST.x,_TEX_2_ST.y,_TEX_2_ST.z,_TEX_2_ST.w);
dc_f_cb0[15]=float4(_TEX_2_Color.x,_TEX_2_Color.y,_TEX_2_Color.z,_TEX_2_Color.w);
dc_f_cb0[16]=float4(_TEX_2_Power,_Mask__UVRot,0,0);
dc_f_cb0[17]=float4(_Mask_Reveal_ST.x,_Mask_Reveal_ST.y,_Mask_Reveal_ST.z,_Mask_Reveal_ST.w);
dc_f_cb0[18]=float4(_Mask_contrastpower_copy,_Mask__UVRot_copy,0,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = v1.xy * dc_f_cb0[7].xy + dc_f_cb0[7].zw;
  r0.xyzw = _Mask_2.Sample(sampler_Mask_2, r0.xy).xyzw;
  r0.x = 1 + -r0.x;
  r0.y = log2(r0.x);
  r0.y = dc_f_cb0[8].x * r0.y;
  r0.y = exp2(r0.y);
  r0.z = 0.0174532924 * dc_f_cb0[16].y;
  sincos(r0.z, r1.x, r2.x);
  r3.z = r1.x;
  r3.y = r2.x;
  r3.x = -r1.x;
  r0.zw = float2(-0.5,-0.5) + v1.xy;
  r1.x = dot(r0.zw, r3.yz);
  r1.y = dot(r0.zw, r3.xy);
  r1.xy = float2(0.5,0.5) + r1.xy;
  r1.xy = r1.xy * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r1.xyzw = _Mask.Sample(sampler_Mask, r1.xy).xyzw;
  r1.x = log2(r1.w);
  r1.x = dc_f_cb0[6].x * r1.x;
  r1.x = exp2(r1.x);
  r1.x = dc_f_cb0[4].x * r1.x;
  r0.y = r1.x * r0.y;
  r1.x = 0.0174532924 * dc_f_cb0[11].x;
  sincos(r1.x, r1.x, r2.x);
  r3.z = r1.x;
  r3.y = r2.x;
  r3.x = -r1.x;
  r1.y = dot(r0.zw, r3.xy);
  r1.x = dot(r0.zw, r3.yz);
  r1.xy = float2(0.5,0.5) + r1.xy;
  r1.xy = r1.xy * dc_f_cb0[10].xy + dc_f_cb0[10].zw;
  r1.xyzw = _Mask_3.Sample(sampler_Mask_3, r1.xy).xyzw;
  r0.y = r1.x * r0.y;
  r1.x = 0.0174532924 * dc_f_cb0[18].y;
  sincos(r1.x, r1.x, r2.x);
  r3.z = r1.x;
  r3.y = r2.x;
  r3.x = -r1.x;
  r1.y = dot(r0.zw, r3.xy);
  r1.x = dot(r0.zw, r3.yz);
  r0.zw = float2(0.5,0.5) + r1.xy;
  r0.zw = r0.zw * dc_f_cb0[17].xy + dc_f_cb0[17].zw;
  r1.xyzw = _Mask_Reveal.Sample(sampler_Mask_Reveal, r0.zw).xyzw;
  r0.z = log2(r1.w);
  r0.z = dc_f_cb0[18].x * r0.z;
  r0.z = exp2(r0.z);
  r0.y = saturate(r0.y * r0.z);
  r0.w = -dc_f_cb1[5].x * dc_f_cb1[5].x;
  r1.xy = v2.xy / v2.ww;
  r1.z = r1.y * r0.w;
  r1.xy = r1.xz * float2(0.5,0.5) + float2(0.5,0.5);
  r0.w = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r2.y = r0.w * dc_f_cb0[4].y + v1.y;
  r2.x = v1.x;
  r1.zw = r2.xy * dc_f_cb0[12].xy + dc_f_cb0[12].zw;
  r3.xyzw = _TEX_Displace.Sample(sampler_TEX_Displace, r1.zw).xyzw;
  r1.zw = dc_f_cb0[13].xx * r3.xy;
  r0.xw = r1.zw * r0.xx;
  r0.xz = r0.xw * r0.zz + r1.xy;
  r1.xyzw = _GrabTexture.Sample(sampler_GrabTexture, r0.xz).xyzw;
  r0.xz = r2.xy * dc_f_cb0[14].xy + dc_f_cb0[14].zw;
  r2.xy = r2.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r2.xyzw = _Texture.Sample(sampler_Texture, r2.xy).xyzw;
  r3.xyzw = _TEX_2.Sample(sampler_TEX_2, r0.xz).xyzw;
  r0.xzw = log2(r3.xyz);
  r0.xzw = dc_f_cb0[16].xxx * r0.xzw;
  r0.xzw = exp2(r0.xzw);
  r0.xzw = dc_f_cb0[15].xyz * r0.xzw;
  r0.xzw = dc_f_cb0[9].xyz * r2.xyz + r0.xzw;
  r0.xzw = r0.xzw + -r1.xyz;
  o0.xyz = r0.yyy * r0.xzw + r1.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
