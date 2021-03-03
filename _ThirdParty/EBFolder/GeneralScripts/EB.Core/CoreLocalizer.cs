using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EB.Sparx;
using Unity.Standard.ScriptsWarp;

namespace EB
{
	public static class Symbols
	{
		public const string Infinity = "\u221E";
		public const string Degrees = "\u00B0";	
		public const string LocIdPrefix = "ID_";
		
		public static Dictionary<string, Language> LanguageCode = new Dictionary<string, Language>
		{
			{"en", Language.English },
			{"fr", Language.French },
			{"it", Language.Italian },
			{"de", Language.German },
			{"es", Language.Spanish },
			{"pt", Language.Portuguese },
			{"ru", Language.Russian },
			{"ko", Language.Korean },
			{"zh-Hans", Language.ChineseSimplified },
			{"zh-Hant", Language.ChineseTraditional },
			{"ja", Language.Japanese },
			{"tr", Language.Turkish },
		};

        public static Dictionary<Language, string> LanguageList = new Dictionary<Language,string>
        {
            { Language.English,"EN" },
            { Language.French ,"FR"},
            { Language.Italian ,"IT"},
            { Language.German,"GE" },
            { Language.Spanish,"SP" },
            { Language.Portuguese ,"PO"},
            { Language.Russian,"RU" },
            { Language.Korean,"KO" },
            { Language.ChineseSimplified,"CN" },
            { Language.ChineseTraditional,"TW" },
            { Language.Japanese,"JP" },
            { Language.Turkish,"TU" },
        };

        public static Dictionary<string, Country> CountryCode = new Dictionary<string, Country>
		{
			{"AR", Country.Argentina },
			{"BO", Country.Bolivia },
			{"BR", Country.Brazil },
			{"CA", Country.Canada },
			{"CL", Country.Chile },
			{"CN", Country.China },
			{"CO", Country.Colombia },
			{"CR", Country.CostaRica},
			{"DO", Country.DominicanRepublic },
			{"EC", Country.Ecuador },
			{"SV", Country.ElSalvador },
			{"FR", Country.France },
			{"DE", Country.Germany },
			{"GT", Country.Guatemala },
			{"HN", Country.Honduras },
			{"HK", Country.HongKong },
			{"IT", Country.Italy },
			{"JP", Country.Japan },
			{"KR", Country.Korea },
			{"MX", Country.Mexico },
			{"NI", Country.Nicaragua },
			{"PA", Country.Panama },
			{"PY", Country.Paraguay },
			{"PT", Country.Portugal },
			{"PE", Country.Peru },
			{"RU", Country.Russia },
			{"ES", Country.Spain },
			{"TR", Country.Turkey },
			{"TW", Country.Taiwan },
			{"UY", Country.Uruguay },
			{"US", Country.USA },
			{"VE", Country.Venezuela }
		};
	}

	public enum Language
	{
		Unknown,
		
		English,
		French,
		Italian,
		German,
		Spanish,
		Portuguese,
		Russian,
		Korean,
		ChineseSimplified,
		ChineseTraditional,
		Japanese,
		Turkish,
	}
	
	public enum Country
	{
		Unknown,
		
		Argentina,
		Bolivia,
		Brazil,
		Canada,
		Chile,
		China,
		Colombia,
		CostaRica,
		DominicanRepublic,
		Ecuador,
		ElSalvador,
		France,
		Germany,
		Guatemala,
		Honduras,
		HongKong,
		Italy,
		Japan,
		Korea,
		Mexico,
		Nicaragua,
		Panama,
		Paraguay,
		Peru,
		Portugal,
		Russia,
		Spain,
		Taiwan,
		Turkey,
		Uruguay,
		USA,
		Venezuela
	}
	
	public enum LocStatus
	{
		Missing,
		Source,
		Placeholder,
		For_Translation,
		Translated,
	}
	
	public class LocFile
	{	
		Dictionary<string,string> _strings;
		Dictionary<string,LocStatus> _status;
		
		public LocFile()
		{
			_strings = new Dictionary<string,string>();
			_status = new Dictionary<string, LocStatus>();
		}
		
		public LocFile( Dictionary<string,string> values )
		{
			_strings = values;
			_status = new Dictionary<string, LocStatus>();
		}
		
		public LocFile( Dictionary<string,string> values, Dictionary<string,LocStatus> status )
		{
			_strings = values;
			_status = status;
		}
		
