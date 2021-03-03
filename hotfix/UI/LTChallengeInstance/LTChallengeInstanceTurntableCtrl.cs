using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceTurntableCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;

            RotationList = new List<Vector3>();
            RotationList.Add(new Vector3(0, 0, -90));
            RotationList.Add(new Vector3(0, 0, -45));
            RotationList.Add(new Vector3(0, 0, 0));
            RotationList.Add(new Vector3(0, 0, 45));
            RotationList.Add(new Vector3(0, 0, 90));
            RotationList.Add(new Vector3(0, 0, 135));
            RotationList.Add(new Vector3(0, 0, 180));
            RotationList.Add(new Vector3(0, 0, -135));

            ItemList = new List<LTShowItem>();
            ItemList.Add(t.GetMonoILRComponent<LTShowItem>("ItemList/Item/GameItem"));
            ItemList.Add(t.GetMonoILRComponent<LTShowItem>("ItemList/Item (1)/GameItem"));
            ItemList.Add(t.GetMonoILRComponent<LTShowItem>("ItemList/Item (2)/GameItem"));
            ItemList.Add(t.GetMonoILRComponent<LTShowItem>("ItemList/Item (3)/GameItem"));
            ItemList.Add(t.GetMonoILRComponent<LTShowItem>("ItemList/Item (4)/GameItem"));
            ItemList.Add(t.GetMonoILRComponent<LTShowItem>("ItemList/Item (5)/GameItem"));
            ItemList.Add(t.GetMonoILRComponent<LTShowItem>("ItemList/Item (6)/GameItem"));
            ItemList.Add(t.GetMonoILRComponent<LTShowItem>("ItemList/Item (7)/GameItem"));
            
            HasGetList = new List<GameObject>();
            HasGetList.Add(t.FindEx("ItemList/Item/Get").gameObject);
            HasGetList.Add(t.FindEx("ItemList/Item (1)/Get").gameObject);
            HasGetList.Add(t.FindEx("ItemList/Item (2)/Get").gameObject);
            HasGetList.Add(t.FindEx("ItemList/Item (3)/Get").gameObject);
            HasGetList.Add(t.FindEx("ItemList/Item (4)/Get").gameObject);
            HasGetList.Add(t.FindEx("ItemList/Item (5)/Get").gameObject);
            HasGetList.Add(t.FindEx("ItemList/Item (6)/Get").gameObject);
            HasGetList.Add(t.FindEx("ItemList/Item (7)/Get").gameObject);

            TurnTran = t.GetComponent<Transform>("Turn");
            CostLabel = t.GetComponent<UILabel>("Btn/Num");
            Index = 7;

            t.GetComponent<UIButton>("Btn").onClick.Add(new EventDelegate(OnBtnClick));
        }
        
        public List<LTShowItem> ItemList;
        public List<GameObject> HasGetList;
    
        public List<Vector3> RotationList;
    
        public Transform TurnTran;
    
        public UILabel CostLabel;
    
        public override bool ShowUIBlocker
        {
            get
            {
                return true;
            }
        }
    
        public override bool IsFullscreen()
        {
            return false;
        }
    
        private int x;
    
        private int y;
    
        private System.Action callback;
        private string requestType;
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            Hashtable data = param as Hashtable;
            if (data != null)
            {
                requestType = EB.Dot.String("type", data, "");
                switch (requestType)
                {
                    case LTInstanceConfig.InChallengeState:
                        x = EB.Dot.Integer("x", data, 0);
                        y = EB.Dot.Integer("y", data, 0);
                        callback = data["callback"] as System.Action;
                        var nodeData = LTInstanceMapModel.Instance.GetNodeByPos(x, y);
                        if (nodeData != null)
                        {
                            mItemDataList = nodeData.WheelData;
                            mCout = nodeData.WheelCount;
                            InitUI();
                        }
                        break;
                    case LTInstanceConfig.OutChallengeState:
                        mItemDataList = data["list"] as List<LTShowItemData>;
                        mCout = (int)data["count"];
                        InitUI();
                        break;
                    default:
                        break;
                }

            }
        }
    
        private List<LTShowItemData> mItemDataList = new List<LTShowItemData>();
    
        private int mCout = 0;
    
        public override IEnumerator OnAddToStack()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.UI_SERVER_REQUEST, 0.8f);
            yield return base.OnAddToStack();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            if (callback != null)
            {
                callback();
            }
            yield return base.OnRemoveFromStack();
        }
    
        private void InitUI()
        {
            if (mItemDataList == null)
            {
                EB.Debug.LogError("为什么输入的数据为空了呢?mItemDataList = null");
                return;
            }
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (i < mItemDataList.Count)
                {
                    HasGetList[i].CustomSetActive(mItemDataList[i].count <= 0);
                    ItemList[i].mDMono.gameObject.CustomSetActive(true);
                    ItemList[i].LTItemData = mItemDataList[i];
                }
                else
                {
                    HasGetList[i].CustomSetActive(false);
                    ItemList[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
    
            CostLabel.transform.parent.gameObject.CustomSetActive(mCout < mItemDataList.Count);
    
            float init = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("LuckWheelPriceInit");
            float step = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("LuckWheelPriceStep");
            float max = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("LuckWheelPriceMax");
            float price = Mathf.Min(init + step * mCout, max);
            CostLabel.text = price.ToString();
        }
    
        private float TimeLimit = 5;
    
        private Vector3 Speed = new Vector3(0, 0, -30);
    
        private IEnumerator ScrollTurn(int index, System.Action callback)
        {
            Vector3 target = RotationList[index];
    
            float time = 0;
    
            while (time < TimeLimit)
            {
                time += Time.deltaTime;
                TurnTran.Rotate(Speed * time / TimeLimit);
                yield return null;
            }
    
            time = 0;
            Vector3 cur = TurnTran.localRotation.eulerAngles;
            if (cur.z <= target.z)
            {
                target -= new Vector3(0, 0, 360);
            }
            float needTime = (cur.z - target.z) / 360f;
            while (time < needTime)
            {
                time += Time.deltaTime;
                Vector3 temp = Vector3.Lerp(cur, target, time / needTime);
                TurnTran.localRotation = Quaternion.Euler(temp);
                yield return null;
            }
    
            yield return new WaitForSeconds(0.2f);
    
            callback();
        }
    
        private bool mIsScroll = false;
    
        public void OnBtnClick()
        {
            if (mIsScroll)
            {
                return;
            }
    
            float init = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("LuckWheelPriceInit");
            float step = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("LuckWheelPriceStep");
            float max = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("LuckWheelPriceMax");
            float price = Mathf.Min(init + step * mCout, max);
    
            int num = 0;
            DataLookupsCache.Instance.SearchIntByID("res.hc.v", out num);
            if (num < price)
            {
                BalanceResourceUtil.HcLessMessage();
                return;
            }
            switch (requestType)
            {
                case LTInstanceConfig.InChallengeState:
                    LTInstanceMapModel.Instance.RequestChallengeCampaignLuckDraw(OnLuckDrawCallback);
                    break;
                case LTInstanceConfig.OutChallengeState:
                    LTInstanceMapModel.Instance.RequestChallengeWipeOutLuckDraw(OnLuckDrawCallback);
                    break;
                default:
                    break;
            }
            mIsScroll = true;
        }
        private ArrayList wheellist;
        private List<LTShowItemData> Newdatalist;
        private List<LTShowItemData> Showdatalist;
        private void OnLuckDrawCallback(Hashtable result)
        {
            string id = string.Empty;
            int index = -1;
            if (requestType.Equals(LTInstanceConfig.InChallengeState))
            {
                id = EB.Dot.String("data", result, string.Empty);
                index = GetItemIndex(id);
            }
            else if (requestType.Equals(LTInstanceConfig.OutChallengeState))
            {
                wheellist = EB.Dot.Array("wheelData.wheelData", result, null);
                if (wheellist == null)
                {
                    mIsScroll = false;
                    return;
                }
                if (Newdatalist == null)
                {
                    Newdatalist = new List<LTShowItemData>();
                }
                else
                {
                    Newdatalist.Clear();
                }

                for (int i = 0; i < wheellist.Count; i++)
                {
                    if (wheellist[i] != null)
                    {
                        Newdatalist.Add(new LTShowItemData(wheellist[i]));
                    }
                    if(Newdatalist[i].count == 0 && mItemDataList[i].count == 1)
                    {
                            index = i;
                    }
                }
            }                       
            if (index >= 0)
            {
                StartCoroutine(ScrollTurn(index, delegate
                {
                    Hashtable hashdata = Johny.HashtablePool.Claim();
                    if (Showdatalist == null)
                    {
                        Showdatalist = new List<LTShowItemData>();
                    }
                    else
                    {
                        Showdatalist.Clear();
                    }
                    Showdatalist.Add(mItemDataList[index]);
                    hashdata.Add("reward", Showdatalist);
                    hashdata.Add("callback", new System.Action(delegate {
                        mItemDataList[index].count = 0;
                        mCout++;
                        InitUI();
                    }));
                    GlobalMenuManager.Instance.Open("LTShowRewardView", hashdata);
                    mIsScroll = false;
                }));
            }
            else
            {
                mIsScroll = false;
            }
        }
        
        private int GetItemIndex(string id)
        {
            int index = -1;
            if (mItemDataList != null)
            {
                for (int i = 0; i < mItemDataList.Count; i++)
                {
                    if (mItemDataList[i].id == id && mItemDataList[i].count > 0)
                    {
                        index = i;
                    }
                }
            }
            return index;
        }
    
        public int Index = 7;
    
        public override void OnCancelButtonClick()
        {
            if (mIsScroll)
            {
                return;
            }
            base.OnCancelButtonClick();
        }
    }
}
