using UnityEngine;
using System.Collections;
using EB;

namespace Hotfix_LT.UI
{
	public class PlayerPlacement
	{
		private Vector3 location;
		public Vector3 Location
		{
			get
			{
				return location;
			}
		}

		private float dir;
		public float Dir
		{
			get
			{
				return dir;
			}
		}

		private string prefab_name;
		public string ModelName
		{

			get
			{
				return prefab_name;
			}
		}

		private long userid;
		public long UserId
		{
			get
			{
				return userid;
			}
		}

		public PlayerPlacement()
		{
			location = new Vector3();
			dir = 0;
			prefab_name = string.Empty;
			userid = 0;
		}

		public PlayerPlacement(long uid, Hashtable data)
		{
			if (data.ContainsKey("dest"))
			{
				location = GM.LitJsonExtension.ImportVector3(data["dest"].ToString());
			}
			else
			{
				string pos_str = EB.Dot.String("pos", data, "");
				if (pos_str.Equals(""))
				{
					EB.Debug.LogWarning("Pos Struct is Error ");
					location = GM.LitJsonExtension.ImportVector3("(0.0,0.0,0.0)");
				}
				else location = GM.LitJsonExtension.ImportVector3(data["pos"].ToString());

			}
			dir = (float)EB.Dot.Double("dir", data, 0);
			prefab_name = EB.Dot.String("mode_name", data, string.Empty);
			userid = uid;
		}
	}

	public class DungeonPlacement
	{
		private string locator;
		public string Locator
		{
			get
			{
				return locator;
			}
		}

		private int isappearing;
		public int IsAppearing
		{
			get
			{
				return isappearing;
			}
		}

		private int appearingway;
		public int AppearingWay
		{
			get
			{
				return appearingway;
			}
		}

		private string role;
		public string Role
		{
			get
			{
				return role;
			}
		}

		private string encounter;
		public string Encounter
		{
			get
			{
				return encounter;
			}
			set
			{
				encounter = value;
			}
		}

		private string script;
		public string Script
		{
			get
			{
				return script;
			}
		}

		private string layout;
		public string Layout
		{
			get
			{
				return layout;
			}
		}

		private string pos;
		public string Pos
		{
			get
			{
				return pos;
			}
		}

		private bool is_fighting;
		public bool IsFighting
		{
			get
			{
				return is_fighting;
			}
		}

		public string attr;
		public string Attr
		{
			get
			{ return attr; }
		}

		public DungeonPlacement()
		{
			isappearing = 0;
			appearingway = 0;
			locator = string.Empty;
			role = string.Empty;
			encounter = string.Empty;
			script = string.Empty;
			layout = string.Empty;
			pos = string.Empty;
			is_fighting = false;
			attr = string.Empty;
		}

