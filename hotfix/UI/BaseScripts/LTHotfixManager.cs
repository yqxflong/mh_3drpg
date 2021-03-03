using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EB.Sparx;
using PixelCrushers.DialogueSystem.SequencerCommands;

namespace Hotfix_LT.UI
{
    public class ManagerUnit
    {
        public virtual void Initialize(Config config)
        {
        }

        public virtual void Dispose()
        {
        }

        public virtual void OnLoggedIn()
        {
        }

        public virtual void Connect()
        {
        }

        public virtual void Disconnect(bool isLogout)
        {
        }

        public virtual void Async(string message, object payload)
        {
        }
        
        // public virtual void Update()
        // {
        // }
        
        public virtual void OnEnteredBackground()
        {
        }

        public virtual void OnEnteredForeground()
        {
        }
    }

    public interface IManagerUnitUpdatable
    {
        void Update();
    }

    public class LTHotfixManager : ManagerHotfix
    {
        private static List<ManagerUnit> ManagerList;
        
        private static List<IManagerUnitUpdatable> UpdateManagerList;

        private void AddManager()
        {
            ManagerList.Add(new GameDataSparxManager());
            ManagerList.Add(new AutoRefreshingManager());
            ManagerList.Add(new LTWorldBossDataManager());
            ManagerList.Add(new LTWelfareDataManager());
            ManagerList.Add(new LTSpeedSnatchLogicILRObject());
            ManagerList.Add(new FriendManager());
            ManagerList.Add(new ArenaManager());
            ManagerList.Add(new LTHeroBattleLogic());
            ManagerList.Add(new LTClimingTowerManager());
            ManagerList.Add(new MailBoxManager());
            ManagerList.Add(new LTResourceInstanceManager());
            ManagerList.Add(new LTDrawCardDataManager());
            ManagerList.Add(new LadderManager());
            ManagerList.Add(new LTPartnerDataManager());
            ManagerList.Add(new LTPartnerEquipDataManager());
            ManagerList.Add(new LegionLogic());
            ManagerList.Add(new LTDailyDataManager());
            ManagerList.Add(new AlliancesManager());
            ManagerList.Add(new LTPartnerHandbookManager());
            ManagerList.Add(new LTChargeManager());
            ManagerList.Add(new LTInstanceMapModel());            
            ManagerList.Add(new NationManager());
            ManagerList.Add(new LTAwakeningInstanceManager());
            ManagerList.Add(new CombatManager());
            ManagerList.Add(new LTFormationDataManager());
            ManagerList.Add(new LTChatManager());
            ManagerList.Add(new LTNewHeroBattleManager());
            ManagerList.Add(new SocialIntactManager());
            ManagerList.Add(new TeamManager());
            ManagerList.Add(new LTMainHudManager());
            ManagerList.Add(new SceneManager());
            ManagerList.Add(new GuideNodeManager());
            ManagerList.Add(new BroadcastMessageManager());
            ManagerList.Add(new LoginManager());
            ManagerList.Add(new InvitesManager());
            ManagerList.Add(new TaskManager());
            ManagerList.Add(new SocialManager());
            ManagerList.Add(new LTRedPointSystem());
            ManagerList.Add(new HonorArenaManager());
            ManagerList.Add(new PlayerInviteManager());
            ManagerList.Add(new LTLegionWarManager());
            ManagerList.Add(new MercenaryManager());
			ManagerList.Add(new LTVIPDataManager());
            ManagerList.Add(new LTPromotionManager());
        }

        public override void Initialize(Config config)
        {
            if (ManagerList == null)
            {
                ManagerList = new List<ManagerUnit>();
                AddManager();
            }
            for (int i = 0; i < ManagerList.Count; ++i)
            {
                ManagerList[i].Initialize(config);
            }

            if (UpdateManagerList == null)
            {
                UpdateManagerList = new List<IManagerUnitUpdatable>();
                for (int i = 0; i < ManagerList.Count; ++i)
                {
                    if (ManagerList[i] is IManagerUnitUpdatable)
                    {
                        UpdateManagerList.Add((IManagerUnitUpdatable)ManagerList[i]);
                    }
                }
            }
            
        }

        public override void Dispose()
        {
            for (int i=0;i<ManagerList .Count;++i)
            {
                ManagerList[i].Dispose();
            }
        }

        public override void OnLoggedIn()
        {
            for (int i = 0; i < ManagerList.Count; ++i)
            {
                ManagerList[i].OnLoggedIn();
            }
        }

        public override void Connect()
        {
            for (int i = 0; i < ManagerList.Count; ++i)
            {
                ManagerList[i].Connect();
            }
        }

        public override void Disconnect(bool isLogout)
        {
            for (int i = 0; i < ManagerList.Count; ++i)
            {
                ManagerList[i].Disconnect(isLogout);
            }
        }

        public override void Async(string message, object payload)
        {
            string[] split = message.Split('|');
            if(split.Length > 1)
            {
                var manager = GetManager(split[0]);
                if (manager != null)
                {
                    manager.Async(split[1], payload);
                }
                else
                {
                   EB.Debug.LogWarning("Failed to find manager: {0}" , split[0]);
                }
            }
        }

        public static ManagerUnit GetManager(string name)
        {
            var ebname =string .Format("Hotfix_LT.UI.{0}",name);
            for (int i = 0, cnt = ManagerList.Count; i < cnt; ++i)
            {
                var manager = ManagerList[i];
                var n = manager.GetType().Name;

                if (n == name || n == ebname)
                {
                    return manager;
                }
            }
            return null;
        }

        public static T GetManager<T>() where T : ManagerUnit {
            return GetManager(typeof(T).Name) as T;
        }
        
        public override void Update()
        {
            for (int i = 0; i < UpdateManagerList.Count; ++i)
            {
                UpdateManagerList[i].Update();
            }
        }

        public override void OnEnteredBackground()
        {
            for (int i = 0; i < ManagerList.Count; ++i)
            {
                ManagerList[i].OnEnteredBackground();
            }
        }

        public override void OnEnteredForeground()
        {
            for (int i = 0; i < ManagerList.Count; ++i)
            {
                ManagerList[i].OnEnteredForeground();
            }
        }
    }
}
