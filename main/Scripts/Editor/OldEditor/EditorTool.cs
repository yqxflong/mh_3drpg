using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class EditorTool {

    [MenuItem("Custom/Delete Persistent File")]
    static void DeletePersistentFile()
    {
        string dataPath = Application.persistentDataPath;
        Debug.Log("persistentDataPath=" + Application.persistentDataPath);
        if (Directory.Exists(dataPath))
        {
            PlayerPrefs.DeleteAll();

            string[] strFile = Directory.GetFiles(dataPath);
            foreach (string str in strFile)
            {
                File.Delete(str);
            }

            Debug.Log("dataPath=" + dataPath + ": Delete Persistent File over!");
            //Directory.Delete(dataPath);
        }
        else
        {
            EB.Debug.LogError(string.Format("donot Exists {0} dataPath", dataPath));
        }
    }

    [MenuItem("Custom/SortAllParticleOrder")]
    public static void SortAllParticleOrder()
    {
        string[] fileList = System.IO.Directory.GetFiles("Assets/Art/fx/particles", "*.prefab", System.IO.SearchOption.AllDirectories);
        List<Material> sortedMats = new List<Material>();
        foreach (string name in fileList)
        {
            GameObject asset = UnityEditor.AssetDatabase.LoadAssetAtPath(name, typeof(GameObject)) as GameObject;

            Renderer[] renders = asset.GetComponentsInChildren<Renderer>();

            foreach (var r in renders)
            {
                if (r != null && r.sharedMaterial != null)
                {
                    if (!sortedMats.Contains(r.sharedMaterial))
                        sortedMats.Add(r.sharedMaterial);
                }
            }
        }

        sortedMats.Sort(delegate (Material left, Material right)
        {
            if (left.shader != right.shader)
            {
                if (left.shader.name == "Particles/Additive")
                    return 1;
                else
                    return -1;
            }
            return 0;
        });

        for (int sortIndex = 0; sortIndex < sortedMats.Count; ++sortIndex)
        {
            Debug.Log(string.Format("SortOrder={0} MaterialName={1}", sortIndex * 10, sortedMats[sortIndex].name));
        }

        foreach (string name in fileList)
        {
            GameObject asset = UnityEditor.AssetDatabase.LoadAssetAtPath(name, typeof(GameObject)) as GameObject;

            Renderer[] renders = asset.GetComponentsInChildren<Renderer>();

            foreach (var r in renders)
            {
                if (r != null && r.sharedMaterial != null)
                {
                    int matSortIndex = sortedMats.IndexOf(r.sharedMaterial);
                    r.sortingOrder = matSortIndex * 10;
                }
            }
        }

        Debug.Log("particles sort over,max sortOrder=" + (sortedMats.Count - 1) * 10);
    }

    [MenuItem("Custom/CheckCharacterMaterialTool")]
    public static void CheckCharacterMaterialTool()
    {
        string[] fileList = System.IO.Directory.GetFiles("Assets/_GameAssets/Res/Character/Characters", "*.mat", System.IO.SearchOption.AllDirectories);
        foreach (string name in fileList)
        {
            Material asset = UnityEditor.AssetDatabase.LoadAssetAtPath(name, typeof(Material)) as Material;

            if (asset.shader.name != "EBG/Character/Uber/UberCartoonOutline")
            {
                EB.Debug.LogError("mat shader Error matname=" + asset.name);
                asset.shader = Shader.Find("EBG/Character/Uber/UberCartoonOutline");
            }
            var coolMap = asset.GetTexture("_CoolTex");
            if (coolMap == null)
            {
                EB.Debug.LogError("coolMap is Null For mat=" + asset);
            }
            asset.SetFloat("EBG_RAMP_MAP", 1);
            asset.SetFloat("_ToonThreshold", 0.73f);

            asset.SetColor("_OutlineColor", Color.black);
            asset.SetFloat("_Outline", 0.015f);
        }
    }

    [MenuItem("Custom/DeleteCharacterDumpedScript")]
    public static void DeleteCharacterDumpedScript()
    {
        string[] fileList = System.IO.Directory.GetFiles("Assets/Bundles/Player/Prefabs", "*.prefab", System.IO.SearchOption.AllDirectories);
        foreach (string name in fileList)
        {
            GameObject asset = UnityEditor.AssetDatabase.LoadAssetAtPath(name, typeof(GameObject)) as GameObject;

            GenericPoolLogic sc = asset.GetComponent<GenericPoolLogic>();
            if (sc != null)
            {
                GameObject.DestroyImmediate(sc, true);
                Debug.Log("delete script successful for asset=" + asset.name);
            }
        }
    }

    [MenuItem("Custom/SettingCharacterHealthBarPos")]
    public static void SettingCharacterHealthBarPos()
    {
        string[] fileList = System.IO.Directory.GetFiles("Assets/Bundles/Player/", "*.prefab", System.IO.SearchOption.TopDirectoryOnly);
        foreach (string name in fileList)
        {
            GameObject asset = UnityEditor.AssetDatabase.LoadAssetAtPath(name, typeof(GameObject)) as GameObject;

            string prefabName = asset.name.Split('-')[0];
            float va = PlayerPrefs.GetFloat(prefabName + ".HealthbarPos", 0);
            if (va != 0)
            {
                asset.transform.Find("HealthBarTarget").position = new Vector3(0, va, 0);
                asset.transform.Find("DamageTextTarget").position = new Vector3(0, va, 0);
            }
            else
            {
                EB.Debug.LogError("Not Found Model HealthPos For:" + prefabName);
            }
        }
    }

    [MenuItem("Custom/CrashInformationClassify")]
    public static void CrashInformationClassify()
    {
        Dictionary<string, string> stackinfos = new Dictionary<string, string>();
        string[] files = System.IO.Directory.GetFiles("E:/SVNSources/Engineering/LostTemple/客户端相关/崩溃信息/splitbugs/", "*.json", System.IO.SearchOption.TopDirectoryOnly);
        foreach (string file in files)
        {
            string fileText = File.ReadAllText(file);
            Hashtable textHash = EB.JSON.Parse(fileText) as Hashtable;
            string stack = textHash["stack"] as string;
            if (!stackinfos.ContainsKey(stack))
            {
                stackinfos.Add(stack, file);
            }
            else
            {
                string firstPath = file.Substring(0, file.LastIndexOf("/") + 1);
                string path = firstPath + Path.GetFileNameWithoutExtension(stackinfos[stack]) + "/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                File.Move(file, path + Path.GetFileName(file));
            }
        }

        foreach (var stack in stackinfos)
        {
            string file = stack.Value;
            string firstPath = stack.Value.Substring(0, file.LastIndexOf("/") + 1);
            string path = firstPath + Path.GetFileNameWithoutExtension(stack.Value) + "/";
            if (Directory.Exists(path))
            {
                File.Move(file, path + Path.GetFileName(file));
            }
        }
    }

    [MenuItem("Custom/Localizer/ExportChinaCharacterInCode")]
    public static void ExportChinaCharacterInCode()
    {
        EB.Localizer._status.Clear();
        EB.Localizer.strings.Clear();
        EB.Localizer.LoadStrings(EB.Language.ChineseSimplified, "all");
        Dictionary<string, string> textCaches = new Dictionary<string, string>();
        foreach (var kv in EB.Localizer.strings)
        {
            if (!textCaches.ContainsKey(kv.Value))
                textCaches.Add(kv.Value, kv.Key);
        }
        string outputStr = "";
        Dictionary<string, string> tempIdCache = new Dictionary<string, string>();
        string[] fileList = System.IO.Directory.GetFiles("Assets/", "*.cs", System.IO.SearchOption.AllDirectories);
        foreach (string fileName in fileList)
        {
            if (fileName.Contains("Editor"))
            {
                continue;
            }
            System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("UTF-8");
            string content = File.ReadAllText(fileName, encoding);
            string contentCopy = string.Copy(content);
            Regex rxChinaCharacter = new Regex("[\u4e00-\u9fa5]+");
            Regex rx = new Regex("\".*\"");
            MatchCollection matchs = rx.Matches(content);
            bool isMatchSuccess = false;
            if (matchs.Count != 0)
            {
                foreach (Match m in matchs)
                {
                    if (rxChinaCharacter.IsMatch(m.Value))
                    {
                        isMatchSuccess = true;
                        string matchValue = m.Value.Substring(1, m.Value.Length - 2);
                        string id;
                        if (textCaches.ContainsKey(matchValue))
                        {
                            id = textCaches[matchValue];
                            Debug.Log("use all.txt id=" + id + ",value=" + matchValue);
                        }
                        else
                        {
                            id = string.Format("ID_codefont_in_{0}_{1}", Path.GetFileNameWithoutExtension(fileName), m.Index);
                            if (!tempIdCache.ContainsKey(m.Value))
                            {
                                tempIdCache.Add(m.Value, id);
                                outputStr += string.Format("{0}|SOURCE,{1}\n", id, matchValue);
                                Debug.Log("add match value=" + m.Value + ",matchIndex=" + m.Index + ",fileName=" + fileName + ",id=" + id);
                            }
                            id = tempIdCache[m.Value];
                        }
                        Debug.Log("match.Value=" + m.Value + ",matchIndex=" + m.Index + ",fileName=" + fileName + ",id=" + id);

                        contentCopy = contentCopy.Replace(m.Value, string.Format("EB.Localizer.GetString(\"{0}\")", id));
                    }
                }
            }
            if (isMatchSuccess)
            {
                Debug.Log("matchSuccess file=" + Path.GetFileNameWithoutExtension(fileName));
                File.WriteAllText(fileName, contentCopy);
            }
        }
        Debug.Log("output=\n" + outputStr);
    }

    [MenuItem("Custom/Localizer/ExportChinaCharacterInUIPrefab")]
    public static void ExportChinaCharacterInUIPrefab()
    {
        EB.Localizer._status.Clear();
        EB.Localizer.strings.Clear();
        EB.Localizer.LoadStrings(EB.Language.ChineseSimplified, "all");
        Dictionary<string, string> textCaches = new Dictionary<string, string>();
        foreach (var kv in EB.Localizer.strings)
        {
            if (!textCaches.ContainsKey(kv.Value))
                textCaches.Add(kv.Value, kv.Key);
        }
        string outputStr = "";
        string[] fileList = System.IO.Directory.GetFiles(CatalogueConfig.UIPrefabPath, "*.prefab", System.IO.SearchOption.AllDirectories);
        foreach (string fileName in fileList)
        {
            GameObject asset = UnityEditor.AssetDatabase.LoadAssetAtPath(fileName, typeof(GameObject)) as GameObject;

            if (asset.GetComponent<UIPanel>() != null)
            {
                UIInput[] inputs = asset.GetComponentsInChildren<UIInput>(true);
                UILabel[] labels = asset.GetComponentsInChildren<UILabel>(true);
                int index = 0;
                Dictionary<string, string> tempIdCache = new Dictionary<string, string>();
                foreach (var l in labels)
                {
                    bool isInputLabel = false;
                    foreach (var ip in inputs)
                    {
                        if (ip.label == l)
                        {
                            isInputLabel = true;
                        }
                    }
                    Regex rxChinaCharacter = new Regex("[\u4e00-\u9fa5]+");
                    if (!isInputLabel && !string.IsNullOrEmpty(l.text) && rxChinaCharacter.IsMatch(l.text) && l.GetComponent<SparxUILocalize>() == null)
                    {
                        string text = l.text;
                        string id;
                        if (textCaches.ContainsKey(text))
                        {
                            id = textCaches[text];
                            Debug.Log("use all.txt id=" + id + ",value=" + text);
                        }
                        else
                        {
                            if (!tempIdCache.ContainsKey(text))
                            {
                                id = string.Format("ID_uifont_in_{0}_{1}_{2}", Path.GetFileNameWithoutExtension(fileName), l.gameObject.name, index);
                                index++;
                                tempIdCache.Add(text, id);
                                outputStr += string.Format("{0}|SOURCE,{1}\n", id, text);
                                Debug.Log("add match value=" + text + ",fileName=" + fileName + ",id=" + id);
                            }
                            id = tempIdCache[text];
                        }
                        l.gameObject.AddComponent<SparxUILocalize>().key = id;
                    }
                }
            }
        }

        Debug.Log("output=\n" + outputStr);
    }

    [MenuItem("Custom/Localizer/CheckMissedLocalizerIdInCode")]
    public static void CheckMissedLocalizerIdInCode()
    {
        EB.Localizer._status.Clear();
        EB.Localizer.strings.Clear();
        EB.Localizer.LoadStrings(EB.Language.ChineseSimplified, "all");
        string[] fileList = System.IO.Directory.GetFiles("Assets/", "*.cs", System.IO.SearchOption.AllDirectories);
        foreach (string fileName in fileList)
        {
            if (fileName.Contains("Editor"))
            {
                continue;
            }
            System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("UTF-8");
            string content = File.ReadAllText(fileName, encoding);
            Regex rx = new Regex("\".*\"");
            MatchCollection matchs = rx.Matches(content);
            if (matchs.Count != 0)
            {
                foreach (Match m in matchs)
                {
                    if (m.Value.Length >= 2)
                    {
                        string matchValue = m.Value.Substring(1, m.Value.Length - 2);
                        if (matchValue.StartsWith("ID_") && !EB.Localizer.strings.ContainsKey(matchValue))
                        {
                            EB.Debug.LogError("Missed id " + matchValue + ",fileName=" + fileName);
                        }
                    }
                }
            }
        }
    }

    [MenuItem("Custom/Localizer/Divide")]
    public static void Divide()
    {
        EB.Localizer._status.Clear();
        EB.Localizer.strings.Clear();
        EB.Localizer.LoadStrings(EB.Language.ChineseSimplified, "all");

        string ids = "";
        string values = "";

        foreach (var kv in EB.Localizer.strings)
        {
            ids += kv.Key + "\n";
            values += kv.Value + "\n";
        }
        File.WriteAllText("Assets/Resources/allId.txt", ids, System.Text.Encoding.GetEncoding("UTF-8"));
        File.WriteAllText("Assets/Resources/allValue.txt", values, System.Text.Encoding.GetEncoding("UTF-8"));
    }

    [MenuItem("Custom/Localizer/RefixUILabelInUIPrefab")]//多语言字体overflow属性修改
    public static void RefixUILabelInUIPrefab()
    {
        string[] fileList = System.IO.Directory.GetFiles(CatalogueConfig.UIPrefabPath, "*.prefab", System.IO.SearchOption.AllDirectories);
        foreach (string fileName in fileList)
        {
            GameObject cheakAsset = UnityEditor.AssetDatabase.LoadAssetAtPath(fileName, typeof(GameObject)) as GameObject;
            if (cheakAsset == null || cheakAsset.GetComponent<UIPanel>() == null)
            {
                continue;
            }
            UnityEngine.Object pre = UnityEditor.AssetDatabase.LoadAssetAtPath(fileName, typeof(UnityEngine.Object)) as UnityEngine.Object;
            GameObject asset = UnityEditor.PrefabUtility.InstantiatePrefab(pre) as GameObject;//UnityEditor.AssetDatabase.LoadAssetAtPath(fileName, typeof(GameObject)) as GameObject;
            if (asset == null)
            {
                EB.Debug.LogError(string.Format("Can't Find {0}" + fileName));
                continue;
            }
            int count = 0;
            int allcount = 0;
            int bgcount = 0;
            if (asset.GetComponent<UIPanel>() != null)
            {
                UIInput[] inputs = asset.GetComponentsInChildren<UIInput>(true);
                UILabel[] labels = asset.GetComponentsInChildren<UILabel>(true);
                if (labels != null) allcount = labels.Length;
                foreach (var label in labels)
                {
                    bool isInputLabel = false;
                    foreach (var ip in inputs)
                    {
                        if (ip.label == label)
                        {
                            isInputLabel = true;
                        }
                    }
                    if (isInputLabel || label.overflowMethod != UILabel.Overflow.ResizeFreely) continue;
                    if (label.transform.parent != null && label.transform.parent.childCount > 2 || label.transform.childCount > 2)
                    {
                        EB.Debug.LogError(string.Format("不可修改预设{0}中{1}的Label，因为同节点下物体或子物体的数量大于2，请自行设置", asset.name, label.name));
                        continue;
                    }
                    if (label.transform.parent.GetComponent<UISprite>() != null)
                    {
                        label.overflowMethod = UILabel.Overflow.ShrinkContent;
                        label.width = (label.width > label.transform.parent.GetComponent<UIWidget>().width - 10) ? label.width + 10 : label.transform.parent.GetComponent<UIWidget>().width - 10;
                        label.height += 10;
                        count++;
                    }
                    else if (label.transform.parent.GetComponent<UILabel>() != null && label.text == label.transform.parent.GetComponent<UILabel>().text)
                    {
                        label.overflowMethod = label.transform.parent.GetComponent<UILabel>().overflowMethod;
                        label.width = label.transform.parent.GetComponent<UIWidget>().width;
                        label.height = label.transform.parent.GetComponent<UIWidget>().height;
                        bgcount++;
                        count++;
                    }
                }
            }
            UnityEditor.PrefabUtility.ReplacePrefab(asset, pre);
            Debug.Log(string.Format("修改预设{0}中{1}个Label（其中的{2}个Label为底影），该预设一共{3}个Label", asset.name, count, bgcount, allcount));
        }
        AssetDatabase.SaveAssets();
    }


    public static List<string> targetNameList = new List<string>() { "Ty_Arena_Icon_Shengli", "Ty_Arena_Icon_Shibai", "Ty_Arena_Icon_Tiaozhan", "Ty_Country_Shibai", "Ty_Icon_Tuijian", "Ty_Icon_Weihuode", "Ty_Icon_Weikaiqi", "Ty_Icon_Yihuode", "Ty_Icon_Yikaiqi", "Ty_Icon_Yilingqu", "Ty_Icon_Yishangzhen", "Ty_Icon_Yizhenwang", "Ty_Icon_Yizhuangbei", "Ty_Legion_Shengli", "Ty_Legion_Taotai", "Ty_Legion_Wanmei" };
    [MenuItem("Custom/Localizer/CheakSomeThingInPrefabs")]//检查筛选Prefabs里的组件
    //  /UI/Prefabs/——UI目录
    //  /Environment/InventoryView/Prefabs/——其他目录
    public static void CheakTipInUIPrefab()
    {
        targetNameList = new List<string>();
        string[] fileList = System.IO.Directory.GetFiles("Assets/_GameAssets/Res/Environment/InventoryView/Prefabs/", "*.prefab", System.IO.SearchOption.AllDirectories);
        foreach (string fileName in fileList)
        {
            GameObject asset = UnityEditor.AssetDatabase.LoadAssetAtPath(fileName, typeof(GameObject)) as GameObject;
            if (asset.GetComponent<UIPanel>() != null)
            {
                UISprite[] targets = asset.GetComponentsInChildren<UISprite>(true);
                foreach (var target in targets)
                {
                    if (target.atlas!=null&&target.atlas.name == "LTGeneral_Atlas")
                    {
                        for (int i=0;i< targetNameList.Count;i++)
                        {
                            if (target.spriteName == targetNameList[i])
                            {
                                EB.Debug.LogError(string.Format("预设名 = {0};物体名 = {1}", asset.gameObject.name, target.name));
                                break;
                            }
                        }
                        
                    }
                }
            }
        }
        Debug.Log("Finish!!");
    }
}
