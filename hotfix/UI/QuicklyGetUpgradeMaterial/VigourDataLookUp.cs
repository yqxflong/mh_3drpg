namespace Hotfix_LT.UI
{
    public class VigourDataLookUp : DataLookupHotfix
    {
        private QuicklyGetUpgradeMaterialController _controller;
        private QuicklyGetUpgradeMaterialController controller
        {
            get
            {
                if (_controller==null)
                {
                    _controller = mDL.transform.GetUIControllerILRComponent<QuicklyGetUpgradeMaterialController>();
                }

                return _controller;
            }
        }
        
        public override void Awake()
        {
            base.Awake();
            if (!mDL.DataIDList.Contains("res.vigor.v"))
            {
                mDL.DataIDList.Add("res.vigor.v");
            }
        }
        public override void OnLookupUpdate(string dataID, object value)
        {
            base.OnLookupUpdate(dataID, value);

            controller.FillMaterial();
        }
    }
}