namespace Hotfix_LT.UI
{
    public class UIConditionTabAdapt : DynamicMonoHotfix {
        public virtual bool IsConditionOk()
        {
            return true;
        }

        public virtual bool ShowConditionMessage()
        {
            return true;
        }
    }
}