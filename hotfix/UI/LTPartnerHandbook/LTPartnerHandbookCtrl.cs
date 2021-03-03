using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{

    public class LTPartnerHandbookCtrl : UIControllerHotfix
    {
        public override bool IsFullscreen() { return true; }
        public List<HandbookItemCellCtrl> ItemCellList;

        Hotfix_LT.Data.MannualScoreTemplate curHandBookInfo;
        Hotfix_LT.Data.MannualScoreTemplate nextHandBookInfo;

        public override void Awake()
        {
            base.Awake();
            controller.TextureCmps["BGTexture"].spriteName = "Game_Background_14";

			controller.backButton = controller.transform.Find("Anchor_TopLeft/CancelBtn").GetComponent<UIButton>();

            controller.UiButtons["HotfixBtn"].onClick.Add(new EventDelegate(OnTipsButtonClick));
            controller.CoolTriggers["HotfixCoolBtn0"].clickEvent.Add(new EventDelegate(OnHandBookLevelUp));
            controller.BoxColliders["UplevelBtnBox"] = controller.CoolTriggers["HotfixCoolBtn0"].GetComponent<BoxCollider>();

            MonoILRFunc();
        }

        private void MonoILRFunc()
        {
            ItemCellList = new List<HandbookItemCellCtrl>();
            ItemCellList.Add(controller.transform.Find("Anchor_Mid/DetialItem/HandBookItem").GetMonoILRComponent<HandbookItemCellCtrl>());
            ItemCellList.Add(controller.transform.Find("Anchor_Mid/DetialItem/HandBookItem (1)").GetMonoILRComponent<HandbookItemCellCtrl>());
            ItemCellList.Add(controller.transform.Find("Anchor_Mid/DetialItem/HandBookItem (2)").GetMonoILRComponent<HandbookItemCellCtrl>());

            for (int i = 0; i < ItemCellList.Count; ++i)
            {
                ItemCellList[i].SetType((Hotfix_LT.Data.eRoleAttr)(i + 1));
            }
        }

        public override void OnDestroy()
        {
            controller.TextureCmps["BGTexture"].spriteName = string.Empty;
            base.OnDestroy();
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            controller.GObjects["UplevelFx"].CustomSetActive(false);
            controller.GObjects["UnlockFx"].CustomSetActive(false);
            //UpdateInfo();
        }

        int mTimer = 0;
		public override IEnumerator OnAddToStack()
		{
			GameDataSparxManager.Instance.RegisterListener(LTPartnerHandbookManager.ListDataId, OnDataListener);
			yield return base.OnAddToStack();
			mTimer = ILRTimerManager.instance.AddTimer(500, 1, OnCheckGetSpoint);
			controller.GObjects["GeneralFx"].GetComponent<ParticleSystemUIComponent>().Play(false);
			controller.GObjects["GeneralCharFx"].GetComponent<ParticleSystemUIComponent>().Play(false);
		}

		public override IEnumerator OnRemoveFromStack()
        {
            if (mTimer != 0)
            {                
                ILRTimerManager.instance.RemoveTimer(mTimer);
                mTimer = 0;
            }
            RemoverLabelTimer();
            GameDataSparxManager.Instance.UnRegisterListener(LTPartnerHandbookManager.ListDataId, OnDataListener);
            DestroySelf();
            yield break;
        }

        private void OnDataListener(string path, INodeData data)
        {
            UpdateInfo();
        }

        #region 14ms
        private void UpdateInfo(bool isUplevel = false)
        {
            int curLevel = LTPartnerHandbookManager.Instance.GetHandBookLevel();
            curHandBookInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMannualScoreTemplateById(curLevel);
            nextHandBookInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMannualScoreTemplateById(curLevel + 1);
            int UnlockLevel = LTPartnerHandbookManager.Instance.UnLockLevel;

            int id = 0;
            float curAtk = 0;
            float curDef = 0;
            float curMaxHP = 0;
            if (curHandBookInfo != null)
            {
                id = curHandBookInfo.id;
                curAtk = curHandBookInfo.ATK;
                curDef = curHandBookInfo.DEF;
                curMaxHP = curHandBookInfo.maxHP;
            }
            controller.UiLabels["curLevelLabel"].text = id.ToString();
            controller.UiLabels["curAddATKLabel"].text = curAtk.ToString();
            controller.UiLabels["curAddDefLabel"].text = curDef.ToString();
            controller.UiLabels["curAddMAXHPLabel"].text = curMaxHP.ToString();

            if (nextHandBookInfo != null)//有下一等级
            {
                controller.UiLabels["nextLevelLabel"].text = nextHandBookInfo.id.ToString();
                controller.UiLabels["nextAddATKLabel"].text = "+" + (nextHandBookInfo.ATK - curAtk).ToString();
                controller.UiLabels["nextAddDefLabel"].text = "+" + (nextHandBookInfo.DEF - curDef).ToString();
                controller.UiLabels["nextAddMaxHPLabel"].text = "+" + (nextHandBookInfo.maxHP - curMaxHP).ToString();
            }
            else//已满级
            {
                controller.UiLabels["nextLevelLabel"].text = string.Empty;
                controller.UiLabels["nextAddATKLabel"].text = string.Empty;
                controller.UiLabels["nextAddDefLabel"].text = string.Empty;
                controller.UiLabels["nextAddMaxHPLabel"].text = string.Empty;
            }

            if (isUplevel)
            {
                if (curLevel == UnlockLevel)
                {
                    controller.GObjects["UnlockFx"].CustomSetActive(false);
                    controller.GObjects["UnlockFx"].CustomSetActive(true);
                }
                else
                {
                    controller.GObjects["UplevelFx"].CustomSetActive(false);
                    controller.GObjects["UplevelFx"].CustomSetActive(true);
                }

                LabelAble(false);

                controller.UiLabels["curAddATKLabel"].GetComponent<TweenScale>().ResetToBeginning();
                controller.UiLabels["curAddATKLabel"].GetComponent<TweenScale>().PlayForward();
                controller.UiLabels["curAddDefLabel"].GetComponent<TweenScale>().ResetToBeginning();
                controller.UiLabels["curAddDefLabel"].GetComponent<TweenScale>().PlayForward();
                controller.UiLabels["curAddMAXHPLabel"].GetComponent<TweenScale>().ResetToBeginning();
                controller.UiLabels["curAddMAXHPLabel"].GetComponent<TweenScale>().PlayForward();
                RemoverLabelTimer();
                mlableTimer = ILRTimerManager.instance.AddTimer(500, 1, EnabelNext);
            }
            else
            {
                LabelAble(true);
            }


            if (curLevel < UnlockLevel)//未解锁
            {
                controller.GObjects["GeneralCharFx"].CustomSetActive(false);
                controller.UiLabels["LockTipLabel"].gameObject.CustomSetActive(true);
                controller.GObjects["UnlockTipObj"].CustomSetActive(false);
                controller.UiLabels["LockTipLabel"].text = string.Format(EB.Localizer.GetString("ID_HANDBOOK_MAGIC_BOOK_LOCK_TIP"), UnlockLevel);//魔法书x级解锁图鉴
                for (int i = 0; i < ItemCellList.Count; ++i)
                {
                    ItemCellList[i].FillData(true, isUplevel);
                }
            }
            else//解锁
            {
                controller.GObjects["GeneralCharFx"].CustomSetActive(true);
                controller.UiLabels["LockTipLabel"].gameObject.CustomSetActive(false);
                controller.GObjects["UnlockTipObj"].CustomSetActive(true);
                controller.UiLabels["CurTipLabel"].text = curHandBookInfo.levelLimit.ToString();
                if (nextHandBookInfo != null && curHandBookInfo.levelLimit != nextHandBookInfo.levelLimit)
                {
                    controller.UiLabels["NextTipLabel"].gameObject.CustomSetActive(true);
                    controller.UiLabels["NextTipLabel"].text = nextHandBookInfo.levelLimit.ToString();
                }
                else
                {
                    controller.UiLabels["NextTipLabel"].gameObject.CustomSetActive(false);
                    //UnlockTipObj.CustomSetActive(false);
                }
                for (int i = 0; i < ItemCellList.Count; ++i)
                {
                    ItemCellList[i].FillData(false, isUplevel);
                }
            }

            UpdateSpoint();

			CloseCallbacks.Add(() =>
			{
				controller.GObjects["GeneralFx"].GetComponent<ParticleSystemUIComponent>().Stop();
				controller.GObjects["GeneralCharFx"].GetComponent<ParticleSystemUIComponent>().Stop();
			});
        }
        #endregion

        int mlableTimer = 0;
        private void EnabelNext(int timer)
        {            
            LabelAble(true);
            mlableTimer = 0;
        }

        private void LabelAble(bool isTrue)
        {
            if (controller != null)
            {
                controller.UiLabels["nextLevelLabel"].gameObject.CustomSetActive(isTrue && !string.IsNullOrEmpty(controller.UiLabels["nextLevelLabel"].text));
                controller.UiLabels["nextAddATKLabel"].gameObject.CustomSetActive(isTrue);
                controller.UiLabels["nextAddDefLabel"].gameObject.CustomSetActive(isTrue);
                controller.UiLabels["nextAddMaxHPLabel"].gameObject.CustomSetActive(isTrue);
            }
        }
        private void RemoverLabelTimer()
        {
            if (mlableTimer > 0)
            {              
                ILRTimerManager.instance.RemoveTimer(mlableTimer);
                mlableTimer = 0;
            }
        }

        private bool canLevelup = false;
        private void UpdateSpoint()
        {
            int curCount = LTPartnerHandbookManager.Instance.GetHandBookSpoint() - (curHandBookInfo != null ? curHandBookInfo.totleScore : 0);
            int nextCount = (nextHandBookInfo != null) ? nextHandBookInfo.score : 0;
            canLevelup = curCount >= nextCount;
            string color = canLevelup ? LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
            if (controller != null)
            {
                var label = controller.UiLabels["SpointCountLabel"];
                if(label != null)
                {
                    label.text = string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat + "{2}", color, curCount, (nextHandBookInfo != null) ? "/" + nextCount : string.Empty);
                }
                var box = controller.BoxColliders["UplevelBtnBox"];
                if (box != null)
                {
                    box.enabled = (nextHandBookInfo != null);
                }
                var label2 = controller.UiLabels["BtnLabel"];
                if(label2 != null)
                {
                    label2.text = (nextHandBookInfo != null) ? EB.Localizer.GetString("ID_HANDBOOK_LEVEL_UP") : EB.Localizer.GetString("ID_HAS_MAX_LEVEL");
                }
                var rp = controller.GObjects["UplevelBtnRedPoint"];
                if (rp != null)
                {
                    rp.CustomSetActive(LTPartnerHandbookManager.Instance.IsHandBookCanLevelUp());
                }            
            }
        }

        private void OnCheckGetSpoint(int timer)
        {
            //判断是否增加了知识点数，打开获取知识点数界面
            int totle = LTPartnerHandbookManager.Instance.GetTotleScore();
            if (totle > LTPartnerHandbookManager.Instance.GetHandBookSpoint())
            {
                System.Action Callback = delegate { UpdateSpoint(); };
                var ht = Johny.HashtablePool.Claim();
                ht.Add("Callback", Callback);
                ht.Add("Count", totle);
                GlobalMenuManager.Instance.Open("LTPartnerHandbookBreakThroughView", ht);
            }
            mTimer = 0;
        }

        public void OnHandBookLevelUp()
        {
            if (canLevelup && nextHandBookInfo != null)
            {
                LTPartnerHandbookManager.Instance.MagicBookLevelUp(nextHandBookInfo.id, delegate
                {
                    UpdateInfo(true);
                    LTPartnerDataManager.Instance.OnDestineTypePowerChanged(Data.eRoleAttr.None, (prm)=>
                    {
                        LTFormationDataManager.OnRefreshMainTeamPower(prm);
                    }, true);
                });
            }
            else
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_HANDBOOK_BTN_TIP2"));
            }
        }

        public void OnTipsButtonClick()
        {
            string text = EB.Localizer.GetString("ID_HANDBOOK_BOOK_RULES");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text);
        }
        
    }
}
