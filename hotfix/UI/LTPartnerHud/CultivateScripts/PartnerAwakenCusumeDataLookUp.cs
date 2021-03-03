using System.Collections.Generic;
using UnityEngine;

///可以用于显示时实时监测物品数量变化

namespace Hotfix_LT.UI
{
    public class PartnerAwakenCusumeDataLookUp : DataLookupHotfix
    {

        public UILabel[] NumLabelArray;
        //private Dictionary<string, int> materialDic = new Dictionary<string, int>();        
        private string colorstr,NeedCountStr;
        private int itemCount;
        private List<string> IDlist = new List<string>();
        private List<int> Countlist = new List<int>();

        public override void Awake()
        {
            base.Awake();
            if (mDL.ObjectParamList != null && mDL.ObjectParamList.Count > 0)
            {
                int count = mDL.ObjectParamList.Count;
                NumLabelArray = new UILabel[count];
                for (int i = 0; i < count; i++)
                {
                    NumLabelArray[i] = (mDL.ObjectParamList[i] as GameObject).GetComponent<UILabel>();
                }
            }
            
        }

        public void InitDataList(List<string> id, List<int> NeedNum, int NeedGold = 0, int NeedHc = 0)
        {
            if (id.Count != NeedNum.Count)
            {
                EB.Debug.LogError("ID count didnot equal to NeedNum Count,please check data");
                return;
            }
            ClearData();
            for (int i = 0; i < id.Count; i++)
            {
                IDlist.Add(id[i]);
                Countlist.Add(NeedNum[i]);
            }
            SetPriData(id, NeedNum, NeedGold, NeedHc);

        }

        private void SetPriData(List<string> id, List<int> NeedNum, int NeedGold, int NeedHc)
        {
            if (id.Count > 0)
            {
                mDL.DataIDList.Add("inventory");
            }
            if (NeedGold > 0)
            {
                mDL.DataIDList.Add("res.gold.v");
            }
            if (NeedHc > 0)
            {
                mDL.DataIDList.Add("res.hc.v");
            }
            int i;
            for (i = 0; i < id.Count; i++)
            {
                SetLabelShow(i, id[i], NeedNum[i]);
                NumLabelArray[i].gameObject.CustomSetActive(true);
            }
            for (; i < NumLabelArray.Length; i++)
            {
                NumLabelArray[i].gameObject.CustomSetActive(false);
            }
        }

        public void InitDataList(Dictionary<string, int> dic, int NeedGold = 0, int NeedHc = 0)
        {
            ClearData();
            foreach (var item in dic)
            {
                IDlist.Add(item.Key);
                Countlist.Add(item.Value);
            }
            SetPriData(IDlist, Countlist, NeedGold, NeedHc);
        }

        public void ClearData()
        {
            mDL.DataIDList.Clear();
            IDlist.Clear();
            Countlist.Clear();
        }
        private void SetLabelShow(int index, string id, int NeedCount = 0)
        {
            bool isRes = false;
            if(id.Equals(LTResID.HcName)||id.Equals(LTResID.GoldName))
            {
                isRes = true;
                itemCount = BalanceResourceUtil.GetResValue(id);
            }
            else
            {
                isRes = false;
                itemCount = GameItemUtil.GetInventoryItemNum(id);
            }                    
            if (NeedCount > 0)
            {
                NeedCountStr = string.Format("/{0}", ApplyNumFormat(NeedCount, isRes));
                colorstr = itemCount >= NeedCount ? "[42fe79]" : "[ff6699]";
            }
            else
            {
                NeedCountStr = "";
                colorstr = "[ffffff]";
            }
            if (NumLabelArray[index].transform.childCount > 0)
            {
                NumLabelArray[index].text = NumLabelArray[index].transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}{1}[-]{2}", colorstr, ApplyNumFormat(itemCount,isRes), NeedCountStr);
            }
            else
            {
                NumLabelArray[index].text = string.Format("{0}{1}[-]{2}", colorstr, ApplyNumFormat(itemCount, isRes), NeedCountStr);
            }
        }

        private string ApplyNumFormat(int data,bool isFormat)
        {
            if (!isFormat)
            {
                return data.ToString();
            }
            if (data >= 1000000000)
            {
                string str = string.Format("{0}G", data / 1000000000);
                return str;
            }
            else if (data >= 1000000)
            {
                string str = string.Format("{0}M", data / 1000000);
                return str;
            }
            else if (data >= 1000)
            {
                string str = string.Format("{0}K", data / 1000);
                return str;
            }
            else
            {
                return data.ToString();
            }
        }

        public override void OnLookupUpdate(string dataID, object value)
        {
            base.OnLookupUpdate(dataID, value);
            for (int i = 0; i < IDlist.Count; i++)
            {
                SetLabelShow(i, IDlist[i], Countlist[i]);
            }
        }
       
    }
}