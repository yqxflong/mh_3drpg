// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Character/Uber/UberCartoonOutline"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_ShadowColor("ShadowColor",Color) = (255, 216, 197, 255)
		
		//_CoolTex("CoolMap", 2D) = "bump" {}
			
		[MaterialToggle] EBG_RAMP_MAP("RAMP Enable", Float) = 1
		_ToonThreshold("Toon Threshold", Range(0,1)) = 0.73
		_BodyShadowFactor("Body Shadow Factor", range(0,0.2)) = 0.01

		[MaterialToggle] EBG_RIM_MAP("RIM Enable", Float) = 0
		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,38.0)) = 3.0


		[MaterialToggle] EBG_SPECULAR_MAP("Specular Enable", Float) = 0
		_SpecularColor("Specular Color", Color) = (1,1,1,1)
		_SpecularPower("Specular Power", Range(-50, 50)) = 1
		_SpecularFresnel("Specular Fresnel Value", Range(-1,1)) = 0.28
        _SpecularBehind("Specular Behind", Color) = (255,255,255,1)
		[Whitespace] _Whitespace("ColorFilter", Float) = 0
		[MaterialToggle] EBG_COLORFILTER("Enable", Float) = 0
		_FinalColor("FinalColor",Color) = (1,1,1,0)
		_ContrastIntansity("ContrastIntansity",range(0,1)) = 1
		_Brightness("Brightness",range(-1,1)) = 0
		_GrayScale("GrayScale",range(0,1)) = 0
		_Factor("Factor", range(0,1)) = 0.55
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(0.000, 10)) = 0.015
		_LightDir("Light Dir", Vector) = (1,1,1,1)
		_LightScale("Light Scale",Range(1, 2)) = 1

		_UseMatCap("Use Matcap", range(0,1)) = 0
		_CameraPosOpen("Camera Pos Open", range(0,1)) = 1
		_MatCap("MatCap (RGB)", 2D) = "white" {}
		_MatCapColor("MatCap Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_MatMaskRCap("MatCap About Mask R (RGB)", 2D) = "white" {}
		_MatMaskGCap("MatCap About Mask G (RGB)", 2D) = "white" {}
		_MatMaskBCap("MatCap About Mask B (RGB)", 2D) = "white" {}
		_MatMaskACap("MatCap About Mask A (RGB)", 2D) = "white" {}
		_MaskMap("Mask: R() G() B() A(metal)", 2D) = "white" {}

		_UseViewDir("Use View Dir", range(0,1)) = 0
		
		_FixViewDir("Fix View Dir", Vector) = (-5,0,0)
		
		_Value("Value", Range(0,3.0)) = 1.2
	}



	Category
	{
		Tags
		{
			"Queue"="Transparent"
			"RenderType"="Character"
			"LightMode"="ForwardBase"
		}
		Lighting Off
		Fog { Mode Off }
		Cull Off
		ZWrite On
		ZTest LEqual
		Blend Off

		Subshader
		{

			// Pass
			// {
			// 	Stencil
			// 	{
			// 		Ref 0
			// 		Comp GEqual
			// 		Pass Zero
			// 	}
			// 	Name "OUTLINE"
			// 	ZWrite Off

			// 	CGPROGRAM

			// 		#include "UnityCG.cginc"
			// 		#pragma target 3.0
			// 		#pragma multi_compile_fwdbase
			// 		#pragma vertex vert
			// 		#pragma fragment frag

			// 		uniform half4 _MainTex_TexelSize;

			// 		half _Outline;
			// 		fixed4 _OutlineColor;
			// 		float _Factor;

			// 		struct V2F
			// 		{
			// 			float4 pos:SV_POSITION;
			// 			half2 uv : TEXCOORD0;
			// 			//half4 offs[6] : TEXCOORD1;
			// 		};

			// 		V2F vert(appdata_full v)
			// 		{
			// 			//test
			// 			_Outline = 0;
			// 			//test end
			// 			V2F o;
			// 			v.vertex.xyz += v.normal * _Outline;
			// 			o.pos = UnityObjectToClipPos(v.vertex);
			// 			return o;
			// 			// V2F o;
			// 			// o.pos = UnityObjectToClipPos(v.vertex);
			// 			// float3 dir = normalize(v.vertex.xyz);
			// 			// float3 dir2 = v.normal;
			// 			// float D = dot(dir,dir2);
			// 			// D = (D/_Factor+1)/(1+1/_Factor);
			// 			// dir = lerp(dir2,dir,D);
			// 			// dir = mul((float3x3)UNITY_MATRIX_IT_MV, dir);
			// 			// float2 offset = TransformViewToProjection(dir.xy);
			// 			// offset = normalize(offset);
			// 			// o.pos.xy += offset * _Outline;
			// 			// return o;
			// 		}

			// 		fixed4 frag(V2F i) :COLOR
			// 		{
			// 			return _OutlineColor;
			// 		}
			// 	ENDCG
			// }
			Pass
			{
				Stencil
				{
					Ref 255
					Comp GEqual
					Pass replace
				}
						//Cull Back
						CGPROGRAM
						#include "UnityCG.cginc"
						#include "AutoLight.cginc"
						#pragma target 3.0
						#pragma vertex vertex_shader
						#pragma fragment fragment_shader
						#pragma multi_compile_fwdbase
						// #pragma multi_compile EBG_RAMP_MAP_OFF EBG_RAMP_MAP_ON
						// #pragma multi_compile EBG_RIM_MAP_OFF EBG_RIM_MAP_ON
						// #pragma multi_compile EBG_SPECULAR_MAP_OFF EBG_SPECULAR_MAP_ON
						#pragma multi_compile EBG_COLORFILTER_OFF EBG_COLORFILTER_ON
						sampler2D _MainTex;
						//sampler2D _CoolTex;
						float _ToonThreshold;
						float4 _RimColor;
						float _RimPower;
						float _RimFresnel;
						float4 _SpecularColor;
						float _SpecularPower;
						float _SpecularFresnel;
						float4 _SpecularBehind;
						float4 _LightDir;
						float _LightScale;
						fixed _GrayScale;
						fixed _ContrastIntansity;
						fixed _Brightness;
						fixed4 _FinalColor;

						fixed _UseViewDir;

						float _UseMatCap;
						float _CameraPosOpen;
						sampler2D _MatCap;
						sampler2D _MatMaskRCap;
						sampler2D _MatMaskGCap;
						sampler2D _MatMaskBCap;
						sampler2D _MatMaskACap;
						fixed4 _MatCapColor;
						sampler2D _MaskMap;
						half _BodyShadowFactor;
						half _Value;
						fixed4 _ShadowColor;

						fixed3 _FixViewDir;

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
							float2 diffuseUVAndMatCapCoords : TEXCOORD4;
							float4 NdotV:COLOR;
							//SHADOW_COORDS(1)
							//float4 outlinePos:SV_POSITION;
						};

						VtoS vertex_shader(Input v)
						{
							VtoS data;
							data.position = UnityObjectToClipPos(v.vertex);
							data.uv_main = v.texcoord0;
							float3 worldN = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
							data.Normal = worldN;
							float3 viewDirNorm = normalize(WorldSpaceViewDir(v.vertex));
							//data.viewDir = _FixViewDir;
							data.viewDir = viewDirNorm;
							float3 lightDirNorm = normalize(_LightDir.xyz);
							data.lightDir = lightDirNorm;

							if(_UseMatCap){
							//MatCap坐标准备：将法线从模型空间转换到观察空间，存储于TEXCOORD1的后两个纹理坐标zw
							data.diffuseUVAndMatCapCoords.x = dot(normalize(UNITY_MATRIX_IT_MV[0].xyz), normalize(v.normal));
							data.diffuseUVAndMatCapCoords.y = dot(normalize(UNITY_MATRIX_IT_MV[1].xyz), normalize(v.normal));
							//归一化的法线值区间[-1,1]转换到适用于纹理的区间[0,1]
							data.diffuseUVAndMatCapCoords.xy = data.diffuseUVAndMatCapCoords.xy * 0.5 + 0.5;
							}
							float3 V = WorldSpaceViewDir(v.vertex);
							V = mul((float3x3)unity_WorldToObject, V);//视方向从世界到模型坐标系的转换
							data.NdotV.x = saturate(dot(normalize(v.normal), normalize(V)));//必须在同一坐标系才能正确做点乘运算
							return data;
						}

						fixed4 fragment_shader(VtoS s) : COLOR0
						{
							// Lighting
							float4 mainTex = tex2D(_MainTex, s.uv_main.xy);
							float3 normal = s.Normal;
							fixed3 lightDir = _UseViewDir > 0 ? (s.viewDir + fixed3(-0.31, 0.5, 0.5)) : s.lightDir;

							//wrapped diffuse term
							half NdotL = dot(normal, lightDir);
							float diff = NdotL * 0.5 + 0.5;
							//half3 cool = tex2D(_CoolTex, s.uv_main.xy).rgb;
							half shadowDiff = saturate(diff - _ToonThreshold);
							//half shadowfactor = (diff - _ToonThreshold)> _BodyShadowFactor? shadowDiff*10:shadowDiff*9;
							half shadowfactor = (diff - _ToonThreshold)> _BodyShadowFactor? 1:shadowDiff;
							//half3 ramp = normal.rgb;
							//shadowfactor = 1 - pow(diff,_ToonThreshold);

							//half3 ramp = lerp(dark.rgb, mainTex.rgb, shadowfactor ).rgb;
							half3 ramp = mainTex.rgb;
							//ramp=lerp(ramp,ramp*1.2*_EBGCharLightProbeScale,(diff>0.98)).rgb;
							//ramp=ramp*diff;

							float3 halfVector = normalize(lightDir + s.viewDir);
							half lDotN = dot(normalize(-lightDir), s.Normal);
							float3 rimlight = float3(0, 0, 0);
							float3 finalSpec = float3(0, 0, 0);
 							half4 c;
							c.rgb = mainTex.rgb + rimlight;

#if defined(EBG_COLORFILTER_ON)
								half nl = saturate(dot(normal, normalize(lightDir)));
								rimlight = (lDotN < -0.1 ? 1 : 0) * pow((1 - s.NdotV.x), _RimPower)* _RimColor.rgb;
								//disney PBR,abort
								// half lh = saturate(dot(halfVector, normalize(lightDir)));
								// half nv = saturate(dot(normal, normalize(s.viewDir)));
								// half Fd90 = 0.5 + 2 * lh * lh * 0.4;
								// half disneyDiffuse = (1 + (Fd90 - 1) * pow(1 - nl, 5)) * (1 + (Fd90 - 1) * pow(1 - nv, 5));
								// ramp = ramp * disneyDiffuse;

								float specBase = pow(saturate(dot(halfVector, normal)), _SpecularPower);// *s.Specular * specFresnel;
								float fresnel = 1.0 - dot(normal, normalize(s.viewDir));
								fresnel = pow(fresnel, 50.0);
								float tmp = fresnel;
								fresnel = _SpecularFresnel + (1 - _SpecularFresnel)*fresnel;
								fresnel = max(0.0, fresnel - .1);
								finalSpec = specBase * fresnel* _SpecularColor*1.5;
								c.rgb =  nl <= 0? lerp(ramp,(_SpecularBehind),_SpecularBehind.a) : (ramp + finalSpec + rimlight);
								c.rgb = lerp(c.rgb, _FinalColor.rgb, _FinalColor.a);
								c.rgb = (c.rgb - 0.5)*_ContrastIntansity + 0.5;
								c.rgb += _Brightness;
								fixed3 grayClr;
								grayClr.rgb = dot(c.rgb, fixed3(0.3, 0.59, 0.11));
								c.rgb = lerp(c.rgb, grayClr, _GrayScale);
								//c.rgb = lerp(c.rgb,_FinalColor.rgb,_FinalColor.a);
#endif

								if(_UseMatCap){
								//float uvx1 = distance > 125 ? 0.3 : (distance - 125);
								float uvx1 = 0;// _CameraPosOpen > 0 ? (_WorldSpaceCameraPos.y - 6.9 < 0) ? 0 : ((_WorldSpaceCameraPos.y - 6.9) > 0.5 ? 0.4 : (_WorldSpaceCameraPos.y - 6.9)) : 0;

								//float uvx1 = distance > 200 ? 0.2 : saturate(distance / 135);
								float uvx2 = 1 - uvx1;

								float2 xy =	s.diffuseUVAndMatCapCoords.xy;
								//从提供的MatCap纹理中，提取出对应光照信息
								float3 matCapColor = tex2D(_MatCap, xy).rgb * _Value;// > 1 ? tex2D(_MatCap, xy).rgb : 1;
								float3 matMaskRCapColor = tex2D(_MatMaskRCap, xy).rgb * _Value;// > 1 ? tex2D(_MatMaskRCap, xy).rgb : 1;
								float3 matMaskGCapColor = tex2D(_MatMaskGCap, xy).rgb * _Value;// > 1 ? tex2D(_MatMaskGCap, xy).rgb : 1;
								float3 matMaskBCapColor = tex2D(_MatMaskBCap, xy).rgb * _Value;// > 1 ? tex2D(_MatMaskBCap, xy).rgb : 1;
								float3 matMaskACapColor = tex2D(_MatMaskACap, xy).rgb * _Value;// > 1 ? tex2D(_MatMaskACap, xy).rgb : 1;

								fixed3 mapCapFinal = c.rgb * matCapColor * _MatCapColor;

								//mapCapFinal = lerp(c.rgb, mapCapFinal, _Value);
									fixed4 mask = tex2D(_MaskMap, s.uv_main);
									c.rgb = (step(uvx1, s.uv_main.x) * step(s.uv_main.x, uvx2)) > 0 ? (_UseMatCap > 0 ? mapCapFinal : c.rgb) : c.rgb;
									c.rgb = (step(uvx1, s.uv_main.x) * step(s.uv_main.x, uvx2)) > 0 ? (_UseMatCap > 0 ? ((mask.a > 0) ? float3(c.rgb * matMaskACapColor) : c.rgb) : c.rgb) : c.rgb;
									c.rgb = (step(uvx1, s.uv_main.x) * step(s.uv_main.x, uvx2)) > 0 ? (_UseMatCap > 0 ? ((mask.r > 0) ? float3(c.rgb * matMaskRCapColor) : c.rgb) : c.rgb) : c.rgb;
									c.rgb = (step(uvx1, s.uv_main.x) * step(s.uv_main.x, uvx2)) > 0 ? (_UseMatCap > 0 ? ((mask.g > 0) ? float3(c.rgb * matMaskGCapColor) : c.rgb) : c.rgb) : c.rgb;
									c.rgb = (step(uvx1, s.uv_main.x) * step(s.uv_main.x, uvx2)) > 0 ? (_UseMatCap > 0 ? ((mask.b > 0) ? float3(c.rgb * matMaskBCapColor) : c.rgb) : c.rgb) : c.rgb;
								}
								c.a = mainTex.a;
								//float attenuation = SHADOW_ATTENUATION(i);
								return c;
								//return lerp(c*_ShadowColor, c, attenuation);
							}
							ENDCG
						}
		}
	}
	FallBack "Diffuse"
}
