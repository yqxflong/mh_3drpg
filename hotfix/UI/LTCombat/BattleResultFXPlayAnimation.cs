using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class BattleResultFXPlayAnimation : DynamicMonoHotfix
    {
        public UITweener[] FlagTweeners;
        public UITweener CenterTweener;
        public GameObject WingLeftObject;
        public GameObject WingRightObject;
        public UITweener FontTweener;
        public GameObject BGLight;
        public float FlagTime;
        public float CenterTime;
        public float WingTime;
        public float FontTime;
        public bool PlayOver;
        public GameObject BGFX;
        public GameObject ForegroundFX;
        public float BGFXDelay = 0.8f;
        public float ForegroundFXDelay = 0.8f;
        public float WaitFXPlayTime = 3;
        public bool isNotAwakePlay;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            FlagTweeners = new UITweener[] { t.GetComponent<UITweener>("TitleBG") };
            CenterTweener = t.GetComponent<UITweener>("Jiantou");
            WingLeftObject = t.FindEx("LeftWing").gameObject;
            WingRightObject = t.FindEx("RightWing").gameObject;
            FontTweener = t.GetComponent<UITweener>("Title");
            BGLight = t.FindEx("BGLight").gameObject;
            FlagTime = 0f;
            CenterTime = 0.1f;
            WingTime = 0.2f;
            FontTime = 0.1f;
            PlayOver = false;
            BGFXDelay = 0f;
            ForegroundFXDelay = 0f;
            WaitFXPlayTime = 3f;
            isNotAwakePlay = false;

            if (!isNotAwakePlay)
            {
                StartCoroutine(PlayCoroutine());
            }
        }

        private IEnumerator PlayBGFX()
        {
            if (BGFX == null)
            {
                yield break;
            }

            yield return new WaitForSeconds(BGFXDelay);
            BGFX.gameObject.SetActive(true);
        }

        private IEnumerator PlayForegroundFX()
        {
            if (ForegroundFX == null)
            {
                yield break;
            }

            yield return new WaitForSeconds(ForegroundFXDelay);
            ForegroundFX.gameObject.SetActive(true);
        }

        private IEnumerator PlayCoroutine()
        {
            PlayOver = false;
            StartCoroutine(PlayBGFX());
            StartCoroutine(PlayForegroundFX());

            if (FlagTweeners != null)
            {
                for (var i = 0; i < FlagTweeners.Length; i++)
                {
                    FlagTweeners[i].gameObject.SetActive(false);
                }
            }

            CenterTweener.gameObject.SetActive(false);
            FontTweener.gameObject.SetActive(false);
            WingLeftObject.SetActive(false);
            WingRightObject.SetActive(false);
            BGLight.gameObject.SetActive(false);

            if (BGFX != null)
            {
                BGFX.gameObject.SetActive(false);
            }

            if (ForegroundFX != null)
            {
                ForegroundFX.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(FlagTime);

            if (FlagTweeners != null)
            {
                for (var i = 0; i < FlagTweeners.Length; i++)
                {
                    var t = FlagTweeners[i];
                    t.gameObject.SetActive(true);
                    t.ResetToBeginning();
                    t.PlayForward();
                }
            }

            yield return new WaitForSeconds(CenterTime);
            CenterTweener.gameObject.SetActive(true);
            CenterTweener.ResetToBeginning();
            CenterTweener.PlayForward();

            yield return new WaitForSeconds(WingTime);
            WingLeftObject.SetActive(true);

            var ts = WingLeftObject.GetComponentsInChildren<UITweener>();

            if (ts != null)
            {
                for (var i = 0; i < ts.Length; i++)
                {
                    var t = ts[i];
                    t.ResetToBeginning();
                    t.PlayForward();
                }
            }

            WingRightObject.SetActive(true);

            if (ts != null)
            {
                for (var i = 0; i < ts.Length; i++)
                {
                    var t = ts[i];
                    t.ResetToBeginning();
                    t.PlayForward();
                }
            }

            yield return new WaitForSeconds(FontTime);
            FontTweener.gameObject.SetActive(true);
            FontTweener.ResetToBeginning();
            FontTweener.PlayForward();
            BGLight.gameObject.SetActive(true);

            if (BGFX != null)
            { 
                yield return new WaitForSeconds(WaitFXPlayTime); 
            }

            PlayOver = true;
            Hotfix_LT.Messenger.Raise("OnBattleResultAnimationEnd");
        }

        public void PlayAni()
        {
            StartCoroutine(PlayCoroutine());
        }
    }
}
