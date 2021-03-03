using System.Collections;
using System.Collections.Generic;
using EB;
using UnityEngine;


namespace Hotfix_LT.UI
{
    /// <summary>
    /// 个人挑战排名榜控件
    /// </summary>
    public class PersonalChallengeInstanceRankController : CommonRankPageController<PersonalChallengeInstanceRankItemData>
    {
        protected override void UpdateUI(ArrayList array)
        {
            rankdatas = new List<PersonalChallengeInstanceRankItemData>();
            long localPlayerId = LoginManager.Instance.LocalUserId.Value;
            m_localPlayerRankData = null;
            for (int i = 0; i < array.Count; i++)
            {
                var data = new PersonalChallengeInstanceRankItemData(array[i] as Hashtable, i);
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
            //rankdatas.Sort(new PersonalChallengeInstanceRankItemDataComparer());
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
            OnEnable();
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
            //层数
            LTUIUtil.SetText(RightText, Localizer.GetString("ID_uifont_in_LTChallengeInstanceHud_Label_0"));
        }

        public override void Awake()
        {
            base.Awake();
            m_PageDataPath = "rank.personal.LostChallenge";
        }
    }
}
