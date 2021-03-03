using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
	public class BalanceResourceUtil
	{
		public const string HcName = "hc";
		// public const string VigorName = "vigor";
		public const string GoldName = "gold";
		// public const string FoodName = "food";
		// public const string OreName = "ore";
		// public const string ArenaGoldName = "arena-gold";
		// public const string ExpeditionGoldName = "expedition-gold";
		// public const string AllianceGoldName = "alliance-gold";
		// public const string AllianceDonateName = "alliance-donate";
		// public const string LadderGoldName = "ladder-gold";
		// public const string TrainingRune = "training-rune";
		// public const string VipPoint = "vippoint";
		public const string AwakeTicket = "awake-ticket";
		// public const string Score = "score";

		public const int MaxNum = 999;

		public static int GetDataLookupValue(string path)
		{
			int value = 0;

			if (!DataLookupsCache.Instance.SearchIntByID(path, out value))
			{
				EB.Debug.LogError("{0} value get fail", path);
			}

			return value;
		}

		public static int GetUserGold()
		{
			int gold = 0;

			if (!DataLookupsCache.Instance.SearchIntByID("res.gold.v", out gold)) return 0;
			return gold;
		}

		public static int GetUserDiamond()
		{
			int diamond = 0;

			if (!DataLookupsCache.Instance.SearchIntByID("res.hc.v", out diamond)) return 0;
			return diamond;
		}

		public static int GetAwakenTicket()
		{
			int num = 0;
			DataLookupsCache.Instance.SearchIntByID("userAwakenCampaign.ticket", out num);
			return num;
		}

		public static int GetUserPoten()
		{
			int poten = 0;

			if (!DataLookupsCache.Instance.SearchIntByID("res.poten-gold.v", out poten)) return 0;
			return poten;
		}

		public static int GetUserLevel()
		{
			int level = 1;

			if (!DataLookupsCache.Instance.SearchIntByID("level", out level)) return 1;
			return level;
		}

		static public string GetUserName()
		{
			string playerName;
			if (!DataLookupsCache.Instance.SearchDataByID<string>("user.name", out playerName))
				playerName = string.Empty;
			return playerName;
		}

		public static int GetUserFood()
		{
			int food = 0;

			if (!DataLookupsCache.Instance.SearchIntByID("res.food.v", out food)) return 0;
			return food;
		}

		public static int GetUserOre()
		{
			int ore = 0;

			if (!DataLookupsCache.Instance.SearchIntByID("res.ore.v", out ore)) return 0;
			return ore;
		}

		public static int GetUserVigor()
		{
			int vigor = 0;

			if (!DataLookupsCache.Instance.SearchIntByID("res.vigor.v", out vigor)) return 0;
			return vigor;
		}
		
		public static bool EnterVigorCheck(int needVigor)
		{
			int curVigor=GetUserVigor();
			if (curVigor>=needVigor)
			{
				return false;
			}
			else
			{
				TurnToVigorGotView();
				return true;
			}
		}

		public static int GetUserVigorMax()
		{
			int vigor = 0;

			if (!DataLookupsCache.Instance.SearchIntByID("res.vigor.max", out vigor)) return 0;
			return vigor;
		}

		public static int GetUserAllianceGold()
		{
			int allianceGold = 0;

			if (!DataLookupsCache.Instance.SearchIntByID("res.alliance-gold.v", out allianceGold)) return 0;
			return allianceGold;
		}

		public static int GetUserAllianceDonate()
		{
			int allianceDonate = 0;

			if (!DataLookupsCache.Instance.SearchIntByID("res.alliance-donate.v", out allianceDonate)) return 0;
			return allianceDonate;
		}

		public static int GetUserLadderGold()
		{
			int gold = 0;

			if (!DataLookupsCache.Instance.SearchIntByID("res.ladder-gold.v", out gold)) return 0;
			return gold;
		}

		public static void CostLocalResource(ResourceContainer cost)
		{

		}

		public static string GetResSpriteName(string res_id)
		{
			switch (res_id)
			{
				case "exp":
					return "Ty_Icon_Jingyan";
				case "hc":
					return "Ty_Icon_Jewel";
				case "vigor":
					return "Ty_Icon_Tili";
				case "gold":
					return "Ty_Icon_Gold";
				case "food":
					return "Ty_Icon_Gold";
				case "ore":
					return "Ty_Icon_Gold";
				case "arena-gold":
					return "Ty_Icon_Jiaodou";//Ty_Icon_Jingjichang
				case "expedition-gold":
					return "Ty_Icon_Yuanzheng";
				case "alliance-gold":
					return "Ty_Icon_Juntuan";// "Ty_Icon_Bangpai";
				case "alliance-donate":
					return "Ty_Icon_Bangpaigongxian";
				case "hero-gold":
					return "Ty_Icon_Yingxiong";
				case "nation-gold":
					return "Ty_Icon_Guojia";
				case "ladder-gold":
					return "Ty_Icon_Tianti";
				case "mana":
					return "Copy_Icon_Moli";
				case "heroshard":
					return "Ty_Icon_Suipian";
				case "poten-gold":
					return "Ty_Icon_Qianneng";
				case "awake-ticket":
					return "Ty_Icon_Shilianshi";
				case "score":
					return "Ty_Icon_Xunzhang";
				case "honorarena-gold":
					return "Ty_Icon_Guojia";
				case "bosschall-6200":
					return "Ty_Icon_BOSSbi_1_1";
				case "bosschall-6201":
					return "Ty_Icon_BOSSbi_1_2";
				case "bosschall-6202":
					return "Ty_Icon_BOSSbi_2_2";
				case "bosschall-6203":
					return "Ty_Icon_BOSSbi_2_1";
				case "bosschall-6204":
					return "Ty_Icon_BOSSbi_3_1";
				case "bosschall-6205":
					return "Ty_Icon_BOSSbi_3_2";
				//case "vipPoint":
					//return "Goods_Prop_18";
				default:
					return "Ty_Icon_Gold";
			}
		}

		public static string GetResIcon(string res_id, int count = -1)
		{
			int id = GetResID(res_id);
			Hotfix_LT.Data.EconemyItemTemplate itemTpl = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(id);
			return itemTpl.IconId;
		}

		public static string GetResName(string res_id)
		{
			int id = GetResID(res_id);
			Hotfix_LT.Data.EconemyItemTemplate itemTpl = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(id);
			return itemTpl.Name;
		}

		public static int GetResID(string res_id)
		{
			switch (res_id)
			{
				case "hc":
					return 1;
				case "gold":
					return 2;
				case "vigor":
					return 3;
				case "exp":
				case "xp":
					return 4;
				case "buddy-exp":
					return 5;
				case "alliance-gold":
					return 6;
				case "nation-gold":
					return 7;
				case "ladder-gold":
					return 8;
				case "arena-gold":
					return 9;
				case "hero-gold":
					return 10;
				case "chall-camp-point":
					return 11;
				case "poten-gold":
					return 12;
				case "awake-ticket":
					return 13;
				case "score":
					return 14;
				case "handbook-point":
					return 15;
				case "honorarena-gold":
					return 16;		
				case "alliance-donate":
					return 17;
				case "vipPoint":
					return 18;
                case "m-control":
                    return 19;
                case "m-normal":
                    return 20;
                case "bosschall-6200":
					return 6200;
				case "bosschall-6201":
					return 6201;
				case "bosschall-6202":
					return 6202;
				case "bosschall-6203":
					return 6203;
				case "bosschall-6204":
					return 6204;
				case "bosschall-6205":
					return 6205;
				default:
					return 1;
			}
		}

		public static string GetResStrID(int id)
		{
			switch (id)
			{
				case 1:
					return "hc";
				case 2:
					return "gold";
				case 3:
					return "vigor";
				case 4:
					return "exp";
				case 5:
					return "buddy-exp";
				case 6:
					return "alliance-gold";
				case 7:
					return "nation-gold";
				case 8:
					return "ladder-gold";
				case 9:
					return "arena-gold";
				case 10:
					return "hero-gold";
				case 11:
					return "chall-camp-point";
				case 12:
					return "poten-gold";
				case 13:
					return "awake-ticket";
				case 14:
					return "score";
				case 15:
					return "handbook-point";
				case 16:
					return "honorarena-gold";
				case 17:
					return "alliance-donate";
				case 18:
					return "vipPoint";
                case 19:
                    return "m-control";
                case 20:
                    return "m-normal";
                case 6200:
					return "bosschall-6200";
				case 6201:
					return "bosschall-6201";
				case 6202:
					return "bosschall-6202";
				case 6203:
					return "bosschall-6203";
				case 6204:
					return "bosschall-6204";
				case 6205:
					return "bosschall-6205";
				default:
					return "gold";
			}
		}

		public static int GetResValue(string res_id)
		{
			int res = 0;
			if (res_id == "mana")
			{
				if (!DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.majordata.sp", out res))
				{
					return 0;
				}
			}
			else if (res_id == "score")
			{
				if (!DataLookupsCache.Instance.SearchIntByID("userHeroMedal.score", out res))
				{
					return 0;
				}
			}
			else if (res_id == "handbook-point")
			{
				int curLevel = LTPartnerHandbookManager.Instance.GetHandBookLevel();
				var curHandBookInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMannualScoreTemplateById(curLevel);
				int curCount = LTPartnerHandbookManager.Instance.GetHandBookSpoint() - (curHandBookInfo != null ? curHandBookInfo.totleScore : 0);
				return curCount;
			}
			else if (res_id == AwakeTicket)
			{
				int num = GetAwakenTicket();
				return num;
			}
			else if (res_id.Equals("vipPoint"))
			{
				return ChargeHc;
			}
            else if(res_id.Equals("m-control"))
            {
                return LTInstanceMapModel.Instance.GetMonopolySpecialDiceCount();
            }
            else if (res_id.Equals("m-normal"))
            {
                return LTInstanceMapModel.Instance.GetMonopolyGeneralDiceCount();
            }
            else if (!DataLookupsCache.Instance.SearchIntByID("res." + res_id + ".v", out res))
			{
				return 0;
            }


			return res;
		}

		public static int VipLevel
		{
			get
			{
				int v;
				if (!DataLookupsCache.Instance.SearchIntByID("vip", out v, null))
				{
                    return 0;
				}
				return v;
			}
		}

		public static int ChargeHc
		{
			get
			{
				int v;
				if (!DataLookupsCache.Instance.SearchIntByID("user.revenue", out v, null))//"vip_hc"
				{
					return 0;
				}
				return v;
			}
		}

		public static int VipHc
		{
			get
			{
				int v;
				if (!DataLookupsCache.Instance.SearchIntByID("vip_hc", out v, null))
				{
					return 0;
				}
				return v;
			}
		}

		public static int ArenaRank
		{
			get
			{
				int v;
				if (!DataLookupsCache.Instance.SearchIntByID("arena.info.rank", out v, null))
				{
					return 0;
				}
				return v + 1;
			}
		}

		public static int GetChatLevel()
		{
			int level = 0;
			if (!DataLookupsCache.Instance.SearchIntByID("chat_level", out level)) level = 10;
			return level;
		}

		public static void HcLessMessage(System.Action callBack = null)
		{
			MessageTemplateManager.ShowMessage(901030, null, delegate (int result)
			{
				if (result == 0)
                {
                    InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                    if (callBack != null)
					{
						callBack();
					}
					GlobalMenuManager.Instance.Open("LTChargeStoreHud", null);
				}
			});
		}

        public static void GoldLessMessage(System.Action callBack = null)
        {
            MessageTemplateManager.ShowMessage(901031, null, delegate (int result)
            {
                if (result == 0)
                {
                    InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                    if (callBack != null)
                    {
                        callBack();
                    }
                    GlobalMenuManager.Instance.Open("LTResourceShopUI");
                }
            });
        }

        public static void ResLessMessage(string resId)
		{
			string name = GetResName(resId);
			if (resId == "hc")
			{
				HcLessMessage();
			}
			else
			{
				MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_BALANCE_RESOURCE_UTIL_LESS"), name));
			}
		}

		public static string NumFormat(string data)
		{
			if (!string.IsNullOrEmpty(data))
			{
				int num = int.Parse(data);

				if (num > 1000000000)
				{
					string str = string.Format("{0}.{1}G", num / 1000000000, ((num % 1000000000) / 100000000));
					return str;
				}
				else if (num > 1000000)
				{
					string str = string.Format("{0}.{1}M", num / 1000000, ((num % 1000000) / 100000));
					return str;
				}
				else if (num >= 1000)
				{
					string str = string.Format("{0}.{1}K", num / 1000, ((num % 1000) / 100));
					return str;
				}
				else
				{
					return num.ToString();
				}
			}

			return data;
		}
		public static void TurnToVigorGotView()
		{
			List<RecoverVigorItemData> recoverVigorItemsData;
			string dataStr = Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("RecoverVigorItems");
			string[] dataArray = dataStr.Split(';');
			recoverVigorItemsData = new List<RecoverVigorItemData>();
			for (int i = 0; i < dataArray.Length; i++)
			{
				var items = dataArray[i].Split(',');
				if (items.Length >= 2 && int.TryParse(items[1], out int num))
				{
					int count = GameItemUtil.GetInventoryItemNum(items[0]);
					recoverVigorItemsData.Add(new RecoverVigorItemData(items[0], num, count));
				}
			}
			recoverVigorItemsData.Sort((x, y) => { return y.num - x.num; });
			if (recoverVigorItemsData.Count > 0 && recoverVigorItemsData[0] != null && recoverVigorItemsData[0].num > 0)
			{
				GlobalMenuManager.Instance.Open("RecoverVigorView", recoverVigorItemsData);
				return;
			}
			MessageTemplateManager.ShowMessage(902085, null, delegate (int result)
			{
				if (result == 0)
				{
					InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
					GlobalMenuManager.Instance.Open("LTResourceShopUI");
				}

			});

		}
	}
}

	/// <summary>
	/// res item hero hero_shard
	/// </summary>
	public class ItemStruct
	{
		public string id;
		public string type;
		public int quantity;

		public ItemStruct(string id, string type, int quantity)
		{
			this.id = id;
			this.type = type;
			this.quantity = quantity;
		}
	}

	public class ResourceContainer
	{
		public int Gold { get; set; }
		public int HC { get; set; }
		public int Exp { get; set; }
		public int Vigor { get; set; }
		public int Arena_gold { get; set; }
		public int Expedition_gold { get; set; }
		public int Alliance_gold { get; set; }
		public int Alliance_donate { get; set; }
		public int Ladder_gold { get; set; }
		public int Food { get; set; }
		public int Ore { get; set; }
		public int Poten_gold { get; set; }

		public Dictionary<int, int> Items { get; private set; }

		public ResourceContainer()
		{
			Items = new Dictionary<int, int>();
		}

		public static ResourceContainer Parse(object value)
		{
			if (value == null)
			{
				return null;
			}

			ArrayList al = null;
			if (value is ArrayList)
			{
				al = value as ArrayList;
			}
			else if (al is IDictionary)
			{
				al = Johny.ArrayListPool.Claim();
				al.Add(al);
			}
			else
			{
				return null;
			}

			ResourceContainer ac = new ResourceContainer();
			for (var i = 0; i < al.Count; i++)
			{
				object obj = al[i];
				string type = EB.Dot.String("type", obj, string.Empty);
				if (string.IsNullOrEmpty(type))
				{
					continue;
				}

				if (type == "res")
				{
					string data = EB.Dot.String("data", obj, string.Empty);
					int quantity = EB.Dot.Integer("quantity", obj, 0);
					switch (data)
					{
						case "hc":
							ac.HC = quantity;
							break;
						case "gold":
							ac.Gold = quantity;
							break;
						case "exp":
							ac.Exp = quantity;
							break;
						case "vigor":
							ac.Vigor = quantity;
							break;
						case "food":
							ac.Food = quantity;
							break;
						case "ore":
							ac.Ore = quantity;
							break;
						case "arena-gold":
							ac.Arena_gold = quantity;
							break;
						case "expedition-gold":
							ac.Expedition_gold = quantity;
							break;
						case "alliance-gold":
							ac.Alliance_gold = quantity;
							break;
						case "alliance-donate":
							ac.Alliance_donate = quantity;
							break;
						case "ladder-gold":
							ac.Ladder_gold = quantity;
							break;
						case "poten-gold":
							ac.Poten_gold = quantity;
							break;
					}
				}
				else if (type == "gaminventory")  //item
				{
					int data = EB.Dot.Integer("data", obj, 0);
					int quantity = EB.Dot.Integer("quantity", obj, 0);
					if (data > 0 && quantity > 0)
					{
						ac.Items[data] = quantity;
					}
				}
			}

			return ac;
		}
	}
