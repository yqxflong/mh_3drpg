//ArrayListPool
//缓存ArrayList
//Johny

// #define JOHNY_NO_RELEASE

using System.Collections;
using System.Collections.Generic;

namespace Johny {
	public static class ArrayListPool{
		static readonly List<ArrayList> pool = new List<ArrayList>();
		static readonly HashSet<ArrayList> inPool = new HashSet<ArrayList>();

		const int DefaultCapacity = 8;

		private static int _claimCount = 0, _releaseCount = 0;

		#region 快捷API
		/// <summary>
		/// 递归回收. 慎用！！
		/// </summary>
		/// <param name="ht"></param>
		public static void ReleaseRecursion(ArrayList al){
			if(al == null){
				return;
			}
			
			var it = al.GetEnumerator();
			while(it.MoveNext()){
				if(it.Current is Hashtable){
					HashtablePool.ReleaseRecursion(it.Current as Hashtable);
				}
				else if(it.Current is ArrayList){
					ReleaseRecursion(it.Current as ArrayList);
				}
			}
			Release(al);
		}
		#endregion

		public static void Prepare(int count){
			for(int i = 0; i < count; i++){
				var al = new ArrayList(DefaultCapacity);
				if(inPool.Add(al)){
					pool.Add(al);
				}
			}
		}

		public static ArrayList Claim (int capacity = DefaultCapacity) {
			_claimCount++;
			if (pool.Count > 0) {
				ArrayList ls = pool[pool.Count-1];
				pool.RemoveAt(pool.Count-1);
				inPool.Remove(ls);
				return ls;
			}

			return new ArrayList(DefaultCapacity);
		}

		/// <summary>
		/// 直接回收，里面的items忽略
		/// </summary>
		/// <param name="ht"></param>
		public static void Release (ArrayList ht) {
#if !JOHNY_NO_RELEASE
			ht.Clear();
			if (inPool.Add(ht)) {
				_releaseCount++;
				pool.Add(ht);
			}
#endif
		}

		public static void Clear () {
			inPool.Clear();
			pool.Clear();
		}

		public static int GetSize () {
			return pool.Count;
		}

#if UNITY_EDITOR 
		public static void DebugConsole_ClaimInfo(){
			EB.Debug.LogError($"ArrayListPool: _claimCount: {_claimCount}===_releaseCount: {_releaseCount}");
		}
#endif
	}
}
