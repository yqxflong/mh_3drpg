using UnityEngine;
using System.Collections;

namespace EB
{
	public class Coroutines  
	{
		class IntervalHandle
		{	
			public bool Running = true;
			public Coroutine Couroutine = null;
		}
		
		private static CoreCoroutines _this = null;

		public static Coroutine Run( IEnumerator function )
		{
			if ( _this == null )
			{
				GameObject go = new GameObject("~COROUTINES");
				GameObject.DontDestroyOnLoad(go);
				go.hideFlags = HideFlags.HideAndDontSave;
				_this = go.AddComponent<CoreCoroutines>();
			}
			return _this.StartCoroutine(function);
		}

		public static void Stop(Coroutine coroutine)
		{
			if (_this == null)
			{
				EB.Debug.LogError("Coroutines.Stop: COROUTINES not created");
				return;
			}

			if (coroutine != null)
			{
				_this.StopCoroutine(coroutine);
			}
		}

		public static void Stop(IEnumerator coroutine)
		{
			if (_this == null)
			{
				EB.Debug.LogError("Coroutines.Stop: COROUTINES not created");
				return;
			}

			if (coroutine != null)
			{
				_this.StopCoroutine(coroutine);
			}
		}

		public static void StopAll()
		{
			if (_this == null)
			{
				EB.Debug.LogError("Coroutines.Stop: COROUTINES not created");
				return;
			}
            Debug.LogError("���ܵ�������ӿڣ���Ϊ���п��ܻ���������Э����Ҫ");
			_this.StopAllCoroutines();
		}
		
		public static object SetUpdate( System.Action cb )
		{
			IntervalHandle handle = new IntervalHandle();
			Run (_SetUpdate(handle,cb));
			return handle;
		}
		
		public static void ClearUpdate( object handle )
		{
            ClearInterval(handle);
		}
		
		public static object SetInterval( System.Action cb, int ms )
		{
			IntervalHandle handle = new IntervalHandle();
			Run (_SetInterval(handle,cb,ms));
			return handle;
		}
		
		public static void ClearInterval( object handle )
		{
			if ( handle != null && handle is IntervalHandle )
			{
                IntervalHandle intervalHandle = (IntervalHandle)handle;
                intervalHandle.Running = false;
                Stop(intervalHandle.Couroutine);
			}
		}
		
		static IEnumerator _SetUpdate( IntervalHandle handle, System.Action cb )
		{
			while(true)
			{
				if (handle.Running)
				{
					cb();
				}
				else 
				{
					yield break;
				}
				
				yield return null;
			}
		}
		
		static IEnumerator _SetInterval( IntervalHandle handle, System.Action cb, int ms )
		{
			var seconds = ms / 1000.0f;
			while(true)
			{
				var end = Time.realtimeSinceStartup + seconds;
				while ( Time.realtimeSinceStartup < end )
				{
					yield return null;
				}
				
				if ( handle.Running )
				{
					cb();
				}
				else
				{
					yield break;
				}
			}
		}
		
		public static object SetTimeout( System.Action cb, int ms )
		{
			IntervalHandle handle = new IntervalHandle();
			handle.Couroutine = Run(_SetTimeout(handle,cb,ms));
			return handle;
		}
		
		public static void ClearTimeout( object handle )
		{
            ClearInterval(handle);
		}
		
		static IEnumerator _SetTimeout( IntervalHandle handle, System.Action cb, int ms )
		{
			var seconds = ms / 1000.0f;
			yield return new WaitForSeconds(seconds);
			if (handle.Running) {
				cb();
			}
		}
		
		public static Coroutine NextFrame( System.Action cb )
		{
			return Run(_NextFrame(cb));
		}
		
		static IEnumerator _NextFrame( System.Action cb )
		{
			yield return null;
			cb();
		}
		
		public static Coroutine EndOfFrame( System.Action cb )
		{
			return Run(_EndOfFrame(cb));
		}
		
		static IEnumerator _EndOfFrame( System.Action cb )
		{
			yield return new WaitForEndOfFrame();
			cb();
		}
	}	
}


// for unity
public class CoreCoroutines : MonoBehaviour {}

