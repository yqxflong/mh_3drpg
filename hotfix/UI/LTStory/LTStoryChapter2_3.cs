using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTStoryChapter2_3 : LTStoryChapter
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            panel = t.GetComponent<UIPanel>("PanelChapter");
            chapterAlphaAni = t.GetComponentEx<TweenAlpha>();
            chapterPosAni = t.GetComponentEx<TweenPosition>();
            chapterScaleAni = t.GetComponentEx<TweenScale>();

            chapterPos = new Vector3[3];
            chapterPos[0] = new Vector3(1432.0f, 1516.0f, 0.0f);
            chapterPos[1] = new Vector3(1108.0f, 65.0f, 0.0f);
            chapterPos[2] = new Vector3(0.0f, 0.0f, 0.0f);

            chapterScale = new float[3];
            chapterScale[0] = 3.5f;
            chapterScale[1] = 2.2f;
            chapterScale[2] = 1;

            chapterDuration = new float[3];
            chapterDuration[0] = 0;
            chapterDuration[1] = 6.5f;
            chapterDuration[2] = 5.5f;

            blackMaskDuration = new float[4];
            blackMaskDuration[0] = 1;
            blackMaskDuration[1] = 2;
            blackMaskDuration[2] = 1;
            blackMaskDuration[3] = 2;

            playTime = new float[10];
            playTime[0] = 0.5f;
            playTime[1] = 4;
            playTime[2] = 5;
            playTime[3] = 7.5f;
            playTime[4] = 8.5f;
            playTime[5] = 9;
            playTime[6] = 10;
            playTime[7] = 10.5f;
            playTime[8] = 12.5f;
            playTime[9] = 15;

            storyPlayShowTime = new float[4];
            storyPlayShowTime[0] = 0.5f;
            storyPlayShowTime[1] = 7.5f;
            storyPlayShowTime[2] = 10.5f;
            storyPlayShowTime[3] = 14.3f;

            storyShowDuration = 1f;
            storyEndDuration = 1f;
            storyMaxShowTime = 3.5f;

            Envi1PosAni = t.GetComponent<TweenPosition>("PanelChapter/Environment/Environment1");
            Human4PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human4");
            Human5PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human5");
            Human6PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human6");
            Envi2ScaleAni = t.GetComponent<TweenScale>("PanelChapter/Environment/Environment2");
            Envi3ScaleAni = t.GetComponent<TweenScale>("PanelChapter/Environment/Environment3");
            HumanOneScaleAni = t.GetComponent<TweenScale>("PanelChapter/Human/HumanOne");
            Human3AlphaAni = t.GetComponent<TweenAlpha>("PanelChapter/Human/Human3");
            Human3OtherAlphaAni = t.GetComponent<TweenAlpha>("PanelChapter/Human/Human3Clone");
            Envi4 = t.FindEx("PanelChapter/Environment/Environment4").gameObject;

            Envi2Scale = new float[5];
            Envi2Scale[0] = 1.17f;
            Envi2Scale[1] = 1.5f;
            Envi2Scale[2] = 1.4f;
            Envi2Scale[3] = 1.2f;
            Envi2Scale[4] = 1;

            Envi2ScaleDuration = new float[5];
            Envi2ScaleDuration[0] = 0;
            Envi2ScaleDuration[1] = 0;
            Envi2ScaleDuration[2] = 1;
            Envi2ScaleDuration[3] = 1.5f;
            Envi2ScaleDuration[4] = 3;

            Envi3Scale = new float[4];
            Envi3Scale[0] = 1.2f;
            Envi3Scale[1] = 1;
            Envi3Scale[2] = 1.3f;
            Envi3Scale[3] = 1;

            Envi3ScaleDuration = new float[4];
            Envi3ScaleDuration[0] = 0;
            Envi3ScaleDuration[1] = 3;
            Envi3ScaleDuration[2] = 0;
            Envi3ScaleDuration[3] = 5.5f;

            HumanOneScale = new float[2];
            HumanOneScale[0] = 1.4f;
            HumanOneScale[1] = 1;

            HumanOneScaleDuration = new float[2];
            HumanOneScaleDuration[0] = 0;
            HumanOneScaleDuration[1] = 5.5f;
        }

        public TweenPosition Envi1PosAni;
        public TweenPosition Human4PosAni;
        public TweenPosition Human5PosAni;
        public TweenPosition Human6PosAni;
        
        public TweenScale Envi2ScaleAni;
        public TweenScale Envi3ScaleAni;
        public TweenScale HumanOneScaleAni;
    
        public TweenAlpha Human3AlphaAni;
        public TweenAlpha Human3OtherAlphaAni;
    
        public GameObject Envi4;
    
        #region 动画参数
    
        public float[] Envi2Scale;
        public float[] Envi2ScaleDuration;
    
        public float[] Envi3Scale;
        public float[] Envi3ScaleDuration;
    
        public float[] HumanOneScale;
        public float[] HumanOneScaleDuration;
    
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
            PlayTween(Envi1PosAni);
            PlayTweenScale(HumanOneScaleAni, Vector3.one, HumanOneScale[0] * Vector3.one, HumanOneScaleDuration[0]);
            PlayTweenScale(Envi3ScaleAni, Vector3.one, Envi3Scale[0] * Vector3.one, Envi3ScaleDuration[0]);
            PlayTweenScale(Envi2ScaleAni, Vector3.one, Envi2Scale[0] * Vector3.one, Envi2ScaleDuration[0]);
    
            //1
            float time1 = GetStopTime(1);
            yield return new WaitForSeconds(time1);
    
            PlayTweenScale(Envi3ScaleAni, Envi3Scale[0] * Vector3.one, Envi3Scale[1] * Vector3.one, Envi3ScaleDuration[1]);
    
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
            PlayTweenScale(HumanOneScaleAni, HumanOneScale[0] * Vector3.one, HumanOneScale[1] * Vector3.one, HumanOneScaleDuration[1]);
            PlayTweenScale(Envi3ScaleAni, Envi3Scale[2] * Vector3.one, Envi3Scale[3] * Vector3.one, Envi3ScaleDuration[3]);
            PlayTweenScale(Envi2ScaleAni, Envi2Scale[1] * Vector3.one, Envi2Scale[2] * Vector3.one, Envi2ScaleDuration[2]);
    
    
            //4
            float time4 = GetStopTime(4);
            yield return new WaitForSeconds(time4);
    
            PlayTweenScale(Envi2ScaleAni, Envi2Scale[2] * Vector3.one, Envi2Scale[3] * Vector3.one, Envi2ScaleDuration[3]);
    
            //5
            float time5 = GetStopTime(5);
            yield return new WaitForSeconds(time5);

            if (Human5PosAni != null)
            {
                Human5PosAni.gameObject.CustomSetActive(true);
            }
            
            PlayTween(Human5PosAni);
    
            //6
            float time6 = GetStopTime(6);
            yield return new WaitForSeconds(time6);

            if (Human4PosAni != null && Human6PosAni != null)
            {
                Human4PosAni.gameObject.CustomSetActive(true);
                Human6PosAni.gameObject.CustomSetActive(true);
            }
            
            PlayTween(Human4PosAni);
            PlayTween(Human6PosAni);
            PlayTweenScale(Envi2ScaleAni, Envi2Scale[3] * Vector3.one, Envi2Scale[4] * Vector3.one, Envi2ScaleDuration[4]);
    
            //7
            float time7 = GetStopTime(7);
            yield return new WaitForSeconds(time7);
    
            PlayTween(Human3OtherAlphaAni);
    
            //8
            float time8 = GetStopTime(8);
            yield return new WaitForSeconds(time8);
    
            PlayBlackMaskAlpha(true, blackMaskDuration[3]);
    
            //9
            float time9 = GetStopTime(9);
            yield return new WaitForSeconds(time9);

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
                //"眼前这个书呆子的实力，令阿拉里克刮目相看",
                //"他收起对苏格拉底的蔑视，准备全力以赴",
                //"士兵们也围了上来，准备享受杀戮的盛宴",
                //"",
    
                EB.Localizer.GetString("ID_STORY_CHAPTER2_3_1"),
                EB.Localizer.GetString("ID_STORY_CHAPTER2_3_2"),
                EB.Localizer.GetString("ID_STORY_CHAPTER2_3_3"),
                "",//空字段，用于控制结束时间
            };
        }
    
    }
}
