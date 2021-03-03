using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFollowCamera : MonoBehaviour
{
    static public MyFollowCamera Instance;

    public bool IsLimitY = false;

    public float minY = 35;
    public float maxY = 325;

    public float minX = 10;
    public float maxX = 45;
    public Transform target;
    public Camera followCamera;
    public float wheelSpeed = 5;
    public float followOffsetY = 1f;
	public float CameraRotationalSpeed = 0.0625f;

	//private Vector3 followOffset;
	public CombatCamera cc;
    public bool isActive = false;
    public bool isOpenTouchView = true;
    private Vector3 _mousePosition;
    private bool _isRotate = false;
    private Transform _rotationTarget;
    private bool _isTouchInView = false;
    private bool _isTouchCharacter;
    private bool _isTouchDown;
    private float touchTime;
    private bool initialized;

    public delegate void TouchDownInView();
    public delegate void TouchCharacter();

    public static TouchDownInView delTouchDownInView;
    public static TouchCharacter delTouchCharacter;

	public static float CAMERA_ROTATIONAL_SPEED = 0.0625f;

	void Awake()
    {
        Instance = this;

        if (GameEngine.Instance == null)
        {
            Init();
        }
        else
        {
            TimerManager.instance.RemoveTimer(OnTimerSetCamera);
            TimerManager.instance.AddTimer(66, 1, OnTimerSetCamera);
        }
        // Hotfix_LT.Messenger.AddListener<string>(Hotfix_LT.EventName.CombatBossCameraEvent, OnCombatBossCameraListenerEx);
    }

    void OnDestory()
    {
        // Hotfix_LT.Messenger.RemoveListener<string>(Hotfix_LT.EventName.CombatBossCameraEvent, OnCombatBossCameraListenerEx);
    }

    public void OnCombatBossCameraListenerEx(string name)
    {
        float size = (float)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.NewGameConfigTemplateManager", "Instance", "GetGameConfigValue", string.Format("{0}CameraParam", name));
        if (size != 0)
        {
            Instance.transform.localPosition = new Vector3(Instance.transform.localPosition.x, size, Instance.transform.localPosition.z);
        }
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

    public void Init()
    {
        //EasyTouchSimpleEvent.SwipeAction += OnSwipe;
        //EasyTouchSimpleEvent.Swipe2FingerAction += OnSwipe2Finger;
        //EasyTouchSimpleEvent.SwipeEnd2FingerAction += OnSwipeEnd2Finger;
        //InputManager.MouseScrollWheelAction += OnMouseScrollWheel;
        if (initialized) return;
        initialized = true;
        if (followCamera == null)
        {
            followCamera = Camera.main;
        }
        if (followCamera == null)//再次确认是否存在,场景里不拖拽相机到这脚本上，就需要代码赋值，但存在camera被隐藏的情况，因此通过名字查找
        {
            Transform obj =transform.parent.Find("Main Camera");
            if (obj != null)
            {
                followCamera = obj.GetComponent<Camera>();
            }
            else
            {
                EB.Debug.LogError("Can't find Main Camera!!!");
            }
        }
        cc = followCamera.GetComponent<CombatCamera>();

        if (isActive)
        {
            //followOffset = new Vector3(0, followOffsetY, 0);
            Transform buffParent = transform.parent;
            followCamera.transform.LookAt(this.transform);
            transform.SetParent(followCamera.transform);
            transform.localEulerAngles = Vector3.zero;
            transform.SetParent(buffParent);
            followCamera.transform.SetParent(this.transform);
            if (cc != null)
            {
                cc.State = CombatCamera.MotionState.Freedom;
            }
        }

        if (_rotationTarget == null)
        {
            _rotationTarget = new GameObject("RotationTarget").transform;
            _rotationTarget.rotation = transform.rotation;
        }

        delTouchCharacter += OnTouchCharacter;
        delTouchDownInView += OnTouchDownInView;

        if (GameEngine.Instance == null)
        {
            isOpenTouchView = false;
        }

		CameraRotationalSpeed = CAMERA_ROTATIONAL_SPEED;
	}

	bool isLerpDefaultCameraView;
    private float lerpDefaultViewSpeed = 2400;
    private float lerpThisThreshold = 1f;
    public bool ResetCameraView()
    {
        if (cc.State != CombatCamera.MotionState.Freedom)
        {
            return false;
        }
        isLerpDefaultCameraView = true;
        return true;
    }

    public void GetIsLerpDefaultCameraView()
    {
        isLerpDefaultCameraView = false;
    }

    public void ResetToDefaultView()
    {
        isActive = false;
        followCamera.transform.SetParent(this.transform.parent);

        if (cc == null)
        {
            return;
        }
        cc.transform.position = cc.OriginalPosition;
        cc.transform.eulerAngles = cc.OriginAngle;

        isActive = true;
        Transform buffParent = transform.parent;
        followCamera.transform.LookAt(this.transform);
        transform.SetParent(followCamera.transform);
        transform.localEulerAngles = Vector3.zero;
        transform.SetParent(buffParent);
        followCamera.transform.SetParent(transform);
        _rotationTarget.rotation = transform.rotation;
        cc.State = CombatCamera.MotionState.Freedom;
    }

    public void ResumeCameraView()
    {
        if (cc == null)
        {
            return;
        }
        transform.rotation = _rotationTarget.rotation;
        followCamera.transform.SetParent(null);
        followCamera.transform.position = cc.OriginalPosition;
        followCamera.transform.eulerAngles = cc.OriginAngle;
        Transform buffParent = transform.parent;
        transform.SetParent(followCamera.transform);
        transform.localEulerAngles = Vector3.zero;
        transform.SetParent(buffParent);
        followCamera.transform.SetParent(transform); //应该恢复至此 
        transform.rotation = _rotationTarget.rotation;
        cc.State = CombatCamera.MotionState.Freedom;
        isActive = true;
    }

    void OnDestroy()
    {
        //EasyTouchSimpleEvent.SwipeAction -= OnSwipe;
        //EasyTouchSimpleEvent.Swipe2FingerAction -= OnSwipe2Finger;
        //EasyTouchSimpleEvent.SwipeEnd2FingerAction -= OnSwipeEnd2Finger;
        //InputManager.MouseScrollWheelAction -= OnMouseScrollWheel;

        Instance = null;

        delTouchCharacter -= OnTouchCharacter;
        delTouchDownInView -= OnTouchDownInView;

        if (_rotationTarget != null)
        {
            Destroy(_rotationTarget.gameObject);
        }
    }

    void OnTouchCharacter()
    {
        if (Time.unscaledTime - touchTime < 0.1f)
        {
            _isTouchCharacter = true;
        }
    }

    void OnTouchDownInView()
    {
        if (Time.unscaledTime - touchTime < 0.1f)
        {
            _isTouchInView = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized || followCamera == null)
        {
            return;
        }

        if (isActive)
        {
            if (_rotationTarget == null)
            {
                _rotationTarget = new GameObject("RotationTarget").transform;
                _rotationTarget.rotation = transform.rotation;
            }

            if (cc != null && cc.State != CombatCamera.MotionState.Freedom)
            {
                cc.State = CombatCamera.MotionState.Freedom;
            }
            followCamera.transform.LookAt(this.transform);
            if (followCamera.transform.localEulerAngles != Vector3.zero)
            {
                followCamera.transform.SetParent(null);
                Transform buffParent = transform.parent;
                transform.SetParent(followCamera.transform);
                transform.localEulerAngles = Vector3.zero;
                transform.SetParent(buffParent);
                followCamera.transform.SetParent(this.transform);
            }

            if (isLerpDefaultCameraView)
            {
                if (cc != null)
                {
                    //if(preLerpDir == Vector3.zero|| (Vector3.Dot(lerpDir, preLerpDir)) > 0)
                    float distanceWithTarget = Vector3.Magnitude(cc.OriginalPosition - cc.transform.position);
                    if (distanceWithTarget > lerpThisThreshold)
                    {
                        Vector2 lerpDir = (cc.transform.position - cc.OriginalPosition).normalized;
                        Vector2 deltaValue = lerpDir * lerpDefaultViewSpeed * Time.deltaTime;
                        //if (Vector3.Magnitude(deltaValue) < distanceWithTarget)
                        OnSwipe(deltaValue, true);
                    }
                    else
                    {
                        EB.Debug.Log("lerpDefaultCameraView over");
                        isLerpDefaultCameraView = false;
                        ResetToDefaultView();
                        // Hotfix_LT.Messenger.Raise<bool>(Hotfix_LT.EventName.DisplayCameraViewButton, false);
                        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "DisplayCameraViewButton", false);
                    }
                }
            }

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                _isTouchCharacter = false;
                _isTouchInView = false;
                touchTime = Time.unscaledTime;
                _mousePosition = Input.mousePosition;
                _isTouchDown = true;
            }
            else if (Input.GetMouseButton(0))
            {
                if (!_isTouchDown) //鼠标移出去的情况
                {
                    _mousePosition = Input.mousePosition;
                    _isTouchDown = true;
                    touchTime = Time.unscaledTime;
                }
                else
                {
                    Vector3 delta = Input.mousePosition - _mousePosition;
                    Vector2 v = new Vector2(delta.x, delta.y);
                    if (!isOpenTouchView || (!_isTouchCharacter && !_isTouchInView)) //不点到角色且不触碰界面才旋转摄像机
                    {
                        OnSwipe(v);
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _isTouchDown = false;
                _isTouchCharacter = false;
                _isTouchInView = false;
                touchTime = Time.unscaledTime;
            }

            _mousePosition = Input.mousePosition;
#else
            if (Input.touchCount == 1) //单点触碰移动摄像机
            {
                if (Input.touches[0].phase == TouchPhase.Moved) //手指在屏幕上移动，移动摄像机
                {
                    if (!isOpenTouchView || (!_isTouchCharacter && !_isTouchInView)) //不点到角色且不触碰界面才旋转摄像机
                    {
                        OnSwipe(Input.touches[0].deltaPosition * 0.5f);// * 0.5f 减慢触控旋转的灵敏度
                    }
                    //transform.Translate(new Vector3(Input.touches[0].deltaPosition.x * Time.fixedDeltaTime, Input.touches[0].deltaPosition.y * Time.fixedDeltaTime, 0));
                }
                else if(Input.touches[0].phase == TouchPhase.Began)
                {
                    _isTouchCharacter = false;
                    _isTouchInView = false;
                    touchTime = Time.unscaledTime;
                }
                else if(Input.touches[0].phase == TouchPhase.Ended|| Input.touches[0].phase == TouchPhase.Canceled)
                {
                    _isTouchInView = false;
                    _isTouchCharacter = false;
                    touchTime = Time.unscaledTime;
                }
            }
#endif
            if (_isRotate)
            {
                if (Quaternion.Angle(transform.rotation, _rotationTarget.rotation) > 0.5f)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, _rotationTarget.rotation, 1.0f);
                }
                else
                {
                    _isRotate = false;
                    transform.rotation = _rotationTarget.rotation;
                }
            }
        }
        else
        {
            isLerpDefaultCameraView = false;
        }
        //transform.position = target.position + followOffset;//Vector3.Lerp(transform.position, target.position + followOffset, 0.05f);
    }

    void OnSwipe(Vector2 deltaPos, bool isResetDefault = false)
    {
        _isRotate = true;
        _rotationTarget.Rotate(Vector3.down * -deltaPos.x * CameraRotationalSpeed, Space.World);
        if (IsLimitY)
        {
            float y = _rotationTarget.transform.localEulerAngles.y;
            if (deltaPos.x > 0)//左边
            {
                if (!IsYVaild(y))
                {
                    if (_rotationTarget.transform.localEulerAngles.y > maxY)
                    {
                        _rotationTarget.transform.localEulerAngles = new Vector3(_rotationTarget.transform.localEulerAngles.x, maxY, _rotationTarget.transform.localEulerAngles.z);
                    }
                }
            }
            else if (deltaPos.x < 0)//右边
            {
                if (!IsYVaild(y))
                {
                    if (_rotationTarget.transform.localEulerAngles.y < minY)
                    {
                        _rotationTarget.transform.localEulerAngles = new Vector3(_rotationTarget.transform.localEulerAngles.x, minY, _rotationTarget.transform.localEulerAngles.z);
                    }
                }
            }
        }

        RenderSettings rs = (RenderSettings)RenderSettingsManager.Instance.GetCurrentRenderSettings();
        rs.GlobalLightRotation += Vector3.down * -deltaPos.x;

        float angleX = CheckAngle(_rotationTarget.localEulerAngles.x);

        if ((deltaPos.y < 0 && angleX < maxX) || (deltaPos.y > 0 && angleX > minX))
        {
            float y = deltaPos.y * 0.25f;
            if (deltaPos.y < 0)
            {
                if (angleX - y > maxX)
                {
                    y = angleX - maxX;
                }
            }
            else
            {
                if (angleX - y < minX)
                {
                    y = angleX - minX;
                }
            }

            _rotationTarget.Rotate(Vector3.right * -y, Space.Self);
            rs.GlobalLightRotation += Vector3.right * -y;
        }

        if (deltaPos.x != 0f || deltaPos.y != 0f)
        {
            if (!isResetDefault)
            {
                isLerpDefaultCameraView = false;
                // Hotfix_LT.Messenger.Raise<bool>(Hotfix_LT.EventName.DisplayCameraViewButton, true);
                GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "DisplayCameraViewButton", true);
            }

        }
    }

    private bool IsYVaild(float y)
    {
        if ((y > 0 && y < maxY) || (y > minY && y < 360))
        {
            return true;
        }
        return false;
    }

    public float CheckAngle(float value)
    {
        float angle = value - 180;

        if (angle > 0)
            return angle - 180;

        return angle + 180;
    }

    //private float oldDistance;

    //void OnSwipe2Finger(float distance)
    //{
    //    if (oldDistance == 0)
    //    {
    //        oldDistance = distance;
    //    }
    //    followCamera.transform.position += followCamera.transform.forward * (distance - oldDistance) * 0.05f;
    //    oldDistance = distance;
    //}

    //void OnSwipeEnd2Finger()
    //{
    //    oldDistance = 0;
    //}

    //void OnMouseScrollWheel(float wheel)
    //{
    //    followCamera.transform.position += followCamera.transform.forward * (wheel) * wheelSpeed;
    //}
}
