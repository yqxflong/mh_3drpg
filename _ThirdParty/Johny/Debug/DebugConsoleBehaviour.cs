using UnityEngine;


namespace Johny
{
    public class DebugConsoleBehaviour : MonoBehaviour
    {
#if UNITY_EDITOR 
        public bool _ShowHashtablePoolStatus = false;

        public bool _ShowArrayListPoolStatus = false;

        public bool _ShowJSONNodePoolStatue = false;

        void Update()
        {
            if(_ShowHashtablePoolStatus){
                HashtablePool.DebugConsole_ClaimInfo();
                HashtablePool.DebugConsole_NewDetailInfo();
            }

            if(_ShowArrayListPoolStatus){
                ArrayListPool.DebugConsole_ClaimInfo();
            }

            if(_ShowJSONNodePoolStatue){
                JSONNodePool.DebugConsole_ClaimInfo();
            }
        }
#endif
    }
}
