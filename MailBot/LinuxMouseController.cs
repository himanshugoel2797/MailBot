using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XSharp;

namespace MailBot
{
    class LinuxMouseController
    {
        public static void MoveCursor(int x, int y)
        {
            XDisplay dsp = new XDisplay(null);
            XWindow root_window = new XWindow(dsp);
            root_window.SelectInput(XEventMask.KeyReleaseMask);

            XPointer ptr = new XPointer(dsp);
            ptr.Warp(root_window, null, 0, 0, 0, 0, x, y);
        }
    }
}
