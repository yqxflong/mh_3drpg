using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTAlienMazeChapter : DynamicCellController<Hotfix_LT.Data.AlienMazeTemplate>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            NameLabel = t.GetComponent<UILabel>("NameLabel");
            IconSprite = t.GetComponent<UISprite>("MainIcon");
            LockObj = t.FindEx("LockSprite").gameObject;
            PrefectObj = t.FindEx("MainIcon/Pass").gameObject;
            HasGetRewardObj = t.FindEx("Box/Open").gameObject;
            LockTip = t.parent .parent .parent .parent.GetMonoILRComponent<LTAlienMazeLockTipController>("TipPanel/LockTipObj");
            FxObj = t.FindEx("Box/Fx").gameObject;

            LockColorWidgetList=new List<UIWidget>();
            LockColorWidgetList.Add(NameLabel);
            LockColorWidgetList.Add(IconSprite);
            LockColorWidgetList.Add(t.GetComponent<UIWidget>("BG"));
            LockColorWidgetList.Add(t.GetComponent<UIWidget>("BG/BG (1)"));
            LockColorWidgetList.Add(t.GetComponent<UIWidget>("Box/BoxSprite"));
            LockColorWidgetList.Add(t.GetComponent<UIWidget>("TipLabel"));

            t.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnChapterBtnClick));
            t.GetComponent<UIButton>("Box").onClick.Add(new EventDelegate(OnRewardBtnClick));
        }
        public UILabel NameLabel;
        public UISprite IconSprite;
        public GameObject LockObj;
        public GameObject PrefectObj;
        public GameObject HasGetRewardObj;
        public LTAlienMazeLockTipController LockTip;
        public List<UIWidget> LockColorWidgetList;
        public GameObject FxObj;
    
        private Hotfix_LT.Data.AlienMazeTemplate Data;
        private bool isLock;
        private bool isPass;
        private bool hasGet;
        
        public override void Clean()
        {
            Data =null;
        }
    
        public override void Fill(Hotfix_LT.Data.AlienMazeTemplate itemData)
        {
            if (Data == null || Data != itemData)
            {
                Data = itemData;
                NameLabel.text = Data.Name;
                IconSprite.spriteName = Data.Icon;
                isLock = LTInstanceMapModel.Instance.GetAlienMazeLockStage(Data.Id, Data.Limit);
    
                LockObj.CustomSetActive(isLock);
                
                if (isLock)
                {
                    IconSprite.color = Color.grey;
                    NameLabel.color= Color.grey;
                    for (int i = 0; i < LockColorWidgetList.Count; ++i)
                    {
                        LockColorWidgetList[i].color = Color.grey;
                    }
                }
                else
                {
                    IconSprite.color = Color.white;
                    NameLabel.color = Color.white;
                    for (int i = 0; i < LockColorWidgetList.Count; ++i)
                    {
                        LockColorWidgetList[i].color = Color.white;
                    }
                }
    
    
                isPass = LTInstanceMapModel.Instance.GetAlienMazeFinish(Data.Id);
                PrefectObj.CustomSetActive(!isLock && isPass);
    
                hasGet = LTInstanceMapModel.Instance.HasGetMazeReward(Data.Id);
                HasGetRewardObj.CustomSetActive(!isLock && isPass && hasGet);
                FxObj.CustomSetActive(!isLock && isPass && !hasGet);
            }
    
        }
    
        private string LockStr = null;
        public void OnChapterBtnClick()
        {
            if (isLock)
            {
                //弹条件解锁Tip
                if (string.IsNullOrEmpty(LockStr)) LockStr = LTInstanceMapModel.Instance.GetAlienMazeLockStr(Data.Id, Data.Limit);
                LockTip.Open(LockStr);
            }
            else if (isPass)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ALIEN_MAZE_PERFECT_PASS"));
            }
            else
            {
                System.Action callback = delegate { LTAlienMazeInstanceHudController.EnterInstance(Data.Id); };
                Hotfix_LT.Messenger.Raise(EventName .PlayCloudFXCallback,callback);
            }
        }
        
        public void OnRewardBtnClick()
        {
            if (isPass && !hasGet)
            {
                LTInstanceMapModel.Instance.RequestGetAlienMazeReward(Data.Id, delegate{
                    hasGet = true;
                    HasGetRewardObj.CustomSetActive(true);
                    FxObj.CustomSetActive(false);
                    FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.puzzle_camp_topic,
                        FusionTelemetry.GamePlayData.puzzle_camp_event_id, FusionTelemetry.GamePlayData.puzzle_camp_umengId, "reward");
                    FusionTelemetry.ItemsUmengCurrency(Data.Reward, "异界迷宫");
                    GlobalMenuManager.Instance.Open("LTShowRewardView", Data.Reward);
                });
            }
            else
            {
                var data = Johny.HashtablePool.Claim();
                data.Add("data", Data .Reward);
                data.Add("tip", (hasGet) ? EB.Localizer .GetString("ID_codefont_in_LadderController_11750") : string.Empty);
                data.Add("title", EB.Localizer.GetString("ID_ALIEN_MAZE_PASS_AWARD"));
                GlobalMenuManager.Instance.Open("LTRewardShowUI", data);
            }
        }
    }
}
