using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTInstanceMapHudController : UIControllerHotfix
    {
        public override bool IsFullscreen() { return true; }

        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            controller.TextureCmps["BGTexture"].spriteName = "Copy_Drawing_Di1";
            Slider = t.GetMonoILRComponent<LTSliderBtnList>("Edge/BottomRight/List");
            challengeEntry = t.GetMonoILRComponent<LTInstanceChallengeEntryCtrl>("Edge/ChallengeEntrance");

            controller.backButton = controller.UiButtons["BackButton"];
			controller.BindingBtnEvent(new List<string>() { "ChatButton", "FriendButton", "SwitchBtn" }, new List<EventDelegate>(){new EventDelegate(OnChatBtnClick), new EventDelegate(OnFriendBtnClick),new EventDelegate(() =>OnSwitchBtnClick(t.GetComponent<Transform>("Edge/BottomRight/Btn")))});
			controller.BindingCoolTriggerEvent(new List<string>() { "MoveToLeft", "MoveToRight" }, new List<EventDelegate>(){new EventDelegate(MoveToLeftLand), new EventDelegate(MoveToRightLand)} );
            ChapterObjInstance = controller.GObjects["ChapterObjInstance"];
            LandObjInstance= controller.GObjects["LandObjInstance"];
            PoolTrans = controller.Transforms["PoolTrans"];
            LandContainerTrans = controller.Transforms["LandContainerTrans"];
            LandName = controller.UiLabels["LandName"];
            LandNameTween = LandName.GetComponent<TweenPosition>();

            InitLand();
            InitChapter();

            Hotfix_LT.Messenger.AddListener<System.Action>(Hotfix_LT.EventName.PlayCloudFxEvent, PlayCloudFxFunc);
            curChapterID = null;
        }

        public override void OnDestroy()
        {
            StopAllCoroutines();
            Hotfix_LT.Messenger.RemoveListener<System.Action>(Hotfix_LT.EventName.PlayCloudFxEvent, PlayCloudFxFunc);
            curChapterID = null;
        }

        public static string curChapterID = null;//用于防止多点触碰章节按钮，导致进入章节出错的问题

        private bool mSliderBarOpened = false;

        public LTSliderBtnList Slider;

        public UILabel LandName;
        public TweenPosition LandNameTween;
        
        private List<string> openChapterIds = new List<string>();
        
        public LTInstanceChallengeEntryCtrl challengeEntry;

        private bool fromDropView = false;//当来自伙伴界面进入，不把大地图界面加入堆栈中

        private bool _isEnterStack = false;

        private int clickCount = 0;

        private string newChapterId = string.Empty;
        private string vaildMaxChapter = string.Empty;

        private GameObject currentLand;
        private GameObject preteritLand;
        
        private int currentLandId = 0;
        private int maxLandId = int.MaxValue;

        private Vector3 leftPos = new Vector3(-3180, -733, 0);
        private Vector3 CenterPos = new Vector3(0, -733, 0);
        private Vector3 RightPos = new Vector3(3180, -733, 0);

        private bool isMoving = false;
        
        private Queue<LTInstanceMapChapterCtrl> ChapterQueue = new Queue<LTInstanceMapChapterCtrl>();
        private Queue<GameObject> LandQueue = new Queue<GameObject>();
        private GameObject ChapterObjInstance;
        private GameObject LandObjInstance;
        private Transform PoolTrans;
        private Transform LandContainerTrans;
        
        public override void StartBootFlash()
        {
			SetCurrentPanelAlpha(1);

            if (controller == null) return;

            UITweener[] tweeners = controller.transform.GetComponents<UITweener>();

            if (tweeners == null) return;

            for (int j = 0; j < tweeners.Length; ++j)
			{
				tweeners[j].enabled = true;
				tweeners[j].ResetToBeginning();
                tweeners[j].tweenFactor = 0;
                tweeners[j].PlayForward();
            }
        }
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            if (param != null) fromDropView = true;
            InitChapter();
        }
    
        public override IEnumerator OnAddToStack()
        {
            FusionAudio.PostEvent("MUS/CampaignView/Demo", false);
            if (!fromDropView) GlobalMenuManager.Instance.PushCache("LTInstanceMapHud");
            ShowLand();
            challengeEntry.SetFxState(true);
            yield return base.OnAddToStack();
            UpdateUI();
            Messenger.Raise(Hotfix_LT.EventName.FuncOpenBtnReflash);

            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.challenge, ChallengeRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.resourceexp, ExpRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.resourcegold, GoldRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.mainbox, MaxBoxRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.alienmaze, AlienMazeRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.awaken, AwakenRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.climingtower, ClimingTowerRP);

            yield return null;
            if (!mSliderBarOpened)
            {
                mSliderBarOpened = true;
                Slider.OnStateChange(mSliderBarOpened);
            }           
            yield return null;            
            _isEnterStack = true;
        }

        public override IEnumerator OnRemoveFromStack()
        {
            if (currentLand != null)
            {
                HideLand(currentLand);
                currentLand = null;
            }
            StopAllCoroutines();
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.challenge, ChallengeRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.resourceexp, ExpRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.resourcegold, GoldRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.mainbox, MaxBoxRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.alienmaze, AlienMazeRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.awaken, AwakenRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.climingtower, ClimingTowerRP);
            currentLandId = 0;
            FusionAudio.PostEvent("UI/New/XinZhangJie", false);
            challengeEntry.SetFxState(false);
            DestroySelf();
            _isEnterStack = false;
            yield break;
        }

        public override void OnFocus()
        {
            base.OnFocus();

            if (_isEnterStack) UpdateUI();
            challengeEntry.SetShowInfo();
            SetLandEnv(currentLandId.ToString());
            FusionAudio.PostEvent("UI/New/DiTu", true);
            SetRP();
        }

        public override void OnBlur()
        {
            base.OnBlur();
            controller.HasPlayedTween = false;
            FusionAudio.StopBGM();
        }

        public override void OnCancelButtonClick()
        {
            if (isMoving) return;
            //新手引导特殊处理
            if (GuideNodeManager.IsGuide && !LTInstanceUtil.IsFirstChapterCompleted())
            {
                if (clickCount >= 3)
                {
                    clickCount = 0;
                    MessageTemplateManager.ShowMessage(901099, null, delegate (int result)
                    {
                        if (result == 0)
                        {
                            GuideNodeManager.currentGuideId = 0;
                            GuideNodeManager.GetInstance().JumpGuide();//跳过主线
                        }
                        return;
                    });
                }
                clickCount++;
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString(GuideNodeManager.GUIDE_CANNOT_RETURN));
                return;
            }
            GlobalMenuManager.Instance.RemoveCache("LTInstanceMapHud");
            base.OnCancelButtonClick();
        }

        #region 红点相关

        public void SetRP()
        {
            LTInstanceMapModel.Instance.GetChallengeRP();
            LTInstanceMapModel.Instance.GetBoxRewardRP();
            LTInstanceMapModel.Instance.GetAlienMazeRP();
            LTResourceInstanceManager.Instance.Expedition_Exp();
            LTResourceInstanceManager.Instance.Expedition_Gold();
            LTClimingTowerManager.Instance.Expedition_SleepTower();
            LTAwakeningInstanceManager.Instance.Expedition_Awaken();
        }
        
        private void ChallengeRP(RedPointNode node)
        {
	        controller.GObjects["ChallengeRPObj"].CustomSetActive(node.num > 0);
        }

        private void ExpRP(RedPointNode node)
        {
			controller.GObjects["ExpIIntanceRPObj"].CustomSetActive(node.num > 0);
        }

        private void GoldRP(RedPointNode node)
        {
           controller.GObjects["GoldInstanceRPObj"].CustomSetActive(node.num > 0);
        }

        private void MaxBoxRP(RedPointNode node)
        {

        }

        private void AlienMazeRP(RedPointNode node)
        {
            controller.GObjects["AlienMazeRPObj"].CustomSetActive(node.num > 0);
        }

        private void AwakenRP(RedPointNode node)
        {
            controller.GObjects["AwakenRPObj"].CustomSetActive(node.num > 0);
        }

        private void ClimingTowerRP(RedPointNode node)
        {
            controller.GObjects["SleepTowerRPObj"].CustomSetActive(node.num > 0);
        }

        private void UpdateGotoBtnRP()
        {
            controller.GObjects["MoveToLeftRP"].CustomSetActive(false);
            controller.GObjects["MoveToRightRP"].CustomSetActive(false);
            bool left = false;
            bool right = false;
            var list = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainChapterBoxRewards();
            for (int i = 0; i < list.Count; ++i)
            {
                int land = int.Parse(list[i].LandId);
                if (currentLandId == land) continue;
                if (left && currentLandId > land) continue;
                if (right && currentLandId < land) continue;

                if (LTInstanceUtil.IsChapterComplete(list[i].ForwardChapterId)&& !LTInstanceMapModel.Instance.GetMainChapterRewardState(list[i].Id))
                {
                    if (currentLandId > land)
                    {
                        controller.GObjects["MoveToLeftRP"].CustomSetActive(true);
                        left = true;
                    }
                    if (currentLandId < land)
                    {
                        controller.GObjects["MoveToRightRP"].CustomSetActive(true);
                        right = true;
                    }
                }
            }
        }
        #endregion
        
        //创建已开放章节列表
        private void InitChapter()
        {
            Hashtable chapterData;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("userCampaignStatus.normalChapters", out chapterData);
            openChapterIds.Clear();
            if (!openChapterIds.Contains("101")) openChapterIds.Add("101");
            if (chapterData != null)
            {
                foreach (DictionaryEntry data in chapterData)
                {
                    string chapterId = data.Key.ToString();
                    if (!openChapterIds.Contains(chapterId))
                    {
                        openChapterIds.Add(chapterId);
                    }
                }
            }
            List<Data.LostMainChapterTemplate> temps= Data.SceneTemplateManager.Instance.GetLostMainChapterBoxRewards();
            for(int i = 0; i < temps.Count; ++i)
            {
                if (!openChapterIds.Contains(temps[i].Id))
                {
                    openChapterIds.Add(temps[i].Id);
                }
            }

            string maxChapter = Hotfix_LT.Data.SceneTemplateManager.Instance.GetMaxChapter(openChapterIds);
            Hotfix_LT.Data.LostMainChapterTemplate tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetNextChapter(maxChapter);
            if (tpl != null && LTInstanceUtil.IsChapterComplete(maxChapter))
            {
                openChapterIds.Add(tpl.Id);
                newChapterId = vaildMaxChapter = tpl.Id;
                maxLandId = int.Parse(tpl.LandId);
            }
            else
            {
                newChapterId = string.Empty;
                vaildMaxChapter = maxChapter;
                Hotfix_LT.Data.LostMainChapterTemplate cur = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainChatpterTplById(maxChapter);
                maxLandId = int.Parse(cur.LandId);
            }
        }

        //创建缓存
        private LTInstanceMapChapterCtrl GetItem(Transform parent)
        {
            LTInstanceMapChapterCtrl temp = null;
            if (ChapterQueue != null && ChapterQueue.Count > 0)
            {
                temp = ChapterQueue.Dequeue();
                temp.mDMono.transform.SetParent(parent);
                temp.mDMono.transform.localScale = Vector3.one;
            }
            else
            {
                GameObject obj = GameObject.Instantiate(ChapterObjInstance, parent);
                temp = obj.GetMonoILRComponent<LTInstanceMapChapterCtrl>();
            }
            return temp;
        }
        private void SetItem(LTInstanceMapChapterCtrl temp)
        {
            if (temp == null) return;
            temp.mDMono.transform.SetParent(PoolTrans);
            temp.mDMono.transform.localPosition = Vector3.zero;
            temp.mDMono.transform.localScale = Vector3.one;
            temp.mDMono.gameObject.CustomSetActive(false);
            if (ChapterQueue != null) ChapterQueue.Enqueue(temp);
        }

        private void InitLand(int count = 2)
        {
            for (int i = 0; i < count; ++i)
            {
                GameObject obj = GameObject.Instantiate(LandObjInstance, LandContainerTrans);
                LandQueue.Enqueue(obj);
            }
        }

        private void InitChapter(int count = 13)
        {
            for (int i = 0; i < count; ++i)
            {
                GameObject obj = GameObject.Instantiate(ChapterObjInstance, PoolTrans);
                var temp = obj.GetMonoILRComponent<LTInstanceMapChapterCtrl>();
                ChapterQueue.Enqueue(temp);
            }
        }

        //创建大陆
        private GameObject CreateLand(string landId)
        {
            if (LandQueue.Count == 0)
            {
                EB.Debug.LogWarning("LandQueue.Count not enough!");
                return null;
            }
            Hotfix_LT.Data.LostMainLandTemplate temp = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainLandTemplateByLandId(landId);
            if(temp==null)
            {
                EB.Debug.LogWarning("LandId is error!landId = {0}", landId);
                return null;
            }

            GameObject land= LandQueue.Dequeue();
            land.GetComponent<CampaignTextureCmp>().spriteName = temp.LineName;
            bool initSelect = false;
            for (int i=0;i< temp.ChapterList.Count; ++i)
            {
                bool show = false;
                string dataId = temp.ChapterList[i];
                if (openChapterIds.Contains(dataId))
                {
                    show = true;
                }
                else
                {
                    var boxTemp = Hotfix_LT.Data.SceneTemplateManager.Instance.GetNextChapter(dataId);
                    if (boxTemp != null && boxTemp.IsBoxRewardType())
                    {
                        show = true;
                    }
                }
                if (show)
                {
                    Hotfix_LT.Data.LostMainChapterTemplate tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainChatpterTplById(dataId);
                    if (tpl == null)
                    {
                        continue;
                    }
                    LTInstanceMapChapterCtrl item = GetItem(land.transform);
                    item.Init(tpl);
                    if (!string.IsNullOrEmpty(newChapterId) && tpl.Id.Equals(newChapterId))
                    {
                        newChapterId = string.Empty;
                        item.PlayNewChapterFX();
                    }
                    else
                    {
                        item.PlayNewChapterFX(false);
                    }

                    if (tpl.Id.Equals(vaildMaxChapter))
                    {
                        initSelect = true;
                        controller.Transforms["ArrowTran"].SetParent(item.mDMono.transform);
                    }
                }
            }
            if (!initSelect) controller.Transforms["ArrowTran"].SetParent(PoolTrans);
             land.name = landId;
            return land;
        }

        public void ShowLand()
        {
            if (currentLand == null)
            {
                string max= Hotfix_LT.Data.SceneTemplateManager.Instance.GetMaxChapter(openChapterIds);
                Hotfix_LT.Data.LostMainChapterTemplate tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainChatpterTplById(max);
                currentLandId = int.Parse(tpl.LandId);
                currentLand = CreateLand(tpl.LandId);
                currentLand.transform.SetParent(LandContainerTrans);
                currentLand.transform.localPosition = CenterPos;
                currentLand.CustomSetActive(true);
            }
        }
        
        private void HideLand(GameObject land)
        {
            if(land != null)
            {
                int chlid = land.transform.childCount;
                for (int i = chlid - 1; i >= 0; --i)
                {
                    LTInstanceMapChapterCtrl ctrl= land.transform.GetChild(i).GetMonoILRComponent<LTInstanceMapChapterCtrl>();
                    SetItem(ctrl);
                }
                land.CustomSetActive(false);
                LandQueue.Enqueue(land);
            }
        }
        
        private void UpdateUI()
        {
            UpdateGotoBtnRP();
            if (LTInstanceMapModel.Instance != null && LTInstanceMapModel.Instance.HasCampOverEvent())//如果从主线副本种刚结束上一章节
            {
                ShowNewChapter();
            }
            else
            {
                LTChargeManager.Instance.UpdateLimitedTimeGiftNotice();
            }
        }
    
        public void MoveToRightLand()
        {
            if (currentLandId >= maxLandId || isMoving) return;
            isMoving = true;

            GameObject fx = controller.GObjects[string.Concat("landParticles", currentLandId)];
            fx.CustomSetActive(false);
            LandName.text = string.Empty;
            LandNameTween.PlayReverse();

            preteritLand = currentLand;
            currentLand = CreateLand((currentLandId + 1).ToString());
            if (currentLand != null)
            {
                currentLandId++;
                currentLand.transform.localPosition = RightPos;
                currentLand.CustomSetActive(true);

                TweenPosition preteritLandTP = preteritLand.GetComponent<TweenPosition>();
                TweenPosition currentLandTP = currentLand.GetComponent<TweenPosition>();
                preteritLandTP.from = CenterPos;
                preteritLandTP.to = leftPos;
                preteritLandTP.duration = 0.3f;

                currentLandTP.from = RightPos;
                currentLandTP.to = CenterPos;
                currentLandTP.duration = 0.5f;

                currentLandTP.AddOnFinished(OnMoveLandFinish);
                preteritLandTP.ResetToBeginning();
                preteritLandTP.PlayForward();
                currentLandTP.ResetToBeginning();
                currentLandTP.PlayForward();
            }
            else
            {
                isMoving = false;
                currentLand = preteritLand;
                preteritLand = null;
            }
        }
    
        public void MoveToLeftLand()
        {
            if (currentLandId == 1 || isMoving) return;
            isMoving = true;
            
            GameObject fx = controller.GObjects[string.Concat("landParticles", currentLandId)];
            fx.CustomSetActive(false);
            LandName.text = string .Empty;
            LandNameTween.PlayReverse();

            preteritLand = currentLand;
            currentLand = CreateLand((currentLandId -1).ToString());
            if (currentLand != null)
            {
                currentLandId--;
                currentLand.transform.localPosition = leftPos;
                currentLand.CustomSetActive(true);

                TweenPosition preteritLandTP = preteritLand.GetComponent<TweenPosition>();
                TweenPosition currentLandTP = currentLand.GetComponent<TweenPosition>();
                preteritLandTP.from = CenterPos;
                preteritLandTP.to = RightPos;
                preteritLandTP.duration = 0.3f;

                currentLandTP.from = leftPos;
                currentLandTP.to = CenterPos;
                currentLandTP.duration = 0.5f;

                currentLandTP.AddOnFinished(OnMoveLandFinish);
                preteritLandTP.ResetToBeginning();
                preteritLandTP.PlayForward();
                currentLandTP.ResetToBeginning();
                currentLandTP.PlayForward();
            }
            else
            {
                isMoving = false;
                currentLand = preteritLand;
                preteritLand = null;
            }
        }

        private void OnMoveLandFinish()
        {
            if (preteritLand != null)
            {
                HideLand(preteritLand);
                preteritLand = null;
            }
            UpdateGotoBtnRP();
            SetLandEnv(currentLandId.ToString());

            TweenPosition currentLandTP = currentLand.GetComponent<TweenPosition>();
            currentLandTP.onFinished.Clear();

            isMoving = false;
        }

        private void ShowNewChapter()
        {
            GlobalMenuManager.Instance.Open("LTInstanceNewChapterView");
            LTInstanceMapModel.Instance.SetCampOver(false);//此处手动设为false防止多次触发
        }
    
        #region 设置Land相关
        public void SetLandEnv(string landId)
        {
            SetLandBGM(landId);
            SetLandFX(landId);
            SetLandName(landId);
        }

        private void SetLandBGM(string landId)
        {
            switch (landId)
            {
                case "1":
                    FusionAudio.PostBGMEvent("BGM/DongLuoMa",true);
                    break;
                case "2":
                    FusionAudio.PostBGMEvent("BGM/XiLuoMa", true);
                    break;
                case "3":
                    FusionAudio.PostBGMEvent("BGM/AiJi", true);
                    break;
                case "4":
                    FusionAudio.PostBGMEvent("BGM/BoSi", true);
                    break;
                case "5":
                    FusionAudio.PostBGMEvent("BGM/ManZuZhiDi", true);
                    break;
                case "6":
                    FusionAudio.PostBGMEvent("BGM/RenYuDao", true);
                    break;
            }
            FusionAudio.StartBGM();
        }
        private void SetLandFX(string landId)
        {
            for (int i = 0; i < 6; i++)
            {
                GameObject fx = controller.GObjects[string.Concat("landParticles", i + 1)];
                fx.CustomSetActive((i + 1) == currentLandId);
            }
        }
        private void SetLandName(string landId)
        {
            string landName = EB.Localizer.GetString(string.Format ( "ID_scenes_lost_main_land_{0}_name",landId));
            LandName.text = landName;
            LandNameTween.PlayForward();
        }
        #endregion

        #region Btn方法
        public void OnSwitchBtnClick(Transform btn)
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (!Slider.PlayOver)
            {
                return;
            }
    
            mSliderBarOpened = !mSliderBarOpened;
            if (mSliderBarOpened)
            {
                btn.Rotate(0, 0, -45);
                controller.GObjects["AdditionBtnRP"].CustomSetActive(false);
            }
            else
            {
                btn.Rotate(0, 0, 45);
                controller.GObjects["AdditionBtnRP"].CustomSetActive(SwitchBtnJudge());
            }
            Slider.OnStateChange(mSliderBarOpened);
        }
        private bool SwitchBtnJudge()
        {
            return  controller.GObjects["GoldInstanceRPObj"].activeSelf || controller.GObjects["ExpIIntanceRPObj"].activeSelf|| controller.GObjects["AlienMazeRPObj"].activeSelf;
        }
    
        public void OnChatBtnClick()
        {
            GlobalMenuManager.Instance.Open("ChatHudView", null);
        }
    
        public void OnFriendBtnClick()
        {
            GlobalMenuManager.Instance.Open("FriendHud", null);
        }
        #endregion

        private void PlayCloudFxFunc(System.Action callback)
        {
            if (callback != null)
            {
                EB.Debug.Log("PlayCloudFxFunc======>");
                UIStack.Instance.ShowLoadingScreen(callback, false, true, true);
            }
        }

    }
}
