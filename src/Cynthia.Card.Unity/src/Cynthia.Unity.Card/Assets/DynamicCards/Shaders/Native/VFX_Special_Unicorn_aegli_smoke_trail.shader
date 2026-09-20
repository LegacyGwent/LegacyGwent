Shader "DynamicCards/Native/VFX_Special_Unicorn_aegli_smoke_trail" {
Properties {
_coulor("_coulor",Color)=(0.5,0.5,0.5,1.0)
_texture("_texture",2D)="white" {}
_gradient("_gradient",2D)="white" {}
_node_283("_node_283",2D)="white" {}
_turbulence("_turbulence",2D)="white" {}
_speed_noise1("_speed_noise1",Float)=-0.009999999776482582
_speed_noise2("_speed_noise2",Float)=-0.009999999776482582
_speed_noise3("_speed_noise3",Float)=0.30000001192092896
_brightness("_brightness",Float)=1.5
_vertex_offset_multiply("_vertex_offset_multiply",Float)=3.0
_vertex_speed("_vertex_speed",Float)=0.009999999776482582
}
SubShader {
Tags {"IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Off
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
SamplerState sampler_gradient;
SamplerState sampler_node_283;
SamplerState sampler_texture;
SamplerState sampler_turbulence;
Texture2D<float4> _gradient;
Texture2D<float4> _node_283;
Texture2D<float4> _texture;
Texture2D<float4> _turbulence;
float _brightness;
float _speed_noise1;
float _speed_noise2;
float _speed_noise3;
float _vertex_offset_multiply;
float _vertex_speed;
float4 _TimeEditor;
float4 _coulor;
float4 _gradient_ST;
float4 _node_283_ST;
float4 _texture_ST;
float4 _turbulence_ST;




















void dc_v(
  float4 v0 : POSITION0,
  float2 v1 : TEXCOORD0,
  out float4 o0 : SV_POSITION0,
  out float2 o1 : TEXCOORD0)
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
float4 dc_v_cb0[10];
dc_v_cb0[0]=float4(0,0,0,0);
dc_v_cb0[1]=float4(0,0,0,0);
dc_v_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_v_cb0[3]=float4(0,0,0,0);
dc_v_cb0[4]=float4(0,0,0,0);
dc_v_cb0[5]=float4(0,0,0,0);
dc_v_cb0[6]=float4(0,0,0,0);
dc_v_cb0[7]=float4(_turbulence_ST.x,_turbulence_ST.y,_turbulence_ST.z,_turbulence_ST.w);
dc_v_cb0[8]=float4(0,0,0,0);
dc_v_cb0[9]=float4(_vertex_offset_multiply,_vertex_speed,0,0);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = dc_v_cb1[0].y + dc_v_cb0[2].y;
  r0.x = r0.x * dc_v_cb0[9].y + v1.y;
  r0.y = dc_v_cb0[7].y * r0.x;
  r0.x = dc_v_cb0[7].x * v1.x;
  r0.xy = r0.xy * float2(12,12) + dc_v_cb0[7].zw;
  r0.xyzw = _turbulence.SampleLevel(sampler_turbulence, r0.xy, 0).xyzw;
  r0.xyz = r0.xyz * float3(2,2,2) + float3(-1,-1,-1);
  r0.w = abs(v1.y) * 12.8000002 + 0.280000001;
  r0.w = min(1, r0.w);
  r0.xyz = r0.www * r0.xyz;
  r0.xyz = r0.xyz * dc_v_cb0[9].xxx + v0.xyz;
  r1.xyzw = dc_v_cb2[1].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb2[0].xyzw * r0.xxxx + r1.xyzw;
  r0.xyzw = dc_v_cb2[2].xyzw * r0.zzzz + r1.xyzw;
  r0.xyzw = dc_v_cb2[3].xyzw + r0.xyzw;
  r1.xyzw = dc_v_cb3[18].xyzw * r0.yyyy;
  r1.xyzw = dc_v_cb3[17].xyzw * r0.xxxx + r1.xyzw;
  r1.xyzw = dc_v_cb3[19].xyzw * r0.zzzz + r1.xyzw;
  o0.xyzw = dc_v_cb3[20].xyzw * r0.wwww + r1.xyzw;
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
float4 dc_f_cb0[9];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_coulor.x,_coulor.y,_coulor.z,_coulor.w);
dc_f_cb0[4]=float4(_texture_ST.x,_texture_ST.y,_texture_ST.z,_texture_ST.w);
dc_f_cb0[5]=float4(_gradient_ST.x,_gradient_ST.y,_gradient_ST.z,_gradient_ST.w);
dc_f_cb0[6]=float4(_node_283_ST.x,_node_283_ST.y,_node_283_ST.z,_node_283_ST.w);
dc_f_cb0[7]=float4(0,0,0,0);
dc_f_cb0[8]=float4(_speed_noise1,_speed_noise2,_speed_noise3,_brightness);



  float4 r0,r1;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.x = v1.x;
  r0.x = dc_f_cb0[4].x * r0.x;
  r0.z = dc_f_cb1[0].y + dc_f_cb0[2].y;
  r1.xyz = dc_f_cb0[8].zyx * r0.zzz + v1.xyy;
  r0.z = dc_f_cb0[4].y * r1.z;
  r1.xy = dc_f_cb0[6].xy * r1.xy;
  r1.xy = r1.xy * float2(0.5,11) + dc_f_cb0[6].zw;
  r1.xyzw = _node_283.Sample(sampler_node_283, r1.xy).xyzw;
  r0.y = 22 * r0.z;
  r0.xy = dc_f_cb0[4].zw + r0.xy;
  r0.xyzw = _texture.Sample(sampler_texture, r0.xy).xyzw;
  r0.xyz = r1.xyz * r0.xyz;
  r0.xyz = dc_f_cb0[3].xyz * r0.xyz;
  r1.xy = v1.xy * dc_f_cb0[5].xy + dc_f_cb0[5].zw;
  r1.xyzw = _gradient.Sample(sampler_gradient, r1.xy).xyzw;
  r0.xyz = r1.xyz * r0.xyz;
  r0.xyz = dc_f_cb0[8].www * r0.xyz;
  r0.w = 1 + -v1.y;
  o0.xyz = abs(r0.www) * r0.xyz;
  o0.w = 1;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
