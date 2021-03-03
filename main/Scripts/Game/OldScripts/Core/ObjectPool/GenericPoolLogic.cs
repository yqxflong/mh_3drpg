///////////////////////////////////////////////////////////////////////
//
//  GenericPoolLogic.cs
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

public class GenericPoolLogic : MonoBehaviour, IPoolable
{
	void Awake()
	{
		// DontDestroyOnLoad(this);
	}
	
	public void OnPoolActivate()
	{
		// EB.Debug.Log ("GenericPoolLogic::OnPoolActivate");
	}
	
	public void OnPoolDeactivate()
	{
		// EB.Debug.Log ("GenericPoolLogic::OnPoolDeactivate");
	}	
}