		public string NextId( string prefix )
		{
			int i = 1;
			while(true)
			{
				var id = string.Format("ID_{0}_{1}", prefix, i).ToUpper();
				if (_strings.ContainsKey(id)==false)
				{
					return id;
				}
				++i;
			}
		}
		
		public void Add( string id, string value )
		{
			_strings[id] = value;
		}	
		
		public LocStatus GetStatus( string id )
		{
			LocStatus s;
			if (_status.TryGetValue(id, out s))
			{
				return s;
			}
			
			return LocStatus.Missing;
		}
		
		public bool Get( string id, out string result )
		{
			if (!_strings.TryGetValue(id, out result))
			{
				result = string.Empty;
				return false;
			}
			return true;
		}
		
#if UNITY_EDITOR
		public string Write()
		{
			List<string> ids = new List<string>(_strings.Keys);
			ids.Sort();
			
			var sb = new System.Text.StringBuilder();
			foreach( var id in ids ) 
			{
				sb.AppendLine( id+","+ _strings[id].Replace("\n", "\\n"));
			}
			
			return sb.ToString();
		}
		
		public string WriteTSV()
		{
			List<string> ids = new List<string>(_strings.Keys);
			ids.Sort();
			
			var sb = new System.Text.StringBuilder();
			foreach( var id in ids ) 
			{
				sb.AppendLine( id+"\t"+ _strings[id].Replace("\n", "\\n").Replace("\t",""));
			}
			
			return sb.ToString();
		}
#endif
		
		public Hashtable ToRead( string data )
		{
			Hashtable result = Johny.HashtablePool.Claim();
			// convert to unix line feeds
			data = data.Replace("\r\n", "\n");

			string[] lines = data.Split('\n');
			foreach (string line in lines)
			{
				if (line.Length > 0 && line[0] == '#')
				{
					continue;
				}
				
				int comma = line.IndexOf(',');
				if (comma > 0)
				{
					string id = line.Substring(0, comma);
					string value = line.Substring(comma + 1);
	
					if ( id.StartsWith(EB.Symbols.LocIdPrefix ))
					{
						var idParts = id.Split('|');
						id = idParts[0];
						
						if (idParts.Length>1)
						{
							_status[id] = (LocStatus)System.Enum.Parse(typeof(LocStatus), idParts[1], true);
						}
						else
						{
							_status[id] = LocStatus.Source;
						}
						
						// convert line feeds
						value = value.Replace("\\n", "\n").Replace("\\t","\t");
	
						Add(id, value);
						
						result[id] = value;
					}
				}
			}
			return result;
		}

		public Hashtable Read(string data)
		{
			//Hashtable result = Johny.HashtablePool.Claim();
			// convert to unix line feeds
			//data = data.Replace("\r\n", "\n");
			List<StringView> array = null;
			List<StringView> idParts = null;

			using (ZString.Block())
			{
				ZString zStr = data;
				ZString str = zStr.Replace("\r\n", "\n");

				StringView view = new StringView(str, 0, str.Length);								
				array = view.Split2List('\n');
			}

			foreach (StringView line in array)
			{
				if (line.Length > 0 && line[0] == '#')
				{
					continue;
				}

				//StringView line = v.SubString(0, v.Length-1);
				int comma = line.IndexOf(',');
				if (comma > 0)
				{
					StringView id = line.Substring(0, comma);					

					if (id.StartsWith(EB.Symbols.LocIdPrefix))
					{
						idParts = id.Split2List('|');
						id = idParts[0];
						string idText = id.ToString();

						if (idParts.Count > 1)
						{
							StringView type = idParts[1];
							if(type.Equals("SOURCE") || type.Equals("Source"))
							{
								_status[idText] = LocStatus.Source;
							}
							else if(type.Equals("PLACEHOLDER") || type.Equals("Placeholder"))
							{
								_status[idText] = LocStatus.Placeholder;
							}
							else if(type.Equals("For_Translation") || type.Equals("FOR_TRANSLATION"))
							{
								_status[idText] = LocStatus.For_Translation;
							}
							else if (type.Equals("Translated") || type.Equals("TRANSLATED"))
							{
								_status[idText] = LocStatus.Translated;
							}
							else
							{
								_status[idText] = LocStatus.Missing;
							}
						}
						else
						{
							_status[idText] = LocStatus.Source;
						}

						string value = line.Substring(comma + 1).ToString();
						// convert line feeds
						value = value.Replace("\\n", "\n").Replace("\\t", "\t");

						Add(idText, value);
						//result[id] = value;
					}
				}
			}
			array = null;
			idParts = null;

			return null;
		}
	}
	
