// CameraShakePreferences.cs
// Copyright (c) 2012-2016 Thinksquirrel Software, LLC.

using System;
using System.Diagnostics;
using System.IO;
using Ionic.Zip;
using Thinksquirrel.CShake.Internal;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Thinksquirrel.CShakeEditor
{
    [InitializeOnLoad]
    static class CameraShakePreferences
    {
        #region Static and Constant Fields
        static bool s_PreferencesLoaded;
        static readonly GUIContent s_Label = new GUIContent();
        static DocumentationSource s_DocumentationSource = DocumentationSource.Online;
        static string s_DocumentationPath;
        static string s_ShortPath;
        static bool s_DownloadEveryUpdate;
        static WWW s_Www;
        static bool s_ShowProgress;
        static bool s_FinishedInstall;
        static bool s_Canceled;
        static string s_Progress;
        static Color s_ProgressColor = Color.white;
        static int s_FrameCount;
        #endregion

        #region Public API
        public enum DocumentationSource
        {
            Online = 0,
            Local
        }
        #endregion

        #region Constructors
        static CameraShakePreferences()
        {
            // Check for first run
            var firstRun = EditorPrefs.GetBool("Thinksquirrel.CShakeEditor.FirstRun_" + VersionInfo.version, true);

            // If first run, check to download docs
            if (firstRun)
            {
                var downloadDocs = EditorPrefs.GetBool("Thinksquirrel.CShakeEditor.DownloadDocsEveryUpdate", false);

                if (downloadDocs)
                {
                    LoadPreferences();

                    var documentationExists = Directory.Exists(Path.Combine(s_DocumentationPath, VersionInfo.version));

                    if (!documentationExists)
                    {
                        DownloadDocumentation();
                    }
                }
            }

            // Set first run flags
            EditorPrefs.SetBool("Thinksquirrel.CShakeEditor.FirstRun_" + VersionInfo.version, false);
        }
        #endregion

        internal static string ComponentUrl(Type type)
        {
            if (!s_PreferencesLoaded)
                LoadPreferences();

            switch (s_DocumentationSource)
            {
                case DocumentationSource.Local:
                    return new Uri(
                        Path.Combine(Path.Combine(s_DocumentationPath, VersionInfo.version),
                                     CameraShakeEditorHelpers.ComponentUrl(type, false))).AbsoluteUri;
                default:
                    return CameraShakeEditorHelpers.ComponentUrl(type);
            }
        }
        internal static void DisplayDocumentationProgress()
        {
            if (s_ShowProgress)
            {
                var c = GUI.color;
                GUI.color = s_ProgressColor;
                GUILayout.Label(s_Progress, EditorStyles.wordWrappedLabel);
                GUI.color = c;
            }
        }
        internal static void DownloadDocumentation(bool fromFirstRunWindow = false)
        {
            if (fromFirstRunWindow)
            {
                s_DownloadEveryUpdate = true;
                s_DocumentationSource = DocumentationSource.Local;
                EditorPrefs.SetInt("Thinksquirrel.CShakeEditor.DocumentationSource", (int)s_DocumentationSource);
                EditorPrefs.SetString("Thinksquirrel.CShakeEditor.DocumentationPath", s_DocumentationPath);
                EditorPrefs.SetBool("Thinksquirrel.CShakeEditor.DownloadDocsEveryUpdate", s_DownloadEveryUpdate);
            }

            s_ShowProgress = true;
            s_FinishedInstall = false;
            s_Progress = "Downloading documentation...\n";
            s_ProgressColor = Color.white;
            s_Www = new WWW(CameraShakeEditorHelpers.ArchiveUrl());
            s_Canceled = false;
            EditorApplication.update += UpdateProgress;
            EditorApplication.update += UpdateProgressBar;
        }
        internal static string ReferenceManualUrl()
        {
            if (!s_PreferencesLoaded)
                LoadPreferences();

            switch (s_DocumentationSource)
            {
                case DocumentationSource.Local:
                    return new Uri(
                        Path.Combine(Path.Combine(s_DocumentationPath, VersionInfo.version), "index.html")).AbsoluteUri;
                default:
                    return CameraShakeEditorHelpers.ReferenceManualUrl();
            }
        }
        internal static string ScriptingUrl(Type type)
        {
            if (!s_PreferencesLoaded)
                LoadPreferences();

            switch (s_DocumentationSource)
            {
                case DocumentationSource.Local:
                    return new Uri(
                        Path.Combine(Path.Combine(s_DocumentationPath, VersionInfo.version),
                                     CameraShakeEditorHelpers.ScriptingUrl(type, false))).AbsoluteUri;
                default:
                    return CameraShakeEditorHelpers.ScriptingUrl(type);
            }
        }
        static void CancelProgress(string errorString)
        {
            s_ProgressColor = Color.red;
            s_Progress = string.Format("Error extracting documentation\n\n{0}", errorString);
            s_FinishedInstall = true;
            EditorApplication.update -= UpdateProgress;
        }
        static void LoadPreferences()
        {
            s_DocumentationSource =
                (DocumentationSource)EditorPrefs.GetInt("Thinksquirrel.CShakeEditor.DocumentationSource", 0);
            s_DocumentationPath = EditorPrefs.GetString("Thinksquirrel.CShakeEditor.DocumentationPath");
            s_DownloadEveryUpdate = EditorPrefs.GetBool("Thinksquirrel.CShakeEditor.DownloadDocsEveryUpdate", false);

            if (string.IsNullOrEmpty(s_DocumentationPath))
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.OSXEditor:
                        s_DocumentationPath =
                            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                         "Camera Shake/Documentation");
                        if (!Directory.Exists(s_DocumentationPath))
                            Directory.CreateDirectory(s_DocumentationPath);
                        break;
                    case RuntimePlatform.WindowsEditor:
                        s_DocumentationPath =
                            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                         "Camera Shake/Documentation");
                        if (!Directory.Exists(s_DocumentationPath))
                            Directory.CreateDirectory(s_DocumentationPath);
                        break;
                }
            }

            s_ShortPath = new DirectoryInfo(s_DocumentationPath).Name;
            s_PreferencesLoaded = true;
        }

        // ReSharper disable once MethodTooLong       
        [PreferenceItem("Camera Shake")]
        static void PreferencesGui()
        {
            if (!s_PreferencesLoaded)
                LoadPreferences();

            // Documentation source
            s_DocumentationSource =
                (DocumentationSource)
                    EditorGUILayout.EnumPopup(
                        TempContent("Documentation Source", "The source for all documentation links."),
                        s_DocumentationSource);

            if (s_DocumentationSource != DocumentationSource.Local) return;

            // Documentation path
            GUILayout.BeginHorizontal();
            EditorGUILayout.TextField(
                TempContent("Documentation Path", "The path where local documentation should be saved."), s_ShortPath);
            var e = GUI.enabled;
            GUI.enabled = !(s_ShowProgress && !s_FinishedInstall);
            if (GUILayout.Button("...", GUILayout.ExpandWidth(false)))
            {
                var newPath = EditorUtility.OpenFolderPanel("Select folder", s_DocumentationPath, string.Empty);

                if (!string.IsNullOrEmpty(newPath))
                {
                    s_DocumentationPath = newPath;
                    s_ShortPath = new DirectoryInfo(s_DocumentationPath).Name;
                    s_ShowProgress = false;
                }
            }
            GUI.enabled = e;
            GUILayout.EndHorizontal();

            // Download documentation with every update
            EditorGUI.BeginChangeCheck();
            var downloadEveryUpdate =
                EditorGUILayout.Toggle(
                    TempContent("Download with Every Update",
                                "If enabled, documentation will be downloaded with every CameraShake update."),
                    s_DownloadEveryUpdate);
            if (EditorGUI.EndChangeCheck() && downloadEveryUpdate != s_DownloadEveryUpdate)
            {
                s_DownloadEveryUpdate = downloadEveryUpdate;
            }

            var c = GUI.color;

            // Download documentation
            var documentationExists = Directory.Exists(Path.Combine(s_DocumentationPath, VersionInfo.version));
            if (!documentationExists) GUI.color = Color.red;
            s_Label.text = documentationExists ? "Update" : "Download";
            s_Label.tooltip = documentationExists
                ? "Update the local documentation from the server"
                : "Download local documentation from the server";
            if (GUILayout.Button(s_Label)) DownloadDocumentation();

            GUI.color = c;

            DisplayDocumentationProgress();

            if (!GUI.changed) return;

            EditorPrefs.SetInt("Thinksquirrel.CShakeEditor.DocumentationSource", (int)s_DocumentationSource);
            EditorPrefs.SetString("Thinksquirrel.CShakeEditor.DocumentationPath", s_DocumentationPath);
            EditorPrefs.SetBool("Thinksquirrel.CShakeEditor.DownloadDocsEveryUpdate", s_DownloadEveryUpdate);
        }
        static GUIContent TempContent(string text, string tooltip)
        {
            s_Label.text = text;
            s_Label.tooltip = tooltip;

            return s_Label;
        }
        static void UpdateProgress()
        {
            if (!string.IsNullOrEmpty(s_Www.error))
            {
                CancelProgress(s_Www.error);
            }
            else if (s_Www.isDone)
            {
                if (s_Canceled)
                {
                    CancelProgress("Canceled by user");
                    return;
                }

                try
                {
                    using (var inputStream = new MemoryStream(s_Www.bytes))
                    {
                        using (var zip = ZipFile.Read(inputStream))
                        {
                            var archivePath = Path.Combine(s_DocumentationPath, VersionInfo.version);
                            if (!Directory.Exists(archivePath)) Directory.CreateDirectory(archivePath);

                            zip.ExtractAll(archivePath, ExtractExistingFileAction.OverwriteSilently);
                        }
                    }

                    s_ProgressColor = Color.green;
                    s_Progress = "Download complete.";
                    s_FinishedInstall = true;
                    EditorApplication.update -= UpdateProgress;
                }
                catch (Exception e)
                {
                    CancelProgress(string.Format("An exception was thrown ({0}): {1} at {2}", e.GetType(), e.Message,
                                                 e.StackTrace));
                }
            }
            else
            {
                if (s_Canceled)
                {
                    CancelProgress("Canceled by user");
                    return;
                }
                s_Progress += ".";
            }
        }
        static void UpdateProgressBar()
        {
            string infoString;
            float progress;

            if (!string.IsNullOrEmpty(s_Www.error))
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError("Error downloading CameraShake documentation: " + s_Www.error);
                EditorApplication.update -= UpdateProgressBar;
            }
            else if (s_Www.isDone)
            {
                infoString = "Extracting documentation";
                progress = 1;

                EditorUtility.DisplayProgressBar("Installing documentation", infoString, progress);

                if (!s_FinishedInstall) return;

                EditorUtility.ClearProgressBar();
                EditorApplication.update -= UpdateProgressBar;
            }
            else
            {
                infoString = "Downloading documentation";
                progress = s_Www.progress;

                if (!EditorUtility.DisplayCancelableProgressBar("Installing documentation", infoString, progress))
                    return;

                EditorUtility.ClearProgressBar();
                Debug.LogError("Error downloading CameraShake documentation: Canceled by user");
                s_Canceled = true;
                s_FinishedInstall = true;
                EditorApplication.update -= UpdateProgressBar;
            }
        }
    }
}
