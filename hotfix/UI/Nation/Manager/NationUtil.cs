using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Hotfix_LT.UI
{
	public class NationUtil
	{
		public static long GetLocalUid()
		{
			return LoginManager.Instance.LocalUserId.Value;
		}

		public static bool GetIsKing(long uid)
		{
			return NationManager.Instance.Detail.KingUid == uid;
		}

		public static bool IsKing
		{
			get { return GetIsKing(GetLocalUid()); }
		}

		public static string LocalizeRankName(string rank)
		{
			switch (rank)
			{
				case "king": return EB.Localizer.GetString("ID_codefont_in_NationUtil_king");
				case "marshal": return EB.Localizer.GetString("ID_codefont_in_NationUtil_marshal");
				case "general": return EB.Localizer.GetString("ID_codefont_in_NationUtil_general");
				case "knight": return EB.Localizer.GetString("ID_codefont_in_NationUtil_knight");
				case "warrior": return EB.Localizer.GetString("ID_codefont_in_NationUtil_warrior");
				case "plebs": return EB.Localizer.GetString("ID_codefont_in_NationUtil_plebs");
				default: EB.Debug.LogError("LocalizeRankName err for rand:{0}" ,rank); return EB.Localizer.GetString("ID_codefont_in_NationUtil_plebs");
			}
		}

		public static string[] RankArr
		{
			get
			{
				return System.Enum.GetNames(typeof(eRanks));
			}
		}

		public static string[] NameArr
		{
			get
			{
				return System.Enum.GetNames(typeof(eNationName));
			}
		}

		public static string[] TeamNames = new string[] { "nation1", "nation2", "nation3" };

		public static string NationFlag(string nation)
		{
			switch (nation)
			{

				case "persian": return "Country_Flag_Green";
				case "roman": return "Country_Flag_Purple";
				case "egypt": return "Country_Flag_Blue";
				case "neutral": return "Country_Flag_Red";
				default: return "";
			}
		}

		public static string NationRoofFlag(string nation)
		{
			switch (nation)
			{
				case "persian": return "Country_Blue_Green_1";
				case "roman": return "Country_Gules_Roof_1";
				case "egypt": return "Country_Blue_Roof_1";
				case "neutral": return "Country_Violet_Roof_1";
				default: return "";
			}
		}

		public static string NationRoofFlagSmall(string nation)
		{
			switch (nation)
			{
				case "persian": return "Country_Blue_Green_2";
				case "roman": return "Country_Gules_Roof_2";
				case "egypt": return "Country_Blue_Roof_2";
				case "neutral": return "Country_Violet_Roof_2";
				default: return "";
			}
		}

		public static string NationGemFlag(string nation)
		{
			switch (nation)
			{
				case "persian": return "Country_Blue_Green_3";
				case "roman": return "Country_Gules_Roof_3";
				case "egypt": return "Country_Blue_Roof_3";
				case "neutral": return "Country_Violet_Roof_3";
				default: return "";
			}
		}

		public static string NationBuildIcon(string nation, string size)
		{
			switch (nation)
			{
				case "persian": return string.Format("Country_City_{0}_Green", size);
				case "roman": return string.Format("Country_City_{0}_Purple", size);
				case "egypt": return string.Format("Country_City_{0}_Blue", size);
				case "neutral": return string.Format("Country_City_{0}_Red", size);
				default: return "";
			}
		}

		public static string NationIcon(string nation)
		{
			switch (nation)
			{
				case "persian": return "Country_Icon_Bosi";
				case "roman": return "Country_Icon_Luoma";
				case "egypt": return "Country_Icon_Aiji";
				case "neutral": return "Country_Icon_Yelusaleng";
				default: return "";
			}
		}

		public static string NationBattleFlag(string nation)
		{
			switch (nation)
			{
				case "persian": return "Country_Battle_Flag_3";
				case "roman": return "Country_Battle_Flag_2";
				case "egypt": return "Country_Battle_Flag_1";
				case "neutral": return "Country_Battle_Flag_4";
				default: return "";
			}
		}

		public static string LocalizeNationName(string nation)
		{
			switch (nation)
			{
				case "persian": return EB.Localizer.GetString("ID_codefont_in_NationUtil_persian");
				case "roman": return EB.Localizer.GetString("ID_codefont_in_NationUtil_roman");
				case "egypt": return EB.Localizer.GetString("ID_codefont_in_NationUtil_egypt");
				case "neutral": return EB.Localizer.GetString("ID_codefont_in_NationUtil_neutral");
				default: return "";
			}
		}

		static public int[] GetTerritotyMap(int index)
		{
			switch (index)
			{
				case 0:
					return new int[] { 1, 2 };
				case 1:
					return new int[] { 0, 3, 4 };
				case 2:
					return new int[] { 0, 4, 5 };
				case 3:
					return new int[] { 1, 4, 6 };
				case 4:
					return new int[] { 1, 2, 3, 5, 7, 8 };
				case 5:
					return new int[] { 2, 4, 9 };
				case 6:
					return new int[] { 3, 7 };
				case 7:
					return new int[] { 4, 6, 8 };
				case 8:
					return new int[] { 4, 7, 9 };
				case 9:
					return new int[] { 5, 8 };
				default:
					return new int[] { };
			}
		}

		static public int GetPathIndex(eBattleDirection dir)
		{
			switch (dir)
			{
				case eBattleDirection.UpAttack:
				case eBattleDirection.UpDefend:
					return 0;
				case eBattleDirection.MiddleAttack:
				case eBattleDirection.MiddleDefend:
					return 1;
				case eBattleDirection.DownAttack:
				case eBattleDirection.DownDefend:
					return 2;
				default:
					return 0;
			}
		}

		static public bool GetIsAttack(eBattleDirection dir)
		{
			switch (dir)
			{
				case eBattleDirection.UpAttack:
				case eBattleDirection.MiddleAttack:
				case eBattleDirection.DownAttack:
					return true;
				default:
					return false;
			}
		}
	}
}
