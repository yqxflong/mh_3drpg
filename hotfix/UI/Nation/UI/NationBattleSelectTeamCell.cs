using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class NationBattleSelectTeamCell : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            GoOnButton = t.GetComponent<UIButton>("GoOnBtn");
            OffBtn = t.GetComponent<UISprite>("OffBtn");
            Mask = t.GetComponent<UISprite>("Mask");

            t.GetComponent<UIButton>("GoOnBtn").onClick.Add(new EventDelegate(() =>OnGoOnBtnClick(t.gameObject)));
        }


    
    	public UIButton GoOnButton;
        public UISprite OffBtn;
        public UISprite Mask;
    	public NationBattleTeam mTeamData;
    	private Coroutine ReviveCoroutine;
        public NationBattleSelectTeamController ctrl;
    
    
        public NationBattleTeam TeamData
        {
            get { return mTeamData; }
        }
      
    
    	public void Fill(NationBattleTeam teamData)
    	{
    		mTeamData = teamData;
    		if (teamData.RealState==eTeamState.Empty)
            {
                GoOnButton.GetComponent<UISprite>().color = Color.magenta;
                GoOnButton.isEnabled = false;
                GoOnButton.gameObject.CustomSetActive(false);
    		    UILabel uilabel = OffBtn.GetComponentInChildren<UILabel>();
    		    LTUIUtil.SetText(uilabel, EB.Localizer.GetString("ID_codefont_in_NationBattleSelectTeamCell_710"));
                OffBtn.gameObject.CustomSetActive(true);
    		    Mask.gameObject.CustomSetActive(false);
            }
    		else if (mTeamData.RealState == eTeamState.InTheWar)
            {
                GoOnButton.GetComponent<UISprite>().color = Color.magenta;
                GoOnButton.isEnabled = false;
                GoOnButton.gameObject.CustomSetActive(true);
    			UILabel uilabel = GoOnButton.GetComponentInChildren<UILabel>();
    			LTUIUtil.SetText(uilabel, EB.Localizer.GetString("ID_codefont_in_NationBattleSelectTeamCell_1019"));
    		    OffBtn.gameObject.CustomSetActive(false);
                Mask.gameObject.CustomSetActive(false);
    		}
    		else if (mTeamData.RealState == eTeamState.Available)
    		{
                GoOnButton.GetComponent<UISprite>().color = Color.white;
                GoOnButton.gameObject.CustomSetActive(true);
    		    GoOnButton.isEnabled = true;
                OffBtn.gameObject.CustomSetActive(false);
    		    Mask.gameObject.CustomSetActive(false);
            }
    		else if (mTeamData.RealState == eTeamState.Death || mTeamData.RealState == eTeamState.Arrive)
    		{
    		    UILabel uilabel = OffBtn.GetComponentInChildren<UILabel>();
    		    LTUIUtil.SetText(uilabel, EB.Localizer.GetString("ID_codefont_in_NationBattleSelectTeamCell_1567"));
                GoOnButton.GetComponent<UISprite>().color = Color.magenta;
                GoOnButton.gameObject.CustomSetActive(false);
                OffBtn.gameObject.CustomSetActive(true);
    		    Mask.gameObject.CustomSetActive(true);
            }
    	}
    
    	public void OnGoOnBtnClick(GameObject sender)
    	{
    		if (mTeamData.RealState != eTeamState.Available)
    		{
    			Fill(mTeamData);
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_NationBattleSelectTeamCell_1929"));
    			return;
    		}
    		int index = int.Parse(sender.gameObject.name);
    		string teamName = "nation" + (index+1);
    		NationManager.Instance.CurrentGoOnTeamName = teamName;
    		NationManager.Instance.StartAction(Path, teamName, ctrl.teamStartActionState, null);
            ctrl.controller.Close();
    		NationBattleHudController.Instance.DisableOperation(true);
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.RedPoint_Nation);
    	}
    
    	string Path {get{
                //return GetComponentInParent<NationBattleSelectTeamController>().Path
                return ctrl.Path;
            }
    	}
    }
}
