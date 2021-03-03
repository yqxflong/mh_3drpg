using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hotfix_LT.UI
{

    public class LegionPageActivity : DynamicMonoHotfix
    {
        UIButton HotfixBtn0;
        UIButton HotfixBtn1;
        UIButton HotfixBtn2;
        UIButton HotfixBtn3;
        UIButton HotfixBtn4;
        UIButton HotfixBtn5;

        public UILabel TransferDartTimeLabel, LegionWarTimeLabel;
        public LTShowItem[] TransferDartItems = new LTShowItem[2], FubenItems = new LTShowItem[2], LegionWarItems = new LTShowItem[2];

        public GameObject TransferDartRedPoint;
        public GameObject FubenRedPoint;
        public GameObject LegionWarRedPoint;

        private LegionWarTimeLine warTime;

        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            TransferDartTimeLabel = t.Find("Scroll View/Grid/Item1/Title/States").GetComponent<UILabel>();
            LegionWarTimeLabel = t.Find("Scroll View/Grid/Item3/Title/States").GetComponent<UILabel>();
            for (int i = 0; i < TransferDartItems.Length; i++)
            {
                TransferDartItems[i] = t.Find("Scroll View/Grid/Item1/Content/Grid/LTShowItem" + (i+1)).GetMonoILRComponent<LTShowItem>();
                FubenItems[i] = t.Find("Scroll View/Grid/Item2/Content/Grid/LTShowItem" + (i+1)).GetMonoILRComponent<LTShowItem>();
                LegionWarItems[i] = t.Find("Scroll View/Grid/Item3/Content/Grid/LTShowItem" + (i + 1)).GetMonoILRComponent<LTShowItem>();
            }
            TransferDartRedPoint = t.Find("Scroll View/Grid/Item1/Button/GotoBtn/RedPoint").gameObject;
            FubenRedPoint = t.Find("Scroll View/Grid/Item2/Button/GotoBtn/RedPoint").gameObject;
            LegionWarRedPoint = t.Find("Scroll View/Grid/Item3/Button/GotoBtn/RedPoint").gameObject;

            HotfixBtn0 = t.Find("Scroll View/Grid/Item1/Button/GotoBtn").GetComponent<UIButton>();
            HotfixBtn0.onClick.Add(new EventDelegate(OnGotoEscortClick));
            HotfixBtn1 = t.Find("Scroll View/Grid/Item1/Button/NoOpenBtn").GetComponent<UIButton>();
            HotfixBtn1.onClick.Add(new EventDelegate(OnGotoEscortClick));
            HotfixBtn2 = t.Find("Scroll View/Grid/Item2/Button/GotoBtn").GetComponent<UIButton>();
            HotfixBtn2.onClick.Add(new EventDelegate(OnGotoLegionFBClick));
            HotfixBtn3 = t.Find("Scroll View/Grid/Item2/Button/NoOpenBtn").GetComponent<UIButton>();
            HotfixBtn3.onClick.Add(new EventDelegate(OnGotoLegionFBClick));
            HotfixBtn4 = t.Find("Scroll View/Grid/Item3/Button/GotoBtn").GetComponent<UIButton>();
            HotfixBtn4.onClick.Add(new EventDelegate(OnGotoLegionWar));
            HotfixBtn5 = t.Find("Scroll View/Grid/Item3/Button/NoOpenBtn").GetComponent<UIButton>();
            HotfixBtn5.onClick.Add(new EventDelegate(OnGotoLegionWar));

            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.convoy , SetTransferDartRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.legionfb, SetLegionFbRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.legionwar, SetLegionWarRP);
        }

        private void SetTransferDartRP(RedPointNode node)
        {
            TransferDartRedPoint.CustomSetActive(node.num>0);
        }
        private void SetLegionWarRP(RedPointNode node)
        {
            LegionWarRedPoint.CustomSetActive(node.num > 0);
        }

        private void SetLegionFbRP(RedPointNode node)
        {
            FubenRedPoint.CustomSetActive(node.num > 0);
        }
        public override void OnDestroy()
        {
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.convoy, SetTransferDartRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.legionfb, SetLegionFbRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.legionwar, SetLegionWarRP);
            base.OnDestroy();
        }


        public void ShowUI(bool isShow)
        {
            if (isShow)
            {
                //军团护送
                for (int i = 0; i < TransferDartItems.Length; i++)
                {
                    TransferDartItems[i].mDMono.gameObject.CustomSetActive(false);
                }
                Hotfix_LT.Data.SpecialActivityTemplate template = Hotfix_LT.Data.EventTemplateManager.Instance.GetSpecialActivity(9005);
                if (template.awards != null)
                    for (int i = 0; i < ((template.awards.Count > TransferDartItems.Length) ? TransferDartItems.Length : template.awards.Count); i++)
                    {
                        if (template.awards[i] != null)
                        {
                            TransferDartItems[i].LTItemData = template.awards[i];
                            TransferDartItems[i].mDMono.gameObject.CustomSetActive(true);
                        }
                        else TransferDartItems[i].mDMono.gameObject.CustomSetActive(false);
                    }

                if (Hotfix_LT.Data.EventTemplateManager.Instance.IsTimeOK("escort_start", "escort_stop"))
                {
                    TransferDartTimeLabel.color = LT.Hotfix.Utility.ColorUtility.GreenColor;
                }
                else
                {
                    TransferDartTimeLabel.color = LT.Hotfix.Utility.ColorUtility.RedColor;
                }
                LTUIUtil.SetText(TransferDartTimeLabel, Hotfix_LT.Data.EventTemplateManager.Instance.GetActivityOpenTimeStr("escort_start", "escort_stop"));
                
                //走配置表来设置奖励内容
                ArrayList aList = EB.JSON.Parse(Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("AllianceFBReward")) as ArrayList;
                List<LTShowItemData> showItemsList = new List<LTShowItemData>();
                if (aList == null)
                {
                    FubenItems[0].LTItemData = new LTShowItemData("arena-gold", 1, "res",false);
                    FubenItems[1].LTItemData = new LTShowItemData("gold", 1, "res", false);
                }
                else
                {
                    for (int i = 0; i < aList.Count; i++)
                    {
                        string id = EB.Dot.String("data", aList[i], string.Empty);
                        int count = EB.Dot.Integer("quantity", aList[i], 0);
                        string type = EB.Dot.String("type", aList[i], string.Empty);
                        if (!string.IsNullOrEmpty(id))
                        {
                            LTShowItemData showItemData = new LTShowItemData(id, count, type,false);
                            showItemsList.Add(showItemData);
                        }
                    }
                    //
                    FubenItems[0].LTItemData = showItemsList[0];
                    FubenItems[1].LTItemData = showItemsList[1];
                }

                //军团战
                Hotfix_LT.Data.SpecialActivityTemplate template2 = Hotfix_LT.Data.EventTemplateManager.Instance.GetSpecialActivity(9004);
                for (int i = 0; i < LegionWarItems.Length; i++)
                {
                    LegionWarItems[i].mDMono.gameObject.CustomSetActive(false);
                }
                if (template.awards != null)
                {
                    for (int i = 0; i < ((template2.awards.Count > LegionWarItems.Length) ? LegionWarItems.Length : template2.awards.Count); i++)
                    {
                        if (template.awards[i] != null)
                        {
                            LegionWarItems[i].LTItemData = template2.awards[i];
                            LegionWarItems[i].mDMono.gameObject.CustomSetActive(true);
                        }
                        else LegionWarItems[i].mDMono.gameObject.CustomSetActive(false);
                    }
                }
                warTime = LegionWarTimeLine.none;
                string Title = string.Empty;
                LegionLogic.GetInstance().IsOpenLegionBattle();
                if (LTLegionWarManager.Instance.IsOpenWarTime())
                {
                    warTime = LTLegionWarManager.GetLegionWarStatus();                   
                }
                else
                {
                    Title = string.Format("[ff6699]{0}[-]\n", EB.Localizer.GetString("ID_NEXT_WEEK"));
                }

                LegionWarTimeLabel.text = string.Format("{0}{1}{2}{3}", Title, GetTimeStr(LTLegionWarManager.Instance.WarOpenTime.QualifyOpenTime, warTime == LegionWarTimeLine.QualifyGame)
                    , GetTimeStr(LTLegionWarManager.Instance.WarOpenTime.SemiOpenTime, warTime == LegionWarTimeLine.SemiFinal)
                    , GetTimeStr(LTLegionWarManager.Instance.WarOpenTime.FinalOpenTime, warTime == LegionWarTimeLine.Final));

                LegionLogic.GetInstance().IsOpenConvoy();
                LegionLogic.GetInstance().IsOpenLegionFB();
            }


            mDMono.gameObject.CustomSetActive(isShow);
        }

        private string GetTimeStr(LTLegionWarTime[] data, bool open = false)
        {
            string day = getDay(data[0].day);
            string colorStr = open ? "[42fe79]" : "[ff6699]";
            string str = string.Format("{0}{1}:{2:00}:{3:00}-{4:00}:{5:00}[-]\n", colorStr, day, data[0].hour, data[0].minute, data[1].hour, data[1].minute);
            return str;
        }
        public static string getDay(int i)
        {
            switch (i)
            {
                case 0: return EB.Localizer.GetString("ID_WEEK_0");
                case 1: return EB.Localizer.GetString("ID_WEEK_1");
                case 2: return EB.Localizer.GetString("ID_WEEK_2");
                case 3: return EB.Localizer.GetString("ID_WEEK_3");
                case 4: return EB.Localizer.GetString("ID_WEEK_4");
                case 5: return EB.Localizer.GetString("ID_WEEK_5");
                case 6: return EB.Localizer.GetString("ID_WEEK_6");
                default: return EB.Localizer.GetString("ID_EVERYDAY");
            }
        }

        public void OnGotoEscortClick()
        {
            if (!Hotfix_LT.Data.EventTemplateManager.Instance.IsTimeOK("escort_start", "escort_stop"))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_AllianceEscortUtil_4329"));
                return;
            }

            if (AllianceUtil.GetIsInTransferDart("")) return;

            GlobalMenuManager.Instance.ComebackToMianMenu();
            Hotfix_LT.Data.SpecialActivityTemplate temp = Hotfix_LT.Data.EventTemplateManager.Instance.GetSpecialActivity(9005);
            string[] strs = temp.nav_parameter.Split(';');
            if (strs.Length < 2)
            {
                EB.Debug.LogError(string.Format("NavParameter is Error, Length less than 2! activityID = {0}, param = {1}", temp.id, temp.nav_parameter));
            }
            else
            {
                EB.Coroutines.Run(FindPath(strs));
            }
        }
        
        public void OnGotoLegionFBClick()
        {
            if (AllianceUtil.GetIsInTransferDart("")) return;

            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTLegionFBUI");
            FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.alliance_camp_topic,
            FusionTelemetry.GamePlayData.alliance_camp_event_id, FusionTelemetry.GamePlayData.alliance_camp_umengId, "open");
        }

        public void OnGotoLegionWar()
        {
            if (AllianceUtil.GetIsInTransferDart("")) return;

            if (!(warTime == LegionWarTimeLine.QualifyGame || warTime == LegionWarTimeLine.SemiFinal || warTime == LegionWarTimeLine.Final))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LegionPageActivity_7613"));
                return;
            }

            Hotfix_LT.Data.SpecialActivityTemplate temp = Hotfix_LT.Data.EventTemplateManager.Instance.GetSpecialActivity(9004);
            string[] strs = temp.nav_parameter.Split(';');
            if (strs.Length < 2)
            {
                EB.Debug.LogError(string.Format("NavParameter is Error, Length less than 2! activityID = {0}, param = {1}", temp.id, temp.nav_parameter));
            }
            else
            {
                GlobalMenuManager.Instance.ComebackToMianMenu();
                EB.Coroutines.Run(FindPath(strs));
            }
        }

        IEnumerator FindPath(string[] strs)
        {
            bool localplayeraction = false;
            SceneRootEntry sceneRoot = MainLandLogic.GetInstance().ThemeLoadManager.GetSceneRoot();
            Transform playersList = sceneRoot.m_SceneRoot.transform.Find("PlayerList");
            if (playersList != null)
            {
                while (!localplayeraction)
                {
                    yield return null;
                    if (playersList.gameObject.activeSelf) localplayeraction = true;
                }
            }
            WorldMapPathManager.Instance.StartPathFindToNpcFly(MainLandLogic.GetInstance().CurrentSceneName, strs[0], strs[1]);
        }


    }

}