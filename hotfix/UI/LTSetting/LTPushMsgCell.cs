using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTPushMsgCell : DynamicCellController<LTDailyData>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            NameLabel = t.GetComponent<UILabel>("Name");
            TimeLabel = t.GetComponent<UILabel>("Time");
            SelectObj = t.GetComponent<UISprite>("Bg/Sprite");

            t.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnPushMsgCellClick));
        }

        public UILabel NameLabel,TimeLabel;
        public UISprite SelectObj;
        private LTDailyData m_Data;
        private bool isSelect;
    
        public override void Clean()
        {
            mDMono.gameObject.SetActive(false);
        }
    
        public override void Fill(LTDailyData itemData)
        {
            if (itemData == null)
            {
                m_Data = null;
                Clean();
                return;
            }
            m_Data = itemData;
            mDMono.gameObject.SetActive(true);
            NameLabel.text = NameLabel.transform.GetChild(0).GetComponent<UILabel>().text = m_Data.ActivityData.display_name;
            TimeLabel.text= m_Data.OpenTimeStr;
            isSelect = UserData.PushMessageDataDic[m_Data.ActivityData.id];
            SelectObj.gameObject.CustomSetActive(isSelect);
        }

        public void OnPushMsgCellClick()
        {
            if (m_Data == null) return;
            isSelect = !isSelect;

            UserData.PushMessageDataDic[m_Data.ActivityData.id] = isSelect;
            SelectObj.gameObject.CustomSetActive(isSelect);
            UserData.SetPushMsgPref(m_Data.ActivityData.id);
        }
    }
}
