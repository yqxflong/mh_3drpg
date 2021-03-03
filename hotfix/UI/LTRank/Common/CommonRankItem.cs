using Hotfix_LT.Data;
using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class CommonRankItem : BaseItem<CommonRankItemData>
    {
        public RankIconOrNumber m_UIRank;
        public UISprite m_UIIcon;
        public UISprite m_FrameIcon;
        public UILabel m_UIName;
        public UILabel m_UILevel;
        private RankType RankType;

        public UISprite m_BadgeIcon;
        public UISprite m_BadgeBG;
        public CommonRankItemData Data { get; set; }

        public override void Awake()
        {
            base.Awake();
            m_UIRank = mDMono.transform.GetMonoILRComponent<RankIconOrNumber>("Rank",false);
            m_UIIcon = mDMono.transform.GetComponent<UISprite>("Pic",false);
            m_FrameIcon = mDMono.transform.GetComponent<UISprite>("Pic/Frame",false);
            m_UIName = mDMono.transform.GetComponent<UILabel>("Name", false); 
            m_UILevel = mDMono.transform.GetComponent<UILabel>("Level", false); 
            m_BadgeIcon = mDMono.transform.GetComponent<UISprite>("AlliancePic/AllianceIcon", false); 
            m_BadgeBG = mDMono.transform.GetComponent<UISprite>("AlliancePic", false); 
            m_UIBg = mDMono.transform.GetComponent<UISprite>("BG", false); 
            m_UIIcon.gameObject.GetComponent<UIEventTrigger>().onClick.Add(new EventDelegate(() =>
            {
                OnIconClick();
            }));
        }

        public override void UpdateUI()
        {
            base.UpdateUI();
            //通用处理
            m_UIRank.Rank = Data.m_Rank;
            if (!m_UIIcon.gameObject.activeInHierarchy) m_UIIcon.gameObject.CustomSetActive(true);
            if (m_BadgeBG!=null&& m_BadgeBG.gameObject.activeInHierarchy) m_BadgeBG.gameObject.CustomSetActive(false);
            m_UIIcon.spriteName = Data.m_Icon;
            m_FrameIcon.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(Data.m_Frame).iconId;
            LTUIUtil.SetText(m_UIName, Data.m_Name);
            LTUIUtil.SetText(m_UILevel, Data.m_Parm);
            //特殊处理
            OnUpdateUI();

        }

        private void OnUpdateUI()
        {
            if (Data is PersonalLevelRankItemData)
            {
                RankType = RankType.PersonalLevel;
            }
            else if (Data is AllianceLevelRankItemData)
            {
                RankType = RankType.AllianceLevel;
                DealAlliance();
            }
            else if (Data is PersonalLadderRankItemData)
            {
                RankType = RankType.PersonalLadder;
            }
            else if (Data is PersonalArenaRankItemData)
            {
                RankType = RankType.PersonalArena;
            }
            else if (Data is InfiniteChallengeRankItemData)
            {
                RankType = RankType.PersonalInfinite;
            }
            else if (Data is PersonalChallengeInstanceRankItemData)
            {
                RankType = RankType.PersonalInstance;
            }
        }

        private void DealAlliance()
        {
            m_UIIcon.gameObject.CustomSetActive(false);
            m_BadgeBG.gameObject.CustomSetActive(true);
            m_BadgeIcon.spriteName = Data.m_Icon;
            m_BadgeBG.spriteName = Data.m_BadgeBGIcon;
        }

        public override void Fill(CommonRankItemData itemData)
        {
            Data = itemData;
            UpdateUI();
        }

        public override void Clean()
        {
            Data = null;
        }

        public virtual void OnIconClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (Data.m_Uid == LoginManager.Instance.LocalUserId.Value || RankType == RankType.AllianceLevel|| Data.m_Uid == 0)
            {
                return;
            }

            CommonRankItemData P_Data = Data;
            Hashtable mainData = Johny.HashtablePool.Claim();
            mainData.Add("name", P_Data.m_Name);
            mainData.Add("icon", P_Data.m_Icon);
            mainData.Add("level", P_Data.m_DrawLevel);
            mainData.Add("headFrame", EconemyTemplateManager.Instance.GetHeadFrame(P_Data.m_Frame).iconId);
            Hashtable data = Johny.HashtablePool.Claim();
            //个人P_Data.m_Index  角斗场P_Data.m_Rank
             int temp = RankType == RankType.PersonalArena ? P_Data.m_Rank : P_Data.m_Index;
            //EB.Debug.LogError("mRank:"+P_Data.m_Rank+" m_Index:"+P_Data.m_Index);
            data.Add(SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM3_TYPE0, temp);
            Hashtable viewData = Johny.HashtablePool.Claim();
            viewData["mainData"] = mainData;
            if (P_Data.m_IsPlayer)
                viewData["infoType"] = eOtherPlayerInfoType.canInteraction;
            else
                viewData["infoType"] = eOtherPlayerInfoType.only;
            viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM0] = P_Data.m_Uid;
            if (RankType == RankType.PersonalArena)
            {
                viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM1] = SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM1_TYPE0;
                viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM2] = SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM2_TYPE0;
            }
            else
            {
                viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM1] = SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM1_TYPE1;
                viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM2] = SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM2_TYPE1;
            }
            viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM3] = data;
            GlobalMenuManager.Instance.Open("LTCheckPlayerFormationInfoUI", viewData);
        }
    }

    public abstract class BaseItem<T> : DynamicCellController<T>
    {
        public UISprite m_UIBg;
        protected const string BG_1 = "Ty_Mail_Di1";
        protected const string BG_2 = "Ty_Mail_Di2";

        public virtual void UpdateUI()
        {
            if (m_UIBg == null) m_UIBg = UnityHelper.GetTheChildNodeComponetScripts<UISprite>(mDMono.gameObject, "BG");
            if (DataIndex % 2 == 0)
            {
                m_UIBg.spriteName = BG_1;
            }
            else
            {
                m_UIBg.spriteName = BG_2;
            }
        }
    }

}
