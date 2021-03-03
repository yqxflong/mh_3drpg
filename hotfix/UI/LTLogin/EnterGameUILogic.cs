using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using EB.Sparx;
    
namespace Hotfix_LT.UI
{
    public class EnterGameUILogic : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            m_Container = t.FindEx("Container").gameObject;
            m_WorldName_Label = t.GetComponent<UILabel>("Container/UIPanel/Tween/ServerBar/BG/WName");
            VersionLabel = t.GetComponent<UILabel>("Container/UIPanel/Tween/DistributeAnchor/Layout/FullVersion");
            TweenParent = t.GetComponent<TweenAlpha>("Container/UIPanel/Tween");
            m_ResetGameBtnPanel = t.FindEx("Container/UIPanel/Tween/ResetGameButtonPanel").gameObject;

            controller.hudRoot = t.GetComponent<Transform>();

            t.GetComponent<UIButton>("Container/UIPanel/Tween/ServerBar/BG").onClick.Add(new EventDelegate(OnSelectWorldBtnClick));
            t.GetComponent<UIButton>("Container/UIPanel/Tween/EnterGameButtonPanel/Button").onClick.Add(new EventDelegate(OnEnterGameBtnClick));
            t.GetComponent<UIButton>("Container/UIPanel/Tween/ResetGameButtonPanel/Button").onClick.Add(new EventDelegate(OnResetGameBtnClick));
            t.GetComponent<UIButton>("Container/UIPanel/Tween/NoticeBtn").onClick.Add(new EventDelegate(OnNoticeBtnClick));
            t.GetComponent<UIButton>("Container/UIPanel/Tween/OfficialWebBtn").onClick.Add(new EventDelegate(OnOfficialWebBtnClick));

            if (ILRDefine.DEBUG || ILRDefine.USE_GM)
            {
                m_ResetGameBtnPanel.gameObject.CustomSetActive(true);
            }
            else
            {
                m_ResetGameBtnPanel.gameObject.CustomSetActive(false);
            }
            FusionAudio.PostGlobalMusicEvent("MUS_CampaignView_Demo", true);

        }
        
        public GameObject m_Container;
        public UILabel m_WorldName_Label;
    	public UILabel VersionLabel;
        public TweenAlpha TweenParent;
        public GameObject m_ResetGameBtnPanel;
        private bool m_showNotice = true;
    
        public override bool Visibility
        {
            get { return m_Container.activeSelf; }
        }
    
        public override void Show(bool isShowing)
        {
            m_Container.CustomSetActive(isShowing);
        }
    
        public override bool CanAutoBackstack()
        {
            return false;
        }
    
        public override IEnumerator OnAddToStack()
        {
    		VersionLabel.text = EB.Version.GetFullVersion();
    		FillWorldData();
    
            yield return base.OnAddToStack();
    
            Hotfix_LT.Messenger.Raise("LTLoginBGPanelCtrlEven");
            TweenAction();
        }

        public override void OnFocus()
        {
            base.OnFocus();
            if (LoginManager.Instance.Notices!=null && m_showNotice)
            {
                OnNoticeBtnClick();
            }
            m_showNotice = false;

            FillWorldData();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            yield return base.OnRemoveFromStack();
        }
        
        public override void OnDestroy()
        {
            FusionAudio.PostGlobalMusicEvent("MUS_CampaignView_Demo", false);
    
            base.OnDestroy();
        }
        
    	private void FillWorldData()
        {
            var gameWorlds = LoginManager.Instance.GameWorlds;
            var gameWorld = System.Array.Find(gameWorlds, w => w.Default);
            if (gameWorld == null)
            {
    			GameWorld[]  words= System.Array.FindAll(gameWorlds, w => w.P1== GameWorld.RECOMMEND);
    			if(words!=null && words.Length>0)
    			{
    				int index = Random.Range(0, words.Length);
    				gameWorld = words[index];
    				gameWorld.Default = true;
    			}
    			else
    			{
    				gameWorld = gameWorlds[gameWorlds.Length - 1];
    				gameWorld.Default = true;
    			}
            }
            LTUIUtil.SetText(m_WorldName_Label,gameWorld.Id + EB.Localizer.GetString("ID_LOGIN_SERVER_NAME") + " " + gameWorld.Name + "   "+string.Format("[42FE79]{0}[-]",EB.Localizer.GetString("ID_CLICK_SEVER")));
        }
    
        public void OnSelectWorldBtnClick()
        {
            Hotfix_LT.Messenger.Raise<string,object,bool>(Hotfix_LT.EventName.ShowMenu, "ServerSelect",null,false);
        }
    
    	public void OnNoticeBtnClick()
    	{
            Hotfix_LT.Messenger.Raise<string, object, bool>(Hotfix_LT.EventName.ShowMenu, "NoticeUI", LoginManager.Instance.Notices,false);
    	}
    
    	public void OnOfficialWebBtnClick()
    	{
    
    	}
    
        public void OnResetGameBtnClick()
        {
            GameEngine.Instance.IsResetUserData = true;
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_EnterGameUILogic_5673"));
        }
    
        public static System.DateTime start = System.DateTime.MinValue;

        /// <summary>
        /// 进入游戏按钮点击回调
        /// </summary>
        public void OnEnterGameBtnClick()
        {
            //执行entergame 逻辑
            //判定该服务器没有角色  则先进入到选择角色界面
            //判定该服务器有角色 则直接进入游戏
            GameEngine.Instance.IsTimeToRootScene = false;
            GameEngine.Instance.IsRunFromEnterGameBtn = true;
            EnterGameUILogic.start = System.DateTime.Now;
            var gameWorlds = LoginManager.Instance.GameWorlds;
            var gameWorld = System.Array.Find(gameWorlds, w => w.Default);
            if (gameWorld == null)
            {
                gameWorld = gameWorlds[gameWorlds.Length - 1];
                gameWorld.Default = true;
            }
    
            if (gameWorld.State == GameWorld.eState.Down)
            {
                UIStack.Instance.GetDialog("Alert",EB.Localizer.GetString("ID_SPARX_SERVER_MAINTENANCE_TITLE"), null);
                return;
            }
    
            var chars = LoginManager.Instance.Account.Users;
            var ch = System.Array.Find(chars, c => c.WorldId == gameWorld.Id);
    
    		if (ch != null && GameEngine.Instance.IsResetUserData)
    		{
    			Hashtable charData = Johny.HashtablePool.Claim();
    			var world = System.Array.Find(LoginManager.Instance.GameWorlds, w => w.Default);
    			charData.Add("worldId", world.Id);
                LoginManager.Instance.DebugResetWorldUser(charData, delegate (string err, object result)
    			{
    				if (!string.IsNullOrEmpty(err))
    				{
    					EB.Debug.LogError("DebugResetWorldUser: error = {0}", err);
    					return;
    				}

                    LoginManager.Instance.Account.Remove(ch);
    				GameEngine.Instance.IsResetUserData = false;
                    
    			});
    		}

            UIStack.Instance.ExitStack(true);
            UIStack.Instance.ShowLoadingScreen(() =>
            {
                Hashtable charData = Johny.HashtablePool.Claim();
                var world = System.Array.Find(LoginManager.Instance.GameWorlds, w => w.Default);
                charData.Add("worldId", world.Id);
                LoginManager.Instance.EnterGame(charData);
            }, false, true);
        }
    
        /// <summary>
        /// 登陆界面动效
        /// </summary>
        public void TweenAction()
        {
            if (TweenParent != null)
            {
                TweenParent.PlayForward();
            }
        }
    }
}
