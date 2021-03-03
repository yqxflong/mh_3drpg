using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataLookupSparxManager : EB.Sparx.SubSystem
{
	private Dictionary<EB.Sparx.Manager, List<string>> m_excludes = new Dictionary<EB.Sparx.Manager, List<string>>();

	private EB.Sparx.EndPoint m_endPoint;
	public EB.Sparx.EndPoint EndPoint
	{
		get { return m_endPoint; }
	}

	#region implement EB.Sparx.Manager
	public override void Initialize(EB.Sparx.Config config)
	{
		m_endPoint = Hub.ApiEndPoint;
		if (Debug.isDebugBuild) DataLookupDebug.Create();

		// PushManager should Initialize before DataLookupSparxManager
		EB.Sparx.PushManager pm = Hub.PushManager;
		if (pm != null)
		{
			pm.OnHandleMessage += new System.Action<string, string, object>(OnHandleMessage);
		}
	}

	public override void Dispose()
	{
		if (Debug.isDebugBuild)
		{
			DataLookupDebug.Dispose();
		}

		m_endPoint = null;
	}

	public override void Async(string message, object options)
	{
		if (options is IDictionary) {
			HandleIncomingData(options as IDictionary);
		}
	}
	#endregion

	public void Service(EB.Sparx.Request request, System.Action<EB.Sparx.Response> callback = null, bool cacheData = true)
	{
		System.Action<EB.Sparx.Response> onResponse = obj => StartCoroutine(OnResponseCoroutine(obj, callback, cacheData));
		m_endPoint.Service(request, onResponse);
	}

	public int NetworkLatencySimulationDuration = 0;

	private IEnumerator OnResponseCoroutine(EB.Sparx.Response response, System.Action<EB.Sparx.Response> callback = null, bool cacheData = true)
	{
		yield return new WaitForSeconds(NetworkLatencySimulationDuration); // simulate network latency

		if (cacheData)
		{
			IDictionary dataDic = response.result as IDictionary;

			if (dataDic == null)
				dataDic = Johny.HashtablePool.Claim();

			dataDic["serverError"] = response.error;
			HandleIncomingData(dataDic);
		}

		if (callback != null) callback(response);
	}

	/// <summary>
	/// 处理接收数据（T）
	/// </summary>
	/// <param name="data"></param>
	/// <typeparam name="T"></typeparam>
	private void HandleIncomingData<T>(T data)
	{
        if (EB.Dot.Object("alliance.account.medal", data, null) != null)
        {
            //这部分数据需要替换，不megra
            DataLookupsCache.Instance.CacheData("alliance.account.medal", null);
        }

		DataLookupsCache.Instance.CacheData(data);
	}

	private void OnHandleMessage(string component, string message, object payload)
	{
		if (!ShouldExclude(Hub.GetManager(component), message))
		{
			Async(message, payload);
		}
	}

	public override void Connect()
	{
		State = EB.Sparx.SubSystemState.Connected;
	}

	public override void Disconnect(bool isLogout)
	{
		DataLookupsCache.Instance.ClearCache();
		State = EB.Sparx.SubSystemState.Disconnected;
	}

	public void AddExclude(EB.Sparx.Manager manager, string message)
	{
		message = message.ToLower();
		if (m_excludes.ContainsKey(manager))
		{
			List<string> excludes = m_excludes[manager];
			if (!excludes.Contains(message))
			{
				excludes.Add(message);
			}
		}
		else
		{
			m_excludes[manager] = new List<string>() { message };
		}
	}

	public void RemoveExclude(EB.Sparx.Manager manager, string message)
	{
		if (m_excludes.ContainsKey(manager))
		{
			message = message.ToLower();
			List<string> excludes = m_excludes[manager];
			excludes.Remove(message);

			if (excludes.Count == 0)
			{
				m_excludes.Remove(manager);
			}
		}
	}

	private bool ShouldExclude(EB.Sparx.Manager manager, string message)
	{
		if (manager == null)
		{
			return false;
		}

		if (!m_excludes.ContainsKey(manager))
		{
			return false;
		}

		message = message.ToLower();
		List<string> excludes = m_excludes[manager];
		return excludes.Contains(message);
	}
}
