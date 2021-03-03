using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace Hotfix_LT.UI
{
	public class ChatHistoryData
	{
		public long id;
		public float ts;
		public string content;
	}

	public class ChatHistoryController
	{
		private int maxFileCount = 7;
		private string folderName
		{
			get
			{
				return Application.persistentDataPath + "//Chat//" + "uid" + LoginManager.Instance.LocalUserId.Value;
			}
		}

		Queue<ChatHistoryData> m_writeThreadQueue = new Queue<ChatHistoryData>();
		Queue<Queue<string>> m_readThreadQueue = new Queue<Queue<string>>();
		bool m_threadStart = true;

		public delegate void LocalDataReadDelegate(List<ChatHistoryData> dataList);
		Queue<LocalDataReadDelegate> m_onReadDoneDelegateQueue = new Queue<LocalDataReadDelegate>();
		//Dictionary<long, Queue<string>> m_localFileNameList = new Dictionary<long, Queue<string>>();
		public Dictionary<long, List<EB.Sparx.ChatMessage>> ChatHistoryDic = new Dictionary<long, List<EB.Sparx.ChatMessage>>();

		public ChatHistoryController()
		{

		}

		public void ThreadUpdate()
		{
			if (m_threadStart)
			{
				while (m_readThreadQueue.Count > 0)
				{
					m_onReadDoneDelegateQueue.Dequeue()(ReadFile(m_readThreadQueue.Dequeue()));
				}

				while (m_writeThreadQueue.Count > 0)
				{
					WriteFile(m_writeThreadQueue.Dequeue());
				}
			}
		}

		public void StopThread()
		{
			m_threadStart = false;
		}

		public bool GetIsHaveHistory()
		{
			return Directory.Exists(folderName);
		}

		public void SaveData(ChatHistoryData data)
		{
			m_writeThreadQueue.Enqueue(data);
			ThreadUpdate();
		}

		public void SaveData(EB.Sparx.ChatMessage msg)
		{
			if (AddChatHistory(msg))
			{
				ChatHistoryData hd = new ChatHistoryData();
                hd.id = LTChatManager.Instance.GetTargetId(msg.uid, msg.privateUid);
                hd.content = EB.JSON.Stringify(msg.json);
				hd.ts = msg.ts;
				SaveData(hd);
			}
		}

		public void GetData(Queue<string> fileQueue, LocalDataReadDelegate del)
		{
			m_readThreadQueue.Enqueue(fileQueue);
			m_onReadDoneDelegateQueue.Enqueue(del);
			ThreadUpdate();
		}

		public void WriteFile(ChatHistoryData data)
		{
			if (!Directory.Exists(folderName))
			{
				Directory.CreateDirectory(folderName);
			}

			string folderNameWithID = folderName + "//" + data.id;
			if (!Directory.Exists(folderNameWithID))
			{
				Directory.CreateDirectory(folderNameWithID);
			}

			if (data == null)
				return;

			System.DateTime dt = EB.Time.FromPosixTime(data.ts).ToLocalTime();
			string fileName = dt.ToString("yyyyMMdd");
			EB.Debug.Log("write yyyymmdd format fileName={0}", fileName);
			FileStream fs = new FileStream(folderNameWithID + "//" + fileName, FileMode.Append);
			BinaryWriter bw = new BinaryWriter(fs);
			bw.Write(System.Text.Encoding.UTF8.GetByteCount(data.content));
			bw.Write(System.Text.Encoding.UTF8.GetBytes(data.content));
			bw.Close();
			fs.Close();
			fs.Dispose();

			EB.Debug.Log("WriteFile targetId={0}--------over", data.id);
		}

		public List<ChatHistoryData> ReadFile(Queue<string> fileNames)
		{
			List<ChatHistoryData> dataList = new List<ChatHistoryData>();

			while (fileNames.Count > 0)
			{
				string fileName = fileNames.Dequeue();
				FileStream fs = new FileStream(fileName, FileMode.Open);
				BinaryReader br = new BinaryReader(fs);

				if (fs == null)
				{
					br.Close();
					fs.Close();
					fs.Dispose();
					EB.Debug.LogError("fs=null filename={0}",fileName);
					continue;
				}
				try
				{
					while (true)
					{
						ChatHistoryData data = new ChatHistoryData();
						int contentLength = br.ReadInt32();
						data.content = System.Text.Encoding.UTF8.GetString(br.ReadBytes(contentLength));
						dataList.Add(data);
					}
				}
				catch (Exception)
				{
					EB.Debug.Log("ReadFile:{0}----done!", fileName);
				}
				br.Close();
				fs.Close();
				fs.Dispose();
			}
			return dataList;
		}

		public bool AddChatHistory(EB.Sparx.ChatMessage msg)
		{
			long targetId = LTChatManager.Instance.GetTargetId(msg.uid, msg.privateUid);

            if (!ChatHistoryDic.ContainsKey(targetId))
			{
				List<EB.Sparx.ChatMessage> msgList = new List<EB.Sparx.ChatMessage>();
				msgList.Add(msg);
				ChatHistoryDic.Add(targetId, msgList);
				return true;
			}
			else
			{
				if (ChatHistoryDic[targetId].Count > 50)
				{
					EB.Debug.Log("remove msg targetId={0}",targetId);
					ChatHistoryDic[targetId].RemoveAt(0);
				}
				ChatHistoryDic[targetId].Add(msg);
				return true;
			}
		}

		/// <summary>  
		/// 通过好友id找到聊天记录，一次获取七天的聊天信息   
		/// </summary>  
		/// <param name="id"></param>  
		public void GetChatHistory(long uid, Action<List<EB.Sparx.ChatMessage>> del)
		{
			if (ChatHistoryDic.ContainsKey(uid))
			{
				del(ChatHistoryDic[uid]);
				return;
			}

			//List<EB.Sparx.ChatMessage> msgs = new List<EB.Sparx.ChatMessage>();
			GetHistoryByID(uid, delegate (List<ChatHistoryData> list)
			{
				if (list == null)
				{
					del(new List<EB.Sparx.ChatMessage>());
					return;
				}

				for (int i=0;i<list.Count;i++)
				{
					if (!string.IsNullOrEmpty(list[i].content) && list[i].content != "null")
					{
						EB.Sparx.ChatMessage msg = EB.Sparx.ChatMessage.Parse(EB.JSON.Parse(list[i].content));
						AddChatHistory(msg);
					}
					//msgs.Add(msg);
				}

				if (ChatHistoryDic.ContainsKey(uid))
				{
					ChatHistoryDic[uid].Sort(delegate (EB.Sparx.ChatMessage x, EB.Sparx.ChatMessage y)
					{
						return (int)(x.ts - y.ts);
					});
					del(ChatHistoryDic[uid]);
				}
				else
					del(new List<EB.Sparx.ChatMessage>());
			});
		}

		private void GetHistoryByID(long id, LocalDataReadDelegate del)
		{
			string folderNameWithID = folderName + "/" + id;

			if (!GetIsHaveHistory())
			{
				Directory.CreateDirectory(folderName);
			}

			if (!Directory.Exists(folderNameWithID))
			{
				if (del != null)
					del(new List<ChatHistoryData>());
				return;
			}

			string[] fileArr = Directory.GetFiles(folderNameWithID);
			List<string> fileList = fileArr.ToList<string>();
			fileList.Sort(delegate (string left, string right)
			{
				int length = left.Length;
				string leftFile = left.Substring(length - 8, 8);
				string rightFile = right.Substring(length - 8, 8);

				int sizeLeft, sizeRight;
				int.TryParse(leftFile, out sizeLeft);
				int.TryParse(rightFile, out sizeRight);

				return sizeLeft - sizeRight;
			});

			if (fileList.Count > maxFileCount)
			{
				int count = fileList.Count;
				for (int i = 0; i < count - maxFileCount; i++)
				{
					string path = fileList[0];
					fileList.RemoveAt(0);
					File.Delete(path);
				}
			}

			Queue<string> fileQueue = new Queue<string>();
			for (int i = fileList.Count - 1; i >= 0; --i)
			{
				fileQueue.Enqueue(fileList[i]);
			}

			if (fileQueue.Count > 0)
				GetData(fileQueue, del);
		}

		public void ReadAudio(long uid, long ts)
		{
			string tag = string.Format("{0}-{1}", uid, ts);
			PlayerPrefs.SetInt(tag, 1);
		}

		public bool GetAudioIsRead(long uid, long ts)
		{
			string tag = string.Format("{0}-{1}", uid, ts);
			int intValue = PlayerPrefs.GetInt(tag, 0);
			return intValue > 0;
		}
	}
}