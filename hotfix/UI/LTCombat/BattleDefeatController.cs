using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class BattleDefeatController : DynamicMonoHotfix
    {
        public GameObject DefeatFX;
        public GameObject SkipPanel;
        public GameObject PartnersDevelopRecommendMark;
        public GameObject PartnersEquipRecommendMark;
        public GameObject PartnersIllustratedHandbookRecommendMark;

        public static bool sDefeatSkip;
        public static string sDefeatSkipPanel;
        public static string sDefeatSkipParam;

        private BattleResultScreenController _battleResultScreenController;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            DefeatFX = t.parent.FindEx("DefeatObj").gameObject;
            SkipPanel = t.FindEx("Grid").gameObject;
            PartnersDevelopRecommendMark = t.FindEx("Grid/Item/Recommend").gameObject;
            PartnersEquipRecommendMark = t.FindEx("Grid/Item (1)/Recommend").gameObject;
            PartnersIllustratedHandbookRecommendMark = t.FindEx("Grid/Item (2)/Recommend").gameObject;
            sDefeatSkip = false;

            t.GetComponent<UIButton>("Grid/Item/Frame").onClick.Add(new EventDelegate(OnGotoPartnersDevelop));
            t.GetComponent<UIButton>("Grid/Item (1)/Frame").onClick.Add(new EventDelegate(OnGotoPartnersEquip));
            t.GetComponent<UIButton>("Grid/Item (2)/Frame").onClick.Add(new EventDelegate(OnGotoPartnersIllustratedHandbook));

            _battleResultScreenController = t.parent.GetUIControllerILRComponent<BattleResultScreenController>();
        }

    	public override void OnEnable()
        {
    		sDefeatSkip = false;
    		SkipPanel.gameObject.CustomSetActive(false);
    		DefeatFX.CustomSetActive(true);
    		if(SceneLogic.BattleType==eBattleType.MainCampaignBattle || SceneLogic.BattleType == eBattleType.SleepTower || SceneLogic.BattleType == eBattleType.InfiniteChallenge)
    			StartCoroutine(UpdateUI());
        }
    
    	public override void OnDisable()
    	{
    		DefeatFX.CustomSetActive(false);
    	}
    
    	IEnumerator UpdateUI()
    	{
            yield return new  WaitForSeconds(0.4f) ;
    		SkipPanel.gameObject.CustomSetActive(true);
    		bool isPartnersDevelop = LTPartnerDataManager.Instance.IsCanPartnerUpAttr();
    		PartnersDevelopRecommendMark.gameObject.CustomSetActive(isPartnersDevelop);
    		PartnersEquipRecommendMark.gameObject.CustomSetActive(LTPartnerDataManager.Instance.IsHasCanEquipUpLvPartnerInGoIntoBattle());
    		PartnersIllustratedHandbookRecommendMark.gameObject.CustomSetActive(LTPartnerHandbookManager.Instance.HasHandBookRedPoint());
    	}
    
    	public void OnGotoPartnersDevelop()
    	{
    		Hotfix_LT.Data.FuncTemplate func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10050);
    		if (!func.IsConditionOK())
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,func.GetConditionStr());
    			return;
    		}
    		sDefeatSkip = true;
    		GlobalMenuManager.Instance.ClearCache();
    		sDefeatSkipPanel = "LTPartnerView";
    		sDefeatSkipParam = "Develop";
            _battleResultScreenController.OnContinueClick();
    	}
    
    	public void OnGotoPartnersEquip()
    	{
    		Hotfix_LT.Data.FuncTemplate func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10050);
    		if (!func.IsConditionOK())
    		{
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, func.GetConditionStr());
                return;
    		}
    		sDefeatSkip = true;
    		GlobalMenuManager.Instance.ClearCache();
    		sDefeatSkipPanel = "LTPartnerView";
    		sDefeatSkipParam = "OpenEquip";
            _battleResultScreenController.OnContinueClick();
    	}
    
    	public void OnGotoPartnersIllustratedHandbook()
    	{
    		Hotfix_LT.Data.FuncTemplate func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10049);
    		if (!func.IsConditionOK())
    		{
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, func.GetConditionStr());
                return;
    		}
    		sDefeatSkip = true;
    		GlobalMenuManager.Instance.ClearCache();
    		sDefeatSkipPanel = "PartnerHandbookHudView";
    		sDefeatSkipParam = null;
            _battleResultScreenController.OnContinueClick();
    	}
    }
}
