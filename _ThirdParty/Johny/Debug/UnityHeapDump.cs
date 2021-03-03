using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Johny
{
	public class UnityHeapDump
	{
		const int ROOT_MIN_SIZE = 100;
		const int CHILD_MIN_SIZE = 1;

		static HashSet<Type> genericTypes = new HashSet<Type>();

		public static void Dump ()
		{
			TypeData.Start();
			List<KeyValuePair<Type, Exception>> parseErrors = new List<KeyValuePair<Type, Exception>>();

			string time = DateTime.Now.ToString().Replace('/', '_').Replace(':', '_');
			string outPutFile = "E://LT_SnapShot/MonoHeap/" + time + ".txt";
			using(var sw = new StreamWriter(outPutFile, false))
			{
				sw.WriteLine("====================Mono Info=====================");
				long monoReserved = UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong();
				long monoUsed = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();
				sw.WriteLine($"monoReserved: {monoReserved / (1024*1024)}MB");
				sw.WriteLine($"monoUsed: {monoUsed / (1024*1024)}MB");

				sw.WriteLine("====================Heap Details=====================");
				foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					foreach (var type in assembly.GetTypes())
					{
						try
						{
							new StructOrClass(type, sw);
						}
						catch (Exception e)
						{
							parseErrors.Add (new KeyValuePair<Type, Exception>(type, e));
						}
					}
				}
			}

			TypeData.Clear();
		}

		class StructOrClass 
		{
			public int Size { get; private set; }
			public Type ParsedType { get; private set; }
			int ArraySize { get; set; }
			string Identifier { get; set; }

			List<StructOrClass> Children = new List<StructOrClass>();

			/// <summary>
			/// Parse static types
			/// </summary>
			public StructOrClass (Type type, StreamWriter writer)
			{
				ParsedType = type;
				HashSet<object> seenObjects = new HashSet<object>();
				Identifier = type.FullName;
				foreach (var fieldInfo in type.GetFields())
				{
					ParseField(fieldInfo, null, seenObjects);
				}
				if (Size < ROOT_MIN_SIZE)
				{
					return;
				}
				
				string parsedType = ParsedType.ToString();
				if(parsedType.Contains("Hotfix_LT"))
				{
					writer.WriteLine($"{parsedType}: {Size} bytes");
					Children.Sort((a, b) => b.Size - a.Size);
					string childIndent = "    ";
					foreach (var child in Children)
					{
						if (child.Size >= CHILD_MIN_SIZE)
						{
							child.Write(writer, childIndent);
						} else
						{
							break;
						}
					}	
				}
			}


			/// <summary>
			/// Parse field objects; only called for arrays, references and structs with references in them
			/// </summary>
			StructOrClass(string name, object root, TypeData rootTypeData, HashSet<object> seenObjects)
			{
				Identifier = name;
				ParsedType = root.GetType();
				Size = rootTypeData.Size;
				if (ParsedType.IsArray)
				{
					int i = 0;
					ArraySize = GetTotalLength((Array)root);
					Type elementType = ParsedType.GetElementType();
					TypeData elementTypeData = TypeData.Get(elementType);
					if (elementType.IsValueType || elementType.IsPrimitive || elementType.IsEnum)
					{
						if (elementTypeData.DynamicSizedFields == null)
						{
							Size += elementTypeData.Size * ArraySize;
							return;
						}

						foreach (var item in (Array)root)
						{
							StructOrClass child = new StructOrClass((i++).ToString(), item, elementTypeData, seenObjects);
							Size += child.Size;
							Children.Add(child);
						}
					}
					else
					{
						Size += IntPtr.Size * ArraySize;
						foreach (var item in (Array)root)
						{
							ParseItem(item, (i++).ToString(), seenObjects);
						}
					}
				} else
				{
					if (rootTypeData.DynamicSizedFields != null)
					{
						foreach (var fieldInfo in rootTypeData.DynamicSizedFields)
						{
							ParseField(fieldInfo, root, seenObjects);
						}
					}
				}
			}

			/// <summary>
			/// Parse the field of the object, ignoring any seenObjects. If root is null, it is considered a static field.
			/// </summary>
			void ParseField(FieldInfo fieldInfo, object root, HashSet<object> seenObjects)
			{
				if (!fieldInfo.FieldType.IsPointer)
				{
					ParseItem(fieldInfo.GetValue(root), fieldInfo.Name, seenObjects);
				}
			}

			void ParseItem (object obj, string objName, HashSet<object> seenObjects)
			{
				if (obj == null)
				{
					return;
				}
				Type type = obj.GetType();
				if (type.IsPointer)
				{
					return; // again, a pointer cast to whatever the fieldtype is, shoo.
				}
				if (type == typeof(string))
				{
					// string needs special handling
					int strSize = 3 * IntPtr.Size + 2;
					strSize += ((string)(obj)).Length * sizeof(char);
					int pad = strSize % IntPtr.Size;
					if (pad != 0)
					{
						strSize += IntPtr.Size - pad;
					}
					Size += strSize;
					return;
				}
				// obj is not null, and a primitive/enum/array/struct/class
				TypeData fieldTypeData = TypeData.Get(type);
				if (type.IsClass || type.IsArray || fieldTypeData.DynamicSizedFields != null)
				{
					// class, array, or struct with pointers
					if (!(type.IsPrimitive || type.IsValueType || type.IsEnum))
					{
						if (!seenObjects.Add(obj))
						{
							return;
						}
					}

					StructOrClass child = new StructOrClass(objName, obj, fieldTypeData, seenObjects);
					Size += child.Size;
					Children.Add(child);
				}
			}

			void Write(StreamWriter writer, string indent)
			{
				if (ParsedType.IsArray)
				{
					writer.WriteLine("{0}{1} ({2}:{3}) : {4}", indent, Identifier, ParsedType, ArraySize, Size);
				} 
				else
				{
					writer.WriteLine("{0}{1} ({2}) : {3}", indent, Identifier, ParsedType, Size);
				}
			}

			static int GetTotalLength(Array val)
			{
				int sum = val.GetLength(0);
				for (int i = 1; i < val.Rank; i++)
				{
					sum *= val.GetLength(i);
				}
				return sum;
			}
		}

		public class TypeData
		{
			public int Size { get; private set; }
			public List<FieldInfo> DynamicSizedFields { get; private set; }

			static Dictionary<Type, TypeData> seenTypeData;
			static Dictionary<Type, TypeData> seenTypeDataNested;

			public static void Clear()
			{
				seenTypeData = null;
			}

			public static void Start()
			{
				seenTypeData = new Dictionary<Type, TypeData>();
				seenTypeDataNested = new Dictionary<Type, TypeData>();
			}

			public static TypeData Get(Type type)
			{
				TypeData data;
				if (!seenTypeData.TryGetValue(type, out data))
				{
					data = new TypeData(type);
					seenTypeData[type] = data;
				}
				return data;
			}

			public static TypeData GetNested(Type type)
			{
				TypeData data;
				if (!seenTypeDataNested.TryGetValue(type, out data))
				{
					data = new TypeData(type, true);
					seenTypeDataNested[type] = data;
				}
				return data;
			}

			public TypeData(Type type, bool nested = false)
			{
				if (type.IsGenericType)
				{
					genericTypes.Add(type);
				}
				Type baseType = type.BaseType;
				if (baseType != null
					&& baseType != typeof(object)
					&& baseType != typeof(ValueType)
					&& baseType != typeof(Array)
					&& baseType != typeof(Enum))
				{
					TypeData baseTypeData = GetNested(baseType);
					Size += baseTypeData.Size;

					if (baseTypeData.DynamicSizedFields != null)
					{
						DynamicSizedFields = new List<FieldInfo>(baseTypeData.DynamicSizedFields);
					}
				}
				if (type.IsPointer)
				{
					Size = IntPtr.Size;
				}
				else if (type.IsArray)
				{
					Type elementType = type.GetElementType();
					Size = ((elementType.IsValueType || elementType.IsPrimitive || elementType.IsEnum) ? 3 : 4) * IntPtr.Size;
				}
				else if (type.IsPrimitive)
				{
					Size = Marshal.SizeOf(type);
				}
				else if (type.IsEnum)
				{
					Size = Marshal.SizeOf(Enum.GetUnderlyingType(type));
				}
				else // struct, class
				{
					if (!nested && type.IsClass)
					{
						Size = 2 * IntPtr.Size;
					}
					foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
					{
						ProcessField(field, field.FieldType);
					}
					if (!nested && type.IsClass)
					{
						Size = Math.Max(3 * IntPtr.Size, Size);
						int pad = Size % IntPtr.Size;
						if (pad != 0)
						{
							Size += IntPtr.Size - pad;
						}
					}
				}
			}

			void ProcessField(FieldInfo field, Type fieldType)
			{
				if (IsStaticallySized(fieldType))
				{
					Size += GetStaticSize(fieldType);
				}
				else
				{
					if (!(fieldType.IsValueType || fieldType.IsPrimitive || fieldType.IsEnum))
					{
						Size += IntPtr.Size;
					}
					if (fieldType.IsPointer)
					{
						return;
					}
					if (DynamicSizedFields == null)
					{
						DynamicSizedFields = new List<FieldInfo>();
					}
					DynamicSizedFields.Add(field);
				}
			}

			static bool IsStaticallySized(Type type)
			{

				if (type.IsPointer || type.IsArray || type.IsClass || type.IsInterface)
				{
					return false;
				}
				if (type.IsPrimitive || type.IsEnum)
				{
					return true;
				}
				foreach (var nestedField in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (!IsStaticallySized(nestedField.FieldType))
					{
						return false;
					}
				}
				return true;
			}

			/// <summary>
			/// Gets size of type. Assumes IsStaticallySized (type) is true. (primitive, enum, {struct or class with no references in it})
			/// </summary>
			static int GetStaticSize(Type type)
			{
				if (type.IsPrimitive)
				{
					return Marshal.SizeOf(type);
				}
				if (type.IsEnum)
				{
					return Marshal.SizeOf(Enum.GetUnderlyingType(type));
				}
				int size = 0;
				foreach (var nestedField in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					size += GetStaticSize(nestedField.FieldType);
				}
				return size;
			}
		}
	}
}