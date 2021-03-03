using System;
using System.Collections.Generic;

namespace LT
{
    public static class MainMessenger
    {
        #region Internal variables
        static public Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

        //Message handlers that should never be removed, regardless of calling Cleanup
        static public List<string> permanentMessages = new List<string>();
        #endregion

        #region AddListener
        static public void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
        {
            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, null);
            }
        }

        static public void OnListenerRemoved(string eventType)
        {
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }

        //No parameters
        static public void AddListener(string eventType, Action handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Action)eventTable[eventType] + handler;
        }

        //Single parameter
        static public void AddListener<T>(string eventType, Action<T> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Action<T>)eventTable[eventType] + handler;
        }

        //Two parameters
        static public void AddListener<T, U>(string eventType, Action<T, U> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Action<T, U>)eventTable[eventType] + handler;
        }

        //Three parameters
        static public void AddListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Action<T, U, V>)eventTable[eventType] + handler;
        }
        static public void AddListenerEx<T>(string eventType, Func<T> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Func<T>)eventTable[eventType] + handler;
        }
        static public void AddListenerEx<T, U>(string eventType, Func<T, U> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Func<T, U>)eventTable[eventType] + handler;
        }
        static public void AddListenerEx<T, U, V>(string eventType, Func<T, U, V> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Func<T, U, V>)eventTable[eventType] + handler;
        }
        #endregion

        #region RemoveListener
        //No parameters
        static public void RemoveListener(string eventType, Action handler)
        {
            eventTable[eventType] = (Action)eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }

        //Single parameter
        static public void RemoveListener<T>(string eventType, Action<T> handler)
        {
            eventTable[eventType] = (Action<T>)eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }

        //Two parameters
        static public void RemoveListener<T, U>(string eventType, Action<T, U> handler)
        {
            eventTable[eventType] = (Action<T, U>)eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }

        //Three parameters
        static public void RemoveListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            eventTable[eventType] = (Action<T, U, V>)eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }
        static public void RemoveListenerEx<T>(string eventType, Func<T> handler)
        {
            eventTable[eventType] = (Func<T>)eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }
        static public void RemoveListenerEx<T, U>(string eventType, Func<T, U> handler)
        {
            eventTable[eventType] = (Func<T, U>)eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }
        static public void RemoveListenerEx<T, U, V>(string eventType, Func<T, U, V> handler)
        {
            eventTable[eventType] = (Func<T, U, V>)eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }
        #endregion

        #region Broadcast
        //No parameters
        static public void Raise(string eventType)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Action callback = d as Action;

                if (callback != null)
                {
                    callback();
                }
                else
                {
                    EB.Debug.LogError(eventType);
                }
            }
        }

        //Single parameter
        static public void Raise<T>(string eventType, T arg1)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Action<T> callback = d as Action<T>;

                if (callback != null)
                {
                    callback(arg1);
                }
                else
                {
                    EB.Debug.LogError(eventType);
                }
            }
        }

        //Two parameters
        static public void Raise<T, U>(string eventType, T arg1, U arg2)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Action<T, U> callback = d as Action<T, U>;

                if (callback != null)
                {
                    callback(arg1, arg2);
                }
                else
                {
                    EB.Debug.LogError(eventType);
                }
            }
        }

        //Three parameters
        static public void Raise<T, U, V>(string eventType, T arg1, U arg2, V arg3)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Action<T, U, V> callback = d as Action<T, U, V>;

                if (callback != null)
                {
                    callback(arg1, arg2, arg3);
                }
                else
                {
                    EB.Debug.LogError(eventType);
                }
            }
        }

        //zero parameters, one return
        static public T RaiseEx<T>(string eventType)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Func<T> callback = d as Func<T>;

                if (callback != null)
                {
                    return callback();
                }
                else
                {
                    EB.Debug.LogError(eventType);
                }
            }

            return default(T);
        }

        //one parameters, one return
        static public U RaiseEx<T, U>(string eventType, T arg1)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Func<T, U> callback = d as Func<T, U>;

                if (callback != null)
                {
                    return callback(arg1);
                }
                else
                {
                    EB.Debug.LogError(eventType);
                }
            }

            return default(U);
        }

        //two parameters, one return
        static public V RaiseEx<T, U, V>(string eventType, T arg1, U arg2)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Func<T, U, V> callback = d as Func<T, U, V>;

                if (callback != null)
                {
                    return callback(arg1, arg2);
                }
                else
                {
                    EB.Debug.LogError(eventType);
                }
            }

            return default(V);
        }
        #endregion
    }
}
