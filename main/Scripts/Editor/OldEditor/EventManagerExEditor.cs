using System.Collections;
using System.Collections.Generic;
using System.IO;
using Fabric;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EventManagerEx))]
public class EventManagerExEditor : Editor
{
    private EditorUndoManager undoManager;

    private EventManagerEx eventManager;

    private int menuListIndex;

    private string _eventName = "";

    public static bool IsNullOrWhiteSpace(string value)
    {
        if (value == null)
        {
            return true;
        }
        for (int i = 0; i < value.Length; i++)
        {
            if (!char.IsWhiteSpace(value[i]))
            {
                return false;
            }
        }
        return true;
    }

    private void OnEnable()
    {
        this.eventManager = (base.target as EventManagerEx);
        this.undoManager = new EditorUndoManager(this.eventManager, this.eventManager.name);
    }

    private void DrawEventListMenu()
    {
        this.eventManager._eventMenuListFoldout = EditorGUILayout.Foldout(this.eventManager._eventMenuListFoldout, "Event Menu List");
        if (this.eventManager._eventMenuListFoldout)
        {
            GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
            if (this.eventManager._eventList.Count == 0)
            {
                this.eventManager._eventList.Add("_UnSet_");
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            bool flag = GUILayout.Button("Add", new GUILayoutOption[0]);
            GUILayout.Label("Event:", new GUILayoutOption[0]);
            this._eventName = EditorGUILayout.TextField("", this._eventName, new GUILayoutOption[]
            {
                    GUILayout.MinWidth(280f),
                    GUILayout.MaxWidth(280f)
            });
            GUILayout.EndHorizontal();
            if (flag && !EventManagerExEditor.IsNullOrWhiteSpace(this._eventName))
            {
                AddEvent(this._eventName);
                this._eventName = "";
            }
            EditorGUILayout.Space();
            string[] array = this.eventManager._eventList.ToArray();
            this.menuListIndex = EditorGUILayout.Popup(this.menuListIndex, array, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            bool flag2 = GUILayout.Button("Del", new GUILayoutOption[0]);
            if (flag2 && this.menuListIndex > 0)
            {
                this.eventManager._eventList.Remove(array[this.menuListIndex]);
                EventManagerExEditor.RenameEvent(array[this.menuListIndex], "");
                array = this.eventManager._eventList.ToArray();
                this.menuListIndex--;
            }
            if (array != null && array.Length > 0)
            {
                if (this.eventManager._eventList[0] == "UnSet")
                {
                    this.eventManager._eventList[0] = "_UnSet_";
                }
                GUILayout.Label("Event:", new GUILayoutOption[0]);
                string text = array[this.menuListIndex];
                text = EditorGUILayout.TextField(text, new GUILayoutOption[]
                {
                        GUILayout.MinWidth(280f),
                        GUILayout.MaxWidth(280f)
                });
                if (text != this.eventManager._eventList[this.menuListIndex] && this.menuListIndex != 0)
                {
                    EventManagerExEditor.RenameEvent(this.eventManager._eventList[this.menuListIndex], text);
                    this.eventManager._eventList[this.menuListIndex] = text;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GameObject[] array2 = null;
            if (GUILayout.Button("Locate EventListeners", new GUILayoutOption[0]))
            {
                array2 = EventManagerExEditor.FindEventListeners(array[this.menuListIndex]);
            }
            if (GUILayout.Button("Locate EventTriggers", new GUILayoutOption[0]))
            {
                array2 = EventManagerExEditor.FindEventTriggers(array[this.menuListIndex]);
            }
            if (array2 != null && array2.Length > 0)
            {
                Selection.objects = array2;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(10f);
            GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("Clear Event List", new GUILayoutOption[0]) && EditorUtility.DisplayDialog("Event Manager List", "You are about to clear the entire event list, Are you sure you want to do this?!!", "Ok", "Cancel"))
            {
                this.ClearEventList();
            }
            if (GUILayout.Button("Clear All", new GUILayoutOption[0]) && EditorUtility.DisplayDialog("Event Manager List", "You are about to clear the entire event list, event listeners and event triggers, Are you sure you want to do this?!!", "Ok", "Cancel"))
            {
                this.ClearAll();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("Load Events From File", new GUILayoutOption[0]))
            {
                string text2 = EditorUtility.OpenFilePanel("Load Event Names from txt file", "", "txt");
                if (text2 != null && text2.Length > 0)
                {
                    string[] files = Directory.GetFiles(text2);
                    for (int i = 0; i < files.Length; i++)
                    {
                        this.eventManager.AddEventNamesFromFile(files[i]);
                        this.menuListIndex = 0;
                    }
                }
            }
            if (GUILayout.Button("Save Events To File", new GUILayoutOption[0]))
            {
                string text3 = EditorUtility.SaveFilePanel("Save Event List to a txt file", "", "EventList.txt", "txt");
                if (text3 != null && text3.Length != 0)
                {
                    this.eventManager.ExportEventNamesToFile(text3);
                }
            }
            if (GUILayout.Button("Unload Events From File", new GUILayoutOption[0]))
            {
                string text4 = EditorUtility.OpenFilePanel("Unload Event Names From txt file", "", "txt");
                if (text4 != null && text4.Length > 0)
                {
                    string[] files2 = Directory.GetFiles(text4);
                    for (int j = 0; j < files2.Length; j++)
                    {
                        this.eventManager.RemoveEventNamesFromFile(files2[j]);
                        this.menuListIndex = 0;
                    }
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }
    }

    public static GameObject[] FindEventListeners(string eventName)
    {
        List<GameObject> list = new List<GameObject>();
        EventListener[] array = Object.FindObjectsOfType(typeof(EventListener)) as EventListener[];
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i]._eventName == eventName || (array[i]._eventName.Length == 0 && eventName == "_UnSet_"))
            {
                list.Add(array[i].gameObject);
            }
        }
        return list.ToArray();
    }

    public static GameObject[] FindEventTriggers(string eventName)
    {
        List<GameObject> list = new List<GameObject>();
        Object[] array = Object.FindObjectsOfType(typeof(GameObject));
        for (int i = 0; i < array.Length; i++)
        {
            GameObject gameObject = (GameObject)array[i];
            if (gameObject.transform.parent == null)
            {
                EventTrigger[] componentsInChildren = gameObject.GetComponentsInChildren<EventTrigger>();
                for (int j = 0; j < componentsInChildren.Length; j++)
                {
                    if (componentsInChildren[j]._eventName == eventName || (componentsInChildren[j]._eventName.Length == 0 && eventName == "_UnSet_"))
                    {
                        list.Add(componentsInChildren[j].gameObject);
                    }
                }
            }
        }
        return list.ToArray();
    }

    private void DrawEventList()
    {
        if (this.eventManager._eventList.Count == 0)
        {
            this.eventManager._eventList.Add("_UnSet_");
        }
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        bool flag = GUILayout.Button("Add", new GUILayoutOption[0]);
        GUILayout.Label("Event:", new GUILayoutOption[0]);
        this._eventName = EditorGUILayout.TextField("", this._eventName, new GUILayoutOption[]
        {
                GUILayout.MinWidth(280f),
                GUILayout.MaxWidth(280f)
        });
        GUILayout.EndHorizontal();
        if (flag && !EventManagerExEditor.IsNullOrWhiteSpace(this._eventName))
        {
            AddEvent(this._eventName);
            this._eventName = "";
        }
        EditorGUILayout.Space();
        List<string> list = new List<string>();
        for (int i = 1; i < this.eventManager._eventList.Count; i++)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            bool flag2 = GUILayout.Button("Del", new GUILayoutOption[0]);
            if (flag2)
            {
                list.Add(this.eventManager._eventList[i]);
            }
            GUILayout.Label("Event:", new GUILayoutOption[0]);
            string text = this.eventManager._eventList[i];
            text = EditorGUILayout.TextField(text, new GUILayoutOption[]
            {
                    GUILayout.MinWidth(280f),
                    GUILayout.MaxWidth(280f)
            });
            if (text != this.eventManager._eventList[i])
            {
                EventManagerExEditor.RenameEvent(this.eventManager._eventList[i], text);
                this.eventManager._eventList[i] = text;
            }
            GUILayout.EndHorizontal();
        }
        for (int j = 0; j < list.Count; j++)
        {
            string text2 = list[j];
            this.eventManager._eventList.Remove(text2);
            EventManagerExEditor.RenameEvent(text2, "");
        }
    }

    public static string BuildEventName(string eventName, EventManagerEx eventManager, EventListComponent[] eventListComponents)
    {
        GUILayout.BeginVertical(new GUILayoutOption[0]);
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        GUILayout.Label("Event Name:", new GUILayoutOption[0]);
        if (eventName == "DynamicMixer" || eventName == "AudioMixer")
        {
            eventName = "_UnSet_";
        }
        List<string> list = new List<string>();
        list.Add("_UnSet_");
        if (eventManager != null)
        {
            if (eventManager._eventList[0] == "UnSet")
            {
                eventManager._eventList[0] = "_UnSet_";
            }
            list = new List<string>(eventManager._eventList.ToArray());
        }
        if (eventListComponents != null)
        {
            for (int i = 0; i < eventListComponents.Length; i++)
            {
                for (int j = 0; j < eventListComponents[i]._eventList.Count; j++)
                {
                    list.Add(eventListComponents[i]._eventList[j]);
                }
            }
        }
        int num = -1;
        for (int k = 0; k < list.Count; k++)
        {
            if (list[k] == eventName)
            {
                num = k;
                break;
            }
        }
        if (num >= 0)
        {
            string[] array = list.ToArray();
            int num2 = EditorGUILayout.Popup(num, array, new GUILayoutOption[0]);
            if (num2 != num)
            {
                eventName = array[num2];
            }
        }
        else
        {
            Color backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            GUILayout.Label(eventName, "Box", new GUILayoutOption[]
            {
                    GUILayout.ExpandWidth(true)
            });
            GUI.backgroundColor = backgroundColor;
            if (GUILayout.Button("Clear", new GUILayoutOption[]
            {
                    GUILayout.MaxWidth(45f)
            }))
            {
                eventName = "_UnSet_";
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        return eventName;
    }

    public void AddEvent(string eventName)
    {
        eventName.Trim();
        if (this.eventManager._eventList.Contains(eventName))
        {
            EditorUtility.DisplayDialog("Error", "event name is existed, Please Check", "OK");
        }
        else
        {
            this.eventManager._eventList.Add(eventName);
        }
        this.eventManager._eventList.Sort();
        base.Repaint();
    }

    public static void RenameEvent(string eventName, string newEventName)
    {
        if (FabricManager.IsInitialised())
        {
            EventListener[] componentsInChildren = FabricManager.Instance.gameObject.GetComponentsInChildren<EventListener>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if (componentsInChildren[i]._eventName == eventName)
                {
                    componentsInChildren[i]._eventName = newEventName;
                }
            }
        }
        new List<GameObject>();
        Object[] array = Object.FindObjectsOfType(typeof(GameObject));
        for (int j = 0; j < array.Length; j++)
        {
            GameObject gameObject = (GameObject)array[j];
            if (gameObject.transform.parent == null)
            {
                EventTrigger[] componentsInChildren2 = gameObject.GetComponentsInChildren<EventTrigger>();
                for (int k = 0; k < componentsInChildren2.Length; k++)
                {
                    if (componentsInChildren2[k]._eventName == eventName)
                    {
                        componentsInChildren2[k]._eventName = newEventName;
                    }
                }
            }
        }
    }

    private void ClearAll()
    {
        if (FabricManager.IsInitialised())
        {
            EventListener[] componentsInChildren = FabricManager.Instance.gameObject.GetComponentsInChildren<EventListener>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i]._eventName = "_UnSet_";
            }
        }
        new List<GameObject>();
        Object[] array = Object.FindObjectsOfType(typeof(GameObject));
        for (int j = 0; j < array.Length; j++)
        {
            GameObject gameObject = (GameObject)array[j];
            if (gameObject.transform.parent == null)
            {
                EventTrigger[] componentsInChildren2 = gameObject.GetComponentsInChildren<EventTrigger>();
                for (int k = 0; k < componentsInChildren2.Length; k++)
                {
                    componentsInChildren2[k]._eventName = "_UnSet_";
                }
            }
        }
        this.ClearEventList();
    }

    private void ClearEventList()
    {
        this.eventManager._eventList.Clear();
    }

    public override void OnInspectorGUI()
    {
        this.undoManager.CheckUndo();
        MenuBar.OnGUI("288079-eventmanager", true, null);
        if (this.eventManager == null)
        {
            return;
        }
        GUILayout.BeginVertical("box", new GUILayoutOption[0]);
        this.eventManager._logHistorySize = EditorGUILayout.IntSlider("Log History:", this.eventManager._logHistorySize, 0, 1000, new GUILayoutOption[0]);
        this.eventManager._forceQueueAllEvents = EditorGUILayout.Toggle("Force Queue All Events:", this.eventManager._forceQueueAllEvents, new GUILayoutOption[0]);
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        GUILayout.Label("Num of Events in Queue: " + this.eventManager.NumOfEventsInQueue(), new GUILayoutOption[0]);
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
        GUILayout.Label("CPU:" + this.eventManager.profiler.percent.ToString("0.000") + "% - ms:" + this.eventManager.profiler.msPerFrame.ToString("0.000"), new GUILayoutOption[0]);
        GUILayout.Space(10f);
        if(GUILayout.Button("Open Editor Window", new GUILayoutOption[0]))
        {
            EventManagerExWindow.Open(eventManager);
        }
        GUILayout.EndVertical();
        this.DrawEventListMenu();
        GUILayout.Space(10f);
        this.eventManager._eventListFoldout = EditorGUILayout.Foldout(this.eventManager._eventListFoldout, "Event List");
        if (this.eventManager._eventListFoldout)
        {
            GUILayout.BeginVertical("box", new GUILayoutOption[0]);
            this.DrawEventList();
            GUILayout.EndVertical();
        }
        GUIHelpers.CheckGUIHasChanged(this.eventManager.gameObject);
        this.undoManager.CheckDirty();
    }

    public static string DropDownEventNames(string eventName)
    {
        EventListComponent[] eventListComponents = Object.FindObjectsOfType(typeof(EventListComponent)) as EventListComponent[];
        return EventManagerExEditor.BuildEventName(eventName, EventManagerEx.Instance, eventListComponents);
    }
    
}
