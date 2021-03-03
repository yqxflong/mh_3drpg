///////////////////////////////////////////////////////////////////////
//
//  DialogueHUDLogic.cs
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

public class DialogueHUDLogic : FusionScreenUI
{
	public GameObject bubbleContainer;
	public GameObject screenBlocker;
	public GameObject characterName;
	public GameObject dialogueBody;
	public GameObject bubbleCorner;
	public GameObject arrow;
	public GameObject speechTail;
	public GameObject thoughtBubbleContainer;
	public GameObject thoughtTail;
	
	private UILabel _uiLabelCharacterName = null;
	private UILabel _uiLabelDialogueBody = null;
	private UISprite _uiSpriteSpeechTail = null;
	private UISprite _uiSpriteThoughtTail = null;
	private UIPanel _uiPanel = null;
	
	private float _initialLocalTailY = 0f;
	private float _initialLocalThoughtTailY = 0f;

	private CameraBase _cameraComponent = null;
	private bool _waitingForCameraTransition = false;

	private Dialogue.DialogueData.DialoguePoint _currentPoint;
	private bool _hasStarted = false;
	private bool _doRefreshPanel = false;

	private int numTimesAddedToUIStack = 0;

	private UITweenActivator openTween;
	private UITweenActivator closeTween;

	public override bool EnstackOnCreate
	{
		get
		{
			return false;
		}
	}

	public override float BackgroundUIFadeTime
	{
		get
		{
			return 1.0f;
		}
	}

	public void OnEnable()
	{
		Dialogue.AddStartCallback(OnInteractionStart);
		DeactivateImediate();
	}

	public void OnDisable()
	{
		Dialogue.RemoveStartCallback(OnInteractionStart);
	}

	public override bool CanAutoBackstack()
	{
		return false;
	}

	public override bool IsRenderingWorldWhileFullscreen()
	{
		return true; // we want to see the world behind the dialogue
	}

	public bool IsOnUIStack()
	{
		return numTimesAddedToUIStack > 0;
	}

	public override IEnumerator OnAddToStack()
	{
		++numTimesAddedToUIStack;
		NGUITools.SetActive(gameObject, true);
		return base.OnAddToStack();
	}

	public override IEnumerator OnRemoveFromStack()
	{
		--numTimesAddedToUIStack;
		NGUITools.SetActive(gameObject, false);
		yield break;
	}

	public void Update()
	{
		if (_doRefreshPanel)
		{
			_uiPanel.Refresh();
			_doRefreshPanel = false;
		}

		if (!_hasStarted)
		{
			_hasStarted = true;
			Dialogue.GetActiveInteraction().Start(Advance);
		}

		Dialogue.GetActiveInteraction().Update();

		if (null == _currentPoint)
		{
			return;
		}

		if (AreAllComponentsCached())
		{
			if (_waitingForCameraTransition && !_cameraComponent.IsCameraTransitioning())
			{
				_waitingForCameraTransition = false;
				SetUpNewConversation(_currentPoint);
			}

			if (!_waitingForCameraTransition)
			{
				Vector3 outPosition = new Vector3();
				Quaternion outRotation = new Quaternion();
				GetLatestCameraTransform(_currentPoint, ref outPosition, ref outRotation);

				PositionBubbleTailHorizontally(_currentPoint.caller, outPosition, outRotation);
			}
		}		
	}

	public void OnForgroundScreenBlockerClicked()
	{
		if (!_waitingForCameraTransition && _hasStarted && Dialogue.GetActiveInteraction().IsTapToProgress())
		{
			#region GaM
			SendBiID(Dialogue.GetActiveInteraction().GetCurrentDialoguePoint().BiID);
			#endregion

			Dialogue.GetActiveInteraction().AdvanceInteraction();
		}
	}

	public static void SendBiID(string biID)
	{
	}

	// is the point in the left horizontal third of the screen
	private bool IsPointInLeftThirdOfScreenWidth(Vector3 screenPoint)
	{
		return (screenPoint.x < CalculateOneThirdOfScreenWidth());					
	}

	private bool IsPointInRightThirdOfScreenWidth(Vector3 screenPoint)
	{
		return (screenPoint.x > (2f * CalculateOneThirdOfScreenWidth()));	
	}

	// calculate one third of screen width
	private float CalculateOneThirdOfScreenWidth()
	{
		const float Third = 3f;
		float screenWidth = Screen.width;
		float oneThird = screenWidth / Third;
		return oneThird;
	}

