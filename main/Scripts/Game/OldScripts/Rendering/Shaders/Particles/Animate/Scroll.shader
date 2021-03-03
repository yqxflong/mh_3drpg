// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Particle/Animate/Scroll"
{
	Properties 
	{
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base", 2D) = "white" {}
		_ScrollX ("X Scroll Cycle Time (s)", Float) = 0
		_ScrollY ("Y Scroll Cycle Time (s)", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendModeSrc("Source Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendModeDst("Destination Blend Mode", Float) = 1
	}
	Category
	{
		Tags {
			"Queue"="Transparent" 
			"RenderType"="Scroll"
			"LightMode"="ForwardBase"
		}
		Cull Back
		ZWrite On
		ZTest Lequal
		Blend[_BlendModeSrc][_BlendModeDst]
		
		SubShader 
		{
			LOD 100
			
			Pass
			{
				CGPROGRAM	
				
				#include "UnityCG.cginc"	
				#include "../../Lib/EBG_Globals.cginc"
										
				sampler2D _MainTex;	
				float4 _MainTex_ST;
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
					data.uv_MainTex = v.texcoord0.xy * _MainTex_ST.xy + float2(_ScrollX, _ScrollY) * _Time.y;
					return data;
				}
				
				fixed4 fragment_lm(VtoS IN) : COLOR0
				{
					fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	               	c = c * _EBGEnvAdjustScale + _EBGEnvAdjustOffset;
	               	return c;
				}   
					
				#pragma vertex vertex_lm 
				#pragma fragment fragment_lm 
				
				ENDCG 
			}
		}
	}
}
