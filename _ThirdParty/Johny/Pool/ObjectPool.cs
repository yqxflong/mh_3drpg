//ObjectPool
//改装版
//Johny

// #define JOHNY_NO_POOLING

using System.Collections.Generic;

namespace Johny {
	public static class ObjectPool<T> where T : class, IEBPooledObject, new(){
		private readonly static List<T> pool = new List<T>();

		public static void Prepare(int count){
			for(int i = 0; i < count; i++){
				pool.Add(new T());
			}
		}

		public static void Clear () {
			pool.Clear();
		}

		public static T Claim () {
#if JOHNY_NO_POOLING
			return new T();
#else
			if (pool.Count > 0) {
				T ls = pool[pool.Count-1];
				pool.RemoveAt(pool.Count-1);
				return ls;
			} else {
				return new T();
			}
#endif
		}

		public static void Warmup (int count) {
			T[] tmp = new T[count];
			for (int i = 0; i < count; i++) tmp[i] = Claim();
			for (int i = 0; i < count; i++) Release(tmp[i]);
		}

		public static void Release (T obj) {
#if !JOHNY_NO_POOLING
			obj.OnEnterPool();
			pool.Add(obj);
#endif
		}

		/** Number of objects of this type in the pool */
		public static int GetSize () {
			return pool.Count;
		}
	}
}
