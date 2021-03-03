namespace Hotfix_LT.UI
{
    public class RefreshFromDataLookup : DataLookupHotfix
    {
        private System.Action _callback;

        public void SetCallback(System.Action callback)
        {
            _callback = null;
            _callback = callback;
        }

        public override void OnLookupUpdate(string dataID, object value)
        {
            if (dataID != null && value != null)
            {
                _callback?.Invoke();
            }
        }
    }
}

