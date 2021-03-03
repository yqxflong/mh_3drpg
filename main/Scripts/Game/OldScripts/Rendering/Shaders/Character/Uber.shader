Shader "EBG/Character/Uber"
{
	Properties
	{
		_MainTex("Diffuse Tex", 2D) = "black" {}

		_NDotLWrap("N.L Wrap", Float) = 0

		[Whitespace] _Whitespace("Second Diffuse", Float) = 0

		_NDotLWrap1("N.L Wrap 2", Float) = 0

		[Whitespace] _Whitespace("SH Probes", Float) = 0

		[MaterialToggle] EBG_SH_PROBES ("Enable", Float) = 0

		[Whitespace] _Whitespace("Color", Float) = 0

		_ColorScale ("Color Scale", range(0,1)) = 1

		[Whitespace] _Whitespace("ColorFilter", Float) = 0
		[MaterialToggle] EBG_COLORFILTER ("Enable", Float) = 0
		_FinalColor ("FinalColor",Color) = (1,1,1,0)
		_ContrastIntansity ("ContrastIntansity",range(0,1))=1
		_Brightness ("Brightness",range(-1,1))=0
		_GrayScale ("GrayScale",range(0,1))=0

		[Whitespace] _Whitespace("Normal Map", Float) = 0

		[MaterialToggle] EBG_NORMAL_MAP ("Enable", Float) = 0
		_NormalMap ("Normal Map", 2D) = "bump" {}

		[Whitespace] _Whitespace("Specular", Float) = 0

		[MaterialToggle] EBG_SPEC ("Enable", Float) = 0
		_SpecTex("Spec Map (A - Gloss)", 2D) = "black" {}
		_SpecularIntensity("Intensity", Float) = 1
		_SpecularGlossModulation("Gloss Modulation", Float) = 1

		[Whitespace] _Whitespace("Emissive", Float) = 0

		[MaterialToggle] EBG_EMISSIVE ("Enable", Float) = 0
		_EmissiveTex("Emissive Map", 2D) = "black" {}
		_EmissiveColor("Color", Color) = (0, 0, 0, 1)

		[Whitespace] _Whitespace("Reflections", Float) = 0

		[MaterialToggle] EBG_REFLECTIONS ("Enable", Float) = 0
		//[MaterialToggle] EBG_BLURRY_REFLECTIONS ("Use Blurry", Float) = 0
		_ReflectionColor("Color", Color) = (0, 0, 0, 1)
		_ReflectionHDR("HDR", Float) = 0
		_ReflectionFresnelIntensity("Fresnel Boost", Float) = 0
		_ReflectionFresnelPower("Fresnel Power", Float) = 1

		[Whitespace] _Whitespace("Fresnel", Float) = 0

		[MaterialToggle] EBG_FRESNEL ("Enable", Float) = 0
		_FresnelIntensity("Intensity", Float) = 0
		_FresnelPower("Power", Float) = 1
		_FresnelColor("Color", Color) = (1, 1, 1, 1)

		[Whitespace] _Whitespace("Color Customization", Float) = 0
		[MaterialToggle] EBG_COLORCUSTOMIZATION("Enable", Float) = 0
		_Tint1("Tint1", Color) 	= (1.0,1.0,1.0,1.0) // tint color customization
		_Tint2("Tint2", Color) 	= (1.0,1.0,1.0,1.0) // tint color customization
		_Tint3("Tint3", Color) 	= (1.0,1.0,1.0,1.0) // tint color customization
		_Tint4("Tint4", Color) 	= (1.0,1.0,1.0,1.0) // tint color customization
		_TintTex("Tint (RGBA)", 2D)  = "black" {}   // tint map texture
		_ShadowColor("Shadow Color", Color) = (1, 1, 1, 1)
	}

	Category
	{
		Tags {
			"Queue"="Geometry"
			"RenderType"="CharacterUnmerged"
			"LightMode"="ForwardBase"
		}
		Cull Back
		ZWrite On
		Lighting On
		ZTest Lequal
		Blend Off
		Fog { Mode Off }

		Subshader
		{
			LOD 300

			Pass
			{
				//ZTest Lequal
				//ZWrite On
				//Blend Off

				Stencil {
					Ref 4
					Comp always
					Pass replace
				}

				CGPROGRAM

				#pragma exclude_renderers xbox360 ps3 flash

				// defs
				#define EBG_POINT_LIGHT
				#undef EBG_TRANSPARENT
				#undef EBG_OCCLUSION

				// features
				#define EBG_FOG_ON
				#undef EBG_FOG_OFF

				#define EBG_RIM_ON
				#undef EBG_RIM_OFF

				#undef EBG_DIFFUSE_ON
				#define EBG_DIFFUSE_OFF

				#undef EBG_DETAIL_ON
				#define EBG_DETAIL_OFF
				
				#undef EBG_ALPHA_CUTOFF_ON
				#define EBG_ALPHA_CUTOFF_OFF
				
				#define EBG_DYNAMIC_SHADOWS_ON
				#undef EBG_DYNAMIC_SHADOWS_OFF

				// keywords
				#pragma multi_compile EBG_COLORFILTER_OFF EBG_COLORFILTER_ON
				//need to put EBG_COLORCUSTOMIZATION option before others... weird
				#pragma multi_compile EBG_COLORCUSTOMIZATION_OFF EBG_COLORCUSTOMIZATION_ON
				#pragma multi_compile EBG_NORMAL_MAP_OFF EBG_NORMAL_MAP_ON
				#pragma multi_compile EBG_SPEC_OFF EBG_SPEC_ON
				#pragma multi_compile EBG_EMISSIVE_OFF EBG_EMISSIVE_ON
				//#pragma multi_compile EBG_ANISOTROPIC_OFF EBG_ANISOTROPIC_ON
				#pragma multi_compile EBG_REFLECTIONS_OFF EBG_REFLECTIONS_ON
				//#pragma multi_compile EBG_BLURRY_REFLECTIONS_OFF EBG_BLURRY_REFLECTIONS_ON
				#pragma multi_compile EBG_FRESNEL_OFF EBG_FRESNEL_ON
				#pragma multi_compile EBG_SH_PROBES_OFF EBG_SH_PROBES_ON

				#pragma target 3.0

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"
				#include "Lib/EBG_Globals.cginc"
				#include "Lib/CharUber.cginc"

				#pragma vertex vertex_shader
				#pragma fragment fragment_shader

				#pragma multi_compile_fwdbase

				ENDCG
			}
		}

		Subshader
		{
			LOD 200

			Pass
			{
				//ZTest Lequal
				//ZWrite On
				//Blend Off

				CGPROGRAM

				#pragma exclude_renderers xbox360 ps3 flash

				// defs
				#define EBG_POINT_LIGHT
				#undef EBG_TRANSPARENT
				#undef EBG_OCCLUSION

				// features
				#define EBG_FOG_ON
				#undef EBG_FOG_OFF

				#undef EBG_RIM_ON
				#define EBG_RIM_OFF

				#undef EBG_DIFFUSE_ON
				#define EBG_DIFFUSE_OFF

				#undef EBG_DETAIL_ON
				#define EBG_DETAIL_OFF

				#undef EBG_ALPHA_CUTOFF_ON
				#define EBG_ALPHA_CUTOFF_OFF

				// keywords
				#undef EBG_SPEC_ON
				#define EBG_SPEC_OFF

				#undef EBG_NORMAL_MAP_ON
				#define EBG_NORMAL_MAP_OFF

				#undef EBG_FRESNEL_ON
				#define EBG_FRESNEL_OFF

				#undef EBG_REFLECTIONS_ON
				#define EBG_REFLECTIONS_OFF

				#define EBG_DYNAMIC_SHADOWS_OFF
				#undef EBG_DYNAMIC_SHADOWS_ON

				#pragma multi_compile EBG_COLORFILTER_OFF EBG_COLORFILTER_ON
				//need to put EBG_COLORCUSTOMIZATION option before others... weird
				#pragma multi_compile EBG_COLORCUSTOMIZATION_OFF EBG_COLORCUSTOMIZATION_ON
				#pragma multi_compile EBG_EMISSIVE_OFF EBG_EMISSIVE_ON
				//#pragma multi_compile EBG_ANISOTROPIC_OFF EBG_ANISOTROPIC_ON
				//#pragma multi_compile EBG_BLURRY_REFLECTIONS_OFF EBG_BLURRY_REFLECTIONS_ON
				#pragma multi_compile EBG_SH_PROBES_OFF EBG_SH_PROBES_ON

				#pragma target 3.0

				#include "UnityCG.cginc"
				#include "Lib/EBG_Globals.cginc"
				#include "Lib/CharUber.cginc"

				#pragma vertex vertex_shader
				#pragma fragment fragment_shader

				ENDCG
			}
		}
	}
	FallBack "Diffuse"
}
