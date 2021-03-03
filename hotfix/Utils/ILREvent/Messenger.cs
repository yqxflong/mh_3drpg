
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT
{
    public static class Messenger
    {
        #region Internal variables
        static public Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();
        #endregion

        #region AddListener
        static public void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
        {
            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, null);
            }
        }

        static public void OnListenerRemoved(string eventType)
        {
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }

        //No parameters
        static public void AddListener(string eventType, Action handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Action)eventTable[eventType] + handler;
        }

        //Single parameter
        static public void AddListener<T>(string eventType, Action<T> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Action<T>)eventTable[eventType] + handler;
        }

        //Two parameters
        static public void AddListener<T, U>(string eventType, Action<T, U> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Action<T, U>)eventTable[eventType] + handler;
        }

        //Three parameters
        static public void AddListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Action<T, U, V>)eventTable[eventType] + handler;
        }
        static public void AddListenerEx<T>(string eventType, Func<T> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Func<T>)eventTable[eventType] + handler;
        }
        static public void AddListenerEx<T, U>(string eventType, Func<T, U> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Func<T, U>)eventTable[eventType] + handler;
        }
        static public void AddListenerEx<T, U, V>(string eventType, Func<T, U, V> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Func<T, U, V>)eventTable[eventType] + handler;
        }
        #endregion

        #region RemoveListener
        //No parameters
        static public void RemoveListener(string eventType, Action handler)
        {
            if(eventTable.TryGetValue(eventType, out var act))
            {
                eventTable[eventType] = (Action)act - handler;
                OnListenerRemoved(eventType);
            }
        }

        //Single parameter
        static public void RemoveListener<T>(string eventType, Action<T> handler)
        {
            if(eventTable.TryGetValue(eventType, out var act))
            {
                eventTable[eventType] = (Action<T>)act - handler;
                OnListenerRemoved(eventType);
            }
        }

        //Two parameters
        static public void RemoveListener<T, U>(string eventType, Action<T, U> handler)
        {
            if(eventTable.TryGetValue(eventType, out var act))
            {
                eventTable[eventType] = (Action<T, U>)act - handler;
                OnListenerRemoved(eventType);
            }
        }

        //Three parameters
        static public void RemoveListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            if(eventTable.TryGetValue(eventType, out var act))
            {
                eventTable[eventType] = (Action<T, U, V>)act - handler;
                OnListenerRemoved(eventType);
            }
        }
        static public void RemoveListenerEx<T>(string eventType, Func<T> handler)
        {
            if(eventTable.TryGetValue(eventType, out var act))
            {
                eventTable[eventType] = (Func<T>)act - handler;
                OnListenerRemoved(eventType);
            }
        }
        static public void RemoveListenerEx<T, U>(string eventType, Func<T, U> handler)
        {
            if(eventTable.TryGetValue(eventType, out var act))
            {
                eventTable[eventType] = (Func<T, U>)act - handler;
                OnListenerRemoved(eventType);
            }
        }
        static public void RemoveListenerEx<T, U, V>(string eventType, Func<T, U, V> handler)
        {
            if(eventTable.TryGetValue(eventType, out var act))
            {
                eventTable[eventType] = (Func<T, U, V>)act - handler;
                OnListenerRemoved(eventType);
            }
        }
        #endregion

        #region Broadcast
        //No parameters
        static public void Raise(string eventType)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Action callback = d as Action;

                if (callback != null)
                {
                    callback();
                }
                else
                {
                    EB.Debug.LogError(eventType);
                }
            }
        }

        //Single parameter
        static public void Raise<T>(string eventType, T arg1)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Action<T> callback = d as Action<T>;

                if (callback != null)
                {
                    callback(arg1);
                }
                else
                {
                    EB.Debug.LogError(eventType);
                }
            }
        }

        //Two parameters
        static public void Raise<T, U>(string eventType, T arg1, U arg2)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Action<T, U> callback = d as Action<T, U>;

                if (callback != null)
                {
                    callback(arg1, arg2);
                }
                else
                {
                   EB.Debug.LogError(eventType);
                }
            }
        }

        //Three parameters
        static public void Raise<T, U, V>(string eventType, T arg1, U arg2, V arg3)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Action<T, U, V> callback = d as Action<T, U, V>;

                if (callback != null)
                {
                    callback(arg1, arg2, arg3);
                }
                else
                {
                    EB.Debug.LogError(eventType);
                }
            }
        }

        //zero parameters, one return
        static public T RaiseEx<T>(string eventType)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Func<T> callback = d as Func<T>;

                if (callback != null)
                {
                    return callback();
                }
                else
                {
                    EB.Debug.LogError(eventType);
                }
            }

            return default(T);
        }

        //one parameters, one return
        static public U RaiseEx<T, U>(string eventType, T arg1)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Func<T, U> callback = d as Func<T, U>;

                if (callback != null)
                {
                    return callback(arg1);
                }
                else
                {
                    EB.Debug.LogError(eventType);
                }
            }

            return default(U);
        }

        //two parameters, one return
        static public V RaiseEx<T, U, V>(string eventType, T arg1, U arg2)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Func<T, U, V> callback = d as Func<T, U, V>;

                if (callback != null)
                {
                    return callback(arg1, arg2);
                }
                else
                {
                    EB.Debug.LogError(eventType);
                }
            }

            return default(V);
        }
        #endregion
    }

    /// <summary>
    /// 用于过渡主工程传过来的消息
    /// </summary>
    public static class MessengerAdapter{

        #region Multiple Call

        public static void OnInteractEvent(GameObject player,GameObject gameObject)
        {
            Messenger.Raise("OnInteractEvent", player, gameObject);
        }
        
        
        //public static void PlayerMoveSyncManagerMove(Vector3 pos)
        //{
        //    Messenger.Raise(Hotfix_LT.EventName.PlayerMoveSyncManagerMove, pos);
        //}

        public static void GameStateChangeOnStart(eGameState state)
        {
            Messenger.Raise<eGameState>("GameStateChangeOnStart", state);
        }

        public static void GameStateChangeOnEnd(eGameState state){
            Messenger.Raise<eGameState>("GameStateChangeOnEnd", state);
        }

        public static bool IsReady(){
            if(UI.LTCombatEventReceiver.Instance != null){
                return UI.LTCombatEventReceiver.Instance.Ready;
            }
            return false;
        }
        #endregion

        #region Wallet
        public static void Wallet_OnOfferPurchaseSuceeded(EB.IAP.Item item, EB.IAP.Transaction trans)
        {
            Messenger.Raise(Hotfix_LT.EventName.OnOfferPurchaseSuceeded, item, trans);
        }

        public static void Wallet_OnOffersFetchSuceeded()
        {
            Messenger.Raise(Hotfix_LT.EventName.OnOffersFetchSuceeded);
        }

        public static void Wallet_OnBalanceUpdated(int balance)
        {
            Messenger.Raise(Hotfix_LT.EventName.OnBalanceUpdated, balance);
        }

        public static void Wallet_OnCreditSuceeded(int id)
        {
            Messenger.Raise(Hotfix_LT.EventName.OnCreditSuceeded, id);
        }

        public static void Wallet_OnDebitFailed(int id)
        {
            Messenger.Raise(Hotfix_LT.EventName.OnDebitFailed, id);
        }

        public static void Wallet_OnDebitSuceeded(int id)
        {
            Messenger.Raise(Hotfix_LT.EventName.OnDebitSuceeded, id);
        }

        public static void Wallet_OnOffersFetched()
        {
            Messenger.Raise(Hotfix_LT.EventName.OnOffersFetched);
        }

        public static void Wallet_OnOfferPurchaseFailed(string error)
        {
            Messenger.Raise(Hotfix_LT.EventName.OnOfferPurchaseFailed, error);
        }

        public static void Wallet_OnOfferPurchaseCanceled()
        {
            Messenger.Raise(Hotfix_LT.EventName.OnOfferPurchaseCanceled);
        }

        public static void Wallet_OnOfferPurchaseRedeemer(object result)
        {
            Messenger.Raise(Hotfix_LT.EventName.OnOfferPurchaseRedeemer, result);
        }

        public static void Wallet_OnOfferPurchaseNoResult(System.Action callback)
        {
            Messenger.Raise(Hotfix_LT.EventName.OnOfferPurchaseNoResult, callback);
        }
        #endregion

        #region FxLib
        public static void BossParticleInSceneComplete(){
            if(Hotfix_LT.UI.LTCombatEventReceiver.Instance != null){
                Hotfix_LT.UI.LTCombatEventReceiver.Instance.OnBoss_ParticleInSceneComplete();
            }
        }
        #endregion
    
        #region LoadingLogic
        public static void ExitWatchAsk(){
            if(UI.LTCombatHudController.Instance != null){
                UI.LTCombatHudController.Instance.ExitWatchAsk();
            }
        }
        #endregion

        #region UIStack
        public static void OnCancelButtonClick(){
            if(UI.LTCombatHudController.Instance != null){
                UI.LTCombatHudController.Instance.OnCancelButtonClick();
            }
        }
        #endregion

        #region MyFollowCamera
        public static void DisplayCameraViewButton(bool isTrue){
            if(UI.LTCombatHudController.Instance != null){
                UI.LTCombatHudController.Instance.DisplayCameraViewButton(isTrue);
            }
        }
        #endregion

        #region BaseFlowAction
        public static void BaseFlowActionOnEnter(string state)
        {
            if(UI.GameFlowHotfixController.Instance != null){
                UI.GameFlowHotfixController.Instance.OnEnter(state);
            }
        }

        public static void BaseFlowActionOnExit(string state)
        {
            if(UI.GameFlowHotfixController.Instance != null){
                UI.GameFlowHotfixController.Instance.OnExit(state);
            }
        }
        #endregion

        #region CombatLogic
        public static void ExitCombat(){
            if(UI.LTCombatEventReceiver.Instance != null){
                UI.LTCombatEventReceiver.Instance.OnExitCombat();
            }
            if (UI.LTCombatHudController.Instance != null)
            {
                UI.LTCombatHudController.Instance.ExitCombat();
            }
        }

        public static void SetStartEvent(object value)
        {
            if (UI.CombatManager.Instance != null)
            {
                UI.CombatManager.Instance.startObj = value;
            }
        }

        public static void RaiseCombatHitDamageEvent(Combat.CombatHitDamageEvent e)
        {
            Messenger.Raise<Combat.CombatHitDamageEvent>(EventName.CombatHitDamageEvent, e);
        }

        public static void RaiseCombatHealEvent(Combat.CombatHealEvent e)
        {
            Messenger.Raise<Combat.CombatHealEvent>(EventName.CombatHealEvent, e);
        }

        public static void RaiseCombatDamageEvent(Combat.CombatDamageEvent e)
        {
            Messenger.Raise<Combat.CombatDamageEvent>(EventName.CombatDamageEvent, e);
        }
        #endregion

        #region CombatEditor 编辑器调用，忽略性能问题

        public static void CE_StartBattle(Hashtable myHash){
            Messenger.Raise<string, Hashtable>(Hotfix_LT.EventName.DoDebugAction, "startBattle", myHash);
        }

        public static void CE_killThem(Hashtable targetHash){
            Messenger.Raise<string, Hashtable>(Hotfix_LT.EventName.DoDebugAction, "killThem", targetHash);
        }

        public static void CE_InfiHP(Hashtable targetHash){
            Messenger.Raise<string, Hashtable>(Hotfix_LT.EventName.DoDebugAction, "InfiHP", targetHash);
        }

        public static void CE_ClearCD(Hashtable targetHash){
            Messenger.Raise<string, Hashtable>(Hotfix_LT.EventName.DoDebugAction, "ClearCD", targetHash);
        }

        public static void CE_ViewProp(Hashtable targetHash){
            Messenger.Raise<string, Hashtable>(Hotfix_LT.EventName.DoDebugAction, "ViewProp", targetHash);
        }

        #endregion
    
        #region DataLookUpCache
        public static void Data_InventoryChanged(object value){
            if(UI.LTPartnerEquipDataManager.Instance != null){
                UI.LTPartnerEquipDataManager.Instance.OnInventoryChanged(value);
            }
        }
        #endregion
    }
}