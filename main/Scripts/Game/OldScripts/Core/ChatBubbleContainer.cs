///////////////////////////////////////////////////////////////////////
//
//  ChatBubbleContainer.cs
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

public class ChatBubbleContainer
{
	public enum ChatBubbleType
	{
		left,
		right,
	}

	static private ChatBubbleContainer _instance = null;

	private class ChatBubble
	{		
		public ChatBubbleType type = ChatBubbleType.left;
		public GameObject bubble;
		public UILabel bubbleLabel;
		public UISprite bubbleSprite;
		public bool isInUseCurrently = false;

		public ChatBubble(GameObject bubble, ChatBubbleType bubbleType)
		{
			this.bubble = bubble;
			type = bubbleType;

			const string ChatBubblePanelName = "ChatBubblePanel";
			Transform chatBubblePanel = this.bubble.transform.Find(ChatBubblePanelName);

			if (chatBubblePanel)
			{
				const string SubSpriteName = "Sprite";
				Transform spriteTransform = chatBubblePanel.Find(SubSpriteName);
				if (spriteTransform)
				{
					bubbleSprite = spriteTransform.gameObject.GetComponent<UISprite>();
				}

				const string SubLabelName = "Label";
				Transform labelTransform = chatBubblePanel.Find(SubLabelName);
				if (labelTransform)
				{
					bubbleLabel = labelTransform.gameObject.GetComponent<UILabel>();
				}
			}
		}
	}
	private List<ChatBubble> _allChatBubbles = new List<ChatBubble>();

	// get the instance of ChatBubbleContainer, or create it
	public static ChatBubbleContainer Instance()
	{
		if (null == _instance)
		{
			_instance = new ChatBubbleContainer();
		}
		return _instance;
	}

	// check that we have a minium amount of chat bubbles for use, remove deleted bubbles, and add new ones as appropriate
	public void EnsureMinimumAmountOfChatBubblesExist()
	{
		CullDeletedChatBubblesFromPool();
					
		const int MinimumRequiredChatBubblesOfType = 2; // unlikely to require more than this amount of simultaneous chat bubbles active
		while (CalculateNumChatBubblesOfType(ChatBubbleType.left) < MinimumRequiredChatBubblesOfType)
		{
			if (!CreateSingleChatBubble(ChatBubbleType.left)) // we failed to create a chat bubble
			{
				break; // break out of the loop as the create failed meaning this loop will become infinite
			}
		}

		while (CalculateNumChatBubblesOfType(ChatBubbleType.right) < MinimumRequiredChatBubblesOfType)
		{
			if (!CreateSingleChatBubble(ChatBubbleType.right)) // we failed to create a chat bubble
			{
				break; // break out of the loop as the create failed meaning this loop will become infinite
			}
		}
	}

	// notify the container that a bubble is required
	public void StartUsingChatBubble(ChatBubbleType type, ref GameObject refChatBubble, ref UILabel refChatBubbleLabel, ref UISprite refChatBubbleSprite)
	{
		// go over all the bubbles looking for one not being used currently
		for (int chatBubbleIndex = 0; chatBubbleIndex < _allChatBubbles.Count; ++chatBubbleIndex)
		{
			if (!_allChatBubbles[chatBubbleIndex].isInUseCurrently && type == _allChatBubbles[chatBubbleIndex].type)
			{
				_allChatBubbles[chatBubbleIndex].isInUseCurrently = true;				
				refChatBubble = _allChatBubbles[chatBubbleIndex].bubble;
				refChatBubbleLabel = _allChatBubbles[chatBubbleIndex].bubbleLabel;		 
				refChatBubbleSprite = _allChatBubbles[chatBubbleIndex].bubbleSprite;
				 return;
			}
		}

		// all our chat bubbles are being used, so we need to create a new one
		if (CreateSingleChatBubble(type))
		{
			// the new one just created will be on the end of the list
			_allChatBubbles[_allChatBubbles.Count - 1].isInUseCurrently = true;
			refChatBubble = _allChatBubbles[_allChatBubbles.Count - 1].bubble;
			refChatBubbleLabel = _allChatBubbles[_allChatBubbles.Count - 1].bubbleLabel;
			refChatBubbleSprite = _allChatBubbles[_allChatBubbles.Count - 1].bubbleSprite;
		}
	}

	public void StopUsingChatBubble(GameObject chatBubble)
	{
		for (int chatBubbleIndex = 0; chatBubbleIndex < _allChatBubbles.Count; ++chatBubbleIndex)
		{
			if (_allChatBubbles[chatBubbleIndex].isInUseCurrently && chatBubble == _allChatBubbles[chatBubbleIndex].bubble)
			{
				UITweenActivator activator = new UITweenActivator(chatBubble, 1);
				activator.onFinished += () =>
				{
					_allChatBubbles[chatBubbleIndex].isInUseCurrently = false;
					NGUITools.SetActive(_allChatBubbles[chatBubbleIndex].bubble, _allChatBubbles[chatBubbleIndex].isInUseCurrently);
				};
				activator.PlayForward();

				return;
			}
		}
		EB.Debug.LogWarning("ChatBubbleContainer.StopUsingChatBubble() chatBubble could not be found");
	}

	// how many of the specified type of chat bubble exist
	private int CalculateNumChatBubblesOfType(ChatBubbleType type)
	{
		int count = 0;
		for (int chatBubbleIndex = 0; chatBubbleIndex < _allChatBubbles.Count; ++chatBubbleIndex)
		{
			if (type == _allChatBubbles[chatBubbleIndex].type)
			{
				++count;
			}
		}
		return count;
	}

	// remove deleted chat bubbles from pool
	private void CullDeletedChatBubblesFromPool()
	{
		for (int chatBubbleIndex = 0; chatBubbleIndex < _allChatBubbles.Count; )
		{
			if (null == _allChatBubbles[chatBubbleIndex].bubble)
			{
				_allChatBubbles.RemoveAt(chatBubbleIndex);
			}
			else
			{
				++chatBubbleIndex;
			}
		}
	}

	// create a chat bubble
	private bool CreateSingleChatBubble(ChatBubbleType type)
	{		
		//TODO: 弃用，后续厚超将弃用的聊天相关代码文件删除。	
		// GameObject newChatBubbleObject = null;
		// switch(type)
		// {
		// 	case ChatBubbleType.left:
		// 		const string ChatBubblePrefabLeft = "UI/UI_ChatBubble";
		// 		newChatBubbleObject = UIHierarchyHelper.Instance.LoadAndPlace(ChatBubblePrefabLeft, UIHierarchyHelper.eUIType.HUD_Dynamic, null); // UIHierarchyHelper is only used on the MainHudUI
		// 		break;
		// 	case ChatBubbleType.right:
		// 		const string ChatBubblePrefabRight = "UI/UI_ChatBubble_Pixie";
		// 		newChatBubbleObject = UIHierarchyHelper.Instance.LoadAndPlace(ChatBubblePrefabRight, UIHierarchyHelper.eUIType.HUD_Dynamic, null); // UIHierarchyHelper is only used on the MainHudUI
		// 		break;
		// 	default:
		// 		EB.Debug.LogError("ChatBubbleContainer.CreateSingleChatBubble() : Unsupported chat bubble type");
		// 		break;
		// }

		// if (null != newChatBubbleObject)
		// {
		// 	_allChatBubbles.Add(new ChatBubble(newChatBubbleObject, type));
		// 	NGUITools.SetActive(newChatBubbleObject, false);
		// 	return true;
		// }
		return false;
	}
}
