using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class AllianceLevelRankController : CommonRankPageController<AllianceLevelRankItemData>
    {
        protected override void UpdateUI(ArrayList array)
        {
            rankdatas = new List<AllianceLevelRankItemData>();
            m_localPlayerRankData = null;
            for (int i = 0; i < array.Count; i++)
            {
                if (i > 0)
                    rankdatas.Add(new AllianceLevelRankItemData(array[i] as Hashtable, i - 1));
                else
                    m_localPlayerRankData = new AllianceLevelRankItemData(array[i] as Hashtable, i);
            }
            rankdatas.Sort((a, b) => { return a.m_Rank - b.m_Rank; });
            //rankdatas.Sort(new AllianceLevelRankItemDataComparer());
            rankdatas = rankdatas.GetRange(0, Mathf.Min(100, rankdatas.Count));
            m_gridScroll.dataItems = rankdatas.ToArray();
            if (rankdatas.Count > 0)
            {
                m_firstPlayerRankData = rankdatas[0];
            }
            else
            {
                m_firstPlayerRankData = null;
            }
            isDataReady = true;
            UpdateInfo();
        }

        public override void ClickTitleRefreshGrid()
        {
            if (rankdatas != null )//&& rankdatas.Count > 0)
            {
                m_gridScroll.dataItems = rankdatas.ToArray();
            }
        }

        protected override void UpdateInfo()
        {
            if (m_firstPlayerRankData != null)
            {
                LTUIUtil.SetText(TheTopOneNameLabel, m_firstPlayerRankData.m_OwnerName);
                LTUIUtil.SetText(TheTopOneLevelLabel, m_firstPlayerRankData.m_OwnerLevel.ToString());
                TheTopOneLevelLabel.gameObject.SetActive(true);
                TheTopOneLevelLabel.UpdateAnchors();
				lTRankListCtrl.OnSwithModel(m_firstPlayerRankData.m_OwnerModel);
            }
            else
            {
                LTUIUtil.SetText(TheTopOneNameLabel, EB.Localizer.GetString("ID_EMPTY_PLACE"));
                TheTopOneLevelLabel.gameObject.SetActive(false);
                lTRankListCtrl.OnSwithModel("");
            }

            if (m_localPlayerRankData.m_Rank >= 0 && m_localPlayerRankData.m_Rank < 100)
            {
                LTUIUtil.SetText(m_localPlayerRankLabel, string.Format("{0}.   {1}   {2}", m_localPlayerRankData.m_Rank + 1, m_localPlayerRankData.m_Name, m_localPlayerRankData.m_Parm));
            }
            else if (m_localPlayerRankData.m_Rank >= 100)
            {
                LTUIUtil.SetText(m_localPlayerRankLabel, string.Format("{0}   {1}   {2}", EB.Localizer.GetString("ID_ARENA_RANK_OUT_OF_RANGE"), m_localPlayerRankData.m_Name, m_localPlayerRankData.m_Parm));
            }
            else
            {
                LTUIUtil.SetText(m_localPlayerRankLabel, string.Format("{0}   {1}   {2}", EB.Localizer.GetString("ID_ARENA_RANK_OUT_OF_RANGE"), BalanceResourceUtil.GetUserName(), EB.Localizer.GetString("ID_NOT_JOIN_ALLIANCE")));
            }
        }

        protected override void InitText()
        {
            //军团名称
            LTUIUtil.SetText(MiddleText, EB.Localizer.GetString("ID_ALLIANCE_NAME"));
            //军团等级
            LTUIUtil.SetText(RightText, EB.Localizer.GetString("ID_ALLIANCE_LEVEL"));
        }

        public override void Awake()
        {
            base.Awake();
            m_PageDataPath = "rank.alliances.Alliance";
        }

    }


}