// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Misc/HideColor"
{
	Properties
	{
		[Whitespace] _Whitespace("Color", Float) = 0
		_Color("Color Scale", Color) = (0, 1, 1, 0.5)

		[Whitespace] _Whitespace("Blending", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendModeSrc("Source Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendModeDst("Destination Blend Mode", Float) = 1
	}
	SubShader
	{
		Tags {
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
			"LightMode" = "ForwardBase"
		}
		LOD 300

		Cull Back
		Lighting Off
		ZWrite Off
		ZTest Greater
		Blend [_BlendModeSrc] [_BlendModeDst]
		//Blend OneMinusDstColor One
		Fog { Mode Off }

		Pass
		{
			Stencil {
				Ref 4
				Comp NotEqual
				Pass Keep
				Fail Keep
				ZFail Keep
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma target 3.0
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return _Color;
			}
			ENDCG
		}
	}
}
