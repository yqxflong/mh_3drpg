///////////////////////////////////////////////////////////////////////
//
//  InteractionSetHelper.cs
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

public class InteractionSetHelper : MonoBehaviour 
{
    [HideInInspector]
    public GameObject selectedObject;

	//private bool _isShowing = true;

    void OnDrawGizmos()
    {
		if (transform.parent == null || transform.parent.parent == null)
		{
			return;
		}
		ZoneHelper zoneHelper = transform.parent.parent.GetComponent<ZoneHelper>();
		if (zoneHelper == null)
		{
			return;
		}

		// tsl: removed hiding for now @ LD's request (since we only have a single interaction set per zone)
		ShowInteractionSet(true);

//		if (zoneHelper.ActiveInteractionSet == transform && !_isShowing)
//        {
//            ShowInteractionSet(true);
//        }
//        else if (zoneHelper.ActiveInteractionSet != transform && _isShowing)
//        {
//            // ShowInteractionSet(false);
//        }
    }

    private void ShowInteractionSet(bool show)
    {
		if (!Application.isPlaying)
		{
			//_isShowing = show;
			foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
			{
				renderer.enabled = show;
			}
		}
    }
}
