using System;
using Hotfix_LT.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.GameState
{
    public class LTGameStateStart : GameStateUnit
    {
        public override eGameState mGameState { get { return eGameState.Start; } }

        public override IEnumerator Start()
        {
            GameStateDownload.LogWithTime("LTGameStateStart.START=====>");
            InitializeSparx(GameEngine.Instance.ApiServerAddress);
            InitializeFusion();
            InitializeReplication();
            yield return new WaitUntil(() => CharacterCatalog.Instance.isModelsLoaded);//等CharacterModel相关资源加载完成
            InitializeOfflineData();
            while (EB.Assets.IsLoadingScene)
            {
                yield return null;
            }
            
            Mgr.SetGameState<GameStateLogin>();
            GameStateDownload.LogWithTime("<=====LTGameStateStart.START");
        }

        public override void End()
        {
            EB.Debug.Log("Ending LTGameState START");
        }

        private void InitializeOfflineData()
        {
            EB.Sparx.DataCacheManager manager = SparxHub.Instance.GetManager<EB.Sparx.DataCacheManager>();

            manager.RegisterJsonEntity("GuideCombat", null);

            manager.RegisterFlatBuffersEntity("Combat", OnFlatBuffersDataCacheUpdated);
            manager.RegisterFlatBuffersEntity("Economy", OnFlatBuffersDataCacheUpdated);
            manager.RegisterFlatBuffersEntity("Task", OnFlatBuffersDataCacheUpdated);
            manager.RegisterFlatBuffersEntity("Resource", OnFlatBuffersDataCacheUpdated);
            manager.RegisterFlatBuffersEntity("Scene", OnFlatBuffersDataCacheUpdated);
            manager.RegisterFlatBuffersEntity("Character", OnFlatBuffersDataCacheUpdated);
            manager.RegisterFlatBuffersEntity("Guide", OnFlatBuffersDataCacheUpdated);
            manager.RegisterFlatBuffersEntity("Event", OnFlatBuffersDataCacheUpdated);
            manager.RegisterFlatBuffersEntity("Vip", OnFlatBuffersDataCacheUpdated);
            manager.RegisterFlatBuffersEntity("Alliance", OnFlatBuffersDataCacheUpdated);
            manager.RegisterFlatBuffersEntity("Shop", OnFlatBuffersDataCacheUpdated);
            manager.RegisterFlatBuffersEntity("NewGameConfig", OnFlatBuffersDataCacheUpdated);
            manager.RegisterFlatBuffersEntity("Gacha", OnFlatBuffersDataCacheUpdated);


            manager.OnBufferHandler += OnFlatBuffersUpdated;
        }

        private void OnFlatBuffersDataCacheUpdated(string name, System.ArraySegment<byte> buffer)
        {
            ClientDataUtil.OnFlatBuffersEntityUpdated(name, buffer);
            #region 强制回收GC
            System.GC.Collect(System.GC.MaxGeneration, System.GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            #endregion
        }

        private void OnFlatBuffersUpdated(FlatBuffers.ByteBuffer buffer)
        {
            ClientDataUtil.OnFlatBuffersUpdated(buffer);
            #region 强制回收GC
            System.GC.Collect(System.GC.MaxGeneration, System.GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            #endregion
        }

        private void InitializeSparx(string apiEndPoint)
        {
            EB.Debug.Log("Api Server Address = {0}", apiEndPoint);

            // register the enet library
            EB.Sparx.NetworkFactory.Register("udp", typeof(EB.Sparx.NetworkENet));

            var config = new EB.Sparx.Config();
            config.ApiEndpoint = apiEndPoint;
            config.ApiKey = new EB.Sparx.Key("]!Q>>r21CHR<GG]||@s/6qc/^w3+kw?|Qty3}N|Kb|H+qK(<Comba/g^+1-_tQ)W");
            config.Locale = UserData.Locale;
            config.WalletConfig.IAPPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAjLtJUDbKn66g4XeYtgcf22DFsPGrslJN+XzxUJmyqIaW6f6X7QCcCEX74/aoGX7NTBvVe6qp0mkR9BtG5WoEf4chTVax1ILNsvRSlyTa9Z833TNAxKkHHKDEsC9KyqwGxKMCF0+8usRx8UwdWD0N90pjrOoA9aM2mKIiRdfG+jSxMMHdHHKC05G7ieJu7s966r8DZPtTtvpQs223XRhCdHy+9cXX5cgYYNKMiodnmIgQ3WLUvk8takMkPlQ166OToQXorhPIzK1/89OFG3MhwIb4GBlDjmRspgVREMU7b1T9pTg3kIlHSR2wuKhsFX5Basgha5yXRYLF3oX5SZLBfQIDAQAB";
            config.WalletConfig.Listener = GameEngine.Instance.WalletListener;
            config.LoginConfig.Listener = GameEngine.Instance.LoginListener;
            config.GameManagerConfig.Listener = GameEngine.Instance.GameListener;
            config.GameCenterConfig.Enabled = false;
            config.GameCenterConfig.LeaderboardPrefix = Application.identifier /*Application.bundleIdentifier*/ + ".leaderboard";
            config.GachaConfig.Groups = new string[] { "Basic_Machine", "Premium_Machine" };

            // Performance manager
            config.PerformanceConfig.DataLoadedHandler += PerformanceManager.Instance.DataLoaded;
            config.PerformanceConfig.GetPlatformHandler += PerformanceManager.Instance.GetPlatform;

            // Add custom managers for Fusion project
            config.GameComponents = new List<System.Type>();
            config.GameComponents.Add(typeof(DataLookupSparxManager));
            config.GameComponents.Add(typeof(EB.Sparx.ResourcesManager));
            config.GameComponents.Add(typeof(EB.Sparx.InventoryManager));

            config.GameComponents.Add(typeof(LTHotfixManagerLogic));

            // initialize sparx
            EB.Sparx.Hub.Create(config, GameEngine.Instance.ShowTipCall);
            
        }

        private void InitializeFusion()
        {
            string path = "_GameAssets/Res/Prefabs/Audio/AudioListener";
            EB.Assets.LoadAsync(path, typeof(GameObject), o => {
                if (o)
                {
                    GameObject.Instantiate(o);
                    GM.AssetUtils.FixShaderInEditor(o);
                }
            });


            Shader.WarmupAllShaders();
			DG.Tweening.DOTween.Init();

            //QuestManager.Instance();
            ProfileManager.Initialize();
            PSPoolManager.InitializeInstance();


            GameEngine.Instance.gameObject.AddComponent<TouchController>();
            GameEngine.Instance.gameObject.AddComponent<FusionSparxHelper>();

            EB.Sparx.SparxAPI.GlobalErrorHandler += eResponseCodeUIExtensions.GlobalErrorHandler;
            EB.Sparx.Response.SetErrorTranslatorHandler(eResponseCodeUIExtensions.ErrorTranslateHandler);

            EB.Sparx.PushManager pm = SparxHub.Instance.GetManager<EB.Sparx.PushManager>();
            EB.Sparx.SparxAPI.GlobalResultHandler += pm.GlobalApiResultSyncHandler;
            SparxHub.Instance.ApiEndPoint.PostHandler += pm.EndPointPostAsyncHandler;
            SparxHub.Instance.ApiEndPoint.SuspendHandler += eResponseCodeUIExtensions.SuspendHandler;
        }

        private void InitializeReplication()
        {
            // Replication.RegisterRPC("MyRPC", (System.Action<int>)this.MyRPC);
            Replication.RegisterRPC("OnReplicatedEventRPC", (System.Action<ReplicatedEvent>)EventManager.OnReplicatedEventRPC);

            // use a custom method for network instantiation to always go through the ObjectManager (pooling) system
            Replication.InstantiateMethod = GameUtils.InstantiateThroughObjectManager;

            Replication.RegisterResource("Bundles/Prefab/Player");
            Replication.RegisterResource("Bundles/Prefab/EnemyCharacter");
            Replication.RegisterResource("Bundles/Prefab/OtherPlayer");

            Replication.RegisterSerializable(typeof(RPCDictionary));
            Replication.RegisterSerializable(typeof(RPCList<EB.Replication.ViewId>));

            Replication.RegisterSerializable(typeof(LevelStartEvent));
            Replication.RegisterSerializable(typeof(SpawnerEnemiesAggroEvent));

            Replication.RegisterSerializable(typeof(EB.Sparx.SocketDataSet));
            Replication.RegisterSerializable(typeof(EB.Sparx.SocketData));
            Replication.RegisterSerializable(typeof(EB.Sparx.SocketData.SpawnerData));
            Replication.RegisterSerializable(typeof(EB.Sparx.SocketData.LootableData));
            Replication.RegisterSerializable(typeof(EB.Sparx.SocketData.CharacterData));
            Replication.RegisterSerializable(typeof(EB.Sparx.SocketData.DropData));

            CharacterCatalog.Instance.GetAssetPath();
            AutoAttackCatalog.Instance.GetAssetPath();

        }
    }
}