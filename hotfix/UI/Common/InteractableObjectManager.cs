using UnityEngine;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
	/// <summary>
	/// 可交互对象管理器
	/// </summary>
	public class InteractableObjectManager : DynamicMonoHotfix
	{
		List<GameObject> m_InteractableObjects;

		private static InteractableObjectManager m_Instance;

		public static InteractableObjectManager Instance
		{
			get
			{
				if (m_Instance == null)
				{
					GameObject go = new GameObject("InteractableObjectManager");
					m_Instance = go.AddMonoILRComponent<InteractableObjectManager>("Hotfix_LT.UI.InteractableObjectManager");
					m_Instance.Initialize();
				}
				return m_Instance;
			}
		}

		void Initialize()
		{
			m_InteractableObjects = new List<GameObject>();
		}

		/// <summary>
		/// 设置父级
		/// </summary>
		/// <param name="trans"></param>
		public void SetParent(Transform trans)
		{
			if (mDMono != null)
			{
				mDMono.transform.SetParent(trans);
			}
		}

		public void LoadInteractableObject(string prefab, Vector3 pos, Quaternion qt)
		{
			EB.Assets.LoadAsync(prefab, typeof(GameObject), o =>
			{
				if (o)
				{
					GameObject instance = GameObject.Instantiate(o, pos, qt) as GameObject;
					instance.transform.SetParent(mDMono.gameObject.transform);
					m_InteractableObjects.Add(instance);
				}
			});
		}

		public void ClearAllIneractableOjbects()
		{
			int count = m_InteractableObjects.Count;
			for (int i = 0; i < count; i++)
			{
				GameObject.Destroy(m_InteractableObjects[i]);
			}

			m_InteractableObjects.Clear();
		}
	}
}