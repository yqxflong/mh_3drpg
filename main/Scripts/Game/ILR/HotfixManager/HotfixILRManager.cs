using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using System;
using System.Reflection;

public class HotfixILRManager : MonoBehaviour
{
    private static HotfixILRManager _instance;
    public static HotfixILRManager GetInstance()
    {
        if (_instance == null)
        {
            EB.Debug.Log("Creat HotfixILRManager Instance.");
            GameObject go = new GameObject("HotfixILRManager");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<HotfixILRManager>();
        }
        return _instance;
    }

    public static System.Action onHotFixLoaded;

    protected void Awake()
    {
        IsInit = false;
        _instance = this;
        //EB.Coroutines.Run(StartProcess());
    }

    public AppDomain appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();

#if ILRuntime
    private MemoryStream dllStream;
    private MemoryStream pdbStream;
#else
    public Assembly assembly;
#endif

    public bool IsInit = false;
    private Coroutine LoadHotFix_Device(){
#if USE_IL2CPP //热更代码合入主工程了
        this.assembly = Assembly.GetExecutingAssembly();
        onHotFixLoaded?.Invoke();
        onHotFixLoaded = null;
        EB.Debug.Log("LoadHotFix_Device Finish!");
        IsInit = true;
        return null;
#else
        return EB.Assets.LoadAsyncAndInit<TextAsset>("Hotfix_LT", (assetName, ta, succ)=>
        {
            if (ta != null)
            {
                EB.Debug.Log("LoadHotFix_Device Success!");
                if (appdomain == null)
                {
                    appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
                }

                System.IO.MemoryStream fs = new MemoryStream(ta.bytes);
                appdomain.LoadAssembly(fs, null, null);
#if ILRuntime
                InitializeILRuntime(appdomain);
#else
                this.assembly = Assembly.Load(ta.bytes, null);
#endif
                onHotFixLoaded?.Invoke();
                onHotFixLoaded = null;
                EB.Debug.Log("LoadHotFix_Device Finish!");
                IsInit = true;
            }
        }, GameEngine.Instance.gameObject, null, false);
#endif
    }

    private IEnumerator LoadHotFix_Editor(){
        string uri = "file:///" + Application.dataPath + "/Hotfix/HotfixDLLPdb/Hotfix_LT.dll.bytes";
        UnityWebRequest request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)
            EB.Debug.LogError(request.error);
        byte[] dll = request.downloadHandler.data;
        request.Dispose();

        uri = "file:///" + Application.dataPath + "/Hotfix/HotfixDLLPdb/Hotfix_LT.pdb.bytes";
        request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();
		if (request.isHttpError || request.isNetworkError)
            EB.Debug.LogError(request.error);
#if ILRuntime
		byte[] pdb = request.downloadHandler.data;
        EB.Debug.Log($"当前使用的是ILRuntime模式");
        if (appdomain == null)appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();

        this.dllStream = new MemoryStream(dll);
        this.pdbStream = new MemoryStream(pdb);

        this.appdomain.LoadAssembly(this.dllStream, this.pdbStream, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());

        InitializeILRuntime(this.appdomain);
        if (onHotFixLoaded != null)
        {
            onHotFixLoaded();
            onHotFixLoaded = null;
        }
        IsInit = true;
        appdomain.DebugService.StartDebugService(56000);
#else
       EB.Debug.Log($"当前使用的是Mono模式");

        this.assembly = Assembly.Load(dll, null);

