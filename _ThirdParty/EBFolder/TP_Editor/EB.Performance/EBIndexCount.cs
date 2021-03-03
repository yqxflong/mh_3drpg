using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class EBIndexCount : ScriptableWizard
{
	[MenuItem("EBG/Performance/Count/Indices")]
	static void Menu() 
	{
		int totalVerts = 0;
		int totalTris = 0;
		int totalIndices = 0;
		foreach( var go in Selection.gameObjects )
		{
			var result = TotalPrefabMesh(go);	
			totalVerts += result[0];	
			totalTris += result[1];	
			totalIndices += result[2];
		}
		Debug.LogError("Vertices: " + totalVerts);
		Debug.LogError("Triangles: " + totalTris);
		Debug.LogError("Indices: " + totalIndices);
	}
	
	static int[] TotalPrefabMesh(GameObject prefab)
	{
		if (prefab == null)
		{
			return new int[] { 0, 0 };
		}
		
		var renderers = EB.Util.FindAllComponents<MeshRenderer>(prefab);
		
		int totalVerts = 0;
		int totalTris = 0;
		int totalIndices = 0;
		
		float i = 0.0f;
		
		foreach( var renderer in renderers )
		{
			EditorUtility.DisplayProgressBar("Totalling mesh", (int)i + " / " + renderers.Length + " - " + renderer.name, i / renderers.Length);
			
			var filter = renderer.GetComponent<MeshFilter>();
			
			if (filter == null || filter.sharedMesh == null)
			{
				continue;
			}
			
			var mesh = filter.sharedMesh;
			
			totalVerts += mesh.vertexCount;
			totalTris += mesh.triangles.Length / 3;
			
			for( var x = 0; x < mesh.subMeshCount; ++x)
			{
				totalIndices += mesh.GetIndices(x).Length;
			}
			
			i += 1.0f;
		}
		
		EditorUtility.ClearProgressBar();
		
		return new int[] { totalVerts, totalTris, totalIndices };
	}
}
