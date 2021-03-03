using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class EBRoadBake : MonoBehaviour 
{
	[MenuItem("EBG/Performance/Bake Selected Roads")]
	public static void BakeRoads()
	{
		char slash = System.IO.Path.DirectorySeparatorChar;
		var dir = "Assets" + slash + "Merge" + slash + "Roads";
		Directory.CreateDirectory(dir);
		
		var selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable | SelectionMode.ExcludePrefab);
		
		for(int i = 0; i < selection.Length; ++i)
		{
			GameObject selected = (GameObject)selection[i];
			
			EditorUtility.DisplayProgressBar("Baking Roads", selected.name, ((float)i)/selection.Length);
			
			//save out all the segments for merging
			Transform segmentsTransform = selected.transform.Find("Segments");
			if (segmentsTransform)
			{	
				GameObject segments = segmentsTransform.gameObject;
				
	         	Renderer[] renderers = EB.Util.FindAllComponents<Renderer>(segments);
				for(int j = 0; j < renderers.Length; ++j)
				{
					Renderer renderer = renderers[j];
					
					var meshFilter = renderer.GetComponent<MeshFilter>();
					if (!meshFilter)
						continue;
					
					var meshRenderer = renderer.GetComponent<MeshRenderer>();
					if (!meshRenderer)
						continue;
				
					EditorUtility.DisplayProgressBar("Baking Roads", selected.name, ((float)i + (((float)j)/selection.Length))/selection.Length);

					renderer.gameObject.layer = LayerMask.NameToLayer("environment");
					
					var mesh = meshFilter.sharedMesh;

					if (mesh == null) continue;

					string subDir = dir + slash + selected.transform.root.name;
					Directory.CreateDirectory(subDir);

					string path = subDir + slash + i + "_" + meshFilter.name + "_" + j + ".asset";
					
					/*
						AssetDatabase.CreateAsset(mesh, dir + "/" + selected.name + "_" + i + "_" + meshFilter.name + "_" + j + ".asset");
					}*/
					
					Mesh savedMesh = AssetDatabase.LoadMainAssetAtPath (path) as Mesh;
					if (savedMesh != null) 
					{
						Debug.Log("Replacing existing mesh: " + path);
						EditorUtility.CopySerialized (mesh, savedMesh);
						AssetDatabase.SaveAssets ();
					}
					else 
					{
						Debug.Log("Creating new mesh: " + path);
						savedMesh = new Mesh();
						EditorUtility.CopySerialized (mesh, savedMesh);
						AssetDatabase.CreateAsset(savedMesh, path);
					}
					
					meshFilter.sharedMesh = (Mesh)AssetDatabase.LoadAssetAtPath(path,typeof(Mesh));
					
					MeshCollider mc = meshFilter.GetComponent<MeshCollider>();
					
					if (mc != null) mc.sharedMesh = meshFilter.sharedMesh;
				}
			}
			
			//mark all buildings as static
			Transform buildingsTransform = selected.transform.Find("Buildings");
			if (buildingsTransform)
			{	
				GameObject buildings = buildingsTransform.gameObject;
				
	         	Renderer[] renderers = EB.Util.FindAllComponents<Renderer>(buildings);
				foreach(var renderer in renderers)
				{
					var meshRenderer = renderer.GetComponent<MeshRenderer>();
					if (!meshRenderer)
						continue;

					renderer.gameObject.layer = LayerMask.NameToLayer("environment");
				}
			}
			
			//mark all the nodes as hidden to avoid merging, disable all the road node scripts
			Transform nodesTransform = selected.transform.Find("Nodes");
			if (nodesTransform)
			{
				GameObject nodes = nodesTransform.gameObject;
				
	         	Renderer[] renderers = EB.Util.FindAllComponents<Renderer>(nodes);
				foreach(var renderer in renderers)
				{
					var meshRenderer = renderer.GetComponent<MeshRenderer>();
					if (!meshRenderer)
						continue;
				
					if (meshRenderer.sharedMaterial.name == "matNode" || meshRenderer.sharedMaterial.name == "matVertex")
					{
						renderer.enabled = false;
						renderer.gameObject.layer = LayerMask.NameToLayer("Default");
						continue;
					}
				}
				
	         	/*RoadNode[] roadnodes = EB.Util.FindAllComponents<RoadNode>(nodes);
				foreach(var roadnode in roadnodes)
				{
					roadnode.enabled = false;
				}*/
			}
		}
		EditorUtility.ClearProgressBar();
	}
}
