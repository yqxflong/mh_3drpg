using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class NationBattleRewardController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            DynamicScroll = t.GetMonoILRComponent<NationBattleRewardDynamicScroll>("Content/ScrollView/Placehodler/Grid");
            controller.backButton = t.GetComponent<UIButton>("Frame/LTPopFrame/CloseButton");


        }


    
    	public override bool ShowUIBlocker { get { return true; } }
    
    	public NationBattleRewardDynamicScroll DynamicScroll;
    
    	public override IEnumerator OnAddToStack()
    	{
    		yield return base.OnAddToStack();
    
    		DynamicScroll.SetItemDatas(Hotfix_LT.Data.EventTemplateManager.Instance.GetNationRatingRewardTpls().ToArray());
    	}
    
    	public override IEnumerator OnRemoveFromStack()
    	{
    		DestroySelf();
    		yield break;
    	}
    }
}
