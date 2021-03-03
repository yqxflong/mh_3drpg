using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 触发3要素  1：collider 2：rigdbody（移动物体身上一定要有） 3：layer interactive
/// 在Locators 下面新增了Triggers 组，配合SceneTriggerComponent组件使用
/// </summary>
/// 
namespace Hotfix_LT.UI
{
    public class TriggerEntry
    {
        public string locator;
        public TriggerEntry(string locator)
        {
            this.locator = locator;
        }
    }

    public class SceneTriggerProcessor
    {
        protected List<TriggerEntry> m_Triggers;

        public virtual void InitTrigger(string scene_name)
        {
            if (m_Triggers == null) m_Triggers = new List<TriggerEntry>();
            m_Triggers.Clear();
        }

        public virtual void ActiveTrigger()
        {
            if (m_Triggers == null) return;
            for (int i = 0; i < m_Triggers.Count; i++)
            {
                GameObject locator = LocatorManager.Instance.GetLocator(m_Triggers[i].locator);
                if (locator != null)
                {
                    BoxCollider collider = locator.GetComponent<BoxCollider>();
                    if (collider != null)
                    {
                        if (!collider.enabled)
                        {
                            collider.enabled = true;
                        }
                    }
                }
            }
        }
        public virtual void ClearTrigger() { }

        public virtual bool ProcessTriggerEnter(string locator, GameObject trigger) { return false; }
    }

    public class SceneTriggerManager
    {

        private static SceneTriggerManager m_Instance;

        public static SceneTriggerManager Instance
        {
            get
            {
                if (m_Instance == null) m_Instance = new SceneTriggerManager();
                return m_Instance;
            }
        }

        private SceneTriggerManager()
        {
            InitTriggerProcessors();
            //TODO-D:未发现触发
            //Hotfix_LT.Messenger.AddListener<string, GameObject>(Hotfix_LT.EventName.OnLocatorTriggerEnterEvent, OnLocatorTriggerEnter);
        }

        private List<SceneTriggerProcessor> m_TriggerProcessors;

        public void Init() { }

        void InitTriggerProcessors()
        {
            m_TriggerProcessors = new List<SceneTriggerProcessor>();
            m_TriggerProcessors.Add(new DialogueTriggerProcessor());
            m_TriggerProcessors.Add(new NpcTriggerProcessor());
            m_TriggerProcessors.Add(new TransferTriggerProcessor());
        }


        /// <summary>
        /// 初始化当前场景的所有触发器， 在进入场景时调用
        /// </summary>
        public void InitTrigger(string scene_name)
        {
            for (int i = 0; i < m_TriggerProcessors.Count; i++)
                m_TriggerProcessors[i].InitTrigger(scene_name);
        }

        public void ActiveTrigger()
        {
            for (int i = 0; i < m_TriggerProcessors.Count; i++)
                m_TriggerProcessors[i].ActiveTrigger();
        }

        /// <summary>
        /// 响应触发器事件
        /// </summary>
        /// <param name="evt"></param>
        public void OnLocatorTriggerEnter(string locator, GameObject trigger)
        {
            bool triger = false;
            for (int i = 0; i < m_TriggerProcessors.Count; i++)
                triger = triger || m_TriggerProcessors[i].ProcessTriggerEnter(locator,trigger);
            if (!triger)
            {
                BoxCollider collider = trigger.GetComponent<BoxCollider>();
                if (collider != null)
                {
                    collider.enabled = false;
                }
            }
        }

        public void ClearTrigger()
        {
            for (int i = 0; i < m_TriggerProcessors.Count; i++)
                m_TriggerProcessors[i].ClearTrigger();
        }

    }

    public class DialogueTriggerEntry : TriggerEntry
    {
        public int id;//dialogue id
        public DialogueTriggerEntry(string locator, int id) : base(locator)
        {
            this.id = id;
        }
    }

    public class TransferTriggerEntry : TriggerEntry
    {
        public string name;//目标地图
        public Vector3 pos;//目标地点
        public ParticleSystem particle;
        public TransferTriggerEntry(string locator, string name, Vector3 pos) : base(locator)
        {
            this.name = name;
            this.pos = pos;
            this.particle = null;
        }
    }

