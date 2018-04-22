using System;
using System.IO;
using Xamaridea.Core.Helpers;

namespace Xamaridea.Core
{
	public abstract class BaseAndroidProjectHelper
	{
		public string AppDataFolderName => $"Xamaridea{DateTime.Now.Ticks.ToString()}";

		//private readonly string _sdkPath;
		//public string SdkPath => _sdkPath ?? TryFindPath();

		private readonly BaseAndroidStudioHelper _androidStudioHelper;
		private readonly Logger _logger;


		protected BaseAndroidProjectHelper(BaseAndroidStudioHelper androidStudioHelper, Logger logger)
		{
			//_sdkPath = sdkPath; 
			_androidStudioHelper = androidStudioHelper;
			_logger = logger;
		}


		/*public string TryFindPath()
		{
			//TODO : find android path from $PATH, or fallback to default, per platform
			return SdkPath;
		}*/

		protected string TempDirectory
		{
			get
			{
				var appData = GetAppDataFolder();
				return Path.Combine(appData, AppDataFolderName);
			}
		}

		protected abstract string GetAppDataFolder();

		internal string CreateTempProject(int androidVersion, string packageName)
		{
			var templateFolder = GetTemplatesFolder(_androidStudioHelper);

			templateFolder = Path.Combine(templateFolder, "NewAndroidProject");
			
			//EnsureFMPPInstalled();
			var fmppCall = $"fmpp --source-root {templateFolder} --output-root {TempDirectory}";

			_logger.AppendLog(fmppCall);

			return TempDirectory;
		}

		protected abstract string GetTemplatesFolder(BaseAndroidStudioHelper androidStudio);
	}
}