using System;
using EB.Sparx;

namespace Hotfix_LT.UI
{
    public class UIServerRequestHotFix:DynamicMonoHotfix
    {
        public Action<Response> response;
        public override void OnFetchData(Response result, int reqInstanceID)
        {
            base.OnFetchData(result, reqInstanceID);
            if (response!=null)
            {
                response(result);
            }
        }
    }
}