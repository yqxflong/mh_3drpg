using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTCombatScrollBag : DynamicMonoHotfix
    {
        public GameObject Container;
        public bool IsHudVisible { get { return Container.activeSelf; } }
        public LTChallengeInstanceBagScroll DynamicScroll;
        public TweenScale ScrollTipsGO;
        public LTShowItem SkillTipsItem;
        public UILabel SkillNameLabel;
        public UILabel CostMagicLabel;
        public UILabel DescLabel;
        private List<LTChallengeInstanceBagData> BagDatas = new List<LTChallengeInstanceBagData>();
        private LTChallengeInstanceBagCell FirstBagCell;
        private LTChallengeInstanceBagCell CurrentSelected;
        bool IsSetPanelSortOrder;
        bool IsInitData;
        bool IsUseOver;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            Container = t.FindEx("Container").gameObject;

            DynamicScroll = t.GetMonoILRComponent<LTChallengeInstanceBagScroll>("Container/BottomRight/Right/Scroll/PlaceHolder/Grid");
            DynamicScroll.SetOnBtnClickAction(OnSkillScrollItemClick);
            FirstBagCell = DynamicScroll.mDMono.transform.GetMonoILRComponent<LTChallengeInstanceBagCell>("Row/Item");
            ScrollTipsGO = t.GetComponent <TweenScale>("Container/SkillTips");
            SkillTipsItem = t.GetMonoILRComponent<LTShowItem>("Container/SkillTips/TweenHUD/UpGroup/LTShowItem");
            SkillNameLabel = t.GetComponent<UILabel>("Container/SkillTips/TweenHUD/UpGroup/Label");
            CostMagicLabel = t.GetComponent<UILabel>("Container/SkillTips/TweenHUD/UpGroup/Magic/Label");
            DescLabel = t.GetComponent<UILabel>("Container/SkillTips/TweenHUD/Desc");

            t.GetComponent<UIButton>("Container/BottomRight/Bg/Top/RuleBtn").onClick.Add(new EventDelegate(OnRuleBtnClick));
            t.GetComponent<UIButton>("Container/BottomRight/Bg/Top/CancelBtn").onClick.Add(new EventDelegate(OnClose));
            t.GetComponent<UIButton>("Container/SkillTips/TweenHUD/UseBtn").onClick.Add(new EventDelegate(OnUseSkillBtnClick));

            GameObject go = t.FindEx("Container/BottomRight/Right/Scroll/PlaceHolder/Grid/Row/Item").gameObject;
            GameObject go1 = t.FindEx("Container/BottomRight/Right/Scroll/PlaceHolder/Grid/Row/Item (1)").gameObject;
            GameObject go2 = t.FindEx("Container/BottomRight/Right/Scroll/PlaceHolder/Grid/Row/Item (2)").gameObject;
            GameObject go3 = t.FindEx("Container/BottomRight/Right/Scroll/PlaceHolder/Grid/Row/Item (3)").gameObject;

            t.GetComponent<UIEventTrigger>("Container/Background").onClick.Add(new EventDelegate(OnClose));
        }
    
    	public void Open()
    	{
    		if (!IsInitData||IsUseOver)
    		{
    			IsInitData = true;
    			IsUseOver = false;
    			BagDatas.Clear();

				var scrollList = Combat.CombatSyncData.Instance.GetMyTeamData().ScrollList;
				for (var i = 0; i < scrollList.Count; i++)
				{
					var data = scrollList[i];
					if (data.count > 0)
					{
						BagDatas.Add(new LTChallengeInstanceBagData(data.type, data.id, data.count));
					}
				}
				BagDatas.Sort((a, b) => {
                    int aid = 0;
                    int.TryParse(a.Id, out aid);
                    int bid = 0;
                    int.TryParse(b.Id, out bid);
                    if (aid < bid)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                });

                if (BagDatas.Count < 20)
                {
                    for (int i = BagDatas.Count; i < 20;++i)
                    {
                        BagDatas.Add(new LTChallengeInstanceBagData(LTShowItemType.TYPE_GAMINVENTORY, string.Empty, 0));
                    }
                }
                Create(BagDatas);
    			if (!IsSetPanelSortOrder)
    			{
    				IsSetPanelSortOrder = true;
    				UIPanel up1 = Container.GetComponent<UIPanel>();
    				up1.sortingOrder = up1.sortingOrder + 5;
    				UIPanel up2 = DynamicScroll.mDMono.transform.parent.parent.gameObject.GetComponent<UIPanel>();
                    up2.sortingOrder = up2.sortingOrder + 5;
    			}
            }

            if (!Container.activeSelf)
            {
                UITweener tw = Container.GetComponentInChildren<UITweener>();
                tw.tweenFactor = 0;
                tw.PlayForward();
            }
            Container.gameObject.CustomSetActive(!Container.activeSelf);

        }
    
    	private void Create(List<LTChallengeInstanceBagData> itemDatas)
    	{
            itemDatas[0].IsSelect = true;
            CurrentSelected = FirstBagCell;
            SetSkillTipInfo(itemDatas[0]);
            DynamicScroll.SetItemDatas(itemDatas);
        }
    
        public void OnSkillScrollItemClick(LTChallengeInstanceBagCell cell)
    	{
            if (string.IsNullOrEmpty(cell.CellData.Id))
            {
                return;
            }
            FusionAudio.PostEvent("UI/General/ButtonClick", true);
            if (CurrentSelected != null)
            {
                CurrentSelected.CellData.IsSelect = false;
                CurrentSelected.SetItemSelect(false);
            }

            CurrentSelected = cell;
            CurrentSelected.CellData.IsSelect = true;
            CurrentSelected.SetItemSelect(true);

            SetSkillTipInfo(CurrentSelected.CellData);
    	}

        private void SetSkillTipInfo(LTChallengeInstanceBagData data)
        {
            ScrollTipsGO.gameObject.CustomSetActive(false);
            if (!string.IsNullOrEmpty(data.Id))
            {
                Hotfix_LT.Data.SkillTemplate skillTpl = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(int.Parse(data.Id));
                SkillTipsItem.LTItemData = new LTShowItemData(data.Id,data.Num,data.Type);
                SkillNameLabel.text = skillTpl.Name;
                CostMagicLabel.text = skillTpl.SPCost.ToString();
                DescLabel.text = skillTpl.Description;
                ScrollTipsGO.ResetToBeginning();
                ScrollTipsGO.PlayForward();
                ScrollTipsGO.gameObject.CustomSetActive(true);
            }
        }
    
    	public void OnUseSkillBtnClick()
    	{
    		if (CurrentSelected.ShowItem.LTItemData.count <= 0)
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTCombatScrollBag_3071"));
    			return;
    		}
    
    		Hotfix_LT.Data.SkillTemplate skillTpl = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(int.Parse(CurrentSelected.ShowItem.LTItemData.id));
    		if (skillTpl.SPCost > Hotfix_LT.Combat.CombatSyncData.Instance.GetMyTeamData().SPoint)
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTCombatScrollBag_3374"));
    			return;
    		}
    
    		LTCombatHudController.Instance.CombatSkillCtrl.OnSelectScrollEvent(int.Parse(CurrentSelected.ShowItem.LTItemData.id));
    		OnClose();
    	}
    
    	public void OnRuleBtnClick()
    	{
            GlobalMenuManager.Instance.Open("LTRuleUIView", EB.Localizer.GetString("ID_RULE_CHALLENGE_BAG"));
        }
    
    	public void OnUse()
    	{
    		CurrentSelected.CellData.Num -= 1;
    		IsUseOver = CurrentSelected.CellData.Num <= 0;
    		CurrentSelected.ShowItem.LTItemData.count -= 1;
    		CurrentSelected.ShowItem.LTItemData = CurrentSelected.ShowItem.LTItemData;
    		SkillTipsItem.LTItemData = CurrentSelected.ShowItem.LTItemData;
    	}
    
    	public void OnClose()
    	{
    		Container.gameObject.CustomSetActive(false);
    	}
    }
}
