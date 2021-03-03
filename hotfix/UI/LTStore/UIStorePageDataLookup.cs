using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class UIStorePageDataLookup : DataLookupHotfix
    {
        private UIStorePageController m_UIStorePageController = null;
        public override void Awake()
        {
            base.Awake();

            m_UIStorePageController = mDL.gameObject.GetMonoILRComponent<UIStorePageController>();
        }

        public override void OnLookupUpdate(string dataID, object value)
        {
            if (dataID != null && value != null)
            {
                m_UIStorePageController.RefreshData();
            }
        }

    }
}

