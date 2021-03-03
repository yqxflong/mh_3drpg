Shader "Fusion/Lambert/Diffuse" {
	Properties { 
		_Color("Diffuse Color", Color) = (1,1,1,1)
		_MainTex("Diffuse Map", 2D) = "white" {}
		_ClipFactor("Clip", float) = 0.0
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" } 
		LOD 200
		Fog { Mode Off }
		CGPROGRAM
		#define HEIGHT_FOG
		#pragma multi_compile DEATH_OFF DEATH_ON
		//#include "../FusionLighting.cginc" 
		#include "CharacterSuperShader.cginc"
		#pragma surface surf Lambert
		
		ENDCG
	} 
	
	SubShader {
		Tags { "RenderType"="Opaque" } 
		LOD 100
		Fog { Mode Off }
		CGPROGRAM
		
		#pragma multi_compile DEATH_OFF DEATH_ON
		//#include "../FusionLighting.cginc"
		#include "CharacterSuperShader.cginc"
		#pragma surface surf Lambert
		
		ENDCG
	} 
	FallBack "Diffuse"
	//FallBack "Fusion/Lambert/Diffuse"
	//CustomEditor "CustomMaterialEditor"
}
