using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI {
    public class GenericRewardShowController : UIControllerHotfix {
        public override bool ShowUIBlocker { get { return true; } }

        public UIGrid RewardsGrid;
        public UIButton SureBtn;
        public UILabel TipLabel;
        public UILabel TitleLabel;
        public LTShowItem ShowItemTemplate;

        private List<LTShowItemData> mAwardDatas;
        private List<LTShowItem> mShowItemList;

        public override void Awake() {
            base.Awake();

            var t = controller.transform;
            RewardsGrid = t.GetComponent<UIGrid>("Content/AwardGrid");
            SureBtn = t.GetComponent<UIButton>("Content/SureBtn");
            TipLabel = t.GetComponent<UILabel>("Content/Tips");
            TitleLabel = t.GetComponent<UILabel>("LTFrame/Content/Title/Title");
            ShowItemTemplate = t.GetMonoILRComponent<LTShowItem>("Content/LTShowItem");

            controller.backButton = t.GetComponent<UIButton>("LTFrame/Content/CloseBtn");
            t.GetComponent<UIButton>("Content/SureBtn").onClick.Add(new EventDelegate(OnConfirmBtn));
        }

        public override void SetMenuData(object param) {
            base.SetMenuData(param);
            Hashtable obj = param as Hashtable;
            mAwardDatas = obj["data"] as List<LTShowItemData>;
            LTUIUtil.SetText(TipLabel, obj["tip"].ToString());

            if (obj["title"] != null) {
                LTUIUtil.SetText(TitleLabel, obj["title"].ToString());
            } else {
                LTUIUtil.SetText(TitleLabel, EB.Localizer.GetString("ID_DIALOG_TITLE_TIPS"));
            }

            ShowReward();
        }

        public override void OnDestroy() {
            mAwardDatas = null;
            mShowItemList = null;
            base.OnDestroy();
        }

        public override IEnumerator OnAddToStack() {
            yield return base.OnAddToStack();
            RewardsGrid.Reposition();
        }

        public override IEnumerator OnRemoveFromStack() {
            if (mShowItemList != null) {
                for (int i = 0; i < mShowItemList.Count; ++i) {
                    mShowItemList[i].mDMono.gameObject.CustomSetActive(false);
                }
            }

            DestroySelf();
            yield break;
        }

        private void ShowReward() {
            if (mAwardDatas == null) {
                return;
            }

            if (mShowItemList == null) {
                mShowItemList = new List<LTShowItem>();
            }

            int count = mAwardDatas.Count - mShowItemList.Count;

            if (count > 0) {
                int num = mShowItemList.Count;

                for (int i = 0; i < count; i++) {
                    LTShowItem showItem = GameUtils.InstantiateEx<Transform>(ShowItemTemplate.mDMono.transform, RewardsGrid.transform, (num + i).ToString()).GetMonoILRComponent<LTShowItem>(); ;
                    mShowItemList.Add(showItem);
                }
            }


            for (int i = 0; i < mShowItemList.Count; ++i) {
                if (i < mAwardDatas.Count) {
                    mShowItemList[i].mDMono.gameObject.CustomSetActive(true);

                    var item = mAwardDatas[i];
                    mShowItemList[i].LTItemData = new LTShowItemData(item.id, item.count, item.type, false);
                } else {
                    mShowItemList[i].mDMono.gameObject.CustomSetActive(false);
                }
            }

            RewardsGrid.enabled = true;
        }

        /// <summary>
        /// 点击确认按钮
        /// </summary>
        public void OnConfirmBtn() {
            base.OnCancelButtonClick();
            //因为天梯奖励已经领取过，再次提示已经领取
            string tips = EB.Localizer.GetString("ID_codefont_in_LadderController_11750");

            if (TipLabel.text.Equals(tips)) {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, tips);
            }
        }
    }

}