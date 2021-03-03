using UnityEngine;
using System.Collections;

namespace GM
{
	public class JSON
	{
		private static JSON _instance = null;
		private static JSON Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new JSON();
					_instance.RegisterCustomExporter();
					_instance.RegisterCustomImporter();
				}
				return _instance;
			}
		}
        /// <summary>
        /// 对象序列化成json文本
        /// </summary>
        /// <param name="value">对象</param>
        /// <returns></returns>
		public static string ToJson(object value)
		{
			return Instance.ToJsonImpl(value);
		}
        /// <summary>
        /// JSON反序列化成对象
        /// </summary>
        /// <typeparam name="T">反序列化的泛型对象</typeparam>
        /// <param name="value">json字符串</param>
        /// <returns></returns>
		public static T ToObject<T>(string value)
		{
			return Instance.ToObjectImpl<T>(value);
		}
        /// <summary>
        /// 对象序列化成json文本
        /// </summary>
        /// <param name="value">对象</param>
        /// <returns></returns>
		private string ToJsonImpl(object value)
		{
			return LitJson.JsonMapper.ToJson(value);
		}

		private T ToObjectImpl<T>(string value)
		{
			return LitJson.JsonMapper.ToObject<T>(value);
		}

		private void RegisterCustomExporter()
		{
			LitJson.JsonMapper.RegisterExporter<float>(LitJsonExtension.ExportSingle);
			LitJson.JsonMapper.RegisterExporter<Vector2>(LitJsonExtension.ExportVector2);
			LitJson.JsonMapper.RegisterExporter<Vector3>(LitJsonExtension.ExportVector3);
			LitJson.JsonMapper.RegisterExporter<Vector4>(LitJsonExtension.ExportVector4);
			LitJson.JsonMapper.RegisterExporter<Quaternion>(LitJsonExtension.ExportQuaternion);
		}

		private void RegisterCustomImporter()
		{
			LitJson.JsonMapper.RegisterImporter<string, float>(LitJsonExtension.ImportSingle);
			LitJson.JsonMapper.RegisterImporter<string, Vector2>(LitJsonExtension.ImportVector2);
			LitJson.JsonMapper.RegisterImporter<string, Vector3>(LitJsonExtension.ImportVector3);
			LitJson.JsonMapper.RegisterImporter<string, Vector4>(LitJsonExtension.ImportVector4);
			LitJson.JsonMapper.RegisterImporter<string, Quaternion>(LitJsonExtension.ImportQuaternion);
		}
	}
}
