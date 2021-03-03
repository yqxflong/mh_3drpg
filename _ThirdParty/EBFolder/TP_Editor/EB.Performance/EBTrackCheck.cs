using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class EBTrackCheck : ScriptableWizard
{
	[MenuItem("EBG/Performance/Check Track")]
	static void Menu() 
	{
		int x = 0;
		var gos = Selection.gameObjects;
		try
		{
			foreach( var go in gos )
			{
				CheckPrefab(go, x++, gos.Length);	
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}

	static string GetName(Transform t)
	{
		return (t.parent != null) ? GetName(t.parent) + "/" + t.gameObject.name : t.gameObject.name;
	}
	
	static void CheckPrefab(GameObject prefab, int index, int total)
	{
		var renderers = EB.Util.FindAllComponents<MeshRenderer>(prefab);
		List<Material> materials = new List<Material>();

		int i = 0;
		foreach( var renderer in renderers )
		{
			foreach(Material material in renderer.sharedMaterials)
			{
				if (material != null && material.shader != null)
				{
					if (!materials.Contains(material))
					{
						materials.Add(material);

						if (material.shader.name != null && !(material.shader.name.StartsWith("EBG/") || material.shader.name.StartsWith("Hidden/")))
						{
							Debug.LogError(GetName(renderer.transform) + " isn't using an EBG Shader: " + material.shader.name);
						}
					}
				}
			}
			++i;
		}
		
		List<string> errors = new List<string>();

		for(i = 0; i < materials.Count; ++i)
		{
			Material m1 = materials[i];

			Shader s1 = m1.shader;

			for(int h = i + 1; h < materials.Count; ++h)
			{
				Material m2 = materials[h];

				if (m1 == m2)
					continue;
				
				Shader s2 = m2.shader;

				if (s1.name != s2.name)
					continue;
				
				int propertyCount1 = ShaderUtil.GetPropertyCount(s1);
				int propertyCount2 = ShaderUtil.GetPropertyCount(s2);

				if (propertyCount1 != propertyCount2)
					continue;
				
				bool match = true;

				for (int j = 0; j < propertyCount1; ++j)
				{
					if (ShaderUtil.IsShaderPropertyHidden(s1, j))
						continue;

					string name = ShaderUtil.GetPropertyName(s1, j);

					if (name == "_lm")
						continue;

					ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(s1, j);

					bool propertyIsInMaterial2 = false;
					for (int k = 0; k < propertyCount2; ++k)
					{
						if ((ShaderUtil.GetPropertyName(s2, k) == name) && (ShaderUtil.GetPropertyType(s2, k)) == type && !ShaderUtil.IsShaderPropertyHidden(s2, k))
						{
							propertyIsInMaterial2 = true;
							break;
						}
					}

					if (!propertyIsInMaterial2)
					{
						match = false;
						break;
					}

					try
					{
						switch(type)
						{
							case ShaderUtil.ShaderPropertyType.Color:
							{
								if (m1.GetColor(name) != m2.GetColor(name))
								{
									match = false;
								}
							}
							break;
							case ShaderUtil.ShaderPropertyType.Float:
							{
								if (m1.GetFloat(name) != m2.GetFloat(name))
								{
									match = false;
								}
							}
							break;
							case ShaderUtil.ShaderPropertyType.TexEnv:
							{
								if (m1.GetTexture(name) != m2.GetTexture(name))
								{
									match = false;
								}
							}
							break;
							case ShaderUtil.ShaderPropertyType.Vector:
							{
								if (m1.GetVector(name) != m2.GetVector(name))
								{
									match = false;
								}
							}
							break;
						}
					}
					catch(System.Exception e)
					{
						Debug.LogWarning(e.Message);
						match = false;
					}
				}

				if (match)
				{
					var m1arr = m1.name.Split('_');
					var m2arr = m2.name.Split('_');
					int partition1, partition2, layer1, layer2;
					if (m1arr.Length >= 3 && m2arr.Length >=3 &&
						System.Int32.TryParse(m1arr[m1arr.Length - 1], out partition1) && System.Int32.TryParse(m2arr[m2arr.Length - 1], out partition2) &&
						System.Int32.TryParse(m1arr[m1arr.Length - 2], out layer1) && System.Int32.TryParse(m2arr[m2arr.Length - 2], out layer2))
					{
						if (partition1 == partition2)
						{
							if (layer1 != layer2)
							{
								string name = m1arr[0];
								for(int x = 1; x < m1arr.Length - 2; ++x)
								{
									name += "_" + m1arr[x];
								}
								string error = "Material " + name + " appears on more than one layer in partition " + partition1;
								if (!errors.Contains(error))
								{
									Debug.LogError(error);
									errors.Add(error);
								}
							}
							else
							{
								string error = "Materials " + m1.name + " and " + m2.name + " are the same.";
								if (!errors.Contains(error))
								{
									Debug.LogError(error);
									errors.Add(error);
								}
							}
						}
						else
						{
							if (layer1 != layer2)
							{
								string name = m1arr[0];
								for(int x = 1; x < m1arr.Length - 2; ++x)
								{
									name += "_" + m1arr[x];
								}
								if (partition2 < partition1)
								{
									int temp = partition2;
									partition2 = partition1;
									partition1 = temp;
								}
								string error = "Material " + name + " appears in a different layer in partition " + partition1 + " than partition " + partition2;
								if (!errors.Contains(error))
								{
									Debug.LogWarning(error);
									errors.Add(error);
								}
							}
							else
							{
								//not an issue, material is just split across partitions
							}
						} 
					}
					else
					{
						string error = "Materials " + m1.name + " and " + m2.name + " are the same.";
						if (!errors.Contains(error))
						{
							Debug.LogError(error);
							errors.Add(error);
						}
					}
				}
			}

			++i;
		}
	}
}
