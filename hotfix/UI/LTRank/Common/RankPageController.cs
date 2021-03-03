using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{

    /// <summary>
    /// 排名页面的控件
    /// </summary>
    public class RankPageController : DynamicMonoHotfix
    {
        /// <summary>
        /// 请求数据的路径
        /// </summary>
        public string m_PageDataPath;

        protected UIServerRequest m_UIServerRequest = null;
        // Use this for initialization
        public override void Start()
        {
            m_UIServerRequest = mDMono.GetComponent<UIServerRequest>();
            if (m_UIServerRequest != null)
            {
                m_UIServerRequest.onResponse.Add(new EventDelegate(mDMono, "OnFetchData"));
            }
            else
            {
                EB.Debug.LogError("Didnot have UIServerRequest In this object{0}", mDMono.name);
            }

            if (NeedRefreshData())
            {
                FetchDataRemote();
            }
            else
            {
                FetchDataLocalAndShow();
            }
        }

        protected virtual bool NeedRefreshData()
        {
            return true;
        }

        protected void FetchDataRemote()
        {
            if (m_UIServerRequest != null)
            {
                LoadingSpinner.Show();
                m_UIServerRequest.SendRequest();
            }
        }

        protected void FetchDataLocalAndShow()
        {
            ArrayList array = null;
            if (DataLookupsCache.Instance.SearchDataByID<ArrayList>(m_PageDataPath, out array) && array != null)
            {
                UpdateUI(array);
            }
        }

        public override void OnFetchData(EB.Sparx.Response result, int reqInstanceID)
        {
            LoadingSpinner.Hide();
            if (result.sucessful && result.hashtable != null)
            {

                ArrayList array = Hotfix_LT.EBCore.Dot.Array(m_PageDataPath, result.hashtable, null);
                if (array != null) UpdateUI(array);
            }
            else if (result.fatal)
            {
                SparxHub.Instance.FatalError(result.localizedError);
            }
            else
            {
                EB.Debug.LogError("Fetch Data Error");
            }
        }

        protected virtual void UpdateUI(ArrayList array)
        {
        }
    }

}