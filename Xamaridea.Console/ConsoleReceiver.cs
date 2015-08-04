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
			if (args.AndroidStudioPath == null) {
				args.AndroidStudioPath = AndroidIdeDetector.TryFindIdePath();
			}

			var projectsSynchronizer = new ProjectsSynchronizer(args.XamarinProjectPath, args.AndroidStudioPath);
            await projectsSynchronizer.MakeResourcesSubdirectoriesAndFilesLowercase(async () =>
                {
                    System.Console.WriteLine("Permissions to change original projec has been requested. Granted.");
                    return true;
                });
            projectsSynchronizer.Sync();
        }
	}

	public class ApplicationArguments
	{
		public string AndroidStudioPath  { get; set; }
		public string XamarinProjectPath { get; set; }
	}
}