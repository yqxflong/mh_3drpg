//资源加载唯一入口
//Edit By Johny


#if USE_DEBUG
#define ENABLE_LOGGING
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace EB
{
    /// <summary>
    /// 资源管理器（静态类）
    /// </summary>
    public static class Assets
    {
        public class AssetPack
        {
            /// <summary>
            /// 包名
            /// </summary>
            public string id;
            /// <summary>
            /// 文件夹路径
            /// </summary>
            public string folder;
            /// <summary>
            /// 这个包里面所有的AssetBundleInfo
            /// </summary>
            public List<AssetBundleInfo> bundles;
            public Hashtable folders;
        }

        public class AssetBundleInfo
        {
            /// <summary>
            /// 包名（对应相应的AssetPack的id）（这里的包指的是多个bundle的集合）
            /// </summary>
            public string packId;
            /// <summary>
            /// bundle信息
            /// </summary>
            public AssetBundle bundle;
            /// <summary>
            /// 该bundle的名称
            /// </summary>
            public string id;
            public int retry;
            /// <summary>
            /// 是否解压了
            /// </summary>
            public bool uncompressed = false;

            /// <summary>
            /// 所有包含的路径
            /// </summary>
            public IEnumerable<string> paths;
            /// <summary>
            /// 父级名称
            /// </summary>
            public string parent;
            /// <summary>
            /// bundle大小
            /// </summary>
            public long size;

            /// <summary>
            /// 该bundle正在被异步加载的次数
            /// </summary>
            public int asyncLoadingCount = 0;
            /// <summary>
            /// 是否被卸载了
            /// </summary>
            public bool unload = false;
            /// <summary>
            /// 版本号
            /// </summary>
            public int hash;

            public long ts = 0;

            /// <summary>
            /// 是否加载完成
            /// </summary>
            public bool isLoaded { get { return bundle != null; } }
        };

        public class AssetSceneInfo
        {
            public string packId;
            public string id;
            public long ts = 0;
        }

        #region DevBundling
        public enum DevBundleType
        {
            Unset = -1,
            BundleServer = 0,
            NoBundles = 1,
            StandardBundles = 2
        }

        private static DevBundleType _currentBundleMode = DevBundleType.Unset;

        #endregion

        #region Member
        /// <summary>
        /// 资源缓存[资源名称，资源]
        /// </summary>
        private static Dictionary<string, AssetBundleInfo> _bundles = new Dictionary<string, AssetBundleInfo>();//和_pathToBundle存的是相同的数据
                                                                                                                /// <summary>
                                                                                                                /// 资源缓存[资源路径，资源]
                                                                                                                /// </summary>
        private static Dictionary<string, AssetBundleInfo> _pathToBundle = new Dictionary<string, AssetBundleInfo>();//和_bundles存的是相同的数据

        /// <summary>
        /// 资源包缓存[资源路径，资源包]
        /// </summary>
        private static Dictionary<string, AssetPack> _packs = new Dictionary<string, AssetPack>();
        
        private static Dictionary<string, AssetSceneInfo> _scenes = new Dictionary<string, AssetSceneInfo>();//目前是没有存入任何数据的，add的地方没有调用

        private static List<AssetBundleInfo> _unloads = new List<AssetBundleInfo>();

        private static GameObject _missingObject;
        
        private static Texture2D _missingTexture;

        private static List<string> _dlcPacks = new List<string>();

        private static AsyncOperation _loadLevelOperation = null;
        #endregion

        #region get and set
        public static string CurrentScene { get; private set; }
        public static bool IsLoadingScene { get; private set; }
        public static bool IsDownloadingManifest { get; set; }
        public static bool IsDownloadingOnLoad { get; set; }
        public static int DownloadedBundles { get; set; }//已下载的Bundle数量
        public static int ToDownloadBundles { get; set; }//需要下载的Bundle数量
        public static int TotalBundles{get;set;}//Bundle总数
        public static long DownloadSpeed { get; set; }

        /// <summary>
        /// 缺省贴图
        /// </summary>
        private static Texture MissingTexture
        {
            get
            {
                if (_missingTexture == null)
                {
                    _missingTexture = new Texture2D(2, 2);
                    _missingTexture.wrapMode = TextureWrapMode.Repeat;
                    _missingTexture.filterMode = FilterMode.Point;
                    _missingTexture.SetPixel(0, 0, Color.green);
                    _missingTexture.SetPixel(1, 1, Color.green);
                    _missingTexture.SetPixel(1, 0, Color.magenta);
                    _missingTexture.SetPixel(0, 1, Color.magenta);
                    _missingTexture.Apply();
                }
                return _missingTexture;
            }
        }

        /// <summary>
        /// 缺省GO
        /// </summary>
        private static GameObject MissingObject
        {
            get
            {
                if (_missingObject == null)
                {
                    _missingObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    _missingObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    _missingObject.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
                    _missingObject.GetComponent<Renderer>().material.mainTexture = MissingTexture;
                    EB.Debug.LogCoreAsset("<color=#ff0000>XXX清除掉图片:{0}</color>", (_missingObject == null) ? "空的图片引用" : _missingObject.name);
                    Object.Destroy(_missingObject.GetComponent<BoxCollider>());
                }
                return _missingObject;
            }
        }
        #endregion

        #region API Query
        public static string FindDiskAsset(string path)
        {
            var index = path.IndexOfAny(new char[] { '/', '\\' });
            var folder = path;
            if (index >= 0)
            {
                folder = path.Substring(0, index);
            }

            var iter = _packs.GetEnumerator();
            while (iter.MoveNext())
            {
                var pack = iter.Current;
                var paths = Dot.Array(folder, pack.Value.folders, null);
                if (paths != null)
                {
                    for (int i = 0; i < paths.Count; ++i)
                    {
                        string p = paths[i] as string;
                        if (p.StartsWith(path))
                        {
                            return Loader.GetBaseURL(pack.Key) + p;
                        }
                    }
                }
            }
            iter.Dispose();

            return string.Empty;
        }

        public static string AssetName(string path)
        {
#if HIDE_ASSET_NAME
			path = path.Replace('\\', '/').ToLower();
			return Hash.StringHash(path).ToString();
#else
            return Path.GetFileNameWithoutExtension(path);
#endif
        }

        public static string BundleName(string path)
        {
            AssetBundleInfo bundle = null;
            if (_pathToBundle.TryGetValue(path.ToLower(), out bundle))
            {
                return bundle.id;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 返回指定类型的obj（如果类型不是组件或需要转换的asset不是这个类型，都会返回null）
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Object ReturnType(Object asset, System.Type type)
        {
            if (asset == null) return null;

            if (type.IsInstanceOfType(asset))
            {
                return asset;
            }
            else if (asset is GameObject)
            {
                GameObject go = (GameObject)asset;
                if (type.IsSubclassOf(typeof(Component)))
                {
                    return go.GetComponent(type);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取相应的bundle信息
        /// </summary>
        /// <param name="id">bundle名称</param>
        /// <returns></returns>
        private static AssetBundleInfo GetBundleInfo(string id)
        {
            AssetBundleInfo info;
            if (_bundles.TryGetValue(id.ToLower(), out info))
            {
                return info;
            }
            //EB.Debug.LogError("GETBUNDLEINFO FAILED ON {0}", id);
            return null;
        }
        #endregion

        #region Others
        /// <summary>
        /// 添加bundle信息
        /// </summary>
        /// <param name="bundleInfo"></param>
        public static void AddAssetBundleInfo(AssetBundleInfo bundleInfo)
        {
            _bundles[bundleInfo.id] = bundleInfo;

			IEnumerator<string> enumerator = bundleInfo.paths.GetEnumerator();

            while (enumerator.MoveNext())
            {
				string path = enumerator.Current;
				//因为英雄的partitions模型，业务内读取只给名字，所以这里存的时候key要只留名字
				if (path.Contains("characterpartitions"))
                {
                    string charPartitionsKey = Path.GetFileNameWithoutExtension(path);
                    _pathToBundle[charPartitionsKey] = bundleInfo;
                }
                else
                {
                    _pathToBundle[path] = bundleInfo;
                }
            }
			enumerator.Dispose();
			enumerator = null;

			if (!_packs.ContainsKey(bundleInfo.packId))
            {
                AssetPack pack = new AssetPack();
                pack.id = bundleInfo.packId;
                pack.folder = Loader.GetBaseURL(bundleInfo.packId);
                pack.bundles = new List<AssetBundleInfo>();
                pack.folders = Johny.HashtablePool.Claim();
                _packs.Add(pack.id, pack);
            }

            _packs[bundleInfo.packId].bundles.Add(bundleInfo);
        }

        public static void AddDlcPacks(string[] packs)
        {
            _dlcPacks.AddRange(packs);
        }

        /// <summary>
        /// 开启协程
        /// </summary>
        /// <param name="co"></param>
        /// <returns></returns>
        private static Coroutine StartCoroutine(IEnumerator co)
        {
            return Coroutines.Run(co);
        }

        /// <summary>
        /// 修正渲染材质
        /// </summary>
        /// <param name="obj"></param>
        public static void FixupEditorMaterials(Object obj)
        {
#if UNITY_EDITOR
            if (obj != null)
            {
                var deps = UnityEditor.EditorUtility.CollectDependencies(new UnityEngine.Object[] { obj });
                List<UnityEngine.Object> objs = new List<Object>(deps);
                objs.Add(obj);

                foreach (var asset in objs)
                {
                    //GameObject go = asset as GameObject;
                    //if (go != null)
                    //{
                    //	Renderer[] skinnedMeshRenderer = go.GetComponentsInChildren<Renderer>(true);
                    //	for (int ii = 0; ii < skinnedMeshRenderer.Length; ++ii)
                    //	{
                    //		for (int jj = 0; jj < skinnedMeshRenderer[ii].sharedMaterials.Length; ++jj)
                    //		{
                    //			Material myMaterial = skinnedMeshRenderer[ii].sharedMaterials[jj];
                    //			if (myMaterial != null)
                    //			{
                    //				myMaterial.shader = Shader.Find(myMaterial.shader.name);
                    //				skinnedMeshRenderer[ii].sharedMaterials[jj] = myMaterial;
                    //			}
                    //		}
                    //	}
                    //}

                    Material mat = asset as Material;
                    if (mat != null)
                    {
                        mat.shader = Shader.Find(mat.shader.name);
                    }
                }

                //Shader.WarmupAllShaders();
            }
#endif
        }

        public static void FixupEditorMaterials()
        {
#if UNITY_EDITOR
            foreach (Material material in Resources.FindObjectsOfTypeAll(typeof(Material)))
            {
                var shader = material.shader;
                if (shader != null)
                {
                    var newShader = Shader.Find(shader.name);
                    if (newShader != null)
                    {
                        material.shader = newShader;
                    }
                }
            }
#endif
        }
#endregion


#region Deprecated 同步加载接口
        const bool DefaultSurrogate = false;

        [System.Obsolete("同步接口无法读取AB包资源，请尽早使用异步！")]
        public static Object Load(string path)
        {
            return Load(path, typeof(Object), DefaultSurrogate);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="bReturnSurrogate"></param>
        /// <returns></returns>
        [System.Obsolete("private, do not care!")]
        private static Object Load(string path, System.Type type, bool bReturnSurrogate)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.Log("EB.Assets.Load sent empty/null path!");
                return null;
            }
            string path_lower = path.Replace("Assets/", string.Empty).ToLower();
            Object obj = Find(path_lower);
            if (obj != null)
            {
                return ReturnType(obj, type);
            }

            System.DateTime start = System.DateTime.Now;
            obj = _Load(path, type, bReturnSurrogate);
            System.DateTime end = System.DateTime.Now;
            LoadCostLog(path_lower, (end - start).TotalMilliseconds);
            return obj;
        }

        /// <summary>
        /// 同步加载资源（不支持AB包读取）
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">类型</param>
        /// <param name="bReturnSurrogate">是否返回一个默认值</param>
        /// <returns></returns>
        [System.Obsolete("private, do not care!")]
        private static Object _Load(string path, System.Type type, bool bReturnSurrogate)
        {
            Object obj = null;
            string path_lower = path.Replace("Assets/", string.Empty).ToLower();
#if UNITY_EDITOR
            bool useBundle = Application.isPlaying;

#if !USE_ASSETBUNDLE_IN_EDITOR
            useBundle = false;
#endif
#if ENABLE_LOGGING
            EB.Debug.LogCoreAsset("CoreAssets_<color=#00ff00>同步加载</color>资源_不会内部实例化:path:<color=#00ff00>{0}</color>,是否使用AB包:<color=#00ff00>{1}</color>,返回组件:<color=#00ff00>{2}</color>"
                , path, useBundle, bReturnSurrogate);
#endif

            if (!useBundle)
            {
                //不使用bundle的加载
                obj = Resources.Load(path, type);
                if (obj != null)
                {
                    Track(path_lower, obj, AssetSource.Resources);
                    return obj;
                }

                Dictionary<System.Type, List<string>> possibleExts = new Dictionary<System.Type, List<string>>()
                {
                    { typeof(GameObject), new List<string>() { ".prefab" } },
                    { typeof(Texture), new List<string>() { ".png", ".tga", ".jpg" } },
                    { typeof(Texture2D), new List<string>() { ".png", ".tga", ".jpg" } },
                    { typeof(ParticleSystem), new List<string>() { ".prefab" } },
                    { typeof(Material), new List<string>() { ".mat" } },
                    { typeof(Object), new List<string>() { ".prefab", ".mat", ".png", ".tga", ".jpg" } },
                };

                if (possibleExts.ContainsKey(type))
                {
                    foreach (var ext in possibleExts[type])
                    {
                        obj = UnityEditor.AssetDatabase.LoadAssetAtPath(string.Format("Assets/{0}{1}", path, ext), type);
                        if (obj != null)
                        {
                            Track(path_lower, obj, AssetSource.Editor);
                            return obj;
                        }
                    }
                }
            }
#endif

            AssetBundleInfo info = null;
            if (_pathToBundle.TryGetValue(path_lower, out info))
            {//使用bundle的加载
#if ENABLE_LOGGING
                EB.Debug.LogCoreAsset("从<color=#00ff00>AssetBundle</color>里面加载:<color=#00ff00>{0}</color>,是否有加载好的资源:<color=#00ff00>{1}</color>", path_lower, info.isLoaded);
#endif
                if (info.isLoaded == false)
                {
                    return obj;
                }

                string assetName = AssetName(path_lower);
                Object asset = info.bundle.LoadAsset(assetName, type);
                if (asset != null)
                {
                    FixupEditorMaterials(asset);
                    Track(path_lower, asset, AssetSource.Bundle);
                    return ReturnType(asset, type);
                }

                //EB.Debug.LogError("Failed to load " + path_lower + " from bundle " + info.id);
                return null;
            }
            else
            {//如果bundle加载不到的话就从Resources文件夹下加载
#if ENABLE_LOGGING
                EB.Debug.LogCoreAsset("从<color=#00ff00>Resoruces</color>里面加载:<color=#00ff00>{0}</color>,加载类型:<color=#00ff00>{1}</color>,_pathToBundle个数:<color=#00ff00>{2}</color>", path, type, _pathToBundle.Count);
#endif
                //
                obj = Resources.Load(path, type);
                if (obj != null)
                {
                    Track(path_lower, obj, AssetSource.Resources);
                    return obj;
                }
            }

            if (bReturnSurrogate)
            {
                obj = ReturnType(MissingObject, type);
            }

            if (obj == null)
            {
                EB.Debug.LogError("Can't find: " + path);
            }

            return obj;
        }
#endregion


#region Recommanded 异步加载接口
        public static Coroutine LoadAllAsync(string path, System.Type type, System.Action<Object[]> callback)
        {
#if ENABLE_LOGGING
            EB.Debug.LogCoreAsset(string.Format("<color=#00ffff>异步加载</color><color=#00ff00>{0}</color>,type:<color=#00ff00>{1}</color>,回调事件:<color=#00ff00>{2}</color>", path, type, callback));
#endif
            return StartCoroutine(_LoadAllAsync(path, type, callback));
        }

        private static IEnumerator _LoadAllAsync(string path, System.Type type, System.Action<Object[]> fn)
        {
            List<Object> objects = new List<Object>();
            string path_lower = path.Replace("Assets/", string.Empty).ToLower();
            int loadCnt = 0;
            int finishLoadCnt = 0;
            foreach (KeyValuePair<string, AssetBundleInfo> kvp in _pathToBundle)
            {
                if (/*kvp.Value.isLoaded &&*/ kvp.Key.StartsWith(path_lower))//全取就不要关注isLoaded的状态了
                {
                    loadCnt++;
                    LoadAsync(kvp.Key, type, o =>
                    {
                        finishLoadCnt++;
                        if (o != null)
                        {
                            objects.Add(o);
                        }
                    });
                }
            }

            if (loadCnt == 0)
            {
                //bundle中没有，从resource中获取
                foreach (var asset in Resources.LoadAll(path, type))
                {
                    var p = (string.Format("{0}/{1}", path, asset.name)).ToLower();
                    var obj = Find(p);
                    if (obj != null)
                    {
                        objects.Add(obj);
                    }
                    else
                    {
                        Track(p, asset, AssetSource.Resources);
                        objects.Add(asset);
                    }
                }

            }
            else
            {
                yield return new WaitUntil(() =>
                {
                    return loadCnt == finishLoadCnt;
                });
            }

            fn(objects.ToArray());
        }

        // public static void PrintPathToBundle()
        // {
        //     var sw = File.CreateText("E://BundlePath.txt");
        //     foreach (var it in _pathToBundle)
        //     {
        //         sw.WriteLine(it.Key);
        //     }
        //     sw.Close();
        //     sw.Dispose();
        // }

        /// <summary>
        /// 根据AssetName来获取资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        /// <param name="target">如果目标为null了，则不回调了</param>
        /// <returns></returns>
        public static Coroutine LoadAsyncAndInit<T>(string assetName, System.Action<string,T,bool> callback, Object target, GameObject parent = null, bool needInit = true) where T : Object
        {
            string path = GM.AssetManager.GetAssetFullPathByAssetName(assetName);
            return LoadAsync(path, typeof(T), o =>
            {
                if(target == null)
                {
                    return;
                }

                if( o != null)
                {
                    if(needInit)
                    {
                        var go = GameObject.Instantiate(o) as T;
                        if(parent != null)
                        {
                            var goo = go as GameObject;
                            goo.transform.parent = parent.transform;
                        }
                        callback?.Invoke(assetName, go, true);
                    }
                    else
                    {
                        callback?.Invoke(assetName, o as T, true);
                    }
                }
                else
                {
                    callback?.Invoke(assetName, null, false);
                }
            });
        }

        /// <summary>
        /// 异步加载指定的资源(通过回调传回该资源)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Coroutine LoadAsync(string path, System.Type type, System.Action<Object> callback)
        {
            try
            {
                return StartCoroutine(_LoadAsync(path, type, callback));
            }
            catch(System.NullReferenceException e)
            {
                EB.Debug.LogError("[NullReference]LoadAsync===>path: {0}", path);
                callback?.Invoke(null);
                return null;
            }
        }

        /// <summary>
        /// 通过资源路径加载相应资源(通过回调传回该资源)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private static IEnumerator _LoadAsync(string path, System.Type type, System.Action<Object> callback)
        {
            Object asset = null;
            //for assetbundle存入key时也会这么做
            string path_lower = path.Replace("Assets/", string.Empty).ToLower();
#if USE_ASSETBUNDLE_IN_EDITOR || !UNITY_EDITOR
			AssetBundleInfo info = null;
			asset = Find(path_lower);
			if (asset != null)
			{//先找缓存池
				asset = ReturnType(asset, type);
			}
			else if (_pathToBundle.TryGetValue(path_lower, out info))
			{
#region Load Bundle
					info.asyncLoadingCount++;
					info.unload = false;
					if (info.isLoaded == false)
					{//Bundle没加载直接加载
						Coroutine coroutine = LoadBundle(info.id);
						if (!info.isLoaded)
						{
							yield return coroutine;
						}
					}
#endregion

#region Load Asset From Bundle
					var assetName = AssetName(path_lower);
					var async = info.bundle.LoadAssetAsync(assetName, type);

                    yield return new WaitUntil(()=> {return async.isDone;});
                    
                    asset = async.asset;
                    if (asset == null)
                    {
                        EB.Debug.LogError("Failed to load " + path_lower + " from bundle " + info.id);
                    }
                    else
                    {
                        FixupEditorMaterials(asset);
                        Track(path_lower, asset, AssetSource.Bundle);
                    }

					// decrement loading count
					info.asyncLoadingCount--;
#endregion
			}
			else
			{
#endif
#if UNITY_EDITOR && !USE_ASSETBUNDLE_IN_EDITOR
                string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(path).ToLower();
                string realPath = GM.AssetManager.GetPathByAssetName(fileNameWithoutExtension);
                if (realPath.Equals(string.Empty))
                {
                    string pathNoAssets = path.Replace("Assets/", string.Empty);
                    if (typeof(UnityEngine.TextAsset) == type)
                    {
                        realPath = $"Assets/{pathNoAssets}.txt";
                    }
                    else if (typeof(UnityEngine.GameObject) == type)
                    {
                        realPath = $"Assets/{pathNoAssets}.prefab";
                    }
                    else if (typeof(UnityEngine.Texture2D) == type)
                    {
                        realPath = $"Assets/{pathNoAssets}.exr";
                    }
                    else if(typeof(UnityEngine.Material) == type)
                    {
                        realPath = $"Assets/{pathNoAssets}.mat";
                    }
                    else if(typeof(UnityEngine.Video.VideoClip) == type)
                    {
                        realPath = $"Assets/{pathNoAssets}.mp4";
                    }
                    else
                    {
                        realPath = $"Assets/{pathNoAssets}.asset";
                    }
                }

                asset = AssetDatabase.LoadAssetAtPath(realPath, type);

#endif

                if (asset == null)
                {
                    ResourceRequest request = Resources.LoadAsync(path, type);
                    yield return new WaitUntil(()=> {return request.isDone;});

                    asset = request.asset;
                    if (asset != null)
                    {
                        Track(path_lower, asset, AssetSource.Resources);
                    }
                }

#if USE_ASSETBUNDLE_IN_EDITOR || !UNITY_EDITOR
			}
#endif
            if (callback != null)
            {
                if (asset == null)
                {
                    EB.Debug.LogError($"LoadAsync Failed! path: {path}");
                }
                callback(asset);
            }

            yield break;
        }

        public static Coroutine LoadLevel(string sceneName, bool forced = false)
        {
            return StartCoroutine(_LoadScene(sceneName, false, forced));
        }

        private static IEnumerator _LoadScene(string sceneName, bool additive, bool forceLoad)
        {
            while (IsLoadingScene)
            {
                yield return null;
            }

            if (CurrentScene == sceneName && !forceLoad)
            {
                yield break;
            }

            IsLoadingScene = true;

            // check to see if this is in a bundle or not
            AssetBundle bundle = null;
            AssetSceneInfo sceneInfo = null;
            if (_scenes.TryGetValue(sceneName, out sceneInfo))
            {
                Loader.CachedLoadHandler loader = new Loader.CachedLoadHandler(Loader.GetBundlePath(sceneInfo.packId, Loader.GetSceneBundleName(sceneInfo.id)), true, sceneInfo.ts);
                Coroutine coroutine = loader.Load();
                if (loader.assetBundle == null)
                {
                    yield return coroutine;
                }
                bundle = loader.assetBundle;
            }

            if (additive)
            {
                _loadLevelOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                while (!_loadLevelOperation.isDone)
                {
                    yield return _loadLevelOperation;
                }
            }
            else
            {
                _loadLevelOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
                while (!_loadLevelOperation.isDone)
                {
                    yield return _loadLevelOperation;
                }
            }

            CurrentScene = sceneName;

            if (bundle != null)
            {
#if ENABLE_LOGGING
                EB.Debug.LogCoreAsset("<color=#ff0000>910_释放AssetBundle</color>" + sceneName);
#endif
                bundle.Unload(false);
                FixupEditorMaterials();
            }

            IsLoadingScene = false;
        }
#endregion


#region Bundle
        /// <summary>
        /// AssetBundle是否已经加载完成
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsBundleLoaded(string id)
        {
            AssetBundleInfo info = GetBundleInfo(id);
            return info != null && info.isLoaded;
        }

        public static Coroutine LoadBundle(string bundleName, System.Action<string, AssetBundle, bool> action, GameObject target)
        {
            return StartCoroutine(DoLoadBundle(bundleName, action, target));
        }

        private static IEnumerator DoLoadBundle(string bundleName, System.Action<string, AssetBundle, bool> action, GameObject target)
        {
            AssetBundleInfo info = GetBundleInfo(bundleName);
            if(info == null)
            {
                action?.Invoke(bundleName, info.bundle, false);
            }
            else
            {
                Coroutine coroutine = LoadBundle(info.id);
                if (!info.isLoaded)
                {
                    yield return coroutine;
                }
                if(target)
                {
                    action?.Invoke(bundleName, info.bundle, true);
                }
            }
        }


        /// <summary>
        /// 开协程加载AssetBundle(同步加载)
        /// </summary>
        /// <param name="id">bundleName</param>
        /// <returns></returns>
        public static Coroutine LoadBundle(string id)
        {
            AssetBundleInfo info = GetBundleInfo(id);
            if (info == null)
            {
                EB.Debug.LogError("ERROR: Bundle " + id + " does not exist!");
                return null;
            }
            info.unload = false;
            if (info.isLoaded)
            {
                return null;
            }

            List<AssetBundleInfo> loadList = new List<AssetBundleInfo>();
            _GatherBundleLoadList(info, loadList);

            return StartCoroutine(_LoadBundles(loadList));
        }

        /// <summary>
        /// 加载AssetBundle,Bundle的加载是同步
        /// </summary>
        /// <param name="loadList">需要加载的列表</param>
        /// <param name="reqTimestamp"></param>
        /// <returns></returns>
        private static IEnumerator _LoadBundles(List<AssetBundleInfo> loadList, long reqTimestamp = 0)
        {
            WaitForFixedUpdate wait = new WaitForFixedUpdate();
            for (int i = 0; i < loadList.Count; i++)
            {
                AssetBundleInfo info = loadList[i];
                info.unload = false; // make sure we don't try to unload this

                if (!info.isLoaded)
                {
                    // see if we can start this load
                    if (!string.IsNullOrEmpty(info.parent) && !IsBundleLoaded(info.parent))
                    {
                        continue;
                    }

#region 加载AssetBundle
                    if (info.uncompressed ) // try and load off disk, if it's not already loading via WWW
                    {
                        var baseURL = "";
                        baseURL = Loader.GetBaseURL(info.packId);
                        var bundlePath = Loader.GetBundlePath(info.id);
                        var path = baseURL + bundlePath;
                        Loader.CachedLoadHandler loader = new Loader.CachedLoadHandler(path, true, reqTimestamp);
                        Coroutine coroutine = loader.Load();
                        if (loader.assetBundle == null)
                        {
                            yield return coroutine;
                        }
                        //
#if ENABLE_LOGGING
                        EB.Debug.LogCoreAsset("加载好_指定的AssetBundle,路径：<color=#00ff00>{0}</color>", path);
#endif
                        info.bundle = loader.assetBundle;

                        if (!info.isLoaded)
                        {
                            FatalLoadError(info);
                            break;
                        }
                        else
                        {
                            info.bundle.name = info.id;
                        }
                    }
#endregion
                }
            }
        }

        /// <summary>
        /// 在loadList中添加所有父级的AssetBundleInfo，包括自己
        /// </summary>
        /// <param name="info"></param>
        /// <param name="loadList"></param>
        private static void _GatherBundleLoadList(AssetBundleInfo info, List<AssetBundleInfo> loadList)
        {
            if (info.isLoaded == false)
            {
                // load parent if needed
                if (string.IsNullOrEmpty(info.parent) == false)
                {
                    AssetBundleInfo parent = GetBundleInfo(info.parent);
                    _GatherBundleLoadList(parent, loadList);
                }

                // add the load
                if (loadList.Contains(info) == false)
                {
                    //EB.Debug.Log("Need to load: " + info.id);
                    loadList.Add(info);
                }

                //PrintBundleLoadList(loadList);
            }
            else if (info.unload == true)
            {
                EB.Debug.LogError("********************** WARNING: skipping unload of " + info.id + " because it was requested to load again");
                // make sure not to unload
                info.unload = false;
            }
        }

#region WWW
        private static bool _fatalLoadError = false;

        private static void FatalLoadError(AssetBundleInfo info)
        {
            EB.Debug.LogError("Something went VERY WRONG loading bundle: " + info.id);
            _fatalLoadError = true;
        }

#endregion
#endregion


#region Unload Assets
        /// <summary>
        /// 卸载不在使用的Assets
        /// </summary>
        /// <param name="path">assetPath</param>
        /// <returns></returns>
        public static void UnloadUnusedAssets()
        {
            CoreAssets.UnloadUnusedAssets();
        }

        /// <summary>
        /// 卸载指定资源对象
        /// </summary>
        /// <param name="assetName">资产名</param>
        /// <param name="isOnlyUnloadAsset">是否只卸载资源</param>
        /// <returns></returns>
        public static bool UnloadAssetByName(string assetName, bool isOnlyUnloadAsset = false)
        {
            string path = GM.AssetManager.GetAssetFullPathByAssetName(assetName);
            return Unload(path, isOnlyUnloadAsset);
        }

        /// <summary>
        /// 卸载指定资源对象
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="isOnlyUnloadAsset">是否只卸载资源</param>
        /// <returns></returns>
        public static bool Unload(string path, bool isOnlyUnloadAsset = false)
        {
            string path_lower = path.Replace("Assets/", string.Empty).ToLower();
            return CoreAssets.Instance.Unload(path_lower, isOnlyUnloadAsset);
        }

        /// <summary>
        /// 卸载指定资源对象
        /// </summary>
        /// <param name="asset">指定的资源</param>
        /// <param name="isOnlyUnloadAsset">是否只卸载资源</param>
        /// <returns></returns>
        public static bool Unload(Object asset, bool isOnlyUnloadAsset = false)
        {
            return CoreAssets.Instance.Unload(asset, isOnlyUnloadAsset);
        }
#endregion


#region Unload Bundle
        /// <summary>
        /// 卸载指定Bundle
        /// </summary>
        /// <param name="id">BundleName</param>
        /// <returns></returns>
        public static void UnloadBundle(string id)
        {
            AssetBundleInfo info = GetBundleInfo(id);
            UnloadBundle(info);
        }

        /// <summary>
        /// 卸载指定Bundle
        /// </summary>
        /// <param name="path">assetPath</param>
        /// <returns></returns>
        public static void UnloadBundleFromResourcePath(string path)
        {
            AssetBundleInfo info = null;
            string path_lower = path.Replace("Assets/", string.Empty).ToLower();
            if (_pathToBundle.TryGetValue(path_lower, out info))
            {
                UnloadBundle(info);
            }
        }

        /// <summary>
        /// 卸载指定Bundle
        /// </summary>
        /// <param name="info">bundleInfo</param>
        /// <returns></returns>
        private static void UnloadBundle(AssetBundleInfo info)
        {
            if (info != null && info.isLoaded)
            {
                info.unload = true;
                if(!_unloads.Contains(info))
                {
                    _unloads.Add(info);
                }
            }
        }

        ///每帧监控是否有要unload的bundle
        public static AsyncOperation ProcessDeferedUnloads()
        {
            if (_unloads.Count == 0)
            {
                return null;
            }

            for (int i = 0; i < _unloads.Count;)
            {
                AssetBundleInfo info = _unloads[i];
                if (info.isLoaded == false || info.unload == false)
                {
                    _unloads.RemoveAt(i);
                }
                else if (info.asyncLoadingCount == 0)
                {
#if ENABLE_LOGGING
                    EB.Debug.LogCoreAsset(string.Format("<color=#ff0000>释放资源包</color><color=#00ff00>{0}</color>: ", info.id));
#endif
                    info.bundle.Unload(true);
                    info.bundle = null;
                    _unloads.RemoveAt(i);
                }
                else
                {
                    EB.Debug.LogWarning("Waiting for bundle to stop loading: " + info.id + " count:" + info.asyncLoadingCount);
                    ++i;
                }
            }

            return null;
        }
#endregion


#region Cache
        /// <summary>
        /// 添加资源进入缓存池
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <param name="src"></param>
        private static void Track(string path, Object obj, AssetSource src)
        {
            if (obj != null)
            {
                CoreAssets.Instance.Add(path, obj, src);
            }
        }

        /// <summary>
        /// 获取CoreAssets.Loaded里面的指定对象，没有则返回null
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static Object Find(string path)
        {
            return CoreAssets.Instance.Find(path);
        }
#endregion


#region Debug And Profile
        /// <summary> 资源加载的耗时列表</summary>
        static List<KeyValuePair<string, double>> loadlog = new List<KeyValuePair<string, double>>();

        /// <summary>
        /// 资源加载耗时日志
        /// </summary>
        /// <param name="assetName">资源名（也可以不是资源，这只是一个识别名）</param>
        /// <param name="cost">耗时</param>
        public static void LoadCostLog(string assetName, double cost)
        {
#if DEBUG
            loadlog.Add(new KeyValuePair<string, double>(assetName, cost));
#endif
        }

        /// <summary>
        /// 打印资源加载耗时日志
        /// </summary>
        /// <returns></returns>
        public static string PrintCostLog()
        {
            string ret = "";
            loadlog.ForEach(p =>
            {
                ret += ("loading " + p.Key + " " + p.Value + "\n");
            });
            EB.Debug.Log(ret);
            loadlog.Clear();
            return ret;
        }
    }
#endregion


#region 资源来源类型
    public enum AssetSource
    {
        /// <summary>
        /// Resources文件夹下
        /// </summary>
        Resources,
        /// <summary>
        /// AssetBundle
        /// </summary>
        Bundle,
        /// <summary>
        /// 编辑模式情况
        /// </summary>
        Editor
    }
#endregion


#region 内核资源管理器
    class CoreAssets : MonoBehaviour
    {
        /// <summary>
        /// 资源信息数据结构
        /// </summary>
        [System.Serializable]
        public class Info
        {
            /// <summary>
            /// 路径
            /// </summary>
            public string path;
            /// <summary>
            /// 资源对象
            /// </summary>
            public Object asset;
            /// <summary>
            /// 引用计数
            /// </summary>
            public int refCount;
            /// <summary>
            /// 资源来源
            /// </summary>
            public AssetSource source;
        }
        /// <summary>
        /// 所有的资源缓存列表
        /// </summary>
        public List<Info> Loaded = new List<Info>();

        private static AsyncOperation _cleanupTask = null;

        static CoreAssets _this = null;
        public static CoreAssets Instance
        {
            get
            {
                if (_this == null)
                {
                    var go = new GameObject("~ASSETS");
                    go.hideFlags = HideFlags.HideAndDontSave;
                    if (Application.isPlaying)
                    {
                        DontDestroyOnLoad(go);
                    }
                    _this = go.AddComponent<CoreAssets>();
                }
                return _this;
            }
        }
        /// <summary>
        /// 获取缓存对像的索引
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <returns></returns>
        int IndexOf(string path)
        {
            for (int i = 0; i < Loaded.Count; ++i)
            {
                if (Loaded[i].path == path)
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// 获取缓存对像的索引
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <returns></returns>
        int IndexOf(Object asset)
        {
            for (int i = 0; i < Loaded.Count; ++i)
            {
                if (Loaded[i].asset == asset)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 获取CoreAssets.Loaded里面的指定对象，没有则返回null
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Object Find(string path)
        {
            for (int i = 0; i < Loaded.Count; ++i)
            {
                if (Loaded[i].path == path)
                {
                    var info = Loaded[i];
                    info.refCount++;
                    return info.asset;
                }
            }

            return null;
        }

        void OnDestroy()
        {
            Loaded.Clear();
        }

        /// <summary>
        /// 添加资源进入缓存池
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="asset">资源</param>
        /// <param name="source">资源类型</param>
        public void Add(string path, Object asset, AssetSource source)
        {
            var index = IndexOf(path);
            if (index == -1)
            {
                index = Loaded.Count;
                Loaded.Add(new Info() { path = path, asset = asset, source = source, refCount = 0 });
                //
                StringBuilder str = new StringBuilder();
                /*
				for (int i=0;i< Loaded.Count;i++)
				{
					str.Append(string.Format("[{0}]<color=#00ff00>{1}</color>", i, Loaded[i].path));
				}*/
#if ENABLE_LOGGING
                EB.Debug.LogCoreAsset(string.Format("CoreAssets：<color=#00ff00>加入资源缓存池</color>path:<color=#00ff00>{0}</color>,资源对象:<color=#00ff00>{1}</color>,资源加载类型:<color=#00ff00>{2}</color>,资源缓存池个数:<color=#00ff00>{3}</color>,-----------{4}"
                    , path, asset, source, Loaded.Count, str));
#endif
            }
            else
            {
                //EB.Debug.LogWarning("Warning: loaded asset more than once for path " + path);
            }
            Loaded[index].refCount++;
        }

        /// <summary>
        /// 卸载指定资源对象
        /// </summary>
        /// <param name="asset">资源</param>
        /// <param name="isOnlyUnloadAsset">是否指卸载资源</param>
        /// <returns></returns>
        public bool Unload(Object asset, bool isOnlyUnloadAsset = false)
        {
            var index = IndexOf(asset);
            if (index >= 0)
            {
                var info = Loaded[index];
                info.refCount--;
                if (info.refCount == 0)
                {
                    Unload(index, isOnlyUnloadAsset);
#if ENABLE_LOGGING
					EB.Debug.LogCoreAsset(string.Format("CoreAssets：<color=#ff0000>移除缓存池</color>,靠对象asset:<color=#00ff00>{0}</color>,是否只卸载资源:<color=#00ff00>{1}</color>,Loaded.Count:<color=#00ff00>{2}</color>"
						, (asset == null ? "空对象" : asset.ToString()), isOnlyUnloadAsset, Loaded.Count));
#endif
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 卸载指定资源对像
        /// </summary>
        /// <param name="name">资源路径</param>
        /// <param name="isOnlyUnloadAsset">是否只卸载资源</param>
        /// <returns></returns>
        public bool Unload(string name, bool isOnlyUnloadAsset = false)
        {
            var index = IndexOf(name);
            if (index >= 0)
            {
                var info = Loaded[index];
                info.refCount--;
                if (info.refCount == 0)
                {
                    Unload(index, isOnlyUnloadAsset);
#if ENABLE_LOGGING
					EB.Debug.LogCoreAsset(string.Format("CoreAssets：<color=#ff0000>移除缓存池</color>,靠名字name:<color=#00ff00>{0}</color>,是否只卸载资源:<color=#00ff00>{1}</color>,Loaded.Count:<color=#00ff00>{2}</color>"
						, name, isOnlyUnloadAsset, Loaded.Count));
#endif

                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="index">资源在缓存池里的索引</param>
        /// <param name="isOnlyUnloadAsset">是否只卸载资源</param>
        void Unload(int index, bool isOnlyUnloadAsset = false)
        {
            var info = Loaded[index];
            if (info.asset != null)
            {
                if (info.asset is GameObject == false)
                {
                    Resources.UnloadAsset(info.asset);
                }
                info.asset = null;
            }
            else
            {
#if ENABLE_LOGGING
				Debug.LogError("为什么缓存池里的资源对像为空?info:", info);
#endif
            }

            // do a pop-back,将资源放这个被释放掉的资源内存里  //疑问：不直接用RemoveAt(index)，这是因为这个list是有序的吗？这样做可以节约内存重新分配的时间吗？ 哦摩西罗伊~
            Loaded[index] = Loaded[Loaded.Count - 1];
            Loaded.RemoveAt(Loaded.Count - 1);
        }

        /// <summary>
        /// 异步卸载未引用的资源
        /// </summary>
        /// <returns></returns>
        public static AsyncOperation UnloadUnusedAssets()
        {
            if (_cleanupTask == null || _cleanupTask.isDone)
            {
#if ENABLE_LOGGING
				EB.Debug.LogCoreAsset("CoreAssets：<color=#00ff00>释放未使用的资源_Resources.UnloadUnusedAssets()</color>");
#endif
                _cleanupTask = Resources.UnloadUnusedAssets();
            }

            return _cleanupTask;
        }
    }
#endregion
}