///////////////////////////////////////////////////////////////////////
//
//  SelectionLogic.cs
//
//  Copyright (c) 2006-2014 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.Method;

public class SelectionLogic : MonoBehaviour
{
	//Editor variables
	public float fingerRadius = 1.0f;
	
	private static int _characterLayers = 0;
	public static int CharacterLayerMask
	{
		get
		{
			return _characterLayers;
		}
		set
		{
			_characterLayers = value;
		}
	}
	
	private static int _groundLayer = 0;
	public static int GroundLayerMask
	{
		get
		{
			return _groundLayer;
		}
		set
		{
			_groundLayer = value;
		}
	}
	
	private int             _firstFingerID;
	private EB.Sparx.Player _owner;
	
	private bool _touchStartedOnPlayer;
	private bool _playerLongPressStarted = false;
	private float _singleTouchStartTime;
	private bool _touchStartOverUI;
	private bool _longPressStarted;
	private float _lastTapTime;
	private bool _didDoubleTap;
	private const float TapLength = 0.15f;
	private const float DoubleTapLength = 0.35f;
	
	private Transform _playerTransform;
	private Transform _muzzleTransform;
	
	private bool _areControlsEnabled = true;

    public static bool IsShowJoystick = false;

    /// <summary>
    /// 外部需要控制
    /// </summary>
    public bool AreControlsEnabled
    {
        get
        {
            return _areControlsEnabled;
        }
        set
        {
            _areControlsEnabled = value;
        }
    }

    private	bool _areSingleTouchEnabled = true;
	private class TouchHistory
	{
		public float _time;
		public Vector3 _position;
	}
	
	private const int TOUCH_HISTORY_LENGTH = 60;
	
	private TouchHistory[] _touchHistory;
	private int m_touch_history_index;

    private UIRoot mRoot = null;
	
	private static int _maxTouches = DEFAULT_MAX_TOUCHES;
	public static int MaxTouches
	{
		get 
		{
			return _maxTouches;
		}
		set 
		{
			_maxTouches = value;
		}
	}

    public const int DEFAULT_MAX_TOUCHES = 2;
	
	private static int _defaultCharacterLayers = 0;
	private static int _defaultGroundLayers = 0;
	
	// don't allow selection
	public void DisablePlayerSelectionControls()
	{
		_characterLayers = 0;
		_groundLayer = 0;
		AreControlsEnabled = false;					
	}
	
	// allow selection
	public void EnablePlayerSelectionControls()
	{
		_characterLayers = _defaultCharacterLayers;
		_groundLayer = _defaultGroundLayers;
		AreControlsEnabled = true;
	}

	// don't allow  single selection
	public void DisableSingleTouch()
	{
		_areSingleTouchEnabled = false;
    }

	// allow single selection
	public void EnableSingleTouch()
	{
		_areSingleTouchEnabled = true;
	}

