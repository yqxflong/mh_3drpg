using UnityEngine;
using System.Collections;

public class WalletListenerFusion : EB.Sparx.DefaultWalletListener
{
    public override void OnOfferPurchaseSuceeded(EB.IAP.Item item, EB.IAP.Transaction trans)
	{
		base.OnOfferPurchaseSuceeded(item, trans);
        // Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnOfferPurchaseSuceeded, item, trans);
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "Wallet_OnOfferPurchaseSuceeded", item, trans);
    }

    public override void OnOffersFetchSuceeded()
    {
        base.OnOffersFetchSuceeded();
        // Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnOffersFetchSuceeded);
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "Wallet_OnOffersFetchSuceeded");
    }

    public override void OnBalanceUpdated(int balance)
    {
        base.OnBalanceUpdated(balance);
        // Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnBalanceUpdated, balance);
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "Wallet_OnBalanceUpdated", balance);
    }

    public override void OnCreditSuceeded(int id)
    {
        base.OnCreditSuceeded(id);
        // Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnCreditSuceeded, id);
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "Wallet_OnCreditSuceeded", id);
    }

    public override void OnDebitFailed(int id)
    {
        base.OnDebitFailed(id);
        // Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnDebitFailed, id);
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "Wallet_OnDebitFailed", id);
    }

    public override void OnDebitSuceeded(int id)
    {
        base.OnDebitSuceeded(id);
        // Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnDebitSuceeded, id);
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "Wallet_OnDebitSuceeded", id);
    }

    public override void OnOffersFetched()
    {
        base.OnOffersFetched();
        // Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnOffersFetched);
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "Wallet_OnOffersFetched");
    }
    
    public override void OnOfferPurchaseFailed(string error)
    {
        base.OnOfferPurchaseFailed(error);
        // Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnOfferPurchaseFailed, error);
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "Wallet_OnOfferPurchaseFailed", error);
    }
    
    public override void OnOfferPurchaseCanceled()
    {
        base.OnOfferPurchaseCanceled();
        // Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnOfferPurchaseCanceled);
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "Wallet_OnOfferPurchaseCanceled");
    }

    public override void OnOfferPurchaseRedeemer(object result)
    {
        base.OnOfferPurchaseRedeemer(result);
        // Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnOfferPurchaseRedeemer, result);
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "Wallet_OnOfferPurchaseRedeemer", result);
    }

    public override void OnOfferPurchaseNoResult(System.Action callback)
    {
        base.OnOfferPurchaseNoResult(callback);
        // Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnOfferPurchaseNoResult, callback);
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "Wallet_OnOfferPurchaseNoResult", callback);
    }
}
