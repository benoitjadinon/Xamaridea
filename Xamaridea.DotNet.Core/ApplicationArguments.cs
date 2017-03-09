using System;
using System.Linq;
using Xamaridea.Core;
using System.Threading.Tasks;
using System.Diagnostics;
using CommandLine;

namespace Xamaridea.Core
{
	public class ApplicationArguments
	{
		[Option('p', "project", Required = true,
		  HelpText = "Path to a Xamarin.Android .csproj file")]
		public string XamarinAndroidProjectFilePath { get; set; }

		[Option('a', "aspatch", Required = false, //Default = AndroidIdeDetector.TryFindIdePath(),
		  HelpText = "Path to Android Studio application")]
		public string AndroidStudioPath  { get; set; }

		[Option('s', "sdk", Required = false,
		        HelpText = "Path to the Android SDK folder")]
		public string AndroidSDKPath { get; set; }

		[Obsolete]
		public string CustomTemplatePath { get; set; }

		[Option(Required = false, Default = false,
		  HelpText = "Prints all messages to standard output.")]
		public bool Verbose { get; set; }
	}
}