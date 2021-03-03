using Hotfix_LT.UI;
using System.Collections.Generic;

namespace Hotfix_LT.Data
{
    public class AllianceWarReward
    {
        public int Stage;
        public int Result;
        public List<LTShowItemData> Rewards;
    }

    public class AllianceWarConfig
    {
        public string name;
        public float value;
    }

    public class AllianceFBBoss
    {
        public int monsterId;
        public int donate;
        public int challenge;
        public List<LTShowItemData> Rewards;
    }

    public class AllianceFBHurt
    {
        public int id;
        public int hurt;
        public int monsterId;
        public List<LTShowItemData> Rewards;
    }

    public class AllianceDonateChest
    {
        public int id;
        public int score;
        public List<LTShowItemData> Rewards;
    }

    public class AllianceTechSkill
    {
        public int skillid;
        public int charType;
        public int maxLevel;
        public int addtionType;
        public string skillIcon;
        public string skillName;
        public string skilldesTemplate;
        public List<AllianceTechSkillUpLevel> levelinfo;
    }

    public class AllianceTechSkillUpLevel
    {
        public int skillid;
        public int level;
        public float addition;
        public int cost;
    }

    public class AllianceTechChest
    {
        public int level;
        public int goldrate;
        public int goldmax;
        public int exprate;
        public int expmax;
        public int vigorrate;
        public int vigormax;
    }

    public enum TechSkillAddtionType
    {
        ATK = 1,
        DEF = 2,
        MaxHp = 3,
    }

    public class AllianceTemplateManager
    {
        private static AllianceTemplateManager s_instance;
        public static AllianceTemplateManager Instance
        {
            get { return s_instance = s_instance ?? new AllianceTemplateManager(); }
        }

        public List<AllianceWarReward> mWarRewardList = new List<AllianceWarReward>();
        public List<AllianceWarConfig> mWarConfigList = new List<AllianceWarConfig>();
        public List<AllianceFBBoss> mFBBossList = new List<AllianceFBBoss>();
        public List<AllianceFBHurt> mFBHurtList = new List<AllianceFBHurt>();
        public List<AllianceTechSkill> mTechSkillList = new List<AllianceTechSkill>();
        public List<AllianceDonateChest> mDonateChestList = new List<AllianceDonateChest>();
        public List<AllianceTechChest> mTechChestList = new List<AllianceTechChest>();
        private List<AllianceTechSkillUpLevel> mTechSkillUpLevelList = new List<AllianceTechSkillUpLevel>();

        public static void ClearUp()
        {
            if (s_instance != null)
            {
                s_instance.mWarRewardList.Clear();
                s_instance.mWarConfigList.Clear();
                s_instance.mFBBossList.Clear();
                s_instance.mFBHurtList.Clear();
                s_instance.mTechSkillList.Clear();
                s_instance.mDonateChestList.Clear();
                s_instance.mTechChestList.Clear();
            }
        }

