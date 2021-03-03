// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

//#include "UnityCG.cginc"
//#include "Lighting.cginc"
//#include "AutoLight.cginc"
sampler2D _MainTex;


float4 _ShadowColor;
#if defined(EBG_T4M_ON)
sampler2D _Splat1;
float4 _Splat1_ST;

sampler2D _Splat2;
float4 _Splat2_ST;

sampler2D _Splat3;
float4 _Splat3_ST;

sampler2D _T4MControl;
float4 _T4MControl_ST;
#endif

#if defined(EBG_DIFFUSE_COLOR)
fixed4 _Color;
#endif

#if defined(EBG_ALPHA_CUTOFF_ON)
    half _Cutoff;
#endif

#if defined(EBG_DETAIL)
sampler2D _DetailTex;
float4 _DetailTex_ST;
#endif

#if defined(EBG_HIDDEN_ON)
sampler2D _lm;
#elif defined(EBG_LIGHTMAP_ON)
// sampler2D unity_Lightmap;
#if defined(EBG_LIGHTMAP_OWN_ON)
UNITY_DECLARE_TEX2D(_Lightmap);
UNITY_DECLARE_TEX2D_NOSAMPLER(_LightmapInd);
#endif
#endif

#if defined(EBG_DIFFUSE_ON)
half _NDotLWrap;
#endif

#if defined(EBG_SH_PROBES)
fixed _SHIntensity;
#endif

#if defined(EBG_NORMAL_MAP_ON)
sampler2D _BumpMap;
float _NormalMapIntensity;
float _NormalMapDamp;
#endif

#if defined(EBG_SPEC_ON) || defined(EBG_EMISSIVE_ON)
sampler2D _SpecEmissiveTex;
#endif

#if defined(EBG_SPEC_ON)
half _SpecularIntensity;
half _SpecularGlossModulation;
#endif

#if defined(EBG_EMISSIVE_ON)
fixed4 _EmissiveColor;
#endif

#if defined(EBG_REFLECTIONS_ON) && !defined(EBG_PLANAR_REFLECTIONS_ON)
fixed4 _ReflectionColor;
#endif

#if defined(EBG_REFLECTIONS_ON) && defined(EBG_PLANAR_REFLECTIONS_ON)
sampler2D _PlanarReflectionTex;
fixed4 _PlanarReflectionColor;
float4 _PlanarReflectionRoughness;
#endif

#if defined(EBG_REFLECTIONS_ON)
half _ReflectionHDR;
#endif

#if defined(EBG_FRESNEL_ON)
half _FresnelScale;
half _FresnelPower;
fixed4 _FresnelColor;
#endif

#if defined(EBG_TWO_TONE_ON)
fixed4 _DiffuseColor0;
fixed4 _DiffuseColor1;
fixed3 _SpecularColor; //custom spec color for two tone
#define EBG_SPEC_COLOR _SpecularColor
#else
#define EBG_SPEC_COLOR _EBGCharLightSpecularColor0.rgb
#endif

#if defined(EBG_EMISSIVE_ON) && defined(EBG_PARALLAX_ON)
float _ParallaxIntensity;
sampler2D _HeightTex;
#endif

struct Input
{
    float4 vertex : POSITION;
    float2 texcoord0 : TEXCOORD0;

#if defined(EBG_NORMAL_MAP_ON) || (!defined(EBG_NORMAL_MAP_ON) && (defined(EBG_DIFFUSE_ON) || defined(EBG_SPEC_ON) || defined(EBG_FRESNEL_ON) || (defined(EBG_REFLECTIONS_ON) && !defined(EBG_PLANAR_REFLECTIONS_ON)))) || defined(EBG_SH_PROBES) || (defined(EBG_POINT_LIGHT) && !defined(EBG_TRANSPARENT)) || defined(EBG_LIGHTMAP_ON)
    float3 normal : NORMAL;
#endif

#if defined(EBG_NORMAL_MAP_ON)
    float4 tangent : TANGENT;
#endif

#if (defined(EBG_HIDDEN_ON) || defined(EBG_LIGHTMAP_ON)) && !defined(EBG_DISABLE_LIGHTMAP_ON)
    float2 texcoord1 : TEXCOORD1;
#endif

#if defined(EBG_DETAIL) || defined(EBG_VERTEX_LIGHTING_ON)
    fixed4 color : COLOR;
#endif
};

