using System.Collections;
using EB.Sparx;
using System;
using System.Collections.Generic;
using Hotfix_LT.Data;
using UnityEngine;
using System.Linq;
using EB;

namespace Hotfix_LT.UI
{
	// 1.接口要根据server修改
	// 2.字符key要根据填入表的key修改
	public class LTVIPDataManager : ManagerUnit
	{
		public override void Dispose() { }
		public override void Connect() { }
		public override void Disconnect(bool isLogout) { }

		private static LTVIPDataManager instance = null;

		public static LTVIPDataManager Instance
		{
			get { return instance = instance ?? LTHotfixManager.GetManager<LTVIPDataManager>(); }
		}

		private VIPData vipData;
		private LTVIPDataApi api;
		public LTVIPDataApi API
		{
			get { if(api == null) api = new LTVIPDataApi(); return api; } private set { if (value != api) api = value; }
		}

		public override void Initialize(Config config)
		{
			Instance.vipData = new VIPData();
			Instance.API.ErrorHandler += ErrorHandler;			

			Hotfix_LT.Messenger.AddListener(EventName.AllRedPointDataRefresh, Instance.DataRefresh);
		}

		public override void OnLoggedIn()
		{
			base.OnLoggedIn();
			ResetData();
			UpdateVIPBaseData();
		}

		private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
		{
			return false;
		}

		private void DataRefresh()
		{
			if(vipData != null && vipData.Level <= 0)
			{
				UpdateVIPBaseData();
			}
			SetReceivedRedPointStatus();
		}

		private void ResetData()
		{
			vipData = new VIPData();
		}

		public void UpdateVIPBaseData()
		{
			if(vipData != null)
			{
				int remoteLevel = BalanceResourceUtil.VipLevel;
				if (vipData.Level > 0 && remoteLevel > 0)
				{
					if(remoteLevel != vipData.Level)
						PlayerPrefs.SetInt(LoginManager.Instance.LocalUserId.Value + "VIPLevel", vipData.Level);
				}
				vipData.Level = remoteLevel;
				int charge = BalanceResourceUtil.VipHc;
				if (vipData.Level <= 0 && charge > 0)
				{
					int curLevel = VIPTemplateManager.Instance.GetLevelByCharge(charge);
					if(curLevel != vipData.Level)
						PlayerPrefs.SetInt(LoginManager.Instance.LocalUserId.Value + "VIPLevel", vipData.Level);
					vipData.Level = curLevel;
				}
				int basic = VIPTemplateManager.Instance.GetNeedChargeNum(vipData.Level);
				vipData.CurrentExp = charge /*- basic*/;
				vipData.NeedExp = VIPTemplateManager.Instance.GetNeedChargeNum(vipData.Level+1) /*- basic*/;
				
				if(vipData.Level == 0)
				{
					int value = PlayerPrefs.GetInt(LoginManager.Instance.LocalUserId.Value + "VIPLevel", 0);
					if (value != 0) PlayerPrefs.SetInt(LoginManager.Instance.LocalUserId.Value + "VIPLevel", vipData.Level);
				}

				if(vipData.VipCertificates == null)
				{
					string vipLicence = NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("VipTextDescriptionShow");
					vipData.VipCertificates = vipLicence.Split(',');
				}
			}
		}

		public void RequestCollectGifts(int level, System.Action<bool> callback)
		{
			UnityEngine.Debug.Log("VIPDataMgr.Say: Request Collect Gifts!");
			API.ReceiveGifts(level, delegate (Hashtable data) {
				if (data != null)
				{
					DataLookupsCache.Instance.CacheData(data);
					UnityEngine.Debug.Log("VIPDataMgr.Say: Got Receive Gift! ... Count = " + data.Count);
					int result = EB.Dot.Integer("vip_got_gift_result", data, 0);
					level = level <= 0 ? GetCheckedLevel() : level;
					DataLookupsCache.Instance.SetCache("vip_reward." + level, true, true);
					callback?.Invoke(result == 1);
				}
				else
					UnityEngine.Debug.LogError("VIPDataMgr.Say: Could not got gift! ... Fail");
			});
		}

