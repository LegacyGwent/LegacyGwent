Shader "DynamicCards/Latest/VFX_Common_Amplify_VertexAnimGrow" {
Properties {
_TintColor("_TintColor",Color)=(1.0,1.0,1.0,1.0)
_Cutoff("_Cutoff",Float)=1.0
_MainTex("_MainTex",2D)="white" {}
_UV_V_slider("_UV_V_slider",Float)=0.0
_VertexOffset("_VertexOffset",Float)=0.0
_ClampPower("_ClampPower",Float)=0.75
_WavingPower("_WavingPower",Vector)=(0.0,0.0,0.0,0.0)
_WavingSpeed("_WavingSpeed",Float)=0.0
_WaveAmpMin("_WaveAmpMin",Float)=0.0
_WaveAmpMax("_WaveAmpMax",Float)=0.20000000298023224
_UV_V_direction_Switch("_UV_V_direction_Switch",Float)=1.0
_EmissionPower("_EmissionPower",Float)=0.0
_Thick("_Thick",Float)=0.75
_texcoord("_texcoord",2D)="white" {}
__dirty("__dirty",Float)=1.0
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "LIGHTMODE"="FORWARDBASE" "QUEUE"="Transparent" "RenderType"="Transparent"}
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
float _ClampPower;
float _Cutoff;
float _EmissionPower;
float _Thick;
float _UV_V_slider;
float _VertexOffset;
float _WaveAmpMax;
float _WaveAmpMin;
float _WavingSpeed;
float3 _WavingPower;
float4 _TintColor;
float4 _texcoord_ST;






















void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : TANGENT0,
  float3 v2 : NORMAL0,
  float4 v3 : TEXCOORD0,
  float4 v4 : TEXCOORD1,
  float4 v5 : TEXCOORD2,
  float4 v6 : TEXCOORD3,
  float4 v7 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float3 o3 : TEXCOORD2,
  out float4 o4 : COLOR0,
  out float3 o5 : TEXCOORD3)
{
float4 dc_v_cb4[21];
dc_v_cb4[0]=float4(0,0,0,0);
dc_v_cb4[1]=float4(0,0,0,0);
dc_v_cb4[2]=float4(0,0,0,0);
dc_v_cb4[3]=float4(0,0,0,0);
dc_v_cb4[4]=float4(0,0,0,0);
dc_v_cb4[5]=float4(0,0,0,0);
dc_v_cb4[6]=float4(0,0,0,0);
dc_v_cb4[7]=float4(0,0,0,0);
dc_v_cb4[8]=float4(0,0,0,0);
dc_v_cb4[9]=float4(0,0,0,0);
dc_v_cb4[10]=float4(0,0,0,0);
dc_v_cb4[11]=float4(0,0,0,0);
dc_v_cb4[12]=float4(0,0,0,0);
dc_v_cb4[13]=float4(0,0,0,0);
dc_v_cb4[14]=float4(0,0,0,0);
dc_v_cb4[15]=float4(0,0,0,0);
dc_v_cb4[16]=float4(0,0,0,0);
dc_v_cb4[17]=float4(0,0,0,0);
dc_v_cb4[18]=float4(0,0,0,0);
dc_v_cb4[19]=float4(0,0,0,0);
dc_v_cb4[20]=float4(0,0,0,0);
float4 dc_v_cb3[7];
dc_v_cb3[0]=float4(0,0,0,0);
dc_v_cb3[1]=float4(0,0,0,0);
dc_v_cb3[2]=float4(0,0,0,0);
dc_v_cb3[3]=float4(0,0,0,0);
dc_v_cb3[4]=float4(0,0,0,0);
dc_v_cb3[5]=float4(0,0,0,0);
dc_v_cb3[6]=float4(0,0,0,0);
float4 dc_v_cb2[46];
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
dc_v_cb2[17]=float4(0,0,0,0);
dc_v_cb2[18]=float4(0,0,0,0);
dc_v_cb2[19]=float4(0,0,0,0);
dc_v_cb2[20]=float4(0,0,0,0);
dc_v_cb2[21]=float4(0,0,0,0);
dc_v_cb2[22]=float4(0,0,0,0);
dc_v_cb2[23]=float4(0,0,0,0);
dc_v_cb2[24]=float4(0,0,0,0);
dc_v_cb2[25]=float4(0,0,0,0);
dc_v_cb2[26]=float4(0,0,0,0);
dc_v_cb2[27]=float4(0,0,0,0);
dc_v_cb2[28]=float4(0,0,0,0);
dc_v_cb2[29]=float4(0,0,0,0);
dc_v_cb2[30]=float4(0,0,0,0);
dc_v_cb2[31]=float4(0,0,0,0);
dc_v_cb2[32]=float4(0,0,0,0);
dc_v_cb2[33]=float4(0,0,0,0);
dc_v_cb2[34]=float4(0,0,0,0);
dc_v_cb2[35]=float4(0,0,0,0);
dc_v_cb2[36]=float4(0,0,0,0);
dc_v_cb2[37]=float4(0,0,0,0);
dc_v_cb2[38]=float4(0,0,0,0);
dc_v_cb2[39]=float4(0,0,0,0);
dc_v_cb2[40]=float4(0,0,0,0);
dc_v_cb2[41]=float4(0,0,0,0);
dc_v_cb2[42]=float4(0,0,0,0);
dc_v_cb2[43]=float4(0,0,0,0);
dc_v_cb2[44]=float4(0,0,0,0);
dc_v_cb2[45]=float4(0,0,0,0);
float4 dc_v_cb1[1];
dc_v_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_v_cb0[10];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(0,0,0,0);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(_UV_V_slider,_VertexOffset,_Thick,0);
dc_v_cb0[5]=float4(_WavingPower.x,_WavingPower.y,_WavingPower.z,_WavingSpeed);
dc_v_cb0[6]=float4(_WaveAmpMin,_WaveAmpMax,0,0);
dc_v_cb0[7]=float4(0,0,0,0);
dc_v_cb0[8]=float4(0,0,0,0);
dc_v_cb0[9]=float4(_texcoord_ST.x,_texcoord_ST.y,_texcoord_ST.z,_texcoord_ST.w);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = log2(v3.y);
  r0.x = dc_v_cb0[4].z * r0.x;
  r0.x = exp2(r0.x);
  r0.x = min(1, r0.x);
  r0.x = saturate(dc_v_cb0[4].x + r0.x);
  r0.x = 1 + -r0.x;
  r0.y = 0.13333334 * r0.x;
  r0.x = r0.x * -0.266666681 + 3;
  r0.y = r0.y * r0.y;
  r0.x = r0.x * r0.y;
  r0.y = dc_v_cb1[0].y * dc_v_cb0[5].w;
  r0.y = sin(r0.y);
  r0.y = 1 + r0.y;
  r0.z = dc_v_cb0[6].y + -dc_v_cb0[6].x;
  r0.y = r0.y * r0.z;
  r0.y = r0.y * 0.5 + dc_v_cb0[6].x;
  r0.yzw = dc_v_cb0[5].xyz * r0.yyy;
  r0.yzw = v3.yyy * r0.yzw;
  r1.xyz = dc_v_cb0[4].yyy * v2.xyz;
  r0.xyz = r0.xxx * r1.xyz + r0.yzw;
  r0.w = 4 * dc_v_cb0[4].x;
  r1.y = v3.y * 4 + r0.w;
  r1.x = 0.75 + v3.x;
  r1.xyzw = _MainTex.SampleLevel(sampler_MainTex, r1.xy, 0).xyzw;
  r0.xyz = r1.xxx * float3(0.100000001,0.100000001,0.100000001) + r0.xyz;
  r0.xyz = v7.www * r0.xyz + v0.xyz;
  r1.xyzw = dc_v_cb3[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb3[2].xyzw * r0.zzzz + r1.xyzw;
  r1.xyzw = dc_v_cb3[3].xyzw + r0.xyzw;
  o3.xyz = dc_v_cb3[3].xyz * v0.www + r0.xyz;
  r0.xyzw = dc_v_cb4[18].xyzw * r1.yyyy;
  r0.xyzw = dc_v_cb4[17].xyzw * r1.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb4[19].xyzw * r1.zzzz + r0.xyzw;
  o0.xyzw = dc_v_cb4[20].xyzw * r1.wwww + r0.xyzw;
  o1.xy = v3.xy * dc_v_cb0[9].xy + dc_v_cb0[9].zw;
  r0.x = dot(v2.xyz, dc_v_cb3[4].xyz);
  r0.y = dot(v2.xyz, dc_v_cb3[5].xyz);
  r0.z = dot(v2.xyz, dc_v_cb3[6].xyz);
  r0.w = dot(r0.xyz, r0.xyz);
  r0.w = rsqrt(r0.w);
  r0.xyz = r0.xyz * r0.www;
  o2.xyz = r0.xyz;
  o4.xyzw = v7.xyzw;
  r1.x = r0.y * r0.y;
  r1.x = r0.x * r0.x + -r1.x;
  r2.xyzw = r0.xyzz * r0.yzzx;
  r3.x = dot(dc_v_cb2[42].xyzw, r2.xyzw);
  r3.y = dot(dc_v_cb2[43].xyzw, r2.xyzw);
  r3.z = dot(dc_v_cb2[44].xyzw, r2.xyzw);
  r1.xyz = dc_v_cb2[45].xyz * r1.xxx + r3.xyz;
  r0.w = 1;
  r2.x = dot(dc_v_cb2[39].xyzw, r0.xyzw);
  r2.y = dot(dc_v_cb2[40].xyzw, r0.xyzw);
  r2.z = dot(dc_v_cb2[41].xyzw, r0.xyzw);
  r0.xyz = r2.xyz + r1.xyz;
  r0.xyz = max(float3(0,0,0), r0.xyz);
  r0.xyz = log2(r0.xyz);
  r0.xyz = float3(0.416666657,0.416666657,0.416666657) * r0.xyz;
  r0.xyz = exp2(r0.xyz);
  r0.xyz = r0.xyz * float3(1.05499995,1.05499995,1.05499995) + float3(-0.0549999997,-0.0549999997,-0.0549999997);
  o5.xyz = max(float3(0,0,0), r0.xyz);
  return;
}














void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float3 v3 : TEXCOORD2,
  float4 v4 : COLOR0,
  float3 v5 : TEXCOORD3,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb0[9];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(0,0,0,0);
dc_f_cb0[3]=float4(0,0,0,0);
dc_f_cb0[4]=float4(_UV_V_slider,0,_Thick,0);
dc_f_cb0[5]=float4(0,0,0,0);
dc_f_cb0[6]=float4(0,0,0,0);
dc_f_cb0[7]=float4(_TintColor.x,_TintColor.y,_TintColor.z,_TintColor.w);
dc_f_cb0[8]=float4(_EmissionPower,_ClampPower,_Cutoff,0);



  float4 r0;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = log2(v1.y);
  r0.x = dc_f_cb0[4].z * r0.x;
  r0.x = exp2(r0.x);
  r0.x = min(1, r0.x);
  r0.x = saturate(dc_f_cb0[4].x + r0.x);
  r0.y = 1 + -dc_f_cb0[8].y;
  r0.x = r0.x * r0.y + dc_f_cb0[8].y;
  r0.y = 100 * v4.w;
  r0.x = saturate(r0.y * r0.x);
  r0.x = -dc_f_cb0[8].z + r0.x;
  r0.x = cmp(r0.x < 0);
  if (r0.x != 0) discard;
  r0.x = 4 * dc_f_cb0[4].x;
  r0.y = v1.y * 4 + r0.x;
  r0.x = 0.75 + v1.x;
  r0.xyzw = _MainTex.Sample(sampler_MainTex, r0.xy).xyzw;
  r0.xyzw = dc_f_cb0[7].xyzw * r0.xyzw;
  r0.xyz = v4.xyz * r0.xyz;
  o0.w = v4.w * r0.w;
  o0.xyz = dc_f_cb0[8].xxx * r0.xyz;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
