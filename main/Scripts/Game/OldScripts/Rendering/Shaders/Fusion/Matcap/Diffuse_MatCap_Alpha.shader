Shader "Fusion/Matcap/Diffuse Matcap-Alpha" {
	Properties { 
		_Color("Diffuse Color", Color) = (1,1,1,1)
		_RimColor("Rim Color", Color) = (0.746,0.805,0.914,0.336)
		_MainTex("Diffuse Map", 2D) = "white" {}
		_MatcapTex ("MatCap (RGB)", 2D) = "grey" {}
		//_Illum ("Illumin Texture", 2D) = "black" {}
		_MatCapPush("MatCap Push", Float) = 1.5
		_ClipFactor("Clip", float) = 0.0
	}
	
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 300
	
		Pass 
		{
			Name "MATCAP"
			Tags { "LightMode" = "Always" }
			Cull Back
			Fog { Mode Off }

			CGPROGRAM
				#pragma vertex vertmatcap
				#pragma fragment fragmatcap

				#define RIM_ON
				#define BAKE_OFF
				#define HEIGHT_FOG

				#pragma glsl
				#pragma glsl_no_auto_normalization
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma exclude_renderers flash xbox360 ps3
				
				#pragma multi_compile DEATH_OFF DEATH_ON
				#include "SuperMatCapShader.cginc"
			ENDCG
		}
	}

	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200
	
		Pass 
		{
			Name "MATCAP"
			Tags { "LightMode" = "Always" }
			Cull Back
			Fog { Mode Off }

			CGPROGRAM
				#pragma vertex vertmatcap
				#pragma fragment fragmatcap

				#define BAKE_OFF
				#define HEIGHT_FOG

				#pragma glsl
				#pragma glsl_no_auto_normalization
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma exclude_renderers flash xbox360 ps3 d3d11 d3d11_9x
				
				#pragma multi_compile DEATH_OFF DEATH_ON
				#include "SuperMatCapShader.cginc"
			ENDCG
		}
	}

	SubShader { 
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100
	
		Pass 
		{
			Name "MATCAP"
			Tags { "LightMode" = "Always" }
			Cull Back
			Fog { Mode Off }

			CGPROGRAM
				#pragma vertex vertmatcap
				#pragma fragment fragmatcap

				#define BAKE_OFF

				#pragma glsl
				#pragma glsl_no_auto_normalization
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma exclude_renderers flash xbox360 ps3 d3d11 d3d11_9x
				
				#pragma multi_compile DEATH_OFF DEATH_ON
				#include "SuperMatCapShader.cginc"
			ENDCG
		}
	}

	CustomEditor "CustomMaterialEditor"  
}