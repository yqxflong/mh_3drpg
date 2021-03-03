using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class UITemporaryPanel : UIPanel
{
	private Dictionary<GameObject, ElementInfo> _elementToInfo = new Dictionary<GameObject, ElementInfo>();
	private int currentDepth = 100;
	private bool _isDestroyed = false;

	private class ElementInfo
	{
		public UIPanel addedPanel;
		public int previousDepth;
	}

	public static UITemporaryPanel CreateTemporaryPanel()
	{
		UITemporaryPanel temporaryPanel = NGUITools.AddChild<UITemporaryPanel>(UIHierarchyHelper.Instance.hudStaticContainer);
		UIHierarchyHelper.Instance.Place(temporaryPanel.gameObject, UIHierarchyHelper.eUIType.NewPanel, null);
		temporaryPanel.depth = temporaryPanel.currentDepth;
		return temporaryPanel;
	}

	public void AddChild(GameObject child)
	{
		if (child != null)
		{
			ElementInfo info = new ElementInfo();

			UIPanel panel = child.GetComponent<UIPanel>();
			UIPanel previousPanel = null;
			if (panel == null)
			{
				previousPanel = NGUITools.FindInParents<UIPanel>(child);
				panel = child.AddComponent<UIPanel>();
				info.addedPanel = panel;
			}
			else
			{
				info.addedPanel = null;
				info.previousDepth = panel.depth;
			}
			panel.depth = ++currentDepth;
			_elementToInfo[child] = info;
			CheckParentInChildren(child);

			if (previousPanel)
			{
				previousPanel.RebuildAllDrawCalls();
			}
			else
			{
				panel.RebuildAllDrawCalls();
			}
		}
	}

	public void RemoveChild(GameObject child)
	{
		if (child != null && _elementToInfo.ContainsKey(child))
		{
			if (_elementToInfo[child].addedPanel != null)
			{
				Destroy(_elementToInfo[child].addedPanel);
			}
			else
			{
				child.GetComponent<UIPanel>().depth = _elementToInfo[child].previousDepth;
			}
			CheckParentInChildren(child);
			_elementToInfo.Remove(child);
		}
	}

	public void Dispose()
	{
		StartCoroutine(Cleanup());
	}

	void OnDestroy()
	{
		//Cleanup();
	}

	IEnumerator Cleanup()
	{
		if (!_isDestroyed)
		{
			_isDestroyed = true;
			foreach (GameObject child in _elementToInfo.Keys)
			{
				if (child == null)
				{
					continue;
				}
				if (_elementToInfo[child].addedPanel != null)
				{
					Destroy(_elementToInfo[child].addedPanel);
				}
				else
				{
					child.GetComponent<UIPanel>().depth = _elementToInfo[child].previousDepth;
				}
				CheckParentInChildren(child);
			}

			yield return new WaitForEndOfFrame();

			UIPanel.list.Sort(UIPanel.CompareFunc);
			System.Array.Sort(NGUITools.FindActive<UIWidget>(), UIWidget.FullCompareFunc);

			for (int i = 0; i < UIPanel.list.Count; i++)
			{
				UIPanel.list[i].RebuildAllDrawCalls();
			}

			_elementToInfo.Clear();

			GameObject.Destroy(gameObject);
		}

		yield break;
	}

	private void CheckParentInChildren(GameObject go)
	{
		foreach (UIWidget widget in go.GetComponentsInChildren<UIWidget>())
		{
			widget.ParentHasChanged();
		}
	}
}
