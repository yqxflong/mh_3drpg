// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

struct Input 
{
	float4 vertex : POSITION;
	float4 texcoord0 : TEXCOORD0;
	fixed4 color : COLOR;
};

struct VtoS
{
	float4 position : SV_POSITION;	
	float2 uv_MainTex : TEXCOORD0; 
	#if defined(EBG_NORMAL_MAP_ON) || defined(EBG_FRESNEL_ON)
	float4 pointLight_fade: TEXCOORD1;
	#endif
	fixed4 color : COLOR;
};

sampler2D _MainTex;
float4 _MainTex_ST;
#ifdef EBG_HUE_SHIFT_ON
fixed4 _HueShiftR;
fixed4 _HueShiftG;
fixed4 _HueShiftB;
#endif
#if defined(EBG_NORMAL_MAP_ON)
half _PointLightIntensity;
#endif
#if defined(EBG_FRESNEL_ON)
float _GroundHeight;
float _HeightFade;
#endif
float2 _UVScroll;

VtoS vertex_lm(Input v)
{   
	VtoS data;
	data.position = UnityObjectToClipPos(v.vertex);
	data.uv_MainTex = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
	data.uv_MainTex += _UVScroll * _Time.yy;
	data.color = v.color;
	#if defined(EBG_NORMAL_MAP_ON) || defined(EBG_FRESNEL_ON)
	float3 worldPos = mul(_Camera2World, v.vertex).xyz;
	#endif
	
#if defined(EBG_NORMAL_MAP_ON) || defined(EBG_FRESNEL_ON)
	data.pointLight_fade = float4(0, 0, 0, 0);
#endif

	#if defined(EBG_NORMAL_MAP_ON)
	data.pointLight_fade.rgb = EBGPointLightTransparentWorldPos(worldPos) * _PointLightIntensity;
	#endif
	
	#if defined(EBG_FRESNEL_ON)
	data.pointLight_fade.a = (worldPos.y - _GroundHeight)/(_HeightFade - _GroundHeight); 
	#endif
	
	return data;
}

fixed4 fragment_lm(VtoS IN) : COLOR0
{
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
	
	#if defined(EBG_FRESNEL_ON)
		//fading
		#if defined(EBG_EMISSIVE_ON)
			//additive
			c.rgb *= saturate(IN.pointLight_fade.a);
		#else
			c.a *= saturate(IN.pointLight_fade.a);
		#endif
		
	#endif
	
	#ifdef EBG_HUE_SHIFT_ON
	fixed r = dot(c, _HueShiftR);
	fixed g = dot(c, _HueShiftG);
	fixed b = dot(c, _HueShiftB);
	c = fixed4(r, g, b, c.a);
	#endif
	
	#if defined(EBG_NORMAL_MAP_ON)
		//point light
		#if defined(EBG_EMISSIVE_ON)
			//additive
			c.rgb += IN.pointLight_fade.rgb * EBGLuminance(c.rgb);
		#else
			c.rgb += IN.pointLight_fade.rgb * c.a; 
		#endif
		
	#endif
	
	#if defined(EBG_SPEC_ON)
		c.rgb = c.rgb * _EBGEnvAdjustScale + _EBGEnvAdjustOffset;
	#endif

	return c;
}   