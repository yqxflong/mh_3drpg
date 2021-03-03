
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PrefabMissChecker : ResChecker
{
    private static List<string> GetAllPrefab(string dir)
    {
        var prefabs = Directory.GetFiles(dir, "*.prefab", SearchOption.AllDirectories).ToList<string>();

        return prefabs;
    }

    const string s_PrefabDir = "Assets/";
    public ResCheckResult Check(ResCheckerCallBack callbacker)
    {
        var prefabs = GetAllPrefab(s_PrefabDir);

        callbacker.BeginCheck(this.Name(), prefabs.Count);

        var error = new List<string>();

        for (int i = 0; i < prefabs.Count; i++)
        {
            var prefab = prefabs[i];
            var goPrefab = AssetDatabase.LoadAssetAtPath(prefab, typeof(GameObject)) as GameObject;
            if (goPrefab != null)
            {
                missScriptInPrefab(goPrefab, ref error);

                //GameObject.DestroyImmediate(goPrefab);
                callbacker.OnCheckProgress(string.Format("Checking{0}", prefab), i, prefabs.Count);
            }
        }

        callbacker.OnCheckEnd();
        return new ResCheckResult(Name(), error);
    }

    private void missScriptInPrefab(GameObject go, ref List<string> error)
    {
        var components = go.GetComponentsInChildren<Component>(true);
        foreach (var c in components)
        {
            if (c == null)
            {
                error.Add(string.Format("prefab名字：{0}有脚本丢失,请确认是否要删除该脚本", go.name));
            }
        }
    }

    public string Name()
    {
        return "PrefabMissChecker";
    }
}









public class PrefabMissMatChecker : ResChecker
{
    private static List<string> GetAllPrefab(string dir)
    {
        var prefabs = Directory.GetFiles(dir, "*.prefab", SearchOption.AllDirectories).ToList<string>();

        return prefabs;
    }
    private static List<string> GetAllFbx(string dir)
    {
        var fbxs = Directory.GetFiles(dir, "*.FBX", SearchOption.AllDirectories).ToList<string>();

        return fbxs;
    }

