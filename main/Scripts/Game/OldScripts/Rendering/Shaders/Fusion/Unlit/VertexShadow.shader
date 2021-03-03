// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Fusion/Unlit/Vertex Shadow" {	
	Properties{
		_ClipFactor("Clip", float) = 0.0
		_MainColor("Color",Color) = (0.0,0.0,0.0,0.0)
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 100

		Pass{
			Name "SHADOW"
			Tags { "LightMode" = "Vertex" }
			Fog {Mode Off}
			Cull Off
			ZWrite Off
			Blend DstColor Zero
			AlphaTest Greater .001
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			#pragma glsl
			#pragma glsl_no_auto_normalization
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash xbox360 ps3
			//#pragma multi_compile DEATH_OFF DEATH_ON

			float _ClipFactor;
	        float4 _MainColor;

			struct v2f_surf {
				float4 pos : SV_POSITION;
				fixed4 vcolor : TEXCOORD1;
			};

			struct appdata_vert {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};

			v2f_surf vert (appdata_vert v) {
				v2f_surf o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.vcolor = v.color;
				//#ifdef DEATH_OFF
					//o.vcolor.a *= (1 - _ClipFactor*2);
				o.vcolor.a *=  1-_ClipFactor *2;
				//#endif
					//o.vcolor.rgb = (1 - o.vcolor.aaa);
					o.vcolor.rgb =  (1 - o.vcolor.aaa* (1-_MainColor.rgb));
					//o.vcolor.rgb = _MainColor.rgb;
				return o;
			}

			fixed4 frag (v2f_surf IN) : COLOR {
				fixed4 c = IN.vcolor;
				return c;
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
	//CustomEditor "CustomMaterialInspector"
}