    public class DialogueTriggerProcessor : SceneTriggerProcessor
    {
        private GameObject AttackTarget;
        public override void InitTrigger(string scene_name)
        {
            if (m_Triggers == null) m_Triggers = new List<TriggerEntry>();
            //if (m_Triggers.Count != 0 && SceneLogicManager.isCampaign()) return;//1 副本里面 只有第一次要重新
            m_Triggers.Clear();
            AttackTarget = null;
            string trigger_text = Hotfix_LT.Data.SceneTemplateManager.GetSceneTriggerDialgue(scene_name, SceneLogicManager.getMultyPlayerSceneType());
            if (string.IsNullOrEmpty(trigger_text)) return;
            string[] triggers = trigger_text.Split(',');
            if (triggers != null)
            {
                for (int i = 0; i < triggers.Length; i++)
                {
                    string[] splits = triggers[i].Split(':');
                    if (splits != null && splits.Length >= 2)
                    {
                        int dialogue_id = 0;
                        if (int.TryParse(splits[1], out dialogue_id))
                        {
                            m_Triggers.Add(new DialogueTriggerEntry(splits[0], dialogue_id));
                        }
                    }
                    else EB.Debug.LogError("Dialogue Trigger Format Error for {0}" , scene_name);
                }
                //凡是副本就额外加一个 这样在打最后一个怪时 如果失败复活 不会导致trigger全部init 呼应上面的注释
                //if (SceneLogicManager.isCampaign()) m_Triggers.Add(new DialogueTriggerEntry("", 0));

            }
        }

        public override void ActiveTrigger()
        {
            if (m_Triggers == null) return;
            for (int i = 0; i < m_Triggers.Count - 1; i++)
            {
                GameObject locator = LocatorManager.Instance.GetLocator(m_Triggers[i].locator);
                if (locator != null)
                {
                    BoxCollider collider = locator.GetComponent<BoxCollider>();
                    if (collider != null)
                    {
                        if (!collider.enabled)
                        {
                            collider.enabled = true;
                        }
                    }
                }
            }
        }

        public override bool ProcessTriggerEnter(string locator, GameObject trigger)
        {
            TriggerEntry target = null;
            for (int i = 0; i < m_Triggers.Count; i++)
            {
                if (m_Triggers[i].locator.Equals(locator))
                {
                    target = m_Triggers[i];
                    DialogueTriggerEntry dte = target as DialogueTriggerEntry;
                    PlayerManager.LocalPlayerController().TargetingComponent.SetMovementTarget(Vector3.zero, true);
                    AttackTarget = PlayerManager.LocalPlayerController().TargetingComponent.AttackTarget;
                    PlayerManager.LocalPlayerController().TargetingComponent.AttackTarget = null;
                    PlayerController.LocalPlayerDisableNavigation();
                    DialoguePlayUtil.Instance.Play(dte.id, DialogueOver);
                    break;
                }
            }
            if (target != null)
            {
                m_Triggers.Remove(target);
                return true;
            }
            return false;
        }

        public void DialogueOver()
        {
            PlayerController.LocalPlayerEnableNavigation();
            //PlayerManager.LocalPlayerController().TargetingComponent.AttackTarget = AttackTarget;
            if (AttackTarget != null)
            {
                PlayerController controller = PlayerManager.LocalPlayerController();
                if (null != controller && controller.TargetingComponent != null)
                {
                    controller.TargetingComponent.AttackTarget = AttackTarget;
                }
            }

            //GuideManager.Instance.ExcuteEx(MainLandLogic.GetInstance().ThemeLoadManager.GetSceneRootObject().transform, null);
            SceneLogic scene = MainLandLogic.GetInstance();
            if (scene != null && scene.ThemeLoadManager != null && scene.ThemeLoadManager.GetSceneRootObject() != null)
            {
                GuideManager.Instance.ExcuteEx(scene.ThemeLoadManager.GetSceneRootObject().transform, null);
            }
        }

        //副本不用清理
        public override void ClearTrigger()
        {
            if (m_Triggers != null)
            {
                //if (SceneLogicManager.isCampaign() ||(SceneLogicManager.isCampaignCombat())) return;
                m_Triggers.Clear();
            }
        }
    }

    public class NpcTriggerProcessor : SceneTriggerProcessor
    {
        public override void InitTrigger(string scene_name) { }

        public override bool ProcessTriggerEnter(string locator, GameObject trigger) { return false; }

        public void NpcLocatorTriggerEnter(string locator, GameObject triggerOb)
        {
            EnemyController enemy = MainLandLogic.GetInstance().GetEnemyController(locator);
            if (enemy==null)
            {
                return;
            }
            Player.EnemyHotfixController ehc = enemy.HotfixController._ilrObject as Player.EnemyHotfixController;
            if (ehc.IsBodyActive())
            {
                return;
            }

            NpcTriggerAction(locator, triggerOb);
        }

        public void NpcTriggerAction(string locator, GameObject triggerOb)
        {
            // 执行触发逻辑
            //将玩家的目标点设置为触发器位罿
            Vector3 dest = new Vector3();
            dest.x = triggerOb.transform.localPosition.x;
            dest.y = triggerOb.transform.localPosition.y;
            dest.z = triggerOb.transform.localPosition.z;
            PlayerManager.LocalPlayerController().TargetingComponent.SetMovementTarget(Vector3.zero, true);
            EnemyController enemy = MainLandLogic.GetInstance().GetEnemyController(locator);
            if (enemy != null)
            {
                Player.EnemyHotfixController ehc = enemy.HotfixController._ilrObject as Player.EnemyHotfixController;
                ehc.DoAppearingWay();
            }
            NpcTriggerActionOver();
        }

