using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class PartnerTakeFieldEvent:GameEvent
    {
        public LTPartnerData Partner;
        public IHandBookAddAttrType TakeFieldCardPos;

        public PartnerTakeFieldEvent(LTPartnerData partnerData, IHandBookAddAttrType Type)
        {
            TakeFieldCardPos = Type;
            Partner = partnerData;
        }
    }
    public class HandbookTakeTheFieldItem : DynamicCellController<LTPartnerData>
    {
        public LTPartnerHandbookTakeTheFieldCtrl Conrtoller;

        public UISprite Icon, IconBorder, IconBg, BookNameBg, RoleType;
        public UIGrid Star;
        public UILabel BreakLabel, AttrAddLabel;
        private UISprite[] _stararray;

        private ParticleSystemUIComponent charFx;
        private EffectClip efClip;

        public GameObject TakeTheFieldBtn, QuitBtn, TransferBtn;
        
        UIButton HotfixBtn0;
        UIButton HotfixBtn1;
        UIButton HotfixBtn2;
        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            Conrtoller = t.parent.parent.parent.parent.parent.parent.parent.GetUIControllerILRComponent<LTPartnerHandbookTakeTheFieldCtrl>();
            Icon = t.Find("HeroIcon/Icon").GetComponent<UISprite>();
            IconBorder = t.Find("HeroIcon/Lvlborder").GetComponent<UISprite>();
            IconBg = t.Find("HeroIcon/Lvlborder/BG").GetComponent<UISprite>();
            BookNameBg = t.Find("AttrLabel/BG").GetComponent<UISprite>();
            RoleType = t.Find("HeroIcon/Attr").GetComponent<UISprite>();
            Star = t.Find("HeroIcon/Stars").GetComponent<UIGrid>();
            _stararray = new UISprite[6];
            for (int i = 0; i < 6; i++)
            {
                _stararray[i] = t.GetComponent<UISprite>($"HeroIcon/Stars/{i}1");
            }
            BreakLabel = t.Find("HeroIcon/Grade").GetComponent<UILabel>();
            AttrAddLabel = t.Find("AttrLabel").GetComponent<UILabel>();
            TakeTheFieldBtn = t.Find("Buttons/TakeTheFieldBtn").gameObject;
            QuitBtn = t.Find("Buttons/QuitBtn").gameObject;
            TransferBtn = t.Find("Buttons/TransferBtn").gameObject;
            HotfixBtn0 = t.Find("Buttons/QuitBtn").GetComponent<UIButton>();
            HotfixBtn0.onClick.Add(new EventDelegate(OnQuitBtnClick));
            HotfixBtn1 = t.Find("Buttons/TakeTheFieldBtn").GetComponent<UIButton>();
            HotfixBtn1.onClick.Add(new EventDelegate(OnTakeTheFieldBtnClick));
            HotfixBtn2 = t.Find("Buttons/TransferBtn").GetComponent<UIButton>();
            HotfixBtn2.onClick.Add(new EventDelegate(OnTransferBtnClick));
        }

        private LTPartnerData partnerData;
        private int IStarCount;
        public override void Fill(LTPartnerData itemData)
        {
            RefreshData(itemData);
        }

        public override void Clean()
        {
            RefreshData(null);
        }

        private void RefreshData(LTPartnerData itemData)
        {
            if (itemData == null)
            {
                mDMono.gameObject.CustomSetActive(false);
                return;
            }
            else
            {
                mDMono.gameObject.CustomSetActive(true);
            }
            partnerData = itemData as LTPartnerData;

            if (partnerData.HeroId > 0)
            {
                Icon.spriteName = partnerData.HeroInfo.icon;
                RoleType.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[partnerData.HeroInfo.char_type]; 
                HotfixCreateFX.ShowCharTypeFX(charFx, efClip, RoleType.transform, (PartnerGrade)partnerData.HeroInfo.role_grade, partnerData.HeroInfo.char_type);

                int quality = 0;
                int addLevel = 0;
                LTPartnerDataManager.GetPartnerQuality(partnerData.UpGradeId, out quality, out addLevel);
                if (addLevel == 0)
                {
                    BreakLabel.text = "";
                }
                else
                {
                    BreakLabel.text = string.Format("+{0}", addLevel);
                }
                BreakLabel.gameObject.CustomSetActive(addLevel != 0);
                IconBorder.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[quality];
                GameItemUtil.SetColorfulPartnerCellFrame(quality, IconBg);

                if (IStarCount != partnerData.Star)
                {
                    IStarCount = partnerData.Star;
                    int awakelevel = partnerData.IsAwaken;
                    for (int i = 0; i < Star.transform.childCount; ++i)
                    {
                        Star.transform.GetChild(i).gameObject.CustomSetActive(i < IStarCount);
                        _stararray[i].spriteName = LTPartnerConfig.PARTNER_AWAKN_STAR_DIC[awakelevel];//觉醒星星显示
                    }
                    Star.cellWidth = (IStarCount > 5) ? 25 : 30;
                    Star.Reposition();
                }
                SetBookNameBg(partnerData.HeroInfo.char_type, BookNameBg);
                SetAttAddNum(partnerData, Conrtoller.Data.handBookAddAttrType);
                SetBtn(LTPartnerHandbookManager.Instance.TheHandbookList.IsPartnerInField(partnerData.HeroId.ToString()), string.IsNullOrEmpty(Conrtoller.Data.BuddyId) ? false : Conrtoller.Data.BuddyId.Equals(partnerData.HeroId.ToString()));
            }
            else
            {
                mDMono.gameObject.CustomSetActive(false);
            }
        }

        private void SetBookNameBg(Hotfix_LT.Data.eRoleAttr type, UISprite Bg)
        {
            if (Bg == null)
            {
                EB.Debug.LogError("Hero Type Icon Set Error");
                return;
            }

            float ColorR = 255;
            float ColorG = 255;
            float ColorB = 255;
            float ColorA = 1;

            switch (type)
            {
                case Hotfix_LT.Data.eRoleAttr.Feng:
                    ColorR = 66;
                    ColorG = 254;
                    ColorB = 121;
                    Bg.gameObject.CustomSetActive(true);
                    break;
                case Hotfix_LT.Data.eRoleAttr.Shui:
                    ColorR = 51;
                    ColorG = 178;
                    ColorB = 255;
                    Bg.gameObject.CustomSetActive(true);
                    break;
                case Hotfix_LT.Data.eRoleAttr.Huo:
                    ColorR = 255;
                    ColorG = 102;
                    ColorB = 153;
                    Bg.gameObject.CustomSetActive(true);
                    break;
                default:
                    ColorR = 255;
                    ColorG = 255;
                    ColorB = 255;
                    Bg.gameObject.CustomSetActive(false);
                    break;
            }
            Bg.color = new Color(ColorR / 255, ColorG / 255, ColorB / 255, ColorA);
        }


        public void OnTakeTheFieldBtnClick()
        {
            switch (partnerData.HeroInfo.role_grade)
            {
                case 1:
                    FusionAudio.PostEvent("UI/New/ShangZhen", true);
                    break;
                case 2:
                    FusionAudio.PostEvent("UI/New/ShangZhenR", true);
                    break;
                case 3:
                    FusionAudio.PostEvent("UI/New/ShangZhenSR", true);
                    break;
                case 4:
                    FusionAudio.PostEvent("UI/New/ShangZhenSSR", true);
                    break;
                case 5:
                    FusionAudio.PostEvent("UI/New/ShangZhenSSR", true);
                    break;
            }
            LTPartnerHandbookManager.Instance.TakeTheField(partnerData.HeroId.ToString(), (int)Conrtoller.Data.handbookId, Conrtoller.Data.index, delegate {
                Messenger.Raise(Hotfix_LT.EventName.PartnerTakeFieldEvent,new PartnerTakeFieldEvent(partnerData, Conrtoller.Data.handBookAddAttrType));
                Conrtoller.OnCancelButtonClick();

            });
        }

        public void OnQuitBtnClick()
        {
            LTPartnerHandbookManager.Instance.QuitTheField(partnerData.HeroId.ToString(), (int)Conrtoller.Data.handbookId, Conrtoller.Data.index, delegate {
                Conrtoller.OnCancelButtonClick();

            });
        }

        public void OnTransferBtnClick()
        {
            var temp = LTPartnerHandbookManager.Instance.TheHandbookList.Find(partnerData.HeroId.ToString());
            if (temp != null)
            {
                switch (partnerData.HeroInfo.role_grade)
                {
                    case 1:
                        FusionAudio.PostEvent("UI/New/ShangZhen", true);
                        break;
                    case 2:
                        FusionAudio.PostEvent("UI/New/ShangZhenR", true);
                        break;
                    case 3:
                        FusionAudio.PostEvent("UI/New/ShangZhenSR", true);
                        break;
                    case 4:
                        FusionAudio.PostEvent("UI/New/ShangZhenSSR", true);
                        break;
                    case 5:
                        FusionAudio.PostEvent("UI/New/ShangZhenSSR", true);
                        break;
                }
                LTPartnerHandbookManager.Instance.TransferField(partnerData.HeroId.ToString(), (int)temp.handbookId, (int)Conrtoller.Data.handbookId, temp.index, Conrtoller.Data.index, delegate {
                    Messenger.Raise(Hotfix_LT.EventName.PartnerTakeFieldEvent,new PartnerTakeFieldEvent(partnerData, Conrtoller.Data.handBookAddAttrType));
                    Conrtoller.OnCancelButtonClick();
                });
            }
            else
            {
                EB.Debug.LogError("HandbookTakeTheFieldDynamicCellController OnTransferBtnClick Can't Find!");
            }
        }

        /// <summary>
        /// 设置按钮状态
        /// </summary>
        /// <param name="takeTheFieldState">是否在阵上</param>
        /// <param name="isThisPartner">是否是这个伙伴</param>
        private void SetBtn(bool takeTheFieldState, bool isThisPartner)
        {
            if (takeTheFieldState)
            {
                if (isThisPartner)
                {
                    QuitBtn.CustomSetActive(true);
                    TransferBtn.CustomSetActive(false);
                }
                else
                {
                    QuitBtn.CustomSetActive(false);
                    TransferBtn.CustomSetActive(true);
                }
            }
            else
            {
                QuitBtn.CustomSetActive(false);
                TransferBtn.CustomSetActive(false);
            }

            TakeTheFieldBtn.CustomSetActive(!takeTheFieldState);

        }

        StringBuilder AddStr = new StringBuilder();
        private void SetAttAddNum(LTPartnerData partnerData, IHandBookAddAttrType Type)
        {
            string Addtxt = LTPartnerHandbookManager.Instance.GetAttAddNum(partnerData, Type);
            AttrAddLabel.text = Addtxt;
        }
    }
}
