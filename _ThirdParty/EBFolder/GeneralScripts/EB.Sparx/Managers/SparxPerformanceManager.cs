using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EB.Sparx
{
	public class PerformanceConfig
	{
		public delegate void DataLoaded(object data);
		public DataLoaded DataLoadedHandler = null;

		public delegate int GetPlatform();
		public GetPlatform GetPlatformHandler = null;
	}

	public class PerformanceManager : SubSystem
	{
        public static string PerformanceUserSetting = null;
        public static string PerformanceCurSetting = "High";
        public static string PerformanceCurVersion = null;

        public static object PerformanceSettingData ;
        PerformanceConfig _config = new PerformanceConfig();
		PerformanceAPI _api = null;

		public object Info { get; private set; }

        public Dictionary<string, object> wrapper = new Dictionary<string, object>();

        public override void Initialize (EB.Sparx.Config config)
		{
			this._config = config.PerformanceConfig;
			_api = new PerformanceAPI(Hub.ApiEndPoint);
		}
		
		public override void Connect ()
		{
            ResetPerformanceData();
            State = EB.Sparx.SubSystemState.Connected;
		}
				
		public override void Disconnect (bool isLogout)
		{
		} 

        public void ResetPerformanceData(System.Action cb = null)
        {
            string path = string.Format("performance.{0}", PerformanceUserSetting != null ? PerformanceUserSetting : PerformanceCurSetting);
            object performanceData = (wrapper.ContainsKey(path)) ? wrapper[path] : null;

            if (performanceData != null)
            {
                OnFetch(null, performanceData);
                if (cb != null)
                {
                    cb();
                }
            }
            else
            {
                Fetch(cb);
            }
        }

		void OnFetch( string err, object data )
		{
			Info = null;
			if (data == null)
			{
				//EB.Debug.LogError("PerformanceManager: fetched null data : " + err);
				return;
			}
			if (_config.DataLoadedHandler != null)
			{
				_config.DataLoadedHandler(data);
			}
		}

#if DEBUG
		public static bool EnablePerformanceDebug = false;
		public static int PerfrmaceLevelDebug = 1;
#endif

		public void Fetch(System.Action cb = null, bool force = false, string forceDevice = "")
		{
			//only fetch once
			if (Info != null && !force)
			{
				if (cb != null)
				{
					cb();
				}
				return;
			}

			string device =(forceDevice.Length == 0) ? UnityEngine.SystemInfo.deviceModel : forceDevice;
			string CPU =UnityEngine.SystemInfo.processorType;
			string GPU = UnityEngine.SystemInfo.graphicsDeviceName;
			int memorySize = UnityEngine.SystemInfo.systemMemorySize;
            int graphicsMemorySize = UnityEngine.SystemInfo.graphicsMemorySize;
#if DEBUG
            if (EnablePerformanceDebug)
			{
				switch(PerfrmaceLevelDebug)
				{
					case 1:
						device = "LowDebugDevice";
						CPU = "LowDebugCPU";
						GPU = "LowDebugGPU";
						memorySize = 996;
						break;
					case 2:
						device = "MediumDebugDevice";
						CPU = "MediumDebugCPU";
						GPU = "MediumDebugGPU";
						memorySize = 2560;
						break;
					case 3:
					default:
						device = "HighDebugDevice";
						CPU = "HighDebugCPU";
						GPU = "HighDebugGPU";
						memorySize = 3789;
						break;
				}
			}
#endif
			int platform = -1;
			if (_config.GetPlatformHandler != null)
			{
				platform = _config.GetPlatformHandler();
			}

			_api.Fetch(device, CPU, GPU, platform, memorySize, delegate(string err, object result)
			{
				if (string.IsNullOrEmpty(err)) 
				{
                    //�汾�Ų�ͬʱ�жϣ�����ʼ��Ϸ��һ����Ҫ���
                    /*string ProfileVersion = EB.Dot.String("profileVersion", result, null);
                    if (!string.IsNullOrEmpty(ProfileVersion)&&!string.IsNullOrEmpty( PerformanceCurVersion)&&!ProfileVersion.Equals(PerformanceCurVersion))
                    {
                        _api.Fetch(device, CPU, GPU, platform, memorySize,delegate(string newerr, object newresult) {
                            if (string.IsNullOrEmpty(newerr))OnFetch(string.Empty, newresult);
                            else OnFetch(newerr, null);
                            if (cb != null)cb();
                        }, null);
                        return;
                    }*/
                    
                    //��ȡ��cpu����Ϊdefaultʱ,�ɱ��������ж�����
                    string profileCPU = EB.Dot.String("profile.name", result, null);
                    if(profileCPU.Equals("Default"))
                    {
                        PerformanceUserSetting = JudgeQualityLevel(memorySize ,graphicsMemorySize);
                        _api.Fetch(device, CPU, GPU, platform, memorySize, delegate (string newerr, object newresult) {
                            if (string.IsNullOrEmpty(newerr)) OnFetch(string.Empty, newresult);
                            else OnFetch(newerr, null);
                            if (cb != null) cb();
                        }, PerformanceUserSetting);
                        return;
                    }

                    OnFetch(string.Empty, result);
				}	
				else
				{
					OnFetch(err, null);
				}
				if (cb != null)
				{
					cb();
				}
			}, PerformanceUserSetting);
		}

        private string JudgeQualityLevel(int memorySize, int graphicsMemorySize)
        {
            if (graphicsMemorySize>2000&& memorySize>4000)
            {
                return "High";
            }
            else if (graphicsMemorySize > 1000 && memorySize > 3000)
            {
                return "Medium";
            }
            else
            {
                return "Low";
            }
        }
	}
}

