using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceSelectRewardCtrl : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            BtnBox = t.GetComponent<BoxCollider>("Btn");
            BtnSprite = t.GetComponent<UISprite>("Btn/Sprite");
            BtnLabel = t.GetComponent<UILabel>("Btn/Label");
            GetObj = t.FindEx("Get").gameObject;
            Service = t.GetComponent<UIServerRequest>();
            Item = t.GetMonoILRComponent<LTShowItem>("Item");

            t.GetComponent<UIButton>("Btn").onClick.Add(new EventDelegate(OnGetBtnClick));

            Service.onResponse.Add(new EventDelegate ( mDMono,"OnFetchData"));
            
            Hotfix_LT.Messenger.AddListener<int>(EventName.LTChallengeInstanceLevelSelect, OnLevelSelect);
        }
        public BoxCollider BtnBox;
        public UISprite BtnSprite;
        public UILabel BtnLabel;
    
        public GameObject GetObj;
    
        public UIServerRequest Service;
    
        public LTShowItem Item;
    
        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener<int>(EventName.LTChallengeInstanceLevelSelect, OnLevelSelect);
        }
    
        private int mLevel;
    
        private int mTaskId;
    
        public void OnLevelSelect(int lv)
        {
            mLevel = lv;
            int level = LTInstanceUtil.GetChallengeLevel(mLevel);
            mTaskId = 7000+level;
            InitState();
            InitItem();
        }
    
        private void InitState()
        {
            string state = string.Empty;
            DataLookupsCache.Instance.SearchDataByID(string.Format("tasks.{0}.state", mTaskId), out state);
    
            BtnSprite.enabled = false;
            if (state == "running")
            {
                GetObj.CustomSetActive(false);
                BtnSprite.spriteName = "Ty_Button_8";
                BtnSprite.color = Color.magenta;
                BtnBox.enabled = false;
                BtnLabel.text = EB.Localizer.GetString("ID_RECEIVE_AWARD");
            }
            else if (state == "finished")
            {
                GetObj.CustomSetActive(false);
                BtnSprite.spriteName = "Ty_Button_9";
                BtnSprite.color = Color.white;
                BtnBox.enabled = true;
                BtnLabel.text = EB.Localizer.GetString("ID_RECEIVE_AWARD");
            }
            else
            {
                GetObj.CustomSetActive(true);
                BtnSprite.spriteName = "Ty_Button_8";
                BtnSprite.color = Color.magenta;
                BtnBox.enabled = false;
                BtnLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL");
            }
            BtnSprite.enabled = true;
        }
    
        private void InitItem()
        {
            Hotfix_LT.Data.TaskTemplate tpl = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(mTaskId);
            List<LTShowItemData> list =  TaskStaticData.GetItemRewardList(mTaskId);
            if (list.Count > 0)
            {
                Item.LTItemData = list[0];
                Item.mDMono.gameObject.CustomSetActive(true);
                BtnBox.gameObject.CustomSetActive(true);
            }
        }
    
        public void OnGetBtnClick()
        {
            if (mTaskId > 0)
            {
                Service.parameters[0].parameter = mTaskId.ToString();
                Service.SendRequest();
            }
        }
    
        public override void OnFetchData(EB.Sparx.Response res, int reqInstanceID)
        {
            if (res.sucessful)
            {
                InitState();
                Hotfix_LT.Data.TaskTemplate tpl = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(mTaskId);
                List<LTShowItemData> list = TaskStaticData.GetItemRewardList(mTaskId);
                for (int i = 0; i < list.Count; i++)
                {
                    LTIconNameQuality icon_name_lvl = LTItemInfoTool.GetInfo(list[i].id, list[i].type);
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceHudController_20066"), icon_name_lvl.name, list[i].count));
                }
                Hotfix_LT.Messenger.Raise(EventName.LTChallengeInstaceRewardGet, mLevel);
            }
            else if (res.fatal)
            {
                SparxHub.Instance.FatalError(res.localizedError);
            }
        }
    }
}
