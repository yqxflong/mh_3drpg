///////////////////////////////////////////////////////////////////////
//
//  UIIndexedNameSortedTable.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class UIIndexedNameSortedTable : UITable 
{
	protected override void Sort(System.Collections.Generic.List<Transform> list)
	{
		list.Sort((Transform t1, Transform t2) =>
		{
			int index1 = int.Parse(t1.name.Split('_')[0]);
			int index2 = int.Parse(t2.name.Split('_')[0]);

			return index2 - index1;
		});
	}
}
