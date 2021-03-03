using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTMainInstanceBlitzCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            DynamicScroll = t.GetMonoILRComponent<LTMainInstanceBlitzGridScroll>("Scroll/PlaceHolder/Grid");
            RequestItemObj = t.FindEx("Scroll/PlaceHolder/RequestItem").gameObject;
            RequestItem = t.GetMonoILRComponent<LTShowItem>("Scroll/PlaceHolder/RequestItem/GameItem");
            NameLabel = t.GetComponent<UILabel>("Scroll/PlaceHolder/RequestItem/GameItem/Name");
            Countlab = t.GetComponent<UILabel>("Scroll/PlaceHolder/RequestItem/CountLab");
            RequestNumLab = t.GetComponent<UILabel>("Scroll/PlaceHolder/RequestItem/RequestNum");
            controller.backButton = t.GetComponent<UIButton>("Bg/Top/CancelBtn");

            t.GetComponent<UIButton>("Btn").onClick.Add(new EventDelegate(OnBtnClick));

        }
        public LTMainInstanceBlitzGridScroll DynamicScroll;
    
        public GameObject RequestItemObj;
        public LTShowItem RequestItem;
        public UILabel NameLabel;
        public UILabel Countlab;
        public UILabel RequestNumLab;
    
        private List<LTMainInstanceBlitzData> DataList;
    
        private int BlitzNum = 0;
    
        private string TargetItemId = string.Empty;
    
        private int moveCount;
    
        public override bool IsFullscreen()
        {
            return false;
        }
    
        public override bool ShowUIBlocker
        {
            get
            {
                return true;
            }
        }
    
        public override void SetMenuData(object param)
        {
            Hashtable data = param as Hashtable;
            if (data != null)
            {
                DataList = data["list"] as List<LTMainInstanceBlitzData>;
                BlitzNum = (int)data["num"];
                TargetItemId = EB.Dot.String("ItemId", data, string.Empty);
            }

        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            RequestItemObj.CustomSetActive(false);
            InitUI();
            //EventManager.instance.Raise(new OnPartnerEquipChange());//装备数量发生变化需要通知发送下
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            LTMainHudManager.Instance.CheckLevelUp();
            DestroySelf();
            isNotCallBack = false;
            clickTime = 0;
            TargetItemId = string.Empty;
            yield break;
        }
    
        private void InitUI()
        {
            if (DataList == null || DataList.Count <= 0)
            {
                return;
            }
            //DynamicScroll.Clear();
            DynamicScroll.SetItemDatas (  DataList.ToArray());
    
            moveCount = DataList.Count;
            if (!string.IsNullOrEmpty(TargetItemId) && LTPartnerDataManager.Instance.itemNeedCount > 0)
            {
                moveCount += 1;
                //EventManager.instance.Raise(new OnPartnerUIRefresh(CultivateType.UpGrade));
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerUIRefresh, CultivateType.UpGrade);
                StartCoroutine(InitRequestItem());
            }
    
            //if (DataList.Count > 1)
            //{
                RequestItemObj.CustomSetActive(false);
                StartCoroutine(SetScrollState());
            //}
    
            LTMainHudManager.Instance.CheckLevelUp();
        }
    
        public WaitForSeconds WaitTime = new WaitForSeconds(0.12f);
    
        private IEnumerator SetScrollState()
        {
            yield return WaitTime;
            Hotfix_LT.Messenger.Raise(EventName.LTBlitzCellTweenEvent, 1);
            for (int i = 0; i < moveCount - 1; i++)
            {
                yield return WaitTime;
                DynamicScroll.MoveInternalNow(i);
                if (i == moveCount - 2 && !string.IsNullOrEmpty(TargetItemId)&& LTPartnerDataManager.Instance.itemNeedCount > 0)
                {
                    RequestItemObj.CustomSetActive(true);
                    DynamicScroll.mDMono.GetComponent<UIGrid>().Reposition();
                }
                Hotfix_LT.Messenger.Raise(EventName.LTBlitzCellTweenEvent, i+2);
            }
        }
    
        private IEnumerator InitRequestItem()
        {
            //RequestItemObj.CustomSetActive(true);
            RequestItemObj.transform.SetParent(DynamicScroll.mDMono.transform);
    
            LTShowItemData data = new LTShowItemData(TargetItemId, 0, "gaminventory", false);
    
            for (int i = 0; i < DataList.Count; i++)
            {
                for (int j = 0; j < DataList[i].ItemList.Count; j++)
                {
                    if (DataList[i].ItemList[j].id == TargetItemId)
                    {
                        if (data.type == string.Empty)
                        {
                            data.type = DataList[i].ItemList[j].type;
                        }
                        data.count += DataList[i].ItemList[j].count;
                        break;
                    }
                }
            }
    
            RequestItem.LTItemData = data;
            NameLabel.color = LT.Hotfix.Utility.ColorUtility.BrownColor;
            Countlab.text = string.Format("*{0}", data.count); 
            int curCount = GameItemUtil.GetInventoryItemNum(TargetItemId);
            int needCount = LTPartnerDataManager.Instance.itemNeedCount;
            RequestNumLab.text = LT.Hotfix.Utility.ColorUtility.FormatEnoughStr(curCount, needCount);
    
            yield return null;
            RequestItemObj.transform.SetAsLastSibling();//不等一帧的话这个操作不会成功
            DynamicScroll.mDMono.GetComponent<UIGrid>().Reposition();
        }
    
        private bool isNotCallBack = false;
        private float oneTime = 0.8f;
        private float tenTime = 2f;
        private float clickTime;
        public void OnBtnClick()
        {
            if (LTMainInstanceCampaignCtrl.CampaignId > 0 && DataList != null && DataList.Count > 0)
            {
    
                int vaildTimes = IsVigorEnough(BlitzNum);
                if (vaildTimes <= 0)
                {
                    BalanceResourceUtil.TurnToVigorGotView();
                    return;
                }
    
                if (!IsCouldClick() || isNotCallBack)
                {
                    return;
                }
    
                isNotCallBack = true;
                LTInstanceMapModel.Instance.RequestMainBlitzCampaign(LTMainInstanceCampaignCtrl.CampaignId, vaildTimes, delegate 
                {
                    StartCoroutine(ResetScroll());
                    //EventManager.instance.Raise(new OnPartnerEquipChange());//装备数量发生变化需要通知发送下
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);
                });
            }
        }
    
        private IEnumerator ResetScroll()
        {
            DataList = LTInstanceUtil.GetBlitzData();
            DynamicScroll.Clear();
            /*TweenScale[] tsArray = DynamicScroll.GetComponentsInChildren<TweenScale>(true);
            for (int i = 0; i < tsArray.Length; i++)
            {
                tsArray[i].enabled = true;
                tsArray[i].ResetToBeginning();
            }*/
            yield return null;
            InitUI();
            isNotCallBack = false;
        }
    
        private int IsVigorEnough(int times)
        {
            int curVigor = 0;
            DataLookupsCache.Instance.SearchIntByID("res.vigor.v", out curVigor);
            Hotfix_LT.Data.LostMainCampaignsTemplate mainTpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainCampaignTplById(LTMainInstanceCampaignCtrl.CampaignId.ToString());
            if (curVigor >= (mainTpl.CostVigor * times))
            {
                return times;
            }
            else
            {
                return curVigor / mainTpl.CostVigor;
            }
        }
    
        private bool IsCouldClick()
        {
            float tempTime = BlitzNum == 1 ? oneTime : tenTime;
            if (Time.time - clickTime < tempTime)
            {
                return false;
            }
            clickTime = Time.time;
            return true;
        }
    }
}
