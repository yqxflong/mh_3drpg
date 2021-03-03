using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class RobDartItem : DynamicMonoHotfix
    {
        public GameObject Container;
        public UILabel DartNameLabel;
        public UISprite DartQualityBG;
        public UILabel PlayerLevelLabel;
        public UISprite HeroPortrait;
        public UISprite FramePortrait;
        public LTShowItem RewardItem1, RewardItem2;
        private RobDartMember mItemData;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            Container = t.FindEx("Container").gameObject;
            DartNameLabel = t.GetComponent<UILabel>("Container/Name");
            DartQualityBG = t.GetComponent<UISprite>("Container/BGs/CardBG (1)");
            PlayerLevelLabel = t.GetComponent<UILabel>("Container/TargetLevelBG/Level");
            HeroPortrait = t.GetComponent<UISprite>("Container/TargetIconBG/Icon");
            FramePortrait = t.GetComponent<UISprite>("Container/TargetIconBG/Icon/Frame");
            RewardItem1 = t.GetMonoILRComponent<LTShowItem>("Container/Award/LTShowItem");
            RewardItem2 = t.GetMonoILRComponent<LTShowItem>("Container/Award/LTShowItem (1)");
            t.GetComponent<UIButton>("Container/RobBtn").onClick.Add(new EventDelegate(() => t.parent.parent.GetMonoILRComponent<RobDartController>().OnRobClick(t.GetMonoILRComponent<RobDartItem>())));
        }


        public RobDartMember GetItemData()
        {
            return mItemData;
        }

        public void Clear()
        {
            Container.CustomSetActive(false);
        }

        public void Fill(RobDartMember data)
        {
            mItemData = data;
            LTUIUtil.SetText(DartNameLabel, AllianceEscortUtil.LocalizeDartName(data.DartName));
            DartQualityBG.spriteName = AllianceEscortUtil.GetDartQualityBGSpriteName(data.DartName);
            PlayerLevelLabel.text = data.PlayerLevel.ToString();
            HeroPortrait.spriteName = data.Portrait;
            FramePortrait.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(data.HeadFrame).iconId;
            RewardItem1.LTItemData = data.Award[0];
            RewardItem2.LTItemData = data.Award[1];
            Container.CustomSetActive(true);
        }
    }
}
