using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTUltimateTrialCompeteRankItemData : PersonalRankItemData
    {
        public LTUltimateTrialCompeteRankItemData() : base()
        {
            m_Parm = string.Format("{0}[fff348]{1}[-]\n{2}[fff348]{1}[-]", EB.Localizer.GetString("ID_ULTIMATE_COMPETE_TIP6"), EB.Localizer.GetString("ID_LEGION_MEDAL_NOT"), EB.Localizer.GetString("ID_ULTIMATE_COMPETE_TOTLETIME3")); 
        }

        public LTUltimateTrialCompeteRankItemData(Hashtable data, int index) : base(data, index)
        {
            int temp = EB.Dot.Integer("ly", data, 0);
            string top =string.Format("{0}[fff348]{1}[-]\n",EB.Localizer .GetString("ID_ULTIMATE_COMPETE_TIP6"), temp);
            temp = EB.Dot.Integer("tt", data, 0);
            string timetemp = EB.Localizer.GetString("ID_LEGION_MEDAL_NOT");
            if (temp >= Data.EventTemplateManager.Instance.GetAllInfiniteCompete().Count)
            {
                int hour= temp / 3600;
                if (hour > 99)
                {
                    timetemp = "99:59:60";
                }
                else
                {
                    timetemp = string.Format("{0}:{1}:{2}", hour.ToString("00"), (temp / 60 % 60).ToString("00"), (temp % 60).ToString("00"));
                }
            }
            string bottom =string.Format("{0}[fff348]{1}[-]", EB.Localizer.GetString("ID_ULTIMATE_COMPETE_TOTLETIME3"), timetemp);

            m_Parm = string.Format("{0}{1}",top, bottom);
        }
    }
}
