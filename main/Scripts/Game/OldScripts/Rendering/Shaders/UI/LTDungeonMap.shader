// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LostTemple/UI/LTDungeonMap" 
{
Properties
	{
		_MainTex ("Main Tex", 2D) = "black" {}

		_OutlineTex("Outline Tex", 2D) = "black" {}

		_OutlineTexRange("Outline Tex Range", Vector) = (0, 0.5, 0, 0.5)//表示x的取值范围和y的取值范围
		
		_GreyColor("Grey Color", Color) = (0.275, 0.275, 0.275, 0.1)//灰色遮罩的颜色
	}
	
	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			//Blend SrcAlpha OneMinusSrcAlpha
			Blend Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _OutlineTex;
			float4 _OutlineTexRange;

			float4 _GreyColor;
	
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			v2f o;

			v2f vert (appdata_t v)
			{
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;
				return o;
			}
				
			fixed4 frag (v2f IN) : SV_Target
			{
				half4 col = tex2D(_MainTex, IN.texcoord);

				if (IN.texcoord.x >= _OutlineTexRange.x && IN.texcoord.x <= _OutlineTexRange.y && IN.texcoord.y >= _OutlineTexRange.z && IN.texcoord.y <= _OutlineTexRange.w)
				{
					float2 outlineTexcoord = float2((IN.texcoord.x - _OutlineTexRange.x) / (_OutlineTexRange.y - _OutlineTexRange.x), (IN.texcoord.y - _OutlineTexRange.z) / (_OutlineTexRange.w - _OutlineTexRange.z));
					float4 outlineCol = tex2D(_OutlineTex, outlineTexcoord);
					return lerp(col, outlineCol, outlineCol.a);
				}
				return lerp(col, _GreyColor, _GreyColor.a);
			}
			ENDCG
		}
	}

	/*SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			//ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}*/

	FallBack "Diffuse"
}
