// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Gameboard/BackgroundLuminance" 
{

	Properties 
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_LuminanceTex ("Luminance Texture", 2D) = "white" {}
		_LuminanceSpeed ("Speed (X,Y)", Vector) = (0, 0, 0, 0)	
		_Intensity("Intensity", Float) = 1
		_Gyro("Gyro Modulation", Float) = 1
		_Offset("Gyro Offset", Float) = 0.5
	}
	Category
	{
	
		Tags { "Queue"="Geometry" "RenderType"="Transparent" }
		Cull Off
		ZWrite Off
		ZTest LEqual
		Blend Off
		
		SubShader 
		{
			LOD 200
			Pass
			{
				CGPROGRAM
				
				#pragma exclude_renderers xbox360 ps3 flash 
				
				#include "UnityCG.cginc"
				#include "../Lib/EBG_Globals.cginc"
				
				sampler2D _MainTex;
				sampler2D _LuminanceTex;
				float _Intensity;
				float _Gyro;
				float _Offset;
				fixed2 _LuminanceSpeed;
				struct Input 
				{
				    float4 vertex : POSITION;
				    float4 texcoord0 : TEXCOORD0;
				};
				
				float4 _MainTex_ST;
				float4 _LuminanceTex_ST;
				
				struct VtoS
				{
		          	float4 position : SV_POSITION;	
					float2 uv_MainTex : TEXCOORD0; 
					float2 uv_LuminanceTex : TEXCOORD1; 
				};
				
				VtoS vertex_lm(Input v)
				{   
					VtoS data;
					data.position = UnityObjectToClipPos(v.vertex); 
					float val = (1 - _Gyro) * 0.5;
					float2 gyroData = float2(_Gyroscope.x * (1 / _Offset), (_Gyroscope.y + _Offset) * (1 / _Offset));
					gyroData = gyroData * 0.5 + 0.5;
					float x = lerp(-val, val, saturate(gyroData.x));
					float y = lerp(-val, val, saturate(gyroData.y));
					data.uv_MainTex = v.texcoord0.xy * _Gyro + float2(val + x, val + y);
					data.uv_LuminanceTex = v.texcoord0.xy * _Gyro + float2(val + x, val + y) + _Time.yy * _LuminanceSpeed; 
					return data;
				}
				
				fixed4 fragment_lm(VtoS IN) : COLOR0
				{
					fixed4 main = tex2D(_MainTex, IN.uv_MainTex);
					fixed4 lumi = tex2D(_LuminanceTex, IN.uv_LuminanceTex);
					return main * lumi * _Intensity;
				}   
				
				#pragma vertex vertex_lm 
				#pragma fragment fragment_lm 
					
				ENDCG
			}
		} 
		
		SubShader 
		{
			LOD 100
			Pass
			{
				CGPROGRAM
				
				#pragma exclude_renderers xbox360 ps3 flash 
				
				#include "UnityCG.cginc"
				#include "../Lib/EBG_Globals.cginc"
				
				sampler2D _MainTex;
				float _Intensity;
				float _Gyro;
				float _Offset;
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
					float val = (1 - _Gyro) * 0.5;
					float2 gyroData = float2(_Gyroscope.x * (1 / _Offset), (_Gyroscope.y + _Offset) * (1 / _Offset));
					gyroData = gyroData * 0.5 + 0.5;
					float x = lerp(-val, val, saturate(gyroData.x));
					float y = lerp(-val, val, saturate(gyroData.y));
					data.uv_MainTex = v.texcoord0.xy * _Gyro + float2(val + x, val + y);
					return data;
				}
				
				fixed4 fragment_lm(VtoS IN) : COLOR0
				{
					fixed4 main = tex2D(_MainTex, IN.uv_MainTex);
					return main * _Intensity;
				}   
				
				#pragma vertex vertex_lm 
				#pragma fragment fragment_lm 
					
				ENDCG
			}
		} 
	}
	FallBack "Diffuse"
}
