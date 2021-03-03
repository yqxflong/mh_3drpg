using Hotfix_LT.Data;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GM.DataCache;
using System;

namespace Hotfix_LT.UI
{
    public class LTClimingTowerRewardCell : DynamicCellController<Hotfix_LT.Data.ClimingTowerRewardTemplate>
    {
        public UILabel RankLabel;
        public List<LTShowItem> Items;
        public UISprite v_Btn;
        public UILabel BtnLabel;
        private ClimingTowerRewardTemplate m_Data;
        private int cur = 0;
        private int state = 0;
        private List<LTShowItemData> reward;

        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            RankLabel = t.GetComponent<UILabel>("Name");
            v_Btn = t.GetComponent<UISprite>("Sprite");
            BtnLabel = t.GetComponent<UILabel>("Sprite/Label");
            var coolbtn= v_Btn.transform.GetComponent<ConsecutiveClickCoolTrigger>();
            coolbtn.clickEvent.Add(new EventDelegate(OnClickBtn));

            Items = new List<LTShowItem>();
            Items.Add(t.GetMonoILRComponent<LTShowItem>("Awards/LTShowItem"));
            Items.Add(t.GetMonoILRComponent<LTShowItem>("Awards/LTShowItem (1)"));
            Items.Add(t.GetMonoILRComponent<LTShowItem>("Awards/LTShowItem (2)"));
        }

        public override void Clean()
        {

        }

        private void OnClickBtn()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            switch (state)
            {
                case 0://不能领取
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_SLEEPTOWER_TIP_3"));
                    break;
                case 1://可以领取
                    LTClimingTowerRewardControl controller = mDMono.transform.parent.parent.parent.parent.parent.GetUIControllerILRComponent<LTClimingTowerRewardControl>();
                    LTClimingTowerHudController.Instance.v_Api.errorProcessFun = (EB.Sparx.Response response) =>
                    {
                        if (response.error != null)
                        {
                            string strObjects = (string)response.error;
                            string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                            switch (strObject[0])
                            {
                                case "no redeems":
                                    {
                                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_SLEEPTOWER_TIP_4"));
                                        //刷新当前界面
                                        LTClimingTowerHudController.Instance.UpdatePanel(true);
                                        controller.OnCancelButtonClick();
                                        return true;
                                    }
                            }
                        }
                        return false;
                    };

                    LTClimingTowerHudController.Instance.v_Api.RequestRecordReward(m_Data.id, (hashtable) =>
                    {
                        if (hashtable != null)
                        {
                            state = 2;
                            DataLookupsCache.Instance.CacheData(hashtable);
                            DataLookupsCache.Instance.CacheData("userSleepTower.rewards_status", hashtable["rewards_status"]);
                            controller.F_SetData();
                            //刷新当前界面
                            LTClimingTowerHudController.Instance.UpdatePanel(false);
                            //
                            if (reward != null && reward.Count != 0)
                            {
                                GlobalMenuManager.Instance.Open("LTShowRewardView", reward);
                            }
                        }
                    });
                    break;
                case 2://已经领取
                    break;
            }
        }

        public override void Fill(Hotfix_LT.Data.ClimingTowerRewardTemplate itemData)
        {
            cur = LTClimingTowerManager.Instance.GetCurrentRecord();
            m_Data = itemData;
            state = cur < m_Data.Record ? 0 : isGetReward() ? 2 : 1;
            switch (state)
            {
                case 0://不能领取
                    v_Btn.color = Color.magenta;
                    LTUIUtil.SetText(BtnLabel, EB.Localizer.GetString("ID_BUTTON_LABEL_PULL"));
                    break;
                case 1://可以领取
                    v_Btn.color = Color.white;
                    LTUIUtil.SetText(BtnLabel, EB.Localizer.GetString("ID_BUTTON_LABEL_PULL"));
                    break;
                case 2://已经领取
                    v_Btn.color = Color.magenta;
                    LTUIUtil.SetText(BtnLabel, EB.Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL"));
                    break;
            }
            RankLabel.text = itemData.Record.ToString();
            Items.ForEach(item => item.mDMono.gameObject.SetActive(false));
            reward = LTUIUtil.GetLTShowItemDataFromStr(m_Data.Reward, false);
            if (itemData != null)
            {
                for (int i = 0; i < reward.Count; i++)
                {
                    LTShowItemData temp = reward[i];

                    Items[i].LTItemData = new LTShowItemData(temp.id, temp.count, temp.type);
                    Items[i].mDMono.gameObject.SetActive(true);
                }
            }
        }

        public bool isGetReward()
        {
            int isGet;
            DataLookupsCache.Instance.SearchDataByID($"userSleepTower.rewards_status.{m_Data.id}", out isGet);
            return isGet == 1;
        }

    }

}