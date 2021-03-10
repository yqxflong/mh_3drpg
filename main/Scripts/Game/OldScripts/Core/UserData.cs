///////////////////////////////////////////////////////////////////////
//
//  UserData.cs
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
using System.Collections.Generic;
using System.Globalization;

public static class UserData
{
    public const string defaultPlayerName = "GuestPlayer";

#if USE_OVERSEAS
    public const string defaultApiServerAddress = "https://apios.manhuang.org";
#else 
    public const string defaultApiServerAddress = "https://api.manhuang.org";

#endif
    public const string defaultAuthServerAddress = "https://auth.ubei.io:20181";

#if !UNITY_EDITOR && UNITY_ANDROID
	public static readonly string localAssetAddress = Application.streamingAssetsPath + "/" + GM.AssetUtils.STREAMING_ASSETS_BUNDLE_FOLDER_NAME;
#else
    public static readonly string localAssetAddress = "file://" + Application.streamingAssetsPath + "/" + GM.AssetUtils.STREAMING_ASSETS_BUNDLE_FOLDER_NAME;
#endif

    public const int defaultCharacterId = 0;
    public const float defaultVolume = 1.0f;
    public const float defaultVolume_Low = 0.6f;
    public const float defaultVolume_Low2 = 0.4f;
    public const bool defaultAudioMuteSet = false;
    public const string defaultCustomDataVersion = "";
    public const EB.Language defaultLanguage = EB.Language.English;

    public static readonly char[] kDefaultNameChars =
        {
            '0','1','2','3','4','5','6','7','8','9',
            'A','B','C','D','E','F','G','H','J','K','L','M','N','P','Q','R','S','T','U','V','W','X','Y','Z',
            'a','b','d','e','f','g','h','i','j','m','n','q','r','t','u','y'
        };

    private static bool _audioEnabled = true;
    public static bool AudioEnabled
    {
        get { return _audioEnabled; }
        set {
            _audioEnabled = value;
            AudioListener.volume = _audioEnabled ? 1.0f : 0;
        }
    }

    private static bool _vigorRecoverFullNotification = true;
    public static bool VigorRecoverFullNotification
    {
        get { return _vigorRecoverFullNotification; }
        set { _vigorRecoverFullNotification = value; }
    }
    
    private static bool _skillCameraRotateEnabled = true;
    public static bool SkillCameraRotateEnabled
    {
        get { return _skillCameraRotateEnabled; }
        set { _skillCameraRotateEnabled = value; }
    }

    private static bool _storeRefreshNotification = true;
    public static bool StoreRefreshNotification
    {
        get { return _storeRefreshNotification; }
        set { _storeRefreshNotification = value; }
    }

    private static bool _receiveVigorNotification = true;
    public static bool ReceiveVigorNotification
    {
        get { return _receiveVigorNotification; }
        set { _receiveVigorNotification = value; }
    }

    private static bool _activityOpenNotification = true;
    public static bool ActivityOpenNotification
    {
        get { return _activityOpenNotification; }
        set { _activityOpenNotification = value; }
    }

    private static bool _allianceBattleOpenNotification = true;
    public static bool AllianceBattleOpenNotification
    {
        get { return _allianceBattleOpenNotification; }
        set { _allianceBattleOpenNotification = value; }
    }

    private static string _initedVersion = string.Empty;
    public static string InitedVersion
    {
        get { return _initedVersion; }
        set { _initedVersion = value; }
    }

    private static string _playerName = defaultPlayerName;
    public static string PlayerName
    {
        get { return _playerName; }
        set { _playerName = value; }
    }

    static private int _playerNum;
    public static int PlayerNum
    {
        get { return _playerNum; }
        set { _playerNum = value; }
    }

    private static int _latestCharacterId = defaultCharacterId;
    public static int LatestCharacterId
    {
        get { return _latestCharacterId; }
        set { _latestCharacterId = value; }
    }

    private static float _musicVolume = defaultVolume;
    public static float MusicVolume
    {
        get { return _musicVolume; }
        set { _musicVolume = value; }
    }

    private static float _sfxVolume = defaultVolume;
    public static float SFXVolume
    {
        get { return _sfxVolume; }
        set { _sfxVolume = value; }
    }

    private static float _ambienceVolume = defaultVolume;
    public static float AmbienceVolume
    {
        get { return _ambienceVolume; }
        set { _ambienceVolume = value; }
    }

    private static bool _isMusicMute = defaultAudioMuteSet;
    public static bool IsMusicMute
    {
        get { return _isMusicMute; }
        set { _isMusicMute = value; }
    }

    private static bool _isSfxMute = defaultAudioMuteSet;
    public static bool IsSfxMute
    {
        get { return _isSfxMute; }
        set { _isSfxMute = value; }
    }

    private static bool _isAmbienceMute = defaultAudioMuteSet;
    public static bool IsAmbienceMute
    {
        get { return _isAmbienceMute; }
        set { _isAmbienceMute = value; }
    }

