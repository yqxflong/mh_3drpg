using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
	public interface INodeData
	{
		void OnUpdate(object obj);
		void OnMerge(object obj);
		void CleanUp();
		object Clone();
	}

	public interface IPathNode
	{
		IPathNode Parent { get; set; }
		string Name { get; set; }
		string FullPath { get; set; }
		IPathNode Find(string sub);
		IPathNode AddChild(IPathNode childe);
		object Clone();
	}

	public interface IDataNode : IPathNode
//#if DEBUG
////	, IDebuggable
//#endif
	{
		INodeData Value { get; set; }
		List<UpdatedData> Dispatch(object obj, bool merge);
		List<UpdatedData> CleanUpRecursive();
		void RemoveRecursive();
	}

	public class UpdatedData
	{
		public string Path
		{
			get; set;
		}

		public INodeData Data
		{
			get; set;
		}

		public UpdatedData()
		{

		}

		public UpdatedData(string path, INodeData data)
		{
			Path = path;
			Data = data;
		}
	}

	/// <summary>
	/// Always update self first, then dispatch
	/// </summary>
	public class ArrayNode : IDataNode, INodeData
	{
		public IPathNode Parent
		{
			get; set;
		}

		public string Name
		{
			get; set;
		}

		public string FullPath
		{
			get; set;
		}

		public IPathNode Find(string sub)
		{
			if (string.IsNullOrEmpty(sub))
			{
				return mDefaultPath;
			}

			int index = -1;
			if (int.TryParse(sub, out index))
			{
				if (index >= mPathes.Length)
				{
					//EB.Debug.LogWarning("ArrayNode.Find: overflow {0} >= {1}", index, mPathes.Length);
					return null;
				}

				return mPathes[index];
			}
			else
			{
				return null;
			}
		}

		public IPathNode AddChild(IPathNode child)
		{
			if (mDefaultPath != null)
			{
				if (mDefaultPath.GetType() != child.GetType())
				{
					EB.Debug.LogError("ArrayNode.AddChild: child type not match, path: {0} current: {1} input: {2}",
						mDefaultPath.FullPath, mDefaultPath.GetType().ToString(), child.GetType().ToString());
					return null;
				}

				return mDefaultPath;
			}

			mDefaultPath = child as IDataNode;
			mDefaultPath.Parent = this;
			mDefaultPath.FullPath = FullPath + "[]";

//#if DEBUG
////			DebugSystem.RegisterSystem(child.FullPath, mDefaultPath, string.IsNullOrEmpty(FullPath) ? "Game Data Root Node" : FullPath);
//#endif

			return mDefaultPath;
		}

		public List<UpdatedData> Dispatch(object obj, bool merge)
		{
			ArrayList al = obj as ArrayList;
			if (al == null)
			{
				return CleanUpRecursive();
			}

			if (merge)
			{
				OnMerge(al);
			}
			else
			{
				OnUpdate(al);
			}

			List<UpdatedData> updatedList = new List<UpdatedData>();

			for (int i = 0; i < al.Count; ++i)
			{
				updatedList.AddRange(mPathes[i].Dispatch(al[i], merge));
			}

			return updatedList;
		}

		public List<UpdatedData> CleanUpRecursive()
		{
			List<UpdatedData> updatedList = new List<UpdatedData>();

			for (int i = 0; i < mPathes.Length; ++i)
			{
				updatedList.AddRange(mPathes[i].CleanUpRecursive());
			}

			CleanUp();

			return updatedList;
		}

		public void RemoveRecursive()
		{
			mDefaultPath.RemoveRecursive();

			CleanUp();
		}

		public object Clone()
		{
			ArrayNode clone = new ArrayNode(Name);

			// only clone structure
			clone.mDefaultPath = mDefaultPath.Clone() as IDataNode;

			return clone;
		}

		public INodeData Value
		{
			get { return this; }
			set { CleanUp(); }
		}

		public void OnUpdate(object obj)
		{
			ArrayList al = obj as ArrayList;

			// rebuild array
			mPathes = new IDataNode[al.Count];
			for (int i = 0; i < mPathes.Length; ++i)
			{
				mPathes[i] = mDefaultPath.Clone() as IDataNode;
				mPathes[i].Name = i.ToString();
				mPathes[i].Parent = this;
				mPathes[i].FullPath = FullPath + "[" + i.ToString() + "]";

//#if DEBUG
////				DebugSystem.RegisterSystem(mPathes[i].FullPath, mPathes[i], string.IsNullOrEmpty(FullPath) ? "Game Data Root Node" : FullPath);
//#endif
			}
		}

		public void OnMerge(object obj)
		{
			// no merge mode
			OnUpdate(obj);
		}

		public void CleanUp()
		{
//#if DEBUG
//			//for (int i = 0; i < mPathes.Length; ++i)
//			//{
//			//	DebugSystem.UnregisterSystem(mPathes[i], true);
//			//}
//#endif

			mPathes = new IDataNode[0];
		}

//#if DEBUG
//		//public void OnDrawDebug()
//		//{

//		//}

//		//public void OnDebugGUI()
//		//{

//		//}

//		//public void OnDebugPanelGUI()
//		//{

//		//}
//#endif

		public ArrayNode(string name)
		{
			Name = name;
		}

		private IDataNode mDefaultPath;
		private IDataNode[] mPathes = new IDataNode[0];
	}

	public class MapNode : IDataNode
	{
		public IPathNode Parent
		{
			get; set;
		}

		public INodeData Value
		{
			get; set;
		}

		public string Name
		{
			get; set;
		}

		public string FullPath
		{
			get; set;
		}

		public IPathNode Find(string sub)
		{
			return mPathes.ContainsKey(sub) ? mPathes[sub] : null;
		}

		public IPathNode AddChild(IPathNode child)
		{
			if (mPathes.ContainsKey(child.Name))
			{
				if (mPathes[child.Name].GetType() != child.GetType())
				{
					EB.Debug.LogError("MapNode.AddChild: child type not match, path: {0} current: {1} input: {2}",
						mPathes[child.Name].FullPath, mPathes[child.Name].GetType().ToString(), child.GetType().ToString());
					return null;
				}

				return mPathes[child.Name];
			}

			mPathes[child.Name] = child as IDataNode;
			child.Parent = this;
			child.FullPath = !string.IsNullOrEmpty(FullPath) ? FullPath + "." + child.Name : child.Name;

//#if DEBUG
////			DebugSystem.RegisterSystem(child.FullPath, mPathes[child.Name], string.IsNullOrEmpty(FullPath) ? "Game Data Root Node" : FullPath);
//#endif

			return child;
		}

		public List<UpdatedData> Dispatch(object obj, bool merge)
		{
			if (obj == null)
			{
				return CleanUpRecursive();
			}

			List<UpdatedData> updatedList = new List<UpdatedData>();

			if (Value != null)
			{
				if (merge)
				{
					Value.OnMerge(obj);
				}
				else
				{
					try
					{
						Value.OnUpdate(obj);
					}
					catch(Exception e)
					{
						EB.Debug.LogError(e);
						Debug.LogError(Value.ToString());
					}
				}

				UpdatedData updated = new UpdatedData(FullPath, Value);
				updatedList.Add(updated);
			}

			if (obj is IDictionary == false)
			{
				// EB.Debug.LogError($"obj is {obj.ToString()}");
				return updatedList;
			}

			IDictionary dict = obj as IDictionary;
			string[] keys = new string[mPathes.Count];
			mPathes.Keys.CopyTo(keys, 0);

			for (var i = 0; i < keys.Length; i++)
			{
				var key = keys[i];

				if (!dict.Contains(key))
				{
					continue;
				}

				updatedList.AddRange(mPathes[key].Dispatch(dict[key], merge));
			}

			return updatedList;
		}

		public void RemoveRecursive()
		{
			string[] keys = new string[mPathes.Count];
			mPathes.Keys.CopyTo(keys, 0);

			for (var i = 0; i < keys.Length; i++)
			{
				mPathes[keys[i]].RemoveRecursive();
			}

			if (Value != null)
			{
				Value = null;
			}
		}

		public List<UpdatedData> CleanUpRecursive()
		{
			List<UpdatedData> updatedList = new List<UpdatedData>();
			string[] keys = new string[mPathes.Count];
			mPathes.Keys.CopyTo(keys, 0);

			for (var i = 0; i < keys.Length; i++)
			{
				updatedList.AddRange(mPathes[keys[i]].CleanUpRecursive());
			}

			if (Value != null)
			{
				Value.CleanUp();
				UpdatedData cleaned = new UpdatedData(FullPath, Value);
				updatedList.Add(cleaned);
			}

			return updatedList;
		}

		public object Clone()
		{
			MapNode clone = new MapNode(Name);

			// clone structure & data
			clone.mPathes = new Dictionary<string, IDataNode>();
			foreach (KeyValuePair<string, IDataNode> pair in mPathes)
			{
				clone.mPathes.Add(pair.Key, pair.Value.Clone() as IDataNode);
			}
			if (Value != null)
			{
				clone.Value = Value.Clone() as INodeData;
			}

			return clone;
		}

//#if DEBUG
		//bool mToggle = false;
		//public void OnDrawDebug()
		//{

		//}

		//public void OnDebugGUI()
		//{

		//}

		//public void OnDebugPanelGUI()
		//{
		//	if (Value != null)
		//	{
		//		GUILayout.BeginVertical();
		//		mToggle = GUILayout.Toggle(mToggle, new GUIContent("Detail", mToggle ? DebugSystem.ToggleOnTexture() : DebugSystem.ToggleOffTexture()));
		//		if (mToggle)
		//		{
		//			LitJson.JsonWriter writer = new LitJson.JsonWriter();
		//			writer.PrettyPrint = true;
		//			LitJson.JsonMapper.ToJson(Value, writer);
		//			GUILayout.TextArea(writer.ToString());
		//		}
		//		GUILayout.EndVertical();
		//	}
		//}
//#endif

		public MapNode(string name)
		{
			Name = name;
		}

		private Dictionary<string, IDataNode> mPathes = new Dictionary<string, IDataNode>();
	}

	public struct GameDataListener
	{
		public string Path;
		public GameDataSparxManager.UpdateDelegate Listener;

		public GameDataListener(string path, GameDataSparxManager.UpdateDelegate listener)
		{
			Path = path;
			Listener = listener;
		}
	}

	public class GameDataSparxManager : ManagerUnit
