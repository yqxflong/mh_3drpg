// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Enviro/Unlit/TransparentPush"
{
	Properties 
	{
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGBA) Mask (A)", 2D) = "white" {}
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendModeSrc ("Source Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendModeDst ("Destination Blend Mode", Float) = 1
	}
	Category
	{
		Tags {
			"Queue"="Transparent+100" 
			"RenderType"="EnvironmentTransparent"
			"LightMode"="ForwardBase"
		}
		Cull Back
		ZWrite On
		Blend [_BlendModeSrc] [_BlendModeDst]
		
		SubShader 
		{
			LOD 100
			
			Pass
			{
				ColorMask 0
				ZTest Lequal
				
				CGPROGRAM	
				
				#include "UnityCG.cginc"	
										
					
				struct Input 
				{
				    float4 vertex : POSITION;
				};
				
				struct VtoS
				{
		          	float4 position : SV_POSITION;	
				};
				
				VtoS vertex_lm(Input v)
				{   
					VtoS data;
					data.position = UnityObjectToClipPos(v.vertex);  
					return data;
				}
				
				fixed4 fragment_lm(VtoS IN) : COLOR0
				{
					return 1;
				}   
					
				#pragma vertex vertex_lm 
				#pragma fragment fragment_lm 
				
				ENDCG 
			}
			
			Pass
			{
				ZTest Equal
				
				CGPROGRAM	
				
				#include "UnityCG.cginc"	
				#include "../../Lib/EBG_Globals.cginc"
										
				sampler2D _MainTex;	
				float4 _MainTex_ST;	
				fixed4 _Color;
					
				struct Input 
				{
				    float4 vertex : POSITION;
				    float4 texcoord0 : TEXCOORD0;
				};
				
				struct VtoS
				{
		          	float4 position : SV_POSITION;	
					float2 uv_MainTex : TEXCOORD0; 
					fixed4 col : COLOR;
				};
				
				VtoS vertex_lm(Input v)
				{   
					VtoS data;
					data.position = UnityObjectToClipPos(v.vertex);  
					data.uv_MainTex = v.texcoord0.xy; 
					data.col = _EBGEnvAdjustScale * _Color;
					return data;
				}
				
				fixed4 fragment_lm(VtoS IN) : COLOR0
				{
					fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
					fixed4 c = mainTex * IN.col;
	               	c.rgb += _EBGEnvAdjustOffset.rgb;
					return c;
				}   
					
				#pragma vertex vertex_lm 
				#pragma fragment fragment_lm 
				
				ENDCG 
			}
		}
	}
}
