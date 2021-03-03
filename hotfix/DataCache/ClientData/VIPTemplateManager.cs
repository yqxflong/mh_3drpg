using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.UI;

namespace Hotfix_LT.Data
{
	public class VIPPrivilege
	{
		private int vipType;
		public int VipType
		{
			set { vipType = value; }
			get { return vipType; }
		}
		private Dictionary<string, int> NumAddTypeDic;
		private Dictionary<string, float> PercentAddTypeDic;

		public VIPPrivilege()
		{
			NumAddTypeDic = new Dictionary<string, int>();
			PercentAddTypeDic = new Dictionary<string, float>();
		}

		public void AddNumTYpeDic(string key, int num)
		{
			NumAddTypeDic.Add(key, num);
		}

		public void AddPercentTypeDic(string key, float percent)
		{
			PercentAddTypeDic.Add(key, percent);
		}

		public int GetTotalNum(string key)
		{
			if (NumAddTypeDic.ContainsKey(key))
			{
				return NumAddTypeDic[key];
			}

			return 0;
		}

		public float GetPercent(string key)
		{
			if (PercentAddTypeDic.ContainsKey(key))
			{
				return PercentAddTypeDic[key];
			}

			return 0;
		}

		public Dictionary<string, int>.Enumerator GetNumEnumerator()
		{
			 return NumAddTypeDic.GetEnumerator();
		}

		public Dictionary<string, float>.Enumerator GetPercentEnumerator()
		{
			return PercentAddTypeDic.GetEnumerator();
		}

		public int Level;
		public int TotalHc;
		public string GiftItemId;
		public string EatEquipmentGift;
		public int GiftPrice;

		//public int BountyTaskTimes;     //通缉任务次数
		//public int BuyVigorTimes;     //购买体力次数
		//public int BuyGoldTimes;      //购买金币次数
		//public int ArenaTimes;        //竞技场次数
		//public int FreeRefiningTimes;       //精炼次数

		//public bool IsFreeRevive;           //是否开启免费复活
		//public bool IsMysteryShopShow;  //是否开启神秘商店刷新
		//public bool CanBlitzTenTimes;           //是否开启扫荡十次
		//public float ExpeditionRating; //蓬莱寻宝收益加成
		//public Dictionary<string, int> FuncUnlockTypeDic;
		//public Dictionary<int, int> VipReward;

		#region 弃用代码

		////public int PrivilegeCount
		////{
		////	get { return NumAddTypeDic.Count; }
		////}

		public IEnumerator GetEnumerator()
		{
			yield break;
			//if (Level <= 0)
			//{
			//	EB.Debug.LogError("GetIsNeedShow vipLevel<=0");
			//}
			//VIPPrivilege frontTemplate = VIPTemplateManager.Instance.GetTemplate(Level - 1);
			//VIPPrivilege template = VIPTemplateManager.Instance.GetTemplate(Level);
			//foreach (var d in NumAddTypeDic)
			//{
			//	if (VIPTemplateManager.Instance.GetIsNeedShow(frontTemplate, template, d.Key))
			//	{
			//		yield return d;
			//	}
			//}
			//foreach (var d in FuncUnlockTypeDic)
			//{
			//	if (VIPTemplateManager.Instance.GetIsNeedShow(frontTemplate, template, d.Key))
			//	{
			//		yield return d;
			//	}
			//}
			//foreach (var d in EarningAddTypeDic)
			//{
			//	if (VIPTemplateManager.Instance.GetIsNeedShow(frontTemplate, template, d.Key))
			//	{
			//		yield return d;
			//	}
			//}
		}

		//public bool GetIsOpen(string key)
		//{
		//	if (FuncUnlockTypeDic.ContainsKey(key))
		//	{
		//		if (FuncUnlockTypeDic[key] >= 0)
		//		{
		//			EB.Debug.LogError("FuncUnlockTypeDic[key]  value>=0");
		//		}
		//		return FuncUnlockTypeDic[key] > -10;
		//	}
		//	else
		//	{
		//		EB.Debug.LogError("vip FuncUnlockTypeDic not ContainsKey(key) key=" + key);
		//		return false;
		//	}
		//}

		#endregion
	}

	public class VIPPrivilegeKey
	{
		/// <summary> 购买体力次数 </summary>
		public const string BuyVigorTimes = "buy_vigor_times";

