using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	public interface WalletListener
	{
		void OnBalanceUpdated( int balance );	
		void OnCreditSuceeded( int id);
		void OnDebitFailed( int id );
		void OnDebitSuceeded( int id );
		void OnOffersFetched();
		void OnOffersFetchSuceeded();
		void OnOfferPurchaseFailed(string error);
		void OnOfferPurchaseSuceeded(EB.IAP.Item item, EB.IAP.Transaction trans);
		void OnOfferPurchaseCanceled();
		void OnOfferPurchaseRedeemer(object result);
		void OnOfferPurchaseNoResult(System.Action callback);
	}	
}
