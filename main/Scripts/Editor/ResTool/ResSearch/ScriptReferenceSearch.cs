using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ScriptReferenceSearch : EditorWindow
{
    public static string constPath = "Assets\\_GameAssets\\Res\\Prefabs\\UIPrefabs";
    [MenuItem("GameTools/资源查找/查找Script对应引用的prefab")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ScriptReferenceSearch));
    }

    private string _scriptName = "";

    void OnGUI()
    {
        DrawFilterFolders();

        if (GUILayout.Button("Search"))
        {
            if (string.IsNullOrEmpty(_scriptName))
            {
                EditorUtility.DisplayDialog("请输入一个有效的脚本名称", "提示", "确定");
                return;
            }

            Type type = _currentType;

            if (_currentType.Equals(typeof(MonoBehaviour)))
            {
                System.Reflection.Assembly assembly = HierarchyUtils.GetAssembly();
                type = assembly.GetType(_scriptName);

                if (type == null)
                {
                    EditorUtility.DisplayDialog("没找到该类：" + _scriptName, "提示", "确定");
                    return;
                }
            } 

            List<string> prefabNames = FindScriptInPrefab(type);
            ShowPrefabNameEditor.ShowWindow(prefabNames);
        }
    }

    private List<string> _searchInFolder = new List<string>(){constPath};
    private Vector2 _filterScrollPos;
    private int _selectedIndex;
    private Type _currentType;
    private Type[] _types = new Type[] 
    { 
        typeof(MonoBehaviour),
        typeof(DynamicMonoILR),
        typeof(UIControllerILR),
        typeof(DataLookupILR)
    };

    private void DrawFilterScriptType()
    {
        var options = new string[_types.Length];

        for (var i = 0; i < _types.Length; i++)
        {
            options[i] = _types[i].Name;
        }

        _selectedIndex = EditorGUILayout.Popup("Script Type:", _selectedIndex, options);
        _currentType = _types[_selectedIndex];
    }

    private void DrawFilterFolders()
    {
        DrawFilterScriptType();

        GUILayout.BeginHorizontal();
        var lab = _currentType.Equals(typeof(MonoBehaviour)) ? "Script Name:" : "Hotfix Class Path:";
        _scriptName = EditorGUILayout.TextField(lab, _scriptName);

        if (GUILayout.Button("Add Filter Folder", GUILayout.Width(120)))
        {
            var path = EditorUtility.OpenFolderPanel("Select Filter Folder", "Assets", "");
            path = path.Replace(Application.dataPath, "Assets");

            if (!string.IsNullOrEmpty(path) && !_searchInFolder.Contains(path))
            {
                _searchInFolder.Add(path);
            }
        }
        GUILayout.EndHorizontal();

        if (_searchInFolder.Count > 0)
        {
            _filterScrollPos = GUILayout.BeginScrollView(_filterScrollPos, EditorStyles.helpBox);

            for (var i = 0; i < _searchInFolder.Count; i++)
            {
                GUILayout.BeginHorizontal(EditorStyles.textField, GUILayout.MinHeight(20f));
                var path = _searchInFolder[i];

                if (GUILayout.Button(path, EditorStyles.label))
                {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path));
                }

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _searchInFolder.RemoveAt(i);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }
    }

    private List<string> FindScriptInPrefab(Type scriptType)
    {
        List<string> prefabNames = new List<string>();
        string[] guids = AssetDatabase.FindAssets("t:prefab", _searchInFolder.ToArray());

        foreach (string guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

            if (go != null)
            {
                var comps = go.GetComponentsInChildren(scriptType, true);

                if (comps == null || comps.Length == 0)
                {
                    continue;
                }

                List<Component> list = new List<Component>();

                if (scriptType.Equals(typeof(DynamicMonoILR)))
                {
                    for (var i = 0; i < comps.Length; i++)
                    {
                        var com = comps[i] as DynamicMonoILR;

                        if (com != null && com.hotfixClassPath.Equals(_scriptName))
                        {
                            list.Add(com);
                        }
                    }
                } 
                else if (scriptType.Equals(typeof(UIControllerILR)))
                {
                    for (var i = 0; i < comps.Length; i++)
                    {
                        var com = comps[i] as UIControllerILR;

                        if (com != null && com.hotfixClassPath.Equals(_scriptName))
                        {
                            list.Add(com);
                        }
                    }
                }
                else if (scriptType.Equals(typeof(DataLookupILR)))
                {
                    for (var i = 0; i < comps.Length; i++)
                    {
                        var com = comps[i] as DataLookupILR;

                        if (com != null && com.hotfixClassPath.Equals(_scriptName))
                        {
                            list.Add(com);
                        }
                    }
                }
                else
                {
                    list.AddRange(comps);
                }

                if (list != null && list.Count > 0)
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        prefabNames.Add(path + "_" + list[i].name);
                    }
                }
            }
        }

        return prefabNames;
    }
}

public class ShowPrefabNameEditor : EditorWindow
{
    private static List<string> PrefabNames = new List<string>();

    public static void ShowWindow(List<string> prefabNames)
    {
        PrefabNames = prefabNames;
        ShowPrefabNameEditor window = (ShowPrefabNameEditor)GetWindow(typeof(ShowPrefabNameEditor), true, "PrefabName");
        window.Show();
    }

    private Vector2 _scrollPos;
    void OnGUI()
    {
        StringBuilder strBuilder = new StringBuilder();
        int count = PrefabNames.Count;

        for (int i = 0; i < count; i++)
        {
            if (i == 0)
            {
                strBuilder.Append(PrefabNames[i]);
            }
            else
            {
                strBuilder.Append("\n");
                strBuilder.Append(PrefabNames[i]);
            }
        }

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        EditorGUILayout.TextArea(strBuilder.ToString());
        EditorGUILayout.EndScrollView();
    }
}
