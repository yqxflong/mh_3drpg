// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Transparent/SimpleAlphaBlend" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

CGINCLUDE

	#include "UnityCG.cginc"
	
	uniform sampler2D _MainTex;
	uniform half4 _MainTex_ST;

	struct appdata {
		float4 vertex : POSITION;
		half2 texcoord : TEXCOORD0;		
	};

	struct v2f {
		float4 pos : SV_POSITION;
		half2	uv : TEXCOORD0;
	};	

	v2f vert (appdata v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos (v.vertex);		
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);	
		return o;
	}
	
	half4 frag (v2f i) : COLOR
	{
		half4 texcol = tex2D( _MainTex, i.uv );
		texcol = texcol;
		return texcol;
	}

	ENDCG

	
SubShader {
	LOD 100
	Tags { "Queue" = "Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Cull Off
	Lighting Off
	Fog { Mode Off }

    Pass {
	ZWrite Off
	AlphaTest Off
	Blend One OneMinusSrcAlpha

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	
	ENDCG
    }
}
Fallback Off
} 