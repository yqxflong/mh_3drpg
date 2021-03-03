using UnityEngine;
using System;

namespace Hotfix_LT.UI
{
    public class FinalPlayerItem : DynamicMonoHotfix {
        public override void Awake() {
            base.Awake();
            if (mDMono.ObjectParamList != null)
            {
                int count = mDMono.ObjectParamList.Count;

                if (count > 0 && mDMono.ObjectParamList[0])
                {
                    Empty = (GameObject)mDMono.ObjectParamList[0];
                }
                if (count > 1 && mDMono.ObjectParamList[1])
                {
                    Filled = (GameObject)mDMono.ObjectParamList[1];
                }
                if (count > 2 && mDMono.ObjectParamList[2])
                {
                    Dead = (GameObject)mDMono.ObjectParamList[2];
                }
                if (count > 3 && mDMono.ObjectParamList[3])
                {
                    Replace = (GameObject)mDMono.ObjectParamList[3];
                    Replace.GetComponent<UIButton>().onClick.Add(new EventDelegate(ReplacePerson));
                }
                if (count > 4 && mDMono.ObjectParamList[4])
                {
                    JoinBtn = (GameObject)mDMono.ObjectParamList[4];
                    JoinBtn.GetComponent<UIButton>().onClick.Add(new EventDelegate(Join));
                }
                if (count > 5 && mDMono.ObjectParamList[5])
                {
                    Icon = ((GameObject)mDMono.ObjectParamList[5]).GetComponentEx<UISprite>();
                }
                if (count > 6 && mDMono.ObjectParamList[6])
                {
                    Name = ((GameObject)mDMono.ObjectParamList[6]).GetComponentEx<UILabel>();
                }
                if (count > 7 && mDMono.ObjectParamList[7])
                {
                    LadderRank = ((GameObject)mDMono.ObjectParamList[7]).GetComponentEx<UILabel>();
                }
                if(count >8&& mDMono.ObjectParamList[8])
                {
                    InfoLabel = ((GameObject)mDMono.ObjectParamList[8]).GetComponentEx<UILabel>();
                }

                if(Icon!=null)
                {
                    Frame = Icon.transform .Find("Frame").GetComponent <UISprite>();
                }
            }

            if (mDMono.BoolParamList != null && mDMono.BoolParamList.Count > 0)
            {
                ishomeTeam = mDMono.BoolParamList[0];
            }
            if (mDMono.IntParamList != null && mDMono.IntParamList.Count > 0)
            {
                CellPosition = mDMono.IntParamList[0];
            }

            Transform t = mDMono.transform;
            t.GetComponent<UIButton>("GM/+").onClick.Add(new EventDelegate(AddRobByGM));
            t.GetComponent<UIButton>("GM/-").onClick.Add(new EventDelegate(RemoveRobByGM));
        }

        public GameObject Empty, Filled,Dead,Replace,JoinBtn;
        public UISprite Icon, Frame;
        public UILabel Name, LadderRank,InfoLabel;
        public int CellPosition;
    
        public FinalPlayerData ItemData;
        public bool ishomeTeam;
        public void SetData(FinalPlayerData Data) {
    
            Vector3 position = mDMono.transform.parent.localPosition;
            position.x = mDMono.transform.parent.localPosition.x > 0 ? 1700 : -1700;
            mDMono.transform.parent.localPosition = position;
    
            ItemData = Data?? new FinalPlayerData();
    
            if (ItemData == null||ItemData.Name == null) {
                Empty.CustomSetActive(true);
                Filled.CustomSetActive(false);
                SetMysterious(true);
            }
            else {
                Empty.CustomSetActive(false);
                Filled.CustomSetActive(true);
                Replace.CustomSetActive(false);
                Icon.spriteName = ItemData.IconName;
                Frame.spriteName = ItemData.FrameName;
                Name.text = Name.transform.GetChild(0).GetComponent<UILabel>().text = ItemData.Name;
                LadderRank.text = LadderRank.transform.GetChild(0).GetComponent<UILabel>().text =string.Format(EB.Localizer.GetString("ID_LADDER_RANK") +":[fff348]{0}", (ItemData.LadderRank > 0 ? ItemData.LadderRank.ToString() : EB.Localizer.GetString("ID_ARENA_RANK_OUT_OF_RANGE")));
                Dead.CustomSetActive(ItemData.Dead);
                
                SetMysterious();
            }
        }
        
        private void SetMysterious(bool isNullItem=false)
        {
            if (InfoLabel != null)
            {
                InfoLabel.text = InfoLabel.transform.GetChild(0).GetComponent<UILabel>().text = (isNullItem) ? EB.Localizer.GetString("ID_codefont_in_FinalPlayerItem_2871_1") : EB.Localizer.GetString("ID_codefont_in_FinalPlayerItem_2871_2");
            }
        }
    
        public void SetInfo(bool CanShow)
        {
            InfoLabel.gameObject.CustomSetActive(CanShow);
            Name.gameObject.CustomSetActive(!CanShow);
            LadderRank.gameObject.CustomSetActive(!CanShow);
        }
    
        public void SetReplaceButton(bool CanReplace)
        {
            if (JoinBtn.activeSelf != CanReplace)
            {
                JoinBtn.CustomSetActive(CanReplace);
                if(ItemData!= null && ItemData.uid == LoginManager.Instance.LocalUserId.Value)
                {
                    Replace.CustomSetActive(false);
                }
                else
                {
                    Replace.CustomSetActive(CanReplace);
                }
               
            }
        }
    
