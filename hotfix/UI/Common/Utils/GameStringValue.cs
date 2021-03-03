using System.Collections.Generic;

namespace Hotfix_LT.UI
{
	static public class GameStringValue
	{
		// public static readonly Dictionary<int, string> CharacterDic = new Dictionary<int, string>()
		// {
		// 	{ 1,"ID_1"},
		// 	{ 2,"ID_2"},
		// 	{ 3,"ID_3"},
		// 	{ 4,"ID_4"},
		// 	{ 5,"ID_5"},
		// 	{ 6,"ID_6"},
		// 	{ 7,"ID_7"},
		// 	{ 8,"ID_8"},
		// 	{ 9,"ID_9"},
		// 	{ 10,"ID_10"}
		// };

		public static readonly List<string> WeekDic = new List<string>() { "ID_LABEL_NAME_NONE", "ID_WEEK_1", "ID_WEEK_2", "ID_WEEK_3", "ID_WEEK_4", "ID_WEEK_5", "ID_WEEK_6", "ID_WEEK_0" };

		//public static readonly Dictionary<string,string> StoreTypeDic = new Dictionary<string, string>()
		//{
		//	{ "herobattle", "ID_STORE_COMMON_NAME" },//英雄交锋
		//	{ "mystery","ID_STORE_MYSTERY_NAME" },//神秘
		//	{ "arena", "ID_STORE_ARENA_NAME" },//角斗场
		//	{ "expedition","ID_STORE_EXPEDITION_NAME" },//国家
		//	{ "ladder","ID_STORE_LADDER_NAME" },//天梯
		//	{ "alliance","ID_STORE_ALLIANCE_NAME" },//军团
		//};

		//五族大战
		// public static readonly string[] FieldOrderNameList = new string[] { "shui", "huo", "jin", "mu", "tu", "eastsea" };

		public static readonly Dictionary<string, string> FieldNameDic = new Dictionary<string, string>()
		{
			{ "jin", "ID_ALLIANCE_JIN_FIELD" },
			{ "mu", "ID_ALLIANCE_MU_FIELD" },
			{ "shui", "ID_ALLIANCE_SHUI_FIELD" },
			{ "huo", "ID_ALLIANCE_HUO_FIELD"},
			{ "tu", "ID_ALLIANCE_TU_FIELD"},
			{ "eastsea", "ID_ALLIANCE_EASTSEA_FIELD" }
		};

		// public static readonly string[] HeroNameArr = new string[] { "Yuanhao", "Muniuma", "Linglong", "Lieyan", "Huanggang" };

		public static readonly Dictionary<string, int> DartIndexDic = new Dictionary<string, int>
		{
			{ "tian", 1 },
			{ "di", 2 },
			{ "xuan", 3 },
			{ "huang", 4 }
		};

		public static readonly Dictionary<string, string> DartNameDic = new Dictionary<string, string>
		{
			{ "tian", "ID_WUZI_DJ" },
			{ "di", "ID_WUZI_XY" },
			{ "xuan", "ID_WUZI_GJ" },
			{ "huang", "ID_WUZI_PT" }
		};

		public static readonly Dictionary<string, string> DartQualityBGDic = new Dictionary<string, string>
		{
			{ "tian", "Ty_Legion_Di_Orange"},
			{ "di", "Ty_Legion_Di_Purple" },
			{ "xuan", "Ty_Legion_Di_Blue" },
			{ "huang", "Ty_Legion_Di_Green" }
		};

		//天梯
		public static readonly string[] Ladder_Stage_Names = new string[] { "qingtong", "baiyin", "huangjin", "baijin", "zuanshi", "dashi", "zongshi", "wangzhe" };

		// public static readonly string DartSprite_Prefix = "Yunbiao_Icon_";

		//生命//攻击//魔法//防御//抗性//攻击穿透//魔法穿透//伤害减免//生命回复
		// public static readonly string[] SkillNameArr = new string[] { MaxHP, PATK, MATK, PDEF, MDEF, Penetration, Spell_penetration, Damage_reduction, Heal_recover };
		// public static readonly string MaxHP = "MaxHP";
		// public static readonly string PATK = "PATK";
		// public static readonly string MATK = "MATK";
		// public static readonly string PDEF = "PDEF";
		// public static readonly string MDEF = "MDEF";
		// public static readonly string Penetration = "penetration";
		// public static readonly string Spell_penetration = "spell_penetration";
		// public static readonly string Damage_reduction = "damage_reduction";
		// public static readonly string Heal_recover = "heal_recover";
		//生命：Partner_Icon_Shengming 攻击：Partner_Icon_Gongji	法术：Partner_Icon_Mofa	防御：Partner_Icon_Fangyu 抗性：Partner_Icon_Kangxing
		//攻击穿透：Skill_Icon_11002401 法术穿透：Skill_Icon_11004901	伤害减免：Skill_Icon_11005403 生命恢复：Skill_Icon_11000208
		// public static readonly string[] AllianceSkillIconArr = new string[]
		// {
		// "Partner_Icon_Shengming",
		// "Partner_Icon_Gongji",
		// "Partner_Icon_Mofa",
		// "Partner_Icon_Fangyu",
		// "Partner_Icon_Kangxing",
		// "Skill_Icon_11002401",
		// "Skill_Icon_11004901",
		// "Skill_Icon_11005403",
		// "Skill_Icon_11000208"
		// };

		// public static readonly string RANK_ICON_1 = "RankingList_Icon_1";
		// public static readonly string RANK_ICON_2 = "RankingList_Icon_2";
		// public static readonly string RANK_ICON_3 = "RankingList_Icon_3";
		// public static readonly string RANK_ICON_45 = "RankingList_Icon_45";

		// public static string GetRankIcon(int rank, bool isX = false)
		// {
		// 	if (rank <= 3)
		// 		return "RankingList_Icon_" + rank;
		// 	else
		// 		return !isX ? "RankingList_Icon_45" : "RankingList_Icon_46";
		// }
	}
}