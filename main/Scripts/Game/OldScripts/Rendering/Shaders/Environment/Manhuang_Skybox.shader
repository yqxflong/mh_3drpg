// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//印庭欣qq17709982创建于2016.3.18
Shader "ManHuang/Scene/SkyBox" {
Properties {
	_Tint ("Tint Color", Color) = (.5, .5, .5, .5)
	_FrontTex ("Front (+Z)", 2D) = "white" {}
	
}

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" }
	Cull Back ZWrite Off Fog { Mode Off }
	
	CGINCLUDE
	#include "UnityCG.cginc"

	fixed4 _EBGTint;
	fixed4 _Tint;
	struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};
	struct v2f {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};
	v2f vert (appdata_t v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = v.texcoord;
		return o;
	}
	fixed4 skybox_frag (v2f i, sampler2D smp)
	{
		fixed4 tex = tex2D (smp, i.texcoord);
		fixed4 col;
		_EBGTint.rgb = _Tint.rgb - _EBGTint.rgb;
		col.rgb = tex.rgb + _EBGTint.rgb - unity_ColorSpaceGrey;
		col.a = tex.a * _EBGTint.a;
		return col;
	}
	ENDCG
	
	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		sampler2D _FrontTex;
		fixed4 frag (v2f i) : COLOR { return skybox_frag(i,_FrontTex); }
		ENDCG 
	}
	
}	

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" }
	Cull Back ZWrite Off Fog { Mode Off }
	Color [_EBGTint]
	Pass {
		SetTexture [_FrontTex] { combine texture +- primary, texture * primary }
	}
	
}
}