using EB.Sparx;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartnerGMTool
{
    public string PartnerStatsID = "";
    public string EquipID = "17";

    private static PartnerGMTool ins;
    public static PartnerGMTool Instance
    {
        get
        {
            if (ins == null)
            {
                ins = new PartnerGMTool();
            }
            return ins;
        }
    }

    public void OnGUI()
    {
        GUILayout.Label("点完之后会出现转圈，打log，等log刷完就好了，此间最好不要乱点，不然，概不负责");

        GUILayout.BeginHorizontal();

        GUILayout.Label("伙伴StatsId：");
        PartnerStatsID = GUILayout.TextField(PartnerStatsID, GUILayout.Height(50), GUILayout.Width(100));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("装备套装Id（1~21）：");
        EquipID = GUILayout.TextField(EquipID, GUILayout.Height(50), GUILayout.Width(100));

        GUILayout.EndHorizontal();

        if (GUILayout.Button("一键毕业", GUILayout.Height(50), GUILayout.Width(100)))
        {
            MaxPower();
        }
        if (GUILayout.Button("满级满阶", GUILayout.Height(50), GUILayout.Width(100)))
        {
            PartnerLevelUp();
        }
        if (GUILayout.Button("满星", GUILayout.Height(50), GUILayout.Width(100)))
        {
            PartnerStarUp();
        }
        if (GUILayout.Button("满技能", GUILayout.Height(50), GUILayout.Width(100)))
        {
            PartnerSkillUp();
        }
        if (GUILayout.Button("满装备", GUILayout.Height(50), GUILayout.Width(100)))
        {
            PartnerEquipAllAndUp();
        }
        if (GUILayout.Button("刷新伙伴", GUILayout.Height(50), GUILayout.Width(100)))
        {
            InitPartnerData();
        }

    }

    public void MaxPower()
    {
        GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTPartnerDataManager", "GMTool", "MaxPower", PartnerStatsID, EquipID);
    }

    public void PartnerLevelUp()
    {
        GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTPartnerDataManager", "GMTool", "PartnerLevelUp", PartnerStatsID, EquipID);
    }

    public void PartnerStarUp()
    {
        GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTPartnerDataManager", "GMTool", "PartnerStarUp", PartnerStatsID, EquipID);
    }
    public void PartnerSkillUp()
    {
        GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTPartnerDataManager", "GMTool", "PartnerSkillUp", PartnerStatsID, EquipID);
    }
    public void PartnerEquipAllAndUp()
    {
        GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTPartnerDataManager", "GMTool", "PartnerEquipAllAndUp", PartnerStatsID, EquipID);
    }
    public void InitPartnerData()
    {
        GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTPartnerDataManager", "GMTool", "InitPartnerData", PartnerStatsID, EquipID);
    }
}
