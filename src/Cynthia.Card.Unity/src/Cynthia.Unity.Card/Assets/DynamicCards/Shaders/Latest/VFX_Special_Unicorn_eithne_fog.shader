Shader "DynamicCards/Latest/VFX_Special_Unicorn_eithne_fog" {
Properties {
_node_331("_node_331",2D)="white" {}
_multiply("_multiply",Float)=0.20000000298023224
_power("_power",Float)=1.0
_node_2453("_node_2453",Color)=(1.0,1.0,1.0,1.0)
_node_5030("_node_5030",2D)="white" {}
_speed_snow("_speed_snow",Float)=0.33000001311302185
_snow_multiply_colour("_snow_multiply_colour",Float)=0.10000000149011612
_snow_colour("_snow_colour",Color)=(1.0,1.0,1.0,1.0)
_node_2747("_node_2747",Color)=(1.0,1.0,1.0,1.0)
_mask_top("_mask_top",Float)=0.0
_cloud_speed("_cloud_speed",Float)=6.0
_snow_power("_snow_power",Float)=1.0
_snowRotation("_snowRotation",Float)=0.0
_FadeOut("_FadeOut",Float)=0.5
_VertexOffsetStrength("_VertexOffsetStrength",Float)=1.0
_VertexFrequency("_VertexFrequency",Float)=1.0
_VertexSpeed("_VertexSpeed",Float)=1.0
_VertexColor("_VertexColor",Float)=0.0
_UVSwich("_UVSwich",Float)=0.0
_MaskVCa("_MaskVCa",Float)=0.0
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
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
SamplerState sampler_node_331;
SamplerState sampler_node_5030;
Texture2D<float4> _node_331;
Texture2D<float4> _node_5030;
float _FadeOut;
float _MaskVCa;
float _UVSwich;
float _VertexColor;
float _VertexFrequency;
float _VertexOffsetStrength;
float _VertexSpeed;
float _cloud_speed;
float _mask_top;
float _multiply;
float _power;
float _snowRotation;
float _snow_multiply_colour;
float _snow_power;
float _speed_snow;
float4 _TimeEditor;
float4 _node_2453;
float4 _node_2747;
float4 _node_331_ST;
float4 _node_5030_ST;
float4 _snow_colour;
















void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  float4 v2 : COLOR0,
  out float4 o0 : SV_POSITION0,
  out float4 o1 : TEXCOORD0,
  out float4 o2 : TEXCOORD1,
  out float4 o3 : COLOR0)
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
dc_v_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_v_cb0[13];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(0,0,0,0);
dc_v_cb0[6]=float4(0,0,0,0);
dc_v_cb0[7]=float4(0,0,0,0);
dc_v_cb0[8]=float4(0,0,0,0);
dc_v_cb0[9]=float4(0,0,0,0);
dc_v_cb0[10]=float4(0,0,0,0);
dc_v_cb0[11]=float4(0,_VertexOffsetStrength,_VertexFrequency,_VertexSpeed);
dc_v_cb0[12]=float4(_VertexColor,0,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xyz = dc_v_cb2[1].xyz * v0.yyy;
  r0.xyz = dc_v_cb2[0].xyz * v0.xxx + r0.xyz;
  r0.xyz = dc_v_cb2[2].xyz * v0.zzz + r0.xyz;
  r0.xyz = dc_v_cb2[3].xyz * v0.www + r0.xyz;
  r0.xyz = r0.xyz / dc_v_cb0[11].zzz;
  r0.w = dc_v_cb1[0].y + dc_v_cb0[2].y;
  r0.xyz = r0.www * dc_v_cb0[11].www + r0.xyz;
  r0.xyz = sin(r0.xyz);
  r0.xyz = dc_v_cb0[11].yyy * r0.xyz;
  r0.xyz = v2.xyz * r0.xyz;
  r0.xyz = dc_v_cb0[12].xxx * r0.xyz + v0.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r1.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  o2.xyzw = dc_v_cb2[3].xyzw * v0.wwww + r0.xyzw;
  r0.xyzw = dc_v_cb3[18].xyzw * r1.yyyy;
  r0.xyzw = dc_v_cb3[17].xyzw * r1.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb3[19].xyzw * r1.zzzz + r0.xyzw;
  o0.xyzw = dc_v_cb3[20].xyzw * r1.wwww + r0.xyzw;
  o1.xy = v1.xy;
  o3.xyzw = v2.xyzw;
  return;
}




















void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  float4 v3 : COLOR0,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[13];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_node_331_ST.x,_node_331_ST.y,_node_331_ST.z,_node_331_ST.w);
dc_f_cb0[4]=float4(_multiply,_power,0,0);
dc_f_cb0[5]=float4(_node_2453.x,_node_2453.y,_node_2453.z,_node_2453.w);
dc_f_cb0[6]=float4(_node_5030_ST.x,_node_5030_ST.y,_node_5030_ST.z,_node_5030_ST.w);
dc_f_cb0[7]=float4(_speed_snow,_snow_multiply_colour,0,0);
dc_f_cb0[8]=float4(_snow_colour.x,_snow_colour.y,_snow_colour.z,_snow_colour.w);
dc_f_cb0[9]=float4(_node_2747.x,_node_2747.y,_node_2747.z,_node_2747.w);
dc_f_cb0[10]=float4(_mask_top,_cloud_speed,_snow_power,_snowRotation);
dc_f_cb0[11]=float4(_FadeOut,0,0,0);
dc_f_cb0[12]=float4(0,_UVSwich,_MaskVCa,0);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = dc_f_cb1[0].xy + dc_f_cb0[2].xy;
  r0.y = r0.y * dc_f_cb0[7].x + v1.x;
  r1.x = 5 * r0.y;
  r1.y = 2 * v1.y;
  r0.yz = float2(-0.5,-0.5) + r1.xy;
  sincos(dc_f_cb0[10].w, r1.x, r2.x);
  r3.z = r1.x;
  r3.y = r2.x;
  r3.x = -r1.x;
  r1.y = dot(r0.yz, r3.xy);
  r1.x = dot(r0.yz, r3.yz);
  r0.yz = float2(0.5,0.5) + r1.xy;
  r0.yz = r0.yz * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r1.xyzw = _node_5030.Sample(sampler_node_5030, r0.yz).xyzw;
  r0.yzw = log2(r1.xyz);
  r0.yzw = dc_f_cb0[10].zzz * r0.yzw;
  r0.yzw = exp2(r0.yzw);
  r0.yzw = dc_f_cb0[7].yyy * r0.yzw;
  r0.yzw = dc_f_cb0[8].xyz * r0.yzw;
  r1.xy = v1.yx + -v1.xy;
  r1.xy = dc_f_cb0[12].yy * r1.xy + v1.xy;
  r1.z = r0.x * dc_f_cb0[10].y + r1.x;
  r1.xy = r1.yz * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r1.xyzw = _node_331.Sample(sampler_node_331, r1.xy).xyzw;
  r1.xyz = log2(r1.xyz);
  r1.xyz = dc_f_cb0[4].yyy * r1.xyz;
  r1.xyz = exp2(r1.xyz);
  r1.xyz = dc_f_cb0[4].xxx * r1.xyz;
  r0.xyz = r1.xyz * dc_f_cb0[5].xyz + r0.yzw;
  r1.xy = float2(1,1) + -v1.yx;
  r0.w = abs(r1.x) * 6.66666698 + -1;
  r1.x = log2(abs(r1.y));
  r1.x = dc_f_cb0[11].x * r1.x;
  r1.x = exp2(r1.x);
  r1.yz = abs(v1.xy) * float2(1.33333302,6.66666698) + float2(-0.133333296,-1);
  r0.w = r1.z * r0.w;
  r2.xyz = r0.xyz * r0.www;
  r3.xyz = dc_f_cb0[9].xyz + -r1.yyy;
  r1.yzw = dc_f_cb0[10].xxx * r3.xyz + r1.yyy;
  r1.yzw = r1.yzw * r2.xyz;
  r1.xyz = r1.yzw * r1.xxx;
  r0.xyz = r0.xyz * v3.www + -r1.xyz;
  o0.xyz = dc_f_cb0[12].zzz * r0.xyz + r1.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
