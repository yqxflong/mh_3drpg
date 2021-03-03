using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class UIStoreRowController : DynamicRowController<StoreItemData, UIStoreCellController>
    {
		public override void Awake()
		{
			base.Awake();

			if (cellCtrls == null)
			{
				var t = mDMono.transform;
				cellCtrls = new UIStoreCellController[t.childCount];

				for (var i = 0; i < t.childCount; i++)
				{
					cellCtrls[i] = t.GetChild(i).GetMonoILRComponentByClassPath<UIStoreCellController>("Hotfix_LT.UI.UIStoreCellController");
				}
			}
		}
	}
}

