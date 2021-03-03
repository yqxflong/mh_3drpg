///////////////////////////////////////////////////////////////////////
//
//  MoveReticleComponent.cs
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

public class MoveReticleComponent : MonoBehaviour
{
	public float reticleScaleAnimation;
	public Projector projector;

	public float CreatureScale
	{
		get; set;
	}

	void Start()
	{
		CreatureScale = 1;
	}

	void Update()
	{
		if (projector != null)
		{
			projector.orthographicSize = CreatureScale * reticleScaleAnimation;
		}
	}
}