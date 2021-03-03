using EB.Sparx;

namespace Hotfix_LT.UI
{
    public class MercenaryManager : ManagerUnit
    {
        private static MercenaryManager sInstance = null;
        public static MercenaryManager Instance
        {
            get { return sInstance = sInstance ?? LTHotfixManager.GetManager<MercenaryManager>(); }
        }
        
        public override void Async(string message, object payload)
        {
            switch (message)
            {
                case "update":
                    DataLookupsCache.Instance.CacheData(payload);
                    break;
            }
        }
    }
}