// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Gameboard/TextureHueShift" 
{
	Properties 
	{
		_MainTex ("Main Texture", 2D) = "white" {}		
		_HueShift ("Hue Shift", Float) = 0
		_Sat ("Saturation", Float) = 1
		_Value ("Value", Float) = 1
	}
	
	SubShader 
	{
		Tags { "Queue"="Geometry" "RenderType"="Transparent" }
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
			fixed4 _HueShiftR;
			fixed4 _HueShiftG;
			fixed4 _HueShiftB;

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
				return data;
			}
			
			fixed4 fragment_lm(VtoS IN) : COLOR0
			{
				fixed4 main = tex2D(_MainTex, IN.uv_MainTex);
				
				fixed r = dot(main, _HueShiftR);
				fixed g = dot(main, _HueShiftG);
				fixed b = dot(main, _HueShiftB);
				return fixed4(r, g, b, main.a);
			}   
			
			#pragma vertex vertex_lm 
			#pragma fragment fragment_lm 
				
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