		public DungeonPlacement(string scene, string locator, Hashtable data)
		{
			this.locator = locator;
			role = EB.Dot.String("role", data, string.Empty);
			pos = EB.Dot.String("pos", data, string.Empty);
			attr = EB.Dot.String("tag_attribute", data, string.Empty);
			is_fighting = EB.Dot.Bool("busy", data, false);
            if (role == NPC_ROLE.CAMPAIGN_ENEMY || role == NPC_ROLE.CAMPAIGN_BOX) 
            {
                var tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetEncounter(scene, locator);
                isappearing = tpl.is_appearing;
                appearingway = tpl.encounter_appearing_way;
                encounter = tpl.encounter_prefab;
                script = tpl.script;
                layout = tpl.combat_layout_name;
            }
            else if (role == NPC_ROLE.GHOST)
            {
                string tmp = locator.Remove(locator.LastIndexOf("_") + 1);
                var tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetMainLandsGhost(scene, tmp);
                isappearing = 1;
                appearingway = 1;
                encounter = tpl.encounter_prefab;
                script = tpl.script;
                layout = tpl.combat_layout_name;
            }
            else if (role == NPC_ROLE.ALLIANCE_CAMPAIGN_ENEMY || role == NPC_ROLE.ALLIANCE_CAMPAIGN_BOSS)
            {
                var tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetAllianceEncounter(scene, locator);
                isappearing = tpl.is_appearing;
                appearingway = tpl.encounter_appearing_way;
                encounter = tpl.encounter_prefab;
                script = tpl.script;
                layout = tpl.combat_layout_name;
            }
            else if (role == NPC_ROLE.WORLD_BOSS)
            {
                var tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetMainLandEncounter(scene, locator);
                isappearing = tpl.is_appearing;
                appearingway = tpl.encounter_appearing_way;

                //encounter = tpl.encounter_prefab;
                string[] split = tpl.encounter_prefab.Split(',');
                string curLayout = EB.Dot.String("bossLayoutId", data, string.Empty);
                if (string.IsNullOrEmpty(curLayout))
                {
                    if (!DataLookupsCache.Instance.SearchDataByID("mainlands.lastWeekBossLayoutId", out curLayout))
                    {
                        curLayout = "Layout50103";
                    }
                }
                int bossIndex = Hotfix_LT.Data.EventTemplateManager.Instance.GetWorldBossIndex(curLayout);
                encounter = bossIndex < 0 ? string.Empty : split[bossIndex];

                script = tpl.script;
                layout = tpl.combat_layout_name;
            }
            else if (role == NPC_ROLE.ARENA_MODLE)//角斗场胜利者模型
            {
                var tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetMainLandEncounter(scene, locator);
                isappearing = tpl.is_appearing;
                appearingway = tpl.encounter_appearing_way;
                //ArenaModelData AData = ArenaManager.Instance.GetArenaModelData();
                string Tid;
                DataLookupsCache.Instance.SearchDataByID<string>(ArenaManager.ArenaModelDataId + ".templateId", out Tid);
                Tid = (Tid == null || Tid.CompareTo("") == 0) ? "15011" : Tid;

                string characterid = Hotfix_LT.Data.CharacterTemplateManager.Instance.TemplateidToCharacterid(Tid);
                int Skin;
                DataLookupsCache.Instance.SearchIntByID(ArenaManager.ArenaModelDataId + ".skin", out Skin);
                var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(characterid, Skin);
                string ModelId = charTpl.model_name; //需添加皮肤
                encounter = ModelId == null ? tpl.encounter_prefab : ModelId;
                script = tpl.script;
                layout = tpl.combat_layout_name;
            }
            else
            {

                var tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetMainLandEncounter(scene, locator);
				if(tpl != null){
					isappearing = tpl.is_appearing;
					appearingway = tpl.encounter_appearing_way;
					encounter = tpl.encounter_prefab;
					script = tpl.script;
					layout = tpl.combat_layout_name;
				}
            }
        }

		public DungeonPlacement(Hotfix_LT.Data.MainLandEncounterTemplate data)
		{
			locator = data.locator;
			role = data.role;
			isappearing = data.is_appearing;
			appearingway = data.encounter_appearing_way;
			encounter = data.encounter_prefab;
			script = data.script;
			layout = data.combat_layout_name;
			pos = string.Empty;
		}
	}

	public struct NPC_ROLE
	{
		public const string GHOST = "ghost";
		public const string WORLD_BOSS = "w_boss";
		public const string FUNC = "func";
		public const string GUARD = "guard";  //守卫，警卫
		public const string HANTED = "hanted";
		public const string BEAT_MONSTER = "m_enemy";
		public const string CAMPAIGN_ENEMY = "c_enemy";
		public const string ALLIANCE_CAMPAIGN_ENEMY = "a_enemy";
		public const string ALLIANCE_CAMPAIGN_BOSS = "a_boss";
		// public const string TRANSFER = "transfer";
		public const string CAMPAIGN_BOX = "c_box";
		public const string ARENA_MODLE = "ar_modle";
	}
}