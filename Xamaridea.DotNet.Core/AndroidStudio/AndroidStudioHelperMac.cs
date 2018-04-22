using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using Xamaridea.Core.Helpers;

namespace Xamaridea.Core
{
    public class AndroidStudioHelperMac : BaseAndroidStudioHelper
    {
        const string AppNameDefault = "Android Studio.app";
        const string AppNameFallback = "IntelliJ IDEA.app";
        const string AppNameFallback2 = "IntelliJ IDEA Ultimate.app";

        private readonly ILogger _logger;

        public AndroidStudioHelperMac(string exePath, ILogger logger) 
            : base(exePath, logger)
        {
            _logger = logger;
        }

        public override string SettingsPath => $"{ExePath}/Contents";

        protected override Process GetOpenProcess(string ideaProjectDir)
        {
            var path = $"{SettingsPath}/MacOS/studio";
            
            // hum
            if (!File.Exists(path))
                path = $"{SettingsPath}/MacOS/idea";

            return Process.Start (new ProcessStartInfo (
                path,
                ideaProjectDir.Replace (" ", "\\ ")
            ) {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            });

        }

        protected override string TryFindPath()
        {
            return GetMacAppPath(AppNameDefault);
        }

        private string GetMacAppPath(string ideName)
        {
            string path = null;
            try
            {
                _logger.AppendLog($"searching for {ideName}...");
                
                Process proc = new Process();
                proc.StartInfo.FileName = "/bin/bash";
                proc.StartInfo.Arguments = $"-c '/System/Library/Frameworks/CoreServices.framework/Versions/A/" +
                                           $"Frameworks/LaunchServices.framework/Versions/A/Support/lsregister" +
                                           $" -dump | grep -w \"{ideName}\"'";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();

                string line;
                while (!proc.StandardOutput.EndOfStream) 
                {
                    line = proc.StandardOutput.ReadLine();
                    if (line.Contains(":") && line.Contains("/Applications"))
                    {
                        var paths = line.Trim().Split(':');
                        path = paths.Last()?.Trim();
                        break;
                    }
                }
            }
            catch (Exception ignored)
            {
                _logger.AppendLog($"cannot find {ideName}");
            }
            
            if (path == null && ideName == AppNameDefault)
            {
                return GetMacAppPath(AppNameFallback);
            }

            if (path == null && ideName == AppNameFallback)
            {
                return GetMacAppPath(AppNameFallback2);
            }
            
            return path;
        }
    }
}