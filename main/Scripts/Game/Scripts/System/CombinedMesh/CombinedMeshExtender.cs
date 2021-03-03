using UnityEngine;

public class CombinedMeshExtender : MonoBehaviour {
	[System.Serializable]
	public struct RowEntry
	{
		public string combineRowName;
		public GameObject mesh;
	}
	public RowEntry[] rowEntries;
}
