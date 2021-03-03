Shader "Hidden/Enviro/Animate/Water"
{
	Properties 
	{
		_Color("Base Color", Color) = (0.0, 0.0, 0.0, 1.0) 
		_NormalMapTex0("Normal Map 0", 2D) = "bump" {}
		_NormalMapTex1("Normal Map 1", 2D) = "bump" {}
		_Speed0("Speed 0", float) = 1.0
		_Direction0("Direction 0", float) = 0.0
		_Speed1("Speed 1", float) = 1.0
		_Direction1("Direction 1", float) = 1.570795
		_PlanarReflectionColor("Reflection Color (RGB) Puddle (A)", Color) = (0.0, 0.0, 0.0, 1.0)
		_PlanarReflectionRoughnessOffset("Reflection Roughness (XY) Offset(ZW)", vector) = (0.0, 0.1, 0.0, -0.1)
		_SpecularGloss("Specular Gloss Modulation", float) = 100
		_SpecularIntensity("Specular Intensity", float) = 0.5
		_lm ("Lightmap - Do Not Assign", 2D) = "white" {}
	}
	Category
	{
		Tags { "Queue"="Geometry" "LightMode" ="Always" "RenderType"="Opaque" }
		Cull Back
		ZWrite On
		ZTest Lequal
		Blend Off
		
		SubShader
		{
			LOD 400
			
			Pass
			{
				CGPROGRAM
				
				#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
								
				#define EBGWATER_BAKED		
				#define EBGWATER_DOUBLE_NORMAL
				#define EBGWATER_REFLECTIONS
				#define EBGWATER_SPECULAR
				#define EBGWATER_FOG 
				
				#include "Lib/EBG_Water.cginc"
					
				#pragma vertex vertex_lm 
				#pragma fragment fragment_lm 
				
				ENDCG 
			}
		}
		
		SubShader
		{
			LOD 300
			
			Pass
			{
				CGPROGRAM
				
				#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
								
				#define EBGWATER_BAKED		
				#undef EBGWATER_REFLECTIONS
				#define EBGWATER_DOUBLE_NORMAL
				#define EBGWATER_SPECULAR
				#define EBGWATER_FOG 
				
				#include "Lib/EBG_Water.cginc"
					
				#pragma vertex vertex_lm 
				#pragma fragment fragment_lm 
				
				ENDCG 
			}
		}
		
		SubShader
		{
			LOD 200
			
			Pass
			{
				CGPROGRAM
				
				#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
								
				#define EBGWATER_BAKED		
				#undef EBGWATER_REFLECTIONS
				#undef EBGWATER_DOUBLE_NORMAL
				#define EBGWATER_SPECULAR
				#define EBGWATER_FOG 
				
				#include "Lib/EBG_Water.cginc"
					
				#pragma vertex vertex_lm 
				#pragma fragment fragment_lm 
				
				ENDCG 
			}
		}
		
		SubShader
		{
			LOD 100
			
			Pass
			{
				CGPROGRAM
				
				#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
								
				#define EBGWATER_BAKED		
				#undef EBGWATER_REFLECTIONS
				#undef EBGWATER_DOUBLE_NORMAL
				#undef EBGWATER_SPECULAR
				#undef EBGWATER_FOG 
				
				#include "Lib/EBG_Water.cginc"
					
				#pragma vertex vertex_lm 
				#pragma fragment fragment_lm 
				
				ENDCG 
			}
		}
	}
}


