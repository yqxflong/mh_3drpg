using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace Hotfix_LT.UI
{
    public class AllChatHistoryData
    {
        public ChatRule.CHAT_CHANNEL channel;
        public float ts;
        public string content;
    }

    public class AllChatHistoryController
    {

        private int maxFileCount = 7;
        private string folderName
        {
            get
            {
                return
                Application.persistentDataPath + "//AllChat//" + "uid" + LoginManager.Instance.LocalUserId.Value;
            }
        }

        public delegate void LocalDataReadDelegate(List<AllChatHistoryData> dataList);
        Queue<AllChatHistoryData> m_writeThreadQueue = new Queue<AllChatHistoryData>();
        Queue<Queue<string>> m_readThreadQueue = new Queue<Queue<string>>();
        Queue<LocalDataReadDelegate> m_onReadDoneDelegateQueue = new Queue<LocalDataReadDelegate>();

        private Dictionary<ChatRule.CHAT_CHANNEL, List<EB.Sparx.ChatMessage>> AllChatHistoryDic = new Dictionary<ChatRule.CHAT_CHANNEL, List<EB.Sparx.ChatMessage>>();

        public void ThreadUpdate()
        {
            while (m_readThreadQueue.Count > 0)
            {
                m_onReadDoneDelegateQueue.Dequeue()(ReadFile(m_readThreadQueue.Dequeue()));
                break;
            }

            while (m_writeThreadQueue.Count > 0)
            {
                WriteFile(m_writeThreadQueue.Dequeue());
                break;
            }
        }

        public bool GetIsHaveHistory()
        {
            return Directory.Exists(folderName);
        }

        public void SaveData(AllChatHistoryData data)
        {
            m_writeThreadQueue.Enqueue(data);
            ThreadUpdate();
        }

        public void SaveData(EB.Sparx.ChatMessage msg, ChatRule.CHAT_CHANNEL channel)
        {
            if (AddChatHistory(msg, channel))
            {
                //世界聊天没必要保存到本地
                if (channel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD)
                {
                	return;
                }
                AllChatHistoryData hd = new AllChatHistoryData();
                hd.channel = channel;
                if (msg.json != null)
                    hd.content = EB.JSON.Stringify(msg.json);
                else
                    hd.content = EB.JSON.Stringify(makeJson(msg));
                hd.ts = msg.ts;
                SaveData(hd);
            }
        }

        private object makeJson(EB.Sparx.ChatMessage msg)
        {
            Hashtable json = Johny.HashtablePool.Claim();
            Hashtable attributes = Johny.HashtablePool.Claim();
            attributes.Add("vip_level", msg.vipLevel);
            attributes.Add("quality", msg.quality);
            attributes.Add("is_audio", msg.isAudio);
            attributes.Add("name", msg.name);
            attributes.Add("head_icon", msg.icon);
            attributes.Add("head_frame", msg.frame);
            attributes.Add("battle_rating", msg.battleRating);
            attributes.Add("uid", msg.uid);
            attributes.Add("level", msg.level);
            attributes.Add("channel_type", msg.channelType);
            attributes.Add("audio", msg.audioClip);
            attributes.Add("alliance_name", msg.allianceName);
            json.Add("uid", msg.uid);
            json.Add("attributes", attributes);
            json.Add("name", msg.name);
            json.Add("ts", msg.ts);
            json.Add("lower", msg.lower);
            json.Add("channel", msg.channel);
            json.Add("locale", msg.ip);
            json.Add("filtered", msg.text);
            json.Add("id", msg.id);
            json.Add("text", msg.text);
            json.Add("lang", msg.language);
            msg.json = json;
            return json;
        }


        public void GetData(Queue<string> fileQueue, LocalDataReadDelegate del)
        {
            m_readThreadQueue.Enqueue(fileQueue);
            m_onReadDoneDelegateQueue.Enqueue(del);
            ThreadUpdate();
        }

        public bool AddChatHistory(EB.Sparx.ChatMessage msg, ChatRule.CHAT_CHANNEL channel)
        {
            if (!AllChatHistoryDic.ContainsKey(channel))
            {
                List<EB.Sparx.ChatMessage> msgList = new List<EB.Sparx.ChatMessage>();
                msgList.Add(msg);
                AllChatHistoryDic.Add(channel, msgList);
                return true;
            }
            else
            {
                if (AllChatHistoryDic[channel].Count >= ChatHudController.MaxHistoryCount)
                {
                    //EB.Debug.Log("remove msg channel=" + channel);
                    AllChatHistoryDic[channel].RemoveAt(0);
                }
                AllChatHistoryDic[channel].Add(msg);
                return true;
            }
        }

        public List<AllChatHistoryData> ReadFile(Queue<string> fileNames)
        {
            List<AllChatHistoryData> dataList = new List<AllChatHistoryData>();

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
                    EB.Debug.LogError("fs=null filename={0}" , fileName);
                    continue;
                }
                try
                {
                    while (true)
                    {
                        AllChatHistoryData data = new AllChatHistoryData();
                        int contentLength = br.ReadInt32();
                        data.content = System.Text.Encoding.UTF8.GetString(br.ReadBytes(contentLength));
                        dataList.Add(data);
                    }
                }
                catch (Exception)
                {
                    EB.Debug.Log("ReadFile:{0}----done!",fileName);
                }
                br.Close();
                fs.Close();
                fs.Dispose();
            }
            return dataList;
        }

        public void WriteFile(AllChatHistoryData data)
        {
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            string folderNameWithID = folderName + "//" + data.channel;
            if (!Directory.Exists(folderNameWithID))
            {
                Directory.CreateDirectory(folderNameWithID);
            }

            if (data == null)
                return;

            System.DateTime dt = EB.Time.FromPosixTime(data.ts).ToLocalTime();
            string fileName = dt.ToString("yyyyMMdd");
            //Debug.Log("write yyyymmdd format fileName=" + fileName);
            FileStream fs = new FileStream(folderNameWithID + "//" + fileName, FileMode.Append);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(System.Text.Encoding.UTF8.GetByteCount(data.content));
            bw.Write(System.Text.Encoding.UTF8.GetBytes(data.content));
            bw.Close();
            fs.Close();
            fs.Dispose();

            //Debug.Log("WriteFile targetId=" + data.channel + "--------over");
        }

        /// <summary>  
        /// 通过好友频道找到聊天记录，一次获取七天的聊天信息   
        /// </summary>  
        /// <param name="id"></param>  
        public void GetAllChatHistory(ChatRule.CHAT_CHANNEL channel, Action<List<EB.Sparx.ChatMessage>> del)
        {
            if (AllChatHistoryDic.ContainsKey(channel))
            {
                del(AllChatHistoryDic[channel]);
                return;
            }

            GetHistoryByChannel(channel, delegate (List<AllChatHistoryData> list)
            {
                if (list == null)
                {
                    del(new List<EB.Sparx.ChatMessage>());
                    return;
                }

                for (int i = 0; i < list.Count; i++)
                {
                    object json = EB.JSON.Parse(list[i].content);
                    if (json!=null)
                    {
                        EB.Sparx.ChatMessage msg = EB.Sparx.ChatMessage.Parse(json);
                        AddChatHistory(msg, channel);
                    }
                }

                if (AllChatHistoryDic.ContainsKey(channel))
                {
                    AllChatHistoryDic[channel].Sort(delegate (EB.Sparx.ChatMessage x, EB.Sparx.ChatMessage y)
                    {
                        return (int)(x.ts - y.ts);
                    });
                    del(AllChatHistoryDic[channel]);
                }
                else
                    del(new List<EB.Sparx.ChatMessage>());
            });
        }

        private void GetHistoryByChannel(ChatRule.CHAT_CHANNEL channel, LocalDataReadDelegate del)
        {
            string folderNameWithChannel = folderName + "/" + channel;

            if (!GetIsHaveHistory())
            {
                Directory.CreateDirectory(folderName);
            }

            if (!Directory.Exists(folderNameWithChannel))
            {
                if (del != null)
                    del(new List<AllChatHistoryData>());
                return;
            }

            string[] fileArr = Directory.GetFiles(folderNameWithChannel);
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

        public List<EB.Sparx.ChatMessage> GetAllChatList()
        {
            List<EB.Sparx.ChatMessage> chatList = new List<EB.Sparx.ChatMessage>();
            var iter = AllChatHistoryDic.GetEnumerator();
            while (iter.MoveNext())
            {
                chatList.AddRange(iter.Current.Value);
            }

            chatList.Sort((EB.Sparx.ChatMessage x, EB.Sparx.ChatMessage y) => { return (int)(x.ts - y.ts); });

            return chatList;
        }

        public void ClearAllChatList()
        {
            AllChatHistoryDic.Clear();
        }
    }
}
