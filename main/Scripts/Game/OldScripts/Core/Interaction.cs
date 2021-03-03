///////////////////////////////////////////////////////////////////////
//
//  Interaction.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class Interaction
{
	public delegate void DialogueAdvanceDelegate(Dialogue.DialogueData.DialoguePoint conversation);
	private DialogueAdvanceDelegate _onDialogueAdvance = null;
	private List<Dialogue.DialogueData.DialoguePoint> _dialoguePoints;

	private const int InteractionNotStarted = -1;
	private int _currentInteraction = InteractionNotStarted;

	private Dialogue.DialogueData _dialogueData;
	private float _lastAdvanceTime = -1f;

	private struct InteractionCharacterInfo
	{
		public GameObject gameObj;
		public LocomotionComponentAPP loco;
		//public NPCStatsComponent stats;

		public InteractionCharacterInfo(GameObject gameObj, LocomotionComponentAPP loco)
		{
			this.gameObj = gameObj;
			this.loco = loco;
		}

		//public InteractionCharacterInfo(GameObject gameObj, LocomotionComponentAPP loco, NPCStatsComponent stats)
		//{
		//	this.gameObj = gameObj;
		//	this.loco = loco;
		//	this.stats = stats;
		//}
	}
	private List<InteractionCharacterInfo> _charactersInfo = new List<InteractionCharacterInfo>();

	public bool IsTapToProgress()
	{
		if (null == _dialogueData)
		{
			return true;
		}
		return Dialogue.DialogueData.Progression.tap == _dialogueData.progressionMethod;
	}

	public bool IsDuringGameplay()
	{
		if (null == _dialogueData)
		{
			return false;
		}
		return _dialogueData.isDuringGameplay;
	}

	public void Update()
	{
		if (IsTapToProgress())
		{
			return;
		}

		if ((Time.time - _lastAdvanceTime) > _dialogueData.progressionTime)
		{
			AdvanceInteraction();
		}
	}

	public void AdvanceInteraction()
	{		
		if (null != _dialogueData)
		{
			AdvanceDialogue();
		}
	}

	// turn on the type of camera we need for this conversation step
	static public void SetUpNewCamera(Dialogue.DialogueData.DialoguePointCamera conversation, CharacterModel inputCaller, List<Dialogue.DialogueData.DialoguePoint> dialoguePoints)
	{
		if (null == conversation.camera)
		{
			return;
		}

		CameraLerp cameraLerp = (null != conversation.cameraLerpOverride) ? conversation.cameraLerpOverride : GlobalDialogueData.Instance.defaultDialogueLerp;

		System.Type cameraType = conversation.camera.GetType();
		if (typeof(GameCameraParams) == cameraType)
		{
			if (null != Camera.main)
			{
				CameraBase cameraComponent = Camera.main.GetComponent<CameraBase>();

				GameCameraParams gameCameraParams = (GameCameraParams)conversation.camera;
				switch (conversation.targets)
				{
					case Dialogue.DialogueData.DialoguePointCamera.CameraTargets.localPlayer:
						if (gameCameraParams.gameCamera)
						{
							cameraComponent.EnterFixedOffsetGameCamera(cameraLerp);
						}
						else
						{
							PlayerController controller = PlayerManager.LocalPlayerController();
							if (null != controller)
							{
								List<GameObject> target = new List<GameObject>();
								target.Add(controller.gameObject);
								cameraComponent.EnterInteractionCamera(ref target, ref gameCameraParams, cameraLerp);
							}
						}
						break;
					case Dialogue.DialogueData.DialoguePointCamera.CameraTargets.all:
						List<GameObject> targets = new List<GameObject>();
						Interaction.GetAllInteractionParticipants(ref targets, dialoguePoints);
						if (targets.Count > 0)
						{
							cameraComponent.EnterInteractionCamera(ref targets, ref gameCameraParams, cameraLerp);
						}
						break;
					case Dialogue.DialogueData.DialoguePointCamera.CameraTargets.caller:
						GameObject caller = GlobalUtils.FindCharacter(inputCaller);
						if (null != caller)
						{
							if (inputCaller.isPlayer && gameCameraParams.gameCamera)
							{
								cameraComponent.EnterFixedOffsetGameCamera(cameraLerp);
							}
							else
							{
								List<GameObject> target = new List<GameObject>();
								target.Add(caller);
								cameraComponent.EnterInteractionCamera(ref target, ref gameCameraParams, cameraLerp);
							}
						}
						break;
					default: break;
				}
			}
		}
		else if (typeof(CinematicCameraParams) == cameraType)
		{
			if (null != conversation.cinematic)
			{
				if (conversation.IsCinematicAPrefabRequiringInstantiation())
				{
					GameObject cinematicObject = (GameObject)GameObject.Instantiate(conversation.cinematic);
					if (null != cinematicObject && cinematicObject.activeInHierarchy)
					{
						Cinematic.Play(cinematicObject.GetComponent<Cinematic>(), PlayerManager.LocalPlayerGameObject());
					}
				}
				else
				{
					Cinematic.Play(conversation.cinematic.GetComponent<Cinematic>(), PlayerManager.LocalPlayerGameObject());
				}
			}
		}
	}

	public void PreStart(ref Dialogue.DialogueData interactionContainer)
	{
		_dialogueData = interactionContainer;
	}

	public void Start(DialogueAdvanceDelegate callback)
	{
		Start(ref _dialogueData.dialoguePoints, callback);
	}

	public void Start(ref List<Dialogue.DialogueData.DialoguePoint> interactions, DialogueAdvanceDelegate callback)
	{
		_onDialogueAdvance = null;
		if (null != callback)
		{
			_onDialogueAdvance += callback;
		}
		else if (null != _dialogueData && _dialogueData.isDuringGameplay)
		{
			_onDialogueAdvance += AdvanceChatBubbles;
		}

		_dialoguePoints = interactions;
		_currentInteraction = InteractionNotStarted;
		AdvanceInteraction();
	}

	public void AdvanceDialogue()
	{
		++_currentInteraction;

		Dialogue.DialogueData.DialoguePoint dialoguePoint = GetCurrentDialoguePoint();
		if (null != dialoguePoint)
		{
			Dialogue.DialogueData.DialoguePointCamera dialoguePointCamera = new Dialogue.DialogueData.DialoguePointCamera();

			dialoguePointCamera.camera = dialoguePoint.camera;
			dialoguePointCamera.cameraLerpOverride = dialoguePoint.cameraLerpOverride;
			dialoguePointCamera.cinematic = dialoguePoint.cinematic;
			dialoguePointCamera.targets = dialoguePoint.targets;

			Interaction.SetUpNewCamera(dialoguePointCamera, dialoguePoint.caller, _dialoguePoints);

			dialoguePoint.animationSequence.Execute();
			dialoguePoint.actions.Execute();
			SetDefaultFacing(dialoguePoint);
		}
		if (null != _onDialogueAdvance)
		{
			_onDialogueAdvance(dialoguePoint);
		}
		if (null == dialoguePoint)
		{
			Dialogue.Stop();		
		}
		_lastAdvanceTime = Time.time;
	}

	public void AdvanceChatBubbles(Dialogue.DialogueData.DialoguePoint conversation)
	{
		if (null != conversation)
		{
			GameObject caller = GlobalUtils.FindCharacter(conversation.caller);
			if (null != caller)
			{
				ShowChatBubble showChat = caller.GetComponent<ShowChatBubble>();
				if (null != showChat)
				{
					showChat.Show(StringUtils.ReplaceTokens(conversation.call));
				}
			}
		}
	}

	// get all the participants in the conversation
	static public void GetAllInteractionParticipants(ref List<GameObject> outParticipants, List<Dialogue.DialogueData.DialoguePoint> dialoguePoints)
	{
		if (null != dialoguePoints)
		{
			for (int dialogue = 0; dialogue < dialoguePoints.Count; ++dialogue)
			{
				if (null != dialoguePoints[dialogue])
				{
					GameObject caller = GlobalUtils.FindCharacter(dialoguePoints[dialogue].caller);
					if (null != caller && !outParticipants.Contains(caller))
					{
						outParticipants.Add(caller);
					}
				}
			}
		}
	}
	 

	// does the dialogue use the participant
	static public bool UsesParticipant(List<Dialogue.DialogueData.DialoguePoint> dialoguePoints, GameObject participant)
	{
		if (null != dialoguePoints)
		{
			for (int dialogue = 0; dialogue < dialoguePoints.Count; ++dialogue)
			{
				if (null != dialoguePoints[dialogue])
				{
					GameObject caller = GlobalUtils.FindCharacter(dialoguePoints[dialogue].caller);
					if (null != caller && participant == caller)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// get the current conversation
	public Dialogue.DialogueData.DialoguePoint GetCurrentDialoguePoint()
	{
		if (null != _dialoguePoints && _currentInteraction < _dialoguePoints.Count)
		{
			return _dialoguePoints[_currentInteraction];
		}
		return null;
	}

	// get the current conversation
	public List<Dialogue.DialogueData.DialoguePoint> GetAllDialoguePoints()
	{
		return _dialoguePoints;
	}

	// lock a character's rotation
	public void LockCharacterRotation(LocomotionComponentAPP loco)
	{
		if (null != loco)
		{
			int index = _charactersInfo.FindIndex(charInfo => charInfo.gameObj == loco.gameObject );

			if (index < 0 || index >= _charactersInfo.Count) // out of range
			{
				_charactersInfo.Add(new InteractionCharacterInfo(loco.gameObject, loco));
			}
			else if (loco != _charactersInfo[index].loco) // update existing entry
			{
				InteractionCharacterInfo info = _charactersInfo[index];
				info.loco = loco;
				_charactersInfo[index] = info;
			}
			loco.ExclusivelyRotate(true);
		}
	}

	// release any character's who have had there rotation locked for the dialogue
	public void ClearCharacterRotationLocks()
	{
		for (int character = 0; character < _charactersInfo.Count; ++character)
		{
			if (null != _charactersInfo[character].loco)
			{
				_charactersInfo[character].loco.ExclusivelyRotate(false);
			}
		}		
	}

	// switch a character's team
	//public void SwitchCharacterTeam(NPCStatsComponent stats)
	//{
	//	if (null != stats && eTeamId.Enemy == stats.Team)
	//	{
	//		int index = _charactersInfo.FindIndex(charInfo => charInfo.gameObj == stats.gameObject);

	//		if (index < 0 || index >= _charactersInfo.Count) // out of range
	//		{
	//			_charactersInfo.Add(new InteractionCharacterInfo(stats.gameObject, null, stats));
	//		}
	//		else if (stats != _charactersInfo[index].stats) // update existing entry
	//		{
	//			InteractionCharacterInfo info = _charactersInfo[index];
	//			info.stats = stats;
	//			_charactersInfo[index] = info;
	//		}
	//		stats.ChangeTeamToInteractable();
	//	}
	//}

	public void SwitchCharactersBackToPreviousTeam()
	{
		//for (int character = 0; character < _charactersInfo.Count; ++character)
		//{
		//	if (null != _charactersInfo[character].stats)
		//	{
		//		_charactersInfo[character].stats.ChangeTeamToEnemy();
		//	}
		//}
	}

	public void ClearCharacterInfo()
	{
		_charactersInfo.Clear();
	}

	// set the facing of the player and the current talker
	private void SetDefaultFacing(Dialogue.DialogueData.DialoguePoint dialoguePoint)
	{
		if (null == dialoguePoint || // we can't do anything without a dialogue point
			dialoguePoint.actions.ContainsFacingActions() || // if this step has facing actions, then don't do anything here
			IsDuringGameplay()) // if this is a speech bubble conversation during gameplay, we're not gonna set facing
		{
			return;
		}

		GameObject caller = GlobalUtils.FindCharacter(dialoguePoint.caller);
		if (null == caller) // if we can't find the character to use
		{
			return;
		}

		GameObject player = PlayerManager.LocalPlayerGameObject();
		if (null == player) // if we have no player we don't know who to look at
		{
			return;
		}

		if (caller != player)
		{
			FacingAction.OrientFacing(caller, player);
		}
	}
}
