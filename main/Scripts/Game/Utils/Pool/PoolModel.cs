//PoolModel
//用于管理模型的池子
//Johny

using System.Collections.Generic;
using UnityEngine;

public static class PoolModel
{
    public struct AsyncModel
    {
        public string resPath;
        public Object obj;
    }

    private static List<Object> _modelQueueSync= new List<Object>();
    private static List<AsyncModel> _modelQueue = new List<AsyncModel>();

    ///异步预加载模板
    public static Coroutine PreloadAsync(string resourcePath, System.Action<Object> act = null)
    {
        return EB.Assets.LoadAsync(resourcePath, typeof(GameObject), o=>{
            act?.Invoke(o);
        });
    }

    ///直接传入Object模板，返回其初始化的Go
    public static Object GetModel(Object o, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion))
    {
        var go = GameObject.Instantiate(o, position, rotation);
        _modelQueueSync.Add(go);
        return go;
    }

    ///异步获取Model，强烈推荐！！
    public static Coroutine GetModelAsync(string resourcePath, Vector3 position, Quaternion rotation, System.Action<UnityEngine.Object, object> callback, object param)
    {
        return EB.Assets.LoadAsync(resourcePath, typeof(GameObject), o=>
        {
            var go = GameObject.Instantiate(o, position, rotation);
            go.name = go.name.Replace("(Clone)", "");
            _modelQueue.Add(new AsyncModel(){resPath = resourcePath, obj = go});
            callback?.Invoke(go, param);
        });
    }

    ///销毁指定Go
    public static void DestroyModel(Object o)
    {
        bool founded = false;
        #region 查看异步加载队列中的model并destroy
        for(int i = 0; i< _modelQueue.Count;i++)
        {
            var model = _modelQueue[i];
            if(model.obj == o)
            {
                _modelQueue[i] = model;
                GameObject.Destroy(model.obj);
                founded = true;
                break;
            }
        }
        #endregion

        #region 查看同步加载队列中的model并destroy
        for(int i = 0; i< _modelQueueSync.Count; i++)
        {
            var model = _modelQueueSync[i];
            if(model == o)
            {
                _modelQueueSync.Remove(model);
                GameObject.Destroy(model);
                founded = true;
                break;
            }
        }
        #endregion 

        //不在队列中，但调用此方法销毁的,帮其销毁
        if(!founded)
        {
            GameObject.Destroy(o);
        }
    }

    ///清除指定资源
    public static void ClearResource(string assetName)
    {
        #region 查找异步队列清除指定资源
        for(int i = 0; i < _modelQueue.Count;)
        {
            var model = _modelQueue[i];
            if(model.resPath.Contains(assetName))
            {
                if(model.obj != null)
                {
                    GameObject.Destroy(model.obj);
                }
                EB.Assets.Unload(model.resPath);
                _modelQueue.RemoveAt(i);
            }
            else if(model.obj == null)
            {
                EB.Assets.Unload(model.resPath);
                _modelQueue.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
        #endregion 

        #region 查找同步队列清除指定资源
        for(int i = 0; i < _modelQueueSync.Count;)
        {
            var model = _modelQueueSync[i];
            if(model == null)
            {
                _modelQueueSync.RemoveAt(i);
            }
            else if(model.name.Contains(assetName))
            {
                GameObject.Destroy(model);
                _modelQueueSync.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
        #endregion
    }

    ///清除所有资源
    public static void ClearAllResource(bool clearTemplatePool = false)
    {
        #region Clear 异步队列
        foreach(var asyncModel in _modelQueue)
        {
            if(asyncModel.obj != null)
            {
                GameObject.Destroy(asyncModel.obj);
            }
            EB.Assets.Unload(asyncModel.resPath);
        }
        _modelQueue.Clear();
        #endregion

        #region Clear 同步队列
        for(int i = 0; i < _modelQueueSync.Count;i++)
        {
            var model = _modelQueueSync[i];
            GameObject.Destroy(model);
        }
        _modelQueueSync.Clear();
        #endregion
    }
}
