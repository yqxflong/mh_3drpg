
using System;
using System.Collections;
using System.Collections.Generic;

namespace EB
{
	public static class Dot
	{
		public static object DeepCopy( object source, object dest )
		{
			if ( source is Hashtable )
			{
				var ht = (Hashtable)source;
				var dht = (Hashtable)dest;
				foreach( DictionaryEntry entry in ht )
				{
					var value = entry.Value;
					if (dht.ContainsKey(entry.Key))
					{
						dht[entry.Key] = DeepCopy(value,dht[entry.Key]);
					}
					else
					{
						dht.Add(entry.Key,entry.Value);
					}
				}
			}
			else 
			{
				dest = source;
			}
			
			return dest;
		}

		#region Base (string.SubString的GC???)
		private static bool CanBeParsedAsNumber(object obj){
			return obj is int
					|| obj is long
					|| obj is float
					|| obj is double
					|| obj is bool;
		}

		private static bool SearchBase(string name, ref string next, ref object obj)
		{
			if (obj == null)
			{
				return false;
			}

			string variable = name;
			int dot     = name.IndexOf('.');
			if (dot != -1)
			{
				variable = name.Substring(0, dot);
				next = name.Substring(dot + 1);
			}

			int index = -1;
			int startBracket = variable.IndexOf('[');
			if (startBracket != -1)
			{
				var endBracket = variable.IndexOf(']', startBracket + 1);
				if (endBracket != -1)
				{
					var tmp = variable.Substring(startBracket + 1, endBracket - startBracket -1);
					if (!int.TryParse(tmp, out index))
					{
						index = -1;
						EB.Debug.LogError("invalid index: " + tmp);
						return false;
					}
					variable = variable.Substring(0, startBracket);
				}

				// OLD path!
				if (!string.IsNullOrEmpty(variable))
				{
					if (obj is Hashtable)
					{
						var ht = (Hashtable)obj;
						obj = ht[variable];
					}
                    else if (obj is IDictionary)
                    {
                        var dic = (IDictionary)obj;
                        obj = dic[variable];
                    }
                }

				if (index >= 0 && obj is ArrayList)
				{
					var list = (ArrayList)obj;
					if (index < list.Count)
					{
						obj = list[index];
					}
					else
					{
						obj = null;
					}
				}

            }
			if (!string.IsNullOrEmpty(variable))
			{
				if (obj is Hashtable)
				{
					var ht = (Hashtable)obj;
					obj = ht[variable];
				}
				else if (obj is ArrayList && int.TryParse(variable, out index))
				{
					var arr = (ArrayList)obj;
					if (index >= 0 && index < arr.Count)
					{
						obj = arr[index];
					}
				}
                else if(obj is IDictionary)
                {
                    var dic = (IDictionary)obj;
                    obj = dic[variable];
                }
			}

			return true;
		}
		#endregion

		#region Integer Query
		public static int Integer(string name, object obj, int defaultValue)
		{
			return FindInt(name, obj, defaultValue);
		}

		private static int FindInt(string name, object obj, int defaultValue)
		{
			if (string.IsNullOrEmpty(name))
			{
				if(CanBeParsedAsNumber(obj)){
					try
					{
						return Convert.ToInt32(obj);
					}
					catch (Exception e)
					{
					}
				}
				else if(obj is string){
					int n;
					int.TryParse(obj as string, out n);
					return n;
				}
				
				return defaultValue;
			}

			string next = string.Empty;
			if (!SearchBase(name, ref next, ref obj))
			{
				return defaultValue;
			}

			return FindInt(next, obj, defaultValue);
		}
		
		public static int[] Integers(object obj, string[] names, int[] defaultValue)
		{
			int[] ret = new int[defaultValue.Length];
			for(int i = 0; i < names.Length; i++){
				ret[i] = FindInt(names[i], obj, defaultValue[i]);
			}
			
			return ret;
		}
		#endregion
		
		#region Uint Query
		private static uint FindUint(string name, object obj, uint defaultValue)
		{
			if (string.IsNullOrEmpty(name))
			{
				if(CanBeParsedAsNumber(obj)){
					return Convert.ToUInt32(obj);
				}
				else if(obj is string){
					uint n;
					uint.TryParse(obj as string, out n);
					return n;
				}
				
				return defaultValue;
			}

			string next = string.Empty;
			if (!SearchBase(name, ref next, ref obj))
			{
				return defaultValue;
			}

			return FindUint(next, obj, defaultValue);
		}

