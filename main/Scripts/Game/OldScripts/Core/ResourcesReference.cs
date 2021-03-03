///////////////////////////////////////////////////////////////////////
//
//  ResourcesReference.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ResourcesReference
{
	public string fileName;

#if UNITY_EDITOR
	public string prefix;
	private UnityEngine.Object _reference;
#endif

	public UnityEngine.Object Value
	{
		get 
		{
#if UNITY_EDITOR
			// Only cache the reference in the editor.
			if (_reference != null)
			{
				return _reference;
			}
#endif
			UnityEngine.Object loadedReference = null;
			if (!string.IsNullOrEmpty(fileName))
			{
				string strippedFilePath = fileName.Replace(Path.GetExtension(fileName), string.Empty);
#if UNITY_EDITOR
				if (string.IsNullOrEmpty(prefix))
				{
#endif
                    loadedReference = EB.Assets.Load(strippedFilePath);
#if UNITY_EDITOR
                }
#endif

#if UNITY_EDITOR
				if (loadedReference == null)
				{
					loadedReference = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(prefix + fileName);
				}

				if (!Application.isPlaying)
				{
					// Assign the loaded reference to our local cache in the editor.
					// If the reference is invalid, the asset was probably moved
					_reference = loadedReference;
					if (_reference == null)
					{
						EB.Debug.LogError("ResourcesReference: Invalid assets for path {0}. Was the asset moved?", fileName);
					}
				}
#endif
			}

			return loadedReference;
		}
#if UNITY_EDITOR
		set
		{
			if (!Application.isPlaying)
			{
				_reference = value;
				string path = AssetDatabase.GetAssetPath(value);
				if (!string.IsNullOrEmpty(path) && path != prefix + fileName)
				{
					if (path.StartsWith("Assets/Resources/"))
					{
						string[] split = path.Split(new string[] { "Resources/" }, System.StringSplitOptions.None);
						if (split.Length < 2)
						{
							EB.Debug.LogError("ResourcesReference: Tried to reference file not in a resources directory: {0}" ,path);
						}
						else
						{
							fileName = split[split.Length - 1];
							prefix = string.Empty;
						}
					}
					else
					{
						string[] split = path.Split(new string[] { "Assets/" }, System.StringSplitOptions.None);
						if (split.Length < 2)
						{
							EB.Debug.LogError("ResourcesReference: Tried to reference file not in a assets directory: {0}", path);
						}
						else
						{
							prefix = "Assets/";
							fileName = split[split.Length - 1];
						}
					}
				}
			}
		}
#endif
	}

	public void UnloadAsset()
	{
		if (!string.IsNullOrEmpty(fileName))
		{
			string strippedFilePath = fileName.Replace(Path.GetExtension(fileName), string.Empty);
			EB.Assets.Unload(strippedFilePath);
		}
	}

#if UNITY_EDITOR
	public ResourcesReference()
	{

	}

	public ResourcesReference(UnityEngine.Object reference)
	{
		Value = reference;
	}
#endif
}
