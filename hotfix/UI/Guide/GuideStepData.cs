//GuideStepData
//新手引导Step数据
//Johny


using System.Collections;
using EB;

namespace Hotfix_LT.UI
{
	public class GuideStepData
	{

		public int guide_id;
		public int rollback_id;
		public int next_id;
		public int fore_id;
		public int type;
		public string trigger_type;
		public string excute_type;
		public int level;
		public int hall_level;
		public string campaign_id;
		public string task_id;
		public string items;
		public string townid;
		public string tips;
		public string view;
		public string target_path;
		public string parameter;
		public int tips_anchor;//1 2 3...8;

		public GuideStepData()
		{
			guide_id = 0;
			rollback_id = 0;
			next_id = 0;
			fore_id = 0;
			type = 0;
			trigger_type = "";
			excute_type = "";
			level = 0;
			hall_level = 0;
			campaign_id = "";
			task_id = "";
			items = "";
			townid = "";
			tips = "";
			tips_anchor = 0;//1 2 3...8;
			view = "";
			target_path = "";
			parameter = "";
		}

        public GuideStepData(GM.DataCache.GuideInfo data)
        {
            guide_id = data.GuideId;

            rollback_id = data.RollbackId;
            next_id = data.NextId;
            fore_id = data.ForeId;
            type = data.Type;
            trigger_type = data.TriggerType;
            excute_type = data.ExcuteType;
            level = data.Level;
            hall_level = data.HallLevel;
            campaign_id = data.Campaignid;
            task_id = data.TaskId;
            items = data.Items;
            townid = data.TownId;
            tips = data.Tips;
            tips_anchor = data.TipsAnchor;//1 2 3...8;
            view = data.View;
            target_path = data.TargetPath;
            parameter = data.Parameter;
        }

        public bool EnoughCondition()
		{
			if (level >= 0 && !EnoughLevel()) return false;
			if (!string.IsNullOrEmpty(task_id) && !EnoughTask()) return false;
			if (!string.IsNullOrEmpty(items) && !EnoughItems()) return false;
			return true;
		}

