#if UNITY_EDITOR || ENABLE_PROFILER
#define USE_DEBUG_SIGNING
#endif
using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	public static class Protect
	{
		static string _sha1 = "";
		
#if USE_DEBUG_SIGNING
		static byte[] kDebugData =  EB.Encoding.GetBytes("WZ^?J{Y7Pw!!WeFDt!hH!|M|ASMTX+o9~31t{^0%gkZAqB:+NHRt03fLQcyr43(S");
#endif

		public static IEnumerator CalculateSha1( System.Action<string> cb )
		{
			if (string.IsNullOrEmpty(_sha1) == false) {
				cb(_sha1);
				yield break;
			}
			
	#if USE_DEBUG_SIGNING
			// editor uses preshared data
			_sha1 = EB.Encoding.ToBase64String(EB.Digest.Sha1().Hash(kDebugData));	
	#endif
			
			cb(_sha1);
			
			yield break;
		}
		
		public static IEnumerator CalculateHmac( string salt, System.Action<string> cb )
		{
			string chal = "";

	#if USE_DEBUG_SIGNING
			// editor uses preshared secret
			var key = new Key(salt + "." + "vf+EUR02fpJu0Vz2YJvwyBXI6EY=" );
			chal = EB.Encoding.ToBase64String(EB.Hmac.Sha1(key.Value).Hash(kDebugData));	
	#endif
			
			cb(chal);
			
			yield break;
		}
	}
}

