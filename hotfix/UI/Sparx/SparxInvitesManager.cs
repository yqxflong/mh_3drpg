using EB.Sparx;
using System.Collections;

namespace Hotfix_LT.UI
{
	public class RemoveData
	{
		public string Id { get; set; }
		public string TargetUid { get; set; }
		public string Catalog { get; set; }

		public static RemoveData Parse(Hashtable ht)
		{
			if (ht == null)
			{
				return null;
			}

			RemoveData invite = new RemoveData();
			invite.Id = EB.Dot.String("_id", ht, invite.Id);
			invite.TargetUid = EB.Dot.String("target", ht, string.Empty);
			invite.Catalog = EB.Dot.String("c", ht, string.Empty);

			return invite;
		}
	}

	public class InviteData
	{
		public enum eType
		{
			Request,
			Invite,
		}

		public string Id { get; set; }
		public eType Type { get; set; }
		public long OpUid { get; set; }
		public long[] AgreeUids { get; set; }
		public long[] ReceiverUids { get; set; }
		public string[] ReceiverNames { get; set; }
		public string SenderName { get; set; }
		public long SenderUid { get; set; }
		public double SendTime { get; set; }
		public double DeadLine { get; set; }
		public string Catalog { get; set; }
		public object OrgnizationId { get; set; }
		public Hashtable Data { get; set; }
		public Hashtable Infos { get; set; }

		public InviteData()
		{
			AgreeUids = new long[0];
			ReceiverUids = new long[0];
			ReceiverNames = new string[0];
		}

		public static InviteData Parse(Hashtable ht)
		{
			if (ht == null)
			{
				return null;
			}

			InviteData invite = new InviteData();
			invite.Id = EB.Dot.String("_id", ht, invite.Id);
			string t = EB.Dot.String("t", ht, string.Empty);
			if (t == "inv")
			{
				invite.Type = eType.Invite;
			}
			else if (t == "req")
			{
				invite.Type = eType.Request;
			}
			ArrayList agreeUids = Hotfix_LT.EBCore.Dot.Array("a", ht, Johny.ArrayListPool.Claim());
			invite.AgreeUids = new long[agreeUids.Count];
			if (agreeUids.Count > 1)
			{
				EB.Debug.LogError("agreeUids.Count > 1");
			}
			for (int i = 0; i < agreeUids.Count; ++i)
			{
				invite.AgreeUids[i] = long.Parse(agreeUids[i].ToString());
			}

			invite.OpUid = EB.Dot.Long("op_uid", ht, 0);

			ArrayList receivers = Hotfix_LT.EBCore.Dot.Array("t_uids", ht, Johny.ArrayListPool.Claim());
			invite.ReceiverUids = new long[receivers.Count];
			for (int i = 0; i < receivers.Count; ++i)
			{
				invite.ReceiverUids[i] = long.Parse(receivers[i].ToString());
			}

			Hashtable infos = EB.Dot.Object("t_infos", ht, Johny.HashtablePool.Claim());
			invite.Infos = infos;
			invite.ReceiverNames = new string[receivers.Count];
			for (int i = 0; i < receivers.Count; ++i)
			{
				string receiveName = EB.Dot.String(receivers[i] + ".name", infos, string.Empty);
				if (receiveName != string.Empty)
				{
					invite.ReceiverNames[i] = receiveName;
				}
				else
				{
					EB.Debug.LogError("receiveName==null uid={0}", receivers[i]);
				}
			}

			//ArrayList receivernames = Hotfix_LT.EBCore.Dot.Array("t_names", ht, Johny.ArrayListPool.Claim());
			//         invite.ReceiverNames = new string[receivernames.Count];
			//         for (int i = 0; i < receivernames.Count; ++i)
			//         {
			//             invite.ReceiverNames[i] = receivernames[i].ToString();
			//         }

			invite.SenderName = EB.Dot.String("s_name", ht, invite.SenderName);
			invite.SenderUid = EB.Dot.Long("s_uid", ht, invite.SenderUid);
			invite.SendTime = EB.Dot.Double("ts", ht, invite.SendTime);
			invite.DeadLine = EB.Dot.Double("e", ht, invite.DeadLine);
			invite.Catalog = EB.Dot.String("c", ht, invite.Catalog);
			invite.OrgnizationId = Hotfix_LT.EBCore.Dot.Find<object>("o_id", ht) ;
			invite.Data = EB.Dot.Object("d", ht, invite.Data);

			return invite;
		}

