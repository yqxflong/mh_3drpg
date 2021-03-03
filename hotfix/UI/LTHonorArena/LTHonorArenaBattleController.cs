using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Debug = EB.Debug;

namespace Hotfix_LT.UI
{
    public static class HonorConfig{
        public static string[] teamName1 = new string[3]{"honor_attack_1","honor_attack_2","honor_attack_3"};
        public static string[] teamName2 = new string[3]{"honor_defend_1","honor_defend_2","honor_defend_3"};
        public static int PERNUM = 6;
        public static int ALLNUM = 18;
        public static string GetDefendName(int pos)
        {
           return teamName2[pos/PERNUM];
        }
        public static string GetAttackName(int pos)
        {
            return teamName1[pos/PERNUM];
        }
    }
    
    public class LTHonorArenaBattleController:UIControllerHotfix
    {
        public UIButton checkEnemyBtn;
        public UIButton startBtn;
        public GameObject startBtnPanel;
        public List<UIButton> tranBtns;
        private List<GameObject> tranBtnsFx;
        public BoxCollider BottomBoxCollider;
        public List<Transform> judgePosList;
        public List<CombatPartnerCellController> cellIcons;
        public CombatPartnerDynamicScroll DynamicScroll;
        public CombatPartnerCellController DragPartnerCell;
        private eAttrTabType mCurPartnerTabType = eAttrTabType.All;
        private int currCost = 0;
        private static int judgeCount=18;
        int dragIndex = 0;
        private bool isDefend;
        private bool isFast;
        private int challengeIndex;
        private string[] currentTeamName;
        private int PERNUM =HonorConfig.PERNUM;
        
        public static void Open(bool isDef,int index,bool isFast)
        {
            Hashtable table = new Hashtable();
            table.Add("isDef",isDef);
            table.Add("index",index);
            table.Add("isFast",isFast);
            //打开布阵界面
            BattleReadyHudController.SetBattleType(eBattleType.HonorArena);
            GlobalMenuManager.Instance.Open("LTHonorArenaBattleUI",table);
        }

        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            
            judgePosList = controller.FetchComponentList<Transform>(GetArray(
                "Center/One/Center/Container", "Center/One/Center/Container (1)", "Center/One/Center/Container (2)", 
                "Center/One/Center/Container (3)", "Center/One/Center/Container (4)", "Center/One/Center/Container (5)",
                "Center/One (1)/Center/Container", "Center/One (1)/Center/Container (1)", "Center/One (1)/Center/Container (2)", 
                "Center/One (1)/Center/Container (3)", "Center/One (1)/Center/Container (4)", "Center/One (1)/Center/Container (5)",
                "Center/One (2)/Center/Container", "Center/One (2)/Center/Container (1)", "Center/One (2)/Center/Container (2)", 
                "Center/One (2)/Center/Container (3)", "Center/One (2)/Center/Container (4)", "Center/One (2)/Center/Container (5)"));
            cellIcons = new List<CombatPartnerCellController>();
            foreach (var judgeItem in judgePosList)
            {
                CombatPartnerCellController temp = judgeItem.GetMonoILRComponent<CombatPartnerCellController>("DragPartnerItem");
                // judgeItem.GetComponent<ContinueClickCDTrigger>().m_CallBackPress.Add(new EventDelegate(() => OnClickOutTeam(judgeItem)));
                cellIcons.Add(temp);
                DragEventDispatcher DragEventDispatcher = judgeItem.GetComponent<DragEventDispatcher>();
                DragEventDispatcher.onDragStartFunc.Add(new EventDelegate(() => { OnDragStartFromCenter(judgeItem); }));
                DragEventDispatcher.onDragFunc.Add(new EventDelegate(OnDrag));
                DragEventDispatcher.onDragEndFunc.Add(new EventDelegate(OnDragEndFromCenter));
            }
            
