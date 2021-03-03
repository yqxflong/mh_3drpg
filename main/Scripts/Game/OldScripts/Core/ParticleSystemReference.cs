using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ParticleSystemReference
{
	public string fileName;

	[System.NonSerialized]
	private string cachedShortName = null;
	public string Name
	{
		get { return cachedShortName = cachedShortName ?? (string.IsNullOrEmpty(fileName) ? string.Empty : Path.GetFileNameWithoutExtension(fileName)); }
	}

	public bool Valiad
	{
		get { return !string.IsNullOrEmpty(fileName); }
	}

#if UNITY_EDITOR
	private ParticleSystem _reference;
#endif

	public ParticleSystem Value
	{
		get
		{
#if UNITY_EDITOR
			if (_reference == null && !Application.isPlaying)
			{
				if (!string.IsNullOrEmpty(fileName))
				{
					_reference = AssetDatabase.LoadAssetAtPath<ParticleSystem>(fileName);
					if (_reference == null)
					{
						EB.Debug.LogError("ParticleSystemReference: Invalid assets for path {0}. Was the asset moved?", fileName);
					}
				}
			}
			return _reference;
#else
			return null;
#endif
		}
		set
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				_reference = value;
				cachedShortName = null;
				fileName = AssetDatabase.GetAssetPath(value);
				if (string.IsNullOrEmpty(fileName) && value != null)
				{
					UnityEngine.Debug.LogError("ParticleSystemReference: Tried to reference file not exists", value);
				}
			}
#endif
		}
	}

#if UNITY_EDITOR
	public ParticleSystemReference()
	{

	}

	public ParticleSystemReference(ParticleSystem reference)
	{
		Value = reference;
	}
#endif
}
