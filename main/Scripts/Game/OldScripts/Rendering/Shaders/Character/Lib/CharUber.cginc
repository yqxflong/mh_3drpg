// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

sampler2D _MainTex;

half _NDotLWrap;
#if defined(EBG_RIM_ON)
half _NDotLWrap1;
#endif

#if defined(EBG_NORMAL_MAP_ON)
sampler2D _NormalMap;
#endif
#if defined(EBG_NORMAL_MAP_ON) && defined(EBG_DETAIL_ON)
sampler2D _DetailNormalMap;
half2 _DetailNormalMapTile;
#endif

#if defined(EBG_EMISSIVE_ON)
sampler2D _EmissiveTex;
fixed4 _EmissiveColor;
#endif

#if defined(EBG_SPEC_ON)
sampler2D _SpecTex;
half _SpecularIntensity;
half _SpecularGlossModulation;
#endif
//#if defined(EBG_SPEC_ON) && defined(EBG_ANISOTROPIC_ON)
//sampler2D _AnisotropicTex;
//float _AnisoMix;
//#endif

float _ColorScale;

#if defined(EBG_FRESNEL_ON) && !defined(EBG_BLUEPRINT)
half _FresnelIntensity;
#define EBG_VAL_FRESNEL_INTENSITY _FresnelIntensity
half _FresnelPower;
#define EBG_VAL_FRESNEL_POWER _FresnelPower
fixed4 _FresnelColor;
#define EBG_VAL_FRESNEL_COLOR _FresnelColor.rgb
#endif

#if defined(EBG_ALPHA_CUTOFF_ON)
	half _Cutoff;
#endif

#if defined(EBG_REFLECTIONS_ON)
fixed4 _ReflectionColor;
half _ReflectionHDR;
half _ReflectionFresnelIntensity;
half _ReflectionFresnelPower;
#endif

//Customization Color
#if defined(EBG_COLORCUSTOMIZATION_ON)
fixed4 _Tint1;			 // tint color 1
fixed4 _Tint2;			 // tint color 2
fixed4 _Tint3;			 // tint color 3
fixed4 _Tint4;			 // tint color 4
sampler2D _TintTex;		 // tint color mask texture
#endif

#if defined(EBG_COLORFILTER_ON)
fixed _GrayScale;
fixed _ContrastIntansity;
fixed _Brightness;
fixed4 _FinalColor;
#endif

//BLUEPRINT

#if defined(EBG_BLUEPRINT)
sampler2D _ScanlineTex;
#define EBG_VAL_FRESNEL_INTENSITY (5.0)
#define EBG_VAL_FRESNEL_POWER (5.0)
#define EBG_VAL_FRESNEL_COLOR fixed3(0, 0.9529, 1)
#endif

struct Input
{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
#if defined(EBG_NORMAL_MAP_ON)
	float4 tangent : TANGENT;
#endif
	float2 texcoord0 : TEXCOORD0;
};

struct VtoS
{
	float4 position : SV_POSITION;

#if defined(EBG_BLUEPRINT) || defined(EBG_FOG_ON)
	float3 uv_main_fog : TEXCOORD0;
#else
	float2 uv_main_fog : TEXCOORD0;
#endif

#if defined(EBG_NORMAL_MAP_ON)
	// WITH NORMAL MAPS
	float3 localSurface2World0	: TEXCOORD1;
	float3 localSurface2World1	: TEXCOORD2;
	float3 localSurface2World2	: TEXCOORD3;
#endif

#if defined(EBG_NORMAL_MAP_ON) && (defined(EBG_SPEC_ON) || defined(EBG_FRESNEL_ON) || defined(EBG_REFLECTIONS_ON))
	float3 viewDir : TEXCOORD4;
#endif

#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_RIM_ON)
	// WITHOUT NORMAL MAPS
	float2 nDotL : TEXCOORD1;
#elif !defined(EBG_NORMAL_MAP_ON)
	// WITHOUT NORMAL MAPS
	float nDotL : TEXCOORD1;
#endif

#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_FRESNEL_ON)
	fixed3 fresnel : TEXCOORD2;
#endif

#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_REFLECTIONS_ON)
	float4 reflectionDir_Fresnel : TEXCOORD3;
#endif

//#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_SPEC_ON) && defined(EBG_ANISOTROPIC_ON)
//	fixed3 spec : TEXCOORD4;
//	float3 anisoN : TEXCOORD5;
#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_SPEC_ON)
	half spec : TEXCOORD4;
#endif

#if defined(EBG_POINT_LIGHT)
	fixed3 pointLight : TEXCOORD6;
#endif

#if defined(EBG_SH_PROBES_ON)
	fixed3 color : TEXCOORD7;
#endif

#if defined(EBG_SPEC_ON)
	float2 specCoord : TEXCOORD9;
#endif
#if defined(EBG_DYNAMIC_SHADOWS_ON)
	float4 _ShadowCoord : TEXCOORD8;
	float4 _ShadowColor;
#endif
};

