using System;
using UnityEngine;

namespace Hotfix_LT.UI {
    public class FormationPartnerItem : DynamicMonoHotfix {
        public UILabel LevelLabel, BreakLabel;//新增突破等级breakLabel
        public UISprite LevelBgSprite;
        public UISprite AttrBGSprite;
        public UISprite QualityBorderSprite;
        public UISprite QualityBorderSpriteBg;//新增框的背景bg
        public DynamicUISprite IconSprite;

        public UISprite[] StarList;
        public GameObject StarUIGrid;
        public OtherPlayerPartnerData PartnerData { get; private set; }

        private ParticleSystemUIComponent charFx, upgradeFx;
        private EffectClip efClip, upgradeefclip;
        private bool canOpen = true;
        public override void Awake() {
            base.Awake();

            var t = mDMono.transform;
            LevelLabel = t.GetComponent<UILabel>("LabelLevel");
            BreakLabel = t.GetComponent<UILabel>("Break");
            LevelBgSprite = t.GetComponent<UISprite>("LabelLevel/LevelSprite");
            AttrBGSprite = t.GetComponent<UISprite>("Property");
            QualityBorderSprite = t.GetComponent<UISprite>("Frame");
            QualityBorderSpriteBg = t.GetComponent<UISprite>("FrameBG");
            IconSprite = t.GetComponent<DynamicUISprite>("Icon");
            StarList = new UISprite[6];
            StarList[0] = t.GetComponent<UISprite>("StarList/Sprite (5)");
            StarList[1] = t.GetComponent<UISprite>("StarList/Sprite (4)");
            StarList[2] = t.GetComponent<UISprite>("StarList/Sprite (3)");
            StarList[3] = t.GetComponent<UISprite>("StarList/Sprite (2)");
            StarList[4] = t.GetComponent<UISprite>("StarList/Sprite (1)");
            StarList[5] = t.GetComponent<UISprite>("StarList/Sprite");

            StarUIGrid = t.FindEx("StarList").gameObject;
            t.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnPartnerClick));
        }

        public void SetCharTypeFx(PartnerGrade partnerGrade, Data.eRoleAttr attr)
        {
            HotfixCreateFX.ShowCharTypeFX(charFx, efClip, AttrBGSprite.transform, partnerGrade, attr);
        }

        public virtual void Fill(OtherPlayerPartnerData data,bool canOpen = true) {
            if (data == null || data.Level==0) {
                mDMono.gameObject.SetActive(false);
                return;
            }

            this.canOpen = canOpen;
            PartnerData = data;
            LevelLabel.gameObject.CustomSetActive(data.Level > 0);
            LevelLabel.text = data.Level.ToString();
            LTUIUtil.SetLevelText(LevelBgSprite,LevelLabel,data.Level+data.AllRoundLevel);
            AttrBGSprite.spriteName =LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[data.Attr]; 
            LTPartnerConfig.SetLevelSprite(AttrBGSprite,data.Attr,data.ArtifactLevel >= 0);
            HotfixCreateFX.ShowCharTypeFX(charFx, efClip, AttrBGSprite.transform, (PartnerGrade)data.QualityLevel, data.Attr);
            IconSprite.spriteName = data.Icon;

            // 新增
            if (BreakLabel != null && QualityBorderSpriteBg != null) {
                SetStarNum(data.Star, data.awakenLevel);
                int quality = 0;
                int addLevel = 0;
                LTPartnerDataManager.GetPartnerQuality(data.UpGradeId, out quality, out addLevel);
                GameItemUtil.SetColorfulPartnerCellFrame(quality, QualityBorderSpriteBg);
                HotfixCreateFX.ShowUpgradeQualityFX(upgradeFx, QualityBorderSprite.transform, quality, upgradeefclip);
                QualityBorderSprite.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[quality];

                if (addLevel > 0) {
                    BreakLabel.gameObject.SetActive(true);
                    BreakLabel.text = "+" + addLevel.ToString();
                } else {
                    BreakLabel.gameObject.SetActive(false);
                }
            } else {
            //    LevelAttrBGSprite.spriteName = UIBuddyShowItem.AttrToLevelBG(data.Attr);
                QualityBorderSprite.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[data.QualityLevel];
                GameItemUtil.SetColorfulPartnerCellFrame(data.QualityLevel, QualityBorderSpriteBg);
                HotfixCreateFX.ShowUpgradeQualityFX(upgradeFx, QualityBorderSprite.transform, data.QualityLevel, upgradeefclip);
                SetStarNum(data.Star, data.awakenLevel);
            }
            if (data != null) {
                mDMono.gameObject.CustomSetActive(true);
            }
        }

        private int lastStarCount = 0;

        private void SetStarNum(int starCount, int awakenLevel) {
            if (StarUIGrid == null) {
                UIGrid[] grids = mDMono.transform.GetComponentsInChildren<UIGrid>();

                for (int i = 0; i < grids.Length; i++) {
                    if (grids[i].name.StartsWith("star", StringComparison.CurrentCultureIgnoreCase)) {
                        StarUIGrid = grids[i].gameObject;
                    }
                }
            }

            int starWidth;

            switch (starCount) {
                case 5:
                    starWidth = 26;
                    break;
                case 6:
                    starWidth = 22;
                    break;
                default:
                    starWidth = 32;
                    break;
            }

            if (starCount != lastStarCount) {
                for (int i = 0; i < StarUIGrid.transform.childCount; i++) {
                    StarUIGrid.transform.GetChild(i).gameObject.SetActive(false);
                }

                lastStarCount = starCount;
                Vector3 firstPos = new Vector3(-(starCount - 1) / 2f * starWidth, 0, 0);

                for (int i = 0; i < starCount; i++) {
                    Transform star = StarUIGrid.transform.GetChild(i);
                    star.transform.GetComponent<UISprite>().spriteName = LTPartnerConfig.PARTNER_AWAKN_STAR_DIC[awakenLevel];
                    star.gameObject.SetActive(true);
                    star.localPosition = firstPos + new Vector3(i * starWidth, 0, 0);
                }
            }
            for (int i = 0; i < starCount; i++)
            {
                Transform star = StarUIGrid.transform.GetChild(i);
                star.transform.GetComponent<UISprite>().spriteName = LTPartnerConfig.PARTNER_AWAKN_STAR_DIC[awakenLevel];
            }
        }

        public void OnPartnerClick() {
            if (canOpen)
            {
                GlobalMenuManager.Instance.Open("LTPartnerInfoView", PartnerData);
            }
        }
    }
}
