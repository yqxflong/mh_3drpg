
using System;
using System.Collections;
using System.Collections.Generic;

namespace EB.Sparx
{
    // no need Updatable, Subsystem
    public class DataCacheManager : Manager
    {
        public delegate void OnCacheUpdatedDelegate(string name, IVersionedDataCache cache);

        public class DataCacheEntity
        {
            public string Name { get; set; }
            public System.Type Type { get; set; }
            public string Version { get; set; }
            public OnCacheUpdatedDelegate OnCacheUpdated { get; set; }
            public IVersionedDataCache Cache { get; set; }
        }

        public System.Action<FlatBuffers.ByteBuffer> OnBufferHandler;
        protected DataCacheAPI _api;

        private Dictionary<string, DataCacheEntity> m_cacheEntities = new Dictionary<string, DataCacheEntity>();
        private string m_cacheDirectory = string.Empty;

        private bool mIsCouldCallBack = false;

        public override void Initialize(Config config)
        {
            m_cacheDirectory = config.DataCacheDirectory;

            _api = new DataCacheAPI(Hub.ApiEndPoint);
        }

        private IEnumerator DoProcessCaches(Hashtable configs, System.Action<string> callback, System.Action<int, string> addPrgAct)
        {
            ArrayList fetchBuffer = Johny.ArrayListPool.Claim();
            var it = m_cacheEntities.GetEnumerator();
            while(it.MoveNext())
            {
                if (configs.ContainsKey(it.Current.Key))
                {
                    Hashtable entry = configs[it.Current.Key] as Hashtable;
                    string version = EB.Dot.String("version", entry, string.Empty);
                    object payload = EB.Dot.Find<object>("payload", entry);

                    if (payload != null)
                    {
                        CacheJsonData(it.Current.Key, version, payload);
                    }
                    else
                    {
                        it.Current.Value.Cache = null;
                        fetchBuffer.Add(it.Current.Key);
                    }
                }
                else if (it.Current.Value.Cache == null)
                {
                    LoadJsonData(it.Current.Key);
                }

                if (it.Current.Value.OnCacheUpdated != null && it.Current.Value.Cache != null)
                {
                    it.Current.Value.OnCacheUpdated(it.Current.Key, it.Current.Value.Cache);
                }

                addPrgAct?.Invoke(1, "DataCacheManager.DoProcessCaches.while(it.MoveNext())");
                yield return null;
            }

            if(fetchBuffer.Count > 0){
                mIsCouldCallBack = true;
            }
            else{
                mIsCouldCallBack = false;
                callback(null);
            }

            #region check更新config
            EB.Debug.Log("DataCacheManager.DoProcessCaches!!");
            
            if (fetchBuffer.Count == 0)
            {
                UnloadAll();
            }
            else
            {
                try
                {
                    EB.Debug.Log("DataCacheManager.GetConfig!!");
                    GetConfig(fetchBuffer, callback);
                }
                catch (Exception err)
                {
                    EB.Debug.LogError("Name:{0}\nFunc:{1}\nErr:{2}\nTip:{3}", err.Source, err.TargetSite, err.StackTrace, err.Message);
                    Hub.Instance.FatalError("ID_SPARX_ERROR_UNKNOWN");
                }
            }

            #endregion

            addPrgAct?.Invoke(5, "DataCacheManager.DoProcessCaches.End");
        }

        public void ProcessCaches(Hashtable configs, System.Action<string> callback, System.Action<int, string> addPrgAct)
        {
            if (configs == null)
            {
                EB.Debug.LogWarning("DataCacheManager.ProcessCaches: their is no configs");
                callback(null);
                return;
            }
            EB.Coroutines.Run(DoProcessCaches(configs, callback, addPrgAct));
        }

        public void ProcessCache(string name, string version, System.ArraySegment<byte> buffer)
        {
            CacheFlatBuffersData(name, version, buffer);

            var entity = m_cacheEntities[name];
            if (entity.OnCacheUpdated != null && entity.Cache != null)
            {
                entity.OnCacheUpdated(name, entity.Cache);
            }
        }

        void GetConfig(ArrayList configs, System.Action<string> callback)
        {
            Hashtable reqData = Johny.HashtablePool.Claim();
            reqData["configs"] = configs;
            _api.GetConfig(reqData, delegate (string err, byte[] bytes)
            {
                if (!string.IsNullOrEmpty(err))
                {
                    if (mIsCouldCallBack)
                    {
                        callback(err);
                    }
                    else
                    {
                        mIsCouldCallBack = true;
                    }
                    return;
                }

                ProcessBuffer(bytes);

                if (mIsCouldCallBack)
                {
                    callback(null);
                }
                else
                {
                    mIsCouldCallBack = true;
                }
            });
        }