        public void StartBootFlash()
        {
            UITweener tweener = mDMono.transform.parent.GetComponent<UITweener>();        
            tweener.ResetToBeginning();
            tweener.PlayForward();
        }
    
        public void ResetFlash()
        {
            UITweener tweener = mDMono.transform.parent.GetComponent<UITweener>();
            tweener.ResetToBeginning();
        }
    
        public void ReplacePerson() {
    
            if (LadderManager.Instance.Info.Rank==0||(ItemData.LadderRank!=0&& LadderManager.Instance.Info.Rank >= ItemData.LadderRank))
            {
                MessageTemplateManager .ShowMessage (eMessageUIType .FloatingText ,EB.Localizer.GetString("ID_codefont_in_FinalPlayerItem_3368"));
                return;
            }
            int ts = 0;
            if (LTLegionWarTimeLine.Instance != null)
            {
                ts  = (int)(((LTLegionWarFinalController._WarType == WarType.Semifinal) ? LTLegionWarTimeLine.Instance.SemiFinalStopTime 
                    : LTLegionWarTimeLine.Instance.FinalStopTime) - EB.Time.Now);
            }
            if (ts <= 0)
            {
                return;
            }   
            if (LTLegionWarManager.Instance.FinalPlayerDataList.MyWarField!=LegionWarField.None)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, string.Format(EB.Localizer.GetString("ID_codefont_in_FinalPlayerItem_3651"), 
                    GetFieldName(LTLegionWarManager .Instance .FinalPlayerDataList.MyWarField),
                    GetFieldName(LTLegionWarFinalController.Instance.WarFiled)), delegate (int r)
                {
                    if (r == 0)
                    {
                        GoIntoBattle();
                    }
                });
                return;
            }
            GoIntoBattle();
        }
    
        public void Join() {
            //布阵先
            if (LTLegionWarFinalController.Instance.ThisFinalStatus.Status !=2)
            {
                EB.Debug.Log(EB.Localizer.GetString("ID_codefont_in_FinalPlayerItem_4620"));
                return;
            }
            int ts = 0;
            if (LTLegionWarTimeLine.Instance != null)
            {
                ts = (int)(((LTLegionWarFinalController._WarType == WarType.Semifinal) ? LTLegionWarTimeLine.Instance.SemiFinalStopTime : LTLegionWarTimeLine.Instance.FinalStopTime) - EB.Time.Now);
            }
            if (ts <= 0)
            {
                return;
            }   
            if (LTLegionWarManager.Instance.FinalPlayerDataList.MyWarField != LegionWarField.None)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, string.Format(EB.Localizer.GetString("ID_codefont_in_FinalPlayerItem_3651"), GetFieldName(LTLegionWarManager.Instance.FinalPlayerDataList.MyWarField), GetFieldName(LTLegionWarFinalController.Instance.WarFiled)), delegate (int r)
                {
                    if (r == 0)
                    {
                        GoIntoBattle();
                    }
                });
                return;
            }
            GoIntoBattle();
        }
    
        private  void GoIntoBattle()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            System.Action startCombatCallback = delegate () {
                LTLegionWarManager.Instance.Api.ErrorRedultFunc = (EB.Sparx.Response response) =>
                {
                    if (response.str != null)
                    {
                        switch ((string)response.str)
                        {
                            case "target is protected":
                                {
                                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_FlowEnemyHud_3732"));
                                    return true;
                                };
                            case "NotReady":
                                {
                                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_FlowEnemyHud_3986"));
                                    return true;
                                };
                        }
                    }
                    if (response.error != null)
                    {
                        switch ((string)response.error)
                        {
                            case "Error: service not found":
                                {
                                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_62898"));
                                    return true;
                                };
                        }
                    }
                    return false;
                };
                LTLegionWarManager.Instance.goIntoBattle(LTLegionWarManager.Instance.SemiFinalField, (int)(LTLegionWarFinalController.Instance.WarFiled), this.CellPosition);
            };
            BattleReadyHudController.Open(eBattleType.AllieanceFinalBattle, startCombatCallback);
        }
        
        private string GetFieldName(LegionWarField WarFiled)
        {
            switch (WarFiled) {
                case LegionWarField.Water: return EB.Localizer.GetString("ID_codefont_in_FinalPlayerItem_5856");
                case LegionWarField.Fire: return EB.Localizer.GetString("ID_codefont_in_FinalPlayerItem_5957");
                default:return EB.Localizer.GetString("ID_codefont_in_FinalPlayerItem_5997");
            }
        }
    
        public void ShowWatchLogInfo(long win,long lose)
        {
            if (ItemData == null || ItemData.Name == null || ItemData.uid == 0)
            {
                return;
            }
            else
            {
                if (ItemData.uid !=0&& ItemData.uid==lose)
                {
                    Dead.CustomSetActive(true);
                    StartBootFlash();
                }
                if (ItemData.uid != 0 && ItemData.uid == win)
                {
                    StartBootFlash();
                }
            }
        }
    
        #region GM
        public void AddRobByGM()
        {
            string camp = ishomeTeam ? "hometeam" : "awayteam";
            LTLegionWarManager.Instance.GetAddBot(LTLegionWarManager.Instance.SemiFinalField, (int)(LTLegionWarFinalController.Instance.WarFiled), this.CellPosition, camp);
        }
        public void RemoveRobByGM()
        {
            string camp = ishomeTeam ? "hometeam" : "awayteam";
            LTLegionWarManager.Instance.GetRemoveBot(LTLegionWarManager.Instance.SemiFinalField, (int)(LTLegionWarFinalController.Instance.WarFiled), this.CellPosition, camp);
        }
        #endregion
    }
}
