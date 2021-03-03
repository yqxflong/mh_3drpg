using System;
using UnityEngine;
using UnityEditor;
using EB.IO;
using System.IO;
using System.Collections.Generic;
using EB.Sparx;
using System.Text.RegularExpressions;
using Hotfix_LT.Combat;

public class MountScripts
{
    static string resPath = "Assets/Bundles/Player";
    static string tarGuid = "9e84b2c726cfb4e868ab81767be1097b";

    //[MenuItem("GameTools/脚本挂载")]
    //public static void CharatersChecker()
    //{
    //    string[] paths = Directory.GetFiles(resPath, "*.prefab", SearchOption.AllDirectories);

    //    foreach(string p in paths)
    //    {
    //        if (!p.Contains("-Campaign") && !p.Contains("-Lobby") && !p.Contains("Variants"))
    //        {
    //            //if (!p.Contains("M103"))
    //            //{
    //            //    continue;
    //            //}

    //            GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(p);
    //            DynamicMonoILR ilrObj = gameObject.AddComponent<DynamicMonoILR>();
    //            ilrObj.hotfixClassPath = "Hotfix_LT.Combat.Combatant";
    //            CapsuleCollider collider = gameObject.GetComponent<CapsuleCollider>();
    //            ilrObj.Vector3ParamList = new List<Vector3>
    //            {
    //                new Vector3(0, 0, collider.radius)
    //            };

    //            DynamicMonoILR ilrObj2 = gameObject.AddComponent<DynamicMonoILR>();
    //            ilrObj2.hotfixClassPath = "Hotfix_LT.Combat.HealthBar2D";

    //            EditorUtility.SetDirty(gameObject);
    //            AssetDatabase.SaveAssets();

    //            DeleteNullScript(p);
    //        }
    //    }
    //    AssetDatabase.Refresh();
    //}

    //private static void DeleteNullScript(string path)
    //{
    //    bool isNull = false;
    //    string s = File.ReadAllText(path);

    //    Regex regBlock = new Regex("MonoBehaviour");

    //    string[] strArray = s.Split(new string[] { "---" }, StringSplitOptions.RemoveEmptyEntries);
    //    for (int i = 0; i < strArray.Length; i++)
    //    {
    //        string blockStr = strArray[i];
    //        if (regBlock.IsMatch(blockStr))
    //        {
    //            Match guidMatch = Regex.Match(blockStr, "m_Script: {fileID: (.*), guid: (?<GuidValue>.*?), type:");
    //            if (guidMatch.Success)
    //            {
    //                string guid = guidMatch.Groups["GuidValue"].Value;
    //                if (string.Equals(guid, tarGuid))
    //                {
    //                    isNull = true;
    //                    s = s.Replace("---" + blockStr, "");

    //                    Match idMatch = Regex.Match(blockStr, "!u!(.*) &(?<idValue>.*?)\n");
    //                    if (idMatch.Success)
    //                    {
    //                        string id = idMatch.Groups["idValue"].Value;
    //                        //Regex quote = new Regex(" - component: {fileID: " + id + "}");
    //                        Regex quote = new Regex("  - (.*): {fileID: " + id + "}\n");
    //                        s = quote.Replace(s, "");
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    if (isNull)
    //    {
    //        File.WriteAllText(path, s);
    //    }
    //}

    //[MenuItem("GameTools/脚本挂载2")]
    //public static void CharatersChecker2()
    //{
    //    string[] paths = Directory.GetFiles(resPath, "*.prefab", SearchOption.AllDirectories);
    //    foreach (string p in paths)
    //    {
    //        if (!p.Contains("-Campaign") && !p.Contains("-Lobby") && !p.Contains("Variants"))
    //        {
    //            //if (!p.Contains("M103"))
    //            //{
    //            //    continue;
    //            //}
    //            string s = File.ReadAllText(p);
    //            Regex quote = new Regex("\n\n");
    //            s = quote.Replace(s, "\n");

    //            File.WriteAllText(p, s);
    //        }
    //    }
    //}
    //[MenuItem("GameTools/删除空脚本")]
    //public static void DeleteNull()
    //{
    //    string resPath = "Assets/Bundles/Player/Prefabs";

    //    string[] paths = Directory.GetFiles(resPath, "*.prefab", SearchOption.AllDirectories);
    //    foreach (string p in paths)
    //    {
    //        DeleteNullScript(p);
    //    }
    //    AssetDatabase.Refresh();
    //}

    [MenuItem("GameTools/脚本挂载2")]
    public static void CharatersCheckerEx()
    {
        string[] paths = Directory.GetFiles(resPath, "*.prefab", SearchOption.AllDirectories);

        foreach (string p in paths)
        {
            if (!p.Contains("-Campaign") && !p.Contains("-Lobby") && !p.Contains("Variants"))
            {
                //if (!p.Contains("M001"))
                //{
                //    continue;
                //}
                GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(p);
                var dynamics = gameObject.GetComponents<DynamicMonoILR>();
                int len = dynamics.Length;
                bool dirty = false;
                for (int i = len - 1; i >= 0; i--)
                {
                    if (string.Equals(dynamics[i].hotfixClassPath, "Hotfix_LT.Combat.Combatant"))
                    {
                        dirty = true;
                        Combatant combatant = gameObject.AddComponent<Combatant>();

                        Keyframe[] keys = new Keyframe[2];
                        keys[0].time = 0;
                        keys[0].value = 0;
                        keys[0].inTangent = 2;
                        keys[0].outTangent = 2;

                        keys[1].time = 1;
                        keys[1].value = 1;
                        keys[1].inTangent = 0;
                        keys[1].outTangent = 0;

                        combatant.myRotationSCurve = new AnimationCurve(keys);

                        combatant.hitOffset = dynamics[i].Vector3ParamList[0];
                        combatant.DamageTextTarget = gameObject.transform.Find("DamageTextTarget");

                        UnityEngine.Object.DestroyImmediate(dynamics[i], true);
                    }
                    else if (string.Equals(dynamics[i].hotfixClassPath, "Hotfix_LT.Combat.HealthBar2D"))
                    {
                        UnityEngine.Object.DestroyImmediate(dynamics[i], true);
                    }
                }

                if (dirty)
                {
                    EditorUtility.SetDirty(gameObject);
                    AssetDatabase.SaveAssets();
                }
            }
        }
        AssetDatabase.Refresh();
    }
}
