using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamaridea.Core.Exceptions;
using Xamaridea.Core.Extensions;
using Xamaridea.Core.Helpers;

namespace Xamaridea.Core
{
	public class XamarinProjectHelper
	{
		public const string ResFolderName = "Resources";

		private bool _grantedPermissionsToChangeMainProject = false;

		private readonly string _projectPath;
		private readonly ILogger _logger;


		public XamarinProjectHelper(string projectPath, ILogger logger)
		{
			_projectPath = projectPath;
			_logger = logger;
		}

		
		public string ProjectFolder 
			=> Path.GetDirectoryName(_projectPath);

		public string ResourceFolder 
			=> Path.Combine(ProjectFolder, ResFolderName);

		public int GetAndroidVersion()
		{
			return 25;//TODO
		}

		internal string GetPackageName()
		{
			//TODO
			throw new NotImplementedException();//.ToLowerInvariant()
		}

		public async Task<bool> MakeResourcesSubdirectoriesAndFilesLowercase(Func<Task<bool>> permissionAsker)
		{
			bool madeChanges = false;

			//we don't need a recursive traversal here since folders in Res directores must not contain subdirectories.
			foreach (var subdir in Directory.GetDirectories(ResourceFolder)) {
				madeChanges |= await RenameToLowercaseAndRemoveAXml (subdir, permissionAsker);
				foreach (var file in Directory.GetFiles(subdir))
					madeChanges |= await RenameToLowercaseAndRemoveAXml (file, permissionAsker);
			}
			return madeChanges;
		}

		private async Task<bool> RenameToLowercaseAndRemoveAXml (string filePath, Func<Task<bool>> permissionAsker)
		{
			var name = Path.GetFileName (filePath);
			bool isFile = !FileExtensions.IsFolder (filePath);
			bool shouldRenameAxml = isFile && ".axml".Equals(Path.GetExtension(filePath), StringComparison.CurrentCultureIgnoreCase);

			/*TODO: rename Resource.ResSubfolder.Something to Resource.ResSubfolder.something everywhere in code*/

			if (name.HasUppercaseChars () || shouldRenameAxml) {
				if (!_grantedPermissionsToChangeMainProject) {
					//so we noticed that xamarin project has some needed directories in upper case, let's aks user to rename them
					if (!await permissionAsker ()) {
						_logger.AppendLog ("Operation cancelled by user");
						throw new OperationCanceledException ("Cancelled by user");
					}
					_grantedPermissionsToChangeMainProject = true;
				}

				if (shouldRenameAxml) {
					_logger.AppendLog ($"Renaming {name} from axml to xml and to lowercase");
					FileExtensions.RenameFileExtensionAndMakeLowercase (filePath, "xml");
					return true;
				} else {
					_logger.AppendLog ($"Renaming {name} to lowercase");
					FileExtensions.RenameFileOrFolderToLowercase (filePath);
					return true;
				}
			}
			return false;
		}
	}
}
