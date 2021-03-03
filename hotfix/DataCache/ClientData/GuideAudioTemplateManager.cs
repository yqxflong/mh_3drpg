using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Data
{
    public class GuideAudioTemplateManager
    {

        private Hashtable m_Audios;
        public Hashtable Audios
        {
            get
            {
                return m_Audios;
            }
        }
        public Dictionary<string, string> mDLGAudio = new Dictionary<string, string>();
        public Dictionary<string, string> mGDEAudio = new Dictionary<string, string>();
        public Dictionary<string, string> mDLGBGM = new Dictionary<string, string>();
        private List<GuideAudioTemplate> GuideAudios = new List<GuideAudioTemplate>();
        GM.DataCache.ConditionGuide conditionSet;

        private static GuideAudioTemplateManager m_Instance;
        public static GuideAudioTemplateManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new GuideAudioTemplateManager();
                }
                return m_Instance;
            }
        }

        private GuideAudioTemplateManager()
        {
            m_Audios = Johny.HashtablePool.Claim();
        }

        public static void ClearUp()
        {
            if (m_Instance != null)
            {
                m_Instance.mDLGAudio.Clear();
                m_Instance.mGDEAudio.Clear();
                m_Instance.mDLGBGM.Clear();
                m_Instance.GuideAudios.Clear();
            }
        }

        public void InitGuideAudioData(GM.DataCache.ConditionGuide data)
        {
            if (data == null) return;
            conditionSet = data;
        }
        public void InitGuideAudioDataEx()
        {
            ClearUp();
            for (int i = 0; i < conditionSet.GuideAudioLength; i++)
            {
                var node = conditionSet.GetGuideAudio(i);
                GuideAudioTemplate tpl = new GuideAudioTemplate();
                tpl.dialogue_id = node.DialogId;
                tpl.dialog_bgm = node.DialogBgm;
                tpl.dialogue_step_id = node.DialogStepId;
                tpl.event_name = node.EventName.Replace("_", "/");
                tpl.guide_node_step_id = node.GuideId;
                if (tpl.dialogue_id != 0)
                {
                    string DLGid = tpl.dialogue_id.ToString() + "/" + tpl.dialogue_step_id.ToString();
                    if (!mDLGAudio.ContainsKey(DLGid))
                    {
                        mDLGAudio.Add(DLGid, tpl.event_name);
                    }
                }

                if (tpl.guide_node_step_id != 0)
                {
                    //Debug.LogError("Add Guide : " + tpl.guide_node_step_id + " Name :"+ tpl.event_name);
                    string GDEid = tpl.guide_node_step_id.ToString();
                    if (!mGDEAudio.ContainsKey(GDEid))
                    {
                        mGDEAudio.Add(tpl.guide_node_step_id.ToString(), tpl.event_name);
                    }
                }

                if (tpl.dialog_bgm != null)
                {
                    string DLGid = tpl.dialogue_id.ToString() + "/" + tpl.dialogue_step_id.ToString();
                    if (!mDLGBGM.ContainsKey(DLGid))
                    {
                        mDLGBGM.Add(DLGid, tpl.dialog_bgm);
                    }
                }
                GuideAudios.Add(tpl);
            }
        }

        private void Init()
        {
            if (GuideAudios.Count < 1)
            {
                InitGuideAudioDataEx();
            }
        }
        public string GetDLGAudio(string DLGid)
        {
            Init();
            var keyItor = mDLGAudio.Keys.GetEnumerator();
            while (keyItor.MoveNext())
            {
                var key = keyItor.Current;
                if (key == DLGid)
                {
                    return mDLGAudio[key];
                }
            }
            EB.Debug.Log("<color=#ff6699ff>Can not find Audio With DLGid: {0}" , DLGid);
            return null;
        }

        public string GetDLGBGM(string DLGid)
        {
            Init();
            var keyItor = mDLGAudio.Keys.GetEnumerator();
            while (keyItor.MoveNext())
            {
                var key = keyItor.Current;
                if (key == DLGid)
                {
                    return mDLGBGM[key];
                }
            }
            EB.Debug.Log("<color=#ff6699ff>Can not find BGM With DLGid: {0}" , DLGid);
            return null;
        }

        public string GetGDEAudio(string GDEid)
        {
            Init();
            var keyItor = mGDEAudio.Keys.GetEnumerator();
            while (keyItor.MoveNext())
            {
                var key = keyItor.Current;
                if (key == GDEid)
                {
                    return mGDEAudio[key];
                }
            }
            EB.Debug.Log("<color=#ff6699ff>Can not find Audio With GDEid: {0}" , GDEid);
            return null;
        }
    }
}