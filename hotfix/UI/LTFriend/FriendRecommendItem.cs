namespace Hotfix_LT.UI
{
    using UnityEngine;
    using System.Collections;
    using System;
    using Hotfix_LT.Data;

    public class FriendRecommendItem : DynamicCellController<RecommendFriendData>
    {
        public UILabel NameLabel, LevelLabel/*,DescLabel*/;
        public UIButton AddBtn;
        public UISprite HeadSprite;
        public UISprite FrameSprite;
        public UISprite BG;
        private RecommendFriendData mData;


        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            NameLabel = t.GetComponent<UILabel>("Name");
            LevelLabel = t.GetComponent<UILabel>("Head/Level/Label");
            AddBtn = t.GetComponent<UIButton>("Add");
            HeadSprite = t.GetComponent<UISprite>("Head/Icon");
            FrameSprite= t.GetComponent<UISprite>("Head/Icon/Frame");
            BG = t.GetComponent<UISprite>("BG/Sprite");

            t.GetComponent<UIButton>("Add").onClick.Add(new EventDelegate(OnAddBtnClick));

            t.GetComponent<UIEventTrigger>("Head/Icon").onClick.Add(new EventDelegate(OnHeadBtnClick));

        }

        public override void Clean()
        {
        }

        public override void Fill(RecommendFriendData data)
        {
            mData = data;
            SetItemBG(DataIndex % 2);

            NameLabel.text = NameLabel.transform.GetChild(0).GetComponent<UILabel>().text = mData.Name;
            LevelLabel.text = mData.Level.ToString();

            Hotfix_LT.Data.HeroInfoTemplate heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(int.Parse(data.Head), data.Skin);
            if (heroInfo != null)
            {
                HeadSprite.spriteName = heroInfo.icon;
            }
            else
            {
                HeadSprite.spriteName = "Partner_Head_Sidatuila";
            }
            FrameSprite.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(data.Frame).iconId;

            AddBtn.gameObject.SetActive(!mData.IsFriend);
        }

        public void SetItemBG(int index)
        {
            // index: 0：浅底，1：深底
            BG.spriteName = index == 0 ? "Ty_Mail_Di1" : "Ty_Mail_Di2";
        }

        public void OnHeadBtnClick()
        {
            FriendHudController.OpenPlayerInfoPanel(mData);
        }

        public void OnAddBtnClick()
        {
            if (mData.IsFriend)
            {
                MessageTemplateManager.ShowMessage(FriendManager.CodeHasFriend);
                return;
            }

            if (FriendManager.Instance.Info.MyFriendNum >= FriendManager.Instance.Config.MaxFriendNum)
            {
                MessageTemplateManager.ShowMessage(FriendManager.CodeFriendNumMax);
                return;
            }
            var ht = Johny.HashtablePool.Claim();
            ht.Add("uid", mData.Uid);
            ht.Add("addWay", eFriendAddWay.Commend);
            GlobalMenuManager.Instance.Open("FriendApplyUI", ht);
        }
    }

}