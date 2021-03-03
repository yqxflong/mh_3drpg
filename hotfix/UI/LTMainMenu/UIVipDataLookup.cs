using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class UIVipDataLookup : DataLookupHotfix
    {
        public GameObject SilverObj;
        public GameObject GoldObj;
        public GameObject SilverAndGoldObj;

        public override void Awake()
        {
            base.Awake();

            var t = mDL.transform;
            SilverObj = t.FindEx("Silver").gameObject;
            GoldObj = t.FindEx("Gold").gameObject;
            SilverAndGoldObj = t.FindEx("SilverAndGold").gameObject;
        }

        public override void OnLookupUpdate(string dataID, object value)
        {
            base.OnLookupUpdate(dataID, value);
    
            Refresh();
        }
    
        private void Refresh()
        {
            bool isSilverVip = LTChargeManager.Instance.IsSilverVIP();
            bool isGoldVip = LTChargeManager.Instance.IsGoldVIP();
    
            SilverObj.CustomSetActive(isSilverVip && !isGoldVip);
            GoldObj.CustomSetActive(!isSilverVip && isGoldVip);
            SilverAndGoldObj.CustomSetActive(isSilverVip && isGoldVip);
        }
    }

	public class UIVipLevelDataLookup : DataLookupHotfix
	{
		public UILabel Level;
		public UIWidget Widget;
		public GameObject SilverAndGoldObj;

		public override void Awake()
		{
			base.Awake();

			Transform t = mDL.transform;
			Level = t.GetComponent<UILabel>("Icon/Level");
			Widget = t.GetComponent<UIWidget>();

			DataLookupsCache.Instance.SearchDataByID<bool>("isOpenEC", out bool isOpen);
			t.gameObject.SetActive(isOpen);
		}

		public override void OnLookupUpdate(string dataID, object value)
		{
			base.OnLookupUpdate(dataID, value);

			LTVIPDataManager.Instance.UpdateVIPBaseData();
			Refresh();
		}

		private void Refresh()
		{
			VIPBaseInfo info = LTVIPDataManager.Instance.GetVIPBaseInfo();
			Level.text = info.Level.ToString();
		}
	}
}