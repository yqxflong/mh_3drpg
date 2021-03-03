using UnityEngine;
using System.Collections;
using EB.Sparx;


namespace Hotfix_LT.UI
{
    public class LTLegionWarApi : EB.Sparx.SparxAPI
    {

        public LTLegionWarApi()
        {
            endPoint = Hub.Instance.ApiEndPoint;
        }

        //预赛敌人
        public void GetQualifyWarEnemyList(System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/awar/barragePlayerList");
            BlockService(request, dataHandler);
        }

        //预赛发起进攻
        public void StartChallenge(string tuid, System.Action<Hashtable> dataHandler = null)
        {
            EB.Sparx.Request request = endPoint.Post("/awar/startbattle");
            //request.AddData("tuid", tuid);//target is protected
            request.AddData("guid", tuid);//target is protected
            BlockService(request, dataHandler);
        }

        //？？
        public void EnterQualifyWar(System.Action<Hashtable> dataHandler = null)
        {
            EB.Sparx.Request request = endPoint.Post("/awar/enterAllianceBattle");
            ErrorRedultFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    return true;
                }
                return false;
            };
            BlockService(request, dataHandler);
        }

        //判断是否锁定阵容
        public void HasAllianceTeam(System.Action<Hashtable> dataHandler = null)
        {
            EB.Sparx.Request request = endPoint.Post("/awar/joinAllianceWar");
            BlockService(request, dataHandler);
        }

        //得到个人积分
        public void GetPersonRank(System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/awar/personRankList");
            ErrorRedultFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    return true;
                }
                return false;
            };
            BlockService(request, dataHandler);
        }

        //得到团队积分
        public void GetLegionRank(System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/awar/allianceRankList");
            ErrorRedultFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    return true;
                }
                return false;
            };
            BlockService(request, dataHandler);
        }

        //得到半决赛名单
        public void InitSemifinal(System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/awar/initSemi");
            BlockService(request, dataHandler);
        }

        //拉赛程列表
        /*public void GetSemiOrFinalAllianceList(System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/awar/getSemiOrFinalAllianceList");
            BlockService(request, dataHandler);
        }*/

        //进入不同场次（风水火）？
        public void EnterSemiFinalFiled(int FieldNumber, int type, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/awar/enterSemibattleField");
            request.AddData("fieldNumber", FieldNumber);
            request.AddData("type", type);
            ErrorRedultFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    return true;
                }
                return false;
            };
            BlockService(request, dataHandler);
        }

        public void LeavePreFiled()//离开预赛赛程
        {
            EB.Sparx.Request request = endPoint.Post("/awar/removeMemberWhenExitPreAwar");
            BlockService(request, null);
        }
        public void LeaveSemiOrFinalFiled()//离开半决/决赛程
        {
            EB.Sparx.Request request = endPoint.Post("/awar/removeMemberWhenExit");
            BlockService(request, null);
        }



        //改变位置
        public void changeBattleFieldPosition(int FieldNumber, int type, int position, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/awar/changeBattleFieldPosition");

            request.AddData("toFieldNumber", FieldNumber);
            request.AddData("toType", type);
            request.AddData("toPosition", position);
            request.AddData("aid", LegionModel.GetInstance().legionData.legionID);

            BlockService(request, dataHandler);
        }

        //帮主换位
        public void leaderChangeMemberPosition(int FieldNumber, int type, int position1, int position2, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/awar/leaderChangeMemberPosition");

            request.AddData("fieldNumber", FieldNumber);
            request.AddData("type", type);
            request.AddData("position1", position1);
            request.AddData("position2", position2);
            request.AddData("aid", LegionModel.GetInstance().legionData.legionID);

            BlockService(request, dataHandler);
        }

        //GM
        public void startSemiOrFinalBattle()//开启赛事 
        {
            EB.Sparx.Request request = endPoint.Post("/awar/startSemiOrFinalBattle");
            BlockService(request, null);
        }
        public void ResetWar(System.Action<Hashtable> dataHandler = null)//恢复半决/决比赛状态
        {
            EB.Sparx.Request request = endPoint.Post("/awar/onResetWar");
            BlockService(request, dataHandler);
        }

        public void debugAutoCombat()
        {
            EB.Sparx.Request request = endPoint.Post("/awar/debugAutoCombat");
            BlockService(request, null);
        }
        public void AddBotInWar(int FieldNumber, int type, int position, string camp, System.Action<Hashtable> dataHandler)//半决/决比赛加机器人
        {
            EB.Sparx.Request request = endPoint.Post("/awar/addOneRobotForSemiOrFinalBattle");

            request.AddData("toFieldNumber", FieldNumber);
            request.AddData("toType", type);
            request.AddData("toPosition", position);
            request.AddData("camp", camp);

            BlockService(request, dataHandler);
        }
        public void RemoveBotInWar(int FieldNumber, int type, int position, string camp, System.Action<Hashtable> dataHandler)//半决/决比赛减机器人
        {
            EB.Sparx.Request request = endPoint.Post("/awar/removeOneRobotForSemiOrFinalBattle");

            request.AddData("toFieldNumber", FieldNumber);
            request.AddData("toType", type);
            request.AddData("toPosition", position);
            request.AddData("camp", camp);

            BlockService(request, dataHandler);
        }
        public void RequireGMBot(System.Action<Hashtable> dataHandler)//预赛呼叫机器人
        {
            EB.Sparx.Request request = endPoint.Post("/awar/addOneRobotForTest");
            BlockService(request, dataHandler);
        }

        public void OnOpenWar(int stage, System.Action<Hashtable> dataHandler = null)//开启赛程
        {
            EB.Sparx.Request request = endPoint.Post("/awar/createService");
            request.AddData("stage", stage);
            var gameWorlds = LoginManager.Instance.GameWorlds;
            var gameWorld = System.Array.Find(gameWorlds, w => w.Default);
            if (gameWorld != null)
            {
                request.AddData("realmId", gameWorld.Id);
            }
            BlockService(request, dataHandler);
        }
        public void OnCloseWar(int stage, System.Action<Hashtable> dataHandler = null)//关闭赛程
        {
            EB.Sparx.Request request = endPoint.Post("/awar/destoryService");
            request.AddData("stage", stage);
            var gameWorlds = LoginManager.Instance.GameWorlds;
            var gameWorld = System.Array.Find(gameWorlds, w => w.Default);
            if (gameWorld != null)
            {
                request.AddData("realmId", gameWorld.Id);
            }
            BlockService(request, dataHandler);
        }
        public void OnCheckWarStage(System.Action<Hashtable> dataHandler = null)//查看赛程状态
        {
            EB.Sparx.Request request = endPoint.Post("/awar/getCurrentStage");
            ErrorRedultFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    return true;
                }

                return false;
            };
            BlockService(request, dataHandler);
        }

        public void OnReceviceBox(string achievementId, System.Action<Hashtable> dataHandler = null)//领取初赛宝箱
        {
            EB.Sparx.Request request = endPoint.Post("/awar/getAwarPreIntegralReward");
            request.AddData("achievementId", achievementId);
            ErrorRedultFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    return true;
                }

                return false;
            };
            BlockService(request, dataHandler);
        }
        public void changeTeam(int index, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/awar/setAllianceInSemiOrFinalForTest");
            request.AddData("index", index);
            BlockService(request, dataHandler);
        }

        public void ReceiveRedPacket(int FieldNumber, int type, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/awar/receiveRedPaper");
            request.AddData("fieldNumber", FieldNumber);
            request.AddData("type", type);
            BlockService(request, dataHandler);
        }

        private int BlockService(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
        {
            LoadingSpinner.Show();

            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                LoadingSpinner.Hide();

                ProcessLegionWarResult(response, dataHandler);
            });
        }

        private int Service(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
        {
            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                ProcessLegionWarResult(response, dataHandler);
            });
        }

        private void DefaultDataHandler(Hashtable LTLegionWarData)
        {
            EB.Debug.Log("LTLEgionWarAPI.DefaultDataHandler: call default data handler");
        }

        public System.Func<EB.Sparx.Response, bool> ErrorRedultFunc = null;
        private void ProcessLegionWarResult(EB.Sparx.Response response, System.Action<Hashtable> dataHandler)
        {
            dataHandler = dataHandler ?? new System.Action<Hashtable>(DefaultDataHandler);
            if (ErrorRedultFunc != null)
            {
                if (ErrorRedultFunc(response))//ture为允许的错误
                {
                    if (response.sucessful)
                    {
                        dataHandler(response.hashtable);
                    }
                    else
                    {
                        dataHandler(null);
                    }
                }
                else
                {
                    if (ProcessResponse(response))
                    {
                        dataHandler(response.hashtable);
                    }
                    else
                    {
                        dataHandler(null);
                    }
                }
            }
            else
            {
                if (ProcessResponse(response))
                {
                    dataHandler(response.hashtable);
                }
                else
                {
                    dataHandler(null);
                }
            }

        }
    }
}
