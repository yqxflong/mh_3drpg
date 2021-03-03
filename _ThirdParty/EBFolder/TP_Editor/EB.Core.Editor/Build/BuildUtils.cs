using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;

public static class BuildUtils 
{	
	public static string BundleDir = "Assets/Bundles/";
	public static string TempBundleDir = "TempBundles/";
	public static string FailedMoveDir = "FailedMoveBundles/";
	public static string ResourcesBundleDir = "Assets/Resources/Bundles/";
		
	public static void ForceDeleteDirectory(string path)
	{
		if (Directory.Exists(path))
		{
			var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };
			
			FileSystemInfo[] infos = directory.GetFileSystemInfos("*");
		
			foreach (var info in infos)
			{
				Debug.Log ("Changing attributes: "+info.FullName);
				if ((info.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				{
					info.Attributes = (info.Attributes & ~FileAttributes.ReadOnly);
				}

				if ((info.Attributes & FileAttributes.Directory) != 0)
				{
					ForceDeleteDirectory(info.FullName);
				}
			}
			
			try 
			{
				directory.Delete(true);
			} 
			catch (System.Exception e)
			{
				Debug.LogError("Failed to delete Directory: "+directory.FullName+" Exception: "+e.ToString());
			}
		}
	}
	
	private static void MoveDirRecursive(string sourceDir, string destDir, string failedDest)
	{
		if (!Directory.Exists(sourceDir))
		{
			Debug.LogError("sourcedir doesn't exist, cannot move: "+sourceDir);
		}
		else
		{
			DirectoryInfo di = new DirectoryInfo(sourceDir);
			string dirName = di.Name;
			
			{	// scoping so the variables created here aren't used after as they're not fixed up if we change destDir
				DirectoryInfo destDirInfo = new DirectoryInfo(destDir);
				string destDirName = destDirInfo.Name;
				if (destDirName.ToLower() != dirName.ToLower())
				{
					destDir = Path.Combine(destDir, dirName);
				}
				DirectoryInfo failedDirInfo = new DirectoryInfo(failedDest);
				string failedDirName = failedDirInfo.Name;
				if (failedDirName.ToLower() != dirName.ToLower())
				{
					failedDest = Path.Combine(failedDest, dirName);
				}
			}
			string path = sourceDir;
			string destDirFullPath =  destDir;
			if (!Directory.Exists(destDir))
			{
				Directory.Move(sourceDir, destDir);
			}
			else
			{
				// destination exists... lets recursively try the dirs inside (sometimes the dest directories are created empty where they were before)
				string[] entries = Directory.GetFileSystemEntries(path);
				foreach (string entry in entries)
				{
					if (Directory.Exists(entry))
					{
						try 
						{
							MoveDirRecursive(entry, destDir, failedDest);
						} 
						catch (System.Exception e)
						{
							Debug.LogError("Error moving: " + entry + " -> " + destDir + ": " + e.Message);
						}							
					}
					else // if (File.Exists(entry))
					{
						try 
						{
							string fileName = Path.GetFileName(entry);
							string destFileFullPath = Path.Combine(destDirFullPath, fileName);
							Debug.Log ("move check : "+destFileFullPath);
							if (!File.Exists(destFileFullPath))
							{
								File.Move(entry, destFileFullPath);
							}
							else
							{
								
								string failedDestFullPath = Path.Combine(failedDest,fileName);
								if (!Directory.Exists(failedDest))
								{
									Directory.CreateDirectory(failedDest);
								}
								Debug.LogWarning("File Exists, saving file ("+entry+") to failed backup: "+failedDestFullPath);
								File.Move(entry, failedDestFullPath);
							}
						} 
						catch (System.Exception e)
						{
							Debug.LogError("Error moving: "+entry+" -> "+destDirFullPath+" exception: "+e.ToString());
						}
					}
				}
				Directory.Delete(path);
				File.Delete(path+".meta");
			}			
		}
	}
	
	private static void MoveBundleDirs(string sourceDir, string destDir)
	{
		if (Directory.Exists(sourceDir))
		{
			List<string> bundleDirs = new List<string>();
			
			string[] dirs = Directory.GetDirectories(sourceDir, "*");
			foreach( string dir in dirs )
			{
				DirectoryInfo info = new DirectoryInfo(dir);
				bundleDirs.Add(info.Name);
			}
			
			string failedMoveDir = Path.Combine(FailedMoveDir, "" + System.DateTime.Now.Ticks);
			
			foreach (string bundleDir in bundleDirs)
			{
				string path = sourceDir + bundleDir;
				string dest = destDir + bundleDir;
				string pathMeta = path + ".meta";			
				string destMeta = dest + ".meta";
				
				//string failedDest = Path.Combine(failedMoveDir, bundleDir);
				try 
				{
					if (System.IO.Directory.Exists(path))
					{
						if (!Directory.Exists(destDir))
						{
							Directory.CreateDirectory(destDir);
						}
						DirectoryInfo pathDirInfo = new DirectoryInfo(path);
						string dirName = pathDirInfo.Name;
						DirectoryInfo destDirInfo = new DirectoryInfo(dest);
						string destDirName = destDirInfo.Name;
						
						string destDirFullPath =  (dirName != destDirName) ? Path.Combine(dest, dirName) : dest;
						if (dirName != destDirName)
						{
							Debug.LogError ("CONSOLE LOG  dirName != destDirName");
						}

						if (Directory.Exists(destDirFullPath))
						{
							MoveDirRecursive(path, destDirFullPath, Path.Combine(failedMoveDir,dirName));
						}
						else
						{
							System.IO.Directory.Move(path, dest);
						}

						//Debug.Log(String.Format("MoveResourceBundlesToBundleDir moved {0} -> {1}", path, dest));
					}

					if (System.IO.File.Exists(pathMeta))
					{
						try
						{
							if (File.Exists(destMeta))
							{
								File.Delete(destMeta);
							}
							System.IO.File.Move(pathMeta, destMeta);
						}
						catch (System.Exception e)
						{
							Debug.Log("Unable to move meta file "+pathMeta+" -> "+destMeta+" exception: "+e.ToString());
						}
						//Debug.Log(String.Format("MoveResourceBundlesToBundleDir moved {0} -> {1}", path, dest));
					}				
				}
				catch (System.Exception e)
				{
					Debug.LogError(string.Format("Exception during MoveResourceBundlesToBundleDir moving {0} -> {1}: {2} ", path, dest, e.ToString()));
				}
			}
		}
	}	
	
	[MenuItem("Builder/Legacy/Helper/Hide Bundles Dir")]
	public static void HideBundlesDirs()
	{
		Debug.LogFormat("HideBundlesDirs: {0} -> {1}", ResourcesBundleDir, TempBundleDir);
		MoveBundleDirs(ResourcesBundleDir, TempBundleDir);
		AssetDatabase.Refresh();
	}

	[MenuItem("Builder/Legacy/Helper/UnHide Bundles Dir")]
	public static void UnHideBundlesDirs()
	{
		Debug.LogFormat("UnHideBundlesDirs: {0} -> {1}", TempBundleDir, ResourcesBundleDir);
		MoveBundleDirs(TempBundleDir, ResourcesBundleDir);
		AssetDatabase.Refresh();
	}

	[MenuItem("Builder/Legacy/Helper/Clean Local Data")]
	public static void CleanData()
	{
		Debug.Log("Removing previous bundles: "+BuildSettings.BuildFolder);
		ForceDeleteDirectory(BuildSettings.BuildFolder);
	}
}
