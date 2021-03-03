using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Thinksquirrel.CShake;

public class CombatCamera : CameraBase
{
    public GameObject m_lookat_locator;
    public GameObject m_skill_select_lookat_locator;
    public GameObject m_qte_lookat_locator;
    public GameObject m_chest_locator;
    public GameObject m_teams_spawn_lookat_locator;
	public GameObject m_teams_victory_lookat_locator;
	public float m_camera_idle_lerp_time = 10f;
    public float m_cameraLerpBackTime = 0.3f;
    public CameraLerp.LerpSmoothing m_cameraLerpBackSmooting = CameraLerp.LerpSmoothing.fastSlow;
    public AnimationCurve m_cameraLerpBackAnimationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
    public string m_showSkillCameraData;
    public string m_showQteCameraData;
    public string m_battleResultCameraData;
    public string m_teamsSpawnCameraData = "TeamsSpawnCamera";
    public GameObject[] waypointObjects;
	public GameObject m_bossBattleViewPos;
    public string battleOverRenderSettings;
    public bool m_enablePlayerOrChallengerMotion = true;
    public bool m_enableOpponentMotion = true;

    private bool m_initialized = false;

    private Vector3 m_origin_pos;
    private Vector3 m_origin_angle;

    private Vector3[] m_waypoints = null;
    private Vector3[] m_currentWaypoints = null;

	private object m_TweenID = null;
    private bool m_isLerpingBackToIdlePath = false;  //lerp的时候设为true，当重新lerp完一个圈后重置为false
    private float m_lerpback_starttime = 0.0f;
    private int m_lerpback_index = 0;
    private Vector3 m_lerpback_startposition = Vector3.zero;
    private Quaternion m_lerpback_startquaterion = Quaternion.identity;
    private Quaternion m_lerpback_endquaterion = Quaternion.identity;

    private bool m_camera_holding = false;
	public Transform m_geometry;
	public static bool isBoss;
	public bool m_bossCameraInTestMode;
	public Vector3 m_bossScale=new Vector3(1.24f,1.24f,1.24f);

    public bool HoldingCamera
    {
        get { return m_camera_holding; }
        set { m_camera_holding = false; }
    }

    public enum MotionState
    {
        Idle,
        Invalid,
        Lerping,
        HangOn,
        LerpingBack,
        Freedom,
        LookAt,
    }

    MotionState _motionState = MotionState.Invalid;
    public MotionState State
    {
        get { return _motionState; }
        set
        {
            if (_motionState != value)
            {
                if (_motionState == MotionState.Idle)
                {
                    if (m_TweenID != null)
                    {
                        DOTween.Kill(m_TweenID);
						m_TweenID = null;
                    }
                    m_isLerpingBackToIdlePath = false;
                }

                _motionState = value;
            }
        }
    }
    
    private bool _resumeFollowCamera;
    public void SetResumeFollowCamera()
    {
        _resumeFollowCamera = true;
    }

    public Vector3 OriginalPosition
    {
        get { return m_origin_pos; }
    }

    public Vector3 OriginAngle
    {
        get { return m_origin_angle; }
    }

    /// <summary>
    /// 自行判空
    /// </summary>
    /// <value></value>
    public static CombatCamera Instance{get; private set;} = null;

    void Awake()
    {
        if (Application.isPlaying)
        {
			//SetBossCameraView();
			// adjust camera position
			float max = 16.0f / 9.0f;
            float min = 16.0f / 10.0f;//by pj 缩小范围 保持镜头不穿帮
            //float current = GetComponent<Camera>().aspect;
            //current = Mathf.Max(min, current);
            //float factor = max / Mathf.Max(min, current);

            // 锁定范围大小，保证所有分辨率比例下显示一致
            float factor = max / min;

			if (waypointObjects != null)
            {
                for (int i = 0; i < waypointObjects.Length; ++i)
                {
                    Vector3 localPosition = waypointObjects[i].transform.localPosition;
                    localPosition *= factor; 
                    waypointObjects[i].transform.localPosition = localPosition;
                }
            }
        }

        if (GetComponent<CameraShake>() == null)
        {
            gameObject.AddComponent<CameraShake>();
        }

        Instance = this;
    }

