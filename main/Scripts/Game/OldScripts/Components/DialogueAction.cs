///////////////////////////////////////////////////////////////////////
//
//  Dialogue.cs
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
public class DialogueAction
{
	public virtual void Execute(){}
}

[System.Serializable]
public class DialogueActionContainer
{
	public bool isExpanded { get; set; }

	public List<AnimationSequence> animationSequenceActions = new List<AnimationSequence>();
	public List<UtteranceSequence> utteranceSequenceActions = new List<UtteranceSequence>();
	public List<FacingAction> facingActions = new List<FacingAction>();
	public List<Dialogue.DialogueData.DialoguePointCamera> cameraActions = new List<Dialogue.DialogueData.DialoguePointCamera>();

	public void Execute()
	{
		for (int index = 0; index < animationSequenceActions.Count; ++index)
		{
			animationSequenceActions[index].Execute();
		}

		for (int index = 0; index < utteranceSequenceActions.Count; ++index)
		{
			utteranceSequenceActions[index].Execute();
		}

		for (int index = 0; index < facingActions.Count; ++index)
		{
			facingActions[index].Execute();
		}

		for (int index = 0; index < cameraActions.Count; ++index)
		{
			cameraActions[index].Execute();
		}
	}

	// is there a facing action
	public bool ContainsFacingActions()
	{
		return facingActions.Count > 0;
	}

	// is there an animation track
	public bool ContainsAnmationTracks()
	{
		for (int index = 0; index < animationSequenceActions.Count; ++index)
		{
			if (animationSequenceActions[index].ContainsTracks())
			{
				return true;
			}
		}
		return false;
	}

	// is there a camera action
	public bool ContainsCameraAction()
	{
		return cameraActions.Count > 0;
	}

#if UNITY_EDITOR
	// add the type we want
	public void Add(System.Type newType)
	{		
		if (typeof(AnimationSequence) == newType)
		{
			animationSequenceActions.Add(new AnimationSequence());
		}
		else if (typeof(UtteranceSequence) == newType)
		{
			utteranceSequenceActions.Add(new UtteranceSequence());
		}
		else if (typeof(FacingAction) == newType)
		{
			facingActions.Add(new FacingAction());
		}
		else if (typeof(Dialogue.DialogueData.DialoguePointCamera) == newType)
		{
			cameraActions.Add(new Dialogue.DialogueData.DialoguePointCamera());
		}
		else
		{
			EB.Debug.LogError("DialogueActionContainer.Add() : DialogueAction SubType cannot be created");
		}		
	}

	// get all
	public void GetAllActions(ref List<DialogueAction> outActions)
	{
		for (int index = 0; index < animationSequenceActions.Count; ++index)
		{
			outActions.Add(animationSequenceActions[index]);
		}

		for (int index = 0; index < utteranceSequenceActions.Count; ++index)
		{
			outActions.Add(utteranceSequenceActions[index]);
		}

		for (int index = 0; index < facingActions.Count; ++index)
		{
			outActions.Add(facingActions[index]);
		}

		for (int index = 0; index < cameraActions.Count; ++index)
		{
			outActions.Add(cameraActions[index]);
		}
	}

	// get all
	public void RemoveAction(DialogueAction action)
	{
		if (typeof(AnimationSequence) == action.GetType())
		{
			if (animationSequenceActions.Contains((AnimationSequence)action))
			{
				animationSequenceActions.Remove((AnimationSequence)action);
			}
		}
		else if (typeof(UtteranceSequence) == action.GetType())
		{
			if (utteranceSequenceActions.Contains((UtteranceSequence)action))
			{
				utteranceSequenceActions.Remove((UtteranceSequence)action);
			}
		}
		else if (typeof(FacingAction) == action.GetType())
		{
			if (facingActions.Contains((FacingAction)action))
			{
				facingActions.Remove((FacingAction)action);
			}
		}
		else if (typeof(Dialogue.DialogueData.DialoguePointCamera) == action.GetType())
		{
			if (cameraActions.Contains((Dialogue.DialogueData.DialoguePointCamera)action))
			{
				cameraActions.Remove((Dialogue.DialogueData.DialoguePointCamera)action);
			}
		}
		else
		{
			EB.Debug.LogError("DialogueActionContainer.RemoveAction() : DialogueAction SubType cannot be removed");
		}	
	}
#endif
}

