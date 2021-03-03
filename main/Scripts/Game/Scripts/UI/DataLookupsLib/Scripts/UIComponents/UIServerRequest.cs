using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EB.Sparx;

public class UIServerRequest : DataLookup
{
    public string API;
    public bool cacheData = true;

    [System.Serializable]
    public class ServerParameter
    {
        public string name;
        public string parameter;
        public string dataIDParameter;
    }

    public ServerParameter[] parameters;

    /// <summary>
    /// Callbacks will get triggered when sending the request.
    /// </summary>
    [HideInInspector] public List<EventDelegate> onSendRequest = new List<EventDelegate>();

    /// <summary>
    /// Callbacks will get triggered when receive the response.
    /// </summary>
    [HideInInspector] public List<EventDelegate> onResponse = new List<EventDelegate>();

    public bool SendOnClick = false;
    public bool SendOnStart = false;

    public enum MethodValue { POST, GET };
    public MethodValue Method;

    private bool isStartComplete;

    public override void Start()
    {
        base.Start();

        if (SendOnClick)
        {
            UIEventTrigger eventTrigger = gameObject.GetUIEventTrigger();

            if (eventTrigger != null)
            {
                eventTrigger.onClick.Add(new EventDelegate(this, "SendRequest"));
            }
        }

        if (SendOnStart) 
        { 
            SendRequest();
        }

        isStartComplete = true;
    }

    protected DataLookupSparxManager DataLookupsSparxManager
    {
        get { return SparxHub.Instance.GetManager<DataLookupSparxManager>(); }
    }

    public void SendRequest()
    {
        Request request = CreateRequest();

        if (DisableUIInputDuringRequest)
        {
            InputBlockerManager.Instance.Block(InputBlockReason.UI_SERVER_REQUEST, 30f);
        }

        if (onSendRequest.Count > 0)
        {
            for (int i = 0; i < onSendRequest.Count; i++)
            {
                onSendRequest[i].Execute();
            }
        }

        DataLookupsSparxManager.Service(request, OnResponse, cacheData);
    }

    void OnResponse(Response res)
    {
        if (onResponse.Count > 0)
        {
            for (int i = 0; i < onResponse.Count; i++)
            {
                if (onResponse[i] == null)
                {
                    continue;
                }

                if (res != null)
                {
                    onResponse[i].parameters[0] = new EventDelegate.Parameter(res);

                    if (onResponse[i].parameters.Length > 1)
                    {
                        onResponse[i].parameters[1] = new EventDelegate.Parameter(GetInstanceID());
                    }
                }

                onResponse[i].Execute();
            }
        }

        if (DisableUIInputDuringRequest)
        {
            InputBlockerManager.Instance.UnBlock(InputBlockReason.UI_SERVER_REQUEST);
        }
    }

    protected virtual Request CreateRequest()
    {
        string finalAPI = "/" + API;

        Request request = (Method == MethodValue.POST) ? DataLookupsSparxManager.EndPoint.Post(finalAPI) : DataLookupsSparxManager.EndPoint.Get(finalAPI);

        foreach (ServerParameter entry in parameters)
        {
            if (!string.IsNullOrEmpty(entry.parameter) && !string.IsNullOrEmpty(entry.dataIDParameter))
            {
                EB.Debug.LogWarning("Parameter {0} has both parameter and dataIDParameter assigned. dataIDParameter will be ignored", entry.name);
            }

            object paramValue;

            if (!string.IsNullOrEmpty(entry.parameter))
            {
                paramValue = entry.parameter;
            }
            else
            {
                DataLookupsCache.Instance.SearchDataByID(entry.dataIDParameter, out paramValue); 
            }

            request.AddData(entry.name, paramValue);
        }

        return request;
    }

    #region send on lookup daa updated
    [System.Serializable]
    public struct SendOnLookupDataUpdatedEntry
    {
        public bool Empty;
        public bool NotEmpty;
    }

    private string cachedSelectedData;

    public SendOnLookupDataUpdatedEntry SendOnLookupDataUpdated;

    public override void OnLookupUpdate(string dataID, object value)
    {
        base.OnLookupUpdate(dataID, value);

        if (!DataLookupsCache.AreDataIDEqual(dataID, DefaultDataID))
        {
            return;
        }

        string valueAsString = value?.ToString();

        if (DataLookupsCache.AreDataIDEqual(valueAsString, cachedSelectedData))
        {
            return;
        }

        if (!isStartComplete)
        {
            cachedSelectedData = valueAsString;
            return;
        }

        cachedSelectedData = valueAsString;

        bool isDataEmpty = (valueAsString == null);

        if ((SendOnLookupDataUpdated.Empty && isDataEmpty) || (SendOnLookupDataUpdated.NotEmpty && !isDataEmpty))
        {
            SendRequest();
        }
    }
    #endregion

    public bool DisableUIInputDuringRequest = true;
}
