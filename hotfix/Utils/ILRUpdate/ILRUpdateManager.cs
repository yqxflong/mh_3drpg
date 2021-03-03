using System;
using System.Linq;
//ILRUpdateManager
//用于负责ILR层的Update函数统一分发
//Johny
using System.Collections.Generic;
using System.Text;
using _HotfixScripts.Utils;
using ILR.HotfixManager;

namespace Hotfix_LT
{
    public static class ILRUpdateManager
    {
        #region IUpdateable

        private static bool dirty = false;
        private static HashSet<Object> _mono_addedList = new HashSet<Object>();
        private static HashSet<Object> _mono_removedList = new HashSet<Object>();
        private static List<Object> _monoUpdateList = new List<Object>();
        private static List<Object> _monoLateList = new List<Object>();
        private static List<Object> _monoFixedList = new List<Object>();

        #endregion

        private static List<System.Action> _updateFuncList = null;
        private static List<System.Action> _fixedUpdateFuncList = null;
        private static List<System.Action> _lateUpdateFuncList = null;

        public static void RegisterNeedUpdateMono(IUpdateable obj)
        {
            _mono_addedList.Add(obj);
            _mono_removedList.Remove(obj);
            dirty = true;
        }

        public static void UnRegisterNeedUpdateMono(IUpdateable obj)
        {
            _mono_removedList.Add(obj);
            _mono_addedList.Remove(obj);
            dirty = true;
        }

        public static void Update()
        {
            #region monos

            var length = _monoUpdateList.Count;
            for (var i = 0; i < length; i++)
            {
                ((IHotfixUpdate) _monoUpdateList[i]).Update();
            }

            #endregion

            #region ILRTimerManager
            ILRTimerManager.instance.Update();
            #endregion
        }

        public static void FixedUpdate()
        {
            #region monos

            var length = _monoFixedList.Count;
            for (var i = 0; i < length; i++)
            {
                ((IHotfixFixedUpdate) _monoFixedList[i]).FixedUpdate();
            }

            #endregion
        }

        public static void LateUpdate()
        {
            #region monos

            {
                var length = _monoLateList.Count;
                for (var i = 0; i < length; i++)
                {
                    ((IHotfixLateUpdate) _monoLateList[i]).LateUpdate();
                }
            }

            if (dirty)
            {
                dirty = false;
                if (_mono_addedList.Count > 0)
                {
                    var it = _mono_addedList.GetEnumerator();
                    while (it.MoveNext())
                    {
                        var m = it.Current;
                        if (m is IHotfixUpdate)
                        {
                            if (_monoUpdateList.IndexOf(m) < 0)
                            {
                                _monoUpdateList.Add(m);
                            }
                        }

                        if (m is IHotfixLateUpdate)
                        {
                            if (_monoLateList.IndexOf(m) < 0)
                            {
                                _monoLateList.Add(m);
                            }
                        }

                        if (m is IHotfixFixedUpdate)
                        {
                            if (_monoFixedList.IndexOf(m) < 0)
                            {
                                _monoFixedList.Add(m);
                            }
                        }
                    }

                    _mono_addedList.Clear();
                }

                if (_mono_removedList.Count > 0)
                {
                    var it = _mono_removedList.GetEnumerator();
                    while (it.MoveNext())
                    {
                        var m = it.Current;
                        _monoUpdateList.Remove(m);
                        _monoLateList.Remove(m);
                        _monoFixedList.Remove(m);
                    }

                    _mono_removedList.Clear();
                }
            }

            #endregion

            #region ILRTimerManager
            ILRTimerManager.instance.LateUpdate();
            #endregion
        }
    }
}