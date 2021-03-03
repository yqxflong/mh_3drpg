using UnityEngine;

[System.Serializable]
public class CameraMotion : ScriptableObject
{
	public bool isExpanded = false; //editor only
	public CameraLerp cameraLerpOverride;
	public CameraParams camera;

	private const string InitialName = "Camera Motion";

	public static CameraMotion Create()
	{
		CameraMotion model = (CameraMotion)ScriptableObject.CreateInstance<CameraMotion>();
		model.name = InitialName;
		return model;
	}
}

