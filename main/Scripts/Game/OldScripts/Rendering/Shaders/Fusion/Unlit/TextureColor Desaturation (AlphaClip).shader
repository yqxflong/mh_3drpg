// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Fusion/Unlit/Texture Color Desaturation (AlphaClip)"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_DesaturationAmount ("Desaturation Amount", Range(0.0, 1.0)) = 0.0
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
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _DesaturationAmount;

			struct appdata_t
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 worldPos : TEXCOORD1;
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				o.worldPos = TRANSFORM_TEX(v.vertex.xy, _MainTex);
				return o;
			}

			half4 frag (v2f i) : COLOR
			{
				// Get the texture color
				half4 texcol = tex2D (_MainTex, i.texcoord);

				// Calculate the average color of the pixels and
				// create the desaturated color
				float avg = 0.33333333 * (texcol.r + texcol.g + texcol.b);
				half4 desatcol = half4(avg, avg, avg, texcol.a);

				half4 col = lerp(texcol, desatcol, _DesaturationAmount) * i.color;

				// Calculates the clipping
				float2 factor = abs(i.worldPos);
				float val = 1.0 - max(factor.x, factor.y);

				if (val < 0.0)
				{
					col.a = 0.0;
				}

				return col;
			}
			ENDCG
		}
	}
}
