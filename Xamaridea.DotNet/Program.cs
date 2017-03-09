using System;
using System.Collections.Generic;
using CommandLine;
using Xamaridea.Core;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Xamaridea.DotNet
{
    class Program
    {
        static void Main(string[] args)
        {
			CommandLine.Parser.Default.ParseArguments<ApplicationArguments>(args).WithParsed(Start);
        }

		static void Start(ApplicationArguments options)
		{
			var xamarinProject = new XamarinProject(options.XamarinAndroidProjectFilePath);
			var androidStudio = new AndroidStudioHelper(options.AndroidStudioPath);
			var androidProject = new AndroidProjectHelper(options.AndroidSDKPath);

			new ProjectsSynchronizer(xamarinProject, androidStudio, androidProject)
				.Sync(permissionAsker);
		}

		static Func<Task<bool>> permissionAsker = async () =>
		{
			Console.WriteLine("allow renaming XS assets for better AS compatibility ? (y/n)");
			var resp = Console.ReadKey();
			return resp.ToString().ToLowerInvariant() == "y";
		};
	}
}
