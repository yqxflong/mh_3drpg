using Hotfix_LT.Data;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTGameSettingManager
    {
        private static LTGameSettingManager instance;

        public static LTGameSettingManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LTGameSettingManager();
                    Hotfix_LT.Messenger.AddListener(EventName.AllRedPointDataRefresh, instance.DataRefresh);
                }
                return instance;
            }
        }

        private const string HeadFrameDataID = "userHeadFrame.head_frame";

        private List<string> FrameItemsList = new List<string>();
        private Dictionary<string, int> HeadFrameDic = new Dictionary<string, int>();
        public void InitFrameData()
        {
            //更新获取当前装备的id
            var temp = EconemyTemplateManager.Instance.GetAllHeadFrame();
            Hashtable Data;
            HeadFrameDic.Clear();
            FrameItemsList.Clear();
            FrameItemsList.Add("0_0");
            DataLookupsCache.Instance.SearchDataByID<Hashtable>(HeadFrameDataID, out Data);
            if (Data != null)
            {
                foreach (DictionaryEntry data in Data)
                {
                    string Id = data.Key.ToString();
                    if (!HeadFrameDic.ContainsKey(Id))
                    {
                        int Num = int.Parse(data.Value.ToString());// EB.Dot.Integer(Id, data, 0);
                        HeadFrameDic.Add(Id, Num);
                    }
                    else
                    {
                        int Num = int.Parse(data.Value.ToString());
                        HeadFrameDic[Id] = Num;
                    }
                }
            }
            for (int i = 0; i < temp.Count; ++i)
            {
                if (HeadFrameDic.ContainsKey(temp[i].id) && HeadFrameDic[temp[i].id] >= temp[i].num)
                {
                    FrameItemsList.Add(string.Format("{0}_{1}", temp[i].id, temp[i].num));
                }
            }


        }

        private void DataRefresh()
        {
            var temp = EconemyTemplateManager.Instance.GetAllHeadFrame();
            Hashtable Data;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>(HeadFrameDataID, out Data);
            if (Data != null)
            {
                foreach (DictionaryEntry data in Data)
                {
                    string Id = data.Key.ToString();
                    if (!HeadFrameDic.ContainsKey(Id))
                    {
                        int Num = int.Parse(data.Value.ToString());
                        HeadFrameDic.Add(Id, Num);
                    }
                    else
                    {
                        int Num = int.Parse(data.Value.ToString());
                        HeadFrameDic[Id] = Num;
                    }
                }
            }
            for (int i = 0; i < temp.Count; ++i)
            {
                string str = string.Format("{0}_{1}", temp[i].id, temp[i].num);
                if (FrameItemsList.Contains(str)) continue;
                if (HeadFrameDic.ContainsKey(temp[i].id) && HeadFrameDic[temp[i].id] >= temp[i].num)
                {
                    LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.otherSetting, 1);
                    return;
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.otherSetting, 0);
        }

        public bool HasFrame(string temp)
        {
            return FrameItemsList.Contains(temp);
        }

        public void ClickFrame(string temp)
        {
            FrameItemsList.Add(temp);
            DataRefresh();
        }
        
        public void RequestRedeemCode(string codeId, System.Action<bool> callback)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/redeemcode/useRedeemCode");
            request.AddData("redeemCodeId", codeId);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                //DataLookupsCache.Instance.CacheData(data);
                callback(data!=null);
            });
        }
    }
}
