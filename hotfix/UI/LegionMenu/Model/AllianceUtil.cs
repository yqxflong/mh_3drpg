namespace Hotfix_LT.UI
{
    public static class AllianceUtil
    {
        public static int[] ParseIntergerArray(string name, object obj, int[] defaultValue)
        {
            return Hotfix_LT.EBCore.Dot.Array(name, obj, defaultValue, delegate (object value)
            {
                return int.Parse(value.ToString());
            });
        }

        public static bool IsOwner(long uid)
        {
            return AlliancesManager.Instance.Detail.OwnerUid == uid;
        }

        public static bool IsExtraOwner(long uid)
        {
            AllianceMember member = AlliancesManager.Instance.DetailMembers.Find(uid);
            if (member == null)
            {
                return false;
            }

            return member.Role == eAllianceMemberRole.ExtraOwner;
        }

        public static bool IsOnlyAdmin(long uid)
        {
            AllianceMember member = AlliancesManager.Instance.DetailMembers.Find(uid);
            if (member == null)
            {
                return false;
            }

            return member.Role == eAllianceMemberRole.Admin;  //member.Role == eAllianceMemberRole.Owner ||
        }

        public static bool IsOneOfAdmin(long uid)
        {
            AllianceMember member = AlliancesManager.Instance.DetailMembers.Find(uid);
            if (member == null)
            {
                return false;
            }

            return member.Role == eAllianceMemberRole.Admin || member.Role == eAllianceMemberRole.ExtraOwner || member.Role == eAllianceMemberRole.Owner;
        }

        public static bool IsMember(long uid)
        {
            AllianceMember member = AlliancesManager.Instance.DetailMembers.Find(uid);
            if (member == null)
            {
                return false;
            }

            return member.Role == eAllianceMemberRole.Member;
        }

        public static int GetExtraOwnerNum()
        {
            int num = 0;
            for (var i = 0; i < AlliancesManager.Instance.DetailMembers.Members.Count; i++)
            {
                var m = AlliancesManager.Instance.DetailMembers.Members[i];
                if (m.Role == eAllianceMemberRole.ExtraOwner)
                    num++;
            }
            return num;
        }

        public static bool GetIsInAlliance(long uid)
        {
            AllianceMember member = AlliancesManager.Instance.DetailMembers.Find(uid);
            return member != null;
        }

        public static bool IsJoinedAlliance
        {
            get
            {
                return AlliancesManager.Instance.Account.AllianceId > 0 && AlliancesManager.Instance.Account.State == eAllianceState.Joined;
            }
        }

        public static bool IsInTransferDart
        {
            get
            {
                return (AlliancesManager.Instance.DartData.State == eAllianceDartCurrentState.Transfer) || (AlliancesManager.Instance.DartData.State == eAllianceDartCurrentState.Transfering);
            }
        }

        public static bool GetIsInTransferDart(string localizerId)
        {
            if (IsInTransferDart)
            {
                string text = "";
                //if (!string.IsNullOrEmpty(localizerId))
                //{
                //	text = string.Format(EB.Localizer.GetString("ID_codefont_in_AllianceUtil_2484") ,EB.Localizer.GetString(localizerId));
                //}
                //else
                //{
                text = EB.Localizer.GetString("ID_codefont_in_AllianceUtil_2604");
                //}

                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, text);
                return true;
            }
            return false;
        }

        public static bool IsLocalPlayer(long uid)
        {
            return LoginManager.Instance.LocalUserId.Value == uid;
        }

        public static AllianceMember GetLocalPlayer()
        {
            long uid = GetLocalUid();
            AllianceMember member = AlliancesManager.Instance.DetailMembers.Find(uid);
            return member;
        }

        public static bool GetIsAdmin()
        {
            long uid = GetLocalUid();
            return IsExtraOwner(uid) || IsOwner(uid);
        }

        public static long GetLocalUid()
        {
            return LoginManager.Instance.LocalUserId.Value;
        }

        public static double Now()
        {
            return EB.Time.Now;
            //System.DateTime baseTime = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            //System.DateTime utcNow = System.DateTime.UtcNow;
            //return (utcNow - baseTime).TotalSeconds;
        }

        public static string ToTFS(this long value)
        {
            return EB.Localizer.FormatNumber(value);
        }

        public static string ToTFS(this int value)
        {
            return EB.Localizer.FormatNumber(value);
        }

        public static string GetRaceSpriteName(eAllianceUserRace race)
        {
            switch (race)
            {
                case eAllianceUserRace.Huo:
                    return "Ty_Huo";
                case eAllianceUserRace.Jin:
                    return "Ty_Jin";
                case eAllianceUserRace.Shui:
                    return "Ty_Shui";
                case eAllianceUserRace.Mu:
                    return "Ty_Mu";
                case eAllianceUserRace.Tu:
                    return "Ty_Tu";
            }

            return string.Empty;
        }

        public static string GetBgSpriteName(int dataIndex)
        {
            return dataIndex % 2 == 0 ? "Gang_Di_9" : "Gang_Di_10";
        }

        public static string GetRoleName(eAllianceMemberRole role)
        {
            switch (role)
            {
                case eAllianceMemberRole.Member:
                    return EB.Localizer.GetString("ID_ALLIANCE_ROLE_MEMBER_NAME");
                case eAllianceMemberRole.Admin:
                    return EB.Localizer.GetString("ID_ALLIANCE_ROLE_ADMIN_NAME");
                case eAllianceMemberRole.ExtraOwner:
                    return EB.Localizer.GetString("ID_ALLIANCE_ROLE_EXTRA_OWNER_NAME");
                case eAllianceMemberRole.Owner:
                    return EB.Localizer.GetString("ID_ALLIANCE_ROLE_OWNER_NAME");
            }

            return string.Empty;
        }

        public static int GetMaxLevel()
        {
            return AlliancesManager.Instance.Config.Levels.Length - 1;
        }

        public static int GetMaxTechLevel()
        {
            return AlliancesManager.Instance.Config.Technologys.Length - 1;
        }

        public static int GetMaxSkillLevel()
        {
            int technology_level = AlliancesManager.Instance.Detail.TechnologyLevel;
            int maxSkillLevel = AlliancesManager.Instance.Config.Technologys[technology_level].MaxSkillLevel;
            return maxSkillLevel;
        }

        public static int GetSkillLevel(string skillName)
        {
            int level = 0;
            if (AlliancesManager.Instance.Skills.SkillDic.ContainsKey(skillName))
            {
                level = AlliancesManager.Instance.Skills.SkillDic[skillName];
            }
            else
            {
                EB.Debug.LogWarning("alliance skillName error skillname={0}" , skillName);
            }

            int maxSkillLevel = GetMaxSkillLevel();
            if (level > maxSkillLevel)
            {
                EB.Debug.LogError("level > MaxSkillLevel skillName={0}" , skillName);
                level = maxSkillLevel;
            }
            return level;
        }

        public static int GetMaxMemberCount(int level)
        {
            if (level >= AlliancesManager.Instance.Config.Levels.Length)
            {
                level = AlliancesManager.Instance.Config.Levels.Length - 1;
            }

            return AlliancesManager.Instance.Config.Levels[level].MaxMemberCount;
        }

        public static int GetTodayDonateGoldLimit(int level)
        {
            if (level >= AlliancesManager.Instance.Config.Levels.Length)
            {
                level = AlliancesManager.Instance.Config.Levels.Length - 1;
            }

            return AlliancesManager.Instance.Config.Levels[level].TodayDonateGoldLimit;
        }

        public static int GetTodayDonateHCLimit(int level)
        {
            if (level >= AlliancesManager.Instance.Config.Levels.Length)
            {
                level = AlliancesManager.Instance.Config.Levels.Length - 1;
            }

            return AlliancesManager.Instance.Config.Levels[level].TodayDonateHCLimit;
        }

        public static int GetLevelUpCost(int targetLevel)
        {
            if (targetLevel >= AlliancesManager.Instance.Config.Levels.Length)
            {
                targetLevel = AlliancesManager.Instance.Config.Levels.Length - 1;
            }

            return AlliancesManager.Instance.Config.Levels[targetLevel].LevelUpCost;
        }

        public static int GetTechLevelUpCost(int targetLevel)
        {
            if (targetLevel >= AlliancesManager.Instance.Config.Technologys.Length)
            {
                EB.Debug.LogError("GetTechLevelUpCost targetLevel>={0}" , AlliancesManager.Instance.Config.Technologys.Length);
                return 0;
            }
            return AlliancesManager.Instance.Config.Technologys[targetLevel].CostBalance;
        }

        public static int GetCopyUnlockNum()
        {
            int unlockNum = 0;
            var infos = AlliancesManager.Instance.CopyInfo.Infos;
            for (var i = 0; i < infos.Count; i++)
            {
                var info = infos[i];
                if (info.State == eAllianceCopyState.Unlock)
                    unlockNum++;
            }
            return unlockNum;
        }
    }
}
