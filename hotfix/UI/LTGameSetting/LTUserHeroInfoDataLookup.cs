using Hotfix_LT.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTUserHeroInfoDataLookup : DataLookupHotfix {

        private string HeadFrameStr;
        private UISprite HeadFrameIcon;
        public override void OnLookupUpdate(string dataID, object value)
        {
            base.OnLookupUpdate(dataID, value);
            if (dataID != null && value != null)
            {
                string tmp = mDL.GetDefaultLookupData<string>();
                if (HeadFrameStr == null || !HeadFrameStr.Equals(tmp))
                {
                    HeadFrameStr = tmp;
                    if (HeadFrameIcon == null) HeadFrameIcon = mDL.GetComponent<UISprite>();
                    if (string.IsNullOrEmpty(HeadFrameStr))
                    {
                        HeadFrameIcon.spriteName = string.Empty;
                    }
                    else
                    {
                        string[] split = HeadFrameStr.Split('_');
                        HeadFrame data = EconemyTemplateManager.Instance.GetHeadFrame(split[0], int.Parse(split[1]));
                        HeadFrameIcon.spriteName = data.iconId;
                    }
                }
            }
        }
    }
}
