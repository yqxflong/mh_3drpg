// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//印庭欣qq17709982创建于2016.3.18;
Shader "ManHuang/Scene/Water" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _ColorFix ("ColorFix", Color) = (0,0.4159702,0.6666667,0.8784314)
        _VFlowSpeed ("VFlowSpeed", Float ) = 0.1
        _UFlowSpeed ("UFlowSpeed", Float ) = 0.1
        _Uoffset ("Uoffset", Range(-1, 1)) = 0.01
        _Voffset ("Voffset", Range(-1, 1)) = -0.1
        _SpecularTex ("SpecularTex", 2D) = "black" {}
        _WarpTex ("WarpTex", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 100
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers  xbox360 xboxone ps3 ps4 psp2 
            #pragma target 2.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _ColorFix;
            uniform sampler2D _WarpTex; uniform float4 _WarpTex_ST;
            uniform float _VFlowSpeed;
            uniform float _UFlowSpeed;
            uniform float _Uoffset;
            uniform float _Voffset;
            uniform sampler2D _SpecularTex; uniform float4 _SpecularTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
////// Lighting:
////// Emissive:
                float4 node_538 = _Time + _TimeEditor;
                float2 node_8133 = float2((i.uv0.r+(node_538.g*_UFlowSpeed)),(i.uv0.g+(node_538.g*_VFlowSpeed)));
                float4 _WarpTex_var = tex2D(_WarpTex,TRANSFORM_TEX(node_8133, _WarpTex));
                float node_9575 = (_WarpTex_var.r*_Uoffset);
                float node_4256 = (_WarpTex_var.g*_Voffset);
                float2 node_5147 = float2((i.uv0.r+node_9575),(i.uv0.g+node_4256));
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(node_5147, _Diffuse));
                float2 node_2329 = (float2(node_9575,node_4256)+i.screenPos.rg);
                float4 _SpecularTex_var = tex2D(_SpecularTex,TRANSFORM_TEX(node_2329, _SpecularTex));
                float3 emissive = ((_Diffuse_var.rgb*_ColorFix.rgb)+_SpecularTex_var.rgb);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,(_ColorFix.a*i.vertexColor.a));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
