///////////////////////////////////////////////////////////////////////
//
//  UITweenActivator.cs
//
//  Copyright (c) 2006-2014 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITweenActivator
{
	public UITweener[] Tweeners { get; private set; }
	public int Group { get; private set; }
	public bool IsRunning
	{
		get
		{
			return _activeTweeners.Count > 0;
		}
	}

	public event System.Action onFinished;

	private List<UITweener> _activeTweeners = new List<UITweener>();

	[ContextMenu("Play Forward")]
	public void PlayForward()
	{
		_activeTweeners.Clear();
		for (int i = 0; i < Tweeners.Length; i++)
		{
			if (Tweeners[i].tweenGroup == Group)
			{
				EventDelegate.Add(Tweeners[i].onFinished, OnFinished);
				_activeTweeners.Add(Tweeners[i]);
				Tweeners[i].tweenFactor = 0.0f;
				Tweeners[i].PlayForward();
			}
		}
		if (_activeTweeners.Count == 0 && onFinished != null)
		{
			onFinished();
		}
	}

	public void PlayReverse()
	{
		_activeTweeners.Clear();
		for (int i = 0; i < Tweeners.Length; i++)
		{
			if (Tweeners[i].tweenGroup == Group)
			{
				EventDelegate.Add(Tweeners[i].onFinished, OnFinished);
				_activeTweeners.Add(Tweeners[i]);
				Tweeners[i].tweenFactor = 1.0f;
				Tweeners[i].PlayReverse();
			}
		}
	}

	public void ResetToBeginning()
	{
		_activeTweeners.Clear();
		for (int i = 0; i < Tweeners.Length; i++)
		{
			if (Tweeners[i].tweenGroup == Group)
			{
				Tweeners[i].ResetToBeginning();
				Tweeners[i].enabled = false;
			}
		}
	}

	public UITweenActivator(GameObject target, int group = 0, bool includeChildren = true)
	{
		Group = group;
		Tweeners = includeChildren ? target.GetComponentsInChildren<UITweener>() : target.GetComponents<UITweener>();
	}

	void OnFinished()
	{
		_activeTweeners.Remove(UITweener.current);
		if (_activeTweeners.Count == 0 && onFinished != null)
		{
			onFinished();
		}
	}

	public void SetDelayTime(float delayTime)
	{
		foreach(UITweener tweener in Tweeners)
		{
			if (tweener.tweenGroup == Group)
			{
				tweener.delay = delayTime;
			}
		}
	}

	public List<GameObject> GetAllGameObjects()
	{
		List<GameObject> tweenerGameObjects = new List<GameObject>();

		foreach(UITweener tweener in Tweeners)
		{
			if (tweener.tweenGroup == Group)
			{
				GameObject go = tweener.gameObject;
				
				if(!tweenerGameObjects.Contains(go))
				{
					tweenerGameObjects.Add(go);
				}
			}
		}

		return tweenerGameObjects;
	}

	public void SetGameObjectsInvisible()
	{
		foreach(UITweener tweener in Tweeners)
		{
			if (tweener.tweenGroup == Group)
			{
				UIWidget widget = tweener.gameObject.GetComponent<UIWidget>();
				if(widget != null)
				{
					widget.alpha = 0;
				}

			}
		}
	}
}
