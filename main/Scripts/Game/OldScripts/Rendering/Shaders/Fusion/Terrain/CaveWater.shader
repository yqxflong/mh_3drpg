// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Fusion/Terrain/CaveWater" {
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Diffuse Map", 2D) = "white" {}
		_ReflectionColor("Reflection Color", Color) = (0, 0, 0, 0)
		_ReflectionMap("Reflection Map", 2D) = "black" {}
	}
	SubShader {
		Tags { "Queue" = "Geometry+1" }	// because this is not transparent water
		LOD 200	
		Lighting Off
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "../../Lib/EBG_Globals.cginc"
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
	
			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _ReflectionMap;
			float4 _MainTex_ST;
			fixed4 _ReflectionColor;
//			half4 _DistortParameter;
			#ifndef LIGHTMAP_OFF
				// float4 unity_LightmapST;
				float4 unity_LightmapFade;
				// sampler2D unity_Lightmap;
			#endif
			
			struct appdata_color {
				float4 vertex   : POSITION;
				//fixed4 color    : COLOR;
				half3 normal	: NORMAL;
				float4 texcoord	: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float4 texcoord1 : TEXCOORD1;
				#endif
			};
			
			struct v2f {
				float4 pos		: SV_POSITION;
				//fixed4 color	: COLOR0;
				half3 normal	: TEXCOORD0;
				half3 uv		: TEXCOORD1;
				half4 screen	: TEXCOORD2;
				#ifndef LIGHTMAP_OFF
					float2 lmap		: TEXCOORD3;
				#endif
			};
			
			v2f vert (appdata_color v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.screen = ComputeScreenPos(o.pos);
				
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.z = EBGFogVertex(v.vertex);
				//o.color = v.color;
				#ifndef LIGHTMAP_OFF
					o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				o.normal = v.normal;
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{				
				fixed4 diffuseColor = tex2D(_MainTex, i.uv.xy);
				#ifndef LIGHTMAP_OFF
					diffuseColor.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				#endif
				
				fixed3 rtRefl = tex2D(_ReflectionMap, (i.screen.xy / i.screen.w) + i.normal.xy).rgb;
				
			    fixed4 c = diffuseColor * _Color + fixed4(rtRefl.rgb, 0.0f) * _ReflectionColor;// * i.color ;
			    c.rgb = EBGFogFragment(c, i.uv.z);
			    return c;
			}
			ENDCG
		}
	} 
	SubShader {
		Tags { "Queue" = "Geometry+1" }	// because this is not transparent water
		LOD 100	
		Lighting Off
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
	
			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _ReflectionMap;
			float4 _MainTex_ST;
			fixed4 _ReflectionColor;
//			half4 _DistortParameter;
			#ifndef LIGHTMAP_OFF
				// float4 unity_LightmapST;
				float4 unity_LightmapFade;
				// sampler2D unity_Lightmap;
			#endif
			
			struct appdata_color {
				float4 vertex   : POSITION;
				//fixed4 color    : COLOR;
				half3 normal	: NORMAL;
				float4 texcoord	: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float4 texcoord1 : TEXCOORD1;
				#endif
			};
			
			struct v2f {
				float4 pos		: SV_POSITION;
				//fixed4 color	: COLOR0;
				half3 normal	: TEXCOORD0;
				half2 uv		: TEXCOORD1;
				half4 screen	: TEXCOORD2;
				#ifndef LIGHTMAP_OFF
					float2 lmap		: TEXCOORD3;
				#endif
			};
			
			v2f vert (appdata_color v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.screen = ComputeScreenPos(o.pos);
				
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				//o.color = v.color;
				#ifndef LIGHTMAP_OFF
					o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				o.normal = v.normal;
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{				
				fixed4 diffuseColor = tex2D(_MainTex, i.uv.xy);
				#ifndef LIGHTMAP_OFF
					diffuseColor.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				#endif
				
				fixed3 rtRefl = tex2D(_ReflectionMap, (i.screen.xy / i.screen.w) + i.normal.xy).rgb;
				
			    return diffuseColor * _Color + fixed4(rtRefl.rgb, 0.0f) * _ReflectionColor;// * i.color ;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}