    /// <summary>
    /// ��������ص�����
    /// </summary>
	public static class Localizer
	{
		class FormatProvider : System.IFormatProvider, System.ICustomFormatter	
		{
			#region IFormatProvider implementation
			public object GetFormat (System.Type formatType)
			{
				return this;
			}
			#endregion
			
			#region ICustomFormatter implementation
			public string Format (string format, object arg, System.IFormatProvider formatProvider)
			{
				var result = string.Empty;
				if ( arg != null )
				{
					result = arg.ToString();
				}
				else
				{
					result = "null";
				}			
				
				if ( result.StartsWith(Symbols.LocIdPrefix) )
				{
					result = GetString(result);
				}
				return result;
			}
			#endregion
		}

		public static bool ShowStringIds 	= false;
		
		private static Dictionary<string, string> _strings = new Dictionary<string, string>();
		public static Dictionary<string, LocStatus> _status = new Dictionary<string, LocStatus>();
		public static Dictionary<string, string> strings { get { return _strings; } }

		private static FormatProvider _provider = new FormatProvider();
		
		public static Language Current { get; private set; }
		
		public static void Clear()
		{
			lock(_status)
			{
				_status.Clear();
			}

			lock(_strings)
			{
				_strings.Clear();
			}
		}
		
		///加载当前语言的基础包
		public static void LoadCurrentLanguageBase(Language locale)
		{
            var l = GetSparxLanguageCode(locale);
            Current = locale;
			EB.Assets.LoadAllAsync($"Languages/{l}", typeof(TextAsset), objs =>
            {
				foreach (TextAsset asset in objs)
				{
					EB.Debug.Log("LoadAllFromResources: Languages/{0}", asset.name);
					var text = Encoding.GetString(asset.bytes);
					LoadStringsInternal(text);
				}
			});
        }

		public static void Dump()
		{
			lock (_strings)
			{
				foreach (var key in _strings.Keys)
				{
					EB.Debug.Log("String (" + key + ") " + Encoding.ToHexString(Encoding.GetBytes(key)) + " " + _strings[key]);
				}
			}
		}
		
		public static string GetLanguageCode( Language locale )
		{
			foreach( KeyValuePair<string, Language> lcode in Symbols.LanguageCode )
			{
				if(lcode.Value == locale)
					return lcode.Key;
			}
			
			return locale.ToString().ToLower();
		}
		
		public static string GetSparxLanguageCode( Language locale )
		{
			switch (locale)
			{
				case Language.English:
					return "en";
				case Language.ChineseSimplified:
					return "zh-CN";
				case Language.ChineseTraditional:
					return "zh-TW";
                case Language.French:
                    return "fr";
                case Language.German:
                    return "ge";
            }

			foreach( KeyValuePair<string, Language> lcode in Symbols.LanguageCode)
			{
				if(lcode.Value == locale)
					return lcode.Key;
			}
			
			return locale.ToString().ToLower();
		}

        public static string GetNameSparxLanguageCode(Language locale)
        {
            switch (locale)
            {
                case Language.ChineseSimplified:
                    return "zh-CN";
                case Language.ChineseTraditional:
                    return "zh-TW";
                default:
                    return Symbols.LanguageList[Language.English].ToLower();

            }
        }


        public static string GetCultureCode(Language locale)
		{
			// old style
			// see: https://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx
			switch (locale)
			{
				case Language.ChineseSimplified:
				case Language.ChineseTraditional:
					return GetSparxLanguageCode(locale);
			}

			// see: http://www.ruanyifeng.com/blog/2008/02/codes_for_language_names.html
			return GetLanguageCode(locale) + "-" + GetCountryCode(GetDefaultCountry(locale));
		}

		public static Language GetSystemDefaultLanguage(Language defaultLocale)
		{
			Language locale = defaultLocale;

			switch (Application.systemLanguage)
            {
                case SystemLanguage.ChineseTraditional:
                    //locale = EB.Language.ChineseTraditional;
                    //break;
                case SystemLanguage.ChineseSimplified:
				case SystemLanguage.Chinese:
					locale = EB.Language.ChineseSimplified;
					break;
				case SystemLanguage.English:
					locale = EB.Language.English;
					break;
                case SystemLanguage.French:
                    locale = EB.Language.French;
                    break;
                case SystemLanguage.German:
                    locale = EB.Language.German;
                    break;
            }

			return locale;
		}

