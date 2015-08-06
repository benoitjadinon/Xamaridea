using System;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace Xamaridea.Core
{
	public static class AndroidIdeDetector
	{
		public static string TryFindIdePath ()
		{
			if (EnvironmentUtils.IsRunningOnMac ())
				return GetForMac ();

			if (EnvironmentUtils.IsRunningOnWindows ())
				return GetForWindows ();

			throw new ApplicationException ("unsupported platform : " + Environment.OSVersion.Platform);
		}

		static string GetForMac (string ideName = "Android Studio")
		{
			string path = null;
			try {
				Process proc = new System.Diagnostics.Process ();
				proc.StartInfo.FileName = "/bin/bash";
				proc.StartInfo.Arguments = "-c '/System/Library/Frameworks/CoreServices.framework/Versions/A/Frameworks/LaunchServices.framework/Versions/A/Support/lsregister -dump | grep -i \\'" + ideName + "\\''";
				proc.StartInfo.UseShellExecute = false;
				proc.StartInfo.RedirectStandardOutput = true;
				proc.StartInfo.RedirectStandardError = true;
				proc.StartInfo.CreateNoWindow = true;
				proc.Start ();

				path = proc.StandardOutput.ReadLine (); //TODO: async
				if (path != null && path.Contains (":")) {
					var paths = path.Trim ().Split (':');
					path = paths.Last ()?.Trim ();
				}
			} catch (Exception ignored) {
				Console.WriteLine("cannot find {0}", ideName);
			}
			if (path == null && ideName == "Android Studio") {
				Console.WriteLine("searching for IntelliJ IDEA...");
				return GetForMac ("IntelliJ IDEA");
			}
			return path;
		}

		static string GetForWindows ()
		{
			try {
				const string androidStudioPath = @"C:\Program Files\Android\Android Studio\bin\studio64.exe";
				if (File.Exists (androidStudioPath))
					return androidStudioPath;

				var jetBrainsFolders = Directory.GetDirectories (@"C:\Program Files (x86)\JetBrains");
				var ideaDir = jetBrainsFolders
                    .Where (dir => Path.GetFileName (dir).StartsWith ("IntelliJ IDEA") && File.Exists (Path.Combine (dir, @"bin\idea.exe")))
                    .OrderByDescending (i => i)
                    .FirstOrDefault ();

				if (ideaDir != null)
					return Path.Combine (ideaDir, @"bin\idea.exe");

				return null;
			} catch (Exception exc) {
				return null;
			}
		}
	}
}
