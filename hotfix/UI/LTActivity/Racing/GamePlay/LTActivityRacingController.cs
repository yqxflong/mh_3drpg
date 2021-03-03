//LTActivityRacingController
//赛跑中
//Johny

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;

namespace Hotfix_LT.UI
{
    public class LTActivityRacingController : UIControllerHotfix
    {
        private class Ground
        {
            public Transform NodeGround;
            public List<Transform> NodeStartList = new List<Transform>(), NodeEndList = new List<Transform>();
        }

        private class SuperButton
        {
            public enum ButtonType
            {
                tEgg,
                tGo,
                tSpeed
            }
            public ConsecutiveClickCoolTrigger Button;
            public UITexture Mask;
            public UILabel Tick;
            public int MaxCDTick = 9; //秒
            public int CDTick; //秒
            public int UpdateSeq = 0;


            #region About Speed
            public List<UISprite> List_SpeedText;
            public int Index_SpeedText = 0;
            #endregion

            #region About Egg or Go
            public UITexture SP_EggOrGO;
            #endregion

            public void Show(bool shown)
            {
                Button.gameObject.CustomSetActive(shown);
            }

            public void ResetSpeed()
            {
                #region About speed
                List_SpeedText[0].gameObject.CustomSetActive(true);
                List_SpeedText[1].gameObject.CustomSetActive(false);
                Index_SpeedText = 0;
                #endregion
            }

            public void ResetEggGO()
            {
                #region About Egg Go
                Tick.gameObject.CustomSetActive(false);
                Mask.fillAmount = 0.0f;
                Mask.gameObject.CustomSetActive(false);
                CDTick = 0;
                ILRTimerManager.instance.RemoveTimerSafely(ref UpdateSeq);
                #endregion
            }
        } 

        private class Start321Go
        {
            public UILabel Label;

            public bool Finished{get;private set;}

            private Queue<string> _sp_nameQ;

            private WaitForSeconds _wait = new WaitForSeconds(1.0f);

            private Coroutine _co;

            public Start321Go(Transform node_ui)
            {
                Label = node_ui.GetComponent<UILabel>("lb_321go");
                _sp_nameQ = new Queue<string>();
                Reset();
            }

            private IEnumerator DoPlay()
            {
                while(_sp_nameQ.Count > 0)
                {
                    string spName = _sp_nameQ.Dequeue();
                    Label.text = spName;
                    if(spName.Equals("GO!"))
                    {
                        FusionAudio.PostEvent("UI/80.GO");
                        Label.transform.DOPunchScale(Vector3.one, 0.5f, 7, 10);
                    }
                    else
                    {
                        FusionAudio.PostEvent("UI/83.Time");
                    }
                    yield return _wait;
                }

                Label.gameObject.CustomSetActive(false);
                Finished = true;
            }

            public void Play()
            {
                Finished = false;
                Label.gameObject.CustomSetActive(true);
                _co = EB.Coroutines.Run(DoPlay());
            }

            public void Reset()
            {
                EB.Coroutines.Stop(_co);_co =null;
                _sp_nameQ.Clear();
                _sp_nameQ.Enqueue("3");
                _sp_nameQ.Enqueue("2");
                _sp_nameQ.Enqueue("1");
                _sp_nameQ.Enqueue("GO!");
                Label.text = "3";
                Label.gameObject.CustomSetActive(false);
                Finished = true;
            }
        }

        private int currentGroup = -1;

        //0: 鸡蛋  1：喝彩  2：加速
        private List<SuperButton> _buttons = new List<SuperButton>(); 
        private List<ActivityRacingPlayer> _curPlayers = new List<ActivityRacingPlayer>();
        private Ground _ground = new Ground();

        private Start321Go _start321go;

        private int _seq_timer = -1;

        private bool _hasSwitch2Result = false;

        private int _curSpeed = 1;

        public override void Awake()
        {
            base.Awake();

            InitControls_UI();
            InitControls_GP();
        }

        public override bool IsFullscreen()
        {
            return true;
        }

