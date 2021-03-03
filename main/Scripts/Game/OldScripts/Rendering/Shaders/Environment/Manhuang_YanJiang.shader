// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "ManHuang/Scene/YanJiang" {
	Properties {
		_GlowColor ("GlowColor", Color) = (1,0,0,1)
		_Maintex ("Maintex", 2D) = "black" {}
		_MaintexRotate ("MaintexRotate", Range(-0.5, 0.5)) = 0
		_DisplaceTex ("DisplaceTex", 2D) = "black" {}
		_DisplaceValue ("DisplaceValue", Range(-1, 1)) = 0.1
		_UMoveSpeed ("UMoveSpeed", Float ) = 0
		_VMoveSpeed ("VMoveSpeed", Float ) = 0
		_OutGlowSize ("OutGlowSize", Float ) = 1
		_InnerGlowSize ("InnerGlowSize", Float ) = 1
	}
	SubShader {
		Tags {
			"RenderType"="Opaque"
			"Queue" = "Geometry"
		}
		Pass {
			Name "FORWARD"
			Tags {
				"LightMode"="ForwardBase"
			}
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#define UNITY_PASS_FORWARDBASE
			#define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			#include "UnityStandardBRDF.cginc"
			#pragma multi_compile_fwdbase_fullshadows
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fog
			#pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
			#pragma target 3.0
			uniform float4 _TimeEditor;
			uniform float4 _GlowColor;
			uniform sampler2D _Maintex; uniform float4 _Maintex_ST;
			uniform sampler2D _DisplaceTex; uniform float4 _DisplaceTex_ST;
			uniform float _UMoveSpeed;
			uniform float _VMoveSpeed;
			uniform float _OutGlowSize;
			uniform float _DisplaceValue;
			uniform float _MaintexRotate;
			uniform float _InnerGlowSize;
			struct VertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 texcoord0 : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				//float2 texcoord2 : TEXCOORD2;
				//float4 vertexColor : COLOR;
			};
			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				//float2 uv2 : TEXCOORD2;
				float4 posWorld : TEXCOORD2;
				float3 normalDir : TEXCOORD3;
				float3 tangentDir : TEXCOORD4;
				float3 bitangentDir : TEXCOORD5;
				//float4 vertexColor : COLOR;
				LIGHTING_COORDS(6,7)
				UNITY_FOG_COORDS(8)
				#if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
					float4 ambientOrLightmapUV : TEXCOORD9;
				#endif
			};
			VertexOutput vert (VertexInput v) {
				VertexOutput o = (VertexOutput)0;
				o.uv0 = v.texcoord0;
				o.uv1 = v.texcoord1;
				//o.uv2 = v.texcoord2;
				//o.vertexColor = v.vertexColor;
				#ifdef LIGHTMAP_ON
					o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
					o.ambientOrLightmapUV.zw = 0;
				#elif UNITY_SHOULD_SAMPLE_SH
				#endif
				#ifdef DYNAMICLIGHTMAP_ON
					//o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
					o.ambientOrLightmapUV.zw = v.texcoord1.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
				o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				float3 lightColor = _LightColor0.rgb;
				o.pos = UnityObjectToClipPos(v.vertex );
				UNITY_TRANSFER_FOG(o,o.pos);
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}
			float4 frag(VertexOutput i) : COLOR {
				i.normalDir = normalize(i.normalDir);
				float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				float3 normalDirection = i.normalDir;
				float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float3 lightColor = _LightColor0.rgb;
////// Lighting:
				float attenuation = LIGHT_ATTENUATION(i);
				float3 attenColor = attenuation * _LightColor0.xyz;
/////// GI Data:
				UnityLight light;
				#ifdef LIGHTMAP_OFF
					light.color = lightColor;
					light.dir = lightDirection;
					light.ndotl = LambertTerm (normalDirection, light.dir);
				#else
					light.color = half3(0.f, 0.f, 0.f);
					light.ndotl = 0.0f;
					light.dir = half3(0.f, 0.f, 0.f);
				#endif
				UnityGIInput d;
				d.light = light;
				d.worldPos = i.posWorld.xyz;
				d.worldViewDir = viewDirection;
				d.atten = attenuation;
				#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
					d.ambient = 0;
					d.lightmapUV = i.ambientOrLightmapUV;
				#else
					d.ambient = i.ambientOrLightmapUV;
				#endif
				Unity_GlossyEnvironmentData ugls_en_data;
				ugls_en_data.roughness = 1.0 - 0;
				ugls_en_data.reflUVW = viewReflectDirection;
				UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
				lightDirection = gi.light.dir;
				lightColor = gi.light.color;
/////// Diffuse:
				float NdotL = max(0.0,dot( normalDirection, lightDirection ));
				float3 directDiffuse = max( 0.0, NdotL) * attenColor;
				float3 indirectDiffuse = float3(0,0,0);
				indirectDiffuse += gi.indirect.diffuse;
				//float vertexalpha = i.vertexColor.a;
				float vertexalpha = 1.0;
				float4 node_2355 = _Time + _TimeEditor;
				float node_4245_ang = node_2355.g;
				float node_4245_spd = _MaintexRotate;
				float node_4245_cos = cos(node_4245_spd*node_4245_ang);
				float node_4245_sin = sin(node_4245_spd*node_4245_ang);
				float2 node_4245_piv = float2(0.5,0.5);
				float4 _DisplaceTex_var = tex2D(_DisplaceTex,TRANSFORM_TEX(i.uv0, _DisplaceTex));
				float node_4995 = frac(((_DisplaceValue*_DisplaceTex_var.b)*vertexalpha));
				float4 node_1374 = _Time + _TimeEditor;
				float2 node_4245 = (mul(float2(((i.uv0.g+node_4995)+(node_1374.g*_UMoveSpeed)),((i.uv0.r+node_4995)+(node_1374.g*_VMoveSpeed)))-node_4245_piv,float2x2( node_4245_cos, -node_4245_sin, node_4245_sin, node_4245_cos))+node_4245_piv);
				float4 _Maintex_var = tex2D(_Maintex,TRANSFORM_TEX(node_4245, _Maintex));
				float3 node_4747 = /*pow(((1.0 - vertexalpha)*_GlowColor.rgb),_OutGlowSize);*/float3(0, 0, 0);
				float3 diffuseColor = ((vertexalpha*_Maintex_var.rgb)+node_4747);
				float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
				float3 emissive = (node_4747+(pow(_Maintex_var.g,_InnerGlowSize)*_GlowColor.rgb));
/// Final Color:
				float3 finalColor = diffuse + emissive;
				float4 finalRGBA = float4(finalColor, 1);
				UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
				return finalRGBA;
			}
			ENDCG
		}
		Pass {
			Name "FORWARD_DELTA"
			Tags {
				"LightMode"="ForwardAdd"
			}
			Blend One One
			
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define UNITY_PASS_FORWARDADD
			#define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			#include "UnityStandardBRDF.cginc"
			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fog
			#pragma exclude_renderers  xbox360 xboxone ps3 ps4 psp2 
			#pragma target 3.0
			uniform float4 _TimeEditor;
			uniform float4 _GlowColor;
			uniform sampler2D _Maintex; uniform float4 _Maintex_ST;
			uniform sampler2D _DisplaceTex; uniform float4 _DisplaceTex_ST;
			uniform float _UMoveSpeed;
			uniform float _VMoveSpeed;
			uniform float _OutGlowSize;
			uniform float _DisplaceValue;
			uniform float _MaintexRotate;
			uniform float _InnerGlowSize;
			struct VertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 texcoord0 : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				//float2 texcoord2 : TEXCOORD2;
				//float4 vertexColor : COLOR;
			};
			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				//float2 uv2 : TEXCOORD2;
				float4 posWorld : TEXCOORD2;
				float3 normalDir : TEXCOORD3;
				float3 tangentDir : TEXCOORD4;
				float3 bitangentDir : TEXCOORD5;
				//float4 vertexColor : COLOR;
				LIGHTING_COORDS(6,7)
				UNITY_FOG_COORDS(8)
			};
			VertexOutput vert (VertexInput v) {
				VertexOutput o = (VertexOutput)0;
				o.uv0 = v.texcoord0;
				o.uv1 = v.texcoord1;
				//o.uv2 = v.texcoord2;
				//o.vertexColor = v.vertexColor;
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
				o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				float3 lightColor = _LightColor0.rgb;
				o.pos = UnityObjectToClipPos(v.vertex );
				UNITY_TRANSFER_FOG(o,o.pos);
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}
			float4 frag(VertexOutput i) : COLOR {
				i.normalDir = normalize(i.normalDir);
				float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				float3 normalDirection = i.normalDir;
				float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
				float3 lightColor = _LightColor0.rgb;
////// Lighting:
				//float attenuation = LIGHT_ATTENUATION(i);
				UNITY_LIGHT_ATTENUATION(attenuation, i, i.posWorld.xyz);
				float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
				float NdotL = max(0.0,dot( normalDirection, lightDirection ));
				float3 directDiffuse = max( 0.0, NdotL) * attenColor;
				//float vertexalpha = i.vertexColor.a;
				float vertexalpha = 1.0;
				float4 node_957 = _Time + _TimeEditor;
				float node_4245_ang = node_957.g;
				float node_4245_spd = _MaintexRotate;
				float node_4245_cos = cos(node_4245_spd*node_4245_ang);
				float node_4245_sin = sin(node_4245_spd*node_4245_ang);
				float2 node_4245_piv = float2(0.5,0.5);
				float4 _DisplaceTex_var = tex2D(_DisplaceTex,TRANSFORM_TEX(i.uv0, _DisplaceTex));
				float node_4995 = frac(((_DisplaceValue*_DisplaceTex_var.b)*vertexalpha));
				float4 node_1374 = _Time + _TimeEditor;
				float2 node_4245 = (mul(float2(((i.uv0.g+node_4995)+(node_1374.g*_UMoveSpeed)),((i.uv0.r+node_4995)+(node_1374.g*_VMoveSpeed)))-node_4245_piv,float2x2( node_4245_cos, -node_4245_sin, node_4245_sin, node_4245_cos))+node_4245_piv);
				float4 _Maintex_var = tex2D(_Maintex,TRANSFORM_TEX(node_4245, _Maintex));
				float3 node_4747 = /*pow(((1.0 - vertexalpha)*_GlowColor.rgb),_OutGlowSize);*/float3(0, 0, 0);
				float3 diffuseColor = ((vertexalpha*_Maintex_var.rgb)+node_4747);
				float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
				float3 finalColor = diffuse;
				fixed4 finalRGBA = fixed4(finalColor * 1,0);
				UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
				return finalRGBA;
			}
			ENDCG
		}
		Pass {
			Name "Meta"
			Tags {
				"LightMode"="Meta"
			}
			Cull Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define UNITY_PASS_META 1
			#define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			#include "UnityStandardBRDF.cginc"
			#include "UnityMetaPass.cginc"
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_shadowcaster
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fog
			#pragma exclude_renderers  xbox360 xboxone ps3 ps4 psp2 
			#pragma target 3.0
			uniform float4 _TimeEditor;
			uniform float4 _GlowColor;
			uniform sampler2D _Maintex; uniform float4 _Maintex_ST;
			uniform sampler2D _DisplaceTex; uniform float4 _DisplaceTex_ST;
			uniform float _UMoveSpeed;
			uniform float _VMoveSpeed;
			uniform float _OutGlowSize;
			uniform float _DisplaceValue;
			uniform float _MaintexRotate;
			uniform float _InnerGlowSize;
			struct VertexInput {
				float4 vertex : POSITION;
				float2 texcoord0 : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				//float2 texcoord2 : TEXCOORD2;
				//float4 vertexColor : COLOR;
			};
			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				//float2 uv2 : TEXCOORD2;
				float4 posWorld : TEXCOORD2;
				//float4 vertexColor : COLOR;
			};
			VertexOutput vert (VertexInput v) {
				VertexOutput o = (VertexOutput)0;
				o.uv0 = v.texcoord0;
				o.uv1 = v.texcoord1;
				//o.uv2 = v.texcoord2;
				//o.vertexColor = v.vertexColor;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				//o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
				o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord0.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );
				return o;
			}
			float4 frag(VertexOutput i) : SV_Target {
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				UnityMetaInput o;
				UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
				
				//float vertexalpha = i.vertexColor.a;
				float vertexalpha = 1.0;
				float3 node_4747 = pow(((1.0 - vertexalpha)*_GlowColor.rgb),_OutGlowSize);
				float4 node_361 = _Time + _TimeEditor;
				float node_4245_ang = node_361.g;
				float node_4245_spd = _MaintexRotate;
				float node_4245_cos = cos(node_4245_spd*node_4245_ang);
				float node_4245_sin = sin(node_4245_spd*node_4245_ang);
				float2 node_4245_piv = float2(0.5,0.5);
				float4 _DisplaceTex_var = tex2D(_DisplaceTex,TRANSFORM_TEX(i.uv0, _DisplaceTex));
				float node_4995 = frac(((_DisplaceValue*_DisplaceTex_var.b)*vertexalpha));
				float4 node_1374 = _Time + _TimeEditor;
				float2 node_4245 = (mul(float2(((i.uv0.g+node_4995)+(node_1374.g*_UMoveSpeed)),((i.uv0.r+node_4995)+(node_1374.g*_VMoveSpeed)))-node_4245_piv,float2x2( node_4245_cos, -node_4245_sin, node_4245_sin, node_4245_cos))+node_4245_piv);
				float4 _Maintex_var = tex2D(_Maintex,TRANSFORM_TEX(node_4245, _Maintex));
				o.Emission = (node_4747+(pow(_Maintex_var.g,_InnerGlowSize)*_GlowColor.rgb));
				
				float3 diffColor = ((vertexalpha*_Maintex_var.rgb)+node_4747);
				o.Albedo = diffColor;
				
				return UnityMetaFragment( o );
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