		public static uint UInteger(string name, object obj, uint defaultValue ) 
		{
			return FindUint(name, obj, defaultValue);
		}
		#endregion
		
		#region long Query
		public static long Long( string name, object obj, long defaultValue ) 
		{
			return FindLong(name, obj, defaultValue);
		}

		private static long FindLong(string name, object obj, long defaultValue)
		{
			if (string.IsNullOrEmpty(name))
			{
				if(CanBeParsedAsNumber(obj)){
					return Convert.ToInt64(obj);
				}
				else if(obj is string){
					long n;
					long.TryParse(obj as string, out n);
					return n;
				}
				return defaultValue;
			}

			string next = string.Empty;
			if (!SearchBase(name, ref next, ref obj))
			{
				return defaultValue;
			}

			return FindLong(next, obj, defaultValue);
		}
		#endregion
		
		#region Single Query
		public static float Single( string name, object obj, float defaultValue ) 
		{
			return FindSingle(name, obj, defaultValue);
		}

		private static float FindSingle(string name, object obj, float defaultValue)
		{
			if (string.IsNullOrEmpty(name))
			{
				if(CanBeParsedAsNumber(obj)){
					return Convert.ToSingle(obj);
				}
				else if(obj is string){
					float n;
					float.TryParse(obj as string, out n);
					return n;
				}
				return defaultValue;
			}

			string next = string.Empty;
			if (!SearchBase(name, ref next, ref obj))
			{
				return defaultValue;
			}

			return FindSingle(next, obj, defaultValue);
		}
		#endregion

		#region Double Query
		public static double Double( string name, object obj, double defaultValue ) 
		{
			return FindDouble(name, obj, defaultValue);
		}		

		private static double FindDouble(string name, object obj, double defaultValue)
		{
			if (string.IsNullOrEmpty(name))
			{
				if(CanBeParsedAsNumber(obj)){
					return Convert.ToDouble(obj);
				}
				else if(obj is string){
					double n;
					double.TryParse(obj as string, out n);
					return n;
				}

				return defaultValue;
			}

			string next = string.Empty;
			if (!SearchBase(name, ref next, ref obj))
			{
				return defaultValue;
			}

			return FindDouble(next, obj, defaultValue);
		}	
		#endregion

		#region Bool Query
		public static bool Bool( string name, object obj, bool defaultValue ) 
		{
			return FindBool(name, obj, defaultValue);
		}

		private static bool FindBool(string name, object obj, bool defaultValue)
		{
			if (string.IsNullOrEmpty(name))
			{
				if(CanBeParsedAsNumber(obj)
				   || obj is string){
					return Convert.ToBoolean(obj);
				}

				return defaultValue;
			}

			string next = string.Empty;
			if (!SearchBase(name, ref next, ref obj))
			{
				return defaultValue;
			}

			return FindBool(next, obj, defaultValue);
		}
		#endregion

		#region String Query
		public static string String( string name, object obj, string defaultValue )
		{
			var value = FindString(name, obj);
			return value != null ? value : defaultValue;
		}
		
		private static bool SearchString(string name, object obj, out string result)
		{
			if (string.IsNullOrEmpty(name))
			{
				result = obj as string;
				if(result == null 
					&& CanBeParsedAsNumber(obj)
					|| obj is string)
				{
					result = obj.ToString();
				}

				return true;
			}

			string next = string.Empty;
			if(!SearchBase(name, ref next, ref obj)){
				result = null;
				return false;
			}

			return SearchString(next, obj, out result);
		}

		private static string FindString(string name, object obj)
		{
			string result;
			if (SearchString(name, obj, out result))
			{
				return result;
			}

			return result;
		}
		#endregion
		
		#region Class Query
		public interface IDotListItem
		{
			bool IsValid { get; }
		}

