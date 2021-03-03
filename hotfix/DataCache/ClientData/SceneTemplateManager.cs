using Hotfix_LT.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;
using GM.DataCache;

#region UnUsed

public enum eChapterType
{
    None = 0,
    Normal = 1,
    Hero = 2
}

public class CampaignTemplate
{
    public string campaign_name;
    public int locator;
    public string icon;
    //public int mode;
    public int chapter;
    public string display_name;
    //public string map_name;
    public string born_locator;
    public string obstacle;
    public string combat_map;
    public string trigger_dialogue;
    public int end_dialogue;
    public string map_desc;
    //public string encounter_group;
    //public string interact_group;
    //public string back_ground_music;
    //public string boss_battle_music;
    public int campaign_type;
    //public int time_limit;
    public int challenge_num;
    public int buy_times;
    public int buy_price;
    public int cost_vigor;
    //public int recommend_level;
    public int recommend_fight;
    public int level_limit;
    //public string task_limit;
    //public string campaign_limit_1;
    //public string campaign_limit_2;
    //public string next_campaign;
    //public string perfect_pass_condition;
    //public string monster_icon;
    public string award_icon;
    //public int hero_exp;
    //public int partner_exp;
    //public int award_gold;
    //public int award_skill_point;
    //public string[] awawrd_items;
    //public string[] once_award_items;
    //public string[] blitz_reward_items;
}

public class AllianceCampaignTemplate
{
    public string campaign_name;
    public int locator;
    public string display_name;
    public string born_locator;
    public string combat_map;
    public string trigger_dialogue;
    public int end_dialogue;
    public int alli_level_limit;
    public int unlock_alli_balance;
    public string show_monster;
    public string boss_desc;
    public string boss_factor;
    public string boss_icon;
    public int award_alli_gold;
    public List<LTShowItemData> award_items;
}

public class Redeem
{
    public string t;
    public string n;
    public int q;
    public Redeem(string t, string n, int q)
    {
        this.t = t;
        this.n = n;
        this.q = q;
    }
}

public class EncounterTemplate
{
    public string encounter_name;
    public string campaign_name;
    public string locator;
    public string combat_layout_name;
    public string display_name;
    public string encounter_prefab;
    public string script;
    public string role;
    public int encounter_appearing_way;
    public int is_appearing;
    public int victory_dialogue;
    public Redeem[] drop;
    public string param1;
    public string param2;
}

public class AllianceEncounterTemplate
{
    public string encounter_name;
    public string campaign_name;
    public string locator;
    public string combat_layout_name;
    public string display_name;
    public string encounter_prefab;
    public string script;
    public string role;
    public int encounter_appearing_way;
    public int is_appearing;
    public int victory_dialogue;
    //public Redeem[] drop;
    public string param1;
    public string param2;
}

public class InteractTemplate
{
    public string interact_Name;
    //public string encounter_group;
    //public string locator;
    //public string encounter_prefab;
    //public string script;
    //public string role;
    //public string interact_type;
}

public class ChapterTemplate
{
    public int name;
    public int number;
    public string display_name;
    public int type;
    //public string first_campaign_name;
    //public int level_count;
    //public string chapter_map;
    //public int total_stars;
    public int hc;
    public ResourceContainer chapter_awards;
    public string hero_shard;
    public int shard_count;
}



public class MainlandsGhostTemplate
{
    public string mainland_name;
    public string locator;
    public string combat_layout_name;
    public string display_name;
    public string encounter_prefab;
    public string script;
    public string role;
}

public class MainlandsGhostRewardTemplate
{
    public int id;
    public string type;
    public List<LTShowItemData> reward;
}

public class CombatMapTemplate
{
    public string name;
}

#endregion

namespace Hotfix_LT.Data
{
    public class LayoutTemplate
    {
        public string combat_layout_name { get { return obj.CombatLayoutName; } }
        public int wave { get { return obj.Wave; } }
        public string f1 { get { return obj.F1; } }
        public string f2 { get { return obj.F2; } }
        public string f3 { get { return obj.F3; } }
        public string m1 { get { return obj.M1; } }
        public string m2 { get { return obj.M2; } }
        public string m3 { get { return obj.M3; } }
        public string b1 { get { return obj.B1; } }
        public string b2 { get { return obj.B2; } }
        public string b3 { get { return obj.B3; } }
        public string Model { get { return obj.Model; } }
        public GM.DataCache.LayoutInfo obj;
    }
    /*public class NewLayoutTemplate//用于方便查看敌方阵容
    {
        public NewLayoutTemplate(string name, List<int> ids)
        {
            combat_layout_name = name;
            MonsterInfoid = ids;
        }
        public string combat_layout_name;
        public List<int> MonsterInfoid = new List<int>();
    }*/


    /// <summary>
    /// 主线章节分页
    /// </summary>
    public class LostMainLandTemplate
    {
        public string Id;
        public string Name;
        //public Vector2 LandPos;
        public string LineName;
        public List<string> ChapterList;
        //public Vector2 LandSize;
    }

    public class MainLandTemplate
    {
        public string mainland_name;
        //public string locator;
        //public string icon;
        public string display_name;
        //public string map_name;
        public string born_locator;
        public string combat_map;
        public string trigger_dialogue;
        public string transfer_points;
        //public string map_desc;
        //public string encounter_group;
        //public string interact_group;
        //public string back_ground_music;
        //public string boss_battle_music;
    }

    public class MainLandEncounterTemplate
    {
        public string mainland_name;
        public string locator;
        //public string random_locators;
        public string combat_layout_name;
        public string display_name;
        public string encounter_prefab;
        public string script;
        public string role;
        public int encounter_appearing_way;
        public int is_appearing;
        public int func_id_1;
        public int func_id_2;
        public int func_id_3;
        //public string header_icon;
        public int dialogue_id;
        //public string is_load;
    }

    public class LostMainChapterTemplate
    {
        public string Id;
        public int LevelLimit;
        public int Limitparam1;
        public int Limitparam2;
        public string ForwardChapterId;
        public string Name;
        public string LandId;
        public Vector2 ChapterPos;
        public string ChapterBg;
        public List<string> CampaignList;
        public Dictionary<int, List<LTShowItemData>> RewardDataDic;
        public string AwardIcon1;
        public string AwardIcon2;
        public string AwardIcon3;
        public string MaskBg;
        public string Icon;
        public string BeforeChapter;
        public string AfterChapter;

        public bool IsBoxRewardType()
        {
            return string.IsNullOrEmpty(MaskBg);
        }
    }

    public class LostChallengeChapter
    { 
        /// <summary>关卡</summary>
        public int Level;
        /// <summary>是否记录层</summary>
        public bool IsCheckPoint;
        /// <summary>是否固定地图</summary>
        public bool IsFixMap;
        /// <summary>是否boss层</summary>
        public bool IsBoss;

        /// <summary>当前层数</summary>
        public int CurChapter;
        /// <summary>当前关卡数</summary>
        public int CurLevel;

        /// <summary> 推荐等级</summary>
        public int RecommendLevel;
    }

    public class LostChallengeWalls
    {
        public enum WallPos
        {
            None,
            Top,
            Bottom,
            Left,
            Right,
            TopBottom,
            LeftRight,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            TopBottomLeft,
            TopBottomRight,
            TopLeftRight,
            BottomLeftRight,
            TopBottomLeftRight,
        }

        public int Id;
        public string Type;
        public WallPos Pos;
        public string Img;
        public Vector3 Rotation;
    }

    public class LostCombatMapTemplate
    {
        public eBattleType Type;
        public string[] SceneList;
    }

    public class LostChallengeStyleTemplate
    {
        public int Id;
        public int TopWell;
        public int NormalTerra;
        public string MapBg;
        public string MapMask;
        public string CombatScene;
    }

    public class LostChallengeRewardTemplate
    {
        public DayOfWeek Week;
        public List<string> DropList;
        public int Floor;
        public float DropRate;
    }

    public class LostMainCampaignsTemplate
    {
        public string ChapterId;
        public string Id;
        public string Name;
        public int RecommendLevel;
        public int PartnerExp;
        public int BlitzPartnerExp;
        public int HeroExp;
        public int CostVigor;
        public int AwardGold;
        public string MapName;
        public string Desc;
        public MainLineNodeType Type;
        public string ModelName;
        public List<string> PreCampaigns;
        public List<string> NextCampaigns;
        public string EncounterGroupId;
        public string StoryId;
        public string RewardId;
        public Vector2 CampaignPos;
        public List<string> AwardIconList;
        public int orderid;
        public int totalcampaign;
    }

    public class LostChallengeChapterRole
    {
        public int Id;

        public string Name;

        public int Type;

        public string Img;

        public string Model;

        public string OtherModel;

        public float ModelScale;

        public string Order;

        public List<string> Params = new List<string>();

        public List<string> Tag = new List<string>();

        public string Group;

        public int KeyId;

        public Vector3 Rotation;

        public Vector2 Span;

        public Vector3 Offset;

        public bool IsCorrelation;

        public string[] Guide;

