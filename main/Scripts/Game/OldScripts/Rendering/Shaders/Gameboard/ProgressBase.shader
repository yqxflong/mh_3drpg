// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Gameboard/ProgressBase" 
{
	Properties 
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}	
		_Speed("Speed", Float) = 500
	}
	
	SubShader 
	{
		Tags { "Queue"="Geometry+10" "RenderType"="Transparent" }
		Cull Off
		ZWrite Off
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
			CGPROGRAM
			
			#pragma exclude_renderers xbox360 ps3 flash 
			
			#include "UnityCG.cginc"
			

			sampler2D _MainTex;
			float _Speed;
			
			struct Input 
			{
			    float4 vertex : POSITION;
			    float4 texcoord0 : TEXCOORD0;
			};
			
			float4 _MainTex_ST;
			
			struct VtoS
			{
	          	float4 position : SV_POSITION;	
	          	float2 uv_MainTex : TEXCOORD0;
			};
			
			VtoS vertex_lm(Input v)
			{   
				VtoS data;
				data.position = UnityObjectToClipPos(v.vertex); 			
				
				data.uv_MainTex = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw; 
				float2 speed = float2(_Speed, 0.0F) * _Time;
				data.uv_MainTex = data.uv_MainTex - speed;
				
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
	FallBack "Diffuse"
}
