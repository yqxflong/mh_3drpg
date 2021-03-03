using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTPlayerLvUpController : UIControllerHotfix
    {
        public static bool m_Open;
        public override bool IsFullscreen() { return true; }
        public UILabel CurLevelLabel;
        public UILabel LevelLabel;
        public UILabel CurVigerLabel;
        public UILabel VigerLabel;
        public GameObject info1;
        public GameObject info2;
        public UIPanel Func;
        public GameObject EmptyObj;
        public GameObject TipObj;
        public LTLevelUpFuncScroll FuncScroll;

        public GameObject FXObject;

        private System.Action CallBackFunc = null;
        private bool isFuncEmpty = false;
        private bool isLimit = false;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            m_Open = false;
            CurLevelLabel = t.GetComponent<UILabel>("Center/Content/Info1/CurLevelLabel");
            LevelLabel = t.GetComponent<UILabel>("Center/Content/Info1/NextLevelLabel");
            CurVigerLabel = t.GetComponent<UILabel>("Center/Content/Info2/CurVigerLabel");
            VigerLabel = t.GetComponent<UILabel>("Center/Content/Info2/NextVigerLabel");
            info1 = t.FindEx("Center/Content/Info1").gameObject;
            info2 = t.FindEx("Center/Content/Info2").gameObject;
            Func = t.GetComponent <UIPanel>("Center/Content/FuncInfo");
            EmptyObj = t.FindEx("Center/Content/EmptyLabel").gameObject;
            TipObj = t.FindEx("Center/Tip").gameObject;
            FuncScroll = t.GetMonoILRComponent<LTLevelUpFuncScroll>("Center/Content/FuncInfo/Placeholder/Grid");

            t.GetComponent<ConsecutiveClickCoolTrigger>("Bg").clickEvent.Add(new EventDelegate(OnCancelButtonClick));
            FXObject= t.Find("Bg/FX").gameObject;
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            CallBackFunc = param as System.Action;
            isLimit = true;
            TipObj.CustomSetActive(false);
            FXObject.CustomSetActive(true);
        }

        public override IEnumerator OnAddToStack()
        {
            m_Open = true;
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 1f);
            yield return base.OnAddToStack();
            int[] temp = EB.Sparx.Hub.Instance.LevelRewardsManager.GetLevelupXpInfo();
            int curlevel = EB.Sparx.Hub.Instance.LevelRewardsManager.CurLevel;
            int level = EB.Sparx.Hub.Instance.LevelRewardsManager.Level;
            Show(curlevel, level, temp[0], temp[1]);
            FusionAudio.PostEvent("UI/LevelUp");

            if (controller.gameObject.activeSelf)
            {
                StartCoroutine(PlayAction());
            }
    
            LTChargeManager.Instance.CheckLimitedTimeGiftTrigger(Hotfix_LT.Data.LTGTriggerType.Level, level.ToString());
        }
        
        public override IEnumerator OnRemoveFromStack()
        {
            m_Open = false;
            FXObject.CustomSetActive(false);
            info1.CustomSetActive(false);
            info2.CustomSetActive(false);
            Func.alpha = 0;
            EmptyObj.CustomSetActive(false);
            StopAllCoroutines();

            if (CallBackFunc != null)
            {
                CallBackFunc();
            }

            if (GuideNodeManager.isFuncOpenGuide)
            {
                GuideNodeManager.isFuncOpenGuide = false;
                GlobalMenuManager.Instance.ComebackToMianMenu();
            }

            DestroySelf();
            yield break;
        }
    
        public override void StartBootFlash()
        {
			SetCurrentPanelAlpha(1);
			
            UITweener[] tweeners = controller.transform.GetComponents<UITweener>();

            for (int j = 0; j < tweeners.Length; ++j)
            {
                tweeners[j].tweenFactor = 0;
                tweeners[j].PlayForward();
            }
        }
    
        public void Show(int curLevel,int Level,int curXp,int Xp)
        {
            CurLevelLabel.text = CurLevelLabel.transform.GetChild(0).GetComponent<UILabel>().text = curLevel.ToString();
            LevelLabel.text = LevelLabel.transform.GetChild(0).GetComponent<UILabel>().text = Level.ToString();
            CurVigerLabel.text = CurVigerLabel.transform.GetChild(0).GetComponent<UILabel>().text = curXp.ToString();
            VigerLabel.text = VigerLabel.transform.GetChild(0).GetComponent<UILabel>().text = Xp.ToString();
            List <Hotfix_LT.Data.FuncTemplate> lists= Hotfix_LT.Data.FuncTemplateManager.Instance .GetLevelUpFunc(curLevel);

            //for (var i = 0; i < lists.Count; i++)
            //{
            //    Data.FuncTemplate obj = lists[i];
            //    if (int.Parse(obj.condition) <= Level)
            //    {
            //        LTMainHudManager.Instance.OpenFuncList.Add(obj);
            //    }
            //} 

            isFuncEmpty = lists.Count==0;
            List<Hotfix_LT.Data.FuncTemplate> showLists = new List<Hotfix_LT.Data.FuncTemplate>();

            if (!isFuncEmpty)
            {
                for (int i = 0; i < 2; ++i)
                {
                    if (lists.Count > i)
                    { 
                        showLists.Add(lists[i]); 
                    }
                }
                
                FuncScroll.SetItemDatas(showLists.ToArray());
            }
        }
    
        IEnumerator PlayAction()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 1f);
            yield return new WaitForSeconds(0.15f);
            info1.CustomSetActive(true);
            info2.CustomSetActive(true);
            yield return new WaitForSeconds(0.15f);
            Func.alpha = isFuncEmpty ? 0 : 1;
            EmptyObj.CustomSetActive(isFuncEmpty );
            yield return new WaitForSeconds(0.3f);
            TipObj.CustomSetActive(true);
            isLimit = false;
            yield break;
        }
    
        public override void OnCancelButtonClick()
        {
            if (isLimit)
            {
                return;
            }
    
            if (LTChargeManager.Instance.UpdateLimitedTimeGiftNotice())
            {
                FusionAudio.PostEvent("UI/General/ButtonClick");
                return;
            }
    
            base.OnCancelButtonClick();
        }
    }
}
