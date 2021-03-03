Shader "Fusion/Lambert/1Dye" {
	Properties { 
		_Color("Diffuse Color", Color) = (1,1,1,1)
		_MainTex("Diffuse Map", 2D) = "white" {}
		_DyeMaskMap("Dye Mask Map", 2D) = "black" {} 
		_PrimaryGradient("Primary Gradient Map", 2D) = "white" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Fog { Mode Off }
		CGPROGRAM
		#define DYE1
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
		#define DYE1
		
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
