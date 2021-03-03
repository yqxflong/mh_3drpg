using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class PlayerFormationInfo : DynamicMonoHotfix
    {
        public UIGrid grid;

        public List<OtherPlayerPartnerData> partnerList;

        public FormationPartnerItem[] Items = new FormationPartnerItem[6];

        public override void Awake()
        {
            base.Awake();
            grid = mDMono.transform.Find("UIGrid").GetComponent<UIGrid>();
            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = mDMono.transform.Find("UIGrid/Item (" + i + ")").GetMonoILRComponent<FormationPartnerItem>();
            }
            
        }

        public void Init(long uid, List<OtherPlayerPartnerData> partnerList)
        {
            if (!mDMono.gameObject.activeInHierarchy)
            {
                return;
            }

            if (partnerList != null)
            {
                this.partnerList = partnerList;
            }

            grid.gameObject.CustomSetActive(false);

            LegionLogic.GetInstance().OnSendGetPlayerFormationInfo(uid, FetchDataHandler);
        }

        private void ShowPlayerFormation()
        {
            grid.gameObject.CustomSetActive(true);
            for (int i = 0; i < Items.Length; i++)
            {
                if (i < AlliancesManager.Instance.PlayerFormationInfo.playerForData.Count)
                {
                    Items[i].Fill(partnerList[i]);
                }
                else
                {
                    Items[i].Fill(null);
                }
            }

            grid.Reposition();
        }

        private void FetchDataHandler(Hashtable alliance)
        {
            if (alliance != null)
            {
                GameDataSparxManager.Instance.ProcessIncomingData(alliance, false);
                ShowPlayerFormation();
            }
        }
    }
}

