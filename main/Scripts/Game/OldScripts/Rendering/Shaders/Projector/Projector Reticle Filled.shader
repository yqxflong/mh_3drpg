// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'

Shader "Projector/Reticle Filled" {
   Properties {
      _ReticleTex ("Texture", 2D) = "gray" { }
      _RedA ("Red A", Color) = (0,0,0,1)
      _RedB ("Red B", Color) = (1,1,1,1)
      _GreenA ("Green A", Color) = (0,0,0,1)
      _GreenB ("Green B", Color) = (0,0,0,1)
   }

   Subshader {
      Tags { "Queue"="Transparent" }
      Pass {
        ZWrite Off
        Offset -1, -1
        Fog { Mode Off }
        ColorMask RGB
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"

        struct v2f
        {
          float4 pos : SV_POSITION;
          float4 uv_Main : TEXCOORD0;
        };

        sampler2D _ReticleTex;
        float4x4 unity_Projector;
        float4 _RedA;
        float4 _RedB;
        float4 _GreenA;
        float4 _GreenB;

        v2f vert(appdata_tan v){
          v2f o;
          o.pos = UnityObjectToClipPos(v.vertex);
          o.uv_Main = mul(unity_Projector, v.vertex);
          return o;
        }

        half4 frag(v2f i) : COLOR {
          half4 tex = tex2Dproj(_ReticleTex, UNITY_PROJ_COORD(i.uv_Main));
          half4 o = tex;

          float fillAlpha = max(min(_RedA.a, 0.97), 0.03);
          if (tex.b < (1 - fillAlpha))
          {
            o.a = 0;
          }

          o.rgb = o.r * _RedA.rgb + (1 - o.r) * _RedB.rgb + o.g * _GreenA.rgb + (1 - o.g) * _GreenB.rgb;
          //o.rgb = o.rbg*o.a;
          return o;
        }

        ENDCG
      }
   }
}

