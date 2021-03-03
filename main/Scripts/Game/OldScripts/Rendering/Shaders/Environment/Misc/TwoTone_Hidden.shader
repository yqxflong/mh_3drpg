Shader "Hidden/Enviro/Misc/TwoTone"
{
	Properties
	{
		_MainTex("Diffuse Tex (R - Grayscale / G - Mask)", 2D) = "black" {}
		_DiffuseColor0("Diffuse Color 0", Color) = (1, 1, 1, 1)
		_DiffuseColor1("Diffuse Color 1", Color) = (1, 1, 1, 1)
		
		_NDotLWrap("N.L Wrap", Float) = 0
		
		[Whitespace] _Whitespace("Specular / Emissive", Float) = 0
		_SpecEmissiveTex("Spec/Emissive Map (R - Spec Mask, G - Gloss, B - Emissive)", 2D) = "black" {}
		_SpecularIntensity("Specular Intensity", Float) = 1
		_SpecularGlossModulation("Specular Gloss Modulation", Float) = 1
		_SpecularColor("Specular Color", Color) = (0, 0, 0, 1)
		_EmissiveColor("Emissive Color", Color) = (0, 0, 0, 1)
		
		[Whitespace] _Whitespace("Emissive Parallax", Float) = 0
		_HeightTex ("Height Map", 2D) = "black" {}
		_ParallaxIntensity("Parallax Intensity (X/Y)", Float) = 0
		
		[Whitespace] _Whitespace("Reflections", Float) = 0
		_ReflectionColor("Reflection Color", Color) = (0, 0, 0, 1)
		_ReflectionHDR("Reflection HDR", Float) = 0
		
		[Whitespace] _Whitespace("Fresnel", Float) = 0
		_FresnelScale("Fresnel Intensity", Float) = 0
		_FresnelPower("Fresnel Power", Float) = 1
		_FresnelColor("Fresnel Color", Color) = (1, 1, 1, 1)
		
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
				#define EBG_TWO_TONE_ON
				#define EBG_HIDDEN
				
				#define EBG_VERTEX_LIGHTING_OFF
				#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
				#define EBG_DIFFUSE_OFF
				#define EBG_NORMAL_MAP_OFF
				#define EBG_SPEC_ON
				#define EBG_EMISSIVE_ON
				#define EBG_REFLECTIONS_ON
				#define EBG_PLANAR_REFLECTIONS_OFF
				#define EBG_FRESNEL_ON
				#define EBG_DISABLE_LIGHTMAP_OFF
				#define EBG_PARALLAX_ON
				
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
