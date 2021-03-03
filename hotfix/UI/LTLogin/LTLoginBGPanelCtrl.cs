using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Hotfix_LT.UI
{
    public class LTLoginBGPanelCtrl : DynamicMonoHotfix
    {
        public UITexture m_Brand;
        public UITexture m_Texture;
        public GameObject m_FX;

        public bool BGFinish = false;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_Brand = t.GetComponent<UITexture>("Logo/Brand");
            m_Texture = t.GetComponent<UITexture>("Texture");
            
            m_FX = t.Find("Texture/FX").gameObject;

            m_Brand.mainTexture = mDMono.ObjectParamList[0] as Texture2D;

            Hotfix_LT.Messenger.AddListener("LTLoginBGPanelCtrlEven", TweenAction);
        }
        
        public override void Start()
        {
            base.Start();
            if(ILRDefine.IS_FX)
            {
                if (GameEngine.Instance.TextureDic.ContainsKey(GameEngine.Instance.LoginBGPath))
                {
                    m_FX.CustomSetActive(false);
                    m_Texture.mainTexture = GameEngine.Instance.TextureDic[GameEngine.Instance.LoginBGPath];
                    m_Texture.MakePixelPerfect();
                    SetUITexture(m_Texture);
                }
                if (GameEngine.Instance.TextureDic.ContainsKey(GameEngine.Instance.BrandPath))
                {
                    m_Brand.mainTexture = GameEngine.Instance.TextureDic[GameEngine.Instance.BrandPath];
                }
            }
            else if (Application.identifier.Equals("com.mkhx.xinkuai"))
            {
                if (GameEngine.Instance.TextureDic.ContainsKey(GameEngine.Instance.BrandPath))
                {
                    m_Brand.mainTexture = GameEngine.Instance.TextureDic[GameEngine.Instance.BrandPath];
                }
                m_FX.CustomSetActive(true);
            }
            else
            {
                m_FX.CustomSetActive(true);
            }
            BGFinish = true;
        }

        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener("LTLoginBGPanelCtrlEven", TweenAction);
            m_Brand.mainTexture = null;
            if (m_Texture.mainTexture != null) m_Texture.mainTexture = null;
        }
    
        /// <summary>
        /// 登陆界面动效
        /// </summary>
        public void TweenAction()
        {
            UITweener[] TWS = m_Brand.GetComponents<UITweener>();
            for(int i=0;i<TWS.Length;i++)
            {
                TWS[i].PlayForward();
            }
        }

        /// <summary>
        /// 设置UITexture
        /// </summary>
        /// <param name="uiTexture"></param>
        private void SetUITexture(UITexture uiTexture)
        {
            float FSWidth = (float)UIRoot.list[0].manualWidth;
            float FSHeight = (float)UIRoot.list[0].manualHeight;
            float ScreenScale = (float)Screen.width / (float)Screen.height;
            float TextureScale = (float)uiTexture.width / (float)uiTexture.height;

            if (FSWidth / Screen.width > FSHeight / Screen.height)
            {
                FSHeight = FSWidth / ScreenScale;
                uiTexture.height = (int)FSHeight + 2;
                uiTexture.width = (int)(uiTexture.height * TextureScale) + 2;
                if (uiTexture.width < FSWidth)
                {
                    uiTexture.width = (int)FSWidth + 2;
                    uiTexture.height = (int)(uiTexture.width / ScreenScale);
                }
            }
            else
            {
                FSWidth = FSHeight * ScreenScale;
                uiTexture.width = (int)FSWidth + 2;
                uiTexture.height = (int)(uiTexture.width / TextureScale) + 2;
                if (uiTexture.height < FSHeight)
                {
                    uiTexture.height = (int)FSHeight + 2;
                    uiTexture.width = (int)(uiTexture.height * ScreenScale);
                }
            }
        }
    }
}