        void ProcessBuffer(byte[] buffer)
        {
            if (buffer == null)
            {
                EB.Debug.LogWarning("DataCacheManager.ProcessBuffer: buffer is empty");
                return;
            }
            if (OnBufferHandler != null)
            {
                OnBufferHandler(new FlatBuffers.ByteBuffer(buffer));
            }
            
            UnloadAll();
        }

        public override void Dispose()
        {
            foreach (var pair in m_cacheEntities)
            {
                if (pair.Value.Cache != null)
                {
                    pair.Value.Cache.Dispose();
                    pair.Value.Cache = null;
                }

                pair.Value.OnCacheUpdated = null;
            }
            m_cacheEntities.Clear();
        }

        private void RegisterEntity(string name, System.Type type, OnCacheUpdatedDelegate onupdated)
        {
            if (m_cacheEntities.ContainsKey(name))
            {
                DataCacheEntity entity = m_cacheEntities[name];
                if (entity.Type != type)
                {
                    EB.Debug.LogError("DataCacheManager.RegisterCacheEntity: exists {0} named {1} not match {2}",
                    m_cacheEntities[name].ToString(), name, type.ToString());

                    return;
                }

                EB.Debug.LogWarning("DataCacheManager.RegisterCacheEntity: {0} named {1} already exists",
                        type.ToString(), name);

                if (onupdated != null)
                {
                    entity.OnCacheUpdated += onupdated;
                }
            }
            else
            {
                DataCacheEntity entity = new DataCacheEntity();
                entity.Name = name;
                entity.Type = type;
                entity.Name = string.Empty;
                if (onupdated != null)
                {
                    entity.OnCacheUpdated += onupdated;
                }
                m_cacheEntities[name] = entity;
            }
        }

        private IVersionedDataCache LoadCache(string name, System.Type type)
        {
            EB.Debug.Log("Start LoadCache: name ={0} time = {1}", name, UnityEngine.Time.time);

            string path = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, System.IO.Path.Combine(m_cacheDirectory, name));
            if (!System.IO.File.Exists(path))
            {
                EB.Debug.Log("DataCacheManager.LoadCache: cache file does't exists {0}", path);
                return null;
            }

            IVersionedDataCache cache = null;
            using (System.IO.FileStream fs = System.IO.File.OpenRead(path))
            {
                IVersionedDataCacheSerializer serializer = DataCacheUtil.GetSerializer(type);
                UnityEngine.Debug.Assert(serializer != null, "DataCacheManager.LoadCache: GetSerializer failed");
                cache = serializer.Deserialize(fs);
                if (cache == null)
                {
                    EB.Debug.LogWarning("DataCacheManager.LoadCache: Deserialize {0} failed, will remove it", path);
                    System.IO.File.Delete(path);
                }
            }

            EB.Debug.Log("End LoadCache: name ={0} time = {1}", name, UnityEngine.Time.time);

            return cache;
        }

