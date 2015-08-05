using System;
using System.Linq;
using Xamaridea.Core;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Xamaridea.Console
{
	public class ConsoleReceiver
	{
		public async Task RunAsync (ApplicationArguments args)
		{
			var projectsSynchronizer = new ProjectsSynchronizer (args.XamarinProjectPath, args.AndroidStudioPath, args.AndroidSDKPath);
			await projectsSynchronizer.MakeResourcesSubdirectoriesAndFilesLowercase (async () => {
				System.Console.WriteLine ("Permissions to change original projec has been requested. Granted.");
				return true;
			});
			projectsSynchronizer.Sync ();
		}
	}

	public class ApplicationArguments
	{
		public string AndroidStudioPath  { get; set; }

		public string AndroidSDKPath { get; set; }

		public string XamarinProjectPath { get; set; }
	}
}