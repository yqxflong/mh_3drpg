using System.Collections.Generic;

namespace EB.Net
{
	using IPAddress = System.Net.IPAddress;
	using DnsBase = System.Net.Dns;
	using System.Runtime.InteropServices;
	// a dns cache that works well with Amazon's ELB
	public static class DNS
	{
		public static Dictionary<string, List<IPAddress>> s_fallbackDNS = new Dictionary<string, List<IPAddress>>();

		class Entry
		{
			public IPAddress        last;
			public System.DateTime  lastT;
			public IPAddress[]  addresses = new IPAddress[0];
		};
		static Dictionary<string,Entry> _entries = new Dictionary<string, Entry>();

		public static void LoadHosts(string assetPath)
		{
			UnityEngine.TextAsset hosts = UnityEngine.Resources.Load<UnityEngine.TextAsset>(assetPath);
			if (hosts == null)
			{
				EB.Debug.LogWarning("LoadHosts: asset not exists {0}", assetPath);
				return;
			}

			var lineSeprators = new string[] { "\r\n", "\r", "\n" };
			var lines = hosts.text.Split(lineSeprators, System.StringSplitOptions.RemoveEmptyEntries);
			var colSeprators = new char[] { ' ', '\t' };
			for (int i = 0; i < lines.Length; ++i)
			{
				string line = lines[i];
				if (line.TrimStart(colSeprators).StartsWith("#"))
				{
					continue;
				}

				var cols = line.Split(colSeprators, System.StringSplitOptions.RemoveEmptyEntries);
				if (cols.Length < 2)
				{
					EB.Debug.LogWarning("LoadHosts: invalid host {0}", line);
					continue;
				}

				string hostname = cols[0];
				// add entry
				if (!s_fallbackDNS.ContainsKey(hostname))
				{
					s_fallbackDNS.Add(hostname, new List<IPAddress>());
				}
				// add address
				for (int j = 1; j < cols.Length; ++j)
				{
					string address = cols[j];
					if (address.StartsWith("#"))
					{
						break;
					}
					s_fallbackDNS[hostname].Add(IPAddress.Parse(address));
				}
				// remove empty entry
				if (s_fallbackDNS[hostname].Count == 0)
				{
					s_fallbackDNS.Remove(hostname);
				}
			}

			UnityEngine.Resources.UnloadAsset(hosts);
		}

		static Entry GetEntry(string host)
		{
			lock (_entries)
			{
				Entry e;
				if (!_entries.TryGetValue(host, out e))
				{
					e = new Entry();
					_entries.Add(host, e);
				}
				return e;
			}
		}

		static IPAddress[] GetAddresses(string host)
		{
			lock (_entries)
			{
				Entry e;
				if (!_entries.TryGetValue(host, out e))
				{
					e = new Entry();
					_entries.Add(host, e);
				}

				return e.last != null ? (IPAddress[])e.addresses.Clone() : null;
			}
		}

		public static void StoreLast(string hostname, IPAddress addr)
		{
			var entry = GetEntry(hostname);
			lock (entry)
			{
				var index = System.Array.IndexOf(entry.addresses, addr);
				if (index >= 0)
				{
					entry.last = addr;
					entry.lastT = System.DateTime.Now;

					// move to the front
					var tmp = entry.addresses[0];
					entry.addresses[0] = entry.addresses[index];
					entry.addresses[index] = tmp;
				}
			}
		}

		public static void Clear(string hostname)
		{
			var entry = GetEntry(hostname);
			lock (entry)
			{
				entry.last = null;
				entry.addresses = new IPAddress[0];
			}
		}

		public static IPAddress[] Lookup(string hostname)
		{
			IPAddress addr;
			if (IPAddress.TryParse(hostname, out addr))
			{
				return ParseIpAddress(addr);
			}

			// safe read
			IPAddress[] addres = GetAddresses(hostname);
			if (addres != null)
			{
				return addres;
			}

			// try to get the host entries
			List<IPAddress> addresses = new List<IPAddress>();
			try
			{
				var async = DnsBase.BeginGetHostAddresses(hostname, null, null);
				if (async.AsyncWaitHandle.WaitOne(System.TimeSpan.FromSeconds(5)) && async.IsCompleted)
				{
					var tmp = DnsBase.EndGetHostAddresses(async);
					if (tmp != null)
					{
						// shuffle
						if (tmp.Length > 1)
						{
							System.Random rand = new System.Random();
							for (int i = 0; i < tmp.Length; ++i)
							{
								var t = tmp[i];
								int r = rand.Next(i, tmp.Length);
								tmp[i] = tmp[r];
								tmp[r] = t;
							}
						}


						// support ipv6
						if (TcpClientUtil.OSSupportsIPv6())
						{
							foreach (var ip in tmp)
							{
								if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
								{
									addresses.Add(ip);
								}
							}
						}

						// ipv4 
						if (TcpClientUtil.OSSupportsIPv4())
						{
							foreach (var ip in tmp)
							{
								if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
								{
									//Debug.Log("addr " + ip);
									addresses.Add(ip);
								}
							}
						}
					}
				}
			}
			catch (System.Exception ex)
			{
				// failure, 
				EB.Debug.LogError("failed to lookup " + hostname + " " + ex);
			}

			// fall back
			if (addresses.Count == 0 && s_fallbackDNS.ContainsKey(hostname))
			{
				EB.Debug.LogWarning("failed to lookup {0}, use fall back dns", hostname);
				addresses.AddRange(s_fallbackDNS[hostname]);
				return addresses.ToArray();
			}

			// not found
			if (addresses.Count == 0)
			{
				EB.Debug.LogError("failed to lookup {0}", hostname);
				return new IPAddress[0];
			}

			// safe update
			Entry e = GetEntry(hostname);
			lock (e)
			{
				e.addresses = addresses.ToArray();
				// keep last in first
				if (e.last != null)
				{
					var index = System.Array.IndexOf(e.addresses, e.last);
					if (index >= 0)
					{
						// move to the front
						var tmp = e.addresses[0];
						e.addresses[0] = e.addresses[index];
						e.addresses[index] = tmp;
					}
				}
				else
				{
					e.last = e.addresses[0];
				}

				return (IPAddress[])e.addresses.Clone();
			}
		}

		public static IPAddress[] ParseIpAddress(IPAddress ip)
		{
#if UNITY_EDITOR
			return DnsBase.GetHostAddresses(ip.ToString());
#elif UNITY_IPHONE
			return new IPAddress[] { IPAddress.Parse(getIPv6(ip.ToString())) };
#else
			return DnsBase.GetHostAddresses(ip.ToString());
#endif
		}

#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		private static extern string getIPv6(string ip);
#endif
	}
}

