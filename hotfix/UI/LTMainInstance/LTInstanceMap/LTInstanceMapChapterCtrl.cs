using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTInstanceMapChapterCtrl : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            ContainerObj = t.Find("Container").gameObject;
            InfoObj = t.Find("Container/Info").gameObject;
            BoxTipObj = t.Find("Container/BoxTip").gameObject;
            BGSprite = t.GetComponent<CampaignTextureCmp>();
            StarLabel = t.GetComponent<UILabel>("Container/Info/Star/Num");
            NameLabel = t.GetComponent<UILabel>("Container/Info/Name");
            BoxLabel = t.GetComponent<UILabel>("Container/BoxTip/Tip");
            RedPoint = t.FindEx("Container/RedPoint").gameObject;
            PerfectObj = t.FindEx("Container/Perfect").gameObject;
            t.GetComponent<ConsecutiveClickCoolTrigger>().clickEvent.Add(new EventDelegate(OnChapterClick));
            newChapterFX = t.Find("Container/NewChapterFX").gameObject;

            NeedShowFX = false;
            chapterId = 0;
        }

        public GameObject ContainerObj;

        public GameObject InfoObj;
        
        public GameObject BoxTipObj;

        public CampaignTextureCmp BGSprite;
        
        public UILabel StarLabel;

        public UILabel NameLabel;

        public UILabel BoxLabel;

        public GameObject RedPoint;

        public GameObject PerfectObj;
       
        private GameObject newChapterFX;

        public bool NeedShowFX = false;

        private Hotfix_LT.Data.LostMainChapterTemplate m_ChapterData;

        public int chapterId;

        public void Init(Hotfix_LT.Data.LostMainChapterTemplate data)
        {
            m_ChapterData = data;
            mDMono.transform.name = m_ChapterData.Id;
            chapterId = int.Parse(m_ChapterData.Id);
            newChapterFX.CustomSetActive(false);
            ContainerObj.CustomSetActive(true);

            BGSprite.spriteName = m_ChapterData.Icon;
            if (m_ChapterData.IsBoxRewardType())
            {
                InfoObj.CustomSetActive(false);
                BoxTipObj.CustomSetActive(true);
                PerfectObj.CustomSetActive(false);
               int mForwardChapterId = 0;
                int.TryParse(m_ChapterData.ForwardChapterId, out mForwardChapterId);
                BoxLabel.text = string.Format(EB.Localizer.GetString("ID_INSTANCE_MAP_BOX_REWRAD_TIP"), mForwardChapterId % 100);
                if (LTInstanceUtil.IsChapterComplete(m_ChapterData.ForwardChapterId))
                {
                    if (LTInstanceMapModel.Instance.GetMainChapterRewardState(m_ChapterData.Id))
                    {
                        ContainerObj.CustomSetActive(false);
                    }
                    else
                    {
                        RedPoint.CustomSetActive(true);
                    }
                }
                else
                {
                    RedPoint.CustomSetActive(false);
                }
                BGSprite.target.width =376;
                BGSprite.target.height =360;
            }
            else
            {
                InfoObj.CustomSetActive(true);
                BoxTipObj.CustomSetActive(false);

                int allStarNum = LTInstanceUtil.GetChapterCurStarNum(m_ChapterData.Id);
                int fullStarNum = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainChapterMaxStarNumById(m_ChapterData.Id);
                PerfectObj.CustomSetActive(LTInstanceUtil.GetChapterIsPerfectComplete(m_ChapterData.Id) && allStarNum == fullStarNum);
                StarLabel.text =  string.Format("{0}{1}/{2}", (allStarNum >= fullStarNum) ? "[42fe76]" : "", allStarNum, fullStarNum);
                NameLabel.text =  m_ChapterData.Name;
                UpdateRedPoint();
                BGSprite.target.width = 360;
                BGSprite.target.height = 420;
            }
            mDMono.transform.localPosition = data.ChapterPos;
            mDMono.gameObject.CustomSetActive(true);
        }
        
        public void PlayNewChapterFX(bool show = true)
        {
            newChapterFX.CustomSetActive(show);
        }
        
        private void UpdateRedPoint()
        {
            bool IsBoxOpen = true;
            int starNum = LTInstanceUtil.GetChapterCurStarNum(m_ChapterData.Id);
            foreach (int key in m_ChapterData.RewardDataDic.Keys)
            {
                if (starNum >= key)
                {
                    IsBoxOpen = LTInstanceUtil.IsChapterStarBoxOpen(m_ChapterData.Id, key);
                }
                if (!IsBoxOpen) break;
            }
            RedPoint.CustomSetActive(!IsBoxOpen);
        }
        
        public void OnChapterClick()
        {
            if (m_ChapterData.IsBoxRewardType())
            {
                if (!LTInstanceUtil.IsChapterComplete(m_ChapterData.ForwardChapterId))
                {
                    int FC = 0;
                    int.TryParse(m_ChapterData.ForwardChapterId, out FC);
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_INSTANCE_MAP_BOX_REWRAD_TIP2"), FC % 100));
                    return;
                }
                if (LTInstanceMapModel.Instance.GetMainChapterRewardState(m_ChapterData.Id)) return;
                LTInstanceMapModel.Instance.RequestMainChapterReward(m_ChapterData.Id, delegate
                {
                    ContainerObj.CustomSetActive(false);
                    GlobalMenuManager.Instance.Open("LTShowRewardView", m_ChapterData.RewardDataDic[0]);
                });
                return;
            }

            FusionAudio.PostEvent("UI/General/ButtonClick", true);
            if (AllianceUtil.GetIsInTransferDart("ID_CAMPAGIN"))
            {
                return;
            }

            //此处判断进阶条件是否满足
            if (!LTInstanceUtil.GetIsChapterLimitConditionComplete(m_ChapterData, out int currNum))
            {
                //打开跳转界面
                GlobalMenuManager.Instance.Open("PlayerUpgradeTipView", new int[3] { m_ChapterData.Limitparam2, m_ChapterData.Limitparam1, currNum });
                return;
            }
            if (BalanceResourceUtil.GetUserLevel() < m_ChapterData.LevelLimit)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTInstanceMapChapterCtrl_2697"), m_ChapterData.LevelLimit));
                //GlobalMenuManager.Instance.Open("LTPlayerLevelUpTipView", "Levelup");
                return;
            }
            
            //主线章节入口
            if (LTInstanceMapHudController.curChapterID != null && LTInstanceMapHudController.curChapterID != m_ChapterData.Id) return;
            LTInstanceMapHudController.curChapterID = m_ChapterData.Id;

            if (!string.IsNullOrEmpty(m_ChapterData.BeforeChapter))
            {
                string flagStr = PlayerPrefs.GetString(LoginManager.Instance.LocalUserId.Value + m_ChapterData.BeforeChapter);

                if (string.IsNullOrEmpty(flagStr))
                {
                    LTStoryController.OpenStory(OnChapterClick, m_ChapterData.BeforeChapter);
                    PlayerPrefs.SetString(LoginManager.Instance.LocalUserId.Value + m_ChapterData.BeforeChapter, "True");//本地临时保存，等服务器做好了会保存到服务器
                    PlayerPrefs.Save();
                    return;
                }
            }
            Action act = new Action(delegate
            {
                LTMainInstanceHudController.EnterInstance(m_ChapterData.Id);
                LTInstanceMapHudController.curChapterID = null;
            });
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.PlayCloudFxEvent, act);
        }
    }
}