struct VtoS
{
    float4 pos : SV_POSITION;

    //LIGHTING_COORDS(8,9)

#if defined(EBG_FOG_ON)
    float3 uv_main_fog : TEXCOORD0;
#else
    float2 uv_main_fog : TEXCOORD0;
#endif

#if defined(EBG_VERTEX_LIGHTING_ON)
    float3 vertexLighting : TEXCOORD1;
#elif (defined(EBG_HIDDEN_ON) || defined(EBG_LIGHTMAP_ON)) && !defined(EBG_DISABLE_LIGHTMAP_ON)
    float2 uv_lm : TEXCOORD1;
#endif

#if defined(EBG_NORMAL_MAP_ON)
    //normal map
    float3 localSurface2World0	: TEXCOORD2;
    float3 localSurface2World1	: TEXCOORD3;
    float3 localSurface2World2	: TEXCOORD4;
#endif

#if defined(EBG_NORMAL_MAP_ON) && (defined(EBG_SPEC_ON) || (defined(EBG_REFLECTIONS_ON) && !defined(EBG_PLANAR_REFLECTIONS_ON)) || defined(EBG_FRESNEL_ON) || (defined(EBG_EMISSIVE_ON) && defined(EBG_PARALLAX_ON)))
    float3 viewDir : TEXCOORD5;
#endif

#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_DIFFUSE_ON) && defined(EBG_SPEC_ON)
    float4 nDotL_spec : TEXCOORD2;
    #define EBG_SPEC_COMPONENT nDotL_spec.w
    #define EBG_NDOTL_COMPONENT nDotL_spec.xyz
#elif !defined(EBG_NORMAL_MAP_ON) && defined(EBG_DIFFUSE_ON)
    float3 nDotL : TEXCOORD2;
    #define EBG_NDOTL_COMPONENT nDotL
#elif !defined(EBG_NORMAL_MAP_ON) && defined(EBG_SPEC_ON)
    float spec : TEXCOORD2;
    #define EBG_SPEC_COMPONENT spec
#endif

#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_FRESNEL_ON)
    fixed3 fresnel : TEXCOORD3;
#endif

#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_PARALLAX_ON) && defined(EBG_EMISSIVE_ON)
    float3 viewDir : TEXCOORD4;
#endif

#if defined(EBG_REFLECTIONS_ON) && defined(EBG_PLANAR_REFLECTIONS_ON)
    float4 proj		: TEXCOORD6;
#elif !defined(EBG_NORMAL_MAP_ON) && defined(EBG_REFLECTIONS_ON) && !defined(EBG_PLANAR_REFLECTIONS_ON)
    float3 reflectionDir : TEXCOORD6;
#endif

#if defined(EBG_POINT_LIGHT) || defined(EBG_DETAIL)
    fixed4 pointLight : TEXCOORD7; //pack detail into a channel
#endif

#if defined(EBG_SH_PROBES)
    fixed3 color : COLOR;
#endif

#if defined(EBG_DYNAMIC_SHADOWS_ON)
    float4 _ShadowCoord : TEXCOORD8;
#endif

#if defined(EBG_LIGHTMAP_ON)
    float3 worldN : TEXCOORD9;
#endif

#if defined(EBG_T4M_ON)
    float2 uv_Splat1 : TEXCOORD10;
    float2 uv_Splat2 : TEXCOORD11;
    float2 uv_Splat3 : TEXCOORD12;
    float2 uv_Control : TEXCOORD13;
#endif
};

#if !defined(EBG_HIDDEN_ON)
float4 _MainTex_ST;
#endif
// float4 unity_LightmapST;