        public LostChallengeChapterRole(GM.DataCache.LostChallengeChapterRole obj)
        {
            this.Id = obj.Id;
            this.Name = obj.Desc;
            this.Type = obj.Type;
            this.Img = obj.Img?? "";
            this.Order = obj.Order;
            if (!string.IsNullOrEmpty(this.Order) && this.Order.Equals("Treasure"))
            {
                Main.DataCache.SceneTemplateManagerHelper.LostChallengeChapterRole_ParseModel(obj.Model, out this.OtherModel, out this.ModelScale);
            }
            else
            {
                Main.DataCache.SceneTemplateManagerHelper.LostChallengeChapterRole_ParseModel(obj.Model, out this.Model, out this.ModelScale);
            }

            for (int i = 0; i < obj.ParamLength; i++)
            {
                this.Params.Add(obj.GetParam(i));
            }

            for (int i = 0; i < obj.TagLength; i++)
            {
                this.Tag.Add(obj.GetTag(i));
            }

            this.Group = obj.Group;
            this.KeyId = obj.Keyid;
            int id = this.Id;

            this.Rotation = Main.DataCache.SceneTemplateManagerHelper.LostChallengeChapterRole_ParseRotation(id, obj.Rotation);
            this.Span = Main.DataCache.SceneTemplateManagerHelper.LostChallengeChapterRole_ParseSpan(id, obj.Span);
            this.Offset = Main.DataCache.SceneTemplateManagerHelper.LostChallengeChapterRole_ParseOffset(id, obj.Offset);
            this.Guide = Main.DataCache.SceneTemplateManagerHelper.LostChallengeChapterRole_ParseGuide(id, obj.Guide)?? this.Guide;

            if (obj.ZOffset > 0)
            {
                Vector2 temp= Main.DataCache.SceneTemplateManagerHelper.LostChallengeChapterRole_ParseOffset(id, obj.Offset);
                this.Offset = new Vector3(temp.x, temp.y, obj.ZOffset);
            }
            else
            {
                this.Offset = Main.DataCache.SceneTemplateManagerHelper.LostChallengeChapterRole_ParseOffset(id, obj.Offset);
            }

            this.IsCorrelation = !string.IsNullOrEmpty(obj.Correlation);
        }
    }

    public class LostChallengeChapterElement
    {
        public int Id;

        public string Desc;

        public int Type;

        public int Color;

        public string Img;

        public bool CanPass;

        public string Group;

        public int RandomGroup;

        public float RP;

        public Vector3 Rotation;
    }

    public class LostChallengeEnv
    {
        public int Id;
        public string Name;

        public string EnvLogic;

        public string Desc;
        public string Pic;
        public string Icon;
    }

    public class LostInstanceMessage
    {
        public int Id;

        public int MsgId;

        public string Icon;

        public string Name;

        public string Desc;
    }


    public class AlienMazeTemplate
    {
        public int Id;

        public string Name;

        public string Icon;

        public string Limit;

        public List<LTShowItemData> Reward;

        //public string Env;

        //public string FixMap;

        public string MapStyle;
    }

    public class MonopolyTemplate
    {
        public int id;

        //public string FixMap;

        public string MapStyle;

        public List<LTShowItemData> Reward;

    }

    public class SceneTemplateManager
    {
        private static SceneTemplateManager sInstance = null;

        #region 没再使用到的,MH的
        private Dictionary<string, CampaignTemplate> mCampaignTbl = null;
        private Dictionary<string, AllianceCampaignTemplate> mAllianceCampaignTbl = null;
        private Dictionary<string, EncounterTemplate> mEncounterTbl = null;
        private Dictionary<string, AllianceEncounterTemplate> mAllianceEncounterTbl = null;
        private Dictionary<string, InteractTemplate> mInteractTbl = null;
        private Dictionary<int, Dictionary<int, ChapterTemplate>> mChapterTbl = null;
        private Dictionary<int, List<CampaignTemplate>> mChapterCampaigns = null;
        private List<CombatMapTemplate> mCombatMapTbl = null;
        #endregion

        private ConditionScene conditionSet;

        private Dictionary<string, LayoutTemplate> mLayoutTbl = new Dictionary<string, LayoutTemplate>();
        private Dictionary<string, MainLandTemplate> mMainLandTbl = null;
        private Dictionary<string, MainLandEncounterTemplate> mMainLandEncounterTbl = null;
        private Dictionary<string, MainlandsGhostTemplate> mMainlandsGhostTbl = null;
        private Dictionary<string, MainlandsGhostRewardTemplate> mMainlandsGhostRewardTbl = null;

        private Dictionary<string, LostMainLandTemplate> mLostMainLandTpl = new Dictionary<string, LostMainLandTemplate>();
        private Dictionary<string, LostMainChapterTemplate> mLostMainChapterTpl = new Dictionary<string, LostMainChapterTemplate>();
        private Dictionary<string, LostMainCampaignsTemplate> mLostMainCampaignsTpl = new Dictionary<string, LostMainCampaignsTemplate>();
        private Dictionary<int, LostChallengeChapter> mLostChallengeChapterTpl = new Dictionary<int, LostChallengeChapter>();
        private Dictionary<string, List<LostChallengeWalls>> mLostChallengeWallTpl = new Dictionary<string, List<LostChallengeWalls>>();
        private Dictionary<eBattleType, LostCombatMapTemplate> mLostCombatMapTpl = new Dictionary<eBattleType, LostCombatMapTemplate>();
        private Dictionary<int, LostChallengeStyleTemplate> mLostChallengeStyleTpl = new Dictionary<int, LostChallengeStyleTemplate>();
        private List<int> TopBlockTerraList = new List<int>();

        public Dictionary<DayOfWeek, List<LostChallengeRewardTemplate>> mLostChallengeRewardTpl = new Dictionary<DayOfWeek, List<LostChallengeRewardTemplate>>();
        public int LostChallengeRewardMaxFloor = 0;

        private Dictionary<int, LostChallengeChapterRole> mLostChallengeChapterRoleTpl = new Dictionary<int, LostChallengeChapterRole>();
        private Dictionary<int, LostChallengeChapterElement> mLostChallengeChapterElementTpl = new Dictionary<int, LostChallengeChapterElement>();
        private Dictionary<int,List<LostChallengeChapterElement>> mRandomGroupElementTpl = new Dictionary<int, List<LostChallengeChapterElement>>();
        private Dictionary<int, LostChallengeEnv> mLostChallengeEnvTpl = new Dictionary<int, LostChallengeEnv>();
        private Dictionary<int, List<LostInstanceMessage>> mLostInstanceMessageTpl = new Dictionary<int, List<LostInstanceMessage>>();

        private List<AlienMazeTemplate> mAlienMazeTpl = new List<AlienMazeTemplate>();
        private List<MonopolyTemplate> mMonopolyTpl = new List<MonopolyTemplate>();

        public static SceneTemplateManager Instance
        {
            get { return sInstance = sInstance ?? new SceneTemplateManager(); }
        }

        public static void ClearUp()
        {
            if (sInstance != null)
            {
                sInstance.mLayoutTbl.Clear();
                sInstance.mMainLandTbl.Clear();
                sInstance.mMainLandEncounterTbl.Clear();
                sInstance.mMainlandsGhostTbl.Clear();
                sInstance.mMainlandsGhostRewardTbl.Clear();
                sInstance.mLostMainLandTpl.Clear();
                sInstance.mLostMainChapterTpl.Clear();
                sInstance.mLostMainCampaignsTpl.Clear();
                sInstance.mLostChallengeChapterTpl.Clear();
                sInstance.mLostChallengeWallTpl.Clear();
                sInstance.mLostCombatMapTpl.Clear();
                sInstance.mLostChallengeStyleTpl.Clear();
                sInstance.TopBlockTerraList.Clear();
                sInstance.mLostChallengeRewardTpl.Clear();
                sInstance.mLostChallengeChapterRoleTpl.Clear();
                sInstance.mLostChallengeChapterElementTpl.Clear();
                sInstance.mLostChallengeEnvTpl.Clear();
                sInstance.mLostInstanceMessageTpl.Clear();
                sInstance.mAlienMazeTpl.Clear();
                sInstance.mMonopolyTpl.Clear();
            }
        }

        public bool InitFromDataCache(GM.DataCache.Scene tbls)
        {
            if (tbls == null)
            {
                EB.Debug.LogError("InitFromDataCache: tbls is null");
                return false;
            }

            conditionSet = tbls.GetArray(0);
            
            if (!InitLayoutTbl(conditionSet))
            {
               EB.Debug.LogError("InitFromDataCache: init layout table failed");
               return false;
            }

            if (!InitMainLandTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init mainland table failed");
                return false;
            }

            if (!InitMainLandEncounterTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init mainland encounter table failed");
                return false;
            }

            if (!InitLostMainChapterTpl(conditionSet))
            {
                EB.Debug.LogError("InitLostMainChapterTpl: init lostMainChapter table failed");
                return false;
            }

            if (!InitLostMainCampaignsTpl(conditionSet))
            {
                EB.Debug.LogError("InitLostMainCampaignsTpl: init lostMainCampaignsTpl table failed");
                return false;
            }

            if (!InitLostChallemgeChapterTpl(conditionSet))
            {
                EB.Debug.LogError("InitLostChallemgeChapterTpl: init lostChallemgeChapterTpl table failed");
                return false;
            }

            if (!InitLostChallengeWallsTpl(conditionSet))
            {
                EB.Debug.LogError("InitLostChallengeWallsTpl: init LostChallengeWalls table failed");
                return false;
            }

            if (!InitLostCombatMaps(conditionSet))
            {
                EB.Debug.LogError("InitLostCombatMaps: init LostCombatMaps table failed");
                return false;
            }

            if (!InitMainlandsGhostTbl(conditionSet))
            {
                EB.Debug.LogError("InitMainlandsGhostTbl: init MainlandsGhostTbl table failed");
                return false;
            }

            if (!InitLostMainLandTpl(conditionSet))
            {
                EB.Debug.LogError("InitLostMainLandTpl: init MainlandsGhostTbl table failed");
                return false;
            }

            if (!InitMainlandsGhostRewardTbl(conditionSet))
            {
                EB.Debug.LogError("InitMainlandsGhostRewardTbl: init MainlandsGhostRewardTbl table failed");
                return false;
            }

            if (!InitLostChallengeStyles(conditionSet))
            {
                EB.Debug.LogError("InitLostChallengeStyles: init LostChallengeStyle table failed");
                return false;
            }

            if (!InitLostChallengeRewards(conditionSet))
            {
                EB.Debug.LogError("InitLostChallengeRewards: init lostChallengeReward table failed");
                return false;
            }

            if (!InitLostChallengeChapterElement(conditionSet))
            {
                EB.Debug.LogError("InitLostChallengeChapterElement: init LostChallengeChapterElement table failed");
                return false;
            }

            if (!InitLostChallengeEnv(conditionSet))
            {
                EB.Debug.LogError("InitLostChallengeEnv: init LostChallengeEnv table failed");
                return false;
            }

            if (!InitLostInstanceMessage(conditionSet))
            {
                EB.Debug.LogError("InitLostInstanceMessage: init LostInstanceMsg table failed");
                return false;
            }

            if (!InitAlienMaze(conditionSet))
            {
                EB.Debug.LogError("InitAlienMaze: init AlienMaze table failed");
                return false;
            }
            
            if(!InitMonopoly(conditionSet))
            {
                EB.Debug.LogError("InitMonopoly: init Monopoly table failed");
                return false;
            }

            if (!InitLostChallengeChapterRole(conditionSet))
            {
                EB.Debug.LogError("InitLostChallengeChapterRole: init InitLostChallengeChapterRole table failed");
                return false;
            }

            return true;
        }

