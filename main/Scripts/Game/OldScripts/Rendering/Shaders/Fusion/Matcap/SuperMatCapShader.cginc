// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#ifndef SUPER_MATCAP_SHADER
#define SUPER_MATCAP_SHADER

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#ifdef HEIGHT_FOG
#include "../../Lib/EBG_Globals.cginc"
#endif

fixed4 _Color;
fixed4 _Emission;
fixed4 _UtilityParameter;
float _MatCapPush;
sampler2D _MainTex;
sampler2D _MatcapTex;
sampler2D _Illum;

float _ClipFactor = 0.0f;

fixed4 _MainTex_ST;

#if defined(SPECULAR) || defined(REFLECTION) || defined(GLOW)
	sampler2D _UtilityMap;
#endif

#ifdef DYE2
	sampler2D _DyeMaskMap;
	sampler2D _PrimaryGradient;
	sampler2D _SkinGradient;
	#elif defined(DYE1)
		sampler2D _DyeMaskMap;
		sampler2D _PrimaryGradient;
#endif

#ifdef REFLECTION
	fixed4 _ReflectionColor;
	samplerCUBE _Cube;
#endif

#ifdef RIM
	fixed4 _RimColor;
	float _RimPower;
#endif

#if defined(RIM_ON) && !defined(RIM)
	fixed4 _RimColor;
#endif

#ifdef NORMALMAP
	sampler2D _BumpMap;
#endif

struct v2f_matcap {
	float4 pos : SV_POSITION;
	float2 pack0 : TEXCOORD0;
	float2 pack1 : TEXCOORD1;
	#ifdef HEIGHT_FOG
	fixed fog : TEXCOORD2;
	#endif
};

v2f_matcap vertmatcap (appdata_tan v)
{
	v2f_matcap o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.pack0 = TRANSFORM_TEX(v.texcoord, _MainTex);
	
	float3 normal = normalize(v.normal);
	float3 binormal = cross( normal, v.tangent.xyz ) * v.tangent.w;
	float3x3 rotation = float3x3( v.tangent.xyz, binormal, normal );
	
	fixed3 xMatrix = mul(rotation, UNITY_MATRIX_IT_MV[0].xyz);
	fixed3 yMatrix = mul(rotation, UNITY_MATRIX_IT_MV[1].xyz);
	fixed3 tnormal = fixed3 (0,0,1);

	half2 matUV = half2(dot(xMatrix, tnormal), dot(yMatrix, tnormal));
	o.pack1 = matUV * 0.5 + 0.5;
	
	#ifdef HEIGHT_FOG
	o.fog = EBGFogVertex(v.vertex);
	#endif
	
	return o;
}

fixed4 fragmatcap (v2f_matcap IN) : COLOR
{
	fixed4 c = fixed4(0,0,0,1);
	fixed4 tex = tex2D(_MainTex, IN.pack0.xy);

	#if defined(SPECULAR) || defined(REFLECTION) || defined(GLOW) || defined(RIM) || defined(RIM_ON)
		#if defined(SPECULAR)
			fixed4 gloss = fixed4(0,0,0,1);
			fixed4 specular = fixed4(0,0,0,1);
		#endif
		#if defined(REFLECTION) || defined(GLOW) || defined(RIM) || defined(RIM_ON)
			fixed4 emission = fixed4(0,0,0,0);
		#endif
		#if defined(REFLECTION) || defined(GLOW) || defined(RIM)
			fixed4 utility = tex2D(_UtilityMap, IN.uv_MainTex);
			utility.rgb *= _UtilityParameter.rgb;
		#endif
	#endif

	#ifdef REFLECTION
		#ifdef NORMALMAP
			emission.rgb = _ReflectionColor.rgb * texCUBE( _Cube, WorldReflectionVector(IN, o.Normal) ).rgb;
			#else
				emission.rgb = _ReflectionColor.rgb * texCUBE( _Cube, IN.worldRefl ).rgb;
		#endif
		emission.rgb *= utility.x;
	#endif

	#ifdef GLOW
		emission.rgb += _Emission * utility.y;
		//fixed4 etex = tex2D(_Illum, IN.pack0.xy);
	#endif

	#ifdef RIM
		_RimColor.a = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
		emission.rgb += _RimColor.rgb * pow(_RimColor.a, _RimPower);
	#endif

	//read in matcap texture
	fixed4 mtex = tex2D(_MatcapTex, IN.pack1.xy);
	mtex *= _MatCapPush;

	#ifdef RIM_ON
		emission += mtex.a * _RimColor * _RimColor.a; 
	#endif

	//#if !DEATH_OFF
	#if defined(DEATH_ON)
		//#if !ALPHA_ON
		#if defined(ALPHA_OFF)
			clip( frac( dot(IN.uv_MainTex.xy, float2(12.9898f, 78.233f)) ) - _ClipFactor );
		#elif defined(ALPHA_ON)
			c.a *= (1 - _ClipFactor);
		#endif
	#endif

	// dye
	#ifdef DYE2
		fixed3 dyeMask = tex2D(_DyeMaskMap, IN.pack0.xy);
		fixed3 primaryDye = tex2D(_PrimaryGradient, fixed2(dyeMask.g, 0.5f)) * dyeMask.r;
		fixed3 skinDye = tex2D(_SkinGradient, fixed2(dyeMask.g, 0.5f)) * dyeMask.b;	
		tex.rgb = primaryDye + skinDye + (saturate(1.0f - dyeMask.r - dyeMask.b) * tex.rgb);
		#elif defined(DYE1)
			fixed3 dyeMask = tex2D(_DyeMaskMap, IN.pack0.xy);
			fixed3 primaryDye = tex2D(_PrimaryGradient, fixed2(dyeMask.g, 0.5f)) * dyeMask.r;
			tex.rgb = primaryDye + (saturate(1.0f - dyeMask.r) * tex.rgb);
	#endif

	fixed4 albedo = tex;

	#if defined(BAKE_OFF)
		c.rgb = albedo.rgb * mtex.rgb;
		#if defined(ALPHA_ON)
			c.a *= albedo.a;
		#endif
		#ifdef SPECULAR
			gloss = utility.z;
			specular = _UtilityParameter.a;
			// if using spec need to clean up use
			c += specular;
		#endif
		#if defined(REFLECTION) || defined(GLOW) || defined(RIM) || defined(RIM_ON)
			c += emission;
		#endif	
	#elif defined(BAKE_ON)
		c.rgb = albedo.rgb;
	#endif
	
	#ifdef HEIGHT_FOG
	c.rgb = EBGFogFragment(c.rgb, IN.fog);
	#endif

	return c;
}

#endif