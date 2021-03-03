using UnityEngine;
using System.Collections;
using Hotfix_LT.Data;
namespace Hotfix_LT.UI
{
	public class AllianceEscortUtil
	{

		static public void FormatResidueTransferDartNum(UILabel label)
		{
			int residueTransferNum = GetResidueTransferDartNum();
			if (residueTransferNum < 0)
			{
				EB.Debug.LogError("residueTransferNum < 0 num={0}" , residueTransferNum);
				residueTransferNum = 0;
			}

			LTUIUtil.SetText(label, LT.Hotfix.Utility.ColorUtility.FormatResidueStr(residueTransferNum, VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.EscortTimes)));
		}

		static public int GetResidueTransferDartNum()
		{
			int residue = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.EscortTimes) - AlliancesManager.Instance.DartData.HaveEscortNum;

			if (residue < 0)
			{
				//EB.Debug.LogError("transferDart ResidueTranferNum <0");
				return 0;
			}

			return residue;
		}

		static public int GetResidueTransferRefreshNum()
		{
			int residue = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.EscortRefreshTimes) - AlliancesManager.Instance.TransferDartInfo.HaveFreeRefreshNum;

			if (residue < 0)
			{
				//EB.Debug.LogError("transferDart ResidueRefreshNum <0");
				return 0;
			}

			return residue;
		}

		static public int GetResidueRobDartNum()
		{
			int totalNum = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.RobTimes);
			int residueNum = totalNum - AlliancesManager.Instance.DartData.HaveRobNum;
			if (residueNum < 0)
			{
				residueNum = 0;
				//EB.Debug.LogError("robDart residueNum < 0");
			}
			return residueNum;
		}

		static public void FormatResidueRobDartNum(UILabel label)
		{
			int totalNum = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.RobTimes);
			int residueNum = GetResidueRobDartNum();
			string residueStr = LT.Hotfix.Utility.ColorUtility.FormatResidueStr(residueNum, totalNum);
			LTUIUtil.SetText(label, residueStr);
		}

		static public string LocalizeDartName(string dartName)
		{
			if (GameStringValue.DartNameDic.ContainsKey(dartName))
				return EB.Localizer.GetString(GameStringValue.DartNameDic[dartName]);
			else
			{
				EB.Debug.LogError("dartName error for {0}" , dartName);
				return "";
			}
		}

		static public string GetDartQualityBGSpriteName(string dartName)
		{
			if (GameStringValue.DartNameDic.ContainsKey(dartName))
				return GameStringValue.DartQualityBGDic[dartName];
			else
			{
				EB.Debug.LogError("dartName error for {0}" , dartName);
				return "";
			}
		}

		static public string GetTransportCartModel(string dartName)
		{
			return string.Format("M100{0}-Variant", 5 - GameStringValue.DartIndexDic[dartName]);
		}

		static public void SetEscortResultHudCache(eDartResultType result)
		{
			DartResultController.sResultType = result;
			DartResultController.sOpenFlag = true;
		}

		static public void GotoTranferDart()
		{
			if (!IsMeetTransferCondition())
			{
				return;
			}

			//UIStack.Instance.ExitStack(false);该操作会引起主界面UI也消失
			Hotfix_LT.Data.SpecialActivityTemplate temp = Hotfix_LT.Data.EventTemplateManager.Instance.GetSpecialActivity(9005);
			switch (temp.nav_type)
			{
				case eActivityNavType.NpcFind:
					ActivityUtil.NpcFind(temp.nav_parameter, temp.id);
					break;
				case eActivityNavType.FunctionId:
					ActivityUtil.FunctionOpen(temp.nav_parameter, temp.id);
					break;
			}
		}

		static public void GotoRobDart()
		{
			if (!IsMeetRobCondition())
			{
				return;
			}

			//UIStack.Instance.ExitStack(false);
			Hotfix_LT.Data.SpecialActivityTemplate temp = Hotfix_LT.Data.EventTemplateManager.Instance.GetSpecialActivity(9005);
			switch (temp.nav_type)
			{
				case eActivityNavType.NpcFind:
					ActivityUtil.NpcFind(temp.nav_parameter, temp.id);
					break;
				case eActivityNavType.FunctionId:
					ActivityUtil.FunctionOpen(temp.nav_parameter, temp.id);
					break;
			}
		}

		public static bool IsMeetTransferCondition()
		{
			if (!Hotfix_LT.Data.EventTemplateManager.Instance.IsTimeOK("escort_start", "escort_stop"))
			{
				MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_AllianceEscortUtil_4329"));
				return false;
			}

			if (!AllianceUtil.IsJoinedAlliance)
			{
				MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_AllianceEscortUtil_4483"));
				return false;
			}

			if (GetResidueTransferDartNum() <= 0)
			{
				MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_AllianceEscortUtil_4639"));
				return false;
			}

			return true;
		}

		public static bool IsMeetRobCondition()
		{
			if (!AllianceUtil.IsJoinedAlliance)
			{
				MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_AllianceEscortUtil_5021"));
				return false;
			}

			if (GetResidueRobDartNum() <= 0)
			{
				MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_AllianceEscortUtil_5172"));
				return false;
			}

			return true;
		}

		public static string ToDartStateStr(eDartState dartState)
		{
			switch (dartState)
			{
				case eDartState.Transfer_Tian:
					return "tian";
				case eDartState.Transfer_Di:
					return "di";
				case eDartState.Transfer_Xuan:
					return "xuan";
				case eDartState.Transfer_Huang:
					return "huang";
			}
			return "tian";
		}

		public static bool GetIsInTransferDart(eDartState dartState)
		{
			if (dartState == eDartState.Transfer_Tian || dartState == eDartState.Transfer_Di || dartState == eDartState.Transfer_Xuan || dartState == eDartState.Transfer_Huang)
				return true;
			return false;
		}
	}
}