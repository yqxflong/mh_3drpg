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
public class AnimationSequence : DialogueAction
{
	[System.Serializable]
	public class AnimationTrack
	{
		public bool IsExpanded { get; set; }

		public CharacterModel characterToUse;
		public eDialogueAnimationSubState animationToPlay = eDialogueAnimationSubState.None;
		public eIdleVariety idleToPlay = eIdleVariety.None;
		public bool doLoop = false; // false is play once
	}

	public bool IsExpanded { get; set; }
	public List<AnimationTrack> animations = new List<AnimationTrack>();

	static public void SetAnimationEnterState(CharacterComponent charTarget)
	{
		SetAnimationState(eDialogueAnimationSubState.Enter, charTarget, false);
	}

	static public void SetAnimationExitState(CharacterComponent charTarget)
	{
		SetAnimationState(eDialogueAnimationSubState.Exit, charTarget, false);
	}

	// does this animation sequence have any tracks
	public bool ContainsTracks()
	{
		return animations.Count > 0;
	}

	// play all the tracks
	public override void Execute()
	{
		for (int track = 0; track < animations.Count; ++track)
		{
			GameObject characterToUse = GlobalUtils.FindCharacter(animations[track].characterToUse);
			if (null != characterToUse)
			{
				CharacterComponent charTarget = characterToUse.GetComponent<CharacterComponent>();
				if (null != charTarget)
				{					
					SetAnimationState(animations[track].animationToPlay, charTarget, animations[track].doLoop);						
				}
			}
		}
	}

	private static void SetAnimationState(eDialogueAnimationSubState animationToPlay, CharacterComponent charTarget, bool doLoop)
	{
		if (null == charTarget || eDialogueAnimationSubState.None == animationToPlay)
		{
			return;
		}

		//if (eCharacterState.Dialogue != charTarget.State)
		//{
			//charTarget.State = eCharacterState.Dialogue;
		//}

		//DialogueStateHandler specialHandler = (charTarget.CurrentHandler as DialogueStateHandler);
		//if (null != specialHandler)
		//{
			//specialHandler.SetAnimationState(animationToPlay, doLoop);
		//}
	}
}
