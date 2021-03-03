Shader "Projector/Reticle alpha slide" {
   Properties {
      _ReticleTex ("Reticle", 2D) = "gray" { TexGen ObjectLinear }
      _Alpha ("alpha", color) = (1,1,1,1)
   }

   Subshader {
      Tags { "Queue"="Transparent" }
      Pass {
        ZWrite Off
        Offset -1, -1
        Fog { Mode Off }
        ColorMask RGB
        Blend One OneMinusSrcAlpha //Blended Additive uses RGB 24 Grey scale as Alpha **
        //Blend OneMinusSrcAlpha SrcAlpha //traditional Alpha blend
        
        // AlphaTest Less 0.7
     
        SetTexture [_ReticleTex] {
        
        Combine texture * constant ConstantColor [_Alpha]
          
          Matrix [_Projector]
        }
      }
   }
}

//**Blended Additive

//Pre multiplied 24RGB image 
//texture set to "Alpha from Grayscale
//Grayscale lerps black = 100% additive, white = 100% blended
