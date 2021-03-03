using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LevelCompensatedRewardController : UIControllerHotfix
    {

        private LTShowItem [] ItemArray;
        private List<LTShowItemData> ShowDataList;
        private int id;
        public override void SetMenuData(object param)
        {
            if (param == null)
            {
                return;
            }
            id = (int)param;
            LevelCompensatedRewardTemplate tempreward = EventTemplateManager.Instance.GetFreeLevelCompensatedReward(id);
            if (tempreward != null)
            {
                ShowDataList = tempreward.rewardList;
                Fill(tempreward.rewardList);
            }
            else
            {
                controller.Close();
            }
        }
        public override void Awake()
        {
            Transform t = controller.transform;
            ItemArray = new LTShowItem[4];
            ItemArray[0] = t.Find("Content/GiftGrid/LTShowItem").GetMonoILRComponent<LTShowItem>();
            ItemArray[1] = t.Find("Content/GiftGrid/LTShowItem (1)").GetMonoILRComponent<LTShowItem>();
            ItemArray[2] = t.Find("Content/GiftGrid/LTShowItem (2)").GetMonoILRComponent<LTShowItem>();
            ItemArray[3] = t.Find("Content/GiftGrid/LTShowItem (3)").GetMonoILRComponent<LTShowItem>();
            t.Find("Content/PriceBtn").GetComponent<ConsecutiveClickCoolTrigger>().clickEvent.Add(new EventDelegate(OnClickReceiveBtn));
        }

        private void Fill(List<LTShowItemData> reward)
        {
            for (int i = 0; i < ItemArray.Length; i++)
            {
                if (i < reward.Count)
                {
                    ItemArray[i].LTItemData = reward[i];
                    ItemArray[i].mDMono.gameObject.CustomSetActive(true);
                }
                else
                {
                    ItemArray[i].mDMono.gameObject.CustomSetActive(false);
                }

            }

        }
        private void OnClickReceiveBtn()
        {
            LTPartnerDataManager.Instance.ReceiveLevelCompensatedReward(id, delegate (bool isSucceed)
             {
                 if (isSucceed)
                 {
                     controller.Close();
                     GlobalMenuManager.Instance.Open("LTShowRewardView", ShowDataList);
                 }
             });
        }
    }
}