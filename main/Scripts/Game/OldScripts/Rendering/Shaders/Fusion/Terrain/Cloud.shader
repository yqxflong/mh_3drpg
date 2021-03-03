// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Fusion/Terrain/Cloud" {
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
		//_CoordBlend("Middle, Length, End", Vector) = (0.5, 0.1, 0.3, 0.0)
		_MoveSpeed("Move Speed", float) = 0.4
		_MainTex("Diffuse Map", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend srcAlpha oneMinusSrcAlpha
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
			half _MoveSpeed;
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
				//half2 uv		: TEXCOORD0;
				float2 uv		: TEXCOORD0;
				#ifndef LIGHTMAP_OFF
					float2 lmap		: TEXCOORD1;
				#endif
			};
			
			v2f vert (appdata_color v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.x += (_MoveSpeed * _Time.x);
				o.color = v.color;
				#ifndef LIGHTMAP_OFF
					o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 diffuseColor = tex2D(_MainTex, i.uv.xy);
								#ifndef LIGHTMAP_OFF
					diffuseColor.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				#endif
			    //return diffuseColor * _Color; //excludes vertex color
			    return diffuseColor * _Color * i.color; //includes vertex color
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
