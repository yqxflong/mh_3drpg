using Fabric;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace LTAudio
{

    /// <summary>
    /// 音频类型
    /// </summary>
    [Flags]
    public enum enAudioType
    {
        /// <summary>
        /// 新手引导
        /// </summary>
        Guild = 1 << 0,
        /// <summary>
        /// 背景音乐
        /// </summary>
        BGM = 1 << 1,
        /// <summary>
        /// 相应的场景背景音频
        /// </summary>
        MUS = 1 << 2,
        /// <summary>
        /// 
        /// </summary>
        AMB = 1 << 3,
    }
    
    /// <summary>
    /// 释放音频内存
    /// </summary>
    public class ClearAudio : MonoBehaviour
    {
        /// <summary>
        /// 上次检查的时间
        /// </summary>
        private float m_LastCheckTime;
        /// <summary>
        /// 检查的时间
        /// </summary>
        private float m_CheckTime;
        /// <summary>
        /// 释放指定的类型
        /// </summary>
        private enAudioType m_ReleaseType;
        /// <summary>
        /// 当前的协程
        /// </summary>
        private Coroutine m_Coroutine;

        private static ClearAudio m_Instance;
        public static ClearAudio v_Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    GameObject obj = new GameObject("ClearAudio");
                    m_Instance = obj.AddComponent<ClearAudio>();
                }
                return m_Instance;
            }
        }

        /// <summary>
        /// 用于管理当前正在播放的音频的引用计数
        /// </summary>
        private Dictionary<AudioClip, int> AudioClipDic = new Dictionary<AudioClip, int>();

        /// <summary>
        /// 刷新当前音频使用的引用计数
        /// </summary>
        /// <param name="audioComponents"></param>
        private void RefreshAudioClipRef(AudioComponent[] audioComponents)
        {
            AudioClipDic.Clear();
            for (int i = 0; i < audioComponents.Length; i++)
            {
                AudioClip audioClip = audioComponents[i].AudioClip;
                if (audioClip==null)
                {
                    continue;
                }
                if (!AudioClipDic.ContainsKey(audioClip))
                {
                    AudioClipDic.Add(audioClip,0);
                }

                if (audioComponents[i].IsPlaying())
                {
                    AudioClipDic[audioClip]++;
                }
            }
        }


        /// <summary>
        /// 初始化
        /// </summary>
        public void F_Start()
        {
            m_CheckTime = 15.0f;
            m_LastCheckTime = Time.time + m_CheckTime;
            m_ReleaseType = enAudioType.BGM | enAudioType.Guild | enAudioType.MUS | enAudioType.AMB;
            //
            DontDestroyOnLoad(this.gameObject);
        }
        
        /// <summary>
        /// 释放内存里所有的
        /// </summary>
        private void Release()
        {
            AudioComponent[] audioComponents = FindObjectsOfType<AudioComponent>();
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }
            RefreshAudioClipRef(audioComponents);
            m_Coroutine = this.StartCoroutine(DelayUnload(audioComponents));
        }

        /// <summary>
        /// 释放指定音频类型内存
        /// </summary>
        /// <param name="type">音频类型</param>
        private void Release(enAudioType type)
        {
            //
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }

            AudioComponent[] audioComponents = FindObjectsOfType<AudioComponent>();
            RefreshAudioClipRef(audioComponents);
            if ((type & enAudioType.Guild) != 0)
            {
                m_Coroutine = StartCoroutine(DelayUnload(audioComponents, "Guide_"));
            }
            if ((type & enAudioType.BGM) != 0)
            {
                m_Coroutine = StartCoroutine(DelayUnload(audioComponents, "BGM_"));
            }
            if ((type & enAudioType.MUS) != 0)
            {
                m_Coroutine = StartCoroutine(DelayUnload(audioComponents, "MUS_"));
            }
            if ((type & enAudioType.AMB) != 0)
            {
                m_Coroutine = StartCoroutine(DelayUnload(audioComponents, "Amb_"));
            }
            //默认清掉其他杂的音频
            //StartCoroutine(DelayUnload(audioComponents, "SFX_Character_Common", false));
        }

        

        /// <summary>
        /// 延时释放音频内存
        /// </summary>
        /// <param name="audioComponents">音频文件列表</param>
        /// <param name="name">指定的音频文件名称</param>
        /// <param name="isContains">是否包含这个名称</param>
        /// <returns></returns>
        IEnumerator DelayUnload(AudioComponent[] audioComponents)
        {
            for (int i = 0; i < audioComponents.Length; i++)
            {
                if (audioComponents[i] != null && audioComponents[i].AudioClip != null && audioComponents[i]._isComponentActive == false && AudioClipDic[audioComponents[i].AudioClip]==0)
                {
                    audioComponents[i].AudioClip.UnloadAudioData();
                    audioComponents[i] = null;
                    yield return null;
                }
            }
            yield return null;
            m_LastCheckTime = Time.time + m_CheckTime;
            //
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }
        }


        void Update()
        {
            if (Time.time - m_LastCheckTime >= m_CheckTime)
            {
                m_LastCheckTime = Time.time + m_CheckTime;
                Release(m_ReleaseType);
            }
        }
        


        /// <summary>
        /// 延时释放音频内存
        /// </summary>
        /// <param name="audioComponents">音频文件列表</param>
        /// <param name="name">指定的音频文件名称</param>
        /// <param name="isContains">是否包含这个名称</param>
        /// <returns></returns>
        IEnumerator DelayUnload(AudioComponent[] audioComponents,string name,bool isContains = true)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < audioComponents.Length; i++)
            {
                //Debug.LogError("unload audioClip : "+ audioComponents[i].AudioClip.name + " from : "+ audioComponents[i].name + "ref : "+ AudioClipDic[audioComponents[i].AudioClip]);
                if (audioComponents[i] != null && audioComponents[i].AudioClip != null 
                    && audioComponents[i].name.Contains(name) == isContains
                    && audioComponents[i]._isComponentActive == false
                    &&AudioClipDic[audioComponents[i].AudioClip] == 0)
                {
                    audioComponents[i].AudioClip.UnloadAudioData();
                    audioComponents[i] = null;
                    yield return null;
                }
            }
            yield return null;
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }
        }

    }

}
