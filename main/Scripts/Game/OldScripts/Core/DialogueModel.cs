///////////////////////////////////////////////////////////////////////
//
//  DialogueModel.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

[System.Serializable]
public class DialogueModel : ScriptableObject
{	
	public Dialogue.DialogueData dialogueData = new Dialogue.DialogueData();
	private const string InitialName = "DialogueModel";

	static public DialogueModel Create()
	{
		DialogueModel model = (DialogueModel)ScriptableObject.CreateInstance<DialogueModel>();		
		model.name = InitialName;
		return model;
	}
}
