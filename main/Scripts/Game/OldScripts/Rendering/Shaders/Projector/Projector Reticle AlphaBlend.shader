// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'

Shader "Projector/Reticle Alpha Blend" {
   Properties {
      _ReticleTex ("Texture", 2D) = "gray" {}
      _Color ("Main Color", Color) = (0,0,0,1)
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
        float4 _Color;

        v2f vert(appdata_tan v){
          v2f o;
          o.pos = UnityObjectToClipPos(v.vertex);
          o.uv_Main = mul(unity_Projector, v.vertex);
          return o;
        }

        half4 frag(v2f i) : COLOR {
          half4 tex = tex2Dproj(_ReticleTex, UNITY_PROJ_COORD(i.uv_Main));

          return tex * _Color;
        }

        ENDCG
      }
   }
}

