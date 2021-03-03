///////////////////////////////////////////////////////////////////////
//
//  SparxSocialManager.cs
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
using EB.Sparx;

namespace Hotfix_LT.UI
{
	public class SocialManager : ManagerUnit
	{
		public const int kAdHocPortraitInfoPullCooldownInSecond = 3;
		public const int kFriendsPortraitInfoPullCooldownInSecond = 10;
		public const int kUniqueTimestampGeneratingCooldownInSecond = 3600; // 1 hour

		// Used for cache invalidation
		public enum PortraitPriority
		{
			High,
			Mid,
			Low
		};

		private class DateTimeRef
		{
			public System.DateTime Value { get; set; }

			public DateTimeRef(System.DateTime value)
			{
				Value = value;
			}
		}

		private SocialAPI _api;
		private EndPoint _socialEndPoint;
		private string _imageBaseUrl;

		private bool _isPullingPortraitInfo = false;
		private Dictionary<long, string> _portraitInfoCache;
		private DateTimeRef _allowPullFriendsPortraitInfoUntil;
		private DateTimeRef _allowPullAdHocPortraitInfoUntil;
		private System.DateTime _allowGenerateUniqueTimestampUntil;
		private string _uniqueTimestamp;

		public SubSystemState State { get; set; }

		public override void Initialize(Config config)
		{
			_api = new SocialAPI();
			_portraitInfoCache = new Dictionary<long, string>();
			_allowGenerateUniqueTimestampUntil = System.DateTime.Now;

			Hub.Instance.StartCoroutine(UniqueTimestampGenerator());
		}

		#region SubSystem implementation
		public override void Connect()
		{
			State = SubSystemState.Connected;

			string portraitId = EB.Dot.String("portraitId", Hub.Instance.DataStore.LoginDataStore.LoginData, "");

			if (portraitId != "")
			{
				SetPortrait(Hub.Instance.DataStore.LoginDataStore.LocalUserId.Value, portraitId, PortraitPriority.High);
			}

			EB.Sparx.Config config = SparxHub.Instance.Config;
			EndPointOptions options = new EndPointOptions { Key = config.ApiKey.Value };
			Hashtable social = EB.Dot.Object("social", Hub.Instance.DataStore.LoginDataStore.LoginData, null);

			if (null == social)
			{
				return;
			}

			string endpointUrl = EB.Dot.String("endpoint", social, null);

			_imageBaseUrl = EB.Dot.String("imageBase", social, null); ;
			_socialEndPoint = EndPointFactory.Create(endpointUrl, options);

			_socialEndPoint.AddData("stoken", Hub.Instance.ApiEndPoint.GetData("stoken"));
			_api.SetSocialEndpoint(_socialEndPoint);

			_allowPullFriendsPortraitInfoUntil = new DateTimeRef(System.DateTime.Now);
			_allowPullAdHocPortraitInfoUntil = new DateTimeRef(System.DateTime.Now);

			UpdateUniqueTimestamp();
			GetFriendsPortraitInfo(null);

			Hub.Instance.GetManager<PushManager>().OnDisconnected += OnPushDisconnected;
		}

		public override void Disconnect(bool isLogout)
		{
			State = SubSystemState.Disconnected;
			Hub.Instance.GetManager<PushManager>().OnDisconnected -= OnPushDisconnected;

			if (_socialEndPoint != null)
			{
				_socialEndPoint.Dispose();
			}

			_socialEndPoint = null;
			_imageBaseUrl = null;
			_portraitInfoCache.Clear();
		}
		#endregion

		public void FindUidByName(string name, System.Action<eResponseCode, List<FriendInfoData>> callback)
		{
			_api.FindUidByName(name, (eResponseCode response, Hashtable result) =>
			{
				if (eResponseCode.Success != response)
				{
					callback(response, null);
					return;
				}

				ArrayList usersList = Hotfix_LT.EBCore.Dot.Array("users", result, Johny.ArrayListPool.Claim());
				List<FriendInfoData> users = new List<FriendInfoData>();

				for (var i = 0; i < usersList.Count; i++)
				{
					Hashtable user = usersList[i] as Hashtable;
					FriendInfoData friendInfoData = new FriendInfoData(user);
					users.Add(friendInfoData);
				}

				callback(response, users);
			});
		}

		private void GetAdHocPortraitInfo(List<Id> uidList, System.Action<eResponseCode> callback)
		{
			Hub.Instance.StartCoroutine(PortraitInfoRequester(_allowPullAdHocPortraitInfoUntil, (System.Action done) => {
				_api.GetAdHocPortraitInfo(uidList, (eResponseCode response, Hashtable result) =>
				{
					_allowPullAdHocPortraitInfoUntil.Value = _allowPullAdHocPortraitInfoUntil.Value.AddSeconds(kAdHocPortraitInfoPullCooldownInSecond);
					done();

					if (eResponseCode.Success != response)
					{
						if (callback != null)
						{
							callback(response);
						}

						return;
					}

					PopulatePortraitInfoCache(result, PortraitPriority.Low);

					if (callback != null)
					{
						callback(response);
					}
				});
			}));
		}

