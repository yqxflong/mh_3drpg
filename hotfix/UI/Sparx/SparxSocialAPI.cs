///////////////////////////////////////////////////////////////////////
//
//  SparxSocialAPI.cs
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
using EB.Sparx;

namespace Hotfix_LT.UI 
{
	public class SocialAPI : SparxAPI
	{
		EndPoint _social;

		public SocialAPI()
		{
			endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
		}

		public void FindUidByName(string name, System.Action<eResponseCode, Hashtable> callback)
		{
			Request request = endPoint.Post("/social/findUidByName");

			request.AddData("name", name);

			endPoint.Service(request, delegate (Response result)
			{
				if (result.sucessful)
				{
					callback(eResponseCode.Success, result.hashtable);
					return;
				}

				callback(CheckError(result.error.ToString()), null);
			});
		}

		public void GetAdHocPortraitInfo(List<Id> uidList, System.Action<eResponseCode, Hashtable> callback)
		{
			Request request = endPoint.Get("/social/getAdHocPortraitInfo");

			request.AddData("uidList", uidList);

			endPoint.Service(request, delegate (Response result)
			{
				if (result.sucessful)
				{
					callback(eResponseCode.Success, result.hashtable);
					return;
				}

				callback(CheckError(result.error.ToString()), null);
			});
		}

		public void GetFriendsPortraitInfo(System.Action<eResponseCode, Hashtable> callback)
		{
			Request request = endPoint.Get("/social/getFriendsPortraitInfo");

			endPoint.Service(request, delegate (Response result)
			{
				if (result.sucessful)
				{
					callback(eResponseCode.Success, result.hashtable);
					return;
				}

				callback(CheckError(result.error.ToString()), null);
			});
		}

		public void SetSocialEndpoint(EndPoint social)
		{
			_social = social;
		}

		public void UploadPortrait(int characterId, string portraitInBase64, System.Action<eResponseCode, Hashtable> callback)
		{
			Request request = _social.Post("/social/uploadPortrait");

			request.AddData("characterId", characterId);
			request.AddData("portrait", portraitInBase64);

			_social.Service(request, delegate (Response result)
			{
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
