using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTStoryChapter1_2 : LTStoryChapter
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            panel = t.GetComponent<UIPanel>("PanelChapter");
            chapterAlphaAni = t.GetComponentEx<TweenAlpha>();
            chapterPosAni = t.GetComponentEx<TweenPosition>();
            chapterScaleAni = t.GetComponentEx<TweenScale>();

            chapterPos = new Vector3[7];
            chapterPos[0] = new Vector3(-43.0f, 870.0f, 0.0f);
            chapterPos[1] = new Vector3(752.0f, -25.0f, 0.0f);
            chapterPos[2] = new Vector3(971.0f, -153.0f, 0.0f);
            chapterPos[3] = new Vector3(28.0f, -460.0f, 0.0f);
            chapterPos[4] = new Vector3(-9.0f, -526.0f, 0.0f);
            chapterPos[5] = new Vector3(0.0f, -273.0f, 0.0f);
            chapterPos[6] = new Vector3(0.0f, 0.0f, 0.0f);

            chapterScale = new float[7];
            chapterScale[0] = 2.13f;
            chapterScale[1] = 2.15f;
            chapterScale[2] = 3.35f;
            chapterScale[3] = 1.85f;
            chapterScale[4] = 2.05f;
            chapterScale[5] = 1.55f;
            chapterScale[6] = 1;

            chapterDuration = new float[7];
            chapterDuration[0] = 0;
            chapterDuration[1] = 0;
            chapterDuration[2] = 4;
            chapterDuration[3] = 0;
            chapterDuration[4] = 5.5f;
            chapterDuration[5] = 0;
            chapterDuration[6] = 8;

            blackMaskDuration = new float[7];
            blackMaskDuration[0] = 1;
            blackMaskDuration[1] = 1;
            blackMaskDuration[2] = 0.8f;
            blackMaskDuration[3] = 1.5f;
            blackMaskDuration[4] = 0.8f;
            blackMaskDuration[5] = 2;
            blackMaskDuration[6] = 0.8f;

            playTime = new float[10];
            playTime[0] = 0.5f;
            playTime[1] = 3;
            playTime[2] = 4.5f;
            playTime[3] = 7;
            playTime[4] = 9;
            playTime[5] = 11.5f;
            playTime[6] = 13.5f;
            playTime[7] = 14.5f;
            playTime[8] = 17;
            playTime[9] = 26.8f;

            storyPlayShowTime = new float[4];
            storyPlayShowTime[0] = 0.3f;
            storyPlayShowTime[1] = 4.5f;
            storyPlayShowTime[2] = 9;
            storyPlayShowTime[3] = 17;

            storyShowDuration = 1f;
            storyEndDuration = 1f;
            storyMaxShowTime = 2f;

            Human1PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human1");
            Human1AlphaAni = t.GetComponent<TweenAlpha>("PanelChapter/Human/Human1");
            Human1OtherAlphaAni = t.GetComponent<TweenAlpha>("PanelChapter/Human/Human1Clone");
            Human3AlphaAni = t.GetComponent<TweenAlpha>("PanelChapter/Human/Human3");
            Human3OtherAlphaAni = t.GetComponent<TweenAlpha>("PanelChapter/Human/Human3Clone");
        }

        public TweenPosition Human1PosAni;
        public TweenAlpha Human1AlphaAni;
        public TweenAlpha Human1OtherAlphaAni;
    
        public TweenAlpha Human3AlphaAni;
        public TweenAlpha Human3OtherAlphaAni;
    
        protected override IEnumerator StoryAni()
        {
            PlayBgm();
    
            float time1 = GetStopTime(0);
            yield return new WaitForSeconds(time1);
    
            skipObj.CustomSetActive(true);

            if (Human1PosAni != null)
            {
                Human1PosAni.gameObject.CustomSetActive(false);
            }
            
            PlayChapterTweenPos(0);
            PlayChapterTweenScale(0);
            PlayBlackMaskAlpha(false, blackMaskDuration[0]);
    
            float time2 = GetStopTime(1);
            yield return new WaitForSeconds(time2);
            
            PlayBlackMaskAlpha(true, blackMaskDuration[1]);
    
            float time3 = GetStopTime(2);
            yield return new WaitForSeconds(time3);
            
            PlayBlackMaskAlpha(false, blackMaskDuration[2]);
    
            PlayChapterTweenPos(2);
            PlayChapterTweenScale(2);
    
            float time4 = GetStopTime(3);
            yield return new WaitForSeconds(time4);
            
            PlayBlackMaskAlpha(true, blackMaskDuration[3]);
    
            float time5 = GetStopTime(4);
            yield return new WaitForSeconds(time5);
            
            PlayBlackMaskAlpha(false, blackMaskDuration[4]);
            
            PlayChapterTweenPos(4);
            PlayChapterTweenScale(4);

            if (Human1PosAni != null)
            {
                Human1PosAni.gameObject.CustomSetActive(true);
            }

            PlayTween(Human1PosAni);
    
            float time6 = GetStopTime(5);
            yield return new WaitForSeconds(time6);
    
            PlayTween(Human1OtherAlphaAni);
            PlayTween(Human1AlphaAni);
    
            float time7 = GetStopTime(6);
            yield return new WaitForSeconds(time7);
    
            PlayTween(Human3AlphaAni);
            PlayTween(Human3OtherAlphaAni);
    
            float time8 = GetStopTime(7);
            yield return new WaitForSeconds(time8);
            
            PlayBlackMaskAlpha(true, blackMaskDuration[5]);
    
            float time9 = GetStopTime(8);
            yield return new WaitForSeconds(time9);
            
            PlayBlackMaskAlpha(false, blackMaskDuration[6]);        
    
            PlayChapterTweenPos(6);
            PlayChapterTweenScale(6);
    
            float time10 = GetStopTime(9);
            yield return new WaitForSeconds(time10);

            if (mDMono != null)
            {
                mDMono.gameObject.CustomSetActive(false);
            }

            Hotfix_LT.Messenger.Raise(EventName.OnStoryPlaySucc);
        }
    
        protected override void InitStoryTalk()
        {
            base.InitStoryTalk();
            if (isInitStoryTalkSucc)
            {
                return;
            }
    
            storyTalk = new string[]
            {
                //"贝尔特朗摔倒在刚刚挖好的坑里，狼狈不堪",
                //"看到菲洛克西安全飞走，苏格拉底停止了攻势",
                //"随后赶到的阿拉里克怒不可遏，将苏格拉底赶出军队",
                //"看着苏格拉底的身影越来越远，菲洛克西转身飞向圣城",
    
                EB.Localizer.GetString("ID_STORY_CHAPTER1_2_1"),
                EB.Localizer.GetString("ID_STORY_CHAPTER1_2_2"),
                EB.Localizer.GetString("ID_STORY_CHAPTER1_2_3"),
                EB.Localizer.GetString("ID_STORY_CHAPTER1_2_4"),
            };
        }
    }
}
