using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GM.DataCache;
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTHeroBattleListCtrl: DynamicMonoHotfix
    {
        public LTNewHeroBattleHudController Controller;
        public UIConditionTabController categoryTabs;
        public GameObject[] lockSprites;
        public LTHeroBattleGridScrolll gridScroll;
    
        public override void Awake()
        {
            base.Awake();
            Controller = mDMono.transform.parent.GetUIControllerILRComponent<LTNewHeroBattleHudController>();
            categoryTabs = mDMono.transform.GetMonoILRComponent<UIConditionTabController>("ContentView");
            categoryTabs.TabLibPrefabs = new List<UITabControllerHotFix.TabLibEntry>();
            gridScroll = mDMono.transform.GetMonoILRComponent<LTHeroBattleGridScrolll>("ContentView/Views/Scroll View/Placeholder/Grid");
            
            InitUITab();
        }

        public void UpdateUI(int select)
        {
            categoryTabs.SelectTab(select);
            List<LTHeroBattleListData> list=GetChangllerList(select);
            gridScroll.SetItemDatas(list.ToArray());
            if (select<2)
            {
                int maxlay = LTNewHeroBattleManager.GetInstance().GetCurrentFinishLayer();
                int showStage = EventTemplateManager.Instance.GetNextHeroBattleData(maxlay).Stage;
                var VS=gridScroll.scrollView.GetComponent<UIPanel>().GetViewSize();
                //最大可见的物品数量
                int ActinCount =(int)( VS.y / gridScroll.mDMono.transform.GetComponent<UIGrid>().cellHeight);
                showStage = Math.Min(showStage-1, list.Count-1 - ActinCount);
                if (select < 1 && !GetIsLock(1))
                {
                    gridScroll.MoveTo(list.Count-1 - ActinCount);
                }
                else
                {
                    gridScroll.MoveTo(showStage);
                }
            }
        }
        

        private bool GetIsLock(int type)
        {
            HeroBattleType i = (HeroBattleType) type;
            return LTNewHeroBattleManager.GetInstance().GetHeroBattleTypeIsLock(i);
        }

        private void InitUITab()
        {
            UITabControllerHotFix.TabLibEntry entry = new UITabControllerHotFix.TabLibEntry();
            Transform mDMono =categoryTabs.mDMono.transform;
            lockSprites = new GameObject[3];
            lockSprites[0] = mDMono.transform.Find("UpButtons/ButtonGrid/Newbie/Lock").gameObject;
            lockSprites[1] = mDMono.transform.Find("UpButtons/ButtonGrid/Middle/Lock").gameObject;
            lockSprites[2] = mDMono.transform.Find("UpButtons/ButtonGrid/High/Lock").gameObject;
            for (int i = 0; i < lockSprites.Length; i++)
            {
                lockSprites[i].CustomSetActive(GetIsLock(i));
            }
            GameObject TabObj1 = mDMono.transform.Find("UpButtons/ButtonGrid/Newbie/Tab1").gameObject;
            GameObject PressedTabObj1 = mDMono.transform.Find("UpButtons/ButtonGrid/Newbie/Tab2").gameObject;
            GameObject GameViewObj1 = mDMono.transform.Find("UpButtons/ButtonGrid/Newbie/Newbie").gameObject;
            entry.TabObj = TabObj1;
            entry.PressedTabObj = PressedTabObj1;
            entry.GameViewObj = GameViewObj1;
            categoryTabs.TabLibPrefabs.Add(entry);
           
            TabObj1.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                // categoryTabs.OnTabPressed(TabObj1);
                // LTHeroBattleListData[] datas = GetChangllerList(0).ToArray();
                // gridScroll.SetItemDatas(datas);
                
                Controller.UpdateUI(0);
            }));

            if (!GetIsLock(1))
            {
                GameObject TabObj2 = mDMono.transform.Find("UpButtons/ButtonGrid/Middle/Tab1").gameObject;
                GameObject PressedTabObj2 = mDMono.transform.Find("UpButtons/ButtonGrid/Middle/Tab2").gameObject;
                GameObject GameViewObj2 = mDMono.transform.Find("UpButtons/ButtonGrid/Middle/Middle").gameObject;
                entry.TabObj = TabObj2;
                entry.PressedTabObj = PressedTabObj2;
                entry.GameViewObj = GameViewObj2;
                categoryTabs.TabLibPrefabs.Add(entry);
            
                TabObj2.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
                {
                    // categoryTabs.OnTabPressed(TabObj2);
                    // LTHeroBattleListData[] datas = GetChangllerList(1).ToArray();
                    // gridScroll.SetItemDatas(datas);
                    Controller.UpdateUI(1);
                }));
            }

            if (!GetIsLock(2))
            {
                GameObject TabObj3 = mDMono.transform.Find("UpButtons/ButtonGrid/High/Tab1").gameObject;
                GameObject PressedTabObj3 = mDMono.transform.Find("UpButtons/ButtonGrid/High/Tab2").gameObject;
                GameObject GameViewObj3 = mDMono.transform.Find("UpButtons/ButtonGrid/High/High").gameObject;
                entry.TabObj = TabObj3;
                entry.PressedTabObj = PressedTabObj3;
                entry.GameViewObj = GameViewObj3;
                categoryTabs.TabLibPrefabs.Add(entry);
           
                TabObj3.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
                {
                    // categoryTabs.OnTabPressed(TabObj3);
                    // LTHeroBattleListData[] datas = GetChangllerList(2).ToArray();
                    // gridScroll.SetItemDatas(datas);
                    Controller.UpdateUI(2);
                }));
            }
         
        }
        
        private  List<LTHeroBattleListData> InitChallengeList()
        {
            int count = 0;
            IDictionary dic;
            DataLookupsCache.Instance.SearchDataByID<IDictionary>("userClashOfHeroes.opponent", out dic);
            List<int> indexs = new List<int>();
            List<bool> isFinishs = new List<bool>();
            List<List<string>> enemys = new List<List<string>>();
            if (dic != null)
            {
                foreach (DictionaryEntry data in dic)
                {
                    indexs.Add(int.Parse((string)data.Key));
                    isFinishs.Add(EB.Dot.Bool("dead", data.Value, false));
                    ArrayList list = Hotfix_LT.EBCore.Dot.Array("selectedHeroes", data.Value, null);
                    List<string> herosList = new List<string>();
                    for (var i = 0; i < list.Count; i++)
                    {
                        var value = list[i] as string;
                        herosList.Add(value);
                    }
                    enemys.Add(herosList);
                    count++;
                }
            }
            List<LTHeroBattleListData> datas = new List<LTHeroBattleListData>();
            for (int i = 0; i < 3; i++)
            {
                LTHeroBattleListData temp = new LTHeroBattleListData();
                temp.index = indexs[i];
                temp.showindex = i+1;
                temp.state = isFinishs[i] ? 2 : 1;
                temp.emenyList = enemys[i];
                datas.Add(temp);
            }
            return datas;
        }

        private List<LTHeroBattleListData> GetChangllerList(int type)
        {
            if (type == 2)
            {
                return InitChallengeList();
            }
            List<LTHeroBattleListData> datas = new List<LTHeroBattleListData>();
            
            List<HeroBattleTemplate> temp= EventTemplateManager.Instance.GetHeroBattleData();
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].Type == type+1)
                {
                    LTHeroBattleListData data = new LTHeroBattleListData();
                    data.desc = temp[i].Desc;
                    data.index = temp[i].Id;
                    data.showindex = temp[i].Stage;
                    data.limitLevel =temp[i].Condition;
                    if (LTNewHeroBattleManager.GetInstance().JudgeFinish(temp[i].Id))
                    {
                        data.state = 2;
                    }
                    else if (LTNewHeroBattleManager.GetInstance().JudgeIsLock(temp[i].Id,temp[i].Stage))
                    {
                        data.state = 0;
                    }
                    else
                    {
                        data.state = 1;
                    }
                    data.Name = temp[i].Name;
                    data.emenyList = temp[i].EnemyHeroConfig;
                    datas.Add(data);
                }
            }

            return datas;
        }
    }
}