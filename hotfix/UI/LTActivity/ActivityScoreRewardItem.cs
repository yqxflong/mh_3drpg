using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{

    public class ActivityScoreRewardItemData
    {
        public int activityid { get; private set; }
        public int id { get; private set; }
        public int stage { get; private set; }
        public int state { get; private set; }//-1可领取，0不可领取，1已领取
        public List<LTShowItemData> reward { get; private set; }

        public ActivityScoreRewardItemData(int activityid, int id, int stage, int state, List<LTShowItemData> reward)
        {
            this.activityid = activityid;
            this.id = id;
            this.stage = stage;
            this.state = state;
            this.reward = reward;
        }

        public void RefreshState(int state)
        {
            this.state = state;
        }
    }
    public class ActivityScoreRewardItem : BaseItem<ActivityScoreRewardItemData>
    {
        private UILabel score, scoreshadow, btndesc, btndescshadow;
        private LTShowItem[] lTShowItems;
        private ConsecutiveClickCoolTrigger recieveBtn;
        private ActivityScoreRewardItemData itemData;
        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.GetComponent<Transform>();
            score = t.GetComponent<UILabel>("Name");
            scoreshadow = t.GetComponent<UILabel>("Name/Label");
            lTShowItems = new LTShowItem[3];
            lTShowItems[0] = t.GetMonoILRComponent<LTShowItem>("Awards/LTShowItem");
            lTShowItems[1] = t.GetMonoILRComponent<LTShowItem>("Awards/LTShowItem (1)");
            lTShowItems[2] = t.GetMonoILRComponent<LTShowItem>("Awards/LTShowItem (2)");
            recieveBtn = t.GetComponent<ConsecutiveClickCoolTrigger>("RecieveBtn");
            recieveBtn.clickEvent.Add(new EventDelegate(SendRecieveScoreReward));
            btndesc = t.GetComponent<UILabel>("RecieveBtn/Label");
            btndescshadow = t.GetComponent<UILabel>("RecieveBtn/Label/Label(Clone)");
        }
        public override void Clean()
        {
            Fill(null);
        }

        public override void Fill(ActivityScoreRewardItemData itemData)
        {
            if (itemData == null)
            {
                mDMono.gameObject.CustomSetActive(false);
                return;
            }
            this.itemData = itemData;
            score.text = scoreshadow.text = itemData.stage.ToString();
            int itemcount = itemData.reward.Count;
            for (int i = 0; i < lTShowItems.Length; i++)
            {
                if (i < itemcount)
                {
                    lTShowItems[i].LTItemData = itemData.reward[i];
                }
                else
                {
                    lTShowItems[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
            bool isgrey = itemData.state != -1;
            recieveBtn.GetComponent<UISprite>().color = isgrey ? Color.magenta : Color.white;
            recieveBtn.GetComponent<BoxCollider>().enabled = !isgrey;
            //LTUIUtil.SetGreyButtonEnable(recieveBtn, itemData.state == -1);
            btndesc.text = btndescshadow.text = itemData.state == 1 ? EB.Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL") : EB.Localizer.GetString("ID_BUTTON_LABEL_PULL");
        }

        private void SendRecieveScoreReward()
        {
            LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "insufficient num":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ACTIVITY_REALM_CHALLENGE_ERROR"));
                                LTMainHudManager.Instance.UpdateActivityLoginData(null);//刷新界面
                                        return true;
                            }
                        case "stage not reach":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ACTIVITY_REALM_CHALLENGE_ERROR"));
                                return true;
                            }
                    }
                }
                return false;
            };
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/specialactivity/gotReward");
            request.AddData("activityId", itemData.activityid);
            request.AddData("stageId", itemData.id);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                List<LTShowItemData> awardDatas = GameUtils.ParseAwardArr(Hotfix_LT.EBCore.Dot.Array("reward", data, null));
                GlobalMenuManager.Instance.Open("LTShowRewardView", awardDatas);
                Messenger.Raise(EventName.OnURScoreRewardRecieve);
            });
        }
    }

    public class ActivityScoreRewardDynamicScroll : DynamicGridScroll<ActivityScoreRewardItemData, ActivityScoreRewardItem>
    {
        public override void Awake()
        {
            base.Awake();
            thresholdFactor = 0.5f;
            padding = 28f;
            addition = 0;
            IsNoNeedForDelayFill = false;
        }
    }

}