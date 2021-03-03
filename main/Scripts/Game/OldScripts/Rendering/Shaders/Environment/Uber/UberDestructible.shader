Shader "EBG/Enviro/Uber/UberDestructible"
{

	Properties
	{
		_MainTex("Diffuse Tex", 2D) = "black" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		
		[Whitespace] _Whitespace("Diffuse", Float) = 0
		_NDotLWrap("N.L Wrap", Float) = 0
		
		[Whitespace] _Whitespace("SH Probes", Float) = 0
		_SHIntensity("Intensity", Float) = 1
		
		[Whitespace] _Whitespace("Normal Map", Float) = 0
		[MaterialToggle] EBG_NORMAL_MAP ("Enable", Float) = 0
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_NormalMapIntensity("Normal Map Itensity", Range(1, 0.1)) = 1
		_NormalMapDamp("Normal Map Dampening", Float) = 0
		
		[Whitespace] _Whitespace("Specular / Emissive", Float) = 0
		_SpecEmissiveTex("Spec/Emissive Map (R - Spec Mask, G - Gloss, B - Emissive)", 2D) = "black" {}
		[MaterialToggle] EBG_SPEC ("Enable Specular", Float) = 0
		_SpecularIntensity("Specular Intensity", Float) = 1
		_SpecularGlossModulation("Specular Gloss Modulation", Float) = 1
		[MaterialToggle] EBG_EMISSIVE ("Enable Emissive", Float) = 0
		_EmissiveColor("Emissive Color", Color) = (0, 0, 0, 1)
		
		[Whitespace] _Whitespace("Reflections", Float) = 0
		[MaterialToggle] EBG_REFLECTIONS ("Enable", Float) = 0
		_ReflectionColor("Reflection Color", Color) = (0, 0, 0, 1)
		_ReflectionHDR("Reflection HDR", Float) = 0
		
		[Whitespace] _Whitespace("Fresnel", Float) = 0
		[MaterialToggle] EBG_FRESNEL ("Enable", Float) = 0
		_FresnelScale("Fresnel Intensity", Float) = 0
		_FresnelPower("FresnelPower", Float) = 1
		_FresnelColor("FresnelColor", Color) = (1, 1, 1, 1)
	} 
	Category
	{
		Tags {
			"Queue"="Geometry"
			"RenderType"="Environment"
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
				
				#define EBG_POINT_LIGHT
				#undef EGB_TRANSPARENT
				#undef EBG_DETAIL
				#define EBG_DIFFUSE_COLOR
				#define EBG_SH_PROBES
				#define EBG_FOG_ON
				
				#define EBG_DIFFUSE_ON
				#pragma multi_compile EBG_NORMAL_MAP_OFF EBG_NORMAL_MAP_ON
				#pragma multi_compile EBG_SPEC_OFF EBG_SPEC_ON
				#pragma multi_compile EBG_EMISSIVE_OFF EBG_EMISSIVE_ON
				#pragma multi_compile EBG_REFLECTIONS_OFF EBG_REFLECTIONS_ON
				#define EBG_PLANAR_REFLECTIONS_OFF
				#pragma multi_compile EBG_FRESNEL_OFF EBG_FRESNEL_ON
				#define EBG_DISABLE_LIGHTMAP_ON
				
				#include "UnityCG.cginc"	
				#include "../../Lib/EBG_Globals.cginc"
				
				#include "Lib/EnviroUber.cginc"

				#pragma target 3.0

				#pragma vertex vertex_shader
				#pragma fragment fragment_shader
					
				ENDCG
			}
		}
	}
}

