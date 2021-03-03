using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{

    public class LTPartnerHandbookTakeTheFieldCtrl : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return true; } }

        public LTHandbookTakeTheFieldScroll _HandbookTakeTheFieldScroll;

        public UILabel BookName;

        public HandbookCardData Data;

        public override void Awake()
        {
            base.Awake();

            _HandbookTakeTheFieldScroll = controller.transform.Find("Anchor_Mid/Content/Partners/Placeholder/Grid").GetMonoILRComponent<LTHandbookTakeTheFieldScroll>();
            BookName = controller.transform.Find("Anchor_Mid/Content/Top/Title").GetComponent<UILabel>();
            controller.backButton = controller.transform.Find("Anchor_Mid/Content/Top/CloseBtn").GetComponent<UIButton>();

        }
        
        public override void SetMenuData(object param)
        {
            Data = (HandbookCardData)param;
            switch (Data.handbookId)
            {
                case Hotfix_LT.Data.eRoleAttr.Feng: BookName.text = EB.Localizer.GetString("ID_feng_HandBook"); break;
                case Hotfix_LT.Data.eRoleAttr.Shui: BookName.text = EB.Localizer.GetString("ID_shui_HandBook"); break;
                case Hotfix_LT.Data.eRoleAttr.Huo: BookName.text = EB.Localizer.GetString("ID_huo_HandBook"); break;
                default: BookName.text = string.Empty; break;
            }
            var temp = LTPartnerDataManager.Instance.GetOwnPartnerListByCharType(Data.handbookId);
            temp.Sort((a, b) =>
            {
                if (a == b) return 0;//比较相同元素需要返回0
                else if (a.HeroId.ToString().Equals(Data.BuddyId) && !b.HeroId.ToString().Equals(Data.BuddyId)) return -1;
                else if (!a.HeroId.ToString().Equals(Data.BuddyId) && b.HeroId.ToString().Equals(Data.BuddyId)) return 1;
                else
                {
                    if (LTPartnerHandbookManager.Instance.GetAttrAddValue(a) > LTPartnerHandbookManager.Instance.GetAttrAddValue(b)) return -1;
                    else return 1;
                }
            });
            _HandbookTakeTheFieldScroll.SetItemDatas(temp);
        }

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            _HandbookTakeTheFieldScroll.Clear();
            DestroySelf();
            yield break;
        }
    }

}
