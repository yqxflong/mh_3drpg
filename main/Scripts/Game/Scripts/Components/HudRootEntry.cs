using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;

/// <summary>
/// 
/// </summary>
public class HudRootEntry
{
	public delegate void Failed(string name);
	public delegate void Finished(string name);

	public Failed		OnFailed;
	public Finished		OnFinished;

	public string		m_HudName;
	public GameObject	m_Root;

	public HudRootEntry(string name)
	{
		m_HudName = name;
	}
	
	public void DestroyHud()
	{
		if (m_Root != null)
		{
			GameObject.Destroy(m_Root);
			m_Root = null;

			EB.Assets.UnloadAssetByName(m_HudName, true);
		}
	}
	
	public void Show()
	{
		if (m_Root != null)
		{
			EB.Debug.Log("[HudRootEntry]Show: m_HudName = {0},ts = {1}", m_HudName, Time.realtimeSinceStartup);
			m_Root.CustomSetActive(true);
		}
		else
		{
			EB.Debug.LogError("[HudRootEntry]Show: m_Root is null. m_HudName = {0}", m_HudName);
		}
	}
	
	public void Hide()
	{
		if (m_Root != null)
		{
			m_Root.CustomSetActive(false);
		}
	}
	
	public void LoadHudAsync(string name, Failed failed, Finished finish)
	{
		EB.Debug.Log("[HudRootEntry]LoadHudAsync: name = {0}, ts = {1}", name, Time.realtimeSinceStartup);

		OnFailed = failed;
		OnFinished = finish;

		if(m_Root != null)
		{
			EB.Debug.Log("[HudRootEntry]LoadHudAsync: The hud asset is already there.");
			HandleFinished();
			return;
		}

		// async load
		// GM.AssetManager.GetAsset<GameObject>(name, DownloadResult, GameEngine.Instance.gameObject);
		EB.Assets.LoadAsyncAndInit<GameObject>(name, DownloadResult, GameEngine.Instance.gameObject);
	}

	void DownloadResult(string assetname, GameObject go, bool bSuccessed)
	{
		EB.Debug.Log("[HudRootEntry]DownloadResult: assetname = {0}, bSuccessed = {1}, ts = {2}", assetname, bSuccessed, Time.realtimeSinceStartup);

		if (bSuccessed)
		{
			m_Root = go;
			m_Root.name = assetname;
			HandleFinished();
		}
		else
		{
			m_Root = null;
			HandleFaild();
		}
	}
	
	void ClearListener()
	{
		OnFailed = null;
		OnFinished = null;
	}

	void HandleFaild ()
	{
		if (OnFailed != null)
		{
			OnFailed(m_HudName);
		}
		
		ClearListener();
		PostLoadCleanUp();
	}
	
	void HandleFinished ()
	{
		this.Hide();
		
		if (OnFinished != null)
		{
			OnFinished(m_HudName);
		}
		
		ClearListener();
		//PostLoadCleanUp();
	}
	
	void PostLoadCleanUp()
	{
		//GM.AssetManager.UnloadUnusedAssets();
		//System.GC.Collect();
	}
}