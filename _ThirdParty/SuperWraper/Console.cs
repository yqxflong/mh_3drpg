using System.Collections.Generic;
using UnityEngine;

namespace Unity.Standard.ScriptsWarp
{
	public class Console
	{
		public static bool close;

		public static string msgTag = "<color=purple>";

		public static string infoTag = "<color=#00bf00>";

		public static string importantTag = "<color=white>";

		public static string warnTag = "<color=yellow>";

		public static string errorTag = "<color=red>";

		public static string actionTag = "<color=#8888ff>";

		public static string funcTag = "<color=#5858FA>";

		public static string processTag = "<color=#40FF00>";

		public static string ended = "</color>";

		/// branch msg 
		public static string MsgFormat(string content)
		{
			return msgTag + content + ended;
		}

		/// native info
		public static string InfoFormat(string content)
		{
			return infoTag + content + ended;
		}

		/// important signal
		public static string ImportantFormat(string content)
		{
			return importantTag + content + ended;
		}

		/// warn signal
		public static string WarnFormat(string content)
		{
			return warnTag + content + ended;
		}

		/// error signal
		public static string ErrorFormat(string content)
		{
			return errorTag + content + ended;
		}

		/// action declarer
		public static string ActionFormat(string content)
		{
			return actionTag + content + ended;
		}

		/// function declarer
		public static string FuncFormat(string content)
		{
			return funcTag + content + ended;
		}

		/// programing mark, and provide info
		public static string ProcessFormat(string content)
		{
			return processTag + content + ended;
		}

		public static void Printf(string content, params object[] args)
		{
			if (close) return;
			Debug.LogFormat(content, args);
		}

		public static void PrintInfo(string content, params object[] args)
		{
			if (close) return;
			Debug.LogFormat(infoTag + content + ended, args);
		}

		/// print important signal
		public static void PrintImportant(string content, params object[] args)
		{
			if (close) return;
			Debug.LogFormat(importantTag + content + ended, args);
		}

		/// print warn
		public static void PrintWarner(string content, params object[] args)
		{
			if (close) return;
			Debug.LogFormat(warnTag + content + ended, args);
		}

		public static void PrintfWarner(string content, params object[] args)
		{
			if (close) return;
			Debug.LogWarningFormat(warnTag + content + ended, args);
		}

		///  print error signal
		public static void PrintError(string content, params object[] args)
		{
			if (close) return;
			Debug.LogFormat(errorTag + content + ended, args);
		}

		public static void PrintfError(string content, params object[] args)
		{
			if (close) return;
			Debug.LogErrorFormat(errorTag + content + ended, args);
		}

		/// print action declarer
		public static void PrintAction(string content, params object[] args)
		{
			if (close) return;
			Debug.LogFormat(actionTag + content + ended, args);
		}

		/// print function declarer
		public static void PrintMethodCall(string content, params object[] args)
		{
			if (close) return;
			Debug.LogFormat(funcTag + content + ended, args);
		}

		/// print List<T>
		public static void PrintList<T>(List<T> list, System.Reflection.BindingFlags binding, bool isProperty = false)
		{
			if (list == null || list.Count <= 0) return;
			//Console.PrintInfo("List.Count = " + list.Count);
			System.Type type = typeof(T);
			string content = "List<" + type.ToString() + "> = { ";

			if (isProperty)
			{
				System.Reflection.PropertyInfo[] propertys = type.GetProperties(binding);

				list.ForEach(item =>
				{
					content += "[" + list.IndexOf(item) + "] : ";
					if (propertys != null && item != null)
					{
						//for (int i = 0; i < propertys.Length; i++)
						//{
						//	IPropertyAccessor accessor = FastReflectionCaches.PropertyAccessorCache.Get(propertys[i]);
						//	string key = propertys[i].Name;
						//	string value = accessor.GetValue(item).ToString();
						//	content += ("{" + key + "=" + value + "}, ");
						//}
					}
				});
				content += " }";
			}
			else
			{
				System.Reflection.FieldInfo[] fields = type.GetFields(binding);

				list.ForEach(item =>
				{
					content += "[" + list.IndexOf(item) + "] : ";
					if (fields != null && item != null)
					{
						for (int i = 0; i < fields.Length; i++)
						{
							string key = fields[i].Name;
							string value = fields[i].GetValue(item).ToString();
							content += ("{" + key + "=" + value + "}, ");
						}
					}
				});
				content += " }";
			}

			Debug.Log("<color=orange>" + content + ended);
		}
	}
}
