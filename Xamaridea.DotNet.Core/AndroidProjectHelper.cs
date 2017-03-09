using System;
using System.IO;

namespace Xamaridea.Core
{
	public class AndroidProjectHelper
	{
		public const string AppDataFolderName = "Xamaridea";

		private readonly string sdkPath;


		public AndroidProjectHelper(string sdkPath)
		{
			this.sdkPath = sdkPath;
		}


		public string TryFindPath()
		{
			//TODO : find android path from PATH ?
			return sdkPath;
		}

		protected string TempDirectory
		{
			get
			{
				string appData;
				if (EnvironmentUtils.IsRunningOnMac())
					appData = "/tmp";
				else
					appData = Path.GetTempPath();
				return Path.Combine(appData, AppDataFolderName);
			}
		}

		internal string CreateTempProject(int androidVersion, string packageName)
		{
			// http://stackoverflow.com/questions/20801042/how-to-create-android-project-with-gradle-from-command-line

			var cmd = $"android create project " +
				"--gradle " +
				//"--gradle-version 0.11.+ " +
				"--activity Main " +
				"--package {packageName} " +
				"--target android-{androidVersion} " +
				"--path {TempDirectory}" 
			;
			string path = ""; //TODO : process run android create

			//TODO : if successful, modify app/build.gradle to change the res directory (res.srcDirs) to point to ours
			// also local.properties for the sdk ? may not be needed anymore since "android create"

			return path;
		}

		/*

       Usage:
       android [global options] create [action options]
       Global options:
		  -s --silent     : Silent mode, shows errors only.
		  -v --verbose    : Verbose mode, shows errors, warnings and all messages.
		     --clear-cache: Clear the SDK Manager repository manifest cache.
		  -h --help       : Help on a specific command.

		Valid actions are composed of a verb and an optional direct object:
		- create avd          : Creates a new Android Virtual Device.
		- create project      : Creates a new Android project.
		- create test-project : Creates a new Android project for a test package.
		- create lib-project  : Creates a new Android library project.
		- create uitest-project: Creates a new UI test project.

		(...)

		                         Action "create project":
		  Creates a new Android project.
		Options:
		  -n --name          : Project name.
		  -a --activity      : Name of the default Activity that is created.
		                       [required]
		  -k --package       : Android package name for the application. [required]
		  -v --gradle-version: Gradle Android plugin version.
		  -t --target        : Target ID of the new project. [required]
		  -g --gradle        : Use gradle template.
		  -p --path          : The new project's directory. [required]

		                              Action "create test-project":
		  Creates a new Android project for a test package.
		Options:
		  -m --main    : Path to directory of the app under test, relative to the test
		                 project directory. [required]
		  -p --path    : The new project's directory. [required]
		  -n --name    : Project name.

		                             Action "create lib-project":
		  Creates a new Android library project.
		Options:
		  -t --target        : Target ID of the new project. [required]
		  -v --gradle-version: Gradle Android plugin version.
		  -g --gradle        : Use gradle template.
		  -n --name          : Project name.
		  -k --package       : Android package name for the library. [required]
		  -p --path          : The new project's directory. [required]

		(...)
		*/
	}
}
