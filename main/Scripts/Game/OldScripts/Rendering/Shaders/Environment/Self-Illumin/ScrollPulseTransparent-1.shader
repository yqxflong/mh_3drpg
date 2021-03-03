// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Enviro/Self-Illumin/ScrollPulseTransparent-1"
{
	Properties 
	{
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGBA) Mask (A)", 2D) = "white" {}
		_AnimTex ("Anim (RGBA)", 2D) = "white" {}
		_ScrollX ("X Scroll Cycle Time (s)", Float) = 0
		_ScrollY ("Y Scroll Cycle Time (s)", Float) = 0
		_Pulse ("Pulse Time (s)", Float) = 0
		_MinPulseColor ("Min Pulse Color", Color) = (0, 0, 0, 0)
		_MaxPulseColor ("Max Pulse Color", Color) = (1, 1, 1, 1)
		_EmissionLM ("Emission (Lightmapper)", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendModeSrc ("Source Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendModeDst ("Destination Blend Mode", Float) = 1
	}
	Category
	{
		Tags {
			"Queue"="Transparent-1" 
			"RenderType"="ScrollPulseTransparent"
			"LightMode"="ForwardBase"
		}
		Cull Back
		ZWrite Off
		ZTest Lequal
		Blend [_BlendModeSrc] [_BlendModeDst]
		
		SubShader 
		{
			LOD 200
			
			Pass
			{
				CGPROGRAM	
				
				#include "UnityCG.cginc"	
				#include "../../Lib/EBG_Globals.cginc"
										
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
					data.uv_MainTex = TRANSFORM_TEX(v.texcoord0.xy, _MainTex); 
					data.uv_AnimTex = data.uv_MainTex + float2(_ScrollX, _ScrollY) * _Time.y; 
					data.color = _Color * lerp(_MinPulseColor, _MaxPulseColor, cos(_Pulse * _Time.y * 3.14159) * 0.5 + 0.5);
					return data;
				}
				
				fixed4 fragment_lm(VtoS IN) : COLOR0
				{
					fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
					fixed4 animTex = tex2D(_AnimTex, IN.uv_AnimTex);
					fixed4 c = mainTex * animTex * IN.color;
	               	c.rgb = c.rgb * _EBGEnvAdjustScale.rgb + _EBGEnvAdjustOffset.rgb;
					return c;
				}   
					
				#pragma vertex vertex_lm 
				#pragma fragment fragment_lm 
				
				ENDCG 
			}
		}
	}
}
