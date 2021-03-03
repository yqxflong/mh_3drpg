using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Tool;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class CameraBase : MonoSingleton<CameraBase>
#if DEBUG
	, IDebuggable
		#endif
{
	public const float TransitionImmediate = -1f;
	
	public enum eDebugCameraType
	{
		SceneCamera,
		FlyCamera,
		None, // no debug camera active
	}
	
	#if UNITY_EDITOR
	private Editor _theEditor;
	public Editor theEditor
	{
		get { return _theEditor; }
		set { _theEditor = value; }
	}
	#endif

	public eDebugCameraType debugCamera = eDebugCameraType.None;
	public bool drawDebug = false;

	// no longer used (moved to GlobalCameraData), keeping the data around in case it needs to be referred to
	public float zoomSpeed = 0.5f;
	//-------------------------------------------------------------------------------------------------------
	
	private float _pinchStartDistance;
	private Vector3 _initialPinch;
	
	// These values are no longer used (moved to GlobalCameraData), keeping the data around in case it needs to be referred to -------------------------------
	// [Range(-89f, 89f)]
	//public float closePitch = 40.4f;
	// [Range(0f, 200f)]
	//public float closeVertical = 6f;
	// [Range(1f, 200f)]
	//public float closeDist = 6f;
	// [Range(-89f, 89f)]
	//public float farPitch = 50.0f;
	// [Range(0f, 200f)]
	//public float farVertical = 17.6f;
	// [Range(1f, 200f)]
	//public float farDist = 21.3f;
	// [Range(0f, 200f)]
	//public float farVerticalOverworld = 28.0f; // 54.6f;
	// [Range(1f, 200f)]
	//public float farDistOverworld = 24.5f; // 55.4f;	
	// [Range(0f, 1f)]
	//public float Curve = 0f;
	// [Range(0f, 359f)]
	//public float camAngleDegrees = 140f;
	// [Range(0f, 10000f)]
	//public float smoothingSpeed = 1.5f; // this value smoothes small changes in position like normal locomotion, going up/down stairs etc
	//--------------------------------------------------------------------------------------------------------------------------------------------------------
	
	protected bool _shaking;
	protected float _lastShakeSqrMagnitude;
	protected string _activeShakeName;
	
	protected float zoomDistance = 1.0f;

	protected Camera _mainCamera = null;
	private List<DynamicVisibilityComponent> _fadingComponents = new List<DynamicVisibilityComponent>();
	public bool useTransparency = true;
	#if DEBUG
	private eDebugCameraType _previousFrameDebugCamera = eDebugCameraType.None;
	#endif
	
	protected GameCameraBehavior _gameCamera = new GameCameraBehavior();
	protected Vector3 _gameCameraPosition = new Vector3();
	protected Quaternion _gameCameraRotation = new Quaternion();
	
	protected Vector3 _stillCameraPosition = new Vector3();
	protected Quaternion _stillCameraRotation = new Quaternion();
	
	#if DEBUG
	private Vector3 _debugCameraPosition = new Vector3();
	private Quaternion _debugCameraRotation = new Quaternion();
	#endif
	
	public class BlendedCameraBehavior
	{
		public struct BlendedCameraOutput
		{
			public Vector3 lookAt;
			public Quaternion rotation;
			public float distFromLookAt;
			
			static public void Lerp(ref BlendedCameraOutput result, ref BlendedCameraOutput from, ref BlendedCameraOutput to, 
									CameraLerp.LerpStyle style, float interpolatePos, float interpolatePitch, float interpolateYaw)
			{
				if (interpolatePos >= 1f && interpolatePitch >= 1.0f && interpolateYaw >= 1.0f)
				{
					result = to;
					return;
				}
				
				switch (style)
				{
				case CameraLerp.LerpStyle.positionToPosition:
					
					Vector3 fromPosition = from.CalculatePosition();
					Vector3 toPosition = to.CalculatePosition();
					Vector3 resultPosition = fromPosition + ((toPosition-fromPosition)*interpolatePos);					
					result.distFromLookAt = from.distFromLookAt + ((to.distFromLookAt - from.distFromLookAt) * interpolatePos);
					Vector3 toEuler = to.rotation.eulerAngles;
					Vector3 fromEuler = from.rotation.eulerAngles;
					Vector3 lerpEuler = Vector3.zero;
					lerpEuler.x = Mathf.LerpAngle(fromEuler.x, toEuler.x, interpolatePitch);
					lerpEuler.y = Mathf.LerpAngle(fromEuler.y, toEuler.y, interpolateYaw);
					//result.rotation = Quaternion.Slerp(from.rotation, to.rotation, interpolate);
					result.rotation = Quaternion.Euler(lerpEuler);
					result.lookAt = resultPosition + GameUtils.CalculateForward(result.rotation) * result.distFromLookAt; 					
					break;
				case CameraLerp.LerpStyle.keepTargetOnScreen:					
					result.distFromLookAt = from.distFromLookAt + ((to.distFromLookAt - from.distFromLookAt) * interpolatePos);
					//result.rotation = Quaternion.Slerp(from.rotation, to.rotation, interpolatePos);
					Vector3 toEulerK = to.rotation.eulerAngles;
					Vector3 fromEulerK = from.rotation.eulerAngles;
					Vector3 lerpEulerK = Vector3.zero;
					lerpEulerK.x = Mathf.LerpAngle(fromEulerK.x, toEulerK.x, interpolatePitch);
					lerpEulerK.y = Mathf.LerpAngle(fromEulerK.y, toEulerK.y, interpolateYaw);
					//result.rotation = Quaternion.Slerp(from.rotation, to.rotation, interpolate);
					result.rotation = Quaternion.Euler(lerpEulerK);
					result.lookAt = from.lookAt + ((to.lookAt - from.lookAt) * interpolatePos);					
					break;
				default:
					result = to;
					break;
				}
			}
			
			public BlendedCameraOutput(Vector3 lookAt, Quaternion rotation, float distFromLookAt)
			{
				this.lookAt = lookAt;
				this.rotation = rotation;
				this.distFromLookAt = distFromLookAt;
			}
			
			public Vector3 CalculatePosition()
			{
				return lookAt - GameUtils.CalculateForward(rotation) * distFromLookAt;
			}
		}
		
		public float transitionStartTime;
		public CameraLerp lerp;
		public GameCameraBehavior behavior;
		public CameraLerp.LerpStyle lerpStyle;
		
		static public bool IsTransitionImmediate(float transitionLength)
		{
			const float ZeroTol = 0.00001f;
			return (transitionLength <= ZeroTol); // is it a cut
		}
		
		static public bool IsTransitionImmediate(CameraLerp lerp)
		{
			return null == lerp || IsTransitionImmediate(lerp.dialogueCameraLerpTime);
		}
		
		public BlendedCameraBehavior(CameraLerp lerp, GameCameraBehavior behavior)
		{
            if(Application.isPlaying)
            {
                transitionStartTime = Time.time;
            }
            else
            {
                transitionStartTime = Time.realtimeSinceStartup;
            }
			this.lerp = lerp;
			
			if (null != lerp)
			{
				lerpStyle = lerp.lerpStyle;
			}
			else
			{
				lerpStyle = CameraLerp.LerpStyle.positionToPosition;
			}
			this.behavior = behavior;			
		}
		
		public bool HasLerpStyleBeenSet()
		{
			return CameraLerp.LerpStyle.determineAtRunTime != lerpStyle;
		}
		
		public float CalculateLerpValue()
		{
			if (IsTransitionImmediate(lerp)) // is it a cut
			{
				return 1f; // fully blended in
			}
			return Application.isPlaying?(Time.time - transitionStartTime) / lerp.dialogueCameraLerpTime : (Time.realtimeSinceStartup - transitionStartTime)/lerp.dialogueCameraLerpTime;
		}

		public bool IsFullyBlendedIn()
		{			
			if(lerp == null)
			{
				return true;
			}
			float total_time = lerp.dialogueCameraLerpTime + lerp.hangonTime;
			if(IsTransitionImmediate(total_time))
			{
				return true;
			}
			return Application.isPlaying ? (Time.time - transitionStartTime) >= total_time : (Time.realtimeSinceStartup - transitionStartTime) >= total_time;
		}
	};
	protected List<BlendedCameraBehavior> _runningBehaviors = new List<BlendedCameraBehavior>();
	
	private class RayHitDistComparer : IComparer<RaycastHit>
	{
		public int Compare(RaycastHit x, RaycastHit y)
		{
			return x.distance.CompareTo(y.distance);
		}
	}
	private RayHitDistComparer _rayHitDistComparer;
	private Hashtable _viewportTweenOptions;
	
	public bool _isCinematicActive = false;
    public bool _isStillCamActive = false;
	
	#if DEBUG
	#if UNITY_EDITOR
	//[MenuItem("Fusion/Camera/Game Camera", false, priority: 9000)]
	#endif
	public static void GameCamera()
	{
		SetDebugCamera(eDebugCameraType.None);
	}
	
	#if UNITY_EDITOR
	//[MenuItem("Fusion/Camera/Scene Camera", false, priority: 9000)]
	#endif
	public static void SceneCamera()
	{
		SetDebugCamera(eDebugCameraType.SceneCamera);
	}
	
	#if UNITY_EDITOR
	//[MenuItem("Fusion/Camera/Fly Camera", false, priority: 9000)] 
	#endif
	public static void FlyCamera()
	{
		SetDebugCamera(eDebugCameraType.FlyCamera);
	}
	#endif
	
	// snap the object to looking at the camera
	public bool CalculateObjectToCameraXZDir(Transform gameObjectTransform, ref Vector3 objectToCamera)
	{
		if (null != gameObjectTransform)
		{
			Transform cameraTransform = transform;
			
			objectToCamera = cameraTransform.position - gameObjectTransform.position;
			return GameUtils.NormalizeXZ(ref objectToCamera);
		}
		return false;
	}
	
	public void SetZoomDistance(float dist)
	{
		zoomDistance = dist;
	}
	
	public float GetZoomDistance()
	{
		return zoomDistance;
	}
	
	void Awake()
	{
		// todo: use a single Camera prefab. Currently some levels overwrite this on their local Main Camera object
		//smoothingSpeed = 8.0f;

		#if DEBUG
		//QuestDebugComponent.AddQuestDebugComponent(); // done here as CameraBase exists everwhere we need the QuestDebugComponent
		#endif
	}
	
	void Start()
	{
		if(!Application.isPlaying){
			return;
		}
		
		_rayHitDistComparer = new RayHitDistComparer();

		OnComponentStart();

		SetFieldOfView(); // called at this point to ensure levels created with the old field of view are updated
		SetZoonSpeed();

        _mainCamera = MainCamera;

#if DEBUG
        debugCamera = _previousFrameDebugCamera = eDebugCameraType.None;
#endif
	}

    protected Camera MainCamera
    {
        get
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                _mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
            }
            return _mainCamera;
        }
    }

	protected virtual void OnComponentStart()
	{
		
	}
	
	void OnEnable()
	{
		#if DEBUG
		//DebugSystem.RegisterSystem("Camera", this);
		debugCamera = _previousFrameDebugCamera = eDebugCameraType.None;
		#endif

		OnComponentEnable();

		#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			//EditorApplication.update += EditorUpdate;
		}
		#endif
	}

	protected virtual void OnComponentEnable()
	{
		_gameCamera.SetPosition(transform.position);
		_gameCamera.SetRotation(transform.rotation);

        if (transform.position == null || (Mathf.Abs(transform.position.x) <= float.Epsilon && Mathf.Abs(transform.position.y) <= float.Epsilon))
        {
            EB.Debug.Log("-----主城黑屏----WARN!: transform.position == null OR X & Y = 0 {0}",transform.position);
        }

		_gameCameraPosition = transform.position;
		_gameCameraRotation = transform.rotation;
		
		EventManager.instance.AddListener<CinematicEvent>(OnCinematicEvent);
		EventManager.instance.AddListener<TwoFingerTouchStartEvent>(OnTwoFingerTouchStartEvent);
		EventManager.instance.AddListener<TwoFingerTouchUpdateEvent>(OnTwoFingerTouchUpdateEvent);
		
		_updateVisibilityIncludeLayers = (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Player"));
		_updateVisibilityPlayerLayer = LayerMask.NameToLayer("Player");

	}
	
	void OnDisable()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			//EditorApplication.update -= EditorUpdate;
		}
