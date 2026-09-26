Shader "DynamicCards/Legacy/Standard" {
Properties {
_Color("_Color",Color)=(1.0,1.0,1.0,1.0)
_MainTex("_MainTex",2D)="white" {}
_Cutoff("_Cutoff",Float)=0.5
_Glossiness("_Glossiness",Float)=0.5
_GlossMapScale("_GlossMapScale",Float)=1.0
_SmoothnessTextureChannel("_SmoothnessTextureChannel",Float)=0.0
_Metallic("_Metallic",Float)=0.0
_MetallicGlossMap("_MetallicGlossMap",2D)="white" {}
_SpecularHighlights("_SpecularHighlights",Float)=1.0
_GlossyReflections("_GlossyReflections",Float)=1.0
_BumpScale("_BumpScale",Float)=1.0
_BumpMap("_BumpMap",2D)="bump" {}
_Parallax("_Parallax",Float)=0.019999999552965164
_ParallaxMap("_ParallaxMap",2D)="black" {}
_OcclusionStrength("_OcclusionStrength",Float)=1.0
_OcclusionMap("_OcclusionMap",2D)="white" {}
_EmissionColor("_EmissionColor",Color)=(0.0,0.0,0.0,1.0)
_EmissionMap("_EmissionMap",2D)="white" {}
_DetailMask("_DetailMask",2D)="white" {}
_DetailAlbedoMap("_DetailAlbedoMap",2D)="grey" {}
_DetailNormalMapScale("_DetailNormalMapScale",Float)=1.0
_DetailNormalMap("_DetailNormalMap",2D)="bump" {}
_UVSec("_UVSec",Float)=0.0
_Mode("_Mode",Float)=0.0
_SrcBlend("_SrcBlend",Float)=1.0
_DstBlend("_DstBlend",Float)=0.0
_ZWrite("_ZWrite",Float)=1.0
}
SubShader { Tags { "RenderType"="Opaque" } UsePass "Standard/FORWARD" UsePass "Standard/FORWARD_DELTA" } Fallback "Standard"
}