using UnityEngine;
using System.Collections;
using System;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceSelectItemCtrl : DynamicCellController<Hotfix_LT.Data.LostChallengeChapter>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            SelectSprite = t.GetComponent<UISprite>("Unlock/Select");
            UnlockTran = t.GetComponent<Transform>("Unlock");
            LockTran = t.GetComponent<Transform>("Lock");
            LevelNumLabel = t.GetComponent<UILabel>("Unlock/Label");
            RedPoint = t.FindEx("RedPoint").gameObject;
            BGSprite = t.GetComponent<UISprite>("Unlock/Tex");

            t.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnItemClick));

            Hotfix_LT.Messenger.AddListener<int>(EventName.LTChallengeInstanceLevelSelect, OnLevelSelect);
            Hotfix_LT.Messenger.AddListener<int>(EventName.LTChallengeInstaceRewardGet, OnRewardGet);
        }

        public UISprite SelectSprite;
    
        public Transform UnlockTran;
    
        public Transform LockTran;
    
        public UILabel LevelNumLabel;
    
        public GameObject RedPoint;
    
        public UISprite BGSprite;
    
        private Hotfix_LT.Data.LostChallengeChapter curLevelData;
        
        private bool isLock = false;
    
        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener<int>(EventName.LTChallengeInstanceLevelSelect, OnLevelSelect);
            Hotfix_LT.Messenger.RemoveListener<int>(EventName.LTChallengeInstaceRewardGet, OnRewardGet);
        }
    
        public override void Fill(Hotfix_LT.Data.LostChallengeChapter itemData)
        {
            curLevelData = itemData;
    
            int maxLevel = 0;
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.bigFloor", out maxLevel);

            isLock = curLevelData.CurChapter > maxLevel + 1;

            UnlockTran.gameObject.CustomSetActive(!isLock);
    
            LockTran.gameObject.CustomSetActive(isLock);
    
            if (!isLock)
            {
                int level = curLevelData.CurChapter;
                LevelNumLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_68326"), level);
    
                SelectSprite.gameObject.CustomSetActive(curLevelData.Level == LTChallengeInstanceSelectHudController.CurSelectLevel);
    
                int taskId = 7000 + curLevelData.CurChapter;
                string state = string.Empty;
                DataLookupsCache.Instance.SearchDataByID(string.Format("tasks.{0}.state", taskId), out state);
                RedPoint.CustomSetActive(state == "finished");
    
                int floor = LTInstanceUtil.GetChallengeLevel(curLevelData.Level);
                Hotfix_LT.Data.LostChallengeRewardTemplate temp = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeReward(System.DateTime.Now.DayOfWeek, floor);
                if (temp != null)
                {
                    string id = temp.DropList[0];
                    var item = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(id);
                    BGSprite.spriteName = string.Format("Goods_Source_Tiaozhanfuben_{0}", item.QualityLevel - 1);
                }
            }
            else
            {
                RedPoint.CustomSetActive(false);
            }
        }
    
        public override void Clean()
        {
            curLevelData = null;
        }
    
        public void OnItemClick()
        {
            if(isLock)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceSelectItemCtrl_1719"));
                return;
            }
    
            if (curLevelData != null)
            {
                Hotfix_LT.Messenger.Raise(EventName.LTChallengeInstanceLevelSelect,curLevelData .Level);
            }
        }
    
        /// <summary>
        /// �ؿ��Ѷ�ѡ���¼�
        /// </summary>
        /// <param name="evt"></param>
        private void OnLevelSelect(int level)
        {
            if (curLevelData == null)
            {
                return;
            }
            SelectSprite.gameObject.CustomSetActive(level == curLevelData.Level);
        }
    
        private void OnRewardGet(int level)
        {
            if (curLevelData == null)
            {
                return;
            }

            if (level == curLevelData.Level)
            {
                RedPoint.CustomSetActive(false);
            }
        }
    }
}
