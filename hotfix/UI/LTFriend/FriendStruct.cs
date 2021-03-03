using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
	public enum eFriendType
	{
		None = 0,
		My = 1,
		Black = 2,
		Recently = 3,
		Team = 4,
		Search = 5,
	}

	public enum eLikeState
	{
		None = 0, Like, Unlike
	}

	public class FriendDataMeta
	{
		public string DataPath
		{
			get; set;
		}

		public float LifeTime
		{
			get; set;
		}

		public float LifeDuration
		{
			get; set;
		}

		public bool Dirty
		{
			get; set;
		}

		public System.Action Updater
		{
			get; set;
		}

		public FriendDataMeta()
		{

		}

		public FriendDataMeta(string path)
		{
			DataPath = path;
		}

		public FriendDataMeta(string path, System.Action updater)
		{
			DataPath = path;
			Updater = updater;
		}

		public FriendDataMeta(string path, System.Action updater, float lifeDuration)
		{
			DataPath = path;
			Updater = updater;
			LifeDuration = lifeDuration;
		}
	}

	public class BaseFriendData
	{
		public long Uid;
		public eFriendType Type;
		public string Name;
		public int Level;
		public string Head;
        public string Frame;
        public int Skin;
		public int Fight;
		public string AlliName;
		public float OfflineTime;
		public bool IsFriend;
		public int index;  // 用于排序，确定当前的data是第几个
	}

	public class FriendData : BaseFriendData
	{
		public bool IsHaveSendVigor;
		public bool IsCanReceiveVigor;
		public eLikeState LikeState;
		public int LikeNum;
		public bool IsSelect;
		public bool IsUnread;
	}

	public class RecommendFriendData : BaseFriendData
	{
		public string Desc;
	}

	public class ApplyFriendData : BaseFriendData
	{
		public string VerifyInfo;
	}

	public class FriendComparer : IComparer<FriendData>
	{
		public int Compare(FriendData x, FriendData y)
		{
			if (x.IsCanReceiveVigor != y.IsCanReceiveVigor)
			{
				if (x.IsCanReceiveVigor)
					return -1;
				else
					return 1;
			}

			if (x.OfflineTime == 0 && y.OfflineTime != 0)
				return -1;
			else if (y.OfflineTime == 0 && x.OfflineTime != 0)
				return 1;

			if (x.OfflineTime != y.OfflineTime)
				return x.OfflineTime - y.OfflineTime < 0 ? 1 : -1;

			return EB.Localizer.Culture.CompareInfo.Compare(x.Name, y.Name);
		}
	}

	public class FriendRejectTarget
	{
		public long Uid { get; set; }
		public float Ts { get; set; }
	}

	public class FriendConfig : INodeData
	{
		public int MaxFriendNum { get; private set; }
		public int MaxBlacklistNum { get; private set; }
		public int MaxRecentlyNum { get; private set; }
		public int MaxTeamNum { get; private set; }
		public int MaxVigorSendNum { get; private set; }
		public int MaxVigorReceiveNum { get; private set; }
		public int GetVigorValue { get; private set; }
		public int SendVigorValue { get; private set; }
		public int RejectTimeInterval { get; private set; }  //seconds

		public void CleanUp()
		{
			MaxFriendNum = 99;
			MaxBlacklistNum = 20;
			MaxRecentlyNum = 20;
			MaxTeamNum = 20;
			MaxVigorSendNum = 10;
			MaxVigorReceiveNum = 10;
			GetVigorValue = 5;
			SendVigorValue = 5;
			RejectTimeInterval = 15;
		}

		public object Clone()
		{
			return new FriendConfig();
		}

		public void OnUpdate(object obj)
		{
			if (obj == null)
			{
				return;
			}
			MaxFriendNum = EB.Dot.Integer("maxFriendNum", obj, MaxFriendNum);
			MaxBlacklistNum = EB.Dot.Integer("maxBlacklistNum", obj, MaxBlacklistNum);
			MaxRecentlyNum = EB.Dot.Integer("maxRecentlyNum", obj, MaxRecentlyNum);
			MaxTeamNum = EB.Dot.Integer("maxTeamNum", obj, MaxTeamNum);
			MaxVigorSendNum = EB.Dot.Integer("maxVigorSendNum", obj, MaxVigorSendNum);
			MaxVigorReceiveNum = EB.Dot.Integer("maxVigorReceiveNum", obj, MaxVigorReceiveNum);
			GetVigorValue = EB.Dot.Integer("getVigorValue", obj, GetVigorValue);
			SendVigorValue = EB.Dot.Integer("sendVigorValue", obj, SendVigorValue);
			RejectTimeInterval = EB.Dot.Integer("rejectTimeInterval", obj, RejectTimeInterval);
		}

		public void OnMerge(object obj)
		{
			OnUpdate(obj);
		}

		public FriendConfig()
		{
			CleanUp();
		}
	}

	public class FriendInfo : INodeData
	{
		public int HaveVigorSendNum { get; set; }
		public int HaveVigorReceiveNum { get; set; }
		public int ApplyCount { get; set; }
		public int MyFriendNum { get; set; }
		public int BlacklistNum { get; set; }
		public bool IsHaveReqHistory { get; set; }
		public bool IsHaveNewMessage { get { return UnreadMessageUIds.Count > 0; } }
		public long NewestSendId { get; set; }
		public List<long> UnreadMessageUIds { get; set; }
		public List<FriendRejectTarget> RejectTargets { get; set; }
		public ArrayList BeBlacklists { get; set; }
		public long LastMessageTs
		{
			get
			{
				string s = PlayerPrefs.GetString(LoginManager.Instance.LocalUserId.Value + ".LastMessageTs", "");

				long ts;
				long.TryParse(s, out ts);
				return ts;
			}
			set
			{
				PlayerPrefs.SetString(LoginManager.Instance.LocalUserId.Value + ".LastMessageTs", value.ToString());
			}
		}

		public void CleanUp()
		{
			HaveVigorSendNum = 0;
			HaveVigorReceiveNum = 0;
			ApplyCount = 0;
			MyFriendNum = 0;
			BlacklistNum = 0;
			IsHaveReqHistory = false;
			NewestSendId = 0;
			UnreadMessageUIds = new List<long>();
			RejectTargets = new List<FriendRejectTarget>();
			BeBlacklists = Johny.ArrayListPool.Claim();
		}

		public object Clone()
		{
			return new FriendInfo();
		}

		public void OnUpdate(object obj)
		{
			HaveVigorSendNum = EB.Dot.Integer("haveVigorSendNum", obj, HaveVigorSendNum);
			HaveVigorReceiveNum = EB.Dot.Integer("haveVigorReceiveNum", obj, HaveVigorReceiveNum);
			ApplyCount = EB.Dot.Integer("applyCount", obj, ApplyCount);
			MyFriendNum = EB.Dot.Integer("friendCount", obj, MyFriendNum);
			BlacklistNum = EB.Dot.Integer("backlistNum", obj, BlacklistNum);
			BeBlacklists = Hotfix_LT.EBCore.Dot.Array("blackedUids", obj, BeBlacklists);
		}

		public void OnMerge(object obj)
		{
			OnUpdate(obj);
		}

		public FriendInfo()
		{
			CleanUp();
		}

		public void AddUnreadMessageId(long uid)
		{
			if (!UnreadMessageUIds.Contains(uid))
				UnreadMessageUIds.Add(uid);
		}

		public void RemoveUnreadMessageId(long uid)
		{
			UnreadMessageUIds.Remove(uid);
		}

		public bool GetIsUnreadMessageId(long uid)
		{
			return UnreadMessageUIds.Contains(uid);
		}
	}

	public class AllFriend : INodeData
	{
		public bool IsHaveUpdate;
		public List<FriendData> MyFriends;
		public List<FriendData> Blacklists;
		public List<FriendData> Recentlys;
		public List<FriendData> Teams;

		public void CleanUp()
		{
			MyFriends.Clear();
			Blacklists.Clear();
			Recentlys.Clear();
			Teams.Clear();
		}

		public object Clone()
		{
			return new AllFriend();
		}

		public void OnUpdate(object obj)
		{
			IsHaveUpdate = true;
			object myfriend = EB.Dot.Object("my", obj, null);
			object blacklist = EB.Dot.Object("blacklist", obj, null);
			object recently = EB.Dot.Object("recently", obj, null);
			object team = EB.Dot.Object("team", obj, null);
			if (myfriend != null)
				MyFriends = Hotfix_LT.EBCore.Dot.List<FriendData, long>(null, myfriend, MyFriends, Parse);
			if (blacklist != null)
				Blacklists = Hotfix_LT.EBCore.Dot.List<FriendData, long>(null, blacklist, Blacklists, Parse);
			if (recently != null)
				Recentlys = Hotfix_LT.EBCore.Dot.List<FriendData, long>(null, recently, Recentlys, Parse);
			if (team != null)
				Teams = Hotfix_LT.EBCore.Dot.List<FriendData, long>(null, team, Teams, Parse);
		}

		public void OnMerge(object obj)
		{
			object myfriend = EB.Dot.Object("my", obj, null);
			object blacklist = EB.Dot.Object("blacklist", obj, null);
			object recently = EB.Dot.Object("recently", obj, null);
			object team = EB.Dot.Object("team", obj, null);
			if (myfriend != null)
				MyFriends = Hotfix_LT.EBCore.Dot.List<FriendData, long>(null, myfriend, MyFriends, Parse, (friend, uid) => friend.Uid == uid);
			if (blacklist != null)
				Blacklists = Hotfix_LT.EBCore.Dot.List<FriendData, long>(null, blacklist, Blacklists, Parse, (friend, uid) => friend.Uid == uid);
			if (recently != null)
				Recentlys = Hotfix_LT.EBCore.Dot.List<FriendData, long>(null, recently, Recentlys, Parse, (friend, uid) => friend.Uid == uid);
			if (team != null)
				Teams = Hotfix_LT.EBCore.Dot.List<FriendData, long>(null, team, Teams, Parse, (friend, uid) => friend.Uid == uid);
		}

		public FriendData Parse(object value, long uid)
		{
			if (value == null)
			{
				return null;
			}

			FriendData friend = Find(uid) ?? new FriendData();
			friend.Uid = EB.Dot.Long("uid", value, friend.Uid);
			friend.Type = FriendManager.Instance.ParseFriendType("type", value, friend.Type);
			friend.Name = EB.Dot.String("name", value, friend.Name);
			friend.Level = EB.Dot.Integer("level", value, friend.Level);
			friend.Head = EB.Dot.String("portrait", value, friend.Head);
			friend.Skin = EB.Dot.Integer("skin", value, friend.Skin);
            friend.Frame = EB.Dot.String("headFrame", value, friend.Frame);
            friend.Fight = EB.Dot.Integer("battleRating", value, friend.Fight);
			friend.AlliName = EB.Dot.String("allianceName", value, friend.AlliName);
			friend.OfflineTime = EB.Dot.Single("offlineTime", value, friend.OfflineTime);
			friend.IsFriend = EB.Dot.Bool("isFriend", value, friend.IsFriend);
			friend.IsHaveSendVigor = EB.Dot.Bool("isHaveSendVigor", value, friend.IsHaveSendVigor);
			friend.IsCanReceiveVigor = EB.Dot.Bool("isCanReceiveVigor", value, friend.IsCanReceiveVigor);
			friend.LikeState = (eLikeState)EB.Dot.Integer("likeState", value, 0);
			friend.LikeNum = EB.Dot.Integer("likeNum", value, friend.LikeNum);
			friend.IsSelect = false;
			return friend;
		}

		public AllFriend()
		{
			MyFriends = new List<FriendData>();
			Blacklists = new List<FriendData>();
			Recentlys = new List<FriendData>();
			Teams = new List<FriendData>();
		}

		public FriendData Find(long uid)
		{
			return MyFriends.Find(f => f.Uid == uid);
		}

		public void Add(FriendData friend)
		{
			MyFriends.Add(friend);
		}

		public void Remove(long uid)
		{
			MyFriends.RemoveAll(f => f.Uid == uid);
		}
	}

	public class MyFriendList : FriendList<FriendData>
	{
		public bool canReceiveVigor
		{//好友赠送体力判断
			get
			{
				if (!FriendManager.Instance.IsResidueVigorReceiveNum())
				{
					return false;
				}

				bool can = false;
				for (int i = 0; i < this.List.Count; i++)
				{
					if (List[i].IsCanReceiveVigor)
					{
						can = true;
						break;
					}
				}
				return can;
			}
		}
		public override void OnUdpateAfter()
		{
			base.OnUdpateAfter();

			FriendManager.Instance.Info.MyFriendNum = List.Count;
		}

		public override FriendData Parse(object value, long uid)
		{
			if (value == null)
			{
				return null;
			}

			FriendData friend = Find(uid) ?? new FriendData();
			friend.Uid = EB.Dot.Long("uid", value, friend.Uid);
			friend.Type = FriendManager.Instance.ParseFriendType("type", value, friend.Type);
			friend.Name = EB.Dot.String("name", value, friend.Name);
			friend.Level = EB.Dot.Integer("level", value, friend.Level);
			friend.Head = EB.Dot.String("portrait", value, friend.Head);
			friend.Skin = EB.Dot.Integer("skin", value, friend.Skin);
            friend.Frame = EB.Dot.String("headFrame", value, friend.Frame);
            friend.Fight = EB.Dot.Integer("battleRating", value, friend.Fight);
			friend.AlliName = EB.Dot.String("allianceName", value, friend.AlliName);
			friend.OfflineTime = EB.Dot.Single("offlineTime", value, friend.OfflineTime);
			friend.IsFriend = EB.Dot.Bool("isFriend", value, friend.IsFriend);
			friend.IsHaveSendVigor = EB.Dot.Bool("isHaveSendVigor", value, friend.IsHaveSendVigor);
			friend.IsCanReceiveVigor = EB.Dot.Bool("isCanReceiveVigor", value, friend.IsCanReceiveVigor);
			friend.LikeState = (eLikeState)EB.Dot.Integer("likeState", value, 0);
			friend.LikeNum = EB.Dot.Integer("likeNum", value, friend.LikeNum);
			friend.IsSelect = false;
			return friend;
		}


		
	}

	public class BlackList : FriendList<FriendData>
	{
		public override void OnUdpateAfter()
		{
			base.OnUdpateAfter();

			FriendManager.Instance.Info.BlacklistNum = List.Count;
		}

		public override FriendData Parse(object value, long uid)
		{
			if (value == null)
			{
				return null;
			}

			FriendData friend = Find(uid) ?? new FriendData();
			friend.Uid = EB.Dot.Long("uid", value, friend.Uid);
			friend.Type = FriendManager.Instance.ParseFriendType("type", value, friend.Type);
			friend.Name = EB.Dot.String("name", value, friend.Name);
			friend.Level = EB.Dot.Integer("level", value, friend.Level);
			friend.Head = EB.Dot.String("portrait", value, friend.Head);
			friend.Skin = EB.Dot.Integer("skin", value, friend.Skin);
            friend.Frame = EB.Dot.String("headFrame", value, friend.Frame);
            friend.Fight = EB.Dot.Integer("battleRating", value, friend.Fight);
			friend.AlliName = EB.Dot.String("allianceName", value, friend.AlliName);
			friend.OfflineTime = EB.Dot.Single("offlineTime", value, friend.OfflineTime);
			friend.IsFriend = EB.Dot.Bool("isFriend", value, friend.IsFriend);
			friend.IsHaveSendVigor = EB.Dot.Bool("isHaveSendVigor", value, friend.IsHaveSendVigor);
			friend.IsCanReceiveVigor = EB.Dot.Bool("isCanReceiveVigor", value, friend.IsCanReceiveVigor);
			friend.LikeState =(eLikeState)EB.Dot.Integer("likeState", value, 0);
			friend.LikeNum = EB.Dot.Integer("likeNum", value, friend.LikeNum);
			friend.IsSelect = false;
			return friend;
		}
	}

	public class RecentlyList : FriendList<FriendData>
	{
		public override FriendData Parse(object value, long uid)
		{
			if (value == null)
			{
				return null;
			}

			FriendData friend = Find(uid) ?? new FriendData();
			friend.Uid = EB.Dot.Long("uid", value, friend.Uid);
			friend.Type = FriendManager.Instance.ParseFriendType("type", value, friend.Type);
			friend.Name = EB.Dot.String("name", value, friend.Name);
			friend.Level = EB.Dot.Integer("level", value, friend.Level);
			friend.Head = EB.Dot.String("portrait", value, friend.Head);
			friend.Skin = EB.Dot.Integer("skin", value, friend.Skin);
            friend.Frame = EB.Dot.String("headFrame", value, friend.Frame);
            friend.Fight = EB.Dot.Integer("battleRating", value, friend.Fight);
			friend.AlliName = EB.Dot.String("allianceName", value, friend.AlliName);
			friend.OfflineTime = EB.Dot.Single("offlineTime", value, friend.OfflineTime);
			friend.IsFriend = EB.Dot.Bool("isFriend", value, friend.IsFriend);
			friend.IsHaveSendVigor = EB.Dot.Bool("isHaveSendVigor", value, friend.IsHaveSendVigor);
			friend.IsCanReceiveVigor = EB.Dot.Bool("isCanReceiveVigor", value, friend.IsCanReceiveVigor);
			friend.LikeState = (eLikeState)EB.Dot.Integer("likeState", value, 0);
			friend.LikeNum = EB.Dot.Integer("likeNum", value, friend.LikeNum);
			friend.IsSelect = false;
			return friend;
		}
	}

	public class TeamList : FriendList<FriendData>
	{
		public override FriendData Parse(object value, long uid)
		{
			if (value == null)
			{
				return null;
			}

			FriendData friend = Find(uid) ?? new FriendData();
			friend.Uid = EB.Dot.Long("uid", value, friend.Uid);
			friend.Type = FriendManager.Instance.ParseFriendType("type", value, friend.Type);
			friend.Name = EB.Dot.String("name", value, friend.Name);
			friend.Level = EB.Dot.Integer("level", value, friend.Level);
			friend.Head = EB.Dot.String("portrait", value, friend.Head);
			friend.Skin = EB.Dot.Integer("skin", value, friend.Skin);
            friend.Frame = EB.Dot.String("headFrame", value, friend.Frame);
            friend.Fight = EB.Dot.Integer("battleRating", value, friend.Fight);
			friend.AlliName = EB.Dot.String("allianceName", value, friend.AlliName);
			friend.OfflineTime = EB.Dot.Single("offlineTime", value, friend.OfflineTime);
			friend.IsFriend = EB.Dot.Bool("isFriend", value, friend.IsFriend);
			friend.IsHaveSendVigor = EB.Dot.Bool("isHaveSendVigor", value, friend.IsHaveSendVigor);
			friend.IsCanReceiveVigor = EB.Dot.Bool("isCanReceiveVigor", value, friend.IsCanReceiveVigor);
			friend.LikeState = (eLikeState)EB.Dot.Integer("likeState", value, 0);
			friend.LikeNum = EB.Dot.Integer("likeNum", value, friend.LikeNum);
			friend.IsSelect = false;
			return friend;
		}
	}

	public class RecommendList : FriendList<RecommendFriendData>
	{
		public override RecommendFriendData Parse(object value, long uid)
		{
			if (value == null)
			{
				return null;
			}

			RecommendFriendData friend = Find(uid) ?? new RecommendFriendData();
			friend.Uid = EB.Dot.Long("uid", value, friend.Uid);
			friend.Type =FriendManager.Instance.ParseFriendType("type", value, friend.Type);
			friend.Name = EB.Dot.String("name", value, friend.Name);
			friend.Level = EB.Dot.Integer("level", value, friend.Level);
			friend.Head = EB.Dot.String("portrait", value, "15011");
			friend.Skin = EB.Dot.Integer("skin", value, friend.Skin);
            friend.Frame = EB.Dot.String("headFrame", value, friend.Frame);
            friend.Fight = EB.Dot.Integer("battleRating", value, friend.Fight);
			friend.AlliName = EB.Dot.String("allianceName", value, friend.AlliName);
			friend.OfflineTime = EB.Dot.Single("offlineTime", value, friend.OfflineTime);
			friend.IsFriend = EB.Dot.Bool("isFriend", value, friend.IsFriend);
			friend.Desc = EB.Dot.String("desc", value, friend.Desc);
			return friend;
		}
	}

	public class SearchList : FriendList<RecommendFriendData>
	{
		public override RecommendFriendData Parse(object value, long uid)
		{
			if (value == null)
			{
				return null;
			}

			RecommendFriendData friend = Find(uid) ?? new RecommendFriendData();
			friend.Uid = EB.Dot.Long("uid", value, friend.Uid);
			friend.Type =FriendManager.Instance.ParseFriendType("type", value, friend.Type);
			friend.Name = EB.Dot.String("name", value, friend.Name);
			friend.Level = EB.Dot.Integer("level", value, friend.Level);
			friend.Head = EB.Dot.String("portrait", value,"15011");
			friend.Skin = EB.Dot.Integer("skin", value, friend.Skin);
            friend.Frame = EB.Dot.String("headFrame", value, friend.Frame);
            friend.Fight = EB.Dot.Integer("battleRating", value, friend.Fight);
			friend.AlliName = EB.Dot.String("allianceName", value, friend.AlliName);
			friend.OfflineTime = EB.Dot.Single("offlineTime", value, friend.OfflineTime);
			friend.IsFriend = EB.Dot.Bool("isFriend", value, friend.IsFriend);
			friend.Desc = EB.Dot.String("desc", value, friend.Desc);
			return friend;
		}
	}

	public class ApplyList : FriendList<ApplyFriendData>
	{
		public override ApplyFriendData Parse(object value, long uid)
		{
			if (value == null)
			{
				return null;
			}

			ApplyFriendData friend = Find(uid) ?? new ApplyFriendData();
			friend.Uid = EB.Dot.Long("uid", value, friend.Uid);
			friend.Type =FriendManager.Instance.ParseFriendType("type", value, friend.Type);
			friend.Name = EB.Dot.String("name", value, friend.Name);
			friend.Level = EB.Dot.Integer("level", value, friend.Level);
			friend.Head = EB.Dot.String("portrait", value, friend.Head);
			friend.Skin = EB.Dot.Integer("skin", value, friend.Skin);
            friend.Frame = EB.Dot.String("headFrame", value, friend.Frame);
            friend.Fight = EB.Dot.Integer("battleRating", value, friend.Fight);
			friend.AlliName = EB.Dot.String("allianceName", value, friend.AlliName);
			friend.OfflineTime = EB.Dot.Single("offlineTime", value, friend.OfflineTime);
			friend.IsFriend = EB.Dot.Bool("isFriend", value, friend.IsFriend);
			friend.VerifyInfo = EB.Dot.String("verifyInfo", value, friend.VerifyInfo);
			return friend;
		}
	}

	public class FriendList<T> : INodeData where T : BaseFriendData
	{
		public bool IsHaveUpdate;
		public List<T> List
		{
			get; set;
		}

		public void CleanUp()
		{
			List.Clear();
		}

		public object Clone()
		{
			return new FriendList<T>();
		}

		public virtual void OnUdpateAfter()
		{

		}

		public void OnUpdate(object obj)
		{
			IsHaveUpdate = true;
			List = Hotfix_LT.EBCore.Dot.List<T, long>(null, obj, List, Parse);

			OnUdpateAfter();
		}

		public void OnMerge(object obj)
		{
			List = Hotfix_LT.EBCore.Dot.List<T, long>(null, obj, List, Parse, (friend, uid) => friend.Uid == uid);
		}

		public virtual T Parse(object value, long uid)
		{
			return default(T);
		}

		public FriendList()
		{
			List = new List<T>();
		}

		public T Find(long uid)
		{
			return List.Find(f => f.Uid == uid);
		}

		public void Add(T friend)
		{
			List.Add(friend);
		}

		public void Remove(long uid)
		{
			List.RemoveAll(f => f.Uid == uid);
		}
	}
}