VtoS vertex_shader(Input v)
{
    VtoS data;

    data.pos = UnityObjectToClipPos(v.vertex);

#if defined(EBG_HIDDEN_ON)
    data.uv_main_fog.xy = v.texcoord0.xy;
#else
    data.uv_main_fog.xy = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
#endif

#if defined(EBG_T4M_ON)
    data.uv_Splat1.xy = v.texcoord0.xy * _Splat1_ST.xy + _Splat1_ST.zw;
    data.uv_Splat2.xy = v.texcoord0.xy * _Splat2_ST.xy + _Splat2_ST.zw;
    data.uv_Splat3.xy = v.texcoord0.xy * _Splat3_ST.xy + _Splat3_ST.zw;
    data.uv_Control.xy = v.texcoord0.xy * _T4MControl_ST.xy + _T4MControl_ST.zw;
#endif

#if defined(EBG_FOG_ON)
    data.uv_main_fog.z = EBGFogVertex(v.vertex);
#endif

#if defined(EBG_VERTEX_LIGHTING_ON)
    data.vertexLighting = v.color.gba * 2.0 * _EBGEnvAdjustScale + _EBGEnvAdjustOffset;
#elif defined(EBG_HIDDEN_ON) && !defined(EBG_DISABLE_LIGHTMAP_ON)
    data.uv_lm = v.texcoord1.xy;
#elif defined(EBG_LIGHTMAP_ON) && !defined(EBG_DISABLE_LIGHTMAP_ON)
    data.uv_lm = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif

#if (!defined(EBG_NORMAL_MAP_ON) && (defined(EBG_DIFFUSE_ON) || defined(EBG_SPEC_ON) || defined(EBG_FRESNEL_ON) || (defined(EBG_REFLECTIONS_ON) && !defined(EBG_PLANAR_REFLECTIONS_ON)))) || defined(EBG_SH_PROBES) || (defined(EBG_POINT_LIGHT) && !defined(EBG_TRANSPARENT) || defined(EBG_LIGHTMAP_ON))
    float3 worldN = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
    #if defined(EBG_LIGHTMAP_ON)
        data.worldN = worldN;
    #endif
#endif

#if defined(EBG_NORMAL_MAP_ON)
    float3 local0 = normalize(mul((float3x3)unity_ObjectToWorld, v.tangent.xyz));
    float3 local2 = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
    float3 local1 = normalize(cross(local2, local0) * v.tangent.w);
    data.localSurface2World0 = float3(local0.x, local1.x, local2.x);
    data.localSurface2World1 = float3(local0.y, local1.y, local2.y);
    data.localSurface2World2 = float3(local0.z, local1.z, local2.z);
#endif

#if defined(EBG_NORMAL_MAP_ON) && (defined(EBG_SPEC_ON) || (defined(EBG_REFLECTIONS_ON) && !defined(EBG_PLANAR_REFLECTIONS_ON)) || defined(EBG_FRESNEL_ON) || (defined(EBG_EMISSIVE_ON) && defined(EBG_PARALLAX_ON)))
    data.viewDir = WorldSpaceViewDir(v.vertex);
#endif

#if !defined(EBG_NORMAL_MAP_ON) && (defined(EBG_FRESNEL_ON) || defined(EBG_REFLECTIONS_ON) || defined(EBG_SPEC_ON) || (defined(EBG_PARALLAX_ON) && defined(EBG_EMISSIVE_ON)))
    float3 viewDirNorm = normalize(WorldSpaceViewDir(v.vertex));
#endif

#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_PARALLAX_ON) && defined(EBG_EMISSIVE_ON)
    data.viewDir = viewDirNorm;
#endif

#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_DIFFUSE_ON)
    fixed nDotL = saturate((dot(worldN, _EBGEnvDirectionToLight0.xyz) + _NDotLWrap) / (1 + _NDotLWrap));
    data.EBG_NDOTL_COMPONENT = nDotL * _EBGEnvLightDiffuseColor0.rgb;
#endif

#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_SPEC_ON)
    fixed3 specularDir = reflect(_EBGEnvLightDirection0.xyz, worldN);
    data.EBG_SPEC_COMPONENT = max(0.0, dot(viewDirNorm, specularDir));
#endif

#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_FRESNEL_ON)
    half f = min(1, 1.0 - dot(viewDirNorm, worldN));
    data.fresnel = _FresnelScale * pow(f, _FresnelPower) * _FresnelColor.rgb;
#endif

#if defined(EBG_REFLECTIONS_ON) && defined(EBG_PLANAR_REFLECTIONS_ON)
    data.proj.xyz = data.pos.xyw;
#elif !defined(EBG_NORMAL_MAP_ON) && defined(EBG_REFLECTIONS_ON) && !defined(EBG_PLANAR_REFLECTIONS_ON)
    data.reflectionDir = reflect(-viewDirNorm, worldN);
#endif

#if defined(EBG_POINT_LIGHT) && defined(EBG_TRANSPARENT)
    data.pointLight.rgb = EBGPointLightTransparent(v.vertex);
    data.pointLight.a = 0;
#elif defined(EBG_POINT_LIGHT)
    data.pointLight.rgb = EBGPointLight(v.vertex, worldN);
    data.pointLight.a = 0;
#endif

#if defined(EBG_DETAIL)
    data.pointLight.a = v.color.r;
#endif

#if defined(EBG_SH_PROBES)
    data.color = ShadeSH9(float4(worldN, 1)) * _SHIntensity;
#endif

#if defined(EBG_DYNAMIC_SHADOWS_ON)
    data._ShadowCoord = float4(0,0,0,0);
    TRANSFER_VERTEX_TO_FRAGMENT(data);
#endif

    return data;
}

