using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class InviteSharePage : DynamicMonoHotfix
    {
        private UILabel inviteCodelabel, inviteCodeShadow, inviteReward, inviteRewardShadow;
        private GameObject dailyrewardobj;
        private const string uidstr = "userid=###", appidstr = "appid=###";
        private string shareStr;
        private int timer;
        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            inviteCodelabel = t.GetComponent<UILabel>("Content/Code");
            inviteCodeShadow = t.GetComponent<UILabel>("Content/Code/Label (1)");
            inviteReward = t.GetComponent<UILabel>("Content/ShareDes");
            inviteRewardShadow = t.GetComponent<UILabel>("Content/ShareDes/Label (1)");
            dailyrewardobj = t.GetComponent<Transform>("Content/ShareDes").gameObject;
            t.GetComponent<UIButton>("ShareBtn").onClick.Add(new EventDelegate(OnClickShareBtn));
            inviteCodelabel.text = inviteCodeShadow.text = PlayerInviteManager.Instance.InviteCode;
            inviteReward.text = inviteRewardShadow.text = string.Format(EB.Localizer.GetString("ID_INVITE_19"), PlayerInviteManager.Instance.DailyShareReward);
            shareStr = string.Format(EB.Localizer.GetString("ID_INVITE_20"), PlayerInviteManager.Instance.InviteCode);
            shareStr = shareStr.Replace(uidstr, string.Format("userid={0}", LoginManager.Instance.LocalUserId));
            int appid = 1;
#if USE_XINKUAISDK
            appid = EB.Sparx.XinkuaiSDKManager.getInstance().GetAppid();
#endif
            shareStr = shareStr.Replace(appidstr, string.Format("appid={0}", appid));
        }

        private void OnClickShareBtn()
        {
            //调用系统分享接口
            if (PlayerInviteManager.Instance.CouldRecieveDailyShare)
            {
                PlayerInviteManager.isFirstShare = true;
            }
            if (timer == 0)
            {
                timer = ILRTimerManager.instance.AddTimer(200, 1, SendSharereq);
            }
        }

        private void SendSharereq(int seq)
        {
            PlayerInviteManager.sharetime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            timer = 0;
            ShareToManager.ShareTextBySystem(shareStr);
        }
        public void Show(bool isshow)
        {
            SetRewardTip();
            mDMono.gameObject.CustomSetActive(isshow);
        }
        private void SetRewardTip()
        {
            dailyrewardobj.CustomSetActive(PlayerInviteManager.Instance.CouldRecieveDailyShare);
        }
        public override void OnEnable()
        {
            Messenger.AddListener(EventName.OnInviteShareSucceed, SetRewardTip);
        }

        public override void OnDisable()
        {
            Messenger.RemoveListener(EventName.OnInviteShareSucceed, SetRewardTip);
            if (timer != 0)
            {
                ILRTimerManager.instance.RemoveTimer(timer);
                timer = 0;
            }
        }
    }
}