using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using Debug = EB.Debug;


namespace Hotfix_LT.UI
{
    public class LTArenaRankListCtrl : DynamicMonoHotfix
    {
        private string m_PageDataPath = "rank.personal.ghonorarena";

        public UIConditionTabController categoryTabs;
        public HonorRankGridScroll gridScroll;

        public UILabel AllRankLabel;
        public UILabel SelfRankLabel;
        public UILabel OnHookincomeLabel;

        public UIServerRequest selfRequest = null;
        public UIServerRequest allRequest = null;

        public override void Awake()
        {
            base.Awake();
            SelfRankLabel = mDMono.transform.parent.GetComponent<UILabel>("LeftTopHold/SelfServerRankOrder");
            AllRankLabel = mDMono.transform.parent.GetComponent<UILabel>("LeftTopHold/AllServerRankOrder");
            OnHookincomeLabel = mDMono.transform.parent.GetComponent<UILabel>("LeftTopHold/OnHookincomeLabel");
            categoryTabs = mDMono.transform.GetMonoILRComponent<UIConditionTabController>("ContentView");
            categoryTabs.TabLibPrefabs = new List<UITabControllerHotFix.TabLibEntry>();
            gridScroll =
                mDMono.transform.GetMonoILRComponent<HonorRankGridScroll>(
                    "ContentView/Views/Scroll View/Placeholder/Grid");
            selfRequest =
                mDMono.transform.GetComponent<UIServerRequest>("ContentView/UpButtons/ButtonGrid/1_Self/Self");
            selfRequest.onResponse.Add(new EventDelegate(mDMono, "OnFetchData"));
            allRequest = mDMono.transform.GetComponent<UIServerRequest>("ContentView/UpButtons/ButtonGrid/0_All/All");
            allRequest.onResponse.Add(new EventDelegate(mDMono, "OnFetchData"));
            HonorArenaRankNeedReq();
            Init();
            Messenger.AddListener(EventName.HonorArenaRankNeedReq, HonorArenaRankNeedReq);
        }


        public override void OnDestroy()
        {
            base.OnDestroy();
            Messenger.RemoveListener(EventName.HonorArenaRankNeedReq, HonorArenaRankNeedReq);
        }

        private void HonorArenaRankNeedReq()
        {
            FetchDataRemote(selfRequest);
            FetchDataRemote(allRequest);
        }


        private void Init()
        {
            UITabControllerHotFix.TabLibEntry entry = new UITabControllerHotFix.TabLibEntry();
            Transform mDMono = categoryTabs.mDMono.transform;

            GameObject TabObj1 = mDMono.transform.Find("UpButtons/ButtonGrid/0_All/Tab1").gameObject;
            GameObject PressedTabObj1 = mDMono.transform.Find("UpButtons/ButtonGrid/0_All/Tab2").gameObject;
            GameObject GameViewObj1 = mDMono.transform.Find("UpButtons/ButtonGrid/0_All/All").gameObject;
            entry.TabObj = TabObj1;
            entry.PressedTabObj = PressedTabObj1;
            entry.GameViewObj = GameViewObj1;
            categoryTabs.TabLibPrefabs.Add(entry);

            TabObj1.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                categoryTabs.OnTabPressed(TabObj1);
                FetchDataRemote(allRequest);
            }));

            GameObject TabObj2 = mDMono.transform.Find("UpButtons/ButtonGrid/1_Self/Tab1").gameObject;
            GameObject PressedTabObj2 = mDMono.transform.Find("UpButtons/ButtonGrid/1_Self/Tab2").gameObject;
            GameObject GameViewObj2 = mDMono.transform.Find("UpButtons/ButtonGrid/1_Self/Self").gameObject;
            entry.TabObj = TabObj2;
            entry.PressedTabObj = PressedTabObj2;
            entry.GameViewObj = GameViewObj2;
            categoryTabs.TabLibPrefabs.Add(entry);

            TabObj2.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                categoryTabs.OnTabPressed(TabObj2);
                FetchDataRemote(selfRequest);
            }));
        }

        public override void OnFetchData(EB.Sparx.Response result, int reqInstanceID)
        {
            LoadingSpinner.Hide();
            if (result.sucessful && result.hashtable != null)
            {
                ArrayList array = Hotfix_LT.EBCore.Dot.Array(m_PageDataPath, result.hashtable, null);
                if (array != null) UpdateUI(array, reqInstanceID);
            }
            else if (result.fatal)
            {
                SparxHub.Instance.FatalError(result.localizedError);
            }
            else
            {
                EB.Debug.LogError("Fetch Data Error");
            }
        }

        private void UpdateUI(ArrayList array, int reqInstanceID)
        {
            List<HonorArenaItemData> rankdatas = new List<HonorArenaItemData>();
            long localPlayerId = LoginManager.Instance.LocalUserId.Value;
            HonorArenaItemData m_localPlayerRankData = null;
            for (int i = 0; i < array.Count; i++)
            {
                var data = new HonorArenaItemData(array[i] as Hashtable, i,
                    reqInstanceID == allRequest.GetInstanceID());
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

            for (int i = rankdatas.Count; i < 4; i++)
            {
                rankdatas.Add(new HonorArenaItemData());
            }

            gridScroll.dataItems = rankdatas.ToArray();
            //更新排名数据
            if (m_localPlayerRankData != null)
            {
                if (reqInstanceID == selfRequest.GetInstanceID())
                {
                    LTUIUtil.SetText(SelfRankLabel, string.Format("{0}", m_localPlayerRankData.m_Rank + 1));
                    string forme = EB.Localizer.GetString("ID_LEGION_TECH_REWARDRATE");
                    LTUIUtil.SetText(OnHookincomeLabel,
                        string.Format(forme,
                            HonorArenaConfig.Instance.GetOneHourByReward(m_localPlayerRankData.m_Rank + 1)));
                    Messenger.Raise(EventName.HonorArenaRankChange, m_localPlayerRankData.m_Rank + 1);
                }
                else
                {
                    LTUIUtil.SetText(AllRankLabel, string.Format("{0}", m_localPlayerRankData.m_Rank + 1));
                }
            }
            else
            {
                if (reqInstanceID == selfRequest.GetInstanceID())
                {
                    LTUIUtil.SetText(SelfRankLabel, EB.Localizer.GetString("ID_ARENA_RANK_OUT_OF_RANGE"));
                }
                else
                {
                    LTUIUtil.SetText(AllRankLabel, EB.Localizer.GetString("ID_ARENA_RANK_OUT_OF_RANGE"));
                }
            }

        }

        protected virtual bool NeedRefreshData()
        {
            return true;
        }

        protected void FetchDataRemote(UIServerRequest serverRequest)
        {
            if (serverRequest != null)
            {
                LoadingSpinner.Show();
                serverRequest.SendRequest();
            }
        }
    }
}