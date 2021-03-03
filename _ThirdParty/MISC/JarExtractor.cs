#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

//move Plugins/ICSharpCode.SharpZipLib.dll to Plugins/Editor/ICSharpCode.SharpZipLib.dll to reduce package size
//using ICSharpCode.SharpZipLib.Zip;
using Pathfinding.Ionic.Zip;
using System.Threading;

public class JarExtractor
{
	private static AndroidJavaClass DetectAndroidJNI;
	public static bool IsValid
	{
		get
		{
			if( JarExtractor.DetectAndroidJNI == null )
			{
				JarExtractor.DetectAndroidJNI = new AndroidJavaClass("android.os.Build");
			}
			return JarExtractor.DetectAndroidJNI.GetRawClass() != System.IntPtr.Zero;
		}
	}
	
	
	static int GenerateJarLoadID( string jarPath, string filePath )
	{
		return EB.Hash.StringHash( jarPath + ":" + filePath );
	}

	static string GetOutFilename( string filename )
	{
		return System.IO.Path.Combine(_outDataPath, Path.GetFileName( filename));
	}
	
	static bool NeedsCopy( string filePath, int expectedHash, out FileInfo hashfileInfo )
	{
		string hashfilePath = System.IO.Path.Combine(_outDataPath, System.IO.Path.GetFileName( filePath + ".hash" ));
		hashfileInfo = new FileInfo(hashfilePath);
		if (hashfileInfo.Exists)
		{
			int readHash = ReadHashData(hashfileInfo);
			return readHash != expectedHash;
		}
		else
		{
			EB.Debug.Log ("no hash information found");
			return true;
		}
	}
	
	static public IEnumerator SyncLoadFromJar( string jarPath, string filePath, long fileSize, string outputPath, int expectedHash, System.Action<bool> callback )
	{
		bool success = false;
		if( JarExtractor.IsValid == true )
		{
			EB.Debug.Log( string.Format( "SyncLoadFromJar JarSize:{0} filePath:{1}, outputPath:{2}", fileSize, filePath, outputPath ) );

			FileInfo hashfileInfo = null;
			if (NeedsCopy(filePath, expectedHash, out hashfileInfo))
			{
				var jarExtractionJNI = new AndroidJavaObject( "org.manhuang.android.JarExtraction", jarPath, filePath, outputPath );
				if( jarExtractionJNI != null )
				{
					success = jarExtractionJNI.Call<bool>( "StartExtractFromJar" );
					if( success == true )
					{
						bool complete = false;
						do
						{
							yield return null;
							complete = jarExtractionJNI.Call<bool>( "StepExtractFromJar", 1024 * 50 );
						}
						while( complete == false );
						jarExtractionJNI.Call( "CompleteExtractFromJar" );
						
						// write hash file afterwards
						EB.Debug.Log ("writing hash information for " + filePath + " => ");
						WriteHashData (hashfileInfo, expectedHash);
					}
				}
			}
			else
			{
				success = true;
			}
		}
		
		callback( success );
	}

	static string _outDataPath = "";
	static JarExtractor()
	{
		_outDataPath = Application.persistentDataPath;
	}

	public static void ExtractAssetBundles( string jarfile, Dictionary<string,int> hashes, System.Action<bool> callback )
	{
		ThreadPool.QueueUserWorkItem(delegate(object state) {
			try {
				var buffer = new byte[1024*1204];

				EB.Debug.Log("Loading jar file: " + jarfile);
				using( var zip = new ZipFile(jarfile) )
				{
					var iter = zip.GetEnumerator();
					while (iter.MoveNext())
					{
						ZipEntry entry = iter.Current;
						if (!entry.IsDirectory && entry.FileName.EndsWith(".assetbundle"))
						{
							EB.Debug.Log("Got entry " + entry.FileName + " " + entry.CompressedSize);
							var id = Path.GetFileNameWithoutExtension(entry.FileName);
							var outPath = GetOutFilename(Path.GetFileName(entry.FileName));

							var outDir = Path.GetDirectoryName(outPath);
							if (!Directory.Exists(outDir))
							{
								Directory.CreateDirectory(outDir);
							}

							if (!hashes.ContainsKey(id))
							{
								EB.Debug.Log("Skipping " + entry.FileName);
								continue;
							}

							var hash = hashes[id];

							FileInfo hashfileInfo = null;
							if (NeedsCopy(outPath,hash, out hashfileInfo))
							{
								EB.Debug.Log("Writing file " + outPath);
								using (var input = entry.InputStream )
								{
									using (var output = File.Create(outPath))
									{
										int read;
										int written = 0;
										while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
										{
											output.Write (buffer, 0, read);
											written += read;
										}

										EB.Debug.Log("Wrote: " + written + " " + entry.UncompressedSize);
									}
								}
								WriteHashData(hashfileInfo, hash);
							}
						}
					}
					iter.Dispose();
				}

				callback(true);
			}
			catch (System.Exception ex) {
				EB.Debug.LogError("Failed to extract bundles!" + ex);
				callback(false);
			}
		});

	}

	static Dictionary<string,ZipFile> _zipFiles = new Dictionary<string,ZipFile>();

	static ZipFile GetZipFile( string jarFile )
	{
		ZipFile file  = null;
		if (!_zipFiles.TryGetValue(jarFile,out file))
		{
			try {
				file = new ZipFile(jarFile);
				_zipFiles[jarFile] = file;
			}
			catch (System.Exception ex){
				EB.Debug.LogError("Failed to create zip file " + ex.ToString());
			}
		}
		return null;
	}

	static private void WriteHashData(FileInfo fi, int hash)
	{
		try
		{
			fi.Delete ();
			StreamWriter sw = new StreamWriter(fi.Create ());
			sw.Write (hash);
			sw.Close ();
		}
		catch (System.Exception ex)
		{
			EB.Debug.LogError ("WriteHashData caught an exception, skipping write: " + ex.Message);
		}
	}
	
	static private int ReadHashData(FileInfo fi)
	{
		int hash = 0;
		try
		{
			StreamReader sr = new StreamReader(fi.OpenRead ());
			string s = sr.ReadToEnd();
			int.TryParse(s, out hash);
		}
		catch (System.Exception ex)
		{
			EB.Debug.LogError ("ReadHashData caught an exception, skipping read: " + ex.Message);
		}
		return hash;
	}
}

#endif