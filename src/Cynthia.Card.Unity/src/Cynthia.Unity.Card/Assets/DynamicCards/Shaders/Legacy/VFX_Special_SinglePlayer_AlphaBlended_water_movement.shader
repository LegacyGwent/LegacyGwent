Shader "DynamicCards/Legacy/VFX_Special_SinglePlayer_AlphaBlended_water_movement" {
Properties {
_fademask("_fademask",2D)="white" {}
_main_tex("_main_tex",2D)="white" {}
_distorttex("_distorttex",2D)="white" {}
_main_tex_rotate("_main_tex_rotate",Float)=0.0
_colorbrightness("_colorbrightness",Float)=0.0
_noisestrenght("_noisestrenght",Float)=0.27550020813941956
_tex_color("_tex_color",Color)=(0.5,0.5,0.5,1.0)
_rotatedistorttex("_rotatedistorttex",Float)=0.0
_floatspeed("_floatspeed",Float)=0.10000000149011612
_top_tex_noise("_top_tex_noise",2D)="white" {}
_top_tex__color("_top_tex__color",Color)=(0.5,0.5,0.5,1.0)
_alpha_exponent("_alpha_exponent",Float)=0.0
_rotate_top_noise("_rotate_top_noise",Float)=0.0
_top_noise_speed("_top_noise_speed",Float)=0.10000000149011612
_top_noise_contrast("_top_noise_contrast",Float)=0.0
_A("_A",Vector)=(0.0,0.0,0.0,0.0)
_Cutoff("_Cutoff",Float)=0.5
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
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
SamplerState sampler_distorttex;
SamplerState sampler_fademask;
SamplerState sampler_main_tex;
SamplerState sampler_top_tex_noise;
Texture2D<float4> _distorttex;
Texture2D<float4> _fademask;
Texture2D<float4> _main_tex;
Texture2D<float4> _top_tex_noise;
float _alpha_exponent;
float _colorbrightness;
float _floatspeed;
float _main_tex_rotate;
float _noisestrenght;
float _rotate_top_noise;
float _rotatedistorttex;
float _top_noise_contrast;
float _top_noise_speed;
float4 _A;
float4 _TimeEditor;
float4 _distorttex_ST;
float4 _fademask_ST;
float4 _main_tex_ST;
float4 _tex_color;
float4 _top_tex__color;
float4 _top_tex_noise_ST;












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
  uint v2 : SV_IsFrontFace0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[15];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_distorttex_ST.x,_distorttex_ST.y,_distorttex_ST.z,_distorttex_ST.w);
dc_f_cb0[4]=float4(_noisestrenght,0,0,0);
dc_f_cb0[5]=float4(_main_tex_ST.x,_main_tex_ST.y,_main_tex_ST.z,_main_tex_ST.w);
dc_f_cb0[6]=float4(_fademask_ST.x,_fademask_ST.y,_fademask_ST.z,_fademask_ST.w);
dc_f_cb0[7]=float4(_floatspeed,0,0,0);
dc_f_cb0[8]=float4(_top_tex_noise_ST.x,_top_tex_noise_ST.y,_top_tex_noise_ST.z,_top_tex_noise_ST.w);
dc_f_cb0[9]=float4(_top_noise_speed,_top_noise_contrast,0,0);
dc_f_cb0[10]=float4(_tex_color.x,_tex_color.y,_tex_color.z,_tex_color.w);
dc_f_cb0[11]=float4(_top_tex__color.x,_top_tex__color.y,_top_tex__color.z,_top_tex__color.w);
dc_f_cb0[12]=float4(_colorbrightness,_rotate_top_noise,_main_tex_rotate,_alpha_exponent);
dc_f_cb0[13]=float4(_rotatedistorttex,0,0,0);
dc_f_cb0[14]=float4(_A.x,_A.y,_A.z,_A.w);



  float4 r0,r1,r2,r3,r4,r5;
  uint4 bitmask, uiDest;
  float4 fDest;

  sincos(dc_f_cb0[13].x, r0.x, r1.x);
  r2.z = r0.x;
  r2.y = r1.x;
  r2.x = -r0.x;
  r0.xy = float2(-0.5,-0.5) + v1.xy;
  r1.x = dot(r0.xy, r2.yz);
  r1.y = dot(r0.xy, r2.xy);
  r0.zw = float2(0.5,0.5) + r1.xy;
  r1.xy = dc_f_cb1[0].yx + dc_f_cb0[2].yx;
  r0.zw = dc_f_cb0[7].xx * r1.yy + r0.zw;
  r1.x = dc_f_cb0[9].x * r1.x;
  r0.zw = r0.zw * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r2.xyzw = _distorttex.Sample(sampler_distorttex, r0.zw).xyzw;
  sincos(dc_f_cb0[12].z, r3.x, r4.x);
  r5.z = r3.x;
  r5.y = r4.x;
  r5.x = -r3.x;
  r3.y = dot(r0.xy, r5.xy);
  r3.x = dot(r0.xy, r5.yz);
  r3.xy = float2(0.5,0.5) + r3.xy;
  r3.z = dc_f_cb0[4].x * r2.x + r3.y;
  r0.zw = r3.xz * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r2.xyzw = _main_tex.Sample(sampler_main_tex, r0.zw).xyzw;
  r1.yzw = dc_f_cb0[12].xxx + dc_f_cb0[10].xyz;
  r1.yzw = r1.yzw * r2.xyz;
  sincos(dc_f_cb0[12].y, r2.x, r3.x);
  r4.z = r2.x;
  r4.y = r3.x;
  r4.x = -r2.x;
  r2.y = dot(r0.xy, r4.xy);
  r2.x = dot(r0.xy, r4.yz);
  r0.xy = float2(0.5,0.5) + r2.xy;
  r0.zw = dc_f_cb0[14].xx * r0.xy;
  r0.zw = r0.zw * dc_f_cb0[8].xy + dc_f_cb0[8].zw;
  r2.xyzw = _top_tex_noise.Sample(sampler_top_tex_noise, r0.zw).xyzw;
  r3.xyzw = dc_f_cb0[14].yyzz * r1.xxxx + r0.xyxy;
  r0.xy = r1.xx * float2(-0.540000021,-0.25) + r0.xy;
  r0.xy = r0.xy * dc_f_cb0[8].xy + dc_f_cb0[8].zw;
  r0.xyzw = _top_tex_noise.Sample(sampler_top_tex_noise, r0.xy).xyzw;
  r3.xyzw = r3.xyzw * dc_f_cb0[8].xyxy + dc_f_cb0[8].zwzw;
  r4.xyzw = _top_tex_noise.Sample(sampler_top_tex_noise, r3.xy).xyzw;
  r3.xyzw = _top_tex_noise.Sample(sampler_top_tex_noise, r3.zw).xyzw;
  r0.x = r4.y * r2.x;
  r0.x = r0.x * r3.z;
  r0.x = r0.x * r0.w + -dc_f_cb0[12].w;
  r0.x = saturate(dc_f_cb0[9].y * r0.x + dc_f_cb0[12].w);
  o0.xyz = dc_f_cb0[11].xyz * r0.xxx + r1.yzw;
  r0.xy = v1.xy * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r0.xyzw = _fademask.Sample(sampler_fademask, r0.xy).xyzw;
  o0.w = r0.x;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
