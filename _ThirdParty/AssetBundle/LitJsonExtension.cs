using UnityEngine;
using System.Collections;

namespace GM
{
	public static class LitJsonExtension
	{
		public static void ExportSingle(float value, LitJson.JsonWriter writer)
		{
			writer.Write(value.ToString());
		}
		
		public static void ExportVector2(Vector2 value, LitJson.JsonWriter writer)
		{
			writer.Write(value.ToString());
		}
		
		public static void ExportVector3(Vector3 value, LitJson.JsonWriter writer)
		{
			writer.Write(value.ToString());
		}
		
		public static void ExportVector4(Vector4 value, LitJson.JsonWriter writer)
		{
			writer.Write(value.ToString());
		}
		
		public static void ExportQuaternion(Quaternion value, LitJson.JsonWriter writer)
		{
			writer.Write(value.ToString());
		}
		
		//////////////////////////////////////////////////////////////////////////////////////////////////
		
		public static float ImportSingle(string value)
		{
			return System.Convert.ToSingle(value);
		}
		
		private static char[] deli = new char[] { '(', ',', ')' };
		public static Vector2 ImportVector2(string value)
		{
			string[] strs = value.Split(deli, System.StringSplitOptions.RemoveEmptyEntries);
			return new Vector2(System.Convert.ToSingle(strs[0]), System.Convert.ToSingle(strs[1]));
		}
		
		public static Vector3 ImportVector3(string value)
		{
			string[] strs = value.Split(deli, System.StringSplitOptions.RemoveEmptyEntries);
			return new Vector3(System.Convert.ToSingle(strs[0]), System.Convert.ToSingle(strs[1]), System.Convert.ToSingle(strs[2]));
		}
		
		public static Vector4 ImportVector4(string value)
		{
			string[] strs = value.Split(deli, System.StringSplitOptions.RemoveEmptyEntries);
			return new Vector4(System.Convert.ToSingle(strs[0]), System.Convert.ToSingle(strs[1]), System.Convert.ToSingle(strs[2]), System.Convert.ToSingle(strs[3]));
		}
		
		public static Quaternion ImportQuaternion(string value)
		{
			string[] strs = value.Split(deli, System.StringSplitOptions.RemoveEmptyEntries);
			return new Quaternion(System.Convert.ToSingle(strs[0]), System.Convert.ToSingle(strs[1]), System.Convert.ToSingle(strs[2]), System.Convert.ToSingle(strs[3]));
		}
	}
}
