// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

fixed4 _EBGEnvLightDiffuseColor0;
fixed4 _EBGEnvLightSpecularColor0;
fixed4 _EBGEnvLightDirection0;
fixed4 _EBGEnvDirectionToLight0;
fixed4 _EBGEnvLightColorScale;

fixed4 _EBGCharLightDiffuseColor0;
fixed4 _EBGCharLightDiffuseColor1;
fixed4 _EBGCharLightSpecularColor0;
fixed4 _EBGCharLightDirection0;
fixed4 _EBGCharDirectionToLight0;
fixed4 _EBGCharLightDirection1;
fixed4 _EBGCharDirectionToLight1;
fixed  _EBGCharLightProbeScale;
fixed4  _EBGCharLightScale;

samplerCUBE _EBGCubemap;
samplerCUBE _EBGCubemapBlurry;

fixed4 _EBGFogColor;
half4 _EBGFogParams;
//_EBGFogParams.x = Fog Distance Start
//_EBGFogParams.y = 1 / (Fog Distance End - Fog Distance Start)
//_EBGFogParams.z = Fog Height End
//_EBGFogParams.w = 1 / (Fog Height End - Fog Height Start)

half3 _Gyroscope;

//For particle transforms
float4x4 _Camera2World;

//for environment adjustments
fixed4 _EBGEnvAdjustScale;
fixed4 _EBGEnvAdjustOffset;
float _EBGEnvScale;

inline fixed EBGFogVertexWorldPos(float3 worldPos)
{
	half dist = length(worldPos - _WorldSpaceCameraPos); 
	
	/*	
	//readible version 
	half distFog = saturate((dist - _FogDistStart)/(_FogDistEnd - _FogDistStart)); 
	half heightFogStart = saturate((_FogHeightEnd - _WorldSpaceCameraPos.y)/(_FogHeightEnd - _FogHeightStart)); 
	half heightFogEnd = saturate((_FogHeightEnd - worldPos.y)/(_FogHeightEnd - _FogHeightStart));  
	return fixed((heightFogStart + heightFogEnd) * 0.5 * distFog);
	*/
	
	//packed, optimized down to a single subtract / multiply / saturate, via cpu packing
	half3 distHeight = half3(_EBGFogParams.x, _WorldSpaceCameraPos.y, worldPos.y);
	half3 fogDistHeight = saturate((half3(dist, _EBGFogParams.zz) - distHeight) * _EBGFogParams.yww);
	return fixed(dot(fogDistHeight.yz, half2(0.5, 0.5)) * fogDistHeight.x) * _EBGFogColor.a;
}

inline fixed EBGFogVertex(float4 vert)
{
	float3 worldPos = mul(unity_ObjectToWorld, vert).xyz; 
	return EBGFogVertexWorldPos(worldPos);
}

inline fixed3 EBGFogFragment(fixed3 colourIn, fixed fogAmount)
{
	return lerp(colourIn, _EBGFogColor.rgb, fogAmount);
}	

fixed4x4 _EBGPointLightColor;
half4x4 _EBGPointLightPosition;
half4 _EBGPointLightMultiplier;
half4 _EBGPointLightIntensity;

inline fixed3 EBGPointLightWordPos(half3 worldPos, float3 worldN)
{
	half3 toLight[4];
	half4 diff;
	half4 lengthSq;
	
	toLight[0] = _EBGPointLightPosition[0].xyz - worldPos.xyz;
	toLight[1] = _EBGPointLightPosition[1].xyz - worldPos.xyz ;
	toLight[2] = _EBGPointLightPosition[2].xyz - worldPos.xyz ;
	toLight[3] = _EBGPointLightPosition[3].xyz - worldPos.xyz ;
	
	diff[0] = dot(worldN, normalize(toLight[0]));
	diff[1] = dot(worldN, normalize(toLight[1]));
	diff[2] = dot(worldN, normalize(toLight[2]));
	diff[3] = dot(worldN, normalize(toLight[3]));
	
	diff = saturate(diff);
	lengthSq[0] = dot(toLight[0], toLight[0]);
	lengthSq[1] = dot(toLight[1], toLight[1]);
	lengthSq[2] = dot(toLight[2], toLight[2]);
	lengthSq[3] = dot(toLight[3], toLight[3]);
	
	fixed4 atten = (fixed4)((diff * _EBGPointLightIntensity) / (lengthSq * _EBGPointLightMultiplier + 1.0));
	
	return mul(_EBGPointLightColor, atten);
}

inline fixed3 EBGPointLight(float4 vert, float3 worldN)
{
	half3 worldPos = mul (unity_ObjectToWorld, vert).xyz;
	return EBGPointLightWordPos(worldPos, worldN);
}

//normal-free version

inline fixed3 EBGPointLightTransparentWorldPos(half3 worldPos)
{
	half3 toLight[4];
	half4 lengthSq;
	
	toLight[0] = _EBGPointLightPosition[0].xyz - worldPos.xyz;
	toLight[1] = _EBGPointLightPosition[1].xyz - worldPos.xyz ;
	toLight[2] = _EBGPointLightPosition[2].xyz - worldPos.xyz ;
	toLight[3] = _EBGPointLightPosition[3].xyz - worldPos.xyz ;
	
	lengthSq[0] = dot(toLight[0], toLight[0]);
	lengthSq[1] = dot(toLight[1], toLight[1]);
	lengthSq[2] = dot(toLight[2], toLight[2]);
	lengthSq[3] = dot(toLight[3], toLight[3]);
	
	fixed4 atten = (fixed4)(_EBGPointLightIntensity / (lengthSq * _EBGPointLightMultiplier + 1.0));
	
	return mul(_EBGPointLightColor, atten);
}

inline fixed3 EBGPointLightTransparent(float4 vert)
{
	half3 worldPos = mul (unity_ObjectToWorld, vert).xyz;
	return EBGPointLightTransparentWorldPos(worldPos);
}

// helper

inline fixed EBGLuminance(fixed3 colour)
{
	return dot(colour, fixed3(0.299, 0.587, 0.114));
}