        public bool InitFromDataCache(GM.DataCache.Alliance alliance)
        {
            if (alliance == null)
            {
                EB.Debug.LogError("InitFromDataCache: alliance is null");
                return false;
            }

            var conditionSet = alliance.GetArray(0);

            if (!InitAllianceWarReward(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init alliance war reward failed");
                return false;
            }

            if (!InitAllianceWarConfig(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init alliance war config failed");
                return false;
            }

            if (!InitAllianceFBBoss(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init alliance FBBoss failed");
                return false;
            }
            if (!InitAllianceFBHurt(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init alliance FBHurt failed");
                return false;
            }
            if (!InitAllianceTechSkillUpLevel(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init alliance TechSkillUpLevel failed");
                return false;
            }
            if (!InitAllianceTechSkill(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init alliance TechSkill failed");
                return false;
            }
            if (!InitAllianceTechChest(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init alliance TechChest failed");
                return false;
            }
            if (!InitAllianceDonateChest(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init alliance DonateChest failed");
                return false;
            }
            return true;
        }

        private bool InitAllianceWarReward(GM.DataCache.ConditionAlliance tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitAllianceWarReward:tbl is null");
                return false;
            }

            mWarRewardList = new List<AllianceWarReward>();
            for (int i = 0; i < tbl.AllianceWarRewardLength; i++)
            {
                AllianceWarReward tpl = ParseWarRewardInfo(tbl.GetAllianceWarReward(i));
                if (tpl != null) mWarRewardList.Add(tpl);
            }

            return true;
        }

        private AllianceWarReward ParseWarRewardInfo(GM.DataCache.AllianceWarReward obj)
        {
            if (obj == null) return null;
            AllianceWarReward tp1 = new AllianceWarReward();
            tp1.Result = obj.Result;
            tp1.Stage = obj.Stage;
            tp1.Rewards = new List<LTShowItemData>();
            if (!string.IsNullOrEmpty(obj.ItemId1)) tp1.Rewards.Add(new LTShowItemData(obj.ItemId1, obj.ItemNum1, obj.ItemType1, false));
            if (!string.IsNullOrEmpty(obj.ItemId2)) tp1.Rewards.Add(new LTShowItemData(obj.ItemId2, obj.ItemNum2, obj.ItemType2, false));
            if (!string.IsNullOrEmpty(obj.ItemId3)) tp1.Rewards.Add(new LTShowItemData(obj.ItemId3, obj.ItemNum3, obj.ItemType3, false));
            if (!string.IsNullOrEmpty(obj.ItemId4)) tp1.Rewards.Add(new LTShowItemData(obj.ItemId4, obj.ItemNum4, obj.ItemType4, false));
            return tp1;
        }


        private bool InitAllianceFBBoss(GM.DataCache.ConditionAlliance tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitAllianceFBBoss:tbl is null");
                return false;
            }

            mFBBossList = new List<AllianceFBBoss>();
            for (int i = 0; i < tbl.AllianceFbBossLength; i++)
            {
                AllianceFBBoss tpl = ParseAllianceFBBoss(tbl.GetAllianceFbBoss(i));
                if (tpl != null) mFBBossList.Add(tpl);
            }

            return true;
        }

        private AllianceFBBoss ParseAllianceFBBoss(GM.DataCache.AlliancesFBBoss obj)
        {
            if (obj == null) return null;
            AllianceFBBoss tp1 = new AllianceFBBoss();
            tp1.monsterId = obj.MonsterId;
            tp1.donate = obj.Donate;
            tp1.challenge = obj.Challenge;
            tp1.Rewards = ParseShowItem(obj.Reward);
            return tp1;
        }

        private bool InitAllianceFBHurt(GM.DataCache.ConditionAlliance tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitAllianceFBHurt:tbl is null");
                return false;
            }

            mFBHurtList = new List<AllianceFBHurt>();
            for (int i = 0; i < tbl.AllianceFbHurtLength; i++)
            {
                AllianceFBHurt tpl = ParseAllianceFBHurt(tbl.GetAllianceFbHurt(i));
                if (tpl != null) mFBHurtList.Add(tpl);
            }

            return true;
        }

        private AllianceFBHurt ParseAllianceFBHurt(GM.DataCache.AlliancesFBHurt obj)
        {
            if (obj == null) return null;
            AllianceFBHurt tp1 = new AllianceFBHurt();
            tp1.id = obj.Id;
            tp1.hurt = obj.Hurt;
            tp1.monsterId = obj.BossId;
            tp1.Rewards = ParseShowItem(obj.Reward);
            return tp1;
        }
        private bool InitAllianceTechSkill(GM.DataCache.ConditionAlliance tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitAllianceTechSkill:tbl is null");
                return false;
            }

            if (mTechSkillList == null)
            {
                mTechSkillList = new List<AllianceTechSkill>();
            }
            else
            {
                mTechSkillList.Clear();
            }
            for (int i = 0; i < tbl.TechSkillLength; i++)
            {
                AllianceTechSkill tpl = ParseAllianceTechSkill(tbl.GetTechSkill(i));
                if (tpl != null)
                {
                    List<AllianceTechSkillUpLevel> data = GetTechSkillUpLevelListById(tpl.skillid);
                    if (data != null)
                    {
                        tpl.levelinfo = data;
                    }
                    mTechSkillList.Add(tpl);
                }
            }
            return true;
        }

        private AllianceTechSkill ParseAllianceTechSkill(GM.DataCache.TechSkill obj)
        {
            if (obj == null) return null;
            AllianceTechSkill tpl = new AllianceTechSkill();
            tpl.skillid = obj.SkillId;
            tpl.charType = obj.CharType;
            tpl.maxLevel = obj.LevelLimit;
            tpl.addtionType = obj.AdditionType;
            tpl.skillIcon = obj.SkillIcon;
            tpl.skillName = EB.Localizer.GetTableString(string.Format("ID_alliances_tech_skill_{0}_skill_name", obj.SkillId), obj.SkillName); //obj.SkillName;
            tpl.skilldesTemplate = EB.Localizer.GetTableString(string.Format("ID_alliances_tech_skill_{0}_skill_des", obj.SkillId),obj.SkillDes);
            return tpl;
        }



        private bool InitAllianceTechSkillUpLevel(GM.DataCache.ConditionAlliance tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitAllianceTechSkillUpLevel:tbl is null");
                return false;
            }

            if (mTechSkillUpLevelList == null)
            {
                mTechSkillUpLevelList = new List<AllianceTechSkillUpLevel>();
            }
            else
            {
                mTechSkillUpLevelList.Clear();
            }
            for (int i = 0; i < tbl.TechSkillLevelUpLength; i++)
            {
                AllianceTechSkillUpLevel data = ParseAllianceSkillUplevel(tbl.GetTechSkillLevelUp(i));
                if (data != null)
                {
                    mTechSkillUpLevelList.Add(data);
                }
            }
            return true;
        }

        private AllianceTechSkillUpLevel ParseAllianceSkillUplevel(GM.DataCache.TechSkillLevelUp obj)
        {
            if (obj == null) return null;
            AllianceTechSkillUpLevel data = new AllianceTechSkillUpLevel();
            data.skillid = obj.SkillId;
            data.level = obj.Level;
            data.addition = obj.Addition;
            data.cost = obj.Cost;
            return data;
        }

        private bool InitAllianceTechChest(GM.DataCache.ConditionAlliance tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitAllianceTechChest:tbl is null");
                return false;
            }

            if (mTechChestList == null)
            {
                mTechChestList = new List<AllianceTechChest>();
            }
            else
            {
                mTechChestList.Clear();
            }
            for (int i = 0; i < tbl.TechLevelChestLength; i++)
            {
                AllianceTechChest tpl = ParseAllianceTechChest(tbl.GetTechLevelChest(i));
                if (tpl != null)
                {
                    mTechChestList.Add(tpl);
                }
            }
            return true;
        }

        private AllianceTechChest ParseAllianceTechChest(GM.DataCache.TechLevelChest obj)
        {
            if (obj == null) return null;
            AllianceTechChest data = new AllianceTechChest();
            data.level = obj.Level;
            data.goldrate = obj.GoldRate;
            data.goldmax = obj.GoldMax;
            data.exprate = obj.ExpRate;
            data.expmax = obj.ExpMax;
            data.vigormax = obj.VigorMax;
            data.vigorrate = obj.VigorRate;
            return data;
        }

        private bool InitAllianceDonateChest(GM.DataCache.ConditionAlliance tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitAllianceDonateChest:tbl is null");
                return false;
            }

            if (mDonateChestList == null)
            {
                mDonateChestList = new List<AllianceDonateChest>();
            }
            else
            {
                mDonateChestList.Clear();
            }
            for (int i = 0; i < tbl.DonateChestLength; i++)
            {
                AllianceDonateChest tpl = ParseAllianceDonateChest(tbl.GetDonateChest(i));
                if (tpl != null)
                {
                    mDonateChestList.Add(tpl);
                }
            }
            return true;
        }

        private AllianceDonateChest ParseAllianceDonateChest(GM.DataCache.DonateChest obj)
        {
            if (obj == null) return null;
            AllianceDonateChest data = new AllianceDonateChest();
            data.id = obj.Id;
            data.score = obj.Score;
            data.Rewards = ParseShowItem(obj.Reward);
            return data;
        }
        private List<AllianceTechSkillUpLevel> GetTechSkillUpLevelListById(int skillId)
        {
            if (mTechSkillUpLevelList == null) return null;
            List<AllianceTechSkillUpLevel> upleveldata = new List<AllianceTechSkillUpLevel>();
            for (int i = 0; i < mTechSkillUpLevelList.Count; i++)
            {
                if(mTechSkillUpLevelList[i].skillid == skillId)
                {
                    upleveldata.Add(mTechSkillUpLevelList[i]);
                }
            }
           return upleveldata;
        }

        public AllianceTechChest GetAllianceTechChestInfoByLevel(int level)
        {
            for (int i = 0; i < mTechChestList.Count; i++)
            {
                if(mTechChestList[i].level == level)
                {
                    return mTechChestList[i];
                }
            }
            return null;
        }

        private List<LTShowItemData> ParseShowItem(string itemStr)
        {
            if (itemStr == null) return null;
            List<LTShowItemData> itemList = new List<LTShowItemData>();
            string[] itemData = itemStr.Split(';');
            for (int i = 0; i < itemData.Length; i++)
            {
                string[] item = itemData[i].Split(',');
                itemList.Add(new LTShowItemData(item[0], int.Parse(item[1]), item[2]));
            }
            return itemList;
        }

        private bool InitAllianceWarConfig(GM.DataCache.ConditionAlliance tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitAllianceWarReward:tbl is null");
                return false;
            }

            mWarConfigList = new List<AllianceWarConfig>();
            for (int i = 0; i < tbl.AllianceWarConfigLength; i++)
            {
                AllianceWarConfig tpl = ParseWarConfigInfo(tbl.GetAllianceWarConfig(i));
                if (tpl != null) mWarConfigList.Add(tpl);
            }

            return true;
        }

        private AllianceWarConfig ParseWarConfigInfo(GM.DataCache.AllianceWarConfig obj)
        {
            if (obj == null) return null;
            AllianceWarConfig tp1 = new AllianceWarConfig();
            tp1.name = obj.Name;
            tp1.value = obj.Value;
            return tp1;
        }

        public AllianceWarReward GetWarReward(int stage, int result)
        {
            AllianceWarReward tp = new AllianceWarReward();
            for (int i = 0; i < mWarRewardList.Count; i++)
            {
                if (mWarRewardList[i].Stage == stage && mWarRewardList[i].Result == result) tp = mWarRewardList[i];
            }
            return tp;
        }

        public float GetWarConfigValue(string name)
        {
            float tp = 0;
            for (int i = 0; i < mWarConfigList.Count; i++)
            {
                if (mWarConfigList[i].name == name) tp = mWarConfigList[i].value;
            }
            return tp;
        }

    }
}