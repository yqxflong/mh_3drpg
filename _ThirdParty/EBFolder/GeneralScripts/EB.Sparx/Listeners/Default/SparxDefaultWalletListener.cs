using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	public class DefaultWalletListener : WalletListener
	{
		#region WalletListener implementation
		public virtual void OnBalanceUpdated (int balance)
		{
			EB.Util.BroadcastMessage("OnBalanceUpdated", balance);
		}

		public virtual void OnCreditSuceeded (int id)
		{
			EB.Util.BroadcastMessage("OnCreditSuceeded", id);
		}

		public virtual void OnDebitFailed (int id)
		{
			EB.Util.BroadcastMessage("OnDebitFailed", id);
		}

		public virtual void OnDebitSuceeded (int id)
		{
			EB.Util.BroadcastMessage("OnDebitSuceeded", id);
		}

		public virtual void OnOffersFetched ()
		{
			EB.Util.BroadcastMessage("OnOffersFetched");
		}

		public virtual void OnOffersFetchSuceeded()
		{
			EB.Util.BroadcastMessage("OnOffersFetchOver");
		}

		public virtual void OnOfferPurchaseFailed (string error)
		{
			EB.Util.BroadcastMessage("OnOfferPurchaseFailed", error);
		}

		public virtual void OnOfferPurchaseSuceeded (EB.IAP.Item item, EB.IAP.Transaction trans)
		{
			EB.Util.BroadcastMessage("OnOfferPurchaseSuceeded", item);
		}

		public virtual void OnOfferPurchaseCanceled ()
		{
			EB.Util.BroadcastMessage("OnOfferPurchaseCanceled");
		}

		public virtual void OnOfferPurchaseRedeemer(object result)
		{
			EB.Util.BroadcastMessage("OnOfferPurchaseRedeemer", result);
		}

		public virtual void OnOfferPurchaseNoResult(System.Action callback)
		{
			EB.Util.BroadcastMessage ("OnOfferPurchaseNoResult", callback);
		}

	#endregion
}	
}

