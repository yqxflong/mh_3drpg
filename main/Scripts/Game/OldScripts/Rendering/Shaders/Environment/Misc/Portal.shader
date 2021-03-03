// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "EBG/Enviro/Misc/Portal" 
{
    Properties 
    {
        _Background ("Background", 2D) = "white" {}
        _Mask ("Mask", 2D) = "white" {}
        _BackgroundScale ("Background Scale", Vector) = (1,1,0,0)
        _ParallaxDepth ("Parallax Depth", Float ) = 1
        _Offset ("Offset", Vector) = (0,0,0,0)
        _EmissiveTint ("Emissive Tint", Color) = (0.5,0.5,0.5,1)
    }
    SubShader 
    {
        Tags 
        {
            "RenderType"="Opaque"
            "LightMode"="ForwardBase"
        }
        LOD 200
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
            uniform sampler2D _Background; 
            uniform sampler2D _Mask; 
            uniform float4 _BackgroundScale;
            uniform float _ParallaxDepth;
            uniform float4 _Offset;
            uniform float4 _EmissiveTint;
            
            struct VertexInput 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            
            struct VertexOutput 
            {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
            };
            
            VertexOutput vert (VertexInput v)
             {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = normalize(mul(float4(v.normal,0), unity_WorldToObject).xyz);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            fixed4 frag(VertexOutput i) : COLOR 
            {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 mask = tex2D(_Mask, i.uv0.xy );
                float2 parralax_uv = (_ParallaxDepth * (mask.r - 1.0) * mul(tangentTransform, viewDirection).xy) + (i.uv0.xy * _BackgroundScale.rg) + _Offset.xy;
                fixed4 c = tex2D(_Background, parralax_uv ) * (1.0 - mask.g) + ( mask.b * _EmissiveTint );
                c = c * _EBGEnvAdjustScale + _EBGEnvAdjustOffset;
                return c;
            }
            ENDCG
        }
    }
    SubShader 
    {
        Tags 
        {
            "RenderType"="Opaque"
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
            uniform sampler2D _Background; 
            
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
                o.uv0 = v.uv0.xy;
                return o;
            }
            
            fixed4 frag(VertexOutput i) : COLOR 
            {
                fixed4 c = tex2D(_Background, i.uv0 );
                c = c * _EBGEnvAdjustScale + _EBGEnvAdjustOffset;
                return c;
            }
            ENDCG
        }
    }
}
