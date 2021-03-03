using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTStoryChapter2_2 : LTStoryChapter
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
            chapterPos[0] = new Vector3(1400.0f, 10.0f, 0.0f);
            chapterPos[1] = new Vector3(1400.0f, 1348.0f, 0.0f);
            chapterPos[2] = new Vector3(-768.0f, 1348.0f, 0.0f);
            chapterPos[3] = new Vector3(-1221.0f, 2854.0f, 0.0f);
            chapterPos[4] = new Vector3(-748.0f, -350.0f, 0.0f);
            chapterPos[5] = new Vector3(0.0f, 0.0f, 0.0f);

            chapterScale = new float[6];
            chapterScale[0] = 2.75f;
            chapterScale[1] = 2.75f;
            chapterScale[2] = 2.75f;
            chapterScale[3] = 4.5f;
            chapterScale[4] = 2.45f;
            chapterScale[5] = 1;

            chapterDuration = new float[6];
            chapterDuration[0] = 0;
            chapterDuration[1] = 3.5f;
            chapterDuration[2] = 4;
            chapterDuration[3] = 3;
            chapterDuration[4] = 0;
            chapterDuration[5] = 4;

            blackMaskDuration = new float[6];
            blackMaskDuration[0] = 1;
            blackMaskDuration[1] = 1.5f;
            blackMaskDuration[2] = 1;
            blackMaskDuration[3] = 2;
            blackMaskDuration[4] = 0.8f;
            blackMaskDuration[5] = 2;

            playTime = new float[12];
            playTime[0] = 0.5f;
            playTime[1] = 1.5f;
            playTime[2] = 5.5f;
            playTime[3] = 7.5f;
            playTime[4] = 9.3f;
            playTime[5] = 13.8f;
            playTime[6] = 17;
            playTime[7] = 19.5f;
            playTime[8] = 21;
            playTime[9] = 22;
            playTime[10] = 27;
            playTime[11] = 29.5f;

            storyPlayShowTime = new float[3];
            storyPlayShowTime[0] = 0.5f;
            storyPlayShowTime[1] = 11;
            storyPlayShowTime[2] = 21;

            storyShowDuration = 1f;
            storyEndDuration = 1f;
            storyMaxShowTime = 4.5f;

            EnviPosAni = t.GetComponent<TweenPosition>("PanelChapter/Environment1");
            Human1PosAni = t.GetComponent<TweenPosition>("PanelChapter/One/Human1");
            HumanOnePosAni = t.GetComponent<TweenPosition>("PanelChapter/One");
            BGPosAni = t.GetComponent<TweenPosition>("PanelChapter/BG");
            BGScaleAni = t.GetComponent<TweenScale>("PanelChapter/BG");
            Human2AlphaAni = t.GetComponent<TweenAlpha>("PanelChapter/One/Human2");
            Human2OtherAlphaAni = t.GetComponent<TweenAlpha>("PanelChapter/One/Human2Clone");
            Envi2 = t.FindEx("PanelChapter/Environment2").gameObject;

            HumanOnePos = new Vector3[4];
            HumanOnePos[0] = new Vector3(0.0f, -60.0f, 0.0f);
            HumanOnePos[1] = new Vector3(0.0f, -15.0f, 0.0f);
            HumanOnePos[2] = new Vector3(10.0f, -35.0f, 0.0f);
            HumanOnePos[3] = new Vector3(0.0f, 0.0f, 0.0f);

            HumanOnePosDuration = new float[4];
            HumanOnePosDuration[0] = 0;
            HumanOnePosDuration[1] = 2.5f;
            HumanOnePosDuration[2] = 11;
            HumanOnePosDuration[3] = 0;

            EnviPos = new Vector3[4];
            EnviPos[0] = new Vector3(0.0f, -240.0f, 0.0f);
            EnviPos[1] = new Vector3(0.0f, 0.0f, 0.0f);
            EnviPos[2] = new Vector3(-210.0f, 0.0f, 0.0f);
            EnviPos[3] = new Vector3(0.0f, 0.0f, 0.0f);

            EnviPosDuration = new float[4];
            EnviPosDuration[0] = 0;
            EnviPosDuration[1] = 5.3f;
            EnviPosDuration[2] = 3;
            EnviPosDuration[3] = 0;

            BGPos = new Vector3[3];
            BGPos[0] = new Vector3(0.0f, 0.0f, 0.0f);
            BGPos[1] = new Vector3(0.0f, 0.0f, 0.0f);
            BGPos[2] = new Vector3(0.0f, 0.0f, 0.0f);

            BGPosDuration = new float[3];
            BGPosDuration[0] = 0;
            BGPosDuration[1] = 0;
            BGPosDuration[2] = 0;

            BGScale = new float[3];
            BGScale[0] = 1.3f;
            BGScale[1] = 1.21f;
            BGScale[2] = 1;

            BGScaleDuration = new float[3];
            BGScaleDuration[0] = 0;
            BGScaleDuration[1] = 0;
            BGScaleDuration[2] = 3.5f;
        }

        public TweenPosition EnviPosAni;
        public TweenPosition Human1PosAni;
        public TweenPosition HumanOnePosAni;
    
        public TweenPosition BGPosAni;
        public TweenScale BGScaleAni;
    
        public TweenAlpha Human2AlphaAni;
        public TweenAlpha Human2OtherAlphaAni;
    
        public GameObject Envi2;
    
        #region 动画参数
        public Vector3[] HumanOnePos;
        public float[] HumanOnePosDuration;
    
        public Vector3[] EnviPos;
        public float[] EnviPosDuration;
    
        public Vector3[] BGPos;
        public float[] BGPosDuration;
        public float[] BGScale;
        public float[] BGScaleDuration;
    
        #endregion
    
        protected override IEnumerator StoryAni()
        {
            PlayBgm();
    
            //0
            float time0 = GetStopTime(0);
            yield return new WaitForSeconds(time0);
    
            skipObj.CustomSetActive(true);
            PlayBlackMaskAlpha(false, blackMaskDuration[0]);
    
            PlayChapterTweenPos(0);
            PlayChapterTweenScale(0);
            PlayTweenScale(BGScaleAni, Vector3.one, BGScale[0] * Vector3.one, BGScaleDuration[0]);
            PlayTweenPos(EnviPosAni, Vector3.zero, EnviPos[0], EnviPosDuration[0]);
            PlayTweenPos(HumanOnePosAni, Vector3.zero, HumanOnePos[0], HumanOnePosDuration[0]);
    
            //1
            float time1 = GetStopTime(1);
            yield return new WaitForSeconds(time1);
    
            PlayChapterTweenPos(1);
            PlayChapterTweenScale(1);
            PlayTweenPos(EnviPosAni, EnviPos[0], EnviPos[1], EnviPosDuration[1]);
            PlayTweenPos(HumanOnePosAni, HumanOnePos[0], HumanOnePos[1], HumanOnePosDuration[1]);
    
            //2
            float time2 = GetStopTime(2);
            yield return new WaitForSeconds(time2);
    
            PlayBlackMaskAlpha(true, blackMaskDuration[1]);
    
            //3
            float time3 = GetStopTime(3);
            yield return new WaitForSeconds(time3);
    
            PlayBlackMaskAlpha(false, blackMaskDuration[2]);
    
            //4
            float time4 = GetStopTime(4);
            yield return new WaitForSeconds(time4);
    
            PlayChapterTweenPos(2);
            PlayChapterTweenScale(2);
            PlayTweenPos(EnviPosAni, EnviPos[1], EnviPos[2], EnviPosDuration[2]);
            PlayTweenPos(HumanOnePosAni, HumanOnePos[1], HumanOnePos[2], HumanOnePosDuration[2]);
    
            //5
            float time5 = GetStopTime(5);
            yield return new WaitForSeconds(time5);
    
            PlayChapterTweenPos(3);
            PlayChapterTweenScale(3);
    
            //6
            float time6 = GetStopTime(6);
            yield return new WaitForSeconds(time6);
    
            PlayBlackMaskAlpha(true, blackMaskDuration[3]);
    
            //7
            float time7 = GetStopTime(7);
            yield return new WaitForSeconds(time7);
    
            PlayChapterTweenPos(4);
            PlayChapterTweenScale(4);
            PlayBlackMaskAlpha(false, blackMaskDuration[4]);
            PlayTweenPos(HumanOnePosAni, HumanOnePos[2], HumanOnePos[3], HumanOnePosDuration[3]);
            PlayTweenPos(EnviPosAni, EnviPos[2], EnviPos[3], EnviPosDuration[3]);
            PlayTweenScale(BGScaleAni, BGScale[0] * Vector3.one, BGScale[1] * Vector3.one, BGScaleDuration[1]);
            Envi2.CustomSetActive(true);
    
            //8
            float time8 = GetStopTime(8);
            yield return new WaitForSeconds(time8);
    
            PlayTween(Human2OtherAlphaAni);
    
            //9
            float time9 = GetStopTime(9);
            yield return new WaitForSeconds(time9);
    
            if (Human2AlphaAni != null)
            {
                Human2AlphaAni.gameObject.CustomSetActive(false);
            }
            
            PlayChapterTweenPos(5);
            PlayChapterTweenScale(5);
            PlayTweenScale(BGScaleAni, BGScale[1] * Vector3.one, BGScale[2] * Vector3.one, BGScaleDuration[2]);
            PlayTween(Human1PosAni);
    
            //10
            float time10 = GetStopTime(10);
            yield return new WaitForSeconds(time10);
    
            PlayBlackMaskAlpha(true, blackMaskDuration[5]);
    
            //11
            float time11 = GetStopTime(11);
            yield return new WaitForSeconds(time11);

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
                //"看着地上散落的机密情报，菲洛克西百感交集",
                //"一只手接近情报，菲洛克西做好了反击的准备",
                //"看到那张熟悉的脸，她的心放了下来",
    
                EB.Localizer.GetString("ID_STORY_CHAPTER2_2_1"),
                EB.Localizer.GetString("ID_STORY_CHAPTER2_2_2"),
                EB.Localizer.GetString("ID_STORY_CHAPTER2_2_3"),
            };
        }
    }
}