		/// <summary> 购买伙伴经验次数 </summary>
		public const string BuyExpTimes = "buy_exp_times";

		/// <summary> 购买金币次数 </summary>
		public const string BuyGoldTimes = "buy_gold_times";

		/// <summary> 幸运悬赏翻倍次数 </summary>
		public const string BountyDoubleTimes = "bounty_double_times";

		/// <summary> 军团护送翻倍次数 </summary>
		public const string EscortDoubleTimes = "escort_double_times";

		/// <summary> 角斗场，每日翻牌奖励翻倍次数 </summary>
		public const string ArenaDoubleTimes = "arena_double_times";

		/// <summary> 招财猫的免费次数 </summary>
		public const string LuckyCat = "lucky_cat";

		/// <summary>试炼石购买次数/// </summary>
		public const string ActicketTimes = "acticket_times";

		/// <summary> 竞技场次数 </summary>
		public const string ArenaTimes = "arena_times";

		/// <summary>补签次数</summary>
		public const string ResignInTimes = "resign_in_times";

		/// <summary>护送次数</summary>
		public const string EscortTimes = "escort_times";

		/// <summary>极限试炼次数</summary>
		public const string InfiniteChallengeTimes = "infinite_challenge_times";

		/// <summary>悬赏任务次数</summary>
		public const string BountyTaskTimes = "bounty_task_times";

		/// <summary>金币副本加成</summary>
		public const string TreasureRating = "treasure_rating";

		/// <summary>经验副本加成</summary>
		public const string ExpSpringRating = "exp_spring_rating";

		/// <summary>挑战副本复活次数</summary>
		public const string LostChallengeReviveTimes = "lost_challenge_revive_times";

		/// <summary>商店折扣</summary>
		public const string ShopDiscount = "shop_discount";

		/// <summary>强化返利</summary>
		public const string EatEquipmentGift = "eat_equipment_gift";

		/// <summary>劫镖次数</summary>
		public const string RobTimes = "rob_times";

		/// <summary>运镖免费刷新次数</summary>
		public const string EscortRefreshTimes = "escort_refresh_times";

		/// <summary>运镖求援次数</summary>
		public const string EscortHelpTimes = "escort_help_times";

		/// <summary>运镖支援次数</summary>
		public const string EscortReinforceTimes = "escort_reinforce_times";

		/// <summary>攻击红名次数</summary>
		public const string RedNameAttackTimes = "red_name_attack_times";

		/// <summary>红名洗白次数</summary>
		public const string RedNameRemovalTimes = "red_name_removal_times";

		/// <summary> 装备背包格子上限增加值 </summary>
		public const string InvEquipCount = "inv_equip_count";

		/// <summary> 挑战副本扫荡功能开启 </summary>
		public const string CanBlitzChallCampaign = "can_blitz_chall_campaign";

		/// <summary> 军团副本扫荡功能开启 </summary>
		public const string CanBlitzAlliCampaign = "can_blitz_alli_campaign";

		/// <summary> 未领取体力存储上限值 </summary>
		public const string BuyFreeVigorTimes = "buy_free_vigor_times";

		/// <summary> 极限试炼超值次数增加值 </summary>
		public const string InfiDiscountTimes = "infi_discount_times";

		/// <summary> 觉醒副本超值次数增加值 </summary>
		public const string AwakenCampDiscountTimes = "awaken_camp_discount_times";

		/// <summary> 荣耀币存储上限增加值 </summary>
		public const string HonorArenaIdleRewardLimit = "honor_arena_idle_reward_limit";

		/// <summary> 军团捐献每日上限 </summary>
		public const string AlliDonateTimes = "alli_donate_times";

		/// <summary> 军团捐献首次双倍功能开启 </summary>
		public const string FirstDonateDouble = "first_donate_double";

		/// <summary> 每日礼包限购次数增加值 </summary>
		public const string DayBuyLimit = "day_buy_limit";

		/// <summary> 每周礼包限购次数增加值 </summary>
		public const string WeeklyBuyLimit = "weekly_buy_limit";

		/// <summary> 每月礼包限购次数增加值 </summary>
		public const string MonthlyBuyLimit = "monthly_buy_limit";

		#region 已暂时弃用字段
		/// <summary>每日免费精炼次数</summary>
		public const string FreeRefiningTimes = "free_refining_times";

