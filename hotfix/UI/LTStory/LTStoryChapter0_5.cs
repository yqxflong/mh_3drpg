using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTStoryChapter0_5 : LTStoryChapter
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
            chapterPos[0] = new Vector3(-992.0f, 375.0f, 0.0f);
            chapterPos[1] = new Vector3(176.0f, -581.0f, 0.0f);
            chapterPos[2] = new Vector3(285.0f, -598.0f, 0.0f);
            chapterPos[3] = new Vector3(2948.0f, 81.0f, 0.0f);
            chapterPos[4] = new Vector3(3000.0f, 94.0f, 0.0f);
            chapterPos[5] = new Vector3(0.0f, 0.0f, 0.0f);

            chapterScale = new float[6];
            chapterScale[0] = 2f;
            chapterScale[1] = 2.4f;
            chapterScale[2] = 2.4f;
            chapterScale[3] = 3.75f;
            chapterScale[4] = 4.2f;
            chapterScale[5] = 1f;

            chapterDuration = new float[6];
            chapterDuration[0] = 0f;
            chapterDuration[1] = 5f;
            chapterDuration[2] = 0f;
            chapterDuration[3] = 5f;
            chapterDuration[4] = 0f;
            chapterDuration[5] = 7f;

            blackMaskDuration = new float[6];
            blackMaskDuration[0] = 1f;
            blackMaskDuration[1] = 1.5f;
            blackMaskDuration[2] = 1f;
            blackMaskDuration[3] = 1.5f;
            blackMaskDuration[4] = 1f;
            blackMaskDuration[5] = 1.5f;

            playTime = new float[8];
            playTime[0] = 0.5f;
            playTime[1] = 4f;
            playTime[2] = 6f;
            playTime[3] = 9.5f;
            playTime[4] = 11.5f;
            playTime[5] = 17f;
            playTime[6] = 18.5f;
            playTime[7] = 20.5f;

            storyPlayShowTime = new float[3];
            storyPlayShowTime[0] = 0.5f;
            storyPlayShowTime[1] = 6f;
            storyPlayShowTime[2] = 11.7f;

            storyShowDuration = 1f;
            storyEndDuration = 1f;
            storyMaxShowTime = 2.5f;
            Environment1PosAni = t.GetComponent<TweenPosition>("PanelChapter/Environment/Environment1");
            Environment4PosAni = t.GetComponent<TweenPosition>("PanelChapter/Environment/Environment4");
            Human1PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human1");
            Human2PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human2");
            Human3PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human3");
            Human3ScaleAni = t.GetComponent<TweenScale>("PanelChapter/Human/Human3");
            Human4PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human4");

            Environment1Pos = new Vector3[2];
            Environment1Pos[0] = new Vector3(60.0f, 0.0f, 0.0f);
            Environment1Pos[1] = new Vector3(0.0f, 0.0f, 0.0f);

            Environment1PosDuration = new float[2];
            Environment1PosDuration[0] = 0f;
            Environment1PosDuration[1] = 7f;

            Environment4Pos = new Vector3[1];
            Environment4Pos[0] = new Vector3(75.0f, 0.0f, 0.0f);

            Environment4PosDuration = new float[1];
            Environment4PosDuration[0] = 0f;

            Human1Pos = new Vector3[4];
            Human1Pos[0] = new Vector3(0.0f, 0.0f, 0.0f);
            Human1Pos[1] = new Vector3(91.0f, 8.0f, 0.0f);
            Human1Pos[2] = new Vector3(82.0f, 0.0f, 0.0f);
            Human1Pos[3] = new Vector3(72.0f, 0.0f, 0.0f);

            Human1PosDuration = new float[4];
            Human1PosDuration[0] = 0;
            Human1PosDuration[1] = 5;
            Human1PosDuration[2] = 0;
            Human1PosDuration[3] = 0;

            Human2Pos = new Vector3[6];
            Human2Pos[0] = new Vector3(10.0f, -10.0f, 0.0f);
            Human2Pos[1] = new Vector3(70.0f, -86.0f, 0.0f);
            Human2Pos[2] = new Vector3(0.0f, 0.0f, 0.0f);
            Human2Pos[3] = new Vector3(132.0f, 23.0f, 0.0f);
            Human2Pos[4] = new Vector3(82.0f, 0.0f, 0.0f);
            Human2Pos[5] = new Vector3(72.0f, 0.0f, 0.0f);

            Human2PosDuration = new float[6];
            Human2PosDuration[0] = 0;
            Human2PosDuration[1] = 5;
            Human2PosDuration[2] = 0;
            Human2PosDuration[3] = 5;
            Human2PosDuration[4] = 0;
            Human2PosDuration[5] = 7;

            Human3Pos = new Vector3[6];
            Human3Pos[0] = new Vector3(0.0f, 0.0f, 0.0f);
            Human3Pos[1] = new Vector3(70.0f, -86.0f, 0.0f);
            Human3Pos[2] = new Vector3(0.0f, 0.0f, 0.0f);
            Human3Pos[3] = new Vector3(230.0f, 30.0f, 0.0f);
            Human3Pos[4] = new Vector3(310.0f, 45.0f, 0.0f);
            Human3Pos[5] = new Vector3(133.0f, 0.0f, 0.0f);

            Human3PosDuration = new float[6];
            Human3PosDuration[0] = 0;
            Human3PosDuration[1] = 5;
            Human3PosDuration[2] = 0;
            Human3PosDuration[3] = 5;
            Human3PosDuration[4] = 0;
            Human3PosDuration[5] = 7;

            Human3Scale = new Vector3[2];
            Human3Scale[0] = new Vector3(1.25f, 1.25f, 1.25f);
            Human3Scale[1] = new Vector3(1.0f, 1.0f, 1.0f);

            Human3ScaleDuration = new float[2];
            Human3ScaleDuration[0] = 0;
            Human3ScaleDuration[1] = 7;

            Human4Pos = new Vector3[4];
            Human4Pos[0] = new Vector3(0.0f, 0.0f, 0.0f);
            Human4Pos[1] = new Vector3(70.0f, -86.0f, 0.0f);
            Human4Pos[2] = new Vector3(140.0f, 0.0f, 0.0f);
            Human4Pos[3] = new Vector3(78.0f, 0.0f, 0.0f);

            Human4PosDuration = new float[4];
            Human4PosDuration[0] = 0;
            Human4PosDuration[1] = 5;
            Human4PosDuration[2] = 0;
            Human4PosDuration[3] = 1.5f;
        }

        public TweenPosition Environment1PosAni;
    
        public TweenPosition Environment4PosAni; 
    
        public TweenPosition Human1PosAni;
    
        public TweenPosition Human2PosAni;
    
        public TweenPosition Human3PosAni;
        public TweenScale Human3ScaleAni;
    
        public TweenPosition Human4PosAni;
    
        #region 动画参数
        public Vector3[] Environment1Pos;
        public float[] Environment1PosDuration;
    
        public Vector3[] Environment4Pos;
        public float[] Environment4PosDuration;
    
        public Vector3[] Human1Pos;
        public float[] Human1PosDuration;
    
        public Vector3[] Human2Pos;
        public float[] Human2PosDuration;
    
        public Vector3[] Human3Pos;
        public float[] Human3PosDuration;
        public Vector3[] Human3Scale;
        public float[] Human3ScaleDuration;
    
        public Vector3[] Human4Pos;
        public float[] Human4PosDuration;
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
    
            PlayTweenPos(Human2PosAni, Human2Pos[0], Human2Pos[1], Human2PosDuration[1]);
            PlayTweenPos(Human3PosAni, Human3Pos[0], Human3Pos[1], Human3PosDuration[1]);
            PlayTweenPos(Human4PosAni, Human4Pos[0], Human4Pos[1], Human4PosDuration[1]);
    
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
    
            PlayTweenPos(Human1PosAni, Human1Pos[0], Human1Pos[1], Human1PosDuration[1]);
            PlayTweenPos(Human2PosAni, Human2Pos[2], Human2Pos[3], Human2PosDuration[3]);
            PlayTweenPos(Human3PosAni, Human3Pos[2], Human3Pos[3], Human3PosDuration[3]);
    
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
    
            PlayTweenPos(Environment1PosAni, Environment1Pos[0], Environment1Pos[1], Environment1PosDuration[1]);
            PlayTweenPos(Environment4PosAni, Vector3.zero, Environment4Pos[0], Environment4PosDuration[0]);
            PlayTweenPos(Human1PosAni, Human1Pos[2], Human1Pos[3], Human1PosDuration[3]);
            PlayTweenPos(Human2PosAni, Human2Pos[4], Human2Pos[5], Human2PosDuration[5]);
            PlayTweenPos(Human3PosAni, Human3Pos[4], Human3Pos[5], Human3PosDuration[5]);
    
            PlayTweenScale(Human3ScaleAni, Human3Scale[0], Human3Scale[1], Human3ScaleDuration[1]);
    
            //5
            float time5 = GetStopTime(5);
            yield return new WaitForSeconds(time5);
    
            PlayTweenPos(Human4PosAni, Human4Pos[2], Human4Pos[3], Human4PosDuration[3]);
    
            //6
            float time6 = GetStopTime(6);
            yield return new WaitForSeconds(time6);
    
            PlayBlackMaskAlpha(true, blackMaskDuration[5]);
    
            //7
            float time7 = GetStopTime(7);
            yield return new WaitForSeconds(time7);

            Hotfix_LT.Messenger.Raise(EventName.OnStoryPlaySucc);
        }
    
        protected override void InitStoryTalk()
        {
            storyTalk = new string[]
            {
                //"当日发生的一切，苏格拉底从未对人提起",
                //"后人只是从菲洛克西口中得知",
                //"那是他们一生中，最接近死亡的时刻",
    
                EB.Localizer.GetString("ID_STORY_CHAPTER0_5_1"),
                EB.Localizer.GetString("ID_STORY_CHAPTER0_5_2"),
                EB.Localizer.GetString("ID_STORY_CHAPTER0_5_3"),
            };
        }
    }
}
