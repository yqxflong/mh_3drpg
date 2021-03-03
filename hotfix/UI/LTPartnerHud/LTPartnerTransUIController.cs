using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


namespace Hotfix_LT.UI
{
    public class LTPartnerTransUIController : UIControllerHotfix
    {
        public UILabel leftNameLabel;
        public UILabel rightNameLabel;
        public UILabel transPriceLabel;
        public UISprite DiamondSprite;
        public GameObject pricefreelabel;
        public CombatPartnerDynamicScroll DynamicScroll;
        public CombatPartnerCellController DragPartnerCell;
        private UILabel mCountdownLabelCache;
        public RenderTexture ModelRt;
        private eAttrTabType mCurPartnerTabType = eAttrTabType.All;
        private int currCost = 0;
        public GameObject LeftFx;
        public GameObject RightFx;
        public UIToggle SelectSwitchEquip;
        public UIGrid uIGrid;
        public UIToggle SelectSwitchPeak;
        public UIToggle SelectSwitchPo;
        public UISprite typeSprite_1;
        public UISprite typeSprite_2;
        
        public static int tranReplaceStoneId = 1428;
        public int tranReplaceStone
        {
            get
            {
                int i = 0;
                i = GameItemUtil.GetInventoryItemNum(tranReplaceStoneId);
                return i;
            }
        }
        
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            uIGrid = t.Find("Edge/Bottom/Grid").GetComponent<UIGrid>();
            SelectSwitchEquip = t.Find("Edge/Bottom/Grid/SwitchEquipBtn/SelectToggle").GetComponent<UIToggle>();
            SelectSwitchPeak = t.Find("Edge/Bottom/Grid/SwitchPeakBtn/SelectToggle").GetComponent<UIToggle>();
            SelectSwitchPo = t.Find("Edge/Bottom/Grid/SwitchPoBtn/SelectToggle").GetComponent<UIToggle>();
            typeSprite_1 = t.GetComponent<UISprite>("Edge/Center/LeftPartCell/Type");
            typeSprite_2 = t.GetComponent<UISprite>("Edge/Center/RightPartCell/Type");
            leftNameLabel = t.GetComponent<UILabel>("Edge/Center/LeftPartCell/Label");
            rightNameLabel = t.GetComponent<UILabel>("Edge/Center/RightPartCell/Label (1)");
            transPriceLabel = t.GetComponent<UILabel>("Edge/Bottom/TransBtn/Label_1");
            DiamondSprite = t.GetComponent<UISprite>("Edge/Bottom/TransBtn/Sprite");
            pricefreelabel = t.FindEx("Edge/Bottom/TransBtn/Label_2").gameObject;
            DynamicScroll = t.GetMonoILRComponent<CombatPartnerDynamicScroll>("Edge/Bottom/BuddyList/Placeholder/PartnerGrid");
            DragPartnerCell = t.GetMonoILRComponent<CombatPartnerCellController>("Edge/DragPanel/DragPartnerItem");
            LeftFx = t.FindEx("Edge/Center/LeftPartCell/Container/fx_hb_UI_Zhuanhuan_1").gameObject;
            RightFx = t.FindEx("Edge/Center/RightPartCell/Container/fx_hb_UI_Zhuanhuan_2").gameObject;
            DRAG_Z = -2f;
            MIN_DRAG_DIST = 0.35f;
            DRAG_OFFSET_DIST = 0.12f;
            MIN_DRAG_IN_DIST = 0.34f;
            CHALLENGE_MIN_DRAG_IN_DIST = 0.2f;
            judgePosLeft = t.GetComponent<Transform>("Edge/Center/LeftPartCell/Container");
            judgePosRight = t.GetComponent<Transform>("Edge/Center/RightPartCell/Container");
            leftIcon = t.GetMonoILRComponent<CombatPartnerCellController>("Edge/Center/LeftPartCell/Container/DragPartnerItem");
            rightIcon = t.GetMonoILRComponent<CombatPartnerCellController>("Edge/Center/RightPartCell/Container/DragPartnerItem");
            MoveSpeed = 5f;
            tempWorldVec = Vector3.zero;
            UIButton backButton = t.GetComponent<UIButton>("Edge/LeftUp/CancelBtn");
            backButton.onClick.Add(new EventDelegate(OnCancelButtonClick));
            
