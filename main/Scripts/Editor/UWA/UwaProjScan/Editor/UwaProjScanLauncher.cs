using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UwaProjScan.Tools;

namespace UwaProjScan
{
    class ApiCompatibilityImp : ICompatApi
    {
        public static readonly ApiCompatibilityImp Instance = new ApiCompatibilityImp();
        public ApiCompatibilityImp()
        {
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += EditorApplication_playmodeStateChanged;
#else
            EditorApplication.playmodeStateChanged += EditorApplication_playmodeStateChanged;
#endif
        }

        private Action _exitplaymodecb = null;
        private Action<bool> _pausemodecb = null;
        private bool _lastPauseState = false;
#if UNITY_2017_2_OR_NEWER
        public void EditorApplication_playmodeStateChanged(PlayModeStateChange p)
#else
        public void EditorApplication_playmodeStateChanged()
#endif
        {
            if (EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (_exitplaymodecb != null) _exitplaymodecb();
            }

            // invoke only when isPaused changed
            if (_lastPauseState != EditorApplication.isPaused)
            {
                if (_pausemodecb != null) _pausemodecb(EditorApplication.isPaused);
            }

            _lastPauseState = EditorApplication.isPaused;
        }

        #region interface imp
        public void RegisterEditorExitPlayMode(Action cb)
        {
            _exitplaymodecb += cb;
        }

        public void RegisterEditorPauseMode(Action<bool> cb)
        {
            _pausemodecb += cb;
        }
        #endregion

    }

    [InitializeOnLoad]
    public static class MainScan
    {
        static MainScan()
        {
            ApiCompatibilityUtils.Instance.Setup(ApiCompatibilityImp.Instance);
        }

        [MenuItem("Tools/UWA Scan/Run", false, 1)]
        public static void DoTestFromMenu()
        {
            API.DoMain(true);
        }

        /// <summary>
        /// 用户通过命令行执行时，调用的是该函数
        /// </summary>
        public static void DoTest()
        {
            API.DoMain(false);
        }

#if UNITY_5_3_OR_NEWER
        [MenuItem("Tools/UWA Scan/Check Effects Scanning", false, 2)]
        public static void EffectRuleCheck()
        {
            API.EffectRuleCheck();
        }
#endif

        [MenuItem("Tools/UWA Scan/Check Custom Rules", false, 2)]
        public static void CustomRuleCheck()
        {
            API.CustomRuleCheck();
        }
    }
}