		private void GetFriendsPortraitInfo(System.Action<eResponseCode> callback)
		{
			Hub.Instance.StartCoroutine(PortraitInfoRequester(_allowPullFriendsPortraitInfoUntil, (System.Action done) => {
				_api.GetFriendsPortraitInfo((eResponseCode response, Hashtable result) =>
				{
					_allowPullFriendsPortraitInfoUntil.Value = _allowPullFriendsPortraitInfoUntil.Value.AddSeconds(kFriendsPortraitInfoPullCooldownInSecond);
					done();

					if (eResponseCode.Success != response)
					{
						if (callback != null)
						{
							callback(response);
						}

						return;
					}

					PopulatePortraitInfoCache(result, PortraitPriority.High);

					if (callback != null)
					{
						callback(response);
					}
				});
			}));
		}

		private void PopulatePortraitInfoCache(Hashtable data, PortraitPriority priority)
		{
			ArrayList rawPortraitInfo = Hotfix_LT.EBCore.Dot.Array("portraitInfo", data, Johny.ArrayListPool.Claim());

			for (var i = 0; i < rawPortraitInfo.Count; i++)
			{
				Hashtable player = rawPortraitInfo[i] as Hashtable;
				SetPortrait(EB.Dot.Long("u", player, -1), EB.Dot.String("p", player, null), priority);
			}
		}

		private WaitForSeconds wait1 = new WaitForSeconds(1f);
		private IEnumerator PortraitInfoRequester(DateTimeRef waitUntil, System.Action<System.Action> callback)
		{
			if (_isPullingPortraitInfo)
			{
				yield return null;
			}

			while (System.DateTime.Now < waitUntil.Value)
			{
				yield return wait1;
			}

			_isPullingPortraitInfo = true;

			callback(() => {
				_isPullingPortraitInfo = false;
			});

			yield break;
		}

		public void SetPortrait(long uid, string portraitId, PortraitPriority priority)
		{
			if (uid < 0)
			{
				return;
			}

			if (null == portraitId)
			{
				return;
			}

			_portraitInfoCache[uid] = portraitId;

			// TODO: cache invalidation
		}

		private IEnumerator UniqueTimestampGenerator()
		{
			int checkCoolDown = System.Math.Min(kUniqueTimestampGeneratingCooldownInSecond, 300);

			while (true)
			{
				while (State != SubSystemState.Connected)
				{
					yield return new WaitForSeconds(60);
				}

				if (System.DateTime.Now >= _allowGenerateUniqueTimestampUntil)
				{
					UpdateUniqueTimestamp();
					_allowGenerateUniqueTimestampUntil = _allowGenerateUniqueTimestampUntil.AddSeconds(kUniqueTimestampGeneratingCooldownInSecond);
				}

				// do a shorter sleep plue _allowGenerateUniqueTimestampUntil checking to make loop running faster after app rusumed
				yield return new WaitForSeconds(checkCoolDown);
			}
		}

		private void UpdateUniqueTimestamp()
		{
			System.DateTime now = System.DateTime.UtcNow;

#pragma warning disable 162
			if (kUniqueTimestampGeneratingCooldownInSecond >= 3600)
			{
				_uniqueTimestamp = now.ToString("%yyyy%MM%dd%HH");
			}
			else
			{
				_uniqueTimestamp = now.ToString("%yyyy%MM%dd%HH%mm");
			}
#pragma warning restore 162
		}

		public void UploadPortrait(int characterId, string portraitInBase64, System.Action<eResponseCode> callback)
		{
			if ((null == _socialEndPoint) || (characterId < 0))
			{
				if (null != callback)
				{
					callback(eResponseCode.Unknown);
				}
				return;
			}

			_api.UploadPortrait(characterId, portraitInBase64, (eResponseCode response, Hashtable result) =>
			{
				if (eResponseCode.Success == response)
				{
					SetPortrait(Hub.Instance.DataStore.LoginDataStore.LocalUserId.Value, EB.Dot.String("portraitId", result, null), PortraitPriority.High);
				}

				if (null != callback)
				{
					callback(response);
				}
			});
		}

		private void OnPushConnected()
		{
			GetFriendsPortraitInfo(null);
		}

		private void OnPushDisconnected()
		{
			Hub.Instance.GetManager<PushManager>().OnConnected -= OnPushConnected;
			Hub.Instance.GetManager<PushManager>().OnConnected += OnPushConnected;
		}
	}
}
