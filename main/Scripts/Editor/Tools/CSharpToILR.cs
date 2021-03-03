using PixelCrushers.DialogueSystem.Aurora;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public class CSharpToILR : EditorWindow
{
    private static string _hotfixProjectPath;

    private MonoBehaviour _target;
    private string _scriptName = "";
    private int _selectedIndex;
    private int _lastSelectedIndex;

    private string ScriptName
    {
        get
        {
            return string.IsNullOrEmpty(_scriptName) ? _target.GetType().Name : _scriptName;
        }
        set
        {
            if (_scriptName != value)
            {
                _scriptName = value;
            }
        }
    }

    [MenuItem("ILRuntime/C# to ILR", priority = 2000)]
    private static void Open()
    {
        _hotfixProjectPath = Path.Combine(Application.dataPath, "_HotfixScripts/UI/");

        var window = GetWindow<CSharpToILR>();
        window.titleContent = new GUIContent("C# to ILR");
        window.minSize = new Vector2(300, 200);
        window.Show();
    }

    private void OnGUI()
    {
        _target = EditorGUILayout.ObjectField("目标对象", _target, typeof(MonoBehaviour), true) as MonoBehaviour;

        if (_target == null)
        {
            return;
        }

        RefreshTarget();
        ScriptName = EditorGUILayout.TextField("热更脚本名称", ScriptName);
        EditorGUILayout.Space();

        if (GUILayout.Button("生成热更代码\n(Assets/_HotfixScripts/UI)"))
        {
            Create();
        }
    }

    private void RefreshTarget()
    {
        var monos = _target.GetComponents<MonoBehaviour>();
        var options = new string[monos.Length];

        for (var i = 0; i < monos.Length; i++)
        {
            options[i] = monos[i].GetType().Name;
        }

        _selectedIndex = EditorGUILayout.Popup("选择目标脚本", _selectedIndex, options);
        _target = monos[_selectedIndex];

        if (_selectedIndex != _lastSelectedIndex)
        {
            ScriptName = options[_selectedIndex];
            _lastSelectedIndex = _selectedIndex;
        }
    }

    private void Create()
    {
        var filter = string.Format("{0} t:Script", _target.GetType().Name);
        var assetPaths = AssetDatabase.FindAssets(filter);
        var targetPath = "";

        if (assetPaths != null && assetPaths.Length > 0)
        {
            for (var i = 0; i < assetPaths.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(assetPaths[i]);
                var fileName = Path.GetFileNameWithoutExtension(path);

                // 在非热更目录中查找
                if (!path.StartsWith("Assets/_HotfixScripts") && fileName.Equals(_target.GetType().Name))
                {
                    targetPath = path;
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(targetPath))
        {
            ShowNotification(new GUIContent("找不到该脚本"));
            return;
        }

        var readPath = Application.dataPath.Replace("Assets", targetPath);
        var lines = new List<string>(File.ReadAllLines(readPath));

        /// 缩进
        for (var i = 0; i < lines.Count; i++)
        {
            if (!lines[i].Contains("using"))
            {
                lines[i] = "    " + lines[i];
            }
        }

        /// 插入命名空间
        for (var i = 0; i < lines.Count; i++)
        {
            if (lines[i].Contains("public"))
            {
                lines.Insert(i, "namespace Hotfix_LT.UI\n{");
                break;
            }
        }

        /// 插入生成代码
        for (var i = 0; i < lines.Count; i++)
        {
            if (lines[i].Contains("class") && lines[i].Contains(_target.GetType().Name))
            {
                var strs = lines[i].Split(':');
                var baseClassName = GetBaseClassName();

                if (strs != null && strs.Length > 1)
                {
                    var s = strs[1].Replace("{", "").Trim();

                    if (s != "MonoBehaviour" && s != "UIController" && s != "DataLookup")
                    {
                        baseClassName = s;
                    }
                }

                var sb = new StringBuilder();
                sb.AppendFormat("    public class {0} : {1}", ScriptName, baseClassName);
                sb.AppendLine("\n    {");
                sb.AppendLine(GenerateScript().ToString());
                lines[i] = sb.ToString();
                break;
            }
        }

        /// 关闭命名空间
        lines.Add("}");

        var writePath = string.Format("{0}/{1}.cs", _hotfixProjectPath, ScriptName);

        if (File.Exists(writePath))
        {
            File.Delete(writePath);
        }

        File.WriteAllLines(writePath, lines);
        AssetDatabase.Refresh();
        ShowNotification(new GUIContent("创建成功"));
    }

    StringBuilder GenerateScript()
    {
        var sb = new StringBuilder();
        sb.AppendLine("        public override void Awake()\n        {");
        sb.AppendLine("            base.Awake();");
        sb.AppendLine();

        HandleField(sb);
        HandleUIButton(sb);
        HandleConsecutiveClickCoolTrigger(sb);
        HandleUIEventTrigger(sb);
        HandleUITweener(sb);
        HandleContinuePressTrigger(sb);
        HandleContinueClickCDTrigger(sb);
        HandleUIServerRequest(sb);
        HandleUIToggle(sb);
        HandlePressOrClick(sb);

        sb.AppendLine("        }");
        return sb;
    }

    private void HandlePressOrClick(StringBuilder sb)
    {
        var actions = _target.gameObject.GetComponentsInChildren<PressOrClick>(true);

        if (actions != null && actions.Length > 0)
        {
            sb.AppendLine();

            for (var i = 0; i < actions.Length; i++)
            {
                var act = actions[i];

                if (act.m_CallBackClick != null && act.m_CallBackClick.Count > 0)
                {
                    for (var j = 0; j < act.m_CallBackClick.Count; j++)
                    {
                        HandleEvent(act, "m_CallBackClick", act.m_CallBackClick[j], sb);
                    }
                }
                if (act.m_CallBackPress != null && act.m_CallBackPress.Count > 0)
                {
                    for (var j = 0; j < act.m_CallBackPress.Count; j++)
                    {
                        HandleEvent(act, "m_CallBackPress", act.m_CallBackPress[j], sb);
                    }
                }
                if (act.m_CallBackRelease != null && act.m_CallBackRelease.Count > 0)
                {
                    for (var j = 0; j < act.m_CallBackRelease.Count; j++)
                    {
                        HandleEvent(act, "m_CallBackRelease", act.m_CallBackRelease[j], sb);
                    }
                }
            }
        }
    }

    private void HandleUIServerRequest(StringBuilder sb)
    {
        var actions = _target.gameObject.GetComponentsInChildren<UIServerRequest>(true);

        if (actions != null && actions.Length > 0)
        {
            sb.AppendLine();

            for (var i = 0; i < actions.Length; i++)
            {
                var act = actions[i];

                if (act.onSendRequest != null && act.onSendRequest.Count > 0)
                {
                    for (var j = 0; j < act.onSendRequest.Count; j++)
                    {
                        HandleEvent(act, "onSendRequest", act.onSendRequest[j], sb);
                    }
                }
                if (act.onResponse != null && act.onResponse.Count > 0)
                {
                    for (var j = 0; j < act.onResponse.Count; j++)
                    {
                        HandleEvent(act, "onResponse", act.onResponse[j], sb);
                    }
                }
            }
        }
    }

    private void HandleContinueClickCDTrigger(StringBuilder sb)
    {
        var actions = _target.gameObject.GetComponentsInChildren<ContinueClickCDTrigger>(true);

        if (actions != null && actions.Length > 0)
        {
            sb.AppendLine();

            for (var i = 0; i < actions.Length; i++)
            {
                var act = actions[i];

                if (act.m_CallBackPress != null && act.m_CallBackPress.Count > 0)
                {
                    for (var j = 0; j < act.m_CallBackPress.Count; j++)
                    {
                        HandleEvent(act, "m_CallBackPress", act.m_CallBackPress[j], sb);
                    }
                }
                if (act.CD_CallBackPress != null && act.CD_CallBackPress.Count > 0)
                {
                    for (var j = 0; j < act.CD_CallBackPress.Count; j++)
                    {
                        HandleEvent(act, "CD_CallBackPress", act.CD_CallBackPress[j], sb);
                    }
                }
            }
        }
    }

    private void HandleContinuePressTrigger(StringBuilder sb)
    {
        var actions = _target.gameObject.GetComponentsInChildren<ContinuePressTrigger>(true);

        if (actions != null && actions.Length > 0)
        {
            sb.AppendLine();

            for (var i = 0; i < actions.Length; i++)
            {
                var act = actions[i];

                if (act.mFasterCallBackPress != null && act.mFasterCallBackPress.Count > 0)
                {
                    for (var j = 0; j < act.mFasterCallBackPress.Count; j++)
                    {
                        HandleEvent(act, "mFasterCallBackPress", act.mFasterCallBackPress[j], sb);
                    }
                }
                if (act.mFastestCallBackPress != null && act.mFastestCallBackPress.Count > 0)
                {
                    for (var j = 0; j < act.mFastestCallBackPress.Count; j++)
                    {
                        HandleEvent(act, "mFastestCallBackPress", act.mFastestCallBackPress[j], sb);
                    }
                }
                if (act.m_CallBackPress != null && act.m_CallBackPress.Count > 0)
                {
                    for (var j = 0; j < act.m_CallBackPress.Count; j++)
                    {
                        HandleEvent(act, "m_CallBackPress", act.m_CallBackPress[j], sb);
                    }
                }
            }
        }
    }

    private void HandleUIEventTrigger(StringBuilder sb)
    {
        var actions = _target.gameObject.GetComponentsInChildren<UIEventTrigger>(true);

        if (actions != null && actions.Length > 0)
        {
            sb.AppendLine();

            for (var i = 0; i < actions.Length; i++)
            {
                var act = actions[i];
                var path = GetPath(act.transform);

                if (act.onHoverOver != null && act.onHoverOver.Count > 0)
                {
                    for (var j = 0; j < act.onHoverOver.Count; j++)
                    {
                        HandleEvent(act, "onHoverOver", act.onHoverOver[j], sb);
                    }
                }
                if (act.onHoverOut != null && act.onHoverOut.Count > 0)
                {
                    for (var j = 0; j < act.onHoverOut.Count; j++)
                    {
                        HandleEvent(act, "onHoverOut", act.onHoverOut[j], sb);
                    }
                }
                if (act.onPress != null && act.onPress.Count > 0)
                {
                    for (var j = 0; j < act.onPress.Count; j++)
                    {
                        HandleEvent(act, "onPress", act.onPress[j], sb);
                    }
                }
                if (act.onRelease != null && act.onRelease.Count > 0)
                {
                    for (var j = 0; j < act.onRelease.Count; j++)
                    {
                        HandleEvent(act, "onRelease", act.onRelease[j], sb);
                    }
                }
                if (act.onSelect != null && act.onSelect.Count > 0)
                {
                    for (var j = 0; j < act.onSelect.Count; j++)
                    {
                        HandleEvent(act, "onSelect", act.onSelect[j], sb);
                    }
                }
                if (act.onDeselect != null && act.onDeselect.Count > 0)
                {
                    for (var j = 0; j < act.onDeselect.Count; j++)
                    {
                        HandleEvent(act, "onDeselect", act.onDeselect[j], sb);
                    }
                }
                if (act.onClick != null && act.onClick.Count > 0)
                {
                    for (var j = 0; j < act.onClick.Count; j++)
                    {
                        HandleEvent(act, "onClick", act.onClick[j], sb);
                    }
                }
                if (act.onDoubleClick != null && act.onDoubleClick.Count > 0)
                {
                    for (var j = 0; j < act.onDoubleClick.Count; j++)
                    {
                        HandleEvent(act, "onDoubleClick", act.onDoubleClick[j], sb);
                    }
                }
                if (act.onDragStart != null && act.onDragStart.Count > 0)
                {
                    for (var j = 0; j < act.onDragStart.Count; j++)
                    {
                        HandleEvent(act, "onDragStart", act.onDragStart[j], sb);
                    }
                }
                if (act.onDragOver != null && act.onDragOver.Count > 0)
                {
                    for (var j = 0; j < act.onDragOver.Count; j++)
                    {
                        HandleEvent(act, "onDragOver", act.onDragOver[j], sb);
                    }
                }
                if (act.onDragOut != null && act.onDragOut.Count > 0)
                {
                    for (var j = 0; j < act.onDragOut.Count; j++)
                    {
                        HandleEvent(act, "onDragOut", act.onDragOut[j], sb);
                    }
                }
                if (act.onDragEnd != null && act.onDragEnd.Count > 0)
                {
                    for (var j = 0; j < act.onDragEnd.Count; j++)
                    {
                        HandleEvent(act, "onDragEnd", act.onDragEnd[j], sb);
                    }
                }
            }
        }
    }

    private void HandleConsecutiveClickCoolTrigger(StringBuilder sb)
    {
        var actions = _target.gameObject.GetComponentsInChildren<ConsecutiveClickCoolTrigger>(true);

        if (actions != null && actions.Length > 0)
        {
            sb.AppendLine();

            for (var i = 0; i < actions.Length; i++)
            {
                var act = actions[i];

                if (act.clickEvent != null && act.clickEvent.Count > 0)
                {
                    for (var j = 0; j < act.clickEvent.Count; j++)
                    {
                        HandleEvent(act, "clickEvent", act.clickEvent[j], sb);
                    }
                }
            }
        }
    }

    private void HandleUIButton(StringBuilder sb)
    {
        var actions = _target.gameObject.GetComponentsInChildren<UIButton>(true);

        if (actions != null && actions.Length > 0)
        {
            sb.AppendLine();

            for (var i = 0; i < actions.Length; i++)
            {
                var act = actions[i];

                if (act.onClick != null && act.onClick.Count > 0)
                {
                    for (var j = 0; j < act.onClick.Count; j++)
                    {
                        HandleEvent(act, "onClick", act.onClick[j], sb);
                    }
                }
            }
        }
    }
    private void HandleUIToggle(StringBuilder sb)
    {
        var actions = _target.gameObject.GetComponentsInChildren<UIToggle>(true);

        if (actions != null && actions.Length > 0)
        {
            sb.AppendLine();

            for (var i = 0; i < actions.Length; i++)
            {
                var act = actions[i];

                if (act.onChange != null && act.onChange.Count > 0)
                {
                    for (var j = 0; j < act.onChange.Count; j++)
                    {
                        HandleEvent(act, "onChange", act.onChange[j], sb);
                    }
                }
            }
        }
    }

    private void HandleUITweener(StringBuilder sb)
    {
        var actions = _target.gameObject.GetComponentsInChildren<UITweener>(true);

        if (actions != null && actions.Length > 0)
        {
            sb.AppendLine();

            for (var i = 0; i < actions.Length; i++)
            {
                var act = actions[i];

                if (act.onFinished != null && act.onFinished.Count > 0)
                {
                    for (var j = 0; j < act.onFinished.Count; j++)
                    {
                        HandleEvent(act, "onFinished", act.onFinished[j], sb);
                    }
                }
            }
        }
    }

    private void HandleEvent(Component comp, string eventName, EventDelegate e, StringBuilder sb)
    {
        var pathParam = new StringBuilder();

        if (e.parameters != null && e.parameters.Length > 0)
        {
            for (var k = 0; k < e.parameters.Length; k++)
            {
                var path = "";
                var param = e.parameters[k];
                var dot = string.IsNullOrEmpty(param.field) ? "" : ".";

                var c = param.obj as Component;
                if (c != null)
                {
                    path = GetPath(c.transform);
                }

                var go = param.obj as GameObject;
                if (go != null)
                {
                    path = GetPath(go.transform);
                }

                if (param.type.Name == "Void")
                {
                    pathParam.Append("null");
                }
                else if (param.type.Name == "GameObject")
                {
                    pathParam.AppendFormat("t.FindEx(\"{0}\").gameObject", path);
                }
                else
                {
                    pathParam.AppendFormat("t.GetComponent<{0}>(\"{1}\"){2}{3}", param.type.Name, path, dot, param.field);
                }

                if (k < e.parameters.Length - 1)
                {
                    pathParam.Append(", ");
                }
            }
        }

        if (e.target == null)
        {
            //Debug.LogErrorFormat("e.target: {0} --> {1}", e.target, GetPath(comp.transform));
            return;
        }

        var handleStr = string.Format("t.GetComponent<{0}>(\"{1}\").{2}", e.target.GetType().Name, GetPath(e.target.transform), e.methodName);

        if ((comp is UIServerRequest) && eventName == "onResponse")
        {
            handleStr = string.Format("{0}, \"{1}\"", GetInstanceName(), e.methodName + " (请将方法名替换为：OnFetchData)");
        } 
        else if (pathParam.Length > 0)
        {
            handleStr = string.Format("() => {0}({1})", handleStr, pathParam.ToString());
        }

        sb.AppendFormat("            t.GetComponent<{0}>(\"{1}\").{2}.Add(new EventDelegate({3}));", comp.GetType().Name, GetPath(comp.transform), eventName, handleStr);
        sb.AppendLine();
    }

    private void HandleField(StringBuilder sb)
    {
        FieldInfo[] fieldInfos = _target.GetType().GetFields();

        if (fieldInfos != null)
        {
            sb.AppendFormat("            var t = {0}.transform;", GetInstanceName());
            sb.AppendLine();

            for (int i = 0; i < fieldInfos.Length; i++)
            {
                var fi = fieldInfos[i];
                var obj = fi.GetValue(_target);
                var typeName = fi.FieldType.Name;

                if (fi.FieldType.IsValueType)
                {
                    HandleValue(typeName, obj, fi.Name, sb);
                    continue;
                }

                var o1 = obj as Component;
                if (o1 != null)
                {
                    sb.AppendFormat("            {0} = t.GetComponent<{1}>(\"{2}\");", fi.Name, typeName, GetPath(o1.transform));
                    sb.AppendLine();
                    continue;
                }

                var o2 = obj as GameObject;
                if (o2 != null)
                {
                    sb.AppendFormat("            {0} = t.FindEx(\"{1}\").gameObject;", fi.Name, GetPath(o2.transform));
                    sb.AppendLine();
                    continue;
                }

                HandleListGameObject(obj, fi.Name, sb);
                HandleList<Transform>(obj, fi.Name, sb);
                HandleList<UILabel>(obj, fi.Name, sb);
                HandleList<UISprite>(obj, fi.Name, sb);
                HandleList<UIButton>(obj, fi.Name, sb);
                HandleList<TweenAlpha>(obj, fi.Name, sb);
                HandleList<TweenColor>(obj, fi.Name, sb);
                HandleList<TweenPosition>(obj, fi.Name, sb);
                HandleList<TweenScale>(obj, fi.Name, sb);
                HandleList<TweenRotation>(obj, fi.Name, sb); 

                HandleArrayGameObject(obj, fi.Name, sb);
                HandleArray<Transform>(obj, fi.Name, sb);
                HandleArray<UILabel>(obj, fi.Name, sb);
                HandleArray<UISprite>(obj, fi.Name, sb);
                HandleArray<UIButton>(obj, fi.Name, sb);
                HandleArray<TweenAlpha>(obj, fi.Name, sb);
                HandleArray<TweenColor>(obj, fi.Name, sb);
                HandleArray<TweenPosition>(obj, fi.Name, sb);
                HandleArray<TweenScale>(obj, fi.Name, sb);
                HandleArray<TweenRotation>(obj, fi.Name, sb);

                HandleValueList<int>(obj, fi.Name, sb);
                HandleValueList<float>(obj, fi.Name, sb);
                HandleValueList<bool>(obj, fi.Name, sb);

                HandleValueArray<int>(obj, fi.Name, sb);
                HandleValueArray<float>(obj, fi.Name, sb);
                HandleValueArray<bool>(obj, fi.Name, sb);

                HandleVectorList<Vector2>(obj, fi.Name, sb);
                HandleVectorList<Vector2Int>(obj, fi.Name, sb);
                HandleVectorList<Vector3>(obj, fi.Name, sb);
                HandleVectorList<Vector3Int>(obj, fi.Name, sb);
                HandleVectorList<Vector4>(obj, fi.Name, sb);

                HandleVectorArray<Vector2>(obj, fi.Name, sb);
                HandleVectorArray<Vector2Int>(obj, fi.Name, sb);
                HandleVectorArray<Vector3>(obj, fi.Name, sb);
                HandleVectorArray<Vector3Int>(obj, fi.Name, sb);
                HandleVectorArray<Vector4>(obj, fi.Name, sb);
            }
        }
    }

    private void HandleList<T>(object obj, string filedName, StringBuilder sb) where T : Component
    {
        var o = obj as List<T>;

        if (o != null && o.Count > 0)
        {
            sb.AppendLine();
            sb.AppendFormat("            {0} = new List<{1}>();", filedName, typeof(T).Name);
            sb.AppendLine();

            for (var j = 0; j < o.Count; j++)
            {
                sb.AppendFormat("            {0}.Add(t.GetComponent<{1}>(\"{2}\"));", filedName, typeof(T).Name, GetPath(o[j].transform));
                sb.AppendLine();
            }

            sb.AppendLine();
        }
    }

    private void HandleListGameObject(object obj, string filedName, StringBuilder sb)
    {
        var o = obj as List<GameObject>;

        if (o != null && o.Count > 0)
        {
            sb.AppendLine();
            sb.AppendFormat("            {0} = new List<GameObject>();", filedName);
            sb.AppendLine();

            for (var j = 0; j < o.Count; j++)
            {
                sb.AppendFormat("            {0}.Add(t.FindEx(\"{1}\").gameObject);", filedName, GetPath(o[j].transform));
                sb.AppendLine();
            }

            sb.AppendLine();
        }
    }

    private void HandleValueList<T>(object obj, string filedName, StringBuilder sb)
    {
        var o = obj as List<T>;

        if (o != null && o.Count > 0)
        {
            sb.AppendLine();
            sb.AppendFormat("            {0} = new List<{1}>();", filedName, GetValueTypeName(typeof(T).Name));
            sb.AppendLine();

            for (var j = 0; j < o.Count; j++)
            {
                sb.AppendFormat("            {0}.Add({1});", filedName, o[j]);
                sb.AppendLine();
            }

            sb.AppendLine();
        }
    }

    private void HandleVectorList<T>(object obj, string filedName, StringBuilder sb)
    {
        var o = obj as List<T>;

        if (o != null && o.Count > 0)
        {
            var typeName = typeof(T).Name;
            sb.AppendLine();
            sb.AppendFormat("            {0} = new List<{1}>();", filedName, typeName);
            sb.AppendLine();

            for (var j = 0; j < o.Count; j++)
            {
                sb.AppendFormat("            {0}.Add(new {1}{2});", filedName, typeName, o[j]);
                sb.AppendLine();
            }

            sb.AppendLine();
        }
    }

    private void HandleArray<T>(object obj, string filedName, StringBuilder sb) where T : Component
    {
        var o = obj as T[];

        if (o != null && o.Length > 0)
        {
            sb.AppendLine();
            sb.AppendFormat("            {0} = new {1}[{2}];", filedName, typeof(T).Name, o.Length);
            sb.AppendLine();

            for (var j = 0; j < o.Length; j++)
            {
                sb.AppendFormat("            {0}[{1}] = t.GetComponent<{2}>(\"{3}\");", filedName, j, typeof(T).Name, GetPath(o[j].transform));
                sb.AppendLine();
            }

            sb.AppendLine();
        }
    }

    private void HandleArrayGameObject(object obj, string filedName, StringBuilder sb)
    {
        var o = obj as GameObject[];

        if (o != null && o.Length > 0)
        {
            sb.AppendLine();
            sb.AppendFormat("            {0} = new GameObject[{1}];", filedName, o.Length);
            sb.AppendLine();

            for (var j = 0; j < o.Length; j++)
            {
                sb.AppendFormat("            {0}[{1}] = t.FindEx(\"{2}\").gameObject;", filedName, j, GetPath(o[j].transform));
                sb.AppendLine();
            }

            sb.AppendLine();
        }
    }

    private void HandleValueArray<T>(object obj, string filedName, StringBuilder sb)
    {
        var o = obj as T[];

        if (o != null && o.Length > 0)
        {
            sb.AppendLine();
            sb.AppendFormat("            {0} = new {1}[{2}];", filedName, GetValueTypeName(typeof(T).Name), o.Length);
            sb.AppendLine();

            for (var j = 0; j < o.Length; j++)
            {
                sb.AppendFormat("            {0}[{1}] = {2};", filedName, j, o[j]);
                sb.AppendLine();
            }

            sb.AppendLine();
        }
    }

    private void HandleVectorArray<T>(object obj, string filedName, StringBuilder sb)
    {
        var o = obj as T[];

        if (o != null && o.Length > 0)
        {
            var typeName = typeof(T).Name;
            sb.AppendLine();
            sb.AppendFormat("            {0} = new {1}[{2}];", filedName, typeName, o.Length);
            sb.AppendLine();

            for (var j = 0; j < o.Length; j++)
            {
                sb.AppendFormat("            {0}[{1}] = new {2}{3};", filedName, j, typeName, o[j]);
                sb.AppendLine();
            }

            sb.AppendLine();
        }
    }

    private string GetValueTypeName(string name)
    {
        switch (name)
        {
            case "Int32":
                return "int";
            case "Boolean":
                return "bool";
            case "Single":
                return "float";
            default:
                return string.Empty;
        }
    }

    private void HandleValue(string typeName, object obj, string filedName, StringBuilder sb)
    {
        switch (typeName)
        {
            case "Int32":
                sb.AppendFormat("            {0} = {1};", filedName, (int)obj);
                break;
            case "Boolean":
                sb.AppendFormat("            {0} = {1};", filedName, ((bool)obj).ToString().ToLower());
                break;
            case "Single":
                sb.AppendFormat("            {0} = {1}f;", filedName, (float)obj);
                break;
            default:
                Debug.LogError(typeName);
                break;
        }

        sb.AppendLine();
    }

    private string GetPath(Transform t)
    {
        if (t == null || _target == null)
        {
            return "";
        }

        Transform root = null;

        if (t.IsChildOf(_target.transform))
        {
            root = _target.transform;
        }

        return t.GetPathWithoutRoot(root);
    }

    private string GetBaseClassName()
    {
        if (_target is UIController)
        {
            return "UIControllerHotfix";
        }

        if (_target is DataLookup)
        {
            return "DataLookupHotfix";
        }

        return "DynamicMonoHotfix";
    }

    private string GetInstanceName()
    {
        if (_target is UIController)
        {
            return "controller";
        }

        if (_target is DataLookup)
        {
            return "mDL";
        }

        return "mDMono";
    }
}
