///////////////////////////////////////////////////////////////////////
//
//  Cinematic.cs
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
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class Cinematic : MonoBehaviour 
{
	public enum Space
	{
		worldSpace,
		objectPositionSpace,
		objectPositionOrientationSpace, // not required until it's required
	}		

	[System.Serializable]
	public class Switches // what to turn on/off during the cinematic
	{
		public bool AllowPlayerNavigation = false; // true would mean the normal player state when navigating the environment (so true would mean no action required)
		public bool AllowNormalNPCBehaviour = false; // true would mean the normal npc behavior, navigating the environment, attacking the player etc (so true would mean no action required)
													// false would pause the normal NPC behavior and allow only scripted movement/actions
		public bool HideUI = true;
		public bool displayLetterBoxBars = true;

		public bool ShowSkipButton = false;

		private bool _transitionToStillCamera = false; // this is NOT an option for the inspector,  this is only used at the end of levels

		// do we want navigation off during the cinematic
		public bool IsPlayerNavigationAllowedDuringCinematic()
		{
			return AllowPlayerNavigation;
		}		

		// do we want the NPC's paused during the cinematic
		public bool IsNormalNPCBehaviourAllowed()
		{
			return AllowNormalNPCBehaviour;
		}

		public bool ShouldHideUIDuringCinematic()
		{
			return HideUI;
		}

		public bool AreLetterBoxBarsRequired()
		{
			return displayLetterBoxBars;
		}

		public bool IsTransitionToStillCameraRequired()
		{
			return _transitionToStillCamera;
		}

		public void SetTransitionToStillCameraRequired(bool transitionToStill)
		{
			_transitionToStillCamera = transitionToStill;
		}

		public bool ShouldShowSkipButton()
		{
			return ShowSkipButton;
		}
	}
	public Switches switches = new Switches();

	public CameraLerp.LerpSmoothing cameraLerpSmoothing = CameraLerp.LerpSmoothing.slowFastSlow;
	public Space cameraSpace = Space.worldSpace; // object position space means the camera will receive an object at trigger time which it will be translated to
	[HideInInspector]
	public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

	[System.Serializable]
	public class CameraWayPoint
	{
		[System.NonSerialized]
		public bool isExpanded = false;
		public GameObject wayPointObject = null;
		public GameObject wayPointTarget = null;
		public float lerpInTime = 1f;
		public float holdTime = 0f;
		public float lerpOutTime = 1f;

		public PosAndQuat CalculateTransform(CameraBase cameraComponent)
		{
			if (null == wayPointObject)
			{
				return new PosAndQuat(); // can't do anything without a wayPointObject
			}

			PosAndQuat transform = new PosAndQuat();
			if (null == wayPointTarget) // we have no target, so we get the transform from the normal fixed game camera
			{
				cameraComponent.GetGameCameraTransform(ref transform.position, ref transform.rotation);
				transform.position = wayPointObject.transform.position;
				return transform;
			}

			// get our transform from the vector from wayPointObject to wayPointTarget
			Vector3 forward = wayPointTarget.transform.position - wayPointObject.transform.position;
			transform.position = wayPointObject.transform.position;
			transform.rotation = Quaternion.LookRotation(Vector3.Normalize(forward));
			return transform;
		}
	}
	[HideInInspector] // DrawTableEditor() in CinematicEditor draws this list
	public List<CameraWayPoint> cameraWayPoints = new List<CameraWayPoint>();

	public delegate void ProgressPointDelegate();
	static private ProgressPointDelegate onProgressPoint = null; // a callback for when to progress to the next cinematic camera

	public class PosAndQuat
	{
		public Vector3 position = new Vector3();
		public Quaternion rotation = new Quaternion();
	}

	private CameraBase _cameraComponent = null;
	private PosAndQuat _currentCameraTransform = new PosAndQuat();	

	const float CinematicInactive = -1f;
	private float _cinematicStartTime = CinematicInactive;

	const float UnpauseTime = -1f;
	private float _pauseTime = UnpauseTime; 

	private GameObject _skipButton = null;

#if UNITY_EDITOR
	[System.NonSerialized]
	public Editor theEditor;
#endif

#if DEBUG
	private Vector3 _rigPositionPreStart = new Vector3(); // the position of the rig just before the cinematic plays
	private Quaternion _rigRotationPreStart = new Quaternion(); // the rotation of the rig just before the cinematic plays
#endif

	private static Cinematic _activeCinematic = null;
	private static bool _isPlayFunctionBlocked = false;

	// do we want a callback for a good point to progress cinematic cameras forward?
	static public void AddProgressPointCallback(ProgressPointDelegate callback)
	{
		RemoveProgressPointCallback(callback); // avoid duplicate addition
		onProgressPoint += callback;
	}

	// do we no longer want a callback for the progress point
	static public void RemoveProgressPointCallback(ProgressPointDelegate callback)
	{
		onProgressPoint -= callback;
	}

	// play the specified cinematic
	static public bool Play(Cinematic cinematicComp, GameObject localTo)
	{
		if (null == cinematicComp || _isPlayFunctionBlocked)
		{
			return false;
		}

		_isPlayFunctionBlocked = true; // code in stop can cause this function to get called again, this blocking flag stops that happening

		if (null != _activeCinematic)
		{
			_activeCinematic.Stop();
		}
		_activeCinematic = cinematicComp;
		cinematicComp.Play(localTo);

		_isPlayFunctionBlocked = false;
		return true;
	}

	// pause the active cinematic
	static public void PauseActiveCinematic()
	{
		if (null != _activeCinematic)
		{
			_activeCinematic.Pause();
		}
	}

	// pause the active cinematic
	static public void PauseSpecifiedCinematic(Cinematic cinematicComp)
	{
		if (_activeCinematic == cinematicComp)
		{
			PauseActiveCinematic();
		}
	}

	// unpause the active cinematic
	static public void UnpauseActiveCinematic()
	{
		if (null != _activeCinematic)
		{
			_activeCinematic.Unpause();
		}
	}

	static public void SearchActiveCinematic(float time)
	{
		if (null != _activeCinematic)
		{
			_activeCinematic.Search(time);
		}
	}

	// pause the active cinematic
	static public void UnpauseSpecifiedCinematic(Cinematic cinematicComp)
	{
		if (_activeCinematic == cinematicComp)
		{
			UnpauseActiveCinematic();
		}
	}

	// is the active cinematic paused
	static public bool IsActiveCinematicPaused()
	{
		return (null != _activeCinematic) && _activeCinematic.IsPaused();
	}

	// get the current camera transform
	static public void GetCurrentCinematicTransform(ref Vector3 position, ref Quaternion rotation)
	{
		if (null != _activeCinematic)
		{
			_activeCinematic.GetTransform(ref position, ref rotation);
		}
	}

	// get the switches for the current cinematic - will return null if no cinematic is currently running
	static public Switches GetCurrentCinematicSwitches()
	{
		if (null != _activeCinematic)
		{
			return _activeCinematic.switches;
		}
		return null;
	}

	// apply the appropriate type of lerp smoothinf
	static public float Smooth(float value, CameraLerp.LerpSmoothing smoothing, AnimationCurve animationCurve = null)
	{
		value = Mathf.Clamp01(value);
		switch (smoothing)
		{
			case CameraLerp.LerpSmoothing.linear:
				// no action required
				break;
			case CameraLerp.LerpSmoothing.slowFastSlow:
				// put the following into a graphing calculator: (sin((((value*2)-1)*1.57))+1)*0.5
				value = (Mathf.Sin((((value * 2) - 1) * (0.5f * Mathf.PI))) + 1) * 0.5f;
				break;
			case CameraLerp.LerpSmoothing.fastSlow:
				// put the following into a graphing calculator: cos((value-1)*1.57)
				value = Mathf.Cos((value - 1f) * (0.5f * Mathf.PI));
				break;
			case CameraLerp.LerpSmoothing.custom:
				if(null != animationCurve)
				{
					value = animationCurve.Evaluate(value);
				}
				break;
			default: break;
		}
		return value;
	}

	// will return null if no cinematic is playing
	static public bool IsCinematicActive()
	{
		return null != _activeCinematic;
	}

#if UNITY_EDITOR
	// will return null if no cinematic is playing
	static public Cinematic GetActiveCinematic()
	{
		return _activeCinematic;
	}

	static public bool IsWayPointLerpInActive(CameraWayPoint point)
	{
		if (null != _activeCinematic && null != point)
		{
			CameraWayPoint fromWayPoint = null;
			CameraWayPoint toWayPoint = null;
			_activeCinematic.CalculateFromAndToWayPoint(ref fromWayPoint, ref toWayPoint);
			return (point == toWayPoint && point != fromWayPoint);	
		}
		return false;
	}

	static public bool IsWayPointHoldActive(CameraWayPoint point)
	{
		if (null != _activeCinematic && null != point)
		{
			CameraWayPoint fromWayPoint = null;
			CameraWayPoint toWayPoint = null;
			_activeCinematic.CalculateFromAndToWayPoint(ref fromWayPoint, ref toWayPoint);
			return (point == toWayPoint && point == fromWayPoint);
		}
		return false;
	}

	static public bool IsWayPointLerpOutActive(CameraWayPoint point)
	{
		if (null != _activeCinematic && null != point)
		{
			CameraWayPoint fromWayPoint = null;
			CameraWayPoint toWayPoint = null;
			_activeCinematic.CalculateFromAndToWayPoint(ref fromWayPoint, ref toWayPoint);
			// only the last way point can have an active lerp out, all other way points, the following way point lerp in will override
			return (point == fromWayPoint && (point == _activeCinematic.cameraWayPoints[_activeCinematic.cameraWayPoints.Count - 1]) && point != toWayPoint && null == toWayPoint);
		}
		return false;
	}
#endif
	
	// stop playing the cinematic
	public void Stop()
	{
		if (this == _activeCinematic)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				EditorApplication.update -= EditorUpdate;
			}
#endif

			if (_skipButton != null)
			{
				Destroy(_skipButton);
			}

			_activeCinematic = null;
			Unpause();
			_cinematicStartTime = CinematicInactive;
#if DEBUG
			PositionRigOnEnd();
#endif
			EventManager.instance.Raise(new CinematicEvent(CinematicEvent.CinematicEventType.ending, switches));
			
#if UNITY_EDITOR
			if (null != theEditor)
			{
				theEditor.Repaint();
			}
#endif

			if (null != onProgressPoint)
			{
				onProgressPoint();
			}
		}
	}

	// how long is the total cinematic time
	public float CalculateCinematicLength()
	{
		float time = 0;
		for (int wayPoint = 0; wayPoint < cameraWayPoints.Count; ++wayPoint)
		{
			time += cameraWayPoints[wayPoint].lerpInTime;
			time += cameraWayPoints[wayPoint].holdTime;
			if (cameraWayPoints.Count - 1 == wayPoint) // the last way point
			{
				time += cameraWayPoints[wayPoint].lerpOutTime; // out length is only used on the last way point
			}
		}
		return time;
	}

