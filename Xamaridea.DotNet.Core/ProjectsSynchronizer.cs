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
	public class ProjectsSynchronizer
	{
		private readonly XamarinProjectHelper _xamarinProjectHelper;
		private readonly AndroidStudioHelper _aStudioHelper;
		private readonly AndroidProjectHelper _androidProjectHelper;


		public ProjectsSynchronizer(XamarinProjectHelper xamarinProjectHelper, AndroidStudioHelper ASHelper, AndroidProjectHelper AndroidProjectHelper)
		{
			_xamarinProjectHelper = xamarinProjectHelper;
			_aStudioHelper = ASHelper;
			_androidProjectHelper = AndroidProjectHelper;
		}


		public async Task Sync(Func<Task<bool>> permissionAsker)
		{
			AppendLog("Cleaning up Xamarin project");
			await _xamarinProjectHelper.MakeResourcesSubdirectoriesAndFilesLowercase(permissionAsker);

			AppendLog("Creating temp project");
			var ideaProjectDir = _androidProjectHelper.CreateTempProject(_xamarinProjectHelper.GetAndroidVersion(), _xamarinProjectHelper.GetPackageName());
			AppendLog("Created project : {0}", ideaProjectDir);

			AppendLog("Opening Android Studio");
			_aStudioHelper.OpenWait(ideaProjectDir);

			AppendLog("Android Studio closed, deleting temp project");
			DeleteProject(ideaProjectDir);
		}

		private void DeleteProject (string ideaProjectDir)
		{
			if (Directory.Exists (ideaProjectDir))
				Directory.Delete (ideaProjectDir, true);
		}


		private void AppendLog (string format, params object[] args)
		{
			Console.WriteLine (" > " + format, args);
		}
	}
}