            BattleReadyTitle battleReady = t.GetMonoILRComponent<BattleReadyTitle>("Edge/Bottom/BG/Title");
            UIButton AllBtn= t.GetComponent<UIButton>("Edge/Bottom/BG/Title/BtnList/AllBtn");
            AllBtn.onClick.Add(new EventDelegate(() => OnRaceTabClick(t.FindEx("Edge/Bottom/BG/Title/BtnList/AllBtn").gameObject)));
            UIButton FengBtn= t.GetComponent<UIButton>("Edge/Bottom/BG/Title/BtnList/FengBtn");
            FengBtn.onClick.Add(new EventDelegate(() => OnRaceTabClick(t.FindEx("Edge/Bottom/BG/Title/BtnList/FengBtn").gameObject)));
            UIButton HuoBtn = t.GetComponent<UIButton>("Edge/Bottom/BG/Title/BtnList/HuoBtn");
            HuoBtn.onClick.Add(new EventDelegate(() => OnRaceTabClick(t.FindEx("Edge/Bottom/BG/Title/BtnList/HuoBtn").gameObject)));
            UIButton ShuiBtn =t.GetComponent<UIButton>("Edge/Bottom/BG/Title/BtnList/ShuiBtn");
            ShuiBtn.onClick.Add(new EventDelegate(() => OnRaceTabClick(t.FindEx("Edge/Bottom/BG/Title/BtnList/ShuiBtn").gameObject)));
            
            AllBtn.onClick.Add(new EventDelegate(() =>  { battleReady.OnTitleBtnClick(AllBtn.transform.FindEx("Sprite").gameObject); }));
            FengBtn.onClick.Add(new EventDelegate(() =>  { battleReady.OnTitleBtnClick(FengBtn.transform.FindEx("Sprite").gameObject); }));
            HuoBtn.onClick.Add(new EventDelegate(() =>  { battleReady.OnTitleBtnClick(HuoBtn.transform.FindEx("Sprite").gameObject); }));
            ShuiBtn.onClick.Add(new EventDelegate(() => { battleReady.OnTitleBtnClick(ShuiBtn.transform.FindEx("Sprite").gameObject); }));
            
            
            t.GetComponent<ConsecutiveClickCoolTrigger>("Edge/Bottom/TransBtn").clickEvent.Add(new EventDelegate(OnPartnerTransClick));
            t.GetComponent<UIButton>("Edge/Bottom/RuleBtn").onClick.Add(new EventDelegate(OnRuleBtnClick));

            t.GetComponent<ContinueClickCDTrigger>("Edge/Center/LeftPartCell/Container").m_CallBackPress.Add(new EventDelegate(() => OnClickOutTeam(t.GetComponent<Transform>("Edge/Center/LeftPartCell/Container"), t.GetComponent<UILabel>("Edge/Center/LeftPartCell/Label"))));
            t.GetComponent<ContinueClickCDTrigger>("Edge/Center/RightPartCell/Container").m_CallBackPress.Add(new EventDelegate(() => OnClickOutTeam(t.GetComponent<Transform>("Edge/Center/RightPartCell/Container"), t.GetComponent<UILabel>("Edge/Center/RightPartCell/Label (1)"))));
        }

        private void OnRuleBtnClick()
        {
              string text = EB.Localizer.GetString("ID_PARTNER_TRANS_RULE");
              GlobalMenuManager.Instance.Open("LTRuleUIView", text);
        }

