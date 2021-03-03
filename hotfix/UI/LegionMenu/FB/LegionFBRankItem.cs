using Hotfix_LT.Data;
using System.Collections;
using System.Collections.Generic;



namespace Hotfix_LT.UI
{
    /// <summary>
    /// 军团副本排名数据项
    /// </summary>


    public class LTLegionFBRankItemData : PersonalRankItemData
    {
        /// <summary>
        /// 伤害
        /// </summary>
        public long m_Hurt;

        public LTLegionFBRankItemData() : base()
        {
            m_Hurt = 0;
        }

        public LTLegionFBRankItemData(Hashtable data, int index) : base(data, index)
        {
            m_Hurt = EB.Dot.Long("hurt", data, 0);
        }
    }

    //public class LTLegionFBRankItemDataComparer : IComparer<LTLegionFBRankItemData>
    //{
    //    public int Compare(LTLegionFBRankItemData x, LTLegionFBRankItemData y)
    //    {
    //        return (int)(y.m_Hurt - x.m_Hurt);
    //    }
    //}

    public class LegionFBRankItem : BaseItem<LTLegionFBRankItemData>
    {
        public UILabel m_UIName;
        public UILabel m_UILevel;
        public RankIconOrNumber m_UIRank;
        public UISprite m_UIIcon;
        public UISprite m_FrameIcon;
        public LTLegionFBRankItemData Data;
        /// <summary>
        /// 文本
        /// </summary>
        public UILabel m_TipsLabel;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_UIName = t.GetComponent<UILabel>("Name");
            m_UILevel = t.GetComponent<UILabel>("Level");
            m_UIRank = t.GetMonoILRComponent<RankIconOrNumber>("Rank");
            m_UIIcon = t.GetComponent<UISprite>("PlayerIcon/Pic");
            m_UIBg = t.GetComponent<UISprite>("BG");
            m_FrameIcon=t.GetComponent<UISprite>("PlayerIcon/Frame");
        }

        public override void UpdateUI()
        {
            base.UpdateUI();
            F_SetData(Data);
        }

        public void F_SetData(LTLegionFBRankItemData Data, bool isSelf = false)
        {
            if (Data != null)
            {
                LTLegionFBRankItemData P_Data = Data;
                m_UIRank.mDMono.gameObject.CustomSetActive(true);
                m_UIRank.Rank = P_Data.m_Rank;
                m_UIIcon.spriteName = P_Data.m_Icon;
                m_FrameIcon.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(P_Data.m_Frame).iconId;
                LTUIUtil.SetText(m_UIName, P_Data.m_Name);
                LTUIUtil.SetText(m_UILevel, P_Data.m_Hurt == 0 ? EB.Localizer.GetString("ID_LEGION_MEDAL_NOT") : P_Data.m_Hurt.ToString());
                //
                if (isSelf)
                {
                    m_UIRank.mDMono.gameObject.CustomSetActive(Data.m_Rank >= 0);
                }
                if (m_TipsLabel != null)
                {
                    m_TipsLabel.gameObject.CustomSetActive(Data.m_Rank < 0);
                    LTUIUtil.SetText(m_TipsLabel, EB.Localizer.GetString("ID_ARENA_RANK_OUT_OF_RANGE"));
                }
            }
            else
            {
                m_UIRank.mDMono.gameObject.CustomSetActive(false);
                LTUIUtil.SetText(m_UILevel, EB.Localizer.GetString("ID_ARENA_RANK_OUT_OF_RANGE"));
            }
        }

        public override void Fill(LTLegionFBRankItemData itemData)
        {
            Data = itemData;
            UpdateUI();
        }

        public override void Clean()
        {
            Data = null;
        }
    }
}