//#if DEBUG
////	, IDebuggable
//#endif
	{
		public delegate void UpdateDelegate(string path, INodeData value);

		private Dictionary<string, UpdateDelegate> mListeners = new Dictionary<string, UpdateDelegate>();

		private IDataNode mRoot = new MapNode(string.Empty);

		private static GameDataSparxManager sInstance = null;
		public static GameDataSparxManager Instance
		{
			get
			{
				if (sInstance != null)
				{
					return sInstance;
				}

				return sInstance = LTHotfixManager.GetManager<GameDataSparxManager>();
			}
		}

		public T Register<T>(string path) where T : INodeData, new()
		{
			path += '.';

			string name = string.Empty;
			IPathNode last = mRoot;

			for (int i = 0; i < path.Length; ++i)
			{
				char c = path[i];

				if (c == '.')
				{
					if (!string.IsNullOrEmpty(name) || last is ArrayNode)
					{
						MapNode node = new MapNode(name);
						last = last.AddChild(node);
						name = string.Empty;
					}
				}
				else if (c == '[')
				{
					ArrayNode node = new ArrayNode(name);
					last = last.AddChild(node);
					name = string.Empty;
				}
				else if (c == ']')
				{
					name = string.Empty;
				}
				else
				{
					name += c;
				}
			}

			IDataNode dataNode = last as IDataNode;
			if (dataNode.Value != null)
			{
				EB.Debug.LogWarning("Register: already registered {0}", path);
			}
			T value = new T();
			dataNode.Value = value;

			return value;
		}

		public bool UnRegister(string path)
		{
			IPathNode last = FindNode(path);
			if (last == null)
			{
				EB.Debug.LogWarning("UnRegister: path not found {0}", path);
				return false;
			}

			IDataNode node = (IDataNode)last;
			node.Value = null;

			return true;
		}

		public IPathNode FindNode(string path)
		{
			path += '.';

			string name = string.Empty;
			IPathNode last = mRoot;

			for (int i = 0; i < path.Length && last != null; ++i)
			{
				char c = path[i];

				if (c == '.')
				{
					if (!string.IsNullOrEmpty(name))
					{
						last = last.Find(name);
						name = string.Empty;
					}
				}
				else if (c == '[')
				{
					if (!string.IsNullOrEmpty(name) || last is ArrayNode == false)
					{
						last = last.Find(name);
						name = string.Empty;
					}
				}
				else if (c == ']')
				{
					last = last.Find(name);
					name = string.Empty;
				}
				else
				{
					name += c;
				}
			}

			return last;
		}

		public T Find<T>(string path) where T : INodeData
		{
			IPathNode last = FindNode(path);
			if (last == null)
			{
				return default(T);
			}

			return (T)(last as IDataNode).Value;
		}

		public void RegisterListener(string path, UpdateDelegate del)
		{
			if (mListeners.ContainsKey(path))
			{
				mListeners[path] += del;
			}
			else
			{
				mListeners[path] = del;
			}

			INodeData data = Find<INodeData>(path);
			if (data != null)
			{
				del(path, data);
			}
		}

		public void RegisterListeners(GameDataListener[] listeners)
		{
			if (listeners == null)
			{
				return;
			}

			for (var i = 0; i < listeners.Length; i++)
			{
				var listener = listeners[i];

				if (mListeners.ContainsKey(listener.Path))
				{
					mListeners[listener.Path] += listener.Listener;
				}
				else
				{
					mListeners[listener.Path] = listener.Listener;
				}
			}

			for (var i = 0; i < listeners.Length; i++)
			{
				var listener = listeners[i];

				if (mListeners[listener.Path] == null)
				{
					continue;
				}

				System.Delegate[] currentListeners = mListeners[listener.Path].GetInvocationList();
				System.Delegate exists = System.Array.Find<System.Delegate>(currentListeners, d => d == (System.Delegate)listener.Listener);
				if (exists == null)
				{
					continue;
				}

				INodeData data = Find<INodeData>(listener.Path);
				if (data != null)
				{
					listener.Listener(listener.Path, data);
				}
			}
		}

		public void UnRegisterListener(string path, UpdateDelegate del)
		{
			if (mListeners.ContainsKey(path))
			{
				mListeners[path] -= del;

				if (mListeners[path] == null)
				{
					mListeners.Remove(path);
				}
			}
			else
			{
				EB.Debug.LogWarning("UnRegisterListener: listener not found {0}", path);
			}
		}

		public void UnRegisterListeners(GameDataListener[] listeners)
		{
			if (listeners == null)
			{
				return;
			}

			for (var i = 0; i < listeners.Length; i++)
			{
				var listener = listeners[i];

				if (mListeners.ContainsKey(listener.Path))
				{
					mListeners[listener.Path] -= listener.Listener;

					if (mListeners[listener.Path] == null)
					{
						mListeners.Remove(listener.Path);
					}
				}
				else
				{
					EB.Debug.LogWarning("UnRegisterListeners: listener not found {0}", listener.Path);
				}
			}
		}

		public bool HasListener(string path)
		{
			return mListeners.ContainsKey(path) && mListeners[path] != null;
		}

		private void NotifyListeners(List<UpdatedData> updatedList)
		{
			if (updatedList == null)
			{
				return;
			}

			for (var i = 0; i < updatedList.Count; i++)
			{
				var updated = updatedList[i];

				if (mListeners.ContainsKey(updated.Path) && mListeners[updated.Path] != null)
				{
					mListeners[updated.Path](updated.Path, updated.Data);
				}
			}
		}

		public void ProcessIncomingData(object obj, bool merge)
		{
			List<UpdatedData> updatedList = mRoot.Dispatch(obj, merge);
			NotifyListeners(updatedList);
		}

		public void SetDirty(string path)
		{
			UpdatedData updated = new UpdatedData();
			updated.Path = path;
			updated.Data = Find<INodeData>(path);
			if (updated.Data == null)
			{
				EB.Debug.LogError("SetDirty: can't find data in path {0}", path);
				return;
			}

			List<UpdatedData> updatedList = new List<UpdatedData>();
			updatedList.Add(updated);

			NotifyListeners(updatedList);
		}

		public EB.Sparx.SubSystemState State { get; set; }

		public override void Connect()
		{
			State = EB.Sparx.SubSystemState.Connected;
		}

		public override void Disconnect(bool isLogout)
		{
			State = EB.Sparx.SubSystemState.Disconnected;

			mRoot.CleanUpRecursive();
		}

		public override void Initialize(EB.Sparx.Config config)
		{
//#if DEBUG
////			DebugSystem.RegisterSystem("View Game Data", this);
////			DebugSystem.RegisterSystem("Game Data Root Node", mRoot, "View Game Data");
//#endif

			// PushManager should Initialize before GameDataManager
			EB.Sparx.PushManager pm = EB.Sparx.Hub.Instance.PushManager;
			if (pm != null)
			{
				pm.OnHandleMessage += new System.Action<string, string, object>(OnHandleMessage);
			}
		}

		public override void Async(string message, object payload)
		{
			if (payload != null)
			{
				ProcessIncomingData(payload, true);
			}
		}

		private void OnHandleMessage(string component, string message, object payload)
		{
			if (payload != null)
			{
				ProcessIncomingData(payload, true);
			}
		}

//#if DEBUG
//		//public void OnDrawDebug()
//		//{

//		//}

//		//public void OnDebugGUI()
//		//{

//		//}

//		//public void OnDebugPanelGUI()
//		//{

//		//}
//#endif
	}
}