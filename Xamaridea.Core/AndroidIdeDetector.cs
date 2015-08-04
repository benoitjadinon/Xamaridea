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
			switch (Environment.OSVersion.Platform) {
			case PlatformID.Unix: // sigh
			case PlatformID.MacOSX:
				return GetForMac();

			case PlatformID.Win32NT:
			case PlatformID.Win32S:
			case PlatformID.Win32Windows:
			case PlatformID.WinCE:
				return GetForWindows();

			default:
				throw new ApplicationException("unsupported platform");
			}
		}

		static string GetForMac ()
		{
			Process proc = new System.Diagnostics.Process ();
			proc.StartInfo.FileName = "/bin/bash";
			proc.StartInfo.Arguments = "-c '/System/Library/Frameworks/CoreServices.framework/Versions/A/Frameworks/LaunchServices.framework/Versions/A/Support/lsregister -dump | grep -i \\'Android Studio\\''";
			proc.StartInfo.UseShellExecute = false;
			proc.StartInfo.RedirectStandardOutput = true;
			proc.StartInfo.RedirectStandardError = true;
			proc.StartInfo.CreateNoWindow = true;
			proc.Start ();
			/*proc.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => {
				Debug.WriteLine (e.Data.ToString ());
			};
			proc.OutputDataReceived += (object sender, DataReceivedEventArgs e) => {
				Debug.WriteLine (e.Data.ToString ());
			};*/
			string path = null;
			path = proc.StandardOutput.ReadLine (); //TODO: async
			if (path != null && path.Contains (":")) {
				var paths = path.Trim ().Split (':');
				path = paths.Last()?.Trim();
			}
			return path;
		}

		static string GetForWindows ()
		{
			try
            {
                const string androidStudioPath = @"C:\Program Files\Android\Android Studio\bin\studio64.exe";
                if (File.Exists(androidStudioPath))
                    return androidStudioPath;

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
