using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterDB
{
	#region NESTED TYPES
	public class MetaData
	{
		public enum Class
		{
			cosmic = 0x01,
			mutant = 0x02,
			chemical = 0x04,
			mystic = 0x08,
			trained = 0x10,
			tech = 0x20
		}
		
		public enum Rarity
		{
			common = 0x1,
			uncommon = 0x2,
			rare = 0x4,
			epic = 0x8,
			legendary = 0x10
		}
		
		public enum Team
		{
			none = 0x1,
			avengers = 0x2,
			defenders = 0x4,
			guardians = 0x8,
			inhumans = 0x10,
			xmen = 0x20,
			aim = 0x40,
			brotherhood = 0x80,
			sinistersix = 0x100,
			thunderbolts = 0x200
		}
		
		public enum Affinity
		{
			neutral = 0x1,
			hero = 0x2,
			villain = 0x4
		}
		
		public enum Costume
		{
			classic = 0x1,
			modern = 0x2
		}

		public enum StatModifierModTypes
		{
			buff,
			debuff
		}
			
		public enum StatModifierTypes
		{
			fury,
			bleed,
			health_steal,
			health_gain,
			mana_steal,
			mana_gain,
			revive,
			heal,
			regen,
			armor_break,
			armor_up,
			health_bonus,
			crit_chance,
			crit_damage,
			stun,
			hit_stun,		// internal use only
			disable_run,	// internal use only
			disable_block,	// internal use only
		}

		public enum StatModifierAttributes
		{
			None,
			HP,
			Attack,
			Mana,
			ManaGain,
			BlockProficiency,
			PerfectBlockChance,
			MaxHP,
			CritChance,
			CritDamage,
			StunChance,
			Armor,
		}
		
		public enum ActivationTypes
		{
			none = 0,
			onLightHit = 1 << 0,
			onMediumHit = 1 << 1,
			onHeavyHit = 1 << 2,
			onNormalHit = onLightHit | onMediumHit | onHeavyHit,
			onSpecial1Hit = 1 << 3,
			onSpecial2Hit = 1 << 4,
			onSpecial3Hit = 1 << 5,
			onSpecialHit = onSpecial1Hit | onSpecial2Hit | onSpecial3Hit,
			onHit = onNormalHit | onSpecialHit,
			onStruck = 1 << 6,
			onDeath = 1 << 7,
			onFury = 1 << 9,
			onHealthSteal = 1 << 9,
			onManaSteal = 1 << 10,
			onCritBonus = 1 << 11
		}

		public enum TargetTypes
		{
			self = 1 << 0,
			opponent = 1 << 1,
			both = self | opponent
		}

		public enum ApplicationTypes
		{
			additive = 0x1,				//Special attribute amount adds to target attribute
			multiplicative = 0x2		//Special attribute amount factors target attribute
		}

		public MetaData(Hashtable data)
		{
			ID = EB.Dot.String("character_id", data, null);
			FriendlyName = EB.Dot.String ("character_friendly", data, null);
			eTeam = EB.Util.GetEnumValueFromString<Team>(EB.Dot.String("character_team", data, null));
			eClass = EB.Util.GetEnumValueFromString<Class>(EB.Dot.String("character_class", data, null));
			eRarity = EB.Util.GetEnumValueFromString<Rarity>(EB.Dot.String("character_rarity", data, null));
			eCostume = EB.Util.GetEnumValueFromString<Costume>(EB.Dot.String("character_costume", data, null));
			LevelMax = EB.Dot.Integer("character_max_level", data, 0);
			LevelStart = EB.Dot.Integer("start_level", data, 0);

			//TODO: Add all of the attributes..
		}

		public override string ToString ()
		{
			return string.Format("id : \t{0}\nfriendly : \t{1}\nteam : \t{2}\nclass : \t{3}\nrarity : \t{4}\ncostume : \t{5}\n", 
			                     ID, FriendlyName, eTeam, eClass, eRarity, eCostume);
		}

		public string				ID;
		public string		 		FriendlyName;
		public Team 				eTeam;
		public Class				eClass;
		public Rarity				eRarity;
		public Costume				eCostume;

		//Upgrade variables
		public int					CurrentISO;				//How much ISO-8 the Player has invested into the Hero.
		public int					LevelCurrent;
		public int					LevelMax;
		public int					LevelStart;
		public int					CurrentRank;
		public int					RequiredToLevel; 		//How much ISO-8 the Player needs to invest to level up.
		public int					EvoCost;
		public int					HeroRating;

		//ATTRIBUTES --
		//Basic attributes - Improve when Hero levels or ranks up.
		public int					HPCurrent;
		public int					HPMin;
		public int					HPMax;
		public int					AttackCurrent;
		public int					AttackMin;
		public int					AttackMax;
		//Advanced - Improves when Hero ranks up.
		public float				CritChance;
		public float				CritDamage;
		public float				PerfectBlockChance;
		public float				BlockProficiency;
		public int					ManaGain;
		public int					ManaBonus;
		public int 					HealthSteal;
		public int					ManaSteal;
		//Special - Affect basic and advanced attributes.
		public int					SpecialDuration;		//How long special abilities lastin seconds when activated. This only applies to 'over time' abilities. i.e., 'Bleed'
		public int					ReviveAmount;			//How much HP the Hero will have when brought back to life.
		public float				ReviveChance;		
		public ActivationTypes 		eReviveActivation;
		public ApplicationTypes		eReviveApplication;
		public int					HealAmount;				//How much HP is given to the Hero.
		public float				HealChance;
		public ActivationTypes		eHealActivation;
		public ApplicationTypes		eHealApplication;
		public int					StunAmount;				//How long the Hero is stunned for.
		public float				StunChance;
		public ActivationTypes		eStunActivation;
		public ApplicationTypes		eStunApplication;
		public int					BleedAmount;			//How much HP the Hero loses over (SpecialDuration) seconds.
		public float				BleedChance;
		public ActivationTypes		eBleedActivation;
		public ApplicationTypes		eBleedApplication;
		public int					ArmorBreakAmount;		//How much armor is ignored on the opponent. 'Armor penetration'
		public float				ArmorBreakChance;
		public ActivationTypes		eArmorBreakActivation;
		public ApplicationTypes		eArmorBreakApplication;
		public int					ArmorBoostAmount;		//How much the Hero's armor is increased buy for (SpecialDuration) seconds.
		public float				ArmorBoostChance;
		public ActivationTypes		eArmorBoostActivation;
		public ApplicationTypes		eArmorBoostApplication;
		public int					FuryAmount;				//How much outgoing damage is increased by for (SpecialDuration) seconds.
		public float				FuryChance;
		public ActivationTypes		eFuryActivation;
		public ApplicationTypes		eFuryApplication;
		public int					HealthBoostAmount;		//How much HP total is increased by for (SpecialDuration) seconds.
		public float				HealthBoostChance;
		public ActivationTypes		eHealthBoostActivation;
		public ApplicationTypes		eHealthBoostApplication;
		public int					ManaBoostAmount;		//How much Mana total is increased by for (SpecialDuration) seconds.
		public float				ManaBoostChance;
		public ActivationTypes		eManaBoostActivation;
		public ApplicationTypes		eManaBoostApplication;

		//Properties
		public bool CanEvolve{ get { return LevelCurrent >= LevelMax; } }
	}

	private class CharacterLoadingInfo
	{
		public string								_name;
		//public GameObject 	_go;

		private struct CallbackData
		{
			public System.Action<GameObject, object>		_cb;
			public object 								_clientReadyData;
		}

		private List<CallbackData> 		_cbs	 = new List<CallbackData>();

		public CharacterLoadingInfo(string name, System.Action<GameObject, object> cb, object clientReadyData)
		{
			_name = name;
			AddCB(cb, clientReadyData);
		}
		
		public void AddCB(System.Action<GameObject, object> cb, object clientReadyData)
		{
			_cbData._clientReadyData = clientReadyData;
			_cbData._cb = cb;
			_cbs.Add(_cbData);
		}
		
		public void OnLoaded(GameObject g)
		{
			for (int i=0; i<_cbs.Count; ++i)
			{
				_cbs[i]._cb(g, _cbs[i]._clientReadyData);
			}
		}

		private CallbackData	_cbData;
	}
	#endregion

	#region PUBLIC MEMBERS
	#endregion

	#region PRIVATE MEMBERS
	private static int 								_availableCount = -1;
	private static string 							_characterRoot = "Bundles/Characters/Merged/";
	private static string 							_warmupCharacter = "cubeman";
	private static GameObject						_fallbackDudePrefab;
	private static List<CharacterLoadingInfo> 		_loads = new List<CharacterLoadingInfo>();
	private static List<MetaData>					_characters = new List<MetaData>();
	private static Dictionary<string, GameObject>	_characterRegistry = new Dictionary<string, GameObject>();
	#endregion

	#region PROPERTIES
	public static GameObject FallbackDude{get{return _fallbackDudePrefab;}}
	public static int CharacterCount{get{return _characters.Count;}}
	private static bool Initialized{get; set;}
	#endregion

	#region HELPER METHODS
	public static MetaData GetCharacterDataByID(string id)
	{
		if (string.IsNullOrEmpty(id)) { EB.Debug.LogWarning ("ERROR -- GetCharacterDataByID() -- Invalid ID!"); return null;}
		if (_characters == null) { EB.Debug.LogWarning ("ERROR -- GetCharacterDataByID() -- Null character data list!"); return null;}

		for (int i = 0; i < _characters.Count; i++)
		{
			if (_characters[i] == null) { continue; }
			if (_characters[i].ID == id) { return _characters[i]; }
		}

		EB.Debug.LogWarning (string.Format("ERROR -- GetCharacterDataByID() -- Character of ID ({0}) does not exist!", id));
		return null;
	}

	public static IEnumerable Characters()
	{
		for (int i=0; i<_characters.Count; ++i)
		{
			yield return _characters[i];
		}
	}

	public static float CharacterHeight(GameObject actor)
	{
		if (actor)
		{
			Animator a = actor.GetComponent<Animator>();
		
			if (a)
			{
				EB.Debug.Log(string.Format("Returning height of {0} as {1}", actor.name,  a.GetBoneTransform(HumanBodyBones.Head).position.y));
				return a.GetBoneTransform(HumanBodyBones.Head).position.y;
			}
		}
		EB.Debug.LogWarning("Character height was called on either a null actor reference or one that contained no Animator on the top level");
		return 2.0f;
	}

	public static float FillPercent(GameObject actor, Camera c)
	{
		if (actor && c)
		{
			Animator a = actor.GetComponent<Animator>();

			Vector3 vptop, vpbot;

			VP_Extents(a, c, out vptop, out vpbot);

			return Vector3.Distance(vptop, vpbot);
		}

		return 0.8f;
	}

	public static void VP_Extents(Animator a, Camera c, out Vector3 vptop, out Vector3 vpbot)
	{
		Vector3 top = a.GetBoneTransform(HumanBodyBones.Head).position;
		Vector3 bot = Vector3.Lerp (a.GetBoneTransform(HumanBodyBones.LeftFoot).position, a.GetBoneTransform(HumanBodyBones.RightFoot).position, 0.5f);
		
		// project these fuckers onto the view plane
		vptop = c.WorldToViewportPoint(top);
		vpbot = c.WorldToViewportPoint(bot);
	}

	public static Vector3 PointAtActor(GameObject actor, Camera c, float topBuffer, float botBuffer)
	{
		if (actor && c)
		{
			Animator a = actor.GetComponent<Animator>();
			
			Vector3 vptop, vpbot;
			
			VP_Extents(a, c, out vptop, out vpbot);

			return PointAt(c, vpbot, botBuffer);
		}

		return c.transform.position + c.transform.forward*10.0f;
	}

	public static Vector3 PointAt(GameObject go, Camera c, float botBuffer)
	{
		if (go && c)
		{	
			Vector3 vpbot = c.WorldToViewportPoint(go.transform.position);
			
			return PointAt(c, vpbot, botBuffer);
		}
		
		return c.transform.position + c.transform.forward*10.0f;
	}

	public static Vector3 PointAt(Camera c, Vector3 botAnchor, float botBuffer)
	{
		Vector3 vplookat = botAnchor + (0.5f-botBuffer)*c.transform.up;
		
		vplookat.z = Vector3.Distance(botAnchor, c.transform.position);
		
		return c.ViewportToWorldPoint(vplookat);
	}

	public static void CharacterObject(string characterName, System.Action<GameObject, object> OnReady, object clientReadyData)
	{
		GameObject character;
		if (_characterRegistry.TryGetValue(characterName, out character))
		{
			// An entry exists - do we need to load the character?
			if (character == null)
			{
				//EB.Debug.Log ("REQUESTING {0} BUT IS NULL...LOADING", characterName);
				// Are we already loading it?
				CharacterLoadingInfo loadingInfo = _loads.Find(delegate(CharacterLoadingInfo info) {return info._name.Equals(characterName);});

				if (loadingInfo == null)
				{
					//EB.Debug.Log("NOT ALREADY LOADING {0}....REALLY LOADING", characterName);
					_loads.Add(new CharacterLoadingInfo(characterName, OnReady, clientReadyData));
					/* troyhack
					EB.Assets.LoadAsync(_characterRoot+characterName, typeof(GameObject), delegate(Object o) {
						// do my stuff
						GameObject go = o as GameObject;

						_characterRegistry[characterName] = go;
						// Let client(s) know
						//EB.Debug.Log("LOADED {0} FROM BUNDLE : o {1}, go {2}", characterName, o, go);

						OnCharacterReady(characterName, go);
					}, false);
					*/
				}
				else
				{
					//EB.Debug.Log("{0} IS ALREADY LOADING FROM BUNDLE...", characterName);  
					loadingInfo.AddCB(OnReady, clientReadyData);

				}
			}
			else
			{
				//EB.Debug.Log ("{0} IS ALREADY LOADED...RETURNING {1}", characterName, character);
				// Call the client's callback right now
				OnReady(character, clientReadyData);
			}
		}
		else
		{
			EB.Debug.LogError("CHARACTERDB - no such character {0} exists", characterName);
		}
	}
	

	public static IEnumerator CleanRegistry(string[] saveList)
	{
		Dictionary<string, GameObject>.KeyCollection registryKeys = _characterRegistry.Keys;

		string[] keys = new string[registryKeys.Count];
		registryKeys.CopyTo(keys, 0);

		// If no characters have been marked as sacrosanct, clear them all
		if (saveList == null)
		{
		_characterRegistry.Clear ();
		}
		else // else, clear all but those in the save list
		{
			foreach (string key in keys)
			{
				if (!System.Array.Exists(saveList, delegate(string s) {return s==key;}))
				{
					_characterRegistry.Remove (key);
				}
			}
		}

		EB.Debug.Log("CleanRegistry: Running cleanup task");
		yield return Resources.UnloadUnusedAssets();

		// Resurrect the registry
		foreach (string k in keys)
		{
			if (!_characterRegistry.ContainsKey(k))
			{
			_characterRegistry.Add(k, null);
		}
	}
	}

	private static void OnCharacterReady(string characterName, GameObject go)
	{
		CharacterLoadingInfo loadingInfo = _loads.Find(delegate(CharacterLoadingInfo info) {return info._name.Equals(characterName);});
		if (loadingInfo != null)
		{
			//EB.Debug.Log ("CHARACTER {0} IS READY...RETURNING {1}", characterName, go);
			loadingInfo.OnLoaded(go);
		}

		_loads.Remove(loadingInfo);
	}

	private static void WarmupCharacterShader()
	{
		EB.Assets.LoadAsync(_characterRoot + _warmupCharacter, typeof(GameObject), delegate (Object o)
		{
			if(o){
				GameObject go = o as GameObject;
				_fallbackDudePrefab = go;
			}
		});
	}

#if GRAEME
	private static void DumpCharacters()
	{
		foreach(MetaData m in _characters)
		{
			Debug.Log(m);
		}
	}
#endif
	#endregion
}
