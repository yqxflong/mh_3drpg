//JSONNodePool
//缓存JSONNode
//Johny

// #define JOHNY_NO_POOLING

using System.Collections.Generic;

namespace Johny {
	public static class JSONNodePool
	{
		static readonly Dictionary<System.Type, Stack<JSONNode>> pool = new Dictionary<System.Type, Stack<JSONNode>>{
			{typeof(JSONObject), new Stack<JSONNode>()},
			{typeof(JSONArray), new Stack<JSONNode>()},
			{typeof(JSONString), new Stack<JSONNode>()}
		};

		private static int _claimCount = 0, _newCount = 0,  _releaseCount = 0;

		private static JSONNode NewOne(System.Type tp){
			if(tp == typeof(JSONObject)){
				return new JSONObject();
			}
			else if(tp == typeof(JSONArray)){
				return new JSONArray();
			}
			else if(tp == typeof(JSONString)){
				return new JSONString();
			}

			EB.Debug.LogError("tp不是一个合适的类型!!");
			return null;
		}

		private static System.Type GetTypeByJNode(JSONNode jnode){
			if(jnode.IsObject){
				return typeof(JSONObject);
			}
			else if(jnode.IsArray){
				return typeof(JSONArray);
			}
			else if(jnode.IsString){
				return typeof(JSONString);
			}

			EB.Debug.LogError("jnode不是一个合适的类型!!");
			return typeof(JSONNode);
		} 

		public static void Prepare(System.Type tp, int count){
			for(int i =0; i < count; i++){
				var jnode = NewOne(tp);
				pool[tp].Push(jnode);
			}
		}

		public static JSONNode Claim(System.Type tp) {
			var list = pool[tp];
			if (list.Count > 0) {
				_claimCount++;
				return list.Pop();
			}

			_newCount++;
			return NewOne(tp);
		}

		/// <summary>
		/// 直接回收，里面的items忽略
		/// </summary>
		/// <param name="ht"></param>
		public static void Release(JSONNode jnode) 
		{
			if(jnode == null){
				return;
			}
			jnode.OnEnterPool();
			var tp = GetTypeByJNode(jnode);
			pool[tp].Push(jnode);
			_releaseCount++;
		}

		public static void Clear () {
			pool.Clear();
			_newCount = _claimCount = _releaseCount = 0;
		}

		public static int GetSize () {
			int count = 0;
			var it = pool.GetEnumerator();
			while(it.MoveNext()){
				count += it.Current.Value.Count;
			}
			return count;
		}

		public static void DebugConsole_ClaimInfo(){
			EB.Debug.LogError($"JSONNodePool: _newCount: {_newCount}===_claimCount: {_claimCount}===_releaseCount: {_releaseCount}===PoolSize: {GetSize()}");
		}
	}
}
