using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// PVP伙伴备选展示面板
    /// </summary>
    public class LTPVPPrepareCardHud
    {
        /// <summary>
        /// 当前选中的伙伴类型
        /// </summary>
        private eAttrTabType mCurPartnerTabType;
        /// <summary>
        /// 展示的伙伴卡片信息
        /// </summary>
        private LTPVPCardInfoHud m_CardItem;
        /// <summary>
        /// 网格排序
        /// </summary>
        private UIGrid m_Grid;
        /// <summary>
        /// 网格排序
        /// </summary>
        private UIScrollView m_ScrollView;
        /// <summary>
        /// 所有的伙伴卡片item
        /// </summary>
        private List<LTPVPCardInfoHud> m_AllCardItem;
        /// <summary>
        /// 当前数据
        /// </summary>
        private HeroBattleChoiceData m_Data;

        public LTPVPPrepareCardHud(Transform transform)
        {
            mCurPartnerTabType = eAttrTabType.None;
            m_CardItem = new LTPVPCardInfoHud(transform.Find("BuddyList/Placeholder/PartnerGrid/Item"));
            m_AllCardItem = new List<LTPVPCardInfoHud>();
            m_Grid = transform.Find("BuddyList/Placeholder/PartnerGrid").GetComponent<UIGrid>();
            m_ScrollView = transform.Find("BuddyList").GetComponent<UIScrollView>();
            transform.Find("BG/Title/BtnList/AllBtn").GetComponent<UIButton>().onClick.Add(new EventDelegate(OnClickAllBtn));
            transform.Find("BG/Title/BtnList/FengBtn").GetComponent<UIButton>().onClick.Add(new EventDelegate(OnClickFengBtn));
            transform.Find("BG/Title/BtnList/HuoBtn").GetComponent<UIButton>().onClick.Add(new EventDelegate(OnClickHuoBtn));
            transform.Find("BG/Title/BtnList/ShuiBtn").GetComponent<UIButton>().onClick.Add(new EventDelegate(OnClickShuiBtn));
            transform.Find("BG/RuleBtn").GetComponent<UIButton>().onClick.Add(new EventDelegate(OnClickRuleBtn));
        }

        /// <summary>
        /// 设置备选卡片信息
        /// </summary>
        /// <param name="allCard">备选的卡片列表</param>
        public void F_SetCardInfos(HeroBattleChoiceData data)
        {
            m_Data = data;
            mCurPartnerTabType = mCurPartnerTabType == eAttrTabType.None ? eAttrTabType.All : mCurPartnerTabType;
            List<HeroBattleChoiceCellData> allCard = new List<HeroBattleChoiceCellData>();
            switch (mCurPartnerTabType)
            {
                case eAttrTabType.Feng:
                    allCard.AddRange(data.dicHeroChoiceData[Hotfix_LT.Data.eRoleAttr.Feng]);
                    break;
                case eAttrTabType.Huo:
                    allCard.AddRange(data.dicHeroChoiceData[Hotfix_LT.Data.eRoleAttr.Huo]);
                    break;
                case eAttrTabType.Shui:
                    allCard.AddRange(data.dicHeroChoiceData[Hotfix_LT.Data.eRoleAttr.Shui]);
                    break;
                case eAttrTabType.All:
                    allCard.AddRange(data.dicHeroChoiceData[Hotfix_LT.Data.eRoleAttr.Feng]);
                    allCard.AddRange(data.dicHeroChoiceData[Hotfix_LT.Data.eRoleAttr.Huo]);
                    allCard.AddRange(data.dicHeroChoiceData[Hotfix_LT.Data.eRoleAttr.Shui]);
                    break;
            }
            //
            for (int i = 0; i < m_AllCardItem.Count; i++)
            {
                m_AllCardItem[i].F_SetCardInfo(null);
            }
            // 排序
            allCard.Sort((x, y) =>
            {
                int result = y.level.CompareTo(x.level);
                if (result == 0)
                {
                    result = y.star.CompareTo(x.star);
                }
                return result;
            });
            //
            for (int i = 0; i < allCard.Count; i++)
            {
                if (i >= m_AllCardItem.Count)
                {
                    m_AllCardItem.Add(m_CardItem.F_Clone(m_Grid.transform));
                }
                m_AllCardItem[i].F_SetCardInfo(allCard[i]);
            }
            m_Grid.Reposition();
            m_ScrollView.ResetPosition();
        }

        private void OnClickAllBtn()
        {
            if (mCurPartnerTabType != eAttrTabType.All)
            {
                mCurPartnerTabType = eAttrTabType.All;
                this.F_SetCardInfos(m_Data);
            }
        }

        private void OnClickFengBtn()
        {
            if (mCurPartnerTabType != eAttrTabType.Feng)
            {
                mCurPartnerTabType = eAttrTabType.Feng;
                this.F_SetCardInfos(m_Data);
            }
        }
        private void OnClickHuoBtn()
        {
            if (mCurPartnerTabType != eAttrTabType.Huo)
            {
                mCurPartnerTabType = eAttrTabType.Huo;
                this.F_SetCardInfos(m_Data);
            }
        }
        private void OnClickShuiBtn()
        {
            if (mCurPartnerTabType != eAttrTabType.Shui)
            {
                mCurPartnerTabType = eAttrTabType.Shui;
                this.F_SetCardInfos(m_Data);
            }
        }

        /// <summary>
        /// 点击规制按钮
        /// </summary>
        private void OnClickRuleBtn()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.5f);
            GlobalMenuManager.Instance.Open("LTAttributeInfo");
        }
    }
}
