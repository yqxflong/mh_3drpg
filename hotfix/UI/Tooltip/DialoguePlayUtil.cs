using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public class DialoguePlayUtil : UIControllerHotfix, IHotfixUpdate
    {
        public UI3DLobby Lobby;
        public bool ShowContext;
        public GM.AssetLoader<GameObject> Loader;
        public string ModelName;
        public int Layout;
        public bool IsLobbyLoadOk;

        public override bool ShowUIBlocker { get { return false; } }
        public DialogueFrame m_DialogueFrame;

        public object onFinish { get; set; }
        public int m_CurrentDialogueId;
        private DialogueData m_DialogueData;
        private int m_CurrentStepIndex = 1;

        private bool m_state = false;
        private bool m_guideToolState = false;
        public bool State { get { return m_state; } }
        public UIPanel m_Panel;

        private RenderTexture rt;

        public Shader repaceTopShader;
        public float dialogueOutlineScale;

        public GameObject GuideObj;

        private static DialoguePlayUtil m_Instance;
        public static DialoguePlayUtil Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    EB.Debug.LogError("DialoguePlayUtil  didnt Init");
                }
                return m_Instance;
            }
        }

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            ShowContext = false;
            Layout = 0;
            IsLobbyLoadOk = false;
            m_DialogueFrame = t.GetMonoILRComponent<DialogueFrame>("DefaultDialogueFrame");
            m_CurrentDialogueId = 0;
            dialogueOutlineScale = 1f;
            GuideObj = t.parent.FindEx("Guide").gameObject;

            t.GetComponent<UIButton>("DefaultDialogueFrame/Container/Down/SkipBtn").onClick.Add(new EventDelegate(OnSkipBtnClick));

            m_Instance = this;
            m_Panel = controller.transform.GetComponentEx<UIPanel>();
            m_Panel.enabled = false;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (m_guideToolState)
            {
                m_guideToolState = false;
                GuideObj.CustomSetActive(true);
                if (GuideNodeManager.ExecuteGuideAudio != null) GuideNodeManager.ExecuteGuideAudio(true);
            }
        }

        public override void OnDestroy()
        {
            m_Instance = null;
            if (rt != null)
            {
                RenderTexture.ReleaseTemporary(rt);
            }

            rt = null;
            base.OnDestroy();
        }

        public override void OnFocus()
        {
            base.OnFocus();
            if (MengBanController.Instance.Fobidden)
            {
                mSaveBlock = true;
                MengBanController.Instance.UnFobiddenUI();
            }
        }

        public override IEnumerator OnAddToStack()
        {
            if (GuideObj.activeSelf)
            {
                m_guideToolState = true;
                GuideObj.CustomSetActive(false);
                if (GuideNodeManager.ExecuteGuideAudio != null) GuideNodeManager.ExecuteGuideAudio(false);
            }
            yield return base.OnAddToStack();
            GlobalMenuManager.Instance.CloseMenu("LTMainInstanceLampView");
            FusionAudio.StartBGM();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            m_DialogueFrame.m_Left.m_SpriteIcon.spriteName = string.Empty;
            m_DialogueFrame.m_Right.m_SpriteIcon.spriteName = string.Empty;
            m_DialogueFrame.m_Left.mDMono.gameObject.CustomSetActive(false);
            m_DialogueFrame.m_Right.mDMono.gameObject.CustomSetActive(false);
            FusionAudio.StopBGM();
            if (GlobalMenuManager.Instance != null)
            {
                GlobalMenuManager.Instance.RemoveOpenController(controller);
            }

            if (m_guideToolState)
            {
                m_guideToolState = false;
                GuideObj.CustomSetActive(true);
                if (GuideNodeManager.ExecuteGuideAudio != null)GuideNodeManager.ExecuteGuideAudio(true);
            }
            return base.OnRemoveFromStack();
        }

        public override void OnCancelButtonClick()
        {
            if (!IsLobbyLoadOk)
                return;

            base.OnCancelButtonClick();
        }

        private bool mSaveBlock = false;
        public void Play(int DialogueId, System.Action callback)
        {
            IsLobbyLoadOk = false;
            DialogueData data = Hotfix_LT.Data.DialogueTemplateManager.Instance.GetDialogueData(DialogueId);
            if (data == null)
            {
                DoFinishCallBack();
                return;
            }
            m_CurrentDialogueId = DialogueId;

            Play(data, callback);
        }

        private void Play(DialogueData data, System.Action callback)
        {
            try
            {
                onFinish = callback;
                if (data == null)
                {
                    DoFinishCallBack();
                    return;
                }
                m_Panel.enabled = true;
                controller.Open();
                m_state = true;
                m_DialogueData = data;
                m_DialogueFrame.onFinish += OnDialogueStepFinish;
                m_CurrentStepIndex = 1;
                if (data.Steps.Length >= m_CurrentStepIndex)
                {
                    DialogueStepData step = data.Steps[m_CurrentStepIndex - 1];
                    if (step != null) PlayStep(step);
                    else
                    {
                        EB.Debug.LogError(string.Format("Dialogue step null in index {0} for {1}", m_CurrentStepIndex, data.DialogueId));
                        DialogueFinish();
                    }
                }
                else DialogueFinish();
            }
            catch (System.Exception e)
            {
                EB.Debug.LogError(e.StackTrace);
                DialogueFinish();
            }
        }

        public bool IsPlayStep
		{
			get { return m_IsPlayStep; }
			set {
				if (value)
					RegisterMonoUpdater();
				else
					ErasureMonoUpdater();
				if (value != m_IsPlayStep) m_IsPlayStep = value;
			}
		}
		private bool m_IsPlayStep = false;

		private DialogueStepData mStep;
        private void PlayStep(DialogueStepData step)
        {
            m_CurrentStepIndex = step.StepId;
            mStep = step;
            m_DialogueFrame.CreateBuddle(step);
            IsPlayStep = true;
        }
        
		public void Update()
        {
            if (IsPlayStep)
            {
                if (ShowContext)
                {
                    m_DialogueFrame.Play(mStep);
                    IsPlayStep = false;
                }
            }
        }

        private void OnDialogueStepFinish()
        {
            try
            {
                m_DialogueFrame.onFinish -= OnDialogueStepFinish;
                m_CurrentStepIndex += 1;

                if (m_DialogueData.Steps.Length >= m_CurrentStepIndex)
                {
                    if (m_state == false)//跳过
                    {
                        DialogueFinish();
                        return;
                    }
                    m_DialogueFrame.onFinish += OnDialogueStepFinish;
                    DialogueStepData step = m_DialogueData.Steps[m_CurrentStepIndex - 1];
                    if (step != null) PlayStep(step);
                    else
                    {
                        EB.Debug.LogError(string.Format("Dialogue step null in index {0} for {1}", m_CurrentStepIndex, m_DialogueData.DialogueId));
                        DialogueFinish();
                    }
                }
                else DialogueFinish();
            }
            catch (System.Exception e)
            {
                EB.Debug.LogError(e.StackTrace);
                DialogueFinish();
            }
        }

        private void DialogueFinish()
        {
            if (m_guideToolState)
            {
                m_guideToolState = false;
                GuideObj.CustomSetActive(true);
                if (GuideNodeManager.ExecuteGuideAudio != null) GuideNodeManager.ExecuteGuideAudio(true);
            }

            try
            {
                m_state = false;
                controller.Close();
                m_Panel.enabled = false;
                DoFinishCallBack();
            }
            catch (System.Exception e)
            {
                EB.Debug.LogError(e.StackTrace);
                DoFinishCallBack();
            }
        }

        void DoFinishCallBack()
        {
            if (mSaveBlock)
            {
                mSaveBlock = false;
                if (GuideManager.Instance.GuideState)
                {
                    MengBanController.Instance.FobiddenUI();
                }
            }
            if (onFinish != null && onFinish is System.Action)
            {
                var cb = (System.Action)onFinish;
                onFinish = null;
                cb();
            }
            if (ModelName != null)
            {
                ModelName = null;
            }
        }

        public void Jump()
        {
            m_state = false;
        }

        public void FinishDialogue()
        {
            if (m_state == true)
            {
                m_state = false;
                m_DialogueFrame.Finish();
            }
        }

        public void OnSkipBtnClick()
        {
            if (m_state == true)
            {
                m_state = false;
                OnDialogueStepFinish();
            }
            else
            {
                DialogueFinish();
            }
        }
    }
}
