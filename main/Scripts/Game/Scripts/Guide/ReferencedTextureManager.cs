using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ReferencedTextureManager {

    private Dictionary<string, int> mCachedTexture2DReferenceDict = new Dictionary<string, int>();
    private static ReferencedTextureManager _instance = null;
    public static ReferencedTextureManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ReferencedTextureManager();
                _instance.Initialize();
            }

            return _instance;
        }
    }

    private ReferencedTextureManager()
    {

    }

    private void Initialize()
    {
        mCachedTexture2DReferenceDict.Clear();
    }

    /// <summary>
    /// 动态加载texture资源，提前打进bundle
    /// </summary>
    /// <param name="texName">贴图名</param>
    /// <param name="action">加载完成回调</param>
    /// <param name="target">事件发送者</param>
    public static  void GetTexture2DAsync(string texName, System.Action<string, Texture2D, bool> action, GameObject target)
    {
        Instance.GetTexture2DAsyncImpl(texName, action, target);
    }

    public void GetTexture2DAsyncImpl(string texName, System.Action<string, Texture2D, bool> action, GameObject target)
    {
        GM.TextureManager.GetTexture2DAsync(texName, action, target);
        if (action != null) AddReference(texName);//给名字标引用，不管是否加载成功
    }

    public static  void ReleaseTexture(string texName)
    {
        Instance.ReleaseTextureImpl(texName);
    }

    public void ReleaseTextureImpl(string texName)
    {
        if (ReduceReference(texName))
        {
            //Debug.LogError("Release : " + texName);
            GM.TextureManager.ReleaseTexture(texName);
        }
    }

    public int AddReference(string texName)
    {
        //Debug.LogError(texName + "add reference");
        int reference = 0;
        if (mCachedTexture2DReferenceDict.ContainsKey(texName))
        {
            reference = mCachedTexture2DReferenceDict[texName];
            reference++;
            mCachedTexture2DReferenceDict[texName] = reference;
        }
        else
        {
            reference++;
            mCachedTexture2DReferenceDict.Add(texName, reference);

        }
        return reference;
    }

    public bool ReduceReference(string texName)
    {
        int reference = 0;
        if (mCachedTexture2DReferenceDict.ContainsKey(texName))
        {
            reference = mCachedTexture2DReferenceDict[texName];
            if (--reference <= 0)
            {
                if (reference < 0)
                {
                    EB.Debug.LogError("ReduceReference: refCount underflow {0} {1}", reference, texName);
                }
                mCachedTexture2DReferenceDict.Remove(texName);
                return true;
            }
        }
        else
        {
            EB.Debug.LogError("ReduceReference: isnot exist {0}", texName);
        }
        return false;
    }
}
