using UnityEditor;
using UnityEngine;
using System.IO;

public class UwaDataUploaderEditorWindow : EditorWindow {
    private class ConfigInfo {
        public string user;
        public string password;
        public string project;
    }

    private class ResultInfo {
        public string status;
        public string reason;
        public string projctid;
    }

    private static string _username;
    private static string _password;
    private static string _projectName;
    private ResultInfo _resultInfo;

    [MenuItem("Tools/UWA Data Uploader", priority = 9)]
    private static void Init() {
        UwaDataUploaderEditorWindow window = (UwaDataUploaderEditorWindow)GetWindow(typeof(UwaDataUploaderEditorWindow));
        window.minSize = new Vector2(300, 150);
        window.Show();
        ReadConfigInfo();
    }

    private void OnGUI() {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        _username = EditorGUILayout.TextField("Username", _username);
        _password = EditorGUILayout.TextField("Password", _password);
        _projectName = EditorGUILayout.TextField("Project Name", _projectName);

        EditorGUILayout.Space();

        if (GUILayout.Button("Upload Scan Data")) {
            SaveConfigInfo();
            UploadDataToUWA();
        }

        if (IsExistResultFile()) {
            if (_resultInfo == null) {
                _resultInfo = ReadResultInfo();
            }

            var msg = _resultInfo.reason;
            var type = MessageType.Info;

            if (_resultInfo.status == "success") {
                msg = string.Format("上传成功【项目ID：{0}】", _resultInfo.projctid);
            } else {
                type = MessageType.Error;
            }

            EditorGUILayout.HelpBox(msg, type, true);
        }
    }

    private void OnDestroy() {
        if (IsExistResultFile()) {
            File.Delete(Application.dataPath.Replace("Assets", "UwaDataUploader/result.json"));
        }
    }

    private static void ReadConfigInfo() {
        var text = File.ReadAllText(Application.dataPath.Replace("Assets", "UwaDataUploader/config.json"));
        var config = JsonUtility.FromJson<ConfigInfo>(text);
        _username = config.user;
        _password = config.password;
        _projectName = config.project;
    }

    private void SaveConfigInfo() {
        var configInfo = new ConfigInfo();
        configInfo.user = _username;
        configInfo.password = _password;
        configInfo.project = _projectName;

        var text = JsonUtility.ToJson(configInfo, true);
        File.WriteAllText(Application.dataPath.Replace("Assets", "UwaDataUploader/config.json"), text);
    }

    private bool IsExistResultFile() {
        return File.Exists(Application.dataPath.Replace("Assets", "UwaDataUploader/result.json"));
    }

    private ResultInfo ReadResultInfo() {
        var text = File.ReadAllText(Application.dataPath.Replace("Assets", "UwaDataUploader/result.json"));
        return JsonUtility.FromJson<ResultInfo>(text);
    }

    private void UploadDataToUWA() {
#if UNITY_EDITOR_WIN
        string command = Application.dataPath.Replace("Assets", "UwaDataUploader/UwaDataUploader.exe");
#else
        string command = "mono " + Application.dataPath.Replace("Assets", "UwaDataUploader/UwaDataUploader.exe");
#endif
        string args = Application.dataPath.Replace("Assets", "UwaScan");
        CommandLineUtils.Run(command, args);
    }
}
