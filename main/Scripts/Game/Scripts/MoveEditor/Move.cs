using UnityEngine;
using System.Collections.Generic;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MoveEditor
{
#region Event Property Class Definitions

	[System.Serializable]
	public class MoveEventProperties
	{
		public string 	_name						= string.Empty;
		public int 		_numHitEvents 				= 0;
		public int 		_numProjectileEvents		= 0;

		public string Serialize()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(string.Format("mn={0};", _name));
			sb.Append(string.Format("nhe={0};", _numHitEvents.ToString()));
			sb.Append(string.Format("npe={0};", _numProjectileEvents.ToString()));

			return sb.ToString();
		}

		public static MoveEventProperties Deserialize(string propertyString)
		{
			Dictionary<string, string> parsed = MoveUtils.ParseEventProperties(propertyString);
			return Deserialize(parsed);
		}
		
		public static MoveEventProperties Deserialize(Dictionary<string, string> parsedProperties)
		{
			MoveEventProperties properties = new MoveEventProperties();
			
			properties._name = MoveUtils.ParseStringParam( parsedProperties, "mn" );
			properties._numHitEvents = MoveUtils.ParseIntParam( parsedProperties, "nhe" );
			properties._numProjectileEvents = MoveUtils.ParseIntParam( parsedProperties, "npe" );
	
			return properties;
		}
	}

	[System.Serializable]
	public class DamageEventProperties
	{
		public int			_hitEventIndex		= 0;
		public AttackLevel _attackLevel			= AttackLevel.kAttackInvalid;
		public float 		_damage				= 0;
		public float 		_mana				= 0;
		public float 		_range				= 1.0f;
		public float		_angle				= 0.0f;
		public BodyPart		_pointOfOrigin		= BodyPart.Hips;
		public Vector3		_offset 			= Vector3.zero;
		public float 		_knockback			= 0;
		public float 		_blockKnockback		= -1.0f;
		public float 		_hitStun 			= 0;
		public float		_blockStun			= 0;
		public string 		_hitReaction		= string.Empty;
		public bool			_unblockable		= false;
		public float		_freezeTime			= 0.1f;
		public float		_freezeTimeBlocking = 0.1f;

		public string Serialize()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(string.Format("hix={0};", _hitEventIndex.ToString()));
			sb.Append(string.Format("al={0};", _attackLevel.ToString()));
			sb.Append(string.Format("d={0};", _damage.ToString()));
			sb.Append(string.Format("m={0};", _mana.ToString()));
			sb.Append(string.Format("r={0};", _range.ToString()));
			sb.Append(string.Format("ang={0};", _angle.ToString()));
			sb.Append(string.Format("poo={0};", _pointOfOrigin.ToString()));
			sb.Append(string.Format("of3={0};", _offset.ToString()));
			sb.Append(string.Format("kb={0};", _knockback.ToString()));
			sb.Append(string.Format("bkb={0};", _blockKnockback.ToString()));
			sb.Append(string.Format("hs={0};", _hitStun.ToString()));
			sb.Append(string.Format("hr={0};", _hitReaction));
			sb.Append(string.Format("ub={0};", _unblockable.ToString()));
			sb.Append(string.Format("ft={0};", _freezeTime.ToString()));
			sb.Append(string.Format("ftb={0};", _freezeTimeBlocking.ToString()));
			sb.Append(string.Format("bdt={0};", _blockStun.ToString()));

			return sb.ToString();
		}
		
		public static DamageEventProperties Deserialize(string propertyString)
		{
			Dictionary<string, string> parsed = MoveUtils.ParseEventProperties(propertyString);
			return Deserialize(parsed);
		}
		
		public static DamageEventProperties Deserialize(Dictionary<string, string> parsedProperties)
		{
			DamageEventProperties properties = new DamageEventProperties();
					
			properties._hitEventIndex = MoveUtils.ParseIntParam( parsedProperties, "hix" );
			properties._attackLevel = MoveUtils.ParseEnumParam<AttackLevel>( parsedProperties, "al", AttackLevel.kAttackLight );
			properties._damage = MoveUtils.ParseFloatParam( parsedProperties, "d" );
			properties._mana = MoveUtils.ParseFloatParam( parsedProperties, "m" );
			properties._range = MoveUtils.ParseFloatParam( parsedProperties, "r", 1.0f );
			properties._angle = MoveUtils.ParseFloatParam( parsedProperties, "ang" );
			properties._pointOfOrigin = MoveUtils.ParseEnumParam<BodyPart>( parsedProperties, "poo", BodyPart.Hips );
			properties._offset = MoveUtils.ParseVector3Param( parsedProperties, "of3", Vector3.zero );
			properties._knockback = MoveUtils.ParseFloatParam( parsedProperties, "kb" );
			properties._blockKnockback = MoveUtils.ParseFloatParam( parsedProperties, "bkb", -1.0f );
			properties._hitStun = MoveUtils.ParseFloatParam( parsedProperties, "hs" );
			properties._hitReaction = MoveUtils.ParseStringParam( parsedProperties, "hr", string.Empty );
			properties._unblockable = MoveUtils.ParseBoolParam( parsedProperties, "ub" );
			properties._freezeTime = MoveUtils.ParseFloatParam( parsedProperties, "ft" );
			properties._freezeTimeBlocking = MoveUtils.ParseFloatParam( parsedProperties, "ftb" );
			properties._blockStun = MoveUtils.ParseFloatParam( parsedProperties, "bdt" );

			return properties;
		}
	}
	[System.Serializable]
	public class PlayHitReactionProperties
	{
		public enum eReactionType
		{
			Hit,
			Back,
			Down,
			LittleFlow,
			LargeFlow,
			Block
		}

		public enum ePlayPriorityType
		{
			UseDefault,
			UseEffect,
		}

		public float framesToPlay = -1.0f;
		public float startAngle;
		public float endAngle;
		public eReactionType defaultReaction = eReactionType.Hit;
		public ePlayPriorityType playPriority = ePlayPriorityType.UseDefault;
		public int weight = 1;
        public bool isOnlyPlayEffect;
		public int totalWeight;
		public int hitNum;
		
		public string Serialize()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format("framesToPlay={0};", framesToPlay.ToString()));
			sb.Append(string.Format("startAngle={0};", startAngle.ToString()));
			sb.Append(string.Format("endAngle={0};", endAngle.ToString()));

			sb.Append(string.Format("defaultReaction={0};", defaultReaction.ToString()));
			sb.Append(string.Format("playPriority={0};", playPriority.ToString()));
			sb.Append(string.Format("weight={0};", weight.ToString()));
            sb.Append(string.Format("isOnlyPlayEffect={0};", isOnlyPlayEffect.ToString()));
			return sb.ToString();
		}
		
		public static PlayHitReactionProperties Deserialize(string propertyString)
		{
			Dictionary<string, string> parsed = MoveUtils.ParseEventProperties(propertyString);
			return Deserialize(parsed);
		}
		
		public static PlayHitReactionProperties Deserialize(Dictionary<string, string> parsedProperties)
		{
			PlayHitReactionProperties properties = new PlayHitReactionProperties();
			properties.framesToPlay = MoveUtils.ParseFloatParam(parsedProperties, "framesToPlay");
			properties.startAngle = MoveUtils.ParseFloatParam(parsedProperties, "startAngle");
			properties.endAngle = MoveUtils.ParseFloatParam(parsedProperties, "endAngle");

			properties.defaultReaction = MoveUtils.ParseEnumParam(parsedProperties, "defaultReaction", eReactionType.Hit);
			properties.playPriority = MoveUtils.ParseEnumParam(parsedProperties, "playPriority", ePlayPriorityType.UseDefault);
			properties.weight = MoveUtils.ParseIntParam(parsedProperties, "weight");
            properties.isOnlyPlayEffect = MoveUtils.ParseBoolParam(parsedProperties, "isOnlyPlayEffect");
            return properties;
		}
	}

	[System.Serializable]
	public class PauseProperties
	{
		public enum PAUSE_TYPE
		{
			PAUSE_TYPE_SELF,
			PAUSE_TYPE_OTHERS,
		};
		public float fPauseTime = 1.0f;
		public PAUSE_TYPE type = PAUSE_TYPE.PAUSE_TYPE_SELF;

		public string Serialize()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format("fPauseTime={0};", fPauseTime.ToString()));
			sb.Append(string.Format("type={0};", type.ToString()));
			return sb.ToString();
		}

		public static PauseProperties Deserialize(string propertyString)
		{
			Dictionary<string, string> parsed = MoveUtils.ParseEventProperties(propertyString);
			return Deserialize(parsed);
		}

		public static PauseProperties Deserialize(Dictionary<string, string> parsedProperties)
		{
			PauseProperties properties = new PauseProperties();
			properties.fPauseTime = MoveUtils.ParseFloatParam(parsedProperties, "fPauseTime");
			properties.type = MoveUtils.ParseEnumParam<PAUSE_TYPE>(parsedProperties, "type", PAUSE_TYPE.PAUSE_TYPE_SELF);
			return properties;
		}
	}

	[System.Serializable]
	public class AnimationSpeedEventProperties
	{
		public float _plusMinus = 0.0f;
		
		public string Serialize()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format("pm={0};", _plusMinus.ToString()));
			return sb.ToString();
		}
		
		public static AnimationSpeedEventProperties Deserialize(string propertyString)
		{
			Dictionary<string, string> parsed = MoveUtils.ParseEventProperties(propertyString);
			return Deserialize(parsed);
		}
		
		public static AnimationSpeedEventProperties Deserialize(Dictionary<string, string> parsedProperties)
		{
			AnimationSpeedEventProperties properties = new AnimationSpeedEventProperties();
			properties._plusMinus = MoveUtils.ParseFloatParam(parsedProperties, "pm");
			return properties;
		}
	}
	[System.Serializable]
	public class BuffEventProperties
	{
		public float _lastFrame = 0.0f;
		
		public string Serialize()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format("_lastFrame={0};", _lastFrame.ToString()));
			return sb.ToString();
		}
		
		public static BuffEventProperties Deserialize(string propertyString)
		{
			Dictionary<string, string> parsed = MoveUtils.ParseEventProperties(propertyString);
			return Deserialize(parsed);
		}
		
		public static BuffEventProperties Deserialize(Dictionary<string, string> parsedProperties)
		{
			BuffEventProperties properties = new BuffEventProperties();
			properties._lastFrame = MoveUtils.ParseFloatParam(parsedProperties, "_lastFrame");
			return properties;
		}
	}

	// 
	[System.Serializable]
	public class TimeScaleProperties
	{
		public AnimationCurve timeScaleCurve = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);
		
		public string Serialize()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format("timeScaleCurve={0};", MoveUtils.StringifyAnimationCurve(timeScaleCurve)));
			return sb.ToString();
		}
		
		public static TimeScaleProperties Deserialize(string propertyString)
		{
			Dictionary<string, string> parsed = MoveUtils.ParseEventProperties(propertyString);
			return Deserialize(parsed);
		}
		
		public static TimeScaleProperties Deserialize(Dictionary<string, string> parsedProperties)
		{
			TimeScaleProperties properties = new TimeScaleProperties();
			properties.timeScaleCurve = MoveUtils.ParseAnimationCurveParam(parsedProperties, "timeScaleCurve", AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f));
			
			return properties;
		}
	}
	[System.Serializable]
	public class CombatEventProperties
	{
		public float _endFrame = -1.0f;

		private StringBuilder _sb = new StringBuilder(32);

		public string Serialize()
		{
			_sb.Clear();
			_sb.Append("_endFrame=");
			_sb.Append(_endFrame);
			_sb.Append(";");

			return _sb.ToString();
		}

		public static CombatEventProperties Deserialize(string propertyString)
		{
			Dictionary<string, string> parsed = MoveUtils.ParseEventProperties(propertyString);
			return Deserialize(parsed);
		}
		
		public static CombatEventProperties Deserialize(Dictionary<string, string> parsedProperties)
		{
			CombatEventProperties properties = new CombatEventProperties();
			properties._endFrame = MoveUtils.ParseFloatParam(parsedProperties, "_endFrame");
			return properties;
		}
	}

    [System.Serializable]
	public class ParticleEventProperties
	{
		public enum ePartition
		{
			None,
			Head,
			Armor,
			Weapon
		}

		public string			_eventName			= string.Empty;
		public bool				_interruptable		= true;
		public bool				_stopOnExit 		= true;
		public bool             _stopOnOverride     = true;
		public bool				_stopOnDuration		= false;
		public float			_duration			= 0.0f;
		public bool             _stopOnEndTurn      = false;
		public bool             _stopAfterTurns     = false;
		public int              _turns              = 0;
		public bool             _cancelIfHit        = false;
		public bool				_killIfBlocked		= false;
		public bool				_cancelIfMissed		= false;
		public string           _partition          = string.Empty;
		public bool 			_applyOnTargetList  = false;
		//public ParticleSystem	_particle			= null;
		//public ParticleSystem	_flippedParticle	= null;
		public ParticleSystemReference _particleReference = new ParticleSystemReference();
		public ParticleSystemReference _flippedParticleReference = new ParticleSystemReference();
		public bool             _spawnAtOpponent    = false;
		public bool				_spawnAtHitPoint	= false;
		public bool				_spawnAtTargetBase  = false;
		public bool				_attachToOpponent	= false;
		public BodyPart 		_bodyPart			= BodyPart.Root;
		public string			_attachmentPath		= "";
		public bool				_parent				= false;
		public bool				_lockRotation		= false;
		public Vector3			_offset				= Vector3.zero;
		public bool				_lockXOffset		= false;
		public HeightLock		_lockYOffset		= HeightLock.Unlocked;
		public bool				_lockZOffset		= false;
		public Vector3			_angles				= Vector3.zero;
		public bool				_worldSpace			= false;
		public Vector3			_scale 				= Vector3.one;

        /// <summary>
        /// 是否是常驻特效
        /// </summary>
        public bool IsResidentFX()
        {
            return !string.IsNullOrEmpty(_partition);
        }

		#region cpu 11ms
		private StringBuilder _sb = new StringBuilder(256);
		private void SBAppend<T>(string s1, T s2)
		{
			_sb.Append(s1);
			_sb.Append(s2);
			_sb.Append(";");
		}
		public string Serialize()
		{
			SBAppend("ps=", _particleReference.Valiad ? _particleReference.Name : _particleName);
			SBAppend("fps=", _flippedParticleReference.Valiad ? _flippedParticleReference.Name : _flippedParticleName);
			SBAppend("int=", _interruptable);
			SBAppend("soo=", _stopOnOverride);
			SBAppend("soe=", _stopOnExit);
			SBAppend("soet=", _stopOnEndTurn);
			SBAppend("sod=", _stopOnDuration);
			SBAppend("dur=", _duration);
			SBAppend("sat=", _stopAfterTurns);
			SBAppend("tus=", _turns);
			SBAppend("cih=", _cancelIfHit);
			SBAppend("cib=", _killIfBlocked);
			SBAppend("cim=", _cancelIfMissed);
			SBAppend("en=", 	_eventName);
			SBAppend("sao=", _spawnAtOpponent);
			SBAppend("shp=", _spawnAtHitPoint);
			SBAppend("satb=", _spawnAtTargetBase);
			SBAppend("ato=", _attachToOpponent);
			SBAppend("bp=", 	_bodyPart);
			SBAppend("pap=", _attachmentPath);
			SBAppend("p=",	_parent);
			SBAppend("plr=", _lockRotation);
			SBAppend("o=", 	_offset);
			SBAppend("lxo=",	_lockXOffset);
			SBAppend("lyo=",	_lockYOffset);
			SBAppend("lzo=",	_lockZOffset);
			SBAppend("a=", 	_angles);
			SBAppend("ws=", 	_worldSpace);
			SBAppend("sc=", 	_scale);
			SBAppend("aotl=", _applyOnTargetList);
			SBAppend("pp=", _partition);
			
			return _sb.ToString();
		}
		#endregion
		
		public static ParticleEventProperties Deserialize(string propertyString)
		{
			Dictionary<string, string> parsed = MoveUtils.ParseEventProperties(propertyString);
			return Deserialize(parsed);
		}
		
		public static ParticleEventProperties Deserialize(Dictionary<string, string> parsedProperties)
		{
			ParticleEventProperties properties = new ParticleEventProperties();
	
			properties._particleName 		= MoveUtils.ParseStringParam( parsedProperties, "ps" );
			properties._flippedParticleName = MoveUtils.ParseStringParam( parsedProperties, "fps");
			properties._interruptable 		= MoveUtils.ParseBoolParam( parsedProperties, "int" );
			properties._stopOnOverride      = MoveUtils.ParseBoolParam(parsedProperties, "soo");
			properties._stopOnExit 			= MoveUtils.ParseBoolParam( parsedProperties, "soe" );
			properties._stopOnEndTurn       = MoveUtils.ParseBoolParam(parsedProperties, "soet");
			properties._stopOnDuration		= MoveUtils.ParseBoolParam( parsedProperties, "sod" );
			properties._duration			= MoveUtils.ParseFloatParam( parsedProperties, "dur" );
			properties._stopAfterTurns      = MoveUtils.ParseBoolParam(parsedProperties, "sat");
			properties._turns               = MoveUtils.ParseIntParam(parsedProperties, "tus");
			properties._cancelIfHit         = MoveUtils.ParseBoolParam(parsedProperties, "cih");
			properties._killIfBlocked 		= MoveUtils.ParseBoolParam( parsedProperties, "cib" );
			properties._cancelIfMissed		= MoveUtils.ParseBoolParam( parsedProperties, "cim" );
			properties._applyOnTargetList	= MoveUtils.ParseBoolParam( parsedProperties, "aotl" );
			properties._eventName 			= MoveUtils.ParseStringParam( parsedProperties, "en", string.Empty );
			properties._spawnAtOpponent     = MoveUtils.ParseBoolParam(parsedProperties, "sao");
			properties._spawnAtHitPoint 	= MoveUtils.ParseBoolParam( parsedProperties, "shp" );
			properties._spawnAtTargetBase   = MoveUtils.ParseBoolParam( parsedProperties, "satb");
			properties._attachToOpponent 	= MoveUtils.ParseBoolParam( parsedProperties, "ato" );
			properties._bodyPart 			= MoveUtils.ParseEnumParam<BodyPart>( parsedProperties, "bp", BodyPart.Root );
			properties._attachmentPath 		= MoveUtils.ParseStringParam( parsedProperties, "pap", "" );
			properties._parent 				= MoveUtils.ParseBoolParam( parsedProperties, "p" );
			properties._lockRotation 		= MoveUtils.ParseBoolParam(parsedProperties, "plr");
			properties._offset 				= MoveUtils.ParseVector3Param( parsedProperties, "o", Vector3.zero );
			properties._lockXOffset			= MoveUtils.ParseBoolParam(parsedProperties, "lxo", false);
			properties._lockYOffset			= MoveUtils.ParseEnumParam<HeightLock>(parsedProperties, "lyo", HeightLock.Unlocked);
			properties._lockZOffset			= MoveUtils.ParseBoolParam(parsedProperties, "lzo", false);
			properties._angles 				= MoveUtils.ParseVector3Param( parsedProperties, "a", Vector3.zero );
			properties._worldSpace 			= MoveUtils.ParseBoolParam ( parsedProperties, "ws" );
			properties._scale 				= MoveUtils.ParseVector3Param( parsedProperties, "sc", Vector3.one );
			properties._partition			= MoveUtils.ParseStringParam(parsedProperties, "pp", string.Empty);
			
			return properties;
		}
		
		public string ParticleName { get { return _particleName; } set { _particleName = value; } }
		private string _particleName;

		public string FlippedParticleName { get { return _flippedParticleName; } set { _flippedParticleName = value; } }
		private string _flippedParticleName;

        public override string ToString()
        {
            return string.Format("特效信息[_particleName:{0},_flippedParticleName:{1},_eventName:{2},_partition:{3},_attachmentPath:{4}]", _particleName, _flippedParticleName, _eventName,
                _partition, _attachmentPath);
        }
    }
	
	[System.Serializable]
	public class TrailRendererEventProperties
	{
		public GameObject		_trailRenderer		= null;
		public GameObject		_trailRendererCrit	= null;
		public bool 			_isInterruptible	= true;
		public string			_eventName			= string.Empty;
		public GameObject		_rigPrefab			= null;
		public AnimationClip	_rigAnimClip		= null;
		public BodyPart			_attachment 		= BodyPart.Root;
		public string			_attachmentPath		= "";
		public bool				_parent				= false;
		public bool				_lockRotation		= false;
		public Vector3			_rigOffset			= Vector3.zero;
		public bool				_lockXOffset		= false;
		public HeightLock		_lockYOffset		= HeightLock.Unlocked;
		public bool				_lockZOffset		= false;
		public bool				_useOffsets			= false;
		public Vector3			_offset1			= Vector3.zero;
		public Vector3			_offset2 			= Vector3.zero;
		public bool				_useLocalOffsets	= false;
		public Vector3			_angles				= Vector3.zero;
		public bool				_worldSpace			= false;
		public float			_fps 				= 30.0f;
		
		public string Serialize()
		{
			if (!_useOffsets)
			{
				_offset1 = Vector3.zero;
				_offset2 = Vector3.zero;
			}

			StringBuilder sb = new StringBuilder();
			string rigPrefabName = _rigPrefab ? _rigPrefab.name : "";
			string rigAnimClipName = _rigAnimClip ? _rigAnimClip.name : "";

			sb.Append(string.Format("trn={0};", _trailRenderer.name));
			sb.Append(string.Format("trc={0};", _trailRendererCrit.name));
			sb.Append(string.Format("int={0};", _isInterruptible.ToString()));
			sb.Append(string.Format("en={0};", 	_eventName));
			sb.Append(string.Format("rpn={0};", rigPrefabName));
			sb.Append(string.Format("ran={0};", rigAnimClipName));
			sb.Append(string.Format("atc={0};", _attachment.ToString()));
			sb.Append(string.Format("tap={0};", _attachmentPath));
			sb.Append(string.Format("p={0};", 	_parent.ToString()));
			sb.Append(string.Format("tlr={0};", _lockRotation.ToString()));
			sb.Append(string.Format("rof={0};", _rigOffset.ToString()));
			sb.Append(string.Format("lxo={0};",	_lockXOffset.ToString()));
			sb.Append(string.Format("lyo={0};",	_lockYOffset.ToString()));
			sb.Append(string.Format("lzo={0};",	_lockZOffset.ToString()));
			sb.Append(string.Format("uof={0};", _useOffsets.ToString()));
			sb.Append(string.Format("of1={0};", _offset1.ToString()));
			sb.Append(string.Format("of2={0};", _offset2.ToString()));
			sb.Append(string.Format("ulo={0};", _useLocalOffsets.ToString()));
			sb.Append(string.Format("tra={0};", _angles.ToString()));
			sb.Append(string.Format("tws={0};", _worldSpace.ToString()));
			sb.Append(string.Format("tfs={0};", _fps.ToString()));
			
			return sb.ToString();
		}
		
		public static TrailRendererEventProperties Deserialize(string propertyString)
		{
			Dictionary<string, string> parsed = MoveUtils.ParseEventProperties(propertyString);
			return Deserialize(parsed);
		}
		
		public static TrailRendererEventProperties Deserialize(Dictionary<string, string> parsedProperties)
		{
			TrailRendererEventProperties properties = new TrailRendererEventProperties();

			properties._trailRendererName 	= MoveUtils.ParseStringParam( parsedProperties, "trn" );
			properties._trailRendererNameCrit = MoveUtils.ParseStringParam( parsedProperties, "trc" );
			properties._isInterruptible 	= MoveUtils.ParseBoolParam( parsedProperties, "int", true );
			properties._rigPrefabName 		= MoveUtils.ParseStringParam( parsedProperties, "rpn" );
			properties._rigAnimClipName 	= MoveUtils.ParseStringParam( parsedProperties, "ran" );
			properties._attachment 			= MoveUtils.ParseEnumParam<BodyPart>( parsedProperties, "atc", BodyPart.Root );
			properties._attachmentPath 		= MoveUtils.ParseStringParam( parsedProperties, "tap" );
			properties._parent 				= MoveUtils.ParseBoolParam( parsedProperties, "p" );
			properties._lockRotation 		= MoveUtils.ParseBoolParam(parsedProperties, "tlr");
			properties._rigOffset 			= MoveUtils.ParseVector3Param(parsedProperties, "rof", Vector3.zero);
			properties._lockXOffset			= MoveUtils.ParseBoolParam(parsedProperties, "lxo", false);
			properties._lockYOffset			= MoveUtils.ParseEnumParam<HeightLock>(parsedProperties, "lyo", HeightLock.Unlocked);
			properties._lockZOffset			= MoveUtils.ParseBoolParam(parsedProperties, "lzo", false);
			properties._offset1 			= MoveUtils.ParseVector3Param(parsedProperties, "of1", Vector3.zero);
			properties._offset2 			= MoveUtils.ParseVector3Param(parsedProperties, "of2", Vector3.zero);
			properties._useLocalOffsets 	= MoveUtils.ParseBoolParam(parsedProperties, "ulo", false);
			properties._angles 				= MoveUtils.ParseVector3Param(parsedProperties, "tra", Vector3.zero);
			properties._worldSpace 			= MoveUtils.ParseBoolParam( parsedProperties, "tws" );
			properties._fps 				= MoveUtils.ParseFloatParam( parsedProperties, "tfs" ); 
			
			return properties;
		}
		public string TrailRendererName { get { return _trailRendererName; } }
		private string _trailRendererName;

		public string TrailRendererNameCrit{ get{ return _trailRendererNameCrit; } }
		private string _trailRendererNameCrit;

		public string RigPrefabName { get { return _rigPrefabName; } }
		private string _rigPrefabName;

		public string RigAnimClipName { get { return _rigAnimClipName; } }
		private string _rigAnimClipName;
	}

	[System.Serializable]
	public class DynamicLightEventProperties
	{
		public GameObject		_dynamicLight	= null;
		public bool 			_interruptable	= false;
		public bool				_stopOnExit		= false;
		public string			_eventName		= string.Empty;
		public BodyPart			_attachment 	= BodyPart.Root;
		public string			_attachmentPath	= "";
		public bool				_parent			= false;
		public Vector3			_offset			= Vector3.zero;

		public string Serialize()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format("dln={0};", _dynamicLight != null ? _dynamicLight.name : _dynamicLightName));
			sb.Append(string.Format("int={0};", _interruptable.ToString()));
			sb.Append(string.Format("soe={0};", _stopOnExit.ToString()));
			sb.Append(string.Format("en={0};", 	_eventName));
			sb.Append(string.Format("atc={0};", _attachment.ToString()));
			sb.Append(string.Format("lap={0};", _attachmentPath));
			sb.Append(string.Format("p={0};", 	_parent.ToString()));
			sb.Append(string.Format("os={0};", 	_offset.ToString()));
			return sb.ToString();
		}
		
		public static DynamicLightEventProperties Deserialize(string propertyString)
		{
			Dictionary<string, string> parsed = MoveUtils.ParseEventProperties(propertyString);
			return Deserialize(parsed);
		}
		
		public static DynamicLightEventProperties Deserialize(Dictionary<string, string> parsedProperties)
		{
			DynamicLightEventProperties properties = new DynamicLightEventProperties();

			properties._dynamicLightName 	= MoveUtils.ParseStringParam(parsedProperties, "dln");
			properties._interruptable 		= MoveUtils.ParseBoolParam(parsedProperties, "int");
			properties._stopOnExit			= MoveUtils.ParseBoolParam(parsedProperties, "soe");
			properties._eventName 			= MoveUtils.ParseStringParam(parsedProperties, "en", string.Empty);
			properties._attachment 			= MoveUtils.ParseEnumParam<BodyPart>(parsedProperties, "atc", BodyPart.Root); 
			properties._attachmentPath 		= MoveUtils.ParseStringParam(parsedProperties, "lap", string.Empty);
			properties._parent 				= MoveUtils.ParseBoolParam(parsedProperties, "p");
			properties._offset 				= MoveUtils.ParseVector3Param(parsedProperties, "os", Vector3.zero);
			
			return properties;
		}

		public string DynamicLightName { get { return _dynamicLightName; } }
		private string _dynamicLightName;
	
	}
		
	[System.Serializable]
	public class CameraShakeEventProperties
	{
		public string	_presetName				= string.Empty;
		public int		_numberOfShakes			= 0;
		public Vector3 	_shakeAmount			= Vector3.zero;
		public Vector3 	_rotationAmount			= Vector3.zero;
		public float 	_distance				= 0;
		public float 	_speed					= 0;
		public float 	_decay					= 0;		
		public int		_numberOfShakesCrit		= 0;
		public Vector3 	_shakeAmountCrit		= Vector3.zero;
		public Vector3	_rotationAmountCrit		= Vector3.zero;
		public float	_distanceCrit			= 0;
		public float	_speedCrit				= 0;
		public float	_decayCrit				= 0;

		public bool 	_multiplyByTimeScale	= true;
		
		public string Serialize()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.Append(string.Format("ns={0};", _numberOfShakes.ToString()));
			sb.Append(string.Format("sa={0};", _shakeAmount.ToString()));
			sb.Append(string.Format("ra={0};", _rotationAmount.ToString()));
			sb.Append(string.Format("ds={0};", _distance.ToString()));
			sb.Append(string.Format("sp={0};", _speed.ToString()));
			sb.Append(string.Format("dc={0};", _decay.ToString()));
			
			sb.Append(string.Format("nsc={0};", _numberOfShakesCrit.ToString()));
			sb.Append(string.Format("sac={0};", _shakeAmountCrit.ToString()));
			sb.Append(string.Format("rac={0};", _rotationAmountCrit.ToString()));
			sb.Append(string.Format("dsc={0};", _distanceCrit.ToString()));
			sb.Append(string.Format("spc={0};", _speedCrit.ToString()));
			sb.Append(string.Format("dcc={0};", _decayCrit.ToString()));

			sb.Append(string.Format("mts={0};", _multiplyByTimeScale.ToString()));
			
			return sb.ToString();
		}
		
		public static CameraShakeEventProperties Deserialize(string propertyString)
		{
			Dictionary<string, string> parsed = MoveUtils.ParseEventProperties(propertyString);
			return Deserialize(parsed);
		}
		
		public static CameraShakeEventProperties Deserialize(Dictionary<string, string> parsedProperties)
		{
			CameraShakeEventProperties properties = new CameraShakeEventProperties();
					
			properties._numberOfShakes = MoveUtils.ParseIntParam(parsedProperties, "ns");
			properties._shakeAmount = MoveUtils.ParseVector3Param(parsedProperties, "sa", Vector3.zero);
			properties._rotationAmount = MoveUtils.ParseVector3Param(parsedProperties, "ra", Vector3.zero);
			properties._distance = MoveUtils.ParseFloatParam(parsedProperties, "ds");
			properties._speed = MoveUtils.ParseFloatParam(parsedProperties, "sp");
			properties._decay = MoveUtils.ParseFloatParam(parsedProperties, "dc");

			properties._numberOfShakesCrit = MoveUtils.ParseIntParam(parsedProperties, "nsc");
			properties._shakeAmountCrit = MoveUtils.ParseVector3Param(parsedProperties, "sac", Vector3.zero);
			properties._rotationAmountCrit = MoveUtils.ParseVector3Param(parsedProperties, "rac", Vector3.zero);
			properties._distanceCrit = MoveUtils.ParseFloatParam(parsedProperties, "dsc");
			properties._speedCrit = MoveUtils.ParseFloatParam(parsedProperties, "spc");
			properties._decayCrit = MoveUtils.ParseFloatParam(parsedProperties, "dcc");

			properties._multiplyByTimeScale = MoveUtils.ParseBoolParam(parsedProperties, "mts", true);
			
			return properties;
		}
	}

	[System.Serializable]
	public class ProjectileEventProperties
	{
		public GameObject _prefab = null;
        public GameObject _changeEffect = null;
        public bool _isTarget = true;
        public bool _isLookAtTarget = false;
        public bool _isOnly = false;

		public bool _autoVelocity;


        public float _flyTime = 0.0f;
        public float _fadeOutTime = 0.0f;
		public BodyPart	_spawnAttachment = BodyPart.Root;
		public string	_spawnAttachmentPath = string.Empty;
		public Vector3	_spawnOffset	= Vector3.zero;
		public Vector3 	_spawnAngles	= Vector3.zero;
		public bool		_worldSpace		= false;
		public float 	_initialVelocity = 0.0f;
		public bool 	_isBoomerang 	= false;
		public float 	_reattachmentVelocity = 0.0f;
		public float 	_reattachmentAngularVelocity = 0.0f;
		public BodyPart	_reattachment = BodyPart.Root;
		public string	_reattachmentPath = string.Empty;
		public Vector3	_reattachmentOffset	= Vector3.zero;
		public Vector3 	_reattachmentAngles	= Vector3.zero;
		public bool		_reattachmentIsWorldSpace		= false;
		//public string	_reattachmentPropName = string.Empty;

		public string Serialize()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(string.Format("pat={0};", _spawnAttachment));
			sb.Append(string.Format("pah={0};", _spawnAttachmentPath));
			sb.Append(string.Format("pso={0};", _spawnOffset));
			sb.Append(string.Format("psa={0};", _spawnAngles));
			sb.Append(string.Format("pws={0};", _worldSpace));
			sb.Append(string.Format("iv={0};", _initialVelocity.ToString()));
			sb.Append(string.Format("ib={0};", _isBoomerang.ToString()));
			sb.Append(string.Format("rv={0};", _reattachmentVelocity.ToString()));
			sb.Append(string.Format("rav={0};", _reattachmentAngularVelocity.ToString()));
			sb.Append(string.Format("rat={0};", _reattachment));
			sb.Append(string.Format("rah={0};", _reattachmentPath));
			sb.Append(string.Format("rao={0};", _reattachmentOffset));
			sb.Append(string.Format("raa={0};", _reattachmentAngles));
			sb.Append(string.Format("rws={0};", _reattachmentIsWorldSpace));
            sb.Append(string.Format("flt={0};", _flyTime));
            sb.Append(string.Format("fot={0};", _fadeOutTime));
			//sb.Append(string.Format("rpn={0};", _reattachmentPropName.ToString()));
			
			return sb.ToString();
		}
		
		public static ProjectileEventProperties Deserialize(string propertyString)
		{
			Dictionary<string, string> parsed = MoveUtils.ParseEventProperties(propertyString);
			return Deserialize(parsed);
		}
		
		public static ProjectileEventProperties Deserialize(Dictionary<string, string> parsedProperties)
		{
			ProjectileEventProperties properties = new ProjectileEventProperties();

			properties._spawnAttachment = MoveUtils.ParseEnumParam<BodyPart>(parsedProperties, "pat", BodyPart.Root);
			properties._spawnAttachmentPath = MoveUtils.ParseStringParam(parsedProperties, "pah");
			properties._spawnOffset = MoveUtils.ParseVector3Param(parsedProperties, "pso", Vector3.zero);
			properties._spawnAngles = MoveUtils.ParseVector3Param(parsedProperties, "psa", Vector3.zero);
			properties._worldSpace = MoveUtils.ParseBoolParam(parsedProperties, "pws"); 
			properties._initialVelocity = MoveUtils.ParseFloatParam(parsedProperties, "iv");
			properties._isBoomerang = MoveUtils.ParseBoolParam(parsedProperties, "ib");
			properties._reattachmentVelocity = MoveUtils.ParseFloatParam(parsedProperties, "rv");
			properties._reattachmentAngularVelocity = MoveUtils.ParseFloatParam(parsedProperties, "rav");
			properties._reattachment = MoveUtils.ParseEnumParam<BodyPart>(parsedProperties, "rat", BodyPart.Root);
			properties._reattachmentPath = MoveUtils.ParseStringParam(parsedProperties, "rah");
			properties._reattachmentOffset = MoveUtils.ParseVector3Param(parsedProperties, "rao", Vector3.zero);
			properties._reattachmentAngles = MoveUtils.ParseVector3Param(parsedProperties, "raa", Vector3.zero);
			properties._reattachmentIsWorldSpace = MoveUtils.ParseBoolParam(parsedProperties, "rws");
            properties._flyTime = MoveUtils.ParseFloatParam(parsedProperties, "flt");
            properties._fadeOutTime = MoveUtils.ParseFloatParam(parsedProperties, "fot");
            //properties._reattachmentPropName = MoveUtils.ParseStringParam(parsedProperties, "rpn");

            return properties;
		}

	
		public string PrefabName { get { return _prefab.name; } }
	}

	[System.Serializable]
	public class CameraSwingEventProperties
	{
		public float _degrees = 0.0f;
		public float _lerpAmount = 0.0f;

		public string Serialize()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.Append(string.Format("d={0};", _degrees.ToString()));
			sb.Append(string.Format("la={0};", _lerpAmount.ToString()));
			
			return sb.ToString();
		}
		
		public static CameraSwingEventProperties Deserialize(string propertyString)
		{
			Dictionary<string, string> parsed = MoveUtils.ParseEventProperties(propertyString);
			return Deserialize(parsed);
		}
		
		public static CameraSwingEventProperties Deserialize(Dictionary<string, string> parsedProperties)
		{
			CameraSwingEventProperties properties = new CameraSwingEventProperties();

			properties._degrees = MoveUtils.ParseFloatParam( parsedProperties, "d" );
			properties._lerpAmount = MoveUtils.ParseFloatParam( parsedProperties, "la" );

			return properties;
		}
	}

	[System.Serializable]
	public class CameraMotionEventProperties
	{
		//public AnimationCurve _lerpCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
		public int _lerpFrames = 30;
		public float _lerpDuration = 1.0f;
		public int _hangonFrames = 0;
		public float _hangonDuration = 0.0f;
		public CameraMotionTrigger _motionTrigger = CameraMotionTrigger.LocalPlayerOnly;
		public CameraMotionTarget _motionTarget = CameraMotionTarget.Attacker;
		//public bool _useLocalOffset = true;
		//public Vector3 _posOffset = Vector3.zero;
		//public Vector3 _targetAngle = new Vector3(30.0f, 320.0f, 0.0f);
		public bool _blendCurrentCamera = false;
		public CameraLerp.LerpSmoothing _blendLerpSmoothing = CameraLerp.LerpSmoothing.fastSlow;
		public AnimationCurve _blendLerpCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
		public CameraLerp.LerpSmoothing _blendPitchLerpSmoothing = CameraLerp.LerpSmoothing.fastSlow;
		public AnimationCurve _blendPitchLerpCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
		public CameraLerp.LerpSmoothing _blendYawLerpSmoothing = CameraLerp.LerpSmoothing.fastSlow;
		public AnimationCurve _blendYawLerpCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
		public float _blendDistanceOffset = 0.0f;
		public float _blendPitchOffset = 0.0f;
		public float _blendYawOffset = 0.0f;
		public float _blendHeightOffset = 0.0f;
        public bool _onlyLookAtTarget = false;

        public string _motionOptions = "[]";

		public string Serialize()
		{
			StringBuilder sb = new StringBuilder();

			//sb.Append(string.Format("lc={0};", MoveUtils.StringifyAnimationCurve(_lerpCurve)));
			sb.Append(string.Format("lf={0};", _lerpFrames.ToString()));
			sb.Append(string.Format("ld={0};", _lerpDuration.ToString()));
			sb.Append(string.Format("hf={0};", _hangonFrames.ToString()));
			sb.Append(string.Format("hd={0};", _hangonDuration.ToString()));
			sb.Append(string.Format("mg={0};", _motionTrigger.ToString()));
			sb.Append(string.Format("mt={0};", _motionTarget.ToString()));
			sb.Append(string.Format("bcc={0};", _blendCurrentCamera.ToString()));
			sb.Append(string.Format("bls={0};", _blendLerpSmoothing.ToString()));
			sb.Append(string.Format("blc={0};", MoveUtils.StringifyAnimationCurve(_blendLerpCurve)));
			sb.Append(string.Format("blps={0};", _blendPitchLerpSmoothing.ToString()));
			sb.Append(string.Format("blpc={0};", MoveUtils.StringifyAnimationCurve(_blendPitchLerpCurve)));
			sb.Append(string.Format("blys={0};", _blendYawLerpSmoothing.ToString()));
			sb.Append(string.Format("blyc={0};", MoveUtils.StringifyAnimationCurve(_blendYawLerpCurve)));
			sb.Append(string.Format("bdo={0};", _blendDistanceOffset.ToString()));
			sb.Append(string.Format("bpo={0};", _blendPitchOffset.ToString()));
			sb.Append(string.Format("byo={0};", _blendYawOffset.ToString()));
			sb.Append(string.Format("bho={0};", _blendHeightOffset.ToString()));
			//sb.Append(string.Format("ulo={0};", _useLocalOffset.ToString()));
			//sb.Append(string.Format("po={0};", _posOffset.ToString()));
			//sb.Append(string.Format("ta={0};", _targetAngle.ToString()));
			sb.Append(string.Format("mos={0};", _motionOptions));
            sb.Append(string.Format("olt={0};", _onlyLookAtTarget.ToString()));
			return sb.ToString();
		}

		public static CameraMotionEventProperties Deserialize(string propertyString)
		{
			Dictionary<string, string> parsed = MoveUtils.ParseEventProperties(propertyString);
			return Deserialize(parsed);
		}
		
		public static CameraMotionEventProperties Deserialize(Dictionary<string, string> parsedProperties)
		{
			CameraMotionEventProperties properties = new CameraMotionEventProperties();

			//properties._lerpCurve = MoveUtils.ParseAnimationCurveParam(parsedProperties, "lc", AnimationCurve.Linear(0.0f, 0.05f, 1.0f, 0.05f));
			properties._lerpFrames = MoveUtils.ParseIntParam(parsedProperties, "lf");
			properties._lerpDuration = MoveUtils.ParseFloatParam(parsedProperties, "ld");
			properties._hangonFrames = MoveUtils.ParseIntParam(parsedProperties, "hf");
			properties._hangonDuration = MoveUtils.ParseFloatParam(parsedProperties, "hd");
			properties._motionTrigger = MoveUtils.ParseEnumParam<CameraMotionTrigger>(parsedProperties, "mg", CameraMotionTrigger.LocalPlayerOnly);
			properties._motionTarget = MoveUtils.ParseEnumParam<CameraMotionTarget>(parsedProperties, "mt", CameraMotionTarget.Attacker);
			properties._blendCurrentCamera = MoveUtils.ParseBoolParam(parsedProperties, "bcc");
			properties._blendLerpSmoothing = MoveUtils.ParseEnumParam<CameraLerp.LerpSmoothing>(parsedProperties, "bls", CameraLerp.LerpSmoothing.fastSlow);
			properties._blendLerpCurve = MoveUtils.ParseAnimationCurveParam(parsedProperties, "blc", AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f));
			properties._blendPitchLerpSmoothing = MoveUtils.ParseEnumParam<CameraLerp.LerpSmoothing>(parsedProperties, "blps", CameraLerp.LerpSmoothing.fastSlow);
			properties._blendPitchLerpCurve = MoveUtils.ParseAnimationCurveParam(parsedProperties, "blpc", AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f));
			properties._blendYawLerpSmoothing = MoveUtils.ParseEnumParam<CameraLerp.LerpSmoothing>(parsedProperties, "blys", CameraLerp.LerpSmoothing.fastSlow);
			properties._blendYawLerpCurve = MoveUtils.ParseAnimationCurveParam(parsedProperties, "blyc", AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f));
			properties._blendDistanceOffset = MoveUtils.ParseFloatParam(parsedProperties, "bdo");
			properties._blendPitchOffset = MoveUtils.ParseFloatParam(parsedProperties, "bpo");
			properties._blendYawOffset = MoveUtils.ParseFloatParam(parsedProperties, "byo");
			properties._blendHeightOffset = MoveUtils.ParseFloatParam(parsedProperties, "bho");
			//properties._useLocalOffset = MoveUtils.ParseBoolParam(parsedProperties, "ulo", true);
			//properties._posOffset = MoveUtils.ParseVector3Param(parsedProperties, "po", Vector3.zero);
			//properties._targetAngle = MoveUtils.ParseVector3Param(parsedProperties, "ta", Vector3.zero);
			properties._motionOptions = MoveUtils.ParseStringParam(parsedProperties, "mos", string.Empty);
            properties._onlyLookAtTarget = MoveUtils.ParseBoolParam(parsedProperties, "olt");
			return properties;
		}
	}
	
	[System.Serializable]
	public class AudioEventProperties
	{
		public string _event	= null;
		public bool   _applyOnTargetList = false;
		public bool   _isPersistent = false;
		public bool   _stopOnOverride = false;
		public bool   _stopOnExitAnim = false;
		public bool   _stopOnEndTurn = false;
		public bool   _stopAfterTurns = false;
		public int    _turns = 0;
		public bool   _stopOnDuration = false;
		public float  _duration = 0.0f;
		public bool   _interruptable = false;
		public bool   _cancelIfMissed = false;
		public bool   _cancelIfBlocked = false;
		public bool   _cancelIfHit = false;
	    public float  _volumn = 0;
		public string Serialize()
		{
			// MaxG: Added some parameters
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format("ev={0};", _event));
			sb.Append(string.Format("ip={0};", _isPersistent.ToString()));
			sb.Append(string.Format("soo={0};", _stopOnExitAnim.ToString()));
			sb.Append(string.Format("se={0};", _stopOnExitAnim.ToString()));
			sb.Append(string.Format("aotl={0};", _applyOnTargetList.ToString()));
			sb.Append(string.Format("soet={0};", _stopOnEndTurn.ToString()));
			sb.Append(string.Format("sats={0};", _stopAfterTurns.ToString()));
			sb.Append(string.Format("turns={0};", _turns.ToString()));
			sb.Append(string.Format("sod={0};", _stopOnDuration.ToString()));
			sb.Append(string.Format("dura={0};", _duration.ToString()));
			sb.Append(string.Format("int={0};", _interruptable.ToString()));
			sb.Append(string.Format("cim={0};", _cancelIfMissed.ToString()));
			sb.Append(string.Format("cib={0};", _cancelIfBlocked.ToString()));
			sb.Append(string.Format("cih={0};", _cancelIfHit.ToString()));
			return sb.ToString();
		}
		
		public static AudioEventProperties Deserialize(string propertyString)
		{
			Dictionary<string, string> parsed = MoveUtils.ParseEventProperties(propertyString);
			return Deserialize(parsed);
		}
		
		public static AudioEventProperties Deserialize(Dictionary<string, string> parsedProperties)
		{
			// MaxG: Added some parameters
			AudioEventProperties properties = new AudioEventProperties();
			properties._event = MoveUtils.ParseStringParam(parsedProperties, "ev");
			properties._applyOnTargetList = MoveUtils.ParseBoolParam(parsedProperties, "aotl");
			properties._isPersistent = MoveUtils.ParseBoolParam(parsedProperties, "ip");
			properties._stopOnOverride = MoveUtils.ParseBoolParam(parsedProperties, "soo");
			properties._stopOnExitAnim = MoveUtils.ParseBoolParam(parsedProperties, "se");            
			properties._stopOnEndTurn = MoveUtils.ParseBoolParam(parsedProperties, "soet");
			properties._stopAfterTurns = MoveUtils.ParseBoolParam(parsedProperties, "sats");
			properties._turns = MoveUtils.ParseIntParam(parsedProperties, "turns");
			properties._stopOnDuration = MoveUtils.ParseBoolParam(parsedProperties, "sod");
			properties._duration = MoveUtils.ParseFloatParam(parsedProperties, "dura");
			properties._interruptable = MoveUtils.ParseBoolParam(parsedProperties, "int");
			properties._cancelIfBlocked = MoveUtils.ParseBoolParam(parsedProperties, "cib");
			properties._cancelIfMissed = MoveUtils.ParseBoolParam(parsedProperties, "cim");
			properties._cancelIfHit = MoveUtils.ParseBoolParam(parsedProperties, "cih");
			return properties;
		}
	}

	[System.Serializable]
	public class EnvironmentLightingProperties
	{
		public Gradient	_multiplyGradient	= new Gradient();
		public float	_multiplyDuration	= 0.4f;
		public Gradient	_addGradient		= new Gradient();
		public float	_addDuration		= 0.4f;
		public float	_speed				= 1.0f;

		public EnvironmentLightingProperties()
		{
#if UNITY_EDITOR
			GradientColorKey[] colorKeys = new GradientColorKey[2];

			for (int i = 0; i < colorKeys.Length; i++)
			{
				colorKeys[i] = new GradientColorKey();
				colorKeys[i].color = Color.gray;
				colorKeys[i].time = (float)i;
			}
			_multiplyGradient.colorKeys = colorKeys;

			for (int i = 0; i < colorKeys.Length; i++)
			{
				colorKeys[i] = new GradientColorKey();
				colorKeys[i].color = Color.black;
				colorKeys[i].time = (float)i;
			}
			_addGradient.colorKeys = colorKeys;
#endif
		}

		public string Serialize()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(string.Format("elmg={0};", MoveUtils.StringifyGradientColorKeys(_multiplyGradient)));
			sb.Append(string.Format("elmd={0};", _multiplyDuration.ToString()));
			sb.Append(string.Format("elag={0};", MoveUtils.StringifyGradientColorKeys(_addGradient)));
			sb.Append(string.Format("elad={0};", _addDuration.ToString()));
			sb.Append(string.Format("els={0};", _speed.ToString()));

			return sb.ToString();
		}

		public static EnvironmentLightingProperties Deserialize(string propertyString)
		{
			Dictionary<string, string> parsed = MoveUtils.ParseEventProperties(propertyString);
			return Deserialize(parsed);
		}

		public static EnvironmentLightingProperties Deserialize(Dictionary<string, string> parsedProperties)
		{
			EnvironmentLightingProperties properties = new EnvironmentLightingProperties();
		
			properties._multiplyGradient	= MoveUtils.ParseGradientParam(parsedProperties, "elmg", null);
			properties._multiplyDuration	= MoveUtils.ParseFloatParam(parsedProperties, "elmd", 0);
			properties._addGradient			= MoveUtils.ParseGradientParam(parsedProperties, "elag", null);
			properties._addDuration			= MoveUtils.ParseFloatParam(parsedProperties, "elad", 0);
			properties._speed				= MoveUtils.ParseFloatParam(parsedProperties, "els", 1.0f);

			return properties;
		}
	}

