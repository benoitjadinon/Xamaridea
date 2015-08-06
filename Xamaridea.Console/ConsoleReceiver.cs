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
			var projectsSynchronizer = new ProjectsSynchronizer (args);
			await projectsSynchronizer.MakeResourcesSubdirectoriesAndFilesLowercase (async () => {
				System.Console.WriteLine ("Permissions to change original projec has been requested. Granted.");
				return true;
			});
			projectsSynchronizer.Sync ();
		}
	}
}