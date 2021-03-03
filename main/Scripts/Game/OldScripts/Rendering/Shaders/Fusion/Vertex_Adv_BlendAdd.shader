// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Fusion/Vertex/Adv/BlendADD" 
{
	Properties {
		_MainTex ("Base layer (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Alpha("Alpha +/-", Float) = 0
		_MMultiplier ("Layer Multiplier", Float) = 1.0
		[MaterialToggle] EBG_HEIGHT_FOG ("Enable fog", Float) = 1
	}
	Category
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend One OneMinusSrcAlpha //AddBlend
		Cull Off 
		Lighting Off 
		ZWrite Off 
		Fog { Mode Off }
		
		SubShader {
			LOD 200
		        
			Pass {		
				CGPROGRAM
				#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON    
				#include "UnityCG.cginc"
				#include "../Lib/EBG_Globals.cginc"
				#pragma vertex vert
				#pragma fragment frag
				#pragma glsl
			    #pragma glsl_no_auto_normalization
			    #pragma exclude_renderers flash xbox360 ps3
				#pragma multi_compile EBG_HEIGHT_FOG_OFF EBG_HEIGHT_FOG_ON
				
				sampler2D _MainTex;
				
				float4 _MainTex_ST;
				float _Alpha;
				float _MMultiplier;
				float4 _Color;

				struct v2f {
					float4 pos : SV_POSITION;
				    #ifdef EBG_HEIGHT_FOG_ON
				    float3  uv_fog : TEXCOORD0;
				    #else
				    float2  uv_fog : TEXCOORD0;
				    #endif
					fixed4 color : TEXCOORD1;
				};
				
				v2f vert (appdata_full v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv_fog.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex);
				    #ifdef EBG_HEIGHT_FOG_ON
				    o.uv_fog.z = EBGFogVertex(v.vertex);
				    #endif
					o.color = _MMultiplier * _Color * v.color;
					return o;
				}

				fixed4 frag (v2f i) : COLOR
				{
					fixed4 o;			
					fixed4 tex = tex2D (_MainTex, i.uv_fog.xy);
					o = tex * i.color;
				 	o.a += _Alpha;
				    #ifdef EBG_HEIGHT_FOG_ON
				    o.rgb = EBGFogFragment(o.rgb, i.uv_fog.z * o.a);
				    #endif
					return o;
				}
				ENDCG 
			}	
		}
		SubShader {
			LOD 100
		        
			Pass {		
				CGPROGRAM
				#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON    
				#include "UnityCG.cginc"
				#pragma vertex vert
				#pragma fragment frag
				#pragma glsl
			    #pragma glsl_no_auto_normalization
			    #pragma exclude_renderers flash xbox360 ps3 d3d11 d3d11_9x
				
				sampler2D _MainTex;
				
				float4 _MainTex_ST;
				float _Alpha;
				float _MMultiplier;
				float4 _Color;

				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					fixed4 color : TEXCOORD1;
				};
				
				v2f vert (appdata_full v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex);
					o.color = _MMultiplier * _Color * v.color;
					return o;
				}

				fixed4 frag (v2f i) : COLOR
				{
					fixed4 o;			
					fixed4 tex = tex2D (_MainTex, i.uv.xy);
					o = tex * i.color;
				 	o.a += _Alpha;
					return o;
				}
				ENDCG 
			}	
		}
	}
}
