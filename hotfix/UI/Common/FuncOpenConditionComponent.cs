using UnityEngine;
using EB.Sparx;

namespace Hotfix_LT.UI
{
    public class FuncOpenConditionComponent : DynamicMonoHotfix
    {
    	public int m_FuncId;
        public bool IsNpcFunc = false;
        public bool IsMainMenuFunc = false;

    	public UISprite m_Sprite;//icon to gray
    	public UILabel m_Label;//label to gray
    	public UILabel m_OpenTipLabel;
        public Color m_GrayColor = new Color(1, 0, 1, 1);
        public Color m_NormalColor = new Color(1, 1, 1, 1);
    
        private Hotfix_LT.Data.FuncTemplate m_FuncTpl;
        private bool IsDayLimitOpenFunc = false;

        public override void Awake()
        {
            base.Awake();

            if (mDMono.IntParamList != null)
            {
                var count = mDMono.IntParamList.Count;

                if (count > 0)
                {
                    m_FuncId = mDMono.IntParamList[0];
                }
            }

            if (mDMono.BoolParamList != null)
            {
                var count = mDMono.BoolParamList.Count;

                if (count > 0)
                {
                    IsNpcFunc = mDMono.BoolParamList[0];
                }
                if (count > 1)
                {
                    IsMainMenuFunc= mDMono.BoolParamList[1];
                }
            }

            if (mDMono.ObjectParamList != null)
            {
                var count = mDMono.ObjectParamList.Count;

                if (count > 0 && mDMono.ObjectParamList[0] != null)
                {
                    m_Sprite = ((GameObject)mDMono.ObjectParamList[0]).GetComponentEx<UISprite>();
                }
                if (count > 1 && mDMono.ObjectParamList[1] != null)
                {
                    m_Label = ((GameObject)mDMono.ObjectParamList[1]).GetComponentEx<UILabel>();
                }
                if (count > 2 && mDMono.ObjectParamList[2] != null)
                {
                    m_OpenTipLabel = ((GameObject)mDMono.ObjectParamList[2]).GetComponentEx<UILabel>();
                }
            }

    		EB.Sparx.Hub.Instance.LevelRewardsManager.OnLevelChange += OnLevelChange;
    	}

        public override void Start()
    	{
            m_FuncTpl = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(m_FuncId);
            IsDayLimitOpenFunc = m_FuncTpl.condition.Contains("d");
    
            if (m_FuncTpl == null)
            {
                EB.Debug.LogWarning("Can Not Find Func = {0}" , m_FuncId);
                return;
            }

            OnLevelChange(null);

			string iconName = m_FuncTpl.iconName;
			if (m_FuncTpl.iconName.Contains("Icons_Renwu"))
				iconName = m_FuncTpl.iconName.Replace("Renwu", "Renwu2");
			else if(m_FuncTpl.iconName.Contains("Icons_Huoban"))
				iconName = m_FuncTpl.iconName.Replace("Huoban", "Huoban2");

			m_Sprite.spriteName = iconName;
            LTUIUtil.SetText(m_Label, m_FuncTpl.display_name);
            UIButton btn = m_Sprite.GetComponent<UIButton>();

            if (btn != null)
            {
                btn.normalSprite = iconName;

                if (IsNpcFunc)
                {
                    btn.onClick.Add(new EventDelegate(OnNpcFuncBtnClick));
                }
                else
                {
                    if (btn.onClick.Count == 0)
                    {
                        btn.onClick.Add(new EventDelegate(OnBtnClick));
                    }
                }
            }
    
            DoCondition();

            if (IsDayLimitOpenFunc)
            {
                Messenger.AddListener(Hotfix_LT.EventName.FuncOpenBtnReflash,OnReflash);
            }
        }

        public override void OnDestroy()
    	{
    		EB.Sparx.Hub.Instance.LevelRewardsManager.OnLevelChange -= OnLevelChange;

            if (IsDayLimitOpenFunc)
            {
                Messenger.RemoveListener(Hotfix_LT.EventName.FuncOpenBtnReflash,OnReflash);
            }
        }
    
        private void OnReflash()
        {
            if (IsDayLimitOpenFunc)
            {
                OnLevelChange(null);
            }
        }
    
    	private void OnLevelChange(LevelRewardsStatus status)
    	{
            var func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(m_FuncId);

            if (func == null)
    		{
    			return;
    		}

    		DoCondition();
        }
    