#if UNITY_EDITOR
	// ensure any incorrectly set-up data is fixed
	public void FixData()
	{
		for (int wayPoint = 0; wayPoint < cameraWayPoints.Count; ++wayPoint)
		{
			if (cameraWayPoints.Count - 1 != wayPoint) // not the last way point
			{
				cameraWayPoints[wayPoint].lerpOutTime = 0f; // out length is only used on the last way point
			}
		}
	}
#endif

	// returns the current time in the cinematic
	public float GetCinematicCurrentTime()
	{
		float currentTime = (GetTime() - _cinematicStartTime);
		return IsPaused() ? Mathf.Min(currentTime, _pauseTime) : currentTime;
	}

	private void OnEnable()
	{
		CacheCamera();
	}

	private void CacheCamera()
	{
		if (!_cameraComponent)
		{
			if (Application.isPlaying)
			{
				if (null != Camera.main)
				{
					_cameraComponent = Camera.main.gameObject.GetComponent<CameraBase>();
				}
			}
			else
			{
				_cameraComponent = GameObject.FindObjectOfType(typeof(CameraBase)) as CameraBase;
			}
		}
	}

	private void Search(float time)
	{
		if (HasCinematicStarted() && time >= 0f && time <= CalculateCinematicLength())
		{
			if (IsPaused())
			{
				_pauseTime = time;
			}
			_cinematicStartTime = GetTime() - time;
		}
	}


	private void OnDisable()
	{
		Stop();	
	}

	// calculate where the camera should be
	private void Update()
	{
		if (IsCinematicPlaying())
		{
			CalculateCameraTransform();
#if UNITY_EDITOR
			if (null != theEditor)
			{
				theEditor.Repaint();
			}
#endif
		}
		else
		{
			Stop();
		}
	}