		public bool IsGiftsReceived(int level)
		{
			if (!DataLookupsCache.Instance.SearchDataByID<bool>("vip_reward." + level, out bool value))
				value = false;
			return value;
		}

		public void UpdateData(System.Action callback)
		{
			
			vipData.RecordLevel = PlayerPrefs.GetInt(LoginManager.Instance.LocalUserId.Value + "_VIPLevel", 0);
			if (vipData.Level != vipData.RecordLevel || !vipData.HasCreated)
			{
				UpdatePrivilege();
				UpdateGifts(vipData.Level);
				//vipData.RecordLevel = vipData.Level;
				//PlayerPrefs.SetInt("VIPLevel", vipData.RecordLevel);
				vipData.HasCreated = true;
			}
			vipData.CheckedLevel = vipData.Level;
			callback?.Invoke();
		}

		public VIPGiftStatus GetTheVIPLevelGiftStatus(int level)
		{
			return level <= vipData.Level ? (IsGiftsReceived(level) ? VIPGiftStatus.Received : VIPGiftStatus.Uncollected) : VIPGiftStatus.Locked;
		}

		public bool IsExistsGreaterOrLessThanTheLevel(int level, bool isGreater)
		{
			if(isGreater)
			{
				return VIPTemplateManager.Instance.PrivilegeList.Exists(p => {
					return p.Level <= vipData.Level && p.Level > level ? (IsGiftsReceived(p.Level) ? false : true) : false;
				});
			}
			else
			{
				return VIPTemplateManager.Instance.PrivilegeList.Exists(p => {
					return p.Level <= vipData.Level && p.Level < level ? (IsGiftsReceived(p.Level) ? false : true) : false;
				});
			}
		}

		private string GetVIPLevelGiftText(int level)
		{
			VIPPrivilege privilege = VIPTemplateManager.Instance.GetTemplate(level);
			return privilege != null ? privilege.GiftItemId : string.Empty;
		}

		private void UpdatePrivilege()
		{			 
			VIPPrivilege privilege = VIPTemplateManager.Instance.GetTemplate(vipData.Level);
			VIPPrivilege oldPrivilege = VIPTemplateManager.Instance.GetTemplate(vipData.Level-1);			

			Dictionary<string, int>.Enumerator numEnumerator = privilege.GetNumEnumerator();
			Dictionary<string, float>.Enumerator percentEnumerator = privilege.GetPercentEnumerator();
			while (numEnumerator.MoveNext())
			{
				string key = numEnumerator.Current.Key;
				if(vipData.VipCertificates.Contains(key))
				{
					int value = privilege.GetTotalNum(key);// + VIPTemplateManager.Instance.GetMonthVIPNum(key);
					AppendOrUpdatePrivilegeItem(ref vipData.Privileges, key, value, value > 0, oldPrivilege);
				}
			}
			while (percentEnumerator.MoveNext())
			{
				string key = percentEnumerator.Current.Key;
				if (vipData.VipCertificates.Contains(key))
				{
					float value = privilege.GetPercent(key);
					bool valid = key.Equals(VIPPrivilegeKey.ShopDiscount) ? value > 0 && value < 1 : value > 0;
					AppendOrUpdatePrivilegeItem(ref vipData.Privileges, key, value, valid, oldPrivilege);
				}
			}

			OnEquipmentGift(ref vipData.Privileges, privilege, oldPrivilege);
		}

		private void AppendOrUpdatePrivilegeItem(ref Dictionary<string, VIPPrivilegeItem> privileges, string key, object value, bool valid, VIPPrivilege comparer)
		{
			Dictionary<string, VIPPrivilegeItem> oldPrivileges = GetPrivilegeDict(comparer);
			if(!valid)
			{
				privileges.Remove(key);
				return;
			}

			bool has = privileges.TryGetValue(key, out VIPPrivilegeItem item);
			if (item == null) item = new VIPPrivilegeItem();

			if (comparer == null || (comparer != null && !oldPrivileges.ContainsKey(key)))
			{				
				item.Status = comparer == null ? PrivilegeStatus.Ordinary : PrivilegeStatus.Newly;				
			}
			else
			{
				if (value is int)
				{
					int oldValue = comparer.GetTotalNum(key);
					item.Status = oldValue != (int)value ? PrivilegeStatus.HasChanged : PrivilegeStatus.Ordinary;
				}
				else
				{
					float oldValue = comparer.GetPercent(key);
					item.Status = oldValue != (float)value ? PrivilegeStatus.HasChanged : PrivilegeStatus.Ordinary;
				}
			}
			item.Format = GetPrivilegeKeyWords(key);
			item.Value = value is int ? (int)value : item.Value;
			item.Percent = value is float ? (float)value : item.Percent;
			if (!has) privileges.Add(key, item);
		}