		public bool EnoughLevel()
		{
			int herolevel;
			if (!DataLookupsCache.Instance.SearchIntByID("level", out herolevel))
			{
				return false;
			}
			if (type == 1)//ForceGuide need strength condition
			{
				if (herolevel == level)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				if (herolevel >= level)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public bool EnoughTask()
		{
			string[] splits = task_id.Split('=');
			if (splits.Length < 2) return false;


			string task_state;
			if (DataLookupsCache.Instance.SearchDataByID<string>("tasks." + splits[0] + ".state", out task_state))
			{
				if (splits[1].Equals("0"))
				{
					if (task_state.Equals("running"))
					{
						return true;
					}
				}
				else
				{
					if (task_state.Equals("finished"))
					{
						return true;
					}
				}
			}
			return false;
		}

		public int GetTask()
		{
			int task = 0;
			string[] splits = task_id.Split('=');
			if (splits.Length < 2) task = 0;
			else
			{
				int.TryParse(splits[0], out task);
			}
			return task;
		}

		public int GetActivity()
		{
			return hall_level;
		}

		public enum eItemsActionType
		{
			None = 0,
			HaveEconomy = 1,
			HaveBuddy = 2,
			HaveBuddyShard = 3,
			EquipLoad = 4,
			BuddyWork = 5,
		}

		public bool EnoughItems()
		{
			if (string.IsNullOrEmpty(items)) return true;
			object ob = JSON.Parse(items);
			if (ob == null || !(ob is ArrayList))
			{
				EB.Debug.LogError("Condition is illegal {0}" , items);
				return true;
			}
			ArrayList array = ob as ArrayList;
			if (array.Count == 0) return true;
			bool flag = true;
			for (int i = 0; i < array.Count; i++)
			{
				eItemsActionType type = Hotfix_LT.EBCore.Dot.Enum<eItemsActionType>("type", array[i], eItemsActionType.None);
				ArrayList itemsArray = Hotfix_LT.EBCore.Dot.Array("items", array[i], null);
				switch (type)
				{
					case eItemsActionType.HaveEconomy:
						flag = flag && EnoughHaveEconomys(itemsArray);
						break;
					case eItemsActionType.HaveBuddy:
						flag = flag && EnoughHaveBuddys(itemsArray);
						break;
					case eItemsActionType.HaveBuddyShard:
						flag = flag && EnoughHaveBuddyShards(itemsArray);
						break;
					case eItemsActionType.EquipLoad:
						flag = flag && EnoughEquipsLoad(itemsArray);
						break;
					case eItemsActionType.BuddyWork:
						flag = flag && EnoughBuddysWork(itemsArray);
						break;
					default:
						flag = flag && true;
						break;
				}
				flag = flag && true;
			}
			return flag;
		}

		public bool EnoughHaveEconomys(ArrayList items)
		{
			bool flag = true;
			for (int i = 0; i < items.Count; i++)
			{
				string id = EB.Dot.String("id", items[i], "");
				int num = EB.Dot.Integer("num", items[i], 1);
				flag = flag && EnoughHaveEconomy(id, num);
			}
			return flag;
		}

		public bool EnoughHaveEconomy(string id, int num = 1)
		{
			if (string.IsNullOrEmpty(id)) return true;
			IDictionary economys;
			if (!DataLookupsCache.Instance.SearchDataByID<IDictionary>("inventory", out economys)) return false;
			else
			{
				foreach (DictionaryEntry de in economys)
				{
					string template_id = EB.Dot.String("economy_id", de.Value, "");
					int number = EB.Dot.Integer("num", de.Value, 0);
					if (id.Equals(template_id) && number == num) return true;
				}
				return false;
			}
		}

		public bool EnoughHaveBuddys(ArrayList items)
		{
			bool flag = true;
			for (int i = 0; i < items.Count; i++)
			{
				string id = EB.Dot.String("id", items[i], "");
				int num = EB.Dot.Integer("num", items[i], 1);
				flag = flag && EnoughHaveBuddy(id, num);
			}
			return flag;
		}

		public bool EnoughHaveBuddy(string id, int num = 1)
		{
			if (string.IsNullOrEmpty(id)) return true;
			IDictionary buddys;
			if (!DataLookupsCache.Instance.SearchDataByID<IDictionary>("heroStats", out buddys)) return false;
			else
			{
				foreach (DictionaryEntry de in buddys)
				{
					string character_id = EB.Dot.String("character_id", de.Value, "");
					int star = EB.Dot.Integer("star", de.Value, 0);
					if (star > 0 && id.Equals(character_id)) return true;
				}
				return false;
			}
		}

		public bool EnoughHaveBuddyShards(ArrayList items)
		{
			bool flag = true;
			for (int i = 0; i < items.Count; i++)
			{
				string id = EB.Dot.String("id", items[i], "");
				int num = EB.Dot.Integer("num", items[i], 1);
				flag = flag && EnoughHaveBuddyShard(id, num);
			}
			return flag;
		}

		public bool EnoughHaveBuddyShard(string id, int num = 1)
		{
			if (string.IsNullOrEmpty(id)) return true;
			if (id.CompareTo("1") == 0) id = GameItemUtil.GetProfessionHeroInfoId();
			IDictionary buddys;
			if (!DataLookupsCache.Instance.SearchDataByID<IDictionary>("heroStats", out buddys)) return false;
			else
			{
				foreach (DictionaryEntry de in buddys)
				{
					string character_id = EB.Dot.String("character_id", de.Value, "");
					int number = EB.Dot.Integer("shard", de.Value, 0);
					if (id.Equals(character_id) && number == num) return true;
				}
				return false;
			}
		}

		public bool EnoughEquipsLoad(ArrayList items)
		{
			bool flag = true;
			for (int i = 0; i < items.Count; i++)
			{
				string id = EB.Dot.String("id", items[i], "");
				flag = flag && EnoughEquipLoad(id);
			}
			return flag;
		}

		public bool EnoughEquipLoad(string id)
		{
			if (string.IsNullOrEmpty(id)) return true;
			IDictionary economys;
			if (!DataLookupsCache.Instance.SearchDataByID<IDictionary>("inventory", out economys)) return false;
			else
			{
				foreach (DictionaryEntry de in economys)
				{
					string template_id = EB.Dot.String("economy_id", de.Value, "");
					string location = EB.Dot.String("location", de.Value, "");
					if (id.Equals(template_id) && location.Equals("equipment")) return true;
				}
				return false;
			}
		}

		public bool EnoughBuddysWork(ArrayList items)
		{
			bool flag = true;
			for (int i = 0; i < items.Count; i++)
			{
				string id = EB.Dot.String("id", items[i], "");
				flag = flag && EnoughBuddyWork(id);
			}
			return flag;
		}

		public bool EnoughBuddyWork(string id)
		{
			string pos1 = "{buddyinventory.pos1.buddy.buddyid}.character_id";
			string pos2 = "{buddyinventory.pos2.buddy.buddyid}.character_id";
			string pos3 = "{buddyinventory.pos3.buddy.buddyid}.character_id";

			string pos1_id;
			string pos2_id;
			string pos3_id;

			DataLookupsCache.Instance.SearchDataByID<string>(pos1, out pos1_id);
			if (!string.IsNullOrEmpty(pos1_id) && pos1_id.Equals(id)) return true;
			DataLookupsCache.Instance.SearchDataByID<string>(pos2, out pos2_id);
			if (!string.IsNullOrEmpty(pos2_id) && pos2_id.Equals(id)) return true;
			DataLookupsCache.Instance.SearchDataByID<string>(pos3, out pos3_id);
			if (!string.IsNullOrEmpty(pos3_id) && pos3_id.Equals(id)) return true;
			return false;
		}
	}
}