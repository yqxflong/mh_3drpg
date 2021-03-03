// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Fusion/Unlit/Transparent Colored Mask (AlphaClip)"
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

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Offset -1, -1
			Fog { Mode Off }
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
				#pragma vertex vertexProgram
				#pragma fragment fragmentProgram

				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex : POSITION;
					half4 color : COLOR;
					float2 textureCoordinate : TEXCOORD0;
				};

				struct vertexToFragment
				{
					float4 vertex : POSITION;
					half4 color : COLOR;
					float2 textureCoordinate : TEXCOORD0;
					float2 worldPos : TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _AlphaTex;

				vertexToFragment vertexProgram(appdata_t vertexData)
				{
					vertexToFragment output;
					output.vertex = UnityObjectToClipPos(vertexData.vertex);
					output.color = vertexData.color;
					output.textureCoordinate = vertexData.textureCoordinate;
					output.worldPos = TRANSFORM_TEX(vertexData.vertex.xy, _MainTex);
					return output;
				}

				half4 fragmentProgram(vertexToFragment input) : COLOR
				{
					// Sample the texture
					half4 computedColor = tex2D(_MainTex, input.textureCoordinate) * input.color;
					half4 alphaGuide = tex2D(_AlphaTex, input.textureCoordinate);

					// Calculates the clipping
					float2 factor = abs(input.worldPos);
					float val = 1.0 - max(factor.x, factor.y);

					if (val < 0.0)
					{
						computedColor.a = 0.0;
					}
					
					computedColor.a *= clamp(alphaGuide, 0.0, 1.0);

					return computedColor;
				}
			ENDCG
		}
	}
}