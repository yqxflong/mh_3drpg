using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTAlienMazeHudController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            DynamicScroll = t.GetMonoILRComponent<LTAlienMazeScroll>("Center/Scroll/PlaceHolder/Grid");
            controller.backButton = t.GetComponent<UIButton>("Edge/TopRight/CancelBtn");
            
            t.GetComponent<UIButton>("Edge/TopLeft/RuleBtn").onClick.Add(new EventDelegate(OnRuleBtnClick));

            t.GetComponent<UIButton>("Edge/TopRight/FriendButton").onClick.Add(new EventDelegate(OnFriendBtnClick));
            t.GetComponent<UIButton>("Edge/TopRight/ChatButton").onClick.Add(new EventDelegate(OnChatBtnClick));
        }

        /// <summary>
        /// 点击聊天界面
        /// </summary>
        public void OnChatBtnClick()
        {
            GlobalMenuManager.Instance.Open("ChatHudView", null);
        }
        /// <summary>
        /// 点击好友界面
        /// </summary>
        public void OnFriendBtnClick()
        {
            GlobalMenuManager.Instance.Open("FriendHud", null);
        }


        public LTAlienMazeScroll DynamicScroll;
        public override bool IsFullscreen(){return true;}
        public override bool ShowUIBlocker { get{return false;}}
    
        public override IEnumerator OnAddToStack()
        {
            FusionAudio.StartBGM();
            yield return base.OnAddToStack();
            LTInstanceMapModel.Instance.RequestChallengeLevelInfo(delegate { OnInfoReady(); }, LTInstanceConfig.AlienMazeTypeStr);
            LTInstanceMapModel.Instance.SetHasMazeState();
            GlobalMenuManager.Instance.PushCache("LTAlienMazeHud");
            Hotfix_LT.Messenger.AddListener<System .Action>(EventName.PlayCloudFXCallback, PlayCloudFxFunc);

        }
        
        public override IEnumerator OnRemoveFromStack()
        {
            Hotfix_LT.Messenger.RemoveListener<System.Action>(EventName.PlayCloudFXCallback, PlayCloudFxFunc);
            FusionAudio.StopBGM();
            if (controller != null)
            {
                DynamicScroll.scrollView.GetComponent<TweenAlpha>().ResetToBeginning();
                DestroySelf();
            }
            StopAllCoroutines();
            CloudFx = null;
            yield break;
        }

        public override void OnCancelButtonClick()
        {
            GlobalMenuManager.Instance.RemoveCache("LTAlienMazeHud");
            base.OnCancelButtonClick();
        }

        private void OnInfoReady()
        {
            var temp = Data.SceneTemplateManager.Instance.GetAllAlienMazeList();
            DynamicScroll.SetItemDatas(temp.ToArray());

            var tweenAlpah = DynamicScroll.scrollView.GetComponent<TweenAlpha>();

            if (tweenAlpah != null)
            {
                tweenAlpah.PlayForward();
            }
        }

        Coroutine CloudFx = null;
        /// <summary>
        /// 开云特效方法
        /// </summary>
        /// <param name="evt"></param>
        private void PlayCloudFxFunc(System.Action Action)
        {
            if (Action != null)
            {
                UIStack.Instance.ShowLoadingScreen(Action, false, true, true);
            }
        }
    
        public void OnRuleBtnClick()
        {
            string text = EB.Localizer.GetString("ID_ALIEN_MAZE_RULE_TEXT");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text);
        }
    }
}
