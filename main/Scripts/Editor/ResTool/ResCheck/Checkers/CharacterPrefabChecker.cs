using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CharacterPrefabChecker : ResChecker
{
    public ResCheckResult Check(ResCheckerCallBack callbacker)
    {
        List<string> errorMsg = new List<string>();

        var prefabs = Directory.GetFiles("Assets/Bundles/Player/", "*.prefab", SearchOption.AllDirectories).ToList<string>();

        callbacker.BeginCheck(this.Name(), prefabs.Count);

        for (int i = 0; i < prefabs.Count; i++)
        {
            var p = prefabs[i];
            var prefab = AssetDatabase.LoadAssetAtPath(p, typeof(GameObject)) as GameObject;
            if (prefab != null)
            {
                CheckCharatersGenericPoolLogic(prefab.transform, ref errorMsg);

                callbacker.OnCheckProgress(string.Format("Checking{0}", prefab), i, prefabs.Count);
            }
        }

        callbacker.OnCheckEnd();
        return new ResCheckResult(Name(), errorMsg);
    }

    /// <summary>
    /// 检查角色是否丢失GenericPoolLogic
    /// </summary>
    /// <param name="goTransform"></param>
    /// <param name="errorMsg"></param>
    private void CheckCharatersGenericPoolLogic(Transform goTransform, ref List<string> errorMsg)
    {
        var genericPoolLogic = goTransform.GetComponentsInChildren<GenericPoolLogic>();
        if (goTransform.name.IndexOf("Variant") != -1)
        {
            if (genericPoolLogic == null || genericPoolLogic.Length == 0)
            {
                errorMsg.Add(string.Format("{0}没有加上GenericPoolLogic组件", goTransform.name));
            }
        }
        else
        {
            if (genericPoolLogic != null || genericPoolLogic.Length > 0)
            {
                foreach (var g in genericPoolLogic)
                {
                    errorMsg.Add(string.Format("{0}的{1}节点不应该包括GenericPoolLogic组件", goTransform.name, g.gameObject.name));
                }
            }
        }
    }

    public string Name()
    {
        return "CharacterPrefabChecker";
    }
}
