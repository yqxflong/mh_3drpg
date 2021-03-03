using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class FocusEffectController : DynamicMonoHotfix
    {
        private bool state = false;
        private int fast_clicks = 0;
        private float last_click_time = 0;
        public float click_delt = 0.5f;
        public int max_clicks = 5;
        public GameObject FxObj;
        
        private bool can_finish = true;
        public bool CanFinish
        {
            get
            {
                return can_finish;
            }
            set
            {
                can_finish = value;
            }
        }

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            click_delt = 0.3f;
            max_clicks = 5;
            FxObj = t.parent.parent.FindEx("Arrow/FingerPanel/FocusFX").gameObject;

            t.GetComponentEx<TweenScale>().onFinished.Add(new EventDelegate(OnPlayEnd));
        }

        public void Play(Vector3 pos)
        {
            state = true;
            //transform.localPosition = pos;
            /*UITweener[] tweens = GetComponentsInChildren<UITweener>();
            for (int i = 0; i < tweens.Length; i++)
            {
                tweens[i].ResetToBeginning();
                tweens[i].PlayForward();
            }*/
            StartCoroutine(PlayFx());
        }
    
        public void OnPlayEnd()
        {
            state = false;
        }
    
        public void OnScreenClick()
        {
            if (GuideManager.Instance.GuidNowAnchor() != eTipAnchorType.center)
            {
    			if (can_finish)
    			{
    			}
    		}
            else
            {
    			if (Time.time - last_click_time <= click_delt)
    			{
    				fast_clicks++;
    			}
    			else
    			{
    				fast_clicks = 1;
    			}
    			last_click_time = Time.time;
                if(fast_clicks>= max_clicks)
                {
                    fast_clicks = 0;
                    if(GameStateManager.Instance!=null)
                    {
                        /* GameStateLoadGame loadState = GameStateManager.Instance.GetGameState<GameStateLoadGame>();
                         if (loadState != null && loadState.flowMgr != null && loadState.flowMgr.m_StateMachine != null) playstate = loadState.flowMgr.m_StateMachine.Fsm.ActiveStateName;
                         if (!playstate.Equals("CombatView"))
                         {*///移除战斗界面跳过机制
                        /*switch (GuideNodeManager.currentGuideId)
                        {
                            case 0:
                                {*/
                                    MessageTemplateManager.ShowMessage(901099, null, delegate (int result)
                                    {
                                        if (result == 0)
                                        {
                                            GuideNodeManager.GetInstance().JumpGuide();
                                        }
                                    });
                         /*       } break;
                            default:
                                {
                                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_codefont_in_FocusEffectController_2664"), delegate (int result)
                                    {
                                        if (result == 0)
                                        {
                                            GuideNodeManager.GetInstance().JumpGuide();
                                        }
                                    });
                                } break;
                        }*/
    
                       // }
                    }
                }
                if (!state)Play(Input.mousePosition);
            }
        }
    
        private WaitForSeconds seconds1 = new WaitForSeconds(1f);
        IEnumerator PlayFx()
        {
            FxObj.CustomSetActive(true);
            yield return seconds1;
            FxObj.CustomSetActive(false);
            state = false;
        }

        public override void OnDisable()
        {
            StopCoroutine(PlayFx());
            FxObj.CustomSetActive(false);
            state = false;
        }
    }
}
