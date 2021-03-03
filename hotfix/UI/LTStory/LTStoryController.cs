using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTStoryController : UIControllerHotfix
    {
        public UIPanel panel;
        //public TweenAlpha whiteMask;
        public TweenAlpha blackMask;
        public UISprite blackSp;
        public TweenAlpha talkAni;
        public UILabel talkLab;
        public UILabel talkShadowLab;
        public GameObject skipObj;
        
        private LTStoryChapter curChapter;
        private System.Action mStoryAction;
        private List<UITexture> mTextureAll;
        private object mParam;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            panel = t.GetComponentEx<UIPanel>();
            blackMask = t.GetComponent<TweenAlpha>("Generally/BlackMask");
            blackSp = t.GetComponent<UISprite>("Generally/BlackMask");
            talkAni = t.GetComponent<TweenAlpha>("Generally/ContentLab");
            talkLab = t.GetComponent<UILabel>("Generally/ContentLab");
            talkShadowLab = t.GetComponent<UILabel>("Generally/ContentLab/ContentLab (1)");

            var skipBtn = t.GetComponent<UIButton>("Generally/SkipBtn");
            skipBtn.onClick.Add(new EventDelegate(OnSkipBtnClick));

            skipObj = skipBtn.gameObject;
            controller.backButton = t.GetComponent<UIButton>("Generally/NullBtn");

            Hotfix_LT.Messenger.AddListener(EventName.OnStoryPlaySucc, OnStoryPlaySuccEvent);
        }

        public override bool IsFullscreen()
        {
            return true;
        }

        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener(EventName.OnStoryPlaySucc, OnStoryPlaySuccEvent);
            base.OnDestroy();
        }
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            mParam = param;
        }
    
        public override IEnumerator OnAddToStack()
        {
            return base.OnAddToStack();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
    
        public override void OnFocus()
        {
            base.OnFocus();
    
            string storyName = "Chapter1_1";
            mStoryAction = null;

            if (mParam != null && !string.IsNullOrEmpty(mParam.ToString()))
            {
                string[] storys = mParam.ToString().Split(',');
                if (storys.Length > 1)
                {
                    string tempStoryName = mParam.ToString().Substring(storys[0].Length + 1);
                    mStoryAction = () => { mParam = tempStoryName; OnFocus();};
                }
                storyName = storys[0];
            }
    
            talkAni.enabled = false;
            blackMask.enabled = false;
            talkLab.alpha = 0;
            blackSp.alpha = 1;
            StartCoroutine(LoadStory(storyName));
        }
        
        private IEnumerator LoadStory(string storyName)
        {
            GM.AssetLoader<GameObject> obj = new GM.AssetLoader<GameObject>(storyName, controller.gameObject);
            yield return obj;
            if (obj!=null&& obj.Success)
            {
                curChapter = obj.Instance.GetMonoILRComponent<LTStoryChapter>();
                if (curChapter == null)
                {
                    OnStoryPlaySuccEvent();
                    EB.Debug.LogError("LTStroyChapter is Null, StoryName:{0}", storyName);
                    yield break;
                }
                
                curChapter.mDMono.name = storyName;
                curChapter.blackMask = blackMask;
                curChapter.talkAni = talkAni;
                curChapter.talkLab = talkLab;
                curChapter.talkShadowLab = talkShadowLab;
                curChapter.skipObj = skipObj;
    
                curChapter.mDMono.transform.SetParent(controller.transform);
                curChapter.mDMono.transform.localPosition = Vector3.zero;
                curChapter.mDMono.transform.localEulerAngles = Vector3.zero;
                curChapter.mDMono.transform.localScale = Vector3.one;
    
                curChapter.panel.depth = panel.depth + 1;
                curChapter.panel.sortingOrder = panel.sortingOrder + 1;
    
                SetTextureAll();
    
                yield return null;
    
                curChapter.PlayAni();
            }
            else
            {
                OnStoryPlaySuccEvent();
                EB.Debug.LogError("LTStroy is Null, StoryName:{0}" ,storyName);
            }
        }
    
        private void OnStoryPlaySuccEvent()
        {
            if (curChapter != null)
            {
                curChapter.StopBgm();
            }
    
            if (mStoryAction != null)
            {
                if (curChapter != null)
                {
                    string name = curChapter.mDMono.name;
                    curChapter.mDMono.gameObject.CustomSetActive(false);
                    skipObj.CustomSetActive(false);
                    Object.Destroy(curChapter.mDMono.gameObject);
                    EB.Assets.UnloadAssetByName(curChapter.mDMono.name, true);
                    curChapter = null;
                }
                mStoryAction();
    
                return;
            }
    
            if (StoryAction != null)
            {
                StoryAction();
                StoryAction = null;
            }

            if (curChapter != null)
            {
                Object.Destroy(curChapter.mDMono.gameObject);
                EB.Assets.UnloadAssetByName(curChapter.mDMono.name, true);
                curChapter = null;
            }

            UnloadAsset();
            controller.Close();
        }
    
        private void SetTextureAll()
        {
            if (mTextureAll == null)
            {
                mTextureAll = new List<UITexture>();
            }
    
            SetTexture(curChapter.mDMono.transform);
        }
    
        private void SetTexture(Transform tf)
        {
            if (tf.childCount > 0)
            {
                for (int i = 0; i < tf.childCount; i++)
                {
                    UITexture tex = tf.GetChild(i).GetComponent<UITexture>();

                    if (tex != null)
                    {
                        mTextureAll.Add(tex);
                    }

                    SetTexture(tf.GetChild(i));
                }
            }
        }
    
        private void UnloadAsset()
        {
            if (mTextureAll == null)
            {
                return;
            }
    
            for (int i = 0; i < mTextureAll.Count; i++)
            {
                if (mTextureAll[i] != null)
                {
                    if (mTextureAll[i].mainTexture != null)
                    {
                        Resources.UnloadAsset(mTextureAll[i].mainTexture);
                    }

                    mTextureAll[i].mainTexture = null;
                    Object.Destroy(mTextureAll[i]);
                    mTextureAll[i] = null;
                }
            }

            mTextureAll.Clear();
            Resources.UnloadUnusedAssets();
        }
    
        public void OnSkipBtnClick()
        {
            Hotfix_LT.Messenger.Raise(EventName.OnStoryPlaySucc);
        }
    
        public override void OnCancelButtonClick()
        {
        }
    
        private static System.Action StoryAction;

        public static void OpenStory(System.Action action, object param = null)
        {
            StoryAction = action;
            GlobalMenuManager.Instance.Open("LTStoryHud", param);
        }

        public static void OpenMovieEx()
        {
            string pausedMusic = FusionAudio.StopMusic();
            LTStoryController.OpenMovie(() =>
            {
                if (LTCombatEventReceiver.Instance != null)
                {
                    LTCombatEventReceiver.Instance.StoryOver(pausedMusic);
                }
            }, "LTPrologueVideo");
        }

        public static void OpenMovie(System.Action action,string Mp4Name)
        {
            EB.Coroutines.Run(PlayVideoCoroutine(action, Mp4Name));
        }
    
        private static IEnumerator PlayVideoCoroutine(System.Action action, string Mp4Name)
        {
            if (!Handheld.PlayFullScreenMovie(string .Format ("{0}.mp4", Mp4Name), Color.black, FullScreenMovieControlMode.Hidden))
            {
                EB.Debug.LogWarning("PlayVideoCoroutine: failed to play {0}.mp4", Mp4Name);
                action?.Invoke();
                yield break;
            }
    
            yield return new WaitForEndOfFrame();
            EB.Debug.Log("PlayVideoCoroutine: application paused to play video");
            yield return new WaitForEndOfFrame();
            EB.Debug.Log("PlayVideoCoroutine: application resumed by video playback completed");
    
            action?.Invoke();
        }
    }
}
