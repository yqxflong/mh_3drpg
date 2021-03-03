using System.Collections;
using System.Collections.Generic;
using EB.Sparx;
using UnityEngine;

namespace Hotfix_LT.UI
{
    //from: https://blog.uwa4d.com/archives/USparkle_RedPoint.html

    public class LTRedPointSystem : ManagerUnit
    {
        private static List<string> TrssList = new List<string>
        {
            RedPointConst.main,
            RedPointConst.challenge,
            RedPointConst.partner,
            RedPointConst.setting ,
            RedPointConst.otherSetting ,
            RedPointConst.expedition ,
            RedPointConst.resourcegold ,
            RedPointConst.resourceexp ,
            RedPointConst.mainbox ,
            RedPointConst.alienmaze,
            RedPointConst.mainchapterreward,

            RedPointConst.awaken ,
            RedPointConst.climingtower ,
            RedPointConst.bag ,
            RedPointConst.legion ,
            RedPointConst.handbook,
            RedPointConst.dayactivity ,
            RedPointConst.specialactivity,
            RedPointConst.task ,
            RedPointConst.sign ,
            RedPointConst.drawcard ,

            RedPointConst.chargegift ,
            RedPointConst.chargeprivilege,
            RedPointConst.welfare,
            RedPointConst .daytask ,
            RedPointConst .weektask ,
            RedPointConst .maintask,
            RedPointConst .chargedaygift,
            RedPointConst .chargeweekgift,
            RedPointConst .chargemonthgift,
            RedPointConst .silverprivilege ,

            RedPointConst .goldprivilege,
            RedPointConst .drawcardgold,
            RedPointConst .drawcardhc,
            RedPointConst .firstcharge ,
            RedPointConst.sevenreward ,
            RedPointConst.growupaward,
            RedPointConst.levelaward,
            RedPointConst.heromedal ,
            RedPointConst.diamondgift,
            RedPointConst.dailyact,

            RedPointConst .limitact,
            RedPointConst .havelegion ,
            RedPointConst.haveapply,
            RedPointConst.donate ,
            RedPointConst.havedonatecount ,
            RedPointConst.havedonatechest ,

            RedPointConst.haveevent ,
            RedPointConst.mercenary ,
            RedPointConst.legionactivity ,
            RedPointConst.convoy,
            RedPointConst.legiontechnology,
            RedPointConst.techchest,
            RedPointConst.techskill1,
            RedPointConst.techskill2,
            RedPointConst.techskill3,
            RedPointConst.techskill4,
            RedPointConst.techskill5,
            RedPointConst.techskill6,
            RedPointConst.techskill7,
            RedPointConst.techskill8,
            RedPointConst.techskill9,
            RedPointConst.legionfb,
            RedPointConst.legionwar ,
            RedPointConst.maininstancegift,

            RedPointConst.comeback,
            RedPointConst .cblogin,
            RedPointConst .cbtask ,

            RedPointConst.invite,
            RedPointConst.invitereward,
            RedPointConst.invitedreward,
            RedPointConst.invitelogin,
            RedPointConst.invitelevelup,
            RedPointConst.invitecharge,
            RedPointConst.invitedlogin,
            RedPointConst.invitedlevelup,
            RedPointConst.invitedcharge,

			RedPointConst.vipgift,
            RedPointConst.promotion,
	};

        private static LTRedPointSystem instance = null;

        public static LTRedPointSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LTHotfixManager.GetManager<LTRedPointSystem>();
                }
                return instance;
            }
        }

        public override void Initialize(Config config)
        {
            base.Initialize(config);
            InitRedPointTressNode();
        }
        
        public delegate void OnNumChange(RedPointNode node);

        private RedPointNode Root;
        private void InitRedPointTressNode()
        {
            Root = new RedPointNode();
            Root.name = RedPointConst.main;

            for(int i=0;i< TrssList.Count;++i)
            {
                RedPointNode node = Root;
                string[] treeNodeAy = TrssList[i].Split('.');
                if (treeNodeAy[0]!=node.name)
                {
                    EB.Debug.LogWarning("InitRedPointTressNode Error:{0}", treeNodeAy[0]);
                    continue;
                }

                if(treeNodeAy .Length > 1)
                {
                    for(int j=1;j< treeNodeAy.Length; ++j)
                    {
                        if(!node.childs.ContainsKey(treeNodeAy[j]))
                        {
                            node.childs.Add(treeNodeAy[j], new RedPointNode());
                        }
                        node.childs[treeNodeAy[j]].name = treeNodeAy[j];
                        node.childs[treeNodeAy[j]].parent = node;

                        node = node.childs[treeNodeAy[j]];
                    }
                }
            }
        }

        public void AddRedPointNodeCallBack(string str,LTRedPointSystem .OnNumChange callBack)
        {
            var nodeList = str.Split('.');
            if(nodeList.Length == 1&& nodeList[0] != RedPointConst.main)
            {
                EB.Debug.LogWarning("SetRedPointNodeCallBack str Error:{0}", str);
                return;
            }
            RedPointNode node = Root;
            for(int i=1;i<nodeList.Length; ++i)
            {
                if(!node.childs.ContainsKey(nodeList[i]))
                {
                    EB.Debug.LogWarning("SetRedPointNodeCallBack str Error in:{0} - {1}", str, nodeList[i]);
                    return;
                }
                node = node.childs[nodeList[i]];

                if(i==nodeList.Length - 1)
                {
                    callBack(node);
                    node.numChangeFunc += callBack;
                    return;
                }
            }
        }

        public void RemoveRedPointNodeCallBack(string str, LTRedPointSystem.OnNumChange callBack)
        {
            var nodeList = str.Split('.');
            if (nodeList.Length == 1 && nodeList[0] != RedPointConst.main)
            {
                EB.Debug.LogWarning("SetRedPointNodeCallBack str Error:{0}", str);
                return;
            }
            RedPointNode node = Root;
            for (int i = 1; i < nodeList.Length; ++i)
            {
                if (!node.childs.ContainsKey(nodeList[i]))
                {
                    EB.Debug.LogWarning("SetRedPointNodeCallBack str Error in:{0} - {1}", str, nodeList[i]);
                    return;
                }
                node = node.childs[nodeList[i]];

                if (i == nodeList.Length - 1)
                {
                    node.numChangeFunc -= callBack;
                    return;
                }
            }
        }

        public void SetRedPointNodeNum(string str,int num)
        {
            var nodeList = str.Split('.');
            if (nodeList.Length == 1 && nodeList[0] != RedPointConst.main)
            {
                EB.Debug.LogWarning("SetRedPointNodeNum str Error:{0}", str);
                return;
            }
            RedPointNode node = Root;
            for (int i = 1; i < nodeList.Length; ++i)
            {
                if (!node.childs.ContainsKey(nodeList[i]))
                {
                    EB.Debug.LogWarning("Please add \"{0}\" to LTRedPointSystem.TreeList.SetRedPointNodeCallBack str Error in:{0} - {1}", str, nodeList[i]);
                    return;
                }
                node = node.childs[nodeList[i]];

                if (i == nodeList.Length - 1)
                {
                    //EB.Debug.LogError("测试红点：Set->{0}:{1}", str, num);
                    node.SetNum(num);
                }
            }
        }

    }
}