    private static string _customDataVersion = defaultCustomDataVersion;
    public static string CustomDataVersion
    {
        get { return _customDataVersion; }
        set { _customDataVersion = value; }
    }

    private static bool _tutorialCompleteForStatTracking = false; 
    public static bool TutorialCompleteForStatTracking
    {
        get { return _tutorialCompleteForStatTracking; }
        set { _tutorialCompleteForStatTracking = value; }
    }

    private static bool _motionEnabled = true;
    public static bool MotionEnabled
    {
        get { return _motionEnabled; }
        set { _motionEnabled = value; }
    }

    private static bool _preferServerAccount;
    public static bool PreferServerAccount
    {
        get { return _preferServerAccount; }
        set { _preferServerAccount = value; }
    }

    private static bool _preferLocalAccount;
    public static bool PreferLocalAccount
    {
        get { return _preferLocalAccount; }
        set { _preferLocalAccount = value; }
    }

    private static EB.Language _locale;
    public static EB.Language Locale
    {
        get { return _locale; }
        set { _locale = value; }
    }

    private static string _userQualitySet = null;//High,Medium,Low
    public static string UserQualitySet
    {
        get { return _userQualitySet; }
        set { _userQualitySet = value; }
    }
    private static string _qualitySetVersion = null;
    public static string QualitySetVersion
    {
        get { return _qualitySetVersion; }
        set { _qualitySetVersion = value; }
    }

    public static void SetUserQuality(int index = 0)
    {
        switch (index)
        {
            case 0: { _userQualitySet = "High"; };break;
            case 1: { _userQualitySet = "Medium"; }; break;
            case 2: { _userQualitySet = "Low"; }; break;
            default: { _userQualitySet = "High"; };break;
        }
    }


    public static Dictionary<string, Vector3> DefaultUIPosDic = new Dictionary<string, Vector3>();
    public static Dictionary<string, Vector3> PersonalUIPosDic = new Dictionary<string, Vector3>();

    private static void SerializePosDic(Dictionary<string, Vector3> posDic, string name)
    {
        Hashtable posHash = ConvertDicToHashtable(posDic);
        PlayerPrefs.SetString(name, EB.JSON.Stringify(posHash));
    }

    private static Hashtable ConvertDicToHashtable(Dictionary<string, Vector3> posDic)
    {
        Hashtable posHash = Johny.HashtablePool.Claim();
        foreach (var pair in posDic)
        {
            posHash.Add(pair.Key, pair.Value.ToString());
        }

        return posHash;
    }

    private static void DeserializePosDic(string name, Dictionary<string, Vector3> posDic)
    {
        Hashtable posHash = (Hashtable)EB.JSON.Parse(PlayerPrefs.GetString(name, string.Empty));

        if(posHash != null){
            foreach (DictionaryEntry entry in posHash)
            {
                posDic[entry.Key.ToString()] = GetVector3(entry.Value.ToString());
            }
        }
    }

    public static Vector3 GetVector3(string rString)
    {
        string[] temp = rString.Substring(1, rString.Length - 2).Split(',');
        float x = float.Parse(temp[0], NumberStyles.Any, CultureInfo.InvariantCulture);
        float y = float.Parse(temp[1], NumberStyles.Any, CultureInfo.InvariantCulture);
        float z = float.Parse(temp[2], NumberStyles.Any, CultureInfo.InvariantCulture);
        Vector3 rValue = new Vector3(x, y, z);

        return rValue;
    }

    public static string GenerateDefaultUsername(string prefix)
    {
        uint targetBase = (uint)kDefaultNameChars.Length;
        uint nameHash = (uint)EB.Hash.StringHash(EB.Version.GetUDID());
        string result = string.Empty;

        do
        {
            result = kDefaultNameChars[nameHash % targetBase] + result;
            nameHash = nameHash / targetBase;
        } while (nameHash > 0);

        return string.Format("{0}{1}", prefix, result);
    }