            BottomBoxCollider = t.GetComponent<BoxCollider>("Edge/Bottom/BuddyList/Placeholder");
            DynamicScroll = t.GetMonoILRComponent<CombatPartnerDynamicScroll>("Edge/Bottom/BuddyList/Placeholder/PartnerGrid");
            DragPartnerCell = t.GetMonoILRComponent<CombatPartnerCellController>("Edge/DragPanel/DragPartnerItem");

            tranBtns = controller.FetchComponentList<UIButton>(GetArray("Center/One/Tran","Center/One (1)/Tran","Center/One (2)/Tran"));
            tranBtns[0].onClick.Add(new EventDelegate(() => { TranBtnClick(tranBtns[0],0);}));
            tranBtns[1].onClick.Add(new EventDelegate(() => { TranBtnClick(tranBtns[1],1);}));
            tranBtns[2].onClick.Add(new EventDelegate(() => { TranBtnClick(tranBtns[2],2);}));
            tranBtnsFx = new List<GameObject>();
            for (int i = 0; i < tranBtns.Count; i++)
            {
                tranBtnsFx.Add(tranBtns[i].transform.Find("FX").gameObject);
            }
            checkEnemyBtn = t.GetComponent<UIButton>("Edge/TopRight/CheckEnemyFormationLabel");
            checkEnemyBtn.onClick.Add(new EventDelegate(OnCheckEnemyBtnClick));
            startBtnPanel = t.Find("Edge/Bottom/StartBtnPanel").gameObject;
            startBtn = t.GetComponent<UIButton>("Edge/Bottom/StartBtnPanel/BG/StartBattleBtn");        
            startBtn.onClick.Add(new EventDelegate(OnStartBtnClick));
            t.GetComponent<UIButton>("Edge/Bottom/BG/RuleBtn").onClick.Add(new EventDelegate(OnAttrInfoBtnClick));
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
        }

        private void OnCheckEnemyBtnClick()
        {
            if (HonorArenaManager.Instance.CurrentChallenger!=null)
            {
                HonorArenaChallenger challenger = HonorArenaManager.Instance.CurrentChallenger;
                LTOtherInfoController.Open(TitleType.Enemy_Title,challenger._id,challenger.name,challenger.level,challenger.worldId,challenger.point,challenger.br);
            }
        }

        private bool _isStartChallengeRequesting = false;