	// set the horizontal position of the bubble
	private void PositionBubbleHorizontally(CharacterModel callerInput, Vector3 latestBehaviorPosition, Quaternion latestBehaviorRotation)
	{
		GameObject caller = GlobalUtils.FindCharacter(callerInput);
		if (null != caller && null != UICamera.mainCamera)
		{
			Vector3 screenPoint = WorldToScreenPoint(caller.transform.position, latestBehaviorPosition, latestBehaviorRotation);

			const float HorizontalShift = 250f; // how much we move the bubble by, based on it being in the left/right third
			if (IsPointInLeftThirdOfScreenWidth(screenPoint))
			{
				bubbleContainer.transform.localPosition = new Vector3(-HorizontalShift, 0f, 0f);
			}
			else if (IsPointInRightThirdOfScreenWidth(screenPoint))
			{
				bubbleContainer.transform.localPosition = new Vector3(HorizontalShift, 0f, 0f);
			}
			else // point must be in the center of the screen
			{
				bubbleContainer.transform.localPosition = Vector3.zero;
			}
		}
	}

	// set the horizontal position of the bubble tail
	private void PositionBubbleTailHorizontally(CharacterModel callerInput, Vector3 latestBehaviorPosition, Quaternion latestBehaviorRotation)
	{
		GameObject caller = GlobalUtils.FindCharacter(callerInput);
		if (null != caller && null != UICamera.mainCamera)
		{
			Vector3 screenPoint = WorldToScreenPoint(caller.transform.position, latestBehaviorPosition, latestBehaviorRotation);			

			Vector3 worldPoint = UICamera.mainCamera.ScreenToWorldPoint(screenPoint);
			worldPoint.z = 0f;

			const float Padding = 0.75f;
			float mainBubbleXExtent = (_uiLabelDialogueBody.localSize.x * 0.5f) * Padding;

			PositionSpeechBubbleTailHorizontally(worldPoint, mainBubbleXExtent);
			PositionThoughtBubbleTailHorizontally(worldPoint, mainBubbleXExtent);
		}
	}

