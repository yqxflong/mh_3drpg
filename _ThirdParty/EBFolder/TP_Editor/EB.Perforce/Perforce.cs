using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using EB.Perforce;

namespace EB.Perforce
{
	public class Perforce
	{
		public class P4Result
		{
			public int resultCode = -1;
			public string stdout = "";
			public string stderr = "";	
		}

		//-------------------------------------------------------------------------------------------
		const string globalP4cmd = "/opt/perforce/p4";
		public static P4Result P4Command(string workingDirectory, string command, bool redirectOutput = true)
		{
			string p4cmd = "p4";
			if (File.Exists(globalP4cmd))
			{
				p4cmd = globalP4cmd;
			}
			return ShellCommand(workingDirectory, command, p4cmd, redirectOutput);
		}

		//-------------------------------------------------------------------------------------------
		private static P4Result ShellCommand(string workingDirectory, string command, string fileName, bool redirectOutput)
		{
			P4Result result = new P4Result();
			string prevDirectory = System.IO.Directory.GetCurrentDirectory();
			
			try {
				System.IO.Directory.SetCurrentDirectory(workingDirectory);	

				ProcessStartInfo processStartInfo = new ProcessStartInfo();
				processStartInfo.FileName = fileName;
				processStartInfo.Arguments = command;
				processStartInfo.RedirectStandardOutput = redirectOutput;
				processStartInfo.RedirectStandardError = redirectOutput;
				processStartInfo.UseShellExecute = false;
				processStartInfo.CreateNoWindow = true;
				if (!processStartInfo.EnvironmentVariables.ContainsKey("P4CONFIG"))
				{
					processStartInfo.EnvironmentVariables.Add("P4CONFIG", "P4CONFIG");
				}

				processStartInfo.WorkingDirectory = workingDirectory;
				Process proc = Process.Start(processStartInfo);
				bool reading = redirectOutput;

				while (reading)
				{
					string outLine = proc.StandardOutput.ReadLine();
					string errLine = proc.StandardError.ReadLine();
					if (outLine != null && outLine != "") 
					{
						result.stdout += outLine + "\r";
					}
					if (errLine != null && errLine != "")
					{
						result.stderr += errLine + "\r";
					}
					reading = (outLine != null) || (errLine != null);
				}
				proc.WaitForExit();
				result.resultCode = proc.ExitCode;
			} 
			catch (System.Exception ex)
			{
				UnityEngine.Debug.LogError("EB.Perforce > Something is wrong: " + ex.ToString());
			}
			finally
			{
				System.IO.Directory.SetCurrentDirectory(prevDirectory);			
			}

			UnityEngine.Debug.Log("EB.Perforce > Running: " + fileName + " " + command + ", cwd: " + workingDirectory);
			UnityEngine.Debug.Log("EB.Perforce > Output: " + result.stdout + " " + result.stderr);
			return result;
		}
		
		//-------------------------------------------------------------------------------------------
		public static string P4GetDirectory(string workingDirectory, string p4Path)
		{
			string path = null;
			
			string args = "where " + p4Path;
			P4Result result = P4Command(workingDirectory, args);
			if (result.resultCode == 0) 
			{
				if (result.stderr == "")
				{
					StringReader strReader = new System.IO.StringReader(result.stdout);
					string pathString = strReader.ReadLine();
					string[] splitPaths = pathString.Split(' ');
					if (splitPaths.Length == 3)
					{
						path = splitPaths[2];
						path = path.Trim(new char[] { ' ', '.'});
					}
				}
				else
				{
					//TODO: add some error output here e.g. -- files not in client view
				}
				
			}
			
			return path;
		}
		
		//-------------------------------------------------------------------------------------------
		public static P4Result P4Checkout(string workingDirectory, string filename, int changelist = -1)
		{
			
			if (changelist == -1)
			{
				return Perforce.P4Command(workingDirectory, "edit " + filename);
			}
			else
		{
				Perforce.P4Command(workingDirectory, "edit -c " + changelist + " " + filename);
				return Perforce.P4Command(workingDirectory, "reopen -c " + changelist + " " + filename);
			}
		}
		
