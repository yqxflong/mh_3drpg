// Copyright (c) egmkang wang. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// StringView supply:
// IndexOf
// IndexOfAny
// LastIndexOf
// LastIndexOfAny
// Contains
// StartsWith
// EndsWith
// Substring
// Split
// Concat
// Join
// ToLower/ToUpper
// ToInt64/32
// Append
// Replace

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity.Standard.ScriptsWarp
{
	public unsafe struct StringView : IEquatable<String>, IEquatable<StringView>
	{
		public static readonly StringView Empty = new StringView("");

		public static implicit operator StringView(string str)
		{
			return new StringView(str);
		}

		public StringView(string str) : this(str, 0, str.Length)
		{

		}

		public StringView(string str, int begin, int length)
		{
			this.Original = str;
			this.Offset = begin;
			this.Length = length;
			if (str.Length > 0)
			{
				if (this.Offset < 0 || this.Offset >= this.Original.Length) throw new ArgumentOutOfRangeException("begin");
				if (this.Offset + this.Length > this.Original.Length) throw new ArgumentOutOfRangeException("length");
			}
		}

		public int IndexOf(char c, int offset = 0)
		{
			fixed (char* p = this.Original)
			{
				for (int i = offset; i<Length; ++i)
				{
					if (p[this.Offset + i] == c) return i;
				}
			}
  
			return -1;
		}

		public int IndexOfAdvanced(char c)
		{
			return this.IndexOf(c, 0, this.Length);
		}

		public int IndexOfAdvanced(char c, int offset)
		{
			return this.IndexOf(c, offset, this.Length - offset);
		}

		private int IndexOf32bit(int offset, int count, char value)
		{
			fixed (char* p = this.Original)
			{
				char* pCh = p + offset;
				while (count >= 4)
				{
					if (*pCh == value) goto ReturnIndex;
					if (*(pCh + 1) == value) goto ReturnIndex1;
					if (*(pCh + 2) == value) goto ReturnIndex2;
					if (*(pCh + 3) == value) goto ReturnIndex3;
					count -= 4;
					pCh += 4;
				}
				while (count > 0)
				{
					if (*pCh == value)
						goto ReturnIndex;
					count--;
					pCh++;
				}
				return -1;
			ReturnIndex3: pCh++;
			ReturnIndex2: pCh++;
			ReturnIndex1: pCh++;
			ReturnIndex:
				return (int)(pCh - p);
			}
		}

		private static int IndexOf8B(char* s, int offset, int length, char c)
		{
			if (length > 8 || length < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(length));
			}
			char* p = s + offset;
			switch (length)
			{
				case 8: goto L8;
				case 7: goto L7;
				case 6: goto L6;
				case 5: goto L5;
				case 4: goto L4;
				case 3: goto L3;
				case 2: goto L2;
				case 1: goto L1;
				case 0: goto L0;
			}
		L0: goto Found;
		L8: if (*p++ == c) goto Found;
			L7: if (*p++ == c) goto Found;
			L6: if (*p++ == c) goto Found;
			L5: if (*p++ == c) goto Found;
			L4: if (*p++ == c) goto Found;
			L3: if (*p++ == c) goto Found;
			L2: if (*p++ == c) goto Found;
			L1: if (*p++ == c) goto Found;
			return -1;
		Found: return (int)(p - s);
		}

		private const ulong l1u = 0x1ul | 0x1ul << 16 | 0x1ul << 32 | 0x1ul << 48;
		private const ulong l80u = 0x8000ul | 0x8000ul << 16 | 0x8000ul << 32 | 0x8000ul << 48;

		private int IndexOf64bit(int offset, int count, char value)
		{
			fixed (char* p = this.Original)
			{
				int align = (int)p & 0x7;
				//8 byte aligned or 4 char aligned
				if (align != 0)
				{
					int index = IndexOf8B(p, offset, align >> 1, value);
					if (index >= 0) return index;
				}
				//strlen's quick check is based on ((i - 0x1) & ~i) & 0x80
				//so, if i add `-expectedChar` onto i
				//then i can reuse this formula
				ulong magic = (ulong)value;
				ulong expected = unchecked(magic | magic << 16 | magic << 32 | magic << 48);
				offset += align >> 1;
				int padLength = (Length - offset) >> 2 << 2;
				for (; offset < padLength; offset += 4)
				{
					ulong p1 = *(ulong*)(p + offset + 0) - expected;
					ulong p2 = *(ulong*)(p + offset + 4) - expected;
					ulong result1 = unchecked((p1 - l1u) & ~p1 & l80u);
					ulong reuslt2 = unchecked((p2 - l1u) & ~p2 & l80u);
					if (result1 != 0)
					{
						return IndexOf8B(p, offset + 0, 4, value);
					}
					if (reuslt2 != 0)
					{
						return IndexOf8B(p, offset + 4, 4, value);
					}
				}
				return IndexOf8B(p, offset, Length - offset, value);
			}
		}

		public int IndexOf(char value, int offset, int count)
		{
			if (offset < 0 || offset >= this.Length) throw new ArgumentOutOfRangeException("offset");
			if (count < 0 || count - 1 < offset) throw new ArgumentOutOfRangeException("count");
			if (sizeof(IntPtr) == 8)
			{
				return IndexOf64bit(offset, count, value);
			}
			return IndexOf32bit(offset, count, value);
		}

		private static bool ArrayContains(char[] array, char c)
		{
			int length = array.Length;
			fixed (char* p = array)
			{
				for (int i = 0; i < length; ++i)
					if (p[i] == c) return true;
			}
			return false;
		}

		public int IndexOfAny(char[] anyOf)
		{
			return this.IndexOfAny(anyOf, 0, this.Length);
		}

		public int IndexOfAny(char[] anyOf, int offset)
		{
			return this.IndexOfAny(anyOf, offset, this.Length - offset);
		}

		public int IndexOfAny(char[] anyOf, int offset, int count)
		{
			if (offset < 0 || offset >= this.Length) throw new ArgumentOutOfRangeException("offset");
			if (count < 0 || count - 1 < offset) throw new ArgumentOutOfRangeException("count");
			if (anyOf.Length == 1) return this.IndexOf(anyOf[0], offset);
			fixed (char* p = this.Original)
			{
				int length = System.Math.Min(this.Length, count);
				for (int i = offset; i < length; ++i)
				{
					if (ArrayContains(anyOf, p[this.Offset + i])) return i;
				}
			}
			return -1;
		}

		public int IndexOf(string s)
		{
			return this.IndexOf(s, 0);
		}

		public int IndexOf(string s, int offset)
		{
			if (s == null) throw new ArgumentNullException("s");
			if (offset < 0 || offset >= this.Length) throw new ArgumentOutOfRangeException("offset");
			int s1_length = this.Original.Length;
			int s2_length = s.Length;
			fixed (char* p1 = this.Original)
			{
				for (int i = offset; i < this.Length; ++i)
				{
					if (p1[this.Offset + i] == s[0] &&
						(this.Length - i) >= s.Length &&
						InternalEquals(this.Original, this.Offset + i, s, 0, s.Length))
					{
						return i;
					}
				}
				return -1;
			}
		}

		public int LastIndexOf(char split)
		{
			return this.LastIndexOf(split, this.Length - 1, this.Length);
		}

		public int LastIndexOf(char split, int offst)
		{
			return this.LastIndexOf(split, Offset, this.Length - Offset);
		}

		public int LastIndexOf(char split, int offset, int count)
		{
			if (offset < 0 || offset >= this.Length) throw new ArgumentOutOfRangeException("offset");
			if (count < 0 || count - 1 > offset) throw new ArgumentOutOfRangeException("count");
			fixed (char* p = this.Original)
			{
				int left = System.Math.Min((offset + 1), count);
				while (left >= 4)
				{
					if (p[this.Offset + --left] == split) goto Found;
					if (p[this.Offset + --left] == split) goto Found;
					if (p[this.Offset + --left] == split) goto Found;
					if (p[this.Offset + --left] == split) goto Found;
				}
				while (left > 0)
				{
					if (p[this.Offset + --left] == split) goto Found;
				}
				return -1;
			Found:
				return left;
			}
		}

		public int LastIndexOfAny(params char[] anyOf)
		{
			return this.LastIndexOfAny(anyOf, this.Length - 1, this.Length);
		}

		public int LastIndexOfAny(char[] anyOf, int offset)
		{
			return this.LastIndexOfAny(anyOf, offset, this.Length - offset);
		}

		public int LastIndexOfAny(char[] anyOf, int offset, int count)
		{
			if (anyOf == null) throw new ArgumentNullException("anyOf");
			if (offset < 0 || offset >= this.Length) throw new ArgumentOutOfRangeException("offset");
			if (count < 0 || count - 1 > offset) throw new ArgumentOutOfRangeException("count");
			fixed (char* p = this.Original)
			{
				int left = System.Math.Min((offset + 1), count);
				while (left >= 4)
				{
					if (ArrayContains(anyOf, p[this.Offset + --left])) goto Found;
					if (ArrayContains(anyOf, p[this.Offset + --left])) goto Found;
					if (ArrayContains(anyOf, p[this.Offset + --left])) goto Found;
					if (ArrayContains(anyOf, p[this.Offset + --left])) goto Found;
				}
				while (left > 0)
				{
					if (ArrayContains(anyOf, p[this.Offset + --left])) goto Found;
				}
				return -1;
			Found:
				return left;
			}
		}

		public int LastIndexOf(string s)
		{
			return this.LastIndexOf(s, this.Length - 1);
		}

		public int LastIndexOf(string s, int offset)
		{
			if (s == null) throw new ArgumentNullException("s");
			if (offset < 0 || offset >= this.Length) throw new ArgumentOutOfRangeException("offset");
			if (s.Length == 0 && this.Length > 0) return 0;
			offset = System.Math.Min(offset, this.Length - s.Length);
			int s1_length = this.Original.Length;
			int s2_length = s.Length;
			fixed (char* p1 = this.Original)
			{
				for (int i = offset; i >= 0; --i)
				{
					if (p1[this.Offset + i] == s[0] &&
						(this.Length - i) >= s.Length &&
						InternalEquals(this.Original, this.Offset + i, s, 0, s.Length))
					{
						return i;
					}
				}
				return -1;
			}
		}

		public bool StartsWith(string s)
		{
			if (s == null) throw new ArgumentNullException("s");
			if (s.Length == 0) return true;
			if (this.Length < s.Length) return false;
			return this.Substring(0, s.Length).Equals(new StringView(s));
		}

		public bool EndsWith(char c)
		{
			if (this.Length != 0 && this[this.Length - 1] == c)
				return true;
			return false;
		}

		public bool EndsWith(string s)
		{
			if (this.Length >= s.Length &&
				this.Substring(this.Length - s.Length, s.Length).Equals(new StringView(s)))
				return true;
			return false;
		}

		public bool Contains(string s)
		{
			return this.IndexOf(s) >= 0;
		}

		/// <summary>
		/// index must in [offset, offset + length)
		/// </summary>
		/// <param name="index">the index of the char array</param>
		/// <returns>the charactor</returns>
		public char this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Length)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				fixed (char* p = this.Original)
				{
					return p[this.Offset + index];
				}
			}
		}

		/// <summary>
		/// generate a readonly StringSlice or StringView
		/// </summary>
		/// <param name="begin">the slice's begin</param>
		/// <param name="count">the slice's length</param>
		/// <returns>the string slice</returns>
		public StringView Substring(int begin)
		{
			return this.Substring(begin, this.Length - begin);
		}

		public StringView Substring(int begin, int count)
		{
			if (this.Length == 0 && begin == 0 && count == 0) return Empty;
			if (begin < 0 || begin >= this.Length)
			{
				//UnityEngine.Debug.LogWarning("ArgumentOutOfRangeException: Specified argument was out of the range of valid values. Parameter name: begin");
				return new StringView();
			}
			if (count < 0 || count + begin > this.Length) throw new ArgumentOutOfRangeException("count");
			return new StringView(this.Original, this.Offset + begin, count);
		}

		/// <summary>
		/// split the StringView into multi StringView, where the `split` is the seperator,
		/// which can be a char or a string.
		/// when split is null, the seperator is `WhiteSpace`.
		/// </summary>
		/// <param name="split">the seperator</param>
		/// <returns>Array of StringView</returns>
		public StringView[] Split(char split)
		{
			int length = this.Length;
			int[] posArray = new int[length];
			int splitCount = MakeSplitIndexArray(split, posArray);
			StringView[] ret = new StringView[splitCount + 1];
			int count = 0;
			int index = 0;
			for (int i = 0; i < splitCount; ++i)
			{
				ret[count++] = this.Substring(index, posArray[i] - index);
				index = posArray[i] + 1;
			}
			if (index != length) ret[count++] = this.Substring(index, length - index);
			return ret;
		}

		public StringView[] Split(params char[] split)
		{
			int length = this.Length;
			int[] posArray = new int[length];
			int splitCount = MakeSplitIndexArray(split, posArray);
			StringView[] ret = new StringView[splitCount + 1];
			int count = 0;
			int index = 0;
			for (int i = 0; i < splitCount; ++i)
			{
				ret[count++] = this.Substring(index, posArray[i] - index);
				index = posArray[i] + 1;
			}
			if (index != length) ret[count++] = this.Substring(index, length - index);
			return ret;
		}

		public StringView[] Split(params string[] split)
		{
			int length = this.Length;
			int[] posArray = new int[length];
			int[] lenArray = new int[length];
			int splitCount = MakeSplitIndexArray(split, posArray, lenArray);
			StringView[] ret = new StringView[splitCount + 1];
			int count = 0;
			int index = 0;
			for (int i = 0; i < splitCount; ++i)
			{
				ret[count++] = this.Substring(index, posArray[i] - index);
				index = posArray[i] + lenArray[i];
			}
			if (index != length) ret[count++] = this.Substring(index, length - index);
			return ret;
		}

		private int MakeSplitIndexArray(char split, int[] posArray)
		{
			fixed (char* p = this.Original)
			{
				int splitCount = 0;
				for (int i = 0; i < this.Length; ++i)
				{
					if (p[this.Offset + i] == split) posArray[splitCount++] = i;
				}
				return splitCount;
			}
		}

		private int MakeSplitIndexArray(char[] split, int[] posArray)
		{
			fixed (char* p = this.Original)
			{
				int splitCount = 0;
				if (split == null || split.Length == 0)
				{
					for (int i = 0; i < this.Length; ++i)
					{
						if (char.IsWhiteSpace(p[this.Offset + i])) posArray[splitCount++] = i;
					}
					return splitCount;
				}
				for (int i = 0; i < this.Length; ++i)
				{
					if (ArrayContains(split, p[this.Offset + i])) posArray[splitCount++] = i;
				}
				return splitCount;
			}
		}

		private int MakeSplitIndexArray(string[] split, int[] posArray, int[] lenArray)
		{
			if (split == null || split.Length == 0)
			{
				int count = this.MakeSplitIndexArray((char[])null, posArray);
				for (int i = 0; i < count; ++i)
					lenArray[i] = 1;
				return count;
			}
			fixed (char* p = this.Original)
			{
				int splitCount = 0;
				for (int i = 0; i < this.Length; ++i)
				{
					foreach (var seperator in split)
					{
						if (String.IsNullOrEmpty(seperator)) continue;
						if (p[this.Offset + i] == seperator[0] && (this.Length - i) >= seperator.Length)
						{
							if (InternalEquals(this.Original, this.Offset + i, seperator, 0, seperator.Length))
							{
								posArray[splitCount] = i;
								lenArray[splitCount] = seperator.Length;
								splitCount++;
								i += seperator.Length - 1;
								break;
							}
						}
					}
				}
				return splitCount;
			}
		}

		/// <summary>
		/// [Split2List] is better for large strings than [Split], and it returns a list
		/// </summary>
		/// <param name="split">the seperator</param>
		/// <returns>List of StringView</returns>
		public List<StringView> Split2List(char split)
		{
			List<StringView> strList = new List<StringView>();

			int index = 0;
			int pos1 = 0, pos2 = 0;
			pos2 = this.IndexOf(split);
			while (pos2 > 0 && pos2 < this.Length)
			{
				strList.Add(new StringView(Original, this.Offset + pos1, pos2 - pos1));
				pos1 = pos2 + 1;
				pos2 = this.IndexOf(split, pos1);
				++index;
			}
			if (pos1 != this.Length) strList.Add(new StringView(Original, this.Offset + pos1, this.Length - pos1));

			return strList;
		}

		private static bool InternalEquals(String strA, int indexA, String strB, int indexB, int count)
		{
			return new StringView(strA, indexA, count) == new StringView(strB, indexB, count);
		}

		public override bool Equals(object obj)
		{
			if (obj is StringView)
			{
				return this.Equals((StringView)obj);
			}
			else if (obj is string)
			{
				return this.Equals((string)obj);
			}
			return false;
		}

		public bool Equals(StringView v)
		{
			return this.Equals(v.Original, v.Offset, v.Length);
		}

		public bool Equals(string s)
		{
			return this.Equals(s, 0, s.Length);
		}

		private bool Equals(string s, int offset, int length)
		{
			if (string.IsNullOrEmpty(Original) || string.IsNullOrEmpty(s))
			{
				if (Original.Equals(null) && s.Equals(null))
					return true;
				else if (Original.Equals(string.Empty) && s.Equals(string.Empty))
					return true;
				else
					return false;
			}
			if (length != this.Length) return false;
			if (length == 0) return true;
			if (object.ReferenceEquals(s, Original) && offset == Offset && length == Length) return true;
			fixed (char* p1 = this.Original, p2 = s)
			{
				return EqualsHelper(p1 + this.Offset, p2 + offset, length);
			}
		}

		public bool Compare(StringView v)
		{
			if (v.Length != this.Length) return false;
			for (int i = Offset; i < this.Length; ++i)
			{
				if (this[i] != v[i]) return false;
			}
			return true;
		}

		public unsafe StringView ToUpper()
		{
			var result = new StringView(Original, Offset, Length);
			var value = ToString();
			fixed (char* ptr_this = value)
			{
				fixed (char* ptr_result = result.ToString())
				{
					for (int i = 0; i < value.Length; i++)
					{
						var ch = ptr_this[i];
						if (char.IsLower(ch))
							ptr_result[i] = char.ToUpper(ch);
						else
							ptr_result[i] = ptr_this[i];
					}
				}
			}
			return result;
		}

		public unsafe StringView ToLower()
		{
			var result = new StringView(Original, Offset, Length);
			var value = ToString();
			fixed (char* ptr_this = value)
			{
				fixed (char* ptr_result = result.ToString())
				{
					for (int i = 0; i < value.Length; i++)
					{
						var ch = ptr_this[i];
						if (char.IsUpper(ch))
							ptr_result[i] = char.ToLower(ch);
						else
							ptr_result[i] = ptr_this[i];
					}
				}
			}
			return result;
		}

		public static bool operator ==(StringView a, StringView b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(StringView a, StringView b)
		{
			return !a.Equals(b);
		}

		//Copy from .NET System.String.EqualsHelper
		private static bool EqualsHelper(char* p1, char* p2, int length)
		{
			int left = length;

			if (sizeof(System.IntPtr) == 8)
			{
				//if (*(int*)p1 != *(int*)2) return false;
				left -= 2; p1 += 2; p2 += 2;
				while (left >= 12)
				{
					if (*(long*)(p1 + 0) != *(long*)(p2 + 0)) goto RetFalse;
					if (*(long*)(p1 + 4) != *(long*)(p2 + 4)) goto RetFalse;
					if (*(long*)(p1 + 8) != *(long*)(p2 + 8)) goto RetFalse;
					left -= 12; p1 += 12; p2 += 12;
				}
			}
			else
			{
				while (left >= 10)
				{
					if (*(int*)(p1 + 0) != *(int*)(p2 + 0)) goto RetFalse;
					if (*(int*)(p1 + 2) != *(int*)(p2 + 2)) goto RetFalse;
					if (*(int*)(p1 + 4) != *(int*)(p2 + 4)) goto RetFalse;
					if (*(int*)(p1 + 6) != *(int*)(p2 + 6)) goto RetFalse;
					if (*(int*)(p1 + 8) != *(int*)(p2 + 8)) goto RetFalse;
					left -= 10; p1 += 10; p2 += 10;
				}
			}
			//StringView's string will not be end with '\0'
			//so must scan by char, not int
			while (left > 0)
			{
				if (p1[0] != p2[0]) goto RetFalse;
				left -= 1; p1 += 1; p2 += 1;
			}
			return true;
		RetFalse:
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int CombineHashCodes(int h1, int h2)
		{
			return (((h1 << 5) + h1) ^ h2);
		}

		public override int GetHashCode()
		{
			int hash_code = 0;
			int left = this.Length;
			fixed (char* pointer = this.Original)
			{
				char* p = pointer + this.Offset;
				while (left >= 4)
				{
					hash_code = CombineHashCodes(hash_code, (*(int*)(p + 0)).GetHashCode());
					hash_code = CombineHashCodes(hash_code, (*(int*)(p + 4)).GetHashCode());
					left -= 4;
					p += 4;
				}
				while (left > 0)
				{
					hash_code = CombineHashCodes(hash_code, p->GetHashCode());
					left -= 1;
					p += 1;
				}
			}
			return hash_code;
		}

		private static Func<int, string> FastAllocateString = InitFastAllocateString();

		private static Func<int, string> InitFastAllocateString()
		{
			var fastAllocate = typeof(string).GetMethod("FastAllocateString", BindingFlags.NonPublic | BindingFlags.Static);
			return (Func<int, string>)fastAllocate.CreateDelegate(typeof(Func<int, string>));
		}

		private unsafe delegate void WStrCpy(char* dmem, char* smem, int charCount);

		private static WStrCpy wstrcpy = InitWStrCpy();

		private static WStrCpy InitWStrCpy()
		{
			var fastAllocate = typeof(string).GetMethod("wstrcpy", BindingFlags.NonPublic | BindingFlags.Static);
			return (WStrCpy)fastAllocate.CreateDelegate(typeof(WStrCpy));
		}

		/// <summary>
		/// check all chars < 0x80
		/// </summary>
		/// <returns>true, if all chars is < 0x80 </returns>
		public bool IsAscii()
		{
			fixed (char* p = this.Original)
			{
				char* p1 = p + this.Offset;
				int left = this.Length;
				if (sizeof(System.IntPtr) == 8)
				{
					while (left >= 32)
					{
						if ((*(ulong*)(p1 + 0) & 0xFF80FF80FF80FF80ul) != 0) goto RetFalse;
						if ((*(ulong*)(p1 + 2) & 0xFF80FF80FF80FF80ul) != 0) goto RetFalse;
						if ((*(ulong*)(p1 + 4) & 0xFF80FF80FF80FF80ul) != 0) goto RetFalse;
						if ((*(ulong*)(p1 + 8) & 0xFF80FF80FF80FF80ul) != 0) goto RetFalse;
						if ((*(ulong*)(p1 + 10) & 0xFF80FF80FF80FF80ul) != 0) goto RetFalse;
						if ((*(ulong*)(p1 + 12) & 0xFF80FF80FF80FF80ul) != 0) goto RetFalse;
						if ((*(ulong*)(p1 + 14) & 0xFF80FF80FF80FF80ul) != 0) goto RetFalse;
						if ((*(ulong*)(p1 + 16) & 0xFF80FF80FF80FF80ul) != 0) goto RetFalse;
						p1 += 32; left -= 32;
					}
				}
				else
				{
					while (left >= 16)
					{
						if ((*(int*)(p1 + 0) & 0xFF80FF80) != 0) goto RetFalse;
						if ((*(int*)(p1 + 2) & 0xFF80FF80) != 0) goto RetFalse;
						if ((*(int*)(p1 + 4) & 0xFF80FF80) != 0) goto RetFalse;
						if ((*(int*)(p1 + 8) & 0xFF80FF80) != 0) goto RetFalse;
						if ((*(int*)(p1 + 10) & 0xFF80FF80) != 0) goto RetFalse;
						if ((*(int*)(p1 + 12) & 0xFF80FF80) != 0) goto RetFalse;
						if ((*(int*)(p1 + 14) & 0xFF80FF80) != 0) goto RetFalse;
						if ((*(int*)(p1 + 16) & 0xFF80FF80) != 0) goto RetFalse;
						p1 += 16; left -= 16;
					}
				}
				while (left >= 8)
				{
					if ((*(int*)(p1 + 0) & 0xFF80FF80) != 0) goto RetFalse;
					if ((*(int*)(p1 + 2) & 0xFF80FF80) != 0) goto RetFalse;
					if ((*(int*)(p1 + 4) & 0xFF80FF80) != 0) goto RetFalse;
					if ((*(int*)(p1 + 8) & 0xFF80FF80) != 0) goto RetFalse;
					p1 += 8; left -= 8;
				}
				while (left > 0)
				{
					if ((*p1 & 0xFF80) != 0) goto RetFalse;
					p1 += 1; left -= 1;
				}
				return true;
			RetFalse:
				return false;
			}
		}

		/// <summary>
		/// allocate one new Char[] and copy all chars
		/// </summary>
		/// <returns>all chars's copy</returns>
		public char[] ToCharArray()
		{
			return this.ToCharArray(0, this.Length);
		}

		public char[] ToCharArray(int offset, int count)
		{
			if (this.Length == 0 && count == this.Length) return new char[0];
			if (offset < 0 || offset >= this.Length) throw new ArgumentOutOfRangeException("offset");
			if (count < 0 || count - 1 < offset) throw new ArgumentOutOfRangeException("count");
			char[] array = new char[count];
			fixed (char* p2 = this.Original, p1 = array)
			{
				wstrcpy(p1, &p2[this.Offset + offset], count);
			}
			return array;
		}

		/// <summary>
		/// concat multi strings into one
		/// high performance then String's operator +
		/// </summary>
		/// <param name="values">array of strings</param>
		/// <returns>one string</returns>
		public static string Concat(params StringView[] values)
		{
			if (values == null) throw new ArgumentNullException("values");
			switch (values.Length)
			{
				case 1: return Concat(values[0]);
				case 2: return Concat(values[0], values[1]);
				case 3: return Concat(values[0], values[1], values[2]);
				case 4: return Concat(values[0], values[1], values[2], values[3]);
			}
			long length = 0;
			foreach (var str in values)
			{
				length += str.Length;
			}
			if (length > int.MaxValue) throw new OutOfMemoryException();
			string ret = FastAllocateString((int)length);
			fixed (char* p1 = ret)
			{
				int currentIndex = 0;
				foreach (var str in values)
				{
					if (str.Length <= 0) continue;
					fixed (char* p2 = str.Original)
					{
						wstrcpy((p1 + currentIndex), (p2 + str.Offset), str.Length);
					}
					currentIndex += str.Length;
				}
			}
			return ret;
		}

		public static string Concat(StringView v0)
		{
			return v0.ToString();
		}

		public static string Concat(StringView v0, StringView v1)
		{
			int nonEmpty = (v0.Length != 0 ? 1 : 0) + (v1.Length != 0 ? 2 : 0);
			switch (nonEmpty)
			{
				case 1: return v0.ToString();
				case 2: return v1.ToString();
			}
			var ret = FastAllocateString(v0.Length + v1.Length);
			fixed (char* p = ret, p0 = v0.Original, p1 = v1.Original)
			{
				wstrcpy(p + 0, p0 + v0.Offset, v0.Length);
				wstrcpy(p + v0.Length, p1 + v1.Offset, v1.Length);
			}
			return ret;
		}

		public static string Concat(StringView v0, StringView v1, StringView v2)
		{
			int nonEmpty = (v0.Length != 0 ? 1 : 0) + (v1.Length != 0 ? 2 : 0) + (v2.Length != 0 ? 4 : 0);
			switch (nonEmpty)
			{
				case 1: return v0.ToString();
				case 2: return v1.ToString();
				case 4: return v2.ToString();
			}
			var ret = FastAllocateString(v0.Length + v1.Length + v2.Length);
			fixed (char* p = ret, p0 = v0.Original, p1 = v1.Original, p2 = v2.Original)
			{
				wstrcpy(p + 0, p0 + v0.Offset, v0.Length);
				wstrcpy(p + v0.Length, p1 + v1.Offset, v1.Length);
				wstrcpy(p + v0.Length + v1.Length, p2 + v2.Offset, v2.Length);
			}
			return ret;
		}

		public static string Concat(StringView v0, StringView v1, StringView v2, StringView v3)
		{
			int nonEmpty = (v0.Length != 0 ? 1 : 0) + (v1.Length != 0 ? 2 : 0) + (v2.Length != 0 ? 4 : 0) + (v3.Length != 0 ? 8 : 0);
			switch (nonEmpty)
			{
				case 1: return v0.ToString();
				case 2: return v1.ToString();
				case 4: return v2.ToString();
				case 8: return v3.ToString();
			}
			var ret = FastAllocateString(v0.Length + v1.Length + v2.Length + v3.Length);
			fixed (char* p = ret, p0 = v0.Original, p1 = v1.Original, p2 = v2.Original, p3 = v3.Original)
			{
				wstrcpy(p + 0, p0 + v0.Offset, v0.Length);
				wstrcpy(p + v0.Length, p1 + v1.Offset, v1.Length);
				wstrcpy(p + v0.Length + v1.Length, p2 + v2.Offset, v2.Length);
				wstrcpy(p + v0.Length + v1.Length + v2.Length, p3 + v3.Offset, v3.Length);
			}
			return ret;
		}

		/// <summary>
		/// concat multi string into one, with a seperator
		/// </summary>
		/// <param name="seperator">seperator string or split string</param>
		/// <param name="values">array of strings</param>
		/// <returns>one string</returns>
		public static string Join(string seperator, params StringView[] values)
		{
			return Join(seperator, values, 0, values.Length);
		}

		public static string Join(string seperator, StringView[] values, int offset, int count)
		{
			if (seperator == null) seperator = String.Empty;
			if (values == null)
				throw new ArgumentNullException("values");
			long length = 0;
			foreach (var str in values)
			{
				if (str.Length == 0) continue;
				length += str.Length;
			}
			length += (count - 1) * seperator.Length;
			if (length > int.MaxValue) throw new OutOfMemoryException();
			string ret = FastAllocateString((int)length);
			fixed (char* p1 = ret, s = seperator)
			{
				int currentIndex = 0;
				foreach (var str in values)
				{
					if (currentIndex != 0 && seperator.Length != 0)
					{
						wstrcpy((p1 + currentIndex), s, seperator.Length);
						currentIndex += seperator.Length;
					}
					if (str.Length <= 0) continue;
					fixed (char* p2 = str.Original)
					{
						wstrcpy((p1 + currentIndex), (p2 + str.Offset), str.Length);
					}
					currentIndex += str.Length;
				}
			}
			return ret;
		}

		private unsafe StringView Replace(string old_value, string new_value)
		{
			string value = Offset != 0 || Length != Original.Length ? ToString() : Original;
			if (old_value == null)
				throw new ArgumentNullException("old_value");
			if (new_value == null)
				throw new ArgumentNullException("new_value");
			int idx = IndexOf(old_value);
			if (idx == -1)
				return value;
			if (g_finds == null) g_finds = new List<int>();
			g_finds.Clear();
			g_finds.Add(idx);

			while (idx + old_value.Length < value.Length)
			{
				idx = IndexOf(old_value, idx + old_value.Length);
				if (idx == -1)
					break;
				g_finds.Add(idx);
			}
			// calc the right new total length
			int new_len;
			int dif = old_value.Length - new_value.Length;
			if (dif > 0)
				new_len = value.Length - (g_finds.Count * dif);
			else
				new_len = value.Length + (g_finds.Count * -dif);
			StringView result = new StringView(value, 0, new_len);
			fixed (char* ptr_this = value)
			{
				fixed (char* ptr_result = result.ToString())
				{
					for (int i = 0, x = 0, j = 0; i < new_len;)
					{
						if (x == g_finds.Count || g_finds[x] != j)
						{
							ptr_result[i++] = ptr_this[j++];
						}
						else
						{
							for (int n = 0; n < new_value.Length; n++)
								ptr_result[i + n] = new_value[n];
							x++;
							i += new_value.Length;
							j += old_value.Length;
						}
					}
				}
			}
			return result;
		}

		/// <summary>
		/// generate a string instance, which will copy the chars of the StringView
		/// </summary>
		/// <returns>a string</returns>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(Original))
				return string.Empty;

			if (Offset == 0 && Length == Original.Length)
				return Original;

			return Original.Substring(Offset, Length);
		}

		public string ToStringWithChar()
		{
			if (string.IsNullOrEmpty(Original))
				return string.Empty;

			if (Offset == 0 && Length == Original.Length)
				return Original;

			return new string(ToCharArray());
		}

		/// <summary>
		/// the real string of the StringView
		/// </summary>
		public string Original { get; }

		/// <summary>
		/// the first char index of the Original String
		/// </summary>
		public int Offset { get; }

		/// <summary>
		/// the length of the StringView
		/// </summary>
		public int Length { get; }

		/// <summary>
		/// the string replace function records the location of substrings
		/// </summary>
		private static List<int> g_finds;
	}

	public static partial class StringViewExtend
	{
		public unsafe static long ToInt64(this StringView view)
		{
			bool negative = false;
			long num = 0;
			if (view.Length <= 0) return num;

			fixed (char* stringPointer = view.Original)
			{
				int left = view.Length;
				char* p = stringPointer + view.Offset;
				int i = 0;
				if (p[0] == '+') { ++i; --left; }
				if (p[0] == '-') { ++i; --left; negative = true; }
				while (left >= 4)
				{
					if (p[i + 0] < '0' || p[i + 0] > '9' ||
						p[i + 1] < '0' || p[i + 1] > '9' ||
						p[i + 2] < '0' || p[i + 2] > '9' ||
						p[i + 3] < '0' || p[i + 3] > '9')
					{
						throw new ArgumentException(String.Format("Wrong Number Char:{0}{1}{2}{3}"
							, p[i + 0], p[i + 1], p[i + 2], p[i + 3]));
					}
					num = num * 10000 +
						(p[i + 0] - '0') * 1000 +
						(p[i + 1] - '0') * 100 +
						(p[i + 2] - '0') * 10 +
						(p[i + 3] - '0');
					i += 4;
					left -= 4;
				}

				for (; i < view.Length; ++i)
				{
					if (p[i] < '0' || p[i] > '9')
					{
						throw new ArgumentException(String.Format("Wrong Number Char:{0}", p[i]));
					}
					num = num * 10 + (p[i] - '0');
				}
			}
			return negative ? -num : num;
		}

		public unsafe static int ToInt32(this StringView view)
		{
			return (int)ToInt64(view);
		}

		public static void Append(this System.Text.StringBuilder builder, StringView view)
		{
			builder.Append(view.Original, view.Offset, view.Length);
		}
	}
}