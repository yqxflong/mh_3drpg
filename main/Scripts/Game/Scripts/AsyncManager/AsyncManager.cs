using System.Collections;
public class AsyncManager
{
	EB.Sparx.EndPoint m_Api;

	public AsyncManager(EB.Sparx.EndPoint api )		
	{
		m_Api = api;
	}

	bool LoadFromLocalFile(string subsystemName)
	{
		return false;
	}

	bool SaveToLocalFile(string subsystemName)
	{
		return false;
	}

	public string GetLocalVersionId(string subsystemName)
	{
		return string.Empty;
	}

	public void RequestRemoteVersionId(string subsystemName, System.Action<string, string> callback)
	{

	}

	[System.Obsolete("仅Editor调用")]
	public void RequestRemoteData(string subsystemName, System.Action<string, object> callback)
	{
		EB.Sparx.Request request = m_Api.Get("/" + subsystemName + "/async");

		m_Api.Service( request, delegate( EB.Sparx.Response response ) {
			if( response.sucessful == true )
			{
				if(callback != null)
				{
					callback( null, response.result);
				}
			}
			else
			{
				if(callback != null)
				{
					callback( response.localizedError, null );
				}
			}
		});
	}
}
