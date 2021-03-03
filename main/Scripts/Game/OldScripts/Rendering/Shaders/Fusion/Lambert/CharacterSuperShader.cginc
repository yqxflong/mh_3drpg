#ifndef SUPER_SHADER
#define SUPER_SHADER

#include "UnityCG.cginc"

#ifdef HEIGHT_FOG
#include "../../Lib/EBG_Globals.cginc"
#endif

fixed4 _Color;
fixed4 _Emission;
fixed4 _UtilityParameter;
sampler2D _MainTex;

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

#ifdef NORMALMAP
sampler2D _BumpMap;
#endif

float4 _VertexWave;
float4 _WiggleParameter;
float _ClipFactor = 0.0f;

struct Input {
	half2 uv_MainTex;
	//half fog;
	#ifdef REFLECTION	
	float3 worldRefl;
		#ifdef NORMALMAP
		INTERNAL_DATA
		#endif
	#endif	
	#ifdef RIM
	float3 viewDir;
	#endif
	#ifdef HEIGHT_FOG
	float3 worldPos;
	#endif
};

void wigglevert(inout appdata_full v, out Input data)
{			
	float4 WiggleParameter = _WiggleParameter.xyzw * v.color.rgba;
	float3 WaveOffset = _VertexWave.xyz * (sin( v.vertex.xyz * WiggleParameter.x + (_Time.x * WiggleParameter.y) ) - 1.0f) *
		(WiggleParameter.w);   
	v.vertex.xyz += WaveOffset;
}

void surf (Input IN, inout SurfaceOutput o) {
	#ifdef NORMALMAP
	o.Normal = UnpackNormal(tex2D (_BumpMap, IN.uv_MainTex));;
	#endif
				
	#if defined(SPECULAR) || defined(REFLECTION) || defined(GLOW)
	fixed4 utility = tex2D(_UtilityMap, IN.uv_MainTex);
	utility.rgb *= _UtilityParameter.rgb;
	#endif
	
	#ifdef REFLECTION
		#ifdef NORMALMAP
		o.Emission.rgb = _ReflectionColor.rgb * texCUBE( _Cube, WorldReflectionVector(IN, o.Normal) ).rgb;
		#else
		o.Emission.rgb = _ReflectionColor.rgb * texCUBE( _Cube, IN.worldRefl ).rgb;
		#endif
	o.Emission.rgb *= utility.x;
	#endif
	
	#ifdef GLOW
	o.Emission += _Emission * utility.y;
	#endif
	
	#ifdef RIM
	_RimColor.a = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
	o.Emission += _RimColor.rgb * pow(_RimColor.a, _RimPower);
	#endif
	
	#ifdef SPECULAR
	o.Gloss = utility.z;
	o.Specular = _UtilityParameter.a;
	#endif
	
	fixed4 diffuseColor = tex2D(_MainTex, IN.uv_MainTex.xy);
	
	#if !DEATH_OFF
		#if !ALPHA_ON
			clip( frac( dot(IN.uv_MainTex.xy, float2(12.9898f, 78.233f)) ) - _ClipFactor );	// this looks good enough, no need for actual noise
			//clip( frac(sin( dot(i.uv.xy, float2(12.9898f,78.233f)) ) * 43758.5453f) - _ClipFactor );
		#elif ALPHA_ON
			diffuseColor.a *= (1 - _ClipFactor);
		#endif
	#endif
	
	// dye
	#ifdef DYE2
	fixed3 dyeMask = tex2D(_DyeMaskMap, IN.uv_MainTex.xy);
	fixed3 primaryDye = tex2D(_PrimaryGradient, fixed2(dyeMask.g, 0.5f)) * dyeMask.r;
	fixed3 skinDye = tex2D(_SkinGradient, fixed2(dyeMask.g, 0.5f)) * dyeMask.b;	
	diffuseColor.rgb = primaryDye + skinDye + (saturate(1.0f - dyeMask.r - dyeMask.b) * diffuseColor.rgb);
	#elif defined(DYE1)
	fixed3 dyeMask = tex2D(_DyeMaskMap, IN.uv_MainTex.xy);
	fixed3 primaryDye = tex2D(_PrimaryGradient, fixed2(dyeMask.g, 0.5f)) * dyeMask.r;
	diffuseColor.rgb = primaryDye + (saturate(1.0f - dyeMask.r) * diffuseColor.rgb);
	#endif
	
	#ifdef HEIGHT_FOG
	
	#endif
	
	diffuseColor *= _Color;
	
	#ifdef HEIGHT_FOG
	
	fixed f = EBGFogVertexWorldPos(IN.worldPos);
	diffuseColor.rgb = EBGFogFragment(diffuseColor.rgb, f);
	
	#endif
	
	o.Albedo = diffuseColor.rgb;
	o.Alpha = diffuseColor.a;
}

#endif