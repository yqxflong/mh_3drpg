using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterPartitionLink : MonoBehaviour {
	[HideInInspector][SerializeField]
	List<PartitionInfo> m_LinkedPartitions = new List<PartitionInfo>();

	public List<PartitionInfo> LinkedPartitions
	{
		get
		{
			return m_LinkedPartitions;
		}
		set
		{
			m_LinkedPartitions = value;
		}
	}

}
