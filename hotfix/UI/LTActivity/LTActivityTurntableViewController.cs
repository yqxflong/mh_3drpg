using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTActivityTurntableViewController : UIControllerHotfix
    {
        public override bool ShowUIBlocker{ get { return true; } }

        private int activityId = 0;

        private UIButton Btn;
        private LTShowItem CostItem;
        private UILabel CostLabel;
        private List<LTShowItem> ItemList;
        private Transform TurnTran;

        private List<int> RotationList = new List<int> {
            115,
            70,
            20,
            -20,
            -70,
            -115,
            -160,
            160
        };
        private LTShowItemData CostItemData; 
        private List<int> costList;
        private List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stagesList;

        public override void Awake()
        {
            base.Awake();
            Btn = controller.transform.Find("Btn").GetComponent<UIButton>();
            Btn.onClick.Add(new EventDelegate(OnBtnClick));
            CostItem = Btn.transform.Find("CostItem").GetMonoILRComponent<LTShowItem>();
            CostLabel = Btn.transform.Find("Num").GetComponent<UILabel>();
            controller .backButton = controller.transform.Find("UINormalFrameBG/CancelBtn").GetComponent<UIButton>();
            ItemList = new List<LTShowItem>();
            Transform ItemRoot = controller.transform.Find("ItemList");
            for (int i=0;i<ItemRoot.childCount; ++i)
            {
                ItemList.Add(ItemRoot.GetChild(i).GetMonoILRComponent<LTShowItem>());
            }

            TurnTran = controller.transform.Find("Turn");
        }

        public override bool IsFullscreen()
        {
            return false;
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            activityId = (int)param;

            Hotfix_LT.Data.TimeLimitActivityTemplate activity = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivity(activityId);
       
            if (activity == null) return;
            if(!string.IsNullOrEmpty(activity.parameter2))
            {
                costList = new List<int>();
                string[] Splits = activity.parameter2.Split('|');
                string[] splits = Splits[0].Split(',');
                CostItemData = new LTShowItemData(splits[1], 0, splits[0]);

                splits = Splits[1].Split(',');
                for(int i = 0; i < splits.Length; ++i)
                {
                    costList.Add(int .Parse ( splits[i]));
                }
            }
            stagesList = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(activity.id);
            InitUI();
        }

        private void InitUI()
        {
            if (stagesList == null)
            {
                return;
            }
            Hashtable activityData;
            DataLookupsCache.Instance.SearchDataByID("tl_acs." + activityId, out activityData);
            int hasGetCount = 0;
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (i < stagesList.Count)
                {
                    ItemList[i].mDMono.gameObject.CustomSetActive(true);
                    ItemList[i].LTItemData = new LTShowItemData(stagesList[i].reward_items[0].id, stagesList[i].reward_items[0].quantity, stagesList[i].reward_items[0].type);

                    int hasGet = EB.Dot.Integer(string .Format ("stages.{0}",stagesList[i].id), activityData, 0);
                    ItemList[i].mDMono.transform.Find("Get").gameObject.CustomSetActive(hasGet>0);
                    if(hasGet>0) hasGetCount++;
                }
                else
                {
                    ItemList[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
            if (hasGetCount >= stagesList.Count)
            {
                Btn.gameObject.CustomSetActive(false);
            }
            else
            {
                Btn.gameObject.CustomSetActive(true);
                CostItem.LTItemData = CostItemData;
                CostLabel.text = costList[hasGetCount].ToString();
            }
        }

        private Vector3 temp;
        private IEnumerator ScrollTurn(int index, System.Action callback)
        {
            Vector3 cur = TurnTran.localRotation.eulerAngles;
            Vector3 target = new Vector3(0, 0, RotationList[index] + Random.Range(0, 10));
            target -= new Vector3(0, 0, 3600);
            
            float WholeRoad = cur.z - target.z;
            float curRoad = 0;
            float Ver = 0;
            while (curRoad < WholeRoad&& Ver>=0)
            {
                temp = Vector3.Lerp(cur, target, curRoad / WholeRoad);
                TurnTran.localRotation = Quaternion.Euler(temp);

                curRoad += Ver * Time.deltaTime;
                if (curRoad < 1500)
                {
                    Ver += Time.deltaTime * 300;
                }
                else if (curRoad > WholeRoad - 1480)
                {
                    Ver -= Time.deltaTime * 300;
                }
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
            
            int price = int.Parse ( CostLabel.text);

            int num = GameItemUtil.GetItemAlreadyHave(CostItem.LTItemData.id, CostItem.LTItemData.type); ;
            
            if (num < price)
            {
                if (CostItem.LTItemData.id =="hc")
                {
                    LTHotfixGeneralFunc.ShowChargeMess();
                }
                else if(CostItem.LTItemData.id == "gold")
                {
                    MessageTemplateManager.ShowMessage(901031, null, delegate (int result)
                    {
                        if (result == 0)
                        {
                            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                            GlobalMenuManager.Instance.Open("LTResourceShopUI");
                        }
                    });
                }
                else
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LegionLogic_33945"));
                }
                //BalanceResourceUtil.HcLessMessage();
                
                return;
            }
            
            //请求
            mIsScroll = true;

            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/specialactivity/gotReward");
            request.AddData("activityId", activityId);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                int id = 0;
                if (data != null)
                {
                    DataLookupsCache.Instance.CacheData(data);
                    Hashtable hash = EB.Dot.Object(string.Format("tl_acs.{0}.stages", activityId),data ,null);
                    foreach (string key in hash.Keys)
                    {
                        id = int.Parse(key);
                    }
                }
                OnReponse(data != null,id);
            });
        }

        public void OnReponse(bool hasData,int id=0)
        {
            if (hasData)
            {
                int index = GetItemIndex(id);
                if (index >= 0)
                {
                    EB.Coroutines.Run(ScrollTurn(index, delegate
                    {
                        InitUI();
                        List<LTShowItemData> list = new List<LTShowItemData>();
                        list.Add(ItemList[index].LTItemData);
                        GlobalMenuManager.Instance.Open("LTShowRewardView", list);
                        mIsScroll = false;
                    }));
                }
            }
            else
            {
                mIsScroll = false;
            }
        }

        private int GetItemIndex(int id)
        {
            int index = -1;
            if (stagesList != null)
            {
                for (int i = 0; i < stagesList.Count; i++)
                {
                    if (stagesList[i].id == id)
                    {
                        index = i;
                    }
                }
            }
            return index;
        }
        
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
