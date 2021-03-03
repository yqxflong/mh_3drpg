using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 装备合成
/// </summary>
namespace Hotfix_LT.UI
{
    public class LTAwakeningGenericTransController : UIControllerHotfix
    {
        //原物品   合成物品
        public LTShowItem SourceItem, DesItem;
        //要合成的数量  合成提示（**合成1）
        public UILabel SourceItemLabel, DesItemLabel, TransNumLabel, TipLabel;

        public UISprite PlusBtn;
        public UISprite SubBtn;
        public UISprite UseButton;
        public UISprite MaxButton;
        //原物品数量
        private int sourceNum;
        //多少个合成1个
        private int transNum = 1;
        public override bool IsFullscreen() { return false; }
        public override bool ShowUIBlocker { get { return true; } }

        private int itemID;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            controller.backButton = t.GetComponent<UIButton>("Bg_Sprites/BG/Top/CloseBtn");

            SourceItem = t.GetMonoILRComponent<LTShowItem>("Content/Item/SourceIcon");
            DesItem = t.GetMonoILRComponent<LTShowItem>("Content/Item/DesIcon");
            SourceItemLabel = t.GetComponent<UILabel>("Content/Item/SourceIcon/Num");
            DesItemLabel = t.GetComponent<UILabel>("Content/Item/DesIcon/Num");
            TransNumLabel = t.GetComponent<UILabel>("Content/Use/Adjust/UseNum");
            TipLabel = t.GetComponent<UILabel>("Content/Item/Label");
            PlusBtn = t.GetComponent<UISprite>("Content/Use/Adjust/Add");
            SubBtn = t.GetComponent<UISprite>("Content/Use/Adjust/Reduce");
            UseButton = t.GetComponent<UISprite>("Content/Use/OkUse");
            MaxButton = t.GetComponent<UISprite>("Content/Use/Adjust/Max");

            t.GetComponent<UIButton>("Content/Use/Adjust/Max").onClick.Add(new EventDelegate(OnMaxBtnClick));
            t.GetComponent<UIButton>("Content/Use/OkUse").onClick.Add(new EventDelegate(OnTransBtnClick));

