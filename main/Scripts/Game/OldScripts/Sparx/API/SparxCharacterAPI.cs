///////////////////////////////////////////////////////////////////////
//
//  SparxCharacterAPI.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections;

namespace EB.Sparx
{	
	public class CharacterAPI : SparxAPI
	{
		public enum eErrorCodes 
		{
			Unknown = -1,
			InsufficientStamina = 1
		}

		public CharacterAPI( EndPoint api ) : base(api)
		{
		}

		public void Create( Hashtable properties, Hashtable generalRecord, System.Action<eResponseCode, Hashtable> callback )
		{
			var request = endPoint.Post("/character/create");

			request.AddData("properties", properties);
			request.AddData("generalRecord", generalRecord);

			LoadingSpinner.Show();

			endPoint.Service(request, delegate(Response result)
			{
				LoadingSpinner.Hide();
				if ( result.sucessful )
				{
					callback(eResponseCode.Success, result.hashtable);
					return;
				}
				
				callback(CheckError(result.error.ToString()), null);
			});
		}

		public void Delete( int characterId, System.Action<eResponseCode> callback )
		{
			var request = endPoint.Post("/character/delete");
			
			request.AddData("characterId", characterId);
			
			endPoint.Service(request, delegate(Response result)
			{
				if ( result.sucessful )
				{
					callback(eResponseCode.Success);
					return;
				}
				
				callback(CheckError(result.error.ToString()));
			});
		}

		public void GetAll(System.Action<eResponseCode, Hashtable> callback )
		{
			var request = endPoint.Get("/character/getAll");

			endPoint.Service(request, delegate(Response result)
			{
				if ( result.sucessful )
				{
					callback(eResponseCode.Success, result.hashtable);
					return;
				}
				
				callback(CheckError(result.error.ToString()), null);
			});
		}

		public void Open( int characterId, bool blockUI, System.Action<eResponseCode, Hashtable> callback )
		{
			var request = endPoint.Post("/character/open");
			
			request.AddData("characterId", characterId);
			
			if (blockUI)
			{
				LoadingSpinner.Show();
			}
			endPoint.Service(request, delegate(Response result)
			{
				if (blockUI)
				{
					LoadingSpinner.Hide();
				}
				
				if ( result.sucessful )
				{
					callback(eResponseCode.Success, result.hashtable);
					return;
				}
				
				callback(CheckError(result.error.ToString()), null);
			});
		}

		public void RefillStamina( int characterId, System.Action<eResponseCode, Hashtable> callback )
		{
			var request = endPoint.Post("/character/refillStamina");

			request.AddData("characterId", characterId);

			LoadingSpinner.Show();
			endPoint.Service(request, delegate(Response result)
			{
				LoadingSpinner.Hide();
				if ( result.sucessful )
				{
					callback(eResponseCode.Success, result.hashtable);
					return;
				}

				callback(CheckError(result.error.ToString()), null);
			});
		}

		public void Revive( int characterId, System.Action<eResponseCode, Hashtable> callback ) 
		{
			var request = endPoint.Post("/character/revive");

			request.AddData("characterId", characterId);

			LoadingSpinner.Show();
			endPoint.Service(request, delegate(Response result)
			{
				LoadingSpinner.Hide();
				if (result.sucessful)
				{
					callback(eResponseCode.Success, result.hashtable);
					return;
				}

				callback(CheckError(result.error.ToString()), null);
			});
		}

		public void DebugSetGold( int characterId, int gold ) 
		{
			var request = endPoint.Post("/character/debugSetGold");

			request.AddData("characterId", characterId);
			request.AddData("gold", gold);

			endPoint.Service(request, delegate(Response result)
			{
				if (result.hashtable.ContainsKey("gold"))
				{
					CharacterManager.Instance.CurrentCharacter.SparxCharacterData.Gold = EB.Dot.Integer("gold", result.hashtable, 0);
				}
			});
		}

		public void DebugSetLevel( int characterId, int level, int xp, System.Action callback) 
		{
			var request = endPoint.Post("/character/debugSetLevel");

			request.AddData("characterId", characterId);
			request.AddData("level", level);
			request.AddData("xp", xp);

			endPoint.Service(request, delegate(Response result)
			{
				if (result.hashtable.ContainsKey("level"))
				{
					CharacterManager.Instance.CurrentCharacter.GeneralRecord.level = EB.Dot.Integer("level", result.hashtable, 0);
					CharacterManager.Instance.CurrentCharacter.GeneralRecord.xp = EB.Dot.Integer("xp", result.hashtable, 0);
				}
				callback();
			});
		}

		public void Update( int characterId, Hashtable properties, bool blockUI, System.Action<eResponseCode, Hashtable> callback ) 
		{
			var request = endPoint.Post("/character/update");
			
			request.AddData("characterId", characterId);
			request.AddData("properties", properties);
			
			if (blockUI)
			{
				LoadingSpinner.Show();
			}
			endPoint.Service(request, delegate(Response result)
			{
				if (blockUI)
				{
					LoadingSpinner.Hide();
				}
				if (result.sucessful)
				{
					callback(eResponseCode.Success, result.hashtable);
					return;
				}
				
				callback(CheckError(result.error.ToString()), null);
			});
		}
	}
}
