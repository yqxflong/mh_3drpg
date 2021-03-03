using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 世界boss战斗场景脚本（目前用于更改场景里的一些obj的状态）
/// </summary>
public class WorldBossCombatScene : MonoBehaviour
{
    private GameObject yaManLa1;
    private GameObject yaManLa2;

    public static WorldBossCombatScene Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start ()
    {
        yaManLa1 = transform.Find("BossYamanla1").gameObject;
        yaManLa2 = transform.Find("BossYamanla2").gameObject;

        if (yaManLa1 == null || yaManLa2 == null)
        {
            EB.Debug.LogError("WorldBossCombatScene Start Error!!");
        }
    }

    /// <summary>
    /// 设置场景状态（1:完整状态，2:场景破碎状态）
    /// </summary>
    /// <param name="status"></param>
    public void SetSceneStatus(int status = 2)
    {
        yaManLa1.CustomSetActive(status == 1);
        yaManLa2.CustomSetActive(status == 2);
    }
}
