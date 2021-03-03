Shader "EBG/Misc/ContactShadow" 
{
    Properties 
    {
		_MainTex ("Base (RGBA)", 2D) = "white" {}
		_Alpha ("Alpha", float) = 1
    }
    SubShader 
    {
        Tags 
        {
			"Queue"="Transparent" 
			"RenderType"="Environment"
			"LightMode"="ForwardBase"
        }
		LOD 100
		Cull Back
		Lighting Off
		ZWrite Off
		ZTest Lequal
		Blend SrcAlpha OneMinusSrcAlpha
	    Fog { Mode Off }
	    
        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform sampler2D _MainTex; 
            uniform float _Alpha;
            uniform float4x4 _Obj2World;
            
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
                
                float4 pos = mul(_Obj2World, v.vertex);
                pos.y = 0.01;
               	
                o.pos = mul(UNITY_MATRIX_VP, pos);
                o.uv0 = v.uv0;
                return o;
            }
            
            fixed4 frag(VertexOutput i) : COLOR 
            {
                fixed4 tex = tex2D(_MainTex, i.uv0);
                tex.a *= _Alpha;
                return tex;
            }
            ENDCG
        }
    }
}