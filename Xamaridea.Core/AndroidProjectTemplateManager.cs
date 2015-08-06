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

		public const string TemplateFolderName = "Template_v.0.8";//TODO: use template zip md5 to compare versions instead ?
		public const string TemplateCustomFolderName = "Template_Custom";

		public const string ProjectsFolderName = "Projects";

		public const string XamarinResFolderVariable = "%XAMARIN_RESOURCES_FOLDER%";
		public const string AndroidSDKFolderVariable = "%ANDROID_SDK_FOLDER%";

		string externalTemplatePath = null;

		public void OpenTempateFolder ()
		{
			ExtractTemplateIfNotExtracted();

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

		public string CreateProjectFromTemplate (string xamarinResourcesDir, string sdkPath = null, string templatePath = null)
		{
			externalTemplatePath = templatePath;

			AppendLog("extracting template...");
			ExtractTemplateIfNotExtracted();

			AppendLog("creating temp dest dir");
			var tempNewProjectDir = Path.Combine (TempDirectory, ProjectsFolderName, Guid.NewGuid ().ToString ("N"));
			FileExtensions.DirectoryCopy (TemplateDirectory, tempNewProjectDir);

			AppendLog("updating gradle script");
			var gradleConfig = Path.Combine (tempNewProjectDir, Path.Combine (@"app", "build.gradle"));
			FileExtensions.ReplacePlaceHolder(gradleConfig, XamarinResFolderVariable, xamarinResourcesDir, true);

			AppendLog("updating local.properties");
			var localProperties = Path.Combine (tempNewProjectDir, @"local.properties");
			FileExtensions.ReplacePlaceHolder(localProperties, AndroidSDKFolderVariable, sdkPath ?? "$ANDROID_HOME", true);

			return tempNewProjectDir;
		}

		protected string TemplateDirectory {
			get {
				return Path.Combine (TempDirectory, externalTemplatePath != null ? TemplateCustomFolderName : TemplateFolderName);
			}
		}

		protected string TempDirectory {
			get {
				string appData;
				if (EnvironmentUtils.IsRunningOnMac())
					appData = "/tmp";
				else 
					appData = Path.GetTempPath();
				return Path.Combine (appData, AppDataFolderName);
			}
		}

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
			if (externalTemplatePath != null || !Directory.Exists (TemplateDirectory)) {
				ExtractTemplate ();
			}else
				AppendLog("extraction not needed");
		}

		private void ExtractTemplate ()
		{
			if (externalTemplatePath != null) {
				if (File.Exists (externalTemplatePath) && Path.GetExtension (externalTemplatePath).Contains ("zip")) {
					AppendLog("extracting custom zip template");
					using (var fileStream = File.Open (externalTemplatePath, FileMode.Open)) {
						ExtractTemplateZip (fileStream);
					}
					return;
				} else if (Directory.Exists (externalTemplatePath)) {
					AppendLog("extracting template folder");
					FileExtensions.DirectoryCopy(externalTemplatePath, TemplateDirectory, true);
					return;
				}

				AppendLog("cannot extract external template, falling back to embedded template");
			}

			using (var embeddedStream = Assembly.GetExecutingAssembly ().GetManifestResourceStream (AndroidTemplateProjectResourceName)) {
				if (embeddedStream == null)
					throw new InvalidOperationException (AndroidTemplateProjectResourceName + " was not found");
				//let's generate new project each time the plugin is called (to avoid file locking)
				//TODO: clean up
				ExtractTemplateZip(embeddedStream);
			}
		}

		void ExtractTemplateZip (Stream embeddedStream)
		{
			using (var archive = new ZipArchive (embeddedStream, ZipArchiveMode.Read)) {
				archive.ExtractToDirectory (TemplateDirectory);
			}
		}

		private void AppendLog (string format, params object[] args)
		{
			Console.WriteLine (" > " + format, args);
		}
	}
}
