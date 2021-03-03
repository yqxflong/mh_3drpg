using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTpartnerInfoItem : DynamicCellController<Hotfix_LT.Data.HeroInfoTemplate>
    {
        public DynamicUISprite MainIcon;
        public UISprite QualityIcon;
        public UIGrid StarGrid;
        public UISprite GradeIcon;
        public UISprite FrameBG;
        public UILabel HCReward;
        public GameObject ShowNotObtained;
        public UIButton showHeroInfoTipsBtn;
        public GameObject showNewHeroFlag;
        public bool ShowGetWay = false;

        private Hotfix_LT.Data.HeroInfoTemplate m_data;
        private ParticleSystemUIComponent charFx;
        private EffectClip efClip;
        private LTPartnerData partnerData;
        private bool isShowReward;
        private int RewardNum;

        public override void Awake()
        {
            base.Awake();

            if (mDMono.ObjectParamList != null)
            {
                int count = mDMono.ObjectParamList.Count;

                if (count > 0 && mDMono.ObjectParamList[0] != null)
                {
                    MainIcon = ((GameObject)mDMono.ObjectParamList[0]).GetComponentEx<DynamicUISprite>();
                }
                if (count > 1 && mDMono.ObjectParamList[1] != null)
                {
                    QualityIcon = ((GameObject)mDMono.ObjectParamList[1]).GetComponentEx<UISprite>();
                }
                if (count > 2 && mDMono.ObjectParamList[2] != null)
                {
                    StarGrid = ((GameObject)mDMono.ObjectParamList[2]).GetComponentEx<UIGrid>();
                }
                if (count > 3 && mDMono.ObjectParamList[3] != null)
                {
                    GradeIcon = ((GameObject)mDMono.ObjectParamList[3]).GetComponentEx<UISprite>();
                }
                if (count > 4 && mDMono.ObjectParamList[4] != null)
                {
                    FrameBG = ((GameObject)mDMono.ObjectParamList[4]).GetComponentEx<UISprite>();
                }
                if (count > 5 && mDMono.ObjectParamList[5] != null)
                {
                    HCReward = ((GameObject)mDMono.ObjectParamList[5]).GetComponentEx<UILabel>();
                }
                if (count > 6 && mDMono.ObjectParamList[6] != null)
                {
                    ShowNotObtained = (GameObject)mDMono.ObjectParamList[6];
                }
                if (count > 7 && mDMono.ObjectParamList[7] != null)
                {
                    showHeroInfoTipsBtn = ((GameObject)mDMono.ObjectParamList[7]).GetComponentEx<UIButton>();
                }
            }

            if (mDMono.BoolParamList != null && mDMono.BoolParamList.Count > 0)
            {
                ShowGetWay = mDMono.BoolParamList[0];
            }

            if (showHeroInfoTipsBtn == null)
            {
                showHeroInfoTipsBtn = mDMono.gameObject.GetComponent<UIButton>();
            }

            if (showHeroInfoTipsBtn)
            {
                showHeroInfoTipsBtn.onClick.Add(new EventDelegate(ShowHeroInfoTipClick));
            }

            if (mDMono.transform.Find("NewHero") != null)
            {
                showNewHeroFlag = mDMono.transform.Find("NewHero").gameObject;
            }
        }

        public override void Clean()
        {
            if (m_data == null)
            {
                mDMono.gameObject.SetActive(false);
                return;
            }
        }

        public override void Fill(Hotfix_LT.Data.HeroInfoTemplate itemData)
        {
            if (itemData == null)
            {
                mDMono.gameObject.SetActive(false);
                return;
            }
            else
            {
                mDMono.gameObject.SetActive(true);
                m_data = itemData;
            }

            MainIcon.spriteName = m_data.icon;
            if (m_data.char_type == Hotfix_LT.Data.eRoleAttr.None)
            {
                QualityIcon.gameObject.CustomSetActive(false);
            }
            else
            {
                QualityIcon.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[m_data.char_type]; 
                string spritename = UIItemLvlDataLookup.LvlToStr((m_data.role_grade + 1).ToString());
                GradeIcon.spriteName = spritename;
                if (showHeroInfoTipsBtn!=null)
                {
                    showHeroInfoTipsBtn.normalSprite = spritename;
                }
                FrameBG.color = UIItemLvlDataLookup.GetItemFrameBGColor((m_data.role_grade + 1).ToString());
                charFx = HotfixCreateFX.ShowCharTypeFX(charFx, efClip, QualityIcon.transform, (PartnerGrade)m_data.role_grade, (Hotfix_LT.Data.eRoleAttr)m_data.char_type, 2);
            }

            if (StarGrid != null)
            {
                //星星设置和伙伴状态屏蔽
                StarGrid.gameObject.CustomSetActive(true);

                for (int i = 0; i < StarGrid.transform.childCount; i++)
                {
                    StarGrid.transform.GetChild(i).gameObject.CustomSetActive(i < m_data.init_star);
                }

                StarGrid.Reposition();
            }
            
            if (ShowNotObtained != null)
            {
                ShowNotObtained.CustomSetActive(false);
            }

            if (HCReward != null)
            {
                HCReward.gameObject.CustomSetActive(false);
            }

            if (showNewHeroFlag != null)
            {
                bool show = false;
                HeroInfoTemplate template =  CharacterTemplateManager.Instance.GetHeroInfoNewest();
                if (template != null)
                {
                    showNewHeroFlag.CustomSetActive(template.id == itemData.id);
                }
            }

            //百科处理
            if (LTDrawCardLookupController.DrawType == DrawCardType.wiki)
            {
                partnerData = LTPartnerDataManager.Instance.GetPartnerByInfoId(itemData.id);
                if (partnerData != null && partnerData.HeroId > 0)
                {
                    //读取是否可以领取收获奖励 
                    ShowNotObtained.CustomSetActive(false);
                    isShowReward = !partnerData.HasReceiveReward;
                    RewardNum = itemData.reward;

                    if (isShowReward && RewardNum > 0)
                    {
                        HCReward.text = RewardNum.ToString();
                        HCReward.gameObject.CustomSetActive(true);
                        StarGrid.gameObject.CustomSetActive(false);
                    }
                    else
                    {
                        HCReward.gameObject.CustomSetActive(false);
                    }
                }
                else  
                {
                    ShowNotObtained.CustomSetActive(true);
                }
            }
        }

        public void ShowHeroInfoTipClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            Vector2 leftSide = new Vector2(-800, 0);
            Vector2 rightSide = new Vector2(3000,0);
            Vector2 screenPos = mDMono.transform.localPosition.x > 0? rightSide : leftSide;
            System.Action act = null;

            //处理伙伴百科请求
            if (m_data != null)
            {
                if (!ShowGetWay)
                {
                    act = () =>
                    {
                        var ht = Johny.HashtablePool.Claim();
                        ht.Add("id", m_data.id);
                        ht.Add("screenPos", screenPos);
                        GlobalMenuManager.Instance.Open("LTHeroToolTipUI", ht);
                    };
                }
                else
                {
                    act = () => GlobalMenuManager.Instance.Open("LTGetHeroView", m_data.id);
                }
            }

            act?.Invoke();

            if (isShowReward && RewardNum > 0 && LTDrawCardLookupController.DrawType == DrawCardType.wiki && isCouldClickReceive)
            {
                RequestReceiveFirstGotReward();
                isCouldClickReceive = false;
            }
        }

        private bool isCouldClickReceive = true;

        private void RequestReceiveFirstGotReward(System.Action callback = null)
        {
            LTPartnerDataManager.Instance.ReceiveFirstGotReward(partnerData.HeroId, delegate (bool isSucceed)
            {
                isShowReward = false;
                isCouldClickReceive = true;

                if (isSucceed)
                {
                    List<LTShowItemData> itemList = new List<LTShowItemData>();
                    itemList.Add(new LTShowItemData("hc", RewardNum, "res"));
                    //友盟统计
                    FusionTelemetry.ItemsUmengCurrency(itemList, "伙伴百科");

                    var ht = Johny.HashtablePool.Claim();
                    ht["reward"] = itemList;
                    ht["callback"] = callback;
                    GlobalMenuManager.Instance.Open("LTShowRewardView", ht);
                    HCReward.gameObject.CustomSetActive(false);
                    StarGrid.gameObject.CustomSetActive(true);

                    //通知红点刷新
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.PartnerWikiRP);
                    //EventManager.instance.(new PartnerWikiRP());
                }
            });
        }
    }
}
