using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{

    /// <summary>
    /// 军团排名控件
    /// 对应挂载的预置体：LTLegionFBUI
    /// </summary>
    public class LTLegionFBRankController : RankPageController
    {

        public LTLegionFBRankGridScroll m_gridScroll;
        public UILabel m_localPlayerRankLabel;
        public int m_FreshDataDeltaTime = 5;
        private bool isDataReady;
        protected LTLegionFBRankItemData m_localPlayerRankData;
        /// <summary>
        /// 自身排名
        /// </summary>
        public LegionFBRankItem v_SelfRank;
        private static int _LastGetDataTime = 0;
        /// <summary>
        /// 当前的BOSSid
        /// </summary>
        private int m_CurrentBossID;
        /// <summary>
        /// 所有缓存的排行数据
        /// </summary>
        private Dictionary<int, List<LTLegionFBRankItemData>> m_AllRank;

        public override void Awake()
        {
            Transform t = mDMono.transform;
            m_PageDataPath = "rank.alliancefb";
            m_gridScroll = t.GetMonoILRComponent<LTLegionFBRankGridScroll>("Scroll View/Placeholder/Grid");
            v_SelfRank = t.GetMonoILRComponent<LegionFBRankItem>("Self");

        }

        /// <summary>
        /// 刷新BOSS排名榜
        /// </summary>
        /// <param name="bossID">BossID</param>
        public void F_UpdateBossRank(int bossID)
        {
            m_CurrentBossID = bossID;
            if (m_AllRank == null)
            {
                m_AllRank = new Dictionary<int, List<LTLegionFBRankItemData>>();
            }
            if (m_AllRank.ContainsKey(bossID))
            {
                SetRank(m_AllRank[bossID]);
            }
            else
            {
                //向服器器发送请求当前的数据
                m_UIServerRequest = mDMono.GetComponent<UIServerRequest>();
                if (m_UIServerRequest != null)
                {
                    UIServerRequestHotFix mysGetRequest = mDMono.transform.GetMonoILRComponentByClassPath<UIServerRequestHotFix>("Hotfix_LT.UI.UIServerRequestHotFix", true);
                    m_UIServerRequest.onResponse.Add(new EventDelegate(mysGetRequest.mDMono, "OnFetchData"));
                }
                else
                {
                    EB.Debug.LogError("Didnot have UIServerRequest In this object{0}" , mDMono.name);
                }
                //
                m_UIServerRequest.parameters = new UIServerRequest.ServerParameter[1];
                m_UIServerRequest.parameters[0] = new UIServerRequest.ServerParameter();
                m_UIServerRequest.parameters[0].name = "BossID";
                m_UIServerRequest.parameters[0].parameter = bossID.ToString();
                //
                FetchDataRemote();
            }
        }

        protected override bool NeedRefreshData()
        {
            if (_LastGetDataTime == 0)
            {
                _LastGetDataTime = EB.Time.Now;
                return true;
            }
            else
            {
                if (EB.Time.Now - _LastGetDataTime >= m_FreshDataDeltaTime)
                {
                    _LastGetDataTime = EB.Time.Now;
                    return true;
                }
            }
            return false;
        }

        protected override void UpdateUI(ArrayList array)
        {
            List<LTLegionFBRankItemData> rankdatas = new List<LTLegionFBRankItemData>();
            for (int i = 0; i < array.Count; i++)
            {
                var data = new LTLegionFBRankItemData();
                data = new LTLegionFBRankItemData(array[i] as Hashtable, i);

                rankdatas.Add(data);
            }
            LTLegionFBRankItemData temp = rankdatas.Find(p => p.m_Uid == LoginManager.Instance.LocalUserId.Value);
            if (temp == null)
            {
                temp = new LTLegionFBRankItemData();
                temp.m_Name = LoginManager.Instance.LocalUser.Name;
                temp.m_Icon = LoginManager.Instance.LocalUser.Icon;
                temp.m_Frame = LTMainHudManager.Instance.UserLeaderHeadFrameStr;
                temp.m_Rank = -1;
            }
            m_localPlayerRankData = temp;
            int total = rankdatas.Count;
            for (int i = 0; i < 4 - total; i++)
            {
                var data = new LTLegionFBRankItemData();
                data.m_Name = EB.Localizer.GetString("ID_codefont_in_NationStruct_17151");
                data.m_Icon = "Ty_Touxiang_Di1";
                rankdatas.Add(data);
            }

            //
            rankdatas.Sort((a, b) => { return (int)(b.m_Hurt - a.m_Hurt); });
            //rankdatas.Sort(new LTLegionFBRankItemDataComparer());
            rankdatas = rankdatas.GetRange(0, Mathf.Min(4, rankdatas.Count));
            for (int i = 0; i < rankdatas.Count; i++)
            {
                rankdatas[i].m_Rank = i;
            }
            //当前的排名数据添加到缓存里
            if (m_AllRank.ContainsKey(m_CurrentBossID))
            {
                m_AllRank[m_CurrentBossID].Clear();
                m_AllRank[m_CurrentBossID] = rankdatas;
            }
            else
            {
                m_AllRank.Add(m_CurrentBossID, rankdatas);
            }
            //
            SetRank(rankdatas);
        }

        private void SetRank(List<LTLegionFBRankItemData> rankdatas)
        {
            m_gridScroll.SetItemDatas(rankdatas.ToArray());
            isDataReady = true;
            this.UpdateInfo();
        }

        public void CleanRank()
        {
            m_AllRank.Clear();
        }
        
        public override void OnEnable()
        {
            //if (!isDataReady)
            //    return;
            //this.UpdateInfo();
        }

        protected virtual void UpdateInfo()
        {
            //在之前已经给该值赋值了 并且改变了m_AllRank的数据
            // UnityEngine.Debug.LogError("LocalUserId:" + LoginManager.Instance.LocalUserId.Value);
            // Debug.LogError("m_CurrentBossID:" + m_CurrentBossID);
            // foreach (var item in m_AllRank)
            // {
            //     EB.Debug.LogError("key:" + item.Key);
            // }
            m_localPlayerRankData = m_AllRank[m_CurrentBossID].Find(p => p.m_Uid == LoginManager.Instance.LocalUserId.Value);
            if (m_localPlayerRankData == null)
            {
                m_localPlayerRankData = new LTLegionFBRankItemData();
                m_localPlayerRankData.m_Name = LoginManager.Instance.LocalUser.Name;
                m_localPlayerRankData.m_Icon = LoginManager.Instance.LocalUser.Icon;
                m_localPlayerRankData.m_Frame = LTMainHudManager.Instance.UserLeaderHeadFrameStr;
                m_localPlayerRankData.m_Rank = -1;
            }
            v_SelfRank.F_SetData(m_localPlayerRankData, true);
        }
    }
}