        private void SaveCache(string name, IVersionedDataCache cache)
        {
            //var begin = System.DateTime.Now;
            //double cost = 0.0;

            string path = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, System.IO.Path.Combine(m_cacheDirectory, name));
            string directory = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(directory))
            {
                if (System.IO.Directory.CreateDirectory(directory) == null)
                {
                    EB.Debug.LogError("DataCacheManager.SaveCache: create directory {0} failed when save {1}", directory, path);
                    return;
                }

                //EB.Debug.Log("DataCacheManager.SaveCache: create directory {0}", directory);
            }

            using (System.IO.FileStream fs = System.IO.File.Create(path))
            {
                IVersionedDataCacheSerializer serializer = DataCacheUtil.GetSerializer(cache.GetType());
                UnityEngine.Debug.Assert(serializer != null, "DataCacheManager.SaveCache: GetSerializer failed");
                serializer.Serialize(fs, cache);

                fs.Close();
                fs.Dispose();

                //cost = (System.DateTime.Now - begin).TotalSeconds;
                //EB.Debug.LogIf(cost > 0.1, "DataCacheManager.SaveCache: {0} saved, cost = {1}", path, cost);
#if UNITY_IPHONE
                UnityEngine.iOS.Device.SetNoBackupFlag(path);
#endif
            }
        }

        private void LoadCaches(string[] entities)
        {
            for (int i = 0; i < entities.Length; ++i)
            {
                string name = entities[i];
                if (!m_cacheEntities.ContainsKey(name))
                {
                    EB.Debug.LogError("DataCacheManager.LoadCaches: entity not found for {0}", name);
                    continue;
                }

                DataCacheEntity entity = m_cacheEntities[name];
                if (entity.Cache != null)
                {
                    continue;
                }

                IVersionedDataCache cache = LoadCache(name, entity.Type);
                if (cache != null)
                {
                    entity.Cache = cache;
                    entity.Version = cache.Version;
                }
            }
        }

        public void RegisterEntity<T>(string name, OnCacheUpdatedDelegate onupdated) where T : IVersionedDataCache
        {
            RegisterEntity(name, typeof(T), onupdated);
        }

        public bool EntityRegistered(string name)
        {
            return m_cacheEntities.ContainsKey(name);
        }

        public bool CompareVersions(Dictionary<string, string> cmp)
        {
            foreach (var pair in cmp)
            {
                if (!m_cacheEntities.ContainsKey(pair.Key))
                {
                    return false;
                }

                if (m_cacheEntities[pair.Key].Version != pair.Value)
                {
                    return false;
                }
            }

            return true;
        }

        public Dictionary<string, string> GetVersions()
        {
            Dictionary<string, string> versions = new Dictionary<string, string>();

            foreach (var pair in m_cacheEntities)
            {
                if (!string.IsNullOrEmpty(pair.Value.Version))
                {
                    versions[pair.Key] = pair.Value.Version;
                }
            }

            return versions;
        }

        public T Load<T>(string name) where T : IVersionedDataCache
        {
            UnityEngine.Debug.Assert(m_cacheEntities.ContainsKey(name), "DataCacheManager.Load: entity not exists");

            LoadCaches(new string[] { name });

            DataCacheEntity entity = m_cacheEntities[name];
            return (T)entity.Cache;
        }

        public void LoadAll()
        {
            var keys = m_cacheEntities.Keys;
            string[] array = new string[keys.Count];
            keys.CopyTo(array, 0);
            LoadCaches(array);
            #region 强制回收GC
            System.GC.Collect(System.GC.MaxGeneration, System.GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            #endregion
        }

        public void Unload(string name)
        {
            if (m_cacheEntities.TryGetValue(name, out var entity))
            {
                if (entity.Cache != null)
                {
                    entity.Cache.Dispose();
                    entity.Cache = null;
                }
            }
        }

        public void UnloadAll()
        {
            foreach (var pair in m_cacheEntities)
            {
                Unload(pair.Key);
            }
        }

        public void Cache(string name, IVersionedDataCache cache)
        {
            UnityEngine.Debug.Assert(cache != null, "DataCacheManager.Cache: cache is null");

            SaveCache(name, cache);

            if (m_cacheEntities.TryGetValue(name, out var entity)) 
            {				
                if (entity.Cache != null)
                {
                    entity.Cache.Dispose ();
                    entity.Cache = null;
                }
                entity.Cache = cache;
                entity.Version = cache.Version;
            }
        }

        /// <summary>
        /// simply use for json data
        /// </summary>
        /// <param name="name"></param>
        /// <param name="json"></param>
        public delegate void OnJsonDataCacheUpdated(string name, object json);

        public void RegisterJsonEntity(string name, OnJsonDataCacheUpdated onupdated)
        {
            if (EntityRegistered(name))
            {
                return;
            }

            RegisterEntity<JsonDataCache>(name, delegate(string dname, IVersionedDataCache dcache)
            {
                JsonDataCache json = dcache as JsonDataCache;
                if (onupdated != null)
                {
                    onupdated(dname, json.Payload);
                }
            });
        }

        public void CacheJsonData(string name, string version, object json)
        {
            JsonDataCache cache = new JsonDataCache(version, json);
            Cache(name, cache);
        }

        public object LoadJsonData(string name)
        {
            JsonDataCache json = Load<JsonDataCache>(name);
            return json == null ? null : json.Payload;
        }

        public delegate void OnFlatBuffersDataCacheUpdated(string name, System.ArraySegment<byte> buffer);

        public void RegisterFlatBuffersEntity(string name, OnFlatBuffersDataCacheUpdated onupdated)
        {
            if (EntityRegistered(name))
            {
                return;
            }

            RegisterEntity<FlatBuffersDataCache>(name, delegate (string dname, IVersionedDataCache dcache)
            {
                FlatBuffersDataCache buffer = dcache as FlatBuffersDataCache;
                if (onupdated != null)
                {
                    onupdated(dname, buffer.Buffer);
                }
            });
        }

        public void CacheFlatBuffersData(string name, string version, System.ArraySegment<byte> buffer)
        {
            FlatBuffersDataCache cache = new FlatBuffersDataCache();
            cache.Version = version;
            cache.Buffer = buffer;
            Cache(name, cache);
        }
    }
}