// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Character/Star/PixivToonOutline"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		//_ShadowColor("ShadowColor",Color) = (255, 216, 197, 255)
				
		//[MaterialToggle] EBG_RAMP_MAP("RAMP Enable", Float) = 1
		// A剔除或者减弱部分擦除所有区域阴影到高光面，R灰面到浅阴影，G只擦除分界线阴影到灰面，B只擦除暗部阴影到分界线
		_SideColorCorrection("Side Color Correction (ARGB)", 2D) = "white" {} 

		// 颜色分层次过度 --------------------------------------------
		_DarkRangeShallow("Side Of Shallow", Vector) = (0.5, 0.2, 0.6, 1)
		_DarkRangeGray ("Side Of Gray", Vector) = (0.2, 0, 0.45, 1)
		_DarkRangeBoundary("Side Of Boundary", Vector) = (0, -0.2, 0.3, 1)
		_DarkRangeDeep("Side Of Dark", Vector) = (-0.2, -0.5, 0.33, 1)
		_DarkRangeBacklight("Side Of Backlight", Vector) = (-0.5, -1, 0.39, 1)
		// 不同层的面，色彩偏向
		_IlluminColor("Color Of Illuminated", Color) = (1,1,1,1)
		_DarkColorShallow("Color Of Shallow Part", Color) = (1,1,1,1)
		_DarkColorGray("Color Of Gray Part", Color) = (1,1,1,1)
		_DarkColorBoundary("Color Of Boundary Part", Color) = (1,1,1,1)
		_DarkColorDeep("Color Of Dark Part", Color) = (1,1,1,1)
		_DarkColorBacklight("Color Of Backlight Part", Color) = (1,1,1,1)
		// -----------------------------------------------------------

		// 冷暖色过度(贯穿所有层) --------------------------------
		_YellowColor("Color Of Warm", Color) = (0,0,0,0)
		_BlueColor("Color Of Cool", Color) = (0,0,0,0)
		_Warmness("Warmness", range(0,1)) = 1
		_Coolness("Coolness", range(0,1)) = 1
		// -------------------------------------------------------

		// 颜色混合方式 --------------------------------------------
		_IllumIntensity("Illum(Part) Light Intensity", Range(0,1)) = 0.82	// 亮部色强
		_RampMixability("Ramp Color Blend Mixability", Range(0,1)) = 0.1	// 变暗色的插值混合程度
		_OverlayMixability("Overlay Blend Mixability", Range(0,1)) = 0.8	// 叠加色的插值混合程度
		_DarkOpacity("Side Color Opacity", Range(0,1)) = 0.1				// 暗部和阴影的通透度，用来调整色彩压暗时干湿程度
		// ---------------------------------------------------------

		_RampIntensity("Side Color Intensity",Range(0,2)) = 0.9
		_ColorCorrection("Global Color Correction (RGB)", 2D) = "white" {} // 修正，R背部阴影，G高光部分和B边光

		[MaterialToggle] EBG_RIM_MAP("RIM Enable", Float) = 0
		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,38.0)) = 3.0
		_Edge("Edge Scale",range(-1,1)) = -0.1

		[MaterialToggle] EBG_SPECULAR_MAP("Specular Enable", Float) = 0
		_SpecularColor("Specular Color", Color) = (1,1,1,1)
		_SpecularPower("Specular Power", Range(-50, 50)) = 1
		_SpecularFresnel("Specular Fresnel Value", Range(-1,1)) = 0.28
        _SpecularBehind("Specular Behind", Color) = (255,255,255,1)
		[Whitespace] _Whitespace("ColorFilter", Float) = 0
		[MaterialToggle] EBG_COLORFILTER("Enable", Float) = 0
		_SolidColor("Solid Color",Color) = (1,1,1,0)
		_ContrastIntansity("ContrastIntansity",range(0,1)) = 1
		_Brightness("Brightness",range(-1,1)) = 0
		_Saturation("Saturation", Range(-1,2.0)) = 0
		_GrayScale("GrayScale",range(0,1)) = 0
		
		_LightDir("Light Dir", Vector) = (1,1,1,1)

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
				
		_Value("Value", Range(0,3.0)) = 1.2
	}

	Category
	{	
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Character"
			"LightMode" = "ForwardBase"
		}

		Lighting Off
		Fog { Mode Off }
		Cull Off
		ZWrite On
		ZTest LEqual
		Blend Off
		//Cull Back

		Subshader
		{
			Pass
			{
				Name "Forward"				
				Stencil
				{
					Ref 255
					Comp GEqual
					Pass replace
				}
				
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
				float4 _MainTex_ST;

				// 边缘光
				float4 _RimColor;
				float _RimPower;
				half _Edge;

				// 高光处理
				float4 _SpecularColor;
				float _SpecularPower;
				float _SpecularFresnel;
				float4 _SpecularBehind;

				float4 _LightDir;

				// 色彩校正
				fixed _Saturation;
				fixed _GrayScale;
				fixed _ContrastIntansity;
				fixed _Brightness;
				fixed4 _SolidColor;

				fixed _UseViewDir;

				// MatCap
				float _UseMatCap;
				float _CameraPosOpen;
				sampler2D _MatCap;
				sampler2D _MatMaskRCap;
				sampler2D _MatMaskGCap;
				sampler2D _MatMaskBCap;
				sampler2D _MatMaskACap;
				fixed4 _MatCapColor;
				sampler2D _MaskMap;
				half _Value;

				//fixed4 _ShadowColor;

				// ToonGradient
				half4 _DarkRangeShallow;
				half4 _DarkRangeGray;
				half4 _DarkRangeBoundary;
				half4 _DarkRangeDeep;
				half4 _DarkRangeBacklight;

				fixed4 _DarkColorShallow;
				fixed4 _DarkColorGray;
				fixed4 _DarkColorBoundary;
				fixed4 _DarkColorDeep;
				fixed4 _DarkColorBacklight;

				fixed4 _IlluminColor;

				sampler2D _SideColorCorrection;

				half4 _YellowColor;
				half4 _BlueColor;
				half _Warmness;
				half _Coolness;			

				sampler2D _ColorCorrection;
				float _IllumIntensity;
				float _RampMixability;
				float _DarkOpacity;
				float _OverlayMixability;
				float _RampIntensity;

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
					//SHADOW_COORDS(1)
					//float4 outlinePos:SV_POSITION;
				};

				VtoS vertex_shader(Input v)
				{
					VtoS data;

					data.position = UnityObjectToClipPos(v.vertex);
					data.uv_main = v.texcoord0;

					data.Normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));

					data.viewDir = normalize(WorldSpaceViewDir(v.vertex));

					data.lightDir = normalize(_LightDir.xyz);

					if(_UseMatCap){
						//MatCap坐标准备：将法线从模型空间转换到观察空间，存储于TEXCOORD1的后两个纹理坐标zw
						data.diffuseUVAndMatCapCoords.x = dot(normalize(UNITY_MATRIX_IT_MV[0].xyz), normalize(v.normal));
						data.diffuseUVAndMatCapCoords.y = dot(normalize(UNITY_MATRIX_IT_MV[1].xyz), normalize(v.normal));
						//归一化的法线值区间[-1,1]转换到适用于纹理的区间[0,1]
						data.diffuseUVAndMatCapCoords.xy = data.diffuseUVAndMatCapCoords.xy * 0.5 + 0.5;
					}

					return data;
				}

				half GetToonGradientValue(float diff, half result, float ceiling, float floor, float target, float open, int index, float4 sideMask, float4 colorMask)
				{
					if (result == 0)
					{											
						half min = step(diff, ceiling); //half ret = step(a, b) if(a<=b) ret=1, else ret=0
						half max = step(floor, diff);

						half value = min * max * target;

						if ( index == 0 && value > 0 ) {
							_IlluminColor = _DarkColorShallow;							
							value = lerp(_IllumIntensity, value, colorMask.g);
							value *= open;
						}

						if ( index == 1 && value > 0 ) {
							_IlluminColor = _DarkColorGray;
							if (open < 0.5)
								value = _DarkRangeShallow.z;
							else
								value = lerp(_DarkRangeShallow.z, value, sideMask.r);
						}
						if ( index == 2 && value > 0 ) {
							_IlluminColor = _DarkColorBoundary;
							if (open < 0.5)
								value = _DarkRangeGray.z;
							else
								value = lerp(_DarkRangeGray.z, value, sideMask.g);
						}
						if ( index == 3 && value > 0 ) {
							_IlluminColor = _DarkColorDeep;
							if (open < 0.5)
								value = _DarkRangeBoundary.z;
							else
								value = lerp(_DarkRangeBoundary.z, value, sideMask.b);
						}
						if ( index == 4 && value > 0 ) {
							_IlluminColor = _DarkColorBacklight;
							value *= open;
						}

						if (value > 0)
							return sideMask.a <= 0 ? 0 : lerp(_IllumIntensity, value, sideMask.a);
						else
							return value;
					}
					return result;
				}

				half3 GetToneBasedColor(half3 color, float diff)
				{
					half3 coolColor = _BlueColor.rgb + color * _Coolness;
					half3 warmColor = _YellowColor.rgb + color * _Warmness;
					return diff * coolColor + (1 - diff) * warmColor;
				}

				half OverlayBlendMode(half basePixel, half blendPixel) {
					if (basePixel < 0.5) {
						return (2.0 * basePixel * blendPixel);
					}
					else {
						return (1.0 - 2.0 * (1.0 - basePixel) * (1.0 - blendPixel));
					}
				}

				inline fixed3 GetSaturation(fixed3 inColor)
				{
					fixed average = (inColor.r + inColor.g + inColor.b) / 3;
					inColor.rgb += (inColor.rgb - average) * _Saturation;
					return inColor;
				}

				fixed4 fragment_shader(VtoS s) : COLOR0
				{
					// base lighting
					float4 mainTex = tex2D( _MainTex, TRANSFORM_TEX(s.uv_main.xy, _MainTex) );
					float3 normal = s.Normal;
					half3 lightDir = _UseViewDir > 0 ? (s.viewDir + half3(-0.31, 0.5, 0.5)) : s.lightDir;

					//wrapped diffuse term, and base color
					half NdotL = dot(normal, lightDir);
					float diff = NdotL * 0.5 + 0.5;

					half3 texColor = mainTex.rgb;
					half4 col = half4(texColor, 1);

					#if defined(EBG_COLORFILTER_ON)
					// toon shadow
					// half shadowDiff = saturate(diff - _ToonThreshold);
					// half shadowfactor = (diff - _ToonThreshold)> _BodyShadowFactor? 1:shadowDiff;
					float4 colorMask = tex2D(_ColorCorrection, s.uv_main);
					float4 sideMask = tex2D(_SideColorCorrection, s.uv_main);

					// toon gradient, multilayered areas
					half gradient = GetToonGradientValue(NdotL, 0, _DarkRangeShallow.x, _DarkRangeShallow.y, _DarkRangeShallow.z, _DarkRangeShallow.w, 0, sideMask, colorMask);
					gradient = GetToonGradientValue(NdotL, gradient, _DarkRangeGray.x, _DarkRangeGray.y, _DarkRangeGray.z, _DarkRangeGray.w, 1, sideMask, colorMask);
					gradient = GetToonGradientValue(NdotL, gradient, _DarkRangeBoundary.x, _DarkRangeBoundary.y, _DarkRangeBoundary.z, _DarkRangeBoundary.w, 2, sideMask, colorMask);
					gradient = GetToonGradientValue(NdotL, gradient, _DarkRangeDeep.x, _DarkRangeDeep.y, _DarkRangeDeep.z, _DarkRangeDeep.w, 3, sideMask, colorMask);
					gradient = GetToonGradientValue(NdotL, gradient, _DarkRangeBacklight.x, _DarkRangeBacklight.y, _DarkRangeBacklight.z, _DarkRangeBacklight.w, 4, sideMask, colorMask);
					if (gradient == 0)gradient = _IllumIntensity;					
					half4 areasCol = _IlluminColor;

					// bass variable
					float3 rimlight = float3(0, 0, 0);
					float3 finalSpec = float3(0, 0, 0);					

					float3 halfVector = normalize(lightDir + s.viewDir);
					half lDotN = dot(normalize(-lightDir), s.Normal);
					half NDotV = saturate(dot(normal, s.viewDir));

					// 边缘光 edge light（colorMask.b可以剔除）
					half nl = saturate(dot(normal, normalize(lightDir)));
					rimlight = (lDotN < _Edge ? 1 : 0) * pow((1 - NDotV), _RimPower) * _RimColor.rgb * colorMask.b;

					// disney PBR, abort
					// half lh = saturate(dot(halfVector, normalize(lightDir)));
					// half nv = saturate(dot(normal, normalize(s.viewDir)));
					// half Fd90 = 0.5 + 2 * lh * lh * 0.4;
					// half disneyDiffuse = (1 + (Fd90 - 1) * pow(1 - nl, 5)) * (1 + (Fd90 - 1) * pow(1 - nv, 5));
					// diffuse = diffuse * disneyDiffuse;

					// 带fresnel的高光的实现（colorMask.g可以剔除）
					// specular
					float specBase = pow( saturate( dot(halfVector, normal) ), _SpecularPower ); 					
					// fresnel
					float fresnel = pow( 1.0 - dot(normal, normalize(s.viewDir)), 50.0 );
					fresnel = max(0.0, ( _SpecularFresnel + (1 - _SpecularFresnel) * fresnel ) - .1);					
					// specular col => s.Specular * specFresnel;
					finalSpec = specBase * fresnel * _SpecularColor * 1.5;			

					// 日式厚涂风格颜色由亮至暗的分区梯度过度 ---------------------------------------					
					half3 areaGradientSolidColor = gradient * areasCol;
					// 叠加,提高色彩对比,本质上是让上边分区块明暗的色彩值使得原贴图色彩暗部更暗，亮部更亮
					half3 overlayCol = texColor;
					overlayCol.r = OverlayBlendMode(texColor.r, areaGradientSolidColor.r);
					overlayCol.g = OverlayBlendMode(texColor.g, areaGradientSolidColor.g);
					overlayCol.b = OverlayBlendMode(texColor.b, areaGradientSolidColor.b);
					// 手绘中的阴影色时覆盖上去的图层，可以调节透明度,DarkOpacity决定颜色在暗部更趋向与贴图纹理色的变暗色还是暗部阴影色块
					half3 overlayRamp = lerp(overlayCol, areaGradientSolidColor, (1 - gradient)*_DarkOpacity) * _RampIntensity;
					// 是否用正片叠底的方式和原图混合,(赛璐璐的方式是要有混合的,因为它是在基本色填充后，再用暗色图层压暗)
					half3 celCol = lerp(texColor, areaGradientSolidColor * texColor, _RampMixability);
					// 最终的表面色彩
					float3 surfaceCol = lerp(celCol, overlayRamp, _OverlayMixability) + finalSpec + rimlight;
					// 背面取纯黑色和贴图的插值，整体压暗背光面(colorMask.r决定是否有暗色调)
					col.rgb = nl <= 0 ? lerp(surfaceCol, (_SpecularBehind), _SpecularBehind.a * colorMask.r) : surfaceCol;
					// 最后是调整冷暖色调
					col.rgb = GetToneBasedColor(col.rgb, diff);
					// ---------------------------------------

					// 纯色化，强调角色状态
					col.rgb = lerp(col.rgb, _SolidColor.rgb, _SolidColor.a);
					// 颜色对比度下降的控制
					col.rgb = (col.rgb - 0.5)*_ContrastIntansity + 0.5;
					// 提高亮度
					col.rgb += _Brightness;
					// 饱和度
					col.rgb = GetSaturation(col.rgb);
					// 去饱和度(灰化)
					fixed3 grayClr = dot(col.rgb, fixed3(0.3, 0.59, 0.11));
					col.rgb = lerp(col.rgb, grayClr, _GrayScale);
					
					col.a = mainTex.a;

					#endif

					//if(_UseMatCap){
					//	float uvx1 = 0;

					//	float uvx2 = 1 - uvx1;

					//	float2 xy =	s.diffuseUVAndMatCapCoords.xy;
					//	//从提供的MatCap纹理中，提取出对应光照信息
					//	float3 matCapColor = tex2D(_MatCap, xy).rgb * _Value;// > 1 ? tex2D(_MatCap, xy).rgb : 1;
					//	float3 matMaskRCapColor = tex2D(_MatMaskRCap, xy).rgb * _Value;// > 1 ? tex2D(_MatMaskRCap, xy).rgb : 1;
					//	float3 matMaskGCapColor = tex2D(_MatMaskGCap, xy).rgb * _Value;// > 1 ? tex2D(_MatMaskGCap, xy).rgb : 1;
					//	float3 matMaskBCapColor = tex2D(_MatMaskBCap, xy).rgb * _Value;// > 1 ? tex2D(_MatMaskBCap, xy).rgb : 1;
					//	float3 matMaskACapColor = tex2D(_MatMaskACap, xy).rgb * _Value;// > 1 ? tex2D(_MatMaskACap, xy).rgb : 1;

					//	fixed3 mapCapFinal = col.rgb * matCapColor * _MatCapColor;

					//	//mapCapFinal = lerp(col.rgb, mapCapFinal, _Value);
					//	fixed4 mask = tex2D(_MaskMap, s.uv_main);
					//	col.rgb = (step(uvx1, s.uv_main.x) * step(s.uv_main.x, uvx2)) > 0 ? (_UseMatCap > 0 ? mapCapFinal : col.rgb) : col.rgb;
					//	col.rgb = (step(uvx1, s.uv_main.x) * step(s.uv_main.x, uvx2)) > 0 ? (_UseMatCap > 0 ? ((mask.a > 0) ? float3(col.rgb * matMaskACapColor) : col.rgb) : col.rgb) : col.rgb;
					//	col.rgb = (step(uvx1, s.uv_main.x) * step(s.uv_main.x, uvx2)) > 0 ? (_UseMatCap > 0 ? ((mask.r > 0) ? float3(col.rgb * matMaskRCapColor) : col.rgb) : col.rgb) : col.rgb;
					//	col.rgb = (step(uvx1, s.uv_main.x) * step(s.uv_main.x, uvx2)) > 0 ? (_UseMatCap > 0 ? ((mask.g > 0) ? float3(col.rgb * matMaskGCapColor) : col.rgb) : col.rgb) : col.rgb;
					//	col.rgb = (step(uvx1, s.uv_main.x) * step(s.uv_main.x, uvx2)) > 0 ? (_UseMatCap > 0 ? ((mask.b > 0) ? float3(col.rgb * matMaskBCapColor) : col.rgb) : col.rgb) : col.rgb;
					//}
					
					//float attenuation = SHADOW_ATTENUATION(i);
					return col;
					//return lerp(col*_ShadowColor, col, attenuation);
				}
				ENDCG
			}
		}
	}
	FallBack "Diffuse"
}