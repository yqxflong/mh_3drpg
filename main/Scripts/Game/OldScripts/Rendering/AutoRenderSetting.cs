using UnityEngine;

public class AutoRenderSetting : MonoBehaviour
{
	void OnEnable()
	{
		RenderSettingsManager.Instance.SetActiveRenderSettings(name);
	}

	//void OnDisable()
	//{
	//	if (string.IsNullOrEmpty(RenderSettingsManager.Instance.defaultSettings))
	//	{
	//		RenderSettingsManager.Instance.ActivateRenderSettings(name, 0);
	//	}
	//	else
	//	{
	//		RenderSettingsManager.Instance.SetActiveRenderSettings(RenderSettingsManager.Instance.defaultSettings);
	//	}
	//}
}
