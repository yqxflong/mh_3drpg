using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public static class CommandLineUtils
{
	public static string TMP = "/tmp";

	const int kMaxThreads = 4;

	public class AsyncTask
	{
		public string command;
		public string args;
		public string dir;
		public string result;
		public bool   done;
	}

	static EB.Collections.Queue<AsyncTask> _queue = new EB.Collections.Queue<AsyncTask>(1024);
	static System.Threading.Semaphore _semaphore = new System.Threading.Semaphore(0,kMaxThreads);
	static bool _running = false;

	static CommandLineUtils()
	{
#if UNITY_EDITOR_WIN
		SetupCygwinEnvironment();

		TMP = EnvironmentUtils.Get("TMP", TMP);
#endif
	}

#if UNITY_EDITOR_WIN
	private static string FindCygwinRoot(string path)
	{
		string mintty = Path.Combine(path, "bin/mintty.exe");
		if (File.Exists(mintty))
		{
			return path;
		}

		if (!Directory.Exists(path))
		{
			return string.Empty;
		}

		foreach (string sub in Directory.GetDirectories(path))
		{
			if (Path.GetFileName(sub).ToLower().Contains("cygwin"))
			{
				string result = FindCygwinRoot(sub);
				if (!string.IsNullOrEmpty(result))
				{
					return result;
				}
			}
		}

		return string.Empty;
	}

	private static string s_cygwinRoot = string.Empty;
	private static string ScanCygwinRoot()
	{
		if (!string.IsNullOrEmpty(s_cygwinRoot))
		{
			return s_cygwinRoot;
		}

		// return [A:\\, B:\\]
		string[] drives = System.Environment.GetLogicalDrives();
		foreach (string drive in drives)
		{
			s_cygwinRoot = FindCygwinRoot(drive);
			if (!string.IsNullOrEmpty(s_cygwinRoot))
			{
				break;
			}
		}

		if (string.IsNullOrEmpty(s_cygwinRoot))
		{
			Debug.LogError("ScanCygwinRoot: cygwin not found");
		}		
		return s_cygwinRoot;
	}

	private static string s_extraSearch = string.Empty;
	private static void SetupCygwinSearchPath(string cygwin_root)
	{
		if (!string.IsNullOrEmpty(s_extraSearch))
		{
			return;
		}

		string origin = System.Environment.GetEnvironmentVariable("PATH");
		string[] originSearches = origin.Split(Path.PathSeparator).Select(x => x.ToLower()).ToArray();

		List<string> searches = new List<string>();
		searches.Add(Path.Combine(cygwin_root, "bin"));
		searches.Add(Path.Combine(cygwin_root, "sbin"));
		searches.Add(Path.Combine(cygwin_root, "usr\\bin"));
		searches.Add(Path.Combine(cygwin_root, "usr\\sbin"));
		searches.Add(Path.Combine(cygwin_root, "usr\\local\\bin"));

		foreach (string search in searches)
		{
			if (!originSearches.Contains(search.ToLower()))
			{
				s_extraSearch += Path.PathSeparator + search;
			}
		}

		System.Environment.SetEnvironmentVariable("PATH", origin + s_extraSearch);
	}

	public static void AddSearchPath(string path)
	{
		string origin = System.Environment.GetEnvironmentVariable("PATH");
		string[] originSearches = origin.Split(Path.PathSeparator).Select(x => x.ToLower()).ToArray();
		string extraSearch = string.Empty;

		List<string> searches = new List<string>();
		searches.Add(path);

		foreach (string search in searches)
		{
			if (!originSearches.Contains(search.ToLower()))
			{
				extraSearch += Path.PathSeparator + search;
			}
		}

		System.Environment.SetEnvironmentVariable("PATH", origin + extraSearch);
	}

	private static void SetupCygwinEnvironment()
	{
		string cygwinRoot = ScanCygwinRoot();
		if (!string.IsNullOrEmpty(cygwinRoot))
		{
			SetupCygwinSearchPath(cygwinRoot);
		}
	}
#endif

	static void _Thread()
	{
		while (_running)
		{
			while (true)
			{
				AsyncTask task = null;

				lock (_queue)
				{
					if (_queue.Count > 0)
					{
						task = _queue.Dequeue();
					}
				}

				if (task == null)
				{
					break;
				}

				task.result = Run(task.command, task.args, task.dir);
				task.done = true;
			}

			_semaphore.WaitOne();
		}
	}

	static System.Diagnostics.Process _RunAsync(string command, string arguments, string workingDir)
	{
		//Debug.Log("Runing: " + command + " " + arguments);
		System.Diagnostics.Process p = new System.Diagnostics.Process();
		p.StartInfo.FileName = command;
		p.StartInfo.Arguments = arguments;
		p.StartInfo.WorkingDirectory = workingDir;
		p.StartInfo.UseShellExecute = false;
		p.StartInfo.CreateNoWindow = true;
		p.StartInfo.RedirectStandardOutput = true;
		p.StartInfo.RedirectStandardError = true;
		p.Start();
		return p;
	}

	static int _Run(string command, string arguments, string workingDir)
	{
		var p = _RunAsync(command, arguments, workingDir);
		
		// StringBuilder q =new StringBuilder();
		// if(!p.HasExited){
		// 	q.Append(p.StandardOutput.ReadToEnd());
		// }
		// string r = q.ToString();
		// EB.Debug.Log("Run done output: " + r);
		
		StringBuilder q1 =new StringBuilder();
		while(!p.HasExited){
			q1.Append(p.StandardError.ReadToEnd());
		}
		string r1 = q1.ToString();
		EB.Debug.Log("Run done output err: " + r1);
		
		p.WaitForExit();
		EB.Debug.Log("Run done " + p.ExitCode);
		var code = p.ExitCode;
		
		
		
		
		p.Dispose();
		System.GC.Collect();
		return code;
	}

	static void _DeleteFile(string file)
	{
		try
		{
			File.Delete(file);
		}
		catch { }
	}

	public static AsyncTask RunAsync(string command, string arguments)
	{
		return RunAsync(command, arguments, Directory.GetCurrentDirectory());
	}

	public static AsyncTask RunAsync(string command, string arguments, string workingDir)
	{
		if (_running == false)
		{
			_running = true;
			for (int i = 0; i < kMaxThreads; ++i)
			{
				new System.Threading.Thread(_Thread).Start();
			}
		}

		AsyncTask task = new AsyncTask();
		task.command = command;
		task.args = arguments;
		task.dir = workingDir;
		lock (_queue)
		{
			_queue.Enqueue(task);
		}

		// wake up threads
		try
		{
			_semaphore.Release();
		}
		catch { }

		return task;
	}

	public static void WaitForTasks()
	{
		while (true)
		{
			bool done = true;
			lock (_queue)
			{
				done = _queue.Count == 0;
			}

			if (done)
			{
				return;
			}

			System.Threading.Thread.Sleep(100);
		}
	}

	public static string Run(string command, string arguments)
	{
		return Run(command, arguments, Path.GetFullPath(Directory.GetCurrentDirectory()));
	}

#if UNITY_EDITOR_OSX
	public static string RunWithAppleScript(string command, string arguments)
	{
		return Run(command, arguments, Directory.GetCurrentDirectory(), true);
	}
#endif

#if UNITY_EDITOR_WIN
	private static int _RunCommandInCygwin(string command, string arguments, string workingDir)
	{
		arguments = string.Format("--login -c cd {0} ; {1} {2}", workingDir, command, arguments);
		command = "bash";

		Debug.Log("Running: " + command + " " + arguments);
		return _Run(command, arguments, workingDir);
	}

	private static int _RunScriptInCygwin(string script, string arguments, string workingDir)
	{
		arguments = string.Format("--login {0} {1}", script, arguments);
		string command = "bash";

		Debug.Log("Running: " + command + " " + arguments);
		return _Run(command, arguments, workingDir);
	}

	public static string Run(string command, string arguments, string workingDir, bool applescript = false)
	{
		// command = command.Replace('\\', '/');
		// arguments = arguments.Replace('\\', '/');
		// workingDir = workingDir.Replace('\\', '/');
		Debug.Log("Running: " + command + " " + arguments);
		
		// RunWindowsSyncCommand("cmd", string.Format("/C {0} {1}", command, arguments));
		RunWindowsSyncCommand(command, arguments);
		return "";


		// int hash        = System.Diagnostics.Process.GetCurrentProcess().Id + Random.Range( 1, 100000 );
		// int threadId    = System.Threading.Thread.CurrentThread.ManagedThreadId;
		//
		// string file     = string.Format("{2}/unity_command_{0}_{1}.sh", hash, threadId, TMP);
		// string outfile  = string.Format("{2}/unity_command_{0}_{1}out.txt", hash, threadId, TMP);
		// string script   = string.Format("#!/bin/bash\nulimit -s 16384\ntouch '{2}'\n{0} {1} > '{2}' 2>&1\n", command, arguments, outfile);
		// _DeleteFile(outfile);
		// File.WriteAllText(file, script);
		// _RunCommandInCygwin("chmod", "u+x " + file, workingDir);
		//
		// _RunScriptInCygwin(file, string.Empty, workingDir);
		//
		// string output = string.Empty;
		// try
		// {
		// 	output = File.ReadAllText(outfile);
		// }
		// catch { }
		//
		// //_DeleteFile(outfile);
		//
		// Debug.LogFormat("output: {0}", output);
		// return output;
	}
#else
	public static string Run(string command, string arguments, string workingDir, bool applescript = false)
	{
		Debug.Log("Running: " + command + " " + arguments + " " + applescript);

		int hash        = System.Diagnostics.Process.GetCurrentProcess().Id + Random.Range( 1, 100000 );
		int threadId    = System.Threading.Thread.CurrentThread.ManagedThreadId;

		string file     = string.Format("/tmp/unity_command_{0}_{1}.sh", hash, threadId);
		string outfile  = string.Format("/tmp/unity_command_{0}_{1}out.txt", hash, threadId);
		string script   = string.Format("#!/bin/bash\nulimit -s 16384\ntouch {2}\n{0} {1} > {2}\n", command, arguments, outfile);
		_DeleteFile(outfile);
		File.WriteAllText(file, script);
		_Run("chmod", "u+x " + file, workingDir);

		if (applescript)
		{
			string applescriptFile  = string.Format("/tmp/unity_command_{0}_{1}.scpt", hash, threadId);
			string applescriptScript = string.Empty;
			applescriptScript += "on is_running(appName)\n";
			applescriptScript += "tell application \"System Events\" to (name of processes) contains appName\n";
			applescriptScript += "end is_running\n";
			applescriptScript += "set terminalRunning to is_running(\"Terminal\")\n";
			applescriptScript += "tell application \"Terminal\"\n";
			applescriptScript += "activate\n";
			applescriptScript += string.Format("set runningTab to do script \"{0}\"\n", file);
			applescriptScript += "delay 1\n";
			applescriptScript += "repeat until busy of runningTab is false\n";
			applescriptScript += "delay 1\n";
			applescriptScript += "end repeat\n";
			applescriptScript += "if terminalRunning is false then\n";
			applescriptScript += "quit\n";
			applescriptScript += "else\n";
			applescriptScript += "close window 1\n";
			applescriptScript += "end if\n";
			applescriptScript += "end tell\n";

			_DeleteFile(applescriptFile);
			File.WriteAllText(applescriptFile, applescriptScript);
			_Run("chmod", "u+x " + applescriptFile, workingDir);

			_Run("osascript", applescriptFile, workingDir);
		}
		else
		{
			_Run(file, string.Empty, workingDir);
		}

		string output = string.Empty;
		try
		{
			output = File.ReadAllText(outfile);
		}
		catch { }

		//_DeleteFile(outfile);

		return output;
	}
#endif

	public static void RunWindowsSyncCommand(string fileName, string command)
	{
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

		startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
		startInfo.FileName = fileName;
		startInfo.Arguments = command;
		startInfo.LoadUserProfile = true;
		// startInfo.UseShellExecute = false;
		process.StartInfo = startInfo;
		process.Start();

		if (!process.WaitForExit(-1))
		{
			Debug.LogError("Could not run windows command: " + fileName + " " + command);
		}
	}

	private static Dictionary<string,string> _customArgs = null;
	private static string _commandLineArgs = null;

	public static string GetCommandLineArgs()
	{
		if (_commandLineArgs == null)
		{
			_commandLineArgs = System.String.Join(" ", System.Environment.GetCommandLineArgs());
		}
		return _commandLineArgs;
	}

	public static Dictionary<string, string> GetBatchModeCommandArgs()
	{
		if (_customArgs == null)
		{
			_customArgs = new Dictionary<string, string>();
			string args = GetCommandLineArgs();
			string matchString = "-batchmodeargs:";

			string argsLower = args.ToLower();
			int bmaStartIndex = argsLower.IndexOf(matchString);
			if (bmaStartIndex > -1)
			{
				int startIndex = bmaStartIndex + matchString.Length;
				int endIndex = args.IndexOf(' ', startIndex);
				string argsValue = "";
				if (endIndex < 0)
				{
					argsValue = args.Substring(startIndex);
				}
				else
				{
					argsValue = args.Substring(startIndex, endIndex - startIndex);
				}
				string[] argsValuesSplit = argsValue.Split(',');
				foreach (string entry in argsValuesSplit)
				{
					string[] splitEntry = entry.Split('=');
					string key, value;
					int length = splitEntry.Length;
					if (length == 1)
					{
						key = splitEntry[0];
						value = key;
					}
					else if (length == 2)
					{
						key = splitEntry[0];
						value = splitEntry[1];
					}
					else {

						Debug.LogError("Warning incorrect # of values: " + argsValue);
						continue;
					}
					Debug.Log("Adding BatchModeArgs: |" + key + "|," + value);
					_customArgs[key] = value;
				}
			}
		}
		return _customArgs;
	}
}
