using System;
using System.Collections.Generic;

public class ZString
{
	#region struct
	struct Byte8192
	{
		Byte4096 a1; Byte4096 a2;
	}

	struct Byte4096
	{
		Byte2048 a1; Byte2048 a2;
	}

	struct Byte2048
	{
		Byte1024 a1; Byte1024 a2;
	}

	struct Byte1024
	{
		Byte512 a1; Byte512 a2;
	}

	struct Byte512
	{
		Byte256 a1; Byte256 a2;
	}

	struct Byte256
	{
		Byte128 a1; Byte128 a2;
	}

	struct Byte128
	{
		Byte64 a1; Byte64 a2;
	}

	struct Byte64
	{
		Byte32 a1; Byte32 a2;
	}

	struct Byte32
	{
		Byte16 a1; Byte16 a2;
	}

	struct Byte16
	{
		Byte8 a1; Byte8 a2;
	}

	struct Byte8
	{
		long a1;
	}

	struct Byte4
	{
		int a1;
	}

	struct Byte2
	{
		short a;
	}

	struct Byte1
	{
		byte a;
	}
	#endregion struct

	#region for IL2cpp时这些struct没有在使用其的方法所在文件中声明
	private static Byte8192 stByte8192;
	private static Byte4096 stByte4096;
	private static Byte2048 stByte2048;
	private static Byte1024 stByte1024;
	private static Byte512 stByte512;
	private static Byte256 stByte256;
	private static Byte128 stByte128;
	private static Byte64 stByte64;
	private static Byte32 stByte32;
	private static Byte16 stByte16;
	private static Byte8 stByte8;
	private static Byte4 stByte4;
	private static Byte2 stByte2;
	private static Byte1 stByte1;
	#endregion

	private static Queue<ZString>[] g_cache;//idx特定字符串长度,深拷贝核心缓存
	private static Dictionary<int, Queue<ZString>> g_secCache;//key特定字符串长度value字符串栈，深拷贝次级缓存
	private static Stack<ZString> g_shallowCache;//浅拷贝缓存
	private static Stack<zstring_block> g_blocks;//zstring_block缓存栈
	private static Stack<zstring_block> g_open_blocks;//zstring已经打开的缓存栈
	private static Dictionary<int, string> g_intern_table;//字符串intern表
	public static zstring_block g_current_block;//zstring所在的block块
	private static List<int> g_finds;//字符串replace功能记录子串位置
	private static ZString[] g_format_args;//存储格式化字符串值
	private static List<int> m_SeperatorPos = new List<int>(); // 分割符的位置池,避免不断分配GC
	private const int INITIAL_BLOCK_CAPACITY = 32;//gblock块数量
	private const int INITIAL_CACHE_CAPACITY = 128;//cache缓存字典容量  128*4Byte 500多Byte
	private const int INITIAL_STACK_CAPACITY = 48;//cache字典每个stack默认nstring容量
	private const int INITIAL_INTERN_CAPACITY = 256;//Intern容量
	private const int INITIAL_OPEN_CAPACITY = 5;//默认打开层数为5
	private const int INITIAL_SHALLOW_CAPACITY = 100;//默认50个浅拷贝用
	private const char NEW_ALLOC_CHAR = 'X';//填充char
	private bool isShallow = false;//是否浅拷贝

	[NonSerialized]
	private string _value;//值

	[NonSerialized]
	private bool _disposed;//销毁标记

	//不支持构造
	private ZString()
	{
		throw new NotSupportedException();
	}

	//带默认长度的构造
	private ZString(int length)
	{
		_value = new string(NEW_ALLOC_CHAR, length);
	}

	//浅拷贝专用构造
	private ZString(string value, bool shallow)
	{
		if (!shallow)
		{
			throw new NotSupportedException();
		}
		_value = value;
		isShallow = true;
	}

	static ZString()
	{
		Initialize(INITIAL_CACHE_CAPACITY,
					INITIAL_STACK_CAPACITY,
					INITIAL_BLOCK_CAPACITY,
					INITIAL_INTERN_CAPACITY,
					INITIAL_OPEN_CAPACITY,
					INITIAL_SHALLOW_CAPACITY
					);
		g_finds = new List<int>(10);
		g_format_args = new ZString[10];
	}

	//析构
	private void dispose()
	{
		if (_disposed)
			throw new ObjectDisposedException(this);
		if (isShallow)//深浅拷贝走不同缓存
		{
			g_shallowCache.Push(this);
		}
		else
		{
			Queue<ZString> stack;
			if (g_cache.Length > Length)
			{
				stack = g_cache[Length];//取出valuelength长度的栈，将自身push进去
			}
			else
			{
				stack = g_secCache[Length];
			}
			stack.Enqueue(this);
		}
		//memcpy(_value, NEW_ALLOC_CHAR);//内存拷贝至value
		_disposed = true;
	}

	//由string获取相同内容zstring，深拷贝
	private static ZString get(string value)
	{
		if (value == null)
			return null;
#if DBG
        if (log != null)
            log("Getting: " + value);
#endif
		var result = get(value.Length);
		memcpy(dst: result, src: value);//内存拷贝
		return result;
	}

	//由string浅拷贝入zstring
	private static ZString getShallow(string value)
	{
		if (g_current_block == null)
		{
			throw new InvalidOperationException("nstring 操作必须在一个nstring_block块中。");
		}
		ZString result;
		if (g_shallowCache.Count == 0)
		{
			result = new ZString(value, true);
		}
		else
		{
			result = g_shallowCache.Pop();
			result._value = value;
		}
		result._disposed = false;
		g_current_block.push(result);//zstring推入块所在栈
		return result;
	}

	//将string加入intern表中
	private static string __intern(string value)
	{
		int hash = value.GetHashCode();
		if (g_intern_table.ContainsKey(hash))
		{
			return g_intern_table[hash];
		}
		else
		{
			string interned = new string(NEW_ALLOC_CHAR, value.Length);
			memcpy(interned, value);
			g_intern_table.Add(hash, interned);
			return interned;
		}
	}

	private static string __internUncache(string value)
	{
		string interned = new string(NEW_ALLOC_CHAR, value.Length);
		memcpy(interned, value);
		return interned;
	}

	//手动添加方法
	private static void getStackInCache(int index, out Queue<ZString> outStack)
	{
		int length = g_cache.Length;
		if (length > index)//从核心缓存中取
		{
			outStack = g_cache[index];
		}
		else//从次级缓存中取
		{
			if (!g_secCache.TryGetValue(index, out outStack))
			{
				outStack = new Queue<ZString>(INITIAL_STACK_CAPACITY);
				g_secCache[index] = outStack;
			}
		}
	}

