﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Fusion/Terrain/SkyScroll" {
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
		//_CoordBlend("Middle, Length, End", Vector) = (0.5, 0.1, 0.3, 0.0)
		_MoveSpeed("Wave X, Wave Y, Diffuse Y", Vector) = (-0.05, 0.1, 0.05, 0.0)
		_MainTex("Diffuse Map", 2D) = "white" {}
		_WaveTex("Wave Map", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue" = "Geometry" }	// because this is not transparent water
		LOD 300	
		ZWrite Off
		Lighting Off
		
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

				_MoveSpeed.xyz *= _Time.x;
				o.uv.xy += _MoveSpeed.xy;
				o.uv.z += _MoveSpeed.z;

				o.color = v.color;
				#ifndef LIGHTMAP_OFF
					o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{				
				//fixed blendFactor = saturate( ( _CoordBlend.z - abs(i.uv.x - _CoordBlend.x) ) / _CoordBlend.y );				
				//_MoveSpeed.xyz *= _Time.x;
				
				//fixed4 waveColor = tex2D(_WaveTex, i.uv.xy + _MoveSpeed.xy);
				fixed4 waveColor = tex2D(_WaveTex, i.uv.xy);
				//i.uv.x += _MoveSpeed.z;
				//fixed4 diffuseColor = tex2D(_MainTex, i.uv.xy);
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
