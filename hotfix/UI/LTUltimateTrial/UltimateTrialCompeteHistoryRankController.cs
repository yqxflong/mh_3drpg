using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class UltimateTrialCompeteHistoryRankController : CommonRankPageController<LTUltimateTrialCompeteRankItemData>
    {
        public override CommonRankGridScroll m_gridScroll
        {
            get
            {
                if (_gridScroll == null)
                    _gridScroll = mDMono.transform.parent.parent.parent.parent.parent.Find("Content/ScrollView/Placehodler/Grid").GetMonoILRComponent<CommonRankGridScroll>();
                return _gridScroll;
            }
            set { _gridScroll = value; }
        }

        public override void Awake()
        {
            m_PageDataPath = "rank.history.speedinifiniteChallenge";
            m_FreshDataDeltaTime = 5;
            m_localPlayerRankLabel = mDMono.transform.parent.parent.parent.parent.GetComponent<UILabel>("Bottom/SelfInfoLabel");
        }

        protected override bool NeedRefreshData()
        {
            return true;
        }

        protected override void UpdateUI(ArrayList array)
        {
            rankdatas = new List<LTUltimateTrialCompeteRankItemData>();
            long localPlayerId = LoginManager.Instance.LocalUserId.Value;
            m_localPlayerRankData = null;
            for (int i = 0; i < array.Count; i++)
            {
                var data = new LTUltimateTrialCompeteRankItemData(array[i] as Hashtable, i);
                if (data.m_Uid == localPlayerId)
                {
                    m_localPlayerRankData = data;
                }
                else
                {
                    if (data.m_Rank >= 0)
                    {
                        rankdatas.Add(data);
                    }
                }
            }
            if (m_localPlayerRankData != null && m_localPlayerRankData.m_Rank >= 0)
            {
                rankdatas.Add(m_localPlayerRankData);
            }
            rankdatas.Sort((x, y) => { return x.m_Rank - y.m_Rank; });
            rankdatas = rankdatas.GetRange(0, Mathf.Min(100, rankdatas.Count));
            if (rankdatas.Count > 0)
            {
                m_firstPlayerRankData = rankdatas[0];
            }
            else
            {
                m_firstPlayerRankData = null;
            }
            m_gridScroll.dataItems = rankdatas.ToArray();
            isDataReady = true;
            defaultocalPlayerRankLabelData = BalanceResourceUtil.GetUserLevel().ToString();
            UpdateInfo();
        }

        protected override void UpdateInfo()
        {
            string temp = string.Empty;
            if(m_localPlayerRankData != null)
            {
                temp = string.Format("{0}[fff348]{1}[-]        {2}", EB.Localizer.GetString("ID_ARENA_LOCAL_RANK"), m_localPlayerRankData.m_Rank + 1, m_localPlayerRankData.m_Parm.Replace("\n", "        "));
            }
            LTUIUtil.SetText(m_localPlayerRankLabel, temp);
        }

        public override void ClickTitleRefreshGrid()
        {
            if (rankdatas != null && rankdatas.Count > 0)
            {
                m_gridScroll.dataItems = rankdatas.ToArray();
            }
            else
            {
                m_gridScroll.Clear();
                LTUIUtil.SetText(m_localPlayerRankLabel, string.Empty);
            }
        }
    }
}