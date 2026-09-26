Shader "DynamicCards/Latest/Hidden_ShaderLibrary_UI_Specific_BarsVisualization" {
Properties {
_MainBot("_MainBot",Color)=(0.0,0.0,0.0,1.0)
_MainTop("_MainTop",Color)=(0.0,0.0,0.0,1.0)
_HighLight("_HighLight",Color)=(0.0,0.0,0.0,1.0)
_GhostColor("_GhostColor",Color)=(0.0,0.0,0.0,1.0)
_MirrorMul("_MirrorMul",Float)=1.0
_MirrorAdd("_MirrorAdd",Float)=0.0
}
SubShader {
Tags {"QUEUE"="Transparent" "RenderType"="Transparent"}
Pass {
Cull Back
ZWrite On
ZTest LEqual
Blend One OneMinusSrcAlpha
CGPROGRAM
#pragma target 3.5
#pragma vertex dc_v
#pragma fragment dc_f
#include "UnityCG.cginc"
#include "Lighting.cginc"
#define cmp -
float _MirrorAdd;
float _MirrorMul;
float4 _Data[32];
float4 _GhostColor;
float4 _HighLight;
float4 _MainBot;
float4 _MainTop;












void dc_v(
  float4 v0 : POSITION0,
  float4 v1 : TEXCOORD0,
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
float4 dc_f_cb0[39];
dc_f_cb0[0]=float4(0,0,0,0);
dc_f_cb0[1]=float4(0,0,0,0);
dc_f_cb0[2]=float4(_MainBot.x,_MainBot.y,_MainBot.z,_MainBot.w);
dc_f_cb0[3]=float4(_MainTop.x,_MainTop.y,_MainTop.z,_MainTop.w);
dc_f_cb0[4]=float4(_HighLight.x,_HighLight.y,_HighLight.z,_HighLight.w);
dc_f_cb0[5]=float4(_GhostColor.x,_GhostColor.y,_GhostColor.z,_GhostColor.w);
dc_f_cb0[6]=float4(_MirrorMul,_MirrorAdd,0,0);
dc_f_cb0[7]=float4(_Data[0].x,_Data[0].y,_Data[0].z,_Data[0].w);
dc_f_cb0[8]=float4(_Data[1].x,_Data[1].y,_Data[1].z,_Data[1].w);
dc_f_cb0[9]=float4(_Data[2].x,_Data[2].y,_Data[2].z,_Data[2].w);
dc_f_cb0[10]=float4(_Data[3].x,_Data[3].y,_Data[3].z,_Data[3].w);
dc_f_cb0[11]=float4(_Data[4].x,_Data[4].y,_Data[4].z,_Data[4].w);
dc_f_cb0[12]=float4(_Data[5].x,_Data[5].y,_Data[5].z,_Data[5].w);
dc_f_cb0[13]=float4(_Data[6].x,_Data[6].y,_Data[6].z,_Data[6].w);
dc_f_cb0[14]=float4(_Data[7].x,_Data[7].y,_Data[7].z,_Data[7].w);
dc_f_cb0[15]=float4(_Data[8].x,_Data[8].y,_Data[8].z,_Data[8].w);
dc_f_cb0[16]=float4(_Data[9].x,_Data[9].y,_Data[9].z,_Data[9].w);
dc_f_cb0[17]=float4(_Data[10].x,_Data[10].y,_Data[10].z,_Data[10].w);
dc_f_cb0[18]=float4(_Data[11].x,_Data[11].y,_Data[11].z,_Data[11].w);
dc_f_cb0[19]=float4(_Data[12].x,_Data[12].y,_Data[12].z,_Data[12].w);
dc_f_cb0[20]=float4(_Data[13].x,_Data[13].y,_Data[13].z,_Data[13].w);
dc_f_cb0[21]=float4(_Data[14].x,_Data[14].y,_Data[14].z,_Data[14].w);
dc_f_cb0[22]=float4(_Data[15].x,_Data[15].y,_Data[15].z,_Data[15].w);
dc_f_cb0[23]=float4(_Data[16].x,_Data[16].y,_Data[16].z,_Data[16].w);
dc_f_cb0[24]=float4(_Data[17].x,_Data[17].y,_Data[17].z,_Data[17].w);
dc_f_cb0[25]=float4(_Data[18].x,_Data[18].y,_Data[18].z,_Data[18].w);
dc_f_cb0[26]=float4(_Data[19].x,_Data[19].y,_Data[19].z,_Data[19].w);
dc_f_cb0[27]=float4(_Data[20].x,_Data[20].y,_Data[20].z,_Data[20].w);
dc_f_cb0[28]=float4(_Data[21].x,_Data[21].y,_Data[21].z,_Data[21].w);
dc_f_cb0[29]=float4(_Data[22].x,_Data[22].y,_Data[22].z,_Data[22].w);
dc_f_cb0[30]=float4(_Data[23].x,_Data[23].y,_Data[23].z,_Data[23].w);
dc_f_cb0[31]=float4(_Data[24].x,_Data[24].y,_Data[24].z,_Data[24].w);
dc_f_cb0[32]=float4(_Data[25].x,_Data[25].y,_Data[25].z,_Data[25].w);
dc_f_cb0[33]=float4(_Data[26].x,_Data[26].y,_Data[26].z,_Data[26].w);
dc_f_cb0[34]=float4(_Data[27].x,_Data[27].y,_Data[27].z,_Data[27].w);
dc_f_cb0[35]=float4(_Data[28].x,_Data[28].y,_Data[28].z,_Data[28].w);
dc_f_cb0[36]=float4(_Data[29].x,_Data[29].y,_Data[29].z,_Data[29].w);
dc_f_cb0[37]=float4(_Data[30].x,_Data[30].y,_Data[30].z,_Data[30].w);
dc_f_cb0[38]=float4(_Data[31].x,_Data[31].y,_Data[31].z,_Data[31].w);



  float4 r0,r1,r2,r3;
  uint4 bitmask, uiDest;
  float4 fDest;

  r0.xy = float2(32,32) * v1.xy;
  r0.z = (int)r0.x;
  r0.w = ceil(dc_f_cb0[r0.z+7].x);
  r0.y = min(r0.y, r0.w);
  r1.xyz = frac(r0.xyx);
  r0.xyw = r1.xyz * float3(2,2,2) + float3(-1,-1,-1);
  r0.xyw = float3(-0.650000036,-0.650000036,-0.849999964) + abs(r0.xyw);
  r0.xyw = max(float3(0,0,0), r0.xyw);
  r0.w = -r0.w * 10 + 1;
  r0.x = dot(r0.xy, r0.xy);
  r0.x = sqrt(r0.x);
  r0.x = -r0.x * 5 + 1;
  r0.xy = max(float2(0,0), r0.xw);
  r1.xyzw = dc_f_cb0[5].xyzw * r0.yyyy;
  r2.xyzw = dc_f_cb0[3].xyzw + -dc_f_cb0[2].xyzw;
  r2.xyzw = v1.yyyy * r2.xyzw + dc_f_cb0[2].xyzw;
  r3.xyzw = dc_f_cb0[4].xyzw + -r2.xyzw;
  r0.y = 1 + -v1.y;
  r0.w = dc_f_cb0[r0.z+7].w * r0.y;
  r0.y = saturate(r0.y * dc_f_cb0[6].x + dc_f_cb0[6].y);
  r2.xyzw = r0.wwww * r3.xyzw + r2.xyzw;
  r0.w = v1.y * 32 + -dc_f_cb0[r0.z+7].y;
  r0.w = 1 + -r0.w;
  r0.z = saturate(r0.w / dc_f_cb0[r0.z+7].z);
  r2.xyzw = -r1.xyzw * r0.zzzz + r2.xyzw;
  r1.xyzw = r1.xyzw * r0.zzzz;
  r1.xyzw = r0.xxxx * r2.xyzw + r1.xyzw;
  o0.xyzw = r1.xyzw * r0.yyyy;
  return;
}
ENDCG
}
}
Fallback "DynamicCards/PortableCard"
}