		public static Country GetDefaultCountry(Language locale)
		{
			Country c = Country.Unknown;
			switch (locale)
			{
				case Language.ChineseSimplified:
					c = Country.China;
					break;
				case Language.ChineseTraditional:
					c = Country.Taiwan;
					break;
				case Language.English:
					c = Country.USA;
					break;
				case Language.German:
					c = Country.Germany;
					break;
				case Language.Italian:
					c = Country.Italy;
					break;
				case Language.Japanese:
					c = Country.Japan;
					break;
				case Language.Russian:
					c = Country.Russia;
					break;
				case Language.Korean:
					c = Country.Korea;
					break;
				case Language.French:
					c = Country.France;
					break;
				case Language.Portuguese:
					c = Country.Portugal;
					break;
				case Language.Spanish:
					c = Country.Spain;
					break;
				case Language.Turkish:
					c = Country.Turkey;
					break;
			}

			return c;
		}

		public static string GetCountryCode(Country country)
		{
			foreach (var ccode in Symbols.CountryCode)
			{
				if (ccode.Value == country)
				{
					return ccode.Key;
				}
			}

			return country.ToString().ToUpper();
		}
		
		public static void LoadStrings( Language locale, string database)
		{
			Current = locale;
			var l = GetSparxLanguageCode(locale);
			var path = "Languages/"+l+"/"+database;

			Assets.LoadAsync(path, typeof(TextAsset), o =>
			{
				TextAsset asset = o as TextAsset;
				if (asset != null)
				{
					var text = Encoding.GetString(asset.bytes);
					LoadStringsInternal(text);
					Assets.Unload(asset);
				}
				else
				{
					EB.Debug.LogWarning("Failed to load local: " + locale + " for database " + database);
				}
			});
		}

		public static void LoadStrings(TextAsset asset)
		{
			try
			{
				var text = Encoding.GetString(asset.bytes);
				LoadStringsInternal(text);
                EB.Debug.Log("After load local: {0} for asset: {1}", Current, asset.name);
			}
			catch(System.NullReferenceException e)
			{
				EB.Debug.LogError("Failed to load local: " + Current + " for asset " + asset.name);
			}
		}
		
		public static bool HasString(string id)
		{
			lock(_strings)
			{
				return _strings.ContainsKey(id);
			}
		}

		public static bool GetString(string id, out string value)
		{
			if (string.IsNullOrEmpty(id))
			{
				Debug.LogError("empty or null string id");
				value = "MS_NULL";
				return false;
			}

			if (!id.StartsWith(Symbols.LocIdPrefix))
			{
				value = id;
				return true;
			}

			if (ShowStringIds)
			{
				value = id;
				return true;
			}

			LocStatus status = LocStatus.Missing;

			// notice: lock order is important
			lock (_strings)
				lock (_status)
				{
					if (_strings.TryGetValue(id, out value) && _status.TryGetValue(id, out status) && (status == LocStatus.Source || status == LocStatus.Translated))
					{
						return true;
					}
				}

			EB.Debug.LogWarning("Missing String (" + id + ") ");
			value = "MS_" + id;
			return false;
		}
	
        public static bool GetTexOrAtlasName(string id,out string value)
        {
            if (string.IsNullOrEmpty(id))
            {
               EB.Debug.Log("empty or null string id");
                value =id;
                return false;
            }

            string []split= id.Split('_');
            //split[split.Length - 1];
            if (Symbols.LanguageList.ContainsValue(split[split.Length - 1]))
            {
                string newValue = null;
                Language Lang = Language.ChineseSimplified;
                if (Hub.Instance != null) Lang = Hub.Instance.Config.Locale;
                split[split.Length - 1] = Symbols.LanguageList[Lang];
                newValue = split[0];
                for (int i=1; i< split.Length; i++)
                {
                    newValue = string.Format("{0}_{1}", newValue, split[i]);
                }
                value = newValue;
            }
            else
                value = id;
            return true;
        }
        
        public static string GetTableString(string id,string def)
        {
#if USE_LANGUAGE_SYSTEM || USE_LANGUAGE_TW || USE_LANGUAGE_EN
            string value = string.Empty;
            GetString(id, out value);
            return value;//(string.IsNullOrEmpty(value)) ? def : value;
#else
            return def;
#endif
        }
        
        public static string GetDiscountChange(float discountValue)
        {
            string str=string .Empty;
            Language Lang = Language.ChineseSimplified;
            if (Hub.Instance != null) Lang = Hub.Instance.Config.Locale;
            if (Lang == Language.ChineseSimplified || Lang == Language.ChineseTraditional)
            {
                str = (discountValue* 10f).ToString();
            }
            else
            {
                str = ((1f - discountValue) * 100f).ToString(); ;
            }
            return str;
        }

