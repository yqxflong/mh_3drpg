using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using Fabric;

public class EventManagerExWindow : EditorWindow
{
    private static EventManagerExWindow mWindow;

    private EventManagerEx eventManager;

    private UnityEngine.Object[] objs;
    private bool[] folds;
    private int[] selectIndexs;
    private int[] lastSelectIndexs;
    private string[] eventNames;
    private List<GroupComponent> groupsList;

    private int maxCount = 5;

    public static void Open(EventManagerEx manager)
    {
        mWindow = EditorWindow.GetWindow<EventManagerExWindow>();
        mWindow.titleContent = new UnityEngine.GUIContent("  添加音效拓展工具  ");
        mWindow.position = new Rect((Screen.currentResolution.width - 640) / 2, (Screen.currentResolution.height - 480) / 2, 640, 480);
        mWindow.InitWindow(manager);
    }

    private void InitWindow(EventManagerEx manager)
    {
        eventManager = manager;
        groupsList = new List<GroupComponent>();
        var groups = eventManager.GetComponentsInChildren<GroupComponent>(true);
        for(int i=0;i< groups.Length; ++i)
        {
            var temp = groups[i].transform.Find("Template");
            if (temp != null)
            {
                groupsList.Add(groups[i]);
            }
        }
        folds = new bool[maxCount];
        selectIndexs = new int[maxCount];
        lastSelectIndexs = new int[maxCount];
        for (int i=0;i< lastSelectIndexs.Length; ++i)
        {
            lastSelectIndexs[i] = -1;
        }
        eventNames = new string[maxCount];
    }

    void OnGUI()
    {
        EditorGUILayout.Space();
        if (Selection.objects != null && Selection.objects.Length > 0)
        {
            objs = Selection.objects;
            int count = 0;
            for (int i=0;i< objs.Length; ++i)
            {
                var select= objs[i] as AudioClip;
                if (select == null) continue;
                if (count >= maxCount) break;
                count++;
                folds[i] = EditorGUILayout.InspectorTitlebar(folds[i], objs[i]);
                if (!folds[i])
                {
                    EditorGUILayout.Space();
                    var options = new string[groupsList.Count];
                    for (var j = 0; j < groupsList.Count; ++j)
                    {
                        options[j] = groupsList[j].transform.name;
                    }
                    selectIndexs[i] = EditorGUILayout.Popup("  目标存放父节点：", selectIndexs[i], options);
                    EditorGUILayout.Space();
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.Label("  目标节点或事件名称：", new GUILayoutOption[0]);
                    eventNames[i] = EditorGUILayout.TextField("", eventNames[i], new GUILayoutOption[0]);
                    if (lastSelectIndexs[i]!= selectIndexs[i])
                    {
                        lastSelectIndexs[i] = selectIndexs[i];
                        string name= AssetDatabase.GetAssetPath(select).Replace("Assets/_GameAssets/Res/Audio/AudioSource/","").Replace(".wav","").Replace(".mp3", "");
                        eventNames[i] = name;
                    }
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    if (GUILayout.Button("  修改预设并保存  ", new GUILayoutOption[0]))
                    {
                        Save(select, groupsList[selectIndexs[i]],eventNames[i]);
                    }
                    EditorGUILayout.Space();
                }
            }

            if (count > 0)
            {
                EditorGUILayout.Space();
                if (count >= maxCount)
                {
                    EditorGUILayout.Space();
                    GUILayout.Label(string.Format ("——最多仅支持同时编辑{0}个——", maxCount), new GUILayoutOption[0]);
                }
            }
            else
            {
                GUILayout.Label("——Project中选中需添加到AudioListener中的音效文件——", new GUILayoutOption[0]);
                EditorGUILayout.Space();
            }
        }
        else
        {
            GUILayout.Label("——Project中选中需添加到AudioListener中的音效文件——", new GUILayoutOption[0]);
            EditorGUILayout.Space();
        }
    }

    private void Save(AudioClip clip, GroupComponent parent,string name)
    {
        if(string.IsNullOrEmpty(name))
        {
            EditorUtility.DisplayDialog("错误", "不能为空，请输入目标节点或事件名称", "确定");
            return;
        }
        name.Trim();
        string eventName = string.Empty;
        string objName = string.Empty;
        if (name.Contains("_"))
        {
            objName = name;
            eventName = name.Replace("_", "/");
        }
        else if(name.Contains("/"))
        {
            eventName = name;
            objName = name.Replace( "/", "_");
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "请正确输入目标节点或事件名称，需包含“_”或“/”字符间隔", "确定");
            return;
        }

        var temp = parent.transform.Find(objName);
        if (temp != null)
        {
            EditorUtility.DisplayDialog("错误", string.Format("{0}节点目录下已存在{1}节点", parent.gameObject.name, name), "确定");
            return;
        }
        
        if (eventManager._eventList.Contains(eventName))
        {
            EditorUtility.DisplayDialog("错误", string.Format("EventManager中已存在该事件名：{0}", eventName), "确定");
            return;
        }
        else
        {
            eventManager._eventList.Add(eventName);
        }
        eventManager._eventList.Sort();
        Repaint();

        temp = parent.transform.Find("Template");
        var obj = GameObject.Instantiate(temp.gameObject, parent.transform);
        obj.name = objName;
        obj.GetComponent<AudioComponent>().AudioClip = clip;
        obj.GetComponent<EventListener>()._eventName = eventName;
    }
}