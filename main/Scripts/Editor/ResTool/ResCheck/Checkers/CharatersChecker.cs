using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class CharatersChecker : ResChecker
{
    private string[] charatersNames = new string[]
    {
        "-model-preview",
        "-model-skin-body",
        "-model-weapon-pike"
    };

    private string[] defaultBones = new string[]
    {
        "Bip001 Pelvis"
    };

    private string[] headBones = new string[]
    {
        "HeadNub",
        "Bip001 HeadNub",
        "Bip001 Head"
    };

    private string[] chestBones = new string[]
    {
        "ChestNub",
        "Bip001 Spine1",
    };

    private string[] footBones = new string[]
    {
        "FootNub",
        "Bip001 L Toe0Nub",
        "Bip001 L Foot"
    };

    private string[] extraMeshChecker = new string[]
    {
        "M1001-model-skin-body",
        "M1003-model-skin-body"
    };

    /// <summary>
    /// 遍历所有模型
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static List<string> GetCharaterModel(string dir)
    {
        var fbxs = Directory.GetFiles(dir, "*.FBX", SearchOption.AllDirectories).ToList<string>();

        for (int i = fbxs.Count - 1; i >= 0; i--)
        {
            var fbx = fbxs[i];
            if (fbx.IndexOf("Animations") != -1)
            {
                fbxs.RemoveAt(i);
            }
        }
        return fbxs;
    }

    /// <summary>
    /// 遍历所有动画
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static List<string> GetCharacterAnim(string dir)
    {
        var fbxs = Directory.GetFiles(dir, "*.FBX", SearchOption.AllDirectories).ToList<string>();
        for (int i = fbxs.Count - 1; i >= 0; i--)
        {
            var fbx = fbxs[i];
            if (fbx.IndexOf("Animations") == -1)
            {
                fbxs.RemoveAt(i);
            }
        }
        return fbxs;
    }

    public const string s_CharactersDir = "Assets/_GameAssets/Res/Character/Characters/";

    public ResCheckResult Check(ResCheckerCallBack callbacker)
    {
        List<string> errMsg = new List<string>();

        //检查模型文件
        var models = GetCharaterModel(CatalogueConfig.CharactersDir);
        callbacker.BeginCheck(this.Name(), models.Count);

        for (int i = 0; i < models.Count; i++)
        {
            var model = models[i];
            var prefab = AssetDatabase.LoadAssetAtPath(model, typeof(GameObject)) as GameObject;
            if (prefab != null)
            {
                var go = GameObject.Instantiate(prefab) as GameObject;
                go.name = prefab.name;
                bool hasBones = HasBones(go.transform, ref errMsg);

                CheckCharatersName(go.name, ref errMsg);

                CheckRootBoneName(go.transform, ref errMsg);

                CheckCharatersMesh(go.transform, ref errMsg);

                CheckModelMaterial(go.transform, ref errMsg);

                GameObject.DestroyImmediate(go);
                callbacker.OnCheckProgress(string.Format("Checking{0}", model), i, models.Count);
            }
        }
        callbacker.OnCheckEnd();

        //检查动画文件
        var anims = GetCharacterAnim(CatalogueConfig.CharactersDir);
        callbacker.BeginCheck(this.Name(), anims.Count);

        for (int i = 0; i < anims.Count; i++)
        {
            var anim = anims[i];
            var prefab = AssetDatabase.LoadAssetAtPath(anim, typeof(GameObject)) as GameObject;
            if (prefab != null)
            {
                var go = GameObject.Instantiate(prefab) as GameObject;
                go.name = prefab.name;

                CheckAnimMesh(go.transform, ref errMsg);

                GameObject.DestroyImmediate(go);
                callbacker.OnCheckProgress(string.Format("Checking{0}", anim), i, anims.Count);
            }
        }
        callbacker.OnCheckEnd();

        return new ResCheckResult(Name(), errMsg);
    }

    private void CheckModelMaterial(Transform tran, ref List<string> errMsg)
    {
        Renderer[] renders = tran.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renders.Length; i++)
        {
            for (int j = 0; j < renders[i].sharedMaterials.Length; j++)
            {
                if (renders[i].sharedMaterials[j] == null)
                {
                    errMsg.Add(string.Format("模型文件 {0} 子物体{1} 材质 {2} 缺失", tran.name, renders[i].gameObject.name, j + 1));
                }
            }
        }
    }

    /// <summary>
    /// 检查是否有多余的武器节点或者身体网格
    /// </summary>
    /// <param name="goTransform"></param>
    /// <param name="errMsg"></param>
    private void CheckCharatersMesh(Transform goTransform, ref List<string> errMsg)
    {
        for (int i = 0; i < goTransform.childCount; i++)
        {
            Transform child = goTransform.GetChild(i);

            //要排除掉特殊的
            if (goTransform.name.IndexOf("-model-skin-body") != -1 &&
                !extraMeshChecker.Contains(goTransform.name))
            {
                if (child.name.ToLower().IndexOf("_weapon_mesh") != -1)
                {
                    errMsg.Add(string.Format("{0} 不应该有Weapon_Mesh", goTransform.name));
                }
            }
            else if (goTransform.name.IndexOf("-model-weapon-pike") != -1 &&
                !extraMeshChecker.Contains(goTransform.name))
            {
                if (child.name.ToLower().IndexOf("_body_mesh") != -1)
                {
                    errMsg.Add(string.Format("{0} 不应该有Body_Mesh", goTransform.name));
                }
            }
        }
    }

    /// <summary>
    /// 检查名字是否正确
    /// </summary>
    /// <param name="goname"></param>
    /// <param name="errMsg"></param>
    private void CheckCharatersName(string goname, ref List<string> errMsg)
    {
        bool isRightName = false;
        for (int i = 0; i < charatersNames.Length; i++)
        {
            if (goname.IndexOf(charatersNames[i]) != -1)
            {
                isRightName = true;
                break;
            }
        }
        if (!isRightName)
        {
            errMsg.Add(string.Format("{0} 名字命名不正确", goname));
        }
    }

    /// <summary>
    /// 检查有没根节点
    /// </summary>
    /// <param name="goTransform"></param>
    /// <param name="errMsg"></param>
    private void CheckRootBoneName(Transform goTransform, ref List<string> errMsg)
    {
        bool hasBip001 = false;
        for (int j = 0; j < goTransform.childCount; j++)
        {
            if (goTransform.GetChild(j).name.CompareTo("Bip001") == 0)
            {
                hasBip001 = true;
                break;
            }
        }
        if (!hasBip001)
        {
            errMsg.Add(string.Format("{0} 没有根节点Bip001", goTransform.name));
        }
    }

    /// <summary>
    /// 是否有指定挂点
    /// </summary>
    /// <param name="goTransform"></param>
    /// <param name="errMsg"></param>
    /// <returns></returns>
    private bool HasBones(Transform goTransform, ref List<string> errMsg)
    {
        bool hasBones = true;
        for (int i = 0; i < headBones.Length; i++)
        {
            hasBones = CheckFromTransform(goTransform, headBones[i]);

            //有节点就退出
            if (hasBones)
            {
                break;
            }
        }
        if (!hasBones)
        {
            errMsg.Add(string.Format("{0} 丢失了头部挂点", goTransform.name));
        }

        for (int i = 0; i < chestBones.Length; i++)
        {
            hasBones = CheckFromTransform(goTransform, chestBones[i]);

            //有节点就退出
            if (hasBones)
            {
                break;
            }
        }
        if (!hasBones)
        {
            errMsg.Add(string.Format("{0} 丢失了胸部挂点", goTransform.name));
        }

        for (int i = 0; i < footBones.Length; i++)
        {
            hasBones = CheckFromTransform(goTransform, footBones[i]);

            //有节点就退出
            if (hasBones)
            {
                break;
            }
        }

        if (!hasBones)
        {
            errMsg.Add(string.Format("{0} 丢失了脚部挂点", goTransform.name));
        }

        return hasBones;
    }

    /// <summary>
    /// 在指定对象中查询骨骼
    /// </summary>
    /// <param name="goTransform"></param>
    /// <param name="boneName"></param>
    /// <returns></returns>
    private bool CheckFromTransform(Transform goTransform, string boneName)
    {
        bool hasBones = false;
        //Debug.LogError(string.Format("transform name:{0} has child :{1}", goTransform.name, goTransform.childCount));
        for (int i = 0; i < goTransform.childCount; i++)
        {
            Transform child = goTransform.GetChild(i);
            if (child.name.CompareTo(boneName) != 0)
            {
                //搜索子对象
                if (child.childCount > 0)
                {
                    hasBones = CheckFromTransform(child, boneName);
                }
            }
            else
            {
                hasBones = true;
            }

            //有节点就退出
            if (hasBones)
            {
                break;
            }
        }
        return hasBones;
    }

    /// <summary>
    /// 检查动画文件是否包含异常物体
    /// </summary>
    /// <param name="tran"></param>
    /// <param name="errMsg"></param>
    private void CheckAnimMesh(Transform tran, ref List<string> errMsg)
    {
        for (int i = 0; i < tran.childCount; i++)
        {
            Transform childTran = tran.GetChild(i);
            errMsg.Add(string.Format("动画文件 {0} 不应该有子物体 {1}", tran.name, childTran.name));
        }
    }

    public string Name()
    {
        return "CharatersChecker";
    }
}