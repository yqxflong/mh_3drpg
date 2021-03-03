// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EBG/Enviro/Blend/Base" 
{
    Properties 
    {
        _Color ("Main Color", Color) = (1, 1, 1, 1)
        _MainTex ("Base (RGBA)", 2D) = "white" {}
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendModeSrc ("Source Blend Mode", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendModeDst ("Destination Blend Mode", Float) = 10
    }
    SubShader 
    {
        Tags 
        {
            "Queue"="Transparent" 
            "RenderType"="EnvironmentTransparent"
            "LightMode"="ForwardBase"
        }
        LOD 100
        Cull Back
        Lighting Off
        ZWrite Off
        ZTest Lequal
        Blend [_BlendModeSrc] [_BlendModeDst]
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
            fixed4 _Color;
            
            struct VertexInput 
            {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            
            struct VertexOutput 
            {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                fixed4 col : TEXCOORD1;
            };
            
            VertexOutput vert (VertexInput v)
            {
                VertexOutput o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
                o.col = _Color * _EBGEnvAdjustScale;
                return o;
            }
            
            fixed4 frag(VertexOutput i) : SV_Target
            {
                return tex2D(_MainTex, i.uv0) * i.col + _EBGEnvAdjustOffset;
            }
            ENDCG
        }
    }
}
