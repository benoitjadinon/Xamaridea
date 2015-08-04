using Xamaridea.Core;
using System.Threading.Tasks;
using Fclp;

namespace Xamaridea.Console
{
    class Program
    {
		static void Main(string[] args)
		{
		   var parser = new FluentCommandLineParser<ApplicationArguments>();

			parser.Setup<string>(arg => arg.AndroidStudioPath)
				.As(CaseType.CaseInsensitive, "a","aspath")
				.SetDefault(null)
#if DEBUG
				//.SetDefault(@"C:\Program Files\Android\Android Studio\bin\studio64.exe")
#endif
				;

			parser.Setup<string>(arg => arg.XamarinProjectPath)
				.As(CaseType.CaseInsensitive, "p","project")
				.SetDefault(null)//TODO: find csproj in executing path
#if DEBUG
				//.SetDefault(@"C:\Projects\_[______________\KinderChat\Android\KinderChat.Android.csproj")
				.SetDefault("/Users/bja/Workspaces/Xamarin/MobilityWeek/Droid/MobilityWeek.Droid.csproj")
#endif
				;

			parser.SetupHelp("?", "h", "help")
				.Callback(text => System.Console.WriteLine(text));

		    var result = parser.Parse(args);

			if (!result.HasErrors) {
				//Task.Run(async () => await new ConsoleReceiver().RunAsync(parser.Object));
				new ConsoleReceiver().RunAsync(parser.Object);
				//System.Console.WriteLine(res);
				System.Console.ReadKey();
			}
			else System.Console.WriteLine(result.ErrorText);
		}
    }
}
