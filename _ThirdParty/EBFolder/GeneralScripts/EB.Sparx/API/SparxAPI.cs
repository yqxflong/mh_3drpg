///////////////////////////////////////////////////////////////////////
//
//  SparxAPI.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections;
using UnityEngine;

namespace EB.Sparx
{
	public abstract class SparxAPIWithoutEndpoint
	{

		public static System.Func<EB.Sparx.Response, EB.Sparx.eResponseCode, bool> GlobalErrorHandler;
		public static System.Action<EB.Sparx.Response> GlobalResultHandler;
		public System.Func<EB.Sparx.Response, EB.Sparx.eResponseCode, bool> ErrorHandler;
		public System.Action<EB.Sparx.Response> ResultHandler;

		public eResponseCode CheckError(string err)
		{
			if (err == null)
			{
				return eResponseCode.Success;
			}

			int errCode = -1;
			if (int.TryParse(err, out errCode))
			{
#if DEBUG
				EB.Debug.LogWarning("Fusion server error: " + ((eResponseCode)errCode).ToString());
#endif

				return (eResponseCode)errCode;
			}

			return eResponseCode.Unknown;
		}

		public virtual bool ProcessResponse(EB.Sparx.Response response)
		{
			if (!response.sucessful && response.fatal)
			{
				EB.Debug.LogError("SparxAPI.ProcessResponse: error {0} occur when request {1}", response.error, response.request.uri);
				ProcessError(response, CheckError(response.error.ToString()));
				return false;
			}

			if (!response.sucessful)
			{
				EB.Sparx.eResponseCode errCode = CheckError(response.error.ToString());
				if (errCode != EB.Sparx.eResponseCode.Success && !ProcessError(response, errCode))
				{
					EB.Debug.LogError("SparxAPI.ProcessResponse: request {0} failed, {1}", response.request.uri, response.error);
					return false;
				}
			}

			return ProcessResult(response);
		}

		public virtual bool ProcessError(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
		{
			if (ErrorHandler != null)
			{
				var dels = ErrorHandler.GetInvocationList();
				foreach (var del in dels)
				{
					var func = del as System.Func<EB.Sparx.Response, EB.Sparx.eResponseCode, bool>;
					if (func(response, errCode))
					{
						EB.Debug.Log("SparxAPI.ProcessError: process error {0} success", errCode);
						return true;
					}
				}
			}

			if (GlobalErrorHandler != null)
			{
				var dels = GlobalErrorHandler.GetInvocationList();
				foreach (var del in dels)
				{
					var func = del as System.Func< EB.Sparx.Response, EB.Sparx.eResponseCode, bool>;
					if (func(response, errCode))
					{
						EB.Debug.Log("SparxAPI.ProcessError: process error {0} success", errCode);
						return true;
					}
				}
			}

			EB.Debug.LogError("SparxAPI.ProcessError: request {0} failed, {1}:{2}", response.request.uri, errCode, response.error);

			return false;
		}

		protected virtual bool ProcessResult(EB.Sparx.Response response)
		{
			if (ResultHandler != null)
			{
				ResultHandler(response);
			}

			if (GlobalResultHandler != null)
			{
				GlobalResultHandler(response);
			}

			return true;
		}
	}

    public abstract class SparxAPI : SparxAPIWithoutEndpoint
	{
		protected EB.Sparx.EndPoint endPoint
		{
			get;
			set;
		}

		public SparxAPI()
		{

		}

		public SparxAPI(EB.Sparx.EndPoint _endPoint)
		{
			UnityEngine.Debug.Assert(_endPoint != null, "sparxAPI endPoint is null");
        	endPoint = _endPoint;
		}

		public Request Post(string path)
		{
			return endPoint.Post(path);
		}

		public Request Get(string path)
		{
			return endPoint.Get(path);
		}
	}
}