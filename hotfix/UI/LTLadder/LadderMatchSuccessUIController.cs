using UnityEngine;
using System.Collections;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LadderMatchSuccessUIController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            Container = t.FindEx("Container").gameObject;
            MyNameLabel = t.GetComponent<UILabel>("Container/PlayerInfo/Self/Name");
            MyScoreLabel = t.GetComponent<UILabel>("Container/PlayerInfo/Self/Point");
            YourNameLabel = t.GetComponent<UILabel>("Container/PlayerInfo/Other/Name");
            YourScoreLabel = t.GetComponent<UILabel>("Container/PlayerInfo/Other/Point");
            MyPortrait = t.GetComponent<UISprite>("Container/PlayerInfo/Self/Icon");
            YourPortrait = t.GetComponent<UISprite>("Container/PlayerInfo/Other/Icon");
            MyFrame = t.GetComponent<UISprite>("Container/PlayerInfo/Self/Icon/Frame");
            YourFrame = t.GetComponent<UISprite>("Container/PlayerInfo/Other/Icon/Frame");
            TypeLabel = t.GetComponent<UILabel>("Container/PlayerInfo/Self/PointFontLabel");
            YourTypeLabel = t.GetComponent<UILabel>("Container/PlayerInfo/Other/PointFontLabel");
            PositionTweener = t.GetComponent<TweenPosition>("Container/PlayerInfo/Self");
            PositionTweener2 = t.GetComponent<TweenPosition>("Container/PlayerInfo/Other");
            isOpen = false;
	        t.GetComponent<UIButton>("Container/PlayerInfo/CancelBtn").onClick
	            .Add(new EventDelegate(OnCancelButtonClick));


        }
        public override bool ShowUIBlocker { get { return true; } }
    
        public GameObject Container;
        public UILabel MyNameLabel, MyScoreLabel;
        public UILabel YourNameLabel, YourScoreLabel;
        public UISprite MyPortrait, YourPortrait;
        public UISprite MyFrame, YourFrame;
        public UILabel TypeLabel, YourTypeLabel;
    	public TweenPosition PositionTweener, PositionTweener2;
    
        private int[] TP1FromX = new int[3] { -2200, -500, 2200 };
        private int[] TP2FromX = new int[3] { 2200, 500, -2200 };
        bool TwFinished;
    
        public static bool isOpen;
    	public class InfoData
        {
            /// <summary>
            ///  0竞技场 1英雄交锋 
            /// </summary>
            public byte type;
            public object info;
        }

        private object mParam;
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            mParam = param;
            TwFinished = false;
    
            PositionTweener.from.x = TP1FromX[0];
            PositionTweener.to.x = TP1FromX[1];
            PositionTweener2.from.x = TP2FromX[0];
            PositionTweener2.to.x = TP2FromX[1];
    
            PositionTweener.SetOnFinished(delegate ()
    		{
    			TwFinished = true;
    		});

            PositionTweener.ResetToBeginning();
            PositionTweener.PlayForward();

            PositionTweener2.ResetToBeginning();
            PositionTweener2.PlayForward();
        }
    
    	public override IEnumerator OnAddToStack()
    	{
            isOpen = true;
            yield return base.OnAddToStack();
    		ShowUI(mParam);
    	}
    
    	public override IEnumerator OnRemoveFromStack()
    	{
            isOpen = false;
            PositionTweener.onFinished.Clear();
            PositionTweener2.onFinished.Clear();
            DestroySelf();
    		yield break;
    	}
    
    	public void ShowUI(object info)
        {
            Container.CustomSetActive(true);
            FusionAudio.PostEvent("UI/New/JueDou", true);
            InfoData data = info as InfoData;
            string myName;
            if (data == null)
            {
                if (!DataLookupsCache.Instance.SearchDataByID<string>("user.name", out myName))
                    myName = string.Empty;
                LTUIUtil.SetText(MyNameLabel, EB.Localizer.GetString(myName));
    
                LTUIUtil.SetText(MyScoreLabel,LadderManager.Instance.Info.Point.ToString());
                MyPortrait.spriteName = LTMainHudManager.Instance.UserHeadIcon;
                MyFrame.spriteName = LTMainHudManager.Instance.UserLeaderHeadFrame.iconId;

                string playerName = EB.Dot.String("ladder.match_result.playerName", info, "");
                int score = EB.Dot.Integer("ladder.match_result.point", info, 0);
                string portrait = EB.Dot.String("ladder.match_result.portrait", info, "");
                int skin = EB.Dot.Integer("ladder.match_result.skin", info, 0);
                string frame = EB.Dot.String("ladder.match_result.headFrame", info, null);
                LTUIUtil.SetText(YourNameLabel, EB.Localizer.GetString(playerName));
    			LTUIUtil.SetText(YourScoreLabel,score.ToString());
    			//记录天梯分值
    			LTHeroBattleModel.GetInstance().choiceData.selfInfo.score = LadderManager.Instance.Info.Point;
    			LTHeroBattleModel.GetInstance().choiceData.otherInfo.score = score;
    			//
    			if (!string.IsNullOrEmpty(portrait))
    				YourPortrait.spriteName = portrait;
    			else
    				YourPortrait.spriteName = "Partner_Head_Sidatuila";
                YourFrame.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(frame).iconId;
                StartCoroutine(TimingSkip());
            }
            else
            {
                switch (data.type)
                {
                    case 0:
                        if (!DataLookupsCache.Instance.SearchDataByID<string>("user.name", out myName))
                            myName = string.Empty;
                        LTUIUtil.SetText(MyNameLabel, EB.Localizer.GetString(myName));
                        LTUIUtil.SetText(MyScoreLabel, LadderManager.Instance.Info.Point.ToString());
                        MyPortrait.spriteName = LTMainHudManager.Instance.UserHeadIcon;
                        MyFrame.spriteName = LTMainHudManager.Instance.UserLeaderHeadFrame.iconId;

                        string playerName = EB.Dot.String("ladder.match_result.playerName", data.info, "");
                        int score = EB.Dot.Integer("ladder.match_result.point", data.info, 0);
                        string portrait = EB.Dot.String("ladder.match_result.portrait", data.info, "");
                        int skin = EB.Dot.Integer("ladder.match_result.skin", data.info, 0);
                        string frame = EB.Dot.String("ladder.match_result.headFrame", data.info, null);
                        LTUIUtil.SetText(YourNameLabel, EB.Localizer.GetString(playerName));
    					LTUIUtil.SetText(YourScoreLabel, score.ToString());
                        //记录天梯分值
                        LTHeroBattleModel.GetInstance().choiceData.selfInfo.score = LadderManager.Instance.Info.Point;
                        LTHeroBattleModel.GetInstance().choiceData.otherInfo.score = score;
                        //
                        if (!string.IsNullOrEmpty(portrait))
    						YourPortrait.spriteName = portrait;
    					else
    						YourPortrait.spriteName = "Partner_Head_Sidatuila";
                        YourFrame.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(frame).iconId;
                        StartCoroutine(TimingSkip());
                        break;
                    case 1:
                        HeroBattleChoiceData choiceData = data.info as HeroBattleChoiceData;
    					LTUIUtil.SetText(TypeLabel, EB.Localizer.GetString("ID_codefont_in_LadderMatchSuccessUIController_3473"));
    					LTUIUtil.SetText(YourTypeLabel, EB.Localizer.GetString("ID_codefont_in_LadderMatchSuccessUIController_3473"));
    					StartCoroutine(OpenHeroBattleHud());
                        if (!DataLookupsCache.Instance.SearchDataByID<string>("user.name", out myName))
                            myName = string.Empty;
                        LTUIUtil.SetText(MyNameLabel, EB.Localizer.GetString(myName));
                        MyPortrait.spriteName = LTMainHudManager.Instance.UserHeadIcon;
                        MyFrame.spriteName = LTMainHudManager.Instance.UserLeaderHeadFrame.iconId;
                        LTUIUtil.SetText(MyScoreLabel, BalanceResourceUtil.GetUserLevel().ToString());
    					LTUIUtil.SetText(YourNameLabel, EB.Localizer.GetString(choiceData.otherInfo.name));
    					YourPortrait.spriteName = choiceData.otherInfo.portrait;
                        YourFrame.spriteName = choiceData.otherInfo.frame;
                        LTUIUtil.SetText(YourScoreLabel, choiceData.otherInfo.level.ToString());
    					break;
                }
            }
        }
    
        private 
        IEnumerator TimingSkip()
        {
    		while (!TwFinished)
    		{
    			yield return null;
    		}
    
            PositionTweener.onFinished.Clear();
            PositionTweener2.SetOnFinished(delegate ()
            {
                FusionAudio.PostEvent("UI/Battle/StartBattle");
                GlobalMenuManager.Instance.Open("LTHeroBattleMenu", LTHeroBattleModel.GetInstance().choiceData);
                Container.CustomSetActive(false);
            });
            PositionTweener.from.x = TP1FromX[1];
            PositionTweener.to.x = TP1FromX[2];
            PositionTweener2.from.x = TP2FromX[1];
            PositionTweener2.to.x = TP2FromX[2];
    
            while (LTHeroBattleModel.GetInstance().choiceData.choiceState != 1)
            {
                yield return null;
            }
    
            PositionTweener.ResetToBeginning();
            PositionTweener.PlayForward();
    
            PositionTweener2.ResetToBeginning();
            PositionTweener2.PlayForward();
        }
    
        IEnumerator OpenHeroBattleHud()
        {
            //英雄交锋已不走这一套逻辑
            yield return null;
            //while (!TwFinished)
            //{
            //	yield return null;
            //}
            //      FusionAudio.PostEvent("UI/Battle/StartBattle");
            //      GlobalMenuManager.Instance.Open("LTHeroBattleMenu",LTHeroBattleModel.GetInstance().choiceData);
        }
    
    	public override void OnCancelButtonClick()
    	{
    
    	}
    }
}
