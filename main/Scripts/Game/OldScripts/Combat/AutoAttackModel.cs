///////////////////////////////////////////////////////////////////////
//
//  AutoAttackModel.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

[System.Serializable]
public class AutoAttackModel : ScriptableObject
{
	public bool isExpanded = true;

	//public EffectModel startAttackEffectModel;
	//public EffectModel attackEffectModel;
	public float attackAnimationVariety;
	public int maxVictims = 3;

	//Charge attacks
	public bool canCharge = false;
	public bool fireAtMaxTime = true;
	public float maxChargeTime = 2.0f;
	//public EffectModel startChargeEffectModel;
	//public EffectModel chargeEffectModel;

	public void Preload()
	{
		//if (startAttackEffectModel != null)
		//{
		//	startAttackEffectModel.Preload();
		//}
		//if (attackEffectModel != null)
		//{
		//	attackEffectModel.Preload();
		//}
		//if (startChargeEffectModel != null)
		//{
		//	startChargeEffectModel.Preload();
		//}
		//if (chargeEffectModel != null)
		//{
		//	chargeEffectModel.Preload();
		//}
	}
}