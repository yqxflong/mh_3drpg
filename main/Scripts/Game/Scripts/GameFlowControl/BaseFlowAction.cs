using HutongGames.PlayMaker;

// public struct PlayState
// {
//     public const string PlayState_InitCombat = "init"; //初始

//     public const string PlayState_MainLand = "MainLand"; //主城
//     public const string PlayState_LCCampaign = "LCCampaign";//副本
//     public const string PlayState_Combat = "Combat";//战斗
// }

public class BaseFlowAction : FsmStateAction
{
    public override void OnEnter()
    {
        base.OnEnter();
        // Hotfix_LT.Messenger.Raise("BaseFlowActionOnEnter", State.Name);
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "BaseFlowActionOnEnter", State.Name);
    }

    public override void OnExit()
    {
        base.OnExit();
        // Hotfix_LT.Messenger.Raise("BaseFlowActionOnExit", State.Name);
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "BaseFlowActionOnExit", State.Name);
    }
}
