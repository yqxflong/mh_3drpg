// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Enviro/Self-Illumin/AnimatedStrip"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Steps("Frames", Float) = 4
		_StepTime("Frame Time", Float) = 1
		_FogFactor("Fog Factor", Float) = 0.5
		
		_EmissionLM ("Emission (Lightmapper)", Float) = 0
		_Color ("Emission Color", Color) = (1,1,1,1)
	}
	SubShader 
	{
		Tags {
			"Queue"="Geometry" 
			"RenderType"="AnimatedStrip"
			"LightMode"="ForwardBase"
		}
		LOD 200
		Cull Back
		ZWrite On
		ZTest Lequal
		Blend Off
		
		Pass
		{
			CGPROGRAM	
			
			#include "UnityCG.cginc"		
			#include "../../Lib/EBG_Globals.cginc"
									
			sampler2D _MainTex;		
			float _Steps;	
			float _StepTime;
			fixed _FogFactor;
				
			struct Input 
			{
			    float4 vertex : POSITION;
			    float4 texcoord0 : TEXCOORD0;
			    float4 color : COLOR;
			};
			
			struct VtoS
			{
	          	float4 position : SV_POSITION;	
				float3 uv_MainTex_Fog : TEXCOORD0; 
			};
			
			VtoS vertex_lm(Input v)
			{   
				VtoS data;
				data.position = UnityObjectToClipPos(v.vertex);
				float baseStepOffset = dot(float4(1, 2, 4, 8), v.color);  
				data.uv_MainTex_Fog.xy = v.texcoord0.xy / float2(_Steps, 1.0f); 	//bring it into the base "step" size
				float currentSteps = round(_Time.y / _StepTime) + baseStepOffset;
				float stepOffset = frac( currentSteps / _Steps );
				data.uv_MainTex_Fog.x += stepOffset;							//shift it through the steps
				data.uv_MainTex_Fog.z = EBGFogVertex(v.vertex) * _FogFactor;
				return data;
			}
			
			fixed4 fragment_lm(VtoS IN) : COLOR0
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex_Fog.xy);
				c.rgb = EBGFogFragment(c.rgb, IN.uv_MainTex_Fog.z);
               	c = c * _EBGEnvAdjustScale + _EBGEnvAdjustOffset;
				return c;
			}   
				
			#pragma vertex vertex_lm 
			#pragma fragment fragment_lm 
			
			ENDCG 
		}
	}
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
			#include "../../Lib/EBG_Globals.cginc"
									
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
               	c = c * _EBGEnvAdjustScale + _EBGEnvAdjustOffset;
				return c;
			}   
				
			#pragma vertex vertex_lm 
			#pragma fragment fragment_lm 
			
			ENDCG 
		}
	}
}
