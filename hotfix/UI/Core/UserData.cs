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

namespace Hotfix_LT.UI
{
    public static class UserData
    {
        public static List<LTDailyData> PushMessageDataList = new List<LTDailyData>();
        public static Dictionary<int, bool> PushMessageDataDic = new Dictionary<int, bool>();

        public static void SetPushMsgPref(int id)
        {
            bool hasKey = PushMessageDataDic.ContainsKey(id) ? (PushMessageDataDic[id] == true) : false;
            PlayerPrefs.SetInt(id.ToString(), hasKey ? 1 : 0);
        }

        public static void SetPushMsgPrefs()
        {
            PushMessageDataList = LTDailyDataManager.Instance.GetDailyDataByDailyType(EDailyType.Limit);
            for (int i = 0; i < PushMessageDataList.Count; i++)
            {
                bool hasKey = PushMessageDataDic.ContainsKey(PushMessageDataList[i].ActivityData.id) ? (PushMessageDataDic[PushMessageDataList[i].ActivityData.id] == true) : false;
                PlayerPrefs.SetInt(PushMessageDataList[i].ActivityData.id.ToString(), hasKey ? 1 : 0);
            }
        }

        public static void PushMsgPrefs()
        {
            PushMessageDataList = LTDailyDataManager.Instance.GetDailyDataByDailyType(EDailyType.Limit);
            for (int i = 0; i < PushMessageDataList.Count; i++)
            {
                if (PushMessageDataDic.ContainsKey(PushMessageDataList[i].ActivityData.id))
                {
                    PushMessageDataDic[PushMessageDataList[i].ActivityData.id] = PlayerPrefs.GetInt(PushMessageDataList[i].ActivityData.id.ToString(), 0) == 1 ? true : false;
                }
                else
                {
                    PushMessageDataDic.Add(PushMessageDataList[i].ActivityData.id, PlayerPrefs.GetInt(PushMessageDataList[i].ActivityData.id.ToString(), 0) == 1 ? true : false);
                }
            }
        }
    }
}