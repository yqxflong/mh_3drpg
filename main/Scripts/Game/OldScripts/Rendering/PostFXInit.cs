using UnityEngine;
using System.Collections.Generic;

public class PostFXInit : MonoBehaviour {

	[EnumFlags]
	public PerformanceInfo.ePOSTFX fxs;

	static GameObject m_fakeui;
	void ShowFakefx()
	{
		if (m_fakeui == null) 
		{
			// GM.AssetManager.GetAsset<GameObject> ("UIPostfxFake", DownloadResult, GameEngine.Instance.gameObject);
			EB.Debug.LogError("UIPostfxFake这个资源Bundle里没有啊，请确认下！");
		}
		else
		{
			m_fakeui.SetActive(true);
		}
	}
	
	void DownloadResult(string assetname, GameObject go, bool bSuccessed)
	{
		if (bSuccessed)
		{
			m_fakeui = go;
			m_fakeui.SetActive(true);
			UIHierarchyHelper.Instance.Place (m_fakeui, UIHierarchyHelper.eUIType.HUD_Dynamic, UIAnchor.Side.Center);
		}
	}
	
	void HideFakefx()
	{
		if(m_fakeui!=null)
		{
			//Debug.LogError("hide fx");
			//m_fakeui.transform.parent = null;
			m_fakeui.SetActive(false);
			DestroyObject(m_fakeui);
			m_fakeui = null;
		}
	}
	
	void OnDisable()
	{
		HideFakefx ();
	}
	// Use this for initialization
	void Start () 
	{     
		PerformanceInfo.ePOSTFX_QUALITY postfxquality = PerformanceInfo.ePOSTFX_QUALITY.Off;
		if(!(GameFlowControlManager.Instance == null))
		{		
			postfxquality = PerformanceManager.Instance.PerformanceInfo.EnvironmentInfoForScene (GameFlowControlManager.Instance.ActiveStateName).postFXQuality;
		}

		List<PerformanceInfo.ePOSTFX> fxtypes = new List<PerformanceInfo.ePOSTFX>(3);
		if ((fxs & PerformanceInfo.ePOSTFX.Bloom) == PerformanceInfo.ePOSTFX.Bloom)
		{
			fxtypes.Add(PerformanceInfo.ePOSTFX.Bloom);
		}
		if ((fxs & PerformanceInfo.ePOSTFX.Vignette) == PerformanceInfo.ePOSTFX.Vignette)
		{
			fxtypes.Add(PerformanceInfo.ePOSTFX.Vignette);
		}
		if ((fxs & PerformanceInfo.ePOSTFX.Vignette2) == PerformanceInfo.ePOSTFX.Vignette2)
		{
			fxtypes.Add(PerformanceInfo.ePOSTFX.Vignette2);
		}
		if ((fxs & PerformanceInfo.ePOSTFX.FakeVignette) == PerformanceInfo.ePOSTFX.FakeVignette)
		{
			fxtypes.Add(PerformanceInfo.ePOSTFX.FakeVignette);
		}
		if ((fxs & PerformanceInfo.ePOSTFX.ColorGrade) == PerformanceInfo.ePOSTFX.ColorGrade)
		{
			fxtypes.Add(PerformanceInfo.ePOSTFX.ColorGrade);
		}
		if ((fxs & PerformanceInfo.ePOSTFX.ToneMap) == PerformanceInfo.ePOSTFX.ToneMap)
		{
			fxtypes.Add(PerformanceInfo.ePOSTFX.ToneMap);
		}
		if ((fxs & PerformanceInfo.ePOSTFX.Warp) == PerformanceInfo.ePOSTFX.Warp)
		{
			fxtypes.Add(PerformanceInfo.ePOSTFX.Warp);
		}
		if ((fxs & PerformanceInfo.ePOSTFX.RadialBlur) == PerformanceInfo.ePOSTFX.RadialBlur)
		{
			fxtypes.Add(PerformanceInfo.ePOSTFX.RadialBlur);
		}

		for (int i = fxtypes.Count - 1; i >= 0; --i)
		{
			if (System.Array.IndexOf(PerformanceManager.Instance.CurrentEnvironmentInfo.postFX, fxtypes[i]) < 0)
			{
				fxtypes.RemoveAt(i);
			}
		}

		if (fxtypes.Contains(PerformanceInfo.ePOSTFX.FakeVignette))
		{
			ShowFakefx();
			fxtypes.Remove(PerformanceInfo.ePOSTFX.FakeVignette);
		}

		if (postfxquality == PerformanceInfo.ePOSTFX_QUALITY.Low) {
			return;
		}

		PostFXManager.Instance.Init (Camera.main, postfxquality, fxtypes.ToArray());
	}
}
