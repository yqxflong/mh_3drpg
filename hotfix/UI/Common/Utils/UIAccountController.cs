using UnityEngine;
using System.Collections;
using EB.Sparx;

namespace Hotfix_LT.UI
{
	public class UIAccountController
	{
		static EndPoint accep = EndPointFactory.Create(GameEngine.Instance.GetAuthAPIAddress(), new EndPointOptions());

		static protected DataLookupSparxManager DataLookupsSparxManager
		{
			get { return SparxHub.Instance.GetManager<DataLookupSparxManager>(); }
		}


		static bool isRequesting;
		//获取验证码
		static public void RegByPhone(string phone, System.Action<string> callback)
		{
			if (isRequesting)
			{
				return;
			}
			isRequesting = true;
			Request r = accep.Post("/user/regbysms");
			r.AddData("phone", phone);
			DataLookupsSparxManager.Service(r, delegate (Response res)
			{
				EB.Debug.Log("MHLogin.msg:{0}" , res.msg);
				isRequesting = false;
				if (res.sucessful && (string)res.msg == "OK")
				{
					callback(string.Empty);
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_UIAccountController_1161"));
				}
				else if (string.IsNullOrEmpty((string)res.msg))
				{
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_UIAccountController_1302"));
				}
				else
				{
					callback((string)res.msg);
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, (string)res.msg);
				}
			}, false);
		}

		//提交验证码
		static public void SMSVerify(string phone, string rcode, System.Action<bool> callback)
		{
			if (isRequesting)
			{
				return;
			}
			isRequesting = true;
			Request r = accep.Post("/user/verifysms");
			r.AddData("phone", phone);
			r.AddData("rcode", rcode);
			DataLookupsSparxManager.Service(r, delegate (Response res)
			{
				EB.Debug.Log("MHLogin.msg:{0}" , res.msg);
				isRequesting = false;
				if (res.sucessful && (string)res.msg == "OK")
				{
					callback(true);
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_UIAccountController_2001"));
				}
				else if (string.IsNullOrEmpty((string)res.msg))
				{
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_UIAccountController_1302"));
				}
				else
				{
					callback(false);
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, (string)res.msg);
				}
			}, false);
		}

		//补全创建信息
		static public void CreateUser(string securepwd, string inviter, System.Action callback)
		{
			if (isRequesting)
			{
				return;
			}
			isRequesting = true;
			Request r = accep.Post("/user/createuser");
			r.AddData("securepwd", securepwd);
			r.AddData("inviter", inviter);
			DataLookupsSparxManager.Service(r, delegate (Response res)
			{
				EB.Debug.Log("MHLogin.msg:{0}" , res.msg);
				isRequesting = false;
				if (res.sucessful && (string)res.msg == "OK")
				{
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_UIAccountController_2826"));
					callback();
				}
				else if (string.IsNullOrEmpty((string)res.msg))
				{
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_UIAccountController_1302"));
				}
				else
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, (string)res.msg);
			}, false);
		}

		//登录
		static public void LoginByPhone(string phone, string securepwd, System.Action<string, string, string, string> callback)
		{
			if (isRequesting)
			{
				return;
			}
			isRequesting = true;
			Request r = accep.Post("/user/login");
			r.AddData("appid", "1");
			r.AddData("logintype", "phone");
			r.AddData("phone", phone);
			r.AddData("securepwd", securepwd);
			DataLookupsSparxManager.Service(r, delegate (Response res)
			{
				EB.Debug.Log("MHLogin.msg:{0}" , res.msg);
				isRequesting = false;
				if (res.sucessful && (string)res.msg == "OK")
				{
					if (res.hashtable == null)
					{
						object obj = EB.JSON.Parse(res.text);
						res.hashtable = obj as Hashtable;
					}
					string openId = res.hashtable["id"] as string;
					string accessToken = EB.Dot.String("oauth.openkey", res.hashtable, string.Empty);
					callback(phone, openId, accessToken, securepwd);
				}
				else if (string.IsNullOrEmpty((string)res.msg))
				{
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_UIAccountController_1302"));
				}
				else
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, (string)res.msg);

			}, false);
		}

		static public void Forgetpassword(string phone, System.Action<string> callback, bool isShowSuccessTips = true)
		{
			if (isRequesting)
			{
				return;
			}
			isRequesting = true;
			Request r = accep.Post("/user/forgetpassword");
			r.AddData("phone", phone);
			DataLookupsSparxManager.Service(r, delegate (Response res)
			{
				EB.Debug.Log("MHLogin.msg:{0}" , res.msg);
				isRequesting = false;
				if (res.sucessful && (string)res.msg == "OK")
				{
					if (isShowSuccessTips)
						MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_UIAccountController_4747"));
					callback((string)res.msg);
				}
				else if (string.IsNullOrEmpty((string)res.msg))
				{
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_UIAccountController_1302"));
				}
				else
				{
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, (string)res.msg);
					callback((string)res.msg);
				}
			}, false);
		}

		static public void ForgetpasswordVerify(string phone, string rcode, System.Action<bool> callback, bool isShowSuccessTips = true)
		{
			if (isRequesting)
			{
				return;
			}
			isRequesting = true;
			Request r = accep.Post("/user/forgetpasswordVerify");
			r.AddData("phone", phone);
			r.AddData("rcode", rcode);
			DataLookupsSparxManager.Service(r, delegate (Response res)
			{
				EB.Debug.Log("MHLogin.msg:{0}" , res.msg);
				isRequesting = false;
				if (res.sucessful && (string)res.msg == "OK")
				{
					if (isShowSuccessTips)
						MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_UIAccountController_5670"));
					callback(true);
				}
				else if (string.IsNullOrEmpty((string)res.msg))
				{
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_UIAccountController_1302"));
				}
				else
				{
					callback(false);
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, (string)res.msg);
				}
			}, false);
		}

		static public void Modifypassword(string phone, string originpwd, string securepwd, System.Action callback, bool isShowSuccessTips = true)
		{
			if (isRequesting)
			{
				return;
			}
			isRequesting = true;
			Request r = accep.Post("/user/modifypassword");
			r.AddData("phone", phone);
			r.AddData("originpwd", originpwd);
			r.AddData("securepwd", securepwd);
			DataLookupsSparxManager.Service(r, delegate (Response res)
			{
				EB.Debug.Log("MHLogin.msg:{0}" , res.msg);
				isRequesting = false;
				if (res.sucessful && (string)res.msg == "OK")
				{
					if (isShowSuccessTips)
						MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_UIAccountController_6616"));
					callback();
				}
				else if (string.IsNullOrEmpty((string)res.msg))
				{
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_UIAccountController_1302"));
				}
				else
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, (string)res.msg);
			}, false);
		}

		static public void License(System.Action<string> callback)
		{
			if (isRequesting)
			{
				return;
			}
			isRequesting = true;
			Request r = accep.Post("/user/license");
			DataLookupsSparxManager.Service(r, delegate (Response res)
			{
				EB.Debug.Log($"MHLogin.msg:  {res.msg}");
				EB.Debug.Log($"MHLogin.result: {res.result.ToString()}");
				isRequesting = false;
				if (res.sucessful && (string)res.msg == "OK")
				{
					//MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_UIAccountController_6616"));
					callback(res.result.ToString());
				}
				else if (string.IsNullOrEmpty((string)res.msg))
				{
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_UIAccountController_1302"));
				}
				else
					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, (string)res.msg);
			}, false);
		}
	}
}