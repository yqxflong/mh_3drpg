using EB;
using EB.Sparx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LoginAPI : SparxAPI
    {
        private Hashtable _deviceInfo;

        public LoginAPI(EndPoint endpoint) : base(endpoint)
        {
            _deviceInfo = Version.GetDeviceInfo();
            if (TestX.Ins == null || string.IsNullOrEmpty(TestX.Ins.login))
            {
                _deviceInfo["udid"] = Device.UniqueIdentifier;
            }
            else
            {
                _deviceInfo["udid"] = TestX.Ins.GetLoginUid(Device.UniqueIdentifier);
            }

        }

        void AddData(Request request)
        {
            request.AddData("platform", Device.MobilePlatform);
            request.AddData("device", _deviceInfo);
            request.AddData("version", Version.GetVersion());
            request.AddData("locale", Version.GetLocale());
            request.AddData("lang", Localizer.GetSparxLanguageCode(Hub.Instance.Config.Locale));
            request.AddData("cellular", Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ? 1 : 0);
            request.AddData("tz", Version.GetTimeZoneOffset());
        }

        Request Post(string path)
        {
            var req = endPoint.Post(path);
            AddData(req);
            return req;
        }

        public override bool ProcessResponse(EB.Sparx.Response response)
        {
            if (!response.sucessful)
            {
                EB.Sparx.eResponseCode errCode = CheckError(response.error.ToString());
                if (errCode != EB.Sparx.eResponseCode.Success && !ProcessError(response, errCode))
                {
                    EB.Debug.LogError("LoginAPI.ProcessResponse: request {0} failed, {1}", response.request.uri, response.error);
                    return false;
                }
            }

            return ProcessResult(response);
        }
        
        protected override bool ProcessResult(EB.Sparx.Response response)
        {
            return true;
        }

        public void Init(System.Action<string, object> callback)
        {
            var req = Post("/auth/init");
            req.numRetries = 1;
            req.suspendMethod = Request.eSuspendMethod.Break;
            endPoint.Service(req, delegate (Response res)
            {
                if (res.sucessful)
                {
                    callback(null, res.result);
                }
                else
                {
                    callback(res.localizedError, null);
                }
            });
        }

        public void PreLogin(System.Action<string, object> callback)
        {
            var req = Post("/auth/prelogin");
            req.numRetries = 1;
            req.suspendMethod = Request.eSuspendMethod.Break;
            endPoint.Service(req, delegate (Response res)
            {
                if (res.sucessful)
                {
                    callback(null, res.result);
                }
                else
                {
                    callback(res.localizedError, res.result);
                }
            });
        }

        public void Login(string authenticator, object credentials, Hashtable addData, System.Action<string, object> callback)
        {
            var req = Post("/auth/login");
            req.numRetries = 1;
            req.suspendMethod = Request.eSuspendMethod.Break;
            req.AddData("authenticator", authenticator);
            req.AddData("credentials", credentials);

            if (addData != null)
            {
                req.AddData(addData);
            }

            endPoint.Service(req, delegate (Response res)
            {
                if (res.sucessful)
                {
                    var stoken = Dot.String("stoken", res.hashtable, null);

                    if (!string.IsNullOrEmpty(stoken))
                    {
                        endPoint.AddData("stoken", stoken);
                        BugReport.AddData("stoken", stoken);
                    }

                    callback(null, res.hashtable);
                }
                else
                {
                    callback(res.localizedError, res.result);
                }
            });
        }

        //#if DEBUG
        public void ResetWorldUser(Hashtable chardata, System.Action<string, object> callback)
        {
            var req = Post("/auth/resetWorldUser");
            req.numRetries = 1;
            req.suspendMethod = Request.eSuspendMethod.Break;
            req.AddData(chardata);
            endPoint.Service(req, delegate (Response res)
            {
                if (res.sucessful)
                {
                    callback(null, res.hashtable);
                }
                else
                {
                    callback(res.localizedError, null);
                }
            });
        }
        //#endif

        public void LoginData(Hashtable data, System.Action<string, Hashtable> callback)
        {
            var req = Post("/auth/accountData");
            req.numRetries = 1;
            req.suspendMethod = Request.eSuspendMethod.Break;
            req.AddData(data);
            endPoint.Service(req, delegate (Response res)
            {
                if (ProcessResponse(res))
                {
                    callback(null, res.hashtable);
                }
                else
                {
                    callback(res.localizedError, null);
                }
            });
        }

        public void CheckName(string name, System.Action<string, object> callback)
        {
            var req = endPoint.Post("/auth/check-name");
            req.numRetries = 1;
            req.suspendMethod = Request.eSuspendMethod.Break;
            req.AddData("name", name);
            endPoint.Service(req, delegate (Response res)
            {
                if (res.sucessful)
                {
                    callback(null, res.hashtable);
                }
                else
                {
                    callback(res.localizedError, null);
                }
            });
        }

        public void ChangeUser(string characterId, System.Action<string, object> callback)
        {
            var req = endPoint.Post("/herostats/changeUserHeroTemplateId");
            req.numRetries = 1;
            req.suspendMethod = Request.eSuspendMethod.Break;
            req.AddData("characterId", characterId);
            endPoint.Service(req, delegate (Response res)
            {
                if (res.sucessful)
                {
                    callback(null, res.hashtable);
                }
                else
                {
                    callback(res.localizedError, null);
                }
            });
        }

        public void SetName(string name, System.Action<string, object> callback)
        {
            var req = endPoint.Post("/account/name");
            req.AddData("name", name);
            endPoint.Service(req, delegate (Response res)
            {
                if (res.sucessful)
                {
                    callback(null, res.hashtable);
                }
                else
                {
                    callback(res.localizedError, null);
                }
            });
        }

        public void GetSupportUrl(System.Action<string, object> callback)
        {
            var req = endPoint.Get("/account/support");
            endPoint.Service(req, delegate (Response res)
            {
                if (res.sucessful)
                {
                    callback(null, res.hashtable);
                }
                else
                {
                    callback(res.localizedError, null);
                }
            });
        }
    }
}