        public override IEnumerator OnAddToStack()
        {
            controller.transform.localPosition = new Vector3(0, 0, 3000);
            GameFlowControlManager.Instance.SetInActivityRacing(true);
            _seq_timer = ILRTimerManager.instance.AddTimer(1, 0, OnUpdate);
            LTActivityRacingManager.Instance.RegisterBroadcastReciever(OnBroadCast);
            _hasSwitch2Result = false;

            yield return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            GameFlowControlManager.Instance.SetInActivityRacing(false);
            LTActivityRacingManager.Instance.UnRegisterBroadcastReciever(OnBroadCast);
            ILRTimerManager.instance.RemoveTimerSafely(ref _seq_timer);
            //销毁选手模型
            DestroyRacingPlayers();
            //清除公告
            MessageTemplateManager.ClearUp();
            //恢复321go
            _start321go.Reset();
            //恢复播放速度
            ResetSpeed();
            //恢复鸡蛋喝彩CD
            ResetEggGoCD();

            DestroySelf();
            yield break;
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);

            var ht = param as Hashtable;
            currentGroup = (int)ht["group"];

            var mgr = LTActivityRacingManager.Instance;

            //3个选手装入缓存
            var gd = mgr.GetGroupData(currentGroup);
            foreach(var it in gd.RacingPlayers)
            {
                _curPlayers.Add(it.Value);
            }

            //载入选手模型
            LoadRacingPlayerModels();
            
            if(!mgr.HasBetPlayer(currentGroup))
            {
                _buttons[0].Show(false);
                _buttons[1].Show(false);
            }
            else
            {
                _buttons[0].Show(!gd.CurIsReplay);
                _buttons[1].Show(!gd.CurIsReplay);
            }
            _buttons[2].Show(gd.CurIsReplay);

            int sinceStart = mgr.GetRacingSinceStart(currentGroup);
            if(gd.CurIsReplay || sinceStart <= 0)
            {
                //321go
                _start321go.Play();
            }
            else
            {
                _start321go.Reset();
                //中间开跑
                int cnt = _curPlayers.Count;
                for(int i = 0; i < cnt; i++)
                {
                    var p = _curPlayers[i];
                    p.JumpToSince(sinceStart);
                }
            }
        }

        private void InitControls_UI()
        {
            var t = controller.transform;
            var node_ui = t.Find("node_ui");

            #region buttons
            {
                var sb = new SuperButton();
                var btn_egg = node_ui.GetComponent<ConsecutiveClickCoolTrigger>("btn_egg");
                btn_egg.clickEvent.Add(new EventDelegate(()=>OnButtonClicked(SuperButton.ButtonType.tEgg)));
                var mask = btn_egg.gameObject.GetComponent<UITexture>("sp_mask");
                var tick = btn_egg.gameObject.GetComponent<UILabel>("lb_tick");
                mask.gameObject.CustomSetActive(false);
                tick.gameObject.CustomSetActive(false);
                sb.Button = btn_egg;
                sb.Mask = mask;
                sb.Tick = tick;
                _buttons.Add(sb);
            }

            {
                var sb = new SuperButton();
                var btn_go = node_ui.GetComponent<ConsecutiveClickCoolTrigger>("btn_go");
                btn_go.clickEvent.Add(new EventDelegate(()=>OnButtonClicked(SuperButton.ButtonType.tGo)));
                var mask= btn_go.gameObject.GetComponent<UITexture>("sp_mask");
                var tick= btn_go.gameObject.GetComponent<UILabel>("lb_tick");
                mask.gameObject.CustomSetActive(false);
                tick.gameObject.CustomSetActive(false);
                sb.Button = btn_go;
                sb.Mask = mask;
                sb.Tick = tick;
                _buttons.Add(sb);
            }

            {
                var sb = new SuperButton();
                var btn_speed = node_ui.GetComponent<ConsecutiveClickCoolTrigger>("btn_speed");
                btn_speed.clickEvent.Add(new EventDelegate(()=>OnButtonClicked(SuperButton.ButtonType.tSpeed)));
                sb.Button = btn_speed;
                sb.List_SpeedText = new List<UISprite>{
                    btn_speed.gameObject.GetComponent<UISprite>("Deactive"),
                    btn_speed.gameObject.GetComponent<UISprite>("Active")
                };
                sb.Index_SpeedText = 0;
                _buttons.Add(sb);
            }
            #endregion
        
            #region 321go
            _start321go = new Start321Go(node_ui);
            #endregion
        }

        private void InitControls_GP()
        {
            var t = controller.transform;
            var node_ground = t.Find("node_ground");
            _ground.NodeGround = node_ground;

            for(int i = 1; i <= 3; i++)
            {
                _ground.NodeStartList.Add(node_ground.Find($"node_start_{i}"));
                _ground.NodeEndList.Add(node_ground.Find($"node_end_{i}"));
            }

            #region eggOrgo
            _buttons[0].SP_EggOrGO = node_ground.GetComponent<UITexture>("sp_egg");
            _buttons[1].SP_EggOrGO = node_ground.GetComponent<UITexture>("sp_go");
            #endregion
        }

