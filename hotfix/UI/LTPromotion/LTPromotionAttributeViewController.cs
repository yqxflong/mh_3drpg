using System;
using UnityEngine;

namespace Hotfix_LT.UI {
    public class LTPromotionAttributeViewController : DynamicMonoHotfix {
        private CampaignTextureCmp _campaignTextureCmp;
        private UITexture _texIcon;
        private UILabel _labName;
        private LTPromotionTrainingButtonController _generalTrainingController;
        private LTPromotionTrainingButtonController _eliteTrainingController;
        private LTPromotionStarGroupController _starGroupController;
        private LTPromotionAttributeGroupController _attrGroupController;
        private GameObject _upstarfx;
        private GameObject _promotionfx;
        private int timer;
        private int timer1;

        private ConsecutiveClickCoolTrigger saveBtn;
        private ConsecutiveClickCoolTrigger cancleBtn;
        public override void Awake() {
            base.Awake();

            var t = mDMono.transform;
            _campaignTextureCmp = t.GetComponent<CampaignTextureCmp>("Icon");
            _texIcon = t.GetComponent<UITexture>("Icon");
            _upstarfx = t.FindEx("Icon/upstarfx").gameObject;
            _promotionfx = t.FindEx("Icon/promotionfx").gameObject;
            _labName = t.GetComponent<UILabel>("Name");
            _generalTrainingController = t.GetMonoILRComponent<LTPromotionTrainingButtonController>("BtnGroup/Lab_GeneralTraining");
            _eliteTrainingController = t.GetMonoILRComponent<LTPromotionTrainingButtonController>("BtnGroup/Lab_EliteTraining");
            _starGroupController = t.GetMonoILRComponent<LTPromotionStarGroupController>("StarRoot");
            _attrGroupController = t.GetMonoILRComponent<LTPromotionAttributeGroupController>("AttrGroup");

            saveBtn = t.GetComponent<ConsecutiveClickCoolTrigger>("BtnGroup/Btn_Save");
            saveBtn.clickEvent.Add(new EventDelegate(delegate {
                FusionAudio.PostEvent("UI/General/ButtonClick");
                if (LTPromotionManager.Instance.HasTrainingResult()) {
                    LTPromotionManager.Instance.Api.RequestSave(true, ht => {
                        DataLookupsCache.Instance.CacheData("promotion", null);
                        DataLookupsCache.Instance.CacheData(ht);
                        if (timer1 != 0) ILRTimerManager.instance.RemoveTimerSafely(ref timer1);
                        timer1 = ILRTimerManager.instance.AddTimer(2000, 1, (int seq) => {
                            mDMono.transform.parent.GetUIControllerILRComponent<LTPromotionUIController>(false)?.Refresh();//保存数据之后刷新晋升界面动画
                        });
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_SAVE_SUCCESSFULLY"));
                        Hotfix_LT.Messenger.Raise<int, bool>(EventName.OnRefreshAllPowerChange, -1, true);
                        SetButtonState();
                    });
                } else {
                    SetButtonState();
                }

            }));
            cancleBtn = t.GetComponent<ConsecutiveClickCoolTrigger>("BtnGroup/Lab_Cancle");
            cancleBtn.clickEvent.Add(new EventDelegate(delegate {
                FusionAudio.PostEvent("UI/General/ButtonClick");
                if (LTPromotionManager.Instance.HasTrainingResult()) {
                    LTPromotionManager.Instance.Api.RequestSave(false, ht => {
                        DataLookupsCache.Instance.CacheData("promotion", null);
                        DataLookupsCache.Instance.CacheData(ht);
                        mDMono.transform.parent.GetUIControllerILRComponent<LTPromotionUIController>(false)?.Refresh();//保存数据之后刷新晋升界面动画
                        SetButtonState();
                    });
                } else {
                    SetButtonState();
                }
            }));

            _campaignTextureCmp.onLoadingOver.Add(new EventDelegate(() => {
                _texIcon.keepAspectRatio = UIWidget.AspectRatioSource.Free;
                _texIcon.MakePixelPerfect();
                _texIcon.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
                _texIcon.height = (int)(_texIcon.mainTexture.height * 1.42f);
            }));
        }

        public void SetButtonState() {
            bool isUnSave = LTPromotionManager.Instance.HasTrainingResult() && LTPromotionManager.Instance.HasFallingAttr();
            saveBtn.transform.gameObject.CustomSetActive(isUnSave);
            cancleBtn.transform.gameObject.CustomSetActive(isUnSave);
            _eliteTrainingController.mDMono.gameObject.CustomSetActive(!isUnSave);
            _generalTrainingController.mDMono.gameObject.CustomSetActive(!isUnSave);
        }

        public override void OnDestroy() {
            if (timer != 0) ILRTimerManager.instance.RemoveTimerSafely(ref timer);
            if (timer1 != 0) ILRTimerManager.instance.RemoveTimerSafely(ref timer1);
        }

        public void Set(bool isPlayFx, Action generalTrainingAction, Action eliteTrainingActioin) {
            var info = LTPromotionManager.Instance.GetPromotion();
            _campaignTextureCmp.spriteName = info.bigIcon;
            _labName.text = string.Format("[{0}]{1}", LT.Hotfix.Utility.ColorUtility.QualityToGradientTopColorHexadecimal(info.qualityLevel), info.name);
            _generalTrainingController.Set(1, generalTrainingAction);
            _eliteTrainingController.Set(2, eliteTrainingActioin);
            _starGroupController.Set(Data.CharacterTemplateManager.Instance.GetPromotionLevelStarCount(info.level), info.star, isPlayFx);
            _attrGroupController.Set(info, 0);
            SetButtonState();
        }

        /// <summary>
        /// 播放晋升特效，0为升星，1为晋升
        /// </summary>
        /// <param name="FxType"></param>
        public void PlayerFx(int FxType, int addlevel) {
            _upstarfx.CustomSetActive(false);
            _promotionfx.CustomSetActive(false);

            if (timer != 0) {
                ILRTimerManager.instance.RemoveTimerSafely(ref timer);
            }

            switch (FxType) {
                case 0:
                    _upstarfx.CustomSetActive(true);
                    timer = ILRTimerManager.instance.AddTimer(2000, 1, (int seq) => {
                        _upstarfx?.CustomSetActive(false);
                        timer = 0;
                    });
                    break;
                case 1:
                    _promotionfx.CustomSetActive(true);
                    timer = ILRTimerManager.instance.AddTimer(2000, 1, (int seq) => {
                        _promotionfx?.CustomSetActive(false);
                        timer = 0;
                    });
                    break;
                default:
                    break;
            }

            if (addlevel > 0) {
                _attrGroupController.PlayPromotiomProgressfx(addlevel);
            }
        }
    }
}
