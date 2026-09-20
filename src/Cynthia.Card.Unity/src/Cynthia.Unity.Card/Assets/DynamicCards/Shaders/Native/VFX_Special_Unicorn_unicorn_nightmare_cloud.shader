Shader "DynamicCards/Native/VFX_Special_Unicorn_unicorn_nightmare_cloud" {
Properties {
_node_8719("_node_8719",2D)="white" {}
_node_4056("_node_4056",Color)=(0.5,0.5,0.5,1.0)
_cloud_speed("_cloud_speed",Float)=-0.019999999552965164
_speed_2("_speed_2",Float)=0.009999999776482582
_alpcha("_alpcha",Float)=0.75
_mask_multiply("_mask_multiply",Float)=2.0
_brightness("_brightness",Float)=1.0
_opacity_power("_opacity_power",Float)=1.0
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
SamplerState sampler_node_8719;
Texture2D<float4> _node_8719;
float _alpcha;
float _brightness;
float _cloud_speed;
float _mask_multiply;
float _opacity_power;
float _speed_2;
float4 _node_4056;
float4 _node_8719_ST;












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
float4 dc_f_cb0[6];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_node_8719_ST.x,_node_8719_ST.y,_node_8719_ST.z,_node_8719_ST.w);
dc_f_cb0[3]=float4(_node_4056.x,_node_4056.y,_node_4056.z,_node_4056.w);
dc_f_cb0[4]=float4(_cloud_speed,_speed_2,_alpcha,_mask_multiply);
dc_f_cb0[5]=float4(_brightness,_opacity_power,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = 3 * abs(v1.y);
  r0.x = min(1, r0.x);
  r0.yz = dc_f_cb1[0].yy * dc_f_cb0[4].yx + v1.xy;
  r0.yz = r0.yz * dc_f_cb0[2].xy + dc_f_cb0[2].zw;
  r1.xyzw = _node_8719.Sample(sampler_node_8719, r0.yz).xyzw;
  r0.x = r1.w * r0.x;
  r0.x = dc_f_cb0[4].z * r0.x;
  r0.x = log2(r0.x);
  r0.x = dc_f_cb0[5].y * r0.x;
  o0.w = exp2(r0.x);
  r0.x = 1 + -v1.y;
  r0.x = min(1, abs(r0.x));
  r0.x = dc_f_cb0[4].w * r0.x;
  r0.xyz = r1.xyz * r0.xxx;
  r0.xyz = dc_f_cb0[3].xyz * r0.xyz;
  o0.xyz = dc_f_cb0[5].xxx * r0.xyz;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
