using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	public class DefaultInventoryListener : InventoryListener
	{
		#region InventoryListener implementation
		public void OnInventorySynced (int requestId)
		{
			EB.Util.BroadcastMessage("OnInventorySynced");
		}

		public void OnInventoryItemsAdded (int requestId, Hashtable items)
		{
			EB.Util.BroadcastMessage("OnInventoryItemsAdded", items);
		}

		public void OnInventoryPurchaseFailed (int requestId)
		{
			EB.Util.BroadcastMessage("OnInventoryPurchaseFailed");
		}
		#endregion
		
	}
	
}