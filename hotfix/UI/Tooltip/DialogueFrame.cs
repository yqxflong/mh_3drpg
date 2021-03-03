using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public enum eDialogueLayout
    {
        Middle = 0,
        Left = 1,
        Right = 2,
    }

    public class DialogueFrame : DynamicMonoHotfix
    {
        public DialogueDataFrame m_Left;
        public DialogueDataFrame m_Right;
        public DialogueDataFrame m_Middle;
        public UISprite m_Shade;
        public GameObject m_Container;
        private bool m_State = false;

        private DialogueStepData m_DialogueStepData;
        public delegate void OnFinish();
        public event OnFinish onFinish;

        private static DialogueFrame m_Instance;
        public static DialogueFrame Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    EB.Debug.LogError("DialogueFrame  didnt Init");
                }
                return m_Instance;
            }
        }

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_Left = t.GetMonoILRComponent<DialogueDataFrame>("Container/Down/Left");
            m_Right = t.GetMonoILRComponent<DialogueDataFrame>("Container/Down/Right");
            m_Middle = t.GetMonoILRComponent<DialogueDataFrame>("Container/Down/Middle");
            m_Container = t.FindEx("Container").gameObject;

            t.GetComponent<UIButton>("Container/Down/Jump/BG").onClick.Add(new EventDelegate(OnJumpClick));
            t.GetComponent<UIEventTrigger>("Container/Down/BlurBG").onClick.Add(new EventDelegate(OnClick));

            m_Instance = this;
        }

        public override void OnDestroy()
        {
            m_Instance = null;
        }

        public void CreateBuddle(DialogueStepData data)
        {
            if (data.Layout != (int)eDialogueLayout.Middle)
            {
                //DialoguePlayUtil.Instance.m_DialogueFrame.SetFrameActive(data);
                if (data.Layout == (int)eDialogueLayout.Left)
                {
                    if (!data.Icon.StartsWith("Header_"))
                        StartCoroutine(m_Left.CreateBuddyModel(data.Layout, data));
                }
                else
                {
                    if (!data.Icon.StartsWith("Header_"))
                        StartCoroutine(m_Right.CreateBuddyModel(data.Layout, data));
                }
            }
        }

        public void ResetActive()
        {
            m_Left.mDMono.gameObject.CustomSetActive(false);
            m_Right.mDMono.gameObject.CustomSetActive(false);
            m_Middle.mDMono.gameObject.CustomSetActive(false);
        }

        public void SetFrameActive(DialogueStepData data)
        {
            if (data.Layout == (int)eDialogueLayout.Left)
            {
                m_Right.mDMono.gameObject.CustomSetActive(false);
                m_Left.mDMono.gameObject.CustomSetActive(true);
            }
            else
            {
                m_Right.mDMono.gameObject.CustomSetActive(true);
                m_Left.mDMono.gameObject.CustomSetActive(false);
            }

            m_Middle.mDMono.gameObject.CustomSetActive(false);
        }

        public void Play(DialogueStepData data)
        {
            try
            {
                m_DialogueStepData = data;
                m_Container.CustomSetActive(true);

                if (data.Layout == (int)eDialogueLayout.Middle)
                {
                    m_Left.mDMono.gameObject.CustomSetActive(false);
                    m_Right.mDMono.gameObject.CustomSetActive(false);
                    m_Middle.Play(data);
                    m_Middle.mDMono.gameObject.CustomSetActive(true);
                }
                else if (data.Layout == (int)eDialogueLayout.Left)
                {
                    m_Right.mDMono.gameObject.CustomSetActive(false);
                    m_Middle.mDMono.gameObject.CustomSetActive(false);
                    m_Left.mDMono.gameObject.CustomSetActive(true);
                    //if (DialoguePlayUtil.Instance.Lobby != null) DialoguePlayUtil.Instance.Lobby.ConnectorTexture = m_Left.lobbyTexture;
                    m_Left.Play(data);
                }
                else
                {
                    m_Left.mDMono.gameObject.CustomSetActive(false);
                    m_Middle.mDMono.gameObject.CustomSetActive(false);
                    m_Right.mDMono.gameObject.CustomSetActive(true);
                    //if (DialoguePlayUtil.Instance.Lobby != null) DialoguePlayUtil.Instance.Lobby.ConnectorTexture = m_Right.lobbyTexture;
                    m_Right.Play(data);
                }

                m_State = true;

                if (m_DialogueStepData.StayTime > 0)
                {
                    StartCoroutine(Play(data.StayTime));
                }
                else
                {
                    m_Container.CustomSetActive(true);
                }
            }
            catch (System.Exception e)
            {
                EB.Debug.LogError(e.StackTrace);
                Finish();
            }
        }

        private IEnumerator Play(float staytime)
        {
            if (m_Container == null)
            {
                Finish();
                yield break;
            }

            float time = 0;

            while (m_State && (time < staytime))
            {
                time += Time.deltaTime;
                yield return null;
            }

            Finish();
            yield break;
        }

        public void Finish()
        {
            StopAllCoroutines();

            if (m_Left.mDMono.gameObject.activeInHierarchy)
            {
                if (m_Left.m_LazySpeakContext.isPlaying)
                {
                    m_Left.m_LazySpeakContext.StopSeedLabel();
                    m_State = true;
                    return;
                }

                m_Left.Finish();
            }
            else if (m_Right.mDMono.gameObject.activeInHierarchy)
            {
                if (m_Right.m_LazySpeakContext.isPlaying)
                {
                    m_Right.m_LazySpeakContext.StopSeedLabel();
                    m_State = true;
                    return;
                }

                m_Right.Finish();
            }

            if (onFinish != null) onFinish();
        }

        public void OnClick()
        {
            try
            {
                if (m_State)
                {
                    m_State = false;
                    Finish();
                }
            }
            catch (System.Exception e)
            {
                EB.Debug.LogError(e.StackTrace);
                Finish();
            }
        }

        public void OnJumpClick()
        {
            try
            {
                if (m_State)
                {
                    m_State = false;
                    DialoguePlayUtil.Instance.Jump();
                    Finish();
                }
            }
            catch (System.Exception e)
            {
                EB.Debug.LogError(e.StackTrace);
                Finish();
            }
        }
    }
}
