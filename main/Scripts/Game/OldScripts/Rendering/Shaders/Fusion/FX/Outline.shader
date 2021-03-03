// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Fusion/FX/Outline" {
	Properties {
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", float) = 0.005
	}
	
	SubShader {
		Tags { "Queue" = "Transparent+1" }
		/*
		// a pass to render the object a lot closer to camera to block the z buffer
		Pass {
			Name "BASE"
			//Cull Back
			//Blend Zero One
			ColorMask 0
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"
	
			struct appdata {
				float4 vertex : POSITION;
			};
			
			struct v2f {
				float4 pos : POSITION;
			};
			
			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.pos.z = 0.0f;//-= 0.3f / o.pos.w;
				return o;
			}
			
			fixed4 frag(v2f i) : COLOR {
				return fixed4(0.0f, 0.0f, 0.0f, 0.0f);
			}
			ENDCG
		}
		*/
		// note that a vertex shader is specified here but its using the one above
		Pass {
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }
			//Cull Front	// uncomment this to show inner details:
			Cull Off
	
			// you can choose what kind of blending mode you want for the outline
			//Blend SrcAlpha OneMinusSrcAlpha // Normal
			//Blend One One // Additive
			//Blend One OneMinusDstColor // Soft Additive
			//Blend DstColor Zero // Multiplicative
			//Blend DstColor SrcColor // 2x Multiplicative
			Blend OneMinusDstAlpha One	

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"
	
			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			
			struct v2f {
				float4 pos : POSITION;
			};
			
			uniform float _Outline;
			uniform fixed4 _OutlineColor;
			
			v2f vert(appdata v) {
				// just make a copy of incoming vertex data but scaled according to normal direction
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				
				float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				float2 offset = TransformViewToProjection(norm.xy);				
				
				o.pos.xy += offset * _Outline * o.pos.z;
//				o.pos.z += 0.2f / o.pos.w;
				return o;
			}
			
			fixed4 frag(v2f i) : COLOR {
				return _OutlineColor;
			}
			ENDCG
		}
	}    
	Fallback "Diffuse"
}