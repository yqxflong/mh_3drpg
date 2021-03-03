///////////////////////////////////////////////////////////////////////
//
//  SparxCharacterData.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EB.Sparx
{
	public class CharacterData
	{
		public int Id { get; private set; }
		public string PortraitId { get; set; }
		public List<Hashtable> Inventory { get; private set; }
		public Hashtable Properties { get; private set; }

		public Hashtable Equipment { get; private set; }
		public Hashtable Spirits { get; private set; }
		public Hashtable General { get; private set; }

		public float FractionalStamina { get; set; }
		public int MaxStamina { get; set; }
		public int LastStaminaTick { get; set; }

		public int CreatedOn { get; set; }
		public int LastDungeonCompleted { get; set; }

		private int _gold;
		public int Gold 
		{
			get
			{
				return _gold;
			}
			set
			{
				if (value != _gold)
				{
					_gold = value;
					//EventManager.instance.Raise(new GoldChangedEvent(_gold));
				}
			}
		}

		public int Stamina 
		{
			get
			{
				double timeDiff = Time.Now - LastStaminaTick;

				FractionalStamina += (1.0f / GlobalBalanceData.Instance.regenInterval) * (float)timeDiff; 
				FractionalStamina = Mathf.Min(MaxStamina, FractionalStamina);

				LastStaminaTick = Time.Now;

				return (int)Mathf.Floor(FractionalStamina);
			}
		}

		public float NormalizedStamina
		{
			get
			{
				return (float)Stamina / MaxStamina;
			}
		}

		public CharacterData( Hashtable data )
		{
			Id = Dot.Integer("characterId", data, -1);

			if ( Id < 0 )
			{
				return;
			}

			PortraitId = Dot.String("portraitId", data, null);

			if ( "" == PortraitId )
			{
				PortraitId = null;
			}

			Inventory = new List<Hashtable>();
			Properties = Johny.HashtablePool.Claim();
			MaxStamina = 0;
			FractionalStamina = 0.0f;
			LastStaminaTick = 0;

			FillInventory(data);
			FillProperties(data);
			FillStats(data);

			CreatedOn = EB.Dot.Integer("createdOn", data, 0);
			LastDungeonCompleted = EB.Dot.Integer("lastDungeonCompleted", data, 0);
		}

		public void FillInventory( Hashtable data )
		{
			ArrayList rawInventory = Dot.Array("inventory", data, null);

			if ( null == rawInventory )
			{
				return;
			}

			foreach (Hashtable item in rawInventory)
			{
				Inventory.Add(item);
			}
		}

		public void FillProperties( Hashtable data )
		{
			Hashtable properties = Dot.Object("properties", data, null);

			if ( null == properties )
			{
				return;
			}

			Properties = properties;
		}

		public void FillStats(Hashtable data )
		{
			Hashtable stats = Dot.Object("stats", data, null);
			
			if ( null == stats )
			{
				return;
			}
			
			Hashtable stamina = Dot.Object("stamina", stats, null);
			
			if ( null != stamina )
			{
				MaxStamina = Dot.Integer("max", stamina, 0);
				FractionalStamina = Dot.Single("current", stamina, 0.0f);
				LastStaminaTick = Time.Now;
			}
			
			Gold = Dot.Integer("gold", stats, 0);
			Equipment = Dot.Object("equipment", stats, Johny.HashtablePool.Claim());
			Spirits = Dot.Object("spirits", stats, Johny.HashtablePool.Claim());
			General = Dot.Object("general", stats, Johny.HashtablePool.Claim());
		}
	}
}
