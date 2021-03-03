using UnityEngine;
using System.Collections;

public class ResourcesReferenceAttribute : PropertyAttribute
{
	public readonly System.Type editorType;

	public ResourcesReferenceAttribute(System.Type type)
	{
		editorType = type;
	}
}