		// 劫运镖次数
		//public const string EscortAndRobTimes = "escort_and_rob_times";

		/// <summary>极限试炼重置次数</summary>
		public const string InfiniteChallengeResetTimes = "infinite_challenge_reset_times";

		//是否开启免费复活
		//public const string IsFreeRevive = "is_free_revive";   

		//是否开启神秘商店刷新
		// public const string IsMysteryShopShow = "is_mystery_shop_show";  

		//是否开启扫荡十次
		// public const string CanBlitzTenTimes = "can_blitz_ten_times";  

		//一键精炼
		// public const string IsCanQuickRefining = "is_can_quick_refining";

		//蓬莱寻宝收益加成
		// public const string ExpeditionRating = "expedition_rating";  
		
		//VIP奖励
		public const string DailyVipReward = "daily_hc_reward";

		// public const string GiftItemId = "gift_item_id";
		// public const string GiftPrice = "gift_price";
		#endregion
	}

	public class VIPTemplateManager
	{
		static private VIPTemplateManager sInstance;

		public static VIPTemplateManager Instance
		{
			get { return sInstance = sInstance ?? new VIPTemplateManager(); }
		}

		public List<VIPPrivilege> PrivilegeList;
		public List<VIPPrivilege> MonthVIPList;

		public static void ClearUp()
		{
			if (sInstance != null)
			{
				sInstance.PrivilegeList.Clear();
				sInstance.PrivilegeList.Clear();
			}
		}

        public bool InitTemplateFromCache(GM.DataCache.Vip vip)  
        {
            if (vip == null)
            {
                EB.Debug.LogError("can not find Vip Privilege data");
                return false;
            }

            PrivilegeList = new List<VIPPrivilege>();
            MonthVIPList = new List<VIPPrivilege>();
            var conditionSet = vip.GetArray(0);
            for (int i = 0; i < conditionSet.VipLength; ++i)
            {
                VIPPrivilege vipData = ParseTemplate(conditionSet.GetVip(i));
                if (vipData.VipType == 2)
                {
                    MonthVIPList.Add(vipData);
                }
                else
                {
                    PrivilegeList.Add(vipData);
                }
            }
            return true;
        }

