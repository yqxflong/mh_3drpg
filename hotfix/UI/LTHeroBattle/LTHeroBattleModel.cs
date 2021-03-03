using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{

    /// <summary>
    /// 战斗需要的数据
    /// </summary>
    public class LTHeroBattleModel
    {
        private static LTHeroBattleModel _instance;
        public static LTHeroBattleModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new LTHeroBattleModel();
            }
            return _instance;
        }
        /// <summary>
        /// 选择的内容数据
        /// </summary>
        public HeroBattleChoiceData choiceData;
        /// <summary>
        /// 英雄交锋 匹配前数据
        /// </summary>
        public HeroBattleMatchData matchData;
        /// <summary> 是否在等待匹配 </summary>
        public bool isWaitingMatch;
        /// <summary> 是否已经打开交锋界面</summary>
        public bool isOpenBattleHud;


        private LTHeroBattleModel()
        {
            choiceData = new HeroBattleChoiceData();
            matchData = new HeroBattleMatchData();
        }
        /// <summary>
        /// 伙伴相应的品质对应相应的花费值
        /// </summary>
        /// <param name="ssrType"></param>
        /// <returns></returns>
        public static int GetSpend(int ssrType)
        {
            int spend = 0;
            switch (ssrType)
            {
                case 2:
                    spend = 0;
                    break;
                case 3:
                    spend = 2;
                    break;
                case 4:
                    spend = 4;
                    break;
            }
            return spend;
        }

        /// <summary>
        /// 是否免费开放英雄 （包含限免和基础可用）
        /// </summary>
        /// <param name="tmpID"></param>
        /// <returns></returns>
        public bool IsFreeHero(int tmpID)
        {
            for (int i = 0; i < matchData.listLimitFreeHero.Count; i++)
            {
                if (matchData.listLimitFreeHero[i] == tmpID)
                {
                    return true;
                }
            }

            for (int i = 0; i < matchData.listBaseHero.Count; i++)
            {
                if (matchData.listBaseHero[i] == tmpID)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary> 获取首个可选择的英雄 </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public HeroBattleChoiceCellData GetFirstChoiceHero(Hotfix_LT.Data.eRoleAttr type)
        {
            if (!choiceData.dicHeroChoiceData.ContainsKey(type))
            {
                EB.Debug.LogError("HeroBattleModel no Init or dont find type ={0}" , type);
                return null;
            }

            List<HeroBattleChoiceCellData> list = choiceData.dicHeroChoiceData[type];

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsCanChoiceUse())
                {
                    return list[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 每回合选英雄前 收到数据添加使用英雄后调用
        /// </summary>
        public void RefreshChoiceState()
        {
            choiceData.choiceSuitIndex = 0; //默认选择英雄第一套

            if (choiceData.openUid == choiceData.selfInfo.uid)
            {
                if (choiceData.choiceType == 0)
                {
                    choiceData.choiceType = Hotfix_LT.Data.eRoleAttr.Shui;
                }
                //HeroBattleChoiceCellData choice = GetFirstChoiceHero(choiceData.choiceType);

                //if (choice == null)
                {
                    choiceData.choiceHeroCellData = null;
                    choiceData.choiceHeroTplID = -1;
                }
                //else
                //{
                //    choiceData.choiceHeroCellData = choice;
                //    choiceData.choiceHeroTplID = choice.heroTplID;
                //}
            }
            else
            {
                //回合结束仍然需要展示使用的模型
                HeroBattleChoiceCellData lastUsed = null;
                if (choiceData.selfChoices.Count > 0) //优先显示最近选用的
                {
                    lastUsed = choiceData.selfChoices[choiceData.selfChoices.Count - 1];
                }
                else if (choiceData.selfBans.Count > 0) //次级显示最近禁用的
                {
                    lastUsed = choiceData.selfBans[choiceData.selfBans.Count - 1];
                }

                if (lastUsed == null)
                {
                    choiceData.choiceHeroCellData = null;
                    choiceData.choiceHeroTplID = -1;
                }
                else
                {
                    choiceData.choiceHeroCellData = lastUsed;
                    choiceData.choiceHeroTplID = lastUsed.heroTplID;
                }
            }

            choiceData.Refresh();

            if (LTHeroBattleEvent.NotifyRefreshChoiceState != null)
            {
                LTHeroBattleEvent.NotifyRefreshChoiceState(choiceData);
            }
        }

        public void SetMatchBase(LTHeroBattleAPI.HeroBattleMatchBase data)
        {
            matchData.isGetReward = data.IsGetReward;
            matchData.victoryNum = data.VictoryCount;
            matchData.defeatNum = data.DefeatCount;
            matchData.defeatMax = (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("heroBattleDefeatMaxCount");
            matchData.victoryMax = (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("heroBattleVictoryMaxCount");
            matchData.listLimitFreeHero.Clear();
            matchData.listLimitFreeCell.Clear();
            matchData.listBaseHero.Clear();
            matchData.listBaseHeroCell.Clear();
            for (int i = 0; i < data.FreeHeros.Count; i++)
            {
                int tplID = int.Parse((string)data.FreeHeros[i]);
                matchData.listLimitFreeHero.Add(tplID);
                Hotfix_LT.Data.HeroStatTemplate heroStat = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(tplID);
                if (heroStat != null)
                {
                    Hotfix_LT.Data.HeroInfoTemplate heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(heroStat.character_id);
                    HeroBattleChoiceCellData choiceCell = new HeroBattleChoiceCellData(tplID, heroInfo, 0);
                    matchData.listLimitFreeCell.Add(choiceCell);
                }
                else
                {
                    EB.Debug.LogError("SetMatchBase.FreeHeros:heroStat is null tplID={0}" , tplID);
                }
            }

            for (int i = 0; i < data.BaseHeroes.Count; i++)
            {
                int tplID = int.Parse((string)data.BaseHeroes[i]);
                matchData.listBaseHero.Add(tplID);
                Hotfix_LT.Data.HeroStatTemplate heroStat = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(tplID);
                if (heroStat != null)
                {
                    Hotfix_LT.Data.HeroInfoTemplate heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(heroStat.character_id);
                    HeroBattleChoiceCellData choiceCell = new HeroBattleChoiceCellData(tplID, heroInfo, 0);
                    matchData.listBaseHeroCell.Add(choiceCell);
                }
                else
                {
                    EB.Debug.LogError("SetMatchBase.BaseHeroes:heroStat is null tplID={0}" , tplID);
                }
            }

            choiceData.AddHeroPool();

            // if (LTHeroBattleEvent.NotifyRefreshMatchData != null)
            // {
            //     LTHeroBattleEvent.NotifyRefreshMatchData(matchData);
            // }
        }

        /// <summary>
        /// 设置选中数据
        /// </summary>
        /// <param name="data">数据</param>
        public void SetChoiceData(LTHeroBattleAPI.HeroBattleData data)
        {
            /*
           EB.Debug.LogError("[" + Time.frameCount + "]服务器已经下发数据：当前的操作行为:Status:" + data.Status 
                + ",是否为玩家:" + (data.OpenUid == LoginManager.Instance.LocalUserId.Value));
                */
            choiceData.choiceState = data.Status.Equals("banhero") ? 0 : data.Status.Equals("selecthero") ? 1 : 2;
            choiceData.openUid = data.OpenUid;
            choiceData.lessTime = EB.Time.After(data.NextTS);

            for (int i = 0; i < data.Sides.Length; i++)
            {
                if (data.Sides[i].Uid ==LoginManager.Instance.LocalUserId.Value) //若是玩家自己
                {
                    choiceData.selfInfo.name = data.Sides[i].Name;
                    choiceData.selfInfo.uid = data.Sides[i].Uid;
                    choiceData.selfInfo.level = data.Sides[i].Level;
                    choiceData.selfInfo.portrait = data.Sides[i].Portrait;
                    choiceData.selfInfo.frame = data.Sides[i].Frame;
                    choiceData.selfInfo.isSelf = true;
                    choiceData.selfBans.Clear();
                    for (int j = 0; j < data.Sides[i].BandTplIds.Length; j++)
                    {
                        int tplID = 0;
                        if (!int.TryParse(data.Sides[i].BandTplIds[j], out tplID))
                        {
                            continue;
                        }

                        HeroBattleChoiceCellData cell = choiceData.selfChoices.Find(p => p.heroTplID == tplID);
                        if (cell != null)
                        {
                            choiceData.selfBans.Add(cell);
                        }
                        else
                        {
                           EB.Debug.LogError("choiceData.selfBans cell is null tplID={0}" , tplID);
                        }
                    }
                    choiceData.selfChoices.Clear();
                    for (int j = 0; j < data.Sides[i].SelectHeroIds.Count; j++)
                    {
                        if (data.Sides[i].SelectHeroIds[j] != null)
                        {
                            int id = EB.Dot.Integer("id", data.Sides[i].SelectHeroIds[j], 0);
                            int tplID = EB.Dot.Integer("tplId", data.Sides[i].SelectHeroIds[j], 0);
                            int star = EB.Dot.Integer("star", data.Sides[i].SelectHeroIds[j], 0);
                            int level = EB.Dot.Integer("level", data.Sides[i].SelectHeroIds[j], 0);
                            int peak = EB.Dot.Integer("proficiency.1.1", data.Sides[i].SelectHeroIds[j], 0);
                            int upgrade = EB.Dot.Integer("upgrade", data.Sides[i].SelectHeroIds[j], 0);
                            int charType = EB.Dot.Integer("charType", data.Sides[i].SelectHeroIds[j], -1);
                            string roleGrade = EB.Dot.String("roleGrade", data.Sides[i].SelectHeroIds[j], "");
                            int awakenLevel = EB.Dot.Integer("awaken", data.Sides[i].SelectHeroIds[j], 0);
                            int skin = EB.Dot.Integer("skin", data.Sides[i].SelectHeroIds[j], 0);
                            int artifactLevel = EB.Dot.Integer("artifact_equip.enhancement_level", data.Sides[i].SelectHeroIds[j], -1);
                            HeroBattleChoiceCellData cell = GetHeroCell(tplID);
                            if (cell != null)
                            {
                                cell.star = star;
                                cell.type = (Hotfix_LT.Data.eRoleAttr)charType;
                                cell.upGrade = upgrade;
                                cell.level = level;
                                cell.peak = peak;
                                cell.artifactLevel = artifactLevel;
                                cell.skin = skin;
                                cell.modelName = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(tplID - 1, skin).model_name;
                                cell.isAwake = awakenLevel;
                                choiceData.selfChoices.Add(cell);
                            }
                            else
                            {
                               EB.Debug.LogError("choiceData.SelectHero cell is null tplID={0}" , tplID);
                            }
                        }
                    }
                }
                else
                {
                    choiceData.otherInfo.name = data.Sides[i].Name;
                    choiceData.otherInfo.uid = data.Sides[i].Uid;
                    //机器人情况下，强行修改和我的分段一样
                    //if (data.Sides[i].Uid == 0)
                    //{
                    //    choiceData.otherInfo.level = choiceData.selfInfo.level;
                    //    choiceData.otherInfo.score = LadderManager.Instance.Info.Point;
                    //}
                    //else
                    {
                        choiceData.otherInfo.level = data.Sides[i].Level;
                    }

                    choiceData.otherInfo.portrait = data.Sides[i].Portrait;
                    choiceData.otherInfo.frame = data.Sides[i].Frame;
                    choiceData.otherInfo.isSelf = false;
                    choiceData.otherBans.Clear();
                    for (int j = 0; j < data.Sides[i].BandTplIds.Length; j++)
                    {
                        int tplID = 0;
                        if (!int.TryParse(data.Sides[i].BandTplIds[j], out tplID))
                        {
                            continue;
                        }
                        HeroBattleChoiceCellData cell = choiceData.otherChoices.Find(p => p.heroTplID == tplID);
                        if (cell != null)
                        {
                            choiceData.otherBans.Add(cell);
                        }
                        else
                        {
                           EB.Debug.LogError("choiceData.otherBans cell is null tplID={0}" , tplID);
                        }
                    }
                    choiceData.otherChoices.Clear();
                    for (int j = 0; j < data.Sides[i].SelectHeroIds.Count; j++)
                    {
                        if (data.Sides[i].SelectHeroIds[j] != null)
                        {
                            int id = EB.Dot.Integer("id", data.Sides[i].SelectHeroIds[j], 0);
                            int tplID = EB.Dot.Integer("tplId", data.Sides[i].SelectHeroIds[j], 0);
                            int star = EB.Dot.Integer("star", data.Sides[i].SelectHeroIds[j], 0);
                            int level = EB.Dot.Integer("level", data.Sides[i].SelectHeroIds[j], 0);
                            int peak = EB.Dot.Integer("proficiency.1.1", data.Sides[i].SelectHeroIds[j], 0);
                            int upgrade = EB.Dot.Integer("upgrade", data.Sides[i].SelectHeroIds[j], 0);
                            int charType = EB.Dot.Integer("charType", data.Sides[i].SelectHeroIds[j], -1);
                            string roleGrade = EB.Dot.String("roleGrade", data.Sides[i].SelectHeroIds[j], "");
                            int awakenLevel = EB.Dot.Integer("awaken", data.Sides[i].SelectHeroIds[j], 0);
                            int skin = EB.Dot.Integer("skin", data.Sides[i].SelectHeroIds[j], 0);
                            HeroBattleChoiceCellData cell = null;
                            //判断当前对手是否为机器人
                            if (data.Sides[i].Uid == 0)
                            {
                                cell = GetMonsterInfo(tplID, skin);
                            }
                            else
                            {
                                cell = GetHeroCellCommon(tplID);
                            }

                            if (cell != null)
                            {
                                cell.star = star;
                                cell.type = (Hotfix_LT.Data.eRoleAttr)charType;
                                cell.upGrade = upgrade;
                                cell.level = level;
                                cell.peak = peak;
                                cell.skin = skin;
                                if (data.Sides[i].Uid != 0)
                                {
                                    cell.modelName = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(cell.heroTplID - 1, skin).model_name;
                                }
                                cell.isAwake = awakenLevel;
                                choiceData.otherChoices.Add(cell);
                            }
                            else
                            {
                               EB.Debug.LogError("choiceData.SelectHero cell is null tplID={0}" , tplID);
                            }
                        }
                    }
                }

            }
            if (data.Status.Equals("battle"))
            {
                //筛选出自己拥有的英雄
                List<LTPartnerData> generalPL = LTPartnerDataManager.Instance.GetOwnPartnerList();
                //开战前存储这次的上场的英雄，好给结算界面调用
                string teamName = FormationUtil.GetCurrentTeamName(eBattleType.LadderBattle);
                string teamDataPath = SmallPartnerPacketRule.USER_TEAM + "." + teamName + ".team_info";
                ArrayList heroIds = Johny.ArrayListPool.Claim();
                for (int i = 0; i < choiceData.selfChoices.Count; i++)
                {
                    if (choiceData.selfBans.Find(p => p.heroTplID == choiceData.selfChoices[i].heroTplID) == null)
                    {
                        LTPartnerData ltParnerData = generalPL.Find(p => p.HeroStat.id == choiceData.selfChoices[i].heroTplID);
                        heroIds.Add(ltParnerData.HeroId);
                    }
                }
                DataLookupsCache.Instance.CacheData(teamDataPath, heroIds);

                //
                if (LTHeroBattleEvent.NotifyHeroBattleHudFinish != null)
                {
                    LTHeroBattleEvent.NotifyHeroBattleHudFinish();
                }
            }
            else if (choiceData.selfChoices.Count > 0 || choiceData.otherChoices.Count > 0) //当有一边选人后延迟场景跳转 放置跳转前没有倒计时问题
            {
                if (LTHeroBattleEvent.NotifyHeroBattleDelayToScene != null)
                {
                    LTHeroBattleEvent.NotifyHeroBattleDelayToScene();
                }
            }

            RefreshChoiceState();
        }

        private HeroBattleChoiceCellData GetHeroCell(int heroTplID)
        {
            HeroBattleChoiceCellData choiceCell = null;
            foreach (var v in choiceData.dicHeroChoiceData)
            {
                for (int i = 0; i < v.Value.Count; i++)
                {
                    if (v.Value[i].heroTplID == heroTplID)
                    {
                        choiceCell = v.Value[i];
                        break;
                    }
                }
                if (choiceCell != null)
                {
                    break;
                }
            }
            return choiceCell;
        }

        /// <summary>
        /// 通过英雄伙伴id获取物怪的信息
        /// </summary>
        /// <param name="heroTplID">英雄伙伴id</param>
        /// <returns></returns>
        private HeroBattleChoiceCellData GetMonsterInfo(int heroTplID, int curSkin)
        {
            Hotfix_LT.Data.MonsterInfoTemplate heroStat = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMonsterInfo(heroTplID);
            if (heroStat != null)
            {
                Hotfix_LT.Data.HeroInfoTemplate heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(heroStat.character_id, curSkin);
                HeroBattleChoiceCellData choiceCell = new HeroBattleChoiceCellData(heroTplID, heroInfo, 0);
                return choiceCell;
            }
            else
            {
                EB.Debug.LogError("SetMatchBase:heroStat is null tplID={0}" , heroTplID);
                return null;
            }
        }

        private HeroBattleChoiceCellData GetHeroCellCommon(int heroTplID)
        {
            Hotfix_LT.Data.HeroStatTemplate heroStat = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(heroTplID);
            if (heroStat != null)
            {
                Hotfix_LT.Data.HeroInfoTemplate heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(heroStat.character_id);
                LTPartnerData data = LTPartnerDataManager.Instance.GetPartnerByStatId(heroTplID);
                HeroBattleChoiceCellData choiceCell = new HeroBattleChoiceCellData(heroTplID, heroInfo, 0);
                int curskin = 0;
                if (data != null)
                {
                    curskin = data.CurSkin;
                    choiceCell.modelName = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(data.InfoId, curskin).model_name;
                }
                return choiceCell;
            }
            else
            {
                EB.Debug.LogError("SetMatchBase:heroStat is null tplID={0}" , heroTplID);
                return null;
            }
        }

        public void SetChoiceHero(int heroTplID)
        {
            choiceData.choiceHeroTplID = heroTplID;
            HeroBattleChoiceCellData data = null;
            //禁用情况的判断
            if (LTHeroBattleModel.GetInstance().choiceData.choiceState == 0 && LTHeroBattleModel.GetInstance().choiceData.otherInfo.uid == 0)
            {
                data = GetMonsterInfo(heroTplID, 0);
            }
            else if (LTHeroBattleModel.GetInstance().choiceData.choiceState == 0 && LTHeroBattleModel.GetInstance().choiceData.otherInfo.uid != 0)
            {
                List<HeroBattleChoiceCellData> templist = LTHeroBattleModel.GetInstance().choiceData.otherChoices;
                for (int i = 0; i < templist.Count; i++)
                {
                    if (templist[i].heroTplID == heroTplID)
                    {
                        data = templist[i];
                        break;
                    }
                }
                if (data == null)
                {
                    data = GetHeroCellCommon(heroTplID);
                }
            }
            else
            {
                data = GetHeroCellCommon(heroTplID);
            }
            choiceData.choiceHeroCellData = data;

            // if (LTHeroBattleEvent.NotifyChangeChoiceHeroTplID != null)
            // {
            //     LTHeroBattleEvent.NotifyChangeChoiceHeroTplID(heroTplID);
            // }
            if (LTHeroBattleEvent.NotifyChangeChoiceHero != null)
            {
                LTHeroBattleEvent.NotifyChangeChoiceHero(choiceData.choiceHeroCellData);
            }
        }

        public void SetChoiceHeroType(Hotfix_LT.Data.eRoleAttr type)
        {
            choiceData.choiceType = type;
            // if (LTHeroBattleEvent.NotifyChoiceHeroType != null)
            // {
            //     LTHeroBattleEvent.NotifyChoiceHeroType(type);
            // }
        }

        static public List<HeroBattleChoiceCellData> GetHeroListByAttr(Hotfix_LT.Data.eRoleAttr type)
        {
            List<HeroBattleChoiceCellData> listCellData = new List<HeroBattleChoiceCellData>();
            Hotfix_LT.Data.HeroStatTemplate[] allPartnerCollection = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStats();
            for (var i = 0; i < allPartnerCollection.Length; i++)
            {
                var heroTpl = allPartnerCollection[i];
                Hotfix_LT.Data.HeroInfoTemplate heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(heroTpl.character_id);
                if (heroInfo.char_type == type && heroInfo.isShowInClash)
                {
                    HeroBattleChoiceCellData choiceCell = new HeroBattleChoiceCellData(heroTpl.id, heroInfo, 0);
                    listCellData.Add(choiceCell);
                }
            }
            return listCellData;
        }

        public void F_ClearData()
        {
            _instance = null;
        }
    }

    /// <summary>
    /// 英雄交锋 匹配前数据
    /// </summary>
    public class HeroBattleMatchData
    {
        /// <summary> 胜利场次数</summary>
        public int victoryNum;
        /// <summary> 胜利胜场总次数</summary>
        public int victoryMax = 5;
        /// <summary> 失败数</summary>
        public int defeatNum;
        /// <summary> 失败允许次数</summary>
        public int defeatMax = 3;
        /// <summary> 限免英雄列表 </summary>
        public List<int> listLimitFreeHero;
        /// <summary> 永免英雄列表 </summary>
        public List<int> listBaseHero;
        /// <summary> 是否已领取过奖励</summary>
        public bool isGetReward;
        /// <summary> 是否在等待</summary>
        public bool isWaiting;

        /// <summary> 限免</summary>
        public List<HeroBattleChoiceCellData> listLimitFreeCell;

        /// <summary> 永免英雄列表</summary>
        public List<HeroBattleChoiceCellData> listBaseHeroCell;

        /// <summary> 奖励总表</summary>
        public List<HeroBattleLevelReward> heroBattleRewards;

        public HeroBattleMatchData()
        {
            listLimitFreeHero = new List<int>();
            listLimitFreeCell = new List<HeroBattleChoiceCellData>();
            listBaseHero = new List<int>();
            listBaseHeroCell = new List<HeroBattleChoiceCellData>();

            heroBattleRewards = new List<HeroBattleLevelReward>();
            List<Hotfix_LT.Data.ClashOfHeroesRewardTemplate> tpls = Hotfix_LT.Data.EventTemplateManager.Instance.GetClashOfHeroesRewardTpls();

            for (int i = 0; i < tpls.Count; i++)
            {
                HeroBattleLevelReward reward = new HeroBattleLevelReward();
                reward.id = tpls[i].id;

                LTShowItemData data1 = new LTShowItemData(tpls[i].item_id1, tpls[i].item_num1, tpls[i].item_type1);
                LTShowItemData data2 = new LTShowItemData(tpls[i].item_id2, tpls[i].item_num2, tpls[i].item_type2);
                LTShowItemData data3 = new LTShowItemData(tpls[i].item_id3, tpls[i].item_num3, tpls[i].item_type3);
                LTShowItemData data4 = new LTShowItemData(tpls[i].item_id4, tpls[i].item_num4, tpls[i].item_type4);

                if (!string.IsNullOrEmpty(data1.id))
                {
                    reward.listShowItemData.Add(data1);
                }
                if (!string.IsNullOrEmpty(data2.id))
                {
                    reward.listShowItemData.Add(data2);
                }
                if (!string.IsNullOrEmpty(data3.id))
                {
                    reward.listShowItemData.Add(data3);
                }
                if (!string.IsNullOrEmpty(data4.id))
                {
                    reward.listShowItemData.Add(data4);
                }

                heroBattleRewards.Add(reward);
            }
        }
    }

    public class SidePlayerInfoData
    {
        public bool isSelf;
        public string name;
        public int uid;
        public int level;
        public string portrait;
        public string frame;
        /// <summary>
        /// 天梯段位分会
        /// </summary>
        public int score;
    }

    /// <summary>
    /// 选择的内容数据
    /// </summary>
    public class HeroBattleChoiceData
    {
        /// <summary> 该回合的用户id</summary>
        public int openUid;

        /// <summary> 选择英雄状态 0禁用英雄 1 选英雄</summary>
        public int choiceState = 2;
        /// <summary>玩家自己的英雄选择总表 按照类型分页 </summary>
        public Dictionary<Hotfix_LT.Data.eRoleAttr, List<HeroBattleChoiceCellData>> dicHeroChoiceData;

        /// <summary>选择类型 水 火 风 等</summary>
        public Hotfix_LT.Data.eRoleAttr choiceType;

        /// <summary> 选择英雄 tplID</summary>
        public int choiceHeroTplID;

        /// <summary> 选择第几套装</summary>
        public int choiceSuitIndex;

        public HeroBattleChoiceCellData choiceHeroCellData;

        public int lessPoint;
        public int otherLessPoint;


        /// <summary>
        /// 剩余时间
        /// </summary>
        public int lessTime;

        public SidePlayerInfoData selfInfo;
        public SidePlayerInfoData otherInfo;

        public List<HeroBattleChoiceCellData> selfBans;
        public List<HeroBattleChoiceCellData> otherBans;
        public List<HeroBattleChoiceCellData> selfChoices;
        public List<HeroBattleChoiceCellData> otherChoices;

        public HeroBattleChoiceData()
        {
            selfInfo = new SidePlayerInfoData();
            otherInfo = new SidePlayerInfoData();
            selfChoices = new List<HeroBattleChoiceCellData>();
            otherChoices = new List<HeroBattleChoiceCellData>();
            selfBans = new List<HeroBattleChoiceCellData>();
            otherBans = new List<HeroBattleChoiceCellData>();
            dicHeroChoiceData = new Dictionary<Hotfix_LT.Data.eRoleAttr, List<HeroBattleChoiceCellData>>();
            dicHeroChoiceData.Add(Hotfix_LT.Data.eRoleAttr.Shui, new List<HeroBattleChoiceCellData>());
            dicHeroChoiceData.Add(Hotfix_LT.Data.eRoleAttr.Huo, new List<HeroBattleChoiceCellData>());
            dicHeroChoiceData.Add(Hotfix_LT.Data.eRoleAttr.Feng, new List<HeroBattleChoiceCellData>());
            InitAllHero();
        }

        /// <summary>
        /// 初始化所有英雄数据
        /// </summary>
        public void InitAllHero()
        {
            foreach (var list in dicHeroChoiceData)
            {
                list.Value.Clear();
            }
            //筛选出自己拥有的英雄
            List<LTPartnerData> generalPL = LTPartnerDataManager.Instance.GetOwnPartnerList();
            //直接使用自己拥有的伙伴列表
            //Dictionary<int, Hotfix_LT.Data.HeroInfoTemplate> AllHBHero = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetAllHBHeroInfoTemplate();
            for(int i = 0; i < generalPL.Count; i++)
            {
                Data.HeroInfoTemplate hero = generalPL[i].HeroInfo;
                HeroBattleChoiceCellData cellData = new HeroBattleChoiceCellData(hero.id + 1, hero, 0);
                LTPartnerData ltPartnerData = generalPL[i];
                if (dicHeroChoiceData.ContainsKey(cellData.type) && ltPartnerData != null)
                {
                    cellData.isUnLock = true;
                    cellData.level = ltPartnerData.Level;
                    cellData.peak = ltPartnerData.AllRoundLevel;
                    cellData.artifactLevel = ltPartnerData.ArtifactLevel;
                    cellData.star = ltPartnerData.Star;
                    cellData.skin = ltPartnerData.CurSkin;
                    cellData.isAwake = ltPartnerData.IsAwaken;
                    cellData.modelName = ltPartnerData.HeroInfo.model_name;
                    cellData.iconName = ltPartnerData.HeroInfo.icon;
                    dicHeroChoiceData[cellData.type].Add(cellData);
                }
            }
        }

        /// <summary>
        /// 刷新全部需要的数据(通过简单数据后使用)
        /// </summary>
        public void Refresh()
        {
            lessPoint = 10;
            for (int i = 0; i < selfChoices.Count; i++)
            {
                lessPoint -= selfChoices[i].choiceSpend;
            }

            //foreach (var baseCellData in LTHeroBattleModel.GetInstance().matchData.listBaseHeroCell)
            //{
            //	if (FindChoicePoolData(baseCellData.heroTplID) == null)
            //	{
            //		dicHeroChoiceData[baseCellData.type].Add(baseCellData);
            //	}
            //}

            //foreach (var freeCellData in LTHeroBattleModel.GetInstance().matchData.listLimitFreeCell)
            //{			
            //	if (FindChoicePoolData(freeCellData.heroTplID)==null)
            //	{
            //		dicHeroChoiceData[freeCellData.type].Add(freeCellData);
            //	}
            //}

            foreach (var type in dicHeroChoiceData.Keys)
            {
                List<HeroBattleChoiceCellData> list = dicHeroChoiceData[type];
                for (int j = 0; j < list.Count; j++)
                {
                    list[j].isBan = false;
                    list[j].isUsed = false;
                    list[j].isFree = LTHeroBattleModel.GetInstance().IsFreeHero(list[j].heroTplID);
                    list[j].isSpend = list[j].choiceSpend <= lessPoint;
                    //list[j].isUnLock = generalPL[i].HeroId > 0;
                }
            }
            //List<LTPartnerData> generalPL = LTPartnerDataManager.Instance.GetGeneralPartnerList();
            //for (int i = 0; i < generalPL.Count; i++)
            //{
            //    Hotfix_LT.Data.eRoleAttr type = generalPL[i].HeroInfo.char_type;
            //    if (!dicHeroChoiceData.ContainsKey(type))
            //    {
            //        EB.Debug.LogError("Dont find Hotfix_LT.Data.eRoleAttr " + type + " or Not InitAllHero");
            //        break;
            //    }

            //    List<HeroBattleChoiceCellData> list = dicHeroChoiceData[type];

            //    for (int j = 0; j < list.Count; j++)
            //    {
            //        if (list[j].heroTplID == generalPL[i].StatId)
            //        {
            //            list[j].isBan = false;
            //            list[j].isUsed = false;
            //            list[j].isFree = LTHeroBattleModel.GetInstance().IsFreeHero(list[j].heroTplID);
            //            list[j].isSpend = list[j].choiceSpend <= lessPoint;
            //            list[j].isUnLock = generalPL[i].HeroId > 0;
            //            break;
            //        }
            //    }
            //}

            lessPoint = 10;
            for (int i = 0; i < selfChoices.Count; i++)
            {
                selfChoices[i].isUsed = true;
                lessPoint -= selfChoices[i].choiceSpend;
            }

            otherLessPoint = 10;
            for (int i = 0; i < otherChoices.Count; i++)
            {
                otherChoices[i].isUsed = true;
                otherLessPoint -= otherChoices[i].choiceSpend;
            }


            for (int i = 0; i < selfBans.Count; i++)
            {
                selfBans[i].isBan = true;
                var choicePoolData = FindChoicePoolData(selfBans[i].heroTplID);
                if (choicePoolData != null)
                {
                    choicePoolData.isBan = true;
                }
            }

            for (int i = 0; i < otherBans.Count; i++)
            {
                otherBans[i].isBan = true;
                var choicePoolData = FindChoicePoolData(otherBans[i].heroTplID);
                if (choicePoolData != null)
                {
                    choicePoolData.isBan = true;
                }
            }
        }

        public void AddHeroPool()
        {
            /*
            foreach (var baseCellData in LTHeroBattleModel.GetInstance().matchData.listBaseHeroCell)
            {
                if (FindChoicePoolData(baseCellData.heroTplID) == null)
                {
                    dicHeroChoiceData[baseCellData.type].Add(baseCellData);
                }
            }

            foreach (var freeCellData in LTHeroBattleModel.GetInstance().matchData.listLimitFreeCell)
            {
                if (FindChoicePoolData(freeCellData.heroTplID) == null)
                {
                    dicHeroChoiceData[freeCellData.type].Add(freeCellData);
                }
            }*/
        }

        HeroBattleChoiceCellData FindChoicePoolData(int tplId)
        {
            foreach (var type in dicHeroChoiceData.Keys)
            {
                List<HeroBattleChoiceCellData> list = dicHeroChoiceData[type];
                var findData = list.Find(hero => hero.heroTplID == tplId);
                if (findData != null)
                {
                    return findData;
                }
            }
            return null;
        }
    }


    public class HeroBattleChoiceCellData
    {
        public HeroBattleChoiceCellData(int heroTplID, Hotfix_LT.Data.HeroInfoTemplate Tpl, int awakeLevel)
        {

            //LTPartnerData data = LTPartnerDataManager.Instance.GetPartnerByStatId(heroTplID);
            //if (data != null) {
            //    Tpl = data.HeroInfo;
            //   EB.Debug.Log("HeroBattleChoiceCellData awaken:" + Tpl.model_name);
            //    this.isAwake = data.IsAwaken;
            //}

            this.heroTplID = heroTplID;
            this.iconName = Tpl.icon;
            this.modelName = Tpl.model_name;
            this.heroName = Tpl.name;
            this.type = Tpl.char_type;
            this.ssrType = Tpl.role_grade;
            this.quality = 6; //默认满品质
            this.choiceSpend = LTHeroBattleModel.GetSpend(this.ssrType);
            this.isUnLock = false;
            this.isFree = true;
            this.isAwake = awakeLevel;

        }

        /// <summary> 英雄标识 </summary>
        public int heroTplID;
        /// <summary> 是否已解锁 </summary>
        public bool isUnLock;
        /// <summary> 是否免费 </summary>
        public bool isFree;
        /// <summary> 是否已经使用 </summary>
        public bool isUsed;
        /// <summary> 是否禁止</summary>
        public bool isBan;
        /// <summary> 是否够开销</summary>
        public bool isSpend = true;
        /// <summary> 选择需要的开销</summary>
        public int choiceSpend;
        /// <summary> 1冰 2火 3风</summary>
        public Hotfix_LT.Data.eRoleAttr type;
        /// <summary> 1n 2r 3sr 4ssr</summary>
        public int ssrType;
        /// <summary> 品质</summary>
        public int quality;
        /// <summary>
        /// 星级
        /// </summary>
        public int star;
        /// <summary>
        /// 级别
        /// </summary>
        public int level;
        
        public int peak;

        public int artifactLevel;
        /// <summary>
        /// 升阶
        /// </summary>
        public int upGrade;
        public int skin;
        public int isAwake;

        public string iconName;
        public string modelName;
        public string heroName;

        /// <summary> 套装的id数组</summary>
        public int[] suits = new int[] { 0, 0 };

        /// <summary> 是否能选择并展示</summary>
        public bool IsChoiceShow()
        {
            return !isUsed && !isBan && (isUnLock || isFree) && isSpend;
        }

        /// <summary> 是否可以被选取确定 </summary>
        public bool IsCanChoiceUse()
        {
            return IsChoiceShow();
        }
    }

    public class HeroBattleLevelReward
    {
        public int id;
        public List<LTShowItemData> listShowItemData;

        public HeroBattleLevelReward()
        {
            listShowItemData = new List<LTShowItemData>();
        }
    }
    
    
    public class HonorArenaReward
    {
        public int id;
        public int top;
        public int down;
        public List<LTShowItemData> listShowItemData;
        public int oneHourGold;
        public HonorArenaReward()
        {
            listShowItemData = new List<LTShowItemData>();
        }

    }

}