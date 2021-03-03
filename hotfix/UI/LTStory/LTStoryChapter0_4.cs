using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTStoryChapter0_4 : LTStoryChapter
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            panel = t.GetComponent<UIPanel>("PanelChapter");
            chapterAlphaAni = t.GetComponentEx<TweenAlpha>();
            chapterPosAni = t.GetComponentEx<TweenPosition>();
            chapterScaleAni = t.GetComponentEx<TweenScale>();

            chapterPos = new Vector3[9];
            chapterPos[0] = new Vector3(986.0f, 1345.0f, 0.0f);
            chapterPos[1] = new Vector3(986.0f, 1385.0f, 0.0f);
            chapterPos[2] = new Vector3(-1061.0f, -1538.0f, 0.0f);
            chapterPos[3] = new Vector3(-497.0f, -1726.0f, 0.0f);
            chapterPos[4] = new Vector3(450.0f, -679.0f, 0.0f);
            chapterPos[5] = new Vector3(672.0f, 1238.0f, 0.0f);
            chapterPos[6] = new Vector3(1895.0f, 96.0f, 0.0f);
            chapterPos[7] = new Vector3(1957.0f, 100.0f, 0.0f);
            chapterPos[8] = new Vector3(0.0f, 0.0f, 0.0f);

            chapterScale = new float[9];
            chapterScale[0] = 3.4f;
            chapterScale[1] = 3.65f;
            chapterScale[2] = 3.4f;
            chapterScale[3] = 3.4f;
            chapterScale[4] = 3.4f;
            chapterScale[5] = 3.4f;
            chapterScale[6] = 3.4f;
            chapterScale[7] = 4.1f;
            chapterScale[8] = 1f;

            chapterDuration = new float[9];
            chapterDuration[0] = 0f;
            chapterDuration[1] = 0.8f;
            chapterDuration[2] = 0.8f;
            chapterDuration[3] = 0.2f;
            chapterDuration[4] = 0.5f;
            chapterDuration[5] = 0f;
            chapterDuration[6] = 4f;
            chapterDuration[7] = 0f;
            chapterDuration[8] = 6.5f;

            blackMaskDuration = new float[6];
            blackMaskDuration[0] = 1f;
            blackMaskDuration[1] = 0.5f;
            blackMaskDuration[2] = 1f;
            blackMaskDuration[3] = 0.5f;
            blackMaskDuration[4] = 1f;
            blackMaskDuration[5] = 1.5f;

            playTime = new float[11];
            playTime[0] = 0.5f;
            playTime[1] = 1.3f;
            playTime[2] = 2.1f;
            playTime[3] = 2.3f;
            playTime[4] = 2.8f;
            playTime[5] = 4f;
            playTime[6] = 7f;
            playTime[7] = 8f;
            playTime[8] = 9f;
            playTime[9] = 14f;
            playTime[10] = 16f;

            storyPlayShowTime = new float[5];
            storyPlayShowTime[0] = 0.5f;
            storyPlayShowTime[1] = 4f;
            storyPlayShowTime[2] = 6.5f;
            storyPlayShowTime[3] = 9f;
            storyPlayShowTime[4] = 12f;

            storyShowDuration = 1f;
            storyEndDuration = 0.8f;
            storyMaxShowTime = 1f;
            Environment1PosAni = t.GetComponent<TweenPosition>("PanelChapter/Environment/Environment1");
            Human1PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human1");
            Human1ScaleAni = t.GetComponent<TweenScale>("PanelChapter/Human/Human1");
            Human3PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human3");
            Human3ScaleAni = t.GetComponent<TweenScale>("PanelChapter/Human/Human3");

            Environment1Pos = new Vector3[4];
            Environment1Pos[0] = new Vector3(-90.0f, -135.0f, 1.0f);
            Environment1Pos[1] = new Vector3(-10.0f, -55.0f, 0.0f);
            Environment1Pos[2] = new Vector3(-80.0f, -30.0f, 0.0f);
            Environment1Pos[3] = new Vector3(0.0f, 0.0f, 0.0f);

            Environment1PosDuration = new float[4];
            Environment1PosDuration[0] = 0f;
            Environment1PosDuration[1] = 4f;
            Environment1PosDuration[2] = 0f;
            Environment1PosDuration[3] = 6.5f;

            Human1Pos = new Vector3[2];
            Human1Pos[0] = new Vector3(5.0f, 5.0f, 0.0f);
            Human1Pos[1] = new Vector3(0.0f, 0.0f, 0.0f);

            Human1PosDuration = new float[2];
            Human1PosDuration[0] = 0f;
            Human1PosDuration[1] = 1f;

            Human1Scale = new Vector3[2];
            Human1Scale[0] = new Vector3(0.85f, 0.85f, 1.0f);
            Human1Scale[1] = new Vector3(1.0f, 1.0f, 1.0f);

            Human1ScaleDuration = new float[2];
            Human1ScaleDuration[0] = 0f;
            Human1ScaleDuration[1] = 6.5f;

            Human3Pos = new Vector3[2];
            Human3Pos[0] = new Vector3(0.0f, -27.0f, 1.0f);
            Human3Pos[1] = new Vector3(0.0f, 0.0f, 1.0f);

            Human3PosDuration = new float[2];
            Human3PosDuration[0] = 0f;
            Human3PosDuration[1] = 0f;

            Human3Scale = new Vector3[2];
            Human3Scale[0] = new Vector3(1.1f, 1.1f, 1.0f);
            Human3Scale[1] = new Vector3(1.0f, 1.0f, 1.0f);

            Human3ScaleDuration = new float[2];
            Human3ScaleDuration[0] = 0f;
            Human3ScaleDuration[1] = 0f;
        }

        public TweenPosition Environment1PosAni;
    
        public TweenPosition Human1PosAni;
        public TweenScale Human1ScaleAni;
    
        public TweenPosition Human3PosAni;
        public TweenScale Human3ScaleAni;
    
        #region 动画参数
        public Vector3[] Environment1Pos;
        public float[] Environment1PosDuration;
    
        public Vector3[] Human1Pos;
        public float[] Human1PosDuration;
        public Vector3[] Human1Scale;
        public float[] Human1ScaleDuration;
    
        public Vector3[] Human3Pos;
        public float[] Human3PosDuration;
        public Vector3[] Human3Scale;
        public float[] Human3ScaleDuration;
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
    
            PlayTweenPos(Environment1PosAni, Vector3.one, Environment1Pos[0], Environment1PosDuration[0]);
            PlayTweenPos(Human3PosAni, Vector3.zero, Human3Pos[0], Human3PosDuration[0]);
    
            PlayTweenScale(Human3ScaleAni, Vector3.one, Human3Scale[0], Human3ScaleDuration[0]);
    
            //1
            float time1 = GetStopTime(1);
            yield return new WaitForSeconds(time1);
    
            PlayChapterTweenPos(2);
            PlayChapterTweenScale(2);
    
            //2
            float time2 = GetStopTime(2);
            yield return new WaitForSeconds(time2);
    
            PlayChapterTweenPos(3);
            PlayChapterTweenScale(3);
    
            //3
            float time3 = GetStopTime(3);
            yield return new WaitForSeconds(time3);
    
            PlayChapterTweenPos(4);
            PlayChapterTweenScale(4);
    
            //4
            float time4 = GetStopTime(4);
            yield return new WaitForSeconds(time4);
    
            PlayBlackMaskAlpha(true, blackMaskDuration[1]);
    
            //5
            float time5 = GetStopTime(5);
            yield return new WaitForSeconds(time5);
    
            PlayBlackMaskAlpha(false, blackMaskDuration[2]);
    
            PlayChapterTweenPos(6);
            PlayChapterTweenScale(6);
    
            PlayTweenPos(Environment1PosAni, Environment1Pos[0], Environment1Pos[1], Environment1PosDuration[1]);
            PlayTweenPos(Human1PosAni, Vector3.zero, Human1Pos[0], Human1PosDuration[0]);
    
            //6
            float time6 = GetStopTime(6);
            yield return new WaitForSeconds(time6);
    
            PlayTweenPos(Human1PosAni, Human1Pos[0], Human1Pos[1], Human1PosDuration[1]);
    
            //7
            float time7 = GetStopTime(7);
            yield return new WaitForSeconds(time7);
    
            PlayBlackMaskAlpha(true, blackMaskDuration[3]);
    
            //8
            float time8 = GetStopTime(8);
            yield return new WaitForSeconds(time8);
    
            PlayBlackMaskAlpha(false, blackMaskDuration[4]);
    
            PlayChapterTweenPos(8);
            PlayChapterTweenScale(8);
    
            PlayTweenPos(Environment1PosAni, Environment1Pos[2], Environment1Pos[3], Environment1PosDuration[3]);
            PlayTweenPos(Human3PosAni, Human3Pos[0], Human3Pos[1], Human3PosDuration[1]);
    
            PlayTweenScale(Human1ScaleAni, Human1Scale[0], Human1Scale[1], Human1ScaleDuration[1]);
            PlayTweenScale(Human3ScaleAni, Human1Scale[0], Human3Scale[1], Human3ScaleDuration[1]);
    
            //9
            float time9 = GetStopTime(9);
            yield return new WaitForSeconds(time9);
    
            PlayBlackMaskAlpha(true, blackMaskDuration[5]);
    
            //10
            float time10 = GetStopTime(10);
            yield return new WaitForSeconds(time10);

            Hotfix_LT.Messenger.Raise(EventName.OnStoryPlaySucc);
        }
    
        protected override void InitStoryTalk()
        {
            storyTalk = new string[]
            {
                //"一切变生肘腋，苏格拉底甚至来不及反应",
                //"他终于明白，“危险武器”指的不是斯妲忒拉的匕首",
                //"更不是她的美貌，而是她的心",
                //"昔日令他赞不绝口，多次拯救他的剑技",
                //"用于伤害他时，依然准确而致命",
    
                EB.Localizer.GetString("ID_STORY_CHAPTER0_4_1"),
                EB.Localizer.GetString("ID_STORY_CHAPTER0_4_2"),
                EB.Localizer.GetString("ID_STORY_CHAPTER0_4_3"),
                EB.Localizer.GetString("ID_STORY_CHAPTER0_4_4"),
                EB.Localizer.GetString("ID_STORY_CHAPTER0_4_5"),
            };
        }
    }
}
