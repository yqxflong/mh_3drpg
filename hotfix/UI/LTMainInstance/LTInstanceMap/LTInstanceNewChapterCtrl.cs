using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTInstanceNewChapterCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            NextChapterFx = t.FindEx("NewChapterFX").gameObject;
            NextChapterSprite = t.GetComponent<UITexture>("Newchapter/NewChapterSprite");
        }
        public GameObject NextChapterFx;
    
        //public UILabel NextChapterLabel;
    
        public UITexture NextChapterSprite;
    
        public override bool IsFullscreen()
        {
            return false;
        }
    
        public override bool ShowUIBlocker
        {
            get
            {
                return false;
            }
        }
    
        private System.Action mCallback;
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            if (param != null)//新手引导主动调用
            {
                mCallback = param as System.Action;
            }
        }
    
        public override IEnumerator OnAddToStack()
        {
            FusionAudio.PostEvent("UI/Map/NewStory");
            NextChapterFx.CustomSetActive(true);
            StartCoroutine(CloseNewChapterFX());
    
    
            yield return base.OnAddToStack();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            if (mCallback != null)
                mCallback();
            DestroySelf();
            yield break;
        }
    
        private float ShowLabelTime = 0.25f;
    
        private float CloseNewChapterTime = 1;
    
        IEnumerator CloseNewChapterFX()
        {
            yield return new WaitForSeconds(ShowLabelTime);
            NextChapterSprite.gameObject.CustomSetActive(true);
            yield return new WaitForSeconds(CloseNewChapterTime);
            NextChapterSprite.gameObject.CustomSetActive(false);
            NextChapterFx.CustomSetActive(false);
            controller.Close();
        }
    }
}
