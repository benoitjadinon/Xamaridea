using System.IO;
using Xamaridea.Core.Helpers;

namespace Xamaridea.Core
{
    public class AndroidProjectHelperMac : BaseAndroidProjectHelper
    {
        public AndroidProjectHelperMac(BaseAndroidStudioHelper androidStudioHelper, Logger logger) 
            : base(androidStudioHelper, logger)
        {
        }

        protected override string GetAppDataFolder() => "/tmp";

        protected override string GetTemplatesFolder(BaseAndroidStudioHelper androidStudio)
            => Path.Combine(androidStudio.SettingsPath, @"plugins\android\lib\templates");
    }
}