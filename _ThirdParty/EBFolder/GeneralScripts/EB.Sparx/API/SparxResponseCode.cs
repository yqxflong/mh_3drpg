///////////////////////////////////////////////////////////////////////
//
//  SparxAPI.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

namespace EB.Sparx
{		
	public enum eResponseCode
	{
		Unknown                                = -1,
		Success                                = 0,
		InsufficientStamina                    = 1,
		InsufficientHardCurrency               = 2,
		PlayerNotFound                         = 3,
		InvalidGoldAmount                      = 4,
		InsufficientSoftCurrency               = 5,
		InvalidArgument                        = 6,
		CharacterNotFound                      = 7,
		FailedUpdateCharacter                  = 8,
		FailedLoadData                         = 9,
		FailedSendPartyStartNotification       = 10,
		FailedComputingPowerUp                 = 11,
		DbError                                = 12,
		PortraitOperationLocked                = 13,
		PortraitNotReady                       = 14,
		InsufficientSocialCurrency             = 15,
		TooManyPlayersForDungeon               = 16,
		InvalidRelicMachine                    = 17,
		InvalidDungeon                         = 18,
		InvalidLayout                          = 19,
		CharacterWasNotInDungeon               = 20,
		GoldCheatDetected                      = 21,
		LootCheatDetected                      = 22,
		ItemDoesNotExist                       = 23,
		SpiritsCheatDetected                   = 24,
		SpiritsNotFound                        = 25,
		DebugEndpointNotAvailable              = 26,
		InvalidNewCharacter                    = 27,
		XPCheatDetected                        = 28,
		CharacterMustBeInOwnDungeon            = 29,
		DungeonPartyInvalid                    = 30,
		NameNotSet                             = 31,
		FriendNotFound                         = 32,
		FriendExist                            = 33,
		FriendRequestCantInviteYourself        = 34,
		FriendRequestAlreadyMutual             = 35,
		FriendRequestAlreadyRequested          = 36,
		FriendRequestNotFound                  = 37,
		FriendNotMutual                        = 38,
		UserMessageNotFound                    = 39,
		MessageNotFound                        = 40,
		MessagePrizesNotFound                  = 41,
		MessageParamsNotFound                  = 42,
		PrizeNotFound                          = 43,
		InvalidPrize                           = 44,
		MessageListingFailed                   = 45,
		MailNotFound                           = 46,
		MailPrizesNotFound                     = 47,
		MailParamsNotFound                     = 48,
		InvalidTransactionId                   = 49,
		FailedGetAdminMailId                   = 50,
		PortraitStorageError                   = 51,
		InvalidLootListName                    = 52,
		ErrorParsingLootList                   = 53,
		TooManyCharacters                      = 54,
		InvalidServerDataVersion               = 55,
		DuplicatedServerDataVersion            = 56,
		ServerDataVersionActive                = 57,
		ServerDataVersionInactive              = 58,
		ServerDataSingleActiveForClientVersion = 59,
		ServerDataNoCurrentVersion             = 60,
		ServerDataNotReady                     = 61,
		ServerDataInvalidVersionId             = 62,
		ServerDataVersionNotReady              = 63,
		UserNameExists                         = 64,
	}
}