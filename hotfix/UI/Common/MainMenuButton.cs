using UnityEngine;

namespace Hotfix_LT.UI
{
    public class MainMenuButton : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            if (mDMono.ObjectParamList != null)
            {
                var count = mDMono.ObjectParamList.Count;

                if (count > 0 && mDMono.ObjectParamList[0] != null)
                {
                    uiInput = ((GameObject)mDMono.ObjectParamList[0]).GetComponentEx<UIInput>();
                }
                if (count > 1 && mDMono.ObjectParamList[1] != null)
                {
                    uiInput2 = ((GameObject)mDMono.ObjectParamList[1]).GetComponentEx<UIInput>();
                }
            }

            var t = mDMono.transform;
            t.GetComponent<UIButton>("LegionCombat").onClick.Add(new EventDelegate(LegionWar));
            t.GetComponent<UIButton>("CreateAnim").onClick.Add(new EventDelegate(Load));
            t.GetComponent<UIButton>("NationCombat").onClick.Add(new EventDelegate(OnOpenNationCombat));
            t.GetComponent<UIButton>("Store").onClick.Add(new EventDelegate(OnOpenStore));
            t.GetComponent<UIButton>("GoldBattle").onClick.Add(new EventDelegate(OnOpenGoldInstance));
            t.GetComponent<UIButton>("ExpBattle").onClick.Add(new EventDelegate(OnOpenExpInstance));
            t.GetComponent<UIButton>("BountyTask").onClick.Add(new EventDelegate(OnOpenBountyTask));
            t.GetComponent<UIButton>("UltimateTrial").onClick.Add(new EventDelegate(OnOpenUltimateTrial));
            t.GetComponent<UIButton>("Conversation/Conversation").onClick.Add(new EventDelegate(OnOpenConversation));
            t.GetComponent<UIButton>("Communication/Communication").onClick.Add(new EventDelegate(OnOpenCommunication));
        }


        private static GameObject sc;
    
        public void Load() {
            TestGMController.Instance.controller.Close();
    
            Object.Destroy(sc);
            sc = (GameObject)Resources.Load("Prefabs/Anim");
            GameObject StoryPrefab = GameObject.Find("StoryPrefab");
            sc = Object.Instantiate(sc);
            sc.transform.parent = StoryPrefab.transform;
        }
    
        /* public void OnOpenQualifyWar() {
             LTLegionWarManager.Instance.OnOpenQualifyWar();
         }
    
         public void OnCloseLegionWar()
         {
             LTLegionWarManager.Instance.CloseAllianceWar();
             MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_MainMenuButton_725"));
         }*/
    
        public void LegionWar() {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            if (LegionModel.GetInstance().isJoinedLegion)
            {
                GlobalMenuManager.Instance.Open("LTLegionWarJoinView");
            }
            else
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_codefont_in_FuncTemplateManager_5098"), delegate (int r)
                {
                    if (r == 0)
                    {
                        GlobalMenuManager.Instance.Open("LTSearchJTMenu");
                        if ((LegionModel.GetInstance().searchItemDatas == null || LegionModel.GetInstance().searchItemDatas.Length == 0) || Time.unscaledTime - LegionModel.GetInstance().searchListTime > 10)  //无军团列表数据或最近一次不是自动搜索或拉取数据超过CD10秒
                        {
                            LegionModel.GetInstance().searchListTime = Time.unscaledTime;
                            AlliancesManager.Instance.RequestAllianceList();
                        }
                    }
                });
            }
        }
    
        public void OnOpenNationCombat()
        {
            GlobalMenuManager.Instance.Open("LTNationHudUI");
        }
    
        public void OnOpenStore() {
            GlobalMenuManager.Instance.Open("LTStoreUI");
        }
    
        public void OnOpenGoldInstance() {
            GlobalMenuManager.Instance.Open("LTResourseInstanceUI", "Gold");
        }
        public void OnOpenExpInstance() {
            GlobalMenuManager.Instance.Open("LTResourseInstanceUI", "Exp");
        }
    
        public void OnOpenBountyTask()
        {
            GlobalMenuManager.Instance.Open("LTBountyTaskHudUI");
        }
    
        public void OnOpenUltimateTrial()
        {
            GlobalMenuManager.Instance.Open("LTUltimateTrialHudView");
        }
    
        public UIInput uiInput;
        public void OnOpenConversation()
        {
            if (uiInput == null)
            {
                return;
            }

            int ConversationId = 1010101;
            int.TryParse(uiInput.value, out ConversationId);
            DialoguePlayUtil.Instance.Play(ConversationId, null);
        }
    
        public UIInput uiInput2;
        public void OnOpenCommunication()
        {
            if (uiInput2 == null)
            {
                return;
            }

            int ConversationId = 10010;
            int.TryParse(uiInput2.value, out ConversationId);
            GlobalMenuManager.Instance.Open("LTCommunicationGM", ConversationId);
        }
    }
}
