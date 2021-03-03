using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hotfix_LT.UI
{
	public class MailRewards
	{
		public List<LTShowItemData> ItemList = new List<LTShowItemData>();
		//public int Gold, Hc;
		//public bool IsContainRes=false;
		public int ItemCount
		{
			get { return ItemList.Count; }
			private set { }
		}

		public LTShowItemData GetItem(int index)
		{
			return ItemList[index];
		}

		public void Add(LTShowItemData r)
		{
			ItemList.Add(r);
		}

		public void Clear()
		{
			if (ItemList != null)
				ItemList.Clear();
		}
	}

	public class MailItemData
	{
		public string MailId;
		public bool HasRead;
		public bool HasReceived;
		public string Title;
		public string Sender;
		public string Text;
		public string[] TextParams;
		public long Time;
		public bool IsContainReward;
		public MailRewards Rewards = new MailRewards();
		public bool IsSelect;
		public int ItemCount
		{
			get { return Rewards.ItemCount; }
		}

		public int ReMainTime;
	}

	public class MailList : INodeData
	{
		public bool DataUpdated;

		public MailList()
		{
			Mails = new List<MailItemData>();
		}

		public object Clone()
		{
			return new MailList();
		}

		public List<MailItemData> Mails
		{
			get; set;
		}

		public void CleanUp()
		{
			Mails.Clear();
		}

		public void OnUpdate(object obj)
		{
			DataUpdated = true;
			Mails = Hotfix_LT.EBCore.Dot.List<MailItemData, int>(null, obj, Mails, Parse);
			Messenger.Raise(EventName.MailUpdateEvent);
		}

		public void OnMerge(object obj)
		{
			//OnUpdate(obj);
			DataUpdated = true;
			Mails = Hotfix_LT.EBCore.Dot.List<MailItemData, int>(null, obj, Mails, Parse, (item, mailId) => item.MailId == mailId.ToString());
			Messenger.Raise(EventName.MailUpdateEvent);
		}

		private MailItemData Parse(object value, int mailId)
		{
			if (value == null)
				return null;

			MailItemData item = Find(mailId.ToString()) ?? new MailItemData();
			item.MailId = EB.Dot.String("mailId", value, item.MailId);
			item.HasRead = EB.Dot.Bool("hasRead", value, item.HasRead);
			item.Title = EB.Dot.String("mailTitle", value, item.Title);
			item.Text = EB.Dot.String("mailText", value, item.Text);
			item.TextParams = Hotfix_LT.EBCore.Dot.Array<string>("param", value, null, delegate (object val) { return val.ToString(); });
			item.Sender = EB.Dot.String("senderName", value, item.Sender);
			item.Time = EB.Dot.Long("createTime", value, item.Time);
			item.HasReceived = EB.Dot.Bool("hasReceived", value, item.HasReceived);
			item.ReMainTime = EB.Dot.Integer("remaing_time", value, item.ReMainTime);
			ArrayList giftArr = Hotfix_LT.EBCore.Dot.Array("gift", value, null);
			if (giftArr != null)
			{
				item.Rewards.Clear();
				//item.Reward.Hc = 0;
				//item.Reward.Gold = 0;
				for (int i = 0; i < giftArr.Count; ++i)
				{
					IDictionary dic = giftArr[i] as IDictionary;
					string type = EB.Dot.String("t", dic, "");
					string id = EB.Dot.String("n", dic, "");
					int quality = EB.Dot.Integer("q", dic, 0);
					LTShowItemData reward = new LTShowItemData(id, quality, type, false);
					item.Rewards.Add(reward);
				}
			}
			item.IsContainReward = !(giftArr == null);
			return item;
		}

		public MailItemData Find(string mailId)
		{
			MailItemData item = Mails.Where(m => m.MailId == mailId).FirstOrDefault();
			return item;
		}

		public void Remove(string mailId)
		{
			Mails.RemoveAll(m => m.MailId == mailId);
		}
	}
}