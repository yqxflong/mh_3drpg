using System.Collections;
using System.Collections.Generic;
using EB.Sparx;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix_LT.UI
{
    public class LTHeroBattleListData
    {
        public int index; //服务器保存传递id
        public int showindex;
        public string Name;
        public List<string> emenyList;
        public int state; //0 lock;1 challenge;2 win;
        public int limitLevel;
        public string desc;
    }
    
    
    public class LTHeroBattleListItem:DynamicCellController<LTHeroBattleListData>
    {
        private static Vector3 newbieT = new Vector3(-526f,-110f,0f);
        private static Vector3 highT =  new Vector3(-526f,-140f,0f);
        
        public UILabel IndexNameLabel;
        public UIButton RuleBtn;
        public UILabel NameLabel;
        public List<LTNewHeroBattleEnemyItem> EmenyList;
        public UIButton ATKBtn; //挑战按钮
        public GameObject WinObj;
        public UILabel LockLabel;
        private LTHeroBattleListData itemData;

        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            IndexNameLabel = t.GetComponent<UILabel>("IndexLabel");
            NameLabel = t.GetComponent<UILabel>("NameLabel");
            RuleBtn = t.GetComponent<UIButton>("Rule");
            EmenyList = new List<LTNewHeroBattleEnemyItem>();
            EmenyList.Add(t.GetMonoILRComponent<LTNewHeroBattleEnemyItem>("Team/Item"));
            EmenyList.Add(t.GetMonoILRComponent<LTNewHeroBattleEnemyItem>("Team/Item (1)"));
            EmenyList.Add(t.GetMonoILRComponent<LTNewHeroBattleEnemyItem>("Team/Item (2)"));
            EmenyList.Add(t.GetMonoILRComponent<LTNewHeroBattleEnemyItem>("Team/Item (3)"));

            ATKBtn = t.GetComponent<UIButton>("Right/AtkBtn");
            WinObj = t.Find("Right/Win").gameObject;
            LockLabel = t.GetComponent<UILabel>("Right/Bg/Label");
            ATKBtn.onClick.Add(new EventDelegate(OnChallengeBtnClick));
            RuleBtn.onClick.Add(new EventDelegate(OnRuleClick));
        }

        private void OnRuleClick()
        {
            string text = itemData.desc;
            Messenger.Raise(EventName.HeroBattleShowDesc,text);
        }

        public void Clear()
        {
            itemData = null;
            mDMono.gameObject.CustomSetActive(false);
        }

        public override void Fill(LTHeroBattleListData itemData)
        {
            if (itemData==null)
            {
                Clear();
                return;
            }            
            
            this.itemData = itemData;
            NameLabel.text = itemData.Name;
            NameLabel.gameObject.CustomSetActive(!string.IsNullOrEmpty(itemData.Name));
            RuleBtn.gameObject.CustomSetActive(!string.IsNullOrEmpty(itemData.desc));
            if (string.IsNullOrEmpty(itemData.Name))
            {
                //第{0}组
                IndexNameLabel.transform.localPosition = highT;
                IndexNameLabel.text = string.Format(EB.Localizer.GetString("ID_HERO_GROUP_NAME"), itemData.showindex);
            }
            else
            {
                //第{0}关
                IndexNameLabel.transform.localPosition = newbieT;
                IndexNameLabel.text = string.Format(EB.Localizer.GetString("ID_LEVEL_TEXT_FORMAT"), itemData.showindex);
            }
            switch (itemData.state)
            {
                case 0: //lock
                    LockLabel.transform.parent.gameObject.CustomSetActive(true);
                    ATKBtn.gameObject.CustomSetActive(false);
                    WinObj.CustomSetActive(false);
                    // 通关前置关卡\n等级{0}级解锁
                    LockLabel.text = string.Format(EB.Localizer.GetString("ID_HERO_CHANGLLER_CONDITION"), itemData.limitLevel);
                    break;
                case 1: //challenge
                    if (BalanceResourceUtil.GetUserLevel()<itemData.limitLevel)
                    {
                        LockLabel.transform.parent.gameObject.CustomSetActive(true);
                        ATKBtn.gameObject.CustomSetActive(false);
                        WinObj.CustomSetActive(false);
                        LockLabel.text = string.Format(EB.Localizer.GetString("ID_HERO_CHANGLLER_CONDITION"), itemData.limitLevel);
                    }
                    else
                    {
                        LockLabel.transform.parent.gameObject.CustomSetActive(false);
                        ATKBtn.gameObject.CustomSetActive(true);
                        WinObj.CustomSetActive(false);
                    }
                    break;
                case 2: //win
                    LockLabel.transform.parent.gameObject.CustomSetActive(false);
                    ATKBtn.gameObject.CustomSetActive(false);
                    WinObj.CustomSetActive(true);
                    break;
            }
            
            for (int i = 0; i < itemData.emenyList.Count; i++)
            {
                EmenyList[i].Fill(itemData.emenyList[i]);
            }
            for (int i = itemData.emenyList.Count; i<EmenyList.Count; i++)
            {
                EmenyList[i].Clear();
            }
        }

        public override void Clean()
        {
            
        }
        
        public void OnChallengeBtnClick()
        {
            int Index = itemData.index;
            if (Index < 0) return;
            if (AutoRefreshingManager.Instance.GetRefreshed(AutoRefreshingManager.RefreshName.HeroBattleRefresh))
            {
                if(Index<100) Messenger.Raise(EventName.HeroBattleUpdateUI,2);
                return;
            }
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            System.Action startCombatCallback = delegate ()
            {
                if (LTNewHeroBattleManager.GetInstance().CurrentType!=HeroBattleType.High)
                {
                    List<int> temp= new List<int>();
                    foreach (var VARIABLE in LTFormationDataManager.Instance.HeroBattleTempPartner)
                    {
                        temp.Add(VARIABLE.Value);
                    }
                    LTNewHeroBattleManager.GetInstance().GetNewHeroBattleChallenge(temp,Index, delegate (bool isSuccessful) { });

                }
                else
                {
                    LTNewHeroBattleManager.GetInstance().GetNewHeroBattleChallenge(Index, delegate (bool isSuccessful) { });
                }
            };
            BattleReadyHudController.Open(eBattleType.HeroBattle, startCombatCallback);
        }
    }
}