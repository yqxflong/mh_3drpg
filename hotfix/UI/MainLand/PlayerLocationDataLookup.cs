using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class PlayerLocationDataLookup
    {
        private long userid;
        public const int PosUpdateInterval = 4;
        public const int DestUpdateInterval = 4;

        public long UserId
        {
            set
            {
                userid = value;
            }
        }

        public void OnLookupUpdate(string dataID, object value, bool hasStarted)
        {
            if (!hasStarted) return;
            PlayerController pc = PlayerManager.GetPlayerController(userid);
            if (pc == null) return;
            if (pc.IsLocal) return;
            if (value == null)
            {
                //PlayerManager.UnregisterPlayerController(pc);
                //PlayerManagerForFilter.Instance.PlayerOut(pc.playerUid);
                //ObjectManager.instance.DestroyManagedObject(pc.GetComponentInChildren<CharacterVariant>().gameObject);
                //GameObject.Destroy(pc.gameObject);
                return;
            }

            if (value is IDictionary)
            {
                var userdata = value as IDictionary;
                if (userdata["dest"] != null)
                {
                    int pos_ts = EB.Dot.Integer("pos_ts", userdata, 0);
                    int dest_ts = EB.Dot.Integer("dest_ts", userdata, 0);
                    int now = EB.Time.Now;
                    if (now - pos_ts > PosUpdateInterval && now - dest_ts > DestUpdateInterval) return;
                    Vector3 v3 = GM.LitJsonExtension.ImportVector3(userdata["dest"].ToString());
                    if (!pc.gameObject.transform.position.Equals(v3))
                    {
                        pc.TargetingComponent.SetMovementTargetNoRPC(v3);
                    }
                }
            }
        }
    }
}