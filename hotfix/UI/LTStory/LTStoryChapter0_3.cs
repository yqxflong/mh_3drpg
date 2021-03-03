using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTStoryChapter0_3 : LTStoryChapter
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
            chapterPos[0] = new Vector3(-951.0f, 854.0f, 0.0f);
            chapterPos[1] = new Vector3(-820.0f, -920.0f, 0.0f);
            chapterPos[2] = new Vector3(3360.0f, -44.0f, 0.0f);
            chapterPos[3] = new Vector3(752.0f, 1169.0f, 0.0f);
            chapterPos[4] = new Vector3(77.0f, -603.0f, 0.0f);
            chapterPos[5] = new Vector3(0.0f, 0.0f, 0.0f);

            chapterScale = new float[6];
            chapterScale[0] = 2.6f;
            chapterScale[1] = 2.42f;
            chapterScale[2] = 3.6f;
            chapterScale[3] = 4.4f;
            chapterScale[4] = 3.5f;
            chapterScale[5] = 1f;

            chapterDuration = new float[6];
            chapterDuration[0] = 0f;
            chapterDuration[1] = 6.5f;
            chapterDuration[2] = 0f;
            chapterDuration[3] = 7f;
            chapterDuration[4] = 0f;
            chapterDuration[5] = 8f;

            blackMaskDuration = new float[6];
            blackMaskDuration[0] = 1f;
            blackMaskDuration[1] = 1.5f;
            blackMaskDuration[2] = 0.8f;
            blackMaskDuration[3] = 1.5f;
            blackMaskDuration[4] = 2f;
            blackMaskDuration[5] = 2f;

            playTime = new float[7];
            playTime[0] = 0.5f;
            playTime[1] = 5.5f;
            playTime[2] = 7.5f;
            playTime[3] = 13f;
            playTime[4] = 15f;
            playTime[5] = 21.5f;
            playTime[6] = 24f;

            storyPlayShowTime = new float[6];
            storyPlayShowTime[0] = 0.5f;
            storyPlayShowTime[1] = 3.3f;
            storyPlayShowTime[2] = 7.5f;
            storyPlayShowTime[3] = 10.5f;
            storyPlayShowTime[4] = 15f;
            storyPlayShowTime[5] = 19f;

            storyShowDuration = 1f;
            storyEndDuration = 2f;
            storyMaxShowTime = 0.8f;
            Environment1PosAni = t.GetComponent<TweenPosition>("PanelChapter/Environment/Environment1");
            Environment1ScaleAni = t.GetComponent<TweenScale>("PanelChapter/Environment/Environment1");
            Environment2PosAni = t.GetComponent<TweenPosition>("PanelChapter/Environment/Environment2");
            Environment2ScaleAni = t.GetComponent<TweenScale>("PanelChapter/Environment/Environment2");
            Human1PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/HumanOne/Human1");
            Human1ScaleAni = t.GetComponent<TweenScale>("PanelChapter/Human/HumanOne/Human1");
            Human2PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/HumanOne/Human2");
            Human3PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/HumanOne/Human3");
            Human3ScaleAni = t.GetComponent<TweenScale>("PanelChapter/Human/HumanOne/Human3");
            Human4PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human4");
            Human5PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human5");
            Human5ScaleAni = t.GetComponent<TweenScale>("PanelChapter/Human/Human5");
            HumanOnePosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/HumanOne");

            Environment1Pos = new Vector3[2];
            Environment1Pos[0] = new Vector3(0.0f, 0.0f, 0.0f);
            Environment1Pos[1] = new Vector3(0.0f, 0.0f, 0.0f);

            Environment1PosDuration = new float[2];
            Environment1PosDuration[0] = 0f;
            Environment1PosDuration[1] = 0f;

            Environment1Scale = new Vector3[2];
            Environment1Scale[0] = new Vector3(1.2f, 1.2f, 1.0f);
            Environment1Scale[1] = new Vector3(1.0f, 1.0f, 1.0f);

            Environment1ScaleDuration = new float[2];
            Environment1ScaleDuration[0] = 0f;
            Environment1ScaleDuration[1] = 0f;

            Environment2Pos = new Vector3[4];
            Environment2Pos[0] = new Vector3(0.0f, -115.0f, 0.0f);
            Environment2Pos[1] = new Vector3(0.0f, 30.0f, 0.0f);
            Environment2Pos[2] = new Vector3(-85.0f, 36.0f, 0.0f);
            Environment2Pos[3] = new Vector3(0.0f, 0.0f, 0.0f);

            Environment2PosDuration = new float[4];
            Environment2PosDuration[0] = 0f;
            Environment2PosDuration[1] = 6.5f;
            Environment2PosDuration[2] = 0f;
            Environment2PosDuration[3] = 8f;

            Environment2Scale = new Vector3[2];
            Environment2Scale[0] = new Vector3(1.5f, 1.5f, 1.0f);
            Environment2Scale[1] = new Vector3(1.0f, 1.0f, 1.0f);

            Environment2ScaleDuration = new float[2];
            Environment2ScaleDuration[0] = 0f;
            Environment2ScaleDuration[1] = 8f;

            Human1Pos = new Vector3[2];
            Human1Pos[0] = new Vector3(31.0f, 0.0f, 0.0f);
            Human1Pos[1] = new Vector3(50.0f, 0.0f, 0.0f);

            Human1PosDuration = new float[2];
            Human1PosDuration[0] = 0f;
            Human1PosDuration[1] = 0f;

            Human1Scale = new Vector3[2];
            Human1Scale[0] = new Vector3(1.1f, 1.1f, 1.0f);
            Human1Scale[1] = new Vector3(1.0f, 1.0f, 1.0f);

            Human1ScaleDuration = new float[2];
            Human1ScaleDuration[0] = 0f;
            Human1ScaleDuration[1] = 0f;

            Human2Pos = new Vector3[2];
            Human2Pos[0] = new Vector3(50.0f, -2.5f, 0.0f);
            Human2Pos[1] = new Vector3(-58.0f, 0.0f, 0.0f);

            Human2PosDuration = new float[2];
            Human2PosDuration[0] = 0f;
            Human2PosDuration[1] = 0f;

            Human3Pos = new Vector3[2];
            Human3Pos[0] = new Vector3(65.5f, 0.0f, 0.0f);
            Human3Pos[1] = new Vector3(40.0f, 0.0f, 0.0f);

            Human3PosDuration = new float[2];
            Human3PosDuration[0] = 0f;
            Human3PosDuration[1] = 0f;

            Human3Scale = new Vector3[2];
            Human3Scale[0] = new Vector3(1.1f, 1.1f, 1.0f);
            Human3Scale[1] = new Vector3(1.0f, 1.0f, 1.0f);

            Human3ScaleDuration = new float[2];
            Human3ScaleDuration[0] = 0f;
            Human3ScaleDuration[1] = 0f;

            Human4Pos = new Vector3[3];
            Human4Pos[0] = new Vector3(-27.0f, -34.0f, 0.0f);
            Human4Pos[1] = new Vector3(-12.0f, -23.0f, 0.0f);
            Human4Pos[2] = new Vector3(-15.0f, 0.0f, 0.0f);

            Human4PosDuration = new float[3];
            Human4PosDuration[0] = 0f;
            Human4PosDuration[1] = 0f;
            Human4PosDuration[2] = 0f;

            Human5Pos = new Vector3[3];
            Human5Pos[0] = new Vector3(-10.0f, -87.0f, 0.0f);
            Human5Pos[1] = new Vector3(-50.0f, 30.0f, 0.0f);
            Human5Pos[2] = new Vector3(0.0f, 0.0f, 0.0f);

            Human5PosDuration = new float[3];
            Human5PosDuration[0] = 0f;
            Human5PosDuration[1] = 0f;
            Human5PosDuration[2] = 7.5f;

            Human5Scale = new Vector3[2];
            Human5Scale[0] = new Vector3(1.1f, 1.1f, 1.0f);
            Human5Scale[1] = new Vector3(1.0f, 1.0f, 1.0f);

            Human5ScaleDuration = new float[2];
            Human5ScaleDuration[0] = 0f;
            Human5ScaleDuration[1] = 7.5f;

            HumanOnePos = new Vector3[2];
            HumanOnePos[0] = new Vector3(15.0f, 0.0f, 0.0f);
            HumanOnePos[1] = new Vector3(-95.0f, 0.0f, 0.0f);

            HumanOnePosDuration = new float[2];
            HumanOnePosDuration[0] = 0f;
            HumanOnePosDuration[1] = 7f;
        }

        public TweenPosition Environment1PosAni;
        public TweenScale Environment1ScaleAni;
    
        public TweenPosition Environment2PosAni;
        public TweenScale Environment2ScaleAni;
    
        public TweenPosition Human1PosAni;
        public TweenScale Human1ScaleAni;
    
        public TweenPosition Human2PosAni;
    
        public TweenPosition Human3PosAni;
        public TweenScale Human3ScaleAni;
    
        public TweenPosition Human4PosAni;
    
        public TweenPosition Human5PosAni;
        public TweenScale Human5ScaleAni;
    
        public TweenPosition HumanOnePosAni;
    
        #region 动画参数
        public Vector3[] Environment1Pos;
        public float[] Environment1PosDuration;
        public Vector3[] Environment1Scale;
        public float[] Environment1ScaleDuration;
    
        public Vector3[] Environment2Pos;
        public float[] Environment2PosDuration;
        public Vector3[] Environment2Scale;
        public float[] Environment2ScaleDuration;
    
        public Vector3[] Human1Pos;
        public float[] Human1PosDuration;
        public Vector3[] Human1Scale;
        public float[] Human1ScaleDuration;
    
        public Vector3[] Human2Pos;
        public float[] Human2PosDuration;
    
        public Vector3[] Human3Pos;
        public float[] Human3PosDuration;
        public Vector3[] Human3Scale;
        public float[] Human3ScaleDuration;
    
        public Vector3[] Human4Pos;
        public float[] Human4PosDuration;
    
        public Vector3[] Human5Pos;
        public float[] Human5PosDuration;
        public Vector3[] Human5Scale;
        public float[] Human5ScaleDuration;
    
        public Vector3[] HumanOnePos;
        public float[] HumanOnePosDuration;
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
    
            PlayTweenPos(Environment2PosAni, Environment2Pos[0], Environment2Pos[1], Environment2PosDuration[1]);
            PlayTweenPos(Human4PosAni, Vector3.zero, Human4Pos[0], Human4PosDuration[0]);
    
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
    
            PlayTweenPos(Human1PosAni, Vector3.zero, Human1Pos[0], Human1PosDuration[0]);
            PlayTweenPos(Human2PosAni, Vector3.zero, Human2Pos[0], Human2PosDuration[0]);
            PlayTweenPos(Human3PosAni, Vector3.zero, Human3Pos[0], Human3PosDuration[0]);
            PlayTweenPos(Human4PosAni, Human4Pos[0], Human4Pos[1], Human4PosDuration[1]);
            PlayTweenPos(Human5PosAni, Vector3.zero, Human5Pos[0], Human5PosDuration[0]);
            PlayTweenPos(HumanOnePosAni, HumanOnePos[0], HumanOnePos[1], HumanOnePosDuration[1]);
    
            PlayTweenScale(Environment1ScaleAni, Vector3.one, Environment1Scale[0], Environment1ScaleDuration[0]);
            PlayTweenScale(Human1ScaleAni, Vector3.one, Human1Scale[0], Human1ScaleDuration[0]);
            PlayTweenScale(Human3ScaleAni, Vector3.one, Human3Scale[0], Human3ScaleDuration[0]);
    
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
    
            PlayTweenPos(Environment2PosAni, Environment2Pos[2], Environment2Pos[3], Environment2PosDuration[3]); 
            PlayTweenPos(Human1PosAni, Human1Pos[0], Human1Pos[1], Human1PosDuration[1]);
            PlayTweenPos(Human2PosAni, Human2Pos[0], Human2Pos[1], Human2PosDuration[1]);
            PlayTweenPos(Human3PosAni, Human3Pos[0], Human3Pos[1], Human3PosDuration[1]);
            PlayTweenPos(Human4PosAni, Human4Pos[1], Human4Pos[2], Human4PosDuration[2]);
            PlayTweenPos(Human5PosAni, Human5Pos[1], Human5Pos[2], Human5PosDuration[2]);
    
            PlayTweenScale(Environment1ScaleAni, Environment1Scale[0], Environment1Scale[1], Environment1ScaleDuration[1]);
            PlayTweenScale(Environment2ScaleAni, Environment2Scale[0], Environment2Scale[1], Environment2ScaleDuration[1]);
            PlayTweenScale(Human1ScaleAni, Human1Scale[0], Human1Scale[1], Human1ScaleDuration[1]);
            PlayTweenScale(Human3ScaleAni, Human3Scale[0], Human3Scale[1], Human3ScaleDuration[1]);
            PlayTweenScale(Human5ScaleAni, Human5Scale[0], Human5Scale[1], Human5ScaleDuration[1]);
    
            //5
            float time5 = GetStopTime(5);
            yield return new WaitForSeconds(time5);
    
            PlayBlackMaskAlpha(true, blackMaskDuration[5]);
    
            //6
            float time6 = GetStopTime(6);
            yield return new WaitForSeconds(time6);

            Hotfix_LT.Messenger.Raise(EventName.OnStoryPlaySucc);
        }
    
        protected override void InitStoryTalk()
        {
            storyTalk = new string[]
            {
                //"胜利的曙光降临在失落之城",
                //"最后的晶石执行者，两河流域的信仰之源，汉莫拉比倒下了",
                //"在他体内运行的凯撒的意志",
                //"和他坚硬的皮肤一起分崩离析",
                //"然而混乱的终结，不都意味着秩序的开启",
                //"也可能是——越发的混乱",
    
                EB.Localizer.GetString("ID_STORY_CHAPTER0_3_1"),
                EB.Localizer.GetString("ID_STORY_CHAPTER0_3_2"),
                EB.Localizer.GetString("ID_STORY_CHAPTER0_3_3"),
                EB.Localizer.GetString("ID_STORY_CHAPTER0_3_4"),
                EB.Localizer.GetString("ID_STORY_CHAPTER0_3_5"),
                EB.Localizer.GetString("ID_STORY_CHAPTER0_3_6"),
            };
        }
    }
}
