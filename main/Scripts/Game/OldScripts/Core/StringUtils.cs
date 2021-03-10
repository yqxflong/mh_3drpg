///////////////////////////////////////////////////////////////////////
//
//  StringUtils.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Globalization;
using UnityEngine;

public class StringUtils
{
	public const string TOKEN_CHAR_GENDER_LOWER = "#CHAR_GENDER_LOWER#";
	public const string TOKEN_CHAR_GENDER_UPPER = "#CHAR_GENDER_UPPER#";
	public const string TOKEN_CHAR_NAME = "#CHAR_NAME#";
	public const string TOKEN_CHAR_NAME_ALLCAPS = "#CHAR_NAME_ALLCAPS#";
	public const string TOKEN_CHAR_CLASS_LOWER = "#CHAR_CLASS_LOWER#";
	public const string TOKEN_CHAR_CLASS_UPPER = "#CHAR_CLASS_UPPER#";
	public const string TOKEN_CHAR_CLASS_HISHER_LOWER = "#CHAR_CLASS_HISHER_LOWER#";
	public const string TOKEN_CHAR_CLASS_HISHER_UPPER = "#CHAR_CLASS_HISHER_UPPER#";
	public const string TOKEN_CHAR_CLASS_HESHE_LOWER = "#CHAR_CLASS_HESHE_LOWER#";
	public const string TOKEN_CHAR_CLASS_HESHE_UPPER = "#CHAR_CLASS_HESHE_UPPER#";

	public static string ReplaceTokens(string inStr)
	{
		CharacterRecord character = CharacterManager.Instance.CurrentCharacter;
		if (character == null)
		{
			EB.Debug.LogWarning("StringUtils::ReplaceTokens character was null");
			return inStr;
		}

		SecureCharacterRecordEntryGeneral record = character.GeneralRecord;
		if (record == null)
		{
			EB.Debug.LogWarning("StringUtils::ReplaceTokens could not obtain CharacterRecordGeneral from character");
			return inStr;
		}

		System.Text.StringBuilder sb = new System.Text.StringBuilder(inStr);

		try
		{
			sb.Replace(TOKEN_CHAR_GENDER_LOWER, character.GeneralRecord.gender.ToString().ToLower());
			sb.Replace(TOKEN_CHAR_GENDER_UPPER, Capitalize(character.GeneralRecord.gender.ToString()));
			sb.Replace(TOKEN_CHAR_NAME, Capitalize(character.GeneralRecord.name));
			sb.Replace(TOKEN_CHAR_NAME_ALLCAPS, character.GeneralRecord.name.ToUpper());
			//sb.Replace(TOKEN_CHAR_CLASS_LOWER, record.characterClass.ToString().ToLower());
			//sb.Replace(TOKEN_CHAR_CLASS_UPPER, Capitalize(record.characterClass.ToString()));
			sb.Replace(TOKEN_CHAR_CLASS_HISHER_LOWER, (record.gender == eGender.Male ? GetLocalized("his") : GetLocalized("her")).ToLower());
			sb.Replace(TOKEN_CHAR_CLASS_HISHER_UPPER, Capitalize(record.gender == eGender.Male ? GetLocalized("his") : GetLocalized("her")));
			sb.Replace(TOKEN_CHAR_CLASS_HESHE_LOWER, (record.gender == eGender.Male ? GetLocalized("he") : GetLocalized("she")).ToLower());
			sb.Replace(TOKEN_CHAR_CLASS_HESHE_UPPER, Capitalize(record.gender == eGender.Male ? GetLocalized("he") : GetLocalized("she")));
		} catch (System.Exception e)
		{
			EB.Debug.LogWarning(e.Message);
			return inStr;
		}

		// testing prms
		
		return sb.ToString();
	}

	public static string Capitalize(string inStr)
	{
		if (string.IsNullOrEmpty(inStr))
		{
			EB.Debug.LogWarning("StringUtils::Capitalize received null string");
			return inStr;
		}

		if (inStr.Length > 1)
		{
			return char.ToUpper(inStr[0]) + inStr.Substring(1);
		}

		return inStr.ToUpper();
	}

	// todo: run literal string through a localization system
	public static string GetLocalized(string str)
	{
		return str;
	}

	public static Vector3 Vector3Parse(string content, char separator = ',')
	{
		content = content.Replace("(", ""); content = content.Replace(")", ""); content.Trim();
		string[] values = content.Split(separator);
		return new Vector3(float.Parse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture), float.Parse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture), float.Parse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture));
	}
}
