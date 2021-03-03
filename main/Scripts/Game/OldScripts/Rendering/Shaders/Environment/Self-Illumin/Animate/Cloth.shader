// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable

Shader "EBG/Enviro/Animate/Cloth"
{
	Properties
	{
		_MainTex("Diffuse Tex", 2D) = "black" {}
		
		_Color("Diffuse Color", Color) = (0, 0, 0, 1)
		
		_TimeOffset("Time Offset", Float) = 0
		
		_YSpeed1("Y Speed 1", Float) = 50
		_YFrequency1("Y Frequency 1", Float) = 10
		_YAmplitudeScale1("Y Amplitude Scale 1", Float) = 1
		_YAmplitudePower1("Y Amplitude Power 1", Float) = 1
		
		_YSpeed2("Y Speed 2", Float) = 45
		_YFrequency2("Y Frequency 2", Float) = 7
		_YAmplitudeScale2("Y Amplitude Scale 2", Float) = 1
		_YAmplitudePower2("Y Amplitude Power 2", Float) = 1
		
		_TwistSpeed("Twist Speed", Float) = 55
		_TwistFrequency("Twist Frequency", Float) = 4
		_TwistAmplitudeScale("Twist Amplitude Scale", Float) = 1
		_TwistAmplitudePower("Twist Amplitude Power", Float) = 1
		
		_LateralSpeed("Lateral Speed", Float) = 40
		_LateralFrequency("Lateral Frequency", Float) = 2
		_LateralAmplitudeScale("Lateral Amplitude Scale", Float) = 0.25
		_LateralAmplitudePower("Lateral Amplitude Power", Float) = 2
		_LateralAmplitudeOffset("Lateral Amplitude Offset", Float) = 0.25
		
		_NormalScale("Normal Scale", Float) = 0.3
	}
	SubShader 
	{		
		Tags {
			"Queue"="Geometry" 
			"RenderType"="Cloth"
			"LightMode"="ForwardBase"
		} 
		LOD 100			
		Cull Back
		Lighting On
		ZWrite On
		ZTest Lequal
		Blend Off
		
		Pass
		{
			CGPROGRAM  

			#include "UnityCG.cginc"	
			#include "../../../Lib/EBG_Globals.cginc"

			sampler2D _MainTex;	
			// sampler2D unity_Lightmap;
			
			float _TimeOffset;
			
			fixed4 _Color;
			float _YSpeed1;
			float _YFrequency1;
			float _YAmplitudeScale1;
			float _YAmplitudePower1;
			
			float _YSpeed2;
			float _YFrequency2;
			float _YAmplitudeScale2;
			float _YAmplitudePower2;
			
			float _TwistSpeed;
			float _TwistFrequency;
			float _TwistAmplitudeScale;
			float _TwistAmplitudePower;
			
			float _LateralSpeed;
			float _LateralFrequency;
			float _LateralAmplitudeScale;
			float _LateralAmplitudePower;
			float _LateralAmplitudeOffset;
			
			float _NormalScale;
							
			struct Input 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 texcoord0 : TEXCOORD0; 
			};

			struct VtoS
			{
				float4 position : SV_POSITION;	
				float2 uv : TEXCOORD0;
				float3 nDotL : TEXCOORD1;
			};
				
			float4 _MainTex_ST;
			// float4 unity_LightmapST;

			VtoS vertex_shader(Input v)
			{
				VtoS data;
				
				float4 pos = v.vertex;
				
				float time = _Time.x + _TimeOffset;
				
				float yOffsetComp1 = time * _YSpeed1 + pow(v.texcoord0.y, _YAmplitudePower1) * _YFrequency1;
				float yOffsetComp2 = time * _YSpeed2 + pow(v.texcoord0.y, _YAmplitudePower2) * _YFrequency2;
				float twistOffsetComp = time * _TwistSpeed + pow(v.texcoord0.x, _TwistAmplitudePower) * _TwistFrequency;
				float lateralOffsetComp = time * _LateralSpeed + pow(v.texcoord0.y, _LateralAmplitudePower) * _LateralFrequency;
				
				float yOffset1 = sin(yOffsetComp1) * _YAmplitudeScale1;
				float yOffset2 = sin(yOffsetComp2) * _YAmplitudeScale2;
				float twistOffset = sin(twistOffsetComp) * _TwistAmplitudeScale;
				float lateralOffset = sin(lateralOffsetComp) * _LateralAmplitudeScale;
				pos.y += v.texcoord0.y * (yOffset1 + yOffset2);
				pos.y += v.texcoord0.y * twistOffset;
				pos.x += v.texcoord0.y * (lateralOffset + _LateralAmplitudeOffset);
				
				data.position = UnityObjectToClipPos(pos);
				
				data.uv = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw; 
				
				float yNormOffset1 = cos(yOffsetComp1) * _YAmplitudeScale1;
				float yNormOffset2 = cos(yOffsetComp2) * _YAmplitudeScale2;
				float twistNormOffset = cos(twistOffsetComp) * _TwistAmplitudeScale;
				float3 normal = v.normal;
				normal.z -= v.texcoord0.y * (yNormOffset1 + yNormOffset2) * _NormalScale;
				normal.z -= v.texcoord0.y * twistNormOffset * _NormalScale;
				normal = normalize(normal);
				
				float3 localSurface2World0 = normalize(mul((float3x3)unity_ObjectToWorld, v.tangent.xyz));
				float3 localSurface2World2 = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
				float3 localSurface2World1 = normalize(cross(localSurface2World2, localSurface2World0) * v.tangent.w);
				float3x3 localSurface2World = float3x3(-localSurface2World0, localSurface2World2, localSurface2World1);
				float3 n = mul(normal, localSurface2World);
				float nDotL = abs(dot(n, _EBGEnvDirectionToLight0.xyz));
				data.nDotL =  (2.0 * nDotL) * _Color.rgb;
				
				return data;
			}

			fixed4 fragment_shader(VtoS IN) : COLOR0
			{ 
				fixed3 mainTex = tex2D(_MainTex, IN.uv).rgb;
				fixed4 c = fixed4(mainTex * IN.nDotL, 0);
				c = c * _EBGEnvAdjustScale + _EBGEnvAdjustOffset;
				return c;
			}
			
			#pragma vertex vertex_shader
			#pragma fragment fragment_shader
	
			ENDCG
		}
	}
}