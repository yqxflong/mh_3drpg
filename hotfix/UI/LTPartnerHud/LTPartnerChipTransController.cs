using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = EB.Debug;

namespace Hotfix_LT.UI
{
    public class LTPartnerChipTransController : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            UseNum = t.GetComponent<UILabel>("DataPanel/Use/Adjust/UseNum");
            SourceItem = t.GetMonoILRComponent<LTShowItem>("DataPanel/Item/SourceIcon");
            DesItem = t.GetMonoILRComponent<LTShowItem>("DataPanel/Item/DesIcon");
            PlusBtn = t.GetComponent<UISprite>("DataPanel/Use/Adjust/Add");
            SubBtn = t.GetComponent<UISprite>("DataPanel/Use/Adjust/Reduce");
            UseButton = t.GetComponent<UISprite>("DataPanel/Use/OkUse");
            SourceItemID = 0;
            DesItemID = 0;
            t.GetComponent<UIButton>("DataPanel/CloseBtn").onClick.Add(new EventDelegate(Close));
            t.GetComponent<UIButton>("DataPanel/Use/Adjust/Max").onClick.Add(new EventDelegate(SetUseNumMax));
            t.GetComponent<UIButton>("DataPanel/Use/OkUse").onClick.Add(new EventDelegate(RequestChipTrans));


