using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class HonorArenaChallenger : INodeData
    {
        public long uid { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public int rank { get; set; }
        public int level { get; set; }
        public int charId { get; set; }
        public int skin { get; set; }
        public string frame { get; set; }
        public int worldId{ get; set; }
        public string _id{ get; set; }
        
        public int changePoint{ get; set; }
        public int point{ get; set; }
        
        public int br{ get; set; }
        
        
        
        public void OnUpdate(object obj)
        {
            _id = EB.Dot.String("_id", obj, _id);
            uid = EB.Dot.Long("uid", obj, uid);
            icon =  EB.Dot.String("icon", obj, name);
            name = EB.Dot.String("name", obj, name);
            rank = EB.Dot.Integer("rank", obj, rank);
            level = EB.Dot.Integer("level", obj, level);
            charId = EB.Dot.Integer("charId", obj, charId);
            skin = EB.Dot.Integer("skin", obj, 0);
            frame = EB.Dot.String("headFrame", obj, "0_0");
            worldId = EB.Dot.Integer("worldId", obj, LoginManager.Instance.LocalUser.WorldId);
            changePoint = EB.Dot.Integer("changePoint", obj, changePoint);
            point = EB.Dot.Integer("point", obj, point);
            br = EB.Dot.Integer("br", obj, br);
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public void CleanUp()
        {
            uid = 0;
            icon =name = frame = string.Empty;
            rank = 0;
            level = 0;
            skin = 0;
            worldId = 0;
            changePoint = 0;
            point = 0;
            br = 0;
        }

        public object Clone()
        {
            return new HonorArenaChallenger();
        }
    }

    public class HonorArenaInfo : INodeData
    {
        public string point { set; get; }
        public int usedTimes { set; get; }
        public int ticket { set; get; }
        public int quantity { set; get; }
        public long last_one_hour { set; get; }
        public ArrayList InfoData { set; get; }
        public int reward { set; get; }

        public bool IsOpen { set; get; }

        public void OnUpdate(object obj)
        {
            IsOpen = EB.Dot.Bool("is_open", obj, IsOpen);
            point = EB.Dot.String("point", obj, point);
            int preuseTime = usedTimes;
            usedTimes = EB.Dot.Integer("usedTimes", obj, usedTimes);
            ticket = EB.Dot.Integer("ticket", obj, ticket);
            quantity = EB.Dot.Integer("buyCost.quantity", obj, quantity);
            int freetimes = HonorArenaManager.Instance.GetHonorArenaFreeTimes();
            if (preuseTime != usedTimes && (freetimes - usedTimes == 0))
            {
                LTDailyDataManager.Instance.SetDailyDataRefreshState();
            }
            last_one_hour = EB.Dot.Long("last_one_hour", obj, last_one_hour);
            InfoData = EBCore.Dot.Array("one_hour_reward", obj, InfoData);
            if (InfoData != null && InfoData.Count>0)
            {
                foreach(var temp in InfoData)
                {
                    reward = EB.Dot.Integer("quantity", temp, reward);
                    return;
                }
            }
            else
            {
                reward = 0;
            }
            
          
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public void CleanUp()
        {
            point = String.Empty;
            usedTimes = 0;
            ticket = 0;
            quantity = 0;
            last_one_hour = 0;
            InfoData = null;
            reward = 0;
        }

        public object Clone()
        {
            return new HonorArenaInfo();
        }
    }
    
}