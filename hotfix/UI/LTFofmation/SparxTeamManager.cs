namespace Hotfix_LT.UI
{
    public class TeamManager : ManagerUnit
    {
        public override void Async(string message, object payload)
        {
            switch (message.ToLower())
            {
                case "sync":
                    LTFormationDataManager.Instance.SetFormationData();
                    break;
                default:
                    break;
            }
        }
    }
}
