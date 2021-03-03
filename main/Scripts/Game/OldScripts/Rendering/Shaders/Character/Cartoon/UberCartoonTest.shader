// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Character/Uber/UberCartoonTest"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		
		_CoolTex("CoolMap", 2D) = "bump" {}
			
		[MaterialToggle] EBG_RAMP_MAP ("RAMP Enable", Float) = 0
		_ToonThreshold("Toon Threshold", Range(0,1)) = 0.28
	

		[MaterialToggle] EBG_RIM_MAP ("RIM Enable", Float) = 0
		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
		
		
		[MaterialToggle] EBG_SPECULAR_MAP ("Specular Enable", Float) = 0
		_SpecularColor("Specular Color", Color) = (1,1,1,1)
		_SpecularPower("Specular Power", Range(-50, 50)) = 1
		_SpecularFresnel("Specular Fresnel Value", Range(0,1)) = 0.28
		
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (.0002, .05)) = 0.015
		
		_LightDir("Light Dir", Vector) = (1,1,1,1)
		_LightScale("Light Scale",Range(1, 2)) = 1
	}
	Category
	{
		Tags
		{
			"Queue"="Geometry"
			"RenderType"="Character"
			"LightMode"="ForwardBase"
		}
		Lighting Off
		Fog { Mode Off }
		Cull Back
		ZWrite On
		ZTest LEqual
		Blend Off

		Subshader
		{
			Pass
			{
				Stencil 
				{
					Ref 4
				    Comp always
				    Pass replace
				}
				CGPROGRAM
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"
				#include "Assets/_GameAssets/Scripts/Game/OldScripts/Rendering/Shaders/Lib/EBG_Globals.cginc"
				#pragma target 3.0
				#pragma vertex vertex_shader
				#pragma fragment fragment_shader
				#pragma multi_compile_fwdbase
				#pragma multi_compile EBG_RAMP_MAP_OFF EBG_RAMP_MAP_ON
				#pragma multi_compile EBG_RIM_MAP_OFF EBG_RIM_MAP_ON
				#pragma multi_compile EBG_SPECULAR_MAP_OFF EBG_SPECULAR_MAP_ON
				sampler2D _MainTex;
				sampler2D _CoolTex;
				float _ToonThreshold;
				float4 _RimColor;
				float _RimPower;
				float _RimFresnel;
				float4 _SpecularColor;
				float _SpecularPower;
				float _SpecularFresnel;
				
				float4 _LightDir;
				float _LightScale;
				struct Input
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
					float2 texcoord0 : TEXCOORD0;
				};
	
				struct VtoS
				{
					float4 position : SV_POSITION;
					float2 uv_main : TEXCOORD0;
					float3 Normal : TEXCOORD1;
					fixed3 lightDir : TEXCOORD2;
					float3 viewDir : TEXCOORD3;
					fixed3 color : TEXCOORD4;
				};
	
				VtoS vertex_shader(Input v)
				{
					VtoS data;
					data.position = UnityObjectToClipPos(v.vertex);
					data.uv_main=v.texcoord0;
					float3 worldN = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
					data.Normal=worldN;
					float3 viewDirNorm = normalize(WorldSpaceViewDir(v.vertex));
					data.viewDir=viewDirNorm;
					float3 lightDirNorm=normalize(_EBGCharDirectionToLight0.xyz*_LightDir.xyz);
					data.lightDir=lightDirNorm;
					data.color=ShadeSH9(float4(worldN, 1)) * _EBGCharLightProbeScale * _LightScale;
					return data;
				}
	
				fixed4 fragment_shader(VtoS s) : COLOR0
				{
					
					// Lighting
					float4 mainTex = tex2D(_MainTex, s.uv_main.xy);
					float3 _1LightColor0=s.color;	
					float3 normal = s.Normal;
				
					//wrapped diffuse term
					half NdotL = dot(normal, s.lightDir);
					float diff = NdotL * 0.5 + 0.5;
					half3 cool = tex2D(_CoolTex, s.uv_main.xy).rgb;
					half3 ramp = lerp(cool*mainTex.rgb, mainTex.rgb, (diff>_ToonThreshold)).rgb;
					//ramp=lerp(ramp,ramp*1.2*_EBGCharLightProbeScale,(diff>0.98)).rgb;
					//ramp=ramp*diff;

					float3 halfVector = normalize(s.lightDir + s.viewDir);
					//rimlight
					#if(EBG_RIM_MAP_ON)
						half rim = 1.0 - saturate(dot(normalize(s.viewDir), s.Normal));
						float3 rimlight = _RimColor.rgb * pow(rim, _RimPower);

						half lh = saturate(dot(halfVector, normalize(s.lightDir)));
						half nv = saturate(dot(normal, normalize(s.viewDir)));
						half nl = saturate(dot(normal, normalize(s.lightDir)));
						half Fd90 = 0.5 + 2 * lh * lh * 0.4;
						half disneyDiffuse = (1 + (Fd90 - 1) * pow(1 - nl, 5)) * (1 + (Fd90 - 1) * pow(1 - nv, 5));
						//ramp=ramp*disneyDiffuse;
						//rimlight = float3(0,0,0);
					#else
					float3 rimlight = float3(0,0,0);
					#endif
					
					//Specular term	
					#if(EBG_SPECULAR_MAP_ON)
						float specBase = pow(saturate(dot(halfVector, normal)), _SpecularPower);// *s.Specular * specFresnel;
						float fresnel = 1.0 - dot(normal, normalize(s.viewDir));
						fresnel = pow(fresnel, 5.0);
						float tmp = fresnel;
						fresnel = _SpecularFresnel + (1 - _SpecularFresnel)*fresnel;
						//fresnel=0.20;
						//float3 finalSpec = lerp(float3(0.5,0.5,0.5),float3(1,1,1),tmp).rgb;
						fresnel = max(0.0, fresnel - .1);
						float3 finalSpec = specBase* fresnel* _1LightColor0.rgb*_SpecularColor;
					#else
						float3 finalSpec = float3(0,0,0);
					#endif
					
					half4 c;
					
					#if defined(EBG_RAMP_MAP_ON)
						c.rgb = _1LightColor0.rgb * ramp+finalSpec  +rimlight;
					#else
						c.rgb = mainTex.rgb * _1LightColor0.rgb  +rimlight;
					#endif

					c.a = mainTex.a;
					return c;
				}
				ENDCG
			}
			
			
			Pass 
			{
				Name "OUTLINE"
				Tags { "LightMode" = "Always" }
				Cull Front
				ZWrite On
				//ColorMask RGB
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"
				#include "Assets/_GameAssets/Scripts/Game/OldScripts/Rendering/Shaders/Lib/EBG_Globals.cginc"
				#pragma vertex vert
				#pragma fragment frag
				uniform float _Outline;
				uniform float4 _OutlineColor;
				sampler2D _MainTex; float4 _MainTex_ST;
				struct appdata 
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float2 texcoord0 : TEXCOORD0;
					//float4 color:COLOR;
				};

				struct v2f 
				{
					/*float4 pos : POSITION;
					float4 color : COLOR;
					float3 Normal : TEXCOORD0;
					float2 uv_main : TEXCOORD1;*/
					float4 pos:SV_POSITION;
					float2 uv_main : TEXCOORD1;
					//float4 color : COLOR;
				};
	
				v2f vert(appdata v) 
				{
					/*v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
					float2 offset = TransformViewToProjection(norm.xy);
					o.color = _OutlineColor;
					o.uv_main=v.texcoord0;
					o.pos.xy += offset * o.pos.z * _Outline;
					return o;*/

					//v2f o;
					//float4 pos = mul(UNITY_MATRIX_MV, v.vertex);
					//float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
					//normal.z -= 0.5;    // 让法线向视角方向外扩，避免物体有背面遮挡正面
					//pos = pos + float4(normalize(normal), 0) * _Outline*_EBGCharOutlineScale;        //对外扩展，出现轮廓
					//o.pos = mul(UNITY_MATRIX_P, pos);
					//o.color = _OutlineColor;
					//return o;

					v2f o;

					v.vertex.xyz += v.normal * _Outline * _EBGCharOutlineScale;
					o.pos = UnityObjectToClipPos(v.vertex);

					UNITY_TRANSFER_FOG(o, o.pos);
					o.uv_main = v.texcoord0;
					//o.color = _OutlineColor;
					return o;
				}
			
				half4 frag(v2f i) :COLOR 
				{ 
					/*float4 mainTex = tex2D(_MainTex, i.uv_main.xy);
					return float4(i.color.rgb*mainTex.rgb,1);*/
					float4 _MainTex_var = tex2D(_MainTex, TRANSFORM_TEX(i.uv_main, _MainTex));
					return fixed4(((_MainTex_var.rgb*_MainTex_var.rgb)*_OutlineColor.rgb), 1);
					//float4 mainTex = tex2D(_MainTex, i.uv_main.xy);
					//return float4(i.color.rgb*mainTex.rgb,1);
				}
				/*v2f vert(appdata v)
				{
					v2f o;
					float4 Vpos = mul(UNITY_MATRIX_MV, v.vertex);
					float3 Vnormal = mul(UNITY_MATRIX_IT_MV, v.color.xyz - 1);
					Vnormal.z = -0.05;
					o.pos = mul(UNITY_MATRIX_P, Vpos + float4(Vnormal, 0)*_Outline* _EBGCharOutlineScale*100);
					o.uv_main = TRANSFORM_TEX(v.texcoord0, _MainTex);

					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					return fixed4(0,1,1,1);
				}*/
				ENDCG
			}
		}
	}
	FallBack "Diffuse"
}
