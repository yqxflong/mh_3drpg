using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GlobalCameraMotionData : ScriptableObject
{
	public static GlobalCameraMotionData _instance;
	public static GlobalCameraMotionData Instance
	{
		get 
		{
			return _instance;
		}
	}

	public static void Init(System.Action<bool> fn)
    {
		EB.Assets.LoadAsync(GetDataPath(), typeof(GlobalCameraMotionData), o =>
		{
			if(o){
				_instance = o as GlobalCameraMotionData;
				fn(true);
			}
		});
	}


	public List<CameraMotion> _motions = new List<CameraMotion>();
	
	
	public static string GetDataPath()
	{
		return "Bundles/DataModels/Camera/GlobalCameraMotion";
	}
	
	public CameraMotion GetCameraMotion(string name)
	{
		int count = _motions.Count;
		for(int i = 0; i < count; i++)
		{
			if(name.Equals(_motions[i].name))
			{
				return _motions[i];
			}
		}
		
		return null;
	}
}
