using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamaridea.Core.Exceptions;
using Xamaridea.Core.Extensions;
using Xamaridea.Core.Helpers;

namespace Xamaridea.Core
{
	public class ProjectsSynchronizer
	{
		private readonly XamarinProjectHelper _xamarinProjectHelper;
		private readonly BaseAndroidStudioHelper _androidStudioHelper;
		private readonly BaseAndroidProjectHelper _androidProjectHelper;
		private readonly ILogger _logger;


		public ProjectsSynchronizer(XamarinProjectHelper xamarinProjectHelper, 
									BaseAndroidStudioHelper AndroidStudioHelper, 
									BaseAndroidProjectHelper AndroidProjectHelper,
									ILogger logger
			)
		{
			_xamarinProjectHelper = xamarinProjectHelper;
			_androidStudioHelper = AndroidStudioHelper;
			_androidProjectHelper = AndroidProjectHelper;
			_logger = logger;
		}


		public async Task Sync(Func<Task<bool>> permissionAsker)
		{
			_logger.AppendLog("Cleaning up Xamarin project");
			await _xamarinProjectHelper.MakeResourcesSubdirectoriesAndFilesLowercase(permissionAsker);

			_logger.AppendLog("Creating temp project");
			var ideaProjectDir = _androidProjectHelper.CreateTempProject(_xamarinProjectHelper.GetAndroidVersion(), _xamarinProjectHelper.GetPackageName());
			_logger.AppendLog("Created project : {0}", ideaProjectDir);

			_logger.AppendLog("Opening Android Studio");
			var result = _androidStudioHelper.OpenAndWait(ideaProjectDir);

			_logger.AppendLog("Android Studio closed, deleting temp project");
			DeleteProject(ideaProjectDir);
		}

		private void DeleteProject (string ideaProjectDir)
		{
			if (Directory.Exists (ideaProjectDir))
				Directory.Delete (ideaProjectDir, true);
		}

	}
}
