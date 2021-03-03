// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Gameboard/Progress" 
{
	Properties 
	{
		 [PerRendererData] _MainTex ("MainTex", 2D) = "black" {}
		_Progress("Progress", Float) = 0.0
		_Alpha("Alpha", Float) = 1
		_Fade("Fade Distance", Float) = 0.1
		_Speed("Speed", Float) = 500
	}
	
	SubShader 
	{
		Tags { "Queue"="Geometry+10" "RenderType"="Transparent" }
		LOD 100
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
			float _Progress;
			float _Fade;
			float _Alpha;
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
				float fadeAmount : TEXCOORD2; 
			};
			
			VtoS vertex_lm(Input v)
			{   
				VtoS data;
				data.position = UnityObjectToClipPos(v.vertex); 			
				
				data.uv_MainTex = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw; 
				
				float2 speed = float2(_Speed, 0.0F) * _Time;
				data.uv_MainTex = data.uv_MainTex + speed;
				
				float fadeMod = abs(2 * _Fade) + 0.001;
				data.fadeAmount = ((data.uv_MainTex.x - _Speed * _Time) + fadeMod - (_Progress * (1 + fadeMod))) / fadeMod;
				
				return data;
			}
			
			fixed4 fragment_lm(VtoS IN) : COLOR0
			{
				//fixed4 unfill = float4(0,0,0,0);
				fixed4 fill = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 c = lerp(fill,0, saturate(IN.fadeAmount));
				return c * _Alpha;
			}   
			
			#pragma vertex vertex_lm 
			#pragma fragment fragment_lm 
				
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
