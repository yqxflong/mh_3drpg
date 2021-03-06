///////////////////////////////////////////////////////////////////////
//
//  CharacterCatalogcs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

public class CharacterCatalog : ModelCatalog<CharacterModel>
{
	private static CharacterCatalog sInstance;
	public static CharacterCatalog Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new CharacterCatalog();
			}
			return sInstance;
		}
	}

	public override string GetAssetPath() 
	{
		return base.GetAssetPath() + "Characters";
	}
}