///////////////////////////////////////////////////////////////////////
//
//  FusionAudio.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fabric;
using Pathfinding.RVO;

public class FusionAudio : MonoBehaviour
#if DEBUG
	, IDebuggableEx
#endif
{	
	private static string _lastGlobalMusicEvent;
	private static string _lastAmbienceEvent;
	
	private static string[] _eventDisplayNames;
	public static string[] EventDisplayNames
	{
		get
		{
			if (_eventDisplayNames == null)
			{
				List<string> displayNameList = new List<string>();

				System.Array enumValues = System.Enum.GetValues(typeof(FusionAudio.eEvent));
				foreach (FusionAudio.eEvent evt in enumValues)
				{
					displayNameList.Add(FusionAudio.EventString(evt));
				}
				
				_eventDisplayNames = displayNameList.ToArray();
			}
			
			return _eventDisplayNames;
		}
	}
	
	private static EnvironmentHelper.eEnvironmentType _currentEnvironmentType = EnvironmentHelper.eEnvironmentType.Unknown;
	private static float _lastChangeEnvironmentTimestamp = 0.0f;

	private static Hashtable _currentGroundTypes = Johny.HashtablePool.Claim();

	private static string _lastDungeonMusicEvent;
	private static float _lastMusicChangeTimestamp = 0.0f;
	private static eDungeonMusicLevel _lastDungeonMusicLevel = eDungeonMusicLevel.Unknown;

	private static string _lastZonePrefix = "";
	private string _lastAmbientEventNameRoot;

	private const float _changeEnvironmentThreshold = 1.8f;
	private const float _changeMusicThreshold = 0.5f;
	
	private static GameObject _playerObject;
	private static GameObject _lastAttackTarget;

	private static CharacterTargetingComponent _localPlayerTargetingComponent;
	private static CombatController _localPlayerCombatController;

	private static Camera _levelCamera;
	
	public enum eEvent
	{
		None = 0,

		// Music 1 - 999
		Music_LoadingStart = 1,
		Music_LoadingEnd,
		Music_DungeonStart,
		Music_DungeonEnd,
		Music_OverworldStart,
		Music_OverworldEnd,
		Music_Level_Cave_Lo,
		Music_Level_Cave_Med,
		Music_Level_Cave_Hi,
		Music_Level_Forest_Lo,
		Music_Level_Forest_Med,
		Music_Level_Forest_Hi,
		Music_OW_Main,
		Music_OW_Explore,
		Music_Level_Cave_Stinger_Win,
		Music_Level_Cave_Stinger_Lose,
		Music_Level_Cave_Stinger_Victory,
		Music_Avatar,
		Music_Level_Forest_Stinger_Win,
		Music_Level_Forest_Stinger_Lose,
		Music_Level_Forest_Stinger_Victory,
		Music_Level_Cave_IntroSting,
		Music_Level_Forest_IntroSting,
		Music_Level_ForestTown_Lo,
		Music_Level_ForestTown_Med,
		Music_Level_ForestTown_Hi,
		Music_Level_ForestTown_IntroSting,		
		Music_Level_Docks_Lo,
		Music_Level_Docks_Med,
		Music_Level_Docks_Hi,
		Music_Level_Docks_IntroSting,		
		Music_Level_Crypt_Lo,
		Music_Level_Crypt_Med,
		Music_Level_Crypt_Hi,
		Music_Level_Crypt_IntroSting,
		SFX_VictoryScreen_Music,
		Music_UI_SpiritScreen,
		Music_Level_Prologue,
		Music_Level_PrologueBoat,
		Music_Level_PrologueDock,
		Music_Level_PrologueWeaponGlow,
		Music_RelicStore,
		SFX_UI_SpiritScreenMusic,
		
		// Combat SFX 1000 - 2999
		SFX_Combat_MeleeSwing = 1000,
		SFX_Combat_MeleeHit,

		// Player Character SFX 3000 - 12999
		// Sorcerer Abilities
		SFX_Sorcerer_Ability_AA1_CastM = 3000, 
		SFX_Sorcerer_Ability_AA1_CastF,
		SFX_Sorcerer_Ability_AA2_CastM,
		SFX_Sorcerer_Ability_AA2_CastF,
		SFX_Sorcerer_Ability_AA3_CastM,
		SFX_Sorcerer_Ability_AA3_CastF = 3005,
		SFX_Sorcerer_Ability_AA_Travel_Physical,
		SFX_Sorcerer_Ability_AA_Travel_Fire,
		SFX_Sorcerer_Ability_AA_Travel_Ice,
		SFX_Sorcerer_Ability_AA_Travel_Nature,
		SFX_Sorcerer_Ability_AA_Travel_Arcane = 3010,
		SFX_Sorcerer_Ability_AA_Travel_Void,
		SFX_Sorcerer_Ability_AA_Hit_Physical,
		SFX_Sorcerer_Ability_AA_Hit_Fire,
		SFX_Sorcerer_Ability_AA_Hit_Ice,
		SFX_Sorcerer_Ability_AA_Hit_Nature = 3015,
		SFX_Sorcerer_Ability_AA_Hit_Arcane,
		SFX_Sorcerer_Ability_AA_Hit_Void,
		SFX_Sorcerer_Ability_Blink_CastM = 3020,
		SFX_Sorcerer_Ability_Blink_CastF,
		SFX_Sorcerer_Ability_Fireball_Prepare = 3040,
		SFX_Sorcerer_Ability_Fireball_Cast,
		SFX_Sorcerer_Ability_Fireball_Travel,
		SFX_Sorcerer_Ability_Fireball_Blast,
		SFX_Sorcerer_Ability_Fireball_ChargeUp,
		SFX_Sorcerer_Ability_Fireball_Loop,
		SFX_Sorcerer_Ability_ChainLightning_Cast = 3060,
		SFX_Sorcerer_Ability_ChainLightning_Hit,
		SFX_Sorcerer_Ability_Disintegration_Cast = 3080,
		SFX_Sorcerer_Ability_Disintegration_Hit,
		SFX_Sorcerer_Ability_Disintegration_Loop,
		SFX_Sorcerer_Ability_Disintegration_LoopEnd,
		SFX_Sorcerer_Ability_FrostNova_BlastM = 3100,
		SFX_Sorcerer_Ability_FrostNova_Sheen,
		SFX_Sorcerer_Ability_FrostNova_Shatter,
		SFX_Sorcerer_Ability_FrostNova_BlastF,
		SFX_Sorcerer_Ability_Detonation_BlastM = 3120,
		SFX_Sorcerer_Ability_Detonation_BlastF,
		SFX_Sorcerer_Ability_EnergyShield_Loop = 3140,
		SFX_Sorcerer_Ability_EnergyShield_Blast,
		SFX_Sorcerer_Ability_EnergyShield_CastM,
		SFX_Sorcerer_Ability_EnergyShield_CastF,
		SFX_Sorcerer_Ability_IceStorm_Loop = 3160,
		SFX_Sorcerer_Ability_IceStorm_LoopEnd,
		SFX_Sorcerer_Ability_IceStorm_Hit,
		SFX_Sorcerer_Ability_IceStorm_CastM,
		SFX_Sorcerer_Ability_IceStorm_CastF,
		SFX_Sorcerer_Ability_ChaosField_Loop = 3180,
		SFX_Sorcerer_Ability_ChaosField_LoopEnd,
		SFX_Sorcerer_Ability_TimeWarp_Loop = 3200,
		SFX_Sorcerer_Ability_TimeWarp_LoopEnd,
		SFX_Sorcerer_Ability_MeteorSwarm_Loop = 3220,
		SFX_Sorcerer_Ability_MeteorSwarm_LoopEnd,
		SFX_Sorcerer_Ability_MeteorSwarm_Telegraph,
		SFX_Sorcerer_Ability_MeteorSwarm_Blast,
		SFX_Sorcerer_Ability_MeteorSwarm_CastM,
		SFX_Sorcerer_Ability_MeteorSwarm_CastF,
		SFX_Sorcerer_Ability_Singularity_Loop = 3240,
		SFX_Sorcerer_Ability_Singularity_LoopEnd,
		SFX_Sorcerer_Ability_Singularity_Blast,
		SFX_Sorcerer_Ability_Singularity_CastM,
		SFX_Sorcerer_Ability_Singularity_CastF,
		SFX_Sorcerer_Ability_PolarityShift_BlastM = 3250,
		SFX_Sorcerer_Ability_PolarityShift_BlastF,
		SFX_Sorcerer_Ability_PolarityShift_Loop,
		SFX_Sorcerer_Ability_Tempest_Loop = 3260,
		SFX_Sorcerer_Ability_Tempest_LoopEnd,
		SFX_Sorcerer_Ability_Tempest_Hit,
		SFX_Sorcerer_Ability_Tempest_CastM,
		SFX_Sorcerer_Ability_Tempest_CastF,
		SFX_Sorcerer_Ability_ConeOfCold_Hit = 3280,
		SFX_Sorcerer_Ability_ConeOfCold_Loop,
		SFX_Sorcerer_Ability_GustingWinds_Hit = 3285,
		SFX_Sorcerer_Ability_GustingWinds_Loop,
		SFX_Sorcerer_Ability_GraspingRoots_BlastM = 3290,
		SFX_Sorcerer_Ability_GraspingRoots_BlastF,
		SFX_Sorcerer_Ability_EssenceLeech_BlastM = 3295,
		SFX_Sorcerer_Ability_EssenceLeech_BlastF,
		SFX_Sorcerer_Ability_FieryBlink_CastM = 3300,
		SFX_Sorcerer_Ability_FieryBlink_Blast,
		SFX_Sorcerer_Ability_FieryBlink_CastF,
		SFX_Sorcerer_Ability_FrozenBlink_CastM = 3305,
		SFX_Sorcerer_Ability_FrozenBlink_Blast,
		SFX_Sorcerer_Ability_FrozenBlink_CastF,
		SFX_Sorcerer_Ability_GustyBlink_CastM = 3310,
		SFX_Sorcerer_Ability_GustyBlink_Blast,
		SFX_Sorcerer_Ability_GustyBlink_CastF,
		SFX_Sorcerer_Ability_MirroredBlink_CastM = 3315,
		SFX_Sorcerer_Ability_MirroredBlink_Blast,
		SFX_Sorcerer_Ability_MirroredBlink_CastF,
		SFX_Sorcerer_Ability_VortexBlink_CastM = 3320,
		SFX_Sorcerer_Ability_VortexBlink_Blast,
		SFX_Sorcerer_Ability_VortexBlink_CastF,
		SFX_Sorcerer_Ability_Thunderbolt_CastM = 3325,
		SFX_Sorcerer_Ability_Thunderbolt_Blast,
		SFX_Sorcerer_Ability_Thunderbolt_CastF,
		SFX_Sorcerer_Ability_FireShield_Blast = 3330,
		SFX_Sorcerer_Ability_FireShield_Loop,
		SFX_Sorcerer_Ability_FireShield_Hit,
		SFX_Sorcerer_Ability_FireShield_CastM,
		SFX_Sorcerer_Ability_FireShield_CastF,
		SFX_Sorcerer_Ability_IceShield_Blast = 3335,
		SFX_Sorcerer_Ability_IceShield_Loop,
		SFX_Sorcerer_Ability_IceShield_Hit,
		SFX_Sorcerer_Ability_IceShield_CastM,
		SFX_Sorcerer_Ability_IceShield_CastF,
		SFX_Sorcerer_Ability_GustyShield_Blast = 3340,
		SFX_Sorcerer_Ability_GustyShield_Loop,
		SFX_Sorcerer_Ability_GustyShield_CastM,
		SFX_Sorcerer_Ability_GustyShield_CastF,
		SFX_Sorcerer_Ability_DarkShield_Blast = 3345,
		SFX_Sorcerer_Ability_DarkShield_Loop,
		SFX_Sorcerer_Ability_DarkShield_Hit,
		SFX_Sorcerer_Ability_DarkShield_CastM,
		SFX_Sorcerer_Ability_DarkShield_CastF,
		SFX_Sorcerer_Ability_StaticBarrier_Hit = 3350,
		SFX_Sorcerer_Ability_StaticBarrier_Loop,
		SFX_Sorcerer_Ability_FlamingHands_Hit = 3355,
		SFX_Sorcerer_Ability_FlamingHands_Loop,
		SFX_Sorcerer_Ability_SurgingPower_Hit = 3360,
		SFX_Sorcerer_Ability_SurgingPower_Loop,
		SFX_Sorcerer_Ability_FrozenOrb_Blast = 3365,
		SFX_Sorcerer_Ability_FrozenOrb_Cast,
		SFX_Sorcerer_Ability_FrozenOrb_Travel,
		SFX_Sorcerer_Ability_NoxiousBlast_Blast = 3370,
		SFX_Sorcerer_Ability_NoxiousBlast_Cast,
		SFX_Sorcerer_Ability_NoxiousBlast_Travel,
		SFX_Sorcerer_Ability_ChargedBolts_BlastM = 3375,
		SFX_Sorcerer_Ability_ChargedBolts_Travel,
		SFX_Sorcerer_Ability_ChargedBolts_Hit,
		SFX_Sorcerer_Ability_ChargedBolts_BlastF,
		SFX_Sorcerer_Ability_SonicBoom_BlastM = 3380,
		SFX_Sorcerer_Ability_SonicBoom_BlastF,
		SFX_Sorcerer_Ability_Incineration_BlastM = 3385,
		SFX_Sorcerer_Ability_Incineration_BlastF,
		SFX_Sorcerer_Ability_Freeze_BlastM = 3390,
		SFX_Sorcerer_Ability_Freeze_Sheen,
		SFX_Sorcerer_Ability_Freeze_Shatter,
		SFX_Sorcerer_Ability_Freeze_BlastF,
		SFX_Sorcerer_Ability_PoisonCloud_BlastM = 3395,
		SFX_Sorcerer_Ability_PoisonCloud_Loop,
		SFX_Sorcerer_Ability_PoisonCloud_BlastF,
		
		// Barbarian Abilities
		SFX_Barbarian_Ability_BarbAA1_Hit = 3400,
		SFX_Barbarian_Ability_BarbAA2_Hit,
		SFX_Barbarian_Ability_BarbAA3_Hit,
		SFX_Barbarian_Ability_Leap_Cast = 3420,
		SFX_Barbarian_Ability_Leap_Blast,
		SFX_Barbarian_Ability_Leap_CastM,
		SFX_Barbarian_Ability_Leap_CastF,
		SFX_Barbarian_Ability_Impact_Loop = 3440,
		SFX_Barbarian_Ability_Impact_LoopEnd,
		SFX_Barbarian_Ability_Impact_Blast,
		SFX_Barbarian_Ability_Impact_ChargeUp,
		SFX_Barbarian_Ability_ArmorBreaker_Loop = 3460,
		SFX_Barbarian_Ability_ArmorBreaker_LoopEnd,
		SFX_Barbarian_Ability_ArmorBreaker_Blast,
		SFX_Barbarian_Ability_ArmorBreaker_ChargeUp,
		SFX_Barbarian_Ability_Laceration_Loop = 3480,
		SFX_Barbarian_Ability_Laceration_LoopEnd,
		SFX_Barbarian_Ability_Laceration_Blast,
		SFX_Barbarian_Ability_Laceration_ChargeUp,
		SFX_Barbarian_Ability_Enrage_Swoosh = 3500,
		SFX_Barbarian_Ability_Enrage_Hit,
		SFX_Barbarian_Ability_Unbalance_Swoosh = 3520,
		SFX_Barbarian_Ability_Unbalance_Hit,
		SFX_Barbarian_Ability_OpenWounds_Swoosh = 3540,
		SFX_Barbarian_Ability_OpenWounds_Hit,
		SFX_Barbarian_Ability_Warcry_Cast = 3560,
		SFX_Barbarian_Ability_Demoralize_Cast = 3580,
		SFX_Barbarian_Ability_Intimidation_Cast = 3600,
		SFX_Barbarian_Ability_Execute_Cast = 3620,
		SFX_Barbarian_Ability_Execute_Blast,
		SFX_Barbarian_Ability_Execute_CastM,
		SFX_Barbarian_Ability_Execute_CastF,
		SFX_Barbarian_Ability_Whirlwind_Loop = 3640,
		SFX_Barbarian_Ability_Whirlwind_LoopEnd,
		SFX_Barbarian_Ability_Whirlwind_Hit,
		SFX_Barbarian_Ability_UndyingWill_Cast = 3660,
		SFX_Barbarian_Ability_UndyingWill_Loop,
		SFX_Barbarian_Ability_UndyingWill_LoopEnd,
		SFX_Barbarian_Ability_UndyingWill_End,
		SFX_Barbarian_Ability_Punt_ChargeUp = 3680,
		SFX_Barbarian_Ability_Punt_Loop,
		SFX_Barbarian_Ability_Punt_Blast,
		SFX_Barbarian_Ability_ThrowingAxe_ChargeUp = 3700,
		SFX_Barbarian_Ability_ThrowingAxe_Loop,
		SFX_Barbarian_Ability_ThrowingAxe_Cast,
		SFX_Barbarian_Ability_ThrowingAxe_Hit,
		SFX_Barbarian_Ability_ThrowingAxe_Travel,
		SFX_Barbarian_Ability_Quake_ChargeUp = 3720,
		SFX_Barbarian_Ability_Quake_Loop,
		SFX_Barbarian_Ability_Quake_Blast,
		SFX_Barbarian_Ability_Quake_Travel,
		SFX_Barbarian_Ability_Quake_Hit,
		SFX_Barbarian_Ability_Flametongue_Cast = 3740,
		SFX_Barbarian_Ability_Flametongue_Loop,
		SFX_Barbarian_Ability_Flametongue_Hit,
		SFX_Barbarian_Ability_Totem_Cast = 3760,
		SFX_Barbarian_Ability_Totem_ChargeUp,
		SFX_Barbarian_Ability_Totem_Blast,	
		SFX_Barbarian_Ability_Cleave_Swoosh = 3780,
		SFX_Barbarian_Ability_Cleave_Hit,
		SFX_Barbarian_Ability_DragonBreath_ChargeUp = 3785,
		SFX_Barbarian_Ability_DragonBreath_Loop,
		SFX_Barbarian_Ability_DragonBreath_Blast,
		SFX_Barbarian_Ability_FrozenAxe_ChargeUp = 3790,
		SFX_Barbarian_Ability_FrozenAxe_Loop,
		SFX_Barbarian_Ability_FrozenAxe_Cast,
		SFX_Barbarian_Ability_FrozenAxe_Hit,
		SFX_Barbarian_Ability_FrozenAxe_Travel,
		SFX_Barbarian_Ability_Shockwave_ChargeUp = 3800,
		SFX_Barbarian_Ability_Shockwave_Loop,
		SFX_Barbarian_Ability_Shockwave_Blast,
		SFX_Barbarian_Ability_SoulSiphon_ChargeUp = 3805,
		SFX_Barbarian_Ability_SoulSiphon_Loop,
		SFX_Barbarian_Ability_SoulSiphon_Blast,
		SFX_Barbarian_Ability_SoulSiphon_Loop2,
		SFX_Barbarian_Ability_SoulSiphon_End,
		SFX_Barbarian_Ability_Frostbrand_Cast = 3810,
		SFX_Barbarian_Ability_Frostbrand_Loop,
		SFX_Barbarian_Ability_Frostbrand_Hit,
		SFX_Barbarian_Ability_CycloneTotem_Cast = 3815,
		SFX_Barbarian_Ability_CycloneTotem_ChargeUp,
		SFX_Barbarian_Ability_CycloneTotem_Blast,
		SFX_Barbarian_Ability_DarkRetribution_Cast = 3820,
		SFX_Barbarian_Ability_DarkRetribution_Loop,
		SFX_Barbarian_Ability_DarkRetribution_Hit,
		SFX_Barbarian_Ability_ExplosiveLeap_Blast = 3825,
		SFX_Barbarian_Ability_ExplosiveLeap_Cast,
		SFX_Barbarian_Ability_ExplosiveLeap_CastM,
		SFX_Barbarian_Ability_ExplosiveLeap_CastF,
		SFX_Barbarian_Ability_ArmoredLeap_Blast = 3830,
		SFX_Barbarian_Ability_ArmoredLeap_Cast,
		SFX_Barbarian_Ability_ArmoredLeap_CastM,
		SFX_Barbarian_Ability_ArmoredLeap_CastF,
		SFX_Barbarian_Ability_GraspingLeap_Blast = 3835,
		SFX_Barbarian_Ability_GraspingLeap_Cast,
		SFX_Barbarian_Ability_GraspingLeap_CastM,
		SFX_Barbarian_Ability_GraspingLeap_CastF,
		SFX_Barbarian_Ability_StunningLeap_Blast = 3840,
		SFX_Barbarian_Ability_StunningLeap_Cast,
		SFX_Barbarian_Ability_StunningLeap_CastM,
		SFX_Barbarian_Ability_StunningLeap_CastF,
		SFX_Barbarian_Ability_VortexLeap_Blast = 3845,
		SFX_Barbarian_Ability_VortexLeap_Cast,
		SFX_Barbarian_Ability_VortexLeap_CastM,
		SFX_Barbarian_Ability_VortexLeap_CastF,
		SFX_Barbarian_Ability_VolcanicStrikes_Cast = 3850,
		SFX_Barbarian_Ability_VolcanicStrikes_Blast,
		SFX_Barbarian_Ability_Hoarfrost_Cast = 3855,
		SFX_Barbarian_Ability_Hoarfrost_Blast,
		SFX_Barbarian_Ability_Hoarfrost_Sheen,
		SFX_Barbarian_Ability_Hoarfrost_Shatter,
		SFX_Barbarian_Ability_VolcanicStrikes_Loop = 3860,
		SFX_Barbarian_Ability_SubZero_ChargeUp = 3865,
		SFX_Barbarian_Ability_SubZero_Loop,
		SFX_Barbarian_Ability_SubZero_Blast,
		SFX_Barbarian_Ability_ZephyrTotem_Cast = 3870,
		SFX_Barbarian_Ability_ZephyrTotem_Buff,
		SFX_Barbarian_Ability_ExplosiveTotem_Cast = 3875,
		SFX_Barbarian_Ability_ArcticTotem_Cast = 3880,
		SFX_Barbarian_Ability_RetributionTotem_Cast = 3885,
		SFX_Barbarian_Ability_ThunderousSteps_Cast = 3890,
		SFX_Barbarian_Ability_ThunderousSteps_Blast,
		SFX_Barbarian_Ability_BurningMan_Cast = 3895,
		SFX_Barbarian_Ability_BurningMan_Blast,
		SFX_Barbarian_Ability_FrenziedRegeneration_Cast = 3900,
		SFX_Barbarian_Ability_FrenziedRegeneration_Loop,
		SFX_Barbarian_Ability_FrenziedRegeneration_End,
		SFX_Barbarian_Ability_FistOfLightning_ChargeUp = 3905,
		SFX_Barbarian_Ability_FistOfLightning_Loop,
		SFX_Barbarian_Ability_FistOfLightning_Cast,
		SFX_Barbarian_Ability_FistOfLightning_Hit,
		SFX_Barbarian_Ability_FistOfLightning_Travel,
		SFX_Barbarian_Ability_FlameSpear_ChargeUp = 3910,
		SFX_Barbarian_Ability_FlameSpear_Loop,
		SFX_Barbarian_Ability_FlameSpear_Cast,
		SFX_Barbarian_Ability_FlameSpear_Hit,
		SFX_Barbarian_Ability_FlameSpear_Travel,
		SFX_Barbarian_Ability_HammerOfTheEarth_ChargeUp = 3915,
		SFX_Barbarian_Ability_HammerOfTheEarth_Loop,
		SFX_Barbarian_Ability_HammerOfTheEarth_Cast,
		SFX_Barbarian_Ability_HammerOfTheEarth_Hit,
		SFX_Barbarian_Ability_HammerOfTheEarth_Travel,
		SFX_Barbarian_Ability_VoidRift_ChargeUp = 3920,
		SFX_Barbarian_Ability_VoidRift_Loop,
		SFX_Barbarian_Ability_VoidRift_Cast,
		SFX_Barbarian_Ability_VoidRift_Hit,
		SFX_Barbarian_Ability_VoidRift_Travel,
		SFX_Barbarian_Ability_ChargedWeapon_Cast = 3925,
		SFX_Barbarian_Ability_ChargedWeapon_Loop,
		SFX_Barbarian_Ability_ChargedWeapon_Hit,
		SFX_Barbarian_Ability_EarthenStrikes_Cast = 3930,
		SFX_Barbarian_Ability_EarthenStrikes_Loop,
		SFX_Barbarian_Ability_EarthenStrikes_Hit,
		SFX_Barbarian_Ability_DarkenWeapon_Cast = 3935,
		SFX_Barbarian_Ability_DarkenWeapon_Loop,
		SFX_Barbarian_Ability_DarkenWeapon_Hit,
		SFX_Barbarian_Ability_ShatterArmor_Cast = 3940,
		SFX_Barbarian_Ability_ShatterArmor_Blast,
		SFX_Barbarian_Ability_ThirstOfSouls_Blast = 3945,
		SFX_Barbarian_Ability_AA1_CastM = 3950,
		SFX_Barbarian_Ability_AA1_CastF,
		SFX_Barbarian_Ability_AA2_CastM,
		SFX_Barbarian_Ability_AA2_CastF,
		SFX_Barbarian_Ability_AA3_CastM,
		SFX_Barbarian_Ability_AA3_CastF = 3955,
		SFX_Barbarian_Ability_AA1_Hit_Physical,
		SFX_Barbarian_Ability_AA1_Hit_Arcane,
		SFX_Barbarian_Ability_AA1_Hit_Fire,
		SFX_Barbarian_Ability_AA1_Hit_Ice,
		SFX_Barbarian_Ability_AA1_Hit_Nature = 3960,
		SFX_Barbarian_Ability_AA1_Hit_Void,
		SFX_Barbarian_Ability_AA2_Hit_Physical,
		SFX_Barbarian_Ability_AA2_Hit_Arcane,
		SFX_Barbarian_Ability_AA2_Hit_Fire,
		SFX_Barbarian_Ability_AA2_Hit_Ice = 3965,
		SFX_Barbarian_Ability_AA2_Hit_Nature,
		SFX_Barbarian_Ability_AA2_Hit_Void,
		SFX_Barbarian_Ability_AA3_Hit_Physical,
		SFX_Barbarian_Ability_AA3_Hit_Arcane,
		SFX_Barbarian_Ability_AA3_Hit_Fire = 3970,
		SFX_Barbarian_Ability_AA3_Hit_Ice,
		SFX_Barbarian_Ability_AA3_Hit_Nature,
		SFX_Barbarian_Ability_AA3_Hit_Void,
		SFX_Barbarian_Ability_AA1_Cast_Arcane,
		SFX_Barbarian_Ability_AA1_Cast_Fire = 3975,
		SFX_Barbarian_Ability_AA1_Cast_Ice,
		SFX_Barbarian_Ability_AA1_Cast_Nature,
		SFX_Barbarian_Ability_AA1_Cast_Void,

		// Barbarian Female Misc
		SFX_Barbarian_Female_Inventory_Helmet = 5000,
		SFX_Barbarian_Female_Inventory_Hands,
		SFX_Barbarian_Female_Inventory_Torso,
		SFX_Barbarian_Female_Inventory_LowerBody,
		SFX_Barbarian_Female_Inventory_Weapon,
		SFX_Barbarian_Misc_Female_CharSelect_Flourish1,
		SFX_Barbarian_Misc_Female_CharSelect_Idle1,
		SFX_Barbarian_Misc_Female_CharSelect_Idle2,
		SFX_Barbarian_Misc_Female_CharSelect_Idle3,
		SFX_Barbarian_Female_Interact_LeverFloorPull,
		SFX_Barbarian_Female_Interact_LeverFloorPush,
		SFX_Barbarian_Female_Interact_LeverWallPull,
		SFX_Barbarian_Female_Interact_LeverWallPush,
		SFX_Barbarian_Female_Interact_ChestOpen,
		SFX_Barbarian_Female_Death,
		SFX_Barbarian_Female_Revive,
		SFX_Barbarian_Female_Dungeon_Start,
		SFX_Barbarian_Female_Dungeon_End,
		SFX_Barbarian_Female_Idle_Flourish01,

		// Barbarian Male Misc
		SFX_Barbarian_Male_Inventory_Helmet = 5500,
		SFX_Barbarian_Male_Inventory_Hands,
		SFX_Barbarian_Male_Inventory_Torso,
		SFX_Barbarian_Male_Inventory_LowerBody,
		SFX_Barbarian_Male_Inventory_Weapon,
		SFX_Barbarian_Misc_Male_CharSelect_Flourish1,
		SFX_Barbarian_Misc_Male_CharSelect_Idle1,
		SFX_Barbarian_Misc_Male_CharSelect_Idle2,
		SFX_Barbarian_Misc_Male_CharSelect_Idle3,
		SFX_Barbarian_Male_Interact_LeverFloorPull,
		SFX_Barbarian_Male_Interact_LeverFloorPush,
		SFX_Barbarian_Male_Interact_LeverWallPull,
		SFX_Barbarian_Male_Interact_LeverWallPush,
		SFX_Barbarian_Male_Interact_ChestOpen,
		SFX_Barbarian_Male_Death,
		SFX_Barbarian_Male_Revive,
		SFX_Barbarian_Male_Dungeon_Start,
		SFX_Barbarian_Male_Dungeon_End,
		SFX_Barbarian_Male_Idle_Flourish01,

		// Sorcerer Female Misc
		SFX_Sorcerer_Female_Inventory_Helmet = 6000,
		SFX_Sorcerer_Female_Inventory_Hands,
		SFX_Sorcerer_Female_Inventory_Torso,
		SFX_Sorcerer_Female_Inventory_LowerBody,
		SFX_Sorcerer_Female_Inventory_Weapon,
		SFX_Sorcerer_Female_Interact_LeverFloorPull,
		SFX_Sorcerer_Female_Interact_LeverFloorPush,
		SFX_Sorcerer_Female_Interact_LeverWallPull,
		SFX_Sorcerer_Female_Interact_LeverWallPush,
		SFX_Sorcerer_Female_Interact_ChestOpen,
		SFX_Sorcerer_Female_Death,
		SFX_Sorcerer_Female_Revive,
		SFX_Sorcerer_Misc_Female_Inventory_Flourish1,
		SFX_Sorcerer_Misc_Female_Inventory_Idle1,
		SFX_Sorcerer_Misc_Female_Inventory_Idle2,
		SFX_Sorcerer_Misc_Female_Inventory_Idle3,
		SFX_Sorcerer_Female_Dungeon_Start,
		SFX_Sorcerer_Female_Dungeon_End,
		SFX_Sorcerer_Female_Idle_Flourish01,


		// Sorcerer Male Misc
		SFX_Sorcerer_Male_Inventory_Helmet = 6500,
		SFX_Sorcerer_Male_Inventory_Hands,
		SFX_Sorcerer_Male_Inventory_Torso,
		SFX_Sorcerer_Male_Inventory_LowerBody,
		SFX_Sorcerer_Male_Inventory_Weapon,
		SFX_Sorcerer_Male_Interact_LeverFloorPull,
		SFX_Sorcerer_Male_Interact_LeverFloorPush,
		SFX_Sorcerer_Male_Interact_LeverWallPull,
		SFX_Sorcerer_Male_Interact_LeverWallPush,
		SFX_Sorcerer_Male_Interact_ChestOpen,
		SFX_Sorcerer_Male_Death,
		SFX_Sorcerer_Male_Revive,
		SFX_Sorcerer_Misc_Male_Inventory_Flourish1,
		SFX_Sorcerer_Misc_Male_Inventory_Idle1,
		SFX_Sorcerer_Misc_Male_Inventory_Idle2,
		SFX_Sorcerer_Misc_Male_Inventory_Idle3,
		SFX_Sorcerer_Male_Dungeon_Start,
		SFX_Sorcerer_Male_Dungeon_End,
		SFX_Sorcerer_Male_Idle_Flourish01,

		// Sorcerer Abilities Continued 9000 - 10000

		// Barbarian Abilities Continued 10000 - 11000

		// Enemy SFX 13000 - 19999

		// Underdwellers || Geckle

		// Champion
		SFX_Monster_Geckle_Champion_AA1_Cast_Generic = 13000,
		SFX_Monster_Geckle_Champion_AA2_Cast_Generic,
		SFX_Monster_Geckle_Champion_Death,
		SFX_Monster_Geckle_Champion_HitReact,
		SFX_Monster_Geckle_Champion_AA1_Hit_Generic,
		SFX_Monster_Geckle_Champion_AA2_Hit_Generic,
		SFX_Monster_Geckle_Champion_Idle,
		SFX_Monster_Geckle_Champion_Run,

		// Minion
		SFX_Monster_Geckle_Minion_AA1_Cast_Generic = 13010,
		SFX_Monster_Geckle_Minion_AA2_Cast_Generic,
		SFX_Monster_Geckle_Minion_Death,
		SFX_Monster_Geckle_Minion_Ability_Transform_Cast,
		SFX_Monster_Geckle_Minion_Ability_Enrage_Cast,
		SFX_Monster_Geckle_Minion_HitReact,
		SFX_Monster_Geckle_Minion_Idle,
		SFX_Monster_Geckle_Minion_Run,
		SFX_Monster_Geckle_Minion_AA1_Hit_Generic,
		SFX_Monster_Geckle_Minion_AA2_Hit_Generic,

		// Spitter
		SFX_Monster_Geckle_Ranged_Death = 13020,
		SFX_Monster_Geckle_Ranged_AA1_Cast_Nature,
		SFX_Monster_Geckle_Ranged_Ability_Spit_Cast,
		SFX_Monster_Geckle_Ranged_Ability_Spit_Loop,
		SFX_Monster_Geckle_Ranged_HitReact,
		SFX_Monster_Geckle_Ranged_AA1_Hit_Nature,
		SFX_Monster_Geckle_Ranged_AA1_Travel_Nature,
		SFX_Monster_Geckle_Ranged_Idle,
		SFX_Monster_Geckle_Ranged_Run,

		// Basic Gnolls

		// Basic Gnoll Champion
		// More SFX for Basic Gnoll Champion below. "Basic Gnoll Champion Cont."
		SFX_Monster_Gnoll_Champion_AA1_Cast_Generic = 13030,
		SFX_Monster_Gnoll_Champion_AA2_Cast_Generic,
		SFX_Monster_Gnoll_Champion_Ability_GroundSlam_Fire_Cast,
		SFX_Monster_Gnoll_Champion_Death,
		SFX_Monster_Gnoll_Champion_HitReact,
		SFX_Monster_Gnoll_Champion_IdleRoar,
		SFX_Monster_Gnoll_Champion_Idle1,
		SFX_Monster_Gnoll_Champion_Idle2,
		SFX_Monster_Gnoll_Champion_Ability_GroundSlam_Ice_Cast,
		SFX_Monster_Gnoll_Champion_Run,

		// Basic Gnoll Melee
		SFX_Monster_Gnoll_Melee_AA1_Cast_Generic = 13040,
		SFX_Monster_Gnoll_Melee_AA2_Cast_Generic,
		SFX_Monster_Gnoll_Melee_AA1_Hit_Generic,
		SFX_Monster_Gnoll_Melee_AA2_Hit_Generic,
		SFX_Monster_Gnoll_Melee_Death,
		SFX_Monster_Gnoll_Melee_HitReact,
		SFX_Monster_Gnoll_Melee_Idle,
		SFX_Monster_Gnoll_Melee_Run,
		SFX_Monster_Gnoll_Melee_Walk,
		
		// Basic Gnoll Ranged
		//Additional SFX below "Basic Gnoll Ranged Cont."
		SFX_Monster_Gnoll_Ranged_AA1_Cast_Generic = 13050,
		SFX_Monster_Gnoll_Ranged_Ability_Blink_Cast,
		SFX_Monster_Gnoll_Ranged_Death,
		SFX_Monster_Gnoll_Ranged_HitReact,
		SFX_Monster_Gnoll_Ranged_AA1_Hit_Generic,
		SFX_Monster_Gnoll_Ranged_AA1_Travel_Generic,
		SFX_Monster_Gnoll_Ranged_Idle1,
		SFX_Monster_Gnoll_Ranged_Idle2,
		SFX_Monster_Gnoll_Ranged_AA1_Cast_Corrupt,
		SFX_Monster_Gnoll_Ranged_Run,

		// Basic Gnoll Minion
		SFX_Monster_Gnoll_Minion_AA1_Cast_Generic = 13060,
		SFX_Monster_Gnoll_Minion_Idle,
		SFX_Monster_Gnoll_Minion_Death,
		SFX_Monster_Gnoll_Minion_HitReact,
		SFX_Monster_Gnoll_Minion_Run,
		SFX_Monster_Gnoll_Minion_AA1_Hit_Generic,

		// MiniDruids (deprecated - use Bushling SFX)

		// Ranged
		SFX_MiniDruid_Ranged_AutoAttack_Fire = 13070,
		SFX_MiniDruid_Ranged_AutoAttack_Travel,
		SFX_MiniDruid_Ranged_AutoAttack_Hit,
		SFX_MiniDruid_Ranged_HitReact,
		SFX_MiniDruid_Ranged_Death,

		// Fire Gnolls

		// Fire Gnoll Champion
		SFX_Monster_Gnoll_Gnolliath_Fire_Attack1 = 13080,
		SFX_Monster_Gnoll_Gnolliath_Fire_Attack2,
		SFX_Monster_Gnoll_Gnolliath_Fire_GroundSlam,
		SFX_Monster_Gnoll_Gnolliath_Fire_Death,
		SFX_Monster_Gnoll_Gnolliath_Fire_Idle1,
		SFX_Monster_Gnoll_Gnolliath_Fire_Idle2,
		SFX_Monster_Gnoll_Gnolliath_Fire_Roar,
		SFX_Monster_Gnoll_Gnolliath_Fire_Stun,

		// Fire Gnoll Melee (Charger)
		SFX_Monster_Gnoll_Charger_Fire_Attack1 = 13090,
		SFX_Monster_Gnoll_Charger_Fire_Attack2,
		SFX_Monster_Gnoll_Charger_Fire_Death,
		SFX_Monster_Gnoll_Charger_Fire_Idle1,
		
		// Fire Gnoll Ranged
		SFX_Monster_Gnoll_Mystic_Fire_AAHit = 13100,
		SFX_Monster_Gnoll_Mystic_Fire_AATravel,
		SFX_Monster_Gnoll_Mystic_Fire_Attack1,
		SFX_Monster_Gnoll_Mystic_Fire_Death,
		SFX_Monster_Gnoll_Mystic_Fire_Blink,
		SFX_Monster_Gnoll_Mystic_Fire_Idle1,
		SFX_Monster_Gnoll_Mystic_Fire_Idle2,
		SFX_Monster_Gnoll_Mystic_Fire_Stun,

		// Fire Gnoll Minion
		SFX_Monster_Gnoll_GryphonDog_Fire_Attack1 = 13110,
		SFX_Monster_Gnoll_GryphonDog_Fire_Death,
		SFX_Monster_Gnoll_GryphonDog_Fire_Idle1,
		SFX_Monster_Gnoll_GryphonDog_Fire_Growl,

		// Nature Geckle Melee
		SFX_Monster_Geckle_Melee_AA1_Cast_Generic = 13120,
		SFX_Monster_Geckle_Melee_AA2_Cast_Generic,
		SFX_Monster_Geckle_Melee_Death,
		SFX_Monster_Geckle_Melee_Idle,
		SFX_Monster_Geckle_Melee_HitReact,
		SFX_Monster_Geckle_Melee_Run,
		SFX_Monster_Geckle_Melee_AA1_Hit_Generic,
		SFX_Monster_Geckle_Melee_AA2_Hit_Generic,

		// Arcane Bushling Scary Beast
		SFX_Monster_Bushling_Champion_AA1_Cast_Generic = 13130,
		SFX_Monster_Bushling_Champion_AA2_Cast_Generic,
		SFX_Monster_Bushling_Champion_Death,
		SFX_Monster_Bushling_Champion_Idle,
		SFX_Monster_Bushling_Champion_Run,
		SFX_Monster_Bushling_Champion_RunToIdle,
		SFX_Monster_Bushling_Champion_HitReact,
		SFX_Monster_Bushling_Champion_AA1_Hit_Generic,
		SFX_Monster_Bushling_Champion_AA2_Hit_Generic,

		// Arcane Bushling Bomber (minion)
		SFX_Monster_Bushling_Bomber_Arcane_Idle = 13140,
		SFX_Monster_Bushling_Bomber_Ability_Suicide,
		SFX_Monster_Bushling_Bomber_Arcane_Run,
		SFX_Monster_Bushling_Bomber_Arcane_RunSuicide,
		SFX_Monster_Bushling_Bomber_Arcane_Death,
		SFX_Monster_Bushling_Bomber_Arcane_SpawnSkaari,
		SFX_Monster_Bushling_Bomber_HitReact,

		// Arcane Bushling Ranged
		// More SFX for Arcane Bushling Ranged below. "Arcane Bushling Ranged Cont."  
		SFX_Monster_Bushling_Ranged_Idle = 13150,
		SFX_Monster_Bushling_Ranged_Run,
		SFX_Monster_Bushling_Ranged_AA1_Cast_Arcane,
		SFX_Monster_Bushling_Ranged_AA2_Cast_Arcane,
		SFX_Monster_Bushling_Ranged_Death,
		SFX_Monster_Bushling_Ranged_ScarySpawn,
		SFX_Monster_Bushling_Ranged_HitReact,
		SFX_Monster_Bushling_Ranged_AA1_Travel_Arcane,
		SFX_Monster_Bushling_Ranged_AA2_Travel_Arcane,
		SFX_Monster_Bushling_Ranged_AA1_Hit_Arcane,

		// Arcane Bushling Totem Dropper
		SFX_Monster_Bushling_TotemDropper_Arcane_Idle1 = 13160,
		SFX_Monster_Bushling_TotemDropper_Arcane_Jog,
		SFX_Monster_Bushling_TotemDropper_Arcane_AttackDropHazard,
		SFX_Monster_Bushling_TotemDropper_Arcane_Death,
		SFX_Monster_Bushling_TotemDropper_Arcane_SpawnSkaari,

		// Arcane Bushling Melee
		SFX_Monster_Bushling_Melee_Idle = 13170,
		SFX_Monster_Bushling_Melee_Run,
		SFX_Monster_Bushling_Melee_AA1_Cast_Generic,
		SFX_Monster_Bushling_Melee_AA2_Cast_Generic,
		SFX_Monster_Bushling_Melee_Death,
		SFX_Monster_Bushling_Melee_ScarySpawn,
		SFX_Monster_Bushling_Melee_HitReact,
		SFX_Monster_Bushling_Melee_AA1_Hit_Generic,
		SFX_Monster_Bushling_Melee_AA2_Hit_Generic,

		// Void Salamander Champion
		// More SFX for Void Salamander Champion below. "Void Salamander Champion Cont."
		SFX_Monster_Salamander_Champion_Idle = 13180,
		SFX_Monster_Salamander_Champion_Run,
		SFX_Monster_Salamander_Champion_AA1_Cast_Generic,
		SFX_Monster_Salamander_Champion_AA2_Cast_Generic,
		SFX_Monster_Salamander_Champion_Ability_Shockwave_Cast,
		SFX_Monster_Salamander_Champion_Death,
		SFX_Monster_Salamander_Champion_HitReact,
		SFX_Monster_Salamander_Champion_AA1_Hit_Generic,
		SFX_Monster_Salamander_Champion_AA2_Hit_Generic,
		SFX_Monster_Salamander_Champion_Ability_Shockwave_Travel,

		// Void Salamander Minion
		SFX_Monster_Salamander_Minion_Idle = 13190,
		SFX_Monster_Salamander_Minion_Run,
		SFX_Monster_Salamander_Minion_AA1_Cast_Generic,
		SFX_Monster_Salamander_Minion_AA2_Cast_Generic,
		SFX_Monster_Salamander_Minion_Death,
		SFX_Monster_Salamander_Minion_HitReact,
		SFX_Monster_Salamander_Minion_AA1_Hit_Generic,
		SFX_Monster_Salamander_Minion_AA2_Hit_Generic,

		// Void Salamander Ranged
		SFX_Monster_Salamander_Ranged_Idle = 13200,
		SFX_Monster_Salamander_Ranged_Run,
		SFX_Monster_Salamander_Ranged_AA1_Travel_Void,
		SFX_Monster_Salamander_Ranged_Ability_VoidShock_Cast,
		SFX_Monster_Salamander_Ranged_Death,
		SFX_Monster_Salamander_Ranged_HitReact,
		SFX_Monster_Salamander_Ranged_AA1_Cast_Void,
		SFX_Monster_Salamander_Ranged_AA1_Hit_Void,
		SFX_Monster_Salamander_Ranged_Ability_VoidShock_Hit,

		// Void Salamander Melee
		SFX_Monster_Salamander_Melee_Idle = 13210,
		SFX_Monster_Salamander_Melee_Run,
		SFX_Monster_Salamander_Melee_AA1_Cast_Generic,
		SFX_Monster_Salamander_Melee_AA2_Cast_Generic,
		SFX_Monster_Salamander_Melee_Death,
		SFX_Monster_Salamander_Melee_HitReact,
		SFX_Monster_Salamander_Melee_AA1_Hit_Generic,
		SFX_Monster_Salamander_Melee_AA2_Hit_Generic,

		// Ice Elemental
		SFX_Monster_Elemental_Ice_Idle = 13220,
		SFX_Monster_Elemental_Ice_Walk,
		SFX_Monster_Elemental_Ice_AA1_Cast_Generic,
		SFX_Monster_Elemental_Ice_AA2_Cast_Generic,
		SFX_Monster_Elemental_Ice_Ability_IceBreath_Cast,
		SFX_Monster_Elemental_Ice_Death,
		SFX_Monster_Elemental_Ice_HitReact,
		SFX_Monster_Elemental_Ice_AA1_Hit_Generic,
		SFX_Monster_Elemental_Ice_AA2_Hit_Generic,
		SFX_Monster_Elemental_Ice_Ability_IceBreath_Hit,

		// Void Elemental
		SFX_Monster_Elemental_Void_Idle = 13230,
		SFX_Monster_Elemental_Void_Run,
		SFX_Monster_Elemental_Void_AA1_Cast_Generic,
		SFX_Monster_Elemental_Void_Ability_EyeLaser_Cast,
		SFX_Monster_Elemental_Void_Ability_GroundPlop_Cast,
		SFX_Monster_Elemental_Void_Death,
		SFX_Monster_Elemental_Void_HitReact,
		SFX_Monster_Elemental_Void_AA1_Hit_Generic,
		SFX_Monster_Elemental_Void_Ability_EyeLaser_Hit,
		SFX_Monster_Elemental_Void_Ability_GroundPlop_Loop,

		// Stone Golem
		SFX_Monster_Beast_StoneGolem_Idle = 13240,
		SFX_Monster_Beast_StoneGolem_Run,
		SFX_Monster_Beast_StoneGolem_AA1_Cast_Generic,
		SFX_Monster_Beast_StoneGolem_AA2_Cast_Generic,
		SFX_Monster_Beast_StoneGolem_Death,
		SFX_Monster_Beast_StoneGolem_SpawnLong,
		SFX_Monster_Beast_StoneGolem_SpawnShort,
		SFX_Monster_Beast_StoneGolem_HitReact,
		SFX_Monster_Beast_StoneGolem_AA1_Hit_Generic,
		SFX_Monster_Beast_StoneGolem_AA2_Hit_Generic,

		// Harmadillo
		SFX_Monster_Beast_Harmadillo_Idle = 13250,
		SFX_Monster_Beast_Harmadillo_Run,
		SFX_Monster_Beast_Harmadillo_AA1_Cast_Generic,
		SFX_Monster_Beast_Harmadillo_AA2_Cast_Generic,
		SFX_Monster_Misc_Harmadillo_Death,
		SFX_Monster_Beast_Harmadillo_HitReact,
		SFX_Monster_Beast_Harmadillo_AA1_Hit_Generic,
		SFX_Monster_Beast_Harmadillo_AA2_Hit_Generic,

		// Basic Gnoll Ranged Cont.
		SFX_Monster_Gnoll_Ranged_AA1_Travel_Corrupt = 13260,
		SFX_Monster_Gnoll_Ranged_AA1_Hit_Corrupt,
		SFX_Monster_Gnoll_Ranged_AA1_Cast_Fire,
		SFX_Monster_Gnoll_Ranged_AA1_Travel_Fire,
		SFX_Monster_Gnoll_Ranged_AA1_Hit_Fire,
		SFX_Monster_Gnoll_Ranged_AA1_Cast_Ice,
		SFX_Monster_Gnoll_Ranged_AA1_Travel_Ice,
		SFX_Monster_Gnoll_Ranged_AA1_Hit_Ice,

		// Basic Gnoll Champion Cont.
		SFX_Monster_Gnoll_Champion_Ability_GroundSlam_Corrupt_Cast = 13270,
		SFX_Monster_Gnoll_Champion_AA1_Hit_Generic,
		SFX_Monster_Gnoll_Champion_AA2_Hit_Generic,
		SFX_Monster_Gnoll_Champion_Ability_GroundSlam_Fire_Blast,
		SFX_Monster_Gnoll_Champion_Ability_GroundSlam_Ice_Blast,
		SFX_Monster_Gnoll_Champion_Ability_GroundSlam_Corrupt_Blast,
				
		// Arcane Bushling Ranged
		SFX_Monster_Bushling_Ranged_AA2_Hit_Arcane = 13280,
		
		// Void Salamander Champion Cont.
		SFX_Monster_Salamander_Champion_Ability_Shockwave_Hit = 13290,
		
		// Spawn
		SFX_Underdweller_Spawn_Water_Small = 18000,
		SFX_Underdweller_Spawn_Water_Large,
		
		// Ambience 20000 - 20999
		Ambience_Level_GenericIndoors = 20000,
		Ambience_Level_GenericOutdoors,
		Ambience_Level_Badlands,
		Ambience_Level_Cave,
		Ambience_Level_Forest,
		Ambience_Level_Grasslands,
		Ambience_Level_Swamp,
		Ambience_Level_Seashore,
		Ambience_Level_Mountain,	
		Ambience_Level_Camp,
		Ambience_Level_Crypt,
		Ambience_Level_ForestTown,
		Ambience_Level_Docks,
		
		// Ambience Overworld 21000 - 21099
		Ambience_OW_Badlands = 21000,
		Ambience_OW_Cave,
		Ambience_OW_Forest,
		Ambience_OW_Grasslands,
		Ambience_OW_Swamp,
		Ambience_OW_Seashore,
		Ambience_OW_Mountain,
		Ambience_OW_Town,
		Ambience_OW_Town_Battle,

		// Miscellaneous, loot drop and pick up 22000 - 30000
		SFX_Loot_Gold_Dropped = 22000,
		SFX_Loot_Gold_PickedUp,
		SFX_Health_Orb_Dropped,         // unused
		SFX_Health_Orb_PickedUp,
		SFX_Health_Orb_Loop,            // unused
		SFX_Mana_Orb_PickedUp,
		SFX_Mana_Orb_Dropped,           // unused
		SFX_Mana_Orb_Loop,              // unused
		SFX_Health_Potion_Used,
		SFX_DamageEmitter_Gas,
		SFX_DamageEmitter_Fire,
		SFX_Cave_RotatingStatue,		// do not move
		SFX_Loot_Collect,
		SFX_Loot_Chainmail,
		SFX_Loot_Cloth,
		SFX_Loot_Leather,
		SFX_Misc_BushRustling,			// do not move
		SFX_Loot_WeaponMetalDull,
		SFX_Loot_MetalHelm,
		SFX_Loot_WeaponMetalSharp,
		SFX_Loot_Scepter,
		SFX_Loot_WeaponWood,
				
		// UI 30000 - 30999
		SFX_UI_ButtonClick = 30000,		
		SFX_UI_Notification,
		SFX_UI_PanelSlideIn,
		SFX_UI_PanelSlideOut,
		SFX_UI_LevelUp,
		SFX_UI_QuestComplete,
		SFX_UI_DungeonWin,
		SFX_UI_Revive,
		SFX_UI_Inventory_Leather,
		SFX_UI_Inventory_Cloth,
		SFX_UI_Inventory_MetalHelm,
		SFX_UI_Inventory_Chainmail,
		SFX_UI_Inventory_WeaponMetalDull,
		SFX_UI_Inventory_Select,
		UI_CharacterCreate_Barbarian,
		UI_CharacterCreate_Sorcerer,
		SFX_UI_Inventory_Trash,
		SFX_UI_Countdown_XP,
		SFX_UI_Countdown_Gold,
		SFX_UI_Inventory_Deselect,
		SFX_UI_ButtonClickBack,
		SFX_UI_ButtonClickSelect,
		SFX_UI_ButtonClickClose,
		SFX_UI_ChatPanelOpen,
		SFX_UI_ChatEnter,
		SFX_UI_ChatReceived,
		SFX_UI_Inventory_TrashItem,
		SFX_UI_Inventory_Barbarian_WeaponWood,
		SFX_UI_Inventory_Barbarian_WeaponMetalDull,
		SFX_UI_Inventory_Barbarian_WeaponMetalSharp,
		SFX_UI_Inventory_Sorcerer_Scepter,
		SFX_UI_Inventory_Sorcerer_Staff,
		SFX_UI_Inventory_Armor_Cloth,
		SFX_UI_Inventory_Armor_Leather,
		SFX_UI_Inventory_Armor_Chainmail,
		SFX_UI_Inventory_Armor_MetalHelm,
		SFX_UI_Inventory_Collection,
		SFX_UI_Dialogue_Open,
		SFX_UI_Dialogue_Close,
		SFX_UI_EnergyFull,
		SFX_UI_CarouselMovement,
		SFX_UI_Pix_IconClick,
		SFX_UI_Pix_SparkleButton,
		SFX_UI_Inventory_SellItem,
		SFX_UI_CameraButton,

		// Interact 31000 - 31999
		// Note: Gender/class specific interacts are in player section
		SFX_Interact_ChestOpen = 31000,
		SFX_Interact_Switch,
		SFX_Interact_FloorFenceDoor_Open,
		SFX_Interact_PropDestroy_Clay,
		SFX_Interact_PropDestroy_Stone,
		SFX_Interact_PropDestroy_Wood,
		SFX_Interact_FloorPressurePlate_On,
		SFX_Interact_FloorPressurePlate_Off,
		SFX_Interact_PushPillarLoop,
		SFX_Interact_CryptDoor_Open,
		SFX_Interact_CryptDoor_Close,
		SFX_Interact_PushPillar,
		SFX_Interact_LevelExit,
		SFX_Stinger_Win,
		SFX_Stinger_Lose,
		SFX_Interact_Spirit_RelicOpen_Fast,
		SFX_Interact_Spirit_RelicDrop,
		SFX_Interact_KeySpawning,
		SFX_Interact_Door_Open,
		SFX_Interact_WoodenGate,
		SFX_Interact_Teleport,

		// Quest 32000 - 32999
		Misc_QuestCompleteRewards = 32000,
		Misc_QuestObjectiveComplete,

		// Footsteps 33000 - 33499
		Footstep_Warrior_Bush,
		Footstep_Warrior_Dirt,
		Footstep_Warrior_Grass,
		Footstep_Warrior_Puddle,
		Footstep_Warrior_Sand,
		Footstep_Warrior_StoneBridge,
		Footstep_Warrior_StoneTile,
		Footstep_Warrior_Wood,
		
		// Emitters 33500 43999
		SFX_Misc_Emitters_Campfire,
		SFX_Misc_Emitters_StreamLarge,
		SFX_Misc_Emitters_StreamSmall,
		SFX_Misc_Emitters_Torch,
		SFX_Misc_Emitters_TorchChandelier,
		SFX_Misc_Emitters_WaterfallLarge,
		SFX_Misc_Emitters_WaterfallMedium,
		SFX_Misc_Emitters_WaterfallSmall,
		SFX_Misc_Emitters_WoodBridge,		
		SFX_Misc_Emitters_Candle,
		SFX_Misc_Emitters_CandleChandelier,
		SFX_Misc_Emitters_Brazier,
		SFX_Misc_Emitters_Fountain,

		// Slow relic open, treasure chest relic drop, relic show UI, spirit management UI
		// 44000 - 44099
		SFX_Interact_Spirit_RelicOpen_Common = 44000,
		SFX_Interact_Spirit_RelicOpen_Uncommon,
		SFX_Interact_Spirit_RelicOpen_Rare,
		SFX_Loot_RelicDrop1,
		SFX_Loot_RelicDrop2,
		SFX_Loot_RelicDrop3,
		SFX_Loot_RelicDrop4,
		SFX_Loot_RelicDrop5,
		SFX_Interact_CelestialEnter,
		SFX_Interact_CelestialExit,
		SFX_UI_RelicPurchase,
		SFX_Interact_Spirit_MachineSmall,
		SFX_Interact_Spirit_MachineBig,
		SFX_Interact_Spirit_Evolve,
		SFX_Interact_Spirit_PowerUp,
		SFX_UI_Inventory_Spirit_Drop_Fire,
		SFX_UI_Inventory_Spirit_Drop_Ice,
		SFX_UI_Inventory_Spirit_Drop_Arcane,
		SFX_UI_Inventory_Spirit_Drop_Void,
		SFX_UI_Inventory_Spirit_Drop_Nature,
		SFX_UI_Inventory_Spirit_Drop_Neutral,
		SFX_UI_Purchase,
		SFX_UI_Inventory_Spirit_Drop_FireLayer1,
		SFX_UI_Inventory_Spirit_Drop_IceLayer1,
		SFX_UI_Inventory_Spirit_Drop_ArcaneLayer1,
		SFX_UI_Inventory_Spirit_Drop_VoidLayer1,
		SFX_UI_Inventory_Spirit_Drop_NatureLayer1,
		SFX_UI_Inventory_Spirit_Drop_NeutralLayer1,

		// Victory Screen sounds
		// 44100 - 44104
		SFX_VictoryScreen_Enter = 44100,
		SFX_VictoryScreen_Spirit,
		SFX_VictoryScreen_Equipment,
		SFX_VictoryScreen_Slide,
		SFX_VictoryScreen_StaminaBar,
		/// DO NOT ADD MORE HERE before SFX_LevelStart_IconColorSplash
		/// DO NOT ADD MORE HERE before SFX_LevelStart_IconColorSplash  

		// Level Start sounds
		// 44105 - 44199
		SFX_LevelStart_IconColorSplash = 44105,
		SFX_LevelStart_IconSwish,

		// Dialogue conversation sounds
		// 44200 - 44299
		DLG_common = 44200,
	}
	
	public enum eDungeonMusicLevel
	{
		Unknown = -1,
		Low,
		Medium,
		High
	}

	public enum eGroundType
	{
		Untagged = -1,
		Bush,
		Dirt,
		Grass,
		Puddle,
		Sand,
		StoneBridge,
		StoneTile,
		Wood
	}

	// Returns true if the event was posted successfully
	public static bool PostEvent(eEvent evt)
	{
		return PostEvent(evt, true);
	}

	// Returns true if the event was posted successfully
	public static bool PostEvent(eEvent evt, bool play)
	{
		if (evt == eEvent.None)
		{
			return false;
		}

		if (EventManagerEx.Instance == null)
		{
			return false;
		}

		//DebugSystem.LogError("Posting audio event " + EventString(evt) + " " + (play? "True" : "False"));
		return EventManagerEx.Instance.PostEvent(EventString(evt), play ? EventAction.PlaySound : EventAction.StopSound);
	}

	// Returns true if the event was posted successfully
	public static bool PostEvent(eEvent evt, GameObject parentGameObject, bool play)
	{
		if (evt == eEvent.None)
		{
			return false;
		}

		if (EventManagerEx.Instance == null)
		{
			return false;
		}

		//Debug.LogError("Posting audio event " + EventString(evt) + " with game object " + parentGameObject);
		return EventManagerEx.Instance.PostEvent(EventString(evt), play ? EventAction.PlaySound : EventAction.StopSound, null, parentGameObject);
	}

	// Returns true if the event was posted successfully
	public static bool PostEvent(string eventName)
	{
	    //Debug.LogError("Post event : " + eventName);
        return PostEvent(eventName, true);
	}

	// Returns true if the event was posted successfully
	public static bool PostEvent(string eventName, bool play)
	{
        //Debug.Log("Post event : "+ eventName + "play : "+ play.ToString());

		if (string.IsNullOrEmpty(eventName))
		{
			return false;
		}

		// Convert event string from c-sharp format to Fabric format. (Ambient_Cave_Start -> Ambient/Cave/Start)
		eventName = eventName.Replace("_", "/");
		if (string.IsNullOrEmpty(eventName))
		{
			return false;
		}

		if (EventManagerEx.Instance == null)
		{
			return false;
		}

#if UNITY_ANDROID
	    //Debug.LogWarning("Posting audio event " + eventName + " " + (play ? "True" : "False"));  
#endif
        return EventManagerEx.Instance.PostEvent(eventName, play ? EventAction.PlaySound : EventAction.StopSound);
	}

    public static bool isPlayingAudioClip = false;
    public static Dictionary<string, AudioSource> PreviewSources = new Dictionary<string, AudioSource>();
    public static Dictionary<string, GameObject> AudioPreViewers = new Dictionary<string, GameObject>();
    public static void PlayEventAudioSource(string eventName,float volumn ,bool play)
    {
        GameObject AudioPreViewer = null;
        AudioSource AudioSourcePreViewer = null;
        String name = eventName.Replace("/", "_");
        GameObject obj = GameObject.Find(name);
        if (obj == null)
        {
            return;
        }
        AudioComponent component = obj.GetComponent<AudioComponent>();
        if (PreviewSources.ContainsKey(name))
        {
            PreviewSources[name].Stop();
            PreviewSources[name].volume = (volumn + 100) / 100f;
            PreviewSources[name].Play();
            return;
        }
        if (AudioPreViewer == null)
        {
            AudioPreViewer = new GameObject();
            AudioPreViewer.name = "AudioPreviewer";
            AudioSourcePreViewer = AudioPreViewer.AddComponent<AudioSource>();
            AudioPreViewer.hideFlags = HideFlags.HideAndDontSave;
            AudioSourcePreViewer.volume = (component.Volume + 100)/100f;
        }

        AudioListener listener = GameObject.FindObjectOfType<AudioListener>();
        if (component._dynamicAsyncAudioClipLoading && component._audioClipHandle.IsAudioClipPathSet())
        {
            AudioSourcePreViewer.clip =
                (AudioClip) Resources.Load(component._audioClipHandle.GetAudioClipPath(), typeof(AudioClip));
        }
        else
        {
            AudioSourcePreViewer.clip = component.AudioClip;
        }
        PreviewSources.Add(name,AudioSourcePreViewer);
        AudioPreViewers.Add(name, AudioPreViewer);
        AudioSourcePreViewer.Play();
    }

    public static void ClearAudioPreviewers()
    {
        foreach (string name in AudioPreViewers.Keys)
        {
            GameObject.DestroyImmediate(AudioPreViewers[name]);

        }
        PreviewSources.Clear();
        AudioPreViewers.Clear();
    }

	// Returns true if the event was posted successfully
	public static bool PostEvent(string eventName, GameObject parentGameObject, bool play)
	{
		if (string.IsNullOrEmpty(eventName))
		{
			return false;
		}

		// Convert event string from c-sharp format to Fabric format. (Ambient_Cave_Start -> Ambient/Cave/Start)
		eventName = eventName.Replace("_", "/");
		if (string.IsNullOrEmpty(eventName))
		{
			return false;
		}

		if (EventManagerEx.Instance == null)
		{
			return false;
		}

		//EB.Debug.LogWarning(Time.timeSinceLevelLoad + ": FusionAudio.PostEvent: " + eventName + "  " + (play ? "Play" : "Stop") + " (parent GameObject: " + parentGameObject + ")");
		return EventManagerEx.Instance.PostEvent(eventName, play ? EventAction.PlaySound : EventAction.StopSound, null, parentGameObject);
	}

    /// <summary>
    /// �Ƿ��Ѽ�������������Ч
    /// </summary>
    public static bool GuideAudioClipHasLoad = true; 

    /// <summary>
    /// ���¼����Ѿ�ж�ص�����Ƶ��Դ
    /// </summary>
    public static void LoadGuideAudioClip()
    {
        //foreach (string audioName in AudioPathDic.Keys)
        //{
        //    string audioPath = AudioPathDic[audioName];
        //    AudioClip clip = Resources.Load<AudioClip>(audioPath);
        //    AudioComponent audioComponent = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioComponent>();
        //    audioComponent.SetAudioClip(clip, null);
        //    audioComponent.LoadAudio();
        //    audioComponent.OverrideParentVolume = true;
        //    audioComponent.SetVolume(0.5f);
        //}
        if (GuideAudioClipHasLoad) return;
        GuideAudioClipHasLoad = true;

        AudioComponent audio;
        EventListener listener;
        GameObject[] audioGos = GameObject.FindGameObjectsWithTag("Audio");
        for (int i = 0; i < audioGos.Length; i++)
        {
            audio = audioGos[i].GetComponent<AudioComponent>();
            listener = audio.gameObject.GetComponent<EventListener>();
            if (audio.AudioClip == null)
            {
                string audioPath = AudioPathDic[listener._eventName];
                AudioClip clip = Resources.Load<AudioClip>(audioPath);
                audio.SetAudioClip(clip, null);
                audio.LoadAudio();
                audio.OverrideParentVolume = true;
                audio.SetVolume(0.5f);
            }
        }
    }

    public static Dictionary<string, string> AudioPathDic;

    /// <summary>
    /// ж�����д��˱�ǵ���Ƶ
    /// </summary>
    public static void ReleaseGuideAudioClips()
    {
        if (!GuideAudioClipHasLoad) return;
        GuideAudioClipHasLoad = false;

        if (AudioPathDic == null)
        {
            AudioPathDic = new Dictionary<string, string>();
        }
        GameObject[] audioGos = GameObject.FindGameObjectsWithTag("Audio");
        AudioComponent audio;
        EventListener listener;
        for (int i = 0; i < audioGos.Length; i++)
        {
            audio = audioGos[i].GetComponent<AudioComponent>();
            listener = audio.gameObject.GetComponent<EventListener>();
            if (audio.AudioClip)
            {
                string path = AUDIO_PATH + "/DLG/" + audio.AudioClip.name;
                //EB.Debug.Log(path);
                if (!AudioPathDic.ContainsKey(listener._eventName))
                {
                    AudioPathDic.Add(listener._eventName, path);
                }
                audio.AudioClip.UnloadAudioData();
                Resources.UnloadAsset(audio.AudioClip);
                audio.AudioClip = null;
            }
            else
            {
                EB.Debug.LogWarning("AudioComponent : {0} Clip is Empty", audio.name);
            }
        }
    }

    private const string AUDIO_PATH = "Audio/AudioSource";

    public enum AUDIO_TYPE
    {
        UI = 0,
        BGM = 1,
        GUIDE = 2,
        COMBAT = 3
    }

	// Get the string representation of an event
	public static string EventString(eEvent evt)
	{
		// eEvent.Ambient_Cave_Start -> "Ambient/Cave/Start"
		string src = System.Enum.GetName(typeof(eEvent), evt);
		if (string.IsNullOrEmpty(src))
		{
			//EB.Debug.LogError("FusionAudio could not find a match for: " + evt.ToString());
			return null;
		}

		string ret = src.Replace("_", "/");
		if (string.IsNullOrEmpty(ret))
		{
			//EB.Debug.LogError("FusionAudio slash replacement failed: " + src);
			return null;
		}

		return ret;
	}

	private void OnEnable()
	{
		RVOController.ZoneChangedEvent += OnZoneChangedEvent;
		RVOController.GroundTypeChangedEvent += OnGroundTypeChangedEvent;
		//EventManager.instance.AddListener<LevelStartEvent>(OnLevelStart);
		EventManager.instance.AddListener<TargetChangedEvent>(OnTargetChangedEvent);
		EventManager.instance.AddListener<SpawnerEnemiesAggroEvent>(OnAggroEvent);
#if DEBUG
		DebugSystem.RegisterSystem("Audio", this, "General", true);
#endif
	}

	private void OnDisable()
	{
		RVOController.ZoneChangedEvent -= OnZoneChangedEvent;
		RVOController.GroundTypeChangedEvent -= OnGroundTypeChangedEvent;
		//EventManager.instance.RemoveListener<LevelStartEvent>(OnLevelStart);
		EventManager.instance.RemoveListener<TargetChangedEvent>(OnTargetChangedEvent);
		EventManager.instance.RemoveListener<SpawnerEnemiesAggroEvent>(OnAggroEvent);
#if DEBUG
		DebugSystem.UnregisterSystem(this, true);
#endif
	}
	
	private void Awake()
	{
		DontDestroyOnLoad(this);
		AudioListener.volume = UserData.AudioEnabled ? 1.0f : 0;
	}
	
	public static void InitLocalPlayer()
	{
		_playerObject = PlayerManager.LocalPlayerGameObject();
		if (_playerObject != null)
		{
			_levelCamera = Camera.main;
			
			_localPlayerTargetingComponent = _playerObject.GetComponent<CharacterTargetingComponent>();
			_localPlayerCombatController = _playerObject.GetComponent<CombatController>();
			
			if (_localPlayerTargetingComponent == null || _localPlayerCombatController == null)
			{
				EB.Debug.LogWarning("FusionAudio could not obtain local player components");
				return;
			}
		}
		else
		{
			_levelCamera = null;
			_localPlayerTargetingComponent = null;
			_localPlayerCombatController = null;
		}
	}

	public static void InitCombat(GameObject localPlayer)
	{
		_levelCamera = Camera.main;
		_playerObject = localPlayer;
		_localPlayerTargetingComponent = null;
		_localPlayerCombatController = null;
	}

	private void Update()
	{
		UnityEngine.Profiling.Profiler.BeginSample("FusionAudio_AudioListener");
		if (_playerObject != null && _levelCamera != null)
		{
			gameObject.transform.position = _playerObject.transform.position;
			gameObject.transform.rotation = _levelCamera.transform.rotation;
		}
		UnityEngine.Profiling.Profiler.EndSample();
	}

	private void OnAudioHelperReloaded()
	{
		_currentEnvironmentType = EnvironmentHelper.eEnvironmentType.Unknown;
	}
    
	private void OnZoneChangedEvent(ZoneDescriptor zone)
	{
		EnvironmentHelper.eEnvironmentType environmentType = zone == null ? EnvironmentHelper.eEnvironmentType.Unknown : zone.environmentType;
		ZoneChanged(environmentType);

		if (zone != null && _currentEnvironmentType == EnvironmentHelper.eEnvironmentType.Unknown && string.IsNullOrEmpty(_lastAmbientEventNameRoot))
		{
			EnvironmentHelper environHelper = zone.GetComponentInParent<EnvironmentHelper>();
			if (environHelper != null && !string.IsNullOrEmpty(environHelper.DefaultAmbientLoopEvent))
			{
				PostAmbientSoundEvent(environHelper.AmbientLoopEvent, true);
			}
		}
	}

	private void ZoneChanged(EnvironmentHelper.eEnvironmentType environmentType)
	{
		if (_lastDungeonMusicLevel == eDungeonMusicLevel.Unknown)
		{
			ResetDungeonMusic();
			EB.Coroutines.Run(PostDungeonMusicSoundEvent(eDungeonMusicLevel.Low));
		}

		_lastZonePrefix = environmentType.ToString();
		if (_currentEnvironmentType == EnvironmentHelper.eEnvironmentType.Unknown && environmentType != EnvironmentHelper.eEnvironmentType.Unknown)
		{
			_currentEnvironmentType = environmentType;
			_lastChangeEnvironmentTimestamp = Time.time;

			// post initial event
			if (PostAmbientSoundEvent(GetAmbientEventName(_lastZonePrefix), true))
			{
				_lastAmbientEventNameRoot = _lastZonePrefix;
			}

			return;
		}

		if (environmentType != _currentEnvironmentType)
		{
			if (Time.time >= _changeEnvironmentThreshold + _lastChangeEnvironmentTimestamp)
			{
				_currentEnvironmentType = environmentType;
				_lastChangeEnvironmentTimestamp = Time.time;

				// stop previous ambient
				if (!string.IsNullOrEmpty(_lastAmbientEventNameRoot))
				{
					PostAmbientSoundEvent(GetAmbientEventName(_lastAmbientEventNameRoot), false);
					_lastAmbientEventNameRoot = string.Empty;
				}

				// start new ambient
				if (PostAmbientSoundEvent(GetAmbientEventName(_lastZonePrefix), true))
				{
					_lastAmbientEventNameRoot = _lastZonePrefix;
				}
			}
		}
	}
	
	private string GetAmbientEventName(string strEnvironment)
	{
		if (strEnvironment == "" || strEnvironment == "Unknown")
			return "";
		
		string overworldPrefix = "";
		//if (GameStateManager.Instance.State == eGameState.Dungeon)
		//{
		//	overworldPrefix = "Level_";
		//}
		//else if (GameStateManager.Instance.State == eGameState.Overworld)
		//{
		//	overworldPrefix = "OW_";
		//}
		//else
		{
			return "";
		}

		return "Ambience_" + overworldPrefix + strEnvironment;
	}
	
	public static IEnumerator PostDungeonMusicSoundEvent(eDungeonMusicLevel level = eDungeonMusicLevel.Low, bool play = true)
	{
		// same music escalation level, do nothing & return
		if (_lastDungeonMusicLevel == level)
			yield break;
			
		//if (GameStateManager.Instance.State != eGameState.Dungeon)
			yield break;
			
		// escalation level trumps, if level is high ignore medium 
		if (level == eDungeonMusicLevel.Medium && _lastDungeonMusicLevel == eDungeonMusicLevel.High)
			yield break;

		// make sure we have registered a valid environment type/zone prefix before continuing
		while (_lastZonePrefix == "")
		{
			yield return null;
		}

		// establish an event name from the last known zone prefix (if any)
		string evtName = GetDungeonMusicEventName(_lastZonePrefix, level);
		if (string.IsNullOrEmpty(evtName))
		{
			EB.Debug.LogWarning("PostMusicSoundEvent: could not resolve event name");
			yield break;
		}

		// stop previous music 
		if (!string.IsNullOrEmpty(_lastDungeonMusicEvent))
		{
			PostEvent(_lastDungeonMusicEvent, false);
		}

		PostEvent(evtName, play);

		// cache the event & play
		_lastDungeonMusicLevel = level;
		_lastDungeonMusicEvent = evtName;
		_lastMusicChangeTimestamp = Time.time;
	}	
	
	public static string GetDungeonMusicEventName(string prefix, eDungeonMusicLevel level)
	{
		if (string.IsNullOrEmpty(prefix))
		{
			EB.Debug.LogWarning("GetDungeonMusicName: invalid or missing prefix!");
			return null;
		}
		
		return "Music_Level_" + prefix + "_" + level.ToString();
	}
	
	private void OnAggroEvent(SpawnerEnemiesAggroEvent evt)
	{
		// if LastDungeonMusicLevel is set to "Hi", then it was done so explicitly via PlayMaker; ignore normal aggro rules
		if (_lastDungeonMusicLevel == eDungeonMusicLevel.High)
			return;

		if (evt.isAggroStarting == true)
		{
			if (_lastDungeonMusicLevel < eDungeonMusicLevel.Medium)
				EB.Coroutines.Run(PostDungeonMusicSoundEvent(eDungeonMusicLevel.Medium));
		}
		else
		{
			EB.Coroutines.Run(PostDungeonMusicSoundEvent(eDungeonMusicLevel.Low));
			PostStingerWin();
		}
	}
	
	private void OnTargetChangedEvent(TargetChangedEvent evt)
	{
		// if LastDungeonMusicLevel is set to "Hi", then it was done so explicitly via PlayMaker; ignore normal target/lack of target rules
		if (_lastDungeonMusicLevel == eDungeonMusicLevel.High)
			return;

		if (evt == null)
		{
			EB.Debug.LogWarning("OnTargetChangedEvent: evt was null!");
			return;
		}

		if (!PlayerManager.IsLocalPlayer(evt.newTarget))
			return;
		
		if (evt.newTarget != null && (_lastDungeonMusicLevel == eDungeonMusicLevel.Unknown) || (_lastDungeonMusicLevel == eDungeonMusicLevel.Low))
		{
			EB.Coroutines.Run(PostDungeonMusicSoundEvent(eDungeonMusicLevel.Medium));
			return;
		}
	}
	
	public static void ResetDungeonMusic()
	{
		if (!string.IsNullOrEmpty(_lastDungeonMusicEvent) && _lastDungeonMusicLevel != eDungeonMusicLevel.High)
		{
			PostEvent(_lastDungeonMusicEvent, false);
			_lastDungeonMusicEvent = null;
			_lastDungeonMusicLevel = eDungeonMusicLevel.Unknown;
		}
	}

	public static void StopAmbience()
	{
		if (!string.IsNullOrEmpty(_lastAmbienceEvent))
		{
			PostEvent(_lastAmbienceEvent, false);
			_lastAmbienceEvent = null;
		}
	}

    public static string _lastBGMEvent = "";
    public static string _resumeMusicEvent;
    public static void PostBGMEvent(string evt, bool play)
    {
        if (play)
        {
            if (evt == _lastBGMEvent)
            {
               return;
            }
            PostEvent(_lastBGMEvent, false);
            _lastBGMEvent = evt;
        }
        PostEvent(evt, play);
    }

    public static void StartBGM()
    {
        //_resumeMusicEvent = StopMusic();
        if (!string.IsNullOrEmpty(_lastGlobalMusicEvent))
        {
            _resumeMusicEvent = _lastGlobalMusicEvent;
            PostEvent(_lastGlobalMusicEvent, false);
            _lastGlobalMusicEvent = null;
        }
        //Debug.LogError("_resumeMusicEvent : "+ _resumeMusicEvent);
    }

    public static void StopBGM()
    {
        PostEvent(_lastBGMEvent, false);
        _lastBGMEvent = "";
        if (_resumeMusicEvent != null)
        {
            //Debug.LogError("Resume");
            ResumeMusic(_resumeMusicEvent);
        }
    }

	public static string StopMusic()
	{
		string current = _lastGlobalMusicEvent;
		if (!string.IsNullOrEmpty(_lastGlobalMusicEvent))
		{
			PostEvent(_lastGlobalMusicEvent, false);
			_lastGlobalMusicEvent = null;
		}

		return current;
	}

	public static void ResumeMusic(string evt)
	{
		if (string.IsNullOrEmpty(_lastGlobalMusicEvent) && !string.IsNullOrEmpty(evt))
		{
			PostGlobalMusicEvent(evt, true);
		}
	}
	
	// played when current spawner group is defeated
	public static void PostStingerWin()
	{
		// string evtNameString = "Music_Level_" + LastZonePrefix + "_Stinger_Win";
		string evtNameString = "SFX_Stinger_Win";
		
		PostEvent(evtNameString);
	}
	
	// player dies
	public static void PostStingerLose()
	{
		// string evtNameString = "Music_Level_" + LastZonePrefix + "_Stinger_Lose";
		string evtNameString = "SFX_Stinger_Lose";
		
		PostEvent(evtNameString);
	}
	
	// dungeon is cleared
	public static void PostStingerVictory()
	{
		string evtNameString = "Music_Level_" + _lastZonePrefix + "_Stinger_Victory";
		
		PostEvent(evtNameString);
	}
	
	// dungeon is entered
	public static void PostIntroSting()
	{
		string evtNameString = "Music_Level_" + _lastZonePrefix + "_IntroSting";

		PostEvent(evtNameString);
	}

	public static void PostCharacterAnimation(string characterAudioName, string animationName, bool play)
	{
		string evtNameString = "SFX_" + characterAudioName + "_" + animationName;

		PostEvent(evtNameString, play);
	}
	
	public static IEnumerator TryPostIntroSting()
	{
		// wait until a valid LastZonePrefix is determined (set on ZoneChangeEvent, via raycast to current ZoneDescriptor type)
		while(true)
		{
			if(string.IsNullOrEmpty(_lastZonePrefix))
			{
				// try again until a valid LastZonePrefix has been established
				yield return null;
			}
			else
			{
				// a valid LastZonePrefix exists, post the audio event and end this coroutine
				PostIntroSting();
				yield break;
			}
		}
	}

	private void OnGroundTypeChangedEvent(GameObject go, string tag)
	{
		eGroundType groundType;
		try
		{
			groundType = (eGroundType)System.Enum.Parse(typeof(eGroundType), tag);
		} catch (System.Exception)
		{
			EB.Debug.LogWarning("OnGroundTypeChangedEvent: " + tag + " is not a valid eGroundType");
			return;
		}

		// assign a ground type for any GameObject that we've heard from. External logic (footsteps) might query this
		_currentGroundTypes[go.GetInstanceID()] = groundType;
	}

	public static void PostFootstepEvent(GameObject go)
	{
		// Only play footsteps for local player character.
		if(!PlayerManager.IsLocalPlayer(go))
		{
			//DebugSystem.Log(Time.timeSinceLevelLoad + ": FusionAudio.PostFootstepEvent: Ignoring event for " + go.name);
			return;
		}

		if (!_currentGroundTypes.Contains(go.GetInstanceID()))
		{
			DebugSystem.Log("PostFootstepEvent: No record for " + go + " (" + go.GetInstanceID() + ")");
			return;
		}

		string evtNameString = "Footstep_Warrior_" + _currentGroundTypes[go.GetInstanceID()].ToString();

		//DebugSystem.Log(Time.timeSinceLevelLoad + ": FusionAudio.PostFootstepEvent: " + evtNameString + " (" + go.name + ")");

		PostEvent(evtNameString, go, true);
	}

	public static void PostGlobalMusicEvent(FusionAudio.eEvent evt)
	{
		PostGlobalMusicEvent(EventString(evt), true);
	}

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="evt">��������</param>
    /// <param name="play">�Ƿ񲥷�</param>
    /// <returns></returns>
	public static bool PostGlobalMusicEvent(string evt, bool play)
	{
		if (play)
		{
			if (evt != _lastGlobalMusicEvent)
			{
				PostEvent(_lastGlobalMusicEvent, false);
				_lastGlobalMusicEvent = evt;
				return PostEvent(evt, play);
			}
		}
		else
		{
			if (_lastGlobalMusicEvent == evt)
			{
				_lastGlobalMusicEvent = null;
			}
			return PostEvent(evt, play);
		}

		return true;
	}

	public static void PostAmbientSoundEvent(FusionAudio.eEvent evt)
	{
		PostAmbientSoundEvent(EventString(evt), true);
	}

    public static void SetAudioParameter(string eventName, string paramName, float paramValue)
    {
        EventManagerEx.Instance.SetParameter(eventName, paramName, paramValue);
    }

    public static void SetGroupAudioPinch(string groupName, float pinchValue)
    {
        GroupComponent group = EventManagerEx.Instance.GetGroupComponent(groupName);
        //Debug.LogError("Current Pitch is : "+ group.Pitch);
        group.SetPitch(pinchValue);
    }

    public static void SetGroupAudioVolume(string groupName, float Volume)
    {
        GroupComponent group = EventManagerEx.Instance.GetGroupComponent(groupName);
        //Debug.LogError("Current Pitch is : "+ group.Pitch);
        group.SetVolume(Volume);
    }

    public static void SetAudioPinch(string eventName, float pinchValue)
    {
        EventManagerEx.Instance.SetParameter(eventName, "Pitch", pinchValue);
    }

	public static bool PostAmbientSoundEvent(string evt, bool play)
	{
		if (play)
		{
			if (evt != _lastAmbienceEvent)
			{
				PostEvent(_lastAmbienceEvent, false);
				_lastAmbienceEvent = evt;
				return PostEvent(evt, true);
			}
		}
		else
		{
			if (_lastAmbienceEvent == evt)
			{
				_lastAmbienceEvent = null;
			}
			return PostEvent(evt, false);
		}

		return true;
	}

	public static eEvent ValueFromIndex(int index)
	{
		if (index >= 0)
		{
			System.Array values = System.Enum.GetValues(typeof(eEvent));
		
			return (eEvent)values.GetValue(index);
		}

		return eEvent.None;
	}

	public static int IndexFromValue(eEvent evt)
	{
		System.Array values = System.Enum.GetValues(typeof(eEvent));

		for (int i = 0; i < values.Length; i++)
		{
			if ((eEvent)values.GetValue(i) == evt)
				return i;
		}
		
		return -1;
	}
	
	#if DEBUG
	public void OnDrawDebug()
	{
	}

	public void OnDebugGUI()
	{
	}

	public void OnDebugPanelGUI()
	{
	}
	
	public void OnPreviousValuesLoaded()
	{
	}

	public void OnValueChanged(System.Reflection.FieldInfo field, object oldValue, object newValue)
	{
	}
	#endif
	
	public static void ResetDungeonAudio()
	{
		if (!string.IsNullOrEmpty(_lastDungeonMusicEvent))
		{
			PostEvent(_lastDungeonMusicEvent, false);
			_lastDungeonMusicEvent = null;
			_lastDungeonMusicLevel = eDungeonMusicLevel.Unknown;
		}
		
		if (!string.IsNullOrEmpty(_lastAmbienceEvent))
		{
			PostEvent(_lastAmbienceEvent, false);
			_lastAmbienceEvent = null;
		}
		
		_lastZonePrefix = "";
	}
}
