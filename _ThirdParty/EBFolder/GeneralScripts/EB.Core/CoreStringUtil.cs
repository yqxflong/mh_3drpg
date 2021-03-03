using UnityEngine;
using System.Collections;
using Unity.Standard.ScriptsWarp;

namespace EB
{
	public static class StringUtil
	{
		public static string Possesive(string value)
        {
            if ( value.EndsWith("s") || value.EndsWith("S") )
            {
                return value + "'";
            }
            return value + "'s";
        }
		
		static char[] valid = "abcdefghijklmnopqrstuvwxyz0123456789/_".ToCharArray();
		
		public static string SafeKey( string src )
		{
			// remove dots and spaces
			src = src.ToLower();
			for (var i = 0; i < src.Length; )
			{
				if ( System.Array.IndexOf(valid, src[i]) < 0 ) 
				{
					// invalid
					src = src.Remove(i, 1);
				}
				else
				{
					++i;
				}
			}
			return src;
		}

		/// <summary>
		/// ZString,Format,不断重用之前char的位置，从而节省空间，这个方法只能在当前帧下次ZString.Format之前，保证返回的string不会变，一般只能用于临时计算，不能缓存
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static string Format(string format, params object[] args)
		{
			if (!format.Contains("{") || !format.Contains("}")) return format;
			using (ZString.Block())
			{
				if (args != null && args.Length > 0)
				{
					ZString @string = format;
					if (args.Length == 1)
						@string = ZString.Format(format, GetString(args[0]));
					else if (args.Length == 2)
						@string = ZString.Format(format, GetString(args[0]), GetString(args[1]));
					else if (args.Length == 3)
						@string = ZString.Format(format, GetString(args[0]), GetString(args[1]), GetString(args[2]));
					else if (args.Length == 4)
						@string = ZString.Format(format, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]));
					else if (args.Length == 5)
						@string = ZString.Format(format, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]));
					else if (args.Length == 6)
						@string = ZString.Format(format, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]));
					else if (args.Length == 7)
						@string = ZString.Format(format, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]), GetString(args[6]));
					else if (args.Length == 8)
						@string = ZString.Format(format, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]), GetString(args[6]), GetString(args[7]));
					else if (args.Length == 9)
						@string = ZString.Format(format, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]), GetString(args[6]), GetString(args[7]), GetString(args[8]));

					args = null;
					return @string;					
				}
				return format;
			}
		}

		public static string TrimNumberEnd(this string content, string zeroNum = "0.00")
		{
			bool hasPercent = false;
			if (content.EndsWith("%"))
			{
				hasPercent = true;
				content = content.TrimEnd('%');
			}

			bool hasDiscount = false;
			if (content.EndsWith("折"))
			{
				hasDiscount = true;
				content = content.TrimEnd('折');
			}

			if (content.Contains(zeroNum))
				content = content.Remove(content.IndexOf('.'));
			else
				content = content.TrimEnd('0', '.');

			if (hasPercent)
				content = content.Concaten("%");

			if (hasDiscount)
				content = content.Concaten("折");

			return content;
		}

		/// <summary>
		/// String,Format,从池子拷贝，从而支持缓存，注意，cache=true，表示让ZString缓存，cache=false，自己在外部有缓存
		/// </summary>
		/// <param name="format"></param>
		/// <param name="cache"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static string Format(string format, bool cache, params object[] args)
		{
			if (!format.Contains("{") || !format.Contains("}")) return format;
			using (ZString.Block())
			{
				if (args != null && args.Length > 0)
				{
					ZString @string = format;
					if (args.Length == 1)
						@string = ZString.Format(format, GetString(args[0]));
					else if (args.Length == 2)
						@string = ZString.Format(format, GetString(args[0]), GetString(args[1]));
					else if (args.Length == 3)
						@string = ZString.Format(format, GetString(args[0]), GetString(args[1]), GetString(args[2]));
					else if (args.Length == 4)
						@string = ZString.Format(format, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]));
					else if (args.Length == 5)
						@string = ZString.Format(format, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]));
					else if (args.Length == 6)
						@string = ZString.Format(format, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]));
					else if (args.Length == 7)
						@string = ZString.Format(format, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]), GetString(args[6]));
					else if (args.Length == 8)
						@string = ZString.Format(format, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]), GetString(args[6]), GetString(args[7]));
					else if (args.Length == 9)
						@string = ZString.Format(format, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]), GetString(args[6]), GetString(args[7]), GetString(args[8]));

					args = null;
					return @string.Intern(cache);
				}
				return format;
			}
		}

		private static ZString GetString(object arg)
		{			
			ZString value = string.Empty;

			if(arg != null)
			{
				if (arg is string)
				{
					value = (string)arg;
				}
				else if (arg is int)
				{
					value = (int)arg;
				}
				else if (arg is float)
				{
					value = (float)arg;
				}
				else if (arg is bool)
				{
					value = (bool)arg;
				}
				else if (arg is long)
				{
					value = (long)arg;
				}
				else if (arg is uint)
				{
					value = (uint)arg;
				}
				else if (arg is char)
				{
					value = (char)arg;
				}
				else if (arg is System.DateTime)
				{
					value = ((System.DateTime)arg).ToShortDateString();
				}
			}			
			return value;			
		}

		/// <summary>
		/// 这个方法只能在当前帧下次ZString.Split之前，保证返回的string数组不会变，一般只能用于临时计算，不能缓存
		/// </summary>
		/// <param name="self"></param>
		/// <param name="separator"></param>
		/// <returns></returns>
		public static string[] Splits(this string self, char separator)
		{
			using (ZString.Block())
			{
				ZString target = self;
				ZString[] zArrays = target.Split(separator);
				string[] array = new string[zArrays.Length];
				for (int i = 0; i < zArrays.Length; i++) { array[i] = zArrays[i]; }
				return array;
			}
		}

		/// <summary>
		/// ZString.Split，从池子拷贝，从而支持缓存，注意，cache=true，表示让ZString缓存，cache=false，自己在外部有缓存
		/// </summary>
		/// <param name="self"></param>
		/// <param name="cache"></param>
		/// <param name="separator"></param>
		/// <returns></returns>
		public static string[] Splits(this string self, bool cache, char separator)
		{
			using (ZString.Block())
			{
				ZString target = self;
				ZString[] zArrays = target.Split(separator, cache);
				string[] array = new string[zArrays.Length];
				for (int i = 0; i < zArrays.Length; i++) array[i] = zArrays[i];
				return array;
			}
		}

		public static string Concaten(this string self, params object[] args)
		{		
			using (ZString.Block())
			{
				if (args != null && args.Length > 0)
				{
					ZString @string = self;
					if (args.Length == 1)
						@string = ZString.Concat(self, GetString(args[0]));
					else if (args.Length == 2)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]));
					else if (args.Length == 3)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]));
					else if (args.Length == 4)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]));
					else if (args.Length == 5)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]));
					else if (args.Length == 6)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]));
					else if (args.Length == 7)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]), GetString(args[6]));
					else if (args.Length == 8)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]), GetString(args[6]), GetString(args[7]));
					else if (args.Length == 9)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]), GetString(args[6]), GetString(args[7]), GetString(args[8]));

					args = null;
					return @string;
				}
				return self;
			}
		}

		public static string Concaten(this string self, bool cache, params object[] args)
		{
			using (ZString.Block())
			{
				if (args != null && args.Length > 0)
				{
					ZString @string = self;
					if (args.Length == 1)
						@string = ZString.Concat(self, GetString(args[0]));
					else if (args.Length == 2)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]));
					else if (args.Length == 3)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]));
					else if (args.Length == 4)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]));
					else if (args.Length == 5)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]));
					else if (args.Length == 6)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]));
					else if (args.Length == 7)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]), GetString(args[6]));
					else if (args.Length == 8)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]), GetString(args[6]), GetString(args[7]));
					else if (args.Length == 9)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]), GetString(args[6]), GetString(args[7]), GetString(args[8]));

					args = null;
					return @string.Intern(cache);
				}
				return self;
			}
		}

		public static string Concat(string self, params object[] args)
		{
			using (ZString.Block())
			{
				if (args != null && args.Length > 0)
				{
					ZString @string = self;
					if (args.Length == 1)
						@string = ZString.Concat(self, GetString(args[0]));
					else if (args.Length == 2)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]));
					else if (args.Length == 3)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]));
					else if (args.Length == 4)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]));
					else if (args.Length == 5)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]));
					else if (args.Length == 6)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]));
					else if (args.Length == 7)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]), GetString(args[6]));
					else if (args.Length == 8)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]), GetString(args[6]), GetString(args[7]));
					else if (args.Length == 9)
						@string = ZString.Concat(self, GetString(args[0]), GetString(args[1]), GetString(args[2]), GetString(args[3]), GetString(args[4]), GetString(args[5]), GetString(args[6]), GetString(args[7]), GetString(args[8]));

					args = null;
					return @string;
				}
				return self;
			}
		}

		public static int IndexOf(this string[] self, string arg)
		{
			int index = -1;
			for(int i=0;i<self.Length;i++)
			{
				if(self[i].Equals(arg))
				{
					index = i;
					break;
				}
			}
			return index;
		}
	}
}