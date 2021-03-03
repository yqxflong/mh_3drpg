///////////////////////////////////////////////////////////////////////
//
//  AnimationSequence.cs
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
public class UtteranceSequence : DialogueAction
{
	public bool IsExpanded { get; set; }
	public CharacterModel characterToUse;
	public FusionAudio.eEvent soundToPlay = FusionAudio.eEvent.None;	

	public override void Execute()
	{
		GameObject characterGameObject = GlobalUtils.FindCharacter(characterToUse);
		if (null != characterGameObject)
		{
			FusionAudio.PostEvent(soundToPlay, characterGameObject, true);
		}
		else
		{
			FusionAudio.PostEvent(soundToPlay);
		}
	}
}