        public void NpcTriggerActionOver()
        {
            //触发逻辑完了回调
            //允许事件输入
            //InputBlockerManager.Instance.UnBlock(InputBlockReason.CONVERT_FLY_ANIM);
        }
    }

    public class TransferTriggerProcessor : SceneTriggerProcessor
    {
        public override void InitTrigger(string scene_name)
        {
            base.InitTrigger(scene_name);
            //locator1:QingDiYuYuan:(1,2,3);locator1:QingDiYuYuan:(1,2,3)
            //string trigger_text = "Triggers_1_4:QingDiYuYuan:(31.18,0.0,36.67);Triggers_1_2:LongGong:(35,-0.5,75)";
            m_Triggers = Hotfix_LT.Data.SceneTemplateManager.GetSceneTransferPoints(scene_name, SceneLogicManager.getMultyPlayerSceneType());
            //if (string.IsNullOrEmpty(trigger_text)) return;
            //string[] triggers = trigger_text.Split(';');
            //if (triggers != null)
            //{
            //    for (int i = 0; i < triggers.Length; i++)
            //    {
            //        string[] splits = triggers[i].Split(':');
            //        if (splits != null && splits.Length >= 3)
            //        {
            //            Vector3 pos = GM.LitJsonExtension.ImportVector3(splits[2]);
            //            m_Triggers.Add(new TransferTriggerEntry(splits[0], splits[1], pos));

            //        }
            //        else EB.Debug.LogError("Transfer Trigger Format Error for " + scene_name);
            //    }
            //}
            IsTransfering = false;
        }

        public override void ActiveTrigger()
        {
            if (m_Triggers == null) return;
            for (int i = 0; i < m_Triggers.Count; i++)
            {
                GameObject locator = LocatorManager.Instance.GetLocator(m_Triggers[i].locator);
                if (locator != null)
                {
                    TransferTriggerEntry tte = m_Triggers[i] as TransferTriggerEntry;
                    ParticleSystem mFX = PSPoolManager.Instance.Use(locator, "fx_p_CJ_chuansongmen");
                    if (mFX != null)
                    {
                        mFX.transform.position = locator.transform.position;
                        mFX.EnableEmission(true);
                        mFX.Simulate(0.0001f, true, true);
                        mFX.Play(true);
                        tte.particle = mFX;
                    }
                    BoxCollider collider = locator.GetComponent<BoxCollider>();
                    if (collider != null)
                    {
                        if (!collider.enabled)
                        {
                            collider.enabled = true;
                        }
                    }
                }
            }
        }

        private bool IsTransfering = false;
        public override bool ProcessTriggerEnter(string locator, GameObject trigger)
        {
            TriggerEntry target = null;
            if (m_Triggers == null)
            {
                EB.Debug.LogWarning("TransferTriggerProcessor.ProcessTriggerEnter: m_Triggers = null!");
                return false;
            }

            for (int i = 0; i < m_Triggers.Count; i++)
            {
                if (m_Triggers[i].locator.Equals(locator))
                {
                    //防止重入，重入会导致场景多次加载
                    if (SceneLogic.SceneState != SceneLogic.eSceneState.SceneLoop || IsTransfering)
                    {
                        return true;
                    }
                    IsTransfering = true;
                    target = m_Triggers[i];
                    TransferTriggerEntry tte = target as TransferTriggerEntry;
                    PlayerManager.LocalPlayerController().TargetingComponent.SetMovementTarget(Vector3.zero, true);
                    //PlayerController.LocalPlayerDisableNavigation();
                    UIStack.Instance.ExitStack();
                    EB.Debug.Log("Transfer!!!");
                    Vector3 from_pos = new Vector3(25.0f, 0.0f, 45.0f);
                    SceneLogic.Transfer("mapview", from_pos, 90.0f, tte.name, tte.pos, 90.0f, delegate (bool sucessful)
                    {
                        if (!sucessful)
                        {
                            IsTransfering = false;
                        }
                    });
                    return true;
                }
            }
            return false;
        }

        public override void ClearTrigger()
        {
            if (m_Triggers != null)
            {
                for (int i = 0; i < m_Triggers.Count; i++)
                {
                    TransferTriggerEntry tte = m_Triggers[i] as TransferTriggerEntry;
                    if (tte.particle != null)
                    {
                        tte.particle.name = tte.particle.name.Replace("(Clone)", "");
                        PSPoolManager.Instance.Recycle(tte.particle);
                    }
                }
            }
        }
    }
}