using EB.IAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EB.Sparx
{
    public static class SparxTransactionHelper
    {
        private static string TransactionUrl
        {
            get
            {
                return Hub.Instance.DataStore.LoginDataStore.LocalUserId.Value + "WalletManagerTransactionIDStr";
            }
        }

        /// <summary>获取某个商品id的所有有效本地订单</summary>
        public static void GetTranByPayoutId(int payoutId, out List<Transaction> cacheTransactions)
        {
            cacheTransactions = new List<Transaction>();
            string productId = string.Empty;
            for (int i = 0; i < Hub.Instance.WalletManager.Payouts.Length; i++)
            {
                if (Hub.Instance.WalletManager.Payouts[i].payoutId == payoutId)
                {
                    productId = Hub.Instance.WalletManager.Payouts[i].productId;
                }
            }

            List<Transaction> list = GetAllLoacalTrans();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].productId == productId)
                {
                    cacheTransactions.Add(list[i]);
                }
            }
        }

        /// <summary>获取所有有效本地订单</summary>
        public static List<Transaction> GetAllVaildLoacalTrans()
        {
            List<Transaction> transList = new List<Transaction>();
            string transStrs = PlayerPrefs.GetString(TransactionUrl, "");
            if (string.IsNullOrEmpty(transStrs))
            {
                EB.Debug.Log("【商城】本地没有订单储存");
                return transList;
            }
            EB.Debug.Log("【商城】本地已添加的总储存订单：{0}", transStrs);

            string[] strs = transStrs.Split(';');
            if (strs == null || strs.Length <= 0)
            {
                EB.Debug.Log("本地订单储存异常");
                return transList;
            }

            for (int i = 0; i < strs.Length; i++)
            {
                IAP.Transaction trans = GetTransInLocal(strs[i]);
                if (trans == null)
                {
                    EB.Debug.Log("【商城】本地存储某个订单是空的,序号i = {0}", i);
                    continue;
                }
                transList.Add(trans);
                EB.Debug.Log("【商城】获得本地存储订单transactionId：{0}", strs[i]);
            }
            return transList;
        }

        /// <summary>获取所有本地订单</summary>
        private static List<Transaction> GetAllLoacalTrans()
        {
            List<Transaction> transList = new List<Transaction>();
            string transStrs = PlayerPrefs.GetString(TransactionUrl, "");
            if (string.IsNullOrEmpty(transStrs))
            {
                EB.Debug.Log("【商城】本地没有本地订单储存");
                return transList;
            }
            EB.Debug.Log("【商城】本地已添加的本地总订单：{0}", transStrs);

            string[] strs = transStrs.Split(';');
            if (strs == null || strs.Length <= 0)
            {
                EB.Debug.Log("本地订单储存异常");
                return transList;
            }

            for (int i = 0; i < strs.Length; i++)
            {
                IAP.Transaction trans = GetTransInLocal(strs[i], false);
                if (trans == null)
                {
                    EB.Debug.Log("【商城】本地存储某个本地订单是空的,序号i = {0}", i);
                    continue;
                }
                transList.Add(trans);
                EB.Debug.Log("【商城】获得本地订单transactionId：{0}", strs[i]);
            }
            return transList;
        }

        /// <summary>添加本地订单</summary>
        public static void AddTransInLocal(IAP.Transaction transaction)
        {
            if (transaction == null)
            {
                UnityEngine.Debug.LogError("【商城】添加本地订单异常，订单为null");
                return;
            }

            string transStrs = PlayerPrefs.GetString(TransactionUrl, "");

            if (transStrs.IndexOf(transaction.transactionId) >= 0)
            {
                EB.Debug.Log("【商城】本地订单已有该订单：{0}", transaction.transactionId);
                return;
            }

            transStrs = string.IsNullOrEmpty(transStrs) ? transaction.transactionId : string.Format("{0};{1}", transStrs, transaction.transactionId);

            PlayerPrefs.SetString(TransactionUrl, transStrs);
            PlayerPrefs.SetString(string.Format("{0}_{1}_productId", TransactionUrl, transaction.transactionId), transaction.productId);
            PlayerPrefs.SetString(string.Format("{0}_{1}_payload", TransactionUrl, transaction.transactionId), transaction.serverPayload);//特殊设置，设置的时候设置服务器所需要的补单数据，而不是服务器下发给客户端的数据
            PlayerPrefs.SetString(string.Format("{0}_{1}_signature", TransactionUrl, transaction.transactionId), transaction.signature);
            PlayerPrefs.SetString(string.Format("{0}_{1}_platform", TransactionUrl, transaction.transactionId), transaction.platform);
            PlayerPrefs.SetString(string.Format("{0}_{1}_iapplatform", TransactionUrl, transaction.transactionId), transaction.IAPPlatform);
            EB.Debug.Log("【商城】本地订单添加订单成功：{0}", string.Format("{0}_{1}", TransactionUrl, transaction.transactionId));
        }

        /// <summary>获取本地订单</summary>
        private static IAP.Transaction GetTransInLocal(string transId, bool needVaild = true)
        {
            if (string.IsNullOrEmpty(transId))
            {
                UnityEngine.Debug.LogError("【商城】获取本地订单异常，订单为null");
                return null;
            }

            EB.Debug.Log("【商城】获得本地订单transactionId：{0}", transId);

            IAP.Transaction transaction = new IAP.Transaction();
            transaction.transactionId = transId;
            transaction.productId = PlayerPrefs.GetString(string.Format("{0}_{1}_productId", TransactionUrl, transaction.transactionId), "");
            transaction.payload = PlayerPrefs.GetString(string.Format("{0}_{1}_payload", TransactionUrl, transaction.transactionId), "");
            transaction.signature = PlayerPrefs.GetString(string.Format("{0}_{1}_signature", TransactionUrl, transaction.transactionId), "");
            transaction.platform = PlayerPrefs.GetString(string.Format("{0}_{1}_platform", TransactionUrl, transaction.transactionId), "");
            transaction.IAPPlatform = PlayerPrefs.GetString(string.Format("{0}_{1}_iapplatform", TransactionUrl, transaction.transactionId), "");

            EB.Debug.Log("【商城】productId = {0};payload = {1};signature = {2};platform = {3};iapplatform = {4}", transaction.productId, transaction.payload, transaction.signature, transaction.platform, transaction.IAPPlatform);

            if (needVaild && (transaction.productId == "" || transaction.payload == "" || transaction.platform == ""))
            {
                EB.Debug.LogWarning("【商城】无效的本地订单");
                return null;
            }
            return transaction;
        }

        /// <summary>删除本地订单</summary>
        public static void DelTransInLocal(IAP.Transaction transaction)
        {
            if (transaction == null|| string.IsNullOrEmpty(transaction.transactionId))
            {
                UnityEngine.Debug.LogError("【商城】删除本地订单异常，订单为null");
                return;
            }

            EB.Debug.Log("【商城】删除本地订单transactionId：{0}", transaction.transactionId);
            
            string transStrs = PlayerPrefs.GetString(TransactionUrl, "");
            if (string.IsNullOrEmpty(transStrs))
            {
                EB.Debug.Log("【商城】删除本地订单异常，本地总订单为null");
                return;
            }
            EB.Debug.Log("【商城】删除订单中本地总订单：{0}", transStrs);

            int strIndex = transStrs.IndexOf(transaction.transactionId);
            if (strIndex < 0)
            {
                EB.Debug.Log("【商城】本地总订单不包含需删除的订单");
                return;
            }

            if (transStrs.Length == transaction.transactionId.Length)
            {
                transStrs = "";
            }
            else if (strIndex == 0)
            {
                transStrs = transStrs.Remove(strIndex, transaction.transactionId.Length + 1);
            }
            else if (strIndex > 0)
            {
                transStrs = transStrs.Remove(strIndex - 1, transaction.transactionId.Length + 1);
            }
            EB.Debug.Log("【商城】删除本地订单后的本地总订单：{0}", transStrs);

            PlayerPrefs.SetString(TransactionUrl, transStrs);

            PlayerPrefs.DeleteKey(string.Format("{0}_{1}_productId", TransactionUrl, transaction.transactionId));
            PlayerPrefs.DeleteKey(string.Format("{0}_{1}_payload", TransactionUrl, transaction.transactionId));
            PlayerPrefs.DeleteKey(string.Format("{0}_{1}_signature", TransactionUrl, transaction.transactionId));
            PlayerPrefs.DeleteKey(string.Format("{0}_{1}_platform", TransactionUrl, transaction.transactionId));
            PlayerPrefs.DeleteKey(string.Format("{0}_{1}_iapplatform", TransactionUrl, transaction.transactionId));
        }
    }
}