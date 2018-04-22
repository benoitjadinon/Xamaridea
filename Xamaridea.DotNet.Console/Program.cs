using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using Xamaridea.Core;
using System.Threading.Tasks;
using Xamaridea.Core.Helpers;

namespace Xamaridea.DotNet.Console
{
    static class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parser.Default
                .ParseArguments<ApplicationArguments>(args)
                .WithParsed(async(o) => await Start(o))
                //.WithNotParsed<ApplicationArguments>((errs) => HandleParseError(errs))
                ;
        }

        class ApplicationArguments
        {
            [Option('p', "project", 
                Required = false,
                HelpText = "Path to a Xamarin.Android .csproj file, otherwise using the first .csproj in current folder")]
            public string XamarinAndroidProjectFilePath { get; set; }

            [Option('a', "aspath", 
                Required = false, //Default = AndroidIdeDetector.TryFindIdePath(),
                HelpText = "Path to Android Studio application")]
            public string AndroidStudioPath  { get; set; }

            //[Option('s', "sdk", 
            //    Required = false, 
            //    HelpText = "Path to the Android SDK folder")]
            //public string AndroidSDKPath { get; set; }
           
            [Option('v', "verbose", Required = false, 
                Default = false, 
                HelpText = "Prints all messages to standard output.")]
            public bool Verbose { get; set; }
        }

        static async Task Start(ApplicationArguments options)
        {
            var logger = new Logger();

            try
            {
                var xamarinProjectHelper = new XamarinProjectHelper(options.XamarinAndroidProjectFilePath, logger);

                BaseAndroidStudioHelper androidStudioHelper;
                BaseAndroidProjectHelper androidProjectHelper;

                if (EnvironmentUtils.IsRunningOnMac())
                {
                    androidStudioHelper = new AndroidStudioHelperMac(options.AndroidStudioPath, logger);
                    androidProjectHelper = new AndroidProjectHelperMac(androidStudioHelper, logger);
                }
                else if (EnvironmentUtils.IsRunningOnWindows())
                {
                    androidStudioHelper = new AndroidStudioHelperWindows(options.AndroidStudioPath, logger);
                    androidProjectHelper = new AndroidProjectHelperWindows(androidStudioHelper, logger);
                }
                else
                    throw new Exception("unsupported platform : " +
                                        System.Runtime.InteropServices.RuntimeInformation.OSDescription);

                await new ProjectsSynchronizer(xamarinProjectHelper, androidStudioHelper, androidProjectHelper, logger)
                    .Sync(ConsolePermissionAsker);
            }
            catch (Exception e)
            {
                logger.AppendLog(e.ToString());
                System.Console.Read();
            }
        }

        private static readonly Func<Task<bool>> ConsolePermissionAsker = async () =>
        {
            //return true;
            System.Console.WriteLine("Renaming XS assets (.xaml to .xml) ? (y/n)");
            var resp = System.Console.ReadKey();
            return resp.ToString().ToLowerInvariant() == "y";
        };
    }
}
