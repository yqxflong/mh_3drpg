///////////////////////////////////////////////////////////////////////
//
//  ShowChatBubble.cs
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

public class ShowChatBubble : MonoBehaviour 
{
	public ChatBubbleContainer.ChatBubbleType bubbleTypeRequired = ChatBubbleContainer.ChatBubbleType.left; 

	private GameObject _chatBubble = null;
	private UILabel _chatBubbleLabel = null;
	private UISprite _chatBubbleSprite = null;

	private Camera _gameCam;
	private Camera _UICam;
	private float _displayStartTime = -1f;
	private float _offsetToTopOfCharactersHead = 0f; // this is calculated, it's the offset from the characters feet
	
	private const float kDisplayTimeSeconds = 4f;

	//private InventoryComponent _inventory;

	// show the specified text
	public void Show(string chatText)
	{
		if (null == _chatBubbleLabel)
		{
			ChatBubbleContainer.Instance().StartUsingChatBubble(bubbleTypeRequired, ref _chatBubble, ref _chatBubbleLabel, ref _chatBubbleSprite);

			if (!_chatBubble || !_chatBubbleLabel || !_chatBubbleSprite) // if we are missing any of the objects we need
			{
				Hide();
			}
		}

		if (null != _chatBubbleLabel)
		{
			_chatBubbleLabel.text = StringUtils.ReplaceTokens(chatText);

			bool hasColorBeenSet = false;
			//if (_inventory) // we only cache the inventory component for the player (see Start function)
			//{
				//DyeModel dye = _inventory.GetArmorDye();
				//if (dye)
				//{
				//	_chatBubbleLabel.color = new Color(dye.displayColor.r, dye.displayColor.g, dye.displayColor.b, 1f);
				//	hasColorBeenSet = true;
				//}
			//}

			if (!hasColorBeenSet)
			{
				CharacterModel model = GameUtils.GetCharacterModel(gameObject);
				if (null != model)
				{
					_chatBubbleLabel.color = model.dialogueTextColor;
				}
			}

			_displayStartTime = Time.time;

			CalculateOffsetToTopOfCharactersHead();
		}		
	}

	// we're finished with the chat bubble
	public void Hide()
	{
		if (null != _chatBubble)
		{
			ChatBubbleContainer.Instance().StopUsingChatBubble(_chatBubble);
			_chatBubble = null;
		}
		_chatBubbleLabel = null;
		_chatBubbleSprite = null;
	}

	private void Start() 
	{
		_gameCam = Camera.main;
		_UICam = UICamera.mainCamera;

		if (PlayerManager.IsLocalPlayer(gameObject))
		{
			ChatBubbleContainer.Instance().EnsureMinimumAmountOfChatBubblesExist();
		}

		CalculateOffsetToTopOfCharactersHead();

		if (PlayerManager.IsPlayer(gameObject)) // we only need the inventory component for the player
		{
			//_inventory = GetComponent<InventoryComponent>();
		}
	}

	private void OnDisable()
	{
		if (null != _chatBubbleLabel)
		{
			Hide();
		}
	}

	private void LateUpdate()
	{
		if (null == _chatBubbleLabel)
		{
			return;
		}

		if ((Time.time - _displayStartTime) > kDisplayTimeSeconds)
		{
			Hide();
			return;
		}

		if (_UICam == null)
		{
			_UICam = UICamera.mainCamera;
		}

		if (_gameCam == null)
		{
			_gameCam = Camera.main;
		}

		if (null == _UICam || null == _gameCam)
		{
			return;
		}

		float horizontalOffset = 0f;
		switch (bubbleTypeRequired)
		{
			case ChatBubbleContainer.ChatBubbleType.left:
				horizontalOffset = -0.5f;
				break;
			case ChatBubbleContainer.ChatBubbleType.right:
				horizontalOffset = 0.5f;
				break;
			default: break;
		}

		Vector3 screenPoint = _gameCam.WorldToScreenPoint(transform.position + Vector3.up * _offsetToTopOfCharactersHead);
		Vector3 bubblePosWorldPoint = _UICam.ScreenToWorldPoint(screenPoint);
		bubblePosWorldPoint.z = 0f;

		if (_chatBubble)
		{
			_chatBubble.transform.position = bubblePosWorldPoint;
			_chatBubble.transform.localPosition += new Vector3(_chatBubbleLabel.localSize.x * horizontalOffset, -_chatBubbleSprite.bottomAnchor.absolute, 0f);
			if (!_chatBubble.activeSelf)
			{
				NGUITools.SetActive(_chatBubble, true);
				UITweenActivator activator = new UITweenActivator(_chatBubble);
				activator.PlayForward();
				//FusionAudio.PostEvent(FusionAudio.eEvent.DLG_common);
			}
		}
	}

	// Calculate the offset to top of the charactersHead
	private void CalculateOffsetToTopOfCharactersHead()
	{
		const string HUDAnchor = "HUDAnchor";

		Transform gameObjectTransform = gameObject.transform;
		Transform HUDAnchorTransform = gameObjectTransform.Find(HUDAnchor);
		if (null != HUDAnchorTransform)
		{
			_offsetToTopOfCharactersHead = HUDAnchorTransform.position.y - gameObjectTransform.position.y;
		}
		else
		{
			Bounds combinedBounds = new Bounds();
			GameUtils.CalculateRenderBounds(gameObject, ref combinedBounds);
			_offsetToTopOfCharactersHead = combinedBounds.extents.y;
		}
	}
}
