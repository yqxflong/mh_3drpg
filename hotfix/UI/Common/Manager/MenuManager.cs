using System.Collections.Generic;

namespace Hotfix_LT.UI
{
	/// <summary>
	/// 菜单管理器
	/// </summary>
	public class MenuManager : DynamicMonoHotfix
	{
		/// <summary>
		/// 菜单缓存池
		/// </summary>
		protected Dictionary<string, UIController> m_menuDict = new Dictionary<string, UIController>();
		protected UIController m_hudMenu = null;

		public override void Awake()
		{
			UIController[] menus = mDMono.transform.GetComponentsInChildren<UIController>(true);

            for(int i= 0; i < menus.Length; ++i)
            {
                var Ilr = menus[i] as UIControllerILR;

                if (Ilr != null)
                {
                    Ilr.ILRObjInit();
                }
            }

			for (int i = 0, len = menus.Length; i < len; ++i)
			{
				menus[i].Show(false);
				AddMenu(menus[i]);
			}

			// default to first one
			if (m_hudMenu == null && menus.Length > 0)
			{
				m_hudMenu = menus[0];
			}
		}

		public override void OnEnable()
		{
			Hotfix_LT.Messenger.AddListener<string,object,bool>(Hotfix_LT.EventName.ShowMenu, OnShowMenuListener);
			Hotfix_LT.Messenger.AddListener<string>(Hotfix_LT.EventName.CloseMenu, OnCloseMenuListener);
		}

		public override void OnDisable()
		{
			if (Hotfix_LT.Messenger.eventTable.ContainsKey(Hotfix_LT.EventName.ShowMenu))
			{
				Hotfix_LT.Messenger.RemoveListener<string, object, bool>(Hotfix_LT.EventName.ShowMenu, OnShowMenuListener);
			}

			if (Hotfix_LT.Messenger.eventTable.ContainsKey(Hotfix_LT.EventName.CloseMenu))
			{
				Hotfix_LT.Messenger.RemoveListener<string>(Hotfix_LT.EventName.CloseMenu, OnCloseMenuListener);
			}

			var iter = m_menuDict.GetEnumerator();

			while (iter.MoveNext())
			{
				CloseMenu(iter.Current.Key);
			}

			iter.Dispose();
		}

		public override void OnDestroy()
		{
			m_menuDict.Clear();
		}

		protected virtual void OnShowMenuListener(string menuName,object menuParam,bool queue)
		{
            if (menuName == mDMono.name)
			{
				OpenHud(menuParam);
			}
			else if (HasMenu(menuName))
			{
				OpenMenu(menuName, menuParam, queue);
			}
		}

		protected virtual void OnCloseMenuListener(string menuName)
		{
			if (menuName == mDMono.name)
			{
				CloseHud();
			}
			else if (HasMenu(menuName))
			{
				CloseMenu(menuName);
			}
		}

		public void AddMenu(UIController menu)
		{
			if (!m_menuDict.ContainsKey(menu.name))
			{
				m_menuDict.Add(menu.name, menu);
			}
		}

		public void RemoveMenu(UIController menu)
		{
			if (m_menuDict.ContainsKey(menu.name))
			{
				m_menuDict.Remove(menu.name);
			}
		}

		public bool HasMenu(string name)
		{
			return m_menuDict.ContainsKey(name);
		}

		public virtual UIController GetMenu(string name)
		{
			return m_menuDict.ContainsKey(name) ? m_menuDict[name] : null;
		}

		protected virtual void OpenMenu(string name, object param, bool queue)
		{
			if (m_menuDict.ContainsKey(name))
			{
				UIController menu = m_menuDict[name];
				menu.SetMenuData(param);
				menu.PlayTween();

				if (queue)
				{
					menu.Queue();
				}
				else
				{
					menu.Open();
				}
			}
		}

		public virtual void CloseMenu(string name)
		{
			if (m_menuDict.ContainsKey(name))
			{
				UIController menu = m_menuDict[name];
				menu.Close();
			}
		}

		public virtual void OpenHud(object param)
		{
			// default open first one
			if (m_hudMenu != null)
			{
				OpenMenu(m_hudMenu.name, param, false);
			}
		}

		public virtual void CloseHud()
		{
			// default close first one
			if (m_hudMenu != null)
			{
				CloseMenu(m_hudMenu.name);
			}
		}

		public static void WarningAlert(string title, string content)
		{
			title = EB.Localizer.GetString(title);
			content = EB.Localizer.GetString(content);
			string ok = EB.Localizer.GetString("ID_DIALOG_BUTTON_OK");
			MessageDialog.Show(title, content, ok, null, false, true, true);
		}

		public static void Warning(string content)
		{
			Alert("ID_DIALOG_TITLE_WARNING", content);
		}

		public static void Alert(string titleFormat, string contentFormat, params object[] args)
		{
			string title = EB.Localizer.Format(titleFormat, args);
			string content = EB.Localizer.Format(contentFormat, args);
			string ok = EB.Localizer.GetString("ID_DIALOG_BUTTON_OK");

			MessageDialog.Show(title, content, ok, null, false, true, true);
		}

		public static void Warning(string contentFormat, params object[] args)
		{
			Alert("ID_DIALOG_TITLE_WARNING", contentFormat, args);
		}

		public static void Alert(string content)
		{
			content = EB.Localizer.GetString(content);
			string ok = EB.Localizer.GetString("ID_DIALOG_BUTTON_OK");

			MessageDialog.Show(EB.Localizer.GetString(MessageTemplate.Title), content, ok, null, true, true, true);
		}

		public static void Alert(string contentFormat, params object[] args)
		{
			string content = EB.Localizer.Format(contentFormat, args);
			string ok = EB.Localizer.GetString("ID_DIALOG_BUTTON_OK");

			MessageDialog.Show(EB.Localizer.GetString(MessageTemplate.Title), content, ok, null, true, true, true);
		}
	}
}