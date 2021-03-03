Shader "Particles/Billboard"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"RenderType"="Transparent"
		}

		Pass
		{
			Cull Off
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag
			
			uniform sampler2D _MainTex;

			struct VertexInput
			{
				float4 Position	: POSITION;
				float4 UV		: TEXCOORD0;
				float4 Color	: COLOR;
			};
			
			struct VertexOutput
			{
				float4 Position	: POSITION;
				float4 UV		: TEXCOORD0;
				float4 Color	: COLOR;
			};
			
			VertexOutput vert(VertexInput input)
			{
				VertexOutput output;
				
				// The basic idea of billboard is to transform only the origin (0,0,0,1) of the object space to
				// view space with the standard model-view transformation UNITY_MATRIX_MV
				output.Position = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0)) +
					float4(input.Position.x, input.Position.y, input.Position.z, 0.0));

				output.UV = input.UV;
				
				output.Color = input.Color;
				
				return output;
			}
			
			float4 frag(VertexOutput input) : COLOR
			{
				return input.Color * tex2D(_MainTex, float2(input.UV.xy));
			}
			
			ENDCG
		}
	}
}