		private Dictionary<string, VIPPrivilegeItem> GetPrivilegeDict(VIPPrivilege profile)
		{
			Dictionary<string, VIPPrivilegeItem> privileges = new Dictionary<string, VIPPrivilegeItem>();
			if (profile == null) return privileges;

			Dictionary<string, int>.Enumerator numEnumerator = profile.GetNumEnumerator();
			Dictionary<string, float>.Enumerator percentEnumerator = profile.GetPercentEnumerator();
			while (numEnumerator.MoveNext())
			{
				string key = numEnumerator.Current.Key;
				if (vipData.VipCertificates.Contains(key))
				{
					int value = profile.GetTotalNum(key);// + VIPTemplateManager.Instance.GetMonthVIPNum(key);
					if (value > 0)
					{
						VIPPrivilegeItem item = new VIPPrivilegeItem();
						item.Value = value;
						privileges.Add(key, item);
					}
				}
			}
			while (percentEnumerator.MoveNext())
			{
				string key = percentEnumerator.Current.Key;
				if (vipData.VipCertificates.Contains(key))
				{
					float value = profile.GetPercent(key);
					bool valid = key.Equals(VIPPrivilegeKey.ShopDiscount) ? value > 0 && value < 1 : value > 0;
					if (valid)
					{
						VIPPrivilegeItem item = new VIPPrivilegeItem();
						item.Percent = value;
						privileges.Add(key, item);
					}
				}
			}
			return privileges;
		}

		private void OnEquipmentGift(ref Dictionary<string, VIPPrivilegeItem> privileges, VIPPrivilege privilege, VIPPrivilege comparer)
		{
			VIPPrivilegeItem item = privileges.ContainsKey(VIPPrivilegeKey.EatEquipmentGift) ? privileges[VIPPrivilegeKey.EatEquipmentGift] : new VIPPrivilegeItem();
			if (!string.IsNullOrEmpty(privilege.EatEquipmentGift))
			{
				item.Format = GetPrivilegeKeyWords(VIPPrivilegeKey.EatEquipmentGift);
				item.Status = comparer == null ? PrivilegeStatus.Ordinary :
					(string.IsNullOrEmpty(comparer.EatEquipmentGift) ? PrivilegeStatus.Newly :
					(comparer.EatEquipmentGift.Equals(privilege.EatEquipmentGift) ? PrivilegeStatus.Ordinary : PrivilegeStatus.HasChanged));
				if(!privileges.ContainsKey(VIPPrivilegeKey.EatEquipmentGift)) privileges.Add(VIPPrivilegeKey.EatEquipmentGift, item);
				item.DataString = privilege.EatEquipmentGift;
			}
			else
				privileges.Remove(VIPPrivilegeKey.EatEquipmentGift);
		}