	//获取特定长度zstring
	private static ZString get(int length)
	{
		if (g_current_block == null || length <= 0)
			throw new InvalidOperationException("zstring 操作必须在一个zstring_block块中。");
		ZString result;
		Queue<ZString> stack;
		getStackInCache(length, out stack);
		//从缓存中取Stack
		if (stack.Count == 0)
		{
			result = new ZString(length);
		}
		else
		{
			result = stack.Dequeue();
		}
		result._disposed = false;
		g_current_block.push(result);//zstring推入块所在栈
		return result;
	}

	//value是10的次方数
	private static int get_digit_count(long value)
	{
		int cnt;
		for (cnt = 1; (value /= 10) > 0; cnt++) ;
		return cnt;
	}

	//value是10的次方数
	private static uint get_digit_count(uint value)
	{
		uint cnt;
		for (cnt = 1; (value /= 10) > 0; cnt++) ;
		return cnt;
	}

	//value是10的次方数
	private static int get_digit_count(int value)
	{
		int cnt;
		for (cnt = 1; (value /= 10) > 0; cnt++) ;
		return cnt;
	}

	//获取char在input中start起往后的下标
	private static int internal_index_of(string input, char value, int start)
	{
		return internal_index_of(input, value, start, input.Length - start);
	}

	//获取string在input中起始0的下标
	private static int internal_index_of(string input, string value)
	{
		return internal_index_of(input, value, 0, input.Length);
	}

	//获取string在input中自0起始下标
	private static int internal_index_of(string input, string value, int start)
	{
		return internal_index_of(input, value, start, input.Length - start);
	}

	//获取格式化字符串
	private unsafe static ZString internal_format(string input, int num_args)
	{
		if (input == null)
			throw new ArgumentNullException("value");
		//新字符串长度
		int new_len = input.Length;
		for (int i = -3; ;)
		{
			i = internal_index_of(input, '{', i + 3);
			if (i == -1)
			{
				break;
			}
			new_len -= 3;
			int arg_idx = input[i + 1] - '0';
			ZString arg = g_format_args[arg_idx];
			new_len += arg.Length;
		}
		ZString result = get(new_len);
		string res_value = result._value;
		int next_output_idx = 0;
		int next_input_idx = 0;
		int brace_idx = -3;
		for (int i = 0, j = 0, x = 0; ; x++) // x < num_args
		{
			brace_idx = internal_index_of(input, '{', brace_idx + 3);
			if (brace_idx == -1)
			{
				break;
			}
			next_input_idx = brace_idx;
			int arg_idx = input[brace_idx + 1] - '0';
			string arg = g_format_args[arg_idx]._value;
			if (brace_idx == -1)
				throw new InvalidOperationException("没有发现大括号{ for argument " + arg);
			if (brace_idx + 2 >= input.Length || input[brace_idx + 2] != '}')
				throw new InvalidOperationException("没有发现大括号} for argument " + arg);
			fixed (char* ptr_input = input)
			{
				fixed (char* ptr_result = res_value)
				{
					for (int k = 0; i < new_len;)
					{
						if (j < brace_idx)
						{
							ptr_result[i++] = ptr_input[j++];
							++next_output_idx;
						}
						else
						{
							if(arg.Length > k)
								ptr_result[i++] = arg[k++];
							++next_output_idx;
							if (k == arg.Length)
							{
								j += 3;
								break;
							}
						}
					}
				}
			}
		}
		next_input_idx += 3;
		for (int i = next_output_idx, j = 0; i < new_len; i++, j++)
		{
			fixed (char* ptr_input = input)
			{
				fixed (char* ptr_result = res_value)
				{
					ptr_result[i] = ptr_input[next_input_idx + j];
				}
			}
		}
		return result;
	}

	//获取char在字符串中start开始的下标
	private unsafe static int internal_index_of(string input, char value, int start, int count)
	{
		if (start < 0 || start >= input.Length)
			// throw new ArgumentOutOfRangeException("start");
			return -1;
		if (start + count > input.Length)
			return -1;
		// throw new ArgumentOutOfRangeException("count=" + count + " start+count=" + start + count);
		fixed (char* ptr_this = input)
		{
			int end = start + count;
			for (int i = start; i < end; i++)
				if (ptr_this[i] == value)
					return i;
			return -1;
		}
	}

	//获取value在input中自start起始下标
	private unsafe static int internal_index_of(string input, string value, int start, int count)
	{
		int input_len = input.Length;
		if (start < 0 || start >= input_len)
			throw new ArgumentOutOfRangeException("start");
		if (count < 0 || start + count > input_len)
			throw new ArgumentOutOfRangeException("count=" + count + " start+count=" + (start + count));
		if (count == 0)
			return -1;
		fixed (char* ptr_input = input)
		{
			fixed (char* ptr_value = value)
			{
				int found = 0;
				int end = start + count;
				for (int i = start; i < end; i++)
				{
					for (int j = 0; j < value.Length && i + j < input_len; j++)
					{
						if (ptr_input[i + j] == ptr_value[j])
						{
							found++;
							if (found == value.Length)
								return i;
							continue;
						}
						if (found > 0)
							break;
					}
				}
				return -1;
			}
		}
	}

	//移除string中自start起始count长度子串
	private unsafe static ZString internal_remove(string input, int start, int count)
	{
		if (start < 0 || start >= input.Length)
			throw new ArgumentOutOfRangeException("start=" + start + " Length=" + input.Length);
		if (count < 0 || start + count > input.Length)
			throw new ArgumentOutOfRangeException("count=" + count + " start+count=" + (start + count) + " Length=" + input.Length);
		if (count == 0)
			return input;
		ZString result = get(input.Length - count);
		internal_remove(result, input, start, count);
		return result;
	}

	//将src中自start起count长度子串复制入dst
	private unsafe static void internal_remove(string dst, string src, int start, int count)
	{
		fixed (char* src_ptr = src)
		{
			fixed (char* dst_ptr = dst)
			{
				for (int i = 0, j = 0; i < dst.Length; i++)
				{
					if (i >= start && i < start + count) // within removal range
						continue;
					dst_ptr[j++] = src_ptr[i];
				}
			}
		}
	}

