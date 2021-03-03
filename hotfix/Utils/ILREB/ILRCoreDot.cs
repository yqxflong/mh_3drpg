//Dot
//ILR层面使用Dot
//Johny

using System;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.EBCore
{
	public static class Dot
	{
		#region Base (string.SubString的GC???)
		private static bool Collect<T, K>(string name, object obj, System.Func<object, K,T> parser, System.Action<T, K> collector)
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
		
		#region Enum Query
		/// <summary>
		/// 递归寻找Enum
		/// </summary>
		[Obsolete("请自行用Dot.Int取出后强转！")]
		public static T Enum<T>( string name, object obj, T defaultValue ) where T : struct, System.IConvertible
		{
			if (!typeof(T).IsEnum) throw new System.ArgumentException("T must be an enumerated type");
			
			return FindEnum<T>(name, obj, defaultValue);
		}

		private static T FindEnum<T>(string name, object obj, T defaultValue) where T : struct
		{
			if (string.IsNullOrEmpty(name))
			{
				if(obj is T)//只能拿到枚举
				{//最佳情况
					return (T)obj;
				}
				else if(obj is string)
				{
					EB.Debug.LogError("此接口已弃用，请尽快使用Dot.String获取!");
					return (T)System.Enum.Parse( typeof( T ), obj.ToString(), true );
				}
				else if(obj is int)
				{
					EB.Debug.LogError("此接口已弃用，请尽快使用Dot.Int获取!");
					return (T)obj;
				}


				if (obj != null)//添加枚举转换报错提示
				{
					EB.Debug.LogError("Can not Parse {0} to Enum so give defaultvalue: {1}",obj,defaultValue);
				}
				return defaultValue;
			}

			string next = string.Empty;
			if (!SearchBase(name, ref next, ref obj))
			{
				return defaultValue;
			}

			return FindEnum(next, obj, defaultValue);
		}
		#endregion

		#region Class Query
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

		public static T[] Array<T>(string name, object obj, T[] defaultValue, System.Func<object,T> parser)
		{
			return Array(name, obj, defaultValue, delegate (object value, int index) { return parser(value); });
		}

		public static T[] Array<T, K>(string name, object obj, T[] defaultValue, System.Func<object, K,T> parser)
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

		public static List<T> List<T, K>(string name, object obj, List<T> defaultValue, System.Func<object, K,T> parser)
		{
			List<T> list = new List<T>();
			if (Collect(name, obj, parser, delegate (T t, K k) { list.Add(t); }))
			{
				return list;
			}
			else
			{
				return defaultValue;
			}
		}

		public static List<T> List<T, K>(string name, object obj, List<T> defaultValue, System.Func<object, K,T> parser, System.Func<T, K,bool> match)
		{
			List<T> list = defaultValue ?? new List<T>();

			Collect(name, obj, parser, delegate (T t, K k)
			{
				int index = match != null ? list.FindIndex(e => match(e, k)) : -1;
				if(GlobalUtils.Comparer<T>(t))
				// if (EqualityComparer<T>.Default.Equals(t, default(T)))
				{
					if (index >= 0)
					{// remove
						list.RemoveAt(index);
					}
				}
				else
				{
					if (index >= 0)
					{// modify
						list[index] = t;
					}
					else
					{// add
						list.Add(t);
					}
				}
			});

			return list;
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
	}
}