        private VIPPrivilege ParseTemplate(GM.DataCache.VipInfo dict)
        {
            VIPPrivilege template = new VIPPrivilege();
            template.VipType = dict.Type;

			template.AddNumTYpeDic(VIPPrivilegeKey.BuyVigorTimes, dict.BuyVigorTimes);
			template.AddNumTYpeDic(VIPPrivilegeKey.BuyExpTimes, dict.BuyBuddyExpTimes);
			template.AddNumTYpeDic(VIPPrivilegeKey.BuyGoldTimes, dict.BuyGoldTimes);

			// 新字段
			template.AddNumTYpeDic(VIPPrivilegeKey.BountyDoubleTimes, dict.BountyDoubleTimes);
			template.AddNumTYpeDic(VIPPrivilegeKey.EscortDoubleTimes, dict.EscortDoubleTimes);
			template.AddNumTYpeDic(VIPPrivilegeKey.ArenaDoubleTimes, dict.ArenaDoubleTimes);

			template.AddNumTYpeDic(VIPPrivilegeKey.LuckyCat, dict.LuckyCat);

			template.AddNumTYpeDic(VIPPrivilegeKey.ActicketTimes, dict.ActicketTimes);
			template.AddNumTYpeDic(VIPPrivilegeKey.ArenaTimes, dict.ArenaTimes);
            template.AddNumTYpeDic(VIPPrivilegeKey.ResignInTimes, dict.ResignInTimes);
            template.AddNumTYpeDic(VIPPrivilegeKey.EscortTimes, dict.EscortTimes);
            template.AddNumTYpeDic(VIPPrivilegeKey.InfiniteChallengeTimes, dict.InfiniteChallengeTimes);
            template.AddNumTYpeDic(VIPPrivilegeKey.BountyTaskTimes, dict.BountyTaskTimes);

			template.AddPercentTypeDic(VIPPrivilegeKey.TreasureRating, dict.TreasureRating);
			template.AddPercentTypeDic(VIPPrivilegeKey.ExpSpringRating, dict.ExpSpringRating);
			template.AddPercentTypeDic(VIPPrivilegeKey.ShopDiscount, dict.ShopDiscount);

			template.AddNumTYpeDic(VIPPrivilegeKey.LostChallengeReviveTimes, dict.LostChallengeReviveTimes);			
			template.AddNumTYpeDic(VIPPrivilegeKey.RobTimes, dict.RobTimes);

			template.AddNumTYpeDic(VIPPrivilegeKey.EscortRefreshTimes, dict.EscortRefreshTimes);
			template.AddNumTYpeDic(VIPPrivilegeKey.EscortHelpTimes, dict.EscortHelpTimes);
			template.AddNumTYpeDic(VIPPrivilegeKey.EscortReinforceTimes, dict.EscortReinforceTimes);

			template.AddNumTYpeDic(VIPPrivilegeKey.RedNameRemovalTimes, dict.RedNameRemovalTimes);
            //template.AddNumTYpeDic(VIPPrivilegeKey.CanBlitzChallCampaign, dict.RedNameAttackTimes);

			// 新字段
			template.AddNumTYpeDic(VIPPrivilegeKey.InvEquipCount, dict.InvEquipCount);
			template.AddNumTYpeDic(VIPPrivilegeKey.CanBlitzChallCampaign, dict.CanBlitzChallCampaign ? 1 : 0);
			template.AddNumTYpeDic(VIPPrivilegeKey.CanBlitzAlliCampaign, dict.CanBlitzAlliCampaign ? 1 : 0);
			template.AddNumTYpeDic(VIPPrivilegeKey.BuyFreeVigorTimes, dict.BuyFreeVigorTimes);
			template.AddNumTYpeDic(VIPPrivilegeKey.InfiDiscountTimes, dict.InfiDiscountTimes);
			template.AddNumTYpeDic(VIPPrivilegeKey.AwakenCampDiscountTimes, dict.AwakenCampDiscountTimes);
			template.AddNumTYpeDic(VIPPrivilegeKey.HonorArenaIdleRewardLimit, dict.HonorArenaIdleRewardLimit);
			template.AddNumTYpeDic(VIPPrivilegeKey.AlliDonateTimes, dict.AlliDonateTimes);
			template.AddNumTYpeDic(VIPPrivilegeKey.FirstDonateDouble, dict.FirstDonateDouble ? 1 : 0);
			template.AddNumTYpeDic(VIPPrivilegeKey.DayBuyLimit, dict.DayBuyLimit);
			template.AddNumTYpeDic(VIPPrivilegeKey.WeeklyBuyLimit, dict.WeeklyBuyLimit);
			template.AddNumTYpeDic(VIPPrivilegeKey.MonthlyBuyLimit, dict.MonthlyBuyLimit);


			template.AddNumTYpeDic(VIPPrivilegeKey.FreeRefiningTimes, dict.FreeRefiningTimes);
            //template.AddNumTYpeDic(VIPPrivilegeKey.EscortAndRobTimes, dict.EscortAndRobTimes);
            template.AddNumTYpeDic(VIPPrivilegeKey.InfiniteChallengeResetTimes, dict.InfiniteChallengeResetTimes);
            //vip 奖励
            template.AddNumTYpeDic(VIPPrivilegeKey.DailyVipReward, dict.DailyHcReward);

			//template.FuncUnlockTypeDic.Add(VIPPrivilegeKey.IsFreeRevive, dict.IsFreeRevive ? -1 : -10);
			//template.FuncUnlockTypeDic.Add(VIPPrivilegeKey.IsMysteryShopShow, dict.IsMysteryShopShow ? -1 : -10);
			//template.FuncUnlockTypeDic.Add(VIPPrivilegeKey.CanBlitzTenTimes, dict.CanBlitzTenTimes ? -1 : -10);
			//template.FuncUnlockTypeDic.Add(VIPPrivilegeKey.IsCanQuickRefining, dict.IsCanQuickRefining ? -1 : -10);

			//template.EarningAddTypeDic.Add(VIPPrivilegeKey.ExpeditionRating, dict.ExpeditionRating);

			template.GiftItemId = dict.GiftItemId; /*dict.EatEquipmentGift;*/
			template.GiftPrice = dict.GiftPrice;
            template.Level = dict.Level;
            template.TotalHc = /*dict.Type*/ dict.TotalHc;
			template.EatEquipmentGift = dict.EatEquipmentGift;

            return template;
        }

