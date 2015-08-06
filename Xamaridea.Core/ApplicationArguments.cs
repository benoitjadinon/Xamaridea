using System;
using System.Linq;
using Xamaridea.Core;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Xamaridea.Core
{

	public class ApplicationArguments
	{
		public string AndroidStudioPath  { get; set; }

		public string AndroidSDKPath { get; set; }

		public string XamarinProjectPath { get; set; }

		public string CustomTemplatePath { get; set; }
	}
}