using System;
using UnityEngine.UI;

namespace Hotfix_LT.UI
{
    using System.Collections;

    public class FriendBlacklistRemoveController : UIControllerHotfix {

        public override bool ShowUIBlocker { get { return true; } }
        private long mUid;
        private Action mCallback;
        UIButton HotfixBtn1;
        UIButton HotfixBtn2;
        public override void Awake()
        {
            base.Awake();
            UIButton backButton = controller.transform.Find("Frame/CloseBtn").GetComponent<UIButton>();
            backButton.onClick.Add(new EventDelegate(base.OnCancelButtonClick));
            
            HotfixBtn1 =  controller.transform.Find("Frame/Content/AddBtn").GetComponent<UIButton>();
            HotfixBtn1.onClick.Add(new EventDelegate(OnAddClick));
            HotfixBtn2 =  controller.transform.Find("Frame/Content/RemoveBtn").GetComponent<UIButton>();
            HotfixBtn2.onClick.Add(new EventDelegate(OnRemoveClick));
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);

            Hashtable ht=param as Hashtable;
            mUid = (long)ht["uid"];
            mCallback = (Action)ht["callback"];
        }

        public void OnAddClick()
        {
            if (FriendManager.Instance.Info.MyFriendNum >= FriendManager.Instance.Config.MaxFriendNum)
            {
                MessageTemplateManager.ShowMessage(FriendManager.CodeFriendNumMax);
                return;
            }

            var ht = Johny.HashtablePool.Claim();
            ht.Add("uid",mUid);
            ht.Add("addWay",eFriendAddWay.FromBlack);
            ht.Add("callback", mCallback);
            GlobalMenuManager.Instance.Open("FriendApplyUI",ht);
            controller.Close();
        }

        public void OnRemoveClick()
        {
            FriendManager.Instance.RemoveBlacklist(mUid, delegate (bool successful)
            {
                if (successful)
                {
                    FriendManager.Instance.Info.BlacklistNum--;
                    FriendManager.Instance.BlackLists.Remove(mUid);
                    mCallback();

                    MessageTemplateManager.ShowMessage(902254);
                }
            });

                controller.Close();
        }
    }

}