    private void OnDestroy() {
        Instance = null;
    }

#if UNITY_EDITOR && DEBUG
    void OnDrawGizmos()
    {
        InitializeWaypoints();

        if (m_currentWaypoints != null && m_currentWaypoints.Length > 0)
        {
            Gizmos.DrawSphere(transform.position, 1f);
            DoTweenUtils.DrawLine(m_currentWaypoints, Color.green);
        }

        if (m_waypoints != null && m_waypoints.Length > 0)
        {
            DoTweenUtils.DrawLine(m_waypoints, Color.blue);
        }
    }
#endif

	void SetBossCameraView()
	{
		if (isBoss || (GameEngine.Instance == null && m_bossCameraInTestMode))
		{
			isBoss = false;
			for (int j = 0; j < waypointObjects.Length; ++j)
			{
				waypointObjects[j].transform.position = m_bossBattleViewPos.transform.position;
			}
			if (m_geometry == null)
				m_geometry = transform.parent.parent.Find("Geometry");
			if (m_geometry == null)
				m_geometry = transform.root.Find("Geometry");
			if (m_geometry == null)
				Debug.LogError("m_geometry == null");
			else
				m_geometry.localScale = m_bossScale;
		}
	}

    public void OnTeamsVictoryListener()
    {
		if (!string.IsNullOrEmpty(m_teamsSpawnCameraData) && m_teams_victory_lookat_locator != null)
		{
			CameraMotion motion = GlobalCameraMotionData.Instance.GetCameraMotion(m_teamsSpawnCameraData);
			if (motion != null)
			{
                try
                {
                    // stop lerp back				
                    DOTween.KillAll(true);
                }
                catch (System.IndexOutOfRangeException e)
                {
                    EB.Debug.LogError(e.ToString());
                }

				_runningBehaviors.Clear();
				List<GameObject> targets = new List<GameObject>();			
				targets.Add(m_teams_victory_lookat_locator);

				GameCameraParams gameCameraParams = (GameCameraParams)motion.camera;
				if (gameCameraParams == null)
					Debug.LogError("OnTeamsVictoryListener gameCameraParams is null");				
				CameraLerp motion_lerp = motion.cameraLerpOverride;
				if (motion_lerp == null)
					Debug.LogError("OnTeamsVictoryListener motion_lerp is null");
				else
				{
					motion_lerp.dialogueCameraLerpTime = 5f;
					motion_lerp.hangonTime = int.MaxValue;
				}
				EnterInteractionCamera(ref targets, ref gameCameraParams, motion_lerp);
			}
		}
	}

    public void SwitchToBattleResultCamera()
    {
        EB.Debug.Log("[CombatCamera]SwitchToBattleResultCamera");

        DisableBlurEffect();
        CameraMotion motion = GlobalCameraMotionData.Instance.GetCameraMotion(m_battleResultCameraData);
        if (motion != null)
        {
            List<GameObject> targets = new List<GameObject>();
            targets.Add(m_skill_select_lookat_locator);
            GameCameraParams gameCameraParams = (GameCameraParams)motion.camera;
            CameraLerp motion_lerp = motion.cameraLerpOverride;
            EnterInteractionCamera(ref targets, ref gameCameraParams, motion_lerp);
        }
    }

    protected override void OnComponentStart()
    {
        base.OnComponentStart();
#if UNITY_EDITOR
        GlobalCameraData.LoadCameraData(GlobalCameraData.COMBAT_VIEW_CAMERA);
#endif
        InitializeWaypoints();
        transform.position = m_waypoints[0];

        if (m_lookat_locator != null)
        {
            transform.LookAt(m_lookat_locator.transform.position);
            _gameCamera.SetGameCameraPositionAndRotation(m_lookat_locator.transform.position, this);
            _gameCamera.GetPosition(ref _gameCameraPosition);
            _gameCamera.GetRotation(ref _gameCameraRotation);
            //transform.position = _gameCameraPosition;
            //transform.rotation = _gameCameraRotation;
        }

        State = MotionState.Idle;//由于此处写明当Start的时候直接是从Invalid变为Idle 可视为默认应该是idle状态 
        m_origin_pos = transform.position;
        m_origin_angle = transform.eulerAngles;
        _gameCameraPosition = transform.position;
        _gameCameraRotation = transform.rotation;

        DisableBlurEffect();
        m_initialized = true;
    }

