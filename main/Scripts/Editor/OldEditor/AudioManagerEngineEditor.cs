using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Fabric
{
    [CustomEditor(typeof(AudioManagerEngine))]
    public class AudioManagerEngineEditor : Editor
    {
        private EditorUndoManager undoManager;

        private FabricManager fabricManager;

        private int selectedMusicSetting;

        private string musicSettingName = "";

        [MenuItem("Fabric/AudioManagerEngine")]
        private static void CreateGameObject()
        {
            if (GetFabricManager.Instance() != null)
            {
                return;
            }
            GameObject gameObject = new GameObject("Audio");
            gameObject.AddComponent<FabricManager>();
            gameObject.AddComponent<EventManagerEx>();
        }

        private void OnEnable()
        {
            this.fabricManager = (base.target as FabricManager);
            this.undoManager = new EditorUndoManager(this.fabricManager, this.fabricManager.name);
        }

        public static FabricManager GetManagerInstance()
        {
            if (FabricManager.Instance != null)
            {
                return FabricManager.Instance;
            }
            if (Selection.activeGameObject != null)
            {
                FabricManager fabricManager = null;
                GameObject activeGameObject = Selection.activeGameObject;
                PrefabType prefabType = PrefabUtility.GetPrefabType(activeGameObject);
                if (prefabType == PrefabType.Prefab)
                {
                    GameObject gameObject = activeGameObject.transform.root.gameObject;
                    if (gameObject != null)
                    {
                        fabricManager = gameObject.GetComponent<FabricManager>();
                        FabricManager.Instance = fabricManager;
                    }
                }
                return fabricManager;
            }
            return null;
        }

        public override void OnInspectorGUI()
        {
            MenuBar.OnGUI("288078-fabricmanager", true, this.fabricManager.gameObject);
            AudioManagerEngineEditor.GetManagerInstance();
            this.undoManager.CheckUndo();
            GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
            this.fabricManager._dontDestroyOnLoad = EditorGUILayout.Toggle("Dont Destroy OnLoad: ", this.fabricManager._dontDestroyOnLoad, new GUILayoutOption[0]);
            this.fabricManager._showAllInstances = EditorGUILayout.Toggle("Show All Instances: ", this.fabricManager._showAllInstances, new GUILayoutOption[0]);
            this.fabricManager._enableVirtualization = EditorGUILayout.Toggle("AudioComponent Virtualization: ", this.fabricManager._enableVirtualization, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            this.fabricManager._createEventListeners = EditorGUILayout.Toggle("Create Event Listeners: ", this.fabricManager._createEventListeners, new GUILayoutOption[0]);
            if (this.fabricManager._createEventListeners)
            {
                this.fabricManager._useFullPathForEventListeners = EditorGUILayout.Toggle("Use full path for name:", this.fabricManager._useFullPathForEventListeners, new GUILayoutOption[0]);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            this.fabricManager.enableTimelineAssetLoader = EditorGUILayout.Toggle("Timeline Loader: ", this.fabricManager.enableTimelineAssetLoader, new GUILayoutOption[0]);
            if (this.fabricManager.enableTimelineAssetLoader && GUILayout.Button("Import ftp Project: ", new GUILayoutOption[0]))
            {
                string text = EditorUtility.OpenFilePanel("import ftp Project", "", "ftp");
                if (text.Length != 0)
                {
                    TimelineLoader.LoadFtpProject(text);
                }
            }
            GUILayout.EndHorizontal();
            this.fabricManager._enableDebugLog = EditorGUILayout.Toggle("Debug Log: ", this.fabricManager._enableDebugLog, new GUILayoutOption[0]);
            this.fabricManager._allowExternalGroupComponents = EditorGUILayout.Toggle("External Groups: ", this.fabricManager._allowExternalGroupComponents, new GUILayoutOption[0]);
            int languageByIndex = EditorGUILayout.Popup("Language: ", this.fabricManager.GetLanguageIndex(), this.fabricManager.GetLanguageNames(), new GUILayoutOption[0]);
            this.fabricManager.SetLanguageByIndex(languageByIndex);
            this.fabricManager._transitionThreshold = (double)EditorGUILayout.Slider("Transition Threshold (sec): ", (float)this.fabricManager._transitionThreshold, 0.01f, 1f, new GUILayoutOption[0]);
            this.fabricManager._volumeThreshold = (double)EditorGUILayout.Slider("Volume Threshold: ", (float)this.fabricManager._volumeThreshold, 0f, 1f, new GUILayoutOption[0]);
            this.fabricManager._onApplicationPauseBehavior = (FabricManager.OnApplicationPauseBehavior)EditorGUILayout.EnumPopup("On Application Pause: ", this.fabricManager._onApplicationPauseBehavior, new GUILayoutOption[0]);
            bool flag = EditorGUILayout.Toggle("Bake Component Inst: ", this.fabricManager._bakeComponentInstances, new GUILayoutOption[0]);
            if (flag != this.fabricManager._bakeComponentInstances)
            {
                if (flag)
                {
                    this.fabricManager.BakeComponentInstances();
                }
                else
                {
                    this.fabricManager.CleanBakedComponentInstances();
                }
            }
            this.fabricManager._bakeComponentInstances = flag;
            bool flag2 = EditorGUILayout.Toggle("Playmode Persistence: ", FabricEditorData.GetData()._playmodePersistance, new GUILayoutOption[0]);
            if (flag2 != FabricEditorData.GetData()._playmodePersistance)
            {
                FabricEditorData.GetData()._playmodePersistance = flag2;
                FabricEditorData.ApplyChanges();
            }
            FabricEditorData.GetData()._enableHiearchyIcons = EditorGUILayout.Toggle("Show Component Icons: ", FabricEditorData.GetData()._enableHiearchyIcons, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.LabelField("Editor Path: " + this.fabricManager._fabricEditorPath, new GUILayoutOption[0]);
            if (GUILayout.Button("Set Path", new GUILayoutOption[0]))
            {
                this.fabricManager._fabricEditorPath = AudioManagerEngineEditor.SetFabricEditorPath(this.fabricManager._fabricEditorPath);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            this.AddRemoveAudioMixerDebugLogComponents();
            GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
            this.fabricManager._forceAxisVector = EditorGUILayout.Vector3Field("Force Axis: ", this.fabricManager._forceAxisVector, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            GUILayout.BeginVertical("Music Time Settings", "Box", new GUILayoutOption[0]);
            GUILayout.Space(15f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            this.musicSettingName = EditorGUILayout.TextField("Name: ", this.musicSettingName, new GUILayoutOption[0]);
            if (GUILayout.Button("Add", new GUILayoutOption[0]))
            {
                MusicTimeSittings musicTimeSittings = new MusicTimeSittings();
                musicTimeSittings._name = this.musicSettingName;
                this.fabricManager._musicTimeSignatures.Add(musicTimeSittings);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            int count = this.fabricManager._musicTimeSignatures.Count;
            if (count > 0)
            {
                string[] array = new string[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = this.fabricManager._musicTimeSignatures[i]._name;
                }
                this.selectedMusicSetting = EditorGUILayout.Popup("Music Time Settings: ", this.selectedMusicSetting, array, new GUILayoutOption[0]);
                MusicTimeSittings musicTimeSittings2 = this.fabricManager._musicTimeSignatures[this.selectedMusicSetting];
                musicTimeSittings2._name = EditorGUILayout.TextField("Name:", musicTimeSittings2._name, new GUILayoutOption[0]);
                musicTimeSittings2._bpm = EditorGUILayout.FloatField("Tempo: ", musicTimeSittings2._bpm, new GUILayoutOption[0]);
                musicTimeSittings2._syncType = (MusicSyncType)EditorGUILayout.EnumPopup("Sync Type: ", musicTimeSittings2._syncType, new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                EditorGUILayout.LabelField("Time Signature: ", new GUILayoutOption[]
                {
                    GUILayout.MaxWidth(148f)
                });
                string[] array2 = new string[17];
                for (int j = 1; j < 17; j++)
                {
                    array2[j] = j.ToString();
                }
                musicTimeSittings2._timeSignatureLower = EditorGUILayout.Popup(musicTimeSittings2._timeSignatureLower, array2, new GUILayoutOption[]
                {
                    GUILayout.MaxWidth(30f)
                });
                EditorGUILayout.LabelField(" / ", new GUILayoutOption[]
                {
                    GUILayout.MaxWidth(25f)
                });
                string[] array3 = new string[]
                {
                    "2",
                    "4",
                    "8"
                };
                int num = 0;
                for (int k = 0; k < array3.Length; k++)
                {
                    if (array3[k] == musicTimeSittings2._timeSignatureUpper.ToString())
                    {
                        num = k;
                        break;
                    }
                }
                num = EditorGUILayout.Popup(num, array3, new GUILayoutOption[]
                {
                    GUILayout.MaxWidth(30f)
                });
                musicTimeSittings2._timeSignatureUpper = int.Parse(array3[num]);
                GUILayout.EndHorizontal();
                GUILayout.Space(5f);
                AudioManagerEngineEditor.MusicSyncToTarget(musicTimeSittings2);
                GUILayout.Space(5f);
                if (this.selectedMusicSetting >= 0 && GUILayout.Button("Delete", new GUILayoutOption[0]))
                {
                    this.fabricManager._musicTimeSignatures.RemoveAt(this.selectedMusicSetting);
                    this.selectedMusicSetting = 0;
                }
            }
            GUILayout.Space(5f);
            GUILayout.EndVertical();
            GUILayout.Space(5f);
            GUILayout.BeginVertical("AudioSource Pool", "Box", new GUILayoutOption[0]);
            GUILayout.Space(20f);
            this.fabricManager._audioSourcePool = EditorGUILayout.IntField("Size: ", this.fabricManager._audioSourcePool, new GUILayoutOption[0]);
            AudioSourcePoolEditor.DisplayProperties(this.fabricManager);
            GUILayout.EndVertical();
            GUILayout.Space(5f);
            GUILayout.BeginVertical("Manager Info", "Box", new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayout.Label("Total gameObjects used: " + this.fabricManager._totalNumberOfGameObjectsUsed, new GUILayoutOption[0]);
            GUILayout.Space(5f);
            float num2 = this.fabricManager._totalMemoryUsed / 1024f / 1024f;
            GUILayout.Label("Total memory used: " + num2 + " Mb", new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayout.Label("CPU:" + this.fabricManager.profiler.percent.ToString("0.000") + "% - ms:" + this.fabricManager.profiler.msPerFrame.ToString("0.000"), new GUILayoutOption[0]);
            GUILayout.EndVertical();
            GUIHelpers.CheckGUIHasChanged(this.fabricManager.gameObject);
            this.undoManager.CheckDirty();
        }

        private void AddRemoveAudioMixerDebugLogComponents()
        {
            GUILayout.Space(5f);
            GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            AudioMixer component = FabricManager.Instance.GetComponent<AudioMixer>();
            if (component == null)
            {
                Color backgroundColor = GUI.backgroundColor;
                GUI.backgroundColor =(Color.red);
                if (GUILayout.Button("Add AudioMixer Manager", new GUILayoutOption[0]))
                {
                    FabricManager.Instance.gameObject.AddComponent<AudioMixer>();
                }
                GUI.backgroundColor =(backgroundColor);
            }
            else
            {
                Color backgroundColor2 = GUI.backgroundColor;
                GUI.backgroundColor =(Color.green);
                if (GUILayout.Button("Remove AudioMixer Manager", new GUILayoutOption[0]))
                {
                    component._destroy = true;
                }
                GUI.backgroundColor =(backgroundColor2);
            }
            DebugLog component2 = FabricManager.Instance.GetComponent<DebugLog>();
            if (component2 == null)
            {
                Color backgroundColor3 = GUI.backgroundColor;
                GUI.backgroundColor =(Color.red);
                if (GUILayout.Button("Add DebugLog", new GUILayoutOption[0]))
                {
                    FabricManager.Instance.gameObject.AddComponent<DebugLog>();
                }
                GUI.backgroundColor =(backgroundColor3);
            }
            else
            {
                Color backgroundColor4 = GUI.backgroundColor;
                GUI.backgroundColor =(Color.green);
                if (GUILayout.Button("Remove DebugLog", new GUILayoutOption[0]))
                {
                    component2._destroy = true;
                }
                GUI.backgroundColor =(backgroundColor4);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(5f);
        }

        public static void MusicSyncToTarget(MusicTimeSittings timeSetting)
        {
            if (timeSetting == null)
            {
                return;
            }
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
            GUILayout.Label("Sync To Target: ", new GUILayoutOption[]
            {
                GUILayout.MaxWidth(100f)
            });
            timeSetting._syncTarget = (Component)EditorGUILayout.ObjectField("", timeSetting._syncTarget, typeof(Component), new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            if (timeSetting._syncTarget != null)
            {
                float num = (float)timeSetting._syncTarget.GetCurrentTime(true);
                float length = timeSetting._syncTarget.GetLength();
                float width = GUILayoutUtility.GetLastRect().width;
                AudioManagerEngineEditor.MakeProgressBar("Time: ", width, 15f, num / length * width, Color.green);
            }
            GUILayout.EndVertical();
        }

        private static void MakeProgressBar(string name, float width, float height, float fillWidth, Color col)
        {
            Texture2D texture2D = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
            float num = 1f / width;
            int num2 = 0;
            while ((float)num2 < width)
            {
                float num3 = (float)num2 * num;
                Color color;
                if ((float)num2 < fillWidth)
                {
                    color = (1f - num3) * Color.green + num3 * Color.yellow;
                }
                else
                {
                    color = Color.black;
                }
                int num4 = 0;
                while ((float)num4 < height)
                {
                    texture2D.SetPixel(num2, num4, color);
                    num4++;
                }
                num2++;
            }
            texture2D.Apply();
            GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
            GUILayout.Label(name, new GUILayoutOption[]
            {
                GUILayout.MaxWidth(100f)
            });
            GUILayout.Box(texture2D, new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true)
            });
            GUILayout.EndHorizontal();
        }

        public static string SetFabricEditorPath(string path)
        {
            string text = EditorUtility.OpenFolderPanel("Fabric's editor folder ", "", "");
            if (text != null && text.Length > 0)
            {
                path = text;
            }
            else
            {
                EditorUtility.DisplayDialog("Fabric Warning", "A folder not selected, a default one will be created instead [Assets/Fabric/Editor]", "Ok");
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            int num = path.LastIndexOf("Assets/");
            if (num >= 0)
            {
                path = path.Remove(0, num);
            }
            return path;
        }

        public static string GetFabricEditorPath()
        {
            string text = "Assets/Fabric/Editor";
            if (GetFabricManager.Instance() != null)
            {
                text = GetFabricManager.Instance()._fabricEditorPath;
                if (!Directory.Exists(text))
                {
                    GetFabricManager.Instance()._fabricEditorPath = AudioManagerEngineEditor.SetFabricEditorPath(text);
                }
            }
            return text;
        }
    }
}
