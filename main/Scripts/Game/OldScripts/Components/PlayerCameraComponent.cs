using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 主城跟随玩家模型的相机
/// </summary>
public class PlayerCameraComponent : CameraBase
{
    /// <summary>
    /// 当前相机Enable的时候
    /// </summary>
    public delegate void OnCameraEnable();
    public OnCameraEnable v_OnCameraEnable;
    private bool m_IsCameraEnable;

    [System.Serializable]
    public enum eViewMode
    {
        Normal,
        Left,
        Right
    }

    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            TimerManager.instance.RemoveTimer(OnTimerSetCamera);
            TimerManager.instance.AddTimer(66, 1, OnTimerSetCamera);
        }
        OnComponentEnable();//镜头缩放
    }
  
    private void OnTimerSetCamera(int seq)
    {
        GameObject go = GameObject.Find("Sequencer Camera");
        if (go != null)
        {
            Destroy(go);
        }
        //把自己下方的相机移除
        //Camera othercamera = this.GetComponentInChildren<Camera>();
        //if (othercamera != null)
        //{
        //    if (othercamera.name.IndexOf("Sequencer Camera") != -1)
        //    {
        //        othercamera.gameObject.CustomSetActive(false);
        //    }
        //}
    }

    public Transform TargetNormal
    {
        get
        {
            return _targetNormal;
        }
        set
        {
            _targetNormal = value;
            if (null != _targetNormal)
            {
                List<GameObject> targets = new List<GameObject>();
                targets.Add(TargetNormal.gameObject);
                _gameCamera.SetTargets(ref targets);
                _gameCamera.SetGameCameraPositionAndRotation(_targetNormal.position, this);
                _gameCamera.GetPosition(ref _gameCameraPosition);
                _gameCamera.GetRotation(ref _gameCameraRotation);
                transform.position = _gameCameraPosition;
                transform.rotation = _gameCameraRotation;
                EnterFixedOffsetGameCamera(null);
            }
        }
    }

    public Transform TargetLeft
    {
        get
        {
            return _targetLeft;
        }
        set
        {
            _targetLeft = value;
        }
    }

    public Transform TargetRight
    {
        get
        {
            return _targetRight;
        }
        set
        {
            _targetRight = value;
        }
    }

    [SerializeField]
    private eViewMode
        _viewMode = eViewMode.Normal;

    public eViewMode ViewMode
    {
        get
        {
            return _viewMode;
        }

        set
        {
            _viewMode = value;

            switch (value)
            {
                case eViewMode.Normal:
                default:
                    _currentTarget = TargetNormal;
                    //zoomDistance = 1.0f;
					if(zoomDistanceMemory==-1)
					{
						zoomDistance = GlobalCameraData.Instance.gameCameraStartZoomDistance;
					}
					else
					{
						zoomDistance = zoomDistanceMemory;
                    }
                    break;

                case eViewMode.Left:
                    _currentTarget = TargetLeft;
                    zoomDistance = 0.0f;
                    break;

                case eViewMode.Right:
                    _currentTarget = TargetRight;
                    zoomDistance = 0.0f;
                    break;
            }
        }
    }

    public Rect leftModeRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
    public Rect rightModeRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField]
    private Transform _targetNormal = null;
    [SerializeField]
    private Transform _targetLeft = null;
    [SerializeField]
    private Transform _targetRight = null;
    private Transform _currentTarget;

    public override void SetInitialCameraPosition()
    {
        _currentTarget = TargetNormal;

        if (_currentTarget == null)
        {
            return;
        }

        List<GameObject> targets = new List<GameObject>();
        targets.Add(TargetNormal.gameObject);
        _gameCamera.SetTargets(ref targets);
        _gameCamera.SetGameCameraPositionAndRotation(_currentTarget.position, this);
        _gameCamera.GetPosition(ref _gameCameraPosition);
        _gameCamera.GetRotation(ref _gameCameraRotation);
        transform.position = _gameCameraPosition;
        transform.rotation = _gameCameraRotation;
        EnterFixedOffsetGameCamera(null);
        EB.Debug.Log("game camera position : {0}", _currentTarget.position);
        EB.Debug.Log("camera position : {0}" , transform.position);
    }

    protected override void OnComponentStart()
    {
#if UNITY_EDITOR
        GlobalCameraData.LoadCameraData(GlobalCameraData.CAMPAIGN_VIEW_CAMERA);
#endif
        base.OnComponentStart();
        ViewMode = eViewMode.Normal;
    }

    protected override void OnComponentEnable()
    {
        base.OnComponentEnable();
    }
    
    protected override void UpdateGameCamera(float deltaTime)
    {
        //
        if (MainCamera.isActiveAndEnabled && v_OnCameraEnable != null && m_IsCameraEnable == false)
        {
            v_OnCameraEnable();
            m_IsCameraEnable = true;
        }
        if (MainCamera.isActiveAndEnabled == false)
        {
            m_IsCameraEnable = false;
        }
        //
        if (_currentTarget == null)
        {
            _currentTarget = TargetNormal;
        }

        if (_currentTarget != null && TargetNormal != null && !_shaking)
        {
            Vector3 cameraTargetPos = Vector3.zero;
            if (_currentTarget == _targetNormal)
            {
                cameraTargetPos = (_currentTarget.position);
            }
            else
            {
                cameraTargetPos = (_targetNormal.position + (_currentTarget.localPosition + Vector3.forward));
            }
           
            if (Application.isPlaying)
            {
                _gameCamera.LerpGameCameraToPositionAndRotation(cameraTargetPos, this, deltaTime);
            }
            else // when the game isn't running, update the player cam immediately (this is used by level designers to preview their levels)
            {
                _gameCamera.SetGameCameraPositionAndRotation(cameraTargetPos, this);
                _gameCamera.GetRotation(ref _gameCameraRotation);
                _gameCamera.GetPosition(ref _gameCameraPosition);
            }

            UpdateCameraBehaviors(deltaTime);

            if (_isCinematicActive)
            {
                EB.Debug.LogError("日志_isCinematicActive电影：{0}" , _isCinematicActive);
            }

            if (!_isCinematicActive)
            {
#if !UNITY_EDITOR
                /*if (Time.frameCount % 100 == 0)
                {
                    EB.Debug.Log(string.Format("[{0}]当前的相机数据_gameCameraPosition:{1},_gameCameraRotation:{2},farClipPlane:{3},nearClipPlane:{4}," +
                        "lookAt:{5},rotation:{6},distFromLookAt:{7},_runningBehaviorResult.lookAt:{8},_runningBehaviorResult.rotation:{9},_runningBehaviorResult.distFromLookAt:{10}",
                        Time.frameCount,
                        _gameCameraPosition,
                        _gameCameraRotation,
                        MainCamera.farClipPlane,
                        MainCamera.nearClipPlane,
                        _blendedResult.lookAt,
                        _blendedResult.rotation,
                        _blendedResult.distFromLookAt,
                        _runningBehaviorResult.lookAt,
                        _runningBehaviorResult.rotation,
                        _runningBehaviorResult.distFromLookAt
                        ));
                }*/
#endif
                //数值无效就修正
                if (float.IsNaN(_gameCameraPosition.x) || float.IsNaN(_gameCameraPosition.y) || float.IsNaN(_gameCameraPosition.z)|| float.IsNaN(_blendedResult.rotation.x) || float.IsNaN(_blendedResult.rotation.y))
                {
                    _gameCameraPosition = new Vector3(95, 11.1f, 82);
                    _gameCameraRotation = Quaternion.Euler(new Vector3(40, 180, 0));
                    //
                    _blendedResult.rotation = Quaternion.Euler(new Vector3(40, 180, 0));
                    _blendedResult.lookAt = Vector3.zero;
                    _blendedResult.distFromLookAt = 0.0f;

                    EB.Debug.Log("相机数值无效就修正!");
                }

                transform.position = _gameCameraPosition;
                transform.rotation = ApplyGyros(_gameCameraRotation, deltaTime);

                // 这段代码先不用采用,印象游戏里~没有高大的动态可拦视野的情况~by:wwh 20190406
                // determine if wall/obstacle should be transparent
                //UpdateObstacleVisibility(TargetNormal.position);
            }
        }
    }
    
	public static bool IsUseCameraEffect;
	public void EnterNpcCameraMotion(GameObject target)
	{
		if (!IsUseCameraEffect)
			return;

		List<GameObject> targets = new List<GameObject>();
		targets.Add(target);
		GameCameraParams gameCameraParams = GlobalCameraData.Instance.FindGameCameraParamsByName("NpcDialogueData");
		CameraLerp motion_lerp = GlobalCameraData.Instance.FindGameCameraLerpByName("dialogue lerp");  //npcDialogueCamMotion.cameraLerpOverride
		EnterInteractionCamera(ref targets, ref gameCameraParams, motion_lerp);
		HideMainMenu();
	}

	private BlendedCameraBehavior _curCameraMotion = null;
	public override BlendedCameraBehavior EnterInteractionCamera(ref List<GameObject> targets, ref GameCameraParams gameCamParams, CameraLerp lerp)
	{
		_curCameraMotion = base.EnterInteractionCamera(ref targets, ref gameCamParams, lerp);
        
		//_runningBehaviors[1].behavior.SetRotation(targets[0].transform.rotation);
		return null;
	}

	protected override void RemoveRunningBehaviors()
	{
        //base.RemoveRunningBehaviors(); // by pj 自动维系序列 移除过期的
	}

	public void ResetCameraState()
	{
		ShowMainMenu();

		if (_curCameraMotion != null )
		{
			//remove curCameraMotion
			if (_runningBehaviors != null && _runningBehaviors.Count > 0)
			{
				for (int i = _runningBehaviors.Count - 1; i >= 0; i--)
				{
					if (_runningBehaviors[i] == _curCameraMotion)
					{
						_runningBehaviors.RemoveAt(i);
					}
				}
			}
			//RemoveThisBehavior(_curCameraMotion);
			_curCameraMotion = null;
		}
	}

	private void ShowMainMenu()
	{
		//ShowMainMenu showMainMenuEvent = new ShowMainMenu();
		//EventManager.instance.Raise(showMainMenuEvent);
	}	

	private void HideMainMenu()
	{
		//HideMainMenu hideMainMenuEvent = new HideMainMenu();
		//EventManager.instance.Raise(hideMainMenuEvent);
	}
}
