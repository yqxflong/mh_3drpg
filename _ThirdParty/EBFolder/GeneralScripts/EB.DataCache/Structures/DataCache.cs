using System.IO;

namespace EB.Sparx
{
    public interface IVersionedDataCache : System.IDisposable
    {
        string Version { get; set; }
    }

    public interface IVersionedDataCacheSerializer
    {
        void Serialize(Stream stream, IVersionedDataCache data_cache_obj);
        IVersionedDataCache Deserialize(Stream stream);
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SerializerAttribute : System.Attribute
    {
        public System.Type SerializerType { get; set; }

        public SerializerAttribute(System.Type serializer_type)
        {
            SerializerType = serializer_type;
        }
    }

    [Serializer(typeof(DefaultDataCacheSerializer))]
    public class DefaultDataCache : IVersionedDataCache
    {
        public string Version { get; set; }
        public string Content { get; set; }

        public DefaultDataCache()
        {
            Version = string.Empty;
            Content = string.Empty;
        }

        public DefaultDataCache(string version, string content)
        {
            Version = version;
            Content = content;
        }

        public void Dispose()
        {
            Version = string.Empty;
            Content = string.Empty;
        }
    }

    public class DefaultDataCacheSerializer : IVersionedDataCacheSerializer
    {
        public IVersionedDataCache Deserialize(Stream stream)
        {
            UnityEngine.Debug.Assert(stream != null && stream.CanRead,
                "stream is null or can not read");

            DefaultDataCache data = new DefaultDataCache();
            using (TextReader reader = new StreamReader(stream))
            {
                data.Version = reader.ReadLine();
                data.Content = reader.ReadToEnd();
            }

            return data;
        }

        public void Serialize(Stream stream, IVersionedDataCache data_cache_obj)
        {
            UnityEngine.Debug.Assert(data_cache_obj != null && data_cache_obj is DefaultDataCache,
                "data_cache_obj is null or is not DefaultDataCache");
            UnityEngine.Debug.Assert(stream != null && stream.CanWrite,
                "stream is null or can not write");

            DefaultDataCache data = data_cache_obj as DefaultDataCache;
            using (TextWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine(data.Version);
                writer.Write(data.Content);
            }                
        }
    }
}