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

public class Dialogue : MonoBehaviour 
{
	[System.Serializable]
	public class DialogueData // what to turn on/off during the cinematic
	{
		public enum Progression
		{
			tap,
			time
		}

		public bool isExpanded = false; // only for the editor
		public bool isDuringGameplay = false; // if true, we don't have a custom UI, and we use the chat bubbles
		public CharacterModel trigger;
		public Progression progressionMethod = Progression.tap;
		public float progressionTime = -1f; // ignored if Progression is tap

		[System.Serializable]
		public class DialoguePointCamera : DialogueAction
		{
			public enum CameraTargets
			{
				localPlayer,
				caller,
				all,
			}

			public CameraLerp cameraLerpOverride;
			public CameraParams camera;
			public CameraTargets targets = CameraTargets.caller; // only applicable to certain camera types
			public GameObject cinematic; // only applicable to the cinematic camera type

			// execute the camera action
			public override void Execute()
			{
				Interaction currentInteraction = Dialogue.GetActiveInteraction();
				if (null != currentInteraction)
				{
					DialoguePoint currentDialoguePoint = currentInteraction.GetCurrentDialoguePoint();
					Interaction.SetUpNewCamera(this, (null != currentDialoguePoint) ? currentDialoguePoint.caller : null, currentInteraction.GetAllDialoguePoints());
				}
			}

			// is the cinematic a prefab, not in the scene
			public bool IsCinematicAPrefabRequiringInstantiation()
			{
				if (null == cinematic)
				{
					return false;
				}
				return !cinematic.activeInHierarchy; // if the cinematic gameObject is not active we assume it's a prefab
			}
		}
		public DialoguePointCamera exitCamera = null;
		public bool exitCameraShouldBeUsed = true;
		public bool removeDialogueHUDOnExit = true;
		public AnimationSequence exitAnimationSequence = new AnimationSequence();
		public DialogueActionContainer exitActions = new DialogueActionContainer();

		[System.Serializable]
		public class DialoguePoint
		{
			public enum ConversationBubble // type of bubble
			{
				speech,
				thought,
			}

			public bool isExpanded = false; // only for the editor
			public string call = "What is your call out?";
			public CharacterModel caller;

			public CameraLerp cameraLerpOverride;
			public CameraParams camera;
			public DialoguePointCamera.CameraTargets targets = DialoguePointCamera.CameraTargets.caller; // only applicable to certain camera types
			public GameObject cinematic; // only applicable to the cinematic camera type

			public DialoguePointCamera dialoguePointCamera = new Dialogue.DialogueData.DialoguePointCamera();
			public ConversationBubble bubbleType = ConversationBubble.speech;
			public AnimationSequence animationSequence = new AnimationSequence();

			public DialogueActionContainer actions = new DialogueActionContainer();

			#region GaM
			public string BiID;
			#endregion			
		}
		public List<DialoguePoint> dialoguePoints = new List<DialoguePoint>();

		public Dialogue.DialogueData.DialoguePointCamera GetExitCamera()
		{
			return (null != exitCamera && exitCameraShouldBeUsed) ? exitCamera : null; 
		}
	}

	public delegate void InteractionStartDelegate(DialogueData interactionContainer);
	public delegate void InteractionEndDelegate();

	static private InteractionStartDelegate onInteractionStart = null; // a callback for when an interaction begins	
	static private InteractionEndDelegate onInteractionEnd = null; // a callback for when an interaction ends

	public DialogueData dialogueData = new DialogueData();

	static private Interaction _interaction = new Interaction();
	static private DialogueData activeDialogue;
	static private GameObject _interactionHud = null;


	static public void CreateDialogueHUD()
	{
		if (null == _interactionHud)
		{
			const string InteractionUIPrefab = "UI/UI_DialogueBubble_Container";
			// UIHierarchyHelper is only used on the MainHudUI
			//_interactionHud = UIHierarchyHelper.Instance.LoadAndPlace(InteractionUIPrefab, UIHierarchyHelper.eUIType.NewPanel, null); 
			UIHierarchyHelper.Instance.LoadAndPlaceAsync(go=>
			{
				_interactionHud = go;
			}, InteractionUIPrefab, UIHierarchyHelper.eUIType.NewPanel, null);
		}
	}

	static public void Play(DialogueData data)
	{
		if (null != activeDialogue)
		{
			Stop();
		}
		activeDialogue = data;
		Start(data);		
	}

	static public void Update()
	{
		if (null != activeDialogue && _interaction.IsDuringGameplay())
		{
			_interaction.Update();
		}
	}

