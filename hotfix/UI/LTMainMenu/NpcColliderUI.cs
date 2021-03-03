using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class NpcColliderUI : DynamicMonoHotfix, IHotfixUpdate
    {
        public List<UILabel> FunctionLabel_List;
        public GameObject m_FuncObject;
        public List<UIButton> listFunIconBtn;
        public GameObject TalkUIObj;
        public TweenPosition rightTweenPosition;
        public GameObject m_FunctionRP;
        public GameObject m_FunctionRP2;
        public string title;
        public GameObject SwithBtnRP;
        public List<GameObject> RPList;
        public Transform BtnArrowSprite;

        private bool RightFuncShow = true;
        private string npc = "";
        private string scene = "";
    	private bool areaTrigger;
        private Hashtable taskData;
        private List<Hotfix_LT.Data.FuncTemplate> m_Functions = new List<Hotfix_LT.Data.FuncTemplate>();

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;

            FunctionLabel_List = new List<UILabel>();
            FunctionLabel_List.Add(t.GetComponent<UILabel>("Grid/IconBtn1/Label"));
            FunctionLabel_List.Add(t.GetComponent<UILabel>("Grid/IconBtn2/Label"));
            FunctionLabel_List.Add(t.GetComponent<UILabel>("Grid/IconBtn3/Label (1)"));
            FunctionLabel_List.Add(t.GetComponent<UILabel>("Grid/IconBtn4/Label (2)"));
            FunctionLabel_List.Add(t.GetComponent<UILabel>("Grid/IconBtn5/Label (2)"));

            m_FuncObject = t.FindEx("Grid").gameObject;

            listFunIconBtn = new List<UIButton>();
            listFunIconBtn.Add(t.GetComponent<UIButton>("Grid/IconBtn1"));
            listFunIconBtn.Add(t.GetComponent<UIButton>("Grid/IconBtn2"));
            listFunIconBtn.Add(t.GetComponent<UIButton>("Grid/IconBtn3"));
            listFunIconBtn.Add(t.GetComponent<UIButton>("Grid/IconBtn4"));
            listFunIconBtn.Add(t.GetComponent<UIButton>("Grid/IconBtn5"));

            TalkUIObj = t.FindEx("TalkUIObj").gameObject;

            var edge = t.parent.parent;
            rightTweenPosition = edge.GetComponent<TweenPosition>("Right/RightFuncPanel");
            m_FunctionRP = t.FindEx("Grid/IconBtn1/RedPoint").gameObject;
            m_FunctionRP2 = t.FindEx("Grid/IconBtn2/RedPoint").gameObject;
            SwithBtnRP = edge.FindEx("Right/RightFuncPanel/Content/Btn/RedPoint").gameObject;

            RPList = new List<GameObject>();
            RPList.Add(edge.FindEx("Right/RightFuncPanel/Content/Function/Task/RedPoint").gameObject);
            RPList.Add(edge.FindEx("Right/RightFuncPanel/Content/Function/Partner/RedPoint").gameObject);
            RPList.Add(edge.FindEx("Right/RightFuncPanel/Content/Campaign/RedPoint").gameObject);

            BtnArrowSprite = edge.GetComponent<Transform>("Right/RightFuncPanel/Content/Btn/Left");

            PlayerController.onCollisionExit += OnCollisionExitDo;
            PlayerController.onCollisionOpen += OnCollisionOpen;
            Hotfix_LT.Messenger.AddListener(EventName.TransferDartEndEvent, OnTransferDartEndListener);
            Hotfix_LT.Messenger.AddListener(EventName.RedPoint_NPCFunc, SetRP);
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }
        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener(EventName.TransferDartEndEvent, OnTransferDartEndListener);
            Hotfix_LT.Messenger.RemoveListener(EventName.RedPoint_NPCFunc, SetRP);
            PlayerController.onCollisionExit -= OnCollisionExitDo;
            PlayerController.onCollisionOpen -= OnCollisionOpen;
        }

        public override void Start()
        {
            mDMono.gameObject.CustomSetActive(false);

            for(int i=0;i< listFunIconBtn.Count;i++)
            {
                switch(i)
                {
                    case 0:
                        listFunIconBtn[i].onClick.Add(new EventDelegate(OnFunc1Click));
                        TalkUIObj .GetComponent<UIButton>().onClick .Add(new EventDelegate(OnFunc1Click));
                        break;
                    case 1:
                        listFunIconBtn[i].onClick.Add(new EventDelegate(OnFunc2Click));
                        break;
                    case 2:
                        listFunIconBtn[i].onClick.Add(new EventDelegate(OnFunc3Click));
                        break;
                    case 3:
                        listFunIconBtn[i].onClick.Add(new EventDelegate(OnFunc4Click));
                        break;
                            
                }
            }
        }
        
        private Transform curHeadTransform = null;

        public void Update()
        {
            if (TalkUIObj!=null&&TalkUIObj.activeSelf&& PlayerController.CurNpcCollision!=null)
            {
                if (curHeadTransform == null)
                {
                    var enemyController = PlayerController.CurNpcCollision.transform.GetComponent<EnemyController>();

                    if (enemyController != null)
                    {
                        curHeadTransform = enemyController.SkinnedRigPrefab.GetComponent<MoveEditor.FXHelper>().HeadNubTransform.parent;
                    }
                }

                if (curHeadTransform != null && Camera.main != null && UICamera.currentCamera != null)
                {
                    Vector2 vector2 = Camera.main.WorldToScreenPoint(curHeadTransform.position);//PlayerController.CurNpcCollision.transform.position);
                    TalkUIObj.transform.position = UICamera.currentCamera.ScreenToWorldPoint(new Vector3(vector2.x + 80, vector2.y ));
                }
            }
        }
    
        public void SetMenuData(object _menuData)
        {
            npc = EB.Dot.String("npc", _menuData, npc);
            scene = EB.Dot.String("scene", _menuData, scene);
    		areaTrigger = EB.Dot.Bool("area", _menuData, false);
    		taskData = EB.Dot.Object("task", _menuData, null);
    		Hashtable data= _menuData as Hashtable ;
            PrepareData();
            UpdateUI();
        }
    
        void PrepareData()
        {
            m_Functions.Clear(); 

            if (!string.IsNullOrEmpty(npc) && !string.IsNullOrEmpty(scene))
            {
                Hotfix_LT.Data.MainLandEncounterTemplate met = Hotfix_LT.Data.SceneTemplateManager.GetMainLandsNPCData(scene, npc);
    
                if (met != null)
                {
                    if (met.func_id_1 > 0)
                    {
                        Hotfix_LT.Data.FuncTemplate func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(met.func_id_1);
                        if (func != null)//红点设置
                        {
                            m_Functions.Add(func);
                            SetRP();
                        }
                    }
                    if (met.func_id_2 > 0)
                    {
                        Hotfix_LT.Data.FuncTemplate func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(met.func_id_2);
                        if (func != null)
                        {
                            m_Functions.Add(func);
                            SetRP2();
                        }
                            
                    }
                    if (met.func_id_3 > 0)
                    {
                        Hotfix_LT.Data.FuncTemplate func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(met.func_id_3);
                        if (func != null)
                            m_Functions.Add(func);
                    }
    
                    if (taskData != null)
                    {
                        Hotfix_LT.Data.FuncTemplate func = new Hotfix_LT.Data.FuncTemplate();
                        func.ui_model = "task";
                        string displayName = EB.Dot.String("taskName", taskData, string.Empty);
                        func.display_name = string.Format("[{0}]", EB.Localizer.GetString("ID_MAIN_LINE")) + displayName;
                        m_Functions.Add(func);
                    }
                }
                else
                {
                    EB.Debug.LogError("Tabel did not have npc={0} in scene={1}", npc , scene);
                }
            }
        }

        void UpdateUI(bool playAnimation=true)
        {
            //if (GuideNodeManager.IsGuide)
            //{
            //    m_FuncObject.CustomSetActive(false);
            //    return;
            //}
            if (m_Functions.Count == 0)
            {
                m_FuncObject.CustomSetActive(false);
            }
            else
            {
                m_FuncObject.CustomSetActive(true);
                if (AlliancesManager.Instance.DartData!=null && AlliancesManager.Instance.DartData.State != eAllianceDartCurrentState.Transfering)
                {
                    for (var k = 0; k < m_Functions.Count; k++)
                    {
                        var f = m_Functions[k];

                        if (f.ui_model == "DeliveryDart")//交镖是限时活动 所以要移除
                        {
                            m_Functions.Remove(f);
                            break;
                        }
                    }
                }
    
                if (m_Functions.Count == 0)
                {
                    m_FuncObject.CustomSetActive(false);
                    return;
                }
                int i = 0;
                for (; i < m_Functions.Count; i++)
                {
                    if (m_Functions[i].ui_model == "LTWorldBossHud" && LTWorldBossDataManager.Instance.IsOpenWorldBoss())
                    {
                        LTWorldBossMainMenuCtrl.Instance.ShowBossUI();
                    }
                    
                    listFunIconBtn[i].gameObject.CustomSetActive(true);
                    LTUIUtil.SetText(FunctionLabel_List[i],m_Functions[i].display_name);
                    listFunIconBtn[i].normalSprite = listFunIconBtn[i] .transform .GetChild (1).GetComponent <UISprite >().spriteName = m_Functions[i].iconName ;
                }
                for (; i < listFunIconBtn.Count; i++)
                {
                    listFunIconBtn[i].gameObject.CustomSetActive(false);
                }
    
                m_FuncObject.CustomSetActive(true);
    			if (!areaTrigger && TalkUIObj !=null && UICamera.mainCamera != null&& UICamera.currentCamera!=null)
                {
                    if (PlayerController.CurNpcCollision == null) return;
                    Vector2 vector2 = UICamera.mainCamera.WorldToScreenPoint(PlayerController.CurNpcCollision.transform.position);
    				TalkUIObj.transform.position = UICamera.currentCamera.ScreenToWorldPoint(new Vector3(vector2.x+80, vector2.y));
    				TalkUIObj.CustomSetActive(true);
    			}
    
                if (playAnimation && mDMono.gameObject != null)
                {
    				if(RightFuncShow) OnRightSwithBtnClick();
                    mDMono.transform.GetComponent<TweenAlpha>().ResetToBeginning();
                    mDMono.transform.GetComponent<TweenAlpha>().PlayForward();
    			}            
            }
        }
    
        public void OnRightSwithBtnClick()
        {
            RightFuncShow = !RightFuncShow;
            if (RightFuncShow) {
                //BtnArrowSprite.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
                //SwithBtnRP.CustomSetActive(false);
                //rightTweenPosition.PlayReverse();
            }
            else
            {
                //BtnArrowSprite.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
                //SwithBtnRP.CustomSetActive(MoreBtnRPJudge());
                //rightTweenPosition.PlayForward();
            }
        }
    
        private bool MoreBtnRPJudge()
        {
            bool isRP = false;
            for (int i = 0; i < RPList.Count; i++)
            {
                if (RPList[i].activeSelf)
                {
                    isRP = true; break;
                }
            }
            return isRP;
        }
    
        public void OnFunc1Click()
        {
            OpenFunc(m_Functions[0]);
        }
    
        public void OnFunc2Click()
        {
            OpenFunc(m_Functions[1]);
        }
    
        public void OnFunc3Click()
        {
            OpenFunc(m_Functions[2]);
        }
    
        public void OnFunc4Click()
        {
            OpenFunc(m_Functions[3]);
        }
    
        private void OpenFunc(Hotfix_LT.Data.FuncTemplate func)
        {
            if (func.ui_model == "SpecialActivity")
            {
                if (func.parameter == "9001")
                {
                    if (!ActivityUtil.IsTimeOk(9001))
                    {
                        MessageTemplateManager.ShowMessage(902139);
                        return;
                    }
                }
                else if (func.parameter == "9002")
                {
                    if (!ActivityUtil.IsTimeOk(9002))
                    {
                        MessageTemplateManager.ShowMessage(902140);
                        return;
                    }
                }
            }
            //Close();
            if (func.ui_model.Equals("TaskChase"))
            {
                EnemyController ec = MainLandLogic.GetInstance().GetEnemyController(npc);
                if (ec != null)
                {
                    NpcTaskDataLookup task_datalookup = ec.GetComponentInChildren<NpcTaskDataLookup>();
                    if (task_datalookup != null && task_datalookup.mDL.DefaultDataID != null)
                    {
                        var ht = Johny.HashtablePool.Claim();
                        ht.Add("taskid", task_datalookup.mDL.DefaultDataID);
                        Hotfix_LT.Data.FuncTemplateManager.OpenFunc(func.id, ht);
                    }
                    else
                    {
                        MessageTemplateManager.ShowMessage(902015, null, null);
                    }
                }
            }
            else if (func.ui_model.Equals("task"))
            {
                System.Action callback = taskData["callback"] as System.Action;
                if (callback != null)
                    callback();
            }
            else if (func.ui_model.Equals("DeliveryDart"))
            {
                DeliveryDart();
            }
            else
            {
                if (string.IsNullOrEmpty(func.parameter))
    				Hotfix_LT.Data.FuncTemplateManager.OpenFunc(func.id, null);
                else
    				Hotfix_LT.Data.FuncTemplateManager.OpenFunc(func.id, func.parameter);
            }
        }
    
        static public void DeliveryDart()
        {
            if (AlliancesManager.Instance.DartData.State == eAllianceDartCurrentState.Transfering)
            {
                string currentDartId = AlliancesManager.Instance.TransferDartInfo.GetCurrentDartId();
    			if (!string.IsNullOrEmpty(currentDartId))
    			{
    				AlliancesManager.Instance.DartData.State = eAllianceDartCurrentState.None;
    				SendDeliveryDartRequest(currentDartId);
    			}
    			else
    			{
    				Debug.LogError("currentDartId IsNullOrEmpty");
    			}
    		}        
        }
    
        static public void SendDeliveryDartRequest(string dartId)
        {
            AlliancesManager.Instance.Finish(dartId,delegate(bool successful) {
    			if (successful)
    			{
    				AlliancesManager.Instance.Complete(dartId, delegate (Hashtable result)
    				{
    					if (result != null)
    					{
    						TransferDartMember dart = AlliancesManager.Instance.TransferDartInfo.Find(dartId);
    						dart.State = eAllianceTransferState.Finished;
    						AlliancesManager.Instance.DartData.State = eAllianceDartCurrentState.None;
    
    						bool escortIslost = EB.Dot.Bool("escortIslost", result, false);
                            object robUserInfo = EB.Dot.Object("robUserInfo", result, null);
                            DartResultController.sResultType = escortIslost ? eDartResultType.TransferFail : eDartResultType.TransferSuccess;
    						
                            var datas = GameUtils.ParseAwardArr(Hotfix_LT.EBCore.Dot.Array("completeAward", result, null));
                            var list = new List<LTShowItemData>();

                            for (var i = 0; i < datas.Count; i++)
                            {
                                var item = datas[i];
                                list.Add(new LTShowItemData(item.id, item.count, item.type, false));
                            }

                            DartResultController.sRewardDataList = list;
    
                            //上传友盟获得钻石，军团护送
                            FusionTelemetry.ItemsUmengCurrency(datas, "军团护送");
                            FusionTelemetry.GamePlayData.PostEsortEvent("reward","esort");
                            GlobalMenuManager.Instance.Open("LTShowDartResultHud", robUserInfo);
    					}
    					else
    					{
    						EB.Debug.LogError("DeliveryDart Fail");
    					}
    				});
    			}
    			else
    			{
    				AlliancesManager.Instance.DartData.State = eAllianceDartCurrentState.None;
    			}
    		});       
        }
        
        void OnCollisionOpen(Hashtable data)
        {
            mDMono.gameObject.CustomSetActive(true);
            SetMenuData(data);
        }
    
        void OnCollisionExitDo(string npcName)
        {
            if (npc.Equals(npcName)|| npcName== GuideNodeManager.GUIDE_FUNCTION_OPEN)
            {
                mDMono.gameObject.CustomSetActive(false);
                TalkUIObj.CustomSetActive(false);
                Hotfix_LT.Messenger.Raise("InteractOverEvent");
                
                if (curHeadTransform != null)
                {
                    curHeadTransform = null;
                }

                if (!RightFuncShow)
                {
                    OnRightSwithBtnClick();
                }

                if (npc.Equals("AreaTrigger_E") && LTWorldBossDataManager.Instance.IsLive())
                {
                    LTWorldBossMainMenuCtrl.Instance.HideBossUI();
                }
            }
        }
    
    	void OnTransferDartEndListener()
    	{
    		UpdateUI(false);
    	}
    
        void SetRP()
        {
            if (m_Functions.Count > 0)
            {
                switch (m_Functions[0].id)
                {
                    case 10015:
                        {
                            m_FunctionRP.CustomSetActive(LTUltimateTrialDataManager.Instance.UltimateTrialRP());
                        }
                        break;
                    case 10018:
                        {
                            m_FunctionRP.CustomSetActive(ArenaManager.Instance.ArenaRP());
                        }
                        break;
                    case 10068:
                        {
                            bool rp = false;
                            FuncTemplate m_FuncTpl = FuncTemplateManager.Instance.GetFunc(10068);
                            if (m_FuncTpl.IsConditionOK())
                            {
                                int time= Mathf.Max(0, LTBountyTaskHudController.TotalHantTimes - LTBountyTaskHudController.HantTimes);
                                rp= time > 0;
                            }
                            m_FunctionRP.CustomSetActive(rp);
                        }
                        break;
                    default:m_FunctionRP.CustomSetActive(false);break;
                }
            }

            SetRP2();
        }
        
        private void SetRP2()
        {
            if (m_Functions.Count > 1)
            {
                switch (m_Functions[1].id)
                {
                    //荣耀角斗场
                    case 10090:
                    {
                        bool rp = false;
                        FuncTemplate m_FuncTpl = FuncTemplateManager.Instance.GetFunc(10090);
                        if (m_FuncTpl.IsConditionOK())
                        {
                            int freetimes = HonorArenaManager.Instance.GetHonorArenaFreeTimes();  
                            int usetimes = HonorArenaManager.Instance.Info.usedTimes;
                            rp = freetimes > usetimes;
                        }
                        m_FunctionRP2.CustomSetActive(rp);
                    }
                        break;
                    default:m_FunctionRP2.CustomSetActive(false);break;
                }
            }
        }
    }
}
