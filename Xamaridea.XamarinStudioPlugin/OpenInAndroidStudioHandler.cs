using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using Mono.TextEditor;
using System;
using MonoDevelop.Projects;
using Xamaridea.Core;
using System.Threading.Tasks;
using Gtk;
using System.Collections.Generic;
using System.Linq;

namespace Xamaridea
{
	class OpenInAndroidStudioHandler : CommandHandler
	{
		protected override async void Run()
		{
			DotNetProject project = (DotNetProject)IdeApp.ProjectOperations.CurrentSelectedItem;

			string projectDirectory = project.FileName;

	        try
			{
				string idePath = AndroidIdeDetector.TryFindIdePath();

				if (idePath == null) {
					idePath = OpenFilePicker();
					//TODO: store for next launches + fallbacks
				}

				if (idePath == null)
					throw new Exception("Android Studio not configured");

				var projectsSynchronizer = new ProjectsSynchronizer(projectDirectory, idePath);
				await projectsSynchronizer.MakeResourcesSubdirectoriesAndFilesLowercase(async () => {
					System.Console.WriteLine("Permissions to change original project has been requested and granted.");
					return true;
				});
				projectsSynchronizer.Sync();
			}
			catch (Exception e)
			{
				MessageService.ShowError("Error trying to open in Android Studio", e);
			}
		}

		protected override void Update(CommandInfo info)
		{
			var project = IdeApp.ProjectOperations.CurrentSelectedItem;

			info.Enabled = 
				project != null && 
				project is DotNetProject && 
				((project as DotNetProject).TargetFramework?.Id?.Identifier?.Contains("MonoAndroid") ?? false);
		}

		protected string OpenFilePicker() 
		{
			string path = null;

			var window = Gtk.Window.ListToplevels().FirstOrDefault();

			string ext = (EnvironmentUtils.IsRunningOnWindows()) ? " .exe" : ".app";

			Gtk.FileChooserDialog filechooser =
			   new Gtk.FileChooserDialog(
				    string.Format("Please Select Android Studio{0} location", ext),
                    window,
					FileChooserAction.Open,
					"Cancel", ResponseType.Cancel,
					"Open", ResponseType.Accept
                );

			var filter = new FileFilter();
			if (EnvironmentUtils.IsRunningOnWindows())
			{
				filter.AddPattern("*.exe");
			}
			else {
				filter.AddPattern("*.app");
				filechooser.SetCurrentFolder("/Applications");
				filechooser.Action = FileChooserAction.SelectFolder;
			}
			filechooser.Filter = filter;

			if (filechooser.Run() == (int)ResponseType.Accept)
			{
				path =  filechooser.Filename;
			}

			filechooser.Destroy();

			return path;
		}
	}
}