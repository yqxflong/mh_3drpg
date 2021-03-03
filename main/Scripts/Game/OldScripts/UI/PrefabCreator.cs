using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 最低级预设 不含UIPlane 一般就是Button  Sprite Label这些组合在一起的小单元 
/// PS:采用热更的界面才使用此方案，因为需要根据路径去拿，如果预设改变是要修改路径的
/// </summary>
[RequireComponent(typeof(UIWidget))]
public class PrefabCreator : MonoBehaviour
{
    /// <summary>
    /// 预设名
    /// </summary>
    public string prefabName;

    /// <summary>
    /// 当前是否读取完成
    /// </summary>
    public bool isCurrendAssetLoaded;

    public bool isLoadAsset;

    public UIWidget[] _uiwidgets;
    public Transform[] _trans;
    
    public void LoadAsset()
    {
        if(prefabName.Equals(string.Empty)&& transform.childCount>0) //不写就默认用第一个子对象的name
        {
            prefabName = transform.GetChild(0).name;
        }
        //isCurrendAssetLoaded = true;
        isLoadAsset = true;
        // GM.AssetManager.GetAsset<GameObject>(prefabName, OnAssetReady, this.gameObject);
        EB.Assets.LoadAsyncAndInit<GameObject>(prefabName, OnAssetReady, this.gameObject);
    }

    public UIWidget GetUIWidget(string name)
    {
        for(int i=0;i<_uiwidgets.Length;i++)
        {
            if(_uiwidgets[i].name.Equals(name))
            {
                return _uiwidgets[i];
            }
        }
        return null;
    }

    public Transform GetTransform(string name)
    {
        for (int i = 0; i < _trans.Length; i++)
        {
            if (_trans[i].name.Equals(name))
            {
                return _trans[i];
            }
        }
        return null;
    }

    void OnAssetReady(string assetname, GameObject go, bool isSuccessed)
    {
        if (!isSuccessed)
        {
            EB.Debug.LogError(string.Format("Asset {0} dont find", prefabName));
        }
        else
        {
            if(this == null)
            {
                Destroy(go);
            }
            else
            {
                if(transform.childCount>0)
                {
                    transform.DestroyChildren();
                }
                go.name = prefabName;
                go.transform.parent = transform;
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;

                UIWidget prefabWidget = gameObject.GetComponent<UIWidget>();
                UIWidget upWidget = go.GetComponent<UIWidget>();
                int upDepth = upWidget == null ? 0 : upWidget.depth;
                UIWidget[] ws = go.GetComponentsInChildren<UIWidget>(true);
                _uiwidgets = ws;
                for (int i=0;i<ws.Length;i++)
                {
                    if(ws[i]!=null)
                    {
                        ws[i].depth = ws[i].depth - upDepth + prefabWidget.depth;
                    }
                }
                _trans = go.GetComponentsInChildren<Transform>(true);

            }
        }
        isCurrendAssetLoaded = true;
    }
}
