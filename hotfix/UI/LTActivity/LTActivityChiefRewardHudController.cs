using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
	public class LTActivityChiefRewardHudController : UIControllerHotfix
	{
		private int m_ActivityId;
		private List<Data.TimeLimitActivityStageTemplate> m_TimeRewardlist;
		private int m_WinCount;
		private List<LTActivityChiefRewardData> m_ChiefRewardList;

		private UILabel m_WinCountLabel;
		private UIGrid m_Grid;
		private UIScrollView m_ScrollView;
		private GameObject m_PrefabTemplate;
	
		public override void Awake()
		{
			base.Awake();

			Transform t = controller.transform;

			m_WinCountLabel = t.GetComponent<UILabel>("Content/StatusArea/Count");
			m_Grid = t.GetComponent<UIGrid>("Content/ScrollView/Placehodler/Grid");
			m_ScrollView = t.GetComponent<UIScrollView>("Content/ScrollView");
			m_PrefabTemplate = t.Find("Content/ScrollView/Placehodler/Template").gameObject;

			UIButton backButton = t.GetComponent<UIButton>("Frame/BG/Top/CloseBtn");
			backButton.onClick.Add(new EventDelegate(OnCancelButtonClick));
		}

		public override bool ShowUIBlocker { get { return true; } }

		public override bool IsFullscreen() { return false; }

		public override void SetMenuData(object param)
		{
			m_ActivityId = param != null ? (int)param : 6519;
			Init();
		}

		private void Init()
		{
			m_TimeRewardlist = Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(m_ActivityId);

			DataLookupsCache.Instance.SearchDataByID("tl_acs." + m_ActivityId, out Hashtable activityData);
			m_WinCount = EB.Dot.Integer("current", activityData, 0);

			RefreshRewardData(activityData);
		}

		public void RefreshRewardData(Hashtable activityData)
		{		
			m_ChiefRewardList = new List<LTActivityChiefRewardData>(4);
			for (int i = 0; i < m_TimeRewardlist.Count; i++)
			{
				var template = m_TimeRewardlist[i];
				LTActivityChiefRewardData itemData = new LTActivityChiefRewardData();

				int receivedValue = EB.Dot.Integer(string.Format("stages.{0}", template.id), activityData, 0);

				itemData.ActivityID = m_ActivityId;
				itemData.HasReceived = receivedValue == 1;
				itemData.RequiredWins = template.stage;
				itemData.ID = template.id;
				itemData.WinCount = m_WinCount;
				itemData.RewardData = new List<LTShowItemData>();

				for (int j = 0; j < template.reward_items.Count; j++)
				{
					ItemStruct rewardStruct = template.reward_items[j];
					itemData.RewardData.Add(new LTShowItemData(rewardStruct.id, rewardStruct.quantity, rewardStruct.type));
				}

				itemData.Controller = this;
				m_ChiefRewardList.Add(itemData);
			}
		}

		public override void Show(bool isShowing)
		{
			base.Show(isShowing);

			if (isShowing)
				OnShow();
			else
				OnHide();
		}

		private void OnShow()
		{
			RefreshUITopBaseInfo();
			RefreshChiefRewardListView();
		}

		private void OnHide(){}

		private void RefreshUITopBaseInfo()
		{
			m_WinCountLabel.text = m_WinCount.ToString();
		}

		private void RefreshChiefRewardListView()
		{
			List<Transform> childList = m_Grid.GetChildList();
			int rowCount = m_ChiefRewardList.Count;

			for (int i = 0; i < rowCount; i++)
			{
				LTActivityChiefRewardData itemData = m_ChiefRewardList[i];

				GameObject itemObject = null;
				if (childList != null && i < childList.Count)
				{
					itemObject = childList[i].gameObject;
				}
				else
				{
					itemObject = Object.Instantiate(m_PrefabTemplate);					
					m_Grid.AddChild(itemObject.transform);
					itemObject.transform.localScale = Vector3.one;
				}

				LTActivityChiefRewardItem item = itemObject.transform.GetMonoILRComponent<LTActivityChiefRewardItem>();
				item.FillData(itemData);
				item.Show(itemObject);
			}

			if (childList != null && rowCount < childList.Count)
			{
				for (int i = rowCount; i < childList.Count; i++)
					childList[i].GetMonoILRComponent<LTActivityChiefRewardItem>().Hide();
			}

			m_Grid.Reposition();
			TimerManager.instance.AddFramer(1, 1, (_)=> { ResetUIScrollView(rowCount); });
		}

		private void ResetUIScrollView(int count, int limit = 4)
		{
			if (m_ScrollView == null) return;
			m_ScrollView.enabled = count >= limit;
			m_ScrollView.SetDragAmount(0, 0, false);
		}
	}

	public class LTActivityChiefRewardData
	{
		public int ID;
		public int ActivityID;
		public int RequiredWins;
		public bool HasReceived;
		public int WinCount;

		public List<LTShowItemData> RewardData;

		public LTActivityChiefRewardHudController Controller;
	}

	public class LTActivityChiefRewardItem : DynamicMonoHotfix
	{
		private UILabel m_RequiredWinsValue, m_BtnDesc;
		private LTShowItem[] m_ShowItems;
		private ConsecutiveClickCoolTrigger m_RecieveBtn;
		private UISprite m_RedPoint;

		private LTActivityChiefRewardData m_ItemData;

		public override void Awake()
		{
			Transform t = mDMono.transform;

			m_ShowItems = new LTShowItem[3];
			m_ShowItems[0] = t.GetMonoILRComponent<LTShowItem>("Awards/LTShowItem");
			m_ShowItems[1] = t.GetMonoILRComponent<LTShowItem>("Awards/LTShowItem (1)");
			m_ShowItems[2] = t.GetMonoILRComponent<LTShowItem>("Awards/LTShowItem (2)");
			m_RecieveBtn = t.GetComponent<ConsecutiveClickCoolTrigger>("RecieveBtn");
			m_RecieveBtn.clickEvent.Add(new EventDelegate(SendRecieveReward));
			m_RequiredWinsValue = t.GetComponent<UILabel>("Count");
			m_BtnDesc = t.GetComponent<UILabel>("RecieveBtn/Label");
			m_RedPoint = t.GetComponent<UISprite>("RecieveBtn/RedPoint");
		}

		public void FillData(LTActivityChiefRewardData data)
		{
			m_ItemData = data;
		}

		public void Show(GameObject selfObject)
		{
			selfObject.GetComponent<UIWidget>().alpha = 1;

			Refresh();
		}

		public void Hide()
		{
			mDMono.GetComponent<UIWidget>().alpha = 0;
		}

		private void Refresh()
		{
			if (m_ItemData == null)
			{
				mDMono.gameObject.CustomSetActive(false);
				return;
			}

			// left number text
			m_RequiredWinsValue.text = m_ItemData.RequiredWins.ToString();

			// reward items
			int dataCount = m_ItemData.RewardData.Count;
			for (int i = 0; i < m_ShowItems.Length; i++)
			{
				if (i < dataCount)
				{
					m_ShowItems[i].LTItemData = m_ItemData.RewardData[i];
				}
				else
				{
					m_ShowItems[i].mDMono.gameObject.CustomSetActive(false);
				}
			}

			// button state
			bool isGrey = m_ItemData.HasReceived || m_ItemData.WinCount < m_ItemData.RequiredWins;
			m_RecieveBtn.GetComponent<UISprite>().color = isGrey ? Color.magenta : Color.white;
			m_RecieveBtn.GetComponent<BoxCollider>().enabled = !isGrey;
			m_BtnDesc.text = m_ItemData.HasReceived ? EB.Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL") : EB.Localizer.GetString("ID_BUTTON_LABEL_PULL");

			UpdateRedPoint();
		}

		private void SendRecieveReward()
		{
			if (m_ItemData.RequiredWins > m_ItemData.WinCount)
			{
				MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_REWARD_NO_TIP"));
				return;
			}

			CheckException();
			EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/specialactivity/gotReward");
			request.AddData("activityId", m_ItemData.ActivityID);
			request.AddData("stageId", m_ItemData.ID);
			LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
			{
				DataLookupsCache.Instance.CacheData(data);
				List<LTShowItemData> awardDatas = GameUtils.ParseAwardArr(Hotfix_LT.EBCore.Dot.Array("reward", data, null));
				GlobalMenuManager.Instance.Open("LTShowRewardView", awardDatas);
				Messenger.Raise(EventName.OnURScoreRewardRecieve);

				DataLookupsCache.Instance.SearchDataByID("tl_acs." + m_ItemData.ActivityID, out Hashtable activityData);
				m_ItemData.Controller.RefreshRewardData(activityData);
			});
		}

		private void CheckException()
		{
			LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) =>
			{
				if (response.error != null)
				{
					string strObjects = (string)response.error;
					string[] strObject = strObjects.Split(",".ToCharArray(), 2);
					switch (strObject[0])
					{
						case "insufficient num":
							{
								MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ACTIVITY_REALM_CHALLENGE_ERROR"));
								LTMainHudManager.Instance.UpdateActivityLoginData(null);//刷新界面
								return true;
							}
						case "stage not reach":
							{
								MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ACTIVITY_REALM_CHALLENGE_ERROR"));
								return true;
							}
					}
				}
				return false;
			};
		}

		public void UpdateRedPoint()
		{
			m_RedPoint.enabled = !m_ItemData.HasReceived && m_ItemData.RequiredWins <= m_ItemData.WinCount;
		}
	}
}