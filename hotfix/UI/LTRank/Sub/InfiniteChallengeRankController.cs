using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EB;

namespace Hotfix_LT.UI
{
    public class InfiniteChallengeRankController : CommonRankPageController<InfiniteChallengeRankItemData>
    {
        protected override void UpdateUI(ArrayList array)
        {
            rankdatas = new List<InfiniteChallengeRankItemData>();
            long localPlayerId = LoginManager.Instance.LocalUserId.Value;
            m_localPlayerRankData = null;
            for (int i = 0; i < array.Count; i++)
            {
                var data = new InfiniteChallengeRankItemData(array[i] as Hashtable, i - 1);
                if (data.m_Uid == localPlayerId)
                {
                    m_localPlayerRankData = data;
                }
                else if (data.m_Rank >= 0)
                {
                    rankdatas.Add(data);
                }
            }

            if (m_localPlayerRankData != null && m_localPlayerRankData.m_Rank >= 0)
            {
                rankdatas.Add(m_localPlayerRankData);
            }
            rankdatas.Sort((x, y) => { return x.m_Rank - y.m_Rank; });
            //rankdatas.Sort(new InifiniteChallengeRankItemDataComparer());
            rankdatas = rankdatas.GetRange(0, Mathf.Min(100, rankdatas.Count));
            if (rankdatas.Count > 0)
            {
                m_firstPlayerRankData = rankdatas[0];
            }
            else
            {
                m_firstPlayerRankData = null;
            }

            m_gridScroll.dataItems = (InfiniteChallengeRankItemData[]) rankdatas.ToArray();

            isDataReady = true;
            UpdateInfo();
        }

        public override void ClickTitleRefreshGrid()
        {
            if (rankdatas != null && rankdatas.Count > 0)
            {
                m_gridScroll.dataItems = rankdatas.ToArray();
            }
        }

        protected override void InitText()
        {
            //玩家名称
            LTUIUtil.SetText(MiddleText, Localizer.GetString("ID_uifont_in_LTRankListHud_Name_0"));
            //最高层数
            LTUIUtil.SetText(RightText, Localizer.GetString("ID_uifont_in_LTRankListHud_Layer_5"));
        }

        public override void Awake()
        {
            base.Awake();
            m_PageDataPath = "rank.personal.inifiniteChallenge";
        }
    }
}