float4 fragment_shader(VtoS IN) : COLOR0
{
    //main tex
    fixed4 mainTex = tex2D(_MainTex, IN.uv_main_fog.xy);

#if defined(EBG_T4M_ON)
fixed4 splat_control = tex2D(_T4MControl, IN.uv_Control);
fixed4 splat1 = tex2D(_Splat1, IN.uv_Splat1);
fixed4 splat2 = tex2D(_Splat2, IN.uv_Splat2);
fixed4 splat3 = tex2D(_Splat3, IN.uv_Splat3);

mainTex.rgb = splat_control.r * mainTex.rgb + splat_control.g * splat1.rgb + splat_control.b * splat2.rgb + splat_control.a * splat3.rgb;
#endif

#if defined(EBG_DIFFUSE_COLOR)
    mainTex *= _Color;
#endif

#if defined(EBG_DETAIL)
    fixed4 detailTex = tex2D(_DetailTex, IN.uv_main_fog.xy * _DetailTex_ST.xy + _DetailTex_ST.zw);
    fixed3 col = lerp(mainTex.rgb, detailTex.rgb, IN.pointLight.a);
#elif defined(EBG_TWO_TONE_ON)
    fixed3 col = mainTex.rrr * lerp(_DiffuseColor0.rgb, _DiffuseColor1.rgb, mainTex.ggg);
#else
    fixed3 col = mainTex.rgb;
#endif

    col *= _EBGEnvLightColorScale;
    col *= _EBGEnvScale;

#if defined(EBG_NORMAL_MAP_ON) && (defined(EBG_SPEC_ON) || (defined(EBG_REFLECTIONS_ON) && !defined(EBG_PLANAR_REFLECTIONS_ON)) || defined(EBG_FRESNEL_ON) || (defined(EBG_EMISSIVE_ON) && defined(EBG_PARALLAX_ON)))
    float3 viewDirNorm = normalize(IN.viewDir);
#elif !defined(EBG_NORMAL_MAP_ON) && defined(EBG_PARALLAX_ON) && defined(EBG_EMISSIVE_ON)
    float3 viewDirNorm = normalize(IN.viewDir);
#endif

#if defined(EBG_NORMAL_MAP_ON)
    //normal map
    fixed3 normalMapTex = UnpackNormal(tex2D(_BumpMap, IN.uv_main_fog.xy));
    normalMapTex.b = (normalMapTex.b * _NormalMapIntensity) + _NormalMapDamp;
    float3 n;
    n.x = dot(normalMapTex, IN.localSurface2World0);
    n.y = dot(normalMapTex, IN.localSurface2World1);
    n.z = dot(normalMapTex, IN.localSurface2World2);
    n = normalize(n);
#endif

    half3 light = 1;
#if defined(EBG_VERTEX_LIGHTING_ON)
    light = IN.vertexLighting;
#elif defined(EBG_HIDDEN_ON) && !defined(EBG_DISABLE_LIGHTMAP_ON)
    //lightmap
    //light = 2.0 * tex2D(_lm, IN.uv_lm).rgb;
    light = (8.0*tex2D(_lm, IN.uv_lm).a) * tex2D(_lm, IN.uv_lm).rgb;
#elif defined(EBG_LIGHTMAP_ON) && !defined(EBG_DISABLE_LIGHTMAP_ON)
    //lightmap
    //light = DecodeLightmap(tex2D(unity_Lightmap, IN.uv_lm));
    //light = (8.0*UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.uv_lm).a) * UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.uv_lm).rgb; //Mark fixed
    //light = (UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.uv_lm).a) * UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.uv_lm).rgb; //modify by xhx

    
    #if defined(EBG_LIGHTMAP_OWN_ON)
         half3 bakedColor = DecodeLightmap(UNITY_SAMPLE_TEX2D(_Lightmap, IN.uv_lm.xy));
         fixed4 bakedDirTex = UNITY_SAMPLE_TEX2D_SAMPLER(_LightmapInd, _Lightmap, IN.uv_lm.xy);
         light = DecodeDirectionalLightmap(bakedColor, bakedDirTex, IN.worldN);
    #else
         half3 bakedColor = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.uv_lm.xy));
         //fixed4 bakedDirTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd, unity_Lightmap, IN.uv_lm.xy);
         //light = DecodeDirectionalLightmap(bakedColor, bakedDirTex, IN.worldN);
		 light = bakedColor;
    #endif