        protected virtual void DoCondition()
    	{
            bool state = m_FuncTpl.IsConditionOK();
            UIButton collider = m_Sprite.GetComponent<UIButton>();

            if (collider != null)
    		{
    			collider.enabled = state;
    		}
            
    		Color color= m_NormalColor;

    		if (!state)
    		{
    			color = m_GrayColor;

                if(mDMono.GetComponent<ConsecutiveClickCoolTrigger>() == null)
                {
                    ConsecutiveClickCoolTrigger CoolBtn = mDMono.gameObject.AddComponent<ConsecutiveClickCoolTrigger>();
                    CoolBtn.clickEvent = new System.Collections.Generic.List<EventDelegate>();
                    CoolBtn.clickEvent.Add(new EventDelegate(OnDisOpenBtnClick));
                }

                if (IsDayLimitOpenFunc)
                {
                    m_OpenTipLabel.gameObject.CustomSetActive(IsDayLimitOpenFunc);
                    m_OpenTipLabel.text = m_FuncTpl.GetConditionStr();
                    m_Label.gameObject.CustomSetActive(!IsDayLimitOpenFunc);
                }
            }
            else
            {
                if (IsDayLimitOpenFunc)
                {
                    m_OpenTipLabel.gameObject.CustomSetActive(false);
                    m_Label.gameObject.CustomSetActive(true);
                }

                ConsecutiveClickCoolTrigger CoolBtn = mDMono.transform.GetComponent<ConsecutiveClickCoolTrigger>();

                if (CoolBtn != null)
                {
                    Object.Destroy(CoolBtn);
                }
            }
    
            m_Sprite.color = color;
			string _objName = UIControllerHotfix.Current.controller.gameObject.name;
			if (IsMainMenuFunc) return;

    		UISprite[] sprites = m_Sprite.GetComponentsInChildren<UISprite>(true);

    		if (sprites != null)
    		{
    			for (int i = 0; i < sprites.Length; i++)
    			{
    				sprites[i].color = color;
    			}
    		}
        }
    
        public void OnDisOpenBtnClick()
        {
            
            if (IsDayLimitOpenFunc)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_FUNC_OPEN_CONDITION"), m_FuncTpl.display_name,m_FuncTpl.GetConditionStr()));
            }
            else
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, m_FuncTpl.GetConditionStr());
            }
        }
        
        public void OnBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");

            if (m_FuncTpl == null)
            {
                return;
            }

            if (AllianceUtil.GetIsInTransferDart("")) return;

    		GlobalMenuManager.Instance.Open(m_FuncTpl.ui_model, m_FuncTpl.parameter);

            if (m_FuncTpl.ui_model!=null && m_FuncTpl.ui_model.Equals("LTResourceInstanceUI"))
            {
                if (m_FuncTpl.parameter == null) return;
                if(m_FuncTpl.parameter.Equals("Exp"))
                {
                                 FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.exp_camp_topic,
                        FusionTelemetry.GamePlayData.exp_camp_event_id, FusionTelemetry.GamePlayData.exp_camp_umengId, "open");
                }
                else if(m_FuncTpl.parameter.Equals("Gold"))
                {
                    FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.gold_camp_topic,
           FusionTelemetry.GamePlayData.gold_camp_event_id, FusionTelemetry.GamePlayData.gold_camp_umengId, "open");
                }
            }else if(m_FuncTpl.ui_model!=null && m_FuncTpl.ui_model.Equals("LTAlienMazeHud"))
            {
                FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.puzzle_camp_topic,
                FusionTelemetry.GamePlayData.puzzle_camp_event_id, FusionTelemetry.GamePlayData.puzzle_camp_umengId, "open");
            }
        }
    
        public void OnNpcFuncBtnClick()
        {
    		if (AllianceUtil.IsInTransferDart)
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_FuncOpenConditionComponent_4248"));
    			return;
    		}

            if (m_FuncTpl == null)
            {
                return;
            }
    
            var encounterTpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetMainLandEncounter(m_FuncId);
            WorldMapPathManager.Instance.StartPathFindToNpcFly(MainLandLogic.GetInstance().CurrentSceneName, encounterTpl.mainland_name, encounterTpl.locator);
        }
    }
}
