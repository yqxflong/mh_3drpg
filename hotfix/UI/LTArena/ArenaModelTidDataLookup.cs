using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class ArenaModelTidDataLookup : DataLookupHotfix
    {
        public string ResetTime;
        public string Tid;
        public int Skin;

        public override void OnLookupUpdate(string dataID, object value)
        {
            base.OnLookupUpdate(dataID, value);
            ChangeOtherPlayerModelFunc();
        }

        private void ChangeOtherPlayerModelFunc()
        {
            string resetTime = mDL.GetDefaultLookupData<string>();
            if (ResetTime == resetTime) return;
            ResetTime = resetTime;
            string tid;
            DataLookupsCache.Instance.SearchDataByID<string>(ArenaManager.ArenaModelDataId + ".templateId", out tid);
            tid = (tid == null || tid.CompareTo("") == 0) ? "15011" : tid;
            int skin;
            DataLookupsCache.Instance.SearchIntByID(ArenaManager.ArenaModelDataId + ".skin", out skin);
            mDL.transform.GetMonoILRComponent<Player.EnemyHotfixController>().ChangeArenaModel(tid, skin, Skin != skin || Tid != tid);
            Tid = tid;
            Skin = skin;
        }
    }
}