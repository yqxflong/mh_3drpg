///////////////////////////////////////////////////////////////////////
//
//  ModelCatalog.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

public abstract class ModelCatalog<T> where T : BaseDataModel
{
	public const int InvalidModelID = 0;
	public List<T> _models = new List<T>();
	public bool isModelsLoaded = false;
	public ModelCatalog()
	{
		ReloadModels();
	}

	public T GetModel(string name)
	{
		T m = _models.Find( model => !string.IsNullOrEmpty(name) && name.Equals(model.GetName()) );
		if (m)
		{
			return m;
		}
		return null;
	}
	
	public T GetModel(int id)
	{
		T m = _models.Find(model => model.GetId() == id);
		if (m)
		{
			return m;
		}
		return null;
	}

	public T GetModelTemplate()
	{
		T m = _models.Find(model => "NPC_Template".Equals(model.GetName()));
		if (m)
		{
			return m;
		}
		EB.Debug.LogError("GetModelTemplate Fail");
		return null;
	}

    public T GetPlayerVariantTemplate()
    {
        T m = _models.Find(model => "PlayerTemplate-Variant".Equals(model.GetName()));
        if (m)
        {
            return m;
        }
        EB.Debug.LogError("GetPlayerVariantTemplate Fail");
        return null;
    }

	public List<T> GetAllModels()
	{
		return _models;
	}

	public static string GetDefaultAssetPath()
	{
		return "Bundles/DataModels/";
	}

	public virtual string GetAssetPath()
	{
		return GetDefaultAssetPath();
	}

	public virtual void ReloadModels()
	{
		_models = new List<T>();
#if UNITY_EDITOR
		//±à¼­Æ÷ÏÂ×ßasset¼ÓÔØ
		if (!UnityEngine.Application.isPlaying)
		{
			string[] paths = System.IO.Directory.GetFiles("Assets/" + GetAssetPath(), "*.asset", System.IO.SearchOption.TopDirectoryOnly);
			foreach (var item in paths)
			{
				UnityEngine.Object asset = UnityEditor.AssetDatabase.LoadAssetAtPath(item, typeof(T));
				T model = asset as T;
				if (model != null)
				{
					_models.Add(model);
				}
			}			
        }
        else{
			EB.Assets.LoadAllAsync(GetAssetPath(), typeof(T), models =>
			{
                foreach (UnityEngine.Object model in models)
                {
                    _models.Add((T)model);
                }
				isModelsLoaded = true;
			});
		}
#else
		EB.Assets.LoadAllAsync(GetAssetPath(), typeof(T), models =>
		{
			foreach (UnityEngine.Object model in models)
			{
				_models.Add((T)model);
			}
			isModelsLoaded = true;
		});
#endif
	}

	public OrderedHashtable GetAllServerData()
	{
		OrderedHashtable serverData = new OrderedHashtable();

		foreach (BaseDataModel model in _models)
		{
			serverData[model.GetId()] = model.GetServerData();
		}

		return serverData;
	}

	public string GetJSONData()
	{
		return EB.JSON.Stringify(GetAllServerData());
	}
}