    const string s_PrefabDir = "Assets/";
    public ResCheckResult Check(ResCheckerCallBack callbacker)
    {
        var prefabs = GetAllPrefab(s_PrefabDir);

        callbacker.BeginCheck(this.Name(), prefabs.Count);

        var error = new List<string>();

        for (int i = 0; i < prefabs.Count; i++)
        {
            var prefab = prefabs[i];
            var goPrefab = AssetDatabase.LoadAssetAtPath(prefab, typeof(GameObject)) as GameObject;
            if (goPrefab != null)
            {

                var skinnedMeshs = goPrefab.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach(var skinnedmesh in skinnedMeshs)
                {
                    foreach (var mat in skinnedmesh.sharedMaterials)
                    {
                        if (mat == null)
                        {
                            error.Add(string.Format("prefab名字：{0}有材质丢失在：{1}节点", goPrefab.name, skinnedmesh.gameObject.name));
                        }
                    }
                }

                var meshRenderers = goPrefab.GetComponentsInChildren<MeshRenderer>();
                foreach (var meshRenderer in meshRenderers)
                {
                    if (meshRenderer == null)
                    {
                        error.Add(string.Format("prefab名字：{0}有mesh丢失在", goPrefab.name));
                        continue;
                    }
                    foreach (var mat in meshRenderer.sharedMaterials)
                    {
                        if (mat == null)
                        {
                            error.Add(string.Format("prefab名字：{0}有材质丢失在：{1}节点", goPrefab.name, meshRenderer.gameObject.name));
                        }
                    }
                }

                var particleSystems = goPrefab.GetComponentsInChildren<Renderer>();
                foreach (var particleSystem in particleSystems)
                {
                    if (particleSystem == null)
                    {
                        error.Add(string.Format("粒子名字：{0}有Renderer丢失在", goPrefab.name));
                        continue;
                    }
                    if (!particleSystem.enabled)
                        continue;

                    int missNum = 0;
                    foreach(var mat in particleSystem.sharedMaterials)
                    {
                        if (mat != null)
                        {
                            continue;
                        }
                        if (mat == null)
                        {
                            missNum++;
                        }
                    }
                    if (missNum >= particleSystem.sharedMaterials.Length)
                        error.Add(string.Format("粒子名字：{0}有材质丢失在：{1}节点", goPrefab.name, particleSystem.gameObject.name));
                }

                //GameObject.DestroyImmediate(goPrefab);
                callbacker.OnCheckProgress(string.Format("Checking{0}", prefab), i, prefabs.Count);
            }
        }

        callbacker.OnCheckEnd();


        var fbxs = GetAllFbx(s_PrefabDir);

        callbacker.BeginCheck(this.Name(), fbxs.Count);

        for (int i = 0; i < fbxs.Count; i++)
        {
            var fbx = fbxs[i];
            var goPrefab = AssetDatabase.LoadAssetAtPath(fbx, typeof(GameObject)) as GameObject;
            if (goPrefab != null)
            {

                var skinnedMeshs = goPrefab.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var skinnedmesh in skinnedMeshs)
                {
                    foreach (var mat in skinnedmesh.sharedMaterials)
                    {
                        if (mat == null)
                        {
                            error.Add(string.Format("fbx名字：{0}有材质丢失在：{1}节点", goPrefab.name, skinnedmesh.gameObject.name));
                        }
                    }
                }

                var meshRenderers = goPrefab.GetComponentsInChildren<MeshRenderer>();
                foreach (var meshRenderer in meshRenderers)
                {
                    if (meshRenderer == null)
                    {
                        error.Add(string.Format("fbx名字：{0}有mesh丢失在", goPrefab.name));
                        continue;
                    }
                    foreach (var mat in meshRenderer.sharedMaterials)
                    {
                        if (mat == null)
                        {
                            error.Add(string.Format("fbx名字：{0}有材质丢失在：{1}节点", goPrefab.name, meshRenderer.gameObject.name));
                        }
                    }
                }
                
                var particleSystems = goPrefab.GetComponentsInChildren<Renderer>();
                foreach (var particleSystem in particleSystems)
                {
                    if (particleSystem == null)
                    {
                        error.Add(string.Format("粒子名字：{0}有Renderer丢失在", goPrefab.name));
                        continue;
                    }
                    if (!particleSystem.enabled)
                        continue;

                    int missNum = 0;
                    foreach (var mat in particleSystem.sharedMaterials)
                    {
                        if (mat != null)
                        {
                            continue;
                        }
                        if (mat == null)
                        {
                            missNum++;
                        }
                    }
                    if (missNum >= particleSystem.sharedMaterials.Length)
                        error.Add(string.Format("粒子名字：{0}有材质丢失在：{1}节点", goPrefab.name, particleSystem.gameObject.name));
                }

                //GameObject.DestroyImmediate(goPrefab);
                callbacker.OnCheckProgress(string.Format("Checking{0}", fbx), i, fbxs.Count);
            }
        }

        callbacker.OnCheckEnd();

        return new ResCheckResult(Name(), error);
    }

    public string Name()
    {
        return "PrefabMissMatChecker";
    }
}