	static public void Stop()
	{
		if (null != activeDialogue)
		{
			// if animation tracks are specified, let that take care of exit animations
			if (!activeDialogue.exitAnimationSequence.ContainsTracks() && !activeDialogue.exitActions.ContainsAnmationTracks())
			{
				TriggerEndAnimations();
			}

			activeDialogue.exitAnimationSequence.Execute();
			activeDialogue.exitActions.Execute();

			Interaction activeInteraction = GetActiveInteraction();
			if (null != activeInteraction)
			{
				activeInteraction.ClearCharacterRotationLocks();
				activeInteraction.SwitchCharactersBackToPreviousTeam();
				activeInteraction.ClearCharacterInfo();
			}

			Dialogue.DialogueData.DialoguePointCamera exitCamera = activeDialogue.GetExitCamera();
			if (null != exitCamera)
			{
				Interaction.SetUpNewCamera(exitCamera, null, activeDialogue.dialoguePoints);
			}
			else if (!activeDialogue.exitActions.ContainsCameraAction() && null != Camera.main)
			{
				CameraBase cameraComponent = Camera.main.gameObject.GetComponent<CameraBase>();
				if (null != cameraComponent)
				{
					CameraLerp defaultDialogueLerp = GlobalDialogueData.Instance.defaultDialogueLerp;
					cameraComponent.EnterFixedOffsetGameCamera(defaultDialogueLerp);
				}
			}
			
			if (!activeDialogue.isDuringGameplay && activeDialogue.removeDialogueHUDOnExit)
			{
				UIStack.Instance.BackStack();
			}
			activeDialogue = null;

			if (null != onInteractionEnd)
			{
				onInteractionEnd();
			}
			EventManager.instance.Raise(new DialogueEvent(DialogueEvent.EventType.stop));
		}
	}

	static public Interaction GetActiveInteraction()
	{
		if (null != activeDialogue)
		{
			return _interaction;
		}
		return null;
	}

	// do we want a callback when an interaction starts?
	static public void AddStartCallback(InteractionStartDelegate callback)
	{
		RemoveStartCallback(callback); // avoid duplicate addition
		onInteractionStart += callback;
	}

	// do we no longer want a callback when an interaction starts?
	static public void RemoveStartCallback(InteractionStartDelegate callback)
	{
		onInteractionStart -= callback;
	}

	// do we want a callback when an interaction ends?
	static public void AddEndCallback(InteractionEndDelegate callback)
	{
		RemoveEndCallback(callback); // avoid duplicate addition
		onInteractionEnd += callback;
	}

	// do we no longer want a callback when an interaction starts?
	static public void RemoveEndCallback(InteractionEndDelegate callback)
	{
		onInteractionEnd -= callback;
	}

	static private void Start(DialogueData interactionContainer)
	{
		EventManager.instance.Raise(new DialogueEvent(DialogueEvent.EventType.start));
		UIStack.Instance.ExitStack(true);
		_interaction.ClearCharacterInfo();

		if (interactionContainer.isDuringGameplay)
		{
			_interaction.PreStart(ref interactionContainer);
			_interaction.Start(null);
		}
		else
		{
			// Show the interaction screen
			CreateDialogueHUD();			
			DialogueHUDLogic dialogueHUDLogic = _interactionHud.GetComponent<DialogueHUDLogic>();
			if (null != dialogueHUDLogic && !dialogueHUDLogic.IsOnUIStack()) // don't need to add it to the stack if it's already on there
			{
				UIStack.Instance.EnStack(dialogueHUDLogic);
			}			
			EnterParticipantsToDialogue();
		}
		if (null != onInteractionStart)
		{
			onInteractionStart(interactionContainer);
		}		
	}

	// perform any initialization on dialogue participants
	static private void EnterParticipantsToDialogue()
	{
		if (null != activeDialogue.dialoguePoints)
		{
			for (int dialogue = 0; dialogue < activeDialogue.dialoguePoints.Count; ++dialogue) // go through all dialogue conversation points
			{
				if (null != activeDialogue.dialoguePoints[dialogue])
				{
					GameObject caller = GlobalUtils.FindCharacter(activeDialogue.dialoguePoints[dialogue].caller);
					if (null != caller)
					{
						CharacterComponent charComp = caller.GetComponent<CharacterComponent>();
						if (null != charComp)
						{
							AnimationSequence.SetAnimationEnterState(charComp); // start the dialogue enter animation

							if (null != charComp.TargetingComponent)
							{
								charComp.TargetingComponent.SetMovementTarget(Vector3.zero ,true); // stop the character movement
								charComp.TargetingComponent.SetAttackTarget(null); // stop the character attacking								
							}							
						}
						//_interaction.SwitchCharacterTeam(caller.GetComponent<NPCStatsComponent>()); // stop an enemy from being targetable
					}
				}
			}
		}
	}

	static private void TriggerEndAnimations()
	{
		if (null != activeDialogue.dialoguePoints)
		{
			for (int dialogue = 0; dialogue < activeDialogue.dialoguePoints.Count; ++dialogue)
			{
				if (null != activeDialogue.dialoguePoints[dialogue])
				{
					GameObject participant = GlobalUtils.FindCharacter(activeDialogue.dialoguePoints[dialogue].caller);
					if (null != participant)
					{
						//CharacterComponent charTarget = participant.GetComponent<CharacterComponent>();
						//if (null != charTarget && eCharacterState.Dialogue == charTarget.State)
						//{
							//AnimationSequence.SetAnimationExitState(charTarget);
							//charTarget.State = eCharacterState.Idle;
						//}	
					}
				}
			}
		}
	}
}
