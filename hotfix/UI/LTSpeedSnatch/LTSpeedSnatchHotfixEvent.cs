using System;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 只在Hotfix_LT域内使用 如果要用跨域事件在Unity项目内创建
    /// </summary>
    public class LTSpeedSnatchHotfixEvent
    {
        public static Action<int[]> SpeedSnatchBase;
        public static Action RefreshModel;
        public static Action<bool> SpeedSnatchActive;
    }
}
