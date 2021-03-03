//SimpleJSON
//改装
//Johny

#define USE_FileIO

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Johny
{
	public interface IEBPooledObject {
		void OnEnterPool ();
	}

	public enum JSONNodeType
	{
		Array = 1,
		Object = 2,
		String = 3,
		Number = 4,
		NullValue = 5,
		Boolean = 6,
		None = 7,
	}

	public abstract class JSONNode : IEBPooledObject
	{
		#region common interface

		public virtual void Clear(){

		}

		public virtual JSONNode Clone(){
			throw new MethodAccessException("JSONNode不可以直接Clone()!");
		}

		public virtual JSONNode this[int aIndex] { get { return null; } set { } }

		public virtual JSONNode this[string aKey] { get { return null; } set { } }

		public virtual string Value { get { return ""; } set { } }

		public virtual int Count { get { return 0; } }

		public virtual bool IsNumber { get { return false; } }
		public virtual bool IsString { get { return false; } }
		public virtual bool IsBoolean { get { return false; } }
		public virtual bool IsNull { get { return false; } }
		public virtual bool IsArray { get { return false; } }
		public virtual bool IsObject { get { return false; } }

		public virtual void Add(string aKey, string s){
			var jnode = JSONNodePool.Claim(typeof(JSONString));
			if(jnode == null){
				jnode = new JSONString();
			}
			jnode.InitString(s);
			this.Add(aKey, jnode);
		}

		public virtual void Add(string aKey, JSONNode aItem)
		{
		}

		public virtual void Add(JSONNode aItem)
		{
			Add("", aItem);
		}

		public virtual JSONNode Remove(string aKey)
		{
			return null;
		}

		public virtual JSONNode Remove(int aIndex)
		{
			return null;
		}

		public virtual JSONNode Remove(JSONNode aNode)
		{
			return aNode;
		}

		#endregion common interface

		#region typecasting properties
		public abstract JSONNodeType Tag { get; }

		public virtual double AsDouble
		{
			get
			{
				double v = 0.0;
				if (double.TryParse(Value, out v))
					return v;
				return 0.0;
			}
			set
			{
				Value = value.ToString();
			}
		}

		public virtual int AsInt
		{
			get { return (int)AsDouble; }
			set { AsDouble = value; }
		}

		public virtual long AsLong{
			get { return (long)AsDouble; }
			set { AsDouble = value; }
		}

		public virtual uint AsUInt
		{
			get{ return (uint)AsDouble; }
			set { AsDouble = value; }
		}

		public virtual float AsFloat
		{
			get { return (float)AsDouble; }
			set { AsDouble = value; }
		}

		public virtual bool AsBool
		{
			get
			{
				bool v = false;
				if (bool.TryParse(Value, out v))
					return v;
				return !string.IsNullOrEmpty(Value);
			}
			set
			{
				Value = (value) ? "true" : "false";
			}
		}

		public virtual JSONArray AsArray
		{
			get
			{
				return this as JSONArray;
			}
		}

		public virtual JSONObject AsObject
		{
			get
			{
				return this as JSONObject;
			}
		}

		public virtual JSONString AsString
		{
			get{
				return this as JSONString;
			}
		}

		public virtual void InitString(string s)
		{
			EB.Debug.LogError("I'm not JSONString");
		}
		#endregion typecasting properties

		#region operators
		public static implicit operator string(JSONNode d)
		{
			return (d == null) ? null : d.Value;
		}

		public static implicit operator JSONNode(double n)
		{
			return new JSONNumber(n);
		}
		public static implicit operator double(JSONNode d)
		{
			return (d == null) ? 0 : d.AsDouble;
		}

		public static implicit operator JSONNode(float n)
		{
			return new JSONNumber(n);
		}
		public static implicit operator float(JSONNode d)
		{
			return (d == null) ? 0 : d.AsFloat;
		}

		public static implicit operator JSONNode(int n)
		{
			return new JSONNumber(n);
		}
		public static implicit operator int(JSONNode d)
		{
			return (d == null) ? 0 : d.AsInt;
		}

		public static implicit operator JSONNode(bool b)
		{
			return new JSONBool(b);
		}
		public static implicit operator bool(JSONNode d)
		{
			return (d == null) ? false : d.AsBool;
		}

		public static bool operator ==(JSONNode a, object b)
		{
			if (ReferenceEquals(a, b))
				return true;
			bool aIsNull = a is JSONNull || ReferenceEquals(a, null) || a is JSONLazyCreator;
			bool bIsNull = b is JSONNull || ReferenceEquals(b, null) || b is JSONLazyCreator;
			return (aIsNull && bIsNull);
		}

		public static bool operator !=(JSONNode a, object b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return System.Object.ReferenceEquals(this, obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#endregion operators
		
		#region string内部的一些字符需要增加转义
		[ThreadStatic]
		private static StringBuilder m_EscapeBuilder;
		internal static StringBuilder EscapeBuilder
		{
			get {
				if (m_EscapeBuilder == null)
					m_EscapeBuilder = new StringBuilder();
				return m_EscapeBuilder;
			}
		}
		internal static string Escape(string aText)
		{
			var sb = EscapeBuilder;
			sb.Length = 0;
			if (sb.Capacity < aText.Length + aText.Length / 10)
				sb.Capacity = aText.Length + aText.Length / 10;
			foreach (char c in aText)
			{
				switch (c)
				{
					case '\\':
						sb.Append("\\\\");
						break;
					case '\"':
						sb.Append("\\\"");
						break;
					case '\n':
						sb.Append("\\n");
						break;
					case '\r':
						sb.Append("\\r");
						break;
					case '\t':
						sb.Append("\\t");
						break;
					case '\b':
						sb.Append("\\b");
						break;
					case '\f':
						sb.Append("\\f");
						break;
					default:
						sb.Append(c);
						break;
				}
			}
			string result = sb.ToString();
			sb.Length = 0;
			return result;
		}
		#endregion
		
		public virtual void OnEnterPool(){
			
		}

		static void ParseElement(JSONNode ctx, string token, string tokenName, bool quoted)
		{
			if (quoted)
			{
				ctx.Add(tokenName, token);
				return;
			}
			string tmp = token.ToLower();
			if (tmp == "false" || tmp == "true")
				ctx.Add(tokenName, tmp == "true");
			else if (tmp == "null")
				ctx.Add(tokenName, null);
			else
			{
				double val;
				if (double.TryParse(token, out val))
					ctx.Add(tokenName, val);
				else
					ctx.Add(tokenName, token);
			}
		}

		public static JSONNode Parse(string aJSON)
		{
			Stack<JSONNode> stack = new Stack<JSONNode>();
			JSONNode ctx = null;
			int i = 0;
			StringBuilder Token = new StringBuilder(1024);
			string TokenName = "";
			bool QuoteMode = false;
			bool TokenIsQuoted = false;
			while (i < aJSON.Length)
			{
				switch (aJSON[i])
				{
				case '{':
					if (QuoteMode)
					{
						Token.Append(aJSON[i]);
						break;
					}

					var jobject = Johny.JSONNodePool.Claim(typeof(JSONObject));
					stack.Push(jobject);
					if (ctx != null)
					{
						ctx.Add(TokenName, stack.Peek());
					}
					TokenName = "";
					Token.Length = 0;
					ctx = stack.Peek();
					break;

				case '[':
					if (QuoteMode)
					{
						Token.Append(aJSON[i]);
						break;
					}

					var jarray = Johny.JSONNodePool.Claim(typeof(JSONArray));
					stack.Push(jarray);
					if (ctx != null)
					{
						ctx.Add(TokenName, stack.Peek());
					}
					TokenName = "";
					Token.Length = 0;
					ctx = stack.Peek();
					break;

				case '}':
				case ']':
					if (QuoteMode)
					{

						Token.Append(aJSON[i]);
						break;
					}
					if (stack.Count == 0)
						throw new Exception("JSON Parse: Too many closing brackets");

					stack.Pop();
					if (Token.Length > 0)
					{
						ParseElement(ctx, Token.ToString(), TokenName, TokenIsQuoted);
						TokenIsQuoted = false;
					}
					TokenName = "";
					Token.Length = 0;
					if (stack.Count > 0)
						ctx = stack.Peek();
					break;

				case ':':
					if (QuoteMode)
					{
						Token.Append(aJSON[i]);
						break;
					}
					TokenName = Token.ToString().Trim();
					Token.Length = 0;
					TokenIsQuoted = false;
					break;

				case '"':
					QuoteMode ^= true;
					TokenIsQuoted |= QuoteMode;
					break;

				case ',':
					if (QuoteMode)
					{
						Token.Append(aJSON[i]);
						break;
					}
					if (Token.Length > 0)
					{
						ParseElement(ctx, Token.ToString(), TokenName, TokenIsQuoted);
						TokenIsQuoted = false;
					}
					TokenName = "";
					Token.Length = 0;
					TokenIsQuoted = false;
					break;

				case '\r':
				case '\n':
					break;

				case ' ':
				case '\t':
					if (QuoteMode)
						Token.Append(aJSON[i]);
					break;

				case '\\':
					++i;
					if (QuoteMode)
					{
						char C = aJSON[i];
						switch (C)
						{
						case 't':
							Token.Append('\t');
							break;
						case 'r':
							Token.Append('\r');
							break;
						case 'n':
							Token.Append('\n');
							break;
						case 'b':
							Token.Append('\b');
							break;
						case 'f':
							Token.Append('\f');
							break;
						case 'u':
							{
								string s = aJSON.Substring(i + 1, 4);
								Token.Append((char)int.Parse(
									s,
									System.Globalization.NumberStyles.AllowHexSpecifier));
								i += 4;
								break;
							}
						default:
							Token.Append(C);
							break;
						}
					}
					break;

				default:
					Token.Append(aJSON[i]);
					break;
				}
				++i;
			}
			if (QuoteMode)
			{
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}
			return ctx;
		}


		#region 序列化
		public virtual void Serialize(System.IO.BinaryWriter aWriter)
		{
		}

		public void SaveToStream(System.IO.Stream aData)
		{
			var W = new System.IO.BinaryWriter(aData);
			Serialize(W);
		}

		public void SaveToFile(string aFileName)
		{
			#if USE_FileIO
			System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
			using (var F = System.IO.File.OpenWrite(aFileName))
			{
				SaveToStream(F);
			}
			#else
			throw new Exception ("Can't use File IO stuff in the webplayer");
			#endif
		}

		public string SaveToBase64()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				SaveToStream(stream);
				stream.Position = 0;
				return System.Convert.ToBase64String(stream.ToArray());
			}
		}

		public override string ToString()
		{
			return "JSONNode";
		}

		/// <summary>
		/// 序列化为人能看懂的标准json字符串格式
		/// </summary>
		/// <returns></returns>
		public virtual void ToJSONFormatString(StringBuilder sb){
			sb.Append("JSONNode");
		}
		#endregion

		#region 反序列化
		public static JSONNode Deserialize(System.IO.BinaryReader aReader)
		{
			JSONNodeType type = (JSONNodeType)aReader.ReadByte();
			switch (type)
			{
			case JSONNodeType.Array:
				{
					int count = aReader.ReadInt32();
					JSONArray tmp = Johny.JSONNodePool.Claim(typeof(JSONArray)).AsArray;
					for (int i = 0; i < count; i++)
						tmp.Add(Deserialize(aReader));
					return tmp;
				}
			case JSONNodeType.Object:
				{
					int count = aReader.ReadInt32();
					JSONObject tmp = Johny.JSONNodePool.Claim(typeof(JSONObject)).AsObject;
					for (int i = 0; i < count; i++)
					{
						string key = aReader.ReadString();
						var val = Deserialize(aReader);
						tmp.Add(key, val);
					}
					return tmp;
				}
			case JSONNodeType.String:
				{
					JSONString jstring = Johny.JSONNodePool.Claim(typeof(JSONString)) as JSONString;
					jstring.InitString(aReader.ReadString());
					return jstring;
				}
			case JSONNodeType.Number:
				{
					return new JSONNumber(aReader.ReadDouble());
				}
			case JSONNodeType.Boolean:
				{
					return new JSONBool(aReader.ReadBoolean());
				}
			case JSONNodeType.NullValue:
				{
					return new JSONNull();
				}
			default:
				{
					throw new Exception("Error deserializing JSON. Unknown tag: " + type);
				}
			}
		}

		public static JSONNode LoadFromStream(System.IO.Stream aData)
		{
			using (var R = new System.IO.BinaryReader(aData))
			{
				return Deserialize(R);
			}
		}

		public static JSONNode LoadFromFile(string aFileName)
		{
			#if USE_FileIO
			using (var F = System.IO.File.OpenRead(aFileName))
			{
				return LoadFromStream(F);
			}
			#else
			throw new Exception ("Can't use File IO stuff in the webplayer");
			#endif
		}

		public static JSONNode LoadFromBase64(string aBase64)
		{
			var tmp = System.Convert.FromBase64String(aBase64);
			var stream = new System.IO.MemoryStream(tmp);
			stream.Position = 0;
			return LoadFromStream(stream);
		}
		#endregion
	}
	// End of JSONNode

	public class JSONArray : JSONNode
	{
		private List<JSONNode> m_List = new List<JSONNode>();

		public override JSONNodeType Tag { get { return JSONNodeType.Array; } }
		public override bool IsArray { get { return true; } }

		public override void OnEnterPool(){
			m_List.Clear();
		}

		public override JSONNode Clone(){
			JSONArray newOne = new JSONArray();
			for(int i = 0; i < m_List.Count; i++){
				newOne.Add(m_List[i].Clone());
			}

			return newOne;
		}

		public override JSONNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= m_List.Count)
					return new JSONLazyCreator(this);
				return m_List[aIndex];
			}
			set
			{
				if (value == null)
					value = new JSONNull();
				if (aIndex < 0 || aIndex >= m_List.Count)
					m_List.Add(value);
				else
					m_List[aIndex] = value;
			}
		}

		public override JSONNode this[string aKey]
		{
			get { return new JSONLazyCreator(this); }
			set
			{
				if (value == null)
					value = new JSONNull();
				m_List.Add(value);
			}
		}

		public override int Count
		{
			get { return m_List.Count; }
		}

		public override void Add(string aKey, JSONNode aItem)
		{
			if (aItem == null)
				aItem = new JSONNull();
			m_List.Add(aItem);
		}

		public override JSONNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= m_List.Count)
				return null;
			JSONNode tmp = m_List[aIndex];
			m_List.RemoveAt(aIndex);
			return tmp;
		}

		public override JSONNode Remove(JSONNode aNode)
		{
			m_List.Remove(aNode);
			return aNode;
		}

		public override void Serialize(System.IO.BinaryWriter aWriter)
		{
			aWriter.Write((byte)JSONNodeType.Array);
			aWriter.Write(m_List.Count);
			for (int i = 0; i < m_List.Count; i++)
			{
				m_List[i].Serialize(aWriter);
			}
		}
	

		public override string ToString()
		{
			return "JSONArray";
		}


		public override void ToJSONFormatString(StringBuilder sb){
			sb.Append("[ ");
			for(int i = 0; i < m_List.Count; i++){
				m_List[i].ToJSONFormatString(sb);
				if(i <= m_List.Count - 2){
					sb.Append(", ");
				}
			}
			sb.Append(" ]");
		}
	}
	// End of JSONArray

	public class JSONObject : JSONNode
	{
		private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();
		public override JSONNodeType Tag { get { return JSONNodeType.Object; } }
		public override bool IsObject { get { return true; } }

		public override void OnEnterPool(){
			m_Dict.Clear();
		}

		public override void Clear(){
			m_Dict.Clear();
		}

		public override JSONNode Clone(){
			JSONObject newOne = new JSONObject();
			foreach(var it in m_Dict)
			{
				newOne.Add(it.Key, it.Value.Clone());
			}

			return newOne;
		}

		public bool Contains(string key){
			JSONNode ret;
			return m_Dict.TryGetValue(key, out ret);
		}
		public override JSONNode this[string aKey]
		{
			get
			{
				if (m_Dict.ContainsKey(aKey))
					return m_Dict[aKey];
				else
					return new JSONLazyCreator(this, aKey);
			}
			set
			{
				if (value == null)
					value = new JSONNull();
				if (m_Dict.ContainsKey(aKey))
					m_Dict[aKey] = value;
				else
					m_Dict.Add(aKey, value);
			}
		}

		public override JSONNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= m_Dict.Count)
					return null;
				return m_Dict.ElementAt(aIndex).Value;
			}
			set
			{
				if (value == null)
					value = new JSONNull();
				if (aIndex < 0 || aIndex >= m_Dict.Count)
					return;
				string key = m_Dict.ElementAt(aIndex).Key;
				m_Dict[key] = value;
			}
		}

		public override int Count
		{
			get { return m_Dict.Count; }
		}

		public override void Add(string aKey, JSONNode aItem)
		{
			if (aItem == null)
				aItem = new JSONNull();

			if (!string.IsNullOrEmpty(aKey))
			{
				if (m_Dict.ContainsKey(aKey))
					m_Dict[aKey] = aItem;
				else
					m_Dict.Add(aKey, aItem);
			}
			else
				m_Dict.Add(Guid.NewGuid().ToString(), aItem);
		}

		public override JSONNode Remove(string aKey)
		{
			if (!m_Dict.ContainsKey(aKey))
				return null;
			JSONNode tmp = m_Dict[aKey];
			m_Dict.Remove(aKey);
			return tmp;
		}

		public override JSONNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= m_Dict.Count)
				return null;
			var item = m_Dict.ElementAt(aIndex);
			m_Dict.Remove(item.Key);
			return item.Value;
		}

		public override JSONNode Remove(JSONNode aNode)
		{
			try
			{
				var item = m_Dict.Where(k => k.Value == aNode).First();
				m_Dict.Remove(item.Key);
				return aNode;
			}
			catch
			{
				return null;
			}
		}

		public IEnumerable<KeyValuePair<string, JSONNode>> Children
		{
			get
			{
				var it = m_Dict.GetEnumerator();
				while(it.MoveNext()){
					yield return it.Current;
				}
			}
		}

		public override void Serialize(System.IO.BinaryWriter aWriter)
		{
			aWriter.Write((byte)JSONNodeType.Object);
			aWriter.Write(m_Dict.Count);
			foreach (string K in m_Dict.Keys)
			{
				aWriter.Write(K);
				m_Dict[K].Serialize(aWriter);
			}
		}
	
		public override string ToString()
		{
			return "JSONObject";
		}

		public override void ToJSONFormatString(StringBuilder sb){
			sb.Append("{ ");
			var it = m_Dict.GetEnumerator();
			int i = 0;
			while(it.MoveNext()){
				sb.Append("\"");
				sb.Append(it.Current.Key);
				sb.Append("\":");
				it.Current.Value.ToJSONFormatString(sb);
				if(i <= m_Dict.Count - 2){
					sb.Append(", ");
				}
				i++;
			}
			sb.Append(" }");
		}
	}
	// End of JSONObject

	public class JSONString : JSONNode
	{
		private string m_Data = string.Empty;

		public override JSONNodeType Tag { get { return JSONNodeType.String; } }
		public override bool IsString { get { return true; } }

		public override string Value
		{
			get { return m_Data; }
			set
			{
				m_Data = value;
			}
		}

		public override void OnEnterPool(){
			m_Data = string.Empty;
		}

		public override JSONNode Clone(){
			JSONString newOne = Johny.JSONNodePool.Claim(typeof(JSONString)) as JSONString;
			newOne.InitString(m_Data);
			return newOne;
		}
		public override void InitString(string aString){
			m_Data = aString;
		}

		public override void Serialize(System.IO.BinaryWriter aWriter)
		{
			aWriter.Write((byte)JSONNodeType.String);
			aWriter.Write(m_Data);
		}

		public override string ToString()
		{
			return m_Data;
		}

		public override void ToJSONFormatString(StringBuilder sb){
			sb.Append("\"");
			sb.Append(Escape(m_Data));
			sb.Append("\"");
		}
	}
	// End of JSONString


	public class JSONNumber : JSONNode
	{
		private double m_Data;

		public override JSONNodeType Tag { get { return JSONNodeType.Number; } }
		public override bool IsNumber { get { return true; } }


		public override string Value
		{
			get { return m_Data.ToString(); }
			set
			{
				double v;
				if (double.TryParse(value, out v))
					m_Data = v;
			}
		}

		public override JSONNode Clone(){
			return new JSONNumber(m_Data);
		}

		public override double AsDouble
		{
			get { return m_Data; }
			set { m_Data = value; }
		}

		public JSONNumber(double aData)
		{
			m_Data = aData;
		}

		public JSONNumber(string aData)
		{
			Value = aData;
		}

		public override void Serialize(System.IO.BinaryWriter aWriter)
		{
			aWriter.Write((byte)JSONNodeType.Number);
			aWriter.Write(m_Data);
		}
	

		public override string ToString()
		{
			return m_Data.ToString();
		}
	
		public override void ToJSONFormatString(StringBuilder sb){
			sb.Append(m_Data);
		}
	}
	// End of JSONNumber


	public class JSONBool : JSONNode
	{
		private bool m_Data;

		public override JSONNodeType Tag { get { return JSONNodeType.Boolean; } }
		public override bool IsBoolean { get { return true; } }

		public override JSONNode Clone(){
			return new JSONBool(m_Data);
		}

		public override string Value
		{
			get { return m_Data.ToString(); }
			set
			{
				bool v;
				if (bool.TryParse(value, out v))
					m_Data = v;
			}
		}
		public override bool AsBool
		{
			get { return m_Data; }
			set { m_Data = value; }
		}

		public JSONBool(bool aData)
		{
			m_Data = aData;
		}

		public JSONBool(string aData)
		{
			Value = aData;
		}

		public override void Serialize(System.IO.BinaryWriter aWriter)
		{
			aWriter.Write((byte)JSONNodeType.Boolean);
			aWriter.Write(m_Data);
		}
	
		public override string ToString()
		{
			return (m_Data) ? "true" : "false";
		}

		public override void ToJSONFormatString(StringBuilder sb){
			sb.Append(ToString());
		}
	}
	// End of JSONBool


	public class JSONNull : JSONNode
	{
		public override JSONNodeType Tag { get { return JSONNodeType.NullValue; } }
		public override bool IsNull { get { return true; } }

		public override JSONNode Clone(){
			return new JSONNull();
		}

		public override string Value
		{
			get { return "null"; }
			set { }
		}
		public override bool AsBool
		{
			get { return false; }
			set { }
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			return (obj is JSONNull);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override void Serialize(System.IO.BinaryWriter aWriter)
		{
			aWriter.Write((byte)JSONNodeType.NullValue);
		}
	

		public override string ToString()
		{
			return "null";
		}

		public override void ToJSONFormatString(StringBuilder sb){
			sb.Append("null");
		}
	}
	// End of JSONNull

	internal class JSONLazyCreator : JSONNode
	{
		private JSONNode m_Node = null;
		private string m_Key = null;

		public override JSONNodeType Tag { get { return JSONNodeType.None; } }

		public JSONLazyCreator(JSONNode aNode)
		{
			m_Node = aNode;
			m_Key = null;
		}

		public JSONLazyCreator(JSONNode aNode, string aKey)
		{
			m_Node = aNode;
			m_Key = aKey;
		}

		private void Set(JSONNode aVal)
		{
			if (m_Key == null)
			{
				m_Node.Add(aVal);
			}
			else
			{
				m_Node.Add(m_Key, aVal);
			}
			m_Node = null; // Be GC friendly.
		}

		public override JSONNode this[int aIndex]
		{
			get
			{
				return new JSONLazyCreator(this);
			}
			set
			{
				var tmp = Johny.JSONNodePool.Claim(typeof(JSONArray));
				tmp.Add(value);
				Set(tmp);
			}
		}

		public override JSONNode this[string aKey]
		{
			get
			{
				return new JSONLazyCreator(this, aKey);
			}
			set
			{
				var tmp = Johny.JSONNodePool.Claim(typeof(JSONObject));
				tmp.Add(aKey, value);
				Set(tmp);
			}
		}

		public override void Add(JSONNode aItem)
		{
			var tmp = Johny.JSONNodePool.Claim(typeof(JSONArray));
			tmp.Add(aItem);
			Set(tmp);
		}

		public override void Add(string aKey, JSONNode aItem)
		{
			var tmp = Johny.JSONNodePool.Claim(typeof(JSONObject));
			tmp.Add(aKey, aItem);
			Set(tmp);
		}

		public static bool operator ==(JSONLazyCreator a, object b)
		{
			if (b == null)
				return true;
			return System.Object.ReferenceEquals(a, b);
		}

		public static bool operator !=(JSONLazyCreator a, object b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return true;
			return System.Object.ReferenceEquals(this, obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return "";
		}

		public override void ToJSONFormatString(StringBuilder sb){
			sb.Append("JSONLazyCreator");
		}

		public override int AsInt
		{
			get
			{
				JSONNumber tmp = new JSONNumber(0);
				Set(tmp);
				return 0;
			}
			set
			{
				JSONNumber tmp = new JSONNumber(value);
				Set(tmp);
			}
		}

		public override float AsFloat
		{
			get
			{
				JSONNumber tmp = new JSONNumber(0.0f);
				Set(tmp);
				return 0.0f;
			}
			set
			{
				JSONNumber tmp = new JSONNumber(value);
				Set(tmp);
			}
		}

		public override double AsDouble
		{
			get
			{
				JSONNumber tmp = new JSONNumber(0.0);
				Set(tmp);
				return 0.0;
			}
			set
			{
				JSONNumber tmp = new JSONNumber(value);
				Set(tmp);
			}
		}

		public override bool AsBool
		{
			get
			{
				JSONBool tmp = new JSONBool(false);
				Set(tmp);
				return false;
			}
			set
			{
				JSONBool tmp = new JSONBool(value);
				Set(tmp);
			}
		}

		public override JSONArray AsArray
		{
			get
			{
				var tmp = Johny.JSONNodePool.Claim(typeof(JSONArray)).AsArray;
				Set(tmp);
				return tmp;
			}
		}

		public override JSONObject AsObject
		{
			get
			{
				var tmp = Johny.JSONNodePool.Claim(typeof(JSONObject)).AsObject;
				Set(tmp);
				return tmp;
			}
		}
	}
	// End of JSONLazyCreator
}