		private void UpdateBrowsedPrivilege(int curLev)
		{
			vipData.BrowsedPrivileges.Clear();

			VIPPrivilege oldPrivilege = VIPTemplateManager.Instance.GetTemplate(curLev - 1);
			VIPPrivilege privilege = VIPTemplateManager.Instance.GetTemplate(curLev);

			Dictionary<string, int>.Enumerator numEnumerator = privilege.GetNumEnumerator();
			Dictionary<string, float>.Enumerator percentEnumerator = privilege.GetPercentEnumerator();
			while (numEnumerator.MoveNext())
			{
				string key = numEnumerator.Current.Key;
				if(vipData.VipCertificates.Contains(key))
				{
					int value = privilege.GetTotalNum(key);// + VIPTemplateManager.Instance.GetMonthVIPNum(key);
					 //if (value > 0) AppendPrivilegeItem(key, value, curLev);
					AppendOrUpdatePrivilegeItem(ref vipData.BrowsedPrivileges, key, value, value > 0, oldPrivilege);
				}
			}
			while (percentEnumerator.MoveNext())
			{
				string key = percentEnumerator.Current.Key;
				if(vipData.VipCertificates.Contains(key))
				{
					float value = privilege.GetPercent(key);
					bool valid = key.Equals(VIPPrivilegeKey.ShopDiscount) ? value > 0 && value < 1 : value > 0;
					//if (value > 0) AppendPrivilegeItem(key, value, curLev);
					AppendOrUpdatePrivilegeItem(ref vipData.BrowsedPrivileges, key, value, valid, oldPrivilege);
				}
			}
			OnEquipmentGift(ref vipData.BrowsedPrivileges, privilege, oldPrivilege);

			vipData.CheckedLevel = curLev;
		}

		#region 弃用代码
		//private void AppendPrivilegeItem(string key, object value, int level)
		//{
		//	VIPPrivilegeItem item = new VIPPrivilegeItem();
		//	if (level > vipData.Level)
		//	{
		//		if (!vipData.Privileges.ContainsKey(key))
		//		{
		//			item.Status = PrivilegeStatus.Newly;
		//			item.Format = GetPrivilegeKeyWords(key);
		//			item.Value = value is int ? (int)value : item.Value;
		//			item.Percent = value is float ? (float)value : item.Percent;
		//		}
		//		else
		//		{
		//			VIPPrivilegeItem myItem = vipData.Privileges[key];
		//			if (value is int)
		//				item.Status = myItem.Value != (int)value ? PrivilegeStatus.HasChanged : PrivilegeStatus.Ordinary;
		//			else
		//				item.Status = myItem.Percent != (float)value ? PrivilegeStatus.HasChanged : PrivilegeStatus.Ordinary;
		//			item.Format = GetPrivilegeKeyWords(key);
		//			item.Value = value is int ? (int)value : item.Value;
		//			item.Percent = value is float ? (float)value : item.Percent;
		//		}
		//	}
		//	else
		//	{
		//		item.Status = vipData.Privileges.ContainsKey(key) ? PrivilegeStatus.Ordinary : PrivilegeStatus.Newly;
		//		item.Format = GetPrivilegeKeyWords(key);
		//		item.Value = value is int ? (int)value : item.Value;
		//		item.Percent = value is float ? (float)value : item.Percent;
		//	}
		//	vipData.BrowsedPrivileges.Add(key, item);
		//}
		#endregion

		public string GetPrivilegeKeyWords(string key)
		{		
			if(key.Equals(VIPPrivilegeKey.BuyVigorTimes))
			{
				return EB.Localizer.GetString("ID_CHARGE_VIGOR_BUY_TIME");
			}
			else if(key.Equals(VIPPrivilegeKey.BuyExpTimes))
			{
				return EB.Localizer.GetString("ID_CHARGE_EXP_BUY_TIME");
			}
			else if(key.Equals(VIPPrivilegeKey.BuyGoldTimes))
			{
				return EB.Localizer.GetString("ID_CHARGE_GOLD_BUY_TIME");
			}
			else if(key.Equals(VIPPrivilegeKey.ResignInTimes))
			{
				return EB.Localizer.GetString("ID_CHARGE_RESIGH_IN_TIME");
			}
			else
				return EB.Localizer.GetString("ID_CHARGE_" + key.ToUpper().Replace("TIMES", "TIME"));
		}

