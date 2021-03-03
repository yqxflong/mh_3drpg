// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Fusion/Unlit/ReflectionLM" {
	Properties {
		_Color("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_ReflectionColor("Reflection Color", Color) = (0, 0, 0, 0)
		_ReflectionMap("Reflection Map", CUBE) = "" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 300
		Lighting Off
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
	
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _ReflectionColor;
			samplerCUBE _ReflectionMap;
			#ifndef LIGHTMAP_OFF
				// float4 unity_LightmapST;
				float4 unity_LightmapFade;
				// sampler2D unity_Lightmap;
			#endif
			
			struct appdata_color {
				float4 vertex   : POSITION;
				fixed4 color    : COLOR;
				half3 normal	: NORMAL;
				float4 texcoord	: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float4 texcoord1 : TEXCOORD1;
				#endif
			};
			
			struct v2f {
				float4 pos		: SV_POSITION;
				fixed4 color	: COLOR0;
				half2 uv		: TEXCOORD0;
				half3 worldRefl : TEXCOORD1;
				#ifndef LIGHTMAP_OFF
					float2 lmap		: TEXCOORD2;
				#endif
			};
			
			v2f vert (appdata_color v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				
				// calculate the view reflection in world space
				float3 viewDir = -ObjSpaceViewDir(v.vertex);
				float3 viewRefl = reflect (viewDir, v.normal);
				o.worldRefl = mul ((float3x3)unity_ObjectToWorld, viewRefl);
  
				#ifndef LIGHTMAP_OFF
					o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
			    fixed4 texcol = tex2D(_MainTex, i.uv);
				#ifndef LIGHTMAP_OFF
					texcol.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				#endif
				
				fixed3 reflection = texCUBE(_ReflectionMap, i.worldRefl).rgb * _ReflectionColor.rgb;
				
			    return texcol * i.color * _Color + fixed4(reflection.rgb, 0.0f);
			}
			ENDCG
		}
	} 
	FallBack "Fusion/Unlit/VertexColorLM"
}