using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xamaridea.Core.Helpers;

namespace Xamaridea.Core
{
    public class AndroidStudioHelperWindows : BaseAndroidStudioHelper
    {
        public AndroidStudioHelperWindows(string exePath, ILogger logger) : base(exePath, logger)
        {
        }

        public override string SettingsPath => Path.GetFullPath(ExePath);

        protected override Process GetOpenProcess(string ideaProjectDir)
        {
            string arguments = string.Format ("\"{0}\"", ideaProjectDir);
            return Process.Start(ExePath, arguments); //TODO: specify exact file
        }

        protected override string TryFindPath()
        {
            try
            {
                const string defaultPath = @"C:\Program Files\Android\Android Studio\bin\studio64.exe";
                if (File.Exists(defaultPath))
                    return defaultPath;

                var jetBrainsFolders = Directory.GetDirectories(@"C:\Program Files (x86)\JetBrains");
                var ideaDir = jetBrainsFolders
                    .Where(dir => Path.GetFileName(dir).StartsWith("IntelliJ IDEA") && 
                                  File.Exists(Path.Combine(dir, @"bin\idea.exe")))
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