	void Awake()
	{
		_defaultCharacterLayers = (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Interactable"));
		_defaultGroundLayers = (1 << LayerMask.NameToLayer("Ground"));
		
		_characterLayers = _defaultCharacterLayers;
		_groundLayer = _defaultGroundLayers;
		
		_touchHistory = new TouchHistory[TOUCH_HISTORY_LENGTH];
		for (int i = 0; i < TOUCH_HISTORY_LENGTH; i++) {
			_touchHistory [i] = new TouchHistory ();
		}
		ResetTouchHistory ();
	}
	
	void ResetTouchHistory()
	{
		m_touch_history_index = 0;

		for (int i = 0; i < TOUCH_HISTORY_LENGTH; i++) {
			_touchHistory[i]._time = 0.0f;
			_touchHistory[i]._position = Vector3.zero;
		}
	}
	
	void Start()
	{
		ReplicationView view = GetComponent<ReplicationView>();
		_owner = view != null ? view.instantiatorPlayer : null;
		
		_playerTransform = transform;
		Controller c = _playerTransform.GetComponent<Controller>();
		_muzzleTransform = _playerTransform;
        if (c != null && c.SkinnedRigPrefab != null)
        {
            _muzzleTransform = GameUtils.SearchHierarchyForBone(c.SkinnedRigPrefab.transform, "muzzle");
        }
        mRoot = GameObject.FindObjectOfType<UIRoot>();

    }
	//热更调用，新增字段
	bool isGuideInstanceNull;
	bool StopLongPressInMove;
	void Update()
    {
        if (!GameEngine.Instance.IsTimeToRootScene)
        {
            return;
        }
        //如果是副本中则不能进入rvo
        /*if (LTInstanceMapModel.Instance.IsInstanceMap())
        {
            return;
        }*/
        if (!AreControlsEnabled)
		{
			return;
		}
		
		if (GameVars.paused || TouchController.Instance == null)
			return;
		
		int numTouches = System.Math.Min(TouchController.Instance.ActiveTouches.Count, MaxTouches);
		
		if (_touchStartOverUI)
		{
			for (int i = 0; i < numTouches; i++)
			{
				TouchWrapper touch = TouchController.Instance.ActiveTouches[i];
				if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				{
					_touchStartOverUI = false;
				}
			}
			return;
		}
		
		// make sure our events aren't being handled by NGUI
		if (UICamera.IsOverUIByRoot(mRoot))
		{
			for (int i = 0; i < numTouches; i++)
			{
				if (TouchController.Instance.ActiveTouches[i].phase == TouchPhase.Began)
				{
					_touchStartOverUI = true;
					return;
				}
			}
		}
		
		if (numTouches == 1)
		{
			if (!_areSingleTouchEnabled) return;
			TouchWrapper touch = TouchController.Instance.ActiveTouches[0];

            Transform target = null;
            Vector3 location = Vector3.zero;
            Vector3 groundPosition = Vector3.zero;
            Vector3 direction = Vector3.zero;
            bool isFindTarget = false;
			if (IsShowJoystick)
			{
                isFindTarget = true;
            }
            else
            {
                isFindTarget = FindTargetAndLocation(touch, out target, out location, out groundPosition, out direction);
            }
            if (touch.phase == TouchPhase.Began)
			{
				_firstFingerID = touch.fingerId;
                StartSingleTouch(groundPosition, isFindTarget);
            }

            if (touch.phase == TouchPhase.Began || (touch.fingerId == _firstFingerID && touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled))
            {
                UpdateSingleTouch(touch, target, location, direction, groundPosition, isFindTarget);
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                EndSingleTouch(touch, target, location, direction, groundPosition, isFindTarget);
                _touchStartOverUI = false;
            }
        }
		else if (numTouches > 1)
		{
			TouchWrapper firstTouch = TouchController.Instance.ActiveTouches[0];
			TouchWrapper secondTouch = TouchController.Instance.ActiveTouches[1];

            Transform target1 = null;
            Vector3 position1= Vector3.zero;
            Vector3 direction1 = Vector3.zero;
            Vector3 groundPosition1 = Vector3.zero;
            bool isFindFirstTarget = false;
			if (IsShowJoystick)
			{
                isFindFirstTarget = true;
            }
            else
            {
                isFindFirstTarget = FindTargetAndLocation(firstTouch, out target1, out position1, out groundPosition1, out direction1);
            }

            Transform target2 = null;
            Vector3 position2 = Vector3.zero;
            Vector3 direction2 = Vector3.zero;
            Vector3 groundPosition2 = Vector3.zero;
            bool isFindSecondTarget = false;
			if (IsShowJoystick)
			{
                isFindSecondTarget = true;
            }
            else
            {
                isFindSecondTarget = FindTargetAndLocation(secondTouch, out target2, out position2, out groundPosition2, out direction2);
            }

            Transform targetCenter = null;
            Vector3 positionCenter = Vector3.zero;
            Vector3 directionCenter = Vector3.zero;
            Vector3 groundPositionCenter = Vector3.zero;
            TwoFingerTouchUpdateEvent evt = new TwoFingerTouchUpdateEvent(firstTouch.position, secondTouch.position);
            bool isFindCenterTarget = false;
			if (IsShowJoystick)
			{
                isFindCenterTarget = true;
            }
            else
            {
                isFindCenterTarget = FindTargetAndLocation(new TouchWrapper(0, TouchPhase.Moved, evt.screenPositionCenter), out targetCenter, out positionCenter, out groundPositionCenter, out directionCenter);
            }

            if (firstTouch.phase == TouchPhase.Began || secondTouch.phase == TouchPhase.Began)
			{
				StartDoubleTouch(firstTouch, secondTouch, target1,position1,direction1,groundPosition1,isFindFirstTarget,target2,position2,direction2,groundPosition2,isFindSecondTarget);
			}
			
			if (firstTouch.phase != TouchPhase.Ended && firstTouch.phase != TouchPhase.Canceled && secondTouch.phase != TouchPhase.Ended && secondTouch.phase != TouchPhase.Canceled)
			{
				UpdateDoubleTouch(firstTouch, secondTouch, target1, position1, direction1, groundPosition1, isFindFirstTarget, target2, position2, direction2, groundPosition2, isFindSecondTarget,
                    evt, isFindCenterTarget, 
                    targetCenter,
                    positionCenter,
                    directionCenter,
                    groundPositionCenter);
			}
			
			if (firstTouch.phase == TouchPhase.Ended || secondTouch.phase == TouchPhase.Ended || firstTouch.phase == TouchPhase.Canceled || secondTouch.phase == TouchPhase.Canceled)
			{
				EndDoubleTouch(firstTouch, secondTouch, target1, position1, direction1, groundPosition1, isFindFirstTarget, target2, position2, direction2, groundPosition2, isFindSecondTarget,
                    evt, isFindCenterTarget,
                    targetCenter,
                    positionCenter,
                    directionCenter,
                    groundPositionCenter);
				_touchStartOverUI = false;
			}
		}
	}
	
	public int SortCameras(Camera cam1, Camera cam2)
	{
		return cam1 == Camera.main ? 1 : -1;
	}
	
	private void OnEnable()
	{
		Cinematic.Switches currentCinematicSwitches = Cinematic.GetCurrentCinematicSwitches();
		if (null != currentCinematicSwitches) // a cinematic is running
		{
			OnCinematicEvent(currentCinematicSwitches, CinematicEvent.CinematicEventType.starting);
		}
		
		EventManager.instance.AddListener<CinematicEvent>(OnCinematicEvent);
	}
	
	private void OnDisable()
	{
		EventManager.instance.RemoveListener<CinematicEvent>(OnCinematicEvent);
	}

	private void OnCinematicEvent(CinematicEvent evt)
	{
		OnCinematicEvent(evt.GetSwitches(), evt.GetCinematicEventType());
	}
	
	private void OnCinematicEvent(Cinematic.Switches switches, CinematicEvent.CinematicEventType eventType)
	{
		if (!switches.IsPlayerNavigationAllowedDuringCinematic()) // if player navigation is not allowed during the cinematic, we need to take action
		{
			if (CinematicEvent.CinematicEventType.starting == eventType) // stop player navigation on cinematic start
			{
				DisablePlayerSelectionControls();
			}
			else if (CinematicEvent.CinematicEventType.ending == eventType) // allow player navigation again on cinematic end
			{
				EnablePlayerSelectionControls();
			}
		}
	}
	
	private void StartSingleTouch(Vector3 groundPosition, bool isFindTarget)
	{
		_singleTouchStartTime = Time.time;
		_longPressStarted = false;
		_playerLongPressStarted = false;
		_touchStartedOnPlayer = false;
		
		float selfSelectRadiusSq = GlobalBalanceData.Instance.selfSelectRadius;
		selfSelectRadiusSq *= selfSelectRadiusSq;
		
		if (isFindTarget)
		{
			if (GameUtils.GetDistSqXZ(groundPosition, _playerTransform.position) < selfSelectRadiusSq)
			{
				_touchStartedOnPlayer = true;
			}
		}
	}
	
	private void UpdateSingleTouch(TouchWrapper touch,
        Transform target,
        Vector3 location,
        Vector3 direction,
        Vector3 groundPosition,
        bool hasValidNavPoint)
	{


        float touchDuration = Time.time - _singleTouchStartTime;

		float selfSelectRadiusSq = GlobalBalanceData.Instance.selfSelectRadius;
		selfSelectRadiusSq *= selfSelectRadiusSq;
		
		if (touchDuration < TapLength)
		{
			// Do nothing yet
		}
		else if (!_longPressStarted)
		{
			// This press is longer than a tap
			_longPressStarted = true;
			
			TouchStartEvent startEvent = (TouchStartEvent)CodeObjectManager.GetEventManager(typeof(TouchStartEvent)).GetNext();
			if (target)
			{
				startEvent.Initialize(touch.position, target, location);
			}
			else
			{
				startEvent.Initialize(touch.position, location);
			}
			startEvent.direction = direction;
			startEvent.groundPosition = groundPosition;
			startEvent.deltaPosition = touch.deltaPosition;
			RaiseEvent(startEvent);
			CodeObjectManager.GetEventManager(typeof(TouchStartEvent)).Destroy(startEvent);
			
		}
		else if (_touchStartedOnPlayer)
		{
            if (!_playerLongPressStarted)
            {
                if (GameUtils.GetDistSqXZ(groundPosition, _playerTransform.position) > selfSelectRadiusSq)
                {
                    DragFromPlayerGestureStartEvent evt = null;
                    if (target)
                    {
                        evt = new DragFromPlayerGestureStartEvent(touch.position, target);
                    }
                    else
                    {
                        evt = new DragFromPlayerGestureStartEvent(touch.position, location);
                    }
                    evt.direction = direction;
                    evt.groundPosition = groundPosition;
                    RaiseEvent(evt);
                    _playerLongPressStarted = true;
                }
            }
            else
            {
                DragFromPlayerGestureUpdateEvent evt = null;
                if (target)
                {
                    evt = new DragFromPlayerGestureUpdateEvent(touch.position, target);
                }
                else
                {
                    evt = new DragFromPlayerGestureUpdateEvent(touch.position, location);
                }
                evt.direction = direction;
                evt.groundPosition = groundPosition;
                RaiseEvent(evt);
            }
		}
		else
        {
            TouchUpdateEvent updateEvent = (TouchUpdateEvent)CodeObjectManager.GetEventManager(typeof(TouchUpdateEvent)).GetNext(); ;
            if (target != null)
            {
                updateEvent.Initialize(touch.position, target, location);
            }
            else
            {
                updateEvent.Initialize(touch.position, location);
            }
            updateEvent.direction = direction;
            updateEvent.groundPosition = groundPosition;
            updateEvent.hasValidNavPoint = hasValidNavPoint;
            RaiseEvent(updateEvent);
            CodeObjectManager.GetEventManager(typeof(TouchUpdateEvent)).Destroy(updateEvent);
		}
		
		//add the touch to the touch history for gesture checking on end
		_touchHistory[m_touch_history_index]._time = Time.realtimeSinceStartup;
		_touchHistory[m_touch_history_index]._position = touch.position;
		m_touch_history_index = (m_touch_history_index + 1) % TOUCH_HISTORY_LENGTH;
	}
	
	private void EndSingleTouch(TouchWrapper touch,
        Transform target,
        Vector3 location,
        Vector3 direction,
        Vector3 groundPosition,
        bool hasValidNavPoint)
    {
		
		float singleTouchDuration = Time.time - _singleTouchStartTime;
		
		if (singleTouchDuration < TapLength)
		{
			if (Time.time - _lastTapTime > DoubleTapLength)
			{
				TapEvent tapEvent = (null != target) ? new TapEvent(touch.position, target, location) : new TapEvent(touch.position, location);
				tapEvent.direction = direction;
				tapEvent.groundPosition = groundPosition;
				tapEvent.hasValidNavPoint = hasValidNavPoint;
#if BUFFER_SINGLE_TAPS
				StartCoroutine(QueueSingleTap(tapEvent));
#else
				_lastTapTime = Time.time;
				_didDoubleTap = false;
				RaiseEvent(tapEvent);
#endif
			}
			else
			{
				DoubleTapEvent doubleTapEvent = (null != target) ? new DoubleTapEvent(touch.position, target, location) : new DoubleTapEvent(touch.position, location);
				doubleTapEvent.direction = direction;
				DoDoubleTap(doubleTapEvent);
			}
		}
		else if (_touchStartedOnPlayer)
        {
            DragFromPlayerGestureEndEvent evt = null;
            if (target)
            {
                evt = new DragFromPlayerGestureEndEvent(touch.position, target);
            }
            else
            {
                evt = new DragFromPlayerGestureEndEvent(touch.position, location);
            }
            evt.direction = direction;
            evt.groundPosition = groundPosition;
            RaiseEvent(evt);
            _touchStartedOnPlayer = false;
		}
		
		TouchEndEvent endEvent = (TouchEndEvent)CodeObjectManager.GetEventManager(typeof(TouchEndEvent)).GetNext();;
		if (target != null)
		{
			endEvent.Initialize(touch.position, target, location);
		}
		else
		{
			endEvent.Initialize(touch.position, location);
		}
		endEvent.direction = direction;
		endEvent.groundPosition = groundPosition;
		RaiseEvent(endEvent);
		CodeObjectManager.GetEventManager(typeof(TouchEndEvent)).Destroy(endEvent);
		
		//check for flick gesture
		float flick_history_check = 1.5f;
		float time_look_back = 0.0f;
		int i = 0;
		int touch_index = m_touch_history_index-1 > 0 ? m_touch_history_index-1: TOUCH_HISTORY_LENGTH-1;

		Vector3 flick_vel = Vector3.zero;
		float last_time = _touchHistory[touch_index]._time;
		Vector3 last_flick = touch.position;
		while (time_look_back < flick_history_check && i < TOUCH_HISTORY_LENGTH-1) 
		{
			if(_touchHistory[touch_index]._time == 0.0f)
			{
				break;
			}

			float time = last_time - _touchHistory[touch_index]._time;
			last_time = _touchHistory[touch_index]._time;
			
			time_look_back += time;
			flick_vel += _touchHistory[touch_index]._position - last_flick;
			last_flick = _touchHistory[touch_index]._position;

			i++;
			touch_index = touch_index - 1 > 0 ? touch_index-1 : TOUCH_HISTORY_LENGTH-1;
		}

		//EB.Debug.Log (i);
		if (time_look_back > 0.0f) {
			flick_vel *= time_look_back;
			if (flick_vel.magnitude > 0.2f) {
				FlickEvent flickEvent = new FlickEvent();
				flickEvent.Initialize(touch.position, flick_vel);
				RaiseEvent(flickEvent);
			}
			else {
				//EB.Debug.Log ("VEL FAIL");
			}
		}
		else 
		{
			//EB.Debug.Log ("TIME FAIL");
		}

		
		ResetTouchHistory();
	}
	
	private void StartDoubleTouch(TouchWrapper firstTouch, 
        TouchWrapper secondTouch,
        Transform target1,
        Vector3 position1,
        Vector3 direction1,
        Vector3 groundPosition1,
        bool isFindFirstTarget,
        Transform target2,
        Vector3 position2,
        Vector3 direction2,
        Vector3 groundPosition2,
        bool isFindSecondTarget)
	{
		TwoFingerTouchStartEvent evt = new TwoFingerTouchStartEvent(firstTouch.position, secondTouch.position);
		if (isFindFirstTarget)
		{
			if (target1 != null)
			{
				evt.target1 = target1;
				evt.position1 = target1.position;
			}
			else
			{
				evt.position1 = position1;
			}
		}
		if (isFindSecondTarget)
		{
			if (target2 != null)
			{
				evt.target2 = target2;
				evt.position2 = target2.position;
			}
			else
			{
				evt.position2 = position2;
			}
		}
		evt.direction1 = direction1;
		evt.direction2 = direction2;
		evt.groundPosition1 = groundPosition1;
		evt.groundPosition2 = groundPosition2;
		RaiseEvent(evt);
	}
	
	private void UpdateDoubleTouch(TouchWrapper firstTouch, TouchWrapper secondTouch,
        Transform target1,
        Vector3 position1,
        Vector3 direction1,
        Vector3 groundPosition1,
        bool isFindFirstTarget,
        Transform target2,
        Vector3 position2,
        Vector3 direction2,
        Vector3 groundPosition2,
        bool isFindSecondTarget,
        TwoFingerTouchUpdateEvent evt,
        bool isFindCenterTarget,
        Transform targetCenter,
        Vector3 positionCenter,
        Vector3 directionCenter,
        Vector3 groundPositionCenter)
	{
		
		if (isFindFirstTarget)
		{
			if (target1 != null)
			{
				evt.target1 = target1;
				evt.position1 = target1.position;
			}
			else
			{
				evt.position1 = position1;
			}
		}
		evt.direction1 = direction1;
		evt.groundPosition1 = groundPosition1;
		
		if (isFindSecondTarget)
		{
			if (target2 != null)
			{
				evt.target2 = target2;
				evt.position2 = target2.position;
			}
			else
			{
				evt.position2 = position2;
			}
		}
		
		
		evt.direction2 = direction2;
		evt.groundPosition2 = groundPosition2;
		
		if (isFindCenterTarget)
		{
			if (targetCenter != null)
			{
				evt.targetCenter = targetCenter;
				evt.positionCenter = targetCenter.position;
			}
			else
			{
				evt.positionCenter = positionCenter;
			}
		}
		
		evt.directionCenter = directionCenter;
		evt.groundPositionCenter = groundPositionCenter;
		
		RaiseEvent(evt);
	}
	
	private void EndDoubleTouch(TouchWrapper firstTouch, TouchWrapper secondTouch,
        Transform target1,
        Vector3 position1,
        Vector3 direction1,
        Vector3 groundPosition1,
        bool isFindFirstTarget,
        Transform target2,
        Vector3 position2,
        Vector3 direction2,
        Vector3 groundPosition2,
        bool isFindSecondTarget,
        TwoFingerTouchUpdateEvent evt,
        bool isFindCenterTarget,
        Transform targetCenter,
        Vector3 positionCenter,
        Vector3 directionCenter,
        Vector3 groundPositionCenter)
	{
		if (isFindFirstTarget)
		{
			if (target1 != null)
			{
				evt.target1 = target1;
				evt.position1 = target1.position;
			}
			else
			{
				evt.position1 = position1;
			}
		}
		
		evt.direction1 = direction1;
		evt.groundPosition1 = groundPosition1;
		
		if (isFindSecondTarget)
		{
			if (target2 != null)
			{
				evt.target2 = target2;
				evt.position2 = target2.position;
			}
			else
			{
				evt.position2 = position2;
			}
		}
		
		evt.direction2 = direction2;
		evt.groundPosition2 = groundPosition2;
		
		if (isFindCenterTarget)
		{
			if (targetCenter != null)
			{
				evt.targetCenter = targetCenter;
				evt.positionCenter = targetCenter.position;
			}
			else
			{
				evt.positionCenter = positionCenter;
			}
		}
		
		evt.direction2 = direction2;
		evt.groundPosition2 = groundPosition2;
		
		RaiseEvent(evt);
	}

    private Camera mMainCamera = null;
	private bool FindTargetAndLocation(TouchWrapper touch, out Transform target, out Vector3 location, out Vector3 groundPosition, out Vector3 direction)
	{
		target = null;
		
		if (Camera.main == null)
		{
			location = default(Vector3);
			groundPosition = default(Vector3);
			direction = default(Vector3);
			return false;
		}
        if (mMainCamera == null)
        {
            mMainCamera = Camera.main;
        }

        Ray ray = mMainCamera.ScreenPointToRay(new Vector3(Mathf.Clamp(touch.position.x, 0, Screen.width), Mathf.Clamp(touch.position.y, 0, Screen.height), 0.0f));
		RaycastHit rayHit;
		
		groundPosition = default(Vector3);
		direction = default(Vector3);
		location = default(Vector3);
		
		if (Physics.Raycast(ray, out rayHit, 200, _groundLayer))
		{	
			groundPosition = rayHit.point;
		}
		
		//Check raycast first for best precision
		if (Physics.Raycast(ray, out rayHit, 200, _characterLayers | _groundLayer))
		{
			if (PlayerManager.IsPlayer(rayHit.transform.gameObject))
			{
				target = rayHit.transform;
			}
			else if (rayHit.transform.gameObject.layer != LayerMask.NameToLayer("Ground"))
			{
				Selectable selectable = rayHit.transform.GetComponent<Selectable>();
				if (selectable == null)
				{
					//If raycast couldn't find a selectable widen to an overlap sphere cast and pick closest selectable
					Collider[] colliders = Physics.OverlapSphere(rayHit.point, fingerRadius, 1 << LayerMask.NameToLayer("Enemy"));
					if (colliders.Length > 0)
					{
						float minSqDistance = float.MaxValue;
						Collider closestCollider = colliders[0];
						foreach (Collider collider in colliders)
						{
							float sqrDistance = Vector3.SqrMagnitude(collider.transform.position - rayHit.point);
							if (sqrDistance < minSqDistance)
							{
								minSqDistance = sqrDistance;
								closestCollider = collider;
							}
						}
						selectable = closestCollider.gameObject.GetComponent<Selectable>();
					}
				}
				
				//Move or target selectable
				if (selectable != null)
				{
					target = selectable.transform;
				}
				else
				{
					target = null;
				}
			}

			//if the object has a ServerCallback then call it!!
			if (rayHit.transform.gameObject != null)
			{
				MeshClick callback = rayHit.transform.gameObject.GetComponent<MeshClick>();
				if(callback != null)
				{
					target = callback.transform;
				}
			}


			if (target != null) 
			{
				location = target.transform.position;
			}
			else 
			{
				location = groundPosition;
			}
			
			if (_playerTransform != null && _muzzleTransform != null)
			{
				Vector3 muzzleYOffset = new Vector3(0, _muzzleTransform.position.y - _playerTransform.position.y, 0);
				location += muzzleYOffset;
				direction = (groundPosition - _playerTransform.position).normalized;
			}

			// todo: FindTargetAndLocation called many times
			//Debug.LogFormat("FindTargetAndLocation: location = {0}", location.ToString());
			if(!AStarPathfindingUtils.IsPointOnNavMeshOptimized(location))
			{
				return false;
			}
			
			return true;
		}
		else if (_playerTransform != null)
		{
			Plane playerPlane = new Plane(Vector3.up, _playerTransform.position);
			float enter = 0;
			if (playerPlane.Raycast(ray, out enter))
			{
				Vector3 hitPoint = ray.GetPoint(enter);
				direction = (hitPoint - _playerTransform.position).normalized;
				groundPosition = hitPoint;
			}
		}
		
		target = null;
		location = default(Vector3);
		return false;
	}

#region GaM Fusion_MeshColliderTutorialHighlight
	private void RaiseEvent(GameEvent e) {
		EventManager.instance.Raise(e);
	}
#endregion
	
	private IEnumerator QueueSingleTap(TapEvent tapEvent)
	{
		_lastTapTime = Time.time;
		_didDoubleTap = false;
		
		do 
		{
			yield return null;
		}
		while (Time.time - _lastTapTime < DoubleTapLength);
		
		if (!_didDoubleTap)
		{
			RaiseEvent(tapEvent);
		}
	}
	
	private void DoDoubleTap(DoubleTapEvent doubleTapEvent)
	{
		StopCoroutine("QueueSingleTap");
		_didDoubleTap = true;
		RaiseEvent(doubleTapEvent);
	}
}
