Shader "DynamicCards/Legacy/_GeraltSwordmaster_Blood_Front_Shader" {
Properties {
_node_1759("_node_1759",2D)="white" {}
_Color_1759("_Color_1759",Color)=(0.5,0.5,0.5,1.0)
_Detail_Texture("_Detail_Texture",2D)="white" {}
_Detail_color("_Detail_color",Color)=(0.5,0.5,0.5,1.0)
_displace_STR("_displace_STR",Float)=0.009999999776482582
_node_7946("_node_7946",2D)="white" {}
_Color_7946("_Color_7946",Color)=(0.5,0.5,0.5,1.0)
_UV_Time_Mult_copy("_UV_Time_Mult_copy",Float)=0.0
_Displace_Textre("_Displace_Textre",2D)="bump" {}
_Displace_Time_Mult("_Displace_Time_Mult",Float)=0.0
_Mask("_Mask",2D)="white" {}
_MaskRotation("_MaskRotation",Float)=0.0
_Mask_contrast("_Mask_contrast",Float)=0.0
_OVERALL_OPACITY("_OVERALL_OPACITY",Float)=1.0
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
SamplerState sampler_Detail_Texture;
SamplerState sampler_Displace_Textre;
SamplerState sampler_Mask;
SamplerState sampler_node_1759;
SamplerState sampler_node_7946;
Texture2D<float4> _Detail_Texture;
Texture2D<float4> _Displace_Textre;
Texture2D<float4> _Mask;
Texture2D<float4> _node_1759;
Texture2D<float4> _node_7946;
float _Displace_Time_Mult;
float _MaskRotation;
float _Mask_contrast;
float _OVERALL_OPACITY;
float _UV_Time_Mult_copy;
float _displace_STR;
float4 _Color_1759;
float4 _Color_7946;
float4 _Detail_Texture_ST;
float4 _Detail_color;
float4 _Displace_Textre_ST;
float4 _Mask_ST;
float4 _TimeEditor;
float4 _node_1759_ST;
float4 _node_7946_ST;












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
  r1.xyzw = dc_v_cb0[3].xyzw + r0.xyzw;
  o2.xyzw = dc_v_cb0[3].xyzw * v0.wwww + r0.xyzw;
  r0.xyzw = dc_v_cb1[18].xyzw * r1.yyyy;
  r0.xyzw = dc_v_cb1[17].xyzw * r1.xxxx + r0.xyzw;
  r0.xyzw = dc_v_cb1[19].xyzw * r1.zzzz + r0.xyzw;
  o0.xyzw = dc_v_cb1[20].xyzw * r1.wwww + r0.xyzw;
  o1.xy = v1.xy;
  return;
}
































void dc_f(
  float4 v0 : SV_POSITION0,
  float4 v1 : TEXCOORD0,
  float4 v2 : TEXCOORD1,
  out float4 o0 : SV_Target0)
{
float4 dc_f_cb1[1];
dc_f_cb1[0]=float4(_Time.x,_Time.y,_Time.z,_Time.w);
float4 dc_f_cb0[15];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_TimeEditor.x,_TimeEditor.y,_TimeEditor.z,_TimeEditor.w);
dc_f_cb0[3]=float4(_node_1759_ST.x,_node_1759_ST.y,_node_1759_ST.z,_node_1759_ST.w);
dc_f_cb0[4]=float4(_Displace_Textre_ST.x,_Displace_Textre_ST.y,_Displace_Textre_ST.z,_Displace_Textre_ST.w);
dc_f_cb0[5]=float4(_displace_STR,_Displace_Time_Mult,0,0);
dc_f_cb0[6]=float4(_node_7946_ST.x,_node_7946_ST.y,_node_7946_ST.z,_node_7946_ST.w);
dc_f_cb0[7]=float4(_UV_Time_Mult_copy,0,0,0);
dc_f_cb0[8]=float4(_Detail_Texture_ST.x,_Detail_Texture_ST.y,_Detail_Texture_ST.z,_Detail_Texture_ST.w);
dc_f_cb0[9]=float4(_Color_7946.x,_Color_7946.y,_Color_7946.z,_Color_7946.w);
dc_f_cb0[10]=float4(_Mask_ST.x,_Mask_ST.y,_Mask_ST.z,_Mask_ST.w);
dc_f_cb0[11]=float4(_Color_1759.x,_Color_1759.y,_Color_1759.z,_Color_1759.w);
dc_f_cb0[12]=float4(_MaskRotation,_Mask_contrast,0,0);
dc_f_cb0[13]=float4(_Detail_color.x,_Detail_color.y,_Detail_color.z,_Detail_color.w);
dc_f_cb0[14]=float4(_OVERALL_OPACITY,0,0,0);



  float4 r0,r1,r2;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = dc_f_cb1[0].xy + dc_f_cb0[2].xy;
  r0.z = dc_f_cb0[5].y * r0.y;
  r1.x = r0.z * 0.5 + v2.y;
  r1.y = r0.y * dc_f_cb0[5].y + v2.x;
  r0.yz = r1.xy * dc_f_cb0[4].xy + dc_f_cb0[4].zw;
  r1.xyzw = _Displace_Textre.Sample(sampler_Displace_Textre, r0.yz).xyzw;
  r0.yz = r1.wy * float2(2,2) + float2(-1,-1);
  r1.xz = dc_f_cb0[5].xx * r0.yz;
  r1.y = v1.y;
  r0.yz = v1.xy + r1.xy;
  r1.x = dc_f_cb0[7].x * r0.x + v1.x;
  r0.x = dc_f_cb0[7].x * r0.x;
  r2.x = r0.x * 0.25 + v1.x;
  r0.xy = r1.xz + r0.yz;
  r0.xy = r0.xy * dc_f_cb0[3].xy + dc_f_cb0[3].zw;
  r0.xyzw = _node_1759.Sample(sampler_node_1759, r0.xy).xyzw;
  r0.xyz = dc_f_cb0[11].xyz + r0.xyz;
  r2.y = v1.y;
  r1.zw = r2.xy * dc_f_cb0[8].xy + dc_f_cb0[8].zw;
  r2.xyzw = _Detail_Texture.Sample(sampler_Detail_Texture, r1.zw).xyzw;
  r2.xyz = dc_f_cb0[13].xyz + r2.xyz;
  r1.y = v1.y;
  r1.xy = r1.xy * dc_f_cb0[6].xy + dc_f_cb0[6].zw;
  r1.xyzw = _node_7946.Sample(sampler_node_7946, r1.xy).xyzw;
  r1.xyz = dc_f_cb0[9].xyz + r1.xyz;
  r0.w = r1.w + r0.w;
  r1.xyz = r1.xyz * r2.xyz;
  o0.xyz = r1.xyz * r0.xyz;
  r0.x = 0.0174532924 * dc_f_cb0[12].x;
  sincos(r0.x, r0.x, r1.x);
  r2.z = r0.x;
  r0.yz = float2(-0.5,-0.5) + v1.xy;
  r2.y = r1.x;
  r2.x = -r0.x;
  r1.y = dot(r0.yz, r2.xy);
  r1.x = dot(r0.yz, r2.yz);
  r0.xy = float2(0.5,0.5) + r1.xy;
  r0.xy = r0.xy * dc_f_cb0[10].xy + dc_f_cb0[10].zw;
  r1.xyzw = _Mask.Sample(sampler_Mask, r0.xy).xyzw;
  r0.x = log2(r1.x);
  r0.x = dc_f_cb0[12].y * r0.x;
  r0.x = exp2(r0.x);
  r0.x = r0.w * r0.x;
  o0.w = dc_f_cb0[14].x * r0.x;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
