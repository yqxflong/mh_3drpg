using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTStoryChapter0_1 : LTStoryChapter
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            panel = t.GetComponent<UIPanel>("PanelChapter");
            chapterAlphaAni = t.GetComponentEx<TweenAlpha>();
            chapterPosAni = t.GetComponentEx<TweenPosition>();
            chapterScaleAni = t.GetComponentEx<TweenScale>();

            chapterPos = new Vector3[6];
            chapterPos[0] = new Vector3(155.0f, -136.0f, 0.0f);
            chapterPos[1] = new Vector3(0.0f, 0.0f, 0.0f);
            chapterPos[2] = new Vector3(-1218.0f, 0.0f, 0.0f);
            chapterPos[3] = new Vector3(-1195.0f, -490.0f, 0.0f);
            chapterPos[4] = new Vector3(44.0f, -457.0f, 0.0f);
            chapterPos[5] = new Vector3(0.0f, 0.0f, 0.0f);

            chapterScale = new float[6];
            chapterScale[0] = 1.2f;
            chapterScale[1] = 1f;
            chapterScale[2] = 2.3f;
            chapterScale[3] = 2.3f;
            chapterScale[4] = 1.6f;
            chapterScale[5] = 1f;

            chapterDuration = new float[6];
            chapterDuration[0] = 0f;
            chapterDuration[1] = 2f;
            chapterDuration[2] = 0f;
            chapterDuration[3] = 2.5f;
            chapterDuration[4] = 0f;
            chapterDuration[5] = 4f;

            blackMaskDuration = new float[5];
            blackMaskDuration[0] = 1f;
            blackMaskDuration[1] = 0.5f;
            blackMaskDuration[2] = 0.5f;
            blackMaskDuration[3] = 0.5f;
            blackMaskDuration[4] = 0.5f;

            playTime = new float[7];
            playTime[0] = 0.5f;
            playTime[1] = 3f;
            playTime[2] = 3.5f;
            playTime[3] = 5.5f;
            playTime[4] = 6f;
            playTime[5] = 8f;
            playTime[6] = 11f;

            storyPlayShowTime = new float[3];
            storyPlayShowTime[0] = 0.5f;
            storyPlayShowTime[1] = 3.5f;
            storyPlayShowTime[2] = 8f;

            storyShowDuration = 0.5f;
            storyEndDuration = 0.5f;
            storyMaxShowTime = 1.5f;

            Human1AlphaAni = t.GetComponent<TweenAlpha>("PanelChapter/Human/Human1");
            Human1CloneAlphaAni = t.GetComponent<TweenAlpha>("PanelChapter/Human/Human1Clone");
            Human2AlphaAni = t.GetComponent<TweenAlpha>("PanelChapter/Human/Human2");
            Human2CloneAlphaAni = t.GetComponent<TweenAlpha>("PanelChapter/Human/Human2Clone");
            Human2PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human2");

            Human2Pos = new Vector3[2];
            Human2Pos[0] = new Vector3(0.0f, -185.0f, 0.0f);
            Human2Pos[1] = new Vector3(0.0f, 0.0f, 0.0f);

            Human2PosDuration = new float[2];
            Human2PosDuration[0] = 0f;
            Human2PosDuration[1] = 2.5f;
        }

        public TweenAlpha Human1AlphaAni;
        public TweenAlpha Human1CloneAlphaAni;
    
        public TweenAlpha Human2AlphaAni;
        public TweenAlpha Human2CloneAlphaAni;
    
        public TweenPosition Human2PosAni;
    
        #region 动画参数
    
        public Vector3[] Human2Pos;
        public float[] Human2PosDuration;
    
        #endregion
        protected override IEnumerator StoryAni()
        {
            PlayBgm();
    
            //0
            float time0 = GetStopTime(0);
            yield return new WaitForSeconds(time0);
    
            skipObj.CustomSetActive(true);
    
            PlayBlackMaskAlpha(false, blackMaskDuration[0]);
    
            PlayChapterTweenPos(1);
            PlayChapterTweenScale(1);
    
            //1
            float time1 = GetStopTime(1);
            yield return new WaitForSeconds(time1);
    
            PlayBlackMaskAlpha(true, blackMaskDuration[1]);
    
            //2
            float time2 = GetStopTime(2);
            yield return new WaitForSeconds(time2);
    
            PlayBlackMaskAlpha(false, blackMaskDuration[2]);
    
            PlayChapterTweenPos(3);
            PlayChapterTweenScale(3);
    
            if (Human2PosAni != null)
            {
                Human2PosAni.gameObject.CustomSetActive(true);
                PlayTweenPos(Human2PosAni, Human2Pos[0], Human2Pos[1], Human2PosDuration[1]);
            }
    
            //3
            float time3 = GetStopTime(3);
            yield return new WaitForSeconds(time3);
    
            PlayBlackMaskAlpha(true, blackMaskDuration[3]);
    
            //4
            float time4 = GetStopTime(4);
            yield return new WaitForSeconds(time4);
    
            PlayBlackMaskAlpha(false, blackMaskDuration[4]);
    
            PlayChapterTweenPos(5);
            PlayChapterTweenScale(5);
    
            //Human1AlphaAni.gameObject.CustomSetActive(true);
    
            //5
            float time5 = GetStopTime(5);
            yield return new WaitForSeconds(time5);
    
            PlayTween(Human1AlphaAni);
            PlayTween(Human1CloneAlphaAni);
    
            PlayTween(Human2AlphaAni);
            PlayTween(Human2CloneAlphaAni);
    
            //PlayBlackMaskAlpha(true, blackMaskDuration[5]);
    
            //6
            float time6 = GetStopTime(6);
            yield return new WaitForSeconds(time6);

            //PlayBlackMaskAlpha(false, blackMaskDuration[6]);

            //PlayChapterTweenPos(7);
            //PlayChapterTweenScale(7);

            //Human2PosAni.gameObject.CustomSetActive(true);
            //PlayTweenPos(Human2PosAni, Human2Pos[0], Human2Pos[1], Human2PosDuration[1]);

            ////7
            //float time7 = GetStopTime(7);
            //yield return new WaitForSeconds(time7);

            //PlayChapterTweenPos(8);
            //PlayChapterTweenScale(8);

            ////8
            //float time8 = GetStopTime(8);
            //yield return new WaitForSeconds(time8);

            //PlayTween(Human2AlphaAni);
            //PlayTween(Human2CloneAlphaAni);

            ////9
            //float time9 = GetStopTime(9);
            //yield return new WaitForSeconds(time9);

            //PlayBlackMaskAlpha(true, blackMaskDuration[7]);

            ////10
            //float time10 = GetStopTime(10);
            //yield return new WaitForSeconds(time10);

            //PlayBlackMaskAlpha(false, blackMaskDuration[8]);

            //PlayChapterTweenPos(10);
            //PlayChapterTweenScale(10);

            ////11
            //float time11 = GetStopTime(11);
            //yield return new WaitForSeconds(time11);

            //PlayTween(Human1AlphaAni);
            //PlayTween(Human1CloneAlphaAni);

            //PlayTween(Human2AlphaAni);
            //PlayTween(Human2CloneAlphaAni);

            ////12
            //float time12 = GetStopTime(12);
            //yield return new WaitForSeconds(time12);

            //PlayBlackMaskAlpha(true, blackMaskDuration[9]);

            ////13
            //float time13 = GetStopTime(13);
            //yield return new WaitForSeconds(time13);

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
                //"拉美西斯受到血腥刺激，挥舞战斧一路屠杀",
                //"苏格拉底被狂暴的拉美西斯深深震慑",
                //"他对拉美西斯施放净化术，避免他误伤自己",
    
                EB.Localizer.GetString("ID_STORY_CHAPTER0_1_1"),
                EB.Localizer.GetString("ID_STORY_CHAPTER0_1_2"),
                EB.Localizer.GetString("ID_STORY_CHAPTER0_1_3"),
            };
        }
    }
}
