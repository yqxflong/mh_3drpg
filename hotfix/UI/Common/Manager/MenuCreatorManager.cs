using System.Collections.Generic;

namespace Hotfix_LT.UI
{
	/// <summary>
	/// 界面创建者管理器
	/// </summary>
	public class MenuCreatorManager : MenuManager
	{
		/// <summary>
		/// 界面缓存池
		/// </summary>
		protected Dictionary<string, MenuCreator> m_creatorDict = new Dictionary<string, MenuCreator>();
		protected Dictionary<string, MenuCreator> m_creatorPrefabDic = new Dictionary<string, MenuCreator>();

		public override void Awake()
		{
			base.Awake();
			MenuCreator[] creators = mDMono.transform.GetMonoILRComponentsInChildren<MenuCreator>("Hotfix_LT.UI.MenuCreator");

			for (int i = 0, len = creators.Length; i < len; ++i)
			{
				AddCreator(creators[i]);
			}
		}

		public override void OnDestroy()
		{
			var iter = m_creatorDict.GetEnumerator();
			while (iter.MoveNext())
			{
				iter.Current.Value.CloseAndDestroyMenu();
			}
			iter.Dispose();
			m_creatorDict.Clear();

			base.OnDestroy();
		}

		public override void OnDisable()
		{
			var iter = m_creatorDict.GetEnumerator();
			while (iter.MoveNext())
			{
				iter.Current.Value.CloseAndDestroyMenu();
			}
			iter.Dispose();

			base.OnDisable();
		}

		public void AddCreator(MenuCreator creator)
		{
			string viewname = creator.menuPrefabName;
			if (!m_creatorDict.ContainsKey(viewname))
			{
				m_creatorDict.Add(viewname, creator);
			}

			if (!m_creatorPrefabDic.ContainsKey(creator.menuPrefabName))
			{
				m_creatorPrefabDic.Add(creator.menuPrefabName, creator);
			}
			else
			{
				if (m_creatorPrefabDic[creator.menuPrefabName].Menu == null) //增加一步安全措施 万一有同名
				{
					m_creatorPrefabDic[creator.menuPrefabName] = creator;
				}
			}
		}

		public void RemoveCreator(MenuCreator creator)
		{
			string viewname = creator.menuPrefabName;
			if (m_creatorDict.ContainsKey(viewname))
			{
				m_creatorDict.Remove(viewname);
			}

			if (m_creatorPrefabDic.ContainsKey(creator.menuPrefabName))
			{
				m_creatorPrefabDic.Remove(creator.menuPrefabName);
			}
		}

		public bool HasMenuCreator(string viewname)
		{
			return m_creatorDict.ContainsKey(viewname);
		}

		public virtual void PreloadMenu(string viewname)
		{
			if (m_creatorDict.ContainsKey(viewname))
			{
				// for dynamic menus
				m_creatorDict[viewname].PreloadMenu();
			}
		}

		protected override void OpenMenu(string viewname, object param, bool queue)
		{
			if (m_creatorDict.ContainsKey(viewname))
			{
				// for dynamic menus
				m_creatorDict[viewname].CreateMenu(param, queue);
			}
			else if (m_menuDict.ContainsKey(viewname))
			{
				// for static menus
				UIController menu = m_menuDict[mDMono.name];
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

		public override void CloseMenu(string viewname)
		{
			if (m_creatorDict.ContainsKey(viewname))
			{
				// for dynamic menus
				m_creatorDict[viewname].CloseMenu();
			}
			else if (m_menuDict.ContainsKey(viewname))
			{
				// for static menus
				UIController menu = m_menuDict[mDMono.name];
				menu.Close();
			}
			else if (m_creatorPrefabDic.ContainsKey(viewname)) //by pj 为了增加对预设name的支持 因为界面没有对应
			{
				m_creatorPrefabDic[viewname].CloseMenu();
			}
		}

		public override UIController GetMenu(string viewname)
		{
			if (m_creatorDict.ContainsKey(viewname))
			{
				// for dynamic menus
				return m_creatorDict[viewname].Menu;
			}
			else if (m_menuDict.ContainsKey(viewname))
			{
				// for static menus
				return m_menuDict[viewname];
			}
			return null;
		}

		//拿去使用时判空
		public UIController GetMenuByPrefabName(string prefabName)
		{
			if (m_creatorPrefabDic.ContainsKey(prefabName))
			{
				return m_creatorPrefabDic[prefabName].Menu;
			}
			return null;
		}
	}
}