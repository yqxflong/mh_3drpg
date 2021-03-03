Shader "EBG/Particle/Environment"
{

	Properties
	{
		[Whitespace] _Whitespace("Diffuse", Float) = 0
		_MainTex("Diffuse Tex", 2D) = "black" {}
		_NDotLWrap("N.L Wrap", Float) = 0
		
		[Whitespace] _Whitespace("SH Probes", Float) = 0
		_SHIntensity("Intensity", Float) = 1
		
		[Whitespace] _Whitespace("Normal Map", Float) = 0
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_NormalMapIntensity("Normal Map Itensity", Range(1, 0.1)) = 1
		_NormalMapDamp("Normal Map Dampening", Float) = 0
		
		[Whitespace] _Whitespace("Specular / Emissive", Float) = 0
		_SpecEmissiveTex("Spec/Emissive Map (R - Spec Mask, G - Gloss, B - Emissive)", 2D) = "black" {}
		_SpecularIntensity("Specular Intensity", Float) = 0
		_SpecularGlossModulation("Specular Gloss Modulation", Float) = 1
		
		[Whitespace] _Whitespace("Reflections", Float) = 0
		_ReflectionColor("Reflection Color", Color) = (0, 0, 0, 1)
		_ReflectionHDR("Reflection HDR", Float) = 0
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
			LOD 500
			
			Pass
			{
				CGPROGRAM  
				
				#define EBG_POINT_LIGHT
				#undef EGB_TRANSPARENT
				#undef EBG_DETAIL
				#define EBG_SH_PROBES
				#undef EBG_DIFFUSE_COLOR
				
				//diffuse is on
				#undef EBG_DIFFUSE_OFF
				#define EBG_DIFFUSE_ON
				
				//normal maps are on
				#undef EBG_NORMAL_MAP_OFF
				#define EBG_NORMAL_MAP_ON
				
				//spec is on
				#undef EBG_SPEC_OFF
				#define EBG_SPEC_ON
				
				//emissive is always off
				#undef EBG_EMISSIVE_ON
				#define EBG_EMISSIVE_OFF
				
				//reflections are always off
				#undef EBG_REFLECTIONS_OFF
				#define EBG_REFLECTIONS_ON
				
				//planar reflections are always off
				#undef EBG_PLANAR_REFLECTIONS_ON
				#define EBG_PLANAR_REFLECTIONS_OFF
				
				//fresnel is always off
				#undef EBG_FRESNEL_ON
				#define EBG_FRESNEL_OFF
				
				//lightmaps are always off
				#undef EBG_DISABLE_LIGHTMAP_OFF
				#define EBG_DISABLE_LIGHTMAP_ON
				
				#include "UnityCG.cginc"	
				#include "../Lib/EBG_Globals.cginc"
				
				#include "../Environment/Uber/Lib/EnviroUber.cginc"

				#pragma target 3.0

				#pragma vertex vertex_shader
				#pragma fragment fragment_shader
					
				ENDCG
			}
		}
	
		SubShader
		{		
			LOD 400
			
			Pass
			{
				CGPROGRAM  
				
				#define EBG_POINT_LIGHT
				#undef EGB_TRANSPARENT
				#undef EBG_DETAIL
				#define EBG_SH_PROBES
				#undef EBG_DIFFUSE_COLOR
				
				//diffuse is on
				#undef EBG_DIFFUSE_OFF
				#define EBG_DIFFUSE_ON
				
				//normal maps are off
				#undef EBG_NORMAL_MAP_ON
				#define EBG_NORMAL_MAP_OFF
				
				//spec is on
				#undef EBG_SPEC_OFF
				#define EBG_SPEC_ON
				
				//emissive is always off
				#undef EBG_EMISSIVE_ON
				#define EBG_EMISSIVE_OFF
				
				//reflections are on
				#undef EBG_REFLECTIONS_OFF
				#define EBG_REFLECTIONS_ON
				
				//planar reflections are always off
				#undef EBG_PLANAR_REFLECTIONS_ON
				#define EBG_PLANAR_REFLECTIONS_OFF
				
				//fresnel is always off
				#undef EBG_FRESNEL_ON
				#define EBG_FRESNEL_OFF
				
				//lightmaps are always off
				#undef EBG_DISABLE_LIGHTMAP_OFF
				#define EBG_DISABLE_LIGHTMAP_ON
				
				#include "UnityCG.cginc"	
				#include "../Lib/EBG_Globals.cginc"
				
				#include "../Environment/Uber/Lib/EnviroUber.cginc"

				#pragma target 3.0

				#pragma vertex vertex_shader
				#pragma fragment fragment_shader
					
				ENDCG
			}
		}
	
		SubShader
		{		
			LOD 300
			
			Pass
			{
				CGPROGRAM  
				
				#define EBG_POINT_LIGHT
				#undef EGB_TRANSPARENT
				#undef EBG_DETAIL
				#define EBG_SH_PROBES
				#undef EBG_DIFFUSE_COLOR
				
				//diffuse is on
				#undef EBG_DIFFUSE_OFF
				#define EBG_DIFFUSE_ON
				
				//normal maps are off
				#undef EBG_NORMAL_MAP_ON
				#define EBG_NORMAL_MAP_OFF
				
				//spec is on
				#undef EBG_SPEC_OFF
				#define EBG_SPEC_ON
				
				//emissive is always off
				#undef EBG_EMISSIVE_ON
				#define EBG_EMISSIVE_OFF
				
				//reflections are off
				#undef EBG_REFLECTIONS_ON
				#define EBG_REFLECTIONS_OFF
				
				//planar reflections are always off
				#undef EBG_PLANAR_REFLECTIONS_ON
				#define EBG_PLANAR_REFLECTIONS_OFF
				
				//fresnel is always off
				#undef EBG_FRESNEL_ON
				#define EBG_FRESNEL_OFF
				
				//lightmaps are always off
				#undef EBG_DISABLE_LIGHTMAP_OFF
				#define EBG_DISABLE_LIGHTMAP_ON
				
				#include "UnityCG.cginc"	
				#include "../Lib/EBG_Globals.cginc"
				
				#include "../Environment/Uber/Lib/EnviroUber.cginc"

				#pragma target 3.0

				#pragma vertex vertex_shader
				#pragma fragment fragment_shader
					
				ENDCG
			}
		}
	
		SubShader
		{		
			LOD 100
			
			Pass
			{
				CGPROGRAM  
				
				#define EBG_POINT_LIGHT
				#undef EGB_TRANSPARENT
				#undef EBG_DETAIL
				#define EBG_SH_PROBES
				#undef EBG_DIFFUSE_COLOR
				
				//diffuse is on
				#undef EBG_DIFFUSE_OFF
				#define EBG_DIFFUSE_ON
				
				//normal maps are on
				#undef EBG_NORMAL_MAP_OFF
				#define EBG_NORMAL_MAP_ON
				
				//spec is off
				#undef EBG_SPEC_ON
				#define EBG_SPEC_OFF
				
				//emissive is always off
				#undef EBG_EMISSIVE_ON
				#define EBG_EMISSIVE_OFF
				
				//reflections are always off
				#undef EBG_REFLECTIONS_ON
				#define EBG_REFLECTIONS_OFF
				
				//planar reflections are always off
				#undef EBG_PLANAR_REFLECTIONS_ON
				#define EBG_PLANAR_REFLECTIONS_OFF
				
				//fresnel is always off
				#undef EBG_FRESNEL_ON
				#define EBG_FRESNEL_OFF
				
				//lightmaps are always off
				#undef EBG_DISABLE_LIGHTMAP_OFF
				#define EBG_DISABLE_LIGHTMAP_ON
				
				#include "UnityCG.cginc"	
				#include "../Lib/EBG_Globals.cginc"
				
				#include "../Environment/Uber/Lib/EnviroUber.cginc"

				#pragma target 3.0

				#pragma vertex vertex_shader
				#pragma fragment fragment_shader
					
				ENDCG
			}
		}
	}
}