#endif

#if defined(EBG_NORMAL_MAP_ON) && defined(EBG_DIFFUSE_ON)
    //diffuse
    fixed nDotL = saturate((dot(n, _EBGEnvDirectionToLight0.xyz) + _NDotLWrap) / (1 + _NDotLWrap));
    light *= nDotL * _EBGEnvLightDiffuseColor0.rgb;
#elif defined(EBG_DIFFUSE_ON)
    //diffuse
    light *= IN.EBG_NDOTL_COMPONENT;
#endif

#if defined(EBG_SH_PROBES)
    //sh probes
    light += IN.color;
#endif

    col *= light;

#if defined(EBG_SPEC_ON) || defined(EBG_EMISSIVE_ON)
    //spec/emissive
    fixed3 specEmissiveTex = tex2D(_SpecEmissiveTex, IN.uv_main_fog.xy).rgb;
#endif

#if defined(EBG_SPEC_ON) || defined(EBG_REFLECTIONS_ON) || defined(EBG_FRESNEL_ON) || defined(EBG_HIGHLIGHTS_IGNORE_ALPHA_ON)
    //highlights
    fixed3 highlights = 0;
#endif

#if defined(EBG_NORMAL_MAP_ON) && defined(EBG_SPEC_ON)
    //spec
    fixed3 specularDir = reflect(_EBGEnvLightDirection0.xyz, n);
    half s = max(0.0, dot(viewDirNorm, specularDir));
#elif defined(EBG_SPEC_ON)
    half s = IN.EBG_SPEC_COMPONENT;
#endif
#if defined(EBG_SPEC_ON)
    fixed3 spec = _SpecularIntensity * pow(s, specEmissiveTex.g * _SpecularGlossModulation) * specEmissiveTex.r * EBG_SPEC_COLOR;
    highlights += spec;
#endif

#if defined(EBG_PARALLAX_ON) && defined(EBG_EMISSIVE_ON)
    //parallax emissive
    float parallaxHeight = tex2D(_HeightTex, IN.uv_main_fog.xy).rr;

    parallaxHeight = parallaxHeight * _ParallaxIntensity - (_ParallaxIntensity * 0.5);
    float3 v = viewDirNorm;
    float2 parallaxOffset = parallaxHeight * (v.xy);


    fixed3 emissive = tex2D(_SpecEmissiveTex, IN.uv_main_fog.xy + parallaxOffset).b * _EmissiveColor.rgb;
    col += emissive;
#elif defined(EBG_EMISSIVE_ON)
    //emissive
    fixed3 emissive = specEmissiveTex.b * _EmissiveColor.rgb;
    col += emissive;
#endif

