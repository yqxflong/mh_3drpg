using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class LocalizationEditor : EditorWindow
{
    public int mToolbarOption = 0;
    public string[] mToolbarTexts = { /*"Upload Translations",*/ "Export TSV", "Download Translations", "Build To Bundle" };

    private enum DownloadType
    {
        CoverToResources = 0,
        PatchToResources = 1,
        //PatchToBundles = 2,
    };

    private DownloadType mDownloadType = DownloadType.CoverToResources;

    [MenuItem("EBG/Localizer")]
    public static void OpenLocalizationEditor()
    {
        EditorWindow.GetWindow(typeof(LocalizationEditor));
    }

    void OnEnable()
    {
        LoadLogin();
    }

    void OnGUI()
    {
        mToolbarOption = GUILayout.Toolbar(mToolbarOption, mToolbarTexts);
        switch (mToolbarOption)
        {
            /*case 0:
                CreateUploadTranslationsWindow();
                break;
            case 1:
                CreateExportTSVWindow();
                break;
            case 2:
                CreateDownloadTranslations();
                break;
            case 3:
                CreateBuildToBundleWindow();
                break;*/
            case 0:
                CreateExportTSVWindow();
                break;
            case 1:
                CreateDownloadTranslations();
                break;
            case 2:
                CreateBuildToBundleWindow();
                break;
        }
    }

    private void CreateUploadTranslationsWindow()
    {
        CreateLoginWindow();

        if (GUILayout.Button("UpLoad"))
        {
            bool success = LocalizationUtils.UploadTranslations();
            SaveLogin(success);
        }
    }

    private void CreateExportTSVWindow()
    {
        if (GUILayout.Button("ExportTSV"))
        {
            LocalizationUtils.ExportTSV();
        }
    }

    private void CreateDownloadTranslations()
    {
        CreateLoginWindow();

        mDownloadType = (DownloadType)EditorGUILayout.EnumPopup("Option:", mDownloadType);

        if (GUILayout.Button("DownLoad"))
        {
            Dictionary<EB.Language, string> transData = LocalizationUtils.DownloadTranslations();
            SaveLogin(transData != null);

            if (transData != null)
            {
                if (mDownloadType == DownloadType.CoverToResources)
                {
                    LocalizationUtils.CoverTranslationsToResources(transData);
                }
                else if (mDownloadType == DownloadType.PatchToResources)
                {
                    LocalizationUtils.PatchTranslationsToResources(transData);
                }
                /*else if (mDownloadType == DownloadType.PatchToBundles)
                {
                    LocalizationUtils.PatchTranslationsToBundles(transData);
                }*/
            }
            else
            {
                Debug.LogWarning("LocalizationEditor.CreateDownloadTranslations DownLoad Failed!");
            }
        }
    }

    private void CreateBuildToBundleWindow()
    {
        if (GUILayout.Button("BuildToBundle"))
        {
            LocalizationUtils.BuildTranslationsToBundles();
        }
    }

    private void CreateLoginWindow()
    {
        GUILayout.BeginHorizontal();
        WWWUtils.Environ = (WWWUtils.Environment)EditorGUILayout.EnumPopup("Server:", WWWUtils.Environ);
        if (WWWUtils.Environ == WWWUtils.Environment.Custom)
        {
            WWWUtils.ServerConfig[(int)WWWUtils.Environ] = EditorGUILayout.TextField(WWWUtils.ServerConfig[(int)WWWUtils.Environ]);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        WWWUtils.Email = EditorGUILayout.TextField("Email:", WWWUtils.Email);
        WWWUtils.Password = EditorGUILayout.PasswordField("PassWord:", WWWUtils.Password);
        GUILayout.EndHorizontal();
    }

    private void SaveLogin(bool success)
    {
        if (success)
        {
            EditorPrefs.SetInt("Localization_ServerNum", (int)WWWUtils.Environ);
            EditorPrefs.SetString("Localization_Email", WWWUtils.Email);
            EditorPrefs.SetString("Localization_PassWord", WWWUtils.Password);
        }
        else
        {
            EditorPrefs.DeleteKey("Localization_ServerNum");
            EditorPrefs.DeleteKey("Localization_Email");
            EditorPrefs.DeleteKey("Localization_PassWord");
        }
    }

    private void LoadLogin()
    {
        WWWUtils.Environ = (WWWUtils.Environment)EditorPrefs.GetInt("Localization_ServerNum");
        WWWUtils.Email = EditorPrefs.GetString("Localization_Email");
        WWWUtils.Password = EditorPrefs.GetString("Localization_PassWord");
    }
}
