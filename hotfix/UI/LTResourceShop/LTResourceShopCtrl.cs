using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTResourceShopCtrl : UIControllerHotfix
    {
        ConsecutiveClickCoolTrigger HotfixCoolBtn0;
        ConsecutiveClickCoolTrigger HotfixCoolBtn1;
        ConsecutiveClickCoolTrigger HotfixCoolBtn2;
        ConsecutiveClickCoolTrigger HotfixCoolBtn3;
        ConsecutiveClickCoolTrigger HotfixCoolBtn4;
        ConsecutiveClickCoolTrigger HotfixCoolBtn5;


        public UILabel[] ResourceLabel, PriceLabel, NumOfTimeLabel;
        public UISprite[] MultiplySprite;
        public GameObject FxObj;
        public GameObject[] LockObj;

        private int currentMultiplyPower;
        private ResourceShopChooseType m_type;

        public static bool m_Open;
        public override bool ShowUIBlocker { get { return true; } }
        public override IEnumerator OnAddToStack()
        {
            m_Open = true;
            yield return base.OnAddToStack();
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            LTResourceShopModel.Instance.ResetTimes(delegate
            {
                Show();
            });
        }

        private Coroutine CurCor;
        public override IEnumerator OnRemoveFromStack()
        {
            m_Open = false;
            EB.Coroutines.Stop(CurCor);
            FxOver();
            DestroySelf();
            yield break;
        }

        private bool BtnClickLimit = false;
        public void OnVigerBtnClick()
        {
            if (BtnClickLimit) return;
            BtnClickLimit = true;
            m_type = ResourceShopChooseType.vigor;
            BuyBtnClick();
        }
        public void OnGoldBtnClick()
        {
            if (BtnClickLimit) return;
            BtnClickLimit = true;
            m_type = ResourceShopChooseType.gold;
            BuyBtnClick();
        }
        //public void OnTicketBtnClick()
        //{
        //    if (BtnClickLimit) return;
        //    BtnClickLimit = true;
        //    m_type = ResourceShopChooseType.ticket;
        //    BuyBtnClick();
        //}
        public void OnExpBtnClick()
        {
            if (BtnClickLimit) return;
            BtnClickLimit = true;
            m_type = ResourceShopChooseType.exp;
            BuyBtnClick();
        }
        public void OnItemClick()
        {
            if (BtnClickLimit) return;
            BtnClickLimit = true;
            m_type = ResourceShopChooseType.Item;
            BuyBtnClick();
        }
        public void OnItemsClick()
        {
            if (BtnClickLimit) return;
            BtnClickLimit = true;
            m_type = ResourceShopChooseType.Items;
            BuyBtnClick();
        }
        public void OnActicketClick()
        {
            if (BtnClickLimit) return;
            BtnClickLimit = true;
            m_type = ResourceShopChooseType.Acticket;
            BuyBtnClick();
        }
        private void BuyBtnClick()
        {
            currentMultiplyPower = LTResourceShopModel.Instance.GetCurrentRate(m_type);
            switch (m_type)
            {
                case ResourceShopChooseType.vigor:
                    {
                        if (LockObj[0].activeSelf) { MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopController_2901")); BtnClickLimit = false; return; }
                        if (LTResourceShopModel.Instance.BuyVigorTimes == VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.BuyVigorTimes)) { MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopController_3145")); BtnClickLimit = false; return; }
                        if (isEnoughMoney(Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyVigor(LTResourceShopModel.Instance.BuyVigorTimes + 1).price_hc)) { BtnClickLimit = false; return; }
                        else
                        {
                            LTResourceShopModel.Instance.BuyVigor(delegate (bool success)
                            {
                                if (success)
                                {
                                    EB.Coroutines.Stop(CurCor);
                                    if (Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyVigor(LTResourceShopModel.Instance.BuyVigorTimes) != null)
                                    {
                                        CurCor = EB.Coroutines.Run(FxPlay((Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyVigor(LTResourceShopModel.Instance.BuyVigorTimes).vigor) * currentMultiplyPower));
                                    }
                                    else
                                    {
                                        CurCor = EB.Coroutines.Run(FxPlay((Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyVigor(1).vigor)));
                                    }
                                }
                            });
                        }
                    }; break;
                case ResourceShopChooseType.gold:
                    {
                        if (LockObj[1].activeSelf) { MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopController_2901")); BtnClickLimit = false; return; }
                        if (LTResourceShopModel.Instance.BuyGoldTimes == VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.BuyGoldTimes)) { MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopController_3145")); BtnClickLimit = false; return; }
                        if (isEnoughMoney(Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyGold(LTResourceShopModel.Instance.BuyGoldTimes + 1).price_hc)) { BtnClickLimit = false; return; }
                        else
                        {
                            LTResourceShopModel.Instance.BuyGold(delegate (bool success)
                            {
                                if (success)
                                {
                                    EB.Coroutines.Stop(CurCor);
                                    int level = 1;
                                    DataLookupsCache.Instance.SearchIntByID("level", out level);
                                    Hotfix_LT.Data.BuyGoldTemplate temp = Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyGold(LTResourceShopModel.Instance.BuyGoldTimes);
                                    if (temp != null)
                                    {
                                        CurCor = EB.Coroutines.Run(FxPlay((temp.base_gold + temp.level_inc_gold * level) * currentMultiplyPower));
                                    }
                                    else
                                    {
                                        temp = Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyGold(1);
                                        CurCor = EB.Coroutines.Run(FxPlay(temp.base_gold + temp.level_inc_gold * level));
                                    }
                                }
                            });
                        }
                    }; break;
                //case ResourceShopChooseType.ticket:
                //    {
                //        if (LockObj[2].activeSelf) { MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopController_2901")); BtnClickLimit = false; return; }
                //        if (LTResourceShopModel.Instance.BuyPowerTimes == Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetPowerTblCount()) { MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopController_3145")); BtnClickLimit = false; return; }
                //        if (isEnoughMoney(Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyPower(LTResourceShopModel.Instance.BuyPowerTimes + 1).price_hc)) { BtnClickLimit = false; return; }
                //        else
                //        {
                //            LTResourceShopModel.Instance.BuyActionPower(delegate (bool success)
                //            {
                //                if (success)
                //                {
                //                    EB.Coroutines.Stop(CurCor);
                //                    if (Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyPower(LTResourceShopModel.Instance.BuyPowerTimes) != null)
                //                    {
                //                        CurCor = EB.Coroutines.Run(FxPlay((Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyPower(LTResourceShopModel.Instance.BuyPowerTimes).base_power * currentMultiplyPower)));
                //                    }
                //                    else
                //                    {
                //                        CurCor = EB.Coroutines.Run(FxPlay((Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyPower(1).base_power)));
                //                    }
                //                }
                //            });
                //        }
                //    }; break;
                case ResourceShopChooseType.exp:
                    {
                        if (LockObj[2].activeSelf) { MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopController_2901")); BtnClickLimit = false; return; }
                        if (LTResourceShopModel.Instance.BuyExpTimes == VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.BuyExpTimes)) { MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopController_3145")); BtnClickLimit = false; return; }
                        if (isEnoughMoney(Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyExp(LTResourceShopModel.Instance.BuyExpTimes + 1).price_hc)) { BtnClickLimit = false; return; }
                        else
                        {
                            LTResourceShopModel.Instance.BuyExp(delegate (bool success)
                            {
                                if (success)
                                {
                                    EB.Coroutines.Stop(CurCor);
                                    int level = 1;
                                    DataLookupsCache.Instance.SearchIntByID("level", out level);
                                    Hotfix_LT.Data.BuyButtyExpTemplate temp = Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyExp(LTResourceShopModel.Instance.BuyExpTimes);
                                    if (temp != null)
                                    {
                                        CurCor = EB.Coroutines.Run(FxPlay((temp.base_exp + temp.level_inc_exp * level) * currentMultiplyPower));
                                    }
                                    else
                                    {
                                        temp = Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyExp(1);
                                        CurCor = EB.Coroutines.Run(FxPlay((temp.base_exp + temp.level_inc_exp * level)));
                                    }
                                }
                            });
                        }
                    }; break;
                case ResourceShopChooseType.Item:
                    {
                        if (LockObj[3].activeSelf) { MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopController_2901")); BtnClickLimit = false; return; }
                        if (isEnoughMoney(LTDrawCardConfig.Once_HCCost)) { BtnClickLimit = false; return; }
                        LTResourceShopModel.Instance.BuyDrawCardItem(1, delegate (bool success)
                         {
                             if (success)
                             {
                                 EB.Coroutines.Stop(CurCor);
                                 CurCor = EB.Coroutines.Run(FxPlay(1));
                             }
                         });
                    }
                    break;
                case ResourceShopChooseType.Items:
                    {
                        if (LockObj[4].activeSelf) { MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopController_2901")); BtnClickLimit = false; return; }
                        if (isEnoughMoney(LTDrawCardConfig.More_HCCost)) { BtnClickLimit = false; return; }
                        LTResourceShopModel.Instance.BuyDrawCardItem(9, delegate (bool success)
                         {
                             if (success)
                             {
                                 EB.Coroutines.Stop(CurCor);
                                 CurCor = EB.Coroutines.Run(FxPlay(9));
                             }
                         });
                    }
                    break;
                case ResourceShopChooseType.Acticket:
                    {
                        if (LockObj[5].activeSelf) { MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopController_2901")); BtnClickLimit = false; return; }
                        if (LTResourceShopModel.Instance.BuyActicketTimes == VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.ActicketTimes)) { MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceShopController_3145")); BtnClickLimit = false; return; }
                        if (isEnoughMoney(Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyActicket(LTResourceShopModel.Instance.BuyActicketTimes + 1).price_hc)) { BtnClickLimit = false; return; }
                        else
                        {
                            LTResourceShopModel.Instance.BuyActicket(delegate (bool success)
                            {
                                if (success)
                                {
                                    EB.Coroutines.Stop(CurCor);
                                    //CurCor = EB.Coroutines.Run(FxPlay((Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyActicket(LTResourceShopModel.Instance.BuyActicketTimes+1).base_acticket)));
                                    Hotfix_LT.Data.BuyActicketTemplate temp = Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyActicket(LTResourceShopModel.Instance.BuyActicketTimes);
                                    if (temp != null)
                                    {
                                        CurCor = EB.Coroutines.Run(FxPlay(temp.base_acticket * currentMultiplyPower));
                                    }
                                    else
                                    {
                                        temp = Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyActicket(1);
                                        CurCor = EB.Coroutines.Run(FxPlay((temp.base_acticket * currentMultiplyPower)));
                                    }
                                }
                            });
                        }
                    }; break;
            }
        }

        private void Show()
        {
            //viger
            ShowVigerItem();
            //gold
            ShowGoldItem();
            //ticket
            //ShowTicketItem();
            //exp
            ShowExpItem();
            //drawCardItem
            ShowBuyDrawCardItem();
            //acticket
            ShowActicketItem();
        }

        private void ShowBuyDrawCardItem()
        {
            //Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10011);//挑战副本id
            //bool isOpen = ft != null && ft.IsConditionOK() || ft == null;
            //PriceLabel[4].gameObject.SetActive(isOpen);
            //LockObj[4].gameObject.SetActive(!isOpen);
            //PriceLabel[5].gameObject.SetActive(isOpen);
            //LockObj[5].gameObject.SetActive(!isOpen);

            ResourceLabel[3].text = ResourceLabel[3].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("[fff348]{0}", 1);
            PriceLabel[3].text = PriceLabel[3].transform.GetChild(0).GetComponent<UILabel>().text = GetMoneyColorStr(LTDrawCardConfig.Once_HCCost);

            ResourceLabel[4].text = ResourceLabel[4].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("[fff348]{0}", 9);
            PriceLabel[4].text = PriceLabel[4].transform.GetChild(0).GetComponent<UILabel>().text = GetMoneyColorStr(LTDrawCardConfig.More_HCCost);
        }

        private void ShowVigerItem()
        {
            int i = LTResourceShopModel.Instance.BuyVigorTimes == VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.BuyVigorTimes) ? 0 : 1;
            ResourceLabel[0].text = ResourceLabel[0].transform.GetChild(0).GetComponent<UILabel>().text = "[fff348]" + Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyVigor(LTResourceShopModel.Instance.BuyVigorTimes + i).vigor.ToString();
            PriceLabel[0].text = PriceLabel[0].transform.GetChild(0).GetComponent<UILabel>().text = GetMoneyColorStr(Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyVigor(LTResourceShopModel.Instance.BuyVigorTimes + i).price_hc);
            string colorStr = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.BuyVigorTimes) - LTResourceShopModel.Instance.BuyVigorTimes > 0 ? "" : "[-][ff6699]";
            NumOfTimeLabel[0].text = NumOfTimeLabel[0].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}{1}", colorStr, VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.BuyVigorTimes) - LTResourceShopModel.Instance.BuyVigorTimes/*Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetVigorTblCount()*/);
            MultiplySpriteShow(0, i == 0 ? 1 : LTResourceShopModel.Instance.GetCurrentRate(ResourceShopChooseType.vigor));
        }
        private void ShowGoldItem()
        {
            int i = LTResourceShopModel.Instance.BuyGoldTimes == VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.BuyGoldTimes) ? 0 : 1;
            int level = 1;
            DataLookupsCache.Instance.SearchIntByID("level", out level);
            Hotfix_LT.Data.BuyGoldTemplate temp = Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyGold(LTResourceShopModel.Instance.BuyGoldTimes + i);
            ResourceLabel[1].text = ResourceLabel[1].transform.GetChild(0).GetComponent<UILabel>().text = "[fff348]" + (temp.base_gold + temp.level_inc_gold * level).ToString();
            PriceLabel[1].text = PriceLabel[1].transform.GetChild(0).GetComponent<UILabel>().text = GetMoneyColorStr(Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyGold(LTResourceShopModel.Instance.BuyGoldTimes + i).price_hc);
            string colorStr = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.BuyGoldTimes) - LTResourceShopModel.Instance.BuyGoldTimes > 0 ? "" : "[-][ff6699]";
            NumOfTimeLabel[1].text = NumOfTimeLabel[1].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}{1}", colorStr, VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.BuyGoldTimes) - LTResourceShopModel.Instance.BuyGoldTimes /*Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetGoldTblCount()*/);
            MultiplySpriteShow(1, i == 0 ? 1 : LTResourceShopModel.Instance.GetCurrentRate(ResourceShopChooseType.gold));
        }
        //private void ShowTicketItem()
        //{
        //    Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10065);//挑战副本id
        //    bool isOpen = ft != null && ft.IsConditionOK() || ft == null;
        //    PriceLabel[2].gameObject.SetActive(isOpen);
        //    NumOfTimeLabel[2].gameObject.SetActive(isOpen);
        //    LockObj[2].gameObject.SetActive(!isOpen);
        //    int i = LTResourceShopModel.Instance.BuyPowerTimes == Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetPowerTblCount() ? 0 : 1;
        //    ResourceLabel[2].text = ResourceLabel[2].transform.GetChild(0).GetComponent<UILabel>().text = "[fff348]" + Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyPower(LTResourceShopModel.Instance.BuyPowerTimes + i).base_power.ToString();
        //    PriceLabel[2].text = PriceLabel[2].transform.GetChild(0).GetComponent<UILabel>().text = GetMoneyColorStr(Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyPower(LTResourceShopModel.Instance.BuyPowerTimes + i).price_hc);
        //    string colorStr = Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetPowerTblCount() - LTResourceShopModel.Instance.BuyPowerTimes > 0 ? "" : "[-][ff6699]";
        //    NumOfTimeLabel[2].text = NumOfTimeLabel[2].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}{1}", colorStr, Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetPowerTblCount() - LTResourceShopModel.Instance.BuyPowerTimes, Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetPowerTblCount());
        //    MultiplySpriteShow(2, i == 0 ? 1 : LTResourceShopModel.Instance.GetCurrentRate(ResourceShopChooseType.ticket));
        //}
        private void ShowExpItem()
        {
            Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10050);//伙伴id
            bool isOpen = ft != null && ft.IsConditionOK() || ft == null;
            PriceLabel[2].gameObject.SetActive(isOpen);
            NumOfTimeLabel[2].gameObject.SetActive(isOpen);
            LockObj[2].gameObject.SetActive(!isOpen);
            int i = LTResourceShopModel.Instance.BuyExpTimes == VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.BuyExpTimes) ? 0 : 1;
            Hotfix_LT.Data.BuyButtyExpTemplate temp = Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyExp(LTResourceShopModel.Instance.BuyExpTimes + i);
            int level = 1;
            DataLookupsCache.Instance.SearchIntByID("level", out level);
            ResourceLabel[2].text = ResourceLabel[2].transform.GetChild(0).GetComponent<UILabel>().text = "[fff348]" + (temp.base_exp + temp.level_inc_exp * level).ToString();
            PriceLabel[2].text = PriceLabel[2].transform.GetChild(0).GetComponent<UILabel>().text = GetMoneyColorStr(Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyExp(LTResourceShopModel.Instance.BuyExpTimes + i).price_hc);
            int time = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.BuyExpTimes) - LTResourceShopModel.Instance.BuyExpTimes;
            string colorStr =time > 0 ? "" : "[-][ff6699]";
            // string.Format("{0}{1}", colorStr, Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetExpTblCount() - LTResourceShopModel.Instance.BuyExpTimes, Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetExpTblCount());
            LTUIUtil.SetText(NumOfTimeLabel[2], colorStr + time);
            MultiplySpriteShow(2, i == 0 ? 1 : LTResourceShopModel.Instance.GetCurrentRate(ResourceShopChooseType.exp));
        }

        private void ShowActicketItem()
        {
            Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10083);//觉醒副本
            bool isOpen = ft != null && ft.IsConditionOK() || ft == null;
            PriceLabel[5].gameObject.SetActive(isOpen);
            NumOfTimeLabel[3].gameObject.SetActive(isOpen);
            LockObj[5].gameObject.SetActive(!isOpen);
            int i = LTResourceShopModel.Instance.BuyActicketTimes == VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.ActicketTimes) ? 0 : 1;
            Hotfix_LT.Data.BuyActicketTemplate temp = Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyActicket(LTResourceShopModel.Instance.BuyActicketTimes + i);
            ResourceLabel[5].text = ResourceLabel[5].transform.GetChild(0).GetComponent<UILabel>().text = "[fff348]" + (temp.base_acticket).ToString();
            PriceLabel[5].text = PriceLabel[5].transform.GetChild(0).GetComponent<UILabel>().text = GetMoneyColorStr(Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetBuyActicket(LTResourceShopModel.Instance.BuyActicketTimes + i).price_hc);
            string colorStr = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.ActicketTimes) - LTResourceShopModel.Instance.BuyActicketTimes > 0 ? "" : "[-][ff6699]";
            NumOfTimeLabel[3].text = NumOfTimeLabel[3].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}{1}", colorStr, Hotfix_LT.Data.BuyResourceTemplateManager.Instance.GetActicketCount() - LTResourceShopModel.Instance.BuyActicketTimes);
            MultiplySpriteShow(5, i == 0 ? 1 : LTResourceShopModel.Instance.GetCurrentRate(ResourceShopChooseType.Acticket));
        }


        private string GetMoneyColorStr(int price)
        {
            int temp;
            DataLookupsCache.Instance.SearchIntByID("res.hc.v", out temp);
            return price <= temp ? price.ToString() : "[ff6699]" + price.ToString();
        }
        private bool isEnoughMoney(int price)
        {
            int temp;
            DataLookupsCache.Instance.SearchIntByID("res.hc.v", out temp);
            if (price > temp)
            {
                LTHotfixGeneralFunc.ShowChargeMess();
            }
            return price > temp;
        }

        private void MultiplySpriteShow(int index, int Multiply = 1)
        {
            switch (Multiply)
            {
                case 2:
                case 3:
                case 5:
                case 10: ShowMultiply(index, Multiply); break;
                default: MultiplySprite[index].gameObject.CustomSetActive(false); break;
            }
        }

        private void ShowMultiply(int index, int Multiply = 1)
        {
            MultiplySprite[index].spriteName = string.Format("Exchange_Word_Baoji_{0}", Multiply);
            MultiplySprite[index].transform.GetChild(0).GetComponent<UISprite>().spriteName = string.Format("Exchange_Light_Baoji_Di{0}", Multiply);
            MultiplySprite[index].gameObject.CustomSetActive(true);
        }

        private WaitForSeconds wait1 = new WaitForSeconds(0.5f);
        IEnumerator FxPlay(int value)
        {
            switch (m_type)
            {
                case ResourceShopChooseType.vigor:
                    {
                        FxObj.transform.localPosition = HotfixCoolBtn0.transform.localPosition;
                    }; break;
                case ResourceShopChooseType.gold:
                    {
                        FxObj.transform.localPosition = HotfixCoolBtn1.transform.localPosition;
                    }; break;
                //case ResourceShopChooseType.ticket:
                //    {
                //        FxObj.transform.localPosition = HotfixCoolBtn2.transform.localPosition;
                //    }; break;
                case ResourceShopChooseType.exp:
                    {
                        FxObj.transform.localPosition = HotfixCoolBtn2.transform.localPosition;
                    }; break;
                default:
                    break;
                case ResourceShopChooseType.Item:
                    {
                        FxObj.transform.localPosition = HotfixCoolBtn3.transform.localPosition;
                    }; break;
                case ResourceShopChooseType.Items:
                    {
                        FxObj.transform.localPosition = HotfixCoolBtn4.transform.localPosition;
                    }; break;
                case ResourceShopChooseType.Acticket:
                    {
                        FxObj.transform.localPosition = HotfixCoolBtn5.transform.localPosition;
                    }; break;
            }
            FxObj.CustomSetActive(true);
            yield return wait1;
            switch (m_type)
            {
                case ResourceShopChooseType.vigor:
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTResourceShopController_21705"), value));
                    }; break;
                case ResourceShopChooseType.gold:
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTResourceShopController_21902"), value));
                    }; break;
                //case ResourceShopChooseType.ticket:
                //    {
                //        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTResourceShopController_22102"), value));
                //    }; break;
                case ResourceShopChooseType.exp:
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTResourceShopController_22300"), value));
                    }; break;
                default:
                    break;
                case ResourceShopChooseType.Item:
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_RESOURCESHOP_BUYDRAWCARD"), value));
                    }; break;
                case ResourceShopChooseType.Items:
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_RESOURCESHOP_BUYDRAWCARD"), value));
                    }; break;
                case ResourceShopChooseType.Acticket:
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_RESOURCESHOP_BUYACTICKET"), value));
                    }; break;
            }
            Show();
            FxOver();
        }
        //private void OnMesteryGiftClick()
        //{
        //    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_uifont_in_LTResourceShopUI_Tip_1"));
        //}
        private void FxOver()
        {
            BtnClickLimit = false;
            FxObj.CustomSetActive(false);
        }

        public override void Awake()
        {
            base.Awake();

            ResourceLabel = new UILabel[6];
            ResourceLabel[0] = controller.transform.Find("Content/MainContent/ResourceItem/Count").GetComponent<UILabel>();
            ResourceLabel[1] = controller.transform.Find("Content/MainContent/ResourceItem (1)/Count").GetComponent<UILabel>();
            ResourceLabel[2] = controller.transform.Find("Content/MainContent/ResourceItem (2)/Count").GetComponent<UILabel>();
            ResourceLabel[3] = controller.transform.Find("Content/MainContent/ResourceItem (3)/Count").GetComponent<UILabel>();
            ResourceLabel[4] = controller.transform.Find("Content/MainContent/ResourceItem (4)/Count").GetComponent<UILabel>();
            ResourceLabel[5] = controller.transform.Find("Content/MainContent/ResourceItem (5)/Count").GetComponent<UILabel>();

            PriceLabel = new UILabel[6];
            PriceLabel[0] = controller.transform.Find("Content/MainContent/ResourceItem/Price/Label (4)").GetComponent<UILabel>();
            PriceLabel[1] = controller.transform.Find("Content/MainContent/ResourceItem (1)/Price/Label (4)").GetComponent<UILabel>();
            PriceLabel[2] = controller.transform.Find("Content/MainContent/ResourceItem (2)/Price/Label (4)").GetComponent<UILabel>();
            PriceLabel[3] = controller.transform.Find("Content/MainContent/ResourceItem (3)/Price/Label (4)").GetComponent<UILabel>();
            PriceLabel[4] = controller.transform.Find("Content/MainContent/ResourceItem (4)/Price/Label (4)").GetComponent<UILabel>();
            PriceLabel[5] = controller.transform.Find("Content/MainContent/ResourceItem (5)/Price/Label (4)").GetComponent<UILabel>();

            NumOfTimeLabel = new UILabel[4];
            NumOfTimeLabel[0] = controller.transform.Find("Content/MainContent/ResourceItem/TimesLabel").GetComponent<UILabel>();
            NumOfTimeLabel[1] = controller.transform.Find("Content/MainContent/ResourceItem (1)/TimesLabel").GetComponent<UILabel>();
            NumOfTimeLabel[2] = controller.transform.Find("Content/MainContent/ResourceItem (2)/TimesLabel").GetComponent<UILabel>();
            NumOfTimeLabel[3] = controller.transform.Find("Content/MainContent/ResourceItem (5)/TimesLabel").GetComponent<UILabel>();
            //NumOfTimeLabel[4] = controller.transform.Find("Content/MainContent/ResourceItem (4)/TimesLabel").GetComponent<UILabel>();
            //NumOfTimeLabel[5] = controller.transform.Find("Content/MainContent/ResourceItem (5)/TimesLabel").GetComponent<UILabel>();

            MultiplySprite = new UISprite[6];
            MultiplySprite[0] = controller.transform.Find("Content/MainContent/ResourceItem/Sprite").GetComponent<UISprite>();
            MultiplySprite[1] = controller.transform.Find("Content/MainContent/ResourceItem (1)/Sprite").GetComponent<UISprite>();
            MultiplySprite[2] = controller.transform.Find("Content/MainContent/ResourceItem (2)/Sprite").GetComponent<UISprite>();
            MultiplySprite[3] = controller.transform.Find("Content/MainContent/ResourceItem (3)/Sprite").GetComponent<UISprite>();
            MultiplySprite[4] = controller.transform.Find("Content/MainContent/ResourceItem (4)/Sprite").GetComponent<UISprite>();
            MultiplySprite[5] = controller.transform.Find("Content/MainContent/ResourceItem (5)/Sprite").GetComponent<UISprite>();

            FxObj = controller.transform.Find("Content/Fx").gameObject;

            LockObj = new GameObject[6];
            LockObj[0] = controller.transform.Find("Content/MainContent/ResourceItem/Price/Lock").gameObject;
            LockObj[1] = controller.transform.Find("Content/MainContent/ResourceItem (1)/Price/Lock").gameObject;
            LockObj[2] = controller.transform.Find("Content/MainContent/ResourceItem (2)/Price/Lock").gameObject;
            LockObj[3] = controller.transform.Find("Content/MainContent/ResourceItem (3)/Price/Lock").gameObject;
            LockObj[4] = controller.transform.Find("Content/MainContent/ResourceItem (4)/Price/Lock").gameObject;
            LockObj[5] = controller.transform.Find("Content/MainContent/ResourceItem (5)/Price/Lock").gameObject;

            UIButton backButton = controller.transform.Find("Content/Bg/TopSprite/CloseBtn").GetComponent<UIButton>(); ;
            controller.backButton = backButton;

            HotfixCoolBtn0 = controller.transform.Find("Content/MainContent/ResourceItem").GetComponent<ConsecutiveClickCoolTrigger>();
            HotfixCoolBtn0.clickEvent.Add(new EventDelegate(OnVigerBtnClick));
            HotfixCoolBtn1 = controller.transform.Find("Content/MainContent/ResourceItem (1)").GetComponent<ConsecutiveClickCoolTrigger>();
            HotfixCoolBtn1.clickEvent.Add(new EventDelegate(OnGoldBtnClick));
            HotfixCoolBtn2 = controller.transform.Find("Content/MainContent/ResourceItem (2)").GetComponent<ConsecutiveClickCoolTrigger>();
            HotfixCoolBtn2.clickEvent.Add(new EventDelegate(OnExpBtnClick));
            HotfixCoolBtn3 = controller.transform.Find("Content/MainContent/ResourceItem (3)").GetComponent<ConsecutiveClickCoolTrigger>();
            HotfixCoolBtn3.clickEvent.Add(new EventDelegate(OnItemClick));
            HotfixCoolBtn4 = controller.transform.Find("Content/MainContent/ResourceItem (4)").GetComponent<ConsecutiveClickCoolTrigger>();
            HotfixCoolBtn4.clickEvent.Add(new EventDelegate(OnItemsClick));
            HotfixCoolBtn5 = controller.transform.Find("Content/MainContent/ResourceItem (5)").GetComponent<ConsecutiveClickCoolTrigger>();
            HotfixCoolBtn5.clickEvent.Add(new EventDelegate(OnActicketClick));
        }

    }

}
