using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EB.Sparx;
using Hotfix_LT.Data;
using System.Linq;

namespace Hotfix_LT.UI
{
    public class LTPartnerDataManager : ManagerUnit
    {
        private static LTPartnerDataManager instance = null;

        public static LTPartnerDataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LTHotfixManager.GetManager<LTPartnerDataManager>();
                }
                return instance;
            }
        }

        private List<LTPartnerData> mPartnerDataList;//所有伙伴数据，无序

        private Dictionary<string, int> mSkillBreakDelGoodsDic;//用于存储技能突破所使用的材料

        private List<int> mBattleTeamIDList;//上阵的伙伴列表

        private List<int> mSkillBreakRecommendGoodsList;//技能突破推荐材料

        private LTPartnerAPI Api;

        public string itemID;//伙伴升阶需要获取的物品ID
        public int itemNeedCount;////伙伴升阶需要获取的物品数量
        public int DropSelectPartnerId = 0;

        public override void Connect()
        {
            State = SubSystemState.Connected;
            EB.Debug.Log("LTPartnerDataManager Connect!!!");
        }

        public SubSystemState State { get; set; }

        public override void Disconnect(bool isLogout)
        {
            State = SubSystemState.Disconnected;
            ClearPartnerData();
            EB.Debug.Log("LTPartnerDataManager Disconnect!!!");
        }

        public override void Initialize(EB.Sparx.Config config)
        {
            Instance.Api = new LTPartnerAPI();
            Instance.Api.ErrorHandler += ErrorHandler;
            Hotfix_LT.Messenger.AddListener(EventName.AllRedPointDataRefresh, Instance.DataRefresh);
        }

        private void DataRefresh()
        {
            if (IsCanSummon() || IsCanPartnerUpAttr()) { }
        }

        public override void OnLoggedIn()
        {
            PlayerVigourTip.isShow = true;
            InitPartnerData(true);
            InitRecommendGoodsList();
        }

        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        #region cpu 55ms
        /// <summary>
        /// 初始化伙伴数据
        /// </summary>
        public void InitPartnerData(bool isSort = false)
        {
            InitAllPartner(isSort);
        }
        #endregion


        /// <summary>
        /// 获取所有的伙伴（包括未拥有但有碎片的）
        /// </summary>
        /// <param name="isSort"></param>
        /// <returns></returns>
        public List<LTPartnerData> GetGeneralPartnerList(bool isSort = false)
        {
            if (mPartnerDataList == null)
            {
                InitPartnerData();
            }

            if (isSort)
            {
                SortPartnerDataList(mPartnerDataList);
            }

            return mPartnerDataList;
        }


        public LTPartnerData RefreshSkinData(int HeroId)
        {
            for (int i = 0; i < mPartnerDataList.Count; i++)
            {
                if (mPartnerDataList[i].HeroId == HeroId)
                {
                    mPartnerDataList[i].HeroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(mPartnerDataList[i].InfoId, mPartnerDataList[i].CurSkin);
                    return mPartnerDataList[i];
                }
            }
            EB.Debug.Log("Cannot Find PartnerData!");
            return null;
        }
        /// <summary>
        /// 获取已拥有的伙伴
        /// </summary>
        /// <returns></returns>
        public List<LTPartnerData> GetOwnPartnerList()
        {
            List<LTPartnerData> ownList = new List<LTPartnerData>();
            if (mPartnerDataList == null)
            {
                InitPartnerData();
            }
            if (mPartnerDataList == null)
            {
                EB.Debug.LogError("LTPartnerDataManager.GetOwnPartnerList mPartnerDataList is null");
                return ownList;
            }
            for (int i = 0; i < mPartnerDataList.Count; i++)
            {
                if (mPartnerDataList[i].HeroId > 0)
                {
                    ownList.Add(mPartnerDataList[i]);
                }
            }
            return ownList;
        }


        public int GetGuardOpenLevel()
        {
            int level = (int)NewGameConfigTemplateManager.Instance.GetGameConfigValue("Guard_MaxLevel");
            return level;
        }

        public int GetPeakOpenLevel()
        {
            int level = (int)NewGameConfigTemplateManager.Instance.GetGameConfigValue("MaxLevel");
            return level;
        }

        public List<LTPartnerData> GetOwnPartnerListSortByPowerData()
        {
            LTPartnerDataManager.Instance.GetGeneralPartnerList();
            List<LTPartnerData> partnerList = LTPartnerDataManager.Instance.GetOwnPartnerList();
            partnerList.Sort((a, b) =>
            {
                if (a.powerData.curPower > b.powerData.curPower)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });
            return partnerList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">StatId</param>
        /// <returns></returns>
        public GuardHeroData GetLTPartnerDataByID(int id)
        {
            for (int i = 0; i < mPartnerDataList.Count; i++)
            {
                if (mPartnerDataList[i].StatId == id)
                {
                    return ParseGuardHeroData(mPartnerDataList[i]);
                }
            }
            LTPartnerData partnerData = new LTPartnerData();
            partnerData.StatId = id; //tempStatID
            partnerData.HeroStat = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(partnerData.StatId);
            partnerData.InfoId = partnerData.HeroStat.character_id;
            partnerData.mHeroId = 0;
            partnerData.HeroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(partnerData.InfoId, partnerData.CurSkin);//暂无需添加
            return ParseGuardHeroData(partnerData);
        }

        public GuardHeroData ParseGuardHeroData(LTPartnerData ltPartnerData)
        {
            GuardHeroData guardHeroData = new GuardHeroData();
            guardHeroData.heroId = ltPartnerData.HeroId;
            guardHeroData.heroName = ltPartnerData.HeroInfo.name;
            guardHeroData.UpGradeId = ltPartnerData.UpGradeId;
            guardHeroData.icon = ltPartnerData.HeroInfo.icon;
            guardHeroData.char_type = ltPartnerData.HeroInfo.char_type;
            guardHeroData.role_grade = ltPartnerData.HeroInfo.role_grade;
            guardHeroData.star = (ltPartnerData.Star == 0 ? ltPartnerData.HeroInfo.init_star : ltPartnerData.Star);
            guardHeroData.heroLevel = ltPartnerData.Level;
            guardHeroData.isawaken = ltPartnerData.IsAwaken;
            return guardHeroData;
        }

        public void ClearSkillBreakDelGoodsDic()
        {
            if (mSkillBreakDelGoodsDic == null)
            {
                mSkillBreakDelGoodsDic = new Dictionary<string, int>();
            }
            mSkillBreakDelGoodsDic.Clear();
        }

        public void AddSkillBreakDelGoods(string goodsId, int num)
        {
            if (mSkillBreakDelGoodsDic.ContainsKey(goodsId))
            {
                mSkillBreakDelGoodsDic[goodsId] += num;
            }
            else
            {
                mSkillBreakDelGoodsDic.Add(goodsId, num);
            }
        }

        public void RemoveSkillBreakDelGoods(string goodsId, int num)
        {
            if (mSkillBreakDelGoodsDic.ContainsKey(goodsId))
            {
                mSkillBreakDelGoodsDic[goodsId] -= num;
                if (mSkillBreakDelGoodsDic[goodsId] <= 0)
                {
                    mSkillBreakDelGoodsDic.Remove(goodsId);
                }
            }
        }

        public int GetSkillBreakDelGoodsByGoodsId(string goodsId)
        {
            int num = 0;
            mSkillBreakDelGoodsDic.TryGetValue(goodsId, out num);
            return num;
        }

        public Dictionary<string, int> GetSkillBreakDelGoodsDic()
        {
            return mSkillBreakDelGoodsDic;
        }

        /*public List<LTPartnerData> GetOrderedPartnerList()
        {
            List<LTPartnerData> allDataList = new List<LTPartnerData>();
            List<LTPartnerData> targetDataList = new List<LTPartnerData>();


            return targetDataList;
        }*/

        /*public List<LTPartnerData> GetOwnPartnerList()
        {
            List<LTPartnerData> allDataList = GetGeneralPartnerList();
            List<LTPartnerData> targetList = new List<LTPartnerData>();
            for (int i = 0; i < allDataList.Count; i++)
            {
                if (allDataList[i].HeroId > 0)
                {
                    targetList.Add(allDataList[i]);
                }
            }
            return targetList;
        }*/

        /// <summary>
        /// 获取某个属性已拥有伙伴列表
        /// </summary>
        /// <param name="charType"></param>
        /// <returns></returns>
        public List<LTPartnerData> GetOwnPartnerListByCharType(Hotfix_LT.Data.eRoleAttr charType)
        {
            List<LTPartnerData> targetList = new List<LTPartnerData>();
            for (int i = 0; i < mPartnerDataList.Count; i++)
            {
                if (mPartnerDataList[i].HeroInfo.char_type == charType)
                {
                    if (mPartnerDataList[i].HeroId > 0)
                    {
                        targetList.Add(mPartnerDataList[i]);
                    }
                }
            }
            return targetList;
        }

        /// <summary>
        /// 获取某个物种已拥有伙伴列表
        /// </summary>
        /// <param name="race"></param>
        /// <returns></returns>
        public List<LTPartnerData> GetOwnPartnerListByRace(int race)
        {
            List<LTPartnerData> targetList = new List<LTPartnerData>();
            for (int i = 0; i < mPartnerDataList.Count; i++)
            {
                if (mPartnerDataList[i].HeroInfo.race == race)
                {
                    if (mPartnerDataList[i].HeroId > 0)
                    {
                        targetList.Add(mPartnerDataList[i]);
                    }
                }
            }
            return targetList;
        }

        /// <summary>
        /// 获取某个品质伙伴列表
        /// </summary>
        /// <param name="grade">品质</param>
        /// <param name="isSort">是否需要排序</param>
        /// <returns></returns>
        public List<LTPartnerData> GetPartnerListByGrade(int grade, bool isSort = false)
        {
            List<LTPartnerData> allDataList = GetGeneralPartnerList(isSort);
            if (grade == (int)PartnerGrade.ALL)
            {
                return allDataList;
            }

            List<LTPartnerData> targetList = new List<LTPartnerData>();
            for (int i = 0; i < allDataList.Count; i++)
            {
                if (allDataList[i].HeroInfo.role_grade == grade)
                {
                    targetList.Add(allDataList[i]);
                }
            }
            return targetList;
        }

        public LTPartnerData GetPartnerByInfoId(int infoId)
        {
            if (mPartnerDataList == null) InitPartnerData();
            if (mPartnerDataList == null) return null;
            for (int i = 0; i < mPartnerDataList.Count; i++)
            {
                var t_data = mPartnerDataList[i];
                if (t_data!=null && t_data.InfoId == infoId)
                {
                    return t_data;
                }
            }
            return null;
        }

        public LTPartnerData GetPartnerByHeroId(int heroId)
        {
            if (mPartnerDataList == null) InitPartnerData();
            for (int i = 0; i < mPartnerDataList.Count; i++)
            {
                if (mPartnerDataList[i].HeroId == heroId)
                {
                    return mPartnerDataList[i];
                }
            }
            return null;
        }

        public LTPartnerData GetPartnerByStatId(int statId)
        {
            if (mPartnerDataList == null) InitPartnerData();
            for (int i = 0; i < mPartnerDataList.Count; i++)
            {
                if (mPartnerDataList[i] != null && mPartnerDataList[i].StatId == statId)
                {
                    return mPartnerDataList[i];
                }
            }
            return null;
        }

        public int GetPartnerCurLevelExp(int statId)
        {
            LTPartnerData partnerData = GetPartnerByStatId(statId);
            if (partnerData != null)
            {
                int sum = Hotfix_LT.Data.CharacterTemplateManager.Instance.GerHeroPastExpSum(partnerData.Level);
                return partnerData.Exp - sum;
            }
            return 0;
        }

        public int GetPartnerCurSkillLevelExp(int statId, SkillType type)
        {
            LTPartnerData partnerData = GetPartnerByStatId(statId);
            return GetPartnerCurSkillLevelExp(partnerData, type);
        }

        public int GetPartnerCurSkillLevelExp(LTPartnerData partnerData, SkillType type)
        {
            if (partnerData != null)
            {
                int sum = 0;
                List<int> levelList = new List<int>(LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC.Keys);
                for (int i = 0; i < levelList.Count; i++)
                {
                    if (type == SkillType.Active)
                    {
                        if (levelList[i] < partnerData.ActiveSkillLevel)
                        {
                            sum += LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC[levelList[i]];
                        }
                    }
                    else if (type == SkillType.Common)
                    {
                        if (levelList[i] < partnerData.CommonSkillLevel)
                        {
                            sum += LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC[levelList[i]];
                        }
                    }
                    else if (type == SkillType.Passive)
                    {
                        if (levelList[i] < partnerData.PassiveSkillLevel)
                        {
                            sum += LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC[levelList[i]];
                        }
                    }
                }
                if (type == SkillType.Active)
                {
                    return Mathf.Clamp(partnerData.ActiveSkillExp - sum, 0, LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC[partnerData.ActiveSkillLevel] - 1);
                }
                else if (type == SkillType.Common)
                {
                    return Mathf.Clamp(partnerData.CommonSkillExp - sum, 0, LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC[partnerData.CommonSkillLevel] - 1);
                }
                else if (type == SkillType.Passive)
                {
                    return Mathf.Clamp(partnerData.PassiveSkillExp - sum, 0, LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC[partnerData.PassiveSkillLevel] - 1);
                }
            }
            return 0;
        }

        public OtherPlayerPartnerData Translated(LTPartnerData data)
        {
            OtherPlayerPartnerData target = new OtherPlayerPartnerData();
            target.Name = data.HeroInfo.name;
            target.InfoId = data.InfoId;
            target.HeroID = data.HeroId;
            target.Attr = data.HeroInfo.char_type;
            target.Icon = data.HeroInfo.icon;
            target.RoleProflie = data.HeroInfo.role_profile;
            target.RoleProflieSprite = data.HeroInfo.role_profile_icon;
            target.QualityLevel = data.HeroInfo.role_grade;
            target.Level = data.Level;
            target.Star = data.Star;
            target.Pos = data.ControlLevel;
            target.UpGradeId = data.UpGradeId;
            target.race = data.HeroInfo.race;
            target.awakenLevel = data.IsHire ? data.HireAwakeLevel : data.IsAwaken;
            target.Attributes = AttributesUtil.GetBaseAttributes(data);
            target.FinalAttributes = AttributesManager.GetPartnerAttributesByParnterData(data);
            target.commonSkill = data.HeroStat.common_skill;
            target.commonSkillLevel = data.CommonSkillLevel;
            target.activeSkill = data.HeroStat.active_skill;
            target.activeSkillLevel = data.ActiveSkillLevel;
            target.passiveSkill = data.HeroStat.passive_skill;
            target.passiveSkilLevel = data.PassiveSkillLevel;
            target.otherPlayerName = data.HeroInfo.name;
            //target.OtherPower = data.powerData.curPower;
            target.AllRoundLevel = data.AllRoundLevel;
            target.ControlLevel = data.ControlLevel;
            target.StrongLevel = data.StrongLevel;
            target.RageLevel = data.RageLevel;
            target.AbsorbedLevel = data.AbsorbedLevel;
            target.equipmentList = target.equipmentList ?? new Hashtable();
            for (int i = 0; i < data.EquipmentsInfo.Length; i++)
            {
                HeroEquipmentInfo equip = data.EquipmentsInfo[i];
                if (equip.Eid != 0)
                {
                    DetailedEquipmentInfo _info = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(equip.Eid);
                    Hashtable info = new Hashtable();
                    info.Add("economy_id", equip.ECid);
                    info.Add("equipment_type", (int)equip.type);
                    info.Add("inventory_id", equip.Eid);
                    info.Add("level", _info.EquipLevel);
                    target.equipmentList.Add(equip.Eid, info);
                }
            }
            return target;
        }

        public bool IsCanLevelUp(int statId, int addLevel, out int messageId)
        {
            messageId = 0;
            LTPartnerData partnerData = GetPartnerByStatId(statId);
            if (partnerData == null || partnerData.HeroId <= 0)
            {
                messageId = 902221;//尚未拥有该伙伴
                return false;
            }

            int buddyExp = 0;
            DataLookupsCache.Instance.SearchIntByID("res.buddy-exp.v", out buddyExp);
            if (buddyExp <= 0)
            {
                messageId = 902262;//经验槽为空
                return false;
            }

            //从之前的等级受阶级限制改为受玩家等级限制
            //Hotfix_LT.Data.UpGradeInfoTemplate tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetUpGradeInfo(partnerData.UpGradeId, partnerData.HeroInfo.char_type);
            int level = BalanceResourceUtil.GetUserLevel();
            if (level <= partnerData.Level && level < LTPartnerConfig.MAX_LEVEL)//(tpl != null && tpl.level_limit <= partnerData.Level)
            {
                messageId = 902261;//已达到等级上限，请先提升您的等级
                return false;
            }
            else if (level <= partnerData.Level && level >= LTPartnerConfig.MAX_LEVEL)//达到最大等级
            {
                messageId = -1;
                return false;
            }

            // **伙伴等级不在受玩家等级限制  2018/9/10
            //int playerLevel = BalanceResourceUtil.GetUserLevel();
            //if (partnerData.Level >= playerLevel)
            //{
            //    messageId = 902004;//伙伴等级不能超过玩家等级
            //    return false;
            //}

            return true;
        }

        public bool IsCanUpGrade(int statId, out int messageId, out int levelLimit)
        {
            messageId = 0;
            levelLimit = 0;
            LTPartnerData partnerData = GetPartnerByStatId(statId);
            if (partnerData == null || partnerData.HeroId <= 0)
            {
                messageId = 902221;//尚未拥有该伙伴
                return false;
            }

            if (partnerData.UpGradeId >= LTPartnerConfig.MAX_GRADE_LEVEL)
            {
                messageId = -10000;
                return false;
            }

            Hotfix_LT.Data.UpGradeInfoTemplate curTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetUpGradeInfo(partnerData.UpGradeId, partnerData.HeroInfo.char_type);
            if (partnerData.Level < curTpl.level_limit)
            {
                messageId = 902009;//需要伙伴达到{0}级
                levelLimit = curTpl.level_limit;
                return false;
            }

            Hotfix_LT.Data.UpGradeInfoTemplate nextTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetUpGradeInfo(partnerData.UpGradeId + 1, partnerData.HeroInfo.char_type);
            if (nextTpl == null)
            {
                EB.Debug.LogError("No config for upgrading to upgrade level {0}", partnerData.UpGradeId + 1);
                return false;
            }
            List<string> itemList = new List<string>(nextTpl.materialDic.Keys);
            for (int i = 0; i < itemList.Count; i++)
            {
                int needNum = nextTpl.materialDic[itemList[i]];
                int ownNum = GameItemUtil.GetInventoryItemNum(itemList[i]);
                if (needNum > ownNum)
                {
                    messageId = 902309; //902204;//材料不足
                    return false;
                }
            }
            if (nextTpl.needGoldNum > 0 && BalanceResourceUtil.GetResValue(LTResID.GoldName) < nextTpl.needGoldNum)
            {
                messageId = 902309; //902204;//材料不足
                return false;
            }
            if (nextTpl.needGoldNum > 0 && BalanceResourceUtil.GetResValue(LTResID.HcName) < nextTpl.needHcNum)
            {
                messageId = 902309; //902204;//材料不足
                return false;
            }
            return true;
        }

        public bool IsCanStarHoleUp(int statId, out int messageId)
        {
            messageId = 0;

            LTPartnerData partnerData = GetPartnerByStatId(statId);
            if (partnerData == null || partnerData.HeroId <= 0)
            {
                messageId = 902221;//尚未拥有该伙伴
                return false;
            }

            Hotfix_LT.Data.StarInfoTemplate tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetStarInfoByLevelHole(partnerData.Star, partnerData.StarHole + 1);
            if (tpl != null && partnerData.ShardNum < tpl.cost_shard)
            {
                messageId = 902006;//伙伴碎片不足
                return false;
            }

            return true;
        }

        public bool IsCanProficiencyUp(int statId, Hotfix_LT.Data.ProficiencyType type, out int messageId)
        {
            messageId = 0;

            LTPartnerData partnerData = GetPartnerByStatId(statId);
            if (partnerData == null || partnerData.HeroId <= 0)
            {
                messageId = 902221;//尚未拥有该伙伴
                return false;
            }

			// 品阶限制的最大等级
			var desc = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetProficiencyDescByType(type);
			int grade = partnerData.HeroInfo.role_grade > desc.limit.Length ? desc.limit.Length+1 : partnerData.HeroInfo.role_grade;
			int levelMax = desc.limit[grade-1];
			int level = partnerData.GetProficiencyLevelByType(type);
			if (level >= levelMax)
			{
				messageId = 902311;//达到最大等级
				return false;
			}

			//获得下一级的数据
			var temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetProficiencyUpByTypeAndLevel(type, partnerData.GetProficiencyLevelByType(type) + 1);
            if (temp == null)
            {
                messageId = 902311;//达到最大等级
                return false;
            }
            else
            {
				grade = partnerData.HeroInfo.role_grade > temp.goldCost.Length ? temp.goldCost.Length+1 : partnerData.HeroInfo.role_grade;
				if (temp.goldCost[grade-1] > 0 && BalanceResourceUtil.GetUserGold() < temp.goldCost[grade-1])
                {
                    messageId = 901031;//901031金币不足
                    return false;
                }
                if (temp.chipCost[grade-1] > 0 && partnerData.ShardNum < temp.chipCost[grade-1] || temp.potenCost > 0 && BalanceResourceUtil.GetUserPoten() < temp.potenCost)
                {
                    messageId = 902310;//902310潜能材料不足
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 是否开放该伙伴的觉醒功能
        /// </summary>
        /// <param name="InfoId"></param>
        /// <returns></returns>
        public bool IsOpenAwakenFun(int InfoId)
        {
            return Hotfix_LT.Data.CharacterTemplateManager.Instance.HasHeroAwakeInfo(InfoId);
        }

        public bool IsHaveSkin(int InfoId)
        {
            Hotfix_LT.Data.HeroAwakeInfoTemplate temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(InfoId);
            if (temp == null) return false;
            else return !string.IsNullOrEmpty(temp.awakeSkin);
        }
        /// <summary>
        /// 是否达到觉醒的条件 用于红点刷新
        /// </summary>
        /// <param name="statId"></param>
        /// <returns></returns>
        public bool IsCanAwaken(LTPartnerData partnerData)
        {
            Hotfix_LT.Data.FuncTemplate m_FuncTpl = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10087);
            if (m_FuncTpl != null && !m_FuncTpl.IsConditionOK()) return false;
            if (partnerData.IsAwaken > 0) return false;
            Hotfix_LT.Data.HeroAwakeInfoTemplate template = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(partnerData.InfoId);
            if (template == null) return false;
            bool isEnough = true;
            var iter = template.awakeMaterDic.GetEnumerator();
            while (iter.MoveNext())
            {
                if (GameItemUtil.GetInventoryItemNum(iter.Current.Key) < iter.Current.Value)
                    return false;
            }
            return partnerData.Level >= template.limitLevel && partnerData.UpGradeId >= template.limitUpgrade && partnerData.Star >= template.limitStar && isEnough;

        }
        /// <summary>
        /// 根据阶级ID获取对应品质
        /// </summary>
        /// <param name="upGradeId"></param>
        /// <returns></returns>
        public static void GetPartnerQuality(int upGradeId, out int quality, out int addLevel)
        {
            quality = 0;

            for (int i = 0; i < LTPartnerConfig.UP_GRADE_ID_DIC.Count; i++)
            {
                int curGrade = LTPartnerConfig.UP_GRADE_ID_DIC.ContainsKey(i) ? LTPartnerConfig.UP_GRADE_ID_DIC[i] : 0;

                if (curGrade > upGradeId)
                {
                    quality = i - 1;
                    break;
                }
                else if (curGrade == upGradeId)
                {
                    quality = i;
                    break;
                }

                if (i == LTPartnerConfig.UP_GRADE_ID_DIC.Count - 1)
                {
                    quality = i;
                    break;
                }
            }

            var grade = 0;

            if (LTPartnerConfig.UP_GRADE_ID_DIC.ContainsKey(quality))
            {
                grade = LTPartnerConfig.UP_GRADE_ID_DIC[quality];
            }

            addLevel = upGradeId - grade;
        }

        public void ShowPartnerMessage(string content)
        {
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, content);
        }

        /// <summary>
        /// 根据伙伴heroID获取该伙伴是否上阵
        /// </summary>
        /// <param name="heroID"></param>
        /// <returns></returns>
        public bool IsGoIntoBattle(int heroID)
        {
            if (mBattleTeamIDList == null)
            {
                InitPartnerBattleTeam(false);
            }
            return mBattleTeamIDList.Contains(heroID);
        }


        /// <summary>
        /// 获取上阵列表
        /// </summary>
        /// <returns></returns>
        public List<int> GetGoIntoBattleList()
        {
            if (mBattleTeamIDList == null)
            {
                InitPartnerBattleTeam(false);
            }
            return mBattleTeamIDList;
        }


        private List<LTPartnerData> BattlepartnerDataList = new List<LTPartnerData>();
        public int GetAllPower()
        {
            InitPartnerBattleTeam(false);
            BattlepartnerDataList = mPartnerDataList.FindAll(m => m.IsGoIntoBattle);
            if (BattlepartnerDataList == null)
            {
                EB.Debug.LogError("LTPartnerDataManager.GetAllPower BattlepartnerDataList is null");
                return 0;
            }
            int allPower = 0;
            for (int i = 0; i < BattlepartnerDataList.Count; i++)
            {
                allPower += BattlepartnerDataList[i].powerData.curPower;
            }

            return allPower;
        }



        /// <summary>
        /// 初始化上阵伙伴列表
        /// </summary>
        public void InitPartnerBattleTeam(bool isSort = true)
        {
            if (mBattleTeamIDList == null)
            {
                mBattleTeamIDList = new List<int>();
            }
            mBattleTeamIDList.Clear();

            List<TeamMemberData> teamMemberDatas = LTFormationDataManager.Instance.GetTeamMemList("team1");

            for (int i = 0; i < teamMemberDatas.Count; i++)
            {
                mBattleTeamIDList.Add(teamMemberDatas[i].HeroID);
            }

            if (isSort)
            {
                try{
                    SortPartnerDataList(mPartnerDataList);
                }
                catch(System.NullReferenceException e)
                {
                    EB.Debug.LogError(e.ToString());
                }
                
            }
        }

        private int m_ownPartnerNum = 0;
        /// <summary>
        /// 获得能上阵的伙伴数量
        /// </summary>
        public int GetOwnPartnerNum()
        {
            return m_ownPartnerNum;
        }

        /// <summary>
        /// 初始化技能突破推荐材料列表
        /// </summary>
        private void InitRecommendGoodsList()
        {
            if (mSkillBreakRecommendGoodsList != null && mSkillBreakRecommendGoodsList.Count > 0)
            {
                return;
            }

            mSkillBreakRecommendGoodsList = new List<int>();
            string goodsStr = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("SkillBreakRecommend");
            if (string.IsNullOrEmpty(goodsStr))
            {
                EB.Debug.LogError("LTPartnerDataManager InitRecommendGoodsList Recommend is Null");
                return;
            }
            string[] tempGoodsStrs = goodsStr.Split(',');
            if (tempGoodsStrs.Length <= 0)
            {
                EB.Debug.Log("LTPartnerDataManager InitRecommendGoodsList GoodsList is 0");
                return;
            }

            int tempId = 0;
            for (int i = 0; i < tempGoodsStrs.Length; i++)
            {
                int.TryParse(tempGoodsStrs[i], out tempId);
                if (tempId > 0 && !mSkillBreakRecommendGoodsList.Contains(tempId))
                {
                    mSkillBreakRecommendGoodsList.Add(tempId);
                }
            }
        }

        /// <summary>
        /// 是否是技能突破的推荐材料
        /// </summary>
        /// <returns></returns>
        public bool IsSkillBreakRecommend(int goodsId)
        {
            InitRecommendGoodsList();

            return mSkillBreakRecommendGoodsList.Contains(goodsId);
        }



        #region API
        public void SummonBuddy(int statId, System.Action<bool> callback = null)
        {
            Api.RequestSummonBuddy(statId, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }

        public void RequestLevelupFetterBuddy(int buddyId, int index, int condition, int toLevel, System.Action<bool> callback)
        {
            Api.RequestLevelupFetterBuddy(buddyId, index, condition, toLevel, delegate (Hashtable result)
               {
                   if (result != null)
                   {
                       DataLookupsCache.Instance.CacheData(result);
                   }
                   if (callback != null)
                   {
                       callback(callback != null);
                   }
               });
        }

        public void LevelUp(int heroId, int level, System.Action<Hashtable> callback = null)
        {
            Api.ExceptionFun = (string error) =>
            {
                if (error.Equals("ID_ERROR_NOT_ENOUGH_BUDDY_EXP"))
                {
                    //GetParExp.Init();
                    //打开升级界面
                    GlobalMenuManager.Instance.Open("LTPartnerGetParExp", "GetExp");
                    return true;
                }
                return false;
            };
            Api.RequestLevelUp(heroId, level, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result);
                }
            });

            //执行升级任务完成
            TaskSystem.RequestBuddyLevelUpFinish();
        }

        public void ChipTrans(int stateId, int num, System.Action<Hashtable> callback = null)
        {
            Api.RequestChipTrans(stateId, num, delegate (Hashtable result)
            {
                if (callback != null)
                {
                    callback(result);
                }
            });
        }

        public void UpGrade(int heroId, System.Action<bool> callback = null)
        {
            Api.RequestUpGrade(heroId, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }

        public void StarUp(int infoId, System.Action<bool> callback = null)
        {
            Api.ExceptionFun = (string error) =>
            {
                if (error.Equals("no shard in bag") || error.Equals("insufficient shard"))
                {
                    Messenger.Raise(Hotfix_LT.EventName.OnPartnerHoleUpError);
                    return true;
                }
                return false;
            };

            Api.RequestStarUp(infoId, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }

        public void ProficiencyUp(int buddyId, int form, Hotfix_LT.Data.ProficiencyType type, int level, System.Action<bool> callback = null)
        {
            Api.RequestProficiencyUp(buddyId, form, (int)type, level, delegate (Hashtable result)
             {
                 if (result != null)
                 {
                     DataLookupsCache.Instance.CacheData(result);
                 }
                 if (callback != null)
                 {
                     callback(result != null);
                 }
             });
        }

        public void SkillBreak(int heroId, int skillId, ArrayList goodsList, System.Action<bool> callback = null)
        {
            Api.ExceptionFun = (string error) =>
            {
                if (error.Equals("skillBreakToBuddy:already starLimit maxLevel"))
                {
                    LTPartnerData data = GetPartnerByHeroId(heroId);
                    string str = string.Empty;
                    if (data.Star >= LTPartnerConfig.MAX_STAR)
                    {
                        str = EB.Localizer.GetString("ID_codefont_in_LTPartnerDataManager_18538");
                    }
                    else
                    {
                        str = EB.Localizer.GetString("ID_codefont_in_LTPartnerDataManager_18644");
                    }

                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, str);
                    return true;
                }
                if (error.Equals("invalid materials"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ERROR_INSUFFICIENT_ITEMS"));
                    return true;
                }
                return false;
            };

            Api.RequestSkillBreak(heroId, skillId, goodsList, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }

        public void SetLeader(int heroId, System.Action<bool> callback = null)
        {
            Api.RequestLeader(heroId, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }

        public void UpgradeArtifact(int buddyId,int level,Action<bool> callback = null)
        {
            Api.ExceptionFun = (string error) =>
            {
                if (error.Equals("no inventory data") || error.Equals("ID_ERROR_NOT_ENOUGH_INVENTORY_EXP"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,EB.Localizer.GetString("ID_ERROR_INSUFFICIENT_ITEMS"));
                    return true;
                }
                if (error.Equals("NOT_ENOUGH_ENHANCEMENT_LEVEL_EXP"))
                {
                 
                }
                return false;
            };
            
            Api.RequestActiveOrUpgradeArtifEquip(buddyId, level,delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }

        /// <summary>
        /// 使用特殊物品
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="amount"></param>
        /// <param name="itemId"></param>
        /// <param name="callback"></param>
        public void UseItem(string inventoryId, int amount, int index, System.Action<bool> callback = null)
        {
            Api.RequestUseItem(inventoryId, amount, index, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }
        public void PartnerTrans(int herooneId, int herotwoId, bool switchEquip, bool useReplaceTicket,bool switchPeak,bool switchPo, Action<bool> callback = null)
        {
            Api.RequestPartnerTrans(herooneId, herotwoId, switchEquip, useReplaceTicket,switchPeak,switchPo, delegate (Hashtable result)
              {
                  if (result != null)
                  {
                      foreach (DictionaryEntry pair in result)
                      {
                          object incomingValue = pair.Value;
                          string incomingKey = pair.Key.ToString();
                        //唯一id
                        string loopID = "heroStats";
                          if (incomingKey.Equals(loopID))
                          {
                              IDictionary incomingValueAsCacheHash = incomingValue as IDictionary;
                              foreach (DictionaryEntry VARIABLE in incomingValueAsCacheHash)
                              {
                                //重置两个伙伴的数据  因为原先的数据Cache后会叠加
                                DataLookupsCache.Instance.CacheData(string.Format("heroStats.{0}", VARIABLE.Key), String.Empty);
                              }
                          }
                      }

                      DataLookupsCache.Instance.CacheData(result);
                  }
                  if (callback != null)
                  {
                      callback(result != null);
                  }
              });
        }

        public void PartnerAwake(int heroId, System.Action<bool> callback = null)
        {
            Api.RequestPartnerAwake(heroId, delegate (Hashtable result)
            {

                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result != null);
                }
            });

        }

        public void PartnerUseAwakeSkin(int heroId, int skinId, int sceneId, System.Action<bool> callback = null)
        {
            Api.RequestUseAwakenSkin(heroId, skinId, sceneId, delegate (Hashtable result)
            {

                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }


        public void ReceiveFirstGotReward(int heroId, System.Action<bool> callback = null)
        {
            Api.RequestReceiveFirstGotReward(heroId, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }

        //领取等级补偿奖励
        public void ReceiveLevelCompensatedReward(int id, System.Action<bool> callback = null)
        {
            Api.RequestReceiveLevelCompensatedReward(id, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }


        /// <summary>
        /// 上报战力变化，未使用
        /// </summary>
        /// <param name="power"></param>
        public void ReportpowerChanged(int power)
        {
            Api.ReportpowerChanged(power);
        }

        #endregion

        #region 伙伴红点逻辑

        //是否有上阵伙伴能提升的
        public bool IsCanPartnerUpAttr()
        {
            if (mPartnerDataList == null)
            {
                return false;
            }

            for (int i = 0; i < mPartnerDataList.Count; i++)
            {
                LTPartnerData partnerData = mPartnerDataList[i];
                if (partnerData.IsGoIntoBattle)
                {
                    if (IsCanStarUp(partnerData) || IsCanGradeUp(partnerData) || IsCanLevelUp(partnerData) ||
                        IsCanSkillBreak(partnerData) || IsCanTopLevelUp(partnerData) || IsCanAwaken(partnerData) ||
                        IsCanReceiveReward(partnerData))
                    {
                        LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.partner, 1);
                        return true;
                    }
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.partner, 0);
            return false;
        }

        //是否可以培养
        public bool IsCanCultivate(LTPartnerData partnerData)
        {
            if (IsCanStarUp(partnerData))
            {
                return true;
            }
            if (IsCanGradeUp(partnerData))
            {
                return true;
            }
            if (IsCanLevelUp(partnerData))
            {
                return true;
            }
            if (IsCanSkillBreak(partnerData))
            {
                return true;
            }
            if (IsCanTopLevelUp(partnerData))
            {
                return true;
            }
            if (IsCanAwaken(partnerData))
            {
                return true;
            }
            if (IsCanGuardLevelUp(partnerData))
            {
                return true;
            }
            
            if (IsCanArtifact(partnerData))
            {
                return true;
            }
            return false;
        }
        
        public bool IsCanArtifact(LTPartnerData data)
        {
            ArtifactEquipmentTemplate template = CharacterTemplateManager.Instance.GetArtifactEquipmentByLevel(data.InfoId,data.ArtifactLevel);
            if (template != null)
            {
                if (LTInstanceUtil.IsCampaignsComplete(ArtifactItemController.GetCapLimit()) && data.Star >= ArtifactItemController.GetStarLimit())
                {
                    string[] args = template.ItemCost.Split(',');
                    if (args.Length >= 2)
                    {
                        int curCount = GameItemUtil.GetInventoryItemNum(args[0]);
                        int.TryParse(args[1], out var needCount);
                        return curCount >= needCount;
                    }
                }
            }
            return false;
        }

        //是否有可召唤的伙伴
        public bool IsCanSummon()
        {
            if (mPartnerDataList == null)
            {
                return false;
            }

            for (int i = 0; i < mPartnerDataList.Count; i++)
            {
                LTPartnerData partnerData = mPartnerDataList[i];
                if (IsCanSummon(partnerData))
                {
                    LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.partner, 1);
                    return true;
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.partner, 0);
            return false;
        }

        //是否可以召唤伙伴
        public bool IsCanSummon(LTPartnerData partnerData)
        {
            if (partnerData.HeroId <= 0 && partnerData.ShardNum >= partnerData.HeroInfo.summon_shard)
            {
                return true;
            }

            return false;
        }

        //是否有可升星的伙伴
        public bool IsCanStarUp()
        {
            if (mPartnerDataList == null)
            {
                return false;
            }

            for (int i = 0; i < mPartnerDataList.Count; i++)
            {
                LTPartnerData partnerData = mPartnerDataList[i];
                if (partnerData.IsGoIntoBattle && IsCanStarUp(partnerData))
                {
                    return true;
                }
            }

            return false;
        }

        //伙伴是否可以升星
        public bool IsCanStarUp(LTPartnerData partnerData)
        {
            if (partnerData.HeroId <= 0)
            {
                return false;
            }

            if (partnerData.StarHole >= 10)
            {
                return true;
            }

            Hotfix_LT.Data.StarInfoTemplate starInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetStarInfoByLevelHole(partnerData.Star, 1);
            if (starInfo != null && partnerData.ShardNum >= starInfo.cost_shard)
            {
                return true;
            }

            if (partnerData.Star >= LTPartnerConfig.MAX_STAR)
            {				
				for (int type = 2; type < 5; type++)
                {
					int level = partnerData.GetProficiencyLevelByType((Hotfix_LT.Data.ProficiencyType)type);
                    var temp = CharacterTemplateManager.Instance.GetProficiencyUpByTypeAndLevel((ProficiencyType)type, level + 1);

					var desc = CharacterTemplateManager.Instance.GetProficiencyDescByType((ProficiencyType)type);
					int grade = partnerData.HeroInfo.role_grade > desc.limit.Length ? desc.limit.Length + 1 : partnerData.HeroInfo.role_grade;
					int levelMax = desc.limit[grade - 1];

					if (temp != null && level < levelMax)
					{
						grade = partnerData.HeroInfo.role_grade > temp.chipCost.Length ? 1 : partnerData.HeroInfo.role_grade;
						if (partnerData.ShardNum >= temp.chipCost[grade - 1] && BalanceResourceUtil.GetUserGold() >= temp.goldCost[grade - 1])
						{
							return true;
						}
					}
                }
            }
            return false;
        }

        //是否有可升阶的伙伴
        public bool IsCanGradeUp()
        {
            if (mPartnerDataList == null)
            {
                return false;
            }

            for (int i = 0; i < mPartnerDataList.Count; i++)
            {
                LTPartnerData partnerData = mPartnerDataList[i];
                if (partnerData.IsGoIntoBattle && IsCanGradeUp(partnerData))
                {
                    return true;
                }
            }

            return false;
        }

        //伙伴是否可以升阶
        public bool IsCanGradeUp(LTPartnerData partnerData)
        {
            if (partnerData.HeroId <= 0)
            {
                return false;
            }

            Hotfix_LT.Data.UpGradeInfoTemplate curEvoTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetUpGradeInfo(partnerData.UpGradeId, partnerData.HeroInfo.char_type);
            if (curEvoTpl != null && partnerData.Level >= curEvoTpl.level_limit)
            {
                Hotfix_LT.Data.UpGradeInfoTemplate evoTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetUpGradeInfo(partnerData.UpGradeId + 1, partnerData.HeroInfo.char_type);
                if (evoTpl != null)
                {
                    var iter = evoTpl.materialDic.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        if (GameItemUtil.GetInventoryItemNum(iter.Current.Key) < iter.Current.Value)
                        {
                            return false;
                        }
                    }
                    if (evoTpl.needGoldNum > 0 && BalanceResourceUtil.GetResValue(LTResID.GoldName) < evoTpl.needGoldNum)
                    {
                        return false;
                    }
                    if (evoTpl.needGoldNum > 0 && BalanceResourceUtil.GetResValue(LTResID.HcName) < evoTpl.needHcNum)
                    {
                        return false;
                    }
                    return true;
                }
            }

            return false;
        }

        //是否有可升级的伙伴
        public bool IsCanLevelUp()
        {
            if (mPartnerDataList == null)
            {
                return false;
            }

            for (int i = 0; i < mPartnerDataList.Count; i++)
            {
                LTPartnerData partnerData = mPartnerDataList[i];
                if (partnerData.IsGoIntoBattle && IsCanLevelUp(partnerData))
                {
                    return true;
                }
            }

            return false;
        }

        //伙伴是否可以升级
        public bool IsCanLevelUp(LTPartnerData partnerData)
        {
            if (partnerData.HeroId <= 0)
            {
                return false;
            }

            if (BalanceResourceUtil.GetUserLevel() <= partnerData.Level)
            {
                return false;
            }

            Hotfix_LT.Data.HeroLevelInfoTemplate levelTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroLevelInfo(partnerData.Level);
            if (levelTpl != null)
            {
                int curNeedExp = levelTpl.buddy_exp;
                int curExp = GetPartnerCurLevelExp(partnerData.StatId);
                int buddyExp = 0;
                DataLookupsCache.Instance.SearchDataByID("res.buddy-exp.v", out buddyExp);
                if (buddyExp >= curNeedExp - curExp)
                {
                    return true;
                }
            }

            return false;
        }

        //是否有可技能突破的伙伴
        public bool IsCanSkillBreak()
        {
            if (mPartnerDataList == null)
            {
                return false;
            }

            for (int i = 0; i < mPartnerDataList.Count; i++)
            {
                LTPartnerData partnerData = mPartnerDataList[i];
                if (partnerData.IsGoIntoBattle && IsCanSkillBreak(partnerData))
                {
                    return true;
                }
            }

            return false;
        }

        //伙伴是否可以技能突破
        public bool IsCanSkillBreak(LTPartnerData partnerData)
        {
            if (partnerData.HeroId <= 0)
            {
                return false;
            }

            if (partnerData.ActiveSkillLevel >= LTPartnerConfig.MAX_SKILL_LEVEL && partnerData.CommonSkillLevel >= LTPartnerConfig.MAX_SKILL_LEVEL && partnerData.PassiveSkillLevel >= LTPartnerConfig.MAX_SKILL_LEVEL)
            {
                return false;
            }

            int totalExp = 0;
            //只需要计算技能突破道具的总经验值，突破道具id = 1021~1025
            InitRecommendGoodsList();
            for (int i = 0; i < mSkillBreakRecommendGoodsList.Count; i++)
            {
                Hotfix_LT.Data.GeneralItemTemplate generalItemData = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(mSkillBreakRecommendGoodsList[i]) as Hotfix_LT.Data.GeneralItemTemplate;
                if (generalItemData != null)
                {
                    totalExp += GameItemUtil.GetInventoryItemNum(mSkillBreakRecommendGoodsList[i]) * generalItemData.Exp;
                }
            }
            int maxSkillLevel = LTPartnerConfig.MAX_SKILL_LEVEL;
            bool isCanSkillBreak = false;
            if (partnerData.ActiveSkillLevel < maxSkillLevel)
            {
                int curExp = GetPartnerCurSkillLevelExp(partnerData, SkillType.Active);
                int level = partnerData.ActiveSkillLevel;
                if (LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC.TryGetValue(partnerData.ActiveSkillLevel, out int needExp))
                {
                    isCanSkillBreak = needExp - curExp <= totalExp;
                }
                else
                {
                    isCanSkillBreak = false;
                }

            }

            if (partnerData.CommonSkillLevel < maxSkillLevel && !isCanSkillBreak)
            {
                int curExp = GetPartnerCurSkillLevelExp(partnerData, SkillType.Common);
                if (LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC.TryGetValue(partnerData.CommonSkillLevel, out int needExp))
                {
                    isCanSkillBreak = needExp - curExp <= totalExp;
                }
                else
                {
                    isCanSkillBreak = false;
                }

            }

            if (partnerData.PassiveSkillLevel < maxSkillLevel && !isCanSkillBreak)
            {
                int curExp = GetPartnerCurSkillLevelExp(partnerData, SkillType.Passive);
                if (LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC.TryGetValue(partnerData.PassiveSkillLevel, out int needExp))
                {
                    isCanSkillBreak = needExp - curExp <= totalExp;
                }
                else
                {
                    isCanSkillBreak = false;
                }
            }

            return isCanSkillBreak;
        }

        //伙伴是否推荐可以提升装备等级
        public bool IsHasCanEquipUpLvPartnerInGoIntoBattle()
        {
            if (mPartnerDataList == null)
            {
                return false;
            }

            for (int i = 0; i < mPartnerDataList.Count; i++)
            {
                LTPartnerData partnerData = mPartnerDataList[i];
                if (partnerData.IsCheckEquipUpLv())
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsCanTopLevelUp()
        {
            if (mPartnerDataList == null)
            {
                return false;
            }

            for (int i = 0; i < mPartnerDataList.Count; i++)
            {
                LTPartnerData partnerData = mPartnerDataList[i];
                if (partnerData.IsGoIntoBattle && IsCanTopLevelUp(partnerData))
                {
                    return true;
                }
            }

            return false;
        }

        //伙伴是否可提升巅峰等级
        public bool IsCanTopLevelUp(LTPartnerData partnerData)
        {
            if (partnerData == null || partnerData.Level < Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMaxPlayerLevel()) return false;
            var temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetProficiencyUpByTypeAndLevel(Hotfix_LT.Data.ProficiencyType.AllRound, partnerData.GetProficiencyLevelByType(Hotfix_LT.Data.ProficiencyType.AllRound) + 1);
            if (temp != null)
            {
                int grade = partnerData.HeroInfo.role_grade > temp.goldCost.Length ? 1 : partnerData.HeroInfo.role_grade;
                if (BalanceResourceUtil.GetUserGold() >= temp.goldCost[grade - 1] && BalanceResourceUtil.GetUserPoten() >= temp.potenCost)
                {
                    return true;
                }
            }
            return false;
        }


        //伙伴是否可以领取初次获得奖励
        public bool IsCanReceiveReward(LTPartnerData partnerData)
        {
            if (partnerData != null && partnerData.HeroId > 0 && !partnerData.HasReceiveReward) return true;
            return false;
        }

        public bool IsCanReceiveReward()
        {
            if (mPartnerDataList == null)
            {
                return false;
            }
            for (int i = 0; i < mPartnerDataList.Count; i++)
            {
                if (mPartnerDataList[i].HeroId > 0 && !mPartnerDataList[i].HasReceiveReward) return true;
            }
            return false;
        }
        #endregion

        #region cpu 41ms -> 15ms
        private Dictionary<string, int> _InitAllPartner_BagItemsSum = new Dictionary<string, int>();
        /// <summary>
        /// 初始化伙伴数据
        /// </summary>
        /// <param name="isSort"></param>
        private void InitAllPartner(bool isSort = false)
        {
            bool isCheck = true;
            if (mPartnerDataList == null)
            {
                mPartnerDataList = new List<LTPartnerData>();
                m_ownPartnerNum = 0;
                isCheck = false;
            }

            Hashtable ownPartnerCollection = Johny.HashtablePool.Claim();
            DataLookupsCache.Instance.SearchDataByID(LTPartnerConfig.DATA_PATH_ROOT, out ownPartnerCollection);
            if (ownPartnerCollection == null)
            {
                EB.Debug.Log("<color=#ff0000>InitAllPartner ownPartnerCollection is Null</color>");
                return;
            }

            #region 获取背包物品sum列表
            _InitAllPartner_BagItemsSum.Clear();
            GameItemUtil.QueryAllBagItemsSum(_InitAllPartner_BagItemsSum);
            #endregion

            var iter = ownPartnerCollection.GetEnumerator();
            while (iter.MoveNext())
            {
                int tempStatID = EB.Dot.Integer("template_id", iter.Value, 0);
                if (isCheck)
                {
                    bool isHave = false;
                    for (int i = 0; i < mPartnerDataList.Count; i++)
                    {
                        if (mPartnerDataList[i].StatId == tempStatID)
                        {
                            if (mPartnerDataList[i].HeroId <= 0)
                            {
                                m_ownPartnerNum++;
                                mPartnerDataList[i].mHeroId = Convert.ToInt32(iter.Key);
                            }
                            isHave = true;
                            break; ;
                        }
                    }

                    if (isHave)
                    {
                        continue;
                    }
                }

                m_ownPartnerNum++;
                LTPartnerData partnerData = new LTPartnerData();
                partnerData.StatId = tempStatID;
                partnerData.InfoId = EB.Dot.Integer("character_id", iter.Value, partnerData.InfoId);
                partnerData.mHeroId = int.Parse(iter.Key.ToString());
                partnerData.HeroStat = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(partnerData.StatId);
                partnerData.HeroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(partnerData.InfoId, partnerData.CurSkin);//暂无需添加
                mPartnerDataList.Add(partnerData);
            }

            Hotfix_LT.Data.HeroStatTemplate[] allPartnerCollection = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStats();

            if (allPartnerCollection == null) return;
            for (int j = 0; j < allPartnerCollection.Length; j++)
            {
                int partnerCollectionId = allPartnerCollection[j].id;
                bool isHave = false;
                for (int i = 0; i < mPartnerDataList.Count; i++)
                {
                    if (mPartnerDataList[i].StatId == partnerCollectionId)
                    {
                        isHave = true;
                        break;
                    }
                }

                if (isHave)
                {
                    continue;
                }

                LTPartnerData partnerData = new LTPartnerData();
                partnerData.StatId = partnerCollectionId;
                partnerData.InfoId = allPartnerCollection[j].character_id;
                partnerData.mHeroId = 0;

                //伙伴没碎片不显示
                int theStatSum = 0;
                _InitAllPartner_BagItemsSum.TryGetValue(partnerData.StatId.ToString(), out theStatSum);
                if (theStatSum == 0)
                {
                    continue;
                }

                partnerData.HeroStat = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(partnerData.StatId);
                partnerData.HeroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(partnerData.InfoId);

                mPartnerDataList.Add(partnerData);
            }

            if (isSort)
            {
                SortPartnerDataList(mPartnerDataList);
            }

            if (mPartnerDataList.Count <= 0)
            {
                EB.Debug.LogError("LTPartnerDataManager 伙伴DataCount less 0");
            }
        }
        #endregion

        /// <summary>
        /// 清理数据
        /// </summary>
        private void ClearPartnerData()
        {
            mPartnerDataList = null;
            m_ownPartnerNum = 0;
            mSkillBreakDelGoodsDic = null;
            mSkillBreakRecommendGoodsList = null;
            mBattleTeamIDList = null;
        }

        #region cpu 94ms -> 7.95ms
        private Dictionary<string, int> _SortPartnerDataList_dic = new Dictionary<string, int>();
        /// <summary>
        /// 对伙伴数据排序
        /// </summary>
        /// <param name="list"></param>
        private void SortPartnerDataList(List<LTPartnerData> list)
        {
            _SortPartnerDataList_dic.Clear();
            GameItemUtil.QueryAllBagItemsSum(_SortPartnerDataList_dic);

            list.Sort((LTPartnerData x, LTPartnerData y) =>
            {
                if (x.HeroInfo == null)
                {
                    EB.Debug.LogError("LTPartnerData HeroInfo is Null ! id = {0}", x.StatId);
                    return 0;
                }

                if (y.HeroInfo == null)
                {
                    EB.Debug.LogError("LTPartnerData HeroInfo is Null ! id = {0}", y.StatId);
                    return 0;
                }

                int flg = 0;

                bool xHave = x.HeroId > 0;
                bool yHave = y.HeroId > 0;

                int xHaveVal = xHave ? 0 : 1;
                int yHaveVal = yHave ? 0 : 1;

                int x_shardNum = 0;
                _SortPartnerDataList_dic.TryGetValue(x.StatId.ToString(), out x_shardNum);
                int y_shardNum = 0;
                _SortPartnerDataList_dic.TryGetValue(y.StatId.ToString(), out y_shardNum);

                bool xCanCall = xHaveVal == 1 && x_shardNum >= x.HeroInfo.summon_shard;
                bool yCanCall = yHaveVal == 1 && y_shardNum >= y.HeroInfo.summon_shard;

                int xCanCallVal = xCanCall ? 0 : 1;
                int yCanCallVal = yCanCall ? 0 : 1;
                flg = xCanCallVal - yCanCallVal;
                if (flg != 0)
                {
                    return flg;//根据是否可召唤
                }

                flg = xHaveVal - yHaveVal;

                if (flg != 0)
                {
                    return flg;//根据当前伙伴有没有
                }

                if (xHave && yHave)
                {
                    int xFlagVal = x.IsGoIntoBattle ? 0 : 1;
                    int yFlagVal = y.IsGoIntoBattle ? 0 : 1;
                    flg = xFlagVal - yFlagVal;
                    if (flg != 0)
                    {
                        return flg;//根据是否上阵
                    }

                    flg = y.Level - x.Level;
                    if (flg != 0)
                    {
                        return flg;//根据等级
                    }

                    flg = y.UpGradeId - x.UpGradeId;
                    if (flg != 0)
                    {
                        return flg;//根据阶级
                    }

                    flg = y.HeroInfo.role_grade - x.HeroInfo.role_grade;
                    if (flg != 0)
                    {
                        return flg;//根据稀有度（ssr - sr - r - n）
                    }
                }
                else
                {
                    flg = -flg;
                    if (flg != 0)
                    {
                        return flg;//都没有的情况下碎片多的排前面
                    }
                }


                //最后是根据ID从小到大
                return x.StatId - y.StatId;
            });

            _SortPartnerDataList_dic.Clear();
        }
        #endregion

        /// <summary>
        /// 返回突破材料的经验值 和当前剩余能选择的数量
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="exp"></param>
        /// <param name="goodsLeftCount"></param>
        public void GetExpByGoodsId(string goodsId, ref int exp, ref int goodsLeftCount)
        {
            IDictionary itemData;
            DataLookupsCache.Instance.SearchDataByID<IDictionary>(string.Format("inventory.{0}", goodsId), out itemData);
            int economyId = EB.Dot.Integer("economy_id", itemData, 0);
            int goodsCount = EB.Dot.Integer("num", itemData, 0);
            //
            goodsLeftCount = goodsCount - GetSkillBreakDelGoodsByGoodsId(goodsId);
            Hotfix_LT.Data.GeneralItemTemplate generalItemData = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(economyId) as Hotfix_LT.Data.GeneralItemTemplate;
            if (generalItemData == null)
            {
                Hotfix_LT.Data.EquipmentItemTemplate equipmentItemData = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(economyId) as Hotfix_LT.Data.EquipmentItemTemplate;
                exp = equipmentItemData.BaseExp;
            }
            else
            {
                exp = generalItemData.Exp;
            }
        }


        public static string PartnerStatsID;
        public static string EquipID;
        #region GM工具相关
        public static void GMTool(string Func, string PID, string EID)
        {
            if (!ILRDefine.DEBUG) return;
            PartnerStatsID = PID;
            EquipID = EID;
            switch (Func)
            {
                case "MaxPower":
                    {
                        instance.MaxPower();
                    }; break;
                case "PartnerLevelUp":
                    {
                        instance.PartnerLevelUp();
                    }; break;
                case "PartnerStarUp":
                    {
                        instance.PartnerStarUp();
                    }; break;
                case "PartnerSkillUp":
                    {
                        instance.PartnerSkillUp();
                    }; break;
                case "PartnerEquipAllAndUp":
                    {
                        instance.PartnerEquipAllAndUp();
                    }; break;
                case "InitPartnerData":
                    {
                        instance.InitPartnerData();
                    }; break;
            }
        }

        /// <summary>
        /// 满级满阶
        /// </summary>
        private void PartnerLevelUp()
        {
            SummonPartner(delegate
            {
                LTPartnerData partnerData = LTPartnerDataManager.Instance.GetPartnerByStatId(int.Parse(PartnerStatsID));
                //添加所需要的伙伴经验，从一级到六十级需要的伙伴经验为：8372360；
                SetBuddyExp(10000000, delegate
                {
                    Dictionary<string, int> goodsDic = new Dictionary<string, int>();
                    for (int i = partnerData.UpGradeId + 1; i < 14; i++)
                    {
                        UpGradeInfoTemplate info = CharacterTemplateManager.Instance.GetUpGradeInfo(i, partnerData.HeroInfo.char_type);
                        foreach (var item in info.materialDic)
                        {
                            if (goodsDic.ContainsKey(item.Key))
                            {
                                goodsDic[item.Key] += item.Value;
                            }
                            else
                            {
                                goodsDic.Add(item.Key, item.Value);
                            }
                        }
                    }

                    List<string> gradeItemIDList = new List<string>(goodsDic.Keys);

                    for (int j = 0; j < gradeItemIDList.Count; j++)
                    {
                        //添加升阶所需要的材料
                        if (j == gradeItemIDList.Count - 1)
                        {
                            AddItemToInv(gradeItemIDList[j], goodsDic[gradeItemIDList[j]], delegate
                            {
                                PartnerLevelGradeUp(partnerData.HeroId, 13 - partnerData.UpGradeId);
                            });
                        }
                        else
                        {
                            AddItemToInv(gradeItemIDList[j], goodsDic[gradeItemIDList[j]]);
                        }
                    }
                });
            });
        }

        /// <summary>
        /// 满星
        /// </summary>
        private void PartnerStarUp()
        {
            SummonPartner(delegate
            {
                LTPartnerData partnerData = LTPartnerDataManager.Instance.GetPartnerByStatId(int.Parse(PartnerStatsID));
                if (partnerData == null || partnerData.Star >= 6)
                {
                    return;
                }
                //添加所需要的伙伴碎片
                AddItemToInv(PartnerStatsID, 650 - partnerData.ShardNum, delegate
                {
                    int starIndex = 6 - partnerData.Star - 1;
                    int holeIndex = 10 - partnerData.StarHole + 1;
                    int count = starIndex * 11 + holeIndex;
                    for (int i = 0; i < count; i++)
                    {
                        LTPartnerDataManager.Instance.StarUp(partnerData.InfoId);
                    }
                });
            });
        }

        /// <summary>
        /// 满技能等级
        /// </summary>
        private void PartnerSkillUp()
        {
            SummonPartner(delegate
            {
                LTPartnerData partnerData = LTPartnerDataManager.Instance.GetPartnerByStatId(int.Parse(PartnerStatsID));
                int count = 0;
                if (partnerData.CommonSkillLevel < 10)
                {
                    count++;
                }
                if (partnerData.ActiveSkillLevel < 10)
                {
                    count++;
                }
                if (partnerData.PassiveSkillLevel < 10)
                {
                    count++;
                }

                int num = count * 100;
                int bagNum = GameItemUtil.GetInventoryItemNum("1025");
                num = Mathf.Max(num - bagNum, 1);
                //添加魔法书（吞噬经验1000）
                AddItemToInv("1025", num, delegate
                {
                    ArrayList tempGoodsList = new ArrayList();
                    Hashtable table = new Hashtable();
                    table.Add("inventoryId", GetServerGoodsId("1025", 100));
                    table.Add("num", 100);
                    tempGoodsList.Add(table);

                    if (partnerData.CommonSkillLevel < 10)
                    {
                        int skillId = partnerData.HeroStat.common_skill;
                        LTPartnerDataManager.Instance.SkillBreak(partnerData.HeroId, skillId, tempGoodsList);
                    }
                    if (partnerData.ActiveSkillLevel < 10)
                    {
                        int skillId = partnerData.HeroStat.active_skill;
                        LTPartnerDataManager.Instance.SkillBreak(partnerData.HeroId, skillId, tempGoodsList);
                    }
                    if (partnerData.PassiveSkillLevel < 10)
                    {
                        int skillId = partnerData.HeroStat.passive_skill;
                        LTPartnerDataManager.Instance.SkillBreak(partnerData.HeroId, skillId, tempGoodsList);
                    }
                });
            });
        }

        /// <summary>
        /// 某个伙伴穿上该套装并且强15
        /// </summary>
        private void PartnerEquipAllAndUp()
        {
            SummonPartner(delegate
            {
                LTPartnerData partnerData = LTPartnerDataManager.Instance.GetPartnerByStatId(int.Parse(PartnerStatsID));
                int equipId = 17;
                int.TryParse(EquipID, out equipId);
                List<int> equipIDList = new List<int>();
                for (int i = 0; i < 6; i++)
                {
                    int eid = 100016 + equipId * 100 + i * 10;
                    equipIDList.Add(eid);
                }
                SetGold(10000000);

                AddItemToInv("1033", 60);
                for (int i = 0; i < equipIDList.Count; i++)
                {
                    string tempeId = equipIDList[i].ToString();
                    EquipPartType partIndex = (EquipPartType)(i + 1);
                    AddItemToInv(tempeId, 1, delegate
                    {
                        string eUid = GetServerGoodsId(tempeId);
                        //EB.Debug.LogError("eUid = {0}, HeroId = {1}, tempeId = {2}， partIndex = {3}", eUid, partnerData.HeroId, tempeId, partIndex);
                        LTPartnerEquipDataManager.Instance.DebugRequireEquip(int.Parse(eUid), partnerData.HeroId, partIndex);

                        string goodId = GetServerGoodsId("1033", 10);
                        List<Hashtable> table = new List<Hashtable>()
                    {
                        new Hashtable() { { "id", goodId }, { "count", 10 } }
                    };
                        LTPartnerEquipDataManager.Instance.RequireUpLevel(int.Parse(eUid), new List<int>(), table);

                    });

                    //if (i == equipIDList.Count - 1)
                    //{
                    //    AddItemToInv(equipIDList[i].ToString(), 1, delegate 
                    //    {

                    //    });
                    //}
                    //else
                    //{
                    //    AddItemToInv(equipIDList[i].ToString(), 1);
                    //}
                }
            });
        }

        /// <summary>
        /// 获取所有套装并且强15
        /// </summary>
        private IEnumerator GetAllMaxEquip()
        {
            //21 * 6 * 10 = 1260 //多给一点
            AddItemToInv("1033", 999);
            AddItemToInv("1033", 271);
            SetGold(136 * 500000);

            List<int> equipIDList = new List<int>();
            for (int i = 0; i < 21; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    int eid = 100116 + i * 100 + j * 10;
                    equipIDList.Add(eid);
                }
            }

            for (int i = 0; i < equipIDList.Count; i++)
            {
                string tempeId = equipIDList[i].ToString();
                bool isGo = false;
                AddItemToInv(tempeId, 1, delegate
                {
                    string eUid = GetServerGoodsId(tempeId);
                    string goodId = GetServerGoodsId("1033", 10);
                    List<Hashtable> table = new List<Hashtable>()
                    {
                        new Hashtable() { { "id", goodId }, { "count", 10 } }
                    };
                    LTPartnerEquipDataManager.Instance.RequireUpLevel(int.Parse(eUid), new List<int>(), table, delegate { isGo = true; });
                });

                while (!isGo)
                {
                    yield return null;
                }
            }
        }

        /// <summary>
        /// 毕业
        /// </summary>
        private void MaxPower()
        {
            SummonPartner(delegate
            {
                LTPartnerData partnerData = LTPartnerDataManager.Instance.GetPartnerByStatId(int.Parse(PartnerStatsID));
                if (partnerData.Star < 6)
                {
                    //添加所需要的伙伴碎片
                    AddItemToInv(PartnerStatsID, 650 - partnerData.ShardNum, delegate
                    {
                        int starIndex = 6 - partnerData.Star - 1;
                        int holeIndex = 10 - partnerData.StarHole + 1;
                        int count = starIndex * 11 + holeIndex;
                        for (int i = 0; i < count; i++)
                        {
                            if (i == count - 1)
                            {
                                LTPartnerDataManager.Instance.StarUp(partnerData.InfoId, delegate
                                {
                                    PartnerSkillUp();
                                });
                            }
                            else
                            {
                                LTPartnerDataManager.Instance.StarUp(partnerData.InfoId);
                            }
                        }
                    });
                }
                else
                {
                    PartnerSkillUp();
                }

                PartnerLevelUp();
                PartnerEquipAllAndUp();
            });
        }

        /// <summary>
        /// 召唤伙伴
        /// </summary>
        /// <param name="callback"></param>
        private void SummonPartner(System.Action<bool> callback = null)
        {
            if (string.IsNullOrEmpty(PartnerStatsID))
            {
                EB.Debug.LogError("弄啥捏，连id都不输你想干嘛子捏！！！");
                return;
            }

            int partnerStatsId = 0;
            if (!int.TryParse(PartnerStatsID, out partnerStatsId))
            {
                EB.Debug.LogError("弄啥捏，数字都不会输了吗！！！");
                return;
            }

            HeroStatTemplate info = CharacterTemplateManager.Instance.GetHeroStat(partnerStatsId);
            if (info == null)
            {
                EB.Debug.LogError("弄啥捏，哪有这个id，我允许你重新组织下语言！！！");
                return;
            }

            LTPartnerData partnerData = LTPartnerDataManager.Instance.GetPartnerByStatId(partnerStatsId);
            if (partnerData == null || partnerData.HeroId <= 0)
            {
                //添加碎片
                AddItemToInv(PartnerStatsID, 60, delegate
                {
                    //召唤伙伴
                    LTPartnerDataManager.Instance.SummonBuddy(partnerStatsId, delegate
                    {
                        LTPartnerDataManager.Instance.InitPartnerData();
                        if (callback != null)
                        {
                            callback(true);
                        }
                    });
                });
            }
            else
            {
                callback(false);
            }

        }

        /// <summary>
        /// 召唤所有伙伴
        /// </summary>
        /// <param name="callback"></param>
        private void SummonAllPartner(System.Action<bool> callback = null)
        {
            HeroStatTemplate[] allPartnerCollection = CharacterTemplateManager.Instance.GetHeroStats();
            for (int i = 0; i < allPartnerCollection.Length; i++)
            {
                int id = allPartnerCollection[i].id;
                if (id / 10000 > 1 || id / 10000 < 1)
                {
                    continue;
                }

                LTPartnerData partnerData = LTPartnerDataManager.Instance.GetPartnerByStatId(id);
                if (partnerData != null && partnerData.HeroId > 0)
                {
                    continue;
                }

                //添加碎片
                AddItemToInv(id.ToString(), 700, delegate
                {
                    //召唤伙伴
                    LTPartnerDataManager.Instance.SummonBuddy(id, delegate
                    {
                        LTPartnerDataManager.Instance.InitPartnerData();
                        if (callback != null)
                        {
                            callback(true);
                        }
                    });
                });
            }
        }

        /// <summary>
        /// 清空背包所有的装备
        /// </summary>
        private void ClearAllEquipInBag(bool isClearLevelMax = false)
        {
            Hashtable inventoryData;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("inventory", out inventoryData);
            if (inventoryData == null)
            {
                return;
            }

            foreach (var item in inventoryData.Cast<DictionaryEntry>())
            {
                var va = item.Value as IDictionary;
                if (va["system"].ToString().Equals("Equipment"))
                {
                    if (int.Parse(va["currentLevel"].ToString()) >= 15 && !isClearLevelMax)
                    {
                        continue;
                    }

                    if (!va["location"].ToString().Equals("equipment"))
                    {
                        RemoveItem(item.Key.ToString(), 1);
                    }
                }
            }
        }

        private void SetBuddyExp(int vaule, System.Action<Response> callback = null)
        {
            SetResRPC("buddy-exp", vaule, callback);
        }

        private void SetGold(int vaule, System.Action<Response> callback = null)
        {
            SetResRPC("gold", vaule, callback);
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="economyId"></param>
        /// <param name="amount"></param>
        /// <param name="callback"></param>
        private void AddItemToInv(string economyId, int amount, System.Action<bool> callback = null)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/gaminventory/debugAdd");

            request.AddData("economyId", economyId);
            request.AddData("num", amount);

            lookupsManager.Service(request, delegate (Response result)
            {
                if (callback != null)
                {
                    callback(result.sucessful);
                }
            }, true);
        }

        /// <summary>
        /// 设置资源
        /// </summary>
        /// <param name="type"></param>
        /// <param name="num"></param>
        /// <param name="callback"></param>
        private void SetResRPC(string type, int num, System.Action<Response> callback = null)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/userres/debugSetRes");
            request.AddData("type", type);
            request.AddData("num", num);
            lookupsManager.Service(request, callback);
        }

        /// <summary>
        /// 移除背包的物品
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="amount"></param>
        /// <param name="callback"></param>
        private void RemoveItem(string inventoryId, int amount, System.Action<bool> callback = null)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            //EB.Sparx.Request request = lookupsManager.EndPoint.Post("/gaminventory/sell");
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/gaminventory/debugRemove");
            request.AddData("inventoryId", inventoryId);
            request.AddData("num", amount);

            lookupsManager.Service(request, delegate (Response result)
            {
                if (callback != null)
                {
                    callback(result.sucessful);
                }
            }, true);
        }

        /// <summary>
        /// 根据物品Id获取服务器的物品唯一id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        private string GetServerGoodsId(string economy_id, int minCount = 0)
        {
            Hashtable inventoryData;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("inventory", out inventoryData);
            if (inventoryData == null)
            {
                return "";
            }

            string key = "";
            foreach (var item in inventoryData.Cast<DictionaryEntry>())
            {
                var va = item.Value as IDictionary;
                if (economy_id.Equals(va["economy_id"].ToString()))
                {
                    if (va["system"].ToString().Equals("Equipment"))
                    {
                        //EB.Debug.LogError("va[location] = {0}, va[currentLevel] = {1}", va["location"], va["currentLevel"]);
                        if (!va["location"].ToString().Equals("equipment") && int.Parse(va["currentLevel"].ToString()) < 15)
                        {
                            //EB.Debug.LogError("item.Key :: {0}", item.Key);
                            key = item.Key.ToString();
                            break;
                        }
                    }
                    else
                    {
                        if (minCount > 0)
                        {
                            int count = int.Parse(va["num"].ToString());
                            if (count >= minCount)
                            {
                                key = item.Key.ToString();
                                break;
                            }
                        }
                        else
                        {
                            key = item.Key.ToString();
                            break;
                        }
                    }
                }
            }

            return key;
        }

        /// <summary>
        /// 升级升阶
        /// </summary>
        private void PartnerLevelGradeUp(int heroId, int count)
        {
            if (count <= 0)
            {
                return;
            }

            LTPartnerDataManager.Instance.LevelUp(heroId, 10, delegate
            {
                LTPartnerDataManager.Instance.UpGrade(heroId, delegate
                {
                    if (count > 0)
                    {
                        PartnerLevelGradeUp(heroId, count - 1);
                    }
                });
            });
        }
        #endregion

        public bool IsCanGuardLevelUp(LTPartnerData partnerData)
        {
            if (partnerData.Level < LTPartnerDataManager.Instance.GetGuardOpenLevel()) return false;
            GuardHeroData datas_01 = GetLTPartnerDataByID(partnerData.HeroStat.HeroFetter1);
            if (IsCanGuardLevelUp(partnerData.HeroId, datas_01, 1)) return true;
            GuardHeroData datas_02 = GetLTPartnerDataByID(partnerData.HeroStat.HeroFetter2);
            if (IsCanGuardLevelUp(partnerData.HeroId, datas_02, 2)) return true;
            GuardHeroData datas_03 = GetLTPartnerDataByID(partnerData.HeroStat.HeroFetter3);
            if (IsCanGuardLevelUp(partnerData.HeroId, datas_03, 3)) return true;
            return false;
        }


        public bool IsCanGuardLevelUp(int HeroId, GuardHeroData guardHeroData, int index)
        {
            if (guardHeroData.heroLevel == 0) return false;
            bool finish_01 = IsCanGuardGetOrAwakenLevelUp(guardHeroData, index, GetGuardLevel(HeroId, index, 1));
            if (finish_01) return true;
            bool finish_02 = IsCanGuardGradeLevelUp(guardHeroData, index, GetGuardLevel(HeroId, index, 2));
            if (finish_02) return true;
            bool finish_03 = IsCanGuardStarLevelUp(guardHeroData, index, GetGuardLevel(HeroId, index, 3));
            if (finish_03) return true;
            return false;
        }

        public int GetGuardLevel(int HeroId, int index, int condition)
        {
            int num = 0;
            DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.fetter.{1}.{2}", HeroId, index, condition), out num);
            return num;
        }

        public Hashtable GetGuardHash(int HeroId)
        {
            Hashtable hash;
            DataLookupsCache.Instance.SearchHashtableByID(string.Format("heroStats.{0}.fetter", HeroId), out hash);
            return hash;
        }

        private bool IsCanGuardGetOrAwakenLevelUp(GuardHeroData guardHeroData, int index, int level)
        {
            HeroGuardTemplate next = CharacterTemplateManager.Instance.GetGuard(index, 1, level + 1);
            bool finish = false;
            if (next == null)
            {
                EB.Debug.LogError("CharacterTemplateManager.Instance.GetGuard({0}, 1, {1}), the ret value of 'next' fetching failed, please check!", index, level + 1);
                return false;
            }
            if (next.IsMaxLevel == false)
            {

                finish = ((int.Parse(next.Param) == 0) && (guardHeroData.heroLevel > 0)) ||
                         ((int.Parse(next.Param) == 1) && (guardHeroData.isawaken > 0));
            }
            return finish;
        }

        private bool IsCanGuardGradeLevelUp(GuardHeroData guardHeroData, int index, int level)
        {
            HeroGuardTemplate next = CharacterTemplateManager.Instance.GetGuard(index, 2, level + 1);
            bool finish = false;
            if (next == null)
            {
                EB.Debug.LogError("CharacterTemplateManager.Instance.GetGuard({0}, 2, {1}), the ret value of 'next' fetching failed, please check!", index, level + 1);
                return false;
            }
            if (next.IsMaxLevel == false)
            {
                finish = guardHeroData.UpGradeId >= int.Parse(next.Param);
            }
            return finish;
        }

        private bool IsCanGuardStarLevelUp(GuardHeroData guardHeroData, int index, int level)
        {
            HeroGuardTemplate next = CharacterTemplateManager.Instance.GetGuard(index, 3, level + 1);
            bool finish = false;
            if (next == null)
            {
                EB.Debug.LogError("CharacterTemplateManager.Instance.GetGuard({0}, 3, {1}), the ret value of 'next' fetching failed, please check!", index, level + 1);
                return false;
            }
            if (next.IsMaxLevel == false)
            {
                finish = guardHeroData.star >= int.Parse(next.Param);
            }
            return finish;
        }

        #region 分帧刷新伙伴战力
        private int _OnDestineTypePowerChanged_Seq = 0;
        private eRoleAttr _OnDestineTypePowerChanged_type = eRoleAttr.None;
        private System.Action<bool> _OnDestineTypePowerChanged_fn = null;
        private bool _OnDestineTypePowerChanged_fn_param = false;
        private Queue<LTPartnerData> _OnDestineTypePowerChanged_queue_battle = new Queue<LTPartnerData>();
        private Queue<LTPartnerData> _OnDestineTypePowerChanged_queue_others = new Queue<LTPartnerData>();

        private void Update_OnDestineTypePowerChanged(int seq)
        {
            if (_OnDestineTypePowerChanged_queue_battle.Count > 0)
            {//上阵英雄战力刷新
                #region 10ms
                var partnerData = _OnDestineTypePowerChanged_queue_battle.Dequeue();
                if (partnerData.HeroId > 0 && (_OnDestineTypePowerChanged_type == eRoleAttr.None || partnerData.HeroInfo.char_type == _OnDestineTypePowerChanged_type))
                {
                    partnerData.powerData.OnValueChanged(partnerData, false, PowerData.RefreshType.Attribute);
                }
                #endregion

                //飘字回调
                if (_OnDestineTypePowerChanged_queue_battle.Count == 0)
                {
                    _OnDestineTypePowerChanged_fn?.Invoke(_OnDestineTypePowerChanged_fn_param);
                    _OnDestineTypePowerChanged_fn = null;
                }
            }
            else if (_OnDestineTypePowerChanged_queue_others.Count > 0)
            {//其他英雄战力刷新
                #region 10ms
                var partnerData = _OnDestineTypePowerChanged_queue_others.Dequeue();
                if (partnerData.HeroId > 0 && (_OnDestineTypePowerChanged_type == eRoleAttr.None || partnerData.HeroInfo.char_type == _OnDestineTypePowerChanged_type))
                {
                    partnerData.isPowerNeedRefresh = true;
                    //partnerData.powerData.OnValueChanged(partnerData, false, PowerData.RefreshType.Attribute);
                }
                #endregion 
            }
            else
            {
                ILRTimerManager.instance.RemoveTimerSafely(ref _OnDestineTypePowerChanged_Seq);
            }
        }

        ///分帧刷新每个伙伴战力，没刷新结束前，调用被忽略！
        public void OnDestineTypePowerChanged(eRoleAttr type, System.Action<bool> fn, bool param)
        {
            if (_OnDestineTypePowerChanged_Seq != 0)
            {
                ILRTimerManager.instance.RemoveTimerSafely(ref _OnDestineTypePowerChanged_Seq);
            }

            _OnDestineTypePowerChanged_type = type;
            _OnDestineTypePowerChanged_fn_param = param;
            _OnDestineTypePowerChanged_fn = fn;

            #region 装载Queue
            _OnDestineTypePowerChanged_queue_battle.Clear();
            _OnDestineTypePowerChanged_queue_others.Clear();
            int cnt = mPartnerDataList.Count;
            for (int i = 0; i < cnt; i++)
            {
                var pd = mPartnerDataList[i];
                if (pd.IsGoIntoBattle)
                {
                    _OnDestineTypePowerChanged_queue_battle.Enqueue(pd);
                }
                else
                {
                    _OnDestineTypePowerChanged_queue_others.Enqueue(pd);
                }

            }
            #endregion

            //启动Timer
            _OnDestineTypePowerChanged_Seq = ILRTimerManager.instance.AddTimer(1, 0, Update_OnDestineTypePowerChanged);
        }
        #endregion


        /// <summary>
        /// 是否有上阵伙伴可以升级
        /// </summary>
        public bool HasPartnerCanLevelUp()
        {
            bool has = false;

            if (Instance != null)
            {
                has = Instance.IsCanLevelUp();
            }

            return has;
        }

        /// <summary>
        /// 上阵伙伴中是否有伙伴可以穿戴装备
        /// </summary>
        public bool HasEquipmentCanDress()
        {
            bool has = false;

            if (Instance != null)
            {
                var heroIds = Instance.GetGoIntoBattleList();

                if (heroIds != null)
                {
                    for (var i = 0; i < heroIds.Count; i++)
                    {
                        var partnerData = Instance.GetPartnerByHeroId(heroIds[i]);

                        if (LTPartnerEquipDataManager.Instance != null)
                        {
                            has = LTPartnerEquipDataManager.Instance.HasAnySuitEquip(partnerData);

                            if (has)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return has;
        }
    }
}