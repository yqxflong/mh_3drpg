using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	public abstract class Manager : System.IDisposable
	{
		// shortcut
		protected Hub Hub { get {return Hub.Instance;} }
		
		public abstract void Initialize( Config config );

		protected Coroutine StartCoroutine( IEnumerator fn )
		{
			return Hub.StartCoroutine(fn);
		}
		
		public virtual void OnLoggedIn() {}
		public virtual void OnEnteredBackground() {}
		public virtual void OnEnteredForeground() {}
		
		// for async notifications
		public virtual void Async( string message, object payload ) {}
		
		#region IDisposable implementation
		public virtual void Dispose ()
		{
		}
		#endregion
	}
}