float4 _MainTex_ST;

VtoS vertex_shader(Input v)
{
	VtoS data;

	data.position = UnityObjectToClipPos(v.vertex);

	data.uv_main_fog.xy = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;

#if defined(EBG_TRANSPARENT_MASK) && !defined(EBG_HIDDEN_ON)
	data.maskCoord.xy = v.texcoord0.xy * _MaskTex_ST.xy + _MaskTex_ST.zw;
#endif

#if defined(EBG_SPEC_ON)
		float2 texcoordSpec = v.texcoord0;
		data.specCoord.xy = TRANSFORM_TEX(texcoordSpec, _MainTex);
#endif

#if defined(EBG_BLUEPRINT)
	float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	data.uv_main_fog.z = worldPos.y/3.0;
#elif defined(EBG_FOG_ON)
	data.uv_main_fog.z = EBGFogVertex(v.vertex);
#endif

#if !defined(EBG_NORMAL_MAP_ON) || defined(EBG_SH_PROBES_ON) || defined(EBG_POINT_LIGHT)
	float3 worldN = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
#endif

#if defined(EBG_NORMAL_MAP_ON)
	float3 local0 = normalize(mul((float3x3)unity_ObjectToWorld, v.tangent.xyz));
	float3 local2 = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
	float3 local1 = normalize(cross(local2, local0) * v.tangent.w);
	data.localSurface2World0 = float3(local0.x, local1.x, local2.x);
	data.localSurface2World1 = float3(local0.y, local1.y, local2.y);
	data.localSurface2World2 = float3(local0.z, local1.z, local2.z);
#endif

#if defined(EBG_NORMAL_MAP_ON) && (defined(EBG_SPEC_ON) || defined(EBG_FRESNEL_ON) || defined(EBG_REFLECTIONS_ON))
	data.viewDir = WorldSpaceViewDir(v.vertex);
#endif

#if !defined(EBG_NORMAL_MAP_ON)
	data.nDotL.x = max(0, (dot(worldN, _EBGCharDirectionToLight0.xyz) + _NDotLWrap) / (1 + _NDotLWrap));
#endif

#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_RIM_ON)
	data.nDotL.y = max(0, (dot(mul((half3x3)UNITY_MATRIX_V, worldN), _EBGCharDirectionToLight1.xyz) + _NDotLWrap1) / (1 + _NDotLWrap1));
#endif

#if !defined(EBG_NORMAL_MAP_ON) && (defined(EBG_FRESNEL_ON) || defined(EBG_REFLECTIONS_ON) || defined(EBG_SPEC_ON))
	float3 viewDirNorm = normalize(WorldSpaceViewDir(v.vertex));
#endif

#if !defined(EBG_NORMAL_MAP_ON) && (defined(EBG_FRESNEL_ON) || defined(EBG_REFLECTIONS_ON))
	float f = saturate(1.0f - dot(viewDirNorm, worldN));
#endif

#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_FRESNEL_ON)
	data.fresnel.rgb = EBG_VAL_FRESNEL_INTENSITY * pow(f, EBG_VAL_FRESNEL_POWER) * EBG_VAL_FRESNEL_COLOR;
#endif

#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_REFLECTIONS_ON)
	data.reflectionDir_Fresnel.xyz = reflect(-viewDirNorm, worldN);
	data.reflectionDir_Fresnel.w = _ReflectionFresnelIntensity * pow(f, _ReflectionFresnelPower) + 1.0f;
#endif

//#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_SPEC_ON) && defined(EBG_ANISOTROPIC_ON)
	//aniso specular half angle
//	data.spec = normalize(_EBGCharLightDirection0.xyz + viewDirNorm);
//	data.anisoN = worldN * _AnisoMix;
#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_SPEC_ON)
	//spec calculation
	float3 specularDir = reflect(_EBGCharLightDirection0.xyz, worldN);
	data.spec = max(0.0, dot(viewDirNorm, specularDir));
#endif

#if defined(EBG_POINT_LIGHT)
	data.pointLight = EBGPointLight(v.vertex, worldN);
#endif

#if defined(EBG_SH_PROBES_ON)
	data.color = ShadeSH9(float4(worldN, 1)) * _EBGCharLightProbeScale;
#endif

#if defined(EBG_DYNAMIC_SHADOWS_ON)
	data._ShadowCoord = float4(0, 0, 0, 0);
	TRANSFER_SHADOW(data);
#endif

	return data;
}

