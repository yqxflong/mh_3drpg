///////////////////////////////////////////////////////////////////////
//
//  CameraLerp.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

[System.Serializable]
public class CameraLerp : ScriptableObject
{
	public enum LerpSmoothing
	{
		linear,
		slowFastSlow,
		fastSlow,
		custom
	}

	public enum LerpStyle
	{
		keepTargetOnScreen, 
		positionToPosition, 
		determineAtRunTime
	}

	public bool isExpanded = false;
	public LerpSmoothing dialogueCameraLerpSmoothing = LerpSmoothing.slowFastSlow;
	public LerpStyle lerpStyle = LerpStyle.determineAtRunTime;
	public LerpSmoothing pitchLerpSmoothing = LerpSmoothing.slowFastSlow;
	public LerpSmoothing yawLerpSmoothing = LerpSmoothing.slowFastSlow;
	public float dialogueCameraLerpTime = 1.5f;
	public float hangonTime = 0.0f;
	[HideInInspector]
	public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
	public AnimationCurve curvePitchLerp = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
	public AnimationCurve curveYawLerp = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

	private const string InitialName = "CameraLerp";

	static public CameraLerp Create()
	{
		CameraLerp model = (CameraLerp)ScriptableObject.CreateInstance<CameraLerp>();
		model.name = InitialName;
		return model;
	}

	static public LerpStyle DetermineBestLerpStyle(GameCameraBehavior from, GameCameraBehavior to)
	{
		if (null != from && null != to)
		{
			Vector3 fromLookAt = Vector3.zero;
			from.GetLookAt(ref fromLookAt);

			Vector3 toLookAt = Vector3.zero;
			to.GetLookAt(ref toLookAt);

			const float CloseTol = 0.5f;
			// if the look at points are close to one and other we should try and keep them on screen throughout 
			if (GameUtils.GetDistSqXZ(fromLookAt, toLookAt) < GameUtils.Square(CloseTol)) 
			{
				return LerpStyle.keepTargetOnScreen;
			}
		}
		return LerpStyle.positionToPosition;
	}
}
