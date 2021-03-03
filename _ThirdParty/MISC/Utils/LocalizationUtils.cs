using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class LocalizationUtils
{
    public static EB.Language[] Languages = new EB.Language[]
    {
        EB.Language.English,        
        EB.Language.ChineseSimplified,
        EB.Language.ChineseTraditional,
        EB.Language.French,
        //EB.Language.Italian,
        EB.Language.German,
        //EB.Language.Spanish,
        //EB.Language.Portuguese,
        //EB.Language.Russian,
        //EB.Language.Korean,
    };

    static Dictionary<string, EB.LocFile> _files = new Dictionary<string, EB.LocFile>();

    public const string LocSourceFolder = "Assets/Resources/Languages/en";

    public static string[] knownLanguages
    {
        get { return Languages.Select(locale => EB.Localizer.GetSparxLanguageCode(locale)).ToArray(); }
    }

    static Dictionary<string, string[]> _dict = null;
    public static Dictionary<string, string[]> dictionary
    {
        get
        {
            if (_dict == null)
            {
                _dict = new Dictionary<string, string[]>();
                Dictionary<EB.Language, Dictionary<string, string>> files = new Dictionary<EB.Language, Dictionary<string, string>>();
                foreach (var locale in Languages)
                {
                    files.Add(locale, LoadAllFromResources(locale, new string[] { "all" }));
                }

                for (int i = 0; i < Languages.Length; ++i)
                {
                    var file = files[Languages[i]];
                    foreach (string key in file.Keys)
                    {
                        if (_dict.ContainsKey(key))
                        {
                            continue;
                        }

                        string[] strs = new string[Languages.Length];
                        _dict.Add(key, strs);
                        for (int j = 0; j < i; ++j)
                        {
                            strs[j] = "Not Translated: " + key;
                        }
                        for (int j = i; j < Languages.Length; ++j)
                        {
                            var locale = Languages[j];
                            if (files[locale].ContainsKey(key) && !string.IsNullOrEmpty(files[locale][key]))
                            {
                                strs[j] = files[locale][key];
                            }
                            else
                            {
                                strs[j] = "Not Translated: " + key;
                            }
                        }
                    }
                }
            }
            return _dict;
        }
        set
        {
            _dict = value;
        }
    }

    public static Dictionary<string, string> LoadAllFromResources(EB.Language locale, string[] locFiles)
    {
        var l = EB.Localizer.GetSparxLanguageCode(locale);
        var dict = new Dictionary<string, string>();
        var file = new EB.LocFile(dict);

        foreach (string name in locFiles)
        {
            file.Read(File.ReadAllText(Path.Combine(Path.GetDirectoryName(LocSourceFolder), l + "/" + name + ".txt")));
        }

        return dict;
    }

    class LocWatcher : UnityEditor.AssetModificationProcessor
    {
        public static void OnWillSaveAssets(string[] paths)
        {
            foreach (string path in paths)
            {
                if (path.StartsWith(path, System.StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }
            SaveDbs();
        }
    }

    public static void SaveDbs()
    {
        if (_files.Count == 0)
        {
            return;
        }

       EB.Debug.Log("Saving Loc dbs");
        foreach (var kvp in _files)
        {
            var file = kvp.Value.Write();
            var name = kvp.Key;
            File.WriteAllText(Path.Combine(LocSourceFolder, name + ".txt"), file);
        }
        _files.Clear();
        AssetDatabase.Refresh();
    }

    public static EB.LocFile GetLocDb(string name)
    {
        EB.LocFile file = null;
        if (!_files.TryGetValue(name, out file))
        {
            file = new EB.LocFile();
            try
            {
                file.Read(File.ReadAllText(Path.Combine(LocSourceFolder, name + ".txt")));
            }
            catch
            {
                //Debug.LogError("Failed to load db " + name);
            }
            _files[name] = file;
        }
        return file;
    }

    public static bool LocTextField(string name, string locId, string database, params GUILayoutOption[] options)
    {
        var db = GetLocDb(database);
        var str = string.Empty;
        db.Get(locId, out str);

        var res = EditorGUILayout.TextField(name, str, options);
        if (res != str)
        {
            db.Add(locId, res);
            return true;
        }
        return false;
    }

    public static bool UploadTranslations()
    {
        string stoken = WWWUtils.AdminLogin();
        if (string.IsNullOrEmpty(stoken))
        {
            return false;
        }

        var url = WWWUtils.AdminUrl("/localization/upload/english");
        EB.Localizer.Clear();
        var result = LoadAllFromResources(EB.Language.English, new string[] { "all" });
        // fixup all the \n
        var data = new Hashtable();
        foreach (var entry in result)
        {
            data[entry.Key] = entry.Value.Trim().Replace("\n", "\\n");
        }

       EB.Debug.Log(" source string count: " + data.Count);

        var form = new WWWForm();
        form.AddField("body", EB.JSON.Stringify(data));
        form.AddField("stoken", stoken);
        form.AddField("format", "json");
        return WWWUtils.PostJson(url, form) != null;
    }

    public static void ExportTSV()
    {
        var tmp = new List<EB.Language>();
        tmp.Add(EB.Language.English);
        tmp.AddRange(Languages);

        foreach (var locale in tmp)
        {
            EB.LocFile file = new EB.LocFile(LoadAllFromResources(locale, new string[] { "all" }));
            var contents = file.WriteTSV();
            File.WriteAllText(locale + ".txt", "\r\n" + contents, System.Text.Encoding.Unicode);
        }
    }

    public static Dictionary<EB.Language, string> DownloadTranslations()
    {
        string stoken = WWWUtils.AdminLogin();
        if (string.IsNullOrEmpty(stoken))
        {
            return null;
        }

        var transData = new Dictionary<EB.Language, string>();

        var langs = new List<EB.Language>();
        langs.Add(EB.Language.English);
        langs.Add(EB.Language.ChineseSimplified);
        langs.Add(EB.Language.ChineseTraditional);

        foreach (var lang in langs)
        {
            var code = EB.Localizer.GetSparxLanguageCode(lang);
            var url = WWWUtils.AdminUrl(string.Format("/localization/export/{0}?stoken={1}&format=csv&status=1&code=1", code, stoken));
            var csv = WWWUtils.Get(url);
            if (string.IsNullOrEmpty(csv))
            {
                Debug.LogWarning("LocalizationUtils.WWWUtils.Get(" + url + ") = null");
                continue;
            }
            csv = csv.Replace(@"\\n", @"\n");
            transData.Add(lang, csv);
        }

        return transData;
    }

    public static void PatchTranslationsToResources(Dictionary<EB.Language, string> transData)
    {
        foreach (var csv in transData)
        {
            //Convert Server Data
            var remote = new Dictionary<string, string>();
            var remoteFile = new EB.LocFile(remote);
            remoteFile.Read(csv.Value);

            //Get Local Data
            var local = LoadAllFromResources(csv.Key, new string[] { "all" });

            //Merge
            foreach (var entry in remote)
            {
                if (local.ContainsKey(entry.Key))
                {
                    local[entry.Key] = entry.Value;
                }
                else
                {
                    local.Add(entry.Key, entry.Value);
                }
            }

            var localFile = new EB.LocFile(local.OrderBy(entry => entry.Key).ToDictionary(entry => entry.Key, entry => entry.Value));
            var bytes = EB.Encoding.GetBytes(localFile.Write());

            var code = EB.Localizer.GetSparxLanguageCode(csv.Key);
            var dir = "Assets/Resources/Languages/" + code;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllBytes(Path.Combine(dir, "all.txt"), bytes);
        }

        AssetDatabase.Refresh();
    }

    public static void CoverTranslationsToResources(Dictionary<EB.Language, string> transData)
    {
        foreach (var csv in transData)
        {
            if (string.IsNullOrEmpty(csv.Value))
            {
                Debug.LogWarningFormat("CoverTranslations: content is empty for {0}", csv.Key);
                continue;
            }

            var code = EB.Localizer.GetSparxLanguageCode(csv.Key);
            var dir = "Assets/Resources/Languages/" + code;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var bytes = EB.Encoding.GetBytes(csv.Value);
            File.WriteAllBytes(Path.Combine(dir, "all.txt"), bytes);
        }

        AssetDatabase.Refresh();
    }

    public static void PatchTranslationsToBundles(Dictionary<EB.Language, string> transData)
    {


        foreach (var csv in transData)
        {
            //Convert Server Data
            var remote = new Dictionary<string, string>();
            var remoteFile = new EB.LocFile(remote);
            remoteFile.Read(csv.Value);

            //Get Local Data
            var local = LoadAllFromResources(csv.Key, new string[] { "all" });

            //Patch
            foreach (var entry in remote)
            {
                if (local.ContainsKey(entry.Key))
                {
                    local[entry.Key] = entry.Value;
                }
                else
                {
                    local.Add(entry.Key, entry.Value);
                }
            }

            var localFile = new EB.LocFile(local.OrderBy(entry => entry.Key).ToDictionary(entry => entry.Key, entry => entry.Value));
            var bytes = EB.Encoding.GetBytes(localFile.Write());

            var code = EB.Localizer.GetSparxLanguageCode(csv.Key);
            var dir = "Assets/Bundles/Languages";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllBytes(Path.Combine(dir, code + ".txt"), bytes);
        }

        AssetDatabase.Refresh();
    }

    public static void BuildTranslationsToBundles()
    {
        foreach (var locale in Languages)
        {
            var local = LoadAllFromResources(locale, new string[] { "all" });

            var localFile = new EB.LocFile(local.OrderBy(entry => entry.Key).ToDictionary(entry => entry.Key, entry => entry.Value));
            var bytes = EB.Encoding.GetBytes(localFile.Write());

            var code = EB.Localizer.GetSparxLanguageCode(locale);
            var dir = "Assets/Bundles/Languages";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllBytes(Path.Combine(dir, code + ".txt"), bytes);
        }
    }
}
