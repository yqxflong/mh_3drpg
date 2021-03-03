using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using Main.MainLand;

namespace Hotfix_LT.UI
{
    public class HeadBars2D : DynamicMonoHotfix
    {
        private float screenScale = 0.001f;
        private float screenHeight = 0f;
        private float screenWidth = 0f;
        private int manualHeight_2;
        private int manualWidth_2;
        private Vector3 locatorPos;
        private Vector3 cameraPos;
        private Main.MainLand.HeadBars2DMonitor mHeadBars2DMonitor;

        public override void Awake()
        {
            InitHeadBar();
        }
        
        public override void OnDestroy()
        {
            ClearBars();
        }
        
        private void InitHeadBar()
        {
            Transform m_locator = mDMono.transform.Find("HealthBarTarget");
            if (m_locator == null)
            {
                MoveEditor.FXHelper fx_helper = mDMono.transform.GetComponent<MoveEditor.FXHelper>();

                if (fx_helper != null)
                {
                    m_locator = fx_helper.HeadNubTransform;
                }
            }

            if (m_locator == null)
            {
                m_locator = mDMono.transform;
            }

            if (mHeadBars2DMonitor == null)
            {
                mHeadBars2DMonitor = mDMono.gameObject.AddComponent<Main.MainLand.HeadBars2DMonitor>();
                mHeadBars2DMonitor.SetLocator(m_locator);
            }
            
        }
        
        private HeadBarHud GetHUDByType(eHeadBarHud hudtype)
        {
            HeadBarHud result = null;
            switch (hudtype)
            {
                case eHeadBarHud.PlayerHeadBarHud:
                    result = PlayerHeadBarHudController.Instance != null ? PlayerHeadBarHudController.Instance.GetHUD() : null;
                    break;
                case eHeadBarHud.FightStateHud:
                    result = FightingHeadBarHudController.Instance != null ? FightingHeadBarHudController.Instance.GetHUD() : null;
                    break;
                default:
                    result = null;
                    break;
            }
            return result;
        }

        public void SetBarHudState(eHeadBarHud hudtype, Hashtable data, bool state)
        {
            if (mHeadBars2DMonitor == null)
            {
                return;
            }

            int type = (int)hudtype;
            if (state)
            {
                HeadBarHud tmp;
                if (mHeadBars2DMonitor.Bars.ContainsKey(type))
                {
                    tmp = mHeadBars2DMonitor.Bars[type].transform.GetMonoILRComponent<HeadBarHud>();
                }
                else
                {
                    tmp = GetHUDByType(hudtype);
                    if (tmp != null)
                    {
                        mHeadBars2DMonitor.Bars.Add(type, tmp.mDMono.GetComponent<HeadBarHUDMonitor>());
                    }
                }

                if (tmp != null)
                {
                    tmp.SetBarState(data, state);
                    mHeadBars2DMonitor.UpdatePosition();
                }
            }
            else
            {
                HeadBarHud tmp;

                if (mHeadBars2DMonitor.Bars.ContainsKey(type)&& mHeadBars2DMonitor.Bars[type]!=null)
                {
                    tmp = mHeadBars2DMonitor.Bars[type].transform.GetMonoILRComponent<HeadBarHud>();

                    if (tmp == null)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

                mHeadBars2DMonitor.Bars.Remove(type);
                tmp.SetBarState(data, state);
                tmp.Recycle();
                tmp = null;
            }
        }

        public void ClearBars()
        {
            Dictionary<int, HeadBarHUDMonitor>.Enumerator enumerator = mHeadBars2DMonitor.Bars.GetEnumerator();
            List<eHeadBarHud> releases = new List<eHeadBarHud>();

            while (enumerator.MoveNext())
            {
                releases.Add((eHeadBarHud)enumerator.Current.Key);
            }

            for (int i = 0; i < releases.Count; ++i)
            {
                SetBarHudState(releases[i], null, false);
            }

            mHeadBars2DMonitor.Bars.Clear();
        }

    }


    public enum eHeadBarHud
    {
        PlayerHeadBarHud = 1,
        FightStateHud = 2,
    }

}