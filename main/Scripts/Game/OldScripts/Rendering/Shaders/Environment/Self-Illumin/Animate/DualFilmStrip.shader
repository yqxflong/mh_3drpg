// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Enviro/Animate/DualFilmStrip"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MainTexFrames("Base Frames (X/Y)", Vector) = (2, 2, 0, 0)
		_MainTexFrameTime("Base Frame Time", Float) = 0.5
		_AnimTex ("Top (RGBA)", 2D) = "white" {}
		_AnimTexFrames("Top Frames (X/Y)", Vector) = (2, 2, 0, 0)
		_AnimTexFrameTime("Top Frame Time", Float) = 0.5
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 400
		Cull Back
		ZWrite On
		ZTest Lequal
		Blend Off
		
		Pass
		{
			CGPROGRAM	
			#include "UnityCG.cginc"		
			#include "../../../Lib/EBG_Globals.cginc"
									
			sampler2D _MainTex;	
			float2 _MainTexFrames;
			float _MainTexFrameTime;
				
			sampler2D _AnimTex;	
			float2 _AnimTexFrames;
			float _AnimTexFrameTime;
			
			struct Input 
			{
			    float4 vertex : POSITION;
				float2 uv : TEXCOORD0; 
			};
			
			struct VtoS
			{
	          	float4 position : SV_POSITION;	
				float3 uv_MainTex_Fog : TEXCOORD0; 
				float2 uv_AnimTex : TEXCOORD1; 
			};
			
			float4 _MainTex_ST;
			
			VtoS vertex_lm(Input v)
			{   
				VtoS data;
				data.position = UnityObjectToClipPos(v.vertex);  
				
				float mainFrame = floor(_Time.y / _MainTexFrameTime);
				data.uv_MainTex_Fog.xy = v.uv / _MainTexFrames;
				data.uv_MainTex_Fog.y = 1.0f - data.uv_MainTex_Fog.y;
				data.uv_MainTex_Fog.x += fmod(mainFrame / _MainTexFrames.x, 1.0f);
				data.uv_MainTex_Fog.y -= fmod(floor(mainFrame / _MainTexFrames.x) / _MainTexFrames.y, 1.0f);
				
				float animFrame = floor(_Time.y / _AnimTexFrameTime);
				data.uv_AnimTex.xy = v.uv / _AnimTexFrames;
				data.uv_AnimTex.y = 1.0f - data.uv_AnimTex.y;
				data.uv_AnimTex.x += fmod(animFrame / _AnimTexFrames.x, 1.0f);
				data.uv_AnimTex.y -= fmod(floor(animFrame / _AnimTexFrames.x) / _AnimTexFrames.y, 1.0f);
				
				data.uv_MainTex_Fog.z = EBGFogVertex(v.vertex);
				
				return data;
			}
			
			fixed4 fragment_lm(VtoS IN) : COLOR0
			{
				fixed3 mainTex = tex2D(_MainTex, IN.uv_MainTex_Fog.xy);
				fixed4 animTex = tex2D(_AnimTex, IN.uv_AnimTex);
				fixed3 c = lerp(mainTex, animTex.rgb, animTex.a);
				c = EBGFogFragment(c, IN.uv_MainTex_Fog.z);
				return fixed4(c, 1.0);
			}  
				
			#pragma vertex vertex_lm 
			#pragma fragment fragment_lm 
			
			ENDCG 
		}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull Back
		ZWrite On
		ZTest Lequal
		Blend Off
		
		Pass
		{
			CGPROGRAM	
			#include "UnityCG.cginc"		
			#include "../../../Lib/EBG_Globals.cginc"
									
			sampler2D _MainTex;	
			float2 _MainTexFrames;
			float _MainTexFrameTime;
				
			sampler2D _AnimTex;	
			float2 _AnimTexFrames;
			float _AnimTexFrameTime;
			
			struct Input 
			{
			    float4 vertex : POSITION;
				float2 uv : TEXCOORD0; 
			};
			
			struct VtoS
			{
	          	float4 position : SV_POSITION;	
				float2 uv_MainTex : TEXCOORD0; 
				float2 uv_AnimTex : TEXCOORD1; 
			};
			
			float4 _MainTex_ST;
			
			VtoS vertex_lm(Input v)
			{   
				VtoS data;
				data.position = UnityObjectToClipPos(v.vertex);  
				
				float mainFrame = floor(_Time.y / _MainTexFrameTime);
				data.uv_MainTex.xy = v.uv / _MainTexFrames;
				data.uv_MainTex.y = 1.0f - data.uv_MainTex.y;
				data.uv_MainTex.x += fmod(mainFrame / _MainTexFrames.x, 1.0f);
				data.uv_MainTex.y -= fmod(floor(mainFrame / _MainTexFrames.x) / _MainTexFrames.y, 1.0f);
				
				float animFrame = floor(_Time.y / _AnimTexFrameTime);
				data.uv_AnimTex.xy = v.uv / _AnimTexFrames;
				data.uv_AnimTex.y = 1.0f - data.uv_AnimTex.y;
				data.uv_AnimTex.x += fmod(animFrame / _AnimTexFrames.x, 1.0f);
				data.uv_AnimTex.y -= fmod(floor(animFrame / _AnimTexFrames.x) / _AnimTexFrames.y, 1.0f);
				
				return data;
			}
			
			fixed4 fragment_lm(VtoS IN) : COLOR0
			{
				fixed3 mainTex = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 animTex = tex2D(_AnimTex, IN.uv_AnimTex);
				fixed3 c = lerp(mainTex, animTex.rgb, animTex.a);
				return fixed4(c, 1.0);
			}  
				
			#pragma vertex vertex_lm 
			#pragma fragment fragment_lm 
			
			ENDCG 
		}
	}
}
