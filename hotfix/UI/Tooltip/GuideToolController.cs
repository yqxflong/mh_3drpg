using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class GuideToolController : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return false; } }
        public override bool CanAutoBackstack() { return false; }

        private static GuideToolController s_Instance;
        public static GuideToolController Instance
        {
            get { return s_Instance; }
        }

        public override void Awake()
        {
            base.Awake();

            if (controller.ObjectParamList != null)
            {
                var count = controller.ObjectParamList.Count;

                if (count > 0 && controller.ObjectParamList[0] != null)
                {
                    m_SpecialCam = ((GameObject)controller.ObjectParamList[0]).GetComponent<Animator>();
                }
            }

            var t = controller.transform;
            m_VirtualBtn = t.GetComponent<Transform>("VirtualBtn");
            m_moveTips = t.FindEx("MoveTips").gameObject;
            m_DragGuide = t.GetComponent<Transform>("DragGuide");
            m_DragHandler = t.GetComponent<TweenPosition>("DragGuide/Sprite");
            m_TargetPoint = t.GetComponent<Transform>("DragGuide/Target");
            SpecialCamLabel = t.GetComponent<UILabel>("GuideLabel");
            m_DragMonlogLabel = t.GetComponent<UILabel>("DragGuide/Monolog/MonologLabel");

            t.GetComponent<ConsecutiveClickCoolTrigger>("VirtualBtn").clickEvent.Add(new EventDelegate(CloseVirtualBtn));

            s_Instance = this;
            Hide();
        }

        public override void OnFocus()
        {
            string focusName = CommonConditionParse.FocusViewName; //为了不干扰焦点
            if (GuideNodeManager.IsGuide && FocusName == null && m_moveTips.activeSelf) FocusName = CommonConditionParse.FocusViewName;
            base.OnFocus();
            CommonConditionParse.FocusViewName = (FocusName == null) ? focusName : FocusName;
            EB.Debug.Log("FocusViewName <color=#800000ff>{0}</color>",CommonConditionParse.FocusViewName);
        }

        public Transform m_VirtualBtn;
        public System.Action VirtualAction;
        public GameObject m_moveTips;
        public Transform m_DragGuide;
        public Transform m_TargetPoint;
        public Animator m_SpecialCam;
        public UILabel SpecialCamLabel;
        public UILabel m_DragMonlogLabel;
        private TweenPosition m_DragHandler;

        /// <summary>
        /// LT虚拟按键
        /// </summary>
        public void SetVirtualBtn(Vector2 pos,int width=250,int height=250, System.Action action =null)
        {
            VirtualAction = null;
            if (action != null) VirtualAction = action;
            m_VirtualBtn.transform.position = UICamera.currentCamera.ScreenToWorldPoint(pos);
            m_VirtualBtn.gameObject.CustomSetActive(true);
            controller.Open();
            controller.gameObject.CustomSetActive(true);
            m_VirtualBtn.GetComponent<UIWidget>().width = width;
            m_VirtualBtn.GetComponent<UIWidget>().height = height;
        }

        private int timer = 0;
        public void SetVirtualBtn(Transform t, int width = 250, int height = 250, System.Action action = null)
        {
            VirtualAction = null;
            if (action != null) VirtualAction = action;

            Vector2 vector2 = Camera.main.WorldToScreenPoint(t.position);
            m_VirtualBtn.transform.position = UICamera.currentCamera.ScreenToWorldPoint(vector2);
            timer =ILRTimerManager .instance .AddTimer(200,20,delegate {
                if (Camera.main != null && UICamera.currentCamera != null && t != null)
                {
                    vector2 = Camera.main.WorldToScreenPoint(t.position);
                    m_VirtualBtn.transform.position = UICamera.currentCamera.ScreenToWorldPoint(vector2);
                }
            });
            m_VirtualBtn.gameObject.CustomSetActive(true);
            controller.Open();
            controller.gameObject.CustomSetActive(true);
            m_VirtualBtn.GetComponent<UIWidget>().width = width;
            m_VirtualBtn.GetComponent<UIWidget>().height = height;
        }

        public override void Show(bool isShowing)
        {

        }
        public void CloseVirtualBtn()
        {
            if (timer > 0)
            {
                ILRTimerManager.instance.RemoveTimer(timer);
                timer = 0;
            }
            m_VirtualBtn.GetComponent<UIWidget>().width = 250;
            m_VirtualBtn.GetComponent<UIWidget>().height = 250;
            m_VirtualBtn.gameObject.CustomSetActive(false);
            controller.Close();
            controller.gameObject.CustomSetActive(false);
            if (VirtualAction != null)
            {
                VirtualAction();
                VirtualAction = null;
            }
        }

        private string FocusName = null;

        /// <summary>
        /// LT 副本移动提示
        /// </summary>
        /// <param name="isOpen"></param>
        /// <param name="parent"></param>
        public void SetLTMoveTips(bool isOpen, int index = 0)
        {
            for (int i = 0; i < m_moveTips.transform.childCount; i++)
            {
                m_moveTips.transform.GetChild(i).gameObject.CustomSetActive(false);
            }

            if (isOpen)
            {
                m_moveTips.transform.GetChild(index).gameObject.CustomSetActive(isOpen);
            }

            m_moveTips.CustomSetActive(isOpen);

            if (isOpen)
            {
                controller.Open();
            }
            else
            {
                FocusName = null;
                controller.Close();
            }

            controller.gameObject.CustomSetActive(isOpen);

        }
        public void SetLTMoveTipsClose()
        {
            for (int i = 0; i < m_moveTips.transform.childCount; i++)
            {
                m_moveTips.transform.GetChild(i).gameObject.CustomSetActive(false);
            }
            m_moveTips.CustomSetActive(false);
        }

        int timerIndex = 0;
        /// <summary>
        /// LT拖拽引导
        /// </summary>
        public void SetLTDargGuide(bool isOpen, string target = null, string content = null, int LabelWeight = 2)
        {
            m_DragGuide.gameObject.CustomSetActive(isOpen);
            controller.gameObject.CustomSetActive(isOpen);

            if (isOpen)
            {
                timerIndex = ILRTimerManager.instance.AddTimer(200, 1, delegate
                {
                    timerIndex = 0;

                    if (!string.IsNullOrEmpty(target))
                    {
                        m_TargetPoint.position = GameObject.Find(target).transform.position;
                    }

                    TweenWidth TW = m_DragGuide.GetComponent<TweenWidth>();
                    TweenHeight TH = m_DragGuide.GetComponent<TweenHeight>();
                    TW.to = (int)m_TargetPoint.localPosition.x;
                    TH.to = (int)m_TargetPoint.localPosition.y;
                    m_DragHandler.to = new Vector3(m_TargetPoint.localPosition.x+100, m_TargetPoint.localPosition.y+100,0);
                    TW.ResetToBeginning();
                    TH.ResetToBeginning();
                    m_DragHandler.ResetToBeginning();
                    if (content != null)
                    {
                        m_DragMonlogLabel.text = content;
                        m_DragMonlogLabel.width = LabelWeight;
                        m_DragMonlogLabel.transform.parent.gameObject.CustomSetActive(true);
                    }
                    else
                    {
                        m_DragMonlogLabel.transform.parent.gameObject.CustomSetActive(false);
                    }

                    TweenAlpha TA = m_DragGuide.GetComponent<TweenAlpha>();
                    TA.ResetToBeginning();
                    TA.PlayForward();
                });

                controller.Open();
            }
            else
            {
                if (timerIndex > 0)
                {
                    ILRTimerManager.instance.RemoveTimer(timerIndex);
                    timerIndex = 0;
                }
                UITexture Tex = m_DragGuide.GetComponent<UITexture>();
                Tex.alpha = 0;
                controller.Close();
            }
        }

        public void Hide()
        {
            //if (GuideUIHelperMengbanController.Instance != null) GuideUIHelperMengbanController.Instance.OpenMengban();
            controller.Close();
            FocusName = null;
            m_VirtualBtn.gameObject.CustomSetActive(false);
            m_moveTips.gameObject.CustomSetActive(false);
            m_DragMonlogLabel.transform.parent.gameObject.CustomSetActive(false);
            m_DragGuide.gameObject.CustomSetActive(false);
            controller.gameObject.CustomSetActive(false);
        }

        public void SetSpecialCam(System.Action CallBack)
        {
            m_CallBack = CallBack;
            controller.gameObject.CustomSetActive(true);
            UICamera.currentCamera.enabled = false;
            Transform CamPartner = UICamera.currentCamera.transform.parent;

            for (int i = 0; i < CamPartner.childCount; i++)
            {
                UIPanel uipanel = CamPartner.GetChild(i).GetComponent<UIPanel>();

                if (uipanel != null && !uipanel.transform.name.Contains("UIHelperPrefab"))
                {
                    uipanel.alpha = 0;
                }
            }

            SpecialCamLabel.gameObject.CustomSetActive(true);
            SpecialCamLabel.GetComponent<TweenAlpha>().PlayForward();
            UICamera.currentCamera.enabled = true;
            StartCoroutine(PlayCinema());
        }

        GameObject AniObj = null;

        IEnumerator PlayCinema()
        {
            //WaitForSeconds wait = new WaitForSeconds(LTMainMenuHudController.Instance.gameObject.GetComponent<TweenAlpha>().duration);
            //yield return wait;


            AniObj = GameObject.Instantiate(m_SpecialCam.gameObject);
            AniObj.CustomSetActive(true);
            AnimationClip[] clips = m_SpecialCam.runtimeAnimatorController.animationClips;
            float length = 0;

            for (int i = 0; i < clips.Length; i++)
            {
                length += clips[i].length;
            }

            int timer = (int)((length - 11f) * 1000);
            ILRTimerManager.instance.AddTimer(timer,1,delegate { HideSpecialCamLabel(); });
            timer= (int)((length - 1.5f) * 1000);
            ILRTimerManager.instance.AddTimer(timer, 1, delegate { HideSpecialCam(); });
            yield break;
        }

        private System.Action m_CallBack = null;

        public void HideSpecialCamLabel()
        {
            SpecialCamLabel.GetComponent<TweenAlpha>().PlayReverse();
        }

        public void HideSpecialCam()
        {
            controller.gameObject.CustomSetActive(false);
            AniObj.CustomSetActive(false);
            Object.Destroy(AniObj);

            SpecialCamLabel.gameObject.CustomSetActive(false);
            Transform CamPartner = UICamera.currentCamera.transform.parent;

            for (int i = 0; i < CamPartner.childCount; i++)
            {
                UIPanel uipanel = CamPartner.GetChild(i).GetComponent<UIPanel>();
                if (uipanel != null && !uipanel.transform.name.Contains("UIHelperPrefab")) uipanel.alpha = 1;
            }
            //UICamera.currentCamera.enabled = true;
            if (m_CallBack != null)
            {
                m_CallBack();
                m_CallBack = null;
            }
        }
    }
}
