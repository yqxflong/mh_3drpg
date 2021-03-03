//LocalizationEditor
//用于处理多语言相关的工具
//Johny

using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class LocalizationEditor
{
    private const string ATLAS_SEARCHPATH = "Assets/_GameAssets/Res/Textures/UI/Atlas";
    private const string ATLAS_DSTPATH = "Assets/_GameAssets/Res/Textures/Language/Atlas";
    private const string TEX_DSTPATH = "Assets/_GameAssets/Res/Textures/Language/Textures";

    private static HashSet<string> LangKeys = new HashSet<string>()
    {
        "CN",//简体为默认，自己管理，删除会导致预设引用miss
        "EN",
        "TW"
    };

    #region GenLanguageAtlas
    private static List<DirectoryInfo> langDirs = new List<DirectoryInfo>();

    [MenuItem("Tools/Localization/GenLanguageAtlas")]
    public static void GenLanguageAtlas()
    {
        //GenLanguageAtlas_ClearDstFiles();//不做删除只更新或创建
        DirectoryInfo dirInfo = new DirectoryInfo(ATLAS_SEARCHPATH);
        langDirs.Clear();
        GenLanguageAtlas_Recursive(dirInfo);
        foreach(var theDir in langDirs)
        {
            GenOneAtlas(theDir, theDir.Name);
        }
    }

    private static void GenLanguageAtlas_ClearDstFiles()
    {
        DirectoryInfo dirInfo_dst = new DirectoryInfo(ATLAS_DSTPATH);
        foreach(var theFile in dirInfo_dst.GetFiles())
        {
            theFile.Delete();
        }
    }

    private static void GenOneAtlas(DirectoryInfo theDir, string theDirName)
    {
        List<Texture> texs = new List<Texture>();
        foreach (var theFile in theDir.GetFiles())
        {
            if (theFile.Extension == ".meta")
            {
                continue;
            }

            string fileFullName = theFile.FullName;
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(theFile.Name);
            int flagIndex_file = fileNameWithoutExt.LastIndexOf('_');
            if (flagIndex_file != -1)
            {
                string flag_file = fileNameWithoutExt.Substring(flagIndex_file + 1);
                if (LangKeys.Contains(flag_file))//不需要打大图，直接移动到指定位置
                {
                    string filePath_dst = TEX_DSTPATH + "/" + theFile.Name;
                    if(!File.Exists(filePath_dst))
                    {
                        Debug.LogErrorFormat("图片{0}，需自行移动到{1}路径下！不然预设上的引用关联不上！", theFile.Name, TEX_DSTPATH);
                        //theFile.CopyTo(filePath_dst);
                    }
                    continue;
                }
            }

            int startIdx = fileFullName.IndexOf("Assets");
            string filePath = fileFullName.Substring(startIdx);
            Texture tex = AssetDatabase.LoadAssetAtPath<Texture>(filePath);
            texs.Add(tex);
        }
        if (texs.Count > 1)
        {
            NGUISettings.atlas = null;
            bool replace = false;
            string theDirPath = ATLAS_DSTPATH + "/" + theDirName + ".prefab";
            if (File.Exists(theDirPath))
            {
                GameObject go = AssetDatabase.LoadAssetAtPath(theDirPath, typeof(GameObject)) as GameObject;
                NGUISettings.atlas = go.GetComponent<UIAtlas>();
            }
            else
            {
                UIAtlasMaker.CreateAtlas(theDirPath, ref replace);
            }
            UIAtlasMaker.UpdateAtlas(texs, false);
        }
        else if(texs.Count == 1)
        {
            Debug.LogErrorFormat("{0}下仅有一张图，不打图集，请加后缀改为单图并移动到{1}路径下！", theDirName,TEX_DSTPATH);
        }
    }

    private static void GenLanguageAtlas_Recursive(DirectoryInfo dirInfo)
    {
        foreach(var theDir in dirInfo.GetDirectories())
        {
            GenLanguageAtlas_Recursive(theDir);
            string theDirName = theDir.Name;
            int flagIndex = theDirName.LastIndexOf('_');
            if(flagIndex != -1)
            {
                string flag = theDirName.Substring(flagIndex + 1);
                if (LangKeys.Contains(flag))
                {
                    langDirs.Add(theDir);
                }
            }
        } 
    }
    #endregion
}
