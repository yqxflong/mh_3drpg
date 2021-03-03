Shader "Projector/Reticle" {
   Properties {
      _ReticleTex ("Reticle", 2D) = "gray" { TexGen ObjectLinear }
   }

   Subshader {
      Tags { "Queue"="Overlay" }
      Pass {
        ZWrite Off
        Offset -1, -1
        Fog { Mode Off }
        ColorMask RGB
        Blend One OneMinusSrcAlpha //Blended Additive uses RGB 24 Grey scale as Alpha **
        //Blend OneMinusSrcAlpha SrcAlpha //traditional Alpha blend
        
        // AlphaTest Less 0.7
     
        SetTexture [_ReticleTex] {
          // combine texture, one - texture  //use this with Traditional alpha blend only
          Matrix [_Projector]
        }
      }
   }
}

//**Blended Additive

//Pre multiplied 24RGB image 
//texture set to "Alpha from Grayscale
//Grayscale lerps black = 100% additive, white = 100% blended
