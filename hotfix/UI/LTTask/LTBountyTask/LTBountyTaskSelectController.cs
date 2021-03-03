using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTBountyTaskSelectController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            m_Scroll = t.GetMonoILRComponent<LTBountyTaskSelectScroll>("ScrollView/Placeholder/HeroGrid");
            Target = 0;
            t.GetComponent<UIButton>("Bg/Bg1/Top/CloseBtn").onClick
                .Add(new EventDelegate(OnCancelButtonClick));
        }

        public override bool ShowUIBlocker { get { return true; } }
    
        public LTBountyTaskSelectScroll m_Scroll;
    
        public static int Target;
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            if (param != null)
            {
                Target = (int)param;
            }
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            List<LTPartnerData> partnerList = LTPartnerDataManager.Instance.GetOwnPartnerList();
            List<LTPartnerData> filterList = new List<LTPartnerData>();
            for (int i = 0; i < partnerList.Count; i++)
            {
                int grade = partnerList[i].HeroInfo.role_grade;
                if (grade > 2&&  grade <5)filterList.Add(partnerList[i]);//SR品阶以上才要显示
            }
            m_Scroll.SetItemDatas(filterList);
            Messenger.AddListener(Hotfix_LT.EventName.BountyTask_Select, CloseEven);
            
    
        }
        public override IEnumerator OnRemoveFromStack()
        {
            Messenger.RemoveListener(Hotfix_LT.EventName.BountyTask_Select, CloseEven);
            DestroySelf();
            yield break;
        }
    
        public void CloseEven()
        {
            OnCancelButtonClick();
        }
    }
}
