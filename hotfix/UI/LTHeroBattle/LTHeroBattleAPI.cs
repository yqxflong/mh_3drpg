using System;
using System.Collections;
using System.Collections.Generic;
using EB.Sparx;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTHeroBattleAPI : EB.Sparx.SparxAPI
    {
        public string battleID = "clashOfHeroes";
        public string baseID = "userClashOfHeroes";

        public HeroBattleMatchBase matchBase;
        public HeroBattleData heroBattleData;

        public LTHeroBattleAPI()
        {
            endPoint = Hub.Instance.ApiEndPoint;
            matchBase = GameDataSparxManager.Instance.Register<HeroBattleMatchBase>(baseID);
            heroBattleData = GameDataSparxManager.Instance.Register<HeroBattleData>(battleID);
        }

        private int BlockService(EB.Sparx.Request request, Func<EB.Sparx.Response, bool> responseFunc)
        {
            LoadingSpinner.Show();

            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                LoadingSpinner.Hide();
                if (responseFunc != null && !responseFunc(response)) //如果没有处理完成
            {
                    ProcessError(response);
                }
            //else
            //{
            //    if (!response.sucessful)
            //    {
            //        ProcessError(response);
            //    }
            //}
        });
        }

        private int Service(EB.Sparx.Request request, Func<EB.Sparx.Response, bool> responseFunc)
        {
            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                if (responseFunc != null && !responseFunc(response)) //如果没有处理完成
            {
                    ProcessError(response);
                }
                else
                {
                    if (!response.sucessful)
                    {
                        ProcessError(response);
                    }
                }

            });
        }

        public void GetMatchBaseInfo(Func<EB.Sparx.Response, bool> responseFunc)
        {
            EB.Sparx.Request request = endPoint.Post("/ladder/getInfo");
            //request.AddData("search", input);
            BlockService(request, responseFunc);
        }

        public void ReloadClash(Func<EB.Sparx.Response, bool> responseFunc)
        {
            EB.Sparx.Request request = endPoint.Post("/ladder/reloadClash");
            //request.AddData("search", input);
            BlockService(request, responseFunc);
        }

        public void StartMatchOther(Func<EB.Sparx.Response, bool> responseFunc)
        {
            EB.Sparx.Request request = endPoint.Post("/ladder/match");
            //request.AddData("search", input);
            BlockService(request, responseFunc);
        }

        public void QuitMatchOther(Func<EB.Sparx.Response, bool> responseFunc)
        {
            EB.Sparx.Request request = endPoint.Post("/ladder/quitMatch");
            //request.AddData("search", input);
            BlockService(request, responseFunc);
        }

        /// <summary> 获取奖励</summary>
        /// <param name="responseFunc"></param>
        public void GetReward(Func<EB.Sparx.Response, bool> responseFunc)
        {
            EB.Sparx.Request request = endPoint.Post("/ladder/getReward");
            BlockService(request, responseFunc);
        }

        public void BanOtherHero(int heroTplID, Func<EB.Sparx.Response, bool> responseFunc)
        {
            EB.Sparx.Request request = endPoint.Post("/ladder/heroBan");
            request.AddData("tplID", heroTplID.ToString());
            BlockService(request, responseFunc);
        }

        public void ChoiceHero(int heroTplID, int suitID, Func<EB.Sparx.Response, bool> responseFunc)
        {
            EB.Sparx.Request request = endPoint.Post("/ladder/heroChoice");
            request.AddData("tplID", heroTplID.ToString());
            request.AddData("suitID", suitID);
            BlockService(request, responseFunc);
        }

        private void ProcessError(EB.Sparx.Response response)
        {
            if (response.fatal)
            {
                EB.Debug.LogError("LTHeroBattleAPI.ProcessError: error {0} occur when request {1}", response.error,
                    response.request.uri);
                ProcessError(response, CheckError(response.error.ToString()));
            }
            else
            {
                EB.Sparx.eResponseCode errCode = CheckError(response.error.ToString());
                if (errCode != EB.Sparx.eResponseCode.Success && !ProcessError(response, errCode))
                {
                    EB.Debug.LogError("LTHeroBattleAPI.ProcessError: request {0} failed, {1}", response.request.uri, response.error);
                }
            }
        }

        public class HeroBattleMatchBase : INodeData
        {
            public int VictoryCount { get; set; }

            public int DefeatCount { get; set; }

            public bool IsGetReward { get; set; }

            public ArrayList FreeHeros { get; set; }

            public ArrayList BaseHeroes { get; set; }

            public HeroBattleMatchBase()
            {
                //TotalDonated = new AllianceDonate();
                //History = new int[0];
            }

            public void OnUpdate(object obj)
            {
                VictoryCount = EB.Dot.Integer("win", obj, VictoryCount);
                DefeatCount = EB.Dot.Integer("lost", obj, DefeatCount);
                IsGetReward = EB.Dot.Bool("isGetReward", obj, IsGetReward);
                FreeHeros = Hotfix_LT.EBCore.Dot.Array("freeHeroes", obj, FreeHeros);
                BaseHeroes = Hotfix_LT.EBCore.Dot.Array("baseHeroes", obj, BaseHeroes);
                if(BaseHeroes!=null)LTHeroBattleModel.GetInstance().SetMatchBase(this);
            }

            public void OnMerge(object obj)
            {
                OnUpdate(obj);
            }

            public void CleanUp()
            {
            }

            public object Clone()
            {
                return new HeroBattleMatchBase();
            }
        }

        public class ClientSideData
        {
            public int Uid;
            public string Name;
            public int Level;
            public string Portrait;
            public string Frame;
            public string[] BandTplIds;
            public List<Hashtable> SelectHeroIds;
        }
        /// <summary>
        /// 全量消息
        /// </summary>
        public class HeroBattleData : INodeData
        {
            public ClientSideData[] Sides { get; set; }

            public string Status { get; set; }

            /// <summary> 当前操作的uid</summary>
            public int OpenUid { get; set; }

            /// <summary> 剩余时间</summary>
            public int NextTS { get; set; }



            public HeroBattleData()
            {
                //TotalDonated = new AllianceDonate();
                //History = new int[0];
            }

            public void OnUpdate(object obj)
            {
                ArrayList sides = Hotfix_LT.EBCore.Dot.Array("sides", obj, null);
                if (sides != null)
                {
                    Sides = new ClientSideData[sides.Count];
                    for (int i = 0; i < sides.Count; i++)
                    {
                        Sides[i] = new ClientSideData();
                        Sides[i].Uid = EB.Dot.Integer("uid", sides[i], Sides[i].Uid);
                        Sides[i].Name = EB.Dot.String("name", sides[i], Sides[i].Name);
                        Sides[i].Level = EB.Dot.Integer("level", sides[i], Sides[i].Level);
                        Sides[i].Portrait = EB.Dot.String("portrait", sides[i], Sides[i].Portrait);
                        if (string.IsNullOrEmpty(Sides[i].Portrait))
                        {
                            Sides[i].Portrait = "Partner_Head_Sidatuila";
                        }
                        string frameStr = EB.Dot.String("headFrame", sides[i], null);
                        Sides[i].Frame = EconemyTemplateManager.Instance.GetHeadFrame(frameStr).iconId;
                        ArrayList arraySelects = Johny.ArrayListPool.Claim();
                        ArrayList arraybands = Johny.ArrayListPool.Claim();
                        arraybands = Hotfix_LT.EBCore.Dot.Array("bandTplIds", sides[i], arraybands);
                        arraySelects = Hotfix_LT.EBCore.Dot.Array("selectedTplIds", sides[i], arraySelects);
                        Sides[i].BandTplIds = new string[arraybands.Count];
                        Sides[i].SelectHeroIds = new List<Hashtable>();

                        for (int j = 0; j < arraybands.Count; j++)
                        {
                            Sides[i].BandTplIds[j] = arraybands[j] as string;
                        }

                        for (var k = 0; k < arraySelects.Count; k++)
                        {
                            Hashtable v = arraySelects[k] as Hashtable;
                            Sides[i].SelectHeroIds.Add(v as Hashtable);
                        }
                    }
                }
                else
                {
                    EB.Debug.Log("sides == null");
                }
                Status = EB.Dot.String("status", obj, Status);
                OpenUid = EB.Dot.Integer("opUid", obj, OpenUid);
                NextTS = EB.Dot.Integer("nextTs", obj, NextTS);
                LTHeroBattleModel.GetInstance().SetChoiceData(this);
            }

            public void OnMerge(object obj)
            {
                OnUpdate(obj);
            }

            public void CleanUp()
            {
            }

            public object Clone()
            {
                return new HeroBattleMatchBase();
            }
        }

    }
}