// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Fusion/Unlit/Texture Color Desaturation"
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
		
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"
	
				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
				};
	
				struct v2f
				{
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
				};
	
				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _DesaturationAmount;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.color = v.color;
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

					return col;
				}
			ENDCG
		}
	}
}
