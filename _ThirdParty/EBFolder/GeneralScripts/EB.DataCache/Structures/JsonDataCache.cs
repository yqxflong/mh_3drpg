using System.Collections;
using System.IO;

namespace EB.Sparx
{
    [Serializer(typeof(JsonDataCacheSerializer))]
    public class JsonDataCache : IVersionedDataCache
    {
        public string Version { get; set; }
        public object Payload { get; set; }

        public JsonDataCache()
        {
            Version = string.Empty;
            Payload = null;
        }

        public JsonDataCache(string version, object json)
        {
            Version = version;
            Payload = json;
        }

        public void Dispose()
        {
            Version = string.Empty;
            Payload = null;
        }
    }

    public class JsonDataCacheSerializer : IVersionedDataCacheSerializer
    {
        public IVersionedDataCache Deserialize(Stream stream)
        {
            UnityEngine.Debug.Assert(stream != null && stream.CanRead,
                "stream is null or can not read");

            JsonDataCache data = new JsonDataCache();
            using (TextReader reader = new StreamReader(stream))
            {
                string content = reader.ReadToEnd();
                Hashtable ht = EB.JSON.Parse(content) as Hashtable;
                if (ht != null)
                {
                    data.Version = ht["version"].ToString();
                    data.Payload = ht["payload"];
                }
            }

            return data.Payload != null ? data : null;
        }

        public void Serialize(Stream stream, IVersionedDataCache data_cache_obj)
        {
            UnityEngine.Debug.Assert(data_cache_obj != null && data_cache_obj is JsonDataCache,
                "data_cache_obj is null or data_cache_obj is not JsonDataCache");
            UnityEngine.Debug.Assert(stream != null && stream.CanWrite,
                "stream is null or can not write");

            JsonDataCache data = data_cache_obj as JsonDataCache;
            Hashtable ht = Johny.HashtablePool.Claim();
            ht["version"] = data.Version;
            ht["payload"] = data.Payload;

            string content = EB.JSON.Stringify(ht);
            using (TextWriter writer = new StreamWriter(stream))
            {
                writer.Write(content);
            }
        }
    }
}

