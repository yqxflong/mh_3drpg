using System;
using UnityEngine;
using UnityEditor;

public class ResCheckMenuItemEntry
{
    static UnityConsoleResCheckReporter resCheckResultReporter
    {
        get
        {
            return new UnityConsoleResCheckReporter();
        }
    }

    [MenuItem("GameTools/资源扫描/角色源文件检查")]
    public static void CharatersChecker()
    {
        ResCheckerMgr.RunChecker<CharatersChecker>(resCheckResultReporter);
    }

    [MenuItem("GameTools/资源扫描/角色预置检查")]
    public static void CharacterPrefabChecker()
    {
        ResCheckerMgr.RunChecker<CharacterPrefabChecker>(resCheckResultReporter);
    }

    [MenuItem("GameTools/资源扫描/ui整体检查")]
    public static void GUIChecker()
    {
        ResCheckerMgr.RunChecker<GUIChecker>(new GUICheckerErrorOutResCheckReporter());
    }

    [MenuItem("GameTools/资源扫描/prefab脚本丢失检查")]
    public static void PrefabsMissChecker()
    {
        ResCheckerMgr.RunChecker<PrefabMissChecker>(new PrefabsMissCheckerErrorOutResCheckReporter());
    }

    [MenuItem("GameTools/资源扫描/prefab的drawcall截断检查")]
    public static void PrefabsDcCutChecker()
    {
        ResCheckerMgr.RunChecker<PrefabsDcCutChecker>(resCheckResultReporter);
    }

    [MenuItem("GameTools/资源扫描/Resources下面是否有不应该有的对象")]
    public static void ResourcesChecker()
    {
        ResCheckerMgr.RunChecker<ResourcesChecker>(resCheckResultReporter);
    }

    [MenuItem("GameTools/资源扫描/Prefab是否有丢失材质")]
    public static void PrefabMissMatChecker()
    {
        ResCheckerMgr.RunChecker<PrefabMissMatChecker>(new PrefabMissMatCheckerErrorOutResCheckReporter());
    }

    [MenuItem("GameTools/资源扫描/UIPrefab检查")]
    public static void UIPrefabChecker()
    {
        ResCheckerMgr.RunChecker<UIPrefabChecker>(resCheckResultReporter);
    }
}
