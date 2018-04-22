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
	public abstract class BaseAndroidStudioHelper
	{
		readonly string _exePath;
		public string ExePath => _exePath ?? TryFindPath();

		public abstract string SettingsPath { get; }


		protected BaseAndroidStudioHelper(string exePath, ILogger logger)
		{
			_exePath = exePath;
		}


		public bool OpenAndWait(string ideaProjectDir)
		{
			Process p = GetOpenProcess(ideaProjectDir);
			p?.WaitForExit();
			return true;
		}

		protected abstract Process GetOpenProcess(string ideaProjectDir);

		protected abstract string TryFindPath();
	}
}
