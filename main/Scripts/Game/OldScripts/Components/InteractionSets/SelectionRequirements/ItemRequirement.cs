using UnityEngine;
using System.Collections;

[SelectionRequirementType(SelectionRequirement.eSelectionRequirementType.Item), System.Serializable]
public class ItemRequirement : SelectionRequirement
{
	//public ItemModel item;

	public override bool IsMet(CharacterRecord record)
	{
		return false;
	}
}
