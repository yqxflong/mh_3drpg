using System;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;
using UnityEngine;
using Debug = EB.Debug;

namespace Hotfix_LT.UI{
    public class HonorArenaItemData: PersonalRankItemData
    {
        public int m_CombatPower;
        public int m_Score;
        public int w_id;
        public bool isAll;
        
        public HonorArenaItemData(Hashtable data, int index,bool isAll) : base(data, index)
        {
            m_Score = EB.Dot.Integer("p", data, m_Score);
            m_CombatPower=EB.Dot.Integer("br", data, m_CombatPower);
            w_id=EB.Dot.Integer("w_id", data, w_id);
            this.isAll = isAll;
        }

        public HonorArenaItemData()
        {
            
        }
    }
    
    
    
    public class HonorArenaItem : BaseItem<HonorArenaItemData>
    {
        public RankIconOrNumber m_UIRank;
        public UISprite m_UIIcon;
        public UISprite m_FrameIcon;
        public UILabel m_UIName;
        public UILabel m_UICombatPower;
        public UILabel m_UIScore;
        public UIWidget m_EmptyGameObject;
        public UIWidget m_DataGameObject;
        private RankType RankType;
        public UILabel m_OnHookincomeLabel;

        public HonorArenaItemData Data { get; set; }

        public override void Awake()
        {
            base.Awake();
            var t= mDMono.transform;
            m_UIBg = t.GetComponent<UISprite>("BG"); 
            m_UIRank = t.GetMonoILRComponent<RankIconOrNumber>("Rank");
            m_UIIcon = t.GetComponent<UISprite>("DataContainer/Pic");
            m_FrameIcon = t.GetComponent<UISprite>("DataContainer/Pic/Frame");
            m_UIName = t.GetComponent<UILabel>("DataContainer/Name"); 
            m_UICombatPower = t.GetComponent<UILabel>("DataContainer/CombatPower/Base"); 
            m_UIScore = t.GetComponent<UILabel>("DataContainer/Score");
            m_OnHookincomeLabel= t.GetComponent<UILabel>("DataContainer/OnHookincomeLabel");
            m_EmptyGameObject = t.GetComponent<UIWidget>("EmptyContainer");
            m_DataGameObject= t.GetComponent<UIWidget>("DataContainer");
            m_UIIcon.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                OnIconClick();
            }));

            Messenger.AddListener<int>(EventName.HonorCombatTeamPowerUpdate, onCombatTeamPowerUpdate);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Messenger.RemoveListener<int>(EventName.HonorCombatTeamPowerUpdate, onCombatTeamPowerUpdate);
        }

        public void onCombatTeamPowerUpdate(int pow)
        {
            if (Data!=null && Data.m_Uid==LoginManager.Instance.LocalUserId.Value)
            {
                Data.m_CombatPower = HonorArenaManager.Instance.AllCombatPower;
                LTUIUtil.SetText(m_UICombatPower, Data.m_CombatPower.ToString());
            }
        }

        public override void UpdateUI()
        {
            base.UpdateUI();
            if (Data==null || Data.m_Uid<=0)
            {
                m_UIRank.Rank = ViewIndex;
                m_EmptyGameObject.alpha = 1;
                m_DataGameObject.alpha = 0;
            }
            else
            {
                m_UIRank.Rank = Data.m_Rank;
                m_EmptyGameObject.alpha = 0;
                m_DataGameObject.alpha = 1;
                m_UIIcon.spriteName = Data.m_Icon;
                m_FrameIcon.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(Data.m_Frame).iconId;
                m_OnHookincomeLabel.gameObject.CustomSetActive(!Data.isAll);
                if (Data.isAll)
                {
                    LTUIUtil.SetText(m_UIName, string.Format("{0}【{1}{2}】", Data.m_Name, Data.w_id, EB.Localizer.GetString("ID_LOGIN_SERVER_NAME")));
                }
                else
                {
                    LTUIUtil.SetText(m_UIName, Data.m_Name);
                    LTUIUtil.SetText(m_OnHookincomeLabel, string.Format(EB.Localizer.GetString("ID_LEGION_TECH_REWARDRATE"), HonorArenaConfig.Instance.GetOneHourByReward(Data.m_Rank + 1)));

                }
                if (Data.m_Uid==LoginManager.Instance.LocalUserId.Value)
                {
                    Data.m_CombatPower = HonorArenaManager.Instance.AllCombatPower;
                }
                LTUIUtil.SetText(m_UICombatPower, Data.m_CombatPower.ToString());
                LTUIUtil.SetText(m_UIScore, Data.m_Score.ToString());
            }
        }

        public override void Fill(HonorArenaItemData itemData)
        {
            Data = itemData;
            UpdateUI();
        }

        public override void Clean()
        {
            Data = null;
            UpdateUI();
        }

        public virtual void OnIconClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (Data.m_Uid == LoginManager.Instance.LocalUserId.Value)
            {
                return;
            }

            LTOtherInfoController.Open(TitleType.Info_Title,Data.m_Uid.ToString(),Data.m_Name,int.Parse(Data.m_DrawLevel),Data.w_id,Data.m_Score,Data.m_CombatPower,Data.m_Rank);
        }
    }
    
} 