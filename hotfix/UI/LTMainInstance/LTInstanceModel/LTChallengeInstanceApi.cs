using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceApi : EB.Sparx.SparxAPI
    {
        public LTChallengeInstanceApi()
        {
            endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
        }

        private void DefaultDataHandler(Hashtable data)
        {
            EB.Debug.Log("LTChallengeInstanceApi.DefaultDataHandler: call default data handler");
        }

        private void ProcessResult(EB.Sparx.Response response, System.Action<Hashtable> dataHandler)
        {
            if (!string.IsNullOrEmpty(response.error))
            {
                if(response.error.Equals("timeout"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDrawCardAPI_3009"));
                    dataHandler(null);
                }
                else
                {
                    UIStack.Instance.ShowLoadingScreen(delegate
                    {
                        LoadingSpinner.Hide();
                        LTHotfixManager.GetManager<SceneManager>().RequestPlayState();
                    }, false, true);
                }
                return;
            }
            dataHandler = dataHandler ?? new System.Action<Hashtable>(DefaultDataHandler);
            if (ProcessResponse(response))
            {
                dataHandler(response.hashtable);
            }
            else
            {
                dataHandler(null);
            }
        }

        private int BlockService(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
        {
            LoadingSpinner.Show();

            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                LoadingSpinner.Hide();

                ProcessResult(response, dataHandler);
            });
        }

        public int Service(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
        {
            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                ProcessResult(response, dataHandler);
            });
        }

        public int ServiceWithError(EB.Sparx.Request request, System.Action<object, Hashtable> dataHandler)
        {
            return endPoint.Service(request, delegate (EB.Sparx.Response res)
            {
                if (res.error != null)
                {
                    dataHandler(res.error, null);
                }
                else
                {
                    ProcessResult(res, delegate (Hashtable data)
                    {
                        dataHandler(null, data);
                    });
                }
            });
        }

        //api后加入参数type，区别与异界迷宫的调用

        public void RequestEnterChapter(int levelNum, System.Action<object, Hashtable> dataHandler, string type = null)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/enterChapter");
            request.AddData("level", levelNum);
            if (!string.IsNullOrEmpty(type)) request.AddData("type", type);
            request.numRetries = 0;
            ServiceWithError(request, dataHandler);
        }

        public void RequestResumeChapter(int levelNum, System.Action<object, Hashtable> dataHandler, string type = null)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/resumeChapter");
            request.AddData("level", levelNum);
            if (!string.IsNullOrEmpty(type)) request.AddData("type", type);
            request.numRetries = 0;
            ServiceWithError(request, dataHandler);
        }

        public void RequestGetChapterState(System.Action<Hashtable> dataHandler, string type = null)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/getChapterState");
            if (!string.IsNullOrEmpty(type)) request.AddData("type", type);
            request.numRetries = 1;
            BlockService(request, dataHandler);
        }

        public void RequestMoveChar(int dir, System.Action<Hashtable> dataHandler, string type = null)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/move");
            request.AddData("dir", dir);
            if (!string.IsNullOrEmpty(type)) request.AddData("type", type);
            request.numRetries = 0;
            Service(request, dataHandler);
        }

        public void RequestLeaveChapter(string save, System.Action<Hashtable> dataHandler, string type = null)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/leaveChapter");
            request.AddData("save", save);
            if (!string.IsNullOrEmpty(type)) request.AddData("type", type);
            request.numRetries = 0;
            BlockService(request, dataHandler);
        }

        public void RequestFinshChapter(int param, int id, System.Action<object, Hashtable> dataHandler, string type = null)
        {
            EB.Debug.Log("RequestFinshChapter——{0}" , id);
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/finish");
            request.AddData("enterNext", param);
            request.AddData("level", id);
            if (!string.IsNullOrEmpty(type)) request.AddData("type", type);
            request.numRetries = 1;
            ServiceWithError(request, dataHandler);
        }

        public void RequestLevelInfo(System.Action<Hashtable> dataHandler, string type = null)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Get("/lostchallengecampaign/getInfo");
            if (!string.IsNullOrEmpty(type)) request.AddData("type", type);
            request.numRetries = 1;
            BlockService(request, dataHandler);
        }

        public void RequestQuickBattle(bool param, System.Action<Hashtable> dataHandler, string type = null)//不进行怪物战斗
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/debug");
            request.AddData("cmd", "instancekill");
            request.AddData("param", param);
            if (!string.IsNullOrEmpty(type)) request.AddData("type", type);
            request.numRetries = 1;
            BlockService(request, dataHandler);
        }

        public void RequestFastCombat(bool param, System.Action<Hashtable> dataHandler, string type = null)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/fastCombat");
            request.AddData("fast", param);
            if (!string.IsNullOrEmpty(type)) request.AddData("type", type);
            request.numRetries = 1;
            BlockService(request, dataHandler);
        }

        public void RequestBuyScroll(int[] pos, int id, System.Action<Hashtable> dataHandler, string type = null)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/action");
            request.AddData("cmd", id);
            request.AddData("pos", pos);
            if (!string.IsNullOrEmpty(type)) request.AddData("type", type);
            request.numRetries = 1;
            BlockService(request, dataHandler);
        }
        
        public void RequestPickScroll(int[] pos, int id,System.Action<Hashtable> dataHandler, string type = null)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/action");
            request.AddData("cmd", "pickScroll");
            request.AddData("pos", pos);
            request.AddData("param", id);
            request.numRetries = 1;
            BlockService(request, dataHandler);
        }

        public void RequestRevive(System.Action<Hashtable> dataHandler, string type = null)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/revive");
            if (!string.IsNullOrEmpty(type)) request.AddData("type", type);
            request.numRetries = 1;
            BlockService(request, dataHandler);
        }

        public void RequestConfirmFastCombat(long combatId, int confirm, System.Action<Hashtable> dataHandler, string type = null)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/combat/fastCombatConfirm");
            request.AddData("combatId", combatId);
            request.AddData("confirm", confirm);
            if (!string.IsNullOrEmpty(type)) request.AddData("type", type);
            request.numRetries = 1;
            BlockService(request, dataHandler);
        }


        public void RequestChallengeGetHeroHP(ArrayList heroes, System.Action<Hashtable> dataHandler, string type = null)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/getHeroHP");
            request.AddData("heroes", heroes);
            if (!string.IsNullOrEmpty(type)) request.AddData("type", type);
            request.numRetries = 1;
            BlockService(request, dataHandler);
        }

        public void RequestChallengeDeath(System.Action<Hashtable> dataHandler, string type = null)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/death");
            if (!string.IsNullOrEmpty(type)) request.AddData("type", type);
            request.numRetries = 1;
            BlockService(request, dataHandler);
        }

        public void RequestChallengeCampaignLuckDraw(System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/action");
            request.numRetries = 1;
            BlockService(request, dataHandler);
        } 

        public void RequestChallengeWipeOutLuckDraw(System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/rollWheelBox");
            request.numRetries = 1;
            BlockService(request, dataHandler);
        }

        public void RequestGetLowestTeamViews(int level, System.Action<Hashtable> callback)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/lostchallengecampaign/getLowestTeamViews");
            request.AddData("level", level);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                callback?.Invoke(data);
            });
        }

        public void RequestBossReward(int[] pos, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/action");
            request.AddData("cmd", "touch");
            request.AddData("pos", pos);
            request.numRetries = 1;
            BlockService(request, dataHandler);
        }
    }
}