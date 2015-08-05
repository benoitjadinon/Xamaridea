using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Xamaridea.Core
{
	public static class EnvironmentUtils
	{
		[DllImport ("libc")]
		static extern int uname (IntPtr buf);

		// better not to use Environment.OSVersion.Platform, cause it's returning PlatformID.Unix
		//http://mono.1490590.n4.nabble.com/Why-does-Environment-OSVersion-Platform-report-PlatformID-Unix-on-OS-X-td4038093.html
		public static bool IsRunningOnMac ()
		{
			IntPtr buf = IntPtr.Zero;
			try {
				buf = Marshal.AllocHGlobal (8192);
				if (uname (buf) == 0) {
					string os = Marshal.PtrToStringAnsi (buf);
					if (os == "Darwin")
						return true;
				}
			} finally {
				if (buf != IntPtr.Zero)
					Marshal.FreeHGlobal (buf);
			}
			return false;
		}

		public static bool IsRunningOnWindows ()
		{
			switch (Environment.OSVersion.Platform) {
			case PlatformID.Win32NT:
			case PlatformID.Win32S:
			case PlatformID.Win32Windows:
			case PlatformID.WinCE:
				return true;

			default:
				return false;
			}
		}
	}
}

