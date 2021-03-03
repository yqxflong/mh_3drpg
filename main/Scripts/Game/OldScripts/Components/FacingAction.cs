///////////////////////////////////////////////////////////////////////
//
//  FacingAction.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FacingAction : DialogueAction
{
	public bool IsExpanded { get; set; }
	public CharacterModel characterToOrient;
	public CharacterModel characterToLookAt;

	static public void OrientFacing(CharacterModel orient, CharacterModel lookat)
	{
		GameObject characterToOrientGameObject = GlobalUtils.FindCharacter(orient);
		if (null != characterToOrientGameObject)
		{
			GameObject characterToLookAtGameObject = GlobalUtils.FindCharacter(lookat);
			if (null != characterToLookAtGameObject)
			{
				OrientFacing(characterToOrientGameObject, characterToLookAtGameObject);
			}
		}
	}

	static public void OrientFacing(GameObject orient, GameObject lookAt)
	{
		if (null != orient && null != lookAt)
		{
			LocomotionComponentAPP loco = orient.GetComponent<LocomotionComponentAPP>();
			if (null != loco)
			{
				loco.LookAtPosition(lookAt.transform.position, true);				
				Interaction activeInteraction = Dialogue.GetActiveInteraction();
				if (null != activeInteraction)
				{
					activeInteraction.LockCharacterRotation(loco);
				}
			}			
		}
	}

	public override void Execute()
	{
		OrientFacing(characterToOrient, characterToLookAt);
	}
}
