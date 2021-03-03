// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Fusion/Terrain/Waterfall" {
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("River Map", 2D) = "white" {}
		_CascadeTex("Cascade Map", 2D) = "white" {}
		_PoolTex("Pool Map", 2D) = "white" {}
		_ScrollSpeed("Scroll Speed", Float) = 1
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
			sampler2D _MainTex;
			sampler2D _CascadeTex;
			sampler2D _PoolTex;
			float4 _MainTex_ST;
			half4 _WaterParameter;	// x is river offset
			float _ScrollSpeed;
			
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
				float4 pos			: SV_POSITION;
				fixed4 color		: COLOR0;
				float4 uv			: TEXCOORD0;
				fixed4 weight_fog	: TEXCOORD1;
			};
			
			v2f vert (appdata_color v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.y += _Time.x * _ScrollSpeed;
				o.color = v.color * _Color;
				
				// calculate the blend weight base on vertex alpha, put into vertex shader later
				o.weight_fog.xyz = abs(v.color.aaa - fixed3(0.5f, 0.333333f, 0.666667f)) * 3.0f;
				o.weight_fog.x -= 0.5f;
				o.weight_fog.xyz = saturate(o.weight_fog.xyz);
				o.weight_fog.yz = 1.0f - o.weight_fog.yz;
				
				o.weight_fog.w = EBGFogVertex(v.vertex);

				o.color = v.color * _Color;

				
				#ifndef LIGHTMAP_OFF
					o.uv.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#else
					o.uv.zw = float2(0, 0);
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 diffuseColor = tex2D(_MainTex, i.uv.xy);
				fixed4 cascadeColor = tex2D(_CascadeTex, i.uv.xy);
				fixed4 poolColor = tex2D(_PoolTex, i.uv.xy);
				
				diffuseColor = diffuseColor * i.weight_fog.xxxx + cascadeColor * i.weight_fog.yyyy + poolColor * i.weight_fog.zzzz;
				#ifndef LIGHTMAP_OFF
					diffuseColor.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv.zw));
				#endif
			    fixed4 c = diffuseColor * i.color;
			    c.rgb = EBGFogFragment(c, i.weight_fog.w);
			    return c;
			}
			ENDCG
		}
	} 
	SubShader {
		Tags { "Queue" = "Geometry" }	// because this is not transparent water
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
			sampler2D _CascadeTex;
			sampler2D _PoolTex;
			float4 _MainTex_ST;
			half4 _WaterParameter;	// x is river offset
			float _ScrollSpeed;
			
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
				float4 uv		: TEXCOORD0;
				fixed3 weight	: TEXCOORD1;
			};
			
			v2f vert (appdata_color v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.y += _Time.x * _ScrollSpeed;
				//o.color = v.color * _Color;
				
				// calculate the blend weight base on vertex alpha, put into vertex shader later
				o.weight = abs(v.color.aaa - fixed3(0.5f, 0.333333f, 0.666667f)) * 3.0f;
				o.weight.x -= 0.5f;
				o.weight = saturate(o.weight);
				o.weight.yz = 1.0f - o.weight.yz;

				o.color = v.color * _Color;

				
				#ifndef LIGHTMAP_OFF
					o.uv.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 diffuseColor = tex2D(_MainTex, i.uv.xy);
				fixed4 cascadeColor = tex2D(_CascadeTex, i.uv.xy);
				fixed4 poolColor = tex2D(_PoolTex, i.uv.xy);
				
				diffuseColor = diffuseColor * i.weight.xxxx + cascadeColor * i.weight.yyyy + poolColor * i.weight.zzzz;
				#ifndef LIGHTMAP_OFF
					diffuseColor.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv.zw));
				#endif
			    return diffuseColor * i.color;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
