using System;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class LTDrawTimesGiftController:DynamicMonoHotfix
    {
        private UILabel timesLabel;
        private UISprite foreground;
        private UIProgressBar progressBar;

        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            timesLabel =t.GetComponent<UILabel>("BG/Contain/ProgressBar/NeedTimes");
            foreground = t.GetComponent<UISprite>("BG/Contain/ProgressBar/Foreground");
            progressBar = t.GetComponent<UIProgressBar>("BG/Contain/ProgressBar");
        }
  
        public void SetState()
        {
            int Currentime = LTDrawCardDataManager.Instance.Currentime;
            int everytimeGift = LTDrawCardDataManager.Instance.everytimeGift;
            progressBar.value = (float) ((Currentime*1.0) / everytimeGift);
            if (Currentime>=everytimeGift)
            {
                foreground.spriteName = "Ty_Strip_Green";
            }
            else
            {
                foreground.spriteName = "Ty_Strip_Blue";
            }
        
           timesLabel.text=string.Format("{0}/{1}", Currentime,everytimeGift);
        }
    
    
    
    }
}