		public static List<T> List<T>(string name, object obj, List<T> defaultValue) where T : IDotListItem
		{
			ArrayList listData = EB.Dot.Array(name, obj, null);
			if (listData != null)
			{
				List<T> items = new List<T>();
				foreach (object data in listData)
				{
					if (data != null)
					{
						Hashtable itemData = data as Hashtable;
						if (itemData != null)
						{
							T item = (T)System.Activator.CreateInstance(typeof(T), itemData);
							if (item.IsValid == true)
							{
								items.Add(item);
							}
						}
					}
				}
				return items;
			}
			else
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// 给定obj中寻找指定AL
		/// </summary>
		/// <param name="name"></param>
		/// <param name="obj"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static ArrayList Array( string name, object obj, ArrayList defaultValue )
		{
			var value = Find<ArrayList>(name, obj);
			if(value != null){
				if(defaultValue != null){
					Johny.ArrayListPool.Release(defaultValue);
				}
				return value;
			}

			return defaultValue;
		}
		public static T[] Array<T>(string name, object obj, T[] defaultValue, System.Func<object, T> parser)
		{
			return Array(name, obj, defaultValue, delegate (object value, int index) { return parser(value); });
		}

		public static T[] Array<T, K>(string name, object obj, T[] defaultValue, System.Func<object, K, T> parser)
		{
			List<T> list = new List<T>();
			if (Collect(name, obj, parser, delegate (T t, K k) { list.Add(t); }))
			{
				return list.ToArray();
			}
			else
			{
				return defaultValue;
			}
		}
		public static bool Collect<T, K>(string name, object obj, System.Func<object, K, T> parser, System.Action<T, K> collector)
		{
			object o = null;
			if (!Search(name, obj, out o))
			{
				return false;
			}

			if (o == null)
			{
				return true;
			}
			else if (o is Hashtable)
			{
				Hashtable hs = o as Hashtable;
				foreach (DictionaryEntry entry in hs)
				{
					System.Type kT = entry.Key.GetType();
					if (kT == typeof(string) || kT.IsPrimitive)
					{
						K key = (K)System.Convert.ChangeType(entry.Key, typeof(K));
						collector(parser(entry.Value, key), key);
					}
					else
					{
						K key = (K)entry.Key;
						collector(parser(entry.Value, key), key);
					}
				}

				return true;
			}
			else if (o is ArrayList)
			{
				ArrayList array = o as ArrayList;
				for (int i = 0; i < array.Count; ++i)
				{
					K key = (K)System.Convert.ChangeType(i, typeof(K));
					collector(parser(array[i], key), key);
				}

				return true;
			}
			else
			{
				EB.Debug.LogError("Collect: invalid object type {0}", o.GetType());
				return true;
			}
		}
		/// <summary>
		/// 无论输入的数据格式，一定返回Hashtable
		/// </summary>
		/// <param name="name"></param>
		/// <param name="obj"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static Hashtable Object( string name, object obj, Hashtable defaultValue )
		{
			var value = Find<Hashtable>(name, obj);
			if(value != null){
				if(defaultValue != null){
					Johny.HashtablePool.Release(defaultValue);
				}
				return value;
			}

			return defaultValue;
		}

		public static T Find<T>( string name, object obj) where T : class
		{
			T result = null;
			if (Search(name, obj, out result))
			{
				return result;
			}

			return result;
		}

		private static bool Search<T>(string name, object obj, out T result) where T : class
		{
			if (string.IsNullOrEmpty(name))
			{
				result = obj as T;
				return true;
			}

			string next = string.Empty;
			if(!SearchBase(name, ref next, ref obj)){
				result = null;
				return false;
			}

			return Search(next, obj, out result);
		}
		#endregion

		public static UnityEngine.Color Colour( string name, object obj, UnityEngine.Color defaultValue )
		{
			var value = EB.Dot.Object( name, obj, null );
			if (value != null)
			{
				UnityEngine.Color c = UnityEngine.Color.white;
				c.r = EB.Dot.Integer("r", value, 255) / 255.0f;
				c.g = EB.Dot.Integer("g", value, 255) / 255.0f;
				c.b = EB.Dot.Integer("b", value, 255) / 255.0f;
				c.a = EB.Dot.Single("a", value, 1.0f);
				return c;
			}
			
			return defaultValue;
		}
	}
}