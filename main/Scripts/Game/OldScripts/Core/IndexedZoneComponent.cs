///////////////////////////////////////////////////////////////////////
//
//  IndexedZoneComponent.cs
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

/// <summary>
/// 区域索引组件（但是在游戏里面已经没有再使用）
/// </summary>
public class IndexedZoneComponent : MonoBehaviour
{
	[SerializeField]
	private int _index;

	public int Index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = value;
		}
	}

	public static Dictionary<int, GameObject> FindIndexedComponents(Transform root) 
	{
		Dictionary<int, GameObject> allComponents = new Dictionary<int, GameObject>();

		EB.Collections.Queue<Transform> queue = new EB.Collections.Queue<Transform>(16);
		queue.Enqueue(root);
		while (queue.Count > 0)
		{
			Transform it = queue.Dequeue();
			IndexedZoneComponent indexer = it.GetComponent<IndexedZoneComponent>();

			if (indexer != null)
			{
				allComponents.Add(indexer.Index, indexer.gameObject);
			}
			
			int childCount = it.childCount;
			for (int i = 0; i < childCount; ++i)
			{
				queue.Enqueue(it.GetChild(i));
			}
		}

		return allComponents;
	}
}
