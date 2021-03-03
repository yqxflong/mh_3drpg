using UnityEngine;
using System.Collections.Generic;

public class SelectionRequirementTypeAttribute : System.Attribute
{
	public SelectionRequirement.eSelectionRequirementType Type
	{
		get;
		private set;
	}

	public SelectionRequirementTypeAttribute(SelectionRequirement.eSelectionRequirementType type)
	{
		Type = type;
	}
}

[System.Serializable]
public class SelectionRequirement : ScriptableObject
{
	public enum eSelectionRequirementType
	{
		None,
		Item,
		Ability,
		Level,
		Class,
	}

	private static Dictionary<eSelectionRequirementType, System.Type> _selectionTypes = null;

	public virtual bool IsMet(CharacterRecord record)
	{
		return true;
	}

	public static SelectionRequirement CreateRequirement(eSelectionRequirementType requirementType)
	{
		if (_selectionTypes == null)
		{
			FillTypesDictionary();
		}

		if (_selectionTypes.ContainsKey(requirementType))
		{
			return (SelectionRequirement)ScriptableObject.CreateInstance(_selectionTypes[requirementType]);
			//return (SelectionRequirement)_selectionTypes[requirementType].GetConstructor(Type.EmptyTypes).Invoke(null);
		}
		else
		{
			return null;
		}
	}

	public static eSelectionRequirementType GetTypeFromRequirement(SelectionRequirement req)
	{
		if (_selectionTypes == null)
		{
			FillTypesDictionary();
		}

		foreach (KeyValuePair<eSelectionRequirementType, System.Type> pair in _selectionTypes)
		{
			if (pair.Value == req.GetType())
			{
				return pair.Key;
			}
		}

		return eSelectionRequirementType.None;
	}

	private static void FillTypesDictionary()
	{
		_selectionTypes = new Dictionary<eSelectionRequirementType, System.Type>();
			foreach (System.Type type in typeof(SelectionRequirement).Assembly.GetTypes())
			{
				if (type.IsSubclassOf(typeof(SelectionRequirement)))
				{
					SelectionRequirementTypeAttribute attr = (SelectionRequirementTypeAttribute)type.GetCustomAttributes(typeof(SelectionRequirementTypeAttribute), false)[0];
					_selectionTypes[attr.Type] = type;
				}
			}
	}
}
