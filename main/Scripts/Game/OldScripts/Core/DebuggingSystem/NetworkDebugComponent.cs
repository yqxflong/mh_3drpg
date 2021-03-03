///////////////////////////////////////////////////////////////////////
//
//  NetworkDebugComponent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

#if DEBUG
public class NetworkDebugComponent : BaseComponent, IDebuggableEx 
{
	public bool debugNetworkOwnership = false; // debug draw network ownership information
	public bool turnOnOwnershipTransfer = true;

	public void OnPreviousValuesLoaded()
	{
		turnOnOwnershipTransfer = true;
	}

	public void OnValueChanged(System.Reflection.FieldInfo field, object oldValue, object newValue)
	{
	}

	public void OnDebugGUI()
	{
	}

	public void OnDebugPanelGUI()
	{
	}

	public void OnDrawDebug()
	{
        //TODO:PlayerControllerµ÷ÓÃÆÁ±Î
		/*if (debugNetworkOwnership)
		{
			const float SphereRadius = 0.8f;

			// draw a sphere around all the players to indicate there colors
			foreach (PlayerController pc in PlayerManager.sPlayerControllers)
			{
				Color col = GetPlayerColor(pc);
				GLRenderingUtils.DoDrawSphere(pc.transform.position, SphereRadius, col);
			}

			Vector3 offset = new Vector3(0f, SphereRadius, 0f);
			// draw a sphere around everything with a NetworkOwnershipComponent and a line back to the owner
			NetworkOwnershipComponent[] networkObjects = FindObjectsOfType(typeof(NetworkOwnershipComponent)) as NetworkOwnershipComponent[];
			foreach (NetworkOwnershipComponent netObj in networkObjects)
			{
				PlayerController pc = PlayerManager.GetPlayerController(netObj.GetOwner());
				Color col = GetPlayerColor(pc);

				GLRenderingUtils.DoDrawSphere(netObj.gameObject.transform.position, SphereRadius, col);
				GLRenderingUtils.DoDrawLine(pc.transform.position + offset, netObj.gameObject.transform.position + offset, col);
			}
		}*/
    }
    
}
#endif