    void InitializeWaypoints()
    {
        if (waypointObjects == null || waypointObjects.Length < 1)
        {
            EB.Debug.LogError("[CombatCamera]No waypoint objects defined!");
        }

        int waypointsCount = waypointObjects.Length;
        m_waypoints = new Vector3[waypointsCount];
        int j = 0;
        //m_waypoints[j++] = waypointObjects[0].transform.position;

        for (int i = 0; i < waypointObjects.Length; i++)
        {
            m_waypoints[j++] = waypointObjects[i].transform.position;
        }

        //m_waypoints[j] = m_waypoints[j - 1];
    }

    protected override void OnComponentEnable()
    {
        base.OnComponentEnable();
        EventManager.instance.AddListener<TapEvent>(OnTapEvent);
        // Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.CombatTeamsVictoryEvent, OnTeamsVictoryListener);
    }

    protected override void OnComponentDisable()
    {
        base.OnComponentDisable();
        transform.position = m_origin_pos;
        transform.eulerAngles = m_origin_angle;
        State = MotionState.Invalid;
        EventManager.instance.RemoveListener<TapEvent>(OnTapEvent);
        // Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.CombatTeamsVictoryEvent, OnTeamsVictoryListener);

        m_camera_holding = false;
        DisableBlurEffect();
    }

    void OnTapEvent(TapEvent evt)
    {
        if (evt.target != null)
        {
            MeshClick mesh_click = evt.target.gameObject.GetComponent<MeshClick>();
            if (mesh_click != null)
            {
                mesh_click.OnMeshClick(evt.target.gameObject);
            }
        }
    }

  //  public override void EnterInteractionCamera(ref List<GameObject> targets, ref GameCameraParams gameCamParams, CameraLerp lerp)
    public override BlendedCameraBehavior EnterInteractionCamera(ref List<GameObject> targets, ref GameCameraParams gameCamParams, CameraLerp lerp)
    {
        base.EnterInteractionCamera(ref targets, ref gameCamParams, lerp);
		_gameCameraPosition = transform.position;
		_gameCameraRotation = transform.rotation;
		if (m_lookat_locator != null)
		{
			_blendedResult.lookAt = m_lookat_locator.transform.position;
			Vector3 lookForward = m_lookat_locator.transform.position - transform.position;
			lookForward.Normalize();
			_blendedResult.rotation = Quaternion.LookRotation(lookForward);
			_blendedResult.distFromLookAt = Vector3.Distance(transform.position, m_lookat_locator.transform.position);
		}
        MainCamera.nearClipPlane = gameCamParams.clipNear;//增加对摄像机近裁剪面的控制
		State = MotionState.Lerping;
        return null;
    }
    

    public void OnIdleCamera()
    {
        State = MotionState.Idle;
        _runningBehaviors.Clear();
    }

    public bool IsIdle()
    {
        //by pj 镜头自由旋转的时候为Freedom 这时候也是需要显示血条 行动也需要
        return (State == MotionState.Idle || State == MotionState.Freedom) /*&& _runningBehaviors.Count == 0*/;
    }

    protected override void UpdateGameCamera(float deltaTime)
    {
        if (!m_initialized)
        {
            return;
        }

        if (m_camera_holding)
        {
            return;
        }

        base.UpdateGameCamera(deltaTime);

        if (State == MotionState.Lerping)
        {
            if (_runningBehaviors.Count > 0)
            {
                UpdateCameraBehaviors(deltaTime);
                if (!_isCinematicActive)
                {
                    transform.position = _gameCameraPosition;
                    transform.rotation = ApplyGyros(_gameCameraRotation, deltaTime);
                }
            }
            else
            {
                State = MotionState.Idle;
                //m_idleTween = null;
                //m_isLerpingBackToIdlePath = true;
                //MyFollowCamera.Instance.isActive = true;
            }
		}
        else if (State == MotionState.Idle)
        {
            UpdateIdleState();
        }
        else if(State == MotionState.LookAt)
        {
            UpdateCurrentLookAtState(deltaTime);
        }
        //by pj 由于写明当Start的时候直接是从Invalid变为Idle 可视为默认应该是idle状态 所以当是无效状态但是脚本激活的情况下默认应该进入Idle
        //{                                    //只要用来解决当编辑模式修改代码时造成脚本更新，以至于默认变为Invaild状态而导致摄像机功能缺失，导致摄像机朝向不正确的问题。
        //    if(enabled)                      //还有一种方案也可以通过修改枚举值顺序，使Idle ==0 Invaild==1这种方案来实现。
        //    {
        //        State = MotionState.Idle;
        //    }
        //}
    }
    private void UpdateCurrentLookAtState(float deltaTime)
    {
        if (MyFollowCamera.Instance)
            MyFollowCamera.Instance.isActive = false;
        if(_lookAtHangonTime>0)
        {
            _lookAtHangonTime-= deltaTime;
            if(_listTargets!=null)
            {
                Vector3 target_pos = GameUtils.CalculateBoundsWithOffset(_listTargets).center;
                CameraLookAt(target_pos);
            }
        }
        else
        {
            State = CombatCamera.MotionState.Freedom;
            if (MyFollowCamera.Instance)
                MyFollowCamera.Instance.isActive = true;
        }

    }

