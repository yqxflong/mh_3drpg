using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTUltimateTrialCompeteItem : DynamicCellController<Hotfix_LT.Data.InfiniteCompeteTemplate>
    {
        public UILabel NumLabel, StateLabel;
        public LTShowItem[] ShowItems;
        public GameObject FirstTipObj,HasGetObj;
        public UISprite BG;

        private int id;

        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            NumLabel = t.GetComponent<UILabel>("IndexLabel");
            BG = t.GetComponent<UISprite>();
            StateLabel = t.GetComponent<UILabel>("InfoLabel");
            ShowItems = new LTShowItem[2];
            ShowItems[0] = t.GetMonoILRComponent<LTShowItem>("Awards/LTShowItem");
            ShowItems[1] = t.GetMonoILRComponent<LTShowItem>("Awards/LTShowItem (1)");
            FirstTipObj = t.Find("Awards/FirstTip").gameObject;
            HasGetObj= t.Find("Awards/HasGet").gameObject;
            t.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnBtnClick));
        }
        
        public override void Clean()
        {
            id = 0;
        }

        public override void Fill(InfiniteCompeteTemplate itemData)
        {
            if (itemData == null)
            {
                Clean();
                return;
            }
            id = itemData.id;
            NumLabel.text = id.ToString();
            int time = LTUltimateTrialDataManager.Instance.GetCurCompeteTime(id);
            if (time>0)
            {
                FirstTipObj.CustomSetActive(false);
                HasGetObj.CustomSetActive(true);
                int hour = time / 60;
                if (hour > 99)
                {
                    StateLabel.text = "99:60";
                }
                else
                {
                    StateLabel.text = string.Format("{0}:{1}", hour.ToString("00"), (time % 60).ToString("00"));
                }
            }
            else
            {
                HasGetObj.CustomSetActive(false);
                FirstTipObj.CustomSetActive(true);
                StateLabel.text = EB.Localizer.GetString("ID_LEGION_MEDAL_NOT");
            }

            for (int i=0;i< ShowItems.Length; ++i)
            {
                if (i < itemData.first_award.Count)
                {
                    ShowItems[i].LTItemData = itemData.first_award[i];
                    ShowItems[i].mDMono.gameObject.CustomSetActive(true);
                }
                else
                {
                    ShowItems[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
            BG.spriteName = (id == LTUltimateTrialDataManager.Instance.curCompeteLevel) ? "Welfare_Flag_Di1": "Welfare_Frame_2";
        }

        public void OnBtnClick()
        {
            if (LTUltimateTrialDataManager.Instance.curCompeteLevel == id) return;
            if (LTUltimateTrialDataManager.Instance.OnCompeteSelect != null)
            {
                LTUltimateTrialDataManager.Instance.curCompeteLevel = id;
                LTUltimateTrialDataManager.Instance.OnCompeteSelect(id);
            }
        }
    }
}