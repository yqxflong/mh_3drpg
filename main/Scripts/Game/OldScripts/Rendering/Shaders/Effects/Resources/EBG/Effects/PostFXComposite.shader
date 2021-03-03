// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Effects/PostFXComposite"
{
	Properties
	{
		_MainTex ("Screen", 2D) = "" {}
		_BloomTex ("Bloom", 2D) = "black" {}
		_VignetteTex ("Vignette", 2D) = "black" {}
		_VignetteTex2("Vignette2MaskTex", 2D) = "black" {}
		_WarpTex ("Warp", 2D) = "white" {}
		_RadialBlurTex("Radial Blur Texture", 2D) = "white" {}
	}
	Category
	{
		Tags {

		}
		SubShader
		{
			LOD 250
			Pass
			{
			 	Tags {"Queue"="Geometry" }
				Cull Off
				ZWrite Off
				ZTest Off
				Blend Off
				CGPROGRAM

				//#pragma multi_compile EBG_POSTFX_COMPOSITE_BLOOM_ON EBG_POSTFX_COMPOSITE_BLOOM_OFF
				//#pragma multi_compile EBG_POSTFX_COMPOSITE_VIGNETTE_ON EBG_POSTFX_COMPOSITE_VIGNETTE_OFF
				#pragma multi_compile EBG_POSTFX_COMPOSITE_VIGNETTE2_ON EBG_POSTFX_COMPOSITE_VIGNETTE2_OFF
				//#pragma multi_compile EBG_POSTFX_COMPOSITE_COLORGRADE_ON EBG_POSTFX_COMPOSITE_COLORGRADE_OFF
				//#pragma multi_compile EBG_POSTFX_COMPOSITE_TONEMAP_ON EBG_POSTFX_COMPOSITE_TONEMAP_OFF
				//#pragma multi_compile EBG_POSTFX_COMPOSITE_WARP_ON EBG_POSTFX_COMPOSITE_WARP_OFF
				#pragma multi_compile EBG_POSTFX_COMPOSITE_RADIALBLUR_ON EBG_POSTFX_COMPOSITE_RADIALBLUR_OFF

				#pragma target 3.0

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _MainTex_ST;

				#if defined(EBG_POSTFX_COMPOSITE_BLOOM_ON)
				half3 _PostFXBloomColor;
				half _PostFXBloomIntensity;
				sampler2D _BloomTex;
				#endif

				#if defined(EBG_POSTFX_COMPOSITE_BLOOM_ON) || defined(EBG_POSTFX_COMPOSITE_VIGNETTE_ON)
				half3 _PostFXVignetteColor;
				#endif

				#if defined(EBG_POSTFX_COMPOSITE_VIGNETTE_ON)
				fixed _PostFXVignetteIntensity;
				sampler2D _VignetteTex;
				#endif

				#if defined(EBG_POSTFX_COMPOSITE_VIGNETTE2_ON)
				sampler2D _Vignette2MaskTex;
				float _Vignette2DarkIntensity;
				float _Vignette2BrightIntensity;
				fixed4 _Vignette2MaskColor;
				fixed4 _Vignette2BrightColor;
				#endif

				#if defined(EBG_POSTFX_COMPOSITE_WARP_ON)
				half2 _PostFXWarpIntensity;
				sampler2D _WarpTex;
				#endif

				#if defined(EBG_POSTFX_COMPOSITE_TONEMAP_ON)
				fixed _PostFXToneMapping;
				#endif

				#if defined(EBG_POSTFX_COMPOSITE_COLORGRADE_ON)
				sampler3D _ColourGradeTex;
				#endif

				#if defined(EBG_POSTFX_COMPOSITE_RADIALBLUR_ON)
				sampler2D _RadialBlurTex;
				half2 _RadialCenter;
				half _RadialSampleDist;
				half _RadialSampleStrength;
				#endif

				struct VertToFrag
				{
		          	float4 position : SV_POSITION;
					float2 screenUV : TEXCOORD0;

					#if defined(EBG_POSTFX_COMPOSITE_BLOOM_ON)
					half3 bloomColor : TEXCOORD1;
					#endif

					#if defined(EBG_FLIP_UVS)
					float2 sampleUV : TEXCOORD2;
					#endif
				};

				VertToFrag vertex_prog(appdata_img v)
				{
					VertToFrag data;
					data.position = UnityObjectToClipPos(v.vertex);
					data.screenUV = v.texcoord;

					#if defined(EBG_POSTFX_COMPOSITE_BLOOM_ON)
					data.bloomColor = _PostFXBloomColor * _PostFXBloomIntensity;
					#endif

					#if defined(EBG_FLIP_UVS)
					data.sampleUV = v.texcoord;
					data.sampleUV.y = 1.0f - data.sampleUV.y;
					#endif
					return data;
				}

				fixed4 fragment_prog(VertToFrag IN) : COLOR0
				{
					#if defined(EBG_FLIP_UVS)
					half2 sampleUV = IN.sampleUV;
					#else
					half2 sampleUV = IN.screenUV;
					#endif

					#if defined(EBG_POSTFX_COMPOSITE_WARP_ON)
						half2 warpSample = (tex2D(_WarpTex, sampleUV).rg - 0.5f) * _PostFXWarpIntensity;
						IN.screenUV += warpSample;

						#if defined(EBG_FLIP_UVS)
						sampleUV += warpSample;
						#endif
					#endif

					half4 screen = tex2D(_MainTex, IN.screenUV);
					#if defined(EBG_POSTFX_COMPOSITE_BLOOM_ON)
						half3 bloomSample = tex2D(_BloomTex, sampleUV).rgb;
						screen.rgb += bloomSample * IN.bloomColor;
						screen.rgb *= _PostFXVignetteColor.r;
					#endif

					#if defined(EBG_POSTFX_COMPOSITE_TONEMAP_ON)
						screen *= _PostFXToneMapping;
					#endif

					#if defined(EBG_POSTFX_COMPOSITE_VIGNETTE_ON)
						//screen.rgb *= lerp(half3(1.0, 1.0, 1.0), tex2D(_VignetteTex, sampleUV).rgb, _PostFXVignetteIntensity);
						//screen.g = 1;
						screen.rgb *= lerp(half3(1.0, 1.0, 1.0), tex2D(_VignetteTex, sampleUV).rgb, _PostFXVignetteIntensity) * half3(1.0, _PostFXVignetteColor.g, _PostFXVignetteColor.b);
					#endif

					#if defined(EBG_POSTFX_COMPOSITE_VIGNETTE2_ON)
						fixed4 mask = tex2D(_Vignette2MaskTex, IN.screenUV);
						//screen *= (1 + mask.r * _Vignette2BrightIntensity) * _Vignette2BrightColor;
						//screen = screen * (1 - mask.g * _Vignette2DarkIntensity) + mask.g * _Vignette2DarkIntensity * _Vignette2MaskColor * _Vignette2MaskColor.a;
						screen.rgb *= (1 + mask.r * _Vignette2BrightIntensity) * _Vignette2BrightColor.rgb;
						half darkIntensity = mask.g * _Vignette2DarkIntensity;
						screen.rgb = screen.rgb * (1 - darkIntensity) + darkIntensity * _Vignette2MaskColor.rgb;
					#endif

					#if defined(EBG_POSTFX_COMPOSITE_COLORGRADE_ON)
						screen.rgb = tex3D(_ColourGradeTex, screen).rgb;
					#endif

					#if defined(EBG_POSTFX_COMPOSITE_RADIALBLUR_ON)
						fixed dist = length(_RadialCenter - IN.screenUV);
						fixed4 blur = tex2D(_RadialBlurTex, IN.screenUV);
						screen.rgb = lerp(screen.rgb, blur.rgb, saturate(_RadialSampleStrength * dist));
					#endif

					return screen;
				}

				#pragma vertex vertex_prog
				#pragma fragment fragment_prog

				ENDCG
			}
		}

		SubShader
		{
			LOD 200
			Pass
			{
			 	Tags {"Queue"="Geometry" }
				Cull Off
				ZWrite Off
				ZTest Off
				Blend Off
				CGPROGRAM

				//#define EBG_POSTFX_COMPOSITE_BLOOM_ON
				//#define EBG_POSTFX_COMPOSITE_VIGNETTE_ON
				//#pragma multi_compile EBG_POSTFX_COMPOSITE_BLOOM_ON EBG_POSTFX_COMPOSITE_BLOOM_OFF
				//#pragma multi_compile EBG_POSTFX_COMPOSITE_VIGNETTE_ON EBG_POSTFX_COMPOSITE_VIGNETTE_OFF
				//#pragma multi_compile EBG_POSTFX_COMPOSITE_VIGNETTE2_ON EBG_POSTFX_COMPOSITE_VIGNETTE2_OFF
				//#pragma multi_compile EBG_POSTFX_COMPOSITE_COLORGRADE_ON EBG_POSTFX_COMPOSITE_COLORGRADE_OFF
				//#undef EBG_POSTFX_COMPOSITE_COLORGRADE_ON
				//#define EBG_POSTFX_COMPOSITE_COLORGRADE_OFF
				//#pragma multi_compile EBG_POSTFX_COMPOSITE_TONEMAP_ON EBG_POSTFX_COMPOSITE_TONEMAP_OFF
				//#pragma multi_compile EBG_POSTFX_COMPOSITE_WARP_ON EBG_POSTFX_COMPOSITE_WARP_OFF
				#pragma multi_compile EBG_POSTFX_COMPOSITE_RADIALBLUR_ON EBG_POSTFX_COMPOSITE_RADIALBLUR_OFF

				#pragma target 2.0

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _MainTex_ST;

				#if defined(EBG_POSTFX_COMPOSITE_BLOOM_ON)
				half3 _PostFXBloomColor;
				half _PostFXBloomIntensity;
				sampler2D _BloomTex;
				#endif

				#if defined(EBG_POSTFX_COMPOSITE_BLOOM_ON) || defined(EBG_POSTFX_COMPOSITE_VIGNETTE_ON)
				half3 _PostFXVignetteColor;
				#endif

				#if defined(EBG_POSTFX_COMPOSITE_VIGNETTE_ON)
				fixed _PostFXVignetteIntensity;
				sampler2D _VignetteTex;
				#endif

				#if defined(EBG_POSTFX_COMPOSITE_VIGNETTE2_ON)
				sampler2D _Vignette2MaskTex;
				float _Vignette2DarkIntensity;
				float _Vignette2BrightIntensity;
				fixed4 _Vignette2MaskColor;
				fixed4 _Vignette2BrightColor;
				#endif

				#if defined(EBG_POSTFX_COMPOSITE_WARP_ON)
				half2 _PostFXWarpIntensity;
				sampler2D _WarpTex;
				#endif

				#if defined(EBG_POSTFX_COMPOSITE_TONEMAP_ON)
				fixed _PostFXToneMapping;
				#endif

				#if defined(EBG_POSTFX_COMPOSITE_COLORGRADE_ON)
				sampler3D _ColourGradeTex;
				#endif

				#if defined(EBG_POSTFX_COMPOSITE_RADIALBLUR_ON)
				sampler2D _RadialBlurTex;
				half2 _RadialCenter;
				half _RadialSampleDist;
				half _RadialSampleStrength;
				#endif

				struct VertToFrag
				{
		          	float4 position : SV_POSITION;
					float2 screenUV : TEXCOORD0;

					#if defined(EBG_POSTFX_COMPOSITE_BLOOM_ON)
					half3 bloomColor : TEXCOORD1;
					#endif

					#if defined(EBG_FLIP_UVS)
					float2 sampleUV : TEXCOORD2;
					#endif
				};

				VertToFrag vertex_prog(appdata_img v)
				{
					VertToFrag data;
					data.position = UnityObjectToClipPos(v.vertex);
					data.screenUV = v.texcoord;

					#if defined(EBG_POSTFX_COMPOSITE_BLOOM_ON)
					data.bloomColor = _PostFXBloomColor * _PostFXBloomIntensity;
					#endif

					#if defined(EBG_FLIP_UVS)
					data.sampleUV = v.texcoord;
					data.sampleUV.y = 1.0f - data.sampleUV.y;
					#endif
					return data;
				}

				fixed4 fragment_prog(VertToFrag IN) : COLOR0
				{
					#if defined(EBG_FLIP_UVS)
					half2 sampleUV = IN.sampleUV;
					#else
					half2 sampleUV = IN.screenUV;
					#endif

					#if defined(EBG_POSTFX_COMPOSITE_WARP_ON)
						half2 warpSample = (tex2D(_WarpTex, sampleUV).rg - 0.5f) * _PostFXWarpIntensity;
						IN.screenUV += warpSample;

						#if defined(EBG_FLIP_UVS)
						sampleUV += warpSample;
						#endif
					#endif

					half4 screen = tex2D(_MainTex, IN.screenUV);
					#if defined(EBG_POSTFX_COMPOSITE_BLOOM_ON)
						half3 bloomSample = tex2D(_BloomTex, sampleUV).rgb;
						screen.rgb += bloomSample * IN.bloomColor;
						screen.rgb *= _PostFXVignetteColor.r;
					#endif

					#if defined(EBG_POSTFX_COMPOSITE_TONEMAP_ON)
						screen *= _PostFXToneMapping;
					#endif

					#if defined(EBG_POSTFX_COMPOSITE_VIGNETTE_ON)
						//screen.rgb *= lerp(half3(1.0, 1.0, 1.0), tex2D(_VignetteTex, sampleUV).rgb, _PostFXVignetteIntensity);
						//screen.g = 1;
						screen.rgb *= lerp(half3(1.0, 1.0, 1.0), tex2D(_VignetteTex, sampleUV).rgb, _PostFXVignetteIntensity) * half3(1.0, _PostFXVignetteColor.g, _PostFXVignetteColor.b);
					#endif

					#if defined(EBG_POSTFX_COMPOSITE_VIGNETTE2_ON)
						fixed4 mask = tex2D(_Vignette2MaskTex, IN.screenUV);
						//screen *= (1 + mask.r * _Vignette2BrightIntensity) * _Vignette2BrightColor;
						//screen = screen * (1 - mask.g * _Vignette2DarkIntensity) + mask.g * _Vignette2DarkIntensity * _Vignette2MaskColor * _Vignette2MaskColor.a;
						screen.rgb *= (1 + mask.r * _Vignette2BrightIntensity) * _Vignette2BrightColor.rgb;
						half darkIntensity = mask.g * _Vignette2DarkIntensity;
						screen.rgb = screen.rgb * (1 - darkIntensity) + darkIntensity * _Vignette2MaskColor.rgb;
					#endif

					#if defined(EBG_POSTFX_COMPOSITE_COLORGRADE_ON)
						screen.rgb = tex3D(_ColourGradeTex, screen).rgb;
					#endif

					#if defined(EBG_POSTFX_COMPOSITE_RADIALBLUR_ON)
						fixed dist = length(_RadialCenter - IN.screenUV);
						fixed4 blur = tex2D(_RadialBlurTex, IN.screenUV);
						screen.rgb = lerp(screen.rgb, blur.rgb, saturate(_RadialSampleStrength * dist));
					#endif

					return screen;
				}

				#pragma vertex vertex_prog
				#pragma fragment fragment_prog

				ENDCG
			}
		}
	}
}