        //载入所有选手模型
        private void LoadRacingPlayerModels()
        {
            foreach(var player in _curPlayers)
            {
                var beginPos = _ground.NodeStartList[player.pd.Num - 1].localPosition;
                var endPos = _ground.NodeEndList[player.pd.Num - 1].localPosition;
                player.LoadModel(_ground.NodeGround, beginPos, endPos);
            }
        }

        //销毁所有选手
        private void DestroyRacingPlayers()
        {
            var gd = LTActivityRacingManager.Instance.GetGroupData(currentGroup);
            foreach(var it in gd.RacingPlayers)
            {
                int pid = it.Key;
                it.Value.Destroy();
            } 
            gd.RacingPlayers.Clear();
            _curPlayers.Clear();
        }

        //刷新button的cd
        private void RefreshButtonCD(SuperButton sb)
        {
            sb.CDTick = sb.MaxCDTick;
            sb.Tick.text = sb.CDTick.ToString();
            sb.Tick.gameObject.CustomSetActive(true);
            sb.Mask.fillAmount = 1.0f;
            sb.Mask.gameObject.CustomSetActive(true);
            if(sb.UpdateSeq == 0)
            {
                sb.UpdateSeq = ILRTimerManager.instance.AddTimer(1000, 0, (seq)=>
                {
                    if(sb.CDTick > 0)
                    {
                        sb.CDTick--;
                        int cd = sb.CDTick;
                        float percent = cd * 1.0f / sb.MaxCDTick;
                        sb.Mask.fillAmount = percent;
                        sb.Tick.text = cd.ToString();
                        if(cd == 0)
                        {
                            sb.Tick.gameObject.CustomSetActive(false);
                            sb.Mask.fillAmount = 0.0f;
                            sb.Mask.gameObject.CustomSetActive(false);
                            sb.CDTick = 0;
                            ILRTimerManager.instance.RemoveTimerSafely(ref sb.UpdateSeq);
                        }
                    }
                });
            }
        }
        
        //切到结算界面
        private void Switch2ResultHud()
        {
            if(!_hasSwitch2Result)
            {
                _hasSwitch2Result = true;
                LTActivityRacingManager.Instance.RequestFinalResult(currentGroup, "result", OnFinalResultResponse);
            }
        }

        //获取目前跑在更前面的选手
        private ActivityRacingPlayer GetCurDisBeyondBetPlayer()
        {
            var gd = LTActivityRacingManager.Instance.GetGroupData(currentGroup);
            var pp = _curPlayers;
            Vector3 myBetDis = Vector3.zero;
            for(int i = 0; i < pp.Count; i++)
            {
                var arp = pp[i];
                if(arp.pd.Num == gd.CurBetPlayerNum)
                {
                    myBetDis = arp.GetCurPosition();
                }
            }
            for(int i = 0; i < pp.Count; i++)
            {
                var arp = pp[i];
                if(arp.GetCurPosition().y > myBetDis.y)
                {
                    return arp;
                }
            }

            return null;
        }

        //获取自己下注的选手
        private ActivityRacingPlayer GetCurMyBetPlayer()
        {
            var gd = LTActivityRacingManager.Instance.GetGroupData(currentGroup);
            var pp = _curPlayers;
            for(int i = 0; i < pp.Count; i++)
            {
                var arp = pp[i];
                if(arp.pd.Num == gd.CurBetPlayerNum)
                {
                    return arp;
                }
            }

            return null;
        }

        //重置加速
        private void ResetSpeed()
        {
            var sb = _buttons[2];
            sb.ResetSpeed();
            _curSpeed = 1;
        }

        //重置鸡蛋喝彩
        private void ResetEggGoCD()
        {
            var sb = _buttons[0];
            sb.ResetEggGO();
            sb = _buttons[1];
            sb.ResetEggGO();
        }