    private void UpdateIdleState()
    {
        if (_resumeFollowCamera)
        {
            _resumeFollowCamera = false;
            MyFollowCamera.Instance.ResumeCameraView();
            RenderSettingsManager.Instance.ReplaceGlobalCharacterOutline(1);
        }
        else if (m_TweenID == null)
		{
            if (!m_isLerpingBackToIdlePath)  //设为idle后，首先进这里  
            {
                float nearest_dist = -1f;
				int nearest_index = 0;

				for (int i = 0; i < waypointObjects.Length; i++)
				{
					float dist = (transform.position - waypointObjects[i].transform.position).sqrMagnitude;

					if (nearest_dist < 0)
					{
						nearest_dist = dist;
					}
					else
					{
						if (dist < nearest_dist)
						{
							nearest_dist = dist;
							nearest_index = i;
						}
					}
				}

				m_lerpback_starttime = Time.realtimeSinceStartup;
				m_lerpback_index = nearest_index;
				m_lerpback_startposition = transform.position;
				m_lerpback_startquaterion = transform.rotation;
				Vector3 lookForward = m_lookat_locator.transform.position - waypointObjects[nearest_index].transform.position;
				lookForward.Normalize();
				m_lerpback_endquaterion = Quaternion.LookRotation(lookForward);
				m_isLerpingBackToIdlePath = true;
				LerpBackToIdlePath(nearest_index);  //m_isLerpingBackToIdlePath标志位设为true，m_idleTween要等lerp到一点后才会设值
            }
			else
			{
				bool lerpback_finish = false;
				if (!Mathf.Approximately(m_cameraLerpBackTime, 0.0f))
				{
					float lerp = Mathf.Clamp01((Time.realtimeSinceStartup - m_lerpback_starttime) / m_cameraLerpBackTime);
					float smooth_lerp = Cinematic.Smooth(lerp, m_cameraLerpBackSmooting, m_cameraLerpBackAnimationCurve);
					transform.position = Vector3.Lerp(m_lerpback_startposition, waypointObjects[m_lerpback_index].transform.position, smooth_lerp);
					transform.rotation = Quaternion.Lerp(m_lerpback_startquaterion, m_lerpback_endquaterion, smooth_lerp);
					if (Time.realtimeSinceStartup - m_lerpback_starttime >= m_cameraLerpBackTime)
					{
						lerpback_finish = true;
					}
				}
				else
				{
					lerpback_finish = true;
				}

				if (lerpback_finish)
				{
					if (m_lerpback_index == 0)
					{
						StartClockWiseTween();
					}
					else if (m_lerpback_index == (waypointObjects.Length - 1))
					{
						StartCounterClockWiseTween();
					}
					else
					{
						LerpFromNearestWaypoint((object)m_lerpback_index);
					}
				}
			}
		}
		else
		{
			if (!m_isLerpingBackToIdlePath)
            {
                transform.LookAt(m_lookat_locator.transform.position);
            }
			if(MyFollowCamera.Instance)
				MyFollowCamera.Instance.isActive = true;
		}
	}

    protected override bool NeedCalculateLerp(int running)
    {
        return true;
    }

    protected override bool NeedSmoothRunningBehavior(int running)
    {
        if (_runningBehaviors == null)
        {
            return false;
        }
        if (running < 0 || running >= _runningBehaviors.Count)
        {
            return false;
        }
        return !_runningBehaviors[running].IsFullyBlendedIn();
    }

    protected override void RemoveRunningBehaviors()
    {
        for (int running = GetNewestRunningBehaviorIndex(); running >= GetOldestRunningBehaviorIndex(); --running)
        {
            if (_runningBehaviors[running].IsFullyBlendedIn()) // this behavior has been fully blended-in
            {
                _runningBehaviors.RemoveRange(0, running + 1); // all older behaviors are not contributing anymore
                break;
            }
        }
    }