        if (onHotFixLoaded != null)
        {
            onHotFixLoaded();
            onHotFixLoaded = null;
        }
        IsInit = true;
#endif
    }

    public IEnumerator StartProcess()
    {
#if UNITY_EDITOR   
        yield return LoadHotFix_Editor();
#else
        yield return LoadHotFix_Device();
#endif
    }


    public static void RegisterCrossBindingAdaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
    {
        appdomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        appdomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());

        // appdomain.RegisterCrossBindingAdaptor(new UIControllerILRObjectAdapter());
        // appdomain.RegisterCrossBindingAdaptor(new LogicILRObjectAdaptor());
        appdomain.RegisterCrossBindingAdaptor(new DynamicMonoILRObjectAdaptor());
        // appdomain.RegisterCrossBindingAdaptor(new DataLookupILRObjectAdapter());
        appdomain.RegisterCrossBindingAdaptor(new SparxAPIAdapter());
        // appdomain.RegisterCrossBindingAdaptor(new GameEventAdapter());
        // appdomain.RegisterCrossBindingAdaptor(new IComparableAdapter());
        // appdomain.RegisterCrossBindingAdaptor(new IComparerAdapter());
        // appdomain.RegisterCrossBindingAdaptor(new IEqualityComparerAdapter());
        // appdomain.RegisterCrossBindingAdaptor(new IEqualityComparerIComparableAdapter());
        // appdomain.RegisterCrossBindingAdaptor(new IEqualityComparerInt32Adapter());
        // appdomain.RegisterCrossBindingAdaptor(new TableAdapter());
        // appdomain.RegisterCrossBindingAdaptor(new EventNameAdapter());
        appdomain.RegisterCrossBindingAdaptor(new ILRuntime.Runtime.GeneratedAdapter.IEnumerable_1_ILTypeInstanceAdapter());
        appdomain.RegisterCrossBindingAdaptor(new ILRuntime.Runtime.GeneratedAdapter.IEnumerator_1_ILTypeInstanceAdapter());

        ILRuntime.Runtime.GeneratedAdapter.CrossBindings.Initialize(appdomain);
    }
    public static void InitializeILRuntime(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
    {
        //这里做一些ILRuntime的注册
        //RegisterCrossBindingAdaptor
        HotfixILRManager.RegisterCrossBindingAdaptor(appdomain);
        
        
        // value type register

        appdomain.RegisterValueTypeBinder(typeof(UnityEngine.Vector2), new Vector2Binder());
        appdomain.RegisterValueTypeBinder(typeof(UnityEngine.Vector3), new Vector3Binder());
        appdomain.RegisterValueTypeBinder(typeof(UnityEngine.Quaternion), new QuaternionBinder());
        
        
        //DelegateManager.RegisterMethodDelegate
        appdomain.DelegateManager.RegisterMethodDelegate<Hashtable>();
        appdomain.DelegateManager.RegisterMethodDelegate<int[]>();
        appdomain.DelegateManager.RegisterMethodDelegate<bool>();
        appdomain.DelegateManager.RegisterMethodDelegate<string,string>();
        appdomain.DelegateManager.RegisterMethodDelegate<string,int>();
        appdomain.DelegateManager.RegisterMethodDelegate<string,int,int>();
        appdomain.DelegateManager.RegisterMethodDelegate<GameObject>();
        appdomain.DelegateManager.RegisterMethodDelegate<Transform>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Int32>();
        appdomain.DelegateManager.RegisterMethodDelegate<GameEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.KeyValuePair<UIEventTrigger, System.Int32>>();
        appdomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Object, System.Object>();
        appdomain.DelegateManager.RegisterMethodDelegate<EB.Sparx.ChatMessage[]>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject, UnityEngine.Vector2>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.List<EB.Sparx.ChatMessage>>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject, System.Boolean>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Int64>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.String>();
        appdomain.DelegateManager.RegisterMethodDelegate<UIPanel>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Texture>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Object>();
        appdomain.DelegateManager.RegisterMethodDelegate<EB.Sparx.Response>();
        appdomain.DelegateManager.RegisterMethodDelegate<DynamicMonoILRObjectAdaptor.Adaptor>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.String, System.String, System.String, System.String>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.String, System.Object>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Object>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.String, System.Int32>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Boolean>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Int64, System.Action<System.Int64>>();
        appdomain.DelegateManager.RegisterMethodDelegate<EB.Sparx.LevelRewardsStatus>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean, System.Boolean>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.String, System.Object, System.Boolean>();
        appdomain.DelegateManager.RegisterMethodDelegate<string, object, bool>();
        appdomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, UnityEngine.Transform>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Action>();
        appdomain.DelegateManager.RegisterMethodDelegate<EB.IAP.Item, EB.IAP.Transaction>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Action<EB.Sparx.Response>>();
        appdomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.GeneratedAdapter.GameEventAdapter.Adapter>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Int64, System.Int32>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Int64, System.Int64>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.String, System.Collections.Hashtable>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Int32>();
        appdomain.DelegateManager.RegisterMethodDelegate<SceneRootEntry>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean, System.String[]>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject, UnityEngine.GameObject>();
        appdomain.DelegateManager.RegisterMethodDelegate<UISprite>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.String, UnityEngine.GameObject, System.Boolean>();
        appdomain.DelegateManager.RegisterMethodDelegate<UIController>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Object, ILRuntime.Runtime.Intepreter.ILTypeInstance>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Single>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Object>();
        appdomain.DelegateManager.RegisterMethodDelegate<TouchStartEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<TouchEndEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<EnemyController, System.Int32>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector3>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Int32, System.Boolean>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Nullable<System.Int32>>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Nullable<UnityEngine.Vector3>>();
        appdomain.DelegateManager.RegisterMethodDelegate<EB.Sparx.Game, EB.Sparx.Player>();
        appdomain.DelegateManager.RegisterMethodDelegate<global::TouchUpdateEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<global::TapEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<global::DoubleTapEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<global::TwoFingerTouchStartEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<global::TwoFingerTouchUpdateEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<global::TwoFingerTouchEndEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector3, System.Boolean>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.String, System.String, System.Object>();
        appdomain.DelegateManager.RegisterMethodDelegate<global::eGameState>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.String, System.ArraySegment<System.Byte>>();
        appdomain.DelegateManager.RegisterMethodDelegate<FlatBuffers.ByteBuffer>();
        appdomain.DelegateManager.RegisterMethodDelegate<EB.Sparx.Authenticator[]>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean, System.String>();
        appdomain.DelegateManager.RegisterMethodDelegate<EB.Sparx.Authenticator>();
        appdomain.DelegateManager.RegisterMethodDelegate<EB.Sparx.Authenticator, ILRuntime.Runtime.Intepreter.ILTypeInstance>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.String, System.Boolean>();
        appdomain.DelegateManager.RegisterMethodDelegate<global::LevelStartEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject, UnityEngine.KeyCode>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.ParticleSystem>();
        appdomain.DelegateManager.RegisterMethodDelegate<Hotfix_LT.Combat.Combatant>();
        appdomain.DelegateManager.RegisterMethodDelegate<Hotfix_LT.Combat.CombatHitDamageEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<Hotfix_LT.Combat.CombatDamageEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<Hotfix_LT.Combat.CombatHealEvent>();
        appdomain.DelegateManager.RegisterMethodDelegate<global::eUIDialogueButtons, global::UIDialogeOption>();
        appdomain.DelegateManager.RegisterMethodDelegate<string, UnityEngine.U2D.SpriteAtlas, bool>();
        appdomain.DelegateManager.RegisterMethodDelegate<string, UnityEngine.Texture2D, bool>();
        #region For JohnyAction
        appdomain.DelegateManager.RegisterMethodDelegate<Johny.Action.ActionAlphaChange.FinishStatus>();
        appdomain.DelegateManager.RegisterMethodDelegate<Johny.Action.ActionCellBornMove.FinishStatus>();
        appdomain.DelegateManager.RegisterMethodDelegate<Johny.Action.ActionCellStampDown.FinishStatus>();
        appdomain.DelegateManager.RegisterMethodDelegate<Johny.Action.ActionCellUpAndDownLoop.FinishStatus>();
        appdomain.DelegateManager.RegisterMethodDelegate<Johny.Action.ActionModelRotation.FinishStatus>();
        appdomain.DelegateManager.RegisterMethodDelegate<Johny.Action.ActionGeneralParticle.FinishStatus>();
        #endregion

        //DelegateManager.RegisterFunctionDelegate
        appdomain.DelegateManager.RegisterFunctionDelegate<EB.Sparx.Response, bool>();
        appdomain.DelegateManager.RegisterFunctionDelegate<UITabController.TabLibEntry, bool>();
        appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<EB.IAP.Item, EB.IAP.Item, System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<EB.Sparx.ChatMessage, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.DictionaryEntry, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.DictionaryEntry, System.Collections.DictionaryEntry, System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<EB.Sparx.Response, EB.Sparx.eResponseCode, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Object, System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<DynamicMonoILRObjectAdaptor.Adaptor, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<EB.Sparx.User, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<ParticleSystemUIComponent, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Int32, System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Int32, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Int32, System.String>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.String>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Object>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.String, System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<EB.Sparx.ChatMessage, EB.Sparx.ChatMessage, System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.DictionaryEntry, System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Text.RegularExpressions.Match, System.String>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.KeyValuePair<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>, System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.KeyValuePair<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>, ILRuntime.Runtime.Intepreter.ILTypeInstance>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.KeyValuePair<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>, System.Collections.Generic.KeyValuePair<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>, System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();
        appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.String>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.Single>();
        // appdomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.List<global::ObjectManager.ManagedInstance>, System.Collections.IDictionary, global::ObjectManager.ManagedInstance>();
        appdomain.DelegateManager.RegisterFunctionDelegate<System.Object, System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>();
        appdomain.DelegateManager.RegisterFunctionDelegate<EB.Sparx.MHAuthenticator.UserInfo, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<Hotfix_LT.Combat.CombatCharacterSyncData, System.Boolean>();
        appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Transform, UnityEngine.Transform, System.Int32>();

        appdomain.DelegateManager.RegisterDelegateConvertor<EventDelegate.Callback>((action) =>
        {
            return new EventDelegate.Callback(() =>
            {
                ((System.Action)action)();
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<UICenterOnChild.OnCenterCallback>((act) => {
            return new UICenterOnChild.OnCenterCallback((centeredObject) => {
                ((System.Action<GameObject>)act)(centeredObject);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<UITabController.TabLibEntry>>((act) =>
        {
            return new System.Predicate<UITabController.TabLibEntry>((obj) =>
            {
                return ((Func<UITabController.TabLibEntry, bool>)act)(obj);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
        {
            return new System.Predicate<ILRuntime.Runtime.Intepreter.ILTypeInstance>((obj) =>
            {
                return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean>)act)(obj);
            });
        });   
        appdomain.DelegateManager.RegisterDelegateConvertor<UIEventListener.VoidDelegate>((act) =>
        {
            return new UIEventListener.VoidDelegate((go) =>
            {
                ((Action<UnityEngine.GameObject>)act)(go);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<EB.IAP.Item>>((act) =>
        {
            return new System.Comparison<EB.IAP.Item>((x, y) =>
            {
                return ((Func<EB.IAP.Item, EB.IAP.Item, System.Int32>)act)(x, y);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<UIEventListener.VectorDelegate>((act) =>
        {
            return new UIEventListener.VectorDelegate((go, delta) =>
            {
                ((Action<UnityEngine.GameObject, UnityEngine.Vector2>)act)(go, delta);
            });
        });
        
        appdomain.DelegateManager.RegisterDelegateConvertor<UIEventListener.BoolDelegate>((act) =>
        {
            return new UIEventListener.BoolDelegate((go, state) =>
            {
                ((Action<UnityEngine.GameObject, System.Boolean>)act)(go, state);
            });
        });

        appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<EB.Sparx.ChatMessage>>((act) =>
        {
            return new System.Predicate<EB.Sparx.ChatMessage>((obj) =>
            {
                return ((Func<EB.Sparx.ChatMessage, System.Boolean>)act)(obj);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<UITable.OnReposition>((act) =>
        {
            return new UITable.OnReposition(() =>
            {
                ((Action)act)();
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Collections.DictionaryEntry>>((act) =>
        {
            return new System.Comparison<System.Collections.DictionaryEntry>((x, y) =>
            {
                return ((Func<System.Collections.DictionaryEntry, System.Collections.DictionaryEntry, System.Int32>)act)(x, y);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
        {
            return new System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>((x, y) =>
            {
                return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>)act)(x, y);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<UIPanel.OnClippingMoved>((act) =>
        {
            return new UIPanel.OnClippingMoved((panel) =>
            {
                ((Action<UIPanel>)act)(panel);
            });
        });
        
        appdomain.DelegateManager.RegisterDelegateConvertor<global::GaussianBlurRT.Callback>((act) =>
        {
            return new global::GaussianBlurRT.Callback((tex) =>
            {
                ((Action<UnityEngine.Texture>)act)(tex);
            });
        });
        
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.String>>((act) =>
        {
            return new System.Predicate<System.String>((obj) =>
            {
                return ((Func<System.String, System.Boolean>)act)(obj);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<DynamicMonoILRObjectAdaptor.Adaptor>>((act) =>
        {
            return new System.Predicate<DynamicMonoILRObjectAdaptor.Adaptor>((obj) =>
            {
                return ((Func<DynamicMonoILRObjectAdaptor.Adaptor, System.Boolean>)act)(obj);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<EB.Sparx.User>>((act) =>
        {
            return new System.Predicate<EB.Sparx.User>((obj) =>
            {
                return ((Func<EB.Sparx.User, System.Boolean>)act)(obj);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<EB.Sparx.LevelRewardsManager.LevelRewardsChangeDel>((act) =>
        {
            return new EB.Sparx.LevelRewardsManager.LevelRewardsChangeDel((status) =>
            {
                ((Action<EB.Sparx.LevelRewardsStatus>)act)(status);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<ParticleSystemUIComponent>>((act) =>
        {
            return new System.Predicate<ParticleSystemUIComponent>((obj) =>
            {
                return ((Func<ParticleSystemUIComponent, System.Boolean>)act)(obj);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<SceneRootEntry.Begin>((act) =>
        {
            return new SceneRootEntry.Begin(() =>
            {
                ((Action)act)();
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<SceneRootEntry.Failed>((act) =>
        {
            return new SceneRootEntry.Failed(() =>
            {
                ((Action)act)();
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<SceneRootEntry.Loading>((act) =>
        {
            return new SceneRootEntry.Loading((int loaded, int total) =>
            {
                ((Action<int, int>)act)(loaded, total);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<SceneRootEntry.Finished>((act) =>
        {
            return new SceneRootEntry.Finished((SceneRootEntry entry) =>
            {
                ((Action<SceneRootEntry>)act)(entry);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<HudLoadManager.HudLoadComplete>((act) =>
        {
            return new HudLoadManager.HudLoadComplete((NoError, Show) =>
            {
                ((Action<System.Boolean, System.String[]>)act)(NoError, Show);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.String>>((act) =>
        {
            return new System.Comparison<System.String>((x, y) =>
            {
                return ((Func<System.String, System.String, System.Int32>)act)(x, y);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<EB.Sparx.ChatMessage>>((act) =>
        {
            return new System.Comparison<EB.Sparx.ChatMessage>((x, y) =>
            {
                return ((Func<EB.Sparx.ChatMessage, EB.Sparx.ChatMessage, System.Int32>)act)(x, y);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Text.RegularExpressions.MatchEvaluator>((act) =>
        {
            return new System.Text.RegularExpressions.MatchEvaluator((match) =>
            {
                return ((Func<System.Text.RegularExpressions.Match, System.String>)act)(match);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Int32>>((act) =>
        {
            return new System.Comparison<System.Int32>((x, y) =>
            {
                return ((Func<System.Int32, System.Int32, System.Int32>)act)(x, y);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
        {
            return new System.EventHandler<ILRuntime.Runtime.Intepreter.ILTypeInstance>((sender, e) =>
            {
                ((Action<System.Object, ILRuntime.Runtime.Intepreter.ILTypeInstance>)act)(sender, e);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<EventManager.EventDelegate<ILRuntime.Runtime.GeneratedAdapter.GameEventAdapter.Adapter>>((act) =>
        {
            return new EventManager.EventDelegate<ILRuntime.Runtime.GeneratedAdapter.GameEventAdapter.Adapter>((e) =>
            {
                ((Action<ILRuntime.Runtime.GeneratedAdapter.GameEventAdapter.Adapter>)act)(e);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<EventManager.EventDelegate<TouchStartEvent>>((act) =>
        {
            return new EventManager.EventDelegate<TouchStartEvent>((e) =>
            {
                ((Action<TouchStartEvent>)act)(e);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<EventManager.EventDelegate<TouchEndEvent>>((act) =>
        {
            return new EventManager.EventDelegate<TouchEndEvent>((e) =>
            {
                ((Action<TouchEndEvent>)act)(e);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Collections.Generic.KeyValuePair<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>>>((act) =>
        {
            return new System.Comparison<System.Collections.Generic.KeyValuePair<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>>((x, y) =>
            {
                return ((Func<System.Collections.Generic.KeyValuePair<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>, System.Collections.Generic.KeyValuePair<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>, System.Int32>)act)(x, y);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<global::CharacterTargetingComponent.MovementTargetChangeRequestEventHandler>((act) =>
        {
            return new global::CharacterTargetingComponent.MovementTargetChangeRequestEventHandler((requestedTarget,isNull) =>
            {
                ((Action<UnityEngine.Vector3,bool>)act)(requestedTarget, isNull);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<global::CharacterTargetingComponent.AttackTargetChangedEventHandler>((act) =>
        {
            return new global::CharacterTargetingComponent.AttackTargetChangedEventHandler((newAttackTarget) =>
            {
                ((Action<UnityEngine.GameObject>)act)(newAttackTarget);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<global::GameListenerFusion.PlayerLeftHandler>((act) =>
        {
            return new global::GameListenerFusion.PlayerLeftHandler((game, player) =>
            {
                ((Action<EB.Sparx.Game, EB.Sparx.Player>)act)(game, player);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<global::EventManager.EventDelegate<global::TouchUpdateEvent>>((act) =>
        {
            return new global::EventManager.EventDelegate<global::TouchUpdateEvent>((e) =>
            {
                ((Action<global::TouchUpdateEvent>)act)(e);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<global::EventManager.EventDelegate<global::TapEvent>>((act) =>
        {
            return new global::EventManager.EventDelegate<global::TapEvent>((e) =>
            {
                ((Action<global::TapEvent>)act)(e);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<global::EventManager.EventDelegate<global::DoubleTapEvent>>((act) =>
        {
            return new global::EventManager.EventDelegate<global::DoubleTapEvent>((e) =>
            {
                ((Action<global::DoubleTapEvent>)act)(e);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<global::EventManager.EventDelegate<global::TwoFingerTouchStartEvent>>((act) =>
        {
            return new global::EventManager.EventDelegate<global::TwoFingerTouchStartEvent>((e) =>
            {
                ((Action<global::TwoFingerTouchStartEvent>)act)(e);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<global::EventManager.EventDelegate<global::TwoFingerTouchEndEvent>>((act) =>
        {
            return new global::EventManager.EventDelegate<global::TwoFingerTouchEndEvent>((e) =>
            {
                ((Action<global::TwoFingerTouchEndEvent>)act)(e);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<global::EventManager.EventDelegate<global::TwoFingerTouchUpdateEvent>>((act) =>
        {
            return new global::EventManager.EventDelegate<global::TwoFingerTouchUpdateEvent>((e) =>
            {
                ((Action<global::TwoFingerTouchUpdateEvent>)act)(e);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<EB.Sparx.DataCacheManager.OnFlatBuffersDataCacheUpdated>((act) =>
        {
            return new EB.Sparx.DataCacheManager.OnFlatBuffersDataCacheUpdated((name, buffer) =>
            {
                ((Action<System.String, System.ArraySegment<System.Byte>>)act)(name, buffer);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<global::EventManager.EventDelegate<global::LevelStartEvent>>((act) =>
        {
            return new global::EventManager.EventDelegate<global::LevelStartEvent>((e) =>
            {
                ((Action<global::LevelStartEvent>)act)(e);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<global::UIEventListener.KeyCodeDelegate>((act) =>
        {
            return new global::UIEventListener.KeyCodeDelegate((go, key) =>
            {
                ((Action<UnityEngine.GameObject, UnityEngine.KeyCode>)act)(go, key);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<EB.Sparx.MHAuthenticator.UserInfo>>((act) =>
        {
            return new System.Predicate<EB.Sparx.MHAuthenticator.UserInfo>((obj) =>
            {
                return ((Func<EB.Sparx.MHAuthenticator.UserInfo, System.Boolean>)act)(obj);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<global::CTimer.OnTimeUpHandler>((act) =>
        {
            return new global::CTimer.OnTimeUpHandler((timerSequence) =>
            {
                ((Action<System.Int32>)act)(timerSequence);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<Hotfix_LT.UI.LTCombatEventReceiver.CombatantCallbackVoid>((act) =>
        {
            return new Hotfix_LT.UI.LTCombatEventReceiver.CombatantCallbackVoid((combatant) =>
            {
                ((Action<Hotfix_LT.Combat.Combatant>)act)(combatant);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<Hotfix_LT.Combat.CombatCharacterSyncData>>((act) =>
        {
            return new System.Predicate<Hotfix_LT.Combat.CombatCharacterSyncData>((obj) =>
            {
                return ((Func<Hotfix_LT.Combat.CombatCharacterSyncData, System.Boolean>)act)(obj);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<global::OnUIDialogueButtonClick>((act) =>
        {
            return new global::OnUIDialogueButtonClick((button, option) =>
            {
                ((Action<global::eUIDialogueButtons, global::UIDialogeOption>)act)(button, option);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback>((act) => 
        {
            return new DG.Tweening.TweenCallback(() => 
            {
                ((Action)act)();
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<UnityEngine.Transform>>((act) =>
        {
            return new System.Comparison<UnityEngine.Transform>((x, y) =>
            {
                return ((Func<UnityEngine.Transform, UnityEngine.Transform, System.Int32>)act)(x, y);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<System.Single>>((act) => 
        {
            return new DG.Tweening.Core.DOSetter<System.Single>((pNewValue) => 
            {
                ((Action<System.Single>)act)(pNewValue);
            });
        });

        //这个必须要！！！
        ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);
    }

    public void OnDestory()
    {
    }
}
