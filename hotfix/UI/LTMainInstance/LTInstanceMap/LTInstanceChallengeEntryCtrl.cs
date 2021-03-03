using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTInstanceChallengeEntryCtrl : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            levelLabel = t.GetComponent<UILabel>("ChallengeBtn/Label");
            curLayerRate = t.GetComponent<UILabel>("EquipmentItem/IMG/Label"); 
            NextLayerRate = t.GetComponent<UILabel>("EquipmentItem1/IMG/Label");
            curBGFrame = t.GetComponent<UISprite>("EquipmentItem/IMG/LvlBorder/Bg");
            curQuaLevel = t.GetComponent<UISprite>("EquipmentItem/IMG/LvlBorder");
            nextBGFrame = t.GetComponent<UISprite>("EquipmentItem1/IMG/LvlBorder/Bg");
            nextQuaLevel = t.GetComponent<UISprite>("EquipmentItem1/IMG/LvlBorder");
            BgSprite = t.GetComponent<UISprite>("ChallengeBtn");
            curEquipIcon = t.GetComponent<DynamicUISprite>("EquipmentItem/IMG");
            nextEquipIcon = t.GetComponent<DynamicUISprite>("EquipmentItem1/IMG");
            showReward = t.GetMonoILRComponent<ShowChallengeRewardCtrl>("Tips");
            Fx = t.FindEx("ChallengeBtn/fx").gameObject;

            t.GetComponent<ConsecutiveClickCoolTrigger>("ChallengeBtn").clickEvent.Add(new EventDelegate(OnCliclkChallengeBtn));
            t.GetComponent<ConsecutiveClickCoolTrigger>("EquipmentItem/IMG").clickEvent.Add(new EventDelegate(OnClickCurFloor));
            t.GetComponent<ConsecutiveClickCoolTrigger>("EquipmentItem1/IMG").clickEvent.Add(new EventDelegate(OnClickNextFloor));
        }
        
        public UILabel levelLabel,curLayerRate,NextLayerRate;
        public UISprite curBGFrame, curQuaLevel,nextBGFrame, nextQuaLevel,BgSprite;
        public DynamicUISprite curEquipIcon, nextEquipIcon;
        public ShowChallengeRewardCtrl showReward;
        public GameObject Fx;
        private int maxLevel = 0;
        private int curQuality;
        private int nextQuality;
        private EquipPartType showType = EquipPartType.part1;
        private Hotfix_LT.Data.LostChallengeRewardTemplate curReward;
        private Hotfix_LT.Data.LostChallengeRewardTemplate nextReward;
        private Hotfix_LT.Data.FuncTemplate m_FuncTpl;
        private int maxfloor;
    
        /// <summary>
        /// 点击挑战入口
        /// </summary>
        public void OnCliclkChallengeBtn()
        {
            if (AllianceUtil.GetIsInTransferDart("")) return;
            if (!m_FuncTpl.IsConditionOK()) MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, m_FuncTpl.GetConditionStr());
            else GlobalMenuManager.Instance.Open("LTChallengeInstanceSelectHud");
    
        }
    
        public void OnClickCurFloor()
        {
            showReward.ShowUI(string.Format("{0}{1}", (curReward.DropRate * 100).ToString("0"), "%"), curQuality);
        }
    
        public void OnClickNextFloor()
        {
            showReward.ShowUI(string.Format("{0}{1}", (nextReward.DropRate * 100).ToString("0"), "%"), nextQuality);
        }
        /// <summary>
        /// 初始化收益显示
        /// </summary>
        public void SetShowInfo()
        {
            int prelevel = maxLevel;
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.bigFloor", out int cacheLevel);
            maxLevel = cacheLevel > maxLevel ? cacheLevel : maxLevel;
            if (maxLevel == 0 || maxLevel > prelevel)
            {            
                maxfloor = Hotfix_LT.Data.SceneTemplateManager.Instance.GetMaxLostChallengeChapterId();
                maxLevel += 1;
                if (maxfloor < maxLevel) maxLevel = maxfloor;
                levelLabel.text = maxLevel.ToString();
                curReward = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeReward(maxLevel);
                nextReward = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeReward(maxLevel + 1);
                curQuality = curReward.DropList[0] == null ? 1 : int.Parse(curReward.DropList[0].Substring(1, 1));
                nextQuality = nextReward.DropList[0] == null ? 1 : int.Parse(nextReward.DropList[0].Substring(1, 1));
                curBGFrame.spriteName = UIItemLvlDataLookup.GetItemFrameBGSprite(curQuality);
                curBGFrame.color = LT.Hotfix.Utility.ColorUtility.QualityToFrameColor(curQuality);
                nextBGFrame.spriteName = UIItemLvlDataLookup.GetItemFrameBGSprite(nextQuality);
                nextBGFrame.color = LT.Hotfix.Utility.ColorUtility.QualityToFrameColor(nextQuality);
                curQuaLevel.spriteName = UIItemLvlDataLookup.LvlToStr(curQuality.ToString());
                nextQuaLevel.spriteName = UIItemLvlDataLookup.LvlToStr(nextQuality.ToString());
                curLayerRate.text = string.Format("{0}{1}", (curReward.DropRate * 100).ToString("0"), "%");
                NextLayerRate.text = string.Format("{0}{1}", (nextReward.DropRate * 100).ToString("0"), "%");
                curEquipIcon.spriteName = LTPartnerEquipDataManager.Instance.GetEquipIconBuyTypeAndQua(showType, curQuality);
                nextEquipIcon.spriteName = LTPartnerEquipDataManager.Instance.GetEquipIconBuyTypeAndQua(showType, nextQuality);
                mDMono.GetComponent<TweenAlpha>().PlayForward();
            }
        } 
        
        //关闭特效
        public void SetFxState(bool isShow)
        {
            if (isShow)
            {
                m_FuncTpl = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10065);
                if (!m_FuncTpl.IsConditionOK()) BgSprite.color = new Color(1, 0, 1, 1);
                else
                {
                    BgSprite.color = new Color(1, 1, 1, 1);
                    Fx.CustomSetActive(true);
                }
            }
            else
            {
                Fx.CustomSetActive(false);
            }
        }
    }
}
