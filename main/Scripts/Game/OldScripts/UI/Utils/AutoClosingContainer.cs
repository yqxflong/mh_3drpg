///////////////////////////////////////////////////////////////////////
//
//  AutoClosingContainer.cs
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

public class AutoClosingContainer : MonoBehaviour
{
	void Update()
	{
		if (transform.childCount == 0)
		{
			Destroy(gameObject);
		}
	}
}
