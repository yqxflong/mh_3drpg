///////////////////////////////////////////////////////////////////////
//
//  TreasureDropComponent.cs
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

public class TreasureDropComponent : MonoBehaviour
{
	//public FlippyComponent.FlippyDistributionArc distributionArc = FlippyComponent.FlippyDistributionArc.FullCircle;

	[HideInInspector]
	public List<int> lootListIDs = new List<int>();
	[SerializeField]
	//[HideInInspector]
	//private List<LootListModel> lootLists = new List<LootListModel>();

	public List<EB.Sparx.SocketData.DropData> PredeterminedDrops { get; set; }

	private InteractableComponent _interactable;
	//private DestructibleComponent _destructible;
	private bool _hasBeenOpened = false;

	void Awake()
	{
		_interactable = GetComponent<InteractableComponent>();
		//_destructible = GetComponent<DestructibleComponent>();
		if (_interactable == null)// && _destructible == null)
		{
			_interactable = gameObject.AddComponent<InteractableComponent>();
		}

		if (_interactable != null)
		{
			_interactable.onInteract += Loot;
		}
		//if (_destructible != null)
		//{
		//	_destructible.onDestroy += Loot;
		//}

		FixIDs();
	}

	public void FixIDs()
	{
		//if (lootLists != null && lootLists.Count > 0)
		//{
		//	List<int> modelIDs = new List<int>(lootLists.Count);
		//	foreach (LootListModel model in lootLists)
		//	{
		//		if (model == null)
		//		{
		//			EB.Debug.LogWarning("Found a null loot list in: " + this);
		//			continue;
		//		}
		//		modelIDs.Add(model.GetId());
		//	}
			
		//	lootListIDs = modelIDs;
		//	lootLists.Clear();
			
		//	if (Application.isEditor && !Application.isPlaying)
		//	{
		//		EB.Debug.LogWarning("TreasureDropComponent: Changed references to IDs");
		//	}
		//}
	}

	public void Loot(GameObject killer)
	{
		if (_hasBeenOpened)
			return;

		if (PredeterminedDrops != null)
		{
			//LootListModel.DropLoot(PredeterminedDrops, transform, PlayerManager.LocalPlayerController().GetComponent<LocomotionComponentAPP>().Simulator, distributionArc);
		}

		if (GetComponent<Animation>() != null)
		{
			GetComponent<Animation>().Play();
		}
		_hasBeenOpened = true;

		//FusionAudio.PostEvent(FusionAudio.eEvent.SFX_Interact_TreasureChestOpen, gameObject);

		if (_interactable != null)
		{
			_interactable.disableInteraction = true;
		}
	}	
}
