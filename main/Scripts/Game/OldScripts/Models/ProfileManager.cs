public class ProfileManager
{	
	public enum eCurrencyType
	{
		Hard = 0,
		Gold = 1,
		Social = 2,
	}

	public event System.Action<eCurrencyType, int> onCurrencyChange;

	private static ProfileManager _instance;
	public static ProfileManager Instance 
	{ 
		get 
		{ 
			if (_instance == null)
			{
				_instance = new  ProfileManager();
			}
			return _instance; 
		} 
	}
    
	public static ProfileManager Initialize()
	{
		return Instance;
	}

	private int _hardCurrency;
	public int HardCurrency
	{
		get
		{
			return _hardCurrency;
		}
		set
		{
			_hardCurrency = value;
			if (onCurrencyChange != null)
			{
				onCurrencyChange(eCurrencyType.Hard, _hardCurrency);
			}
		}
	}

	private int _socialCurrency;
	public int SocialCurrency
	{
		get
		{
			return _socialCurrency;
		}
		set
		{
			_socialCurrency = value;
			if (onCurrencyChange != null)
			{
				onCurrencyChange(eCurrencyType.Social, _socialCurrency);
			}
		}
	}
    
    public void OnHardCurrencyChange(int newHardCurrency)
    {
        HardCurrency = newHardCurrency;
    }

    public void OnSocialCurrencyChange(int newSocialCurrency)
    {
        SocialCurrency = newSocialCurrency;
    }
    
}
