using UnityEngine;
using System.Collections;
using EB.Sparx;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hotfix_LT.UI
{
    public class ChangeNicknameController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            InputLabel = t.GetComponent<UIInput>("Control/Table/InputBtn");
            InputLabel.onChange.Add(new EventDelegate(()=> { InputLimit(InputLabel); })) ;
            CostLabel = t.GetComponent<UILabel>("Control/BG/BG/ButtonGrid/OKButton/CostLabel");
            FirstRenameObj = t.FindEx("Control/BG/BG/ButtonGrid/OKButton/FreeLabel").gameObject;
            GeneralObj = new List<GameObject>();
            GeneralObj.Add(t.FindEx("Control/BG/Title/CloseButton").gameObject);
            GeneralObj.Add(t.FindEx("Control/BG/BG/ButtonGrid").gameObject);
            GuideObj = t.FindEx("Control/GuideObj").gameObject;
            TitleLabel = t.GetComponent<UILabel>("Control/BG/Title/Label");
            controller.backButton = t.GetComponent<UIButton>("Control/BG/Title/CloseButton");
            t.GetComponent<UIButtonText>("Control/BG/BG/ButtonGrid/CancelButton").onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<UIButtonText>("Control/GuideObj/GuideGrid/RandomButton").onClick.Add(new EventDelegate(OnRandomNameBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Control/BG/BG/ButtonGrid/OKButton").clickEvent.Add(new EventDelegate(OnSureBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Control/GuideObj/GuideGrid/OKButton").clickEvent.Add(new EventDelegate(OnSureBtnClick));
            UIServerRequestHotFix m_Request = t.GetMonoILRComponent<UIServerRequestHotFix>();
            m_Request.response = OnRequestResponse;
            t.GetComponent<UIServerRequest>().onResponse.Add(new EventDelegate(m_Request.mDMono, "OnFetchData"));
        }  
    	public UIInput  InputLabel;
        public UILabel CostLabel;
        public GameObject FirstRenameObj;
        private bool isHadRename;
        public List<GameObject> GeneralObj;
        public GameObject GuideObj;
        public UILabel TitleLabel;
    
        private bool isRandomName=false;
        private bool isRandomType = false;
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            isRandomName = false;
            isRandomType = false;
            if (GuideNodeManager.IsGuide)
            {
                GuideObj.CustomSetActive(true);
                for(int i=0;i<GeneralObj.Count; i++)
                {
                    GeneralObj[i].CustomSetActive(false);
                }
                RandomNameFactory.GuideRandom(delegate(string name) {
                    isRandomName = true;
                    isRandomType = true;
                    InputLabel.value = name;
                });
            }
            TitleLabel.text = (GuideNodeManager.IsGuide)? EB.Localizer.GetString("ID_INPUT_NAME") : EB.Localizer.GetString("ID_uifont_in_LTChangeNickNameView_Label_0");
        }
        
        public override IEnumerator OnAddToStack()
    	{        
            SetCostLabel();
            yield return base.OnAddToStack();
            if (GuideNodeManager.IsGuide)
            {
                OnRandomNameBtnClick();
            }
        }
        
        public override bool ShowUIBlocker
    	{
    		get	{ return true; }
    	}

        public override IEnumerator OnRemoveFromStack()
    	{
    		DestroySelf();
    		yield break;
    	}
    
        private void SetCostLabel()
        {
            DataLookupsCache.Instance.SearchDataByID<bool>("user.isHadRename", out isHadRename);
            string colorStr1 = (BalanceResourceUtil.GetUserDiamond() < (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("changeNameCost")) ? "[ff6699]" : "";
            CostLabel.text = CostLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}{1}", colorStr1, (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("changeNameCost"));
    
            CostLabel.gameObject.CustomSetActive(isHadRename);
            FirstRenameObj.CustomSetActive(!isHadRename);
        }
    
    	public void OnSureBtnClick()
    	{
    		if (string.IsNullOrEmpty(InputLabel.value))
    		{
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_INPUT_EMPTY")); //MenuManager.Warning("ID_INPUT_EMPTY");
                return;
    		}
    
    		if (InputLabel.value.IndexOf(" ") >= 0)
    		{
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_INPUT_CONTAINS_SPACE")); //MenuManager.Warning("ID_INPUT_CONTAINS_SPACE");
                return;
    		}
    
    		if (InputLabel.value.IndexOf("\n") >= 0)
    		{
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_INPUT_CONTAINS_NEWLINE")); //MenuManager.Warning("ID_INPUT_CONTAINS_NEWLINE");
                return;
    		}
    
    		if (InputLabel.value.Equals(LTGameSettingController.GetPlayerName()))
    		{
    			MessageTemplateManager.ShowMessage(901023);
    			return;
    		}
    
    		if(isHadRename && BalanceResourceUtil.GetUserDiamond()< (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("changeNameCost"))
            {
                BalanceResourceUtil.HcLessMessage();
                return;
    		}
    
    		if (!isRandomName&&(!EB.ProfanityFilter.Test(InputLabel.value) || !IsNormalName(InputLabel.value)))
    		{
    			MessageDialog.Show(EB.Localizer.GetString("ID_MESSAGE_TITLE_STR"),
    					EB.Localizer.GetString("ID_NAME_ILLEGEL"),
    					EB.Localizer.GetString("ID_MESSAGE_BUTTON_STR"), null, false, true, true, null, NGUIText.Alignment.Center);
    			return;
    		}
    
    		LoadingSpinner.Show();
    		var req = controller.transform.GetComponent<UIServerRequest>();
    		req.parameters[0].parameter = InputLabel.value;
    		req.SendRequest();
    	}
    
    	public void OnRequestResponse(EB.Sparx.Response res)
    	{
    		LoadingSpinner.Hide();
    		if (res.sucessful)
    		{
    			var user = EB.Dot.Object("user", res.hashtable, null);
    			if (user == null)
    			{
    				Debug.LogError("Missing user object on set name!!!");
    			}
    			else
    				LoginManager.Instance.LocalUser.Update(user);
    
    			MessageTemplateManager.ShowMessage(902045);
    			DataLookupsCache.Instance.CacheData("name", InputLabel.value);
                string name = null;
                if(DataLookupsCache.Instance.SearchDataByID<string>(string.Format("mainlands.pl.{0}.un", LoginManager.Instance.LocalUserId),out name)&&!string.IsNullOrEmpty ( name)) DataLookupsCache.Instance.CacheData(string.Format("mainlands.pl.{0}.un", LoginManager.Instance.LocalUserId), InputLabel.value);
    
                if(isHadRename) FusionTelemetry.PostBuy(((int)FusionTelemetry.UseHC.hc_playername).ToString(), 1, (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("changeNameCost"));
                controller.Close();
    		}
    		else 
    		{
    			res.CheckAndShowModal();
    		}
    	}
    
    	public void OnRandomNameBtnClick()
    	{
            isRandomName = true;
            isRandomType = true;
            InputLabel.value = GetRandomName();
    	}
    
    	private string GetRandomName()
    	{
    		string character_id;
    		if (!DataLookupsCache.Instance.SearchDataByID<string>("{buddyinventory.pos0.buddy.buddyid}.character_id", out character_id))
    		{
    			EB.Debug.LogError("SearchDataByID heroData fail");
    			return string.Empty;
    		}
    
            if (RandomNameFactory.Instance == null) return string.Empty;
    
    		if (character_id.Equals("10000400") || character_id.Equals("10000500"))
    		{
    			return RandomNameFactory.Instance.RandomName(true);
    		}
    		return RandomNameFactory.Instance.RandomName(false);
    	}
    
        private UITexture HightlightObjl;
        public void InputLimit(UIInput uiinput)
        {
            if (HightlightObjl == null&& uiinput.label.transform.Find("Input Highlight")!=null)
            {
                HightlightObjl = uiinput.label.transform .Find("Input Highlight").GetComponent<UITexture>();
            }
            if (HightlightObjl != null&&HightlightObjl.enabled) return;
            if (isRandomType)
            {
                isRandomType = false;
            }
            else
            {
                isRandomName = false;
            }
            while (uiinput.label.width >350|| uiinput.label.height>50)
            {
                string str = uiinput.value;
                uiinput.value = str.Substring(0, str.Length - 1);
                uiinput.label.text = uiinput.value;
            }
        }

        static public bool IsNormalName(string name)
        {
            Regex reg = new Regex("^[\u4e00-\u9fa5a-zA-Z0-9]+$");
            Match m = reg.Match(name);
            return m.Success;
        }
    }
}
