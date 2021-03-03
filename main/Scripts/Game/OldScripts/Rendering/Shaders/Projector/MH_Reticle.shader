// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'

Shader "Projector/MH_Reticle" {
	Properties{
	   _ReticleTex("Texture", 2D) = "gray" {}
	   _Color("Main Color", Color) = (0,0,0,1)
	}

		Subshader{
		   Tags { "Queue" = "Transparent-100" }
		   Pass {

				ZWrite Off
			 Fog { Mode Off }
			 Blend SrcAlpha OneMinusSrcAlpha

			 CGPROGRAM
			 #pragma vertex vert
			 #pragma fragment frag

			 #include "UnityCG.cginc"
			struct Input
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
					float2 texcoord0 : TEXCOORD0;
				};
			 struct v2f
			 {
			   float4 pos : SV_POSITION;
			   float2 uv_main : TEXCOORD0;
			 };

			 sampler2D _ReticleTex;
			 float4x4 unity_Projector;
			 float4 _Color;

			 v2f vert(Input v) {
			   v2f o;
			   o.pos = UnityObjectToClipPos(v.vertex);
			   o.uv_main = v.texcoord0;
			   return o;
			 }

			 half4 frag(v2f i) : COLOR {
			   float4 mainTex = tex2D(_ReticleTex, i.uv_main.xy);
			   mainTex.a = mainTex.a * _Color.a;
			   return mainTex;
			 }

			 ENDCG
		   }
	   }
}

