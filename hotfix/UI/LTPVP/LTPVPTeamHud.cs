using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 队伍阵容UI管件
    /// </summary>
    public class LTPVPTeamHud
    {
        /// <summary>
        /// 所有的阵容卡片
        /// </summary>
        private LTPVPTeamCardInfo[] m_TeamCard;
        /// <summary>
        /// 是否为自己
        /// </summary>
        private bool m_IsSelfTeam;
        /// <summary>
        /// 设置数量
        /// </summary>
        private int m_SetTotal;


        public LTPVPTeamHud(Transform transform, bool isSelfTeam)
        {
            m_IsSelfTeam = isSelfTeam;
            m_TeamCard = new LTPVPTeamCardInfo[6];
            for (int i = 0; i < m_TeamCard.Length; i++)
            {
                m_TeamCard[i] = new LTPVPTeamCardInfo(transform.Find("Content/Grid/Item_" + i), OnClickPartnerItem);
            }
            m_SetTotal = 0;
        }

        /// <summary>
        /// 设置队伍
        /// </summary>
        /// <param name="team">队伍数据</param>
        /// <param name="playerInfo">玩家信息</param>
        /// <param name="bans">禁用的数据</param>
        /// <param name="isSelfTeam">是否为我方回合</param>
        public void F_SetTeam(List<HeroBattleChoiceCellData> team, SidePlayerInfoData playerInfo, List<HeroBattleChoiceCellData> bans)
        {
            if (m_TeamCard != null)
            {
                for (int i = 0; i < m_TeamCard.Length; i++)
                {
                    if (team != null && i < team.Count)
                    {
                        bool needSetFx = m_SetTotal != team.Count ? i == team.Count - 1 : false;
                        m_TeamCard[i].F_SetCardInfo(team[i], playerInfo, bans != null && bans.Find(p => p.heroTplID == team[i].heroTplID) != null, needSetFx);
                    }
                    else
                    {
                        m_TeamCard[i].F_SetCardInfo(null, playerInfo, false, false);
                    }
                }
            }

            if (team != null)
            {
                m_SetTotal = team.Count;
            }
        }

        /// <summary>
        /// 是否设置高亮
        /// </summary>
        /// <param name="isHeightLight">是否高亮</param>
        /// <param name="maxIndex">最大索引</param>
        /// <param name="banState">是否为禁用英雄状态</param>
        public void F_SetHeightLight(bool isHeightLight, int maxIndex, bool banState)
        {
            for (int i = 0; i < m_TeamCard.Length; i++)
            {
                if (banState)//禁用状态全亮
                {
                    bool heightLight = i <= maxIndex ? isHeightLight : false;
                    m_TeamCard[i].F_SetHeightLight(heightLight, m_IsSelfTeam);
                }
                else//选人状态最后一个
                {
                    bool heightLight = i == maxIndex ? isHeightLight : false;
                    m_TeamCard[i].F_SetHeightLight(heightLight, m_IsSelfTeam);
                }
            }
        }

        /// <summary>
        /// 点击阵型里的伙伴
        /// </summary>
        /// <param name="cardItem">点击的卡片item</param>
        private void OnClickPartnerItem(LTPVPTeamCardInfo cardItem)
        {
            HeroBattleChoiceCellData partnerData = cardItem.F_GetCurrentPartnerData();
            if (LTHeroBattleModel.GetInstance().choiceData.choiceState == 0
                && LTHeroBattleModel.GetInstance().choiceData.openUid == LTHeroBattleModel.GetInstance().choiceData.selfInfo.uid
                && !m_IsSelfTeam
                && partnerData != null)
            {
                //通知选中的人
                FusionAudio.PostEvent("UI/General/ButtonClick", true);
                LTHeroBattleEvent.ChoiceHero(partnerData.heroTplID);
                //
                for (int i = 0; i < m_TeamCard.Length; i++)
                {
                    m_TeamCard[i].F_SetLockState(m_TeamCard[i] == cardItem);
                }
            }
        }

        /// <summary>
        /// 托管情况下获取英雄
        /// </summary>
        /// <returns></returns>
        public int GetAutoSelectHero()
        {
            int temp = 0;
            if (m_IsSelfTeam)
            {
                List<LTPartnerData> partnerList = LTPartnerDataManager.Instance.GetOwnPartnerListSortByPowerData();
                for (int i = 0; i < partnerList.Count; ++i)
                {
                    if (HeroInTeam(partnerList[i].HeroStat.id))
                    {
                        continue;
                    }
                    else
                    {
                        temp = partnerList[i].HeroStat.id;
                        break;
                    }
                }
            }
            else
            {
                int level = 0;
                int star = 0;
                for (int i = 0; i < m_TeamCard.Length; i++)
                {
                    HeroBattleChoiceCellData partnerData = m_TeamCard[i].F_GetCurrentPartnerData();
                    if (partnerData != null && partnerData.heroTplID != 0)
                    {
                        if (partnerData.level > level|| partnerData.level == level && partnerData.star > star)
                        {
                            level = partnerData.level;
                            star = partnerData.star;
                            temp = partnerData.heroTplID;
                        }
                    }
                }
            }
            return temp;
        }

        private bool HeroInTeam(int herotpid)
        {
            for (int i = 0; i < m_TeamCard.Length; i++)
            {
                HeroBattleChoiceCellData partnerData = m_TeamCard[i].F_GetCurrentPartnerData();
                if (partnerData!=null&&LTHeroBattleModel.GetInstance().choiceData!=null&&LTHeroBattleModel.GetInstance().choiceData.choiceState != 0 && herotpid== partnerData.heroTplID)
                {
                    return true;
                }
            }
            return false;
        }
    }
}