using LT.Hotfix.Utility;
using System;
using UnityEngine;
using DG.Tweening;

namespace Hotfix_LT.UI
{
    public class LTPromotionAttributeItemController : DynamicMonoHotfix
    {
        public const float dynamicEffectDuration = 0.5f;

        private UIProgressBar _uiProgressBar;
        private UILabel _labName;
        private UILabel _labProgress;
        private UILabel _labIncrement;
        private UILabel _labUnlock;
        private GameObject _goUp;
        private GameObject _goDown;
        private GameObject fxprg;
        private UISprite _progrerssBarForeground;
        private int PageId;
        private int timer;
        private int promotiontimer;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            _uiProgressBar = t.GetComponent<UIProgressBar>();
            _labName = t.GetComponent<UILabel>("Lab_Name");
            _labProgress = t.GetComponent<UILabel>("Lab_Progress");
            _labUnlock = t.GetComponent<UILabel>("Lab_Unlock");
            _progrerssBarForeground = t.GetComponent<UISprite>("Foreground");
            _labIncrement = t.GetComponent<UILabel>("Lab_Increment");
            _goUp = t.FindEx("Lab_Increment/Up").gameObject;
            _goDown = t.FindEx("Lab_Increment/Down").gameObject;
            fxprg = t.FindEx("prgfx").gameObject;
            fxprg.GetComponent<Transform>().localScale = new Vector3(2.2f, 1, 1);
            Reset();
        }
        public override void OnDestroy()
        {
            if (timer != 0) ILRTimerManager.instance.RemoveTimerSafely(ref timer);
            if (promotiontimer != 0) ILRTimerManager.instance.RemoveTimerSafely(ref promotiontimer);
        }
        public void Set(Data.PromotionTemplate info, Data.PromotionAttributeLevelTemplate attrInfo, int PageId)
        {
            var attrList = LTPromotionManager.Instance.GetAttrList();

            if (attrList != null && attrList.Contains(attrInfo))
            {
                _labProgress.gameObject.CustomSetActive(true);
                _labUnlock.gameObject.CustomSetActive(false);
            }
            else
            {
                _labProgress.gameObject.CustomSetActive(false);
                _labUnlock.gameObject.CustomSetActive(true);
                var promotionInfo = Data.CharacterTemplateManager.Instance.GetPromotionInfo(attrInfo.unlockLevel, 0);

                if (promotionInfo != null)
                {
                    _labUnlock.text = string.Format(EB.Localizer.GetString("ID_PROMOTION_ATTRIBUTE_UNLOCK_TIPS"), promotionInfo.name);
                }
            }
            this.PageId = PageId;
            To(info, attrInfo);
        }

        public void To(Data.PromotionTemplate info, Data.PromotionAttributeLevelTemplate attrInfo)
        {
            Reset();

            string attrName = attrInfo.name;
            int attrId = attrInfo.id;
            bool isPercentNum = attrInfo.attrValue.ToString().Contains(".");
            int nextId = Data.CharacterTemplateManager.Instance.GetNextPromotionId(info.id);
            string formatStr;
            if (PageId == 1)
            {
                formatStr = isPercentNum ? "{0}/{1}%" : "{0}/{1}";
            }
            else
            {
                formatStr = isPercentNum ? "{0}/{1}%({2})" : "{0}/{1}({2})";
            }
            Data.PromotionTemplate nextInfo = Data.CharacterTemplateManager.Instance.GetPromotionInfo(nextId);
            int currentAttrLevel = LTPromotionManager.Instance.GetCurrentAttrLevel(attrId);
            float attrValue = isPercentNum ? attrInfo.attrValue * 100 : attrInfo.attrValue;
            string deltaFormat = nextInfo == null ? "Max" : isPercentNum ? "+{0}%" : "+{0}";
            string deltaStr = string.Format(deltaFormat, nextInfo == null ? "" : ((nextInfo.attributeLevelUpperLimit - info.attributeLevelUpperLimit) * attrValue).ToString());
            _labName.text = EquipmentUtility.AttrTypeTrans(attrName, false);
            int trainingAfterAttrLevel;
            if (!LTPromotionManager.Instance.GetTrainingAfterAttrLevel(attrId, out trainingAfterAttrLevel))
            {
                trainingAfterAttrLevel = currentAttrLevel;
            }
            PlayProgressfx(trainingAfterAttrLevel * attrValue - currentAttrLevel * attrValue > 0, false);
            DOTween.To(val =>
            {
                float targetValue = info.attributeLevelUpperLimit * attrValue;
                string sign = isPercentNum ? "%" : string.Empty;
                string currentValue = LT.Hotfix.Utility.ColorUtility.FormatColorByGreen(val, targetValue, sign, isPercentNum ? 1 : 0);
                _labProgress.text = EB.StringUtil.Format(formatStr, currentValue, targetValue.ToString(isPercentNum ? "0.0" : "0"), deltaStr);
            }, currentAttrLevel * attrValue, trainingAfterAttrLevel * attrValue, dynamicEffectDuration);
            DOTween.To(val =>
            {
                _uiProgressBar.value = val;
                SetProgressBarForeground(val > 0.99f);
            }, currentAttrLevel / (float)info.attributeLevelUpperLimit, trainingAfterAttrLevel / (float)info.attributeLevelUpperLimit, dynamicEffectDuration).OnComplete(() =>
            {
                ShowIncrement(currentAttrLevel * attrValue, trainingAfterAttrLevel * attrValue, isPercentNum);
            });
        }
        public void PlayProgressfx(bool playfx, bool ispromotion = false, float data = 0, bool ispercent = false)
        {
            fxprg.CustomSetActive(false);
            if (timer != 0) ILRTimerManager.instance.RemoveTimerSafely(ref timer);
            if (playfx)//进度条特效显示
            {
                fxprg.CustomSetActive(true);
                timer = ILRTimerManager.instance.AddTimer(3500, 1, (int seq) =>
                {
                    fxprg?.CustomSetActive(false);
                    timer = 0;
                });
            }
            if (ispromotion)
            {
                Reset();
                if (promotiontimer != 0) ILRTimerManager.instance.RemoveTimerSafely(ref promotiontimer);
                _goUp.CustomSetActive(true);
                ShowIncrement(0, data, ispercent, true);
                promotiontimer = ILRTimerManager.instance.AddTimer(2500, 1, (int seq) =>
                {
                    Reset();
                    promotiontimer = 0;
                });
            }
        }

        public void Reset()
        {
            _labIncrement.text = string.Empty;
            _goUp.CustomSetActive(false);
            _goDown.CustomSetActive(false);
        }

        private void ShowIncrement(float current, float after, bool isPercent, bool frompromotion = false)
        {
            var result = after - current;
            if (result != 0)
            {
                if (isPercent)
                {
                    _labIncrement.text = result != 0 ? LT.Hotfix.Utility.ColorUtility.FormatColorPercentIncrement(current, after, isPercent ? 1 : 0) : string.Empty;
                }
                else
                {
                    _labIncrement.text = result != 0 ? LT.Hotfix.Utility.ColorUtility.FormatColorIncrement(current, after, isPercent ? 1 : 0) : string.Empty;
                }
                if (!frompromotion)
                {
                    _goUp.CustomSetActive(result > 0);
                    _goDown.CustomSetActive(result < 0);
                }
            }
        }

        private void SetProgressBarForeground(bool isFull)
        {
            _progrerssBarForeground.spriteName = isFull ? "Ty_Strip_Yellow" : "Ty_Strip_Blue";
        }
    }
}
