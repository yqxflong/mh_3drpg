using System.IO;
using UnityEditor;
using UnityEngine;

public class DataCacheEditor : EditorWindow
{
    private string ServerPath = "F:/FileBackup/lostTemple_server/sparx_gam/bfbs";

    private string TableName = "";

    [MenuItem("DataCache/Tool")]
    public static void OpenDataCacheEditor()
    {
        EditorWindow.GetWindow(typeof(DataCacheEditor));
    }

    private void OnGUI()
    {
        string schemaPath = Path.Combine(Application.dataPath, "../../Source/FlatBuffers/Schema");
        string dataCachePath = Path.Combine(Application.dataPath, "../../Source/FlatBuffers/DataCache");
        string gamePath = Path.Combine(Application.dataPath, "GM/Scripts/DataCache");

        if (GUILayout.Button("Open Floder"))
        {
            string path = schemaPath.Replace("/", "\\");
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        GUILayout.BeginHorizontal();
        TableName = EditorGUILayout.TextField("TableName:", TableName);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Generate"))
        {
            string sourceFile = string.Format("{0}/{1}.fbs", schemaPath, TableName);//生成
            string tempPath = string.Format("{0}/DataCache", schemaPath);
            CommandLineUtils.Run("flatc", string.Format("-c -b --schema --csharp --gen-onefile -o {0} {1}", tempPath, sourceFile));

            string[] tempFiles = Directory.GetFiles(string.Format("{0}/DataCache", schemaPath));//拷贝至DataCache目录
            foreach (string tempFile in tempFiles)
            {
                string dataCacheFile = string.Format("{0}/{1}", dataCachePath, Path.GetFileName(tempFile));
                File.Copy(tempFile, dataCacheFile, true);
            }
        }

        if (GUILayout.Button("Copy To Client"))
        {
            string gameFile = string.Format("{0}/{1}.cs", gamePath, TableName);//拷贝至工程内
            string tempGameFile = string.Format("{0}/{1}.cs", dataCachePath, TableName);
            File.Copy(tempGameFile, gameFile, true);
        }

        GUILayout.BeginHorizontal();
        ServerPath = EditorGUILayout.TextField("Server Path:", ServerPath);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Copy To Server"))
        {
            string targetFile = string.Format("{0}/{1}.bfbs", ServerPath, TableName);
            string sourceFile = string.Format("{0}/{1}.bfbs", dataCachePath, TableName);
            File.Copy(sourceFile, targetFile, true);
        }

        if (GUILayout.Button("Delete Temp"))
        {
            string targetPath = string.Format("{0}/DataCache", schemaPath);
            string[] targetFiles = Directory.GetFiles(targetPath);
            foreach (string targetFile in targetFiles)
            {
                File.Delete(targetFile);
            }
            Directory.Delete(targetPath);
        }
    }
}