		//-------------------------------------------------------------------------------------------
		public static void P4CheckoutDirectory(string dirPath, int changelist = -1)
		{
			string path = dirPath;
			
			if(!System.IO.Directory.Exists(path))
			{
				return;
			}
			
			if(!path.EndsWith("/"))
		{
				path += "/";
			}

			if (changelist == -1)
			{
				Perforce.P4Command(path, "edit " + path + "...");
			}
				else
			{
				Perforce.P4Command(path, "edit -c " + changelist + " " + path + "...", false);
				Perforce.P4Command(path, "reopen -c " + changelist + " " + path + "...", false); //reopen so that any already open files are moved there
			}
		}
		
		//-------------------------------------------------------------------------------------------
		public static P4Result P4Checkout(string filePath, int changelist = -1)
		{
			if(System.IO.Directory.Exists(filePath))
			{
				P4CheckoutDirectory(filePath, changelist);
				return null;
			}
			else
			{
				char separtor = System.IO.Path.DirectorySeparatorChar;
				if(!filePath.Contains(separtor.ToString()))
					separtor = System.IO.Path.AltDirectorySeparatorChar;
				
				int lastSlashIndex = filePath.LastIndexOf(separtor) + 1;
				string directory = filePath.Substring(0, lastSlashIndex);
				string fileName = filePath.Substring(lastSlashIndex, filePath.Length - lastSlashIndex);
					
				if (changelist == -1)
				{
					return Perforce.P4Command(directory, "edit " + fileName);
				}
				else
				{
					Perforce.P4Command(directory, "edit -c " + changelist + " " + fileName);
					return Perforce.P4Command(directory, "reopen -c " + changelist + " " + fileName);
				}
			}
		}
		
		//-------------------------------------------------------------------------------------------
		public static void P4AddDirectory(string dirPath, int changelist = -1)
		{
			string path = dirPath;
			
			if(!System.IO.Directory.Exists(path))
			{
				return;
			}
			
			if(!path.EndsWith("/"))
			{
				path += "/";
			}
			string filename;
			string command;
			if (Application.platform == UnityEngine.RuntimePlatform.OSXEditor || Application.platform == UnityEngine.RuntimePlatform.OSXPlayer)
			{
				filename = "/bin/bash";
				
				if (changelist == -1)
				{
					command = "-c \"find . -type f -print | p4 -x - add\"";
				}
				else
				{
					command = "-c \"find . -type f -print | p4 -x - add -c " + changelist +"\"";
				}
			}
			else
			{
				filename = "cmd.exe";
				
				if (changelist == -1)
				{
					command = "/C dir * /B /S | p4 -x - add";
				}
				else
				{
					command = "/C dir * /B /S | p4 -x - add -c " + changelist;
				}
			}
			Perforce.ShellCommand(path, command, filename, false);
		}
		
		//-------------------------------------------------------------------------------------------
		public static P4Result P4Add(string filePath, int changelist = -1)
		{
			char separtor = System.IO.Path.DirectorySeparatorChar;
			if(!filePath.Contains(separtor.ToString()))
				separtor = System.IO.Path.AltDirectorySeparatorChar;
			
			int lastSlashIndex = filePath.LastIndexOf(separtor) + 1;
			string directory = filePath.Substring(0, lastSlashIndex);
			string fileName = filePath.Substring(lastSlashIndex, filePath.Length - lastSlashIndex);
				
			if (changelist == -1)
			{
				return Perforce.P4Command(directory, "add -f " + fileName);
			}
			else
			{
				return Perforce.P4Command(directory, "add -c " + changelist + " -f " + fileName);
			}
		}
		
