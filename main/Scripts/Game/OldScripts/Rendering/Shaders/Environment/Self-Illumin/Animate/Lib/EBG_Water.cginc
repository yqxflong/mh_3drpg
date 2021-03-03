// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

// define options
// EBGWATER_REFLECTIONS
// EBGWATER_SPECULAR
// EBGWATER_FOG
// EBGWATER_DOUBLE_NORMAL

#include "UnityCG.cginc"	
#include "../../../Lib/EBG_Globals.cginc"
                        
//sampler2D _MainTex;	
fixed4 _Color;
#ifdef EBGWATER_BAKED
    sampler2D _lm;
#else
    // sampler2D unity_Lightmap;
#endif	  

#if defined(EBGWATER_REFLECTIONS) || defined(EBGWATER_SPECULAR)	 

    float _Speed0;
    float _Direction0;
    sampler2D _NormalMapTex0;
    
    #if defined(EBGWATER_DOUBLE_NORMAL)
    
        float _Speed1;
        float _Direction1;
        sampler2D _NormalMapTex1;
    
    #endif
#endif

#ifdef EBGWATER_REFLECTIONS
sampler2D _PlanarReflectionTex;
fixed4 _PlanarReflectionRoughnessOffset;
fixed4 _PlanarReflectionColor;
#endif

#ifdef EBGWATER_SPECULAR
float _SpecularGloss;
float _SpecularIntensity;
#endif

struct Input 
{
    float4 vertex 		: POSITION;
    float4 texcoord0 	: TEXCOORD0;
    float4 texcoord1 	: TEXCOORD1;
};

struct VtoS
{
    float4 position 		: SV_POSITION;
    float2 uv_LightMapTex 	: TEXCOORD0; 
    
#if defined(EBGWATER_REFLECTIONS) && defined(EBGWATER_SPECULAR)

    float2 uv_NormalMap0	: TEXCOORD1; 
    #if defined(EBGWATER_DOUBLE_NORMAL)
        float2 uv_NormalMap1	: TEXCOORD2;  
    #endif
    float4 proj_fog 		: TEXCOORD3;
    float3 viewDir 			: TEXCOORD4;
    #define EBGWATER_VIEWDIR_COMPONENT 	viewDir
    #define EBGWATER_FOG_COMPONENT 		proj_fog.w
    
#elif defined(EBGWATER_REFLECTIONS)

    float2 uv_NormalMap0	: TEXCOORD1; 
    #if defined(EBGWATER_DOUBLE_NORMAL)
        float2 uv_NormalMap1	: TEXCOORD2;  
    #endif 
    float4 proj_fog 		: TEXCOORD3;
    #define EBGWATER_FOG_COMPONENT 		proj_fog.w
    
#elif defined(EBGWATER_SPECULAR)

    float2 uv_NormalMap0	: TEXCOORD1; 
    #if defined(EBGWATER_DOUBLE_NORMAL)
        float2 uv_NormalMap1	: TEXCOORD2;  
    #endif
    float4 viewDir_fog	: TEXCOORD3;
 
    #define EBGWATER_VIEWDIR_COMPONENT 	viewDir_fog.rgb
    #define EBGWATER_FOG_COMPONENT 		viewDir_fog.w
    
#elif defined(EBGWATER_FOG)

    float fog 				: TEXCOORD2;
    #define EBGWATER_FOG_COMPONENT 		fog
    
#endif
};

// float4 unity_LightmapST;
float4 _MainTex_ST;
float4 _NormalMapTex0_ST;
float4 _NormalMapTex1_ST;

VtoS vertex_lm(Input v)
{   
    VtoS data;
    data.position = UnityObjectToClipPos(v.vertex); 
#ifdef EBGWATER_BAKED
    data.uv_LightMapTex = v.texcoord1.xy; 
#else
    data.uv_LightMapTex = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif
#if defined(EBGWATER_REFLECTIONS) || defined(EBGWATER_SPECULAR)
    float2 offset0 = float2(cos(_Direction0), sin(_Direction0)) * _Time.x * _Speed0;
    data.uv_NormalMap0 = v.texcoord0.xy * _NormalMapTex0_ST.xy + _NormalMapTex0_ST.zw + offset0;  
    #if defined(EBGWATER_DOUBLE_NORMAL)
    float2 offset1 = float2(cos(_Direction1), sin(_Direction1)) * _Time.x * _Speed1;
    data.uv_NormalMap1 = v.texcoord0.xy * _NormalMapTex1_ST.xy + _NormalMapTex1_ST.zw + offset1;  
    #endif 
#endif
#ifdef EBGWATER_REFLECTIONS
    data.proj_fog.xyz = data.position.xyw;
    #ifdef EBGWATER_SPECULAR
        data.viewDir = WorldSpaceViewDir(v.vertex);
    #endif
    #ifdef EBGWATER_FOG
        data.proj_fog.w = EBGFogVertex(v.vertex);
    #endif
#elif defined(EBGWATER_SPECULAR)
    data.viewDir_fog.xyz = WorldSpaceViewDir(v.vertex);
    #ifdef EBGWATER_FOG
        data.viewDir_fog.w = EBGFogVertex(v.vertex);
    #endif
#elif defined(EBGWATER_FOG)
    data.fog = EBGFogVertex(v.vertex);
#endif

    return data;
}

fixed4 fragment_lm(VtoS IN) : COLOR0
{
    float2 uv_LightMapTex = IN.uv_LightMapTex;
    
    fixed3 c = _Color.rgb; 
    
#if defined(EBGWATER_REFLECTIONS) || defined(EBGWATER_SPECULAR) 
    //calcalate normal map
    half3 normalMapTex0 = UnpackNormal(tex2D(_NormalMapTex0, IN.uv_NormalMap0));
    #if defined(EBGWATER_DOUBLE_NORMAL)
    half3 normalMapTex1 = UnpackNormal(tex2D(_NormalMapTex1, IN.uv_NormalMap1));
    half3 normalMapTex = normalize(normalMapTex0 + normalMapTex1);
    #else
    half3 normalMapTex = normalMapTex0;
    #endif
#endif
  
#ifdef EBGWATER_REFLECTIONS
    half2 noise = normalMapTex.rg * _PlanarReflectionRoughnessOffset.xy + _PlanarReflectionRoughnessOffset.zw;
    float2 screenUV = (IN.proj_fog.xy * 0.5)/IN.proj_fog.z + half2(0.5, 0.5);
    half2 reflectionUV = screenUV + noise;
    fixed4 reflection = tex2D(_PlanarReflectionTex, reflectionUV);
    c += reflection.rgb * 2.0 * _PlanarReflectionColor * _PlanarReflectionColor.a;
#endif
  
#ifdef EBGWATER_SPECULAR
    float3 halfdir = normalize( normalize( IN.EBGWATER_VIEWDIR_COMPONENT ) - _EBGEnvLightDirection0);
    float specularAmount = max( 0.0, dot(halfdir, normalMapTex.rgb ));
    c += _SpecularIntensity * pow(specularAmount, _SpecularGloss) * _EBGEnvLightSpecularColor0.rgb;
#endif
    
#ifdef EBGWATER_BAKED
    c *= tex2D(_lm, uv_LightMapTex).rgb * 2.0f;
#else
    #if LIGHTMAP_ON
        c *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, uv_LightMapTex));
    #endif
#endif

#ifdef EBGWATER_FOG
    c = EBGFogFragment(c, IN.EBGWATER_FOG_COMPONENT);
#endif
    
    c = c * _EBGEnvAdjustScale + _EBGEnvAdjustOffset;

    return fixed4(c, 0);
}   

