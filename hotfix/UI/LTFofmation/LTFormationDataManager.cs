using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hotfix_LT.Data;


namespace Hotfix_LT.UI
{
    public class LTFormationDataManager : ManagerUnit
    {
        private static LTFormationDataManager sInstance = null;
        public static LTFormationDataManager Instance
        {
            get { return sInstance = sInstance ?? LTHotfixManager.GetManager<LTFormationDataManager>(); }
        }

        public FormationAPI API { get; private set; }

        public override void Connect()
        {
            //State = EB.Sparx.SubSystemState.Connected;
        }

        public static int MainTeamPower;//主队战力

        public Dictionary<int,int> HeroBattleTempPartner;//pos,statId
        public void InitHeroBattleTp()
        {
            HeroBattleTempPartner = new Dictionary<int,int>();
        }
        
        public LTPartnerData GetHeroBattleDataByStatId(int statId)
        {
            LTPartnerData partnerData = new LTPartnerData();
            partnerData.StatId = statId;
            partnerData.InfoId = partnerData.StatId - 1;
            partnerData.HeroStat = CharacterTemplateManager.Instance.GetHeroStat(partnerData.StatId);
            partnerData.HeroInfo = CharacterTemplateManager.Instance.GetHeroInfo(partnerData.InfoId);
            return partnerData;
        }
        public int GetHeroBattleDataPosByStatId(int statId)
        {
            foreach (var VARIABLE in HeroBattleTempPartner)
            {
                if (VARIABLE.Value==statId)
                {
                    return VARIABLE.Key;
                }
            }
            return -1;
        }
        