    //m_isLerpingBackToIdlePath标志位设为true，m_idleTween要等lerp到一点后才会设值
    void LerpBackToIdlePath(int nearest_index)
    {
		//
        m_isLerpingBackToIdlePath = true;
        Vector3[] lerpBackPath = new Vector3[] { /*transform.position, */transform.position, waypointObjects[nearest_index].transform.position/*, waypointObjects[nearest_index].transform.position*/ };
		var option = transform.DOPath(lerpBackPath, m_cameraLerpBackTime, PathType.CatmullRom);

		if (nearest_index == 0)
		{
			option.onComplete = StartClockWiseTween;
		}
		else if(nearest_index == (waypointObjects.Length - 1))
		{
			option.onComplete = StartCounterClockWiseTween;
		}
		else
		{
			option.OnComplete(delegate() { LerpFromNearestWaypoint(nearest_index); });
		}

        // Lerp camera rotation.
        Vector3 lookForward = m_lookat_locator.transform.position - waypointObjects[nearest_index].transform.position;
        lookForward.Normalize();
        Quaternion newQuat = Quaternion.LookRotation(lookForward);       
		transform.DORotateQuaternion(newQuat, m_cameraLerpBackTime);
    }

    void LerpFromNearestWaypoint(object indexObj)
    {
        int index = (int)indexObj;
        //EB.Debug.Log("[CombatCamera]LerpToNearestWaypoint: index = " + index.ToString());
        bool isClockWise = true;
        m_currentWaypoints = new Vector3[waypointObjects.Length - index/* + 2*/];
        int j = 0;

        if (index == waypointObjects.Length/* - 2*/)
        {
            isClockWise = false;
        }
        else
        {
            Vector3 toNearest = waypointObjects[index].transform.position - transform.position;
            Vector3 toNextOne = waypointObjects[index + 1].transform.position - transform.position;
            float angle = Vector3.Angle(toNearest, toNextOne);

            //如果相邻的两个点间隔过大 直接跳到跳过
            if (angle > 90)
            {
                index += 1;
            }
        }

        //m_currentWaypoints[j++] = waypointObjects[index].transform.position;

        if (isClockWise)
        {
            for (int i = index; i < waypointObjects.Length; i++)
            {
                m_currentWaypoints[j++] = waypointObjects[i].transform.position;
            }

            //m_currentWaypoints[j] = m_currentWaypoints[j - 1];
        }
        else
        {
            for (int i = waypointObjects.Length - 1; i <= index && i >= 0 ; i--)
            {
                m_currentWaypoints[j++] = waypointObjects[i].transform.position;
            }

            //m_currentWaypoints[j] = m_currentWaypoints[j - 1];
        }

        float partialTimer = m_camera_idle_lerp_time * (waypointObjects.Length - index) / waypointObjects.Length;

		if (m_TweenID != null)
			DOTween.Kill(m_TweenID);

		var option = transform.DOPath(m_currentWaypoints, partialTimer, PathType.CatmullRom);
		m_TweenID = option.id;

		if (isClockWise)
		{
			option.onComplete = StartCounterClockWiseTween;
		}
		else
		{
			option.onComplete = StartClockWiseTween;
		}
	}

    void StartCounterClockWiseTween()
    {
#if UNITY_EDITOR && DEBUG
        InitializeWaypoints();
#endif
        //EB.Debug.Log("[CombatCamera]StartCounterClockWiseTween:");
        m_isLerpingBackToIdlePath = false;
        m_currentWaypoints = null;
        int j = 0;

        m_currentWaypoints = new Vector3[waypointObjects.Length/*+2*/];
        //m_currentWaypoints[j++] = waypointObjects[waypointObjects.Length - 1].transform.position;

        for (int i = waypointObjects.Length - 1; i >= 0; i--)
        {
            m_currentWaypoints[j++] = waypointObjects[i].transform.position;
        }

        //m_currentWaypoints[j] = m_currentWaypoints[j - 1];

		if (m_TweenID != null)
			DOTween.Kill(m_TweenID);

		var option = transform.DOPath(m_currentWaypoints, m_camera_idle_lerp_time, PathType.CubicBezier); m_TweenID = option.id;

		option.onComplete = StartClockWiseTween;

		//option.OnWaypointChange(index => { transform.DOLookAt(m_currentWaypoints[index], 0.5f, AxisConstraint.Y, null); });
	}

