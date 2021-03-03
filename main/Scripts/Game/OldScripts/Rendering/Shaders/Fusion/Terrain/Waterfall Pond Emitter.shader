// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Simplified Alpha Blended Particle shader. Differences from regular Alpha Blended Particle one:
// - no Tint color
// - no Smooth particle support
// - no AlphaTest
// - no ColorMask

Shader "Fusion/Terrain/Particle Alpha with fog " 
{
	Properties 
	{
		_MainTex ("Particle Texture", 2D) = "white" {}
		[MaterialToggle] EBG_HEIGHT_FOG ("Enable fog", Float) = 1
	}
	Category
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off 
		Lighting Off 
		ZWrite Off 
		Fog { Mode Off }
		
		SubShader 
		{
			LOD 200
			
			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "../../Lib/EBG_Globals.cginc"
				#pragma multi_compile EBG_HEIGHT_FOG_OFF EBG_HEIGHT_FOG_ON
		
				sampler2D _MainTex;
				float4 _MainTex_ST;
				
				struct appdata_color {
					float4 vertex   : POSITION;
					float4 texcoord	: TEXCOORD0;
					float4 color 	: COLOR;
				};
				
				struct v2f {
					float4 pos		: SV_POSITION;
				    #ifdef EBG_HEIGHT_FOG_ON
				    float3  uv_fog : TEXCOORD0;
				    #else
				    float2  uv_fog : TEXCOORD0;
				    #endif
					fixed4 color	: COLOR0;
				};
				
				v2f vert (appdata_color v) 
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv_fog.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				    #ifdef EBG_HEIGHT_FOG_ON
				    o.uv_fog.z = EBGFogVertex(v.vertex);
				    #endif
					o.color = v.color;
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
				    fixed4 col = tex2D(_MainTex, i.uv_fog.xy) * i.color;
				    #ifdef EBG_HEIGHT_FOG_ON
				    col.rgb = EBGFogFragment(col.rgb, i.uv_fog.z);
				    #endif
				    return col;
				}
				ENDCG
			}
		} 
		SubShader 
		{
			LOD 100
			
			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
		
				sampler2D _MainTex;
				float4 _MainTex_ST;
				
				struct appdata_color {
					float4 vertex   : POSITION;
					float4 texcoord	: TEXCOORD0;
					float4 color 	: COLOR;
				};
				
				struct v2f {
					float4 pos		: SV_POSITION;
					float2 uv		: TEXCOORD0;
					fixed4 color	: COLOR0;
				};
				
				v2f vert (appdata_color v) 
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.color = v.color;
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
				    fixed4 col = tex2D(_MainTex, i.uv) * i.color;
				    return col;
				}
				ENDCG
			}
		} 
	}
}
