using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class NationBattleEntryController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            ChatHint = t.FindEx("UIFrame/TopLeftAnchor/ChatButton/ChatHint").gameObject;
            CombatCountdownTimeLabel = t.GetComponent<UILabel>("Content/Corner/CombatTime/Countdown/TimeLabel");
            CombatOpenTimeLabel = t.GetComponent<UILabel>("Content/Corner/CombatTime/OpenStartTime/TimeLabel");

            TerritoryGOs = new GameObject[10];
            TerritoryGOs[0] = t.FindEx("Content/DistributionMap/Main (1)").gameObject;
            TerritoryGOs[1] = t.FindEx("Content/DistributionMap/Small (1)").gameObject;
            TerritoryGOs[2] = t.FindEx("Content/DistributionMap/Small (2)").gameObject;
            TerritoryGOs[3] = t.FindEx("Content/DistributionMap/Small (3)").gameObject;
            TerritoryGOs[4] = t.FindEx("Content/DistributionMap/Neutrality").gameObject;
            TerritoryGOs[5] = t.FindEx("Content/DistributionMap/Small (4)").gameObject;
            TerritoryGOs[6] = t.FindEx("Content/DistributionMap/Main (2)").gameObject;
            TerritoryGOs[7] = t.FindEx("Content/DistributionMap/Small (5)").gameObject;
            TerritoryGOs[8] = t.FindEx("Content/DistributionMap/Small (6)").gameObject;
            TerritoryGOs[9] = t.FindEx("Content/DistributionMap/Main (3)").gameObject;

            controller.backButton = t.GetComponent<UIButton>("UIFrame/TopLeftAnchor/CancelBtn");

            t.GetComponent<UIButton>("UIFrame/TopLeftAnchor/ChatButton").onClick.Add(new EventDelegate(OnChatBtnClick));
            t.GetComponent<UIButton>("UIFrame/TopLeftAnchor/FriendButton").onClick.Add(new EventDelegate(OnFriendBtnClick));
            t.GetComponent<UIButton>("Content/Corner/RuleBtn").onClick.Add(new EventDelegate(OnRuleBtnClick));
            t.GetComponent<UIButton>("Content/Corner/BattleRewardBtn").onClick.Add(new EventDelegate(OnBattleRewardBtnClick));
            t.GetComponent<UIButton>("Content/Corner/DonateListBtn").onClick.Add(new EventDelegate(OnDonateRankListClick));

            t.GetComponent<UIEventTrigger>("Content/DistributionMap/Main (1)").onClick.Add(new EventDelegate(() => OnTerritoryItemClick(t.FindEx("Content/DistributionMap/Main (1)").gameObject)));
            t.GetComponent<UIEventTrigger>("Content/DistributionMap/Small (1)").onClick.Add(new EventDelegate(() => OnTerritoryItemClick(t.FindEx("Content/DistributionMap/Small (1)").gameObject)));
            t.GetComponent<UIEventTrigger>("Content/DistributionMap/Small (2)").onClick.Add(new EventDelegate(() => OnTerritoryItemClick(t.FindEx("Content/DistributionMap/Small (2)").gameObject)));
            t.GetComponent<UIEventTrigger>("Content/DistributionMap/Small (3)").onClick.Add(new EventDelegate(() => OnTerritoryItemClick(t.FindEx("Content/DistributionMap/Small (3)").gameObject)));
            t.GetComponent<UIEventTrigger>("Content/DistributionMap/Neutrality").onClick.Add(new EventDelegate(() => OnTerritoryItemClick(t.FindEx("Content/DistributionMap/Neutrality").gameObject)));
            t.GetComponent<UIEventTrigger>("Content/DistributionMap/Small (4)").onClick.Add(new EventDelegate(() => OnTerritoryItemClick(t.FindEx("Content/DistributionMap/Small (4)").gameObject)));
            t.GetComponent<UIEventTrigger>("Content/DistributionMap/Main (2)").onClick.Add(new EventDelegate(() => OnTerritoryItemClick(t.FindEx("Content/DistributionMap/Main (2)").gameObject)));
            t.GetComponent<UIEventTrigger>("Content/DistributionMap/Small (5)").onClick.Add(new EventDelegate(() => OnTerritoryItemClick(t.FindEx("Content/DistributionMap/Small (5)").gameObject)));
            t.GetComponent<UIEventTrigger>("Content/DistributionMap/Small (6)").onClick.Add(new EventDelegate(() => OnTerritoryItemClick(t.FindEx("Content/DistributionMap/Small (6)").gameObject)));
            t.GetComponent<UIEventTrigger>("Content/DistributionMap/Main (3)").onClick.Add(new EventDelegate(() => OnTerritoryItemClick(t.FindEx("Content/DistributionMap/Main (3)").gameObject)));
            Instance = this;
        }



        [System.Serializable]
        public class NationOccupancyEntry
        {
            public string natinName;
            public UILabel percentLabel;
            public UIProgressBar progressBar;
        }

        public GameObject ChatHint;
        public NationOccupancyEntry[] OccupancyEntrys;
        public UILabel CombatCountdownTimeLabel;
        public UILabel CombatOpenTimeLabel;
        public LTShowItem[] CombatRewardShowItems;
        public GameObject[] TerritoryGOs;
        int SkipIndex = -1;
        public static NationBattleEntryController Instance;

        public override bool IsFullscreen()
        {
            return true;
        }


        public override void OnDestroy()
        {
            base.OnDestroy();
            Instance = null;
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);

            if (param != null)
            {
                SkipIndex = (int)param;
            }
            if (NationManager.Instance.List.Members.Count <= 0)
            {
                NationManager.Instance.GetInfo();
            }
        }

        public override IEnumerator OnAddToStack()
        {
            if (NationManager.Instance.BattleTimeConfig.GetIsAtBattleTime())
            {
                NationManager.Instance.BattleTimeConfig.InitCountDownTime();
                StartCoroutine(CountdownBattleEndTime());
                CombatOpenTimeLabel.transform.parent.gameObject.SetActive(false);
                CombatCountdownTimeLabel.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                LTUIUtil.SetText(CombatOpenTimeLabel, NationManager.Instance.BattleTimeConfig.TimeStr);
                CombatOpenTimeLabel.transform.parent.gameObject.SetActive(true);
                CombatCountdownTimeLabel.transform.parent.gameObject.SetActive(false);
            }
            yield return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }

        public override void OnFocus()
        {
            NationManager.Instance.GetTerritoryInfo(null);
            GameDataSparxManager.Instance.RegisterListener(NationManager.TerritoryDataId, OnTerritoryListListener);

            base.OnFocus();
        }

        public override void OnBlur()
        {
            GameDataSparxManager.Instance.UnRegisterListener(NationManager.TerritoryDataId, OnTerritoryListListener);

            base.OnBlur();
        }

        void OnTerritoryListListener(string path, INodeData data)
        {
            if (this == null)
                return;
            //StartCoroutine(WaitforUpdateUI(data));
            var list = data as NationTerritoryList;
            SetTerritoryStateData(list);
            for (int i = 0; i < list.TerritoryList.Length; ++i)
            {
                ShowTerritory(TerritoryGOs[i], list.TerritoryList[i]);
            }
            if (SkipIndex >= 0 && NationManager.Instance.TerritoryList.TerritoryList.Length > 0)
            {
                TerritoryData territoryData = NationManager.Instance.TerritoryList.TerritoryList[SkipIndex];
                if (territoryData.ADType != eTerritoryAttackOrDefendType.None)
                {
                    GlobalMenuManager.Instance.Open("LTNationBattleHudUI", territoryData);
                }
                SkipIndex = -1;
            }
        }

        public void SkipToBattleHudUI(int skipIndex)
        {
            TerritoryData territoryData = NationManager.Instance.TerritoryList.TerritoryList[skipIndex];
            if (territoryData.ADType != eTerritoryAttackOrDefendType.None)
            {
                GlobalMenuManager.Instance.Open("LTNationBattleHudUI", territoryData);
            }
        }

        IEnumerator WaitforUpdateUI(INodeData data)
        {
            while (NationManager.Instance.List.Members.Count <= 0)
            {
                yield return null;
            }
            SetOccupancy();

            var list = data as NationTerritoryList;
            SetTerritoryStateData(list);
            for (int i = 0; i < list.TerritoryList.Length; ++i)
            {
                ShowTerritory(TerritoryGOs[i], list.TerritoryList[i]);
            }
        }

        void SetOccupancy()
        {
            for (var i = 0; i < NationManager.Instance.List.Members.Count; i++)
            {
                var nation = NationManager.Instance.List.Members[i];
                int territory_count = System.Array.FindAll(NationManager.Instance.TerritoryList.TerritoryList, m => m.Owner == nation.Name).Length;
                nation.Occupancy = (float)territory_count / NationManager.Instance.TerritoryList.TerritoryList.Length;
            }

            for (var i = 0; i < OccupancyEntrys.Length; i++)
            {
                var entry = OccupancyEntrys[i];
                NationData data = NationManager.Instance.List.Find(entry.natinName);
                LTUIUtil.SetText(entry.percentLabel, (data.Occupancy * 100) + "%");
                entry.progressBar.value = data.Occupancy;
            }
        }

        void SetTerritoryStateData(NationTerritoryList territoryList)
        {
            string myNationName = NationManager.Instance.Account.NationName;
            for (int index = 0; index < territoryList.TerritoryList.Length; ++index)
            {
                TerritoryData curData = territoryList.TerritoryList[index];
                if (curData.HP <= 0)
                    curData.HP = NationManager.Instance.Config.TerritoryConfigs[index].MaxHp;
                if (curData.HP <= 0)
                   EB.Debug.LogError("hp<=0 for territoryindex:{0}",index);
                curData.MaxHP = NationManager.Instance.Config.TerritoryConfigs[index].MaxHp;
                curData.ADType = eTerritoryAttackOrDefendType.None;
                curData.Index = index;
                if (curData.State == eTerritoryState.Capture)
                {
                    curData.ADType = eTerritoryAttackOrDefendType.None;
                }
                else if (curData.State == eTerritoryState.Open && System.Array.Find(curData.CanAttackArr, d => d == myNationName) != null)
                {
                    curData.ADType = eTerritoryAttackOrDefendType.Attack;
                }
                else if (curData.State == eTerritoryState.Open && System.Array.Find(curData.CanDefendArr, d => d == myNationName) != null)
                {
                    curData.ADType = eTerritoryAttackOrDefendType.Defend;
                }

                //if (curData.Owner == myNationName)  //自己国家只能防守
                //{
                //	if (curData.State == eTerritoryState.Close||curData.State==eTerritoryState.Occupied||curData.State==eTerritoryState.Invincible)
                //	{
                //		curData.ADType = eTerritoryAttackOrDefendType.None;
                //	}
                //	else if (curData.State == eTerritoryState.Open)
                //	{
                //		int[] nearbyIndexs = NationUtil.GetTerritotyMap(index);
                //		bool isNearbyOther = false;
                //		foreach (var nearbyIndex in nearbyIndexs)
                //		{
                //			TerritoryData nearbydata = territoryList.TerritoryList[nearbyIndex];
                //			if (myNationName != nearbydata.Owner && nearbydata.Owner != eTerritoryType.Neutral.ToString())
                //			{
                //				isNearbyOther = true;
                //				break;
                //			}
                //		}
                //		curData.ADType = isNearbyOther == true ? eTerritoryAttackOrDefendType.Defend : eTerritoryAttackOrDefendType.None;
                //	}				
                //}
                //else
                //{
                //	if (curData.State == eTerritoryState.Open)
                //	{
                //		int[] nearbyIndexs = NationUtil.GetTerritotyMap(index);
                //		bool isNearbyI = false;
                //		foreach (var nearbyIndex in nearbyIndexs)
                //		{
                //			TerritoryData nearbydata = territoryList.TerritoryList[nearbyIndex];
                //			if (myNationName == nearbydata.Owner)
                //			{
                //				isNearbyI = true;
                //				break;
                //			}
                //		}
                //		curData.ADType = isNearbyI == true ? eTerritoryAttackOrDefendType.Attack : eTerritoryAttackOrDefendType.None;
                //	}				
                //}
            }
        }

        void ShowTerritory(GameObject territoryGO, TerritoryData territoryData)
        {
            //List<GameObject> roofGOs = new List<GameObject>();
            //List<GameObject> roofSmallGOs = new List<GameObject>();
            //List<GameObject> gemGOs = new List<GameObject>();
            //foreach (Transform t in territoryGO.transform)
            //{
            //	if (t.gameObject.name == "RoofFlag")
            //		roofGOs.Add(t.gameObject);
            //	else if (t.gameObject.name == "RoofFlagSmall")
            //		roofSmallGOs.Add(t.gameObject);
            //	else if (t.gameObject.name == "LingFlag")
            //		gemGOs.Add(t.gameObject);
            //}
            //roofGOs.ForEach(go=>go.GetComponent<UISprite>().spriteName = NationUtil.NationRoofFlag(territoryData.Owner));
            //roofSmallGOs.ForEach(go => go.GetComponent<UISprite>().spriteName = NationUtil.NationRoofFlagSmall(territoryData.Owner));
            //gemGOs.ForEach(go => go.GetComponent<UISprite>().spriteName = NationUtil.NationGemFlag(territoryData.Owner));
            if (territoryData.Index == 0 || territoryData.Index == 6 || territoryData.Index == 9)
            {
                territoryGO.GetComponent<UISprite>().spriteName = NationUtil.NationBuildIcon(territoryData.Owner, "Zhong");
            }
            else if (territoryData.Index == 4)
            {
                territoryGO.GetComponent<UISprite>().spriteName = NationUtil.NationBuildIcon(territoryData.Owner, "Da");
            }
            else
            {
                territoryGO.GetComponent<UISprite>().spriteName = NationUtil.NationBuildIcon(territoryData.Owner, "Xiao");
            }

            string battleFlagSprite = "";
            if (territoryData.ADType == eTerritoryAttackOrDefendType.Attack)
                battleFlagSprite = "Country_Icon_Jinggong";
            else if (territoryData.ADType == eTerritoryAttackOrDefendType.Defend)
                battleFlagSprite = "Country_Icon_Fangshou";
            if (!string.IsNullOrEmpty(battleFlagSprite))
            {
                Transform flagTrans = territoryGO.transform.Find("Flag/BattleFlag");
                flagTrans.GetComponent<UISprite>().spriteName = battleFlagSprite;
                flagTrans.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                Transform flagTrans = territoryGO.transform.Find("Flag/BattleFlag");
                flagTrans.transform.parent.gameObject.SetActive(false);
            }

            UILabel cityNameLabel = territoryGO.transform.Find("CityName").GetComponent<UILabel>();
            LTUIUtil.SetText(cityNameLabel, EB.Localizer.GetString(territoryData.CityName));
            cityNameLabel.color = territoryData.Owner == NationManager.Instance.Account.NationName ? LT.Hotfix.Utility.ColorUtility.GreenColor : Color.white;
            UIProgressBar hpProgressBar = territoryGO.GetComponentInChildren<UIProgressBar>();
            float hpPercent = (float)territoryData.HP / territoryData.MaxHP;
            if (hpPercent > 1)
            {
               EB.Debug.LogError("hpPercent>1 for territoryIndex:{0}",territoryData.Index);
                hpPercent = 1;
            }
            hpProgressBar.value = hpPercent;
            UISprite hpBarForeground = territoryGO.transform.Find("HP/Foreground").GetComponent<UISprite>();
            hpBarForeground.color = territoryData.Owner == NationManager.Instance.Account.NationName ? new Color32(95, 224, 56, 255) : new Color32(226, 8, 35, 255);
            territoryGO.transform.Find("SelfFlag").gameObject.SetActive(territoryData.Owner == NationManager.Instance.Account.NationName);

            Transform LeaderPosTable = territoryGO.transform.Find("LeaderPosTable");
            for (var i = 0; i < LeaderPosTable.childCount; i++)
            {
                LeaderPosTable.GetChild(i).gameObject.SetActive(false);
            }
            foreach (KeyValuePair<int, int> leaderData in territoryData.LeaderPositionDic)
            {
                Transform pt = LeaderPosTable.Find(leaderData.Key.ToString());
                if (pt == null)
                {
                   EB.Debug.LogError(string.Format("posTrans is null for territoryIndex:{0},position:{1}", territoryData.Index, leaderData.Key));
                    continue;
                }
                LTUIUtil.SetText(pt.GetComponentInChildren<UILabel>(), leaderData.Value.ToString());
                pt.gameObject.SetActive(true);
            }
            LeaderPosTable.GetComponent<UIGrid>().Reposition();
        }

        IEnumerator CountdownBattleEndTime()
        {
            while (true)
            {
                bool isEnd;
                LTUIUtil.SetText(CombatCountdownTimeLabel, NationManager.Instance.BattleTimeConfig.GetCountDownStr(out isEnd));
                if (isEnd)
                {
                    GlobalMenuManager.Instance.Open("LTNationBattleResultUI");
                    yield break;
                }
                yield return new WaitForSeconds(1f);
            }
        }

        public void OnChatBtnClick()
        {
            GlobalMenuManager.Instance.Open("ChatHudView");
            ChatHint.SetActive(false);
        }

        private void OnMessages(EB.Sparx.ChatMessage[] msgs)
        {
            if (ChatHint != null)
                ChatHint.SetActive(true);
        }

        public void OnFriendBtnClick()
        {
            GlobalMenuManager.Instance.Open("FriendHud");
            ChatHint.SetActive(false);
        }

        public void OnRuleBtnClick()
        {
            string text = EB.Localizer.GetString("ID_NATION_BATTLE_RULE");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text);
        }

        public void OnTerritoryItemClick(GameObject go)
        {
            if (AllianceUtil.GetIsInTransferDart(""))
            {
                return;
            }

            int index = System.Array.IndexOf(TerritoryGOs, go);
            TerritoryData territoryData = NationManager.Instance.TerritoryList.TerritoryList[index];
            if (territoryData.ADType != eTerritoryAttackOrDefendType.None)
            {
                GlobalMenuManager.Instance.Open("LTNationBattleHudUI", territoryData);
            }
        }

        public void OnDonateRankListClick()
        {
            GlobalMenuManager.Instance.Open("LTNationBattleDonateListUI");
        }

        public void OnBattleRewardBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTNationBattleRewardUI");
        }
    }
}
