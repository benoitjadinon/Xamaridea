using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;
using Xamaridea.Core.Extensions;

namespace Xamaridea.Core
{
	public class AndroidProjectTemplateManager
	{
		public const string AndroidTemplateProjectResourceName = "Xamaridea.Core.AndroidProjectTemplate.zip";
		public const string AppDataFolderName = "Xamaridea";
		public const string TemplateFolderName = "Template_v.0.8";//TODO: use template zip md5 to compare versions
		public const string ProjectsFolderName = "Projects";
		public const string XamarinResourcesFolderVariable = "%XAMARIN_RESOURCES_FOLDER%";
		public const string AndroidSDKFolderVariable = "%ANDROID_SDK_FOLDER%";

		public void Reset ()
		{
			DeleteTemplate();
			ExtractTemplate ();
		}

		public void DeleteTemplate ()
		{
			if (Directory.Exists (TemplateDirectory))
				Directory.Delete (TemplateDirectory, true);
		}

		public void ExtractTemplateIfNotExtracted ()
		{
			if (!Directory.Exists (TemplateDirectory)) {
				ExtractTemplate ();
			}
		}

		public void OpenTempateFolder ()
		{
			if (EnvironmentUtils.IsRunningOnWindows ())
				Process.Start ("explorer.exe", TemplateDirectory);
			else {
				Process proc = new System.Diagnostics.Process ();
				proc.StartInfo.FileName = "open";
				proc.StartInfo.Arguments = "\\'" + TemplateDirectory + "\\'";
				proc.StartInfo.UseShellExecute = false;
				proc.StartInfo.RedirectStandardOutput = true;
				proc.StartInfo.RedirectStandardError = true;
				proc.StartInfo.CreateNoWindow = true;
				proc.Start ();
			}
		}

		public string CreateProjectFromTemplate (string xamarinResourcesDir, string sdkPath = null)
		{
			var tempNewProjectDir = Path.Combine (TempDirectory, ProjectsFolderName, Guid.NewGuid ().ToString ("N"));
			FileExtensions.DirectoryCopy (TemplateDirectory, tempNewProjectDir);

			//gradle
			var gradleConfig = Path.Combine (tempNewProjectDir, Path.Combine (@"app", "build.gradle"));
			ReplacePlaceHolder(gradleConfig, XamarinResourcesFolderVariable, xamarinResourcesDir, true);

			//local.properties
			var localProperties = Path.Combine (tempNewProjectDir, @"local.properties");
			ReplacePlaceHolder(localProperties, AndroidSDKFolderVariable, sdkPath ?? "", true);

			return tempNewProjectDir;
		}

		void ReplacePlaceHolder (string file, string search, string replace, bool fixPaths = false)
		{
			var content = File.ReadAllText (file);
			content = content.Replace (search, replace); //gradle is awesome, it allows us to specify any folder as resource container
			if (fixPaths)
                content = content.Replace (@"\", "/"); //change backslashes to common ones
			File.WriteAllText (file, content);
		}

		private string TemplateDirectory {
			get {
				return Path.Combine (TempDirectory, TemplateFolderName);
			}
		}

		private string TempDirectory {
			get {
				string appData;
				if (EnvironmentUtils.IsRunningOnMac())
					appData = "/tmp";
				else 
					appData = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
				return Path.Combine (appData, AppDataFolderName);
			}
		}

		private void ExtractTemplate ()
		{
			using (var embeddedStream = Assembly.GetExecutingAssembly ().GetManifestResourceStream (AndroidTemplateProjectResourceName)) {
				if (embeddedStream == null)
					throw new InvalidOperationException (AndroidTemplateProjectResourceName + " was not found");
				//let's generate new project each time the plugin is called (to avoid file locking)
				//TODO: clean up
				using (var archive = new ZipArchive (embeddedStream, ZipArchiveMode.Read)) {
					archive.ExtractToDirectory (TemplateDirectory);
				}
			}
		}
	}
}
