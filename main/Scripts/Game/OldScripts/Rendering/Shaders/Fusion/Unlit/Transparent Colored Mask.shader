// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Fusion/Unlit/Transparent Colored Mask"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
		_AlphaTex ("MaskTexture", 2D) = "white" {}
	}
 
	SubShader
	{
		LOD 100
 
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
 
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha
 
		Pass
		{
			CGPROGRAM
				#pragma vertex vertexProgram
				#pragma fragment fragmentProgram
 
				#include "UnityCG.cginc"
 
				struct appdata_t
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 textureCoordinate : TEXCOORD0;
				};
 
				struct vertexToFragment
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					half2 textureCoordinate : TEXCOORD0;
				};
 
				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _AlphaTex;
 
				vertexToFragment vertexProgram(appdata_t vertexData)
				{
					vertexToFragment output;
					output.vertex = UnityObjectToClipPos(vertexData.vertex);
					output.textureCoordinate = TRANSFORM_TEX(vertexData.textureCoordinate, _MainTex);
					output.color = vertexData.color;
					return output;
				}
 
				fixed4 fragmentProgram(vertexToFragment input) : COLOR
				{
					fixed4 computedColor = tex2D(_MainTex, input.textureCoordinate) * input.color;
					fixed4 alphaGuide = tex2D(_AlphaTex, input.textureCoordinate);

					computedColor.a *= clamp(alphaGuide, 0.0, 1.0);
 
					return computedColor;
				}
			ENDCG
		}
	}
}