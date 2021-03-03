// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// #warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Fusion/Unlit/Foliage" {
	Properties {
		_Color("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_WiggleSpeed("Wiggle Speed", Float) = 1
		_WiggleAmount("Wiggle Amount", Float) = 0.25
		_WindStrength("Wind Strength", Float) = 1
		_windSpeed("Wind Speed", Float) = 5
		_MainTex("Base (RGB)", 2D) = "white" {}
		_SinWorldScale("Sin World Scale", Vector) = (3, 0.01, 6, 1)
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 300
		Lighting Off
		Fog { Mode Off }
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "../../Lib/EBG_Globals.cginc"
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
	
			fixed4 _Color;
			float _WiggleSpeed;
			float _WiggleAmount;
			float _WindStrength;
			float _windSpeed;
			float _WiggleSinTime = 0.0f;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _SinWorldScale;

			float4x4 _WindDirection;
			float4x4 _InverseWindDirection;

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
				half3 uv_fog		: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float2 lmap		: TEXCOORD1;
				#endif
			};			
			
			v2f vert (appdata_color v) {
				v2f o;

				float3 vertexOffset = float3(0, 0, 0);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex ).xyz;

				float3 worldPosWind = mul(float4(worldPos,1), _InverseWindDirection);
				
				float3 sinWorldPos = worldPosWind + (float3(_Time.w,_Time.w,-_Time.w) * _windSpeed);
				sinWorldPos *= _SinWorldScale.xyz * 0.01;
				sinWorldPos = sin(sinWorldPos);
			
				//float3 dWave3d = float3(sinWorldPos.xy, abs(sinWorldPos.z)) * (_SinWorldScale.xyz * _WindStrength * 0.1);
				float3 dWave3d = sinWorldPos.xyz * (_SinWorldScale.xyz * _WindStrength * 0.1);
				dWave3d = mul(float4(dWave3d,1),_WindDirection).xyz;

				vertexOffset += dWave3d;

				float wiggleAmount = (_WiggleAmount * 0.1) + ((sinWorldPos + 1 * 0.5) * _WiggleAmount * 0.1);

				float2 vec_k = float2 (worldPosWind.xz * 3.14159265);
				float sqrtMagnitude = sqrt((vec_k.x * vec_k.x) + (vec_k.y * vec_k.y));
				float iwkt = (sqrtMagnitude) * _Time.x * _WiggleSpeed * 0.1;

				vertexOffset += cos(iwkt) * wiggleAmount;

				v.vertex.xyz += vertexOffset * v.color.a * 1.0;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv_fog.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv_fog.z = EBGFogVertexWorldPos(worldPos);
				o.color = fixed4(v.color.rgb, 1.0f) * _Color;
				#ifndef LIGHTMAP_OFF
					o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
			    half4 texcol = tex2D(_MainTex, i.uv_fog.xy);
				#ifndef LIGHTMAP_OFF
					texcol.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				#endif
			    fixed4 c = texcol * i.color;
			    c.rgb = EBGFogFragment(c, i.uv_fog.z);
			    c *= _EBGEnvLightColorScale;
			    c.rgb *= _EBGEnvScale;
			    return c;
			}
			ENDCG
		}
	} 
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Lighting Off
		Fog { Mode Off }
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "../../Lib/EBG_Globals.cginc"
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
	
			fixed4 _Color;
			float _WiggleSpeed;
			float _WiggleAmount;
			float _WindStrength;
			float _windSpeed;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _SinWorldScale;

			float4x4 _WindDirection;
			float4x4 _InverseWindDirection;

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
				half3 uv_fog		: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float2 lmap		: TEXCOORD1;
				#endif
			};			
			
			v2f vert (appdata_color v) {
				v2f o;

				float3 vertexOffset = float3(0, 0, 0);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
				float3 sinWorldPos = worldPos + (float3(_Time.w,_Time.w,-_Time.w) * _windSpeed);
				sinWorldPos *= _SinWorldScale.xyz * 0.01;
				sinWorldPos = sin(sinWorldPos);
			
				float3 dWave3d = float3(sinWorldPos.xy, abs(sinWorldPos.z)) * (_SinWorldScale.xyz * _WindStrength * 0.1);
				dWave3d = mul(float4(dWave3d,1),_WindDirection).xyz;
				
				vertexOffset += dWave3d;
				v.vertex.xyz += vertexOffset * v.color.a;
				
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv_fog.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv_fog.z = EBGFogVertexWorldPos(worldPos);
				o.color = fixed4(v.color.rgb, 1.0f) * _Color;
				#ifndef LIGHTMAP_OFF
					o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
			    half4 texcol = tex2D(_MainTex, i.uv_fog.xy);
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
		Tags { "RenderType"="Opaque" }
		LOD 100
		Lighting Off
		Fog { Mode Off }
		
		Pass {
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
				o.color = fixed4(v.color.rgb, 1.0f) * _Color;
				#ifndef LIGHTMAP_OFF
					o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
			    half4 texcol = tex2D(_MainTex, i.uv);
				#ifndef LIGHTMAP_OFF
					texcol.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				#endif
			    return texcol * i.color;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}