using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MailBot
{
    class WindowsController
    {
        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010,
            Wheel = 0x800,
            XDown = 0x80,
            XUp = 0x100
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        private static void SetCursorPosition(int X, int Y)
        {
            SetCursorPos(X, Y);
        }

        private static void SetCursorPosition(Point point)
        {
            SetCursorPos(point.X, point.Y);
        }

        private static Point GetCursorPosition()
        {
            Point currentMousePoint;
            MousePoint c;

            var gotPoint = GetCursorPos(out c);
            if (gotPoint) currentMousePoint = new Point(c.X, c.Y);
            else currentMousePoint = new Point(0, 0);
            return currentMousePoint;
        }

        private static void MouseEvent(MouseEventFlags value, int data = 0)
        {
            Point position = GetCursorPosition();

            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 data,
                 0)
                ;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }

        }

        public static void MoveCursor(int x, int y)
        {
            SetCursorPosition(x, y);
        }

        public static Point GetCursor()
        {
            return GetCursorPosition();
        }

        public static void SendMouseEvent(MouseButtons b, bool down)
        {
            if (b == MouseButtons.Left && down) MouseEvent(MouseEventFlags.LeftDown);
            else if (b == MouseButtons.Left && !down) MouseEvent(MouseEventFlags.LeftUp);
            else if (b == MouseButtons.Middle && down) MouseEvent(MouseEventFlags.MiddleDown);
            else if (b == MouseButtons.Middle && !down) MouseEvent(MouseEventFlags.MiddleUp);
            else if (b == MouseButtons.Right && down) MouseEvent(MouseEventFlags.RightDown);
            else if (b == MouseButtons.Right && !down) MouseEvent(MouseEventFlags.RightUp);
            else if (b == MouseButtons.Back && down) MouseEvent(MouseEventFlags.XDown, 1);
            else if (b == MouseButtons.Back && !down) MouseEvent(MouseEventFlags.XUp, 1);
            else if (b == MouseButtons.ScrollUp) MouseEvent(MouseEventFlags.Wheel, 1);
            else if (b == MouseButtons.ScrollDown) MouseEvent(MouseEventFlags.Wheel, -1);

        }

        public static Point GetResolution()
        {
            return (Point)Screen.PrimaryScreen.Bounds.Size;
        }
    }
}
