// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 Shader "EBG/Particle/Blend Premultiply" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		[Enum(UnityEngine.Rendering.CompareFunction)] _DepthTest ("Depth test", Float) = 4
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100
		Cull Off
		ZWrite Off
		ZTest [_DepthTest]
		Blend One OneMinusSrcAlpha
		
		Pass
		{
			CGPROGRAM	
			
			#include "UnityCG.cginc"	
									
			sampler2D _MainTex;	
				
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
				fixed4 color : TEXCOORD1;
			};
			
			float4 _MainTex_ST;
			
			VtoS vertex_lm(Input v)
			{   
				VtoS data;
				data.position = UnityObjectToClipPos(v.vertex);  
				data.uv_MainTex = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw; 
				data.color = v.color * v.color.a;
				return data;
			}
			
			fixed4 fragment_lm(VtoS IN) : COLOR0
			{
				return tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			}   
				
			#pragma vertex vertex_lm 
			#pragma fragment fragment_lm 
			
			ENDCG 
		}
	}
}