	// calculate where the point would be on screen at the camera position and rotation specified
	private Vector3 WorldToScreenPoint(Vector3 point, Vector3 cameraPosition, Quaternion cameraRotation)
	{		
		Matrix4x4 transformMat = Matrix4x4.TRS(cameraPosition, cameraRotation, Vector3.one);
		Vector3 localPoint = transformMat.inverse.MultiplyVector(point - cameraPosition); // put the point into the local space of the camera
		if (localPoint.z >= Camera.main.nearClipPlane) // if the point would be in front of the camera
		{
			// calculate the screen height of the point
			float screenHeightAtNearClip = Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad) * Camera.main.nearClipPlane;
			
			Vector3 localPointHeight = new Vector3(0f, localPoint.y, localPoint.z);
			localPointHeight.Normalize();
			float angleRad = Mathf.Acos(localPointHeight.z);

			float pointHeightAtNearClip = Mathf.Tan(angleRad) * Camera.main.nearClipPlane;
			float resultFromHeight = pointHeightAtNearClip / screenHeightAtNearClip;

			float screenHeightHalf = 0.5f * Screen.height;
			float finalScreenResultHeight = screenHeightHalf + ((localPointHeight.y > 0f) ? (resultFromHeight * screenHeightHalf) : -(resultFromHeight * screenHeightHalf));

			// calculate the screen width of the point
			Vector3 localPointWidth = new Vector3(localPoint.x, 0f, localPoint.z);
			localPointWidth.Normalize();
			angleRad = Mathf.Acos(localPointWidth.z);

			float pointWidthAtNearClip = Mathf.Tan(angleRad) * Camera.main.nearClipPlane;
			float screenWidthAtNearClip = screenHeightAtNearClip * Camera.main.aspect;
			float resultFromWidth = pointWidthAtNearClip / screenWidthAtNearClip;

			float screenWidthHalf = 0.5f * Screen.width;
			float finalScreenResultWidth = screenWidthHalf + ((localPointWidth.x > 0f) ? (resultFromWidth * screenWidthHalf) : -(resultFromWidth * screenWidthHalf));

			return new Vector3(finalScreenResultWidth, finalScreenResultHeight, localPoint.z);
		}
		return new Vector3(Screen.width * 2f, Screen.height * 2f, localPoint.z); // position off screen
	}

	private void PositionSpeechBubbleTailHorizontally(Vector3 worldPos, float localXLimit)
	{
		// speech tail positioning
		speechTail.transform.position = worldPos;
		// shift right, so the left side of the tail (the bottom) is correctly aligned with the character
		float localXRightShifted = speechTail.transform.localPosition.x + (_uiSpriteSpeechTail.localSize.x * 0.5f);
		// clamp the position of the tail so it doesn't get positioned to the left or right of the main bubble
		float localXClamped = Mathf.Clamp(localXRightShifted, -localXLimit, localXLimit);
		speechTail.transform.localPosition = new Vector3(localXClamped, _initialLocalTailY, speechTail.transform.localPosition.z);
	}

	private void PositionThoughtBubbleTailHorizontally(Vector3 worldPos, float localXLimit)
	{
		// though tail positioning
		thoughtTail.transform.position = worldPos;
		// shift right, so the left side of the tail (the bottom) is correctly aligned with the character
		float localXRightShifted = thoughtTail.transform.localPosition.x + (_uiSpriteThoughtTail.localSize.x * 0.5f);
		// clamp the position of the tail so it doesn't get positioned to the left or right of the main bubble					
		float localXClamped = Mathf.Clamp(localXRightShifted, -localXLimit, localXLimit);
		thoughtTail.transform.localPosition = new Vector3(localXClamped, _initialLocalThoughtTailY, thoughtTail.transform.localPosition.z);
	}

	private void SetCharacterName(CharacterModel callerInput)
	{
		if (null != callerInput)
		{
			if (callerInput.isPlayer)
			{
				//PlayerController localController = PlayerManager.LocalPlayerController();
				bool hasColorBeenSet = false;
				//if (localController && localController.Inventory) // we only cache the inventory component for the player (see Start function)
				//{
					//DyeModel dye = localController.Inventory.GetArmorDye();
					//if (dye)
					//{
						//_uiLabelCharacterName.color = new Color(dye.displayColor.r, dye.displayColor.g, dye.displayColor.b, 1f);
						//hasColorBeenSet = true;
					//}
				//}

				if (!hasColorBeenSet)
				{
					// conversation.caller is a CharacterModel for a player, however, it might not be the CharacterModel of the current local player
					CharacterModel playerModel = GameUtils.GetLocalPlayerCharacterModel(); 
					if (null != playerModel)
					{
						_uiLabelCharacterName.color = playerModel.dialogueTextColor;
					}
				}
				_uiLabelCharacterName.text = StringUtils.ReplaceTokens(StringUtils.TOKEN_CHAR_NAME);
			}
			else
			{
				_uiLabelCharacterName.color = callerInput.dialogueTextColor;
				_uiLabelCharacterName.text = StringUtils.ReplaceTokens(callerInput.displayName);
			}
		}
		else
		{
			_uiLabelCharacterName.text = "";
		}
		NGUITools.SetActive(_uiLabelCharacterName.gameObject, true); // force a refresh (ensures text is rendered)
	}

	private bool AreAllComponentsCached()
	{
		return (null != GlobalCameraData.Instance &&
				null != _cameraComponent && 
				null != bubbleContainer && 
				null != screenBlocker &&
				null != _uiLabelCharacterName && 
				null != dialogueBody &&
				null != bubbleCorner &&
				null != arrow &&
				null != _uiSpriteSpeechTail &&
				null != thoughtBubbleContainer &&
				null != _uiSpriteThoughtTail &&
				null != _uiPanel);
	}

	private void CacheComponents()
	{
		if (null != Camera.main)
		{
			_cameraComponent = Camera.main.gameObject.GetComponent<CameraBase>();
		}

		if (null != characterName)
		{
			_uiLabelCharacterName = characterName.GetComponent<UILabel>();
		}

		if (null != dialogueBody)
		{
			_uiLabelDialogueBody = dialogueBody.GetComponent<UILabel>();
		}

		if (null != speechTail)
		{
			_uiSpriteSpeechTail = speechTail.GetComponent<UISprite>();
		}

		if (null != thoughtTail)
		{
			_uiSpriteThoughtTail = thoughtTail.GetComponent<UISprite>();
		}

		_uiPanel = GetComponent<UIPanel>();
	}

	private void OnInteractionStart(Dialogue.DialogueData interactionContainer)
	{
		_hasStarted = false;
		CacheComponents();

		if (AreAllComponentsCached())
		{
			_initialLocalTailY = speechTail.transform.localPosition.y;
			_initialLocalThoughtTailY = thoughtTail.transform.localPosition.y;

			DeactivateImediate();
		}

		Dialogue.GetActiveInteraction().PreStart(ref interactionContainer);

		if (null != screenBlocker)
		{
			NGUITools.SetActive(screenBlocker, true);
		}
	}

	private void Advance(Dialogue.DialogueData.DialoguePoint conversation)
	{
		_currentPoint = conversation;
		if (null != conversation)
		{
			if (AreAllComponentsCached())
			{
				if (_cameraComponent.IsCameraTransitioning()) // the camera will be transitioning if a new camera was added (apart from cuts) (or it was already transitioning)
				{
					_waitingForCameraTransition = true;
					ActivateBubbleContainer(false);					
				}
				else
				{
					SetUpNewConversation(conversation);
				}
			}
		}
	}

	private void DeactivateImediate()
	{
		if (closeTween != null) // Stop closing tweens that might be running
		{
			closeTween.onFinished -= OnCloseTweenFinished;
			closeTween.ResetToBeginning();
		}

		if (openTween != null)
		{
			openTween.ResetToBeginning();
		}

		NGUITools.SetActive(bubbleContainer, false);
	}

	private void ActivateBubbleContainer(bool activate)
	{
		if (activate)
		{			
			bool doTweenOn = false;
			if (bubbleContainer.activeSelf) 
			{
				if (closeTween != null && closeTween.IsRunning) // Stop closing tweens that might be running
				{
					closeTween.onFinished -= OnCloseTweenFinished;
					closeTween.ResetToBeginning();
					doTweenOn = true;
				}				
			}
			else
			{
				NGUITools.SetActive(bubbleContainer, true);
				doTweenOn = true;
			}

			if (doTweenOn)
			{
				// play open tween
				if (openTween == null)
				{
					openTween = new UITweenActivator(bubbleContainer, 0, false);
				}
				openTween.PlayForward();
			}
		}
		else if (bubbleContainer.activeSelf)
		{
			// play end tween

			if (closeTween == null)
			{
				closeTween = new UITweenActivator(bubbleContainer, 1, false);
			}

			if (!closeTween.IsRunning)
			{
				closeTween.onFinished += OnCloseTweenFinished;
				closeTween.PlayForward();
			}
		}
	}

	private void OnCloseTweenFinished()
	{
		closeTween.onFinished -= OnCloseTweenFinished;
		NGUITools.SetActive(bubbleContainer, false);
	}

	private void SetUpNewConversation(Dialogue.DialogueData.DialoguePoint conversation)
	{
		FusionAudio.PostEvent(FusionAudio.eEvent.DLG_common);
		ActivateBubbleContainer(true);
		SetCharacterName(conversation.caller);
		
		_uiLabelDialogueBody.text = StringUtils.ReplaceTokens(conversation.call);
		NGUITools.SetActive(_uiLabelDialogueBody.gameObject, true); // force a refresh (ensures text is rendered)

		ToggleBubbleType(conversation);

		Vector3 outPosition = new Vector3();
		Quaternion outRotation = new Quaternion();
		GetLatestCameraTransform(conversation, ref outPosition, ref outRotation);
		PositionBubbleHorizontally(conversation.caller, outPosition, outRotation);
		PositionBubbleTailHorizontally(conversation.caller, outPosition, outRotation);

		_doRefreshPanel = true;
	}

	// turn on/off the visuals for speech/thought bubbles
	private void ToggleBubbleType(Dialogue.DialogueData.DialoguePoint conversation)
	{
		bool areThoughtBubbleSpritesRequired = false;
		switch (conversation.bubbleType)
		{
			case Dialogue.DialogueData.DialoguePoint.ConversationBubble.speech:
				break; // thought bubble sprites will default to off
			case Dialogue.DialogueData.DialoguePoint.ConversationBubble.thought:
				areThoughtBubbleSpritesRequired = true;
				break;
			default:
				EB.Debug.LogError("DialogueHUDLogic.SetUpNewConversationStep() : Unsupported conversation bubble type");
				break;
		}
		if (null != thoughtBubbleContainer)
		{
			NGUITools.SetActive(thoughtBubbleContainer, areThoughtBubbleSpritesRequired); // turn on thought bubble sprites if required
		}
		if (null != speechTail)
		{
			NGUITools.SetActive(speechTail, !areThoughtBubbleSpritesRequired); // if the thought bubble sprites are required, the speech tail will not be required (and vice-versa)
		}
	}

	// after the dialogue starts cameras, get the transform
	private void GetLatestCameraTransform(Dialogue.DialogueData.DialoguePoint conversation, ref Vector3 outPosition, ref Quaternion outRotation)
	{
		if (Cinematic.IsCinematicActive())
		{
			Cinematic.GetCurrentCinematicTransform(ref outPosition, ref outRotation);
		}
		else
		{
			_cameraComponent.GetNewestGameCameraBehaviorTransform(ref outPosition, ref outRotation);
		}
	}
}
