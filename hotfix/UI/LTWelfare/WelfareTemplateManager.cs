using System.Collections.Generic;

namespace Hotfix_LT.UI
{
	public class SigninAward
	{
		//public int    Month { get; set; }
		//public int	  SignCount { get; set; }
		public string Type { get; set; }
		public string Id { get; set; }
		public int Count { get; set; }
	}

	public enum eReceiveState
	{
		can,
		cannot,
		have,
	}

	public enum eEverydayReceiveState
	{
		first = 0,
		second,
		can,
		cannot,
		have,
	}

	public class EverydayAward
	{
		public int Id { get; set; }
		public string Type { get; set; }
		public string Title { get; set; }
		public string Desc { get; set; }
		public LTShowItemData AwardItem { get; set; }
		public eEverydayReceiveState ReceiveState;
	}

	public class FirstChargeAward
	{
		public string Type { get; set; }
		public string Title { get; set; }
		public string Desc { get; set; }
		public List<LTShowItemData> AwardItemList { get; set; }
	}

	public class WelfareTemplateManager
	{
		public static WelfareTemplateManager s_instance;
		public static WelfareTemplateManager Instance
		{
			get { return s_instance = s_instance ?? new WelfareTemplateManager(); }
		}

		private WelfareTemplateManager()
		{

		}

		private Dictionary<int, List<SigninAward>> SigninDic = new Dictionary<int, List<SigninAward>>();
		public List<EverydayAward> EverydayAwardList = new List<EverydayAward>();
		public FirstChargeAward FirstChargeAwardData = new FirstChargeAward();
		public FirstChargeAward FirstChargeAward1Data = new FirstChargeAward();
		public FirstChargeAward FirstChargeAward2Data = new FirstChargeAward();
		public FirstChargeAward FirstChargeAward3Data = new FirstChargeAward();


		public List<SigninAward> GetTemplate(int month)
		{
			return SigninDic[month];
		}

		public SigninAward GetTemplate(int month, int day)
		{
			if (day >= SigninDic[month].Count)
				EB.Debug.LogError("Index >= m_SigninAwardList.Count Index={0}", SigninDic[month].Count);
			return SigninDic[month][day];
		}

		//ToDo: 临时增加，等待Hotfix_LT.Data.ClientDataUtil类迁移完成后再移除
		public static bool InitTemplateFromCacheILR(GM.DataCache.Event evt)
		{
			return Instance.InitTemplateFromCache(evt);
		}

		public bool InitTemplateFromCache(GM.DataCache.Event evt)
		{
			if (evt == null)
			{
				EB.Debug.LogError("can not find sign_in_reward data");
				return false;
			}

			SigninDic.Clear();
			var conditionSet = evt.GetArray(0);
			for (int i = 0; i < conditionSet.SignInRewardLength; ++i)
			{
				var d = conditionSet.GetSignInReward(i);
				int month = d.Month;
				int signCount = d.SignCount;
				if (!SigninDic.ContainsKey(month))
				{
					SigninDic.Add(month, new List<SigninAward>());
					if (signCount != 1)
					{
						EB.Debug.LogError("InitTemplateFromCache: signCount != 1");
					}
					SigninDic[month].Add(ParseSigninTemplate(d));
				}
				else
				{
					if (signCount != SigninDic[month].Count + 1)
					{
						EB.Debug.LogError("InitTemplateFromCache: signCount!= SigninDic[month].Count+1");
					}
					SigninDic[month].Add(ParseSigninTemplate(d));
				}
			}

			EverydayAwardList.Clear();
			for (int i = 0; i < conditionSet.DailyRewardLength; ++i)
			{
				var data = conditionSet.GetDailyReward(i);
				string type = data.Type;
				if (type == "first_charge")
				{
					FirstChargeAwardData = ParseFirstChargeTemplate(data);
				}
				else if (type == "first_charge_1")
				{
					FirstChargeAward1Data = ParseFirstChargeTemplate(data);
				}
				else if (type == "first_charge_2")
				{
					FirstChargeAward2Data = ParseFirstChargeTemplate(data);
				}
				else if (type == "first_charge_3")
				{
					FirstChargeAward3Data = ParseFirstChargeTemplate(data);
				}
				else if (type != "share_reward")
				{
					EverydayAwardList.Add(ParseEverydayTemplate(data));
				}
			}

			return true;
		}

		private SigninAward ParseSigninTemplate(GM.DataCache.SignInReward dict)
		{
			SigninAward award = new SigninAward();

			award.Type = dict.ItemType;
			award.Id = dict.ItemId;
			award.Count = dict.ItemNum;

			return award;
		}

		private EverydayAward ParseEverydayTemplate(GM.DataCache.DailyReward dict)
		{
			EverydayAward award = new EverydayAward();
			award.Id = dict.Id;
			award.Type = dict.Type;
			award.Title = dict.Title;
			award.Desc = dict.Desc;
			string type = dict.ItemType;
			string id = dict.ItemId;
			int count = dict.ItemNum;
			award.AwardItem = new LTShowItemData(id, count, type);

			return award;
		}

		private FirstChargeAward ParseFirstChargeTemplate(GM.DataCache.DailyReward dict)
		{
			FirstChargeAward award = new FirstChargeAward();
			award.Type = dict.Type;
			award.Title = dict.Title;
			award.Desc = dict.Desc;
			award.AwardItemList = new List<LTShowItemData>();
			string type = dict.ItemType;
			string id = dict.ItemId;
			int count = dict.ItemNum;
			if (!string.IsNullOrEmpty(id))
				award.AwardItemList.Add(new LTShowItemData(id, count, type));
			type = dict.ItemType2;
			id = dict.ItemId2;
			count = dict.ItemNum2;
			if (!string.IsNullOrEmpty(id))
				award.AwardItemList.Add(new LTShowItemData(id, count, type));
			type = dict.ItemType3;
			id = dict.ItemId3;
			count = dict.ItemNum3;
			if (!string.IsNullOrEmpty(id))
				award.AwardItemList.Add(new LTShowItemData(id, count, type));
			type = dict.ItemType4;
			id = dict.ItemId4;
			count = dict.ItemNum4;
			if (!string.IsNullOrEmpty(id))
				award.AwardItemList.Add(new LTShowItemData(id, count, type));
			return award;
		}

		public EverydayAward GetEverydayAwardById(int id)
		{
			for (int i = 0; i < EverydayAwardList.Count; i++)
			{
				if (EverydayAwardList[i].Id == id)
				{
					return EverydayAwardList[i];
				}
			}

			return null;
		}

		public EverydayAward GetEverydayAwardByType(string eType)
		{
			for (int i = 0; i < EverydayAwardList.Count; i++)
			{
				if (EverydayAwardList[i].Type.CompareTo(eType) == 0)
				{
					return EverydayAwardList[i];
				}
			}

			return null;
		}
	}
}