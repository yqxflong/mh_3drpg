using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using GM;
using System.Linq;

public class BundleList : List<GM.BundleInfo>
{
	public override string ToString()
	{
		if (Count == 0)
		{
			return string.Empty;
		}

		if (Count == 1)
		{
			return this[0].BundleName;
		}

		return this[0].BundleName + "...";
	}
}

public class ItemPriorityEditor : EditorWindow {
	public static ItemPriorityEditor Instance { get; private set; }
	public static bool IsOpen {
		get { return Instance != null; }
	}

	void OnEnable()
	{
		Debug.Log("ItemPriorityEditor OnEnable");
		Instance = this;
	}

	GUIDragHandler mDragHandler = null;
	GM.BundleInfo mCurrentRecieving = null;
	
	//Vector2 mScrollPosBlock = Vector2.zero;
	Vector2 mScrollPosUnBlock = Vector2.zero;
	List<GM.BundleInfo> renderList = new List<GM.BundleInfo>();
	void OnGUI()
	{
		if (mDragHandler == null)
		{
			mDragHandler = new GUIDragHandler();
			mDragHandler.dragIdentifier = "ItemPriorityEditor";
			mDragHandler.AddRecieveIdentifier(mDragHandler.dragIdentifier);
			mDragHandler.canRecieveCallBack = OnCanRecieve;
			mDragHandler.reciveDragCallBack = OnRecieve;
		}

		EditorGUILayout.BeginVertical(BMGUIStyles.GetStyle("OL Box"));
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
			{
				Rect loadBtnRect = GUILayoutUtility.GetRect(new GUIContent("Load"), EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(false));
				if (GUI.Button(loadBtnRect, "Load", EditorStyles.toolbarDropDown))
				{
					GenericMenu menu = new GenericMenu();
					menu.AddItem(new GUIContent("Load Bundle Ship Info File"), false, LoadBundleShipInfoFile);
					menu.AddItem(new GUIContent("Save Bundle Ship Info File"), false, SaveBundleShipInfoFile);
					menu.AddItem(new GUIContent("Sort By Tree And Priority"), false, SortByTreeAndPriority);
					menu.DropDown(loadBtnRect);
				}
			}
			EditorGUILayout.EndHorizontal();

			if (mCurrentEditItems != null)
			{
				mScrollPosUnBlock = EditorGUILayout.BeginScrollView(mScrollPosUnBlock);
				{
					if (mCurrentEditItemsChanged)
					{
						renderList.Clear();
						renderList.AddRange(mCurrentEditItems);
						mCurrentEditItemsChanged = false;
					}
					
					foreach (GM.BundleInfo itemDesc in renderList)
					{
						GUIDrawItemDesc(itemDesc);
					}
				}
				EditorGUILayout.EndScrollView();
			}
		}
		EditorGUILayout.EndVertical();
	}

	void GUIDrawItemDesc(GM.BundleInfo itemDesc)
	{
		bool isReceiving = mCurrentRecieving == itemDesc;
		bool isSelecting = mSelections.Contains(itemDesc);
		GUIStyle currentLabelStyle = BMGUIStyles.GetStyle("TreeItemUnSelect");
		if (isReceiving)
		{
			currentLabelStyle = BMGUIStyles.GetStyle("receivingLable");
		}
		else if (isSelecting)
		{
			currentLabelStyle = HasFocus() ? BMGUIStyles.GetStyle("TreeItemSelectBlue") : BMGUIStyles.GetStyle("TreeItemSelectGray");
		}

		Rect itemRect = EditorGUILayout.BeginHorizontal(currentLabelStyle);

		GUILayout.Label(BMGUIStyles.GetIcon("assetBundleIcon"), BMGUIStyles.GetStyle("BItemLabelNormal"), GUILayout.ExpandWidth(false));

		GUILayout.Label(new GUIContent(itemDesc.BundleName), isSelecting ? BMGUIStyles.GetStyle("BItemLabelActive") : BMGUIStyles.GetStyle("BItemLabelNormal"), GUILayout.ExpandWidth(true));

		EditorGUILayout.EndHorizontal();

		if (DragProcess(itemRect, itemDesc))
		{
			return;
		}

		SelectProcess(itemRect, itemDesc);
	}

	bool DragProcess(Rect itemRect, GM.BundleInfo itemDesc)
	{
		if (Event.current.type == EventType.Repaint || itemRect.height <= 0)
		{
			return false;
		}

		if (!IsMouseOn(itemRect))
		{
			if (mCurrentRecieving != null && mCurrentRecieving == itemDesc)
			{
				mCurrentRecieving = null;
				Repaint();
			}

			return false;
		}

		mDragHandler.detectRect = itemRect;
		mDragHandler.dragData.customDragData = (object)mSelections;
		mDragHandler.dragAble = (mSelections != null && mSelections.Count > 0);

		GUIDragHandler.DragState dragState =mDragHandler.GUIDragUpdate();
		if (dragState == GUIDragHandler.DragState.Receiving)
		{
			mCurrentRecieving = itemDesc;
			Repaint();
		}
		else if (dragState == GUIDragHandler.DragState.Received)
		{
			mCurrentRecieving = null;
		}
		else if (mCurrentRecieving == itemDesc)
		{
			mCurrentRecieving = null;
			Repaint();
		}

		return dragState == GUIDragHandler.DragState.Received;
	}

	void SelectProcess(Rect itemRect, GM.BundleInfo itemDesc)
	{
		if (IsRectClicked(itemRect) && !Event.current.alt)
		{
			if (Control())
			{
				if (mSelections.Contains(itemDesc))
				{
					mSelections.Remove(itemDesc);
				}
				else
				{
					mSelections.Add(itemDesc);
				}
			}
			else if (Event.current.shift)
			{
				ShiftSelection(itemDesc);
			}
			else if (Event.current.button == 0 || !mSelections.Contains(itemDesc))
			{
				mSelections.Clear();
				mSelections.Add(itemDesc);
			}

			Repaint();
		}
	}

	void ShiftSelection(GM.BundleInfo newSelect)
	{
		if (mSelections.Count == 0)
		{
			mSelections.Add(newSelect);
			return;
		}

		int _min = System.Int32.MaxValue;
		int _max = System.Int32.MinValue;
		foreach (GM.BundleInfo _desc in mSelections)
		{
			int _idx = mCurrentEditItems.IndexOf(_desc);
			if (_min > _idx)
			{
				_min = _idx;
			}

			if (_max < _idx)
			{
				_max = _idx;
			}
		}

		int _newIdx = mCurrentEditItems.IndexOf(newSelect);
		if (_newIdx < _min)
		{
			_min = _newIdx;
		}

		if (_newIdx > _max)
		{
			_max = _newIdx;
		}

		if (_min < 0)
		{
			_min = 0;
		}
		if (_max >= mCurrentEditItems.Count)
		{
			_max = mCurrentEditItems.Count - 1;
		}

		mSelections.Clear();
		mSelections.AddRange(mCurrentEditItems.GetRange(_min, _max - _min + 1));
	}

	bool OnCanRecieve(GUIDragHandler.DragDatas revieverData, GUIDragHandler.DragDatas dragData)
	{
		try
		{
			List<GM.BundleInfo> dragItems = dragData.customDragData as List<GM.BundleInfo>;
			if (mCurrentRecieving != null)
			{
				if (dragItems.Contains(mCurrentRecieving))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return true;
			}
		}
		catch (System.Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
		return false;
	}

	void OnRecieve(GUIDragHandler.DragDatas recieverData, GUIDragHandler.DragDatas dragData)
	{
		try
		{
			GM.BundleInfo recieverItem = mCurrentRecieving;
			if (recieverItem == null)
			{
				return;
			}

			List<GM.BundleInfo> dragItems = dragData.customDragData as List<GM.BundleInfo>;

			List<GM.BundleInfo> tmpList = mCurrentEditItems;
			mCurrentEditItems = null;
			foreach (GM.BundleInfo _desc in dragItems)
			{
				tmpList.Remove(_desc);
			}

			int _idx = tmpList.IndexOf(recieverItem);
			tmpList.InsertRange(_idx, dragItems);
			mCurrentEditItems = tmpList;
			tmpList = null;

			mCurrentEditItemsChanged = true;

			//SaveBundleShipInfoFile();

			Repaint();
		}
		catch (System.Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
	}

	bool IsRectClicked(Rect rect)
	{
		return Event.current.type == EventType.MouseDown && IsMouseOn(rect);
	}
	
	bool Control()
	{
		return (Event.current.control && Application.platform == RuntimePlatform.WindowsEditor) ||
			(Event.current.command && Application.platform == RuntimePlatform.OSXEditor);
	}
	
	bool IsMouseOn(Rect rect)
	{
		return rect.Contains(Event.current.mousePosition);
	}

	bool HasFocus()
	{
		return this == EditorWindow.focusedWindow;
	}

	List<BundleData> TreeToList(BundleData root)
	{
		List<BundleData> list = new List<BundleData>();
		list.Add(root);
		foreach (var child in root.children)
		{
			list.AddRange(TreeToList(BundleManager.GetBundleData(child)));
		}
		return list;
	}

	public void SortByTreeAndPriority()
	{
		if (mCurrentEditItems != null)
		{
			List<BundleInfo> tmpList = new List<BundleInfo>();
			foreach (var root in BundleManager.Roots)
			{
				foreach (var node in TreeToList(root))
				{
					var bundle = mCurrentEditItems.Find(item => item.BundleName == node.name);
					if (bundle != null)
					{
						mCurrentEditItems.Remove(bundle);
						tmpList.Add(bundle);
					}
				}
			}
			tmpList.AddRange(mCurrentEditItems);
			tmpList = tmpList.OrderBy(item =>
			{
				return item.Priority;
			}).ToList();

			mCurrentEditItems.Clear();
			foreach (var bundle in tmpList)
			{
				EB.Collections.Stack<BundleInfo> list = new EB.Collections.Stack<BundleInfo>();

				var iter = bundle;
				list.Push(iter);
				while (iter != null && !string.IsNullOrEmpty(iter.Parent))
				{
					iter = tmpList.Find(item => item.BundleName == iter.Parent);
					if (iter != null)
					{
						list.Add(iter);
					}
				}

				while (list.Count > 0)
				{
					var item = list.Pop();
					if (!mCurrentEditItems.Contains(item))
					{
						mCurrentEditItems.Add(item);
					}
				}
			}

			mCurrentEditItemsChanged = true;
		}
	}

	public void LoadBundleShipInfoFile()
	{
		if (!System.IO.File.Exists(mBundleInfoFilePath))
		{
			string path = EditorUtility.OpenFilePanel("Load Bundle Ship Info File", "", "json");
			if (!string.IsNullOrEmpty(path))
			{
				mBundleInfoFilePath = path;
			}
		}

		try
		{
			using(System.IO.TextReader reader = new System.IO.StreamReader(mBundleInfoFilePath))
			{
				string jsonStr = reader.ReadToEnd();
				mCurrentEditItems = GM.JSON.ToObject<List<GM.BundleInfo>>(jsonStr);
				for (int i = 0; i < mCurrentEditItems.Count; ++i)
				{
					var bundle = BundleManager.GetBundleData(mCurrentEditItems[i].BundleName);
					mCurrentEditItems[i].Priority = bundle != null ? bundle.priority : mCurrentEditItems[i].Priority;
				}
				mCurrentEditItemsChanged = true;
			}
		}
		catch(System.Exception ex)
		{
			mBundleInfoFilePath = "";
			mCurrentEditItems = null;
			Debug.LogErrorFormat("LoadBundleShipInfoFile Failed. Reason: {0}", ex.ToString());
		}
	}

	public void SaveBundleShipInfoFile()
	{
		try
		{
			System.IO.TextWriter tw = new System.IO.StreamWriter(mBundleInfoFilePath);
			string jsonStr = JsonFormatter.PrettyPrint(GM.JSON.ToJson(mCurrentEditItems));
			tw.Write(jsonStr);
			tw.Flush();
			tw.Close();

			if (BundleTreeWin.IsOpen)
			{
				BMDataAccessor.ReloadBundleShipInfos();
			}

			BuildHelper.ExportBundleShipInfoFileToOutput();
		}
		catch (System.Exception ex)
		{
			Debug.Log(ex.ToString());
		}
	}
	
	List<GM.BundleInfo> mCurrentEditItems = null;
	bool mCurrentEditItemsChanged = true;
	string mBundleInfoFilePath = BMDataAccessor.BundleShipInfoPath;
	BundleList mSelections = new BundleList();

	[MenuItem("Window/Item Priority Editor")]
	static void Init()
	{
		GetWindow<ItemPriorityEditor>();
	}
}
