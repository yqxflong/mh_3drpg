using UnityEngine;
using System.Collections;

public interface IInteractable
{
	float InteractionRangeSq
	{
		get;
	}

	InteractableComponent Interactable
	{
		get;
	}

	bool ShouldOutline
	{
		get;
		set;
	}

	bool IsInRange(Transform character);
	void Interact(GameObject player);
}