#endif
		
#if DEBUG
		//DebugSystem.UnregisterSystem(this);
#endif

		OnComponentDisable();
	}

	protected virtual void OnComponentDisable()
	{
		EventManager.instance.RemoveListener<CinematicEvent>(OnCinematicEvent);
		EventManager.instance.RemoveListener<TwoFingerTouchStartEvent>(OnTwoFingerTouchStartEvent);
		EventManager.instance.RemoveListener<TwoFingerTouchUpdateEvent>(OnTwoFingerTouchUpdateEvent);
		//Clear current running behaviors
		_runningBehaviors.Clear();
	}
	
	public virtual void SetFieldOfView()
	{
		Camera cameraComp = GetComponent<Camera>();
		float fov = 45.0f;
		if(GlobalCameraData.Instance != null)
		{
			fov = GlobalCameraData.Instance.gameCameraFieldsOfView;
		}
		if(cameraComp)cameraComp.fieldOfView = fov;
	}

	public void SetZoonSpeed()
	{
		if (GlobalCameraData.Instance != null)
		{
			zoomSpeed = GlobalCameraData.Instance.gameCameraZoomSpeed;
		}
	}
	
	void TiltCamera(float tiltDeg)
	{
		// rotate the camera forwards around it's local x axis
		transform.rotation *= Quaternion.AngleAxis(tiltDeg, Vector3.right);
	}

	public virtual BlendedCameraBehavior EnterInteractionCamera(ref List<GameObject> targets, ref GameCameraParams gameCamParams, CameraLerp lerp)
	{
		GameCameraBehavior newGameCamera = new GameCameraBehavior(ref targets, ref gameCamParams, this);
		return AddNewBehavior(newGameCamera, lerp);
	}
	
	public void EnterFixedOffsetGameCamera(CameraLerp lerp)
	{
		if (BlendedCameraBehavior.IsTransitionImmediate(lerp))
		{
			_runningBehaviors.Clear();
		}
		
		AddNewBehavior(_gameCamera, lerp);
	}
	
	public void EnterStillCamera()
	{
		_stillCameraPosition = transform.position;
		_stillCameraRotation = transform.rotation;
		_isStillCamActive = true;
	}
	
	// final game camera transform, includes blending game cameras
	public void GetGameCameraTransform(ref Vector3 position, ref Quaternion rotation)
	{
		position = _gameCameraPosition;
		GetGameCameraOrientation(ref rotation);
	}
	
	// final game camera orientation, includes blending game cameras
	public void GetGameCameraOrientation(ref Quaternion rotation)
	{
		rotation = _gameCameraRotation;
	}
	
	// the transform of the latest game camera behavior
	public void GetNewestGameCameraBehaviorTransform(ref Vector3 position, ref Quaternion rotation)
	{
		if (GetNewestRunningBehaviorIndex() >= 0)
		{
			_runningBehaviors[GetNewestRunningBehaviorIndex()].behavior.GetPosition(ref position);
			_runningBehaviors[GetNewestRunningBehaviorIndex()].behavior.GetRotation(ref rotation);
		}
	}
	
	// is the camera currently blending between multiple camera behaviors
	public bool IsCameraTransitioning()
	{
		return null != _runningBehaviors && _runningBehaviors.Count > 1; // if there is more than one behavior, the camera is transitioning
	}
	
	public void Update()
    {
		if (Application.isPlaying)
        {
            if (GameEngine.Instance && !GameEngine.Instance.IsTimeToRootScene)
            {
                return;
            }

			//如果是副本中则不能进入rvo
			//ToDo:暂时屏蔽，方便解耦
			//if (GameEngine.Instance && LTInstanceMapModel.Instance != null && LTInstanceMapModel.Instance.IsInstanceMap())
            //{
            //    return;
            //}
			if (GameEngine.Instance && (bool)GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTInstanceMapModel", "IsInstanceMapFromILR"))
			{
				return;
			}

			InternalUpdate(Time.deltaTime);
        }
    }
	
	private void InternalUpdate(float deltaTime)
	{
		UpdateGameCamera(deltaTime);
		
		if (_isCinematicActive)
		{
			Vector3 tempPosition = Vector3.zero;
			Quaternion tempQuat = Quaternion.identity;
			Cinematic.GetCurrentCinematicTransform(ref tempPosition, ref tempQuat);
			
			transform.position = tempPosition;
			transform.rotation = tempQuat;
		}
		
		if (_isStillCamActive)
		{
			transform.position = _stillCameraPosition;
			transform.rotation = _stillCameraRotation;
		}
		
#if DEBUG
		// if we are turning the debug camera on
		if (_previousFrameDebugCamera != debugCamera && eDebugCameraType.None == _previousFrameDebugCamera)
		{
			_debugCameraPosition = transform.position;
			_debugCameraRotation = transform.rotation;
		}
		
		switch (debugCamera)
		{
		case eDebugCameraType.SceneCamera:					
			if (null != Camera.current && null != Camera.current.transform)
			{
				_debugCameraPosition = Camera.current.transform.position;
				_debugCameraRotation = Camera.current.transform.rotation;
			}
			break;
			
		case eDebugCameraType.FlyCamera:
			UpdateDebugFlyCamera();
			break;
			
		default: break;
		}
		
		if (eDebugCameraType.None != debugCamera)
		{
			transform.position = _debugCameraPosition;
			transform.rotation = _debugCameraRotation;	
		}
		
		_previousFrameDebugCamera = debugCamera;					
#endif
	}

	protected virtual void UpdateGameCamera(float deltaTime)
	{

	}
	
#if UNITY_EDITOR
	public void EditorUpdate(float deltaTime)
	{
		if (!Application.isPlaying)
		{
			InternalUpdate(deltaTime);
		}		
	}
	
	// get the type of the current camera
	public string GetCurrentCameraTypeText()
	{
		Cinematic activeCinematic = Cinematic.GetActiveCinematic();
		if (null != activeCinematic)
		{
			return "Cinematic";
		}
		return "GameCameraBehavior";
	}
	
	// get a description of the current camera
	public string GetCurrentCameraText()
	{
		Cinematic activeCinematic = Cinematic.GetActiveCinematic();
		if (null != activeCinematic)
		{
			return activeCinematic.name;
		}
		
		if (Application.isPlaying)
		{
			if (GetNewestRunningBehaviorIndex() >= 0)
			{
				return _runningBehaviors[GetNewestRunningBehaviorIndex()].behavior.GetCameraText();
			}
			return "NO CAMERA";
		}
		else // editor
		{
			return "game cam default";
		}
	}
#endif
	
	private float _lastObstacleUpdate;
	private int _updateVisibilityIncludeLayers = 0;
	private int _updateVisibilityPlayerLayer = 0;
	private const int MaxColliderOverlap = 30;
	private RaycastHit[] rayHits = new RaycastHit[MaxColliderOverlap];
	private int rayHitsLength = 0;
	public void UpdateObstacleVisibility(Vector3 targetPos)
	{
#if !UNITY_EDITOR
		if (!useTransparency )
			return;
#endif
		
		// Limit this sphere cast to 4 times per second
		if (Time.time >= _lastObstacleUpdate + 0.25f)
		{
			Vector3 screenPos = MainCamera.WorldToScreenPoint(targetPos);
			Ray ray = MainCamera.ScreenPointToRay(screenPos);

			rayHitsLength = Physics.SphereCastNonAlloc(ray, 1.1f, rayHits, 200, _updateVisibilityIncludeLayers);
			// check overflow
			while (rayHitsLength == rayHits.Length)
			{
				rayHits = new RaycastHit[rayHitsLength * 2];
				rayHitsLength = Physics.SphereCastNonAlloc(ray, 1.1f, rayHits, 200, _updateVisibilityIncludeLayers);
			}
			_lastObstacleUpdate = Time.time;
			
			if (rayHitsLength > 0)
			{
				System.Array.Sort(rayHits, 0, rayHitsLength, _rayHitDistComparer);
			}
		}
		
		if (rayHitsLength > 0)
		{
			for (int i = 0; i < rayHitsLength; i++)
			{
				// bail when we reach the player
				Transform tr = rayHits[i].transform;
				if (null != tr && tr.gameObject.layer == _updateVisibilityPlayerLayer)
					break;
				
				DynamicVisibilityComponent dynamicVisibilityComponent = null != tr ? tr.GetComponentInChildren<DynamicVisibilityComponent>() : null;
				if (dynamicVisibilityComponent != null)
				{
					if (!_fadingComponents.Contains(dynamicVisibilityComponent))
					{
						_fadingComponents.Add(dynamicVisibilityComponent);
					}
					dynamicVisibilityComponent.FadeOut();
				}
			}
		}
		
		// fade-in all other DynamicVisibilityComponent, and remove the one complete fade-in object
		for (int i = _fadingComponents.Count - 1; i >= 0; --i)
		{
			DynamicVisibilityComponent it = _fadingComponents[i];
			if (it == null)
			{
				_fadingComponents.RemoveAt(i);
				continue;
			}
			if (it.FadeOutFrameCount != Time.frameCount)
			{
				if (it.FadeIn())
				{
					_fadingComponents.RemoveAt(i);
				}
			}
		}
	}
	
	public void StartShake(Vector3 intensity, float duration)
	{
		if (_shaking && intensity.sqrMagnitude > _lastShakeSqrMagnitude)
		{
			EndShake();
		}
		
		if (!_shaking)
		{
			_shaking = true;
			_lastShakeSqrMagnitude = intensity.sqrMagnitude;
			_activeShakeName = "cameraShake" + Time.time;
//			iTween.ShakePosition(gameObject, iTween.Hash("name", _activeShakeName, "isLocal", true, "amount", intensity, "time", duration));
			gameObject.transform.DOShakePosition(duration, intensity);
		}
	}
	
	public void EndShake()
	{
		if (_shaking)
		{
			_shaking = false;
//			iTween.StopByName(gameObject, _activeShakeName);
			gameObject.transform.DOKill();
		}
	}
	
	public virtual void SetInitialCameraPosition()
	{

	}
	
#if UNITY_EDITOR
	public void DrawDebugData()
	{	
		EditorGUILayout.LabelField("Current Camera Type: " + GetCurrentCameraTypeText());
		EditorGUILayout.LabelField("Current Camera: " + GetCurrentCameraText());
		EditorGUILayout.LabelField("Shake: " + (_shaking ? "ACTIVE" : "Inactive"));
	}
#endif
	
#if DEBUG
	public void OnDebugPanelGUI()
	{
		DrawGUI();
	}
	
	// draw the gui
	public void DrawGUI()
	{	
		if (Application.isPlaying)
		{
			Color previousColor = GUI.backgroundColor;
			
#if UNITY_EDITOR
			DrawDebugData();
#endif
			
			if (eDebugCameraType.FlyCamera == debugCamera)
			{
				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Turn Off Debug Fly Cam"))
				{
					debugCamera = eDebugCameraType.None;
				}
			}
			else // not fly camera
			{
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Turn On Debug Fly Cam"))
				{
					debugCamera = eDebugCameraType.FlyCamera;
				}
			}		
			GUI.backgroundColor = previousColor;
		}
	}
	
	private void UpdateDebugFlyCamera()
	{
		const float LinearSpeed = 5f;
		const float LinearSpeedFastMultiplier = 4f;
		
		const float OrientationSpeed = 20f;
		const float OrientationSpeedFastMultiplier = 4f;
		
		// first set our movement speeds
		float speedDelta = LinearSpeed * Time.deltaTime;
		float orientationSpeedDelta = OrientationSpeed * Time.deltaTime;
		if (Input.GetKey(KeyCode.LeftShift))
		{
			speedDelta *= LinearSpeedFastMultiplier;
			orientationSpeedDelta *= OrientationSpeedFastMultiplier;
		}
		
		// see if we want to orient right
		if (Input.GetKey(KeyCode.L))
		{
			GameUtils.Normalize(ref _debugCameraRotation);
			Quaternion multiplier = Quaternion.AngleAxis(orientationSpeedDelta, Vector3.up);
			GameUtils.Normalize(ref multiplier);
			_debugCameraRotation = multiplier * _debugCameraRotation;
			GameUtils.Normalize(ref _debugCameraRotation);
		}
		
		// see if we want to orient left
		if (Input.GetKey(KeyCode.J))
		{
			GameUtils.Normalize(ref _debugCameraRotation);
			Quaternion multiplier = Quaternion.AngleAxis(-orientationSpeedDelta, Vector3.up);
			GameUtils.Normalize(ref multiplier);
			_debugCameraRotation = multiplier * _debugCameraRotation;
			GameUtils.Normalize(ref _debugCameraRotation);
		}
		
		// see if we want to pitch down
		if (Input.GetKey(KeyCode.K))
		{
			GameUtils.Normalize(ref _debugCameraRotation);
			Quaternion multiplier = Quaternion.AngleAxis(orientationSpeedDelta, Vector3.right);
			GameUtils.Normalize(ref multiplier);
			_debugCameraRotation *= multiplier;
			GameUtils.Normalize(ref _debugCameraRotation);
		}
		
		// see if we want to pitch up
		if (Input.GetKey(KeyCode.I))
		{
			GameUtils.Normalize(ref _debugCameraRotation);
			Quaternion multiplier = Quaternion.AngleAxis(-orientationSpeedDelta, Vector3.right);
			GameUtils.Normalize(ref multiplier);
			_debugCameraRotation *= multiplier;
			GameUtils.Normalize(ref _debugCameraRotation);
		}
		
		// see if we want to move up
		if (Input.GetKey(KeyCode.E))
		{
			_debugCameraPosition.y += speedDelta;
		}
		
		// see if we want to move down
		if (Input.GetKey(KeyCode.Q))
		{
			_debugCameraPosition.y -= speedDelta;
		}
		
		Matrix4x4 orientationMat = Matrix4x4.identity;
		orientationMat.SetTRS(Vector3.zero, _debugCameraRotation, Vector3.one);
		
		Vector3 flyForward = orientationMat.inverse.GetRow(2);		
		// see if we want to move forward
		if (Input.GetKey(KeyCode.R))
		{
			_debugCameraPosition += flyForward * speedDelta;
		}
		
		// see if we want to move backward
		if (Input.GetKey(KeyCode.F))
		{
			_debugCameraPosition -= flyForward * speedDelta;
		}
		
		Vector3 panForwardOnGroundPlane = new Vector3(flyForward.x, 0f, flyForward.z);
		if (GameUtils.NormalizeXZ(ref panForwardOnGroundPlane)) // if the forward vector was not pointing straight up/down the y axis
		{
			// see if we want to pan forward on ground plane
			if (Input.GetKey(KeyCode.W))
			{
				_debugCameraPosition += panForwardOnGroundPlane * speedDelta;
			}
			
			// see if we want to pan backward on ground plane
			if (Input.GetKey(KeyCode.S))
			{
				_debugCameraPosition -= panForwardOnGroundPlane * speedDelta;
			}
		}
		
		Vector3 flyRight = orientationMat.inverse.GetRow(0);
		// see if we want to move right
		if (Input.GetKey(KeyCode.D))
		{
			_debugCameraPosition += flyRight * speedDelta;
		}
		
		// see if we want to move left
		if (Input.GetKey(KeyCode.A))
		{
			_debugCameraPosition -= flyRight * speedDelta;
		}
	}

	public void OnDrawDebug()
	{
		if (!drawDebug)
		{
			return;
		}
		for (int running = GetOldestRunningBehaviorIndex(); running <= GetNewestRunningBehaviorIndex(); ++running)
		{
			_runningBehaviors[running].behavior.OnDrawDebug();
		}
	}

	public void OnDebugGUI()
	{
	}

	private static void SetDebugCamera(eDebugCameraType type)
	{
		CameraBase cameraComponent = null;
		if (Application.isPlaying)
		{
			if (null != Camera.main && null != Camera.main.gameObject)
			{
				cameraComponent = Camera.main.gameObject.GetComponent<CameraBase>();
			}
		}
		else
		{
			cameraComponent = GameObject.FindObjectOfType(typeof(CameraBase)) as CameraBase;
		}

		if (null != cameraComponent)
		{
			cameraComponent.debugCamera = type;
		}
	}
#endif

	public BlendedCameraBehavior.BlendedCameraOutput _blendedResult = new BlendedCameraBehavior.BlendedCameraOutput();
	protected BlendedCameraBehavior.BlendedCameraOutput _runningBehaviorResult = new BlendedCameraBehavior.BlendedCameraOutput();
	
	protected void UpdateCameraBehaviors(float delatTime)
	{		
		//if (Application.isPlaying) // we only want the output from the running behaviors if we're playing
		//{
			for (int running = GetOldestRunningBehaviorIndex(); running <= GetNewestRunningBehaviorIndex(); ++running)
			{
				if (_gameCamera != _runningBehaviors[running].behavior) // we don't tick the game camera here, as that is already ticked in the update function
				{
					_runningBehaviors[running].behavior.Update(this, delatTime);
				}
				
				if (!_runningBehaviors[running].HasLerpStyleBeenSet())
				{
					GameCameraBehavior olderBehavior = (GetOldestRunningBehaviorIndex() == running) ? null : _runningBehaviors[running-1].behavior;
					_runningBehaviors[running].lerpStyle = CameraLerp.DetermineBestLerpStyle(olderBehavior, _runningBehaviors[running].behavior);
				}
				
				_runningBehaviors[running].behavior.GetLookAt(ref _runningBehaviorResult.lookAt);
				_runningBehaviors[running].behavior.GetRotation(ref _runningBehaviorResult.rotation);
				_runningBehaviorResult.distFromLookAt = _runningBehaviors[running].behavior.GetDistFromLookAt();
				
				// oldest camera is always fully blended in
				float lerp = 1f;
				if(NeedCalculateLerp(running))
				{
					lerp = Mathf.Clamp01(_runningBehaviors[running].CalculateLerpValue());
				}
				float smoothedLerp = 1f;
				float smoothedLerpPitch = 1f;
				float smoothedLerpYaw = 1f;
				if (NeedSmoothRunningBehavior(running)) // not the oldest running cam and not fully blended in
				{
					smoothedLerp = Cinematic.Smooth(lerp, _runningBehaviors[running].lerp.dialogueCameraLerpSmoothing, _runningBehaviors[running].lerp.animationCurve);
					smoothedLerpPitch = Cinematic.Smooth(lerp, _runningBehaviors[running].lerp.pitchLerpSmoothing, _runningBehaviors[running].lerp.curvePitchLerp);
					smoothedLerpYaw = Cinematic.Smooth(lerp, _runningBehaviors[running].lerp.yawLerpSmoothing, _runningBehaviors[running].lerp.curveYawLerp);
				}				
				BlendedCameraBehavior.BlendedCameraOutput.Lerp(ref _blendedResult, ref _blendedResult, ref _runningBehaviorResult, 
															   _runningBehaviors[running].lerpStyle, smoothedLerp, smoothedLerpPitch, smoothedLerpYaw);
				_gameCameraPosition = _blendedResult.CalculatePosition();
				_gameCameraRotation = _blendedResult.rotation;
        }
        //}
        RemoveRunningBehaviors();
	}
	
	// remove any running behaviors which have been blended out
	protected virtual void RemoveRunningBehaviors()
	{
		for (int running = GetNewestRunningBehaviorIndex(); running > GetOldestRunningBehaviorIndex(); --running)
		{
			if (_runningBehaviors[running].IsFullyBlendedIn()) // this behavior has been fully blended-in
			{
				_runningBehaviors.RemoveRange(0, running); // all older behaviors are not contributing anymore
				break;
			}
		}
	}

	protected virtual bool NeedCalculateLerp(int running)
	{
		return GetOldestRunningBehaviorIndex() != running;
	}

	protected virtual bool NeedSmoothRunningBehavior(int running)
	{
		if(_runningBehaviors == null)
		{
			return false;
		}
		if(running < 0 || running >= _runningBehaviors.Count)
		{
			return false;
		}
		return GetOldestRunningBehaviorIndex() != running && !_runningBehaviors[running].IsFullyBlendedIn();
	}
	
	protected int GetNewestRunningBehaviorIndex()
	{
		return _runningBehaviors.Count - 1;
	}
	
	protected int GetOldestRunningBehaviorIndex()
	{
		return 0;
	}
	
	private void OnTweenViewport(float f)
	{
	}
	
	private void OnCinematicEvent(CinematicEvent evt)
	{
		if (CinematicEvent.CinematicEventType.starting == evt.GetCinematicEventType())
		{
			_isCinematicActive = true;
		}
		else if (CinematicEvent.CinematicEventType.ending == evt.GetCinematicEventType())
		{
			_isCinematicActive = false;
			if (evt.GetSwitches().IsTransitionToStillCameraRequired())
			{
				EnterStillCamera();
			}
		}
		
#if UNITY_EDITOR
		if (null != theEditor)
		{
			theEditor.Repaint();
		}
#endif
	}

	private BlendedCameraBehavior AddNewBehavior(GameCameraBehavior newGameCamera, CameraLerp lerp)
	{
		if (GetNewestRunningBehaviorIndex() >= 0) // if there are any behaviors currently
		{
			if (GameCameraBehavior.AreInputParametersEqual(_runningBehaviors[GetNewestRunningBehaviorIndex()].behavior, newGameCamera)) // are we trying to add an exact same behavior?
			{
				return null; // no need to add a duplicate
			}
			
			if (BlendedCameraBehavior.IsTransitionImmediate(lerp)) // if it is a cut
			{
				_runningBehaviors.Clear(); // can remove everything currently in the list as it will not effect output after cutting to new camera
			}
		}
		
		BlendedCameraBehavior newRunningBehavior = new BlendedCameraBehavior(lerp, newGameCamera);
		_runningBehaviors.Add(newRunningBehavior);
		
#if UNITY_EDITOR
		if (null != theEditor)
		{
			theEditor.Repaint();
		}
#endif
		return newRunningBehavior;
	}
	
	private void OnTwoFingerTouchStartEvent(TwoFingerTouchStartEvent evt)
	{
		_initialPinch = evt.screenPosition2 - evt.screenPosition1;
		StartPinch();
	}
	
	private void OnTwoFingerTouchUpdateEvent(TwoFingerTouchUpdateEvent evt)
	{
		Vector3 currentPinch = evt.screenPosition2 - evt.screenPosition1;
		float pinchRatio = currentPinch.magnitude / _initialPinch.magnitude;
		UpdatePinch(pinchRatio);
	}
	
	private void StartPinch()
	{
		_pinchStartDistance = zoomDistance;
	}

	protected static float zoomDistanceMemory=-1f;
	private void UpdatePinch(float pinchRatio)
	{
		zoomDistance = _pinchStartDistance + zoomSpeed * (1 - pinchRatio);
		zoomDistance = Mathf.Clamp(zoomDistance, 0f, 1f);
		zoomDistanceMemory = zoomDistance;
    }
	
	private ParallaxGyroMonitor _parallaxMonitor;
	
	protected Quaternion ApplyGyros(Quaternion initialRotation,float deltaTime)
	{
		if (_parallaxMonitor == null)
		{
			_parallaxMonitor = new ParallaxGyroMonitor();
		}
		_parallaxMonitor.Update(deltaTime);
		
		Quaternion gyroModifier = Quaternion.identity;
		gyroModifier.eulerAngles = _parallaxMonitor.RotationValue;
		return initialRotation * gyroModifier;
	}

	protected void RemoveThisBehavior(BlendedCameraBehavior thisBehavior)
	{
		_runningBehaviors.Remove(thisBehavior);
	}
}