        public VIPPrivilege GetTemplate(int vipLevel)
		{
			if (vipLevel >= PrivilegeList.Count)
			{
				Debug.LogWarningFormat("vipLevel >= PrivilegeList.Count vipLevel={0}, Vip Count={1}", vipLevel, PrivilegeList.Count-1);
				return null;
			}

			if (vipLevel < 0 || vipLevel >= PrivilegeList.Count) return null;

			return PrivilegeList[vipLevel];
		}

		/// <summary>
		/// 得到Key对应的当前vip等级次数上限
		/// </summary>
		/// <param name="key"></param>
		/// <returns>返回对应Key当前vip等级次数上限</returns>
		public int GetTotalNum(string key)
		{
			int v = BalanceResourceUtil.VipLevel;
			return GetTotalNum(v, key) + GetMonthVIPNum(key);
		}

		public float GetVIPPercent(string key,bool shopDisCount = true)
		{
			//不读月卡
			// if (MonthVIPList.Count > type)
			// {
			// 	percent = MonthVIPList[type].GetPercent(key);
			// }
			
			int v = BalanceResourceUtil.VipLevel;
			float percent = GetPercent(v, key);

			if (shopDisCount && percent <= 0)
			{
				return 1;
			}

			return percent;
		}

		/// <summary>
		/// 得到Key对应的当前vip等级次数上限
		/// </summary>
		/// <param name="vipLevel">vip等级</param>
		/// <param name="key">key</param>
		/// <returns></returns>
		private int GetTotalNum(int vipLevel, string key)
		{
			var template = GetTemplate(vipLevel);
			if (template != null)
			{
				return template.GetTotalNum(key);
			}
			else
			{
				return 0;
			}
		}
		
		private float GetPercent(int vipLevel, string key)
		{
			var template = GetTemplate(vipLevel);
			if (template != null)
			{
				return template.GetPercent(key);
			}
			else
			{
				return 0;
			}
		}

		public int GetMonthVIPNum(string key)
		{
			int num = 0;
			if (MonthVIPList != null)
			{
                if (LTChargeManager.Instance.IsSilverVIP() && MonthVIPList.Count > 0)
                {
                    num += MonthVIPList[0].GetTotalNum(key);
                }

                if (LTChargeManager.Instance.IsGoldVIP() && MonthVIPList.Count > 1)
                {
                    num += MonthVIPList[1].GetTotalNum(key);
                }
            }

			return num;
		}

		public int GetVipLevelUpperLimit()
		{
			return PrivilegeList.Count - 1;
		}

		public int GetNeedChargeNum(int vipLevel)
		{
			if (vipLevel <= 0)
				return 0;

			var template = GetTemplate(vipLevel);
			if (template != null)
				return GetTemplate(vipLevel).TotalHc;
			else
				return 0;
		}

		public int GetLevelByCharge(int charge)
		{
			for(int i=0;i< PrivilegeList.Count;i++)
			{
				VIPPrivilege vip = PrivilegeList[i];
				if(vip.TotalHc > charge)
				{
					int index = i - 1;
					if(index >= 0)
					{
						return PrivilegeList[index].Level;
					}
				}
				else if(vip.TotalHc == charge)
				{
					return vip.Level;
				}
			}
			return 0;
		}

		#region 弃用代码

		public bool GetIsOpen(string key)
		{
			return false;
			//int v = BalanceResourceUtil.VipLevel;
			//return GetIsOpen(v, key);
		}


		//   public bool GetIsOpen(int vipLevel, string key)
		//{
		//       return false;
		// //      var template = GetTemplate(vipLevel);
		//	//if (template != null)
		//	//{
		//	//	return template.GetIsOpen(key);
		//	//}
		//	//else
		//	//{
		//	//	return false;
		//	//}
		//}

		public int GetOpenLevel(string key)
		{
			return 0;
			//int levelMaxValue = GetVipLevelUpperLimit();
			//for (int i = 0; i <= levelMaxValue; ++i)
			//{
			//	if (GetIsOpen(i, key))
			//	{
			//		return i;
			//	}
			//}
			//EB.Debug.LogError("GetVipOpenLevel error key="+key);
			//return levelMaxValue;
		}

