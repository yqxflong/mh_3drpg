using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ResourcesChecker : ResChecker
{
    const string s_Dir = "Assets/Resources/";

    private void GetFileFromDir(string fileReg, string errorName, ref List<string> error)
    {
        var objs = Directory.GetFiles(s_Dir, fileReg, SearchOption.AllDirectories).ToList<string>();

        foreach (var obj in objs)
        {
            error.Add(string.Format("Resource下面不应该包括{0}文件：{1}", errorName, obj));
        }

    }

    public ResCheckResult Check(ResCheckerCallBack callbacker)
    {

        callbacker.BeginCheck(this.Name(), 1);
        var error = new List<string>();

        GetFileFromDir("*.tga", "tga", ref error);
        GetFileFromDir("*.png", "png", ref error);
        GetFileFromDir("*.jpg", "jpg", ref error);
        GetFileFromDir("*.gif", "gif", ref error);
        GetFileFromDir("*.mat", "mat", ref error);
        GetFileFromDir("*.shader", "shader", ref error);

        callbacker.OnCheckEnd();
        return new ResCheckResult(Name(), error);
    }

    public string Name()
    {
        return "ResourcesChecker";
    }
}
