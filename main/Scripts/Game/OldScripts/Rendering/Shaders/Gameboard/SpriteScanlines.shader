// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Gameboard/SpriteScanlines"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_ScanlineTex ("Scanline Texture", 2D) = "black" {}
		_ScanlineSpeedX ("X Speed", Float) = 0
		_ScanlineSpeedY ("Y Speed", Float) = 0
		_ScanlineAlpha ("Alpha", Float) = 0
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			sampler2D _ScanlineTex;
			float _ScanlineSpeedX;
			float _ScanlineSpeedY;
			float4 _ScanlineTex_ST;
			float _ScanlineAlpha;
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float3 proj		: TEXCOORD1;
			};
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.proj.xyz = OUT.vertex.xyw;
				OUT.color = IN.color;
				OUT.texcoord = IN.texcoord;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;  
				c.rgb *= c.a;
				
				float2 screenUV = (IN.proj.xy * half2(_ScanlineTex_ST.x,_ScanlineTex_ST.y))/IN.proj.z + half2(0.5, 0.5); 
				fixed4 scanLines = tex2D(_ScanlineTex, screenUV + _Time.yy * fixed2(_ScanlineSpeedX,_ScanlineSpeedY));
				return (c * scanLines.a *_ScanlineAlpha );
			}
		ENDCG
		}
	}
}