#if UNITY_EDITOR
	private void EditorUpdate()
	{
		Update();
	}
#endif

	// get the current camera transform
	private void GetTransform(ref Vector3 position, ref Quaternion rotation)
	{
		position = _currentCameraTransform.position;
		rotation = _currentCameraTransform.rotation;
	}

	// calculate the camera transform for our current time in the cinematic
	private void CalculateCameraTransform()
	{
		PosAndQuat fromTransform = new PosAndQuat();
		float fromTime = 0f;

		PosAndQuat toTransform = new PosAndQuat();
		float toTime = 0f;

		CalculateFromAndToTransformAndTime(ref fromTransform, ref fromTime, ref toTransform, ref toTime);
		_currentCameraTransform = InterpolateBetween(fromTransform, fromTime, toTransform, toTime, GetCinematicCurrentTime());
	}

	// start playing the cinematic
	private void Play(GameObject localTo)
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			EditorApplication.update += EditorUpdate; // this makes sure we get a tick in the editor
		}
#endif

		// Change here to re-allow skip button
		if (switches.ShouldShowSkipButton())
		{
			//_skipButton = UIHierarchyHelper.Instance.LoadAndPlace("UI/GlobalUI_Elements/UI_SkipButton", UIHierarchyHelper.eUIType.HUD_Dynamic, UIAnchor.Side.Bottom);
			UIHierarchyHelper.Instance.LoadAndPlaceAsync(go =>
			{
				_skipButton = go;
			}, "UI/GlobalUI_Elements/UI_SkipButton", UIHierarchyHelper.eUIType.HUD_Dynamic, UIAnchor.Side.Bottom);
		}

		Unpause();