fixed4 fragment_shader(VtoS IN) : COLOR0
{
#if defined(EBG_DIFFUSE_ON)
	fixed4 mainTex = fixed4(0.5,0.5,0.5,1);
#else
	fixed4 mainTex = tex2D(_MainTex, IN.uv_main_fog.xy).rgba;
#endif

#if defined(EBG_NORMAL_MAP_ON)
	//normal map
	fixed3 normalMapTex = UnpackNormal(tex2D(_NormalMap, IN.uv_main_fog.xy));
#endif
#if defined(EBG_NORMAL_MAP_ON) && defined(EBG_DETAIL_ON)
	//detail normal map
	fixed3 detailNormalMapTex = UnpackNormal(tex2D(_NormalMap, IN.uv_main_fog.xy * _DetailNormalMapTile.xy));
	normalMapTex += detailNormalMapTex;
#endif
#if defined(EBG_NORMAL_MAP_ON)
	float3 n;
	n.x = dot(normalMapTex, IN.localSurface2World0);
	n.y = dot(normalMapTex, IN.localSurface2World1);
	n.z = dot(normalMapTex, IN.localSurface2World2);
	n = normalize(n);

	half nDotL = max(0, (dot(n, _EBGCharDirectionToLight0.xyz) + _NDotLWrap) / (1 + _NDotLWrap));
#else
	half nDotL = IN.nDotL.x;
#endif

#if defined(EBG_NORMAL_MAP_ON) && (defined(EBG_SPEC_ON) || defined(EBG_FRESNEL_ON) || defined(EBG_REFLECTIONS_ON))
	float3 viewDirNorm = normalize(IN.viewDir);
#endif

#if defined(EBG_EMISSIVE_ON)
	//emissive
	fixed4 emissiveTex = tex2D(_EmissiveTex, IN.uv_main_fog.xy);
	fixed3 emissive = emissiveTex * _EmissiveColor.rgb;
	#if defined(EBG_REFLECTIONS_ON)
		fixed emissiveLuminance = EBGLuminance(emissive);
	#endif
#endif

	//diffuse
#if defined(EBG_SH_PROBES_ON)
	fixed3 res = nDotL * _EBGCharLightDiffuseColor0.rgb + IN.color;
#else
	fixed3 res = nDotL * _EBGCharLightDiffuseColor0.rgb;
#endif

#if defined(EBG_COLORCUSTOMIZATION_ON)
	fixed4 tint_factors = tex2D(_TintTex, IN.uv_main_fog.xy).rgba;
	fixed4 main_factors = tex2D(_MainTex, IN.uv_main_fog.xy).rgba;
	// Blend tint factors using an additive blend
	// This is better than the above multiplicative version in the transitions between tint regions where
	// more than one tint factor is > 0

	fixed4 blend_color  = tint_factors.rrrr * _Tint1.rgba;
	blend_color += tint_factors.gggg * _Tint2.rgba;
	blend_color += tint_factors.bbbb * _Tint3.rgba;
	blend_color += tint_factors.aaaa * _Tint4.rgba;
	// Compensate for (0,0,0,0) tint factors by making the total tint (1, 1, 1, 1) in this case.
	// This is necessary in the multiplicative case above.
	fixed4 zero_compensate = 1.0f - (tint_factors.r + tint_factors.g + tint_factors.b + tint_factors.a);
	blend_color.rgba += zero_compensate.rgba;
	blend_color.rgba *= main_factors;
	mainTex.rgb = blend_color.rgb;
#endif

#if defined(EBG_COLORFILTER_ON)

	mainTex.rgb = lerp(mainTex.rgb,_FinalColor.rgb,_FinalColor.a);
	mainTex.rgb = (mainTex.rgb-0.5)*_ContrastIntansity+0.5;
	mainTex.rgb += _Brightness;

	fixed3 grayClr;
	grayClr.rgb = dot(mainTex.rgb,fixed3(0.3,0.59,0.11));
	mainTex.rgb = lerp(mainTex.rgb,grayClr,_GrayScale);

#endif




	res *= mainTex;



#if defined(EBG_RIM_ON) && defined(EBG_NORMAL_MAP_ON)
	//rim, purposely additive
	half nDotL1 = max(0, (dot(mul((half3x3)UNITY_MATRIX_V, n), _EBGCharDirectionToLight1.xyz) + _NDotLWrap1) / (1 + _NDotLWrap1));
	res += nDotL1 * _EBGCharLightDiffuseColor1.rgb;
#elif defined(EBG_RIM_ON)
	res += IN.nDotL.y * _EBGCharLightDiffuseColor1.rgb;
#endif

#if defined(EBG_POINT_LIGHT)
	//point light
	res += IN.pointLight;
#endif

#if defined(EBG_SPEC_ON)
	//spec
	fixed4 specTex = tex2D(_SpecTex, IN.specCoord.xy);

//#if defined(EBG_ANISOTROPIC_ON)
//	fixed3 anisotropicTex = tex2D(_AnisotropicTex, IN.uv_main_fog.xy);
//	anisotropicTex = normalize(mul((float3x3)_World2Object, anisotropicTex));
//	#if defined(EBG_NORMAL_MAP_ON)
//	fixed3 h = normalize(_EBGCharLightDirection0.xyz + viewDirNorm);
//	fixed HdotA = dot(normalize(anisotropicTex + n * _AnisoMix), h);
//	#else
//	fixed3 h = IN.spec;
//	float3 anisoN = IN.anisoN;
//	fixed HdotA = dot(normalize(anisotropicTex + anisoN), h);
//	#endif
//	half s = abs(sin(HdotA * 3.14159));
#if defined(EBG_NORMAL_MAP_ON)
	fixed3 specularDir = reflect(_EBGCharLightDirection0.xyz, n);
	half s = max(0.0, dot(viewDirNorm, specularDir));
#else
	half s = IN.spec;
#endif
	half gloss = specTex.a * _SpecularGlossModulation;
	fixed3 spec = _SpecularIntensity * pow(s, gloss) * specTex.rgb * _EBGCharLightSpecularColor0;
	res += spec;
#endif

#if defined(EBG_NORMAL_MAP_ON) && (defined(EBG_FRESNEL_ON) || defined(EBG_REFLECTIONS_ON))
	half f = min(1, 1.0f - dot(viewDirNorm, n));
#endif

#if defined(EBG_FRESNEL_ON) && defined(EBG_NORMAL_MAP_ON)
	//fresnel
	fixed3 fresnel = EBG_VAL_FRESNEL_INTENSITY * pow(f, EBG_VAL_FRESNEL_POWER) * EBG_VAL_FRESNEL_COLOR;
	res += fresnel;
#elif defined(EBG_FRESNEL_ON)
	res += IN.fresnel;
#endif

#if defined(EBG_REFLECTIONS_ON) && defined(EBG_NORMAL_MAP_ON)
	//reflections
	float3 reflectionDir = reflect(-viewDirNorm, n);
#elif defined(EBG_REFLECTIONS_ON)
	//reflections
	float3 reflectionDir = normalize(IN.reflectionDir_Fresnel.xyz);
#endif
//#if defined(EBG_REFLECTIONS_ON) && defined(EBG_BLURRY_REFLECTIONS_ON)
	//blurry reflections
	//fixed4 reflectionTex = texCUBE(_EBGCubemapBlurry, reflectionDir);
#if defined(EBG_REFLECTIONS_ON)
	//sharp reflections
	fixed4 reflectionTex = texCUBE(_EBGCubemap, reflectionDir);
#endif
#if defined(EBG_REFLECTIONS_ON)
	fixed3 reflection = _ReflectionColor.rgb * reflectionTex.rgb * (1 + reflectionTex.a * _ReflectionHDR);
#endif
#if defined(EBG_REFLECTIONS_ON) && defined(EBG_SPEC_ON)
	//spec map masks out reflection
	reflection *= specTex.rgb;
#endif
#if defined(EBG_REFLECTIONS_ON) && defined(EBG_EMISSIVE_ON)
	//emission masks out reflection
	reflection *= (1 - emissiveLuminance);
#endif
#if defined(EBG_REFLECTIONS_ON) && defined(EBG_NORMAL_MAP_ON)
	reflection *=_ReflectionFresnelIntensity * pow(f, _ReflectionFresnelPower) + 1.0f;
#elif defined(EBG_REFLECTIONS_ON)
	reflection *= IN.reflectionDir_Fresnel.w;
#endif
#if defined(EBG_REFLECTIONS_ON)
	res += reflection;
#endif

#if defined(EBG_EMISSIVE_ON)
	res = max(emissive, res);
#endif

#if defined(EBG_BLUEPRINT)
	res += tex2D(_ScanlineTex, (IN.uv_main_fog.xy * 1.5) + (_Time.xx * 0.5)).rgb;
	res *= saturate(IN.uv_main_fog.z);
#elif defined(EBG_FOG_ON)
	res = EBGFogFragment(res, IN.uv_main_fog.z);
#endif

	res.rgb *= _EBGCharLightScale;
	res.rgb *= _ColorScale;

#if defined(EBG_TRANSPARENT)
	fixed alpha = mainTex.a;

	#if defined(EBG_ALPHA_CUTOFF_ON)
	clip(alpha - _Cutoff);
	#endif
#else
	fixed alpha = 1.0;
#endif

#if defined(EBG_DYNAMIC_SHADOWS_ON)
	float  atten = SHADOW_ATTENUATION(IN);
	res = fixed3(res.r * (atten < 1 ? _ShadowColor.r : 1) * atten, res.g * (atten < 1 ? _ShadowColor.g : 1) * atten, res.b * (atten < 1 ? _ShadowColor.b : 1) * atten);
	//res = res * atten;
#endif

	return fixed4(res, alpha);
}
