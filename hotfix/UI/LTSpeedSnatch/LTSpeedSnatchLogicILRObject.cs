using System;
using System.Collections.Generic;
using EB.Sparx;
using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTSpeedSnatchLogicILRObject : ManagerUnit
    {
        private Coroutine _activeMonitor;
        private const int activityId = 9007;
        
        public override void Initialize(Config config)
        {
            LTSpeedSnatchILRModel.GetInstance().CurrentTagAttr = null;
            Hotfix_LT.Messenger.AddListener("LTSpeedSnatchEvent.IdleFollow", OnIdleFollow);
            Hotfix_LT.Messenger.AddListener<string, string>("LTSpeedSnatchEvent.TouchEnemy", OnTouchEnemy);
            Hotfix_LT.Messenger.AddListener("GetSpeedSnatchBaseData", GetSpeedSnatchBase);
        }

        public override void Dispose()
        {
            Hotfix_LT.Messenger.RemoveListener("LTSpeedSnatchEvent.IdleFollow", OnIdleFollow);
            Hotfix_LT.Messenger.RemoveListener<string, string>("LTSpeedSnatchEvent.TouchEnemy", OnTouchEnemy);
            Hotfix_LT.Messenger.RemoveListener("GetSpeedSnatchBaseData", GetSpeedSnatchBase);
        }

        public override void Connect()
        {
            LTSpeedSnatchILRModel.GetInstance();

            GameDataSparxManager.Instance.Register<LTSpeedSnatchBaseNodeData>("special_activity.mainland_ghost");
            GameDataSparxManager.Instance.Register<LTSpeedSnatchRewardData>("combat.rewards");
            GameDataSparxManager.Instance.Register<LTSpeedSnatchNpcData>("mainlands.npc_list");

            Dictionary<string, MainlandsGhostRewardTemplate> gRewards = Hotfix_LT.Data.SceneTemplateManager.Instance.GetAllMainlandsGhostReward();

            LTSpeedSnatchILRModel.GetInstance().ListGhostReward.Clear();
            GhostReward but = null;
            foreach (var v in gRewards)
            {
                if (v.Value.type.Equals("111") || v.Value.type.Equals("222") || v.Value.type.Equals("333") || v.Value.type.Equals("123"))
                {
                    GhostReward reward = new GhostReward();
                    reward.attrs = new int[v.Value.type.Length];
                    reward.spriteNames = new string[reward.attrs.Length];
                    for (int i = 0; i < reward.attrs.Length; i++)
                    {
                        reward.attrs[i] = int.Parse(v.Value.type[i].ToString());
                        reward.spriteNames[i] = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[(Hotfix_LT.Data.eRoleAttr)reward.attrs[i]]; 
                    }
                    reward.rewards = v.Value.reward;
                    LTSpeedSnatchILRModel.GetInstance().ListGhostReward.Add(reward);
                }
                else if (but == null && v.Value.type.Equals("112")) //认为112和其他组都是一样的道具
                {
                    but = new GhostReward();
                    but.attrs = new int[v.Value.type.Length];
                    but.spriteNames = new string[but.attrs.Length];
                    for (int i = 0; i < but.attrs.Length; i++)
                    {
                        but.attrs[i] = int.Parse(v.Value.type[i].ToString());
                        but.spriteNames[i] = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[(Hotfix_LT.Data.eRoleAttr)but.attrs[i]]; 
                    }
                    but.rewards = v.Value.reward;
                    LTSpeedSnatchILRModel.GetInstance().ListGhostReward.Add(but);
                }

            }

            LTSpeedSnatchILRModel.GetInstance().OpenTime = Hotfix_LT.Data.EventTemplateManager.Instance.GetActivityTimeByCronJobsName("main_land_ghost_start");
            LTSpeedSnatchILRModel.GetInstance().EndTime = Hotfix_LT.Data.EventTemplateManager.Instance.GetActivityTimeByCronJobsName("main_land_ghost_stop");
            LTSpeedSnatchILRModel.GetInstance().isActiviyMonitor = true;
            _activeMonitor = EB.Coroutines.Run(ActivityMonitor());
        }



        public override void Disconnect(bool isLogout)
        {
            if (_activeMonitor != null)
            {
                EB.Coroutines.Stop(_activeMonitor);
            }
            LTSpeedSnatchILRModel.GetInstance().isActiviyMonitor = false;
        }

        IEnumerator ActivityMonitor()
        {
            while (LTSpeedSnatchILRModel.GetInstance().isActiviyMonitor)
            {
                if (SceneLogicManager.isMainlands()) ActivityMonitorFunc();
                yield return LTSpeedSnatchILRModel.GetInstance().monitorWFS;
            }
        }

        private void ActivityMonitorFunc()
        {
            if (!Hotfix_LT.Data.EventTemplateManager.Instance.GetRealmIsOpen("main_land_ghost_start"))
            {
                if (LTSpeedSnatchILRModel.GetInstance().IsActive)
                    LTSpeedSnatchILRModel.GetInstance().IsActive = false;
            }
            else if (Hotfix_LT.Data.EventTemplateManager.Instance.IsTimeOKEX(LTSpeedSnatchILRModel.GetInstance().OpenTime, LTSpeedSnatchILRModel.GetInstance().EndTime)) //Hotfix_LT.Data.EventTemplateManager.Instance.IsTimeOK("main_land_ghost_start", "main_land_ghost_stop"))
            {
                if (AutoRefreshingManager.Instance.GetRefreshed(AutoRefreshingManager.RefreshName.SpecialActivity))
                {
                    LTSpeedSnatchILRModel.GetInstance().IsReset = true;
                    LTSpeedSnatchILRModel.GetInstance().CurrentTagAttr = null;
                }
                //在本地活动时间,进行挑战次数判断
                int curTimes =0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("special_activity.{0}.c_times", activityId), out curTimes);
                if (!LTSpeedSnatchILRModel.GetInstance().IsReset && !(LTSpeedSnatchILRModel.GetInstance().CurrentTagAttr!=null&&LTSpeedSnatchILRModel.GetInstance().CurrentTagAttr.Length==3)&& curTimes >= challengeTimes )
                {
                    //Debug.Log("SpeedSnatch times is over!");
                    if (LTSpeedSnatchILRModel.GetInstance().IsActive)
                        LTSpeedSnatchILRModel.GetInstance().IsActive = false;
                }
                else if (!LTSpeedSnatchILRModel.GetInstance().IsActive)
                    LTSpeedSnatchILRModel.GetInstance().IsActive = true;
            }
            else if (LTSpeedSnatchILRModel.GetInstance().IsActive && !LTSpeedSnatchILRModel.GetInstance().isNpcGhostFind)
            {
                //本地活动时间截止 且 怪物移除
                if(LTSpeedSnatchILRModel.GetInstance().IsActive)
                    LTSpeedSnatchILRModel.GetInstance().IsActive = false;
            }
        }
        

        /// <summary>
        /// 这个消息需要服务器告诉推送给哪个模块的 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="payload"></param>
        public override void Async(string message, object payload)
        {
            switch (message)
            {
                default:break;
            }
        }

        private int challengeTimes =6;//总挑战次数
        public override void OnLoggedIn()
        {
            Hotfix_LT.Data.SpecialActivityTemplate temp = Hotfix_LT.Data.EventTemplateManager.Instance.GetSpecialActivity(activityId);
            if(temp!=null) challengeTimes = temp.times;
            LTSpeedSnatchILRModel.GetInstance().hasRequire =false;
        }

        IEnumerator TestInitUI()
        {
            while (GlobalMenuManager.Instance == null)
            {
                yield return null;
            }
            GlobalMenuManager.Instance.Open("LTSpeedSnatchHudUI");
        }

        void OnIdleFollow()
        {
            if (LTSpeedSnatchILRModel.GetInstance().hasRequire)return;

            if (MainLandLogic.GetInstance().SceneId == 0) return;
            LTSpeedSnatchILRModel.GetInstance().firstEnterMainLand = true;
            GetSpeedSnatchBase();
        }
        
        void OnTouchEnemy(string ghostId,string sceneName)
        {
            int curTimes = 0;
            DataLookupsCache.Instance.SearchIntByID(string.Format("special_activity.{0}.c_times", activityId), out curTimes);
            if (!LTSpeedSnatchILRModel.GetInstance().IsReset && curTimes >= challengeTimes)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_SPEED_SNATCH_TIMESOVER"));
                return;
            }
            PostSpeedSnatchAttackEnemy(ghostId, sceneName);
        }

        void OnNewNpc()
        {
            if (!LTSpeedSnatchILRModel.GetInstance().IsActive)
            {
                LTSpeedSnatchILRModel.GetInstance().IsActive = true;
            }
        }

        void OnLeaveNpc()
        {
            if (LTSpeedSnatchILRModel.GetInstance().IsActive)
            {
                LTSpeedSnatchILRModel.GetInstance().IsActive = false;
            }
        }

        void OnOpenAward()
        {
            if (LTSpeedSnatchILRModel.GetInstance().listRewardShowItemData.Count>0)
            {
                GlobalMenuManager.Instance.Open("LTShowRewardView", LTSpeedSnatchILRModel.GetInstance().listRewardShowItemData);
            }
        }

        public void GetSpeedSnatchBase()
        {
            Func<Response,bool> func = (EB.Sparx.Response response) => {
                if((!response.sucessful|| response.fatal)&&!response.error.Equals(string.Empty))
                {
                    if(response.error.Equals("ID_NOT_IN_ACTIVITY_TIME")) //活动没有开启 正常截断
                    {
                        return true;
                    }
                }
                return false; }; //如果有属于正常处理的异常就写错误提示并return true
            LTHotfixApi.GetInstance().ExceptionFunc = func;
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/specialactivity/getUserGhostData");
            request.AddData("sceneId", MainLandLogic.GetInstance().SceneId.ToString());
            LTHotfixApi.GetInstance().BlockService(request, (Hashtable res) => 
            {
                LTHotfixApi.GetInstance().FetchDataHandler(res);
                Disconnect(false);
                Connect();
            });
        }
        private bool _isPostAttackEnemy;

        public void PostSpeedSnatchAttackEnemy(string ghostId,string sceneId)
        {
            if (_isPostAttackEnemy) return; //同时只能允许一次 等待回复
            _isPostAttackEnemy = true;
            InputBlockerManager.Instance.Block(InputBlockReason.UI_SERVER_REQUEST, 2);
            LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) =>{
                _isPostAttackEnemy = false;
                if ((!response.sucessful || response.fatal) && !response.error.Equals(string.Empty))
                {
                    if (response.error.Equals("ghost is busy")) //活动没有开启 正常截断
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTSpeedSnatchLogicILRObject_9900"));
                        return true;
                    }
                    else if(response.error.Equals("layout not found"))
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTSpeedSnatchLogicILRObject_10156"));
                        return true;
                    }
                    else if (response.error.Equals("not such ghost"))
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTSpeedSnatchLogicILRObject_10413"));
                        return true;
                    }
                    else if(response.error.Equals("level too low"))
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTSpeedSnatchLogicILRObject_10662"));
                        return true;
                    }//ID_NOT_IN_ACTIVITY_TIME
                    else if (response.error.Equals("ID_NOT_IN_ACTIVITY_TIME"))
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTSpeedSnatchLogicILRObject_10961"));
                        return true;
                    }
                    else if(response.error.Equals("max times"))
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_SPEED_SNATCH_TIMESOVER"));
                        return true;
                    }
                }
                return false;  };
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/specialactivity/startGhostChallenge");
            request.AddData("sceneId", sceneId);
            request.AddData("ghostId", ghostId);
            LTHotfixApi.GetInstance().BlockService(request, (Hashtable res) =>
            {
                UIStack.Instance.ShowLoadingScreen(null, false, true);
                LTHotfixApi.GetInstance().FetchDataHandler(res);
            });
        }
    }
    public class LTSpeedSnatchILRModel
    {
        private static LTSpeedSnatchILRModel _instance;
        public static LTSpeedSnatchILRModel GetInstance()
        {
            if(_instance ==null)
            {
                _instance = new LTSpeedSnatchILRModel();
            }
            return _instance;
        }

        public int AttrLength = 0;
        public bool IsReset = false;
        public bool firstEnterMainLand = false;
        public bool hasRequire = false;

        private int[] _currentTagAttr;
        public int[] CurrentTagAttr
        {
            get { return _currentTagAttr; }
            set
            {
                _currentTagAttr = value;
                if (LTSpeedSnatchHotfixEvent.SpeedSnatchBase != null)
                    LTSpeedSnatchHotfixEvent.SpeedSnatchBase(value);
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                var activitytmp = Hotfix_LT.Data.EventTemplateManager.Instance.GetSpecialActivity(9007);
                if (activitytmp != null)
                {
                    var func = Data.FuncTemplateManager.Instance.GetFunc(activitytmp.funcId);
                    if (func!=null&&!func.IsConditionOK())  
                    {
                        _isActive = false;//不满足等级时 强制关闭
                    }
                } 

               
                if (LTSpeedSnatchHotfixEvent.SpeedSnatchActive != null)
                    LTSpeedSnatchHotfixEvent.SpeedSnatchActive(_isActive);
            }
        }

        private List<GhostReward> _listGhostReward;
        public List<GhostReward> ListGhostReward
        {
            get
            {
                return _listGhostReward;
            }
            set
            {
                _listGhostReward = value;
            }
        }

        public Hotfix_LT.Data.LTActivityTime OpenTime =new Hotfix_LT.Data.LTActivityTime (), EndTime= new Hotfix_LT.Data.LTActivityTime();

        public List<LTShowItemData> listRewardShowItemData;
        //public string dateCondition;
        //public ArrayList timeCondition;
        public WaitForSeconds monitorWFS;
        public bool isActiviyMonitor;
        public bool isNpcGhostFind;

        protected LTSpeedSnatchILRModel()
        {
            LTSpeedSnatchHotfixEvent.RefreshModel = OnRefreshModel;
            ListGhostReward = new List<GhostReward>();
            listRewardShowItemData = new List<LTShowItemData>();
            monitorWFS = new WaitForSeconds(10f);
        }

        private void OnRefreshModel()
        {
            CurrentTagAttr = CurrentTagAttr;
            IsActive = IsActive;
        }
    }

    public class GhostReward
    {
        public int[] attrs;
        public string[] spriteNames;
        public List<LTShowItemData> rewards;
    }

    public class LTSpeedSnatchBaseNodeData: INodeData
    {
        public bool isMainlandGhostTime;
        public int [] currentTagAttr;

        public void CleanUp()
        {
            
        }

        public object Clone()
        {
            return new LTSpeedSnatchBaseNodeData();
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);

        }

        public void OnUpdate(object obj)
        {
            isMainlandGhostTime = EB.Dot.Bool("isMainlandGhostTime", obj, false);
            string str = EB.Dot.String("MainlandGhostTag", obj, string.Empty);

            if (str.Equals(string.Empty))
            {
                currentTagAttr = null;
            }
            else
            {
                currentTagAttr = new int[str.Length];
                for (int i = 0; i < currentTagAttr.Length; i++)
                {
                    currentTagAttr[i] = int.Parse(str[i].ToString());
                }
            }
            //Debug.LogError("SpeedSnatch TagAttr Change!");
            LTSpeedSnatchILRModel.GetInstance().IsReset = false;
            LTSpeedSnatchILRModel.GetInstance().IsActive = isMainlandGhostTime;
            LTSpeedSnatchILRModel.GetInstance().CurrentTagAttr = currentTagAttr;
            if (!LTSpeedSnatchILRModel.GetInstance().hasRequire) LTSpeedSnatchILRModel.GetInstance().hasRequire = true;
        }
    }

    public class LTSpeedSnatchRewardData: INodeData
    {
        public void OnUpdate(object obj)
        {
            LTSpeedSnatchILRModel.GetInstance().listRewardShowItemData.Clear();
            ArrayList al = obj as ArrayList;
            for (var k = 0; k < al.Count; k++)
            {
                var v = al[k];
                Hashtable has = v as Hashtable;
                if (has.ContainsKey("TagAttrRedeemItem"))
                {
                    ArrayList items = has["TagAttrRedeemItem"] as ArrayList;
                    if (items.Count>0)
                    {
                        for(int i=0;i<items.Count;i++)
                        {
                            Hashtable ih = items[i] as Hashtable;
                            string type = EB.Dot.String("type",ih,string.Empty);
                            string id = EB.Dot.String("data", ih, string.Empty);
                            int count = EB.Dot.Integer("quantity", ih, 0);
                            LTShowItemData data = new LTShowItemData(id, count, type);
                            LTSpeedSnatchILRModel.GetInstance().listRewardShowItemData.Add(data);
                        }
                    }
                }
            }
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public void CleanUp()
        {
            
        }

        public object Clone()
        {
            return new LTSpeedSnatchRewardData();
        }
    }

    public class LTSpeedSnatchNpcData: INodeData
    {
        public void CleanUp()
        {
            
        }

        public object Clone()
        {
            return new LTSpeedSnatchNpcData();
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public void OnUpdate(object obj)
        {
            bool isFind = false;
            Hashtable hs = obj as Hashtable;
            string key = "role";
            string use = "ghost";
            foreach (var v in hs.Values)
            {
                string role = EB.Dot.String(key, v, string.Empty);
                if(role.Equals(use))
                {
                    isFind = true;
                    break;
                }
            }
            LTSpeedSnatchILRModel.GetInstance().isNpcGhostFind = isFind;
        }

    }
}
