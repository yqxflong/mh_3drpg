using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class ShowUpgradeMaterialState : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            NumberLabel = t.Find("StateShow/Label").GetComponent<UILabel>();
            NumberLabelShader = t.Find("StateShow/Label/Label(Clone)").GetComponent<UILabel>();
            NumSlider = t.Find("StateShow/Sprite/Sprite").GetComponent<UISprite>();
            TotalNumBg = t.Find("StateShow/Sprite").GetComponent<UISprite>();
            ToGetButton = t.Find("StateShow/ToGetButton").GetComponent<ConsecutiveClickCoolTrigger>();
            ItemBtn = t.Find("Item").GetComponent<ConsecutiveClickCoolTrigger>();
            UnLockChapter = t.Find("StateShow/ToGetButton/Lock/Label").GetComponent<UILabel>();
            UnlockChapterName = t.Find("StateShow/ToGetButton/Lock/Label (1)").GetComponent<UILabel>();
            LockObj = t.Find("StateShow/ToGetButton/Lock").gameObject;
            VigourCost = t.Find("StateShow/ToGetButton/QuicklyGet/VigourNum").GetComponent<UILabel>();
            CenterShowLabel = t.Find("StateShow/ToGetButton/CenterLabel").GetComponent<UILabel>();
            QuicklyGet = t.Find("StateShow/ToGetButton/QuicklyGet").gameObject;
            Item = t.Find("Item").GetMonoILRComponent<LTShowItem>();
            RateObj = t.Find("Item/Rate").gameObject;
            RateLabel = t.Find("Item/Rate/Label").GetComponent<UILabel>();
            selectBoxTemplate = t.parent.parent.FindEx("golditemSelect");
        }

        private UILabel NumberLabel, NumberLabelShader, UnLockChapter, UnlockChapterName, VigourCost, CenterShowLabel, RateLabel;
        private UISprite NumSlider, TotalNumBg;
        private ConsecutiveClickCoolTrigger ToGetButton, ItemBtn;
        private LTShowItem Item;
        private GameObject LockObj, QuicklyGet, RateObj;

        private int materialNum, sweepsNumber, curId, curNeednum;
        private int showVigour, curVigour, totalvigour, costPerCam;
        private Hotfix_LT.Data.DropDataBase dropData;
        private string dropPath1, colorStr;
        private int m_StarNum;
        private bool m_IsComplete, isExtremityTrialDouble, isSceneDouble;
        private float rate = 1.0f, Exrate = 1.0f;
        private List<Data.DropDataBase> dropList;
        private Dictionary<string, LTShowItemData> rewardDic;
        private List<LTShowItemData> rewardList;
        //极限试炼相关
        private int dayDisCountTimes = 0;
        private int newVigor = 0;
        private int oldVigor = 0;

        public void Fill(int materialId, int NeedNum, bool isExtremityTrialDouble, bool isSceneDouble)
        {
            curId = materialId;
            curNeednum = NeedNum;
            this.isExtremityTrialDouble = isExtremityTrialDouble;
            this.isSceneDouble = isSceneDouble;
            Item.LTItemData = new LTShowItemData(materialId.ToString(),1, LTShowItemType.TYPE_GAMINVENTORY);
            if(ItemBtn.clickEvent != null)
            {
                ItemBtn.clickEvent.Clear();
            }
            ItemBtn.clickEvent.Add(new EventDelegate(OnClickItem));
            materialNum = GameItemUtil.GetInventoryItemNum(materialId);
            SetGoldMaterialSelect(materialId,materialNum,NeedNum);
            ToGetButton.GetComponent<UISprite>().spriteName = "Ty_Button_1";
            //进度条显示
            SetSliderState(materialNum, NeedNum);
            dropData = EconemyTemplateManager.Instance.GetItem(materialId).DropDatas[0];
            //下面处理右边按钮的显示状态
            if (materialNum >= NeedNum)
            {
                CenterShowLabel.gameObject.CustomSetActive(true);
                CenterShowLabel.text = CenterShowLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_PARTNER_UPGRADE_TIP_9");
                ToGetButton.GetComponent<UISprite>().color = new Color(1, 0, 1);
                ToGetButton.GetComponent<BoxCollider>().enabled = false;
                QuicklyGet.CustomSetActive(false);
                LockObj.CustomSetActive(false);
            }
            else
            {
                ToGetButton.GetComponent<UISprite>().color = new Color(1, 1, 1);
                ToGetButton.GetComponent<BoxCollider>().enabled = true;
                if (dropData == null) return;
                if (dropData.Type == DropType.ExtremityTrial)//途径为极限试炼
                {
                    SetExtremityTrialGet(materialId, NeedNum, isExtremityTrialDouble);

                }
                else if (dropData.Type == DropType.Scene)//途径为主线副本
                {
                    SetSceneGet(materialId, NeedNum,isSceneDouble);
                }

            }

        }


        /// <summary>
        /// 主线副本途径处理
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="NeedNum"></param>
        private void SetSceneGet(int materialId, int NeedNum, bool isDouble)
        {
            //双倍材料显示       
            rate = isDouble ? ActivityUtil.GetTimeLimitActivityMulWithoutReLog(1002) : 1.0f;
            if (isDouble)
            {
                RateObj.CustomSetActive(true);
                RateLabel.text = "×" + rate.ToString();
            }
            else
            {
                RateObj.CustomSetActive(false);
            }
            SceneDropData tempDrop = dropData as SceneDropData;
            dropPath1 = EconemyTemplateManager.Instance.GetItem(materialId).DropChickId1;
            string[] chapterId = dropPath1.Split(':')[1].Split('_');
            LostMainChapterTemplate chapterData = SceneTemplateManager.Instance.GetLostMainChatpterTplById(chapterId[0]);
            DataLookupsCache.Instance.SearchDataByID<int>(string.Format("userCampaignStatus.normalChapters.{0}.campaigns.{1}.star", chapterId[0], chapterId[1]), out m_StarNum);
            DataLookupsCache.Instance.SearchDataByID<bool>(string.Format("userCampaignStatus.normalChapters.{0}.campaigns.{1}.complete", chapterId[0], chapterId[1]), out m_IsComplete);
            if (BalanceResourceUtil.GetUserLevel() < chapterData.LevelLimit ||!LTInstanceUtil.GetIsChapterLimitConditionComplete(chapterData,out int num) || !LTInstanceUtil.IsPreChapterComplete(chapterId[0]) || !m_IsComplete)//等级未达到解锁条件，或者前置关卡未解锁
            {
                CenterShowLabel.gameObject.CustomSetActive(false);
                ToGetButton.GetComponent<UISprite>().spriteName = "Ty_Button_4";
                if (ToGetButton.clickEvent != null)
                {
                    ToGetButton.clickEvent.Clear();
                    ToGetButton.clickEvent.Add(new EventDelegate(ToCurMaxChapterTip));
                }
                QuicklyGet.CustomSetActive(false);
                LockObj.gameObject.CustomSetActive(true);
                UnLockChapter.text = UnLockChapter.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_PARTNER_UPGRADE_TIP_2");
                UnlockChapterName.text = UnlockChapterName.transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString("ID_PARTNER_UPGRADE_TIP_10"), chapterData.Name);

            }
            else if (m_IsComplete && m_StarNum < 3)
            {

                CenterShowLabel.gameObject.CustomSetActive(true);
                CenterShowLabel.text = CenterShowLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_PARTNER_UPGRADE_TIP_1");
                if (ToGetButton.clickEvent != null)
                {
                    ToGetButton.clickEvent.Clear();
                }
                ToGetButton.clickEvent.Add(new EventDelegate(() => GotoSceneChapter(int.Parse(chapterId[1]), materialId.ToString())));
                //ToGetButton.GetComponent<UISprite>().color = new Color(1, 1, 1);
                //ToGetButton.GetComponent<BoxCollider>().enabled = true;
                QuicklyGet.CustomSetActive(false);
                LockObj.gameObject.CustomSetActive(false);
            }
            else if (m_IsComplete && m_StarNum == 3)//可以扫荡
            {
                CenterShowLabel.gameObject.CustomSetActive(false);
                //ToGetButton.GetComponent<UISprite>().color = new Color(1, 1, 1);
                //ToGetButton.GetComponent<BoxCollider>().enabled = true;
                costPerCam = SceneTemplateManager.Instance.GetLostMainCampaignTplById(chapterId[1]).CostVigor;
                totalvigour = Mathf.CeilToInt((NeedNum - materialNum) / rate) * costPerCam;
                DataLookupsCache.Instance.SearchDataByID<int>("res.vigor.v", out curVigour);
                QuicklyGet.CustomSetActive(true);
                if (ToGetButton.clickEvent != null)
                {
                    ToGetButton.clickEvent.Clear();
                }
                if (curVigour >= totalvigour)
                {
                    sweepsNumber = Mathf.CeilToInt((NeedNum - materialNum) / (rate * 1));
                    ToGetButton.clickEvent.Add(new EventDelegate(() => OnQuicklyGetButtonClick(int.Parse(chapterId[1]))));
                    showVigour = totalvigour;
                    colorStr = LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal;
                }
                else if (curVigour < totalvigour && curVigour >= costPerCam)
                {
                    sweepsNumber = curVigour / costPerCam;
                    showVigour = sweepsNumber * costPerCam;
                    ToGetButton.clickEvent.Add(new EventDelegate(() => OnQuicklyGetButtonClick(int.Parse(chapterId[1]))));
                    colorStr = LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal;
                }
                else
                {
                    showVigour = totalvigour;
                    colorStr = LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
                    ToGetButton.clickEvent.Add(new EventDelegate(()=>BalanceResourceUtil.TurnToVigorGotView()));
                }

                VigourCost.text = VigourCost.transform.GetChild(0).GetComponent<UILabel>().text = string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat, colorStr, showVigour);
                LockObj.gameObject.CustomSetActive(false);
            }
        }


        //极限试炼获取途径处理
        private void SetExtremityTrialGet(int materialId, int NeedNum, bool isDouble)
        {

            Exrate = isDouble ? ActivityUtil.GetTimeLimitActivityMul(1003) : 1.0f;
            if (isDouble)
            {
                RateObj.CustomSetActive(true);
                RateLabel.text = "×" + Exrate.ToString();
            }
            else
            {
                RateObj.CustomSetActive(false);
            }
            //极限试炼分为可扫荡和不可扫荡
            //获取当最高通关卡
            int curlayer = GetExtremityTrialCurMaxLayer();
            dropList = Data.EconemyTemplateManager.Instance.GetItem(materialId).DropDatas;
            DataLookupsCache.Instance.SearchDataByID<int>("res.vigor.v", out curVigour);
            int approLayer = 1;
            if (GetApproLayer(curlayer, out approLayer))//可以扫荡，显示快速获取
            {
                CenterShowLabel.gameObject.CustomSetActive(false);
                QuicklyGet.CustomSetActive(true);
                LockObj.gameObject.CustomSetActive(false);
                sweepsNumber = Mathf.CeilToInt((NeedNum - materialNum) / (Exrate * 1));
                totalvigour = GetTotalNeedVigour(sweepsNumber);
                if (ToGetButton.clickEvent != null)
                {
                    ToGetButton.clickEvent.Clear();
                }
                if (curVigour >= totalvigour)
                {

                    ToGetButton.clickEvent.Add(new EventDelegate());
                    showVigour = totalvigour;
                    ToGetButton.clickEvent.Add(new EventDelegate(() => OnQuicklyGetExtremityTrialDrop(approLayer, sweepsNumber, () => ShowReward())));
                    colorStr = LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal;
                }
                else if (curVigour < newVigor || (dayDisCountTimes <= 0 && curVigour < oldVigor))//一次都不能扫荡的
                {
                    showVigour = totalvigour;
                    colorStr = LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
                    ToGetButton.clickEvent.Add(new EventDelegate(()=> BalanceResourceUtil.TurnToVigorGotView()));
                }
                else
                {
                    if (dayDisCountTimes > 0)
                    {
                        if (curVigour - dayDisCountTimes * newVigor >= 0)
                        {
                            sweepsNumber = dayDisCountTimes + Mathf.FloorToInt((curVigour - dayDisCountTimes * newVigor) / (float)oldVigor);
                            showVigour = dayDisCountTimes * newVigor + (sweepsNumber - dayDisCountTimes) * oldVigor;
                        }
                        else
                        {
                            sweepsNumber = Mathf.FloorToInt(curVigour / (float)newVigor);
                            showVigour = sweepsNumber * newVigor;

                        }

                    }
                    else
                    {
                        sweepsNumber = Mathf.FloorToInt(curVigour / (float)oldVigor);
                        showVigour = sweepsNumber * oldVigor;
                    }
                    ToGetButton.clickEvent.Add(new EventDelegate(() => OnQuicklyGetExtremityTrialDrop(approLayer, sweepsNumber, () => ShowReward())));
                    colorStr = LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal;
                }
                VigourCost.text = VigourCost.transform.GetChild(0).GetComponent<UILabel>().text = string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat, colorStr, showVigour);
            }
            else//无法扫荡显示前往获取
            {
                CenterShowLabel.gameObject.CustomSetActive(true);
                CenterShowLabel.text = CenterShowLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_PARTNER_UPGRADE_TIP_1");
                if (ToGetButton.clickEvent != null)
                {
                    ToGetButton.clickEvent.Clear();
                }
                ToGetButton.clickEvent.Add(new EventDelegate(() => dropData.GotoDrop()));
                //ToGetButton.GetComponent<UISprite>().color = new Color(1, 1, 1);
                //ToGetButton.GetComponent<BoxCollider>().enabled = true;
                QuicklyGet.CustomSetActive(false);
                LockObj.gameObject.CustomSetActive(false);
            }
        }

        //获取进度条显示
        private void SetSliderState(int materialNum, int NeedNum)
        {
            string NumColor = LT.Hotfix.Utility.ColorUtility.WhiteColorHexadecimal;
            string spriteName = "Ty_Strip_Blue";
            if (materialNum >= NeedNum)
            {
                materialNum = NeedNum;
                NumColor = LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal;
                spriteName = "Ty_Strip_Yellow";
            }
            NumberLabel.text = NumberLabelShader.text = string.Format("[{0}]{1}/{2}[-]", NumColor, materialNum, NeedNum);//显示数量
            if (materialNum <= 0)
            {
                NumSlider.gameObject.CustomSetActive(false);
            }
            else
            {
                NumSlider.gameObject.CustomSetActive(true);
                NumSlider.spriteName = spriteName;
                NumSlider.SetAnchor(TotalNumBg.gameObject, 5, 5, -5 - TotalNumBg.width * (NeedNum - materialNum) / NeedNum, -5);
            }
        }

        //获取极限试炼快速获取需要体力
        private int GetTotalNeedVigour(int neednum)
        {
            int totalTimes = 0;
            int totalvigor = 0;
            DataLookupsCache.Instance.SearchDataByID<int>("infiniteChallenge.info.currentTimes", out totalTimes);
            Data.NewGameConfigTemplateManager.Instance.GetEnterVigor(eBattleType.InfiniteChallenge, out dayDisCountTimes, out newVigor, out oldVigor);
            dayDisCountTimes = dayDisCountTimes - totalTimes;
            if (dayDisCountTimes > 0 && dayDisCountTimes >= neednum)
            {
                totalvigor = newVigor * neednum;
            }
            else if (dayDisCountTimes > 0 && dayDisCountTimes < neednum)
            {
                totalvigor = newVigor * dayDisCountTimes + (neednum - dayDisCountTimes) * oldVigor;
            }
            else
            {
                totalvigor = oldVigor * neednum;
            }
            return totalvigor;
        }


        //获取当前扫荡的极限试炼关卡
        private bool GetApproLayer(int curlayer, out int appprolayer)
        {
            bool canQuicklyGet = false;
            int Needlayer = 0;
            for (int i = 0; i < dropList.Count; i++)
            {
                if (dropList[i].Type == Data.DropType.ExtremityTrial)
                {
                    int layer = 99;
                    int.TryParse(dropList[i].Index1.Split('_')?[0],out layer);
                    //for (int j = (layer - 1) * 5 + 1; j < layer * 5 + 1; j++)
                    //{
                    //    if (j > curlayer)
                    //    {
                    //        break;
                    //    }
                    //    if (j <= curlayer && j > Needlayer)
                    //    {
                    //        Needlayer = j;
                    //        canQuicklyGet = true;
                    //    }
                    //}
                    Needlayer = layer * 5;
                    canQuicklyGet = curlayer >= Needlayer;

                }
            }
            appprolayer = Needlayer;
            return canQuicklyGet;
        }

        private int GetExtremityTrialCurMaxLayer()
        {
            int MaxComplete = 1;
            DataLookupsCache.Instance.SearchDataByID<int>("infiniteChallenge.info.currentlayer", out MaxComplete);
            bool isComplete = false;
            if (!DataLookupsCache.Instance.SearchDataByID<bool>("infiniteChallenge.info.isComplete", out isComplete) || !isComplete)
            {
                MaxComplete -= 1;
            }
            return MaxComplete;
        }

        //主线副本快速获取
        public void OnQuicklyGetButtonClick(int campaignId)
        {
            LTInstanceMapModel.Instance.RequestMainBlitzCampaign(campaignId, sweepsNumber, delegate
            {
                List<LTShowItemData> list = LTInstanceUtil.GetQuicklyBlitzData();
                GlobalMenuManager.Instance.Open("LTShowRewardView", list);
                //处理后续显示,以及伙伴界面刷新
                //Fill(curId, curNeednum);
                //controller.FillMaterial();
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerUIRefresh, CultivateType.UpGrade);

            });
        }
        //快速获取极限试炼材料
        public void OnQuicklyGetExtremityTrialDrop(int level, int SweepNum, System.Action callback)
        {

            for (int i = 0; i < SweepNum; i++)
            {
                if (i == SweepNum - 1)
                {
                    RequestSweepByIndex(level, delegate (Hashtable data)
                    {
                        DealData(data);
                        callback();
                    });
                }
                else
                {
                    RequestSweepByIndex(level, delegate (Hashtable data) { DealData(data); });

                }

            }

        }

        private void DealData(Hashtable data)
        {
            string id, type;
            int num;
            ArrayList list = Hotfix_LT.EBCore.Dot.Array("infiniteChallenge.sweepAward", data, null);
            if (list == null)
            {
                EB.Debug.LogError("None Reward Sweep infiniteChallenge");
                return;
            }
            if (rewardDic == null)
            {
                rewardDic = new Dictionary<string, LTShowItemData>();
            }
            for (int i = 0; i < list.Count; i++)
            {
                num = EB.Dot.Integer("quantity", list[i], 0);
                id = EB.Dot.String("data", list[i], null);
                type = EB.Dot.String("type", list[i], "gaminventory");
                if (num < 0) continue;
                if (rewardDic.ContainsKey(id))
                {
                    rewardDic[id].count += num;
                }
                else
                {
                    rewardDic.Add(id, new LTShowItemData(id, num, type));
                }
            }

        }

        //完成扫荡的回调
        private void ShowReward()
        {
            if (rewardList == null)
            {
                rewardList = new List<LTShowItemData>();
            }

            if (rewardDic != null)
            {
                foreach (var item in rewardDic)
                {
                    rewardList.Add(item.Value);
                }
            }

            Hashtable hashtable = Johny.HashtablePool.Claim();
            hashtable.Add("reward", rewardList);
            hashtable.Add("callback", new System.Action(ClearData));
            GlobalMenuManager.Instance.Open("LTShowRewardView", hashtable);
        }

        private void ClearData()
        {
            rewardList.Clear();
            rewardDic.Clear();
            Hotfix_LT.Messenger.Raise<CultivateType>(Hotfix_LT.EventName.OnPartnerUIRefresh, CultivateType.UpGrade);
        }
        public void RequestSweepByIndex(int level, System.Action<Hashtable> callback)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/infinitechallenge/startSweepByIndex");
            request.AddData("level", level);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                callback(data);
            });
        }

        private void GotoSceneChapter(int partID,string EconomyId)
        {
            Hashtable data = Johny.HashtablePool.Claim();
            data.Add("id", partID);
            data.Add("targetItemId", EconomyId);
            GlobalMenuManager.Instance.Open("LTMainInstanceCampaignView", data);
        }
    
        private void OnClickItem()
        {
            if (curId != 0)
            {
                LTResToolTipController.Show(LTShowItemType.TYPE_GAMINVENTORY, curId.ToString());
            }
        }
    
        //private void VigourBugTips()
        //{
        //    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, EB.Localizer.GetString("ID_PARTNER_UPGRADE_TIP_11"), delegate (int r)
        //    {
        //        if (r == 0)
        //        {
        //            GlobalMenuManager.Instance.Open("LTResourceShopUI");
        //        }
        //    });
        //}
    
        /// <summary>
        /// 通向主线副本当前解锁关卡
        /// </summary>
        private void ToCurMaxChapterTip()
        {
            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, EB.Localizer.GetString("ID_PARTNER_UPGRADE_TIP_12"), delegate (int r)
            {
                if (r == 0)
                {  
                    //直接跳转征战界面
                    GlobalMenuManager.Instance.Open("LTInstanceMapHud", null);
                }
            });
        }

        private List<SelectBox> selectBoxes;
        private Transform selectBoxTemplate;
        private QuicklyGetGoldItemController selectBoxContrl;
        private string BoxinventoryId;
        //自选金子界面显示
        private void SetGoldMaterialSelect(int itemId, int Itemamount, int Needamount)
        {
            if (selectBoxes == null)
            {
                selectBoxes = EconemyTemplateManager.Instance.GetSelectBoxListById(QuicklyGetUpgradeMaterialController.GoldBoxId);
            }
            if (Needamount > Itemamount)
            {
                var tempselectBox = selectBoxes.Find(m => m.ri1.Equals(itemId.ToString()));
                if (tempselectBox != null)
                {
                    int boxamount = GameItemUtil.GetInventoryItemNum(QuicklyGetUpgradeMaterialController.GoldBoxId,out BoxinventoryId);
                    if (boxamount > 0)
                    {
                        LTShowItemData data = tempselectBox == null ? null : new LTShowItemData(QuicklyGetUpgradeMaterialController.GoldBoxId, boxamount, LTShowItemType.TYPE_GAMINVENTORY);
                        setSelectBoxState(true, data, tempselectBox.index);
                        return;
                    }
                }
            }
            setSelectBoxState(false, null,0);
        }

        private void setSelectBoxState(bool isShow, LTShowItemData item, int index)
        {
            if (isShow)
            {
                selectBoxContrl = selectBoxContrl ?? mDMono.transform.GetMonoILRComponent<QuicklyGetGoldItemController>("golditemSelect(Clone)", false);
                if(selectBoxContrl == null)
                {
                    Transform selectBox;
                    selectBox = GameObject.Instantiate(selectBoxTemplate, mDMono.transform);
                    selectBoxContrl = selectBox.GetMonoILRComponent<QuicklyGetGoldItemController>();
                    selectBox.localPosition = new Vector3(-888, 8, 0);
                    selectBox.localScale = Vector3.one;
                    selectBox.gameObject.CustomSetActive(true);
                }
                selectBoxContrl.Fill(item, delegate { UseItemBox(index); });
            }
            else
            {
                selectBoxContrl = selectBoxContrl ?? mDMono.transform.GetMonoILRComponent<QuicklyGetGoldItemController>("golditemSelect(Clone)", false);
                if (selectBoxContrl != null) selectBoxContrl.Fill(null, null);
            }

        }

        private void UseItemBox(int index)
        {
            LTPartnerDataManager.Instance.UseItem(BoxinventoryId, 1, index, 
                delegate {
                    Fill(curId,curNeednum,isExtremityTrialDouble,isSceneDouble);
                    //GlobalMenuManager.Instance.Open("LTShowRewardView", showItemsList);
                });
        }



    }
    
}
