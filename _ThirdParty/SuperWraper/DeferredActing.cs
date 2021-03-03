using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

namespace Unity.Standard.ScriptsWarp
{
	public class DeferredActing : Delegater
	{
		private CancellationTokenSource tokenSource;
		public CancellationTokenSource TokenSource { get{ if (tokenSource == null) tokenSource = new CancellationTokenSource(); return tokenSource; } }

		public bool IsWaitFrame { get; set; }
		private int waitFrame;

		public Coroutine Coroutine { get; private set; }


		// Use this for initialization
		public static DeferredActing Create(float wait, GameObject provider, TheDelegater func, ActionMode mode = default(ActionMode), string actionName = "", WaiterType type = WaiterType.Inovker, bool isFrame = false)
		{
			DeferredActing delayer = null;
			switch (mode)
			{
				case ActionMode.Ascending:
					delayer = provider.AddComponent<DeferredActing>();
					break;
				case ActionMode.Require:
					DeferredActing[] deferrers = provider.GetComponents<DeferredActing>(); bool has = false;
					foreach(var deferrer in deferrers) { if (deferrer.DelegaterName == actionName) { delayer = deferrer; has = true; break; } }
					if(!has) delayer = provider.AddComponent<DeferredActing>();
					break;
				case ActionMode.Replace:
					DeferredActing[] _deferrers = provider.GetComponents<DeferredActing>();
					foreach (var deferrer in _deferrers) { if (deferrer.DelegaterName == actionName) { Destroy(deferrer); } }
					delayer = provider.AddComponent<DeferredActing>();
					break;
			}
			delayer.SetDelegater(func);
			switch(type)
			{
				case WaiterType.Coroutine:
					delayer.IsWaitFrame = isFrame;
					delayer.Coroutine = delayer.StartCoroutine(delayer.CoroutineCallback(wait));
					break;
				case WaiterType.Inovker:
					delayer.Invoke("InvokeCallback", wait);
					break;
				case WaiterType.AsyncAwait:
					delayer.AsyncCall(wait, delayer.TokenSource);
					break;
			}
			delayer.DelegaterName = actionName;

			return delayer;
		}

		private IEnumerator CoroutineCallback(float interval)
		{
			if(IsWaitFrame)
			{
				waitFrame = Mathf.RoundToInt(interval);				
				yield return new WaitForEndOfFrame();			
			}
			else
			{
				yield return new WaitForSeconds(interval);
				OnDelegater();
				Destruct();
			}
			waitFrame = 0;
		}

		private void InvokeCallback()
		{
			OnDelegater();
			Destruct();
		}

		private async void AsyncCall(float interval, CancellationTokenSource tokenSource)
		{
			try
			{
				await Task.Delay(Mathf.RoundToInt(interval * 1000), tokenSource.Token);				
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
			}

			if (!tokenSource.IsCancellationRequested)
			{
				OnDelegater();
			}
		}

		public void Stop()
		{
			StopAllCoroutines();
			Destruct();
		}

		public void StopInovke()
		{
			if (IsInvoking("InvokeCallback"))
				CancelInvoke("InvokeCallback");
			Destruct();
		}

		public void StopTask()
		{
			TokenSource.Cancel();
			Destruct();
		}

		public enum ActionMode
		{
			Ascending,
			Require,
			Replace
		}

		public enum WaiterType
		{
			Coroutine,
			Inovker,
			AsyncAwait
		}
	}
}