		public Hashtable ToJson()
		{
			Hashtable hs = Johny.HashtablePool.Claim();

			hs["_id"] = Id;
			if (Type == eType.Invite)
			{
				hs["t"] = "inv";
			}
			else if (Type == eType.Request)
			{
				hs["t"] = "req";
			}
			else
			{
				hs["t"] = string.Empty;
			}
			//hs["t_names"] = new ArrayList(ReceiverNames);
			hs["t_infos"] = Infos;
			hs["t_uids"] = new ArrayList(ReceiverUids);
			hs["s_name"] = SenderName;
			hs["s_uid"] = SenderUid;
			hs["ts"] = SendTime;
			hs["e"] = DeadLine;
			hs["c"] = Catalog;
			hs["o_id"] = OrgnizationId;
			hs["d"] = Data;

			hs["a"] = new ArrayList(ReceiverUids);

			hs["r"] = new ArrayList(ReceiverUids);

			return hs;
		}
	}

	public class InvitesManager : ManagerUnit
	{
		public System.Action<InviteData> OnAcceptListener;
		public System.Action<InviteData> OnRejectListener;
		public System.Action<InviteData> OnInviteListener;
		public System.Action<InviteData> OnRequestListener;
		public System.Action<InviteData> OnRemoveListener;
		public System.Action<RemoveData> OnRemoveTargetListener;
		public System.Action OnRemoveInviteListener;

		private EndPoint m_endPoint;

		public override void Initialize(Config config)
		{
			m_endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
		}

		public override void Dispose()
		{
			m_endPoint = null;
		}

		public override void Async(string message, object payload)
		{
			switch (message)
			{
				case "accept":
					OnAccept(payload);
					break;
				case "reject":
					OnReject(payload);
					break;
				case "inv":
					OnInvite(payload);
					break;
				case "req":
					OnRequest(payload);
					break;
				case "updated":
					OnRequest(payload);
					break;
				case "remove":
					OnRemove(payload);
					break;
				case "remove-target":
					OnRemoveTarget(payload);
					break;
				case "removeInvite":
					if (OnRemoveInviteListener != null)
						OnRemoveInviteListener();
					break;
			}
		}

		private void OnAccept(object payload)
		{
			if (OnAcceptListener != null)
			{
				Hashtable obj = EB.Dot.Object("intact.invite", payload, null);
				var invite = InviteData.Parse(obj);
				if (invite == null)
				{
					EB.Debug.LogWarning("InvitesManager.OnAccept: parse invite data failed");
				}
				else
				{
					OnAcceptListener(invite);
				}
			}
		}

		private void OnReject(object payload)
		{
			if (OnRejectListener != null)
			{
				Hashtable obj = EB.Dot.Object("intact.invite", payload, null);
				var invite = InviteData.Parse(obj);
				if (invite == null)
				{
					EB.Debug.LogWarning("InvitesManager.OnReject: parse invite data failed");
				}
				else
				{
					OnRejectListener(invite);
				}
			}
		}

		private void OnInvite(object payload)
		{
			if (OnInviteListener != null)
			{
				Hashtable obj = EB.Dot.Object("intact.invite", payload, null);
				var invite = InviteData.Parse(obj);
				if (invite == null)
				{
					EB.Debug.LogWarning("InvitesManager.OnInvite: parse invite data failed");
				}
				else
				{
					OnInviteListener(invite);
				}
			}
		}

		private void OnRequest(object payload)
		{
			if (OnRequestListener != null)
			{
				Hashtable obj = EB.Dot.Object("intact.invite", payload, null);
				var invite = InviteData.Parse(obj);
				if (invite == null)
				{
					EB.Debug.LogWarning("InvitesManager.OnRequest: parse invite data failed");
				}
				else
				{
					OnRequestListener(invite);
				}
			}
		}

		private void OnRemove(object payload)
		{
			if (OnRemoveListener != null)
			{
				Hashtable obj = EB.Dot.Object("intact.invite", payload, null);
				var invite = InviteData.Parse(obj);
				if (invite == null)
				{
					EB.Debug.LogWarning("InvitesManager.OnRemove: parse invite data failed");
				}
				else
				{
					OnRemoveListener(invite);
				}
			}
		}

		private void OnRemoveTarget(object payload)
		{
			if (OnRemoveTargetListener != null)
			{
				Hashtable obj = EB.Dot.Object("intact.invite", payload, null);
				var remove = RemoveData.Parse(obj);
				if (remove == null)
				{
					EB.Debug.LogWarning("InvitesManager.OnRemoveTarget: parse remove data failed");
				}
				else
				{
					OnRemoveTargetListener(remove);
				}
			}
		}

		//accept   invite_id  [xxx,xxx]
		public void Accept(string[] invites, System.Action<Response> callback)
		{
			EB.Sparx.Request request = m_endPoint.Post("/invites/accept");
			request.AddData("invite_id", invites);
			m_endPoint.Service(request, callback);
		}

		//reject   invite_id  [xxx,xxx]
		public void Reject(string[] invites, System.Action<Response> callback)
		{
			EB.Sparx.Request request = m_endPoint.Post("/invites/reject");
			request.AddData("invite_id", invites);
			m_endPoint.Service(request, callback);
		}
	}
}
