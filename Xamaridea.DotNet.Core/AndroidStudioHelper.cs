using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamaridea.Core.Exceptions;
using Xamaridea.Core.Extensions;

namespace Xamaridea.Core
{
	public class AndroidStudioHelper
	{
		readonly string _androidStudioPath;


		public AndroidStudioHelper(string androidStudioPath)
		{
			this._androidStudioPath = androidStudioPath;
		}


		public void OpenWait(string ideaProjectDir)
		{
			var androidStudioPath = _androidStudioPath ?? TryFindPath();
			Process p;
			if (EnvironmentUtils.IsRunningOnMac ()) {
				var path = string.Format("{0}{1}", androidStudioPath, "/Contents/MacOS/studio");
				if (!File.Exists(path))
					path = string.Format("{0}{1}", androidStudioPath, "/Contents/MacOS/idea");

				p = Process.Start (new ProcessStartInfo (
					path,
					ideaProjectDir.Replace (" ", "\\ ")
				) {
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardError = true,
					RedirectStandardOutput = true,
				});
			} else {
				string arguments = String.Format ("\"{0}\"", ideaProjectDir);
				p = Process.Start(androidStudioPath, arguments); //TODO: specify exact file
			}
			p?.WaitForExit();
		}

		public string TryFindPath()
		{
			if (EnvironmentUtils.IsRunningOnMac())
				return GetForMac();

			if (EnvironmentUtils.IsRunningOnWindows())
				return GetForWindows();

			throw new Exception("unsupported platform : " + System.Runtime.InteropServices.RuntimeInformation.OSDescription);
		}

		private string GetForMac(string ideName = "Android Studio")
		{
			string path = null;
			try
			{
				Process proc = new Process();
				proc.StartInfo.FileName = "/bin/bash";
				proc.StartInfo.Arguments = "-c '/System/Library/Frameworks/CoreServices.framework/Versions/A/Frameworks/LaunchServices.framework/Versions/A/Support/lsregister -dump | grep -i \\'" + ideName + "\\''";
				proc.StartInfo.UseShellExecute = false;
				proc.StartInfo.RedirectStandardOutput = true;
				proc.StartInfo.RedirectStandardError = true;
				proc.StartInfo.CreateNoWindow = true;
				proc.Start();

				path = proc.StandardOutput.ReadLine(); //TODO: async
				if (path != null && path.Contains(":"))
				{
					var paths = path.Trim().Split(':');
					path = paths.Last()?.Trim();
				}
			}
			catch (Exception ignored)
			{
				Console.WriteLine("cannot find {0}", ideName);
			}
			if (path == null && ideName == "Android Studio")
			{
				Console.WriteLine("searching for IntelliJ IDEA...");
				return GetForMac("IntelliJ IDEA");
			}
			return path;
		}

		private string GetForWindows()
		{
			try
			{
				const string defaultPath = @"C:\Program Files\Android\Android Studio\bin\studio64.exe";
				if (File.Exists(defaultPath))
					return defaultPath;

				var jetBrainsFolders = Directory.GetDirectories(@"C:\Program Files (x86)\JetBrains");
				var ideaDir = jetBrainsFolders
					.Where(dir => Path.GetFileName(dir).StartsWith("IntelliJ IDEA") && File.Exists(Path.Combine(dir, @"bin\idea.exe")))
					.OrderByDescending(i => i)
					.FirstOrDefault();

				if (ideaDir != null)
					return Path.Combine(ideaDir, @"bin\idea.exe");

				return null;
			}
			catch (Exception exc)
			{
				return null;
			}
		}
	}
}
