using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class MissingReferenceEditor : EditorWindow
{
    private static MissingReferenceEditorModel _model;
    private static readonly string _modelPath = "Assets/_GameAssets/Scripts/Editor/Tools/MissingReferenceEditorModel.asset";

    private static GUIStyle _foldoutStyle;
    private static string _dataPath;
    private bool _showFilterSetting;
    private bool _showContainSetting;
    private float _indent = 20f;

    private static List<Object> _selects = new List<Object>();
    private static List<string> _searchPrefabPaths = new List<string>();
    private static Regex _regex = new Regex("(Assets){1}");
    private static int _searchPrefabsCount = 0;
    private static bool _hasError = false;

    [MenuItem("Assets/Tools/Missing Reference/Search")]
    private static void FindMissingReferenceFromSelect()
    {
        Init();
        FindPrefabFromSelect();
    }

    [MenuItem("Tools/Missing Reference/Search")]
    public static bool FindMissingReferenceFromAssets()
    {
        Init();
        FindPrefabFromAssets();
        return _hasError;
    }

    [MenuItem("Tools/Missing Reference/Setting")]
    private static void MissingReferenceSetting()
    {
        var window = GetWindow<MissingReferenceEditor>("Missing Reference Setting");
        Init();
        window.Show();
    }

    private static void Init()
    {
        _foldoutStyle = EditorStyles.foldout;

        if (_foldoutStyle != null)
        {
            _foldoutStyle.fontStyle = FontStyle.Bold;
        }

        _dataPath = Application.dataPath;
        _model = AssetDatabase.LoadAssetAtPath<MissingReferenceEditorModel>(_modelPath);

        if (_model == null)
        {
            Debug.LogError("MissingReferenceEditorModel.asset not found!");
            return;
        }

        if (_model.filterList == null)
        {
            _model.filterList = new List<string>();
        }
    }

    private static void Clear()
    {
        _hasError = false;
        _searchPrefabsCount = 0;
        _searchPrefabPaths.Clear();
        _selects.Clear();
    }

    public static void FindPrefabFromAssets()
    {
        Clear();

        var guids = AssetDatabase.FindAssets("t:prefab");
        var len = guids.Length;
        string path;

        for (var i = 0; i < len; i++)
        {
            path = AssetDatabase.GUIDToAssetPath(guids[i]);

            if (!InFilterList(path) && InContainList(path))
            {
                _searchPrefabPaths.Add(path);
            }
        }

        _searchPrefabsCount = _searchPrefabPaths.Count;

        if (_searchPrefabsCount <= 0)
        {
            Debug.LogError("Error: there is no '.prefab' file in your selected files or folders!");
            return;
        }

        Execute();
    }

    public static void FindPrefabFromSelect()
    {
        Clear();
        AddGameObjectToSelects(Selection.objects);
        AddGameObjectToSelects(Selection.activeGameObject);

        if (!GetFilesBySelect())
        {
            Debug.LogError("Error: please select file or folder at first!");
            return;
        }

        _searchPrefabsCount = _searchPrefabPaths.Count;

        if (_searchPrefabsCount <= 0)
        {
            Debug.LogError("Error: there is no '.prefab' file in your selected files or folders!");
            return;
        }

        Execute();
    }

    private static void Execute()
    {
        for (var i = 0; i < _searchPrefabsCount; i++)
        {
            SearchMissing(i);

            if (i + 1 >= _searchPrefabsCount)
            {
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }
        }
    }

    private static void AddGameObjectToSelects(Object obj)
    {
        if (obj == null)
        {
            return;
        }

        if (_selects.IndexOf(obj) >= 0)
        {
            return;
        }

        _selects.Add(obj);
    }

    private static void AddGameObjectToSelects(Object[] objs)
    {
        if (objs == null)
        {
            return;
        }

        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i] == null)
            {
                continue;
            }

            AddGameObjectToSelects(objs[i]);
        }
    }

    private static bool GetFilesBySelect()
    {
        if (_selects.Count <= 0)
        {
            return false;
        }

        int length = _selects.Count;

        for (int i = 0; i < length; i++)
        {
            Object obj = _selects[i] as Object;
            string path = AssetDatabase.GetAssetPath(obj);

            if (path.IndexOf(".meta") >= 0)
            {
                continue;
            }

            path = Application.dataPath + _regex.Replace(path, "", 1, 0);

            if (Directory.Exists(path))
            {
                GetFilesByDir(Directory.CreateDirectory(path));
                continue;
            }

            AddFileToArray(path);
        }

        return true;
    }

    private static void GetFilesByDir(DirectoryInfo di)
    {
        FileInfo[] allFile = di.GetFiles();
        DirectoryInfo[] allDir = di.GetDirectories();
        int fileCount = allFile.Length;
        int dirCount = allDir.Length;

        for (int i = 0; i < fileCount; i++)
        {
            FileInfo fi = allFile[i];
            AddFileToArray(fi.DirectoryName + "/" + fi.Name);
        }

        for (int j = 0; j < dirCount; j++)
        {
            GetFilesByDir(allDir[j]);
        }
    }

    private static void AddFileToArray(string path)
    {
        if (path.IndexOf(".meta") >= 0)
        {
            return;
        }

        if (path.IndexOf(".prefab") >= 0)
        {
            path = path.Replace('\\', '/');
            string savePath = "Assets" + path.Replace(Application.dataPath, "");

            if (_searchPrefabPaths.IndexOf(savePath) == -1)
            {
                _searchPrefabPaths.Add(savePath);
            }
        }
    }

    private static void ShowError(string path, string objectName, string propertyName)
    {
        Debug.LogErrorFormat("【{0}】Missing reference:【Path】{1},【Property】{2}", path, objectName, propertyName);
    }

    private static string FullObjectPath(GameObject go)
    {
        return go.transform.parent == null ? go.name : FullObjectPath(go.transform.parent.gameObject) + "/" + go.name;
    }

    private static void SearchMissing(int index)
    {
        if (index >= _searchPrefabPaths.Count)
        {
            return;
        }

        //Debug.LogFormat("INFO【Path】{0}", _searchPrefabPaths[index]);

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_searchPrefabPaths[index]);
        var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        GameObject[] sceneObjs = GameObject.FindObjectsOfType<GameObject>();
        int sceneObjsLength = sceneObjs.Length;

        for (int i = 0; i < sceneObjsLength; i++)
        {
            var comps = sceneObjs[i].GetComponents<Component>();

            foreach (var c in comps)
            {
                if (c == null)
                {
                    Debug.LogErrorFormat("【{0}】Missing script:【Path】{1}", _searchPrefabPaths[index], FullObjectPath(sceneObjs[i]));
                    _hasError = true;
                    continue;
                }

                SerializedObject serializedObject = new SerializedObject(c);
                SerializedProperty serializedProperty = serializedObject.GetIterator();

                while (serializedProperty.NextVisible(true))
                {
                    if (serializedProperty.propertyType != SerializedPropertyType.ObjectReference)
                    {
                        continue;
                    }

                    if (serializedProperty.objectReferenceValue == null && serializedProperty.objectReferenceInstanceIDValue != 0)
                    {
                        ShowError(_searchPrefabPaths[index], FullObjectPath(sceneObjs[i]), serializedProperty.name);
                        _hasError = true;
                    }
                }
            }
        }

        DestroyImmediate(instance);
        EditorUtility.DisplayProgressBar("Seaching...", _searchPrefabPaths[index], (float)(index) / _searchPrefabsCount);
    }

    private void OnGUI()
    {
        _showFilterSetting = EditorGUILayout.Foldout(_showFilterSetting, "过滤目录【不检查这些目录中的内容】", true, _foldoutStyle);

        if (_showFilterSetting)
        {
            BeginContent();
            DrawFilterList();
            EndContent();
        }

        _showContainSetting = EditorGUILayout.Foldout(_showContainSetting, "包含目录【只检查这些目录中的内容，不填表示整个Assets目录】（如果内容也存在过滤目录中，则内容不会被检查）", true, _foldoutStyle);

        if (_showContainSetting)
        {
            BeginContent();
            DrawContainList();
            EndContent();
        }
    }

    private void OnDestroy()
    {
        EditorUtility.SetDirty(_model);
    }

    private void BeginContent()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(_indent);
        GUILayout.BeginVertical();
    }

    private void EndContent()
    {
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    #region Filter Setting
    private string _lastFilterPath = "Assets";
    private Vector2 _filterScrollPos;

    private void DrawFilterList()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Folder", GUILayout.Width(100)))
        {
            var path = EditorUtility.OpenFolderPanel("Select Filter Folder", _lastFilterPath, "");
            path = path.Replace(_dataPath, "Assets");

            if (!string.IsNullOrEmpty(path) && !_model.filterList.Contains(path))
            {
                _model.filterList.Add(path);
            }

            _lastFilterPath = path;
        }

        if (GUILayout.Button("Add File", GUILayout.Width(100)))
        {
            var path = EditorUtility.OpenFilePanel("Select Filter File", _lastFilterPath, "");
            path = path.Replace(_dataPath, "Assets");

            if (!string.IsNullOrEmpty(path) && !_model.filterList.Contains(path))
            {
                _model.filterList.Add(path);
            }

            _lastFilterPath = path;
        }

        GUILayout.EndHorizontal();

        if (_model.filterList.Count > 0)
        {
            _filterScrollPos = GUILayout.BeginScrollView(_filterScrollPos, EditorStyles.helpBox);

            for (var i = 0; i < _model.filterList.Count; i++)
            {
                GUILayout.BeginHorizontal(EditorStyles.textField, GUILayout.MinHeight(20f));
                var path = _model.filterList[i];

                if (GUILayout.Button(path, EditorStyles.label))
                {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(path));
                }

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _model.filterList.RemoveAt(i);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }
    }

    private static bool InFilterList(string path)
    {
        for (var i = 0; i < _model.filterList.Count; i++)
        {
            if (path.StartsWith(_model.filterList[i]))
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region Contain Setting
    private string _lastContainPath = "Assets";
    private Vector2 _containScrollPos;

    private void DrawContainList()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Folder", GUILayout.Width(100)))
        {
            var path = EditorUtility.OpenFolderPanel("Select Contain Folder", _lastContainPath, "");
            path = path.Replace(_dataPath, "Assets");

            if (!string.IsNullOrEmpty(path) && !_model.containList.Contains(path))
            {
                _model.containList.Add(path);
            }

            _lastContainPath = path;
        }

        if (GUILayout.Button("Add File", GUILayout.Width(100)))
        {
            var path = EditorUtility.OpenFilePanel("Select Contain File", _lastContainPath, "");
            path = path.Replace(_dataPath, "Assets");

            if (!string.IsNullOrEmpty(path) && !_model.containList.Contains(path))
            {
                _model.containList.Add(path);
            }

            _lastContainPath = path;
        }

        GUILayout.EndHorizontal();

        if (_model.containList.Count > 0)
        {
            _containScrollPos = GUILayout.BeginScrollView(_containScrollPos, EditorStyles.helpBox);

            for (var i = 0; i < _model.containList.Count; i++)
            {
                GUILayout.BeginHorizontal(EditorStyles.textField, GUILayout.MinHeight(20f));
                var path = _model.containList[i];

                if (GUILayout.Button(path, EditorStyles.label))
                {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(path));
                }

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _model.containList.RemoveAt(i);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }
    }

    private static bool InContainList(string path)
    {
        if (_model.containList == null || _model.containList.Count < 1)
        {
            // 列表为空表示整个Assets目录
            return true;
        }

        for (var i = 0; i < _model.containList.Count; i++)
        {
            if (path.StartsWith(_model.containList[i]))
            {
                return true;
            }
        }

        return false;
    }
    #endregion
}