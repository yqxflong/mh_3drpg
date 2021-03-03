using UnityEngine;
using System.Collections;

public class UISymbolAnimation : UISpriteAnimation
{
	public bool Snap
	{
		get { return mSnap; }
		set { mSnap = value; }
	}
}
