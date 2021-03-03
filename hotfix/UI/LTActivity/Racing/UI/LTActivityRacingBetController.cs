//LTActivityRacingBetController
//赛跑活动竞猜
//Johny

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTActivityRacingBetController : UIControllerHotfix
    {
        private class Player
        {
            public RacingPlayerData pd;
            public ConsecutiveClickCoolTrigger TouchArea;
            public ParticleSystemUIComponent PS_Select;
            public UILabel Label_Num;
            public UILabel Label_Name;
            public UnityEngine.Transform Node_Model;
            public UnityEngine.GameObject Model;
            public MoveController ModelMC;
            public List<Material> MaterialList = new List<Material>();
        }
        
        private class BetControls
        {
            public class AwardPreview
            {
                public UISprite Icon;
                public UILabel Label;
            }

            public Transform Node;

            public class BetButton
            {
                public UILabel Label_Text, Label_Pay;
                public UISprite PayType;
                public ConsecutiveClickCoolTrigger Button;
            }

            public AwardPreview Preview = new AwardPreview();
            public BetButton Button_Bet = new BetButton();
            public UILabel Label_Title, Label_CountDown, Label_MaxBetTimes;
            //key: num
            public List<UILabel> SupportList = new List<UILabel>();

            #region Params
            public int currentSelectPlayer = -1;

            //倒计时句柄
            public int betCountDownSeq = -1;

            //开赛倒计时
            public System.TimeSpan currentBetCountDown, _ts_1sec = new System.TimeSpan(0,0,1);
            #endregion

            public BetControls(Transform node_bet, EventDelegate betClicked)
            {
                Node = node_bet;
                //title & cd
                Label_Title = node_bet.GetComponent<UILabel>("lb_title");
                Label_CountDown = node_bet.GetComponent<UILabel>("lb_countdown");

                #region BetButton
                //button
                var btn_bet = node_bet.GetComponent<ConsecutiveClickCoolTrigger>("btn_bet");
                btn_bet.clickEvent.Add(betClicked);
                Button_Bet.Button = btn_bet;

                //text
                var btn_bet_text = btn_bet.transform.GetComponent<UILabel>("lb_support");
                Button_Bet.Label_Text = btn_bet_text;

                //paytype
                var btn_bet_icon = btn_bet.transform.GetComponent<UISprite>("sp_currency");
                Button_Bet.PayType = btn_bet_icon;

                //pay
                var btn_bet_count = btn_bet.transform.GetComponent<UILabel>("lb_pay");
                Button_Bet.Label_Pay = btn_bet_count;
                #endregion

                //最大下注次数
                var lb_maxBetNum = node_bet.GetComponent<UILabel>("lb_maxbetnum");
                lb_maxBetNum.gameObject.CustomSetActive(false);
                Label_MaxBetTimes = lb_maxBetNum;

                //奖励预览
                Preview = new BetControls.AwardPreview();
                Preview.Icon = node_bet.GetComponent<UISprite>("node_preview/sp_currency");
                Preview.Label = node_bet.GetComponent<UILabel>("node_preview/lb_count");

                //人气值
                var node_support = node_bet.Find("node_support");
                for(int i = 1; i <= 3; i++)
                {
                    SupportList.Add(node_support.GetComponent<UILabel>($"lb_support_{i}"));
                }
            }

            public void Init()
            {
                betCountDownSeq = ILRTimerManager.instance.AddTimer(1000, 0, OnTimer_CountDown);
            }

            public void ResetParams()
            {
                currentSelectPlayer = -1;
                ILRTimerManager.instance.RemoveTimerSafely(ref betCountDownSeq);
                currentBetCountDown = new System.TimeSpan();
            }

            //下注选手
            public void BetThePlayer(int group, Player player, System.Action<bool> act)
            {
                if(player != null)
                {
                    //对选定选手下注
                    LTActivityRacingManager.Instance.RequestBet(group, player.pd.Num, (group_, succ)=>
                    {
                        if(succ)
                        {
                            RefreshAwardPayoutDisplay(group);
                            RefreshMaxBetNumDisplay(group);
                            RefreshBetCountDisplay(group);
                            RefreshBetHint(group);
                            RefreshPlayerSupport(group);
                            act?.Invoke(true);
                        }
                        else
                        {
                            act?.Invoke(false);
                        }
                    });
                }
                else
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_RACING_BET_TIPS_CHOOSE"));
                    act?.Invoke(false);
                }
            }

            //增加下注
            public void AddBet(int group)
            {
                LTActivityRacingManager.Instance.RequestAddBet(group, (group_, succ)=>
                {
                    RefreshMaxBetNumDisplay(group);
                    RefreshAwardPayoutDisplay(group);
                    RefreshBetCountDisplay(group);
                    RefreshPlayerSupport(group);
                });
            }

            #region Refresh
            public void Refresh(int group)
            {
                var mgr = LTActivityRacingManager.Instance;
                currentBetCountDown = mgr.GetGuessLeftTime(group);
                RefreshBetDuringDisplay();
                RefreshAwardPayoutDisplay(group);
                RefreshMaxBetNumDisplay(group);
                RefreshBetCountDisplay(group);
                RefreshPlayerSupport(group);
            }

            //刷新奖励预览
            private void RefreshAwardPayoutDisplay(int group)
            {
                var mgr = LTActivityRacingManager.Instance;
                Preview.Icon.spriteName = mgr.GetBetCurrencyTypeSpriteName(group);
                Preview.Label.text = mgr.GetCurrentPayOut(group).ToString();
            }

            //刷新最大下注次数显示
            private void RefreshMaxBetNumDisplay(int group)
            {
                if(!LTActivityRacingManager.Instance.HasBetPlayer(group))
                {
                    return;
                }

                var lb_maxBet = Label_MaxBetTimes;
                lb_maxBet.gameObject.CustomSetActive(true);
                lb_maxBet.text = LTActivityRacingManager.Instance.GetCurrentAddBetTimeDisplay(group);
                if(!LTActivityRacingManager.Instance.HasEnoughAddBetTime(group))
                {
                    lb_maxBet.color = LT.Hotfix.Utility.ColorUtility.LightRedColor;
                }
            }

            //刷新下注额显示
            public void RefreshBetCountDisplay(int group)
            {
                var mgr = LTActivityRacingManager.Instance;
                var sp_payType = Button_Bet.PayType;
                sp_payType.spriteName = mgr.GetBetCurrencyTypeSpriteName(group);
                var lb_pay = Button_Bet.Label_Pay;
                lb_pay.text = mgr.GetOnceBetAmount(group).ToString();
                if(!mgr.HasEnoughCurrency(group))
                {
                    lb_pay.color = LT.Hotfix.Utility.ColorUtility.LightRedColor;
                }
                else
                {
                    lb_pay.color = UnityEngine.Color.white;
                }
            }

            //刷新竞猜倒计时显示
            private void RefreshBetDuringDisplay()
            {
                if(currentBetCountDown.TotalSeconds <= 0)
                {
                    Label_CountDown.gameObject.CustomSetActive(false);
                }
                else
                {
                    string hr = currentBetCountDown.Hours < 10 ? $"0{currentBetCountDown.Hours}" : currentBetCountDown.Hours.ToString();
                    string min = currentBetCountDown.Minutes < 10 ? $"0{currentBetCountDown.Minutes}" : currentBetCountDown.Minutes.ToString();
                    string sec = currentBetCountDown.Seconds < 10 ? $"0{currentBetCountDown.Seconds}" : currentBetCountDown.Seconds.ToString();
                    string strTime = $"{hr}:{min}:{sec}";
                    Label_CountDown.text = EB.Localizer.Format("ID_RACING_START_COUNTDOWN", strTime);
                }
            }
            
            //刷新下注文字提示
            public void RefreshBetHint(int group)
            {
                var gd = LTActivityRacingManager.Instance.GetGroupData(group);
                if(gd.CurBetPlayerNum > 0)
                {
                    Button_Bet.Label_Text.text = EB.Localizer.GetString("ID_RACING_BET_N");
                }
                else
                {
                    Button_Bet.Label_Text.text = EB.Localizer.GetString("ID_RACING_BET_1");
                }
            }
            
            //刷新选手人气值
            public void RefreshPlayerSupport(int group)
            {
                var gd = LTActivityRacingManager.Instance.GetGroupData(group);
                var ll = gd.OrderedPids;
                var pds = gd.Players;
                for(int i = 0; i < ll.Count; i++)
                {
                    int pid = ll[i];
                    var pd = pds[pid];
                    var lb_support = SupportList[i];
                    lb_support.text = EB.Localizer.Format("ID_RACING_PLAYER_SUPPORT", pd.Support);
                }
            }
            #endregion

            //倒计时器
            private void OnTimer_CountDown(int seq)
            {
                if(currentBetCountDown.TotalSeconds <= 0)
                {
                    return;
                }

                currentBetCountDown -= _ts_1sec;
                RefreshBetDuringDisplay();
            }
        }

        private class ResultControls
        {
            public class AwardCls
            {
                public Transform Node;
                public UILabel Label_Text, Label_Award;
                public UISprite AwardType;
            }

            public class ReplayButton
            {
                public UILabel Label_Text;
                public UISprite Sprite;
                public ConsecutiveClickCoolTrigger Button;
            }

            public Transform Node;
            //title & tips
            public UILabel Label_Title, Label_Tips;
            public AwardCls Award = new AwardCls();
            public ReplayButton Button_Replay = new ReplayButton();
            //key: rank
            public List<UILabel> RankList = new List<UILabel>();

            public ResultControls(Transform node_result, EventDelegate replayClicked)
            {
                Node = node_result;

                //title & tips
                Label_Title = node_result.GetComponent<UILabel>("lb_title");
                Label_Tips = node_result.GetComponent<UILabel>("lb_tips");
                
                //award
                var node_award = node_result.Find("node_award");
                Award.Node = node_award;
                Award.Label_Text = node_award.GetComponent<UILabel>("lb_title");
                Award.Label_Award = node_award.GetComponent<UILabel>("lb_award");
                Award.AwardType = node_award.GetComponent<UISprite>("sp_award");

                //button
                var btn_replay = node_result.GetComponent<ConsecutiveClickCoolTrigger>("btn_replay");
                btn_replay.clickEvent.Add(replayClicked);
                Button_Replay.Button = btn_replay;

                //rank
                var node_rank = node_result.Find("node_rank");
                for(int i = 1; i <= 3; i++)
                {
                    RankList.Add(node_rank.GetComponent<UILabel>($"sp_rank{i}/Label"));
                }
            }

            public void Refresh(int group)
            {
                //返奖情况
                RefreshAwardDisplay(group);
            }

            private void RefreshAwardDisplay(int group)
            {
                var mgr = LTActivityRacingManager.Instance;
                //返奖情况
                var gd = mgr.GetGroupData(group);
                int betPlayerRank = gd.RankByNum(gd.CurBetPlayerNum);
                if(betPlayerRank == 1)
                {
                    Award.Node.gameObject.CustomSetActive(true);
                    Award.AwardType.spriteName = mgr.GetBetCurrencyTypeSpriteName(group);
                    Award.Label_Award.text = mgr.GetCurrentPayOut(group).ToString();
                    Label_Tips.text = EB.Localizer.GetString("ID_RACING_AWARD_HINT_WIN");
                }
                else
                {
                    Award.Node.gameObject.CustomSetActive(false);
                    Label_Tips.text = EB.Localizer.GetString("ID_RACING_AWARD_HINT_LOSE");
                }
            }
        }

        #region 容器
        private Dictionary<string, ConsecutiveClickCoolTrigger> buttonControls = new Dictionary<string, ConsecutiveClickCoolTrigger>();
        //key: num
        private List<Player> _players = new List<Player>();
        #endregion

        private BetControls _betControls;
        private ResultControls _resultControls;

        private string currentView = "bet";
        private int currentGroup = -1;

        private int playerLoadCnt = 0;
        
        public override void Awake()
        {
            base.Awake();
            InitControls();
        }

        public override bool IsFullscreen()
        {
            return true;
        }

        public override IEnumerator OnAddToStack()
        {
            controller.transform.localPosition = new Vector3(0, 0, 3000);
            _betControls.Init();
            //注册货币变化监听
            var dl = controller.transform.GetDataLookupILRComponent<LTRacingBetDataLookup>();
            dl.holder = this;

            yield return base.OnAddToStack();           
        }

        public override IEnumerator OnRemoveFromStack()
        {
            //解注册货币变化监听
            var dl = controller.transform.GetDataLookupILRComponent<LTRacingBetDataLookup>();
            dl.holder = null;
            //--
            DestroyModels();
            _betControls.ResetParams();
            currentGroup = -1;
            RefreshSelectFX(0);
            DestroySelf();
            yield break;
        }

        private void InitControls()
        {
            var t = controller.transform;
            var node_content = t.Find("CONTENT");
            
            //info button
            var btn_info = t.GetComponent<ConsecutiveClickCoolTrigger>("NewCurrency/InfoButton");
            buttonControls.Add("info", btn_info);
            btn_info.clickEvent.Add(new EventDelegate(OnInfoButtonClicked));

            //3个选手
            for(int i = 1; i <= 3; i++)
            {
                var player = new Player();
                _players.Add(player);
                var node_player = node_content.Find($"node_player{i}");
                player.Node_Model = node_player;
                // player button
                var btn_player = node_player.GetComponent<ConsecutiveClickCoolTrigger>();
                btn_player.clickEvent.Add(new EventDelegate(()=> OnPlayerClicked(btn_player)));
                player.TouchArea =  btn_player;
                //player select fx
                player.PS_Select = node_player.GetComponent<ParticleSystemUIComponent>("Fx");
                //other player info
                var node_info = node_player.Find("node_info");
                player.Label_Num = node_info.GetComponent<UILabel>("lb_num");
                player.Label_Name = node_info.GetComponent<UILabel>("lb_name");
            }

            var node_bet = node_content.Find("node_bet");
            var node_result = node_content.Find("node_result");
            _betControls = new BetControls(node_bet, new EventDelegate(OnBetButtonClicked));
            _resultControls = new ResultControls(node_result, new EventDelegate(OnReplayButtonClicked));
        }

        public override void SetMenuData(object param)
        {
            var ht = param as Hashtable;
            currentView = ht["view"] as string;
            currentGroup = (int)ht["group"];
            
            var mgr = LTActivityRacingManager.Instance;

            if(currentView.Equals("bet"))
            {
                _betControls.Node.gameObject.CustomSetActive(true);
                _resultControls.Node.gameObject.CustomSetActive(false);

                _betControls.Refresh(currentGroup);

                #region 准备选手
                var gd = mgr.GetGroupData(currentGroup);
                var orderedPids = gd.OrderedPids;
                for(int i = 1; i <= 3; i++)
                {
                    var pid = orderedPids[i - 1];
                    var pd = gd.GetPlayerData(pid);
                    var player = _players[i - 1];
                    player.pd = pd;
                    LoadPlayerInfo(player);
                }
                #endregion
                
                _betControls.RefreshBetHint(currentGroup);
            }
            else
            {
                _betControls.Node.gameObject.CustomSetActive(false);
                _resultControls.Node.gameObject.CustomSetActive(true);

                _resultControls.Refresh(currentGroup);

                #region 准备选手
                var gd = mgr.GetGroupData(currentGroup);
                var orderedPids = gd.OrderedPids;
                for(int i = 1; i <= 3; i++)
                {
                    var pid = orderedPids[i - 1];
                    var pd = gd.GetPlayerData(pid);
                    int rank = pd.Rank;
                    Player player;
                    if(rank == 1)
                    {
                        player = _players[1];
                    }
                    else if(rank == 2)
                    {
                        player = _players[0];
                    }
                    else
                    {
                        player = _players[2];
                    }
                    player.pd = pd;
                    LoadPlayerInfo(player);
                }
                #endregion
            }
        }

        #region About 选手
        //载入选手
        private void LoadPlayerInfo(Player player)
        {
            //create model
            string prefab_path = $"Bundles/Player/Variants/{player.pd.ModelName}-I";
            PoolModel.GetModelAsync(prefab_path, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, delegate (UnityEngine.Object obj, object param)
            {
                var variantObj = obj as UnityEngine.GameObject;
                if (variantObj == null)
                {
                    EB.Debug.LogError("[Racing]Failed to create model!!!");
                    return;
                }
                variantObj.transform.parent = player.Node_Model.transform;
                variantObj.transform.localPosition = new Vector3(0,0,-500);
                player.Model = variantObj;
                player.ModelMC = InitModel(variantObj);
                
                if(currentView.Equals("bet"))
                {
                    playerLoadCnt += 1;
                    if(playerLoadCnt == 3)
                    {
                        var gd = LTActivityRacingManager.Instance.GetGroupData(currentGroup);
                        int curBetPlayer = gd.CurBetPlayerNum;
                        if(curBetPlayer > 0)
                        {
                            RefreshModelLight(curBetPlayer);
                        }
                    }
                }
            }, null);

            //选手号码和名字刷新
            player.Label_Num.text = EB.Localizer.Format("ID_RACING_BET_NUM", player.pd.Num.ToString());
            player.Label_Name.text = player.pd.Name;
        }

        private void DestroyModels()
        {
            playerLoadCnt = 0;
            for(int i = 0; i < _players.Count; i++)
            {
                var pd = _players[i];
                GameObject.Destroy(pd.Model);
            }
        }

        private MoveController InitModel(GameObject variantObj)
        {
            CharacterVariant variant = variantObj.GetComponent<CharacterVariant>();
            variant.InstantiateCharacter();
            UnityEngine.GameObject character = variant.CharacterInstance;
            character.transform.SetParent(variant.transform);
            character.transform.localScale = UnityEngine.Vector3.one * 0.4f;
            character.transform.localRotation = UnityEngine.Quaternion.Euler(0, 180, 0);
            character.transform.localPosition = UnityEngine.Vector3.zero;
            SetObjLayer(variantObj, GameEngine.Instance.ui3dLayer);
            MoveController mc = character.GetComponent<MoveController>();
            return mc;
        }

        private void SetObjLayer(UnityEngine.GameObject obj, int layer)
        {
            obj.transform.SetChildLayer(layer);
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                renderers[i].gameObject.layer = layer;
                Renderer render = renderers[i];
                Material[] materials = render.materials;
                for (int j = 0; j < materials.Length; j++)
                {
                    Material ImageMaterial = new Material(materials[j]);
                    ImageMaterial.SetColor("_RimColor", new Color(0, 0, 0, 1f));
                    materials[j] = ImageMaterial;
                }
                render.materials = materials;
            }
        }
        
        private void RefreshModelLight(int betNum)
        {
            for(int i = 1; i <= _players.Count; i++)
            {
                var player = _players[i - 1];
                if(betNum != i)
                {
                    SetModelDark(player);
                }
                else
                {
                    RestoreModelDark(player);
                }
            }
        }

        private void SetModelDark(Player player)
        {
            Renderer[] renderers = player.Model.GetComponentsInChildren<Renderer>();

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer render = renderers[i];
                Material[] materials = render.materials;
                player.MaterialList.AddRange(render.materials);
                Material goldMat = materials[0];
                for (int j = 0; j < materials.Length; j++)
                {
                    Material GoldMaterial = new Material(goldMat);
                    GoldMaterial.SetColor("_FinalColor", new Color(0, 0, 0, 0.5f));
                    GoldMaterial.SetFloat("_ContrastIntansity", 0.95f);
                    GoldMaterial.SetFloat("_Brightness", 0.01f);
                    GoldMaterial.SetFloat("_GrayScale", 0.15f);
                    GoldMaterial.SetFloat("_SpecularPower", 1f);
                    GoldMaterial.SetFloat("_SpecularFresnel", 1f);
                    GoldMaterial.SetFloat("_Outline", 0f);
                    GoldMaterial.SetFloat("_ToonThreshold", 0.827f);
                    GoldMaterial.SetFloat("_RimPower", 8f);
                    GoldMaterial.SetFloat("_SpecularPower", 8f);
                    GoldMaterial.SetFloat("_SpecularFresnel", 1f);
                    GoldMaterial.SetFloat("_LightScale", 1.4f);
                    GoldMaterial.SetFloat("_UseMatCap", 0.0f);

                    GoldMaterial.EnableKeyword("EBG_COLORFILTER_ON");
                    GoldMaterial.EnableKeyword("EBG_SPECULAR_MAP_ON");
                    //GoldMaterial.SetInt("EBG_COLORFILTER",1);
                    //GoldMaterial.SetInt("EBG_SPECULAR_MAP",1);
                    materials[j] = GoldMaterial;
                }
                render.materials = materials;
            }
        }

        private void RestoreModelDark(Player player)
        {
            if(player.MaterialList.Count == 0)
            {
                return;
            }

            Renderer[] renderers = player.Model.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer render = renderers[i];
                Material[] materials = render.materials;
                Material goldMat = materials[0];
                int index = 0;
                for (int j = 0; j < materials.Length; j++)
                {
                    Material GoldMaterial = player.MaterialList[index];
                    index++;
                    materials[j] = GoldMaterial;
                }
                render.materials = materials;
            }

            player.MaterialList.Clear();
        }
        
        private void RefreshSelectFX(int num)
        {
            for(int i = 0; i < _players.Count; i++)
            {
                var player = _players[i];
                if(_betControls.currentSelectPlayer - 1 == i)
                {
                    player.PS_Select.gameObject.CustomSetActive(true);
                }
                else
                {
                    player.PS_Select.gameObject.CustomSetActive(false);
                }
            }
        }
        #endregion

        #region About 回调事件
        //接收货币变化通知
        public void OnDataListener()
        {
            if(!currentView.Equals("bet"))
            {
                return;
            }

            var mgr = LTActivityRacingManager.Instance;
            if(mgr.HasEnoughCurrency(currentGroup))
            {
                _betControls.RefreshBetCountDisplay(currentGroup);
            }
        }

        //帮助界面点击
        private void OnInfoButtonClicked()
        {
            GlobalMenuManager.Instance.Open("LTRuleUIView", EB.Localizer.GetString("ID_RACING_RULES"));
        }

        //选手被点到
        public void OnPlayerClicked(ConsecutiveClickCoolTrigger btn)
        {
            if(LTActivityRacingManager.Instance.HasBetPlayer(currentGroup))
            {
                return;
            }
            int num = int.Parse(btn.name.Replace("node_player", ""));
            _betControls.currentSelectPlayer = num;
            RefreshSelectFX(num);
        }

        //下注按钮点击
        private void OnBetButtonClicked()
        {
            if(!LTActivityRacingManager.Instance.HasEnoughAddBetTime(currentGroup))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_RACING_BET_TIPS_NOCOUNT"));
                return;
            }

            if(!LTActivityRacingManager.Instance.HasEnoughCurrency(currentGroup))
            {
                MessageTemplateManager.ShowMessage(LegionConfig.CodeHcUnenough, null, delegate (int r)
                {
                    if (r == 0)
                    {
                        InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                        GlobalMenuManager.Instance.Open("LTChargeStoreHud", null);
                    }
                });
                return;
            }
            
            var gd = LTActivityRacingManager.Instance.GetGroupData(currentGroup);
            int curBetPlayer = gd.CurBetPlayerNum;
            if(curBetPlayer > 0)
            {
                _betControls.AddBet(currentGroup);
            }
            else
            {
                Player player = null;
                if(_betControls.currentSelectPlayer > 0)
                {
                    player = _players[_betControls.currentSelectPlayer - 1];
                }

                _betControls.BetThePlayer(currentGroup, player, succ=>
                {
                    if(succ)
                    {
                        RefreshModelLight(_betControls.currentSelectPlayer);
                    }
                });
            }
        }
        
        //回放
        private void OnReplayButtonClicked()
        {
            var mgr = LTActivityRacingManager.Instance;
            var gd = mgr.GetGroupData(currentGroup);
            gd.CurIsReplay = true;
            LTActivityRacingManager.Instance.RequestAllResult(currentGroup, OnRacingDataResponse);
        }

        private void OnRacingDataResponse(int group, bool succ)
        {
            if(succ)
            {
                controller.Close();
                //数据准备完备，打开赛跑界面
                var ht = Johny.HashtablePool.Claim();
                ht.Add("group", group);
                GlobalMenuManager.Instance.Open("LTActivityRacingHud", ht);
            }
            else
            {
                EB.Debug.LogError("[Racing]没有比赛数据!!");
            }
        }
        #endregion
    }
}