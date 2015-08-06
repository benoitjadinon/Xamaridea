using Xamaridea.Core;
using System.Threading.Tasks;
using Fclp;

namespace Xamaridea.Console
{
	class Program
	{
		//mono Xamaridea.Console.exe --project:/Users/bja/Workspaces/Xamarin/SomeProj/Droid/SomeProj.Droid.csproj
		static void Main (string[] args)
		{
			var parser = new FluentCommandLineParser<ApplicationArguments> ();


			//AndroidStudioPath

			var pAS = parser.Setup<string> (arg => arg.AndroidStudioPath)
				.As (CaseType.CaseInsensitive, "a", "aspath")
				.WithDescription("Path to Android Studio application")
				;
			try {
				pAS.SetDefault (AndroidIdeDetector.TryFindIdePath ());
			} catch (System.Exception ex) {
				pAS.Required ();
			}


			// XamarinProjectPath

			var pProj = parser.Setup<string> (arg => arg.XamarinProjectPath)
				.As (CaseType.CaseInsensitive, "p", "project")
				.WithDescription("Path to a Xamarin.Android .csproj file")
				.Required ()
				;


			// AndroidSDKPath

			parser.Setup<string> (arg => arg.AndroidSDKPath)
				.As (CaseType.CaseInsensitive, "s", "sdk")
				.WithDescription("Path to the Android SDK folder")
				.SetDefault (null)
				;


			// CustomTemplatePath

			parser.Setup<string> (arg => arg.CustomTemplatePath)
				.As (CaseType.CaseInsensitive, "t", "template")
				.WithDescription("Path to a custom android project template (zip or directory)")
				.SetDefault (null)
				;


			// Help
			parser.SetupHelp ("?", "h", "help")
				.Callback (text => System.Console.WriteLine (text));


			// Parse args

			var result = parser.Parse (args);

			//

			if (!result.HasErrors) {
				//Task.Run(async () => await new ConsoleReceiver().RunAsync(parser.Object));
				new ConsoleReceiver ().RunAsync (parser.Object);
				//System.Console.WriteLine(res);
				System.Console.ReadKey ();
			} else {
				System.Console.WriteLine (result.ErrorText);
				parser.HelpOption.ShowHelp(parser.Options);
			}
		}
	}
}
