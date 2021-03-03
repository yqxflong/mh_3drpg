namespace Hotfix_LT.UI
{
    using UnityEngine;
    using System.Collections.Generic;

    public class ArenaLogScroll : DynamicGridScroll<ArenaBattleLog, ArenaLogCell>
    {
        public void SetDataItems(List<ArenaBattleLog> logs)
        {
            List<ArenaBattleLog> ordered = new List<ArenaBattleLog>(logs);
            ordered.Sort(new ArenaBattleLogComparer());
            SetItemDatas(ordered.ToArray());
        }
    }

}