using EB.Sparx;
using System.Collections;
using EB;

namespace Hotfix_LT.UI
{
   public class SocialIntactManager : ManagerUnit, IManagerUnitUpdatable
	{
		private EndPoint m_endPoint;
		private const float m_pkInvervalTime=30f;
		private const int m_pkCountPerTimes = 5;
		private float m_pkTimer=0f;
		private int m_pkCount=0;

		
		private static SocialIntactManager sInstance = null;
		
		public static SocialIntactManager Instance
		{
			get { return sInstance = sInstance ?? LTHotfixManager.GetManager<SocialIntactManager>(); }
		}
		
		public override void Initialize(Config config)
		{
			m_endPoint = Hub.Instance.ApiEndPoint;
		}

		public override void Dispose()
		{
			m_endPoint = null;
		}

		public override void Async(string message, object payload)
		{
			switch (message)
			{
				case "like":
					OnLike(payload);//rolling_message.like.from:{user_name:xxx,uid:xxx}    to:{user_name:xxx,uid:xxx}
					break;
				case "unlike":
					OnUnLike(payload);//from:{user_name:xxx,uid:xxx}    to:{user_name:xxx,uid:xxx}
					break;
				case "provoke":
					OnProvoke(payload);//from:{user_name:xxx,uid:xxx}    to:{user_name:xxx,uid:xxx}
					break;
				case "loser_list":
					break;
			}
		}

		public bool UpdateOffline
		{
			get { return false; }
		}

		public void Update()
		{
			if (m_pkCount <= 0)
				return;

			m_pkTimer += Time.deltaTime;
			if (m_pkTimer > m_pkInvervalTime)
			{
				m_pkTimer = 0;
				m_pkCount = 0;
			}
		}

		private void OnLike(object payload)
		{
			Hashtable data = payload as Hashtable;
			string from = EB.Dot.String("rolling_message.like.from.user_name", data,"");
			string LikeFormat = EB.Localizer.GetString("ID_SOCIAL_LIKE_MESSAGE");
			//string to = EB.Dot.String("rolling_message.like.to.user_name", data, "");

			UIBroadCastMessageController.Instance.PutOneMessage(string.Format(LikeFormat, from));

			SparxHub.Instance.ChatManager.HandleSystemMessage(string.Format(LikeFormat, from));

		}

		private void OnUnLike(object payload)
		{
			Hashtable data = payload as Hashtable;
			string from = EB.Dot.String("rolling_message.unlike.from.user_name", data, "");
			//string to = EB.Dot.String("rolling_message.unlike.to.user_name", data, "");
			string UnLikeFormat = EB.Localizer.GetString("ID_SOCIAL_UNLIKE_MESSAGE");
			UIBroadCastMessageController.Instance.PutOneMessage(string.Format(UnLikeFormat, from));
			SparxHub.Instance.ChatManager.HandleSystemMessage(string.Format(UnLikeFormat, from));
		}

		private void OnProvoke(object payload)
		{
			Hashtable data = payload as Hashtable;
			string from = EB.Dot.String("rolling_message.provoke.from.user_name", data, "");
			//string to = EB.Dot.String("rolling_message.provoke.to.user_name", data, "");
			string ProvokeFormat = EB.Localizer.GetString("ID_SOCIAL_PROVOKE_MESSAGE");
			UIBroadCastMessageController.Instance.PutOneMessage(string.Format(ProvokeFormat, from));
			SparxHub.Instance.ChatManager.HandleSystemMessage(string.Format(ProvokeFormat, from));
		}

		public void Like(long uid, System.Action<Response> callback)
		{
			EB.Sparx.Request request = m_endPoint.Post("/social/like");
			request.AddData("targetUid", uid);
			m_endPoint.Service(request, callback);
		}

		public void UnLike(long uid, System.Action<Response> callback)
		{
			EB.Sparx.Request request = m_endPoint.Post("/social/unlike");
			request.AddData("targetUid", uid);
			m_endPoint.Service(request, callback);
		}

		public void Provoke(long uid, System.Action<Response> callback)
		{
			EB.Sparx.Request request = m_endPoint.Post("/social/provoke");
			request.AddData("targetUid", uid);
			m_endPoint.Service(request, callback);
		}
		//暂未使用，需要保留
		public void InvitePVP(long uid, System.Action<Response> callback)
		{
			if (m_pkCount>= m_pkCountPerTimes)
			{
				MessageTemplateManager.ShowMessage(902160);
				return;
			}
			m_pkCount++;
			EB.Sparx.Request request = m_endPoint.Post("/social/requestPVP");
			if (FriendManager.Instance.CheckBeblack(uid))
			{
				Response r = new Response(request);
				r.sucessful = true;
				r.hashtable = null;
				callback(r);
				return;
			}
			request.AddData("targetUid", uid);
			m_endPoint.Service(request, callback);
		}


		public void SocialCombat(long uid, System.Action<Response> callback)
		{
			if (m_pkCount >= m_pkCountPerTimes)
			{
				MessageTemplateManager.ShowMessage(902160);
				return;
			}
			m_pkCount++;
			EB.Sparx.Request request = m_endPoint.Post("/social/startSocialCombat");
			if (FriendManager.Instance.CheckBeblack(uid))
			{
				Response r = new Response(request);
				r.sucessful = true;
				r.hashtable = null;
				callback(r);
				return;
			}
			request.AddData("targetUid", uid);
			m_endPoint.Service(request, callback);
		}

		public void CancelPVP(long uid, System.Action<Response> callback)
		{
			EB.Sparx.Request request = m_endPoint.Post("/social/cancelPVP");
			if (FriendManager.Instance.CheckBeblack(uid))
			{
				Response r = new Response(request);
				r.sucessful = true;
				r.hashtable = null;
				callback(r);
				return;
			}
			request.AddData("targetUid", uid);
			m_endPoint.Service(request, callback);
		}

		public void SaveReceivePkRequest(bool isReceive, System.Action<Response> callback)
		{
			EB.Sparx.Request request = m_endPoint.Post("/social/saveReceivePkRequest");
			request.AddData("isReceive", isReceive);
			m_endPoint.Service(request, callback);
		}

		//intact.like_list.{xxxx:true}
		public static bool IsLike(long uid)
		{
			bool state;
			if (!DataLookupsCache.Instance.SearchDataByID<bool>("intact.like_list." + uid, out state)) return false;
			return state;
		}

		public static bool IsUnLike(long uid)
		{
			bool state;
			if (!DataLookupsCache.Instance.SearchDataByID<bool>("intact.like_list." + uid, out state)) return false;
			return !state;
		}
		public static bool IsCanProvoke(long uid)
		{
			//在我击败的人中间
			if (!IsMyLoser(uid)) return false;
			//我Provoke人以外
			if(!IsProvoke(uid)) return true;

			return false;
		}
		//intact.loser_list.{xxxx:true}
		public static bool IsMyLoser(long uid)
		{
			bool state;
			if (!DataLookupsCache.Instance.SearchDataByID<bool>("intact.loser_list." + uid, out state)) return false;
			return state;
		}

		public static bool IsMyWiner(long uid)
		{
			bool state;
			if (!DataLookupsCache.Instance.SearchDataByID<bool>("intact.loser_list." + uid, out state)) return false;
			return !state;
		}
		//intact.provoke_list.{uid}
		public static bool IsProvoke(long uid)
		{
			int state;
			if (!DataLookupsCache.Instance.SearchIntByID("intact.provoke_list." + uid, out state)) return false;
			return true;
		}
	}
}