#endregion
	public class MoveAnimationEvent
	{
	
		public AnimationEvent mAniEvent;


		public MoveAnimationEvent()
		{
			mAniEvent = new AnimationEvent ();
		}


		public  string data
		{

			get
			{
				return mAniEvent.stringParameter;
			}

			set
			{
				mAniEvent.stringParameter = value;

			}
		}
		
		public  float floatParameter
		{

			get
			{
				return mAniEvent.floatParameter;
			}

			set
			{
				mAniEvent.floatParameter = value;
			}
		}
		
		public  string functionName
		{

			get
			{
				return mAniEvent.functionName;
			}

			set
			{
				mAniEvent.functionName = value;

			}
		}
		
		public  int intParameter
		{

			get
			{
				return mAniEvent.intParameter;
			}

			set
			{
				mAniEvent.intParameter = value;
			}
		}
		
		public  SendMessageOptions messageOptions
		{

			get
			{
				return mAniEvent.messageOptions;
			}
		
			set
			{
				mAniEvent.messageOptions = value;
			}
		}
		
		public  Object objectReferenceParameter
		{
		
			get
			{
				return mAniEvent.objectReferenceParameter;
			}

			set
			{
				mAniEvent.objectReferenceParameter = value;
			}
		}

		public  EventInfo EventRef
		{
			
			get;
			
			set;
		}

		public  string stringParameter
		{

			get
			{
				return mAniEvent.stringParameter;
			}

			set
			{
				mAniEvent.stringParameter = value;
			}
		}
		
		public  float time
		{

			get
			{
				return mAniEvent.time;
			}

			set
			{
				mAniEvent.time = value;
			}
		}
		

	}
