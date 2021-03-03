using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hotfix_LT.Data;


namespace Hotfix_LT.UI
{
    public class LegionDonateChest : DynamicMonoHotfix
    {
        private UILabel chestScore;
        private GameObject gotMask;
        private AllianceDonateChest chestData;
        private Collider chestCollider;
        private GameObject fx;
        private bool couldRecieve, hasRecieve;
        private int aid;


        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            chestScore = t.GetComponent<UILabel>();
            gotMask = t.GetComponent<Transform>("Sprite").gameObject;
            chestCollider = t.GetComponent<Collider>("sp");
            fx = t.GetComponent<Transform>("fx").gameObject;
            t.GetComponent<ConsecutiveClickCoolTrigger>("sp").clickEvent.Add(new EventDelegate(OnClickChest));
        }

        public override void OnEnable()
        {
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnLegionDonateChestInfoChange, SetRecievedState);
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnLegionDonateTimesChaged, UpdataChestState);
        }

        public override void OnDisable()
        {
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnLegionDonateChestInfoChange, SetRecievedState);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnLegionDonateTimesChaged, UpdataChestState);
        }
        public void Fill(AllianceDonateChest data)
        {
            if (data == null)
            {
                EB.Debug.LogError("LegionDonateChest data is null,please check");
                return;
            }
            chestScore.text = data.score.ToString();
            chestData = data;            
            aid = LegionModel.GetInstance().legionData.legionID;
            SetRecievedState();
        }

        public void UpdataChestState()
        {
            if (!hasRecieve && chestData !=null)
            {
                couldRecieve = AlliancesManager.Instance.Detail.TodayTotalExp >= chestData.score;
                fx.CustomSetActive(couldRecieve);
            }
            else
            {
                fx.CustomSetActive(false);
            }
        }

        private void SetRecievedState()
        {
            if (chestData != null)
            {
                var recieveinfo = AlliancesManager.Instance.CurDonateInfo.recievedChestDic;
                recieveinfo.TryGetValue(chestData.id, out int times);
                hasRecieve = times == 1;
                if (hasRecieve)
                {                   
                    gotMask.CustomSetActive(true);
                    chestCollider.enabled = false;
                }
                else
                {                  
                    gotMask.CustomSetActive(false);
                    chestCollider.enabled = true;                  
                }
                UpdataChestState();
            }
        }

        private void OnClickChest()
        {
            if (couldRecieve)
            {
                LegionLogic.GetInstance().SendRecieveDonateChest(aid, chestData.id, delegate
                {
                    Hashtable data = Johny.HashtablePool.Claim();
                    data.Add("reward", chestData.Rewards);                  
                    GlobalMenuManager.Instance.Open("LTShowRewardView", data);
                });
                LegionLogic.GetInstance().OnSendGetCurDonateInfo(FetchDataHandler);
            }
            else
            {
                Hashtable data = Johny.HashtablePool.Claim();
                data.Add("data", chestData.Rewards);
                data.Add("tip", string.Format(EB.Localizer.GetString("ID_LEGION_DONATECHEST_REWARD"), chestData.score));
                GlobalMenuManager.Instance.Open("LTRewardShowUI", data);
            }

        }
        private void FetchDataHandler(Hashtable alliance)
        {
            if (alliance != null)
            {
                GameDataSparxManager.Instance.ProcessIncomingData(alliance, false);
            }
        }
    }
}
