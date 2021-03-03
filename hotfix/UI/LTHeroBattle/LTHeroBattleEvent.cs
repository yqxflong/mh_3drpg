namespace Hotfix_LT.UI
{
    public class LTHeroBattleEvent
    {
        /// <summary> 改变显示的英雄类型 水 火 风等 </summary>
        // public static System.Action<Hotfix_LT.Data.eRoleAttr> ChangeHeroType;

        /// <summary> 选择的英雄id</summary>
        public static System.Action<int> ChoiceHero;

        /// <summary> 确认选择的英雄</summary>
        public static System.Action ConfirmChoiceHero;

        /// <summary> 确认禁用的英雄</summary>
        public static System.Action ConfirmBanHero;

        /// <summary> 匹配其他玩家 </summary>
        // public static System.Action MatchOther;

        /// <summary> 取消匹配</summary>
        // public static System.Action QuitMatchOther;

        /// <summary> 关闭并取消匹配</summary>
        // public static System.Action CloseAndQuitMatch;

        /// <summary> 获取奖励</summary>
        // public static System.Action GetReward;

        public static System.Action<int> ChoiceSuitIndex;

        /// <summary> 触碰套装提示</summary>
        public static System.Action<int> TouchSuitIndexTips;

        /// <summary> 显示套装提示</summary>
        // public static System.Action<int, string> ShowSuitTips;

        /// <summary> 点击限免英雄按钮</summary>
        // public static System.Action<string> ClickLimitFree;

        /// <summary> 延迟跳转战斗场景</summary>
        // public static System.Action<long, object, System.Action<long, object>> DelayCombatTransition;

        /// <summary> 获取比赛匹配数据 </summary>
        // public static System.Func<HeroBattleMatchData> GetMatchData;

        /// <summary> 重连获取数据</summary>
        public static System.Action GetReloadData;

        /// <summary> 通知刷新英雄选择 收到服务器更新使用英雄信息后调用</summary>
        public static System.Action<HeroBattleChoiceData> NotifyRefreshChoiceState;

        /// <summary> 通知改变选择的英雄tlpid</summary>
        // public static System.Action<int> NotifyChangeChoiceHeroTplID;

        /// <summary> 通知改变选择的英雄 </summary>
        public static System.Action<HeroBattleChoiceCellData> NotifyChangeChoiceHero;

        /// <summary> 通知改变选择的属性系列</summary>
        // public static System.Action<Hotfix_LT.Data.eRoleAttr> NotifyChoiceHeroType;

        /// <summary> 通知匹配对手进入等待 </summary>
        // public static System.Action NotifyMatchOtherWaiting;

        /// <summary> 通知取消匹配对手</summary>
        // public static System.Action NotifyQuitMatchOther;

        /// <summary> 通知刷新匹配界面数据</summary>
        // public static System.Action<HeroBattleMatchData> NotifyRefreshMatchData;

        /// <summary> 通知英雄交锋完成</summary>
        public static System.Action NotifyHeroBattleHudFinish;

        /// <summary> 通知英雄交锋限制场景跳转</summary>
        public static System.Action NotifyHeroBattleDelayToScene;

    }
}