#if DEBUG
		_rigPositionPreStart = gameObject.transform.position;
		_rigRotationPreStart = gameObject.transform.rotation;
#endif
		PositionRigOnStart(localTo);
		CacheCamera();
		_cinematicStartTime = GetTime();
		CalculateCameraTransform(); // this initiates our camera transform so it has good values in preperation

		EventManager.instance.Raise(new CinematicEvent(CinematicEvent.CinematicEventType.starting, switches));
	}

	// pause playing the cinematic
	private void Pause()
	{
		if (!IsPaused())
		{
			_pauseTime = GetCinematicCurrentTime();
		}
	}

	// unpause the cinematic
	private void Unpause()
	{
		if (IsPaused())
		{
			Search(_pauseTime);
			_pauseTime = UnpauseTime;
		}
	}

	// is the cinematic currently paused
	private bool IsPaused()
	{
		return _pauseTime >= 0f;
	}

	// position this game object (and children) relative to the passed-in game object
	private void PositionRigOnStart(GameObject localTo)
	{		
		if (null != localTo)
		{
			switch (cameraSpace)
			{
				case Space.objectPositionSpace:
					gameObject.transform.position = localTo.transform.position;
					break;
				case Space.objectPositionOrientationSpace:
					gameObject.transform.position = localTo.transform.position;
					gameObject.transform.rotation = localTo.transform.rotation;
					break;
				default: break;
			}
		}
	}

#if DEBUG
	// reposition the rig after the cinematic ends
	private void PositionRigOnEnd()
	{
		 gameObject.transform.position = _rigPositionPreStart;
		 gameObject.transform.rotation = _rigRotationPreStart;		
	}
