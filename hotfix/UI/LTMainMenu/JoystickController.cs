using _HotfixScripts.Utils;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class JoystickController : DynamicMonoHotfix, IHotfixUpdate
    {
    	//static public JoystickController Instance;
    
    	public Transform MoveNavObject;
    	public Transform EntiretyObject;
    	public UISprite[] BGSprites, DirectionNavSprites;
    	public int RadiusSize = 260;
    	public float OffsetLength = 3;
    	public float MoveSpeed = 1;
    	private bool _IsPress = false;
    	private Hotfix_LT .Player.PlayerHotfixController _PlayerCtrl;
    	private Vector2 _OriginalPos;
    	private Vector2 _CenterPos;
    
		private bool HasRegisterUpdater;
        
    	public void AddTouchListener()
    	{		
    		EventManager.instance.AddListener<TouchStartEvent>(OnTouchStartEvent);
    		EventManager.instance.AddListener<TouchEndEvent>(OnTouchEndEvent);
    	}
    
    	public void RemoveTouchListener()
    	{
    		_IsPress = false;
    		EndDrag();
    		EventManager.instance.RemoveListener<TouchStartEvent>(OnTouchStartEvent);
    		EventManager.instance.RemoveListener<TouchEndEvent>(OnTouchEndEvent);
    	}

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            CurrentPanel = t.GetComponent<UIPanel>();
            MoveNavObject = t.FindEx("Nav");
            EntiretyObject = t;

            BGSprites = new UISprite[1];
            BGSprites[0] = t.GetComponent<UISprite>("DiBG");

            DirectionNavSprites = new UISprite[1];
            DirectionNavSprites[0] = t.GetComponent<UISprite>("Nav/DiBG");

            RadiusSize = 260;
            OffsetLength = 160f;
            MoveSpeed = 1f;
            SelectionLogic.IsShowJoystick = false;

    		_OriginalPos = EntiretyObject.localPosition;
    		_CenterPos = UICamera.mainCamera.WorldToScreenPoint(MoveNavObject.position);
    		InitAlpha(); 
    	}

        public override void OnDestroy()
    	{
            SelectionLogic.IsShowJoystick = false;
            //if (Instance != null)
            //	Instance = null;
        }
    
    	private void OnTouchStartEvent(TouchStartEvent evt)
    	{
    		if (AllianceUtil.IsInTransferDart)
    		{
    			return;
    		}

			if (!HasRegisterUpdater) { RegisterMonoUpdater(); HasRegisterUpdater = true; }

			StartDrag(evt.screenPosition, evt.deltaPosition);

            if (_PlayerCtrl != null)
            {
                _PlayerCtrl.HideMoveReticle();
            }
			
			//StartDrag(TouchController.Instance.ActiveTouches[0].position, TouchController.Instance.ActiveTouches[0].deltaPosition);
		}
    
    	private void OnTouchEndEvent(TouchEndEvent evt)
    	{
    		EndDrag();
			if (HasRegisterUpdater) { ErasureMonoUpdater(); HasRegisterUpdater = false; }
		}

        public void Update()
    	{
    		if (_IsPress)
    		{
    			Vector2 touch_pos = UICamera.lastEventPosition;
    			float distance = Vector2.Distance(touch_pos, _CenterPos);

    			if (distance < RadiusSize)
    			{
    				MoveNavObject.localPosition = (touch_pos - _CenterPos).normalized * distance;
    			}
    			else
    			{
    				MoveNavObject.localPosition = (touch_pos - _CenterPos).normalized * RadiusSize;
    			}
    
    			float x = MoveNavObject.localPosition.x;
    			float z = MoveNavObject.localPosition.y;
    
    			if (_PlayerCtrl == null)
    			{
    				EndDrag();
    				return;
    			}

    			//_PlayerCtrl.OnJoystickAxis(new JoystickAxisEvent(x, z));
                //Hotfix_LT.Messenger.Raise("JoystickAxisEvent", x, z);
                _PlayerCtrl.OnJoystickAxis(x, z);
            }
    	}
    
    	public void StartDrag(Vector3 screenPosition,Vector3 deltaPosition)
    	{
    		_PlayerCtrl = _PlayerCtrl ?? PlayerManager.LocalPlayerController().transform .GetMonoILRComponent<Player.PlayerHotfixController>();

    		if (_PlayerCtrl == null)
    		{
    			EB.Debug.LogError("joyControllerLogic playerCtrl = null");
    			return;
    		}
            //_PlayerCtrl.GetComponent<SelectionLogic>().DisablePlayerSelectionControls();

            SelectionLogic.IsShowJoystick = true;

            /*for (int i = 0; i < BGSprites.Length; ++i)
    		{
    			BGSprites[i].alpha = 0.9f;
    		}
    		for (int i = 0; i < DirectionNavSprites.Length; ++i)
    		{
    			DirectionNavSprites[i].alpha = 1f;
    		}*/
            if (EntiretyObject == null) return;
            EntiretyObject.position = UICamera.mainCamera.ScreenToWorldPoint(screenPosition);
    		EntiretyObject.localPosition -= deltaPosition.normalized * (OffsetLength);
    		_CenterPos = UICamera.mainCamera.WorldToScreenPoint(MoveNavObject.position);
    		_IsPress = true;
    	}
    
    	public void EndDrag()
    	{
    		_IsPress = false;
    		_PlayerCtrl = null;
            if (EntiretyObject == null) return;
    		EntiretyObject.localPosition = _OriginalPos;
    		MoveNavObject.localPosition = Vector3.zero;
    		if(UICamera.mainCamera!=null)
    			_CenterPos = UICamera.mainCamera.WorldToScreenPoint(MoveNavObject.position);
    		InitAlpha();
    	}
    
    	private void InitAlpha()
        {
            SelectionLogic.IsShowJoystick = false;
            /*for (int i = 0; i < BGSprites.Length; ++i)
    		{
    			BGSprites[i].alpha = 0.0f;
    		}
    		for (int i = 0; i < DirectionNavSprites.Length; ++i)
    		{
    			DirectionNavSprites[i].alpha = 0.0f;
    		}*/
    	}
    
    	public bool InTouched()
    	{
    		return mDMono.transform.GetComponent<BoxCollider>().bounds.Contains(UICamera.lastWorldPosition);
    	}
    
    	//public static bool IsTouchEnable()
    	//{
    	//	if (Instance != null)
    	//	{
    	//		LTMainMenuHudController mainMenu = Instance.gameObject.GetComponentInParent<LTMainMenuHudController>();
    	//		if (mainMenu == null)
    	//		{
    	//			Debug.LogError("mainMenu is null");
    	//			return false;
    	//		}
    
    	//		if (!mainMenu.gameObject.activeSelf)
    	//			return false;
    	//		UIPanel panel = mainMenu.GetComponent<UIPanel>();
    	//		if (panel.alpha <= 0)
    	//			return false;
    	//		return Instance.InTouched();
    	//	}
    	//	return false;
    	//}
    }
}