		public string GetPrivilegeContent(VIPPrivilegeItem item, string key)
		{
			string format = item.Format;
			string colorText = item.Status == PrivilegeStatus.Ordinary ? "[ECF5FD]" : (item.Status == PrivilegeStatus.Newly ? "[FFFF6E]" : "[41FC7B]");

			if (!key.Equals(VIPPrivilegeKey.EatEquipmentGift))
			{
				int multiplier = 0;
				string content = string.Format(format, (item.Value != 0) ? item.Value : item.Percent * multiplier);
                if (key.EndsWith("discount"))
                {
					multiplier = 10;
					string per =
					content = string.Format(format, global::UserData.Locale == EB.Language.English ?
					string.Format("{0}%", ((1f - item.Percent) * 100).ToString("0")) 
					: (item.Percent * multiplier).ToString("0"));
				}
                else
                {
					multiplier = 100;
					content = string.Format(format, (item.Value != 0) ? item.Value : item.Percent * multiplier);
				}

				//content = content.TrimNumberEnd();
				int index = item.Status == PrivilegeStatus.HasChanged ? (content.Contains("+") ? content.IndexOf("+") : 
					(content.Contains(":") ? content.IndexOf(":")+1 :format.IndexOf("{"))) : 0;
				return content.Length > index && index >= 0 ? content.Insert(index, colorText) : content;
			}
			else
			{
				string words = string.Empty;
				if(!string.IsNullOrEmpty(item.Format))
				{
					string[] array = item.DataString.Split('|');
					List<LTShowItemData> itemDatas = SceneTemplateManager.Instance.ParseShowItem(array[1]);

					itemDatas.ForEach(data => {
						//EconemyItemTemplate itemTemplate = EconemyTemplateManager.Instance.GetItem(data.id);
						if (item.Status == PrivilegeStatus.Ordinary || item.Status == PrivilegeStatus.Newly)
							words += colorText + StringUtil.Format(format, array[0], data.count/*, itemTemplate.Name*/) + "\n";
						else
							words += StringUtil.Format(format, array[0], colorText + data.count/*, itemTemplate.Name*/) + "\n";
					});
				}
				
				return words.TrimEnd('\n');
			}
		}

		private void UpdateGifts(int level)
		{
			vipData.Gifts.Clear();
			vipData.Gifts = SceneTemplateManager.Instance.ParseShowItem(GetVIPLevelGiftText(level));
		}

		public void BrowsePrivilegesAndGiftsPage(int level, bool apply, System.Action<Dictionary<string, VIPPrivilegeItem>> callback)
		{
			if (vipData.Level == level)
			{
				if (apply)
				{
					UpdateData(() => {
						callback?.Invoke(vipData.Privileges);
					});
				}
				else
				{
					UpdateGifts(level);
					vipData.CheckedLevel = level;
					callback?.Invoke(vipData.Privileges);
				}
			}
			else
			{
				if (level != vipData.CheckedLevel)
				{
					UpdateGifts(level);
					UpdateBrowsedPrivilege(level);
				}
				else
				{
					if (apply)
					{
						UpdateData(() => {
							callback?.Invoke(vipData.Privileges);
						});
						return;
					}						
				}
				callback?.Invoke(vipData.BrowsedPrivileges);
			}
		}

		public void ResetCheckedLevel()
		{
			vipData.CheckedLevel = vipData.Level;
		}

		public int GetCheckedLevel()
		{
			int level = vipData.CheckedLevel < 0 ? vipData.Level : vipData.CheckedLevel;
			return level;
		}

		public List<LTShowItemData> GetVIPGiftItemData()
		{
			return vipData.Gifts;
		}

		public string GetVIPEXPRuleWords()
		{
			return EB.Localizer.GetString("ID_VIP_EXPRULE");
		}

		public void SetReceivedRedPointStatus()
		{
			bool hasGifts = IsExistsGiftUnreceived();			
			LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.vipgift, hasGifts ? 1 : 0);
		}

		public bool IsExistsGiftUnreceived()
		{
			return VIPTemplateManager.Instance.PrivilegeList.Exists(p => {
				return p.Level <= vipData.Level ? (IsGiftsReceived(p.Level) ? false : true) : false;
			});
		}

		public List<KeyValuePair<string, VIPPrivilegeItem>> GetSortPrivilegeList(Dictionary<string, VIPPrivilegeItem> dict)
		{
			var list = dict.ToList();
			list.Sort((left, right) =>
			{
				if ((int)left.Value.Status > (int)right.Value.Status)
					return -1;
				else if ((int)left.Value.Status == (int)right.Value.Status)
				{
					int leftPos = vipData.VipCertificates.IndexOf(left.Key);
					int rightPos = vipData.VipCertificates.IndexOf(right.Key);
					return leftPos.CompareTo(rightPos);
				}
				else
					return 1;
			});
			return list;
		}