#if defined(EBG_REFLECTIONS_ON) && defined(EBG_PLANAR_REFLECTIONS_ON)
    //planar reflection
    #if defined(EBG_NORMAL_MAP_ON)
    float2 screenUV = (IN.proj.xy * 0.5 - n.xz * _PlanarReflectionRoughness)/IN.proj.z + half2(0.5, 0.5);
    #else
        float2 screenUV = (IN.proj.xy * 0.5)/IN.proj.z + half2(0.5, 0.5);
    #endif
    half2 reflectionUV = screenUV;
    fixed4 planarReflection = tex2D(_PlanarReflectionTex, reflectionUV);
    fixed3 reflection = planarReflection.rgb * 8.0 * _PlanarReflectionColor.rgb;
#elif !defined(EBG_NORMAL_MAP_ON) && defined(EBG_REFLECTIONS_ON) && !defined(EBG_PLANAR_REFLECTIONS_ON)
    float3 reflectionDir = IN.reflectionDir;
#elif defined(EBG_REFLECTIONS_ON)
    float3 reflectionDir = reflect(-viewDirNorm, n);
#endif
#if defined(EBG_REFLECTIONS_ON) && !defined(EBG_PLANAR_REFLECTIONS_ON)
    //reflection
    fixed4 reflectionTex = texCUBE(_EBGCubemap, reflectionDir);
    fixed3 reflection = _ReflectionColor.rgb * reflectionTex.rgb * (1 + reflectionTex.a * _ReflectionHDR);
#endif

#if defined(EBG_REFLECTIONS_ON)

    #if defined(EBG_SPEC_ON)
    //mask by spec intensity
    reflection *= specEmissiveTex.r;
    #endif

    #if defined(EBG_EMISSIVE_ON)
    //mask out where emissive
    fixed emissiveLuminance = dot(fixed3(0.299, 0.587, 0.114), emissive);
    reflection *= (1 - emissiveLuminance);
    #endif

    highlights += reflection;

#endif

#if !defined(EBG_NORMAL_MAP_ON) && defined(EBG_FRESNEL_ON)
    fixed3 fresnel = IN.fresnel;
    highlights += fresnel;
#elif defined(EBG_FRESNEL_ON)
    //fresnel
    half f = min(1, 1.0 - dot(viewDirNorm, n));
    fixed3 fresnel = _FresnelScale * pow(f, _FresnelPower) * _FresnelColor.rgb;
    highlights += fresnel;
#endif

#if defined(EBG_SPEC_ON) || defined(EBG_REFLECTIONS_ON) || defined(EBG_FRESNEL_ON)
    //mix in highlights
    col += highlights;
#endif

#if defined(EBG_TRANSPARENT) && defined(EBG_HIGHLIGHTS_IGNORE_ALPHA_ON)
    //highlights affect transparency
    fixed alpha = max(mainTex.a, dot(fixed3(0.299, 0.587, 0.114), highlights));
#elif defined(EBG_TRANSPARENT)
    //transparency
    fixed alpha = mainTex.a;
#else
    fixed alpha = 1.0;
#endif

#if defined(EBG_FOG_ON)
    col = EBGFogFragment(col, IN.uv_main_fog.z);
#endif

#if !defined(EBG_VERTEX_LIGHTING_ON) && defined(EBG_POINT_LIGHT)
    col = col * (_EBGEnvAdjustScale.rgb + IN.pointLight.rgb) + _EBGEnvAdjustOffset.rgb;
#elif defined(EBG_POINT_LIGHT)
    col += IN.pointLight.rgb;
#elif !defined(EBG_VERTEX_LIGHTING_ON)
    col = col * _EBGEnvAdjustScale + _EBGEnvAdjustOffset;
#endif

#if defined(EBG_DYNAMIC_SHADOWS_ON)
    float  atten = SHADOW_ATTENUATION(IN);
    //col = atten;
    //_ShadowColor = fixed4(255, 216, 197, 255);
    //col = _ShadowColor;
	col = fixed3(col.r * (atten < 1 ? _ShadowColor.r : 1) * atten, col.g * (atten < 1 ? _ShadowColor.g : 1) * atten, col.b * (atten < 1 ? _ShadowColor.b : 1) * atten);
    //col = col * atten;
#endif

#if defined(EBG_ALPHA_CUTOFF_ON)
    clip (alpha - _Cutoff);
#endif
    return fixed4(col, alpha);
}
