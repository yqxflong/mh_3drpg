///////////////////////////////////////////////////////////////////////
//
//  ICharacterRecordEntry.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections;

public interface ICharacterRecordEntry
{
	void Initialize(); //Set up default values
	void PrepareData(); //Do any extra recording before we start saving out values
	void SaveUniqueData(Hashtable record); //Save any non-primitive data into the record
	void LoadUniqueData(Hashtable record); //Load any non-primitive data from the record
}