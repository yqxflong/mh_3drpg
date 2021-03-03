// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LostTemple/UI/LTMengBan"
{
	Properties
	{
	}
	SubShader{
		CGINCLUDE

		#include "UnityCG.cginc"

		uniform float4 _MengBanTransparent;
	    uniform float _MengBanScreenHWRatio;
		uniform float _MengBanRadius;
		uniform float _MengBanAlphaMax;

		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		v2f vert(appdata_img v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);

			o.uv = v.texcoord;

			return o;
		}

		fixed4 frag(v2f IN) : SV_Target
		{
			fixed4 finalColor = fixed4(0, 0, 0, _MengBanAlphaMax);
		    
		    float endx = _MengBanTransparent.x + _MengBanTransparent.z;
			float endy = _MengBanTransparent.y - _MengBanTransparent.w;

			if (IN.uv.x > _MengBanTransparent.x&&IN.uv.x<endx &&
				IN.uv.y>endy&&IN.uv.y < _MengBanTransparent.y)
			{
				finalColor.a = 0;
				float x = IN.uv.x;
				float y = IN.uv.y;
				bool isOK = false;
				float c = _MengBanTransparent.y - _MengBanRadius;
				float squD = 1;

				if (y > c) //判断X夹层
				{
					if (x<_MengBanTransparent.x + _MengBanRadius)
					{
						float b = _MengBanTransparent.x + _MengBanRadius;
						squD = pow(x - b, 2) + pow((y - c), 2);
						isOK = true;
					}
					else if (x>endx - _MengBanRadius)
					{
						float b = endx - _MengBanRadius;
						squD = pow(x - b, 2) + pow((y - c), 2);
						isOK = true;
					}
					else
					{
						float squR = pow(_MengBanRadius, 2);
						squD = pow((y - c), 2);
						finalColor.a = min(_MengBanAlphaMax, _MengBanAlphaMax*squD / squR);
					}
				}
				else if (y < endy + _MengBanRadius)
				{
					c = endy + _MengBanRadius;
					if (x<_MengBanTransparent.x + _MengBanRadius)
					{
						float b = _MengBanTransparent.x + _MengBanRadius;
						squD = pow(x - b, 2) + pow((y - c), 2);
						isOK = true;
					}
					else if (x>endx - _MengBanRadius)
					{
						float b = endx - _MengBanRadius;
						squD = pow(x - b, 2) + pow((y - c), 2);
						isOK = true;
					}
					else 
					{
						float squR = pow(_MengBanRadius, 2);
						squD = pow((y - c), 2);
						finalColor.a = min(_MengBanAlphaMax, _MengBanAlphaMax*squD / squR);
					}
				}
				else //特殊剩余 y夹层
				{
					if (x >= endx - _MengBanRadius)
					{
						float squR = pow(_MengBanRadius, 2);
						squD = pow(x-(endx - _MengBanRadius), 2);
						finalColor.a = min(_MengBanAlphaMax, _MengBanAlphaMax*squD/squR);
					}
					else if(x<= _MengBanTransparent.x + _MengBanRadius)
					{
						float squR = pow(_MengBanRadius, 2);
						squD = pow(x - (_MengBanTransparent.x + _MengBanRadius), 2);
						finalColor.a = min(_MengBanAlphaMax, _MengBanAlphaMax*squD / squR);
					}
				}
				if (isOK)
				{
					/*float f = abs(IN.uv.x - _MengBanTransparent.x);
					f = min(f, abs(IN.uv.x - endx));
					f = min(f, abs(IN.uv.y - _MengBanTransparent.y));
					f = min(f, abs(IN.uv.y - endy));
					finalColor.a = f>_MengBanRadius ? 0 : 0.5 - f / (_MengBanRadius*2);*/
					//finalColor.a = 0.5f;

					float squR = pow(_MengBanRadius, 2);
					if (squD > squR)
					{
						finalColor.a = _MengBanAlphaMax;
					}
					else
					{
						finalColor.a = min(_MengBanAlphaMax, _MengBanAlphaMax*squD / squR);
					}
				}
			}
			return finalColor;
		}

		ENDCG

		Pass {
			CGPROGRAM

			#pragma vertex vert  
			#pragma fragment frag  

			ENDCG
		}
	}
	FallBack Off
}
