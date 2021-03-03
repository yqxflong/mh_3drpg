using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Data
{
	public class BuildingTemplate
	{
		public int id;
		public string name;
		public string type;
		public int current_level;
		public int required_player_level;
		public int required_hall_level;
		public int food_cost;
		public int ore_cost;
		public string model_name;
		public int slot_id;
		public string desc;
		public string upgrade_desc;

		public static BuildingTemplate Search = new BuildingTemplate();
		public static BuildingTemplateComparer Comparer = new BuildingTemplateComparer();
	}

	public class BuildingTemplateComparer : IComparer<BuildingTemplate>
	{
		public int Compare(BuildingTemplate x, BuildingTemplate y)
		{
			return x.id - y.id;
		}
	}

	public class CityWallTemplate
	{
		public int id;
		public string name;
		public int level;
		public int damage;

		public static CityWallTemplate Search = new CityWallTemplate();
		public static CityWallTemplateComparer Comparer = new CityWallTemplateComparer();
	}

	public class CityWallTemplateComparer : IComparer<CityWallTemplate>
	{
		public int Compare(CityWallTemplate x, CityWallTemplate y)
		{
			return x.id - y.id;
		}
	}

	public class CityBlackSmithTemplate
	{
		public int id;
		public string name;
		public int level;
		public string reward_box_name;
		public int food_cost;
		public int ore_cost;
		public int cool_down;

		public static CityBlackSmithTemplate Search = new CityBlackSmithTemplate();
		public static CityBlackSmithTemplateComparer Comparer = new CityBlackSmithTemplateComparer();
	}

	public class CityBlackSmithTemplateComparer : IComparer<CityBlackSmithTemplate>
	{
		public int Compare(CityBlackSmithTemplate x, CityBlackSmithTemplate y)
		{
			return x.id - y.id;
		}
	}

	public class CityMarketTemplate
	{
		public int id;
		public string name;
		public int level;
		public int trade_limit;
		public int taxes;
		public int[] refreshTimes;

		public static CityMarketTemplate Search = new CityMarketTemplate();
		public static CityMarketTemplateComparer Comparer = new CityMarketTemplateComparer();
	}

	public class CityMarketTemplateComparer : IComparer<CityMarketTemplate>
	{
		public int Compare(CityMarketTemplate x, CityMarketTemplate y)
		{
			return x.id - y.id;
		}
	}

	public class CityTavernTemplate
	{
		public int id;
		public string name;
		public int level;

		public static CityTavernTemplate Search = new CityTavernTemplate();
		public static CityTavernTemplateComparer Comparer = new CityTavernTemplateComparer();
	}

	public class CityTavernTemplateComparer : IComparer<CityTavernTemplate>
	{
		public int Compare(CityTavernTemplate x, CityTavernTemplate y)
		{
			return x.id - y.id;
		}
	}

	public class CityWareHouseTemplate
	{
		public int id;
		public string name;
		public int level;
		public int food_storage_capacity;
		public int ore_storage_capacity;

		public static CityWareHouseTemplate Search = new CityWareHouseTemplate();
		public static CityWareHouseTemplateComparer Comparer = new CityWareHouseTemplateComparer();
	}

	public class CityWareHouseTemplateComparer : IComparer<CityWareHouseTemplate>
	{
		public int Compare(CityWareHouseTemplate x, CityWareHouseTemplate y)
		{
			return x.id - y.id;
		}
	}

	public class CityArenaTemplate
	{
		public int id;
		public string name;
		public int level;
		public int capacity;
		public int reward_exp;
		public int time;

		public static CityArenaTemplate Search = new CityArenaTemplate();
		public static CityArenaTemplateComparer Comparer = new CityArenaTemplateComparer();
	}

	public class CityArenaTemplateComparer : IComparer<CityArenaTemplate>
	{
		public int Compare(CityArenaTemplate x, CityArenaTemplate y)
		{
			return x.id - y.id;
		}
	}

	public class CityResidenceTemplate
	{
		public int id;
		public string name;
		public int level;
		public int gold_per_second;
		public int capacity;

		public static CityResidenceTemplate Search = new CityResidenceTemplate();
		public static CityResidenceTemplateComparer Comparer = new CityResidenceTemplateComparer();
	}

	public class CityResidenceTemplateComparer : IComparer<CityResidenceTemplate>
	{
		public int Compare(CityResidenceTemplate x, CityResidenceTemplate y)
		{
			return x.id - y.id;
		}
	}

	public class CityFarmTemplate
	{
		public int id;
		public string name;
		public int level;
		public int food_per_second;
		public int capacity;

		public static CityFarmTemplate Search = new CityFarmTemplate();
		public static CityFarmTemplateComparer Comparer = new CityFarmTemplateComparer();
	}

	public class CityFarmTemplateComparer : IComparer<CityFarmTemplate>
	{
		public int Compare(CityFarmTemplate x, CityFarmTemplate y)
		{
			return x.id - y.id;
		}
	}

	public class CityMineTemplate
	{
		public int id;
		public string name;
		public int level;
		public int ore_per_second;
		public int capacity;

		public static CityMineTemplate Search = new CityMineTemplate();
		public static CityMineTemplateComparer Comparer = new CityMineTemplateComparer();
	}

	public class CityMineTemplateComparer : IComparer<CityMineTemplate>
	{
		public int Compare(CityMineTemplate x, CityMineTemplate y)
		{
			return x.id - y.id;
		}
	}

	public class CityBankTemplate
	{
		public int id;
		public string name;
		public int level;
		public int rate;
		public int capacity;

		public static CityBankTemplate Search = new CityBankTemplate();
		public static CityBankTemplateComparer Comparer = new CityBankTemplateComparer();
	}

	public class CityBankTemplateComparer : IComparer<CityBankTemplate>
	{
		public int Compare(CityBankTemplate x, CityBankTemplate y)
		{
			return x.id - y.id;
		}
	}

	public class CityAgricultureLabTemplate
	{
		public int id;
		public string name;
		public int level;
		public int rate;

		public static CityAgricultureLabTemplate Search = new CityAgricultureLabTemplate();
		public static CityAgricultureLabTemplateComparer Comparer = new CityAgricultureLabTemplateComparer();
	}

	public class CityAgricultureLabTemplateComparer : IComparer<CityAgricultureLabTemplate>
	{
		public int Compare(CityAgricultureLabTemplate x, CityAgricultureLabTemplate y)
		{
			return x.id - y.id;
		}
	}

	public class CityLibraryTemplate
	{
		public int id;
		public string name;
		public int level;
		public int player_exp;

		public static CityLibraryTemplate Search = new CityLibraryTemplate();
		public static CityLibraryTemplateComparer Comparer = new CityLibraryTemplateComparer();
	}

	public class CityLibraryTemplateComparer : IComparer<CityLibraryTemplate>
	{
		public int Compare(CityLibraryTemplate x, CityLibraryTemplate y)
		{
			return x.id - y.id;
		}
	}

	public class CityPortTemplate
	{
		public int id;
		public string name;
		public int level;
		public int refresh_count;
		public int action_count;
		public int refresh_cost;

		public static CityPortTemplate Search = new CityPortTemplate();
		public static CityPortTemplateComparer Comparer = new CityPortTemplateComparer();
	}

	public class CityPortTemplateComparer : IComparer<CityPortTemplate>
	{
		public int Compare(CityPortTemplate x, CityPortTemplate y)
		{
			return x.id - y.id;
		}
	}

	public class BuildingTemplateManager
	{
		private static BuildingTemplateManager sInstance = null;

		private BuildingTemplate[] mBuildingTbl = null;
		private CityWallTemplate[] mWallTbl = null;
		private CityBlackSmithTemplate[] mBlackSmithTbl = null;
		private CityMarketTemplate[] mMarketTbl = null;
		private CityTavernTemplate[] mTavernTbl = null;
		private CityWareHouseTemplate[] mWareHouseTbl = null;
		private CityArenaTemplate[] mArenaTbl = null;
		private CityResidenceTemplate[] mResidenceTbl = null;
		private CityFarmTemplate[] mFarmTbl = null;
		private CityMineTemplate[] mMineTbl = null;
		private CityBankTemplate[] mBankTbl = null;
		private CityAgricultureLabTemplate[] mAricultureLabTbl = null;
		private CityLibraryTemplate[] mLibraryTbl = null;
		private CityPortTemplate[] mPortTbl = null;

		public static BuildingTemplateManager Instance
		{
			get { return sInstance = sInstance ?? new BuildingTemplateManager(); }
		}

		public bool InitFromDataCache(object tbls)
		{
			if (tbls == null)
			{
				EB.Debug.LogError("InitFromDataCache: tbls is null");
				return false;
			}

			ArrayList buildingTbl = Hotfix_LT.EBCore.Dot.Array("buildings", tbls, null);
			if (!InitBuildingTbl(buildingTbl))
			{
				EB.Debug.LogError("InitFromDataCache: init building table failed");
				return false;
			}

			ArrayList wallTbl = Hotfix_LT.EBCore.Dot.Array("wall", tbls, null);
			if (!InitWallTbl(wallTbl))
			{
				EB.Debug.LogError("InitFromDataCache: init wall table failed");
				return false;
			}

			ArrayList blacksmithTbl = Hotfix_LT.EBCore.Dot.Array("blacksmith", tbls, null);
			if (!InitBlackSmithTbl(blacksmithTbl))
			{
				EB.Debug.LogError("InitFromDataCache: init blacksmith table failed");
				return false;
			}

			ArrayList marketTbl = Hotfix_LT.EBCore.Dot.Array("market", tbls, null);
			if (!InitMarketTbl(marketTbl))
			{
				EB.Debug.LogError("InitFromDataCache: init market table failed");
				return false;
			}

			ArrayList tavernTbl = Hotfix_LT.EBCore.Dot.Array("tavern", tbls, null);
			if (!InitTavernTbl(tavernTbl))
			{
				EB.Debug.LogError("InitFromDataCache: init tavern table failed");
				return false;
			}

			ArrayList warehouseTbl = Hotfix_LT.EBCore.Dot.Array("warehouse", tbls, null);
			if (!InitWareHouseTbl(warehouseTbl))
			{
				EB.Debug.LogError("InitFromDataCache: init warehouse table failed");
				return false;
			}

			ArrayList arenaTbl = Hotfix_LT.EBCore.Dot.Array("arena", tbls, null);
			if (!InitArenaTbl(arenaTbl))
			{
				EB.Debug.LogError("InitFromDataCahce: init arena table failed");
				return false;
			}

			ArrayList residenceTbl = Hotfix_LT.EBCore.Dot.Array("residence", tbls, null);
			if (!InitResidenceTbl(residenceTbl))
			{
				EB.Debug.LogError("InitFromDataCache: init residence table failed");
				return false;
			}

			ArrayList farmTbl = Hotfix_LT.EBCore.Dot.Array("farm", tbls, null);
			if (!InitFarmTbl(farmTbl))
			{
				EB.Debug.LogError("InitFromDataCache: init farm table failed");
				return false;
			}

			ArrayList mineTbl = Hotfix_LT.EBCore.Dot.Array("mine", tbls, null);
			if (!InitMineTbl(mineTbl))
			{
				EB.Debug.LogError("InitFromDataCache: init mine table failed");
				return false;
			}

			ArrayList bankTbl = Hotfix_LT.EBCore.Dot.Array("bank", tbls, null);
			if (!InitBankTbl(bankTbl))
			{
				EB.Debug.LogError("InitFromDataCache: init bank table failed");
				return false;
			}

			ArrayList ariculturelabTbl = Hotfix_LT.EBCore.Dot.Array("agriculture_lab", tbls, null);
			if (!InitAgricultureLabTbl(ariculturelabTbl))
			{
				EB.Debug.LogError("InitFromDataCache: init agriculture lab table failed");
				return false;
			}

			ArrayList libraryTbl = Hotfix_LT.EBCore.Dot.Array("library", tbls, null);
			if (!InitLibraryTbl(libraryTbl))
			{
				EB.Debug.LogError("InitFromDataCache: init library table failed");
				return false;
			}

			ArrayList portTbl = Hotfix_LT.EBCore.Dot.Array("port", tbls, null);
			if (!InitPortTbl(portTbl))
			{
				EB.Debug.LogError("InitFromDataCache: init port table failed");
				return false;
			}

			return true;
		}

		private bool InitBuildingTbl(ArrayList tbl)
		{
			if (tbl == null)
			{
				EB.Debug.LogError("InitBuildingTbl: tbl is null");
				return false;
			}

			mBuildingTbl = new BuildingTemplate[tbl.Count];
			for (int i = 0; i < mBuildingTbl.Length; ++i)
			{
				mBuildingTbl[i] = ParseBuilding(tbl[i]);
			}
			System.Array.Sort(mBuildingTbl, BuildingTemplate.Comparer);
			return true;
		}

		private BuildingTemplate ParseBuilding(object obj)
		{
			BuildingTemplate tpl = new BuildingTemplate();
			tpl.id = EB.Dot.Integer("id", obj, tpl.id);
			tpl.name = EB.Dot.String("name", obj, tpl.name);
			tpl.type = EB.Dot.String("type", obj, tpl.type);
			tpl.current_level = EB.Dot.Integer("current_level", obj, tpl.current_level);
			tpl.required_player_level = EB.Dot.Integer("required_player_level", obj, tpl.required_player_level);
			tpl.required_hall_level = EB.Dot.Integer("required_hall_level", obj, tpl.required_hall_level);
			tpl.food_cost = EB.Dot.Integer("food_cost", obj, tpl.food_cost);
			tpl.ore_cost = EB.Dot.Integer("ore_cost", obj, tpl.ore_cost);
			tpl.model_name = EB.Dot.String("model_name", obj, tpl.model_name);
			tpl.slot_id = EB.Dot.Integer("slot_id", obj, tpl.slot_id);
			tpl.desc = EB.Dot.String("desc", obj, tpl.desc);
			tpl.upgrade_desc = EB.Dot.String("upgrade_desc", obj, tpl.upgrade_desc);
			return tpl;
		}

		private bool InitWallTbl(ArrayList tbl)
		{
			if (tbl == null)
			{
				EB.Debug.LogError("InitWallTabl: tbl is null");
				return false;
			}

			mWallTbl = new CityWallTemplate[tbl.Count];
			for (int i = 0; i < mWallTbl.Length; ++i)
			{
				mWallTbl[i] = ParseWall(tbl[i]);
			}
			System.Array.Sort(mWallTbl, CityWallTemplate.Comparer);
			return true;
		}

		private CityWallTemplate ParseWall(object obj)
		{
			CityWallTemplate tpl = new CityWallTemplate();
			tpl.id = EB.Dot.Integer("id", obj, tpl.id);
			tpl.name = EB.Dot.String("name", obj, tpl.name);
			tpl.level = EB.Dot.Integer("level", obj, tpl.level);
			tpl.damage = EB.Dot.Integer("damage", obj, tpl.damage);
			return tpl;
		}

		private bool InitBlackSmithTbl(ArrayList tbl)
		{
			if (tbl == null)
			{
				EB.Debug.LogError("InitBlackSmithTbl: tbl is null");
				return false;
			}

			mBlackSmithTbl = new CityBlackSmithTemplate[tbl.Count];
			for (int i = 0; i < mBlackSmithTbl.Length; ++i)
			{
				mBlackSmithTbl[i] = ParseBlackSmith(tbl[i]);
			}
			System.Array.Sort(mBlackSmithTbl, CityBlackSmithTemplate.Comparer);
			return true;
		}

		private CityBlackSmithTemplate ParseBlackSmith(object obj)
		{
			CityBlackSmithTemplate tpl = new CityBlackSmithTemplate();
			tpl.id = EB.Dot.Integer("id", obj, tpl.id);
			tpl.name = EB.Dot.String("name", obj, tpl.name);
			tpl.level = EB.Dot.Integer("level", obj, tpl.level);
			tpl.reward_box_name = EB.Dot.String("reward_box_name", obj, tpl.reward_box_name);
			tpl.food_cost = EB.Dot.Integer("food_cost", obj, tpl.food_cost);
			tpl.ore_cost = EB.Dot.Integer("ore_cost", obj, tpl.ore_cost);
			tpl.cool_down = EB.Dot.Integer("cool_down", obj, tpl.cool_down);
			return tpl;
		}

		private bool InitMarketTbl(ArrayList tbl)
		{
			if (tbl == null)
			{
				EB.Debug.LogError("InitMarketTbl: tbl is null");
				return false;
			}

			mMarketTbl = new CityMarketTemplate[tbl.Count];
			for (int i = 0; i < mMarketTbl.Length; ++i)
			{
				mMarketTbl[i] = ParseMarket(tbl[i]);
			}
			System.Array.Sort(mMarketTbl, CityMarketTemplate.Comparer);
			return true;
		}

		private CityMarketTemplate ParseMarket(object obj)
		{
			CityMarketTemplate tpl = new CityMarketTemplate();
			tpl.id = EB.Dot.Integer("id", obj, tpl.id);
			tpl.name = EB.Dot.String("name", obj, tpl.name);
			tpl.level = EB.Dot.Integer("level", obj, tpl.level);
			tpl.trade_limit = EB.Dot.Integer("trade_limit", obj, tpl.trade_limit);
			tpl.taxes = EB.Dot.Integer("taxes", obj, tpl.taxes);

			List<int> refreshTimes = new List<int>();
			for (int i = 1; ; ++i)
			{
				string key = string.Format("refresh_time_{0}", i);
				int val = EB.Dot.Integer(key, obj, -1);
				if (val < 0)
				{
					break;
				}

				refreshTimes.Add(val);
			}
			tpl.refreshTimes = refreshTimes.ToArray();
			return tpl;
		}

		private bool InitTavernTbl(ArrayList tbl)
		{
			if (tbl == null)
			{
				EB.Debug.LogError("InitTavernTbl: tbl is null");
				return false;
			}

			mTavernTbl = new CityTavernTemplate[tbl.Count];
			for (int i = 0; i < mTavernTbl.Length; ++i)
			{
				mTavernTbl[i] = ParseTavern(tbl[i]);
			}
			System.Array.Sort(mTavernTbl, CityTavernTemplate.Comparer);
			return true;
		}

		private CityTavernTemplate ParseTavern(object obj)
		{
			CityTavernTemplate tpl = new CityTavernTemplate();
			tpl.id = EB.Dot.Integer("id", obj, tpl.id);
			tpl.name = EB.Dot.String("name", obj, tpl.name);
			tpl.level = EB.Dot.Integer("level", obj, tpl.level);
			return tpl;
		}

		private bool InitWareHouseTbl(ArrayList tbl)
		{
			if (tbl == null)
			{
				EB.Debug.LogError("InitWareHouseTbl: tbl is null");
				return false;
			}

			mWareHouseTbl = new CityWareHouseTemplate[tbl.Count];
			for (int i = 0; i < mWareHouseTbl.Length; ++i)
			{
				mWareHouseTbl[i] = ParseWareHouse(tbl[i]);
			}
			System.Array.Sort(mWareHouseTbl, CityWareHouseTemplate.Comparer);
			return true;
		}

		private CityWareHouseTemplate ParseWareHouse(object obj)
		{
			CityWareHouseTemplate tpl = new CityWareHouseTemplate();
			tpl.id = EB.Dot.Integer("id", obj, tpl.id);
			tpl.name = EB.Dot.String("name", obj, tpl.name);
			tpl.level = EB.Dot.Integer("level", obj, tpl.level);
			tpl.food_storage_capacity = EB.Dot.Integer("food_storage_capacity", obj, tpl.food_storage_capacity);
			tpl.ore_storage_capacity = EB.Dot.Integer("ore_storage_capacity", obj, tpl.ore_storage_capacity);
			return tpl;
		}

		private bool InitArenaTbl(ArrayList tbl)
		{
			if (tbl == null)
			{
				EB.Debug.LogError("InitArenaTbl: tbl is null");
				return false;
			}

			mArenaTbl = new CityArenaTemplate[tbl.Count];
			for (int i = 0; i < mArenaTbl.Length; ++i)
			{
				mArenaTbl[i] = ParseArena(tbl[i]);
			}
			System.Array.Sort(mArenaTbl, CityArenaTemplate.Comparer);
			return true;
		}

		private CityArenaTemplate ParseArena(object obj)
		{
			CityArenaTemplate tpl = new CityArenaTemplate();
			tpl.id = EB.Dot.Integer("id", obj, tpl.id);
			tpl.name = EB.Dot.String("name", obj, tpl.name);
			tpl.level = EB.Dot.Integer("level", obj, tpl.level);
			tpl.capacity = EB.Dot.Integer("capacity", obj, tpl.capacity);
			tpl.reward_exp = EB.Dot.Integer("reward_exp", obj, tpl.reward_exp);
			tpl.time = EB.Dot.Integer("time", obj, tpl.time);
			return tpl;
		}

		private bool InitResidenceTbl(ArrayList tbl)
		{
			if (tbl == null)
			{
				EB.Debug.LogError("InitResidenceTbl: tbl is null");
				return false;
			}

			mResidenceTbl = new CityResidenceTemplate[tbl.Count];
			for (int i = 0; i < mResidenceTbl.Length; ++i)
			{
				mResidenceTbl[i] = ParseResidence(tbl[i]);
			}
			System.Array.Sort(mResidenceTbl, CityResidenceTemplate.Comparer);
			return true;
		}

		private CityResidenceTemplate ParseResidence(object obj)
		{
			CityResidenceTemplate tpl = new CityResidenceTemplate();
			tpl.id = EB.Dot.Integer("id", obj, tpl.id);
			tpl.name = EB.Dot.String("level", obj, tpl.name);
			tpl.level = EB.Dot.Integer("level", obj, tpl.level);
			tpl.gold_per_second = EB.Dot.Integer("gold_per_second", obj, tpl.gold_per_second);
			tpl.capacity = EB.Dot.Integer("capacity", obj, tpl.capacity);
			return tpl;
		}

		private bool InitFarmTbl(ArrayList tbl)
		{
			if (tbl == null)
			{
				EB.Debug.LogError("InitFarmTbl: tbl is null");
				return false;
			}
			mFarmTbl = new CityFarmTemplate[tbl.Count];
			for (int i = 0; i < mFarmTbl.Length; ++i)
			{
				mFarmTbl[i] = ParseFarm(tbl[i]);
			}
			System.Array.Sort(mFarmTbl, CityFarmTemplate.Comparer);
			return true;
		}

		private CityFarmTemplate ParseFarm(object obj)
		{
			CityFarmTemplate tpl = new CityFarmTemplate();
			tpl.id = EB.Dot.Integer("id", obj, tpl.id);
			tpl.name = EB.Dot.String("level", obj, tpl.name);
			tpl.level = EB.Dot.Integer("level", obj, tpl.level);
			tpl.food_per_second = EB.Dot.Integer("food_per_second", obj, tpl.food_per_second);
			tpl.capacity = EB.Dot.Integer("capacity", obj, tpl.capacity);
			return tpl;
		}

		private bool InitMineTbl(ArrayList tbl)
		{
			if (tbl == null)
			{
				EB.Debug.LogError("InitMineTbl: tbl is null");
				return false;
			}
			mMineTbl = new CityMineTemplate[tbl.Count];
			for (int i = 0; i < mMineTbl.Length; ++i)
			{
				mMineTbl[i] = ParseMine(tbl[i]);
			}
			System.Array.Sort(mMineTbl, CityMineTemplate.Comparer);
			return true;
		}

		private CityMineTemplate ParseMine(object obj)
		{
			CityMineTemplate tpl = new CityMineTemplate();
			tpl.id = EB.Dot.Integer("id", obj, tpl.id);
			tpl.name = EB.Dot.String("level", obj, tpl.name);
			tpl.level = EB.Dot.Integer("level", obj, tpl.level);
			tpl.ore_per_second = EB.Dot.Integer("ore_per_second", obj, tpl.ore_per_second);
			tpl.capacity = EB.Dot.Integer("capacity", obj, tpl.capacity);
			return tpl;
		}

		private bool InitBankTbl(ArrayList tbl)
		{
			if (tbl == null)
			{
				EB.Debug.LogError("InitBankTbl: tbl is null");
				return false;
			}

			mBankTbl = new CityBankTemplate[tbl.Count];
			for (int i = 0; i < mBankTbl.Length; ++i)
			{
				mBankTbl[i] = ParseBank(tbl[i]);
			}
			System.Array.Sort(mBankTbl, CityBankTemplate.Comparer);
			return true;
		}

		private CityBankTemplate ParseBank(object obj)
		{
			CityBankTemplate tpl = new CityBankTemplate();
			tpl.id = EB.Dot.Integer("id", obj, tpl.id);
			tpl.name = EB.Dot.String("name", obj, tpl.name);
			tpl.level = EB.Dot.Integer("level", obj, tpl.level);
			tpl.rate = EB.Dot.Integer("rate", obj, tpl.rate);
			tpl.capacity = EB.Dot.Integer("capacity", obj, tpl.capacity);
			return tpl;
		}

		private bool InitAgricultureLabTbl(ArrayList tbl)
		{
			if (tbl == null)
			{
				EB.Debug.LogError("InitAgricultureLabTabl: tbl is null");
				return false;
			}

			mAricultureLabTbl = new CityAgricultureLabTemplate[tbl.Count];
			for (int i = 0; i < mAricultureLabTbl.Length; ++i)
			{
				mAricultureLabTbl[i] = ParseAgricultureLab(tbl[i]);
			}
			System.Array.Sort(mAricultureLabTbl, CityAgricultureLabTemplate.Comparer);
			return true;
		}

		private CityAgricultureLabTemplate ParseAgricultureLab(object obj)
		{
			CityAgricultureLabTemplate tpl = new CityAgricultureLabTemplate();
			tpl.id = EB.Dot.Integer("id", obj, tpl.id);
			tpl.name = EB.Dot.String("name", obj, tpl.name);
			tpl.level = EB.Dot.Integer("level", obj, tpl.level);
			tpl.rate = EB.Dot.Integer("rate", obj, tpl.rate);
			return tpl;
		}

		private bool InitLibraryTbl(ArrayList tbl)
		{
			if (tbl == null)
			{
				EB.Debug.LogError("InitLibraryTbl: tbl is null");
				return false;
			}

			mLibraryTbl = new CityLibraryTemplate[tbl.Count];
			for (int i = 0; i < mLibraryTbl.Length; ++i)
			{
				mLibraryTbl[i] = ParseLibrary(tbl[i]);
			}
			System.Array.Sort(mLibraryTbl, CityLibraryTemplate.Comparer);
			return true;
		}

		private CityLibraryTemplate ParseLibrary(object obj)
		{
			CityLibraryTemplate tpl = new CityLibraryTemplate();
			tpl.id = EB.Dot.Integer("id", obj, tpl.id);
			tpl.name = EB.Dot.String("name", obj, tpl.name);
			tpl.level = EB.Dot.Integer("level", obj, tpl.level);
			tpl.player_exp = EB.Dot.Integer("player_exp", obj, tpl.player_exp);
			return tpl;
		}

		private bool InitPortTbl(ArrayList tbl)
		{
			if (tbl == null)
			{
				EB.Debug.LogError("InitPortTbl: tbl is null");
				return false;
			}

			mPortTbl = new CityPortTemplate[tbl.Count];
			for (int i = 0; i < mPortTbl.Length; ++i)
			{
				mPortTbl[i] = ParsePort(tbl[i]);
			}
			System.Array.Sort(mPortTbl, CityPortTemplate.Comparer);
			return true;
		}

		private CityPortTemplate ParsePort(object obj)
		{
			CityPortTemplate port = new CityPortTemplate();
			port.id = EB.Dot.Integer("id", obj, port.id);
			port.name = EB.Dot.String("name", obj, port.name);
			port.level = EB.Dot.Integer("level", obj, port.level);
			port.refresh_count = EB.Dot.Integer("refresh_count", obj, port.refresh_count);
			port.action_count = EB.Dot.Integer("action_count", obj, port.action_count);
			port.refresh_cost = EB.Dot.Integer("refresh_cost", obj, port.refresh_cost);
			return port;
		}

		public BuildingTemplate GetBuilding(int id)
		{
			BuildingTemplate.Search.id = id;
			int index = System.Array.BinarySearch(mBuildingTbl, BuildingTemplate.Search, BuildingTemplate.Comparer);
			if (index >= 0)
			{
				return mBuildingTbl[index];
			}
			else
			{
				EB.Debug.LogWarning("GetBuilding: building not found, id = {0}", id);
				return null;
			}
		}

		public BuildingTemplate[] GetBuildings()
		{
			return mBuildingTbl;
		}

		public CityWallTemplate GetWall(int id)
		{
			CityWallTemplate.Search.id = id;
			int index = System.Array.BinarySearch(mWallTbl, CityWallTemplate.Search, CityWallTemplate.Comparer);
			if (index >= 0)
			{
				return mWallTbl[index];
			}
			else
			{
				EB.Debug.LogWarning("GetWall: wall not found, id = {0}", id);
				return null;
			}
		}

		public CityBlackSmithTemplate GetBlackSmith(int id)
		{
			CityBlackSmithTemplate.Search.id = id;
			int index = System.Array.BinarySearch(mBlackSmithTbl, CityBlackSmithTemplate.Search, CityBlackSmithTemplate.Comparer);
			if (index >= 0)
			{
				return mBlackSmithTbl[index];
			}
			else
			{
				EB.Debug.LogWarning("GetBlackSmith: blacksmith not found, id = {0}", id);
				return null;
			}
		}

		public CityMarketTemplate GetMarket(int id)
		{
			CityMarketTemplate.Search.id = id;
			int index = System.Array.BinarySearch(mMarketTbl, CityMarketTemplate.Search, CityMarketTemplate.Comparer);
			if (index >= 0)
			{
				return mMarketTbl[index];
			}
			else
			{
				EB.Debug.LogWarning("GetMarket: market not found, id = {0}", id);
				return null;
			}
		}

		public CityTavernTemplate GetTavern(int id)
		{
			CityTavernTemplate.Search.id = id;
			int index = System.Array.BinarySearch(mTavernTbl, CityTavernTemplate.Search, CityTavernTemplate.Comparer);
			if (index >= 0)
			{
				return mTavernTbl[index];
			}
			else
			{
				EB.Debug.LogWarning("GetTavern: tavern not found, id = {0}", id);
				return null;
			}
		}

		public CityWareHouseTemplate GetWareHouse(int id)
		{
			CityWareHouseTemplate.Search.id = id;
			int index = System.Array.BinarySearch(mWareHouseTbl, CityWareHouseTemplate.Search, CityWareHouseTemplate.Comparer);
			if (index >= 0)
			{
				return mWareHouseTbl[index];
			}
			else
			{
				EB.Debug.LogWarning("GetWareHouse: warehouse not found, id = {0}", id);
				return null;
			}
		}

		public CityArenaTemplate GetArena(int id)
		{
			CityArenaTemplate.Search.id = id;
			int index = System.Array.BinarySearch(mArenaTbl, CityArenaTemplate.Search, CityArenaTemplate.Comparer);
			if (index >= 0)
			{
				return mArenaTbl[index];
			}
			else
			{
				EB.Debug.LogWarning("GetArena: arena not found, id = {0}", id);
				return null;
			}
		}

		public CityResidenceTemplate GetResidence(int id)
		{
			CityResidenceTemplate.Search.id = id;
			int index = System.Array.BinarySearch(mResidenceTbl, CityResidenceTemplate.Search, CityResidenceTemplate.Comparer);
			if (index >= 0)
			{
				return mResidenceTbl[index];
			}
			else
			{
				EB.Debug.LogWarning("GetResidence: redidence not found, id = {0}", id);
				return null;
			}
		}

		public CityFarmTemplate GetFarm(int id)
		{
			CityFarmTemplate.Search.id = id;
			int index = System.Array.BinarySearch(mFarmTbl, CityFarmTemplate.Search, CityFarmTemplate.Comparer);
			if (index >= 0)
			{
				return mFarmTbl[index];
			}
			else
			{
				EB.Debug.LogWarning("GetFarm: farm not found, id = {0}", id);
				return null;
			}
		}

		public CityMineTemplate GetMine(int id)
		{
			CityMineTemplate.Search.id = id;
			int index = System.Array.BinarySearch(mMineTbl, CityMineTemplate.Search, CityMineTemplate.Comparer);
			if (index >= 0)
			{
				return mMineTbl[index];
			}
			else
			{
				EB.Debug.LogWarning("GetMine: mine not found, id = {0}", id);
				return null;
			}
		}

		public CityBankTemplate GetBank(int id)
		{
			CityBankTemplate.Search.id = id;
			int index = System.Array.BinarySearch(mBankTbl, CityBankTemplate.Search, CityBankTemplate.Comparer);
			if (index >= 0)
			{
				return mBankTbl[index];
			}
			else
			{
				EB.Debug.LogWarning("GetBank: bank not found, id = {0}", id);
				return null;
			}
		}

		public CityAgricultureLabTemplate GetAgricultureLab(int id)
		{
			CityAgricultureLabTemplate.Search.id = id;
			int index = System.Array.BinarySearch(mAricultureLabTbl, CityAgricultureLabTemplate.Search, CityAgricultureLabTemplate.Comparer);
			if (index >= 0)
			{
				return mAricultureLabTbl[index];
			}
			else
			{
				EB.Debug.LogWarning("GetAgricultureLab: agriculture not found, id = {0}", id);
				return null;
			}
		}

		public CityLibraryTemplate GetLibrary(int id)
		{
			CityLibraryTemplate.Search.id = id;
			int index = System.Array.BinarySearch(mLibraryTbl, CityLibraryTemplate.Search, CityLibraryTemplate.Comparer);
			if (index >= 0)
			{
				return mLibraryTbl[index];
			}
			else
			{
				EB.Debug.LogWarning("GetLibrary: library not found, id = {0}", id);
				return null;
			}
		}

		public CityPortTemplate GetPort(int id)
		{
			CityPortTemplate.Search.id = id;
			int index = System.Array.BinarySearch(mPortTbl, CityPortTemplate.Search, CityPortTemplate.Comparer);
			if (index >= 0)
			{
				return mPortTbl[index];
			}
			else
			{
				EB.Debug.LogWarning("GetPort: port not found, id = {0}", id);
				return null;
			}
		}
	}
}