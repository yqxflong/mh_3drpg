// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Fusion/Terrain/ShoreScroll" {
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
		//_CoordBlend("Middle, Length, End", Vector) = (0.5, 0.1, 0.3, 0.0)
		_MoveSpeed("Wave X, Wave Y, Diffuse Y", Vector) = (-0.05, 0.1, 0.05, 0.0)
		_MainTex("Diffuse Map", 2D) = "white" {}
		_WaveTex("Wave Map", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue" = "Geometry" }	// because this is not transparent water
		LOD 200	
		Lighting Off
		Fog { Mode Off }
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "../../Lib/EBG_Globals.cginc"
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
	
			fixed4 _Color;
			float4 _MoveSpeed;
			sampler2D _MainTex;
			sampler2D _WaveTex;
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
				float3 uv_fog	: TEXCOORD0;
				float2 uv2		: TEXCOORD1;
				#ifndef LIGHTMAP_OFF
					float2 lmap		: TEXCOORD2;
				#endif
			};
			
			v2f vert (appdata_color v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv_fog.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv_fog.z = EBGFogVertex(v.vertex);
				o.uv2 = o.uv_fog.xy;

				_MoveSpeed.xyz *= _Time.y;
				
				o.uv_fog.xy += _MoveSpeed.xy;
				o.uv2.y += _MoveSpeed.z;

				o.color = v.color;
				#ifndef LIGHTMAP_OFF
					o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{				
				fixed4 waveColor = tex2D(_WaveTex, i.uv_fog.xy);
				fixed4 diffuseColor = tex2D(_MainTex, i.uv2.xy);				
				
				diffuseColor.rgb += (1.0f - diffuseColor.rgb) * waveColor.rgb * i.color.a; 
				
				#ifndef LIGHTMAP_OFF
					diffuseColor.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				#endif
			    fixed4 c = diffuseColor * _Color;
			    c.rgb = EBGFogFragment(c, i.uv_fog.z);
			    return c;
			}
			ENDCG
		}
	} 
	SubShader {
		Tags { "Queue" = "Geometry" }	// because this is not transparent water
		LOD 100	
		Lighting Off
		Fog { Mode Off }
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
	
			fixed4 _Color;
			float4 _MoveSpeed;
			sampler2D _MainTex;
			sampler2D _WaveTex;
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
				//half4 uv		: TEXCOORD0;
				float4 uv		: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float2 lmap		: TEXCOORD1;
				#endif
			};
			
			v2f vert (appdata_color v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.zw = o.uv.xy;

				_MoveSpeed.xyz *= _Time.y;
				
				o.uv.xy += _MoveSpeed.xy;
				o.uv.w += _MoveSpeed.z;

				o.color = v.color;
				#ifndef LIGHTMAP_OFF
					o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{				
				//fixed blendFactor = saturate( ( _CoordBlend.z - abs(i.uv.x - _CoordBlend.x) ) / _CoordBlend.y );				
				//_MoveSpeed.xyz *= _Time.y;
				
				fixed4 waveColor = tex2D(_WaveTex, i.uv.xy);
				//i.uv.y += _MoveSpeed.z;
				fixed4 diffuseColor = tex2D(_MainTex, i.uv.zw);				
				
				diffuseColor.rgb += (1.0f - diffuseColor.rgb) * waveColor.rgb * i.color.a; 
						
				//waveColor.rgb *= blendFactor;	
						
				//waveColor.rgb *= i.color.a;
				//diffuseColor.rgb += ( (1.0f - diffuseColor.rgb) * waveColor.rgb ) * i.color.rgb;
				
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
