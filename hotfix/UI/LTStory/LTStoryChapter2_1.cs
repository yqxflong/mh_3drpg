using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTStoryChapter2_1 : LTStoryChapter
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            panel = t.GetComponent<UIPanel>("PanelChapter");
            chapterAlphaAni = t.GetComponentEx<TweenAlpha>();
            chapterPosAni = t.GetComponentEx<TweenPosition>();
            chapterScaleAni = t.GetComponentEx<TweenScale>();

            chapterPos = new Vector3[8];
            chapterPos[0] = new Vector3(3707.0f, 2090.0f, 0.0f);
            chapterPos[1] = new Vector3(2092.0f, 710.0f, 0.0f);
            chapterPos[2] = new Vector3(440.0f, 315.0f, 0.0f);
            chapterPos[3] = new Vector3(-25.0f, 160.0f, 0.0f);
            chapterPos[4] = new Vector3(0.0f, 0.0f, 0.0f);
            chapterPos[5] = new Vector3(-707.0f, -277.0f, 0.0f);
            chapterPos[6] = new Vector3(-1001.0f, -184.0f, 0.0f);
            chapterPos[7] = new Vector3(-2576.0f, 202.0f, 0.0f);

            chapterScale = new float[8];
            chapterScale[0] = 4.1f;
            chapterScale[1] = 2.55f;
            chapterScale[2] = 2.25f;
            chapterScale[3] = 1.35f;
            chapterScale[4] = 1;
            chapterScale[5] = 1.9f;
            chapterScale[6] = 2.5f;
            chapterScale[7] = 3.9f;

            chapterDuration = new float[8];
            chapterDuration[0] = 0;
            chapterDuration[1] = 3.5f;
            chapterDuration[2] = 3.5f;
            chapterDuration[3] = 2.8f;
            chapterDuration[4] = 1.2f;
            chapterDuration[5] = 0;
            chapterDuration[6] = 1.5f;
            chapterDuration[7] = 5;

            blackMaskDuration = new float[6];
            blackMaskDuration[0] = 0.3f;
            blackMaskDuration[1] = 1.2f;
            blackMaskDuration[2] = 0.5f;
            blackMaskDuration[3] = 1;
            blackMaskDuration[4] = 0.5f;
            blackMaskDuration[5] = 2;

            playTime = new float[14];
            playTime[0] = 0.5f;
            playTime[1] = 1.5f;
            playTime[2] = 3.5f;
            playTime[3] = 5.5f;
            playTime[4] = 9;
            playTime[5] = 11.8f;
            playTime[6] = 12;
            playTime[7] = 14;
            playTime[8] = 15;
            playTime[9] = 15.5f;
            playTime[10] = 16.5f;
            playTime[11] = 19;
            playTime[12] = 19.7f;
            playTime[13] = 21.5f;

            storyPlayShowTime = new float[5];
            storyPlayShowTime[0] = 0.5f;
            storyPlayShowTime[1] = 5.5f;
            storyPlayShowTime[2] = 9;
            storyPlayShowTime[3] = 14;
            storyPlayShowTime[4] = 17;

            storyShowDuration = 1f;
            storyEndDuration = 0.8f;
            storyMaxShowTime = 2f;

            Human1PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human1");
            Human1ScaleAni = t.GetComponent<TweenScale>("PanelChapter/Human/Human1");
            Human1AlphaAni = t.GetComponent<TweenAlpha>("PanelChapter/Human/Human1");
            Human1OtherPosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human1Clone");
            Human1OtherAlphaAni = t.GetComponent<TweenAlpha>("PanelChapter/Human/Human1Clone");
            Human2PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human2");
            Human3PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human3");
            Human4PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human4");
            Environment2PosAni = t.GetComponent<TweenPosition>("PanelChapter/Environment/Environment2");

            Human1Pos = new Vector3[5];
            Human1Pos[0] = new Vector3(-30.0f, -17.0f, 0.0f);
            Human1Pos[1] = new Vector3(0.0f, 0.0f, 0.0f);
            Human1Pos[2] = new Vector3(-111.0f, -40.0f, 0.0f);
            Human1Pos[3] = new Vector3(0.0f, 0.0f, 0.0f);
            Human1Pos[4] = new Vector3(-11.0f, -22.0f, 0.0f);

            Human1PosDuration = new float[5];
            Human1PosDuration[0] = 0;
            Human1PosDuration[1] = 4.5f;
            Human1PosDuration[2] = 0;
            Human1PosDuration[3] = 7.5f;
            Human1PosDuration[4] = 0;

            Human1Scale = new float[2];
            Human1Scale[0] = 1.2f;
            Human1Scale[1] = 1;

            Human1ScaleDuration = new float[2];
            Human1ScaleDuration[0] = 0;
            Human1ScaleDuration[1] = 7.5f;

            Human2Pos = new Vector3[4];
            Human2Pos[0] = new Vector3(-47.0f, -11.0f, 0.0f);
            Human2Pos[1] = new Vector3(10.0f, 0.0f, 0.0f);
            Human2Pos[2] = new Vector3(-97.0f, -23.0f, 0.0f);
            Human2Pos[3] = new Vector3(0.0f, 0.0f, 0.0f);

            Human2PosDuration = new float[4];
            Human2PosDuration[0] = 0;
            Human2PosDuration[1] = 4.5f;
            Human2PosDuration[2] = 0;
            Human2PosDuration[3] = 7.5f;

            Human3Pos = new Vector3[5];
            Human3Pos[0] = new Vector3(-15.0f, 0.0f, 0.0f);
            Human3Pos[1] = new Vector3(20.0f, 0.0f, 0.0f);
            Human3Pos[2] = new Vector3(-77.0f, -39.0f, 0.0f);
            Human3Pos[3] = new Vector3(0.0f, 0.0f, 0.0f);
            Human3Pos[4] = new Vector3(0.0f, -15.0f, 0.0f);

            Human3PosDuration = new float[5];
            Human3PosDuration[0] = 0;
            Human3PosDuration[1] = 4.5f;
            Human3PosDuration[2] = 0;
            Human3PosDuration[3] = 7.5f;
            Human3PosDuration[4] = 0;

            Human4Pos = new Vector3[3];
            Human4Pos[0] = new Vector3(470.0f, 0.0f, 0.0f);
            Human4Pos[1] = new Vector3(-30.0f, -5.0f, 0.0f);
            Human4Pos[2] = new Vector3(-20.0f, -5.0f, 0.0f);

            Human4PosDuration = new float[3];
            Human4PosDuration[0] = 0;
            Human4PosDuration[1] = 3.2f;
            Human4PosDuration[2] = 1.5f;
        }

        public TweenPosition Human1PosAni;
        public TweenScale Human1ScaleAni;
        public TweenAlpha Human1AlphaAni;
        public TweenPosition Human1OtherPosAni;
        public TweenAlpha Human1OtherAlphaAni;
    
        public TweenPosition Human2PosAni;
        public TweenPosition Human3PosAni;
        public TweenPosition Human4PosAni;
        public TweenPosition Environment2PosAni;
    
        #region 动画参数
        public Vector3[] Human1Pos;
        public float[] Human1PosDuration;
        public float[] Human1Scale;
        public float[] Human1ScaleDuration;
    
        public Vector3[] Human2Pos;
        public float[] Human2PosDuration;
    
        public Vector3[] Human3Pos;
        public float[] Human3PosDuration;
    
        public Vector3[] Human4Pos;
        public float[] Human4PosDuration;
        #endregion
    
    
        protected override IEnumerator StoryAni()
        {
            PlayBgm();
    
            //0 镜头淡入，移动镜头，Human2开始1号平移动画
            float time0 = GetStopTime(0);
            yield return new WaitForSeconds(time0);
    
            skipObj.CustomSetActive(true);
            PlayBlackMaskAlpha(false, blackMaskDuration[0]);
            
            PlayChapterTweenPos(1);
            PlayChapterTweenScale(1);
            
            PlayTweenPos(Human2PosAni, Human2Pos[0], Human2Pos[1], Human2PosDuration[1]);
    
            //1 
            float time1 = GetStopTime(1);
            yield return new WaitForSeconds(time1);
    
            PlayTweenPos(Human1PosAni, Human1Pos[0], Human1Pos[1], Human1PosDuration[1]);
            PlayTweenPos(Human3PosAni, Human3Pos[0], Human3Pos[1], Human3PosDuration[1]);
    
            //2
            float time2 = GetStopTime(2);
            yield return new WaitForSeconds(time2);
    
            PlayBlackMaskAlpha(true, blackMaskDuration[1]);
    
            //3
            float time3 = GetStopTime(3);
            yield return new WaitForSeconds(time3);
    
            PlayBlackMaskAlpha(false, blackMaskDuration[2]);
    
            PlayChapterTweenPos(2);
            PlayChapterTweenScale(2);
    
            PlayTweenPos(Human1PosAni, Human1Pos[2], Human1Pos[3], Human1PosDuration[3]);
            PlayTweenPos(Human2PosAni, Human2Pos[2], Human2Pos[3], Human2PosDuration[3]);
            PlayTweenPos(Human3PosAni, Human3Pos[2], Human3Pos[3], Human3PosDuration[3]);
    
            PlayTweenScale(Human1ScaleAni, Human1Scale[0] * Vector3.one, Human1Scale[1] * Vector3.one, Human1ScaleDuration[1]);
    
            PlayTween(Environment2PosAni);
    
            //4
            float time4 = GetStopTime(4);
            yield return new WaitForSeconds(time4);
    
            PlayChapterTweenPos(3);
            PlayChapterTweenScale(3);
    
            //5
            float time5 = GetStopTime(5);
            yield return new WaitForSeconds(time5);
    
            PlayChapterTweenPos(4);
            PlayChapterTweenScale(4);
    
            //6
            float time6 = GetStopTime(6);
            yield return new WaitForSeconds(time6);
    
            PlayBlackMaskAlpha(true, blackMaskDuration[3]);
    
            //7
            float time7 = GetStopTime(7);
            yield return new WaitForSeconds(time7);
    
            PlayBlackMaskAlpha(false, blackMaskDuration[4]);
    
            PlayChapterTweenPos(6);
            PlayChapterTweenScale(6);
    
            PlayTweenPos(Human1PosAni, Human1Pos[3], Human1Pos[4], Human1PosDuration[4]);
            PlayTweenPos(Human3PosAni, Human3Pos[3], Human3Pos[4], Human3PosDuration[4]);
    
            //8
            float time8 = GetStopTime(8);
            yield return new WaitForSeconds(time8);
    
            PlayTween(Human1AlphaAni);
            PlayTween(Human1OtherAlphaAni);
    
            //9
            float time9 = GetStopTime(9);
            yield return new WaitForSeconds(time9);
    
            PlayChapterTweenPos(7);
            PlayChapterTweenScale(7);
    
            //10
            float time10 = GetStopTime(10);
            yield return new WaitForSeconds(time10);

            if (Human4PosAni != null)
            {
                Human4PosAni.gameObject.CustomSetActive(true);
            }

            PlayTween(Human1OtherPosAni);
            PlayTweenPos(Human4PosAni, Human4Pos[0], Human4Pos[1], Human4PosDuration[1]);
    
            //11
            float time11 = GetStopTime(11);
            yield return new WaitForSeconds(time11);
    
            PlayBlackMaskAlpha(true, blackMaskDuration[5]);
    
            //12
            float time12 = GetStopTime(12);
            yield return new WaitForSeconds(time12);
    
            PlayTweenPos(Human4PosAni, Human4Pos[1], Human4Pos[2], Human4PosDuration[2]);
    
            //13
            float time13 = GetStopTime(13);
            yield return new WaitForSeconds(time13);

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
                //"西省的士兵不屑去做消防员",
                //"他们更愿意做纵火犯",
                //"他们的首领阿拉里克没有一丝怜悯",
                //"他不为征服而来，只想毁灭一切",
                //"任何人都无法熄灭他满腔的怒火",
    
                EB.Localizer.GetString("ID_STORY_CHAPTER2_1_1"),
                EB.Localizer.GetString("ID_STORY_CHAPTER2_1_2"),
                EB.Localizer.GetString("ID_STORY_CHAPTER2_1_3"),
                EB.Localizer.GetString("ID_STORY_CHAPTER2_1_4"),
                EB.Localizer.GetString("ID_STORY_CHAPTER2_1_5"),
            };
        }
    }
}
