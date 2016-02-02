using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XSharp;

namespace MailBot
{
    class LinuxController
    {
        static XDisplay dsp = new XDisplay(null);
        static XWindow root_window = new XWindow(dsp);

        public static void MoveCursor(int x, int y)
        {
            root_window.SelectInput(XEventMask.KeyReleaseMask);

            XPointer ptr = new XPointer(dsp);

            Point p = GetCursor();
            ptr.Warp(root_window, null, 0, 0, 0, 0, -p.X, -p.Y);
            ptr.Warp(root_window, null, 0, 0, 0, 0, x, y);
            dsp.Flush();
        }

        public static Point GetCursor()
        {
            XPointer ptr = new XPointer(dsp);
            var d = ptr.Query(root_window);
            return new Point(d.root_x, d.root_y);
        }

        public static void SendMouseEvent(MouseButtons mouseButton)
        {
            XButtonEvent ev = new XButtonEvent()
            {
                button = (int)LinuxEnumConverter.E(mouseButton)
            };

            XEvent e = new XEvent(dsp);
        }

    }
}
