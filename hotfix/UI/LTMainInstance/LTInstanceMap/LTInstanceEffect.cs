using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTInstanceEffect : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();
            Effects = new ParticleSystem[4];
            Effects[0]= ((GameObject)mDMono.ObjectParamList[0]).GetComponent <ParticleSystem>();
            Effects[1] = ((GameObject)mDMono.ObjectParamList[1]).GetComponent<ParticleSystem>();
            Effects[2] = ((GameObject)mDMono.ObjectParamList[2]).GetComponent<ParticleSystem>();
            //新增副本门特效
			if(mDMono.ObjectParamList.Count > 12)
				Effects[3] = ((GameObject)mDMono.ObjectParamList[12]).GetComponent<ParticleSystem>();

            RoleObjs = new GameObject[9];
            RoleObjs[0] = (GameObject)mDMono.ObjectParamList[3];
            RoleObjs[1] = (GameObject)mDMono.ObjectParamList[4];
            RoleObjs[2] = (GameObject)mDMono.ObjectParamList[5];
            RoleObjs[3] = (GameObject)mDMono.ObjectParamList[6];
            RoleObjs[4] = (GameObject)mDMono.ObjectParamList[7];
            RoleObjs[5] = (GameObject)mDMono.ObjectParamList[8];
            RoleObjs[6] = (GameObject)mDMono.ObjectParamList[9];
            RoleObjs[7] = (GameObject)mDMono.ObjectParamList[10];
            RoleObjs[8] = (GameObject)mDMono.ObjectParamList[11];

            instance = this;
        }
        
        private static LTInstanceEffect instance;
        public static LTInstanceEffect Instance
        {
            get
            {
                if (instance == null)
                {
                    EB.Debug.LogError("LTInstanceEffect has not Instance");
                }
                return instance;
            }
        }
    
        public ParticleSystem[] Effects;
        public GameObject[] RoleObjs;
    
        public override void OnDestroy()
        {
            instance = null;
        }
        
        public ParticleSystem GetParticleByEffectName(string effectName)
        {
            for (int i = 0; i < Effects.Length; i++)
            {
                if (Effects[i].name.CompareTo(effectName) == 0)
                {
                    return Effects[i];
                }
            }
            return null;
        }
    
        public GameObject GetRoleItemByImgName(string roleType)
        {
            for (int i = 0; i < RoleObjs.Length; i++)
            {
                if (RoleObjs[i].name.CompareTo(roleType) == 0)
                {
                    return RoleObjs[i];
                }
            }
            return RoleObjs[0];
        }
    }
}
