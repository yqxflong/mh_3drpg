using EB.Sparx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{

    public class PlayerInviteApi : EB.Sparx.SparxAPI
    {
        public PlayerInviteApi()
        {
            endPoint = Hub.Instance.ApiEndPoint;
        }

        private int BlockService(EB.Sparx.Request request, System.Action<Hashtable> datahandler)
        {
            LoadingSpinner.Show();

            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                LoadingSpinner.Hide();
                ProcessInviteResult(response, datahandler);
            });
        }

        private int Service(EB.Sparx.Request request, System.Action<Hashtable> datahandler)
        {
            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                ProcessInviteResult(response, datahandler);
            });
        }

        private void ProcessInviteResult(EB.Sparx.Response response, System.Action<Hashtable> datahandler)
        {
            if (response.sucessful && response.result != null)
            {
                ProcessResult(response);
                datahandler(response.hashtable);
            }
            else if (!response.sucessful)
            {
                if(!ErrorHandle(response, CheckError(response.error.ToString())))
                {
                    ProcessError(response, CheckError(response.error.ToString()));
                }
                
            }
        }


        public void GotDailyShareReward(System.Action<Hashtable> datahandler)
        {
            Request request = endPoint.Post("/invitefriend/getShareAward");
            BlockService(request, datahandler);
        }

        public void RecieveInviteTaskReward(System.Action<Hashtable> datahandler)
        {
            Request request = endPoint.Post("/invite/gottaskreward");
            BlockService(request, datahandler);
        }

        public void BindInvitePlayer(string code, System.Action<Hashtable> datahandler)
        {
            Request request = endPoint.Post("/invitefriend/useInviteCode");
            request.AddData("inviteCode", code);
            BlockService(request, datahandler);
        }

        private bool ErrorHandle(EB.Sparx.Response response, EB.Sparx.eResponseCode code)
        {
            string errorcode = response.error.ToString();
            switch (errorcode)
            {
                case "same ip or same device":
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_INVITE_24"));//相同ip或设备
                    return true;
                case "invalid code":
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_INVITE_25"));//错误邀请码
                    return true;
                case "used":
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_INVITE_27"));//只能绑定一次邀请码
                    return true;
                default:
                    break;
            }
            return false;
        }

    }
}