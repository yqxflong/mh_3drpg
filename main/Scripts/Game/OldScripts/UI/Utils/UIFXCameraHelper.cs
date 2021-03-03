///////////////////////////////////////////////////////////////////////
//
//  UIFXCameraHelper.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UIFXCameraHelper : MonoBehaviour 
{
	[System.Serializable]
	public class UICameraParams
	{
		public float size;
		public Vector3 localPosition;
		public Vector3 localRotation;
		public Vector3 localScale;
	}

	private UICameraParams _previousParams = null;
	private IStackableUI _ui = null;
	private System.Type _uiType = null;

	[SerializeField, HideInInspector] public UICameraParams relicRevealCameraParams = null;

	void Awake()
	{
		if (Application.isPlaying)
		{
			_ui = GetComponent<IStackableUI>();
			if (_ui != null)
			{
				_uiType = _ui.GetType();
			}

			UIStack.Instance.onEnstack += OnEnstack;
			UIStack.Instance.onBackstack += OnBackstack;
		}
	}

	void OnDestroy()
	{
		if (UIStack.Instance != null)
		{
			UIStack.Instance.onEnstack -= OnEnstack;
		}
	}

	void OnEnstack(IStackableUI stackable)
	{
		if (stackable.GetType() == _uiType && GetComponent<Camera>() != null)
		{
			_previousParams = GetCameraParams(GetComponent<Camera>());

			if (relicRevealCameraParams != null)
			{
				SetCameraParams(GetComponent<Camera>(), relicRevealCameraParams);
			}
		}
	}

	void OnBackstack(IStackableUI stackable)
	{
		if (stackable.GetType() == _uiType && GetComponent<Camera>() != null)
		{
			SetCameraParams(GetComponent<Camera>(), _previousParams);
		}
	}

	public static UICameraParams GetCameraParams(Camera camera)
	{
		UICameraParams result = new UICameraParams();
		result.size = camera.orthographicSize;
		result.localPosition = camera.transform.localPosition;
		result.localRotation = camera.transform.localEulerAngles;
		result.localScale = camera.transform.localScale;
		return result;
	}

	public static void SetCameraParams(Camera camera, UICameraParams cameraParams)
	{
		camera.orthographicSize = cameraParams.size;
		camera.transform.localPosition = cameraParams.localPosition;
		camera.transform.localEulerAngles = cameraParams.localRotation;
		camera.transform.localScale = cameraParams.localScale;
	}
}
