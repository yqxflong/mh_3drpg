///////////////////////////////////////////////////////////////////////
//
//  ComponentUpdateManager.cs
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

public static class ComponentUpdateManager
{
	public static Coroutine StartUpdate(MonoBehaviour component, System.Action update,  int numPerSecond)
	{
		return StartUpdate(component, update, 1f / numPerSecond);
	}

	public static Coroutine StartUpdate(MonoBehaviour component, System.Action update, float delay)
	{
		return component.StartCoroutine(InternalCoroutine(delay, update));
	}

	private static IEnumerator InternalCoroutine(float delay, System.Action update)
	{
		WaitForSeconds wait = new WaitForSeconds(delay);

		while (true)
		{
			yield return wait;
			update();
		}	
	}
}