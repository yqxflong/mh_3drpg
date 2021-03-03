// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Gameboard/SpriteHueShift"
{
	Properties
	{
		[PerRendererData]_MainTex ("Sprite Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "black" {}	
		_HueShift ("Hue Shift", Float) = 0
		_Sat ("Saturation", Float) = 1
		_Value ("Value", Float) = 1
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Geometry" 
			"RenderType"="Transparent" 
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _MaskTex;
			fixed4 _HueShiftR;
			fixed4 _HueShiftG;
			fixed4 _HueShiftB;
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float2 texcoord  : TEXCOORD0;
			};
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 main = tex2D(_MainTex, IN.texcoord); 
				fixed4 mask = tex2D(_MaskTex, IN.texcoord); 
				fixed3 c;
				c.r = dot(main, _HueShiftR);
				c.g = dot(main, _HueShiftG);
				c.b = dot(main, _HueShiftB);
				fixed3 f = lerp(main, c, mask.a);
				return fixed4(f.rgb, main.a);
			}
		ENDCG
		}
	}
}

