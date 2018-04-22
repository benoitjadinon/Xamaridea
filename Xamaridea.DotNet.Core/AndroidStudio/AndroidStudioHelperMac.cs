using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xamaridea.Core.Helpers;

namespace Xamaridea.Core
{
    public class AndroidStudioHelperMac : BaseAndroidStudioHelper
    {
        const string AppNameDefault = "Android Studio";
        const string AppNameFallback = "IntelliJ IDEA";

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
                Process proc = new Process();
                proc.StartInfo.FileName = "/bin/bash";
                proc.StartInfo.Arguments = $"-c '/System/Library/Frameworks/CoreServices.framework/Versions/A/" +
                                           $"Frameworks/LaunchServices.framework/Versions/A/Support/lsregister" +
                                           $" -dump | grep -i '{ideName}''";
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
                _logger.AppendLog($"cannot find {ideName}");
            }
            
            if (path == null && ideName == AppNameDefault)
            {
                _logger.AppendLog("searching for IntelliJ IDEA...");
                return GetMacAppPath(AppNameFallback);
            }
            return path;
        }
    }
}