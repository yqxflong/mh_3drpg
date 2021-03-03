using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 人物排名榜控件
    /// </summary>
    public class PersonalLevelRankController : CommonRankPageController<PersonalLevelRankItemData>
    {
        protected override void UpdateUI(ArrayList array)
        {
            rankdatas = new List<PersonalLevelRankItemData>();
            long localPlayerId = LoginManager.Instance.LocalUserId.Value;
            m_localPlayerRankData = null;
            for (int i = 0; i < array.Count; i++)
            {
                var data = new PersonalLevelRankItemData(array[i] as Hashtable, i);
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
            //rankdatas.Sort(new PersonalLevelRankItemDataComparer());
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

        public override void ClickTitleRefreshGrid()
        {
            if (rankdatas != null && rankdatas.Count > 0)
            {
                m_gridScroll.dataItems = rankdatas.ToArray();
            }
        }

        public override void Awake()
        {
            base.Awake();
            m_PageDataPath = "rank.personal.level";
            m_FreshDataDeltaTime = 5;
        }
    }

}