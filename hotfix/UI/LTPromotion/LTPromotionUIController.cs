using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTPromotionUIController : UIControllerHotfix
    {
        private LTPromotionAttributeViewController _attributeViewController;
        private LTPromotionTaskViewController _taskViewController;

        private int refreshtimer;
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            controller.backButton = t.GetComponent<UIButton>("Top/Btn_Back");
            _attributeViewController = t.FindEx("AttributeView").GetMonoILRComponent<LTPromotionAttributeViewController>();
            _taskViewController = t.FindEx("TaskView").GetMonoILRComponent<LTPromotionTaskViewController>();
        }

        public override bool IsFullscreen()
        {
            return true;
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
        }

        public override IEnumerator OnAddToStack()
        {
            if (AutoRefreshingManager.Instance.GetRefreshed(AutoRefreshingManager.RefreshName.Promotion)) {
                LTPromotionManager.Instance.SetTrainingFree(1);
                LTPromotionManager.Instance.ResetUsedTrainingTimes(1);
                LTPromotionManager.Instance.ResetUsedTrainingTimes(2);
            }

            Refresh();
            yield return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            yield return base.OnRemoveFromStack();
            DestroySelf();
        }

        public override void OnFocus()
        {
            base.OnFocus();
            _taskViewController.Set();
        }

        public void Refresh(bool fromPromotion = false)
        {
            _taskViewController.Set();
            _attributeViewController.Set(fromPromotion, () => OpenTrainingView(1), () => OpenTrainingView(2));
        }

        public void PlayUplevelFx(int fxType, int  addlevel)
        {
            _attributeViewController.PlayerFx(fxType, addlevel);
        }


        public void OpenTrainingView(int trainingId)
        {
            if (isCouldTraining(trainingId))
            {
                RequestTraning(trainingId);
            }
        }

        private void RequestTraning(int trainingId)
        {
            LTPromotionManager.Instance.Api.RequestTraining(trainingId, ht =>
            {
                DataLookupsCache.Instance.CacheData("promotion", null);
                DataLookupsCache.Instance.CacheData(ht);
                if (refreshtimer != 0)
                {
                    ILRTimerManager.instance.RemoveTimerSafely(ref refreshtimer);
                }
                if (!LTPromotionManager.Instance.HasFallingAttr())
                {
                    _attributeViewController.Set(false, () => OpenTrainingView(1), () => OpenTrainingView(2));

                    LTPromotionManager.Instance.Api.RequestSave(true, result =>
                    {
                        DataLookupsCache.Instance.CacheData("promotion", null);
                        DataLookupsCache.Instance.CacheData(result);
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_SAVE_SUCCESSFULLY"));
                        Hotfix_LT.Messenger.Raise<int, bool>(EventName.OnRefreshAllPowerChange, -1, true);
                    });
                    refreshtimer = ILRTimerManager.instance.AddTimer((int)(LTPromotionAttributeItemController.dynamicEffectDuration * 1000) + 2000, 1, seq =>
                    {
                        Refresh();
                        refreshtimer = 0;
                    });
                }
                else
                {
                    _attributeViewController.Set(false, () => OpenTrainingView(1), () => OpenTrainingView(2));
                }
            });
        }

        public void OpenTrainingTipView(int trainingId)
        {
            if (isCouldTraining(trainingId))
            {
                if (LTPromotionManager.Instance.HasTrainingResult())
                {
                    LTPromotionManager.Instance.Api.RequestSave(false, ht =>
                    {
                        DataLookupsCache.Instance.CacheData("promotion", null);
                        DataLookupsCache.Instance.CacheData(ht);
                    });
                }
                RequestTraning(trainingId);
            }
        }

        private bool isCouldTraining(int trainingId)
        {
            var info = LTPromotionManager.Instance.GetPromotion();

            if (info == null)
            {
                EB.Debug.LogError("LTPromotionUIController.OpenTrainingView -> info is null");
                return false;
            }

            if (!LTPromotionManager.Instance.IsTrainingFree(trainingId) && trainingId == 1 && BalanceResourceUtil.GetUserGold() < LTPromotionManager.Instance.GetTrainingCost(trainingId))
            {
                BalanceResourceUtil.ResLessMessage("gold");
                return false;
            }

            if (!LTPromotionManager.Instance.IsTrainingFree(trainingId) && trainingId == 2 && BalanceResourceUtil.GetUserDiamond() < LTPromotionManager.Instance.GetTrainingCost(trainingId))
            {
                BalanceResourceUtil.ResLessMessage("hc");
                return false;
            }

            if (LTPromotionManager.Instance.GetUsedTrainingTimes(trainingId) >= LTPromotionManager.Instance.GetTrainingTotalTimes(trainingId))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_BUY_TIMES_NOT_ENOUGH"));
                return false;
            }

            if (!LTPromotionManager.Instance.HasUnfinishedAttr())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_PROMOTION_TRAINING_UPPER_LIMIT_TIPS"));
                return false;
            }
            return true;
        }

    }
}
