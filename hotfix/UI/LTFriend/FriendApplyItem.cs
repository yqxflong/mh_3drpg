
namespace Hotfix_LT.UI
{
    using UnityEngine;
    using System.Collections;
    using System;
    using Hotfix_LT.Data;

    public class FriendApplyItem : DynamicCellController<ApplyFriendData>
    {
        public UILabel NameLabel, LevelLabel, OfflineTimeLabel, VerifyLabel;
        public UISprite HeadSprite;
        public UISprite FrameSprite;
        public UISprite BG;
        private ApplyFriendData mData;


        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            NameLabel = t.GetComponent<UILabel>("Name");
            LevelLabel = t.GetComponent<UILabel>("Head/Level/Label");
            OfflineTimeLabel = t.GetComponent<UILabel>("Time");
            VerifyLabel = t.GetComponent<UILabel>("Hello");
            HeadSprite = t.GetComponent<UISprite>("Head/Icon");
            FrameSprite = t.GetComponent<UISprite>("Head/Icon/Frame");
            BG = t.GetComponent<UISprite>("BG/Sprite");

            t.GetComponent<ConsecutiveClickCoolTrigger>("Agree").clickEvent.Add(new EventDelegate(OnAgreeBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Reject").clickEvent.Add(new EventDelegate(OnRejectBtnClick));

            t.GetComponent<UIEventTrigger>("Head/Icon").onClick.Add(new EventDelegate(OnHeadBtnClick));

        }

        public override void Clean()
        {

        }

        public override void Fill(ApplyFriendData data)
        {
            mData = data;
            //data.Head = "Partner_Head_Sidatuila";//Header_Sidatuilayitong";头像更改
            NameLabel.text = NameLabel.transform.GetChild(0).GetComponent<UILabel>().text = mData.Name;
            LevelLabel.text = mData.Level.ToString();
            OfflineTimeLabel.text = AlliancesManager.FormatOfflineDuration(mData.OfflineTime);
            //HeadSprite.spriteName = string.Format("{0}", data.Head);//_Main
            VerifyLabel.text = mData.VerifyInfo;
            SetItemBG(DataIndex % 2);

            Hotfix_LT.Data.HeroInfoTemplate heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(int.Parse(data.Head), data.Skin);
            if (heroInfo != null)
            {
                HeadSprite.spriteName = heroInfo.icon;
            }
            FrameSprite.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(data.Frame).iconId;
        }

        public void SetItemBG(int index)
        {
            // index: 0：浅底，1：深底
            BG.spriteName = index == 0 ? "Ty_Mail_Di1" : "Ty_Mail_Di2";
        }

        public void SetItemIndex(int index)
        {
            //设置该底的名字为0或1，用于将来设置该底使用哪张底图
            BG.name = index.ToString();
        }

        public void OnHeadBtnClick()
        {
            FriendHudController.OpenPlayerInfoPanel(mData);
        }

        public void OnAgreeBtnClick()
        {
            if (FriendManager.Instance.Info.MyFriendNum >= FriendManager.Instance.Config.MaxFriendNum)
            {
                MessageTemplateManager.ShowMessage(FriendManager.CodeFriendNumMax);
                return;
            }
            FriendManager.Instance.Info.MyFriendNum++;//容错，如果有多个好友申请的时候快速点同意，此时服务器还没有把好友数据下发下来，如果此时好友正好满了就会报错；客户端先加，服务器返回数据之后会重新设置

            string name = mData.Name;
            FriendManager.Instance.Agree(mData.Uid, delegate (bool successful)
            {
                if (successful)
                {
                    FriendManager.Instance.Applys.Remove(mData.Uid);
                    FriendManager.Instance.Info.ApplyCount--;
                    if (FriendManager.Instance.Info.ApplyCount <= 0)
                    {
                        Messenger.Raise(Hotfix_LT.EventName.FriendApplyEvent, false);
                    }
                    GameDataSparxManager.Instance.SetDirty(FriendManager.ApplyListId);
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("0", name);
                    MessageTemplateManager.ShowMessage(FriendManager.CodeAddFriend, ht, null);
                }
            });
        }

        public void OnRejectBtnClick()
        {
            FriendManager.Instance.Reject(mData.Uid, delegate (bool successful)
            {
                if (successful)
                {
                    FriendManager.Instance.Applys.Remove(mData.Uid);
                    FriendManager.Instance.Info.ApplyCount--;
                    if (FriendManager.Instance.Info.ApplyCount <= 0)
                    {
                        Messenger.Raise(Hotfix_LT.EventName.FriendApplyEvent, false);
                    }
                    GameDataSparxManager.Instance.SetDirty(FriendManager.ApplyListId);
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("0", mData.Name);
                    MessageTemplateManager.ShowMessage(FriendManager.CodeRejectOtherInvite, ht, null);
                }
            });
        }
    }

}