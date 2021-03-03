
namespace Hotfix_LT.UI
{
    public class TaskStateLookup : DataLookupHotfix
    {
        public override void OnLookupUpdate(string dataID, object value)
        {
            UpdateRedpoint();
        }

        private void UpdateRedpoint()
        {
            var tc = mDL.GetComponent<UIControllerILR>().ilinstance as UITaskSystemController;
            if (tc != null)
                tc.DrawAllRedPoint();
        }

    }
}