		public VIPBaseInfo GetVIPBaseInfo()
		{
			VIPBaseInfo info = new VIPBaseInfo();
			info.Level = vipData.Level;
			info.NextLevelText = (vipData.Level+1).ToString();
			info.CurrentExp = vipData.CurrentExp;
			info.FullExp = vipData.NeedExp;
			info.NecessaryExp = vipData.NeedExp - vipData.CurrentExp;
			return info;
		}

		public VIPGiftStatus GetCurCheckedLevelGiftsStatus()
		{
			if (vipData.CheckedLevel <= vipData.Level)
				return IsGiftsReceived(vipData.CheckedLevel) ? VIPGiftStatus.Received : VIPGiftStatus.Uncollected;
			else
				return VIPGiftStatus.Locked;
		}

		public int GetLevelCount()
		{
			return VIPTemplateManager.Instance.GetVipLevelUpperLimit();
		}

		public int GetMaxShowLevel()
		{
			int maxLevel = (int)NewGameConfigTemplateManager.Instance.GetGameConfigValue("VipMaxLevelShow");
			return vipData.Level > maxLevel ? vipData.Level : (maxLevel <= GetLevelCount() ? maxLevel : GetLevelCount());
		}
	}

	public class VIPData
	{
		public int Level;
		public int CurrentExp;
		public int NeedExp;
		public Dictionary<string, VIPPrivilegeItem> Privileges = new Dictionary<string, VIPPrivilegeItem>();
		public int RecordLevel = -1;
		public int CheckedLevel = -1;
		public Dictionary<string, VIPPrivilegeItem> BrowsedPrivileges = new Dictionary<string, VIPPrivilegeItem>();
		public List<LTShowItemData> Gifts = new List<LTShowItemData>();
		public bool HasCreated;
		public string[] VipCertificates;
	}

	public struct VIPBaseInfo
	{
		public int Level;
		public string NextLevelText;
		public int CurrentExp;
		public int FullExp;
		public int NecessaryExp;
	}

	public enum PrivilegeStatus
	{
		Unknown = 0,
		Ordinary,		
		HasChanged,
		Newly
	}

	public class VIPPrivilegeItem
	{
		public string Format;
		public int Value;
		public float Percent;
		public string DataString;
		public PrivilegeStatus Status;
	}

	public enum VIPGiftStatus
	{
		Locked,
		Uncollected,
		Received
	}

	public class LTVIPDataApi : EB.Sparx.SparxAPI
	{
		public LTVIPDataApi()
		{
			endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
		}

		private void DefaultDataHandler(Hashtable data)
		{
			EB.Debug.Log("LTVIPDataApi.DefaultDataHandler: call default data handler");
		}

		private void ProcessResult(EB.Sparx.Response response, Action<Hashtable> dataHandler)
		{
			dataHandler = dataHandler ?? new Action<Hashtable>(DefaultDataHandler);
			if (ProcessResponse(response))
			{
				dataHandler(response.hashtable);
			}
			else
			{
				dataHandler(null);
			}
		}

		private int BlockService(EB.Sparx.Request request, Action<Hashtable> dataHandler)
		{
			LoadingSpinner.Show();

			return endPoint.Service(request, delegate (EB.Sparx.Response response)
			{
				LoadingSpinner.Hide();

				ProcessResult(response, dataHandler);
			});
		}

		private int Service(EB.Sparx.Request request, Action<Hashtable> dataHandler)
		{
			return endPoint.Service(request, delegate (EB.Sparx.Response response)
			{
				ProcessResult(response, dataHandler);
			});
		}

		public void ReceiveGifts(int level, Action<Hashtable> dataHandler)
		{
			EB.Sparx.Request request = endPoint.Post("/vip/buyVipPrize");
			request.AddData("level", level);
			BlockService(request, dataHandler);
		}
	}
}