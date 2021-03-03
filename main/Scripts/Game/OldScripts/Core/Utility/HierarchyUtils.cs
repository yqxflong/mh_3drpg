///////////////////////////////////////////////////////////////////////
//
//  HierarchyUtils.cs
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

public class HierarchyUtils
{
    public static T GetComponentInParent<T>(Transform go) where T : Component
    {
        if (go == null || go.parent == null)
        {
            return null;
        }

        T comp = go.parent.GetComponent<T>();
        if (comp != null)
        {
            return comp;
        }
        return GetComponentInParent<T>(go.parent);
    }

	public static string GetTransformPath(Transform t, char separator = '/')
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder(t.name);
		Transform current = t;
		while (current.parent != null)
		{
			sb.Insert(0, separator);
			sb.Insert(0, current.parent.name);
			current = current.parent;
		}
		return sb.ToString();
	}

	public static string GetGameObjectPath(GameObject go, char separator = '/')
	{
		if (go != null)
		{
			return GetTransformPath(go.transform);
		}
		else
		{
			return "";
		}
	}

	public static int IndexOfChild(Transform parent, Transform child)
	{
		for (int i = 0; i < parent.childCount; i++)
		{
			if (parent.GetChild(i) == child)
			{
				return i;
			}
		}

		return -1;
	}

    public static System.Reflection.Assembly GetAssembly()
    {
        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        return assembly;
    }

}
