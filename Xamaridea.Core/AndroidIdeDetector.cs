using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Data.Common;

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

		const string FallbackIntelliJ = "IntelliJ IDEA";

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
				proc.StartInfo.RedirectStandardInput = true;
				proc.StartInfo.CreateNoWindow = true;
				proc.Start ();

				var pathsString = proc.StandardOutput.ReadToEnd();
				if (!string.IsNullOrEmpty (pathsString)) {
					var paths = pathsString.Split (Environment.NewLine.ToCharArray ())
						.Select (line => line.Trim ())
						.Where (line => line.StartsWith ("path:") 
							&& line.Contains (ideName) 
							&& line.EndsWith (".app")
							&& !line.Contains ("Time Machine")
						)
						.Select(line => line.Split (':') [1].Trim ()/*todo : regexp*/)
						.Distinct()
						.ToList();

					if (paths.Any ()) {
						if (IsConsoleApplication){
							Console.WriteLine ("please choose the right {0} :", ideName);
							for (int i = 0; i < paths.Count (); i++) {
								Console.WriteLine ("[{0}] {1}", (i + 1), paths [i]);
							}
							int posEntered = 0;
							if (paths.Count() >= 10) {
								var pos = Console.ReadLine();
								int.TryParse (pos, out posEntered);
							} else {
								var inchar = Console.ReadKey(true);
								if (char.IsDigit(inchar.KeyChar))
									int.TryParse (inchar.KeyChar.ToString(), out posEntered);
							}
							Console.WriteLine("> {0}", posEntered);
							path = paths [posEntered-1];
						} else {
							//throw new Exception ("multiple IDEs found");
							path = paths[0]; //TODO : UI in IDE ?
						}
					} else 
						throw new FileNotFoundException ("IDE does not seem to be installed");
				} else
					throw new FileNotFoundException ("IDE does not seem to be installed");

			} catch (Exception e) {
				Console.WriteLine("cannot find {0} : {1}", ideName, e.Message);
			}
			if (path == null && ideName != FallbackIntelliJ) {
				Console.WriteLine("searching for IntelliJ IDEA...");
				return GetForMac (FallbackIntelliJ);
			}
			return path;
		}

		public static bool IsConsoleApplication
		{
			get { return Console.In != StreamReader.Null; }
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