        #region About Button 回调事件
        private void OnUpdate(int seq)
        {
            float dt = Time.deltaTime;

            #region 321go
            if(!_start321go.Finished)
            {
                return;
            }
            #endregion

            #region 遍历选手
            bool allFinished = true;
            int cnt = _curPlayers.Count;
            for(int i = 0; i < cnt; i++)
            {
                var p = _curPlayers[i];
                allFinished = p.Update(dt) || p.IsVictory();
            }
            #endregion

            #region 检测全部结束
            if(allFinished)
            {
                EB.Debug.LogError("全部选手到达终点~~");
                Switch2ResultHud();
            }
            #endregion
        }

        private void OnButtonClicked(SuperButton.ButtonType type)
        {
            switch(type)
            {
                case SuperButton.ButtonType.tEgg:
                {
                    var sb = _buttons[0];
                    if(sb.CDTick <= 0)
                    {
                        RefreshButtonCD(sb);
                        EB.Coroutines.Run(DoThrowEgg());
                    }
                }
                    break;
                case SuperButton.ButtonType.tGo:
                {
                    var sb = _buttons[1];
                    if(sb.CDTick <= 0)
                    {
                        RefreshButtonCD(sb);
                        EB.Coroutines.Run(DoThrowGo());
                    }
                }
                    break;
                case SuperButton.ButtonType.tSpeed:
                {
                    var sb = _buttons[2];
                    var ll = sb.List_SpeedText;
                    ll[sb.Index_SpeedText].gameObject.CustomSetActive(false);
                    sb.Index_SpeedText = 1 - sb.Index_SpeedText;
                    ll[sb.Index_SpeedText].gameObject.CustomSetActive(true);
                    _curSpeed = sb.Index_SpeedText + 1;
                    int cnt = _curPlayers.Count;
                    for(int i = 0; i < cnt; i++)
                    {
                        var p = _curPlayers[i];
                        p.ChangeSpeed(_curSpeed);
                    }
                }
                    break;
                default:
                    break;
            }
        }

        //丢鸡蛋
        private IEnumerator DoThrowEgg()
        {
            var arp = GetCurDisBeyondBetPlayer();
            if(arp != null)
            {
                OnBroadCast(EB.Localizer.Format("ID_RACING_EVENT_EGG", arp.pd.Num, arp.pd.Name));
                FusionAudio.PostEvent("UI/82.Egg");
                //表现
                var playerPos = arp.GetCurPosition();
                var spEgg = _buttons[0].SP_EggOrGO;
                playerPos.z = -500;
                spEgg.transform.localPosition = playerPos;
                spEgg.transform.DOPunchScale(Vector3.one, 0.5f, 7, 10);

                spEgg.gameObject.CustomSetActive(true);

                yield return new WaitForSeconds(0.5f);

                spEgg.gameObject.CustomSetActive(false);
            }

            yield break;
        }

        //喝彩
        private IEnumerator DoThrowGo()
        {
            var arp = GetCurMyBetPlayer();
            if(arp != null)
            {
                var gd = LTActivityRacingManager.Instance.GetGroupData(currentGroup);
                var pd = gd.GetCurBetPlayerData();
                OnBroadCast(EB.Localizer.Format("ID_RACING_EVENT_GO", pd.Num, pd.Name));
                FusionAudio.PostEvent("SFX/Character/Common/HuoJianBaoZha");
                //表现
                var playerPos = arp.GetCurPosition();
                var spEgg = _buttons[1].SP_EggOrGO;
                playerPos.z = -500;
                spEgg.transform.localPosition = playerPos;
                spEgg.transform.DOPunchScale(Vector3.one, 0.5f, 7, 10);

                spEgg.gameObject.CustomSetActive(true);

                yield return new WaitForSeconds(0.5f);

                spEgg.gameObject.CustomSetActive(false);
            }

            yield break;
        }
        
        private void OnBroadCast(string text)
        {
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, text);
        }
        
        private void OnFinalResultResponse(int group, bool succ, string view)
        {
            if(view.Equals("bet"))
            {
                if(succ)
                {
                    controller.Close();
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("view", "bet");
                    ht.Add("group", group);
                    GlobalMenuManager.Instance.Open("LTActivityRacingBetHud", ht);
                }
                else
                {
                    EB.Debug.LogError("[Racing]没有合适的数据!!");
                }
            }
            else 
            {
                if(succ)
                {
                    controller.Close();
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("view", "result");
                    ht.Add("group", group);
                    GlobalMenuManager.Instance.Open("LTActivityRacingBetHud", ht);
                }
                else
                {
                    EB.Debug.LogError("[Racing]没有结算数据!!");
                }
            }
        }
        #endregion
    }
}