#endif

	private bool IsCinematicPlaying()
	{
		return (HasCinematicStarted() && !HasCinematicEnded());
	}

	private bool HasCinematicStarted()
	{
		if (_cinematicStartTime > 0f && this == _activeCinematic)
		{
			return true;
		}
		return false;
	}

	private bool HasCinematicEnded()
	{
		if (HasCinematicStarted() && // we have started the cinematic
			(GetCinematicCurrentTime()) > CalculateCinematicLength())
		{
			return true;
		}
		return false;
	}

	// get the time representation differently based on whether we are playing or not
	private float GetTime()
	{
		if (Application.isPlaying)
		{
			return Time.time;
		}
		else
		{
			return Time.realtimeSinceStartup;
		}
	}

	// calculate which way points we're interpolating between
	private void CalculateFromAndToWayPoint(ref CameraWayPoint fromWayPoint, ref CameraWayPoint toWayPoint)
	{
		PosAndQuat fromTransform = new PosAndQuat();
		float fromTime = 0f;

		PosAndQuat toTransform = new PosAndQuat();
		float toTime = 0f;

		CalculateFromAndToWayPointTransformAndTime(ref fromWayPoint, ref fromTransform, ref fromTime, ref toWayPoint, ref toTransform, ref toTime);
	}

	// calculate which transform's we're interpolating between
	private void CalculateFromAndToTransformAndTime(ref PosAndQuat fromTransform, ref float fromTime,
													ref PosAndQuat toTransform, ref float toTime)
	{
		CameraWayPoint fromWayPoint = null;
		CameraWayPoint toWayPoint = null;
		CalculateFromAndToWayPointTransformAndTime(ref fromWayPoint, ref fromTransform, ref fromTime, ref toWayPoint, ref toTransform, ref toTime);
	}

	// calculate which transform's we're interpolating between
	private void CalculateFromAndToWayPointTransformAndTime(ref CameraWayPoint fromWayPoint, ref PosAndQuat fromTransform, ref float fromTime, 
															ref CameraWayPoint toWayPoint, ref PosAndQuat toTransform, ref float toTime)
	{
		float currentCinematicTime = GetCinematicCurrentTime();

		_cameraComponent.GetGameCameraTransform(ref fromTransform.position, ref fromTransform.rotation);
		toTransform = fromTransform;

		fromWayPoint = null;
		toWayPoint = fromWayPoint;

		fromTime = 0f;
		for (int wayPoint = 0; wayPoint < cameraWayPoints.Count; ++wayPoint)
		{
			CameraWayPoint currentWayPoint = cameraWayPoints[wayPoint];

			fromTime += currentWayPoint.lerpInTime;
			if (currentCinematicTime < fromTime)
			{
				toTime = fromTime;
				toTransform = currentWayPoint.CalculateTransform(_cameraComponent);
				toWayPoint = currentWayPoint;

				fromTime -= currentWayPoint.lerpInTime;
				return;
			}
			fromTransform = currentWayPoint.CalculateTransform(_cameraComponent);
			fromWayPoint = currentWayPoint;

			fromTime += currentWayPoint.holdTime;
			if (currentCinematicTime < fromTime)
			{
				toTime = fromTime;
				toTransform = fromTransform;
				toWayPoint = fromWayPoint;

				fromTime -= currentWayPoint.holdTime;
				return;
			}

			if (cameraWayPoints.Count - 1 == wayPoint) // the last way point
			{
				toTime = fromTime + currentWayPoint.lerpOutTime; // out length is only used on the last way point
			}
		}
	}

	// interpolate between two transforms
	private PosAndQuat InterpolateBetween(PosAndQuat fromTransform, float fromTime, PosAndQuat toTransform, float toTime, float time)
	{
		PosAndQuat interpolatedPosAndQuat = new PosAndQuat();
		float clampedTime = Mathf.Clamp(time, fromTime, Mathf.Max(fromTime, toTime));
		
		float period = (toTime - fromTime);
		const float ZeroTol = 0.0001f;
		float interpolationValue = (period > ZeroTol) ? (clampedTime - fromTime) / period : 0f;
		interpolationValue = Smooth(interpolationValue, cameraLerpSmoothing);

		interpolatedPosAndQuat.position = Vector3.Lerp(fromTransform.position, toTransform.position, interpolationValue);
		interpolatedPosAndQuat.rotation = Quaternion.Slerp(fromTransform.rotation, toTransform.rotation, interpolationValue);

		return interpolatedPosAndQuat;
	}
}
