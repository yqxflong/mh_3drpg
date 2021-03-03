using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace EB
{
	public class ThreadPool
	{
		private int _maxThreads;
		public int MaxThreads { get { return _maxThreads; } }

		private int _poolState;
		
		public class AsyncTask
		{
			public System.Exception exception;
			public bool done;
			public System.Action<object> action;
			public object state;
		}
		
		EB.Collections.Queue<AsyncTask> _queue;
		System.Threading.AutoResetEvent _event;
		bool _running = false;
		
		public ThreadPool(int maxThreads)
		{
			_maxThreads = maxThreads;
			_queue = new EB.Collections.Queue<AsyncTask>(16);
			_event = new System.Threading.AutoResetEvent(false);
			Start();
		}
		
		/// <summary>
		/// called in master thread
		/// </summary>
		/// <param name="action"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public AsyncTask Queue(System.Action<object> action, object state )
		{
			AsyncTask task = new AsyncTask();
			task.action = action;
			task.state = state;
			
			lock(_queue)
			{
				_queue.Enqueue(task);
			}
			
			Wakeup();
			return task;
		}
		
		/// <summary>
		/// called in master thread
		/// </summary>
		public void Wait()
		{
			// join the queue until its done
			while( JoinForOne() ) { }
		}

		/// <summary>
		/// called in master thread
		/// </summary>
		/// <param name="maxThreads"></param>
		public void Resize(int maxThreads)
		{
			if (maxThreads == _maxThreads)
			{
				return;
			}
			
			int currentThreads = _maxThreads;

			// wake up all threads to finish
			++_poolState;
			_maxThreads = maxThreads;
			for (int i = 0; i < currentThreads; ++i)
			{
				_event.Set();
			}

			// create threads
			for (int i = currentThreads; i < maxThreads; ++i)
			{
				var thread = new System.Threading.Thread(this._Thread, 256 * 1024);
				thread.Start(new Hashtable() { { "poolIndex", i }, { "poolState", _poolState } });
			}
		}
		
		bool JoinForOne()
		{
			AsyncTask task = null;
					
			lock(_queue)
			{
				if ( _queue.Count > 0 )
				{
					task = _queue.Dequeue();
				}
			}
			
			if ( task == null )
			{
				return false;
			}

			//EB.Debug.Log("ThreadPool.JoinForOne: start run task");
			try
			{
				task.action(task.state);
			}
			catch(System.Exception e ) 
			{
				task.exception = e;
			}
			task.done = true;
			//EB.Debug.Log("ThreadPool.JoinForOne: end run task");
			return true;
		}
		
		void _Thread(object param)
		{
			//EB.Debug.Log("ThreadPool._Thread: start");

			Hashtable ht = param as Hashtable;
			int index = (int)ht["poolIndex"];
			int state = (int)ht["poolState"];

			try
			{
				System.Threading.Thread.CurrentThread.Name = "threadpool";
			}catch {}
			
			while (_running)
			{
				// resize pool
				if (state != _poolState)
				{
					if (index >= _maxThreads)
					{
						break;
					}

					state = _poolState;
				}

				if (!JoinForOne())
				{
					_event.WaitOne();
				}
			}

			//EB.Debug.Log("ThreadPool._Thread: exit");
		}
		
		void Start()
		{
			if ( !_running )
			{
				_running = true;
				for ( int i = 0; i < _maxThreads; ++i )
				{
					var thread = new System.Threading.Thread(this._Thread, 256 * 1024);
					thread.Start(new Hashtable() { { "poolIndex", i }, { "poolState", _poolState } });
				}
			}
		}
		
		void Wakeup()
		{
			// wake up threads
			try
			{
				_event.Set();
			}
			catch {}
		}
	}
}