#region Event Info Class Definitions
	
	[System.Serializable]
	public abstract class EventInfo
	{
		public float _frame;
		public bool _required = false;
		public bool _playOnce = false;
		public bool _persistent = false;

		protected MoveAnimationEvent e = null;
		
		public virtual MoveEvent ToMoveEvent(float numFrames, float endFrame)
		{
			return new MoveEvent(ToAnimationEvent(numFrames), _required, _playOnce, _persistent, endFrame);
		}
		
		public virtual MoveAnimationEvent ToAnimationEvent(float numFrames)
		{
			if(e==null)
			{
				float frame = Mathf.Clamp(_frame, 0, numFrames);
				
				// TJ: round time to the thousandth place
				float time = frame / numFrames;
				time *= 1000.0f;
				time = Mathf.Floor(time);
				time /= 1000;
				
				time = Mathf.Max(time, 0.001f);
				
				e 	= new MoveAnimationEvent();
				e.time 				= time;
				e.functionName		= GetFunctionName();
				e.messageOptions	= SendMessageOptions.DontRequireReceiver;

				e.EventRef = this;
				//e.o = this;
			}

			return e;
		}
		
		protected abstract string GetFunctionName();
	}
	
	[System.Serializable]
	public class CombatEventInfo : EventInfo
	{
		public enum CombatEventType
		{
			// DO NOT REORDER THIS ENUM
			// THESE ARE EMBEDDED AS INTEGERS
			ToTargetStart,
			ToTargetEnd,
			ReturnStart,
			ReturnEnd,
			ReturnJumpStart,
			PlayTargetHitReaction,//need
			ToNextRotation,
			IgniteEmbeddedProjectile,
			PostJumpWalkBackStart,
			ReturnJumpEnd,
			Jump1Start,
			Jump2Start,
			JumpToTargetStart,
			TargetBeenStonedReaction,
			RaiseCombatHitCombatantEvent,
			TimeScale,
			AnimationOver,//need
			TriggerFrame,//need
			Pause,//need

			// mark frame
			FlowUpStart,
			FlowDownStart,
			FlowLandStart,
			BackStart,
			BackCollideStart,
			BackReturnStart,
			DownLieStart,
			DownStandUpStart
			// ADD NEW TYPES HERE
		};
		
		//string functionName = "none";

		public CombatEventType eventType = CombatEventType.ToTargetStart;
		
		public CombatEventProperties combatEventProps = new CombatEventProperties();
		public PlayHitReactionProperties hitRxnProps = new PlayHitReactionProperties();
		public BuffEventProperties buffEventProps = new BuffEventProperties();
		public TimeScaleProperties timeScaleProps = new TimeScaleProperties();
		public PauseProperties pauseProps = new PauseProperties();

		public CombatEventInfo()
		{
			combatEventProps = new CombatEventProperties();
			hitRxnProps = new PlayHitReactionProperties();
			buffEventProps = new BuffEventProperties();
			timeScaleProps = new TimeScaleProperties();
			pauseProps = new PauseProperties();
		}
		public CombatEventInfo(string propertyString)
		{
			Dictionary<string, string> parsedProperties = MoveUtils.ParseEventProperties(propertyString);
			
			combatEventProps = CombatEventProperties.Deserialize(parsedProperties);
			hitRxnProps = PlayHitReactionProperties.Deserialize(parsedProperties);
			buffEventProps = BuffEventProperties.Deserialize(parsedProperties);
			timeScaleProps = TimeScaleProperties.Deserialize(parsedProperties);
			pauseProps = PauseProperties.Deserialize(parsedProperties);
		}

		public bool IsTriggerFrame()
		{
			switch(eventType)
			{
			case CombatEventType.ToTargetStart:
			case CombatEventType.ToTargetEnd:
			case CombatEventType.ReturnStart:
			case CombatEventType.ReturnEnd:
				return true;
			}
			return false;
		}
		public bool ShouldSetAnimationEvent()
		{
			return GetFunctionName() != "none";
		}
		
		private StringBuilder _sb_ToAnimationEvent = new StringBuilder(256);
		
		public override MoveAnimationEvent ToAnimationEvent (float numFrames)
		{
			if (e != null) 
			{
				return e;
			}

			base.ToAnimationEvent(numFrames);
			e.functionName = GetFunctionName();
			
			_sb_ToAnimationEvent.Clear();
			_sb_ToAnimationEvent.Append(combatEventProps.Serialize());
			_sb_ToAnimationEvent.Append(hitRxnProps.Serialize());
			_sb_ToAnimationEvent.Append(buffEventProps.Serialize());
			_sb_ToAnimationEvent.Append(timeScaleProps.Serialize());
			_sb_ToAnimationEvent.Append(pauseProps.Serialize());
			
			e.stringParameter = _sb_ToAnimationEvent.ToString();
		

			return e;
		}

		protected override string GetFunctionName()
		{
			switch(eventType)
			{
				case CombatEventType.PlayTargetHitReaction:
					return "PlayTargetHitReaction";
				case CombatEventType.AnimationOver:
					return "OnAnimationOver";
				case CombatEventType.TriggerFrame:
					return "OnTriggerFrame";
				case CombatEventType.Pause:
					return "OnPauseEvent";
				case CombatEventType.ReturnStart:
					return "ComputeAttackExitTime";
				case CombatEventType.Jump2Start:
					return "TryBeginReturn";
				case CombatEventType.Jump1Start:
					return "TryBeginReturn";
				case CombatEventType.ToNextRotation:
					return "ToNextRotation";
				case CombatEventType.IgniteEmbeddedProjectile:
					return "IgniteEmbeddedProjectile";
				case CombatEventType.PostJumpWalkBackStart:
					return "TryBeginWalkBack";
				case CombatEventType.JumpToTargetStart:
					return "TryJumpToTarget";
				case CombatEventType.TargetBeenStonedReaction:
					return "TargetBeenStonedReaction";
				case CombatEventType.RaiseCombatHitCombatantEvent:
					return "RaiseCombatHitCombatantEvent";
				case CombatEventType.TimeScale:
					return "OnCombatTimeScale";
			}
			return "none";
		}
	}
	public class GenericEventInfo : EventInfo
	{
		public string				_functionName		= string.Empty;
		public float				_floatParameter		= 0;
		public int					_intParameter		= 0;
		public string				_stringParameter	= string.Empty;
		public Object				_objectParameter	= null;
		public SendMessageOptions	_messageOptions		= SendMessageOptions.DontRequireReceiver;
		
		public override MoveAnimationEvent ToAnimationEvent (float numFrames)
		{
			if(e!=null)
			{
				return e;
			}
			base.ToAnimationEvent(numFrames);
			
			e.floatParameter 			= _floatParameter;
			e.intParameter				= _intParameter;
			e.stringParameter			= _stringParameter;
			e.objectReferenceParameter	= _objectParameter;
			e.messageOptions			= _messageOptions;
			
			return e;
		}
		
		protected override string GetFunctionName()
		{
			return _functionName;
		}
	}

	[System.Serializable]
	public class MoveEventInfo : EventInfo
	{
		public MoveEventProperties _moveEventProperties;

		public MoveEventInfo()
		{
			_moveEventProperties = new MoveEventProperties();
		}

		public MoveEventInfo(string propertyString)
		{
			Dictionary<string, string> parsedProperties = MoveUtils.ParseEventProperties(propertyString);
			
			_moveEventProperties 	= MoveEventProperties.Deserialize(parsedProperties);
		}

		public override MoveAnimationEvent ToAnimationEvent(float numFrames)
		{
			if(e!=null)
			{
				return e;
			}
			base.ToAnimationEvent(numFrames);
			
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(_moveEventProperties.Serialize());
			
			e.stringParameter = sb.ToString();
			
			return e;
		}
		
		protected override string GetFunctionName()
		{
			return "OnMoveEvent";
		}
	}
	
	[System.Serializable]
	public class HitEventInfo : EventInfo
	{
		//We don't need damage properties from marvel! only particle effect & audio properties are needed in GM.
		public DamageEventProperties _damageProperties;
		public ParticleEventProperties _particleProperties;
		public AudioEventProperties	_audioProperties;
		public PlayHitReactionProperties _hitRxnProps = new PlayHitReactionProperties();
		
		public HitEventInfo()
		{
			//_damageProperties 	= new DamageEventProperties();
			_particleProperties = new ParticleEventProperties();
			_audioProperties	= new AudioEventProperties();

			_particleProperties._applyOnTargetList = true;
			_audioProperties._applyOnTargetList = true;
		}
		
		public HitEventInfo(string propertyString)
		{
			Dictionary<string, string> parsedProperties = MoveUtils.ParseEventProperties(propertyString);
			
			//_damageProperties 	= DamageEventProperties.Deserialize(parsedProperties);
			_particleProperties = ParticleEventProperties.Deserialize(parsedProperties);
			_audioProperties 	= AudioEventProperties.Deserialize(parsedProperties);
			_hitRxnProps        = PlayHitReactionProperties.Deserialize(parsedProperties);
		}
		
		public override MoveAnimationEvent ToAnimationEvent(float numFrames)
		{
			if(e!=null)
			{
				return e;
			}
			base.ToAnimationEvent(numFrames);
			
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			//sb.Append(_damageProperties.Serialize());
			sb.Append(_particleProperties.Serialize());
			sb.Append(_audioProperties.Serialize());
			sb.Append(_hitRxnProps.Serialize());
			
			e.stringParameter = sb.ToString();
			
			return e;
		}
		
		protected override string GetFunctionName()
		{
			//return "OnInflictDamage";
			return "OnInflictHit";
		}
	}

	[System.Serializable]
	public class ProjectileEventInfo : EventInfo
	{
		public ProjectileEventProperties _projectileProperties;
		//public HitEventInfo _hitEvent;
		
		public ProjectileEventInfo()
		{
			_projectileProperties = new ProjectileEventProperties();
			//_hitEvent = new HitEventInfo();
		}
		
		public ProjectileEventInfo(string propertyString)
		{
			Dictionary<string, string> parsedProperties = MoveUtils.ParseEventProperties(propertyString);

			_projectileProperties = ProjectileEventProperties.Deserialize(parsedProperties);
			//_hitEvent = new HitEventInfo();
			//_hitEvent._damageProperties = DamageEventProperties.Deserialize(parsedProperties);
			//_hitEvent._particleProperties = ParticleEventProperties.Deserialize(parsedProperties);
			//_hitEvent._audioProperties 	= AudioEventProperties.Deserialize(parsedProperties);
		}
		
		public override MoveAnimationEvent ToAnimationEvent(float numFrames)
		{
			if(e!=null)
			{
				return e;
			}
			base.ToAnimationEvent(numFrames);
			
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(_projectileProperties.Serialize());
			//sb.Append(_hitEvent._damageProperties.Serialize());
			//sb.Append(_hitEvent._particleProperties.Serialize());
			//sb.Append(_hitEvent._audioProperties.Serialize());
			
			e.stringParameter = sb.ToString();
			
			return e;
		}
		
		protected override string GetFunctionName()
		{
			return "OnSpawnProjectile";
		}
	}
	
	[System.Serializable]
	public class ParticleEventInfo : EventInfo
	{
		public ParticleEventProperties _particleProperties;
		
		public ParticleEventInfo()
		{
			_particleProperties = new ParticleEventProperties();
		}
		
		public ParticleEventInfo(string propertyString)
		{
			_particleProperties = ParticleEventProperties.Deserialize(propertyString);
		}
		
		public override MoveAnimationEvent ToAnimationEvent(float numFrames)
		{
			if(e!=null)
			{
				return e;
			}
			base.ToAnimationEvent(numFrames);
			e.stringParameter = _particleProperties.Serialize();
			return e;
		}
		
		protected override string GetFunctionName()
		{
			return "OnPlayParticle";
		}
	}
	
	[System.Serializable]
	public class TrailRendererEventInfo : EventInfo
	{
		public TrailRendererEventProperties _trailRendererProperties;
		public bool _isEditCrit = false;
		
		public TrailRendererEventInfo()
		{
			_trailRendererProperties = new TrailRendererEventProperties();
		}
		
		public TrailRendererEventInfo(MoveAnimationEvent e)
		{
			_trailRendererProperties = TrailRendererEventProperties.Deserialize(e.stringParameter);
			_trailRendererProperties._trailRenderer = (GameObject)e.objectReferenceParameter;
		}
		
		public override MoveAnimationEvent ToAnimationEvent (float numFrames)
		{
			if(e!=null)
			{
				return e;
			}
			base.ToAnimationEvent (numFrames);
			e.stringParameter = _trailRendererProperties.Serialize();
			e.objectReferenceParameter = _trailRendererProperties._trailRenderer;
			return e;
		}
		
		protected override string GetFunctionName()
		{
			return "OnPlayTrailRenderer";
		}
	}

	[System.Serializable]
	public class DynamicLightEventInfo : EventInfo
	{
		public DynamicLightEventProperties _dynamicLightProperties;
		
		public DynamicLightEventInfo()
		{
			_dynamicLightProperties = new DynamicLightEventProperties();
		}
		
		public DynamicLightEventInfo(MoveAnimationEvent e)
		{
			_dynamicLightProperties = DynamicLightEventProperties.Deserialize(e.stringParameter);
			_dynamicLightProperties._dynamicLight = (GameObject)e.objectReferenceParameter;
		}
		
		public override MoveAnimationEvent ToAnimationEvent (float numFrames)
		{
			if(e!=null)
			{
				return e;
			}
			base.ToAnimationEvent (numFrames);
			e.stringParameter = _dynamicLightProperties.Serialize();
			e.objectReferenceParameter = _dynamicLightProperties._dynamicLight;
			return e;
		}
		
		protected override string GetFunctionName()
		{
			return "OnPlayDynamicLight";
		}
	}

	[System.Serializable]
	public class CameraShakeEventInfo : EventInfo
	{
		public CameraShakeEventProperties _cameraShakeProperties;
		
		public CameraShakeEventInfo()
		{
			_cameraShakeProperties = new CameraShakeEventProperties();
		}
		
		public CameraShakeEventInfo(string propertyString)
		{
			_cameraShakeProperties = CameraShakeEventProperties.Deserialize(propertyString);
		}
		
		public override MoveAnimationEvent ToAnimationEvent (float numFrames)
		{
			if(e!=null)
			{
				return e;
			}
			base.ToAnimationEvent (numFrames);
			e.stringParameter = _cameraShakeProperties.Serialize();
			return e;
		}
		
		protected override string GetFunctionName ()
		{
			return "OnPlayCameraShake";
		}
	}

	[System.Serializable]
	public class CameraSwingEventInfo : EventInfo
	{
		public CameraSwingEventProperties _cameraSwingProperties;
		
		public CameraSwingEventInfo()
		{
			_cameraSwingProperties = new CameraSwingEventProperties();
		}
		
		public CameraSwingEventInfo(string propertyString)
		{
			_cameraSwingProperties = CameraSwingEventProperties.Deserialize(propertyString);
		}
		
		public override MoveAnimationEvent ToAnimationEvent (float numFrames)
		{
			if(e!=null)
			{
				return e;
			}
			base.ToAnimationEvent (numFrames);
			e.stringParameter = _cameraSwingProperties.Serialize();
			return e;
		}
		
		protected override string GetFunctionName ()
		{
			return "OnSwingCamera";
		}
	}

	[System.Serializable]
	public class CameraMotionEventInfo : EventInfo
	{
		public CameraMotionEventProperties _cameraMotionProperties;

		public CameraMotionEventInfo()
		{
			_cameraMotionProperties = new CameraMotionEventProperties();
			_required = true;
			_playOnce = true;
		}

		public CameraMotionEventInfo(string propertyString)
		{
			_cameraMotionProperties = CameraMotionEventProperties.Deserialize(propertyString);
		}

		public override MoveAnimationEvent ToAnimationEvent(float numFrames)
		{
			if(e!=null)
			{
				return e;
			}
			base.ToAnimationEvent(numFrames);
			e.stringParameter = _cameraMotionProperties.Serialize();
			return e;
		}

		protected override string GetFunctionName ()
		{
			return "OnCameraMotion";
		}
	}

	[System.Serializable]
	public class AudioEventInfo : EventInfo
	{
		public AudioEventProperties _audioProperties;

		public AudioEventInfo()
		{
			_audioProperties = new AudioEventProperties();
		}

		public AudioEventInfo(string propertyString)
		{
			_audioProperties = AudioEventProperties.Deserialize(propertyString);
		}

		public override MoveAnimationEvent ToAnimationEvent(float numFrames)
		{
			if(e!=null)
			{
				return e;
			}
			base.ToAnimationEvent(numFrames);
			e.stringParameter = _audioProperties.Serialize();
			return e;
		}
		
		protected override string GetFunctionName()
		{
			return "OnPlayAudio";
		}
	}

	[System.Serializable]
	public class EnvironmentLightingEventInfo : EventInfo
	{
		public EnvironmentLightingProperties _lightingProperties;

		public EnvironmentLightingEventInfo()
		{
			_lightingProperties = new EnvironmentLightingProperties();
		}

		public EnvironmentLightingEventInfo(string propertyString)
		{
			_lightingProperties = EnvironmentLightingProperties.Deserialize(propertyString);
		}

		public void SetSpeed(float speed)
		{
			_lightingProperties._speed = speed;
		}

		public override MoveAnimationEvent ToAnimationEvent(float numFrames)
		{
			if(e!=null)
			{
				return e;
			}
			base.ToAnimationEvent(numFrames);
			e.stringParameter = _lightingProperties.Serialize();
			return e;
		}

		protected override string GetFunctionName ()
		{
			return "OnModifyEnvironmentLighting";
		}
	}

	[System.Serializable]
	public class PostFxEventInfo : EventInfo
	{
		public AnimationCurve 	_animationCurve = new AnimationCurve(new Keyframe[]{ new Keyframe(0, 0), new Keyframe(1, 0) });
		public float			_speed 			= 1.0f;

		public PostFxEventInfo() 
		{
		}

		public PostFxEventInfo(string propertyString) 
		{
			Deserialize(MoveUtils.ParseEventProperties(propertyString));
		}

		public bool Update(float deltaTime)
		{
			_elapsed += deltaTime * _speed;
			Sim(_elapsed);
			return _elapsed >= Duration;
		}

		public void Sim(float elapsedTime)
		{
			float t = Mathf.Clamp01(_animationCurve.Evaluate(elapsedTime));
			OnUpdate(t);
		}

		public override MoveAnimationEvent ToAnimationEvent (float numFrames)
		{
			if(e!=null)
			{
				return e;
			}
			base.ToAnimationEvent(numFrames);
			e.stringParameter = Serialize();
			return e;
		}
		
		protected override string GetFunctionName()
		{
			return "OnPostFxEvent";
		}

		protected virtual string Serialize() 
		{ 
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format("ac={0};", MoveUtils.StringifyAnimationCurve(_animationCurve)));
			sb.Append(string.Format("sp={0};", _speed.ToString()));
			return sb.ToString();
		}

		protected virtual void Deserialize(Dictionary<string, string> parsedProperties)
		{
			_animationCurve = MoveUtils.ParseAnimationCurveParam(parsedProperties, "ac", new AnimationCurve());
			_speed			= MoveUtils.ParseFloatParam(parsedProperties, "sp", 1.0f);
		}

		protected virtual void OnUpdate(float t) 
		{
		}

		public float Duration
		{
			get
			{
				if (_animationCurve.keys.Length > 1)
				{
					return _animationCurve.keys[_animationCurve.keys.Length - 1].time;
				}
				else
				{
					return 0;
				}
			}
		}

		private float _elapsed = 0.0f;
	}

	[System.Serializable]
	public class PostBloomEventInfo : PostFxEventInfo
	{
		public Color	_color		= Color.white;
		public float	_mix		= 0.0f;
		public float	_intensity	= 1;
		public float	_ramp		= 2.0f;
		public float 	_blur		= 2.0f;

		public PostBloomEventInfo()
		{
		}

		public PostBloomEventInfo(string propertyString) : base(propertyString)
		{
		}

		protected override string GetFunctionName()
		{
			return "OnPostFxBloomEvent";
		}

		protected override void OnUpdate(float t)
		{
			Color color		= Color.Lerp(Color.gray, _color, t);
			float ramp		= Mathf.Lerp(2.0f, _ramp, t);
			float blur		= Mathf.Lerp(2.0f, _blur, t);
			float mix		= Mathf.Lerp(0.0f, _mix, t);
			float intensity = Mathf.Lerp(1.0f, _intensity, t);

			RenderGlobals.SetBloom(mix, blur, ramp, intensity, color);
		}

		protected override string Serialize()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.Serialize());
			sb.Append(string.Format("bpc={0};", 	_color.ToString()));
			sb.Append(string.Format("bpr={0};", 	_ramp.ToString()));
			sb.Append(string.Format("bpbl={0};",	_blur.ToString()));
			sb.Append(string.Format("bpm={0};",	_mix.ToString()));
			sb.Append(string.Format("bpi={0};", 	_intensity.ToString()));
			return sb.ToString();
		}

		protected override void Deserialize (Dictionary<string, string> parsedProperties)
		{
			base.Deserialize(parsedProperties);

			_color 		= MoveUtils.ParseColorParam(parsedProperties, "bpc", Color.white);
			_ramp 		= MoveUtils.ParseFloatParam(parsedProperties, "bpr", 2.0f);
			_blur 		= MoveUtils.ParseFloatParam(parsedProperties, "bpbl", 2.0f);
			_mix		= MoveUtils.ParseFloatParam(parsedProperties, "bpm", 0.0f);
			_intensity	= MoveUtils.ParseFloatParam(parsedProperties, "bpi", 1.0f);
		}
	}

	[System.Serializable]
	public class PostVignetteEventInfo : PostFxEventInfo
	{
		public float _mix		= 0.0f;
		public float _intensity = 0.3f;

		public PostVignetteEventInfo()
		{
		}
		
		public PostVignetteEventInfo(string propertyString) : base(propertyString)
		{
		}

		protected override string GetFunctionName()
		{
			return "OnPostFxVignetteEvent";
		}

		protected override void OnUpdate(float t)
		{
			float intensity = Mathf.Lerp(1.0f, _intensity, t);
			float mix		= Mathf.Lerp(0.0f, _mix, t);

			RenderGlobals.SetVignette(mix, intensity, Color.white);
		}
		
		protected override string Serialize()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.Serialize());
			sb.Append(string.Format("vpi={0};", _intensity.ToString()));
			sb.Append(string.Format("vpm={0};", _mix.ToString()));
			return sb.ToString();
		}

		protected override void Deserialize (Dictionary<string, string> parsedProperties)
		{
			base.Deserialize(parsedProperties);

			_intensity 	= MoveUtils.ParseFloatParam(parsedProperties, "vpi", 0.3f);
			_mix		= MoveUtils.ParseFloatParam(parsedProperties, "vpm", 0.0f);
		}
	}

	[System.Serializable]
	public class PostWarpEventInfo : PostFxEventInfo
	{
		public float	_mix		= 0.0f;
		public Vector2 	_intensity	= new Vector2(0.05f, 0.05f);

		public PostWarpEventInfo()
		{
		}
		
		public PostWarpEventInfo(string propertyString) : base(propertyString)
		{
		}

		protected override string GetFunctionName()
		{
			return "OnPostFxWarpEvent";
		}

		protected override void OnUpdate(float t)
		{
			Vector2	intensity 	= Vector2.Lerp(new Vector2(0.05f, 0.05f), _intensity, t);
			float 	mix			= Mathf.Lerp(0.0f, _mix, t);

			RenderGlobals.SetWarp(mix, intensity);
		}
		
		protected override string Serialize()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.Serialize());
			sb.Append(string.Format("wpi={0};", _intensity.ToString()));
			sb.Append(string.Format("wpm={0};", _mix.ToString()));
			return sb.ToString();
		}

		protected override void Deserialize (Dictionary<string, string> parsedProperties)
		{
			base.Deserialize(parsedProperties);
			
			_intensity 	= MoveUtils.ParseVector2Param(parsedProperties, "wpi", new Vector2(0.05f, 0.05f));
			_mix		= MoveUtils.ParseFloatParam(parsedProperties, "wpm", 0.0f);
		}
	}

	[System.Serializable]
	public class AnimationSpeedEventInfo: GenericEventInfo
	{
		public bool _slideToOpponent = false;
		public AnimationSpeedEventProperties asep = new AnimationSpeedEventProperties();

		public AnimationSpeedEventInfo()
		{
			_floatParameter = 1.0f;
			_required = true;
			_playOnce = true;
		}
		public AnimationSpeedEventInfo(string propertyString)
		{
			Dictionary<string, string> parsedProperties = MoveUtils.ParseEventProperties(propertyString);
			asep = AnimationSpeedEventProperties.Deserialize(parsedProperties);
		}
		

		public override MoveAnimationEvent ToAnimationEvent(float numFrames)
		{
			// hack to update when building
			_required = true;
			_playOnce = true;

			if(e!=null)
			{
				return e;
			}
			base.ToAnimationEvent(numFrames);
			e.intParameter = _slideToOpponent ? 1 : 0;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(asep.Serialize());
			e.stringParameter = sb.ToString();
			return e;
		}

		protected override string GetFunctionName()
		{
			return "OnSetAnimationSpeed";
		}
	}

	[System.Serializable]
	public class SwitchRenderSettingsEventInfo : PostFxEventInfo
	{
		public string _name = "";
		
		public SwitchRenderSettingsEventInfo()
		{
		}
		
		public SwitchRenderSettingsEventInfo(string propertyString) : base(propertyString)
		{
		}
		
		protected override string GetFunctionName()
		{
			return "OnSwitchRenderSettings";
		}
		
		protected override string Serialize()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.Serialize());
			sb.Append(string.Format("vpi={0};", _name));
			return sb.ToString();
		}
		
		protected override void Deserialize (Dictionary<string, string> parsedProperties)
		{
			base.Deserialize(parsedProperties);
			_name = MoveUtils.ParseStringParam (parsedProperties, "vpi");
		}
	}

