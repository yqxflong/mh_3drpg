Shader "EBG/Enviro/Uber/UberT4M"
{
	Properties
	{
		_MainTex("Layer 1 (R)", 2D) = "black" {}//_Splat0,R
		_Splat1("Layer 2 (G)", 2D) = "black" {}
		_Splat2("Layer 3 (B)", 2D) = "black" {}
		_Splat3("Layer 4 (A)", 2D) = "black" {}
		_T4MControl("Control (RGBA)", 2D) = "black" {}

		[Whitespace] _Whitespace("Diffuse", Float) = 0
		[MaterialToggle] EBG_DIFFUSE("Enable", Float) = 0
		_NDotLWrap("N.L Wrap", Float) = 0
	}
	Category
	{
		Tags {
			"Queue"="Geometry"
			"RenderType"="EnvironmentT4M"
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
				#undef EBG_TRANSPARENT
				#undef EBG_DETAIL
				#undef EBG_SH_PROBES
				#undef EBG_DIFFUSE_COLOR
				//#define SHADOWS_SCREEN
				#define EBG_T4M_ON

				// features
				#define EBG_FOG_ON
				#undef EBG_FOG_OFF

				#define EBG_DYNAMIC_SHADOWS_ON
				#undef EBG_DYNAMIC_SHADOWS_OFF

				#define EBG_LIGHTMAP_ON
				#undef EBG_LIGHTMAP_OFF

				#undef EBG_VERTEX_LIGHTING_ON
				#define EBG_VERTEX_LIGHTING_OFF

				#undef EBG_ALPHA_CUTOFF_ON
				#define EBG_ALPHA_CUTOFF_OFF

				#undef EBG_HIGHLIGHTS_IGNORE_ALPHA_ON
				#define EBG_HIGHLIGHTS_IGNORE_ALPHA_OFF

				#undef EBG_NORMAL_MAP_ON
				#define EBG_NORMAL_MAP_OFF

				#undef EBG_SPEC_ON
				#define EBG_SPEC_OFF

				#undef EBG_EMISSIVE_ON
				#define EBG_EMISSIVE_OFF

				#undef EBG_REFLECTIONS_ON
				#define EBG_REFLECTIONS_OFF

				#undef EBG_PLANAR_REFLECTIONS_ON
				#define EBG_PLANAR_REFLECTIONS_OFF

				#undef EBG_FRESNEL_ON
				#define EBG_FRESNEL_OFF

				#undef EBG_DISABLE_LIGHTMAP_ON
				#define EBG_DISABLE_LIGHTMAP_OFF

				// keywords
				#pragma multi_compile EBG_DIFFUSE_OFF EBG_DIFFUSE_ON

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
				#undef EBG_TRANSPARENT
				#undef EBG_DETAIL
				#undef EBG_SH_PROBES
				#undef EBG_DIFFUSE_COLOR
				//#define SHADOWS_SCREEN
				#define EBG_T4M_ON

				// features
				#define EBG_FOG_ON
				#undef EBG_FOG_OFF

				#undef EBG_DYNAMIC_SHADOWS_ON
				#define EBG_DYNAMIC_SHADOWS_OFF

				#define EBG_LIGHTMAP_ON
				#undef EBG_LIGHTMAP_OFF

				#undef EBG_VERTEX_LIGHTING_ON
				#define EBG_VERTEX_LIGHTING_OFF

				#undef EBG_ALPHA_CUTOFF_ON
				#define EBG_ALPHA_CUTOFF_OFF

				#undef EBG_HIGHLIGHTS_IGNORE_ALPHA_ON
				#define EBG_HIGHLIGHTS_IGNORE_ALPHA_OFF

				#undef EBG_NORMAL_MAP_ON
				#define EBG_NORMAL_MAP_OFF

				#undef EBG_SPEC_ON
				#define EBG_SPEC_OFF

				#undef EBG_EMISSIVE_ON
				#define EBG_EMISSIVE_OFF

				#undef EBG_REFLECTIONS_ON
				#define EBG_REFLECTIONS_OFF

				#undef EBG_PLANAR_REFLECTIONS_ON
				#define EBG_PLANAR_REFLECTIONS_OFF

				#undef EBG_FRESNEL_ON
				#define EBG_FRESNEL_OFF

				#undef EBG_DISABLE_LIGHTMAP_ON
				#define EBG_DISABLE_LIGHTMAP_OFF

				// keywords
				#pragma multi_compile EBG_DIFFUSE_OFF EBG_DIFFUSE_ON

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
