///////////////////////////////////////////////////////////////////////
//
//  EffectEvents.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

[System.Flags]
public enum eEffectEvent
{
	//NOTE: The unity flags dropdown doesn't understand non-sequential values. In other words, don't skip any numbers.

	AbilityPreparingStart = 1 << 0,
	AbilityPreparingCancel = 1 << 1,
	AbilityCastingStart = 1 << 2,
	AbilityHitEvent = 1 << 3,
	AutoAttackStart = 1 << 4,
	AutoAttackHit = 1 << 5,
	AutoAttackCancel = 1 << 6,
	ChargeAttackStart = 1 << 7,
	ChargeAttackHit = 1 << 8,
	ChargeAttackCancel = 1 << 9,
	AbilityInterrupt = 1 << 10,
	AbilityEnd = 1 << 11,
	AutoAttackChanged = 1 << 12
}
