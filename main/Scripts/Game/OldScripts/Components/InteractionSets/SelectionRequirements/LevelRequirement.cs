using UnityEngine;
using System.Collections;

[SelectionRequirementType(SelectionRequirement.eSelectionRequirementType.Level), System.Serializable]
public class LevelRequirement : SelectionRequirement
{
	public int levelRequired;

	public override bool IsMet(CharacterRecord record)
	{
		return false;
	}
}
