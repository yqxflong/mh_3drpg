using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class OtherPlayerTidDataLookup : DataLookupHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDL.transform;
            Skin = 0;

        }
        
        public string ModelTid;
        public int Skin;
    	public eDartState DartState;
    	public override void OnLookupUpdate(string dataID, object value)
        {
            base.OnLookupUpdate(dataID, value);
            
    		string[] dataIdSps= dataID.Split('.');
    		if (dataIdSps[dataIdSps.Length - 1] == "tid")
    		{
    			string tid = value as string;
                int skin = 0;
                DataLookupsCache .Instance .SearchDataByID <int>(dataID.Replace(".tid",".skin"),out skin);
    			if ((tid != ModelTid || skin!=Skin) && !AllianceUtil .IsInTransferDart)
    			{
    				if (!string.IsNullOrEmpty(tid))
                        mDL.transform.GetMonoILRComponent<Player.PlayerHotfixController>().ChangeLeaderModel(tid, skin);
    				else
    					EB.Debug.LogError("OtherPlayerTidDataLookup Error: tid NUll ModelTid={0}", ModelTid);
    			}
    			ModelTid = tid;
                Skin = skin;
    		}
    		else if (dataIdSps[dataIdSps.Length - 1] == "TOR")
    		{
    			eDartState dartState = (eDartState)System.Convert.ToInt32(value);
    
    			float scale_size = 1f;
    			if (dartState!= DartState)
    			{
    				if (AllianceEscortUtil.GetIsInTransferDart(dartState))
    				{
    					string dartName = dartState.ToString();
    					string modelName = AllianceEscortUtil.GetTransportCartModel(AllianceEscortUtil.ToDartStateStr(dartState));
    					scale_size = (modelName.IndexOf("M1003") >= 0 || modelName.IndexOf("M1004") >= 0) ? 0.6f : 1;
                        mDL.transform.GetMonoILRComponent<Player.PlayerHotfixController>().ChangeModel(modelName, false, scale_size);
    				}
    				else
    				{
                        int skin = 0;
                        DataLookupsCache.Instance.SearchIntByID(dataID.Replace(".tid", ".skin"), out skin);
                        mDL.transform.GetMonoILRComponent<Player.PlayerHotfixController>().ChangeLeaderModel(ModelTid, skin);
    				}
    			}
    			DartState = dartState;
    		}
    	
    	}
    }
}
