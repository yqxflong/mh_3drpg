///////////////////////////////////////////////////////////////////////
//
//  AStarPathfindingWalkableArea.cs
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

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class AStarPathfindingWalkableArea : BaseComponent
{
	public void EnableRendererComponent(bool enable)
	{
		MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
		if (null != renderer)
		{
			renderer.enabled = enable;
		}
	}
}