		//public bool GetIsNeedShow(VIPPrivilege front,VIPPrivilege cur, string key)
		//{
		//	if (cur.NumAddTypeDic.ContainsKey(key))
		//	{
		//		int frontLevelNum = front.GetTotalNum(key);
		//		int currentLevelNum = cur.GetTotalNum(key);
		//		if (frontLevelNum > currentLevelNum)
		//		{
		//			EB.Debug.LogError("frontLevelNum > currentLevelNum key=" + key);
		//		}
		//		return frontLevelNum != currentLevelNum;
		//	}
		//	//else if (cur.FuncUnlockTypeDic.ContainsKey(key))
		//	//{
		//	//	bool frontIsUnlock = front.GetIsOpen(key);
		//	//	bool isUnlock = cur.GetIsOpen(key);
		//	//	if (false == isUnlock && true == frontIsUnlock)
		//	//	{
		//	//		EB.Debug.LogError("vip isUnlock == false && frontIsUnlock==true");
		//	//		return false;
		//	//	}
		//	//	else if (true == isUnlock && false == frontIsUnlock)
		//	//	{
		//	//		return true;
		//	//	}
		//	//	return false;
		//	//}
		//	//else if (cur.EarningAddTypeDic.ContainsKey(key))
		//	//{
		//	//	int frontLevelNum = front.EarningAddTypeDic[key];
		//	//	int currentLevelNum = cur.EarningAddTypeDic[key];
		//	//	if (frontLevelNum > currentLevelNum)
		//	//	{
		//	//		EB.Debug.LogError("frontLevelNum > currentLevelNum key=" + key);
		//	//	}
		//	//	return frontLevelNum != currentLevelNum;
		//	//}
		//	else
		//	{
		//		EB.Debug.LogError("vip Privilege Key error key="+key);
		//		return false;
		//	}
		//}

		//public void SetVipResidueNum(string key,int usedNum,UILabel label)
		//{
		//	int totalNum = GetTotalNum(key);
		//	int residueNum = totalNum - usedNum;
		//	if (residueNum < 0)
		//	{
		//		EB.Debug.LogError("residueNum < 0 Vipkey="+key);
		//		residueNum = 0;
		//	}
		//	label.text = string.Format("{0}/{1}", residueNum, totalNum);
		//}

		public static int GetUsedNum(string key)
		{
			return 0;
			//switch (key)
			//{
			//	case VIPPrivilegeKey.BountyTaskTimes:
			//		return SearchUsedNumByID("userTaskStatus.bounty_times");
			//	case VIPPrivilegeKey.FreeRefiningTimes:
			//		return SearchUsedNumByID("limit_state.refining.current");
			//	case VIPPrivilegeKey.BuyGoldTimes:
			//		return SearchUsedNumByID("limit_state.buy_limit.gold.current");			
			//	default:
			//		EB.Debug.LogError("GetUsedNum key error key="+key);
			//		return 0;
			//}
		}

		public int GetResidueNum(string key)
		{
			return 0;
			//switch (key)
			//{
			//	case VIPPrivilegeKey.BuyGoldTimes:
			//		int totalNum = GetTotalNum(VIPPrivilegeKey.BuyGoldTimes);
			//		int usedNum = 0;
			//		if (!DataLookupsCache.Instance.SearchIntByID("limit_state.buy_limit.gold.current", out usedNum, null))
			//		{
			//			EB.Debug.LogError("searchdata buygold num fail");
			//			usedNum = 0;
			//		}
			//		int residueNum = totalNum - usedNum;
			//		if (residueNum < 0)
			//		{
			//			EB.Debug.LogError("goldBuy residueNum < 0");
			//			return 0;
			//		}
			//		return residueNum;
			//	default:
			//		EB.Debug.LogError("GetResidueNum key error key="+key);
			//		return 0;
			//}
		}

		//private static int SearchUsedNumByID(string strId)
		//{
		//	double usedNumFloat;
		//	int usedNum;
		//	if (!DataLookupsCache.Instance.SearchDataByID<double>(strId, out usedNumFloat, null))
		//	{
		//		EB.Debug.LogError("SearchUsedNumByID<double> fail id="+ strId);
		//		usedNum = 0;
		//	}
		//	usedNum = (int)usedNumFloat;
		//	return usedNum;
		//}

		#endregion
	}
}