            t.GetComponent<ContinuePressTrigger>("Content/Use/Adjust/Reduce").m_CallBackPress.Add(new EventDelegate(OnSubBtnClick));
            t.GetComponent<ContinuePressTrigger>("Content/Use/Adjust/Add").m_CallBackPress.Add(new EventDelegate(OnPlusBtnClick));
        }
        /// <summary>
        /// 原物品的ID  进来要先判断物品是不是可以合成
        /// </summary>
        /// <param name="param"></param>
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            itemID = int.Parse((string)param);
            Refresh();
        }
    
        public void Refresh()
        {
            //原物品信息
            Hotfix_LT.Data.GeneralItemTemplate SourceItemDate = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetGeneral(itemID);
            sourceNum = GameItemUtil.GetInventoryItemNum(itemID);
    
            SourceItem.LTItemData = new LTShowItemData((itemID).ToString(), sourceNum, LTShowItemType.TYPE_GAMINVENTORY);
            //LTUIUtil.SetText(SourceItem.Count, sourceNum.ToString());
            SourceItemLabel.text = sourceNum.ToString();

            int DesItemID = 0;
            int.TryParse(SourceItemDate.CompoundItem, out DesItemID);
            if (CheckCanCompound(DesItemID))
            {
                int desNum = GameItemUtil.GetInventoryItemNum(DesItemID);
                DesItem.LTItemData = new LTShowItemData(DesItemID.ToString(), desNum, LTShowItemType.TYPE_GAMINVENTORY);
                //LTUIUtil.SetText(DesItem.Count, desNum.ToString());
                DesItemLabel.text = desNum.ToString();

                transNum = SourceItemDate.NeedNum;
                LTUIUtil.SetText(TransNumLabel, Mathf.Min(sourceNum / transNum, 1).ToString());
                LTUIUtil.SetText(TipLabel, string.Format("{0}{1}1", transNum, EB.Localizer.GetString("ID_AWAKENDUNGEON_TRANS")));
                CheckPlusSubBtn();
            }
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
        
        private void CheckPlusSubBtn()
        {
            int useNum = int.Parse(TransNumLabel.text);
            if (useNum == 0)
            {
                SubBtn.color = Color.magenta;
                PlusBtn.color = Color.magenta;
                UseButton.color = Color.magenta;
                UseButton.GetComponent<BoxCollider>().enabled = false;
                return;
            }
            UseButton.color = Color.white;
            UseButton.GetComponent<BoxCollider>().enabled = true;
            PlusBtn.color = useNum >= Mathf.Min(sourceNum / transNum , BalanceResourceUtil.MaxNum) ? Color.magenta : Color.white;
            SubBtn.color = useNum <= 0 ? Color.magenta : Color.white;
        }
    
        public void OnPlusBtnClick()
        {
            //得到将要的使用数量
            int useNum = int.Parse(TransNumLabel.text)  + 1;
            if (useNum > Mathf.Min(sourceNum / transNum, BalanceResourceUtil.MaxNum))
            {
                useNum = Mathf.Min(sourceNum / transNum, BalanceResourceUtil.MaxNum);
            }
            LTUIUtil.SetText(TransNumLabel, useNum.ToString());
            if (useNum > 0)
            {
                SubBtn.color = Color.white;
                UseButton.color = Color.white;
                UseButton.GetComponent<BoxCollider>().enabled = true;
            }
    
            if (useNum >= Mathf.Min(sourceNum / transNum, BalanceResourceUtil.MaxNum))
            {
                PlusBtn.color = Color.magenta;
            }
        }
    
        public void OnMaxBtnClick()
        {
            LTUIUtil.SetText(TransNumLabel, Mathf.Min(sourceNum / transNum, BalanceResourceUtil.MaxNum).ToString());
            PlusBtn.color = Color.magenta;
            int useNum = int.Parse(TransNumLabel.text);
            if (useNum > 0)
            {
                SubBtn.color = Color.white;
                UseButton.color = Color.white;
                UseButton.GetComponent<BoxCollider>().enabled = true;
            }
        }
    
        public void OnSubBtnClick()
        {
            int useNum = int.Parse(TransNumLabel.text) - 1;
            if (useNum < 0)
            {
                useNum = 0;
            }
            LTUIUtil.SetText(TransNumLabel, useNum.ToString());
            if (useNum < Mathf.Min(sourceNum / transNum, BalanceResourceUtil.MaxNum))
            {
                PlusBtn.color = Color.white;
            }
            if (int.Parse(TransNumLabel.text) <= 0)
            {
                SubBtn.color = Color.magenta;
                UseButton.color = Color.magenta;
                UseButton.GetComponent<BoxCollider>().enabled = false;
            }
            else
            {
                SubBtn.color = Color.white;
            }
        }
    
        public void OnTransBtnClick()
        {
            LTAwakeningInstanceManager.Instance.RequestCompoundItem(itemID, int.Parse(TransNumLabel.text),()=>
                {
                    Refresh();
                    if (UIInventoryBagLogic.Instance!=null)
                    {
                        UIInventoryBagLogic.Instance.RefeshBag(ShowBagContent.Instance.CurType);
                    }
                });
        }
    
    
        private bool CheckCanCompound(int DesItemID)
        {
            //错误判断  没有查找到对应的物品
            if (DesItemID == 0)
            {
                SubBtn.color = Color.magenta;
                PlusBtn.color = Color.magenta;
                UseButton.color = Color.magenta;
                MaxButton.color = Color.magenta;
    
                SubBtn.GetComponent<BoxCollider>().enabled = false;
                PlusBtn.GetComponent<BoxCollider>().enabled = false;
                UseButton.GetComponent<BoxCollider>().enabled = false;
                MaxButton.GetComponent<BoxCollider>().enabled = false;
                DesItem.mDMono.enabled = false;
                return false;
            }
            else
            {
                SubBtn.color = Color.white;
                PlusBtn.color = Color.white;
                UseButton.color = Color.white;
                MaxButton.color = Color.white;
    
                SubBtn.GetComponent<BoxCollider>().enabled = true;
                PlusBtn.GetComponent<BoxCollider>().enabled = true;
                UseButton.GetComponent<BoxCollider>().enabled = true;
                MaxButton.GetComponent<BoxCollider>().enabled = true;
                DesItem.mDMono.enabled = true;
                return true;
            }
        }
    }
}
