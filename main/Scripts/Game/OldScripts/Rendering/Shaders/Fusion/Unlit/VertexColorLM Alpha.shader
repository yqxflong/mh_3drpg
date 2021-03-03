// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Fusion/Unlit/VertexColorLM Alpha" {
	Properties {
		_Color("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
	
	//Dual pass rendering to hide fading artifacts
	SubShader {
		Tags { "Queue"="Transparent" }
		LOD 300
		Cull Back
		Lighting Off
		Fog { Mode Off }
		
		
		Pass {
			ZWrite On
			Blend Off
			ColorMask 0
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
	
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			#ifndef LIGHTMAP_OFF
				// float4 unity_LightmapST;
				float4 unity_LightmapFade;
				// sampler2D unity_Lightmap;
			#endif
			
			struct appdata_color {
				float4 vertex   : POSITION;
				fixed4 color    : COLOR;
				float4 texcoord	: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float4 texcoord1 : TEXCOORD1;
				#endif
			};
			
			struct v2f {
				float4 pos		: SV_POSITION;
			};
			
			v2f vert (appdata_color v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
			    return 0;
			}
			ENDCG
		}
		
		Pass {
			ZWrite On
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Equal
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "../../Lib/EBG_Globals.cginc"
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
	
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			#ifndef LIGHTMAP_OFF
				// float4 unity_LightmapST;
				float4 unity_LightmapFade;
				// sampler2D unity_Lightmap;
			#endif
			
			struct appdata_color {
				float4 vertex   : POSITION;
				fixed4 color    : COLOR;
				float4 texcoord	: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float4 texcoord1 : TEXCOORD1;
				#endif
			};
			
			struct v2f {
				float4 pos		: SV_POSITION;
				fixed4 color	: COLOR0;
				half3 uv_fog	: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float2 lmap		: TEXCOORD1;
				#endif
			};
			
			v2f vert (appdata_color v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv_fog.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv_fog.z = EBGFogVertex(v.vertex);
				//float3 worldPos = mul(_Object2World, v.vertex);
				//float heightFade = max(0.1f, 5.5f - worldPos.y);
				//_Color.a = lerp(heightFade, 1, _Color.a);
				o.color = v.color * _Color;
				#ifndef LIGHTMAP_OFF
					o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
			    fixed4 texcol = tex2D(_MainTex, i.uv_fog.xy);
				#ifndef LIGHTMAP_OFF
					texcol.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				#endif
			    fixed4 c = texcol * i.color;
			    c.rgb = EBGFogFragment(c, i.uv_fog.z);
			    return c;
			}
			ENDCG
		}
	} 
	
	//FOG
	SubShader {
		Tags { "Queue"="Transparent" }
		LOD 200
		Cull Back
		Lighting Off
		Fog { Mode Off }
		
		Pass {
			ZWrite On
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest LEqual
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "../../Lib/EBG_Globals.cginc"
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
	
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			#ifndef LIGHTMAP_OFF
				// float4 unity_LightmapST;
				float4 unity_LightmapFade;
				// sampler2D unity_Lightmap;
			#endif
			
			struct appdata_color {
				float4 vertex   : POSITION;
				fixed4 color    : COLOR;
				float4 texcoord	: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float4 texcoord1 : TEXCOORD1;
				#endif
			};
			
			struct v2f {
				float4 pos		: SV_POSITION;
				fixed4 color	: COLOR0;
				half3 uv_fog	: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float2 lmap		: TEXCOORD1;
				#endif
			};
			
			v2f vert (appdata_color v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv_fog.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv_fog.z = EBGFogVertex(v.vertex);
				//float3 worldPos = mul(_Object2World, v.vertex);
				//float heightFade = max(0.1f, 5.5f - worldPos.y);
				//_Color.a = lerp(heightFade, 1, _Color.a);
				o.color = v.color * _Color;
				#ifndef LIGHTMAP_OFF
					o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
			    fixed4 texcol = tex2D(_MainTex, i.uv_fog.xy);
				#ifndef LIGHTMAP_OFF
					texcol.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				#endif
			    fixed4 c = texcol * i.color;
			    c.rgb = EBGFogFragment(c, i.uv_fog.z);
			    return c;
			}
			ENDCG
		}
	} 
	
	SubShader {
		Tags { "Queue"="Transparent" }
		LOD 100
		Cull Back
		Lighting Off
		Fog { Mode Off }
		
		Pass {
			ZWrite On
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest LEqual
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
	
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			#ifndef LIGHTMAP_OFF
				// float4 unity_LightmapST;
				float4 unity_LightmapFade;
				// sampler2D unity_Lightmap;
			#endif
			
			struct appdata_color {
				float4 vertex   : POSITION;
				fixed4 color    : COLOR;
				float4 texcoord	: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float4 texcoord1 : TEXCOORD1;
				#endif
			};
			
			struct v2f {
				float4 pos		: SV_POSITION;
				fixed4 color	: COLOR0;
				half2 uv		: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float2 lmap		: TEXCOORD1;
				#endif
			};
			
			v2f vert (appdata_color v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float heightFade = max(0.1f, 5.5f - worldPos.y);
				_Color.a = lerp(heightFade, 1, _Color.a);
				o.color = v.color * _Color;
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
			    return texcol * i.color;
			}
			ENDCG
		}
	} 
}
