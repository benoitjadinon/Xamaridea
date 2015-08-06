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
			try {
				var projectsSynchronizer = new ProjectsSynchronizer (args);
				await projectsSynchronizer.MakeResourcesSubdirectoriesAndFilesLowercase (async () => {
					System.Console.WriteLine ("Permissions to change original project has been requested and granted.");
					return true;
				});
				projectsSynchronizer.Sync ();
			} catch (Exception e) {
				System.Console.WriteLine(e.Message);
			}
		}
	}
}