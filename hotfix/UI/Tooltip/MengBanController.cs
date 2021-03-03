using UnityEngine;
using System.Collections;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public enum eTipAnchorType
    {
        center = 0,
        up = 1,
        upright = 2,
        right = 3,
        downright = 4,
        down = 5,
        downleft = 6,
        left = 7,
        upleft = 8
    }

    public class MengBanController : UIControllerHotfix, IHotfixUpdate
    {
        public override bool ShowUIBlocker { get { return false; } }
        public override bool CanAutoBackstack() { return false; }

        public UIWidget m_Middle;
        private BoxCollider m_MiddleCollider;
        public GameObject m_Container;
        public FocusEffectController m_FocusComponent;
        public GameObject m_GuideClickPrefab;
        public GameObject m_GuideClickClipPrefab;
        public UITexture m_TouchMengBanHolder; //by pj 添加区域蒙版

        private UIWidget m_TargetWidget;
        private GameObject m_TargetObject;

        [HideInInspector] public bool m_State;
        public UIWidget m_Tip;
        public UIWidget m_Arrow;
		protected UIPanel m_FingerPanel;
        public GameObject m_ClickFx;
        public GameObject m_DragFx;
        public GameObject m_Drag2Fx;
        public GameObject m_PinchFx;
        public UISprite m_Hole;
        public UILabel m_TipContext;
        public UISprite m_Forbidden;
        public int width_offset = 100;
        public int hight_offset = 100;
        public int fontsize = 60;

        public GuideNodeMonologView m_MonologView;
        public GameObject bigLogSprite;
        public GuideNodeMengbanHelper mengbanHelper;
        public UIProgressBar m_waitTimeProgress;
        public UILabel m_waitTimeLabel;

        public string FingerJudgeStr = null;
        public GameObject FingerJudgeObj = null;


        private static MengBanController s_Instance;
        public static MengBanController Instance
        {
            get { return s_Instance; }
        }

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            m_Middle = t.GetComponent<UIWidget>("Container/Middle");
            m_Container = t.FindEx("Container").gameObject;
            m_FocusComponent = t.GetMonoILRComponent<FocusEffectController>("Container/Middle/Focus");
            m_TouchMengBanHolder = t.GetComponent<UITexture>("Container/MengBanHolder");
            m_State = false;
            m_Tip = t.GetComponent<UIWidget>("Container/Tip");
            m_Arrow = t.GetComponent<UIWidget>("Container/Arrow");
			m_FingerPanel = m_Arrow.GetComponentInChildren<UIPanel>();
			m_ClickFx = t.FindEx("Container/Arrow/OldFxObj").gameObject;
            m_DragFx = t.FindEx("Container/Arrow/OldFxObj").gameObject;
            m_Drag2Fx = t.FindEx("Container/Arrow/OldFxObj").gameObject;
            m_PinchFx = t.FindEx("PinchFx").gameObject;
            m_Hole = t.GetComponent<UISprite>("Container/Middle/Hole");
            m_TipContext = t.GetComponent<UILabel>("Container/Tip/Tips");
            m_Forbidden = t.GetComponent<UISprite>("Forbidden");
            width_offset = 160;
            hight_offset = 160;
            fontsize = 60;
            m_MonologView = t.GetMonoILRComponent<GuideNodeMonologView>("Monolog");
            bigLogSprite = t.FindEx("BigLogSprite").gameObject;
            mengbanHelper = t.GetMonoILRComponent<GuideNodeMengbanHelper>();
            m_waitTimeProgress = t.GetComponent<UIProgressBar>("ProgressHolder/WaitProgressBar");
            m_waitTimeLabel = t.GetComponent<UILabel>("ProgressHolder/WaitProgressBar/Label");

            var focusEffectController = t.GetMonoILRComponent<FocusEffectController>("Container/Middle/Focus");
            t.GetComponent<UIButton>("Container/up").onClick.Add(new EventDelegate(focusEffectController.OnScreenClick));
            t.GetComponent<UIButton>("Container/down").onClick.Add(new EventDelegate(focusEffectController.OnScreenClick));
            t.GetComponent<UIButton>("Container/left").onClick.Add(new EventDelegate(focusEffectController.OnScreenClick));
            t.GetComponent<UIButton>("Container/right").onClick.Add(new EventDelegate(focusEffectController.OnScreenClick));

            s_Instance = this;
            t.gameObject.CustomSetActive(false);
            m_State = false;
            Init();
            m_MiddleCollider = m_Middle.GetComponent<BoxCollider>();
        }

        void Init()
        {
            UnFobiddenAll();
            DisappearDragFx();
            DisappearDrag2Fx();
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

		public override void OnDestroy()
        {
            s_Instance = null;
            base.OnDestroy();
        }

        public override void Show(bool isShowing)
        {
            if (!isShowing)
            {
                controller.transform.GetComponent<UIPanel>().alpha = 0;
                return;
            }
            else
            {
                controller.transform.GetComponent<UIPanel>().alpha = 1;
            }
        }

        public override void OnFocus()
        {
            string focusName = CommonConditionParse.FocusViewName; //为了不干扰焦点
            CommonConditionParse.SetFocusViewName(controller.gameObject.name.Replace("(Clone)", ""));
            CommonConditionParse.FocusViewName = focusName;
            EB.Debug.Log("FocusViewName <color=#800000ff>{0}</color>", CommonConditionParse.FocusViewName);
        }

        public void Update()
        {
            UpdatePos();
        }

        private int framelazytoUnFrobidden = 0;
        private float NpcUIExit_timer = 10;
        void UpdatePos()
        {
            //if (CommonConditionParse.FocusViewName == "LTMainMenu" && m_Middle.transform.localPosition.x > (float)UIRoot.list[0].manualWidth / 2f)//主城场景下，判断手指是否在屏幕外
            //{
            //    if (NpcUIExit_timer <= 2)
            //    {
            //        NpcUIExit_timer += EB.Time.deltaTime;
            //    }
            //    else
            //    {
            //        NpcUIExit_timer = 0;
            //        if (PlayerController.onCollisionExit != null)
            //        {
            //            PlayerController.CurNpcCollision = null;
            //            PlayerController.onCollisionExit(GuideNodeManager.GUIDE_FUNCTION_OPEN);
            //        }
            //    }
            //}

            if (!m_State) return;
            if (m_TargetWidget == null && m_TargetObject == null)
            {
                return;
            }

            if (isFobidden && NeedUnFobiddenAll)
            {
                if (framelazytoUnFrobidden <= 0)
                {
                    framelazytoUnFrobidden++;
                }
                else
                {
                    framelazytoUnFrobidden = 0;
                    NeedUnFobiddenAll = false;
                    UnFobiddenAll();
                }
            }

            if (m_TargetObject != null)
            {
                if (m_TargetObject.activeInHierarchy)
                {
                    Vector3 position = m_TargetObject.transform.TransformPoint(Vector3.zero);
                    Vector3 screen_point = Camera.main.WorldToScreenPoint(position);

                    float screenHeight = Screen.height;
                    float screenWidth = Screen.width;
                    float screenScale = 1.0f;
                    if (UIRoot.list[0].fitHeight)
                    {
                        screenScale = (float)UIRoot.list[0].manualHeight / (float)screenHeight;
                    }
                    else
                    {
                        screenScale = (float)UIRoot.list[0].manualWidth / (float)screenWidth;
                    }

                    screen_point.x -= screenWidth / 2;
                    screen_point.y -= screenHeight / 2;
                    screen_point *= screenScale;
                    screen_point.z = 0;
                    m_Middle.transform.localPosition = screen_point;
                    m_Middle.pivot = UIWidget.Pivot.Center;
                    m_Middle.width = 200;
                    m_Middle.height = 200;
                    m_Middle.gameObject.CustomSetActive(true);
                    m_Container.CustomSetActive(true);
                }
                else
                {
                    m_Container.CustomSetActive(false);
                }
            }
            else
            {
                m_Middle.gameObject.CustomSetActive(true);
                m_Middle.transform.position = m_TargetWidget.transform.position;
                m_Middle.pivot = m_TargetWidget.pivot;
                m_Middle.width = m_TargetWidget.width;
                m_Middle.height = m_TargetWidget.height;
                m_MiddleCollider.size = new Vector3(m_Middle.width, m_Middle.height, 0);
                m_MiddleCollider.center = m_Middle.localCenter;


                m_Arrow.gameObject.CustomSetActive(true);
                m_Container.CustomSetActive(true);
            }

            if (FingerJudgeStr != null)//
            {
                if (FingerJudgeObj == null)
                {
                    FingerJudgeObj = m_TargetWidget.transform.Find(FingerJudgeStr).gameObject;
                }

                if (FingerJudgeObj == null || !FingerJudgeObj.activeInHierarchy)
                {
                    if (GuideNodeManager.ExecuteJump != null)
                    {
                        FingerJudgeStr = null;
                        GuideNodeManager.ExecuteJump(NodeMessageManager.ForbidInstanceFloor);
                        GuideNodeManager.ExecuteJump(NodeMessageManager.ForbidOther);
                    }
                }
            }

        }

		private Vector3 oriPos;
		private Vector3 endPos;
		private Quaternion oriRot;
		private Quaternion endRot;
		private bool isFingerModified;

		private void RecordFinger()
		{
			Transform original = m_FingerPanel.transform.Find("Container");
			Transform end = m_FingerPanel.transform.Find("Container1");
			oriPos = original.localPosition;
			endPos = end.localPosition;
			oriRot = original.localRotation;
			endRot = end.localRotation;
		}

		public void SetFinger(string order)
		{
			if(m_FingerPanel == null || string.IsNullOrEmpty(order))
			{
				return;
			}

			RecordFinger();

			string[] contents = order.Split('|');
			for (int i = 0; i < contents.Length; i++)
			{
				if (contents[i].StartsWith("OriginalPos"))
				{
					Transform original = m_FingerPanel.transform.Find("Container");
					string posText = contents[i].Replace("OriginalPos=", string.Empty);
					Vector3 pos = StringUtils.Vector3Parse(posText, ';');
					original.localPosition = pos;
				}
				else if (contents[i].StartsWith("EndPos"))
				{
					Transform end = m_FingerPanel.transform.Find("Container1");
					string posText = contents[i].Replace("EndPos=", string.Empty);
					Vector3 pos = StringUtils.Vector3Parse(posText, ';');
					end.localPosition = pos;
				}
				else if (contents[i].StartsWith("OriginalAngles"))
				{
					Transform original = m_FingerPanel.transform.Find("Container");
					string eulerText = contents[i].Replace("OriginalAngles=", string.Empty);
					original.localRotation = Quaternion.Euler(StringUtils.Vector3Parse(eulerText, ';'));
				}
				else if (contents[i].StartsWith("EndAngles"))
				{
					Transform end = m_FingerPanel.transform.Find("Container1");
					string eulerText = contents[i].Replace("EndAngles=", string.Empty);
					end.localRotation = Quaternion.Euler(StringUtils.Vector3Parse(eulerText, ';'));
				}
				else if(contents[i].StartsWith("DragArrow"))
				{
					Transform arrow = m_FingerPanel.transform.Find("DragArrow");
					if (arrow) arrow.gameObject.SetActive(true);

					if(contents[i].StartsWith("DragArrowPos"))
					{
						string posText = contents[i].Replace("DragArrowPos=", string.Empty);
						Vector3 pos = StringUtils.Vector3Parse(posText, ';');
						arrow.localPosition = pos;

						SpriteFingerAnim.ArrowPoint = pos;
					}
					else if(contents[i].StartsWith("DragArrowAngles"))
					{
						string eulerText = contents[i].Replace("DragArrowAngles=", string.Empty);
						arrow.localRotation = Quaternion.Euler(StringUtils.Vector3Parse(eulerText, ';'));
					}
				}
			}
			isFingerModified = true;
		}

		public void RestoreFinger()
		{
			if (m_FingerPanel == null || !isFingerModified)
			{
				return;
			}

			Transform original = m_FingerPanel.transform.Find("Container");
			Transform end = m_FingerPanel.transform.Find("Container1");
			original.localPosition = oriPos;
			end.localPosition = endPos;
			original.localRotation = oriRot;
			end.localRotation = endRot;

			Transform arrow = m_FingerPanel.transform.Find("DragArrow");
			if (arrow) arrow.gameObject.SetActive(false);

			 isFingerModified = false;
		}

        private string _logoHei = "[000000]";
        private string _logoEnd = "[-]";
        public void SetMonolog(float x, float y, int width, string content)
        {
            if (m_MonologView == null)
            {
                return;
            }
            m_MonologView.mDMono.gameObject.CustomSetActive(true);
            m_MonologView.SetLabel(_logoHei + content + _logoEnd, width);
            float bigIconX = x;


            m_MonologView.mDMono.transform.localPosition = new Vector3(x, y, m_MonologView.mDMono.transform.localPosition.z);
        }
        public void SetLogIcon(bool isShow)
        {
            m_MonologView.SetIcon(isShow);

            bigLogSprite.CustomSetActive(!isShow);
        }

        public void SetBigIcon(float x)
        {
            if (x > -300) //居中计算 所以大于0就是在右边
            {
                m_MonologView.bgSpt.flip = UIBasicSprite.Flip.Horizontally;
            }
            else
            {
                m_MonologView.bgSpt.flip = UIBasicSprite.Flip.Nothing;
            }
            bigLogSprite.transform.localPosition = new Vector3(x, bigLogSprite.transform.localPosition.y, bigLogSprite.transform.localPosition.z);
        }

        private bool NeedUnFobiddenAll = false;
        
        public void SetMiddle(UIWidget widget, int tipAnchor, string tip)
        {
            //UnFobiddenAll();
            NeedUnFobiddenAll = true;
            m_Arrow.gameObject.CustomSetActive(false);

            m_Middle.pivot = UIWidget.Pivot.Center;
            m_Middle.width = 200;
            m_Middle.height = 200;//先初始化

            controller.Open();
            m_State = true;
            m_TargetWidget = widget;
            m_TargetObject = null;
            Tip((eTipAnchorType)tipAnchor, tip);
            CheckHideMengBan();
            //为了防止蒙版等需要关闭焦点后再次打开其他方式没有焦点提示
            m_FocusComponent.mDMono.gameObject.CustomSetActive(true);
            m_TouchMengBanHolder.gameObject.CustomSetActive(false);

            //新方式
            UpEvent(m_Middle, widget);
        }

        public void RemoveEvent(UIWidget middle)
        {
            UIEventTrigger middleET = middle.GetComponent<UIEventTrigger>();

            if (middleET != null)
            {
                middleET.onClick.Clear();
                middleET.onDrag.Clear();
                middleET.onDragOut.Clear();
                middleET.onDragOver.Clear();
                middleET.onHoverOut.Clear();
                middleET.onHoverOver.Clear();
                middleET.onPress.Clear();
                middleET.onRelease.Clear();
                middleET.onDeselect.Clear();
                middleET.onDoubleClick.Clear();
                middleET.onDragStart.Clear();
                middleET.onDragEnd.Clear();
            }

            UIEventListener middleListener = middle.GetComponent<UIEventListener>();

            if (middleListener != null)
            {
                middleListener.onDrag = null;
                middleListener.onClick = null;
                middleListener.onDoubleClick = null;
                middleListener.onDragEnd = null;
                middleListener.onDragOut = null;
                middleListener.onDragOver = null;
                middleListener.onDragStart = null;
                middleListener.onDrop = null;
                middleListener.onHover = null;
                middleListener.onKey = null;
                middleListener.onPress = null;
                middleListener.onScroll = null;
                middleListener.onSelect = null;
                middleListener.onSubmit = null;
            }
        }
        public void UpEvent(UIWidget middle, UIWidget target)
        {
            if (middle == null || target == null)
            {
                return;
            }

            RemoveEvent(middle);

            UIEventTrigger middleET = middle.GetComponent<UIEventTrigger>();
            UIEventListener middleListener = middle.GetComponent<UIEventListener>();

            bool isFind = false;

            if (middleET != null && target.GetComponent<UIButton>() != null)
            {
                UIButton widgetBtn = target.GetComponent<UIButton>();

                middleET.onClick.Add(new EventDelegate(() =>
                {
                    if (widgetBtn.OnClickAction != null)
                    {
                        widgetBtn.OnClickAction();
                    }
                }));
                middleET.onDragOut.Add(new EventDelegate(() =>
                {
                    if (widgetBtn.OnDragOutAction != null)
                    {
                        widgetBtn.OnDragOutAction();
                    }
                }));
                middleET.onDragOver.Add(new EventDelegate(() =>
                {
                    if (widgetBtn.OnDragOverAction != null)
                    {
                        widgetBtn.OnDragOverAction();
                    }
                }));
                middleET.onPress.Add(new EventDelegate(() =>
                {
                    if (widgetBtn.OnPressAction != null)
                    {
                        widgetBtn.OnPressAction(true);
                    }
                }));
                middleET.onRelease.Add(new EventDelegate(() =>
                {
                    if (widgetBtn.OnPressAction != null)
                    {
                        widgetBtn.OnPressAction(false);
                    }
                }));
                isFind = true;
            }

            if (middleET != null && target.GetComponent<UIEventTrigger>() != null)
            {
                UIEventTrigger targetET = target.GetComponent<UIEventTrigger>();
                if (targetET.onClick != null) middleET.onClick.AddRange(targetET.onClick);
                if (targetET.onDrag != null) middleET.onDrag.AddRange(targetET.onDrag);
                if (targetET.onDragOut != null) middleET.onDragOut.AddRange(targetET.onDragOut);
                if (targetET.onDragOver != null) middleET.onDragOver.AddRange(targetET.onDragOver);
                if (targetET.onHoverOut != null) middleET.onHoverOut.AddRange(targetET.onHoverOut);
                if (targetET.onHoverOver != null) middleET.onHoverOver.AddRange(targetET.onHoverOver);
                if (targetET.onPress != null) middleET.onPress.AddRange(targetET.onPress);
                if (targetET.onRelease != null) middleET.onRelease.AddRange(targetET.onRelease);
                if (targetET.onDeselect != null) middleET.onDeselect.AddRange(targetET.onDeselect);
                if (targetET.onDoubleClick != null) middleET.onDoubleClick.AddRange(targetET.onDoubleClick);
                if (targetET.onDragStart != null) middleET.onDragStart.AddRange(targetET.onDragStart);
                if (targetET.onDragEnd != null) middleET.onDragEnd.AddRange(targetET.onDragEnd);

                isFind = true;

            }

            if (middleListener != null && target.GetComponent<UIEventListener>() != null)
            {
                UIEventListener targetListener = target.GetComponent<UIEventListener>();
                if (targetListener.onDrag != null) middleListener.onDrag = new UIEventListener.VectorDelegate((GameObject go, Vector2 d) => { targetListener.onDrag(go, d); });
                if (targetListener.onClick != null) middleListener.onClick = new UIEventListener.VoidDelegate((GameObject go) => { targetListener.onClick(go); });
                if (targetListener.onDoubleClick != null) middleListener.onDoubleClick = new UIEventListener.VoidDelegate((GameObject go) => { targetListener.onDoubleClick(go); });
                if (targetListener.onDragEnd != null) middleListener.onDragEnd = new UIEventListener.VoidDelegate((GameObject go) => { targetListener.onDragEnd(go); });
                if (targetListener.onDragOut != null) middleListener.onDragOut = new UIEventListener.VoidDelegate((GameObject go) => { targetListener.onDragOut(go); });
                if (targetListener.onDragOver != null) middleListener.onDragOver = new UIEventListener.VoidDelegate((GameObject go) => { targetListener.onDragOver(go); });
                if (targetListener.onDragStart != null) middleListener.onDragStart = new UIEventListener.VoidDelegate((GameObject go) => { targetListener.onDragStart(go); });
                if (targetListener.onDrop != null) middleListener.onDrop = new UIEventListener.ObjectDelegate((GameObject go, GameObject obj) => { targetListener.onDrop(go, obj); });
                if (targetListener.onHover != null) middleListener.onHover = new UIEventListener.BoolDelegate((GameObject go, bool obj) => { targetListener.onHover(go, obj); });
                if (targetListener.onKey != null) middleListener.onKey = new UIEventListener.KeyCodeDelegate((GameObject go, KeyCode obj) => { targetListener.onKey(go, obj); });
                if (targetListener.onPress != null) middleListener.onPress = new UIEventListener.BoolDelegate((GameObject go, bool obj) => { targetListener.onPress(go, obj); });
                if (targetListener.onScroll != null) middleListener.onScroll = new UIEventListener.FloatDelegate((GameObject go, float obj) => { targetListener.onScroll(go, obj); });
                if (targetListener.onSelect != null) middleListener.onSelect = new UIEventListener.BoolDelegate((GameObject go, bool obj) => { targetListener.onSelect(go, obj); });
                if (targetListener.onSubmit != null) middleListener.onSubmit = new UIEventListener.VoidDelegate((GameObject go) => { targetListener.onSubmit(go); });
                isFind = true;
            }

            if (middleET != null && target.GetComponent<ConsecutiveClickCoolTrigger>() != null)
            {
                ConsecutiveClickCoolTrigger ccct = target.GetComponent<ConsecutiveClickCoolTrigger>();

                if (ccct.enabled)
                {
                    middleET.onClick.Add(new EventDelegate(() => { if (ccct != null) ccct.OnClick(); }));
                }

                isFind = true;
            }

            if (middleET != null && target.GetComponent<ContinueClickCDTrigger>() != null)
            {
                ContinueClickCDTrigger cdCom = target.GetComponent<ContinueClickCDTrigger>();

                if (cdCom.enabled)
                {
                    middleET.onClick.Add(new EventDelegate(() =>
                    {
                        if (cdCom != null)
                        {
                            cdCom.OnPress(true);
                        }
                    }));
                }

                isFind = true;
            }

            if (isFind)
            {
                middle.gameObject.GetComponent<Collider>().enabled = true;
            }
        }
        
        public void SetMiddle(Vector3 localpos, Vector2 size, int tipAnchor, string tip, bool isTouchMengBan = false)
        {
            //UnFobiddenAll();
            m_Container.gameObject.CustomSetActive(true);
            if (isTouchMengBan)
            {
                m_Arrow.gameObject.CustomSetActive(false);
                m_TouchMengBanHolder.gameObject.CustomSetActive(true);
                m_FocusComponent.mDMono.gameObject.CustomSetActive(false);
                mengbanHelper.Mix((RenderTexture) => { m_TouchMengBanHolder.SetTexture(RenderTexture); });

            }
            else
            {
                m_TouchMengBanHolder.gameObject.CustomSetActive(false);
                //为了防止蒙版等需要关闭焦点后再次打开其他方式没有焦点提示
                m_FocusComponent.mDMono.gameObject.CustomSetActive(true);
            }
            NeedUnFobiddenAll = true;
            controller.Open();
            m_State = true;
            m_TargetWidget = null;
            m_TargetObject = null;
            m_Middle.gameObject.CustomSetActive(true);
            m_Middle.transform.localPosition = localpos;
            m_Middle.width = (int)size.x;
            m_Middle.height = (int)size.y;
            m_MiddleCollider.size = new Vector3(m_Middle.width, m_Middle.height, 0);
            m_Middle.pivot = UIWidget.Pivot.Center;
            Tip((eTipAnchorType)tipAnchor, tip);

            CheckHideMengBan();
        }

        public void AddCollider(EventDelegate item)
        {
            m_Middle.gameObject.GetComponent<Collider>().enabled = true;

            if (item != null && m_Middle.GetComponent<UIEventTrigger>() != null)
            {
                m_Middle.GetComponent<UIEventTrigger>().onClick.Add(item);
            }

            for (var i = 0; i < m_TouchMengBanHolder.transform.childCount; i++)
            {
                Transform v = m_TouchMengBanHolder.transform.GetChild(i);
                v.gameObject.GetComponent<Collider>().enabled = true;
                if (item != null && v.gameObject.GetComponent<UIEventTrigger>() != null)
                {
                    v.gameObject.GetComponent<UIEventTrigger>().onClick.Add(item);
                }
            }
        }
        public void RemoveCollider(EventDelegate item)
        {
            m_Middle.gameObject.GetComponent<Collider>().enabled = false;
            UIEventTrigger et = m_Middle.GetComponent<UIEventTrigger>();
            if (et != null)
            {
                if (et.onClick.Contains(item))
                {
                    et.onClick.Remove(item);
                }
            }

            for (var i = 0; i < m_TouchMengBanHolder.transform.childCount; i++)
            {
                Transform v = m_TouchMengBanHolder.transform.GetChild(i);
                v.gameObject.GetComponent<Collider>().enabled = false;
                if (v.gameObject.GetComponent<UIEventTrigger>() != null)
                {
                    v.gameObject.GetComponent<UIEventTrigger>().onClick.Remove(item);
                }
            }
        }
        public void RemoveCollider()
        {
            RemoveEvent(m_Middle);
            m_Middle.gameObject.GetComponent<Collider>().enabled = false;

            for (var i = 0; i < m_TouchMengBanHolder.transform.childCount; i++)
            {
                Transform v = m_TouchMengBanHolder.transform.GetChild(i);
                v.gameObject.GetComponent<Collider>().enabled = false;
                if (v.gameObject.GetComponent<UIEventTrigger>() != null)
                {
                    v.gameObject.GetComponent<UIEventTrigger>().onClick.Clear();
                }
            }
        }
        
        private void Tip(eTipAnchorType tipAnchor, string tip)
        {
            int width = 0;
            int hight = 0;
            if (!string.IsNullOrEmpty(tip))
            {
                m_Tip.gameObject.CustomSetActive(true);
                m_TipContext.text = tip;
                width = m_TipContext.width + 120;
                hight = m_TipContext.height + 50;
            }
            else
            {
                m_Tip.gameObject.CustomSetActive(false);
            }

            if (tipAnchor == eTipAnchorType.center)
            {
                m_Arrow.leftAnchor.relative = 0.5f;
                m_Arrow.leftAnchor.absolute = 0;

                m_Arrow.rightAnchor.relative = 0.5f;
                m_Arrow.rightAnchor.absolute = 0;

                m_Arrow.bottomAnchor.relative = 0.5f;
                m_Arrow.bottomAnchor.absolute = 0;

                m_Arrow.topAnchor.relative = 0.5f;
                m_Arrow.topAnchor.absolute = 0;
                //m_Hole.gameObject.CustomSetActive(false);
                return;
            }

            m_Hole.gameObject.CustomSetActive(false);
            //m_Hole.gameObject.CustomSetActive(true);
            if (tipAnchor == eTipAnchorType.up || tipAnchor == eTipAnchorType.upright || tipAnchor == eTipAnchorType.upleft)
            {

                m_Tip.bottomAnchor.relative = 1;


                m_Tip.topAnchor.relative = 1;


                if (tipAnchor == eTipAnchorType.up)
                {
                    if (!string.IsNullOrEmpty(tip))
                    {
                        m_Tip.bottomAnchor.absolute = hight_offset;
                        m_Tip.topAnchor.absolute = (hight + hight_offset);

                        m_Tip.leftAnchor.relative = 0.5f;
                        m_Tip.leftAnchor.absolute = -1 * (width / 2);

                        m_Tip.rightAnchor.relative = 0.5f;
                        m_Tip.rightAnchor.absolute = width / 2;
                    }
                }
                else if (tipAnchor == eTipAnchorType.upright)
                {
                    if (!string.IsNullOrEmpty(tip))
                    {
                        m_Tip.bottomAnchor.absolute = (int)(0.7 * hight_offset);
                        m_Tip.topAnchor.absolute = (int)(hight + 0.7 * hight_offset);

                        m_Tip.leftAnchor.relative = 1f;
                        m_Tip.leftAnchor.absolute = (int)(0.7 * width_offset);

                        m_Tip.rightAnchor.relative = 1f;
                        m_Tip.rightAnchor.absolute = (int)(width + 0.7 * width_offset);
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(tip))
                    {
                        m_Tip.bottomAnchor.absolute = (int)(0.7 * hight_offset);
                        m_Tip.topAnchor.absolute = (int)(hight + 0.7 * hight_offset);

                        m_Tip.leftAnchor.relative = 0f;
                        m_Tip.leftAnchor.absolute = -1 * (int)(width + 0.7 * width_offset);

                        m_Tip.rightAnchor.relative = 0f;
                        m_Tip.rightAnchor.absolute = -1 * (int)(0.7 * width_offset);
                    }
                }

            }
            else if (tipAnchor == eTipAnchorType.down || tipAnchor == eTipAnchorType.downright || tipAnchor == eTipAnchorType.downleft)
            {
                m_Tip.bottomAnchor.relative = 0;
                m_Tip.topAnchor.relative = 0;


                if (tipAnchor == eTipAnchorType.down)
                {
                    if (!string.IsNullOrEmpty(tip))
                    {
                        m_Tip.bottomAnchor.absolute = -1 * (hight + hight_offset);
                        m_Tip.topAnchor.absolute = -1 * hight_offset;

                        m_Tip.leftAnchor.relative = 0.5f;
                        m_Tip.leftAnchor.absolute = -1 * (width / 2);

                        m_Tip.rightAnchor.relative = 0.5f;
                        m_Tip.rightAnchor.absolute = width / 2;
                    }
                }
                else if (tipAnchor == eTipAnchorType.downright)
                {
                    if (!string.IsNullOrEmpty(tip))
                    {
                        m_Tip.bottomAnchor.absolute = -1 * (int)(hight + 0.7 * hight_offset);
                        m_Tip.topAnchor.absolute = -1 * (int)(0.7 * hight_offset);

                        m_Tip.leftAnchor.relative = 1f;
                        m_Tip.leftAnchor.absolute = (int)(0.7 * width_offset);

                        m_Tip.rightAnchor.relative = 1f;
                        m_Tip.rightAnchor.absolute = (int)(width + 0.7 * width_offset);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(tip))
                    {
                        m_Tip.bottomAnchor.absolute = -1 * (int)(hight + 0.7 * hight_offset);
                        m_Tip.topAnchor.absolute = -1 * (int)(0.7 * hight_offset);

                        m_Tip.leftAnchor.relative = 0f;
                        m_Tip.leftAnchor.absolute = -1 * (int)(width + 0.7 * width_offset);

                        m_Tip.rightAnchor.relative = 0f;
                        m_Tip.rightAnchor.absolute = -1 * (int)(0.7 * width_offset);

                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(tip))
                {
                    m_Tip.bottomAnchor.relative = 0.5f;
                    m_Tip.bottomAnchor.absolute = -1 * (hight / 2);

                    m_Tip.topAnchor.relative = 0.5f;
                    m_Tip.topAnchor.absolute = (hight / 2);
                }

                if (tipAnchor == eTipAnchorType.left)
                {
                    if (!string.IsNullOrEmpty(tip))
                    {
                        m_Tip.leftAnchor.relative = 0f;
                        m_Tip.leftAnchor.absolute = -1 * (width + width_offset);

                        m_Tip.rightAnchor.relative = 0f;
                        m_Tip.rightAnchor.absolute = -1 * (width_offset);
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(tip))
                    {
                        m_Tip.leftAnchor.relative = 1f;
                        m_Tip.leftAnchor.absolute = (width_offset);

                        m_Tip.rightAnchor.relative = 1f;
                        m_Tip.rightAnchor.absolute = (width + width_offset);
                    }
                }
            }

            m_TipContext.text = tip;
        }

        private void GetTipBorder(string tip, out int width, out int hight)
        {
            string[] lines = tip.Split('\n');
            int lineMaxLength = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lineMaxLength < lines[i].ToCharArray().Length) lineMaxLength = lines[i].ToCharArray().Length;
            }

            width = lineMaxLength * fontsize;
            hight = lines.Length * fontsize + 50;

        }

        private int eventSequence = 0;
        public void Hide()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.4f);
            controller.Close();
            m_State = false;
            m_TargetWidget = null;
            m_TargetObject = null;
            FingerJudgeStr = null;
            FingerJudgeObj = null;
            m_Arrow.gameObject.CustomSetActive(false);
            m_FocusComponent.mDMono.gameObject.CustomSetActive(false);
            m_Container.CustomSetActive(false);
            m_Middle.gameObject.CustomSetActive(false);
            m_TouchMengBanHolder.gameObject.CustomSetActive(false);
            m_MonologView.mDMono.gameObject.CustomSetActive(false);
            bigLogSprite.CustomSetActive(false);
            NpcUIExit_timer = 10;
            AddHideMengBan();
        }

        private void CheckHideMengBan()
        {
            if (eventSequence > 0)
            {
                ILRTimerManager.instance.RemoveTimer(eventSequence);
                eventSequence = 0;
            }

            controller.gameObject.CustomSetActive(true);
        }

        private void AddHideMengBan()
        {
            if (eventSequence > 0)
            {
                ILRTimerManager.instance.RemoveTimer(eventSequence);
                eventSequence = 0;
            }

            eventSequence = ILRTimerManager.instance.AddTimer(1000, 1, HideMengBanObj);
        }

        private void HideMengBanObj(int timerSequence)
        {
            controller.gameObject.CustomSetActive(false);
            eventSequence = 0;
        }

        private bool isFobidden = false;
        public bool Fobidden
        {
            get { return isFobidden; }
            set { isFobidden = value; }
        }

        public void FobiddenAll()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 3600f);
            PlayerController.LocalPlayerDisableNavigation();
            isFobidden = true;
        }

        public void UnFobiddenAll()
        {
            InputBlockerManager.Instance.UnBlock(InputBlockReason.CONVERT_FLY_ANIM);
            PlayerController.LocalPlayerEnableNavigation();
            m_Container.gameObject.CustomSetActive(true);
            isFobidden = false;
        }

        public void FobiddenUI()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 3600f);
        }

        public void UnFobiddenUI()
        {
            InputBlockerManager.Instance.UnBlock(InputBlockReason.CONVERT_FLY_ANIM);
        }

        public void ShowClickFx()
        {
            m_ClickFx.CustomSetActive(true);
        }

        public void DisappearClickFx()
        {
            m_ClickFx.CustomSetActive(false);
        }

        public void ShowDragFx()
        {
            m_DragFx.CustomSetActive(true);
        }

        public void DisappearDragFx()
        {
            m_DragFx.CustomSetActive(false);
        }

        public void ShowDrag2Fx()
        {
            m_Drag2Fx.CustomSetActive(true);
        }

        public void DisappearDrag2Fx()
        {
            m_Drag2Fx.CustomSetActive(false);
        }

        public void ShowPinchGuide()
        {
            UnFobiddenAll();
            m_Container.CustomSetActive(false);
            UICamera mainUICamera = UICamera.mainCamera.GetComponent<UICamera>();
            bool useTouch = mainUICamera.useTouch;
            bool useMouse = mainUICamera.useMouse;
            FobiddenUI();
            isFobidden = true;

            if (useTouch)
            {
                mainUICamera.ProcessTouches();
            }

            if (useMouse)
            {
                mainUICamera.ProcessMouse();
            }

            controller.Open();
            m_PinchFx.CustomSetActive(true);
        }

        public void HidePinchGuide()
        {
            m_PinchFx.CustomSetActive(false);
            UnFobiddenUI();
            controller.Close();
        }

        public void SetProgress(float totalTime, float lessTime)
        {
            if (lessTime > 0)
            {
                m_waitTimeProgress.gameObject.CustomSetActive(true);
            }
            else
            {
                m_waitTimeProgress.gameObject.CustomSetActive(false);
            }

            float progress = (totalTime - lessTime) / totalTime;
            m_waitTimeProgress.value = progress;
            LTUIUtil.SetText(m_waitTimeLabel, ((int)(lessTime + 0.9)).ToString());
        }
    }
}
