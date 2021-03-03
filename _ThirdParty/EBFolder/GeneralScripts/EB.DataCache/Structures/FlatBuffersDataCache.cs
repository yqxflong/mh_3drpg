using System.IO;

namespace EB.Sparx
{
	[Serializer(typeof(FlatBuffersDataCacheSerializer))]
	public class FlatBuffersDataCache : IVersionedDataCache
	{
		public string Version
		{
			get; set;
		}

		public System.ArraySegment<byte> Buffer
		{
			get; set;
		}

		public void Dispose()
		{
			
		}
	}

	public class FlatBuffersDataCacheSerializer : IVersionedDataCacheSerializer
	{
		const int VERSION_OFFSET = 32;

		public IVersionedDataCache Deserialize(Stream stream)
		{
			FlatBuffersDataCache data = new FlatBuffersDataCache();
			using (BinaryReader br = new BinaryReader(stream))
			{
				byte[] version = br.ReadBytes(VERSION_OFFSET);
				data.Version = EB.Encoding.GetString(version).TrimEnd(new char[] { (char)0 });

				using (var ms = new MemoryStream())
				{
					byte[] buffer = new byte[512];
					int count = 0;
					while ((count = br.Read(buffer, 0, buffer.Length)) != 0)
					{
						ms.Write(buffer, 0, count);
					}
					data.Buffer = new System.ArraySegment<byte>(ms.GetBuffer(), 0, (int)ms.Position);
				}
			}
			return data;
		}

		public void Serialize(Stream stream, IVersionedDataCache data_cache_obj)
		{
			FlatBuffersDataCache data = data_cache_obj as FlatBuffersDataCache;
			using (BinaryWriter bw = new BinaryWriter(stream))
			{
				byte[] version = EB.Encoding.GetBytes(data.Version);
				if (version.Length != VERSION_OFFSET)
				{
					EB.Debug.LogWarning("FlatBufferTableDataCacheSerializer.Serialize: resize invalid version {0}", data.Version);

					byte[] buffer = new byte[VERSION_OFFSET];
					System.Buffer.BlockCopy(version, 0, buffer, 0, System.Math.Min(version.Length, VERSION_OFFSET));
					version = buffer;
				}

				bw.Write(version);
				bw.Write(data.Buffer.Array, data.Buffer.Offset, data.Buffer.Count);
			}
		}
	}
}
