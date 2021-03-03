using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTUltimateSelectLevelCtrl : DynamicCellController<int>
    {
        private Transform CurTran;

        private UISprite BGSprite;
        private UIButton btn;

        private UILabel LevelLabel;

        private Transform Mask;

        public override void Awake()
        {
            base.Awake();
            btn = mDMono.transform.GetComponent<UIButton>();
            btn.onClick.Add(new EventDelegate(OnClick));

            CurTran = mDMono.transform.Find("Cur");
            BGSprite = mDMono.transform.GetComponent<UISprite>();
            LevelLabel = mDMono.transform.Find("Label").GetComponent<UILabel>();
            Mask = mDMono.transform.Find("Mask");
            LTUltimateTrialDataManager.Instance.OnLevelSelect += OnSelectFunc;
        }

        public override void OnDestroy()
        {
            LTUltimateTrialDataManager.Instance.OnLevelSelect -= OnSelectFunc;
            base.OnDestroy();
        }

        private int mLevel;

        private int mHeightestLevel;

        public override void Clean()
        {
            mLevel = 0;
            mDMono.gameObject.CustomSetActive(false);
        }

        public override void Fill(int level)
        {
            mLevel = level;
            mHeightestLevel = Hotfix_LT.Data.EventTemplateManager.Instance.GetInfiniteChallengeLevelByLayer(LTUltimateTrialDataManager.Instance.GetCurLayer());
            CurTran.gameObject.CustomSetActive(mLevel == mHeightestLevel);
            LevelLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_68326"), mLevel);
            Mask.gameObject.CustomSetActive(mLevel > mHeightestLevel);
            OnSelectFunc(LTUltimateTrialDataManager.Instance.curSelectLevel);
            mDMono.gameObject.CustomSetActive(true);
        }

        private void OnClick()
        {
            if (mLevel > mHeightestLevel)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTUltimateSelectLevelCtrl_1751"));
                return;
            }

            if (LTUltimateTrialDataManager.Instance.OnLevelSelect != null)
            {
                LTUltimateTrialDataManager.Instance.OnLevelSelect(mLevel);
            }
        }

        private void OnSelectFunc(int selectLevel)
        {
            if (mLevel == selectLevel)
            {
                BGSprite.spriteName = btn.hoverSprite = btn.normalSprite = btn.pressedSprite  = "Ty_Mail_Di3";
            }
            else
            {
                if (mLevel % 2 == 0)
                {
                    BGSprite.spriteName = btn.hoverSprite= btn.normalSprite = btn.pressedSprite = "Ty_Mail_Di2";
                }
                else
                {
                    BGSprite.spriteName = btn.hoverSprite = btn.normalSprite = btn.pressedSprite = "Ty_Mail_Di1";
                }
            }
        }
    }
}
