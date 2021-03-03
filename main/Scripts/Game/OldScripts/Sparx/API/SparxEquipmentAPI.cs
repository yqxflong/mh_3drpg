///////////////////////////////////////////////////////////////////////
//
//  EquipmentAPI.cs
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

namespace EB.Sparx
{
	public class EquipmentAPI : SparxAPI
	{
		EB.Sparx.Data _data;

		public EquipmentAPI(EndPoint api) : base(api)
		{
		}

		public void Update(int characterId, Hashtable itemRecord, System.Action<eResponseCode, Hashtable> callback = null, bool force = false) 
		{
			CharacterRecord currentRecord = CharacterManager.Instance.CurrentCharacter;
			if (characterId != currentRecord.Id) 
			{
				EB.Debug.LogError("Cannot update equipment on a character you are not currently playing");
			}

			var request = endPoint.Post("/equipment/update");
			if (force) {
				request = endPoint.Post("/equipment/forceUpdate");
			}

			request.AddData("characterId", characterId);
			request.AddData("record", itemRecord);

			endPoint.Service(request, delegate(Response response) {
				if (response.sucessful) 
				{
					Hashtable responseRecord = EB.Dot.Object("record", response.hashtable, Johny.HashtablePool.Claim());
					currentRecord.EquipmentRecord.Load(responseRecord);
					if (callback != null)
					{
						callback(eResponseCode.Success, responseRecord);
					}	
				}
				else
				{
					if (callback != null)
					{
						callback(CheckError(response.error.ToString()), null);
					}
				}
			});
		}

		public void BuySlots(int characterId, System.Action<eResponseCode, int, int> callback)
		{
			CharacterRecord currentRecord = CharacterManager.Instance.CurrentCharacter;
			if (characterId != currentRecord.Id) 
			{
				EB.Debug.LogError("Cannot buy item slots on a character you are not currently playing");
				return;
			}

			var request = endPoint.Post("/equipment/buySlots");
			request.AddData("characterId", characterId);

			LoadingSpinner.Show();
			endPoint.Service(request, delegate (Response response) {
				LoadingSpinner.Hide();
				if (response.sucessful)
				{
					int maxItems = EB.Dot.Integer("maxItems", response.hashtable, currentRecord.EquipmentRecord.maxItems);
					int hardCurrency = EB.Dot.Integer("hardCurrency", response.hashtable, ProfileManager.Instance.HardCurrency);

					currentRecord.EquipmentRecord.maxItems = maxItems;
					ProfileManager.Instance.HardCurrency = hardCurrency;

					DebugSystem.Log("Item slots: " + maxItems);
					DebugSystem.Log("Hard currency: " + hardCurrency);

					if (callback != null)
					{
						callback(eResponseCode.Success, maxItems, hardCurrency);
					}	
				}
				else
				{
					if (callback != null)
					{
						callback(CheckError(response.error.ToString()), 0, 0);
					}
				}
			});
		}
	}
}