        public override bool IsFullscreen()
        {
            return true;
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            DynamicScroll.mDMono.gameObject.CustomSetActive(true);
            BattleReadyHudController.SetBattleType(eBattleType.MainCampaignBattle);
            LTPartnerDataManager.Instance.InitPartnerData();
            SetJudgePos();
            RefreshPartnerList(mCurPartnerTabType);
            ReflashTransCard();
            InitTransCost();
            
            SelectSwitchPeak.transform.parent.gameObject.CustomSetActive(false);
            SelectSwitchPo.transform.parent.gameObject.CustomSetActive(false);
            SelectSwitchPeak.value = true;
            SelectSwitchPo.value = true;
            typeSprite_1.gameObject.CustomSetActive(false);
            typeSprite_2.gameObject.CustomSetActive(false);
        }
    
        public override IEnumerator OnAddToStack()
        {
            if (UIRoot.list != null && UIRoot.list.Count > 0)
            {
                if (ILRDefine.UNITY_EDITOR)
                {
                    ModelRt = new RenderTexture(Screen.width, Screen.height, 0);
                }
                else
                {
                    ModelRt = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB1555);
                }
            }
            controller.transform.localPosition = new Vector3(0, 0, 1500);
            //SetBattleBtn();
            yield return base.OnAddToStack();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            UICamera.mainCamera.transform.localPosition = new Vector3(0, 0, 0);
    
