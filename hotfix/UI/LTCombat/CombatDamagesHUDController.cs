using UnityEngine;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class CombatDamagesHUDController : DynamicMonoHotfix
    {
        public CombatDamagesHUD HUDTemplate;
    	public CombatFloatFontUIHUD NewFloatFontHUDTemplate;
        private Transform cachedTransform = null;
        private Dictionary<GameObject, float> DamageEventTimeRecord;

        #region Instance
        private static CombatDamagesHUDController _instance = null;
        /// <summary>
        /// 不一定存在自行判空！
        /// </summary>
        /// <value></value>
        public static CombatDamagesHUDController Instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            HUDTemplate = (mDMono.ObjectParamList[0] as GameObject).GetMonoILRComponent<CombatDamagesHUD>();
            NewFloatFontHUDTemplate = (mDMono.ObjectParamList[1] as GameObject).GetMonoILRComponent<CombatFloatFontUIHUD>();

            cachedTransform = mDMono.transform;
            DamageEventTimeRecord = new Dictionary<GameObject, float>();
        }
    
        public override void Start()
        {
            InitHUDSPool();
        }
        
        public override void OnEnable()
        {
            //本单例生效
            _instance = this;

            Hotfix_LT.Messenger.AddListener<Hotfix_LT.Combat.CombatHitDamageEvent>(Hotfix_LT.EventName.CombatHitDamageEvent, OnHitCombatantListener);
        }

        public override void OnDisable()
        {
            //本单例失效
            _instance = null;

            Hotfix_LT.Messenger.RemoveListener<Hotfix_LT.Combat.CombatHitDamageEvent>(Hotfix_LT.EventName.CombatHitDamageEvent, OnHitCombatantListener);

            ClearTimer();
            StopAllCoroutines();
    
            var huds = hudsPool.ToArray();
            var t = mDMono.transform;
            var childCount = t.childCount;
            
            for (int i = 0; i < childCount; ++i)
            {
                var child = t.GetChild(i).GetMonoILRComponent<CombatDamagesHUD>();

                if (child != null && System.Array.Find(huds, hud => hud == child) == null)
                {
                    PutHUDInPool(child);
                }
            }
        }
        
        public void OnHitCombatantListener(Hotfix_LT.Combat.CombatHitDamageEvent e)
        {
            Hotfix_LT.Combat.Combatant combatant = e.TargetCombatant.GetComponent<Hotfix_LT.Combat.Combatant>();
            Vector3 offset = combatant.damageTextOffset;
            
            CombatDamagesHUD.eDamageTextType text_type = CombatDamagesHUD.eDamageTextType.Attack;
            if (e.ShowDamage < 0)
            {
                text_type = CombatDamagesHUD.eDamageTextType.Heal;
                offset = combatant.healTextOffset;
            }
            else if (e.IsCrit)
            {
                text_type = CombatDamagesHUD.eDamageTextType.Crit;
            }
            ShowDamages(combatant.Data.Index.GetHashCode(), combatant.DamageTextTarget, offset, e.Shield > 0 ? e.Damage : e.ShowDamage, text_type);
        }

        public void OnCombatFloatFontListener(Hotfix_LT.Combat.CombatFloatFontEvent e)
    	{
            if (e.Target == null) return;
            ShowNewFloatFont(e.Target.Data .GetHashCode(), e.Target.DamageTextTarget, e.Target.floatBuffFontTextOffset, (CombatFloatFontUIHUD.eFloatFontType)e.Type, e.Font); 
        }

        private Dictionary<int, Queue<DamageData>> DamageQueueDic = new Dictionary<int, Queue<DamageData>>();
        private Dictionary<int, Queue<FontData>> FontQueueDic = new Dictionary<int, Queue<FontData>>();
        private Dictionary<int,int> timerDic = new Dictionary<int, int>();
        public class DamageData
        {
            public int index;
            public Transform spawn_point;
            public Vector3 offset;
            public int damage;
            public CombatDamagesHUD.eDamageTextType text_type;

            public DamageData(int mindex,Transform mspawn_point, Vector3 moffset, int mdamage, CombatDamagesHUD.eDamageTextType mtext_type)
            {
                spawn_point = mspawn_point;
                offset= moffset;
                damage = mdamage;
                text_type = mtext_type;
                index = mindex;
            }
        }
        public class FontData
        {
            public int index;
            public Transform spawn_point;
            public Vector3 offset;
            public Hotfix_LT.UI.CombatFloatFontUIHUD.eFloatFontType floatFontType;
            public string font;
            
            public FontData(int index, Transform spawn_point, Vector3 offset, Hotfix_LT.UI.CombatFloatFontUIHUD.eFloatFontType floatFontType, string font)
            {
                this.index = index;
                this.spawn_point = spawn_point;
                this.offset = offset;
                this.floatFontType = floatFontType;
                this.font = font;
            }
        }

        private void ClearTimer()
        {
            var enumerator = timerDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ILRTimerManager.instance.RemoveTimer(enumerator.Current.Key);
            }
            timerDic.Clear();
            DamageQueueDic.Clear();
        }

        private void ShowDamages(int index, Transform spawn_point, Vector3 offset, int damage, CombatDamagesHUD.eDamageTextType text_type)
        {
            DamageData data = new DamageData(index,spawn_point, offset, damage, text_type);
            bool isNew = false;
            if (!DamageQueueDic.ContainsKey(index))
            {
                DamageQueueDic.Add(index, new Queue<DamageData>());
                isNew = true;
            }
            DamageQueueDic[index].Enqueue(data);
            
            if (isNew)
            {
                int timer = ILRTimerManager.instance.AddTimer(150,0, ShowDamagesTimer);
                timerDic.Add(timer, index);
            }
        }
        private void ShowDamagesTimer(int time)
        {
            if (timerDic.ContainsKey(time))
            {
                int index = timerDic[time];
                DamageData damage = DamageQueueDic[index].Dequeue();
                ShowDamagesFunc(damage.spawn_point, damage.offset, damage.damage, damage.text_type);
                if (DamageQueueDic[index].Count == 0)
                {
                    DamageQueueDic.Remove(index);
                    timerDic.Remove(time);
                    ILRTimerManager.instance.RemoveTimer(time);
                }
            }
        }
        private void ShowDamagesFunc(Transform spawn_point, Vector3 offset, int damage, CombatDamagesHUD.eDamageTextType text_type)
        {
            if (_instance == null|| spawn_point==null) return;

            CombatDamagesHUD hud = GetHUDFromPool();
            hud.ShowDamage(damage, spawn_point, offset, text_type, delegate ()
            {
                PutHUDInPool(hud);
            });
        }

        private void ShowNewFloatFont(int index,Transform spawn_point, Vector3 offset, Hotfix_LT.UI.CombatFloatFontUIHUD.eFloatFontType floatFontType,string font)
    	{
            FontData data = new FontData(index, spawn_point, offset, floatFontType, font);
            bool isNew = false;
            if (!FontQueueDic.ContainsKey(index))
            {
                FontQueueDic.Add(index, new Queue<FontData>());
                isNew = true;
            }
            FontQueueDic[index].Enqueue(data);

            if (isNew)
            {
                int timer = ILRTimerManager.instance.AddTimer(150, 0, ShowNewFloatFontTimer);
                timerDic.Add(timer, index);
            }
        }
        private void ShowNewFloatFontTimer(int time)
        {
            if (timerDic.ContainsKey(time))
            {
                int index = timerDic[time];
                FontData damage = FontQueueDic[index].Dequeue();
                ShowNewFloatFontFunc(damage.spawn_point, damage.offset, damage.floatFontType, damage.font);
                if (FontQueueDic[index].Count == 0)
                {
                    FontQueueDic.Remove(index);
                    timerDic.Remove(time);
                    ILRTimerManager.instance.RemoveTimer(time);
                }
            }
        }
        private void ShowNewFloatFontFunc(Transform spawn_point, Vector3 offset, Hotfix_LT.UI.CombatFloatFontUIHUD.eFloatFontType floatFontType, string font)
        {
            if (_instance == null || spawn_point == null) return;

            CombatFloatFontUIHUD hud = GetNewHUDFromPool();
            hud.ShowBuffEffect(floatFontType, font, spawn_point, offset, delegate ()
            {
                PutNewHUDInPool(hud);
            });
        }
        
        #region Huds Pool
        private const int MAX_NUM_COMBATANTS = 8;
        private const int MAX_NUM_TEAMS = 2;
    
        private EB.Collections.Stack<CombatDamagesHUD> hudsPool = new EB.Collections.Stack<CombatDamagesHUD>();
    	private EB.Collections.Stack<CombatFloatFontUIHUD> newHudsPool = new EB.Collections.Stack<CombatFloatFontUIHUD>();
    
        private void InitHUDSPool()
        {
            for (int i = 0; i < MAX_NUM_COMBATANTS; i++)
                PutHUDInPool(CreateHUD());
    
    		for (int i = 0; i < MAX_NUM_COMBATANTS; ++i)
    			PutNewHUDInPool(CreateNewHUD());
            
        }
    
        private CombatDamagesHUD GetHUDFromPool()
        {
            CombatDamagesHUD hud = null;
    
            if (hudsPool.Count > 0)
            {
                hud = hudsPool.Pop();
            }
            else
            {
                hud = CreateHUD();
            }
    
            return hud;
        }
    
    	private CombatFloatFontUIHUD GetNewHUDFromPool()
    	{
    		CombatFloatFontUIHUD hud = null;
    
    		if (newHudsPool.Count > 0)
    		{
    			hud = newHudsPool.Pop();
    		}
    		else
    		{
    			hud = CreateNewHUD();
    		}
    
    		return hud;
    	}
    
        private void PutHUDInPool(CombatDamagesHUD hud)
        {
            hud.Clean();
            hud.mDMono.transform.localPosition = new Vector3(2000, 2000, 0);
            hudsPool.Push(hud);
        }
    
    	private void PutNewHUDInPool(CombatFloatFontUIHUD hud)
    	{
            if (hud != null && hud.mDMono != null && newHudsPool != null)
            {
                hud.Clean();
                hud.mDMono.transform.localPosition = new Vector3(2000, 2000, 0);
                newHudsPool.Push(hud);
            }
    	}
    
        private CombatDamagesHUD CreateHUD()
        {
            return InstantiateObj<CombatDamagesHUD>(HUDTemplate);
        }

        private CombatFloatFontUIHUD CreateNewHUD()
    	{
            return InstantiateObj<CombatFloatFontUIHUD>(NewFloatFontHUDTemplate);
        }
        
        private T InstantiateObj<T>(T template) where T: DynamicMonoHotfix
        {
            GameObject obj = GameObject.Instantiate(template.mDMono.gameObject);
            obj.transform.SetParent(cachedTransform);
            obj.transform.localScale = Vector3.one;

            return obj.GetMonoILRComponent<T>();
        }
        #endregion
    }
}