		public static string GetString(string id)
		{
			string value;
			GetString(id, out value);
			return value;
		}

		public static bool TryFetchString(string id, out string value)
		{
			return GetString(id, out value);
		}
		
		public static LocStatus GetStatus( string id )
		{
			LocStatus s;
			lock(_status)
			{
				if (_status.TryGetValue(id, out s))
				{
					return s;
				}
			}
			return LocStatus.Missing;
		}
		
		public static UnityEngine.Color GetColor( string id )
		{
			var status = GetStatus(id);
			//Debug.LogError("GetColor: " + id + " " + status);
			switch(status)
			{
			case LocStatus.Translated:
				return Color.blue;
			case LocStatus.Placeholder:
				return Color.yellow;
			case LocStatus.For_Translation:
				return Color.yellow;
			case LocStatus.Missing:
				return Color.red;
			}
			return Color.white;
		}
	
		public static string Format(string id, params object[] args)
		{
			if (ShowStringIds)
			{
				return GetString(id);
			}

			try
			{
				return string.Format(_provider, GetString(id), args); 
			}
			catch(System.ArgumentNullException)
			{
#if UNITY_EDITOR
				Debug.Log("CoreLocalizer: Arguments can't be null. Check format and args for localiztion key: '" + id + "'");
#endif
				return id;
			}
			catch(System.FormatException)
			{
#if UNITY_EDITOR
				Debug.Log("CoreLocalizer: Text format is invalid for localization key: '" + id + "'");
#endif
				return id;
			}
			catch(System.Exception e)
			{
				Debug.LogWarning("CoreLocalizer: Exception[" + e.ToString() + "] triggered!");
				return id;
			}
		}

		private static Hashtable LoadStringsInternal(string data)
		{
			// notice: lock order is important
			lock (_strings)
				lock (_status)
				{
					var file = new LocFile(_strings, _status);
//#if UNITY_EDITOR || UNITY_IOS
					return file.Read(data);
//#else
//					return file.ToRead(data);
//#endif
				}
		}
	
		public static string FormatTimeHundredthsSeconds(int totalHundredthsSeconds)
		{
			int minutes, seconds, hundredths;
			hundredths = totalHundredthsSeconds;
			seconds = Mathf.FloorToInt(hundredths/100);
			hundredths -= hundredths * 100;
			minutes = Mathf.FloorToInt(seconds/60);
			
			string timeString = (minutes < 10 && minutes >=0 ? "0" : "") + (minutes > 0 ? minutes.ToString() + "+" : "") 
					+ ((seconds < 10 && seconds >=0 && minutes > 0) ? "0" : "") + seconds.ToString() 
					+ ":" + (totalHundredthsSeconds % 100 < 10 ? "0" :"" + totalHundredthsSeconds %100).ToString();
			return timeString;
		}
		
		public static string FormatTime(int totalSeconds, bool showSeconds)
		{
			int minutes, hours, seconds;
			
			seconds = totalSeconds;
			hours = Mathf.FloorToInt(seconds / 3600);
			seconds -= hours * 3600;
			minutes = Mathf.FloorToInt(seconds / 60);
						
			string timeString = (hours > 0 ? hours.ToString() + ":" : "") + ((minutes < 10 && minutes>=0 && hours > 0) ? "0" : "") + minutes.ToString();
			if(showSeconds)
			{
				timeString += ":" + ((totalSeconds % 60 < 10) ? "0" : "") + (totalSeconds % 60).ToString();
			}
			return timeString;
		}
		
		private static System.Globalization.CultureInfo _culture = null;

		public static System.Globalization.CultureInfo Culture
		{
			get { return _culture = _culture ?? new System.Globalization.CultureInfo(GetCultureCode(Current)); }
		}

        public static object GM { get; private set; }

        public static string FormatNumber( float value, bool removeDecimal )
		{
			if (_culture == null)
			{
				_culture = new System.Globalization.CultureInfo(GetCultureCode(Current));
			}

			string str = string.Format(_culture, "{0:N}", value);
			if (removeDecimal)
			{
				int dec = str.IndexOf('.');
				if ( dec >= 0 ) return str.Substring(0, dec);
			}
			return str;
		}