            t.GetComponent<ContinuePressTrigger>("DataPanel/Use/Adjust/Reduce").m_CallBackPress.Add(new EventDelegate(SetUseNumSub));
            t.GetComponent<ContinuePressTrigger>("DataPanel/Use/Adjust/Add").m_CallBackPress.Add(new EventDelegate(SetUseNumPlus));

        }

        public override void Start()
        {
            base.Start();
            var t = mDMono.transform;
            t.GetComponent<UIPanel>().sortingOrder = t.parent.parent.GetComponent<UIPanel>().sortingOrder + 1;
            t.GetComponent<UIPanel>().depth = t.parent.parent.GetComponent<UIPanel>().depth + 100;
        }

        public UILabel UseNum;
    
        public LTShowItem SourceItem;
    
        public LTShowItem DesItem;
    
        public UISprite PlusBtn;
    
        public UISprite SubBtn;
    
        public UISprite UseButton;
    
        public int SourceItemID;
    
        public int DesItemID;
    
        private int sourceNum;
    
        private int desNum;
        
        private LTPartnerData PartnerData;

        public LTPartnerCultivateController _PartnerCtrl;

        public LTPartnerCultivateController PartnerCtrl
        {
            get
            {
                if (_PartnerCtrl==null)
                {
                    _PartnerCtrl = mDMono.transform.parent.parent
                        .GetMonoILRComponent<LTPartnerCultivateController>();
                }

                return _PartnerCtrl;
            }
        }
    
       
        public void InitWithPartnerData(LTPartnerData partnerData)
        {
            PartnerData = partnerData;
            DesItemID = partnerData.StatId;
            if(partnerData.HeroInfo.role_grade == (int)PartnerGrade.UR)
            {
                SourceItemID = 1427;
            }
            else
            {
                switch (partnerData.HeroInfo.char_type)
                {
                    case Hotfix_LT.Data.eRoleAttr.Feng:
                        SourceItemID = 1422;
                        break;
                    case Hotfix_LT.Data.eRoleAttr.Huo:
                        SourceItemID = 1421;
                        break;
                    case Hotfix_LT.Data.eRoleAttr.Shui:
                        SourceItemID = 1423;
                        break;
                }
            }          
            sourceNum = GameItemUtil.GetInventoryItemNum(SourceItemID);
            desNum = partnerData.ShardNum;
            SubBtn.GetComponent<UIButton>().tweenTarget = null;
            UseButton.GetComponent<UIButton>().tweenTarget = null;
            PlusBtn.GetComponent<UIButton>().tweenTarget = null;
            SourceItem.LTItemData = new LTShowItemData(SourceItemID.ToString(),sourceNum, LTShowItemType.TYPE_GAMINVENTORY, false);
            DesItem.LTItemData = new LTShowItemData(DesItemID.ToString(), sourceNum, LTShowItemType.TYPE_HEROSHARD, false);
            LTUIUtil.SetText(SourceItem.Count,sourceNum.ToString());
            LTUIUtil.SetText(DesItem.Count, desNum.ToString());
            SourceItem.Count.gameObject.CustomSetActive(true);
            DesItem.Count.gameObject.CustomSetActive(true);
            
            LTUIUtil.SetText(UseNum, Mathf.Min(sourceNum, BalanceResourceUtil.MaxNum).ToString());
            CheckPlusSubBtn();
        }
    
        public void RefreshUIInfo()
        {
            PartnerData = LTPartnerDataManager.Instance.GetPartnerByStatId(PartnerData.StatId);
            if (PartnerData.HeroInfo.role_grade == (int)PartnerGrade.UR)
            {
                SourceItemID = 1427;
            }
            else
            {
                switch (PartnerData.HeroInfo.char_type)
                {
                    case Hotfix_LT.Data.eRoleAttr.Feng:
                        SourceItemID = 1422;
                        break;
                    case Hotfix_LT.Data.eRoleAttr.Huo:
                        SourceItemID = 1421;
                        break;
                    case Hotfix_LT.Data.eRoleAttr.Shui:
                        SourceItemID = 1423;
                        break;
                }
            }
            sourceNum = GameItemUtil.GetInventoryItemNum(SourceItemID);
            desNum = PartnerData.ShardNum;
            SourceItem.LTItemData = new LTShowItemData(SourceItemID.ToString(), sourceNum, LTShowItemType.TYPE_GAMINVENTORY, false);
            DesItem.LTItemData = new LTShowItemData(DesItemID.ToString(), desNum, LTShowItemType.TYPE_GAMINVENTORY, false);
            LTUIUtil.SetText(SourceItem.Count, sourceNum.ToString());
            LTUIUtil.SetText(DesItem.Count, desNum.ToString());
            LTUIUtil.SetText(UseNum, Mathf.Min(sourceNum, BalanceResourceUtil.MaxNum).ToString());
            CheckPlusSubBtn();
            SourceItem.Count.gameObject.CustomSetActive(true);
            DesItem.Count.gameObject.CustomSetActive(true);
        }
    
        public void RequestChipTrans()
        {
            LTPartnerDataManager.Instance.ChipTrans(DesItemID, int.Parse(UseNum.text), ShowAward);
        }
    
        public void CheckPlusSubBtn()
        {
            int useNum = int.Parse(UseNum.text);
            if (sourceNum == 0)
            {
                SubBtn.color = new Color(255 / 255f, 0, 255 / 255f, 1);
                PlusBtn.color = new Color(255 / 255f, 0, 255 / 255f, 1);
                UseButton.color = Color.magenta;
                UseButton.GetComponent<BoxCollider>().enabled = false;
                return;
            }
            UseButton.color = Color.white;
            UseButton.GetComponent<BoxCollider>().enabled = true;
            PlusBtn.color = useNum >= Mathf.Min(sourceNum, BalanceResourceUtil.MaxNum) ? new Color(255 / 255f, 0, 255 / 255f, 1) : Color.white;
            SubBtn.color = useNum <= 0 ? new Color(255 / 255f, 0, 255 / 255f, 1) : Color.white;
        }
    
        private void ShowAward(Hashtable data)
        {
            if (data == null)
            {
                return;
            }
            if (data.ContainsKey("inventory"))
            {
                Hashtable allInventoryData = data["inventory"] as Hashtable;
                List<LTShowItemData> chips = new List<LTShowItemData>();
                var iter = allInventoryData.GetEnumerator();
                while (iter.MoveNext())
                {
                    string inventoryId = EB.Dot.String("inventory_id", iter.Value, string.Empty);
                    int inventoryFormerNum = 0;
                    DataLookupsCache.Instance.SearchIntByID("inventory." + inventoryId + ".num", out inventoryFormerNum);
                    int inventoryCurNumValue = EB.Dot.Integer("num", iter.Value, 0);
                    string economy_id = EB.Dot.String("economy_id", iter.Value, string.Empty);
                    if (inventoryCurNumValue > inventoryFormerNum)
                    {
                        LTShowItemData itemData = new LTShowItemData(economy_id, (inventoryCurNumValue - inventoryFormerNum), LTShowItemType.TYPE_HEROSHARD);
                        chips.Add(itemData);
                    }
                }
                if(chips.Count>0)GlobalMenuManager.Instance.Open("LTShowRewardView", chips);
    
                DataLookupsCache.Instance.CacheData(data);
                RefreshUIInfo();               
                Hotfix_LT.Messenger.Raise(EventName.OnRefreshPartnerCellRP, true, true);
            }
        }
    
        public void OnResponse(EB.Sparx.Response res)
        {
            LoadingSpinner.Hide();
            if (res.sucessful)
            {
                ShowAward(res.hashtable);
            }
            else if (res.fatal)
            {
                SparxHub.Instance.FatalError(res.localizedError);
            }
        }
    
        public void SetUseNumPlus()
        {
            int useNum = int.Parse(UseNum.text )+ 1;
            if (useNum >Mathf.Min( sourceNum, BalanceResourceUtil.MaxNum))
            {
                useNum = Mathf.Min(sourceNum, BalanceResourceUtil.MaxNum);
            }
            LTUIUtil.SetText(UseNum,useNum.ToString());
            if (useNum > 0)
            {
                SubBtn.color = Color.white;
                UseButton.color = Color.white;
                UseButton.GetComponent<BoxCollider>().enabled = true;
            }
    
            if (useNum >= Mathf.Min(sourceNum, BalanceResourceUtil.MaxNum))
            {
                PlusBtn.color = new Color(255 / 255f, 0, 255 / 255f, 1);
            }
        }
    
        public void SetUseNumMax()
        {
            int sourceNum = GameItemUtil.GetInventoryItemNum(SourceItemID);
            LTUIUtil.SetText(UseNum, Mathf.Min(sourceNum, BalanceResourceUtil.MaxNum).ToString());
            PlusBtn.color = new Color(255/255f, 0, 255/255f,1);
            int useNum = int.Parse(UseNum.text);
            if (useNum > 0)
            {
                SubBtn.color = Color.white;
                UseButton.color = Color.white;
                UseButton.GetComponent<BoxCollider>().enabled = true;
            }
        }
    
        public void SetUseNumSub()
        {
            int useNum = int.Parse(UseNum.text) - 1;
            if (useNum < 0)
            {
                useNum = 0;
            }
            LTUIUtil.SetText(UseNum, useNum.ToString());
            if (useNum < Mathf.Min(sourceNum, BalanceResourceUtil.MaxNum))
            {
                PlusBtn.color = Color.white;
            }
            if (int.Parse(UseNum.text) <= 0)
            {
                SubBtn.color = new Color(255 / 255f, 0, 255 / 255f, 1);
                UseButton.color = Color.magenta;
                UseButton.GetComponent<BoxCollider>().enabled = false;
            }
            else
            {
                SubBtn.color = Color.white;
            }
        }
    
        public void Close()
        {
            PartnerCtrl.InitStarUp();
                mDMono.gameObject.CustomSetActive(false);
        }
    }
}
