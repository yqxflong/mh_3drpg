Shader "EBG/Enviro/Misc/Prop"
{
	Properties
	{
		_MainTex("Diffuse Tex", 2D) = "black" {}
		
		_NDotLWrap("N.L Wrap", Float) = 0
		
		[Whitespace] _Whitespace("Normal Map", Float) = 0
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_NormalMapIntensity("Normal Map Itensity", Range(1, 0.1)) = 1
		_NormalMapDamp("Normal Map Dampening", Float) = 0
		
		[Whitespace] _Whitespace("Specular / Emissive", Float) = 0
		_SpecEmissiveTex("Spec/Emissive Map (R - Spec Mask, G - Gloss, B - Emissive)", 2D) = "black" {}
		_SpecularIntensity("Specular Intensity", Float) = 1
		_SpecularGlossModulation("Specular Gloss Modulation", Float) = 1
		_EmissiveColor("Emissive Color", Color) = (0, 0, 0, 1)
		
		[Whitespace] _Whitespace("Reflections", Float) = 0
		_ReflectionColor("Reflection Color", Color) = (0, 0, 0, 1)
		_ReflectionHDR("Reflection HDR", Float) = 0
		
		[Whitespace] _Whitespace("Fresnel", Float) = 0
		_FresnelScale("Fresnel Intensity", Float) = 0
		_FresnelPower("FresnelPower", Float) = 1
		_FresnelColor("FresnelColor", Color) = (1, 1, 1, 1)
		
	} 
	Category
	{
		Tags {
			"Queue"="Geometry"
			"RenderType"="EnvironmentUnmerged"
			"LightMode"="ForwardBase"
		}
		Cull Back
		Lighting Off
		ZWrite On
		ZTest Lequal
		Blend Off
		Fog { Mode Off }
	
		SubShader
		{		
			LOD 100
			
			Pass
			{
				CGPROGRAM  
				
				#undef EBG_POINT_LIGHT
				#undef EGB_TRANSPARENT
				#undef EBG_DETAIL
				#undef EBG_SH_PROBES
				#undef EBG_DIFFUSE_COLOR
				#undef EBG_FOG_ON
				
				#define EBG_VERTEX_LIGHTING_OFF
				#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
				#define EBG_DIFFUSE_OFF
				#define EBG_NORMAL_MAP_ON
				#define EBG_SPEC_ON
				#define EBG_EMISSIVE_ON
				#define EBG_REFLECTIONS_ON
				#define EBG_PLANAR_REFLECTIONS_OFF
				#define EBG_FRESNEL_ON
				#define EBG_DISABLE_LIGHTMAP_OFF
				
				#include "UnityCG.cginc"	
				#include "../../Lib/EBG_Globals.cginc"
				
				#include "../Uber/Lib/EnviroUber.cginc"

				#pragma target 3.0

				#pragma vertex vertex_shader
				#pragma fragment fragment_shader
					
				ENDCG
			}
		}
	}
}