#endregion
	
	public class Move : MonoBehaviour 
	{
		// DONT CHANGE THE ORDER
		public enum eSkillActionPositionType
		{
			INVALID = -1,
			ORIGINAL = 0,
			MIDPOINT = 1,
			TARGET = 2,
			MIDLINE = 3,
			TRANSFORM = 4,
		}

		// DONT CHANGE THE ORDER
		public enum eSkillTargetPositionType
		{
			NONE = 0,
			SELECT_TARGET_OR_FIRST_TARGET = 1,
			FIRST_TARGET = 2,
			TARGETS = 3,
			TRANSFORM = 4,
		}

#if UNITY_EDITOR
		public string                               _path                       = "";
		public string                               _motionTransform            = "Bip001";
		[ResourcesReference(typeof(GameObject))]
		public ResourcesReference                   _animationObject           = new ResourcesReference();
		[HideInInspector]
		public Transform _follow = null;
#endif

		public MoveController.CombatantMoveState    _moveState                  = MoveController.CombatantMoveState.kIdle;
		public AnimationClip 						_animationClip				= null;
		public AnimationSpeedEventInfo				_animationSpeedEvent		= null;
		public MoveEventInfo						_moveEvent					= null;
		public List<AnimationSpeedEventInfo>		_animationSpeedEvents		= new List<AnimationSpeedEventInfo>();
		public List<CombatEventInfo>				_combatEvents				= new List<CombatEventInfo>();
		public List<GenericEventInfo>				_genericEvents				= new List<GenericEventInfo>();
		public List<HitEventInfo> 					_hitEvents 					= new List<HitEventInfo>();
		public List<ProjectileEventInfo>			_projectileEvents 			= new List<ProjectileEventInfo>();
		public List<ParticleEventInfo> 				_particleEvents 			= new List<ParticleEventInfo>();
		public List<TrailRendererEventInfo>			_trailRendererEvents		= new List<TrailRendererEventInfo>();
		public List<DynamicLightEventInfo>			_dynamicLightEvents			= new List<DynamicLightEventInfo>();
		public List<CameraShakeEventInfo>			_cameraShakeEvents 			= new List<CameraShakeEventInfo>();
		public List<CameraSwingEventInfo>			_cameraSwingEvents			= new List<CameraSwingEventInfo>();
		public List<CameraMotionEventInfo>			_cameraMotionEvents			= new List<CameraMotionEventInfo>();
		public List<EnvironmentLightingEventInfo>	_environmentLightingEvents 	= new List<EnvironmentLightingEventInfo>();
		public List<AudioEventInfo>					_audioEvents 				= new List<AudioEventInfo>();
		public List<PostBloomEventInfo>				_postBloomEvents			= new List<PostBloomEventInfo>();
		public List<PostVignetteEventInfo>			_postVignetteEvents			= new List<PostVignetteEventInfo>();
		public List<PostWarpEventInfo>				_postWarpEvents				= new List<PostWarpEventInfo>();
		public List<SwitchRenderSettingsEventInfo>	_switchRenderSettingsEvents = new List<SwitchRenderSettingsEventInfo>();

		public AnimationCurve _toAttack = null;
		public AnimationCurve _toAttackDist = null;
		public AnimationCurve _toFrameVsDistanceToTarget = null;
		public AnimationCurve _toReturn = null;
		public AnimationCurve _toReturnDist = null;
		public AnimationCurve _toEndDist = null;
		public AnimationCurve _zOffsetCurve = null;
		public AnimationCurve _yOffsetCurve = null;
		public Quaternion[] rot = null;
		public Vector3[] pos = null;

		// skill only
		public eSkillActionPositionType _actionPosition = eSkillActionPositionType.INVALID;
		public eSkillTargetPositionType _targetPosition = eSkillTargetPositionType.NONE;
		public float _attackRadius = 0.0f;
		public string _AttackLocationNode = "";
		public string _TargetLocationNode = "";
		public bool _cinematicSkill = false;

		[HideInInspector][System.NonSerialized]
		public AnimationEvent[] _animationEvents = null;
		[HideInInspector][System.NonSerialized]
		public Transform _attackLocation = null;
		[HideInInspector][System.NonSerialized]
		public Transform _targetLocation = null;

		public float    NumFrames       { get { return _animationClip != null ? _animationClip.frameRate * _animationClip.length : 1; } }
		public float    Speed           { get { return _animationSpeedEvent != null ? _animationSpeedEvent._floatParameter : 1.0f; } }
		public float    AdjustedLength  { get { return _animationClip != null ? _animationClip.length / Speed : 0.0f; } }

		public void Init()
		{
			GameObject myNode = null;

			_attackLocation = null;
			if(!string.IsNullOrEmpty(_AttackLocationNode) && _actionPosition == eSkillActionPositionType.TRANSFORM)
			{
				myNode = GameObject.Find("AttackLocations");
				if(myNode != null)
				{
					_attackLocation = myNode.transform.Find(_AttackLocationNode);
					if(_attackLocation == myNode)
					{
						_attackLocation = null;
						EB.Debug.LogError("could not find attack location: {0}", _AttackLocationNode);
					}
				}
				if(_attackLocation == null)
				{
					EB.Debug.LogError("could not find attack location: {0}" , _AttackLocationNode);
				}
			}

			_targetLocation = null;
			if (!string.IsNullOrEmpty(_TargetLocationNode) && _targetPosition == eSkillTargetPositionType.TRANSFORM )
			{
				myNode = myNode ?? GameObject.Find("AttackLocations");
				if (myNode != null)
				{
					_targetLocation = myNode.transform.Find(_TargetLocationNode);
					if (_targetLocation == myNode)
					{
						_targetLocation = null;
						EB.Debug.LogError("could not find target location: {0}" , _TargetLocationNode);
					}
				}
				if (_targetLocation == null)
				{
					EB.Debug.LogError("could not find target location: {0}", _TargetLocationNode);
				}
			}
		}

		public bool GetPosAndRot(float frame, ref Vector3 position, ref Quaternion rotation)
		{
			int intFrame = (int)Mathf.Floor (frame);
			if (intFrame+1 >= pos.Length || intFrame+1 >= rot.Length){
				return false;
			}
			position = Vector3.Lerp (pos [intFrame], pos [intFrame + 1], frame - intFrame);
			rotation = Quaternion.Slerp (rot [intFrame], rot [intFrame + 1], frame - intFrame);
			return true;
		}
		// 
		void TryAddMoveEvent(ref List<MoveEvent> events, EventInfo eventInfo, float numFrames, bool requiredEvents, float endFrame=-1.0f)
		{
			if(eventInfo._required == requiredEvents)
			{
				events.Add(eventInfo.ToMoveEvent(numFrames, endFrame));
			}
		}

		// runtime cache array
		private MoveEvent[] _cachedRequiredEvents = null;
		private MoveEvent[] _cachedEvents = null;
		public MoveEvent[] CompileMoveEvents(bool requiredEvents)
		{
			if (Application.isPlaying)
			{
				if (requiredEvents && _cachedRequiredEvents != null)
				{
					for (int i = 0, len = _cachedRequiredEvents.Length; i < len; ++i)
					{
						_cachedRequiredEvents[i].Reset();
					}
					return _cachedRequiredEvents;
				}

				if (!requiredEvents && _cachedEvents != null)
				{
					for (int i = 0, len = _cachedEvents.Length; i < len; ++i)
					{
						_cachedEvents[i].Reset();
					}
					return _cachedEvents;
				}
			}

			List<MoveEvent> events = new List<MoveEvent>();
			
			_moveEvent._moveEventProperties._name = name;
			_moveEvent._moveEventProperties._numHitEvents = _hitEvents.Count;
			_moveEvent._moveEventProperties._numProjectileEvents = _projectileEvents.Count;
			
			TryAddMoveEvent(ref events, _moveEvent, NumFrames, requiredEvents);
			TryAddMoveEvent(ref events, _animationSpeedEvent, NumFrames, requiredEvents);
			
			for (int i = 0; i < _animationSpeedEvents.Count; ++i)
			{
				TryAddMoveEvent(ref events, _animationSpeedEvents[i], NumFrames, requiredEvents);
			}

			_combatEvents.Sort(delegate (CombatEventInfo x, CombatEventInfo y)
			{
				if (x._frame == y._frame)
					return 0;
				if (x._frame < y._frame)
					return -1;
				return 1;
			});

			FillHitEventData();
			for (int i = 0; i < _combatEvents.Count; ++i)
			{
				if(_combatEvents[i].ShouldSetAnimationEvent())
				{
					TryAddMoveEvent(ref events, _combatEvents[i], NumFrames, requiredEvents, _combatEvents[i].combatEventProps._endFrame);
				}
			}
			
			for (int i = 0; i < _genericEvents.Count; i++)
				TryAddMoveEvent(ref events, _genericEvents[i], NumFrames, requiredEvents);

			_hitEvents.Sort(delegate(HitEventInfo x, HitEventInfo y)
			{
				if (x._frame == y._frame)
					return 0;
				if (x._frame < y._frame)
					return -1;
				return 1;
			});

			for (int i = 0; i < _hitEvents.Count; i++)
			{
				//_hitEvents[i]._damageProperties._hitEventIndex = i;
				//_hitEvents[i]._damageProperties._attackLevel = _attackLevel;
				TryAddMoveEvent(ref events, _hitEvents[i], NumFrames, requiredEvents);
			}            
			
			for (int i = 0; i < _projectileEvents.Count; i++)
			{
				//_projectileEvents[i]._hitEvent._damageProperties._hitEventIndex = i;
				//_projectileEvents[i]._hitEvent._damageProperties._attackLevel = _attackLevel;
				TryAddMoveEvent(ref events, _projectileEvents[i], NumFrames, requiredEvents);
			}
			
			for (int i = 0; i < _particleEvents.Count; i++)
				TryAddMoveEvent(ref events, _particleEvents[i], NumFrames, requiredEvents);
			
			for (int i = 0; i < _trailRendererEvents.Count; i++)
				TryAddMoveEvent(ref events, _trailRendererEvents[i], NumFrames, requiredEvents);
			
			for (int i = 0; i < _dynamicLightEvents.Count; i++)
				TryAddMoveEvent(ref events, _dynamicLightEvents[i], NumFrames, requiredEvents);
			
			for (int i = 0; i < _cameraShakeEvents.Count; i++)
				TryAddMoveEvent(ref events, _cameraShakeEvents[i], NumFrames, requiredEvents);
			
			for (int i = 0; i < _cameraSwingEvents.Count; i++)
				TryAddMoveEvent(ref events, _cameraSwingEvents[i], NumFrames, requiredEvents);
			
			for(int i = 0; i < _cameraMotionEvents.Count; i++)
			{
				TryAddMoveEvent(ref events, _cameraMotionEvents[i], NumFrames, requiredEvents);
			}
			
			for (int i = 0; i < _audioEvents.Count; i++)
				TryAddMoveEvent(ref events, _audioEvents[i], NumFrames, requiredEvents);
			
			for (int i = 0; i < _environmentLightingEvents.Count; i++)
			{
				_environmentLightingEvents[i].SetSpeed(Speed);
				TryAddMoveEvent(ref events, _environmentLightingEvents[i], NumFrames, requiredEvents);
			}
			
			for (int i = 0; i < _postBloomEvents.Count; i++)
			{
				_postBloomEvents[i]._speed = Speed;
				TryAddMoveEvent(ref events, _postBloomEvents[i], NumFrames, requiredEvents);
			}
			
			for (int i = 0; i < _postVignetteEvents.Count; i++)
			{
				_postVignetteEvents[i]._speed = Speed;
				TryAddMoveEvent(ref events, _postVignetteEvents[i], NumFrames, requiredEvents);
			}
			
			for (int i = 0; i < _postWarpEvents.Count; i++)
			{
				_postWarpEvents[i]._speed = Speed;
				TryAddMoveEvent(ref events, _postWarpEvents[i], NumFrames, requiredEvents);
			}

			for (int i = 0; i < _switchRenderSettingsEvents.Count; i++)
			{
				_switchRenderSettingsEvents[i]._speed = Speed;
				TryAddMoveEvent(ref events, _switchRenderSettingsEvents[i], NumFrames, requiredEvents);
			}

			events.Sort(delegate (MoveEvent x, MoveEvent y)
			{
				if(x.myAnimEvent.time == y.myAnimEvent.time)
					return 0;
				if(x.myAnimEvent.time < y.myAnimEvent.time)
					return -1;
				return 1;
			});
			
			MoveEvent[] array = events.ToArray();

			if (Application.isPlaying)
			{
				if (requiredEvents)
				{
					_cachedRequiredEvents = array;
				}
				else
				{
					_cachedEvents = array;
				}
			}

			return array;
		}

		#region cpu 25ms -> 分帧 4.35（Max）
		public AnimationEvent[] CompileEvents()
		{
			List<AnimationEvent> events = new List<AnimationEvent>();
			
			_moveEvent._moveEventProperties._name = name;
			_moveEvent._moveEventProperties._numHitEvents = _hitEvents.Count;
			_moveEvent._moveEventProperties._numProjectileEvents = _projectileEvents.Count;
			
			events.Add(_moveEvent.ToAnimationEvent(NumFrames).mAniEvent);
			events.Add(_animationSpeedEvent.ToAnimationEvent(NumFrames).mAniEvent);
			
			for (int i = 0; i < _animationSpeedEvents.Count; ++i)
			{
				events.Add(_animationSpeedEvents[i].ToAnimationEvent(NumFrames).mAniEvent);
			}

			_combatEvents.Sort(delegate (CombatEventInfo x, CombatEventInfo y)
			{
				if(x._frame == y._frame)
					return 0;
				if(x._frame < y._frame)
					return -1;
				return 1;
			});

			FillHitEventData();
			for (int i = 0; i < _combatEvents.Count; ++i)
			{
				if(_combatEvents[i].ShouldSetAnimationEvent())
				{
					events.Add(_combatEvents[i].ToAnimationEvent(NumFrames).mAniEvent);
				}
			}
			
			for (int i = 0; i < _genericEvents.Count; i++)
				events.Add(_genericEvents[i].ToAnimationEvent(NumFrames).mAniEvent);
			
			for (int i = 0; i < _hitEvents.Count; i++)
			{
				//_hitEvents[i]._damageProperties._hitEventIndex = i;
				//_hitEvents[i]._damageProperties._attackLevel = _attackLevel;
				events.Add(_hitEvents[i].ToAnimationEvent(NumFrames).mAniEvent);
			}
			
			for (int i = 0; i < _projectileEvents.Count; i++)
			{
				//_projectileEvents[i]._hitEvent._damageProperties._hitEventIndex = i;
				//_projectileEvents[i]._hitEvent._damageProperties._attackLevel = _attackLevel;
				events.Add(_projectileEvents[i].ToAnimationEvent(NumFrames).mAniEvent);
			}
			
			for (int i = 0; i < _particleEvents.Count; i++)
				events.Add(_particleEvents[i].ToAnimationEvent(NumFrames).mAniEvent);
			
			for (int i = 0; i < _trailRendererEvents.Count; i++)
				events.Add(_trailRendererEvents[i].ToAnimationEvent(NumFrames).mAniEvent);
			
			for (int i = 0; i < _dynamicLightEvents.Count; i++)
				events.Add(_dynamicLightEvents[i].ToAnimationEvent(NumFrames).mAniEvent);
			
			for (int i = 0; i < _cameraShakeEvents.Count; i++)
				events.Add(_cameraShakeEvents[i].ToAnimationEvent(NumFrames).mAniEvent);
			
			for (int i = 0; i < _cameraSwingEvents.Count; i++)
				events.Add(_cameraSwingEvents[i].ToAnimationEvent(NumFrames).mAniEvent);
			
			for(int i = 0; i < _cameraMotionEvents.Count; i++)
			{
				events.Add(_cameraMotionEvents[i].ToAnimationEvent(NumFrames).mAniEvent);
			}
			
			for (int i = 0; i < _audioEvents.Count; i++)
				events.Add(_audioEvents[i].ToAnimationEvent(NumFrames).mAniEvent);
			
			for (int i = 0; i < _environmentLightingEvents.Count; i++)
			{
				_environmentLightingEvents[i].SetSpeed(Speed);
				events.Add(_environmentLightingEvents[i].ToAnimationEvent(NumFrames).mAniEvent);
			}
			
			for (int i = 0; i < _postBloomEvents.Count; i++)
			{
				_postBloomEvents[i]._speed = Speed;
				events.Add(_postBloomEvents[i].ToAnimationEvent(NumFrames).mAniEvent);
			}
			
			for (int i = 0; i < _postVignetteEvents.Count; i++)
			{
				_postVignetteEvents[i]._speed = Speed;
				events.Add(_postVignetteEvents[i].ToAnimationEvent(NumFrames).mAniEvent);
			}
			
			for (int i = 0; i < _postWarpEvents.Count; i++)
			{
				_postWarpEvents[i]._speed = Speed;
				events.Add(_postWarpEvents[i].ToAnimationEvent(NumFrames).mAniEvent);
			}

			for (int i = 0; i < _switchRenderSettingsEvents.Count; i++)
			{
				_switchRenderSettingsEvents[i]._speed = Speed;
				events.Add(_switchRenderSettingsEvents[i].ToAnimationEvent(NumFrames).mAniEvent);
			}
			
			events.Sort(delegate (AnimationEvent x, AnimationEvent y)
			{
				if(x.time == y.time)
					return 0;
				if(x.time < y.time)
					return -1;
				return 1;
			});
			
			return events.ToArray();
		}
		#endregion
		
		private void FillHitEventData()
		{
			int totalWeight = 0;
			int totalHitNum = 0;
			for (int i = 0; i < _combatEvents.Count; i++)
			{
				if (_combatEvents[i].eventType == CombatEventInfo.CombatEventType.PlayTargetHitReaction)
				{
					totalWeight += _combatEvents[i].hitRxnProps.weight;
					totalHitNum++;
				}
			}
			
			for (int i = 0; i < _combatEvents.Count; i++)
			{
				if (_combatEvents[i].eventType == CombatEventInfo.CombatEventType.PlayTargetHitReaction)
				{
					_combatEvents[i].hitRxnProps.totalWeight = totalWeight;
					_combatEvents[i].hitRxnProps.hitNum = totalHitNum;
				}
			}
			
			totalWeight = 0;
			totalHitNum = 0;
			for (int i = 0; i < _hitEvents.Count; ++i)
			{
				totalWeight += _hitEvents[i]._hitRxnProps.weight;
				totalHitNum++;
			}
			for (int i = 0; i < _hitEvents.Count; ++i)
			{
				_hitEvents[i]._hitRxnProps.totalWeight = totalWeight;
				_hitEvents[i]._hitRxnProps.hitNum = totalHitNum;
			}
		}

#if UNITY_EDITOR
		public int[] GetCombatSkillTriggerFrames()
		{
			int[] result = null;
			if(_combatEvents.Count > 0)
			{
				int lastFrame = (int)(_animationClip.length * _animationClip.frameRate);
				result = new int[] { 0, -1, -1, lastFrame };
				for(int ii = 0; ii < _combatEvents.Count; ++ii)
				{
					if(_combatEvents[ii].IsTriggerFrame())
					{
						result[(int)_combatEvents[ii].eventType] = (int)_combatEvents[ii]._frame;
					}
				}
			}
			if(result != null && result[1] != -1 && result[2] != -1)
				return result;
			return null;
		}

		public void AddEvent(EventInfo evt)
		{
			if (evt != null)
			{
				if (evt is GenericEventInfo)
				{
					_genericEvents.Add((GenericEventInfo)evt);
				}
				else if (evt is AnimationSpeedEventInfo)
				{
					_animationSpeedEvent = (AnimationSpeedEventInfo)evt;
				}
				else if (evt is MoveEventInfo)
				{
					_moveEvent = (MoveEventInfo)evt;
				}
				else if (evt is HitEventInfo)
				{
					_hitEvents.Add((HitEventInfo)evt);
				}
				else if (evt is ProjectileEventInfo)
				{
					_projectileEvents.Add((ProjectileEventInfo)evt);
				}
				else if (evt is ParticleEventInfo)
				{
					_particleEvents.Add((ParticleEventInfo)evt);
				}
				else if(evt is SwitchRenderSettingsEventInfo)
				{
					_switchRenderSettingsEvents.Add((SwitchRenderSettingsEventInfo)evt);
				}
				else if (evt is TrailRendererEventInfo)
				{
					_trailRendererEvents.Add((TrailRendererEventInfo)evt);
				}
				else if (evt is DynamicLightEventInfo)
				{
					_dynamicLightEvents.Add((DynamicLightEventInfo)evt);
				}
				else if (evt is CameraShakeEventInfo)
				{
					_cameraShakeEvents.Add((CameraShakeEventInfo)evt);
				}
				else if (evt is CameraSwingEventInfo)
				{
					_cameraSwingEvents.Add((CameraSwingEventInfo)evt);
				}
				else if(evt is CameraMotionEventInfo)
				{
					_cameraMotionEvents.Add((CameraMotionEventInfo)evt);
				}
				else if (evt is EnvironmentLightingEventInfo)
				{
					_environmentLightingEvents.Add((EnvironmentLightingEventInfo)evt);
				}
				else if (evt is AudioEventInfo)
				{
					_audioEvents.Add((AudioEventInfo)evt);
				}

				else
				{
					EB.Debug.LogWarning("COULD NOT COPY EVENT OF TYPE: {0}" , evt.GetType());
				}
			}
			else
			{
				EB.Debug.LogWarning("TRIED TO COPY NULL EVENT");
			}
		}

		public int GetIndex(EventInfo evt)
		{
			if (evt != null)
			{
				if (evt is GenericEventInfo)
				{
					return _genericEvents.IndexOf((GenericEventInfo)evt);
				}
				else if (evt is AnimationSpeedEventInfo)
				{
					return 0;
				}
				else if (evt is MoveEventInfo)
				{
					return 0;
				}
				else if (evt is HitEventInfo)
				{
					return _hitEvents.IndexOf((HitEventInfo)evt);
				}
				else if (evt is ProjectileEventInfo)
				{
					return _projectileEvents.IndexOf((ProjectileEventInfo)evt);
				}
				else if (evt is ParticleEventInfo)
				{
					return _particleEvents.IndexOf((ParticleEventInfo)evt);
				}
				else if(evt is SwitchRenderSettingsEventInfo)
				{
					return _switchRenderSettingsEvents.IndexOf((SwitchRenderSettingsEventInfo)evt);
				}
				else if (evt is TrailRendererEventInfo)
				{
					return _trailRendererEvents.IndexOf((TrailRendererEventInfo)evt);
				}
				else if (evt is DynamicLightEventInfo)
				{
					return _dynamicLightEvents.IndexOf((DynamicLightEventInfo)evt);
				}
				else if (evt is CameraShakeEventInfo)
				{
					return _cameraShakeEvents.IndexOf((CameraShakeEventInfo)evt);
				}
				else if (evt is CameraSwingEventInfo)
				{
					return _cameraSwingEvents.IndexOf((CameraSwingEventInfo)evt);
				}
				else if(evt is CameraMotionEventInfo)
				{
					return _cameraMotionEvents.IndexOf((CameraMotionEventInfo)evt);
				}
				else if (evt is EnvironmentLightingEventInfo)
				{
					return _environmentLightingEvents.IndexOf((EnvironmentLightingEventInfo)evt);
				}
				else if (evt is AudioEventInfo)
				{
					return _audioEvents.IndexOf((AudioEventInfo)evt);
				}
				else
				{
					EB.Debug.LogWarning("event was not found: {0}", evt);
					return -2;
				}
			}
			else
			{
				EB.Debug.LogWarning("event is null");
				return -1;
			}
		}

		public void SetPendingChanges()
		{
			//if (!IsCheckedOut() && !IsMarkedForAdd())
			//{
			//    UnityEditor.VersionControl.Task task = UnityEditor.VersionControl.Provider.Checkout(PrefabUtility.GetPrefabObject(this), UnityEditor.VersionControl.CheckoutMode.Both);
			//    task.Wait();
			//}

			EditorUtility.SetDirty(this);
		}

		public bool HasChangesPending()
		{
			return IsCheckedOut() || IsMarkedForAdd();
		}

		private bool IsCheckedOut()
		{
			return GetVCAsset().IsState(UnityEditor.VersionControl.Asset.States.CheckedOutLocal);
		}

		private bool IsMarkedForAdd()
		{
			return GetVCAsset().IsState(UnityEditor.VersionControl.Asset.States.AddedLocal);
		}

		private UnityEditor.VersionControl.Asset GetVCAsset()
		{
			if (_asset == null)
			{
				string path = AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabObject(this));
				_asset = UnityEditor.VersionControl.Provider.GetAssetByPath(path);
			}

			return _asset;
		}

		private UnityEditor.VersionControl.Asset _asset = null;
#endif
	}
}
