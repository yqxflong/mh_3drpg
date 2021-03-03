// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Enviro/Unlit/Base" 
{
    Properties 
    {
		_MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader 
    {
        Tags 
        {
			"Queue"="Geometry"
			"RenderType"="UnlitBase"
			"LightMode"="ForwardBase"
        }
		LOD 100
		Cull Back
		Lighting Off
		ZWrite On
		ZTest Lequal
		Blend Off
	    Fog { Mode Off }
	    
        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
			#include "../../Lib/EBG_Globals.cginc"
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform sampler2D _MainTex; 
            uniform float4 _MainTex_ST;
            
            struct VertexInput 
            {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            
            struct VertexOutput 
            {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            
            VertexOutput vert (VertexInput v)
             {
                VertexOutput o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
                return o;
            }
            
            fixed4 frag(VertexOutput i) : COLOR 
            {
                fixed4 c = tex2D(_MainTex, i.uv0);
               	c = c * _EBGEnvAdjustScale + _EBGEnvAdjustOffset;
               	return c;
            }
            ENDCG
        }
    }
}