	void StartClockWiseTween()
    {
#if UNITY_EDITOR && DEBUG
        InitializeWaypoints();
#endif
        //EB.Debug.Log("[CombatCamera]StartClockWiseTween:");
        m_isLerpingBackToIdlePath = false;
        m_currentWaypoints = null;
        int j = 0;

        m_currentWaypoints = new Vector3[waypointObjects.Length/*+2*/];
        //m_currentWaypoints[j++] = waypointObjects[0].transform.position;

        for (int i = 0; i < waypointObjects.Length; i++)
        {
            m_currentWaypoints[j++] = waypointObjects[i].transform.position;
        }

        //m_currentWaypoints[j] = m_currentWaypoints[j - 1];

		if (m_TweenID != null)
			DOTween.Kill(m_TweenID);

		var option = transform.DOPath(m_currentWaypoints, m_camera_idle_lerp_time, PathType.CubicBezier); m_TweenID = option.id;
		option.onComplete = StartCounterClockWiseTween;

		//option.OnWaypointChange(index => { transform.DOLookAt(m_currentWaypoints[index], 0.5f, AxisConstraint.Y, null); });
	}

    private bool IsCloseEnough(Vector3 v1, Vector3 v2)
    {
        return Vector3.Distance(v1, v2) < 0.1f;
    }

    public void EnableBlurEffect()
    {
        if (battleOverRenderSettings != string.Empty && Application.isPlaying)
        {
            RenderSettingsManager.Instance.SetActiveRenderSettings(battleOverRenderSettings);
        }
    }

    public void DisableBlurEffect()
    {
        if (Application.isPlaying)
        {
            RenderSettingsManager.Instance.SetActiveRenderSettings(RenderSettingsManager.Instance.defaultSettings);
        }        
    }
    private List<GameObject> _listTargets;
    private float _lookAtHangonTime;
    public void CurrentCameraLookAt(ref List<GameObject> targets,float HangonTime)
    {
        State = MotionState.LookAt;
        if (MyFollowCamera.Instance)
            MyFollowCamera.Instance.isActive = false;
        Vector3 target_pos = GameUtils.CalculateBoundsWithOffset(targets).center;
        CameraLookAt(target_pos);
        _lookAtHangonTime = HangonTime;
        _listTargets = targets;
    }
    public void CameraLookAt(Vector3 target_pos)
    {
        Vector3 lookForward = target_pos - transform.position;
        transform.rotation = Quaternion.LookRotation(lookForward);
    }

    public void BlendCurrentCamera(ref List<GameObject> targets, float distOffset, float pitchOffset, float yawOffset, float heightOffset, CameraLerp lerp)
    {
        GameCameraParams param = ScriptableObject.CreateInstance<GameCameraParams>();
        if (param == null)
        {
            EB.Debug.LogError("Failed to create GameCameraParams instance in CombatCamera.BlendCurrentCamera");
        }

        if (targets == null || targets.Count == 0)
        {
            return;
        }

        Vector3 target_pos = GameUtils.CalculateBoundsWithOffset(targets).center;
        Vector3 camera_pos = transform.position;
        float dist = Vector3.Distance(camera_pos, target_pos);

        param.distance = dist + distOffset;
        param.pitch = pitchOffset;
        param.yawOffset = yawOffset;
        param.heightOffset = heightOffset;
        EnterInteractionCamera(ref targets, ref param, lerp);
        
    }

    public void CheckUnregisterTarget(GameObject target)
    {
        if (_runningBehaviors == null || _runningBehaviors.Count == 0)
        {
            return;
        }

        //iterator current running behavior list from tail to head and remove items that is focusing target. 
        for (int i = _runningBehaviors.Count - 1; i >= 0; i--)
        {
            if (_runningBehaviors[i].behavior != null && _runningBehaviors[i].behavior.IsFocusingTarget(target))
            {
                //this camera behavior has lost its target, so remove it.
                _runningBehaviors.RemoveAt(i);
            }
        }
    }

    public override void SetFieldOfView()
    {
        Camera cameraComp = GetComponent<Camera>();
        float fov = 55.0f;
        cameraComp.fieldOfView = fov;
    }
}