		public static string FormatNumber(long value)
		{
			if (_culture == null)
			{
				_culture = new System.Globalization.CultureInfo(GetCultureCode(Current));
			}

			string separator = _culture.NumberFormat.NumberGroupSeparator;
			_culture.NumberFormat.NumberGroupSeparator = " ";
			string str = string.Format(_culture, "{0:N0}", value);
			_culture.NumberFormat.NumberGroupSeparator = separator;
			return str;
		}
		
		const int SecondsInMin = 60;
		const int SecondsInHour = SecondsInMin * 60;
		const int SecondsInDay = SecondsInHour * 24;
		public static string FormatDuration(int seconds, bool showSeconds)
		{
			string duration = "";

			showSeconds = showSeconds || (seconds < SecondsInMin);

			if (seconds >= SecondsInDay)
			{
				int days = seconds / SecondsInDay;

				duration += Format(days > 1 ? "ID_SPARX_DURATION_FORMAT_DAYS" : "ID_SPARX_DURATION_FORMAT_DAY", days);

				seconds = seconds % SecondsInDay;
			}
			
			if (seconds >= SecondsInHour)
			{
				int hours = seconds / SecondsInHour;

				duration += Format(hours > 1 ? "ID_SPARX_DURATION_FORMAT_HOURS" : "ID_SPARX_DURATION_FORMAT_HOUR", hours);

				seconds = seconds % SecondsInHour;
			}

			if (seconds >= SecondsInMin)
			{
				int mins = seconds / SecondsInMin;

				duration += Format(mins > 1 ? "ID_SPARX_DURATION_FORMAT_MINUTES" : "ID_SPARX_DURATION_FORMAT_MINUTE", mins);

				seconds = seconds % SecondsInMin;
			}

			if (seconds > 0 && showSeconds)
			{
				duration += Format(seconds > 1 ? "ID_SPARX_DURATION_FORMAT_SECONDS" : "ID_SPARX_DURATION_FORMAT_SECOND", seconds);
			}

			return duration.Trim();
		}

		public static string FormatPassedDuration(double timestamp)
		{
			System.DateTime baseTime = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
			System.TimeSpan ts = System.DateTime.UtcNow - baseTime;
			double delta = ts.TotalSeconds - timestamp;
			if (delta < 0)
			{
				delta = 0;
			}

			System.TimeSpan tsDelta = System.TimeSpan.FromSeconds(delta);
			if (tsDelta.TotalDays >= 60)
			{
				return baseTime.AddSeconds(timestamp).ToLocalTime().ToShortDateString();
			}
			else if (tsDelta.TotalDays >= 30)
			{
				int monthes = (int)(tsDelta.TotalDays / 30);
				return Format(monthes > 1 ? "ID_SPARX_PASSED_TIME_FORMAT_MONTHES" : "ID_SPARX_PASSED_TIME_FORMAT_MONTH", monthes);
			}
			else if (tsDelta.TotalDays >= 7)
			{
				int weeks = (int)(tsDelta.TotalDays / 7);
				return Format(weeks > 1 ? "ID_SPARX_PASSED_TIME_FORMAT_WEEKS" : "ID_SPARX_PASSED_TIME_FORMAT_WEEK", weeks);
			}
			else if (tsDelta.TotalDays >= 1)
			{
				int days = (int)tsDelta.TotalDays;
				return Format(days > 1 ? "ID_SPARX_PASSED_TIME_FORMAT_DAYS" : "ID_SPARX_PASSED_TIME_FORMAT_DAY", days);
			}
			else if (tsDelta.TotalHours >= 1)
			{
				int hours = (int)tsDelta.TotalHours;
				return Format(hours > 1 ? "ID_SPARX_PASSED_TIME_FORMAT_HOURS" : "ID_SPARX_PASSED_TIME_FORMAT_HOUR", hours);
			}
			else if (tsDelta.TotalMinutes >= 1)
			{
				int minutes = (int)tsDelta.TotalMinutes;
				return Format(minutes > 1 ? "ID_SPARX_PASSED_TIME_FORMAT_MINUTES" : "ID_SPARX_PASSED_TIME_FORMAT_MINUTE", minutes);
			}
			else
			{
				int seconds = (int)System.Math.Max(tsDelta.TotalSeconds, 1);
				return Format(seconds > 1 ? "ID_SPARX_PASSED_TIME_FORMAT_SECONDS" : "ID_SPARX_PASSED_TIME_FORMAT_SECOND", seconds);
			}
		}

		public static string ToTFS(this long value)
		{
			return FormatNumber(value);
		}

		public static string ToTFS(this int value)
		{
			return FormatNumber(value);
		}
	}	
}
	
