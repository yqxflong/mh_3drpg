using UnityEngine;
using System.Collections;

public class PostfxFakeRendersetting : MonoBehaviour {
	
	// Update is called once per frame
	public UITexture m_Tex;

	Color colorcache;
	float alphacache;

	void Update () {
		RenderSettings rs = (RenderSettings)RenderSettingsManager.Instance.GetCurrentRenderSettings();	
		if(colorcache!=rs.PostFXVignetteColor)
		{
			colorcache = rs.PostFXVignetteColor;
			m_Tex.color = colorcache*0.2f;
		}
		if(alphacache!=rs.PostFXVignetteIntensity)
		{
			alphacache = rs.PostFXVignetteIntensity;
			m_Tex.alpha =alphacache;
		}
	}
}
