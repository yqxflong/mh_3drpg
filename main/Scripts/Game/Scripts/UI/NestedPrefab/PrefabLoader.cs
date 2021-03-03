using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLoader : MonoBehaviour
{
    public string PrefabName;

    public bool IsAssetLoaded = false;


    public void LoadPrefab()
    {
        if (PrefabName.Equals(string.Empty))
        {
            return;
        }
        // GM.AssetManager.GetAsset<GameObject>(PrefabName, OnAssetReady, this.gameObject);
        EB.Assets.LoadAsyncAndInit<GameObject>(PrefabName, OnAssetReady, this.gameObject);
    }

    private void OnAssetReady(string assetname, GameObject obj, bool isSuccessed)
    {
        if (!isSuccessed)
        {
            EB.Debug.LogError("Asset {0} dont find", PrefabName);
        }
        else
        {
            if (this == null)
            {
                Destroy(obj);
            }
            else
            {
                obj.name = PrefabName;
                obj.transform.parent = transform;
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
            }
        }

        IsAssetLoaded = true;
    }
}
