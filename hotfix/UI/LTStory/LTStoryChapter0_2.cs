using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTStoryChapter0_2 : LTStoryChapter
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
            chapterPos[0] = new Vector3(700.0f, -150.0f, 0.0f);
            chapterPos[1] = new Vector3(639.0f, -40.0f, 0.0f);
            chapterPos[2] = new Vector3(200.0f, -50.0f, 0.0f);
            chapterPos[3] = new Vector3(-170.0f, 50.0f, 0.0f);
            chapterPos[4] = new Vector3(0.0f, 0.0f, 0.0f);
            chapterPos[5] = new Vector3(0.0f, 0.0f, 0.0f);

            chapterScale = new float[6];
            chapterScale[0] = 2f;
            chapterScale[1] = 1.5f;
            chapterScale[2] = 1.6f;
            chapterScale[3] = 1.2f;
            chapterScale[4] = 1.4f;
            chapterScale[5] = 1f;

            chapterDuration = new float[6];
            chapterDuration[0] = 0f;
            chapterDuration[1] = 3f;
            chapterDuration[2] = 0f;
            chapterDuration[3] = 3f;
            chapterDuration[4] = 0f;
            chapterDuration[5] = 3f;

            blackMaskDuration = new float[5];
            blackMaskDuration[0] = 1f;
            blackMaskDuration[1] = 0.5f;
            blackMaskDuration[2] = 0.5f;
            blackMaskDuration[3] = 0.5f;
            blackMaskDuration[4] = 0.5f;

            playTime = new float[6];
            playTime[0] = 0.5f;
            playTime[1] = 3f;
            playTime[2] = 3.5f;
            playTime[3] = 6f;
            playTime[4] = 6.5f;
            playTime[5] = 10.5f;

            storyPlayShowTime = new float[3];
            storyPlayShowTime[0] = 0.5f;
            storyPlayShowTime[1] = 3.5f;
            storyPlayShowTime[2] = 6.5f;

            storyShowDuration = 1f;
            storyEndDuration = 0.8f;
            storyMaxShowTime = 1.8f;
            Environment1PosAni = t.GetComponent<TweenPosition>("PanelChapter/Environment/Environment1");
            Environment1ScaleAni = t.GetComponent<TweenScale>("PanelChapter/Environment/Environment1");
            Human1PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human1");
            Human1ScaleAni = t.GetComponent<TweenScale>("PanelChapter/Human/Human1");
            Human2PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human2");
            Human2ScaleAni = t.GetComponent<TweenScale>("PanelChapter/Human/Human2");
            Human3PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human3");
            Human3ScaleAni = t.GetComponent<TweenScale>("PanelChapter/Human/Human3");

            Environment1Scale = new Vector3[2];
            Environment1Scale[0] = new Vector3(1.0f, 1.0f, 1.0f);
            Environment1Scale[1] = new Vector3(1.2f, 1.2f, 1.0f);

            Environment1ScaleDuration = new float[2];
            Environment1ScaleDuration[0] = 0;
            Environment1ScaleDuration[1] = 5;

            Human1Pos = new Vector3[2];
            Human1Pos[0] = new Vector3(-100.0f, -100.0f, 0.0f);
            Human1Pos[1] = new Vector3(0.0f, 0.0f, 0.0f);

            Human1PosDuration = new float[2];
            Human1PosDuration[0] = 0f;
            Human1PosDuration[1] = 3f;

            Human1Scale = new Vector3[2];
            Human1Scale[0] = new Vector3(0.9f, 0.9f, 0.9f);
            Human1Scale[1] = new Vector3(1.0f, 1.0f, 1.0f);

            Human1ScaleDuration = new float[2];
            Human1ScaleDuration[0] = 0f;
            Human1ScaleDuration[1] = 0f;

            Human2Pos = new Vector3[2];
            Human2Pos[0] = new Vector3(0.0f, 0.0f, 0.0f);
            Human2Pos[1] = new Vector3(-145.0f, 75.0f, 0.0f);

            Human2PosDuration = new float[2];
            Human2PosDuration[0] = 0f;
            Human2PosDuration[1] = 3f;

            Human2Scale = new Vector3[1];
            Human2Scale[0] = new Vector3(0.83f, 0.83f, 0.83f);

            Human2ScaleDuration = new float[1];
            Human2ScaleDuration[0] = 0;

            Human3Pos = new Vector3[4];
            Human3Pos[0] = new Vector3(-60.0f, 24.0f, 0.0f);
            Human3Pos[1] = new Vector3(0.0f, 0.0f, 0.0f);
            Human3Pos[2] = new Vector3(95.0f, 0.0f, 0.0f);
            Human3Pos[3] = new Vector3(15.0f, -15.0f, 0.0f);

            Human3PosDuration = new float[4];
            Human3PosDuration[0] = 0f;
            Human3PosDuration[1] = 3.5f;
            Human3PosDuration[2] = 3f;
            Human3PosDuration[3] = 0f;

            Human3Scale = new Vector3[3];
            Human3Scale[0] = new Vector3(1.0f, 1.0f, 1.0f);
            Human3Scale[1] = new Vector3(1.2f, 1.2f, 1.2f);
            Human3Scale[2] = new Vector3(1.3f, 1.3f, 1.3f);

            Human3ScaleDuration = new float[3];
            Human3ScaleDuration[0] = 0f;
            Human3ScaleDuration[1] = 3f;
            Human3ScaleDuration[2] = 0f;
        }

        public TweenPosition Environment1PosAni;
        public TweenScale Environment1ScaleAni;
    
        public TweenPosition Human1PosAni;
        public TweenScale Human1ScaleAni;
    
        public TweenPosition Human2PosAni;
        public TweenScale Human2ScaleAni;
    
        public TweenPosition Human3PosAni;
        public TweenScale Human3ScaleAni;
    
        #region 动画参数
        public Vector3[] Environment1Pos;
        public float[] Environment1PosDuration;
        public Vector3[] Environment1Scale;
        public float[] Environment1ScaleDuration;
    
        public Vector3[] Human1Pos;
        public float[] Human1PosDuration;
        public Vector3[] Human1Scale;
        public float[] Human1ScaleDuration;
    
        public Vector3[] Human2Pos;
        public float[] Human2PosDuration;
        public Vector3[] Human2Scale;
        public float[] Human2ScaleDuration;
    
        public Vector3[] Human3Pos;
        public float[] Human3PosDuration;
        public Vector3[] Human3Scale;
        public float[] Human3ScaleDuration;
        #endregion
    
        protected override IEnumerator StoryAni()
        {
            PlayBgm();
    
            ////0
            //float time0 = GetStopTime(0);
            //yield return new WaitForSeconds(time0);
    
            //skipObj.CustomSetActive(true);
    
            //PlayBlackMaskAlpha(false, blackMaskDuration[0]);
    
            //PlayChapterTweenPos(1);
            //PlayChapterTweenScale(1);
    
            //PlayTweenPos(Human1PosAni, Human1Pos[0], Human1Pos[1], Human1PosDuration[1]);
            //PlayTweenPos(Human2PosAni, Human2Pos[0], Human2Pos[1], Human2PosDuration[1]);
            //PlayTweenScale(Human1ScaleAni, Vector3.one, Human1Scale[0], Human1ScaleDuration[0]);
    
            ////1
            //float time1 = GetStopTime(1);
            //yield return new WaitForSeconds(time1);
    
            //PlayChapterTweenPos(2);
            //PlayChapterTweenScale(2);
    
            ////2
            //float time2 = GetStopTime(2);
            //yield return new WaitForSeconds(time2);
    
            //PlayBlackMaskAlpha(true, blackMaskDuration[1]);
    
            ////3
            //float time3 = GetStopTime(3);
            //yield return new WaitForSeconds(time3);
    
            //PlayBlackMaskAlpha(false, blackMaskDuration[2]);
    
            //PlayChapterTweenPos(4);
            //PlayChapterTweenScale(4);
    
            //PlayTweenPos(Human3PosAni, Human3Pos[0], Human3Pos[1], Human3PosDuration[1]);
    
            //PlayTweenScale(Human2ScaleAni, Vector3.one, Human2Scale[0], Human2ScaleDuration[0]);
    
            ////4
            //float time4 = GetStopTime(4);
            //yield return new WaitForSeconds(time4);
    
            //PlayChapterTweenPos(5);
            //PlayChapterTweenScale(5);
    
            //PlayTweenPos(Human3PosAni, Human3Pos[1], Human3Pos[2], Human3PosDuration[2]);
    
            //PlayTweenScale(Human3ScaleAni, Human3Scale[0], Human3Scale[1], Human3ScaleDuration[1]);
    
            ////5
            //float time5 = GetStopTime(5);
            //yield return new WaitForSeconds(time5);
    
            //PlayBlackMaskAlpha(true, blackMaskDuration[3]);
    
            ////6
            //float time6 = GetStopTime(6);
            //yield return new WaitForSeconds(time6);
    
            //PlayBlackMaskAlpha(false, blackMaskDuration[4]);
    
            //PlayChapterTweenPos(7);
            //PlayChapterTweenScale(7);
    
            //PlayTweenPos(Human1PosAni, Human1Pos[1], Human1Pos[2], Human1PosDuration[2]);
            //PlayTweenPos(Human3PosAni, Human3Pos[2], Human3Pos[3], Human3PosDuration[3]);
    
            //PlayTweenScale(Environment1ScaleAni, Environment1Scale[0], Environment1Scale[1], Environment1ScaleDuration[1]);
            //PlayTweenScale(Human3ScaleAni, Human1Scale[1], Human3Scale[2], Human3ScaleDuration[2]);
    
            ////7
            //float time7 = GetStopTime(7);
            //yield return new WaitForSeconds(time7);
    
            //PlayBlackMaskAlpha(true, blackMaskDuration[5]);
    
            ////8
            //float time8 = GetStopTime(8);
            //yield return new WaitForSeconds(time8);
    
    
            //0
            float time0 = GetStopTime(0);
            yield return new WaitForSeconds(time0);
    
            skipObj.CustomSetActive(true);
    
            PlayBlackMaskAlpha(false, blackMaskDuration[0]);
    
            PlayChapterTweenPos(1);
            PlayChapterTweenScale(1);
    
            if (Human2PosAni != null && Human3PosAni != null)
            {
                Human2PosAni.gameObject.CustomSetActive(false);
                Human3PosAni.gameObject.CustomSetActive(false);
            }

            PlayTweenPos(Human1PosAni, Human1Pos[0], Human1Pos[1], Human1PosDuration[1]);
    
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

            if (Human1PosAni != null && Human2PosAni != null && Human3PosAni != null)
            {
                Human1PosAni.gameObject.CustomSetActive(false);
                Human2PosAni.gameObject.CustomSetActive(true);
                Human3PosAni.gameObject.CustomSetActive(true);
            }
    
            PlayTweenPos(Human2PosAni, Human2Pos[0], Human2Pos[1], Human2PosDuration[1]);
    
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

            if (Human1PosAni != null)
            {
                Human1PosAni.gameObject.CustomSetActive(true);
            }
    
            //5
            float time5 = GetStopTime(5);
            yield return new WaitForSeconds(time5);

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
                //"羽族和猫族伙伴呼唤着苏格拉底，失落之城在天际浮现",
                //"苏格拉底带着拉美西斯脱离战场，与伙伴们会合",
                //"众人眯起眼睛，在冰冷刺骨的空气中接近这片神秘的土地",
    
                EB.Localizer.GetString("ID_STORY_CHAPTER0_2_1"),
                EB.Localizer.GetString("ID_STORY_CHAPTER0_2_2"),
                EB.Localizer.GetString("ID_STORY_CHAPTER0_2_3"),
            };
        }
    }
}
