using UnityEngine;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
	public class FormationUtil
	{
		private static string OnGetCurrentTeamName()
		{
			string strTeamId;
			DataLookupsCache.Instance.SearchDataByID<string>(SmallPartnerPacketRule.USER_TEAM + "." + SmallPartnerPacketRule.USER_TEAM_CUR_TEAM, out strTeamId);
			return strTeamId;
		}

		public static string GetCurrentTeamName(eBattleType battle_type = eBattleType.None)
		{
			if(battle_type==eBattleType.None)battle_type = BattleReadyHudController.sBattleType;
			if (battle_type == eBattleType.ArenaBattle)
			{
				return "arena";
			}
			else if (battle_type == eBattleType.ChallengeCampaign)
			{
				return "lt_challenge_camp";
			}
			else if (battle_type == eBattleType.NationBattle)
			{
				string teamName = "nation1";
				if (!DataLookupsCache.Instance.SearchDataByID<string>("combat.useTeam", out teamName))
				{
					Debug.LogError("Not Found NationBattle combat.useTeam ");
				}
				return teamName;
			}
			else if (battle_type == eBattleType.AllieanceFinalBattle || battle_type == eBattleType.AllieancePreBattle)
			{
				return "awar";
			}
			else if (battle_type == eBattleType.HeroBattle)
			{
				return "lt_coh";
			}
			else if (battle_type == eBattleType.LadderBattle)
			{
				return "ladder_battle";
			}
			else if (battle_type == eBattleType.AlienMazeBattle)
			{
				return "lt_aw_camp";
			}
			else if (battle_type == eBattleType.SleepTower)
			{
				return "lt_st";
			} else if (battle_type == eBattleType.LegionMercenary)
			{
				return "lt_mercenary";
			}
			else
			{
				return OnGetCurrentTeamName();
			}
		}

		public static void SetCurrentTeamName(string teamName)
		{
			DataLookupsCache.Instance.CacheData(SmallPartnerPacketRule.USER_TEAM + "." + SmallPartnerPacketRule.USER_TEAM_CUR_TEAM, teamName);
		}

		public static int GetCurrentTeamIndex()
		{
			string teamName = GetCurrentTeamName();
			return SmallPartnerPacketRule.USER_TEAM_LIST.IndexOf(teamName);
		}

		public static bool IsHave(LTPartnerData partnerData)
		{
			return partnerData.Star > 0;
		}

		// public static bool IsInTeam(int heroID)
		// {
		// 	List<TeamMemberData> teamList = LTFormationDataManager.Instance.GetCurrentTeamMemList();
		//
		// 	for (int i = 0; i < teamList.Count; i++)
		// 	{
		// 		if (teamList[i].HeroID == heroID)
		// 			return true;
		// 	}
		// 	return false;
		// }

		public static bool IsAlive(int heroId, bool isHire)
		{
			if (isHire)
			{
				heroId = -heroId;
			}

			float curHeroHp = 0;

			if (!DataLookupsCache.Instance.SearchDataByID<float>(string.Format("{0}.{1}.HP", LTChallengeInstanceHpCtrl.curHpInfoPath, heroId), out curHeroHp))
            {
                if (GetGlobalHPLoss() >= 100)
                {
                    return false;
                }

                return true;
			}

			if (curHeroHp <= 0)
			{
				return false;
			}
			return true;
		}

        public static int GetGlobalHPLoss()
        {
            int globalHPLoss = 0;
            DataLookupsCache.Instance.SearchDataByID<int>(LTChallengeInstanceHpCtrl.curGlobalHpLoss, out globalHPLoss);
            return globalHPLoss;
        }
        
        public static bool IsAllPartnerDied()
		{
			List<LTPartnerData> datas = LTPartnerDataManager.Instance.GetGeneralPartnerList();
			for (int i = 0; i < datas.Count; i++)
			{
				bool isAlive=IsAlive(datas[i].HeroId, datas[i].IsHire);
				if (isAlive)
				{
					return false;
				}
			}

			return true;
		}

		public static TeamMemberData NewTeamMemberData(LTPartnerData partnerData, int posIndex)
		{
			TeamMemberData teamMemData = new TeamMemberData();
			teamMemData.Uid = partnerData.uid;
			teamMemData.HeroID = partnerData.HeroId;
			teamMemData.InfoID = partnerData.InfoId;
			var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(partnerData.InfoId, partnerData.CurSkin);
			teamMemData.ModelName = charTpl.model_name;//需添加皮肤
			teamMemData.Pos = posIndex;
			teamMemData.IsHire = partnerData.IsHire;
			return teamMemData;
		}
		public static bool GetTeamOpen(string teamName)
		{
			int userLevel = BalanceResourceUtil.GetUserLevel();
			if (teamName == "team1")
			{
				return true;
			}
			else if (teamName == "team2")
			{
				return Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10061).IsConditionOK();
			}
			else if (teamName == "team3")
			{
				return Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10062).IsConditionOK();
			}
			else
			{ 
				return true; 
			}
		}
	}
}