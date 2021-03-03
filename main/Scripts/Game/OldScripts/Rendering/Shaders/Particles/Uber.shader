Shader "EBG/Particle/Uber" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		
		[Whitespace] _Whitespace("Blending", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcFactor ("Source Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DstFactor ("Destination Blend Mode", Float) = 10
		[Enum(Add,0,Subtract,1,ReverseSubtract,2,Min,3,Max,4)] _BlendOp ("Blend Operation", Float) = 0
		[MaterialToggle] EBG_EMISSIVE ("Additive point light/height fade", Float) = 0
		
		[Whitespace] _Whitespace("Depth Testing", Float) = 0 
		[Enum(UnityEngine.Rendering.CompareFunction)] _DepthTest ("Depth test", Float) = 4
		
		[Whitespace] _Whitespace("Hue Shift", Float) = 0
		[MaterialToggle] EBG_HUE_SHIFT ("Enable", Float) = 0
		_HueShift ("Hue Shift", Float) = 0
		_Sat ("Saturation", Float) = 1
		_Value ("Value", Float) = 1
		
		[Whitespace] _Whitespace("Dynamic Point Lights", Float) = 0
		[MaterialToggle] EBG_NORMAL_MAP ("Enable", Float) = 0
		_PointLightIntensity("Intensity", Float) = 0.5
		
		[Whitespace] _Whitespace("Height Fade", Float) = 0
		[MaterialToggle] EBG_FRESNEL ("Enable", Float) = 0
		_HeightFade("Fade Out Start Height", Float) = 0.1
		_GroundHeight("Fade Out End Height", Float) = 0
		
		[Whitespace] _Whitespace("UV Scroll", Float) = 0
		_UVScroll("X/Y Speed", Vector) = (0, 0, 0, 0)
		
		[Whitespace] _Whitespace("Adjustments", Float) = 0
		[MaterialToggle] EBG_SPEC ("Enable", Float) = 0
		_ShadowColor("Shadow Color", Color) = (1, 1, 1, 1)
	}
	Category
	{
		Tags {
			"Queue"="Transparent" 
			"RenderType"="Transparent"
			"LightMode"="ForwardBase"
		}	
		Cull Off
		ZWrite Off
		ZTest [_DepthTest]
		Blend [_SrcFactor] [_DstFactor]
		BlendOp [_BlendOp]
		Fog { Mode Off }
		
		SubShader
		{
			LOD 200
			
			Pass
			{
				Name "Main"
				
				CGPROGRAM	
				
				//#include "UnityCG.cginc"	
				#include "../Lib/EBG_Globals.cginc"
				
				#pragma multi_compile EBG_HUE_SHIFT_OFF 			EBG_HUE_SHIFT_ON
				
				#pragma multi_compile EBG_NORMAL_MAP_OFF			EBG_NORMAL_MAP_ON
				//keyword reuse: point light
				
				#pragma multi_compile EBG_FRESNEL_OFF				EBG_FRESNEL_ON
				//keyword reuse: height fade
				
				#pragma multi_compile EBG_EMISSIVE_OFF 				EBG_EMISSIVE_ON
				//keyword reuse: additive
				
				#pragma multi_compile EBG_SPEC_OFF 					EBG_SPEC_ON
				//keyword reuse: adjustments
				
				#include "Lib/ParticleUber.cginc"
					
				#pragma vertex vertex_lm 
				#pragma fragment fragment_lm 
				
				ENDCG 
			}
		}
		
		SubShader
		{
			LOD 100
			
			Pass
			{
				Name "Main"
				
				CGPROGRAM	
				
				//#include "UnityCG.cginc"	
				#include "../Lib/EBG_Globals.cginc"
				
				#pragma multi_compile EBG_HUE_SHIFT_OFF 			EBG_HUE_SHIFT_ON
				
				#undef EBG_NORMAL_MAP_ON
				#define EBG_NORMAL_MAP_OFF				
				//keyword reuse: point light
				
				#undef EBG_FRESNEL_ON
				#define EBG_FRESNEL_OFF	
				//keyword reuse: height fade
				
				#pragma multi_compile EBG_EMISSIVE_OFF 				EBG_EMISSIVE_ON 			
				//keyword reuse: additive
				
				#pragma multi_compile EBG_SPEC_OFF 					EBG_SPEC_ON
				//keyword reuse: adjustments
				
				#include "Lib/ParticleUber.cginc"
					
				#pragma vertex vertex_lm 
				#pragma fragment fragment_lm 
				
				ENDCG 
			}
		}
	}
}