    public static void SerializePrefs()
    {
        PlayerPrefs.SetInt("AudioEnabled", AudioEnabled == true ? 1 : 0);

        PlayerPrefs.SetInt("VigorRecoverFull", VigorRecoverFullNotification == true ? 1 : 0);
        PlayerPrefs.SetInt("SkillCameraRotate", SkillCameraRotateEnabled == true ? 1 : 0);
        PlayerPrefs.SetInt("StoreRefresh_12_18", StoreRefreshNotification == true ? 1 : 0);
        PlayerPrefs.SetInt("ReceiveVigor_12_18", ReceiveVigorNotification == true ? 1 : 0);
        PlayerPrefs.SetInt("ActivityOpen", ActivityOpenNotification == true ? 1 : 0);
        PlayerPrefs.SetInt("AllianceBattleOpen", AllianceBattleOpenNotification == true ? 1 : 0);

        PlayerPrefs.SetInt("SettingPlayerNum", PlayerNum);

        PlayerPrefs.SetString("PlayerName", PlayerName);
        PlayerPrefs.SetInt("LatestCharacterId", LatestCharacterId);
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
        PlayerPrefs.SetFloat("AmbienceVolume", AmbienceVolume);
        PlayerPrefs.SetInt("IsMusicMute", IsMusicMute == true ? 1 : 0);
        PlayerPrefs.SetInt("IsSfxMute", IsSfxMute == true ? 1 : 0);
        PlayerPrefs.SetInt("IsAmbienceMute", IsAmbienceMute == true ? 1 : 0);
        PlayerPrefs.SetString("CustomDataVersion", CustomDataVersion);
        PlayerPrefs.SetInt("TutorialCompleteForStatTracking", TutorialCompleteForStatTracking == true ? 1 : 0);
        PlayerPrefs.SetInt("MotionEnabled", MotionEnabled == true ? 1 : 0);
        PlayerPrefs.SetInt("PreferServerAccount", PreferServerAccount == true ? 1 : 0);
        PlayerPrefs.SetInt("PreferLocalAccount", PreferLocalAccount == true ? 1 : 0);
        PlayerPrefs.SetInt("Locale", (int)Locale);
        PlayerPrefs.SetString("InitedVersion", InitedVersion);
        PlayerPrefs.SetString("UserQualitySetting", UserQualitySet);
        PlayerPrefs.SetString("QualitySetVersion", QualitySetVersion);

        SerializePosDic(DefaultUIPosDic, "DefaultUIPosDic");
        SerializePosDic(PersonalUIPosDic, "PersonalUIPosDic");

        PlayerPrefs.Save();
    }

    public static void DeserializePrefs()
    {
        PlayerName = PlayerPrefs.GetString("PlayerName", GenerateDefaultUsername("Guest"));
        AudioEnabled = PlayerPrefs.GetInt("AudioEnabled", 1) == 1 ? true : false;

        VigorRecoverFullNotification = PlayerPrefs.GetInt("VigorRecoverFull", 1) == 1 ? true : false;
        SkillCameraRotateEnabled = PlayerPrefs.GetInt("SkillCameraRotate", 1) == 1 ? true : false;
        StoreRefreshNotification = PlayerPrefs.GetInt("StoreRefresh_12_18", 1) == 1 ? true : false;
        ReceiveVigorNotification = PlayerPrefs.GetInt("ReceiveVigor_12_18", 1) == 1 ? true : false;
        ActivityOpenNotification = PlayerPrefs.GetInt("ActivityOpen", 1) == 1 ? true : false;
        AllianceBattleOpenNotification = PlayerPrefs.GetInt("AllianceBattleOpen", 1) == 1 ? true : false;

        PlayerNum = PlayerPrefs.GetInt("SettingPlayerNum", -1);

        LatestCharacterId = PlayerPrefs.GetInt("LatestCharacterId", defaultCharacterId);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", defaultVolume_Low2);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", defaultVolume_Low);
        AmbienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", defaultVolume_Low);
        IsMusicMute = PlayerPrefs.GetInt("IsMusicMute", 0) == 1 ? true : false;
        IsSfxMute = PlayerPrefs.GetInt("IsSfxMute", 0) == 1 ? true : false;
        IsAmbienceMute = PlayerPrefs.GetInt("IsAmbienceMute", 0) == 1 ? true : false;
        CustomDataVersion = PlayerPrefs.GetString("CustomDataVersion", defaultCustomDataVersion);
        TutorialCompleteForStatTracking = PlayerPrefs.GetInt("TutorialCompleteForStatTracking", 0) == 1 ? true : false;
        MotionEnabled = PlayerPrefs.GetInt("MotionEnabled", 1) == 1 ? true : false;
        PreferServerAccount = PlayerPrefs.GetInt("PreferServerAccount", 0) == 1 ? true : false;
        PreferLocalAccount = PlayerPrefs.GetInt("PreferLocalAccount", 0) == 1 ? true : false;
        InitedVersion = PlayerPrefs.GetString("InitedVersion", string.Empty);

        UserQualitySet = PlayerPrefs.GetString("UserQualitySetting", string.Empty);
        EB.Sparx.PerformanceManager.PerformanceUserSetting = UserQualitySet;

        QualitySetVersion = PlayerPrefs.GetString("QualitySetVersion", string.Empty);

        int locale = PlayerPrefs.GetInt("Locale", -1);
        if (locale < 0 && Locale == EB.Language.Unknown)
        {
#if USE_LANGUAGE_SYSTEM
            Locale = EB.Localizer.GetSystemDefaultLanguage(defaultLanguage);
#elif USE_LANGUAGE_TW
            Locale = EB.Language.ChineseTraditional;
#elif USE_LANGUAGE_EN
            Locale = EB.Language.English;
#else
            Locale = EB.Language.ChineseSimplified;
#endif
        }
        else if (Locale == EB.Language.Unknown)
        {
            Locale = (EB.Language)locale;
        }

        DeserializePosDic("DefaultUIPosDic", DefaultUIPosDic);
        DeserializePosDic("PersonalUIPosDic", PersonalUIPosDic);
    }
}
