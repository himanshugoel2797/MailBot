using System;

namespace MailBot
{
	public static class Controller
	{
		public static void MoveCursor(int x, int y)
		{
			switch (PlatformInfo.RunningPlatform ()) {
			case Platform.Linux:
				LinuxController.MoveCursor (x, y);
				break;
			case Platform.Windows:
				WindowsController.MoveCursor (x, y);
				break;
			default:
				throw new PlatformNotSupportedException ();
			}
		}

		public static System.Drawing.Point GetCursor()
		{
			switch (PlatformInfo.RunningPlatform ()) {
			case Platform.Linux:
				return LinuxController.GetCursor ();
			case Platform.Windows:
				return WindowsController.GetCursor ();
			default:
				throw new PlatformNotSupportedException ();
			}
		}

		public static void SendMouseEvent(MouseButtons b, bool doubleClick)
		{
			switch (PlatformInfo.RunningPlatform ()) {
			case Platform.Linux:
				LinuxController.SendMouseEvent(b, doubleClick);
				break;
			case Platform.Windows:
				WindowsController.SendMouseEvent(b, doubleClick);
				break;
			default:
				throw new PlatformNotSupportedException ();
			}
		}
	}
}

