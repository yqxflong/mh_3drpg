#if USE_DEBUG
#define ENABLE_LOGGING
#endif
using System.Collections;

namespace EB
{
    /// <summary>
    /// 日志输出管理器
    /// </summary>
	public static class Debug
	{
        /// <summary>
        /// 偶现BUG日志标签
        /// </summary>
        public static string ACCIDENTAL = "偶现BUG";
#if ENABLE_REMOTE_LOGGING
		private static Collections.CircularBuffer _buffer = new Collections.CircularBuffer(256);
#endif
		
		class FormatProvider : System.IFormatProvider, System.ICustomFormatter	
		{
			#region IFormatProvider implementation
			public object GetFormat (System.Type formatType)
			{
				return this;
			}
			#endregion
			
			#region ICustomFormatter implementation
			public string Format (string format, object arg, System.IFormatProvider formatProvider)
			{
				var result = string.Empty;
				if ( arg != null )
				{
					result = arg.ToString();
					if ( arg is ICollection )
					{
						result = JSON.Stringify(arg) ?? result;
					}
				}
				else
				{
					result = "null";
				}			
				return result;
			}
			#endregion
		}
		private static FormatProvider _provider = new FormatProvider();
		
		public static string Format( object message, params object[] args)
		{
			if (args == null || args.Length == 0)
			{
				return message.ToString();
			}
			
			return string.Format(_provider, message.ToString(), args);
		}
		
		public static void LogIf( bool condition, object message, params object[] args )
		{
			if ( condition )
			{
				Log(message,args);
			}
		}

        /// <summary>
        /// 场景管理_专用的日志输出通道
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogLoadLevel(object message, params object[] args)
        {
#if ENABLE_LOGGING
            //try { UnityEngine.Debug.Log(Format("<color=#53EFE2>[" + UnityEngine.Time.frameCount + "]Level:" + message + "</color>", args)); } catch { }
#endif
        }

        /// <summary>
        /// CoreAsset资源管理_专用的日志输出通道
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogCoreAsset(object message, params object[] args)
        {
#if ENABLE_LOGGING
            //try { UnityEngine.Debug.Log(Format("<color=#407FB5>[" + UnityEngine.Time.frameCount + "]CoreAsset:" + message + "</color>", args)); } catch { }
#endif
        }

        /// <summary>
        /// AssetManager资源管理_专用的日志输出通道
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogAssetManager(object message, params object[] args)
        {
#if ENABLE_LOGGING
            try { UnityEngine.Debug.Log(Format("<color=#AAAB36>[" + UnityEngine.Time.frameCount + "]AssetManager:" + message + "</color>", args)); } catch { }
#endif
        }
        
        /// <summary>
        /// ObjectManager缓存管理_专用的日志输出通道
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogObjectMgrAsset(object message, params object[] args)
        {
#if ENABLE_LOGGING
            //try { UnityEngine.Debug.Log(Format("<color=#1D8C52>[" + UnityEngine.Time.frameCount + "]ObjectManager:" + message + "</color>", args)); } catch { }
#endif
        }
        
        /// <summary>
        /// 特效缓存_专用的日志输出通道
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogPSPoolAsset(object message, params object[] args)
        {
#if ENABLE_LOGGING
            //try { UnityEngine.Debug.Log(Format("<color=#A46023>[" + UnityEngine.Time.frameCount + "]ps:" + message + "</color>", args)); } catch { }
#endif
        }
        
        /// <summary>
        /// UI管理_专用的日志输出通道
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogUI(object message, params object[] args)
        {
#if ENABLE_LOGGING
            //try { UnityEngine.Debug.Log(Format("<color=#C63998>[" + UnityEngine.Time.frameCount + "]ui:" + message + "</color>", args)); } catch { }
#endif
        }

        /// <summary>
        /// Lookup缓存管理_专用的日志输出通道
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogLookup(object message, params object[] args)
        {
#if ENABLE_LOGGING
            //try { UnityEngine.Debug.Log(Format("<color=#9EEA3B>[" + UnityEngine.Time.frameCount + "]" + message + "</color>", args)); } catch { }
#endif
        }

        /// <summary>
        /// UI管理_统计ui加载用时专用的日志输出通道
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogUIStatistics(object message, params object[] args)
        {
#if ENABLE_LOGGING
            //try { UnityEngine.Debug.Log(Format("<color=#D1792A>[" + UnityEngine.Time.frameCount + "]" + message + "</color>", args)); } catch { }
#endif
        }
        /// <summary>
        /// 战斗_专用的日志输出通道
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogCombat(object message, params object[] args)
        {
#if ENABLE_LOGGING
            try { UnityEngine.Debug.Log(Format(message, args)); } catch {}
#endif
        }

        /// <summary>
        /// ObjectManager缓存管理_专用的日志输出通道
        /// </summary>
        public static void LogGhostBug(object message, params object[] args)
        {
#if ENABLE_LOGGING
            try { UnityEngine.Debug.Log(Format("<color=#FF00FF>[" + UnityEngine.Time.frameCount + "GhostBug]:" + message + "</color>", args)); } catch { }
#endif
        }

        public static void Log( object message, params object[] args )
		{
#if ENABLE_LOGGING
			try { UnityEngine.Debug.Log(Format(message, args)); } catch {}
#endif

			
#if ENABLE_REMOTE_LOGGING
			lock(_buffer)
			{
                if (args != null)
                {
                    _buffer.Push(System.DateTime.UtcNow.ToString() + " I:" + Format(message, args));
                }
			}
#endif
		}

        

        public static void LogWarning(object message, params object[] args)
		{
#if ENABLE_LOGGING
			 try { UnityEngine.Debug.LogWarning(Format(message, args)); } catch {}
#endif
			
#if ENABLE_REMOTE_LOGGING
			lock(_buffer)
            {
                if (args != null)
                {
                    _buffer.Push(System.DateTime.UtcNow.ToString() + " W:" + Format(message, args));
                }
            }
#endif			
		}

		public static void LogError(object message, params object[] args)
		{
#if !EB_DISABLE_LOGERROR
			try { UnityEngine.Debug.LogError(Format(message, args)); } catch {}
#endif
			
#if ENABLE_REMOTE_LOGGING
			lock (_buffer)
            {
                if (args != null)
                {
                    _buffer.Push(System.DateTime.UtcNow.ToString() + " E:" + Format(message, args));
                }
            }
#endif				
		}

	    public static void PushToDebugBuffer(object message, params object[] args)
	    {
#if ENABLE_REMOTE_LOGGING
	        lock (_buffer)
            {
                if (args != null)
                {
                    _buffer.Push(System.DateTime.UtcNow.ToString() + " E:" + Format(message, args));
                }
            }
#endif
        }

        public static void Dump( Hashtable table )
		{
#if ENABLE_REMOTE_LOGGING
			ArrayList list = new ArrayList();
			lock(_buffer)
			{
				foreach( string s in _buffer )
				{
					list.Add(s);
				}
			}
			table["log"] = list;
#endif
		}

		public static void LogTs(string flag)
		{
#if ENABLE_LOGGING
			Log(flag+".TimeRecord:" + ((float)((System.DateTime.UtcNow.Ticks / 10000) % 100000) / 1000));
#endif
		}
    }

}

