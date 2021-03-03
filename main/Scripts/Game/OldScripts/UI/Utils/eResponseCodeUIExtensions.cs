///////////////////////////////////////////////////////////////////////
//
//  eResponseCodeUIExtensions.cs
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

public static class eResponseCodeUIExtensions
{
	public static string TranslateToUIText(EB.Sparx.eResponseCode response)
	{
		bool isNetworkReachable = Application.internetReachability != NetworkReachability.NotReachable;

		string result = null;
		switch (response)
		{
			case EB.Sparx.eResponseCode.Success:
				result = "Operation successful!";
				break;
			case EB.Sparx.eResponseCode.InsufficientHardCurrency:
				result = "You don't have enough gems. Go to the store to buy more!";
				break;
			case EB.Sparx.eResponseCode.InsufficientSocialCurrency:
				result = "You don't have enough tickets for this purchase.";
				break;
			case EB.Sparx.eResponseCode.InsufficientSoftCurrency:
				result = "You don't have enough gold for this purchase.";
				break;
			case EB.Sparx.eResponseCode.InsufficientStamina:
				result = "You don't have enough stamina. Get more using the + button!";
				break;
			default:
				if (!isNetworkReachable)
				{
					result = "Could not connect to network.";
				}
				else
				{
					result = "An error occurred.";
				}
				break;
		}

		return result;
	}

	public static void ShowErrorDialogue(string message)
	{
		UIStack.Instance.GetDialog("Error", message, null);
	}

	public static void ShowErrorDialogue(string message, System.Action callback)
	{
		UIStack.Instance.GetDialog("Error", message, delegate(eUIDialogueButtons button, UIDialogeOption option)
		{
			callback();
		});
	}

	public static bool GlobalErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
	{
		if (response.fatal)
		{
			SparxHub.Instance.FatalError(response.localizedError);
		}
		else
		{
			ShowErrorDialogue(response.localizedError);
		}

		return false;
	}

	public static void SuspendHandler(EB.Sparx.Response response)
	{
		EB.Sparx.Request request = response.request;
		if (request.userData != null && request.userData is System.Action<EB.Sparx.Response>)
		{
			EB.Debug.LogWarning("SuspendHandler: callback with empty result, url = {0}", response.url);
			// callback empty result
			EB.Sparx.Response empty = new EB.Sparx.Response(request);
			empty.sucessful = true;
			Johny.HashtablePool.ReleaseRecursion(empty.hashtable);
			empty.hashtable = null;

			System.Action<EB.Sparx.Response> callback = request.userData as System.Action<EB.Sparx.Response>;
			if (System.Object.ReferenceEquals(callback.Target, null) || !callback.Target.Equals(null))
			{
				callback(empty);
			}
		}
	}

	public static string ErrorTranslateHandler(string error)
	{
		int errCode = -1;
		if (int.TryParse(error, out errCode))
		{
			return TranslateToUIText((EB.Sparx.eResponseCode)errCode);
		}

		return error;
	}

	public static void ShowErrorModal(this EB.Sparx.eResponseCode response)
	{
		ShowErrorDialogue(TranslateToUIText(response));
	}

	public static void ShowErrorModal(this EB.Sparx.Response response)
	{
		ShowErrorDialogue(response.localizedError);
	}

	public static void ShowErrorModal(this EB.Sparx.Response response, System.Action callback)
	{
		ShowErrorDialogue(response.localizedError, callback);
	}

	public static bool CheckAndShowModal(this EB.Sparx.eResponseCode response)
	{
		if (response == EB.Sparx.eResponseCode.Success)
		{
			return true;
		}
		else
		{
			ShowErrorDialogue(TranslateToUIText(response));
			return false;
		}
	}

	public static bool CheckAndShowModal(this EB.Sparx.Response response)
	{
		if (response.sucessful)
		{
			return true;
		}

		if (response.fatal)
		{
			SparxHub.Instance.FatalError(response.localizedError);
		}
		else
		{
			ShowErrorDialogue(response.localizedError);
		}

		return false;
	}
}
