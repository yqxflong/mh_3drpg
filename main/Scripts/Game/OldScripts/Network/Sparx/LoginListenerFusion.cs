public class LoginListenerFusion :  EB.Sparx.LoginConfigListener
{
	public event System.Action LoginEvent;
	public event System.Action<string> LoginFailedEvent;
	public event System.Action<string> LogoffEvent;
    public event System.Action<object> ResolveMHLoginEvent;
	public event System.Action<string> UpdateEvent;
    

    public void OnMHLogin(object param)
    {
        if (ResolveMHLoginEvent != null)
        {
            ResolveMHLoginEvent(param);
        }
    }
    
	public void OnLoggedIn()
	{
        if (LoginEvent != null)
        {
            LoginEvent();
        }
    }
    
    public void OnLoginFailed( string error )
	{
        if (LoginFailedEvent != null)
        {
            LoginFailedEvent(error);
        }
    }

	public void OnDisconnected(string error)
	{
        if (LogoffEvent != null)
        {
            LogoffEvent(error);
        }
    }

	public void OnUpdateRequired(string storeUrl)
	{
        if (UpdateEvent != null)
        {
            UpdateEvent(storeUrl);
        }
    }
}