	//字符串replace，原字符串，需替换子串，替换的新子串
	private unsafe static ZString internal_replace(string value, string old_value, string new_value)
	{
		// "Hello, World. There World" | World->Jon =
		// "000000000000000000000" (len = orig - 2 * (world-jon) = orig - 4
		// "Hello, 00000000000000"
		// "Hello, Jon00000000000"
		// "Hello, Jon. There 000"
		// "Hello, Jon. There Jon"
		// "Hello, World. There World" | World->Alexander =
		// "000000000000000000000000000000000" (len = orig + 2 * (alexander-world) = orig + 8
		// "Hello, 00000000000000000000000000"
		// "Hello, Alexander00000000000000000"
		// "Hello, Alexander. There 000000000"
		// "Hello, Alexander. There Alexander"
		if (old_value == null)
			throw new ArgumentNullException("old_value");
		if (new_value == null)
			throw new ArgumentNullException("new_value");
		int idx = internal_index_of(value, old_value);
		if (idx == -1)
			return value;
		g_finds.Clear();
		g_finds.Add(idx);
		// 记录所有需要替换的idx点
		while (idx + old_value.Length < value.Length)
		{
			idx = internal_index_of(value, old_value, idx + old_value.Length);
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
		ZString result = get(new_len);
		fixed (char* ptr_this = value)
		{
			fixed (char* ptr_result = result._value)
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

	//向字符串value中自start位置插入count长度的to_insertChar
	private unsafe static ZString internal_insert(string value, char to_insert, int start, int count)
	{
		// "HelloWorld" (to_insert=x, start=5, count=3) -> "HelloxxxWorld"
		if (start < 0 || start >= value.Length)
			throw new ArgumentOutOfRangeException("start=" + start + " Length=" + value.Length);
		if (count < 0)
			throw new ArgumentOutOfRangeException("count=" + count);
		if (count == 0)
			return get(value);
		int new_len = value.Length + count;
		ZString result = get(new_len);
		fixed (char* ptr_value = value)
		{
			fixed (char* ptr_result = result._value)
			{
				for (int i = 0, j = 0; i < new_len; i++)
				{
					if (i >= start && i < start + count)
						ptr_result[i] = to_insert;
					else
						ptr_result[i] = ptr_value[j++];
				}
			}
		}
		return result;
	}

	//向input字符串中插入to_insert串，位置为start
	private unsafe static ZString internal_insert(string input, string to_insert, int start)
	{
		if (input == null)
			throw new ArgumentNullException("input");
		if (to_insert == null)
			throw new ArgumentNullException("to_insert");
		if (start < 0 || start >= input.Length)
			throw new ArgumentOutOfRangeException("start=" + start + " Length=" + input.Length);
		if (to_insert.Length == 0)
			return get(input);
		int new_len = input.Length + to_insert.Length;
		ZString result = get(new_len);
		internal_insert(result, input, to_insert, start);
		return result;
	}

	//字符串拼接
	private unsafe static ZString internal_concat(string s1, string s2)
	{
		int total_length = s1.Length + s2.Length;
		ZString result = get(total_length);
		fixed (char* ptr_result = result._value)
		{
			fixed (char* ptr_s1 = s1)
			{
				fixed (char* ptr_s2 = s2)
				{
					memcpy(dst: ptr_result, src: ptr_s1, length: s1.Length, src_offset: 0);
					memcpy(dst: ptr_result, src: ptr_s2, length: s2.Length, src_offset: s1.Length);
				}
			}
		}
		return result;
	}

	//将to_insert串插入src的start位置，内容写入dst
	private unsafe static void internal_insert(string dst, string src, string to_insert, int start)
	{
		fixed (char* ptr_src = src)
		{
			fixed (char* ptr_dst = dst)
			{
				fixed (char* ptr_to_insert = to_insert)
				{
					for (int i = 0, j = 0, k = 0; i < dst.Length; i++)
					{
						if (i >= start && i < start + to_insert.Length)
							ptr_dst[i] = ptr_to_insert[k++];
						else
							ptr_dst[i] = ptr_src[j++];
					}
				}
			}
		}
	}

	//将长度为count的数字插入dst中，起始位置为start，dst的长度需大于start+count
	private unsafe static void longcpy(char* dst, long value, int start, int count)
	{
		int end = start + count;
		for (int i = end - 1; i >= start; i--, value /= 10)
			*(dst + i) = (char)(value % 10 + 48);
	}

	//将长度为count的数字插入dst中，起始位置为start，dst的长度需大于start+count
	private unsafe static void intcpy(char* dst, int value, int start, int count)
	{
		int end = start + count;
		for (int i = end - 1; i >= start; i--, value /= 10)
			*(dst + i) = (char)(value % 10 + 48);
	}

	private static unsafe void _memcpy4(byte* dest, byte* src, int size)
	{
		/*while (size >= 32) {
            // using long is better than int and slower than double
            // FIXME: enable this only on correct alignment or on platforms
            // that can tolerate unaligned reads/writes of doubles
            ((double*)dest) [0] = ((double*)src) [0];
            ((double*)dest) [1] = ((double*)src) [1];
            ((double*)dest) [2] = ((double*)src) [2];
            ((double*)dest) [3] = ((double*)src) [3];
            dest += 32;
            src += 32;
            size -= 32;
        }*/
		while (size >= 16)
		{
			((int*)dest)[0] = ((int*)src)[0];
			((int*)dest)[1] = ((int*)src)[1];
			((int*)dest)[2] = ((int*)src)[2];
			((int*)dest)[3] = ((int*)src)[3];
			dest += 16;
			src += 16;
			size -= 16;
		}
		while (size >= 4)
		{
			((int*)dest)[0] = ((int*)src)[0];
			dest += 4;
			src += 4;
			size -= 4;
		}
		while (size > 0)
		{
			((byte*)dest)[0] = ((byte*)src)[0];
			dest += 1;
			src += 1;
			--size;
		}
	}

	private static unsafe void _memcpy2(byte* dest, byte* src, int size)
	{
		while (size >= 8)
		{
			((short*)dest)[0] = ((short*)src)[0];
			((short*)dest)[1] = ((short*)src)[1];
			((short*)dest)[2] = ((short*)src)[2];
			((short*)dest)[3] = ((short*)src)[3];
			dest += 8;
			src += 8;
			size -= 8;
		}
		while (size >= 2)
		{
			((short*)dest)[0] = ((short*)src)[0];
			dest += 2;
			src += 2;
			size -= 2;
		}
		if (size > 0)
		{
			((byte*)dest)[0] = ((byte*)src)[0];
		}
	}

	//从src，0位置起始拷贝count长度字符串src到dst中
	//private unsafe static void memcpy(char* dest, char* src, int count)
	//{
	//    // Same rules as for memcpy, but with the premise that
	//    // chars can only be aligned to even addresses if their
	//    // enclosing types are correctly aligned
	//    superMemcpy(dest, src, count);
	//    //if ((((int)(byte*)dest | (int)(byte*)src) & 3) != 0)//转换为byte指针
	//    //{
	//    //    if (((int)(byte*)dest & 2) != 0 && ((int)(byte*)src & 2) != 0 && count > 0)
	//    //    {
	//    //        ((short*)dest)[0] = ((short*)src)[0];
	//    //        dest++;
	//    //        src++;
	//    //        count--;
	//    //    }
	//    //    if ((((int)(byte*)dest | (int)(byte*)src) & 2) != 0)
	//    //    {
	//    //        _memcpy2((byte*)dest, (byte*)src, count * 2);//转换为short*指针一次两个字节拷贝
	//    //        return;
	//    //    }
	//    //}
	//    //_memcpy4((byte*)dest, (byte*)src, count * 2);//转换为int*指针一次四个字节拷贝
	//}
	//--------------------------------------手敲memcpy-------------------------------------//
	private static int m_charLen = sizeof(char);

	private unsafe static void memcpy(char* dest, char* src, int count)
	{
		byteCopy((byte*)dest, (byte*)src, count * m_charLen);
	}

	private unsafe static void byteCopy(byte* dest, byte* src, int byteCount)
	{
		if (byteCount < 128)
		{
			goto g64;
		}
		else if (byteCount < 2048)
		{
			goto g1024;
		}
		while (byteCount >= 8192)
		{
			((Byte8192*)dest)[0] = ((Byte8192*)src)[0];
			dest += 8192;
			src += 8192;
			byteCount -= 8192;
		}
		if (byteCount >= 4096)
		{
			((Byte4096*)dest)[0] = ((Byte4096*)src)[0];
			dest += 4096;
			src += 4096;
			byteCount -= 4096;
		}
		if (byteCount >= 2048)
		{
			((Byte2048*)dest)[0] = ((Byte2048*)src)[0];
			dest += 2048;
			src += 2048;
			byteCount -= 2048;
		}
	g1024: if (byteCount >= 1024)
		{
			((Byte1024*)dest)[0] = ((Byte1024*)src)[0];
			dest += 1024;
			src += 1024;
			byteCount -= 1024;
		}
		if (byteCount >= 512)
		{
			((Byte512*)dest)[0] = ((Byte512*)src)[0];
			dest += 512;
			src += 512;
			byteCount -= 512;
		}
		if (byteCount >= 256)
		{
			((Byte256*)dest)[0] = ((Byte256*)src)[0];
			dest += 256;
			src += 256;
			byteCount -= 256;
		}
		if (byteCount >= 128)
		{
			((Byte128*)dest)[0] = ((Byte128*)src)[0];
			dest += 128;
			src += 128;
			byteCount -= 128;
		}
	g64: if (byteCount >= 64)
		{
			((Byte64*)dest)[0] = ((Byte64*)src)[0];
			dest += 64;
			src += 64;
			byteCount -= 64;
		}
		if (byteCount >= 32)
		{
			((Byte32*)dest)[0] = ((Byte32*)src)[0];
			dest += 32;
			src += 32;
			byteCount -= 32;
		}
		if (byteCount >= 16)
		{
			((Byte16*)dest)[0] = ((Byte16*)src)[0];
			dest += 16;
			src += 16;
			byteCount -= 16;
		}
		if (byteCount >= 8)
		{
			((Byte8*)dest)[0] = ((Byte8*)src)[0];
			dest += 8;
			src += 8;
			byteCount -= 8;
		}
		if (byteCount >= 4)
		{
			((Byte4*)dest)[0] = ((Byte4*)src)[0];
			dest += 4;
			src += 4;
			byteCount -= 4;
		}
		if (byteCount >= 2)
		{
			((Byte2*)dest)[0] = ((Byte2*)src)[0];
			dest += 2;
			src += 2;
			byteCount -= 2;
		}
		if (byteCount >= 1)
		{
			((Byte1*)dest)[0] = ((Byte1*)src)[0];
			dest += 1;
			src += 1;
			byteCount -= 1;
		}
	}

	//-----------------------------------------------------------------------------------------//
	//将字符串dst用字符src填充
	private unsafe static void memcpy(string dst, char src)
	{
		fixed (char* ptr_dst = dst)
		{
			int len = dst.Length;
			for (int i = 0; i < len; i++)
				ptr_dst[i] = src;
		}
	}

	//将字符拷贝到dst指定index位置
	private unsafe static void memcpy(string dst, char src, int index)
	{
		fixed (char* ptr = dst)
			ptr[index] = src;
	}

	//将相同长度的src内容拷入dst
	private unsafe static void memcpy(string dst, string src)
	{
		if (dst.Length != src.Length)
			throw new InvalidOperationException("两个字符串参数长度不一致。");
		fixed (char* dst_ptr = dst)
		{
			fixed (char* src_ptr = src)
			{
				memcpy(dst_ptr, src_ptr, dst.Length);
			}
		}
	}

	//将src指定length内容拷入dst，dst下标src_offset偏移
	private unsafe static void memcpy(char* dst, char* src, int length, int src_offset)
	{
		memcpy(dst + src_offset, src, length);
	}

	private unsafe static void memcpy(string dst, string src, int length, int src_offset)
	{
		fixed (char* ptr_dst = dst)
		{
			fixed (char* ptr_src = src)
			{
				memcpy(ptr_dst + src_offset, ptr_src, length);
			}
		}
	}

	public class zstring_block : IDisposable
	{
		private readonly Stack<ZString> stack;

		internal zstring_block(int capacity)
		{
			stack = new Stack<ZString>(capacity);
		}

		internal void push(ZString str)
		{
			stack.Push(str);
		}

		internal IDisposable begin()//构造函数
		{
#if DBG
            if (log != null)
                log("Began block");
#endif
			return this;
		}

		void IDisposable.Dispose()//析构函数
		{
#if DBG
            if (log != null)
                log("Disposing block");
#endif
			while (stack.Count > 0)
			{
				var str = stack.Pop();
				str.dispose();//循环调用栈中zstring的Dispose方法
			}
			ZString.g_blocks.Push(this);//将自身push入缓存栈
										//赋值currentBlock
			g_open_blocks.Pop();
			if (g_open_blocks.Count > 0)
			{
				ZString.g_current_block = g_open_blocks.Peek();
			}
			else
			{
				ZString.g_current_block = null;
			}
		}
	}

	// Public API

	#region

	public static Action<string> Log = null;
	public static uint DecimalAccuracy = 3; // 小数点后精度位数

											//获取字符串长度
	public int Length
	{
		get { return _value.Length; }
	}

	//类构造：cache_capacity缓存栈字典容量，stack_capacity缓存字符串栈容量，block_capacity缓存栈容量，intern_capacity缓存,open_capacity默认打开层数
	public static void Initialize(int cache_capacity, int stack_capacity, int block_capacity, int intern_capacity, int open_capacity, int shallowCache_capacity)
	{
		g_cache = new Queue<ZString>[cache_capacity];
		g_secCache = new Dictionary<int, Queue<ZString>>(cache_capacity);
		g_blocks = new Stack<zstring_block>(block_capacity);
		g_intern_table = new Dictionary<int, string>(intern_capacity);
		g_open_blocks = new Stack<zstring_block>(open_capacity);
		g_shallowCache = new Stack<ZString>(shallowCache_capacity);
		for (int c = 0; c < cache_capacity; c++)
		{
			var stack = new Queue<ZString>(stack_capacity);
			for (int j = 0; j < stack_capacity; j++)
				stack.Enqueue(new ZString(c));
			g_cache[c] = stack;
		}
		for (int i = 0; i < block_capacity; i++)
		{
			var block = new zstring_block(block_capacity * 2);
			g_blocks.Push(block);
		}
		for (int i = 0; i < shallowCache_capacity; i++)
		{
			g_shallowCache.Push(new ZString(null, true));
		}
	}

	//using语法所用。从zstring_block栈中取出一个block并将其置为当前g_current_block，在代码块{}中新生成的zstring都将push入块内部stack中。当离开块作用域时，调用块的Dispose函数，将内栈中所有zstring填充初始值并放入zstring缓存栈。同时将自身放入block缓存栈中。（此处有个问题：使用Stack缓存block，当block被dispose放入Stack后g_current_block仍然指向此block，无法记录此block之前的block，这样导致zstring.Block()无法嵌套使用）
	public static IDisposable Block()
	{
		if (g_blocks.Count == 0)
			g_current_block = new zstring_block(INITIAL_BLOCK_CAPACITY * 2);
		else
			g_current_block = g_blocks.Pop();
		g_open_blocks.Push(g_current_block);//新加代码，将此玩意压入open栈
		return g_current_block.begin();
	}

	//将zstring value放入intern缓存表中以供外部使用
	public string Intern(bool cache = true)
	{
		//string interned = new string(NEW_ALLOC_CHAR, _value.Length);
		//memcpy(interned, _value);
		//return interned;
		if (cache)
			return __intern(_value);
		else
			return __internUncache(_value);
	}

	//将string放入zstring intern缓存表中以供外部使用
	public static string Intern(string value)
	{
		return __intern(value);
	}

	public static void Intern(string[] values)
	{
		for (int i = 0; i < values.Length; i++)
			__intern(values[i]);
	}

	//下标取值函数
	public char this[int i]
	{
		get { return _value[i]; }
		set { memcpy(this, value, i); }
	}

	//获取hashcode
	public override int GetHashCode()
	{
		return _value.GetHashCode();
	}

	//字面值比较
	public override bool Equals(object obj)
	{
		if (obj == null)
			return ReferenceEquals(this, null);
		string str = obj as string;
		if (str != null)
			return str.Equals(_value);
		var gstr = obj as ZString;
		if (gstr != null)
			return gstr._value.Equals(_value);
			
		return false;
	}

	//转化为string
	public override string ToString()
	{
		return _value;
	}

	//bool->zstring转换
	public static implicit operator ZString(bool value)
	{
		return get(value ? "True" : "False");
	}

	// long - >zstring转换
	public unsafe static implicit operator ZString(long value)
	{
		// e.g. 125
		// first pass: count the number of digits
		// then: get a zstring with length = num digits
		// finally: iterate again, get the char of each digit, memcpy char to result
		bool negative = value < 0;
		value = Math.Abs(value);
		int num_digits = get_digit_count(value);
		ZString result;
		if (negative)
		{
			result = get(num_digits + 1);
			fixed (char* ptr = result._value)
			{
				*ptr = '-';
				longcpy(ptr, value, 1, num_digits);
			}
		}
		else
		{
			result = get(num_digits);
			fixed (char* ptr = result._value)
				longcpy(ptr, value, 0, num_digits);
		}
		return result;
	}

	//int->zstring转换
	public unsafe static implicit operator ZString(int value)
	{
		// e.g. 125
		// first pass: count the number of digits
		// then: get a zstring with length = num digits
		// finally: iterate again, get the char of each digit, memcpy char to result
		bool negative = value < 0;
		value = Math.Abs(value);
		int num_digits = get_digit_count(value);
		ZString result;
		if (negative)
		{
			result = get(num_digits + 1);
			fixed (char* ptr = result._value)
			{
				*ptr = '-';
				intcpy(ptr, value, 1, num_digits);
			}
		}
		else
		{
			result = get(num_digits);
			fixed (char* ptr = result._value)
				intcpy(ptr, value, 0, num_digits);
		}
		return result;
	}

	//float->zstring转换
	public unsafe static implicit operator ZString(float value)
	{
		// e.g. 3.148
		bool negative = value < 0;
		if (negative) value = -value;
		long mul = (long)Math.Pow(10, DecimalAccuracy);
		long number = (long)(value * mul); // gets the number as a whole, e.g. 3148
		int left_num = (int)(number / mul); // left part of the decimal point, e.g. 3
		int right_num = (int)(number % mul); // right part of the decimal pnt, e.g. 148
		int left_digit_count = get_digit_count(left_num); // e.g. 1
		int right_digit_count = get_digit_count(right_num); // e.g. 3
															//int total = left_digit_count + right_digit_count + 1; // +1 for '.'
		int total = left_digit_count + (int)DecimalAccuracy + 1; // +1 for '.'
		ZString result;
		if (negative)
		{
			result = get(total + 1); // +1 for '-'
			fixed (char* ptr = result._value)
			{
				*ptr = '-';
				intcpy(ptr, left_num, 1, left_digit_count);
				*(ptr + left_digit_count + 1) = '.';
				int offest = (int)DecimalAccuracy - right_digit_count;
				for (int i = 0; i < offest; i++)
					*(ptr + left_digit_count + i + 1) = '0';
				intcpy(ptr, right_num, left_digit_count + 2 + offest, right_digit_count);
			}
		}
		else
		{
			result = get(total);
			fixed (char* ptr = result._value)
			{
				intcpy(ptr, left_num, 0, left_digit_count);
				*(ptr + left_digit_count) = '.';
				int offest = (int)DecimalAccuracy - right_digit_count;
				for (int i = 0; i < offest; i++)
					*(ptr + left_digit_count + i + 1) = '0';
				intcpy(ptr, right_num, left_digit_count + 1 + offest, right_digit_count);
			}
		}
		return result;
	}

	//string->zstring转换
	public static implicit operator ZString(string value)
	{
		//return get(value);
		return getShallow(value);
	}

	//string->zstring转换
	public static ZString shallow(string value)
	{
		return getShallow(value);
	}

	//zstring->string转换
	public static implicit operator string(ZString value)
	{
		if (value == null) return default;
		return value._value;
	}

	//+重载
	public static ZString operator +(ZString left, ZString right)
	{
		return internal_concat(left, right);
	}

	//==重载
	public static bool operator ==(ZString left, ZString right)
	{
		if (ReferenceEquals(left, null))
			return ReferenceEquals(right, null);
		if (ReferenceEquals(right, null))
			return false;
		return left._value == right._value;
	}

	//!=重载
	public static bool operator !=(ZString left, ZString right)
	{
		return !(left._value == right._value);
	}

	//转换为大写
	public unsafe ZString ToUpper()
	{
		var result = get(Length);
		fixed (char* ptr_this = this._value)
		{
			fixed (char* ptr_result = result._value)
			{
				for (int i = 0; i < _value.Length; i++)
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

	//转换为小写
	public unsafe ZString ToLower()
	{
		var result = get(Length);
		fixed (char* ptr_this = this._value)
		{
			fixed (char* ptr_result = result._value)
			{
				for (int i = 0; i < _value.Length; i++)
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

	//移除剪切
	public ZString Remove(int start)
	{
		return Remove(start, Length - start);
	}

	//移除剪切
	public ZString Remove(int start, int count)
	{
		return internal_remove(this._value, start, count);
	}

	//插入start起count长度字符
	public ZString Insert(char value, int start, int count)
	{
		return internal_insert(this._value, value, start, count);
	}

	//插入start起字符串
	public ZString Insert(string value, int start)
	{
		return internal_insert(this._value, value, start);
	}

	//子字符替换
	public unsafe ZString Replace(char old_value, char new_value)
	{
		ZString result = get(Length);
		fixed (char* ptr_this = this._value)
		{
			fixed (char* ptr_result = result._value)
			{
				for (int i = 0; i < Length; i++)
				{
					ptr_result[i] = ptr_this[i] == old_value ? new_value : ptr_this[i];
				}
			}
		}
		return result;
	}

	//子字符串替换
	public ZString Replace(string old_value, string new_value)
	{
		return internal_replace(this._value, old_value, new_value);
	}

	//剪切start位置起后续子串
	public ZString Substring(int start)
	{
		return Substring(start, Length - start);
	}

	//剪切start起count长度的子串
	public unsafe ZString Substring(int start, int count)
	{
		if (start < 0 || start >= Length)
			throw new ArgumentOutOfRangeException("start");
		if (count > Length)
			throw new ArgumentOutOfRangeException("count");
		ZString result = get(count);
		fixed (char* src = this._value)
		fixed (char* dst = result._value)
			memcpy(dst, src + start, count);
		return result;
	}

	public unsafe ZString[] Split(char seperator)
	{
		if (!Contains(seperator)) { return new ZString[] { this }; }

		//int[] posArray = new int[Length];
		int splitCount = MakeSplitIndexArray(seperator);
		ZString[] result = null;
		int count = 0; int index = 0;

		if (splitCount <= 0 && result.Length == 1)
		{
			result = new ZString[splitCount + 1];
			result[0] = this; return result;
		}
		else
		{
			if (m_SeperatorPos[splitCount - 1] == 0 || m_SeperatorPos[splitCount - 1] == Length - 1)
			{
				result = new ZString[splitCount];
			}
			else
				result = new ZString[splitCount + 1];
		}

		for (int i = 0; i < splitCount; ++i)
		{
			int length = m_SeperatorPos[i] - index;
			if (length > 0) result[count++] = Substring(index, length);
			index = m_SeperatorPos[i] + 1;
		}
		if (index != Length) result[count++] = Substring(index, Length - index);

		//Console.PrintImportant("SeperatorPos List Pool, Counter: {0}", m_SeperatorPos.Count);
		if (m_SeperatorPos.Count > 64) { m_SeperatorPos.Clear(); }

		return result;
	}

	public unsafe ZString[] Split(char seperator, bool cache)
	{
		if (!Contains(seperator)) { return new ZString[] { this }; }

		//int[] posArray = new int[Length];
		int splitCount = MakeSplitIndexArray(seperator);
		ZString[] result = null;
		int count = 0; int index = 0;

		if (splitCount <= 0 && result.Length == 1)
		{
			result = new ZString[splitCount + 1];
			result[0] = this; return result;
		}
		else
		{
			if(m_SeperatorPos[splitCount-1] == 0 || m_SeperatorPos[splitCount - 1] == Length-1)
			{
				result = new ZString[splitCount];
			}
			else
				result = new ZString[splitCount+1];
		}
			
		for (int i = 0; i < splitCount; ++i)
		{
			int length = m_SeperatorPos[i] - index;
			if (length > 0)
			{
				ZString inter = Substring(index, m_SeperatorPos[i] - index);
				result[count++] = inter.Intern(cache);
			}				
			index = m_SeperatorPos[i] + 1;
		}
		if (index != Length)
		{
			ZString _inter = Substring(index, Length - index);
			result[count++] = _inter.Intern(cache);
		}

		//Console.PrintImportant("SeperatorPos List Pool, Counter: {0}", m_SeperatorPos.Count);
		if (m_SeperatorPos.Count > 64) { m_SeperatorPos.Clear(); }

		return result;
	}

	private unsafe int MakeSplitIndexArray(char split)
	{
		fixed (char* p = _value)
		{
			int splitCount = 0;
			for (int i = 0; i < Length; ++i)
			{					
				if (p[i] == split)
				{						
					if (m_SeperatorPos.Count <= splitCount) m_SeperatorPos.Add(0);
					m_SeperatorPos[splitCount] = i;
					splitCount++;
				}
			}
			return splitCount;
		}
	}

	//子串包含判断
	public bool Contains(string value)
	{
		return IndexOf(value) != -1;
	}

	//字符包含判断
	public bool Contains(char value)
	{
		return IndexOf(value) != -1;
	}

	//子串第一次出现位置
	public int LastIndexOf(string value)
	{
		int idx = -1;
		int last_find = -1;
		while (true)
		{
			idx = internal_index_of(this._value, value, idx + value.Length);
			last_find = idx;
			if (idx == -1 || idx + value.Length >= this._value.Length)
				break;
		}
		return last_find;
	}

	//字符第一次出现位置
	public int LastIndexOf(char value)
	{
		int idx = -1;
		int last_find = -1;
		while (true)
		{
			idx = internal_index_of(this._value, value, idx + 1);
			last_find = idx;
			if (idx == -1 || idx + 1 >= this._value.Length)
				break;
		}
		return last_find;
	}

	//字符第一次出现位置
	public int IndexOf(char value)
	{
		return IndexOf(value, 0, Length);
	}

	//字符自start起第一次出现位置
	public int IndexOf(char value, int start)
	{
		return internal_index_of(this._value, value, start);
	}

	//字符自start起count长度内，
	public int IndexOf(char value, int start, int count)
	{
		return internal_index_of(this._value, value, start, count);
	}

	//子串第一次出现位置
	public int IndexOf(string value)
	{
		return IndexOf(value, 0, Length);
	}

	//子串自start位置起，第一次出现位置
	public int IndexOf(string value, int start)
	{
		return IndexOf(value, start, Length - start);
	}

	//子串自start位置起，count长度内第一次出现位置
	public int IndexOf(string value, int start, int count)
	{
		return internal_index_of(this._value, value, start, count);
	}

	//是否以某字符串结束
	public unsafe bool EndsWith(string postfix)
	{
		if (postfix == null)
			throw new ArgumentNullException("postfix");
		if (this.Length < postfix.Length)
			return false;
		fixed (char* ptr_this = this._value)
		{
			fixed (char* ptr_postfix = postfix)
			{
				for (int i = this._value.Length - 1, j = postfix.Length - 1; j >= 0; i--, j--)
					if (ptr_this[i] != ptr_postfix[j])
						return false;
			}
		}
		return true;
	}

	//是否以某字符串开始
	public unsafe bool StartsWith(string prefix)
	{
		if (prefix == null)
			throw new ArgumentNullException("prefix");
		if (this.Length < prefix.Length)
			return false;
		fixed (char* ptr_this = this._value)
		{
			fixed (char* ptr_prefix = prefix)
			{
				for (int i = 0; i < prefix.Length; i++)
					if (ptr_this[i] != ptr_prefix[i])
						return false;
			}
		}
		return true;
	}

	//获取某长度字符串缓存数量
	public static int GetCacheCount(int length)
	{
		Queue<ZString> stack;
		getStackInCache(length, out stack);
		return stack.Count;
	}

	//自身+value拼接
	public ZString Concat(ZString value)
	{
		return internal_concat(this, value);
	}

	//静态拼接方法簇
	public static ZString Concat(ZString s0, ZString s1) { return s0 + s1; }

	public static ZString Concat(ZString s0, ZString s1, ZString s2)
	{
		return s0 + s1 + s2;
	}

	public static ZString Concat(ZString s0, ZString s1, ZString s2, ZString s3)
	{
		return s0 + s1 + s2 + s3;
	}

	public static ZString Concat(ZString s0, ZString s1, ZString s2, ZString s3, ZString s4)
	{
		return s0 + s1 + s2 + s3 + s4;
	}

	public static ZString Concat(ZString s0, ZString s1, ZString s2, ZString s3, ZString s4, ZString s5)
	{
		return s0 + s1 + s2 + s3 + s4 + s5;
	}

	public static ZString Concat(ZString s0, ZString s1, ZString s2, ZString s3, ZString s4, ZString s5, ZString s6)
	{
		return s0 + s1 + s2 + s3 + s4 + s5 + s6;
	}

	public static ZString Concat(ZString s0, ZString s1, ZString s2, ZString s3, ZString s4, ZString s5, ZString s6, ZString s7)
	{
		return s0 + s1 + s2 + s3 + s4 + s5 + s6 + s7;
	}

	public static ZString Concat(ZString s0, ZString s1, ZString s2, ZString s3, ZString s4, ZString s5, ZString s6, ZString s7, ZString s8)
	{
		return s0 + s1 + s2 + s3 + s4 + s5 + s6 + s7 + s8;
	}

	public static ZString Concat(ZString s0, ZString s1, ZString s2, ZString s3, ZString s4, ZString s5, ZString s6, ZString s7, ZString s8, ZString s9)
	{
		return s0 + s1 + s2 + s3 + s4 + s5 + s6 + s7 + s8 + s9;
	}

	//静态格式化方法簇
	public static ZString Format(string input, ZString arg0, ZString arg1, ZString arg2, ZString arg3, ZString arg4, ZString arg5, ZString arg6, ZString arg7, ZString arg8, ZString arg9)
	{
		if (arg0 == null) throw new ArgumentNullException("arg0");
		if (arg1 == null) throw new ArgumentNullException("arg1");
		if (arg2 == null) throw new ArgumentNullException("arg2");
		if (arg3 == null) throw new ArgumentNullException("arg3");
		if (arg4 == null) throw new ArgumentNullException("arg4");
		if (arg5 == null) throw new ArgumentNullException("arg5");
		if (arg6 == null) throw new ArgumentNullException("arg6");
		if (arg7 == null) throw new ArgumentNullException("arg7");
		if (arg8 == null) throw new ArgumentNullException("arg8");
		if (arg9 == null) throw new ArgumentNullException("arg9");
		g_format_args[0] = arg0;
		g_format_args[1] = arg1;
		g_format_args[2] = arg2;
		g_format_args[3] = arg3;
		g_format_args[4] = arg4;
		g_format_args[5] = arg5;
		g_format_args[6] = arg6;
		g_format_args[7] = arg7;
		g_format_args[8] = arg8;
		g_format_args[9] = arg9;
		return internal_format(input, 10);
	}

	public static ZString Format(string input, ZString arg0, ZString arg1, ZString arg2, ZString arg3, ZString arg4, ZString arg5, ZString arg6, ZString arg7, ZString arg8)
	{
		if (arg0 == null) throw new ArgumentNullException("arg0");
		if (arg1 == null) throw new ArgumentNullException("arg1");
		if (arg2 == null) throw new ArgumentNullException("arg2");
		if (arg3 == null) throw new ArgumentNullException("arg3");
		if (arg4 == null) throw new ArgumentNullException("arg4");
		if (arg5 == null) throw new ArgumentNullException("arg5");
		if (arg6 == null) throw new ArgumentNullException("arg6");
		if (arg7 == null) throw new ArgumentNullException("arg7");
		if (arg8 == null) throw new ArgumentNullException("arg8");
		g_format_args[0] = arg0;
		g_format_args[1] = arg1;
		g_format_args[2] = arg2;
		g_format_args[3] = arg3;
		g_format_args[4] = arg4;
		g_format_args[5] = arg5;
		g_format_args[6] = arg6;
		g_format_args[7] = arg7;
		g_format_args[8] = arg8;
		return internal_format(input, 9);
	}

	public static ZString Format(string input, ZString arg0, ZString arg1, ZString arg2, ZString arg3, ZString arg4, ZString arg5, ZString arg6, ZString arg7)
	{
		if (arg0 == null) throw new ArgumentNullException("arg0");
		if (arg1 == null) throw new ArgumentNullException("arg1");
		if (arg2 == null) throw new ArgumentNullException("arg2");
		if (arg3 == null) throw new ArgumentNullException("arg3");
		if (arg4 == null) throw new ArgumentNullException("arg4");
		if (arg5 == null) throw new ArgumentNullException("arg5");
		if (arg6 == null) throw new ArgumentNullException("arg6");
		if (arg7 == null) throw new ArgumentNullException("arg7");
		g_format_args[0] = arg0;
		g_format_args[1] = arg1;
		g_format_args[2] = arg2;
		g_format_args[3] = arg3;
		g_format_args[4] = arg4;
		g_format_args[5] = arg5;
		g_format_args[6] = arg6;
		g_format_args[7] = arg7;
		return internal_format(input, 8);
	}

	public static ZString Format(string input, ZString arg0, ZString arg1, ZString arg2, ZString arg3, ZString arg4, ZString arg5, ZString arg6)
	{
		if (arg0 == null) throw new ArgumentNullException("arg0");
		if (arg1 == null) throw new ArgumentNullException("arg1");
		if (arg2 == null) throw new ArgumentNullException("arg2");
		if (arg3 == null) throw new ArgumentNullException("arg3");
		if (arg4 == null) throw new ArgumentNullException("arg4");
		if (arg5 == null) throw new ArgumentNullException("arg5");
		if (arg6 == null) throw new ArgumentNullException("arg6");
		g_format_args[0] = arg0;
		g_format_args[1] = arg1;
		g_format_args[2] = arg2;
		g_format_args[3] = arg3;
		g_format_args[4] = arg4;
		g_format_args[5] = arg5;
		g_format_args[6] = arg6;
		return internal_format(input, 7);
	}

	public static ZString Format(string input, ZString arg0, ZString arg1, ZString arg2, ZString arg3, ZString arg4, ZString arg5)
	{
		if (arg0 == null) throw new ArgumentNullException("arg0");
		if (arg1 == null) throw new ArgumentNullException("arg1");
		if (arg2 == null) throw new ArgumentNullException("arg2");
		if (arg3 == null) throw new ArgumentNullException("arg3");
		if (arg4 == null) throw new ArgumentNullException("arg4");
		if (arg5 == null) throw new ArgumentNullException("arg5");
		g_format_args[0] = arg0;
		g_format_args[1] = arg1;
		g_format_args[2] = arg2;
		g_format_args[3] = arg3;
		g_format_args[4] = arg4;
		g_format_args[5] = arg5;
		return internal_format(input, 6);
	}

	public static ZString Format(string input, ZString arg0, ZString arg1, ZString arg2, ZString arg3, ZString arg4)
	{
		if (arg0 == null) throw new ArgumentNullException("arg0");
		if (arg1 == null) throw new ArgumentNullException("arg1");
		if (arg2 == null) throw new ArgumentNullException("arg2");
		if (arg3 == null) throw new ArgumentNullException("arg3");
		if (arg4 == null) throw new ArgumentNullException("arg4");
		g_format_args[0] = arg0;
		g_format_args[1] = arg1;
		g_format_args[2] = arg2;
		g_format_args[3] = arg3;
		g_format_args[4] = arg4;
		return internal_format(input, 5);
	}

	public static ZString Format(string input, ZString arg0, ZString arg1, ZString arg2, ZString arg3)
	{
		if (arg0 == null) throw new ArgumentNullException("arg0");
		if (arg1 == null) throw new ArgumentNullException("arg1");
		if (arg2 == null) throw new ArgumentNullException("arg2");
		if (arg3 == null) throw new ArgumentNullException("arg3");
		g_format_args[0] = arg0;
		g_format_args[1] = arg1;
		g_format_args[2] = arg2;
		g_format_args[3] = arg3;
		return internal_format(input, 4);
	}

	public static ZString Format(string input, ZString arg0, ZString arg1, ZString arg2)
	{
		if (arg0 == null) throw new ArgumentNullException("arg0");
		if (arg1 == null) throw new ArgumentNullException("arg1");
		if (arg2 == null) throw new ArgumentNullException("arg2");
		g_format_args[0] = arg0;
		g_format_args[1] = arg1;
		g_format_args[2] = arg2;
		return internal_format(input, 3);
	}

	public static ZString Format(string input, ZString arg0, ZString arg1)
	{
		if (arg0 == null) throw new ArgumentNullException("arg0");
		if (arg1 == null) throw new ArgumentNullException("arg1");
		g_format_args[0] = arg0;
		g_format_args[1] = arg1;
		return internal_format(input, 2);
	}

	public static ZString Format(string input, ZString arg0)
	{
		if (arg0 == null) throw new ArgumentNullException("arg0");
		g_format_args[0] = arg0;
		return internal_format(input, 1);
	}

	// 普通的float->string是隐式转换，小数点后只保留三位有效数字
	// 对于更高精确度需求，隐式转换，可以修改静态变量DecimalAccuracy
	// 显式转换使用此方法即可，函数结束DecimalAccuracy值和之前的一样
	public static ZString FloatToZstring(float value, uint DecimalAccuracy)
	{
		uint oldValue = ZString.DecimalAccuracy;
		ZString.DecimalAccuracy = DecimalAccuracy;
		ZString target = value;
		ZString.DecimalAccuracy = oldValue;
		return target;
	}

	//判空或长度
	public static bool IsNullOrEmpty(ZString str)
	{
		return str == null || str.Length == 0;
	}

	//是否以value结束
	public static bool IsPrefix(ZString str, string value)
	{
		return str.StartsWith(value);
	}

	//是否以value开始
	public static bool isPostfix(ZString str, string postfix)
	{
		return str.EndsWith(postfix);
	}

	#endregion
}