            this.DynamicScroll.mDMono.gameObject.CustomSetActive(false);
            DynamicScroll.Clear();
            DestroySelf();
            yield break;
        }
    
    
       
        public override void OnCancelButtonClick()
        {
            base.OnCancelButtonClick();
            ResetTransInfo();
        }
    
        /// <summary>
        /// 重置转换队列
        /// </summary>
        private void ResetTransInfo()
        {
            rightNameLabel.text = leftNameLabel.text = EB.Localizer.GetString("ID_PARTNER_TRANSTIPS");
            currCost = 0;
            tempTransData[0] = null;
            tempTransData[1] = null;
        }
        private LTPartnerData[] tempTransData = new LTPartnerData[2];
        /// <summary>
        /// 队列中是否存在该英雄
        /// </summary>
        private bool IsInTransTeam(int heroid)
        {
            for(int i = 0; i < tempTransData.Length; i++)
            {
                if(tempTransData[i]!=null)               
                {
                    if(tempTransData[i].HeroId == heroid)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 拖入图标到转换位置
        /// </summary>
        /// <param name="data"></param>
        private void ToTransTeam(Transform target)
        {
            int positionNum = target.position == judgeposl ? 0 : 1;
            tempTransData[positionNum] = partnerData;
            RefreshPartnerList(mCurPartnerTabType);
        }
    
        /// <summary>
        /// 刷新伙伴数据列表
        /// </summary>
        /// <param name="tab_type"></param>
        private void RefreshPartnerList(eAttrTabType tab_type)
        {
            mCurPartnerTabType = tab_type;
            List<LTPartnerData> partnerList = LTPartnerDataManager.Instance.GetOwnPartnerList();
    
            List<LTPartnerData> filterList = new List<LTPartnerData>();
            for (int i = 0; i < partnerList.Count; i++)
            {
                if (IsInTransTeam(partnerList[i].HeroId))
                    continue;
                if (tab_type == eAttrTabType.All)
                    filterList.Add(partnerList[i]);
                else if (tab_type == eAttrTabType.Feng && partnerList[i].HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Feng)
                    filterList.Add(partnerList[i]);
                else if (tab_type == eAttrTabType.Huo && partnerList[i].HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Huo)
                    filterList.Add(partnerList[i]);
                else if (tab_type == eAttrTabType.Shui && partnerList[i].HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Shui)
                    filterList.Add(partnerList[i]);
            }

            DynamicScroll.SetItemDatas(filterList.ToArray());
            SetIconDragAction();
            SetBtnState();
        }


        private void SetBtnState()
        {
            SelectSwitchPeak.transform.parent.gameObject.CustomSetActive(false);
            SelectSwitchPo.transform.parent.gameObject.CustomSetActive(false);
            typeSprite_1.gameObject.CustomSetActive(false);
            typeSprite_2.gameObject.CustomSetActive(false);
            
            
            if (tempTransData[0]!=null)
            {
                if (tempTransData[0].Level>= LTPartnerDataManager.Instance.GetPeakOpenLevel())
                {
                    SelectSwitchPeak.transform.parent.gameObject.CustomSetActive(true);
                }
                typeSprite_1.gameObject.CustomSetActive(true);
                typeSprite_1.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)tempTransData[0].HeroInfo.role_grade];
            }
            
            if (tempTransData[1]!=null)
            {
                if (tempTransData[1].Level>= LTPartnerDataManager.Instance.GetPeakOpenLevel())
                {
                    SelectSwitchPeak.transform.parent.gameObject.CustomSetActive(true);
                }
                typeSprite_2.gameObject.CustomSetActive(true);
                typeSprite_2.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)tempTransData[1].HeroInfo.role_grade];
            }

            if (tempTransData[0] != null && tempTransData[1] != null)
            {
                if (tempTransData[0].HeroInfo.role_grade == tempTransData[1].HeroInfo.role_grade && tempTransData[0].Star ==
                tempTransData[1].Star && tempTransData[0].Star == 6)
                {
                    SelectSwitchPo.transform.parent.gameObject.CustomSetActive(true);
                }
            }
            
            uIGrid.Reposition();
        }
        
        
        /// <summary>
        /// 点击伙伴转换按钮
        /// </summary>
        public void OnPartnerTransClick()
        {
            if (isOnDrag)
            {
                DragEndFunc();
            }
            //判断是否有足够角色
            if (tempTransData[0] != null&&tempTransData[1]!=null)
            {
                bool switchPeak = SelectSwitchPeak.gameObject.activeInHierarchy & SelectSwitchPeak.value;
                bool switchPo = SelectSwitchPo.gameObject.activeInHierarchy & SelectSwitchPo.value;
                
                //判断角色的货币是否够
                int diamond = 0;
                DataLookupsCache.Instance.SearchIntByID("res.hc.v", out diamond);
                if (tranReplaceStone <= 0 && currCost > diamond)
                {
                    //钻石不够
                    //提示充值
                    BalanceResourceUtil.HcLessMessage();
                    return;
                }
                else
                {
                    //播放特效
                    //发送请求
                    int[] heroid = new int[2];
                    
                    heroid[0] = tempTransData[0].HeroId;
                    heroid[1] = tempTransData[1].HeroId;
                    LTPartnerDataManager.Instance.PartnerTrans(heroid[0], heroid[1], SelectSwitchEquip.value,tranReplaceStone > 0,switchPeak,switchPo,delegate (bool sucess)
                    {
                        if (sucess)
                        {
                            //播放特效//刷新显示 
                            StartCoroutine(PlayTransFx());
                            FusionAudio.PostEvent("UI/New/HuoBanZhuanHuan", true);
                            tempTransData[0].powerData.OnValueChanged(tempTransData[0],false, PowerData.RefreshType.All);
                            tempTransData[1].powerData.OnValueChanged(tempTransData[1], false, PowerData.RefreshType.All);
                            LTFormationDataManager.OnRefreshMainTeamPower(true);
                            Hotfix_LT.Messenger.Raise<int, bool>(Hotfix_LT.EventName.onPartnerCombatPowerUpdate, 0, false);
                            if (SelectSwitchEquip.value)
                            {
                                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);
                            }
                        }

                    });

                }
            }
            else
            {
                //提示放入角色
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_PARTNER_TRANS_ADDPARTNERTIP"));
            }
    
        }
    
        private IEnumerator PlayTransFx()
        {
    
            InputBlockerManager.Instance.Block(InputBlockReason.SCREEN_TRANSITION_MASK, 2.3f);
            LeftFx.CustomSetActive(true);
            RightFx.CustomSetActive(true);
            yield return new WaitForSeconds(1.2f);
            ReflashTransCard();
            yield return new WaitForSeconds(1.0f);
            LeftFx.CustomSetActive(false);
            RightFx.CustomSetActive(false);
            InitTransCost();
        }
        public void OnClickOutTeam(Transform parent, UILabel label)
        {
           
            if (parent.childCount > 0)
            {
                int positionNum = parent.position == judgeposl ? 0 : 1;         
                tempTransData[positionNum] = null;
                ReflashTransCard();
                InitTransCost();          
                RefreshPartnerList(mCurPartnerTabType);
            }
        }
        public void OnRaceTabClick(GameObject obj)
        {
            if (isOnDrag)
            {
                DragEndFunc();
            }          
            eAttrTabType tabType = ParseTabType(obj.name);
            RefreshPartnerList(tabType);
        }
    
    
        public void OnAttrInfoBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.5f);
            GlobalMenuManager.Instance.Open("LTAttributeInfo");
        }
    
        private eAttrTabType ParseTabType(string str)
        {
            if (str.Contains("All"))
                return eAttrTabType.All;
            else if (str.Contains("Feng"))
                return eAttrTabType.Feng;
            else if (str.Contains("Huo"))
                return eAttrTabType.Huo;
            else if (str.Contains("Shui"))
                return eAttrTabType.Shui;
           EB.Debug.LogError("ParseTabType error str={0}", str);
            return eAttrTabType.All;
        }
        private void InitTransCost()
        {
            int tempcost = 0;
            currCost = 0;
            for (int i = 0;i<tempTransData.Length; i++)
            {
                if(tempTransData[i] != null)
                {
                    tempcost = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetSwitchCostByUpGrade(tempTransData[i].UpGradeId);
                    if (tempcost > currCost)
                    {
                        currCost = tempcost;
                    }
                }
                
            }
            SetPriceLabel(currCost == 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isfree"></param>
        private void SetPriceLabel(bool isfree)
        {
            //免费
            if (isfree)
            {
                pricefreelabel.CustomSetActive(true);
                transPriceLabel.text = "";
                DiamondSprite.gameObject.CustomSetActive(false);
            }
            else
            {
                pricefreelabel.CustomSetActive(false);
                DiamondSprite.gameObject.CustomSetActive(true); 
                //是否有转换券
                if (tranReplaceStone>0)
                {
                    DiamondSprite.spriteName = "Goods_Prop_1428";
                    transPriceLabel.text = tranReplaceStone+"/1";
                    transPriceLabel.color = Color.green;
                }
                else
                {
                    DiamondSprite.spriteName = "Ty_Icon_Jewel";
                    transPriceLabel.text = currCost.ToString();
                    transPriceLabel.color = Color.white;
                }
            }
        }
    
        #region drag
        public float DRAG_Z = 0f;
        public float MIN_DRAG_DIST = 0.5f;
        public float DRAG_OFFSET_DIST = 0.2f;
        public float MIN_DRAG_IN_DIST = 0.5f;
        public float CHALLENGE_MIN_DRAG_IN_DIST = 0.2f;
        public Transform judgePosLeft;
        public Transform judgePosRight;
        public CombatPartnerCellController leftIcon;
        public CombatPartnerCellController rightIcon;
        //private HashSet<string> resourceList = new HashSet<string>();
        //private eDragType curDragType = eDragType.None;
    
        private void SetIconDragAction()
        {
            DynamicScroll.SetItemDragStartAction(OnDragStartByIcon);
            DynamicScroll.SetItemDragAction(OnDrag);
            DynamicScroll.SetItemDragEndAction(DragEndFunc);
        }
    
        public void DragEndFunc()
        {
            isOnDrag = false;
            SetJudgePos();
            float dragcellX = DragPartnerCell.mDMono.gameObject.transform.position.x;
            float dragcellY = DragPartnerCell.mDMono.gameObject.transform.position.y;
            if ((dragcellX > judgeposl.x - halfjudgewidth) && (dragcellX < judgeposl.x + halfjudgewidth) && (dragcellY > judgeposl.y - halfjudgeheight) && (dragcellY < judgeposl.y + halfjudgeheight))
            {
                //实例化
                ReleaseInTransArea(judgePosLeft.transform);           
                InitTransCost();
                return;
            }
            else if ((dragcellX > judgeposr.x - halfjudgewidth) && (dragcellX < judgeposr.x + halfjudgewidth) && (dragcellY > judgeposr.y - halfjudgeheight) && (dragcellY < judgeposr.y + halfjudgeheight))
            {
                ReleaseInTransArea(judgePosRight.transform);          
                InitTransCost();
                return;
            }
            else
            {
                StartCoroutine(MoveToTarget(tempWorldVec));
                //返回原地，刷新队列；
            }
    
    
        }
    
        /// <summary>
        /// 拖入转换框
        /// </summary>
        private void ReleaseInTransArea(Transform target)
        {
            
            ToTransTeam(target);
            ReflashTransCard();
    
        }
        /// <summary>
        /// 实例化卡片
        /// </summary>
        /// <param name="parent"></param>
        public void ReflashTransCard()
        {
            
            if (tempTransData[0] != null)
            {
                leftIcon.Fill( LTPartnerDataManager.Instance.GetPartnerByHeroId(tempTransData[0].HeroId));
                leftNameLabel.text = leftIcon.ItemData.HeroInfo.name;
                leftIcon.mDMono.gameObject.CustomSetActive(true);
            }
            else
            {
                leftIcon.mDMono.gameObject.CustomSetActive(false);
                leftNameLabel.text = EB.Localizer.GetString("ID_PARTNER_TRANSTIPS");
            }
            if (tempTransData[1] != null)
            {
                rightIcon.Fill(LTPartnerDataManager.Instance.GetPartnerByHeroId(tempTransData[1].HeroId));
                rightNameLabel.text = rightIcon.ItemData.HeroInfo.name;
                rightIcon.mDMono.gameObject.CustomSetActive(true);
            }
            else
            {
                rightIcon.mDMono.gameObject.CustomSetActive(false);
                rightNameLabel.text = EB.Localizer.GetString("ID_PARTNER_TRANSTIPS");
            }
            
            DragPartnerCell.mDMono.gameObject.CustomSetActive(false);
    
        }
        public float MoveSpeed = 0.05f;
        IEnumerator MoveToTarget(Vector3 targetPos)
        {
    
            Vector3 dir = (targetPos - DragPartnerCell.mDMono.transform.position).normalized;
            float dis = (targetPos - DragPartnerCell.mDMono.transform.position).sqrMagnitude;
            while (dis > 0.05)
            {
                DragPartnerCell.mDMono.transform.position += dir * Time.deltaTime * MoveSpeed;
                dis = (targetPos - DragPartnerCell.mDMono.transform.position).sqrMagnitude;
                yield return new WaitForSeconds(0.02f);
            }
            DragPartnerCell.mDMono.transform.position = targetPos;
            DragPartnerCell.mDMono.gameObject.CustomSetActive(false);
            RefreshPartnerList(mCurPartnerTabType);
            yield return null;
        }
        private float halfjudgewidth;
        private float halfjudgeheight;
        private Vector3 judgeposl;
    
        private Vector3 judgeposr;
        private void SetJudgePos()
        {
            float scaleRatio = NGUITools.FindInParents<UIRoot>(controller.transform).transform.localScale.x;
            halfjudgewidth = judgePosLeft.GetComponent<UIWidget>().width / 2 * scaleRatio;
            halfjudgeheight = judgePosLeft.GetComponent<UIWidget>().height / 2 * scaleRatio;
            judgeposl = judgePosLeft.position;
            judgeposr = judgePosRight.position;
        }
        private void SetParent(Transform target, Transform parent)
        {
            target.SetParent(parent);
            target.localPosition = Vector3.zero;
            target.localScale = Vector3.one;
        }
    
    
        private void SetVector(float x, float y, float z, Transform go)
        {
            Vector3 curr = go.position;
            curr.x = x;
            curr.y = y;
            curr.z = z;
            go.position = curr;
        }
        //private void SetVector(float x, float y, out Vector2 vec)
        //{
        //    vec.x = x;
        //    vec.y = y;
        //}
        private bool isOnDrag = false;
        public void OnDrag()
        {
            if (isOnDrag)
            {
                SetVector(UICamera.lastWorldPosition.x, UICamera.lastWorldPosition.y, tempCell.mDMono.transform.position.z, DragPartnerCell.mDMono.transform);
            }        
        }
    
    
        //void PlayEntryAction(GameObject variantOBJ)
        //{
        //    if (variantOBJ.activeInHierarchy)
        //    {
        //        ILRTimerManager.instance.AddFramer(2, 1, OnFrameUpHandler, variantOBJ);
        //    }
        //}
    
        //private void OnFrameUpHandler(int seq, object arg)
        //{
        //    var variantOBJ = arg as GameObject;
        //    if (variantOBJ == null)
        //    {
        //        return;
        //    }
        //    CharacterVariant cv = variantOBJ.GetComponent<CharacterVariant>();
        //    MoveController mc = cv.CharacterInstance.GetComponent<MoveController>();
        //    mc.TransitionTo(MoveController.CombatantMoveState.kEntry);
        //    mc.CrossFade(MoveController.m_entry_hash, 0.2f, 0, 0f);
    
        //    StartCoroutine(TransitionToIdleState(mc));
        //}
    
    
        //IEnumerator TransitionToIdleState(MoveController mc)
        //{
        //    yield return null;
        //    yield return null;
    
        //    if ((int)mc.CurrentState == (int)MoveController.CombatantMoveState.kEntry)
        //    {
        //        while (mc.GetCurrentStateInfo().normalizedTime < 1)
        //        {
        //            yield return null;
        //        }
        //        mc.TransitionTo(MoveController.CombatantMoveState.kIdle);
        //        mc.CrossFade(MoveController.m_idle_hash, 0.2f, 0, 0f);
        //    }
    
        //}
    
        private Vector3 tempVec;
        public Vector3 tempWorldVec;
        private CombatPartnerCellController tempCell;
        private LTPartnerData partnerData;
        /// <summary>
        /// 拖拽Icon上阵开始
        /// </summary>
        /// <param name="partnerCell"></param>
        public void OnDragStartByIcon(CombatPartnerCellController partnerCell)
        {
            if (isOnDrag)return;
            isOnDrag = true;
            partnerData = partnerCell.ItemData;
           EB.Debug.Log(partnerData.HeroId);
            tempCell = partnerCell;
            tempWorldVec = partnerCell.mDMono.transform.position;
            tempVec = partnerCell.mDMono.transform.localPosition;
            tempCell.mDMono.transform.localPosition = new Vector3(tempVec.x, tempVec.y + 1000, tempVec.z);
    
            DragPartnerCell.mDMono.gameObject.CustomSetActive(true);
            DragPartnerCell.Fill(partnerData);
            DragPartnerCell.mDMono.transform.position = new Vector3(partnerCell.mDMono.transform.position.x, partnerCell.mDMono.transform.position.y, DRAG_Z);
            OnDrag();       
        }
    
    
        WaitForSeconds wait01 = new WaitForSeconds(0.1f);
        private IEnumerator RepositionCell()
        {
            yield return wait01;
        }
        #endregion
    
        /// <summary>
        /// 摧毁对象
        /// </summary>
    	public override void OnDestroy()
        {
           EB.Debug.Log("Ondestroy");
            //resourceList.Clear();
            base.OnDestroy();
        }
    }
}
