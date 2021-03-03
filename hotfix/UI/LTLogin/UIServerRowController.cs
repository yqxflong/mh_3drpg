using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class UIServerRowController : DynamicRowController<ServerData,UIServerCellController>
    {
        public override void Awake()
        {
            base.Awake();
            if (cellCtrls == null)
            {
                var t = mDMono.transform;
                cellCtrls = new UIServerCellController[t.childCount];

                for (var i = 0; i < t.childCount; i++)
                {
                    cellCtrls[i] = t.GetChild(i).GetMonoILRComponent<UIServerCellController>();
                }
            }
        }
    }
}
