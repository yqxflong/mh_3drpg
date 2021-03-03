using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LogObjectPath
{
    [MenuItem("GameObject/LogPath/Select Scene Object Path", priority = 0)]
    static void SelectSceneObjectPath()
    {
        GameObject go = Selection.activeObject as GameObject;
        string path = "";
        if (go!=null)
        {
            path = go.name;
            path = GetParentPath(path,go.transform.parent);
        }
        Debug.Log(path);
    }

    static string GetParentPath( string path, Transform t)
    {
        if(t!=null)
        {
            path = t.name + "/" + path;
            path = GetParentPath(path, t.parent);
        }
        return path;
    }
}