        private bool InitLayoutTbl(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitLayoutTbl: layout tbl is null");
                return false;
            }

            int len = tbl.CombatLayoutLength;
            mLayoutTbl = new Dictionary<string, LayoutTemplate>(len);
            for (int i = 0; i < len; ++i)
            {
                GM.DataCache.LayoutInfo obj = tbl.GetCombatLayout(i);
                LayoutTemplate tpl = new LayoutTemplate()
                {
                    obj = obj
                };
                string key = $"{tpl.combat_layout_name}|{tpl.wave}";
                mLayoutTbl[key] = tpl;
            }

            return true;
        }

        private bool InitMainlandsGhostTbl(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitMainlandsGhostTbl: tbl is null");
                return false;
            }

            mMainlandsGhostTbl = new Dictionary<string, MainlandsGhostTemplate>();

            for (int i = 0; i < tbl.MainlandsGhostLength; ++i)
            {
                var tpl = ParseMainLandsGhost(tbl.GetMainlandsGhost(i));
                string key = string.Format("{0}|{1}", tpl.mainland_name, tpl.locator);
                if (mMainlandsGhostTbl.ContainsKey(key))
                {
                    EB.Debug.LogError("InitMainlandsGhostTbl: {0} exists", key);
                    mMainlandsGhostTbl.Remove(key);
                }
                mMainlandsGhostTbl.Add(key, tpl);
            }
            return true;
        }
        private MainlandsGhostTemplate ParseMainLandsGhost(GM.DataCache.MainLandsGhost obj)
        {
            MainlandsGhostTemplate tmpl = new MainlandsGhostTemplate();
            tmpl.mainland_name = obj.MainlandName;
            tmpl.locator = obj.Locator;
            tmpl.combat_layout_name = obj.CombatLayoutName;
            tmpl.display_name = EB.Localizer.GetTableString(string.Format("ID_scenes_mainlands_ghost_{0}_display_name", tmpl.combat_layout_name), obj.DisplayName);// "";obj.DisplayName;
            tmpl.encounter_prefab = obj.EncounterPrefab;
            tmpl.script = obj.Script;
            tmpl.role = obj.Role;
            return tmpl;
        }

        private bool InitMainlandsGhostRewardTbl(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitMainlandsGhostRewardTbl: tbl is null");
                return false;
            }

            mMainlandsGhostRewardTbl = new Dictionary<string, MainlandsGhostRewardTemplate>();

            for (int i = 0; i < tbl.MainlandsGhostRewardLength; ++i)
            {
                var tpl = ParseMainlandsGhostRewardTemplate(tbl.GetMainlandsGhostReward(i));
                string key = tpl.type;
                if (mMainlandsGhostRewardTbl.ContainsKey(key))
                {
                    EB.Debug.LogError("InitMainlandsGhostRewardTbl: {0} exists", key);
                    mMainlandsGhostRewardTbl.Remove(key);
                }
                mMainlandsGhostRewardTbl.Add(key, tpl);
            }
            return true;
        }
		
        private MainlandsGhostRewardTemplate ParseMainlandsGhostRewardTemplate(GM.DataCache.MainLandsGhostReward obj)
        {
            MainlandsGhostRewardTemplate tmpl = new MainlandsGhostRewardTemplate();
            tmpl.id = obj.Id;
            tmpl.reward = ParseShowItem(obj.Reward);
            tmpl.type = obj.Type;
            return tmpl;
        }
		
        public List<LTShowItemData> ParseShowItem(string itemStr)
        {		
            List<LTShowItemData> itemList = new List<LTShowItemData>();
			if (string.IsNullOrEmpty(itemStr))
			{
				return itemList;
			}
			string[] itemData = itemStr.Split(';');
            for (int i = 0; i < itemData.Length; i++)
            {
                string[] item = itemData[i].Split(',');
                itemList.Add(new LTShowItemData(item[0], int.Parse(item[1]), item[2], false));
            }
            return itemList;
        }

        private bool InitMainLandTbl(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitMainLandTbl: mainland tbl is null");
                return false;
            }

            mMainLandTbl = new Dictionary<string, MainLandTemplate>(tbl.MainlandsLength);
            for (int i = 0; i < tbl.MainlandsLength; ++i)
            {
                var tpl = ParseMainLand(tbl.GetMainlands(i));
                if (mMainLandTbl.ContainsKey(tpl.mainland_name))
                {
                    EB.Debug.LogError("InitMainLandTbl: {0} exists", tpl.mainland_name);
                    mMainLandTbl.Remove(tpl.mainland_name);
                }
                mMainLandTbl.Add(tpl.mainland_name, tpl);
            }

            return true;
        }

        private MainLandTemplate ParseMainLand(GM.DataCache.Mainland obj)
        {
            MainLandTemplate tpl = new MainLandTemplate();
            tpl.mainland_name = obj.MainlandName;
            tpl.display_name = obj.DisplayName;
            tpl.born_locator = obj.BornLocator;
            tpl.combat_map = obj.CombatMap;
            tpl.trigger_dialogue = obj.TriggerDialogue;
            tpl.transfer_points = obj.TransferPoints;
            return tpl;
        }

        private bool InitMainLandEncounterTbl(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitMainLandEncounterTbl: mainland encounter tbl is null");
                return false;
            }

            mMainLandEncounterTbl = new Dictionary<string, MainLandEncounterTemplate>(tbl.MainlandsEncountersLength);
            for (int i = 0; i < tbl.MainlandsEncountersLength; ++i)
            {
                var tpl = ParseMainLandEncounter(tbl.GetMainlandsEncounters(i));
                string key = string.Format("{0}|{1}", tpl.mainland_name, tpl.locator);
                if (mMainLandEncounterTbl.ContainsKey(key))
                {
                    EB.Debug.LogError("InitMainLandEncounterTbl: {0} exists", key);
                    mMainLandEncounterTbl.Remove(key);
                }
                mMainLandEncounterTbl.Add(key, tpl);
            }

            return true;
        }

        private MainLandEncounterTemplate ParseMainLandEncounter(GM.DataCache.MainlandEncounter obj)
        {
            MainLandEncounterTemplate tpl = new MainLandEncounterTemplate();
            tpl.mainland_name = obj.MainlandName;
            tpl.locator = obj.Locator;
            tpl.combat_layout_name = obj.CombatLayoutName;
            tpl.display_name = EB.Localizer.GetTableString(string.Format("ID_scenes_mainLands_encounters_{0}_{1}_display_name", tpl.mainland_name, tpl.locator), obj.DisplayName); //obj.DisplayName;
            tpl.encounter_prefab = obj.EncounterPrefab;
            tpl.script = obj.Script;
            tpl.role = obj.Role;
            int.TryParse(obj.EncounterAppearingWay, out tpl.encounter_appearing_way);
            int.TryParse(obj.IsAppearing, out tpl.is_appearing);
            tpl.func_id_1 = obj.FuncId1;
            tpl.func_id_2 = obj.FuncId2;
            tpl.func_id_3 = obj.FuncId3;
            //tpl.header_icon = obj.HeadIcon;
            tpl.dialogue_id = obj.DialogueId;
            return tpl;
        }

        private bool InitLostMainLandTpl(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitLostMainLandTpl: tbl is null");
                return false;
            }

            mLostMainLandTpl = new Dictionary<string, LostMainLandTemplate>();
            for (int i = 0; i < tbl.LostMainLandLength; ++i)
            {
                var tpl = ParseLostMainLand(tbl.GetLostMainLand(i));
                string key = tpl.Id;
                if (mLostMainLandTpl.ContainsKey(key))
                {
                    EB.Debug.LogError("InitLostMainLandTpl: {0} exists", key);
                    mLostMainLandTpl.Remove(key);
                }
                mLostMainLandTpl.Add(key, tpl);
            }
            return true;
        }

        private LostMainLandTemplate ParseLostMainLand(GM.DataCache.LostMainLand obj)
        {
            LostMainLandTemplate tpl = new LostMainLandTemplate();
            tpl.Id = obj.Id;
            tpl.Name = obj.Name;
            tpl.LineName = obj.LineName;
            List<string> chapterList = new List<string>();
            for (int i = 0; i < obj.ChapterListLength; i++)
            {
                chapterList.Add(obj.GetChapterList(i));
            }
            tpl.ChapterList = chapterList;
            return tpl;
        }

        /// <summary>
        /// 根据landid获取对应land表
        /// </summary>
        /// <param name="landId"></param>
        /// <returns></returns>
        public LostMainLandTemplate GetLostMainLandTemplateByLandId(string landId)
        {
            var enumerator = mLostMainLandTpl.Values.GetEnumerator();
            while(enumerator.MoveNext())
            {
                LostMainLandTemplate land = enumerator.Current;
                if (land.Id == landId)
                {
                    return land;
                }
            }
           EB.Debug.LogWarning("Could not find land by landId : {0}" , landId);
            return null;
        }

        public LostMainLandTemplate GetLostMainLandTemplateByChapterId(string ChapterId)
        {
            var enumerator = mLostMainLandTpl.Values.GetEnumerator();
            while(enumerator.MoveNext())
            {
                LostMainLandTemplate land = enumerator.Current;
                if (land.ChapterList.Contains(ChapterId))
                {
                    return land;
                }
            }
            EB.Debug.LogWarning("Could not find land by ChapterId : {0}" , ChapterId);
            return null;
        }

        private bool InitLostMainChapterTpl(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitLostMainChapterTpl: tbl is null");
                return false;
            }

            mLostMainChapterTpl = new Dictionary<string, LostMainChapterTemplate>();
            for (int i = 0; i < tbl.LostMainChapterLength; ++i)
            {
                var tpl = ParseLostMainChapter(tbl.GetLostMainChapter(i));
                string key = tpl.Id;
                if (mLostMainChapterTpl.ContainsKey(key))
                {
                    EB.Debug.LogError("InitLostMainChapterTpl: {0} exists", key);
                    mLostMainChapterTpl.Remove(key);
                }
                mLostMainChapterTpl.Add(key, tpl);
            }
            return true;
        }

        private LostMainChapterTemplate ParseLostMainChapter(GM.DataCache.LostMainChapter obj)
        {
            LostMainChapterTemplate tpl = new LostMainChapterTemplate();

            tpl.Id = obj.Id;
            tpl.LevelLimit = obj.LevelLimit;
            tpl.Limitparam1 = obj.LimitParam1;
            tpl.Limitparam2 = obj.LimitParam2;
            tpl.ForwardChapterId = obj.ForwardChapterId;
            tpl.Name = EB.Localizer.GetTableString(string.Format("ID_scenes_lost_main_chapter_{0}_name", tpl.Id), obj.Name);//;
            tpl.LandId = obj.LandId;
            tpl.ChapterPos = new Vector2(float.Parse(obj.ChapterPos.Split(',')[0]), float.Parse(obj.ChapterPos.Split(',')[1]));
            tpl.ChapterBg = obj.ChapterBg;

            tpl.CampaignList = new List<string>();
            for (int i = 0; i < obj.CampaignListLength; i++)
            {
                tpl.CampaignList.Add(obj.GetCampaignList(i));
            }

            if (obj.BoxList != null)
            {
                tpl.RewardDataDic = new Dictionary<int, List<LTShowItemData>>();
                string[] starList = obj.BoxList.Split(',');
                if (starList.Length >= 1)
                {
                    tpl.RewardDataDic.Add(int.Parse(starList[0]), GetShowItemDataByRewardStr(obj.AwardIcon1));
                }

                if (starList.Length >= 2)
                {
                    tpl.RewardDataDic.Add(int.Parse(starList[1]), GetShowItemDataByRewardStr(obj.AwardIcon2));
                }

                if (starList.Length >= 3)
                {
                    tpl.RewardDataDic.Add(int.Parse(starList[2]), GetShowItemDataByRewardStr(obj.AwardIcon3));
                }
            }

            tpl.AwardIcon1 = obj.AwardIcon1;
            tpl.AwardIcon2 = obj.AwardIcon2;
            tpl.AwardIcon3 = obj.AwardIcon3;
            tpl.MaskBg = obj.MaskBg;
            tpl.Icon = obj.Icon;
            if (tpl.IsBoxRewardType())
            {
                tpl.RewardDataDic = new Dictionary<int, List<LTShowItemData>>();
                tpl.RewardDataDic.Add(0, GetShowItemDataByRewardStr(obj.AwardIcon1));
            }
            tpl.BeforeChapter = obj.BeforeChapter;
            tpl.AfterChapter = obj.AfterChapter;

            return tpl;
        }

        private List<LTShowItemData> GetShowItemDataByRewardStr(string rewardStr)
        {
            if (string.IsNullOrEmpty(rewardStr))
            {
                EB.Debug.LogError("LostMainChapterTemplate Init Error, ChapterReward is Null!");
                return null;
            }

            List<LTShowItemData> dataList = new List<LTShowItemData>();
            string[] tempRewardStrs = rewardStr.Split(';');
            for (int i = 0; i < tempRewardStrs.Length; i++)
            {
                string[] tempReStr = tempRewardStrs[i].Split(',');
                if (tempReStr.Length < 3)
                {
                    EB.Debug.LogError("LostMainChapterTemplate Init Error, ChapterReward is Error, ChpaterReward :{0}" , rewardStr);
                    return dataList;
                }

                LTShowItemData tempData = new LTShowItemData(tempReStr[0], int.Parse(tempReStr[1]), tempReStr[2], false);
                dataList.Add(tempData);
            }

            return dataList;
        }

        private bool InitLostMainCampaignsTpl(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitLostMainCampaignsTpl: tbl is null");
                return false;
            }

            mLostMainCampaignsTpl = new Dictionary<string, LostMainCampaignsTemplate>();
            string chapterid = string.Empty;
            int t_order = 0;
            int t_totalnum = 0;
            for (int i = 0; i < tbl.LostMainCampaignsLength; ++i)
            {
                var tpl = ParseLostMainCampaigns(tbl.GetLostMainCampaigns(i));
                if (string.IsNullOrEmpty(chapterid) || tpl.ChapterId.Equals(chapterid))
                {
                    t_order += 1;                 
                }
                else
                {
                    t_order = 1;
                }
                chapterid = tpl.ChapterId;
                t_totalnum += 1;
                tpl.orderid = t_order;
                tpl.totalcampaign = t_totalnum;
                string key = tpl.Id;
                if (mLostMainCampaignsTpl.ContainsKey(key))
                {
                    EB.Debug.LogError("InitLostMainCampaignsTpl: {0} exists", key);
                    mLostMainCampaignsTpl.Remove(key);
                }
                mLostMainCampaignsTpl.Add(key, tpl);
            }
            return true;
        }

        private LostMainCampaignsTemplate ParseLostMainCampaigns(GM.DataCache.LostMainCampaigns obj)
        {
            LostMainCampaignsTemplate tpl = new LostMainCampaignsTemplate();

            tpl.ChapterId = obj.Chapter;
            tpl.Id = obj.Id;
            tpl.Name = EB.Localizer.GetTableString(string.Format("ID_scenes_lost_main_campaigns_{0}_name", tpl.Id), obj.Name);
            tpl.RecommendLevel = obj.RecommendLevel;
            tpl.PartnerExp = obj.PartnerExp;
            tpl.BlitzPartnerExp = obj.BlitzPartnerExp;
            tpl.HeroExp = obj.HeroExp;
            tpl.CostVigor = obj.CostVigor;
            tpl.AwardGold = obj.AwardGold;
            tpl.MapName = obj.MapName;
            tpl.Desc = EB.Localizer.GetTableString(string.Format("ID_scenes_lost_main_campaigns_{0}_desc", tpl.Id), obj.Desc);
            tpl.Type = (MainLineNodeType)obj.Type;
            tpl.ModelName = obj.Name;

            tpl.PreCampaigns = new List<string>();
            for (int i = 0; i < obj.PreCampaignsLength; i++)
            {
                tpl.PreCampaigns.Add(obj.GetPreCampaigns(i));
            }

            tpl.NextCampaigns = new List<string>();
            for (int i = 0; i < obj.NextCampaignsLength; i++)
            {
                tpl.NextCampaigns.Add(obj.GetNextCampaigns(i));
            }

            tpl.EncounterGroupId = obj.EncounterGroupId;
            tpl.StoryId = obj.StoryId;
            tpl.RewardId = obj.RewardId;
            tpl.CampaignPos = new Vector2((float)obj.GetCampaignPos(0), (float)obj.GetCampaignPos(1));
            if (!string.IsNullOrEmpty(obj.AwardIcon))
                tpl.AwardIconList = new List<string>(obj.AwardIcon.Split(','));
            else
                tpl.AwardIconList = new List<string>();
            return tpl;
        }

        private bool InitLostChallemgeChapterTpl(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitLostChallemgeChapterTpl: tbl is null");
                return false;
            }

            mLostChallengeChapterTpl = new Dictionary<int, LostChallengeChapter>();
            int chapter = 0;
            int level = 0;
            for (int i = 0; i < tbl.LostChallengeChapterLength; ++i)
            {
                var tpl = ParseLostChallengeChapters(tbl.GetLostChallengeChapter(i));
                int key = tpl.Level;
                if (mLostChallengeChapterTpl.ContainsKey(key))
                {
                    EB.Debug.LogError("InitLostChallemgeChapterTpl: {0} exists", key);
                    mLostChallengeChapterTpl.Remove(key);
                }
                if (tpl.IsCheckPoint) { chapter++; level = 1; }
                else { level++; }
                tpl.CurChapter = chapter;
                tpl.CurLevel = level;
                mLostChallengeChapterTpl.Add(key, tpl);
            }
            return true;
        }

        private LostChallengeChapter ParseLostChallengeChapters(GM.DataCache.LostChallengeChapter obj)
        {
            LostChallengeChapter tpl = new LostChallengeChapter();

            tpl.Level = obj.Level;
            tpl.IsCheckPoint = obj.IsCheckPoint;
            tpl.RecommendLevel = obj.RecommendLevel;
            string FixMap = obj.FixMap;
            tpl.IsFixMap = !string.IsNullOrEmpty(FixMap);
            tpl.IsBoss = tpl.IsFixMap && FixMap.StartsWith("sceneBOSS"); 
            return tpl;
        }

        public bool InitLostChallengeWallsTpl(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitLostChallengeWallsTpl: tbl is null");
                return false;
            }

            mLostChallengeWallTpl = new Dictionary<string, List<LostChallengeWalls>>();
            for (int i = 0; i < tbl.LostChallengeWallsLength; ++i)
            {
                var tpl = ParseLostChallengeWalls(tbl.GetLostChallengeWalls(i));
                if (mLostChallengeWallTpl.ContainsKey(tpl.Type))
                {
                    mLostChallengeWallTpl[tpl.Type].Add(tpl);
                }
                else
                {
                    List<LostChallengeWalls> list = new List<LostChallengeWalls>();
                    mLostChallengeWallTpl[tpl.Type] = list;
                    mLostChallengeWallTpl[tpl.Type].Add(tpl);
                }
            }
            return true;
        }

        private LostChallengeWalls ParseLostChallengeWalls(GM.DataCache.LostChallengeWalls obj)
        {
            LostChallengeWalls tpl = new LostChallengeWalls();
            tpl.Id = obj.Id;
            tpl.Type = obj.Type;
            tpl.Pos = (LostChallengeWalls.WallPos)Enum.Parse(typeof(LostChallengeWalls.WallPos), obj.Pos);
            tpl.Img = obj.Img;
            string[] split = obj.Rotation.Split(',');
            tpl.Rotation = new Vector3(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
            return tpl;
        }

        private bool InitLostCombatMaps(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitLostCombatMps: tbl is null");
                return false;
            }

            mLostCombatMapTpl = new Dictionary<eBattleType, LostCombatMapTemplate>();
            for (int i = 0; i < tbl.LostCombatMapLength; i++)
            {
                var tpl = ParseLostCombatMap(tbl.GetLostCombatMap(i));
                if (mLostCombatMapTpl.ContainsKey(tpl.Type))
                {
                    EB.Debug.LogError("InitLostCombatMaps: {0} exists", tpl.Type);
                    mLostCombatMapTpl.Remove(tpl.Type);
                }
                mLostCombatMapTpl.Add(tpl.Type, tpl);
            }
            return true;
        }

        private LostCombatMapTemplate ParseLostCombatMap(GM.DataCache.LostCombatMap obj)
        {
            LostCombatMapTemplate tpl = new LostCombatMapTemplate();
            tpl.Type = (eBattleType)obj.BattleType;
            tpl.SceneList = obj.Scene.Split(',');
            return tpl;
        }

        private bool InitLostChallengeStyles(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitLostChallengeStyles: tbl is null");
                return false;
            }

            mLostChallengeStyleTpl = new Dictionary<int, LostChallengeStyleTemplate>();
            TopBlockTerraList = new List<int>();
            for (int i = 0; i < tbl.LostChallengeStyleLength; i++)
            {
                var tpl = ParseLostChallengeStyle(tbl.GetLostChallengeStyle(i));
                if (mLostChallengeStyleTpl.ContainsKey(tpl.Id))
                {
                    mLostChallengeStyleTpl.Remove(tpl.Id);
                    EB.Debug.LogError("InitLostChallengeStyles: {0} exists", tpl.Id);
                }
                mLostChallengeStyleTpl.Add(tpl.Id, tpl);
                if (tpl.TopWell > 0 && !TopBlockTerraList.Contains(tpl.TopWell))
                {
                    TopBlockTerraList.Add(tpl.TopWell);
                }
            }
            return true;
        }

        private LostChallengeStyleTemplate ParseLostChallengeStyle(GM.DataCache.LostChallengeStyle obj)
        {
            LostChallengeStyleTemplate tpl = new LostChallengeStyleTemplate();
            tpl.Id = obj.Id;
            tpl.TopWell = obj.TopWell;
            tpl.NormalTerra = obj.NormalTerra;
            tpl.MapBg = obj.MapBg;
            tpl.MapMask = obj.MapMask;
            tpl.CombatScene = obj.CombatScene;
            return tpl;
        }

        public bool InitLostChallengeRewards(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitLostChallengeRewards: tbl is null");
                return false;
            }

            mLostChallengeRewardTpl = new Dictionary<DayOfWeek, List<LostChallengeRewardTemplate>>();
            mLostChallengeRewardTpl.Add(DayOfWeek.Sunday, new List<LostChallengeRewardTemplate>());
            mLostChallengeRewardTpl.Add(DayOfWeek.Monday, new List<LostChallengeRewardTemplate>());
            mLostChallengeRewardTpl.Add(DayOfWeek.Tuesday, new List<LostChallengeRewardTemplate>());
            mLostChallengeRewardTpl.Add(DayOfWeek.Wednesday, new List<LostChallengeRewardTemplate>());
            mLostChallengeRewardTpl.Add(DayOfWeek.Thursday, new List<LostChallengeRewardTemplate>());
            mLostChallengeRewardTpl.Add(DayOfWeek.Friday, new List<LostChallengeRewardTemplate>());
            mLostChallengeRewardTpl.Add(DayOfWeek.Saturday, new List<LostChallengeRewardTemplate>());
            for (int i = 0; i < tbl.LostChallengeRewardLength; i++)
            {
                var tpl = ParseLostChallengeRward(tbl.GetLostChallengeReward(i));
                if (mLostChallengeRewardTpl.ContainsKey(tpl.Week))
                {
                    mLostChallengeRewardTpl[tpl.Week].Add(tpl);
                }
            }
            return true;
        }

        private LostChallengeRewardTemplate ParseLostChallengeRward(GM.DataCache.LostChallengeReward obj)
        {
            LostChallengeRewardTemplate tpl = new LostChallengeRewardTemplate();
            tpl.Week = (DayOfWeek)obj.Week;
            tpl.Floor = obj.Floor;
            if (LostChallengeRewardMaxFloor < tpl.Floor) LostChallengeRewardMaxFloor = tpl.Floor;
            tpl.DropList = new List<string>();
            if (obj.Drop1 != null)
            {
                tpl.DropList.Add(obj.Drop1);
            }

            if (obj.Drop2 != null)
            {
                tpl.DropList.Add(obj.Drop2);
            }

            if (obj.Drop3 != null)
            {
                tpl.DropList.Add(obj.Drop3);
            }

            if (obj.Drop4 != null)
            {
                tpl.DropList.Add(obj.Drop4);
            }

            tpl.DropRate = obj.DropRate;
            return tpl;
        }

        public LostChallengeRewardTemplate GetLostChallengeReward(DayOfWeek week, int floor)
        {
            LostChallengeRewardTemplate DropList = new LostChallengeRewardTemplate();
            if (mLostChallengeRewardTpl.ContainsKey(week))
            {
                List<LostChallengeRewardTemplate> tplList = mLostChallengeRewardTpl[week];
                for (int i = 0; i < tplList.Count; i++)
                {
                    if (tplList[i].Floor == floor)
                    {
                        DropList = tplList[i];
                    }
                }
            }
            return DropList;
        }

        /// <summary>
        /// 用于挑战副本奖励预览
        /// </summary>
        /// <param name="week"></param>
        /// <param name="floor"></param>
        /// <returns></returns>
        public LostChallengeRewardTemplate GetLostChallengeReward(int floor)
        {
            int i = 0;
            LostChallengeRewardTemplate DropList = new LostChallengeRewardTemplate();
            if (mLostChallengeRewardTpl.ContainsKey(System.DayOfWeek.Monday))
            {
                List<LostChallengeRewardTemplate> tplList = mLostChallengeRewardTpl[System.DayOfWeek.Monday];
                for (; i < tplList.Count; i++)
                {
                    if (tplList[i].Floor == floor)
                    {
                        DropList = tplList[i];
                        return DropList;
                    }
                }
                return tplList[i - 1];
            }
            return null;
        }

        #region 这个表加载要2秒
        private bool InitLostChallengeChapterRole(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitLostChallengeChapterRole: tbl is null");
                return false;
            }

            mLostChallengeChapterRoleTpl.Clear();
            int len = tbl.LostChallengeChapterRoleLength;
            for (int i = 0; i < len; i++)
            {
                var tpl = new LostChallengeChapterRole(tbl.GetLostChallengeChapterRole(i));
                mLostChallengeChapterRoleTpl[tpl.Id] = tpl;
            }
            return true;
        }
        #endregion

        private bool InitLostChallengeChapterElement(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitLostChallengeChapterElement: tbl is null");
                return false;
            }

            mLostChallengeChapterElementTpl = new Dictionary<int, LostChallengeChapterElement>();
            mRandomGroupElementTpl = new Dictionary<int, List<LostChallengeChapterElement>>();
            for (int i = 0; i < tbl.LostChallengeChapterElementLength; ++i)
            {
                var tpl = ParseLostChallengeChapterElement(tbl.GetLostChallengeChapterElement(i));
                if (mLostChallengeChapterElementTpl.ContainsKey(tpl.Id))
                {
                    mLostChallengeChapterElementTpl.Remove(tpl.Id);
                    EB.Debug.LogError("InitLostChallengeChapterElement: {0} exists", tpl.Id);
                }
                mLostChallengeChapterElementTpl.Add(tpl.Id, tpl);
                if (TopBlockTerraList.Contains(tpl.RandomGroup))
                {
                    if (!mRandomGroupElementTpl.ContainsKey(tpl.RandomGroup))
                    {
                        mRandomGroupElementTpl.Add(tpl.RandomGroup,new List<LostChallengeChapterElement>());
                    }
                    mRandomGroupElementTpl[tpl.RandomGroup].Add(tpl);
                }
            }
            TopBlockTerraList.Clear();
            return true;
        }

        private LostChallengeChapterElement ParseLostChallengeChapterElement(GM.DataCache.LostChallengeChapterElement obj)
        {
            LostChallengeChapterElement tpl = new LostChallengeChapterElement();
            tpl.Id = obj.Id;
            tpl.Desc = obj.Desc;
            tpl.Type = obj.Type;
            tpl.Color = obj.Color;
            tpl.Img = obj.Img;
            if (tpl.Img == null) tpl.Img = "";
            tpl.CanPass = (obj.CanPass != 0);
            tpl.Group = obj.Group;
            tpl.RandomGroup = obj.RandomGroup;
            tpl.RP = obj.RP;
            if (!string.IsNullOrEmpty(obj.Rotation))
            {
                string[] rotationSplit = obj.Rotation.Split(',');
                if (rotationSplit.Length >= 3)
                {
                    tpl.Rotation = new Vector3(int.Parse(rotationSplit[0]), int.Parse(rotationSplit[1]), int.Parse(rotationSplit[2]));
                }
                else
                {
                    EB.Debug.LogError("Error Terra Config Rotation need Vector3, terra id = {0}", tpl.Id);
                }
            }
            return tpl;
        }

        public LostChallengeChapterRole GetChallengeChapterRole(int id)
        {
            mLostChallengeChapterRoleTpl.TryGetValue(id, out LostChallengeChapterRole info);
            return info;
        }

        private bool InitLostChallengeEnv(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitLostChallengeEnv: tbl is null");
                return false;
            }

            mLostChallengeEnvTpl.Clear();
            for (int i = 0; i < tbl.LostChallengeEnvLength; i++)
            {
                var tpl = ParseLostChallengeEnv(tbl.GetLostChallengeEnv(i));
                mLostChallengeEnvTpl[tpl.Id] = tpl;
            }
            return true;
        }

        private LostChallengeEnv ParseLostChallengeEnv(GM.DataCache.LostChallengeEnv obj)
        {
            LostChallengeEnv tpl = new LostChallengeEnv();

            tpl.Id = obj.Id;
            tpl.Name = EB.Localizer.GetTableString(string.Format("ID_scenes_lost_challenge_env_{0}_name", tpl.Id), obj.Name); //obj.Name;

            tpl.EnvLogic = obj.EnvLogic;

            tpl.Desc = EB.Localizer.GetTableString(string.Format("ID_scenes_lost_challenge_env_{0}_desc", tpl.Id), obj.Desc); //obj.Desc;
            tpl.Pic = obj.Pic;
            tpl.Icon = obj.Icon;
            return tpl;
        }

        private bool InitAlienMaze(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitAlienMaze: tbl is null");
                return false;
            }

            mAlienMazeTpl = new List<AlienMazeTemplate>();
            for (int i = 0; i < tbl.AlienMazeLength; ++i)
            {
                var tpl = ParseAlienMaze(tbl.GetAlienMaze(i));
                if (tpl.Id != 0)
                {
                    mAlienMazeTpl.Add(tpl);
                }
            }
            return true;
        }

        private AlienMazeTemplate ParseAlienMaze(GM.DataCache.AlienMaze obj)
        {
            AlienMazeTemplate tpl = new AlienMazeTemplate();
            tpl.Id = obj.Id;
            tpl.Name = EB.Localizer.GetTableString(string.Format("ID_scenes_alien_maze_{0}_name", tpl.Id), obj.Name);
            tpl.Icon = obj.Icon;
            tpl.Limit = obj.Limit;
            tpl.Reward = ParseShowItem(obj.Reward);
            tpl.MapStyle = obj.MapStyle;
            return tpl;
        }

        private bool InitMonopoly(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitMonopoly: tbl is null");
                return false;
            }

            mMonopolyTpl = new List<MonopolyTemplate>();
            for (int i = 0; i < tbl.MonopolyLength; ++i)
            {
                var tpl = ParseMonopoly(tbl.GetMonopoly(i));
                if (tpl.id != 0)
                {
                    mMonopolyTpl.Add(tpl);
                }
            }
            return true;
        }

        private MonopolyTemplate ParseMonopoly(GM.DataCache.Monopoly obj)
        {
            MonopolyTemplate tpl = new MonopolyTemplate();
            tpl.id = obj.Id;
            tpl.Reward = ParseShowItem(obj.Reward);
            tpl.MapStyle = obj.MapStyle;
            return tpl;
        }

        public MonopolyTemplate GetMonopolyById(int id)
        {
            for (int i = 0; i < mMonopolyTpl.Count; ++i)
            {
                if (mMonopolyTpl[i].id == id || i == mMonopolyTpl.Count - 1)
                {
                    return mMonopolyTpl[i];
                }
            }
            return null;
        }

        public List<LostChallengeChapterElement> GetRandomGroupElementList(int tRandomGroup)
        {
            List<LostChallengeChapterElement> temp = null;
            if(!mRandomGroupElementTpl.TryGetValue(tRandomGroup,out temp))
            {
                EB.Debug.LogError("GetRandomGroupElementList Can't find tRandomGroup:{0}", tRandomGroup);
            }
            return temp;
        }

        public List<AlienMazeTemplate> GetAllAlienMazeList()
        {
            return mAlienMazeTpl;
        }

        public AlienMazeTemplate GetAlienMazeById(int id)
        {
            for (int i = 0; i < mAlienMazeTpl.Count; ++i)
            {
                if (mAlienMazeTpl[i].Id == id)
                {
                    return mAlienMazeTpl[i];
                }
            }
            return null;
        }

        public Dictionary<int, LostChallengeEnv> GetAllLostChallengeEnv()
        {
            return mLostChallengeEnvTpl;
        }

        public LostChallengeEnv GetLostChallengeEnvById(int id)
        {
            if (mLostChallengeEnvTpl.ContainsKey(id))
            {
                return mLostChallengeEnvTpl[id];
            }
            return null;
        }

        private bool InitLostInstanceMessage(GM.DataCache.ConditionScene tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitLostInstanceMessage: tbl is null");
                return false;
            }

            mLostInstanceMessageTpl = new Dictionary<int, List<LostInstanceMessage>>();

            for (int i = 0; i < tbl.LostInstanceMessageLength; i++)
            {
                var tpl = ParseLostInstanceMsg(tbl.GetLostInstanceMessage(i));
                if (mLostInstanceMessageTpl.ContainsKey(tpl.MsgId))
                {
                    mLostInstanceMessageTpl[tpl.MsgId].Add(tpl);
                }
                else
                {
                    mLostInstanceMessageTpl[tpl.MsgId] = new List<LostInstanceMessage>();
                    mLostInstanceMessageTpl[tpl.MsgId].Add(tpl);
                }
            }
            return true;
        }

        private LostInstanceMessage ParseLostInstanceMsg(GM.DataCache.LostInstanceMessage obj)
        {
            LostInstanceMessage tpl = new LostInstanceMessage();
            tpl.Id = obj.Id;
            tpl.MsgId = obj.MessageId;
            tpl.Name = EB.Localizer.GetTableString(string.Format("ID_scenes_lost_instance_message_{0}_name", tpl.Id), obj.Name); //obj.Name;
            tpl.Icon = obj.Icon;
            tpl.Desc = EB.Localizer.GetTableString(string.Format("ID_scenes_lost_instance_message_{0}_desc", tpl.Id), obj.Desc); //obj.Desc;
            return tpl;
        }

        public List<LostInstanceMessage> GetLostInstanceMsgList(int id)
        {
            List<LostInstanceMessage> list = new List<LostInstanceMessage>();
            mLostInstanceMessageTpl.TryGetValue(id, out list);
            return list;
        }

        public LostChallengeChapterElement GetLostChallengeChapterElement(int id)
        {
            LostChallengeChapterElement ret;
            mLostChallengeChapterElementTpl.TryGetValue(id, out ret);
            return ret;
        }

        public CampaignTemplate GetCampaign(string name)
        {
            CampaignTemplate result = null;
            if (!mCampaignTbl.TryGetValue(name, out result))
            {
                EB.Debug.LogWarning("GetCampaign: campaign not found, name  = {0}", name);
            }
            return result;
        }

        public EncounterTemplate GetEncounter(string campaignName, string locator)
        {
            EncounterTemplate result = null;
            string key = string.Format("{0}|{1}", campaignName, locator);
            if (!mEncounterTbl.TryGetValue(key, out result))
            {
                EB.Debug.LogWarning("GetEncounter: encounter not found, key = {0}", key);
            }

            return result;
        }

        public Dictionary<string, AllianceCampaignTemplate> GetAllianceCampaigns()
        {
            return mAllianceCampaignTbl;
        }

        public AllianceCampaignTemplate GetAllianceCampaign(string name)
        {
            AllianceCampaignTemplate result = null;
            if (!mAllianceCampaignTbl.TryGetValue(name, out result))
            {
                EB.Debug.LogWarning("GetAllianceCampaign: campaign not found, id  = {0}", name);
            }
            return result;
        }

        public AllianceEncounterTemplate GetAllianceEncounter(string campaignName, string locator)
        {
            AllianceEncounterTemplate result = null;
            string key = string.Format("{0}|{1}", campaignName, locator);
            if (!mAllianceEncounterTbl.TryGetValue(key, out result))
            {
                EB.Debug.LogWarning("GetAllianceEncounter: encounter not found, key = {0}", key);
            }

            return result;
        }

        public LayoutTemplate GetLayout(string layoutName, int wave)
        {
            var key = $"{layoutName}|{wave}";
            mLayoutTbl.TryGetValue(key, out LayoutTemplate result);
            return result;
        }

        public LayoutTemplate[] GetLayouts()
        {
            if (mLayoutTbl.Count < 1)
            {
                InitLayoutTbl(conditionSet);
            }
            return new List<LayoutTemplate>(mLayoutTbl.Values).ToArray();
        }

        public List<int> GetLayoutsByNameEx(string layoutName)
        {
            List<LayoutTemplate> results = new List<LayoutTemplate>();
            var enumerator = mLayoutTbl.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var v = enumerator.Current;
                string str = v.Key.Substring(0, v.Key.Length - (v.Value.wave.ToString().Length + 1));
                if (str.Equals(layoutName))
                {
                    results.Add(v.Value);
                }
            }

            results.Sort((x, y) => x.wave.CompareTo(y.wave));

            int len = results.Count;
            if (len < 1)
            {
                return null;
            }

            List<int> realResult = new List<int>(len);
            for (int i = 0; i < len; i++)
            {
                realResult.Add(results[i].wave);
            }
            return realResult;
        }

        public List<string> GetLayoutEx(string layoutName, int wave)
        {
            LayoutTemplate result = null;
            string key = string.Format("{0}|{1}", layoutName, wave);
            if (!mLayoutTbl.TryGetValue(key, out result))
            {
                EB.Debug.Log("GetLayout: layout not found, key = {0}", key);
                return null;
            }
            List<string> realResult = new List<string>();
            realResult.Add(result.f1);
            realResult.Add(result.f2);
            realResult.Add(result.f3);
            realResult.Add(result.m1);
            realResult.Add(result.m2);
            realResult.Add(result.m3);
            realResult.Add(result.b1);
            realResult.Add(result.b2);
            realResult.Add(result.b3);

            return realResult;
        }

        //combateditor 用
        public string[] GetLayoutsEx1()
        {
            #region TODO: _HotfixScripts下不能用Linq
            // return GetLayouts().OrderBy(l => l.combat_layout_name.Length).ThenBy(l => l.combat_layout_name).Distinct().Select(l => l.combat_layout_name).ToArray();

            LayoutTemplate[] lt = GetLayouts();
            List<string> ll = new List<string>();
            for (int i = 0; i < lt.Length; i++)
            {
                if (!ll.Contains(lt[i].combat_layout_name))
                {
                    ll.Add(lt[i].combat_layout_name);
                }
            }
            ll.Sort((l, r) => {
                return l.Length.CompareTo(r.Length);
            });
            ll.Sort((l, r) => {
                return l.CompareTo(r);
            });

            return ll.ToArray();
            #endregion
        }

        public string[] GetLayoutsEx2()
        {
            #region TODO: _HotfixScripts下不能用Linq
            // return GetLayouts().OrderBy(l => l.combat_layout_name.Length).ThenBy(l => l.combat_layout_name).Distinct().Select(l =>
            // {
            //     if (l.combat_layout_name.Length > 2)
            //     {
            //         return l.combat_layout_name.Insert(l.combat_layout_name.Length - 2, "/");
            //     }
            //     else
            //     {
            //         return l.combat_layout_name;
            //     }
            // }).ToArray();

            LayoutTemplate[] lt = GetLayouts();
            List<string> ll = new List<string>();
            for (int i = 0; i < lt.Length; i++)
            {
                string name = lt[i].combat_layout_name;
                if (!ll.Contains(name))
                {
                    if (name.Length > 2)
                    {
                        name.Insert(name.Length - 2, "/");
                    }
                    ll.Add(name);
                }
            }
            ll.Sort((l, r) => {
                return l.Length.CompareTo(r.Length);
            });
            ll.Sort((l, r) => {
                return l.CompareTo(r);
            });

            return ll.ToArray();
            #endregion
        }

        public LayoutTemplate GetMaxWaveLayout(string layoutName)
        {
            LayoutTemplate temp = null;
            for (int i = 1; i <= 3; i++)
            {
                LayoutTemplate target = GetLayout(layoutName, i);
                temp = target != null ? target : temp;
            }
            return temp;
        }

        /// <summary>
        /// 使用layoutName得到 仅在战斗编辑器使用
        /// </summary>
        /// <param name="layoutName"></param>
        /// <returns></returns>
        public List<LayoutTemplate> GetLayoutsByName(string layoutName)
        {
            int len = conditionSet.CombatLayoutLength;
            if (mLayoutTbl.Count != len)
            {
                for (int i = 0; i < len; i++)
                {
                    LayoutInfo obj = conditionSet.GetCombatLayout(i);
                    string key = string.Format($"{obj.CombatLayoutName}|{obj.Wave}");

                    if (mLayoutTbl.ContainsKey(key) == false)
                    {
                        LayoutTemplate tpl = new LayoutTemplate()
                        {
                            obj = obj
                        };
                        mLayoutTbl.Add(key, tpl);
                    }
                }
            }

            List<LayoutTemplate> results = new List<LayoutTemplate>();

            var enumerator = mLayoutTbl.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var v = enumerator.Current;
                string str = v.Key.Substring(0, v.Key.Length - (v.Value.wave.ToString().Length + 1));
                if (str.Equals(layoutName))
                {
                    results.Add(v.Value);
                }
            }

            return results;
        }

        public MainLandTemplate GetMainLand(string name)
        {
            MainLandTemplate result = null;
            if (!mMainLandTbl.TryGetValue(name, out result))
            {
                EB.Debug.LogWarning("GetMainLand: mainland not found, name = {0}", name);
            }
            return result;
        }

        public MainLandEncounterTemplate GetMainLandEncounter(string mainlandName, string locator)
        {
            MainLandEncounterTemplate result = null;
            string key = string.Format("{0}|{1}", mainlandName, locator);
            if (!mMainLandEncounterTbl.TryGetValue(key, out result))
            {
                //EB.Debug.LogWarning("GetMainLandEncounter: mainland encounter not found, key = {0}", key);
            }
            return result;
        }

        public MainLandEncounterTemplate GetMainLandEncounter(int func_id)
        {
            var enumerator = mMainLandEncounterTbl.Values.GetEnumerator();
            while(enumerator.MoveNext())
            {
                MainLandEncounterTemplate tpl = enumerator.Current;
                if (tpl.func_id_1 == func_id || tpl.func_id_2 == func_id || tpl.func_id_3 == func_id)
                    return tpl;

            }
            EB.Debug.LogError("GetMainLandEncounter: mainland encounter not found,func_id={0}" , func_id);
            return null;
        }

        public MainlandsGhostTemplate GetMainLandsGhost(string mainlandName, string locator)
        {
            MainlandsGhostTemplate result = null;
            string key = string.Format("{0}|{1}", mainlandName, locator);
            if (!mMainlandsGhostTbl.TryGetValue(key, out result))
            {
                EB.Debug.LogWarning("GetMainLandsGhost: mainland ghost not found, key = {0}", key);
            }
            return result;
        }

        public Dictionary<string, MainlandsGhostRewardTemplate> GetAllMainlandsGhostReward()
        {
            return mMainlandsGhostRewardTbl;
        }

        public LostMainChapterTemplate GetLostMainChatpterTplById(string id)
        {
            LostMainChapterTemplate result = null;
            if (!mLostMainChapterTpl.TryGetValue(id, out result))
            {
                EB.Debug.LogWarning("GetLostMainChatpterTplById: not found, key = {0}", id);
            }
            return result;
        }

        /// <summary>
        /// 获取当前主线章节最大星星数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetLostMainChapterMaxStarNumById(string id)
        {
            int num = 0;
            LostMainChapterTemplate result = null;
            if (!mLostMainChapterTpl.TryGetValue(id, out result))
            {
                EB.Debug.LogWarning("GetLostMainChapterMaxStarNumById: not found, key = {0}", id);
                return num;
            }
            num = result.CampaignList.Count * 3;
            return num;
        }

        public List<LostMainChapterTemplate> GetLostMainChapterBoxRewards()
        {
            List<LostMainChapterTemplate> result = new List<LostMainChapterTemplate>();
            var enumerator = mLostMainChapterTpl.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                if (pair.Value.IsBoxRewardType())
                {
                    result.Add(pair.Value);
                }
            }
            return result;
        }

        /// <summary>
        /// 根据章节ID和星级获得对应奖励
        /// </summary>
        /// <param name="chapterId"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public List<LTShowItemData> GetLostMainChapterStarReward(string chapterId, int num)
        {
            LostMainChapterTemplate tpl = GetLostMainChatpterTplById(chapterId);
            if (tpl != null)
            {
                var enumerator = tpl.RewardDataDic.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    var pair = enumerator.Current;
                    if (pair.Key == num)
                    {
                        return pair.Value;
                    }
                }
            }
            return null;
        }

        public LostMainCampaignsTemplate GetLostMainCampaignTplById(string id)
        {
            LostMainCampaignsTemplate result = null;
            if (!mLostMainCampaignsTpl.TryGetValue(id, out result))
            {
                EB.Debug.LogWarning("GetLostMainCampaignTplById: not found, key = {0}", id);
            }
            return result;
        }

        public bool IsCampaignBoss(string id)
        {
            var tpl = GetLostMainCampaignTplById(id);
            if (tpl != null)
            {
                var bossTpl = GetBossLostMainCampaignTplByChapterId(tpl.ChapterId);
                return bossTpl.Id == id;
            }
            return false;
        }

        public LostMainCampaignsTemplate GetBossLostMainCampaignTplByChapterId(string chapterId)
        {
            LostMainCampaignsTemplate result = null;
            int campaignId = 0;

            var enumerator = mLostMainCampaignsTpl.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                if (pair.Value.ChapterId == chapterId)
                {
                    int curCampaignId = 0;
                    if (int.TryParse(pair.Value.Id, out curCampaignId))
                    {
                        if (curCampaignId > campaignId)
                        {
                            result = pair.Value;
                        }
                    }
                }
            }
            return result;
        }

        public LostMainChapterTemplate GetNextChapter(string chapterId)
        {
            var enumerator = mLostMainChapterTpl.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                if (pair.Value.ForwardChapterId == chapterId && !string.IsNullOrEmpty(pair.Value.ChapterBg))
                {
                    return pair.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取队列中最大章节数
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string GetMaxChapter(List<string> list)
        {
            int index = -1;
            int sortIndex = -1;
            List<string> sortList = new List<string>(mLostMainChapterTpl.Keys);
            for (int i = 0; i < list.Count; i++)
            {
                if (mLostMainChapterTpl.ContainsKey(list[i]) && mLostMainChapterTpl[list[i]].IsBoxRewardType())
                {
                    continue;
                }
                int curSortIndex = sortList.IndexOf(list[i]);
                if (curSortIndex > sortIndex)
                {
                    sortIndex = curSortIndex;
                    index = i;
                }
            }
            return list[index];
        }

        public List<LostChallengeChapter> GetLostChallengeCheckPointChapterList()
        {
            List<LostChallengeChapter> list = new List<LostChallengeChapter>();

            var enumerator = mLostChallengeChapterTpl.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var data = enumerator.Current;
                if (data.Value.IsCheckPoint)
                {
                    list.Add(data.Value);
                }
            }
            return list;
        }

        public LostChallengeChapter GetLostChallengeChapterById(int level)
        {
            if (mLostChallengeChapterTpl.ContainsKey(level))
            {
                return mLostChallengeChapterTpl[level];
            }
            return null;
        }

        public int GetMaxLostChallengeChapterId()
        {
            int maxLevel = 0;
            var enumerator = mLostChallengeChapterTpl.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var data = enumerator.Current;
                if (data.Value.CurChapter > maxLevel)
                {
                    maxLevel = data.Value.CurChapter;
                }
            }
            return maxLevel;
        }

        public LostChallengeChapter GetCheckPointChapter(int level)
        {
            List<LostChallengeChapter> list = GetLostChallengeCheckPointChapterList();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Level > level)
                {
                    return list[i - 1];
                }
            }
            if (level > mLostChallengeChapterTpl.Count)
            {
                return null;
            }
            else
            {
                return list[list.Count - 1];
            }
        }

        public LostChallengeChapter GetCheckPointChapterByChapter(int chapter)
        {
            List<LostChallengeChapter> list = GetLostChallengeCheckPointChapterList();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].CurChapter == chapter)
                {
                    return list[i];
                }
            }
            return null;
        }

        public bool IsLostChallengeWallTypeVaild(string type)
        {
            return mLostChallengeWallTpl.ContainsKey(type);
        }

        public LostChallengeWalls GetLostChallengeWallByPosAndType(string type, LostChallengeWalls.WallPos pos)
        {
            if (mLostChallengeWallTpl.ContainsKey(type))
            {
                for (int i = 0; i < mLostChallengeWallTpl[type].Count; i++)
                {
                    if (mLostChallengeWallTpl[type][i].Pos == pos)
                    {
                        return mLostChallengeWallTpl[type][i];
                    }
                }
            }
            return null;
        }

        public string GetRandomLostCombatMapByType(eBattleType type)
        {
            if (mLostCombatMapTpl.ContainsKey(type))
            {
                string[] list = mLostCombatMapTpl[type].SceneList;
                int index = UnityEngine.Random.Range(0, list.Length);
                return list[index];
            }
            return null;
        }

        public LostChallengeStyleTemplate GetChallengeStyleById(int id)
        {
            if (mLostChallengeStyleTpl.ContainsKey(id))
            {
                return mLostChallengeStyleTpl[id];
            }
            return null;
        }

        public static MainLandEncounterTemplate GetMainLandsNPCData(string sceneid, string npcid)
        {
            return SceneTemplateManager.Instance.GetMainLandEncounter(sceneid, npcid);
        }

        public static string GetNPCModel(string sceneid, string role, string npcid, string param = null)//此处为了世界BOSS的随机性加入param
        {
            string model = "";
            if (role == UI.NPC_ROLE.CAMPAIGN_ENEMY || role == UI.NPC_ROLE.CAMPAIGN_BOX)
            {
                var tpl = SceneTemplateManager.Instance.GetEncounter(sceneid, npcid);
                if (tpl != null) model = tpl.encounter_prefab;
            }
            else if (role == UI.NPC_ROLE.GHOST)
            {
                string tmp = npcid.Remove(npcid.LastIndexOf("_") + 1);
            var tpl = SceneTemplateManager.Instance.GetMainLandsGhost("s001a"/*先直接获取EB.Sparx.SceneManager.MainLandName*/, tmp);
                if (tpl != null) model = tpl.encounter_prefab;
            }
            else if (role == UI.NPC_ROLE.WORLD_BOSS)
            {
                var tpl = SceneTemplateManager.Instance.GetMainLandEncounter(sceneid, npcid);
                if (tpl != null)
                {
                    string[] split = tpl.encounter_prefab.Split(',');
                    int bossIndex = EventTemplateManager.Instance.GetWorldBossIndex(param);
                    model = bossIndex < 0 ? string.Empty : split[bossIndex];
                }
            }
            else
            {
                var tpl = SceneTemplateManager.Instance.GetMainLandEncounter(sceneid, npcid);
                if (tpl != null) model = tpl.encounter_prefab;
            }
            return model;
        }

        public static string GetNPCName(string sceneid, string role, string npcid)
        {
            string display_name = "";
            if (role == UI.NPC_ROLE.CAMPAIGN_ENEMY || role == UI.NPC_ROLE.CAMPAIGN_BOX)
            {
                var tpl = SceneTemplateManager.Instance.GetEncounter(sceneid, npcid);
                if (tpl != null) display_name = tpl.display_name;
            }
            else if (role == UI.NPC_ROLE.GHOST)
            {
                string tmp = npcid.Remove(npcid.LastIndexOf("_") + 1);
                var tpl = SceneTemplateManager.Instance.GetMainLandsGhost(sceneid, tmp);
                if (tpl != null) display_name = tpl.display_name;
            }
            else if (role == UI.NPC_ROLE.ALLIANCE_CAMPAIGN_BOSS || role == UI.NPC_ROLE.ALLIANCE_CAMPAIGN_ENEMY)
            {
                var tpl = SceneTemplateManager.Instance.GetAllianceEncounter(sceneid, npcid);
                if (tpl != null) display_name = tpl.display_name;
            }
            else if (role == UI.NPC_ROLE.ARENA_MODLE)
            {
                ArenaModelData AData = ArenaManager.Instance.GetArenaModelData();
                if (AData.Uid != 0)
                {
                    string AName = (AData.AName != null && AData.AName != "") ? string.Format("[{0}]", AData.AName) : "";
                    display_name = string.Format("[ff6600]{0}[{1}][-]", AName, AData.UName);
                }
                else
                {
                    var tpl = SceneTemplateManager.Instance.GetMainLandEncounter(sceneid, npcid);
                    if (tpl != null) display_name = tpl.display_name;
                }
            }
            else
            {
                var tpl = SceneTemplateManager.Instance.GetMainLandEncounter(sceneid, npcid);
                if (tpl != null) display_name = tpl.display_name;
            }
            return display_name;
        }

        public static string GetSceneTriggerDialgue(string name, string type)
        {
            if (type.Equals("mainlands"))
            {
                return SceneTemplateManager.Instance.GetMainLand(name).trigger_dialogue;
            }
            else if (type.Equals("campaigns"))
            {
                return SceneTemplateManager.Instance.GetAllianceCampaign(name).trigger_dialogue;
                //return SceneTemplateManager.Instance.GetCampaign(name).trigger_dialogue;
            }
            else
            {
                return null;
            }
        }

        public static List<TriggerEntry> GetSceneTransferPoints(string name, string type)
        {
            List<TriggerEntry> transferpots = new List<TriggerEntry>();
            if (type.Equals("mainlands"))
            {
                string trigger_text = SceneTemplateManager.Instance.GetMainLand(name).transfer_points;
                if (string.IsNullOrEmpty(trigger_text)) return null;
                string[] triggers = trigger_text.Split(';');
                if (triggers != null)
                {
                    for (int i = 0; i < triggers.Length; i++)
                    {
                        string[] splits = triggers[i].Split(':');
                        if (splits != null && splits.Length >= 3)
                        {
                            Vector3 pos = GM.LitJsonExtension.ImportVector3(splits[2]);
                            transferpots.Add(new TransferTriggerEntry(splits[0], splits[1], pos));

                        }
                        else EB.Debug.LogError("Transfer Trigger Format Error for {0}" , name);
                    }
                }
            }
            else if (type.Equals("campaigns"))
            {
            }
            else
            {
            }
            return transferpots;
        }
        private string LayoutID = string.Empty;
        private List<int> NewNewLayoutTemp;
        public List<int> GetNewNewLayoutList(string id)//查看敌方信息时使用
        {
            if (id.Equals(LayoutID))
            {
                return NewNewLayoutTemp;
            }
            LayoutID = id;
            if (NewNewLayoutTemp == null)
                NewNewLayoutTemp = new List<int>();
            else
                NewNewLayoutTemp.Clear();

            List<LayoutTemplate> temps = GetLayoutsByName(id);
            for (int i = 0; i < temps.Count; i++)
            {
                int showCount = 6;//目前查看敌方阵容6个，所以设为6
                int curCount = 0;
                if (curCount < showCount && !string.IsNullOrEmpty(temps[i].f1))
                { NewNewLayoutTemp.Add(int.Parse(temps[i].f1)); curCount++; }
                if (curCount < showCount && !string.IsNullOrEmpty(temps[i].f2))
                { NewNewLayoutTemp.Add(int.Parse(temps[i].f2)); curCount++; }
                if (curCount < showCount && !string.IsNullOrEmpty(temps[i].f3))
                { NewNewLayoutTemp.Add(int.Parse(temps[i].f3)); curCount++; }
                if (curCount < showCount && !string.IsNullOrEmpty(temps[i].m1))
                { NewNewLayoutTemp.Add(int.Parse(temps[i].m1)); curCount++; }
                if (curCount < showCount && !string.IsNullOrEmpty(temps[i].m2))
                { NewNewLayoutTemp.Add(int.Parse(temps[i].m2)); curCount++; }
                if (curCount < showCount && !string.IsNullOrEmpty(temps[i].m3))
                { NewNewLayoutTemp.Add(int.Parse(temps[i].m3)); curCount++; }
                for (; curCount < showCount; curCount++)
                { NewNewLayoutTemp.Add(0); }
            }
            return NewNewLayoutTemp;
        }
    }
}