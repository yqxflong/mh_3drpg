using System.Collections.Generic;
using UnityEngine;

public class UIAtlasRefCount : MonoBehaviour
{
    private List<UIAtlas> _uiAtlasList;

    //void Start()
    //{
    //    _uiAtlasList = AddUIAtlasToDict(gameObject);
    //}

    //void OnDestroy()
    //{
    //    if (_uiAtlasList != null && _uiAtlasList.Count > 0)
    //    {
    //        for (var i = 0; i < _uiAtlasList.Count; i++)
    //        {
    //            RemoveUIAtlasFromDict(_uiAtlasList[i], gameObject);
    //        }
    //    }
    //}

    #region UIAtlas Ref Count
    private class AtlasRefCount
    {
        public UIAtlas uiAtlas;
        public int refCount;

        public AtlasRefCount(UIAtlas uiAtlas, int refCount)
        {
            this.uiAtlas = uiAtlas;
            this.refCount = refCount;
        }
    }

    private static Dictionary<string, AtlasRefCount> _uiAtlasRefCountDict = new Dictionary<string, AtlasRefCount>();

    public static List<UIAtlas> AddUIAtlasToDict(GameObject go)
    {
        UISprite[] uiSprites = go.GetComponentsInChildren<UISprite>(true);
        List<UIAtlas> uiAtlasList = new List<UIAtlas>();
        UIAtlas uiAtlas;

        for (int i = 0; i < uiSprites.Length; i++)
        {
            uiAtlas = uiSprites[i].atlas;

            if (uiAtlas != null && !uiAtlasList.Contains(uiAtlas))
            {
                uiAtlasList.Add(uiAtlas);
                AddUIAtlasToDict(uiAtlas, go);
            }
        }

        return uiAtlasList;
    }

    public static void AddUIAtlasToDict(UIAtlas uiAtlas, GameObject go)
    {
        if (_uiAtlasRefCountDict.ContainsKey(uiAtlas.name))
        {
            _uiAtlasRefCountDict[uiAtlas.name].refCount += 1;
        }
        else
        {
            _uiAtlasRefCountDict.Add(uiAtlas.name, new AtlasRefCount(uiAtlas, 1));
        }
        
        //Debug.LogError(go.name + " # Add UIAtlas: 【" + uiAtlas.name + "】 --> ref count: " + _uiAtlasRefCountDict[uiAtlas.name].refCount);
    }

    public static void RemoveUIAtlasFromDict(UIAtlas uiAtlas, GameObject go)
    {
        if (_uiAtlasRefCountDict.ContainsKey(uiAtlas.name))
        {
            _uiAtlasRefCountDict[uiAtlas.name].refCount -= 1;

            //Debug.LogError(go.name + " # Remove UIAtlas: 【" + uiAtlas.name + "】 --> ref count: " + _uiAtlasRefCountDict[uiAtlas.name].refCount);

            if (_uiAtlasRefCountDict[uiAtlas.name].refCount <= 0)
            {
                var atlas = _uiAtlasRefCountDict[uiAtlas.name].uiAtlas;
                Resources.UnloadAsset(atlas.texture);
                Resources.UnloadAsset(atlas.spriteMaterial);
                _uiAtlasRefCountDict.Remove(uiAtlas.name);
            }
        }
    }
    #endregion
}
