using UnityEngine;
using System;

namespace Hotfix_LT.UI
{
    public class FinalRankCell : DynamicMonoHotfix
    {
        public override void Awake() {
            base.Awake();
            var t = mDMono.transform;
            Filled = t.FindEx("FillObj").gameObject;
            Empty = t.FindEx("EmptyObj").gameObject;
            WaitState = t.FindEx("FillObj/NumLabel").gameObject;
            Name = t.GetComponent<UILabel>("FillObj/NameLabel");
            Icon = t.GetComponent<UISprite>("FillObj/Badge/LegionIcon");
            IconBG = t.GetComponent<UISprite>("FillObj/Badge/IconBG");
            EnterIcon = t.GetComponent<UISprite>("FillObj/Result");         
            if (mDMono.ObjectParamList != null && mDMono.ObjectParamList.Count > 0)
            {
                Enemy = ((GameObject)mDMono.ObjectParamList[0]).GetMonoILRComponent<FinalRankCell>();
            }
            if(mDMono.IntParamList!= null&& mDMono.IntParamList.Count > 0)
            {
                type = (WarType)mDMono.IntParamList[0];
            }
        }


    
        public GameObject Filled, Empty,WaitState;
        public UILabel  Name;
        public UISprite Icon, IconBG, EnterIcon;
        public LegionRankData item;
        public FinalRankCell Enemy;
    
        public WarType type = WarType.Qualify;
    
        public void Fill() {
            if (item==null||item.Name == null) {
                Filled.CustomSetActive(false);
                Empty.CustomSetActive(true);
            }
            else {
                Filled.CustomSetActive(true);
                Empty.CustomSetActive(false);
                Name.text = Name.transform.GetChild(0).GetComponent<UILabel>().text = item.Name;
                if (item!=null&&item.enter|| Enemy.item != null && Enemy.item.enter)SetEndState(true);
                else SetEndState(false);
                SetIcon(item.Icon);
            }
        }
    
        public void SetIcon(string iconStr)
        {
            int iconID = 0;
            int.TryParse(iconStr, out iconID);
            int legionIconIndex = iconID % 100;
            int legionBgIconIndex = iconID / 100;
            if (LegionModel.GetInstance().dicLegionSpriteName.ContainsKey(legionIconIndex))
            {
                Icon.spriteName = LegionModel.GetInstance().dicLegionSpriteName[legionIconIndex];
            }
            if (LegionModel.GetInstance().dicLegionBGSpriteName.ContainsKey(legionBgIconIndex))
            {
                IconBG.spriteName = LegionModel.GetInstance().dicLegionBGSpriteName[legionBgIconIndex];
            }
        }
    
        public void inWarFill(LegionRankData data)
        {
            item = data;
            if (item == null || item.Name == null)
            {
                Filled.CustomSetActive(false);
                Empty.CustomSetActive(true);
            }
            else
            {
                Filled.CustomSetActive(true);
                Empty.CustomSetActive(false);
                Name.text = Name.transform.GetChild(0).GetComponent<UILabel>().text = item.Name;
                SetIcon(item.Icon);
            }
        }
    
        public void SetCurStatu(bool isEnd, bool isWin = false)
        {
            if (item == null || item.Name == null) return;
            EnterIcon.spriteName = (isWin) ? "Ty_Legion_Shengli" : "Ty_Legion_Taotai";
            EnterIcon.gameObject.CustomSetActive(isEnd);
            WaitState.CustomSetActive(!isEnd);
        }
    
        public void SetEndState(bool endState)
        {
            if (item == null || item.Name == null) return;
            EnterIcon.spriteName =(item.enter) ? "Ty_Legion_Shengli" : "Ty_Legion_Taotai";
            EnterIcon.gameObject.CustomSetActive(endState);
            if (!endState)
            {
                switch (type)
                {
                    case WarType.Semifinal:
                        {
                            int ts = -1;
                            if (LTLegionWarTimeLine.Instance != null)
                            {
                                ts = Convert.ToInt32(LTLegionWarTimeLine.Instance.SemiFinalStopTime - EB.Time.Now);
                            }
                            WaitState.GetComponent<UILabel>().text = WaitState.transform.GetChild(0).GetComponent<UILabel>().text = ts< 0 ? EB.Localizer.GetString("ID_codefont_in_LTLegionWarFinalController_4204") : EB.Localizer.GetString("ID_uifont_in_LTLegionWarJoin_StateLabel_5");
                        }; break;
                    case WarType.Final:
                        {
                            int ts = -1;
                            if (LTLegionWarTimeLine.Instance != null)
                            {
                                ts = Convert.ToInt32(LTLegionWarTimeLine.Instance.FinalStopTime - EB.Time.Now);
                            }
                            WaitState.GetComponent<UILabel>().text = WaitState.transform.GetChild(0).GetComponent<UILabel>().text = ts < 0 ? EB.Localizer.GetString("ID_codefont_in_LTLegionWarFinalController_4204") : EB.Localizer.GetString("ID_uifont_in_LTLegionWarJoin_StateLabel_5");
                        }; break;
                    default: {
                            WaitState.GetComponent<UILabel>().text = WaitState.transform.GetChild(0).GetComponent<UILabel>().text = string.Empty;
                        } break;
                }
            }
            WaitState.CustomSetActive(!endState);
        }
    }
}
