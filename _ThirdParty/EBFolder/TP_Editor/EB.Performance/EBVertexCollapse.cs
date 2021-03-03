using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class EBVertexCollapse : ScriptableWizard
{
	[MenuItem("EBG/Performance/Count/Collapsable Vertices")]
	static void Menu() 
	{
		int collapsedVerts = 0;
		foreach( var go in Selection.gameObjects )
		{
			collapsedVerts += TotalPrefabMesh(go);	
		}
		Debug.LogError("Collapsable vertices: " + collapsedVerts);
	}
	
	static int TotalPrefabMesh(GameObject prefab)
	{
		if (prefab == null)
		{
			return 0;
		}
		
		var renderers = EB.Util.FindAllComponents<MeshRenderer>(prefab);
		
		int collapsedVerts = 0;
		
		for(int i = 0; i < renderers.Length; ++i)
		{
			var renderer = renderers[i];
			
			EditorUtility.DisplayProgressBar("Processing mesh", (int)i + " / " + renderers.Length + " - " + renderer.name, (float)i / (float)renderers.Length);
			
			var filter = renderer.GetComponent<MeshFilter>();
			
			if (filter == null || filter.sharedMesh == null)
				continue;
			
			var mesh = filter.sharedMesh;
			var optimizedMesh = EBMeshUtils.Optimize(mesh);
			
			collapsedVerts += mesh.vertexCount - optimizedMesh.vertexCount;
		}
		
		EditorUtility.ClearProgressBar();
		
		return collapsedVerts;
	}
}
