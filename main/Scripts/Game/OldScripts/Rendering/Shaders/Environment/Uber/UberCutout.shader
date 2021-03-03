Shader "EBG/Enviro/Uber/UberCutout"
{
	Properties
	{
		_MainTex("Diffuse Tex", 2D) = "black" {}

		[Whitespace] _Whitespace("Highlights", Float) = 0
		[MaterialToggle] EBG_HIGHLIGHTS_IGNORE_ALPHA ("Highlights ignore alpha", Float) = 1

		[Whitespace] _Whitespace("Diffuse", Float) = 0
		[MaterialToggle] EBG_DIFFUSE ("Enable", Float) = 0
		_NDotLWrap("N.L Wrap", Float) = 0

		[Whitespace] _Whitespace("Alpha Cutoff", Float) = 0
		[MaterialToggle] EBG_ALPHA_CUTOFF ("Enable", Float) = 0
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

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
		//[MaterialToggle] EBG_PLANAR_REFLECTIONS ("Use Planar Reflections", Float) = 0
		//_PlanarReflectionColor("Planar Reflection Color", Color) = (0, 0, 0, 1)
		//_PlanarReflectionRoughness("Planar Reflection Roughness", Vector) = (0, 0, 0, 0)

		[Whitespace] _Whitespace("Fresnel", Float) = 0
		[MaterialToggle] EBG_FRESNEL ("Enable", Float) = 0
		_FresnelScale("Fresnel Intensity", Float) = 0
		_FresnelPower("FresnelPower", Float) = 1
		_FresnelColor("FresnelColor", Color) = (1, 1, 1, 1)

		[Whitespace] _Whitespace("Lightmaps", Float) = 0
		[MaterialToggle] EBG_DISABLE_LIGHTMAP ("Disable", Float) = 0
		//[Whitespace] _Whitespace("Vertex LightMaps", Float) = 0
		//[Enum(AlwaysOn,0,LowEndOnly,1,AlwaysOff,2)] _VertexLightmapOption ("Vertex Lightmap Options", Float) = 1
		_ShadowColor("Shadow Color", Color) = (255, 216, 197, 255)
		
	}
	Category
	{
		Tags {
			"Queue"="Geometry"
			"RenderType" = "EnvironmentUnmerged"
			"LightMode"="ForwardBase"
		}
		Cull Back
		Lighting On
		ZWrite On
		ZTest Lequal
		Blend Off
		Fog { Mode Off }

		SubShader
		{
			LOD 400

			Pass
			{

				CGPROGRAM

				// defs
				#undef EBG_POINT_LIGHT
				#define EBG_TRANSPARENT
				#undef EBG_DETAIL
				#undef EBG_SH_PROBES
				#undef EBG_DIFFUSE_COLOR
				//#define SHADOWS_SCREEN

				// features
				#define EBG_FOG_ON
				#undef EBG_FOG_OFF

				#define EBG_DYNAMIC_SHADOWS_ON
				#undef EBG_DYNAMIC_SHADOWS_OFF

				#define EBG_LIGHTMAP_ON
				#undef EBG_LIGHTMAP_OFF

				#undef EBG_VERTEX_LIGHTING_ON
				#define EBG_VERTEX_LIGHTING_OFF

				// keywords
				#pragma multi_compile EBG_DIFFUSE_OFF EBG_DIFFUSE_ON
				#pragma multi_compile EBG_NORMAL_MAP_OFF EBG_NORMAL_MAP_ON
				#pragma multi_compile EBG_SPEC_OFF EBG_SPEC_ON
				#pragma multi_compile EBG_EMISSIVE_OFF EBG_EMISSIVE_ON
				#pragma multi_compile EBG_REFLECTIONS_OFF EBG_REFLECTIONS_ON

				//#pragma multi_compile EBG_PLANAR_REFLECTIONS_OFF EBG_PLANAR_REFLECTIONS_ON
				#undef EBG_PLANAR_REFLECTIONS_ON
				#define EBG_PLANAR_REFLECTIONS_OFF

				#pragma multi_compile EBG_FRESNEL_OFF EBG_FRESNEL_ON
				#pragma multi_compile EBG_DISABLE_LIGHTMAP_OFF EBG_DISABLE_LIGHTMAP_ON
				#pragma multi_compile EBG_ALPHA_CUTOFF_OFF EBG_ALPHA_CUTOFF_ON
				#pragma multi_compile EBG_HIGHLIGHTS_IGNORE_ALPHA_OFF EBG_HIGHLIGHTS_IGNORE_ALPHA_ON

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"
				#include "../../Lib/EBG_Globals.cginc"

				#include "Lib/EnviroUber.cginc"

				#pragma target 3.0

				#pragma vertex vertex_shader
				#pragma fragment fragment_shader
				#pragma multi_compile_fwdbase

				ENDCG
			}
		}

		SubShader
		{
			LOD 200

			Pass
			{
				CGPROGRAM

				// defs
				#undef EBG_POINT_LIGHT
				#define EBG_TRANSPARENT
				#undef EBG_DETAIL
				#undef EBG_SH_PROBES
				#undef EBG_DIFFUSE_COLOR
				//#define SHADOWS_SCREEN

				// features
				#define EBG_FOG_ON
				#undef EBG_FOG_OFF

				#undef EBG_DYNAMIC_SHADOWS_ON
				#define EBG_DYNAMIC_SHADOWS_OFF

				#define EBG_LIGHTMAP_ON
				#undef EBG_LIGHTMAP_OFF

				#undef EBG_VERTEX_LIGHTING_ON
				#define EBG_VERTEX_LIGHTING_OFF

				// keywords
				#pragma multi_compile EBG_DIFFUSE_OFF EBG_DIFFUSE_ON
				#pragma multi_compile EBG_NORMAL_MAP_OFF EBG_NORMAL_MAP_ON
				#pragma multi_compile EBG_SPEC_OFF EBG_SPEC_ON
				#pragma multi_compile EBG_EMISSIVE_OFF EBG_EMISSIVE_ON
				#pragma multi_compile EBG_REFLECTIONS_OFF EBG_REFLECTIONS_ON

				//#pragma multi_compile EBG_PLANAR_REFLECTIONS_OFF EBG_PLANAR_REFLECTIONS_ON
				#undef EBG_PLANAR_REFLECTIONS_ON
				#define EBG_PLANAR_REFLECTIONS_OFF

				#pragma multi_compile EBG_FRESNEL_OFF EBG_FRESNEL_ON
				#pragma multi_compile EBG_DISABLE_LIGHTMAP_OFF EBG_DISABLE_LIGHTMAP_ON
				#pragma multi_compile EBG_ALPHA_CUTOFF_OFF EBG_ALPHA_CUTOFF_ON
				#pragma multi_compile EBG_HIGHLIGHTS_IGNORE_ALPHA_OFF EBG_HIGHLIGHTS_IGNORE_ALPHA_ON

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"
				#include "../../Lib/EBG_Globals.cginc"

				#include "Lib/EnviroUber.cginc"

				#pragma target 3.0

				#pragma vertex vertex_shader
				#pragma fragment fragment_shader
				#pragma multi_compile_fwdbase

				ENDCG
			}
		}
	}
	FallBack "Diffuse"
}
