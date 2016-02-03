using Gma.System.MouseKeyHook;
using System;
using System.Drawing;

namespace MailBot
{
    public static class Controller
    {
        public static void MoveCursor(int x, int y)
        {
            switch (PlatformInfo.RunningPlatform())
            {
                case Platform.Linux:
                    LinuxController.MoveCursor(x, y);
                    break;
                case Platform.Windows:
                    WindowsController.MoveCursor(x, y);
                    break;
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        public static System.Drawing.Point GetCursor()
        {
            switch (PlatformInfo.RunningPlatform())
            {
                case Platform.Linux:
                    return LinuxController.GetCursor();
                case Platform.Windows:
                    return WindowsController.GetCursor();
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        public static void SendMouseEvent(MouseButtons b, bool down)
        {
            switch (PlatformInfo.RunningPlatform())
            {
                case Platform.Linux:
                    LinuxController.SendMouseEvent(b, down);
                    break;
                case Platform.Windows:
                    WindowsController.SendMouseEvent(b, down);
                    break;
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        public static Point GetResolution()
        {
            switch (PlatformInfo.RunningPlatform())
            {
                case Platform.Linux:
                    return LinuxController.GetResolution();
                case Platform.Windows:
                    return WindowsController.GetResolution();
                default:
                    throw new PlatformNotSupportedException();
            }
        }
    }
}

