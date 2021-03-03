using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTMonopolyInstanceHudController : LTGeneralInstanceHudController
    {
        public static void EnterInstance()
        {
            LTInstanceMapModel.Instance.RequestEnterMonopoly(delegate (bool notEmpty)
            {
                RequestInstanceCallBack(notEmpty);
            });
        }
        private static void RequestInstanceCallBack(bool notEmpty)
        {
            if (!notEmpty)
            {
                LTInstanceMapModel.Instance.ClearInstanceData();
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ACTIVITY_NOTSTARTED"));
                UIStack.Instance.HideLoadingScreen();
                return;
            }
            PlayerManagerForFilter.Instance.StopShowPlayerForGoToCombat();
            if (GameFlowControlManager.Instance != null)
            {
                GameFlowControlManager.Instance.SendEvent("GoToInstanceView");
            }
        }
        
        public UILabel GeneralDiceNumLabel;
        public UILabel GeneralDiceCostLabel;
        public UISprite GeneralDiceCostSprite;
        public UILabel SpecialDiceNumLabel;
        public UILabel SpecialDiceCostLabel;
        public UISprite SpecialDiceCostSprite;
        public UILabel RateOfProgressLabel;
        public UISlider Slider;
        public TouziTest DiceCom;
        public UILabel MapLabel;
        public UILabel NumLabel;

        private int curNum = -1;
        private int theLast = 0;
        private int numTemp = 0;
        private UITweener[] tweener;
        private LTInstanceNode curNode;
        private LTInstanceNode endNode;
        private List<Vector2> moveActionList;
        private bool showMove = false;

        private string DiceCost = "DiceCost";
        private string ExDiceCost = "ExDiceCost";
        private string DiceMaxTimes = "DiceMaxTimes";
        private string ExDiceMaxTimes = "ExDiceMaxTimes";
        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;

            GeneralDiceNumLabel = t.GetComponent<UILabel>("Edge/Bottom/Btn/Num");
            GeneralDiceCostLabel = t.GetComponent<UILabel>("Edge/Bottom/Btn/Cost");
            GeneralDiceCostSprite = t.GetComponent<UISprite>("Edge/Bottom/Btn/Cost/Icon");
            SpecialDiceNumLabel = t.GetComponent<UILabel>("Edge/Bottom/Btn (1)/Num");
            SpecialDiceCostLabel = t.GetComponent<UILabel>("Edge/Bottom/Btn (1)/Cost");
            SpecialDiceCostSprite = t.GetComponent<UISprite>("Edge/Bottom/Btn (1)/Cost/Icon");
            RateOfProgressLabel = t.GetComponent<UILabel>("Edge/Bottom/Back/Label");
            MapLabel = t.GetComponent<UILabel>("Edge/TopRight/MapPanel/Map/LevelLabel");
            NumLabel = t.GetComponent<UILabel>("Edge/PlayerPanel/Panel/Label");
            tweener = NumLabel.GetComponents<UITweener>();
            Slider = t.GetComponent<UISlider>("Edge/Bottom/Back");
            controller.FindAndBindingBtnEvent(new List<string>(3) { "Edge/Bottom/Btn", "Edge/Bottom/Btn (1)" },
                new List<EventDelegate>(3) { new EventDelegate(OnGeneralDiceBtnClick), new EventDelegate(OnSpecialDiceBtnClick)});
            controller.FindAndBindingCoolTriggerEvent(new List<string>(2) { "Edge/TopLeft/BackBtn", "Edge/Bottom/Back/Box" },
               new List<EventDelegate>(2) { new EventDelegate(OnCancelButtonClick), new EventDelegate(OnRewardBtnClick) });

            string itemStr = Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue(DiceCost);
            LTShowItemData item = GetItemData(itemStr);
            GeneralDiceCostLabel.text = item.count.ToString();
            GeneralDiceCostSprite.spriteName = BalanceResourceUtil.GetResSpriteName(item.id);

            itemStr = Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue(ExDiceCost);
            item = GetItemData(itemStr);
            SpecialDiceCostLabel.text = item.count.ToString();
            SpecialDiceCostSprite.spriteName = BalanceResourceUtil.GetResSpriteName(item.id);
        }

        public override IEnumerator OnAddToStack()
        {
            Hotfix_LT.Messenger.AddListener(EventName.OnLevelDataUpdate, OnLevelDataUpdateFunc);//更新层级事件
            yield return base.OnAddToStack();
            InitMap();
        }

        public override void OnFocus()
        {
            base.OnFocus();
            ResetLabelColor();
        }
        
        public override IEnumerator OnRemoveFromStack()
        {
            Hotfix_LT.Messenger.RemoveListener(EventName.OnLevelDataUpdate, OnLevelDataUpdateFunc);//更新层级事件
            ClearData();
            DestroySelf();
            yield break;
        }

        public override void OnCancelButtonClick()
        {
            LTInstanceMapModel.Instance.ClearInstanceData();
            if (LTInstanceMapModel.Instance.IsInsatnceViewAction())
            {
                LTInstanceMapModel.Instance.SwitchViewAction(false, true, delegate
                {
                    if (controller != null) controller.Close();
                });
            }
        }

        protected override void InitMap()
        {
            LTInstanceMapCtrl.EnterCallback = EnterCallback;
            LTInstanceMapCtrl.FloorDown = FloorDown;
            GlobalMenuManager.CurGridMap_MajorDataUpdateFunc();
        }

        protected override void InitUI()
        {
            InitGeneralBtn();
            InitSpecialBtn();
            InitSlider();
            InitMapName();
        }

        public override void OnFloorClickFunc(LTInstanceNode NodeData, Transform Target)
        {
            if (NodeData.RoleData != null && NodeData.RoleData.CampaignData != null && NodeData.RoleData.CampaignData.Bonus.Count > 1)
            {
                Hashtable data = Johny.HashtablePool.Claim();
                data.Add("data", NodeData.RoleData.CampaignData.Bonus);
                data.Add("tip", string.Format(EB.Localizer.GetString("ID_MONOPOLY_TIP2")));
                data.Add("title", string.Format(EB.Localizer.GetString("ID_MONOPOLY_TITLE2")));
                InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                GlobalMenuManager.Instance.Open("LTRewardShowUI", data);
            }
        }

        private void ClearData()
        {
            LTInstanceMapModel.Instance.ClearInstanceData();
            DataLookupsCache.Instance.CacheData("redeemerResult", null);
            numTemp = 0;
        }

        private void ResetLabelColor()
        {
            GeneralDiceCostLabel.color = (CheckRes(DiceCost, false)) ? LT.Hotfix.Utility.ColorUtility.WhiteColor : LT.Hotfix.Utility.ColorUtility.RedColor;
            SpecialDiceCostLabel.color = (CheckRes(ExDiceCost, false)) ? LT.Hotfix.Utility.ColorUtility.WhiteColor : LT.Hotfix.Utility.ColorUtility.RedColor;
        }

        protected void OnLevelDataUpdateFunc()
        {
            var tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetChallengeStyleById(LTInstanceMapModel.Instance.CurMapStyle);
            if (tpl != null)
            {
                MapCtrl.SetBGTexture(tpl.MapBg);
                MaskTex.spriteName = tpl.MapMask;
            }
        }

        //初始化设置
        private void EnterCallback()
        {
            bool bossRewardStop = false;
            LTInstanceMapModel.Instance.NodeDataHashDic.TryGetValue(LTInstanceMapModel.Instance.StartHash, out curNode);
            LTInstanceMapModel.Instance.NodeDataHashDic.TryGetValue(LTInstanceMapModel.Instance.DoorHash, out endNode);
            moveActionList = LTInstanceMapModel.Instance.GetRoad(curNode, endNode, false, ref bossRewardStop);
            InitUI();

            if (LTInstanceMapModel.Instance.CurNode.x != curNode.x || LTInstanceMapModel.Instance.CurNode.y != curNode.y)
            {
                showMove = true;
                MapCtrl.OnRealEnd = delegate { RealEnd(); };
                MapCtrl.OnDestinationMove(LTInstanceMapModel.Instance.CurNode, curNode, 999);
            }
            else if (LTInstanceMapModel.Instance.CurNode.x == endNode.x && LTInstanceMapModel.Instance.CurNode.y == endNode.y)
            {
                GotoFinish();
            }
            else
            {
                GetFreeDice();
                if (!showMove && moveActionList != null && moveActionList.Count > curNum)
                {
                    var vec2 = moveActionList[curNum];
                    Hotfix_LT.Instance.LTInstanceOptimizeManager.Instance.SetMoveDir(curNode, vec2);
                }
            }
        }

        private void GetFreeDice()
        {
            bool canget = LTInstanceMapModel.Instance.GetFreeDice();
            if (canget)
            {
                int count = (int)Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("FreeNormalDice");
                if (count > 0)
                {
                    LTInstanceMapModel.Instance.RequestFreeDice(delegate (Hashtable result)
                    {
                        if (result != null)
                        {
                            Hashtable data = Johny.HashtablePool.Claim();
                            data.Add("data", new List<LTShowItemData>() { new LTShowItemData("m-normal", count, LTShowItemType.TYPE_ACTICKET) });
                            data.Add("tip", string.Format(EB.Localizer.GetString("ID_MONOPOLY_FREE_REWARD_TIP")));
                            data.Add("title", string.Format(EB.Localizer.GetString("ID_EVERYDAY_REWARD")));
                            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                            GlobalMenuManager.Instance.Open("LTRewardShowUI", data);

                            InitGeneralBtn();
                        }
                        else
                        {
                            OverActivity();
                        }
                    });
                }
            }
        }

        private void OverActivity()
        {
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_uifont_in_LTLegionWarQualify_End_4"));
            OnCancelButtonClick();
        }

        private void FloorDown()
        {
            SetPlayerNumLabel();
        }

        private void SetPlayerNumLabel(int num=-1)
        {
            numTemp += num;
            if (numTemp >= 0)
            {
                NumLabel.text = numTemp.ToString();
                for (int i = 0; i < tweener.Length; ++i)
                {
                    tweener[i].ResetToBeginning();
                    tweener[i].PlayForward();
                }
            }
            else
            {
                numTemp = 0;
            }
        }

        private void InitGeneralBtn()
        {
            int num = LTInstanceMapModel.Instance.GetMonopolyGeneralDiceCount();
            if (num > 0)
            {
                GeneralDiceNumLabel.text = num.ToString();
                GeneralDiceCostLabel.gameObject.CustomSetActive(false);
            }
            else
            {
                int max = (int)Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue(DiceMaxTimes);
                num= max - LTInstanceMapModel.Instance.GetMonopolyGeneralDiceEXCount();
                string colorStr = num > 0 ? LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
                GeneralDiceNumLabel.text = string.Format("[{0}]{1}/{2}[-]", colorStr, num, max);
                GeneralDiceCostLabel.gameObject.CustomSetActive(true);
                GeneralDiceCostLabel.color = (CheckRes(DiceCost, false)) ? LT.Hotfix.Utility.ColorUtility.WhiteColor : LT.Hotfix.Utility.ColorUtility.RedColor;
            }

        }
        private void InitSpecialBtn()
        {
            int num = LTInstanceMapModel.Instance.GetMonopolySpecialDiceCount();
            if (num > 0)
            {
                SpecialDiceNumLabel.text = num.ToString();
                SpecialDiceCostLabel.gameObject.CustomSetActive(false);
            }
            else
            {
                int max = (int)Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue(ExDiceMaxTimes);
                num = max - LTInstanceMapModel.Instance.GetMonopolySpecialDiceEXCount();
                string colorStr = num > 0 ? LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
                SpecialDiceNumLabel.text = string.Format("[{0}]{1}/{2}[-]", colorStr, num, max);
                SpecialDiceCostLabel.gameObject.CustomSetActive(true);
                SpecialDiceCostLabel.color = (CheckRes(ExDiceCost, false)) ? LT.Hotfix.Utility.ColorUtility.WhiteColor : LT.Hotfix.Utility.ColorUtility.RedColor;
            }
        }

        private void InitSlider(bool isInit = true)
        {
            ArrayList list = Johny.ArrayListPool.Claim();
            DataLookupsCache.Instance.SearchDataByID<ArrayList>("monopoly.major.dices", out list);
            int curnum = 0;
            if (list != null)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    int cur =int.Parse(list[i].ToString());
                    curnum += cur;
                    if (i == list.Count - 1) theLast = cur;
                    if (isInit && moveActionList.Count > curnum)
                    {
                        var vec2 = moveActionList[curnum - 1];
                        HideFloorRole(vec2);
                    }
                }
            }
            if (isInit)
            {
                MapCtrl.RestSmallMap();
                curNum = curnum;
                SetCurNode();
                SetSlider();
            }
        }

        private void HideFloorRole(Vector2 vec2)
        {
            LTInstanceNode node = LTInstanceMapModel.Instance.GetNodeByPos((int)vec2.x, (int)vec2.y);
            if (node != null)
            {
                node.RoleData = new LTInstanceNodeRoleData();
                Instance.LTInstanceFloorTemp floorTmp = MapCtrl.GetNodeObjByPos(vec2) as Instance.LTInstanceFloorTemp;
                if (floorTmp != null)
                {
                    floorTmp.ClearRoleItem();
                }
            }
        }

        private void InitMapName()
        {
            int level = 1;
            DataLookupsCache.Instance.SearchIntByID("monopoly.level", out level);
            MapLabel.text =string.Format (EB.Localizer .GetString("ID_ROUND"), level);
        }

        public void OnGeneralDiceBtnClick()
        {
            if (showMove||MapCtrl.HasNext()|| (curNode.x == endNode.x && curNode.y == endNode.y)) return;
            bool isbuy = LTInstanceMapModel.Instance.GetMonopolyGeneralDiceCount()<=0;
            if (isbuy )
            {
                int max = (int)Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue(DiceMaxTimes);
                int num = max - LTInstanceMapModel.Instance.GetMonopolyGeneralDiceEXCount();
                if (num <= 0)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,EB.Localizer .GetString ("ID_BUY_TIMES_NOT_ENOUGH"));
                    return;
                }
                if (!CheckRes(DiceCost)) return;
            }
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            LTInstanceMapModel.Instance.RequestDice(LTInstanceConfig.MonopolyDice1, 0, isbuy, delegate(Hashtable result) {
                if (result!=null)
                {
                    InitSlider(false);
                    InitGeneralBtn();
                    LoadDice(theLast, ShowMove);
                }
                else
                {
                    OverActivity();
                }
            });
        }

        public void OnSpecialDiceBtnClick()
        {
            if (showMove || MapCtrl.HasNext() || (curNode.x == endNode.x && curNode.y == endNode.y)) return;
            bool isbuy = LTInstanceMapModel.Instance.GetMonopolySpecialDiceCount() <= 0;
            if (isbuy )
            {
                int max = (int)Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue(ExDiceMaxTimes);
                int num = max - LTInstanceMapModel.Instance.GetMonopolySpecialDiceEXCount();
                if (num <= 0)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_BUY_TIMES_NOT_ENOUGH"));
                    return;
                }
                if (!CheckRes(ExDiceCost)) return;
            }
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTMonopolyDiceUI");
        }

        public void OnRewardBtnClick()
        {
            int level = 1;
            DataLookupsCache.Instance.SearchIntByID("monopoly.level", out level);
            Data.MonopolyTemplate temp = Data.SceneTemplateManager.Instance.GetMonopolyById(level);
            if (temp != null)
            {
                Hashtable data = Johny.HashtablePool.Claim();
                data.Add("data", temp.Reward);
                data.Add("tip", string.Format(EB.Localizer.GetString("ID_MONOPOLY_TIP1")));
                data.Add("title", string.Format(EB.Localizer.GetString("ID_MONOPOLY_TITLE1")));
                InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                GlobalMenuManager.Instance.Open("LTRewardShowUI", data);
            }
      
        }

        public void OnDiceRequest(int Num)
        {
            bool isbuy = LTInstanceMapModel.Instance.GetMonopolySpecialDiceCount() <= 0;

            LTInstanceMapModel.Instance.RequestDice(LTInstanceConfig.MonopolyDice2,Num, isbuy,delegate (Hashtable result)
            {
                if (result != null)
                {
                    InitSlider(false);
                    InitSpecialBtn();
                    LoadDice(Num, ShowMove);
                }
                else
                {
                    OverActivity();
                }
            });
        }

        private void ShowMove(int num)
        {
            showMove = true;
            MapCtrl.OnRealEnd = delegate { RealEnd();};
            curNum += num;
            MapCtrl.OnDestinationMove(curNode, endNode, num);
            SetCurNode();
            SetSlider();
            SetPlayerNumLabel(num);
        }

        private void SetCurNode()
        {
            if (curNum == 0) return;
            int cur = Mathf.Min(curNum - 1, moveActionList.Count-1);
            var vec2 = moveActionList[cur];
            curNode = LTInstanceMapModel.Instance.GetNodeByPos((int)vec2.x, (int)vec2.y);
        }

        private void SetSlider()
        {
            int totle = moveActionList.Count;
            RateOfProgressLabel.text = string.Format("{0}/{1}", curNum, totle);
            Slider.value = (float)curNum / (float)totle;
        }

        private void RealEnd()
        {
            Hashtable data= Johny.HashtablePool.Claim();
            data.Add("x", curNode.x);
            data.Add("y", curNode.y);
            DataLookupsCache.Instance.CacheData("redeemerResult", null);
            LTInstanceMapModel.Instance.RequestMove(data, delegate(Hashtable result) 
            {
                if (result != null)
                {
                    if (curNode.x == endNode.x && curNode.y == endNode.y)
                    {
                        GotoFinish();
                    }
                    else
                    {
                        if (result != null)
                        {
                            ArrayList list = EB.Dot.Array("redeemerResult", result, null);
                            if (list != null)
                            {
                                List<LTShowItemData> dataList = new List<LTShowItemData>();
                                for (int i = 0; i < list.Count; i++)
                                {
                                    var wheelData = list[i];
                                    dataList.Add(new LTShowItemData(wheelData));
                                }
                                InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                                if (dataList.Count > 1)
                                {
                                    GlobalMenuManager.Instance.Open("LTShowBoxView", dataList);
                                }
                                else
                                {
                                    GlobalMenuManager.Instance.Open("LTShowRewardView", dataList);
                                }
                                HideFloorRole(new Vector2(curNode.x, curNode.y));
                            }
                        }
                    }
                    showMove = false;
                }
                else
                {
                    OverActivity();
                }
            });
        }

        private void GotoFinish()
        {
            LTInstanceMapModel.Instance.RequestFinishMonopoly(GotoNext);
        }
        
        private void GotoNext(Hashtable result)
        {
            if (result != null)
            {
                ArrayList list = EB.Dot.Array("redeemerResult", result, null);
                if (list != null)
                {
                    System.Action cb = delegate {
                        ClearData();
                        System.Action callBack = delegate {
                            MapCtrl.ReInit();
                            ClearData();
                            DataLookupsCache.Instance.CacheData("monopoly", null);
                            LTInstanceMapModel.Instance.RequestEnterMonopoly(delegate { InitMap();});
                        };
                        GlobalMenuManager.Instance.Open("LTCloudFXUI", callBack);
                    };

                    List<LTShowItemData> dataList = new List<LTShowItemData>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        var wheelData = list[i];
                        dataList.Add(new LTShowItemData(wheelData));
                    }

                    InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("reward", dataList);
                    ht.Add("callback", cb);
                    ht.Add("title", EB.Localizer.GetString("ID_MONOPOLY_BOX_REWRAD_TIP"));
                    GlobalMenuManager.Instance.Open("LTShowBoxView", ht);
                }
            }
            else
            {
                OverActivity();
            }
        }

        private LTShowItemData GetItemData(string strValue)
        {
            object obj = EB.JSON.Parse(strValue);
            return new LTShowItemData(obj);
        }

        private bool CheckRes(string str, bool showTip = true)
        {
            string itemStr = Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue(str);
            LTShowItemData item = GetItemData(itemStr);
            return CheckRes(item.id, item.count, showTip);
        }

        public bool CheckRes(string res,int cost, bool showTip = true)
        {
            if (res.Equals("hc"))
            {
                if (BalanceResourceUtil.GetUserDiamond() < cost)
                {
                    if(showTip) BalanceResourceUtil.HcLessMessage();
                    return false;
                }
            }
            else if (res.Equals("gold"))
            {
                if (BalanceResourceUtil.GetUserGold() < cost)
                {
                    if (showTip) BalanceResourceUtil.GoldLessMessage();
                    return false;
                }
            }
            return true;
        }

        #region 摇色相关
        private void LoadDice(int num, System.Action<int> callback)
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 3f);
            FusionAudio.PostEvent("UI/New/Dice", true);
            if(DiceCom != null)
            {
                InitDice(num, callback);
                return;
            }
            string path = "_GameAssets/Res/MISC/Dice/DiceObj";
            EB.Assets.LoadAsync(path, typeof(GameObject), o => 
            {
                if (!o) return;

                GameObject diceObj = GameObject.Instantiate(o) as GameObject;
                diceObj.transform.parent = controller.transform;
                diceObj.transform.localScale = Vector3.one;
                DiceCom = diceObj.GetComponent<TouziTest>();

                if (DiceCom != null)
                {
                    InitDice(num, callback);
                    return;
                }
            });
        }

        private void InitDice(int num, System.Action<int> callback)
        {
            DiceCom.InitDice(num - 1, delegate
            {
                FusionAudio.PostEvent("UI/New/Dice", false);
                InputBlockerManager.Instance.UnBlock(InputBlockReason.FUSION_BLOCK_UI_INTERACTION);
                callback(num);
            });
        }
        #endregion
    }
}
