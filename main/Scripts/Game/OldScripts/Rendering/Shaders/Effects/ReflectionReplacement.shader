// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Effects/ReflectionReplacement" 
{
	//ENVIRONMENT
	SubShader
	{		 
		Tags 
		{
			"Queue"="Geometry"
			"RenderType"="Environment"
			"LightMode"="ForwardBase"
		}
		Fog { Mode Off }
		LOD 100
		Cull Back
		Lighting Off
		ZWrite On
		ZTest Lequal
		Blend Off
		
		Pass
		{
			CGPROGRAM
			
			#undef EBG_POINT_LIGHT
			#undef EGB_TRANSPARENT
			#undef EBG_DETAIL
			
			#define EBG_HIDDEN_ON
			
			#undef EBG_LIGHTMAP_ON
			#define EBG_LIGHTMAP_OFF
			
			#undef EBG_DIFFUSE_ON
			#define EBG_DIFFUSE_OFF
			
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
			
			#pragma multi_compile EBG_DISABLE_LIGHTMAP_OFF EBG_DISABLE_LIGHTMAP_ON
			
			#include "UnityCG.cginc"
			#include "../Lib/EBG_Globals.cginc"
			
			#include "../Environment/Uber/Lib/EnviroUber.cginc"

			#pragma target 3.0

			#pragma vertex vertex_shader
			#pragma fragment fragment_shader
			
			ENDCG
		}
	}
	
	//ENVIRONMENT UNMERGED
	SubShader
	{		 
		Tags 
		{
			"Queue"="Geometry"
			"RenderType"="EnvironmentUnmerged"
			"LightMode"="ForwardBase"
		}
		Fog { Mode Off }
		LOD 100
		Cull Back
		Lighting Off
		ZWrite On
		ZTest Lequal
		Blend Off
		
		Pass
		{
			CGPROGRAM
			
			#undef EBG_POINT_LIGHT
			#undef EGB_TRANSPARENT
			#undef EBG_DETAIL
			
			#undef EBG_HIDDEN_ON
			
			#pragma multi_compile EBG_LIGHTMAP_ON EBG_LIGHTMAP_OFF
			
			#undef EBG_DIFFUSE_ON
			#define EBG_DIFFUSE_OFF
			
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
			
			#pragma multi_compile EBG_DISABLE_LIGHTMAP_OFF EBG_DISABLE_LIGHTMAP_ON
			
			#include "UnityCG.cginc"
			#include "../Lib/EBG_Globals.cginc"
			
			#include "../Environment/Uber/Lib/EnviroUber.cginc"

			#pragma target 3.0

			#pragma vertex vertex_shader
			#pragma fragment fragment_shader
			
			ENDCG
		}
	}

	//ENVIRONMENT T4M
	SubShader
	{
		Tags
		{
			"Queue" = "Geometry"
			"RenderType" = "EnvironmentT4M"
			"LightMode" = "ForwardBase"
		}
		LOD 100
		Cull Back
		Lighting Off
		ZWrite On
		ZTest Lequal
		Blend Off

		Pass
		{
			CGPROGRAM

			#undef EBG_POINT_LIGHT
			#undef EBG_TRANSPARENT
			#undef EBG_DETAIL
			#undef EBG_SH_PROBES
			#undef EBG_DIFFUSE_COLOR
			#define EBG_T4M_ON

			#define EBG_HIDDEN_ON

			#define EBG_FOG_ON
			#undef EBG_FOG_OFF

			#undef EBG_DYNAMIC_SHADOWS_ON
			#define EBG_DYNAMIC_SHADOWS_OFF

			#undef EBG_LIGHTMAP_ON
			#define EBG_LIGHTMAP_OFF

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

			#pragma multi_compile EBG_DIFFUSE_OFF EBG_DIFFUSE_ON

			#include "UnityCG.cginc"	
			#include "../Lib/EBG_Globals.cginc"

			#include "../Environment/Uber/Lib/EnviroUber.cginc"

			#pragma target 3.0

			#pragma vertex vertex_shader
			#pragma fragment fragment_shader

			ENDCG
		}
	}
		
	//ENVIRONMENT TRANSPARENT
	SubShader
	{		 
		Tags {
			"Queue"="Transparent"
			"RenderType"="EnvironmentTransparent"
			"LightMode"="ForwardBase"
		}
		LOD 100
		Cull Back
		Lighting Off
		ZWrite Off
		ZTest Lequal
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
			CGPROGRAM  
			
			#undef EBG_POINT_LIGHT
			#define EGB_TRANSPARENT
			#undef EBG_DETAIL
			
			#define EBG_HIDDEN_ON
			
			#undef LIGHTMAP_ON
			#define LIGHTMAP_OFF
			
			#pragma multi_compile EBG_DIFFUSE_OFF EBG_DIFFUSE_ON
			
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
			
			#pragma multi_compile EBG_DISABLE_LIGHTMAP_OFF EBG_DISABLE_LIGHTMAP_ON
			
			#include "UnityCG.cginc"	
			#include "../Lib/EBG_Globals.cginc"
			
			#include "../Environment/Uber/Lib/EnviroUber.cginc"

			#pragma target 3.0

			#pragma vertex vertex_shader
			#pragma fragment fragment_shader
				
			ENDCG
		}
	}
	
	//UNLIT BASE
	SubShader 
	{
		Tags {
			"Queue"="Geometry" 
			"RenderType"="UnlitBase"
			"LightMode"="ForwardBase"
		}
		Cull Back
		ZWrite On
		ZTest Lequal
		Blend Off
		LOD 100
		
		Pass
		{
			CGPROGRAM	
			
			#include "UnityCG.cginc"	
									
			sampler2D _MainTex;		
				
			struct Input 
			{
			    float4 vertex : POSITION;
			    float4 texcoord0 : TEXCOORD0;
			};
			
			struct VtoS
			{
	          	float4 position : SV_POSITION;	
				float2 uv_MainTex : TEXCOORD0; 
			};
			
			float4 _MainTex_ST;
			
			VtoS vertex_lm(Input v)
			{   
				VtoS data;
				data.position = UnityObjectToClipPos(v.vertex);  
				data.uv_MainTex = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw; 
				return data;
			}
			
			fixed4 fragment_lm(VtoS IN) : COLOR0
			{
				return tex2D(_MainTex, IN.uv_MainTex);
			}   
				
			#pragma vertex vertex_lm 
			#pragma fragment fragment_lm 
			
			ENDCG 
		}
	}
	
	//SCROLL PUSLE OPAQUE
	SubShader 
	{
		Tags {
			"Queue"="Geometry" 
			"RenderType"="ScrollPulseOpaque"
			"LightMode"="ForwardBase"
		}
		Cull Back
		ZWrite On
		ZTest Lequal
		Blend Off
		LOD 100
		
		Pass
		{
			CGPROGRAM	
			
			#include "UnityCG.cginc"	
									
			sampler2D _MainTex;		
			sampler2D _AnimTex;	
			fixed4 _Color;
			float _ScrollX;
			float _ScrollY;
			float _Pulse;
			fixed4 _MinPulseColor;
			fixed4 _MaxPulseColor;
				
			struct Input 
			{
			    float4 vertex : POSITION;
			    float4 texcoord0 : TEXCOORD0;
			};
			
			struct VtoS
			{
	          	float4 position : SV_POSITION;	
				float2 uv_MainTex : TEXCOORD0; 
				float2 uv_AnimTex : TEXCOORD1; 
				fixed4 color : COLOR;
			};
			
			float4 _MainTex_ST;
			
			VtoS vertex_lm(Input v)
			{   
				VtoS data;
				data.position = UnityObjectToClipPos(v.vertex);  
				data.uv_MainTex = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw; 
				data.uv_AnimTex = data.uv_MainTex + float2(_ScrollX, _ScrollY) * _Time.y; 
				data.color = _Color * lerp(_MinPulseColor, _MaxPulseColor, cos(_Pulse * _Time.y * 3.14159) * 0.5 + 0.5);
				return data;
			}
			
			fixed4 fragment_lm(VtoS IN) : COLOR0
			{
				fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 animTex = tex2D(_AnimTex, IN.uv_AnimTex);
				fixed4 c = mainTex * animTex * IN.color;
				return c;
			}   
				
			#pragma vertex vertex_lm 
			#pragma fragment fragment_lm 
			
			ENDCG 
		}
	}

	//SCROLL PULSE TRANSPARENT
	SubShader 
	{
		Tags {
			"Queue"="Transparent"
			"RenderType"="ScrollPulseTransparent"
			"LightMode"="ForwardBase"
		}
		LOD 100
		Cull Back
		Lighting Off
		ZWrite Off
		ZTest Lequal
		Blend [_BlendModeSrc] [_BlendModeDst]
		
		Pass
		{
			CGPROGRAM	
			
			#include "UnityCG.cginc"	
									
			sampler2D _MainTex;		
			sampler2D _AnimTex;	
			fixed4 _Color;
			float _ScrollX;
			float _ScrollY;
			float _Pulse;
			fixed4 _MinPulseColor;
			fixed4 _MaxPulseColor;
				
			struct Input 
			{
			    float4 vertex : POSITION;
			    float4 texcoord0 : TEXCOORD0;
			};
			
			struct VtoS
			{
	          	float4 position : SV_POSITION;	
				float2 uv_MainTex : TEXCOORD0; 
				float2 uv_AnimTex : TEXCOORD1; 
				fixed4 color : COLOR;
			};
			
			float4 _MainTex_ST;
			
			VtoS vertex_lm(Input v)
			{   
				VtoS data;
				data.position = UnityObjectToClipPos(v.vertex);  
				data.uv_MainTex = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw; 
				data.uv_AnimTex = data.uv_MainTex + float2(_ScrollX, _ScrollY) * _Time.y; 
				data.color = _Color * lerp(_MinPulseColor, _MaxPulseColor, cos(_Pulse * _Time.y * 3.14159) * 0.5 + 0.5);
				return data;
			}
			
			fixed4 fragment_lm(VtoS IN) : COLOR0
			{
				fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 animTex = tex2D(_AnimTex, IN.uv_AnimTex);
				fixed4 c = mainTex * animTex * IN.color;
				return c;
			}   
				
			#pragma vertex vertex_lm 
			#pragma fragment fragment_lm 
			
			ENDCG 
		}
	}
	
	//SCROLL
	SubShader 
	{
		LOD 100
		Tags {
			"Queue"="Geometry" 
			"RenderType"="Scroll"
			"LightMode"="ForwardBase"
		}
		Cull Back
		ZWrite On
		ZTest Lequal
		Blend Off
		
		Pass
		{
			CGPROGRAM	
			
			#include "UnityCG.cginc"	
									
			sampler2D _MainTex;	
			fixed4 _Color;
			float _ScrollX;
			float _ScrollY;
				
			struct Input 
			{
			    float4 vertex : POSITION;
			    float4 texcoord0 : TEXCOORD0;
			};
			
			struct VtoS
			{
	          	float4 position : SV_POSITION;	
				float2 uv_MainTex : TEXCOORD0; 
			};
			
			VtoS vertex_lm(Input v)
			{   
				VtoS data;
				data.position = UnityObjectToClipPos(v.vertex);  
				data.uv_MainTex = v.texcoord0.xy + float2(_ScrollX, _ScrollY) * _Time.y; 
				return data;
			}
			
			fixed4 fragment_lm(VtoS IN) : COLOR0
			{
				return tex2D(_MainTex, IN.uv_MainTex) * _Color;
			}   
				
			#pragma vertex vertex_lm 
			#pragma fragment fragment_lm 
			
			ENDCG 
		}
	}
	
	//ANIMATED STRIP
	SubShader 
	{
		Tags {
			"Queue"="Geometry" 
			"RenderType"="AnimatedStrip"
			"LightMode"="ForwardBase"
		}
		LOD 100
		Cull Back
		ZWrite On
		ZTest Lequal
		Blend Off
		
		Pass
		{
			CGPROGRAM	
			
			#include "UnityCG.cginc"		
									
			sampler2D _MainTex;		
			float _Steps;	
			float _StepTime;
				
			struct Input 
			{
			    float4 vertex : POSITION;
			    float4 texcoord0 : TEXCOORD0;
			    float4 color : COLOR;
			};
			
			struct VtoS
			{
	          	float4 position : SV_POSITION;	
				float2 uv_MainTex : TEXCOORD0; 
			};
			
			VtoS vertex_lm(Input v)
			{   
				VtoS data;
				data.position = UnityObjectToClipPos(v.vertex);
				float baseStepOffset = dot(float4(1, 2, 4, 8), v.color);  
				data.uv_MainTex = v.texcoord0.xy / float2(_Steps, 1.0f); 	//bring it into the base "step" size
				float currentSteps = round(_Time.y / _StepTime) + baseStepOffset;
				float stepOffset = frac( currentSteps / _Steps );
				data.uv_MainTex.x += stepOffset;							//shift it through the steps
				return data;
			}
			
			fixed4 fragment_lm(VtoS IN) : COLOR0
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				return c;
			}   
				
			#pragma vertex vertex_lm 
			#pragma fragment fragment_lm 
			
			ENDCG 
		}
	}

	//CHARACTER
	SubShader
	{
		Tags {
			"Queue"="Geometry"
			"RenderType"="Character"
			"LightMode"="ForwardBase"
		}
		LOD 100
		Cull Back
		Lighting Off
		ZWrite On
		ZTest Lequal
		Blend Off
		
		Pass
		{
			CGPROGRAM  
		
			#undef EBG_POINT_LIGHT
			#define EBG_RIM_ON
			#undef EBG_DIFFUSE_ON
			
			#undef EBG_NORMAL_MAP_ON
			#define EBG_NORMAL_MAP_OFF
			
			#undef EBG_DETAIL_ON
			#define EBG_DETAIL_OFF
			
			#undef EBG_SPEC_ON
			#define EBG_SPEC_OFF
			
			#undef EBG_EMISSIVE_ON
			#define EBG_EMISSIVE_OFF
			
			//#undef EBG_ANISOTROPIC_ON
			//#define EBG_ANISOTROPIC_OFF
			
			#undef EBG_REFLECTIONS_ON
			#define EBG_REFLECTIONS_OFF
			
			//#undef EBG_BLURRY_REFLECTIONS_ON
			//#define EBG_BLURRY_REFLECTIONS_OFF
			
			#undef EBG_FRESNEL_ON
			#define EBG_FRESNEL_OFF
			
			#pragma multi_compile EBG_SH_PROBES_OFF EBG_SH_PROBES_ON
			
			#include "UnityCG.cginc"	
			#include "../Lib/EBG_Globals.cginc"
			
			#include "../Character/Lib/CharUber.cginc"

			#pragma target 3.0

			#pragma vertex vertex_shader
			#pragma fragment fragment_shader
				
			ENDCG
		}
	}

	//PARTICLES
	SubShader
	{
		Tags {
			"Queue"="Transparent"
			"RenderType"="Transparent"
			"LightMode"="ForwardBase"
		}
		LOD 100
		Cull Back
		Lighting Off
		ZWrite Off
		ZTest [_DepthTest]
		Blend [_SrcFactor] [_DstFactor]
		BlendOp [_BlendOp]
		
		Pass
		{
			
			CGPROGRAM	
			
			#include "UnityCG.cginc"	
			#include "../Lib/EBG_Globals.cginc"
			
			#pragma multi_compile EBG_HUE_SHIFT_OFF EBG_HUE_SHIFT_ON
								
			struct Input 
			{
			    float4 vertex : POSITION;
			    float4 texcoord0 : TEXCOORD0;
				fixed4 color : COLOR;
			};
			
			struct VtoS
			{
	          	float4 position : SV_POSITION;	
				float2 uv_MainTex : TEXCOORD0; 
				fixed4 color : COLOR;
			};
				
			sampler2D _MainTex;
			float4 _MainTex_ST;
			#ifdef EBG_HUE_SHIFT_ON
			fixed4 _HueShiftR;
			fixed4 _HueShiftG;
			fixed4 _HueShiftB;
			#endif
			
			VtoS vertex_lm(Input v)
			{   
				VtoS data;
				data.position = UnityObjectToClipPos(v.vertex);  
				data.uv_MainTex = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw; 
				data.color = v.color;
				return data;
			}
			
			fixed4 fragment_lm(VtoS IN) : COLOR0
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
				#ifdef EBG_HUE_SHIFT_ON
				fixed r = dot(c, _HueShiftR);
				fixed g = dot(c, _HueShiftG);
				fixed b = dot(c, _HueShiftB);
				c = fixed4(r, g, b, c.a);
				#endif
				return c;
			}   
				
			#pragma vertex vertex_lm 
			#pragma fragment fragment_lm 
			
			ENDCG 
		}
	}
}