        /// <summary>
        /// 英雄交锋获取虚拟伙伴
        /// </summary>
        /// <param name="statIdList"></param>
        /// <returns></returns>
        public List<LTPartnerData> GetVirtualPartnerList()
        {
            List<LTPartnerData> mList = new List<LTPartnerData>();
            
            int id = LTNewHeroBattleManager.GetInstance().GetCurrentFinishLayer();
            HeroBattleTemplate temp = EventTemplateManager.Instance.GetNextHeroBattleData(id);
            foreach (var statId in temp.OurHeroConfig)
            {
                try
                {
                    if (HeroBattleTempPartner.ContainsValue(int.Parse(statId))) continue;
                    LTPartnerData partnerData =GetHeroBattleDataByStatId(int.Parse(statId));
                    mList.Add(partnerData);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
            return mList;
        }

        public static void OnRefreshMainTeamPower(bool isShowFloat)
        {
            if (MainTeamPower == 0)
            {
                MainTeamPower = LTPartnerDataManager.Instance.GetAllPower();
                return;
            }

            if (isShowFloat)
            {
                int perpower = MainTeamPower;
                MainTeamPower = LTPartnerDataManager.Instance.GetAllPower();
                int addNum = MainTeamPower - perpower;
                if (addNum == 0) return;
                string sym = addNum > 0 ? "+" : "";
                MessageTemplateManager.ShowMessage(eMessageUIType.CombatPowerText, string.Format("{0},{1}{2}", MainTeamPower, sym, addNum));
            }
            else
            {
                MainTeamPower = LTPartnerDataManager.Instance.GetAllPower();
            }
            Hotfix_LT.Messenger.Raise<int>(EventName.onCombatTeamPowerUpdate, MainTeamPower);


        }

        public override void Disconnect(bool isLogout)
        {
            //State = EB.Sparx.SubSystemState.Disconnected;
            ClearData();
        }

        public override void Initialize(EB.Sparx.Config config)
        {
            Instance.API = new FormationAPI();
            Instance.API.ErrorHandler += ErrorHandler;
        }

        public override void OnLoggedIn()
        {
            base.OnLoggedIn();
            SetFormationData();
        }

        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        #region 荣耀竞技场
        //荣耀竞技场上阵伙伴
        private static int HonorALLNUM = HonorConfig.ALLNUM;
        private LTPartnerData[] ArenaPartnerData_Def = new LTPartnerData[HonorALLNUM];
        private LTPartnerData[] ArenaPartnerData_Attack = new LTPartnerData[HonorALLNUM];

        public bool IsInTransTeam(int heroid, bool isDef)
        {
            LTPartnerData[] ArenaPartnerData = isDef ? ArenaPartnerData_Def : ArenaPartnerData_Attack;
            for (int i = 0; i < ArenaPartnerData.Length; i++)
            {
                if (ArenaPartnerData[i] != null)
                {
                    if (ArenaPartnerData[i].HeroId == heroid)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void SetArenaPartnerData(LTPartnerData data, int index, bool isDef)
        {
            LTPartnerData[] ArenaPartnerData = isDef ? ArenaPartnerData_Def : ArenaPartnerData_Attack;
            ArenaPartnerData[index] = data;
        }

        public LTPartnerData[] GetArenaPartnerData(bool isDef)
        {
            LTPartnerData[] ArenaPartnerData = isDef ? ArenaPartnerData_Def : ArenaPartnerData_Attack;
            return ArenaPartnerData;
        }

        public void SwapArenaPartnerDataTeam(int i, int j, bool isDef)
        {
            if (i == j || i < 0 || j < 0 || i > 2 || j > 2)
            {
                Debug.LogError("arg Error");
                return;
            }
            LTPartnerData[] ArenaPartnerData = isDef ? ArenaPartnerData_Def : ArenaPartnerData_Attack;
            LTPartnerData[] temp = new LTPartnerData[6];
            Array.Copy(ArenaPartnerData, i * 6, temp, 0, 6);
            Array.Copy(ArenaPartnerData, j * 6, ArenaPartnerData, i * 6, 6);
            Array.Copy(temp, 0, ArenaPartnerData, j * 6, 6);
        }

        public void SwapArenaPartnerDataSingle(int i, int j, bool isDef)
        {
            LTPartnerData[] ArenaPartnerData = isDef ? ArenaPartnerData_Def : ArenaPartnerData_Attack;
            LTPartnerData temp = ArenaPartnerData[i];
            ArenaPartnerData[i] = ArenaPartnerData[j];
            ArenaPartnerData[j] = temp;
        }

        /// <summary>
        /// 荣耀角斗场阵型是否为空
        /// </summary>
        /// <returns></returns>
        public bool IsArenaPartnerDataEmpty(string[] teamName1)
        {
            for (int i = 0; i < teamName1.Length; i++)
            {
                int temp = IsArenaPartnerDataEmpty(teamName1[i]);
                if (temp == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public int IsArenaPartnerDataEmpty(string teamName1)
        {
            List<TeamMemberData> temp = GetTeamMemList(teamName1);
            return temp.Count;
        }

        #endregion


        private Dictionary<string, List<TeamMemberData>> TeamMemDataDic = new Dictionary<string, List<TeamMemberData>>();
    
        /// <summary>
        /// refresh all team
        /// </summary>
        /// <param name="refreshName">only refresh "refreshName“ team</param>
        public void SetFormationData(string refreshName = "")
        {
            for (int i = 0; i < SmallPartnerPacketRule.USER_TEAM_LIST.Count; i++)
            {
                string teamName = SmallPartnerPacketRule.USER_TEAM_LIST[i];
                if (!refreshName.Equals("") && !teamName.Equals(refreshName))
                {
                    continue;
                }

                List<TeamMemberData> oneTeamMemDataList = GetOneTeamMemList(teamName);
                if (!TeamMemDataDic.ContainsKey(teamName))
                    TeamMemDataDic.Add(teamName, oneTeamMemDataList);
                else
                {
                    TeamMemDataDic[teamName].Clear();
                    TeamMemDataDic[teamName] = oneTeamMemDataList;
                    if (CurTeamMemberData != null && ContainMercenaryType())
                    {
                        TeamMemDataDic[teamName].Add(CurTeamMemberData);
                    }
                }
            }
        }

        public bool ContainMercenaryType()
        {
           string name= FormationUtil.GetCurrentTeamName();
           if (name.Equals("team1") || name.Equals("lt_challenge_camp"))
           {
               return true;
           }

           return false;
        }

        private List<TeamMemberData> GetOneTeamMemList(string teamname)
        {
            List<TeamMemberData> oneTeamMemDataList = new List<TeamMemberData>();
            try
            {
                ArrayList teamHash;
                DataLookupsCache.Instance.SearchDataByID<ArrayList>(SmallPartnerPacketRule.USER_TEAM + "." + teamname + "." + SmallPartnerPacketRule.USER_TEAM_FORMATION, out teamHash);
                int nPos = -1;
                Hashtable partnerServerData = null;
                if (teamHash != null)
                {
                    for (var i = 0; i < teamHash.Count; i++)
                    {
                        var teamMemData = teamHash[i];
                        nPos++;
                        IDictionary teamMemDataDic = teamMemData as IDictionary;
                        if (teamMemDataDic == null || !teamMemDataDic.Contains(SmallPartnerPacketRule.USER_TEAM_FORMATION_HERO_ID) || teamMemDataDic[SmallPartnerPacketRule.USER_TEAM_FORMATION_HERO_ID] == null)
                        {
                            continue;
                        }

                        int nHeroID = EB.Dot.Integer(SmallPartnerPacketRule.USER_TEAM_FORMATION_HERO_ID, teamMemDataDic, 0);
                        if (nHeroID > 0)
                        {
                            DataLookupsCache.Instance.SearchDataByID<Hashtable>(SmallPartnerPacketRule.OWN_HERO_STATS + "." + nHeroID, out partnerServerData);
                            if (partnerServerData == null)
                            {
                                EB.Debug.LogError("cannot find heroStat data for heroID ={0}", nHeroID);
                                continue;
                            }
                            int nStatsID = EB.Dot.Integer(SmallPartnerPacketRule.HERO_TEMPLATE_ID, partnerServerData, -1);
                            var tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(nStatsID);
                            int nInfoID = tpl.character_id;
                            var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(nInfoID, EB.Dot.Integer(SmallPartnerPacketRule.HERO_SKIN, partnerServerData, 0));

                            TeamMemberData teamMemDataNew = new TeamMemberData();
                            teamMemDataNew.HeroID = nHeroID;
                            teamMemDataNew.InfoID = nInfoID;
                            teamMemDataNew.ModelName = charTpl.model_name;//需添加皮肤
                            teamMemDataNew.Pos = nPos;
                            oneTeamMemDataList.Add(teamMemDataNew);
                        }
                        else
                        {
                            var hireInfo = LTInstanceHireUtil.GetHireInfoByHeroId(nHeroID);
                            if (hireInfo == null || teamname.IndexOf("team") < 0)
                            {
                                EB.Debug.Log("userTeam data heroID <= {0}", nHeroID);
                                continue;
                            }
                            int nInfoID = int.Parse(hireInfo.character_id);
                            var tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStatByInfoId(nInfoID);
                            var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(nInfoID);

                            TeamMemberData teamMemDataNew = new TeamMemberData();
                            teamMemDataNew.HeroID = -nHeroID;
                            teamMemDataNew.InfoID = nInfoID;
                            teamMemDataNew.ModelName = charTpl.model_name;
                            teamMemDataNew.Pos = nPos;
                            teamMemDataNew.IsHire = true;
                            oneTeamMemDataList.Add(teamMemDataNew);
                        }
                    }
                }
            }
            catch(NullReferenceException e)
            {
                EB.Debug.LogError(e.ToString());
            }
            return oneTeamMemDataList;
        }


        public List<TeamMemberData> GetTeamMemList(string teamName)
        {
            if (teamName.Equals("lt_mercenary"))
            {
                List<TeamMemberData> temp = new List<TeamMemberData>();
                int ser_heroId = AlliancesManager.Instance.GetMercenaryHeroId();
                if (ser_heroId > 0)
                {
                    LTPartnerData parData = LTPartnerDataManager.Instance.GetPartnerByHeroId(ser_heroId);
                    TeamMemberData teamMemData = new TeamMemberData();
                    teamMemData.HeroID = ser_heroId;
                    teamMemData.InfoID = parData.InfoId;
                    teamMemData.ModelName = parData.HeroInfo.model_name;
                    teamMemData.Pos = 0;
                    temp.Add(teamMemData);
                }
            
                return temp;
            }
            
            if (TeamMemDataDic == null || string.IsNullOrEmpty(teamName))
            {
                return new List<TeamMemberData>();
            }
            if (!TeamMemDataDic.ContainsKey(teamName))
            {
                SetFormationData(teamName);
            }
            return TeamMemDataDic[teamName];
        }

        public TeamMemberData[] GetTeamMemArray(string teamName)
        {
            List<TeamMemberData> list = GetTeamMemList(teamName);
            TeamMemberData[] array = new TeamMemberData[6];
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].Pos < 6) array[list[j].Pos] = list[j];
                }
            }
            return array;
        }


        /// <summary>
        /// 想要获取的队伍信息，用于释放资源ObjectManager
        /// </summary>
        private readonly List<string> UserTeamList = new List<string>
        {
            "team1",
            "team2",
            "team3",
            "lt_challenge_camp",
            "lt_aw_camp",
        };

        public static List<string> GetTeamsModeListFromILR()
        {
            if (Instance != null)
            {
                return Instance.GetTeamsModeList();
            }

            return new List<string>();
        }

        /// <summary>
        /// 获取所有伙伴模型数据
        /// </summary>
        /// <returns></returns>
        public List<string> GetTeamsModeList()
        {
            List<string> temp = new List<string>();
            Hotfix_LT.Data.HeroInfoTemplate heroStat = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(LTMainHudManager.Instance.UserLeaderTID - 1, LTMainHudManager.Instance.UserLeaderSkin);
            if (heroStat != null)
            {
                string name = heroStat.model_name.Replace("-Variant", "");//需添加皮肤
                if (!temp.Contains(name))
                {
                    temp.Add(name);
                }
            }
            for (var k = 0; k < UserTeamList.Count; k++)
            {
                var listName = UserTeamList[k];
                if (!TeamMemDataDic.ContainsKey(listName)) continue;
                List<TeamMemberData> lists = TeamMemDataDic[listName];
                for (int i = 0; i < lists.Count; i++)
                {
                    if (!string.IsNullOrEmpty(lists[i].ModelName))
                    {
                        string name = lists[i].ModelName.Replace("-Variant", "");
                        if (!temp.Contains(name))
                        {
                            temp.Add(name);
                        }
                    }
                }
            }
            return temp;
        }

        /// <summary>
        /// 判定该伙伴下阵后是否只剩雇佣兵
        /// </summary>
        /// <returns></returns>
        public bool IsRequestDragoutVaild(int heoId, string teamName)
        {
            var curTeamMemDataList = LTFormationDataManager.Instance.GetTeamMemList(teamName);
            if (curTeamMemDataList.Count > 0)
            {
                for (int i = 0; i < curTeamMemDataList.Count; i++)
                {
                    if (curTeamMemDataList[i].HeroID != heoId && !curTeamMemDataList[i].IsHire)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 判定该雇佣兵上阵后是否只剩雇佣兵
        /// </summary>
        /// <param name="heroId"></param>
        /// <param name="index"></param>
        /// <param name="teamName"></param>
        /// <returns></returns>
        public bool IsRequestDragInVaild(int pos, string teamName)
        {
            var curTeamMemDataList = LTFormationDataManager.Instance.GetTeamMemList(teamName);
            if (curTeamMemDataList.Count > 0)
            {
                for (int i = 0; i < curTeamMemDataList.Count; i++)
                {
                    if (pos != curTeamMemDataList[i].Pos && !curTeamMemDataList[i].IsHire)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<TeamMemberData> GetCurrentTeamMemList()
        {
            return GetTeamMemList(FormationUtil.GetCurrentTeamName());
        }

        public bool TeamMemListHasSame(int InfoId)
        {
            foreach (var VARIABLE in TeamMemDataDic[FormationUtil.GetCurrentTeamName()])
            {
                if (VARIABLE.InfoID == InfoId)
                {
                    return true;
                }
            }

            return false;
        }
        
        
        Dictionary<string, float> DIC_ATTR_CritP = new Dictionary<string, float>();
        Dictionary<string, float> DIC_ATTR_Speed = new Dictionary<string, float>();
        Dictionary<string, float> DIC_ATTR_CRIresist = new Dictionary<string, float>();
        Dictionary<string, float> Dic_ATTR_DMGincrease = new Dictionary<string, float>();
        Dictionary<string, float> Dic_ATTR_DMGreduction = new Dictionary<string, float>();
        Dictionary<string, float> handboodAddATKData = new Dictionary<string, float>()
    {
        {"1",0 },
        {"2",0 },
        {"3",0 },
    };
        Dictionary<string, float> handboodAddDEFData = new Dictionary<string, float>()
    {
        {"1",0 },
        {"2",0 },
        {"3",0 },
    };
        Dictionary<string, float> handboodAddMAXHPData = new Dictionary<string, float>()
    {
        {"1",0 },
        {"2",0 },
        {"3",0 },
    };
        int Handbook_Level;
        /// <summary>
        /// 处理其他玩家的图鉴属性加成相关数据，计算的数据储存在CurrentPartnerAttrDic中
        /// </summary>
        private void HandleOtherPlayerPartnerAttributeData(Hashtable otherPlayerBuddy)
        {
            if (!otherPlayerBuddy.ContainsKey("mannualInfo"))
            {
                return;
            }

            Handbook_Level = EB.Dot.Integer("level", otherPlayerBuddy, 0);
            Hashtable mannualInfo = otherPlayerBuddy["mannualInfo"] as Hashtable;

            if (mannualInfo != null)
            {
                foreach (DictionaryEntry handbook in mannualInfo)
                {
                    Hashtable handbookData = handbook.Value as Hashtable;
                    string handbookId = handbook.Key.ToString();
                    int handbookbreak = 0;

                    if (handbookData["break"] != null)
                    {
                        handbookbreak = int.Parse(handbookData["break"].ToString());
                    }

                    Data.MannualBreakTemplate mannualBreak = Data.CharacterTemplateManager.Instance.GetBreakTemplateByLevel((Data.eRoleAttr)int.Parse(handbookId), handbookbreak);
                    //这里开始处理每个图鉴的五个伙伴，按顺序
                    ArrayList buddyInfos = EBCore.Dot.Array("buddyInfo", handbookData, null);
                    int bookScore = 0;

                    if (buddyInfos != null)
                    {
                        var count = buddyInfos.Count;

                        for (int i = 0; i < count; i++)
                        {
                            Hashtable buddyInfo = buddyInfos[i] as Hashtable;

                            if (buddyInfo != null && buddyInfo["id"] != null)
                            {
                                int starCount = int.Parse(buddyInfo["star"].ToString());
                                string characterId = buddyInfo["character_id"].ToString();
                                Hashtable state = buddyInfo["stat"] as Hashtable;
                                int heroLevel = int.Parse(state["level"].ToString());
                                Hashtable awake = state["awake"] as Hashtable;
                                int awakelevel = 0;

                                if (awake != null && awake["level"] != null)
                                {
                                    awakelevel = int.Parse(awake["level"].ToString());
                                }

                                Data.HeroInfoTemplate heroInfo = Data.CharacterTemplateManager.Instance.GetHeroInfo(characterId);//, skin);
                                int upgradeLevel = heroInfo.role_grade;
                                int cardScore = LTPartnerHandbookManager.Instance.HandleHandbookCardScore(mannualBreak, upgradeLevel, awakelevel);
                                bookScore += cardScore;

                                Data.MannualRoleGradeTemplate roleGrade = Data.CharacterTemplateManager.Instance.GetMannualRoleGradeTempleteByRoleGrade(upgradeLevel);
                                switch (i)
                                {
                                    //第一张卡是暴击
                                    case 0:
                                    case 5:
                                        if (DIC_ATTR_CritP.ContainsKey(handbookId))
                                            DIC_ATTR_CritP[handbookId] += starCount * roleGrade.star_addition;
                                        else
                                            DIC_ATTR_CritP[handbookId] = starCount * roleGrade.star_addition;
                                        break;
                                    //第二张卡是暴击抵抗
                                    case 1:
                                    case 6:
                                        if (DIC_ATTR_CRIresist.ContainsKey(handbookId)) DIC_ATTR_CRIresist[handbookId] += starCount * roleGrade.star_addition;
                                        else DIC_ATTR_CRIresist[handbookId] = starCount * roleGrade.star_addition;
                                        break;
                                    case 2:
                                    case 7://速度卡
                                        if (DIC_ATTR_Speed.ContainsKey(handbookId))
                                            DIC_ATTR_Speed[handbookId] += starCount * roleGrade.star_addition;
                                        else
                                            DIC_ATTR_Speed[handbookId] = starCount * roleGrade.star_addition;
                                        break;
                                    case 3:
                                    case 8:
                                        if (Dic_ATTR_DMGincrease.ContainsKey(handbookId)) Dic_ATTR_DMGincrease[handbookId] += starCount * roleGrade.star_addition;
                                        else Dic_ATTR_DMGincrease[handbookId] = starCount * roleGrade.star_addition;
                                        break;
                                    case 4:
                                    case 9:
                                        if (Dic_ATTR_DMGreduction.ContainsKey(handbookId)) Dic_ATTR_DMGreduction[handbookId] += starCount * roleGrade.star_addition;
                                        else Dic_ATTR_DMGreduction[handbookId] = starCount * roleGrade.star_addition;
                                        break;
                                }
                            }
                        }
                    }

                    if (!handboodAddATKData.ContainsKey(handbookId))
                    {
                        handboodAddATKData.Add(handbookId, 0);
                    }
                    if (!handboodAddDEFData.ContainsKey(handbookId))
                    {
                        handboodAddDEFData.Add(handbookId, 0);
                    }
                    if (!handboodAddMAXHPData.ContainsKey(handbookId))
                    {
                        handboodAddMAXHPData.Add(handbookId, 0);
                    }
                    handboodAddATKData[handbookId] = 0;
                    handboodAddDEFData[handbookId] = 0;
                    handboodAddMAXHPData[handbookId] = 0;
                    if (mannualBreak != null)
                    {
                        handboodAddATKData[handbookId] = mannualBreak.IncATK;
                        handboodAddDEFData[handbookId] = mannualBreak.IncDEF;
                        handboodAddMAXHPData[handbookId] = mannualBreak.IncMaxHp;
                    }
                }
            }
        }

        //private float allianceATKAdd;
        //private float allianceDEFAdd;
        //private float allianceMaxHpAdd;

        private AllianceAttriAddtion otherallianceAttri;
        private Dictionary<int, int> TechlevelDic;
        /// <summary>
        /// 处理其他玩家的军团属性加成
        /// </summary>
        /// <param name="allianceTable"></param>
        public void HandleOtherPlayerAllianceAdd(Hashtable allianceTable)
        {
            Hashtable skilldata = EB.Dot.Object("skills", allianceTable, null);
            if (allianceTable != null && allianceTable.Count > 0 && skilldata != null)
            {
                //int allianceLevel = int.Parse(allianceTable["lv"].ToString());
                UpdataTechSkill(skilldata);
                List<Data.AllianceTechSkill> skilllist = Data.AllianceTemplateManager.Instance.mTechSkillList;
                if (otherallianceAttri == null)
                {
                    otherallianceAttri = new AllianceAttriAddtion();
                }
                else
                {
                    otherallianceAttri.Clear();
                }
                for (int i = 0; i < skilllist.Count; i++)
                {
                    var tempskill = skilllist[i];

                    if (TechlevelDic.TryGetValue(tempskill.skillid, out int level))
                    {
                        int typecard = tempskill.charType * 10 + tempskill.addtionType;//fengshuihuo123/atk,def,hp123
                        switch (typecard)
                        {
                            case 11:
                                otherallianceAttri.FengAtk += tempskill.levelinfo[level].addition;
                                break;
                            case 12:
                                otherallianceAttri.FengDef += tempskill.levelinfo[level].addition;
                                break;
                            case 13:
                                otherallianceAttri.FengMhp += tempskill.levelinfo[level].addition;
                                break;
                            case 21:
                                otherallianceAttri.ShuiAtk += tempskill.levelinfo[level].addition;
                                break;
                            case 22:
                                otherallianceAttri.ShuiDef += tempskill.levelinfo[level].addition;
                                break;
                            case 23:
                                otherallianceAttri.ShuiMhp += tempskill.levelinfo[level].addition;
                                break;
                            case 31:
                                otherallianceAttri.HuoAtk += tempskill.levelinfo[level].addition;
                                break;
                            case 32:
                                otherallianceAttri.HuoDef += tempskill.levelinfo[level].addition;
                                break;
                            case 33:
                                otherallianceAttri.HuoMhp += tempskill.levelinfo[level].addition;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private void UpdataTechSkill(Hashtable data)
        {
            if (data != null)
            {
                if (TechlevelDic == null)
                {
                    TechlevelDic = new Dictionary<int, int>();
                }
                else
                {
                    TechlevelDic.Clear();
                }
                foreach (DictionaryEntry ad in data)
                {
                    int skillId = EB.Dot.Integer("", ad.Key, 0);
                    int level = EB.Dot.Integer("", ad.Value, 0);
                    if (skillId != 0)
                    {
                        TechlevelDic.Add(skillId, level);
                    }
                }

            }
        }

        private LTAttributesData HanleBaseAttribute(int heroStatId, int heroUpgradeId, int star, int starHole, int heroLevel, int AllRoundLevel, int ControlLevel, int StrongLevel, int RageLevel, int AbsorbedLevel, int awakenLevel, int InfoId, IDictionary fetter, int commonSkillLevel, int activeSkillLevel, int passiveSkillLevel)
        {
            LTAttributesData baseAttrData = AttributesManager.GetPartnerBaseAttributesByParnterData(heroStatId, heroUpgradeId, star, starHole, heroLevel, AllRoundLevel, ControlLevel, StrongLevel, RageLevel, AbsorbedLevel, awakenLevel, InfoId, fetter, commonSkillLevel, activeSkillLevel, passiveSkillLevel);
            return baseAttrData;
        }


        /// <summary>
        /// 处理其他人的对应伙伴的属性，处理军团及图鉴加成
        /// </summary>
        private LTAttributesData HandleOtherPlayerAttribute(LTAttributesData baseAttrData, int charType)
        {
            LTAttributesData totalAttrData = new LTAttributesData(baseAttrData);

            //图鉴固定值
            LTAttributesData handbookAttr = AttributesManager.GetPartnerAHandbookAttributes(Handbook_Level);
            totalAttrData.Add(handbookAttr);

            //军团及图鉴加成
            float allianceMaxHpAdd = 0;
            float allianceATKAdd = 0;
            float allianceDEFAdd = 0;
            if (otherallianceAttri != null)
            {
                otherallianceAttri.GetAttrAddtionByCharType((Data.eRoleAttr)charType, out allianceATKAdd, out allianceDEFAdd, out allianceMaxHpAdd);
            }
            totalAttrData.m_MaxHP += baseAttrData.m_MaxHP * (allianceMaxHpAdd + handboodAddMAXHPData[charType.ToString()]);
            totalAttrData.m_ATK += baseAttrData.m_ATK * (allianceATKAdd + handboodAddATKData[charType.ToString()]);
            totalAttrData.m_DEF += baseAttrData.m_DEF * (allianceDEFAdd + handboodAddDEFData[charType.ToString()]);

            float speedAdd1 = DIC_ATTR_Speed.ContainsKey(charType.ToString()) ? DIC_ATTR_Speed[charType.ToString()] : 0;
            float critpAdd1 = DIC_ATTR_CritP.ContainsKey(charType.ToString()) ? DIC_ATTR_CritP[charType.ToString()] : 0;
            float critdAdd1 = DIC_ATTR_CRIresist.ContainsKey(charType.ToString()) ? DIC_ATTR_CRIresist[charType.ToString()] : 0;
            float dmgincAdd1 = Dic_ATTR_DMGincrease.ContainsKey(charType.ToString()) ? Dic_ATTR_DMGincrease[charType.ToString()] : 0;
            float dmgrdcAdd1 = Dic_ATTR_DMGreduction.ContainsKey(charType.ToString()) ? Dic_ATTR_DMGreduction[charType.ToString()] : 0;
            totalAttrData.m_Speed += baseAttrData.m_Speed * (speedAdd1);
            totalAttrData.m_CritP += (critpAdd1);
            totalAttrData.m_CritDef += critdAdd1;
            totalAttrData.m_DamageAdd += dmgincAdd1;
            totalAttrData.m_DamageReduce += dmgrdcAdd1;

            //晋升加成
            if (_promotionAttrData != null) {
                totalAttrData.Add(_promotionAttrData);
                totalAttrData.SpecialAdd(baseAttrData,_promotionAttrData);
            }

            //最终白字
            return totalAttrData;
        }

        public void GetOtherPlayerData(long uid, string type, string dataType, object data, System.Action<List<OtherPlayerPartnerData>, string> callback)
        {
            RequestOtherPlayerData(uid, type, dataType, data, delegate (Hashtable result)
            {
                string alliancePath = uid + "." + SmallPartnerPacketRule.USER_TEAM + "." + type + "." + SmallPartnerPacketRule.OTHER_PALYER_ALLIANCE;
                string alliName = EB.Dot.String(alliancePath + ".un", result, "");
                Hashtable InfoData = EB.Dot.Object(uid.ToString(), result, null);
                var datalist = GetOtherPalyerPartnerDataList(type, InfoData);
                callback(datalist, alliName);
            });
        }

        public void GetOtherHonorPlayerData(string uid, object data, System.Action<List<OtherPlayerPartnerData>> callback)
        {
            RequestHonorOtherPlayerData(uid, data, delegate (Hashtable result)
            {
                string dataPath = string.Format("{0}.{1}.{2}", uid, "userTeam", "honor_arena");
                ArrayList InfoData = EB.Dot.Array(dataPath, result, null);
                List<OtherPlayerPartnerData> datas = new List<OtherPlayerPartnerData>();
                foreach (IDictionary temp in InfoData)
                {
                    //得到伙伴数据OtherPlayerPartnerData
                    Hashtable hs = (Hashtable)temp;
                    datas.AddRange(GetOtherPalyerPartnerDataList("honor_arena", hs, true));
                }

                callback(datas);
            });
        }

        public void GetMercenaryPlayerData(string uid,int heroId,Action<List<OtherPlayerPartnerData>> callback)
        {
            GetHeroInfoForView(heroId,delegate (Hashtable result)
            {
                string dataPath = string.Format("{0}.{1}.{2}", "mercenary", "herostatView", uid);
                Hashtable InfoData = EB.Dot.Object(dataPath, result, null);
                List<OtherPlayerPartnerData> datas = GetOtherPalyerPartnerDataList("mercenary", InfoData, true);
                callback(datas);
            });
        }

        public List<OtherPlayerPartnerData> GetOtherPalyerPartnerDataList(string TeamName, Hashtable result, bool simplePath = false)
        {
            string alliancePath = simplePath ? "alliance" : string.Format("{0}.{1}.{2}", "userTeam", TeamName, "alliance");
            string dataPath = simplePath ? "formation_info" : string.Format("{0}.{1}.{2}", "userTeam", TeamName, "formation_info");
            string buddyMannualPath = simplePath ? "buddyMannual" : string.Format("{0}.{1}.{2}", "userTeam", TeamName, "buddyMannual");
            string promotionPath = simplePath ? "promotion" : string.Format("{0}.{1}.{2}", "userTeam", TeamName, "promotion");

            Handbook_Level = 0;
            DIC_ATTR_CritP.Clear();
            DIC_ATTR_Speed.Clear();
            DIC_ATTR_CRIresist.Clear();
            Dic_ATTR_DMGincrease.Clear();
            Dic_ATTR_DMGreduction.Clear();

            var count = handboodAddATKData.Count;

            for (int i = 1; i <= count; ++i)
            {
                var str = i.ToString();
                handboodAddATKData[str] = 0;
                handboodAddDEFData[str] = 0;
                handboodAddMAXHPData[str] = 0;
            }

            Hashtable buddyHandbookData = EB.Dot.Object(buddyMannualPath, result, null);

            if (buddyHandbookData != null)
            {
                HandleOtherPlayerPartnerAttributeData(buddyHandbookData);
            }

            if (otherallianceAttri == null) otherallianceAttri = new AllianceAttriAddtion();
            else otherallianceAttri.Clear();
             Hashtable aliianceData = EB.Dot.Object(alliancePath, result, null);

            if (aliianceData != null)
            {
                HandleOtherPlayerAllianceAdd(aliianceData);
            }

            //晋升
            Hashtable promotionData = EB.Dot.Object(promotionPath, result, null);

            if (promotionData != null) {
                _promotionAttrData = GetOtherPartnerPromotionAttributes(promotionData);
            } else {
                _promotionAttrData = null;
            }

            ArrayList checkOnlyAllPartnerData = EBCore.Dot.Array(dataPath, result, null);
            var datalist = GetCheckOnlyPartnerDataList(checkOnlyAllPartnerData);
            return datalist;
        }

        private LTAttributesData _promotionAttrData;

        /// <summary>
        /// 获取晋升加成 
        /// </summary>
        private static LTAttributesData GetOtherPartnerPromotionAttributes(Hashtable ht) {
            LTAttributesData attrData = new LTAttributesData();
            int level = EB.Dot.Integer("level", ht, 0);
            var attrList = CharacterTemplateManager.Instance.GetPromotionAttributeLevelList(level); 
            for (int i=0;i< attrList.Count; ++i)
            {
                float value= EB.Dot.Integer(string.Format ("attrs.{0}.level", attrList[i].id), ht, 0)* attrList[i].attrValue;
                attrData.Add(attrList[i].name, value);
            }
            return attrData;
        }

        private const int OtherPartnerLength = 6;
        /// <summary>
        /// 构建其他玩家的布阵伙伴数据
        /// </summary>
        /// <param name="serverDataList"></param>
        /// <returns></returns>
        private List<OtherPlayerPartnerData> GetCheckOnlyPartnerDataList(ArrayList serverDataList)
        {
            var checkOnlyPartnerDataList = new List<OtherPlayerPartnerData>();
            if (serverDataList == null)
            {
                //填充空的数据
                for (int i = checkOnlyPartnerDataList.Count; i < OtherPartnerLength; i++)
                {
                    checkOnlyPartnerDataList.Add(new OtherPlayerPartnerData());
                }
                return checkOnlyPartnerDataList;
            }

            var count = Math.Min(serverDataList.Count, OtherPartnerLength);
            for (var k = 0; k < count; k++)
            {
                var serverData = serverDataList[k];
                // 单个formation
                IDictionary serverDataDic = serverData as IDictionary;

                if (serverDataDic == null)
                {
                    checkOnlyPartnerDataList.Add(new OtherPlayerPartnerData());
                    continue;
                }

                OtherPlayerPartnerData smallPartnerData = new OtherPlayerPartnerData();
                string type = "";
                type = serverDataDic[SmallPartnerPacketRule.OTHER_PLAYER_DATA_PARAM1_DATA4].ToString();
                int statlevel = EB.Dot.Integer(SmallPartnerPacketRule.OTHER_PLAYER_DATA_PARAM1_DATA2, serverDataDic, 0);
                int infoId;
                int.TryParse(serverDataDic[SmallPartnerPacketRule.OTHER_PLAYER_DATA_PARAM1_DATA6].ToString(), out infoId);
                int skin = 0;

                if (serverDataDic[SmallPartnerPacketRule.OTHER_PLAYER_DATA_PARAM1_DATA8] != null)
                {
                    int.TryParse(serverDataDic[SmallPartnerPacketRule.OTHER_PLAYER_DATA_PARAM1_DATA8].ToString(), out skin);
                }

                var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(infoId, skin);
                int star = 0;
                int.TryParse(serverDataDic[SmallPartnerPacketRule.OTHER_PLAYER_DATA_PARAM1_DATA3].ToString(), out star);

                if (star == 0 && charTpl != null)
                {
                    star = charTpl.init_star;
                }

                //护卫加成
                IDictionary fetter = null;
                if (serverDataDic.Contains(SmallPartnerPacketRule.OTHER_PALYER_Fetter))
                {
                    fetter = serverDataDic[SmallPartnerPacketRule.OTHER_PALYER_Fetter] as IDictionary;
                }

                int awakenLevel = EB.Dot.Integer(SmallPartnerPacketRule.OTHER_PLAYER_DATA_PARAM1_DATA9, serverDataDic, 0);
                int upgrade = EB.Dot.Integer(SmallPartnerPacketRule.OTHER_PLAYER_DATA_PARAM1_DATA7, serverDataDic, 0);
                int templeteId = infoId + 1;//不直接取templeteId是因为机器人用的为怪物表templeteId。
                ArrayList skillList = EBCore.Dot.Array("skillList", serverDataDic, null);
                Hashtable equipments = EB.Dot.Object("equipments", serverDataDic, null);
                int starHole = 0;

                if (serverDataDic.Contains("starhole") && serverDataDic["starhole"] != null)
                {
                    int.TryParse(serverDataDic["starhole"].ToString(), out starHole);
                }

                smallPartnerData.AllRoundLevel = EB.Dot.Integer("proficiency.1.1", serverDataDic, 0);
                smallPartnerData.ControlLevel = EB.Dot.Integer("proficiency.2.2", serverDataDic, 0);
                smallPartnerData.StrongLevel = EB.Dot.Integer("proficiency.2.3", serverDataDic, 0);
                smallPartnerData.RageLevel = EB.Dot.Integer("proficiency.2.4", serverDataDic, 0);
                smallPartnerData.AbsorbedLevel = EB.Dot.Integer("proficiency.2.5", serverDataDic, 0);
                smallPartnerData.ArtifactLevel = EB.Dot.Integer("artifact_equip.enhancement_level", serverDataDic, -1);
                var heroState = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(templeteId);
                int commonSkill = 0, activeSkill = 0, passiveSkill = 0;
                int commonskilllevel = 1, activeSkilllevel = 1, passiveSkilllevel = 1;
                if (heroState != null)
                {
                    commonSkill = heroState.common_skill;
                    activeSkill = heroState.active_skill;
                    passiveSkill = heroState.passive_skill;
                }
                if (skillList != null && skillList.Count != 0)
                {
                    var len = skillList.Count;

                    for (int i = 0; i < len; i++)
                    {
                        int skillId = EB.Dot.Integer("id", skillList[i], -1);
                        int skillLevel = EB.Dot.Integer("lv", skillList[i], -1);
                        if (skillId == commonSkill)
                        {
                            commonskilllevel = skillLevel;
                        }
                        else if (skillId == activeSkill)
                        {
                            activeSkilllevel = skillLevel;
                        }
                        else if (skillId == passiveSkill)
                        {
                            passiveSkilllevel = skillLevel;
                        }
                    }
                }

                LTAttributesData Attributes = HanleBaseAttribute(templeteId, upgrade, star, starHole, statlevel,smallPartnerData.AllRoundLevel, smallPartnerData.ControlLevel, smallPartnerData.StrongLevel, smallPartnerData.RageLevel, smallPartnerData.AbsorbedLevel,awakenLevel, infoId, fetter, commonskilllevel, activeSkilllevel, passiveSkilllevel);
                smallPartnerData.Attributes = Attributes;
                int charType = (int)charTpl.char_type;
                smallPartnerData.FinalAttributes = HandleOtherPlayerAttribute(Attributes, charType);

                //机器人的数据是固定的不需要算
                if (type == SmallPartnerPacketRule.OTHER_PALYER_TYPE2)  //enemy => 机器人
                {
                    int BotTempleteId = 0;
                    int.TryParse(serverDataDic[SmallPartnerPacketRule.OTHER_PLAYER_DATA_PARAM1_DATA1].ToString(), out BotTempleteId);
                    var BotData = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMonsterInfo(BotTempleteId);

                    if (BotData != null)
                    {
                        smallPartnerData.FinalAttributes.m_MaxHP = BotData.base_MaxHP;
                        smallPartnerData.FinalAttributes.m_ATK = BotData.base_ATK;
                        smallPartnerData.FinalAttributes.m_DEF = BotData.base_DEF;
                        smallPartnerData.FinalAttributes.m_Speed = BotData.speed;
                        smallPartnerData.FinalAttributes.m_CritP = BotData.CritP;
                        smallPartnerData.FinalAttributes.m_CritV = BotData.CritV;
                        smallPartnerData.FinalAttributes.m_SpExtra = BotData.SpExtra;
                        smallPartnerData.FinalAttributes.m_SpRes = BotData.SpRes;
                    }
                }

                smallPartnerData.commonSkill = commonSkill;
                smallPartnerData.commonSkillLevel = commonskilllevel;
                smallPartnerData.activeSkill = activeSkill;
                smallPartnerData.activeSkillLevel = activeSkilllevel;
                smallPartnerData.passiveSkill = passiveSkill;
                smallPartnerData.passiveSkilLevel = passiveSkilllevel;


                if (equipments != null)
                {
                    smallPartnerData.equipmentList = equipments;
                }

                smallPartnerData.HeroID = GetCheckOnlyHeroId(serverDataDic);
                smallPartnerData.Name = charTpl.name;
                smallPartnerData.Attr = charTpl.char_type;
                smallPartnerData.race = charTpl.race;
                smallPartnerData.Icon = charTpl.icon;
                smallPartnerData.UpGradeId = EB.Dot.Integer("stat.upgrade", serverDataDic, 0);
                smallPartnerData.RoleProflie = charTpl.role_profile;
                smallPartnerData.RoleProflieSprite = charTpl.role_profile_icon;
                smallPartnerData.QualityLevel = charTpl.role_grade;
                smallPartnerData.Level = statlevel;
                smallPartnerData.Star = star;
                smallPartnerData.awakenLevel = awakenLevel;
                smallPartnerData.InfoId = infoId;

                checkOnlyPartnerDataList.Add(smallPartnerData);
            }
            //填充空的数据
            for (int i = checkOnlyPartnerDataList.Count; i < OtherPartnerLength; i++)
            {
                checkOnlyPartnerDataList.Add(new OtherPlayerPartnerData());
            }
            return checkOnlyPartnerDataList;
        }

        private int GetCheckOnlyHeroId(IDictionary serverDataDic)
        {
            string type = serverDataDic[SmallPartnerPacketRule.OTHER_PLAYER_DATA_PARAM1_DATA4].ToString();
            if (type != SmallPartnerPacketRule.OTHER_PALYER_TYPE2)
            {
                int heroId = 0;
                int.TryParse(serverDataDic[SmallPartnerPacketRule.OTHER_PLAYER_DATA_PARAM1_DATA0].ToString(), out heroId);
                return heroId;
            }
            else
            {
                return 0;
            }
        }

        public void RequestSwitchTeam(string teamName, System.Action callback)
        {
            API.RequestSwitchTeam(teamName, delegate (Hashtable result)
            {
                if (result != null)
                {
                    Hashtable resultDataHash = result[SmallPartnerPacketRule.BUDDY_INVENTORY] as Hashtable;
                    DataLookupsCache.Instance.CacheDataIgnoreNull(SmallPartnerPacketRule.BUDDY_INVENTORY, resultDataHash);
                    Hashtable resultDataHash2 = result[SmallPartnerPacketRule.USER_TEAM] as Hashtable;
                    DataLookupsCache.Instance.CacheDataIgnoreNull(SmallPartnerPacketRule.USER_TEAM, resultDataHash2);
                    callback();
                }
            });
        }
        //当前上阵的雇佣兵 包括id和位置  
        public TeamMemberData CurTeamMemberData
        {
            get
            {
                TeamMemberData data=null; 
                int hero_id;
                if (DataLookupsCache.Instance.SearchDataByID("mercenary.info.use_info.hero_id", out hero_id))
                {
                    data=new TeamMemberData();
                    data.HeroID = hero_id;
                    int pos;
                    DataLookupsCache.Instance.SearchDataByID("mercenary.info.use_info.pos", out pos);
                    data.Pos = pos;
                    int uid;
                    DataLookupsCache.Instance.SearchDataByID("mercenary.info.use_info.uid", out uid);
                    data.Uid = uid;

                    string StaId;
                    DataLookupsCache.Instance.SearchDataByID("mercenary.info.use_info.tpl_id", out StaId);
                    data.InfoID = int.Parse(StaId)-1;
                    var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(data.InfoID, 0);
                    data.ModelName = charTpl.model_name;//需添加皮肤
                    data.IsHire = true;
                }
                
                return data;
            }   
        }

        public void SetCurTeamMemberData(Action action =null)
        {
            TeamMemberData data=CurTeamMemberData; 
            if (data!=null)
            {
                int use;
                DataLookupsCache.Instance.SearchDataByID($"mercenary.info.used_uids.{data.Uid}", out use);
                if (use == 1)
                {
                    //已使用佣兵下阵 换战力最高的伙伴
                    DefaultRequestDragMaxPowerPartner(data.Pos);
                    UnUseAllianceMercenary(data.HeroID,data.Pos,null);
                    data = null;
                }
                else
                {
                    if (ContainMercenaryType())
                    {
                        //同一位置有佣兵和伙伴或者同一team有佣兵和伙伴是相同InfoID,下阵佣兵
                        List<TeamMemberData> teamList = GetCurrentTeamMemList();
                        for (int i = 0; i < teamList.Count; i++)
                        {
                            if (teamList[i].Uid <= 0)
                            {
                                if (teamList[i].Pos == data.Pos || teamList[i].InfoID == data.InfoID)
                                {
                                    UnUseAllianceMercenary(data.HeroID, data.Pos, action);
                                    data = null;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DefaultRequestDragMaxPowerPartner(int pos)
        {
            //上阵一个最高战力的伙伴
            Dictionary<int,int> dict = new Dictionary<int,int>();
            List<LTPartnerData> partnerList = LTPartnerDataManager.Instance.GetOwnPartnerList();
            List<TeamMemberData> memberDatas= GetCurrentTeamMemList();
            for (int i = 0; i < partnerList.Count; i++)
            {
                dict.Add(partnerList[i].HeroId,partnerList[i].powerData.curPower);
            }

            for (int i = 0; i < memberDatas.Count; i++)
            {
                if(dict.ContainsKey(memberDatas[i].HeroID))
                {
                    dict.Remove(memberDatas[i].HeroID);
                }
            }

            int maxHeroID=0;
            int maxPower=0;
            foreach (var VARIABLE in dict)
            {
                if (VARIABLE.Value>maxPower)
                {
                    maxPower = VARIABLE.Value;
                    maxHeroID = VARIABLE.Key;
                }
            }

            if (maxHeroID != 0)
            {
                RequestDragHeroToFormationPos(maxHeroID,pos,FormationUtil.GetCurrentTeamName(),null);
            }
        }

        public void RequestDragHeroToFormationPosWithMer(int heroId, int targetPosIndex, string teamName,
            System.Action callback)
        {
            //1
            bool oneisHire = heroId < 0;
            bool swap = false;
            int oldpos = -1;
            //2
            bool newposhavePerson = false;
            bool newposhaveMer = false;
            TeamMemberData cur = null;
            List<TeamMemberData> teamList = GetCurrentTeamMemList();
            for (int i = 0; i < teamList.Count; i++)
            {
                if (teamList[i].Pos == targetPosIndex && teamList[i].Uid <= 0)
                {
                    newposhavePerson = true;
                    cur = teamList[i];
                }

                if (teamList[i].HeroID == heroId || teamList[i].HeroID == -heroId)
                {
                    swap = true;
                    oldpos = teamList[i].Pos;
                }
            }

            if (CurTeamMemberData != null && CurTeamMemberData.Pos == targetPosIndex)
            {
                newposhaveMer = true;
                cur = CurTeamMemberData;
            }

            if (oneisHire == false)
            {
                RequestDragHeroToFormationPos(heroId, targetPosIndex, teamName, () =>
                {
                    if (newposhaveMer)
                    {
                        //TODO 雇佣兵下阵或者交换
                        if (swap)
                        {
                            UseAllianceMercenary(CurTeamMemberData.HeroID, oldpos,teamName, (ha) => { callback(); });
                        }
                        else
                        {
                            UnUseAllianceMercenary(CurTeamMemberData.HeroID, CurTeamMemberData.Pos, callback);
                        }
                    }
                    else
                    {
                        callback.Invoke();
                    }
                });
            }
            else
            {
                //TODO 替换雇佣兵 
                UseAllianceMercenary(-heroId, targetPosIndex,teamName, (ha) =>
                {
                    if (newposhavePerson)
                    {
                        //真人请求交换
                        if (swap)
                        {
                            RequestDragHeroToFormationPos(cur.HeroID, oldpos, teamName, callback);
                        }
                        //真人下阵
                        else
                        {
                            RequestRemoveHeroFormation(cur.HeroID, teamName, callback);
                        }
                    }
                    else
                    {
                      
                        callback.Invoke();
                    }
                });
            }
        }


        public void RequestDragHeroToFormationPos(int heroId, int statId,int targetPosIndex, string teamName, System.Action callback)
        {
            if (heroId<=0)
            {
                //如果是换阵 老位置移除
                int item = GetHeroBattleDataPosByStatId(statId);
                if(item!=-1)HeroBattleTempPartner.Remove(item);
                //如果新位置有人 交换
                if ( HeroBattleTempPartner.TryGetValue(targetPosIndex, out int temp))
                {
                    HeroBattleTempPartner.Remove(targetPosIndex);
                    if(item!=-1) HeroBattleTempPartner.Add(item,temp);
                }
                HeroBattleTempPartner.Add(targetPosIndex,statId);
                callback?.Invoke();
                return;
            }
            RequestDragHeroToFormationPos(heroId, targetPosIndex, teamName, callback);
        }

        

        //拖拽上阵
        public void RequestDragHeroToFormationPos(int heroId, int targetPosIndex, string teamName, System.Action callback)
        {
            API.RequestDragHeroToFormationPos(heroId, teamName, targetPosIndex, delegate (Hashtable result)
            {
                if (result != null)
                {
                    Hashtable resultDataHash = result[SmallPartnerPacketRule.BUDDY_INVENTORY] as Hashtable;
                    DataLookupsCache.Instance.CacheDataIgnoreNull(SmallPartnerPacketRule.BUDDY_INVENTORY, resultDataHash);
                    Hashtable resultDataHash2 = result[SmallPartnerPacketRule.USER_TEAM] as Hashtable;
                    DataLookupsCache.Instance.CacheDataIgnoreNull(SmallPartnerPacketRule.USER_TEAM, resultDataHash2);
                    SetFormationData(teamName);

                    if (teamName == "team1")
                    {
                        OnRefreshMainTeamPower(true);
                    }
                    else if (teamName == "lt_challenge_camp")
                    {
                        float curHp = 0;
                        if (!DataLookupsCache.Instance.SearchDataByID<float>(string.Format("{0}.{1}.HP", LTChallengeInstanceHpCtrl.curHpInfoPath, heroId), out curHp))
                        {
                            ArrayList heroes = Johny.ArrayListPool.Claim();
                            heroes.Add(LTChallengeInstanceHpCtrl.GetHeroDataById(heroId, GetIndex(heroId, false)));
                            LTInstanceMapModel.Instance.RequestChallengeGetHeroHp(heroes);
                        }
                    }
                    else if (teamName == "lt_aw_camp")
                    {
                        float curHp = 0;
                        if (!DataLookupsCache.Instance.SearchDataByID<float>(string.Format("{0}.{1}.HP", LTChallengeInstanceHpCtrl.curHpInfoPath, heroId), out curHp))
                        {
                            ArrayList heroes = Johny.ArrayListPool.Claim();
                            heroes.Add(LTChallengeInstanceHpCtrl.GetHeroDataById(heroId, GetIndex(heroId, true)));
                            LTInstanceMapModel.Instance.RequestChallengeGetHeroHp(heroes, null, LTInstanceConfig.AlienMazeTypeStr);
                        }
                    }

                    callback?.Invoke();
                }
            });
        }
        
        public void UseAllianceMercenary(int heroId,int position,string teamName,Action<Hashtable> callback)
        {
            API.UseAllianceMercenary(heroId, position, (ha) =>
            {
                DataLookupsCache.Instance.CacheData(ha);
                SetFormationData(teamName);
                callback.Invoke(ha);
            });
        }
        
        public void UnUseAllianceMercenary(int heroId,int position,Action callback)
        {
            API.UnUseAllianceMercenary(heroId, position, (ha) =>
            {
                DataLookupsCache.Instance.CacheData(ha);
                SetFormationData();
                callback?.Invoke();
            });
        }
        
        public void GetHeroInfoForView(int heroId,Action<Hashtable> callback)
        {
            API.GetHeroInfoForView(heroId, (ha) =>
            {
                // DataLookupsCache.Instance.CacheData(ha);
                callback?.Invoke(ha);
            });
        }

        public int GetIndex(int heroId, bool isAlienMaze)
        {
            ArrayList teaminfodata;
            if (DataLookupsCache.Instance.SearchDataByID<ArrayList>(SmallPartnerPacketRule.USER_TEAM + (isAlienMaze ? ".lt_aw_camp." : ".lt_challenge_camp.") + SmallPartnerPacketRule.USER_TEAM_TEAM_INFO, out teaminfodata))
            {
                for (int i = 0; i < teaminfodata.Count; i++)
                {
                    if (EB.Dot.Integer("hero_id", teaminfodata[i], 0) == heroId)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        //拖拽上阵到另一队伍
        public void RequestDragHeroToOtherTeam(int fromHeroId, int fromTargetPosIndex, string fromTeamName, int toHeroId, int toTargetPosIndex, string toTeamName, System.Action callback)
        {
            API.RequestDragHeroToOtherTeam(fromHeroId, fromTeamName, fromTargetPosIndex, toHeroId, toTeamName, toTargetPosIndex, delegate (Hashtable result)
            {
                if (result != null)
                {
                    Hashtable resultDataHash = result[SmallPartnerPacketRule.BUDDY_INVENTORY] as Hashtable;
                    DataLookupsCache.Instance.CacheDataIgnoreNull(SmallPartnerPacketRule.BUDDY_INVENTORY, resultDataHash);
                    Hashtable resultDataHash2 = result[SmallPartnerPacketRule.USER_TEAM] as Hashtable;
                    DataLookupsCache.Instance.CacheDataIgnoreNull(SmallPartnerPacketRule.USER_TEAM, resultDataHash2);
                    SetFormationData();
                    callback();
                }
            });
        }

        public void RequestRemoveHeroFormation(int heroId, int statId, string teamName, System.Action callback = null)
        {
            if (heroId<=0)
            {
                int item = GetHeroBattleDataPosByStatId(statId);
                if(item!=-1)HeroBattleTempPartner.Remove(item);
                callback?.Invoke();
                return;
            }
            RequestRemoveHeroFormation(heroId,teamName,callback);
        }

        //拖拽下阵
        public void RequestRemoveHeroFormation(int heroId, string teamName, System.Action callback = null)
        {
            API.RemoveHeroFromFormation(heroId, teamName, delegate (Hashtable result)
            {
                if (result != null)
                {
                    Hashtable resultDataHash= result[SmallPartnerPacketRule.BUDDY_INVENTORY] as Hashtable;
                    DataLookupsCache.Instance.CacheDataIgnoreNull(SmallPartnerPacketRule.BUDDY_INVENTORY, resultDataHash);
                    Hashtable resultDataHash2 = result[SmallPartnerPacketRule.USER_TEAM] as Hashtable;
                    DataLookupsCache.Instance.CacheDataIgnoreNull(SmallPartnerPacketRule.USER_TEAM, resultDataHash2);
                    SetFormationData(teamName);
                    if (teamName == "team1") OnRefreshMainTeamPower(true);
                    if (callback != null)
                    {
                        callback();
                    }
                }
            });
        }

        //请求其他玩家阵形数据
        public void RequestOtherPlayerData(long playerUid, string type, string dataType, object data, System.Action<Hashtable> callback)
        {
            API.errorProcessFun = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "ID_ERROR_NOT_IN_LEADERBOARD":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ERROR_CHALLENGE_NOT_IN_RANK"));
                                return true;
                            }
                    }
                }
                return false;
            };
            API.RequestOtherPlayerData(playerUid, type, dataType, data, delegate (Hashtable result)
            {
                callback(result);
            });
        }

        public void RequestHonorOtherPlayerData(string playerUid, object data, Action<Hashtable> callback)
        {
            API.errorProcessFun = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "ID_ERROR_CHALLENGE_NOT_IN_RANK":
                        case "not in challenge list":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_HONOR_ARENA_CHALLER_CHANGE"));
                                return true;
                            }
                    }
                }
                return false;
            };
            API.RequestHonorOtherPlayerData(playerUid, data, callback);
        }
        

        public void RequestHonorMulToFormationPos(Action<Hashtable> callback)
        {
            //0.是否需要上阵
            // bool isEmpty = IsArenaPartnerDataEmpty(HonorConfig.teamName2);
            // if (!isEmpty)
            // {
            //     return;
            // }
            //1.伙伴数据
            List<LTPartnerData> partnerList = LTPartnerDataManager.Instance.GetOwnPartnerList();
            int length = Mathf.Min(partnerList.Count, HonorConfig.ALLNUM);
            List<Hashtable> tables = new List<Hashtable>();
            int[] posArray = { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5 };
            for (int i = 0; i < length; i++)
            {
                tables.Add(GetFormationData(partnerList[i].HeroId, HonorConfig.teamName2[i % HonorConfig.teamName2.Length],
                    posArray[i]));
            }
            API.RequestMulToFormationPos(tables, (result) =>
            {
                Hashtable resultDataHash2 = result[SmallPartnerPacketRule.USER_TEAM] as Hashtable;
                DataLookupsCache.Instance.CacheDataIgnoreNull(SmallPartnerPacketRule.USER_TEAM, resultDataHash2);
                SetFormationData();
                callback(result);
            });
            
            API.RequestHonorArenaOpen((ha) => { });
        }


        public void SwapFormationPos(string form1, string form2, Action<Hashtable> callback)
        {
            TeamMemberData[] list1 = GetTeamMemArray(form1);
            TeamMemberData[] list2 = GetTeamMemArray(form2);
            List<Hashtable> arg = new List<Hashtable>();
            for (int i = 0; i < list1.Length; i++)
            {
                int fromHeroId = list1[i] != null ? list1[i].HeroID : 0;
                int toHeroId = list2[i] != null ? list2[i].HeroID : 0;
                if (fromHeroId != 0 || toHeroId != 0)
                {
                    arg.Add(GetSwapFormationData(fromHeroId, form1, i,
                        toHeroId, form2, i));
                }
            }
            API.switchHerosMulti(arg, (result) =>
            {
                if (result != null)
                {
                    Hashtable resultDataHash2 = result[SmallPartnerPacketRule.USER_TEAM] as Hashtable;
                    DataLookupsCache.Instance.CacheDataIgnoreNull(SmallPartnerPacketRule.USER_TEAM, resultDataHash2);
                    SetFormationData();
                }
                callback(result);
            });
        }


        /// <summary>
        /// 构造上阵伙伴参数
        /// </summary>
        /// <param name="heroId"></param>
        /// <param name="teamName"></param>
        /// <param name="formationPos"></param>
        /// <returns></returns>
        private Hashtable GetFormationData(int heroId, string teamName, int formationPos)
        {
            Hashtable hashtable = Johny.HashtablePool.Claim();
            hashtable.Add("heroId", heroId);
            hashtable.Add("teamName", teamName);
            hashtable.Add("formationPos", formationPos);
            return hashtable;
        }


        /// <summary>
        /// 构造交换伙伴参数
        /// </summary>
        /// <param name="heroId"></param>
        /// <param name="teamName"></param>
        /// <param name="formationPos"></param>
        /// <returns></returns>
        private Hashtable GetSwapFormationData(int fromHeroId, string fromTeamName, int fromFormationPos,
            int toHeroId, string toTeamName, int toFormationPos)
        {
            Hashtable hashtable = Johny.HashtablePool.Claim();
            hashtable.Add("fromHeroId", fromHeroId);
            hashtable.Add("fromTeamName", fromTeamName);
            hashtable.Add("fromFormationPos", fromFormationPos);

            hashtable.Add("toHeroId", toHeroId);
            hashtable.Add("toTeamName", toTeamName);
            hashtable.Add("toFormationPos", toFormationPos);
            return hashtable;
        }


        private void ClearData()
        {
            Array.Clear(ArenaPartnerData_Def, 0, ArenaPartnerData_Def.Length);
            Array.Clear(ArenaPartnerData_Attack, 0, ArenaPartnerData_Attack.Length);
            TeamMemDataDic.Clear();
            MainTeamPower = 0;
        }
    }

    public class TeamMemberData
    {
        public int HeroID;
        public int InfoID;
        public int Pos;
        public string ModelName;
        public bool IsHire;
        public long Uid;
    }

    public class OtherPlayerPartnerData
    {
        public string Name;
        public int InfoId;
        public int HeroID;
        public Hotfix_LT.Data.eRoleAttr Attr;
        public string Icon;
        public string RoleProflie;
        public string RoleProflieSprite;
        public int QualityLevel;
        public int Level;
        public int Star;
        public int Pos;
        public int UpGradeId;
        public int race;
        public int awakenLevel;
        public LTAttributesData Attributes;
        public LTAttributesData FinalAttributes;
        public int commonSkill;
        public int commonSkillLevel;
        public int activeSkill;
        public int activeSkillLevel;
        public int passiveSkill;
        public int passiveSkilLevel;

        public Hashtable equipmentList;
        public string otherPlayerName;

        //潜能等级
        public int AllRoundLevel;
        public int ControlLevel;
        public int StrongLevel;
        public int RageLevel;
        public int AbsorbedLevel;

        public int ArtifactLevel=-1;

        public float OtherPower;

        public Dictionary<string, float> equipmentAdds = new Dictionary<string, float>();
        public Dictionary<int, int> equipmentSuits = new Dictionary<int, int>();
        public float GetOtherPower()
        {
            if (OtherPower > 0)
            {
                return OtherPower;
            }

            equipmentSuits.Clear();
            equipmentAdds.Clear();

            List<int> eidList = new List<int>();
            if (equipmentList != null)
            {
                foreach (Hashtable info in equipmentList.Values)
                {
                    if (info == null || info["equipment_type"] == null)
                    {
                        continue;
                    }
                    int eid = int.Parse(info["economy_id"].ToString());
                    DetailedEquipmentInfo einfo = info.ContainsKey("currentLevel") ? 
						LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(info) :
						LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(int.Parse(info["inventory_id"].ToString()));

                    eidList.Add(int.Parse(einfo.ECid));
                    EquipmentAttr mainAdds = einfo.MainAttributes;
                    List<EquipmentAttr> exAdds = einfo.ExAttributes;
                    string mainAddsName = mainAdds.Name;
                    float mainadd = mainAdds.Value;
                    if (!equipmentAdds.ContainsKey(mainAddsName))
                    {
                        equipmentAdds.Add(mainAddsName, mainadd);
                    }
                    else
                    {
                        equipmentAdds[mainAddsName] += mainadd;
                    }

                    for (int i = 0; i < exAdds.Count; i++)
                    {
                        EquipmentAttr exadd = exAdds[i];
                        string addName = exadd.Name;
                        float add = exadd.Value;
                        if (!equipmentAdds.ContainsKey(addName))
                        {
                            equipmentAdds.Add(addName, add);
                        }
                        else
                        {
                            equipmentAdds[addName] += add;
                        }
                    }
                    Hotfix_LT.Data.EquipmentItemTemplate tpl = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetEquipment(eid);
                    if (tpl.SuitAttrId_1 != 0)
                    {
                        if (!equipmentSuits.ContainsKey(tpl.SuitAttrId_1))
                        {
                            equipmentSuits[tpl.SuitAttrId_1] = 0;
                        }
                        equipmentSuits[tpl.SuitAttrId_1] += 1;
                        GetEquipmentSuitAttr(2, tpl.SuitAttrId_1);
                    }

                    if (tpl.SuitAttrId_2 != 0)
                    {
                        if (!equipmentSuits.ContainsKey(tpl.SuitAttrId_2))
                        {
                            equipmentSuits[tpl.SuitAttrId_2] = 0;
                        }
                        equipmentSuits[tpl.SuitAttrId_2] += 1;
                        GetEquipmentSuitAttr(4, tpl.SuitAttrId_2);
                    }
                }
            }

            HandleSuitTypeRateAttrAdd();
            LTAttributesData buite = new LTAttributesData(FinalAttributes);
            foreach (string attrAdd in equipmentAdds.Keys)
            {
                float add = float.Parse(equipmentAdds[attrAdd].ToString());
                switch (attrAdd)
                {
                    case "ATK": buite.m_ATK += add; break;
                    case "MaxHP": buite.m_MaxHP += add; break;
                    case "DEF": buite.m_DEF += add; break;
                    case "CritP": buite.m_CritP += add; break;
                    case "CritV": buite.m_CritV += add; break;
                    case "SpExtra": buite.m_SpExtra += add; break;
                    case "SpRes": buite.m_SpRes += add; break;
                    case "speed": buite.m_Speed += add; break;
                    default: EB.Debug.LogWarning("Equipment MainAttribute Miss{0}", attrAdd); break;
                }
            }
            OtherPower = AttributesManager.GetOtherCombatPower(buite, new int[3] { commonSkill, activeSkill, passiveSkill },
                new int[3] { commonSkillLevel, activeSkillLevel, passiveSkilLevel }, eidList, InfoId, Star, awakenLevel);
            return OtherPower;
        }

        public void GetEquipmentSuitAttr(int NeedCount, int suitId)
        {
            Hotfix_LT.Data.SuitAttribute attr = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetSuitAttrByID(suitId);
            if (attr == null || equipmentSuits[suitId] < NeedCount)
            {
                return;
            }
            if (!equipmentAdds.ContainsKey(attr.attr))
            {
                equipmentAdds[attr.attr] = 0;
            }
            int attrCount = (int)(equipmentSuits[suitId] / NeedCount);
            equipmentAdds[attr.attr] += attr.value * attrCount;
            equipmentSuits[suitId] = 0;//套装计算一次后需要重置累计数量
        }



        private void HandleSuitTypeRateAttrAdd()
        {
            List<string> equipAddKeys = new List<string>(equipmentAdds.Keys);
            for (var i = 0; i < equipAddKeys.Count; i++)
            {
                string equipAdd = equipAddKeys[i];
                float value = equipmentAdds[equipAdd];
                switch (equipAdd)
                {
                    case "MaxHPrate":
                        if (!equipmentAdds.ContainsKey("MaxHP"))
                        {
                            equipmentAdds["MaxHP"] = 0;
                        }
                        equipmentAdds["MaxHP"] += (float)(Attributes.m_MaxHP * value);
                        break;
                    case "ATKrate":
                        if (!equipmentAdds.ContainsKey("ATK"))
                        {
                            equipmentAdds["ATK"] = 0;
                        }
                        equipmentAdds["ATK"] += (float)(Attributes.m_ATK * value);
                        break;
                    case "DEFrate":
                        if (!equipmentAdds.ContainsKey("DEF"))
                        {
                            equipmentAdds["DEF"] = 0;
                        }
                        equipmentAdds["DEF"] += (float)(Attributes.m_DEF * value);
                        break;
                    case "speedrate":
                        if (!equipmentAdds.ContainsKey("speed"))
                        {
                            equipmentAdds["speed"] = 0;
                        }
                        equipmentAdds["speed"] += (float)(Attributes.m_Speed * value);
                        break;
                }
            }
        }
    }

    public class TeamID
    {
        static public string Team1 = "team1";
        // static public string Team2 = "team2";
        // static public string Team3 = "team3";
    }

    public class LTTeamData : INodeData
    {
        public Dictionary<string, List<TeamMemberData>> TeamDic;

        public void CleanUp()
        {
        }

        public object Clone()
        {
            return new AllianceBattleMembers();
        }

        public LTTeamData()
        {
            TeamDic = new Dictionary<string, List<TeamMemberData>>();
        }

        public void OnUpdate(object obj)
        {
            Hashtable userTeamData = obj as Hashtable;
            if (userTeamData != null)
            {
                if (userTeamData.Contains(TeamID.Team1))
                {
                    TeamDic[TeamID.Team1] = GetOneTeamMemList(userTeamData, TeamID.Team1);
                }
            }
        }

        public void OnMerge(object obj)
        {

        }

        public List<TeamMemberData> GetOneTeamMemList(Hashtable userTeamData, string teamID)
        {
            List<TeamMemberData> oneTeamMemDataList = new List<TeamMemberData>();
            ArrayList teamHash = Hotfix_LT.EBCore.Dot.Array(teamID + ".formation_info", userTeamData, null);
            //DataLookupsCache.Instance.SearchDataByID<ArrayList>(SmallPartnerPacketRule.USER_TEAM + "." + _strTeam + "." + SmallPartnerPacketRule.USER_TEAM_FORMATION, out teamHash);
            int nPos = -1;
            Hashtable partnerServerData = null;

            if (teamHash != null)
            {
                for (var i = 0; i < teamHash.Count; i++)
                {
                    var teamMemData = teamHash[i];
                    nPos++;
                    IDictionary teamMemDataDic = teamMemData as IDictionary;
                    if (teamMemDataDic == null || !teamMemDataDic.Contains(SmallPartnerPacketRule.USER_TEAM_FORMATION_HERO_ID) || teamMemDataDic[SmallPartnerPacketRule.USER_TEAM_FORMATION_HERO_ID] == null)
                        continue;
                    int nHeroID = EB.Dot.Integer(SmallPartnerPacketRule.USER_TEAM_FORMATION_HERO_ID, teamMemDataDic, 0);
                    if (nHeroID <= 0)
                    {
                        EB.Debug.LogError("userTeam data heroID<=0");
                        continue;
                    }
                    DataLookupsCache.Instance.SearchDataByID<Hashtable>(SmallPartnerPacketRule.OWN_HERO_STATS + "." + nHeroID, out partnerServerData);
                    if (partnerServerData == null)
                    {
                        EB.Debug.LogError("cannot find heroStat data for heroID={0}", nHeroID);
                        continue;
                    }
                    int nStatsID = EB.Dot.Integer(SmallPartnerPacketRule.HERO_TEMPLATE_ID, partnerServerData, -1);
                    var tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(nStatsID);
                    int nInfoID = tpl.character_id;
                    var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(nInfoID, EB.Dot.Integer(SmallPartnerPacketRule.HERO_SKIN, teamMemDataDic, 0));

                    TeamMemberData teamMemDataNew = new TeamMemberData();
                    teamMemDataNew.HeroID = nHeroID;
                    teamMemDataNew.InfoID = nInfoID;
                    teamMemDataNew.ModelName = charTpl.model_name;//需添加皮肤
                    teamMemDataNew.Pos = nPos;
                    oneTeamMemDataList.Add(teamMemDataNew);
                }
            }
            return oneTeamMemDataList;
        }
      

    }
}