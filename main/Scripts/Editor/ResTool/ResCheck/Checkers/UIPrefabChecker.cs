using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UIPrefabChecker : ResChecker
{
    private static List<string> GetAllPrefab(string dir)
    {
        var prefabs = Directory.GetFiles(dir, "*.prefab", SearchOption.AllDirectories).ToList<string>();

        return prefabs;
    }

    public ResCheckResult Check(ResCheckerCallBack callbacker)
    {
        var prefabs = GetAllPrefab(CatalogueConfig.UIPrefabPath);

        callbacker.BeginCheck(this.Name(), prefabs.Count);

        var errMsg = new List<string>();

        for (int i = 0; i < prefabs.Count; i++)
        {
            var prefab = prefabs[i];
            var goPrefab = AssetDatabase.LoadAssetAtPath(prefab, typeof(GameObject)) as GameObject;
            if (goPrefab != null)
            {
                CheckUIWidget(goPrefab, ref errMsg);

                callbacker.OnCheckProgress(string.Format("Checking{0}", prefab), i, prefabs.Count);
            }
        }

        callbacker.OnCheckEnd();

        return new ResCheckResult(Name(), errMsg);
    }

    private void CheckUIWidget(GameObject go, ref List<string> errMsg)
    {
        var rects = go.GetComponentsInChildren<UIRect>(true);
        foreach (var rect in rects)
        {
            if (rect.updateAnchors == UIRect.AnchorUpdate.OnUpdate)
            {
                errMsg.Add(string.Format("UI预置 {0} 子物体{1} AnchorUpdate 错误", go.name, rect.gameObject.name));
            }
        }
    }
    public string Name()
    {
        return "UIPrefabChecker";
    }
}
