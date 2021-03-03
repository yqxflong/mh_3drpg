using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
	public class LTVIPRewardHudController : UIControllerHotfix
	{
		private GameObject m_PrivilegeItemPrefab;
		private GameObject m_GiftItemPrefab;
		private List<LTShowItem> m_ShowItems;
		private bool isFirstEnter;
		private bool opRewardUI;
		private bool waitResult;

		public override void Awake()
		{
			base.Awake();

			controller.BindingBtnEvent(new List<string>() { "RuleBtn", "ReceiveBtn", "PagingLeft", "PagingRight" }, 
				new List<EventDelegate>() {
				new EventDelegate(() => { OnRuleBtnClicked(); }), new EventDelegate(() => { OnReceiveBtnClicked(); }),
				new EventDelegate(() => { OnPagingLeftBtnClicked(); }), new EventDelegate(() => { OnPagingRightBtnClicked(); })
			});
			m_ShowItems = new List<LTShowItem>();
		}

		public override IEnumerator OnAddToStack()
		{
			m_PrivilegeItemPrefab = controller.GObjects["PrivilegeItem"];
			m_GiftItemPrefab = controller.GObjects["GiftItem"];
			isFirstEnter = true;

			yield return base.OnAddToStack();
		}

		public override IEnumerator OnRemoveFromStack()
		{
            ResetUIScrollView(controller.UiGrids["PrivilegeGrid"], 99);
            DestroySelf();
			yield break;
		}

		public override bool IsFullscreen()
		{
			return true;
		}

		public override void Show(bool isShowing)
		{
			base.Show(isShowing);
			if (!isShowing) { if (!opRewardUI) LTVIPDataManager.Instance.ResetCheckedLevel(); return; }
			
			LTVIPDataManager.Instance.UpdateVIPBaseData();

			int level = LTVIPDataManager.Instance.GetCheckedLevel();
			OnRefreshTopVIPBaseInfo();

			LTVIPDataManager.Instance.BrowsePrivilegesAndGiftsPage(level, !opRewardUI, (data) => {
				OnRefurbishRewardInfo(data);
			});
			isFirstEnter = false;
			opRewardUI = false;
		}
		
		public override void StartBootFlash()
		{
			
		}

		private void OnRuleBtnClicked()
		{
			GlobalMenuManager.Instance.Open("LTRuleUIView", LTVIPDataManager.Instance.GetVIPEXPRuleWords());
		}

		private void OnReceiveBtnClicked()
		{
			if (waitResult) return;

			int level = LTVIPDataManager.Instance.GetCheckedLevel();
			VIPGiftStatus status = LTVIPDataManager.Instance.GetTheVIPLevelGiftStatus(level);
			if (status == VIPGiftStatus.Locked)
			{
				MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_REWARD_NO_TIP"));
				return;
			}
			waitResult = true;

			LTVIPDataManager.Instance.RequestCollectGifts(level, (result) => {
				List<LTShowItemData> list = LTVIPDataManager.Instance.GetVIPGiftItemData();
				opRewardUI = true;
				GlobalMenuManager.Instance.Open("LTShowRewardView", list);
				OnRefreshVIPGiftButtonStatus();
				waitResult = false;
			});
		}

		private void OnPagingLeftBtnClicked()
		{
			int level = LTVIPDataManager.Instance.GetCheckedLevel() - 1;
			LTVIPDataManager.Instance.BrowsePrivilegesAndGiftsPage(level, false, (data) => {
				OnRefurbishRewardInfo(data);
			});
		}

		private void OnPagingRightBtnClicked()
		{
			int level = LTVIPDataManager.Instance.GetCheckedLevel() + 1;
			LTVIPDataManager.Instance.BrowsePrivilegesAndGiftsPage(level, false, (data) => {
				OnRefurbishRewardInfo(data);
			});
		}

		private void OnRefurbishRewardInfo(Dictionary<string, VIPPrivilegeItem> data)
		{			
			OnRefreshVIPPrivilegeUIInfo(LTVIPDataManager.Instance.GetSortPrivilegeList(data));
			OnRefreshVIPGiftUIInfo();
			OnRefreshVIPGiftButtonStatus();
			OnRefreshPagingBtnStatus();
		}

		private void OnRefreshTopVIPBaseInfo()
		{
			VIPBaseInfo info = LTVIPDataManager.Instance.GetVIPBaseInfo();
			controller.UiLabels["Level"].text = info.Level.ToString();			

			if(info.Level == LTVIPDataManager.Instance.GetLevelCount())
			{
				controller.UiLabels["Experience"].text = string.Concat(info.CurrentExp.ToString(), "/MAX");
                controller.UiLabels["NecessaryExp"].text = EB.Localizer.GetString("ID_HAS_MAX_LEVEL");
				controller.UiLabels["ExpFetch"].text = string.Empty;
			}
			else
			{
				controller.UiLabels["Experience"].text = string.Concat(info.CurrentExp.ToString(), "/", info.FullExp);
				controller.UiLabels["NecessaryExp"].text = string.Concat("[FFF54B]", info.NecessaryExp.ToString(), string.Format("[-] {0} [FFF54B]VIP{1}",EB.Localizer.GetString("ID_VIP_REWARD_TIP1") ,info.NextLevelText));
				controller.UiLabels["NecessaryExp"].transform.GetChild(0).GetComponent<UILabel>().text = string.Concat(info.NecessaryExp.ToString(),string.Format ( " {0} VIP{1}", EB.Localizer.GetString("ID_VIP_REWARD_TIP1"), info.NextLevelText));
				controller.UiLabels["ExpFetch"].text = EB.Localizer .GetString("ID_REGAIN");
			}

			controller.UiProgressBars["ExpertProgressBar"].value = info.CurrentExp / (float)info.FullExp;
		}

		private void OnRefreshVIPPrivilegeUIInfo(List<KeyValuePair<string, VIPPrivilegeItem>> list)
		{
			UIGrid gridParent = controller.UiGrids["PrivilegeGrid"];
			List<Transform> itemList = gridParent.GetChildList();
			if(itemList != null) itemList.Sort((left, right) => { if (left.localPosition.y < right.localPosition.y) { return 1; } else return -1; });

			int index = 0;
			int posindex = 0;
			for(int i = 0; i < list.Count; i++)
			{
				VIPPrivilegeItem itemData = list[i].Value;
				GameObject itemObject = null;
				if (itemList != null && index < itemList.Count) {
					itemObject = itemList[index].gameObject;
				} else {
					itemObject = Object.Instantiate(m_PrivilegeItemPrefab);
					gridParent.AddChild(itemObject.transform);
				}
				itemObject.GetComponent<UIWidget>().alpha = 1;
				UILabel label = itemObject.transform.Find("Content").GetComponent<UILabel>();
				UISprite icon = itemObject.transform.Find("Icon").GetComponent<UISprite>();
				label.transform.gameObject.CustomSetActive(true);
				icon.transform.gameObject.CustomSetActive(true);
				itemObject.transform.localScale = Vector3.one;
				label.text = LTVIPDataManager.Instance.GetPrivilegeContent(itemData, list[i].Key);
				icon.color = itemData.Status == PrivilegeStatus.Newly ? Color.yellow : Color.white;
				posindex = label.height / 40-1;
				if (posindex > 0)
				{
                    for (int j = 0; j < posindex; j++)
                    {
						index++;
						if (itemList != null && index < itemList.Count)
						{
							itemObject = itemList[index].gameObject;
						}
						else
						{
							itemObject = Object.Instantiate(m_PrivilegeItemPrefab);
							gridParent.AddChild(itemObject.transform);
						}
						itemObject.transform.Find("Icon").gameObject.CustomSetActive(false);
						itemObject.transform.Find("Content").gameObject.CustomSetActive(false);
						itemObject.transform.localScale = Vector3.one;
					}
				}
				index++;
			}

			if(itemList != null && index < itemList.Count)
			{
				for (int i = index; i < itemList.Count; i++)
					itemList[i].gameObject.GetComponent<UIWidget>().alpha = 0;
			}

			controller.GObjects["PrivilegeArrow"].SetActive(index > 12);

			gridParent.Reposition();
			ResetUIScrollView(gridParent, index);
		}

		private void OnRefreshVIPGiftUIInfo()
		{
			UIGrid gridParent = controller.UiGrids["GiftGrid"];
			List<LTShowItemData> itemDatas = LTVIPDataManager.Instance.GetVIPGiftItemData();
			UpdateShowItems(itemDatas.Count, gridParent);

			for (int i=0; i < itemDatas.Count; i++)
			{
				if(i < m_ShowItems.Count)
				{
					m_ShowItems[i].LTItemData = itemDatas[i];
					m_ShowItems[i].DisappearName();
				}
			}
			controller.GObjects["GiftArrow"].SetActive(itemDatas.Count > 4);

			ResetUIScrollView(gridParent, itemDatas.Count, 5);
			gridParent.Reposition();
		}

		private void ResetUIScrollView(UIGrid grid, int count, int limit = 12)
		{
			UIScrollView scrollView = grid.transform.parent.GetComponent<UIScrollView>();
			scrollView.enabled = count >= limit;
			if (isFirstEnter) return;			
			scrollView.SetDragAmount(0, 0, false);			
		}

		private void OnRefreshVIPGiftButtonStatus()
		{
			UIButton button = controller.UiButtons["ReceiveBtn"];
			VIPGiftStatus status = LTVIPDataManager.Instance.GetCurCheckedLevelGiftsStatus();
			switch(status)
			{
				case VIPGiftStatus.Received:
					button.GetComponent<Collider>().enabled = false;
					button.GetComponentInChildren<UISprite>().color = Color.magenta;
					LTUIUtil.SetText(button.GetComponentInChildren<UILabel>(), EB.Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL"));
					controller.GObjects["ButtonRedPoint"].SetActive(false);
					break;
				case VIPGiftStatus.Uncollected:
					button.GetComponent<Collider>().enabled = true;
					button.GetComponentInChildren<UISprite>().color = Color.white;
					LTUIUtil.SetText(button.GetComponentInChildren<UILabel>(), EB.Localizer.GetString("ID_BUTTON_LABEL_PULL"));
					controller.GObjects["ButtonRedPoint"].SetActive(true);
					break;
				case VIPGiftStatus.Locked:
					button.GetComponent<Collider>().enabled = true;
					button.GetComponentInChildren<UISprite>().color = Color.magenta;
					LTUIUtil.SetText(button.GetComponentInChildren<UILabel>(), EB.Localizer.GetString("ID_BUTTON_LABEL_PULL"));
					controller.GObjects["ButtonRedPoint"].SetActive(false);
					break;
			}
		}

		private void UpdateShowItems(int necessaryCount, UIGrid gridParent)
		{
			if (m_ShowItems.Count > necessaryCount)
			{
				for (int i = 0; i < necessaryCount; i++)
				{
					m_ShowItems[i].mDMono.transform.gameObject.SetActive(true);
				}
				for (int i = necessaryCount; i < m_ShowItems.Count; i++)
				{
					m_ShowItems[i].mDMono.transform.gameObject.SetActive(false);
				}
			}
			else
			{
				for (int i = 0; i < m_ShowItems.Count; i++)
				{
					m_ShowItems[i].mDMono.transform.gameObject.SetActive(true);
				}
				if(necessaryCount > m_ShowItems.Count)
				{
					int begin = m_ShowItems.Count;
					for (int i = begin; i < necessaryCount; i++)
					{
						GameObject itemObject = Object.Instantiate(m_GiftItemPrefab, gridParent.transform);
						gridParent.AddChild(itemObject.transform);
						itemObject.transform.localScale = Vector3.one;
						LTShowItem showItem = itemObject.GetMonoILRComponent<LTShowItem>();
						m_ShowItems.Add(showItem);
						itemObject.SetActive(true);
					}
				}
			}
		}

		private void OnRefreshPagingBtnStatus()
		{
			controller.UiLabels["Title"].text = EB.StringUtil.Format("VIP{0} {1}", LTVIPDataManager.Instance.GetCheckedLevel(), EB.Localizer .GetString ("ID_PRIVILEGE"));

			int checkedLevel = LTVIPDataManager.Instance.GetCheckedLevel();

			UIButton button = controller.UiButtons["PagingLeft"];		
			button.GetComponent<BoxCollider>().enabled = checkedLevel > 0;
			button.enabled = checkedLevel > 0 ? true : false;
			button.disabledColor = Color.magenta;
			button.GetComponentInChildren<UISprite>().color = checkedLevel > 0 ? Color.white : Color.magenta;
			button.defaultColor = checkedLevel > 0 ? Color.white : Color.magenta;			
			controller.GObjects["PagingRedPointLeft"].SetActive(LTVIPDataManager.Instance.IsExistsGreaterOrLessThanTheLevel(checkedLevel, false));

			int maxLevel = LTVIPDataManager.Instance.GetMaxShowLevel();
			button = controller.UiButtons["PagingRight"];
			button.GetComponent<BoxCollider>().enabled = checkedLevel < maxLevel;
			button.enabled = checkedLevel < maxLevel ? true : false;
			button.disabledColor = Color.magenta;
			button.GetComponentInChildren<UISprite>().color = checkedLevel < maxLevel ? Color.white : Color.magenta;
			button.defaultColor = checkedLevel < maxLevel ? Color.white : Color.magenta;			
			controller.GObjects["PagingRedPoint"].SetActive(LTVIPDataManager.Instance.IsExistsGreaterOrLessThanTheLevel(checkedLevel, true));
		}
	}
}