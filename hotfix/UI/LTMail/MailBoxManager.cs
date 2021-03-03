using System;
using System.Collections;
using EB.Sparx;

namespace Hotfix_LT.UI
{
	public class MailBoxManager : ManagerUnit
	{
		private static MailBoxManager sInstance = null;

		public static MailBoxManager Instance
		{
			get { return sInstance = sInstance ?? LTHotfixManager.GetManager<MailBoxManager>(); }
		}

		public int CapacityNum
		{
			get
			{
				int num;
				DataLookupsCache.Instance.SearchIntByID("mailbox.mailLimit", out num);
				return num;
			}
		}
		static public string ListDataId = "mailbox.mails";
		public MailList MailList;
		//public int UnReadMailnum = 0;

		public override void Initialize(Config config)
		{
			Instance.Api = new MailAPI();
			Instance.Api.ErrorHandler += ErrorHandler;

			MailList = GameDataSparxManager.Instance.Register<MailList>(ListDataId);
		}

		private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
		{
			return false;
		}

		public override void Async(string message, object payload)
		{
			// payload already processed by PushManager/GameDataManager
			switch (message)
			{
				case "update":
					break;
				case "newMail":
					//UnReadMailnum++;
					GetMailList();
					break;
			}
		}

		//Api Request
		public MailAPI Api
		{
			get; private set;
		}

		public void GetMailList()
		{
			//UnReadMailnum = 0;
			Api.GetMailList(-1, FetchDataHandler);
		}

		public void ReceiveGift(string mailId, System.Action<bool> callback)
		{
			Api.ReceiveGift(mailId, delegate (Hashtable result)
			{
				callback(result != null);
				if (result != null)
				{
					result.Remove("async");
					DataLookupsCache.Instance.CacheData(result);
				}
			});
		}

		public void HasRead(string mailId)
		{
			Api.HasRead(mailId, FetchDataHandler);
		}

		public void SendUserMail(long receiverId, string mailTitle, string mailText)
		{
			Api.SendUserMail(receiverId, mailTitle, mailText, FetchDataHandler);
		}

		public void OneKeyReceive(System.Action<Hashtable> callback)
		{
			Api.OneKeyReceive(delegate (Hashtable result)
			{
				callback(result);
				if (result != null)
				{
					result.Remove("async");
					DataLookupsCache.Instance.CacheData(result);
				}
			});
		}

		public void OneKeyDelete(System.Action<Hashtable> callback)
		{
			Api.OneKeyDelete((result) =>
			{
				callback(result);
				if (result != null)
				{
					result.Remove("async");
					DataLookupsCache.Instance.CacheData(result);
				}
			});
		}

		public void SingleDelete(string maidId, System.Action<Hashtable> callback)
		{
			Api.DeleteReceive(maidId, (result) =>
			{
				callback(result);
				if (result != null)
				{
					result.Remove("async");
					DataLookupsCache.Instance.CacheData(result);
				}
			});

		}


		private void FetchDataHandler(Hashtable data)
		{
			if (data != null)
			{
				GameDataSparxManager.Instance.ProcessIncomingData(data, false);
			}
		}

		private void MergeDataHandler(Hashtable data)
		{
			if (data != null)
			{
				GameDataSparxManager.Instance.ProcessIncomingData(data, true);
			}
		}
	}
}