using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
    public static class DataCacheUtil
    {
        public static IVersionedDataCacheSerializer GetSerializer(System.Type type)
        {
            if (!typeof(IVersionedDataCache).IsAssignableFrom(type))
            {
                EB.Debug.LogError("DataCacheUtil.GetSerializer: {0} is not IVersionedDataCache", type.ToString());
                return null;
            }

            object[] attributes = type.GetCustomAttributes(typeof(SerializerAttribute), false);
            if (attributes.Length < 1)
            {
                EB.Debug.LogError("DataCacheUtil.GetSerializer: {0} can't find SerializerAttribute", type.ToString());
                return null;
            }

            UnityEngine.Debug.Assert(attributes.Length == 1 && attributes[0] is SerializerAttribute,
                string.Format("DataCacheUtil.GetSerializer: multiple SerializerAttribute found in {0}", type.ToString()));

            SerializerAttribute attribute = attributes[0] as SerializerAttribute;
            return System.Activator.CreateInstance(attribute.SerializerType) as IVersionedDataCacheSerializer;
        }

        public static IVersionedDataCacheSerializer GetSerializer<T>()
        {
            return GetSerializer(typeof(T));
        }

        public static void ClearDataCache()
        {
            string path = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "Caches");
            if (System.IO.Directory.Exists(path))
            {
                System.IO.Directory.Delete(path, true);
                EB.Debug.Log("DataBase cache cleared: " + path);
            }
        }
    }
}