public class PrefabsDcCutChecker : ResChecker
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

        var error = new List<string>();

        for (int i = 0; i < prefabs.Count; i++)
        {
            var prefab = prefabs[i];
            var goPrefab = AssetDatabase.LoadAssetAtPath(prefab, typeof(GameObject)) as GameObject;
            if (goPrefab != null)
            {
                CheckPrefabAtlasTextureDepth(goPrefab, ref error);

                //GameObject.DestroyImmediate(goPrefab);
                callbacker.OnCheckProgress(string.Format("Checking{0}", prefab), i, prefabs.Count);
            }
        }

        callbacker.OnCheckEnd();
        return new ResCheckResult(Name(), error);
    }
    
    private void CheckPrefabAtlasTextureDepth(GameObject go, ref List<string> error)
    {
        List<Dictionary<string, int>> saveAtlasDepth = new List<Dictionary<string, int>>();
        List<Dictionary<string, int>> saveSpriteDepth = new List<Dictionary<string, int>>();
        var sprites = go.GetComponentsInChildren<UISprite>(true);
        foreach (var sprite in sprites)
        {
            if (sprite != null && sprite.atlas != null)
            {
                string atlasCode = sprite.atlas.name;
                Dictionary<string, int> item = new Dictionary<string, int>();
                item.Add(atlasCode, sprite.depth);
                saveAtlasDepth.Add(item);
                Dictionary<string, int> item2 = new Dictionary<string, int>();
                item2.Add(sprite.name, sprite.depth);
                saveSpriteDepth.Add(item2);
            }
        }


        saveAtlasDepth.Sort(
            (left, right) =>
            {
                var aItem = left.GetEnumerator();
                var bItem = right.GetEnumerator();

                int xValue = 0;
                int yValue = 0;
                while (aItem.MoveNext())
                {
                    xValue = aItem.Current.Value;
                }
                while (bItem.MoveNext())
                {
                    yValue = bItem.Current.Value;
                }
                if (xValue > yValue)
                {
                    return -1;
                }
                return 1;
            });

        saveSpriteDepth.Sort(
            (left, right) =>
            {
                var aItem = left.GetEnumerator();
                var bItem = right.GetEnumerator();

                int xValue = 0;
                int yValue = 0;
                while (aItem.MoveNext())
                {
                    xValue = aItem.Current.Value;
                }
                while (bItem.MoveNext())
                {
                    yValue = bItem.Current.Value;
                }
                if (xValue > yValue)
                {
                    return -1;
                }
                return 1;
            });

        List<string> errorName = new List<string>();
        string diff = "";
        string diffSpriteName = "";
        for (int i = 0; i < saveAtlasDepth.Count; i++)
        {
            diff = "";

            var iItem = saveAtlasDepth[i].GetEnumerator();
            string iKey = "";
            int iValue = 0;
            while (iItem.MoveNext())
            {
                iKey = iItem.Current.Key;
                iValue = iItem.Current.Value;
            }
            //Debug.LogError("iKey ：" + iKey);

            for (int j = i + 1; j < saveAtlasDepth.Count; j++)
            {
                var jItem = saveAtlasDepth[j].GetEnumerator();
                string jKey = "";
                int jValue = 0;
                while (jItem.MoveNext())
                {
                    jKey = jItem.Current.Key;
                    jValue = jItem.Current.Value;
                }
                if (iKey == jKey && string.IsNullOrEmpty(diff))
                {
                    break;
                }
                else
                {
                    if (string.IsNullOrEmpty(diff))
                    {
                        //还没记录要比较的就记录
                        diff = jKey;

                        var jSprites = saveSpriteDepth[j].GetEnumerator();

                        while (jSprites.MoveNext())
                        {
                            diffSpriteName = jSprites.Current.Key;
                        }
                        continue;
                    }
                    //如果比较的对象和这一个对象不同则说明这个对象的atlas又改了，然后又等于saveAtlasDepth[i].GetEnumerator().Current.Key的说明saveAtlasDepth[i].GetEnumerator().Current.Key被截断了
                    if (jKey != diff &&
                        jKey == iKey)
                    {
                        var iSprites = saveSpriteDepth[i].GetEnumerator();

                        string cutName = "";
                        while (iSprites.MoveNext())
                        {
                            cutName = iSprites.Current.Key;
                        }
                        if (!errorName.Contains(diffSpriteName))
                        {
                            errorName.Add(diffSpriteName);
                            error.Add(string.Format("atlas的depth被截断：{0}截断了{1},具体的对象是{2}截断了{3}", diff, iKey, diffSpriteName, cutName));
                        }
                    }
                }
            }
        }

    }

    public string Name()
    {
        return "PrefabsDcCutChecker";
    }
}
