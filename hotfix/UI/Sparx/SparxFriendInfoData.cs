///////////////////////////////////////////////////////////////////////
//
//  SparxFriendInfoData.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using EB.Sparx;
using System.Collections;

namespace Hotfix_LT.UI
{
	public class FriendInfoData
	{
		public Id PlayerId { get; private set; }
		public string Name { get; private set; }
		public float lastUpdateTimestamp;
		public bool isOnline;

		public FriendInfoData(Hashtable data)
		{
			PlayerId = new Id(Hotfix_LT.EBCore.Dot.Find<object>("uid", data));
			Name = EB.Dot.String("name", data, string.Empty);
		}
	}
}