        private void OnStartBtnClick()
        {
            if (_isStartChallengeRequesting)
            {
                return;
            }

            _isStartChallengeRequesting = true;

            if (LTFormationDataManager.Instance.IsArenaPartnerDataEmpty(currentTeamName))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_HONOR_ARENA_RAKN_BATTLE_TIP"), null);
                _isStartChallengeRequesting = false;
            }
            else
            {
                ReportPower();
                HonorArenaManager.Instance.StartChallenge(challengeIndex, isFast);
                TimerManager.instance.AddTimer(3000, 1, timerSequence => _isStartChallengeRequesting = false);
            }
        }

        public void ReportPower()
        {
            if(isDefend)HonorArenaManager.Instance.setBR();
        }
        
        public override void OnCancelButtonClick() 
        {
            base.OnCancelButtonClick();
            ReportPower();
        }     

        private void TranBtnClick(UIButton tranBtn,int i)
        {
            int j=0;
            UIButton selectBtn = IsSelectOneTran(out j);
            if (selectBtn!=null)
            {
                //请求服务器
                LTFormationDataManager.Instance.SwapFormationPos(currentTeamName[i],currentTeamName[j]
                    , (hs) =>
                    {
                        if (hs!=null)
                        {
                            //内存中交互
                            LTFormationDataManager.Instance.SwapArenaPartnerDataTeam(i,j,isDefend);
                            InitCell();
                        }
                        InitTransState();
                    });
            }
            else
            {
                tranBtn.isEnabled = false;
                //另外两个亮
                for (int n = 0; n < tranBtnsFx.Count; n++)
                {
                    if(i!=n)tranBtnsFx[n].CustomSetActive(true);
                }
            }
        }


        private void InitTransState()
        {
            //全部灭
            for (int n = 0; n < tranBtnsFx.Count; n++)
            {
                tranBtns[n].isEnabled = true;
                tranBtnsFx[n].CustomSetActive(false);
            }
        }
        

        private UIButton IsSelectOneTran(out int index)
        {
            for (int i = 0; i < tranBtns.Count; i++)
            {
                if (tranBtns[i].isEnabled==false)
                {
                    index = i;
                    return tranBtns[i];
                }
            }

            index = 0;
            return null;
        }

        public override bool IsFullscreen()
        {
            return true;
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            Hashtable table = param as Hashtable;
            isDefend = (bool)table["isDef"];
            challengeIndex = (int)table["index"];
            isFast = (bool)table["isFast"];
            currentTeamName = isDefend ? HonorConfig.teamName2 : HonorConfig.teamName1;
            checkEnemyBtn.gameObject.CustomSetActive(!isDefend);
            startBtnPanel.gameObject.CustomSetActive(!isDefend);
            DynamicScroll.mDMono.gameObject.CustomSetActive(true);
            LTPartnerDataManager.Instance.InitPartnerData();
            
            for (int i = 0; i < currentTeamName.Length; i++)
            {
                List<TeamMemberData> temp=LTFormationDataManager.Instance.GetTeamMemList(currentTeamName[i]);
                for (int j = 0; j < temp.Count; j++)
                {
                    if (temp[j]!=null && temp[j].HeroID!=null)
                    {
                        LTFormationDataManager.Instance.SetArenaPartnerData(LTPartnerDataManager.Instance.GetPartnerByHeroId(temp[j].HeroID),i*PERNUM+temp[j].Pos,isDefend);
                        RefreshTransCard(i*PERNUM+temp[j].Pos);
                    }
                }
            }
            RefreshPartnerList(mCurPartnerTabType);
            InitCell();
        }
    
        
        public void OnAttrInfoBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.5f);
            GlobalMenuManager.Instance.Open("LTAttributeInfo");
        }
    
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            UICamera.mainCamera.transform.localPosition = new Vector3(0, 0, 0);
    
            DynamicScroll.mDMono.gameObject.CustomSetActive(false);
            DynamicScroll.Clear();
            DestroySelf();
            yield break;
        }
       
        private void RefreshPartnerList(eAttrTabType tab_type)
        {
            mCurPartnerTabType = tab_type;
            List<LTPartnerData> partnerList = LTPartnerDataManager.Instance.GetOwnPartnerList();
    
            List<LTPartnerData> filterList = new List<LTPartnerData>();
            for (int i = 0; i < partnerList.Count; i++)
            {
                if (LTFormationDataManager.Instance.IsInTransTeam(partnerList[i].HeroId,isDefend))
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
            if (!isDefend)
            {
                filterList.AddRange(new LTPartnerData[2]);
            }
            DynamicScroll.SetItemDatas(filterList.ToArray());
            SetIconDragAction();
        }
        public void OnRaceTabClick(GameObject obj)
        {
            if (isOnDrag)
            {
                OnDragEnd();
            }          
            eAttrTabType tabType = GameUtils.ParseTabType(obj.name);
            RefreshPartnerList(tabType);
        }
        
        #region drag
        private Vector3 tempVec;
        private Vector3 tempWorldVec;
        private CombatPartnerCellController tempCell;
        private LTPartnerData partnerData;
        private bool isOnDrag = false;
        private void OnDrag()
        {
            if (isOnDrag)
            {
                DragPartnerCell.mDMono.transform.SetVector(UICamera.lastWorldPosition.x, UICamera.lastWorldPosition.y, tempCell.mDMono.transform.position.z);
            }        
        }
        private void SetIconDragAction()
        {
            DynamicScroll.SetItemDragStartAction(OnDragStart);
            DynamicScroll.SetItemDragAction(OnDrag);
            DynamicScroll.SetItemDragEndAction(OnDragEnd);
        }
        
        private void OnDragStart(CombatPartnerCellController partnerCell)
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
            DragPartnerCell.mDMono.transform.position = new Vector3(partnerCell.mDMono.transform.position.x, partnerCell.mDMono.transform.position.y, 0);
            OnDrag();       
        }
    
        public void OnDragEnd()
        {
            isOnDrag = false;
            SetJudgePos(judgePosList[0]);
            float dragcellX = DragPartnerCell.mDMono.gameObject.transform.position.x;
            float dragcellY = DragPartnerCell.mDMono.gameObject.transform.position.y;
            for (int i = 0; i < judgePosList.Count; i++)
            {
                Vector3 judgepos = judgePosList[i].position;
                if ((dragcellX > judgepos.x - halfjudgewidth) && (dragcellX < judgepos.x + halfjudgewidth) &&
                    (dragcellY > judgepos.y - halfjudgeheight) && (dragcellY < judgepos.y + halfjudgeheight))
                {
                    OnDrag2ModelPos(partnerData.HeroId, i, true, () => { });
                    //实例化
                    LTFormationDataManager.Instance.GetArenaPartnerData(isDefend)[i] = partnerData;
                    RefreshPartnerList(mCurPartnerTabType);
                    RefreshTransCard(i);
                    return;
                }
            }
            //返回原地，刷新队列；
           StartCoroutine(MoveToTarget(tempWorldVec));
        }
        
        private void OnDragStartFromCenter(Transform target)
        {
            if (isOnDrag) return;
            isOnDrag = true;
            dragIndex = GameUtils.FindComponentListIndex<Transform>(judgePosList, target);
            tempCell = DragPartnerCell;
            DragPartnerCell.mDMono.gameObject.CustomSetActive(true);
            DragPartnerCell.mDMono.transform.position = target.position;
            DragPartnerCell.Fill(LTFormationDataManager.Instance.GetArenaPartnerData(isDefend)[dragIndex]);
            cellIcons[dragIndex].mDMono.gameObject.CustomSetActive(false);
        }
        
        public void OnDragEndFromCenter()
        {
            isOnDrag = false;
            SetJudgePos(judgePosList[0]);
            float dragcellX = DragPartnerCell.mDMono.gameObject.transform.position.x;
            float dragcellY = DragPartnerCell.mDMono.gameObject.transform.position.y;

            if (LTFormationDataManager.Instance.IsArenaPartnerDataEmpty(GetCurrentTeamName(dragIndex))==1)
            {
                RefreshTransCard(dragIndex);
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_HONOR_ARENA_RAKN_BATTLE_TIP"));
                return;
            }
            
            if (BottomBoxCollider.bounds.Contains(UICamera.lastWorldPosition))
            {
                LTFormationDataManager.Instance.RequestRemoveHeroFormation(LTFormationDataManager.Instance.GetArenaPartnerData(isDefend)[dragIndex].HeroId,
                    GetCurrentTeamName(dragIndex), () =>
                    {
                        LTFormationDataManager.Instance.GetArenaPartnerData(isDefend)[dragIndex] = null;
                        DragPartnerCell.mDMono.gameObject.CustomSetActive(false);
                        RefreshPartnerList(mCurPartnerTabType);
                    });
                return;
            }
            
            for (int i = 0; i < judgePosList.Count; i++)
            {
                Vector3 judgepos = judgePosList[i].position;
                if ((dragcellX > judgepos.x - halfjudgewidth) && (dragcellX < judgepos.x + halfjudgewidth) &&
                    (dragcellY > judgepos.y - halfjudgeheight) && (dragcellY < judgepos.y + halfjudgeheight))
                {
                    if (i==dragIndex)
                    { 
                        RefreshTransCard(dragIndex);
                        return;
                    }

                    Action call = () =>
                    {
                        //实例化
                        LTFormationDataManager.Instance.SwapArenaPartnerDataSingle(i, dragIndex, isDefend);
                        RefreshTransCard(i);
                        RefreshTransCard(dragIndex);
                    };
                    if (GetCurrentTeamName(i)==GetCurrentTeamName(dragIndex))
                    {
                        OnDrag2ModelPos(LTFormationDataManager.Instance.GetArenaPartnerData(isDefend)[dragIndex].HeroId,
                            i,true, call);
                    }
                    else
                    {
                        OnDrag2ModelPos(dragIndex,i,call);
                    }
                    return;
                }
            }
            //返回原地
            RefreshTransCard(dragIndex);
        }
        
        public float MoveSpeed = 5f;
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
        
        private void InitCell()
        {
            for (int j = 0; j < LTFormationDataManager.Instance.GetArenaPartnerData(isDefend).Length; j++)
            {
                if (LTFormationDataManager.Instance.GetArenaPartnerData(isDefend)[j] == null)
                {
                    cellIcons[j].mDMono.gameObject.CustomSetActive(false);
                }
                else
                {
                    cellIcons[j].Fill(LTFormationDataManager.Instance.GetArenaPartnerData(isDefend)[j]);
                    cellIcons[j].mDMono.gameObject.CustomSetActive(true);
                }
            }
        }
        
        private void RefreshTransCard(int i)
        {
            LTPartnerData data= LTFormationDataManager.Instance.GetArenaPartnerData(isDefend)[i];
            if (data!=null)
            {
                cellIcons[i].Fill(data);
                cellIcons[i].mDMono.gameObject.CustomSetActive(true);
            }
            else
            {
                cellIcons[i].mDMono.gameObject.CustomSetActive(false);
            }
            DragPartnerCell.mDMono.gameObject.CustomSetActive(false);
        }
        
        
        private float halfjudgewidth;
        private float halfjudgeheight;
        private void SetJudgePos(Transform judgePos)
        {
            float scaleRatio = NGUITools.FindInParents<UIRoot>(controller.transform).transform.localScale.x;
            halfjudgewidth = judgePos.GetComponent<UIWidget>().width / 2 * scaleRatio;
            halfjudgeheight = judgePos.GetComponent<UIWidget>().height / 2 * scaleRatio;
        }
    
        #endregion

        /// <summary>
        /// 请求上阵
        /// </summary>
        /// <param name="heroID">上阵英雄Id</param>
        /// <param name="index">上阵位置0-18</param>
        /// <param name="isNeedReq"></param>
        /// <param name="isHire"></param>
        private void OnDrag2ModelPos(int heroID, int index, bool isNeedReq,Action callback)
        {
            int real = index % PERNUM;
            if (isNeedReq)
            {
                LTFormationDataManager.Instance.RequestDragHeroToFormationPos(heroID, real, GetCurrentTeamName(index), callback);
            }
        }
        
        private void OnDrag2ModelPos(int fromIndex, int toIndex,Action callback)
        {
            string fromTeamName = GetCurrentTeamName(fromIndex);
            string toTeamName = GetCurrentTeamName(toIndex);
            int fromHeroId = LTFormationDataManager.Instance.GetArenaPartnerData(isDefend)[fromIndex].HeroId;
            if (LTFormationDataManager.Instance.GetArenaPartnerData(isDefend)[toIndex]==null)
            {
                //第二个为空 先下阵第一个
                LTFormationDataManager.Instance.RequestRemoveHeroFormation(fromHeroId,
                    GetCurrentTeamName(fromIndex), () => { OnDrag2ModelPos(fromHeroId,toIndex,true,
                        callback); });
            }
            else
            {
                int toHeroId = LTFormationDataManager.Instance.GetArenaPartnerData(isDefend)[toIndex].HeroId;
                LTFormationDataManager.Instance.RequestDragHeroToOtherTeam(fromHeroId, fromIndex%PERNUM, fromTeamName, toHeroId, toIndex%PERNUM, toTeamName, callback);
            }
          
        }

        private string GetCurrentTeamName(int index)
        {
            int real = index / PERNUM;
            return currentTeamName[real];
        }
    }
}