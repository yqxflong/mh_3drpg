
//HashtablePool
//根据Capacity分档缓存hashtable
//Johny

// #define JOHNY_NO_RELEASE
// #define DEBUG_CLAIM_STACKS

using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Johny {
	public enum HashtablePoolType
	{
		t16 = 16,
		t32 = 32,
		t64 = 64,
		tOther = 1024,
	}

	public static class HashtablePool{
		/** Internal pool */
		static readonly Dictionary<HashtablePoolType, Stack<Hashtable>> pool = new Dictionary<HashtablePoolType, Stack<Hashtable>>{
			//0~16
			{HashtablePoolType.t16, new Stack<Hashtable>()},
			//17~32
			{HashtablePoolType.t32, new Stack<Hashtable>()},
			//33~64
			{HashtablePoolType.t64, new Stack<Hashtable>()},
			//other
			{HashtablePoolType.tOther, new Stack<Hashtable>()},
		};

		static readonly HashSet<Hashtable> inPool = new HashSet<Hashtable>();

		#region DEBUG INFO
		private static int _newCount = 0, _claimCount = 0, _releaseCount = 0;
#if UNITY_EDITOR
		private static Dictionary<HashtablePoolType, int> _debug_newInfo = new Dictionary<HashtablePoolType, int>{
			{HashtablePoolType.t16, 0},
			{HashtablePoolType.t32, 0},
			{HashtablePoolType.t64, 0},
			{HashtablePoolType.tOther, 0},
		};

		private static Dictionary<string, int> _debug_claimStacks = new Dictionary<string, int>();
#endif
		#endregion

		private static HashtablePoolType GetTypeByCapacity(int cap){
			if(cap <= (int)HashtablePoolType.t16){
				return HashtablePoolType.t16;
			}
			else if(cap <= (int)HashtablePoolType.t32){
				return HashtablePoolType.t32;
			}
			else if(cap <= (int)HashtablePoolType.t64){
				return HashtablePoolType.t64;
			}

			return HashtablePoolType.tOther;
		}

		#region 快捷API
		/// <summary>
		/// 根据dic的count找到对应档位取出ht
		/// </summary>
		/// <param name="dic"></param>
		/// <returns></returns>
		public static Hashtable Claim(IDictionary dic){
			var tp = GetTypeByCapacity(dic.Count);
			var ht = Claim(tp);
			var it = dic.GetEnumerator();
			while(it.MoveNext()){
				ht.Add(it.Key, it.Value);
			}
			return ht;
		}

		/// <summary>
		/// 有回收后还使用的情况，所以先不要缓存，后面具体模块自己做缓存。
		/// </summary>
		/// <param name="ht"></param>
		public static void ReleaseRecursion(Hashtable ht){
			// if(ht == null){
			// 	return;
			// }
			
			// var it = ht.GetEnumerator();
			// while(it.MoveNext()){
			// 	if(it.Value is Hashtable){
			// 		ReleaseRecursion(it.Value as Hashtable);
			// 	}
			// 	else if(it.Value is ArrayList){
			// 		ArrayListPool.ReleaseRecursion(it.Value as ArrayList);
			// 	}
			// }
			// Release(ht);
		}
		#endregion

		/// <summary>
		/// 有回收后还使用的情况，所以先不要缓存，后面具体模块自己做缓存。
		/// </summary>
		/// <param name="count"></param>
		public static void Prepare(int count){
			// for(int i = 0; i < count; i++){
			// 	var ht = new Hashtable((int)HashtablePoolType.t16);
			// 	if(inPool.Add(ht)){
			// 		pool[HashtablePoolType.t16].Push(ht);
			// 	}
			// }
		}

		/// <summary>
		/// 有回收后还使用的情况，所以先不要缓存，后面具体模块自己做缓存。
		/// </summary>
		/// <param name="tp"></param>
		/// <returns></returns>
		public static Hashtable Claim (HashtablePoolType tp = HashtablePoolType.t16) {
// #if UNITY_EDITOR && DEBUG_CLAIM_STACKS
// 			string stackInfo = new System.Diagnostics.StackTrace().ToString();
// 			if(_debug_claimStacks.ContainsKey(stackInfo)){
// 			   _debug_claimStacks[stackInfo] += 1;
// 			}
// 			else{
// 			   _debug_claimStacks[stackInfo] = 1;
// 			}
// #endif		
			
// 			var list = pool[tp];
// 			if (list.Count > 0) {
// 				_claimCount++;
// 				Hashtable ht = list.Pop();
// 				inPool.Remove(ht);
// 				return ht;
// 			}

// 			_newCount++;
// #if UNITY_EDITOR
// 			_debug_newInfo[tp]++;
// #endif
			return new Hashtable((int)tp);
		}

		/// <summary>
		/// 有回收后还使用的情况，所以先不要缓存，后面具体模块自己做缓存。
		/// </summary>
		/// <param name="ht"></param>
		public static void Release (Hashtable ht) {
// #if !JOHNY_NO_RELEASE
// 			if(ht != null){
// 				ht.Clear();
// 				if (inPool.Add(ht)) {
// 					_releaseCount++;
// 					var tp = GetTypeByCapacity(ht.Count);
// 					pool[tp].Push(ht);
// 				}
// 			}
// #endif
		}

		public static void Clear () {
			inPool.Clear();
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

#if UNITY_EDITOR
		public static void DebugConsole_ClaimInfo(){
			EB.Debug.LogError($"HashtablePool: _newCount: {_newCount}===_claimCount: {_claimCount}===_releaseCount: {_releaseCount}===PoolSize: {GetSize()}");
			EB.Debug.LogError("============HashtablePool Claim Stack Info================");
			var keys = _debug_claimStacks.Keys.ToList();
			var vals = _debug_claimStacks.Values.ToList();
			vals.Sort();
			for(int i = 0; i < vals.Count; i++)
			{
				var val = vals[i];
				EB.Debug.LogError($"{keys[i]} ===> {val}");
			}
			EB.Debug.LogError("============HashtablePool Claim Stack Info================");
		}

		public static void DebugConsole_NewDetailInfo(){
			StringBuilder sb = new StringBuilder();
			var it = _debug_newInfo.GetEnumerator();
			while(it.MoveNext()){
				sb.Append(it.Current.Key);
				sb.Append(": ");
				sb.Append(it.Current.Value);
				sb.Append(";");
			}
			EB.Debug.LogError($"HashtablePool: {sb.ToString()}");
		}
#endif
	}
}

