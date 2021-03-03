using UnityEngine;
using System.Text.RegularExpressions;

public class UILabelDataLookup : DataLookup
{
	public bool ShowEmptyIfDataMissingInTextTemplateMode = true;
	public bool UseNumFormat = false;

	UILabel _cacheLabel = null;
	protected UILabel UILabel
	{
		get
		{
			if (_cacheLabel == null)
			{
				_cacheLabel = GetComponent<UILabel>();
				return _cacheLabel;
			}
			else
				return _cacheLabel;
		}
	}

	protected string LabelText
	{
		get { return UILabel.text; }
		set { UILabel.text = value; }
	}

	public int SubNum()
	{
		int current;
		bool enable= int.TryParse(LabelText,out current);
		if (enable)
		{
			if (current >= 1)
			{
				current = current - 1;
			}
		}
		else
		{
			current = 0;
		}
		LabelText = current.ToString();
		return current;
	}

	public int AddNum()
	{
		int current;
		bool enable = int.TryParse(LabelText, out current);
		if (enable)
		{
			current = current + 1;
		}
		else
		{
			current = 1;
		}
		LabelText = current.ToString();
		return current;
	}

	protected bool IsVigorLookup;
	public override void Awake()
	{
		base.Awake();

		UILabel.supportEncoding = true;
		if (DataIDList.Contains("res.vigor.v"))
		{
			IsVigorLookup = true;
			DataIDList.Add("res.vigor.max");
		}
	}

	public override void OnLookupUpdate(string dataID, object value)
	{
		base.OnLookupUpdate(dataID, value);

		SeedLabel();
	}

	protected virtual void SeedLabel()
	{
		if (string.IsNullOrEmpty(TextTemplate))
		{
			LabelText = ApplyNumFormat(GetDefaultLookupData<string>());
		}
		else
		{
			bool hasDataNotFound = false;
			
			MatchEvaluator matchEval = delegate(Match match)
			{
				// replaces "{index}" by the actual lookup data
				int dataIDIndex = System.Convert.ToInt32(match.Value.Trim('{', '}'));

				if(dataIDIndex >= 0 && dataIDIndex < RegisteredDataIDList.Count)
				{
					string dataID = RegisteredDataIDList[dataIDIndex];
					
					string value;
					bool isDataFound = DataLookupsCache.Instance.SearchDataByID<string>(dataID, out value);

					if(!isDataFound)
						hasDataNotFound = true;

					if (dataID.Equals("res.vigor.v") && isDataFound)
						ApplyVigorColor(ref value);
					return value;
				}
				else
				{
					hasDataNotFound = true;
					return "";
				}
			};

			Regex regex = new Regex("{[^{}]+}"); // matches "{index}", ex : "{22}"

			string processedString = string.Empty;
			if (TextTemplate.StartsWith(EB.Symbols.LocIdPrefix))
			{
				processedString = EB.Localizer.Format(TextTemplate, GetDefaultLookupData<object>());
			}
			else
			{
				if (IsVigorLookup)
				{
					int vigorMaxValue = GetDataLookupValue("res.vigor.max");
					string tempTextTemplate = "{0}/" + vigorMaxValue;
					processedString = regex.Replace(tempTextTemplate, matchEval);
				}
				else
					processedString = regex.Replace(TextTemplate, matchEval);
			}

			LabelText = !(hasDataNotFound && ShowEmptyIfDataMissingInTextTemplateMode) ? ApplyNumFormat(processedString) : "";
		}
	}

	protected string ApplyNumFormat(string data)
	{
		if (UseNumFormat && !string.IsNullOrEmpty(data))
		{
			int num = int.Parse(data);
            
            if (num>1000000000)
            {
                string str = string.Format("{0}.{1}G", num / 1000000000, ((num % 1000000000) / 100000000));
                return str;
            }
            else if (num > 1000000)
            {
                string str = string.Format("{0}.{1}M", num / 1000000, ((num % 1000000) / 100000));
                return str;
            }
            else if (num >= 1000)
            {
                string str = string.Format("{0}.{1}K",num/1000, ((num % 1000) / 100));
                return str;
            }
			else
			{
				return num.ToString();
			}
		}

		return data;
	}

	protected void ApplyVigorColor(ref string value)
	{
		int vigorMaxValue = GetDataLookupValue("res.vigor.max");
		int vigorValue = int.Parse(value);
		if (vigorValue > vigorMaxValue)
			value = GameColorValue.GetFormatColorStr(GameColorValue.Green_Str_LT, value);
		else if (vigorValue < 10)
			value = GameColorValue.GetFormatColorStr(GameColorValue.Red_Str_LT, value);
		else
			value = GameColorValue.GetFormatColorStr(GameColorValue.Write_Str, value);
	}

	protected int GetDataLookupValue(string path)
	{
		int value = 0;
		if (!DataLookupsCache.Instance.SearchIntByID(path, out value))
			EB.Debug.LogError("{0} value get fail", path);
		return value;
	}

	/// <summary>
	/// If not NULL, Label will use this template to format the text. Use {dataID} to represent dynamic values.
	/// Ex. : Gold : {player.gold} - Points : {player.points}
	/// </summary>
	[Multiline]
	public string TextTemplate;
}