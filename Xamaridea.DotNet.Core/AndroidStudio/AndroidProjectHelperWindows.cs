using System.IO;
using Xamaridea.Core.Helpers;

namespace Xamaridea.Core
{
    public class AndroidProjectHelperWindows : BaseAndroidProjectHelper
    {
        public AndroidProjectHelperWindows(BaseAndroidStudioHelper androidStudioHelper, Logger logger) 
            : base(androidStudioHelper, logger)
        {
        }

        protected override string GetAppDataFolder() => Path.GetTempPath();
        
        protected override string GetTemplatesFolder(BaseAndroidStudioHelper androidStudio)
            => Path.Combine(androidStudio.SettingsPath, @"plugins/android/lib/templates");
    }
}