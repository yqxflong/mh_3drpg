namespace Hotfix_LT.UI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class FriendItemDynamicScrollManager:DynamicMonoHotfix
    {
        public FriendItemDynamicScroll first;
        public FriendItemDynamicScroll second;


        public override void Awake()
        {
            base.Awake();
            first = mDMono.transform.GetMonoILRComponent<FriendItemDynamicScroll>("ScrollPos1/MyFriendScrollViewPanel/Placeholder/Grid");
            second = mDMono.transform.GetMonoILRComponent<FriendItemDynamicScroll>("ScrollView/MyFriendScrollViewPanel/Placeholder/Grid");
        }

        public void SetDataItems(List<FriendData> list,bool sendVigor)
        {
            if (sendVigor)
            {
                first.scrollView.transform.parent.gameObject.SetActive(list.Count>0);
                second.scrollView.transform.parent.gameObject.SetActive(false);
                first. SetItemDatas(list.ToArray());
            }
            else
            {
                first.scrollView.transform.parent.gameObject.SetActive(false);
                second.scrollView.transform.parent.gameObject.SetActive(true);
                second.SetItemDatas(list.ToArray());
            }
        }

        public void MoveTo(int index,bool sendVigor)
        {
            if (sendVigor)
            {
                first.MoveTo(index);
            }
            else
            {
                second.MoveTo(index);
            }
        }
    }
}