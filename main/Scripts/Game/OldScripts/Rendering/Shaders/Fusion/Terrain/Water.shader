// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Fusion/Terrain/Water" {
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
		_DistortParameter("Distrotion Intensity, Distrotion Speed", Vector) = (0.1, 0.5, 0.0, 0.0)
		//_DetailDis("Detail Distance", Float ) = 320.0
		_MainTex("Diffuse Map", 2D) = "white" {}
		_DistortMap("Distortion Map", 2D) = "bump" {}
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
			sampler2D _DistortMap;
			float4 _MainTex_ST;
			half4 _DistortParameter;
			#ifndef LIGHTMAP_OFF
				// float4 unity_LightmapST;
				float4 unity_LightmapFade;
				// sampler2D unity_Lightmap;
			#endif
			
			struct appdata_color {
				float4 vertex   : POSITION;
				float4 texcoord	: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float4 texcoord1 : TEXCOORD1;
				#endif
			};
			
			struct v2f {
				float4 pos		: SV_POSITION;
				float4 uv		: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float2 lmap		: TEXCOORD1;
				#endif
			};
			
			v2f vert (appdata_color v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.z = sin(_DistortParameter.y * _Time.y) * _DistortParameter.x;
				o.uv.w = EBGFogVertex(v.vertex);
				#ifndef LIGHTMAP_OFF
					o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				half2 distortUV = UnpackNormal( tex2D(_DistortMap, i.uv.xy) ).xy * i.uv.zz;
				distortUV += i.uv.xy;
				
				fixed4 diffuseColor = tex2D(_MainTex, distortUV);
				#ifndef LIGHTMAP_OFF
					diffuseColor.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				#endif
			    fixed4 c = diffuseColor * _Color;
			    c.rgb = EBGFogFragment(c, i.uv.w);
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
			sampler2D _DistortMap;
			float4 _MainTex_ST;
			half4 _DistortParameter;
			#ifndef LIGHTMAP_OFF
				// float4 unity_LightmapST;
				float4 unity_LightmapFade;
				// sampler2D unity_Lightmap;
			#endif
			
			struct appdata_color {
				float4 vertex   : POSITION;
				float4 texcoord	: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float4 texcoord1 : TEXCOORD1;
				#endif
			};
			
			struct v2f {
				float4 pos		: SV_POSITION;
				float3 uv		: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float2 lmap		: TEXCOORD1;
				#endif
			};
			
			v2f vert (appdata_color v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.z = sin(_DistortParameter.y * _Time.y) * _DistortParameter.x;
				#ifndef LIGHTMAP_OFF
					o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				half2 distortUV = UnpackNormal( tex2D(_DistortMap, i.uv.xy) ).xy * i.uv.zz;
				distortUV += i.uv.xy;
				
				fixed4 diffuseColor = tex2D(_MainTex, distortUV);
				#ifndef LIGHTMAP_OFF
					diffuseColor.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				#endif
			    return diffuseColor * _Color;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
