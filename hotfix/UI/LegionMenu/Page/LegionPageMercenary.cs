using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LegionPageMercenary: DynamicMonoHotfix
    {
        public CombatPartnerCellController CellController;
        public UILabel NameLabel;
        public UISprite UITypeSprite;
        public UIProgressBar ProgressBar;
        public UILabel ProgressLabel;
        private List<LTShowItem> RewardItemList;
        public LTShowItem ItemPrefab;
        public UIGrid ItemRoot;

        public LegionMercenaryScroll Scroll;

        public Transform ShowObj;
        public Transform EmptyObj;
        public UISprite GetButton;
        public UILabel buttonLabel;

        private bool initReward;
        private bool needRefresh;
        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            ShowObj = t.FindEx("Left/BG/Show");
            EmptyObj = t.FindEx("Left/BG/Empty");
            
            CellController = t.GetMonoILRComponent<CombatPartnerCellController>("Left/BG/Show/Item");
            NameLabel = t.GetComponent<UILabel>("Left/BG/Show/NameLabel");
            UITypeSprite = t.GetComponent<UISprite>("Left/BG/Show/Type");
            ProgressBar = t.GetComponent<UIProgressBar>("Left/BG/Show/ProgressWidget/ProgressBar");
            ProgressLabel= t.GetComponent<UILabel>("Left/BG/Show/ProgressWidget/ProgressBar/Label");
            ItemRoot = t.GetComponent<UIGrid>("Left/BG/Show/RewardGrid");
            ItemPrefab = t.GetMonoILRComponent<LTShowItem>("Left/BG/Show/RewardGrid/LTShowItem");
            
            RewardItemList = new List<LTShowItem>();
            Scroll = t.GetMonoILRComponent<LegionMercenaryScroll>("Right/Scroll View/holder/Grid");
            GetButton=t.GetComponent<UISprite>("Left/BG/Show/GetBtn");
            buttonLabel = t.GetComponent<UILabel>("Left/BG/Show/GetBtn/Label");
            t.GetComponent<UIButton>("Left/BG/RuleBtn").onClick.Add(new EventDelegate(RuleBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Left/BG/Show/ChangeBtn").clickEvent.Add(new EventDelegate(ChangeBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Left/BG/Empty/ChangeBtn").clickEvent.Add(new EventDelegate(ChangeBtnClick));
            UIEventListener.Get(GetButton.gameObject).onClick = GetBtnClick;
            Messenger.AddListener(EventName.LegionMercenaryUpdateUI,NeedUpdateUI);
            initReward = false;
            needRefresh = false;
        }

        private void NeedUpdateUI()
        {
            needRefresh = true;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Messenger.RemoveListener(EventName.LegionMercenaryUpdateUI,NeedUpdateUI);
        }

        private void RuleBtnClick()
        {
            string text = EB.Localizer.GetString("ID_MERCENARY_RULE_TEXT");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text);
        }

        private void GetBtnClick(GameObject go)
        {
            bool isReward = AlliancesManager.Instance.GetIsReward();
            if (isReward)
            {
                return;
            }
            int cur = AlliancesManager.Instance.GetMercenaryTime();
            int max = AlliancesManager.Instance.GetMercenaryMaxTime();
            if (cur>=max)
            {
                //请求奖励
                AlliancesManager.Instance.ReqMercenaryReward((b) =>
                {
                    if (b)
                    {
                        GlobalMenuManager.Instance.Open("LTShowRewardView", AlliancesManager.Instance.GetMercenaryReward());
                    }
                    LegionLogic.GetInstance().IsHaveMercenary();
                    OnInit();
                });
            }
        }

        private void ChangeBtnClick()
        {
            BattleReadyHudController.Open(eBattleType.LegionMercenary, null);
        }

        public void ShowUI(bool isShow)
        {
            mDMono.gameObject.CustomSetActive(isShow);

            if (isShow)
            {
                OnInit();
                AlliancesManager.Instance.GetAllianceMercenaries((ha) =>
                {
                    List<LegionMemberData> datas= LegionModel.GetInstance().legionData.listMember;
                    datas = datas.GetRange(0,datas.Count);
                    for (int i = 0; i < datas.Count; i++)
                    {
                      LTPartnerData partnerData= AlliancesManager.Instance.hireDatas.Find((da) =>
                        {
                            var b = da.uid == datas[i].uid;
                            return b;
                        });
                      datas[i].br = partnerData?.br ?? 0;
                      if (datas[i].uid == LoginManager.Instance.LocalUserId.Value)
                      {
                          int HeroId = AlliancesManager.Instance.GetMercenaryHeroId();
                          if (HeroId > 0)
                          {
                              LTPartnerData parData = LTPartnerDataManager.Instance.GetPartnerByHeroId(HeroId);
                              datas[i].br = parData.powerData.curPower;
                          }
                          else
                          {
                              datas[i].br = 0;
                          }
                      }
                    }
                    datas.Sort((x, y) => y.br.CompareTo(x.br));
                    Scroll.SetItemDatas(datas.ToArray());
                });
            }
        }

        private void OnInit()
        {
            int HeroId = AlliancesManager.Instance.GetMercenaryHeroId();
            OnInitReward(AlliancesManager.Instance.GetMercenaryReward());
            if (HeroId>0)
            {
                ShowObj.gameObject.CustomSetActive(true);
                EmptyObj.gameObject.CustomSetActive(false);
                
                LTPartnerData parData = LTPartnerDataManager.Instance.GetPartnerByHeroId(HeroId);
                CellController.SetItem(parData);
                LTUIUtil.SetText(NameLabel,parData.HeroInfo.name);
                UITypeSprite.spriteName=LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)parData.HeroInfo.role_grade];

                ProgressBar.value = AlliancesManager.Instance.GetMercenaryTime() * 1.0f /
                                    AlliancesManager.Instance.GetMercenaryMaxTime();
                ProgressLabel.text = string.Format("{0}/{1}", AlliancesManager.Instance.GetMercenaryTime(),
                    AlliancesManager.Instance.GetMercenaryMaxTime());
            
                int cur = AlliancesManager.Instance.GetMercenaryTime();
                int max = AlliancesManager.Instance.GetMercenaryMaxTime();
                if (cur<max)
                {
                    GetButton.color =Color.magenta;
                    LTUIUtil.SetText(buttonLabel, EB.Localizer.GetString("ID_BUTTON_LABEL_PULL"));
                }
                else
                {
                    bool isReward = AlliancesManager.Instance.GetIsReward();
                    GetButton.color  = isReward?Color.magenta:Color.white;
                    LTUIUtil.SetText(buttonLabel,isReward?EB.Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL"):
                        EB.Localizer.GetString("ID_BUTTON_LABEL_PULL")); 
                }
            }
            else
            {
                ShowObj.gameObject.CustomSetActive(false);
                EmptyObj.gameObject.CustomSetActive(true);
            }
 
        }
        
        private void OnInitReward(List<LTShowItemData> dataList)
        {
            if (dataList==null || initReward)
            {
                return;
            }
            
            for (int i = 0; i < RewardItemList.Count; i++)
            {
                RewardItemList[i].mDMono.gameObject.CustomSetActive(false);
            }
            
            for (int i = 0; i < dataList.Count; i++)
            {
                if (i < RewardItemList.Count)
                {
                    RewardItemList[i].LTItemData = dataList[i];
                    RewardItemList[i].mDMono.gameObject.CustomSetActive(true);
                }
                else
                {
                    LTShowItem temp = GameObject.Instantiate(ItemPrefab.mDMono, ItemRoot.transform).transform.GetMonoILRComponent<LTShowItem>();
                    temp.LTItemData = dataList[i];
                    temp.mDMono.gameObject.CustomSetActive(true);
                    RewardItemList.Add(temp);
                }
            }
            ItemRoot.Reposition();
            initReward = true;
        }

        public void OnFocus()
        {
            if (needRefresh)
            {
                OnInit();
                Messenger.Raise(EventName.LegionMercenaryUpdateUIDelay);
                needRefresh = false;
            }
        }
    }
}