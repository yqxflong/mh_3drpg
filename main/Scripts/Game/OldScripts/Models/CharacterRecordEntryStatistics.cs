///////////////////////////////////////////////////////////////////////
//
//  CharacterRecordEntryStatistics.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections;

public class CharacterRecordEntryStatistics : ICharacterRecordEntry
{
	public int killCount;

	public void Initialize()
	{
		killCount = 0;
	}

	public void PrepareData()
	{
		
	}

	public void SaveUniqueData(Hashtable record)
	{

	}

	public void LoadUniqueData(Hashtable record)
	{

	}
}