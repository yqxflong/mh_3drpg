using UnityEngine;
using System.Collections.Generic;
using EB;

namespace Hotfix_LT.UI
{
	public abstract class NameFile
	{
		public virtual string RandomName(bool isfemale)
		{
			return string.Empty;
		}

		public virtual void Read(string data)
		{

		}

		public virtual void Clear()
		{

		}
	}

	public class ZH_CN_NameFile : NameFile
	{
		public class NameEntry
		{
			public string Prefix;
			public string FirstName;
			public string LastName;
			public string Postfix;
			public NameEntry(string prefix, string firstName, string lastName, string postfix)
			{
				this.Prefix = prefix;
				this.FirstName = firstName;
				this.LastName = lastName;
				this.Postfix = postfix;
			}
		}

		public List<NameEntry> NameArray;
		public int Count;

		public ZH_CN_NameFile()
		{
			NameArray = new List<NameEntry>(3000);
			Count = 0;
		}

		public override void Read(string data)
		{
			int firstline = 0;
			data = data.Replace("\r\n", "\n");
			string[] lines = data.Split('\n');
			for (var i = 0; i < lines.Length; i++)
			{
				string line = lines[i];
				if (line.Length == 0) continue;
				if (firstline == 0)
				{
					firstline = 1;
					continue;
				}
				string[] tokens = line.Split(',');
				if (tokens.Length < 5) return;
				Count++;
				string prefix = tokens[1];
				string firstName = tokens[2];
				string lastName = tokens[3];
				string postfix = tokens[4];
				NameArray.Add(new NameEntry(prefix, firstName, lastName, postfix));
			}
		}

		/// <summary>
		/// 返回一个随机名字
		/// </summary>
		/// <returns></returns>
		public override string RandomName(bool isfemale)
		{
			string RandName = "";
			int type = Random.Range(0, 3);
			string partName = GetPartName();
			switch (type)
			{
				case 0:
					{//A+B+C
						int typeIndex = Random.Range(0, Count);
						RandName = string.Format("{0}{1}", NameArray[typeIndex].Prefix, partName);
					}
					break;
				case 1:
					{//B+C+D
						int typeIndex = Random.Range(0, Count);
						RandName = string.Format("{0}·{1}", partName, NameArray[typeIndex].Postfix);
					}
					break;
				default:
					{//B+C
						RandName = partName;
					}
					break;
			}

			return RandName;
		}

		public override void Clear()
		{
			if (NameArray != null)
			{
				NameArray.Clear();
			}
			Count = 0;
		}

		public string GetPartName()
		{
			int firstIndex = Random.Range(0, Count);
			int secondIndex = Random.Range(0, Count);
			string partName = string.Format("{0}{1}", NameArray[firstIndex].FirstName, NameArray[secondIndex].LastName);
			return partName;
		}

	}

	public class RandomNameFactory
	{
		public static RandomNameFactory m_Instance;

		private NameFile m_File;

		public static RandomNameFactory Instance
		{
			get
			{
				if (m_Instance == null)
				{
					m_Instance = new RandomNameFactory();
					m_Instance.LoadAllFromResources(global::UserData.Locale, false);
				}
				return m_Instance;
			}
		}

        public static void GuideRandom(System.Action<string> callback = null)
        {
            m_Instance = new RandomNameFactory();
            m_Instance.LoadAllFromResources(global::UserData.Locale, false, callback);
        }

		/// <summary>
		/// 初始化名字库
		/// </summary>
		public void LoadAllFromResources(EB.Language locale, bool loadCommon,System .Action<string> callback=null)
		{
			var language = Localizer.GetNameSparxLanguageCode(locale);
			string path = "Bundles/Name/RandomName_" + language;
			EB.Assets.LoadAsync(path, typeof(TextAsset), o=>{
				if(!o){
					return;
				}
				TextAsset nameAsset = o as TextAsset;
				var Text = Encoding.GetString(nameAsset.bytes);
				LoadStringsInternal(Text, locale);

                if (callback != null)
                {
                    callback(RandomName(false));
                }
			});
		}
        
		public void Release()
		{
			if (m_File != null)
			{
				m_File.Clear();
			}
		}

		private void LoadStringsInternal(string data, EB.Language locale)
		{
			switch (locale)
			{
				case EB.Language.ChineseSimplified:
				case EB.Language.ChineseTraditional:
					m_File = new ZH_CN_NameFile();
					break;
				default:
					m_File = new ZH_CN_NameFile();
					break;
			}
			m_File.Read(data);
		}

		/// <summary>
		/// 返回一个随机名字
		/// </summary>
		/// <returns></returns>
		public string RandomName(bool isfemale)
		{
			if (m_File == null) return string.Empty;
			return m_File.RandomName(isfemale);
		}
	}
}