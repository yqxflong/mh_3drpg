// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Fusion/Particles/UVs Alpha" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		[MaterialToggle] EBG_HEIGHT_FOG ("Enable fog", Float) = 1
	}
	SubShader {
		Pass {
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			Lighting Off
			Zwrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			Fog { Mode Off }
			LOD 200
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "../Lib/EBG_Globals.cginc"
			
			#pragma multi_compile EBG_HEIGHT_FOG_OFF EBG_HEIGHT_FOG_ON
			
			fixed4 _Color;
			sampler2D _MainTex;
			
			struct v2f {
				float4  pos : SV_POSITION;
				#ifdef EBG_HEIGHT_FOG_ON
				float3  uv_fog : TEXCOORD0;
				#else
				float2  uv_fog : TEXCOORD0;
				#endif
				fixed4 	color : TEXCOORD1;
			};
			
			float4 _MainTex_ST;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv_fog.xy = TRANSFORM_TEX (v.texcoord, _MainTex);
				#ifdef EBG_HEIGHT_FOG_ON
				o.uv_fog.z = EBGFogVertex(v.vertex);
				#endif
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.uv_fog.xy) * i.color * _Color;
				#ifdef EBG_HEIGHT_FOG_ON
				col.rgb = EBGFogFragment(col.rgb, i.uv_fog.z * col.a);
				#endif
				return col;
			}
			ENDCG
		}
	}
	SubShader {
		Pass {
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			Lighting Off
			Zwrite Off
			Cull Off
			Blend One OneMinusSrcAlpha
			Fog { Mode Off }
			LOD 100
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			float4 _Color;
			sampler2D _MainTex;
			
			struct v2f {
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};
			
			float4 _MainTex_ST;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				return tex2D(_MainTex, i.uv) * _Color;
			}
			ENDCG
		}
	}
	Fallback "VertexLit"
} 