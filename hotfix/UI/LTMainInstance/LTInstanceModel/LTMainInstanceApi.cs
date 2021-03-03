using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTMainInstanceApi : EB.Sparx.SparxAPI
    {
        public LTMainInstanceApi()
        {
            endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
        }

        private void DefaultDataHandler(Hashtable data)
        {
            EB.Debug.Log("LTMainInstanceApi.DefaultDataHandler: call default data handler");
        }

        private void ProcessResult(EB.Sparx.Response response, System.Action<Hashtable> dataHandler)
        {
            dataHandler = dataHandler ?? new System.Action<Hashtable>(DefaultDataHandler);
            if (ProcessResponse(response))
            {
                if (response.sucessful)
                {
                    dataHandler(response.hashtable);
                    return;
                }

                if (response.error.Equals("notEnoughPrayPoint"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTMainInstanceApi_926"));
                }
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

        private int Service(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
        {
            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                ProcessResult(response, dataHandler);
            });
        }

        public void RequestEnterChapter(int chapterId, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/enterChapter");
            request.AddData("chapter", chapterId);
            request.numRetries = 1;
            BlockService(request, dataHandler);
        }

        public void RequestFightCampaign(int campaignId, bool isFast, System.Action<Hashtable> dataHandler, int campaignType)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostmaincampaign/fightCampaign");
            request.AddData("campaignId", campaignId);
            request.AddData("campaignType", campaignType);
            request.AddData("fast", isFast);
            request.numRetries = 0;
            BlockService(request, dataHandler);
        }

        public void RequestBlitzCampaign(int campaignId, int times, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostmaincampaign/blitzCampaign");
            request.AddData("campaignId", campaignId);
            request.AddData("times", times);
            request.numRetries = 1;
            BlockService(request, dataHandler);
        }

        public void RequestGetStarReward(int star, string chapterId, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostmaincampaign/getChapterReward");
            request.AddData("stars", star);
            request.AddData("chapterId", chapterId);
            request.numRetries = 1;
            BlockService(request, dataHandler);
        }

        public void RequestPray(int index, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostchallengecampaign/pray");
            request.AddData("option", index);
            BlockService(request, dataHandler);
        }
        
        public void RequestMainChapterReward(string chapterId, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = SparxHub.Instance.ApiEndPoint.Post("/lostmaincampaign/getChapterLotteryReward");
            request.AddData("chapterId", chapterId);
            request.numRetries = 1;
            BlockService(request, dataHandler);
        }
    }
}