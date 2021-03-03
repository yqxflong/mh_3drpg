using UnityEngine;

public class VFXPerformanceControll : MonoBehaviour {

	GameObject HDVersion = null;
	GameObject SDVersion = null;

	void Start()
	{

	}

	void OnEnable()
	{
		if(HDVersion==null||SDVersion==null)  
		{
			HDVersion = gameObject.transform.Find("HD").gameObject;
			SDVersion = gameObject.transform.Find("SD").gameObject;
		}
	
		if(GameFlowControlManager.Instance == null)
		{
			return;
		}
		
		PerformanceInfo.eVFX_QUALITY vfx = PerformanceManager.Instance.PerformanceInfo.EnvironmentInfoForScene (GameFlowControlManager.Instance.ActiveStateName).vfxQuality;

		switch(vfx)
		{
		case PerformanceInfo.eVFX_QUALITY.HD:
			for(int i=0;i<HDVersion.transform.childCount;i++)
			{
				HDVersion.transform.GetChild(i).gameObject.SetActive(true);
			}
			break;
		case PerformanceInfo.eVFX_QUALITY.SD:
			for(int i=0;i<HDVersion.transform.childCount;i++)
			{
				HDVersion.transform.GetChild(i).gameObject.SetActive(true);
			}
			break;
		case PerformanceInfo.eVFX_QUALITY.OFF:
			break;
		default:
			break;
		}
	}
	void OnDisable()
	{
		for(int i=0;i<HDVersion.transform.childCount;i++)
		{
			HDVersion.transform.GetChild(i).gameObject.SetActive(false);
		}

		for(int i=0;i<SDVersion.transform.childCount;i++)
		{
			SDVersion.transform.GetChild(i).gameObject.SetActive(false);
		}

	}
}
