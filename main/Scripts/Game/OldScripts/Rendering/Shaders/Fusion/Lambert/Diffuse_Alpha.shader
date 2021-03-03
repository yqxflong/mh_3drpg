Shader "Fusion/Lambert/Diffuse-Alpha" {
	Properties { 
		_Color("Diffuse Color", Color) = (1,1,1,1)
		_MainTex("Diffuse Map", 2D) = "white" {}
		_ClipFactor("Clip", float) = 0.0
	}
	
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200
		Fog { Mode Off }
		CGPROGRAM
		#define HEIGHT_FOG
		
		#pragma multi_compile DEATH_OFF DEATH_ON
		#pragma multi_compile ALPHA_ON ALPHA_OFF
		//#include "../FusionLighting.cginc" 
		#include "CharacterSuperShader.cginc"
		#pragma surface surf Lambert alpha
		
		ENDCG
	} 
	
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100
		Fog { Mode Off }
		CGPROGRAM
		
		#pragma multi_compile DEATH_OFF DEATH_ON
		#pragma multi_compile ALPHA_ON ALPHA_OFF
		//#include "../FusionLighting.cginc" 
		#include "CharacterSuperShader.cginc"
		#pragma surface surf Lambert alpha
		
		ENDCG
	} 
	FallBack "Diffuse"
	//FallBack "Fusion/Lambert/Diffuse"
	//CustomEditor "CustomMaterialEditor"
}