		//---------------------------------------------------------------------------------
		public static P4Result P4RevertUnchanged(int changelist = -1)
		{
			if (changelist == -1)
		{
			return Perforce.P4Command(Application.dataPath, "revert -a", false);
		}
			else
			{
				return Perforce.P4Command(Application.dataPath, "revert -a -c " + changelist, false);
			}
		}
		
		//---------------------------------------------------------------------------------
		public static List<string> GetAllFiles(string directoryPath) 
		{
			List<string> fileNameList = new List<string>();
			
			if(Directory.Exists(directoryPath))
			{
				GetAllNestedFiles(directoryPath, fileNameList);
			}
			else
			{
				fileNameList.Add(directoryPath);
			}
			
			return fileNameList;
		}
		
		//---------------------------------------------------------------------------------
		private static void GetAllNestedFiles(string directoryPath, List<string> fileNameList) 
		{
			foreach (string dirPath in Directory.GetDirectories(directoryPath))
	        {
	            GetAllNestedFiles(dirPath, fileNameList);
	        }
			
			foreach( string filePath in Directory.GetFiles(directoryPath))
			{
				if(!Directory.Exists(filePath))
				{
					fileNameList.Add(filePath);
				}
			}
		}
		
		//-------------------------------------------------------------------------------------------
		public static int CreateChangelist(string description)
		{
			string directory = System.IO.Directory.GetCurrentDirectory();
			string filename;
			string command;
			if (Application.platform == UnityEngine.RuntimePlatform.OSXEditor || Application.platform == UnityEngine.RuntimePlatform.OSXPlayer)
			{
				filename = "/bin/bash";
				command = "-c \"(p4 change -o | grep '^\\(Change\\|Client\\|User\\|Description\\)'; echo ' " + description + "') | p4 change -i\"";
			}
			else
			{
				filename = "cmd.exe";
				command = "/C p4 change -o | findstr /R /B \"Change Client User Description\" > p4.txt & echo     " + description + " >> p4.txt & type p4.txt | p4 change -i";
			}
			var res = Perforce.ShellCommand(directory, command, filename, true);
			//parse out the CL number from "Change 12345 created ..."
			return System.Convert.ToInt32(res.stdout.Split(' ')[1]); 
		}

		//-------------------------------------------------------------------------------------------
		public static string GetCurrentClient()
		{
			string directory = System.IO.Directory.GetCurrentDirectory();
			string filename;
			string command;
			if (Application.platform == UnityEngine.RuntimePlatform.OSXEditor || Application.platform == UnityEngine.RuntimePlatform.OSXPlayer)
			{
				filename = "/bin/bash";
				command = "-c \"p4 client -o | grep '^Client:'\"";
			}
			else
			{
				filename = "cmd.exe";
				command = "/C p4 change -o | findstr /R /B \"Client\"";
			}
			var res = Perforce.ShellCommand(directory, command, filename, true);
			return res.stdout.Split('\t')[1].Trim();
		}
		
		//-------------------------------------------------------------------------------------------
		public static int GetChangelist(string description)
		{
			string directory = System.IO.Directory.GetCurrentDirectory();
			string client = Perforce.GetCurrentClient();
			string filename;
			string command;
			if (Application.platform == UnityEngine.RuntimePlatform.OSXEditor || Application.platform == UnityEngine.RuntimePlatform.OSXPlayer)
			{
				filename = "/bin/bash";
				command = "-c \"p4 changes -c " + client + " -s pending | grep '" + description + "'\"";
			}
			else
			{
				filename = "cmd.exe";
				command = "/C p4 changes -c " + client + " -s pending | findstr /R \"" + description + "\"";
			} 
			var res = Perforce.ShellCommand(directory, command, filename, true);
			string change = res.stdout;
			if (change.Length == 0)
			{
				return -1;
			}
			
			return System.Convert.ToInt32(res.stdout.Split(' ')[1]); 
		}
	}
}
