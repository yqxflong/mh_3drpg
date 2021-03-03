using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BakeMeshHelper : MonoBehaviour
{
	public GameObject m_GeometryRoot = null;

	public Vector2 m_ZoneStart = Vector2.zero;
	public Vector2 m_ZoneEnd = Vector2.one * GameVars.GridSize;
	public int m_SplitUnitsX = GameVars.GridSize;
	public int m_SplitUnitsY = GameVars.GridSize;

	public bool m_DrawSplitGrids = false;

#if UNITY_EDITOR
	private Dictionary<int, LightmapParameters> m_LightmapParams = new Dictionary<int, LightmapParameters>();
#endif

	void OnDrawGizmos()
	{
		if (!m_DrawSplitGrids)
		{
			return;
		}

		int gridsX = Mathf.CeilToInt(Mathf.Abs(m_ZoneEnd.x - m_ZoneStart.x) / m_SplitUnitsX);
		int gridsZ = Mathf.CeilToInt(Mathf.Abs(m_ZoneEnd.y - m_ZoneStart.y) / m_SplitUnitsY);

		Gizmos.color = Color.green;
		for (int i = 0; i <= gridsX; i++)
		{
			float posX = m_ZoneStart.x + i * m_SplitUnitsX;
			if (posX > m_ZoneEnd.x)
			{
				posX = m_ZoneEnd.x;
			}
			Gizmos.DrawLine(new Vector3(posX, 0, m_ZoneStart.y), new Vector3(posX, 0, m_ZoneEnd.y));
		}

		for (int i = 0; i <= gridsZ; i++)
		{
			float posZ = m_ZoneStart.y + i* m_SplitUnitsY;
			if (posZ > m_ZoneEnd.y)
			{
				posZ = m_ZoneEnd.y;
			}
			Gizmos.DrawLine(new Vector3(m_ZoneStart.x, 0, posZ), new Vector3(m_ZoneEnd.x, 0, posZ));
		}
	}

#if UNITY_EDITOR
	public void GroupGeometryMeshes()
	{
		if (m_LightmapParams.Count > 0)
		{
			Debug.LogError("GroupGeometryMeshs: Grouped, ResetGroup first!");
			return;
		}

		if (m_GeometryRoot == null)
		{
			Debug.LogError("GroupGeometryMeshs: Geometry root is not assigned yet!");
			return;
		}

		int gridsX = Mathf.CeilToInt(Mathf.Abs(m_ZoneEnd.x - m_ZoneStart.x) / m_SplitUnitsX);
		int gridsZ = Mathf.CeilToInt(Mathf.Abs(m_ZoneEnd.y - m_ZoneStart.y) / m_SplitUnitsY);
		for (int i = 0; i < gridsX; i++)
		{
			for (int j = 0; j < gridsZ; j++)
			{
				int tag = i * gridsZ + j;
				LightmapParameters param = new LightmapParameters();
				param.name = m_GeometryRoot.name + tag;
				// ignore Precomputed Realtime GI

				// set Baked GI
				param.bakedLightmapTag = tag;

				// set Baked AO
				m_LightmapParams.Add(tag, param);
			}
		}

		MeshRenderer[] meshes = CollectMeshes();
		foreach (MeshRenderer mesh in meshes)
		{
			int offx = Mathf.FloorToInt((mesh.bounds.center.x - m_ZoneStart.x) / m_SplitUnitsX);
			int offz = Mathf.FloorToInt((mesh.bounds.center.z - m_ZoneStart.y) / m_SplitUnitsY);
			if (offx >= 0 && offx < gridsX && offz >= 0 && offz < gridsZ && ((GameObjectUtility.GetStaticEditorFlags(mesh.gameObject) & StaticEditorFlags.LightmapStatic) != 0))
			{
				SerializedObject serializedObject = new SerializedObject(mesh);
				SerializedProperty property = serializedObject.FindProperty("m_LightmapParameters");
				property.objectReferenceValue = m_LightmapParams[offx * gridsZ + offz];
				serializedObject.ApplyModifiedProperties();
			}
		}
	}

	public void ResetGroup()
	{
		int gridsX = Mathf.CeilToInt(Mathf.Abs(m_ZoneEnd.x - m_ZoneStart.x) / m_SplitUnitsX);
		int gridsZ = Mathf.CeilToInt(Mathf.Abs(m_ZoneEnd.y - m_ZoneStart.y) / m_SplitUnitsY);

		MeshRenderer[] meshes = CollectMeshes();
		foreach (MeshRenderer mesh in meshes)
		{
			int offx = Mathf.FloorToInt((mesh.bounds.center.x - m_ZoneStart.x) / m_SplitUnitsX);
			int offz = Mathf.FloorToInt((mesh.bounds.center.z - m_ZoneStart.y) / m_SplitUnitsY);
			if (offx >= 0 && offx < gridsX && offz >= 0 && offz < gridsZ)
			{
				SerializedObject serializedObject = new SerializedObject(mesh);
				SerializedProperty property = serializedObject.FindProperty("m_LightmapParameters");
				property.objectReferenceValue = null;
				serializedObject.ApplyModifiedProperties();
			}
		}

		foreach (var entry in m_LightmapParams)
		{
			DestroyImmediate(entry.Value);
		}
		m_LightmapParams.Clear();
	}
#endif

	public bool ValidateGroup()
	{
		if (m_GeometryRoot == null)
		{
			Debug.LogError("ValidateGroup: Geometry root is not assigned yet!");
			return false;
		}

		int gridsX = Mathf.CeilToInt(Mathf.Abs(m_ZoneEnd.x - m_ZoneStart.x) / m_SplitUnitsX);
		int gridsZ = Mathf.CeilToInt(Mathf.Abs(m_ZoneEnd.y - m_ZoneStart.y) / m_SplitUnitsY);

		Dictionary<int, List<int>> groupLightmapIndexes = new Dictionary<int, List<int>>();
		Dictionary<int, List<MeshRenderer>> groupMeshs = new Dictionary<int, List<MeshRenderer>>();
		for (int i = 0; i < gridsX; i++)
		{
			for (int j = 0; j < gridsZ; j++)
			{
				int tag = i * gridsZ + j;
				groupLightmapIndexes.Add(tag, new List<int>());
				groupMeshs.Add(tag, new List<MeshRenderer>());
			}
		}
		
		MeshRenderer[] meshes = CollectMeshes();
		foreach (MeshRenderer mesh in meshes)
		{
			int offx = Mathf.FloorToInt((mesh.bounds.center.x - m_ZoneStart.x) / m_SplitUnitsX);
			int offz = Mathf.FloorToInt((mesh.bounds.center.z - m_ZoneStart.y) / m_SplitUnitsY);
			if (offx >= 0 && offx < gridsX && offz >= 0 && offz < gridsZ)
			{
				int tag = offx * gridsZ + offz;
				int lightmapIndex = mesh.lightmapIndex;
				if (lightmapIndex >= 0 && lightmapIndex < 255)
				{
					groupMeshs[tag].Add(mesh);
					if (!groupLightmapIndexes[tag].Contains(lightmapIndex))
					{
						groupLightmapIndexes[tag].Add(lightmapIndex);
					}
				}
			}
		}

		bool result = true;
		foreach (var entry in groupLightmapIndexes)
		{
			if (entry.Value.Count > 1)
			{
				foreach (var mesh in groupMeshs[entry.Key])
				{
					Debug.LogWarningFormat(mesh, "ValidateGroup: tag = {2}, {0}.lightmapIndex = {1}", mesh.name, mesh.lightmapIndex, entry.Key);
				}

				result = false;
			}
		}

		groupLightmapIndexes.Clear();
		groupMeshs.Clear();

		if (!result)
		{
			Debug.LogErrorFormat(m_GeometryRoot, "ValidateGroup: {0} bake check failed, please adjust Scale In Lightmap of each MeshRenderer to keep all lightmap in one atlas!", m_GeometryRoot.name);
		}
		else
		{
			Debug.LogFormat(m_GeometryRoot, "ValidateGroup: {0} bake check ok.", m_GeometryRoot.name);
		}
		return result;
	}

	private MeshRenderer[] CollectMeshes()
	{
		if (m_GeometryRoot == null)
		{
			Debug.LogError("CollectMeshs: Geometry root is not assigned yet!");
			return new MeshRenderer[0];
		}

		return m_GeometryRoot.GetComponentsInChildren<MeshRenderer>();
	}
}
